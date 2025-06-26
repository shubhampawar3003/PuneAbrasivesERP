using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Reporting.WebForms;

public partial class Reports_PartyLedgerReport : System.Web.UI.Page
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
                
                btn.Visible = true;
                GetData();
            }
        }
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("PartyLedgerReport.aspx");
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

                com.CommandText = "select DISTINCT Companyname from tbl_CompanyMaster where " + "Companyname like @Search + '%' AND IsDeleted=0 ";

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
    //int count = 0;
    //protected void ddltype_TextChanged(object sender, EventArgs e)
    //{
    //    GetData();
    //    if (ddltype.Text == "SALE")
    //    {
    //        AutoCompleteExtender1.Enabled = true;
    //        txtPartyName.Text = string.Empty;
    //        GetCustomerList(txtPartyName.Text, count);
    //        AutoCompleteExtender2.Enabled = false;

    //    }

    //    else if (ddltype.Text == "PURCHASE")
    //    {
    //        AutoCompleteExtender2.Enabled = true;
    //        txtPartyName.Text = string.Empty;
    //        GetSupplierList(txtPartyName.Text, count);
    //        AutoCompleteExtender1.Enabled = false;

    //    }
    //}

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
      
        Report("PDF");
    }

   

    protected void ExportExcel(object sender, EventArgs e)
    {
        Report("Excel");
   
    }

    protected void Report(string flg)
    {


        DataSet Dtt = new DataSet();
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand("[SP_PartyLedgerRDLC]", con))
            {
                
                string fdate;
                string tdate;
                string ft = txtfromdate.Text;
                string tt = txttodate.Text;
                if (ft == "")
                {
                    fdate = "";
                }
                else
                {
                    DateTime ftdate = Convert.ToDateTime(ft, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    fdate = ftdate.ToString("yyyy-MM-dd");

                    //var fttime = Convert.ToDateTime(ft);
                    //fdate = fttime.ToString("yyyy-MM-dd");
                }

                if (tt == "")
                {
                    tdate = "";
                }
                else
                {

                    DateTime date = Convert.ToDateTime(tt, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    tdate = date.ToString("yyyy-MM-dd");
                    //var tttime = Convert.ToDateTime(tt);
                    //tdate = tttime.ToString("yyyy-MM-dd");
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", "SALE");
                cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                if (fdate != null && fdate != "")
                {
                    cmd.Parameters.AddWithValue("@FromDate", fdate);
                }
                if (tdate != null && tdate != "")
                {
                    cmd.Parameters.AddWithValue("@ToDate", tdate);
                }
              
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
                ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PartyLedger.rdlc";
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
                    Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + "_PartyLadger.xls"); //Give file name here

                    Response.BinaryWrite(bytePdfRep);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    
                }
                else
                {
                    //string filePath = Server.MapPath("~") + "/PDF_Files/PartyLedgerReport.pdf";
                    //System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                    //ifrRight6.Attributes["src"] = @"../PDF_Files/" + "PartyLedgerReport.pdf";
                    Response.ContentType = "application/vnd.pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + "_PartyLadger.pdf"); //Give file name here

                    Response.BinaryWrite(bytePdfRep);
                    ReportViewer1.LocalReport.DataSources.Clear();

                }
                this.ReportViewer1.Reset();


            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
            }
        }
    }

    public void GetData()
    {
        DataSet Dtt = new DataSet();
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand("[SP_PartyLedgerRDLC]", con))
            {

                string fdate;
                string tdate;
                string ft = txtfromdate.Text;
                string tt = txttodate.Text;
                if (ft == "")
                {
                    fdate = "";
                }
                else
                {
                    DateTime ftdate = Convert.ToDateTime(ft, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    fdate = ftdate.ToString("yyyy-MM-dd");

                    //var fttime = Convert.ToDateTime(ft);
                    //fdate = fttime.ToString("yyyy-MM-dd");
                }

                if (tt == "")
                {
                    tdate = "";
                }
                else
                {

                    DateTime date = Convert.ToDateTime(tt, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    tdate = date.ToString("yyyy-MM-dd");
                    //var tttime = Convert.ToDateTime(tt);
                    //tdate = tttime.ToString("yyyy-MM-dd");
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", "SALE");
                cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                if (fdate != null && fdate != "")
                {
                    cmd.Parameters.AddWithValue("@FromDate", fdate);
                }
                if (tdate != null && tdate != "")
                {
                    cmd.Parameters.AddWithValue("@ToDate", tdate);
                }

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
        if(Dtt.Tables.Count>0)
        {
            if(Dtt.Tables[0].Rows.Count>0)
            {
                GVfollowup.DataSource = Dtt;
                GVfollowup.DataBind();
            }
        }
    }

    protected void GVfollowup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;
            decimal totalbalance = 0;
     
            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GVfollowup.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    totalDebit += Convert.ToDecimal((row.FindControl("lblDebit") as Label).Text);
                    totalCredit += Convert.ToDecimal((row.FindControl("lblCredit") as Label).Text);
                    totalbalance += Convert.ToDecimal((row.FindControl("lblBalance") as Label).Text);
              
                }
            }

      // Display the totals in the footer labels
      (e.Row.FindControl("lblTotalDebit") as Label).Text = totalDebit.ToString();
            (e.Row.FindControl("lblTotalCredit") as Label).Text = totalCredit.ToString();
            (e.Row.FindControl("lblTotalBalance") as Label).Text = totalbalance.ToString();
       
        }
    }

    protected void txtPartyName_TextChanged(object sender, EventArgs e)
    {
        GetData();
    }

    protected void GVfollowup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVfollowup.PageIndex = e.NewPageIndex;
        GetData();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        GetData();
    }
}
