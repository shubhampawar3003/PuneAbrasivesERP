using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



public partial class Admin_CompanyMaster : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable Dt_Component = new DataTable();
    CommonCls objcls = new CommonCls();
    DataTable Dt = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserCode"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            ViewState["CODE"] = null;
            if (!IsPostBack)
            {

                ViewState["RowNo"] = 0;
                Dt_Component.Columns.AddRange(new DataColumn[6] { new DataColumn("id"), new DataColumn("Name"), new DataColumn("Number"), new DataColumn("EmailID"), new DataColumn("Department"), new DataColumn("Designation") });
                ViewState["ContactDetails"] = Dt_Component;

                ViewState["RowNo"] = 0;
                DataTable dtb = new DataTable();
                dtb.Columns.AddRange(new DataColumn[6] { new DataColumn("id"), new DataColumn("BillAddress"), new DataColumn("BillLocation"), new DataColumn("BillPincode"), new DataColumn("BillState"), new DataColumn("GSTNo") });
                ViewState["BillDetails"] = dtb;


                ViewState["RowNo"] = 0;
                DataTable dtS = new DataTable();
                dtS.Columns.AddRange(new DataColumn[6] { new DataColumn("id"), new DataColumn("ShippingAddress"), new DataColumn("ShipLocation"), new DataColumn("ShipPincode"), new DataColumn("ShipState"), new DataColumn("ShipGSTNo") });
                ViewState["ShipDetails"] = dtS;


                FillddlState();
                CompanyCode();
                fillddlCountryCode();
                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                    btnsave.Text = "Update";
                    txtcompanycode.ReadOnly = true;
                    ShowDtlEdit();
                }


            }
        }
    }
    private void FillddlState()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlBState.DataSource = dt;
            ddlBState.DataValueField = "statename";
            ddlBState.DataTextField = "statename";
            ddlBState.DataBind();
            ddlBState.Items.Insert(0, "-- select state --");

            ddlSState.DataSource = dt;
            ddlSState.DataValueField = "statename";
            ddlSState.DataTextField = "StateName";
            ddlSState.DataBind();
            ddlSState.Items.Insert(0, "-- Select State --");
        }
    }
    private void fillddlCountryCode()
    {
        SqlDataAdapter adpt = new SqlDataAdapter("select * from tblCountryCode", Cls_Main.Conn);
        DataTable dtpt = new DataTable();
        adpt.Fill(dtpt);

        if (dtpt.Rows.Count > 0)
        {
            ddlCountryCode.DataSource = dtpt;
            ddlCountryCode.DataValueField = "CountryCode";
            ddlCountryCode.DataTextField = "CountryName";
            ddlCountryCode.DataBind();
            ddlCountryCode.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select--", "0"));
        }
    }
    //Company Code Auto
    protected void CompanyCode()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_CompanyMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            txtcompanycode.Text = "PAPL/COMP-" + (maxid + 1).ToString();
        }
        else
        {
            txtcompanycode.Text = string.Empty;
        }
    }
    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CompanyMaster] WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            hhd.Value = Dt.Rows[0]["ID"].ToString();
            txtvendorcode.Text = Dt.Rows[0]["VendorCode"].ToString();
            txtcompanyname.Text = Dt.Rows[0]["Companyname"].ToString();
            txtcompanycode.Text = Dt.Rows[0]["CompanyCode"].ToString();
            txtPrimaryEmail.Text = Dt.Rows[0]["PrimaryEmailID"].ToString();
            txtSecondaryemailid.Text = Dt.Rows[0]["SecondaryEmailID"].ToString();
            txtgstno.Text = Dt.Rows[0]["GSTno"].ToString();
            if (Dt.Rows[0]["GSTno"].ToString() == "URP")
            {
                contry.Visible = true;
            }
            else
            {
                contry.Visible = false;
            }
            txtUDYAM.Text = Dt.Rows[0]["UDYAMNO"].ToString();
            txtCINNO.Text = Dt.Rows[0]["CINNO"].ToString();
            txtCompanyPan.Text = Dt.Rows[0]["Companypancard"].ToString();
            ddlClientType.SelectedItem.Text = Dt.Rows[0]["Clienttype"].ToString();
            txtWebsiteLink.Text = Dt.Rows[0]["WebsiteLink"].ToString();
            TxtCreditLimit.Text = Dt.Rows[0]["creditlimit"].ToString();
            ddlTypeofSupply.SelectedValue = Dt.Rows[0]["E_inv_Typeof_supply"].ToString();
            ddlCountryCode.SelectedValue = Dt.Rows[0]["CountryCode"].ToString();
            txtBPincode.Text = Dt.Rows[0]["Billingpincode"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
            lblPath1.Text = Dt.Rows[0]["VisitingCardPath"].ToString(); FileUpload1.Enabled = true;
            BindBillingAddressGrid(ID);
            BindShippingAddressGrid(ID);
        }
    }

    private void BindShippingAddressGrid(string id)
    {
        ViewState["RowNo"] = 0;
        DataTable Dtproduct = new DataTable();
        SqlDataAdapter Daa = new SqlDataAdapter("SELECT * FROM tbl_ShippingAddress WHERE c_id='" + id + "' ", con);
        Daa.Fill(Dtproduct);
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
        DataTable Dt = ViewState["ShipDetails"] as DataTable;
        //int count = 1;
        if (Dtproduct.Rows.Count > 0)
        {
            for (int i = 0; i < Dtproduct.Rows.Count; i++)
            {
                Cls_Main.Conn_Open();

                Dt.Rows.Add(ViewState["RowNo"], Dtproduct.Rows[i]["ShippingAddress"].ToString(), Dtproduct.Rows[i]["ShipLocation"].ToString(), Dtproduct.Rows[i]["ShipPincode"].ToString(), Dtproduct.Rows[i]["ShipStatename"].ToString(), Dtproduct.Rows[i]["GSTNo"].ToString());
                //count = count + 1;
                ViewState["ShipDetails"] = Dt;
            }
        }

        GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"];
        GVSAddress.DataBind();
    }
    private void BindBillingAddressGrid(string id)
    {
        ViewState["RowNo"] = 0;
        DataTable Dtproduct = new DataTable();
        SqlDataAdapter Daa = new SqlDataAdapter("SELECT * FROM tbl_BillingAddress WHERE c_id='" + id + "' ", con);
        Daa.Fill(Dtproduct);
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
        DataTable Dt = ViewState["BillDetails"] as DataTable;
        //int count = 1;
        if (Dtproduct.Rows.Count > 0)
        {
            for (int i = 0; i < Dtproduct.Rows.Count; i++)
            {

                Dt.Rows.Add(ViewState["RowNo"], Dtproduct.Rows[i]["BillAddress"].ToString(), Dtproduct.Rows[i]["BillLocation"].ToString(), Dtproduct.Rows[i]["BillPincode"].ToString(), Dtproduct.Rows[i]["BillStatename"].ToString(), Dtproduct.Rows[i]["GSTNo"].ToString());
                //count = count + 1;
                ViewState["BillDetails"] = Dt;
            }
        }

        GVBAddress.DataSource = (DataTable)ViewState["BillDetails"];
        GVBAddress.DataBind();
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtcompanyname.Text == "" || txtPrimaryEmail.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {

                string PathH = null;

                if (dgvContactDetails.Rows.Count > 0)
                {
                    //if (GVBAddress.Rows.Count > 0 && GVSAddress.Rows.Count > 0)
                    //{

                    if (btnsave.Text == "Update")
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_CompanyMaster", Cls_Main.Conn);
                        HttpPostedFile postedFile = FileUpload1.PostedFile;
                        if (FileUpload1.HasFile)
                        {

                            foreach (HttpPostedFile PostedFile in FileUpload1.PostedFiles)
                            {
                                string filename = Path.GetFileName(postedFile.FileName);
                                string[] pdffilename = filename.Split('.');
                                string pdffilename1 = pdffilename[0];
                                string filenameExt = pdffilename[1];
                                //if (filenameExt == "pdf" || filenameExt == "PDF")
                                //{
                                string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                                postedFile.SaveAs(Server.MapPath("~/VisitingcardFiles/") + pdffilename1 + time1 + "." + filenameExt);

                                Cmd.Parameters.AddWithValue("@VisitingCardPath", "VisitingcardFiles/" + pdffilename1 + time1 + "." + filenameExt);
                                //}
                                //else
                                //{
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                                //}
                            }
                        }
                        else
                        {
                            Cmd.Parameters.AddWithValue("@VisitingCardPath", lblPath1.Text);
                        }
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Action", "Update");
                        Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                        Cmd.Parameters.AddWithValue("@Companyname", txtcompanyname.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CompanyCode", txtcompanycode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@TypeofSupply", ddlTypeofSupply.SelectedValue);
                        Cmd.Parameters.AddWithValue("@CreditLimit", TxtCreditLimit.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Vendorcode", txtvendorcode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@PrimaryEmail", txtPrimaryEmail.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Secondaryemailid", txtSecondaryemailid.Text.Trim());
                        Cmd.Parameters.AddWithValue("@GSTno", txtgstno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@UDYAMNO", txtUDYAM.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CINNO", txtCINNO.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CompanyPancard", txtCompanyPan.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Clienttype", ddlClientType.SelectedValue);
                        Cmd.Parameters.AddWithValue("@WebsiteLink", txtWebsiteLink.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Countrycode", ddlCountryCode.SelectedValue);
                        Cmd.Parameters.AddWithValue("@Paymentterm", txtPaymentTerm.Text);
                        Cmd.Parameters.AddWithValue("@BillingPincode", txtBPincode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        Cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();

                        // Delete Contact Details
                        Cls_Main.Conn_Open();
                        SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_CompanyContactDetails WHERE CompanyCode=@CompanyCode", Cls_Main.Conn);
                        cmddelete.Parameters.AddWithValue("@CompanyCode", txtcompanycode.Text);
                        cmddelete.ExecuteNonQuery();
                        Cls_Main.Conn_Close();

                        //Save Contact Details 
                        foreach (GridViewRow grd1 in dgvContactDetails.Rows)
                        {
                            string lblname = (grd1.FindControl("lblname") as Label).Text;
                            string lblnumber = (grd1.FindControl("lblnumber") as Label).Text;
                            string lblemailid = (grd1.FindControl("lblemailid") as Label).Text;
                            string lblDepartment = (grd1.FindControl("lblDepartment") as Label).Text;
                            string lbldesignation = (grd1.FindControl("lbldesignation") as Label).Text;
                            Cls_Main.Conn_Open();
                            SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_CompanyContactDetails (CompanyCode,Name,Number,EmailID,Department,Designation,CreatedBy,CreatedOn) VALUES (@CompanyCode,@Name,@Number,@EmailID,@Department,@Designation,@CreatedBy,@createdOn)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@CompanyCode", txtcompanycode.Text.Trim());

                            cmdd.Parameters.AddWithValue("@Name", lblname);
                            cmdd.Parameters.AddWithValue("@Number", lblnumber);
                            cmdd.Parameters.AddWithValue("@EmailID", lblemailid);
                            cmdd.Parameters.AddWithValue("@Department", lblDepartment);
                            cmdd.Parameters.AddWithValue("@Designation", lbldesignation);
                            cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                            cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                            cmdd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }

                        //DataTable Dtt = new DataTable();
                        //SqlDataAdapter Daa = new SqlDataAdapter("SELECT * FROM tbl_ShippingAddress WHERE c_id ='" + hhd.Value + "'", Cls_Main.Conn);
                        //Daa.Fill(Dtt);
                        if (GVBAddress.Rows.Count > 0)
                        {
                            Cls_Main.Conn_Open();
                            SqlCommand cmddelete1 = new SqlCommand("DELETE FROM tbl_BillingAddress WHERE c_id=@c_id", Cls_Main.Conn);
                            cmddelete1.Parameters.AddWithValue("@c_id", hhd.Value);
                            cmddelete1.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                            foreach (GridViewRow g2 in GVBAddress.Rows)
                            {
                                string BillAddress = (g2.FindControl("lblBillAddress") as Label).Text;
                                string BillLocation = (g2.FindControl("lblBillLocation") as Label).Text;
                                string BillPincode = (g2.FindControl("lblBillPincode") as Label).Text;
                                string BillState = (g2.FindControl("lblBillState") as Label).Text;
                                string BGSTno = (g2.FindControl("lblBGSTno") as Label).Text;
                                string statecode = string.Empty;
                                DataTable Dt1 = Cls_Main.Read_Table("SELECT * FROM tbl_States WHERE StateName ='" + BillState + "' ");
                                if (Dt1.Rows.Count > 0)
                                {
                                    statecode = Dt1.Rows[0]["StateCode"].ToString();
                                }

                                Cls_Main.Conn_Open();
                                SqlCommand Cmdd = new SqlCommand("INSERT INTO tbl_BillingAddress ([c_id],[BillLocation],[BillAddress],[BillPincode],[BillStatecode],GSTNo,BillStateName) VALUES (@c_id,@BillLocation,@BillAddress,@BillPincode,@BillStatecode,@BGSTno,@BillStatename)", Cls_Main.Conn);
                                Cmdd.Parameters.AddWithValue("@c_id", hhd.Value);
                                Cmdd.Parameters.AddWithValue("@BillLocation", BillLocation);
                                Cmdd.Parameters.AddWithValue("@BillAddress", BillAddress);
                                Cmdd.Parameters.AddWithValue("@BillPincode", BillPincode);
                                Cmdd.Parameters.AddWithValue("@BillStatecode", statecode);
                                Cmdd.Parameters.AddWithValue("@BillStatename", BillState);
                                Cmdd.Parameters.AddWithValue("@BGSTno", BGSTno);
                                Cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();

                            }
                        }
                        if (GVSAddress.Rows.Count > 0)
                        {
                            Cls_Main.Conn_Open();
                            SqlCommand cmddelete2 = new SqlCommand("DELETE FROM tbl_ShippingAddress WHERE c_id=@c_id", Cls_Main.Conn);
                            cmddelete2.Parameters.AddWithValue("@c_id", hhd.Value);
                            cmddelete2.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                            foreach (GridViewRow g2 in GVSAddress.Rows)
                            {
                                string ShipAddress = (g2.FindControl("lblShipAddress") as Label).Text;
                                string ShipLocation = (g2.FindControl("lblShipLocation") as Label).Text;
                                string ShipPincode = (g2.FindControl("lblShipPincode") as Label).Text;
                                string ShipState = (g2.FindControl("lblShipState") as Label).Text;
                                string SGSTno = (g2.FindControl("lblSGSTno") as Label).Text;
                                string statecode = string.Empty;
                                DataTable Dt1 = Cls_Main.Read_Table("SELECT * FROM tbl_States WHERE StateName ='" + ShipState + "' ");
                                if (Dt1.Rows.Count > 0)
                                {
                                    statecode = Dt1.Rows[0]["StateCode"].ToString();
                                }

                                Cls_Main.Conn_Open();
                                SqlCommand Cmdd = new SqlCommand("INSERT INTO tbl_ShippingAddress ([c_id],[ShipLocation],[ShippingAddress],[ShipPincode],[ShipStatecode],GSTNo,ShipStatename) VALUES (@c_id,@ShipLocation,@ShippingAddress,@ShipPincode,@ShipStatecode,@SGSTno,@ShipStatename)", Cls_Main.Conn);
                                Cmdd.Parameters.AddWithValue("@c_id", hhd.Value);
                                Cmdd.Parameters.AddWithValue("@ShipLocation", ShipLocation);
                                Cmdd.Parameters.AddWithValue("@ShippingAddress", ShipAddress);
                                Cmdd.Parameters.AddWithValue("@ShipPincode", ShipPincode);
                                Cmdd.Parameters.AddWithValue("@ShipStatecode", statecode);
                                Cmdd.Parameters.AddWithValue("@ShipStatename", ShipState);
                                Cmdd.Parameters.AddWithValue("@SGSTno", SGSTno);
                                Cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();

                            }
                        }

                        if (Request.QueryString["CODE"] != null)
                        {

                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Updated Successfully..!!'); ", true);
                            Response.Redirect("QuatationMaster.aspx?CODE=" + Request.QueryString["CODE"].ToString() + "");

                        }
                        else
                        if (Request.QueryString["OAID"] != null)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Updated Successfully..!!'); ", true);
                            Response.Redirect("AddCustomerPO.aspx?OAID=" + objcls.encrypt(txtcompanyname.Text) + "");

                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Updated Successfully..!!');window.location='CompanyMasterList.aspx'; ", true);
                        }
                    }
                    else
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand Cmd = new SqlCommand("SP_CompanyMaster", Cls_Main.Conn);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        HttpPostedFile postedFile = FileUpload1.PostedFile;
                        if (FileUpload1.HasFile)
                        {
                            foreach (HttpPostedFile PostedFile in FileUpload1.PostedFiles)
                            {
                                string filename = Path.GetFileName(postedFile.FileName);
                                string[] pdffilename = filename.Split('.');
                                string pdffilename1 = pdffilename[0];
                                string filenameExt = pdffilename[1];
                                //if (filenameExt == "pdf" || filenameExt == "PDF")
                                //{
                                string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                                postedFile.SaveAs(Server.MapPath("~/VisitingcardFiles/") + pdffilename1 + time1 + "." + filenameExt);

                                Cmd.Parameters.AddWithValue("@VisitingCardPath", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                                //}
                                //else
                                //{
                                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                                //}
                            }
                        }
                        Cmd.Parameters.AddWithValue("@Action", "Save");
                        Cmd.Parameters.AddWithValue("@ID", hhd.Value);
                        Cmd.Parameters.AddWithValue("@Companyname", txtcompanyname.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CompanyCode", txtcompanycode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@TypeofSupply", ddlTypeofSupply.SelectedValue);
                        Cmd.Parameters.AddWithValue("@CreditLimit", TxtCreditLimit.Text.Trim());
                        // Cmd.Parameters.AddWithValue("@BState", ddlBStateCode.SelectedValue);
                        Cmd.Parameters.AddWithValue("@Countrycode", ddlCountryCode.SelectedValue);
                        Cmd.Parameters.AddWithValue("@SState", DBNull.Value);
                        Cmd.Parameters.AddWithValue("@PrimaryEmail", txtPrimaryEmail.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Vendorcode", txtvendorcode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Secondaryemailid", txtSecondaryemailid.Text.Trim());
                        Cmd.Parameters.AddWithValue("@GSTno", txtgstno.Text.Trim());
                        Cmd.Parameters.AddWithValue("@UDYAMNO", txtUDYAM.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CINNO", txtCINNO.Text.Trim());
                        Cmd.Parameters.AddWithValue("@CompanyPancard", txtCompanyPan.Text.Trim());
                        Cmd.Parameters.AddWithValue("@Clienttype", ddlClientType.SelectedValue);
                        Cmd.Parameters.AddWithValue("@WebsiteLink", txtWebsiteLink.Text.Trim());
                        Cmd.Parameters.AddWithValue("@VisitingCardPath", PathH);
                        Cmd.Parameters.AddWithValue("@Paymentterm", txtPaymentTerm.Text);
                        //   Cmd.Parameters.AddWithValue("@BillingAddress", txtBillingAddress.Text.Trim());
                        //  Cmd.Parameters.AddWithValue("@Shippingaddress", txtshippingaddress.Text.Trim());
                        Cmd.Parameters.AddWithValue("@BillingPincode", txtBPincode.Text.Trim());
                        Cmd.Parameters.AddWithValue("@ShippingPincode", DBNull.Value);
                        //  Cmd.Parameters.AddWithValue("@billinglocation", txtbillinglocation.Text.Trim());
                        Cmd.Parameters.AddWithValue("@shippinglocation", DBNull.Value);
                        Cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        Cmd.Parameters.AddWithValue("@IsDeleted", '0');
                        Cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                        Cmd.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        Cls_Main.Conn_Dispose();

                        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CompanyMaster] WHERE CompanyCode ='" + txtcompanycode.Text + "' ");
                        if (Dt.Rows.Count > 0)
                        {

                            hhd.Value = Dt.Rows[0]["ID"].ToString();
                        }
                        if (GVBAddress.Rows.Count > 0)
                        {
                            foreach (GridViewRow g2 in GVBAddress.Rows)
                            {
                                string BillAddress = (g2.FindControl("lblBillAddress") as Label).Text;
                                string BillLocation = (g2.FindControl("lblBillLocation") as Label).Text;
                                string BillPincode = (g2.FindControl("lblBillPincode") as Label).Text;
                                string BillState = (g2.FindControl("lblBillState") as Label).Text;
                                string BGSTno = (g2.FindControl("lblBGSTno") as Label).Text;
                                string statecode = string.Empty;
                                DataTable Dt1 = Cls_Main.Read_Table("SELECT * FROM tbl_States WHERE StateName ='" + BillState + "' ");
                                if (Dt1.Rows.Count > 0)
                                {
                                    statecode = Dt1.Rows[0]["StateCode"].ToString();
                                }

                                Cls_Main.Conn_Open();
                                SqlCommand Cmdd = new SqlCommand("INSERT INTO tbl_BillingAddress ([c_id],[BillLocation],[BillAddress],[BillPincode],[BillStatecode],GSTNo,BillStateName) VALUES (@c_id,@BillLocation,@BillAddress,@BillPincode,@BillStatecode,@BGSTno,@BillStatename)", Cls_Main.Conn);
                                Cmdd.Parameters.AddWithValue("@c_id", hhd.Value);
                                Cmdd.Parameters.AddWithValue("@BillLocation", BillLocation);
                                Cmdd.Parameters.AddWithValue("@BillAddress", BillAddress);
                                Cmdd.Parameters.AddWithValue("@BillPincode", BillPincode);
                                Cmdd.Parameters.AddWithValue("@BillStatecode", statecode);
                                Cmdd.Parameters.AddWithValue("@BillStatename", BillState);
                                Cmdd.Parameters.AddWithValue("@BGSTno", BGSTno);
                                Cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();

                            }
                        }
                        if (GVSAddress.Rows.Count > 0)
                        {
                            foreach (GridViewRow g2 in GVSAddress.Rows)
                            {
                                string ShipAddress = (g2.FindControl("lblShipAddress") as Label).Text;
                                string ShipLocation = (g2.FindControl("lblShipLocation") as Label).Text;
                                string ShipPincode = (g2.FindControl("lblShipPincode") as Label).Text;
                                string ShipState = (g2.FindControl("lblShipState") as Label).Text;
                                string SGSTno = (g2.FindControl("lblSGSTno") as Label).Text;
                                string statecode = string.Empty;
                                DataTable Dt1 = Cls_Main.Read_Table("SELECT * FROM tbl_States WHERE StateName ='" + ShipState + "' ");
                                if (Dt1.Rows.Count > 0)
                                {
                                    statecode = Dt1.Rows[0]["StateCode"].ToString();
                                }

                                Cls_Main.Conn_Open();
                                SqlCommand Cmdd = new SqlCommand("INSERT INTO tbl_ShippingAddress ([c_id],[ShipLocation],[ShippingAddress],[ShipPincode],[ShipStatecode],GSTNo,ShipStatename) VALUES (@c_id,@ShipLocation,@ShippingAddress,@ShipPincode,@ShipStatecode,@SGSTno,@ShipStatename)", Cls_Main.Conn);
                                Cmdd.Parameters.AddWithValue("@c_id", hhd.Value);
                                Cmdd.Parameters.AddWithValue("@ShipLocation", ShipLocation);
                                Cmdd.Parameters.AddWithValue("@ShippingAddress", ShipAddress);
                                Cmdd.Parameters.AddWithValue("@ShipPincode", ShipPincode);
                                Cmdd.Parameters.AddWithValue("@ShipStatecode", statecode);
                                Cmdd.Parameters.AddWithValue("@ShipStatename", ShipState);
                                Cmdd.Parameters.AddWithValue("@SGSTno", SGSTno);
                                Cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();

                            }

                        }
                        //Save Contact Details 
                        foreach (GridViewRow grd1 in dgvContactDetails.Rows)
                        {

                            string lblname = (grd1.FindControl("lblname") as Label).Text;
                            string lblnumber = (grd1.FindControl("lblnumber") as Label).Text;
                            string lblemailid = (grd1.FindControl("lblemailid") as Label).Text;
                            string lblDepartment = (grd1.FindControl("lblDepartment") as Label).Text;
                            string lbldesignation = (grd1.FindControl("lbldesignation") as Label).Text;

                            Cls_Main.Conn_Open();
                            SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_CompanyContactDetails (CompanyCode,Name,Number,EmailID,Department,Designation,CreatedBy,CreatedOn) VALUES (@CompanyCode,@Name,@Number,@EmailID,@Department,@Designation,@CreatedBy,@createdOn)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@CompanyCode", txtcompanycode.Text.Trim());

                            cmdd.Parameters.AddWithValue("@Name", lblname);
                            cmdd.Parameters.AddWithValue("@Number", lblnumber);
                            cmdd.Parameters.AddWithValue("@EmailID", lblemailid);
                            cmdd.Parameters.AddWithValue("@Department", lblDepartment);
                            cmdd.Parameters.AddWithValue("@Designation", lbldesignation);
                            cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                            cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                            cmdd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Added Successfully..!!');window.location='CompanyMasterList.aspx'; ", true);




                    }


                    //}
                    //else
                    //{
                    //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Fill Atleast One Address..!!'); ", true);
                    //}
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Fill Atleast One Contact detail..!!'); ", true);
                }

            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }
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

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("CompanyMasterList.aspx", true);
    }
    protected void ShowDtlEdit()
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_CompanyContactDetails] WHERE CompanyCode='" + txtcompanycode.Text + "'", Cls_Main.Conn);
        DataTable DTCOMP = new DataTable();
        Da.Fill(DTCOMP);

        int count = 0;
        if (DTCOMP.Rows.Count > 0)
        {
            if (Dt_Component.Columns.Count < 1)
            {
                Show_Grid();
            }

            for (int i = 0; i < DTCOMP.Rows.Count; i++)
            {
                Dt_Component.Rows.Add(count, DTCOMP.Rows[i]["Name"].ToString(), DTCOMP.Rows[i]["Number"].ToString(), DTCOMP.Rows[i]["EmailID"].ToString(), DTCOMP.Rows[i]["Department"].ToString(), DTCOMP.Rows[i]["Designation"].ToString());
                count = count + 1;
            }
        }

        dgvContactDetails.EmptyDataText = "No Data Found";
        dgvContactDetails.DataSource = Dt_Component;
        dgvContactDetails.DataBind();
    }

    //Add contact person details
    protected void btnAddMore_Click(object sender, EventArgs e)
    {
        btnsave.Focus();
        if (txtname.Text == "" || txtmobile.Text == "" || txtemaili.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill Contact Information  !!!');", true);
        }
        else
        {
            Show_Grid();
        }
    }

    private void Show_Grid()
    {
        DataTable Dt = (DataTable)ViewState["ContactDetails"];
        Dt.Rows.Add(ViewState["RowNo"], txtname.Text.Trim(), txtmobile.Text.Trim(), txtemaili.Text.Trim(), txtDepartment.Text.Trim(), txtdesignation.Text.Trim());
        ViewState["ContactDetails"] = Dt;
        txtDepartment.Text = string.Empty;
        txtname.Text = string.Empty;
        txtmobile.Text = string.Empty;
        txtemaili.Text = string.Empty;
        txtdesignation.Text = string.Empty;
        dgvContactDetails.DataSource = (DataTable)ViewState["ContactDetails"];
        dgvContactDetails.DataBind();
    }
    protected void lnkbtnDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["ContactDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["ContactDetails"] = dt;
        dgvContactDetails.DataSource = (DataTable)ViewState["ContactDetails"];
        dgvContactDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Contact Details Delete Succesfully !!!');", true);
        //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }
    protected void gv_cancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string id = ((TextBox)row.FindControl("lblsno")).Text;
        string Name = ((TextBox)row.FindControl("lblname")).Text;
        string Number = ((TextBox)row.FindControl("lblnumber")).Text;
        string Email = ((TextBox)row.FindControl("lblemailid")).Text;
        string Department = ((TextBox)row.FindControl("lblDepartment")).Text;
        string Designation = ((TextBox)row.FindControl("lbldesignation")).Text;

        DataTable Dt = ViewState["ContactDetails"] as DataTable;
        Dt.Rows[row.RowIndex]["id"] = id;
        Dt.Rows[row.RowIndex]["Name"] = Name;
        Dt.Rows[row.RowIndex]["Number"] = Number;
        Dt.Rows[row.RowIndex]["EmailID"] = Email;
        Dt.Rows[row.RowIndex]["Department"] = Department;
        Dt.Rows[row.RowIndex]["Designation"] = Designation;
        Dt.AcceptChanges();
        ViewState["ContactDetails"] = Dt;
        dgvContactDetails.EditIndex = -1;
        dgvContactDetails.DataSource = (DataTable)ViewState["ContactDetails"];
        dgvContactDetails.DataBind();
        //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void dgvContactDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvContactDetails.EditIndex = e.NewEditIndex;
        dgvContactDetails.DataSource = (DataTable)ViewState["ContactDetails"];
        dgvContactDetails.DataBind();
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void gv_update_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        string Name = ((TextBox)row.FindControl("txtname")).Text;
        string Number = ((TextBox)row.FindControl("txtmobile")).Text;
        string Email = ((TextBox)row.FindControl("txtemaili")).Text;
        string Department = ((TextBox)row.FindControl("txtDepartment")).Text;
        string Designation = ((TextBox)row.FindControl("txtdesignation")).Text;
        DataTable Dt = ViewState["ContactDetails"] as DataTable;

        Dt.Rows[row.RowIndex]["Name"] = Name;
        Dt.Rows[row.RowIndex]["Number"] = Number;
        Dt.Rows[row.RowIndex]["EmailID"] = Email;
        Dt.Rows[row.RowIndex]["Department"] = Department;
        Dt.Rows[row.RowIndex]["Designation"] = Designation;
        Dt.AcceptChanges();
        ViewState["ContactDetails"] = Dt;
        dgvContactDetails.EditIndex = -1;
        dgvContactDetails.DataSource = (DataTable)ViewState["ContactDetails"];
        dgvContactDetails.DataBind();
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void check_addresss_CheckedChanged(object sender, EventArgs e)
    {
        if (check_addresss.Checked == true)
        {
            GridViewRow row = (sender as CheckBox).NamingContainer as GridViewRow;

            DataTable dt = ViewState["BillDetails"] as DataTable;

            if (dt != null)
            {
                DataTable shipDetails = ViewState["ShipDetails"] as DataTable;
                ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;

                // Loop through each row in BillDetails and add to ShipDetails
                foreach (DataRow row1 in dt.Rows)
                {
                    DataRow newRow = shipDetails.NewRow();
                    newRow["id"] = ViewState["RowNo"];
                    newRow["ShippingAddress"] = row1["BillAddress"].ToString();
                    newRow["ShipLocation"] = row1["BillLocation"].ToString();
                    newRow["ShipPincode"] = row1["BillPincode"].ToString();
                    newRow["ShipState"] = row1["BillState"].ToString();
                    newRow["ShipGSTNo"] = row1["gstno"].ToString(); // If required for Shipping as well

                    shipDetails.Rows.Add(newRow);
                }
                ViewState["ShipDetails"] = shipDetails;
                GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"]; ;
                GVSAddress.DataBind();
            }
        }
    }

    protected void ddlTypeofSupply_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlTypeofSupply.SelectedItem.Text == "EXPWOP")
            {
                fillddlCountryCode();
                txtgstno.Text = "URP"; txtgstno.Enabled = false;
                txtBGST.Text = "URP"; txtBGST.Enabled = false;
                txtSGST.Text = "URP"; txtSGST.Enabled = false;
                txtBPincode.Text = "999999"; txtBPincode.Enabled = false;
                txtSPincode.Text = "999999"; txtSPincode.Enabled = false;
                ddlBState.Text = "Other Territory"; ddlBState.Enabled = false;
                ddlSState.Text = "Other Territory"; ddlSState.Enabled = false;
                contry.Visible = true;
            }
            else
            {
                txtgstno.Text = ""; txtgstno.Enabled = true;
                txtBPincode.Text = ""; txtBPincode.Enabled = true;

                contry.Visible = false;
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    //protected void txtgstno_TextChanged(object sender, EventArgs e)
    // {
    //try
    //{
    //    if (hhd.Value == "")
    //    {
    //        Cls_Main.Conn_Open();
    //        int count = 0;
    //        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_CompanyMaster where GSTno='" + txtgstno.Text.Trim() + "'", Cls_Main.Conn);
    //        count = Convert.ToInt16(cmd.ExecuteScalar());
    //        Cls_Main.Conn_Close();
    //        if (count > 0)
    //        {
    //            txtBGST.Text = "";
    //            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Same GST number Company already available...!');", true);
    //        }
    //        else
    //        {
    //            string gstNumber = txtBGST.Text.Trim();
    //            string pattern = @"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$";

    //            if (System.Text.RegularExpressions.Regex.IsMatch(gstNumber, pattern))
    //            {
    //                string stateCode = gstNumber.Substring(0, 2);
    //                int numericStateCode;
    //                if (int.TryParse(stateCode, out numericStateCode))
    //                {
    //                    // ddlBStateCode.SelectedValue = numericStateCode.ToString();
    //                }
    //                else
    //                {
    //                    // Handle cases where the stateCode is not a valid number
    //                    // ddlBStateCode.SelectedValue = stateCode;  // Set to original value or handle accordingly
    //                }

    //                txtCompanyPan.Text = gstNumber.Substring(2, 10);
    //            }
    //            else
    //            {
    //                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Invalid GST Number. GST number should be in the format- 27ATFPS1959J1Z4');", true);
    //            }
    //        }
    //    }
    //}
    //catch (Exception)
    //{

    //}

    //  }


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

    protected void txtcompanyname_TextChanged(object sender, EventArgs e)
    {
        if (txtcompanyname.Text != null)
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CompanyMaster] WHERE Companyname ='" + txtcompanyname.Text.Trim() + "' ");
            if (Dt.Rows.Count > 0)
            {
                // txtcompanyname.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Same Name customer already available..!!')", true);
            }
        }
    }

    protected void txtPrimaryEmail_TextChanged(object sender, EventArgs e)
    {
        if (txtPrimaryEmail.Text != null)
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CompanyMaster] WHERE Companyname='" + txtcompanyname.Text.Trim() + "' AND PrimaryEmailID ='" + txtPrimaryEmail.Text.Trim() + "' ");
            if (Dt.Rows.Count > 0)
            {
                txtcompanyname.Text = string.Empty;
                txtPrimaryEmail.Text = string.Empty;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Same EmailID customer already available..!!')", true);
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CompanyMasterList.aspx");
    }

    //Billing Address Methods added by 17022025
    protected void btnAddBAddress_Click(object sender, EventArgs e)
    {
        if (txtBAddress.Text == "" || txtBLocation.Text == "" || txtBPincode.Text == "" || ddlBState.SelectedValue == "0" || txtBGST.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill all details !!!');", true);
        }
        else
        {
            DataTable Dt = (DataTable)ViewState["BillDetails"];
            Dt.Rows.Add(ViewState["RowNo"], txtBAddress.Text.Trim(), txtBLocation.Text.Trim(), txtBPincode.Text.Trim(), ddlBState.SelectedItem.Text.Trim(), txtBGST.Text.Trim());
            ViewState["BillDetails"] = Dt;
            txtBAddress.Text = string.Empty;
            txtBLocation.Text = string.Empty;
            txtBPincode.Text = string.Empty;
            txtBGST.Text = string.Empty;
            ddlBState.SelectedItem.Text = string.Empty;
            GVBAddress.DataSource = (DataTable)ViewState["BillDetails"];
            GVBAddress.DataBind();
        }
    }

    protected void GVBAddress_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GVBAddress.EditIndex = e.NewEditIndex;
        GVBAddress.DataSource = (DataTable)ViewState["BillDetails"];
        GVBAddress.DataBind();
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnDeleteB_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["BillDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["BillDetails"] = dt;
        GVBAddress.DataSource = (DataTable)ViewState["BillDetails"];
        GVBAddress.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Address Delete Succesfully !!!');", true);

    }

    protected void gv_updateB_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string state = string.Empty;
        string txtBAddress = ((TextBox)row.FindControl("txtBillAddress")).Text;
        string txtBLocation = ((TextBox)row.FindControl("txtBillLocation")).Text;
        string txtBPincode = ((TextBox)row.FindControl("txtBillPincode")).Text;
        string txtGST = ((TextBox)row.FindControl("txtBGSTno")).Text;
        string txthdnstate = ((HiddenField)row.FindControl("lblBillStatehdn")).Value;
        string ddlBState = ((DropDownList)row.FindControl("ddlBState1")).Text;
        DataTable Dt = ViewState["BillDetails"] as DataTable;
        Cls_Main.Conn_Open();
        if (ddlBState == "")
        {
            state = txthdnstate;
        }
        else
        {
            state = ddlBState;
        }

        Dt.Rows[row.RowIndex]["BillAddress"] = txtBAddress;
        Dt.Rows[row.RowIndex]["BillLocation"] = txtBLocation;
        Dt.Rows[row.RowIndex]["BillPincode"] = txtBPincode;
        Dt.Rows[row.RowIndex]["BillState"] = state;
        Dt.Rows[row.RowIndex]["GSTNo"] = txtGST;
        Dt.AcceptChanges();
        ViewState["BillDetails"] = Dt;
        GVBAddress.EditIndex = -1;
        GVBAddress.DataSource = (DataTable)ViewState["BillDetails"];
        GVBAddress.DataBind();
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void gv_cancelB_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["BillDetails"] as DataTable;
        ViewState["BillDetails"] = dt;
        GVBAddress.EditIndex = -1;
        GVBAddress.DataSource = (DataTable)ViewState["BillDetails"];
        GVBAddress.DataBind();
    }

    //protected void GVBAddress_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow) // Only for edit mode
    //    {
    //        DropDownList ddlBState = (DropDownList)e.Row.FindControl("ddlBState");

    //        if (ddlBState != null)
    //        {
    //            SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn);
    //            DataTable dt = new DataTable();
    //            ad.Fill(dt);
    //            if (dt.Rows.Count > 0)
    //            {
    //                ddlBState.DataSource = dt;
    //                ddlBState.DataValueField = "statecode";
    //                ddlBState.DataTextField = "statename";
    //                ddlBState.DataBind();
    //                ddlBState.Items.Insert(0, "-- select state --");
    //            }

    //           // Optionally, set the selected value if available
    //            string currentState = ((Label)e.Row.FindControl("lblBillState")).Text; // Current state from the grid
    //            if (!string.IsNullOrEmpty(currentState))
    //            {
    //                ddlBState.SelectedValue = currentState; // Set the selected value in the dropdown
    //            }
    //        }
    //    }
    //}
    protected void GVBAddress_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) // Only for edit mode
        {
            LinkButton lnkedit = e.Row.FindControl("btn_editB") as LinkButton;
            if (lnkedit == null)
            {
                string state = DataBinder.Eval(e.Row.DataItem, "BillState") as string;
                DropDownList ddlBState1 = e.Row.FindControl("ddlBState1") as DropDownList;
                if (state != null)
                {
                    try
                    {

                        using (SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn))
                        {
                            DataTable dt = new DataTable();
                            ad.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                ddlBState1.DataSource = dt;
                                ddlBState1.DataValueField = "statename";
                                ddlBState1.DataTextField = "statename";
                                ddlBState1.DataBind();
                                ddlBState1.Items.Insert(0, new ListItem(state, ""));
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error: " + ex.Message);
                    }

                    ListItem selectedItem = ddlBState1.Items.FindByText(state); // Find by state name
                    if (selectedItem != null)
                    {
                        ddlBState1.SelectedValue = selectedItem.Value; // Set the selected value
                    }
                }
            }
            else
            {

                Label currentState = (e.Row.FindControl("lblBillState") as Label);

                if (currentState.Text != null)
                {
                    try
                    {
                        using (SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn))
                        {
                            DataTable dt = new DataTable();
                            ad.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                ddlBState.DataSource = dt;
                                ddlBState.DataValueField = "statecode";
                                ddlBState.DataTextField = "statename";
                                ddlBState.DataBind();
                                ddlBState.Items.Insert(0, new ListItem("-- select state --", ""));
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error: " + ex.Message);
                    }

                    // Optionally, set the selected value if available
                    // Label currentState = (e.Row.FindControl("hdnBillState") as Label);  // Current state from the grid
                    if (!string.IsNullOrEmpty(currentState.Text))
                    {
                        //    // Only set the selected value if it exists in the dropdown list
                        ListItem selectedItem = ddlBState.Items.FindByText(currentState.Text);
                        if (selectedItem != null)
                        {
                            ddlBState.SelectedItem.Text = currentState.Text; // Set the selected value in the dropdown
                        }
                    }
                }
            }
        }
    }

    //Shipping Address Methods added by 18022025
    protected void txtSbtnAdd_Click(object sender, EventArgs e)
    {
        if (txtSAddress.Text == "" || txtSLocation.Text == "" || txtSPincode.Text == "" || ddlSState.SelectedValue == "0" || txtSGST.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill all details !!!');", true);
        }
        else
        {
            DataTable Dt = (DataTable)ViewState["ShipDetails"];
            Dt.Rows.Add(ViewState["RowNo"], txtSAddress.Text.Trim(), txtSLocation.Text.Trim(), txtSPincode.Text.Trim(), ddlSState.SelectedItem.Text.Trim(), txtSGST.Text.Trim());
            ViewState["ShipDetails"] = Dt;
            txtSAddress.Text = string.Empty;
            txtSLocation.Text = string.Empty;
            txtSPincode.Text = string.Empty;
            txtSGST.Text = string.Empty;
            ddlSState.SelectedItem.Text = string.Empty;
            GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"];
            GVSAddress.DataBind();
        }
    }
    protected void GVSAddress_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GVSAddress.EditIndex = e.NewEditIndex;
        GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"];
        GVSAddress.DataBind();
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void GVSAddress_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow) // Only for edit mode
        {
            LinkButton lnkedit = e.Row.FindControl("btn_editS") as LinkButton;
            if (lnkedit == null)
            {
                string state = DataBinder.Eval(e.Row.DataItem, "ShipState") as string;
                DropDownList ddlSState1 = e.Row.FindControl("ddlSState1") as DropDownList;
                if (state != null)
                {
                    try
                    {

                        using (SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn))
                        {
                            DataTable dt = new DataTable();
                            ad.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                ddlSState1.DataSource = dt;
                                ddlSState1.DataValueField = "statecode";
                                ddlSState1.DataTextField = "statename";
                                ddlSState1.DataBind();
                                ddlSState1.Items.Insert(0, new ListItem(state, ""));
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error: " + ex.Message);
                    }

                    ListItem selectedItem = ddlSState1.Items.FindByText(state); // Find by state name
                    if (selectedItem != null)
                    {
                        ddlSState1.SelectedValue = selectedItem.Value; // Set the selected value
                    }
                }
            }
            else
            {

                Label currentState = (e.Row.FindControl("lblShipState") as Label);

                if (currentState.Text != null)
                {
                    try
                    {
                        using (SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM [tbl_States]", Cls_Main.Conn))
                        {
                            DataTable dt = new DataTable();
                            ad.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                ddlSState.DataSource = dt;
                                ddlSState.DataValueField = "statecode";
                                ddlSState.DataTextField = "statename";
                                ddlSState.DataBind();
                                ddlSState.Items.Insert(0, new ListItem("-- select state --", ""));
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Error: " + ex.Message);
                    }

                    // Optionally, set the selected value if available
                    // Label currentState = (e.Row.FindControl("hdnBillState") as Label);  // Current state from the grid
                    if (!string.IsNullOrEmpty(currentState.Text))
                    {
                        //    // Only set the selected value if it exists in the dropdown list
                        ListItem selectedItem = ddlSState.Items.FindByText(currentState.Text);
                        if (selectedItem != null)
                        {
                            ddlSState.SelectedItem.Text = currentState.Text; // Set the selected value in the dropdown
                        }
                    }
                }
            }
        }
    }

    protected void lnkbtnDeleteS_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["ShipDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["ShipDetails"] = dt;
        GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"];
        GVSAddress.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Address Delete Succesfully !!!');", true);

    }

    protected void gv_cancelS_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["ShipDetails"] as DataTable;
        ViewState["ShipDetails"] = dt;
        GVSAddress.EditIndex = -1;
        GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"];
        GVSAddress.DataBind();
    }

    protected void gv_updateS_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string state = string.Empty;
        string txtSSAddress = ((TextBox)row.FindControl("txtShipAddress")).Text;
        string txtSSLocation = ((TextBox)row.FindControl("txtShipLocation")).Text;
        string txtSSPincode = ((TextBox)row.FindControl("txtShipPincode")).Text;
        string txtSSGST = ((TextBox)row.FindControl("txtSGSTno")).Text;
        string ddlSState1 = ((DropDownList)row.FindControl("ddlSState1")).Text;
        string txthdnstate = ((HiddenField)row.FindControl("lblShipStatehdn")).Value;
        DataTable Dt = ViewState["ShipDetails"] as DataTable;
        if (ddlSState1 == "")
        {
            state = txthdnstate;
        }
        else
        {
            state = ddlSState1;
        }

        Dt.Rows[row.RowIndex]["ShippingAddress"] = txtSSAddress;
        Dt.Rows[row.RowIndex]["ShipLocation"] = txtSSLocation;
        Dt.Rows[row.RowIndex]["ShipPincode"] = txtSSPincode;
        Dt.Rows[row.RowIndex]["ShipState"] = state;
        Dt.Rows[row.RowIndex]["ShipGSTNo"] = txtSSGST;
        Dt.AcceptChanges();
        ViewState["ShipDetails"] = Dt;
        GVSAddress.EditIndex = -1;
        GVSAddress.DataSource = (DataTable)ViewState["ShipDetails"];
        GVSAddress.DataBind();

    }


}