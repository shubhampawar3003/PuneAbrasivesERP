using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Gov_Bills_EInv_CrDbNote : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

    private readonly DateTimeFormatInfo UkDtfi = new CultureInfo("en-GB", false).DateTimeFormat;

    private SqlCommand Cmd;
    public string AuthToken = "";


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
            //SqlDataAdapter sad = new SqlDataAdapter("select TOP 10 * from tblCreditDebitNoteHdr where isdeleted='0' order by CreatedOn DESC", con);
            SqlDataAdapter sad = new SqlDataAdapter("select TOP 10 * from tblCreditDebitNoteHdr where NoteFor='Sale' order by Id desc", con);
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
                //com.CommandText = "select DISTINCT BillingCustomer from tblCreditDebitNoteHdr where " + "BillingCustomer like @Search + '%' AND isdeleted='0'  ";
                com.CommandText = "select DISTINCT SupplierName from tblCreditDebitNoteHdr where " + "SupplierName like @Search + '%' AND isdeleted='0'  ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> BillingCustomer = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        BillingCustomer.Add(sdr["SupplierName"].ToString());
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
            //SqlDataAdapter sad = new SqlDataAdapter("select * from tblCreditDebitNoteHdr where BillingCustomer='" + txtCustomerName.Text + "' AND isdeleted='0' order by CreatedOn DESC", con);
            SqlDataAdapter sad = new SqlDataAdapter("select * from tblCreditDebitNoteHdr where SupplierName='" + txtCustomerName.Text + "' AND isdeleted='0' order by CreatedOn DESC", con);
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
        Response.Redirect("EInv_CrDbNote.aspx");
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
                Session["PDFID"] = e.CommandArgument.ToString();
                Response.Redirect("E_InvoicePDF.aspx?Idd=" + encrypt(e.CommandArgument.ToString()) + "");
                // Response.Write("<script>window.open('PurchaseBillPDF.aspx','_blank');</script>");
                //  Response.Write("<script>window.open ('E_InvoicePDF.aspx?Idd=" + encrypt(e.CommandArgument.ToString()) + "','_blank');</script>");
            }
        }
        if (e.CommandName == "RowCancel")
        {
            string ID = e.CommandArgument.ToString();
            Cancel_EInv(ID);
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

            //Check API Configuration
            //string UserName = "mastergst";
            //string Password = "Malli#123";

            // Excel Live Credentials
            //string UserName = "API_ExcelEnclosures";
            //string Password = "ExcelEnc@Admin@123";

            //Check Already genereate E Invoice
            SqlCommand cmde_invoice_status = new SqlCommand("select e_invoice_status from tblCreditDebitNoteHdr where Id='" + ID + "'", con);
            Object e_invoice_status = cmde_invoice_status.ExecuteScalar();

            if (e_invoice_status == "1")
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Already Genereted Please Select Another..!!');", true);
            }
            else
            {
                //SqlCommand cmdCname = new SqlCommand("select BillingCustomer from tblCreditDebitNoteHdr where Id='" + ID + "'", con);
                //Object F_Cname = cmdCname.ExecuteScalar();
                //Get Company Details from invoice Table
                DataTable dtCompany = new DataTable();
                SqlDataAdapter sadCompany = new SqlDataAdapter("select * from [tblCreditDebitNoteHdr] where Id='" + ID + "'", con);
                sadCompany.Fill(dtCompany);

                #region Seller Details

                //Declare Variables
                string Seller_Gst_No = "", Invoice_No = "", Invoice_No_Export = "", CountryCode_Export = "", Transaction_Date = "", Seller_Firm_Name = "", Seller_Firm_Address = "", Seller_Location = "", Seller_Pin_Code = "", Seller_State_Code = "";

                //Set variable
                //Seller_Gst_No = "29AABCT1332L000";  // for testing
                Seller_Gst_No = GST; // for excel
                if (string.IsNullOrWhiteSpace(Seller_Gst_No))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller GST No Not Set Please Enter Seller GST No..!');", true);
                }
                //Invoice_No = grd_E_Invoice.GetFocusedRowCellValue("Invoice_No").ToString();
                //Invoice_No = "INV027758";

                //updated changes for e-Inv at 19-12-23
                //string Invoice_No = dtCompany.Rows[0]["invoiceno"].ToString();
                //if (string.IsNullOrWhiteSpace(Invoice_No))
                //{
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Invoice_No Not Set Please Enter Invoice_NO..!');", true);
                //}

                //string InvNoChk = dtCompany.Rows[0]["DocNo"].ToString();
                //if (InvNoChk == null || InvNoChk == "")
                //{
                //    Invoice_No = dtCompany.Rows[0]["FinalBasic"].ToString();
                //}
                //else
                //{
                //    Invoice_No = dtCompany.Rows[0]["invoiceno"].ToString();
                //}

                Invoice_No = dtCompany.Rows[0]["DocNo"].ToString();
                //Invoice_No = "CRDB/4";//dtCompany.Rows[0]["DocNo"].ToString(); //For Testing

                if (string.IsNullOrWhiteSpace(Invoice_No))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('DocNo Not Set Please Enter Invoice_NO..!');", true);
                }

                //DateTime ffff1 = Convert.ToDateTime(dtCompany.Rows[0]["invoicedate"].ToString());
                string ffff1 = dtCompany.Rows[0]["DocDate"].ToString();
                //Transaction_Date = ffff1.ToString("dd/MM/yyyy");
                //Transaction_Date = "10/09/2023";
                Transaction_Date = ffff1.Replace("-", "/");
                //string Transaction_Date1 = "10/09/2023";

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

                Seller_Pin_Code = "411019";  //for Pune Abrasives
                //Seller_Pin_Code = "587315";  // for testing
                if (string.IsNullOrWhiteSpace(Seller_Pin_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Seller_Pin_Code Not Set Please Enter Seller_Pin_Code..!');", true);
                }
                //Seller_State_Code = "29"; // for testing
                Seller_State_Code = "27"; // for Excel
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
                Buyer_Firm_Name = dtCompany.Rows[0]["SupplierName"].ToString();
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

                #region Dispatch Details
                ////declare variables
                //string Dispatch_Name = "", Dispatch_Address = "", Dispatch_Location = "", Dispatch_Pin_Code = "", Dispatch_State_Code = "";
                ////set variables
                //Dispatch_Name = cmb_Firm_Name.GetColumnValue("firm_name").ToString();
                //if (string.IsNullOrWhiteSpace(Dispatch_Name))
                //{
                //    XtraMessageBox.Show("Dispatch_Name Not Set Please Enter Dispatch_Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //Dispatch_Address = cmb_Firm_Name.GetColumnValue("address").ToString().Trim();
                //if (string.IsNullOrWhiteSpace(Dispatch_Address))
                //{
                //    XtraMessageBox.Show("Dispatch_Address Not Set Please Enter Dispatch_Address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //Dispatch_Location = cmb_Firm_Name.GetColumnValue("city_name").ToString();
                //if (string.IsNullOrWhiteSpace(Dispatch_Location))
                //{
                //    XtraMessageBox.Show("Dispatch_Location Not Set Please Enter Dispatch_Location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //Dispatch_Pin_Code = cmb_Firm_Name.GetColumnValue("pin_code").ToString();
                //if (string.IsNullOrWhiteSpace(Dispatch_Pin_Code))
                //{
                //    XtraMessageBox.Show("Dispatch_Pin_Code Not Set Please Enter Dispatch_Pin_Code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                //Dispatch_State_Code = cmb_Firm_Name.GetColumnValue("state_code").ToString();
                //if (string.IsNullOrWhiteSpace(Dispatch_State_Code))
                //{
                //    XtraMessageBox.Show("Dispatch_State_Code Not Set Please Enter Dispatch_Pin_Code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}
                #endregion

                // For Export Invoice on 20-12-23 Start
                #region Export Details
                //string InvNoChkExp = dtCompany.Rows[0]["invoiceno"].ToString();
                //if (InvNoChkExp == null || InvNoChkExp == "")
                //{
                //    Invoice_No_Export = dtCompany.Rows[0]["FinalBasic"].ToString();

                //    SqlCommand cmd_CountryCode_status = new SqlCommand("select CountryCode from Company where cname='" + Buyer_Firm_Name + "'", con);
                //    Object CountryCode = cmd_CountryCode_status.ExecuteScalar();
                //    CountryCode_Export = CountryCode.ToString();
                //    if (string.IsNullOrWhiteSpace(CountryCode_Export))
                //    {
                //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('CountryCode_Export Not Set Please Enter CountryCode_Export..!!');", true);
                //    }
                //}
                #endregion
                // For Export Invoice on 20-12-23 End

                #region Ship Party Details
                //Declare Variables
                string Ship_Firm_Name = "", Ship_Firm_Address = "", Ship_Location = "", Ship_Pin_Code = "", Ship_Firm_State_Code = "";
                //Set variables

                Ship_Firm_Name = dtCompany.Rows[0]["ShippingCustomer"].ToString();
                // Ship_Firm_Name = "Web Link Services Pvt. Ltd.";
                if (string.IsNullOrWhiteSpace(Ship_Firm_Name))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Firm_Name Not Set Please Enter Ship_Firm_Name..!!');", true);
                }
                string Ship_Firm_Address1 = dtCompany.Rows[0]["ShortSAddress"].ToString();
                // Ship_Firm_Address = dtCompany.Rows[0]["ShippingAddress"].ToString();
                Ship_Firm_Address = Regex.Replace(Ship_Firm_Address1, @"\s+", " ");
                // Ship_Firm_Address = "Pimpale Saudagar";
                if (string.IsNullOrWhiteSpace(Ship_Firm_Address))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Firm_Address Not Set Please Enter Ship_Firm_Address..!!');", true);
                }
                Ship_Location = dtCompany.Rows[0]["ShippingLocation"].ToString();
                // Ship_Location = "Pune";
                if (string.IsNullOrWhiteSpace(Ship_Location))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Location Not Set Please Enter Ship_Location..!!');", true);
                }
                Ship_Pin_Code = dtCompany.Rows[0]["ShippingPincode"].ToString();
                // Ship_Pin_Code = "411062";
                if (string.IsNullOrWhiteSpace(Ship_Pin_Code))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Ship_Pin_Code Not Set Please Enter Ship_Pin_Code..!!');", true);
                }
                Ship_Firm_State_Code = dtCompany.Rows[0]["ShippingStatecode"].ToString();
                // Ship_Firm_State_Code = "27";
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
                SqlCommand cmdFright = new SqlCommand("select Basic from [tblCreditDebitNoteHdr] where id='" + ID + "'", con);
                Object F_Fright = cmdFright.ExecuteScalar();
                string FrightValuee = F_Fright.ToString();
                if (FrightValuee == "0")
                {
                    //DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from tblcreditdebitnotedtls where HeaderId='" + ID + "'", con);
                    sad.Fill(dt);
                }
                else
                {
                    //DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from VW_EINVTransport_CrDbNote_Sale where HeaderId='" + ID + "'", con);
                    sad.Fill(dt);
                }
                //Fright Charges Check End

                //DataTable dt = new DataTable();
                //SqlDataAdapter sad = new SqlDataAdapter("select * from tblcreditdebitnotedtls where HeaderId='" + ID + "'", con);
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
                    double Discount_Per = Convert.ToDouble(dt.Rows[i]["Discount"].ToString());
                    double Discount_Amt = (Basic * Discount_Per) / 100;
                    Item_Discount = Discount_Amt.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(Item_Discount))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Discount Not Set Please Enter Discount....!!');", true);
                    }
                    string cgstper = dt.Rows[i]["CGSTPer"].ToString();
                    string igstper = dt.Rows[i]["IGSTPer"].ToString();
                    if (cgstper != "0")
                    {
                        GstRt = "18";
                    }
                    else if (igstper != "0")
                    {
                        GstRt = "18";
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
                    object Obj_TotItemVal = dt.Rows[i]["Total"].ToString();
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
                    //itemlist.Append("{\"SlNo\":\"" + cnt + "\",\"IsServc\":\"" + IsServc + "\",\"HsnCd\":\"" + HsnCd + "\",\"BchDtls\":{\"Nm\":\"123456\"},\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}");
                    itemlist.Append("{\"SlNo\":\"" + cnt + "\",\"IsServc\":\"" + IsServc + "\",\"HsnCd\":\"" + HsnCd + "\",\"Qty\":" + Qty + ",\"Unit\":\"" + Unit + "\",\"UnitPrice\":" + UnitPrice + ",\"TotAmt\":" + TotAmt + ",\"Discount\":" + Item_Discount + ",\"AssAmt\":" + AssAmt + ",\"GstRt\":" + GstRt + ",\"CgstAmt\":" + CgstAmt + ",\"SgstAmt\":" + SgstAmt + ",\"IgstAmt\":" + IgstAmt + ",\"TotItemVal\":" + TotItemVal + "}");


                    itemlist.Append(",");
                }
                //}

                char[] charsToTrim = { ',', ' ' };
                string list = itemlist.ToString().Trim().TrimEnd(charsToTrim);
                string AssVal;
                if (FrightValuee == "0")
                {
                    //Total Basic of all products
                    SqlCommand cmdAssVal = new SqlCommand("select SUM(cast(Amount as float)) FROM [tblcreditdebitnotedtls] where headerId='" + ID + "'", con);
                    Object F_AssVal = cmdAssVal.ExecuteScalar();
                    double AssValValuee = Convert.ToDouble(F_AssVal);
                    AssVal = AssValValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(AssVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final AssVal Not Set Please Enter AssVal....!!');", true);
                    }

                    // Final Grandtotal
                    //SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(GrandTotal as float)) FROM [tblcreditdebitnotedtls] where headerId='" + ID + "'", con);
                    SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(Total as float)) FROM [tblcreditdebitnotedtls] where headerId='" + ID + "'", con);
                    Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                    double TotInvValuee = Convert.ToDouble(F_TotInvVal);
                    TotInvVal = TotInvValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(TotInvVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final TotInvVal Not Set Please Enter AssVal....!!');", true);
                    }

                    // Final SgstVal
                    SqlCommand cmdSgstVal = new SqlCommand("select SUM(cast(SGSTAmt as float)) FROM [tblcreditdebitnotedtls] where headerId='" + ID + "'", con);
                    Object F_SgstVal = cmdSgstVal.ExecuteScalar();
                    double sgstValuee = Convert.ToDouble(F_SgstVal);
                    SgstVal = sgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(SgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final SGSTAmt Not Set Please Enter SGSTAmt....!!');", true);
                    }

                    // Final CgstVal
                    SqlCommand cmdCgstVal = new SqlCommand("select SUM(cast(CGSTAmt as float)) FROM [tblcreditdebitnotedtls] where headerId='" + ID + "'", con);
                    Object F_CgstVal = cmdCgstVal.ExecuteScalar();
                    double cgstValuee = Convert.ToDouble(F_CgstVal);
                    CgstVal = cgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(CgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final CGSTAmt Not Set Please Enter CGSTAmt....!!');", true);
                    }

                    // Final IgstVal
                    SqlCommand cmdIgstVal = new SqlCommand("select SUM(cast(IGSTAmt as float)) FROM [tblcreditdebitnotedtls] where headerId='" + ID + "'", con);
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
                    SqlCommand cmdAssVal = new SqlCommand("select SUM(cast(Amount as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + ID + "'", con);
                    Object F_AssVal = cmdAssVal.ExecuteScalar();
                    double AssValValuee = Convert.ToDouble(F_AssVal);
                    AssVal = AssValValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(AssVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final AssVal Not Set Please Enter AssVal....!!');", true);
                    }

                    // Final Grandtotal
                    SqlCommand cmdTotInvVal = new SqlCommand("select SUM(cast(Total as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + ID + "'", con);
                    Object F_TotInvVal = cmdTotInvVal.ExecuteScalar();
                    double TotInvValuee = Convert.ToDouble(F_TotInvVal);
                    TotInvVal = TotInvValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(TotInvVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final TotInvVal Not Set Please Enter AssVal....!!');", true);
                    }

                    // Final SgstVal
                    SqlCommand cmdSgstVal = new SqlCommand("select SUM(cast(SGSTAmt as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + ID + "'", con);
                    Object F_SgstVal = cmdSgstVal.ExecuteScalar();
                    double sgstValuee = Convert.ToDouble(F_SgstVal);
                    SgstVal = sgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(SgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final SGSTAmt Not Set Please Enter SGSTAmt....!!');", true);
                    }

                    // Final CgstVal
                    SqlCommand cmdCgstVal = new SqlCommand("select SUM(cast(CGSTAmt as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + ID + "'", con);
                    Object F_CgstVal = cmdCgstVal.ExecuteScalar();
                    double cgstValuee = Convert.ToDouble(F_CgstVal);
                    CgstVal = cgstValuee.ToString("0.00");
                    if (string.IsNullOrWhiteSpace(CgstVal))
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Final CGSTAmt Not Set Please Enter CGSTAmt....!!');", true);
                    }

                    // Final IgstVal
                    SqlCommand cmdIgstVal = new SqlCommand("select SUM(cast(IGSTAmt as float)) FROM [VW_EINVTransport_CrDbNote_Sale] where headerId='" + ID + "'", con);
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
                   Seller_Location, Seller_Pin_Code, Seller_State_Code, Buyer_GST_No, Buyer_Firm_Name, Buyer_Address, Buyer_Location, Buyer_Pin_Code, Buyer_State_Code, Seller_Firm_Name, Seller_Firm_Address, Seller_Location, Seller_Pin_Code, Seller_State_Code, Ship_Firm_Name, Ship_Firm_Address, Ship_Location, Ship_Pin_Code, Ship_Firm_State_Code, list, AssVal, TotInvVal, SgstVal, CgstVal, IgstVal, out _AckNo, out _AckDt, out _Irn, out _SignedInvoice, out _SignedQRCode, out _Status, out _Remarks, out _Messege);
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
                    Cmd = new SqlCommand("UPDATE tblCreditDebitNoteHdr SET AckNo=@AckNo,AckDt=@AckDt,Irn=@Irn,SignedInvoice=@SignedInvoice,SignedQRCode=@SignedQRCode,Status=@Status,Remarks=@Remarks,e_invoice_status=@e_invoice_status,e_invoice_created_by=@e_invoice_created_by WHERE Id=" + ID + "", con);
                    Cmd.Parameters.AddWithValue("@AckNo", _AckNo);
                    Cmd.Parameters.AddWithValue("@AckDt", _AckDt);
                    Cmd.Parameters.AddWithValue("@Irn", _Irn);
                    Cmd.Parameters.AddWithValue("@SignedInvoice", _SignedInvoice);
                    Cmd.Parameters.AddWithValue("@SignedQRCode", _SignedQRCode);
                    Cmd.Parameters.AddWithValue("@Status", _Status);
                    Cmd.Parameters.AddWithValue("@Remarks", _Remarks);
                    Cmd.Parameters.AddWithValue("@e_invoice_status", 1);
                    Cmd.Parameters.AddWithValue("@e_invoice_created_by", Session["Username"].ToString());
                    Cmd.ExecuteNonQuery();
                    con.Close();
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Genereted Successfully...!!');", true);
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
    //string OthChrg, string RndOffAmt removed
    protected void EInvoice(string User_Name, string Password, string Seller_GST_No, string ID, string Invoice_No, string CountryCode, string Date_Dt, string Seller_Firm_Name, string Seller_Address, string Seller_location, string Seller_Pin_Code, string Seller_State_Code, string Buyer_GST_No, string Buyer_Firm_Name, string Buyer_Address, string Buyer_Location, string Buyer_Pin_Code, string Buyer_State_Code, string Dispatch_Name, string Dispatch_Address, string Dispatch_Location, string Dispatch_Pin_Code, string Dispatch_State_Code, string Ship_Firm_Name, string Ship_Address, string Ship_Location, string Ship_Pin_Code, string Ship_State_Code, string list, string AssVal, string TotInvVal, string SgstVal, string CgstVal, string IgstVal, out string _AckNo, out string _AckDt, out string _Irn, out object _SignedInvoice, out string _SignedQRCode, out string _Status, out string _Remarks, out string _Messege)
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
                    AuthToken = DT_AuthToken.Rows[0]["AuthToken"].ToString();
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
            string DocType = "";

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
                SqlCommand cmdCompName = new SqlCommand("select SupplierName from tblcreditdebitnotehdr where Id='" + ID + "'", con);
                Object F_CompNamec = cmdCompName.ExecuteScalar();
                string BillingCust = F_CompNamec.ToString();

                //SqlCommand cmdInvbasic = new SqlCommand("select E_inv_Typeof_supply from Company where cname='" + BillingCust + "'", con);
                SqlCommand cmdInvbasic = new SqlCommand("select TOP 1 E_inv_Typeof_supply from tbl_CompanyMaster where Companyname='" + BillingCust + "' order by id desc", con);
                Object F_cmdInvbasic = cmdInvbasic.ExecuteScalar();
                string Invbasic = F_cmdInvbasic.ToString();

                string postData = "";

                #region Code Before 22-09-23
                //if (Invbasic == "B2B")
                //{
                //    //Regular Invoice
                //    postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"B2B\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"DispDtls\":{\"Nm\":\"" + Dispatch_Name + "\",\"Addr1\":\"" + Dispatch_Address + "\",\"Loc\":\"" + Dispatch_Location + "\",\"Pin\":" + Dispatch_Pin_Code + ",\"Stcd\":\"" + Dispatch_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                //}
                //else if (Invbasic == "SEZWOP")
                //{
                //    //Export Invoice
                //    postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"SEZWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"DispDtls\":{\"Nm\":\"" + Dispatch_Name + "\",\"Addr1\":\"" + Dispatch_Address + "\",\"Loc\":\"" + Dispatch_Location + "\",\"Pin\":" + Dispatch_Pin_Code + ",\"Stcd\":\"" + Dispatch_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                //}
                //else if (Invbasic == "EXPWOP")
                //{
                //    //Export Invoice
                //    postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"EXPWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"DispDtls\":{\"Nm\":\"" + Dispatch_Name + "\",\"Addr1\":\"" + Dispatch_Address + "\",\"Loc\":\"" + Dispatch_Location + "\",\"Pin\":" + Dispatch_Pin_Code + ",\"Stcd\":\"" + Dispatch_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                //}
                ////string postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"B2B\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"DispDtls\":{\"Nm\":\"" + Dispatch_Name + "\",\"Addr1\":\"" + Dispatch_Address + "\",\"Loc\":\"" + Dispatch_Location + "\",\"Pin\":" + Dispatch_Pin_Code + ",\"Stcd\":\"" + Dispatch_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                #endregion

                #region Code After 23-09-23 for Modification of E-Way Bill
                // Dispatch Details Removed & Billing Shipping Condition
                //check Shipping Cust
                SqlCommand cmdShippingCompName = new SqlCommand("select ShippingCustomer from tblcreditdebitnotehdr where Id='" + ID + "'", con);
                Object F_ShippingCompNamec = cmdShippingCompName.ExecuteScalar();
                string ShippingCust = F_ShippingCompNamec.ToString();

                SqlCommand cmdShippingAddress = new SqlCommand("select ShippingAddress from tblcreditdebitnotehdr where Id='" + ID + "'", con);
                Object F_ShippingAddress = cmdShippingAddress.ExecuteScalar();
                string ShippingAdd = F_ShippingAddress.ToString();

                SqlCommand cmdBillingAddress = new SqlCommand("select BillingAddress from tblcreditdebitnotehdr where Id='" + ID + "'", con);
                Object F_BillingAddress = cmdBillingAddress.ExecuteScalar();
                string BillingAdd = F_BillingAddress.ToString();

                SqlCommand cmdNoteType = new SqlCommand("select NoteType from tblcreditdebitnotehdr where Id='" + ID + "'", con);
                Object F_cmdNoteType = cmdNoteType.ExecuteScalar();
                string NoteType = F_cmdNoteType.ToString();
                if (NoteType == "Credit_Sale")
                {
                    DocType = "CRN";
                }
                else if (NoteType == "Debit_Sale")
                {
                    DocType = "DBN";
                }

                if (ShippingAdd == BillingAdd)
                {
                    if (Invbasic == "B2B")
                    {
                        //Regular Invoice
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"B2B\"},\"DocDtls\":{\"Typ\":\"" + DocType + "\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "SEZWOP")
                    {
                        //Export Invoice
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"SEZWOP\"},\"DocDtls\":{\"Typ\":\"" + DocType + "\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "EXPWOP")
                    {
                        //Export Invoice
                        //postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"EXPWOP\"},\"DocDtls\":{\"Typ\":\"INV\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";  //commented on 20-12-23 for Export E-inv
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"EXPWOP\"},\"DocDtls\":{\"Typ\":\"" + DocType + "\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"ExpDtls\":{\"CntCode\":\"" + CountryCode + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                }
                else if (ShippingAdd != BillingAdd)
                {
                    if (Invbasic == "B2B")
                    {
                        //Regular Invoice
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"B2B\"},\"DocDtls\":{\"Typ\":\"" + DocType + "\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "SEZWOP")
                    {
                        //Export Invoice
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"SEZWOP\"},\"DocDtls\":{\"Typ\":\"" + DocType + "\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                    else if (Invbasic == "EXPWOP")
                    {
                        //Export Invoice
                        postData = "{\"Version\":\"1.1\",\"TranDtls\":{\"TaxSch\":\"GST\",\"SupTyp\":\"EXPWOP\"},\"DocDtls\":{\"Typ\":\"" + DocType + "\",\"No\":\"" + Invoice_No + "\",\"Dt\":\"" + Date_Dt + "\"},\"ExpDtls\":{\"CntCode\":\"" + CountryCode + "\"},\"SellerDtls\":{\"Gstin\":\"" + Seller_GST_No + "\",\"LglNm\":\"" + Seller_Firm_Name + "\",\"Addr1\":\"" + Seller_Address + "\",\"Loc\":\"" + Seller_location + "\",\"Pin\":" + Seller_Pin_Code + ",\"Stcd\":\"" + Seller_State_Code + "\"},\"BuyerDtls\":{\"Gstin\":\"" + Buyer_GST_No + "\",\"LglNm\":\"" + Buyer_Firm_Name + "\",\"Pos\":\"" + Buyer_State_Code + "\",\"Addr1\":\"" + Buyer_Address + "\",\"Loc\":\"" + Buyer_Location + "\",\"Pin\":" + Buyer_Pin_Code + ",\"Stcd\":\"" + Buyer_State_Code + "\"},\"ShipDtls\":{\"LglNm\":\"" + Ship_Firm_Name + "\",\"Addr1\":\"" + Ship_Address + "\",\"Loc\":\"" + Ship_Location + "\",\"Pin\":" + Ship_Pin_Code + ",\"Stcd\":\"" + Ship_State_Code + "\"},\"ItemList\":[" + list + "],\"ValDtls\":{\"AssVal\":" + AssVal + ",\"CgstVal\":" + CgstVal + ",\"SgstVal\":" + SgstVal + ",\"IgstVal\":" + IgstVal + ",\"TotInvVal\":" + TotInvVal + "}}";
                    }
                }
                #endregion

                if (postData != "")
                {
                    //Save JSON IN DATABASE update by pawar 03/01/2025                 
                    Cmd = new SqlCommand("UPDATE tblcreditdebitnotehdr SET E_Invoice_JSON=@E_Invoice_JSON WHERE Id=" + ID + "", con);
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

        string uri = "https://api.mastergst.com/einvoice/authenticate?email=erp%40weblinkservices.net";
        WebResponse response;
        WebRequest request = WebRequest.Create(uri);

        request.Method = "GET";
        request.Headers.Add("username", User_Name);
        request.Headers.Add("password", Password);
        request.Headers.Add("ip_address", "103.174.254.209");
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

            int idd = Convert.ToInt32(GvInvoiceList.DataKeys[e.Row.RowIndex].Values[0]);
            DataTable Dtt = new DataTable();
            SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblcreditdebitnotehdr where Id = '" + idd + "'", con);
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
            SqlCommand cmde_invoice_status = new SqlCommand("select e_invoice_cancel_status from tblCreditDebitNoteHdr where Id='" + ID + "'", con);
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
                SqlCommand cmdGetIRN = new SqlCommand("select IRN from tblCreditDebitNoteHdr where Id='" + ID + "'", con);
                Object F_GetIRN = cmdGetIRN.ExecuteScalar();

                 Cancel_EInvoice(MailID, GST, UserName, Password, F_GetIRN.ToString(), out _Status_CD, out _Messege, out _CancelDate);
               
                if (_Status_CD == "0")
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
                    Cmd = new SqlCommand("UPDATE tblCreditDebitNoteHdr SET e_invoice_cancel_status=@e_invoice_cancel_status,e_invoice_cancel_date=@e_invoice_cancel_date,e_invoice_cancel_by=@e_invoice_cancel_by WHERE Id=" + ID + "", con);
                    Cmd.Parameters.AddWithValue("@e_invoice_cancel_status", 1);
                    Cmd.Parameters.AddWithValue("@e_invoice_cancel_date", _CancelDate);
                    Cmd.Parameters.AddWithValue("@e_invoice_cancel_by", Session["Username"].ToString());
                    Cmd.ExecuteNonQuery();
                    

                    SqlCommand cmdDocNo = new SqlCommand("SELECT BillNumber FROM [tblCreditDebitNoteHdr]  where Id='" + ID + "'", con);
                    Object mxDocNo = cmdDocNo.ExecuteScalar();
                    string DocNo = mxDocNo.ToString();
                    con.Close();

                    SqlCommand cmddelete2 = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + DocNo + "' ", con);
                    con.Open();
                    cmddelete2.ExecuteNonQuery();
                    con.Close();

                    Cls_Main.Conn_Open();
                    SqlCommand Cmd1 = new SqlCommand(@"Insert into tbl_InventoryOutwardManage([OrderNo]
      ,[Particular]
      ,[ComponentName]
      ,[Description]
      ,[HSN]
      ,[Quantity]
      ,[Units]
      ,[Rate]
      ,[CGSTPer]
      ,[CGSTAmt]
      ,[SGSTPer]
      ,[SGSTAmt]
      ,[IGSTPer]
      ,[IGSTAmt]
      ,[Total]
      ,[Discountpercentage]
      ,[DiscountAmount]
      ,[Alltotal]    
      ,[CreatedBy]
      ,[CreatedOn]     
      ,[Batch])
select [OrderNo]
      ,[Particular]
      ,[ComponentName]
      ,[Description]
      ,[HSN]
      ,[Quantity]
      ,[Units]
      ,[Rate]
      ,[CGSTPer]
      ,[CGSTAmt]
      ,[SGSTPer]
      ,[SGSTAmt]
      ,[IGSTPer]
      ,[IGSTAmt]
      ,[Total]
      ,[Discountpercentage]
      ,[DiscountAmount]
      ,[Alltotal]   
      ,[CreatedBy]
      ,GETDATE()     
      ,[Batch] 
from tbl_OutwardEntryComponentsDtls where OrderNo=@ID", Cls_Main.Conn);
                    Cmd1.Parameters.AddWithValue("@ID", DocNo);
                    Cmd1.ExecuteNonQuery();
                    Cls_Main.Conn_Close();

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Cancelled Successfully...!!');window.location='EInv_CrDbNote.aspx';", true);
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
        Response.Redirect("../Gov_Bills/EWayBillList_CrDbNote.aspx");
    }
}