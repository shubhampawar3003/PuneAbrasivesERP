using System;
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

public partial class Admin_PaymentReport : System.Web.UI.Page
{
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("../Login.aspx");
            }

            if (Request.QueryString["customername"] != null)
            {
                string customername = objcls.Decrypt(Request.QueryString["customername"].ToString());
                lblcompanyname.Text = customername;
                GetTransaction(customername);
                GetInvoice(customername);
                Session["customername"] = customername;
            }
            else
            {

            }
        }
    }

    public void GetTransaction(string customername)
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
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetTransaction"));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", customername));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        Gvreceipt.DataSource = Dt;
                        Gvreceipt.DataBind();



                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //throw;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }

    }


    protected void ddlcompnay_SelectedIndexChanged(object sender, EventArgs e)
    {

    }


    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        string customername = objcls.Decrypt(Request.QueryString["customername"].ToString());
        lblcompanyname.Text = customername;
        GetTransaction(customername);
        GetInvoice(customername);
        Session["customername"] = customername;
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {

    }

    protected void btnsearch_Click(object sender, EventArgs e)
    {
        string Invoice = ddlinvoice.Text;
        string customer = Session["customername"].ToString();
        string FromDate = string.Empty;
        string ToDate = string.Empty;
        if((txtfromdate.Text!=null && txtfromdate.Text!="") && (txttodate.Text != null && txttodate.Text != ""))
        {
             FromDate = txtfromdate.Text;
             ToDate = txttodate.Text;
        }
        else
        {
            FromDate = null;
            ToDate = null;

        }
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetSearchreports"));
                    cmd.Parameters.Add(new SqlParameter("@Invoiceno", Invoice));
                    cmd.Parameters.Add(new SqlParameter("@FromDate", FromDate));
                    cmd.Parameters.Add(new SqlParameter("@ToDate", ToDate));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", customer));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        Gvreceipt.DataSource = Dt;
                        Gvreceipt.DataBind();
                    }
                    else
                    {
                        Gvreceipt.EmptyDataText = "<span class='empty-data-message'>No records found.</span>";
                        Gvreceipt.DataBind();
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //throw;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }

    }
    
    public void GetInvoice(string customername)
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
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetInvoice"));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", customername));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        ddlinvoice.DataSource = Dt;
                        ddlinvoice.DataTextField = "InvoiceNo"; // Specify the column name to be displayed in the dropdown
                        ddlinvoice.DataValueField = "InvoiceNo"; // Specify the column name for the value of each item
                        ddlinvoice.DataBind();
                        ddlinvoice.Items.Insert(0, " --  Select Invoice No.-- ");
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            //throw;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }
    }

    protected void Gvreceipt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblpending = (Label)e.Row.FindControl("txtPending");

            if (lblpending.Text == "0.00")
            {
                //e.Row.BackColor = System.Drawing.Color.LightCoral;
                e.Row.BackColor = System.Drawing.Color.FromArgb(255, 153, 51); // Bhagwa color
                                                                               // Saffron color

            }
            //lblpending.Text = "0.00";

        }
    }


}