
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_MarginReport : System.Web.UI.Page
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

    void GridView()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_Reports", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetMarginReportlist");       
            cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);        
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            if (ds.Tables[0].Columns.Count > 0)
            {
                GVfollowup.DataSource = ds.Tables[0];
                GVfollowup.DataBind();

                Gvcreditdebit.DataSource = ds.Tables[2];//added on 6/12/2024
                Gvcreditdebit.DataBind();

                GvTotalSummary.DataSource = ds.Tables[1];
                GvTotalSummary.DataBind();
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
    

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        GridView();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
    
    protected void btnDownload_Click(object sender, EventArgs e)
    {
        Report("EXCEL");
    }

    protected void btnPDF_Click(object sender, EventArgs e)
    {
        Report("PDF");
    }

    public void Report(string flag)
    {
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetMarginReportlist");
                    cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
                    cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(Dtt);

                        }
                    }
                }
            }

            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);                   
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);                   
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);                   
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\MarginReport.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render(flag, null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;

                    if (flag == "EXCEL")
                    {
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AddHeader("content-disposition", "attachment; filename=MarginReport.xls");
                    }
                    else
                    {
                        Response.ContentType = "application/vnd.pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=MarginReport.pdf");
                    }

                    Response.BinaryWrite(bytePdfRep);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.Reset();

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                }
            }
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    protected void GVfollowup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Int64 Quantity = 0;
            decimal SalePrice = 0;
            decimal PurchasePrice = 0;
            decimal Sales = 0;
            decimal Consumtion = 0;

            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GVfollowup.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    Quantity += Convert.ToInt64((row.FindControl("lblQty") as Label).Text);
                    SalePrice += Convert.ToDecimal((row.FindControl("lblSalePrice") as Label).Text);
                    PurchasePrice += Convert.ToDecimal((row.FindControl("lblPurchasePrice") as Label).Text);
                    Sales += Convert.ToDecimal((row.FindControl("lblSales") as Label).Text);
                    Consumtion += Convert.ToDecimal((row.FindControl("lblConsumtion") as Label).Text);
                }
            }
            
            decimal margin = Sales - Consumtion;
            decimal addition = margin / Sales;
            Int32 total = Convert.ToInt32(addition * 100);
       // Display the totals in the footer labels
       (e.Row.FindControl("lblTotalQuantity") as Label).Text = Quantity.ToString()+" Kg";
            (e.Row.FindControl("lblTotalSalePrice") as Label).Text = SalePrice.ToString();
            (e.Row.FindControl("lblTotalPurchasePrice") as Label).Text = PurchasePrice.ToString();
            (e.Row.FindControl("lblTotalSalesPrice") as Label).Text = Sales.ToString();
            (e.Row.FindControl("lblTotalConsumtion") as Label).Text = Consumtion.ToString();
            (e.Row.FindControl("lblTotalMargin") as Label).Text = total.ToString()+" %";
        }
    }

    //Search Customers methods
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
                com.CommandText = "select DISTINCT companyname from tbl_OutwardEntryHdr where  " + "companyname like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["companyname"].ToString());
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


    protected void GvTotalSummary_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Int64 Quantity = 0;
            decimal SalePrice = 0;
            decimal PurchasePrice = 0;       

            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GvTotalSummary.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    Quantity += Convert.ToInt64((row.FindControl("lblQty") as Label).Text);
                    SalePrice += Convert.ToDecimal((row.FindControl("lblSales") as Label).Text);
                    PurchasePrice += Convert.ToDecimal((row.FindControl("lblConsumption") as Label).Text);                
                }
            }

            decimal margin = SalePrice - PurchasePrice;
            decimal addition = margin / SalePrice;
            Int32 total = Convert.ToInt32(addition * 100);
            // Display the totals in the footer labels
            (e.Row.FindControl("lblTotalQuantity") as Label).Text = Quantity.ToString() + " Kg";
            (e.Row.FindControl("lblTotalSales") as Label).Text = SalePrice.ToString();
            (e.Row.FindControl("lblTotalConsumption") as Label).Text = PurchasePrice.ToString();      
            (e.Row.FindControl("lblTotalMargin") as Label).Text = total.ToString() + " %";
        }
    }
}