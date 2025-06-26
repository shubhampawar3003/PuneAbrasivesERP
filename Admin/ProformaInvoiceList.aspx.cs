
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_ProformaInvoiceList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    byte[] bytePdf;
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
                FillGrid();

            }
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddProformaInvoice.aspx");
    }

    //Fill GridView
    private void FillGrid()
    {
        if (Session["Role"].ToString() == "Admin")
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE TI.IsDeleted = 0 order by TI.CreatedOn DESC");
            GVPurchase.DataSource = Dt;
            GVPurchase.DataBind();
        }
        else
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE TI.CreatedBy='" + Session["UserCode"].ToString() + "' AND TI.IsDeleted = 0 order by TI.CreatedOn DESC");
            GVPurchase.DataSource = Dt;
            GVPurchase.DataBind();
        }

    }

    protected void GVPurchase_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("AddProformaInvoice.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowSendmail")
        {
            mailsendforCustomer(e.CommandArgument.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Proforma Invoice Send Successfully..!!')", true);
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_ProformaTaxInvoiceHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Tax Invoice Deleted Successfully..!!')", true);
            FillGrid();
        }
        if (e.CommandName == "RowView")
        {
            Report(e.CommandArgument.ToString(), "show");
            // Response.Redirect("PDF_ProformaInvoice.aspx?Invoiceno=" + objcls.encrypt(e.CommandArgument.ToString()) + " ");
            // Response.Write("<script>window.open ('Pdf_Quotation.aspx?Quotationno=" + (e.CommandArgument.ToString()) + "','_blank');</script>");
        }
    }

    protected void GVPurchase_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVPurchase.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    public void Report(string Invoiceno, string mail)
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
                    cmd.Parameters.AddWithValue("@Action", "GetProformaInvoiceDetails");
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
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\ProformaInvoice.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    bytePdf = null;
                    bytePdf = bytePdfRep;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    if (mail == "show")
                    {
                        Response.Buffer = true;
                        string Filename = Invoiceno + "_ProformaInvoice.pdf";
                        Response.ContentType = "application/vnd.pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "");
                        Response.BinaryWrite(bytePdfRep);
                    }
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

    protected void GVPurchase_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Authorization
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

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
                    btnCreate.Visible = false;
                    //GVQuotation.Columns[15].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }

    //Search Company Search methods
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
                com.CommandText = "SELECT DISTINCT BillingCustomer FROM [tbl_ProformaTaxInvoiceHdr] where " + "BillingCustomer like @Search + '%' and IsDeleted=0";

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
        if (txtCustomerName.Text != "" || txtCustomerName.Text != null)
        {
            string company = txtCustomerName.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE BillingCustomer='" + txtCustomerName.Text + "' AND TI.IsDeleted = 0 ORDER BY TI.ID DESC", Cls_Main.Conn);
            sad.Fill(dt);
            GVPurchase.EmptyDataText = "Not Records Found";
            GVPurchase.DataSource = dt;
            GVPurchase.DataBind();
        }
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProformaInvoiceList.aspx");
    }

    //Search Customer P.O.  Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCponoList(string prefixText, int count)
    {
        return AutoFillCponoName(prefixText);
    }

    public static List<string> AutoFillCponoName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT * FROM [tbl_ProformaTaxInvoiceHdr] where " + "Invoiceno like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Invoiceno"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void txtCpono_TextChanged(object sender, EventArgs e)
    {
        if (txtCpono.Text != "" || txtCpono.Text != null)
        {
            string Cpono = txtCpono.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE Invoiceno='" + Cpono + "' AND TI.IsDeleted = 0 ORDER BY TI.ID DESC", Cls_Main.Conn);
            sad.Fill(dt);
            GVPurchase.EmptyDataText = "Not Records Found";
            GVPurchase.DataSource = dt;
            GVPurchase.DataBind();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtCustomerName.Text) && string.IsNullOrEmpty(txtCpono.Text) && string.IsNullOrEmpty(txtfromdate.Text) && string.IsNullOrEmpty(txttodate.Text))
            {
                FillGrid();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Search Record');", true);
            }
            else
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    if (txtCpono.Text != "")
                    {
                        string Quono = txtCpono.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy where Invoiceno = '" + Quono + "' AND TI.IsDeleted = 0 ORDER BY TI.ID DESC", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                    if (txtCustomerName.Text != "")
                    {
                        string company = txtCustomerName.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy where BillingCustomer = '" + company + "' AND TI.IsDeleted = 0 ORDER BY TI.ID DESC", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE  TI.IsDeleted = 0 AND Invoicedate between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        sad.Fill(dt);

                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                }
                else
                {
                    if (txtCpono.Text != "")
                    {
                        string Quono = txtCpono.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy where CreatedBy='" + Session["UserCode"].ToString() + "'  AND SerialNo = '" + Quono + "' AND TI.IsDeleted = 0 ORDER BY TI.ID DESC", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                    if (txtCustomerName.Text != "")
                    {
                        string company = txtCustomerName.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy where CreatedBy='" + Session["UserCode"].ToString() + "'  AND CustomerName = '" + company + "' AND TI.IsDeleted = 0 ORDER BY TI.ID DESC", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE CreatedBy='" + Session["UserCode"].ToString() + "'  AND  TI.IsDeleted = 0 AND CreatedOn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        sad.Fill(dt);

                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void mailsendforCustomer(string Invoiceno)
    {
        string mail = string.Empty;
        string Customer = string.Empty;
        try
        {
            Report(Invoiceno, "mail");
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ProformaTaxInvoiceHdr] AS TI INNER JOIN tbl_UserMaster AS UM on UM.UserCode=TI.CreatedBy WHERE TI.InvoiceNo='" + Invoiceno + "' AND TI.IsDeleted = 0 order by TI.CreatedOn DESC");
            if (Dt.Rows.Count > 0)
            {
                mail = Dt.Rows[0]["EmailID"].ToString();
                Customer = Dt.Rows[0]["BillingCustomer"].ToString();
            }
            string strMessage = "Hello Sir/Ma'am <br/>" +
                 "We sent you an PI." + "PI - " + Invoiceno + "_ProformaInvoice.pdf" + "<br/>" +

                 "Kind Regards," + "<br/>" +
                 "<strong>Pune Abrasives Pvt. Ltd.<strong>";
            string fileName = Invoiceno + "_ProformaInvoice.pdf";
            string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
            string mailTo = mail.Trim().ToLower();
            MailMessage mm = new MailMessage();
            // mm.From = new MailAddress(fromMailID);

            mm.Subject = Customer + "_ProformaInvoice.pdf";
            mm.To.Add(mailTo);
            mm.CC.Add("girish.kulkarni@puneabrasives.com");
            //mm.CC.Add("b.tikhe@puneabrasives.com");
            mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
            if (bytePdf != null)
            {
                Stream stream = new MemoryStream(bytePdf);
                Attachment aa = new Attachment(stream, fileName);
                mm.Attachments.Add(aa);
            }

            StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
            string readFile = reader.ReadToEnd();
            string myString = "";
            myString = readFile;

            string multilineText = strMessage;
            string formattedText = multilineText.Replace("\n", "<br />");

            myString = myString.Replace("$Comment$", formattedText);

            mm.Body = myString.ToString();
            mm.IsBodyHtml = true;
            //mm.From = new MailAddress("girish.kulkarni@puneabrasives.com", fromMailID);
            mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);

            // Set the "Reply-To" header to indicate the desired display address
            mm.ReplyToList.Add(new MailAddress(fromMailID));

            SmtpClient smtp = new SmtpClient();
            smtp.Host = ConfigurationManager.AppSettings["Host"]; ;
            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
            System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
            NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToString();

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };
             smtp.Send(mm);
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
}


