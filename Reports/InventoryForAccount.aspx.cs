
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_InventoryForAccount : System.Web.UI.Page
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
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetInventoryList");
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
                    GVInentory.DataSource = Dtt;
                    GVInentory.DataBind();
                }
            }

        }
        catch (Exception ex)
        {
            //throw;
            //string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Not Found....!');", true);

        }

    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("InventoryForAccount.aspx");
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

    public void Report()
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
                    cmd.Parameters.AddWithValue("@Action", "GetInventoryList");
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
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\Inventory.rdlc";

                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment; filename=InventoryReports.xls");


                    Response.BinaryWrite(bytePdfRep);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.Reset();
                    //string filePath = Server.MapPath("~/PDF_Files/") + "InventoryList.pdf";
                    //// Save the file to the specified path
                    //System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                    //Response.Redirect("~/PDF_Files/InventoryList.pdf");
                    //ReportViewer1.LocalReport.DataSources.Clear();
                    //ReportViewer1.Reset();
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
            // ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please try again leter....!');", true);
        }
    }

    protected void GVInentory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label Batch = (Label)e.Row.FindControl("Batch");
            if (Batch.Text == "Total")
            {
                e.Row.BackColor = System.Drawing.Color.LightPink;
            }

        }
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        Report();
    }

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }
}