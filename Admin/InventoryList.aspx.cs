
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


public partial class Admin_InventoryList : System.Web.UI.Page
{
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.ModalPopupHistory.Hide();
            if (Session["UserCode"] == null)
            {
                Response.Redirect("../Login.aspx");
            }
            else
            {
                FillGrid();
                ActiveCount();
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
                    cmd.Parameters.AddWithValue("@Action", "GetInventory");
                    cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
                    cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
                    cmd.Parameters.AddWithValue("@ComponentName", txtproduct.Text);
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

    protected void ActiveCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        SqlCommand cmd = new SqlCommand("WITH ComponentBalances AS ( SELECT BC.ComponentName, SUM(BC.BalanceQty) AS BalanceQty, CM.lessqtylimit FROM vw_batchwisecomponent AS BC JOIN tbl_ComponentMaster AS CM ON BC.ComponentName = CM.ComponentName GROUP BY BC.ComponentName, CM.lessqtylimit HAVING SUM(BC.BalanceQty) <= CM.lessqtylimit ) SELECT COUNT(*) FROM ComponentBalances", Cls_Main.Conn);
        count = Convert.ToInt16(cmd.ExecuteScalar());

        lblcount.Text = count.ToString();
        Cls_Main.Conn_Close();
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
                com.CommandText = "select Distinct ComponentName from vw_batchwisecomponent where  " + "ComponentName like '%'+ @Search + '%' ";

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
        FillGrid();

    }

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        Report();
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
                    cmd.Parameters.AddWithValue("@Action", "GetInventory");
                    cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
                    cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
                    cmd.Parameters.AddWithValue("@ComponentName", txtproduct.Text);
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
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\Stock.rdlc";

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

    }

    protected void GVInentory_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "ShowDetails") // Ensure this matches the CommandName used in your GridView
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument)-1;
            GridViewRow row = GVInentory.Rows[rowIndex];

            Label Productname = row.FindControl("Productname") as Label;
            Label Batch = row.FindControl("Batch") as Label;    

            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            DataSet Dtt = new DataSet();

            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetInOutComponent");
                    cmd.Parameters.AddWithValue("@ComponentName", Productname.Text);
                    cmd.Parameters.AddWithValue("@Batch", Batch.Text);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        sda.Fill(Dtt);
                    }
                }
            }

            if (Dtt.Tables.Count > 0 && Dtt.Tables[0].Rows.Count > 0)
            {
                gvDetails.DataSource = Dtt.Tables[0];
                gvDetails.DataBind();
                this.ModalPopupHistory.Show();
            }
            else
            {
                this.ModalPopupHistory.Hide();
            }

        
            
        }
    }

}