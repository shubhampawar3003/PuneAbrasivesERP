using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_AddRole : System.Web.UI.Page
{
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
                UserCode();

                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                }
            }
        }
    }

    //Company Code Auto
    protected void UserCode()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([ID]) as maxid FROM [tbl_Role]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            txtRolecode.Text = "PAPL/R-" + (maxid + 1).ToString();
        }
        else
        {
            txtRolecode.Text = string.Empty;
        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_Role WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            txtRolename.Text = Dt.Rows[0]["RoleName"].ToString();
            txtRolecode.Text = Dt.Rows[0]["RoleCode"].ToString();
      
           
        }
    }

   

    //Save and Update Record
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtRolecode.Text == "" || txtRolename.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_Role WHERE RoleName = '" + txtRolename.Text + "' AND IsDeleted='0'");
                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_Role", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");                    
                    Cmd.Parameters.AddWithValue("@RoleName", txtRolename.Text.Trim());
                    Cmd.Parameters.AddWithValue("@RoleCode", txtRolecode.Text.Trim());                   
                    Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now); 
                    Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                    Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Role Update Successfully..!!');window.location='RoleList.aspx'; ", true);
                }
                else
                {
                    if (Dt.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Role Alredy Exists..!!')", true);
                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_Role", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");                      
                        Cmd.Parameters.AddWithValue("@RoleName", txtRolename.Text.Trim());
                        Cmd.Parameters.AddWithValue("@RoleCode", txtRolecode.Text.Trim());                       
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());                      
                        Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Role Added Successfully..!!');window.location='RoleList.aspx'; ", true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //Page Redirect/Refresh
    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("RoleList.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("RoleList.aspx");
    }
}