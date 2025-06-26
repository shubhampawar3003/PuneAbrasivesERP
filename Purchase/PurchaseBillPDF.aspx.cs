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

public partial class Admin_PurchaseBillPDF : System.Web.UI.Page
{
    CommonCls obj = new CommonCls();
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
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
                if (Request.QueryString["Id"] != null)
                {
                    ////id = Session["PDFID"].ToString();// Decrypt(Request.QueryString["Id"].ToString());
                    string id = obj.Decrypt(Request.QueryString["Id"].ToString());
                    Pdf(id);
                }
            }
        }
    }

    private void Pdf(string id)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PurchaseBill where Id = '" + id + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -11f, 0f);

        string DocName = (Dt.Rows[0]["SupplierName"].ToString() + "-" + Dt.Rows[0]["BillNo"].ToString() + "_PBill.pdf").Replace("/", "_");

        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + DocName, FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();

        //Price Format
        System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en-IN");

        string imageURL = Server.MapPath("~") + "/Content/Img/pune_abrassiv_logo.jpg";

        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //Resize image depend upon your need

        png.ScaleToFit(70, 100);

        //For Image Position
        png.SetAbsolutePosition(40, 755);
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
            string SupplierBillNo = Dt.Rows[0]["SupplierBillNo"].ToString();
            string BillDate = Convert.ToDateTime(Dt.Rows[0]["BillDate"]).ToString("dd-MM-yyyy").Replace("12:00AM", "");
            string AgainstNumber = Dt.Rows[0]["AgainstNumber"].ToString();
            string SupplierName = Dt.Rows[0]["SupplierName"].ToString();
            string TransportMode = Dt.Rows[0]["TransportMode"].ToString();
            string TransportDescription = Dt.Rows[0]["TransportDescription"].ToString();
            string VehicleNo = Dt.Rows[0]["VehicleNo"].ToString();
            string EBillNumber = Dt.Rows[0]["EBillNumber"].ToString();
            string chargeDescription = Dt.Rows[0]["ChargesDescription"].ToString() == "" ? "" : Dt.Rows[0]["ChargesDescription"].ToString();
            string HSNSAC = Dt.Rows[0]["HSNSAC"].ToString() == "" ? "" : Dt.Rows[0]["HSNSAC"].ToString();
            string Rate = Dt.Rows[0]["Rate"].ToString() == "" ? "0" : Dt.Rows[0]["Rate"].ToString();
            string Basic = Dt.Rows[0]["Basic"].ToString() == "" ? "0" : Dt.Rows[0]["Basic"].ToString();
            string CGSTPerr = Dt.Rows[0]["CGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["CGSTPer"].ToString();
            string SGSTPerr = Dt.Rows[0]["SGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["SGSTPer"].ToString();
            string IGSTPerr = Dt.Rows[0]["IGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["IGSTPer"].ToString();
            string Cost = Dt.Rows[0]["Cost"].ToString() == "" ? "0" : Dt.Rows[0]["Cost"].ToString();

            string TransportationCharges = Dt.Rows[0]["TransportationCharges"].ToString() == "" ? "0" : Dt.Rows[0]["TransportationCharges"].ToString();
            string TCGSTPer = Dt.Rows[0]["TCGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TCGSTPer"].ToString();
            string TCGSTAmt = Dt.Rows[0]["TCGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TCGSTAmt"].ToString();
            string TSGSTPer = Dt.Rows[0]["TSGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTPer"].ToString();
            string TSGSTAmt = Dt.Rows[0]["TSGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTAmt"].ToString();
            //string TIGSTAmt = Dt.Rows[0]["TIGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTAmt"].ToString();

            string TIGSTPer = Dt.Rows[0]["TIGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TIGSTPer"].ToString();
            string TIGSTAmt = Dt.Rows[0]["TIGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TIGSTAmt"].ToString();

            string TotalCost = Dt.Rows[0]["TotalCost"].ToString() == "" ? "0" : Dt.Rows[0]["TotalCost"].ToString();

            string DateOfReceived = Dt.Rows[0]["DateOfReceived"].ToString() == "" ? "0" : Dt.Rows[0]["DateOfReceived"].ToString();

            string TCSPer = Dt.Rows[0]["TCSPer"].ToString() == "" ? "0" : Dt.Rows[0]["TCSPer"].ToString();
            string TCSAmount = Dt.Rows[0]["TCSAmount"].ToString() == "" ? "0" : Dt.Rows[0]["TCSAmount"].ToString();
            string Remarks = Dt.Rows[0]["Remarks"].ToString() == "" ? "" : Dt.Rows[0]["Remarks"].ToString();


            //17 march 2022
            string Transporattioncharges = Dt.Rows[0]["TotalCost"].ToString();
            string TGST = Dt.Rows[0]["TCGSTPer"].ToString();
            string TIGST = Dt.Rows[0]["TIGSTPer"].ToString();
            string gstper = "0";
            if (TIGST == "0")
            {
                gstper = TGST.ToString();
            }
            else
            {
                gstper = TIGST.ToString();
            }

            string BillToAddress = "";
            string ShipToAddress = "";
            string StateName = "";
            string GSTNo = "";
            string PANNo = "";
            string EmailID = "";
            string PODate = "";
            string contNo = "";

            double CGSTamt = 0;
            double SGSTamt = 0;
            double IGSTamt = 0;

            SqlDataAdapter add = new SqlDataAdapter("select * from tblPurchaseOrderHdr where PONo='" + AgainstNumber + "'", con);
            DataTable dtt = new DataTable();
            add.Fill(dtt);
            if (dtt.Rows.Count > 0)
            {
                PODate = dtt.Rows[0]["PODate"].ToString().TrimEnd("0:0".ToCharArray());
            }

            SqlDataAdapter ad = new SqlDataAdapter("select * from tbl_VendorMaster where VendorName='" + SupplierName + "'", con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                BillToAddress = dt.Rows[0]["Address"].ToString();
                ShipToAddress = dt.Rows[0]["Address"].ToString();
                StateName = dt.Rows[0]["State"].ToString();
                GSTNo = dt.Rows[0]["GSTNo"].ToString();
                PANNo = dt.Rows[0]["PANNo"].ToString();
                EmailID = dt.Rows[0]["EmailID"].ToString();

            }

            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(28f, 740f, 560f, 80f);
            cb.Stroke();
            // Header 

            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 20);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, SupplierName, 300, 790, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 9);
            cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, BillToAddress, 290, 775, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 9);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 250, 748, 0);
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
            cbbb.Rectangle(28f, 740f, 560f, 25f);
            cbbb.Stroke();
            // Header 
            cbbb.BeginText();
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GSTIN :" + GSTNo + "", 48, 750, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: " + PANNo + "", 170, 750, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "EMAIL : " + EmailID + "", 280, 750, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : " + contNo + "", 455, 750, 0);
            cbbb.EndText();

            PdfContentByte cd = writer.DirectContent;
            cd.Rectangle(28f, 715f, 560f, 25f);
            cd.Stroke();
            // Header 
            cd.BeginText();
            cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
            cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PURCHASE BILL", 270, 722, 0);
            cd.EndText();

            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 1f;

            PdfPTable mtable = new PdfPTable(2);
            mtable.WidthPercentage = 102;
            mtable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 275f;
            table.LockedWidth = true;
            table.HorizontalAlignment = 1;
            table.SetWidths(new float[] { 180f });
            table.AddCell(new Phrase(" Details of Consignee \n\n Pune Abrasives Pvt. Ltd. \n Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk,Near Shell Petrol Pump,Pune - 411019 \n\n GSTIN :27ABCCS7002A1ZW  PAN No: ATFP****J  \n\n State Name:Maharashtra(27) \n  ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(" Details of Shipped to \n\n Pune Abrasives Pvt. Ltd. \n Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk,Near Shell Petrol Pump,Pune - 411019 \n\n GSTIN :27ABCCS7002A1ZW  PAN No: ATFP****J \n\n State Name:Maharashtra(27) \n  ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            mtable.AddCell(table);

            table = new PdfPTable(2);
            float[] widths2 = new float[] { 100, 180 };
            table.SetWidths(widths2);
            table.TotalWidth = 285f;
            table.HorizontalAlignment = 2;
            table.LockedWidth = true;

            var date = DateTime.Now.ToString("yyyy-MM-dd");

            table.AddCell(new Phrase("Bill Number : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(SupplierBillNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Bill Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(BillDate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("PO No : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(AgainstNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("PO Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(PODate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Transportation Mode", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(TransportMode, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Vehicle No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VehicleNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Place of Supply :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(BillToAddress, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Date of Received :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(DateOfReceived, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("E-Bill No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(EBillNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Reverse Charge :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            mtable.AddCell(table);


            paragraphTable1.Add(mtable);
            doc.Add(paragraphTable1);

            PdfPCell tblcell = null;
            Paragraph paragraphTable2 = new Paragraph();
            paragraphTable2.SpacingAfter = 0f;
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                float[] widths3 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f };
                table.SetWidths(widths3);
            }
            else
            {
                table = new PdfPTable(10);
                float[] widths3 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f };
                table.SetWidths(widths3);
            }
            double Ttotal_price = 0;
            double CGST_price = 0;
            double SGST_price = 0;
            double IGST_price = 0;
            if (Dt.Rows.Count > 0)
            {
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                tblcell = new PdfPCell(new Phrase("SN.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Name Of Particulars", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("HSN Code", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Qty", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Rate", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Disc (%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Amount", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                if (Dt.Rows[0]["igstperc"].ToString() == "0")
                {
                    tblcell = new PdfPCell(new Phrase("CGST(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("CGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("SGST(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("SGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                else
                {
                    tblcell = new PdfPCell(new Phrase("IGST(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("IGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                tblcell = new PdfPCell(new Phrase("Total", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);

                int rowid = 1;
                foreach (DataRow dr in Dt.Rows)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;
                    table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;

                    double Ftotal = Convert.ToDouble(dr["grandtot"].ToString());
                    string _ftotal = Ftotal.ToString("##.00");

                    string Description = dr["Particulars"].ToString() + "\n" + dr["Description"].ToString();

                    var amt = dr["Amount"].ToString();
                    var cgstper = dr["cgstperc"].ToString() == "" ? "0" : dr["cgstperc"].ToString();
                    var sgstper = dr["sgstperc"].ToString() == "" ? "0" : dr["sgstperc"].ToString();
                    var igstper = dr["igstperc"].ToString() == "" ? "0" : dr["igstperc"].ToString();

                    var cgstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(cgstper) / 100;
                    var sgstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(sgstper) / 100;
                    var igstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(igstper) / 100;

                    var UOM = dr["UOM"].ToString();

                    tblcell = new PdfPCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Description, FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(dr["HSN"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(dr["Qty"].ToString() + " " + UOM, FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(dr["ratee"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    tblcell = new PdfPCell(new Phrase(dr["Discount"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    tblcell = new PdfPCell(new Phrase(dr["Amount"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    if (Dt.Rows[0]["igstperc"].ToString() == "0")
                    {
                        tblcell = new PdfPCell(new Phrase(Convert.ToDouble(dr["cgstperc"].ToString() == "" ? "0" : dr["cgstperc"].ToString()).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(cgstamt.ToString(), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(Convert.ToDouble(dr["sgstperc"].ToString() == "" ? "0" : dr["sgstperc"].ToString()).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(sgstamt.ToString(), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                    }
                    else
                    {
                        tblcell = new PdfPCell(new Phrase(Convert.ToDouble(dr["igstperc"].ToString() == "" ? "0" : dr["igstperc"].ToString()).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(igstamt.ToString(), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                    }
                    tblcell = new PdfPCell(new Phrase(_ftotal, FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    rowid++;

                    Ttotal_price += Convert.ToDouble(dr["Amount"].ToString());
                    CGST_price += Convert.ToDouble(cgstamt);
                    SGST_price += Convert.ToDouble(sgstamt);
                    IGST_price += Convert.ToDouble(igstamt);
                }

            }

            var Resultamt = Ttotal_price + Convert.ToDouble(Basic) + Convert.ToDouble(TransportationCharges); ;

            string amount = Resultamt.ToString();
            paragraphTable2.Add(table);
            doc.Add(paragraphTable2);

            //var CGSTResAmt = Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["cgstperc"].ToString()) / 100;
            //var SGSTResAmt = Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["sgstperc"].ToString()) / 100;
            //var IGSTResAmt = Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["igstperc"].ToString()) / 100;



            //Space
            Paragraph paragraphTable3 = new Paragraph();

            string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font10 = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            Paragraph paragraph = new Paragraph("", font12);

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f });
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
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                if (Dt.Rows.Count == 1)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 2)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 3)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 4)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                }
                doc.Add(table);
            }
            else
            {
                table = new PdfPTable(10);
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                if (Dt.Rows.Count == 1)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 2)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 3)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 4)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                }
                doc.Add(table);
            }

            //Freight
            Paragraph paragraphTable222 = new Paragraph();
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.TotalWidth = 560f;
                float[] widths33 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f };
                paragraphTable222.SpacingAfter = 0f;
                table.SetWidths(widths33);
            }
            else
            {
                table = new PdfPTable(10);
                table.TotalWidth = 560f;
                float[] widths33 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f };
                paragraphTable222.SpacingAfter = 0f;
                table.SetWidths(widths33);
            }

            if (Dt.Rows.Count > 0)
            {
                table.LockedWidth = true;
                //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(chargeDescription, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(HSNSAC, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(Convert.ToDouble(Basic).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                if (Dt.Rows[0]["igstperc"].ToString() == "0")
                {
                    tblcell = new PdfPCell(new Phrase(Convert.ToDouble(CGSTPerr).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    CGSTamt = Convert.ToDouble(Basic) * Convert.ToDouble(CGSTPerr) / 100;
                    SGSTamt = Convert.ToDouble(Basic) * Convert.ToDouble(SGSTPerr) / 100;

                    tblcell = new PdfPCell(new Phrase(CGSTamt.ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Convert.ToDouble(SGSTPerr).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(SGSTamt.ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                else
                {
                    IGSTamt = Convert.ToDouble(Basic) * Convert.ToDouble(IGSTPerr) / 100;
                    tblcell = new PdfPCell(new Phrase(Convert.ToDouble(IGSTPerr).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    tblcell = new PdfPCell(new Phrase(IGSTamt.ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                tblcell = new PdfPCell(new Phrase(Cost, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
            }
            paragraphTable222.Add(table);
            doc.Add(paragraphTable222);


            /////////////////////////
            Paragraph paragraphTable55 = new Paragraph();

            string[] itemssszx = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font1311 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font1111 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphhhj = new Paragraph();
            //paragraphh.SpacingAfter = 10f;

            for (int i = 0; i < itemssszx.Length; i++)
            {
                paragraphhhj.Add(new Phrase());
            }


            if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f });
            }
            else
            {
                table = new PdfPTable(10);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f });
            }

            table.AddCell(paragraphhhj);

            if (Dt.Rows.Count > 0)
            {
                tblcell = new PdfPCell(new Phrase("Transportation Charge", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);

                var Transporattioncharges_1 = Convert.ToDouble(Dt.Rows[0]["TransportationCharges"].ToString() == "" ? "0" : Dt.Rows[0]["TransportationCharges"].ToString());
                string TCharges = Transporattioncharges_1.ToString("N2", info);

                var TCGSTPer_1 = Convert.ToDouble(Dt.Rows[0]["TCGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TCGSTPer"].ToString());
                string TCGSTPerf = TCGSTPer_1.ToString("N2", info);

                var TSGSTPer_1 = Convert.ToDouble(Dt.Rows[0]["TSGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTPer"].ToString());
                string TSGSTPerf = TSGSTPer_1.ToString("N2", info);

                var TIGSTPer_1 = Convert.ToDouble(Dt.Rows[0]["TIGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TIGSTPer"].ToString());
                string TIGSTPerf = TIGSTPer_1.ToString("N2", info);

                if (Dt.Rows[0]["TIGSTPer"].ToString() == "0")
                {
                    tblcell = new PdfPCell(new Phrase(TCharges + "\n\n" + "", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(TCGSTPerf, FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Dt.Rows[0]["TCGSTAmt"].ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(TSGSTPerf, FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Dt.Rows[0]["TSGSTAmt"].ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                else
                {
                    tblcell = new PdfPCell(new Phrase(TIGSTPerf, FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Dt.Rows[0]["TIGSTAmt"].ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                tblcell = new PdfPCell(new Phrase(Dt.Rows[0]["TotalCost"].ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);

                //table.AddCell(new Phrase("\n        ", FontFactory.GetFont("Arial", 8, Font.BOLD)));
            }


            doc.Add(table);
            ///

            var CGSTResAmt = CGST_price + CGSTamt + Convert.ToDouble(TCGSTAmt); //Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["cgstperc"].ToString()) / 100;
            var SGSTResAmt = SGST_price + SGSTamt + Convert.ToDouble(TSGSTAmt); //Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["sgstperc"].ToString()) / 100;
            var IGSTResAmt = IGST_price + IGSTamt + Convert.ToDouble(TIGSTAmt); //Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["igstperc"].ToString()) / 100;


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

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);
            PdfPCell cell = new PdfPCell(new Phrase("Value of Supply", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);

            var valueTotalf = Convert.ToDouble(amount);

            string ValueTotalamt = valueTotalf.ToString("N2", info);

            PdfPCell cell11 = new PdfPCell(new Phrase(ValueTotalamt, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
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

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);

            decimal Taxamot = 0; decimal Totalamt = 0;
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                PdfPCell cell444 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell444);
                PdfPCell cell555 = new PdfPCell(new Phrase(CGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell555);

                PdfPCell cell4440 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell4440);
                PdfPCell cell5550 = new PdfPCell(new Phrase(CGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell5550);

                doc.Add(table);

                //double Taxamot = CGST_price + SGST_price;
                Taxamot = Convert.ToDecimal(CGSTResAmt + SGSTResAmt);

                Totalamt = Convert.ToDecimal(amount) + Convert.ToDecimal(Taxamot) + Convert.ToDecimal(TCSAmount);

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                paragraph.Alignment = Element.ALIGN_RIGHT;

                table.SetWidths(new float[] { 0f, 119f, 14f });
                table.AddCell(paragraph);
                PdfPCell cell4444 = new PdfPCell(new Phrase("Add SGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell4444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell4444);

                PdfPCell cell5555 = new PdfPCell(new Phrase(SGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell5555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell5555);
                doc.Add(table);

            }
            else
            {
                PdfPCell cell444 = new PdfPCell(new Phrase("Add IGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell444);
                PdfPCell cell555 = new PdfPCell(new Phrase(IGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell555);

                //PdfPCell cell4440 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                //cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                //table.AddCell(cell4440);
                //PdfPCell cell5550 = new PdfPCell(new Phrase(CGSTResAmt.ToString(), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                //cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                //table.AddCell(cell5550);

                doc.Add(table);

                //double Taxamot = CGST_price + SGST_price;
                Taxamot = Convert.ToDecimal(IGSTResAmt);

                Totalamt = Convert.ToDecimal(amount) + Convert.ToDecimal(Taxamot) + Convert.ToDecimal(TCSAmount);
            }
            ///

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);
            PdfPCell cellTaxAmount = new PdfPCell(new Phrase("Tax Amount", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTaxAmount.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTaxAmount);

            PdfPCell cellTaxAmount1 = new PdfPCell(new Phrase(Taxamot.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTaxAmount1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTaxAmount1);
            doc.Add(table);

            //TCS
            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);
            PdfPCell cellTCharges = new PdfPCell(new Phrase("TCS [ " + TCSPer + " % ]", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTCharges.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTCharges);

            PdfPCell cellTCharges1 = new PdfPCell(new Phrase(TCSAmount, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTCharges1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTCharges1);
            doc.Add(table);

            //////////////////////////////

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);

            PdfPCell cell44 = new PdfPCell(new Phrase("Total Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell44.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell44);

            //decimal FinalTot = Math.Round(Totalamt);

            var Totalamtf = Convert.ToDouble(Totalamt);
            var Totalamtfff = Math.Round(Totalamtf);
            string FinaleTotalamt = Totalamtfff.ToString("N2", info);

            //var FTotalf = Convert.ToDouble(FinalTot);
            //string FTotalamt = FTotalf.ToString("N2", info);

            PdfPCell cell440 = new PdfPCell(new Phrase(FinaleTotalamt, FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell440.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell440);
            doc.Add(table);

            string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Totalamtf));

            //Total amount In word
            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 0f });
            table.AddCell(paragraph);

            PdfPCell cell4434 = new PdfPCell(new Phrase("Total Amount: " + Amtinword + " Only" + "\n\n" + "Remarks :" + Remarks, FontFactory.GetFont("Arial", 9, Font.BOLD)));
            cell4434.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell4434);

            //var finaleTotalf = Convert.ToDouble(FinalTot);
            //string FinaleTotalamt = finaleTotalf.ToString("N2", info);

            PdfPCell cell44044 = new PdfPCell(new Phrase(FinaleTotalamt + "\n\n" + "Remarks :" + Remarks, FontFactory.GetFont("Arial", 9, Font.BOLD)));
            cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell44044);
            doc.Add(table);

            Paragraph paragraphTable99 = new Paragraph(" Remarks :\n\n", font12);

            //Puja Enterprises Sign
            string[] itemss = {
                "Declaration  : I/We hereby certify that my/our registration certificate under the GST Act, 2017 is in force on the date on which the supply of the goods specified in this tax invoice is made by me/us and that the transaction of supplies covered by this tax invoice has been effected by me/us and it shall be accounted for in the turnover of supplies while filing of return and the due tax, if any, payable on the supplies has been paid or shall be paid.",
                " \n",
                        };

            Font font14 = FontFactory.GetFont("Arial", 11);
            Font font15 = FontFactory.GetFont("Arial", 9, Font.NORMAL);
            Font fontBold = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphhh = new Paragraph(" Terms & Condition :\n\n", font10);


            for (int i = 0; i < itemss.Length; i++)
            {
                //paragraphhh.Add(new Phrase("\u2022 \u00a0" + itemss[i] + "\n", font15));
                paragraphhh.Add(new Phrase(itemss[i] + "\n", font15));
            }

            table = new PdfPTable(1);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 560f });

            table.AddCell(paragraphhh);
            //table.AddCell(new Phrase("Puja Enterprises \n\n\n\n         Sign", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            //doc.Add(table);

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
            table.AddCell(paragraphhh);
            table.AddCell(new Phrase("" + SupplierName + " \n\n\n\n\n\n\nAuthorised Signature", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            doc.Add(table);
            doc.Close();


            Byte[] FileBuffer = File.ReadAllBytes(Server.MapPath("~/PDF_Files/") + DocName);

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
            ifrRight6.Attributes["src"] = @"../PDF_Files/" + DocName;
        }
        doc.Close();
        //Session["PDFID"] = null;
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