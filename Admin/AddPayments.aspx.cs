
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_AddPayments : System.Web.UI.Page
{
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["UserName"] == null)
            {
                Response.Redirect("../Login.aspx");
            }

            if (Request.QueryString["customername"] != null)
            {
                string customername = objcls.Decrypt(Request.QueryString["customername"].ToString());
                LoadRecord(customername);
                txtCustomerName.Text = customername;
                btnsave.Text = "Update";                
                txtTransactionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                txtTransactionDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
             
            }
        }
    }

    protected void txtquantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
    }

    protected void ddlinvoice_SelectedIndexChanged(object sender, EventArgs e)
    {

        //try
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add(new SqlParameter("@Action", "GetInvoiceDate"));
        //            //cmd.Parameters.Add(new SqlParameter("@Invoiceno", ddlinvoice.SelectedValue));
        //            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //            DataTable Dt = new DataTable();
        //            adapter.Fill(Dt);
        //            if (Dt.Rows.Count > 0)
        //            {
        //                //txtinvoicedate.Text = ((DateTime)Dt.Rows[0]["Invoicedate"]).ToString("yyyy-MM-dd");
        //                //txtinoiceno.Text = ddlinvoice.SelectedItem.Text;
        //                GetCashDetails();
        //            }
        //        }
        //        connection.Close();
        //    }
        //}
        //catch (Exception ex)
        //{

        //    //throw;
        //    string errorMsg = "An error occurred : " + ex.Message;
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);


        //}
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtCustomerName.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {

                Cls_Main.Conn_Open();
                SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", Cls_Main.Conn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@InvoiceNo", ddlinvoice.SelectedValue);
                cmd.Parameters.AddWithValue("@Companyname", txtCustomerName.Text);
                //cmd.Parameters.AddWithValue("@InvoiecDate", txtinvoicedate.Text);
                cmd.Parameters.AddWithValue("@TransactionnDate", txtTransactionDate.Text);
                cmd.Parameters.AddWithValue("@Status", "1");
                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
                cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                cmd.Parameters.AddWithValue("@Action", "InsertHdr");
                cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                Cls_Main.Conn_Dispose();
                //Save Product Details 


                foreach (GridViewRow row in Gvreceipt.Rows)
                {
                    Cls_Main.Conn_Open();
                    Label txtPaymentTerm = (Label)row.FindControl("txtPayment");
                    Label LabelInvoice = (Label)row.FindControl("txtinoiceno");
                    Label lblbillno = (Label)row.FindControl("txtbillno");
                    Label lblinvoicedate = (Label)row.FindControl("txtinvoicedate");
                    Label LblGrandtotal = (Label)row.FindControl("txtGrandtotal");
                    Label lblRecived = (Label)row.FindControl("txtRecived");
                    Label lblPending = (Label)row.FindControl("txtPending");
                    TextBox txtPaid = (TextBox)row.FindControl("txtPaid");
                    TextBox txtRemarks = (TextBox)row.FindControl("Remarks");
                    Label lblTransactiondate = (Label)row.FindControl("TransactionnDate");
                    Label txtcurrentrecived = (Label)row.FindControl("txtcurrentrecived");
                  
                    var Pending = "";
                    string status = "";
                    if (LblGrandtotal.Text == lblRecived.Text)
                    {
                        Pending = "00";
                        status = "ISPAID";
                    }
                    else

                    {
                        Pending = lblPending.Text;
                        status = "";
                    }
                    Cls_Main.Conn_Open();
                    SqlCommand cmdinv = new SqlCommand("[SP_PaymentDetails]", Cls_Main.Conn);
                    cmdinv.CommandType = CommandType.StoredProcedure;
                    cmdinv.Parameters.AddWithValue("@Action", "Insertandupdatedtl");
                    cmdinv.Parameters.AddWithValue("@InvoiceNo", LabelInvoice.Text);
                    cmdinv.Parameters.AddWithValue("@BillNO", lblbillno.Text);
                    cmdinv.Parameters.AddWithValue("@GrandTotal", LblGrandtotal.Text);
                    cmdinv.Parameters.AddWithValue("@Recived", lblRecived.Text);
                    cmdinv.Parameters.AddWithValue("@Paid", txtPaid.Text);
                    cmdinv.Parameters.AddWithValue("@Pending", Pending);
                    cmdinv.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
                    cmdinv.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
                    cmdinv.Parameters.AddWithValue("@PaymentTerm", txtPaymentTerm.Text);
                    cmdinv.Parameters.AddWithValue("@Companyname", txtCustomerName.Text);
                    cmdinv.Parameters.AddWithValue("@CurrentRecivedamount", txtcurrentrecived.Text);
                    cmdinv.Parameters.AddWithValue("@Status", status);
                    //cmdinv.Parameters.AddWithValue("@LastTransactionDate ", txtTransactionDate.Text);
                    //cmdinv.Parameters.AddWithValue("@InvoiecDate", lblinvoicedate.Text);
                    if (btnsave.Text == "Update")
                    {
                        if (lblTransactiondate.Text == "")
                        {
                            //DateTime ldate = Convert.ToDateTime(txtTransactionDate.Text);
                            //cmdinv.Parameters.AddWithValue("@LastTransactionDate ", ldate);
                            cmdinv.Parameters.AddWithValue("@LastTrDate", txtTransactionDate.Text);
                        }
                        else
                        {
                            //DateTime ldate = Convert.ToDateTime(lblTransactiondate.Text);
                            //cmdinv.Parameters.AddWithValue("@LastTransactionDate", ldate);
                            cmdinv.Parameters.AddWithValue("@LastTrDate", lblTransactiondate.Text);
                        }


                    }
                    else
                    {
                        if (lblTransactiondate.Text == "")
                        {
                            cmdinv.Parameters.AddWithValue("@LastTrDate", txtTransactionDate.Text);
                            //DateTime ldate = Convert.ToDateTime(txtTransactionDate.Text);
                            //cmdinv.Parameters.AddWithValue("@LastTransactionDate ", ldate);
                        }
                        else
                        {
                            cmdinv.Parameters.AddWithValue("@TrDate", txtTransactionDate.Text);
                            //DateTime ldate = Convert.ToDateTime(txtTransactionDate.Text);

                            //cmdinv.Parameters.AddWithValue("@TransactionDate", ldate);
                        }

                    }
                    cmdinv.Parameters.AddWithValue("@InvDate", lblinvoicedate.Text);
                    //DateTime Idate = Convert.ToDateTime(lblinvoicedate.Text);
                    //cmdinv.Parameters.AddWithValue("@InvoiecDate", Idate);
                    cmdinv.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();

                    btnsave.Enabled = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Payment Details Saved Successfully..!!'); window.location.href='PaymentList.aspx';", true);

                }
                InserTransactionsHistory();
            }
        }
        catch (Exception ex)
        {

            //throw;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {

        Response.Redirect("AddPayments.aspx");
    }

    public void LoadRecord(string customername)
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetCasDetialsByInvoice"));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", customername));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        Gvreceipt.DataSource = Dt;
                        Gvreceipt.DataBind();
                        divtag.Visible = true;

                    }
                }
            }
        }
        catch (Exception ex)
        {

            //throw;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void chkheader_CheckedChanged(object sender, EventArgs e)
    {


        foreach (GridViewRow row in Gvreceipt.Rows)
        {
            CheckBox chckheader = (CheckBox)Gvreceipt.HeaderRow.FindControl("chkHeader");
            CheckBox chk = (CheckBox)row.FindControl("chkRow");

            if (chckheader.Checked == true)
            {
                chk.Checked = true;
            }
            else
            {
                chk.Checked = false;
            }
        }

    }

    protected void Gvreceipt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (btnsave.Text != "Update")
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Find controls in the current row
                Label lblInvoice = (Label)e.Row.FindControl("txtinoiceno");
                Label lblBillNo = (Label)e.Row.FindControl("txtbillno");
                Label lblInvoiceDate = (Label)e.Row.FindControl("txtinvoicedate");
                Label lblGrandTotal = (Label)e.Row.FindControl("txtGrandtotal");
                Label lblReceived = (Label)e.Row.FindControl("txtRecived");
                Label lblPending = (Label)e.Row.FindControl("txtPending");
                TextBox txtPaid = (TextBox)e.Row.FindControl("txtPaid");
                TextBox txtRemarks = (TextBox)e.Row.FindControl("Remarks");
                CheckBox chk = (CheckBox)e.Row.FindControl("chkRow");

                // Calculate totals
                txtPaid.Text = "00";


                decimal grandTotal = Convert.ToDecimal(lblGrandTotal.Text);
                decimal receivedAmount = Convert.ToDecimal(txtPaid.Text) + Convert.ToDecimal(lblReceived.Text);
                decimal pendingAmount = grandTotal - receivedAmount;

                // Update labels with calculated values
                lblReceived.Text = receivedAmount.ToString();
                lblPending.Text = pendingAmount.ToString();
                if (grandTotal == receivedAmount)
                {
                    txtPaid.Text = "00";
                    txtPaid.Enabled = false;
                    chk.Visible = false;
                }
            }
        }


        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            // Find footer controls
            Label lblGrandTotalFooter = (Label)e.Row.FindControl("footerGrandtotal");
            Label lblReceivedFooter = (Label)e.Row.FindControl("footerrecived");
            Label lblPendingFooter = (Label)e.Row.FindControl("footerpending");

            // Update footer totals
            decimal sumOfGrandTotal = 0;
            decimal sumOfReceived = 0;
            decimal sumOfPending = 0;

            foreach (GridViewRow row in Gvreceipt.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkRow");

                Label lblGrandTotal = (Label)row.FindControl("txtGrandtotal");
                Label lblReceived = (Label)row.FindControl("txtRecived");

                sumOfGrandTotal += Convert.ToDecimal(lblGrandTotal.Text);
                sumOfReceived += Convert.ToDecimal(lblReceived.Text);
            }
            sumOfPending = sumOfGrandTotal - sumOfReceived;
            lblGrandTotalFooter.Text = sumOfGrandTotal.ToString();
            lblReceivedFooter.Text = sumOfReceived.ToString();
            lblPendingFooter.Text = sumOfPending.ToString();
        }
    }

    public void InserTransactionsHistory()
    {
        Cls_Main.Conn_Open();

        foreach (GridViewRow row in Gvreceipt.Rows)
        {
            Cls_Main.Conn_Open();
            Label LabelInvoice = (Label)row.FindControl("txtinoiceno");
            Label lblbillno = (Label)row.FindControl("txtbillno");
            Label lblinvoicedate = (Label)row.FindControl("txtinvoicedate");
            Label LblGrandtotal = (Label)row.FindControl("txtGrandtotal");
            Label lblRecived = (Label)row.FindControl("txtRecived");
            Label lblPending = (Label)row.FindControl("txtPending");
            TextBox txtPaid = (TextBox)row.FindControl("txtPaid");
            TextBox txtRemarks = (TextBox)row.FindControl("Remarks");
            Label lblTransactiondate = (Label)row.FindControl("TransactionnDate");

            Label txtcurrentrecived = (Label)row.FindControl("txtcurrentrecived");
            CheckBox chk = (CheckBox)row.FindControl("chkRow");

            var Pending = "";
            string status = "";
            if (LblGrandtotal.Text == lblRecived.Text)
            {
                Pending = "00";
                status = "ISPAID";
            }
            else

            {
                Pending = lblPending.Text;
                status = "";
            }

            SqlCommand cmdinv = new SqlCommand("[SP_PaymentDetails]", Cls_Main.Conn);
            cmdinv.CommandType = CommandType.StoredProcedure;
            cmdinv.Parameters.AddWithValue("@Action", "InserHistory");
            cmdinv.Parameters.AddWithValue("@InvoiceNo", LabelInvoice.Text);
            cmdinv.Parameters.AddWithValue("@BillNO", lblbillno.Text);
            cmdinv.Parameters.AddWithValue("@GrandTotal", LblGrandtotal.Text);
            cmdinv.Parameters.AddWithValue("@Recived", lblRecived.Text);
            cmdinv.Parameters.AddWithValue("@Paid", txtPaid.Text);
            cmdinv.Parameters.AddWithValue("@Pending", Pending);
            cmdinv.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
            cmdinv.Parameters.AddWithValue("@Remarks", txtRemarks.Text);
            cmdinv.Parameters.AddWithValue("@Companyname", txtCustomerName.Text);
            cmdinv.Parameters.AddWithValue("@CurrentRecivedamount", txtcurrentrecived.Text);
            if (btnsave.Text == "Update")
            {
                if (lblTransactiondate.Text == "")
                {
                    //DateTime ldate = Convert.ToDateTime(txtTransactionDate.Text);
                    //cmdinv.Parameters.AddWithValue("@LastTransactionDate ", ldate);
                    cmdinv.Parameters.AddWithValue("@LastTrDate", txtTransactionDate.Text);
                }
                else
                {
                    //DateTime ldate = Convert.ToDateTime(lblTransactiondate.Text);
                    //cmdinv.Parameters.AddWithValue("@LastTransactionDate", ldate);
                    cmdinv.Parameters.AddWithValue("@LastTrDate", lblTransactiondate.Text);
                }


            }
            else
            {
                if (lblTransactiondate.Text == "")
                {
                    cmdinv.Parameters.AddWithValue("@LastTrDate", txtTransactionDate.Text);
                    //DateTime ldate = Convert.ToDateTime(txtTransactionDate.Text);
                    //cmdinv.Parameters.AddWithValue("@LastTransactionDate ", ldate);
                }
                else
                {
                    cmdinv.Parameters.AddWithValue("@TrDate", txtTransactionDate.Text);
                    //DateTime ldate = Convert.ToDateTime(txtTransactionDate.Text);

                    //cmdinv.Parameters.AddWithValue("@TransactionDate", ldate);
                }

            }
            cmdinv.Parameters.AddWithValue("@InvDate", lblinvoicedate.Text);
            //DateTime Idate = Convert.ToDateTime(lblinvoicedate.Text);
            //cmdinv.Parameters.AddWithValue("@InvoiecDate", Idate);
            cmdinv.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            Cls_Main.Conn_Dispose();

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Cash Details Saved Successfully..!!'); window.location.href='CashMasterList.aspx';", true);
        }
    }

    private Dictionary<TextBox, int> changeCount = new Dictionary<TextBox, int>();
    protected void txtPaid_TextChanged(object sender, EventArgs e)
    {

        foreach (GridViewRow row in Gvreceipt.Rows)
        {
            Label txtrecived = (Label)row.FindControl("txtcurrentrecived");
            TextBox txtpaid = (TextBox)row.FindControl("txtPaid");
            if (txtpaid.Text != "00")
            {
                txtrecived.Text = txtpaid.Text;
            }
            // Assign the value of txtpaid, not its string representation
        }

        Double sumofGrandTotal = 0;
        Double SumOfTotalFooter = 0;
        Double sumoftotalrecived = 0;
        Double sumoftotalpending = 0;
        foreach (GridViewRow row in Gvreceipt.Rows)
        {
            Label LabelInvoice = (Label)row.FindControl("txtinoiceno");
            Label lblbillno = (Label)row.FindControl("txtbillno");
            Label lblinvoicedate = (Label)row.FindControl("txtinvoicedate");
            Label LblGrandtotal = (Label)row.FindControl("txtGrandtotal");
            Label lblRecived = (Label)row.FindControl("txtRecived");
            Label lblPending = (Label)row.FindControl("txtPending");
            TextBox txtPaid = (TextBox)row.FindControl("txtPaid");

            TextBox txtremarks = (TextBox)row.FindControl("Remarks");
            CheckBox chk = (CheckBox)row.FindControl("chkRow");
            Label lblgrandtotal = (Label)Gvreceipt.FooterRow.FindControl("footerGrandtotal");
            Label lblfooterpending = (Label)Gvreceipt.FooterRow.FindControl("footerpending");
            Label lblrecived = (Label)Gvreceipt.FooterRow.FindControl("footerrecived");
            Label currentrecived = (Label)Gvreceipt.FooterRow.FindControl("txtcurrentrecived");


            var TotalAmt = Convert.ToDecimal(LblGrandtotal.Text.Trim()) - Convert.ToDecimal(txtPaid.Text.Trim()) - Convert.ToDecimal(lblRecived.Text.Trim());
            var GrandTotal = Convert.ToDecimal(LblGrandtotal.Text.Trim());
            var TotalRecived = ((Convert.ToDecimal(txtPaid.Text.Trim()) + Convert.ToDecimal(lblRecived.Text.Trim())));
            var Pending = GrandTotal - TotalRecived;
            // var TotalAmt = Convert.ToDecimal(txtGrandtotal.Text.Trim()) -  Convert.ToDecimal(txtRecived.Text.Trim());
            lblRecived.Text = TotalRecived.ToString();
            lblPending.Text = Pending.ToString();
            lblRecived.Text = TotalRecived.ToString();
            lblPending.Text = Pending.ToString();
            //sumofGrandTotal += Convert.ToDouble(GrandTotal);


            txtPaid.Text = "00";
            if (LblGrandtotal.Text == lblRecived.Text)
            {
                txtPaid.Text = "00";
                txtPaid.Enabled = false;
                chk.Visible = false;

            }

            sumofGrandTotal += Convert.ToDouble(LblGrandtotal.Text);
            sumoftotalrecived += Convert.ToDouble(lblRecived.Text);
            sumoftotalpending += Convert.ToDouble(lblPending.Text);

            lblgrandtotal.Text = Convert.ToString(sumofGrandTotal);

            lblfooterpending.Text = Convert.ToString(sumoftotalpending);

            lblrecived.Text = Convert.ToString(sumoftotalrecived);

        }
    }

    //Search Customers methods
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
                com.CommandText = "SELECT DISTINCT TH.BillingCustomer,TC.CompanyCode FROM tbl_CompanyMaster AS TC INNER JOIN tblTaxInvoiceHdr AS TH ON TC.Companyname = TH.BillingCustomer WHERE " + "TH.BillingCustomer like @Search + '%' AND TH.IsDeleted =0 AND Status = 3 AND TH.Invoiceno NOT IN (SELECT Invoiceno FROM tbl_PaymentDetailsDtl ) GROUP BY TH.BillingCustomer,TC.CompanyCode;";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["BillingCustomer"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {

        try
        {
            Gvreceipt.DataSource = null;
            Gvreceipt.DataBind();
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetCashDetails"));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", txtCustomerName.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {

                        Gvreceipt.DataSource = dt;
                        Gvreceipt.DataBind();
                        divtag.Visible = true;

                    }


                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

}
