using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using ClosedXML.Excel;
using System.Globalization;
using Spire.Pdf;
using Spire.Pdf.Texts;
using iTextSharp.text.pdf.parser;
using iTextSharp.tool.xml.html.table;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Font = iTextSharp.text.Font;
using DocumentFormat.OpenXml;
using iTextSharp.tool.xml;
using Spire.Pdf.Conversion;
using Microsoft.Reporting.WebForms;

public partial class Repoerts_OutstandingReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                GetBinddata();
            }
        }
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("OutstandingReport.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCustomerList(string prefixText, int count)
    {

        return AutoFillCustomerlist(prefixText);
    }

    public static List<string> AutoFillCustomerlist(string prefixText)
    {

        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {

                com.CommandText = "select DISTINCT Companyname from tbl_CompanyMaster where " + "Companyname like @Search + '%' AND  IsDeleted=0 ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> cname = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        cname.Add(sdr["Companyname"].ToString());
                    }
                }
                con.Close();
                return cname;
            }

        }
    }
    int count = 0;
    protected void ddltype_TextChanged(object sender, EventArgs e)
    {
        if (ddltype.Text == "SALE")
        {
            AutoCompleteExtender1.Enabled = true;
            txtPartyName.Text = string.Empty;
            GetCustomerList(txtPartyName.Text, count);
            // AutoCompleteExtender2.Enabled = false;

        }
        else if (ddltype.Text == "PURCHASE")
        {
            // AutoCompleteExtender2.Enabled = true;
            txtPartyName.Text = string.Empty;
            GetSupplierList(txtPartyName.Text, count);
            AutoCompleteExtender1.Enabled = false;

        }
    }
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierList(string prefixText, int count)
    {

        return AutoFillSupplierlist(prefixText);
    }

    public static List<string> AutoFillSupplierlist(string prefixText)
    {

        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {

                com.CommandText = "select DISTINCT VendorName from tbl_VendorMaster where " + "VendorName like @Search + '%' AND IsDeleted=0 ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> SupplierName = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        SupplierName.Add(sdr["VendorName"].ToString());
                    }
                }
                con.Close();
                return SupplierName;
            }

        }
    }

    protected void btnpdf_Click(object sender, EventArgs e)
    {

        GetOutstandingReports("PDF");
    }

    protected void ExportExcel(object sender, EventArgs e)
    {

        GetOutstandingReports("Excel");
    }

    public void GetOutstandingReports(string flg)
    {
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlConnection con = new SqlConnection(strConnString))
        {
            //string storeProcedure = "SP_OutstandingRForRDLC";
            string storeProcedure = "SP_OutstandingRForRDLReports";

            con.Open();

            using (SqlCommand cmd = new SqlCommand(storeProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", ddltype.SelectedItem.Text);
                cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                if (txtfromdate.Text != "")
                {
                    //cmd.Parameters.AddWithValue("@FromDate", DateTime.Parse(txtfromdate.Text));
                    cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
                }
                if (txttodate.Text != "")
                {
                    //cmd.Parameters.AddWithValue("@ToDate", DateTime.Parse(txttodate.Text));
                    cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
                }
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        ReportDataSource obj1 = new ReportDataSource("DataSet1", ds.Tables[0]);
                        //ReportDataSource obj2 = new ReportDataSource("DataSet1", ds.Tables[1]);
                        // ReportDataSource obj3 = new ReportDataSource("DataSet1", ds.Tables[2]);

                        ReportViewer1.LocalReport.DataSources.Add(obj1);
                        // ReportViewer1.LocalReport.DataSources.Add(obj2);
                        // ReportViewer1.LocalReport.DataSources.Add(obj3);

                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\Outstandingrdlc.rdlc";
                        ReportViewer1.LocalReport.Refresh();

                        //-------- Print PDF directly without showing ReportViewer ----
                        Warning[] warnings;
                        string[] streamids;
                        string mimeType;
                        string encoding;
                        string extension;

                        byte[] bytePdfRep = ReportViewer1.LocalReport.Render(flg, null, out mimeType, out encoding, out extension, out streamids, out warnings);

                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;

                        if (flg == "Excel")
                        {
                            Response.ContentType = "application/vnd.xls";
                            Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + ".xls"); //Give file name here

                            Response.BinaryWrite(bytePdfRep);
                            ReportViewer1.LocalReport.DataSources.Clear();

                        }
                        else
                        {

                            Response.ContentType = "application/vnd.pdf";
                            Response.AddHeader("content-disposition", "attachment; filename=OutStandingReports.pdf");


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
        }
    }

    public void GetBinddata()
    {
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        using (SqlConnection con = new SqlConnection(strConnString))
        {
            //string storeProcedure = "SP_OutstandingRForRDLC";
            string storeProcedure = "SP_OutstandingRForRDLReports";

            con.Open();

            using (SqlCommand cmd = new SqlCommand(storeProcedure, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", ddltype.SelectedItem.Text);
                cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                if (txtfromdate.Text != "")
                {
                    //cmd.Parameters.AddWithValue("@FromDate", DateTime.Parse(txtfromdate.Text));
                    cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@FromDate", DBNull.Value);
                }
                if (txttodate.Text != "")
                {
                    //cmd.Parameters.AddWithValue("@ToDate", DateTime.Parse(txttodate.Text));
                    cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ToDate", DBNull.Value);
                }
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        dgvOutstanding.DataSource = ds.Tables[0];
                        dgvOutstanding.DataBind();


                    }
                    else
                    {

                        dgvOutstanding.DataSource = null;
                        dgvOutstanding.DataBind();


                    }
                }
            }
        }
    }

    protected void txtPartyName_TextChanged(object sender, EventArgs e)
    {
        GetBinddata();
    }

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        GetBinddata();
    }


    decimal cumulativeBalance = 0;
    protected void dgvOutstanding_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblBalance = (Label)e.Row.FindControl("lblbalance");
            if (lblBalance != null)
            {
                decimal balance = 0;
                if (decimal.TryParse(lblBalance.Text, out balance))
                {
                    // Add the balance to the cumulative total
                    cumulativeBalance += balance;

                    // Find the label that should display the cumulative balance
                    Label lblCumulativeBalance = (Label)e.Row.FindControl("lblCum_Balance");
                    if (lblCumulativeBalance != null)
                    {
                        // Set the cumulative balance in the current row
                        lblCumulativeBalance.Text = cumulativeBalance.ToString("N2");
                    }
                }
            }
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
         
            
            decimal totalPayable = 0;
            decimal totalReceived = 0;
            decimal totalBalance = 0;
            decimal totalCumBalance = 0;

            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in dgvOutstanding.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    totalPayable += Convert.ToDecimal((row.FindControl("lblPayable") as Label).Text);
                    totalReceived += Convert.ToDecimal((row.FindControl("lblReceived") as Label).Text);
                    totalBalance += Convert.ToDecimal((row.FindControl("lblbalance") as Label).Text);
                    totalCumBalance += Convert.ToDecimal((row.FindControl("lblCum_Balance") as Label).Text);

                }
            }

        // Display the totals in the footer labels
        (e.Row.FindControl("lblSumPayable") as Label).Text = totalPayable.ToString();
            (e.Row.FindControl("lblsumReceived") as Label).Text = totalReceived.ToString();
            (e.Row.FindControl("lblsumBalance") as Label).Text = totalBalance.ToString();
            (e.Row.FindControl("lblsumCum_Balance") as Label).Text = totalCumBalance.ToString();

        }
    }
}
































