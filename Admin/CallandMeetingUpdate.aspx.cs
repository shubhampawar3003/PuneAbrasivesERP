
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_CallandMeetingUpdate : System.Web.UI.Page
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
                txttodate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                BindDdl_Company();
                if (Request.QueryString["ID"] != null)
                {
                    string ID = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    Load_Record(ID); /*ShowDtlEdit();*/
                    hhd.Value = ID;
                    btnadd.Text = "Update";
                }
                else
                {

                    DivCallUpdate.Visible = false;
                    DivMeetingUpdate.Visible = false;
                    // DivTypeofclient.Visible = false;
                    DivMailID.Visible = false;
                    txtmail.Visible = false;
                    lblfollowupdate.Visible = false;
                    txtfollowupdate.Visible = false;
                    FillddlUsers();
                }
            }
        }
    }
    private void FillddlUsers()
    {
        SqlDataAdapter ad = new SqlDataAdapter("select Username,UserCode from tbl_UserMaster where (Designation='Sales Manager' OR Designation='M.D') and Status=1 and IsDeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlmeetingwithm.DataSource = dt;
            ddlmeetingwithm.DataValueField = "UserCode";
            ddlmeetingwithm.DataTextField = "Username";
            ddlmeetingwithm.DataBind();
            ddlmeetingwithm.Items.Insert(0, "-- Select User Name--");
        }
    }
    protected void Load_Record(string ID)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from tbl_CallandMeetingDetails where ID='" + ID + "' ", Cls_Main.Conn);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            if (!string.IsNullOrEmpty(dt.Rows[0]["followupdate"].ToString()))
            {
                DateTime ffff1 = Convert.ToDateTime(dt.Rows[0]["followupdate"].ToString());
                txtfollowupdate.Text = ffff1.ToString("yyyy-MM-dd");
            }
            txtcompanyname.Text = dt.Rows[0]["CompanyName"].ToString();
            txtccode.Text = dt.Rows[0]["CompanyCode"].ToString();
            //ddlCompanyname.SelectedItem.Text = dt.Rows[0]["CompanyName"].ToString();
            txtownname.Text = dt.Rows[0]["PersonName"].ToString();
            txtcontactno.Text = dt.Rows[0]["ContactNo"].ToString();
            txtaddress.Text = dt.Rows[0]["Address"].ToString();
            txtfeedback.Text = dt.Rows[0]["Feedback"].ToString();
            txtarea.Text = dt.Rows[0]["Area"].ToString();
            ddlmeetingupdate.Text = dt.Rows[0]["UpdateStatus"].ToString();
            txtmail.Text = dt.Rows[0]["clientmail"].ToString();
            /// txtdealdetails.Text = dt.Rows[0]["Dealdetails"].ToString();
            ddlmeetingwithm.SelectedItem.Value = dt.Rows[0]["Meetingwith"].ToString(); ddlmeetingwithm.Enabled = true;
            ddlupdatefor.SelectedItem.Text = dt.Rows[0]["Updatefor"].ToString(); ddlupdatefor.Enabled = true;
            ddlcallstatus.SelectedItem.Text = dt.Rows[0]["UpdateStatus"].ToString(); ddlcallstatus.Enabled = true;
            //ddlproductsize.SelectedItem.Text = dt.Rows[0]["Size"].ToString();
            ddltype.SelectedItem.Text = dt.Rows[0]["Type"].ToString(); ddltype.Enabled = true;
            //ddltypeofclient.SelectedItem.Text = dt.Rows[0]["TypeofClient"].ToString(); ddltypeofclient.Enabled = true;

            if (ddlupdatefor.SelectedItem.Text == "Call")
            {

                DivMeetingUpdate.Visible = false;
                DivCallUpdate.Visible = true;
                DivMailID.Visible = false;
            }
            else
            {
                DivCallUpdate.Visible = false;
                DivMeetingUpdate.Visible = true;
                DivMailID.Visible = true;
            }

            if (ddlcallstatus.SelectedItem.Text == "Follow-up" || ddlmeetingupdate.SelectedItem.Text == "Follow-up")
            {
                lblfollowupdate.Visible = true;
                txtfollowupdate.Visible = true;
            }
            else
            {
                lblfollowupdate.Visible = false;
                txtfollowupdate.Visible = false;
            }
            //if (ddlmeetingupdate.SelectedItem.Text == "Closed")
            //{
            //    DivTypeofclient.Visible = true;
            //}
            //else
            //{
            //    DivTypeofclient.Visible = false;
            //}


        }
    }
    static string ComCode = string.Empty;
    protected void GenerateComCode()
    {
        try
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_CompanyMaster]", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
                ComCode = "WLSPL/CMP-" + (maxid + 1).ToString();
            }
            else
            {
                ComCode = string.Empty;
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    private void BindDdl_Company()
    {
        try
        {
            //string CurrentUserr = Session["UserName"].ToString();
            //DataTable dtUsers = new DataTable();
            //SqlDataAdapter sadusers = new SqlDataAdapter("select * from tbl_CompanyMaster", con);
            //sadusers.Fill(dtUsers);
            //ddlCompanyname.DataValueField = "CompanyCode";
            //ddlCompanyname.DataTextField = "Companyname";
            //ddlCompanyname.DataSource = dtUsers;
            //ddlCompanyname.DataBind();
            //ddlCompanyname.Items.Insert(0, new ListItem("--Select--", "0"));

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
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
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Companyname] from [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and IsDeleted=0";

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

    protected void btnadd_Click1(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select TOP 1 * from tbl_CallandMeetingDetails where CompanyName='" + txtcompanyname.Text.Trim() + "' AND Status=1 AND CreatedBy='" + Session["UserCode"].ToString() + "' ", Cls_Main.Conn);
            sad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                if (Request.QueryString["ID"] != null)
                {

                    string ReID = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    Cls_Main.Conn_Open();
                    SqlCommand CmdDelete = new SqlCommand("Update tbl_CallandMeetingDetails Set Status=@Status,UpdatedBy=@UpdatedBy,UpdatedOn=@UpdatedOn where ID=@ID", Cls_Main.Conn);
                    CmdDelete.Parameters.AddWithValue("@ID", Convert.ToInt32(ReID));
                    CmdDelete.Parameters.AddWithValue("@Status", 2);
                    CmdDelete.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                    CmdDelete.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);

                    CmdDelete.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    SaveDetails();
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please close already opend DSR') ", true);
                }
            }
            else
            {
                SaveDetails();
            }

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    protected void txtcompanyname_TextChanged(object sender, EventArgs e)
    {
        try
        {
            SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM LEFT JOIN tbl_CompanyContactDetails AS CC ON CC.CompanyCode=CM.CompanyCode  WHERE Companyname='" + txtcompanyname.Text + "'", con);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                txtownname.Text = Dt.Rows[0]["Name"].ToString();
                txtcontactno.Text = Dt.Rows[0]["Number"].ToString();
                txtarea.Text = Dt.Rows[0]["Billinglocation"].ToString();
                txtaddress.Text = Dt.Rows[0]["Billingaddress"].ToString();
                txtccode.Text = Dt.Rows[0]["CompanyCode"].ToString();
                txtmail.Text = Dt.Rows[0]["EmailID"].ToString();



            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    protected void ddlupdatefor_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlupdatefor.SelectedItem.Text == "Call")
            {
                DivCallUpdate.Visible = true;
                DivMeetingUpdate.Visible = false;
                //  DivTypeofclient.Visible = false;
                DivMailID.Visible = false;
                txtmail.Visible = false;
                lblfollowupdate.Visible = false;
                txtfollowupdate.Visible = false;
            }

            if (ddlupdatefor.SelectedItem.Text == "Meeting")
            {
                DivMeetingUpdate.Visible = true;
                DivMailID.Visible = true;
                DivCallUpdate.Visible = false;
                txtmail.Visible = true;
                lblfollowupdate.Visible = false;
                txtfollowupdate.Visible = false;
            }

            if (ddlupdatefor.SelectedItem.Text == "--Select--")
            {
                DivCallUpdate.Visible = false;
                DivMeetingUpdate.Visible = false;
                // DivTypeofclient.Visible = false;
                txtmail.Visible = false;
                lblfollowupdate.Visible = false;
                txtfollowupdate.Visible = false;
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    protected void ddlmeetingupdate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //if (ddlmeetingupdate.SelectedItem.Text == "Closed")
            //{
            //    DivTypeofclient.Visible = true;
            //}
            //else
            //{
            //    DivTypeofclient.Visible = false;
            //}

            if (ddlmeetingupdate.SelectedItem.Text == "Follow-up")
            {
                lblfollowupdate.Visible = true;
                txtfollowupdate.Visible = true;
            }
            else
            {
                lblfollowupdate.Visible = false;
                txtfollowupdate.Visible = false;
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    //private void mailsendforCustomer()
    //{
    //    try
    //    {
    //        string fromMailID = Session["adminemail"].ToString().Trim().ToLower();
    //        string mailTo = txtmail.Text.Trim().ToLower();
    //        MailMessage mm = new MailMessage();
    //        mm.From = new MailAddress(fromMailID);

    //        mm.Subject = txtcompanyname.Text + " - UPDATE - From Web Link Services Pvt Ltd.";
    //        mm.To.Add(mailTo);

    //        mm.CC.Add(Session["adminemail"].ToString().Trim().ToLower());

    //        StreamReader reader = new StreamReader(Server.MapPath("~/CommentPage_templet.html"));
    //        string readFile = reader.ReadToEnd();
    //        string myString = "";
    //        myString = readFile;

    //        string multilineText = txtfeedback.Text;
    //        string formattedText = multilineText.Replace("\n", "<br />");

    //        myString = myString.Replace("$Comment$", formattedText);

    //        mm.Body = myString.ToString();

    //        mm.IsBodyHtml = true;
    //        SmtpClient smtp = new SmtpClient();
    //        smtp.Host = ConfigurationManager.AppSettings["Host"]; ;
    //        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
    //        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
    //        NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
    //        NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToString();

    //        smtp.UseDefaultCredentials = false;
    //        smtp.Credentials = NetworkCred;
    //        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
    //        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    //        {
    //            return true;
    //        };
    //        smtp.Send(mm);
    //    }
    //    catch (Exception ex)
    //    {
    //        string errorMsg = "An error occurred : " + ex.Message;
    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
    //    }
    //}

    protected void ddlcallstatus_TextChanged(object sender, EventArgs e)
    {
        if (ddlcallstatus.SelectedItem.Text == "Follow-up")
        {
            lblfollowupdate.Visible = true;
            txtfollowupdate.Visible = true;
        }
        else
        {
            lblfollowupdate.Visible = false;
            txtfollowupdate.Visible = false;
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("Dashboard.aspx");
    }

    public void SaveDetails()
    {
        try
        {

            if (txtcompanyname.Text == "" || txtarea.Text == "")
            {

                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {

                string multilineText = txtfeedback.Text;
                string formattedText = multilineText.Replace("\n", "<br />");
                //string multilineTextt = txtdealdetails.Text;
                // string DealDetailsText = multilineTextt.Replace("\n", "<br />");
                con.Open();
                SqlCommand Cmd = new SqlCommand("SP_CallandMettingDetails", con);
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.Parameters.AddWithValue("@companyname", txtcompanyname.Text.Trim());
                Cmd.Parameters.AddWithValue("@Ownername", txtownname.Text.Trim());
                Cmd.Parameters.AddWithValue("@contactno", txtcontactno.Text.Trim());
                Cmd.Parameters.AddWithValue("@address", txtaddress.Text.Trim());
                Cmd.Parameters.AddWithValue("@feedback", formattedText);
                if (ddlupdatefor.SelectedItem.Text == "Meeting")
                {
                    if (ddlmeetingupdate.SelectedItem.Text == "Follow-up")
                    {
                        Cmd.Parameters.AddWithValue("@Status", 1);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Status", 2);
                    }
                }
                else
                {
                    if (ddlcallstatus.SelectedItem.Text == "Follow-up")
                    {
                        Cmd.Parameters.AddWithValue("@Status", 1);
                    }
                    else
                    {
                        Cmd.Parameters.AddWithValue("@Status", 2);
                    }


                }

                Cmd.Parameters.AddWithValue("@updatefor", ddlupdatefor.SelectedItem.Text.Trim());
                Cmd.Parameters.AddWithValue("@area", txtarea.Text.Trim());
                if (txtfollowupdate.Text != "")
                {
                    //if(hhd.Value==null)
                    //{
                    //Cmd.Parameters.AddWithValue("@followupdate", txtfollowupdate.Text);
                    //Cmd.Parameters.AddWithValue("@followdate", txtfollowupdate.Text);
                    //DateTime date11 = DateTime.ParseExact(Request.Form[txtfollowupdate.UniqueID].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    //Cmd.Parameters.AddWithValue("@followdate", date11);
                    //Cmd.Parameters.AddWithValue("@followupdate", date11);
                    //}
                    //else
                    //{
                    DateTime date11 = Convert.ToDateTime(txtfollowupdate.Text);//DateTime.ParseExact(Request.Form[txtfollowupdate.UniqueID].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    Cmd.Parameters.AddWithValue("@followdate", date11);
                    Cmd.Parameters.AddWithValue("@followupdate", date11);
                    //}

                }
                else
                {
                    Cmd.Parameters.AddWithValue("@followupdate", "");
                    Cmd.Parameters.AddWithValue("@followdate", "");
                }
                Cmd.Parameters.AddWithValue("@Meetingwith", ddlmeetingwithm.SelectedValue);
                Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                Cmd.Parameters.AddWithValue("@Date", txttodate.Text);

                if (ddlupdatefor.SelectedItem.Text == "Call")
                {
                    Cmd.Parameters.AddWithValue("@UpdateStatus", ddlcallstatus.SelectedItem.Text.Trim());
                }
                else if (ddlupdatefor.SelectedItem.Text == "Meeting")
                {
                    Cmd.Parameters.AddWithValue("@UpdateStatus", ddlmeetingupdate.SelectedItem.Text.Trim());
                }
                Cmd.Parameters.AddWithValue("@Type", ddltype.SelectedItem.Text.Trim());
                Cmd.Parameters.AddWithValue("@Typeofclient", DBNull.Value);
                Cmd.Parameters.AddWithValue("@Dealdetails", DBNull.Value);
                Cmd.Parameters.AddWithValue("@clientmail", txtmail.Text.Trim());
                //Audio file save
                if (ViewState["AudioFile"] != null)
                {
                    // Retrieve the file content from ViewState
                    byte[] fileContent = (byte[])ViewState["AudioFile"];

                    // Split the filename to get the name and extension
                    string[] pdffilename = lblfile.Text.Split('.');
                    string pdffilename1 = pdffilename[0];
                    string filenameExt = pdffilename[1];

                    // Generate a unique timestamp
                    string time1 = DateTime.Now.ToString("ddMMyyyyHHmmss");

                    // Construct the full path where the file will be saved
                    string filePath = Server.MapPath("~/AudioFile/") + pdffilename1 + time1 + "." + filenameExt;

                    // Save the file to the specified path
                    System.IO.File.WriteAllBytes(filePath, fileContent);

                    // Add the file path to the command parameters
                    Cmd.Parameters.AddWithValue("@fileName", "AudioFile/" + pdffilename1 + time1 + "." + filenameExt);

                }


                Cmd.Parameters.AddWithValue("@Action", "");

                if (btnadd.Text == "Update")
                {
                    Cmd.Parameters.AddWithValue("@Companycode", txtccode.Text);
                }
                else
                {
                    GenerateComCode();
                    Cmd.Parameters.AddWithValue("@Companycode", ComCode);
                }

                Cmd.ExecuteNonQuery();
                con.Close();
                con.Dispose();
                if (ddlupdatefor.SelectedItem.Text == "Call")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Call Update Successfully..!!') ;window.location='CallandMettingReport.aspx'; ", true);
              
                }
                if (ddlupdatefor.SelectedItem.Text == "Meeting")
                {
                    //Mail Sending
                    if (CheckBox1.Checked == true)
                    {
                        if (txtmail.Text != null && txtmail.Text != "")
                        {
                            mailsendforCustomer();
                        }
                    }


                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Meeting Update Successfully..!!');window.location='CallandMettingReport.aspx'; ", true);

                    Response.Redirect("CallandMettingReport.aspx", true);
                }



            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    private void mailsendforCustomer()
    {
        try
        {
            string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
            string mailTo = txtmail.Text.Trim().ToLower();
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"], fromMailID);

            mm.Subject = txtcompanyname.Text + " - UPDATE - Pune Abrasives Pvt. Ltd.";
            mm.To.Add(mailTo);

            mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
            mm.CC.Add("girish.kulkarni@puneabrasives.com");
            StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
            string readFile = reader.ReadToEnd();
            string myString = "";
            myString = readFile;

            string multilineText = txtfeedback.Text;
            string formattedText = multilineText.Replace("\n", "<br />");

            myString = myString.Replace("$Comment$", formattedText);

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
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    //protected void ddlCompanyname_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM inner join tbl_CompanyContactDetails AS CCD on CM.CompanyCode=CCD.CompanyCode WHERE Companyname='" + txtcompanyname.Text + "'", con);
    //        DataTable Dt = new DataTable();
    //        Da.Fill(Dt);
    //        if (Dt.Rows.Count > 0)
    //        {
    //            txtownname.Text = Dt.Rows[0]["Name"].ToString();
    //            txtcontactno.Text = Dt.Rows[0]["Number"].ToString();
    //            txtarea.Text = Dt.Rows[0]["Area"].ToString();
    //            txtaddress.Text = Dt.Rows[0]["Address"].ToString();
    //            txtccode.Text = Dt.Rows[0]["CompanyCode"].ToString();
    //            txtmail.Text = Dt.Rows[0]["EmailID"].ToString();
    //            btnadd.Text = "Update";
    //            txtownname.Enabled = false;
    //            txtcontactno.Enabled = false;
    //            txtarea.Enabled = false;
    //            txtaddress.Enabled = false;


    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        string errorMsg = "An error occurred : " + ex.Message;
    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
    //    }
    //}

    protected void btnAddCompany_Click(object sender, EventArgs e)
    {
        Response.Redirect("CompanyMaster.aspx");
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CallandMettingReport.aspx");
    }

    //Audio file save in viewstate convert to byte
    protected void uploadfile_Click(object sender, EventArgs e)
    {
        if (AudiofileUpload.HasFile)
        {
            string fileName = Path.GetFileName(AudiofileUpload.PostedFile.FileName);
            byte[] fileContent;

            using (Stream fs = AudiofileUpload.PostedFile.InputStream)
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fileContent = br.ReadBytes((int)fs.Length);
                }
            }
            lblfile.Text = string.Empty;
            ViewState["AudioFile"] = null;
            ViewState["AudioFile"] = fileContent;
            lblfile.Text = fileName;
        }

    }
}


