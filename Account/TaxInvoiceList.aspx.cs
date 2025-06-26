using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using Microsoft.Reporting.WebForms;

public partial class Account_TaxInvoiceList : System.Web.UI.Page
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
                GVBinddata();
            }
        }
    }

    protected void GVBinddata()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_GetTableDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetTaxInvoiceList");
            if (Session["Designation"].ToString() == "Sales Manager")
            {
                cmd.Parameters.AddWithValue("@UserName", Session["UserCode"].ToString());
            }
            else
            {
                cmd.Parameters.AddWithValue("@UserName", DBNull.Value);
            }
            cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);          
            cmd.Parameters.AddWithValue("@PageSize", Convert.ToInt32(ddlPageSize.SelectedValue));   
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Columns.Count > 0)
            {
                GvInvoiceList.DataSource = dt;
                GvInvoiceList.DataBind();
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
                com.CommandText = "select DISTINCT BillingCustomer from tblTaxInvoiceHdr where " + "BillingCustomer like @Search + '%' AND isdeleted='0'  ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> BillingCustomer = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        BillingCustomer.Add(sdr["BillingCustomer"].ToString());
                    }
                }
                con.Close();
                return BillingCustomer;
            }

        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        GVBinddata();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("TaxInvoiceList.aspx");
    }

    protected void GvInvoiceList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("TaxInvoice.aspx?Id=" + encrypt(e.CommandArgument.ToString()));
        }
        if (e.CommandName == "DownloadPDF")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                Report(e.CommandArgument.ToString());
            }
        }
        if (e.CommandName == "RowDelete")
        {
            con.Open();
            SqlCommand cmdget = new SqlCommand("select AgainstNumber from tblTaxInvoiceHdr WHERE Id=@Id", con);
            cmdget.Parameters.AddWithValue("@Id", Convert.ToInt32(e.CommandArgument.ToString()));
            string pono = cmdget.ExecuteScalar().ToString();

            SqlCommand CmduptDtl = new SqlCommand("update OrderAccept set status=null where pono=@pono", con);
            CmduptDtl.Parameters.AddWithValue("@pono", pono);
            CmduptDtl.ExecuteNonQuery();

            SqlCommand Cmd = new SqlCommand("delete from tblTaxInvoiceHdr WHERE Id=@Id", con);
            Cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.ExecuteNonQuery();

            SqlCommand CmddeleteDtl = new SqlCommand("delete from tblTaxInvoiceDtls where HeaderID=@Id", con);
            CmddeleteDtl.Parameters.AddWithValue("@Id", Convert.ToInt32(e.CommandArgument.ToString()));
            CmddeleteDtl.ExecuteNonQuery();


            con.Close();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Deleted Sucessfully');window.location.href='TaxInvoiceList.aspx';", true);
        }
    }

    public string encrypt(string encryptString)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }

    protected void GvInvoiceList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                con.Open();
                int id = Convert.ToInt32(GvInvoiceList.DataKeys[e.Row.RowIndex].Values[0]);
                SqlCommand cmd = new SqlCommand("select COUNT(HeaderID) from tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
                Object Procnt = cmd.ExecuteScalar();
                Label grandtotal = (Label)e.Row.FindControl("lblProduct");
                grandtotal.Text = Procnt == null ? "0" : Procnt.ToString();
                con.Close();

                Label lblGrandTotal = (Label)e.Row.FindControl("lblGrandTotal");
                var gtot = Math.Round(Convert.ToDouble(lblGrandTotal.Text));

                lblGrandTotal.Text = gtot.ToString("#0.00");
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
                LinkButton lnkPDF = (LinkButton)e.Row.FindControl("lnkPDF");
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkDelete");
                Label lblInvoiceNo = (Label)e.Row.FindControl("lblInvoiceNo");
                Label lblFinalBasic = (Label)e.Row.FindControl("lblFinalBasic");

                if (lblInvoiceNo.Text != "")
                {
                    lblInvoiceNo.Text = lblInvoiceNo.Text;
                }
                else
                {
                    lblInvoiceNo.Text = lblFinalBasic.Text;
                }
                con.Open();
                Label lblDispatch = (Label)e.Row.FindControl("lblDispatch");
                Label lblpending = (Label)e.Row.FindControl("lblpending");
                Label lblapproved = (Label)e.Row.FindControl("lblapproved");
                SqlCommand cmd1 = new SqlCommand("select Status from tblTaxInvoiceHdr where Id='" + id + "'", con);
                Object Estatus = cmd1.ExecuteScalar();
                con.Close();
                if (Estatus.ToString() == "1")
                {
                    lblpending.Text = "Pending for Approval";
                }
                if (Estatus.ToString() == "2")
                {
                    lblapproved.Text = "Approved";
                }
                if (Estatus.ToString() == "3")
                {
                    lblDispatch.Text = "Dispatched";
                }
                string empcode = Session["UserCode"].ToString();
                DataTable Dt = new DataTable();
                SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
                Sd.Fill(Dt);
                if (Dt.Rows.Count > 0)
                {
                    string idd = Dt.Rows[0]["ID"].ToString();
                    DataTable Dtt = new DataTable();
                    SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + idd + "' AND PageName = 'CustomerTaxInvoiceList.aspx' AND PagesView = '1'", con);
                    Sdd.Fill(Dtt);
                    if (Dtt.Rows.Count > 0)
                    {
                        // Button1.Visible = false;
                        lnkEdit.Visible = false;
                        lnkDelete.Visible = false;
                        lnkPDF.Visible = true;
                    }
                }


                con.Close();
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private static DataTable GetData(string SP, int id)
    {
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand(SP, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                using (SqlDataAdapter sda_MismatchedValues = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda_MismatchedValues.SelectCommand = cmd;
                    using (DataSet ds_mis = new DataSet())
                    {
                        DataTable Dt_MismatchedValues = new DataTable();
                        sda_MismatchedValues.Fill(Dt_MismatchedValues);
                        return Dt_MismatchedValues;
                    }
                }
            }
        }
    }

    public void Report(string Invoiceno)
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
                    cmd.Parameters.AddWithValue("@Action", "GetE_InvoiceDetails");
                    cmd.Parameters.AddWithValue("@Invoiceno", Invoiceno);

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
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.Reset();
                    // DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\TaxInvoice.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    Response.ClearContent();
                    Response.ClearHeaders();
                    string filePath = Server.MapPath("~/PDF_Files/") + "TaxInvoice.pdf";

                    // Save the file to the specified path
                    System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                    Response.Redirect("~/PDF_Files/TaxInvoice.pdf");
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

    protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        GVBinddata();
    }
}