
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


public partial class Admin_EnquiryFile : System.Web.UI.Page
{
    string id;
    string Fpath;
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserCode"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (Request.QueryString["Fileid1"] != null)
            {
                string id = objcls.Decrypt(Request.QueryString["Fileid1"].ToString());
                Fpath = "filepath1";
                Display(id, Fpath);
                //string id = id1;

            }
            if (Request.QueryString["Fileid2"] != null)
            {
                string id = objcls.Decrypt(Request.QueryString["Fileid2"].ToString());
                //Display(Request.QueryString["Fileid"].ToString());
                Fpath = "filepath2";
                Display(id, Fpath);
            }
            if (Request.QueryString["Fileid3"] != null)
            {
                string id = objcls.Decrypt(Request.QueryString["Fileid3"].ToString());
                Fpath = "filepath3";
                //Display(Request.QueryString["Fileid"].ToString());
                Display(id, Fpath);
            }
            if (Request.QueryString["Fileid4"] != null)
            {
                string id = objcls.Decrypt(Request.QueryString["Fileid4"].ToString());

                Fpath = "filepath4";
                //Display(Request.QueryString["Fileid"].ToString());
                Display(id, Fpath);
            }
            if (Request.QueryString["Fileid5"] != null)
            {

                string id = objcls.Decrypt(Request.QueryString["Fileid5"].ToString());

                Fpath = "filepath5";
                //Display(Request.QueryString["Fileid"].ToString());
                Display(id, Fpath);
            }
            else
            {
                lblnotfound.Text = "File Not Found or Not Available !!";
            }
        }
    }



    public void Display(string id, string Fpath)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string CmdText = "SELECT [Id],'../'+[" + Fpath + "] as Path FROM [tbl_EnquiryData] where id='" + id + "'";

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
                        lblnotfound.Text = "File Not Found or Not Available !!";
                    }
                }
                else
                {
                    lblnotfound.Text = "File Not Found or Not Available !!";
                }

            }
        }
    }


}


