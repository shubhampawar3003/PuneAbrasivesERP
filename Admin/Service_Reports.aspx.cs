

using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_Service_Reports : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable Dt_Product = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {


            if (Session["UserCode"] == null)
            {
                Response.Redirect("../Login.aspx");
            }
            else
            {
                if (Request.QueryString["ID"] != null)
                {
                    FillddlProduct();
                    string Id = Decrypt(Request.QueryString["ID"].ToString());
                    Label3.Text = Session["Username"].ToString();
                    txtticket.Text = Id;
                    Load_Record(Id);

                }
            }



            ViewState["RowNo"] = 0;

            Dt_Product.Columns.AddRange(new DataColumn[17] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
            ViewState["QuotationProduct"] = Dt_Product;
            FillDropDownList3();


            if (txtinstallation.Text == "Installation")
            {
                installation.Visible = true;
            }
            else if (txtinstallation.Text != "Installation")
            {
                installation.Visible = false;
            }
        }
    }
    private void FillDropDownList3()
    {
        SqlDataAdapter ad = new SqlDataAdapter("select * from tbl_UserMaster where Designation='Service Engineer'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            DropDownList3.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            DropDownList3.DataTextField = "Username";
            DropDownList3.DataBind();
            DropDownList3.Items.Insert(0, "-- Select Service Engineer --");
        }
    }
    //Data Fetch
    private void Load_Record(string ID)
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        String query = "select TicketNo, CompanyName, Address, OwnerName, MobileNo, EmailID, call_category, ProductName, convert(varchar, CreatedOn, 120) as callregister_datetime from tbl_ComplaintRegister where TicketNo='" + txtticket.Text + "'";
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = query;
        cmd.Connection = con;
        try
        {
            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {

                TextBox1.Text = sdr["CompanyName"].ToString();
                TextBox3.Text = sdr["Address"].ToString();
                TextBox5.Text = sdr["OwnerName"].ToString();
                TextBox2.Text = sdr["MobileNo"].ToString();
                TextBox4.Text = sdr["EmailID"].ToString();
                txtinstallation.Text = sdr["call_category"].ToString();
                TextBox11.Text = sdr["callregister_datetime"].ToString();
                txtmachinemodel.Text = sdr["ProductName"].ToString();
                ddlProduct.SelectedItem.Text = sdr["ProductName"].ToString();
                ShowDtlEdit();
            }
        }
        catch (Exception ex)
        {
            Response.Redirect("~/Error.aspx");
        }
        finally
        {
            con.Close();
            con.Dispose();
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

    protected void ShowDtlEdit()
    {
        divTotalPart.Visible = true;
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ProductMaster] WHERE ProductName='" + ddlProduct.SelectedItem.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            txtdescription.Text = Dt.Rows[0]["Description"].ToString();
            txthsnsac.Text = Dt.Rows[0]["HSN"].ToString();
            txtrate.Text = Dt.Rows[0]["ServiceCharges"].ToString();
            txtunit.Text = Dt.Rows[0]["Unit"].ToString();
            txtCGSTamt.Text = "0.00";
            txtSGSTamt.Text = "0.00";
            txtIGSTamt.Text = "0.00";
            txtdiscount.Text = "0.00";
            txtdiscountamt.Text = "0.00";
            txtgrandtotal.Focus();
        }
        //SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_QuotationDtls] WHERE Quotation_no='" + txtquotationno.Text + "'", Cls_Main.Conn);
        //DataTable DTCOMP = new DataTable();
        //Da.Fill(DTCOMP);

        //int count = 0;

        //if (DTCOMP.Rows.Count > 0)
        //{
        //    if (Dt_Product.Columns.Count < 0)
        //    {
        //        Show_Grid();
        //    }

        //    for (int i = 0; i < DTCOMP.Rows.Count; i++)
        //    {
        //        Dt_Product.Rows.Add(count, DTCOMP.Rows[i]["Productname"].ToString(),  DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
        //        count = count + 1;
        //    }
        //}

        dgvMachineDetails.EmptyDataText = "No Data Found";
        dgvMachineDetails.DataSource = Dt_Product;
        dgvMachineDetails.DataBind();

    }


    private void Show_Grid()
    {
        divTotalPart.Visible = true;
        DataTable Dt = (DataTable)ViewState["QuotationProduct"];
        Dt.Rows.Add(ViewState["RowNo"], ddlProduct.SelectedItem.Text, txtdescription.Text.Trim(), txthsnsac.Text.Trim(), txtquantity.Text, txtunit.Text, txtrate.Text, txttotal.Text, txtCGST.Text, txtCGSTamt.Text, txtSGST.Text, txtSGSTamt.Text, txtIGST.Text, txtIGSTamt.Text, txtdiscount.Text, txtdiscountamt.Text, txtgrandtotal.Text);
        ViewState["QuotationProduct"] = Dt;
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
        dgvMachineDetails.DataSource = (DataTable)ViewState["QuotationProduct"];
        dgvMachineDetails.DataBind();
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

    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (Session["UserCode"] != null)
            {
                if (TextBox12.Text == string.Empty || TextBox13.Text == string.Empty)
                {
                    lblmsg.Visible = true;
                    lblmsg1.Visible = true;
                }
                else
                {

                    string query = "select *from dbo.tbl_ServiceReport where ticketno='" + txtticket.Text + "'";
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                    if (dt.Rows.Count < 1)
                    {
                        using (SqlCommand cmm = new SqlCommand())
                        {
                            con.Open();
                            string quiry = "BEGIN TRY " +
                                             "BEGIN TRANSACTION " +
                         "INSERT INTO [tbl_ServiceReport]([SessionName],[ticketno],[customer_name],[customer_address],[contact_person],[mobile],[email],[call_category],[ProductModel],[SerialNumber],[ProductStatus],[Installation_date],[Warranty_Duration],[Warrantyperiod_from],[Warrantyperiod_to] ,[PreventiveSchedule1],[PreventiveSchedule2],[PreventiveSchedule3],[PreventiveSchedule4] ,[csm_model],[csm_head],[csm_size] ,[NatureOfComplaint] ,[Callregistered_datetime],[CallAttended_datetime],[CallCompleted_datetime],[complainDescription],[sparetype],[advice] ,[service_attendwith],[location])" +
                            " VALUES(@SessionName,@ticketno,@customer_name,@customer_address,@contact_person,@mobile,@email,@call_category,@ProductModel,@SerialNumber,@ProductStatus,@Installation_date,@Warranty_Duration,@Warrantyperiod_from,@Warrantyperiod_to,@PreventiveSchedule1,@PreventiveSchedule2,@PreventiveSchedule3,@PreventiveSchedule4,@csm_model,@csm_head,@csm_size ,@NatureOfComplaint ,@Callregistered_datetime,@CallAttended_datetime,@CallCompleted_datetime,@complainDescription,@sparetype,@advice ,@service_attendwith,@location)";

                            quiry += "update tbl_ComplaintRegister set Status ='" + 1 + "' where ticketno='" + txtticket.Text + "'";

                            quiry += " COMMIT TRANSACTION " +
                                                             "END TRY " +
                                                             "BEGIN CATCH " +
                                                             "ROLLBACK TRANSACTION " +
                                                             " END CATCH";

                            cmm.Parameters.AddWithValue("@SessionName", Session["UserCode"].ToString());
                            cmm.Parameters.AddWithValue("@ticketno", txtticket.Text);
                            cmm.Parameters.AddWithValue("@customer_name", TextBox1.Text);
                            cmm.Parameters.AddWithValue("@customer_address", TextBox3.Text);
                            cmm.Parameters.AddWithValue("@contact_person", TextBox5.Text);
                            cmm.Parameters.AddWithValue("@mobile", TextBox2.Text);
                            cmm.Parameters.AddWithValue("@email", TextBox4.Text);
                            cmm.Parameters.AddWithValue("@call_category", txtinstallation.Text);
                            cmm.Parameters.AddWithValue("@ProductModel", txtmachinemodel.Text);
                            cmm.Parameters.AddWithValue("@SerialNumber  ", TextBox8.Text);
                            cmm.Parameters.AddWithValue("@ProductStatus", DropDownList2.Text);
                            cmm.Parameters.AddWithValue("@sparetype", "");


                            if (txtinstallation.Text == "Installation")
                            {
                                var Installation_date = Convert.ToDateTime(TextBox6.Text);
                                cmm.Parameters.AddWithValue("@Installation_date", Installation_date);

                                cmm.Parameters.AddWithValue("@Warranty_Duration", DropDownList7.Text);
                                var Warrantyperiod_from = Convert.ToDateTime(TextBox9.Text);
                                cmm.Parameters.AddWithValue("@Warrantyperiod_from", Warrantyperiod_from);
                                var Warrantyperiod_to = Convert.ToDateTime(TextBox7.Text);
                                cmm.Parameters.AddWithValue("@Warrantyperiod_to", Warrantyperiod_to);

                                //List<string> selectedItems = new List<string>();

                                //foreach (System.Web.UI.WebControls.ListItem item in ListBox1.Items)
                                //{
                                //    if (item.Selected == true)
                                //    {
                                //        selectedItems.Add(item.Value);
                                //    }
                                //}
                                //Label1.Text = string.Join(",", selectedItems.ToArray());


                                cmm.Parameters.AddWithValue("@PreventiveSchedule1", ddlmonth1.Text + " " + ddldate1.Text);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule2", ddlmonth2.Text + " " + ddldate2.Text);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule3", ddlmonth3.Text + " " + ddldate3.Text);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule4", ddlmonth4.Text + " " + ddldate4.Text);

                            }

                            else
                            {
                                cmm.Parameters.AddWithValue("@Installation_date", DBNull.Value);
                                cmm.Parameters.AddWithValue("@Warranty_Duration", DBNull.Value);
                                cmm.Parameters.AddWithValue("@Warrantyperiod_from", DBNull.Value);
                                cmm.Parameters.AddWithValue("@Warrantyperiod_to", DBNull.Value);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule1", DBNull.Value);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule2", DBNull.Value);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule3", DBNull.Value);
                                cmm.Parameters.AddWithValue("@PreventiveSchedule4", DBNull.Value);
                            }
                            cmm.Parameters.AddWithValue("@csm_model", "");
                            cmm.Parameters.AddWithValue("@csm_head", "");
                            cmm.Parameters.AddWithValue("@csm_size", "");
                            cmm.Parameters.AddWithValue("@NatureOfComplaint", TextBox10.Text);

                            cmm.Parameters.AddWithValue("@Callregistered_datetime", DateTime.Parse(TextBox11.Text));

                            var CallAttended_datetime = Convert.ToDateTime(TextBox12.Text);
                            cmm.Parameters.AddWithValue("@CallAttended_datetime", CallAttended_datetime);

                            var CallCompleted_datetime = Convert.ToDateTime(TextBox13.Text);
                            cmm.Parameters.AddWithValue("@CallCompleted_datetime", CallCompleted_datetime);


                            cmm.Parameters.AddWithValue("@complainDescription", TextBox17.Text);
                            cmm.Parameters.AddWithValue("@advice", TextBox61.Text);
                            cmm.Parameters.AddWithValue("@service_attendwith", DropDownList3.Text);
                            cmm.Parameters.AddWithValue("@location", ddllocation.Text);


                            cmm.Connection = con;
                            cmm.CommandType = CommandType.Text;
                            cmm.CommandText = quiry;
                            cmm.ExecuteNonQuery();
                            //table data inseart
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
                                SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_ServiceReportDtls (Tiket_no,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn) VALUES(@Tiket_no,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn)", Cls_Main.Conn);
                                cmdd.Parameters.AddWithValue("@Tiket_no", txtticket.Text);
                                cmdd.Parameters.AddWithValue("@Productname", lblproduct);
                                cmdd.Parameters.AddWithValue("@Description", lblDescription);
                                cmdd.Parameters.AddWithValue("@HSN", lblhsn);
                                cmdd.Parameters.AddWithValue("@Quantity", lblQuantity);
                                cmdd.Parameters.AddWithValue("@Units", lblUnit);
                                cmdd.Parameters.AddWithValue("@Rate", lblRate);
                                cmdd.Parameters.AddWithValue("@CGSTPer", lblCGSTPer);
                                cmdd.Parameters.AddWithValue("@CGSTAmt", lblCGST);
                                cmdd.Parameters.AddWithValue("@SGSTPer", lblSGSTPer);
                                cmdd.Parameters.AddWithValue("@SGSTAmt", lblSGST);
                                cmdd.Parameters.AddWithValue("@IGSTPer", lblIGSTPer);
                                cmdd.Parameters.AddWithValue("@IGSTAmt", lblIGST);
                                cmdd.Parameters.AddWithValue("@Total", lblTotal);
                                cmdd.Parameters.AddWithValue("@Discountpercentage", lblDiscount);
                                cmdd.Parameters.AddWithValue("@DiscountAmount", lblDiscountAmount);

                                cmdd.Parameters.AddWithValue("@Alltotal", lblAlltotal);
                                cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                                cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                                cmdd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();
                            }

                            string query1 = "select Status from tbl_ComplaintRegister where TicketNo='" + txtticket.Text + "'";
                            SqlCommand cmd1 = new SqlCommand(query1, con);
                            DataTable dt1 = new DataTable();
                            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                            da1.Fill(dt1);
                            if (dt1.Rows.Count == 1)
                            {
                                string active = dt1.Rows[0]["Status"].ToString();
                                if (active == "1")
                                {
                                    MSG();
                                    SendPDFEmail();
                                    pdf();
                                    Cls_Main.Conn_Open();
                                    SqlCommand cmdUpdate = new SqlCommand("update tbl_ComplaintRegister set Status=2 where TicketNo=@Ticketno", Cls_Main.Conn);
                                    cmdUpdate.Parameters.AddWithValue("@Ticketno", txtticket.Text);
                                    cmdUpdate.ExecuteNonQuery();
                                    Cls_Main.Conn_Close();
                                    //btninvoice.Visible = true;
                                    ScriptManager.RegisterStartupScript(this, GetType(), "Failed", "alert('Call Closed Succesfully!!.');window.location='Dashboard.aspx'; ", true);
                                }
                                else
                                {
                                    ScriptManager.RegisterStartupScript(this, GetType(), "Failed", "alert('Call Not Closed!!.');", true);
                                }

                            }




                        }
                    }

                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record Already Exists!!');", true);
                    }

                }
            }
            else
            {
                Response.Redirect("../Login.aspx");
            }
        }
        catch
        {

        }
    }

    protected void txtCGST_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
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

    protected void txtquantity_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());
        txttotal.Text = TotalAmt.ToString();
    }

    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT[ProductName] FROM[tbl_ProductMaster]", Cls_Main.Conn);
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
            txtrate.Text = Dt.Rows[0]["ServiceCharges"].ToString();
            txtunit.Text = Dt.Rows[0]["Unit"].ToString();
            txtCGSTamt.Text = "0.00";
            txtSGSTamt.Text = "0.00";
            txtIGSTamt.Text = "0.00";
            txtdiscount.Text = "0.00";
            txtdiscountamt.Text = "0.00";
            txtgrandtotal.Focus();
        }

    }

    protected void check_addresss_CheckedChanged(object sender, EventArgs e)
    {
        //if (check_addresss.Checked == true)
        //{
        //    txtshippingaddress.Text = txtaddress.Text;
        //}
    }

    protected void lnkbtnDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["QuotationProduct"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["QuotationProduct"] = dt;
        dgvMachineDetails.DataSource = (DataTable)ViewState["QuotationProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Product Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
        DataTable Dt = ViewState["QuotationProduct"] as DataTable;
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
        ViewState["QuotationProduct"] = Dt;
        dgvMachineDetails.EditIndex = -1;
        dgvMachineDetails.DataSource = (DataTable)ViewState["QuotationProduct"];
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
        DataTable Dt = ViewState["QuotationProduct"] as DataTable;
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
        ViewState["QuotationProduct"] = Dt;
        dgvMachineDetails.EditIndex = -1;
        dgvMachineDetails.DataSource = (DataTable)ViewState["QuotationProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void dgvMachineDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvMachineDetails.EditIndex = e.NewEditIndex;
        dgvMachineDetails.DataSource = (DataTable)ViewState["QuotationProduct"];
        dgvMachineDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        decimal DiscountAmt;
        decimal val1 = Convert.ToDecimal(txtgrandtotal.Text);
        decimal val2 = Convert.ToDecimal(txtdiscount.Text);
        DiscountAmt = (val1 * val2 / 100);
        txtgrandtotal.Text = (val1 - DiscountAmt).ToString();

        txtdiscountamt.Text = DiscountAmt.ToString();
    }

    protected void txtIGST_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
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

    protected void txtSGST_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
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



    //CONVRT NUMBERS TO WORD START

    public string ConvertNumbertoWords(int number)
    {
        if (number == 0)
            return "ZERO";
        if (number < 0)
            return "minus " + ConvertNumbertoWords(Math.Abs(number));
        string words = "";
        if ((number / 1000000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000000) + " Million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000) + " Thousand ";
            number %= 1000;
        }
        if ((number / 100) > 0)
        {
            words += ConvertNumbertoWords(number / 100) + " Hundred ";
            number %= 100;
        }
        if (number > 0)
        {
            if (words != "")
                words += "And ";
            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
        }
        return words;
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
            val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
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
        Response.Redirect("CompanyMaster.aspx");
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Dashboard.aspx");
    }

    protected void txtQuantity_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        // var TotalAmt = Convert.ToDecimal(txtquantity.Text.Trim()) * Convert.ToDecimal(txtrate.Text.Trim());
        // txttotal.Text = TotalAmt.ToString();

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

        txtgrandtotal.Focus();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void txtCGSTPer_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtSGSTPer_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtIGSTPer_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        txtgrandtotal.Focus();
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }


    protected void DropDownList7_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DropDownList7.Text == "6 months")
        {
            pshead1.Visible = true;
            pshead2.Visible = true;
            ps1.Visible = true;
            ps2.Visible = true;

        }
        else if (DropDownList7.Text == "12 months")
        {
            pshead1.Visible = true;
            pshead2.Visible = true;
            pshead3.Visible = true;
            pshead4.Visible = true;
            ps1.Visible = true;
            ps2.Visible = true;
            ps3.Visible = true;
            ps4.Visible = true;

        }
        else if (DropDownList7.Text == " None ")
        {
            pshead1.Visible = false;
            pshead2.Visible = false;
            pshead3.Visible = false;
            pshead4.Visible = false;
            ps1.Visible = false;
            ps2.Visible = false;
            ps3.Visible = false;
            ps4.Visible = false;

        }
    }

    byte[] result;
    private void SendPDFEmail()
    {
        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                //border = '0.5'
                StringBuilder sb = new StringBuilder();

                sb.Append("<table  cellspacing='0'  border='0.5' cellpadding='1' width='100%'>");
                sb.Append("<tr>");
                sb.Append("<td  colspan='8'  align='center'>SERVICE CALL REPORT </td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='8'  align='center'><h3  style='font-size:13px; font-weight:900; color:#317abe;'>WEB LINK SERVICES  PVT LTD.</h3></td>");
                sb.Append("</tr>");


                if (ddllocation.Text == "Kolkata")
                {
                    sb.Append("<tr><td colspan='8' align='center' style='color:#000; font-size:9px;'>216/3A, A. J. C. BOSE ROAD, Geetanjali Bulding, 1ST Floor, Kolkata, West Bengal, INDIA – 700017 <br />Email : service@sripl.com     &nbsp; &nbsp; &nbsp;www.sripl.com   &nbsp;&nbsp; &nbsp;   Mob.- +91 9748200174</td></tr>");
                }
                else if (ddllocation.Text == "Guwahati")
                {
                    sb.Append("<tr><td colspan='8' align='center' style='color:#000; font-size:9px;'>House No. 519, P.O Sawkuchi, Lokhra Road, Guwahati-40, Assam  <br />Email : service@sripl.com     &nbsp; &nbsp; &nbsp;www.sripl.com   &nbsp;&nbsp; &nbsp;   Mob.- +91 9748200174</td></tr>");
                }
                //sb.Append("<tr><td colspan='8'><br /></td></tr>");

                sb.Append("<tr><td colspan='2'  align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Ticket No</td> <td colspan='2' align='center' style='font-size:8px; font-weight:900;'>" + txtticket.Text + "</td> <td colspan='2' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Customer Name</td> <td colspan='2'  align='center' style='font-size:8px; font-weight:900;'>" + TextBox1.Text + "</td></tr>");
                sb.Append("<tr><td colspan='2'  align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Mobile</td> <td colspan='2' align='center' style='font-size:8px; font-weight:900;'>" + TextBox2.Text + "</td> <td colspan='2' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Company Address</td> <td colspan='2'  align='center' style='font-size:8px; font-weight:900;'>" + TextBox3.Text + "</td></tr>");
                sb.Append("<tr><td colspan='2'  align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Email</td> <td colspan='2' align='center' style='font-size:8px; font-weight:900;'>" + TextBox4.Text + "</td> <td colspan='2' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Contact Person</td> <td colspan='2'  align='center' style='font-size:8px; font-weight:900;'>" + TextBox5.Text + "</td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900;  color:#317abe;'>Call Category </td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900;  color:#317abe;'>Machine model</td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Serial Number</td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Machine Status</td>");
                sb.Append("</tr>");


                sb.Append("<tr>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + txtinstallation.Text + " </td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + txtmachinemodel.Text + "</td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + TextBox8.Text + "</td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + DropDownList2.Text + "</td>");
                sb.Append("</tr>");


                if (txtinstallation.Text == "Installation")
                {
                    sb.Append("<tr>");
                    sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Date Of Installation </td>");
                    sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Warranty duration</td>");
                    sb.Append("<td colspan='4' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Warranty Period</td>");

                    sb.Append("</tr>");


                    sb.Append("<tr>");
                    sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + TextBox6.Text + " </td>");
                    sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + DropDownList7.Text + "</td>");
                    sb.Append("<td colspan='4' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + TextBox9.Text + " - " + TextBox7.Text + "</td>");
                    sb.Append("</tr>");

                    //List<string> selectedItems = new List<string>();

                    //foreach (System.Web.UI.WebControls.ListItem item in ListBox1.Items)
                    //{
                    //    if (item.Selected == true)
                    //    {
                    //        selectedItems.Add(item.Value);
                    //    }
                    //}
                    //Label1.Text = string.Join(",", selectedItems.ToArray());
                    // sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + Label1.Text + "</td>");

                    if (DropDownList7.Text == "6 months")
                    {
                        sb.Append("<tr>");
                        sb.Append("<td colspan='4' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Preventive schedule 1 </td>");
                        sb.Append("<td colspan='4' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Preventive schedule 2</td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td colspan='4' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + ddlmonth1.Text + " " + ddldate1.Text + " </td>");
                        sb.Append("<td colspan='4' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + ddlmonth2.Text + " " + ddldate2.Text + "</td>");
                        sb.Append("</tr>");
                    }
                    else if (DropDownList7.Text == "12 months")
                    {
                        sb.Append("<tr>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Preventive schedule 1 </td>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Preventive schedule 2</td>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Preventive schedule 3</td>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Preventive schedule 4</td>");
                        sb.Append("</tr>");

                        sb.Append("<tr>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + ddlmonth1.Text + " " + ddldate1.Text + " </td>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + ddlmonth2.Text + " " + ddldate2.Text + "</td>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + ddlmonth3.Text + " " + ddldate3.Text + "</td>");
                        sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'>" + ddlmonth4.Text + " " + ddldate4.Text + "</td>");
                        sb.Append("</tr>");
                    }
                }


                sb.Append("<tr>");
                sb.Append("<td colspan='2'  align='center'  style='font-size:9px; font-weight:900; color:#317abe;'>Nature of Complaint </td>");
                sb.Append("<td  colspan='2' align='center' style='font-size:9px; font-weight:900;   color:#000;'>" + TextBox10.Text + "</td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'><p> Call registered  </p> <p> Call Attended  </p> <p> Call Completed  </p></td>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#000;'><p>" + TextBox11.Text + "</p> <p>" + TextBox12.Text + " </p> <p> " + TextBox13.Text + " </p> </td>");

                sb.Append("</tr>");

                //sb.Append("<tr  colspan='8'><td><br />  </td></tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='2' align='center' style='font-size:9px; font-weight:900; color:#317abe;'>Complaint Description</td>");
                sb.Append("<td colspan='6' align='center' style='font-size:9px; font-weight:900; color:#000; text-align:center;'>" + TextBox17.Text + "</td>");
                sb.Append("</tr>");


                //sb.Append("<tr  colspan='8'><td><br />  </td></tr>");


                sb.Append(" </table>");
                sb.Append(" </br>");
                sb.Append("<table  border='0.5' cellspacing='0' cellpadding='1' width='100%'>");
                sb.Append("<tr>");
                sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Advice by Web Link Services Pvt. Ltd.</td>");
                sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + TextBox61.Text + "</td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Name </td>");
                sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + Label3.Text + "</td>");
                sb.Append("</tr>");


                sb.Append("<tr>");
                sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Designation </td>");
                sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + Label4.Text + "</td>");
                sb.Append("</tr>");

                sb.Append(" </table>");
                sb.Append(" </br>");
                SqlDataAdapter Daa = new SqlDataAdapter("SELECT distinct(Productname),[ID],[Tiket_no],[Description],[HSN],[Quantity],[Units],[Rate],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],[Discountpercentage],[DiscountAmount],[Alltotal] FROM [tbl_ServiceReportDtls]  WHERE Tiket_no ='" + txtticket.Text + "'  ", con);
                DataTable Dtt = new DataTable();
                Daa.Fill(Dtt);

                decimal DiscPercentage = Convert.ToDecimal(Dtt.Rows[0]["Discountpercentage"]);

                decimal CGSTAmt = Convert.ToDecimal(Dtt.Rows[0]["CGSTAmt"]);
                decimal SGSTAmt = Convert.ToDecimal(Dtt.Rows[0]["SGSTAmt"]);
                decimal IGSTAmt = Convert.ToDecimal(Dtt.Rows[0]["IGSTAmt"]);

                decimal Basic = Convert.ToDecimal(Dtt.Rows[0]["Total"]);
                var Disc = Basic * DiscPercentage / 100;
                double Ttotal_price = 0, CGST_Total = 0, SGST_Total = 0, IGST_Total = 0;

                var Taxable = Basic - Disc;

                if (Dtt.Rows.Count > 0)
                {
                    SqlDataAdapter Daaa = new SqlDataAdapter("SELECT distinct(Productname) FROM [tbl_ServiceReportDtls]  WHERE Tiket_no ='" + txtticket.Text + "'  ", con);
                    DataTable Dttt = new DataTable();
                    Daaa.Fill(Dttt);
                    sb.Append("<table  border='0.5' cellspacing='0' cellpadding='1' width='100%'>");
                    sb.Append("<tr>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'> No.</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Item & Description </td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>HSN </td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Qty </td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Unit</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Rate</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Discount Amount </td>");
                    //sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Taxable</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>CGST</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>SGST</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>IGST</td>");
                    sb.Append("<td colspan='3' align='center' style='font-size:8px; font-weight:900; color:#317abe;'>Amount</td>");

                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    int rowid = 1;
                    foreach (DataRow dr in Dtt.Rows)
                    {
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + rowid.ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["Productname"].ToString() + "-" + dr["Description"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["HSN"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["Quantity"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["Units"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["Rate"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["DiscountAmount"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["CGSTPer"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["SGSTPer"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["IGSTPer"].ToString() + "</td>");
                        sb.Append("<td colspan='5' align='center' style='font-size:8px; font-weight:900; color:#000; text-align:center;'>" + dr["Alltotal"].ToString() + "</td>");
                        rowid++;


                    }
                    sb.Append("</tr>");
                    sb.Append(" </table>");

                }

                StringReader sr = new StringReader(sb.ToString());
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    byte[] bytes = memoryStream.ToArray();
                    result = memoryStream.GetBuffer();
                    memoryStream.Close();
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("erp@weblinkservices.net", "");

                    mail.Subject = "Web Link Services Report";
                    if (TextBox4.Text != string.Empty)
                    {
                        mail.To.Add(TextBox4.Text);
                        MailAddress Bcopy = new MailAddress("erp@weblinkservices.net");
                        mail.CC.Add(Bcopy);
                        MailAddress Bcopy1 = new MailAddress("erp@weblinkservices.net");
                        mail.Bcc.Add(Bcopy1);
                    }
                    else
                    {
                        MailAddress Bcopy = new MailAddress("erp@weblinkservices.net");
                        mail.CC.Add(Bcopy);
                        MailAddress Bcopy1 = new MailAddress("erp@weblinkservices.net");
                        mail.Bcc.Add(Bcopy1);
                    }
                    mail.Body = "Web Link Services pvt Ltd Service Report";
                    mail.Attachments.Add(new Attachment(new MemoryStream(bytes), "Service Report.pdf"));


                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["Host"];

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


                    smtp.Send(mail);
                    ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('Sent mail Successfully!!.');", true);
                }
            }
        }
    }

    void pdf()
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        SqlCommand cmd1 = new SqlCommand();
        cnn.Open();
        cmd1.CommandText = "insert into dbo.tbl_ServiceReportPdf(SessionName,ticketno,company_name,customer_name,email,pdfname,pdftype,pdf) " + "values(@SessionName,@ticketno,@company_name,@customer_name,@email,@pdfname,@pdftype,@pdf)";
        cmd1.Parameters.AddWithValue("@SessionName", Session["UserCode"].ToString());
        cmd1.Parameters.AddWithValue("@ticketno", txtticket.Text);
        cmd1.Parameters.AddWithValue("@company_name", TextBox1.Text);
        cmd1.Parameters.AddWithValue("@customer_name", TextBox5.Text);
        cmd1.Parameters.AddWithValue("@email", TextBox4.Text);
        cmd1.Parameters.AddWithValue("@pdfname", "Service Report");
        cmd1.Parameters.AddWithValue("@pdftype", "application/pdf");
        cmd1.Parameters.AddWithValue("@pdf", result);
        cmd1.Connection = cnn;
        cmd1.ExecuteNonQuery();
    }
    string name1 = string.Empty; string empMob = string.Empty;
    void MSG()
    {
        if (TextBox2.Text != null && TextBox2.Text != string.Empty)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                try
                {
                    string no = TextBox2.Text;
                    string Ticket_No = txtticket.Text;
                    name1 = Session["UserCode"].ToString();
                    Getemp_mobile();

                    //// var date1 = DateTime.Parse(TextBox7.Text).ToString("dd/MM/yyyy");
                    //// var date2 = DateTime.Parse(date1).ToString("MMM dd yyyy");

                    String message = HttpUtility.UrlEncode("Your service request no. " + Ticket_No + " has solved by Mr. " + name1 + " - " + empMob + " . If any query pls contact Web Link Services Pvt. Ltd.");
                    //String message = HttpUtility.UrlEncode("Dear Customer, Your service request no. " + Ticket_No + " has been assigned to S.E. Mr. " + name1 + " - " + empMob + " . His schedule visit is on " + date2 + " between 10:30 AM - 12:30 PM. Shree Raj Int. Pvt. Ltd. For any other assistance please contact Mrs. Samapti Chatterjee 03322815546");
                    //string url = "http://sms.weblinkservices.net/api/mt/SendSMS?APIKey=F5tV4Jgm2EmHVivsO3noFQ&senderid=SRIRAJ&channel=Trans&DCS=0&flashsms=0&number=" + no + "&text=" + message + "&route=06";
                    //string result = client.DownloadString(url);
                    //ScriptManager.RegisterStartupScript(this, GetType(), "Success", "alert('send Successfully.');", true);

                }
                catch (Exception ex)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
                }
            }
        }

    }
    void Getemp_mobile()
    {
        SqlConnection con1 = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        DataTable dt = new DataTable();
        con1.Open();
        SqlDataReader myReader = null;
        SqlCommand myCommand = new SqlCommand("select Mobileno from tbl_UserMaster where EmailID='" + name1 + "'", con1);

        myReader = myCommand.ExecuteReader();

        while (myReader.Read())
        {
            empMob = (myReader["Mobileno"].ToString());

        }

        con1.Close();
    }

    //protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if(DropDownList2.SelectedItem.Text== "Warranty")
    //    {
    //        DropDownList7.Visible = true;
    //        TextBox9.Visible = true;
    //        TextBox7.Visible = true;
    //        lblwarrantyduration.Visible = true;
    //        lblwarrantyperiod.Visible = true;
    //    }
    //    else if(DropDownList2.SelectedItem.Text== "None")
    //    {
    //        lblwarrantyduration.Visible = false;
    //        lblwarrantyperiod.Visible = false;
    //        DropDownList7.Visible = false;
    //        TextBox9.Visible = false;
    //        TextBox7.Visible = false;
    //    }
    //}
}