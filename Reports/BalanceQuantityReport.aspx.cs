using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_BalanceQuantityReport : System.Web.UI.Page
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
                FillGrid();
            }
        }
    }

    //Fill GridView
    private void FillGrid()
    {
        DataTable Dt = Cls_Main.Read_Table("select POD.ID,CustomerName,POH.Pono,Balance,Productname from tbl_CustomerPurchaseOrderHdr AS POH INNER JOIN tbl_CustomerPurchaseOrderDtls AS POD ON POD.Pono=POH.Pono where POH.IsDeleted=0 ORDER BY POD.ID DESC");
        GVBalance.DataSource = Dt;
        GVBalance.DataBind();
    }
    
    protected void GVBalance_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVBalance.PageIndex = e.NewPageIndex;
        FillGrid();
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
                com.CommandText = "SELECT DISTINCT [ID],[Companyname] FROM [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and IsDeleted=0";

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

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        if (txtCustomerName.Text != "" || txtCustomerName.Text != null)
        {
            string company = txtCustomerName.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select POD.ID,CustomerName,POH.Pono,Balance,Productname from tbl_CustomerPurchaseOrderHdr AS POH INNER JOIN tbl_CustomerPurchaseOrderDtls AS POD ON POD.Pono=POH.Pono where POH.IsDeleted=0 AND  CustomerName = '" + company + "' ORDER BY POD.ID DESC", Cls_Main.Conn);
            sad.Fill(dt);
            GVBalance.EmptyDataText = "Not Records Found";
            GVBalance.DataSource = dt;
            GVBalance.DataBind();
        }
    }

    //Search Company Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetProductList(string prefixText, int count)
    {
        return AutoFillProductName(prefixText);
    }

    public static List<string> AutoFillProductName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [Productname] FROM [tbl_CustomerPurchaseOrderDtls] where " + "Productname like @Search + '%' ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Productname"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtProductname_TextChanged(object sender, EventArgs e)
    {
        if (txtProductname.Text != "" || txtProductname.Text != null)
        {
            string company = txtProductname.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select POD.ID,CustomerName,POH.Pono,Balance,Productname from tbl_CustomerPurchaseOrderHdr AS POH INNER JOIN tbl_CustomerPurchaseOrderDtls AS POD ON POD.Pono=POH.Pono where POH.IsDeleted=0 AND  Productname = '" + company + "' ORDER BY POD.ID DESC", Cls_Main.Conn);
            sad.Fill(dt);
            GVBalance.EmptyDataText = "Not Records Found";
            GVBalance.DataSource = dt;
            GVBalance.DataBind();
        }
    }

    //Search Company Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetPOCOdeList(string prefixText, int count)
    {
        return AutoFillPOCOdeName(prefixText);
    }

    public static List<string> AutoFillPOCOdeName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [Pono] FROM [tbl_CustomerPurchaseOrderDtls] where " + "Pono like @Search + '%' ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Pono"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void txtPOCode_TextChanged(object sender, EventArgs e)
    {
        if (txtPOCode.Text != "" || txtPOCode.Text != null)
        {
            string company = txtPOCode.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select POD.ID,CustomerName,POH.Pono,Balance,Productname from tbl_CustomerPurchaseOrderHdr AS POH INNER JOIN tbl_CustomerPurchaseOrderDtls AS POD ON POD.Pono=POH.Pono where POH.IsDeleted=0 AND POH.Pono = '" + company + "' ORDER BY POD.ID DESC", Cls_Main.Conn);
            sad.Fill(dt);
            GVBalance.EmptyDataText = "Not Records Found";
            GVBalance.DataSource = dt;
            GVBalance.DataBind();
        }
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("BalanceQuantityReport.aspx");
    }
}