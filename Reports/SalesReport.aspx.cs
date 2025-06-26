
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

public partial class Reports_SalesReport : System.Web.UI.Page
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
            cmd.Parameters.AddWithValue("@Action", "GetSalesReportNew");
            cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            sda.Fill(dt);
            if (dt.Tables.Count > 0)
            {
                GVfollowup.DataSource = dt.Tables[0];
                GVfollowup.DataBind();

                GvTotalSummary.DataSource = dt.Tables[1];
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
                    cmd.Parameters.AddWithValue("@Action", "GetSalesReportNew");
                    cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedItem.Text);
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
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\SalesReport.rdlc";
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
                        Response.AddHeader("content-disposition", "attachment; filename=SalesReports.xls");
                    }
                    else
                    {
                        Response.ContentType = "application/vnd.pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=SalesReports.pdf");
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
            int TotalQuantity = 0;

            decimal totalBasic1 = 0;
            decimal totalCGST1 = 0;
            decimal totalSGST1 = 0;
            decimal totalIGST1 = 0;
            decimal grandTotal1 = 0;
            decimal TotalQuantity1 = 0;

            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GVfollowup.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    totalBasic += Convert.ToDecimal((row.FindControl("lblBasic") as Label).Text);               
                    totalCGST += Convert.ToDecimal((row.FindControl("lblCGSTAmt") as Label).Text);
                    totalSGST += Convert.ToDecimal((row.FindControl("lblSGSTAmt") as Label).Text);
                    totalIGST += Convert.ToDecimal((row.FindControl("lblIGSTAmt") as Label).Text);
                    grandTotal += Convert.ToDecimal((row.FindControl("lblTOTAL") as Label).Text);
                    TotalQuantity += Convert.ToInt32((row.FindControl("lblQuantity") as Label).Text);

                    totalBasic1 += Convert.ToDecimal((row.FindControl("lblOTTotal") as Label).Text);
                    totalCGST1 += Convert.ToDecimal((row.FindControl("lblOTCGST") as Label).Text);
                    totalSGST1 += Convert.ToDecimal((row.FindControl("lblOTSGST") as Label).Text);
                    totalIGST1 += Convert.ToDecimal((row.FindControl("lblOTIGST") as Label).Text);
                    grandTotal1 += Convert.ToDecimal((row.FindControl("lblOTALLTotal") as Label).Text);
                    TotalQuantity1 += Convert.ToDecimal((row.FindControl("lblTotalgrandTOTAL") as Label).Text);
                }
            }

       // Display the totals in the footer labels
       (e.Row.FindControl("lblTotalBasic") as Label).Text = totalBasic.ToString();
            (e.Row.FindControl("lblTotalCGST") as Label).Text = totalCGST.ToString();
            (e.Row.FindControl("lblTotalSGST") as Label).Text = totalSGST.ToString();
            (e.Row.FindControl("lblTotalIGST") as Label).Text = totalIGST.ToString();
            (e.Row.FindControl("lblGrandTotal") as Label).Text = grandTotal.ToString();
            (e.Row.FindControl("lblTotalQuantity") as Label).Text = TotalQuantity.ToString();

            (e.Row.FindControl("lblOTTotalBasic") as Label).Text = totalBasic1.ToString();
            (e.Row.FindControl("lblOTTotalCGST") as Label).Text = totalCGST1.ToString();
            (e.Row.FindControl("lblOTTotalSGST") as Label).Text = totalSGST1.ToString();
            (e.Row.FindControl("lblOTTotalIGST") as Label).Text = totalIGST1.ToString();
            (e.Row.FindControl("lblOTTotal") as Label).Text = grandTotal1.ToString();
            (e.Row.FindControl("lblallGrandTotal") as Label).Text = TotalQuantity1.ToString();
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
            decimal totalBasic = 0;
            decimal totalCGST = 0;
            decimal totalSGST = 0;
            decimal totalIGST = 0;
            decimal grandTotal = 0;
            int TotalQuantity = 0;

       
            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GvTotalSummary.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    totalBasic += Convert.ToDecimal((row.FindControl("lblBasicTotal") as Label).Text);
                    totalCGST += Convert.ToDecimal((row.FindControl("lblCGST") as Label).Text);
                    totalSGST += Convert.ToDecimal((row.FindControl("lblSGST") as Label).Text);
                    totalIGST += Convert.ToDecimal((row.FindControl("lblIGST") as Label).Text);
                    grandTotal += Convert.ToDecimal((row.FindControl("lblGrandTptals") as Label).Text);
                     TotalQuantity += Convert.ToInt32((row.FindControl("lblQty") as Label).Text);


                }
            }

    // Display the totals in the footer labels
    (e.Row.FindControl("totalBasicTotal") as Label).Text = totalBasic.ToString();
            (e.Row.FindControl("totalCGST") as Label).Text = totalCGST.ToString();
            (e.Row.FindControl("totalSGST") as Label).Text = totalSGST.ToString();
            (e.Row.FindControl("totalIGST") as Label).Text = totalIGST.ToString();
            (e.Row.FindControl("totalgrand") as Label).Text = grandTotal.ToString();
            (e.Row.FindControl("totalQty") as Label).Text = TotalQuantity.ToString();

           
        }
    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView();
    }
}