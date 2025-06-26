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
using System.Globalization;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public partial class Admin_TaxInvoiceList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

    private readonly DateTimeFormatInfo UkDtfi = new CultureInfo("en-GB", false).DateTimeFormat;

    private SqlCommand Cmd;
    public string AuthToken = "";

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
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select TOP 10 * from tblTaxInvoiceHdr where isdeleted='0' AND Status>2 order by CreatedOn DESC", con);
            sad.Fill(dt);
            GvInvoiceList.DataSource = dt;
            GvInvoiceList.DataBind();
            GvInvoiceList.EmptyDataText = "Record Not Found";
            //ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvInvoiceList.ClientID + "', 400, 1020 , 40 ,true); </script>", false);
        }
        catch (Exception)
        {
            throw;
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
        try
        {
            DataTable dtt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceHdr where BillingCustomer='" + txtCustomerName.Text + "' AND isdeleted='0' order by CreatedOn DESC", con);
            sad.Fill(dtt);
            GvInvoiceList.DataSource = dtt;
            GvInvoiceList.DataBind();
            GvInvoiceList.EmptyDataText = "Record Not Found";
        }
        catch (Exception)
        {
            throw;
        }
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("EWayBillList.aspx");
    }

    protected void GvInvoiceList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RowCreate")
        {
            string ID = e.CommandArgument.ToString();
            Response.Redirect("EwayBill.aspx?Id=" + encrypt(e.CommandArgument.ToString()));
        }
        if (e.CommandName == "DownloadPDF")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                Session["PDFID"] = e.CommandArgument.ToString();
                Response.Redirect("E_WayBill_PDF.aspx?Id=" + encrypt(e.CommandArgument.ToString()));
               // Response.Write("<script>window.open ('E_WayBill_PDF.aspx?Id=" + encrypt(e.CommandArgument.ToString()) + "','_blank');</script>");
            }
        }
        if (e.CommandName == "RowCancel")
        {
            string ID = e.CommandArgument.ToString();
            Response.Redirect("EwayBill.aspx?CnlId=" + encrypt(e.CommandArgument.ToString()));
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
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            con.Open();
            LinkButton lnkCreateInv = (LinkButton)e.Row.FindControl("lnkCreateInv");
            LinkButton lnkCancel = (LinkButton)e.Row.FindControl("lnkCancel");
            LinkButton lnkPDF = (LinkButton)e.Row.FindControl("lnkPDF");
            Label lblInvoiceNo = (Label)e.Row.FindControl("lblInvoiceNo");
            Label lblFinalBasic = (Label)e.Row.FindControl("lblFinalBasic");

            int idd = Convert.ToInt32(GvInvoiceList.DataKeys[e.Row.RowIndex].Values[0]);
            DataTable Dtt = new DataTable();
            SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tbltaxinvoicehdr where Id = '" + idd + "'", con);
            Sdd.Fill(Dtt);
            if (Dtt.Rows.Count > 0)
            {
                string e_way_status = Dtt.Rows[0]["e_way_status"].ToString();
                string e_way_cancel_status = Dtt.Rows[0]["e_way_cancel_status"].ToString();

                if (e_way_status == true.ToString() && e_way_cancel_status == true.ToString())
                {
                    lnkCreateInv.Visible = false;
                    lnkPDF.Visible = true;
                    lnkCancel.Visible = false;
                    lnkCancel.Enabled = false;
                    e.Row.BackColor = System.Drawing.Color.LightPink;
                }
                else if (e_way_status == true.ToString()) 
                {
                    lnkCreateInv.Visible = false;
                    lnkPDF.Visible = true;
                    lnkCancel.Visible = true;
                    lnkCancel.Enabled = true;
                    //lnkCancel.Enabled = false;
                    e.Row.BackColor = System.Drawing.Color.LightGray;
                }

                if (lblInvoiceNo.Text != "")
                {
                    lblInvoiceNo.Text = lblInvoiceNo.Text;
                }
                else
                {
                    lblInvoiceNo.Text = lblFinalBasic.Text;
                }
            }
            con.Close();
        }
    }

    protected void btneinvoice_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Admin/EInvoiceList.aspx");
    }
}