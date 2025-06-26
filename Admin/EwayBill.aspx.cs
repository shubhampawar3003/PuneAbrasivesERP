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
using Microsoft.Reporting.WebForms;

public partial class Admin_AddCompany : System.Web.UI.Page
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
                //con.Open();
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
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_TaxInvoicePDF where Id = '" + id + "'", con);
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            lblIrn.Text = Dt.Rows[0]["IRN"].ToString();
            lblHiddenid.Text = Dt.Rows[0]["Id"].ToString();
            lblAckNo.Text = Dt.Rows[0]["AckNo"].ToString();
            lblAckDate.Text = Dt.Rows[0]["AckDt"].ToString();
            string invdate = Dt.Rows[0]["InvoiceDate"].ToString();
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

            if (Dt.Rows[0]["InvoiceNo"].ToString() == "")
            {
                lblInvoiceNo.Text = Dt.Rows[0]["FinalBasic"].ToString();
                GetPDFDetailsExport();
            }
            else
            {
                lblInvoiceNo.Text = Dt.Rows[0]["InvoiceNo"].ToString();
                GetPDFDetails();
            }

            string BillingPincode = Dt.Rows[0]["BillingPincode"].ToString();
            if (BillingPincode == "411019")
            {
                lblDistance.Visible = true;
                txtDistance.Enabled = true;
            }
            else
            {
                lblDistance.Visible = false;
                //txtDistance.Text = "NA";
                //txtDistance.Enabled = false;
            }

        }
    }

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("EWayBillList.aspx");
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



    private void GenerateQR(string QR_String)
    {
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img.jpg");
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
        img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img.jpg";
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
            string fileName = "Barcode_Img.png"; // Generate a unique filename
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
        CreateEWayBillby_EInvoiceAPI(); // for Creating EInvoice API       
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

    private string CheckAuthToken()
    {
        string uri = "https://api.mastergst.com/ewaybillapi/v1.03/authenticate?email=erp%40weblinkservices.net&username=API_PuneAbr_WLSPL&password=Pune%40004%23";
        //in url no use of symboles use symbol numbers for api. like(@ = %40, # = %23)
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

    private void Load_E_WayBillData(string id)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbltaxinvoicehdr where Id = '" + id + "'", con);
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
        SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tbltaxinvoicehdr where InvoiceNo='" + lblInvoiceNo.Text + "'", con);
        Object Qrdtl = cmdQrdtl.ExecuteScalar();
        string F_Qrdtl = Convert.ToString(Qrdtl);
        con.Close();
        if (F_Qrdtl != "")
        {
            con.Open();
            SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tbltaxinvoicehdr where InvoiceNo='" + lblInvoiceNo.Text + "'", con);
            Object ACkdtl = cmdAckdtl.ExecuteScalar();
            string F_Ackdtl = Convert.ToString(ACkdtl);
            con.Close();

            GenerateQR(F_Qrdtl); GenerateBarcode(F_Ackdtl);

        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
        }
    }

    protected void GetPDFDetailsExport()
    {
        //con.Open();
        SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tbltaxinvoicehdr where FinalBasic='" + lblInvoiceNo.Text + "'", con);
        Object Qrdtl = cmdQrdtl.ExecuteScalar();
        string F_Qrdtl = Convert.ToString(Qrdtl);
        con.Close();
        if (F_Qrdtl != "")
        {
            con.Open();
            SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tbltaxinvoicehdr where FinalBasic='" + lblInvoiceNo.Text + "'", con);
            Object ACkdtl = cmdAckdtl.ExecuteScalar();
            string F_Ackdtl = Convert.ToString(ACkdtl);
            con.Close();

            GenerateQR(F_Qrdtl); GenerateBarcode(F_Ackdtl);


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
            SqlDataAdapter sadCompany = new SqlDataAdapter("select *,convert(varchar(10), cast(Invoicedate as date), 103) AS DATEI from [tblTaxInvoiceHdr] where Id='" + lblHiddenid.Text + "'", con);
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
                requestObjPost.Headers.Add("ip_address", IPAddress);
                requestObjPost.Headers.Add("client_id", E_Invoice_API_Client_ID);
                requestObjPost.Headers.Add("client_secret", E_Invoice_API_Secret);
                requestObjPost.Headers.Add("username", UserName);
                requestObjPost.Headers.Add("auth-token", AuthToken);
                requestObjPost.Headers.Add("gstin", GST);

                ////check Values
                con.Open();
                //string ffff1 = dtCompany.Rows[0]["DATEI"].ToString();
                DateTime ffff1 = Convert.ToDateTime(txtdate.Text);
                string ffff2 = ffff1.ToString("dd-MM-yyyy");
                // string Transaction_Date = ffff1.Replace("-", "/");
                string Transaction_Date = dtCompany.Rows[0]["DATEI"].ToString();
                string TransactionDoc_Date = ffff2.Replace("-", "/");
                string ShippingGST = dtCompany.Rows[0]["ShippingGST"].ToString();
                string BillingPincode = dtCompany.Rows[0]["BillingPincode"].ToString();
                string ShippingPincode = dtCompany.Rows[0]["ShippingPincode"].ToString();
                string ShippingStatecode = dtCompany.Rows[0]["ShippingStatecode"].ToString();

                //Total Invoice Value
                SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(GrandTotal as float)) FROM [VW_EINVTransport] where headerId='" + lblHiddenid.Text + "'", con);
                Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                double TotInvValuee = Convert.ToDouble(F_TotInvVal);
                string TotInvVal = TotInvValuee.ToString("0.00");

                //Total mainHsnCode Value
                SqlCommand cmdmainHsnCode = new SqlCommand("select top 1 HSN from [tblTaxInvoiceDtls] where HeaderID='" + lblHiddenid.Text + "'", con);
                Object F_mainHsnCode = cmdmainHsnCode.ExecuteScalar();
                string mainHsnCode = F_mainHsnCode.ToString();
                string postData = "";
                con.Close();

                if (txtTransporterID.Text == "" && txtTransporterName.Text == "")
                {
                    if (txtVehicleNumber.Text == "")
                    {
                        if (BillingPincode == "411019")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    else
                    {
                        if (BillingPincode == "411019")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            if (txtDistance.Text == "")
                            {
                                postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                            }
                            else
                            {
                                postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                            }
                        }
                    }
                }
                else
                {
                    if (txtVehicleNumber.Text == "")
                    {
                        //when Distance Same
                        if (BillingPincode == "411019")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"null\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"null\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"null\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"null\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }


                    }
                    else if (txtVehicleNumber.Text != "" && txtTransporterDocNo.Text != "")
                    {
                        if (BillingPincode == "411019")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            if (txtDistance.Text == "")
                            {
                                postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                            }
                            else
                            {
                                postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                            }
                        }
                    }
                    else if (ddlTransportationMode.SelectedValue == "0")
                    {
                        if (BillingPincode == "411019")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransId\":\"" + txtTransporterID.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                    }
                    else if (txtTransporterID.Text != "" && txtVehicleNumber.Text != "" && txtTransporterDocNo.Text == "")
                    {
                        if (BillingPincode == "411019")
                        {
                            postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                        }
                        else
                        {
                            if (txtDistance.Text == "")
                            {
                                postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":0,\"TransId\":\"" + txtTransporterID.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                            }
                            else
                            {
                                postData = "{\"Irn\":\"" + lblIrn.Text + "\",\"Distance\":" + txtDistance.Text + ",\"TransId\":\"" + txtTransporterID.Text + "\",\"VehType\":\"" + ddlVehicleType.SelectedValue + "\",\"TransMode\":\"" + ddlTransportationMode.SelectedValue + "\",\"VehNo\":\"" + txtVehicleNumber.Text + "\",\"TransName\":\"" + txtTransporterName.Text + "\",\"supplyType\":\"O\",\"subSupplyType\":" + ddlSubType.SelectedValue + ",\"TransDocDt\":\"" + TransactionDoc_Date + "\",\"fromGstin\":\"" + GST + "\",\"fromPincode\":411019,\"fromStateCode\":27,\"actualFromStateCode\":27,\"toGstin\":\"" + ShippingGST + "\",\"toPincode\":" + ShippingPincode + ",\"toStateCode\":" + ShippingStatecode + ",\"actualToStateCode\":" + ShippingStatecode + ",\"totInvValue\":" + TotInvVal + "}";
                            }
                        }
                    }
                }
                //Save JSON IN DATABASE update by pawar 28/09/2024
                con.Open();
                Cmd = new SqlCommand("UPDATE tblTaxInvoiceHdr SET E_WAY_Bill_JSON=@E_WAY_Bill_JSON WHERE Id=" + lblHiddenid.Text + "", con);
                Cmd.Parameters.AddWithValue("@E_WAY_Bill_JSON", postData);
                Cmd.ExecuteNonQuery();
                con.Close();

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
                Cmd = new SqlCommand("UPDATE tblTaxInvoiceHdr SET EwbNo=@EwbNo,EwbDt=@EwbDt,EwbValidTill=@EwbValidTill,e_way_status=@e_way_status,e_way_created_by=@e_way_created_by WHERE Id=" + lblHiddenid.Text + "", con);
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
            //string uri = "https://api.mastergst.com/einvoice/type/SYNC_GSTIN_FROMCP/version/V1_03?param1=27AUXPS9994G1Z8&email=";
            string uri1 = "https://api.mastergst.com/einvoice/type/SYNC_GSTIN_FROMCP/version/V1_03?param1=";
            string uri3 = uri1 + GST_Par + "&email=erp%40weblinkservices.net";
            WebResponse response;
            WebRequest request = WebRequest.Create(uri3);

            request.Method = "GET";
            request.Headers.Add("param1", GST_Par);
            request.Headers.Add("email", MailID);
            request.Headers.Add("ip_address", IPAddress);
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

        con.Open();
        SqlCommand cmdQrdtl = new SqlCommand("select EwbNo from tbltaxinvoicehdr where Id='" + lblHiddenid.Text + "'", con);
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
            SqlDataAdapter Da = new SqlDataAdapter("select * from tbltaxinvoicehdr where Id = '" + lblHiddenid.Text + "'", con);
            Da.Fill(Dt);
            string EwbNo = Dt.Rows[0]["EwbNo"].ToString();
            string date = Dt.Rows[0]["EwbDt"].ToString();
            string EwbDt = date.Replace("/", "-");
            con.Close();

            GenerateQR_Eway(EwbNo, GST, EwbDt);
            GenerateBarcode_Eway(EwbNo);
            EwayPdf();
            divCancelEwayBill.Visible = true;
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
            con.Open();
            bool F_e_way_status;
            //Check Already Canceled E Invoice
            SqlCommand cmde_way_status = new SqlCommand("select e_way_cancel_status from tblTaxInvoiceHdr where Id='" + lblHiddenid.Text + "'", con);
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
                SqlCommand cmdGetEwbNo = new SqlCommand("select EwbNo from tblTaxInvoiceHdr where Id='" + lblHiddenid.Text + "'", con);
                Object F_GetEwbNo = cmdGetEwbNo.ExecuteScalar();

                // Cancellation start from Eway API
                string Status_Desc = "Invalid Token";
                string Status_CD = "";
                string CancelDate = "";

                if (!string.IsNullOrWhiteSpace(AuthToken))
                {
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
                }

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
                    Cmd = new SqlCommand("UPDATE tblTaxInvoiceHdr SET e_way_cancel_status=@e_way_cancel_status,e_way_cancel_date=@e_way_cancel_date,e_way_cancel_by=@e_way_cancel_by WHERE Id=" + lblHiddenid.Text + "", con);
                    Cmd.Parameters.AddWithValue("@e_way_cancel_status", 1);
                    Cmd.Parameters.AddWithValue("@e_way_cancel_date", DateTime.Now);
                    Cmd.Parameters.AddWithValue("@e_way_cancel_by", Session["Username"].ToString());
                    Cmd.ExecuteNonQuery();
                    con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Way Bill Cancelled Successfully...!!');window.location.href='../Admin/EWayBillList.aspx';", true);
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
        Response.Redirect("EWayBillList.aspx");
    }

    private void GenerateQR_Eway(string EwbNo, string GST, string EwbDt)
    {
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        string QR_String = EwbNo + "/" + GST + "/" + EwbDt;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img_Eway.jpg");
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
        img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img_Eway.jpg";
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
            string fileName = "Barcode_Img_Eway.png"; // Generate a unique filename
            string filePath = System.IO.Path.Combine(folderPath, fileName);
            barcodeBitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            // Display the barcode image on the web page
            imgBarcode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(barcodeBytes);
            imgBarcode.Visible = false;
        }
    }

    protected void EwayPdf()
    {
        string id = Decrypt(Request.QueryString["CnlId"].ToString());
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbltaxinvoicehdr where Id = '" + id + "'", con);
        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
        //string Path = ;
        string billingCustomer = Dt.Rows[0]["BillingCustomer"].ToString();
        //05-06-2022
        string InvoiceNo = Dt.Rows[0]["InvoiceNo"].ToString();
        string stringInvoiceNo = InvoiceNo.Replace("/", "-");
        string Docname = billingCustomer + "_" + stringInvoiceNo + "_E-WayBill6.pdf";

        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + Docname, FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();
        string imageURL = Server.MapPath("~") + "/E_Inv_QrCOde/QR_Img_Eway.jpg";
        string imageURLbarcode = Server.MapPath("~") + "/E_Inv_QrCOde/Barcode_Img_Eway.png";
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

            SqlCommand cmdGethsn = new SqlCommand("select TOP 1 HSN from tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
            string HSNcode = cmdGethsn.ExecuteScalar().ToString();

            #region get Info from Eway bill API    
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

                }
            }

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
            table2.AddCell(new Phrase("Pune MAHARASHTRA 411019", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

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
        Iframe1.Attributes["src"] = @"../PDF_Files/" + Docname;
    }

    protected void btneinvoice_Click(object sender, EventArgs e)
    {
        con.Open();
        SqlCommand cmdQrdtl = new SqlCommand("select ID from tbltaxinvoicehdr where InvoiceNo='" + lblInvoiceNo.Text + "'", con);
        Object Qrdtl = cmdQrdtl.ExecuteScalar();
        string InvoiceNo = Convert.ToString(Qrdtl);
        con.Close();
        Report(InvoiceNo);
    }
    public void Report(string Invoiceno)
    {
        byte[] imageBytes = null;
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetE_InvoiceDetails");
                    cmd.Parameters.AddWithValue("@Invoiceno", Invoiceno);

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(Dtt);

                        }
                    }
                }
            }

            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    // Genrate Qr Code For E-Invoice RDLC
                    con.Open();
                    string id = Invoiceno;
                    SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tbltaxinvoicehdr where Id='" + id + "'", con);
                    Object Qrdtl = cmdQrdtl.ExecuteScalar();
                    string F_Qrdtl = Convert.ToString(Qrdtl);
                    con.Close();
                    if (F_Qrdtl != "")
                    {
                        con.Open();
                        SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tbltaxinvoicehdr where Id='" + id + "'", con);
                        Object ACkdtl = cmdAckdtl.ExecuteScalar();
                        string F_Ackdtl = Convert.ToString(ACkdtl);
                        con.Close();
                        imageBytes = GenerateQRForReport(F_Qrdtl);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("QRCodeImage", typeof(byte[]));

                    // Add the QR code image to the table
                    DataRow row = dt.NewRow();
                    row["QRCodeImage"] = imageBytes;
                    dt.Rows.Add(row);

                    // DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                    ReportDataSource obj4 = new ReportDataSource("DataSet4", dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.DataSources.Add(obj4);

                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\EInvoice.rdlc";

                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    string filePath = Server.MapPath("~/PDF_Files/") + "EInvoice.pdf";

                    // Save the file to the specified path
                    System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                    Response.Redirect("~/PDF_Files/EInvoice.pdf");


                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }
    }
    private byte[] GenerateQRForReport(string QR_String)
    {
        byte[] QrBytes = null;
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img.jpg");
        var barcodeBitmap = new System.Drawing.Bitmap(result);

        using (MemoryStream memory = new MemoryStream())
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {

                barcodeBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                QrBytes = memory.ToArray();
                fs.Write(QrBytes, 0, QrBytes.Length);
            }
        }
        //img_QrCode.Visible = true;
        //img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img.jpg";
        return QrBytes;
    }
    protected void btnEWaylist_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Admin/EWayBillList.aspx");
    }

    protected void btnewaybill_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Admin/EWayBillList.aspx");
    }
}
