
using DocumentFormat.OpenXml.Spreadsheet;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

public partial class Account_ComponentRequestList : System.Web.UI.Page
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
                if (Request.QueryString["Id"] != null)
                {
                    if (Request.UrlReferrer != null)
                    {
                        ViewState["PreviousPageUrl"] = Request.UrlReferrer.ToString();
                    }
                    string Id = objcls.Decrypt(Request.QueryString["Id"].ToString());

                    FillGrid();
                }
            }
        }
    }

    //Fill GridView
    private void FillGrid()
    {
        try
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_OutwardEntryHdr] WHERE IsEditApproval=1");
            if (Dt.Rows.Count > 0)
            {
                GVInvoice.DataSource = Dt;
                GVInvoice.DataBind();
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
    protected void GVInvoice_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Approve")
            {
                Cls_Main.Conn_Open();
                SqlCommand Cmd = new SqlCommand("UPDATE [tbl_OutwardEntryHdr] SET ApprovedBy=@ApprovedBy,Approveddate=@Approveddate,IsEditApproval=@IsEditApproval WHERE id=@invoiceNo", Cls_Main.Conn);
                Cmd.Parameters.AddWithValue("@IsEditApproval", 2);
                Cmd.Parameters.AddWithValue("@invoiceNo", e.CommandArgument.ToString());
                Cmd.Parameters.AddWithValue("@ApprovedBy", Session["Username"].ToString());
                Cmd.Parameters.AddWithValue("@Approveddate", DateTime.Now);
                Cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Approved Successfully..!!');window.location='../Account/ComponentRequestList.aspx'; ", true);

            }
            if(e.CommandName == "RowCancel")
            {
                Cls_Main.Conn_Open();
                SqlCommand Cmd = new SqlCommand("UPDATE [tbl_OutwardEntryHdr] SET IsEditApproval=@IsEditApproval WHERE id=@invoiceNo", Cls_Main.Conn);
                Cmd.Parameters.AddWithValue("@IsEditApproval", 0);
                Cmd.Parameters.AddWithValue("@invoiceNo", e.CommandArgument.ToString());
                Cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Reject Request  Successfully..!!');window.location='../Account/ComponentRequestList.aspx'; ", true);

            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        if (ViewState["PreviousPageUrl"] != null)
        {
            Response.Redirect(ViewState["PreviousPageUrl"].ToString());
        }
        else
        {
            Response.Redirect("../Admin/Dashboard.aspx");
        }
    }

    protected void GVInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lnkCancel = (LinkButton)e.Row.FindControl("lnkCancel");
            LinkButton lnkbtnapprove = (LinkButton)e.Row.FindControl("lnkbtnapprove");
            string Role = Session["Role"].ToString();

            if (Role!="Admin")
            {
                GVInvoice.Columns[5].Visible = false;
            }

        }

    }
}