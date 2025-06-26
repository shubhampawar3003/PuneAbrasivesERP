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
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Web.UI.DataVisualization.Charting;
using System.Globalization;
using System.Drawing;

public partial class Admin_AuditLogDashboard : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
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
                GvEnquiryListBind();
                GvQuatationListBind();
                // GvOrderAcceptanceListBind();
                //GvLoginLogBind();
                btnEnquiry.BackColor = Color.Green;
            }
        }
    }

    void GvEnquiryListBind()
    {
        string query = string.Empty;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "SP_AuditLogDashboard";

        cmd.Parameters.AddWithValue("@FromDate", txtFromdate.Text == "" ? (object)DBNull.Value : txtFromdate.Text);
        cmd.Parameters.AddWithValue("@ToDate", txtTodate.Text == "" ? (object)DBNull.Value : txtTodate.Text);
        cmd.Parameters.AddWithValue("@ActionName", "Enquiry");
        cmd.Connection = con;
        con.Open();
        try
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                //bind the 1st resultset
                dgvEnquiryList.EmptyDataText = "No Records Found";
                dgvEnquiryList.DataSource = reader;
                dgvEnquiryList.DataBind();
                dgvEnquiryList.ShowHeader = true;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        con.Close();

    }

    void GvQuatationListBind()
    {
        string query = string.Empty;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "SP_AuditLogDashboard";

        cmd.Parameters.AddWithValue("@FromDate", txtFromDate1.Text == "" ? (object)DBNull.Value : txtFromDate1.Text);
        cmd.Parameters.AddWithValue("@ToDate", txtToDate1.Text == "" ? (object)DBNull.Value : txtToDate1.Text);
        cmd.Parameters.AddWithValue("@ActionName", "Quatation");
        cmd.Connection = con;
        con.Open();
        try
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                //bind the 1st resultset
                dgvQuatationList.EmptyDataText = "No Records Found";
                dgvQuatationList.DataSource = reader;
                dgvQuatationList.DataBind();
                dgvQuatationList.ShowHeader = true;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        con.Close();

    }

    //private void GvLoginLogBind()
    //{

    //    string query = string.Empty;
    //    SqlCommand cmd = new SqlCommand();
    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.CommandText = "SP_AuditLogDashboard";

    //    cmd.Parameters.AddWithValue("@ToDate", txtDate.Text == "" ? (object)DBNull.Value : txtDate.Text);
    //    cmd.Parameters.AddWithValue("@ActionName", "Login");
    //    cmd.Connection = con;
    //    con.Open();
    //    try
    //    {
    //        using (SqlDataReader reader = cmd.ExecuteReader())
    //        {
    //            //bind the 1st resultset
    //            GvLoginlog.EmptyDataText = "No Records Found";
    //            GvLoginlog.DataSource = reader;
    //            GvLoginlog.DataBind();
    //            GvLoginlog.ShowHeader = true;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }

    //    con.Close();
    //}

    protected void btnEnquiry_Click(object sender, EventArgs e)
    {
        btnEnquiry.BackColor = Color.Green;
        btnQuatation.BackColor = Color.DarkCyan;
        btnOrderAccept.BackColor = Color.DarkCyan;
        btnLoginList.BackColor = Color.DarkCyan;

        DivEnquiry.Visible = true;
        DivQuatation.Visible = false;
        DivOrderAccept.Visible = false;
        DivLoginList.Visible = false;
    }

    protected void btnQuatation_Click(object sender, EventArgs e)
    {
        btnQuatation.BackColor = Color.Green;
        btnEnquiry.BackColor = Color.DarkCyan;
        btnOrderAccept.BackColor = Color.DarkCyan;
        btnLoginList.BackColor = Color.DarkCyan;

        DivEnquiry.Visible = false;
        DivQuatation.Visible = true;
        DivOrderAccept.Visible = false;
        DivLoginList.Visible = false;
    }

    protected void btnOrderAccept_Click(object sender, EventArgs e)
    {
        btnEnquiry.BackColor = Color.DarkCyan;
        btnQuatation.BackColor = Color.DarkCyan;
        btnOrderAccept.BackColor = Color.Green;
        btnLoginList.BackColor = Color.DarkCyan;

        DivEnquiry.Visible = false;
        DivQuatation.Visible = false;
        DivOrderAccept.Visible = true;
        DivLoginList.Visible = false;
    }

    protected void btnLoginList_Click(object sender, EventArgs e)
    {
        btnEnquiry.BackColor = Color.DarkCyan;
        btnQuatation.BackColor = Color.DarkCyan;
        btnOrderAccept.BackColor = Color.DarkCyan;
        btnLoginList.BackColor = Color.Green;

        DivEnquiry.Visible = false;
        DivQuatation.Visible = false;
        DivOrderAccept.Visible = false;
        DivLoginList.Visible = true;
        string query = string.Empty;
        DateTime date = DateTime.Now;

        query = @"SELECT * FROM tbl_LoginInfo WHERE CONVERT(date, createdon) = @createdon ORDER BY id DESC";

        // Use a parameterized query to prevent SQL injection and handle the date format correctly
        SqlDataAdapter ad = new SqlDataAdapter(query, con);

        // Define the parameter and add it to the SqlDataAdapter
        ad.SelectCommand.Parameters.AddWithValue("@createdon", date.Date);

        DataTable dt = new DataTable();
        ad.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            GvLoginlog.DataSource = dt;
            GvLoginlog.DataBind();
            Divnotfountimg.Visible = false;
        }
        else
        {
            GvLoginlog.DataSource = null;
            GvLoginlog.DataBind();
            Divnotfountimg.Visible = true;
        }

    }

    protected void btnEnquirySearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtFromdate.Text == "" && txtTodate.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select Date of From Date and To Date !!');", true);
            }
            else
            {
                GvEnquiryListBind();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btnRefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("AuditLogDashboard.aspx");
    }

    protected void btnQuatationSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtFromDate1.Text == "" && txtToDate1.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select Date of From Date and To Date !!');", true);
            }
            else
            {
                GvQuatationListBind();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void btnLoginSearch_Click(object sender, EventArgs e)
    {
        GvLoginLogBind();
    }

    protected void btnOrderAcceptance_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtfromDate2.Text == "" && txtToDate2.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select Date of From Date and To Date !!');", true);
            }
            else
            {
                GvOrderAcceptanceListBind();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    void GvOrderAcceptanceListBind()
    {
        string query = string.Empty;
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "SP_AuditLogDashboard";

        cmd.Parameters.AddWithValue("@FromDate", txtfromDate2.Text == "" ? (object)DBNull.Value : txtfromDate2.Text);
        cmd.Parameters.AddWithValue("@ToDate", txtToDate2.Text == "" ? (object)DBNull.Value : txtToDate2.Text);
        cmd.Parameters.AddWithValue("@ActionName", "POList");
        cmd.Connection = con;
        con.Open();
        try
        {
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                //bind the 1st resultset
                dgvOrderAcceptList.EmptyDataText = "No Records Found";
                dgvOrderAcceptList.DataSource = reader;
                dgvOrderAcceptList.DataBind();
                dgvOrderAcceptList.ShowHeader = true;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

        con.Close();

    }

    private void GvLoginLogBind()
    {

        string query = string.Empty;

        query = @"select * from tbl_LoginInfo  where CONVERT(date,createdon)='" + (txtDate.Text == "" ? (object)DBNull.Value : txtDate.Text) + "' order by id desc";

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvLoginlog.DataSource = dt;
            GvLoginlog.DataBind();
            Divnotfountimg.Visible = false;
        }
        else
        {
            GvLoginlog.DataSource = null;
            GvLoginlog.DataBind();
            Divnotfountimg.Visible = true;

        }
    }

    protected void GvLoginlog_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvLoginlog.PageIndex = e.NewPageIndex;
        this.GvLoginLogBind();
    }
}