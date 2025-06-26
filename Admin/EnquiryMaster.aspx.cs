
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_EnquiryMaster : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    DataTable Dt_Product = new DataTable();
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
                ViewState["RowNo"] = 0;
                FillddlProduct();
                FillddlState();
                Dt_Product.Columns.AddRange(new DataColumn[17] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
                ViewState["PurchaseOrderProduct"] = Dt_Product;
                ComCode = string.Empty; ComCodeUpdate = string.Empty; regdate = string.Empty;
                //if (Request.QueryString["ID"] != null)
                //{
                //    GetCompanyData(objcls.Decrypt(Request.QueryString["ID"].ToString()));
                //    //GetCompanyDataByName(objcls.Decrypt(Request.QueryString["ID"].ToString()));
                //}
                FillddlCompanyname();
                if (Request.QueryString["code"] != null)
                {
                    ViewState["UpdateRowId"] = objcls.Decrypt(Request.QueryString["code"].ToString());
                    if (!string.IsNullOrEmpty(ViewState["UpdateRowId"].ToString()))
                    {
                        GetCompanyData(ViewState["UpdateRowId"].ToString());
                    }

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
            ddlBStateCode.DataSource = dt;
            ddlBStateCode.DataValueField = "StateCode";
            ddlBStateCode.DataTextField = "StateName";
            ddlBStateCode.DataBind();
            ddlBStateCode.Items.Insert(0, "-- Select State --");

        }
    }
    static string regdate = string.Empty;
    protected void GetCompanyData(string id)
    {
        string query1 = string.Empty;
        query1 = "SELECT E.[id],EnqCode,[ccode],[cname], [filepath1],[filepath2],[filepath3], [filepath4],[filepath5],[remark], Sample,SampleDate,BillingAddress,Billinglocation,PrimaryEmailID,ContactNo,Ownername,Billing_statecode FROM [tbl_EnquiryData] AS E INNER JOIN tbl_CompanyMaster AS C ON C.CompanyCode=E.ccode where E.id='" + id + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            string sample = dt.Rows[0]["Sample"].ToString();
            if (sample == "True")
            {
                rdsample.SelectedValue = "1";
                divsampledate.Visible = true;
                DateTime ffff1 = Convert.ToDateTime(dt.Rows[0]["SampleDate"].ToString());
                txtSampledate.Text = ffff1.ToString("yyyy-MM-dd");
            }
            else
            {
                rdsample.SelectedValue = "0";
                divsampledate.Visible = false;
            }

            txtcompanyname.Text = dt.Rows[0]["cname"].ToString();
            txtownname.Text = dt.Rows[0]["Ownername"].ToString();
            txtcontactno.Text = dt.Rows[0]["ContactNo"].ToString();
            txtarea.Text = dt.Rows[0]["Billinglocation"].ToString();
            txtaddress.Text = dt.Rows[0]["BillingAddress"].ToString();
            CompanycodeID.Value = dt.Rows[0]["ccode"].ToString();
            txtmail.Text = dt.Rows[0]["PrimaryEmailID"].ToString();
               ddlBStateCode.SelectedValue = dt.Rows[0]["Billing_statecode"].ToString();
            HFccode.Value = dt.Rows[0]["ccode"].ToString();
            txtremark.Text = dt.Rows[0]["remark"].ToString();
            HFfile1.Value = dt.Rows[0]["filepath1"].ToString();
            if (!string.IsNullOrEmpty(dt.Rows[0]["filepath1"].ToString()))
            {
                string a1 = dt.Rows[0]["filepath1"].ToString().Remove(0, 13);// "Has file";
                lblfile1.Text = a1.Remove(a1.Length - 18, 18) + "...";
                ImageButtonfile1.Visible = true;
            }
            else
            {
                ImageButtonfile1.Visible = false;
                lblfile1.Text = "file not available";
            }

            HFfile2.Value = dt.Rows[0]["filepath2"].ToString();
            if (!string.IsNullOrEmpty(dt.Rows[0]["filepath2"].ToString()))
            {
                ImageButtonfile2.Visible = true;
                string a1 = dt.Rows[0]["filepath2"].ToString().Remove(0, 13);// "Has file";
                lblfile2.Text = a1.Remove(a1.Length - 18, 18) + "...";
            }
            else
            {
                ImageButtonfile2.Visible = false;
                lblfile2.Text = "file not available";
            }
            HFfile3.Value = dt.Rows[0]["filepath3"].ToString();
            if (!string.IsNullOrEmpty(dt.Rows[0]["filepath3"].ToString()))
            {
                ImageButtonfile3.Visible = true;
                string a1 = dt.Rows[0]["filepath3"].ToString().Remove(0, 13);// "Has file";
                lblfile3.Text = a1.Remove(a1.Length - 18, 18) + "...";
            }
            else
            {
                ImageButtonfile3.Visible = false;
                lblfile3.Text = "file not available";
            }
            HFfile4.Value = dt.Rows[0]["filepath4"].ToString();
            if (!string.IsNullOrEmpty(dt.Rows[0]["filepath4"].ToString()))
            {
                ImageButtonfile4.Visible = true;
                string a1 = dt.Rows[0]["filepath4"].ToString().Remove(0, 13);// "Has file";
                lblfile4.Text = a1.Remove(a1.Length - 18, 18) + "...";
            }
            else
            {
                ImageButtonfile4.Visible = false;
                lblfile4.Text = "file not available";
            }
            HFfile5.Value = dt.Rows[0]["filepath5"].ToString();
            if (!string.IsNullOrEmpty(dt.Rows[0]["filepath5"].ToString()))
            {
                ImageButtonfile5.Visible = true;
                string a1 = dt.Rows[0]["filepath5"].ToString().Remove(0, 13);// "Has file";
                lblfile5.Text = a1.Remove(a1.Length - 18, 18) + "...";
            }
            else
            {
                ImageButtonfile5.Visible = false;
                lblfile5.Text = "file not available";
            }
            btnadd.Text = "Update";
            string dtlsid= dt.Rows[0]["EnqCode"].ToString();
            ShowDtlEdit(dtlsid);
        }
    }

    protected void ShowDtlEdit(string ID)
    {
        divTotalPart.Visible = true;
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT Productname,Description,HSN,Quantity,Units,Rate,Total,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Discountpercentage,DiscountAmount,Alltotal FROM [tbl_ProformaInvoiceDtls] WHERE Invoiceno='" + txtinvoiceno.Text + "'", Cls_Main.Conn);
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_EnquiryDtls] WHERE Invoiceno='" + ID + "'", Cls_Main.Conn);
        DataTable DTCOMP = new DataTable();
        Da.Fill(DTCOMP);

        int count = 0;
        if (DTCOMP.Rows.Count > 0)
        {
            if (Dt_Product.Columns.Count < 0)
            {
                Show_Grid();
            }

            for (int i = 0; i < DTCOMP.Rows.Count; i++)
            {
                Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
                count = count + 1;
            }
        }

        dgvMachineDetails.EmptyDataText = "No Data Found";
        dgvMachineDetails.DataSource = Dt_Product;
        dgvMachineDetails.DataBind();
    }

    protected void GetCompanyDataByName(string cname)
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT top 1 ID,CompanyCode,Companyname FROM [tbl_CompanyMaster] where [isdeleted]=0 and Companyname='" + cname.Trim() + "' order by ID desc ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            // ddlcompanyname.SelectedItem.Text = cname;
            btnadd.Enabled = true;
            //lblmsg.Visible = false;
            HFccode.Value = dt.Rows[0]["CompanyCode"].ToString();
        }
        else
        {
            // lblmsg.Visible = true;
            // lblmsg.Text = "Company Not found in our data base, Please add company first !!";
            btnadd.Enabled = false;
        }
    }

    static string UpdateHistorymsg = string.Empty;

    static string ComCode = string.Empty;
    protected void GenerateComCode()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([id]) as maxid FROM [tbl_EnquiryData]", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            ComCode = "PAPL/ENQ-" + (maxid + 1).ToString();
        }
        else
        {
            ComCode = string.Empty;
        }
    }

    static string ComCodeUpdate = string.Empty; static string visitingcardPath = string.Empty;
    protected void btnadd_Click(object sender, EventArgs e)
    {
        if (rdsample.SelectedValue != null || rdsample.SelectedValue != "" && ddlBStateCode.SelectedItem.Text!= "-- Select State --")
        {
            string multilineText = txtremark.Text;
            string formattedText = multilineText.Replace("\n", "<br />");
            #region Insert
            if (btnadd.Text == "Add Enquiry")
            {
                GenerateComCode();

                if (!string.IsNullOrEmpty(ComCode))
                {
                    SqlCommand cmd = new SqlCommand("SP_EnquiryData", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "Insert");
                    cmd.Parameters.AddWithValue("@EnqCode", ComCode);
                    if (string.IsNullOrEmpty(CompanycodeID.Value))
                    {
                        CompanyCode();

                    }
                    cmd.Parameters.AddWithValue("@ccode", CompanycodeID.Value);
                    cmd.Parameters.AddWithValue("@cname", txtcompanyname.Text);

                    cmd.Parameters.AddWithValue("@Ownername", txtownname.Text.Trim());
                    cmd.Parameters.AddWithValue("@contactno", txtcontactno.Text.Trim());
                    cmd.Parameters.AddWithValue("@address", txtaddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@remark", formattedText);
                    cmd.Parameters.AddWithValue("@area", txtarea.Text.Trim());
                    cmd.Parameters.AddWithValue("@State", ddlBStateCode.SelectedValue);
                    cmd.Parameters.AddWithValue("@Email", txtmail.Text.Trim());

                    cmd.Parameters.AddWithValue("@Sample", rdsample.SelectedValue);
                    if (txtSampledate.Text == null || txtSampledate.Text == "")
                    {
                        cmd.Parameters.AddWithValue("@SampleDate", DBNull.Value);

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SampleDate", Convert.ToDateTime(txtSampledate.Text));

                    }
                    cmd.Parameters.AddWithValue("@sessionname", Session["UserCode"].ToString());
                    // Delete Contact Details
                    Cls_Main.Conn_Open();
                    SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_CompanyContactDetails WHERE CompanyCode=@CompanyCode", Cls_Main.Conn);
                    cmddelete.Parameters.AddWithValue("@CompanyCode", CompanycodeID.Value);
                    cmddelete.ExecuteNonQuery();
                    Cls_Main.Conn_Close();

                    //Save Contact Details 
                    Cls_Main.Conn_Open();
                    SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_CompanyContactDetails (CompanyCode,Name,Number,EmailID,DOB,Department,Designation,CreatedBy,CreatedOn) VALUES (@CompanyCode,@Name,@Number,@EmailID,@DOB,@Department,@Designation,@CreatedBy,@createdOn)", Cls_Main.Conn);
                    cmdd.Parameters.AddWithValue("@CompanyCode", CompanycodeID.Value);
                    cmdd.Parameters.AddWithValue("@Name", txtownname.Text.Trim());
                    cmdd.Parameters.AddWithValue("@Number", txtcontactno.Text.Trim());
                    cmdd.Parameters.AddWithValue("@EmailID", txtmail.Text.Trim());
                    cmdd.Parameters.AddWithValue("@DOB", DBNull.Value);
                    cmdd.Parameters.AddWithValue("@Department", DBNull.Value);
                    cmdd.Parameters.AddWithValue("@Designation", DBNull.Value);
                    cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    cmdd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();


                    if (FileUpload1.HasFile)
                    {
                        foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
                        {
                            string filename = Path.GetFileName(postedFile.FileName);
                            string[] pdffilename = filename.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];
                            //if (filenameExt == "pdf" || filenameExt == "PDF")
                            //{
                            string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                            postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                            cmd.Parameters.AddWithValue("@filepath1", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                            //}
                            //else
                            //{
                            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                            //}
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@filepath1", DBNull.Value);
                    }

                    if (FileUpload2.HasFile)
                    {
                        foreach (HttpPostedFile postedFile in FileUpload2.PostedFiles)
                        {
                            string filename = Path.GetFileName(postedFile.FileName);
                            string[] pdffilename = filename.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];
                            //if (filenameExt == "pdf" || filenameExt == "PDF")
                            //{
                            string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                            postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                            cmd.Parameters.AddWithValue("@filepath2", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                            //}
                            //else
                            //{
                            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                            //}
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@filepath2", DBNull.Value);
                    }

                    if (FileUpload3.HasFile)
                    {
                        foreach (HttpPostedFile postedFile in FileUpload3.PostedFiles)
                        {
                            string filename = Path.GetFileName(postedFile.FileName);
                            string[] pdffilename = filename.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];
                            //if (filenameExt == "pdf" || filenameExt == "PDF")
                            //{
                            string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                            postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                            cmd.Parameters.AddWithValue("@filepath3", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                            //}
                            //else
                            //{
                            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                            //}
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@filepath3", DBNull.Value);
                    }

                    if (FileUpload4.HasFile)
                    {
                        foreach (HttpPostedFile postedFile in FileUpload4.PostedFiles)
                        {
                            string filename = Path.GetFileName(postedFile.FileName);
                            string[] pdffilename = filename.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];
                            //if (filenameExt == "pdf" || filenameExt == "PDF")
                            //{
                            string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                            postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                            cmd.Parameters.AddWithValue("@filepath4", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                            //}
                            //else
                            //{
                            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                            //}
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@filepath4", DBNull.Value);
                    }

                    if (FileUpload5.HasFile)
                    {
                        foreach (HttpPostedFile postedFile in FileUpload5.PostedFiles)
                        {
                            string filename = Path.GetFileName(postedFile.FileName);
                            string[] pdffilename = filename.Split('.');
                            string pdffilename1 = pdffilename[0];
                            string filenameExt = pdffilename[1];
                            //if (filenameExt == "pdf" || filenameExt == "PDF")
                            //{
                            string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                            postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                            cmd.Parameters.AddWithValue("@filepath5", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                            //}
                            //else
                            //{
                            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                            //}
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@filepath5", DBNull.Value);
                    }



                    int a = 0;
                    cmd.Connection.Open();
                    a = cmd.ExecuteNonQuery();
                    cmd.Connection.Close();
                    //Save Product Details 
                    foreach (GridViewRow grd1 in dgvMachineDetails.Rows)
                    {
                        string lblproduct = (grd1.FindControl("lblproduct") as Label).Text;
                        string lblDescription = (grd1.FindControl("lblDescription") as Label).Text;
                        string lblhsn = (grd1.FindControl("lblhsn") as Label).Text;
                        string lblQuantity = (grd1.FindControl("lblQuantity") as Label).Text;
                        string lblUnit = (grd1.FindControl("lblUnit") as Label).Text;
                        string lblRate = (grd1.FindControl("lblRate") as Label).Text;
                        string lblTotal = (grd1.FindControl("lblTotal") as Label).Text;
                        string lblCGSTPer = (grd1.FindControl("lblCGSTPer") as Label).Text;
                        string lblCGST = (grd1.FindControl("lblCGST") as Label).Text;
                        string lblSGSTPer = (grd1.FindControl("lblSGSTPer") as Label).Text;
                        string lblSGST = (grd1.FindControl("lblSGST") as Label).Text;
                        string lblIGSTPer = (grd1.FindControl("lblIGSTPer") as Label).Text;
                        string lblIGST = (grd1.FindControl("lblIGST") as Label).Text;
                        string lblDiscount = (grd1.FindControl("lblDiscount") as Label).Text;
                        string lblDiscountAmount = (grd1.FindControl("lblDiscountAmount") as Label).Text;
                        string lblAlltotal = (grd1.FindControl("lblAlltotal") as Label).Text;

                        Cls_Main.Conn_Open();
                        SqlCommand cmdd1 = new SqlCommand("INSERT INTO [tbl_EnquiryDtls] (Invoiceno,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn) VALUES(@Invoiceno,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn)", Cls_Main.Conn);
                        cmdd1.Parameters.AddWithValue("@Invoiceno", ComCode);
                        cmdd1.Parameters.AddWithValue("@Productname", lblproduct);
                        cmdd1.Parameters.AddWithValue("@Description", lblDescription);
                        cmdd1.Parameters.AddWithValue("@HSN", lblhsn);
                        cmdd1.Parameters.AddWithValue("@Quantity", lblQuantity);
                        cmdd1.Parameters.AddWithValue("@Units", lblUnit);
                        cmdd1.Parameters.AddWithValue("@Rate", lblRate);
                        cmdd1.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                        cmdd1.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                        cmdd1.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                        cmdd1.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                        cmdd1.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                        cmdd1.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                        cmdd1.Parameters.AddWithValue("@Total", lblTotal);
                        cmdd1.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                        cmdd1.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);
                        cmdd1.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                        cmdd1.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                        cmdd1.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                        cmdd1.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                    }
                    if (a > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('Enquiry Added Sucessfully');window.location='EnquiryList.aspx';", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Not Saved !!');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Enquiry Code Generation Problem Please Try Again !!');", true);
                }
            }
            #endregion Insert

            #region Update
            if (btnadd.Text == "Update")
            {
                //GenerateComCode();
                //if (!string.IsNullOrEmpty(ComCode))
                //{

                SqlCommand cmd = new SqlCommand("SP_EnquiryData", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "Update");
                cmd.Parameters.AddWithValue("@id", ViewState["UpdateRowId"].ToString());
                //cmd.Parameters.AddWithValue("@EnqCode", ComCode);
                if (string.IsNullOrEmpty(CompanycodeID.Value))
                {
                    CompanyCode();

                }
                cmd.Parameters.AddWithValue("@ccode", CompanycodeID.Value);
                cmd.Parameters.AddWithValue("@cname", txtcompanyname.Text);
                cmd.Parameters.AddWithValue("@Ownername", txtownname.Text.Trim());
                cmd.Parameters.AddWithValue("@contactno", txtcontactno.Text.Trim());
                cmd.Parameters.AddWithValue("@address", txtaddress.Text.Trim());
                cmd.Parameters.AddWithValue("@remark", formattedText);
                cmd.Parameters.AddWithValue("@area", txtarea.Text.Trim());
                cmd.Parameters.AddWithValue("@State", ddlBStateCode.SelectedValue);
                cmd.Parameters.AddWithValue("@Email", txtmail.Text.Trim());
                cmd.Parameters.AddWithValue("@Sample", rdsample.SelectedValue);
                //  cmd.Parameters.AddWithValue("@ccode", ddlcompanyname.SelectedValue);
                if (txtSampledate.Text != null && txtSampledate.Text != "")
                {
                    cmd.Parameters.AddWithValue("@SampleDate", Convert.ToDateTime(txtSampledate.Text));
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SampleDate", DBNull.Value);
                }
                // Delete Contact Details
                Cls_Main.Conn_Open();
                SqlCommand cmddelete = new SqlCommand("DELETE FROM tbl_CompanyContactDetails WHERE CompanyCode=@CompanyCode", Cls_Main.Conn);
                cmddelete.Parameters.AddWithValue("@CompanyCode", CompanycodeID.Value);
                cmddelete.ExecuteNonQuery();
                Cls_Main.Conn_Close();

                //Save Contact Details  
                Cls_Main.Conn_Open();
                SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_CompanyContactDetails (CompanyCode,Name,Number,EmailID,DOB,Department,Designation,CreatedBy,CreatedOn) VALUES (@CompanyCode,@Name,@Number,@EmailID,@DOB,@Department,@Designation,@CreatedBy,@createdOn)", Cls_Main.Conn);
                cmdd.Parameters.AddWithValue("@CompanyCode", CompanycodeID.Value);
                cmdd.Parameters.AddWithValue("@Name", txtownname.Text.Trim());
                cmdd.Parameters.AddWithValue("@Number", txtcontactno.Text.Trim());
                cmdd.Parameters.AddWithValue("@EmailID", txtmail.Text.Trim());
                cmdd.Parameters.AddWithValue("@DOB", DBNull.Value);
                cmdd.Parameters.AddWithValue("@Department", DBNull.Value);
                cmdd.Parameters.AddWithValue("@Designation", DBNull.Value);
                cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                cmdd.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                //  cmd.Parameters.AddWithValue("@cname", ddlcompanyname.SelectedItem.Text);
                // cmd.Parameters.AddWithValue("@remark", txtremark.Text.Trim());
                //cmd.Parameters.AddWithValue("@sessionname", Session["empcode"].ToString());
                if (FileUpload1.HasFile)
                {
                    foreach (HttpPostedFile postedFile in FileUpload1.PostedFiles)
                    {
                        string filename = Path.GetFileName(postedFile.FileName);
                        string[] pdffilename = filename.Split('.');
                        string pdffilename1 = pdffilename[0];
                        string filenameExt = pdffilename[1];
                        //if (filenameExt == "pdf" || filenameExt == "PDF")
                        //{
                        string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                        postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                        cmd.Parameters.AddWithValue("@filepath1", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                        //}
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@filepath1", HFfile1.Value);
                }

                if (FileUpload2.HasFile)
                {
                    foreach (HttpPostedFile postedFile in FileUpload2.PostedFiles)
                    {
                        string filename = Path.GetFileName(postedFile.FileName);
                        string[] pdffilename = filename.Split('.');
                        string pdffilename1 = pdffilename[0];
                        string filenameExt = pdffilename[1];
                        //if (filenameExt == "pdf" || filenameExt == "PDF")
                        //{
                        string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                        postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                        cmd.Parameters.AddWithValue("@filepath2", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                        //}
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@filepath2", HFfile2.Value);
                }

                if (FileUpload3.HasFile)
                {
                    foreach (HttpPostedFile postedFile in FileUpload3.PostedFiles)
                    {
                        string filename = Path.GetFileName(postedFile.FileName);
                        string[] pdffilename = filename.Split('.');
                        string pdffilename1 = pdffilename[0];
                        string filenameExt = pdffilename[1];
                        //if (filenameExt == "pdf" || filenameExt == "PDF")
                        //{
                        string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                        postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                        cmd.Parameters.AddWithValue("@filepath3", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                        //}
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@filepath3", HFfile3.Value);
                }

                if (FileUpload4.HasFile)
                {
                    foreach (HttpPostedFile postedFile in FileUpload4.PostedFiles)
                    {
                        string filename = Path.GetFileName(postedFile.FileName);
                        string[] pdffilename = filename.Split('.');
                        string pdffilename1 = pdffilename[0];
                        string filenameExt = pdffilename[1];
                        //if (filenameExt == "pdf" || filenameExt == "PDF")
                        //{
                        string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                        postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                        cmd.Parameters.AddWithValue("@filepath4", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                        //}
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@filepath4", HFfile4.Value);
                }

                if (FileUpload5.HasFile)
                {
                    foreach (HttpPostedFile postedFile in FileUpload5.PostedFiles)
                    {
                        string filename = Path.GetFileName(postedFile.FileName);
                        string[] pdffilename = filename.Split('.');
                        string pdffilename1 = pdffilename[0];
                        string filenameExt = pdffilename[1];
                        //if (filenameExt == "pdf" || filenameExt == "PDF")
                        //{
                        string time1 = DateTime.Now.ToString("ddmmyyyyttmmss");
                        postedFile.SaveAs(Server.MapPath("~/EnquiryFiles/") + pdffilename1 + time1 + "." + filenameExt);
                        cmd.Parameters.AddWithValue("@filepath5", "EnquiryFiles/" + pdffilename1 + time1 + "." + filenameExt);
                        //}
                        //else
                        //{
                        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please select a pdf file only !!');", true);
                        //}
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@filepath5", HFfile5.Value);
                }

                int a = 0;
                cmd.Connection.Open();
                a = cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                Cls_Main.Conn_Open();
                SqlCommand cmddelete1 = new SqlCommand("DELETE FROM tbl_EnquiryDtls WHERE Invoiceno=@Invoiceno", Cls_Main.Conn);
                cmddelete1.Parameters.AddWithValue("@Invoiceno", ComCode);
                cmddelete1.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                //Save Product Details 
                foreach (GridViewRow grd1 in dgvMachineDetails.Rows)
                {
                    string lblproduct = (grd1.FindControl("lblproduct") as Label).Text;
                    string lblDescription = (grd1.FindControl("lblDescription") as Label).Text;
                    string lblhsn = (grd1.FindControl("lblhsn") as Label).Text;
                    string lblQuantity = (grd1.FindControl("lblQuantity") as Label).Text;
                    string lblUnit = (grd1.FindControl("lblUnit") as Label).Text;
                    string lblRate = (grd1.FindControl("lblRate") as Label).Text;
                    string lblTotal = (grd1.FindControl("lblTotal") as Label).Text;
                    string lblCGSTPer = (grd1.FindControl("lblCGSTPer") as Label).Text;
                    string lblCGST = (grd1.FindControl("lblCGST") as Label).Text;
                    string lblSGSTPer = (grd1.FindControl("lblSGSTPer") as Label).Text;
                    string lblSGST = (grd1.FindControl("lblSGST") as Label).Text;
                    string lblIGSTPer = (grd1.FindControl("lblIGSTPer") as Label).Text;
                    string lblIGST = (grd1.FindControl("lblIGST") as Label).Text;
                    string lblDiscount = (grd1.FindControl("lblDiscount") as Label).Text;
                    string lblDiscountAmount = (grd1.FindControl("lblDiscountAmount") as Label).Text;
                    string lblAlltotal = (grd1.FindControl("lblAlltotal") as Label).Text;

                    Cls_Main.Conn_Open();
                    SqlCommand cmdd1 = new SqlCommand("INSERT INTO [tbl_EnquiryDtls] (Invoiceno,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn) VALUES(@Invoiceno,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn)", Cls_Main.Conn);
                    cmdd1.Parameters.AddWithValue("@Invoiceno", ComCode);
                    cmdd1.Parameters.AddWithValue("@Productname", lblproduct);
                    cmdd1.Parameters.AddWithValue("@Description", lblDescription);
                    cmdd1.Parameters.AddWithValue("@HSN", lblhsn);
                    cmdd1.Parameters.AddWithValue("@Quantity", lblQuantity);
                    cmdd1.Parameters.AddWithValue("@Units", lblUnit);
                    cmdd1.Parameters.AddWithValue("@Rate", lblRate);
                    cmdd1.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                    cmdd1.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                    cmdd1.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                    cmdd1.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                    cmdd1.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                    cmdd1.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                    cmdd1.Parameters.AddWithValue("@Total", lblTotal);
                    cmdd1.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                    cmdd1.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);
                    cmdd1.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                    cmdd1.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    cmdd1.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmdd1.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                }
                if (a > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('Enquiry Updated Sucessfully');window.location='EnquiryList.aspx';", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Not Updated !!');", true);
                }
                //}
                //else
                //{
                //    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Enquiry Code Generation Problem Please Try Again !!');", true);
                //}
            }
            #endregion Update
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please Fill all details !!');", true);
        }
    }

    private void FillddlCompanyname()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT CompanyCode,[Companyname] FROM [tbl_CompanyMaster] WHERE IsDeleted = '0'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            //ddlcompanyname.DataSource = dt;
            //ddlcompanyname.DataValueField = "CompanyCode";
            //ddlcompanyname.DataTextField = "Companyname";
            //ddlcompanyname.DataBind();
            //ddlcompanyname.Items.Insert(0, " --- Select Company --- ");
        }
    }
    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryList.aspx");
    }

    //protected void txtcname_TextChanged(object sender, EventArgs e)
    //{
    //    GetCompanyDataByName(txtcname.Text);
    //}

    //[System.Web.Script.Services.ScriptMethod()]
    //[System.Web.Services.WebMethod]
    //public static List<string> GetCompanyList(string prefixText, int count)
    //{
    //    return AutoFillCompanyName(prefixText);
    //}

    //public static List<string> AutoFillCompanyName(string prefixText)
    //{
    //    using (SqlConnection con = new SqlConnection())
    //    {
    //        con.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

    //        using (SqlCommand com = new SqlCommand())
    //        {
    //            com.CommandText = "Select DISTINCT  Companyname from [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and [isdeleted]=0";

    //            com.Parameters.AddWithValue("@Search", prefixText);
    //            com.Connection = con;
    //            con.Open();
    //            List<string> countryNames = new List<string>();
    //            using (SqlDataReader sdr = com.ExecuteReader())
    //            {
    //                while (sdr.Read())
    //                {
    //                    countryNames.Add(sdr["cname"].ToString());
    //                }
    //            }
    //            con.Close();
    //            return countryNames;
    //        }
    //    }
    //}


    protected void Deletefile(string id, string fileno)
    {
        SqlCommand cmd = new SqlCommand("SP_EnquiryData", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", "UpdateFile");
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@fileno", fileno);
        int a = 0;
        cmd.Connection.Open();
        a = cmd.ExecuteNonQuery();
        cmd.Connection.Close();
        if (a > 0)
        {
            Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Sucess", "alert('Enquiry Updated Sucessfully');window.location='Addenquiry.aspx';", true);
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('File Not Deleted !!');", true);
        }

    }

    protected void ImageButtonfile1_Click(object sender, ImageClickEventArgs e)
    {
        Deletefile(ViewState["UpdateRowId"].ToString(), "1");
    }

    protected void ImageButtonfile2_Click(object sender, ImageClickEventArgs e)
    {
        Deletefile(ViewState["UpdateRowId"].ToString(), "2");
    }

    protected void ImageButtonfile3_Click(object sender, ImageClickEventArgs e)
    {
        Deletefile(ViewState["UpdateRowId"].ToString(), "3");
    }

    protected void ImageButtonfile4_Click(object sender, ImageClickEventArgs e)
    {
        Deletefile(ViewState["UpdateRowId"].ToString(), "4");
    }

    protected void ImageButtonfile5_Click(object sender, ImageClickEventArgs e)
    {
        Deletefile(ViewState["UpdateRowId"].ToString(), "5");
    }


    protected void rdsample_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdsample.SelectedValue == "1")
        {
            divsampledate.Visible = true;
        }
        else
        {
            divsampledate.Visible = false;

        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryList.aspx");
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

    protected void txtcompanyname_TextChanged(object sender, EventArgs e)
    {
        try
        {
            SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_CompanyMaster AS CM LEFT join tbl_CompanyContactDetails AS CCD on CM.CompanyCode=CCD.CompanyCode WHERE Companyname='" + txtcompanyname.Text + "'", con);
            DataTable Dt = new DataTable();
            Da.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {


                txtownname.Text = Dt.Rows[0]["Name"].ToString();
                txtcontactno.Text = Dt.Rows[0]["Number"].ToString();
                txtarea.Text = Dt.Rows[0]["Billinglocation"].ToString();
                txtaddress.Text = Dt.Rows[0]["BillingAddress"].ToString();
                CompanycodeID.Value = Dt.Rows[0]["CompanyCode"].ToString();
                txtmail.Text = Dt.Rows[0]["EmailID"].ToString();



            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }
    protected void CompanyCode()
    {

        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tbl_CompanyMaster]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            CompanycodeID.Value = "WLSPL/CMP-" + (maxid + 1).ToString();
        }
        else
        {
            CompanycodeID.Value = string.Empty;
        }
    }


    //Search Part name methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetPartList(string prefixText, int count)
    {
        return AutoFillPartName(prefixText);
    }

    public static List<string> AutoFillPartName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster] where " + "ProductName like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> PartNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        PartNames.Add(sdr["ProductName"].ToString());
                    }
                }
                con.Close();
                return PartNames;
            }
        }
    }

    protected void ddlProduct_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ProductMaster] WHERE ProductName='" + ddlProduct.SelectedItem.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            txtdescription.Text = Dt.Rows[0]["Description"].ToString();
            txthsnsac.Text = Dt.Rows[0]["HSN"].ToString();
            txtrate.Text = Dt.Rows[0]["Price"].ToString();
            txtunit.Text = Dt.Rows[0]["Unit"].ToString();
     
            if(ddlBStateCode.SelectedValue=="27")
            {
                txtCGST.Text = "9";
                txtSGST.Text = "9";
                txtIGST.Text = "0";
            }
            else
            {
                txtCGST.Text = "0";
                txtSGST.Text = "0";
                txtIGST.Text = "18";
            }
          
           
            txtCGSTamt.Text = "0.00";
            txtSGSTamt.Text = "0.00";
            txtIGSTamt.Text = "0.00";
            txtdiscount.Text = "0.00";
            txtdiscountamt.Text = "0.00";
            ddlProduct.Focus();
        }
    }

    protected void txtquantity_TextChanged(object sender, EventArgs e)
    {
        int value;
        if (int.TryParse(txtquantity.Text, out value))
        {
            // Check if the value is a multiple of 25
            if (value % 25 == 0)
            {
                var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());
                txttotal.Text = Convert.ToString(TotalAmt);
                decimal total;
                decimal Percentage;
                if (txtIGST.Text == null || txtIGST.Text == "" || txtIGST.Text == "0")
                {
                    Percentage = Convert.ToDecimal(txtCGST.Text);
                    total = (TotalAmt * Percentage / 100);

                    txtCGSTamt.Text = total.ToString();

                    txtSGSTamt.Text = txtCGSTamt.Text;

                    txtSGST.Text = txtCGST.Text;
                    var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
                    txtgrandtotal.Text = GrandTotal.ToString();
                }
                else
                {
                    Percentage = Convert.ToDecimal(txtIGST.Text);
                    total = (TotalAmt * Percentage / 100);

                    txtIGSTamt.Text = total.ToString();
                    var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtIGSTamt.Text.Trim());
                    txtgrandtotal.Text = GrandTotal.ToString();
                }
            }
            else
            {
                txtquantity.Text = "";
                txtgrandtotal.Text = "";
                txttotal.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a Quantity that is a multiple of 25...!!')", true);

            }
        }
    }

    protected void txtCGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtCGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtCGSTamt.Text = total.ToString();

        txtSGSTamt.Text = txtCGSTamt.Text;

        txtSGST.Text = txtCGST.Text;

        var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
        txtgrandtotal.Text = GrandTotal.ToString();


        if (txtCGST.Text == "0" || txtCGST.Text == "")
        {
            txtIGST.Enabled = true;
            txtIGST.Text = "0";
        }
        else
        {
            txtIGST.Enabled = false;
            txtIGST.Text = "0";
        }
    }

    protected void txtSGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtSGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtSGSTamt.Text = total.ToString();

        txtCGSTamt.Text = txtSGSTamt.Text;

        txtCGST.Text = txtSGST.Text;

        var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
        txtgrandtotal.Text = GrandTotal.ToString();

        if (txtSGST.Text == "0" || txtSGST.Text == "")
        {
            txtIGST.Enabled = true;
            txtIGST.Text = "0";
        }
        else
        {
            txtIGST.Enabled = false;
            txtIGST.Text = "0";
        }
    }

    protected void txtIGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtIGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtIGSTamt.Text = total.ToString();

        var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtIGSTamt.Text.Trim());
        txtgrandtotal.Text = GrandTotal.ToString();


        if (txtIGST.Text == "0" || txtIGST.Text == "")
        {
            txtCGST.Enabled = true;
            txtCGST.Text = "0";
            txtSGST.Enabled = true;
            txtSGST.Text = "0";
        }
        else
        {
            txtCGST.Enabled = false;
            txtCGST.Text = "0";
            txtSGST.Enabled = false;
            txtSGST.Text = "0";
        }
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        decimal DiscountAmt;
        decimal val1 = Convert.ToDecimal(txtgrandtotal.Text);
        decimal val2 = Convert.ToDecimal(txtdiscount.Text);
        DiscountAmt = (val1 * val2 / 100);
        txtgrandtotal.Text = (val1 - DiscountAmt).ToString();

        txtdiscountamt.Text = DiscountAmt.ToString();
    }

    protected void btnAddMore_Click(object sender, EventArgs e)
    {
        if (txtquantity.Text == "" || txtrate.Text == "" || txtgrandtotal.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill Quantity and Price !!!');", true);
            txtquantity.Focus();
        }
        else
        {
            Show_Grid();
        }
    }
    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT[ProductName] FROM [tbl_ProductMaster] where status=1 AND isdeleted=0 ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlProduct.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            ddlProduct.DataTextField = "ProductName";
            ddlProduct.DataBind();
            ddlProduct.Items.Insert(0, "-- Select Product --");
        }
    }
    private void Show_Grid()
    {
        divTotalPart.Visible = true;

        DataTable Dt = (DataTable)ViewState["PurchaseOrderProduct"];
        Dt.Rows.Add(ViewState["RowNo"], ddlProduct.SelectedItem.Text, txtdescription.Text.Trim(), txthsnsac.Text.Trim(), txtquantity.Text, txtunit.Text, txtrate.Text, txttotal.Text, txtCGST.Text, txtCGSTamt.Text, txtSGST.Text, txtSGSTamt.Text, txtIGST.Text, txtIGSTamt.Text, txtdiscount.Text, txtdiscountamt.Text, txtgrandtotal.Text);
        ViewState["PurchaseOrderProduct"] = Dt;
        FillddlProduct();
        txtdescription.Text = string.Empty;
        txthsnsac.Text = string.Empty;
        txtquantity.Text = string.Empty;
        txtunit.Text = string.Empty;
        txtrate.Text = string.Empty;
        txttotal.Text = string.Empty;
        txtCGST.Text = string.Empty;
        txtCGSTamt.Text = string.Empty;
        txtSGST.Text = string.Empty;
        txtSGSTamt.Text = string.Empty;
        txtIGST.Text = string.Empty;
        txtIGSTamt.Text = string.Empty;
        txtdiscount.Text = string.Empty;
        txtdiscountamt.Text = string.Empty;
        txtgrandtotal.Text = string.Empty;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
    }

    protected void dgvMachineDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvMachineDetails.EditIndex = e.NewEditIndex;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    string lblTotal, lblCGST, lblSGST;
    string lblproduct, lblproducttype, lblsubtype, lblproductbrand, lblton, Description, hsn, Quantity, Unit, Rate, subTotal, CGSTPer, CGST, SGSTPer, SGST, IGSTPer, IGST, Discount, lblDiscountAmount, Grandtotal;
    private decimal Total, CGSTAmt, SGSTAmt, IGSTAmt, Alltotal;
    protected void dgvMachineDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkedit = e.Row.FindControl("btn_edit") as LinkButton;
                if (lnkedit == null)
                {

                    lblproduct = (e.Row.FindControl("txtproduct") as TextBox).Text;
                    Description = (e.Row.FindControl("txtDescription") as TextBox).Text;
                    hsn = (e.Row.FindControl("txthsn") as TextBox).Text;
                    Quantity = (e.Row.FindControl("txtQuantity") as TextBox).Text;
                    Unit = (e.Row.FindControl("txtUnit") as TextBox).Text;
                    Rate = (e.Row.FindControl("txtRate") as TextBox).Text;
                    subTotal = (e.Row.FindControl("txtTotal") as TextBox).Text;
                    CGSTPer = (e.Row.FindControl("txtCGSTPer") as TextBox).Text;
                    CGST = (e.Row.FindControl("txtCGST") as TextBox).Text;
                    SGSTPer = (e.Row.FindControl("txtSGSTPer") as TextBox).Text;
                    SGST = (e.Row.FindControl("txtSGST") as TextBox).Text;
                    IGSTPer = (e.Row.FindControl("txtIGSTPer") as TextBox).Text;
                    IGST = (e.Row.FindControl("txtIGST") as TextBox).Text;
                    Discount = (e.Row.FindControl("txtDiscount") as TextBox).Text;
                    lblDiscountAmount = (e.Row.FindControl("txtDiscountAmount") as TextBox).Text;
                    Grandtotal = (e.Row.FindControl("txtAlltotal") as TextBox).Text;

                }
                else
                {
                    Total += Convert.ToDecimal((e.Row.FindControl("lblTotal") as Label).Text);
                    txt_Subtotal.Text = Total.ToString("0.00");

                    CGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblCGST") as Label).Text);
                    txt_cgstamt.Text = CGSTAmt.ToString("0.00");

                    SGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblSGST") as Label).Text);
                    txt_sgstamt.Text = SGSTAmt.ToString("0.00");

                    IGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblIGST") as Label).Text);
                    txt_igstamt.Text = IGSTAmt.ToString("0.00");

                    Alltotal += Convert.ToDecimal((e.Row.FindControl("lblAlltotal") as Label).Text);
                    txt_grandTotal.Text = Alltotal.ToString("0.00");

                    //Amount Convert into word
                    //string number = txt_grandTotal.Text;
                    //number = Convert.ToDouble(number).ToString();
                    //string Amtinword = ConvertNumbertoWords(Convert.ToInt32(number));
                    //lbl_total_amt_Value.Text = Amtinword;


                    string isNegative = "";
                    try
                    {
                        string number = txt_grandTotal.Text;

                        number = Convert.ToDouble(number).ToString();

                        if (number.Contains("-"))
                        {
                            isNegative = "Minus ";
                            number = number.Substring(1, number.Length - 1);
                        }
                        else
                        {
                            lbl_total_amt_Value.Text = isNegative + ConvertToWords(number);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    protected void gv_update_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Product = ((TextBox)row.FindControl("txtproduct")).Text;
        string Description = ((TextBox)row.FindControl("txtDescription")).Text;
        string HSN = ((TextBox)row.FindControl("txthsn")).Text;
        string Quantity = ((TextBox)row.FindControl("txtQuantity")).Text;
        string Unit = ((TextBox)row.FindControl("txtUnit")).Text;
        string Rate = ((TextBox)row.FindControl("txtRate")).Text;
        string Total = ((TextBox)row.FindControl("txtTotal")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGST")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGST")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGST")).Text;
        string Discount = ((TextBox)row.FindControl("txtDiscount")).Text;
        string DiscountAmt = ((TextBox)row.FindControl("txtDiscountAmount")).Text;
        string AllTotal = ((TextBox)row.FindControl("txtAlltotal")).Text;
        DataTable Dt = ViewState["PurchaseOrderProduct"] as DataTable;
        Dt.Rows[row.RowIndex]["Productname"] = Product;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Quantity"] = Quantity;
        Dt.Rows[row.RowIndex]["Units"] = Unit;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Total"] = Total;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["Discountpercentage"] = Discount;
        Dt.Rows[row.RowIndex]["DiscountAmount"] = DiscountAmt;
        Dt.Rows[row.RowIndex]["Alltotal"] = AllTotal;
        Dt.AcceptChanges();
        ViewState["PurchaseOrderProduct"] = Dt;
        dgvMachineDetails.EditIndex = -1;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void gv_cancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Product = ((TextBox)row.FindControl("txtproduct")).Text;
        string Description = ((TextBox)row.FindControl("txtDescription")).Text;
        string HSN = ((TextBox)row.FindControl("txthsn")).Text;
        string Quantity = ((TextBox)row.FindControl("txtQuantity")).Text;
        string Unit = ((TextBox)row.FindControl("txtUnit")).Text;
        string Rate = ((TextBox)row.FindControl("txtRate")).Text;
        string Total = ((TextBox)row.FindControl("txtTotal")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGST")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGST")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGST")).Text;
        string Discount = ((TextBox)row.FindControl("txtDiscount")).Text;
        string AllTotal = ((TextBox)row.FindControl("txtAlltotal")).Text;
        DataTable Dt = ViewState["PurchaseOrderProduct"] as DataTable;
        Dt.Rows[row.RowIndex]["Productname"] = Product;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Quantity"] = Quantity;
        Dt.Rows[row.RowIndex]["Units"] = Unit;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Total"] = Total;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["Discountpercentage"] = Discount;
        Dt.Rows[row.RowIndex]["Alltotal"] = AllTotal;
        Dt.AcceptChanges();
        ViewState["PurchaseOrderProduct"] = Dt;
        dgvMachineDetails.EditIndex = -1;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["PurchaseOrderProduct"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["PurchaseOrderProduct"] = dt;
        dgvMachineDetails.DataSource = (DataTable)ViewState["PurchaseOrderProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Purchase Order Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    //CONVRT NUMBERS TO WORD START

    public static string ConvertNumbertoWords(string numbers)
    {
        Boolean paisaconversion = false;
        var pointindex = numbers.ToString().IndexOf(".");
        var paisaamt = 0;
        if (pointindex > 0)
            paisaamt = Convert.ToInt32(numbers.ToString().Substring(pointindex + 1, 2));

        int number = Convert.ToInt32(numbers);

        if (number == 0) return "Zero";
        if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
        int[] num = new int[4];
        int first = 0;
        int u, h, t;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (number < 0)
        {
            sb.Append("Minus ");
            number = -number;
        }
        string[] words0 = { "", "One ", "Two ", "Three ", "Four ", "Five ", "Six ", "Seven ", "Eight ", "Nine " };
        string[] words1 = { "Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ", "Fifteen ", "Sixteen ", "Seventeen ", "Eighteen ", "Nineteen " };
        string[] words2 = { "Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ", "Seventy ", "Eighty ", "Ninety " };
        string[] words3 = { "Thousand ", "Lakh ", "Crore " };
        num[0] = number % 1000; // units
        num[1] = number / 1000;
        num[2] = number / 100000;
        num[1] = num[1] - 100 * num[2]; // thousands
        num[3] = number / 10000000; // crores
        num[2] = num[2] - 100 * num[3]; // lakhs
        for (int i = 3; i > 0; i--)
        {
            if (num[i] != 0)
            {
                first = i;
                break;
            }
        }
        for (int i = first; i >= 0; i--)
        {
            if (num[i] == 0) continue;
            u = num[i] % 10; // ones
            t = num[i] / 10;
            h = num[i] / 100; // hundreds
            t = t - 10 * h; // tens
            if (h > 0) sb.Append(words0[h] + "Hundred ");
            if (u > 0 || t > 0)
            {
                if (h > 0 || i == 0) sb.Append("and ");
                if (t == 0)
                    sb.Append(words0[u]);
                else if (t == 1)
                    sb.Append(words1[u]);
                else
                    sb.Append(words2[t - 2] + words0[u]);
            }
            if (i != 0) sb.Append(words3[i - 1]);
        }

        if (paisaamt == 0 && paisaconversion == false)
        {
            sb.Append("Rupees ");
        }
        else if (paisaamt > 0)
        {
            var paisatext = ConvertNumbertoWords(Convert.ToString(paisaamt));
            sb.AppendFormat("Rupees {0} paise", paisatext);
        }
        return sb.ToString().TrimEnd();
    }
    private static String ones(String Number)
    {
        int _Number = Convert.ToInt32(Number);
        String name = "";
        switch (_Number)
        {

            case 1:
                name = "One";
                break;
            case 2:
                name = "Two";
                break;
            case 3:
                name = "Three";
                break;
            case 4:
                name = "Four";
                break;
            case 5:
                name = "Five";
                break;
            case 6:
                name = "Six";
                break;
            case 7:
                name = "Seven";
                break;
            case 8:
                name = "Eight";
                break;
            case 9:
                name = "Nine";
                break;
        }
        return name;
    }
    private static String tens(String Number)
    {
        int _Number = Convert.ToInt32(Number);
        String name = null;
        switch (_Number)
        {
            case 10:
                name = "Ten";
                break;
            case 11:
                name = "Eleven";
                break;
            case 12:
                name = "Twelve";
                break;
            case 13:
                name = "Thirteen";
                break;
            case 14:
                name = "Fourteen";
                break;
            case 15:
                name = "Fifteen";
                break;
            case 16:
                name = "Sixteen";
                break;
            case 17:
                name = "Seventeen";
                break;
            case 18:
                name = "Eighteen";
                break;
            case 19:
                name = "Nineteen";
                break;
            case 20:
                name = "Twenty";
                break;
            case 30:
                name = "Thirty";
                break;
            case 40:
                name = "Fourty";
                break;
            case 50:
                name = "Fifty";
                break;
            case 60:
                name = "Sixty";
                break;
            case 70:
                name = "Seventy";
                break;
            case 80:
                name = "Eighty";
                break;
            case 90:
                name = "Ninety";
                break;
            default:
                if (_Number > 0)
                {
                    name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                }
                break;
        }
        return name;
    }
    private static String ConvertWholeNumber(String Number)
    {
        string word = "";
        try
        {
            bool beginsZero = false;//tests for 0XX  
            bool isDone = false;//test if already translated  
            double dblAmt = (Convert.ToDouble(Number));
            //if ((dblAmt > 0) && number.StartsWith("0"))  
            if (dblAmt > 0)
            {//test for zero or digit zero in a nuemric  
                beginsZero = Number.StartsWith("0");

                int numDigits = Number.Length;
                int pos = 0;//store digit grouping  
                String place = "";//digit grouping name:hundres,thousand,etc...  
                switch (numDigits)
                {
                    case 1://ones' range  

                        word = ones(Number);
                        isDone = true;
                        break;
                    case 2://tens' range  
                        word = tens(Number);
                        isDone = true;
                        break;
                    case 3://hundreds' range  
                        pos = (numDigits % 3) + 1;
                        place = " Hundred ";
                        break;
                    case 4://thousands' range  
                    case 5:
                    case 6:
                        pos = (numDigits % 4) + 1;
                        place = " Thousand ";
                        break;
                    case 7://millions' range  
                    case 8:
                        pos = (numDigits % 6) + 1;
                        place = " Lac ";
                        break;
                    case 9:
                        pos = (numDigits % 8) + 1;
                        place = " Million ";
                        break;
                    case 10://Billions's range  
                    case 11:
                    case 12:

                        pos = (numDigits % 10) + 1;
                        place = " Billion ";
                        break;
                    //add extra case options for anything above Billion...  
                    default:
                        isDone = true;
                        break;
                }
                if (!isDone)
                {//if transalation is not done, continue...(Recursion comes in now!!)  
                    if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                    {
                        try
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                        }
                        catch { }
                    }
                    else
                    {
                        word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                    }

                    //check for trailing zeros  
                    //if (beginsZero) word = " and " + word.Trim();  
                }
                //ignore digit grouping names  
                if (word.Trim().Equals(place.Trim())) word = "";
            }
        }
        catch { }
        return word.Trim();
    }
    private static String ConvertToWords(String numb)
    {
        String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
        String endStr = "Only";
        try
        {
            int decimalPlace = numb.IndexOf(".");
            if (decimalPlace > 0)
            {
                wholeNo = numb.Substring(0, decimalPlace);
                points = numb.Substring(decimalPlace + 1);
                if (Convert.ToInt32(points) > 0)
                {
                    andStr = "and";// just to separate whole numbers from points/cents  
                    endStr = "Paisa " + endStr;//Cents  
                    pointStr = ConvertDecimals(points);
                }
            }
            val = String.Format("{0} {1}{2} {3}", ConvertNumbertoWords(wholeNo).Trim(), andStr, pointStr, endStr);
        }
        catch { }
        return val;
    }
    private static String ConvertDecimals(String number)
    {
        String cd = "", digit = "", engOne = "";
        for (int i = 0; i < number.Length; i++)
        {
            digit = number[i].ToString();
            if (digit.Equals("0"))
            {
                engOne = "Zero";
            }
            else
            {
                engOne = ones(digit);
            }
            cd += " " + engOne;
        }
        return cd;
    }

    //CONVRT NUMBERS TO WORD START END


    protected void lnkBtmNew_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryMaster.aspx");
    }



    protected void txtQuantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    public void Calculations(GridViewRow row)
    {
        TextBox Rate = (TextBox)row.FindControl("txtrate");
        TextBox Qty = (TextBox)row.FindControl("txtquantity");
        TextBox Total = (TextBox)row.FindControl("txttotal");
        TextBox CGSTPer = (TextBox)row.FindControl("txtCGSTPer");
        TextBox SGSTPer = (TextBox)row.FindControl("txtSGSTPer");
        TextBox IGSTPer = (TextBox)row.FindControl("txtIGSTPer");
        TextBox txtCGSTamt = (TextBox)row.FindControl("txtCGST");
        TextBox txtSGSTamt = (TextBox)row.FindControl("txtSGST");
        TextBox txtIGSTamt = (TextBox)row.FindControl("txtIGST");
        TextBox Disc_Per = (TextBox)row.FindControl("txtdiscount");
        TextBox txtDiscountAmount = (TextBox)row.FindControl("txtDiscountAmount");

        TextBox GrossTotal = (TextBox)row.FindControl("txtgrandtotal");
        TextBox txtAlltotal = (TextBox)row.FindControl("txtAlltotal");
        int value;
        if (int.TryParse(Qty.Text, out value))
        {
            // Check if the value is a multiple of 25
            if (value % 25 == 0)
            {
                var total = Convert.ToDecimal(Rate.Text) * Convert.ToDecimal(Qty.Text);
                Total.Text = string.Format("{0:0.00}", total);

                decimal tax_amt;
                decimal cgst_amt;
                decimal sgst_amt;
                decimal igst_amt;

                if (string.IsNullOrEmpty(CGSTPer.Text))
                {
                    cgst_amt = 0;
                }
                else
                {
                    cgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(CGSTPer.Text) / 100;
                }
                txtCGSTamt.Text = string.Format("{0:0.00}", cgst_amt);

                if (string.IsNullOrEmpty(SGSTPer.Text))
                {
                    sgst_amt = 0;
                }
                else
                {
                    sgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(SGSTPer.Text) / 100;
                }
                txtSGSTamt.Text = string.Format("{0:0.00}", sgst_amt);

                if (string.IsNullOrEmpty(IGSTPer.Text))
                {
                    igst_amt = 0;
                }
                else
                {
                    igst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(IGSTPer.Text) / 100;
                }
                txtIGSTamt.Text = string.Format("{0:0.00}", igst_amt);

                tax_amt = cgst_amt + sgst_amt + igst_amt;

                var totalWithTax = Convert.ToDecimal(total.ToString()) + Convert.ToDecimal(tax_amt.ToString());
                decimal disc_amt;
                if (string.IsNullOrEmpty(Disc_Per.Text))
                {
                    disc_amt = 0;
                }
                else
                {
                    disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
                    //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
                }

                var Grossamt = Convert.ToDecimal(total.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
                txtAlltotal.Text = string.Format("{0:0.00}", Grossamt);
                txtDiscountAmount.Text = string.Format("{0:0}", disc_amt);
            }
            else
            {
                txtAlltotal.Text = "";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please enter a Quantity that is a multiple of 25...!!')", true);

            }
        }



        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }


    protected void txtrate_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());
        txttotal.Text = Convert.ToString(TotalAmt);
        decimal total;
        decimal Percentage;
        if (txtIGST.Text == null || txtIGST.Text == "" || txtIGST.Text == "0")
        {
            Percentage = Convert.ToDecimal(txtCGST.Text);
            total = (TotalAmt * Percentage / 100);

            txtCGSTamt.Text = total.ToString();

            txtSGSTamt.Text = txtCGSTamt.Text;

            txtSGST.Text = txtCGST.Text;
            var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtCGSTamt.Text.Trim()) + Convert.ToDecimal(txtSGSTamt.Text.Trim());
            txtgrandtotal.Text = GrandTotal.ToString();
        }
        else
        {
            Percentage = Convert.ToDecimal(txtIGST.Text);
            total = (TotalAmt * Percentage / 100);

            txtIGSTamt.Text = total.ToString();
            var GrandTotal = Convert.ToDecimal(txttotal.Text.Trim()) + Convert.ToDecimal(txtIGSTamt.Text.Trim());
            txtgrandtotal.Text = GrandTotal.ToString();
        }
    }
}