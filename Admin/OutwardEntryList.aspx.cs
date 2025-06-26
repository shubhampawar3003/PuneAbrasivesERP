
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


public partial class Admin_OutwardEntryList : System.Web.UI.Page
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
                
                FillGrid();
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
                com.CommandText = "SELECT DISTINCT [companyname] FROM [tbl_OutwardEntryHdr] where " + "companyname like @Search + '%' and IsDeleted=0";

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
        if (txtCustomerName.Text != "" || txtCustomerName.Text != null)
        {
            string company = txtCustomerName.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_OutwardEntryHdr] where companyname = '" + company + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVOutward.EmptyDataText = "Not Records Found";
            GVOutward.DataSource = dt;
            GVOutward.DataBind();
        }
    }
    //Search Company Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetOrderNoList(string prefixText, int count)
    {
        return AutoFillOrderNo(prefixText);
    }

    public static List<string> AutoFillOrderNo(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT [ID],[ChallanNo] FROM [tbl_OutwardEntryHdr] where " + "ChallanNo like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["ChallanNo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtOrderno_TextChanged(object sender, EventArgs e)
    {
        string challanno = txtOrderno.Text;

        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_OutwardEntryHdr] where ChallanNo = '" + challanno + "' AND IsDeleted = 0", Cls_Main.Conn);
        sad.Fill(dt);
        GVOutward.EmptyDataText = "Not Records Found";
        GVOutward.DataSource = dt;
        GVOutward.DataBind();

    }


    //Fill GridView
    private void FillGrid()
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_OutwardEntryHdr] WHERE IsDeleted = 0 ORDER BY ID DESC");
        GVOutward.DataSource = Dt;
        GVOutward.DataBind();
    }

    //private void FillddlQuotationno()
    //{
    //    SqlDataAdapter ad = new SqlDataAdapter("SELECT [ID],[ChallanNo] FROM [tbl_OutwardEntryHdr] WHERE IsDeleted = 0", Cls_Main.Conn);
    //    DataTable dt = new DataTable();
    //    ad.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        ddlOrderno.DataSource = dt;
    //        ddlOrderno.DataValueField = "ID";
    //        ddlOrderno.DataTextField = "ChallanNo";
    //        ddlOrderno.DataBind();
    //        ddlOrderno.Items.Insert(0, " --  Select Challan No. -- ");
    //    }
    //}





    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("OutwardEntry.aspx");
    }

    protected void GVOutward_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("OutwardEntry.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }

        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_OutwardEntryHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Outward Entry Deleted Successfully..!!')", true);
            FillGrid();
        }
        if (e.CommandName == "show")
        {
            ViewState["ID"] = e.CommandArgument.ToString();
            this.ModalPopupHistory.Show();

        }
        if (e.CommandName == "Letter")
        {
            string id = e.CommandArgument.ToString();
            Display(id);

        }
    }

    protected void GVOutward_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVOutward.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVOutward_RowDataBound(object sender, GridViewRowEventArgs e)
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'OutwardEntryList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                   
                    GVOutward.Columns[9].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtOrderno.Text) && string.IsNullOrEmpty(txtCustomerName.Text) && string.IsNullOrEmpty(txtfromdate.Text) && string.IsNullOrEmpty(txttodate.Text))
            {
                FillGrid();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Search Record');", true);
            }
            else
            {
                if (txtOrderno.Text != "")
                {
                    string challanno = txtOrderno.Text;

                    DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_OutwardEntryHdr] where ChallanNo = '" + challanno + "' AND IsDeleted = 0", Cls_Main.Conn);
                    sad.Fill(dt);
                    GVOutward.EmptyDataText = "Not Records Found";
                    GVOutward.DataSource = dt;
                    GVOutward.DataBind();
                }
                if (txtCustomerName.Text != "")
                {
                    string company = txtCustomerName.Text;

                    DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_OutwardEntryHdr] where companyname = '" + company + "' AND IsDeleted = 0", Cls_Main.Conn);
                    sad.Fill(dt);
                    GVOutward.EmptyDataText = "Not Records Found";
                    GVOutward.DataSource = dt;
                    GVOutward.DataBind();
                }

                if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                {
                    DataTable dt = new DataTable();

                    //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                    SqlDataAdapter sad = new SqlDataAdapter(" SELECT * FROM [tbl_OutwardEntryHdr] WHERE ChallanDate between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                    sad.Fill(dt);

                    GVOutward.EmptyDataText = "Not Records Found";
                    GVOutward.DataSource = dt;
                    GVOutward.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        string PathH = null;

        HttpPostedFile postedFile = FileUpload1.PostedFile;
        if (FileUpload1.HasFile)
        {
            foreach (HttpPostedFile PostedFile in FileUpload1.PostedFiles)
            {
                string filename = Path.GetFileName(postedFile.FileName);
                string[] pdffilename = filename.Split('.');
                string pdffilename1 = pdffilename[0];
                string filenameExt = pdffilename[1];
                if (filenameExt == "pdf" || filenameExt == "PDF")
                {
                    string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                    postedFile.SaveAs(Server.MapPath("~/PDF_Files/") + pdffilename1 + time1 + "." + filenameExt);
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("UPDATE [tbl_OutwardEntryHdr] SET LRLetterPath=@LRLetterPath,LRNo=@LRNo,LRUpdatedby=@LRUpdatedby,LRUploadOn=@LRUploadOn WHERE ID=@ID", Cls_Main.Conn);
                    Cmd.Parameters.AddWithValue("@LRLetterPath", "PDF_Files/" + pdffilename1 + time1 + "." + filenameExt);
                    Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(ViewState["ID"].ToString()));
                    Cmd.Parameters.AddWithValue("@LRNo", txtLRno.Text);
                    Cmd.Parameters.AddWithValue("@LRUpdatedby", Session["UserCode"].ToString());
                    Cmd.Parameters.AddWithValue("@LRUploadOn", DateTime.Now);
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    string file = pdffilename1 + time1 + "." + filenameExt;
                    mailsendforCustomer(file);
                    Response.Redirect("OutwardEntryList.aspx");
                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('LR Letter updated Successfully...!');window.location='OutwardEntryList.aspx';", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                }
            }
        }
    }

    public void Display(string id)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                Int64 ID = Convert.ToInt64(id);
                string CmdText = "SELECT [ID],'../'+[LRLetterPath] as Path FROM [tbl_OutwardEntryHdr] where ID='" + ID + "'";

                SqlDataAdapter ad = new SqlDataAdapter(CmdText, con);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //Response.Write(dt.Rows[0]["Path"].ToString());
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Path"].ToString()))
                    {
                        Response.Redirect(dt.Rows[0]["Path"].ToString());
                    }
                    else
                    {
                        // lblnotfound.Text = "File Not Found or Not Available !!";
                    }
                }
                else
                {
                    // lblnotfound.Text = "File Not Found or Not Available !!";
                }

            }
        }
    }


    private void mailsendforCustomer(string fileName)
    {
        try
        {
            string mailTo = string.Empty;
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select email from tbl_OutwardEntryHdr where ID='"+ Convert.ToInt32(ViewState["ID"].ToString()) + "'", Cls_Main.Conn);
            sad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                mailTo = dt.Rows[0]["email"].ToString();
               // mailTo = "shubhpawar59@gmail.com";
            }
            string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
            MailMessage mm = new MailMessage();
          //  mm.From = new MailAddress(fromMailID);
            string strMessage =
"We sent you an LR Letter." + fileName + "<br/>" +

"Please find herewith attached LR Letter for your reference." + "<br/>" +

"Feel free to contact us for any further queries & Clarifications." + "<br/>" +

"Kind Regards," + "<br/>" +
"<strong>Pune Abrasive Pvt. Ltd.<strong>";
            mm.Subject = " - UPDATE - Pune Abrasive Pvt. Ltd.";
            mm.To.Add(mailTo);
            mm.CC.Add("girish.kulkarni@puneabrasives.com");
            mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
            if (!string.IsNullOrEmpty(fileName))
            {
                byte[] file = File.ReadAllBytes(Server.MapPath("~/PDF_Files/") + fileName);

                Stream stream = new MemoryStream(file);
                Attachment aa = new Attachment(stream, fileName);
                mm.Attachments.Add(aa);
            }
            StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
            string readFile = reader.ReadToEnd();
            string myString = "";
            myString = readFile;

            string multilineText = strMessage;
            string formattedText = multilineText.Replace("\n", "<br />");
            mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);
            myString = myString.Replace("$Comment$", formattedText);
            mm.ReplyToList.Add(new MailAddress(fromMailID));
            mm.Body = myString.ToString();

            mm.IsBodyHtml = true;
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


            // Alternatively, you can set the "Sender" header
            // message.Sender = new MailAddress("info@endeavours.in");

            //message.From = new System.Net.Mail.MailAddress("info@endeavours.in");// Email-ID of Sender  
            //  message.From = new System.Net.Mail.MailAddress("enquiry@weblinkservices.net");// Email-ID of Sender  
            //mm.From = new MailAddress("girish.kulkarni@puneabrasives.com", fromMailID);        

            //// Set the "Reply-To" header to indicate the desired display address
            //mm.ReplyToList.Add(new MailAddress(fromMailID));
            //SmtpClient SmtpMail = new SmtpClient();
            //SmtpMail.Host = "us2.smtp.mailhostbox.com"; // Name or IP-Address of Host used for SMTP transactions  
            //SmtpMail.Port = 25; // Port for sending the mail  
            //SmtpMail.Credentials = new System.Net.NetworkCredential("girish.kulkarni@puneabrasives.com", "Qi#dKZN1"); // Username/password of network, if apply  
            //SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
            //SmtpMail.EnableSsl = false;

            //SmtpMail.ServicePoint.MaxIdleTime = 0;
            //SmtpMail.ServicePoint.SetTcpKeepAlive(true, 2000, 2000);
            //mm.BodyEncoding = Encoding.Default;
            //mm.Priority = MailPriority.High;
            //SmtpMail.Send(mm);
     
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
}