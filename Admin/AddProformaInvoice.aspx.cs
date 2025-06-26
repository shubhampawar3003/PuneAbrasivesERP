
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_AddProformaInvoice : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable Dt_Product = new DataTable();
    CommonCls objcls = new CommonCls();
    DataTable Dt_Component = new DataTable();
    byte[] bytePdf;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

            if (Session["UserCode"] == null)
            {
                Response.Redirect("../Login.aspx");
            }
            else
            {
                txtinvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                txtpodate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                //txt.Text = DateTime.Now.ToString("yyyy-MM-dd");
                FillddlProduct(); TaxInvoiceNo();

                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                }
                ViewState["RowNo"] = 0;
                Dt_Product.Columns.AddRange(new DataColumn[18] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("Batchno"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
                ViewState["TaxInvoiceDetails"] = Dt_Product;

                //Edit 
                if (Request.QueryString["ID"] != null)
                {
                    ID = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    // txtcompanycode.ReadOnly = true;
                    btnsave.Text = "Update";
                    ShowDtlEdit();
                    hhd.Value = ID;

                }
            }


        }
    }
    protected void TaxInvoiceNo()
    {

        string FinYear = null;
        string FinFullYear = null;
        if (DateTime.Today.Month > 3)
        {
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
            FinFullYear = DateTime.Today.AddYears(1).ToString("yyyy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();

            var finfYear = DateTime.Today.AddYears(1).ToString("yyyy");
            FinFullYear = (Convert.ToInt32(finfYear) - 1).ToString();
        }
        string previousyear = (Convert.ToDecimal(FinFullYear) - 1).ToString();
        string strInvoiceNumber = "";
        string fY = previousyear.ToString() + "-" + FinYear;
        string strSelect = @"select     CONCAT(
        '2024-25/', 
        RIGHT('0000' + CAST(CAST(SUBSTRING(MAX(InvoiceNo), 10, 4) AS INT)  AS VARCHAR), 4)
    ) AS maxno from tbl_ProformaTaxInvoiceHdr where InvoiceNo like '%" + fY + "%' ";
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
            strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/00" + numbervalue;
        }
        else
        {
            strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + "01";
        }
        txtInvoiceno.Text = strInvoiceNumber;
    }
    //Bind Product Type
    private void Fillddlagainstnumber()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [Pono] FROM [tbl_CustomerPurchaseOrderHdr] WHERE IsDeleted=0 AND CustomerName='" + txtbillingcustomer.Text + "'  ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtagainstNumber.DataSource = dt;
            txtagainstNumber.DataTextField = "Pono";
            txtagainstNumber.DataBind();
            txtagainstNumber.Items.Insert(0, " ---  Select Type  --- ");
        }
        //SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [CustomerPO] FROM [tbl_ProformaTaxInvoiceHdr] ", Cls_Main.Conn);
        //DataTable dt = new DataTable();
        //ad.Fill(dt);
        //if (dt.Rows.Count > 0)
        //{
        //    txtagainstNumber.DataSource = dt;
        //    txtagainstNumber.DataTextField = "CustomerPO";
        //    txtagainstNumber.DataBind();
        //    txtagainstNumber.Items.Insert(0, " ---  Select Type  --- ");
        //}
    }

    //Bind Product
    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster] WHERE status=1 AND isdeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlProduct.DataSource = dt;
            ddlProduct.DataTextField = "ProductName";
            ddlProduct.DataBind();
            ddlProduct.Items.Insert(0, "-- Select Product --");
        }
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


    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            Fillddlshippingaddress(Dt.Rows[0]["ShippingCustomer"].ToString());
            txtbillingcustomer.Text = Dt.Rows[0]["BillingCustomer"].ToString();
            txtshippingcustomer.Text = Dt.Rows[0]["ShippingCustomer"].ToString();
            ddlBillAddress.SelectedItem.Text = Dt.Rows[0]["BillingAddress"].ToString();
            //Fillddlshippingaddress(txtshippingcustomer.Text);
            ddlShippingaddress.SelectedItem.Text = Dt.Rows[0]["ShippingAddress"].ToString();
            DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["Invoicedate"].ToString());
            txtinvoiceDate.Text = ffff1.ToString("yyyy-MM-dd");
            txtcustomerPoNo.Text = Dt.Rows[0]["CustomerPO"].ToString();
            DateTime ffff2 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            txtpodate.Text = ffff2.ToString("yyyy-MM-dd");
            txtchallanNo.Text = Dt.Rows[0]["ChallanNo"].ToString();
            DateTime fff = Convert.ToDateTime(Dt.Rows[0]["ChallanDate"].ToString());
            txtchallanDate.Text = fff.ToString("yyyy-MM-dd");
            txtinvoiceagainst.SelectedItem.Text = Dt.Rows[0]["InvoiceAgainst"].ToString();
            Fillddlagainstnumber();
            txtagainstNumber.SelectedItem.Text = Dt.Rows[0]["AgaintsNumber"].ToString();
            //.Text = Dt.Rows[0]["PayTerms"].ToString();
            txtInvoiceno.Text = Dt.Rows[0]["InvoiceNo"].ToString();
            txtbillinglocation.Text = Dt.Rows[0]["Billinglocation"].ToString();
            txtshippinglocation.Text = Dt.Rows[0]["Shippinglocation"].ToString();
            txtbillingPincode.Text = Dt.Rows[0]["BillingPINCODE"].ToString();
            txtshippingPincode.Text = Dt.Rows[0]["ShippingPINCODE"].ToString();
            txtbillingGST.Text = Dt.Rows[0]["BillingGST"].ToString();
            txtshippingGST.Text = Dt.Rows[0]["ShippingGST"].ToString();
            txtbillingstatecode.Text = Dt.Rows[0]["BillingStateCode"].ToString();
            txtshippingstatecode.Text = Dt.Rows[0]["ShippingStateCode"].ToString();
            txtContactNo.Text = Dt.Rows[0]["ContactNo"].ToString();
            txtemail.Text = Dt.Rows[0]["EmailID"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
            txttransportMode.SelectedItem.Text = Dt.Rows[0]["TransportMode"].ToString();
            if (txttransportMode.SelectedItem.Text == "By Road" || txttransportMode.SelectedItem.Text == "By Courier")
            {
                divtransportdetails.Visible = true;
                txtvehicalNumber.Visible = true;
                txtvehicalNumber.Text = Dt.Rows[0]["TransportDetails"].ToString();
            }
            if (txttransportMode.SelectedItem.Text == "By Air")
            {
                divtransportdetails.Visible = true;
                txtByAir.Visible = true;
                txtByAir.Text = Dt.Rows[0]["TransportDetails"].ToString();
            }
            if (txttransportMode.SelectedItem.Text == "By Hand")
            {
                divtransportdetails.Visible = true;
                txtByHand.Visible = true;
                txtByHand.Text = Dt.Rows[0]["TransportDetails"].ToString();
            }
            txtremark.Text = Dt.Rows[0]["Remark"].ToString();
            txtebillnumber.Text = Dt.Rows[0]["EwaybillNo"].ToString();
            txtpanno.Text = Dt.Rows[0]["Panno"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();

            txtPayment.Text = Dt.Rows[0]["Payment"].ToString();
            txtTransport.Text = Dt.Rows[0]["Transport"].ToString();
            txtDeliveryTime.Text = Dt.Rows[0]["DeliveryTime"].ToString();
            txtPacking.Text = Dt.Rows[0]["Packing"].ToString();
            txtTaxs.Text = Dt.Rows[0]["Taxs"].ToString();
            //ShowDtlEdit();
        }
    }


    protected void ShowDtlEdit()
    {
        if (txtinvoiceagainst.SelectedItem.Text == "Direct")
        {
            txtagainstNumber.Enabled = false;
            TableDirect.Visible = true;
            TableOrder.Visible = true;
        }
        else if (txtinvoiceagainst.SelectedItem.Text == "Order")
        {
            TableDirect.Visible = false;
            TableOrder.Visible = true;
        }

        divTotalPart.Visible = true;
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT Productname,Description,HSN,Quantity,Units,Rate,Total,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Discountpercentage,DiscountAmount,Alltotal FROM [tbl_ProformaInvoiceDtls] WHERE Invoiceno='" + txtinvoiceno.Text + "'", Cls_Main.Conn);
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ProformaTaxInvoiceDtls] WHERE Invoiceno='" + txtInvoiceno.Text + "'", Cls_Main.Conn);
        DataTable DTCOMP = new DataTable();
        Da.Fill(DTCOMP);

        int count = 0;
        if (DTCOMP.Rows.Count > 0)
        {
            if (Dt_Product.Columns.Count < 0)
            {
                Show_Grid();
            }

            for (int i = 0; i < DTCOMP.Rows.Count; i++)
            {
                Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["Batchno"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
                count = count + 1;
            }
        }

        dgvTaxinvoiceDetails.EmptyDataText = "No Data Found";
        dgvTaxinvoiceDetails.DataSource = Dt_Product;
        dgvTaxinvoiceDetails.DataBind();
    }

    private void Show_Grid()
    {
        divTotalPart.Visible = true;
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
        DataTable Dt = (DataTable)ViewState["TaxInvoiceDetails"];
        Dt.Rows.Add(ViewState["RowNo"], ddlProduct.SelectedItem.Text, txtdescription.Text.Trim(), txtBatchno.Text.Trim(), txthsnsac.Text.Trim(), txtquantity.Text, txtunit.Text, txtrate.Text, txttotal.Text, txtCGST.Text, txtCGSTamt.Text, txtSGST.Text, txtSGSTamt.Text, txtIGST.Text, txtIGSTamt.Text, txtdiscount.Text, txtdiscountamt.Text, txtgrandtotal.Text);
        ViewState["TaxInvoiceDetails"] = Dt;
        FillddlProduct();
        txtdescription.Text = string.Empty;
        txtBatchno.Text = string.Empty;
        txthsnsac.Text = string.Empty;
        txtquantity.Text = string.Empty;
        txtunit.Text = string.Empty;
        txtrate.Text = string.Empty;
        txttotal.Text = string.Empty;
        txtCGST.Text = string.Empty;
        txtCGSTamt.Text = string.Empty;
        txtSGST.Text = string.Empty;
        txtSGSTamt.Text = string.Empty;
        txtIGST.Text = string.Empty;
        txtIGSTamt.Text = string.Empty;
        txtdiscount.Text = string.Empty;
        txtdiscountamt.Text = string.Empty;
        txtgrandtotal.Text = string.Empty;
        dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
        dgvTaxinvoiceDetails.DataBind();
        TableOrder.Visible = true;
    }

    protected void btnAddMore_Click(object sender, EventArgs e)
    {
        if (txtquantity.Text == "" || txtrate.Text == "" || txtgrandtotal.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill Quantity and Price !!!');", true);
            txtquantity.Focus();
        }
        else
        {
            Show_Grid();
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        SaveRecord();
    }
    bool flgs;
    protected void SaveRecord()
    {

        try
        {
            if (txtbillingcustomer.Text == "" || txtshippingcustomer.Text == "" || txtbillingstatecode.Text == "" || txtbillingPincode.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                if (dgvTaxinvoiceDetails.Rows.Count > 0)
                {
                    if (txtbillingcustomer.Text == "" && txtinvoiceDate.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
                    }
                    else
                    {
                        if (btnsave.Text == "Save")
                        {
                            Cls_Main.Conn_Open();
                            SqlCommand Cmd = new SqlCommand("SP_ProformaTaxInvoiceHdr", Cls_Main.Conn);
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.Parameters.AddWithValue("@Action", "Save");
                            Cmd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                            Cmd.Parameters.AddWithValue("@InvoiceType", ddlInvoiceType.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoiceDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingcustomer", txtbillingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingcustomer", txtshippingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingaddress", ddlBillAddress.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingAddress", ddlShippingaddress.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billinglocation", txtbillinglocation.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippinglocation", txtshippinglocation.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingGST", txtbillingGST.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingGST", txtshippingGST.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingPincode", txtbillingPincode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingPincode", txtshippingPincode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingstatecode", txtbillingstatecode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingstatecode", txtshippingstatecode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim());
                            Cmd.Parameters.AddWithValue("@invoiceagainst", txtinvoiceagainst.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@againstNumber", txtagainstNumber.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@customerPoNo", txtcustomerPoNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@podate", txtpodate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanNo", txtchallanNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanDate", txtchallanDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@AmountInWords", ConvertToWords(txt_grandTotal.Text));
                            if (txttransportMode.SelectedItem.Text == "--Select--")
                            {
                                Cmd.Parameters.AddWithValue("@transportMode", DBNull.Value);
                            }
                            else
                            {
                                Cmd.Parameters.AddWithValue("@transportMode", txttransportMode.SelectedItem.Text.Trim());
                            }

                            if (txttransportMode.SelectedItem.Text == "By Road" || txttransportMode.SelectedItem.Text == "By Courier")
                            {
                                Cmd.Parameters.AddWithValue("@TransportDetails", txtvehicalNumber.Text);
                            }
                            if (txttransportMode.SelectedItem.Text == "By Air")
                            {

                                Cmd.Parameters.AddWithValue("@TransportDetails", txtByAir.Text);
                            }
                            if (txttransportMode.SelectedItem.Text == "By Hand")
                            {

                                Cmd.Parameters.AddWithValue("@TransportDetails", txtByHand.Text);
                            }

                            Cmd.Parameters.AddWithValue("@remark", txtremark.Text.Trim());
                            Cmd.Parameters.AddWithValue("@PaymentTerm", Convert.ToInt64(txtPaymentTerm.Text));
                            Cmd.Parameters.AddWithValue("@Payment", txtPayment.Text);
                            Cmd.Parameters.AddWithValue("@Transport", txtTransport.Text);
                            Cmd.Parameters.AddWithValue("@DeliveryTime", txtDeliveryTime.Text);
                            Cmd.Parameters.AddWithValue("@Packing", txtPacking.Text);
                            Cmd.Parameters.AddWithValue("@Taxs", txtTaxs.Text);
                            Cmd.Parameters.AddWithValue("@Panno", txtpanno.Text.Trim());
                            Cmd.Parameters.AddWithValue("@ebillnumber", txtebillnumber.Text.Trim());
                            Cmd.Parameters.AddWithValue("@CGST_Amt", txt_cgstamt.Text);
                            Cmd.Parameters.AddWithValue("@SGST_Amt", txt_sgstamt.Text);
                            Cmd.Parameters.AddWithValue("@IGST_Amt", txt_igstamt.Text);
                            Cmd.Parameters.AddWithValue("@Total_Price", txt_grandTotal.Text);
                            Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                            Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                            Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                            Cmd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                            Cls_Main.Conn_Dispose();

                            //Save Tax Invoice Details 

                            foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
                            {
                                string lblproduct = (grd1.FindControl("lblproduct") as Label).Text;
                                string lblDescription = (grd1.FindControl("lblDescription") as Label).Text;
                                string lblhsn = (grd1.FindControl("lblhsn") as Label).Text;
                                string lblQuantity = (grd1.FindControl("lblQuantity") as Label).Text;
                                string lblUnit = (grd1.FindControl("lblUnit") as Label).Text;
                                string lblRate = (grd1.FindControl("lblRate") as Label).Text;
                                string lblTotal = (grd1.FindControl("lblTotal") as Label).Text;
                                string lblCGSTPer = (grd1.FindControl("lblCGSTPer") as Label).Text;
                                string lblCGST = (grd1.FindControl("lblCGST") as Label).Text;
                                string lblSGSTPer = (grd1.FindControl("lblSGSTPer") as Label).Text;
                                string lblSGST = (grd1.FindControl("lblSGST") as Label).Text;
                                string lblIGSTPer = (grd1.FindControl("lblIGSTPer") as Label).Text;
                                string lblIGST = (grd1.FindControl("lblIGST") as Label).Text;
                                string lblDiscount = (grd1.FindControl("lblDiscount") as Label).Text;
                                string lblDiscountAmount = (grd1.FindControl("lblDiscountAmount") as Label).Text;
                                string lblAlltotal = (grd1.FindControl("lblAlltotal") as Label).Text;
                                string Batchno = (grd1.FindControl("lblBatchno") as Label).Text;

                                Cls_Main.Conn_Open();
                                SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_ProformaTaxInvoiceDtls] (Invoiceno,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batchno) VALUES(@Invoiceno,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batchno)", Cls_Main.Conn);
                                cmdd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                                cmdd.Parameters.AddWithValue("@Productname", lblproduct);
                                cmdd.Parameters.AddWithValue("@Description", lblDescription);
                                cmdd.Parameters.AddWithValue("@Batchno", Batchno);
                                cmdd.Parameters.AddWithValue("@HSN", lblhsn);
                                cmdd.Parameters.AddWithValue("@Quantity", lblQuantity);
                                cmdd.Parameters.AddWithValue("@Units", lblUnit);
                                cmdd.Parameters.AddWithValue("@Rate", lblRate);
                                cmdd.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                                cmdd.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                                cmdd.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                                cmdd.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                                cmdd.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                                cmdd.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                                cmdd.Parameters.AddWithValue("@Total", lblTotal);
                                cmdd.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                                cmdd.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);
                                cmdd.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                                cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                                cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                                cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();
                            }
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Proforma Invoice Save Successfully..!!');window.location='ProformaInvoiceList.aspx'; ", true);
                            //Save Contact Details End
                        }
                        else if (btnsave.Text == "Update")
                        {
                            Cls_Main.Conn_Open();
                            SqlCommand Cmd = new SqlCommand("SP_ProformaTaxInvoiceHdr", Cls_Main.Conn);
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.Parameters.AddWithValue("@Action", "Update");
                            Cmd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                            Cmd.Parameters.AddWithValue("@InvoiceType", ddlInvoiceType.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoiceDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingcustomer", txtbillingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingcustomer", txtshippingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingaddress", ddlBillAddress.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingAddress", ddlShippingaddress.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billinglocation", txtbillinglocation.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippinglocation", txtshippinglocation.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingGST", txtbillingGST.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingGST", txtshippingGST.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingPincode", txtbillingPincode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingPincode", txtshippingPincode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingstatecode", txtbillingstatecode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingstatecode", txtshippingstatecode.Text.Trim());
                            Cmd.Parameters.AddWithValue("@ContactNo", txtContactNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@email", txtemail.Text.Trim());
                            Cmd.Parameters.AddWithValue("@invoiceagainst", txtinvoiceagainst.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@againstNumber", txtagainstNumber.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@customerPoNo", txtcustomerPoNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@podate", txtpodate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanNo", txtchallanNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanDate", txtchallanDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@AmountInWords", ConvertToWords(txt_grandTotal.Text));
                            if (txttransportMode.SelectedItem.Text == "--Select--")
                            {
                                Cmd.Parameters.AddWithValue("@transportMode", DBNull.Value);
                            }
                            else
                            {
                                Cmd.Parameters.AddWithValue("@transportMode", txttransportMode.SelectedItem.Text.Trim());
                            }
                            if (txttransportMode.SelectedItem.Text == "By Road" || txttransportMode.SelectedItem.Text == "By Courier")
                            {
                                Cmd.Parameters.AddWithValue("@TransportDetails", txtvehicalNumber.Text);
                            }
                            if (txttransportMode.SelectedItem.Text == "By Air")
                            {

                                Cmd.Parameters.AddWithValue("@TransportDetails", txtByAir.Text);
                            }
                            if (txttransportMode.SelectedItem.Text == "By Hand")
                            {

                                Cmd.Parameters.AddWithValue("@TransportDetails", txtByHand.Text);
                            }
                            Cmd.Parameters.AddWithValue("@remark", txtremark.Text.Trim());
                            Cmd.Parameters.AddWithValue("@Panno", txtpanno.Text.Trim());
                            Cmd.Parameters.AddWithValue("@PaymentTerm", Convert.ToInt64(txtPaymentTerm.Text));
                            Cmd.Parameters.AddWithValue("@Payment", txtPayment.Text);
                            Cmd.Parameters.AddWithValue("@Transport", txtTransport.Text);
                            Cmd.Parameters.AddWithValue("@DeliveryTime", txtDeliveryTime.Text);
                            Cmd.Parameters.AddWithValue("@Packing", txtPacking.Text);
                            Cmd.Parameters.AddWithValue("@Taxs", txtTaxs.Text);
                            Cmd.Parameters.AddWithValue("@ebillnumber", txtebillnumber.Text.Trim());
                            Cmd.Parameters.AddWithValue("@CGST_Amt", txt_cgstamt.Text);
                            Cmd.Parameters.AddWithValue("@SGST_Amt", txt_sgstamt.Text);
                            Cmd.Parameters.AddWithValue("@IGST_Amt", txt_igstamt.Text);
                            Cmd.Parameters.AddWithValue("@Total_Price", txt_grandTotal.Text);
                            Cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                            Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                            Cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                            Cmd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                            Cls_Main.Conn_Dispose();

                            Cls_Main.Conn_Open();
                            SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_ProformaTaxInvoiceDtls WHERE Invoiceno=@Invoiceno", Cls_Main.Conn);
                            cmddelete.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                            cmddelete.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                            //Save Tax Invoice Details 

                            foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
                            {
                                string lblproduct = (grd1.FindControl("lblproduct") as Label).Text;
                                string lblDescription = (grd1.FindControl("lblDescription") as Label).Text;
                                string lblhsn = (grd1.FindControl("lblhsn") as Label).Text;
                                string lblQuantity = (grd1.FindControl("lblQuantity") as Label).Text;
                                string lblUnit = (grd1.FindControl("lblUnit") as Label).Text;
                                string lblRate = (grd1.FindControl("lblRate") as Label).Text;
                                string lblTotal = (grd1.FindControl("lblTotal") as Label).Text;
                                string lblCGSTPer = (grd1.FindControl("lblCGSTPer") as Label).Text;
                                string lblCGST = (grd1.FindControl("lblCGST") as Label).Text;
                                string lblSGSTPer = (grd1.FindControl("lblSGSTPer") as Label).Text;
                                string lblSGST = (grd1.FindControl("lblSGST") as Label).Text;
                                string lblIGSTPer = (grd1.FindControl("lblIGSTPer") as Label).Text;
                                string lblIGST = (grd1.FindControl("lblIGST") as Label).Text;
                                string lblDiscount = (grd1.FindControl("lblDiscount") as Label).Text;
                                string lblDiscountAmount = (grd1.FindControl("lblDiscountAmount") as Label).Text;
                                string lblAlltotal = (grd1.FindControl("lblAlltotal") as Label).Text;
                                string Batchno = (grd1.FindControl("lblBatchno") as Label).Text;

                                Cls_Main.Conn_Open();
                                SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_ProformaTaxInvoiceDtls] (Invoiceno,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batchno) VALUES(@Invoiceno,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batchno)", Cls_Main.Conn);
                                cmdd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                                cmdd.Parameters.AddWithValue("@Productname", lblproduct);
                                cmdd.Parameters.AddWithValue("@Description", lblDescription);
                                cmdd.Parameters.AddWithValue("@Batchno", Batchno);
                                cmdd.Parameters.AddWithValue("@HSN", lblhsn);
                                cmdd.Parameters.AddWithValue("@Quantity", lblQuantity);
                                cmdd.Parameters.AddWithValue("@Units", lblUnit);
                                cmdd.Parameters.AddWithValue("@Rate", lblRate);
                                cmdd.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                                cmdd.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                                cmdd.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                                cmdd.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                                cmdd.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                                cmdd.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                                cmdd.Parameters.AddWithValue("@Total", lblTotal);
                                cmdd.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                                cmdd.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);
                                cmdd.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                                cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                                cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                                cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();
                            }


                            //Save Tax Invoice Details End
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Proforma Invoice Updated Successfully..!!');window.location='ProformaInvoiceList.aspx'; ", true);
                        }

                        if (gvUserMail.Rows.Count > 0)
                        {
                            foreach (GridViewRow g2 in gvUserMail.Rows)
                            {
                                bool chk = (g2.FindControl("chkSelect") as CheckBox).Checked;

                                while (chk == true)
                                {
                                    flgs = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            flgs = false;
                        }
                        if (flgs == true)
                        {

                            mailsendforCustomerContact();

                        }
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Product data Data..!!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProformaInvoiceList.aspx");
    }

    protected void gv_cancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Product = ((TextBox)row.FindControl("txtproduct")).Text;
        string Description = ((TextBox)row.FindControl("txtDescription")).Text;
        string HSN = ((TextBox)row.FindControl("txthsn")).Text;
        string Quantity = ((TextBox)row.FindControl("txtQuantity")).Text;
        string Unit = ((TextBox)row.FindControl("txtUnit")).Text;
        string Rate = ((TextBox)row.FindControl("txtRate")).Text;
        string Total = ((TextBox)row.FindControl("txtTotal")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGST")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGST")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGST")).Text;
        string Discount = ((TextBox)row.FindControl("txtDiscount")).Text;
        string AllTotal = ((TextBox)row.FindControl("txtAlltotal")).Text;
        string Batchno = ((TextBox)row.FindControl("txtBatchno")).Text;
        DataTable Dt = ViewState["TaxInvoiceDetails"] as DataTable;
        Dt.Rows[row.RowIndex]["Productname"] = Product;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["Batchno"] = Batchno;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Quantity"] = Quantity;
        Dt.Rows[row.RowIndex]["Units"] = Unit;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Total"] = Total;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["Discountpercentage"] = Discount;
        Dt.Rows[row.RowIndex]["Alltotal"] = AllTotal;
        Dt.AcceptChanges();
        ViewState["TaxInvoiceDetails"] = Dt;
        dgvTaxinvoiceDetails.EditIndex = -1;
        dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
        dgvTaxinvoiceDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["TaxInvoiceDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["TaxInvoiceDetails"] = dt;
        dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
        dgvTaxinvoiceDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Proforma Details Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    //protected void dgvContactDetails_RowEditing(object sender, GridViewEditEventArgs e)
    //{
    //    dgvTaxinvoiceDetails.EditIndex = e.NewEditIndex;
    //    dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
    //    dgvTaxinvoiceDetails.DataBind();
    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    //}

    protected void gv_update_Click(object sender, EventArgs e)
    {

        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Product = ((TextBox)row.FindControl("txtproduct")).Text;
        string Description = ((TextBox)row.FindControl("txtDescription")).Text;
        string HSN = ((TextBox)row.FindControl("txthsn")).Text;
        string Quantity = ((TextBox)row.FindControl("txtQuantity")).Text;
        string Unit = ((TextBox)row.FindControl("txtUnit")).Text;
        string Rate = ((TextBox)row.FindControl("txtRate")).Text;
        string Total = ((TextBox)row.FindControl("txtTotal")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGST")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGST")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGST")).Text;
        string Discount = ((TextBox)row.FindControl("txtDiscount")).Text;
        string DiscountAmt = ((TextBox)row.FindControl("txtDiscountAmount")).Text;
        string AllTotal = ((TextBox)row.FindControl("txtAlltotal")).Text;
        string Batchno = ((TextBox)row.FindControl("txtBatchno")).Text;
        DataTable Dt = ViewState["TaxInvoiceDetails"] as DataTable;
        Dt.Rows[row.RowIndex]["Productname"] = Product;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["Batchno"] = Batchno;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Quantity"] = Quantity;
        Dt.Rows[row.RowIndex]["Units"] = Unit;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Total"] = Total;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["Discountpercentage"] = Discount;
        Dt.Rows[row.RowIndex]["DiscountAmount"] = DiscountAmt;
        Dt.Rows[row.RowIndex]["Alltotal"] = AllTotal;
        Dt.AcceptChanges();
        ViewState["TaxInvoiceDetails"] = Dt;
        dgvTaxinvoiceDetails.EditIndex = -1;
        dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
        dgvTaxinvoiceDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void check_addresss_CheckedChanged(object sender, EventArgs e)
    {
        //if (check_addresss.Checked == true)
        //{
        //    txtshippingaddress.Text = txtaddress.Text;
        //}
    }

    protected void dgvTaxinvoiceDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvTaxinvoiceDetails.EditIndex = e.NewEditIndex;
        dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
        dgvTaxinvoiceDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void txtquantity_TextChanged(object sender, EventArgs e)
    {
        int value;
        if (int.TryParse(txtquantity.Text, out value))
        {

            var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());
            txttotal.Text = Convert.ToString(TotalAmt);
            decimal total;
            decimal Percentage;
            if (txtIGST.Text == null || txtIGST.Text == "")
            {
                Percentage = Convert.ToDecimal(txtCGST.Text);
                total = (TotalAmt * Percentage / 100);

                txtCGSTamt.Text = total.ToString();

                txtSGSTamt.Text = txtCGSTamt.Text;

                txtSGST.Text = txtCGST.Text;
                var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
                txtgrandtotal.Text = GrandTotal.ToString();
            }
            else
            {
                Percentage = Convert.ToDecimal(txtIGST.Text);
                total = (TotalAmt * Percentage / 100);

                txtIGSTamt.Text = total.ToString();
                var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtIGSTamt.Text.Trim());
                txtgrandtotal.Text = GrandTotal.ToString();
            }

        }
    }

    protected void txtCGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtCGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtCGSTamt.Text = total.ToString();

        txtSGSTamt.Text = txtCGSTamt.Text;

        txtSGST.Text = txtCGST.Text;

        var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
        txtgrandtotal.Text = GrandTotal.ToString();


        if (txtCGST.Text == "0" || txtCGST.Text == "")
        {
            txtIGST.Enabled = true;
            txtIGST.Text = "0";
        }
        else
        {
            txtIGST.Enabled = false;
            txtIGST.Text = "0";
        }
    }

    protected void txtSGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtSGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtSGSTamt.Text = total.ToString();

        txtCGSTamt.Text = txtSGSTamt.Text;

        txtCGST.Text = txtSGST.Text;

        var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
        txtgrandtotal.Text = GrandTotal.ToString();

        if (txtSGST.Text == "0" || txtSGST.Text == "")
        {
            txtIGST.Enabled = true;
            txtIGST.Text = "0";
        }
        else
        {
            txtIGST.Enabled = false;
            txtIGST.Text = "0";
        }
    }

    protected void txtIGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtIGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtIGSTamt.Text = total.ToString();

        var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtIGSTamt.Text.Trim());
        txtgrandtotal.Text = GrandTotal.ToString();


        if (txtIGST.Text == "0" || txtIGST.Text == "")
        {
            txtCGST.Enabled = true;
            txtCGST.Text = "0";
            txtSGST.Enabled = true;
            txtSGST.Text = "0";
        }
        else
        {
            txtCGST.Enabled = false;
            txtCGST.Text = "0";
            txtSGST.Enabled = false;
            txtSGST.Text = "0";
        }
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        decimal DiscountAmt;
        decimal val1 = Convert.ToDecimal(txtgrandtotal.Text);
        decimal val2 = Convert.ToDecimal(txtdiscount.Text);
        DiscountAmt = (val1 * val2 / 100);
        txtgrandtotal.Text = (val1 - DiscountAmt).ToString();

        txtdiscountamt.Text = DiscountAmt.ToString();
    }


    string lblTotal, lblCGST, lblSGST;
    private decimal Total, CGSTAmt, SGSTAmt, IGSTAmt, Alltotal;
    string lblproduct, lblproducttype, lblsubtype, lblproductbrand, lblton, Description, hsn, Quantity, Unit, Rate, subTotal, CGSTPer, CGST, SGSTPer, SGST, IGSTPer, IGST, Discount, Grandtotal;
    protected void dgvTaxinvoiceDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkedit = e.Row.FindControl("btn_edit") as LinkButton;
                if (lnkedit == null)
                {

                    lblproduct = (e.Row.FindControl("txtproduct") as TextBox).Text;

                    Description = (e.Row.FindControl("txtDescription") as TextBox).Text;
                    hsn = (e.Row.FindControl("txthsn") as TextBox).Text;
                    Quantity = (e.Row.FindControl("txtQuantity") as TextBox).Text;
                    Unit = (e.Row.FindControl("txtUnit") as TextBox).Text;
                    Rate = (e.Row.FindControl("txtRate") as TextBox).Text;
                    subTotal = (e.Row.FindControl("txtTotal") as TextBox).Text;
                    CGSTPer = (e.Row.FindControl("txtCGSTPer") as TextBox).Text;
                    CGST = (e.Row.FindControl("txtCGST") as TextBox).Text;
                    SGSTPer = (e.Row.FindControl("txtSGSTPer") as TextBox).Text;
                    SGST = (e.Row.FindControl("txtSGST") as TextBox).Text;
                    IGSTPer = (e.Row.FindControl("txtIGSTPer") as TextBox).Text;
                    IGST = (e.Row.FindControl("txtIGST") as TextBox).Text;
                    Discount = (e.Row.FindControl("txtDiscount") as TextBox).Text;
                    Grandtotal = (e.Row.FindControl("txtAlltotal") as TextBox).Text;

                }
                else
                {
                    Total += Convert.ToDecimal((e.Row.FindControl("lblTotal") as Label).Text);
                    txt_Subtotal.Text = Total.ToString("0.00");

                    CGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblCGST") as Label).Text);
                    txt_cgstamt.Text = CGSTAmt.ToString("0.00");

                    SGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblSGST") as Label).Text);
                    txt_sgstamt.Text = SGSTAmt.ToString("0.00");

                    IGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblIGST") as Label).Text);
                    txt_igstamt.Text = IGSTAmt.ToString("0.00");

                    Alltotal += Convert.ToDecimal((e.Row.FindControl("lblAlltotal") as Label).Text);
                    txt_grandTotal.Text = Alltotal.ToString("0.00");

                    //Amount Convert into word
                    //string number = txt_grandTotal.Text;
                    //number = Convert.ToDouble(number).ToString();
                    //string Amtinword = ConvertNumbertoWords(Convert.ToInt32(number));
                    //lbl_total_amt_Value.Text = Amtinword;

                    string isNegative = "";
                    try
                    {
                        string number = txt_grandTotal.Text;

                        number = Convert.ToDouble(number).ToString();

                        if (number.Contains("-"))
                        {
                            isNegative = "Minus ";
                            number = number.Substring(1, number.Length - 1);
                        }
                        else
                        {
                            lbl_total_amt_Value.Text = isNegative + ConvertToWords(number);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        catch (Exception ex)
        {

            throw ex;
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


    protected void ddlagainst_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //DataTable dtorderno = new DataTable();
            //SqlDataAdapter sadorderno = new SqlDataAdapter("select * from [tbl_QuotationDtls] where Quotation_no='" + ddlagainst.SelectedValue + "'", Cls_Main.Conn);
            //sadorderno.Fill(dtorderno);

            //dgvTaxinvoiceDetails.DataSource = dtorderno;
            //dgvTaxinvoiceDetails.DataBind();

            //DataTable dtQuoDetails = new DataTable();
            //SqlDataAdapter saQuDetails = new SqlDataAdapter("select * from tbl_QuotationHdr where Quotationno='" + ddlagainst.SelectedValue + "'", Cls_Main.Conn);
            //saQuDetails.Fill(dtQuoDetails);
            //this.dgvTaxinvoiceDetails.Columns[17].Visible = false;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void lnkBtmNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("CompanyMaster.aspx");
    }

    protected void txtQuantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    public void Calculations(GridViewRow row)
    {
        TextBox Rate = (TextBox)row.FindControl("txtrate");
        TextBox Qty = (TextBox)row.FindControl("txtquantity");
        TextBox Total = (TextBox)row.FindControl("txttotal");
        TextBox CGSTPer = (TextBox)row.FindControl("txtCGSTPer");
        TextBox SGSTPer = (TextBox)row.FindControl("txtSGSTPer");
        TextBox IGSTPer = (TextBox)row.FindControl("txtIGSTPer");
        TextBox txtCGSTamt = (TextBox)row.FindControl("txtCGST");
        TextBox txtSGSTamt = (TextBox)row.FindControl("txtSGST");
        TextBox txtIGSTamt = (TextBox)row.FindControl("txtIGST");
        TextBox Disc_Per = (TextBox)row.FindControl("txtdiscount");
        TextBox txtDiscountAmount = (TextBox)row.FindControl("txtDiscountAmount");

        TextBox GrossTotal = (TextBox)row.FindControl("txtgrandtotal");
        TextBox txtAlltotal = (TextBox)row.FindControl("txtAlltotal");
        int value;
        if (int.TryParse(Qty.Text, out value))
        {
            // Check if the value is a multiple of 25
            if (value % 25 == 0)
            {
                var total = Convert.ToDecimal(Rate.Text) * Convert.ToDecimal(Qty.Text);
                Total.Text = string.Format("{0:0.00}", total);

                decimal tax_amt;
                decimal cgst_amt;
                decimal sgst_amt;
                decimal igst_amt;

                if (string.IsNullOrEmpty(CGSTPer.Text))
                {
                    cgst_amt = 0;
                }
                else
                {
                    cgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(CGSTPer.Text) / 100;
                }
                txtCGSTamt.Text = string.Format("{0:0.00}", cgst_amt);

                if (string.IsNullOrEmpty(SGSTPer.Text))
                {
                    sgst_amt = 0;
                }
                else
                {
                    sgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(SGSTPer.Text) / 100;
                }
                txtSGSTamt.Text = string.Format("{0:0.00}", sgst_amt);

                if (string.IsNullOrEmpty(IGSTPer.Text))
                {
                    igst_amt = 0;
                }
                else
                {
                    igst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(IGSTPer.Text) / 100;
                }
                txtIGSTamt.Text = string.Format("{0:0.00}", igst_amt);

                tax_amt = cgst_amt + sgst_amt + igst_amt;

                var totalWithTax = Convert.ToDecimal(total.ToString()) + Convert.ToDecimal(tax_amt.ToString());
                decimal disc_amt;
                if (string.IsNullOrEmpty(Disc_Per.Text))
                {
                    disc_amt = 0;
                }
                else
                {
                    disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
                    //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
                }

                var Grossamt = Convert.ToDecimal(total.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
                txtAlltotal.Text = string.Format("{0:0.00}", Grossamt);
                txtDiscountAmount.Text = string.Format("{0:0}", disc_amt);
            }
            else
            {
                txtAlltotal.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a Quantity that is a multiple of 25...!!')", true);

            }
        }



        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }


    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void ddlProduct_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ProductMaster] WHERE ProductName='" + ddlProduct.SelectedItem.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            txtdescription.Text = Dt.Rows[0]["Description"].ToString();
            txthsnsac.Text = Dt.Rows[0]["HSN"].ToString();
            txtrate.Text = Dt.Rows[0]["Price"].ToString();
            txtunit.Text = Dt.Rows[0]["Unit"].ToString();

            string gstNumber = txtbillingGST.Text.Trim();
            string stateCode = gstNumber.Substring(0, 2);
            if (stateCode == "27")
            {
                txtCGST.Text = "9";
                txtSGST.Text = "9";
            }
            else
            {
                txtIGST.Text = "18";
            }
            txtCGSTamt.Text = "0.00";
            txtSGSTamt.Text = "0.00";
            txtIGSTamt.Text = "0.00";
            txtdiscount.Text = "0.00";
            txtdiscountamt.Text = "0.00";
            ddlProduct.Focus();
        }
    }

    protected void txtshippingcustomer_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode WHERE Companyname='" + txtshippingcustomer.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            Fillddlshippingaddress(dt.Rows[0]["Companyname"].ToString());
            if (!string.IsNullOrEmpty(dt.Rows[0]["Shippingaddress"].ToString()))
            {
                ddlBillAddress.SelectedItem.Text = dt.Rows[0]["Billingaddress"].ToString();
            }
            if (!string.IsNullOrEmpty(dt.Rows[0]["Shippingaddress"].ToString()))
            {
                ddlShippingaddress.SelectedValue = dt.Rows[0]["Shippingaddress"].ToString();
            }
            txtbillinglocation.Text = dt.Rows[0]["Billinglocation"].ToString();
            txtshippinglocation.Text = dt.Rows[0]["Shippinglocation"].ToString();
            txtbillingPincode.Text = dt.Rows[0]["Billingpincode"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["Shippingpincode"].ToString();
            txtbillingstatecode.Text = dt.Rows[0]["Billing_Statecode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["Shipping_Statecode"].ToString();
            txtbillingcustomer.Text = dt.Rows[0]["Companyname"].ToString();
            txtbillingGST.Text = dt.Rows[0]["GSTno"].ToString();
            if (string.IsNullOrEmpty(dt.Rows[0]["Companypancard"].ToString()))
            {
                string gstNumber = txtbillingGST.Text.Trim();
                txtpanno.Text = gstNumber.Substring(2, 10);

            }
            else
            {
                txtpanno.Text = dt.Rows[0]["Companypancard"].ToString();
            }
            txtshippingGST.Text = dt.Rows[0]["GSTno"].ToString();
            txtContactNo.Text = dt.Rows[0]["ContactNo"].ToString();
            txtemail.Text = dt.Rows[0]["PrimaryEmailID"].ToString();
            txtPaymentTerm.Text = dt.Rows[0]["PaymentTerm"].ToString();
            lblst.Text = dt.Rows[0]["Billing_Statecode"].ToString();
        }
    }

    protected void txtbillingcustomer_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM  WHERE Companyname='" + txtbillingcustomer.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            Fillddlshippingaddress(dt.Rows[0]["Companyname"].ToString());
            if (!string.IsNullOrEmpty(dt.Rows[0]["Billingaddress"].ToString()))
            {
                ddlBillAddress.SelectedItem.Text = dt.Rows[0]["Billingaddress"].ToString();
            }
            if (!string.IsNullOrEmpty(dt.Rows[0]["Shippingaddress"].ToString()))
            {
                ddlShippingaddress.SelectedValue = dt.Rows[0]["Shippingaddress"].ToString();
            }

            txtbillinglocation.Text = dt.Rows[0]["Billinglocation"].ToString();
            txtshippinglocation.Text = dt.Rows[0]["Shippinglocation"].ToString();
            txtbillingPincode.Text = dt.Rows[0]["Billingpincode"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["Shippingpincode"].ToString();
            txtbillingstatecode.Text = dt.Rows[0]["Billing_Statecode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["Shipping_Statecode"].ToString();
            txtshippingcustomer.Text = dt.Rows[0]["Companyname"].ToString();
            txtbillingGST.Text = dt.Rows[0]["GSTno"].ToString();
            if (string.IsNullOrEmpty(dt.Rows[0]["Companypancard"].ToString()))
            {
                string gstNumber = txtbillingGST.Text.Trim();
                txtpanno.Text = gstNumber.Substring(2, 10);

            }
            else
            {
                txtpanno.Text = dt.Rows[0]["Companypancard"].ToString();
            }
            txtshippingGST.Text = dt.Rows[0]["GSTno"].ToString();
            txtContactNo.Text = dt.Rows[0]["ContactNo"].ToString();
            txtemail.Text = dt.Rows[0]["PrimaryEmailID"].ToString();
            txtPaymentTerm.Text = dt.Rows[0]["PaymentTerm"].ToString();
            lblst.Text = dt.Rows[0]["StateCode"].ToString();


        }
    }

    protected void txtinvoiceagainst_TextChanged(object sender, EventArgs e)
    {
        if (txtinvoiceagainst.Text == "Direct")
        {
            txtagainstNumber.Enabled = false;
            TableDirect.Visible = true;
            TableOrder.Visible = false;
        }
        else if (txtinvoiceagainst.Text == "Order")
        {
            TableDirect.Visible = false;
            TableOrder.Visible = true;
            txtagainstNumber.Enabled = true;
            Fillddlagainstnumber();


        }
        else
        {
            txtagainstNumber.SelectedValue = "0";
        }
    }

    protected void txtagainstNumber_TextChanged(object sender, EventArgs e)
    {
        if (txtagainstNumber.SelectedItem.Text != "--Select--" && txtagainstNumber.SelectedItem.Text != "")
        {
            Dt_Product.Columns.AddRange(new DataColumn[18] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("Batchno"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
            ViewState["PurchaseOrderProduct"] = Dt_Product;
            divTotalPart.Visible = true;

            SqlDataAdapter Da = new SqlDataAdapter("SELECT *, '' AS Batchno FROM [tbl_CustomerPurchaseOrderDtls] WHERE Pono='" + txtagainstNumber.SelectedItem.Text + "'", Cls_Main.Conn);
            DataTable DTCOMP = new DataTable();
            Da.Fill(DTCOMP);

            int count = 0;

            if (DTCOMP.Rows.Count > 0)
            {
                if (Dt_Product.Columns.Count < 0)
                {
                    Show_Grid();
                }

                for (int i = 0; i < DTCOMP.Rows.Count; i++)
                {
                    Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["Batchno"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
                    count = count + 1;
                }
            }
            ViewState["TaxInvoiceDetails"] = Dt_Product;
            dgvTaxinvoiceDetails.EmptyDataText = "No Data Found";
            dgvTaxinvoiceDetails.DataSource = Dt_Product;
            dgvTaxinvoiceDetails.DataBind();

        }
        SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] WHERE Pono='" + txtagainstNumber.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            TableOrder.Visible = true;
            txtcustomerPoNo.Enabled = false;
            txtcustomerPoNo.Text = dt.Rows[0]["SerialNo"].ToString();
        }
    }

    protected void txttransportMode_TextChanged(object sender, EventArgs e)
    {

        if (txttransportMode.Text == "By Road" || txttransportMode.Text == "By Courier")
        {
            divtransportdetails.Visible = true;
            txtvehicalNumber.Visible = true;
            txtByAir.Visible = false;
            txtByHand.Visible = false;
        }
        else if (txttransportMode.Text == "By Hand")
        {
            divtransportdetails.Visible = true;
            txtvehicalNumber.Visible = false;
            txtByAir.Visible = false;
            txtByHand.Visible = true;
        }
        else if (txttransportMode.Text == "By Air")
        {
            divtransportdetails.Visible = true;
            txtvehicalNumber.Visible = false;
            txtByAir.Visible = true;
            txtByHand.Visible = false;
        }
        else
        {
            divtransportdetails.Visible = false;
            txtvehicalNumber.Visible = false;
            txtByAir.Visible = false;
            txtByHand.Visible = false;
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProformaInvoiceList.aspx");
    }

    private void Fillddlshippingaddress(string ID)
    {
        try
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT SA.ShippingAddress FROM tbl_ShippingAddress  AS SA INNER JOIN tbl_CompanyMaster AS CM ON CM.ID=SA.c_id where Companyname='" + ID + "'", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddlShippingaddress.DataSource = dt;
                ddlShippingaddress.DataValueField = "ShippingAddress";
                ddlShippingaddress.DataTextField = "ShippingAddress";
                ddlShippingaddress.DataBind();
                ddlShippingaddress.Items.Insert(0, "-Select Shipping Address-");
            }
            else
            {
                ddlShippingaddress.DataSource = null;
                ddlShippingaddress.DataBind();
                ddlShippingaddress.Items.Insert(0, "-Select Shipping Address-");
            }


            SqlDataAdapter ad1 = new SqlDataAdapter("SELECT SA.BillAddress FROM tbl_BillingAddress  AS SA INNER JOIN tbl_CompanyMaster AS CM ON CM.ID=SA.c_id where Companyname='" + ID + "'", Cls_Main.Conn);
            DataTable dt1 = new DataTable();
            ad1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                ddlBillAddress.DataSource = dt1;
                ddlBillAddress.DataValueField = "BillAddress";
                ddlBillAddress.DataTextField = "BillAddress";
                ddlBillAddress.DataBind();
                ddlBillAddress.Items.Insert(0, "-Select Billing Address-");
            }
            else
            {
                ddlBillAddress.DataSource = null;
                ddlBillAddress.DataBind();
                ddlBillAddress.Items.Insert(0, "-Select Billing Address-");
            }
        }
        catch { }
    }

    protected void ddlShippingaddress_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM tbl_CompanyMaster AS CM INNER JOIN tbl_ShippingAddress AS SA ON CM.ID=SA.c_id WHERE CM.Companyname='" + txtshippingcustomer.Text.Trim() + "' AND  SA.ShippingAddress='" + ddlShippingaddress.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtshippinglocation.Text = dt.Rows[0]["ShipLocation"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["ShipPincode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["ShipStatecode"].ToString();
            txtshippingGST.Text = dt.Rows[0]["GSTNo1"].ToString();

        }
    }

    protected void txtrate_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());
        txttotal.Text = Convert.ToString(TotalAmt);
        decimal total;
        decimal Percentage;
        if (txtIGST.Text == null || txtIGST.Text == "" || txtIGST.Text == "0")
        {
            Percentage = Convert.ToDecimal(txtCGST.Text);
            total = (TotalAmt * Percentage / 100);

            txtCGSTamt.Text = total.ToString();

            txtSGSTamt.Text = txtCGSTamt.Text;

            txtSGST.Text = txtCGST.Text;
            var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
            txtgrandtotal.Text = GrandTotal.ToString();
        }
        else
        {
            Percentage = Convert.ToDecimal(txtIGST.Text);
            total = (TotalAmt * Percentage / 100);

            txtIGSTamt.Text = total.ToString();
            var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtIGSTamt.Text.Trim());
            txtgrandtotal.Text = GrandTotal.ToString();
        }
    }
    //add 07/07/2024
    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox1.Checked == true)
        {
            CompanyContactUsers.Visible = true;
            SqlDataAdapter ad = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM INNER JOIN tbl_CompanyContactDetails AS CCD ON CCD.CompanyCode=CM.CompanyCode where Companyname='" + txtbillingcustomer.Text + "' ", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                gvUserMail.DataSource = dt;
                gvUserMail.DataBind();

            }
        }
    }
    protected void chkHeader_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvUserMail.Rows)
        {
            CheckBox ChkHeader = (CheckBox)gvUserMail.HeaderRow.FindControl("chkHeader");
            CheckBox ChkRow = (CheckBox)row.FindControl("chkSelect");
            if (ChkHeader.Checked == true)
            {
                ChkRow.Checked = true;
            }
            else
            {
                ChkRow.Checked = false;
            }
        }
    }

    string mailTo = string.Empty;
    byte[] bytefile;
    private void mailsendforCustomerContact()
    {
        try
        {
            Report(txtInvoiceno.Text);
            foreach (GridViewRow g2 in gvUserMail.Rows)
            {
                CheckBox chkBox = (CheckBox)g2.FindControl("chkSelect");
                if (chkBox != null && chkBox.Checked)
                {
                    string strMessage = "Hello Sir/Ma'am " + txtbillingcustomer.Text.Trim() + "<br/>" +
             "We wish to thank you for your valued inquiry towards " + "<strong>Pune Abrasives Pvt. Ltd.....!!!<strong>" + "<br/>" +
                "We sent you an Proforma Invoice." + "Proforma Invoice - " + txtInvoiceno.Text.Trim() + "/" + txtinvoiceDate.Text.Trim() + ".pdf" + "<br/>" +

                "Please find herewith attached best offer for your reference." + "<br/>" +

                "Hope this offer is in line with your requirements." + "<br/>" +

                //"Looking forward to your valuable reply with Quotation." + "<br/>" +

                "Feel free to contact us for any further queries & Clarifications." + "<br/>" +

                "Kind Regards," + "<br/>" +
                "<strong>Pune Abrasives Pvt. Ltd.<strong>";
                    string fileName = txtInvoiceno.Text + "-" + "ProformaInvoice.pdf";
                    string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
                    string mailTo = ((Label)g2.FindControl("lblEmailID")).Text.Trim();
                    MailMessage mm = new MailMessage();
                    // mm.From = new MailAddress(fromMailID);

                    mm.Subject = txtbillingcustomer.Text + "_ProformaInvoice.pdf";
                    mm.To.Add(mailTo);
                    // mm.To.Add("shubhpawar59@gmail.com");

                    mm.CC.Add("girish.kulkarni@puneabrasives.com");
                    //mm.CC.Add("b.tikhe@puneabrasives.com");
                    mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
                    if (bytePdf != null)
                    {
                        Stream stream = new MemoryStream(bytePdf);
                        Attachment aa = new Attachment(stream, fileName);
                        mm.Attachments.Add(aa);
                    }
                    StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
                    string readFile = reader.ReadToEnd();
                    string myString = "";
                    myString = readFile;

                    string multilineText = strMessage;
                    string formattedText = multilineText.Replace("\n", "<br />");

                    myString = myString.Replace("$Comment$", formattedText);

                    mm.Body = myString.ToString();
                    mm.IsBodyHtml = true;
                    //mm.From = new MailAddress("girish.kulkarni@puneabrasives.com", fromMailID);
                    mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);

                    // Set the "Reply-To" header to indicate the desired display address
                    mm.ReplyToList.Add(new MailAddress(fromMailID));

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["Host"]; ;
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
                    NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToString();

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };
                    smtp.Send(mm);
                }
            }

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }

    }

    public void Report(string Invoiceno)
    {
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetProformaInvoiceDetails");
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
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\ProformaInvoice.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    bytePdf = null;
                    bytePdf = bytePdfRep;
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

    protected void ddlBillAddress_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBillAddress.SelectedItem.Text != "-Select Billing Address-")
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM tbl_BillingAddress  AS SA where BillAddress='" + ddlBillAddress.SelectedItem.Text + "'", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtbillinglocation.Text = dt.Rows[0]["BillLocation"].ToString();
                txtbillingPincode.Text = dt.Rows[0]["BillPincode"].ToString();
                txtbillingstatecode.Text = dt.Rows[0]["Billstatecode"].ToString();
                txtbillingGST.Text = dt.Rows[0]["GSTNo"].ToString();
            }
        }
    }
}


