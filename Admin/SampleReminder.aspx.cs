using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_SampleReminder : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
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
                FillGrid(); MarkNotificationsAsSeen();
            }
        }
    }

    //Fill GridView
    private void FillGrid()
    {

        DataTable dt = new DataTable();
        if (Session["Role"].ToString() == "Admin")
        {
            SqlDataAdapter sad = new SqlDataAdapter("select id, cname,EnqCode,CONVERT(nvarchar(10),SampleDate,105) AS SampleDate from tbl_EnquiryData where IsActive=1 AND Sample=1 AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE) ORDER BY SampleDate DESC", Cls_Main.Conn);
            sad.Fill(dt);
        }
        else
        {
            SqlDataAdapter sad = new SqlDataAdapter("select id, cname,EnqCode,CONVERT(nvarchar(10),SampleDate,105) AS SampleDate from tbl_EnquiryData where IsActive=1 AND Sample=1 AND  sessionname='" + Session["UserCode"].ToString() + "'  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE) ORDER BY CONVERT(nvarchar(10),SampleDate,105) DESC", Cls_Main.Conn);
            sad.Fill(dt);
        }

        if (dt.Rows.Count > 0)
        {
            grdnotification.DataSource = dt;
            grdnotification.DataBind();
        }

    }

    public  void MarkNotificationsAsSeen()
    {

        Cls_Main.Conn_Open();
        SqlCommand Cmd = new SqlCommand("update tbl_EnquiryData set Notiification=1 where IsActive=1 AND  Notiification=0  AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE)", Cls_Main.Conn);
        //  Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
        //  Cmd.Parameters.AddWithValue("@IsDeleted", '1');     
        Cmd.ExecuteNonQuery();
        Cls_Main.Conn_Close();

    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddRole.aspx");
    }

    protected void grdnotification_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdnotification.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void grdnotification_RowDataBound(object sender, GridViewRowEventArgs e)
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'RoleList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                   
                    grdnotification.Columns[3].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }
}