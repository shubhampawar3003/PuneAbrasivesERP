
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_EnquiryList : System.Web.UI.Page
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
                //DdlSalesBind();
                Gvbind();
                if (Session["Role"].ToString() != "Admin")
                {
                    ViewAuthorization();
                }

            }
        }
    }

    protected void GvCompany_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvCompany.PageIndex = e.NewPageIndex;
        Gvbind();
    }

    private void ViewAuthorization()
    {
        string empcode = Session["UserCode"].ToString();
        DataTable Dt = new DataTable();
        SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
        Sd.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            string id = Dt.Rows[0]["ID"].ToString();
            DataTable Dtt = new DataTable();
            SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'EnquiryList.aspx' AND PagesView = '1'", con);
            Sdd.Fill(Dtt);
            if (Dtt.Rows.Count > 0)
            {
                GvCompany.Columns[12].Visible = false;
                Button1.Visible = false;
            }
        }
    }

    protected void GvCompany_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ViewState["CompRowId"] = e.CommandArgument.ToString();
        if (e.CommandName == "RowEdit")
        {
            ViewState["id"] = e.CommandArgument.ToString();
            Response.Redirect("EnquiryMaster.aspx?code=" + objcls.encrypt(e.CommandArgument.ToString()));
        }
        if (e.CommandName == "Createquotation")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            // Retrieve the ID using DataKey value
            //string id = GvCompany.DataKeys[index].Values["id"].ToString();
            //int rowIndex = Convert.ToInt32(e.CommandArgument);            
            //string id = GvCompany.DataKeys[rowIndex]["ID"].ToString();
            ViewState["EID"] = e.CommandArgument.ToString();
            Response.Redirect("QuatationMaster.aspx?CODE=" + objcls.encrypt(e.CommandArgument.ToString()));
        }


        if (e.CommandName == "DeleteData")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                SqlCommand cmd = new SqlCommand("UPDATE [dbo].[tbl_EnquiryData] SET [IsActive] = 0 WHERE id='" + e.CommandArgument.ToString() + "'", con);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('Enquiry Deleted Sucessfully');window.location='EnquiryList.aspx';", true);
            }
        }
    }

    private void Gvbind()
    {
        string query = string.Empty;
        if (Session["Role"].ToString() == "Admin")
        {
            if (!string.IsNullOrEmpty(txtCustomerName.Text.Trim()) && (txtfromdate.Text != null && txtfromdate.Text != "") && (txttodate.Text != null && txttodate.Text != ""))
            {
                query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where  cname='" + txtCustomerName.Text.Trim() + "' AND [regdate] between CONVERT(VARCHAR(10),'" + txtfromdate.Text + "', 105) AND CONVERT(VARCHAR(10), '" + txttodate.Text + "', 105) AND IsActive=1 AND status=1 order by status desc,id desc";
            }
            else
            if ((txtfromdate.Text != null && txtfromdate.Text != "") && (txttodate.Text != null && txttodate.Text != ""))
            {
                query = @"SELECT  [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where [regdate] between  CONVERT(VARCHAR(10),'" + txtfromdate.Text + "', 105)  AND CONVERT(VARCHAR(10),'" + txttodate.Text + "', 105) AND  IsActive=1 AND status=1 order by status desc,id desc";
            }
            else
            if (!string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where cname like '" + txtCustomerName.Text.Trim() + "%' AND IsActive=1 AND status=1 order by status desc,id desc";
            }
            else
            if (!string.IsNullOrEmpty(ddlStatus.SelectedItem.Text.Trim()))
            {
                query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where status='" + ddlStatus.SelectedValue + "' AND IsActive=1 order by status desc,id desc";
            }
            //else
            //{
            //    query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where IsActive=1 AND status=1 order by status desc,id desc";
            //}
        }
        else
        {
            if (!string.IsNullOrEmpty(txtCustomerName.Text.Trim()) && (txtfromdate.Text != null && txtfromdate.Text != "") && (txttodate.Text != null && txttodate.Text != ""))
            {
                query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where sessionname='" + Session["UserCode"].ToString() + "'  AND  cname='" + txtCustomerName.Text.Trim() + "' AND [regdate] between CONVERT(VARCHAR(10),'" + txtfromdate.Text + "', 105) AND CONVERT(VARCHAR(10), '" + txttodate.Text + "', 105) AND IsActive=1 AND status=1 order by status desc,id desc";
            }
            else
            if ((txtfromdate.Text != null && txtfromdate.Text != "") && (txttodate.Text != null && txttodate.Text != ""))
            {
                query = @"SELECT  [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where sessionname='" + Session["UserCode"].ToString() + "'  AND [regdate] between  CONVERT(VARCHAR(10),'" + txtfromdate.Text + "', 105)  AND CONVERT(VARCHAR(10),'" + txttodate.Text + "', 105) AND  IsActive=1 AND status=1 order by status desc,id desc";
            }
            else
            if (!string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where sessionname='" + Session["UserCode"].ToString() + "'  AND cname like '" + txtCustomerName.Text.Trim() + "%' AND IsActive=1 AND status=1 order by status desc,id desc";
            }
            else
            if (!string.IsNullOrEmpty(ddlStatus.SelectedItem.Text.Trim()))
            {
                query = @"SELECT [id],[EnqCode],[ccode],[cname],[filepath1],[filepath2],[filepath3],[filepath4],[filepath5],case when status=1 then 'Open' else 'Close' END AS status,convert(varchar(20),[regdate],105) as [regdate],[sessionname],IsActive, CASE WHEN Sample=1 then 'Yes' ELSE 'No' END AS Sample,convert(varchar(20),[SampleDate],105) as [SampleDate] FROM [tbl_EnquiryData] where sessionname='" + Session["UserCode"].ToString() + "'  AND status='" + ddlStatus.SelectedValue + "' AND IsActive=1 order by status desc,id desc";
            }
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvCompany.DataSource = dt;
            GvCompany.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvCompany.ClientID + "', 900, 1020 , 40 ,true); </script>", false);
        }
        else
        {
            GvCompany.DataSource = null;
            GvCompany.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvCompany.ClientID + "', 900, 1020 , 40 ,true); </script>", false);
        }
    }

    protected void linkbtnfile_Click(object sender, EventArgs e)
    {
        string id = objcls.encrypt(((sender as ImageButton).CommandArgument).ToString());

        Response.Redirect("EnquiryFile.aspx?Fileid1=" + id + "&SN=1','','width=700px,height=600px'");
        //  Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid1=" + id + "&SN=1','','width=700px,height=600px');", true);
    }

    protected void linkbtnfile2_Click(object sender, EventArgs e)
    {
        string id = objcls.encrypt(((sender as ImageButton).CommandArgument).ToString());
        Response.Redirect("EnquiryFile.aspx?Fileid2=" + id + "&SN=1','','width=700px,height=600px'");
        // Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid2=" + id + "&SN=2','','width=700px,height=600px');", true);
    }

    protected void linkbtnfile3_Click(object sender, EventArgs e)
    {
        string id = objcls.encrypt(((sender as ImageButton).CommandArgument).ToString());
        Response.Redirect("EnquiryFile.aspx?Fileid3=" + id + "&SN=1','','width=700px,height=600px'");
        // Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid3=" + id + "&SN=3','','width=700px,height=600px');", true);
    }

    protected void linkbtnfile4_Click(object sender, EventArgs e)
    {
        string id = objcls.encrypt(((sender as ImageButton).CommandArgument).ToString());
        Response.Redirect("EnquiryFile.aspx?Fileid4=" + id + "&SN=1','','width=700px,height=600px'");
        //  Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid4=" + id + "&SN=4','','width=700px,height=600px');", true);
    }

    protected void linkbtnfile5_Click(object sender, EventArgs e)
    {
        string id = objcls.encrypt(((sender as ImageButton).CommandArgument).ToString());
        Response.Redirect("EnquiryFile.aspx?Fileid5=" + id + "&SN=1','','width=700px,height=600px'");
        // Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid5=" + id + "&SN=5','','width=700px,height=600px');", true);
    }

    protected void GvCompany_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label file1 = e.Row.FindControl("lblfilepath1") as Label;
            Label file2 = e.Row.FindControl("lblfilepath2") as Label;
            Label file3 = e.Row.FindControl("lblfilepath3") as Label;
            Label file4 = e.Row.FindControl("lblfilepath4") as Label;
            Label file5 = e.Row.FindControl("lblfilepath5") as Label;

            ImageButton ImageButtonfile1 = e.Row.FindControl("ImageButtonfile1") as ImageButton;
            ImageButton ImageButtonfile2 = e.Row.FindControl("ImageButtonfile2") as ImageButton;
            ImageButton ImageButtonfile3 = e.Row.FindControl("ImageButtonfile3") as ImageButton;
            ImageButton ImageButtonfile4 = e.Row.FindControl("ImageButtonfile4") as ImageButton;
            ImageButton ImageButtonfile5 = e.Row.FindControl("ImageButtonfile5") as ImageButton;

            if (string.IsNullOrEmpty(file1.Text))
            {
                ImageButtonfile1.Enabled = false;
                ImageButtonfile1.ToolTip = "File Not Available";
            }
            if (string.IsNullOrEmpty(file2.Text))
            {
                ImageButtonfile2.Enabled = false;
                ImageButtonfile2.ToolTip = "File Not Available";
            }
            if (string.IsNullOrEmpty(file3.Text))
            {
                ImageButtonfile3.Enabled = false;
                ImageButtonfile3.ToolTip = "File Not Available";
            }
            if (string.IsNullOrEmpty(file4.Text))
            {
                ImageButtonfile4.Enabled = false;
                ImageButtonfile4.ToolTip = "File Not Available";
            }
            if (string.IsNullOrEmpty(file5.Text))
            {
                ImageButtonfile5.Enabled = false;
                ImageButtonfile5.ToolTip = "File Not Available";
            }

            //Label lblstatus1 = e.Row.FindControl("status") as Label;
            //Label lblstatus2 = e.Row.FindControl("lblstatus2") as Label;
            //if (lblstatus1.Text == "Open")
            //{
            //    lblstatus2.Text = "Open";
            //    lblstatus2.ForeColor = Color.Green;
            //}
            //else
            //{
            //    lblstatus2.Text = "Close";
            //    lblstatus2.ForeColor = Color.Red;
            //}
            //Label lblIsActive = e.Row.FindControl("lblIsActive") as Label;
            //Button btnEdit = e.Row.FindControl("Button4") as Button;
            //Button btnsendquot = e.Row.FindControl("btnsendquot") as Button;
            //LinkButton Linkbtndelete = e.Row.FindControl("Linkbtndelete") as LinkButton;
            //if (lblIsActive.Text == "False")
            //{
            //    btnEdit.Visible = false;
            //    btnsendquot.Visible = false;
            //    Linkbtndelete.Visible = false;
            //}

        }
    }

    protected void btnAddEnquiry_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryMaster.aspx");
    }



    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryList.aspx");
    }

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        Gvbind();
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
                com.CommandText = "select DISTINCT cname from tbl_EnquiryData where  " + "cname like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["cname"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        Gvbind();
    }



    protected void ddlfollowupStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        Gvbind();
    }
}