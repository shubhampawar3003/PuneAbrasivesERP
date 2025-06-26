
using Microsoft.Reporting.WebForms;
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

public partial class Admin_PaymentList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("../Login.aspx");
            }
            else
            {
                GetCashList();
            }
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddPayments.aspx");
    }



    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaymentList.aspx");
    }

    public void GetCashList()
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
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetCashmasterlist"));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", txtCustomerName.Text));
                    //cmd.Parameters.Add(new SqlParameter("@Invoiceno", txtinoiceno.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        GvCasHDetails.DataSource = Dt;
                        GvCasHDetails.DataBind();
                    }
                    else
                    {
                        GvCasHDetails.DataSource =null;
                        GvCasHDetails.DataBind();
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {

            //throw;
            //string errorMsg = "An error occurred : " + ex.Message;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }

    }

    protected void GvCasHDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvCasHDetails.PageIndex = e.NewPageIndex;
        GetCashList();
    }

    protected void GvCasHDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

            int rowIndex = Convert.ToInt32(e.CommandArgument);
            GridViewRow selectedRow = GvCasHDetails.Rows[rowIndex];


            string customername = GvCasHDetails.DataKeys[selectedRow.RowIndex]["customername"].ToString();


            if (e.CommandName == "RowEdit")
            {
                //Response.Redirect("Cashmangements.aspx?InvoiceNo=" + encrypt(e.CommandArgument.ToString()) + "");

                // Encrypt the InvoiceNo
                string encryptedInvoiceNo = objcls.encrypt(customername);

                // Redirect to Cashmangements.aspx with encrypted InvoiceNo
                Response.Redirect("AddPayments.aspx?customername=" + encryptedInvoiceNo);
            }


            if (e.CommandName == "RowDelete")
            {
                Cls_Main.Conn_Open();
                SqlCommand Cmd = new SqlCommand("UPDATE [tbl_PaymentDetailsHdr] SET IsDeleted=@IsDeleted WHERE InvoiceNo=@InvoiceNo", Cls_Main.Conn);
                //Cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                Cmd.Parameters.AddWithValue("@IsDeleted", '1');
                Cmd.Parameters.AddWithValue("@DeletedBy", Session["Username"].ToString());
                Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
                Cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Deleted Successfully..!!')", true);
                GetCashList();
            }



            if (e.CommandName == "RowPDF")
            {
                GetOutstandingReports(customername);
            }
            if (e.CommandName == "View")
            {
                // GetTransaction(customername);
                string encryptedInvoiceNo = objcls.encrypt(customername);
                Response.Redirect("PaymentReport.aspx?customername=" + encryptedInvoiceNo);
      
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
        GetCashList();
        //try
        //{
        //    string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.Add(new SqlParameter("@Action", "GetInvoiceforFiletr"));
        //            cmd.Parameters.Add(new SqlParameter("@Companyname", ddlcompnay.SelectedItem.Text));
        //            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //            DataTable Dt = new DataTable();
        //            adapter.Fill(Dt);
        //            if (Dt.Rows.Count > 0)
        //            {
        //                ddlinvoice.DataSource = Dt;
        //                ddlinvoice.DataValueField = "InvoiceNo";
        //                ddlinvoice.DataTextField = "InvoiceNo";
        //                ddlinvoice.DataBind();
        //                ddlinvoice.Items.Insert(0, " --  Select InvoiceNo-- ");

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



    public void GetOutstandingReports(string customername)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Action", "GetReports");
                cmd.Parameters.AddWithValue("@Companyname", customername);

                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ReportDataSource obj1 = new ReportDataSource("DataSet1", ds.Tables[0]);
                        ReportDataSource obj2 = new ReportDataSource("DataSet2", ds.Tables[1]);
                        // ReportDataSource obj3 = new ReportDataSource("DataSet1", ds.Tables[2]);

                        ReportViewer1.LocalReport.DataSources.Add(obj1);
                        ReportViewer1.LocalReport.DataSources.Add(obj2);
                        // ReportViewer1.LocalReport.DataSources.Add(obj3);

                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PaymentDetails.rdlc";
                        ReportViewer1.LocalReport.Refresh();

                        //-------- Print PDF directly without showing ReportViewer ----
                        Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string extension;

                        byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;

                        Response.ContentType = "application/pdf";
                        Response.AddHeader("content-disposition", "attachment;filename=\"" + "PartyTransactionreports" + ".PDF"); // Give file name here

                        Response.BinaryWrite(bytePdfRep);

                        ReportViewer1.LocalReport.DataSources.Clear();
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                    }
                }
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
                    cmd.Parameters.Add(new SqlParameter("@Action", "Gettransactionhistory"));
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

    protected void GvCasHDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblpending = (Label)e.Row.FindControl("lblPending");

            if (lblpending.Text == "0.00")
            {
                //e.Row.BackColor = System.Drawing.Color.LightCoral;
                e.Row.BackColor = System.Drawing.Color.FromArgb(255, 153, 51); // Bhagwa color
                                                                               // Saffron color

            }
            //lblpending.Text = "0.00";

        }

        //Authorization
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

            string empcode = Session["UserCode"].ToString();
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                string id = Dt.Rows[0]["ID"].ToString();
                DataTable Dtt = new DataTable();
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'PaymentList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate.Visible = false;
                    //GVQuotation.Columns[15].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
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
                com.CommandText = "Select Distinct CustomerName from tbl_PaymentDetailsHdr where  " + "CustomerName like @Search + '%' and IsDeleted is Null";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["CustomerName"].ToString());
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
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("[SP_PaymentDetails]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetCashmasterlist"));
                    cmd.Parameters.Add(new SqlParameter("@Companyname", txtCustomerName.Text));
                    //cmd.Parameters.Add(new SqlParameter("@Invoiceno", txtinoiceno.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        GvCasHDetails.DataSource = Dt;
                        GvCasHDetails.DataBind();
                    }
                    else
                    {
                        GvCasHDetails.DataSource = null;
                        GvCasHDetails.DataBind();
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {

            //throw;
            //string errorMsg = "An error occurred : " + ex.Message;
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }
    }

}