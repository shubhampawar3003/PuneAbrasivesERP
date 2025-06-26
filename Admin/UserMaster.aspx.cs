using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_UserMaster : System.Web.UI.Page
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
                UserCode(); FillddlRole();

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
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_UserMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            txtusercode.Text = "PAPL/UID-" + (maxid + 1).ToString();
        }
        else
        {
            txtusercode.Text = string.Empty;
        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_UserMaster WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            txtusernmae.Text = Dt.Rows[0]["Username"].ToString();
            txtusercode.Text = Dt.Rows[0]["UserCode"].ToString();
            txtmobileno.Text = Dt.Rows[0]["Mobileno"].ToString();
            txtemailid.Text = Dt.Rows[0]["EmailID"].ToString();
            txtpassword.Text = Dt.Rows[0]["Password"].ToString();
            ddlrole.SelectedItem.Text = Dt.Rows[0]["Role"].ToString();
            ddldesignation.SelectedItem.Text = Dt.Rows[0]["Designation"].ToString();
            txtpanno.Text = Dt.Rows[0]["Panno"].ToString();
            txtaadharno.Text = Dt.Rows[0]["Aadharno"].ToString();
            txtaccountdetails.Text = Dt.Rows[0]["Accountdetails"].ToString();
            DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["Birthdate"].ToString());
            txtbirthdate.Text = ffff1.ToString("yyyy-MM-dd");
            txtaddres.Text = Dt.Rows[0]["Address"].ToString();
            txtEmailIDPASS.Text = Dt.Rows[0]["EmailIDPass"].ToString();
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
    private void FillddlRole()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT [RoleName] FROM [tbl_Role] where IsDeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlrole.DataSource = dt;
            ddlrole.DataTextField = "RoleName";
            ddlrole.DataBind();
            ddlrole.Items.Insert(0, "-- Select Role --");
        }
    }


    //Save and Update Record
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtusernmae.Text == "" || txtmobileno.Text == "" || txtpassword.Text == "" || txtemailid.Text == ""  || ddlrole.SelectedItem.Text == "--Select Role--" || ddldesignation.SelectedItem.Text == "--Select Designation--")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_UserMaster WHERE EmailID = '" + txtemailid.Text + "' AND IsDeleted='0'");
                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_UserMaster", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                    Cmd.Parameters.AddWithValue("@Username", txtusernmae.Text.Trim());
                    Cmd.Parameters.AddWithValue("@UserCode", txtusercode.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text.Trim());
                    Cmd.Parameters.AddWithValue("@EmailID", txtemailid.Text.Trim());
                    Cmd.Parameters.AddWithValue("@EmailIDPass", txtEmailIDPASS.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Password", txtpassword.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Role", ddlrole.SelectedItem.Text);
                    Cmd.Parameters.AddWithValue("@Designation", ddldesignation.SelectedItem.Text);
                    Cmd.Parameters.AddWithValue("@Panno", txtpanno.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Aadharno", txtaadharno.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Accountdetails", txtaccountdetails.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Birthdate", txtbirthdate.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Address", txtaddres.Text.Trim());
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Update Successfully..!!');window.location='UserMasterList.aspx'; ", true);
                }
                else
                {
                    if (Dt.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Alredy Exists..!!')", true);
                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_UserMaster", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                        Cmd.Parameters.AddWithValue("@Username", txtusernmae.Text.Trim());
                        Cmd.Parameters.AddWithValue("@UserCode", txtusercode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@EmailID", txtemailid.Text.Trim());
                        Cmd.Parameters.AddWithValue("@EmailIDPass", txtEmailIDPASS.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Password", txtpassword.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Role", ddlrole.SelectedItem.Text);
                        Cmd.Parameters.AddWithValue("@Designation", ddldesignation.SelectedItem.Text);
                        Cmd.Parameters.AddWithValue("@Panno", txtpanno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Aadharno", txtaadharno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Accountdetails", txtaccountdetails.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Birthdate", txtbirthdate.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Address", txtaddres.Text.Trim());
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
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Added Successfully..!!');window.location='UserMasterList.aspx'; ", true);
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
        Response.Redirect("UserMasterList.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("UserMasterList.aspx");
    }
}