
using iTextSharp.text;
using iTextSharp.text.pdf;
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


public partial class Admin_CustomerPurchaseOrderList : System.Web.UI.Page
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
        Response.Redirect("AddCustomerPO.aspx");
    }

    //Fill GridView
    private void FillGrid()
    {
        if (Session["Role"].ToString() == "Admin")
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE CP.IsDeleted = 0 ORDER BY CP.ID DESC");
            GVPurchase.DataSource = Dt;
            GVPurchase.DataBind();
        }
        else
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  CP.IsDeleted = 0 ORDER BY CP.ID DESC");
            GVPurchase.DataSource = Dt;
            GVPurchase.DataBind();
        }

    }

    protected void GVPurchase_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("AddCustomerPO.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "AddInvoice")
        {
            Response.Redirect("../Account/TaxInvoice.aspx?PONO=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowSendmail")
        {
            mailsendforCustomer(e.CommandArgument.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Mail Send Successfully..!!')", true);
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_CustomerPurchaseOrderHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Order Acceptance Deleted Successfully..!!')", true);
            FillGrid();
        }
        if (e.CommandName == "RowView")
        {
            Report(e.CommandArgument.ToString(),"PDF");
            //Response.Redirect("Pdf_CustomerPurchase.aspx?Pono=" + objcls.encrypt(e.CommandArgument.ToString()) + " ");
            // Response.Write("<script>window.open ('Pdf_Quotation.aspx?Quotationno=" + (e.CommandArgument.ToString()) + "','_blank');</script>");
        }
    }

    protected void GVPurchase_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVPurchase.PageIndex = e.NewPageIndex;
        FillGrid();
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'CustomerPurchaseOrderList.aspx' AND PagesView = '1'", con);
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

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
            Label OANumber = e.Row.FindControl("Pono") as Label;
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select TOP 1 InvoiceNo from tblTaxInvoiceHdr where AgainstNumber='" + OANumber.Text + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                btnEdit.Visible = false;
                btnDelete.Visible = false;
            }
            else
            {
                btnEdit.Visible = true;
                btnDelete.Visible = true;
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
                com.CommandText = "SELECT DISTINCT [ID],[Companyname] FROM [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Companyname"].ToString());
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
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE CP.IsDeleted = 0 AND   CustomerName='" + txtCustomerName.Text + "' ORDER BY CP.ID DESC", Cls_Main.Conn);
            sad.Fill(dt);
            GVPurchase.EmptyDataText = "Not Records Found";
            GVPurchase.DataSource = dt;
            GVPurchase.DataBind();
        }
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("CustomerPurchaseOrderList.aspx");
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
                com.CommandText = "SELECT * FROM [tbl_CustomerPurchaseOrderHdr] where " + "SerialNo like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["SerialNo"].ToString());
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
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName AND SerialNo='" + Cpono + "' ORDER BY CP.ID DESC", Cls_Main.Conn);
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
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where SerialNo = '" + Quono + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                    if (txtCustomerName.Text != "")
                    {
                        string company = txtCustomerName.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where CustomerName = '" + company + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE  CP.IsDeleted = 0 AND CP.CreatedOn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
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
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  SerialNo = '" + Quono + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                    if (txtCustomerName.Text != "")
                    {
                        string company = txtCustomerName.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  CustomerName = '" + company + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  CP.CreatedOn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
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

    //Search GST WIse Company methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetGSTList(string prefixText, int count)
    {
        return AutoFillGSTName(prefixText);
    }

    public static List<string> AutoFillGSTName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT GSTNo FROM [tbl_CustomerPurchaseOrderHdr] where " + "GSTNo like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["GSTNo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtGST_TextChanged(object sender, EventArgs e)
    {
        if (txtGST.Text != "" || txtGST.Text != null)
        {
            string GST = txtGST.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where GSTNo = '" + GST + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVPurchase.EmptyDataText = "Not Records Found";
            GVPurchase.DataSource = dt;
            GVPurchase.DataBind();
        }
    }

    protected void ImageButtonfile5_Click(object sender, ImageClickEventArgs e)
    {
        string id = ((sender as ImageButton).CommandArgument).ToString();

        Display(id);
    }

    public void Display(string id)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string CmdText = "select fileName from tbl_CustomerPurchaseOrderHdr where IsDeleted=0 AND ID='" + id + "'";

                SqlDataAdapter ad = new SqlDataAdapter(CmdText, con);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //Response.Write(dt.Rows[0]["Path"].ToString());
                    if (!string.IsNullOrEmpty(dt.Rows[0]["fileName"].ToString()))
                    {
                        Response.Redirect("~/PDF_Files/" + dt.Rows[0]["fileName"].ToString());
                    }
                    else
                    {
                        //lblnotfound.Text = "File Not Found or Not Available !!";
                    }
                }
                else
                {
                    //lblnotfound.Text = "File Not Found or Not Available !!";
                }

            }
        }
    }
 
    //added 11/11/2024 mail send creation
    private void mailsendforCustomer(string Pono)
    {
        string mail = string.Empty;
        string Customer = string.Empty;
        string OANo = string.Empty;
        string OADate = string.Empty;
        try
        {
            Report(Pono,"Mail");
            DataTable Dt = Cls_Main.Read_Table("SELECT  CONVERT(nvarchar(10),PoDate,103) AS PoDate, * FROM tbl_CustomerPurchaseOrderHdr WHERE  IsDeleted=0 AND Pono='" + Pono + "' ");
            if (Dt.Rows.Count > 0)
            {
                mail = Dt.Rows[0]["EmailID"].ToString();
                Customer = Dt.Rows[0]["CustomerName"].ToString();
                OANo = Dt.Rows[0]["Pono"].ToString();
                OADate = Dt.Rows[0]["PoDate"].ToString();
            }
            string strMessage = "Hello Sir/Ma'am <br/>" +
                 "We sent you  OA-" + Customer + "_" + OANo + "_" + OADate + ".pdf" + "<br/>" +

                 "Kind Regards," + "<br/><br/>" +
                 "<strong>Pune Abrasives Pvt. Ltd.<strong>";
            string fileName = Pono + "_OA.pdf";
            string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
            string mailTo = mail.Trim().ToLower();
            MailMessage mm = new MailMessage();
            // mm.From = new MailAddress(fromMailID);

            mm.Subject = Customer + "_" + OANo + "_" + OADate + ".pdf";
            //  mm.To.Add("erpdeveloper3003@gmail.com");
            mm.To.Add(mailTo);
            mm.CC.Add("girish.kulkarni@puneabrasives.com");
            mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
            StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
            string readFile = reader.ReadToEnd();
            string myString = "";
            myString = readFile;
            string multilineText = strMessage;
            string formattedText = multilineText.Replace("\n", "<br />");
            myString = myString.Replace("$Comment$", formattedText);
            mm.Body = myString.ToString();
            mm.IsBodyHtml = true;
            string filePath = Server.MapPath("~/PDF_Files/_OrderAcceptance.pdf");
            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                Stream stream = new MemoryStream(fileBytes);
                Attachment aa = new Attachment(stream, fileName);
                mm.Attachments.Add(aa);
            }
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

    public void Report(string OANO,string Type)
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
                    cmd.Parameters.AddWithValue("@Action", "GetOADetails");
                    cmd.Parameters.AddWithValue("@Invoiceno", OANO);

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
                    DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\OrderAcceptance.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    bytePdf = bytePdfRep;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    if(Type =="PDF")
                    {
                        Response.Buffer = true;
                        string Filename = OANO + "_OrderAcceptance.pdf";
                        Response.ContentType = "application/vnd.pdf";
                        Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "");
                        Response.BinaryWrite(bytePdfRep);
                    }
                    else
                    {
                        string filePath = Server.MapPath("~/PDF_Files/") + "_OrderAcceptance.pdf";

                        // Save the file to the specified path
                        System.IO.File.WriteAllBytes(filePath, bytePdfRep);
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
}


