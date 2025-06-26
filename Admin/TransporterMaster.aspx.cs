
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


public partial class Admin_TransportorMastre : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["ID"] != null)
            {
                string Id = Decrypt(Request.QueryString["ID"].ToString());
                hhd.Value = Id;
                Load_Record(Id);
            }
        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_TransporterMaster WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            txttransporter.Text = Dt.Rows[0]["TransporterName"].ToString();
            txtmobileno.Text = Dt.Rows[0]["Mobileno"].ToString();
            txtemail.Text = Dt.Rows[0]["Email"].ToString();
            txtaddress.Text = Dt.Rows[0]["Address"].ToString();       
            if (Dt.Rows[0]["Status"].ToString() == "False")
            {
                chkisactive.Checked = false;
            }
            else
            {
                chkisactive.Checked = true;
            }
        }
    }

    //Encrept and Decrypt
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

    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txttransporter.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_TransporterMaster WHERE TransporterName = '" + txttransporter.Text + "' AND IsDeleted='0'");
                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_Transporter", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                    Cmd.Parameters.AddWithValue("@TransporterName", txttransporter.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Email", txtemail.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Address", txtaddress.Text.Trim());                  
                    Cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                    bool Status = true;
                    if (chkisactive.Checked == true)
                    {
                        Status = true;
                    }
                    else
                    {
                        Status = false;
                    }
                    Cmd.Parameters.AddWithValue("@Status", Status);
                    Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                    Cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Transporter Update Successfully..!!');window.location='TransporterList.aspx'; ", true);
                }
                else
                {
                    if (Dt.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Transporter Alredy Exists..!!')", true);
                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_Transporter", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                        Cmd.Parameters.AddWithValue("@TransporterName", txttransporter.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Email", txtemail.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Address", txtaddress.Text.Trim());                  
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());

                        bool Status = true;
                        if (chkisactive.Checked == true)
                        {
                            Status = true;
                        }
                        else
                        {
                            Status = false;
                        }
                        Cmd.Parameters.AddWithValue("@Status", Status);
                        Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Transporter Added Successfully..!!');window.location='TransporterList.aspx'; ", true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("TransporterList.aspx");
    }
}