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
using iTextSharp.text;
using ZXing.Common;
using ZXing;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Newtonsoft.Json;

public partial class Gov_Bills_EwayBill_CrDbNote : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

    private readonly DateTimeFormatInfo UkDtfi = new CultureInfo("en-GB", false).DateTimeFormat;

    private SqlCommand Cmd;
    public string AuthToken = "";
    string id = "";

    string IPAddress = ConfigurationManager.AppSettings["IPAddress"].ToString();
    string MailID = ConfigurationManager.AppSettings["EmailID"].ToString();
    //E-Way Bill Credentials
    public string E_Way_Client_ID = ConfigurationManager.AppSettings["EWayBillClientID"].ToString();
    public string E_Way_Secret = ConfigurationManager.AppSettings["EWayBillSecretCode"].ToString();
    string UserName = ConfigurationManager.AppSettings["EUserName"].ToString();
    string Password = ConfigurationManager.AppSettings["EPassword"].ToString();
    string GST = ConfigurationManager.AppSettings["EGST"].ToString();

    //E-Invoice  Credentials
    string E_Invoice_API_Client_ID = ConfigurationManager.AppSettings["EInvoiceClientID"].ToString();
    string E_Invoice_API_Secret = ConfigurationManager.AppSettings["EInvoiceSecretCode"].ToString();


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {   //for Generation
            if (Request.QueryString["Id"] != null)
            {
                divGenerateEwayBill.Visible = true;
                divCancelEwayBill.Visible = false;
                con.Open();
                id = Decrypt(Request.QueryString["Id"].ToString());
                Load_EinvData(id);
            }
            //for cancellation
            if (Request.QueryString["CnlId"] != null)
            {
                divGenerateEwayBill.Visible = false;
                divCancelEwayBill.Visible = true;
                con.Open();
                id = Decrypt(Request.QueryString["CnlId"].ToString());
                Load_E_WayBillData(id);
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    private void Load_EinvData(string id)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_CrDbNotePDF where Id = '" + id + "'", con);
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            lblIrn.Text = Dt.Rows[0]["IRN"].ToString();
            lblHiddenid.Text = Dt.Rows[0]["Id"].ToString();
            lblAckNo.Text = Dt.Rows[0]["AckNo"].ToString();
            lblAckDate.Text = Dt.Rows[0]["AckDt"].ToString();
            lblInvoiceNo.Text = Dt.Rows[0]["DocNo"].ToString();
            string invdate = Dt.Rows[0]["DocDate"].ToString();
            lblInvoiceDate.Text = invdate.Replace("-", "/"); ;
            txtdate.Text = DateTime.Today.ToString("dd-MM-yyyy");
            string TransportMode = Dt.Rows[0]["TransportMode"].ToString();
            if (TransportMode == "By Road")
            {
                ddlTransportationMode.SelectedValue = "1";
                string Vehicleno = Dt.Rows[0]["VehicalNo"].ToString();
                txtVehicleNumber.Text = Vehicleno.Replace(" ", ""); ;
                txtVehicleNumber.Enabled = false;
                ddlVehicleType.SelectedValue = "R";
                lblTransporterDocNo.Text = "Transporter Doc. No";
            }
            else if (TransportMode == "By Air")
            {
                ddlVehicleType.SelectedValue = "R";
                txtTransporterName.Text = "Air";
                txtVehicleNumber.Enabled = false;
                lblTransporterDocNo.Text = "Airway Bill No.";
            }
            GetPDFDetails();
        }
    }

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("EWayBillList_CrDbNote.aspx");
    }

    protected void txtTransporterID_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GenrateGSTDtls(UserName, Password, GST, txtTransporterID.Text);
            con.Open();
            GetPDFDetails();
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void Pdf()
    {
        string id = Decrypt(Request.QueryString["Id"].ToString());

        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_CrDbNotePDF where Id = '" + id + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());
        string billingCustomer = Dt.Rows[0]["SupplierName"].ToString();
        //05-06-2022
        string InvoiceNo = Dt.Rows[0]["DocNo"].ToString();
        string stringInvoiceNo = InvoiceNo.Replace("/", "-");

        Document doc = new Document(PageSize.A4, 30f, 10f, 30f, 0f);


        //Document doc = new Document(PageSize.A4, 10f,);
        //string Path = ;
        string Docname = stringInvoiceNo + "_EInvoice2.pdf";
        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + Docname, FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        #region Pdf For Oroginal 
        doc.Open();
        //string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";
        string imageURL = Server.MapPath("~/E_Inv_QrCOde/QR_Img_CrDbNote.jpg");
        string imagecancel = Server.MapPath("~/Content/img/CancelInvoice.png");  //grey
                                                                         //string imagecancel = Server.MapPath("~/img/CancelInvoice_Red.png");  //red

        //Price Format
        System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en-IN");

        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
        iTextSharp.text.Image pngcancel = iTextSharp.text.Image.GetInstance(imagecancel);

        //Resize image depend upon your need

        png.ScaleToFit(130, 130);
        pngcancel.ScaleToFit(350, 350);

        //For Image Position
        //png.SetAbsolutePosition(40, 750);
        png.SetAbsolutePosition(450, 685);
        pngcancel.SetAbsolutePosition(115, 350);
        //var document = new Document();

        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;
        pngcancel.SpacingBefore = 50f;

        //Give some space after the image

        png.SpacingAfter = 1f;
        pngcancel.SpacingAfter = 1f;

        png.Alignment = Element.ALIGN_LEFT;
        pngcancel.Alignment = Element.ALIGN_LEFT;


        doc.Add(png);
        string cancelstatus = Dt.Rows[0]["e_invoice_cancel_status"].ToString();
        if (cancelstatus == true.ToString())
        {
            doc.Add(pngcancel);
        }
        else
        {

        }



        if (Dt.Rows.Count > 0)
        {
            var CreateDate = DateTime.Now.ToString("yyyy-MM-dd");

            string invoicedate = Dt.Rows[0]["DocNo"].ToString().TrimEnd("0:0".ToCharArray());
            //string AckDt = Dt.Rows[0]["AckDt"].ToString().TrimEnd("0:0".ToCharArray());
            string IRN = Dt.Rows[0]["Irn"].ToString();
            string AckDt = Dt.Rows[0]["AckDt"].ToString();

            //DateTime parsedDate = DateTime.ParseExact(AckDt, "dd/MM/yyyy HH:mm:ss", null);
            //string F_AckDt1 = parsedDate.ToString("dd-MM-yyyy HH:mm:ss");
            //
            DateTime parsedDate1 = Convert.ToDateTime(Dt.Rows[0]["AckDt"].ToString());
            string F_AckDt = parsedDate1.ToString("dd-MM-yyyy HH:mm:ss");
            //
            string AckNo = Dt.Rows[0]["AckNo"].ToString();
            string customerPoNo = Dt.Rows[0]["BillNumber"].ToString();
            //string ChallanNo = Dt.Rows[0]["BillDate"].ToString();
            string PODate = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
            //string ChallanDate = Dt.Rows[0]["ChallanDate"].ToString().TrimEnd("0:0".ToCharArray());
            //string EBillNo = Dt.Rows[0]["E_BillNo"].ToString();
            string transactionmode = Dt.Rows[0]["TransportMode"].ToString();
            string vehicalNo = Dt.Rows[0]["VehicalNo"].ToString();
            string placeOfSupply = Dt.Rows[0]["ShippingAddress"].ToString();
            string dateOfSupply = Dt.Rows[0]["DocDate"].ToString();

            string ShippingCustomer = Dt.Rows[0]["ShippingCustomer"].ToString();
            string ShippingAddress = Dt.Rows[0]["ShippingAddress"].ToString();
            string BillingAddress = Dt.Rows[0]["ShippingAddress"].ToString();
            string grandtotal = Dt.Rows[0]["Grandtotal"].ToString();

            //string ContactNo = Dt.Rows[0]["ContactNo"].ToString();
            //string EmailID = Dt.Rows[0]["Email"].ToString();
            //string TCSAmt = Dt.Rows[0]["TCSAmt"].ToString();
            //string TCSPercent = Dt.Rows[0]["TCSPercent"].ToString();

            string GSTNo = "";
            string PanNo = "";
            string SGSTNo = "";
            string SPanNo = "";
            string SBillingAddress = "";
            string StateName = "";
            string Shipcontact = "";
            string ShipEmail = "";
            string billingCustomerStateName = "";
            string shippingCustomerStateName = "";
            string billaddress = "";
            string shipaddress = "";

            if (billingCustomer == ShippingCustomer)
            {
                DataTable dtgstno = new DataTable();
                SqlDataAdapter sadgst = new SqlDataAdapter("select * from tbl_CompanyMaster where  Companyname='" + billingCustomer + "'", con);
                sadgst.Fill(dtgstno);
                if (dtgstno.Rows.Count > 0)
                {
                    GSTNo = dtgstno.Rows[0]["gstno"].ToString();
                    string billingaddress = dtgstno.Rows[0]["billingaddress"].ToString();
                    billaddress = Regex.Replace(billingaddress, @"\s+", " ");
                    string shippingaddress = dtgstno.Rows[0]["shippingaddress"].ToString();
                    shipaddress = Regex.Replace(shippingaddress, @"\s+", " ");

                    if (GSTNo == "")
                    {
                        PanNo = "";
                        GSTNo = "";
                    }
                    else
                    {
                        string result = GSTNo.Substring(0, 2);
                        SqlCommand cmdcheck = new SqlCommand("select StateName from tbl_States where StateCode='" + result + "'", con);
                        con.Open();
                        string stateName = cmdcheck.ExecuteScalar().ToString();
                        StateName = stateName + "(" + result + ")";
                        con.Close();
                    }
                }

                if (GSTNo == "")
                {
                    PanNo = "NA";
                    GSTNo = "NA";
                }
                else
                {
                    string MyString = GSTNo;
                    string res = MyString.Substring(0, 2);
                    string word1 = res;
                    string word2 = "1Z";
                    PanNo = stringBetween(MyString, word1, word2);
                }
            }
            else
            {
                DataTable dtgstno = new DataTable();
                SqlDataAdapter sadgst = new SqlDataAdapter("select * from tbl_CompanyMaster where status=0 and Companyname='" + billingCustomer + "'", con);
                sadgst.Fill(dtgstno);
                if (dtgstno.Rows.Count > 0)
                {
                    /////new addition changes 18/01/2023
                    GSTNo = dtgstno.Rows[0]["gstno"].ToString();
                    billaddress = dtgstno.Rows[0]["billingaddress"].ToString();
                    //////

                    SGSTNo = dtgstno.Rows[0]["gstno"].ToString();
                    if (GSTNo == "")
                    {
                        SPanNo = "";
                        SGSTNo = "";
                    }
                    else
                    {
                        SBillingAddress = dtgstno.Rows[0]["billingaddress"].ToString();

                        string result = SGSTNo.Substring(0, 2);

                        SqlCommand cmdcheck = new SqlCommand("select StateName from tbl_States where StateCode='" + result + "'", con);
                        con.Open();
                        string stateName = cmdcheck.ExecuteScalar().ToString();
                        billingCustomerStateName = stateName + "(" + result + ")";
                        con.Close();
                    }
                }

                if (SGSTNo == "")
                {
                    SPanNo = "NA";
                    SGSTNo = "NA";
                }
                else
                {
                    string MyString = SGSTNo;
                    string res = MyString.Substring(0, 2);
                    string word1 = res;
                    string word2 = "1Z";
                    SPanNo = stringBetween(MyString, word1, word2);
                }

                DataTable dtgstno1 = new DataTable();
                SqlDataAdapter sadgst1 = new SqlDataAdapter("select * from tbl_CompanyMaster where status=0 and Companyname='" + ShippingCustomer + "'", con);
                sadgst1.Fill(dtgstno1);
                if (dtgstno1.Rows.Count > 0)
                {
                    GSTNo = dtgstno1.Rows[0]["gstno"].ToString();

                    if (GSTNo == "")
                    {
                        PanNo = "";
                        GSTNo = "";
                    }
                    else
                    {
                        BillingAddress = dtgstno1.Rows[0]["shippingaddress"].ToString();
                        //Shipcontact = dtgstno1.Rows[0]["mobile1"].ToString();
                        //ShipEmail = dtgstno1.Rows[0]["email1"].ToString();

                        string result = GSTNo.Substring(0, 2);

                        SqlCommand cmdcheck = new SqlCommand("select StateName from tbl_States where StateCode='" + result + "'", con);
                        con.Open();
                        string stateName = cmdcheck.ExecuteScalar().ToString();
                        shippingCustomerStateName = stateName + "(" + result + ")";
                        con.Close();
                    }
                }

                if (GSTNo == "")
                {
                    PanNo = "NA";
                    GSTNo = "NA";
                }
                else
                {
                    string MyString = GSTNo;
                    string res = MyString.Substring(0, 2);
                    string word1 = res;
                    string word2 = "1Z";
                    PanNo = stringBetween(MyString, word1, word2);
                }
            }



            PdfContentByte cb = writer.DirectContent;
            //cb.Rectangle(27.5f, 731f, 560f, 80f);
            cb.Rectangle(27.5f, 676f, 560f, 149f);

            cb.Stroke();
            // Header 
            cb.BeginText();

            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 30);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Pune Abrasives Pvt. Ltd.", 50, 780, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 18);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "27ABCCS7002A1ZW", 50, 740, 0);


            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PLOT NO.84, D-II BLOCK, MIDC,", 50, 725, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CHINCHWAD PUNE- 411019.", 50, 715, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Maharashtra", 50, 705, 0);
            cb.EndText();





            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 0f;

            PdfPTable table = new PdfPTable(4);

            float[] widths2 = new float[] { 100, 180, 100, 180 };
            table.SetWidths(widths2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            //string datee = ffff1.ToString("yyyy-MM-dd");
            table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;

            var date = DateTime.Now.ToString("yyyy-MM-dd");

            table.AddCell(new Phrase(" IRN : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(IRN, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Ack No. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(AckNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Ack Date :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(F_AckDt, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Document Type : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("Tax Invoice", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Document No. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(InvoiceNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Document Date :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(invoicedate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Supply Type Code : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("B2B", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Place of Supply :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(ShippingAddress, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            //table.AddCell(new Phrase("E-Bill No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //table.AddCell(new Phrase(EBillNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            //table.AddCell(new Phrase("Reverse Charge :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //table.AddCell(new Phrase("No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            paragraphTable1.Add(table);
            doc.Add(paragraphTable1);

            //Bill To
            Paragraph paragraphTable44 = new Paragraph();
            paragraphTable44.SpacingAfter = 0f;

            Font font141 = FontFactory.GetFont("Arial", 9);
            Font font151 = FontFactory.GetFont("Arial", 9);

            table = new PdfPTable(2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 280f, 280f });

            //table.AddCell(paragraphTable4);
            if (billingCustomer == ShippingCustomer)
            {
               

                table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n GSTIN :  " + GSTNo + " \n " + ShippingCustomer + "\n " + shipaddress + "  \n " + StateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n GSTIN :  " + GSTNo + " \n " + ShippingCustomer + "\n " + shipaddress + "  \n " + StateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                //table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n " + ShippingCustomer + ", \n Address: " + shipaddress + "\n GSTIN: " + GSTNo + "      Pan No.: " + PanNo + " \n State Name:" + StateName + "      Contact No.: " + ContactNo + "\n Email ID.: " + EmailID + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }
            else
            {
                table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n GSTIN :  " + SGSTNo + " \n " + billingCustomer + "\n " + SBillingAddress + "  \n " + billingCustomerStateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n GSTIN :  " + GSTNo + " \n " + ShippingCustomer + "\n " + ShippingAddress + "  \n " + shippingCustomerStateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

                //table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n " + billingCustomer + "\n Address: " + SBillingAddress + " \n GSTIN: " + SGSTNo + "      Pan No.: " + SPanNo + " \n State Name:" + billingCustomerStateName + "      Contact No.: " + ContactNo + "\n Email ID.: " + EmailID + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                //table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n " + ShippingCustomer + ", \n Address: " + ShippingAddress + "\n GSTIN: " + GSTNo + "      Pan No.: " + PanNo + " \n State Name:" + shippingCustomerStateName + "      Contact No.: " + Shipcontact + "\n Email ID.: " + ShipEmail + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }
            doc.Add(table);
            // End

            ///Description Table
            Paragraph paragraphTable2 = new Paragraph();
            paragraphTable2.SpacingAfter = 0f;

            if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
            {
                table = new PdfPTable(12);
                float[] widths3 = new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 8f, 10f, 15f };
                table.SetWidths(widths3);
            }
            else
            {
                table = new PdfPTable(10);
                float[] widths3 = new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 15f };
                table.SetWidths(widths3);
            }


            double TDisc_Amt = 0;
            double Ttotal_price = 0;
            double CGST_price = 0;
            double SGST_price = 0;
            double IGST_price = 0;
            double GrandTotal1 = 0;
            string CGSTPer = "";
            string SGSTPer = "";
            string IGSTPer = "";


            if (Dt.Rows.Count > 0)
            {
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.AddCell(new Phrase("SN.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("                  Item Description", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("HSN Code", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(" Qty", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("    Rate", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(" Disc(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Taxable Amount", FontFactory.GetFont("Arial", 9, Font.BOLD)));

                if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
                {
                    table.AddCell(new Phrase("CGST (%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    table.AddCell(new Phrase("CGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    table.AddCell(new Phrase("SGST (%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    table.AddCell(new Phrase("SGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("IGST (%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    table.AddCell(new Phrase("IGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }


                table.AddCell(new Phrase("      Total", FontFactory.GetFont("Arial", 9, Font.BOLD)));

                int rowid = 1;
                foreach (DataRow dr in Dt.Rows)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;
                    table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;

                    string Rate = dr["Rate"].ToString();
                    var ConvRate = Convert.ToDouble(Rate);
                    string FinaleRate = ConvRate.ToString("N2", info);

                    string TaxableAmt = dr["Amount"].ToString();
                    var ConvTaxableAmt = Convert.ToDouble(TaxableAmt);
                    string FinaleTaxableAmt = ConvTaxableAmt.ToString("N2", info);

                    double Ftotal = Convert.ToDouble(dr["GrandTotal"].ToString());
                    string _ftotal = Ftotal.ToString("##.00");

                    string partic = dr["Particulars"].ToString().Replace("Enclosure For Control Panel.", "");

                    string Description = partic + "\n" + dr["Description"].ToString();

                    var amt = dr["Amount"].ToString();

                    decimal cgstamt = 0;
                    decimal sgstamt = 0;
                    decimal igstamt = 0;
                    if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
                    {
                        var cgstper = dr["CGSTPer"].ToString();
                        var sgstper = dr["SGSTPer"].ToString();

                        cgstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(cgstper) / 100;
                        sgstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(sgstper) / 100;
                    }
                    else
                    {
                        var igstper = dr["IGSTPer"].ToString();

                        igstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(igstper) / 100;
                    }

                    double Rateee = Convert.ToDouble(dr["Rate"].ToString());
                    double Quantity = Convert.ToDouble(dr["Qty"].ToString());
                    double DiscPer = Convert.ToDouble(dr["Discount"].ToString());
                    double RtQty = (Convert.ToDouble(Rateee)) * (Convert.ToDouble(Quantity));
                    double DiscAmt = (Convert.ToDouble(RtQty)) * (Convert.ToDouble(DiscPer)) / 100;

                    table.AddCell(new Phrase(" " + rowid.ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(dr["Description"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(" " + dr["HSN"].ToString(), FontFactory.GetFont("Arial", 9)));

                    //new changes 4/2/23
                    //table.AddCell(new Phrase(dr["Qty"].ToString() + " Nos", FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase("   " + dr["Qty"].ToString() + " " + dr["UOM"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(" " + FinaleRate, FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase("      " + dr["Discount"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase("" + FinaleTaxableAmt, FontFactory.GetFont("Arial", 9)));

                    if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
                    {
                        table.AddCell(new Phrase("   " + dr["CGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("" + cgstamt.ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("   " + dr["SGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("" + sgstamt.ToString(), FontFactory.GetFont("Arial", 9)));
                    }
                    else
                    {
                        table.AddCell(new Phrase("   " + dr["IGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("" + igstamt.ToString(), FontFactory.GetFont("Arial", 9)));
                    }
                    table.AddCell(new Phrase(" " + _ftotal, FontFactory.GetFont("Arial", 9)));
                    rowid++;
                    CGSTPer = dr["CGSTPer"].ToString();
                    SGSTPer = dr["SGSTPer"].ToString();
                    IGSTPer = dr["IGSTPer"].ToString();
                    Ttotal_price += Convert.ToDouble(dr["Amount"].ToString());
                    GrandTotal1 += Convert.ToDouble(dr["GrandTotal"].ToString());
                    CGST_price += Convert.ToDouble(cgstamt);
                    SGST_price += Convert.ToDouble(sgstamt);
                    IGST_price += Convert.ToDouble(igstamt);
                    TDisc_Amt += Convert.ToDouble(DiscAmt);
                }

            }
            string FinalDiscAmt = TDisc_Amt.ToString("00.00");
            string amount = Ttotal_price.ToString();
            paragraphTable2.Add(table);
            doc.Add(paragraphTable2);
            ///End Description table
            //Space
            Paragraph paragraphTable3 = new Paragraph();

            string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font12 = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Font font10 = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Paragraph paragraph = new Paragraph("", font12);

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }

            if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.SetWidths(new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 8f, 10f, 15f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                if (Dt.Rows.Count == 1)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 2)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 3)
                {
                    table.AddCell(new Phrase("\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                doc.Add(table);
                //Space end
            }
            else
            {
                table = new PdfPTable(10);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.SetWidths(new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 15f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                if (Dt.Rows.Count == 1)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 2)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 3)
                {
                    table.AddCell(new Phrase("\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                doc.Add(table);
            }
            //Fright description
            Paragraph paragraphTable4 = new Paragraph();
            paragraphTable4.SpacingAfter = 0f;
            if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
            {
                table = new PdfPTable(12);
                float[] widths33 = new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 8f, 10f, 15f };
                table.SetWidths(widths33);
            }
            else
            {
                table = new PdfPTable(10);
                float[] widths33 = new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 15f };
                table.SetWidths(widths33);
            }
            double Ttotal_price1 = 0;
            double CGST_price1 = 0;
            double SGST_price1 = 0;
            double IGST_price1 = 0;
            int rowidd = 1;
            decimal ValueOFSupply = 0;
            decimal CGStTotal = 0;
            decimal SGStTotal = 0;
            decimal IGStTotal = 0;
            decimal TotalAmount = 0;
            SqlCommand cmd = new SqlCommand("select * from vw_TaxInvoicePDF where Id='" + id + "'", con);
            con.Open();
            SqlDataReader dr1 = cmd.ExecuteReader();
            if (dr1.Read())
            {
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                double Ftotal = Convert.ToDouble(dr1["Cost"].ToString());
                string _ftotal = Ftotal.ToString("00.00");
                string Description1 = dr1["ChargesDescription"].ToString();
                var amt = dr1["Basic"].ToString();

                decimal cgstamt1 = 0;
                decimal sgstamt1 = 0;
                decimal igstamt1 = 0;
                if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
                {
                    var cgstper1 = dr1["CGST"].ToString();
                    var sgstper1 = dr1["SGST"].ToString();

                    cgstamt1 = Convert.ToDecimal(amt) * Convert.ToDecimal(cgstper1) / 100;
                    sgstamt1 = Convert.ToDecimal(amt) * Convert.ToDecimal(sgstper1) / 100;
                }
                else
                {
                    var igstper1 = dr1["IGST"].ToString();

                    igstamt1 = Convert.ToDecimal(amt) * Convert.ToDecimal(igstper1) / 100;
                }
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("Transport Charges", FontFactory.GetFont("Arial", 9)));
                //table.AddCell(new Phrase(Description1, FontFactory.GetFont("Arial", 9)));
                //table.AddCell(new Phrase(dr1["HSNTcs"].ToString(), FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(" 9965", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(" " + dr1["Basic"].ToString(), FontFactory.GetFont("Arial", 9)));
                if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
                {
                    table.AddCell(new Phrase("   " + dr1["CGST"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(cgstamt1.ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase("   " + dr1["SGST"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(sgstamt1.ToString(), FontFactory.GetFont("Arial", 9)));
                }
                else
                {
                    table.AddCell(new Phrase("   " + dr1["IGST"].ToString(), FontFactory.GetFont("Arial", 9)));
                    table.AddCell(new Phrase(igstamt1.ToString(), FontFactory.GetFont("Arial", 9)));
                }
                table.AddCell(new Phrase(" " + _ftotal, FontFactory.GetFont("Arial", 9)));
                //rowidd++;

                Ttotal_price1 += Convert.ToDouble(dr1["TaxableAmt"].ToString());
                CGST_price1 += Convert.ToDouble(cgstamt1);
                SGST_price1 += Convert.ToDouble(sgstamt1);
                IGST_price1 += Convert.ToDouble(sgstamt1);

                ValueOFSupply = Convert.ToDecimal(Ttotal_price) + Convert.ToDecimal(dr1["Basic"]);
                CGStTotal = Convert.ToDecimal(CGST_price) + Convert.ToDecimal(cgstamt1);
                SGStTotal = Convert.ToDecimal(SGST_price) + Convert.ToDecimal(sgstamt1);
                IGStTotal = Convert.ToDecimal(IGST_price) + Convert.ToDecimal(igstamt1);
                TotalAmount = Convert.ToDecimal(Ftotal) + Convert.ToDecimal(GrandTotal1);
                dr1.Close();
                con.Close();
            }

            string amount1 = Ttotal_price1.ToString();
            paragraphTable4.Add(table);
            doc.Add(paragraphTable4);

            //end Fright description

            ////calculation supply
            ///value of supply
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

            // footer Details Table Start 
            Paragraph paragraphTable12 = new Paragraph();
            paragraphTable12.SpacingAfter = 1f;
            paragraphTable12.SpacingBefore = 1f;


            table = new PdfPTable(10);
            //float[] widths32 = new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 15f };
            float[] widths32 = new float[] { 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f };
            table.SetWidths(widths32);



            double Ttotal_pricee = 0;
            double CGST_pricee = 0;
            double SGST_pricee = 0;
            double IGST_pricee = 0;
            double GrandTotal12 = 0;
            double CGSTPerr = 0;
            double SGSTPerr = 0;
            double IGSTPerr = 0;

            double ConvValueOFSupply = Convert.ToDouble(ValueOFSupply);
            string FinaleValueOFSupply = ConvValueOFSupply.ToString("00.00");

            double CGSTAmount = 0;
            double SGSTAmount = 0;
            double IGSTAmount = 0;
            double GrandTOTALAmount = 0;

            CGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(CGSTPer.Trim())) / 100;
            SGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(SGSTPer.Trim())) / 100;
            IGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(IGSTPer.Trim())) / 100;
            GrandTOTALAmount = Convert.ToDouble(ValueOFSupply) + CGSTAmount + SGSTAmount + IGSTAmount;

            if (Dt.Rows.Count > 0)
            {
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.AddCell(new Phrase("Tax'ble Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("  CGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("  SGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("  IGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(" CESS Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(" State CESS", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("  Discount", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Other Charges", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Round off Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Tot Inv. Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));


                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;


                table.AddCell(new Phrase(FinaleValueOFSupply.ToString(), FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(CGSTAmount.ToString("00.00"), FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(SGSTAmount.ToString("00.00"), FontFactory.GetFont("Arial", 9)));

                //new changes 4/2/23
                //table.AddCell(new Phrase(dr["Qty"].ToString() + " Nos", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(IGSTAmount.ToString("00.00"), FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("00.00", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("00.00", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(FinalDiscAmt, FontFactory.GetFont("Arial", 9)));

                table.AddCell(new Phrase("00.00", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("00.00", FontFactory.GetFont("Arial", 9)));

                table.AddCell(new Phrase(GrandTOTALAmount.ToString("00.00"), FontFactory.GetFont("Arial", 9)));


            }
            string amountt = Ttotal_price.ToString();
            paragraphTable12.Add(table);
            doc.Add(paragraphTable12);
            ///End Description table
            //Space
            Paragraph paragraphTable32 = new Paragraph();

            string[] items2 = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font122 = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Font font100 = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Paragraph paragraph1 = new Paragraph("", font122);

            for (int i = 0; i < items.Length; i++)
            {
                paragraph1.Add(new Phrase("", font100));
            }


            table = new PdfPTable(10);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.SetWidths(new float[] { 4f, 39f, 12f, 6f, 12f, 10f, 12f, 8f, 10f, 15f });
            table.SetWidths(new float[] { 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f });
            table.AddCell(paragraph);
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));


            doc.Add(table);


            ////calculation supply
            ///value of supply
            Paragraph paragraphTable51 = new Paragraph();

            string[] itemsss1 = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font131 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font111 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphh1 = new Paragraph("", font111);



            for (int i = 0; i < items.Length; i++)
            {
                paragraphh1.Add(new Phrase("", font10));
            }




            if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
            {
                CGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(CGSTPer.Trim())) / 100;
                SGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(SGSTPer.Trim())) / 100;
            }
            else
            {
                IGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(IGSTPer.Trim())) / 100;
            }

            //Declaration
            Paragraph paragraphTable99 = new Paragraph(" Remarks :\n\n", font12);

            //Puja Enterprises Sign
            string[] itemss = {
                "Declaration  : I/We hereby certify that my/our registration certificate under the GST Act, 2017 is in force on the date on which the supply of",
                "the goods specified in this tax invoice is made by me/us and that the transaction of supplies covered by this tax invoice has been effected",
                "by me/us and it shall be accounted for in the turnover of supplies while filing of return and the due tax, if any, payable on the supplies",
                "has been paid or shall be paid. \n",

                "Subject To Pune Jurisdiction Only.",

                        };

            Font font14 = FontFactory.GetFont("Arial", 9);
            Font font15 = FontFactory.GetFont("Arial", 9, Font.NORMAL);
            Paragraph paragraphhh = new Paragraph(" Terms & Condition :\n\n", font15);


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
            ///end declaration
            ///

            ///Sign Authorization
            ///

            // Bind stamp Image
            string imageStamp = Server.MapPath("~") + "/Content/img/Account.png";
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imageStamp);
            image1.ScaleToFit(600, 120);
            PdfPCell imageCell = new PdfPCell(image1);
            imageCell.PaddingLeft = 10f;
            imageCell.PaddingTop = 0f;
            /////////////////

            Paragraph paragraphTable10000 = new Paragraph();


            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();
            PdfPTable table25 = new PdfPTable(1);
            table25.TotalWidth = 560f;
            table25.LockedWidth = true;
            table25.SetWidths(new float[] { 400f });




            // Create a cell for the image
            PdfPCell cell = new PdfPCell();
            //cell.Border = PdfPCell.NO_BORDER;


            // Load and add the image to the cell
            string imageURLL = Server.MapPath("~/E_Inv_QrCOde/Barcode_Img_CrDbNote.png");
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageURLL);
            image.ScaleToFit(185, 200);
            image.SetAbsolutePosition(1200, 1600);
            image.SpacingBefore = 10f;
            image.SpacingAfter = 10f;

            string format = "dd-MM-yyyy HH:mm:ss";
            DateTime currentDate = DateTime.Now;
            string formattedDate = currentDate.ToString(format);

            cell.AddElement((new Phrase(" Generated By :  27ABCCS7002A1ZW \n Print Date : " + formattedDate + " ", FontFactory.GetFont("Arial", 9, Font.BOLD))));
            image.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(image);

            // Add the cell to the table
            table25.AddCell(cell);
            // Add content to the paragraph

            // Add the table to the paragraph
            paragraphhhhhff.Add(table25);

            // Add the paragraph to the document
            doc.Add(paragraphhhhhff);


            //doc.Close();
            ///end Sign Authorization


            doc.NewPage();

            //doc.Add(table);//Add the paragarh to the document  
        }

        //doc.Close();

        //ifrRight6.Attributes["src"] = @"../files/" + Docname;
        #endregion



        doc.Close();

        ifrRight6.Attributes["src"] = @"../PDF_Files/" + Docname;

    }

    private void GenerateQR(string QR_String)
    {
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img_CRDBNote.jpg");
        var barcodeBitmap = new System.Drawing.Bitmap(result);

        using (MemoryStream memory = new MemoryStream())
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                barcodeBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] bytes = memory.ToArray();
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        //img_QrCode.Visible = true;
        img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img_CRDBNote.jpg";
    }

    private void GenerateBarcode(string AckNo)
    {
        // The string value you want to encode as a barcode
        string data = AckNo;

        // Generate the barcode image
        BarcodeWriter barcodeWriter = new BarcodeWriter();
        barcodeWriter.Format = BarcodeFormat.CODE_128; // You can choose other barcode formats as needed
        EncodingOptions encodingOptions = new EncodingOptions
        {
            Width = 250, // Set the width of the barcode image
            Height = 65 // Set the height of the barcode image
        };
        barcodeWriter.Options = encodingOptions;
        System.Drawing.Bitmap barcodeBitmap = barcodeWriter.Write(data);

        // Convert the System.Drawing.Bitmap to a byte array
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        {
            barcodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] barcodeBytes = ms.ToArray();

            // Save the barcode image to a folder on the server
            string folderPath = Server.MapPath("~/E_Inv_QrCOde/"); // Change this to your desired folder path
            string fileName = "Barcode_Img_CRDBNote.png"; // Generate a unique filename
            string filePath = System.IO.Path.Combine(folderPath, fileName);
            barcodeBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            // Display the barcode image on the web page
            imgBarcode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(barcodeBytes);
            imgBarcode.Visible = false;
        }
    }

    public static string stringBetween(string Source, string Start, string End)
    {
        string result = "";
        if (Source.Contains(Start) && Source.Contains(End))
        {
            int StartIndex = Source.IndexOf(Start, 0) + Start.Length;
            int EndIndex = Source.IndexOf(End, StartIndex);
            result = Source.Substring(StartIndex, EndIndex - StartIndex);
            return result;
        }
        return result;
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        //CreateEWayBillby_EInvoiceAPI(); // for Creating EInvoice API
        GenerateEWayBill_ByEwayApi(); // for Creating EWAY Bill API API       
    }

    protected void ddlTransportationMode_TextChanged(object sender, EventArgs e)
    {
        try
        {
            con.Open();
            if (ddlTransportationMode.SelectedItem.Value == "1")
            {
                ddlVehicleType.SelectedValue = "R";
                lblTransporterDocNo.Text = "Transporter Doc. No";
                txtVehicleNumber.Enabled = true;
            }
            else if (ddlTransportationMode.SelectedItem.Value == "2")
            {
                ddlVehicleType.SelectedValue = "R";
                txtTransporterName.Text = "Rail";
                txtVehicleNumber.Enabled = false;
                lblTransporterDocNo.Text = "RR No.";
            }
            else if (ddlTransportationMode.SelectedItem.Value == "3")
            {
                ddlVehicleType.SelectedValue = "R";
                txtTransporterName.Text = "Air";
                txtVehicleNumber.Enabled = false;
                lblTransporterDocNo.Text = "Airway Bill No.";
            }
            else if (ddlTransportationMode.SelectedItem.Value == "4")
            {
                ddlVehicleType.SelectedValue = "R";
                txtTransporterName.Text = "";
                txtVehicleNumber.Enabled = false;
                lblTransporterDocNo.Text = "Bill of lading No.";
            }
            GetPDFDetails();
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    private void Load_E_WayBillData(string id)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from tblcreditdebitnotehdr where Id = '" + id + "'", con);
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            lblHiddenid.Text = Dt.Rows[0]["Id"].ToString();
            lblEwaybillno.Text = Dt.Rows[0]["EwbNo"].ToString();
            GetPDFDetails_EwayBill();
        }
    }

    protected void GetPDFDetails()
    {
        //con.Open();
        SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tblcreditdebitnotehdr where DocNo='" + lblInvoiceNo.Text + "'", con);
        Object Qrdtl = cmdQrdtl.ExecuteScalar();
        string F_Qrdtl = Convert.ToString(Qrdtl);
        con.Close();
        if (F_Qrdtl != "")
        {
            con.Open();
            SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tblcreditdebitnotehdr where DocNo='" + lblInvoiceNo.Text + "'", con);
            Object ACkdtl = cmdAckdtl.ExecuteScalar();
            string F_Ackdtl = Convert.ToString(ACkdtl);
            con.Close();

            GenerateQR(F_Qrdtl); GenerateBarcode(F_Ackdtl);
            Pdf();

        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
        }
    }

    #region E-Invoice API - Generate Eway Bill, AuthToken, Get GST Details
    protected void CreateEWayBillby_EInvoiceAPI()
    {
        try
        {

            con.Open();
            DataTable dtCompany = new DataTable();
            SqlDataAdapter sadCompany = new SqlDataAdapter("select * from [tblCreditDebitNoteHdr] where Id='" + lblHiddenid.Text + "'", con);
            sadCompany.Fill(dtCompany);

            string _TokenExpiry = "", _Sek = "", _AuthKey = "", _Messeg = "";
            DataTable DT_AuthToken = new DataTable();
            //SqlDataAdapter sad_AuthToken = new SqlDataAdapter("SELECT * FROM AUTH_TOKEN_MASTER WHERE UserName='" + UserName + "'", con);
            SqlDataAdapter sad_AuthToken = new SqlDataAdapter("SELECT * FROM AUTH_TOKEN_MASTER", con);
            sad_AuthToken.Fill(DT_AuthToken);
            if (DT_AuthToken.Rows.Count > 0)
            {
                DateTime Token_Expiry = Convert.ToDateTime(DT_AuthToken.Rows[0]["TokenExpiry"]);

                if (Token_Expiry <= DateTime.Now)
                {
                    GenrateAuthKey(UserName, Password, GST, out _TokenExpiry, out _Sek, out _AuthKey, out _Messeg);

                    AuthToken = _AuthKey;

                    if (string.IsNullOrWhiteSpace(AuthToken))
                    {
                        //XtraMessageBox.Show(_Messeg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    AuthToken = DT_AuthToken.Rows[0]["AuthToken"].ToString();
                    con.Close();
                }
            }
            //con.Close();

            // Code Start For Generation
            string Status_Desc = "";
            string Status_CD = "";
            string EwbNo = "";
            string EwbDt = "";
            string EwbValidTill = "";

            if (!string.IsNullOrWhiteSpace(AuthToken))
            {
                string strUrl = string.Format("https://api.mastergst.com/einvoice/type/GENERATE_EWAYBILL/version/V1_03?email=erp%40weblinkservices.net");  //E-Invoice API
                WebRequest requestObjPost = WebRequest.Create(strUrl);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";
                requestObjPost.Headers.Add("ip_address",IPAddress);
                requestObjPost.Headers.Add("client_id", E_Invoice_API_Client_ID);
                requestObjPost.Headers.Add("client_secret", E_Invoice_API_Secret);
                requestObjPost.Headers.Add("username", UserName);
                requestObjPost.Headers.Add("auth-token", AuthToken);
                requestObjPost.Headers.Add("gstin", GST);

                ////check Values
                con.Open();
                string ffff1 = dtCompany.Rows[0]["DocDate"].ToString();
                string ffff2 = txtdate.Text;
                string Transaction_Date = ffff1.Replace("-", "/");
                string TransactionDoc_Date = ffff2.Replace("-", "/");
                string ShippingGST = dtCompany.Rows[0]["ShippingGST"].ToString();
                string BillingPincode = dtCompany.Rows[0]["BillingPincode"].ToString();
                string ShippingPincode = dtCompany.Rows[0]["ShippingPincode"].ToString();
                string ShippingStatecode = dtCompany.Rows[0]["ShippingStatecode"].ToString();

                //Total Invoice Value
                SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(Total as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + lblHiddenid.Text + "'", con);
                Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                double TotInvValuee = Convert.ToDouble(F_TotInvVal);
                string TotInvVal = TotInvValuee.ToString("0.00");

                //Total mainHsnCode Value
                SqlCommand cmdmainHsnCode = new SqlCommand("select top 1 HSN from [tblCreditDebitNotedtls] where HeaderID='" + lblHiddenid.Text + "'", con);
                Object F_mainHsnCode = cmdmainHsnCode.ExecuteScalar();
                string mainHsnCode = F_mainHsnCode.ToString();
                string postData = "";
                con.Close();


                //string postData = "{\"version\":\"1.0.1118\",\"billLists\":[{\"userGstin\":\"" + GST + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"subSupplyDesc\":\"\",\"docType\":\"INV\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + Transaction_Date + "\",\"transType\":" + ddlTransactionType.SelectedValue + ",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\"transMode\":" + ddlTransportationMode.SelectedValue + ",\"transDistance\":" + txtDistance.Text+",\"transporterName\":\"" + txtTransporterName.Text + "\",\"transporterId\":\"" + txtTransporterID.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"transDocDate\":\"" + TransactionDoc_Datet + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"mainHsnCode\":\"" + mainHsnCode + "\"}]}";
                //string postData = "{\"version\":\"1.0.1118\",\"billLists\":[{\"userGstin\":\"" + GST + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"subSupplyDesc\":\"\",\"docType\":\"INV\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + Transaction_Date + "\",\"transType\":" + ddlTransactionType.SelectedValue + ",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\"transMode\":" + ddlTransportationMode.SelectedValue + ",\"Distance\":" + txtDistance.Text + ",\"transporterName\":\"" + txtTransporterName.Text + "\",\"transporterId\":\"" + txtTransporterID.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"mainHsnCode\":\"" + mainHsnCode + "\"}]}";
                //string postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"TransDocNo\":\"" + txtTransporterDocNo.Text + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\""+GST+ "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\""+ ShippingGST + "\",\"toPincode\":"+ ShippingPincode + ",\"toStateCode\":"+ ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":"+ TotInvVal + "}";
                //final
                if (txtTransporterID.Text == "" && txtTransporterName.Text == "")
                {
                    if (txtVehicleNumber.Text == "")
                    {//before 30-1023
                     //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                     //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed
                     //if (BillingPincode == ShippingPincode)
                        if (BillingPincode == "411062")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    else
                    {//before 30-1023
                     //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                     //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                        //if (BillingPincode == ShippingPincode)
                        if (BillingPincode == "411062")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                }
                else
                {
                    if (txtVehicleNumber.Text == "")
                    {//before 30-1023
                     //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                     //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                        //if (BillingPincode == ShippingPincode)
                        if (BillingPincode == "411062")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    else if (txtVehicleNumber.Text != "" && txtTransporterDocNo.Text != "")
                    {//before 30-1023
                     //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                     //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                        //if (BillingPincode == ShippingPincode)
                        if (BillingPincode == "411062")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    else if (ddlTransportationMode.SelectedValue == "0")
                    {//before 30-1023
                     //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                     //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                        //if (BillingPincode == ShippingPincode)
                        if (BillingPincode == "411062")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    else if (txtTransporterID.Text != "" && txtVehicleNumber.Text != "" && txtTransporterDocNo.Text == "")
                    {//before 30-1023
                     //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                     //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                        //if (BillingPincode == ShippingPincode)
                        if (BillingPincode == "411062")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransId\":\"" + txtTransporterID.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    //else
                    //{
                    //    postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                    //}
                }

                //Save JSON IN DATABASE update by pawar 28/09/2024
                con.Open();
                Cmd = new SqlCommand("UPDATE tblCreditDebitNoteHdr SET E_WAY_Bill_JSON=@E_WAY_Bill_JSON WHERE Id=" + lblHiddenid.Text + "", con);
                Cmd.Parameters.AddWithValue("@E_WAY_Bill_JSON", postData);
                Cmd.ExecuteNonQuery();
                con.Close();
                using (var streamwriter = new StreamWriter(requestObjPost.GetRequestStream()))
                {
                    //streamwriter.Write(postData);
                    streamwriter.Write(postData);
                    streamwriter.Flush();
                    streamwriter.Close();
                    var httpResponse = (HttpWebResponse)requestObjPost.GetResponse();
                    using (var streamreader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result2 = streamreader.ReadToEnd();
                        var JsonRespons = JObject.Parse(result2);
                        Status_Desc = JsonRespons["status_desc"].ToString();
                        Status_CD = JsonRespons["status_cd"].ToString();
                        if (Status_CD == "1")
                        {
                            EwbNo = JsonRespons["data"]["EwbNo"].ToString();
                            EwbDt = JsonRespons["data"]["EwbDt"].ToString();
                            EwbValidTill = JsonRespons["data"]["EwbValidTill"].ToString();
                        }
                    }
                }
            }

            //Return Value             
            string _Messege = Status_Desc;
            string _EwbNo = EwbNo;
            string _EwbDt = EwbDt;
            string _EwbValidTill = EwbValidTill;

            if (string.IsNullOrWhiteSpace(_EwbNo))
            {
                con.Close();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + _Messege + "');", true);
                con.Open();
                GetPDFDetails();
            }

            ////Update Data                
            if (_EwbNo == "" || _EwbDt == "")
            {
                con.Close();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Something Went Wrong. Kindly Create Again..!!');", true);
            }
            else
            {
                con.Open();
                Cmd = new SqlCommand("UPDATE tblCreditDebitNoteHdr SET EwbNo=@EwbNo,EwbDt=@EwbDt,EwbValidTill=@EwbValidTill,e_way_status=@e_way_status,e_way_created_by=@e_way_created_by WHERE Id=" + lblHiddenid.Text + "", con);
                Cmd.Parameters.AddWithValue("@EwbNo", _EwbNo);
                Cmd.Parameters.AddWithValue("@EwbDt", _EwbDt);
                Cmd.Parameters.AddWithValue("@EwbValidTill", _EwbValidTill);
                //Cmd.Parameters.AddWithValue("@Status", _Status);
                Cmd.Parameters.AddWithValue("@e_way_status", 1);
                Cmd.Parameters.AddWithValue("@e_way_created_by", Session["Username"].ToString());
                Cmd.ExecuteNonQuery();
                con.Close();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Genereted Sucessfully');window.location.href='EWayBillList.aspx';", true);
            }
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    private string GenrateAuthKey(string User_Name, string Password, string Seller_GST_No, out string _TokenExpiry, out string _Sek, out string _AuthKey, out string _Messege)
    {
       

        string uri = "https://api.mastergst.com/einvoice/authenticate?email=erp%40weblinkservices.net";
        WebResponse response;
        WebRequest request = WebRequest.Create(uri);
        con.Close();

        request.Method = "GET";
        request.Headers.Add("username", UserName);
        request.Headers.Add("password", Password);
        request.Headers.Add("ip_address", IPAddress);
        request.Headers.Add("client_id", E_Invoice_API_Client_ID);
        request.Headers.Add("client_secret", E_Invoice_API_Secret);
        request.Headers.Add("gstin", GST);
        response = request.GetResponse();

        using (Stream dataStream = response.GetResponseStream())
        {
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            var myJsonString = responseFromServer;
            var jo = JObject.Parse(myJsonString);

            string Status_Desc = jo["status_desc"].ToString();
            string Status_CD = jo["status_cd"].ToString();

            string TokenExpiry = "";
            string Sek = "";
            string ClientId = "";
            string AuthKey = "";

            if (Status_CD != "0")
            {
                TokenExpiry = jo["data"]["TokenExpiry"].ToString();
                Sek = jo["data"]["Sek"].ToString();
                ClientId = jo["data"]["ClientId"].ToString();
                AuthKey = jo["data"]["AuthToken"].ToString();

                con.Open();
                Cmd = new SqlCommand("DELETE FROM AUTH_TOKEN_MASTER", con);
                Cmd.ExecuteNonQuery();

                Cmd = new SqlCommand("INSERT INTO AUTH_TOKEN_MASTER (UserName,TokenExpiry,Sek,ClientId,AuthToken)VALUES(@UserName,@TokenExpiry,@Sek,@ClientId,@AuthToken)", con);
                Cmd.Parameters.AddWithValue("@UserName", User_Name);
                Cmd.Parameters.AddWithValue("@TokenExpiry", Convert.ToDateTime(TokenExpiry).AddMinutes(-45));
                Cmd.Parameters.AddWithValue("@Sek", Sek);
                Cmd.Parameters.AddWithValue("@ClientId", ClientId);
                Cmd.Parameters.AddWithValue("@AuthToken", AuthKey);
                Cmd.ExecuteNonQuery();
                con.Close();
            }

            _TokenExpiry = TokenExpiry;
            _Sek = Sek;
            _AuthKey = AuthKey;
            _Messege = Status_Desc;

            return AuthKey;
        }
    }

    private string GenrateGSTDtls(string User_Name, string Password, string Seller_GST_No, string GST_Par)
    {
        try
        {

            //AuthToken
            string _TokenExpiry = "", _Sek = "", _AuthKey = "", _Messeg = "", _TradeName = "", _Messege = "", _LegalName = "", _Status = "";
            DataTable DT_AuthToken = new DataTable();
            //SqlDataAdapter sad_AuthToken = new SqlDataAdapter("SELECT * FROM AUTH_TOKEN_MASTER WHERE UserName='" + User_Name + "'", con);
            SqlDataAdapter sad_AuthToken = new SqlDataAdapter("SELECT * FROM AUTH_TOKEN_MASTER", con);
            sad_AuthToken.Fill(DT_AuthToken);
            if (DT_AuthToken.Rows.Count > 0)
            {
                DateTime Token_Expiry = Convert.ToDateTime(DT_AuthToken.Rows[0]["TokenExpiry"]);

                if (Token_Expiry <= DateTime.Now)
                {
                    GenrateAuthKey(User_Name, Password, Seller_GST_No, out _TokenExpiry, out _Sek, out _AuthKey, out _Messeg);

                    AuthToken = _AuthKey;

                    if (string.IsNullOrWhiteSpace(AuthToken))
                    {
                        //XtraMessageBox.Show(_Messeg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    AuthToken = DT_AuthToken.Rows[0]["AuthToken"].ToString();
                }
            }

            ///
            //string uri = "https://api.mastergst.com/einvoice/type/SYNC_GSTIN_FROMCP/version/V1_03?param1=27AUXPS9994G1Z8&email=erp%40weblinkservices.net";
            string uri1 = "https://api.mastergst.com/einvoice/type/SYNC_GSTIN_FROMCP/version/V1_03?param1=";
            string uri2 = "&email=erp%40weblinkservices.net";
            string uri3 = uri1 + GST_Par + uri2;
            WebResponse response;
            WebRequest request = WebRequest.Create(uri3);

            request.Method = "GET";
            request.Headers.Add("param1", GST_Par);
            request.Headers.Add("email", MailID);
            request.Headers.Add("ip_address", "103.174.254.209");
            request.Headers.Add("client_id", E_Invoice_API_Client_ID);
            request.Headers.Add("client_secret", E_Invoice_API_Secret);
            request.Headers.Add("username", UserName);
            request.Headers.Add("auth-token", AuthToken);
            request.Headers.Add("gstin", GST);
            response = request.GetResponse();

            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                var myJsonString = responseFromServer;
                var jo = JObject.Parse(myJsonString);

                string Status_Desc = jo["status_desc"].ToString();
                string Status_CD = jo["status_cd"].ToString();

                string TradeName = "";
                string LegalName = "";
                string Status = "";

                if (Status_CD != "0")
                {
                    TradeName = jo["data"]["TradeName"].ToString();
                    LegalName = jo["data"]["LegalName"].ToString();
                    Status = jo["data"]["Status"].ToString();
                }

                _TradeName = TradeName;
                _LegalName = LegalName;
                _Status = Status;
                _Messege = Status_Desc;
                txtTransporterName.Text = TradeName;
                return TradeName;
            }
        }
        catch (Exception ex)
        {
            throw ex;
            //string errorMsg = "An error occurred : " + ex.Message;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }
    #endregion

    protected void GetPDFDetails_EwayBill()
    {
        //con.Open();
        SqlCommand cmdQrdtl = new SqlCommand("select EwbNo from tblcreditdebitnotehdr where Id='" + lblHiddenid.Text + "'", con);
        Object Qrdtl = cmdQrdtl.ExecuteScalar();
        string F_Qrdtl = Convert.ToString(Qrdtl);
        con.Close();
        if (F_Qrdtl != "")
        {
            con.Open();
            //SqlCommand cmdAckdtl = new SqlCommand("select EwbNo from tblcreditdebitnotehdr where Id='" + id + "'", con);
            //Object ACkdtl = cmdAckdtl.ExecuteScalar();
            //string F_Ackdtl = Convert.ToString(ACkdtl);
            DataTable Dt = new DataTable();
            SqlDataAdapter Da = new SqlDataAdapter("select * from tblcreditdebitnotehdr where Id = '" + lblHiddenid.Text + "'", con);
            Da.Fill(Dt);
            string EwbNo = Dt.Rows[0]["EwbNo"].ToString();
            string date = Dt.Rows[0]["EwbDt"].ToString();
            string EwbDt = date.Replace("/", "-");
            con.Close();

            GenerateQR_Eway(EwbNo, GST, EwbDt);
            GenerateBarcode_Eway(EwbNo);
            EwayPdf();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Not Created. Kindly Create First..!!');", true);
        }
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        string _Status_CD = "", _Messege = "", _CancelDate = "";
        try
        {
            con.Open();
            bool F_e_way_status;
            //Check Already Canceled E Invoice
            SqlCommand cmde_way_status = new SqlCommand("select e_way_cancel_status from tblcreditdebitnotehdr where Id='" + lblHiddenid.Text + "'", con);
            Object e_way_status = cmde_way_status.ExecuteScalar();
            if (e_way_status == "")
            {
                F_e_way_status = true;
            }
            else
            {
                F_e_way_status = false;
            }

            if (F_e_way_status == true)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Already Canceled ..!!');", true);
            }
            else
            {
                SqlCommand cmdGetEwbNo = new SqlCommand("select EwbNo from tblcreditdebitnotehdr where Id='" + lblHiddenid.Text + "'", con);
                Object F_GetEwbNo = cmdGetEwbNo.ExecuteScalar();

                // Cancellation start from Eway API
                string Status_Desc = "Invalid Token";
                string Status_CD = "";
                string CancelDate = "";

                // if (!string.IsNullOrWhiteSpace(AuthToken))
                // {
                string strUrl = string.Format("https://api.mastergst.com/ewaybillapi/v1.03/ewayapi/canewb?email=erp%40weblinkservices.net");

                WebRequest requestObjPost = WebRequest.Create(strUrl);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";
                requestObjPost.Headers.Add("email", MailID);
                requestObjPost.Headers.Add("ip_address", IPAddress);
                requestObjPost.Headers.Add("client_id", E_Way_Client_ID);
                requestObjPost.Headers.Add("client_secret", E_Way_Secret);
                requestObjPost.Headers.Add("username", UserName);
                requestObjPost.Headers.Add("gstin", GST);

                string postData = "{\r\n  \"ewbNo\": " + lblEwaybillno.Text + ",\r\n  \"cancelRsnCode\": " + ddlcnlReason.SelectedValue + ",\r\n  \"cancelRmrk\": \'" + txtcnlRemark.Text + "'\r\n}";

                using (var streamwriter = new StreamWriter(requestObjPost.GetRequestStream()))
                {
                    streamwriter.Write(postData);
                    streamwriter.Flush();
                    streamwriter.Close();
                    var httpResponse = (HttpWebResponse)requestObjPost.GetResponse();
                    using (var streamreader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result2 = streamreader.ReadToEnd();
                        var JsonRespons = JObject.Parse(result2);
                        Status_Desc = JsonRespons["status_desc"].ToString();
                        Status_CD = JsonRespons["status_cd"].ToString();

                        if (Status_CD == "1")
                        {
                            //// Deserialize the JSON string into a dynamic object
                            //dynamic jsonObject = JsonConvert.DeserializeObject(result2);
                            //// Access the "VehiclListDetails" array and get the first element
                            //JToken data = jsonObject["data"];
                            //if (data == null || !data.HasValues)
                            //{
                            //    //vehicleNo = "";
                            //    //transDocNo = "";
                            //    ////string transDocDate1 = vehicleDetails.transDocDate;
                            //    //transDocDate = F_EwbDtt;
                            //    //transMode = "1";
                            //}
                            //else
                            //{
                            //    dynamic CancelData = jsonObject.data[0];
                            //    // Access the "vehicleNo" property
                            //    CancelDate = CancelData.cancelDate;
                            //}                                                                                   
                            //CancelDate = JsonRespons["cancelDate"].ToString();


                            // Deserialize the JSON string into a dynamic object
                            dynamic jsonObject = JsonConvert.DeserializeObject(result2);
                            // Access the "data" object
                            dynamic data = jsonObject["data"];
                            if (data == null)
                            {
                            }
                            else
                            {
                                CancelDate = data.cancelDate;
                            }
                        }
                    }
                }
                //}
                //Return Value     
                _Messege = Status_Desc;
                _CancelDate = CancelDate;
                _Status_CD = Status_CD;

                //
                if (_Status_CD == "0")
                {
                    con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + _Messege + "');", true);
                }
                else
                {
                    Cmd = new SqlCommand("UPDATE tblcreditdebitnotehdr SET e_way_cancel_status=@e_way_cancel_status,e_way_cancel_date=@e_way_cancel_date,e_way_cancel_by=@e_way_cancel_by WHERE Id=" + lblHiddenid.Text + "", con);
                    Cmd.Parameters.AddWithValue("@e_way_cancel_status", 1);
                    Cmd.Parameters.AddWithValue("@e_way_cancel_date", DateTime.Now);
                    Cmd.Parameters.AddWithValue("@e_way_cancel_by", Session["Username"].ToString());
                    Cmd.ExecuteNonQuery();
                    con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Cancelled Successfully...!!');window.location.href='EWayBillList_CrDbNote.aspx';", true);
                }
            }
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void btncnlreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("EWayBillList_CrDbNote.aspx");
    }

    private void GenerateQR_Eway(string EwbNo, string GST, string EwbDt)
    {
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        string QR_String = EwbNo + "/" + GST + "/" + EwbDt;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img_Eway_CRDBNote.jpg");
        var barcodeBitmap = new System.Drawing.Bitmap(result);

        using (MemoryStream memory = new MemoryStream())
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                barcodeBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] bytes = memory.ToArray();
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        //img_QrCode.Visible = true;
        img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img_Eway_CRDBNote.jpg";
    }

    private void GenerateBarcode_Eway(string EwbNo)
    {
        // The string value you want to encode as a barcode
        string data = EwbNo;

        // Generate the barcode image
        BarcodeWriter barcodeWriter = new BarcodeWriter();
        barcodeWriter.Format = BarcodeFormat.CODE_128; // You can choose other barcode formats as needed
        EncodingOptions encodingOptions = new EncodingOptions
        {
            Width = 250, // Set the width of the barcode image
            Height = 65 // Set the height of the barcode image
        };
        barcodeWriter.Options = encodingOptions;
        System.Drawing.Bitmap barcodeBitmap = barcodeWriter.Write(data);

        // Convert the System.Drawing.Bitmap to a byte array
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        {
            barcodeBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] barcodeBytes = ms.ToArray();

            // Save the barcode image to a folder on the server
            string folderPath = Server.MapPath("~/E_Inv_QrCOde/"); // Change this to your desired folder path
            string fileName = "Barcode_Img_Eway_CRDBNote.png"; // Generate a unique filename
            string filePath = System.IO.Path.Combine(folderPath, fileName);
            barcodeBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            // Display the barcode image on the web page
            imgBarcode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(barcodeBytes);
            imgBarcode.Visible = false;
        }
    }

    protected void EwayPdf()
    {
        // string id = Decrypt(Request.QueryString["Id"].ToString());
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from tblcreditdebitnotehdr where Id = '" + lblHiddenid.Text + "'", con);
        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
        //string Path = ;
        string billingCustomer = Dt.Rows[0]["SupplierName"].ToString();
        //05-06-2022
        string InvoiceNo = Dt.Rows[0]["DocNo"].ToString();
        string stringInvoiceNo = InvoiceNo.Replace("/", "-");
        string Docname =   stringInvoiceNo + "_E-WayBill.pdf";

        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + Docname, FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();
        string imageURL = Server.MapPath("~") + "/E_Inv_QrCOde/QR_Img_Eway_CRDBNote.jpg";
        string imageURLbarcode = Server.MapPath("~") + "/E_Inv_QrCOde/Barcode_Img_Eway_CRDBNote.png";
        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
        iTextSharp.text.Image png1 = iTextSharp.text.Image.GetInstance(imageURLbarcode);
        //Resize image depend upon your need
        png.ScaleToFit(115, 115);
        png1.ScaleToFit(125, 125);
        //For Image Position
        png.SetAbsolutePosition(240, 715);
        png1.SetAbsolutePosition(245, 200);
        //var document = new Document();
        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;
        png1.SpacingBefore = 50f;
        //Give some space after the image
        png.SpacingAfter = 1f;
        png1.SpacingAfter = 1f;
        png.Alignment = Element.ALIGN_CENTER;
        png1.Alignment = Element.ALIGN_CENTER;
        doc.Add(png);
        doc.Add(png1);

        if (Dt.Rows.Count > 0)
        {
            con.Open();
            string EwbNo = Dt.Rows[0]["EwbNo"].ToString();
            //string EwbDt = Dt.Rows[0]["EwbDt"].ToString();            
            DateTime EwbDt = Convert.ToDateTime(Dt.Rows[0]["EwbDt"].ToString());
            string F_EwbDt = EwbDt.ToString("dd-MM-yyyy hh:mm tt");
            DateTime EwbValidTill = Convert.ToDateTime(Dt.Rows[0]["EwbValidTill"].ToString());
            string F_EwbValidTill = EwbValidTill.ToString("dd-MM-yyyy");
            string Irn = Dt.Rows[0]["Irn"].ToString();
            string AckNo = Dt.Rows[0]["AckNo"].ToString();
            DateTime AckDt = Convert.ToDateTime(Dt.Rows[0]["AckDt"].ToString());
            string F_AckDt = EwbDt.ToString("dd-MM-yyyy hh:mm tt");

            SqlCommand cmdGethsn = new SqlCommand("select TOP 1 HSN from tblCreditDebitNoteDtls where HeaderID='" + lblHiddenid.Text + "'", con);
            string HSNcode = cmdGethsn.ExecuteScalar().ToString();

            #region get Info from Eway bill API    
            CheckAuthToken();
            // get Info from Eway bill API
            string uri1 = "https://api.mastergst.com/ewaybillapi/v1.03/ewayapi/getewaybill?email=erp%40weblinkservices.net&ewbNo=";
            string uri = uri1 + EwbNo;
            WebResponse response;
            WebRequest request = WebRequest.Create(uri);
            con.Close();

            request.Method = "GET";
            request.Headers.Add("email", MailID);
            request.Headers.Add("ewbNo", EwbNo);
            request.Headers.Add("ip_address", IPAddress);
            request.Headers.Add("client_id", E_Way_Client_ID);
            request.Headers.Add("client_secret", E_Way_Secret);
            request.Headers.Add("gstin", GST);
            response = request.GetResponse();

            string actualDist = "";
            string totInvValue = "";
            string toGstin = "";
            string toTrdName = "";
            string toPlace = "";
            string toPincode = "";
            string toStateCode = "";
            string docNo = "";
            string docDate = "";
            string transactionType = "";
            string subSupplyType = "";
            string transporterId = "";
            string transporterName = "";
            string vehicleNo = "";
            string transDocDate = "";
            string transDocNo = "";
            string transMode = "";

            using (Stream dataStream = response.GetResponseStream())
            {
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                var myJsonString = responseFromServer;
                var jo = JObject.Parse(myJsonString);

                string Status_Desc = jo["status_desc"].ToString();
                string Status_CD = jo["status_cd"].ToString();

                if (Status_CD != "0")
                {
                    actualDist = jo["data"]["actualDist"].ToString();
                    totInvValue = jo["data"]["totInvValue"].ToString();
                    toGstin = jo["data"]["toGstin"].ToString();
                    toTrdName = jo["data"]["toTrdName"].ToString();
                    toPlace = jo["data"]["toPlace"].ToString();
                    toPincode = jo["data"]["toPincode"].ToString();
                    toStateCode = jo["data"]["toStateCode"].ToString();
                    docNo = jo["data"]["docNo"].ToString();
                    docDate = jo["data"]["docDate"].ToString();
                    transactionType = jo["data"]["transactionType"].ToString();
                    subSupplyType = jo["data"]["subSupplyType"].ToString();
                    transporterId = jo["data"]["transporterId"].ToString();
                    transporterName = jo["data"]["transporterName"].ToString();

                    // Deserialize the JSON string into a dynamic object
                    dynamic jsonObject = JsonConvert.DeserializeObject(myJsonString);
                    // Access the "VehiclListDetails" array and get the first element
                    dynamic vehicleDetails = jsonObject.data.VehiclListDetails[0];
                    // Access the "vehicleNo" property
                    vehicleNo = vehicleDetails.vehicleNo;
                    transDocNo = vehicleDetails.transDocNo;
                    string transDocDate1 = vehicleDetails.transDocDate;
                    transDocDate = transDocDate1.Replace("/", "-");
                    transMode = vehicleDetails.transMode;


                }
            }
            //
            #endregion           

            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(28f, -100f, 560f, 80f);
            cb.Stroke();
            // Header 
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 17);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "e-Way Bill", 262, 815, 0);
            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Gat No. 1567, Shelar Vasti, Dehu-Alandi Road, Chikhali, Pune - 411062", 170, 785, 0);         
            cb.EndText();

            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 20f;

            PdfPTable table = new PdfPTable(2);

            //float[] widths2 = new float[] { 100, 240, 100, 100 };
            float[] widths2 = new float[] { 120, 440 };
            table.SetWidths(widths2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            //string datee = ffff1.ToString("yyyy-MM-dd");
            //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.DefaultCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;

            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("        E-Way Bill No : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table.AddCell(new Phrase(EwbNo + "\n", FontFactory.GetFont("Arial", 11, Font.BOLD)));

            table.AddCell(new Phrase("        E-Way Bill Date : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table.AddCell(new Phrase(F_EwbDt, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table.AddCell(new Phrase("        Generated By : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table.AddCell(new Phrase("27ABCCS7002A1ZW    Pune Abrasives Pvt. Ltd.", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table.AddCell(new Phrase("        Valid From : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table.AddCell(new Phrase(F_EwbDt + " [ " + actualDist + "KM ]", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table.AddCell(new Phrase("        Valid Until : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table.AddCell(new Phrase(F_EwbValidTill, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            paragraphTable1.Add(table);
            doc.Add(paragraphTable1);

            // Create a PdfContentByte object for drawing
            PdfContentByte contentByte = writer.DirectContent;
            // Set line properties
            float lineThickness = 0.5F; // Adjust the line thickness as needed
            BaseColor lineColor = BaseColor.BLACK;
            // Set the position and dimensions of the line
            float xPosition = 50f; // X-coordinate of the starting point
            float yPosition = 640f; // Y-coordinate of the starting point
            float lineLength = 500f; // Length of the line
            // Draw the line
            contentByte.SetLineWidth(lineThickness);
            contentByte.SetColorStroke(lineColor);
            contentByte.MoveTo(xPosition, yPosition);
            contentByte.LineTo(xPosition + lineLength, yPosition);
            contentByte.Stroke();

            PdfContentByte cbb = writer.DirectContent;
            cbb.Rectangle(28f, -100f, 560f, 80f);
            cbb.Stroke();
            // Header
            cbb.BeginText();
            cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 12);
            cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "IRN Details", 52, 623, 0);
            cbb.EndText();

            Paragraph paragraphTable2 = new Paragraph();
            paragraphTable2.SpacingBefore = 1f;
            paragraphTable2.SpacingAfter = 20f;

            PdfPTable table1 = new PdfPTable(2);

            //float[] widths2 = new float[] { 100, 240, 100, 100 };
            float[] widths22 = new float[] { 120, 440 };
            table1.SetWidths(widths22);
            table1.TotalWidth = 560f;
            table1.LockedWidth = true;

            //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            //string datee = ffff1.ToString("yyyy-MM-dd");
            //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.DefaultCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;

            table1.DefaultCell.Border = Rectangle.NO_BORDER;

            table1.AddCell(new Phrase("        IRN : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table1.AddCell(new Phrase(Irn, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table1.AddCell(new Phrase("        Ack No : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table1.AddCell(new Phrase(AckNo, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table1.AddCell(new Phrase("        Ack Date : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table1.AddCell(new Phrase(F_AckDt, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            paragraphTable2.Add(table1);
            doc.Add(paragraphTable2);

            // Create a PdfContentByte object for drawing
            PdfContentByte contentByte1 = writer.DirectContent;
            // Set line properties
            float lineThickness1 = 0.5F; // Adjust the line thickness as needed
            BaseColor lineColor1 = BaseColor.BLACK;
            // Set the position and dimensions of the line
            float xPosition1 = 50f; // X-coordinate of the starting point
            float yPosition1 = 554f; // Y-coordinate of the starting point
            float lineLength1 = 500f; // Length of the line
            // Draw the line
            contentByte1.SetLineWidth(lineThickness1);
            contentByte1.SetColorStroke(lineColor1);
            contentByte1.MoveTo(xPosition1, yPosition1);
            contentByte1.LineTo(xPosition1 + lineLength1, yPosition1);
            contentByte1.Stroke();

            PdfContentByte cbbb = writer.DirectContent;
            cbbb.Rectangle(28f, -100f, 560f, 80f);
            cbbb.Stroke();
            // Header
            cbbb.BeginText();
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 12);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Part - A", 52, 538, 0);
            cbbb.EndText();

            Paragraph paragraphTable3 = new Paragraph();
            paragraphTable3.SpacingBefore = 1f;
            paragraphTable3.SpacingAfter = 20f;

            PdfPTable table2 = new PdfPTable(2);

            //float[] widths2 = new float[] { 100, 240, 100, 100 };
            float[] widths23 = new float[] { 120, 440 };
            table2.SetWidths(widths23);
            table2.TotalWidth = 560f;
            table2.LockedWidth = true;

            con.Open();
            SqlCommand cmdcheck = new SqlCommand("select StateName from tbl_States where StateCode='" + toStateCode + "'", con);
            string stateName = cmdcheck.ExecuteScalar().ToString();

            table2.DefaultCell.Border = Rectangle.NO_BORDER;

            table2.AddCell(new Phrase("        GSTIN of Supplier : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase("27ABCCS7002A1ZW   \nPune Abrasives Pvt. Ltd.", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table2.AddCell(new Phrase("        Place of Dispatch : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase("Pune MAHARASHTRA 411062", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table2.AddCell(new Phrase("        GSTIN of Recipient : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(toGstin + "\n" + toTrdName, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table2.AddCell(new Phrase("        Place of Delivery : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(toPlace + " " + stateName + " " + toPincode, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table2.AddCell(new Phrase("        Document No. : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(docNo, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table2.AddCell(new Phrase("        Document Date : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(docDate, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            if (transactionType == "1")
            {
                table2.AddCell(new Phrase("        Transaction Type : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                table2.AddCell(new Phrase("Regular", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            }
            else if (transactionType == "2")
            {
                table2.AddCell(new Phrase("        Transaction Type : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                table2.AddCell(new Phrase("Bill To-Ship To", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            }

            table2.AddCell(new Phrase("        Value of \n        Goods : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(totInvValue, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            table2.AddCell(new Phrase("        HSN Code : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(HSNcode, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            if (subSupplyType == "1  ")
            {
                table2.AddCell(new Phrase("        Reason for \n        Transportation : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                table2.AddCell(new Phrase("Outward - Supply", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            }
            else if (subSupplyType == "2  ")
            {
                table2.AddCell(new Phrase("        Reason for \n        Transportation : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                table2.AddCell(new Phrase("Outward - Export", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            }

            table2.AddCell(new Phrase("        Transporter : ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
            table2.AddCell(new Phrase(transporterId + " - " + transporterName, FontFactory.GetFont("Arial", 10, Font.NORMAL)));

            paragraphTable3.Add(table2);
            doc.Add(paragraphTable3);

            // Create a PdfContentByte object for drawing
            PdfContentByte contentByte11 = writer.DirectContent;
            // Set line properties
            float lineThickness11 = 0.5F; // Adjust the line thickness as needed
            BaseColor lineColor11 = BaseColor.BLACK;
            // Set the position and dimensions of the line
            float xPosition11 = 50f; // X-coordinate of the starting point
            float yPosition11 = 330f; // Y-coordinate of the starting point
            float lineLength11 = 500f; // Length of the line
            // Draw the line
            contentByte11.SetLineWidth(lineThickness11);
            contentByte11.SetColorStroke(lineColor11);
            contentByte11.MoveTo(xPosition11, yPosition11);
            contentByte11.LineTo(xPosition11 + lineLength11, yPosition11);
            contentByte11.Stroke();

            PdfContentByte cbbb1 = writer.DirectContent;
            cbbb1.Rectangle(28f, -100f, 560f, 80f);
            cbbb1.Stroke();
            // Header
            cbbb1.BeginText();
            cbbb1.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 12);
            cbbb1.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Part - B", 52, 313, 0);
            cbbb1.EndText();

            /// new code starts
            // footer Details Table Start 
            Paragraph paragraphTable12 = new Paragraph();
            paragraphTable12.SpacingAfter = 1f;
            paragraphTable12.SpacingBefore = 1f;


            table = new PdfPTable(7);
            //float[] widths32 = new float[] { 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f };
            float[] widths32 = new float[] { 11f, 11f, 9f, 11f, 13f, 11f, 11f };
            table.SetWidths(widths32);

            if (Dt.Rows.Count > 0)
            {
                //table.TotalWidth = 560f;
                table.TotalWidth = 510f;
                table.LockedWidth = true;
                table.AddCell(new Phrase("         Mode", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(" Vehicle / Trans \n   Doc No & Dt.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("       From", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   Entered Date", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("      Entered By", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("     CEWB No. \n        (If any)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(" Multi Veh. Info \n         (If any)", FontFactory.GetFont("Arial", 9, Font.BOLD)));

                //table.TotalWidth = 560f;
                table.TotalWidth = 510f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;



                //for transMode
                if (transMode == "1")
                {
                    table.AddCell(new Phrase("Road", FontFactory.GetFont("Arial", 9)));
                }
                else if (transMode == "2")
                {
                    table.AddCell(new Phrase("Rail", FontFactory.GetFont("Arial", 9)));
                }
                else if (transMode == "3")
                {
                    table.AddCell(new Phrase("Air", FontFactory.GetFont("Arial", 9)));
                }
                else if (transMode == "4")
                {
                    table.AddCell(new Phrase("Ship or Ship Cum Road/Rail", FontFactory.GetFont("Arial", 9)));
                }

                //for vehicleNo & transDocNo & transDocDate
                if (vehicleNo != "" && transDocNo != "")
                {
                    table.AddCell(new Phrase(vehicleNo + " / " + transDocNo + " & " + transDocDate, FontFactory.GetFont("Arial", 9)));
                }
                else if (vehicleNo != "" && transDocNo == "")
                {
                    table.AddCell(new Phrase(vehicleNo + " & " + transDocDate, FontFactory.GetFont("Arial", 9)));
                }
                else if (vehicleNo == "" && transDocNo == "")
                {
                    table.AddCell(new Phrase(transDocDate, FontFactory.GetFont("Arial", 9)));
                }

                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(transDocDate, FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase("27ABCCS7002A1ZW", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(" -", FontFactory.GetFont("Arial", 9)));
                table.AddCell(new Phrase(" -", FontFactory.GetFont("Arial", 9)));


            }

            paragraphTable12.Add(table);
            doc.Add(paragraphTable12);
            ///End Description table
            //Space
            Paragraph paragraphTable32 = new Paragraph();

            string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font12 = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Font font10 = FontFactory.GetFont("Arial", 9, Font.BOLD);
            Paragraph paragraph = new Paragraph("", font12);

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font12));
            }


            table = new PdfPTable(7);
            //table.TotalWidth = 560f;
            table.TotalWidth = 510f;
            table.LockedWidth = true;
            table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.SetWidths(new float[] { 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f, 11f });
            table.SetWidths(new float[] { 11f, 11f, 9f, 11f, 13f, 11f, 11f });
            table.AddCell(paragraph);
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));


            doc.Add(table);


            ////calculation supply
            ///value of supply
            Paragraph paragraphTable51 = new Paragraph();

            string[] itemsss1 = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font131 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font111 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphh1 = new Paragraph("", font111);



            for (int i = 0; i < items.Length; i++)
            {
                paragraphh1.Add(new Phrase("", font10));
            }




            //if (Dt.Rows[0]["IGSTPer"].ToString() == "0")
            //{
            //    CGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(CGSTPer.Trim())) / 100;
            //    SGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(SGSTPer.Trim())) / 100;
            //}
            //else
            //{
            //    IGSTAmount = Convert.ToDouble(ValueOFSupply) * (Convert.ToDouble(IGSTPer.Trim())) / 100;
            //}

            //Declaration
            Paragraph paragraphTable99 = new Paragraph(" Remarks :\n\n", font12);

            //Puja Enterprises Sign
            string[] itemss = {
                "Declaration  : I/We hereby certify that my/our registration certificate under the GST Act, 2017 is in force on the date on which the supply of",
                "the goods specified in this tax invoice is made by me/us and that the transaction of supplies covered by this tax invoice has been effected",
                "by me/us and it shall be accounted for in the turnover of supplies while filing of return and the due tax, if any, payable on the supplies",
                "has been paid or shall be paid. \n",

                "Subject To Pune Jurisdiction Only.",

                        };

            Font font14 = FontFactory.GetFont("Arial", 9);
            Font font15 = FontFactory.GetFont("Arial", 9, Font.NORMAL);
            Paragraph paragraphhh = new Paragraph(" Terms & Condition :\n\n", font15);


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
            ///end declaration
            ///

            ///Sign Authorization
            ///

            // Bind stamp Image
            string imageStamp = Server.MapPath("~") + "/Content/img/Account.png";
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imageStamp);
            image1.ScaleToFit(600, 120);
            PdfPCell imageCell = new PdfPCell(image1);
            imageCell.PaddingLeft = 10f;
            imageCell.PaddingTop = 0f;
            /////////////////

            Paragraph paragraphTable10000 = new Paragraph();


            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();
            PdfPTable table25 = new PdfPTable(1);
            table25.TotalWidth = 510f;
            table25.LockedWidth = true;
            table25.SetWidths(new float[] { 400f });

            // Create a cell for the image
            PdfPCell cell = new PdfPCell();
            // Add the cell to the table
            table25.AddCell(cell);
            // Add content to the paragraph
            // Add the table to the paragraph
            paragraphhhhhff.Add(table25);
            // Add the paragraph to the document
            doc.Add(paragraphhhhhff);
        }
        doc.Close();
        Iframe1.Attributes["src"] = @"../PDF_Files/" + Docname;
    }

    #region E-Way Bill API - Generate Eway Bill, AuthToken Developed on 4-1-24
    protected void GenerateEWayBill_ByEwayApi()
    {
        try
        {
            con.Open();
            CheckAuthToken();
            DataTable dtCompany = new DataTable();
            SqlDataAdapter sadCompany = new SqlDataAdapter("select * from [tblCreditDebitNoteHdr] where Id='" + lblHiddenid.Text + "'", con);
            sadCompany.Fill(dtCompany);

            // Code Start For Generation
            string Status_Desc = "";
            string Status_CD = "";
            string EwbNo = "";
            string EwbDt = "";
            string EwbValidTill = "";

            //string strUrl = string.Format("https://api.mastergst.com/einvoice/type/GENERATE_EWAYBILL/version/V1_03?email=erp%40weblinkservices.net");  //E-Invoice API
            string strUrl = string.Format("https://api.mastergst.com/ewaybillapi/v1.03/ewayapi/genewaybill?email=erp%40weblinkservices.net");  //E-Way Bill API
            WebRequest requestObjPost = WebRequest.Create(strUrl);
            requestObjPost.Method = "POST";
            requestObjPost.ContentType = "application/json";
            requestObjPost.Headers.Add("ip_address", IPAddress);
            requestObjPost.Headers.Add("client_id", E_Way_Client_ID);
            requestObjPost.Headers.Add("client_secret", E_Way_Secret);
            //requestObjPost.Headers.Add("username", UserName);
            requestObjPost.Headers.Add("gstin", GST);
            requestObjPost.Headers.Add("email", MailID);


            ////check Values
            con.Open();
            string ffff1 = dtCompany.Rows[0]["DocDate"].ToString();
            string ffff2 = txtdate.Text;
            string Transaction_Date = ffff1.Replace("-", "/");
            string TransactionDoc_Date = ffff2.Replace("-", "/");
            string ShippingGST = dtCompany.Rows[0]["ShippingGST"].ToString();
            string ShippingPincode = dtCompany.Rows[0]["ShippingPincode"].ToString();
            string ShippingStatecode = dtCompany.Rows[0]["ShippingStatecode"].ToString();
            string BillingGST = dtCompany.Rows[0]["BillingGST"].ToString();
            string BillingPincode = dtCompany.Rows[0]["BillingPincode"].ToString();
            string BillingStatecode = dtCompany.Rows[0]["BillingStatecode"].ToString();

            //Total Invoice Value
            SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(Total as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + lblHiddenid.Text + "'", con);
            Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
            double TotInvValuee = Convert.ToDouble(F_TotInvVal);
            string TotInvVal = TotInvValuee.ToString("0.00");

            //Total mainHsnCode Value
            //SqlCommand cmdmainHsnCode = new SqlCommand("select top 1 HSN from [tblTaxInvoiceDtls] where HeaderID='" + lblHiddenid.Text + "'", con);
            //Object F_mainHsnCode = cmdmainHsnCode.ExecuteScalar();
            //string mainHsnCode = F_mainHsnCode.ToString();
            //string postData = "";

            #region ItemList

            //Declare variables
            string HsnCd = "", Qty = "", Unit = "", UnitPrice = "", TotAmt = "", Item_Discount = "", AssAmt = "", GstRt = "",
                SgstAmt = "null", IgstAmt = "null", CgstAmt = "null", IgstVal = "null", CgstVal = "null", SgstVal = "null", TotInvValFc = "", TotItemVal = "", IsServc = "";
            //Fright Charges Check Start
            DataTable dt = new DataTable();
            SqlCommand cmdFright = new SqlCommand("select Basic from [tblCreditDebitNoteHdr] where id='" + lblHiddenid.Text + "'", con);
            Object F_Fright = cmdFright.ExecuteScalar();
            string FrightValuee = F_Fright.ToString();
            if (FrightValuee == "0")
            {
                //DataTable dt = new DataTable();
                SqlDataAdapter sad = new SqlDataAdapter("select * from tblCreditDebitNotedtls where HeaderId='" + lblHiddenid.Text + "'", con);
                sad.Fill(dt);
            }
            else
            {
                //DataTable dt = new DataTable();
                SqlDataAdapter sad = new SqlDataAdapter("select * from VW_EINVTransport_CrDbNote_Sale where HeaderId='" + lblHiddenid.Text + "'", con);
                sad.Fill(dt);
            }

            StringBuilder itemlist = new StringBuilder();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                HsnCd = dt.Rows[i]["HSN"].ToString();
                if (string.IsNullOrWhiteSpace(HsnCd))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Hsn Not Set Please Enter Hsn....!!');", true);
                }
                Qty = dt.Rows[i]["Qty"].ToString();
                if (string.IsNullOrWhiteSpace(Qty))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Qty Not Set Please Enter Qty....!!');", true);
                }
                Unit = dt.Rows[i]["UOM"].ToString();
                if (string.IsNullOrWhiteSpace(Unit))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Unit Not Set Please Enter Unit....!!');", true);
                }
                object Obj_UnitPrice = dt.Rows[i]["Rate"].ToString();
                double UnitPriceValuee = Convert.ToDouble(Obj_UnitPrice);
                UnitPrice = UnitPriceValuee.ToString("0.00");
                if (string.IsNullOrWhiteSpace(UnitPrice))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('UnitPrice Not Set Please Enter UnitPrice....!!');", true);
                }
                double Basic = Convert.ToDouble(UnitPrice) * Convert.ToDouble(Qty);
                TotAmt = Basic.ToString("0.00");
                if (string.IsNullOrWhiteSpace(TotAmt))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Basic Not Set Please Enter Basic....!!');", true);
                }
                object Obj_AssAmt = dt.Rows[i]["Amount"].ToString();
                double AssAmtValuee = Convert.ToDouble(Obj_AssAmt);
                AssAmt = AssAmtValuee.ToString("0.00");
                if (string.IsNullOrWhiteSpace(AssAmt))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Taxable Value Not Set Please Enter Taxable Value....!!');", true);
                }
                object Obj_SgstAmt = dt.Rows[i]["CGSTPer"].ToString();
                double SgstAmtValuee = Convert.ToDouble(Obj_SgstAmt);
                SgstAmt = SgstAmtValuee.ToString("0.00");
                if (string.IsNullOrWhiteSpace(SgstAmt))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('SgstAmt Not Set Please Enter SgstAmt...!!');", true);
                }
                object Obj_IgstAmt = dt.Rows[i]["IGSTPer"].ToString();
                double IgstAmtValuee = Convert.ToDouble(Obj_IgstAmt);
                IgstAmt = IgstAmtValuee.ToString("0.00");
                if (string.IsNullOrWhiteSpace(IgstAmt))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('IgstAmt Not Set Please Enter IgstAmt...!!');", true);
                }
                object Obj_CgstAmt = dt.Rows[i]["CGSTPer"].ToString();
                double CgstAmtValuee = Convert.ToDouble(Obj_CgstAmt);
                CgstAmt = CgstAmtValuee.ToString("0.00");
                if (string.IsNullOrWhiteSpace(CgstAmt))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('CgstAmt Not Set Please Enter CgstAmt...!!');", true);
                }

                //{\"SlNo\":\"1\",\"IsServc\":\"N\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}
                int cnt = i + 1;
                //itemlist.Append("{\"SlNo\":\"" + cnt + "\",\"IsServc\":\"" + IsServc + "\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}");
                //itemlist.Append("{\n\"hsnCode\": " + HsnCd + ",\n\"taxableAmount\": " + AssAmt + "\n }");
                itemlist.Append("{\n\"hsnCode\": " + HsnCd + ",\n\"taxableAmount\": " + AssAmt + "\n,\n\"sgstRate\": " + SgstAmt + "\n,\n\"cgstRate\": " + CgstAmt + "\n,\n\"igstRate\": " + IgstAmt + "\n }");
                itemlist.Append(",");
            }
            //}

            char[] charsToTrim = { ',', ' ' };
            string list = itemlist.ToString().Trim().TrimEnd(charsToTrim);


            #endregion
            string postData1 = "";

            #region New PostData
            if (txtTransporterID.Text == "" && txtTransporterName.Text == "")
            {
                if (txtVehicleNumber.Text == "")
                {//before 30-1023
                 //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                 //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed
                 //if (BillingPincode == ShippingPincode)
                    if (BillingPincode == "411062")
                    {
                        //postData1 = "{ \"docType\":BIL,\"docNo\":" + lblInvoiceNo.Text + ",\"docDate\":" + lblInvoiceDate.Text + ",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"" + txtDistance.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                        postData1 = "{ \"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"" + txtDistance.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                    }
                    else
                    {
                        postData1 = "{ \"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"0\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                    }
                }
                else
                {//before 30-1023
                 //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                 //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                    //if (BillingPincode == ShippingPincode)
                    if (BillingPincode == "411062")
                    {
                        //postData = "{ \"docType\":BIL,\"docNo\":" + lblInvoiceNo.Text + ",\"docDate\":" + lblInvoiceDate.Text + ",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":" + txtDistance.Text + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                        postData1 = "{ \"docType\":\"\"BIL\"\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDistance\":\"" + txtDistance.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                    }
                    else
                    {
                        //postData = "{ \"docType\":BIL,\"docNo\":" + lblInvoiceNo.Text + ",\"docDate\":" + lblInvoiceDate.Text + ",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":0,\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                        postData1 = "{ \"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDistance\":\"0\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + ",\n\"itemList\": [" + list + "]\n}";
                    }
                }
            }
            else
            {
                if (txtVehicleNumber.Text == "")
                {//before 30-1023
                 //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                 //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                    //if (BillingPincode == ShippingPincode)
                    if (BillingPincode == "411062")
                    {
                        postData1 = "{ \"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"" + txtDistance.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n}";
                    }
                    else
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"0\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n}";
                    }
                }
                else if (txtVehicleNumber.Text != "" && txtTransporterDocNo.Text != "")
                {//before 30-1023
                 //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                 //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                    //if (BillingPincode == ShippingPincode)
                    if (BillingPincode == "411062")
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"" + txtDistance.Text + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n";
                    }
                    else
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"0\",\"transDocNo\":\"" + txtTransporterDocNo.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n";
                    }
                }
                else if (ddlTransportationMode.SelectedValue == "0")
                {//before 30-1023
                 //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                 //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                    //if (BillingPincode == ShippingPincode)
                    if (BillingPincode == "411062")
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"" + txtDistance.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n";
                    }
                    else
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"0\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n";
                    }
                }
                else if (txtTransporterID.Text != "" && txtVehicleNumber.Text != "" && txtTransporterDocNo.Text == "")
                {//before 30-1023
                 //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                 //After 30-1023   \"transType\":" + ddlTransactionType.SelectedValue + ", removed

                    //if (BillingPincode == ShippingPincode)
                    if (BillingPincode == "411062")
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"" + txtDistance.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n";
                    }
                    else
                    {
                        postData1 = "{\"docType\":\"BIL\",\"docNo\":\"" + lblInvoiceNo.Text + "\",\"docDate\":\"" + lblInvoiceDate.Text + "\",\"transactionType\":" + ddlTransactionType.SelectedValue + ",\"transDocNo\":" + txtTransporterDocNo.Text + ",\"transDistance\":\"0\",\"TransId\":\"" + txtTransporterID.Text + "\",\"vehicleType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"vehicleNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":\"" + ddlSubType.SelectedValue + "\",\"transDocDate\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "},\n\"itemList\": [" + list + "]\n";
                    }
                }
                //else
                //{
                //    postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                //}
            }
            #endregion

            //if (txtTransporterID.Text == "")
            //{
            //    if (txtVehicleNumber.Text == "")
            //    {
            //        //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
            //        //
            //        //postData = "{\n\"supplyType\": \"O\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";


            //        postData = "{\n\"supplyType\": \"O\",\n\"transMode\": \"" + ddlTransportationMode.SelectedValue + "\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";
            //    }
            //    else
            //    {
            //        //main testing
            //        //postData = "{\n\"supplyType\": \"O\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transporterId\": \"" + txtTransporterID.Text + "\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";
            //        //postData = "{\n\"supplyType\": \"O\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",,\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";


            //        postData = "{\n\"supplyType\": \"O\",\n\"transMode\": \"" + ddlTransportationMode.SelectedValue + "\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";
            //    }
            //}
            //else
            //{
            //    if (ddlTransportationMode.SelectedValue == "0")
            //    {
            //        postData = "{\n\"supplyType\": \"O\",\n\"transMode\": \"" + ddlTransportationMode.SelectedValue + "\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";

            //        //postData = "{\n\"supplyType\": \"O\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transporterId\": \"" + txtTransporterID.Text + "\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";
            //        ////old
            //        //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
            //    }
            //    else
            //    {
            //        postData = "{\n\"supplyType\": \"O\",\n\"transMode\": \"" + ddlTransportationMode.SelectedValue + "\",\n\"subSupplyType\": \"" + ddlSubType.SelectedValue + "\",\n\"docType\": \"INV\",\n  \"docNo\": \"" + lblInvoiceNo.Text + "\",\n\"docDate\": \"" + lblInvoiceDate.Text + "\",\n\"fromGstin\": \"" + GST + "\",\n\"actFromStateCode\": 27,\n\"fromPincode\": 411062,\n\"fromStateCode\": 27,\n\"toGstin\": \"" + ShippingGST + "\",\n\"toPincode\": " + ShippingPincode + ",\n\"actToStateCode\": " + ShippingStatecode + ",\n\"toStateCode\": " + ShippingStatecode + ",\n\"transactionType\": " + ddlTransactionType.SelectedValue + ",\n\"totInvValue\": " + TotInvVal + ",\n\"transDistance\": \"0\",\n\"transDocNo\": \"" + txtTransporterDocNo.Text + "\",\n  \"transDocDate\": \"" + TransactionDoc_Date + "\",\n  \"vehicleNo\": \"" + txtVehicleNumber.Text + "\",\n  \"vehicleType\": \"" + ddlVehicleType.SelectedValue + "\",\n\"itemList\": [" + list + "]\n}";

            //        //postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"transType\":" + ddlTransactionType.SelectedValue + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411062,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
            //    }
            //}

            using (var streamwriter = new StreamWriter(requestObjPost.GetRequestStream()))
            {
                streamwriter.Write(postData1);
                streamwriter.Flush();
                streamwriter.Close();
                var httpResponse = (HttpWebResponse)requestObjPost.GetResponse();
                using (var streamreader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result2 = streamreader.ReadToEnd();

                    var JsonRespons = JObject.Parse(result2);
                    Status_Desc = JsonRespons["status_desc"].ToString();
                    Status_CD = JsonRespons["status_cd"].ToString();

                    if (Status_CD == "1")
                    {
                        //EwbNo = JsonRespons["data"]["EwbNo"].ToString();
                        //EwbDt = JsonRespons["data"]["EwbDt"].ToString();
                        //EwbValidTill = JsonRespons["data"]["EwbValidTill"].ToString();

                        EwbNo = JsonRespons["data"]["ewayBillNo"].ToString();
                        EwbDt = JsonRespons["data"]["ewayBillDate"].ToString();
                        EwbValidTill = JsonRespons["data"]["validUpto"].ToString();
                    }
                }
            }

            //Return Value             
            string _Messege = Status_Desc;
            string _EwbNo = EwbNo;
            string _EwbDt = EwbDt;
            string _EwbValidTill = EwbValidTill;

            if (string.IsNullOrWhiteSpace(_EwbNo))
            {
                con.Close();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + _Messege + "');", true);
            }

            ////Update Data                
            if (_EwbNo == "" || _EwbDt == "")
            {
                con.Close();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Something Went Wrong. Kindly Create Again..!!');", true);
            }
            else
            {
                Cmd = new SqlCommand("UPDATE tblcreditdebitnotehdr SET EwbNo=@EwbNo,EwbDt=@EwbDt,EwbValidTill=@EwbValidTill,e_way_status=@e_way_status,e_way_created_by=@e_way_created_by WHERE Id=" + lblHiddenid.Text + "", con);
                Cmd.Parameters.AddWithValue("@EwbNo", _EwbNo);
                Cmd.Parameters.AddWithValue("@EwbDt", _EwbDt);
                Cmd.Parameters.AddWithValue("@EwbValidTill", _EwbValidTill);
                //Cmd.Parameters.AddWithValue("@Status", _Status);
                Cmd.Parameters.AddWithValue("@e_way_status", 1);
                Cmd.Parameters.AddWithValue("@e_way_created_by", Session["Username"].ToString());
                Cmd.ExecuteNonQuery();
                con.Close();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Genereted Sucessfully');window.location.href='EWayBillList_CrDbNote.aspx';", true);
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    private string CheckAuthToken()
    {
        string uri = "https://api.mastergst.com/ewaybillapi/v1.03/authenticate?email=erp%40weblinkservices.net&username=05AAACH6188F1ZM&password=abc123%40%40"; // For SandBox Checking

        //string uri = "https://api.mastergst.com/ewaybillapi/v1.03/authenticate?email=erp%40weblinkservices.net&username=API_ExcelEnclosures&password=ExcelEnc%40Admin%40123"; // For Production 

        WebResponse response;
        WebRequest request = WebRequest.Create(uri);
        con.Close();

        request.Method = "GET";
        request.Headers.Add("username", UserName);
        request.Headers.Add("password", Password);
        request.Headers.Add("ip_address", "103.174.254.209");
        request.Headers.Add("client_id", E_Way_Client_ID);
        request.Headers.Add("client_secret", E_Way_Secret);
        request.Headers.Add("gstin", GST);
        response = request.GetResponse();

        using (Stream dataStream = response.GetResponseStream())
        {
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();

            var myJsonString = responseFromServer;
            var jo = JObject.Parse(myJsonString);

            string Status_Desc = jo["status_desc"].ToString();
            string Status_CD = jo["status_cd"].ToString();

            //if (Status_CD != "0")
            //{
            //    TokenExpiry = jo["data"]["TokenExpiry"].ToString();
            //    Sek = jo["data"]["Sek"].ToString();
            //    ClientId = jo["data"]["ClientId"].ToString();
            //    AuthKey = jo["data"]["AuthToken"].ToString();                
            //}
            //_Messege = Status_Desc;

            return Status_Desc;
        }
    }
    #endregion
}
