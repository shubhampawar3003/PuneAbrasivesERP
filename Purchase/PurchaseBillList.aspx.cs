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
using System.Net;
using Microsoft.Reporting.WebForms;

public partial class Purchase_PurchaseBillList : System.Web.UI.Page
{
    CommonCls obj = new CommonCls();
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
                int jobCount = GetJobCount();
                lblcount.Text = jobCount.ToString();
                if (Convert.ToInt32(lblcount.Text) > 0)
                {
                    lnkshow.Attributes.Add("class", "bell-bounce");
                }
                else
                {
                    lnkshow.Attributes.Remove("class");
                }
                Gvbind();
            }
        }
    }
    private void Gvbind()
    {
        string query = string.Empty;
        query = @"select CONVERT(nvarchar(10),BillDate,105) AS BillDate,* from tblPurchaseBillHdr PB
INNER JOIN  tbl_UserMaster AS UM ON UM.usercode=PB.Createdby order by PB.Id desc";
        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvPurchaseBill.DataSource = dt;
            GvPurchaseBill.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvPurchaseBill.ClientID + "', 500, 1020 , 40 ,true); </script>", false);
        }
        else
        {
            GvPurchaseBill.DataSource = null;
            GvPurchaseBill.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvPurchaseBill.ClientID + "', 500, 1020 , 40 ,true); </script>", false);
        }
    }
    protected void GvPurchaseBill_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ViewState["CompRowId"] = e.CommandArgument.ToString();
        if (e.CommandName == "RowEdit")
        {
            ViewState["id"] = e.CommandArgument.ToString();
            Response.Redirect("PurchaseBillEntry.aspx?ID=" + obj.encrypt(e.CommandArgument.ToString()));
        }
        if (e.CommandName == "Suppliername")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                //GetSupplierDataPopup(e.CommandArgument.ToString());
                //this.modelprofile.Show();
            }
        }

        if (e.CommandName == "DownloadPDF")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                //Report(e.CommandArgument.ToString());
                Session["PDFID"] = e.CommandArgument.ToString();
                // Response.Write("<script>window.open('PurchaseBillPDF.aspx','_blank');</script>");
                Response.Redirect("PurchaseBillPDF.aspx?Id=" + obj.encrypt(e.CommandArgument.ToString()) + " ");
            }
        }

        if (e.CommandName == "RowDelete")
        {
            con.Open();
            SqlCommand cmdget = new SqlCommand("select AgainstNumber from tblPurchaseBillHdr where Id=@Id", con);
            cmdget.Parameters.AddWithValue("@Id", Convert.ToInt32(e.CommandArgument.ToString()));
            string POno = cmdget.ExecuteScalar().ToString();

            SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed =null where PONo=@PONO", con);
            cmdupdate.Parameters.AddWithValue("@PONO", POno);
            cmdupdate.ExecuteNonQuery();
            con.Close();

            con.Open();
            SqlCommand Cmd = new SqlCommand("Delete From [tblPurchaseBillHdr] WHERE Id=@Id", con);
            Cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.ExecuteNonQuery();
            con.Close();

            con.Open();
            SqlCommand Cmdd = new SqlCommand("Delete From [tblPurchaseBillDtls] WHERE BillNo=@HeaderID", con);
            Cmdd.Parameters.AddWithValue("@HeaderID", POno);
            Cmdd.ExecuteNonQuery();
            con.Close();


            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Deleted Sucessfully');window.location.href='PurchaseBillList.aspx';", true);
        }


    }
    #region Filter
    protected void txtcnamefilter_TextChanged(object sender, EventArgs e)
    {
        string query = string.Empty;
        if (!string.IsNullOrEmpty(txtcnamefilter.Text.Trim()))
        {
            query = "SELECT CONVERT(nvarchar(10),BillDate,105) AS BillDate,* FROM tblPurchaseBillHdr where SupplierName like '%" + txtcnamefilter.Text.Trim() + "%' order by Id desc";
        }
        else
        {
            query = "SELECT CONVERT(nvarchar(10),BillDate,105) AS BillDate,* FROM tblPurchaseBillHdr where order by Id desc";
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvPurchaseBill.DataSource = dt;
            GvPurchaseBill.DataBind();
        }
        else
        {
            GvPurchaseBill.DataSource = null;
            GvPurchaseBill.DataBind();
        }
    }
    protected void txtSupplierBill_TextChanged(object sender, EventArgs e)
    {
        string query = string.Empty;
        if (!string.IsNullOrEmpty(txtSupplierBill.Text.Trim()))
        {
            query = "SELECT CONVERT(nvarchar(10),BillDate,105) AS BillDate,* FROM tblPurchaseBillHdr where SupplierBillNo='" + txtSupplierBill.Text.Trim() + "' order by Id desc";
        }
        else
        {
            query = "SELECT CONVERT(nvarchar(10),BillDate,105) AS BillDate,* FROM tblPurchaseBillHdr order by Id desc";
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvPurchaseBill.DataSource = dt;
            GvPurchaseBill.DataBind();
        }
        else
        {
            GvPurchaseBill.DataSource = null;
            GvPurchaseBill.DataBind();
        }
    }
    #endregion Filter
    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseBillList.aspx");
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
                com.CommandText = "Select DISTINCT [VendorName] from [tbl_VendorMaster] where " + "VendorName like '%'+ @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["VendorName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetbillnoList(string prefixText, int count)
    {
        return AutoFillbillno(prefixText);
    }
    public static List<string> AutoFillbillno(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT SupplierBillNo FROM tblPurchaseBillHdr where SupplierBillNo like @Search + '%' ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["SupplierBillNo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    private string GetMailIdOfEmpl(string Empcode)
    {
        string query1 = "SELECT [email] FROM [employees] where [empcode]='" + Empcode + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        string email = string.Empty;
        if (dt.Rows.Count > 0)
        {
            email = dt.Rows[0]["email"].ToString();
        }
        return email;
    }
    protected void ddlsalesMainfilter_TextChanged(object sender, EventArgs e)
    {
        Gvbind();
    }
    protected void btnAddEnq_Click(object sender, EventArgs e)
    {
        string Cname = ((sender as Button).CommandArgument).ToString();
        Response.Redirect("AddEnquiry.aspx?Cname=" + obj.encrypt(Cname));
        //Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid=" + id + "','','width=700px,height=600px');", true);
    }
    protected void linkbtnfile_Click(object sender, EventArgs e)
    {
        string id = obj.encrypt(((sender as ImageButton).CommandArgument).ToString());

        string POID = obj.Decrypt(id);

        string strQuery = "select SupplierName,RefDocument from tblPurchaseBillHdr where Id=@id";
        SqlCommand cmd = new SqlCommand(strQuery);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(POID);
        DataTable dt = GetData(cmd);
        if (dt != null)
        {
            download(dt);
        }
        //Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid=" + id + "&SN=1','','width=700px,height=600px');", true);
    }
    private DataTable GetData(SqlCommand cmd)
    {
        DataTable dt = new DataTable();
        String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        SqlConnection con = new SqlConnection(strConnString);
        SqlDataAdapter sda = new SqlDataAdapter();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        try
        {
            con.Open();
            sda.SelectCommand = cmd;
            sda.Fill(dt);
            return dt;
        }
        catch
        {
            return null;
        }
        finally
        {
            con.Close();
            sda.Dispose();
            con.Dispose();
        }
    }
    private void download(DataTable dt)
    {
        if (dt.Rows[0]["RefDocument"].ToString() != "")
        {
            Byte[] bytes = (Byte[])dt.Rows[0]["RefDocument"];
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = dt.Rows[0]["ContentType"].ToString();
            Response.AddHeader("content-disposition", "attachment;filename=" + dt.Rows[0]["SupplierName"].ToString());
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('File Not Found !!');", true);
        }
    }
    protected void GvPurchaseBill_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
            LinkButton btnPDF = e.Row.FindControl("btnPDF") as LinkButton;
            string empcode = Session["UserCode"].ToString();
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                string id = Dt.Rows[0]["ID"].ToString();
                DataTable Dtt = new DataTable();
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'ProformaInvoiceList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    LinkButton1.Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                    btnPDF.Visible = true;
                }
            }

        }
    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseBillEntry.aspx");
    }
    //---------------------------------------------------------------------------
    public void Report(string BillNo)
    {

        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_PurchaseBill]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetBillDetails");
                    cmd.Parameters.AddWithValue("@BillNo", BillNo);

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
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PurchaseBill.rdlc";
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
                    string Filename = BillNo + "_PurchaseBill.pdf";
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

    // New Bell icon by Nikhil 05-05-2025
    protected void lnkshow_Click(object sender, EventArgs e)
    {

        string query = string.Empty;
        query = @"SELECT * FROM tbl_PendingInwardHdr WHERE orderno NOT IN (SELECT OrderNo from tblPurchaseBillHdr) order by Id desc";
        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            gv_EstimationList.DataSource = dt;
            gv_EstimationList.DataBind();
            modelprofile.Show();
        }
        else
        {
            gv_EstimationList.DataSource = "No Data Found";
            gv_EstimationList.DataBind();
            modelprofile.Show();
        }

    }
    public static int GetJobCount()
    {
        int jobCount = 0;
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(connString))
        {

            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tbl_PendingInwardHdr WHERE orderno NOT IN(SELECT OrderNo from tblPurchaseBillHdr)", con);
            con.Open();
            jobCount = (int)cmd.ExecuteScalar();
        }

        return jobCount;
    }
    protected void gv_EstimationList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RowCorrect")
        {
            Response.Redirect("PurchaseBillEntry.aspx?OrderNo=" + obj.encrypt(e.CommandArgument.ToString()));
        }
    }
}