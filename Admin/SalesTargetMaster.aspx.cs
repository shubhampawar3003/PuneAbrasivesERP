using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_SalesTargetMaster : System.Web.UI.Page
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
                GetSalesPerson();
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
        SqlDataAdapter ad = new SqlDataAdapter("SELECT  ISNULL(MAX(CAST(SUBSTRING(TargetCode, CHARINDEX('-', TargetCode) + 1, LEN(TargetCode)) AS INT)), 0) + 1 AS maxno FROM [tbl_SalesTargetMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxno"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxno"].ToString());
            txtTargetcode.Text = "PAPL/ST-" + maxid.ToString();
        }
        else
        {
            txtTargetcode.Text = string.Empty;
        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("select * from tbl_SalesTargetMaster WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            ddlYear.SelectedItem.Text = Dt.Rows[0]["Year"].ToString();
            ddlMonth.SelectedValue = Dt.Rows[0]["Month"].ToString();
            ddlSalesperson.SelectedItem.Text = Dt.Rows[0]["SalesPerson"].ToString();
            txtRate.Text = Dt.Rows[0]["Rate"].ToString();
            txtcompanyname.Text = Dt.Rows[0]["Customername"].ToString();
            txtcomponent.Text = Dt.Rows[0]["Component"].ToString();
            txtGrade.Text = Dt.Rows[0]["Grade"].ToString();
            txtQuantity.Text = Dt.Rows[0]["Quantity"].ToString();
            txtamount.Text = Dt.Rows[0]["Amount"].ToString();
            hfCalculatedAmount.Value = Dt.Rows[0]["Amount"].ToString();
            txtTargetcode.Text = Dt.Rows[0]["TargetCode"].ToString();


        }
    }
    private void GetSalesPerson()
    {
        DataTable Dt = Cls_Main.Read_Table("Select Username,usercode from tbl_UserMaster where IsDeleted=0");
        if (Dt.Rows.Count > 0)
        {
            ddlSalesperson.DataSource = Dt;
            ddlSalesperson.DataTextField = "Username";
            ddlSalesperson.DataValueField = "usercode";
            ddlSalesperson.DataBind();
            ddlSalesperson.Items.Insert(0, new ListItem("--Select--", "0"));
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


            if (txtRate.Text == "" || txtQuantity.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {

                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_SalesTargetMaster", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@TargetCode", hhd.Value);
                    Cmd.Parameters.AddWithValue("@Amount", hfCalculatedAmount.Value);
                    Cmd.Parameters.AddWithValue("@companyname", txtcompanyname.Text.Trim());
                    Cmd.Parameters.AddWithValue("@component", txtcomponent.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Grade", txtGrade.Text.Trim());
                    Cmd.Parameters.AddWithValue("@User", ddlSalesperson.SelectedItem.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text.Trim());
                    Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Sales Target Update Successfully..!!');window.location='SalesTargetList.aspx'; ", true);
                }
                else
                {
                    DataTable Dt = Cls_Main.Read_Table("Select * from tbl_SalesTargetMaster where IsDeleted=0 AND Year='" + ddlYear.SelectedItem.Text.Trim() + "' AND Month='" + ddlMonth.SelectedItem.Text.Trim() + "' AND CustomerName='" + txtcompanyname.Text + "' AND Grade='" + txtGrade.Text + "' AND salesperson='" + ddlSalesperson.SelectedItem.Text + "'");
                    if (Dt.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Already registerd please check and edit..!!')", true);
                        txtcompanyname.Text = null;
                        txtGrade.Text = null;
                        txtRate.Text = null;
                        txtQuantity.Text = null;

                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_SalesTargetMaster", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@companyname", txtcompanyname.Text.Trim());
                        Cmd.Parameters.AddWithValue("@component", txtcomponent.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedItem.Text.Trim());
                        Cmd.Parameters.AddWithValue("@TargetCode", txtTargetcode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedValue);
                        Cmd.Parameters.AddWithValue("@Amount", hfCalculatedAmount.Value);
                        Cmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
                        Cmd.Parameters.AddWithValue("@User", ddlSalesperson.SelectedItem.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Grade", txtGrade.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Sales Target Added Successfully..!!');window.location='SalesTargetList.aspx'; ", true);

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
        Response.Redirect("SalesTargetList.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("SalesTargetList.aspx");
    }

    [ScriptMethod()]
    [WebMethod]
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

    [ScriptMethod()]
    [WebMethod]
    public static List<string> GetGradeList(string prefixText, int count)
    {
        return AutoFillGradeName(prefixText);
    }

    public static List<string> AutoFillGradeName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Grade] from [tbl_ComponentMaster] where " + "Grade like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Grade"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }


    //Search Components  methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetComponentList(string prefixText, int count)
    {
        return AutoFillComponentName(prefixText);
    }

    public static List<string> AutoFillComponentName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [ComponentName] FROM [tbl_ComponentMaster] where " + "ComponentName like '%'+ @Search + '%' and IsDeleted=0 AND Status = '1'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["ComponentName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }


    protected void txtcomponent_TextChanged(object sender, EventArgs e)
    {
        if (txtcomponent.Text != "" || txtcomponent.Text != null)
        {
            string Compo = txtcomponent.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ComponentMaster] where ComponentName = '" + Compo + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtGrade.Text = dt.Rows[0]["Grade"].ToString();
                txtGrade.Enabled = true;
            }
        }
    }
}