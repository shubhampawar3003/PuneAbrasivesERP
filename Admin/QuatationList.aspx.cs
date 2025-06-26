
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


public partial class Admin_QuatationList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
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
                FillGrid();
            }
        }
    }
    //Fill GridView
    private void FillGrid()
    {
        if (Session["Role"].ToString() == "Admin")
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy WHERE QH.IsDeleted = 0 ORDER BY QH.ID DESC");
            GVQuotation.DataSource = Dt;
            GVQuotation.DataBind();
        }
        else
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy WHERE QH.CreatedBy='" + Session["UserCode"].ToString() + "'  AND QH.IsDeleted = 0 ORDER BY QH.ID DESC");
            GVQuotation.DataSource = Dt;
            GVQuotation.DataBind();

        }

    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("QuatationMaster.aspx");
    }

    protected void GVQuotation_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("QuatationMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "&Action=OLD");
        }
        if (e.CommandName == "RowNew")
        {
            Response.Redirect("QuatationMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "&Action=NEW");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_QuotationHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Quotation Deleted Successfully..!!')", true);
            FillGrid();
        }
        if (e.CommandName == "RowView")
        {
            Report(e.CommandArgument.ToString());
           // Response.Redirect("Pdf_Quotation.aspx?Quotationno=" + (e.CommandArgument.ToString()) + " ");
            // Response.Write("<script>window.open ('Pdf_Quotation.aspx?Quotationno=" + (e.CommandArgument.ToString()) + "','_blank');</script>");
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
                    cmd.Parameters.AddWithValue("@Action", "GetQuotationDetails");
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
                    DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"]+"'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\Quotation.rdlc";
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
                    string Filename = Invoiceno + "_Quotation.pdf";
                    Response.ContentType = "application/vnd.pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "");
                    Response.BinaryWrite(bytePdfRep);
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

    protected void GVQuotation_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVQuotation.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVQuotation_RowDataBound(object sender, GridViewRowEventArgs e)
    {
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'QuatationList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate.Visible = false;
                    //GVQuotation.Columns[15].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }

            //Label Quotationno = e.Row.FindControl("Quotationno") as Label;
            //string[] parts = Quotationno.Split('-');

            //if (parts.Length >= 2)
            //{
            //    // Get the numeric part and increment it
            //    string numericPart = parts[1];
            //    Double total = Convert.ToDouble(numericPart) + 1;
            //    string words = parts[0];            
            //}


        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtQuotationNo.Text) && string.IsNullOrEmpty(txtbillingcustomer.Text) && string.IsNullOrEmpty(txtfromdate.Text) && string.IsNullOrEmpty(txttodate.Text))
            {
                FillGrid();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Search Record');", true);
            }
            else
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    if (txtQuotationNo.Text != "")
                    {
                        string Quono = txtQuotationNo.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy where QH.Quotationno = '" + Quono + "' AND QH.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVQuotation.EmptyDataText = "Not Records Found";
                        GVQuotation.DataSource = dt;
                        GVQuotation.DataBind();
                    }
                    if (txtbillingcustomer.Text != "")
                    {
                        string company = txtbillingcustomer.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy where QH.Companyname = '" + company + "' AND QH.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVQuotation.EmptyDataText = "Not Records Found";
                        GVQuotation.DataSource = dt;
                        GVQuotation.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy WHERE  QH.IsDeleted = 0 AND QH.Quotationdate between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        sad.Fill(dt);

                        GVQuotation.EmptyDataText = "Not Records Found";
                        GVQuotation.DataSource = dt;
                        GVQuotation.DataBind();
                    }

                }
                else
                {
                    if (txtQuotationNo.Text != "")
                    {
                        string Quono = txtQuotationNo.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy where QH.CreatedBy='" + Session["UserCode"].ToString() + "'  AND QH.Quotationno = '" + Quono + "' AND QH.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVQuotation.EmptyDataText = "Not Records Found";
                        GVQuotation.DataSource = dt;
                        GVQuotation.DataBind();
                    }
                    if (txtbillingcustomer.Text != "")
                    {
                        string company = txtbillingcustomer.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy where QH.CreatedBy='" + Session["UserCode"].ToString() + "'  AND QH.Companyname = '" + company + "' AND QH.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVQuotation.EmptyDataText = "Not Records Found";
                        GVQuotation.DataSource = dt;
                        GVQuotation.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy WHERE QH.CreatedBy='" + Session["UserCode"].ToString() + "'  AND  IsDeleted = 0 AND QH.Quotationdate between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        sad.Fill(dt);

                        GVQuotation.EmptyDataText = "Not Records Found";
                        GVQuotation.DataSource = dt;
                        GVQuotation.DataBind();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    //Search Company
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
                com.CommandText = "SELECT DISTINCT [Companyname] FROM [tbl_QuotationHdr] where " + "Companyname like '%'+ @Search + '%' and IsDeleted=0";

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
        if (txtbillingcustomer.Text != "")
        {
            string company = txtbillingcustomer.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy where QH.Companyname = '" + company + "' AND QH.IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVQuotation.EmptyDataText = "Not Records Found";
            GVQuotation.DataSource = dt;
            GVQuotation.DataBind();
        }
    }

    //Search Quotation No.
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetQuotationNoList(string prefixText, int count)
    {
        return AutoFillQuotationNoList(prefixText);
    }

    public static List<string> AutoFillQuotationNoList(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [Quotationno] FROM [tbl_QuotationHdr] where " + "Quotationno like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Quotationno"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtQuotationNo_TextChanged(object sender, EventArgs e)
    {
        if (txtQuotationNo.Text != "")
        {
            string Quono = txtQuotationNo.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_QuotationHdr] AS QH INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=QH.CreatedBy where QH.Quotationno = '" + Quono + "' AND QH.IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVQuotation.EmptyDataText = "Not Records Found";
            GVQuotation.DataSource = dt;
            GVQuotation.DataBind();
        }
    }
}