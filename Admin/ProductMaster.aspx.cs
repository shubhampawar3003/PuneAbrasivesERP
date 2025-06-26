
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


public partial class Admin_ProductMaster : System.Web.UI.Page
{
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
                ProductCode();
                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                }
            }
        }
    }



    //Data Fetch
    private void Load_Record(string ID)
    {
        try
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ProductMaster] WHERE ID ='" + ID + "' ");
            if (Dt.Rows.Count > 0)
            {
                btnsave.Text = "Update";
                //  ddlbrandname.SelectedItem.Text = Dt.Rows[0]["BrandName"].ToString();
                txtQTY.Text = Dt.Rows[0]["QTY"].ToString();
                txtPrice.Text = Dt.Rows[0]["Price"].ToString();
                txtDescription.Text = Dt.Rows[0]["Description"].ToString();           
                txttariff.Text = Dt.Rows[0]["HSN"].ToString();
                ddlUnit.SelectedItem.Text = Dt.Rows[0]["Unit"].ToString();
                txtproductcode.Text = Dt.Rows[0]["ProductCode"].ToString();
                txtproductname.Text = Dt.Rows[0]["ProductName"].ToString();

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
        catch
        {

        }
    }

    //Brand Code Auto
    protected void ProductCode()
    {
        try
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_ProductMaster]", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
                txtproductcode.Text = "PAPL/P-Code-" + (maxid + 1).ToString();
            }
            else
            {
                txtproductcode.Text = string.Empty;
            }
        }
        catch
        {

        }
    }


    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtproductname.Text == "" || txttariff.Text == "" || txtDescription.Text == ""  || txtPrice.Text == "" || txtproductcode.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_ProductMaster WHERE  ProductName = '" + txtproductname.Text + "'  AND IsDeleted='0'");
                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("[SP_ProductMaster]", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                    Cmd.Parameters.AddWithValue("@Unit", ddlUnit.SelectedItem.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
                    Cmd.Parameters.AddWithValue("@QTY", txtQTY.Text.Trim());                 
                    Cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    Cmd.Parameters.AddWithValue("@HSN", txttariff.Text.Trim());
                    Cmd.Parameters.AddWithValue("@ProductName", txtproductname.Text.Trim());
                    Cmd.Parameters.AddWithValue("@ProductCode", txtproductcode.Text.Trim());
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
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Product Update Successfully..!!');window.location='ProductMasterList.aspx'; ", true);
                }
                else
                {
                    if (Dt.Rows.Count > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Data Alredy Exists..!!')", true);
                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("[SP_ProductMaster]", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                        // Cmd.Parameters.AddWithValue("@BrandName", ddlbrandname.SelectedItem.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Unit", ddlUnit.SelectedItem.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Price", txtPrice.Text.Trim());
                        Cmd.Parameters.AddWithValue("@QTY", txtQTY.Text.Trim());                    
                        Cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        Cmd.Parameters.AddWithValue("@HSN", txttariff.Text.Trim());
                        Cmd.Parameters.AddWithValue("@ProductName", txtproductname.Text.Trim());
                        Cmd.Parameters.AddWithValue("@ProductCode", txtproductcode.Text.Trim());
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
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Product Added Successfully..!!');window.location='ProductMasterList.aspx'; ", true);
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
        Response.Redirect("ProductMasterList.aspx");
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProductMasterList.aspx");
    }
}