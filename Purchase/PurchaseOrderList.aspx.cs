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
using iTextSharp.text.pdf.codec;
using Microsoft.Reporting.WebForms;
using DocumentFormat.OpenXml.Spreadsheet;

public partial class Purchase_PurchaseOrderList : System.Web.UI.Page
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
                ddlStatus.SelectedValue = "Open";
                Gvbind();
            }
        }
    }

    private void Gvbind()
    {
        string query = string.Empty;
        query = @"select * from tblPurchaseOrderHdr order by Id desc";
        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvPurchaseOrder.DataSource = dt;
            GvPurchaseOrder.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvPurchaseOrder.ClientID + "', 500, 1020 , 40 ,true); </script>", false);
        }
        else
        {
            GvPurchaseOrder.DataSource = null;
            GvPurchaseOrder.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvPurchaseOrder.ClientID + "', 500, 1020 , 40 ,true); </script>", false);
        }
    }

    protected void GvPurchaseOrder_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ViewState["CompRowId"] = e.CommandArgument.ToString();
        if (e.CommandName == "RowEdit")
        {
            ViewState["id"] = e.CommandArgument.ToString();
            Response.Redirect("PurchaseOrder.aspx?ID=" + encrypt(e.CommandArgument.ToString())+"&Action=OLD");
        }
        if (e.CommandName == "RowNew")
        {
            Response.Redirect("PurchaseOrder.aspx?ID=" + encrypt(e.CommandArgument.ToString()) + "&Action=NEW");
        }

        if (e.CommandName == "DownloadPDF")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                Session["PDFID"] = e.CommandArgument.ToString();
                // Response.Redirect("PurchaseOrderPDF.aspx?ID=" + encrypt(e.CommandArgument.ToString()));
                Report(e.CommandArgument.ToString());
            }
        }

        if (e.CommandName == "RowDelete")
        {
            con.Open();
            SqlCommand Cmd = new SqlCommand("Delete From [tblPurchaseOrderHdr] WHERE Id=@Id", con);
            Cmd.Parameters.AddWithValue("@Id", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.ExecuteNonQuery();
            con.Close();
            con.Open();
            SqlCommand Cmdd = new SqlCommand("Delete From [tblPurchaseOrderDtls] WHERE HeaderID=@HeaderID", con);
            Cmdd.Parameters.AddWithValue("@HeaderID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmdd.ExecuteNonQuery();
            con.Close();
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Deleted Sucessfully');window.location.href='PurchaseOrderList.aspx';", true);

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

    public string Decrypt(string cipherText)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }


    #region Filter

    protected void txtcnamefilter_TextChanged(object sender, EventArgs e)
    {
        string query = string.Empty;
        if (!string.IsNullOrEmpty(txtcnamefilter.Text.Trim()))
        {
            query = "SELECT * FROM tblPurchaseOrderHdr where Mode='Open' and  SupplierName like '%" + txtcnamefilter.Text.Trim() + "%' order by Id desc";
        }
        else
        {
            query = "SELECT * FROM tblPurchaseOrderHdr where Mode='Open' order by DeliveryDate desc";
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvPurchaseOrder.DataSource = dt;
            GvPurchaseOrder.DataBind();
        }
        else
        {
            GvPurchaseOrder.DataSource = null;
            GvPurchaseOrder.DataBind();
        }
    }
    #endregion Filter

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseOrderList.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierList(string prefixText, int count)
    {
        return AutoFillSupplierName(prefixText);
    }

    public static List<string> AutoFillSupplierName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [VendorName] from [tbl_VendorMaster] where " + "VendorName like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["VendorName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierOwnerList(string prefixText, int count)
    {
        return AutoFillSupplierOwnerName(prefixText);
    }

    public static List<string> AutoFillSupplierOwnerName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Ownername] from [tbl_VendorMaster] where Ownername like @Search + '%' and Status=1 and [IsDeleted]=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Ownername"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }


    protected void ddlsalesMainfilter_TextChanged(object sender, EventArgs e)
    {
        Gvbind();
    }


    protected void linkbtnfile_Click(object sender, EventArgs e)
    {
        string id = encrypt(((sender as ImageButton).CommandArgument).ToString());

        string POID = Decrypt(id);

        string strQuery = "select SupplierName,RefDocuments from tblPurchaseOrderHdr where Id=@id";
        SqlCommand cmd = new SqlCommand(strQuery);
        cmd.Parameters.Add("@id", SqlDbType.Int).Value = Convert.ToInt32(POID);
        DataTable dt = GetData(cmd);
        if (dt != null)
        {
            download(dt);
        }
        //Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid=" + id + "&SN=1','','width=700px,height=600px');", true);
    }

    private DataTable GetData(SqlCommand cmd)
    {
        DataTable dt = new DataTable();
        String strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        SqlConnection con = new SqlConnection(strConnString);
        SqlDataAdapter sda = new SqlDataAdapter();
        cmd.CommandType = CommandType.Text;
        cmd.Connection = con;
        try
        {
            con.Open();
            sda.SelectCommand = cmd;
            sda.Fill(dt);
            return dt;
        }
        catch
        {
            return null;
        }
        finally
        {
            con.Close();
            sda.Dispose();
            con.Dispose();
        }
    }

    private void download(DataTable dt)
    {
        if (dt.Rows[0]["RefDocuments"].ToString() != "")
        {
            Byte[] bytes = (Byte[])dt.Rows[0]["RefDocuments"];
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.ContentType = dt.Rows[0]["ContentType"].ToString();
            Response.AddHeader("content-disposition", "attachment;filename=" + dt.Rows[0]["SupplierName"].ToString());
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.End();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('File Not Found !!');", true);
        }
    }
    protected void txtPONo_TextChanged(object sender, EventArgs e)
    {
        string query = string.Empty;
        if (!string.IsNullOrEmpty(txtPONo.Text.Trim()))
        {
            query = "SELECT * FROM tblPurchaseOrderHdr where Mode='Open' and  PONo like '%" + txtPONo.Text.Trim() + "%' order by Id desc";
        }
        else
        {
            query = "SELECT * FROM tblPurchaseOrderHdr where Mode='Open' order by DeliveryDate desc";
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvPurchaseOrder.DataSource = dt;
            GvPurchaseOrder.DataBind();
        }
        else
        {
            GvPurchaseOrder.DataSource = null;
            GvPurchaseOrder.DataBind();
        }
    }


    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetPOList(string prefixText, int count)
    {
        return AutoFillPO(prefixText);
    }

    public static List<string> AutoFillPO(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [PONo] from [tblPurchaseOrderHdr] where " + "PONo like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["PONo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void GvPurchaseOrder_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
            LinkButton btnPDF = e.Row.FindControl("btnPDF") as LinkButton;
            string empcode = Session["UserCode"].ToString();
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                string id = Dt.Rows[0]["ID"].ToString();
                DataTable Dtt = new DataTable();
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'ProformaInvoiceList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    LinkButton1.Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                    btnPDF.Visible = true;
                }
            }
            //string empcode = Session["empcode"].ToString();
            //DataTable Dt = new DataTable();
            //SqlDataAdapter Sd = new SqlDataAdapter("Select id from [employees] where [empcode]='" + empcode + "'", con);
            //Sd.Fill(Dt);
            //if (Dt.Rows.Count > 0)
            //{
            //    string id = Dt.Rows[0]["id"].ToString();
            //    DataTable Dtt = new DataTable();
            //    SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName ='PurchaseOrderList.aspx' AND PagesView = '1'", con);
            //    Sdd.Fill(Dtt);
            //    if (Dtt.Rows.Count > 0)
            //    {
            //        btnAddPO.Visible = false;
            //        btnEdit.Visible = false;
            //        btnDelete.Visible = false;
            //        btnPDF.Visible = true;
            //    }
            //}

            /// Label lblMode = e.Row.FindControl("lblMode") as Label;
            // if (lblMode.Text == "Open")
            //{
            //    lblMode.Text = "Unpaid";
            // }
            // else {
            //    lblMode.Text = "Paid";
            // }

        }
    }
    protected void ddlStatus_TextChanged(object sender, EventArgs e)
    {
        try
        {
            string query = string.Empty;
            if (ddlStatus.Text == "0" || ddlStatus.Text == "Open" || ddlStatus.Text == "Close" && !string.IsNullOrEmpty(txtcnamefilter.Text.Trim()))
            {
                if (ddlStatus.Text == "0")
                {
                    query = "SELECT * FROM tblPurchaseOrderHdr where SupplierName like '%" + txtcnamefilter.Text.Trim() + "%' order by Id desc";
                }
                else
                {
                    query = "SELECT * FROM tblPurchaseOrderHdr where Mode='" + ddlStatus.Text + "' and  SupplierName like '%" + txtcnamefilter.Text.Trim() + "%' order by Id desc";
                }
            }
            else if (ddlStatus.Text == "0")
            {
                query = "SELECT * FROM tblPurchaseOrderHdr order by Id desc";
            }
            else if (ddlStatus.Text == "Open")
            {
                query = "SELECT * FROM tblPurchaseOrderHdr where Mode='Open' order by Id desc";
            }
            else
            {
                query = "SELECT * FROM tblPurchaseOrderHdr where Mode='Close' order by Id desc";
            }

            SqlDataAdapter ad = new SqlDataAdapter(query, con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                GvPurchaseOrder.DataSource = dt;
                GvPurchaseOrder.DataBind();
            }
            else
            {
                GvPurchaseOrder.DataSource = null;
                GvPurchaseOrder.DataBind();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseOrder.aspx");
    }

    public void Report(string PONO)
    {
        //try
        //{
        DataSet Dtt = new DataSet();
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand("[SP_GetTableDetails]", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "GetPOReport");
                cmd.Parameters.AddWithValue("@PONO", PONO);

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
                ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PurchaseOrder.rdlc";
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
                string filePath = Server.MapPath("~/PDF_Files/") + "PurchaseOrder1.pdf";

                // Save the file to the specified path
                System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                Response.Redirect("~/PDF_Files/PurchaseOrder1.pdf");

                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.Reset();

            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
            }
        }
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
    }
}