
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


public partial class Admin_DSRReports : System.Web.UI.Page
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

                GridView();

            }
        }
    }
    //Fill GridView
    void GridView()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("[SP_Reports]", con);
            cmd.CommandType = CommandType.StoredProcedure;        
            if (txtCustomerName.Text == null || txtCustomerName.Text == "")
            {
                cmd.Parameters.AddWithValue("@CompanyName", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
            }
            if (ddlfollowupStatus.SelectedItem.Text == "- Select Status of Update -" || ddlfollowupStatus.SelectedItem.Text == null)
            {
                cmd.Parameters.AddWithValue("@UpdateStatus", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@UpdateStatus", ddlfollowupStatus.SelectedItem.Text);
            }
            if(txtfromdate.Text!="")
            {
                cmd.Parameters.AddWithValue("@FromDate", Convert.ToDateTime(txtfromdate.Text));
            }
            else
            {
                cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            }
            if (txttodate.Text != "")
            {
                cmd.Parameters.AddWithValue("@ToDate", Convert.ToDateTime(txttodate.Text));
            }
            else
            {
                cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            }
          
            cmd.Parameters.AddWithValue("@Action", "GetCallMettingDetails");

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

    //Search Company Search methods
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
                com.CommandText = "select distinct CompanyName from tbl_CallandMeetingDetails where " + "CompanyName like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["CompanyName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        GridView();
    }


    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        GridView();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("DSRReports.aspx");
    }

    protected void ddlfollowupStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlfollowupStatus.SelectedValue != "0")
        {
            GridView();
        }
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
                    Cls_Main.Conn_Open();
                    SqlCommand CmdDelete = new SqlCommand("Update tbl_CallandMeetingDetails Set Status=@Status,UpdatedBy=@UpdatedBy,UpdatedOn=@UpdatedOn where ID=@ID", Cls_Main.Conn);
                    CmdDelete.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
                    CmdDelete.Parameters.AddWithValue("@Status", 2);
                    CmdDelete.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                    CmdDelete.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);

                    CmdDelete.ExecuteNonQuery();
                    Cls_Main.Conn_Close();

                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Detail (Close Followup)...!!');window.location='ImageMasterList.aspx'; ", true);

                    //lblCompanyDetail.Text = "Company Detail (Close Followup)";
                    Response.Redirect("CallandMeetingUpdate.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
                }
            }
            if (e.CommandName == "SaveComment")
            {            
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVfollowup.Rows[rowIndex];

                TextBox txtComment = (TextBox)row.FindControl("txtComment");
                Label lblID = (Label)row.FindControl("lblID");

                //Edit Comment Box
                Cls_Main.Conn_Open();
                SqlCommand CmdEdit = new SqlCommand("Update tbl_CallandMeetingDetails Set Comment=@Comment where ID=@ID", Cls_Main.Conn);
                CmdEdit.Parameters.AddWithValue("@ID", Convert.ToInt32(lblID.Text));
                CmdEdit.Parameters.AddWithValue("@Comment", txtComment.Text.Trim());
                CmdEdit.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                txtComment.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Comment Save Successfully...!!');window.location='DSRReports.aspx';", true);

            }
        }
        catch (Exception ex)
        {

            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }


    protected void GVfollowup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVfollowup.PageIndex = e.NewPageIndex;
        GridView();
    }


}