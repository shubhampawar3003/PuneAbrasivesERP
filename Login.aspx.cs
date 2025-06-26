using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web.Security;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;

public partial class Login : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            if (Request.Cookies["Username"] != null)
                txtUsername.Text = Request.Cookies["Username"].Value;
            if (Request.Cookies["Password"] != null)
                txtPassword.Attributes.Add("value", Request.Cookies["Password"].Value);
            if (Request.Cookies["Username"] != null && Request.Cookies["Password"] != null)
                chkremember.Checked = true;
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        Cls_Main.Conn_Open();
        SqlCommand cmd = new SqlCommand("SELECT * FROM tbl_UserMaster WHERE EmailID='" + txtUsername.Text + "' AND Password='" + txtPassword.Text + "'", con);
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
        cmd.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
        cmd.Connection.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        if (dr.HasRows)
        {
            while (dr.Read())
            {

                string Username = dr["Username"].ToString();
                string Role = dr["Role"].ToString();
                string status = dr["Status"].ToString();
                if (status == "True")
                {
                    FormsAuthentication.SetAuthCookie(txtUsername.Text.Trim(), chkremember.Checked);

                    if (chkremember.Checked)
                    {
                        Response.Cookies["Username"].Value = txtUsername.Text.ToLower().Trim();
                        Response.Cookies["Password"].Value = txtPassword.Text.Trim();
                        Response.Cookies["Username"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(30);
                    }
                    else
                    {
                        // Remove the cookie if "Remember Me" is not checked
                        if (Request.Cookies["RememberMe"] != null)
                        {
                            Response.Cookies["Username"].Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                        }
                    }
                    if (!string.IsNullOrEmpty(Username))
                    {
                        Session["ID"] = dr["ID"].ToString();
                        Session["Username"] = dr["Username"].ToString();
                        Session["Role"] = dr["Role"].ToString();
                        Session["EmailID"] = dr["EmailID"].ToString();
                        Session["Mobileno"] = dr["Mobileno"].ToString();
                        Session["UserCode"] = dr["UserCode"].ToString();
                        Session["Designation"] = dr["Designation"].ToString();
                        txtUsername.Text = ""; txtPassword.Text = "";
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("INSERT INTO [tbl_LoginInfo] ([CreatedBy] ,[CreatedOn] ,[UserName]) VALUES (@CreatedBy ,@CreatedOn ,@UserName)", Cls_Main.Conn);
                        Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                        Cmd.Parameters.AddWithValue("@UserName", Session["Username"].ToString());
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        // ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabel('Login Successfully..!!')", true);
                        Response.Redirect("Admin/Dashboard.aspx");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Login Failed, Activate Your Account First..!!');window.location='Login.aspx'; ", true);
                    // ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabelerror('Login Failed, Activate Your Account First..!!')", true);
                    txtUsername.Text = ""; txtPassword.Text = "";
                }
            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Login Failed, Incorrect Username or Password..!!');window.location='Login.aspx'; ", true);
            // ClientScript.RegisterStartupScript(this.GetType(), "alert", "HideLabelerror('Login Failed, Incorrect Username or Password..!!')", true);
            txtUsername.Text = ""; txtPassword.Text = "";
        }
        cmd.Connection.Close();
    }

}

