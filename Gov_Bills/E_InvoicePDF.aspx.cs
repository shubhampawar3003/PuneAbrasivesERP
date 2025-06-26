using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using ZXing;
using ZXing.Common;

public partial class Gov_Bills_E_InvoicePDF : System.Web.UI.Page
{
    //string id;
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["Idd"] != null)
            {
                con.Open();
                string id = Decrypt(Request.QueryString["Idd"].ToString()); //Session["PDFID"].ToString();// 
                SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tblCreditDebitNoteHdr where Id='" + id + "'", con);
                Object Qrdtl = cmdQrdtl.ExecuteScalar();
                string F_Qrdtl = Convert.ToString(Qrdtl);
                con.Close();
                if (F_Qrdtl != "")
                {
                    con.Open();
                    SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tblCreditDebitNoteHdr where Id='" + id + "'", con);
                    Object ACkdtl = cmdAckdtl.ExecuteScalar();
                    string F_Ackdtl = Convert.ToString(ACkdtl);
                    con.Close();

                    GenerateQR_CRDBNote(F_Qrdtl); GenerateBarcode_CRDBNote(F_Ackdtl);
                    Pdf_CRDBNote();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
                }
                //Pdf();
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

    #region E-Invoice From Credit Debit Note Section
    private void GenerateQR_CRDBNote(string QR_String)
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

    private void GenerateBarcode_CRDBNote(string AckNo)
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

    protected void Pdf_CRDBNote()
    {
        string id = Decrypt(Request.QueryString["Idd"].ToString());

        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_CreditDebitNote where Id = '" + id + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());
        string billingCustomer = Dt.Rows[0]["SupplierName"].ToString();
        //05-06-2022
        string InvoiceNo = "";

        InvoiceNo = Dt.Rows[0]["DocNo"].ToString();

        string stringInvoiceNo = InvoiceNo.Replace("/", "-");

        Document doc = new Document(PageSize.A4, 30f, 10f, 30f, 0f);


        //Document doc = new Document(PageSize.A4, 10f,);
        //string Path = ;
        string Docname = stringInvoiceNo + "_EInvoice.pdf";
        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + Docname, FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        #region Pdf For Oroginal 
        doc.Open();
        //string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";
        string imageURL = Server.MapPath("~/E_Inv_QrCOde/QR_Img_CRDBNote.jpg");
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

            string invoicedate = Dt.Rows[0]["DocDate"].ToString().TrimEnd("0:0".ToCharArray());
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
            string BillNumber = Dt.Rows[0]["BillNumber"].ToString();
            //string ChallanNo = Dt.Rows[0]["ChallanNo"].ToString();
            string BillDate = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
            string PaymentDueDate = Dt.Rows[0]["PaymentDueDate"].ToString().TrimEnd("0:0".ToCharArray());
            //string EBillNo = Dt.Rows[0]["E_BillNo"].ToString();
            string transactionmode = Dt.Rows[0]["TransportMode"].ToString();
            string vehicalNo = Dt.Rows[0]["VehicalNo"].ToString();
            string placeOfSupply = Dt.Rows[0]["ShippingAddress"].ToString();
            string dateOfSupply = Dt.Rows[0]["DocDate"].ToString();

            string ShippingCustomer = Dt.Rows[0]["ShippingCustomer"].ToString();
            string ShippingAddress = Dt.Rows[0]["ShippingAddress"].ToString();
            string BillingAddress = Dt.Rows[0]["BillingAddress"].ToString();
            string BillingGST = Dt.Rows[0]["BillingGST"].ToString();
            string ShippingGST = Dt.Rows[0]["ShippingGST"].ToString();
            string BillingPincode = Dt.Rows[0]["BillingPincode"].ToString();
            string ShippingPincode = Dt.Rows[0]["ShippingPincode"].ToString();
            string BillingStatecode = Dt.Rows[0]["BillingStatecode"].ToString();
            string ShippingStatecode = Dt.Rows[0]["ShippingStatecode"].ToString();
            string grandtotal = Dt.Rows[0]["Total"].ToString();
            string NoteType = Dt.Rows[0]["NoteType"].ToString();

            con.Open();
            SqlCommand cmdE_inv_Typeof_supply = new SqlCommand("select E_inv_Typeof_supply from tbl_CompanyMaster where Companyname='" + billingCustomer + "'", con);
            Object E_inv_Typeof_supply = cmdE_inv_Typeof_supply.ExecuteScalar();
            string F_E_inv_Typeof_supply = Convert.ToString(E_inv_Typeof_supply);
            con.Close();

            //string ContactNo = Dt.Rows[0]["ContactNo"].ToString();
            //string EmailID = Dt.Rows[0]["Email"].ToString();
            //string TCSAmt = Dt.Rows[0]["TCSAmt"].ToString();
            //string TCSPercent = Dt.Rows[0]["TCSPercent"].ToString();

            //string GSTNo = "";
            //string PanNo = "";
            //string SGSTNo = "";
            //string SPanNo = "";
            //string SBillingAddress = "";
            //string StateName = "";
            //string Shipcontact = "";
            //string ShipEmail = "";
            //string billingCustomerStateName = "";
            //string shippingCustomerStateName = "";
            //string billaddress = "";
            //string shipaddress = "";

            //if (billingCustomer == ShippingCustomer)
            //{
            //    DataTable dtgstno = new DataTable();
            //    SqlDataAdapter sadgst = new SqlDataAdapter("select * from Company where status=0 and cname='" + billingCustomer + "'", con);
            //    sadgst.Fill(dtgstno);
            //    if (dtgstno.Rows.Count > 0)
            //    {
            //        GSTNo = dtgstno.Rows[0]["gstno"].ToString();
            //        string billingaddress = dtgstno.Rows[0]["billingaddress"].ToString();
            //        billaddress = Regex.Replace(billingaddress, @"\s+", " ");
            //        string shippingaddress = dtgstno.Rows[0]["shippingaddress"].ToString();
            //        shipaddress = Regex.Replace(shippingaddress, @"\s+", " ");

            //        if (GSTNo == "")
            //        {
            //            PanNo = "";
            //            GSTNo = "";
            //        }
            //        else
            //        {
            //            string result = GSTNo.Substring(0, 2);
            //            SqlCommand cmdcheck = new SqlCommand("select StateName from tblStateNameCode where Code='" + result + "'", con);
            //            con.Open();
            //            string stateName = cmdcheck.ExecuteScalar().ToString();
            //            StateName = stateName + "(" + result + ")";
            //            con.Close();
            //        }
            //    }

            //    if (GSTNo == "")
            //    {
            //        PanNo = "NA";
            //        GSTNo = "NA";
            //    }
            //    else
            //    {
            //        string MyString = GSTNo;
            //        string res = MyString.Substring(0, 2);
            //        string word1 = res;
            //        string word2 = "1Z";
            //        PanNo = stringBetween(MyString, word1, word2);
            //    }
            //}
            //else
            //{
            //    DataTable dtgstno = new DataTable();
            //    SqlDataAdapter sadgst = new SqlDataAdapter("select * from Company where status=0 and cname='" + billingCustomer + "'", con);
            //    sadgst.Fill(dtgstno);
            //    if (dtgstno.Rows.Count > 0)
            //    {
            //        /////new addition changes 18/01/2023
            //        GSTNo = dtgstno.Rows[0]["gstno"].ToString();
            //        billaddress = dtgstno.Rows[0]["billingaddress"].ToString();
            //        //////

            //        SGSTNo = dtgstno.Rows[0]["gstno"].ToString();
            //        if (GSTNo == "")
            //        {
            //            SPanNo = "";
            //            SGSTNo = "";
            //        }
            //        else
            //        {
            //            SBillingAddress = dtgstno.Rows[0]["billingaddress"].ToString();

            //            string result = SGSTNo.Substring(0, 2);

            //            SqlCommand cmdcheck = new SqlCommand("select StateName from tblStateNameCode where Code='" + result + "'", con);
            //            con.Open();
            //            string stateName = cmdcheck.ExecuteScalar().ToString();
            //            billingCustomerStateName = stateName + "(" + result + ")";
            //            con.Close();
            //        }
            //    }

            //    if (SGSTNo == "")
            //    {
            //        SPanNo = "NA";
            //        SGSTNo = "NA";
            //    }
            //    else
            //    {
            //        string MyString = SGSTNo;
            //        string res = MyString.Substring(0, 2);
            //        string word1 = res;
            //        string word2 = "1Z";
            //        SPanNo = stringBetween(MyString, word1, word2);
            //    }

            //    DataTable dtgstno1 = new DataTable();
            //    SqlDataAdapter sadgst1 = new SqlDataAdapter("select * from Company where status=0 and cname='" + ShippingCustomer + "'", con);
            //    sadgst1.Fill(dtgstno1);
            //    if (dtgstno1.Rows.Count > 0)
            //    {
            //        GSTNo = dtgstno1.Rows[0]["gstno"].ToString();

            //        if (GSTNo == "")
            //        {
            //            PanNo = "";
            //            GSTNo = "";
            //        }
            //        else
            //        {
            //            BillingAddress = dtgstno1.Rows[0]["BillingAddress"].ToString();
            //            Shipcontact = dtgstno1.Rows[0]["mobile1"].ToString();
            //            ShipEmail = dtgstno1.Rows[0]["email1"].ToString();

            //            string result = GSTNo.Substring(0, 2);

            //            SqlCommand cmdcheck = new SqlCommand("select StateName from tblStateNameCode where Code='" + result + "'", con);
            //            con.Open();
            //            string stateName = cmdcheck.ExecuteScalar().ToString();
            //            shippingCustomerStateName = stateName + "(" + result + ")";
            //            con.Close();
            //        }
            //    }

            //    if (GSTNo == "")
            //    {
            //        PanNo = "NA";
            //        GSTNo = "NA";
            //    }
            //    else
            //    {
            //        string MyString = GSTNo;
            //        string res = MyString.Substring(0, 2);
            //        string word1 = res;
            //        string word2 = "1Z";
            //        PanNo = stringBetween(MyString, word1, word2);
            //    }
            //}



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
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " CHINCHWAD PUNE- 411019.", 50, 715, 0);
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

            if (NoteType == "Credit_Sale")
            {
                table.AddCell(new Phrase(" Document Type : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Credit Note", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }
            else if (NoteType == "Debit_Sale")
            {
                table.AddCell(new Phrase(" Document Type : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Debit Note", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }

            table.AddCell(new Phrase(" Document No. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(InvoiceNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(" Document Date :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(invoicedate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            if (F_E_inv_Typeof_supply == "EXPWOP")
            {
                table.AddCell(new Phrase(" Supply Type Code : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("Export, Without Payment", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

                table.AddCell(new Phrase(" Place of Supply :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("OTHER COUNTRIES", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }
            else if (F_E_inv_Typeof_supply == "B2B")
            {
                table.AddCell(new Phrase(" Supply Type Code : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("B2B", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

                table.AddCell(new Phrase(" Place of Supply :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(ShippingAddress, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }
            else if (F_E_inv_Typeof_supply == "SEZWOP")
            {
                table.AddCell(new Phrase(" Supply Type Code : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("SEZ, Without Payment", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

                table.AddCell(new Phrase(" Place of Supply :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase(ShippingAddress, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            }

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
            //if (billingCustomer == ShippingCustomer)
            //{
            //    //table.AddCell(new Phrase(" Supplier : \n\n " + "GSTIN : 27ATFPS1959J1Z4" + "\n " + "Excel Enclosures" + " \n " + "Gat No 1567 Shelar Wasti" + "\n" + " Dehu Alandi Road, Chikhali 411062" + "  \n " + "Maharashtra", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            //    table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n GSTIN :  " + GSTNo + " \n " + billingCustomer + "\n " + billaddress + "  \n " + StateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //    table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n GSTIN :  " + GSTNo + " \n " + ShippingCustomer + "\n " + shipaddress + "  \n " + StateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //    //table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n " + ShippingCustomer + ", \n Address: " + shipaddress + "\n GSTIN: " + GSTNo + "      Pan No.: " + PanNo + " \n State Name:" + StateName + "      Contact No.: " + ContactNo + "\n Email ID.: " + EmailID + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //}
            //else
            //{
            //    table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n GSTIN :  " + SGSTNo + " \n " + billingCustomer + "\n " + SBillingAddress + "  \n " + billingCustomerStateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //    table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n GSTIN :  " + GSTNo + " \n " + ShippingCustomer + "\n " + ShippingAddress + "  \n " + shippingCustomerStateName + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            //    //table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n " + billingCustomer + "\n Address: " + SBillingAddress + " \n GSTIN: " + SGSTNo + "      Pan No.: " + SPanNo + " \n State Name:" + billingCustomerStateName + "      Contact No.: " + ContactNo + "\n Email ID.: " + EmailID + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //    //table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n " + ShippingCustomer + ", \n Address: " + ShippingAddress + "\n GSTIN: " + GSTNo + "      Pan No.: " + PanNo + " \n State Name:" + shippingCustomerStateName + "      Contact No.: " + Shipcontact + "\n Email ID.: " + ShipEmail + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            //}

            string F_BillingStatecode = "", F_ShippingStatecode = "";
            if (F_E_inv_Typeof_supply == "EXPWOP")
            {
                //SqlCommand cmdcheck = new SqlCommand("select StateName from tblStateNameCode where Code='" + BillingStatecode + "'", con);
                //con.Open();
                //string stateName = cmdcheck.ExecuteScalar().ToString();
                //F_BillingStatecode = stateName + "(" + BillingStatecode + ")";
                //con.Close();

                //SqlCommand cmdcheckS = new SqlCommand("select StateName from tblStateNameCode where Code='" + ShippingStatecode + "'", con);
                //con.Open();
                //string stateNameS = cmdcheck.ExecuteScalar().ToString();
                //F_ShippingStatecode = stateNameS + "(" + ShippingStatecode + ")";
                //con.Close();

                table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n GSTIN :  " + BillingGST + " \n " + billingCustomer + "\n " + BillingAddress + "  \n 999999 OTHER COUNTRIES \n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n GSTIN :  " + ShippingGST + " \n " + ShippingCustomer + "\n " + ShippingAddress + "  \n 999999 OTHER COUNTRIES \n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            }
            else
            {
                SqlCommand cmdcheck = new SqlCommand("select StateName from tbl_States where StateCode='" + BillingStatecode + "'", con);
                con.Open();
                string stateName = cmdcheck.ExecuteScalar().ToString();
                F_BillingStatecode = stateName + "(" + BillingStatecode + ")";
                con.Close();

                SqlCommand cmdcheckS = new SqlCommand("select StateName from tbl_States where StateCode='" + ShippingStatecode + "'", con);
                con.Open();
                string stateNameS = cmdcheck.ExecuteScalar().ToString();
                F_ShippingStatecode = stateNameS + "(" + ShippingStatecode + ")";
                con.Close();

                table.AddCell(new Phrase(" Recipient (Billing Address) : \n\n GSTIN :  " + BillingGST + " \n " + billingCustomer + "\n " + BillingAddress + "  \n " + F_BillingStatecode + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                table.AddCell(new Phrase(" Recipient (Shipping Address) : \n\n GSTIN :  " + ShippingGST + " \n " + ShippingCustomer + "\n " + ShippingAddress + "  \n " + F_ShippingStatecode + "\n\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

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

                    string TaxableAmt = dr["TaxableAmt"].ToString();
                    var ConvTaxableAmt = Convert.ToDouble(TaxableAmt);
                    string FinaleTaxableAmt = ConvTaxableAmt.ToString("N2", info);

                    double Ftotal = Convert.ToDouble(dr["Total"].ToString());
                    string _ftotal = Ftotal.ToString("##.00");

                    string partic = dr["Particulars"].ToString().Replace("Enclosure For Control Panel.", "");

                    string Description = partic + "\n" + dr["Description"].ToString();

                    var amt = dr["TaxableAmt"].ToString();

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
                    Ttotal_price += Convert.ToDouble(dr["TaxableAmt"].ToString());
                    GrandTotal1 += Convert.ToDouble(dr["Total"].ToString());
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
            SqlCommand cmd = new SqlCommand("select * from vw_CreditDebitNote where Id='" + id + "'", con);
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
            string imageURLL = Server.MapPath("~/E_Inv_QrCOde/Barcode_Img_CRDBNote.png");
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
    #endregion

}