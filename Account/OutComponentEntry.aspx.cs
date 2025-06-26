
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_OutComponentEntry : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    DataTable Dt_Product = new DataTable();
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
                hdnAVQty.Value = string.Empty;
                ViewState["RowNo"] = 0;
                Dt_Product.Columns.AddRange(new DataColumn[5] { new DataColumn("id"), new DataColumn("Particular"), new DataColumn("ComponentName"), new DataColumn("Batch"), new DataColumn("Quantity") });
                ViewState["gvcomponent"] = Dt_Product;
                if (Request.QueryString["Id"] != null)
                {
                    ID = objcls.Decrypt(Request.QueryString["Id"].ToString());
                    hhd.Value = ID;
                    Load_Record(ID);

                }
            }
        }
    }
    protected void ChallanNo()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([ID]) as maxid FROM [tbl_OutwardEntryHdr]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            // txtInvoiceno.Text = "WLSPL/TI-" + (maxid + 1).ToString();
            txtchallanno.Text = "Ch-" + (maxid + 1).ToString("D4") + "/" + currentYear.ToString().Substring(2) + "-" + nextYear.ToString().Substring(2);
        }
        else
        {
            txtchallanno.Text = string.Empty;
        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = new DataTable();
        try
        {
            SqlDataAdapter sad = new SqlDataAdapter("select * from tbl_OutwardEntryHdr where invoiceno='" + ID + "'", Cls_Main.Conn);
            sad.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                btnsave.Text = "Update";
                txtbillingcustomer.Text = Dt.Rows[0]["companyname"].ToString();
                txtemail.Text = Dt.Rows[0]["email"].ToString();
                txtAgainstnumber.Text = Dt.Rows[0]["invoiceno"].ToString();
                txtInvoicedate.Text = Dt.Rows[0]["invoicedate"].ToString();
                txtchallanno.Text = Dt.Rows[0]["ChallanNo"].ToString();
                txtchallandate.Text = Dt.Rows[0]["ChallanDate"].ToString();
                txtpono.Text = Dt.Rows[0]["pono"].ToString();
                txtpodate.Text = Dt.Rows[0]["PODate"].ToString();
                txtbillito.Text = Dt.Rows[0]["companyname"].ToString();
                txtaddress.Text = Dt.Rows[0]["companyaddress"].ToString();
                txtvehicleno.Text = Dt.Rows[0]["vehicleno"].ToString();
                txtpartygstno.Text = Dt.Rows[0]["companygstno"].ToString();
                txtpartypanno.Text = Dt.Rows[0]["companypanno"].ToString();
                txtewaybillno.Text = Dt.Rows[0]["EwayBillno"].ToString();
                txtpaymentterm.Text = Dt.Rows[0]["payementterm"].ToString();
                txtShiftTo.Text = Dt.Rows[0]["ShiftTo"].ToString();
                txtShiftAddress.Text = Dt.Rows[0]["ShiftAddress"].ToString();
                txtstatecode.Text = Dt.Rows[0]["companystatecode"].ToString();


                DataTable dtt = new DataTable();
                SqlDataAdapter sad3 = new SqlDataAdapter("select ID,Particular,ComponentName,Batch,Quantity from tbl_OutwardEntryComponentsDtls where OrderNo='" + ID + "'", con);
                sad3.Fill(dtt);
                if (dtt.Rows.Count > 0)
                {
                    ViewState["gvcomponent"] = dtt;
                    gvcomponent.DataSource = dtt;
                    gvcomponent.DataBind();
                }

                DataTable dtt1 = new DataTable();
                SqlDataAdapter sad31 = new SqlDataAdapter("select TD.Id, Particular, Qty, TD.Batchno,* from tblTaxInvoiceDtls AS TD INNER JOIN tblTaxInvoiceHdr AS TH ON TH.Id=TD.HeaderID where InvoiceNo='" + ID + "'", con);
                sad31.Fill(dtt1);
                if (dtt1.Rows.Count > 0)
                {

                    dgvTaxinvoiceDetails.DataSource = dtt1;
                    dgvTaxinvoiceDetails.DataBind();
                }
            }
            else
            {
                ChallanNo();
                txtchallandate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                Int32 Iid = 0;
                SqlDataAdapter sad1 = new SqlDataAdapter("select * from tblTaxInvoiceHdr where InvoiceNo='" + ID + "' AND Status=2", Cls_Main.Conn);
                sad1.Fill(Dt);
                if (Dt.Rows.Count > 0)
                {
                    Iid = Convert.ToInt32(Dt.Rows[0]["Id"].ToString());
                    txtbillingcustomer.Text = Dt.Rows[0]["BillingCustomer"].ToString();
                    txtemail.Text = Dt.Rows[0]["Email"].ToString();
                    txtAgainstnumber.Text = Dt.Rows[0]["InvoiceNo"].ToString();
                    txtInvoicedate.Text = Dt.Rows[0]["Invoicedate"].ToString();
                    txtpono.Text = Dt.Rows[0]["CustomerPONo"].ToString();
                    txtpodate.Text = Dt.Rows[0]["PODate"].ToString();
                    txtbillito.Text = Dt.Rows[0]["BillingCustomer"].ToString();
                    txtaddress.Text = Dt.Rows[0]["BillingAddress"].ToString();
                    txtvehicleno.Text = Dt.Rows[0]["VehicalNo"].ToString();
                    txtpartygstno.Text = Dt.Rows[0]["BillingGST"].ToString();
                    if(txtpartygstno.Text!= "URP")
                    {
                        txtpartypanno.Text = txtpartygstno.Text.Substring(2, 10);
                    }
                    else
                    {
                        txtpartypanno.Text = "URP";
                    }                
                    txtewaybillno.Text = Dt.Rows[0]["E_BillNo"].ToString();
                    txtpaymentterm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
                    txtShiftTo.Text = Dt.Rows[0]["ShippingCustomer"].ToString();
                    txtShiftAddress.Text = Dt.Rows[0]["ShippingAddress"].ToString();
                    txtstatecode.Text = Dt.Rows[0]["BillingStatecode"].ToString();


                    DataTable dtt = new DataTable();
                    SqlDataAdapter sad3 = new SqlDataAdapter("select Id,Particular,Description,Batchno,HSN,Qty,UOM,Rate,TaxableAmt,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Discount,GrandTotal from tblTaxInvoiceDtls where HeaderID='" + Iid + "'", con);
                    sad3.Fill(dtt);
                    if (dtt.Rows.Count > 0)
                    {

                        dgvTaxinvoiceDetails.DataSource = dtt;
                        dgvTaxinvoiceDetails.DataBind();
                    }
                    DataTable dt = new DataTable();
                    SqlDataAdapter sa = new SqlDataAdapter("select ID,Particular,ComponentName,Batch,Quantity from tbl_OutwardEntryComponentsDtls where OrderNo='" + ID + "'", con);
                    sa.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        ViewState["gvcomponent"] = dt;
                        gvcomponent.DataSource = dt;
                        gvcomponent.DataBind();
                    }

                }
            }


        }
        catch (Exception)
        {

            throw;
        }
    }


    protected void btnsave_Click(object sender, EventArgs e)
    {
        SaveRecord();
    }

    protected void SaveRecord()
    {
        string Batchno = string.Empty;
        string Id = string.Empty;
        try
        {
            if (txtbillingcustomer.Text == "" || txtaddress.Text == "" || txtInvoicedate.Text == "" || txtchallandate.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                if (dgvTaxinvoiceDetails.Rows.Count > 0)
                {
                    if (txtbillingcustomer.Text == "" && txtchallandate.Text == "")
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
                    }
                    else
                    {
                        if (dgvTaxinvoiceDetails.Rows.Count > 0)
                        {
                            foreach (GridViewRow gr1 in dgvTaxinvoiceDetails.Rows)
                            {
                                Batchno = (gr1.FindControl("txtBatchno") as TextBox).Text;
                                Id = (gr1.FindControl("lblid") as Label).Text;
                                Cls_Main.Conn_Open();
                                SqlCommand Cmd = new SqlCommand("UPDATE [tblTaxInvoiceDtls] SET Batchno=@BatchNo WHERE Id=@InvoiceNo", Cls_Main.Conn);
                                Cmd.Parameters.AddWithValue("@InvoiceNo", Convert.ToInt64(Id));
                                Cmd.Parameters.AddWithValue("@BatchNo", Batchno);
                                Cmd.ExecuteNonQuery();
                                Cls_Main.Conn_Open();

                            }
                            if (Batchno == "")
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter Batch No on Products..!!')", true);
                            }
                            else
                            {

                                if (btnsave.Text == "Save")
                                {
                                    Cls_Main.Conn_Open();
                                    SqlCommand cmd = new SqlCommand("SP_OutwardEntry", Cls_Main.Conn);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@Action", "Save");
                                    cmd.Parameters.AddWithValue("@podate", txtpodate.Text);
                                    cmd.Parameters.AddWithValue("@pono", txtpono.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@EwayBillno", txtewaybillno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companystatecode", txtstatecode.Text);
                                    cmd.Parameters.AddWithValue("@companyname", txtbillito.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companyaddress", txtaddress.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companypanno", txtpartypanno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companygstno", txtpartygstno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@payementterm", txtpaymentterm.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@vehicleno", txtvehicleno.Text);
                                    cmd.Parameters.AddWithValue("@email", txtemail.Text);

                                    // cmd.Parameters.AddWithValue("@updatedby", Session["UserCode"].ToString().ToUpper());
                                    cmd.Parameters.AddWithValue("@customername", txtbillingcustomer.Text);
                                    cmd.Parameters.AddWithValue("@shiftTo", txtShiftTo.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@shiftaddress", txtShiftAddress.Text.ToUpper());

                                    cmd.Parameters.AddWithValue("@Outwardtime", DBNull.Value);
                                    // cmd.Parameters.AddWithValue("@EntryType", ddlentrytype.SelectedItem.Text);
                                    // cmd.Parameters.AddWithValue("@Updatedate", DateTime.Now);

                                    cmd.Parameters.AddWithValue("@invoiceno", txtAgainstnumber.Text);
                                    cmd.Parameters.AddWithValue("@invoicedate", txtInvoicedate.Text);

                                    cmd.Parameters.AddWithValue("@ChallanNo", txtchallanno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@ChallanDate", txtchallandate.Text);
                                    cmd.Parameters.AddWithValue("@Sub_total", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@CGST_Amt", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@SGST_Amt", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@IGST_Amt", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@Total_Price", DBNull.Value);
                                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@IsDeleted", '0');
                                    cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                                    cmd.ExecuteNonQuery();
                                    Cls_Main.Conn_Close();
                                    Cls_Main.Conn_Dispose();

                                    //Save Component Details 
                                    if (gvcomponent.Rows.Count > 0)
                                    {
                                        foreach (GridViewRow grd1 in gvcomponent.Rows)
                                        {
                                            string Product = (grd1.FindControl("lblproduct") as Label).Text;
                                            string lblCompo = (grd1.FindControl("lblComComPonent") as Label).Text;
                                            string lblbatch = (grd1.FindControl("lblComBatch") as Label).Text;
                                            string lblQuantity = (grd1.FindControl("lblComQuantity") as Label).Text;

                                            string lblDescription = "";
                                            string lblhsn = "";
                                            string lblUnit = "";
                                            string lblRate = "";
                                            string lblTotal = "";
                                            string lblCGSTPer = "";
                                            string lblCGST = "";
                                            string lblSGSTPer = "";
                                            string lblSGST = "";
                                            string lblIGSTPer = "";
                                            string lblIGST = "";
                                            string lblDiscount = "";
                                            string lblDiscountAmount = "";
                                            string lblAlltotal = "";

                                            con.Open(); // Open the connection

                                            SqlDataAdapter sad1 = new SqlDataAdapter("SELECT  * FROM vw_batchwisecomponent WHERE ComponentName='" + lblCompo + "' AND Batch='" + lblbatch + "'", con);
                                            DataTable Dt = new DataTable();
                                            sad1.Fill(Dt);

                                            con.Close(); // Close the connection when done

                                            if (Dt.Rows.Count > 0)
                                            {
                                                //lblRate = Dt.Rows[0]["Rate"].ToString();
                                                //lblCGSTPer = Dt.Rows[0]["CGSTPer"].ToString();
                                                //lblSGSTPer = Dt.Rows[0]["SGSTPer"].ToString();
                                                //lblIGSTPer = Dt.Rows[0]["IGSTPer"].ToString();
                                                lblUnit = Dt.Rows[0]["Units"].ToString();
                                                lblDescription = Dt.Rows[0]["Description"].ToString();
                                                lblhsn = Dt.Rows[0]["HSN"].ToString();
                                            }
                                            SqlDataAdapter sad2 = new SqlDataAdapter("select * from tbl_OutwardEntryDtls where RefNo='" + txtAgainstnumber.Text + "' AND Productname='" + Product + "'", con);
                                            DataTable Dt1 = new DataTable();
                                            sad2.Fill(Dt1);
                                            con.Close(); // Close the connection when done
                                            if (Dt1.Rows.Count > 0)
                                            {
                                                lblRate = Dt1.Rows[0]["Rate"].ToString();
                                                lblCGSTPer = Dt1.Rows[0]["CGSTPer"].ToString();
                                                lblSGSTPer = Dt1.Rows[0]["SGSTPer"].ToString();
                                                lblIGSTPer = Dt1.Rows[0]["IGSTPer"].ToString();
                                            }
                                            var total = Convert.ToDecimal(lblRate) * Convert.ToDecimal(lblQuantity);
                                            lblTotal = string.Format("{0:0.00}", total);
                                            decimal tax_amt;
                                            decimal cgst_amt;
                                            decimal sgst_amt;
                                            decimal igst_amt;

                                            if (string.IsNullOrEmpty(lblCGSTPer))
                                            {
                                                cgst_amt = 0;
                                            }
                                            else
                                            {
                                                cgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblCGSTPer) / 100;
                                            }
                                            lblCGST = string.Format("{0:0.00}", cgst_amt);

                                            if (string.IsNullOrEmpty(lblSGSTPer))
                                            {
                                                sgst_amt = 0;
                                            }
                                            else
                                            {
                                                sgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblSGSTPer) / 100;
                                            }
                                            lblSGST = string.Format("{0:0.00}", sgst_amt);

                                            if (string.IsNullOrEmpty(lblIGSTPer))
                                            {
                                                igst_amt = 0;
                                            }
                                            else
                                            {
                                                igst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblIGSTPer) / 100;
                                            }
                                            lblIGST = string.Format("{0:0.00}", igst_amt);

                                            tax_amt = cgst_amt + sgst_amt + igst_amt;

                                            var totalWithTax = Convert.ToDecimal(total.ToString()) + Convert.ToDecimal(tax_amt.ToString());
                                            decimal disc_amt;
                                            if (string.IsNullOrEmpty(lblDiscount))
                                            {
                                                disc_amt = 0;
                                            }
                                            else
                                            {
                                                disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(lblDiscount) / 100;
                                                //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
                                            }

                                            var Grossamt = Convert.ToDecimal(total.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
                                            lblAlltotal = string.Format("{0:0.00}", Grossamt);
                                            //  lblCDiscountAmount = string.Format("{0:0}", disc_amt);

                                            Cls_Main.Conn_Open();
                                            SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_OutwardEntryComponentsDtls] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                                            cmdd.Parameters.AddWithValue("@OrderNo", txtAgainstnumber.Text);
                                            cmdd.Parameters.AddWithValue("@Particular", Product);
                                            cmdd.Parameters.AddWithValue("@ComponentName", lblCompo);
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
                                            cmdd.Parameters.AddWithValue("@Batch", lblbatch);
                                            cmdd.ExecuteNonQuery();
                                            Cls_Main.Conn_Close();
                                            Cls_Main.Conn_Open();

                                            SqlCommand cmdd1 = new SqlCommand("INSERT INTO [tbl_InventoryOutwardManage] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                                            cmdd1.Parameters.AddWithValue("@OrderNo", txtAgainstnumber.Text);
                                            cmdd1.Parameters.AddWithValue("@Particular", Product);
                                            cmdd1.Parameters.AddWithValue("@ComponentName", lblCompo);
                                            cmdd1.Parameters.AddWithValue("@Description", lblDescription);
                                            cmdd1.Parameters.AddWithValue("@HSN", lblhsn);
                                            cmdd1.Parameters.AddWithValue("@Quantity", lblQuantity);
                                            cmdd1.Parameters.AddWithValue("@Units", lblUnit);
                                            cmdd1.Parameters.AddWithValue("@Rate", lblRate);
                                            cmdd1.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                                            cmdd1.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                                            cmdd1.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                                            cmdd1.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                                            cmdd1.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                                            cmdd1.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                                            cmdd1.Parameters.AddWithValue("@Total", lblTotal);
                                            cmdd1.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                                            cmdd1.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);

                                            cmdd1.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                                            cmdd1.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                                            cmdd1.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                                            cmdd1.Parameters.AddWithValue("@Batch", lblbatch);
                                            cmdd1.ExecuteNonQuery();
                                            Cls_Main.Conn_Close();
                                        }
                                    }
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Outward Entry Save Successfully..!!');window.location='WarehouseInvoiceList.aspx'; ", true);
                                    //Save Contact Details End
                                }
                                else if (btnsave.Text == "Update")
                                {

                                    Cls_Main.Conn_Open();
                                    SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_OutwardEntryDtls WHERE RefNo=@RefNo", Cls_Main.Conn);
                                    cmddelete.Parameters.AddWithValue("@RefNo", txtAgainstnumber.Text);
                                    cmddelete.ExecuteNonQuery();
                                    Cls_Main.Conn_Close();

                                    Cls_Main.Conn_Open();
                                    SqlCommand cmd = new SqlCommand("SP_OutwardEntry", Cls_Main.Conn);
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@Action", "Update");
                                    cmd.Parameters.AddWithValue("@podate", txtpodate.Text);
                                    cmd.Parameters.AddWithValue("@pono", txtpono.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@EwayBillno", txtewaybillno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companystatecode", txtstatecode.Text);
                                    cmd.Parameters.AddWithValue("@companyname", txtbillito.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companyaddress", txtaddress.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companypanno", txtpartypanno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@companygstno", txtpartygstno.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@payementterm", txtpaymentterm.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@vehicleno", txtvehicleno.Text);
                                    cmd.Parameters.AddWithValue("@email", txtemail.Text);
                                    cmd.Parameters.AddWithValue("@customername", txtbillingcustomer.Text);
                                    cmd.Parameters.AddWithValue("@shiftTo", txtShiftTo.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@shiftaddress", txtShiftAddress.Text.ToUpper());
                                    cmd.Parameters.AddWithValue("@invoiceno", txtAgainstnumber.Text);
                                    cmd.Parameters.AddWithValue("@invoicedate", txtInvoicedate.Text);
                                    cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@IsDeleted", '0');
                                    cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                                    cmd.ExecuteNonQuery();
                                    Cls_Main.Conn_Close();
                                    Cls_Main.Conn_Dispose();


                                    Cls_Main.Conn_Open();
                                    SqlCommand cmddelete1 = new SqlCommand("DELETE FROM tbl_OutwardEntryComponentsDtls WHERE OrderNo=@OrderNo", Cls_Main.Conn);
                                    cmddelete1.Parameters.AddWithValue("@OrderNo", txtAgainstnumber.Text);
                                    cmddelete1.ExecuteNonQuery();
                                    Cls_Main.Conn_Close();
                                    Cls_Main.Conn_Dispose();

                                    SqlCommand cmddelete2 = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + txtAgainstnumber.Text.Trim() + "' ", con);
                                    con.Open();
                                    cmddelete2.ExecuteNonQuery();
                                    con.Close();

                                    //Save Component Details 
                                    if (gvcomponent.Rows.Count > 0)
                                    {
                                        foreach (GridViewRow grd1 in gvcomponent.Rows)
                                        {
                                            string Product = (grd1.FindControl("lblproduct") as Label).Text;
                                            string lblCompo = (grd1.FindControl("lblComComPonent") as Label).Text;
                                            string lblbatch = (grd1.FindControl("lblComBatch") as Label).Text;
                                            string lblQuantity = (grd1.FindControl("lblComQuantity") as Label).Text;

                                            string lblDescription = "";
                                            string lblhsn = "";
                                            string lblUnit = "";
                                            string lblRate = "";
                                            string lblTotal = "";
                                            string lblCGSTPer = "";
                                            string lblCGST = "";
                                            string lblSGSTPer = "";
                                            string lblSGST = "";
                                            string lblIGSTPer = "";
                                            string lblIGST = "";
                                            string lblDiscount = "";
                                            string lblDiscountAmount = "";
                                            string lblAlltotal = "";

                                            con.Open(); // Open the connection

                                            SqlDataAdapter sad1 = new SqlDataAdapter("SELECT  * FROM vw_batchwisecomponent WHERE ComponentName='" + lblCompo + "' AND Batch='" + lblbatch + "'", con);
                                            DataTable Dt = new DataTable();
                                            sad1.Fill(Dt);
                                            con.Close(); // Close the connection when done
                                            if (Dt.Rows.Count > 0)
                                            {
                                                //lblRate = Dt.Rows[0]["Rate"].ToString();
                                                //lblCGSTPer = Dt.Rows[0]["CGSTPer"].ToString();
                                                //lblSGSTPer = Dt.Rows[0]["SGSTPer"].ToString();
                                                //lblIGSTPer = Dt.Rows[0]["IGSTPer"].ToString();
                                                lblUnit = Dt.Rows[0]["Units"].ToString();
                                                lblDescription = Dt.Rows[0]["Description"].ToString();
                                                lblhsn = Dt.Rows[0]["HSN"].ToString();
                                            }
                                            SqlDataAdapter sad2 = new SqlDataAdapter("select * from tbl_OutwardEntryDtls where RefNo='" + txtAgainstnumber.Text + "' AND Productname='" + Product + "'", con);
                                            DataTable Dt1 = new DataTable();
                                            sad2.Fill(Dt1);
                                            con.Close(); // Close the connection when done
                                            if (Dt1.Rows.Count > 0)
                                            {
                                                lblRate = Dt1.Rows[0]["Rate"].ToString();
                                                lblCGSTPer = Dt1.Rows[0]["CGSTPer"].ToString();
                                                lblSGSTPer = Dt1.Rows[0]["SGSTPer"].ToString();
                                                lblIGSTPer = Dt1.Rows[0]["IGSTPer"].ToString();
                                            }
                                            var total = Convert.ToDecimal(lblRate) * Convert.ToDecimal(lblQuantity);
                                            lblTotal = string.Format("{0:0.00}", total);
                                            decimal tax_amt;
                                            decimal cgst_amt;
                                            decimal sgst_amt;
                                            decimal igst_amt;

                                            if (string.IsNullOrEmpty(lblCGSTPer))
                                            {
                                                cgst_amt = 0;
                                            }
                                            else
                                            {
                                                cgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblCGSTPer) / 100;
                                            }
                                            lblCGST = string.Format("{0:0.00}", cgst_amt);

                                            if (string.IsNullOrEmpty(lblSGSTPer))
                                            {
                                                sgst_amt = 0;
                                            }
                                            else
                                            {
                                                sgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblSGSTPer) / 100;
                                            }
                                            lblSGST = string.Format("{0:0.00}", sgst_amt);

                                            if (string.IsNullOrEmpty(lblIGSTPer))
                                            {
                                                igst_amt = 0;
                                            }
                                            else
                                            {
                                                igst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblIGSTPer) / 100;
                                            }
                                            lblIGST = string.Format("{0:0.00}", igst_amt);

                                            tax_amt = cgst_amt + sgst_amt + igst_amt;

                                            var totalWithTax = Convert.ToDecimal(total.ToString()) + Convert.ToDecimal(tax_amt.ToString());
                                            decimal disc_amt;
                                            if (string.IsNullOrEmpty(lblDiscount))
                                            {
                                                disc_amt = 0;
                                            }
                                            else
                                            {
                                                disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(lblDiscount) / 100;
                                                //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
                                            }

                                            var Grossamt = Convert.ToDecimal(total.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
                                            lblAlltotal = string.Format("{0:0.00}", Grossamt);
                                            //  lblCDiscountAmount = string.Format("{0:0}", disc_amt);

                                            Cls_Main.Conn_Open();
                                            SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_OutwardEntryComponentsDtls] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                                            cmdd.Parameters.AddWithValue("@OrderNo", txtAgainstnumber.Text);
                                            cmdd.Parameters.AddWithValue("@Particular", Product);
                                            cmdd.Parameters.AddWithValue("@ComponentName", lblCompo);
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
                                            cmdd.Parameters.AddWithValue("@Batch", lblbatch);
                                            cmdd.ExecuteNonQuery();
                                            Cls_Main.Conn_Close();

                                          
                                           

                                            Cls_Main.Conn_Open();

                                            SqlCommand cmdd1 = new SqlCommand("INSERT INTO [tbl_InventoryOutwardManage] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                                            cmdd1.Parameters.AddWithValue("@OrderNo", txtAgainstnumber.Text);
                                            cmdd1.Parameters.AddWithValue("@Particular", Product);
                                            cmdd1.Parameters.AddWithValue("@ComponentName", lblCompo);
                                            cmdd1.Parameters.AddWithValue("@Description", lblDescription);
                                            cmdd1.Parameters.AddWithValue("@HSN", lblhsn);
                                            cmdd1.Parameters.AddWithValue("@Quantity", lblQuantity);
                                            cmdd1.Parameters.AddWithValue("@Units", lblUnit);
                                            cmdd1.Parameters.AddWithValue("@Rate", lblRate);
                                            cmdd1.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                                            cmdd1.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                                            cmdd1.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                                            cmdd1.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                                            cmdd1.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                                            cmdd1.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                                            cmdd1.Parameters.AddWithValue("@Total", lblTotal);
                                            cmdd1.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                                            cmdd1.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);

                                            cmdd1.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                                            cmdd1.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                                            cmdd1.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                                            cmdd1.Parameters.AddWithValue("@Batch", lblbatch);
                                            cmdd1.ExecuteNonQuery();
                                            Cls_Main.Conn_Close();

                                        }
                                    }
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Outward Entry Updated Successfully..!!');window.location='WarehouseInvoiceList.aspx'; ", true);
                                }
                            }
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
            // throw ex;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please try again..!!')", true);
        }
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("WarehouseInvoiceList.aspx");
    }

    protected void dgvTaxinvoiceDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlComponent = (DropDownList)e.Row.FindControl("ddlComponent");
                DropDownList ddlBatch = (DropDownList)e.Row.FindControl("ddlBatch");

                //SqlDataAdapter ad = new SqlDataAdapter("select DISTINCT ComponentName from vw_batchwisecomponent where BalanceQty>0 ", Cls_Main.Conn);
                //DataTable dt = new DataTable();
                //ad.Fill(dt);
                //if (dt.Rows.Count > 0)
                //{
                //    ddlComponent.DataSource = dt;
                //    ddlComponent.DataTextField = "ComponentName";
                //    ddlComponent.DataValueField = "ComponentName";
                //    ddlComponent.DataBind();
                //    ddlComponent.Items.Insert(0, "---Select Component Name---");
                //}


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

    protected void ddlComponent_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlComponent = (DropDownList)sender;
        GridViewRow row = (GridViewRow)ddlComponent.NamingContainer;
        DropDownList ddlBatch = (DropDownList)row.FindControl("ddlBatch");

        if (ddlComponent.SelectedItem.Text != null)
        {
            SqlDataAdapter ad = new SqlDataAdapter("select Batch from vw_batchwisecomponent where BalanceQty>0 AND ComponentName='" + ddlComponent.SelectedItem.Text + "' ", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddlBatch.DataSource = dt;
                ddlComponent.DataTextField = "Batch";
                ddlComponent.DataValueField = "Batch";
                ddlBatch.DataBind();
            }
            else
            {
                ddlBatch.DataSource = null;
                //        //ddlBatch.DataValueField = "ID";
                ddlBatch.DataTextField = "Data Not Found....!";
                ddlBatch.DataBind();
                ddlBatch.Items.Insert(0, "Not Available....!");
            }

        }
    }

    [WebMethod]
    public static List<ListItem> GetComponent()
    {
        List<ListItem> batches = new List<ListItem>();

        SqlDataAdapter ad = new SqlDataAdapter("select DISTINCT ComponentName from vw_batchwisecomponent where BalanceQty>0 ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            // Loop through the DataTable and create ListItem objects
            foreach (DataRow row in dt.Rows)
            {
                batches.Add(new ListItem(row["ComponentName"].ToString(), row["ComponentName"].ToString()));
            }
        }

        return batches;
    }

    [WebMethod]
    public static List<ListItem> GetBatches(string Component)
    {
        List<ListItem> batches = new List<ListItem>();

        SqlDataAdapter ad = new SqlDataAdapter("select Batch from vw_batchwisecomponent where BalanceQty>0 AND ComponentName='" + Component + "' ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            // Loop through the DataTable and create ListItem objects
            foreach (DataRow row in dt.Rows)
            {
                batches.Add(new ListItem(row["Batch"].ToString(), row["Batch"].ToString()));
            }
        }

        return batches;
    }

    [WebMethod]
    public static string GetBatchesWiseQty(string Component, string Batch)
    {
        string Quontity = string.Empty;
        SqlDataAdapter ad = new SqlDataAdapter("select BalanceQty from vw_batchwisecomponent where BalanceQty>0 AND ComponentName='" + Component + "' AND Batch='" + Batch + "' ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            Quontity = dt.Rows[0]["BalanceQty"].ToString();
        }
        return Quontity;
    }


    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("WarehouseInvoiceList.aspx");
    }


    protected void btnSaveCompo_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as Button).NamingContainer as GridViewRow;
        Double Mapqty = 0;
        string Quantity = ((TextBox)row.FindControl("txtQuontity")).Text;
        Quantity = Quantity.Replace(",", string.Empty);
        string avqty = hdnQty.Value;
        string Batch = hdnbatch.Value;
        string ComponentName = hdncompo.Value;
        string id = ((Label)row.FindControl("lblid")).Text;
        string Particular = ((Label)row.FindControl("lblproduct")).Text;
        string lblQuantity = ((Label)row.FindControl("lblQuantity")).Text;
        if (Convert.ToDouble(lblQuantity) >= Convert.ToDouble(Quantity))
        {

            if (gvcomponent.Rows.Count > 0)
            {
                foreach (GridViewRow grd1 in gvcomponent.Rows)
                {
                    string Product = (grd1.FindControl("lblproduct") as Label).Text;
                    string lblQuantiy = (grd1.FindControl("lblComQuantity") as Label).Text;
                    if (Particular == Product)
                    {
                        Mapqty += Convert.ToDouble(lblQuantiy);
                    }
                }
            }

            Double Total = Convert.ToDouble(Quantity) + Mapqty;
            if (Convert.ToDouble(lblQuantity) >= Total)
            {
                hdnAVQty.Value += lblQuantity;
                ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
                DataTable Dt = (DataTable)ViewState["gvcomponent"];
                Dt.Rows.Add(ViewState["RowNo"], Particular, ComponentName, Batch, Quantity);
                ViewState["gvcomponent"] = Dt;
                gvcomponent.DataSource = (DataTable)ViewState["gvcomponent"];
                gvcomponent.DataBind();
            }
            else
              {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Component Quantity is more then Product Quantity..!!')", true);
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Component Quantity is more then Product Quantity..!!')", true);
        }

    }

    protected void gvcomponent_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvcomponent.EditIndex = e.NewEditIndex;
        gvcomponent.DataSource = (DataTable)ViewState["gvcomponent"];
        gvcomponent.DataBind();
    }

    protected void gv_Compupdate_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Particular = ((TextBox)row.FindControl("txtCOMPParticular")).Text;
        string Component = ((TextBox)row.FindControl("txtCOMPComponent")).Text;
        string Batch = ((TextBox)row.FindControl("txtCOMPBatch")).Text;
        string Quantity = ((TextBox)row.FindControl("txtCOMPQuantity")).Text;

        DataTable Dt = ViewState["gvcomponent"] as DataTable;
        Dt.Rows[row.RowIndex]["Particular"] = Particular;
        Dt.Rows[row.RowIndex]["ComponentName"] = Component;
        Dt.Rows[row.RowIndex]["Batch"] = Batch;
        Dt.Rows[row.RowIndex]["Quantity"] = Quantity;
        Dt.AcceptChanges();
        ViewState["gvcomponent"] = Dt;
        gvcomponent.EditIndex = -1;
        gvcomponent.DataSource = (DataTable)ViewState["gvcomponent"];
        gvcomponent.DataBind();


    }

    protected void lnkbtnCompDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["gvcomponent"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["gvcomponent"] = dt;
        gvcomponent.DataSource = (DataTable)ViewState["gvcomponent"];
        gvcomponent.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Component Delete Succesfully !!!');", true);

    }

  
}


