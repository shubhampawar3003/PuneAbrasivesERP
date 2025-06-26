using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_SalesTargetList : System.Web.UI.Page
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
              
                if (Session["Designation"].ToString() == "Sales Manager")
                {
                    txtusername.Enabled = false;
                    txtusername.Text = Session["Username"].ToString();
                
                }
                GetYears();
                FillGrid();
            }
        }
    }

    //Fill GridView
    private void FillGrid()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_SalesTargetMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetSalesTargetList");
            cmd.Parameters.AddWithValue("@YEAR", ddlYear.SelectedValue);
            cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@User", txtusername.Text);
            cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
            cmd.Parameters.AddWithValue("@companyname", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@component", txtcomponent.Text);
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            sda.Fill(dt);
            if (dt.Tables.Count > 0)
            {
                GVSalesTarget.DataSource = dt.Tables[0];
                GVSalesTarget.DataBind();


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
    public void GetYears()
    {
        DataTable Dt = Cls_Main.Read_Table("Select DISTINCT Year from tbl_SalesTargetMaster where IsDeleted=0");
        if (Dt.Rows.Count > 0)
        {
            ddlYear.DataSource = Dt;
            ddlYear.DataTextField = "Year";
            ddlYear.DataBind();
            ddlYear.Items.Insert(0, new ListItem("-- Select Year --", "0"));
        }

    }
    //Search customer method
    [ScriptMethod()]
    [WebMethod]
    public static List<string> GetCustomerList(string prefixText, int count)
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
                com.CommandText = "Select DISTINCT [Customername] from [tbl_SalesTargetMaster] where " + "Customername like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Customername"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    //Search Sales person method
    [ScriptMethod()]
    [WebMethod]
    public static List<string> GetUserNameList(string prefixText, int count)
    {
        return AutoFillUserName(prefixText);
    }

    public static List<string> AutoFillUserName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [SalesPerson] from [tbl_SalesTargetMaster] where " + "SalesPerson like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["SalesPerson"].ToString());
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
                com.CommandText = "SELECT DISTINCT [Component] FROM [tbl_SalesTargetMaster] where " + "Component like '%'+ @Search + '%' and IsDeleted=0 ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Component"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtcomponent_TextChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    //Search Grade method
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
                com.CommandText = "Select DISTINCT [Grade] from [tbl_SalesTargetMaster] where " + "Grade like @Search + '%' and IsDeleted=0";

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
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("SalesTargetMaster.aspx");
    }

    protected void GVSalesTarget_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("SalesTargetMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_SalesTargetMaster] SET IsDeleted=@IsDeleted WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", 1);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Sales Target Deleted Successfully..!!')", true);
            FillGrid();
        }
    }

    protected void GVSalesTarget_RowDataBound(object sender, GridViewRowEventArgs e)
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'SalesTargetList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate1.Visible = false;
                    GVSalesTarget.Columns[12].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("SalesTargetList.aspx");
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void txtusername_TextChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void txtGrade_TextChanged(object sender, EventArgs e)
    {
        FillGrid();

    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        //required to avoid the run time error "
        //Control 'GridView1' of type 'Grid View' must be placed inside a form tag with runat=server."
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_SalesTargetMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetSalesTargetList");
            cmd.Parameters.AddWithValue("@YEAR", ddlYear.SelectedValue);
            cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@User", txtusername.Text);
            cmd.Parameters.AddWithValue("@Grade", txtGrade.Text);
            cmd.Parameters.AddWithValue("@companyname", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Columns.Count > 0)
            {

                Response.Clear();
                Response.Buffer = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Charset = "";
                string FileName = "SalesTarget_List_" + DateTime.Now + ".xls";
                StringWriter strwritter = new StringWriter();
                HtmlTextWriter htmltextwrtter = new HtmlTextWriter(strwritter);
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", "attachment;filename=" + FileName);

                GVSalesTarget.DataSource = dt;
                GVSalesTarget.DataBind();
                GVSalesTarget.GridLines = GridLines.Both;
                GVSalesTarget.HeaderStyle.Font.Bold = true;
                GVSalesTarget.RenderControl(htmltextwrtter);


                Response.Write(strwritter.ToString());
                Response.End();

            }
        }
        catch (Exception ex) { }
    }

}