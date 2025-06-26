
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

public partial class Reports_PerformanceReport : System.Web.UI.Page
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
                //GetTotalSumary();
            }
        }
    }

    void GridView()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_Reports", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetPerformanceReport");
            cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@UserName", txtusername.Text);
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            if (ds.Tables[0].Columns.Count > 0)
            {
                GVfollowup.DataSource = ds.Tables[0];
                GVfollowup.DataBind();

                rptsales.DataSource = ds.Tables[1];
                rptsales.DataBind();
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
                    cmd.Parameters.AddWithValue("@Action", "GetPerformanceReport");
                    cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@UserName", txtusername.Text);
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
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PerformanceReport.rdlc";
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
                        Response.AddHeader("content-disposition", "attachment; filename=PerformanceReport.xls");
                    }
                    else
                    {
                        Response.ContentType = "application/vnd.pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=PerformanceReport.pdf");
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
        catch (Exception ex)
        {
            throw (ex);
        }
    }

    protected void GVfollowup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            decimal totalBasic = 0;
            decimal totalCGST = 0;
            decimal totalSGST = 0;
            decimal totalIGST = 0;
            decimal grandTotal = 0;
            decimal TotalQuantity = 0;



            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GVfollowup.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    totalBasic += Convert.ToDecimal((row.FindControl("lblOSAmount") as Label).Text);
                    totalCGST += Convert.ToDecimal((row.FindControl("lblMorethenonetwenty") as Label).Text);
                    totalSGST += Convert.ToDecimal((row.FindControl("lblNigntytoonetwenty") as Label).Text);
                    totalIGST += Convert.ToDecimal((row.FindControl("lblSixtytonignty") as Label).Text);
                    grandTotal += Convert.ToDecimal((row.FindControl("lblthirtytoSexty") as Label).Text);
                    TotalQuantity += Convert.ToDecimal((row.FindControl("lblLessThenthirty") as Label).Text);
                }
            }

       // Display the totals in the footer labels
       (e.Row.FindControl("lblTotalOSAmount") as Label).Text = totalBasic.ToString();
            (e.Row.FindControl("lblTotalMorethenonetwenty") as Label).Text = totalCGST.ToString();
            (e.Row.FindControl("lblTotalNigntytoonetwenty") as Label).Text = totalSGST.ToString();
            (e.Row.FindControl("lblTotalSixtytonignty") as Label).Text = totalIGST.ToString();
            (e.Row.FindControl("lblTotalthirtytoSexty") as Label).Text = grandTotal.ToString();
            (e.Row.FindControl("lblTotalLessThenthirty") as Label).Text = TotalQuantity.ToString();
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
                com.CommandText = "select DISTINCT BillingCustomer from tblTaxInvoiceHdr where  " + "BillingCustomer like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["BillingCustomer"].ToString());
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

    //Search Sales person methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetUserNameList(string prefixText, int count)
    {
        return AutoFillSalesName(prefixText);
    }

    public static List<string> AutoFillSalesName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT UM.UserName FROM tblTaxInvoiceHdr AS TH LEFT JOIN tbl_CustomerPurchaseOrderHdr AS CPO ON CPO.Pono = TH.AgainstNumber LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode = CPO.UserName WHERE  e_invoice_cancel_status IS NULL AND AckNo IS NOT NULL AND  " + "UM.UserName like '%'+ @Search + '%' GROUP BY UM.UserName";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["UserName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtusername_TextChanged(object sender, EventArgs e)
    {
        GridView();
    }
}