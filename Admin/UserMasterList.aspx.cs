using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_UserMasterList : System.Web.UI.Page
{
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
                FillGrid();
            }
        }
    }

    //Fill GridView
    private void FillGrid()
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_UserMaster WHERE IsDeleted = 0");
        GVUser.DataSource = Dt;
        GVUser.DataBind();
    }

   

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("UserMaster.aspx");
    }

    protected void GVUser_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("UserMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_UserMaster] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Deleted Successfully..!!')", true);
            FillGrid();
        }
    }

    protected void GVUser_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVUser.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVUser_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton Lnk_Delete = (LinkButton)e.Row.FindControl("btnDelete");
            LinkButton Lnk_Edit = (LinkButton)e.Row.FindControl("btnEdit");
            //  DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_UserMaster] WHERE Username='" + Session["UserCode"].ToString() + "'");
            //if (Dt.Rows.Count > 0)
            //{
            //    string Role = Dt.Rows[0]["Role"].ToString();
            //    if (Role == "Super User" || Role == "User")
            //    {
            //        Lnk_Delete.Visible = false;
            //        Lnk_Edit.Visible = false;
            //        GVUser.Columns[6].Visible = false;
            //    }
            //    else
            //    {
            //        Lnk_Delete.Visible = true;
            //        Lnk_Edit.Visible = true;
            //    }
            //}
        }
    }
}