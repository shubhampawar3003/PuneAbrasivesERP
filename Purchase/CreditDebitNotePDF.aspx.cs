using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Net.Mail;
using iTextSharp.text.pdf;
using System.Globalization;
using System.Threading.Tasks;
using iTextSharp.text;
using System.Collections;

public partial class Purchase_CreditDebitNotePDF : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserCode"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                Pdf();
            }
        }
    }

    private void Pdf()
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_CreditDebitNote where Id = '" + Session["PDFID"].ToString() + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 10f, 10f, 55f, 0f);

        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "CreditDebitNote.pdf", FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();

        string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";

        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //Resize image depend upon your need

        png.ScaleToFit(70, 100);

        //For Image Position
        png.SetAbsolutePosition(40, 718);
        //var document = new Document();

        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;

        //Give some space after the image

        png.SpacingAfter = 1f;

        png.Alignment = Element.ALIGN_LEFT;

        //paragraphimage.Add(png);
        //doc.Add(paragraphimage);
        doc.Add(png);

        if (Dt.Rows.Count > 0)
        {
            var CreateDate = DateTime.Now.ToString("yyyy-MM-dd");
            string DocNo = Dt.Rows[0]["DocNo"].ToString();
            string NoteType = Dt.Rows[0]["NoteType"].ToString();
            string CategoryName = Dt.Rows[0]["CategoryName"].ToString();
            string DocDate = Dt.Rows[0]["DocDate"].ToString().TrimEnd("0:0".ToCharArray());
            string BillDate = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
            string BillNumber = Dt.Rows[0]["BillNumber"].ToString();
            string SupplierName = Dt.Rows[0]["SupplierName"].ToString();
            string Remarks = Dt.Rows[0]["Remarks"].ToString();
            string Grandtotal = Dt.Rows[0]["Grandtotal"].ToString();

            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(17f, 710f, 560f, 60f);
            cb.Stroke();
            // Header 
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Excel Enclosure", 250, 745, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Gat No. 1567, Shelar Vasti, Dehu-Alandi Road, Chikhali, Pune - 411062", 145, 728, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
            cb.EndText();

            //PdfContentByte cbb = writer.DirectContent;
            //cbb.Rectangle(17f, 710f, 560f, 25f);
            //cbb.Stroke();
            //// Header 
            //cbb.BeginText();
            //cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " CONTACT : 9225658662   Email ID : mktg@excelenclosures.com", 153, 722, 0);
            //cbb.EndText();

            PdfContentByte cbbb = writer.DirectContent;
            cbbb.Rectangle(17f, 685f, 560f, 25f);
            cbbb.Stroke();
            // Header 
            cbbb.BeginText();
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GSTIN : 27ATFPS1959J1Z4", 30, 695, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: ATFPS1959J", 160, 695, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "EMAIL : mktg@excelenclosures.com", 270, 695, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : 9225658662", 440, 695, 0);
            cbbb.EndText();

            PdfContentByte cd = writer.DirectContent;
            cd.Rectangle(17f, 660f, 560f, 25f);
            cd.Stroke();
            // Header 
            cd.BeginText();
            cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
            cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, NoteType + " Note", 260, 667, 0);
            cd.EndText();

            string BillToAddress = "";
            string ShipToAddress = "";
            string StateName = "";
            string GSTNo = "";
            string PANNo = "";

            SqlCommand cmdsum = new SqlCommand("select * from tblSupplierMaster where SupplierName", con);
            SqlDataAdapter ad = new SqlDataAdapter("select * from tblSupplierMaster where SupplierName='" + SupplierName + "'", con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                BillToAddress = dt.Rows[0]["BillToAddress"].ToString();
                ShipToAddress = dt.Rows[0]["ShipToAddress"].ToString();
                StateName = dt.Rows[0]["StateName"].ToString();
                GSTNo = dt.Rows[0]["GSTNo"].ToString();
                PANNo = dt.Rows[0]["PANNo"].ToString();
            }

            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 10f;

            PdfPTable table = new PdfPTable(4);

            float[] widths2 = new float[] { 100, 180, 100, 180 };
            table.SetWidths(widths2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            var date = DateTime.Now.ToString("yyyy-MM-dd");


            table.AddCell(new Phrase("Document Number : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(DocNo, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("Document Date :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(DocDate, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("Bill No : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(BillNumber, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("Bill Date :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(BillDate, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("Customer Debit Note For", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(CategoryName, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("Supplier Details", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(SupplierName + "\n\nAddress: " + BillToAddress + "\n\nState: " + StateName, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            table.AddCell(new Phrase("PAN: " + PANNo, FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("GSTIN :" + GSTNo, FontFactory.GetFont("Arial", 9, Font.BOLD)));

            paragraphTable1.Add(table);
            doc.Add(paragraphTable1);

            Paragraph paragraphTable2 = new Paragraph();
            paragraphTable2.SpacingAfter = 0f;
            table = new PdfPTable(11);
            float[] widths3 = new float[] { 4f, 40f, 11f, 6f, 10f, 12f, 8f, 10f, 8f, 10f, 15f };
            table.SetWidths(widths3);

            double Ttotal_price = 0;
            double CGST_price = 0;
            double SGST_price = 0;
            if (Dt.Rows.Count > 0)
            {
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.AddCell(new Phrase("SN.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Name Of Particulars", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Hsn/Sac", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Qty", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Rate", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("CGST(%)", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("CGST Amt", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("SGST(%)", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("SGST Amt", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Total", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                int rowid = 1;
                foreach (DataRow dr in Dt.Rows)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;

                    double Ftotal = Convert.ToDouble(dr["Total"].ToString());
                    string _ftotal = Ftotal.ToString("##.00");
                    table.AddCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["Particulars"].ToString() + "\n" + dr["Description"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["HSN"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["Qty"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["Rate"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["Amount"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["CGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["CGSTAmt"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["SGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["SGSTAmt"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(_ftotal, FontFactory.GetFont("Arial", 9)));
                    rowid++;

                    Ttotal_price += Convert.ToDouble(dr["Amount"].ToString());
                    CGST_price += Convert.ToDouble(dr["CGSTAmt"].ToString());
                    SGST_price += Convert.ToDouble(dr["SGSTAmt"].ToString());
                }

            }
            string amount = Ttotal_price.ToString();
            paragraphTable2.Add(table);
            doc.Add(paragraphTable2);

            //Space
            Paragraph paragraphTable3 = new Paragraph();

            string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font10 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraph = new Paragraph("", font12);

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }

            table = new PdfPTable(11);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 4f, 40f, 11f, 6f, 10f, 12f, 8f, 10f, 8f, 10f, 15f });
            table.AddCell(paragraph);
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

            doc.Add(table);

            //Add Total Row start
            Paragraph paragraphTable5 = new Paragraph();

            string[] itemsss = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font13 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font11 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphh = new Paragraph("", font12);



            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 15f });
            table.AddCell(paragraph);
            PdfPCell cell = new PdfPCell(new Phrase("Value of Supply", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);
            PdfPCell cell11 = new PdfPCell(new Phrase(amount, FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell11.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell11);
            doc.Add(table);



            //Grand total Row STart
            Paragraph paragraphTable17 = new Paragraph();
            paragraphTable5.SpacingAfter = 0f;

            string[] itemm = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font16 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font17 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphhhhh = new Paragraph("", font12);

            //paragraphh.SpacingAfter = 10f;

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            table.SetWidths(new float[] { 0f, 119f, 15f });
            table.AddCell(paragraph);

            PdfPCell cell444 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell444);
            PdfPCell cell555 = new PdfPCell(new Phrase(CGST_price.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell555);


            PdfPCell cell4440 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell4440);
            PdfPCell cell5550 = new PdfPCell(new Phrase(CGST_price.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell5550);

            doc.Add(table);

            double Taxamot = CGST_price + SGST_price;

            var Totalamt = Convert.ToDecimal(amount) + Convert.ToDecimal(Taxamot);

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 15f });
            table.AddCell(paragraph);
            PdfPCell cell4444 = new PdfPCell(new Phrase("Add SGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell4444.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell4444);

            PdfPCell cell5555 = new PdfPCell(new Phrase(SGST_price.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell5555.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell5555);
            doc.Add(table);

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 15f });
            table.AddCell(paragraph);
            PdfPCell cellTaxAmount = new PdfPCell(new Phrase("Tax Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cellTaxAmount.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTaxAmount);

            PdfPCell cellTaxAmount1 = new PdfPCell(new Phrase(Taxamot.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cellTaxAmount1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTaxAmount1);
            doc.Add(table);

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 15f });
            table.AddCell(paragraph);

            PdfPCell cell44 = new PdfPCell(new Phrase("Total Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell44.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell44);

            PdfPCell cell440 = new PdfPCell(new Phrase(Totalamt.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell440.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell440);
            doc.Add(table);

            string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Totalamt));

            //Total amount In word
            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 0f });
            table.AddCell(paragraph);

            PdfPCell cell4434 = new PdfPCell(new Phrase("Total Amount: " + Amtinword + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            cell4434.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell4434);

            PdfPCell cell44044 = new PdfPCell(new Phrase(Totalamt.ToString(), FontFactory.GetFont("Arial", 9, Font.BOLD)));
            cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell44044);
            doc.Add(table);

            Paragraph paragraphTable99 = new Paragraph(" Remarks :\n\n", font12);

            //Puja Enterprises Sign
            string[] itemss = {
                "REMARKS      :    "+Remarks+"\n\n\n",
                "Declaration  : I/We hereby certify that my/our registration certificate under the GST Act, 2017 is in force on the \n date on which the supply of the goods specified in this tax invoice is made by me/us and that the transaction \n of supplies covered by this tax invoice has been effected by me/us and it shall be accounted for in the \n turnover of supplies while filing of return and the due tax, if any, payable on the supplies has been paid or \n shall be paid.\n",
                        };

            Font font14 = FontFactory.GetFont("Arial", 11);
            Font font15 = FontFactory.GetFont("Arial", 10);
            Paragraph paragraphhh = new Paragraph("Term & Condition\n\n", font10);


            for (int i = 0; i < itemss.Length; i++)
            {
                paragraphhh.Add(new Phrase("\u2022 \u00a0" + itemss[i] + "\n", font15));
            }

            table = new PdfPTable(1);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 560f });

            table.AddCell(paragraphhh);
            //table.AddCell(new Phrase("Puja Enterprises \n\n\n\n         Sign", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            doc.Add(table);

            Paragraph paragraphTable10000 = new Paragraph();

            //Puja Enterprises Sign
            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();


            //for (int i = 0; i < itemss4.Length; i++)
            //{
            //    paragraphhhhhff.Add(new Phrase("\u2022 \u00a0" + itemss4[i] + "\n", font155));
            //}

            table = new PdfPTable(2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 300f, 100f });

            //table.AddCell(paragraphhhhhff);
            table.AddCell(new Phrase(" ", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("         For Excel Enclosures \n\n\n\n\n\n         Authorised Signature", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            doc.Add(table);
            doc.Close();


            Byte[] FileBuffer = File.ReadAllBytes(Server.MapPath("~/files/") + "CreditDebitNote.pdf");

            Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(FileBuffer);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        if (i == 1)
                        {

                        }
                        else
                        {
                            var pdfbyte = stamper.GetOverContent(i);
                            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageURL);
                            image.ScaleToFit(70, 100);
                            image.SetAbsolutePosition(40, 792);
                            image.SpacingBefore = 50f;
                            image.SpacingAfter = 1f;
                            image.Alignment = Element.ALIGN_LEFT;
                            pdfbyte.AddImage(image);
                        }
                        var PageName = "Page No. " + i.ToString();
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(PageName, blackFont), 568f, 820f, 0);
                    }
                }
                FileBuffer = stream.ToArray();
            }


            //string empFilename = QuatationNumber + " " + PartyName + ".pdf";

            //if (FileBuffer != null)
            //{
            //    Response.ContentType = "application/pdf";
            //    Response.AddHeader("content-length", FileBuffer.Length.ToString());
            //    Response.BinaryWrite(FileBuffer);
            //    Response.AddHeader("Content-Disposition", "attachment;filename=" + empFilename);
            //}
            ifrRight6.Attributes["src"] = @"../files/" + "CreditDebitNote.pdf";
           
        }
        doc.Close();
        Session["PDFID"] = null;
    }


    public static string ConvertNumbertoWords(int number)
    {
        if (number == 0)
            return "ZERO";
        if (number < 0)
            return "minus " + ConvertNumbertoWords(Math.Abs(number));
        string words = "";
        if ((number / 1000000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000000) + " MILLION ";
            number %= 1000000;
        }
        if ((number / 1000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
            number %= 1000;
        }
        if ((number / 100) > 0)
        {
            words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
            number %= 100;
        }
        if (number > 0)
        {
            if (words != "")
                words += "AND ";
            var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
            var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
        }
        return words;
    }

}