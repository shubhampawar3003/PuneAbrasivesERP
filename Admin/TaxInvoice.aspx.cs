
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_TaxInvoice : System.Web.UI.Page
{
    DataTable Dt_Product = new DataTable();
    CommonCls objcls = new CommonCls();
    DataTable Dt_Component = new DataTable();
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
                FillddlProduct(); TaxInvoiceNo();

                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                }

                txtinvoiceDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                txtpodate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                //txt.Text = DateTime.Now.ToString("yyyy-MM-dd");

                ViewState["RowNo"] = 0;
                Dt_Product.Columns.AddRange(new DataColumn[17] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
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
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([ID]) as maxid FROM [tbl_TaxInvoiceHdr]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            // txtInvoiceno.Text = "WLSPL/TI-" + (maxid + 1).ToString();
            txtInvoiceno.Text = (maxid + 1).ToString("D4") + "/" + currentYear.ToString() + "-" + nextYear.ToString().Substring(2);
        }
        else
        {
            txtInvoiceno.Text = string.Empty;
        }
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
            txtagainstNumber.Items.Insert(0, " ---  Select Number  --- ");
        }
    }

    //Bind Product
    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            //ddlProduct.DataSource = dt;
            //ddlProduct.DataTextField = "ProductName";
            //ddlProduct.DataBind();
            //ddlProduct.Items.Insert(0, "-- Select Product --");
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
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_TaxInvoiceHdr] WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            txtbillingcustomer.Text = Dt.Rows[0]["BillingCustomer"].ToString();
            txtshippingcustomer.Text = Dt.Rows[0]["ShippingCustomer"].ToString();
            txtbillingaddress.Text = Dt.Rows[0]["BillingAddress"].ToString();
            txtshippingAddress.Text = Dt.Rows[0]["ShippingAddress"].ToString();
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
            txtpaymentterm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
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
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_TaxInvoiceDtls] WHERE Invoiceno='" + txtInvoiceno.Text + "'", Cls_Main.Conn);
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
                Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
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
        Dt.Rows.Add(ViewState["RowNo"], txtPartname.Text, txtdescription.Text.Trim(), txthsnsac.Text.Trim(), txtquantity.Text, txtunit.Text, txtrate.Text, txttotal.Text, txtCGST.Text, txtCGSTamt.Text, txtSGST.Text, txtSGSTamt.Text, txtIGST.Text, txtIGSTamt.Text, txtdiscount.Text, txtdiscountamt.Text, txtgrandtotal.Text);
        ViewState["TaxInvoiceDetails"] = Dt;
        FillddlProduct();
        txtdescription.Text = string.Empty;
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

    protected void SaveRecord()
    {
        try
        {
            if (txtbillingcustomer.Text == "" || txtshippingcustomer.Text == "" || txtbillingstatecode.Text == "" || txtbillingPincode.Text == "" || txtpanno.Text == "" || txtpaymentterm.Text == "" || txtbillingGST.Text == "")
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
                            SqlCommand Cmd = new SqlCommand("SP_TaxInvoiceHdr", Cls_Main.Conn);
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.Parameters.AddWithValue("@Action", "Save");
                            Cmd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                            Cmd.Parameters.AddWithValue("@InvoiceType", ddlInvoiceType.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoiceDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingcustomer", txtbillingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingcustomer", txtshippingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingaddress", txtbillingaddress.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingAddress", txtshippingAddress.Text.Trim());
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
                            Cmd.Parameters.AddWithValue("@invoiceagainst", txtinvoiceagainst.SelectedItem.Text);
                            if(txtagainstNumber.SelectedItem.Text!= " ---  Select Number  --- ")
                            {
                                Cmd.Parameters.AddWithValue("@againstNumber", txtagainstNumber.SelectedItem.Text);
                            }
                            else
                            {
                                Cmd.Parameters.AddWithValue("@againstNumber", null);
                            }
                           
                            Cmd.Parameters.AddWithValue("@customerPoNo", txtcustomerPoNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@podate", txtpodate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanNo", txtchallanNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanDate", txtchallanDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@transportMode", txttransportMode.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@AmountInWords", ConvertToWords(txt_grandTotal.Text));
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
                            Cmd.Parameters.AddWithValue("@Paymentterm", txtpaymentterm.Text.Trim());
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

                                Cls_Main.Conn_Open();
                                SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_TaxInvoiceDtls] (Invoiceno,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn) VALUES(@Invoiceno,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn)", Cls_Main.Conn);
                                cmdd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                                cmdd.Parameters.AddWithValue("@Productname", lblproduct);
                                cmdd.Parameters.AddWithValue("@Description", lblDescription);
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
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Tax Invoice Save Successfully..!!');window.location='CustomerTaxInvoiceList.aspx'; ", true);
                            //Save Contact Details End
                        }
                        else if (btnsave.Text == "Update")
                        {
                            Cls_Main.Conn_Open();
                            SqlCommand Cmd = new SqlCommand("SP_TaxInvoiceHdr", Cls_Main.Conn);
                            Cmd.CommandType = CommandType.StoredProcedure;
                            Cmd.Parameters.AddWithValue("@Action", "Update");
                            Cmd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                            Cmd.Parameters.AddWithValue("@InvoiceType", ddlInvoiceType.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoiceDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingcustomer", txtbillingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingcustomer", txtshippingcustomer.Text.Trim());
                            Cmd.Parameters.AddWithValue("@billingaddress", txtbillingaddress.Text.Trim());
                            Cmd.Parameters.AddWithValue("@shippingAddress", txtshippingAddress.Text.Trim());
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
                            if (txtagainstNumber.SelectedItem.Text != " ---  Select Number  --- ")
                            {
                                Cmd.Parameters.AddWithValue("@againstNumber", txtagainstNumber.SelectedItem.Text);
                            }
                            else
                            {
                                Cmd.Parameters.AddWithValue("@againstNumber", null);
                            }
                            Cmd.Parameters.AddWithValue("@customerPoNo", txtcustomerPoNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@podate", txtpodate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanNo", txtchallanNo.Text.Trim());
                            Cmd.Parameters.AddWithValue("@challanDate", txtchallanDate.Text.Trim());
                            Cmd.Parameters.AddWithValue("@transportMode", txttransportMode.SelectedItem.Text.Trim());
                            Cmd.Parameters.AddWithValue("@AmountInWords", ConvertToWords(txt_grandTotal.Text));
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
                            Cmd.Parameters.AddWithValue("@Paymentterm", txtpaymentterm.Text.Trim());
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
                            SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_TaxInvoiceDtls WHERE Invoiceno=@Invoiceno", Cls_Main.Conn);
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

                                Cls_Main.Conn_Open();
                                SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_TaxInvoiceDtls] (Invoiceno,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn) VALUES(@Invoiceno,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn)", Cls_Main.Conn);
                                cmdd.Parameters.AddWithValue("@Invoiceno", txtInvoiceno.Text);
                                cmdd.Parameters.AddWithValue("@Productname", lblproduct);
                                cmdd.Parameters.AddWithValue("@Description", lblDescription);
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
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('TaxInvoice Updated Successfully..!!');window.location='CustomerTaxInvoiceList.aspx'; ", true);
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
        Response.Redirect("CustomerTaxInvoiceList.aspx");
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
        DataTable Dt = ViewState["TaxInvoiceDetails"] as DataTable;
        Dt.Rows[row.RowIndex]["Productname"] = Product;
        Dt.Rows[row.RowIndex]["Description"] = Description;
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
       // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
       // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void dgvContactDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvTaxinvoiceDetails.EditIndex = e.NewEditIndex;
        dgvTaxinvoiceDetails.DataSource = (DataTable)ViewState["TaxInvoiceDetails"];
        dgvTaxinvoiceDetails.DataBind();
      //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

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
        DataTable Dt = ViewState["TaxInvoiceDetails"] as DataTable;
        Dt.Rows[row.RowIndex]["Productname"] = Product;
        Dt.Rows[row.RowIndex]["Description"] = Description;
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
      //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
     //   ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void txtquantity_TextChanged(object sender, EventArgs e)
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

    public string ConvertNumbertoWords(int number)
    {
        if (number == 0)
            return "ZERO";
        if (number < 0)
            return "minus " + ConvertNumbertoWords(Math.Abs(number));
        string words = "";
        if ((number / 1000000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000000) + " Million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000) + " Thousand ";
            number %= 1000;
        }
        if ((number / 100) > 0)
        {
            words += ConvertNumbertoWords(number / 100) + " Hundred ";
            number %= 100;
        }
        if (number > 0)
        {
            if (words != "")
                words += "And ";
            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

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
            val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
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


    protected void txtshippingcustomer_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode WHERE Companyname='" + txtshippingcustomer.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtbillingaddress.Text = dt.Rows[0]["Billingaddress"].ToString();
            txtshippingAddress.Text = dt.Rows[0]["Shippingaddress"].ToString();
            txtbillinglocation.Text = dt.Rows[0]["Billinglocation"].ToString();
            txtshippinglocation.Text = dt.Rows[0]["Shippinglocation"].ToString();
            txtbillingPincode.Text = dt.Rows[0]["Billingpincode"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["Shippingpincode"].ToString();
            txtbillingstatecode.Text = dt.Rows[0]["StateCode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["StateCode"].ToString();
            txtbillingcustomer.Text = dt.Rows[0]["Companyname"].ToString();
            txtpanno.Text = dt.Rows[0]["Companypancard"].ToString();
            txtbillingGST.Text = dt.Rows[0]["GSTno"].ToString();
            txtshippingGST.Text = dt.Rows[0]["GSTno"].ToString();
            txtContactNo.Text = dt.Rows[0]["Number"].ToString();
            txtemail.Text = dt.Rows[0]["EmailID"].ToString();
            lblst.Text = dt.Rows[0]["StateCode"].ToString();
            txtpaymentterm.Text = dt.Rows[0]["PaymentTerm"].ToString();
            if (txtbillingaddress.Text == "" || txtbillinglocation.Text == "" || txtbillingPincode.Text == "" || txtbillingstatecode.Text == "" || txtbillingGST.Text == "" || txtContactNo.Text == "" || txtemail.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data in Commpany Master..!!')", true);
            }


        }
    }

    protected void txtbillingcustomer_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode WHERE Companyname='" + txtbillingcustomer.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtbillingaddress.Text = dt.Rows[0]["Billingaddress"].ToString();
            txtshippingAddress.Text = dt.Rows[0]["Shippingaddress"].ToString();
            txtbillinglocation.Text = dt.Rows[0]["Billinglocation"].ToString();
            txtshippinglocation.Text = dt.Rows[0]["Shippinglocation"].ToString();
            txtbillingPincode.Text = dt.Rows[0]["Billingpincode"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["Shippingpincode"].ToString();
            txtbillingstatecode.Text = dt.Rows[0]["StateCode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["StateCode"].ToString();
            txtshippingcustomer.Text = dt.Rows[0]["Companyname"].ToString();
            txtpanno.Text = dt.Rows[0]["Companypancard"].ToString();
            txtbillingGST.Text = dt.Rows[0]["GSTno"].ToString();
            txtshippingGST.Text = dt.Rows[0]["GSTno"].ToString();
            txtContactNo.Text = dt.Rows[0]["Number"].ToString();
            txtemail.Text = dt.Rows[0]["EmailID"].ToString();
            lblst.Text = dt.Rows[0]["StateCode"].ToString();
            txtpaymentterm.Text = dt.Rows[0]["PaymentTerm"].ToString();
            if (txtbillingaddress.Text == "" || txtbillinglocation.Text == "" || txtbillingPincode.Text == "" || txtbillingstatecode.Text == "" || txtbillingGST.Text == "" || txtContactNo.Text == "" || txtemail.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data in Commpany Master..!!')", true);
            }


        }
    }

    protected void txtinvoiceagainst_TextChanged(object sender, EventArgs e)
    {
      
        if (txtinvoiceagainst.Text == "Direct")
        {
            Fillddlagainstnumber();
            // txtagainstNumber.SelectedItem.Text = " ---  Select Number  --- ";
            txtagainstNumber.Enabled = false;
            TableDirect.Visible = true;
            TableOrder.Visible = false;
            ViewState["RowNo"] = 0;
            Dt_Product.Columns.AddRange(new DataColumn[17] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
            ViewState["TaxInvoiceDetails"] = Dt_Product;
            dgvTaxinvoiceDetails.DataSource = null;
            dgvTaxinvoiceDetails.DataBind();
        }
        else if (txtinvoiceagainst.Text == "Order")
        {
            txtagainstNumber.Items.Clear();
            Fillddlagainstnumber();
            TableDirect.Visible = false;
            TableOrder.Visible = true;
            txtagainstNumber.Enabled = true;
           
        }
        else
        {
            txtagainstNumber.SelectedValue = "0";
        }


    }

    protected void txtagainstNumber_TextChanged(object sender, EventArgs e)
    {
        if (txtagainstNumber.SelectedItem.Text != " ---  Select Number  --- " && txtagainstNumber.SelectedItem.Text != "")
        {
            Dt_Product.Columns.AddRange(new DataColumn[17] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
            ViewState["PurchaseOrderProduct"] = Dt_Product;
            divTotalPart.Visible = true;

            SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderDtls] WHERE Pono='" + txtagainstNumber.SelectedItem.Text + "'", Cls_Main.Conn);
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
                    Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
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

    //Search Part name methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetPartList(string prefixText, int count)
    {
        return AutoFillPartName(prefixText);
    }

    public static List<string> AutoFillPartName(string prefixText)
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

    protected void txtPartname_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ProductMaster] WHERE ProductName='" + txtPartname.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            txtdescription.Text = Dt.Rows[0]["Description"].ToString();
            txthsnsac.Text = Dt.Rows[0]["HSN"].ToString();
            txtrate.Text = Dt.Rows[0]["Price"].ToString();
            txtunit.Text = Dt.Rows[0]["Unit"].ToString();
            if (lblst.Text == "27")
            {
                txtCGST.Text = "9";
                txtSGST.Text = "9";
                txtIGST.Text = "0";
            }
            else
            {
                txtIGST.Text = "18";
                txtCGST.Text = "0";
                txtSGST.Text = "0";
            }
            txtCGSTamt.Text = "0.00";
            txtSGSTamt.Text = "0.00";
            txtIGSTamt.Text = "0.00";
            txtdiscount.Text = "0.00";
            txtdiscountamt.Text = "0.00";
            //  ddlProduct.Focus();
        }
    }
}


