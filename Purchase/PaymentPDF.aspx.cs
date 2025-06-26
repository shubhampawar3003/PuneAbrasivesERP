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
using System.Net;
using iTextSharp.text.html.simpleparser;
using Image = iTextSharp.text.Image;

public partial class Purchase_PaymentPDF : System.Web.UI.Page
{
    string id;
    CommonCls obj = new CommonCls();
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["Id"] != null)
            {
                id = obj.Decrypt(Request.QueryString["Id"].ToString());
                Pdf(id);
            }
        }
    }

    protected void Pdf(string id)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PaymentPDF where Id = '" + id + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
        //string Path = ;
        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + "Payment.pdf", FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();
        string imageURL = Server.MapPath("~") + "/Content/Img/pune_abrassiv_logo.jpg";


        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //Resize image depend upon your need

        png.ScaleToFit(70, 100);

        //For Image Position
        png.SetAbsolutePosition(40, 775);
        //var document = new Document();

        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;

        //Give some space after the image

        png.SpacingAfter = 1f;

        png.Alignment = Element.ALIGN_LEFT;


        doc.Add(png);

        if (Dt.Rows.Count > 0)
        {
            var fromtime = Convert.ToDateTime(Dt.Rows[0]["CreatedOn"].ToString());
			
            var VoucherDate = fromtime.ToString("dd-MM-yyyy");//DateTime.Now.ToString("dd-MM-yyyy");
            string VoucherNo = Dt.Rows[0]["Id"].ToString();
            string chequedate = Dt.Rows[0]["postDate"].ToString().TrimEnd("0:0".ToCharArray());
            string bankName = Dt.Rows[0]["BankName"].ToString();
            string TransactionMode = Dt.Rows[0]["TransactionMode"].ToString();
            string ModeDiscription = Dt.Rows[0]["Modedescription"].ToString();
            string Paidto = Dt.Rows[0]["Partyname"].ToString();
            string Remark = Dt.Rows[0]["TransactionRemark"].ToString();
            string Paid = Dt.Rows[0]["Paid"].ToString();
            string AgainstNumber = Dt.Rows[0]["BillNo"].ToString();
            string Invoicedates = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
            string TDS = Dt.Rows[0]["TDS"].ToString();
            string Adjusted = Dt.Rows[0]["Adjust"].ToString();
            string Excess = Dt.Rows[0]["Excess"].ToString();
            string Notes = Dt.Rows[0]["Note"].ToString();
            string Total = Dt.Rows[0]["Total"].ToString();
            string Amount = Dt.Rows[0]["Amount"].ToString();

            string Against = Dt.Rows[0]["Against"].ToString();
			
			string EmailID = BindEmailId(Paidto);

            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(28f, -100f, 560f, 80f);

            cb.Stroke();
            // Header 
            cb.BeginText();
           
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Pune Abrasives Pvt. Ltd.", 200, 800, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk,Near Shell Petrol Pump,Pune - 411019", 110, 785, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Mb No : +91 9860441689, 9511712429    EMAIL : girish.kulkarni@puneabrasives.com  ", 158, 772, 0);
         //   cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "girish.kulkarni@puneabrasives.com ", 357, 762, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
            cb.EndText();


            //PdfContentByte cbbb = writer.DirectContent;
            //cbbb.Rectangle(28f, 0f, 560f, 25f);
            //cbbb.Stroke();
            ////Header
            //cbbb.BeginText();
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GSTIN :27ATFPS1959J1Z4" + "", 48, 765, 0);
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: ATFPS1959J" + "", 170, 765, 0);
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "MAHARASHTRA STATE GST CODE : 27" + "", 280, 765, 0);
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : 9225658662", 455, 765, 0);
            //cbbb.EndText();

            //Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            //doc.Add(p);


            PdfContentByte cd = writer.DirectContent;
            cd.Rectangle(28f, 760f, 560f, 0f);
            cd.Stroke();
            // Header 
            cd.BeginText();
            cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
            cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAYMENT VOUCHER", 250, 739, 0);
            cd.EndText();


            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 20f;

            PdfPTable table = new PdfPTable(4);

            float[] widths2 = new float[] { 100, 240, 100, 100 };
            table.SetWidths(widths2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;


            //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            //string datee = ffff1.ToString("yyyy-MM-dd");
            //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.DefaultCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("Voucher Number : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VoucherNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Voucher Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VoucherDate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Bank", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(bankName, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Cheque Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(chequedate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(TransactionMode + " :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(ModeDiscription, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Amount :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("Rs. " + Convert.ToDouble(Amount).ToString("##.00"), FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Paid From :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(Paidto, FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("Paid Against :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("Email ID :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(EmailID, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
 
            table.AddCell(new Phrase("Remark :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(Remark, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));


            paragraphTable1.Add(table);
            doc.Add(paragraphTable1);

            if (Against == "Invoice Bill")
            {
                ///Description Table

                Paragraph paragraphTable2 = new Paragraph();
                paragraphTable2.SpacingAfter = 0f;
                table = new PdfPTable(8);
                float[] widths33 = new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f };
                table.SetWidths(widths33);

                decimal SumOfTotal = 0;
                decimal Paidval = 0;
                decimal ExcessVal = 0;
                decimal TDSs = 0;
                decimal Adjust = 0;
                if (Dt.Rows.Count > 0)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;

                    table.AddCell(new Phrase("SN.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Invoice No.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Invoice Date", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Paid", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("TDS", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Adjusted", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Excess", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Note", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                    int rowid = 1;
                    foreach (DataRow dr in Dt.Rows)
                    {
                        table.TotalWidth = 560f;
                        table.LockedWidth = true;
                        //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        table.AddCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["BillNo"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["BillDate"].ToString().TrimEnd("0:0".ToCharArray()), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Paid"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["TDS"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Adjust"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Excess"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Note"].ToString(), FontFactory.GetFont("Arial", 9)));
                        rowid++;
                        //SumOfTotal += Convert.ToDecimal(dr["Total"].ToString());
                        Paidval += Convert.ToDecimal(dr["Paid"].ToString());
                        TDSs += Convert.ToDecimal(dr["TDS"].ToString());
                        Adjust += Convert.ToDecimal(dr["Adjust"].ToString());
                        ExcessVal += Convert.ToDecimal(dr["Excess"].ToString());
                    }
                }

                paragraphTable2.Add(table);
                doc.Add(paragraphTable2);
                ///End Description table
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

                table = new PdfPTable(8);
                //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Total", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(Paidval.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(TDSs.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(Adjust.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(ExcessVal.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));


                doc.Add(table);



                Paragraph paragraphTable5 = new Paragraph();
                string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Paidval));
                //Total amount In word
                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.NO_BORDER;

                table.SetWidths(new float[] { 0f, 199f, 0f });
                table.AddCell(paragraphTable5);
                PdfPCell cell44345 = new PdfPCell(new Phrase("The Sum Of Rupees: " + Amtinword + " Only.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                cell44345.HorizontalAlignment = Element.ALIGN_LEFT;

                table.AddCell(cell44345);
                PdfPCell cell44044 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell44044);
                doc.Add(table);
            }
            //Authorization
            Paragraph paragraphTable10000 = new Paragraph();


            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();
            table = new PdfPTable(2);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.SpacingBefore = 30f;
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 150f, 150f });


            // Bind stamp Image
            //string imageStamp = Server.MapPath("~") + "/Content/img/Account.png";
            //iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imageStamp);
            //image1.ScaleToFit(800, 120);
            //PdfPCell imageCell = new PdfPCell(image1);
            //imageCell.PaddingLeft = 140f;
            //imageCell.PaddingTop = 0f;
            //imageCell.Border = Rectangle.NO_BORDER;
            /////////////////

            //table.AddCell(paragraphhhhhff);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.AddCell(new Phrase("                              \n\n\n\n\n\n\n\n\n\n        Receiver Signature", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
           // table.AddCell(imageCell);

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            doc.Add(table);
            //doc.Close();
            ///end Sign Authorization
            //End Authorization

        }
        doc.Close();
        string filePath = @Server.MapPath("~/PDF_Files/") + "Payment.pdf";
        Response.ContentType = "Receipt.pdf";
        Response.WriteFile(filePath);
    }
	
	 public string BindEmailId(string SupplierName)
    {
        string EmailID = "";
        SqlDataAdapter ad = new SqlDataAdapter("SELECT [EmailID] FROM [tbl_VendorMaster] where VendorName='" + SupplierName.Trim() + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            return EmailID = dt.Rows[0]["EmailID"].ToString() == "" ? "Email Id not found" : dt.Rows[0]["EmailID"].ToString();
        }
        else
        {
            return EmailID = "Email Id not found";
        }
    }

    public static string ConvertNumbertoWords(int numbers)
    {
        Boolean paisaconversion = false;
        var pointindex = numbers.ToString().IndexOf(".");
        var paisaamt = 0;
        if (pointindex > 0)
            paisaamt = Convert.ToInt32(numbers.ToString().Substring(pointindex + 1, 2));

        int number = Convert.ToInt32(numbers);

        if (number == 0) return "Zero";
        if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
        int[] num = new int[4];
        int first = 0;
        int u, h, t;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (number < 0)
        {
            sb.Append("Minus ");
            number = -number;
        }
        string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
        string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
        string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
        string[] words3 = { "Thousand ", "Lakh ", "Crore " };
        num[0] = number % 1000; // units
        num[1] = number / 1000;
        num[2] = number / 100000;
        num[1] = num[1] - 100 * num[2]; // thousands
        num[3] = number / 10000000; // crores
        num[2] = num[2] - 100 * num[3]; // lakhs
        for (int i = 3; i > 0; i--)
        {
            if (num[i] != 0)
            {
                first = i;
                break;
            }
        }
        for (int i = first; i >= 0; i--)
        {
            if (num[i] == 0) continue;
            u = num[i] % 10; // ones
            t = num[i] / 10;
            h = num[i] / 100; // hundreds
            t = t - 10 * h; // tens
            if (h > 0) sb.Append(words0[h] + "Hundred ");
            if (u > 0 || t > 0)
            {
                if (h > 0 || i == 0) sb.Append("and ");
                if (t == 0)
                    sb.Append(words0[u]);
                else if (t == 1)
                    sb.Append(words1[u]);
                else
                    sb.Append(words2[t - 2] + words0[u]);
            }
            if (i != 0) sb.Append(words3[i - 1]);
        }

        if (paisaamt == 0 && paisaconversion == false)
        {
            sb.Append("Rupees ");
        }
        else if (paisaamt > 0)
        {
            var paisatext = ConvertNumbertoWords(paisaamt);
            sb.AppendFormat("Rupees {0} paise", paisatext);
        }
        return sb.ToString().TrimEnd();
    }
}