
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_TransporterList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        FillGrid();
    }

    //Fill GridView
    private void FillGrid()
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_TransporterMaster] WHERE IsDeleted = 0");
        GVTransporter.DataSource = Dt;
        GVTransporter.DataBind();
    }

    //Encrypt
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

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("TransporterMaster.aspx");
    }

    protected void GVTransporter_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("TransporterMaster.aspx?Id=" + encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_TransporterMaster] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Transporter Deleted Successfully..!!')", true);
            FillGrid();
        }
    }

    protected void GVTransporter_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVTransporter.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVTransporter_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton Lnk_Delete = (LinkButton)e.Row.FindControl("btnDelete");
            LinkButton Lnk_Edit = (LinkButton)e.Row.FindControl("btnEdit");
            //  DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_UserMaster] WHERE Username='" + Session["UserCode"].ToString() + "'");
            //if (Dt.Rows.Count > 0)
            //{
            //    string Role = Dt.Rows[0]["Role"].ToString();
            //    if (Role == "Super User" || Role == "User")
            //    {
            //        Lnk_Delete.Visible = false;
            //        Lnk_Edit.Visible = false;
            //        GVUser.Columns[6].Visible = false;
            //    }
            //    else
            //    {
            //        Lnk_Delete.Visible = true;
            //        Lnk_Edit.Visible = true;
            //    }
            //}
        }
    }
}