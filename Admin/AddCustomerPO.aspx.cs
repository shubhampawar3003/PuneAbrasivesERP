
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_AddCustomerPO : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    DataTable Dt_Product = new DataTable();
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
                FillddlProduct(); POCode(); FillddlUsers();
                txtshippingGST.Enabled = false;
                if (Request.QueryString["OAID"] != null)
                {
                    txtcompanyname.Text = objcls.Decrypt(Request.QueryString["OAID"].ToString());
                    GetCompanyDetails();
                }
                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                }

                if (Session["UserCode"] == null)
                {
                    Response.Redirect("../Login.aspx");
                }
                else
                {
                    txtpodate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    txtdeliverydate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                }

                ViewState["RowNo"] = 0;

                Dt_Product.Columns.AddRange(new DataColumn[20] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal"), new DataColumn("Orderquantity"), new DataColumn("ShippingQuantity"), new DataColumn("Balance") });
                ViewState["PurchaseOrderProduct"] = Dt_Product;

                //Edit 
                if (Request.QueryString["ID"] != null)
                {
                    ID = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    btnsave.Text = "Update";
                    ShowDtlEdit();
                    hhd.Value = ID;

                }
            }
        }

    }
    protected void POCode()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max(CAST(SUBSTRING(Pono, CHARINDEX('-', Pono) + 1, LEN(Pono)) AS INT)) as maxid FROM [tbl_CustomerPurchaseOrderHdr]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"]);
            txtpono.Text = "PAPL/OA-" + (maxid + 1).ToString();
        }
        else
        {
            txtpono.Text = "PAPL/OA-1";
        }
    }
    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where CP.IsDeleted=0 AND CP.ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";

            txtcompanyname.Text = Dt.Rows[0]["CustomerName"].ToString();
            txtpaymentterm.Text = Dt.Rows[0]["paymentterm"].ToString();
            txtpono.Text = Dt.Rows[0]["Pono"].ToString();
            txtserialno.Text = Dt.Rows[0]["SerialNo"].ToString();
            FillKittens();
            lblfile1.Text = Dt.Rows[0]["fileName"].ToString();

            ddlContacts.SelectedItem.Text = Dt.Rows[0]["KindAtt"].ToString();

            Fillddlshippingaddress(txtcompanyname.Text);
            Fillddlbillingaddress(txtcompanyname.Text);
            ddlUser.SelectedValue = Dt.Rows[0]["UserCode"].ToString();
            DateTime ffff2 = Convert.ToDateTime(Dt.Rows[0]["PoDate"].ToString());
            txtmobileno.Text = Dt.Rows[0]["Mobileno"].ToString();
            txtbillingGST.Text = Dt.Rows[0]["GSTNo"].ToString();
            txtpanno.Text = Dt.Rows[0]["PANNo"].ToString();
            txtemail.Text = Dt.Rows[0]["EmailID"].ToString();
            ddlBillAddress.SelectedItem.Text = Dt.Rows[0]["BillingAddress"].ToString();
            txtshortBillingaddress.Text = Dt.Rows[0]["ShortBAddress"].ToString();
            txtshortShippingaddress.Text = Dt.Rows[0]["ShortSAddress"].ToString();

            ddlShippingaddress.SelectedItem.Text = Dt.Rows[0]["ShippingAddress"].ToString();
            ViewState["Address"] = null;
            ViewState["Address"] = Dt.Rows[0]["ShippingAddress"].ToString();
            txtbillingGST.Text = Dt.Rows[0]["BillingGST"].ToString();
            txtshippingGST.Text = Dt.Rows[0]["ShippingGST"].ToString();
            txtbillinglocation.Text = Dt.Rows[0]["BillingLocation"].ToString();
            txtshippinglocation.Text = Dt.Rows[0]["ShippingLocation"].ToString();
            txtbillingPincode.Text = Dt.Rows[0]["BillingPincode"].ToString();
            txtshippingPincode.Text = Dt.Rows[0]["ShippingPincode"].ToString();
            txtbillingstatecode.Text = Dt.Rows[0]["BillingStatecode"].ToString();
            txtshippingstatecode.Text = Dt.Rows[0]["ShippingStatecode"].ToString();

            txtPayment.Text = Dt.Rows[0]["Payment"].ToString();
            txtTransport.Text = Dt.Rows[0]["Transport"].ToString();
            txtDeliveryTime.Text = Dt.Rows[0]["DeliveryTime"].ToString();
            txtPacking.Text = Dt.Rows[0]["Packing"].ToString();
            txtTaxs.Text = Dt.Rows[0]["Taxs"].ToString();

            DateTime ffff3 = Convert.ToDateTime(Dt.Rows[0]["Deliverydate"].ToString());
            txtreferquotation.Text = Dt.Rows[0]["Referquotation"].ToString();
            txtremark.Text = Dt.Rows[0]["Remarks"].ToString();
            txtTermsofdelivery.Text = Dt.Rows[0]["Termofdelivery"].ToString();
            //txtagainstNumber.SelectedItem.Text = Dt.Rows[0]["AgainstNumber"].ToString();
            Fillddlagainstnumber();
            txtinvoiceagainst.SelectedItem.Text = Dt.Rows[0]["InvoiceAgainst"].ToString();
            if (txtinvoiceagainst.SelectedItem.Text == "Direct")
            {

                idheader.Visible = true;
                idproducttable.Visible = true;
                txtagainstNumber.Enabled = false;
            }
            else
            {
                txtagainstNumber.SelectedItem.Text = Dt.Rows[0]["AgainstNumber"].ToString();
                idheader.Visible = false;
                idproducttable.Visible = false;
                txtagainstNumber.Enabled = true;
            }


        }
    }

    protected void ShowDtlEdit()
    {
        divTotalPart.Visible = true;

        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderDtls] WHERE Pono='" + txtpono.Text + "'", Cls_Main.Conn);
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
                Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString(), DTCOMP.Rows[i]["Orderquantity"].ToString(), DTCOMP.Rows[i]["ShippingQuantity"].ToString(), DTCOMP.Rows[i]["Balance"].ToString());
                count = count + 1;
            }
        }

        dgvMachineDetails.EmptyDataText = "No Data Found";
        dgvMachineDetails.DataSource = Dt_Product;
        dgvMachineDetails.DataBind();
    }

    private void Show_Grid()
    {
        divTotalPart.Visible = true;
        DataTable Dt = (DataTable)ViewState["PurchaseOrderProduct"];
        Dt.Rows.Add(ViewState["RowNo"], ddlProduct.SelectedItem.Text, txtdescription.Text.Trim(), txthsnsac.Text.Trim(), txtquantity.Text, txtunit.Text, txtrate.Text, txttotal.Text, txtCGST.Text, txtCGSTamt.Text, txtSGST.Text, txtSGSTamt.Text, txtIGST.Text, txtIGSTamt.Text, txtdiscount.Text, txtdiscountamt.Text, txtgrandtotal.Text, txtorderq.Text, txtshippingq.Text, txtbalance.Text);
        ViewState["PurchaseOrderProduct"] = Dt;
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
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
    }

    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster] WHERE status=1 AND isdeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlProduct.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            ddlProduct.DataTextField = "ProductName";
            ddlProduct.DataBind();
            ddlProduct.Items.Insert(0, "-- Select Product --");
        }
    }
    private void FillddlUsers()
    {
        SqlDataAdapter ad = new SqlDataAdapter("select Username,UserCode from tbl_UserMaster where (Designation='Sales Manager' OR Designation='M.D') and Status=1 and IsDeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlUser.DataSource = dt;
            ddlUser.DataValueField = "UserCode";
            ddlUser.DataTextField = "Username";
            ddlUser.DataBind();
            ddlUser.Items.Insert(0, "-- Select User Name--");
        }
    }
    private void FillKittens()
    {
        SqlDataAdapter ad = new SqlDataAdapter("select CCD.ID,CCD.Name from tbl_CompanyMaster AS CM Inner JOIN tbl_CompanyContactDetails AS CCD ON CCD.CompanyCode=CM.CompanyCode where Companyname='" + txtcompanyname.Text + "' ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlContacts.DataSource = dt;
            ddlContacts.DataValueField = "ID";
            ddlContacts.DataTextField = "Name";
            ddlContacts.DataBind();
            ddlContacts.Items.Insert(0, "-- Select Kindd Att --");
        }
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


        }
        catch { }
    }

    private void Fillddlbillingaddress(string ID)
    {
        try
        {

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

    [ScriptMethod()]
    [WebMethod]
    public static List<string> GetCompanyList(string prefixText, int count)
    {
        return AutoFillCompanyName(prefixText);
    }

    public static List<string> AutoFillCompanyName(string prefixText)
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

    protected async void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtcompanyname.Text == " --- Select Company --- " || ddlUser.SelectedItem.Text == "-- Select User Name--" || ddlContacts.SelectedItem.Text == "-- Select Kindd Att --" || ddlContacts.SelectedItem.Text == "" || txtpodate.Text == "" || txtdeliverydate.Text == "" || txtserialno.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                if (dgvMachineDetails.Rows.Count > 0)
                {
                    if (btnsave.Text == "Save")
                    {

                        Cls_Main.Conn_Open();
                        SqlCommand cmd = new SqlCommand("SP_CustomerPurchaseOrderHdr", Cls_Main.Conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerName", txtcompanyname.Text);
                        cmd.Parameters.AddWithValue("@Pono", txtpono.Text);
                        cmd.Parameters.AddWithValue("@SerialNo", txtserialno.Text);
                        cmd.Parameters.AddWithValue("@KindAtt", ddlContacts.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@PoDate", txtpodate.Text);
                        cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text);
                        cmd.Parameters.AddWithValue("@EmailID", txtemail.Text);
                        cmd.Parameters.AddWithValue("@ShortBAddress", txtshortBillingaddress.Text);
                        cmd.Parameters.AddWithValue("@ShortSAddress", txtshortShippingaddress.Text);

                        //NewDetails For E-Invoice                      
                        cmd.Parameters.AddWithValue("@BillingLocation", txtbillinglocation.Text);
                        cmd.Parameters.AddWithValue("@ShippingLocation", txtshippinglocation.Text);
                        cmd.Parameters.AddWithValue("@BillingGST", txtbillingGST.Text);
                        cmd.Parameters.AddWithValue("@ShippingGST", txtshippingGST.Text);
                        cmd.Parameters.AddWithValue("@BillingPincode", txtbillingPincode.Text);
                        cmd.Parameters.AddWithValue("@ShippingPincode", txtshippingPincode.Text);
                        cmd.Parameters.AddWithValue("@BillingStatecode", txtbillingstatecode.Text);
                        cmd.Parameters.AddWithValue("@ShippingStatecode", txtshippingstatecode.Text);

                        cmd.Parameters.AddWithValue("@GSTNo", txtbillingGST.Text);
                        cmd.Parameters.AddWithValue("@PANNo", txtpanno.Text);

                        if (ddlUser.SelectedValue == "-- Select User Name--")
                        {
                            cmd.Parameters.AddWithValue("@UserName", Session["UserCode"].ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@UserName", ddlUser.SelectedValue);
                        }
                        if (ViewState["attachment"] != null)
                        {
                            byte[] fileContent = (byte[])ViewState["attachment"];
                            cmd.Parameters.AddWithValue("@fileName", lblfile1.Text);
                            string[] pdffilename = lblfile1.Text.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];

                            string filePath = Server.MapPath("~/PDF_Files/") + pdffilename1 + "." + filenameExt;

                            // Save the file to the specified path
                            System.IO.File.WriteAllBytes(filePath, fileContent);

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@fileName", DBNull.Value);
                        }
                        cmd.Parameters.AddWithValue("@BillingAddress", ddlBillAddress.SelectedItem.Text);

                        cmd.Parameters.AddWithValue("@ShippingAddress", ViewState["Address"].ToString());

                        cmd.Parameters.AddWithValue("@Deliverydate", txtdeliverydate.Text);
                        cmd.Parameters.AddWithValue("@Referquotation", txtreferquotation.Text);
                        cmd.Parameters.AddWithValue("@Remarks", txtremark.Text);
                        cmd.Parameters.AddWithValue("@CGST_Amt", txt_cgstamt.Text);
                        cmd.Parameters.AddWithValue("@SGST_Amt", txt_sgstamt.Text);
                        cmd.Parameters.AddWithValue("@IGST_Amt", txt_igstamt.Text);
                        cmd.Parameters.AddWithValue("@Total_Price", txt_grandTotal.Text);
                        cmd.Parameters.AddWithValue("@Paymentterm", txtpaymentterm.Text);
                        cmd.Parameters.AddWithValue("@Totalinword", lbl_total_amt_Value.Text);
                        cmd.Parameters.AddWithValue("@InvoiceAgainst", txtinvoiceagainst.SelectedItem.Text);
                        if (txtinvoiceagainst.SelectedItem.Text == "Direct")
                        {
                            cmd.Parameters.AddWithValue("@AgainstNumber", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@AgainstNumber", txtagainstNumber.SelectedItem.Text);
                        }

                        cmd.Parameters.AddWithValue("@Payment", txtPayment.Text);
                        cmd.Parameters.AddWithValue("@Transport", txtTransport.Text);
                        cmd.Parameters.AddWithValue("@DeliveryTime", txtDeliveryTime.Text);
                        cmd.Parameters.AddWithValue("@Packing", txtPacking.Text);
                        cmd.Parameters.AddWithValue("@Taxs", txtTaxs.Text);

                        cmd.Parameters.AddWithValue("@Termofdelivery", txtTermsofdelivery.Text);
                        cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Action", "Save");
                        cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();

                        //Save Product Details 
                        foreach (GridViewRow grd1 in dgvMachineDetails.Rows)
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
                            string OrderQ = (grd1.FindControl("lblOquantity") as Label).Text;
                            string ShippingQ = (grd1.FindControl("lblSQuantity") as Label).Text;
                            string txtAllBalance = (grd1.FindControl("lblbalance") as Label).Text;


                            Cls_Main.Conn_Open();
                            SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_CustomerPurchaseOrderDtls (Pono,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,Orderquantity,ShippingQuantity,Balance,CreatedOn) VALUES(@Pono,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@OrderQ,@ShippingQ,@txtAllBalance,@CreatedOn)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@Pono", txtpono.Text);
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
                            cmdd.Parameters.AddWithValue("@OrderQ", OrderQ);
                            cmdd.Parameters.AddWithValue("@ShippingQ", ShippingQ);
                            cmdd.Parameters.AddWithValue("@txtAllBalance", txtAllBalance);
                            cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                            cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                            cmdd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Order Acceptance Save Successfully..!!');window.location='CustomerPurchaseOrderList.aspx'; ", true);
                    }
                    else if (btnsave.Text == "Update")
                    {
                        DateTime Date = DateTime.Now;
                        Cls_Main.Conn_Open();

                        SqlCommand cmd = new SqlCommand("SP_CustomerPurchaseOrderHdr", Cls_Main.Conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerName", txtcompanyname.Text);
                        cmd.Parameters.AddWithValue("@Pono", txtpono.Text);
                        cmd.Parameters.AddWithValue("@SerialNo", txtserialno.Text);
                        cmd.Parameters.AddWithValue("@KindAtt", ddlContacts.SelectedItem.Text);
                        if (ddlUser.SelectedValue == "-- Select User Name--")
                        {
                            cmd.Parameters.AddWithValue("@UserName", Session["UserCode"].ToString());
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@UserName", ddlUser.SelectedValue);
                        }
                        cmd.Parameters.AddWithValue("@PoDate", txtpodate.Text);
                        cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text);
                        cmd.Parameters.AddWithValue("@EmailID", txtemail.Text);
                        cmd.Parameters.AddWithValue("@ShortBAddress", txtshortBillingaddress.Text);
                        cmd.Parameters.AddWithValue("@ShortSAddress", txtshortShippingaddress.Text);

                        //NewDetails For E-Invoice                      
                        cmd.Parameters.AddWithValue("@BillingLocation", txtbillinglocation.Text);
                        cmd.Parameters.AddWithValue("@ShippingLocation", txtshippinglocation.Text);
                        cmd.Parameters.AddWithValue("@BillingGST", txtbillingGST.Text);
                        cmd.Parameters.AddWithValue("@ShippingGST", txtshippingGST.Text);
                        cmd.Parameters.AddWithValue("@BillingPincode", txtbillingPincode.Text);
                        cmd.Parameters.AddWithValue("@ShippingPincode", txtshippingPincode.Text);
                        cmd.Parameters.AddWithValue("@BillingStatecode", txtbillingstatecode.Text);
                        cmd.Parameters.AddWithValue("@ShippingStatecode", txtshippingstatecode.Text);

                        cmd.Parameters.AddWithValue("@GSTNo", txtbillingGST.Text);
                        cmd.Parameters.AddWithValue("@PANNo", txtpanno.Text);
                        cmd.Parameters.AddWithValue("@BillingAddress", ddlBillAddress.SelectedItem.Text);
                        cmd.Parameters.AddWithValue("@ShippingAddress", ViewState["Address"].ToString());

                        cmd.Parameters.AddWithValue("@Deliverydate", txtdeliverydate.Text);
                        cmd.Parameters.AddWithValue("@Referquotation", txtreferquotation.Text);
                        cmd.Parameters.AddWithValue("@Remarks", txtremark.Text);
                        cmd.Parameters.AddWithValue("@CGST_Amt", txt_cgstamt.Text);
                        cmd.Parameters.AddWithValue("@SGST_Amt", txt_sgstamt.Text);
                        cmd.Parameters.AddWithValue("@IGST_Amt", txt_igstamt.Text);
                        cmd.Parameters.AddWithValue("@Total_Price", txt_grandTotal.Text);
                        cmd.Parameters.AddWithValue("@Totalinword", lbl_total_amt_Value.Text);
                        cmd.Parameters.AddWithValue("@Paymentterm", txtpaymentterm.Text);
                        cmd.Parameters.AddWithValue("@InvoiceAgainst", txtinvoiceagainst.SelectedItem.Text);
                        if (txtinvoiceagainst.SelectedItem.Text == "Direct")
                        {
                            cmd.Parameters.AddWithValue("@AgainstNumber", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@AgainstNumber", txtagainstNumber.SelectedItem.Text);
                        }
                        cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        if (ViewState["attachment"] != null)
                        {
                            byte[] fileContent = (byte[])ViewState["attachment"];
                            cmd.Parameters.AddWithValue("@fileName", lblfile1.Text);
                            string[] pdffilename = lblfile1.Text.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];

                            string filePath = Server.MapPath("~/PDF_Files/") + pdffilename1 + "." + filenameExt;

                            // Save the file to the specified path
                            System.IO.File.WriteAllBytes(filePath, fileContent);

                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@fileName", lblfile1.Text);
                        }
                        cmd.Parameters.AddWithValue("@Payment", txtPayment.Text);
                        cmd.Parameters.AddWithValue("@Transport", txtTransport.Text);
                        cmd.Parameters.AddWithValue("@DeliveryTime", txtDeliveryTime.Text);
                        cmd.Parameters.AddWithValue("@Packing", txtPacking.Text);
                        cmd.Parameters.AddWithValue("@Taxs", txtTaxs.Text);

                        cmd.Parameters.AddWithValue("@Termofdelivery", txtTermsofdelivery.Text);
                        cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                        cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Action", "Update");
                        cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();

                        //DELETE DETAILS DATA FOR UPDATE
                        Cls_Main.Conn_Open();
                        SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_CustomerPurchaseOrderDtls WHERE Pono=@Pono", Cls_Main.Conn);
                        cmddelete.Parameters.AddWithValue("@Pono", txtpono.Text);
                        cmddelete.ExecuteNonQuery();
                        Cls_Main.Conn_Close();

                        //Save Product Details 
                        foreach (GridViewRow grd1 in dgvMachineDetails.Rows)
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
                            string OrderQ = (grd1.FindControl("lblOquantity") as Label).Text;
                            string ShippingQ = (grd1.FindControl("lblSQuantity") as Label).Text;
                            string txtAllBalance = (grd1.FindControl("lblbalance") as Label).Text;


                            Cls_Main.Conn_Open();
                            SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_CustomerPurchaseOrderDtls (Pono,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,Orderquantity,ShippingQuantity,Balance,CreatedOn) VALUES(@Pono,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@OrderQ,@ShippingQ,@txtAllBalance,@CreatedOn)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@Pono", txtpono.Text);
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
                            cmdd.Parameters.AddWithValue("@OrderQ", OrderQ);
                            cmdd.Parameters.AddWithValue("@ShippingQ", ShippingQ);
                            cmdd.Parameters.AddWithValue("@txtAllBalance", txtAllBalance);
                            cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                            cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                            cmdd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Order Acceptance Update Successfully..!!');window.location='CustomerPurchaseOrderList.aspx'; ", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Product Data..!!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
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
            hhdstate.Value = stateCode;
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

    protected void txtquantity_TextChanged(object sender, EventArgs e)
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

    protected void dgvMachineDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvMachineDetails.EditIndex = e.NewEditIndex;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    string lblTotal, lblCGST, lblSGST;
    string lblproduct, lblproducttype, lblsubtype, lblproductbrand, lblton, Description, hsn, Quantity, Unit, Rate, subTotal, CGSTPer, CGST, SGSTPer, SGST, IGSTPer, IGST, Discount, lblDiscountAmount, Grandtotal;
    private decimal Total, CGSTAmt, SGSTAmt, IGSTAmt, Alltotal;
    protected void dgvMachineDetails_RowDataBound(object sender, GridViewRowEventArgs e)
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
                    lblDiscountAmount = (e.Row.FindControl("txtDiscountAmount") as TextBox).Text;
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
        string OrderQ = ((TextBox)row.FindControl("txtOquantity")).Text;
        string ShippingQ = ((TextBox)row.FindControl("txtSQuantity")).Text;
        string txtAllBalance = ((TextBox)row.FindControl("txtAllBalance")).Text;

        DataTable Dt = ViewState["PurchaseOrderProduct"] as DataTable;
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
        Dt.Rows[row.RowIndex]["Orderquantity"] = OrderQ;
        Dt.Rows[row.RowIndex]["Shippingquantity"] = ShippingQ;
        Dt.Rows[row.RowIndex]["Balance"] = txtAllBalance;
        Dt.AcceptChanges();
        ViewState["PurchaseOrderProduct"] = Dt;
        dgvMachineDetails.EditIndex = -1;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
        string OrderQ = ((TextBox)row.FindControl("txtOquantity")).Text;
        string ShippingQ = ((TextBox)row.FindControl("txtSQuantity")).Text;
        string txtAllBalance = ((TextBox)row.FindControl("txtAllBalance")).Text;
        DataTable Dt = ViewState["PurchaseOrderProduct"] as DataTable;
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
        Dt.Rows[row.RowIndex]["Orderquantity"] = OrderQ;
        Dt.Rows[row.RowIndex]["Shippingquantity"] = ShippingQ;
        Dt.Rows[row.RowIndex]["Balance"] = txtAllBalance;
        Dt.AcceptChanges();
        ViewState["PurchaseOrderProduct"] = Dt;
        dgvMachineDetails.EditIndex = -1;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["PurchaseOrderProduct"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["PurchaseOrderProduct"] = dt;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Order Acceptance Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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



    // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);



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

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerPurchaseOrderList.aspx");
    }

    protected void txtcompanyname_TextChanged(object sender, EventArgs e)
    {
        GetCompanyDetails();
    }

    public void GetCompanyDetails()
    {
        SqlDataAdapter Da = new SqlDataAdapter("select TOP 1 * from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode  WHERE Companyname='" + txtcompanyname.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            Fillddlshippingaddress(Dt.Rows[0]["Companyname"].ToString());
            Fillddlbillingaddress(Dt.Rows[0]["Companyname"].ToString());
            // Fillddlshippingaddress(Dt.Rows[0]["Companyname"].ToString());
            txtmobileno.Text = Dt.Rows[0]["Number"].ToString();
            txtemail.Text = Dt.Rows[0]["PrimaryEmailID"].ToString();
            // txtbillingGST.Text = Dt.Rows[0]["GSTno"].ToString();
            //ddlBillAddress.Text = Dt.Rows[0]["Billingaddress"].ToString();
            txtpaymentterm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
            txtpanno.Text = Dt.Rows[0]["Companypancard"].ToString();
            hhdstate.Value = Dt.Rows[0]["StateCode"].ToString();

            FillKittens();
        }
    }

    protected void txtagainstNumber_TextChanged(object sender, EventArgs e)
    {
        if (txtagainstNumber.SelectedItem.Text != "--Select--" && txtagainstNumber.SelectedItem.Text != "")
        {
            Dt_Product.Columns.AddRange(new DataColumn[20] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal"), new DataColumn("Orderquantity"), new DataColumn("ShippingQuantity"), new DataColumn("Balance") });
            ViewState["PurchaseOrderProduct"] = Dt_Product;
            divTotalPart.Visible = true;

            SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_QuotationDtls] WHERE Quotation_no='" + txtagainstNumber.SelectedItem.Text + "'", Cls_Main.Conn);
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

            dgvMachineDetails.EmptyDataText = "No Data Found";
            dgvMachineDetails.DataSource = Dt_Product;
            dgvMachineDetails.DataBind();

        }
    }

    protected void txtinvoiceagainst_TextChanged(object sender, EventArgs e)
    {
        if (txtinvoiceagainst.SelectedItem.Text == "Order")
        {
            idheader.Visible = false;
            idproducttable.Visible = false;
            txtagainstNumber.Enabled = true;

            Fillddlagainstnumber();

        }
        if (txtinvoiceagainst.SelectedItem.Text == "Direct")
        {
            //  Fillddlagainstnumber();
            idheader.Visible = true;
            idproducttable.Visible = true;
            txtagainstNumber.Enabled = false;
            dgvMachineDetails.DataSource = null;
            dgvMachineDetails.DataBind();
            divTotalPart.Visible = false;
            ViewState.Remove("PurchaseOrderProduct");
            ViewState["RowNo"] = 0;

            Dt_Product.Columns.AddRange(new DataColumn[20] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal"), new DataColumn("Orderquantity"), new DataColumn("ShippingQuantity"), new DataColumn("Balance") });
            ViewState["PurchaseOrderProduct"] = Dt_Product;
            // FillddlProduct();
        }
    }
    private void Fillddlagainstnumber()
    {
        txtagainstNumber.Items.Clear();
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [Quotationno] FROM [tbl_QuotationHdr] WHERE  Companyname='" + txtcompanyname.Text + "' AND IsDeleted=0  AND Status=1", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtagainstNumber.DataSource = dt;
            txtagainstNumber.DataTextField = "Quotationno";
            txtagainstNumber.DataBind();
            txtagainstNumber.Items.Insert(0, "-- Select Quotation No --");
        }
        else
        {
            txtagainstNumber.DataSource = null;
            txtagainstNumber.DataBind();
            txtagainstNumber.Items.Insert(0, "-- Select Quotation No --");
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerPurchaseOrderList.aspx");
    }

    protected void txtshippingq_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtorderq.Text.Trim()) - Convert.ToDecimal(txtshippingq.Text.Trim());
        txtbalance.Text = TotalAmt.ToString();
    }

    protected void txtOquantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtSQuantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
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

    protected void uploadfile_Click(object sender, EventArgs e)
    {
        if (AttachmentUpload.HasFile)
        {
            string fileName = Path.GetFileName(AttachmentUpload.PostedFile.FileName);
            byte[] fileContent;

            using (Stream fs = AttachmentUpload.PostedFile.InputStream)
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fileContent = br.ReadBytes((int)fs.Length);
                }
            }
            ViewState["attachment"] = null;
            ViewState["attachment"] = fileContent;
            lblfile1.Text = fileName;
        }

    }

    protected void ddlShippingaddress_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT TOP 1 * FROM tbl_ShippingAddress  where ShippingAddress='" + ddlShippingaddress.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ViewState["Address"] = null;
            ViewState["Address"] = ddlShippingaddress.SelectedItem.Text;
            txtshortShippingaddress.Text = ddlShippingaddress.SelectedItem.Text;
            txtshippinglocation.Text = dt.Rows[0]["ShipLocation"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["ShipPincode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["ShipStatecode"].ToString();
            txtshippingGST.Text = dt.Rows[0]["GSTNo"].ToString();

        }
    }

    protected void lnkEditCompany_Click(object sender, EventArgs e)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT ID FROM [tbl_CompanyMaster] where CompanyName='" + txtcompanyname.Text + "' ");
        Response.Redirect("CompanyMaster.aspx?ID=" + objcls.encrypt(Dt.Rows[0]["ID"].ToString()) + "&OAID=" + objcls.encrypt(txtpono.Text) + "");
    }

    protected void ddlContacts_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select Name,EmailID,Number from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode  WHERE CC.ID='" + ddlContacts.SelectedValue + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {

            txtmobileno.Text = Dt.Rows[0]["Number"].ToString();
            txtemail.Text = Dt.Rows[0]["EmailID"].ToString();
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
                txtshortBillingaddress.Text = ddlBillAddress.SelectedItem.Text;
                txtbillinglocation.Text = dt.Rows[0]["BillLocation"].ToString();
                txtbillingPincode.Text = dt.Rows[0]["BillPincode"].ToString();
                txtbillingstatecode.Text = dt.Rows[0]["Billstatecode"].ToString();
                txtbillingGST.Text = dt.Rows[0]["GSTNo"].ToString();
            }
        }
    }

}


