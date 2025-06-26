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

public partial class Sales_getupdationCompany : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["salesname"] == null || Session["salesempcode"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                ddlhistorytype.Text = "All"; DdlEmpBind(); UpdateHistorymsg = string.Empty;
                UpdateTMEHistorymsg = string.Empty; dtfullhistory.Clear(); DdlSalesBind();
                Gvbind(); BindDdl_Username(); Getmanger();

            }
        }
    }

    private void BindDdl_Username()
    {
        try
        {
            #region This Code is For Sale Module 
            string CurrentUser = Session["salesempcode"].ToString();
            // Create a DataTable to store the distinct user names
            DataTable dtDistinctUsers = new DataTable();
            dtDistinctUsers.Columns.Add("empcode", typeof(string));
            dtDistinctUsers.Columns.Add("name", typeof(string));
            // Create a list to keep track of selected user names to prevent duplicates
            List<string> selectedUserNames = new List<string>();
            DataRow currentUserRow = dtDistinctUsers.NewRow();
            currentUserRow["empcode"] = CurrentUser;
            //currentUserRow["name"] = "Current User";
            currentUserRow["name"] = Session["salesname"].ToString();
            dtDistinctUsers.Rows.Add(currentUserRow);
            con.Open();
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[employees] WHERE status = '1' AND isdeleted = '0' AND TL_Manager = @CurrentUser", con))
            {
                cmd.Parameters.AddWithValue("@CurrentUser", CurrentUser);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string empcode = reader["empcode"].ToString();
                    string name = reader["name"].ToString();

                    // Check if the name is not already selected
                    if (!selectedUserNames.Contains(name))
                    {
                        selectedUserNames.Add(name);
                        dtDistinctUsers.Rows.Add(empcode, name);
                    }
                }
                reader.Close();
            }
            ddlTeamuser.DataValueField = "empcode";
            ddlTeamuser.DataTextField = "name";
            ddlTeamuser.DataSource = dtDistinctUsers;
            ddlTeamuser.DataBind();
            //ddlTeamuser.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
            #endregion

            #region This Code is For Admin & SubAdmin Module 
            //string CurrentUserr = Session["adminempcode"].ToString();
            //DataTable dtUsers = new DataTable();
            //SqlDataAdapter sadusers = new SqlDataAdapter("SELECT * FROM [dbo].[employees] where status='1' AND isdeleted='0'", con);
            //sadusers.Fill(dtUsers);
            //ddlTeamuser.DataValueField = "empcode";
            //ddlTeamuser.DataTextField = "name";
            //ddlTeamuser.DataSource = dtUsers;
            //ddlTeamuser.DataBind();
            //ddlTeamuser.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select SM --", "0"));
            #endregion       
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    private void BindDdl_TransferTo()
    {
        try
        {
            #region This Code is For Sale Module 
            //string CurrentUser = Session["adminempcode"].ToString();
            //// Create a DataTable to store the distinct user names
            //DataTable dtDistinctUsers = new DataTable();
            //dtDistinctUsers.Columns.Add("empcode", typeof(string));
            //dtDistinctUsers.Columns.Add("name", typeof(string));
            //// Create a list to keep track of selected user names to prevent duplicates
            //List<string> selectedUserNames = new List<string>();
            //DataRow currentUserRow = dtDistinctUsers.NewRow();
            //currentUserRow["empcode"] = CurrentUser;
            ////currentUserRow["name"] = "Current User";
            //currentUserRow["name"] = Session["adminname"].ToString();
            //dtDistinctUsers.Rows.Add(currentUserRow);
            //con.Open();
            //using (SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[employees] WHERE status = '1' AND isdeleted = '0' AND TL_Manager = @CurrentUser", con))
            //{
            //    cmd.Parameters.AddWithValue("@CurrentUser", CurrentUser);
            //    SqlDataReader reader = cmd.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        string empcode = reader["empcode"].ToString();
            //        string name = reader["name"].ToString();

            //        // Check if the name is not already selected
            //        if (!selectedUserNames.Contains(name))
            //        {
            //            selectedUserNames.Add(name);
            //            dtDistinctUsers.Rows.Add(empcode, name);
            //        }
            //    }
            //    reader.Close();
            //}
            //ddlTeamuser.DataValueField = "empcode";
            //ddlTeamuser.DataTextField = "name";
            //ddlTeamuser.DataSource = dtDistinctUsers;
            //ddlTeamuser.DataBind();
            ////ddlTeamuser.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
            #endregion

            #region This Code is For Admin & SubAdmin Module 
            string CurrentUserr = Session["adminempcode"].ToString();
            DataTable dtUsers = new DataTable();
            SqlDataAdapter sadusers = new SqlDataAdapter("SELECT * FROM [dbo].[employees] where status='1' AND isdeleted='0'", con);
            sadusers.Fill(dtUsers);
            ddlTeamuser.DataValueField = "empcode";
            ddlTeamuser.DataTextField = "name";
            ddlTeamuser.DataSource = dtUsers;
            ddlTeamuser.DataBind();
            ddlTeamuser.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select SM --", "0"));
            #endregion       
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    private void Gvbind()
    {
        SqlDataAdapter ad = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand("[stswlspl].SP_GetNotUpdatedCompany", con);
        cmd.Parameters.Add(new SqlParameter("@DaysFilter", ddldaysfilter.SelectedValue));
        cmd.CommandType = CommandType.StoredProcedure;
        ad.SelectCommand = cmd;
        ad.SelectCommand.CommandTimeout = 60;
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvCompany.DataSource = dt;
            GvCompany.DataBind();
            lblnodatafoundComp.Visible = false;
        }
        else
        {
            GvCompany.DataSource = null;
            GvCompany.DataBind();
            lblnodatafoundComp.Text = "No TBRO/Remainder Data Found !! ";
            lblnodatafoundComp.Visible = true;
            lblnodatafoundComp.ForeColor = System.Drawing.Color.Red;
        }
       
    }

    protected void ddldaysfilter_TextChanged(object sender, EventArgs e)
    {
        Gvbind();
        Getmanger();
    }

    private void DdlEmpBind()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT [id],[empcode],[name] FROM [employees] where [status]=1 and [isdeleted]=0 order by id desc", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlbdepopup.DataSource = dt;
            ddlbdepopup.DataValueField = "empcode";
            ddlbdepopup.DataTextField = "name";
            ddlbdepopup.DataBind();
            ddlbdepopup.Items.Insert(0, "Select");

        }
    }

    private void DdlSalesBind()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT [id],[empcode],[name] FROM [employees] where [status]=1 and [isdeleted]=0 and [role]='Sales' order by id desc", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            //ddlsalesMainfilter.DataSource = dt;
            //ddlsalesMainfilter.DataValueField = "empcode";
            //ddlsalesMainfilter.DataTextField = "name";
            //ddlsalesMainfilter.DataBind();
            //ddlsalesMainfilter.Items.Insert(0, "All");

            ddlregbypopup.DataSource = dt;
            ddlregbypopup.DataValueField = "empcode";
            ddlregbypopup.DataTextField = "name";
            ddlregbypopup.DataBind();
            ddlregbypopup.Items.Insert(0, "Select");
        }
    }

    protected void GvCompany_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ViewState["CompRowId"] = e.CommandArgument.ToString();
        //if (e.CommandName == "RowEdit")
        //{
        //    //string empcode= GetSessionName(e.CommandArgument.ToString());
        //    // if (empcode == Session["adminempcode"].ToString())
        //    // {
        //    ViewState["id"] = e.CommandArgument.ToString();
        //    Response.Redirect("AddCompany.aspx?code=" + encrypt(e.CommandArgument.ToString()));
        //    //}
        //    //else
        //    //{
        //    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('You do not have permission to edit this !!');", true);
        //    //}  
        //}
        if (e.CommandName == "companyname")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                GetCompanyDataPopup(e.CommandArgument.ToString());
                //GetCompanyDataBDEPopup(e.CommandArgument.ToString());
                this.modelprofile.Show();
                this.ModalPopupHistory.Hide();
            }
        }
    }

    private void GetCompanyDataPopup(string id)
    {
        string query1 = string.Empty;
        query1 = "SELECT top 1 A.id, A.[ccode],A.[cname],A.[oname],A.[email],A.[mobile],A.[visitingcard],A.[type],A.[address],format(A.[visitdate],'dd-MMM-yyyy') as visitdate,A.[website],format(A.[regdate],'dd-MMM-yyyy hh:mm tt') as [regdate],A.[sessionname],A.[BDE],B.name,B.email as Empemail FROM [Company] A join employees B on A.sessionname=B.empcode where A.ccode='" + id + "' and A.status=0 and A.isdeleted=0 order by A.id desc ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblccode.Text = dt.Rows[0]["ccode"].ToString();
            lblcname.Text = dt.Rows[0]["cname"].ToString();
            lbloname.Text = dt.Rows[0]["oname"].ToString();
            lblemail.Text = dt.Rows[0]["email"].ToString();
            lblmobile.Text = dt.Rows[0]["mobile"].ToString();
            hrefmob.HRef = "tel:" + dt.Rows[0]["mobile"].ToString();
            if (!string.IsNullOrEmpty(dt.Rows[0]["visitingcard"].ToString()))
            {
                Imgvisitingcard.ImageUrl = "../" + dt.Rows[0]["visitingcard"].ToString();
            }
            else
            {
                Imgvisitingcard.ImageUrl = "../img/logoWeb.png";
            }

            lblvisitdate.Text = dt.Rows[0]["visitdate"].ToString();
            if (dt.Rows[0]["type"].ToString().ToLower() == "paid")
            {
                lblclienttype.Text = "Paid";
                lblclienttype.ForeColor = System.Drawing.Color.Green;
            }
            if (dt.Rows[0]["type"].ToString().ToLower() == "unpaid")
            {
                lblclienttype.Text = "Unpaid";
                lblclienttype.ForeColor = System.Drawing.Color.Red;
            }

            string websitelink1 = dt.Rows[0]["website"].ToString();
            if (websitelink1.Contains("http://"))
            {
                hrefwebsite.HRef = websitelink1;
            }
            if (websitelink1.Contains("https://"))
            {
                hrefwebsite.HRef = websitelink1;
            }
            if (!websitelink1.Contains("https://") && !websitelink1.Contains("http://"))
            {
                hrefwebsite.HRef = "http://" + websitelink1;
            }
            lblwebsite.Text = dt.Rows[0]["website"].ToString();
            lblRegdate.Text = dt.Rows[0]["regdate"].ToString();
            lblregBy.Text = dt.Rows[0]["name"].ToString();
            ddlregbypopup.SelectedItem.Text = dt.Rows[0]["name"].ToString();
            lbladdress.Text = dt.Rows[0]["address"].ToString();
            //dt.Rows[0]["sessionname"].ToString();// Current Sales code
            ViewState["CurrentSalesEmail"] = dt.Rows[0]["Empemail"].ToString();// Current Sales Email
            GetCompanyDataBDEPopup(dt.Rows[0]["BDE"].ToString());
        }
    }

    private void GetCompanyDataBDEPopup(string id)
    {
        string query1 = string.Empty;
        query1 = "SELECT name as BDE,email as TMEemail,empcode FROM employees  where empcode='" + id + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblbde.Text = dt.Rows[0]["BDE"].ToString();
            ddlbdepopup.SelectedItem.Text = dt.Rows[0]["BDE"].ToString();
            BdeCode.Value = dt.Rows[0]["empcode"].ToString();
            ViewState["CurrentTMEEmail"] = dt.Rows[0]["TMEemail"].ToString();
        }
    }

    //private void GetBDEemail(string BdeCode)
    //{
    //    string query1 = string.Empty;
    //    query1 = "SELECT [email] FROM [employees] where [empcode]='" + BdeCode + "' ";
    //    SqlDataAdapter ad = new SqlDataAdapter(query1, con);
    //    DataTable dt = new DataTable();
    //    ad.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        BdeMailId.Value = dt.Rows[0]["email"].ToString();
    //    }
    //}

    protected void GvCompany_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvCompany.PageIndex = e.NewPageIndex;
        Gvbind();
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

    #region Comment
    protected void btnComment_Click(object sender, EventArgs e)
    {
        SqlCommand cmd = new SqlCommand("SP_CommentHistory", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "Insert");
        cmd.Parameters.AddWithValue("@sessionname", Session["adminempcode"].ToString());
        cmd.Parameters.AddWithValue("@ccode", lblccode.Text);
        cmd.Parameters.AddWithValue("@frommail", Session["adminemail"].ToString().Trim().ToLower());
        cmd.Parameters.AddWithValue("@adminmail", ConfigurationManager.AppSettings["AdminMailBcc"]);
        cmd.Parameters.AddWithValue("@clientmail", lblemail.Text.Trim().ToLower());
        //GetBDEemail(BdeCode.Value);
        if (!string.IsNullOrEmpty(BdeMailId.Value))
        {
            cmd.Parameters.AddWithValue("@bdemail", BdeMailId.Value.Trim().ToLower());
        }
        else
        {
            cmd.Parameters.AddWithValue("@bdemail", DBNull.Value);
        }

        cmd.Parameters.AddWithValue("@additionalmail", txtaddemail.Text.Trim().ToLower());
        cmd.Parameters.AddWithValue("@message", txtcomment.Text);

        int a = 0;
        cmd.Connection.Open();
        a = cmd.ExecuteNonQuery();
        cmd.Connection.Close();
        if (a > 0)
        {
            mailsendforCustomer(); txtcomment.Text = string.Empty; txtaddemail.Text = string.Empty; Gvbind();
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('Your enquiry submitted successfully!');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('There are some problem, Plese try again !!');", true);
        }
    }
    private void mailsendforCustomer()
    {
        string fromMailID = Session["adminemail"].ToString().Trim().ToLower();
        string mailTo = lblemail.Text.Trim().ToLower();
        MailMessage mm = new MailMessage();
        mm.From = new MailAddress(fromMailID);

        mm.Subject = lblcname.Text + " - UPDATE - From Web Link Services Pvt Ltd.";
        mm.To.Add(mailTo);
        //if (!string.IsNullOrEmpty(BdeMailId.Value))
        //{
        //    mm.To.Add(BdeMailId.Value.Trim().ToLower());
        //}
        mm.CC.Add(Session["adminemail"].ToString().Trim().ToLower());

        if (!string.IsNullOrEmpty(ViewState["CurrentSalesEmail"].ToString()))
        {
            mm.CC.Add(ViewState["CurrentSalesEmail"].ToString().Trim().ToLower());
        }

        if (!string.IsNullOrEmpty(ViewState["CurrentTMEEmail"].ToString()))
        {
            if (ViewState["CurrentSalesEmail"].ToString().Trim().ToLower() != ViewState["CurrentTMEEmail"].ToString().Trim().ToLower())
            {
                mm.CC.Add(ViewState["CurrentTMEEmail"].ToString().Trim().ToLower());
            }
        }

        mm.Bcc.Add(ConfigurationManager.AppSettings["AdminMailBcc"]);
        if (!string.IsNullOrEmpty(txtaddemail.Text))
        {
            mm.CC.Add(txtaddemail.Text.Trim().ToLower());
        }
        StreamReader reader = new StreamReader(Server.MapPath("~/CommentPage_templet.html"));
        string readFile = reader.ReadToEnd();
        string myString = "";
        myString = readFile;

        //string DomainName = ConfigurationManager.AppSettings["DomainName"];
        myString = myString.Replace("$Comment$", txtcomment.Text);

        mm.Body = myString.ToString();

        mm.IsBodyHtml = true;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = ConfigurationManager.AppSettings["Host"];
        //smtp.Host = "smtp.gmail.com";
        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
        //smtp.EnableSsl = true;
        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
        NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
        NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToLower();

        smtp.UseDefaultCredentials = false;
        smtp.Credentials = NetworkCred;
        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        smtp.Send(mm);
    }
    #endregion Comment


    #region History
    protected void btnhistory_Click(object sender, EventArgs e)
    {
        dtfullhistory.Clear();
        ddlhistorytype.Text = "All";
        GetCompanyHistoryPopup(lblccode.Text);
        this.ModalPopupHistory.Show();
        this.modelprofile.Hide();
        //MergeHistoryDataBindRpt();
    }

    protected void GetCompanyHistoryPopup(string Compcode)
    {
        string query1 = string.Empty;
        query1 = "SELECT [ccode],[cname],[email] FROM [Company] where ccode='" + Compcode + "' and [status]=0 and [isdeleted]=0 order by id desc ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblccodeHistory.Text = dt.Rows[0]["ccode"].ToString();
            lblcnameHistory.Text = dt.Rows[0]["cname"].ToString();
            lblemailHistory.Text = dt.Rows[0]["email"].ToString();
            GetCompanyHistoryComments(dt.Rows[0]["ccode"].ToString());
            //GetCompanyDetailHistory(dt.Rows[0]["ccode"].ToString());
            //GetRemainderHistory(dt.Rows[0]["ccode"].ToString());     
        }
    }


    static DataTable dtfullhistory = new DataTable();
    DataView dvfullhistory;
    protected void GetCompanyHistoryComments(string Compcode)
    {
        string query1 = string.Empty;
        query1 = @"SELECT [commentdatetime],[typeoftbl],[message],[name] FROM (SELECT format(A.[commentdatetime],'dd-MMM-yyyy hh:mm tt') as [commentdatetime],A.[typeoftbl],A.[message],B.[name] FROM [CommentHistory] A left join [employees] B ON A.[sessionname]=B.[empcode] where A.ccode='" + Compcode + "' UNION SELECT format(A.[updateddatetime],'dd-MMM-yyyy hh:mm tt') as [commentdatetime],A.[typeoftbl],A.[message],B.[name] FROM [CompanyHistory] A left join [employees] B ON A.[sessionname]=B.[empcode] where A.ccode= '" + Compcode + "' UNION SELECT format(A.[setdatetime],'dd-MMM-yyyy hh:mm tt') as [commentdatetime],A.[typeoftbl],A.[remark] as message,B.[name] FROM [RemainderData] A left join [employees] B ON A.[sessionname]=B.[empcode] where A.ccode= '" + Compcode + "') AS T order by convert(datetime, [commentdatetime]) desc";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        ad.Fill(dtfullhistory);
        BindHistoryData();
    }

    protected void BindHistoryData()
    {
        if (ddlhistorytype.Text != "All")
        {
            if (dtfullhistory.Rows.Count > 0)
            {
                dvfullhistory = new DataView(dtfullhistory);
                dvfullhistory.RowFilter = "typeoftbl='" + ddlhistorytype.Text + "'";
                RptComments.DataSource = dvfullhistory;
                RptComments.DataBind();
            }
            else
            {
                RptComments.DataSource = null;
                RptComments.DataBind();
            }
        }
        else
        {
            if (dtfullhistory.Rows.Count > 0)
            {
                RptComments.DataSource = dtfullhistory;
                RptComments.DataBind();
            }
            else
            {
                RptComments.DataSource = null;
                RptComments.DataBind();
            }
        }
    }

    #endregion History

    protected void btnShowComDetail_Click(object sender, EventArgs e)
    {
        GetCompanyDataPopup(ViewState["CompRowId"].ToString()); GetCompanyDataBDEPopup(ViewState["CompRowId"].ToString());
        this.modelprofile.Show();
        this.ModalPopupHistory.Hide();

    }

    #region Reminder
    protected void GetCompanyDataReminderPopup(string Compcode)
    {
        string query1 = string.Empty;
        query1 = "SELECT [ccode],[cname],[email],[sessionname] FROM [Company] where ccode='" + Compcode + "' and status=0 ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblccodeRem.Text = dt.Rows[0]["ccode"].ToString();
            lblCnameRem.Text = dt.Rows[0]["cname"].ToString();
            //lblemailHistory.Text = dt.Rows[0]["email"].ToString();
        }
    }
    protected void btnreminder_Click(object sender, EventArgs e)
    {
        btnreminder.Enabled = false;
        GetCompanyDataReminderPopup(lblccode.Text);
        this.modelprofile.Hide();
        this.ModalPopupHistory.Hide();
        ModalPopupRemainder.Show();
        btnreminder.Enabled = true;
    }

    protected void btnsaveRemainder_Click(object sender, EventArgs e)
    {
        SqlCommand cmd = new SqlCommand("SP_RemainderData", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "Insert");

        cmd.Parameters.AddWithValue("@sessionname", Session["adminempcode"].ToString());
        cmd.Parameters.AddWithValue("@ccode", lblccodeRem.Text);
        cmd.Parameters.AddWithValue("@cname", lblCnameRem.Text);
        cmd.Parameters.AddWithValue("@title", txttitleRem.Text);
        //DateTime oDate = DateTime.Parse(txtdateRem.Text);
        cmd.Parameters.AddWithValue("@dateofreminder", txtdateRem.Text);
        cmd.Parameters.AddWithValue("@remark", txtremarkRem.Text);

        int a = 0;
        cmd.Connection.Open();
        a = cmd.ExecuteNonQuery();
        cmd.Connection.Close();
        if (a > 0)
        {
            txttitleRem.Text = string.Empty; txtdateRem.Text = string.Empty; txtremarkRem.Text = string.Empty;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('Remainder Saved Successfully!');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('There are some problem, Plese try again !!');", true);
        }
    }
    #endregion Reminder


    // Filter History popup data
    protected void ddlhistorytype_TextChanged(object sender, EventArgs e)
    {
        //GetCompanyHistoryPopup(lblccode.Text);
        BindHistoryData();
        this.ModalPopupHistory.Show();
        this.modelprofile.Hide();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("AllCompanylist.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCompanyList(string prefixText, int count)
    {
        return AutoFillCompanyName(prefixText);
    }

    public static List<string> AutoFillCompanyName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [cname] from [Company] where " + "cname like @Search + '%' and status=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["cname"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCompanyOwnerList(string prefixText, int count)
    {
        return AutoFillCompanyOwnerName(prefixText);
    }

    public static List<string> AutoFillCompanyOwnerName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [oname] from [Company] where " + "oname like @Search + '%' and status=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["oname"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void btnSalesChange_Click(object sender, EventArgs e)
    {
        if (ddlregbypopup.SelectedItem.Text.Trim().ToLower() != lblregBy.Text.Trim().ToLower())
        {
            btnSalesChange.Enabled = false;
            //string ab = ddlregbypopup.SelectedValue;
            //Change Sales in DB // Mail to new Sales, Old sales & Admin // Create a History // For perticular Company
            ViewState["NewSalesEmail"] = GetMailIdOfEmpl(ddlregbypopup.SelectedValue);
            //ViewState["NewSalesEmail"] = GetMailIdOfEmpl("sbwankhade96@gmail.com");
            UpdateSalesDB(ddlregbypopup.SelectedValue.ToString(), lblccode.Text);
            btnSalesChange.Enabled = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('Sales Person Changed Successfully.');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Please select different Sales Person !!');", true);
        }
    }

    protected void btnBdeChange_Click(object sender, EventArgs e)
    {
        //string ab = ddlbdepopup.SelectedValue;
        if (ddlbdepopup.SelectedItem.Text.Trim().ToLower() != lblbde.Text.Trim().ToLower())
        {
            btnBdeChange.Enabled = false;
            //string ab = ddlbdepopup.SelectedValue;
            //Change Sales in DB // Mail to new Sales, Old sales & Admin // Create a History // For perticular Company
            //ViewState["CurrentTMEEmail"] = GetMailIdOfEmpl(ddlbdepopup.SelectedValue);
            ViewState["NewTMEEmail"] = GetMailIdOfEmpl(ddlbdepopup.SelectedValue);
            UpdateTMEDB(ddlbdepopup.SelectedValue.ToString(), lblccode.Text);
            btnBdeChange.Enabled = true;
            ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('BDE/TME Person Changed Successfully.');", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Please select different BDE/TME Person !!');", true);
        }
    }

    #region Sales Person Change
    static string UpdateHistorymsg = string.Empty;
    protected void CreateSalesHistory()
    {
        if (ddlregbypopup.SelectedItem.Text.Trim().ToLower() != lblregBy.Text.Trim().ToLower())
        {
            UpdateHistorymsg = "Sales Person has been changed from '" + lblregBy.Text + "' to '" + ddlregbypopup.SelectedItem.Text + "' ";
        }
        if (!string.IsNullOrEmpty(UpdateHistorymsg))
        {
            SqlCommand cmd = new SqlCommand("SP_CompanyHistory", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@sessionname", Session["adminempcode"].ToString());
            cmd.Parameters.AddWithValue("@ccode", lblccode.Text);
            cmd.Parameters.AddWithValue("@message", UpdateHistorymsg);
            cmd.Connection.Open();
            int a = 0;
            a = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

    }

    protected void UpdateSalesDB(string Salescode, string Compcode)
    {
        try
        {
            using (SqlCommand cmm = new SqlCommand())
            {
                cmm.Connection = con;
                cmm.CommandType = CommandType.Text;
                cmm.CommandText = "Update [Company] set [sessionname]='" + Salescode + "'  where [ccode]='" + Compcode + "'";
                cmm.Connection.Open();
                int a = 0;
                a = cmm.ExecuteNonQuery();
                cmm.Connection.Close();
                if (a > 0)
                {
                    //CreateSalesHistory(); mailsendforSalesChange();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Not Updated !!');", true);
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    private void mailsendforSalesChange()
    {
        string fromMailID = Session["adminemail"].ToString().Trim().ToLower();
        string mailTo = string.Empty;
        if (!string.IsNullOrEmpty(ViewState["CurrentSalesEmail"].ToString()))
        {
            mailTo = ViewState["CurrentSalesEmail"].ToString().Trim().ToLower();// Sales both
          
        }
        else
        {
            mailTo = ConfigurationManager.AppSettings["AdminMailBcc"];
        }
        //mailTo = "sbwankhade96@gmail.com";
        MailMessage mm = new MailMessage();
        mm.From = new MailAddress(fromMailID);

        mm.Subject = lblcname.Text + " - Sales Person Changed";
        mm.To.Add(mailTo);
        string mailCC = string.Empty;
        if (!string.IsNullOrEmpty(ViewState["NewSalesEmail"].ToString()))
        {
            mailCC = ViewState["NewSalesEmail"].ToString().Trim().ToLower();// Sales both
            mm.CC.Add(mailCC);
        }
        if (!string.IsNullOrEmpty(ViewState["CurrentTMEEmail"].ToString()))
        {
            if (ViewState["CurrentSalesEmail"].ToString().Trim().ToLower() != ViewState["CurrentTMEEmail"].ToString().Trim().ToLower())
            {
                mm.CC.Add(ViewState["CurrentTMEEmail"].ToString().Trim().ToLower());
            }
        }

        mm.Bcc.Add(ConfigurationManager.AppSettings["AdminMailBcc"]);

        StreamReader reader = new StreamReader(Server.MapPath("~/CommentPage_templet.html"));
        string readFile = reader.ReadToEnd();
        string myString = "";
        myString = readFile;

        //string DomainName = ConfigurationManager.AppSettings["DomainName"];
        myString = myString.Replace("$Comment$", lblcname.Text + "'s " + UpdateHistorymsg + " by the " + Session["adminname"].ToString());

        mm.Body = myString.ToString();

        mm.IsBodyHtml = true;
        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = ConfigurationManager.AppSettings["Host"];
        //smtp.Host = "smtp.gmail.com";
        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
        //smtp.EnableSsl = true;
        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
        NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
        NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToLower();

        smtp.UseDefaultCredentials = false;
        smtp.Credentials = NetworkCred;
        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        smtp.Send(mm);
    }
    #endregion Sales Person Change


    #region TME Person Change
    static string UpdateTMEHistorymsg = string.Empty;
    protected void CreateTMEHistory()
    {
        if (ddlbdepopup.SelectedItem.Text.Trim().ToLower() != lblbde.Text.Trim().ToLower())
        {
            UpdateTMEHistorymsg = "BDE/TME Person has been changed from '" + lblbde.Text + "' to '" + ddlbdepopup.SelectedItem.Text + "' ";
        }
        if (!string.IsNullOrEmpty(UpdateTMEHistorymsg))
        {
            SqlCommand cmd = new SqlCommand("SP_CompanyHistory", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@sessionname", Session["adminempcode"].ToString());
            cmd.Parameters.AddWithValue("@ccode", lblccode.Text);
            cmd.Parameters.AddWithValue("@message", UpdateTMEHistorymsg);
            cmd.Connection.Open();
            int a = 0;
            a = cmd.ExecuteNonQuery();
            cmd.Connection.Close();
        }

    }

    protected void UpdateTMEDB(string TMEcode, string Compcode)
    {
        try
        {
            using (SqlCommand cmm = new SqlCommand())
            {
                cmm.Connection = con;
                cmm.CommandType = CommandType.Text;
                cmm.CommandText = "Update [Company] set [BDE]='" + TMEcode + "'  where [ccode]='" + Compcode + "'";
                cmm.Connection.Open();
                int a = 0;
                a = cmm.ExecuteNonQuery();
                cmm.Connection.Close();
                if (a > 0)
                {
                    CreateTMEHistory(); mailsendforTMEChange();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Not Updated !!');", true);
                }
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }

    private void mailsendforTMEChange()
    {
        string fromMailID = Session["adminemail"].ToString().Trim().ToLower();
        string mailTo = string.Empty;
        if (!string.IsNullOrEmpty(ViewState["CurrentTMEEmail"].ToString()))
        {
            mailTo = ViewState["CurrentTMEEmail"].ToString().Trim().ToLower();// Sales both
        }
        else
        {
            mailTo = ConfigurationManager.AppSettings["AdminMailBcc"];
        }
        //mailTo = "sbwankhade96@gmail.com";
        MailMessage mm = new MailMessage();
        mm.From = new MailAddress(fromMailID);

        mm.Subject = lblcname.Text + " - BDE/TME Person Changed";
        mm.To.Add(mailTo);
        string mailCC = string.Empty;
        if (!string.IsNullOrEmpty(ViewState["NewTMEEmail"].ToString()))
        {
            mailCC = ViewState["NewTMEEmail"].ToString().Trim().ToLower();// Sales both
            mm.CC.Add(mailCC);
        }

        if (!string.IsNullOrEmpty(ViewState["CurrentSalesEmail"].ToString()))
        {
            mm.CC.Add(ViewState["CurrentSalesEmail"].ToString().Trim().ToLower());
        }

        if (!string.IsNullOrEmpty(ViewState["CurrentTMEEmail"].ToString()))
        {
            if (ViewState["CurrentSalesEmail"].ToString().Trim().ToLower() != ViewState["CurrentTMEEmail"].ToString().Trim().ToLower())
            {
                mm.CC.Add(ViewState["CurrentTMEEmail"].ToString().Trim().ToLower());
            }
        }
        mm.Bcc.Add(ConfigurationManager.AppSettings["AdminMailBcc"]);

        StreamReader reader = new StreamReader(Server.MapPath("~/CommentPage_templet.html"));
        string readFile = reader.ReadToEnd();
        string myString = "";
        myString = readFile;

        //string DomainName = ConfigurationManager.AppSettings["DomainName"];
        myString = myString.Replace("$Comment$", lblcname.Text + "'s " + UpdateTMEHistorymsg + " by the " + Session["adminname"].ToString());

        mm.Body = myString.ToString();

        mm.IsBodyHtml = true;
        SmtpClient smtp = new SmtpClient();
        smtp.Host = ConfigurationManager.AppSettings["Host"];
        //smtp.Host = "smtp.gmail.com";
        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
        //smtp.EnableSsl = true;
        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
        NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
        NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToLower();

        smtp.UseDefaultCredentials = false;
        smtp.Credentials = NetworkCred;
        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        smtp.Send(mm);
    }
    #endregion TME Person Change

    private string GetMailIdOfEmpl(string Empcode)
    {
        string query1 = "SELECT [email] FROM [employees] where [empcode]='" + Empcode + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        string email = string.Empty;
        if (dt.Rows.Count > 0)
        {
            email = dt.Rows[0]["email"].ToString();
        }
        return email;
    }


    protected void ddluserstatus_TextChanged(object sender, EventArgs e)
    {

    }
    protected void chkRow_CheckedChanged(object sender, EventArgs e)
    {
        Dvapprove.Visible = true;
        int selectedCount = 0;
        foreach (GridViewRow row in GvCompany.Rows)
        {
            CheckBox selectedchk = (CheckBox)row.FindControl("chkRow");
            DropDownList ddlmanager = (DropDownList)row.FindControl("ddlTransferTo");
            if (selectedchk != null && selectedchk.Checked)
            {
              
                ddlmanager.Enabled = false;
                selectedCount++;
                row.BackColor = System.Drawing.ColorTranslator.FromHtml("#FF9933");
    
            }
            else
            {
                ddlmanager.Enabled = true;
                row.BackColor = System.Drawing.Color.White;
        


            }
        }
        string message = Convert.ToString(selectedCount);
        btntransfer.Text = " Total Transfer=" + " " + message;
        btntransfer.Visible = true;
    }

    protected void Getmanger()
    {
        GridView gridView = GvCompany;
        foreach (GridViewRow row in gridView.Rows)
        {
            DropDownList ddlTransferTo = (DropDownList)row.FindControl("ddlTransferTo");

            if (ddlTransferTo != null)
            {

                GetSalesmanager(ddlTransferTo);
            }
            //else
            //{
            //    Salesmanger((ddlTransferTo));
            //}
        }
    }


    public void GetSalesmanager(DropDownList ddlTransferTo)
    {
        SqlDataAdapter ad = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand("[stswlspl].SP_NotUpdatedcompanyForadmin", con);
        cmd.Parameters.Add(new SqlParameter("@ACtion", "Getsalesmanager"));
        cmd.CommandType = CommandType.StoredProcedure;
        ad.SelectCommand = cmd;
        ad.SelectCommand.CommandTimeout = 60;
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlTransferTo.DataSource = dt;
            ddlTransferTo.DataValueField = "Manager";
            ddlTransferTo.DataTextField = "Manager";
            ddlTransferTo.DataBind();
            ddlTransferTo.Items.Insert(0, "--Select--");
        }
    }

    protected void btntransfer_Click(object sender, EventArgs e)
    {
     
        
    }

    protected void ddlTransferTo_SelectedIndexChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in GvCompany.Rows)
        {
            CheckBox selectedchk = (CheckBox)row.FindControl("chkRow");
            DropDownList ddl = (DropDownList)row.FindControl("ddlTransferTo");

            if (ddl.SelectedItem.Text != "--Select--")
            {
                selectedchk.Visible = true;
                selectedchk.Checked = true;
            }
            else if (ddl.SelectedItem.Text == "--Select--")
            {
                selectedchk.Visible = false;
                selectedchk.Checked = false;
            }
        }
    }


    public void BulkUpdate(string Salescode, string Compcode)
    {
      
            try
            {
                using (SqlCommand cmm = new SqlCommand())
                {
                    cmm.Connection = con;
                    cmm.CommandType = CommandType.Text;
                    cmm.CommandText = "Update [Company] set [sessionname]='" + Salescode + "'  where [ccode]='" + Compcode + "'";
                    cmm.Connection.Open();
                    int a = 0;
                    a = cmm.ExecuteNonQuery();
                    cmm.Connection.Close();
                    if (a > 0)
                    {
                        CreateSalesHistory(); mailsendforSalesChange();
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "Alert", "alert('Not Updated !!');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        
    }



    public void Salesmanger  (DropDownList ddlTransferTo)
    {
        SqlDataAdapter ad = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand("[stswlspl].SP_NotUpdatedcompanyForadmin", con);
        cmd.Parameters.Add(new SqlParameter("@ACtion", "Getsalesmanager"));
        cmd.CommandType = CommandType.StoredProcedure;
        ad.SelectCommand = cmd;
        ad.SelectCommand.CommandTimeout = 60;
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlTransferTo.DataSource = dt;
            ddlTransferTo.DataValueField = "Manager";
            ddlTransferTo.DataTextField = "Manager";
            ddlTransferTo.DataBind();
            ddlTransferTo.Items.Insert(0, "--Select--");
        }
    }
}