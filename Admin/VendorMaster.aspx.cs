
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


public partial class Admin_VendorMaster : System.Web.UI.Page
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
                VendorCode();
                FillddlState();
                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                }
            }
        }
    }

    //Vendor Code Auto
    protected void VendorCode()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_VendorMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            txtvendorcode.Text = "PAPL/SPC-" + (maxid + 1).ToString();
        }
        else
        {
            txtvendorcode.Text = string.Empty;
        }
    }
    private void FillddlState()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlStatecode.DataSource = dt;
            ddlStatecode.DataValueField = "StateCode";
            ddlStatecode.DataTextField = "StateName";
            ddlStatecode.DataBind();
            ddlStatecode.Items.Insert(0, "-- Select State --");

       
        }
    }
    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM tbl_VendorMaster WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update"; 
            txtvendorname.Text = Dt.Rows[0]["Vendorname"].ToString();
            txtvendorcode.Text = Dt.Rows[0]["Vendorcode"].ToString();
            txtownername.Text = Dt.Rows[0]["Ownername"].ToString();
            txtemailid.Text = Dt.Rows[0]["EmailID"].ToString();
            txtgstno.Text = Dt.Rows[0]["GSTNo"].ToString();
            txtmobileno.Text = Dt.Rows[0]["MobileNo"].ToString();
            txtaddressline1.Text = Dt.Rows[0]["Address"].ToString();
            txtaddresLINE2.Text = Dt.Rows[0]["Address2"].ToString();
            txtcity.Text = Dt.Rows[0]["City"].ToString();
            txtPincode.Text = Dt.Rows[0]["Pincode"].ToString();
            txtlocation.Text = Dt.Rows[0]["location"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
            txtPanno.Text = Dt.Rows[0]["Panno"].ToString();
            ddlStatecode.SelectedValue = Dt.Rows[0]["State"].ToString();

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

    //Save and Update Record
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtvendorname.Text == "" )
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
     
                if (btnsave.Text == "Update")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand Cmd = new SqlCommand("SP_VendorMaster", Cls_Main.Conn);
                    Cmd.CommandType = CommandType.StoredProcedure;
                    Cmd.Parameters.AddWithValue("@Action", "Update");
                    Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                    Cmd.Parameters.AddWithValue("@Vendorname", txtvendorname.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Vendorcode", txtvendorcode.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Ownername", txtownername.Text.Trim());
                    Cmd.Parameters.AddWithValue("@EmailID", txtemailid.Text.Trim());
                    Cmd.Parameters.AddWithValue("@GSTNo", txtgstno.Text.Trim());
                    Cmd.Parameters.AddWithValue("@PANNo", txtPanno.Text.Trim());                    
                    Cmd.Parameters.AddWithValue("@MobileNo", txtmobileno.Text.Trim());
                    Cmd.Parameters.AddWithValue("@State", ddlStatecode.SelectedValue);
                    Cmd.Parameters.AddWithValue("@Address", txtaddressline1.Text.Trim());       
                    Cmd.Parameters.AddWithValue("@Address2", txtaddresLINE2.Text.Trim());       
                    Cmd.Parameters.AddWithValue("@City", txtcity.Text.Trim());
                    Cmd.Parameters.AddWithValue("@Pincode", txtPincode.Text.Trim());
                    Cmd.Parameters.AddWithValue("@location", txtlocation.Text.Trim());       
                    Cmd.Parameters.AddWithValue("@PaymentTerm", txtPaymentTerm.Text.Trim());       
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
                    // Cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                    Cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Suplier Updated Successfully..!!');window.location='VendorMasterList.aspx'; ", true);
                }
                else
                {
                 
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_VendorMaster", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                        Cmd.Parameters.AddWithValue("@Vendorname", txtvendorname.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Vendorcode", txtvendorcode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Ownername", txtownername.Text.Trim());
                        Cmd.Parameters.AddWithValue("@EmailID", txtemailid.Text.Trim());
                        Cmd.Parameters.AddWithValue("@GSTNo", txtgstno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@PANNo", txtPanno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@State", ddlStatecode.SelectedValue);
                        Cmd.Parameters.AddWithValue("@Address", txtaddressline1.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Address2", txtaddresLINE2.Text.Trim());
                        Cmd.Parameters.AddWithValue("@City", txtcity.Text.Trim());
                          Cmd.Parameters.AddWithValue("@Pincode", txtPincode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@MobileNo", txtmobileno.Text.Trim());
                    //    Cmd.Parameters.AddWithValue("@Address", txtaddress.Text.Trim());
                        Cmd.Parameters.AddWithValue("@location", txtlocation.Text.Trim());
                        Cmd.Parameters.AddWithValue("@PaymentTerm", txtPaymentTerm.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

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
                        // Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Suplier Insert Successfully..!!');window.location='VendorMasterList.aspx'; ", true);
                    
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //Page Redirect/Refresh
    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("VendorMasterList.aspx");
    }

    protected void txtgstno_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtgstno.Text != null)
            {
                string gstNumber = txtgstno.Text.Trim();
                string stateCode = gstNumber.Substring(0, 2);
                txtPanno.Text = gstNumber.Substring(2, 10);
                int numericStateCode;
                if (int.TryParse(stateCode, out numericStateCode))
                {
                    ddlStatecode.SelectedValue = numericStateCode.ToString();
                }
                else
                {
                    // Handle cases where the stateCode is not a valid number
                    ddlStatecode.SelectedValue = stateCode;  // Set to original value or handle accordingly
                }
              
            }
        }
        catch { }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("VendorMasterList.aspx");
    }
}