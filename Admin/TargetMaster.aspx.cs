using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_TargetMaster : System.Web.UI.Page
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
                GetYears();
                GetMonths();
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
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([ID]) as maxid FROM [TargetMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            txtTargetcode.Text = "PAPL/T-" + (maxid + 1).ToString();
        }
        else
        {
            txtTargetcode.Text = string.Empty;
        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("select * from TargetMaster WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            ddlYear.SelectedItem.Text = Dt.Rows[0]["Year"].ToString();
            ddlMonth.SelectedItem.Text = Dt.Rows[0]["Month"].ToString();
            txtTarget.Text = Dt.Rows[0]["Amount"].ToString();
            txtkgston.Text = Dt.Rows[0]["Quantity"].ToString();
            txtTargetcode.Text = Dt.Rows[0]["TargetCode"].ToString();


        }
    }

    public void GetYears()
    {
        DateTime date = DateTime.Now;
        List<string> financialYears = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            string finYear;
            if (date.Month > 3) // If current month is after March
            {
                finYear = (date.Year + 1 + i).ToString(); // Next financial year starts next year
            }
            else // If current month is before or in March
            {
                finYear = (date.Year + i).ToString(); // Current financial year ends this year
            }

            string previousYear = (Convert.ToInt32(finYear) - 1).ToString();
            financialYears.Add(previousYear + "-" + finYear);
        }

        // Bind financial years to the dropdown list
        ddlYear.DataSource = financialYears;
        ddlYear.DataBind();
    }
    private void GetMonths()
    {
       

        ddlMonth.Items.Clear();
        string selectedFinancialYear = ddlYear.SelectedValue;
        int currentMonth = DateTime.Now.Month;
        int currenMonth = currentMonth == 12 ? 1 : currentMonth + 1;

        for (int month = 1; month <= 12; month++)
        {
            ListItem item = new ListItem(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month), month.ToString());

            if (month < currenMonth)
            {
                item.Attributes.Add("disabled", "disabled");
            }

            ddlMonth.Items.Add(item);
        }

        ddlMonth.SelectedIndex = -1;
        ddlMonth.SelectedValue = Convert.ToString(currentMonth);

    }
    //Save and Update Record
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtTarget.Text == "" || txtkgston.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {

                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_TargetMaster", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@TargetCode", hhd.Value);
                    Cmd.Parameters.AddWithValue("@Target", txtTarget.Text.Trim());
                    Cmd.Parameters.AddWithValue("@kgston", txtkgston.Text.Trim());
                    Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Target Update Successfully..!!');window.location='TargetList.aspx'; ", true);
                }
                else
                {

                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_TargetMaster", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Save");
                    Cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedItem.Text.Trim());
                    Cmd.Parameters.AddWithValue("@TargetCode", txtTargetcode.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedItem.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Target", txtTarget.Text.Trim());
                    Cmd.Parameters.AddWithValue("@kgston", txtkgston.Text.Trim());
                    Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Target Added Successfully..!!');window.location='TargetList.aspx'; ", true);

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
        Response.Redirect("TargetList.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("TargetList.aspx");
    }
}