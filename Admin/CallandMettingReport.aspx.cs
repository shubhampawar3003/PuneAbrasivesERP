
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

public partial class Admin_CallandMettingReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
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
                BindDdl_Username();
                GridView();
                ddlfollowupStatus.Items.Add(new ListItem("Follow-Up", "2"));
            }
        }
    }
    //Fill GridView

    void GridView()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_GetCallandMeetingDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Role", Session["Role"].ToString());
            cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
            if (ddlfollowup.SelectedItem.Text == "- Select Type of Update -" || ddlfollowup.SelectedItem.Text == null)
            {
                cmd.Parameters.AddWithValue("@Updatefor", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Updatefor", ddlfollowup.SelectedItem.Text);
            }
            if (ddlfollowupStatus.SelectedItem.Text == "- Select Status of Update -" || ddlfollowupStatus.SelectedItem.Text == null)
            {
                cmd.Parameters.AddWithValue("@UpdateStatus", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@UpdateStatus", ddlfollowupStatus.SelectedItem.Text);
            }
            if (ddlTeamuser.SelectedItem.Text == null || ddlTeamuser.SelectedItem.Text == "--Select--")
            {
                cmd.Parameters.AddWithValue("@Teamuser", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Teamuser", ddlTeamuser.SelectedValue);
            }

            if (ddltype.SelectedItem.Text == null || ddltype.SelectedItem.Text == "- Select Type -")
            {
                cmd.Parameters.AddWithValue("@Type", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@Type", ddltype.SelectedValue);
            }

            if (txtcompanyname.Text == null || txtcompanyname.Text == "")
            {
                cmd.Parameters.AddWithValue("@CompanyName", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@CompanyName", txtcompanyname.Text);
            }

            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Columns.Count > 0)
            {
                dt.Columns.Add("Base64Audio", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    if (row["fileName"] != DBNull.Value)
                    {
                        if (row["fileName"] != DBNull.Value)
                        {
                            string filePath = row["fileName"].ToString();
                            string audioUrl = ResolveUrl("~/" + filePath);
                            row["Base64Audio"] = audioUrl;
                        }
                    }
                }
                GVfollowup.DataSource = dt;
                GVfollowup.DataBind();
            }
        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    private void BindDdl_Username()
    {
        try
        {
            #region This Code is For Sale Module 

            #endregion

            #region This Code is For Admin & SubAdmin Module 
            string CurrentUserr = Session["UserName"].ToString();
            DataTable dtUsers = new DataTable();
            SqlDataAdapter sadusers = new SqlDataAdapter("select Username,UserCode from tbl_UserMaster where (Designation='Sales Manager' OR Designation='M.D') and Status=1 and IsDeleted=0", con);
            sadusers.Fill(dtUsers);
            ddlTeamuser.DataValueField = "UserCode";
            ddlTeamuser.DataTextField = "UserName";
            ddlTeamuser.DataSource = dtUsers;
            ddlTeamuser.DataBind();
            ddlTeamuser.Items.Insert(0, new ListItem("--Select--", "0"));
            #endregion       
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    protected void GVfollowup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //con.Open();
                //LinkButton lnkCloseUpdate = (LinkButton)e.Row.FindControl("lnkCloseUpdate");

                //int idd = Convert.ToInt32(GVfollowup.DataKeys[e.Row.RowIndex].Values[0]);
                //DataTable Dtt = new DataTable();
                //SqlDataAdapter Sdd = new SqlDataAdapter("Select * from [VW_FollowUpRpt] where ID_CommentHistory = '" + idd + "'", con);
                //Sdd.Fill(Dtt);
                //if (Dtt.Rows.Count > 0)
                //{
                //    string Status = Dtt.Rows[0]["UpdateStatus"].ToString();

                //    if (Status == "Follow-up")
                //    {
                //        lnkCloseUpdate.Visible = true;
                //    }
                //    else
                //    {

                //        lnkCloseUpdate.Visible = false;
                //    }
                //}
                //con.Close();
                //DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CallandMeetingDetails] AS Cm INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=Cm.CreatedBy WHERE Cm.CreatedBy='" + Session["UserCode"].ToString() + "'");
                //Dt.Columns.Add("Base64Audio", typeof(string));
                //foreach (DataRow row in Dt.Rows)
                //{
                //    if (row["fileName"] != DBNull.Value)
                //    {
                //        if (row["fileName"] != DBNull.Value)
                //        {
                //            string filePath = row["fileName"].ToString();
                //            string audioUrl = ResolveUrl("~/" + filePath);
                //            row["Base64Audio"] = audioUrl;
                //        }
                //    }
                //}
                //GVfollowup.DataSource = Dt;
                //GVfollowup.DataBind();
            }

            //Authorization
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //   // LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            //    LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

            //    string empcode = Session["UserCode"].ToString();
            //    DataTable Dt = new DataTable();
            //    SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
            //    Sd.Fill(Dt);
            //    if (Dt.Rows.Count > 0)
            //    {
            //        string id = Dt.Rows[0]["ID"].ToString();
            //        DataTable Dtt = new DataTable();
            //        SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'CallandMettingReport.aspx' AND PagesView = '1'", con);
            //        Sdd.Fill(Dtt);
            //        if (Dtt.Rows.Count > 0)
            //        {
            //            btnCreate.Visible = false;
            //            //GVQuotation.Columns[15].Visible = false;
            //            //btnEdit.Visible = false;
            //            btnDelete.Visible = false;
            //        }
            //    }
            //}
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        GridView();
    }
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("CallandMeetingUpdate.aspx");
    }
    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("CallandMettingReport.aspx");
    }
    protected void ddlfollowup_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {

            // Check the selected value of the first DropDownList
            string selectedValue = ddlfollowup.SelectedValue;

            if (ddlfollowup.SelectedValue == "- Select Type of Update -")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Select Type of Update..!!') ", true);
                ddlfollowupStatus.Enabled = false;
            }
            else if (ddlfollowup.SelectedValue == "Call")
            {
                ddlfollowupStatus.Items.Clear(); // Clear existing items
                ddlfollowupStatus.Items.Add(new ListItem("Follow-Up", "1"));
                ddlfollowupStatus.Items.Add(new ListItem("Not interested", "2"));
                ddlfollowupStatus.Items.Add(new ListItem("Ringing", "3"));
                ddlfollowupStatus.Items.Add(new ListItem("Busy", "4"));
                ddlfollowupStatus.Items.Add(new ListItem("Call-Closed", "5"));

                //ddlfollowupStatus.Enabled = true;
                //ListItem listItemToRemove = ddlfollowupStatus.Items.FindByValue("5"); // "5" is the value for "Closed"
                //if (listItemToRemove != null)
                //{
                //    ddlfollowupStatus.Items.Remove(listItemToRemove);
                //}

                //ListItem listItemToAdd = new ListItem("Ringing", "2");
                //ListItem listItemToAdd1 = new ListItem("Busy", "4");
                //if (ddlfollowupStatus.Items.FindByValue("2") == null)
                //{
                //    ddlfollowupStatus.Items.Add(listItemToAdd);
                //}
                //if (ddlfollowupStatus.Items.FindByValue("4") == null)
                //{
                //    ddlfollowupStatus.Items.Add(listItemToAdd1);
                //}
            }
            else if (ddlfollowup.SelectedValue == "Meeting")
            {
                ddlfollowupStatus.Items.Add(new ListItem("Follow-Up", "1"));
                ddlfollowupStatus.Items.Add(new ListItem("Not interested", "2"));
                ddlfollowupStatus.Items.Add(new ListItem("Closed", "3"));
                //ddlfollowupStatus.Enabled = true;
                //ListItem listItemToRemove = ddlfollowupStatus.Items.FindByValue("2"); // "5" is the value for "Closed"
                //ListItem listItemToRemove1 = ddlfollowupStatus.Items.FindByValue("4"); // "5" is the value for "Closed"
                //if (listItemToRemove != null)
                //{
                //    ddlfollowupStatus.Items.Remove(listItemToRemove);
                //    ddlfollowupStatus.Items.Remove(listItemToRemove1);
                //}

                //ListItem listItemToAdd = new ListItem("Closed", "5");
                //if (ddlfollowupStatus.Items.FindByValue("5") == null)
                //{
                //    ddlfollowupStatus.Items.Add(listItemToAdd);
                //}
            }
            GridView();
        }
        catch (Exception ex)
        {//throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    protected void ddlfollowupStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView();
    }
    protected void ddlTeamuser_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView();
    }
    protected void ddlmeetingwith_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView();
    }
    protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView();
    }
    protected void ddlCompanyname_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView();
    }

    protected void GVfollowup_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "CloseFollowup")
            {
                if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
                {
                    string ID = e.CommandArgument.ToString();


                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Detail (Close Followup)...!!');window.location='ImageMasterList.aspx'; ", true);

                    //lblCompanyDetail.Text = "Company Detail (Close Followup)";
                    Response.Redirect("CallandMeetingUpdate.aspx?Id=" + encrypt(e.CommandArgument.ToString()) + "");
                }
            }
        }
        catch (Exception ex)
        {

            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    public string encrypt(string encryptString)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }

    protected void GVfollowup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVfollowup.PageIndex = e.NewPageIndex;
        GridView();
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCompanyList(string prefixText, int count)
    {
        return AutoFillCompanyName(prefixText);
    }

    public static List<string> AutoFillCompanyName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Companyname] from [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and IsDeleted=0";

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

    protected void txtcompanyname_TextChanged(object sender, EventArgs e)
    {
        if (txtcompanyname.Text != null)
        {
            GridView();
        }
    }

}