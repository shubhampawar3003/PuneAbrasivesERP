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
using iTextSharp.text.pdf.parser;
using ZXing;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using ZXing.Common;
using iTextSharp.text.pdf.draw;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public partial class Admin_TaxInvoicePDF : System.Web.UI.Page
{
    //string id;
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

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
        {
            //From Credit-Debit Note
            if (Request.QueryString["Idd"] != null)
            {
                con.Open();
                string id = Decrypt(Request.QueryString["Idd"].ToString()); //Session["PDFID"].ToString();// 
                SqlCommand cmdQrdtl = new SqlCommand("select EwbNo from tblcreditdebitnotehdr where Id='" + id + "'", con);
                Object Qrdtl = cmdQrdtl.ExecuteScalar();
                string F_Qrdtl = Convert.ToString(Qrdtl);
                con.Close();
                if (F_Qrdtl != "")
                {
                    con.Open();
                    //SqlCommand cmdAckdtl = new SqlCommand("select EwbNo from tbltaxinvoicehdr where Id='" + id + "'", con);
                    //Object ACkdtl = cmdAckdtl.ExecuteScalar();
                    //string F_Ackdtl = Convert.ToString(ACkdtl);
                    DataTable Dt = new DataTable();
                    SqlDataAdapter Da = new SqlDataAdapter("select * from tblcreditdebitnotehdr where Id = '" + id + "'", con);
                    Da.Fill(Dt);
                    string EwbNo = Dt.Rows[0]["EwbNo"].ToString();
                    string date = Dt.Rows[0]["EwbDt"].ToString();
                    string EwbDt = date.Replace("/", "-");
                    con.Close();

                    GenerateQR_CrDbNote(EwbNo, GST, EwbDt);
                    GenerateBarcode_CrDbNote(EwbNo);
                    EwayPdf_CrDbNote();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Not Created. Kindly Create First..!!');", true);
                }
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

    private string GetEwayDetails(string id)
    {
        //https://api.mastergst.com/ewaybillapi/v1.03/ewayapi/getewaybill?email=erp%40weblinkservices.net&ewbNo=122318320280479
        string uri1 = "https://api.mastergst.com/ewaybillapi/v1.03/ewayapi/getewaybill?email=erp%40weblinkservices.net&ewbNo=";
        string uri2 = "https://api.mastergst.com/einvoice/authenticate?email=erp%40weblinkservices.net";
        string uri = "";
        WebResponse response;
        WebRequest request = WebRequest.Create(uri);
        con.Close();

        request.Method = "GET";
        request.Headers.Add("email", MailID);
        request.Headers.Add("ewbNo", MailID);
        request.Headers.Add("ip_address", IPAddress);
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
            }
            return AuthKey;
        }
    }

    private string CheckAuthToken()
    {

        string uri = "https://api.mastergst.com/ewaybillapi/v1.03/authenticate?email=erp%40weblinkservices.net&username=API_ExcelEnclosures&password=ExcelEnc%40Admin%40123";
        WebResponse response;
        WebRequest request = WebRequest.Create(uri);
        con.Close();

        request.Method = "GET";
        request.Headers.Add("username", UserName);
        request.Headers.Add("password", Password);
        request.Headers.Add("ip_address", IPAddress);
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

    #region From Credit Debit Note
    private void GenerateQR_CrDbNote(string EwbNo, string GST, string EwbDt)
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

    private void GenerateBarcode_CrDbNote(string EwbNo)
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

    protected void EwayPdf_CrDbNote()
    {
        string id = Decrypt(Request.QueryString["Idd"].ToString());
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from tblcreditdebitnotehdr where Id = '" + id + "'", con);
        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
        //string Path = ;
        string billingCustomer = Dt.Rows[0]["SupplierName"].ToString();
        //05-06-2022
        string InvoiceNo = Dt.Rows[0]["DocNo"].ToString();
        string stringInvoiceNo = InvoiceNo.Replace("/", "-");
        string Docname = billingCustomer + "_" + stringInvoiceNo + "_E-WayBill.pdf";

        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + Docname, FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();
        string imageURL = Server.MapPath("~") + "/E_Inv_QrCOde/QR_Img_Eway_CRDBNote.jpg";
        string imageURLbarcode = Server.MapPath("~") + "/E_Inv_QrCOde/Barcode_Img_Eway_CRDBNote.png";
        string imagecancel = Server.MapPath("~/Content/img/CancelInvoice.png");  //grey
        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
        iTextSharp.text.Image png1 = iTextSharp.text.Image.GetInstance(imageURLbarcode);
        iTextSharp.text.Image pngcancel = iTextSharp.text.Image.GetInstance(imagecancel);
        //Resize image depend upon your need
        png.ScaleToFit(115, 115);
        png1.ScaleToFit(125, 125);
        pngcancel.ScaleToFit(350, 350);
        //For Image Position
        png.SetAbsolutePosition(240, 715);
        png1.SetAbsolutePosition(245, 200);
        pngcancel.SetAbsolutePosition(115, 350);
        //var document = new Document();
        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;
        png1.SpacingBefore = 50f;
        pngcancel.SpacingBefore = 50f;
        //Give some space after the image
        png.SpacingAfter = 1f;
        png1.SpacingAfter = 1f;
        pngcancel.SpacingAfter = 1f;
        png.Alignment = Element.ALIGN_CENTER;
        png1.Alignment = Element.ALIGN_CENTER;
        pngcancel.Alignment = Element.ALIGN_LEFT;
        doc.Add(png);
        doc.Add(png1);

        string cancelstatus = Dt.Rows[0]["e_way_cancel_status"].ToString();
        if (cancelstatus == true.ToString())
        {
            doc.Add(pngcancel);
        }
        else
        {

        }

        if (Dt.Rows.Count > 0)
        {
            con.Open();
            string EwbNo = Dt.Rows[0]["EwbNo"].ToString();
            //string EwbDt = Dt.Rows[0]["EwbDt"].ToString();            
            DateTime EwbDt = Convert.ToDateTime(Dt.Rows[0]["EwbDt"].ToString());
            string F_EwbDt = EwbDt.ToString("dd-MM-yyyy hh:mm tt");
            string F_EwbDtt = EwbDt.ToString("dd-MM-yyyy");
            DateTime EwbValidTill = Convert.ToDateTime(Dt.Rows[0]["EwbValidTill"].ToString());
            string F_EwbValidTill = EwbValidTill.ToString("dd-MM-yyyy");
            string Irn = Dt.Rows[0]["Irn"].ToString();
            string AckNo = Dt.Rows[0]["AckNo"].ToString();
            DateTime AckDt = Convert.ToDateTime(Dt.Rows[0]["AckDt"].ToString());
            string F_AckDt = EwbDt.ToString("dd-MM-yyyy hh:mm tt");

            SqlCommand cmdGethsn = new SqlCommand("select TOP 1 HSN from tblCreditDebitNoteDtls where HeaderID='" + id + "'", con);
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
                    JToken vehiclListDetails = jsonObject["data"]["VehiclListDetails"];
                    if (vehiclListDetails == null || !vehiclListDetails.HasValues)
                    {
                        vehicleNo = "";
                        transDocNo = "";
                        //string transDocDate1 = vehicleDetails.transDocDate;
                        transDocDate = F_EwbDtt;
                        transMode = "1";
                    }
                    else
                    {
                        dynamic vehicleDetails = jsonObject.data.VehiclListDetails[0];
                        // Access the "vehicleNo" property
                        vehicleNo = vehicleDetails.vehicleNo;
                        transDocNo = vehicleDetails.transDocNo;
                        string transDocDate1 = vehicleDetails.transDocDate;
                        transDocDate = transDocDate1.Replace("/", "-");
                        transMode = vehicleDetails.transMode;
                    }
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
            table.AddCell(new Phrase("27ABCCS7002A1ZW  Pune Abrasives Pvt. Ltd.", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

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
                table.AddCell(new Phrase(GST, FontFactory.GetFont("Arial", 9)));
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
        ifrRight6.Attributes["src"] = @"../PDF_Files/" + Docname;
    }
    #endregion

}