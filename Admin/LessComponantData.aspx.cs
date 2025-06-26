using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_LessComponantData : System.Web.UI.Page
{
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
        try
        {
            SqlDataAdapter Da = new SqlDataAdapter("SELECT BC.ComponentName, SUM(BC.BalanceQty) AS BalanceQty, CM.lessqtylimit FROM vw_batchwisecomponent AS BC JOIN tbl_ComponentMaster AS CM ON BC.ComponentName = CM.ComponentName GROUP BY BC.ComponentName, CM.lessqtylimit HAVING SUM(BC.BalanceQty) <= CM.lessqtylimit", Cls_Main.Conn);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                GVInentory.DataSource = Dt;
                GVInentory.DataBind();
            }
        }
        catch (Exception ex)
        {
            //throw;
            //string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Not Found....!');", true);

        }

    }
    
    protected void GVInentory_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVInentory.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("InventoryList.aspx");
    }

    //Search Customers methods
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
                com.CommandText = "select Distinct ComponentName from vw_batchwisecomponent where  " + "ComponentName like @Search + '%' ";

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


    protected void txtproduct_TextChanged(object sender, EventArgs e)
    {
        try
        {

            if (txtproduct.Text != "")
            {
                string Product = txtproduct.Text;
                SqlDataAdapter Da = new SqlDataAdapter("SELECT BC.* FROM vw_batchwisecomponent AS BC JOIN tbl_ComponentMaster AS CM ON BC.ComponentName = CM.ComponentName  WHERE TRY_CAST(BC.BalanceQty AS DECIMAL(18,2)) <= TRY_CAST(CM.lessqtylimit AS DECIMAL(18,2))  AND ComponentName='" + Product + "'  ORDER BY BC.ID DESC", Cls_Main.Conn);
                DataTable Dt = new DataTable();
                Da.Fill(Dt);
                if (Dt.Rows.Count > 0)
                {
                    GVInentory.DataSource = Dt;
                    GVInentory.DataBind();
                }

            }


        }
        catch (Exception ex)
        {
            //throw ex;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Not Found....!');", true);
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("InventoryList.aspx");
    }
}