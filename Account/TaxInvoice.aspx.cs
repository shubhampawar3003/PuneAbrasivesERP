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
using System.Net.Mime;
using iTextSharp.tool.xml;

public partial class Account_TaxInvoice : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable dtparticular = new DataTable();
    DataTable dtparticularorder = new DataTable();
    string id;
    string invNo;

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
                FillddlProduct();
                //GenerateCode();
                txtinvoicedate.Text = DateTime.Now.ToString("yyyy-MM-dd");//DateTime.Today.ToString("dd-MM-yyyy");
                manuallytable.Visible = false;
                ViewState["RowNo"] = 0;
                dtparticular.Columns.AddRange(new DataColumn[17] { new DataColumn("id"),
                 new DataColumn("Particular"),new DataColumn("Description"),new DataColumn("Batchno"), new DataColumn("HSN")
                , new DataColumn("Qty"),new DataColumn("UOM"), new DataColumn("Rate"),
                    new DataColumn("TaxableAmt"),new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer"),new DataColumn("SGSTAmt")
                    ,new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("Discount"),new DataColumn("GrandTotal")
            });

                ViewState["ParticularDetails"] = dtparticular;
                if (Request.QueryString["PONO"] != null)
                {
                    id = Decrypt(Request.QueryString["PONO"].ToString());

                    btnSubmit.Text = "Submit";
                    LoadPOData(id);
                    GenerateCode();
                }
                else
                if (Request.QueryString["Id"] != null)
                {
                    id = Decrypt(Request.QueryString["Id"].ToString());

                    btnSubmit.Text = "Update";
                    hidden1.Value = id;

                    LoadData(id);
                }


            }
        }
    }
    private void LoadPOData(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where CP.IsDeleted=0 AND CP.ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            hdnUsername.Value = null;
            hdnUsername.Value= Dt.Rows[0]["Username"].ToString();
            txtbillingcustomer.Text = Dt.Rows[0]["CustomerName"].ToString();
            txtshippingcustomer.Text = Dt.Rows[0]["CustomerName"].ToString();
            txtShippingaddress.Text = Dt.Rows[0]["ShippingAddress"].ToString();
            txtshortBillingaddress.Text = Dt.Rows[0]["ShortBAddress"].ToString();
            txtshortShippingaddress.Text = Dt.Rows[0]["ShortSAddress"].ToString();
            txtbillingaddress.Text = Dt.Rows[0]["BillingAddress"].ToString();
     
            txtbillinglocation.Text = Dt.Rows[0]["BillingLocation"].ToString();
            txtshippinglocation.Text = Dt.Rows[0]["ShippingLocation"].ToString();
            txtbillingPincode.Text = Dt.Rows[0]["BillingPincode"].ToString();
            txtshippingPincode.Text = Dt.Rows[0]["ShippingPincode"].ToString();
            txtbillingstatecode.Text = Dt.Rows[0]["BillingStatecode"].ToString();
            txtshippingstatecode.Text = Dt.Rows[0]["ShippingStatecode"].ToString();
            txtpaymentterm.Text = Dt.Rows[0]["paymentterm"].ToString();
            txtcustomerPoNo.Text = Dt.Rows[0]["SerialNo"].ToString();
            txtagainstno.Text = Dt.Rows[0]["Pono"].ToString();
            txtemail.Text = Dt.Rows[0]["EmailID"].ToString();
            DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PoDate"].ToString());
            txtpodate.Text = ffff1.ToString("yyyy-MM-dd");
            txtContactNo.Text = Dt.Rows[0]["Mobileno"].ToString();
            txtbillingGST.Text = Dt.Rows[0]["GSTNo"].ToString();
            txtTermofdelivery.Text = Dt.Rows[0]["Termofdelivery"].ToString();
            if (Dt.Rows[0]["GSTNo"].ToString() == "URP")
            {
                revGSTNumber.Enabled = false;
            }
            else
            {
                revGSTNumber.Enabled = true;
            }
            if (Dt.Rows[0]["GSTNo"].ToString() == "URP")
            {
                RegularExpressionValidator1.Enabled = false;
            }
            else
            {
                RegularExpressionValidator1.Enabled = true;
            }
            txtshippingGST.Text = Dt.Rows[0]["GSTNo"].ToString();
            txtagainstno.Text = Dt.Rows[0]["Pono"].ToString();

            manuallytable.Visible = true;
            DataTable dtt = new DataTable();
            SqlDataAdapter sad3 = new SqlDataAdapter("select ID AS Id,Productname AS Particular,Description,'' AS Batchno,HSN,Quantity AS Qty, Units AS UOM,Rate,Total AS TaxableAmt,CAST(ISNULL(CGSTPer, 0) AS INT) AS CGSTPer,CGSTAmt,CAST(ISNULL(SGSTPer, 0) AS INT) AS SGSTPer,SGSTAmt,CAST(ISNULL(IGSTPer, 0) AS INT) AS IGSTPer,IGSTAmt, Discountpercentage AS Discount,Alltotal AS GrandTotal  from tbl_CustomerPurchaseOrderDtls where Pono='" + txtagainstno.Text + "'", con);
            sad3.Fill(dtt);
            if (dtt.Rows.Count > 0)
            {
                ViewState["RowNo"] = 0;
                if (dtt.Rows.Count > 0)
                {
                    if (dtparticular.Columns.Count < 1)
                    {
                        Show_Grid();
                    }

                    for (int i = 0; i < dtt.Rows.Count; i++)
                    {
                        dtparticular.Rows.Add(dtt.Rows[i]["Id"].ToString(), dtt.Rows[i]["Particular"].ToString(), dtt.Rows[i]["Description"].ToString(), dtt.Rows[i]["Batchno"].ToString(), dtt.Rows[i]["HSN"].ToString(), dtt.Rows[i]["Qty"].ToString(), dtt.Rows[i]["UOM"].ToString(), dtt.Rows[i]["Rate"].ToString(), dtt.Rows[i]["TaxableAmt"].ToString(), dtt.Rows[i]["CGSTPer"].ToString(), dtt.Rows[i]["CGSTAmt"].ToString(), dtt.Rows[i]["SGSTPer"].ToString(), dtt.Rows[i]["SGSTAmt"].ToString(), dtt.Rows[i]["IGSTPer"].ToString(), dtt.Rows[i]["IGSTAmt"].ToString(), dtt.Rows[i]["Discount"].ToString(), dtt.Rows[i]["GrandTotal"].ToString());
                        // ViewState["RowNo"] = ViewState["RowNo"] + 1;
                    }
                }
                gvinvoiceParticular.DataSource = dtparticular;
                gvinvoiceParticular.DataBind();
            }

            txtbillingaddress.Enabled = false;
            txtbillingcustomer.Enabled = false;
            txtshippingcustomer.Enabled = false;
            txtbillinglocation.Enabled = false;
            txtshippinglocation.Enabled = false;
            txtbillingPincode.Enabled = false;
            txtshippingPincode.Enabled = false;
            txtbillingGST.Enabled = false;
            txtshippingGST.Enabled = false;
            txtbillingstatecode.Enabled = false;
            txtshippingstatecode.Enabled = false;
            txtContactNo.Enabled = false;
            txtcustomerPoNo.Enabled = false;
            txtpodate.Enabled = false;
            txtagainstno.Enabled = false;
            txtemail.Enabled = false;
            txtShippingaddress.Enabled = false;

        }
    }
    protected void LoadData(string id)
    {
        DataTable dtload = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceHdr where Id='" + id + "'", con);
        sad.Fill(dtload);
        if (dtload.Rows.Count > 0)
        {
            txtbillingcustomer.Text = dtload.Rows[0]["BillingCustomer"].ToString();

            txtshippingcustomer.Text = dtload.Rows[0]["ShippingCustomer"].ToString();
            txtpaymentterm.Text = dtload.Rows[0]["PaymentTerm"].ToString();
            txtShippingaddress.Text = dtload.Rows[0]["ShippingAddress"].ToString();
            txtshortBillingaddress.Text = dtload.Rows[0]["ShortBAddress"].ToString();
            txtshortShippingaddress.Text = dtload.Rows[0]["ShortSAddress"].ToString();
            txtinvoicedate.Text = dtload.Rows[0]["Invoicedate"].ToString();
            txtinvoiceno.Text = dtload.Rows[0]["InvoiceNo"].ToString();
            txtagainstno.Text = dtload.Rows[0]["AgainstNumber"].ToString();
            txtcustomerPoNo.Text = dtload.Rows[0]["CustomerPONo"].ToString();
            txtpaymentterm.Text = dtload.Rows[0]["PaymentTerm"].ToString();
            txtTermofdelivery.Text = dtload.Rows[0]["Termofdelivery"].ToString();
            ddlInvoiceType.SelectedItem.Text = dtload.Rows[0]["InvoiceType"].ToString();

            txtbillingGST.Text = dtload.Rows[0]["BillingGST"].ToString();
            if (dtload.Rows[0]["BillingGST"].ToString() == "URP")
            {
                revGSTNumber.Enabled = false;
            }
            else
            {
                revGSTNumber.Enabled = true;
            }
            txtshippingGST.Text = dtload.Rows[0]["ShippingGST"].ToString();
            if (dtload.Rows[0]["ShippingGST"].ToString() == "URP")
            {
                RegularExpressionValidator1.Enabled = false;
            }
            else
            {
                RegularExpressionValidator1.Enabled = true;
            }
            txtbillingaddress.Text = dtload.Rows[0]["BillingAddress"].ToString();
            txtbillinglocation.Text = dtload.Rows[0]["BillingLocation"].ToString();
            txtshippinglocation.Text = dtload.Rows[0]["ShippingLocation"].ToString();
            txtbillingPincode.Text = dtload.Rows[0]["BillingPincode"].ToString();
            txtshippingPincode.Text = dtload.Rows[0]["ShippingPincode"].ToString();
            txtbillingstatecode.Text = dtload.Rows[0]["BillingStatecode"].ToString();
            txtshippingstatecode.Text = dtload.Rows[0]["ShippingStatecode"].ToString();

            //DataTable dtorder = new DataTable();
            //SqlDataAdapter sadorder = new SqlDataAdapter("select * from tbl_CustomerPurchaseOrderHdr where  CustomerName='" + txtbillingcustomer.Text + "'", con);
            //sadorder.Fill(dtorder);
            //txtagainstNumber.DataValueField = "Pono";
            //txtagainstNumber.DataTextField = "Pono";
            //txtagainstNumber.DataSource = dtorder;
            //txtagainstNumber.DataBind();

            txtagainstno.Text = dtload.Rows[0]["AgainstNumber"].ToString();
            txtcustomerPoNo.Text = dtload.Rows[0]["CustomerPONo"].ToString();
            //.Text = dt.Rows[0]["PODate"].ToString();
            txttransportMode.Text = dtload.Rows[0]["TransportMode"].ToString();
            if (txttransportMode.Text == "By Road" || txttransportMode.Text == "By Courier")
            {
                txtvehicalNumber.Text = dtload.Rows[0]["VehicalNo"].ToString();
                txtvehicalNumber.Visible = true;
            }
            else if (txttransportMode.Text == "By Hand")
            {
                txtByHand.Text = dtload.Rows[0]["VehicalNo"].ToString();
                txtByHand.Visible = true;
            }
            else if (txttransportMode.Text == "By Air")
            {
                txtByAir.Text = dtload.Rows[0]["VehicalNo"].ToString();
                txtByAir.Visible = true;
            }
            //txtvehicalNumber.Text = dtload.Rows[0]["VehicalNo"].ToString();
            //txtpaymentDuedate.Text = dt.Rows[0]["Paymentdate"].ToString();
            txtremark.Text = dtload.Rows[0]["Remark"].ToString();
            txtebillnumber.Text = dtload.Rows[0]["E_BillNo"].ToString();
            sumofAmount.Text = dtload.Rows[0]["SumOfProductAmt"].ToString();
            txtDescription.Text = dtload.Rows[0]["ChargesDescription"].ToString();
            txthsntcs.Text = dtload.Rows[0]["HSNTcs"].ToString();
            txtrateTcs.Text = dtload.Rows[0]["RateTcs"].ToString();
            txtBasic.Text = dtload.Rows[0]["Basic"].ToString();
            CGSTPertcs.Text = dtload.Rows[0]["CGST"].ToString();
            SGSTPertcs.Text = dtload.Rows[0]["SGST"].ToString();
            IGSTPertcs.Text = dtload.Rows[0]["IGST"].ToString();
            txtCost.Text = dtload.Rows[0]["Cost"].ToString();
            txtTCSPer.Text = dtload.Rows[0]["TCSPercent"].ToString();
            txtTCSAmt.Text = dtload.Rows[0]["TCSAmt"].ToString();
            txtGrandTot.Text = dtload.Rows[0]["GrandTotalFinal"].ToString();
            txtContactNo.Text = dtload.Rows[0]["ContactNo"].ToString();
            txtemail.Text = dtload.Rows[0]["Email"].ToString();
            //txtBatchNo.Text = dtload.Rows[0]["BatchNo"].ToString();
            txtchallanNo.Text = dtload.Rows[0]["ChallanNo"].ToString();
            txtchallanDate.Text = dtload.Rows[0]["ChallanDate"].ToString();

            //string str = dtload.Rows[0]["Invoicedate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time = Convert.ToDateTime(str);
            txtinvoicedate.Text = dtload.Rows[0]["Invoicedate"].ToString();

            //string str1 = dtload.Rows[0]["PODate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time1 = Convert.ToDateTime(str);
            txtpodate.Text = dtload.Rows[0]["PODate"].ToString();

            //string str2 = dtload.Rows[0]["Paymentdate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time2 = Convert.ToDateTime(str);
            //txtpaymentDuedate.Text = time.ToString("dd/MM/yyyy");
            string email = dtload.Rows[0]["IsEmail"].ToString();
            // IsSedndMail.Checked = email == "True" ? true : false;

            //if (txtinvoiceagainst.Text == "Direct")
            //{
            //    txtagainstNumber.Enabled = false;
            //}
            ///BindDiscription

            manuallytable.Visible = true;
            DataTable dtt = new DataTable();
            SqlDataAdapter sad3 = new SqlDataAdapter("select Id,Particular,Description,Batchno,HSN,Qty,UOM,Rate,TaxableAmt,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Discount,GrandTotal from tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
            sad3.Fill(dtt);
            if (dtt.Rows.Count > 0)
            {
                ViewState["RowNo"] = 0;
                if (dtt.Rows.Count > 0)
                {
                    if (dtparticular.Columns.Count < 1)
                    {
                        Show_Grid();
                    }

                    for (int i = 0; i < dtt.Rows.Count; i++)
                    {
                        dtparticular.Rows.Add(dtt.Rows[i]["Id"].ToString(), dtt.Rows[i]["Particular"].ToString(), dtt.Rows[i]["Description"].ToString(), dtt.Rows[i]["Batchno"].ToString(), dtt.Rows[i]["HSN"].ToString(), dtt.Rows[i]["Qty"].ToString(), dtt.Rows[i]["UOM"].ToString(), dtt.Rows[i]["Rate"].ToString(), dtt.Rows[i]["TaxableAmt"].ToString(), dtt.Rows[i]["CGSTPer"].ToString(), dtt.Rows[i]["CGSTAmt"].ToString(), dtt.Rows[i]["SGSTPer"].ToString(), dtt.Rows[i]["SGSTAmt"].ToString(), dtt.Rows[i]["IGSTPer"].ToString(), dtt.Rows[i]["IGSTAmt"].ToString(), dtt.Rows[i]["Discount"].ToString(), dtt.Rows[i]["GrandTotal"].ToString());
                        // ViewState["RowNo"] = ViewState["RowNo"] + 1;
                    }
                }
                gvinvoiceParticular.DataSource = dtparticular;
                gvinvoiceParticular.DataBind();
            }

        }
    }
    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT[ProductName] FROM[tbl_ProductMaster] WHERE status=1 AND isdeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtParticulars.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            txtParticulars.DataTextField = "ProductName";
            txtParticulars.DataBind();
            txtParticulars.Items.Insert(0, "-- Select Product --");
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

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCustomerList(string prefixText, int count)
    {
        return AutoFillCustomerName(prefixText);
    }

    public static List<string> AutoFillCustomerName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Companyname] from [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Companyname"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtbillingcustomer_TextChanged(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tbl_CompanyMaster where  Companyname='" + txtbillingcustomer.Text + "'", con);
            sad.Fill(dt);
            if (dt.Rows.Count > 0)
            {

                txtbillingaddress.Text = dt.Rows[0]["Billingaddress"].ToString();
                txtShippingaddress.Text = dt.Rows[0]["Shippingaddress"].ToString();
                txtbillinglocation.Text = dt.Rows[0]["Billinglocation"].ToString();
                txtshippinglocation.Text = dt.Rows[0]["Shippinglocation"].ToString();
                txtbillingPincode.Text = dt.Rows[0]["Billingpincode"].ToString().Replace(" ", "");
                txtshippingPincode.Text = dt.Rows[0]["Shippingpincode"].ToString().Replace(" ", "");
                txtbillingstatecode.Text = dt.Rows[0]["Billing_statecode"].ToString().Replace(" ", "");
                txtshippingstatecode.Text = dt.Rows[0]["Shipping_statecode"].ToString().Replace(" ", "");
                txtshippingcustomer.Text = dt.Rows[0]["Companyname"].ToString();
                txtbillingGST.Text = dt.Rows[0]["GSTno"].ToString().Replace(" ", "");
                txtshippingGST.Text = dt.Rows[0]["GSTno"].ToString().Replace(" ", "");
                if (dt.Rows[0]["GSTno"].ToString() == "URP")
                {
                    revGSTNumber.Enabled = false;
                    RegularExpressionValidator1.Enabled = false;
                }
                else if (dt.Rows[0]["GSTno"].ToString() != "URP")
                {
                    revGSTNumber.Enabled = true;
                    RegularExpressionValidator1.Enabled = true;
                }

                txtContactNo.Text = dt.Rows[0]["ContactNo"].ToString();
                txtemail.Text = dt.Rows[0]["PrimaryEmailID"].ToString();


                manuallytable.Visible = false;

            }

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]

    public static List<string> GetItemList(string prefixText, int count)
    {
        return AutoFilItem(prefixText);
    }

    public static List<string> AutoFilItem(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster] where " + "ProductName like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> PartNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        PartNames.Add(sdr["ProductName"].ToString());
                    }
                }
                con.Close();
                return PartNames;
            }
        }
    }

    protected void txtParticulars_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ProductMaster] WHERE ProductName='" + txtParticulars.SelectedItem.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            txtdiscription.Text = Dt.Rows[0]["Description"].ToString();
            txtHSN.Text = Dt.Rows[0]["HSN"].ToString();
            txtRate.Text = Dt.Rows[0]["Price"].ToString();
            txtuom.Text = Dt.Rows[0]["Unit"].ToString();

            string gstNumber = txtbillingGST.Text.Trim();
            string stateCode = gstNumber.Substring(0, 2);
            if (stateCode == "27")
            {
                CGSTPer.Text = "9";
                SGSTPer.Text = "9";
            }
            else
            {
                IGSTPer.Text = "18";
            }
            CGSTAmt.Text = "0.00";
            SGSTAmt.Text = "0.00";
            IGSTAmt.Text = "0.00";
            txtdiscount.Text = "0.00";
            txtGrandtotal.Text = "0.00";

        }
    }
    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        //CGSTPer.Text = "0";
        //SGSTPer.Text = "0";
        //txtdiscount.Text = "0";
        //IGSTPer.Text = "0";
        GST_Calculation();
    }

    private void GST_Calculation()
    {
        var rate = txtRate.Text == "" ? "0" : txtRate.Text;

        var totalamt = Convert.ToDouble(txtQty.Text.Trim()) * Convert.ToDouble(rate);

        Double AmtWithDiscount;
        if (txtdiscount.Text != "")
        {
            var disc = totalamt * (Convert.ToDouble(txtdiscount.Text.Trim())) / 100;

            totalamt = totalamt - disc;
        }
        else
        {
            totalamt = totalamt + 0;
        }


        var CGSTamt = totalamt * (Convert.ToDouble(CGSTPer.Text.Trim())) / 100;
        var SGSTamt = totalamt * (Convert.ToDouble(SGSTPer.Text.Trim())) / 100;
        var IGSTamt = totalamt * (Convert.ToDouble(IGSTPer.Text.Trim())) / 100;

        double GSTtotal = 0;
        //if (IGSTPer.Text == "0")
        //{
        //    CGSTAmt.Text = CGSTamt.ToString();
        //    SGSTAmt.Text = SGSTamt.ToString();
        //    GSTtotal = SGSTamt + CGSTamt;
        //}
        //else
        //{
        //    IGSTAmt.Text = IGSTamt.ToString() == "" ? "0" : IGSTamt.ToString();
        //    GSTtotal = IGSTamt;
        //}

        if (IGSTPer.Text != "0")
        {
            CGSTAmt.Text = "0";
            SGSTAmt.Text = "0";
            IGSTAmt.Text = IGSTamt.ToString() == "" ? "0" : IGSTamt.ToString();
            GSTtotal = IGSTamt;
        }
        else
        {
            IGSTAmt.Text = "0";
            CGSTAmt.Text = CGSTamt.ToString();
            SGSTAmt.Text = SGSTamt.ToString();
            GSTtotal = SGSTamt + CGSTamt;
        }

        txtAmountt.Text = totalamt.ToString();
        var NetAmt = totalamt + GSTtotal;
        //var NetAmt = totalamt;



        txtGrandtotal.Text = NetAmt.ToString("##.00");
    }

    protected void SGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();

        if (SGSTPer.Text == "" || SGSTPer.Text == "0")
        {
            IGSTPer.Enabled = true;
            IGSTPer.Text = "0";
        }
        else
        {
            IGSTPer.Enabled = false;
            IGSTPer.Text = "0";
        }
    }

    private void GridOrderList(GridViewRow row)
    {
        TextBox txt_Qty = (TextBox)row.FindControl("lblQty");
        TextBox txt_price = (TextBox)row.FindControl("lblRate");
        TextBox txt_CGST = (TextBox)row.FindControl("lblCGSTPer");
        TextBox txt_SGST = (TextBox)row.FindControl("lblSGSTPer");
        TextBox txt_IGST = (TextBox)row.FindControl("lblIGSTPer");
        Label txt_amount = (Label)row.FindControl("txtGrandTotal");
        Label taxableamt = (Label)row.FindControl("lblAmount");
        TextBox txt_discount = (TextBox)row.FindControl("txtdiscount");

        var totalamt = Convert.ToDouble(txt_Qty.Text.Trim()) * Convert.ToDouble(txt_price.Text.Trim());
        taxableamt.Text = totalamt.ToString();
        var CGSTamt = totalamt * (Convert.ToDouble(txt_CGST.Text.Trim())) / 100;
        var SGSTamt = totalamt * (Convert.ToDouble(txt_SGST.Text.Trim())) / 100;
        var IGSTamt = totalamt * (Convert.ToDouble(txt_IGST.Text.Trim())) / 100;

        var GSTtotal = SGSTamt + CGSTamt;

        var NetAmt = totalamt + GSTtotal;
        //var NetAmt = totalamt;

        Double AmtWithDiscount;
        if (txt_discount.Text != "" || txt_discount.Text != null)
        {
            var disc = NetAmt * (Convert.ToDouble(txt_discount.Text.Trim())) / 100;

            AmtWithDiscount = NetAmt - disc;
        }
        else
        {
            AmtWithDiscount = 0;
        }

        txt_amount.Text = AmtWithDiscount.ToString("##.00");
    }

    private void GRID_GST_Calculation(GridViewRow row)
    {
        TextBox txt_Qty = (TextBox)row.FindControl("txtQty");
        TextBox txt_price = (TextBox)row.FindControl("txtrate");
        TextBox txt_CGST = (TextBox)row.FindControl("txtCGSTPer");
        Label lblCgstAmt = (Label)row.FindControl("lblCgstAmt");
        TextBox txt_SGST = (TextBox)row.FindControl("txtSGSTPer");
        Label lblSgstAmt = (Label)row.FindControl("lblSgstAmt");
        TextBox txt_IGST = (TextBox)row.FindControl("txtIGSTPer");
        Label lblIGSTAmt = (Label)row.FindControl("lblIGSTAmt");
        Label txt_amount = (Label)row.FindControl("txtGrandTotal");
        Label taxableamt = (Label)row.FindControl("lblAmount");
        TextBox txt_discount = (TextBox)row.FindControl("txtdiscountedit");

        var totalamt = Convert.ToDouble(txt_Qty.Text.Trim()) * Convert.ToDouble(txt_price.Text.Trim());
        taxableamt.Text = totalamt.ToString();

        Double AmtWithDiscount;
        if (txt_discount.Text != "" || txt_discount.Text != null)
        {
            var disc = totalamt * (Convert.ToDouble(txt_discount.Text.Trim())) / 100;
            taxableamt.Text = (Convert.ToDecimal(totalamt) - Convert.ToDecimal(disc)).ToString();
        }
        else
        {
            AmtWithDiscount = 0;
        }

        var CGSTamt = Convert.ToDouble(taxableamt.Text) * (Convert.ToDouble(txt_CGST.Text.Trim())) / 100;
        var SGSTamt = Convert.ToDouble(taxableamt.Text) * (Convert.ToDouble(txt_SGST.Text.Trim())) / 100;
        var IGSTamt = Convert.ToDouble(taxableamt.Text) * (Convert.ToDouble(txt_IGST.Text.Trim())) / 100;
        var GSTtotal = SGSTamt + CGSTamt + IGSTamt;

        lblCgstAmt.Text = CGSTamt.ToString();
        lblSgstAmt.Text = SGSTamt.ToString();
        lblIGSTAmt.Text = IGSTamt.ToString();
        var NetAmt = Convert.ToDouble(taxableamt.Text) + GSTtotal;

        txt_amount.Text = NetAmt.ToString("##.00");

        hdnGrandtotal.Value = GrandTotalamt11.ToString();
        sumofAmount.Text = Totalamt111.ToString();
        var total = Convert.ToDouble(GrandTotalamt11) + Convert.ToDouble(txtCost.Text) + Convert.ToDouble(txtTCSAmt.Text);
        txtGrandTot.Text = total.ToString("#0.00");
    }

    protected void btnAddMore_Click(object sender, EventArgs e)
    {
        if (txtQty.Text == "" || txtParticulars.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill All Required fields !!!');", true);
        }
        else
        {
            Show_Grid();
        }
    }

    private void Show_Grid()
    {
        ViewState["RowNo"] = 0;
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
        DataTable dt = (DataTable)ViewState["ParticularDetails"];

        dt.Rows.Add(ViewState["RowNo"], txtParticulars.SelectedItem.Text, txtdiscription.Text, txtBatchno.Text, txtHSN.Text, txtQty.Text, txtuom.Text, txtRate.Text, txtAmountt.Text, CGSTPer.Text, CGSTAmt.Text, SGSTPer.Text, SGSTAmt.Text, IGSTPer.Text, IGSTAmt.Text, txtdiscount.Text, txtGrandtotal.Text);
        ViewState["ParticularDetails"] = dt;

        gvinvoiceParticular.DataSource = (DataTable)ViewState["ParticularDetails"];
        gvinvoiceParticular.DataBind();

        //txtParticulars.SelectedItem.Text = string.Empty;
        txtdiscription.Text = string.Empty;
        txtQty.Text = string.Empty;
        txtBatchno.Text = string.Empty;
        txtRate.Text = string.Empty;
        txtAmountt.Text = string.Empty;
        CGSTPer.Text = string.Empty;
        SGSTPer.Text = string.Empty;
        //CGSTAmt.Text = string.Empty;
        txtdiscount.Text = "0";
        //SGSTPer.Text = "9";
        SGSTAmt.Text = "0";
        // CGSTPer.Text = "9";
        CGSTAmt.Text = "0";
        IGSTPer.Text = "0";
        IGSTAmt.Text = "0";
        txtGrandtotal.Text = string.Empty;
        //txtuom.Text = string.Empty;

    }

    protected void gvinvoiceParticular_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvinvoiceParticular.EditIndex = e.NewEditIndex;
        gvinvoiceParticular.DataSource = (DataTable)ViewState["ParticularDetails"];
        gvinvoiceParticular.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnUpdate_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string Discription = ((TextBox)row.FindControl("txtDescription")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string UOM = ((Label)row.FindControl("txtUOM")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((TextBox)row.FindControl("txtrate")).Text;
        string Batchno = ((TextBox)row.FindControl("txtBatchno")).Text;

        string Amount = ((Label)row.FindControl("lblAmount")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string CGSAmt = ((Label)row.FindControl("lblCgstAmt")).Text;
        string SGSAmt = ((Label)row.FindControl("lblSgstAmt")).Text;
        string IGSTAmt = ((Label)row.FindControl("lblIGSTAmt")).Text;
        string Discount = ((TextBox)row.FindControl("txtdiscountedit")).Text;
        string grandtotal = ((Label)row.FindControl("txtGrandTotal")).Text;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;

        Dt.Rows[row.RowIndex]["Particular"] = Particulars;
        Dt.Rows[row.RowIndex]["Description"] = Discription;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Qty"] = Qty;
        Dt.Rows[row.RowIndex]["UOM"] = UOM;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Batchno"] = Batchno;
        Dt.Rows[row.RowIndex]["TaxableAmt"] = Amount;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["Discount"] = Discount;
        Dt.Rows[row.RowIndex]["GrandTotal"] = grandtotal;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSAmt;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSAmt;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;

        Dt.AcceptChanges();

        ViewState["ParticularDetails"] = Dt;
        gvinvoiceParticular.EditIndex = -1;

        gvinvoiceParticular.DataSource = (DataTable)ViewState["ParticularDetails"];
        gvinvoiceParticular.DataBind();

        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkCancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;
        gvinvoiceParticular.EditIndex = -1;

        ViewState["ParticularDetails"] = Dt;
        gvinvoiceParticular.EditIndex = -1;

        gvinvoiceParticular.DataSource = (DataTable)ViewState["ParticularDetails"];
        gvinvoiceParticular.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    Double Totalamt = 0;
    Double GrandTotalamt = 0;
    protected void gvinvoiceParticular_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Totalamt += Convert.ToDouble((e.Row.FindControl("lblAmount") as Label).Text);
            GrandTotalamt += Convert.ToDouble((e.Row.FindControl("txtGrandTotal") as Label).Text);
            hdnGrandtotal.Value = GrandTotalamt.ToString();
            sumofAmount.Text = Totalamt.ToString();

            var Total = Convert.ToDouble(txtCost.Text) + GrandTotalamt + Convert.ToDouble(txtTCSAmt.Text);
            txtGrandTot.Text = Total.ToString("##.00");
        }

    }

    protected void txtCGSTPer_TextChanged1(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
        TextBox txt_CGST = (TextBox)row.FindControl("txtCGSTPer");
        Label lbl_CGSTAmt = (Label)row.FindControl("lblCgstAmt");
        Label Amount = (Label)row.FindControl("lblAmount");
        TextBox txt_IGST = (TextBox)row.FindControl("txtIGSTPer");
        if (txt_CGST.Text == "" || txt_CGST.Text == "0")
        {
            txt_IGST.Enabled = true;
            txt_IGST.Text = "0";
        }
        else
        {
            Double CGStAmt = Convert.ToDouble(Amount.Text) * Convert.ToDouble(txt_CGST.Text) / 100;
            lbl_CGSTAmt.Text = CGStAmt.ToString();
            txt_IGST.Enabled = false;
            txt_IGST.Text = "0";
        }

    }

    protected void CGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();

        if (CGSTPer.Text == "" || CGSTPer.Text == "0")
        {

            IGSTPer.Enabled = true;
            IGSTPer.Text = "0";
        }
        else
        {
            Double CgstAmt = Convert.ToDouble(txtAmountt.Text) * Convert.ToDouble(CGSTPer.Text) / 100;
            CGSTAmt.Text = CgstAmt.ToString();
            IGSTPer.Enabled = false;
            IGSTPer.Text = "0";
        }
    }

    protected void SGSTPer_TextChanged1(object sender, EventArgs e)
    {
        GST_Calculation();

        if (SGSTPer.Text == "" || SGSTPer.Text == "0")
        {
            IGSTPer.Enabled = true;
            IGSTPer.Text = "0";
        }
        else
        {
            Double SgstAmt = Convert.ToDouble(txtAmountt.Text) * Convert.ToDouble(SGSTPer.Text) / 100;
            SGSTAmt.Text = SgstAmt.ToString();
            IGSTPer.Enabled = false;
            IGSTPer.Text = "0";
        }
    }

    protected void IGSTPer_TextChanged1(object sender, EventArgs e)
    {
        GST_Calculation();

        if (IGSTPer.Text == "" || IGSTPer.Text == "0")
        {
            // SGSTPer.Enabled = true;
            //CGSTPer.Enabled = true;
            SGSTPer.Text = "0";
            CGSTPer.Text = "0";
        }
        else
        {
            Double IgstAmt = Convert.ToDouble(txtAmountt.Text) * Convert.ToDouble(IGSTPer.Text) / 100;
            IGSTAmt.Text = IgstAmt.ToString();
            //SGSTPer.Enabled = false;
            // CGSTPer.Enabled = false;
            SGSTPer.Text = "0";
            CGSTPer.Text = "0";
        }
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtQty_TextChanged2(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtrate_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;

        GRID_GST_Calculation(row);
        TextBox txt_SGST = (TextBox)row.FindControl("txtSGSTPer");
        TextBox txt_IGST = (TextBox)row.FindControl("txtIGSTPer");
        Label lbl_SGSTAmt = (Label)row.FindControl("lblSgstAmt");
        Label Amount = (Label)row.FindControl("lblAmount");

        if (txt_SGST.Text == "" || txt_SGST.Text == "0")
        {
            txt_IGST.Enabled = true;
            txt_IGST.Text = "0";
        }
        else
        {
            Double SGSTAmt = Convert.ToDouble(Amount.Text) * Convert.ToDouble(txt_SGST.Text) / 100;
            lbl_SGSTAmt.Text = SGSTAmt.ToString();
            txt_IGST.Enabled = false;
            txt_IGST.Text = "0";
        }
    }

    protected void txtIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
        TextBox txt_CGST = (TextBox)row.FindControl("txtCGSTPer");
        TextBox txt_SGST = (TextBox)row.FindControl("txtSGSTPer");

        TextBox txt_IGST = (TextBox)row.FindControl("txtIGSTPer");
        Label lbl_SGSTAmt = (Label)row.FindControl("lblSgstAmt");
        Label Amount = (Label)row.FindControl("lblAmount");
        Label lblIGSTAmt = (Label)row.FindControl("lblIGSTAmt");

        if (txt_IGST.Text == "" || txt_IGST.Text == "0")
        {
            txt_CGST.Enabled = true;
            txt_SGST.Enabled = true;
            //txt_IGST.Enabled = true;
            txt_SGST.Text = "0";
            txt_CGST.Text = "0";
        }
        else
        {

            Double IGSTAmt = Convert.ToDouble(Amount.Text) * Convert.ToDouble(txt_IGST.Text) / 100;
            lblIGSTAmt.Text = IGSTAmt.ToString();
            txt_CGST.Enabled = false;
            txt_SGST.Enabled = false;
            txt_SGST.Text = "0";
            txt_CGST.Text = "0";
        }

    }

    protected void txtdiscountedit_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable dt = ViewState["ParticularDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["ParticularDetails"] = dt;
        gvinvoiceParticular.DataSource = (DataTable)ViewState["ParticularDetails"];
        gvinvoiceParticular.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void txtrateTcs_TextChanged(object sender, EventArgs e)
    {
        string Amt = sumofAmount.Text;
        string Rate = txtrateTcs.Text;
        if (Rate == "0")
        {
            txtBasic.Text = "0";
            txtCost.Text = "0";
            CGSTPertcs.Text = "0";
            SGSTPertcs.Text = "0";
            IGSTPertcs.Text = "0";
        }
        else
        {
            var Basic = Convert.ToDouble(Amt) * Convert.ToDouble(Rate) / 100;
            txtBasic.Text = Basic.ToString("##.00");

            var grandtot = Convert.ToDouble(Basic) + Convert.ToDouble(hdnGrandtotal.Value);
            txtGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void CGSTPertcs_TextChanged(object sender, EventArgs e)
    {
        GstCalculationTcs();
    }

    protected void GstCalculationTcs()
    {
        string Basic = txtBasic.Text;
        string CGST = CGSTPertcs.Text;
        string SGST = SGSTPertcs.Text;
        if (CGST == "0" || SGST == "0")
        {
            if (CGST == "0" && SGST == "0" && IGSTPertcs.Text == "0")
            {
                IGSTPertcs.Enabled = true;
                CGSTPertcs.Enabled = true;
                SGSTPertcs.Enabled = true;
                txtCost.Text = Basic.ToString();
            }
            else
            {
                if (IGSTPertcs.Text == "0")
                {
                    IGSTPertcs.Enabled = false;
                    CGSTPertcs.Enabled = true;
                    SGSTPertcs.Enabled = true;
                    var CGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(CGST) / 100;
                    var SGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(SGST) / 100;
                    var GSTTaxTotal = Convert.ToDouble(Basic) + CGSTAmt + SGSTAmt;
                    txtCost.Text = GSTTaxTotal.ToString("##.00");

                    var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value);
                    txtGrandTot.Text = grandtot.ToString("##.00");
                }
                else
                {
                    IGSTPertcs.Enabled = true;
                    CGSTPertcs.Enabled = false;
                    SGSTPertcs.Enabled = false;
                    var IGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(IGSTPertcs.Text) / 100;
                    var GSTTaxTotal = Convert.ToDouble(Basic) + IGSTAmt;
                    txtCost.Text = GSTTaxTotal.ToString("##.00");

                    var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCSAmt.Text);
                    txtGrandTot.Text = grandtot.ToString("##.00");
                }
            }
        }
        else
        {
            IGSTPertcs.Enabled = false;
            CGSTPertcs.Enabled = true;
            SGSTPertcs.Enabled = true;
            var CGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(CGST) / 100;
            var SGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(SGST) / 100;

            var GSTTaxTotal = Convert.ToDouble(Basic) + CGSTAmt + SGSTAmt;
            txtCost.Text = GSTTaxTotal.ToString("##.00");

            var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value);
            txtGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void SGSTPertcs_TextChanged(object sender, EventArgs e)
    {
        GstCalculationTcs();
        btnSubmit.Enabled = true;
    }

    protected void IGSTPertcs_TextChanged(object sender, EventArgs e)
    {
        GstCalculationTcs();
        btnSubmit.Enabled = true;
    }

    protected void txtTCSPer_TextChanged(object sender, EventArgs e)
    {

    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SaveRecord();
    }

    public string GenerateCode()
    {

        string FinYear = null;
        string FinFullYear = null;
        if (DateTime.Today.Month > 3)
        {
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
            FinFullYear = DateTime.Today.AddYears(1).ToString("yy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();

            var finfYear = DateTime.Today.AddYears(1).ToString("yy");
            FinFullYear = (Convert.ToInt32(finfYear) - 1).ToString();
        }
        string previousyear = (Convert.ToDecimal(FinFullYear) - 1).ToString();
        string strInvoiceNumber = "";
        string fY = previousyear.ToString() + "-" + FinYear;
        string strSelect = @"select ISNULL(MAX(InvoiceNo), '450') AS maxno from tblTaxInvoiceHdr where InvoiceNo like '%" + fY + "%'";
        // string strSelect = @"SELECT TOP 1 MAX(ID) FROM tblTaxInvoiceHdr where InvoiceNo like '%" + fY + "%' ";

        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = strSelect;
        con.Open();
        string result;

        result = cmd.ExecuteScalar().ToString();


        con.Close();
        if (result != "")
        {
            int numbervalue = Convert.ToInt32(result.Substring(result.LastIndexOf("/") + 1));
            numbervalue = numbervalue + 1;
            strInvoiceNumber = "PAPL/" + previousyear.ToString() + "-" + FinYear + "/" + numbervalue;
        }
        else
        {
            strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + "01";
        }
        txtinvoiceno.Text = strInvoiceNumber;
        return strInvoiceNumber;
    }

    protected void SaveRecord()
    {
        try
        {
            int id, idupdate;
            string invoiceno = "";
            string Exinvoiceno = "";
            if (ddlInvoiceType.Text == "Regular")
            {
                invoiceno = GenerateCode();
            }
            else
            {
                Exinvoiceno = GenerateExportInvoiceCode();
            }
            bool flgs = false;
            hiddeninvoiceno.Value = invoiceno;
            if (btnSubmit.Text == "Submit")
            {
                if (gvinvoiceParticular.Rows.Count > 0)
                {
                    flgs = true;

                }
                else
                {
                    flgs = false;
                }

                if (flgs == true)
                {
                    if (!string.IsNullOrEmpty(txtbillingcustomer.Text))
                    {
                        SqlCommand cmd = new SqlCommand("SP_TaxInvoiceHdrs", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (ddlInvoiceType.Text == "Regular")
                        {
                            cmd.Parameters.AddWithValue("@InvoiceNo", invoiceno);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@InvoiceNo", Exinvoiceno);
                        }

                        cmd.Parameters.AddWithValue("@BillingCustomer", txtbillingcustomer.Text);
                        cmd.Parameters.AddWithValue("@ShippingCustomer", txtshippingcustomer.Text);
                        cmd.Parameters.AddWithValue("@ShortBAddress", txtshortBillingaddress.Text);
                        cmd.Parameters.AddWithValue("@ShortSAddress", txtshortShippingaddress.Text);
                        cmd.Parameters.AddWithValue("@ShippingAddress", txtShippingaddress.Text);
                        cmd.Parameters.AddWithValue("@InvoiceType", ddlInvoiceType.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                        cmd.Parameters.AddWithValue("@Email", txtemail.Text);
                        cmd.Parameters.AddWithValue("@ChallanNo", txtchallanNo.Text);
                        cmd.Parameters.AddWithValue("@PaymentTerm", txtpaymentterm.Text);
                        cmd.Parameters.AddWithValue("@Termofdelivery", txtTermofdelivery.Text);
                        //cmd.Parameters.AddWithValue("@ChallanDate", txtchallanDate.Text);
                        cmd.Parameters.AddWithValue("@Invoicedate", txtinvoicedate.Text.Trim());
                        cmd.Parameters.AddWithValue("@PODate", txtpodate.Text.Trim());
                        cmd.Parameters.AddWithValue("@ChallanDate", txtchallanDate.Text.Trim());
                        cmd.Parameters.AddWithValue("@InvoiceAgainst", txtinvoicetype.Text);
                        cmd.Parameters.AddWithValue("@AgainstNumber", txtagainstno.Text);
                        cmd.Parameters.AddWithValue("@CustomerPONo", txtcustomerPoNo.Text);
                        cmd.Parameters.AddWithValue("@TransportMode", txttransportMode.Text);
                        cmd.Parameters.AddWithValue("@BatchNo", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Username", hdnUsername.Value);
                        string TransportDeatils = "";
                        if (txttransportMode.Text == "By Road" || txttransportMode.Text == "By Courier")
                        {
                            TransportDeatils = txtvehicalNumber.Text;
                        }
                        else if (txttransportMode.Text == "By Hand")
                        {
                            TransportDeatils = txtByHand.Text;
                        }
                        else if (txttransportMode.Text == "By Air")
                        {
                            TransportDeatils = txtByAir.Text;
                        }
                        else
                        {
                            TransportDeatils = "";
                        }

                        cmd.Parameters.AddWithValue("@VehicalNo", TransportDeatils);
                        //cmd.Parameters.AddWithValue("@Paymentdate", txtpaymentDuedate.Text);
                        //cmd.Parameters.Add(new SqlParameter()
                        //{
                        //    ParameterName = "@Paymentdate",
                        //    DbType = System.Data.DbType.DateTime,
                        //    SqlDbType = System.Data.SqlDbType.DateTime,
                        //    Value = DateTime.Parse(txtpaymentDuedate.Text)
                        //});
                        cmd.Parameters.AddWithValue("@Remark", txtremark.Text);
                        cmd.Parameters.AddWithValue("@E_BillNo", txtebillnumber.Text);
                        cmd.Parameters.AddWithValue("@SumOfProductAmt", sumofAmount.Text);
                        cmd.Parameters.AddWithValue("@ChargesDescription", txtDescription.Text);
                        cmd.Parameters.AddWithValue("@HSN", txthsntcs.Text);
                        cmd.Parameters.AddWithValue("@Rate", txtrateTcs.Text);
                        cmd.Parameters.AddWithValue("@Basic", txtBasic.Text);
                        cmd.Parameters.AddWithValue("@CGST", CGSTPertcs.Text);
                        cmd.Parameters.AddWithValue("@SGST", SGSTPertcs.Text);
                        cmd.Parameters.AddWithValue("@IGST", IGSTPertcs.Text);
                        cmd.Parameters.AddWithValue("@Cost", txtCost.Text);
                        //NewDetails For E-Invoice
                        cmd.Parameters.AddWithValue("@BillingAddress", txtbillingaddress.Text);
                        cmd.Parameters.AddWithValue("@BillingLocation", txtbillinglocation.Text);
                        cmd.Parameters.AddWithValue("@ShippingLocation", txtshippinglocation.Text);
                        cmd.Parameters.AddWithValue("@BillingGST", txtbillingGST.Text);
                        cmd.Parameters.AddWithValue("@ShippingGST", txtshippingGST.Text);
                        cmd.Parameters.AddWithValue("@BillingPincode", txtbillingPincode.Text);
                        cmd.Parameters.AddWithValue("@ShippingPincode", txtshippingPincode.Text);
                        cmd.Parameters.AddWithValue("@BillingStatecode", txtbillingstatecode.Text);
                        cmd.Parameters.AddWithValue("@ShippingStatecode", txtshippingstatecode.Text);
                        //ExportInvoiceNumber Insert
                        cmd.Parameters.AddWithValue("@FinalBasic", Exinvoiceno);
                        cmd.Parameters.AddWithValue("@TCSPercent", txtTCSPer.Text);
                        cmd.Parameters.AddWithValue("@TCSAmt", txtTCSAmt.Text);
                        cmd.Parameters.AddWithValue("@GrandTotal", txtGrandTot.Text);
                        cmd.Parameters.AddWithValue("@IsEmail", DBNull.Value);
                        cmd.Parameters.Add("@Iddd", SqlDbType.Int).Direction = ParameterDirection.Output;

                        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@isdeleted", '0');
                        cmd.Parameters.AddWithValue("@Action", "Insert");
                        //id8 = Convert.ToInt32(cmd.Parameters["@Iddd"].Value);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        id = Convert.ToInt32(cmd.Parameters["@Iddd"].Value);
                        string idd = id.ToString();
                        foreach (GridViewRow g1 in gvinvoiceParticular.Rows)
                        {
                            string particular = (g1.FindControl("lblParticulars") as Label).Text;
                            string Description = (g1.FindControl("txtDescription") as Label).Text;
                            string HSN = (g1.FindControl("lblHSN") as Label).Text;
                            string QTY = (g1.FindControl("lblQty") as Label).Text;
                            string UOM = (g1.FindControl("txtUOM") as Label).Text;
                            string RATE = (g1.FindControl("lblRate") as Label).Text;
                            string AMOUNT = (g1.FindControl("lblAmount") as Label).Text;
                            string CGST = (g1.FindControl("lblCGSTPer") as Label).Text;
                            string SGST = (g1.FindControl("lblSGSTPer") as Label).Text;
                            string IGST = (g1.FindControl("lblIGSTPer") as Label).Text;
                            string DISCOUNT = (g1.FindControl("txtdiscount") as Label).Text;
                            string Grandtotal = (g1.FindControl("txtGrandTotal") as Label).Text;
                            string OAId = (g1.FindControl("lblid") as Label).Text;
                            string CGSTAmt = (g1.FindControl("lblCgstAmt") as Label).Text;
                            string SGSTAmt = (g1.FindControl("lblSgstAmt") as Label).Text;
                            string IGSTAmt = (g1.FindControl("lblIGSTAmt") as Label).Text;
                            string Batchno = (g1.FindControl("lblBatchno") as Label).Text;

                            SqlCommand cmd1 = new SqlCommand("SP_TaxInvoiceDtls", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@HeaderID", id);
                            cmd1.Parameters.AddWithValue("@CustomerName", txtbillingcustomer.Text);
                            cmd1.Parameters.AddWithValue("@Particular", particular);
                            cmd1.Parameters.AddWithValue("@Batchno", Batchno);
                            cmd1.Parameters.AddWithValue("@HSN", HSN);
                            cmd1.Parameters.AddWithValue("@Qty", QTY);
                            cmd1.Parameters.AddWithValue("@UOM", UOM);
                            cmd1.Parameters.AddWithValue("@Rate", RATE);
                            cmd1.Parameters.AddWithValue("@Discount", DISCOUNT);
                            cmd1.Parameters.AddWithValue("@TaxableAmt", AMOUNT);
                            cmd1.Parameters.AddWithValue("@CGSTPer", CGST);
                            cmd1.Parameters.AddWithValue("@SGSTPer", SGST);
                            cmd1.Parameters.AddWithValue("@IGSTPer", IGST);
                            cmd1.Parameters.AddWithValue("@CGSTAmt", CGSTAmt);
                            cmd1.Parameters.AddWithValue("@SGSTAmt", SGSTAmt);
                            cmd1.Parameters.AddWithValue("@IGSTAmt", IGSTAmt);
                            // cmd1.Parameters.Add("@Idddup", SqlDbType.Int).Direction = ParameterDirection.Output;
                            cmd1.Parameters.AddWithValue("@Description", Description);
                            cmd1.Parameters.AddWithValue("@GrandTotal", Grandtotal);
                            cmd1.Parameters.AddWithValue("@OAId", OAId);
                            cmd1.Parameters.AddWithValue("@Action", "Insert");
                            con.Open();
                            cmd1.ExecuteNonQuery();
                            con.Close();
                        }

                        DataTable dt546665 = new DataTable();
                        SqlDataAdapter sadparticular = new SqlDataAdapter("select * from tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
                        sadparticular.Fill(dt546665);
                        if (dt546665.Rows.Count > 0)
                        {

                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('At Least Select One Record.');", true);
                        }
                        // }
                        //  if (IsSedndMail.Checked == true)
                        //  {
                        // Send_Mail(idd);
                        // }

                        //if (Session["isclosed"] != null)
                        //{
                        //}
                        //else
                        //{
                        //    con.Open();
                        //    SqlCommand cmdupOA = new SqlCommand("update OrderAccept set status='close' where pono='" + txtagainstNumber.SelectedItem.Text + "'", con);
                        //    cmdupOA.ExecuteNonQuery();
                        //    con.Close();
                        //}                     
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='TaxInvoiceList.aspx';", true);

                        Session["isclosed"] = null;
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Enter Party Name.');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please add Particulars');", true);
                }
            }
            else if (btnSubmit.Text == "Update")
            {
                SqlCommand cmd = new SqlCommand("SP_TaxInvoiceHdrs", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@InvoiceNo", invNo);
                cmd.Parameters.AddWithValue("@BillingCustomer", txtbillingcustomer.Text);
                cmd.Parameters.AddWithValue("@ShippingCustomer", txtshippingcustomer.Text);
                cmd.Parameters.AddWithValue("@ShortBAddress", txtshortBillingaddress.Text);
                cmd.Parameters.AddWithValue("@ShortSAddress", txtshortShippingaddress.Text);
                cmd.Parameters.AddWithValue("@InvoiceType", ddlInvoiceType.SelectedItem.Text);
                cmd.Parameters.AddWithValue("@ShippingAddress", txtShippingaddress.Text);

                cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text);
                cmd.Parameters.AddWithValue("@Email", txtemail.Text);
                cmd.Parameters.AddWithValue("@ChallanNo", txtchallanNo.Text);
                cmd.Parameters.AddWithValue("@Invoicedate", txtinvoicedate.Text.Trim());
                cmd.Parameters.AddWithValue("@PODate", txtpodate.Text.Trim());
                cmd.Parameters.AddWithValue("@PaymentTerm", txtpaymentterm.Text);
                cmd.Parameters.AddWithValue("@ChallanDate", txtchallanDate.Text.Trim());
                cmd.Parameters.AddWithValue("@InvoiceAgainst", txtinvoicetype.Text);
                cmd.Parameters.AddWithValue("@AgainstNumber", txtagainstno.Text);
                cmd.Parameters.AddWithValue("@CustomerPONo", txtcustomerPoNo.Text);
                cmd.Parameters.AddWithValue("@TransportMode", txttransportMode.Text);
                cmd.Parameters.AddWithValue("@Termofdelivery", txtTermofdelivery.Text);
                cmd.Parameters.AddWithValue("@BatchNo", DBNull.Value);
                string TransportDeatils = "";
                if (txttransportMode.Text == "By Road" || txttransportMode.Text == "By Courier")
                {
                    TransportDeatils = txtvehicalNumber.Text;
                }
                else if (txttransportMode.Text == "By Hand")
                {
                    TransportDeatils = txtByHand.Text;
                }
                else if (txttransportMode.Text == "By Air")
                {
                    TransportDeatils = txtByAir.Text;
                }
                else
                {
                    TransportDeatils = "";
                }
                cmd.Parameters.AddWithValue("@VehicalNo", TransportDeatils);
                //cmd.Parameters.AddWithValue("@Paymentdate", txtpaymentDuedate.Text);
                //cmd.Parameters.Add(new SqlParameter()
                //{
                //    ParameterName = "@Paymentdate",
                //    DbType = System.Data.DbType.DateTime,
                //    SqlDbType = System.Data.SqlDbType.DateTime,
                //    Value = DateTime.Parse(txtpaymentDuedate.Text)
                //});

                cmd.Parameters.AddWithValue("@Remark", txtremark.Text);
                cmd.Parameters.AddWithValue("@Id", hidden1.Value);
                cmd.Parameters.AddWithValue("@E_BillNo", txtebillnumber.Text);
                cmd.Parameters.AddWithValue("@SumOfProductAmt", sumofAmount.Text);
                cmd.Parameters.AddWithValue("@ChargesDescription", txtDescription.Text);
                cmd.Parameters.AddWithValue("@HSN", txthsntcs.Text);
                cmd.Parameters.AddWithValue("@Rate", txtrateTcs.Text);
                cmd.Parameters.AddWithValue("@Basic", txtBasic.Text);
                cmd.Parameters.AddWithValue("@CGST", CGSTPertcs.Text);
                cmd.Parameters.AddWithValue("@SGST", SGSTPertcs.Text);
                cmd.Parameters.AddWithValue("@IGST", IGSTPertcs.Text);
                cmd.Parameters.AddWithValue("@Cost", txtCost.Text);
                //NewDetails For E-Invoice
                cmd.Parameters.AddWithValue("@BillingAddress", txtbillingaddress.Text);
                cmd.Parameters.AddWithValue("@BillingLocation", txtbillinglocation.Text);
                cmd.Parameters.AddWithValue("@ShippingLocation", txtshippinglocation.Text);
                cmd.Parameters.AddWithValue("@BillingGST", txtbillingGST.Text);
                cmd.Parameters.AddWithValue("@ShippingGST", txtshippingGST.Text);
                cmd.Parameters.AddWithValue("@BillingPincode", txtbillingPincode.Text);
                cmd.Parameters.AddWithValue("@ShippingPincode", txtshippingPincode.Text);
                cmd.Parameters.AddWithValue("@BillingStatecode", txtbillingstatecode.Text);
                cmd.Parameters.AddWithValue("@ShippingStatecode", txtshippingstatecode.Text);
                //cmd.Parameters.AddWithValue("@FinalBasic", Exinvoiceno);
                cmd.Parameters.AddWithValue("@TCSPercent", txtTCSPer.Text);
                cmd.Parameters.AddWithValue("@TCSAmt", txtTCSAmt.Text);
                cmd.Parameters.AddWithValue("@GrandTotal", txtGrandTot.Text);
                cmd.Parameters.AddWithValue("@AmountInWords", ConvertToWords(txtGrandTot.Text));
                cmd.Parameters.AddWithValue("@IsEmail", DBNull.Value);
                cmd.Parameters.Add("@Iddd", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Updatedby", Session["Username"].ToString());
                cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@Action", "Update");
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                SqlCommand CmdDelete = new SqlCommand("DELETE FROM tblTaxInvoiceDtls WHERE HeaderID=@HeaderID", con);
                CmdDelete.Parameters.AddWithValue("@HeaderID", hidden1.Value);
                con.Open();
                CmdDelete.ExecuteNonQuery();
                con.Close();

                foreach (GridViewRow g1 in gvinvoiceParticular.Rows)
                {
                    string particular = (g1.FindControl("lblParticulars") as Label).Text;
                    string Description = (g1.FindControl("txtDescription") as Label).Text;
                    string HSN = (g1.FindControl("lblHSN") as Label).Text;
                    string QTY = (g1.FindControl("lblQty") as Label).Text;
                    string UOM = (g1.FindControl("txtUOM") as Label).Text;
                    string RATE = (g1.FindControl("lblRate") as Label).Text;
                    string AMOUNT = (g1.FindControl("lblAmount") as Label).Text;
                    string CGST = (g1.FindControl("lblCGSTPer") as Label).Text;
                    string SGST = (g1.FindControl("lblSGSTPer") as Label).Text;
                    string IGST = (g1.FindControl("lblIGSTPer") as Label).Text;
                    string DISCOUNT = (g1.FindControl("txtdiscount") as Label).Text;
                    string Grandtotal = (g1.FindControl("txtGrandTotal") as Label).Text;
                    string CGSTAmt = (g1.FindControl("lblCgstAmt") as Label).Text;
                    string SGSTAmt = (g1.FindControl("lblSgstAmt") as Label).Text;
                    string IGSTAmt = (g1.FindControl("lblIGSTAmt") as Label).Text;
                    //string Batchno = (g1.FindControl("lblBatchno") as Label).Text;

                    SqlCommand cmd1 = new SqlCommand("SP_TaxInvoiceDtls", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@HeaderID", hidden1.Value);
                    cmd1.Parameters.AddWithValue("@CustomerName", txtbillingcustomer.Text);
                    cmd1.Parameters.AddWithValue("@Particular", particular);
                    cmd1.Parameters.AddWithValue("@Batchno", DBNull.Value);
                    cmd1.Parameters.AddWithValue("@HSN", HSN);
                    cmd1.Parameters.AddWithValue("@Qty", QTY);
                    cmd1.Parameters.AddWithValue("@UOM", UOM);
                    cmd1.Parameters.AddWithValue("@Rate", RATE);
                    cmd1.Parameters.AddWithValue("@Discount", DISCOUNT);
                    cmd1.Parameters.AddWithValue("@TaxableAmt", AMOUNT);
                    cmd1.Parameters.AddWithValue("@CGSTPer", CGST);
                    cmd1.Parameters.AddWithValue("@SGSTPer", SGST);
                    cmd1.Parameters.AddWithValue("@IGSTPer", IGST);
                    cmd1.Parameters.AddWithValue("@Description", Description);
                    cmd1.Parameters.AddWithValue("@GrandTotal", Grandtotal);
                    cmd1.Parameters.AddWithValue("@CGSTAmt", CGSTAmt);
                    cmd1.Parameters.AddWithValue("@SGSTAmt", SGSTAmt);
                    cmd1.Parameters.AddWithValue("@IGSTAmt", IGSTAmt);
                    // cmd1.Parameters.AddWithValue("@OAId", OAId);
                    cmd1.Parameters.AddWithValue("@Action", "Insert");
                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();
                }


                // if (IsSedndMail.Checked == true)
                // {
                // Send_Mail(hidden1.Value);
                //}

                //if (Session["isclosed"] != null)
                //{

                //}
                //else
                //{
                //    con.Open();
                //    SqlCommand cmdupOA = new SqlCommand("update OrderAccept set status='close' where pono='" + txtagainstNumber.SelectedItem.Text + "'", con);
                //    cmdupOA.ExecuteNonQuery();
                //    con.Close();
                //}

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Updated Sucessfully');window.location.href='ApprovedInvoiceList.aspx';", true);


            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
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

    protected void Send_Mail(string Id)
    {
        string strMessage = "Hello " + txtbillingcustomer.Text.Trim() + "<br/>" +


                        "Greetings From " + "<strong>Excel Encloser<strong>" + "<br/>" +
                        "We sent you  Tax Invoice." + "Tax - " + hiddeninvoiceno.Value.Trim() + "/" + txtinvoicedate.Text.Trim() + ".pdf" + "<br/>" +

                         "We Look Foward to Conducting Future Business with you." + "<br/>" +

                        "Kind Regards," + "<br/>" +
                        "<strong>Excel Encloser<strong>";
        string pdfname = "TaxInv - " + hiddeninvoiceno.Value.Trim() + "/" + txtinvoicedate.Text.Trim() + ".pdf";

        MailMessage message = new MailMessage();
        DataTable dt666 = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from Company where status='0' and cname='" + txtbillingcustomer.Text + "' ", con);
        sad.Fill(dt666);

        message.To.Add(dt666.Rows[0]["email1"].ToString());// Email-ID of Receiver  
        if (btnSubmit.Text == "Submit")
        {
            message.Subject = "Tax Invoice";// Subject of Email  
        }
        else
        {
            message.Subject = "Updated Tax Invoice";// Subject of Email  
        }
        message.Body = strMessage;
        message.From = new System.Net.Mail.MailAddress("enquiry@weblinkservices.net");// Email-ID of Sender  
        message.IsBodyHtml = true;



        //MemoryStream file = new MemoryStream(PDF(Id).ToArray());

        //file.Seek(0, SeekOrigin.Begin);
        //Attachment data = new Attachment(file, pdfname, "application/pdf");
        //ContentDisposition disposition = data.ContentDisposition;
        //disposition.CreationDate = System.DateTime.Now;
        //disposition.ModificationDate = System.DateTime.Now;
        //disposition.DispositionType = DispositionTypeNames.Attachment;
        //message.Attachments.Add(data);//Attach the file  


        //message.Body = txtmessagebody.Text;
        SmtpClient SmtpMail = new SmtpClient();
        SmtpMail.Host = "smtpout.secureserver.net";//name or IP-Address of Host used for SMTP transactions  
        SmtpMail.Port = 587;//Port for sending the mail  
        SmtpMail.Credentials = new System.Net.NetworkCredential("enquiry@weblinkservices.net", "wlspl@123");//username/password of network, if apply  
        SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
        SmtpMail.EnableSsl = true;
        SmtpMail.ServicePoint.MaxIdleTime = 0;
        SmtpMail.ServicePoint.SetTcpKeepAlive(true, 2000, 2000);
        message.BodyEncoding = Encoding.Default;
        message.Priority = MailPriority.High;
        SmtpMail.Send(message); //Smtpclient to send the mail message  

        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };



    }

    Double Totalamt111 = 0, GrandTotalamt11 = 0;
    private void calculationA(GridViewRow row)
    {
        TextBox txt_Qty = (TextBox)row.FindControl("lblQty");
        TextBox txt_price = (TextBox)row.FindControl("lblRate");
        TextBox txt_CGST = (TextBox)row.FindControl("lblCGSTPer");
        TextBox txt_SGST = (TextBox)row.FindControl("lblSGSTPer");
        TextBox txt_IGST = (TextBox)row.FindControl("lblIGSTPer");
        Label lblCGSTAmt = (Label)row.FindControl("lblCGSTAmt");
        Label lblSGSTAmt = (Label)row.FindControl("lblSGSTAmt");
        Label lblIGSTAmt = (Label)row.FindControl("lblIGSTAmt");
        Label txt_amount = (Label)row.FindControl("txtGrandTotal");
        TextBox txt_discount = (TextBox)row.FindControl("txtdiscount");
        Label Amount = (Label)row.FindControl("lblAmount");



        var totalamt = Convert.ToDouble(txt_Qty.Text.Trim()) * Convert.ToDouble(txt_price.Text.Trim());
        Amount.Text = totalamt.ToString("#0.00");

        Double AmtWithDiscount;
        if (txt_discount.Text != "" || txt_discount.Text != null)
        {
            var disc = Convert.ToDouble(Amount.Text) * (Convert.ToDouble(txt_discount.Text.Trim())) / 100;

            AmtWithDiscount = Convert.ToDouble(Amount.Text) - disc;
        }
        else
        {
            AmtWithDiscount = 0;
        }
        Amount.Text = AmtWithDiscount.ToString("#0.00");

        var CGSTamt = Convert.ToDouble(Amount.Text) * (Convert.ToDouble(txt_CGST.Text.Trim())) / 100;
        var SGSTamt = Convert.ToDouble(Amount.Text) * (Convert.ToDouble(txt_SGST.Text.Trim())) / 100;
        var IGSTamt = Convert.ToDouble(Amount.Text) * (Convert.ToDouble(txt_IGST.Text.Trim())) / 100;

        lblCGSTAmt.Text = CGSTamt.ToString("#0.00");
        lblSGSTAmt.Text = SGSTamt.ToString("#0.00");
        lblIGSTAmt.Text = IGSTamt.ToString("#0.00");

        double GSTtotal;
        if (lblIGSTAmt.Text != "0.00")
        {
            GSTtotal = IGSTamt;
        }
        else
        {
            GSTtotal = SGSTamt + CGSTamt;
        }

        var NetAmt = Convert.ToDouble(Amount.Text) + GSTtotal;
        txt_amount.Text = NetAmt.ToString();



        hdnGrandtotal.Value = GrandTotalamt11.ToString();
        sumofAmount.Text = Totalamt111.ToString();
        var total = Convert.ToDouble(GrandTotalamt11) + Convert.ToDouble(txtCost.Text) + Convert.ToDouble(txtTCSAmt.Text);
        txtGrandTot.Text = total.ToString("#0.00");
    }

    protected void lblQty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Label lblAmount1 = (Label)row.FindControl("lblAmount");
        calculationA(row);

        Session["isclosed"] = "open";
    }

    protected void lblRate_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void lblCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        TextBox txt_CGST = (TextBox)row.FindControl("lblCGSTPer");
        TextBox txt_SGST = (TextBox)row.FindControl("lblSGSTPer");
        TextBox txt_IGST = (TextBox)row.FindControl("lblIGSTPer");
        calculationA(row);
        if (txt_CGST.Text == "" || txt_CGST.Text == "0")
        {
            txt_IGST.Enabled = true;
            txt_IGST.Text = "0";
        }
        else
        {
            txt_IGST.Enabled = false;
            txt_IGST.Text = "0";
        }
    }

    protected void lblSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        TextBox txt_CGST = (TextBox)row.FindControl("lblCGSTPer");
        TextBox txt_SGST = (TextBox)row.FindControl("lblSGSTPer");
        TextBox txt_IGST = (TextBox)row.FindControl("lblIGSTPer");
        calculationA(row);
        if (txt_SGST.Text == "" || txt_SGST.Text == "0")
        {
            txt_IGST.Enabled = true;
            txt_IGST.Text = "0";
        }
        else
        {
            txt_IGST.Enabled = false;
            txt_IGST.Text = "0";
        }
    }

    protected void lblIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        TextBox txt_CGST = (TextBox)row.FindControl("lblCGSTPer");
        TextBox txt_SGST = (TextBox)row.FindControl("lblSGSTPer");
        TextBox txt_IGST = (TextBox)row.FindControl("lblIGSTPer");
        calculationA(row);
        if (txt_IGST.Text == "" || txt_IGST.Text == "0")
        {
            txt_SGST.Enabled = true;
            txt_CGST.Enabled = true;
            txt_SGST.Text = "0";
            txt_CGST.Text = "0";
        }
        else
        {
            txt_SGST.Enabled = false;
            txt_CGST.Enabled = false;
            txt_SGST.Text = "0";
            txt_CGST.Text = "0";
        }
    }

    protected void txtdiscount_TextChanged1(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtTCSPer_TextChanged1(object sender, EventArgs e)
    {
        if (txtTCSPer.Text == "0" || txtTCSPer.Text == "")
        {
            //var tot = Convert.ToDouble(sumofAmount.Text) + Convert.ToDouble(txtCost.Text);
            //var TcsAmt = Convert.ToDouble(txtTCSPer.Text) * tot / 100;
            //txtTCSAmt.Text = TcsAmt.ToString("##.00");

            //var grandtot = Convert.ToDouble(txtTCSAmt.Text) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtCost.Text);
            //txtGrandTot.Text = grandtot.ToString("##.00");
            //txtTCSAmt.Text = "0";
            //changes for TCS by shubham wankhade
            var grandtotwithoutTcs = Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtCost.Text);
            var tot = Convert.ToDouble(sumofAmount.Text) + Convert.ToDouble(txtBasic.Text);
            var TcsAmt = Convert.ToDouble(txtTCSPer.Text) * grandtotwithoutTcs / 100;
            txtTCSAmt.Text = TcsAmt.ToString("##.00");
            var grandtotWithTCs = Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtCost.Text) + Convert.ToDouble(TcsAmt);
            txtGrandTot.Text = grandtotWithTCs.ToString("##.00");
            txtTCSAmt.Text = "0";
        }
        else
        {
            //var tot = Convert.ToDouble(sumofAmount.Text) + Convert.ToDouble(txtCost.Text);
            // var TcsAmt = Convert.ToDouble(txtTCSPer.Text) * tot / 100;
            //txtTCSAmt.Text = TcsAmt.ToString("##.00");

            //var grandtot = Convert.ToDouble(txtTCSAmt.Text) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtCost.Text);
            // txtGrandTot.Text = grandtot.ToString("##.00");


            //changes for TCS by shubham wankhade
            var grandtotwithoutTcs = Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtCost.Text);
            var tot = Convert.ToDouble(sumofAmount.Text) + Convert.ToDouble(txtBasic.Text);
            var TcsAmt = Convert.ToDouble(txtTCSPer.Text) * grandtotwithoutTcs / 100;
            txtTCSAmt.Text = TcsAmt.ToString("##.00");
            var grandtotWithTCs = Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtCost.Text) + Convert.ToDouble(TcsAmt);
            txtGrandTot.Text = grandtotWithTCs.ToString("##.00");
        }
    }

    protected void btnSubmit_Click1(object sender, EventArgs e)
    {
        SaveRecord();
    }

    protected void txtBasic_TextChanged(object sender, EventArgs e)
    {
        //    if (txtinvoiceagainst.Text == "Direct")
        //    {
        //        if (txtBasic.Text != "0")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Enter GST');", true);
        //            btnSubmit.Enabled = false;
        //        }
        //    }

        string Amt = sumofAmount.Text;
        string Basic = txtBasic.Text;
        if (txtbillingstatecode.Text == "27")
        {
            CGSTPertcs.Text = "9";
            SGSTPertcs.Text = "9";
            IGSTPertcs.Text = "0";
        }
        else
        {
            CGSTPertcs.Text = "0";
            SGSTPertcs.Text = "0";
            IGSTPertcs.Text = "18";
        }
        if (Basic == "0")
        {
            txtBasic.Text = "0";
            txtCost.Text = "0";
            CGSTPertcs.Text = "0";
            SGSTPertcs.Text = "0";
            IGSTPertcs.Text = "0";
        }
        else
        {

            var Per = Convert.ToDouble(Basic) / Convert.ToDouble(Amt) * 100;
            txtrateTcs.Text = Per.ToString("##.00");

            if (IGSTPertcs.Text == "0")
            {
                IGSTPertcs.Enabled = true;
                CGSTPertcs.Enabled = true;
                SGSTPertcs.Enabled = true;
                var CGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(CGSTPertcs.Text) / 100;
                var SGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(SGSTPertcs.Text) / 100;

                var GSTTaxTotal = Convert.ToDouble(Basic) + CGSTAmt + SGSTAmt;
                txtCost.Text = GSTTaxTotal.ToString("##.00");

                var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCSAmt.Text);
                txtGrandTot.Text = grandtot.ToString("##.00");
            }
            else
            {
                IGSTPertcs.Enabled = true;
                CGSTPertcs.Enabled = false;
                SGSTPertcs.Enabled = false;
                var IGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(IGSTPertcs.Text) / 100;
                var GSTTaxTotal = Convert.ToDouble(Basic) + IGSTAmt;
                txtCost.Text = GSTTaxTotal.ToString("##.00");

                var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCSAmt.Text);
                txtGrandTot.Text = grandtot.ToString("##.00");
            }

            //var grandtot = Convert.ToDouble(Basic) + Convert.ToDouble(hdnGrandtotal.Value);
            //txtGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txttransportMode_TextChanged(object sender, EventArgs e)
    {
        if (txttransportMode.Text == "By Road" || txttransportMode.Text == "By Courier")
        {
            txtvehicalNumber.Visible = true;
            txtByAir.Visible = false;
            txtByHand.Visible = false;
        }
        else if (txttransportMode.Text == "By Hand")
        {
            txtvehicalNumber.Visible = false;
            txtByAir.Visible = false;
            txtByHand.Visible = true;
        }
        else if (txttransportMode.Text == "By Air")
        {
            txtvehicalNumber.Visible = false;
            txtByAir.Visible = true;
            txtByHand.Visible = false;
        }
        else
        {
            txtvehicalNumber.Visible = false;
            txtByAir.Visible = false;
            txtByHand.Visible = false;
        }
    }

    protected void txtshippingcustomer_TextChanged(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from tbl_CompanyMaster  where Companyname='" + txtshippingcustomer.Text + "'", con);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtShippingaddress.Text = dt.Rows[0]["Shippingaddress"].ToString();
            txtshippinglocation.Text = dt.Rows[0]["Shippinglocation"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["Shippingpincode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["StateCode"].ToString();
            txtshippingGST.Text = dt.Rows[0]["GSTno"].ToString();
        }
    }
    public string GenerateExportInvoiceCode()
    {
        string FinYear = null;
        string FinFullYear = null;
        if (DateTime.Today.Month > 3)
        {
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
            FinFullYear = DateTime.Today.AddYears(1).ToString("yy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();

            var finfYear = DateTime.Today.AddYears(1).ToString("yy");
            FinFullYear = (Convert.ToInt32(finfYear) - 1).ToString();
        }
        string previousyear = (Convert.ToDecimal(FinFullYear) - 1).ToString();
        string strInvoiceNumber = "";
        string fY = previousyear.ToString() + "-" + FinYear;
        string strSelect = @"select ISNULL(MAX(FinalBasic), '') AS maxno from tblTaxInvoiceHdr where FinalBasic like '%EX%'";
        //string strSelect = @"SELECT TOP 1 FinalBasic FROM tblTaxInvoiceHdr where FinalBasic like '%" + fY + "%' ORDER BY ID DESC";
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = strSelect;
        con.Open();
        string result = cmd.ExecuteScalar().ToString();
        con.Close();
        if (result != "")
        {
            int numbervalue = Convert.ToInt32(result.Substring(result.LastIndexOf("/") + 1));
            if (numbervalue < 9)
            {
                numbervalue = numbervalue + 1;
                strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + numbervalue.ToString("000");
            }
            else if (numbervalue < 99)
            {
                numbervalue = numbervalue + 1;
                strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + "0" + numbervalue.ToString("00");
            }
            else
            {
                numbervalue = numbervalue + 1;
                strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + numbervalue.ToString();
            }

        }
        else
        {
            strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + "001";
        }
        txtinvoiceno.Text = "EX/" + strInvoiceNumber;
        return "EX/" + strInvoiceNumber;
    }

    //Search Part name methods
    //[System.Web.Script.Services.ScriptMethod()]
    //[System.Web.Services.WebMethod]
    //public static List<string> GetPartList(string prefixText, int count)
    //{
    //    return AutoFillPartName(prefixText);
    //}

    //public static List<string> AutoFillPartName(string prefixText)
    //{
    //    using (SqlConnection con = new SqlConnection())
    //    {
    //        con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

    //        using (SqlCommand com = new SqlCommand())
    //        {
    //            com.CommandText = "SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster] where " + "ProductName like '%'+ @Search + '%' and IsDeleted=0";

    //            com.Parameters.AddWithValue("@Search", prefixText);
    //            com.Connection = con;
    //            con.Open();
    //            List<string> PartNames = new List<string>();
    //            using (SqlDataReader sdr = com.ExecuteReader())
    //            {
    //                while (sdr.Read())
    //                {
    //                    PartNames.Add(sdr["ProductName"].ToString());
    //                }
    //            }
    //            con.Close();
    //            return PartNames;
    //        }
    //    }
    //}

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["Id"] != null)
        {
            Response.Redirect("ApprovedInvoiceList.aspx");
        }
        else
        {
            Response.Redirect("TaxInvoiceList.aspx");
        }

    }


    //CONVRT NUMBERS TO WORD START

    public static string ConvertNumbertoWords(string numbers)
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
            var paisatext = ConvertNumbertoWords(Convert.ToString(paisaamt));
            sb.AppendFormat("Rupees {0} paise", paisatext);
        }
        return sb.ToString().TrimEnd();
    }
    private static String ones(String Number)
    {
        int _Number = Convert.ToInt32(Number);
        String name = "";
        switch (_Number)
        {

            case 1:
                name = "One";
                break;
            case 2:
                name = "Two";
                break;
            case 3:
                name = "Three";
                break;
            case 4:
                name = "Four";
                break;
            case 5:
                name = "Five";
                break;
            case 6:
                name = "Six";
                break;
            case 7:
                name = "Seven";
                break;
            case 8:
                name = "Eight";
                break;
            case 9:
                name = "Nine";
                break;
        }
        return name;
    }
    private static String tens(String Number)
    {
        int _Number = Convert.ToInt32(Number);
        String name = null;
        switch (_Number)
        {
            case 10:
                name = "Ten";
                break;
            case 11:
                name = "Eleven";
                break;
            case 12:
                name = "Twelve";
                break;
            case 13:
                name = "Thirteen";
                break;
            case 14:
                name = "Fourteen";
                break;
            case 15:
                name = "Fifteen";
                break;
            case 16:
                name = "Sixteen";
                break;
            case 17:
                name = "Seventeen";
                break;
            case 18:
                name = "Eighteen";
                break;
            case 19:
                name = "Nineteen";
                break;
            case 20:
                name = "Twenty";
                break;
            case 30:
                name = "Thirty";
                break;
            case 40:
                name = "Fourty";
                break;
            case 50:
                name = "Fifty";
                break;
            case 60:
                name = "Sixty";
                break;
            case 70:
                name = "Seventy";
                break;
            case 80:
                name = "Eighty";
                break;
            case 90:
                name = "Ninety";
                break;
            default:
                if (_Number > 0)
                {
                    name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                }
                break;
        }
        return name;
    }
    private static String ConvertWholeNumber(String Number)
    {
        string word = "";
        try
        {
            bool beginsZero = false;//tests for 0XX  
            bool isDone = false;//test if already translated  
            double dblAmt = (Convert.ToDouble(Number));
            //if ((dblAmt > 0) && number.StartsWith("0"))  
            if (dblAmt > 0)
            {//test for zero or digit zero in a nuemric  
                beginsZero = Number.StartsWith("0");

                int numDigits = Number.Length;
                int pos = 0;//store digit grouping  
                String place = "";//digit grouping name:hundres,thousand,etc...  
                switch (numDigits)
                {
                    case 1://ones' range  

                        word = ones(Number);
                        isDone = true;
                        break;
                    case 2://tens' range  
                        word = tens(Number);
                        isDone = true;
                        break;
                    case 3://hundreds' range  
                        pos = (numDigits % 3) + 1;
                        place = " Hundred ";
                        break;
                    case 4://thousands' range  
                    case 5:
                    case 6:
                        pos = (numDigits % 4) + 1;
                        place = " Thousand ";
                        break;
                    case 7://millions' range  
                    case 8:
                        pos = (numDigits % 6) + 1;
                        place = " Lac ";
                        break;
                    case 9:
                        pos = (numDigits % 8) + 1;
                        place = " Million ";
                        break;
                    case 10://Billions's range  
                    case 11:
                    case 12:

                        pos = (numDigits % 10) + 1;
                        place = " Billion ";
                        break;
                    //add extra case options for anything above Billion...  
                    default:
                        isDone = true;
                        break;
                }
                if (!isDone)
                {//if transalation is not done, continue...(Recursion comes in now!!)  
                    if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                    {
                        try
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                        }
                        catch { }
                    }
                    else
                    {
                        word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                    }

                    //check for trailing zeros  
                    //if (beginsZero) word = " and " + word.Trim();  
                }
                //ignore digit grouping names  
                if (word.Trim().Equals(place.Trim())) word = "";
            }
        }
        catch { }
        return word.Trim();
    }
    private static String ConvertToWords(String numb)
    {
        String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
        String endStr = "Only";
        try
        {
            int decimalPlace = numb.IndexOf(".");
            if (decimalPlace > 0)
            {
                wholeNo = numb.Substring(0, decimalPlace);
                points = numb.Substring(decimalPlace + 1);
                if (Convert.ToInt32(points) > 0)
                {
                    andStr = "and";// just to separate whole numbers from points/cents  
                    endStr = "Paisa " + endStr;//Cents  
                    pointStr = ConvertDecimals(points);
                }
            }
            val = String.Format("{0} {1}{2} {3}", ConvertNumbertoWords(wholeNo).Trim(), andStr, pointStr, endStr);
        }
        catch { }
        return val;
    }
    private static String ConvertDecimals(String number)
    {
        String cd = "", digit = "", engOne = "";
        for (int i = 0; i < number.Length; i++)
        {
            digit = number[i].ToString();
            if (digit.Equals("0"))
            {
                engOne = "Zero";
            }
            else
            {
                engOne = ones(digit);
            }
            cd += " " + engOne;
        }
        return cd;
    }

    //CONVRT NUMBERS TO WORD START END

    protected void ddlInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlInvoiceType.SelectedItem.Text == "Regular")
        {
            GenerateCode();
        }
        else
        {
            GenerateExportInvoiceCode();
        }
    }
}
