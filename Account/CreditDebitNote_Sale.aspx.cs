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
using System.Globalization;

public partial class Account_CreditDebitNote : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable dt = new DataTable();
    CommonCls objcls = new CommonCls();
    DataTable Dt_Product = new DataTable();
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
                GenerateComCode_CR();
                tblparticular.Visible = false;
                DivManual.Visible = true;
                txtDocdate.Text = DateTime.Today.ToString("dd-MM-yyyy");
                //fillddlCategory();
                ViewState["RowNo"] = 0;
                Dt_Product.Columns.AddRange(new DataColumn[5] { new DataColumn("id"), new DataColumn("Particular"), new DataColumn("ComponentName"), new DataColumn("Batch"), new DataColumn("Quantity") });
                ViewState["gvcomponent"] = Dt_Product;
                UpdateHistorymsg = string.Empty; regdate = string.Empty;
                if (Request.QueryString["ID"] != null)
                {
                    dvupdatepearticullar.Visible = false;
                    ViewState["RowNo"] = 0;
                    dt.Columns.AddRange(new DataColumn[18] { new DataColumn("id"),new DataColumn("InvoiceNo"),
                 new DataColumn("Particulars"),new DataColumn("Description"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"), new DataColumn("Amount"),new DataColumn("UOM"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Discount"),new DataColumn("Remarks")
            });

                    ViewState["ParticularDetails"] = dt;

                    ViewState["UpdateRowId"] = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    GetCreditDebitNoteData(ViewState["UpdateRowId"].ToString());
                }
                else
                {
                    ViewState["RowNo"] = 0;
                    dt.Columns.AddRange(new DataColumn[18] { new DataColumn("id"),new DataColumn("InvoiceNo"),
                 new DataColumn("Particulars"),new DataColumn("Description"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"), new DataColumn("Amount"),new DataColumn("UOM"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Discount"),new DataColumn("Remarks")
            });
                    ViewState["ParticularDetails"] = dt;
                    //txtDocNo.Text = GenerateComCode();
                }


            }
        }
    }

    //private void fillddlCategory()
    // {
    //     SqlDataAdapter adpt = new SqlDataAdapter("select * from tblDebitCreditCategory", con);
    //     DataTable dtpt = new DataTable();
    //     adpt.Fill(dtpt);

    //     if (dtpt.Rows.Count > 0)
    //     {
    //          ddlCategory.DataSource = dtpt;
    //        ddlCategory.DataValueField = "Category";
    //        ddlCategory.DataTextField = "Category";
    //        ddlCategory.DataBind();

    //        ddlCategory.Items.Insert(0, new ListItem("--Select Category--", "0"));
    //   }
    //  }

    static string regdate = string.Empty;
    protected void GetCreditDebitNoteData(string id)
    {
        try
        {
            string query1 = string.Empty;
            query1 = @"select * from tblCreditDebitNoteHdr where Id='" + id + "' ";
            SqlDataAdapter ad = new SqlDataAdapter(query1, con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtSupplierName.Text = dt.Rows[0]["SupplierName"].ToString();
                txtDocNo.Text = dt.Rows[0]["DocNo"].ToString();
                //txtTCharge.Text = dt.Rows[0]["TransportationCharge"].ToString();
                //txtTCGSTPer.Text = dt.Rows[0]["TCGSTPer"].ToString();
                //txtTCGSTamt.Text = dt.Rows[0]["TCGSTAmt"].ToString();
                //txtTSGSTPer.Text = dt.Rows[0]["TSGSTPer"].ToString();
                //txtTSGSTamt.Text = dt.Rows[0]["TSGSTAmt"].ToString();
                //txtTIGSTPer.Text = dt.Rows[0]["TIGSTPer"].ToString();
                //txtTIGSTamt.Text = dt.Rows[0]["TIGSTAmt"].ToString();
                //txtTCost.Text = dt.Rows[0]["TotalCost"].ToString();

                sumofAmount.Text = dt.Rows[0]["SumOfProductAmt"].ToString();
                TxtChargesDesc.Text = dt.Rows[0]["ChargesDescription"].ToString();
                txthsntcs.Text = dt.Rows[0]["HSNTcs"].ToString();
                txtrateTcs.Text = dt.Rows[0]["RateTcs"].ToString();
                txtBasic.Text = dt.Rows[0]["Basic"].ToString();
                CGSTPertcs.Text = dt.Rows[0]["CGST"].ToString();
                SGSTPertcs.Text = dt.Rows[0]["SGST"].ToString();
                IGSTPertcs.Text = dt.Rows[0]["IGST"].ToString();
                txtCost.Text = dt.Rows[0]["Cost"].ToString();
                txtshortBillingaddress.Text = dt.Rows[0]["ShortBAddress"].ToString();
                txtshortShippingaddress.Text = dt.Rows[0]["ShortSAddress"].ToString();
                //  fillddlCategory();
                BindBillNO();

                // ddlProcess.Text = dt.Rows[0]["Process"].ToString();
                ddlNoteType.Text = dt.Rows[0]["NoteType"].ToString();
                txtcategory.Text = dt.Rows[0]["CategoryName"].ToString();
                txtDocdate.Text = dt.Rows[0]["DocDate"].ToString();

                ddlBillNumber.SelectedItem.Text = dt.Rows[0]["BillNumber"].ToString();
                if (dt.Rows[0]["BillDate"].ToString() == "")
                {
                }
                else
                {
                    txtBillDate.Text = dt.Rows[0]["BillDate"].ToString();
                }

                if (dt.Rows[0]["Process"].ToString() == "Manual")
                {
                    ddlBillNumber.Enabled = false;
                    txtBillDate.Enabled = false;
                }
                else
                {
                    ddlBillNumber.SelectedItem.Text = dt.Rows[0]["BillNumber"].ToString();
                    txtBillDate.Text = dt.Rows[0]["BillDate"].ToString();
                }
                txtPaymentDueDate.Text = dt.Rows[0]["PaymentDueDate"].ToString();
                txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();
                getParticularsdts(id);
                //DivManual.Visible = true;
                btnadd.Text = "Update";
                Fillddlshippingaddress(txtSupplierName.Text);
                FillddlBillingAddress(txtSupplierName.Text);
                //New Details for E-Invoice
                txtshippingcustomer.Text = dt.Rows[0]["ShippingCustomer"].ToString();
                ddlShippingaddress.SelectedValue = dt.Rows[0]["ShippingAddress"].ToString();
                txtbillingGST.Text = dt.Rows[0]["BillingGST"].ToString();
                txtshippingGST.Text = dt.Rows[0]["ShippingGST"].ToString();
                ddlBillAddress.SelectedItem.Text = dt.Rows[0]["BillingAddress"].ToString();
                txtbillinglocation.Text = dt.Rows[0]["BillingLocation"].ToString();
                txtshippinglocation.Text = dt.Rows[0]["ShippingLocation"].ToString();
                txtbillingPincode.Text = dt.Rows[0]["BillingPincode"].ToString();
                txtshippingPincode.Text = dt.Rows[0]["ShippingPincode"].ToString();
                txtbillingstatecode.Text = dt.Rows[0]["BillingStatecode"].ToString();
                txtshippingstatecode.Text = dt.Rows[0]["ShippingStatecode"].ToString();

                txttransportMode.Text = dt.Rows[0]["TransportMode"].ToString();
                if (txttransportMode.Text == "By Road" || txttransportMode.Text == "By Courier")
                {
                    txtvehicalNumber.Text = dt.Rows[0]["VehicalNo"].ToString();
                    txtvehicalNumber.Visible = true;
                }
                else if (txttransportMode.Text == "By Hand")
                {
                    txtByHand.Text = dt.Rows[0]["VehicalNo"].ToString();
                    txtByHand.Visible = true;
                }
                else if (txttransportMode.Text == "By Air")
                {
                    txtByAir.Text = dt.Rows[0]["VehicalNo"].ToString();
                    txtByAir.Visible = true;
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please try again leter....!');", true);
        }
    }

    protected void getParticularsdts(string id)
    {
        try
        {
            DataTable Dtproduct = new DataTable();
            SqlDataAdapter daa = new SqlDataAdapter("select * from tblCreditDebitNoteDtls where HeaderID='" + id + "'", con);
            daa.Fill(Dtproduct);
            ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;

            DataTable dt = ViewState["ParticularDetails"] as DataTable;

            if (Dtproduct.Rows.Count > 0)
            {
                for (int i = 0; i < Dtproduct.Rows.Count; i++)
                {

                    dt.Rows.Add(ViewState["RowNo"], Dtproduct.Rows[i]["InvoiceNo"].ToString(), Dtproduct.Rows[i]["Particulars"].ToString(), Dtproduct.Rows[i]["Description"].ToString(), Dtproduct.Rows[i]["HSN"].ToString(), Dtproduct.Rows[i]["Qty"].ToString(),
                        Dtproduct.Rows[i]["Rate"].ToString(), Dtproduct.Rows[i]["Amount"].ToString(), Dtproduct.Rows[i]["UOM"].ToString(), Dtproduct.Rows[i]["CGSTPer"].ToString(), Dtproduct.Rows[i]["CGSTAmt"].ToString(),
                        Dtproduct.Rows[i]["SGSTPer"].ToString(), Dtproduct.Rows[i]["SGSTAmt"].ToString(), Dtproduct.Rows[i]["IGSTPer"].ToString(), Dtproduct.Rows[i]["IGSTAmt"].ToString(),
                        Dtproduct.Rows[i]["Total"].ToString(), Dtproduct.Rows[i]["Discount"].ToString(), Dtproduct.Rows[i]["Remarks"].ToString());
                    ViewState["ParticularDetails"] = dt;
                }
            }
            dgvParticularsDetails.DataSource = dt;
            dgvParticularsDetails.DataBind();

            //changes on 6/12/2024
            DataTable dtt = new DataTable();
            SqlDataAdapter sad3 = new SqlDataAdapter("select ID,Particular,ComponentName,Batch,Quantity from tbl_CrditDebitSaleComponents where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "'", con);
            sad3.Fill(dtt);
            if (dtt.Rows.Count > 0)
            {
                ViewState["gvcomponent"] = dtt;
                gvcomponent.DataSource = dtt;
                gvcomponent.DataBind();
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please try again leter....!');", true);
        }
    }

    static string UpdateHistorymsg = string.Empty;

    protected string GenerateComCode_CR()
    {

        string invoiceno;
        DateTime date = DateTime.Now;
        string currentyeaar = date.ToString();

        string FinYear = null;

        if (DateTime.Today.Month > 3)
        {
            //FinYear = DateTime.Today.Year.ToString();
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();
        }


        string previousyear = (Convert.ToDecimal(FinYear) - 1).ToString();

        //SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tblCreditDebitNoteHdr] with (nolock) where NoteType='"+ ddlNoteType.SelectedValue+ "'", con);
        SqlDataAdapter ad = new SqlDataAdapter("SELECT ID_Max as maxid FROM [tblCreditDebitNote_ID] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            invoiceno = previousyear.ToString() + "-" + FinYear + "/" + (maxid + 1).ToString();
            if (maxid < 9)
            {
                invoiceno = "CRS/" + previousyear.ToString() + "-" + FinYear + "/" + "000" + (maxid + 1).ToString();
                //invoiceno = "CRS/" + "23-24"+ "/" + "000" + (maxid + 1).ToString();
            }
            else if (maxid <= 100)
            {
                invoiceno = "CRS/" + previousyear.ToString() + "-" + FinYear + "/" + "00" + (maxid + 1).ToString();
                //invoiceno = "CRS/" + "23-24" + "/" + "00" + (maxid + 1).ToString();
            }

        }
        else
        {
            invoiceno = string.Empty;
        }
        txtDocNo.Text = invoiceno;
        return invoiceno;

    }

    protected string GenerateComCode_DB()
    {

        string invoiceno;
        DateTime date = DateTime.Now;
        string currentyeaar = date.ToString();

        string FinYear = null;

        if (DateTime.Today.Month > 3)
        {
            //FinYear = DateTime.Today.Year.ToString();
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();
        }


        string previousyear = (Convert.ToDecimal(FinYear) - 1).ToString();

        //SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tblCreditDebitNoteHdr] with (nolock) where NoteType='"+ ddlNoteType.SelectedValue+ "'", con);
        SqlDataAdapter ad = new SqlDataAdapter("SELECT ID_Max as maxid FROM [tblCreditDebitNote_ID] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            invoiceno = previousyear.ToString() + "-" + FinYear + "/" + (maxid + 1).ToString();
            if (maxid < 9)
            {
                invoiceno = "DBS/" + previousyear.ToString() + "-" + FinYear + "/" + "000" + (maxid + 1).ToString();
            }
            else if (maxid <= 100)
            {
                invoiceno = "DBS/" + previousyear.ToString() + "-" + FinYear + "/" + "00" + (maxid + 1).ToString();
            }

        }
        else
        {
            invoiceno = string.Empty;
        }
        txtDocNo.Text = invoiceno;
        return invoiceno;

    }

    bool flgs;
    protected void btnadd_Click(object sender, EventArgs e)
    {
        #region Insert
        if (btnadd.Text == "Submit")
        {
            if (dgvAutomatic.Rows.Count > 0)
            {
                foreach (GridViewRow g2 in dgvAutomatic.Rows)
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
                //string DocNo = GenerateComCode();
                if (ddlNoteType.SelectedValue == "Select" && txtDocNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Kindly Select Note Type..!!');", true);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("SP_CreditDebitNote", con);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "insert");
                    cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
                    cmd.Parameters.AddWithValue("@DocNo", txtDocNo.Text);
                    cmd.Parameters.AddWithValue("@DocDate", txtDocdate.Text);

                    //cmd.Parameters.AddWithValue("@Process", ddlProcess.Text.Trim());
                    cmd.Parameters.AddWithValue("@NoteType", ddlNoteType.Text.Trim());
                    cmd.Parameters.AddWithValue("@CategoryName", txtcategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@BillNumber", ddlBillNumber.SelectedItem.Text == "--Select Bill Number--" ? "" : ddlBillNumber.SelectedItem.Text.Trim());
                    cmd.Parameters.AddWithValue("@BillDate", txtBillDate.Text.Trim());
                    cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text);
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
                    cmd.Parameters.AddWithValue("@GrandTotal", txtFGrandTot.Text);
                    //cmd.Parameters.AddWithValue("@TransportationCharge", txtTCharge.Text);
                    //cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text);
                    //cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text);
                    //cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text);
                    //cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text);
                    //cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text);
                    //cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text);
                    //cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text);
                    cmd.Parameters.AddWithValue("@NoteFor", "Sale");
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());

                    //new Basic & TCS Addition 26-12-23 Start
                    cmd.Parameters.AddWithValue("@SumOfProductAmt", sumofAmount.Text);
                    cmd.Parameters.AddWithValue("@ChargesDescription", TxtChargesDesc.Text);
                    cmd.Parameters.AddWithValue("@HSN", txthsntcs.Text);
                    cmd.Parameters.AddWithValue("@Rate", txtrateTcs.Text);
                    cmd.Parameters.AddWithValue("@Basic", txtBasic.Text);
                    cmd.Parameters.AddWithValue("@CGST", CGSTPertcs.Text);
                    cmd.Parameters.AddWithValue("@SGST", SGSTPertcs.Text);
                    cmd.Parameters.AddWithValue("@IGST", IGSTPertcs.Text);
                    cmd.Parameters.AddWithValue("@Cost", txtCost.Text);
                    //new Basic & TCS Addition 26-12-23 End

                    //New Details For E-Invoice Start
                    cmd.Parameters.AddWithValue("@ShortBAddress", txtshortBillingaddress.Text);
                    cmd.Parameters.AddWithValue("@ShortSAddress", txtshortShippingaddress.Text);
                    cmd.Parameters.AddWithValue("@ShippingCustomer", txtshippingcustomer.Text);
                    cmd.Parameters.AddWithValue("@ShippingAddress", ddlShippingaddress.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@TransportMode", txttransportMode.Text);
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
                    cmd.Parameters.AddWithValue("@BillingAddress", ddlBillAddress.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@BillingLocation", txtbillinglocation.Text);
                    cmd.Parameters.AddWithValue("@ShippingLocation", txtshippinglocation.Text);
                    cmd.Parameters.AddWithValue("@BillingGST", txtbillingGST.Text);
                    cmd.Parameters.AddWithValue("@ShippingGST", txtshippingGST.Text);
                    cmd.Parameters.AddWithValue("@BillingPincode", txtbillingPincode.Text);
                    cmd.Parameters.AddWithValue("@ShippingPincode", txtshippingPincode.Text);
                    cmd.Parameters.AddWithValue("@BillingStatecode", txtbillingstatecode.Text);
                    cmd.Parameters.AddWithValue("@ShippingStatecode", txtshippingstatecode.Text);

                    int a = 0;
                    con.Open();
                    a = cmd.ExecuteNonQuery();
                    con.Close();

                    ////////// Provide Max id from Max_iD from new table - 18/2/23
                    SqlCommand cmdmax = new SqlCommand("SELECT max([Id]) as maxid FROM [tblCreditDebitNoteHdr] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
                    con.Open();
                    Object mx = cmdmax.ExecuteScalar();
                    con.Close();
                    int MaxId = Convert.ToInt32(mx.ToString());

                    //DataTable dt = new DataTable();
                    //ad.Fill(dt);
                    //if (dt.Rows.Count > 0)
                    //{

                    //    int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
                    //    invoiceno = previousyear.ToString() + "-" + FinYear + "/" + (maxid + 1).ToString();
                    //}


                    //SqlCommand cmdmax = new SqlCommand("select MAX(Id) as MAxID from tblCreditDebitNoteHdr", con);
                    //con.Open();
                    //Object mx = cmdmax.ExecuteScalar();
                    //con.Close();
                    //int MaxId = Convert.ToInt32(mx.ToString());


                    if (dgvAutomatic.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvAutomatic.Rows)
                        {
                            bool chkrow = ((CheckBox)row.FindControl("chkSelect")).Checked;
                            string InvoiceNo = ((Label)row.FindControl("lblinvoice")).Text;
                            string Particulars = ((Label)row.FindControl("txtParticulars")).Text;
                            string Description = ((TextBox)row.FindControl("txtDescription")).Text;
                            string HSN = ((TextBox)row.FindControl("txtHSN")).Text;
                            string Qty = ((TextBox)row.FindControl("txtautQty")).Text;
                            string Rate = ((TextBox)row.FindControl("txtRate")).Text;
                            string Amount = ((Label)row.FindControl("txtAmount")).Text;
                            string DiscPer = ((TextBox)row.FindControl("txtDiscPer")).Text;
                            string txtUOM = ((TextBox)row.FindControl("txtUOM")).Text;
                            string CGSTPer = ((TextBox)row.FindControl("txtCGST")).Text;
                            string SGSTPer = ((TextBox)row.FindControl("txtSGST")).Text;
                            string IGSTPer = ((TextBox)row.FindControl("txtIGST")).Text;
                            string TotalAmount = ((TextBox)row.FindControl("txtGrandTotal")).Text;
                            string Remarks = ((TextBox)row.FindControl("txtremarks")).Text;

                            var CGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(CGSTPer.Trim())) / 100;
                            var SGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(SGSTPer.Trim())) / 100;
                            var IGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(IGSTPer.Trim())) / 100;

                            SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount],[UOM],[Remarks])
                            VALUES(" + MaxId + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                             "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                             "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + InvoiceNo + "','" + DiscPer + "','" + txtUOM + "','" + Remarks + "')", con);
                            con.Open();
                            if (chkrow == true)
                            {
                                cmdParticulardata.ExecuteNonQuery();

                            }
                            con.Close();
                        }
                        //DataTable dt546665 = new DataTable();
                        //SqlDataAdapter sadparticular = new SqlDataAdapter("select * from tblSaleCreditDebitNoteDtls where HeaderID='" + MaxId + "'", con);
                        //sadparticular.Fill(dt546665);
                        //if (dt546665.Rows.Count > 0)
                        //{

                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('At Least Select One Record.');", true);
                        //}

                    }
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

                            SqlDataAdapter sad2 = new SqlDataAdapter("select * from tbl_OutwardEntryComponentsDtls where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "' AND ComponentName='" + lblCompo + "'", con);
                            DataTable Dt1 = new DataTable();
                            sad2.Fill(Dt1);
                            con.Close(); // Close the connection when done
                            if (Dt1.Rows.Count > 0)
                            {
                                lblUnit = Dt1.Rows[0]["Units"].ToString();
                                lblDescription = Dt1.Rows[0]["Description"].ToString();
                                lblhsn = Dt1.Rows[0]["HSN"].ToString();
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
                            SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_CrditDebitSaleComponents] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@OrderNo", ddlBillNumber.SelectedItem.Text);
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
                            GetInventoryCalculation(lblCompo, lblQuantity, Product, lblbatch);
                        }
                    }

                    // Update ID in CreditdebitNoteId Table For Increment value- 18/2/2023
                    SqlCommand cmdmaxx = new SqlCommand("SELECT ID_Max as maxid FROM [tblCreditDebitNote_ID] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
                    con.Open();
                    Object mxx = cmdmaxx.ExecuteScalar();
                    con.Close();
                    int MaxIdd = 0;
                    if (mxx == null)
                    {
                        MaxIdd = 0;
                    }
                    else
                    {
                        MaxIdd = Convert.ToInt32(mxx.ToString()) + 1;
                    }

                    SqlCommand cmdUpdateId = new SqlCommand(@"Update tblCreditDebitNote_ID Set ID_Max='" + MaxIdd + "' where NoteType='" + ddlNoteType.SelectedValue + "'", con);
                    con.Open();
                    cmdUpdateId.ExecuteNonQuery();
                    con.Close();

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='CreditDebitNoteSaleList.aspx';", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Select At Least One Record.');", true);

            }
        }
        #endregion Insert

        #region Update
        if (btnadd.Text == "Update")
        {
            SqlCommand cmd = new SqlCommand("SP_CreditDebitNote", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "update");
            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(ViewState["UpdateRowId"].ToString()));
            cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
            cmd.Parameters.AddWithValue("@DocNo", txtDocNo.Text.Trim());
            cmd.Parameters.AddWithValue("@DocDate", txtDocdate.Text.Trim());
            // cmd.Parameters.AddWithValue("@Process", ddlProcess.Text.Trim());
            cmd.Parameters.AddWithValue("@NoteType", ddlNoteType.Text.Trim());
            cmd.Parameters.AddWithValue("@CategoryName", txtcategory.Text.Trim());
            cmd.Parameters.AddWithValue("@BillNumber", ddlBillNumber.SelectedItem.Text == "--Select Bill Number--" ? "" : ddlBillNumber.SelectedItem.Text.Trim());
            cmd.Parameters.AddWithValue("@BillDate", txtBillDate.Text.Trim());
            cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text.Trim());

            //cmd.Parameters.AddWithValue("@TransportationCharge", txtTCharge.Text);
            //cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text);
            //cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text);
            //cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text);
            //cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text);
            //cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text);
            //cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text);
            //cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text);

            //new Basic & TCS Addition 26-12-23 Start
            cmd.Parameters.AddWithValue("@SumOfProductAmt", sumofAmount.Text);
            cmd.Parameters.AddWithValue("@ChargesDescription", TxtChargesDesc.Text);
            cmd.Parameters.AddWithValue("@HSN", txthsntcs.Text);
            cmd.Parameters.AddWithValue("@Rate", txtrateTcs.Text);
            cmd.Parameters.AddWithValue("@Basic", txtBasic.Text);
            cmd.Parameters.AddWithValue("@CGST", CGSTPertcs.Text);
            cmd.Parameters.AddWithValue("@SGST", SGSTPertcs.Text);
            cmd.Parameters.AddWithValue("@IGST", IGSTPertcs.Text);
            cmd.Parameters.AddWithValue("@Cost", txtCost.Text);
            //new Basic & TCS Addition 26-12-23 End

            cmd.Parameters.AddWithValue("@NoteFor", "Sale");
            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
            cmd.Parameters.AddWithValue("@GrandTotal", txtFGrandTot.Text);
            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());

            //New Details For E-Invoice Start
            cmd.Parameters.AddWithValue("@ShortBAddress", txtshortBillingaddress.Text);
            cmd.Parameters.AddWithValue("@ShortSAddress", txtshortShippingaddress.Text);
            cmd.Parameters.AddWithValue("@ShippingCustomer", txtshippingcustomer.Text);
            cmd.Parameters.AddWithValue("@ShippingAddress", ddlShippingaddress.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@TransportMode", txttransportMode.Text);
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
            cmd.Parameters.AddWithValue("@BillingAddress", ddlBillAddress.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@BillingLocation", txtbillinglocation.Text);
            cmd.Parameters.AddWithValue("@ShippingLocation", txtshippinglocation.Text);
            cmd.Parameters.AddWithValue("@BillingGST", txtbillingGST.Text);
            cmd.Parameters.AddWithValue("@ShippingGST", txtshippingGST.Text);
            cmd.Parameters.AddWithValue("@BillingPincode", txtbillingPincode.Text);
            cmd.Parameters.AddWithValue("@ShippingPincode", txtshippingPincode.Text);
            cmd.Parameters.AddWithValue("@BillingStatecode", txtbillingstatecode.Text);
            cmd.Parameters.AddWithValue("@ShippingStatecode", txtshippingstatecode.Text);
            //New Details For E-Invoice End

            int a = 0;
            cmd.Connection.Open();
            a = cmd.ExecuteNonQuery();
            cmd.Connection.Close();





            if (dgvParticularsDetails.Rows.Count > 0)
            {
                SqlCommand cmddelete = new SqlCommand("delete from tblCreditDebitNoteDtls where HeaderID='" + Convert.ToInt32(ViewState["UpdateRowId"].ToString()) + "'", con);
                con.Open();
                cmddelete.ExecuteNonQuery();
                con.Close();

                foreach (GridViewRow row in dgvParticularsDetails.Rows)
                {
                    string InvoiceNo = ((Label)row.FindControl("lblinvoice")).Text;
                    string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
                    string Description = ((Label)row.FindControl("lblDescription")).Text;
                    string HSN = ((Label)row.FindControl("lblHSN")).Text;
                    string Qty = ((Label)row.FindControl("lblQty")).Text;
                    string Rate = ((Label)row.FindControl("lblRate")).Text;
                    string Amount = ((Label)row.FindControl("lblAmount")).Text;
                    string disc = ((Label)row.FindControl("lbldisc")).Text;
                    string CGSTPer = ((Label)row.FindControl("lblCGSTPer")).Text;
                    string CGSTAmt = ((Label)row.FindControl("lblCGSTAmt")).Text;
                    string SGSTPer = ((Label)row.FindControl("lblSGSTPer")).Text;
                    string SGSTAmt = ((Label)row.FindControl("lblSGSTAmt")).Text;
                    string IGSTPer = ((Label)row.FindControl("lblIGSTPer")).Text;
                    string IGSTAmt = ((Label)row.FindControl("lblIGSTAmt")).Text;
                    string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;
                    string txtUOM = ((Label)row.FindControl("lblUOM")).Text;
                    string Remarks = ((TextBox)row.FindControl("txtremarks")).Text;

                    SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount],[UOM],[Remarks])
                             VALUES(" + ViewState["UpdateRowId"].ToString() + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                     "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                     "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + InvoiceNo + "','" + disc + "','" + txtUOM + "', '" + Remarks + "')", con);
                    con.Open();
                    cmdParticulardata.ExecuteNonQuery();
                    con.Close();
                }
            }


            //Save Component Details 
            if (gvcomponent.Rows.Count > 0)
            {
                SqlCommand cmddelete1 = new SqlCommand("delete from tbl_CrditDebitSaleComponents where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "'", con);
                con.Open();
                cmddelete1.ExecuteNonQuery();
                con.Close();
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

                    SqlDataAdapter sad2 = new SqlDataAdapter("select * from tbl_OutwardEntryComponentsDtls where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "' AND ComponentName='" + lblCompo + "'", con);
                    DataTable Dt1 = new DataTable();
                    sad2.Fill(Dt1);
                    con.Close(); // Close the connection when done
                    if (Dt1.Rows.Count > 0)
                    {
                        lblUnit = Dt1.Rows[0]["Units"].ToString();
                        lblDescription = Dt1.Rows[0]["Description"].ToString();
                        lblhsn = Dt1.Rows[0]["HSN"].ToString();
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
                    SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_CrditDebitSaleComponents] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                    cmdd.Parameters.AddWithValue("@OrderNo", ddlBillNumber.SelectedItem.Text);
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
                    GetInventoryCalculation(lblCompo, lblQuantity, Product, lblbatch);
                }
            }

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Updated Sucessfully');window.location.href='CreditDebitNoteSaleList.aspx';", true);
        }
        #endregion Update
    }

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreditDebitNote.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierList(string prefixText, int count)
    {
        return AutoFillSupplierName(prefixText);
    }

    public static List<string> AutoFillSupplierName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                //com.CommandText = "Select DISTINCT [SupplierName] from tblSupplierMaster where " + "SupplierName like @Search + '%'";
                com.CommandText = "Select DISTINCT [billingcustomer] from tbltaxinvoicehdr where " + "billingcustomer like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> SupplierNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        SupplierNames.Add(sdr["billingcustomer"].ToString());
                    }
                }
                con.Close();
                return SupplierNames;
            }
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
                com.CommandText = "Select DISTINCT [Companyname] from tbl_CompanyMaster where " + "Companyname like @Search + '%'  AND Isdeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> cname = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        cname.Add(sdr["Companyname"].ToString());
                    }
                }
                con.Close();
                return cname;
            }
        }
    }

    protected void txtSupplierName_TextChanged(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tbl_CompanyMaster where  Companyname='" + txtSupplierName.Text + "'", con);
            sad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
               // Fillddlshippingaddress(txtSupplierName.Text);
                FillddlBillingAddress(txtSupplierName.Text);
                GetInvoice();
                BindBillNO();
                // getOrderDatailsdts();
            }

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }

    }

    protected void BindBillNO()
    {
        string com = "SELECT * FROM tbltaxinvoicehdr where billingcustomer='" + txtSupplierName.Text.Trim() + "' AND isdeleted=0 AND Status>=2";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        ddlBillNumber.DataSource = dt;
        ddlBillNumber.DataBind();
        //ddlBillNumber.DataTextField = "BillNo";
        ddlBillNumber.DataTextField = "Invoiceno";
        ddlBillNumber.DataValueField = "Id";
        ddlBillNumber.DataBind();
        ddlBillNumber.Items.Insert(0, new ListItem("--Select Invoice Number--", "0"));
    }

    protected void Insert(object sender, EventArgs e)
    {
        if (txtQty.Text == "" || txtParticulars.Text == "" || txtTotalamt.Text == "")
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
        try
        {

            ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
            DataTable dt = (DataTable)ViewState["ParticularDetails"];

            dt.Rows.Add(ViewState["RowNo"], ddlinvoice.SelectedItem.Text, txtParticulars.SelectedItem.Text, txtDescription.Text, txtHSN.Text, txtQty.Text, txtRate.Text, txtAmountt.Text, txtUOM.Text, CGSTPer.Text, CGSTAmt.Text, SGSTPer.Text, SGSTAmt.Text, IGSTPer.Text, IGSTAmt.Text, txtTotalamt.Text, txtDisct.Text, txtremarkspar.Text);
            ViewState["ParticularDetails"] = dt;

            dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
            dgvParticularsDetails.DataBind();

            txtParticulars.SelectedItem.Text = "--Select--";
            ddlinvoice.ClearSelection();
            txtDescription.Text = string.Empty;
            txtQty.Text = string.Empty;
            txtHSN.Text = string.Empty;
            txtRate.Text = string.Empty;
            txtAmountt.Text = string.Empty;
            txtUOM.Text = string.Empty;
            CGSTPer.Text = string.Empty;
            CGSTAmt.Text = string.Empty;
            SGSTPer.Text = string.Empty;
            SGSTAmt.Text = string.Empty;
            IGSTPer.Text = string.Empty;
            IGSTAmt.Text = string.Empty;
            txtTotalamt.Text = string.Empty;
            txtDisct.Text = string.Empty;

            txtremarkspar.Text = string.Empty;

        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    protected void dgvParticularsDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void dgvParticularsDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvParticularsDetails.EditIndex = e.NewEditIndex;
        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnUpdate_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string Description = ((TextBox)row.FindControl("txtDescr")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((Label)row.FindControl("lblRate")).Text;
        string Amount = ((Label)row.FindControl("lblAmount")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGSTAmt")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGSTAmt")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGSTAmt")).Text;
        string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;
        string disc = ((TextBox)row.FindControl("txtdisc")).Text;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;

        Dt.Rows[row.RowIndex]["Particulars"] = Particulars;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Qty"] = Qty;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Amount"] = Amount;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["TotalAmount"] = TotalAmount;
        Dt.Rows[row.RowIndex]["Discount"] = disc;

        Dt.AcceptChanges();

        ViewState["ParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();

        //foreach (GridViewRow gv in dgvParticularsDetails.Rows)
        //{
        //    Label GrandTotal = (Label)gv.FindControl("lblTotalAmount");
        //    Grandtotal2 += Convert.ToDouble(GrandTotal.Text);
        //    hidden1.Value = Grandtotal2.ToString();
        //}

        //txtFGrandTot.Text = Convert.ToDouble(txtTCost.Text) + Grandtotal2.ToString();
        //double hiddenValue = Convert.ToDouble(hidden1.Value);
        //double sum = Convert.ToDouble(txtTCost.Text) + hiddenValue;
        //double sum = hiddenValue;
        // txtFGrandTot.Text = sum.ToString();

        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable dt = ViewState["ParticularDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["ParticularDetails"] = dt;
        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void lnkCancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;
        dgvParticularsDetails.EditIndex = -1;

        ViewState["ParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    //public static List<string> GetItemList(string prefixText, int count)
    //{
    //    return AutoFilItem(prefixText);
    //}

    //public static List<string> AutoFilItem(string prefixText)
    //{
    //    using (SqlConnection con = new SqlConnection())
    //    {
    //        con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

    //        using (SqlCommand com = new SqlCommand())
    //        {
    //            com.CommandText = "Select DISTINCT [ItemName] from tblItemMaster where " + "ItemName like @Search + '%'";

    //            com.Parameters.AddWithValue("@Search", prefixText);
    //            com.Connection = con;
    //            con.Open();
    //            List<string> Items = new List<string>();
    //            using (SqlDataReader sdr = com.ExecuteReader())
    //            {
    //                while (sdr.Read())
    //                {
    //                    Items.Add(sdr["ItemName"].ToString());
    //                }
    //            }
    //            con.Close();
    //            return Items;
    //        }
    //    }
    //}

    protected void txtParticulars_TextChanged(object sender, EventArgs e)
    {
        //SqlDataAdapter ad = new SqlDataAdapter("SELECT HSN,PurchaseRate FROM tblItemMaster where ItemName='" + txtParticulars.Text.Trim() + "' ", con);
        //DataTable dt = new DataTable();
        //ad.Fill(dt);
        //if (dt.Rows.Count > 0)
        //{
        //    txtHSN.Text = dt.Rows[0]["HSN"].ToString() == "" ? "0" : dt.Rows[0]["HSN"].ToString();
        //    txtRate.Text = dt.Rows[0]["PurchaseRate"].ToString() == "" ? "0" : dt.Rows[0]["PurchaseRate"].ToString();
        //}
        //else
        //{

        //}
        if (txtParticulars.SelectedItem.Text == "Scrap Material")
        {
            txtHSN.Text = "7204";
            txtDescription.Text = txtParticulars.SelectedItem.Text;
        }
        else
        {
            txtHSN.Text = "85381010";
            txtDescription.Text = txtParticulars.SelectedItem.Text;
        }
    }

    private void GST_Calculation()
    {
        var TotalAmt = Convert.ToDecimal(txtQty.Text.Trim()) * Convert.ToDecimal(txtRate.Text == "" ? "0" : txtRate.Text);

        decimal AmtwithDisc;
        if (txtDisct.Text != "0")
        {
            var disAmt = TotalAmt * Convert.ToDecimal(txtDisct.Text == "" ? "0" : txtDisct.Text) / 100;
            AmtwithDisc = TotalAmt - disAmt;
        }
        else
        {
            AmtwithDisc = TotalAmt;
        }

        txtAmountt.Text = AmtwithDisc.ToString();

        decimal CGST;
        if (string.IsNullOrEmpty(CGSTPer.Text))
        {
            CGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(AmtwithDisc);
            decimal Val2 = Convert.ToDecimal(CGSTPer.Text);

            CGST = (Val1 * Val2 / 100);
        }
        CGSTAmt.Text = CGST.ToString();

        decimal SGST;
        if (string.IsNullOrEmpty(SGSTPer.Text))
        {
            SGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(AmtwithDisc);
            decimal Val2 = Convert.ToDecimal(SGSTPer.Text);

            SGST = (Val1 * Val2 / 100);
        }
        SGSTAmt.Text = SGST.ToString();


        decimal IGST;
        if (string.IsNullOrEmpty(IGSTPer.Text))
        {
            IGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(AmtwithDisc);
            decimal Val2 = Convert.ToDecimal(IGSTPer.Text);

            IGST = (Val1 * Val2 / 100);
        }
        IGSTAmt.Text = IGST.ToString();

        decimal GSTTotal = 0;
        if (IGSTPer.Text != "0")
        {
            GSTTotal = IGST;
        }
        else
        {
            GSTTotal = CGST + SGST;
        }


        txtTotalamt.Text = (AmtwithDisc + GSTTotal).ToString();
    }

    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void IGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();

        if (IGSTPer.Text == "" || IGSTPer.Text == "0")
        {
            SGSTPer.Enabled = true;
            CGSTPer.Enabled = true;
            SGSTPer.Text = "0";
            CGSTPer.Text = "0";
        }
        else
        {
            SGSTPer.Enabled = false;
            CGSTPer.Enabled = false;
            SGSTPer.Text = "0";
            CGSTPer.Text = "0";
        }
    }

    protected void SGSTPer_TextChanged(object sender, EventArgs e)
    {
        CGSTPer.Text = SGSTPer.Text;
        SGSTAmt.Text = CGSTAmt.Text;

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
        GST_Calculation();
    }

    protected void CGSTPer_TextChanged(object sender, EventArgs e)
    {
        SGSTPer.Text = CGSTPer.Text;
        SGSTAmt.Text = CGSTAmt.Text;


        if (CGSTPer.Text == "" || CGSTPer.Text == "0")
        {
            IGSTPer.Enabled = true;
            IGSTPer.Text = "0";
        }
        else
        {
            IGSTPer.Enabled = false;
            IGSTPer.Text = "0";
        }
        GST_Calculation();
    }
    Double Grandtotal2 = 0;
    private void GRID_GST_Calculation(GridViewRow row)
    {
        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((Label)row.FindControl("lblRate")).Text;
        string Amount = ((Label)row.FindControl("lblAmount")).Text;
        Label lblamt = (Label)row.FindControl("lblAmount");
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        TextBox CGSTAmt = (TextBox)row.FindControl("txtCGSTAmt");
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        TextBox SGSTAmt = (TextBox)row.FindControl("txtSGSTAmt");
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        TextBox IGSTAmt = (TextBox)row.FindControl("txtIGSTAmt");
        Label TotalAmount = (Label)row.FindControl("lblTotalAmount");
        TextBox disc = (TextBox)row.FindControl("txtdisc");

        var totalamt = Convert.ToDecimal(Qty) * Convert.ToDecimal(Rate);
        decimal AmtwithDisc;
        if (disc.Text != "0")
        {
            var discamt = totalamt * Convert.ToDecimal(disc.Text) / 100;
            AmtwithDisc = totalamt - discamt;
        }
        else
        {
            AmtwithDisc = totalamt;
        }
        lblamt.Text = AmtwithDisc.ToString();

        decimal Vcgst;
        if (string.IsNullOrEmpty(CGSTAmt.Text))
        {
            Vcgst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(AmtwithDisc);
            decimal val2 = Convert.ToDecimal(CGSTPer == "" ? "0" : CGSTPer);

            Vcgst = (val1 * val2 / 100);
        }
        CGSTAmt.Text = Vcgst.ToString();

        decimal Vsgst;
        if (string.IsNullOrEmpty(SGSTAmt.Text))
        {
            Vsgst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(AmtwithDisc);
            decimal val2 = Convert.ToDecimal(SGSTPer == "" ? "0" : SGSTPer);

            Vsgst = (val1 * val2 / 100);
        }
        SGSTAmt.Text = Vsgst.ToString();

        decimal Vigst;
        if (string.IsNullOrEmpty(IGSTAmt.Text))
        {
            Vigst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(AmtwithDisc);
            decimal val2 = Convert.ToDecimal(IGSTPer == "" ? "0" : IGSTPer);

            Vigst = (val1 * val2 / 100);
        }
        IGSTAmt.Text = Vigst.ToString();

        decimal GSTTotal;
        if (IGSTPer == "0")
        {
            GSTTotal = Vcgst + Vsgst;
        }
        else
        {
            GSTTotal = Vigst;
        }

        TotalAmount.Text = (AmtwithDisc + GSTTotal).ToString();
    }

    protected void txtQty_TextChanged1(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    Double Totalamt = 0, GrandTotal = 0, TotalG = 0;
    Double GrandTotalamtt = 0;
    protected void dgvParticularsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Totalamt += Convert.ToDouble((e.Row.FindControl("lblAmount") as Label).Text);
            GrandTotalamtt += Convert.ToDouble((e.Row.FindControl("lblTotalAmount") as Label).Text);
            // TotalG = Convert.ToDouble((e.Row.FindControl("lbltotal") as Label).Text);
            hdnGrandtotal.Value = GrandTotalamtt.ToString();
            sumofAmount.Text = Totalamt.ToString();

            var Total = Convert.ToDouble(txtCost.Text) + GrandTotalamtt;
            txtFGrandTot.Text = Total.ToString("##.00");

        }


        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    Totalamt += Convert.ToDouble((e.Row.FindControl("lblTotalAmount") as Label).Text);
        //    hdnGrandtotal.Value = Totalamt.ToString();
        //    //GrandTotal = Convert.ToDouble(Totalamt.ToString()) + Convert.ToDouble(txtTCost.Text);
        //    GrandTotal = Convert.ToDouble(Totalamt.ToString());
        //    txtFGrandTot.Text = GrandTotal.ToString();
        //}
        //if (e.Row.RowType == DataControlRowType.Footer)
        //{
        //    (e.Row.FindControl("lbltotal") as Label).Text = Totalamt.ToString();
        //}
    }

    protected void ddlBillNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        //string com = "SELECT * FROM tblPurchaseBillHdr where BillNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "'";
        string com = "SELECT * FROM tbltaxinvoicehdr where Invoiceno='" + ddlBillNumber.SelectedItem.Text.Trim() + "'";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            //string str = dt.Rows[0]["Invoicedate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time = Convert.ToDateTime(str);
            txtBillDate.Text = dt.Rows[0]["Invoicedate"].ToString();
        }

        getOrderDatailsdts();

        //changes on 6/12/2024
        DataTable dtt = new DataTable();
        SqlDataAdapter sad3 = new SqlDataAdapter("select ID,Particular,ComponentName,Batch,Quantity from tbl_OutwardEntryComponentsDtls where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "'", con);
        sad3.Fill(dtt);
        if (dtt.Rows.Count > 0)
        {
            ViewState["gvcomponent"] = dtt;
            gvcomponent.DataSource = dtt;
            gvcomponent.DataBind();
        }
    }



    protected void getOrderDatailsdts()
    {
        try
        {
            string ID = ddlBillNumber.SelectedValue;
            //SqlDataAdapter ad = new SqlDataAdapter("select * from tblPurchaseBillDtls where HeaderID='" + ID.Trim() + "' ", con);
            //SqlDataAdapter ad = new SqlDataAdapter("select * from tblTaxInvoiceDtls where HeaderID='" + ID.Trim() + "' ", con);
            SqlDataAdapter ad = new SqlDataAdapter("select * from tbltaxinvoicehdr as Hdr Inner Join  tblTaxInvoiceDtls As dtl  on Hdr.Id = dtl.HeaderID where  Hdr.Id='" + ID + "' ", con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                dgvAutomatic.DataSource = dt;
                dgvAutomatic.DataBind();
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Invoice Details Not Found !!');", true);
            }
        }
        catch (Exception ex)
        {


        }
    }

    protected void txtautQty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    Double FinalGeandTotal = 0;
    Double FinalBasicTotal = 0;
    private void calculationA(GridViewRow row)
    {
        foreach (GridViewRow gv in dgvAutomatic.Rows)
        {
            CheckBox chkrow = (CheckBox)gv.FindControl("chkSelect");
            TextBox txt_Qty = (TextBox)gv.FindControl("txtautQty");
            TextBox txt_price = (TextBox)gv.FindControl("txtRate");
            TextBox txt_CGST = (TextBox)gv.FindControl("txtCGST");
            TextBox txt_SGST = (TextBox)gv.FindControl("txtSGST");
            TextBox txt_IGST = (TextBox)gv.FindControl("txtIGST");
            TextBox txt_amount = (TextBox)gv.FindControl("txtGrandTotal");
            TextBox txtDiscPer = (TextBox)gv.FindControl("txtDiscPer");
            Label Amount = (Label)gv.FindControl("txtAmount");

            var totalamt = Convert.ToDecimal(txt_Qty.Text.Trim()) * Convert.ToDecimal(txt_price.Text.Trim());
            decimal AmtWithDiscount;
            if (txtDiscPer.Text != "" || txtDiscPer.Text != null)
            {
                var disc = totalamt * (Convert.ToDecimal(txtDiscPer.Text.Trim())) / 100;
                AmtWithDiscount = totalamt - disc;
            }
            else
            {
                AmtWithDiscount = totalamt;
            }
            Amount.Text = AmtWithDiscount.ToString();
            //sumofAmount.Text = AmtWithDiscount.ToString();


            var CGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_CGST.Text.Trim())) / 100;
            var SGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_SGST.Text.Trim())) / 100;
            var IGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_IGST.Text.Trim())) / 100;

            decimal GSTtotal;
            if (txt_IGST.Text == "0")
            {
                GSTtotal = SGSTamt + CGSTamt;
            }
            else
            {
                GSTtotal = IGSTamt;
            }

            var NetAmt = AmtWithDiscount + GSTtotal;

            txt_amount.Text = NetAmt.ToString("##.00");
            if (chkrow.Checked == true)
            {
                FinalGeandTotal += Convert.ToDouble(txt_amount.Text);
                FinalBasicTotal += Convert.ToDouble(Amount.Text);
            }
        }
        //Double GrantotalFinal = Convert.ToDouble(FinalGeandTotal.ToString()) + Convert.ToDouble(txtTCost.Text);
        Double GrantotalFinal = Convert.ToDouble(FinalGeandTotal.ToString());
        txtFGrandTot.Text = GrantotalFinal.ToString();
        hdnGrandtotal.Value = GrantotalFinal.ToString();

        Double BasicFinal = Convert.ToDouble(FinalBasicTotal.ToString());
        sumofAmount.Text = BasicFinal.ToString();
        hdnBasictotal.Value = BasicFinal.ToString();
    }

    protected void txtCGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtSGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtIGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    decimal GrandTotalamt = 0;
    protected void dgvAutomatic_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            GrandTotalamt += Convert.ToDecimal((e.Row.FindControl("txtGrandTotal") as TextBox).Text);
            hdnGrandtotal.Value = GrandTotalamt.ToString();
            txtFGrandTot.Text = GrandTotalamt.ToString();
            TextBox CGST = (TextBox)e.Row.FindControl("txtCGST");
            TextBox SGST = (TextBox)e.Row.FindControl("txtSGST");
            TextBox IGST = (TextBox)e.Row.FindControl("txtIGST");

            //if (IGST.Text == "0")
            //{
            //    txtTCGSTPer.Text = CGST.Text;
            //    txtTSGSTPer.Text = SGST.Text;
            //    txtTIGSTPer.Enabled = false;
            //}
            //else
            //{
            //    txtTCGSTPer.Enabled = false;
            //    txtTSGSTPer.Enabled = false;
            //    txtTIGSTPer.Text = IGST.Text;
            //}


            TextBox txt_Qty = (TextBox)e.Row.FindControl("txtautQty");
            CheckBox chk = (CheckBox)e.Row.FindControl("chkSelect");
            CheckBox chkheader = (CheckBox)dgvAutomatic.HeaderRow.FindControl("chkHeader");
            int id = Convert.ToInt32(dgvAutomatic.DataKeys[e.Row.RowIndex].Values[0]);
            int sumofqty;
            DataTable dtinvoice = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tblCreditDebitNoteHdr where BillNumber='" + ddlBillNumber.SelectedItem.Text + "'", con);
            //SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM tbltaxinvoicehdr AS Hdr INNER JOIN tblTaxInvoiceDtls AS dtl ON Hdr.Id = dtl.HeaderID WHERE BillingCustomer='" + txtSupplierName.Text + "' GROUP BY Hdr.InvoiceNo", con);


            sad.Fill(dtinvoice);
            // string HeaderId = dtinvoice.Rows[0]["Id"].ToString();
            SqlCommand cmdmax = new SqlCommand("SELECT Max(Id) FROM tblCreditDebitNoteDtls where InvoiceNo='" + ddlBillNumber.SelectedItem.Text + "'", con);
            //SqlCommand cmdmax = new SqlCommand("select Max(dtl.Id) from tbltaxinvoicehdr as Hdr Inner Join  tblTaxInvoiceDtls As dtl  on Hdr.Id = dtl.HeaderID where BillingCustomer='" + txtSupplierName.Text + "'GROUP BY Hdr.InvoiceNo", con);
            con.Open();
            Object mxid = cmdmax.ExecuteScalar();
            con.Close();

            //SqlCommand cmdsumQty = new SqlCommand("SELECT SUM(CAST(Qty as int)) FROM tblCreditDebitNoteDtls where InvoiceNo='" + ddlBillNumber.SelectedItem.Text + "'", con);
            SqlCommand cmdsumQty = new SqlCommand(
            "SELECT SUM(CAST(dtl.Qty AS decimal(18,2))) " +
            "FROM tbltaxinvoicehdr AS Hdr " +
            "INNER JOIN tblTaxInvoiceDtls AS dtl ON Hdr.Id = dtl.HeaderID " +
            "WHERE Hdr.isdeleted = 0 AND Hdr.BillingCustomer='" + txtSupplierName.Text + "' " +
            "GROUP BY Hdr.InvoiceNo", con);


            con.Open();
            Object smQty = cmdsumQty.ExecuteScalar();
            //sumofqty = Convert.ToInt32(smQty);
            con.Close();

            var mxiddd = mxid.ToString() == "" ? null : mxid.ToString();
            var smquantity = smQty.ToString() == "" ? "0" : smQty.ToString();

            DataTable dtTIDetails = new DataTable();
            SqlDataAdapter saTIDetails = new SqlDataAdapter("select * from tblCreditDebitNoteDtls where InvoiceNo='" + ddlBillNumber.SelectedItem.Text + "' and Id='" + Convert.ToInt32(mxiddd) + "'", con);
            saTIDetails.Fill(dtTIDetails);
            if (dtTIDetails.Rows.Count > 0)
            {
                //var ExistQty = dtTIDetails.Rows[0]["Qty"].ToString();
                //var Qty = txt_Qty.Text;

                var minusQty = Convert.ToInt32(txt_Qty.Text) - Convert.ToInt32(smquantity);
                txt_Qty.Text = minusQty.ToString();
                if (minusQty == 0)
                {
                    chk.Enabled = false;
                    chkheader.Enabled = false;
                }

            }
        }
    }

    Double GrandAmount = 0;
    protected void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow row in dgvAutomatic.Rows)
            {
                TextBox txtRate = (TextBox)row.FindControl("txtRate");
                TextBox txt_Qty = (TextBox)row.FindControl("txtautQty");
                TextBox description = (TextBox)row.FindControl("txtDescription");
                Label Amount = (Label)row.FindControl("txtAmount");
                TextBox CGST = (TextBox)row.FindControl("txtCGST");
                TextBox SGST = (TextBox)row.FindControl("txtSGST");
                TextBox IGST = (TextBox)row.FindControl("txtIGST");
                CheckBox ChkRow = (CheckBox)row.FindControl("chkSelect");
                TextBox GrandTotal = (TextBox)row.FindControl("txtGrandTotal");

                if (ChkRow.Checked == true)
                {

                    txt_Qty.Enabled = true;
                    txtRate.Enabled = true;
                    if (IGST.Text == "" || IGST.Text == "0")
                    {
                        CGST.Enabled = true;
                        SGST.Enabled = true;
                    }
                    else
                    {
                        IGST.Enabled = true;
                    }


                    GrandAmount += Convert.ToDouble(GrandTotal.Text);
                    Totalamt += Convert.ToDouble(Amount.Text);
                    // calculationA(row);
                }
                else
                {
                    CGST.Enabled = false;
                    SGST.Enabled = false;
                    IGST.Enabled = false;
                    txt_Qty.Enabled = false;
                }
            }
            DivAutomatic.Visible = true;
            sumofAmount.Text = Totalamt.ToString();
            //Double Geandtotal = Convert.ToDouble(GrandAmount.ToString()) + Convert.ToDouble(txtTCost.Text);
            Double Geandtotal = Convert.ToDouble(GrandAmount.ToString());
            txtFGrandTot.Text = Geandtotal.ToString();
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void chkHeader_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow row in dgvAutomatic.Rows)
            {
                TextBox txtRate = (TextBox)row.FindControl("txtRate");
                TextBox txt_Qty = (TextBox)row.FindControl("txtautQty");
                TextBox description = (TextBox)row.FindControl("txtDescription");
                TextBox CGST = (TextBox)row.FindControl("txtCGST");
                TextBox SGST = (TextBox)row.FindControl("txtSGST");
                TextBox IGST = (TextBox)row.FindControl("txtIGST");
                Label Amount = (Label)row.FindControl("txtAmount");
                CheckBox ChkRow = (CheckBox)row.FindControl("chkSelect");
                CheckBox ChkHeader = (CheckBox)dgvAutomatic.HeaderRow.FindControl("chkHeader");
                TextBox GrandTotal = (TextBox)row.FindControl("txtGrandTotal");

                if (ChkHeader.Checked == true)
                {
                    ChkRow.Checked = true;
                    txt_Qty.Enabled = true;
                    txtRate.Enabled = true;
                    //CGST.Enabled = true;
                    //SGST.Enabled = true;
                    //IGST.Enabled = true;
                    if (IGST.Text == "" || IGST.Text == "0")
                    {
                        CGST.Enabled = true;
                        SGST.Enabled = true;
                    }
                    else
                    {
                        IGST.Enabled = true;
                    }
                    GrandAmount += Convert.ToDouble(GrandTotal.Text);
                    Totalamt += Convert.ToDouble(Amount.Text);
                    // calculationA(row);
                }
                else
                {
                    ChkRow.Checked = false;
                    CGST.Enabled = false;
                    SGST.Enabled = false;
                    IGST.Enabled = false;
                    txt_Qty.Enabled = false;
                }
            }
            DivAutomatic.Visible = true;
            sumofAmount.Text = Totalamt.ToString();
            //Double Geandtotal = Convert.ToDouble(GrandAmount.ToString()) + Convert.ToDouble(txtTCost.Text);
            Double Geandtotal = Convert.ToDouble(GrandAmount.ToString());
            txtFGrandTot.Text = Geandtotal.ToString();
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }


    }
    Double GrandAmount1 = 0, GAmount = 0;


    //protected void txtTCharge_TextChanged(object sender, EventArgs e)
    //{
    //    Transportation_Calculation();
    //}

    //protected void txtTCGSTPer_TextChanged(object sender, EventArgs e)
    //{
    //    Transportation_Calculation();
    //}

    //protected void txtTSGSTPer_TextChanged(object sender, EventArgs e)
    //{
    //    Transportation_Calculation();
    //}

    //protected void txtTIGSTPer_TextChanged(object sender, EventArgs e)
    //{
    //    Transportation_Calculation();
    //}

    protected void txtPaymentDueDate_TextChanged(object sender, EventArgs e)
    {
        //DateTime fromdate = DateTime.Parse(Convert.ToDateTime(txtDocdate.Text).ToShortDateString());
        //DateTime todate = DateTime.Parse(Convert.ToDateTime(txtPaymentDueDate.Text).ToShortDateString());
        DateTime fromdate = Convert.ToDateTime(txtDocdate.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        DateTime todate = Convert.ToDateTime(txtPaymentDueDate.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

        if (fromdate > todate)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Document Date Date is greater than Payment Due Date...Please Choose Correct Date.');", true);
            btnadd.Enabled = false;
        }
        else
        {
            btnadd.Enabled = true;
        }
    }

    protected void txtDisct_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtdisc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtDiscPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void ddlNoteType_TextChanged(object sender, EventArgs e)
    {
        if (ddlNoteType.SelectedValue == "Select")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Kindly Select Note Type..!!!');", true);
            txtDocNo.Text = "";
        }
        else if (ddlNoteType.SelectedValue == "Credit_Sale")
        {
            GenerateComCode_CR();
        }
        else if (ddlNoteType.SelectedValue == "Debit_Sale")
        {
            GenerateComCode_DB();
        }
    }

    protected void txtshippingcustomer_TextChanged(object sender, EventArgs e)
    {
        try
        {

            Fillddlshippingaddress(txtshippingcustomer.Text);

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
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

    protected void txtRate_TextChanged1(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    //private void Transportation_Calculation()
    //{
    //    var TotalAmt = Convert.ToDecimal(txtTCharge.Text.Trim());
    //    decimal CGST;
    //    if (string.IsNullOrEmpty(txtTCGSTPer.Text))
    //    {
    //        CGST = 0;
    //    }
    //    else
    //    {
    //        decimal Val1 = Convert.ToDecimal(txtTCharge.Text.Trim());
    //        decimal Val2 = Convert.ToDecimal(txtTCGSTPer.Text);

    //        CGST = (Val1 * Val2 / 100);
    //    }
    //    txtTCGSTamt.Text = CGST.ToString("0.00", CultureInfo.InvariantCulture);

    //    decimal SGST;
    //    if (string.IsNullOrEmpty(txtTSGSTPer.Text))
    //    {
    //        SGST = 0;
    //    }
    //    else
    //    {
    //        decimal Val1 = Convert.ToDecimal(txtTCharge.Text);
    //        decimal Val2 = Convert.ToDecimal(txtTSGSTPer.Text);

    //        SGST = (Val1 * Val2 / 100);
    //    }
    //    txtTSGSTamt.Text = SGST.ToString("0.00", CultureInfo.InvariantCulture);


    //    decimal IGST;
    //    if (string.IsNullOrEmpty(txtTIGSTPer.Text))
    //    {
    //        IGST = 0;
    //    }
    //    else
    //    {
    //        decimal Val1 = Convert.ToDecimal(txtTCharge.Text);
    //        decimal Val2 = Convert.ToDecimal(txtTIGSTPer.Text);

    //        IGST = (Val1 * Val2 / 100);
    //    }
    //    txtTIGSTamt.Text = IGST.ToString("0.00", CultureInfo.InvariantCulture);

    //    var GSTTotal = CGST + SGST + IGST;

    //    var Finalresult = Convert.ToDecimal(txtTCharge.Text) + GSTTotal;

    //    txtTCost.Text = Finalresult.ToString("0.00", CultureInfo.InvariantCulture);
    //    if (dgvAutomatic.Rows.Count > 0)
    //    {
    //        foreach (GridViewRow row in dgvAutomatic.Rows)
    //        {
    //            TextBox GrandTotalGrid = (TextBox)row.FindControl("txtGrandTotal");
    //            GrandAmount1 += Convert.ToDouble(GrandTotalGrid.Text);
    //        }
    //        GAmount = Convert.ToDouble(txtTCost.Text) + Convert.ToDouble(GrandAmount1.ToString());
    //        txtFGrandTot.Text = GAmount.ToString();
    //    }
    //    else
    //    {
    //        foreach (GridViewRow row in dgvParticularsDetails.Rows)
    //        {
    //            Label GrandTotalGrid = (Label)row.FindControl("lblTotalAmount");
    //            GrandAmount1 += Convert.ToDouble(GrandTotalGrid.Text);
    //        }
    //        GAmount = Convert.ToDouble(txtTCost.Text) + Convert.ToDouble(GrandAmount1.ToString());
    //        txtFGrandTot.Text = GAmount.ToString();
    //    }

    //}

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
            txtFGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void txtBasic_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string Amt = sumofAmount.Text;
            string Basic = txtBasic.Text;
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

                    var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value);
                    txtFGrandTot.Text = grandtot.ToString("##.00");
                }
                else
                {
                    IGSTPertcs.Enabled = true;
                    CGSTPertcs.Enabled = false;
                    SGSTPertcs.Enabled = false;
                    var IGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(IGSTPertcs.Text) / 100;
                    var GSTTaxTotal = Convert.ToDouble(Basic) + IGSTAmt;
                    txtCost.Text = GSTTaxTotal.ToString("##.00");

                    var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value);
                    txtFGrandTot.Text = grandtot.ToString("##.00");
                }

                //var grandtot = Convert.ToDouble(Basic) + Convert.ToDouble(hdnGrandtotal.Value);
                //txtGrandTot.Text = grandtot.ToString("##.00");
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }
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
                    txtFGrandTot.Text = grandtot.ToString("##.00");
                }
                else
                {
                    IGSTPertcs.Enabled = true;
                    CGSTPertcs.Enabled = false;
                    SGSTPertcs.Enabled = false;
                    var IGSTAmt = Convert.ToDouble(Basic) * Convert.ToDouble(IGSTPertcs.Text) / 100;
                    var GSTTaxTotal = Convert.ToDouble(Basic) + IGSTAmt;
                    txtCost.Text = GSTTaxTotal.ToString("##.00");

                    var grandtot = Convert.ToDouble(GSTTaxTotal) + Convert.ToDouble(hdnGrandtotal.Value);
                    txtFGrandTot.Text = grandtot.ToString("##.00");
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
            txtFGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void CGSTPertcs_TextChanged(object sender, EventArgs e)
    {
        GstCalculationTcs();
    }

    protected void SGSTPertcs_TextChanged(object sender, EventArgs e)
    {
        GstCalculationTcs();
    }

    protected void IGSTPertcs_TextChanged(object sender, EventArgs e)
    {
        GstCalculationTcs();
    }
    protected void GetInvoice()
    {
        string com = "SELECT * FROM tbltaxinvoicehdr WHERE BillingCustomer = '" + txtSupplierName.Text + "' AND isdeleted = 0";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        ddlinvoice.DataSource = dt;
        ddlinvoice.DataBind();
        //ddlinvoice.DataTextField = "BillNo";
        ddlinvoice.DataTextField = "Invoiceno";
        ddlinvoice.DataValueField = "Id";
        ddlinvoice.DataBind();
        ddlinvoice.Items.Insert(0, new ListItem("--Select Invoice Number--", "0"));
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreditDebitNoteSaleList.aspx");
    }
    private void FillddlBillingAddress(string id)
    {

        SqlDataAdapter ad1 = new SqlDataAdapter("SELECT SA.BillAddress FROM tbl_BillingAddress  AS SA INNER JOIN tbl_CompanyMaster AS CM ON CM.ID=SA.c_id where Companyname='" + id + "'", Cls_Main.Conn);
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

    protected void ddlShippingaddress_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM tbl_CompanyMaster AS CM INNER JOIN tbl_ShippingAddress AS SA ON CM.ID=SA.c_id WHERE CM.Companyname='" + txtshippingcustomer.Text.Trim() + "' AND  SA.ShippingAddress='" + ddlShippingaddress.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        Da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtshippinglocation.Text = dt.Rows[0]["ShipLocation"].ToString();
            txtshortShippingaddress.Text = dt.Rows[0]["ShippingAddress1"].ToString();
            txtshippingPincode.Text = dt.Rows[0]["ShipPincode"].ToString();
            txtshippingstatecode.Text = dt.Rows[0]["ShipStatecode"].ToString();
            txtshippingGST.Text = dt.Rows[0]["GSTNo"].ToString();

        }
    }


    //changes on 6/12/2024
    protected void gvcomponent_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvcomponent.EditIndex = e.NewEditIndex;
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

    public void GetInventoryCalculation(string lblCompo, string lblnewQuantity, string Product, string Batch)
    {
        string lblDescription = "";
        string lblhsn = "";
        string lblUnit = "";
        string lblRate = "";
        string lblQuantity = "";
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
        string lblbatch = "";
        decimal TotalQuantity = 0;
        SqlDataAdapter sad2 = new SqlDataAdapter("select * from tbl_OutwardEntryComponentsDtls where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "' AND ComponentName='" + lblCompo + "'AND Batch='" + Batch + "'", con);
        DataTable Dt1 = new DataTable();
        sad2.Fill(Dt1);
        con.Close(); // Close the connection when done
        if (Dt1.Rows.Count > 0)
        {
            lblUnit = Dt1.Rows[0]["Units"].ToString();
            lblDescription = Dt1.Rows[0]["Description"].ToString();
            lblhsn = Dt1.Rows[0]["HSN"].ToString();
            lblRate = Dt1.Rows[0]["Rate"].ToString();
            lblCGSTPer = Dt1.Rows[0]["CGSTPer"].ToString();
            lblSGSTPer = Dt1.Rows[0]["SGSTPer"].ToString();
            lblIGSTPer = Dt1.Rows[0]["IGSTPer"].ToString();
            lblQuantity = Dt1.Rows[0]["Quantity"].ToString();
            lblbatch = Dt1.Rows[0]["Batch"].ToString();

            TotalQuantity = Convert.ToDecimal(lblQuantity) - Convert.ToDecimal(lblnewQuantity);


        }
        if (TotalQuantity == 0)
        {
            SqlCommand cmddelete = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "' AND ComponentName='" + lblCompo + "'", con);
            con.Open();
            cmddelete.ExecuteNonQuery();
            con.Close();
            Cls_Main.Conn_Open();
        }
        else
        {


            var total = Convert.ToDecimal(lblRate) * Convert.ToDecimal(TotalQuantity);
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

            SqlCommand cmddelete = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "' AND ComponentName='" + lblCompo + "'", con);
            con.Open();
            cmddelete.ExecuteNonQuery();
            con.Close();
            Cls_Main.Conn_Open();

            SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_InventoryOutwardManage] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
            cmdd.Parameters.AddWithValue("@OrderNo", ddlBillNumber.SelectedItem.Text);
            cmdd.Parameters.AddWithValue("@Particular", Product);
            cmdd.Parameters.AddWithValue("@ComponentName", lblCompo);
            cmdd.Parameters.AddWithValue("@Description", lblDescription);
            cmdd.Parameters.AddWithValue("@HSN", lblhsn);
            cmdd.Parameters.AddWithValue("@Quantity", TotalQuantity);
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
                txtshortBillingaddress.Text = dt.Rows[0]["BillAddress"].ToString();
                txtbillinglocation.Text = dt.Rows[0]["BillLocation"].ToString();
                txtbillingPincode.Text = dt.Rows[0]["BillPincode"].ToString();
                txtbillingstatecode.Text = dt.Rows[0]["Billstatecode"].ToString();
                txtbillingGST.Text = dt.Rows[0]["GSTNo"].ToString();
            }
        }
    }
}