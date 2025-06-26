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
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Microsoft.Reporting.WebForms;
using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;

using Spire.Pdf.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class Admin_TaxInvoiceList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

    private readonly DateTimeFormatInfo UkDtfi = new CultureInfo("en-GB", false).DateTimeFormat;

    private SqlCommand Cmd;
    public string AuthToken = "";

    // Credentials
    public string E_Invoice_Client_ID = ConfigurationManager.AppSettings["EInvoiceClientID"].ToString();
    public string E_Invoice_Secret = ConfigurationManager.AppSettings["EInvoiceSecretCode"].ToString();
    string UserName = ConfigurationManager.AppSettings["EUserName"].ToString();
    string Password = ConfigurationManager.AppSettings["EPassword"].ToString();
    string GST = ConfigurationManager.AppSettings["EGST"].ToString();

    string MailID = ConfigurationManager.AppSettings["EmailID"].ToString();
    string IPAddress = ConfigurationManager.AppSettings["IPAddress"].ToString();


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                GVBinddata();
            }
        }
    }

    protected void GVBinddata()
    {
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceHdr where isdeleted='0'  AND Status>2 order by CreatedOn DESC", con);
            sad.Fill(dt);
            GvInvoiceList.DataSource = dt;
            GvInvoiceList.DataBind();
            GvInvoiceList.EmptyDataText = "Record Not Found";
            //ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvInvoiceList.ClientID + "', 400, 1020 , 40 ,true); </script>", false);
        }
        catch (Exception)
        {

            throw;
        }
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCustomerList(string prefixText, int count)
    {
        return AutoFillCustomerlist(prefixText);
    }

    public static List<string> AutoFillCustomerlist(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "select DISTINCT BillingCustomer from tblTaxInvoiceHdr where " + "BillingCustomer like @Search + '%' AND isdeleted='0'  ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> BillingCustomer = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        BillingCustomer.Add(sdr["BillingCustomer"].ToString());
                    }
                }
                con.Close();
                return BillingCustomer;
            }

        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        try
        {
            DataTable dtt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceHdr where BillingCustomer='" + txtCustomerName.Text + "' AND isdeleted='0' order by CreatedOn DESC", con);
            sad.Fill(dtt);
            GvInvoiceList.DataSource = dtt;
            GvInvoiceList.DataBind();
            GvInvoiceList.EmptyDataText = "Record Not Found";
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("EInvoiceList.aspx");
    }
    byte[] bytePdfRep = null;
    protected void GvInvoiceList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RowCreate")
        {
            string ID = e.CommandArgument.ToString();
            Create_EInv(ID);
        }
        if (e.CommandName == "DownloadPDF")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                con.Open();
                SqlCommand cmdQrdtl = new SqlCommand("select e_invoice_cancel_status from tbltaxinvoicehdr where Id='" + e.CommandArgument.ToString() + "'", con);
                Object Qrdtl = cmdQrdtl.ExecuteScalar();
                string e_invoice_cancel_status = Convert.ToString(Qrdtl);
                con.Close();
                if (e_invoice_cancel_status == "True")
                {
                    CancelReport(e.CommandArgument.ToString(), "");
                }
                else
                {
                    ViewState["PDFID"] = e.CommandArgument.ToString();
                    string[] types = { "ORIGINAL", "DUPLICATE", "TRIPLICATE" };
                    foreach (var type in types)
                    {
                        // Generate the report and get the PDF byte array
                        EInvoiceReports(e.CommandArgument.ToString(), "show", type);

                        using (MemoryStream pdfStream = new MemoryStream(bytePdfRep))
                        {
                            try
                            {
                                using (PdfReader reader = new PdfReader(pdfStream))
                                {
                                    // Define the file path for each type
                                    string filePath = Server.MapPath("~/PDF_Files/" + type + "_EInvoice.pdf");

                                    // Use FileStream to write the PDF to disk
                                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                    {
                                        // Create a document and copy the content
                                        using (Document document = new Document())
                                        {
                                            iTextSharp.text.pdf.PdfCopy copy = new PdfCopy(document, fileStream);
                                            document.Open();
                                            copy.AddDocument(reader);
                                            document.Close();
                                        }
                                    }
                                }
                            }
                            catch (iTextSharp.text.pdf.PdfException pdfEx)
                            {
                                // Handle PDF-specific errors (log or notify)
                                throw new Exception("Error reading PDF for '" + type + "': {pdfEx.Message}");
                            }
                        }
                    }
                    string folder3 = HttpContext.Current.Server.MapPath("~/PDF_Files/Triplicate_EInvoice.pdf");
                    string folder2 = HttpContext.Current.Server.MapPath("~/PDF_Files/Duplicate_EInvoice.pdf");
                    string folder1 = HttpContext.Current.Server.MapPath("~/PDF_Files/Original_EInvoice.pdf");
                    string folder4 = HttpContext.Current.Server.MapPath("~/PDF_Files/TermCondition.pdf");
                    string outputPath = HttpContext.Current.Server.MapPath("~/PDF_Files/_EInvoice.pdf");

                    // Array containing the file paths
                    string[] fileArray = { folder1, folder2, folder3, folder4 };
                    //string[] fileArray = { folder1, folder4, folder2, folder4, folder3, folder4 };
                    // Merge the PDF files
                    MergePDFFiles(fileArray, outputPath);
                    Response.Redirect("~/PDF_Files/_EInvoice.pdf");
                }


            }
        }
        if (e.CommandName == "RowCancel")
        {
            string ID = e.CommandArgument.ToString();
            Cancel_EInv(ID);
            //Response.Redirect("EInvoiceList.aspx");
        }
    }



    public string encrypt(string encryptString)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }

    #region Create E-INVOICE
    private void Create_EInv(string ID)
    {
        try
        {
            string _AckNo = "", _AckDt = "", _Irn = "", _SignedQRCode = "", _Status = "", _Remarks = "", _Messege = "";
            object _SignedInvoice = "";
            con.Open();

            //Check Already genereate E Invoice
            SqlCommand cmde_invoice_status = new SqlCommand("select e_invoice_status from tblTaxInvoiceHdr where Id='" + ID + "'", con);
            Object e_invoice_status = cmde_invoice_status.ExecuteScalar();

            if (e_invoice_status == "1")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Already Genereted Please Select Another..!!');", true);
            }
            else
            {
                //SqlCommand cmdCname = new SqlCommand("select BillingCustomer from tblTaxInvoiceHdr where Id='" + ID + "'", con);
                //Object F_Cname = cmdCname.ExecuteScalar();
                //Get Company Details from invoice Table
                DataTable dtCompany = new DataTable();
                SqlDataAdapter sadCompany = new SqlDataAdapter("select *,convert(varchar(10), cast(Invoicedate as date), 103) AS DATEI from [tblTaxInvoiceHdr] where Id='" + ID + "'", con);
                sadCompany.Fill(dtCompany);

                #region Seller Details

                //Declare Variables
                string Seller_Gst_No = "", Invoice_No = "", Invoice_No_Export = "", CountryCode_Export = "", Transaction_Date = "", Seller_Firm_Name = "", Seller_Firm_Address = "", Seller_Location = "", Seller_Pin_Code = "", Seller_State_Code = "";

                //Set variable
                Seller_Gst_No = GST; // for testing
                                     //Seller_Gst_No = GST;  // for pune Abrasives
                if (string.IsNullOrWhiteSpace(Seller_Gst_No))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller GST No Not Set Please Enter Seller GST No..!');", true);
                }


               
                string InvNoChk = "123456";//dtCompany.Rows[0]["invoiceno"].ToString();
                if (InvNoChk == null || InvNoChk == "")
                {
                    Invoice_No = dtCompany.Rows[0]["FinalBasic"].ToString();
                }
                else
                {
                    Invoice_No ="123456";// dtCompany.Rows[0]["invoiceno"].ToString();
                }
                if (string.IsNullOrWhiteSpace(Invoice_No))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Invoice_No Not Set Please Enter Invoice_NO..!');", true);
                }

                string ffff1 = dtCompany.Rows[0]["DATEI"].ToString();
                Transaction_Date = ffff1;

                //Transaction_Date = dtCompany.Rows[0]["invoicedate"].ToString();
                if (string.IsNullOrWhiteSpace(Transaction_Date))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Transaction_Date Not Set Please Enter Transaction_Date..!');", true);
                }

                Seller_Firm_Name = "Pune Abrasives Pvt. Ltd.";
                if (string.IsNullOrWhiteSpace(Seller_Firm_Name))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller_Firm_Name Not Set Please Enter Seller_Firm_Name..!');", true);
                }

                Seller_Firm_Address = "Plot No. 84, D2 Block, MIDC Chinchwad,Pune 411019, Maharashtra,India.";
                if (string.IsNullOrWhiteSpace(Seller_Firm_Address))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller_Firm_Address Not Set Please Enter Seller_Firm_Address..!');", true);
                }
                Seller_Location = "Pune";
                if (string.IsNullOrWhiteSpace(Seller_Location))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller_Location Not Set Please Enter Seller_Location..!');", true);
                }

               //  Seller_Pin_Code = "411019";  //for pune abrasives
                Seller_Pin_Code = "587315";  // for testing
                if (string.IsNullOrWhiteSpace(Seller_Pin_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller_Pin_Code Not Set Please Enter Seller_Pin_Code..!');", true);
                }
                Seller_State_Code = "29"; // for testing
                        //                   Seller_State_Code = "27"; // for pune abrasives
                if (string.IsNullOrWhiteSpace(Seller_State_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller_State_Code Not Set Please Enter Seller_State_Code..!');", true);
                }

                #endregion

                #region  Buyer Details
                //Declare Variables
                string Buyer_GST_No = "", Buyer_Firm_Name = "", Buyer_Address = "", Buyer_Location = "", Buyer_Pin_Code = "", Buyer_State_Code = "";
                //set variables
                Buyer_GST_No = dtCompany.Rows[0]["BillingGST"].ToString();
                //Buyer_GST_No = "27AABCW8929J2ZP";
                if (string.IsNullOrWhiteSpace(Buyer_GST_No))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Buyer_GST_No Not Set Please Enter Buyer_GST_No...!!');", true);
                }
                Buyer_Firm_Name = dtCompany.Rows[0]["BillingCustomer"].ToString();
                //Buyer_Firm_Name = "Web Link Services Pvt. Ltd.";
                if (string.IsNullOrWhiteSpace(Buyer_Firm_Name))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Buyer_Firm_Name Not Set Please Enter Buyer_Firm_Name....!!');", true);
                }
                string Buyer_Address1 = dtCompany.Rows[0]["ShortBAddress"].ToString();
                //Buyer_Address = Buyer_Address1.Replace(" ", "");
                Buyer_Address = Regex.Replace(Buyer_Address1, @"\s+", " ");
                //Buyer_Address = "Pimpale Saudagar";
                if (string.IsNullOrWhiteSpace(Buyer_Address))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Buyer_Address Not Set Please Enter Buyer_Address...!!');", true);
                }
                Buyer_Location = dtCompany.Rows[0]["BillingLocation"].ToString();
                //Buyer_Location = "Pune";
                if (string.IsNullOrWhiteSpace(Buyer_Location))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Buyer_Location Not Set Please Enter Buyer_Location...!!');", true);
                }
                Buyer_Pin_Code = dtCompany.Rows[0]["BillingPincode"].ToString();
                //Buyer_Pin_Code = "411062";
                if (string.IsNullOrWhiteSpace(Buyer_Pin_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Buyer_Pin_Code Not Set Please Enter Buyer_Pin_Code...!!');", true);
                }
                Buyer_State_Code = dtCompany.Rows[0]["BillingStatecode"].ToString();
                //Buyer_State_Code = "27";
                if (string.IsNullOrWhiteSpace(Buyer_State_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Buyer_State_Code Not Set Please Enter Buyer_State_Code...!!');", true);
                }
                #endregion

                // For Export Invoice Start
                #region Export Details
                string InvNoChkExp = dtCompany.Rows[0]["InvoiceType"].ToString();
                if (InvNoChkExp == "Export")
                {
                    Invoice_No_Export = dtCompany.Rows[0]["InvoiceNo"].ToString();

                    SqlCommand cmd_CountryCode_status = new SqlCommand("select CountryCode from tbl_CompanyMaster where Companyname='" + Buyer_Firm_Name + "'", con);
                    Object CountryCode = cmd_CountryCode_status.ExecuteScalar();
                    CountryCode_Export = CountryCode.ToString();
                    if (string.IsNullOrWhiteSpace(CountryCode_Export))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('CountryCode_Export Not Set Please Enter CountryCode_Export..!!');", true);
                    }
                }
                #endregion
                // For Export Invoice End

                #region Ship Party Details
                //Declare Variables
                string Ship_Firm_Name = "", Ship_Firm_Address = "", Ship_Location = "", Ship_Pin_Code = "", Ship_Firm_State_Code = "";
                //Set variables

                Ship_Firm_Name = dtCompany.Rows[0]["ShippingCustomer"].ToString();
                //Ship_Firm_Name = "Web Link Services Pvt. Ltd.";
                if (string.IsNullOrWhiteSpace(Ship_Firm_Name))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Firm_Name Not Set Please Enter Ship_Firm_Name..!!');", true);
                }
                string Ship_Firm_Address1 = dtCompany.Rows[0]["ShortSAddress"].ToString();
                // Ship_Firm_Address = dtCompany.Rows[0]["ShippingAddress"].ToString();
                Ship_Firm_Address = Regex.Replace(Ship_Firm_Address1, @"\s+", " ");
                //Ship_Firm_Address = "Pimpale Saudagar";
                if (string.IsNullOrWhiteSpace(Ship_Firm_Address))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Firm_Address Not Set Please Enter Ship_Firm_Address..!!');", true);
                }
                Ship_Location = dtCompany.Rows[0]["ShippingLocation"].ToString();
                //Ship_Location = "Pune";
                if (string.IsNullOrWhiteSpace(Ship_Location))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Location Not Set Please Enter Ship_Location..!!');", true);
                }
                Ship_Pin_Code = dtCompany.Rows[0]["ShippingPincode"].ToString();
                //Ship_Pin_Code = "411062";
                if (string.IsNullOrWhiteSpace(Ship_Pin_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Pin_Code Not Set Please Enter Ship_Pin_Code..!!');", true);
                }
                Ship_Firm_State_Code = dtCompany.Rows[0]["ShippingStatecode"].ToString();
                //Ship_Firm_State_Code = "27";
                if (string.IsNullOrWhiteSpace(Ship_Firm_State_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Firm_State_Code Not Set Please Enter Ship_Firm_State_Code..!!');", true);
                }
                #endregion

                #region ItemList

                //Declare variables
                string HsnCd = "", Qty = "", Unit = "", UnitPrice = "", TotAmt = "", Item_Discount = "", AssAmt = "", GstRt = "",
                    SgstAmt = "null", IgstAmt = "null", CgstAmt = "null", IgstVal = "null", CgstVal = "null", SgstVal = "null", TotInvVal = "", TotInvValFc = "", TotItemVal = "", IsServc = "";

                //Fright Charges Check Start
                DataTable dt = new DataTable();
                SqlCommand cmdFright = new SqlCommand("select Basic from [tblTaxInvoiceHdr] where id='" + ID + "'", con);
                Object F_Fright = cmdFright.ExecuteScalar();
                string FrightValuee = F_Fright.ToString();
                if (FrightValuee == "0")
                {
                    //DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceDtls where HeaderId='" + ID + "'", con);
                    sad.Fill(dt);
                }
                else
                {
                    //DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from VW_EINVTransport where HeaderId='" + ID + "'", con);
                    sad.Fill(dt);
                }
                //Fright Charges Check End

                //DataTable dt = new DataTable();
                //SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceDtls where HeaderId='" + ID + "'", con);
                //sad.Fill(dt);
                //  if (dt.Rows.Count > 0)
                //  {
                StringBuilder itemlist = new StringBuilder();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string Description = dt.Rows[i]["Description"].ToString();
                    //string Particular = dt.Rows[i]["Particular"].ToString();
                    if (Description == "Transport Charges")
                    {
                        IsServc = "Y";
                    }
                    else
                    {
                        IsServc = "N";
                    }

                    //if (Description == "Labour Charges")
                    //{
                    //    IsServc = "Y";
                    //}
                    //else
                    //{
                    //    IsServc = "N";
                    //}

                    HsnCd = dt.Rows[i]["HSN"].ToString();
                    if (string.IsNullOrWhiteSpace(HsnCd))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Hsn Not Set Please Enter Hsn....!!');", true);
                    }
                    else
                    {
                        if (HsnCd.Length == 6)
                        {
                            IsServc = "Y";
                        }
                    }
                    Qty = dt.Rows[i]["Qty"].ToString();
                    if (string.IsNullOrWhiteSpace(Qty))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Qty Not Set Please Enter Qty....!!');", true);
                    }
                    Unit = "KGS";//dt.Rows[i]["UOM"].ToString();
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
                    object Obj_AssAmt = dt.Rows[i]["TaxableAmt"].ToString();
                    double AssAmtValuee = Convert.ToDouble(Obj_AssAmt);
                    AssAmt = AssAmtValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(AssAmt))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Taxable Value Not Set Please Enter Taxable Value....!!');", true);
                    }
                    double Discount_Per = Convert.ToDouble(dt.Rows[i]["Discount"].ToString());
                    double Discount_Amt = (Basic * Discount_Per) / 100;
                    Item_Discount = Discount_Amt.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(Item_Discount))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Discount Not Set Please Enter Discount....!!');", true);
                    }
                    string cgstper = dt.Rows[i]["CGSTPer"].ToString();
                    string sgstper = dt.Rows[i]["SGSTPer"].ToString(); //added by pawar 05102024
                    string igstper = dt.Rows[i]["IGSTPer"].ToString();
                    if (cgstper != "0")
                    {
                        GstRt = Convert.ToString(Convert.ToDouble(cgstper) + Convert.ToDouble(sgstper));
                    }
                    else if (igstper != "0")
                    {
                        GstRt = igstper;
                    }
                    else if (cgstper == "0" && igstper == "0")
                    {
                        GstRt = "0";
                    }
                    if (string.IsNullOrWhiteSpace(GstRt))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('GST Rate Not Set Please Enter GST Rate...!!');", true);
                    }
                    object Obj_SgstAmt = dt.Rows[i]["SGSTAmt"].ToString();
                    double SgstAmtValuee = Convert.ToDouble(Obj_SgstAmt);
                    SgstAmt = SgstAmtValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(SgstAmt))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('SgstAmt Not Set Please Enter SgstAmt...!!');", true);
                    }
                    object Obj_IgstAmt = dt.Rows[i]["IGSTAmt"].ToString();
                    double IgstAmtValuee = Convert.ToDouble(Obj_IgstAmt);
                    IgstAmt = IgstAmtValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(IgstAmt))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('IgstAmt Not Set Please Enter IgstAmt...!!');", true);
                    }
                    object Obj_CgstAmt = dt.Rows[i]["CGSTAmt"].ToString();
                    double CgstAmtValuee = Convert.ToDouble(Obj_CgstAmt);
                    CgstAmt = CgstAmtValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(CgstAmt))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('CgstAmt Not Set Please Enter CgstAmt...!!');", true);
                    }
                    object Obj_TotItemVal = dt.Rows[i]["GrandTotal"].ToString();
                    double TotItemValuee = Convert.ToDouble(Obj_TotItemVal);
                    TotItemVal = TotItemValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(TotItemVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('TotItemVal Not Set Please Enter TotItemVal...!!');", true);
                    }

                    //{\"SlNo\":\"1\",\"IsServc\":\"N\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}
                    int cnt = i + 1;
                    //if (Description== "Labour Charges")
                    //{
                    //    itemlist.Append("{\"SlNo\":\"" + cnt + "\",\"IsServc\":\"" + IsServc + "\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}");
                    //}
                    //else
                    //{
                    //    itemlist.Append("{\"SlNo\":\"" + cnt + "\",\"IsServc\":\"" + IsServc + "\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}");
                    //}
                    itemlist.Append("{\"SlNo\":\"" + cnt + "\",\"IsServc\":\"" + IsServc + "\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}");


                    itemlist.Append(",");
                }
                //}

                char[] charsToTrim = { ',', ' ' };
                string list = itemlist.ToString().Trim().TrimEnd(charsToTrim);
                string AssVal;
                string OthChrg;
                if (FrightValuee == "0")
                {
                    //Total Basic of all products
                    SqlCommand cmdAssVal = new SqlCommand("select SUM(cast(TaxableAmt as float)) FROM [tblTaxInvoiceDtls] where headerId='" + ID + "'", con);
                    Object F_AssVal = cmdAssVal.ExecuteScalar();
                    double AssValValuee = Convert.ToDouble(F_AssVal);
                    AssVal = AssValValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(AssVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final AssVal Not Set Please Enter AssVal....!!');", true);
                    }
                    //Total TCS of all products
                    SqlCommand cmdOthChrg = new SqlCommand("select SUM(cast(TCSAmt as float)) FROM tbltaxinvoicehdr where id='" + ID + "'", con);
                    Object F_OthChrg = cmdOthChrg.ExecuteScalar();
                    double OthChrgValuee = Convert.ToDouble(F_OthChrg);
                    OthChrg = OthChrgValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(OthChrg))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('OthChrg Not Set Please Enter AssVal....!!');", true);
                    }

                    // Final Grandtotal
                    if (OthChrg == "0.00")
                    {
                        SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(GrandTotal as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                        Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                        double TotInvValuee = Convert.ToDouble(F_TotInvVal);
                        TotInvVal = TotInvValuee.ToString("0.00");
                        if (string.IsNullOrWhiteSpace(TotInvVal))
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final TotInvVal Not Set Please Enter AssVal....!!');", true);
                        }
                    }
                    else
                    {
                        SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(GrandTotal as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                        Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                        double TotInvValuee = Convert.ToDouble(F_TotInvVal) + Convert.ToDouble(OthChrg);
                        TotInvVal = TotInvValuee.ToString("0.00");
                        if (string.IsNullOrWhiteSpace(TotInvVal))
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final TotInvVal Not Set Please Enter AssVal....!!');", true);
                        }
                    }

                    // Final SgstVal
                    SqlCommand cmdSgstVal = new SqlCommand("select SUM(cast(SGSTAmt as float)) FROM [tblTaxInvoiceDtls] where headerId='" + ID + "'", con);
                    Object F_SgstVal = cmdSgstVal.ExecuteScalar();
                    double sgstValuee = Convert.ToDouble(F_SgstVal);
                    SgstVal = sgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(SgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final SGSTAmt Not Set Please Enter SGSTAmt....!!');", true);
                    }

                    // Final CgstVal
                    SqlCommand cmdCgstVal = new SqlCommand("select SUM(cast(CGSTAmt as float)) FROM [tblTaxInvoiceDtls] where headerId='" + ID + "'", con);
                    Object F_CgstVal = cmdCgstVal.ExecuteScalar();
                    double cgstValuee = Convert.ToDouble(F_CgstVal);
                    CgstVal = cgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(CgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final CGSTAmt Not Set Please Enter CGSTAmt....!!');", true);
                    }

                    // Final IgstVal
                    SqlCommand cmdIgstVal = new SqlCommand("select SUM(cast(IGSTAmt as float)) FROM [tblTaxInvoiceDtls] where headerId='" + ID + "'", con);
                    Object F_IgstVal = cmdIgstVal.ExecuteScalar();
                    double igstValuee = Convert.ToDouble(F_IgstVal);
                    IgstVal = igstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(IgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final IGSTAmt Not Set Please Enter IGSTAmt....!!');", true);
                    }
                }
                else
                {
                    //Total Basic of all products
                    SqlCommand cmdAssVal = new SqlCommand("select SUM(cast(TaxableAmt as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                    Object F_AssVal = cmdAssVal.ExecuteScalar();
                    double AssValValuee = Convert.ToDouble(F_AssVal);
                    AssVal = AssValValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(AssVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final AssVal Not Set Please Enter AssVal....!!');", true);
                    }

                    //Total TCS of all products
                    SqlCommand cmdOthChrg = new SqlCommand("select SUM(cast(TCSAmt as float)) FROM tbltaxinvoicehdr where id='" + ID + "'", con);
                    Object F_OthChrg = cmdOthChrg.ExecuteScalar();
                    double OthChrgValuee = Convert.ToDouble(F_OthChrg);
                    OthChrg = OthChrgValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(OthChrg))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('OthChrg Not Set Please Enter AssVal....!!');", true);
                    }

                    // Final Grandtotal
                    if (OthChrg == "0")
                    {
                        SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(GrandTotal as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                        Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                        double TotInvValuee = Convert.ToDouble(F_TotInvVal);
                        TotInvVal = TotInvValuee.ToString("0.00");
                        if (string.IsNullOrWhiteSpace(TotInvVal))
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final TotInvVal Not Set Please Enter AssVal....!!');", true);
                        }
                    }
                    else
                    {
                        SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(GrandTotal as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                        Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                        double TotInvValuee = Convert.ToDouble(F_TotInvVal) + Convert.ToDouble(OthChrg);
                        TotInvVal = TotInvValuee.ToString("0.00");
                        if (string.IsNullOrWhiteSpace(TotInvVal))
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final TotInvVal Not Set Please Enter AssVal....!!');", true);
                        }
                    }


                    // Final SgstVal
                    SqlCommand cmdSgstVal = new SqlCommand("select SUM(cast(SGSTAmt as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                    Object F_SgstVal = cmdSgstVal.ExecuteScalar();
                    double sgstValuee = Convert.ToDouble(F_SgstVal);
                    SgstVal = sgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(SgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final SGSTAmt Not Set Please Enter SGSTAmt....!!');", true);
                    }

                    // Final CgstVal
                    SqlCommand cmdCgstVal = new SqlCommand("select SUM(cast(CGSTAmt as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                    Object F_CgstVal = cmdCgstVal.ExecuteScalar();
                    double cgstValuee = Convert.ToDouble(F_CgstVal);
                    CgstVal = cgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(CgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final CGSTAmt Not Set Please Enter CGSTAmt....!!');", true);
                    }

                    // Final IgstVal
                    SqlCommand cmdIgstVal = new SqlCommand("select SUM(cast(IGSTAmt as float)) FROM [VW_EINVTransport] where headerId='" + ID + "'", con);
                    Object F_IgstVal = cmdIgstVal.ExecuteScalar();
                    double igstValuee = Convert.ToDouble(F_IgstVal);
                    IgstVal = igstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(IgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final IGSTAmt Not Set Please Enter IGSTAmt....!!');", true);
                    }
                }

                #endregion

                //Einvoice Method Pass Value
                EInvoice(UserName, Password, Seller_Gst_No, ID, Invoice_No, CountryCode_Export, Transaction_Date, Seller_Firm_Name, Seller_Firm_Address,
                   Seller_Location, Seller_Pin_Code, Seller_State_Code, Buyer_GST_No, Buyer_Firm_Name, Buyer_Address, Buyer_Location, Buyer_Pin_Code, Buyer_State_Code, Seller_Firm_Name, Seller_Firm_Address, Seller_Location, Seller_Pin_Code, Seller_State_Code, Ship_Firm_Name, Ship_Firm_Address, Ship_Location, Ship_Pin_Code, Ship_Firm_State_Code, list, AssVal, OthChrg, TotInvVal, SgstVal, CgstVal, IgstVal, out _AckNo, out _AckDt, out _Irn, out _SignedInvoice, out _SignedQRCode, out _Status, out _Remarks, out _Messege);
                //OthChrg, RndOffAmt removed

                //Check Accknolegement number
                //if (string.IsNullOrWhiteSpace(_AckNo))
                //{
                //    con.Close();
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + _Messege + "');", true);
                //}

                ////Update Data                
                if (_AckNo == "" || _AckDt == "" || _Irn == "" || _SignedInvoice == "" || _SignedQRCode == "" || _Status == "")
                {
                    //con.Close();
                    //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Something Went Wrong. Kindly Create Again..!!');", true);
                    con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + _Messege + "');", true);
                }
                else
                {
                    Cmd = new SqlCommand("UPDATE tblTaxInvoiceHdr SET AckNo=@AckNo,AckDt=@AckDt,Irn=@Irn,SignedInvoice=@SignedInvoice,SignedQRCode=@SignedQRCode,InvoiceStatus=@InvoiceStatus,Remarks=@Remarks,e_invoice_status=@e_invoice_status,e_invoice_created_by=@e_invoice_created_by WHERE Id=" + ID + "", con);
                    Cmd.Parameters.AddWithValue("@AckNo", _AckNo);
                    Cmd.Parameters.AddWithValue("@AckDt", _AckDt);
                    Cmd.Parameters.AddWithValue("@Irn", _Irn);
                    Cmd.Parameters.AddWithValue("@SignedInvoice", _SignedInvoice);
                    Cmd.Parameters.AddWithValue("@SignedQRCode", _SignedQRCode);
                    Cmd.Parameters.AddWithValue("@InvoiceStatus", _Status);
                    Cmd.Parameters.AddWithValue("@Remarks", _Remarks);
                    Cmd.Parameters.AddWithValue("@e_invoice_status", 1);
                    Cmd.Parameters.AddWithValue("@e_invoice_created_by", Session["Username"].ToString());
                    Cmd.ExecuteNonQuery();
                    con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Genereted Successfully...!!');window.location='EInvoiceList.aspx';", true);
                }
                GVBinddata();

            }
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void EInvoice(string User_Name, string Password, string Seller_GST_No, string ID, string Invoice_No, string CountryCode, string Date_Dt, string Seller_Firm_Name, string Seller_Address, string Seller_location, string Seller_Pin_Code, string Seller_State_Code, string Buyer_GST_No, string Buyer_Firm_Name, string Buyer_Address, string Buyer_Location, string Buyer_Pin_Code, string Buyer_State_Code, string Dispatch_Name, string Dispatch_Address, string Dispatch_Location, string Dispatch_Pin_Code, string Dispatch_State_Code, string Ship_Firm_Name, string Ship_Address, string Ship_Location, string Ship_Pin_Code, string Ship_State_Code, string list, string AssVal, string OthChrg, string TotInvVal, string SgstVal, string CgstVal, string IgstVal, out string _AckNo, out string _AckDt, out string _Irn, out object _SignedInvoice, out string _SignedQRCode, out string _Status, out string _Remarks, out string _Messege)
    {
        _AckNo = ""; _AckDt = ""; _Irn = ""; _SignedInvoice = ""; _SignedQRCode = ""; _Status = ""; _Remarks = ""; _Messege = "";
        try
        {
            //AuthToken
            string _TokenExpiry = "", _Sek = "", _AuthKey = "", _Messeg = "";
            DataTable DT_AuthToken = new DataTable();
            SqlDataAdapter sad_AuthToken = new SqlDataAdapter("SELECT * FROM AUTH_TOKEN_MASTER WHERE UserName='" + User_Name + "'", con);
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
                    GenrateAuthKey(User_Name, Password, Seller_GST_No, out _TokenExpiry, out _Sek, out _AuthKey, out _Messeg);

                    AuthToken = _AuthKey;

                    if (string.IsNullOrWhiteSpace(AuthToken))
                    {
                        //XtraMessageBox.Show(_Messeg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // AuthToken = DT_AuthToken.Rows[0]["AuthToken"].ToString();
                }
            }
            else
            {
                GenrateAuthKey(User_Name, Password, Seller_GST_No, out _TokenExpiry, out _Sek, out _AuthKey, out _Messeg);
                AuthToken = _AuthKey;
                if (string.IsNullOrWhiteSpace(AuthToken))
                {
                    //XtraMessageBox.Show(_Messeg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string Status_Desc = "Invvalid Token";
            string Status_CD = "";
            string AckNo = "";
            string AckDt = "";
            string Irn = "";
            string SignedInvoice = "";
            string SignedQRCode = "";
            string Status = "";
            string Remarks = "";

            if (!string.IsNullOrWhiteSpace(AuthToken))
            {
                //string strUrl = string.Format("https://api.mastergst.com/einvoice/type/GENERATE/version/V1_03?email=shariztechmalegaon%40gmail.com");

                string strUrl = string.Format("https://api.mastergst.com/einvoice/type/GENERATE/version/V1_03?email=erp%40weblinkservices.net");

                WebRequest requestObjPost = WebRequest.Create(strUrl);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";
                requestObjPost.Headers.Add("ip_address", IPAddress);
                requestObjPost.Headers.Add("client_id", E_Invoice_Client_ID);
                requestObjPost.Headers.Add("client_secret", E_Invoice_Secret);
                requestObjPost.Headers.Add("username", User_Name);
                requestObjPost.Headers.Add("auth-token", AuthToken);
                requestObjPost.Headers.Add("gstin", GST);

                //check Company Type
                SqlCommand cmdCompName = new SqlCommand("select BillingCustomer from tbltaxinvoicehdr where Id='" + ID + "'", con);
                Object F_CompNamec = cmdCompName.ExecuteScalar();
                string BillingCust = F_CompNamec.ToString();

                //SqlCommand cmdInvbasic = new SqlCommand("select E_inv_Typeof_supply from Company where cname='" + BillingCust + "'", con);
                SqlCommand cmdInvbasic = new SqlCommand("select TOP 1 E_inv_Typeof_supply from tbl_CompanyMaster where Companyname='" + BillingCust + "' order by id desc", con);
                Object F_cmdInvbasic = cmdInvbasic.ExecuteScalar();
                string Invbasic = F_cmdInvbasic.ToString();

                string postData = "";

                #region Code After 23-09-23 for Modification of E-Way Bill
                // Dispatch Details Removed & Billing Shipping Condition
                //check Shipping Cust
                SqlCommand cmdShippingCompName = new SqlCommand("select ShippingCustomer from tbltaxinvoicehdr where Id='" + ID + "'", con);
                Object F_ShippingCompNamec = cmdShippingCompName.ExecuteScalar();
                string ShippingCust = F_ShippingCompNamec.ToString();

                SqlCommand cmdShippingAddress = new SqlCommand("select ShortSAddress from tbltaxinvoicehdr where Id='" + ID + "'", con);
                Object F_ShippingAddress = cmdShippingAddress.ExecuteScalar();
                string ShippingAdd = F_ShippingAddress.ToString();

                SqlCommand cmdBillingAddress = new SqlCommand("select ShortBAddress from tbltaxinvoicehdr where Id='" + ID + "'", con);
                Object F_BillingAddress = cmdBillingAddress.ExecuteScalar();
                string BillingAdd = F_BillingAddress.ToString();

                if (ShippingAdd == BillingAdd)
                {
                    if (Invbasic == "B2B")
                    {
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"B2B\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"OthChrg\":" + OthChrg + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "SEZWOP")
                    {
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"SEZWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"OthChrg\":" + OthChrg + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "EXPWOP")
                    {
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"EXPWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"ExpDtls\":{\"CntCode\":\"" + CountryCode + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"OthChrg\":" + OthChrg + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                }
                else if (ShippingAdd != BillingAdd)
                {
                    if (Invbasic == "B2B")
                    {
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"B2B\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"OthChrg\":" + OthChrg + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "SEZWOP")
                    {
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"SEZWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"OthChrg\":" + OthChrg + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "EXPWOP")
                    {
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"EXPWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"ExpDtls\":{\"CntCode\":\"" + CountryCode + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"OthChrg\":" + OthChrg + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                }
                #endregion

                if (postData != "")
                {
                    //Save JSON IN DATABASE update by pawar 28/09/2024                 
                    Cmd = new SqlCommand("UPDATE tblTaxInvoiceHdr SET E_Invoice_JSON=@E_Invoice_JSON WHERE Id=" + ID + "", con);
                    Cmd.Parameters.AddWithValue("@E_Invoice_JSON", postData);
                    Cmd.ExecuteNonQuery();
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
                            AckNo = "";
                            AckDt = "";
                            Irn = "";
                            SignedInvoice = "";
                            SignedQRCode = "";
                            Status = "";
                            Remarks = "";

                            if (Status_CD != "0")
                            {
                                AckNo = JsonRespons["data"]["AckNo"].ToString();
                                AckDt = JsonRespons["data"]["AckDt"].ToString();
                                Irn = JsonRespons["data"]["Irn"].ToString();
                                SignedInvoice = JsonRespons["data"]["SignedInvoice"].ToString();
                                SignedQRCode = JsonRespons["data"]["SignedQRCode"].ToString();
                                Status = JsonRespons["data"]["Status"].ToString();
                                Remarks = JsonRespons["data"]["Remarks"].ToString();
                            }
                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Type of Supply For E-Invoice Not Set in Company Master. Kindly Check Company Master...!');", true);
                }
            }

            //Return Value
            _AckNo = AckNo;
            _AckDt = AckDt;
            _Irn = Irn;
            _SignedInvoice = SignedInvoice;
            _SignedQRCode = SignedQRCode;
            _Status = Status;
            _Remarks = Remarks;
            _Messege = Status_Desc;
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    #endregion

    private string GenrateAuthKey(string User_Name, string Password, string Seller_GST_No, out string _TokenExpiry, out string _Sek, out string _AuthKey, out string _Messege)
    {

        //string uri = "https://api.mastergst.com/einvoice/authenticate?email=shariztechmalegaon@gmail.com";
        string uri = "https://api.mastergst.com/einvoice/authenticate?email=erp%40weblinkservices.net";
        WebResponse response;
        WebRequest request = WebRequest.Create(uri);

        request.Method = "GET";
        request.Headers.Add("username", User_Name);
        request.Headers.Add("password", Password);
        request.Headers.Add("ip_address", IPAddress);
        request.Headers.Add("client_id", E_Invoice_Client_ID);
        request.Headers.Add("client_secret", E_Invoice_Secret);
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

                Cmd = new SqlCommand("DELETE FROM AUTH_TOKEN_MASTER", con);
                Cmd.ExecuteNonQuery();

                Cmd = new SqlCommand("INSERT INTO AUTH_TOKEN_MASTER (UserName,TokenExpiry,Sek,ClientId,AuthToken)VALUES(@UserName,@TokenExpiry,@Sek,@ClientId,@AuthToken)", con);
                Cmd.Parameters.AddWithValue("@UserName", User_Name);
                //Cmd.Parameters.AddWithValue("@TokenExpiry", Convert.ToDateTime(TokenExpiry).AddMinutes(-45));
                Cmd.Parameters.AddWithValue("@TokenExpiry", Convert.ToDateTime(TokenExpiry).AddMinutes(-120)); // updated on 25-9
                Cmd.Parameters.AddWithValue("@Sek", Sek);
                Cmd.Parameters.AddWithValue("@ClientId", ClientId);
                Cmd.Parameters.AddWithValue("@AuthToken", AuthKey);
                Cmd.ExecuteNonQuery();
            }

            _TokenExpiry = TokenExpiry;
            _Sek = Sek;
            _AuthKey = AuthKey;
            _Messege = Status_Desc;

            return AuthKey;
        }
    }

    protected void GvInvoiceList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            con.Open();
            LinkButton lnkCreateInv = (LinkButton)e.Row.FindControl("lnkCreateInv");
            LinkButton lnkCancel = (LinkButton)e.Row.FindControl("lnkCancel");
            LinkButton lnkPDF = (LinkButton)e.Row.FindControl("lnkPDF");
            Label lblInvoiceNo = (Label)e.Row.FindControl("lblInvoiceNo");

            int idd = Convert.ToInt32(GvInvoiceList.DataKeys[e.Row.RowIndex].Values[0]);
            DataTable Dtt = new DataTable();
            SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tbltaxinvoicehdr where Id = '" + idd + "'", con);
            Sdd.Fill(Dtt);
            if (Dtt.Rows.Count > 0)
            {
                string e_invoice_status = Dtt.Rows[0]["e_invoice_status"].ToString();
                string e_invoice_cancel_status = Dtt.Rows[0]["e_invoice_cancel_status"].ToString();

                if (e_invoice_status == true.ToString() && e_invoice_cancel_status == true.ToString())
                {
                    lnkCreateInv.Visible = false;
                    lnkPDF.Visible = true;
                    lnkCancel.Visible = false;
                    e.Row.BackColor = System.Drawing.Color.LightPink;
                }
                else if (e_invoice_status == true.ToString())
                {
                    lnkCreateInv.Visible = false;
                    lnkPDF.Visible = true;
                    lnkCancel.Visible = true;
                    e.Row.BackColor = System.Drawing.Color.LightGray;
                }
                if (lblInvoiceNo.Text == null || lblInvoiceNo.Text == "")
                {
                    lblInvoiceNo.Text = Dtt.Rows[0]["FinalBasic"].ToString();
                }
            }
            con.Close();
        }
    }


    #region Cancel E-INVOICE
    private void Cancel_EInv(string ID)
    {
        string _Status_CD = "", _Messege = "", _CancelDate = "";
        try
        {
            con.Open();
            bool F_e_invoice_status;
            //Check Already Canceled E Invoice
            SqlCommand cmde_invoice_status = new SqlCommand("select e_invoice_cancel_status from tblTaxInvoiceHdr where Id='" + ID + "'", con);
            Object e_invoice_status = cmde_invoice_status.ExecuteScalar();
            if (e_invoice_status == "")
            {
                F_e_invoice_status = true;
            }
            else
            {
                F_e_invoice_status = false;
            }

            if (F_e_invoice_status == true)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Already Canceled ..!!');", true);
            }
            else
            {
                SqlCommand cmdGetIRN = new SqlCommand("select IRN from tblTaxInvoiceHdr where Id='" + ID + "'", con);
                Object F_GetIRN = cmdGetIRN.ExecuteScalar();

                Cancel_EInvoice(MailID, GST, UserName, Password, F_GetIRN.ToString(), out _Status_CD, out _Messege, out _CancelDate);

                if (_Status_CD == "1")
                {
                    con.Close();
                    if (_Messege == "[{\"ErrorCode\":\"2270\",\"ErrorMessage\":\"The allowed cancellation time limit is crossed, you cannot cancel the IRN\"}]")
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('The Allowed Cancellation Time limit is Crossed. You Cannot cancel the IRN..!!');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + _Messege + "');", true);
                    }
                }
                else
                {

                    Cmd = new SqlCommand("UPDATE tblTaxInvoiceHdr SET e_invoice_cancel_status=@e_invoice_cancel_status,e_invoice_cancel_date=@e_invoice_cancel_date,e_invoice_cancel_by=@e_invoice_cancel_by WHERE Id=" + ID + "", con);
                    Cmd.Parameters.AddWithValue("@e_invoice_cancel_status", 1);
                    Cmd.Parameters.AddWithValue("@e_invoice_cancel_date", DateTime.Now);
                    Cmd.Parameters.AddWithValue("@e_invoice_cancel_by", Session["Username"].ToString());
                    Cmd.ExecuteNonQuery();

                    SqlCommand cmd1 = new SqlCommand("select InvoiceNo from tblTaxInvoiceHdr where Id = " + Convert.ToInt32(ID) + "", con);
                    Object InvoiceNo = cmd1.ExecuteScalar();

                    //Delete Outward Data For Inventory
                    SqlCommand Cmd2 = new SqlCommand("delete tbl_OutwardEntryHdr where invoiceno=@invoiceno", con);
                    Cmd2.Parameters.AddWithValue("@invoiceno", InvoiceNo);
                    Cmd2.ExecuteNonQuery();

                    SqlCommand Cmd3 = new SqlCommand("delete tbl_OutwardEntryDtls where RefNo=@invoiceno", con);
                    Cmd3.Parameters.AddWithValue("@invoiceno", InvoiceNo);
                    Cmd3.ExecuteNonQuery();

                    SqlCommand Cmd4 = new SqlCommand("delete tbl_OutwardEntryComponentsDtls where OrderNo=@invoiceno", con);
                    Cmd4.Parameters.AddWithValue("@invoiceno", InvoiceNo);
                    Cmd4.ExecuteNonQuery();

                    con.Close();

                    SqlCommand cmddelete2 = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + InvoiceNo + "' ", con);
                    con.Open();
                    cmddelete2.ExecuteNonQuery();
                    con.Close();

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Cancelled Successfully...!!');window.location='EInvoiceList.aspx';", true);
                    // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Cancelled Successfully...!!');", true);

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
    protected void Cancel_EInvoice(string MailID, string GSTNo, string UserName, string Password, string Irn, out string _Status_CD, out string _Messege, out string _CancelDate)
    {
        _Status_CD = "";
        _Messege = "";
        _CancelDate = "";
        try
        {
            //AuthToken
            string _TokenExpiry = "", _Sek = "", _AuthKey = "", _Messeg = "";
            DataTable DT_AuthToken = new DataTable();
            SqlDataAdapter sad_AuthToken = new SqlDataAdapter("SELECT * FROM AUTH_TOKEN_MASTER WHERE UserName='" + UserName + "'", con);
            sad_AuthToken.Fill(DT_AuthToken);
            if (DT_AuthToken.Rows.Count > 0)
            {
                DateTime Token_Expiry = Convert.ToDateTime(DT_AuthToken.Rows[0]["TokenExpiry"]);

                if (Token_Expiry <= DateTime.Now)
                {
                    GenrateAuthKey(UserName, Password, GSTNo, out _TokenExpiry, out _Sek, out _AuthKey, out _Messeg);

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
            else
            {
                GenrateAuthKey(UserName, Password, GSTNo, out _TokenExpiry, out _Sek, out _AuthKey, out _Messeg);
                AuthToken = _AuthKey;
                if (string.IsNullOrWhiteSpace(AuthToken))
                {
                    //XtraMessageBox.Show(_Messeg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            string Status_Desc = "Invvalid Token";
            string Status_CD = "";
            string CancelDate = "";

            if (!string.IsNullOrWhiteSpace(AuthToken))
            {
                string strUrl = string.Format("https://api.mastergst.com/einvoice/type/CANCEL/version/V1_03?email=erp%40weblinkservices.net");

                WebRequest requestObjPost = WebRequest.Create(strUrl);
                requestObjPost.Method = "POST";
                requestObjPost.ContentType = "application/json";
                requestObjPost.Headers.Add("email", MailID);
                requestObjPost.Headers.Add("ip_address", "");
                requestObjPost.Headers.Add("client_id", E_Invoice_Client_ID);
                requestObjPost.Headers.Add("client_secret", E_Invoice_Secret);
                requestObjPost.Headers.Add("username", UserName);
                requestObjPost.Headers.Add("auth-token", AuthToken);
                requestObjPost.Headers.Add("gstin", GST);

                string postData = "{\"Irn\":\"" + Irn + "\",\"CnlRsn\":\"1\",\"CnlRem\":\"Wrong entry\"}";

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

                        if (Status_CD != "0")
                        {
                            CancelDate = JsonRespons["data"]["CancelDate"].ToString();
                        }
                    }
                }
            }

            //Return Value     
            _Messege = Status_Desc;
            _CancelDate = CancelDate;
            _Status_CD = Status_CD;
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }
    }
    #endregion   


    protected void btnewaybill_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Admin/EWayBillList.aspx");
    }

    public void CancelReport(string Invoiceno, string mail)
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
                        imageBytes = GenerateQR(F_Qrdtl);
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


                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\CancelEInvoice.rdlc";


                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    string filePath = Server.MapPath("~/PDF_Files/") + "CancelEInvoice.pdf";

                    // Save the file to the specified path
                    System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                    Response.Redirect("~/PDF_Files/CancelEInvoice.pdf");
                    //Response.ClearContent();
                    //Response.ClearHeaders();
                    //if (mail == "show")
                    ////{
                    ////    Response.Buffer = true;
                    ////    string Filename = Invoiceno + "_EInvoice.pdf";
                    ////    Response.ContentType = "application/vnd.pdf";
                    ////    Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "");
                    ////    Response.BinaryWrite(bytePdfRep);
                    ////}
                    //ReportViewer1.LocalReport.DataSources.Clear();
                    //ReportViewer1.Reset();

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

    private byte[] GenerateQR(string QR_String)
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

    public void EInvoiceReports(string Invoiceno, string mail, string type)
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
                    string id = ViewState["PDFID"].ToString();
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
                        imageBytes = GenerateQR(F_Qrdtl);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("QRCodeImage", typeof(byte[]));
                    dt.Columns.Add("Type", typeof(string));
                    DataRow row = dt.NewRow();
                    if (type == "ORIGINAL")
                    {  // Add the QR code image to the table
                        row["QRCodeImage"] = imageBytes;
                        row["Type"] = "(ORIGINAL FOR RECIPIENT)";
                        dt.Rows.Add(row);
                    }
                    if (type == "DUPLICATE")
                    {
                        // Add the QR code image to the table
                        row["QRCodeImage"] = imageBytes;
                        row["Type"] = "(DUPLICATE FOR TRANSPORTER)";
                        dt.Rows.Add(row);

                    }
                    if (type == "TRIPLICATE")
                    {
                        // Add the QR code image to the table
                        row["QRCodeImage"] = imageBytes;
                        row["Type"] = "(TRIPLICATE FOR SUPPLIER)";
                        dt.Rows.Add(row);
                    }

                    // DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                    ReportDataSource obj4 = new ReportDataSource("DataSet4", dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.DataSources.Add(obj4);

                    string e_invoice_cancel_status = Dtt.Tables[0].Rows[0]["e_invoice_cancel_status"].ToString();
                    if (e_invoice_cancel_status == true.ToString())
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\CancelEInvoice.rdlc";
                    }
                    else
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\EInvoice.rdlc";
                    }

                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    Response.ClearContent();
                    Response.ClearHeaders();
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.Reset();



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

    public static void MergePDFFiles(string[] fileArray, string outputFile)
    {
        // Create a Document object to hold the final merged PDF
        Document document = new Document();
        try
        {
            // Create a PdfCopy instance to write the combined PDF
            PdfCopy writer = new PdfCopy(document, new FileStream(outputFile, FileMode.Create));
            document.Open();

            PdfReader reader;
            PdfImportedPage page;

            // Iterate over each PDF file to merge
            foreach (string file in fileArray)
            {
                reader = new PdfReader(file);
                int numberOfPages = reader.NumberOfPages;

                // Copy each page from the current PDF to the final document
                for (int i = 1; i <= numberOfPages; i++)
                {
                    page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            // Close the document after all pages have been added
            document.Close();
        }
    }

}

