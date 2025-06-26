
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_ComponentMaster : System.Web.UI.Page
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
                ComponentCode();
                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);

                }
            }
        }
    }



    //Data Fetch
    private void Load_Record(string ID)
    {
        try
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ComponentMaster] WHERE ID ='" + ID + "' ");
            if (Dt.Rows.Count > 0)
            {
                btnsave.Text = "Update";

                txtGarde.Text = Dt.Rows[0]["Grade"].ToString();
                txtComponentname.Text = Dt.Rows[0]["ComponentName"].ToString();
                txtComponentcode.Text = Dt.Rows[0]["ComponentCode"].ToString();
                txtHSN.Text = Dt.Rows[0]["HSN"].ToString();
                ddlUnit.SelectedValue = Dt.Rows[0]["Unit"].ToString();
                txtQtylimit.Text = Dt.Rows[0]["LessQtyLimit"].ToString();

                if (Dt.Rows[0]["Status"].ToString() == "False")
                {
                    chkisactive.Checked = false;
                }
                else
                {
                    chkisactive.Checked = true;
                }
            }
        }
        catch
        {

        }
    }

    //Brand Code Auto
    protected void ComponentCode()
    {
        try
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_ComponentMaster]", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
                txtComponentcode.Text = "PAPL/CopT-" + (maxid + 1).ToString();
            }
            else
            {
                txtComponentcode.Text = string.Empty;
            }
        }
        catch
        {

        }
    }


    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtComponentname.Text == "" || txtHSN.Text == "" || txtComponentcode.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_ComponentMaster WHERE  ComponentName = '" + txtComponentname.Text + "'  AND IsDeleted='0'");
                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("[SP_ComponentMaster]", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                    Cmd.Parameters.AddWithValue("@Unit", ddlUnit.SelectedValue);
                    //Cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());                 
                    Cmd.Parameters.AddWithValue("@LessQtyLimit", txtQtylimit.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Grade", txtGarde.Text.Trim());
                    Cmd.Parameters.AddWithValue("@HSN", txtHSN.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Componentname", txtComponentname.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Componentcode", txtComponentcode.Text.Trim());
                    Cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                    bool Status = true;
                    if (chkisactive.Checked == true)
                    {
                        Status = true;
                    }
                    else
                    {
                        Status = false;
                    }
                    Cmd.Parameters.AddWithValue("@Status", Status);
                    Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                    Cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Component Update Successfully..!!');window.location='ComponentList.aspx'; ", true);
                }
                else
                {
                    if (Dt.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Data Alredy Exists..!!')", true);
                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("[SP_ComponentMaster]", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@Unit", ddlUnit.SelectedValue);
                        Cmd.Parameters.AddWithValue("@LessQtyLimit", txtQtylimit.Text.Trim());
                        //Cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
                        //Cmd.Parameters.AddWithValue("@Tax", txtTax.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Grade", txtGarde.Text.Trim());
                        Cmd.Parameters.AddWithValue("@HSN", txtHSN.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Componentname", "C-" + txtComponentname.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Componentcode", txtComponentcode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());

                        bool Status = true;
                        if (chkisactive.Checked == true)
                        {
                            Status = true;
                        }
                        else
                        {
                            Status = false;
                        }
                        Cmd.Parameters.AddWithValue("@Status", Status);
                        Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Component Added Successfully..!!');window.location='ComponentList.aspx'; ", true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("ComponentList.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("ComponentList.aspx");
    }
}