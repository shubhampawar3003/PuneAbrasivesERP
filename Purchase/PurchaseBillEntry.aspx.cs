
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_PurchaseBillEntry : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
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

                // Create a DataTable with the required columns
                DataTable dt = new DataTable();
                dt.Columns.Add("IsSelected");
                dt.Columns.Add("id");
                dt.Columns.Add("Particulars");
                dt.Columns.Add("Description");
                dt.Columns.Add("HSN");
                dt.Columns.Add("Qty");
                dt.Columns.Add("Units");
                dt.Columns.Add("Rate");
                dt.Columns.Add("CGSTPer");
                dt.Columns.Add("SGSTPer");
                dt.Columns.Add("IGSTPer");
                dt.Columns.Add("Discount");
                dt.Columns.Add("Batchno");

                //Added Empty Row
                dt.Rows.Add(dt.NewRow());


                // Bind to GridView
                dgvTaxinvoiceDetails.DataSource = dt;
                dgvTaxinvoiceDetails.DataBind();

                txtBilldate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtDOR.Text = DateTime.Now.ToString("yyyy-MM-dd");
                if (Request.QueryString["Id"] != null)
                {
                    ID = objcls.Decrypt(Request.QueryString["Id"].ToString());
                    hhd.Value = ID;
                    Load_Record(ID);
                    btnsave.Text = "Update";
                }
                if (Request.QueryString["OrderNo"] != null)
                {
                    ID = objcls.Decrypt(Request.QueryString["OrderNo"].ToString());
                    hhd.Value = ID;
                    Load_OrderNORecord(ID);
                }


                enable();

            }
        }
    }

    public void enable()
    {

        txtCost.Enabled = false;
        txtTCGSTamt.Enabled = false;
        txtTSGSTamt.Enabled = false;
        txtTIGSTamt.Enabled = false;
        txtTCost.Enabled = false;
        txtTCSAmt.Enabled = false;
        txtGrandTot.Enabled = false;

    }
    //Data Fetch
    private void Load_Record(string ID)
    {
        try
        {
            DataTable dt = Cls_Main.Read_Table("SELECT * FROM [tblPurchaseBillHdr] WHERE ID='" + ID + "' ");
            if (dt.Rows.Count > 0)
            {
                btnsave.Text = "Update";
                txtsupliername.Text = dt.Rows[0]["SupplierName"].ToString();
                txtSupplierBillNo.Text = dt.Rows[0]["SupplierBillNo"].ToString();
                txtBillNo.Text = dt.Rows[0]["BillNo"].ToString();
                hdnBillNo.Value = txtBillNo.Text;
                DateTime BillDate = Convert.ToDateTime(dt.Rows[0]["BillDate"].ToString());
                txtBilldate.Text = BillDate.ToString("dd-MM-yyyy");
                ddlBillAgainst.Text = dt.Rows[0]["BillAgainst"].ToString();
                BindPO(ddlBillAgainst.Text);
                ddlAgainstNumber.SelectedValue = dt.Rows[0]["AgainstNumber"].ToString();
                ddlAgainstNumber.Enabled = false;
                BindInwardByPO();
                ddinwardAgainstNumber.SelectedItem.Text = dt.Rows[0]["OrderNo"].ToString();
                ddinwardAgainstNumber.Enabled = false;
                txtTransportMode.Text = dt.Rows[0]["TransportMode"].ToString();
                txtVehicleNumber.Text = dt.Rows[0]["VehicleNo"].ToString();
                txtPaymentDueDate.Text = dt.Rows[0]["PaymentDueDate"].ToString();
                txtAccontHead.Text = dt.Rows[0]["AccountHead"].ToString();
                txtRemark.Text = dt.Rows[0]["Remarks"].ToString();
                txtEBillNumber.Text = dt.Rows[0]["EBillNumber"].ToString();
                txtDescription.Text = dt.Rows[0]["ChargesDescription"].ToString();
                txtHSN.Text = dt.Rows[0]["HSNSAC"].ToString();
                txtRate.Text = dt.Rows[0]["Rate"].ToString();
                txtBasic.Text = dt.Rows[0]["Basic"].ToString();
                CGSTPer.Text = dt.Rows[0]["CGSTPer"].ToString();
                SGSTPer.Text = dt.Rows[0]["SGSTPer"].ToString();
                IGSTPer.Text = dt.Rows[0]["IGSTPer"].ToString();
                txtCost.Text = dt.Rows[0]["Cost"].ToString();
                txtTCSPer.Text = dt.Rows[0]["TCSPer"].ToString();
                txtTCSAmt.Text = dt.Rows[0]["TCSAmount"].ToString();
                sumofAmount.Text = dt.Rows[0]["sumofAmount"].ToString();
                txtGrandTot.Text = dt.Rows[0]["GrandTotal"].ToString();
                hdnfileData.Value = dt.Rows[0]["RefDocument"].ToString();
                txtTCharge.Text = dt.Rows[0]["TransportationCharges"].ToString();
                txtTCGSTPer.Text = dt.Rows[0]["TCGSTPer"].ToString();
                txtTCGSTamt.Text = dt.Rows[0]["TCGSTAmt"].ToString();
                txtTSGSTPer.Text = dt.Rows[0]["TSGSTPer"].ToString();
                txtTSGSTamt.Text = dt.Rows[0]["TSGSTAmt"].ToString();
                txtTIGSTPer.Text = dt.Rows[0]["TIGSTPer"].ToString();
                txtTIGSTamt.Text = dt.Rows[0]["TIGSTAmt"].ToString();
                txtTCost.Text = dt.Rows[0]["TotalCost"].ToString();

                txtDOR.Text = dt.Rows[0]["DateOfReceived"].ToString();

                if (dt.Rows[0]["RefDocument"].ToString() != "")
                {
                    spnFileUploadData.InnerText = "File Already Exsist, if you can update then update it.";
                }
                else
                {
                    spnFileUploadData.InnerText = "File Not Found";
                }

                binddetails(txtBillNo.Text);

            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    private void Load_OrderNORecord(string ID)
    {
        try
        {
            DataTable dt = Cls_Main.Read_Table("SELECT * FROM [tbl_PendingInwardHdr] WHERE OrderNo='" + ID + "' ");
            if (dt.Rows.Count > 0)
            {
                txtsupliername.Text = dt.Rows[0]["SupplierName"].ToString();
                txtsupliername.ReadOnly = true;
                DateTime BillDate = DateTime.Now;
                txtBilldate.Text = BillDate.ToString("dd-MM-yyyy");
                string pochk = dt.Rows[0]["pono"].ToString();
                if (pochk != "")
                {
                    ddlBillAgainst.SelectedItem.Text = "Order";
                    BindPO(ddlBillAgainst.Text);
                }
                else
                {
                    ddlBillAgainst.SelectedItem.Text = "Verbal";
                    BindPO(ddlBillAgainst.Text);
                    ddlAgainstNumber.SelectedItem.Text = dt.Rows[0]["invoiceno"].ToString();
                }
                ddlBillAgainst.Enabled = false;
                if (pochk != "")
                {
                    ddlAgainstNumber.SelectedItem.Text = dt.Rows[0]["pono"].ToString();
                }
                ddlAgainstNumber.Enabled = false;
                BindInwardByPO();
                ddinwardAgainstNumber.SelectedItem.Text = dt.Rows[0]["OrderNo"].ToString();
                ddinwardAgainstNumber.Enabled = false;
                bindOrderNodetails(ID);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
    public void binddetails(string id)
    {
        DataTable dtt1 = new DataTable();
        SqlDataAdapter sad31 = new SqlDataAdapter(@"select componentname as Particulars,Quantity AS Qty,Quantity AS InQty,Batch AS Batchno,Discountpercentage as Discount, * from tblPurchaseBillDtls  WHERE BillNo = '" + id + "'", con);
        sad31.Fill(dtt1);
        if (dtt1.Rows.Count > 0)
        {
            dgvTaxinvoiceDetails.DataSource = dtt1;
            dgvTaxinvoiceDetails.DataBind();
        }
    }
    public void bindOrderNodetails(string id)
    {
        DataTable dtt1 = new DataTable();
        SqlDataAdapter sad31 = new SqlDataAdapter(@"select componentname as Particulars,Quantity AS Qty,Quantity AS InQty,Batch AS Batchno,Discountpercentage as Discount, * from tbl_PendingInwardDtls  WHERE OrderNo = '" + id + "'", con);
        sad31.Fill(dtt1);
        if (dtt1.Rows.Count > 0)
        {
            dgvTaxinvoiceDetails.DataSource = dtt1;
            dgvTaxinvoiceDetails.DataBind();
        }
    }
    bool flg = false;
    protected void btnsave_Click(object sender, EventArgs e)
    {
        foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
        {
            CheckBox Check = (grd1.FindControl("chkRow") as CheckBox);
            if (Check.Checked == true)
            {
                flg = true;
                break;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Check atleast one checkbox...!!');", true);
                break;
            }
        }
        if (flg == false)
        {
            txtGrandTot.Text = hdnGrandtotal.Value;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please check atleast one checkbox..!!');", true);
        }
        else
        {
            bool isSave = btnsave.Text == "Save";
            if (isSave)
            {
                DataTable Dt = Cls_Main.Read_Table("select dbo.GenerateBillNo() AS BillNo");
                if (Dt.Rows.Count > 0)
                {
                    string BillNo = Dt.Rows[0]["BillNo"].ToString();
                    SaveHeader("insert", BillNo);
                    SaveComponents(BillNo);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Purchase Bill Saved Successfully..!!');window.location='PurchaseBillList.aspx';", true);

                }
            }
            else
            {
                string BillNo = hdnBillNo.Value;
                SaveHeader("Update", BillNo);
                DeleteExistingComponents(BillNo);
                SaveComponents(BillNo);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Purchase Bill Updated Successfully..!!');window.location='PurchaseBillList.aspx';", true);

            }
        }

    }

    private void SaveHeader(string action, string BillNo)
    {

        byte[] bytes = null;
        if (hdnfileData.Value == "")
        {
            if (UploadRefDocs.HasFile)
            {
                string filename = Path.GetFileName(UploadRefDocs.PostedFile.FileName);
                string contentType = UploadRefDocs.PostedFile.ContentType;
                using (Stream fs = UploadRefDocs.PostedFile.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }
            }
        }
        else
        {
            if (UploadRefDocs.HasFile)
            {
                string filename = Path.GetFileName(UploadRefDocs.PostedFile.FileName);
                string contentType = UploadRefDocs.PostedFile.ContentType;
                using (Stream fs = UploadRefDocs.PostedFile.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }
            }
        }
        SqlCommand cmd = new SqlCommand("SP_PurchaseBill", con);
        cmd.Parameters.Clear();
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", action);
        cmd.Parameters.AddWithValue("@SupplierName", txtsupliername.Text.Trim());
        cmd.Parameters.AddWithValue("@SupplierBillNo", txtSupplierBillNo.Text.Trim());
        cmd.Parameters.AddWithValue("@BillNo", BillNo);
        cmd.Parameters.AddWithValue("@BillDate", txtBilldate.Text);
        cmd.Parameters.AddWithValue("@BillAgainst", ddlBillAgainst.Text.Trim());
        cmd.Parameters.AddWithValue("@AgainstNumber", ddlAgainstNumber.SelectedItem.Text.Trim());
        cmd.Parameters.AddWithValue("@OrderNo", ddinwardAgainstNumber.SelectedItem.Text.Trim());

        //DataTable dt = Cls_Main.Read_Table("SELECT OrderNo FROM [tbl_PendingInwardHdr] WHERE pono='" + ddlAgainstNumber.SelectedItem.Text + "' ");
        //if (dt.Rows.Count > 0)
        //{
        //    cmd.Parameters.AddWithValue("@OrderNo", dt.Rows[0]["OrderNo"].ToString());
        //}
        //else
        //{
        //    cmd.Parameters.AddWithValue("@OrderNo", DBNull.Value);
        //}
        cmd.Parameters.AddWithValue("@TransportMode", txtTransportMode.Text.Trim());
        cmd.Parameters.AddWithValue("@VehicleNo", txtVehicleNumber.Text.Trim());
        cmd.Parameters.AddWithValue("@TransportDescription", txtTransportMode.Text.Trim());
        cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text.Trim());
        cmd.Parameters.AddWithValue("@RefDocument", bytes);
        cmd.Parameters.AddWithValue("@AccountHead", txtAccontHead.Text.Trim());
        cmd.Parameters.AddWithValue("@Remarks", txtRemark.Text.Trim());
        cmd.Parameters.AddWithValue("@EBillNumber", txtEBillNumber.Text.Trim());
        cmd.Parameters.AddWithValue("@ChargesDescription", txtDescription.Text.Trim());
        cmd.Parameters.AddWithValue("@HSNSAC", txtHSN.Text.Trim());
        cmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
        cmd.Parameters.AddWithValue("@Basic", txtBasic.Text.Trim());
        cmd.Parameters.AddWithValue("@CGSTPer", CGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@SGSTPer", SGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@IGSTPer", IGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@Cost", hdnFCost.Value);
        cmd.Parameters.AddWithValue("@TCSPer", txtTCSPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TCSAmount", hdnTCSAmount.Value);
        cmd.Parameters.AddWithValue("@sumofAmount", sumofAmount.Text.Trim());
        cmd.Parameters.AddWithValue("@GrandTotal", hdnGrandtotal.Value);
        cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
        cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

        //17 March 2022
        cmd.Parameters.AddWithValue("@TransportationCharges", txtTCharge.Text.Trim());
        cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text.Trim());
        cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text.Trim());
        cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text.Trim());
        cmd.Parameters.AddWithValue("@TotalCost", hdnTCost.Value);
        cmd.Parameters.AddWithValue("@DateOfReceived", txtDOR.Text.Trim());
        int a = 0;
        con.Open();
        a = cmd.ExecuteNonQuery();
        con.Close();

    }

    private void SaveComponents(string BillNo)
    {
        foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
        {
            string txtProduct = (grd1.FindControl("txtProduct") as TextBox).Text ?? string.Empty;
            string txtDescription = (grd1.FindControl("txtDescription") as TextBox).Text ?? string.Empty;
            string txtHSN = (grd1.FindControl("txtHSN") as TextBox).Text ?? string.Empty;
            string txtunit = (grd1.FindControl("txtunit") as TextBox).Text ?? string.Empty;
            string txtQuantity = (grd1.FindControl("txtQuantity") as TextBox).Text ?? "0"; // Default to "0" if null
            string txtRate = (grd1.FindControl("txtRate") as TextBox).Text ?? "0"; // Default to "0" if null
            string txtCGST = (grd1.FindControl("txtCGST") as TextBox).Text ?? "0"; // Default to "0" if null
            string txtSGST = (grd1.FindControl("txtSGST") as TextBox).Text ?? "0"; // Default to "0" if null
            string txtIGST = (grd1.FindControl("txtIGST") as TextBox).Text ?? "0"; // Default to "0" if null
            string txtDiscount = (grd1.FindControl("txtDiscount") as TextBox).Text ?? "0"; // Default to "0" if null
            string txtBatchno = (grd1.FindControl("txtBatchno") as TextBox).Text ?? string.Empty;
            bool chkRow = (grd1.FindControl("chkRow") as CheckBox).Checked;
            if (chkRow == true)
            {
                decimal quantity = 0;
                decimal rate = 0;
                decimal cgst = 0;
                decimal sgst = 0;
                decimal igst = 0;
                decimal discount = 0;

                // Try parsing each field and use 0 if parsing fails
                decimal.TryParse(txtQuantity, out quantity);
                decimal.TryParse(txtRate, out rate);
                decimal.TryParse(txtCGST, out cgst);
                decimal.TryParse(txtSGST, out sgst);
                decimal.TryParse(txtIGST, out igst);
                decimal.TryParse(txtDiscount, out discount);

                var basic = quantity * rate;

                // Initialize GST amounts
                Decimal CGSTAmt = 0;
                Decimal SGSTAmt = 0;
                Decimal IGSTAmt = 0;

                // Calculate GST amounts
                if (igst == 0 && cgst > 0)
                {
                    CGSTAmt = basic * cgst / 100;
                    SGSTAmt = CGSTAmt;
                }
                else if (cgst == 0 && igst > 0)
                {
                    IGSTAmt = basic * igst / 100;
                }

                // Total GST
                decimal gstamt = CGSTAmt + SGSTAmt + IGSTAmt;

                // Discount calculation
                decimal discountAmount = basic * discount / 100;
                decimal afterDiscount = basic - discountAmount;
                decimal taxAmount = gstamt;
                decimal total = afterDiscount + taxAmount;
                total = total != 0 ? total : 0;

                Cls_Main.Conn_Open();
                SqlCommand cmdd1 = new SqlCommand("INSERT INTO [tblPurchaseBillDtls] (BillNo,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,CreatedBy,Batch,IsSelected) VALUES (@BillNo,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@CreatedBy,@Batch,@chkRow)", Cls_Main.Conn);
                cmdd1.Parameters.AddWithValue("@BillNo", BillNo);
                cmdd1.Parameters.AddWithValue("@Particular", txtProduct);
                cmdd1.Parameters.AddWithValue("@ComponentName", txtProduct);
                cmdd1.Parameters.AddWithValue("@Description", txtDescription);
                cmdd1.Parameters.AddWithValue("@HSN", txtHSN);
                cmdd1.Parameters.AddWithValue("@Quantity", txtQuantity);
                cmdd1.Parameters.AddWithValue("@Units", txtunit);
                cmdd1.Parameters.AddWithValue("@Rate", txtRate);
                cmdd1.Parameters.AddWithValue("@CGSTPer", txtCGST);
                cmdd1.Parameters.AddWithValue("@CGSTAmt", CGSTAmt);
                cmdd1.Parameters.AddWithValue("@SGSTPer", txtSGST);
                cmdd1.Parameters.AddWithValue("@SGSTAmt", SGSTAmt);
                cmdd1.Parameters.AddWithValue("@IGSTPer", txtIGST);
                cmdd1.Parameters.AddWithValue("@IGSTAmt", IGSTAmt);
                cmdd1.Parameters.AddWithValue("@Total", basic);
                cmdd1.Parameters.AddWithValue("@Discountpercentage", txtDiscount);
                cmdd1.Parameters.AddWithValue("@DiscountAmount", discountAmount);
                cmdd1.Parameters.AddWithValue("@Alltotal", total.ToString());
                cmdd1.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                cmdd1.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                cmdd1.Parameters.AddWithValue("@Batch", txtBatchno);
                cmdd1.Parameters.AddWithValue("@chkRow", chkRow);
                cmdd1.ExecuteNonQuery();
                Cls_Main.Conn_Close();



                Cls_Main.Conn_Open();
                SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_InwardComponentsdtls] (OrderNo,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                cmdd.Parameters.AddWithValue("@OrderNo", ddinwardAgainstNumber.SelectedItem.Text);
                cmdd.Parameters.AddWithValue("@ComponentName", txtProduct);
                cmdd.Parameters.AddWithValue("@Description", txtDescription);
                cmdd.Parameters.AddWithValue("@HSN", txtHSN);
                cmdd.Parameters.AddWithValue("@Quantity", txtQuantity);
                cmdd.Parameters.AddWithValue("@Units", txtunit);
                cmdd.Parameters.AddWithValue("@Rate", txtRate);
                cmdd.Parameters.AddWithValue("@CGSTPer", txtCGST);
                cmdd.Parameters.AddWithValue("@CGSTAmt", CGSTAmt);
                cmdd.Parameters.AddWithValue("@SGSTPer", txtSGST);
                cmdd.Parameters.AddWithValue("@SGSTAmt", SGSTAmt);
                cmdd.Parameters.AddWithValue("@IGSTPer", txtIGST);
                cmdd.Parameters.AddWithValue("@IGSTAmt", IGSTAmt);
                cmdd.Parameters.AddWithValue("@Total", basic);
                cmdd.Parameters.AddWithValue("@Discountpercentage", txtDiscount);
                cmdd.Parameters.AddWithValue("@DiscountAmount", discountAmount);

                cmdd.Parameters.AddWithValue("@Alltotal", total.ToString());
                cmdd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                cmdd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                cmdd.Parameters.AddWithValue("@Batch", txtBatchno);
                cmdd.ExecuteNonQuery();
                Cls_Main.Conn_Close();



            }


        }
    }

    private void DeleteExistingComponents(string BillNo)
    {
        Cls_Main.Conn_Open();
        SqlCommand deleteCmd1 = new SqlCommand("DELETE FROM tblPurchaseBillDtls WHERE BillNo = @BillNo", Cls_Main.Conn);
        deleteCmd1.Parameters.AddWithValue("@BillNo", BillNo);
        deleteCmd1.ExecuteNonQuery();


        SqlCommand deleteCmd2 = new SqlCommand("DELETE FROM tbl_InwardComponentsdtls WHERE OrderNo = @OrderNo", Cls_Main.Conn);
        deleteCmd2.Parameters.AddWithValue("@OrderNo", ddinwardAgainstNumber.SelectedItem.Text);
        deleteCmd2.ExecuteNonQuery();
        Cls_Main.Conn_Close();


    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("StoreInwardList.aspx");
    }

    protected void dgvTaxinvoiceDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                TextBox txtProduct = (TextBox)e.Row.FindControl("txtProduct");
                TextBox txtDescription = (TextBox)e.Row.FindControl("txtDescription");
                TextBox txtHSN = (TextBox)e.Row.FindControl("txtHSN");
                TextBox txtQuantity = (TextBox)e.Row.FindControl("txtQuantity");
                TextBox txtInQuantity = (TextBox)e.Row.FindControl("txtInQuantity");
                CheckBox Check = (CheckBox)e.Row.FindControl("chkRow");

                if (txtProduct.Text != "")
                {
                    txtProduct.Enabled = false;
                }
                if (txtDescription.Text != "")
                {
                    txtDescription.Enabled = false;
                }
                if (txtHSN.Text != "")
                {
                    txtHSN.Enabled = false;
                }
                if (ddlBillAgainst.SelectedValue != "0")
                {
                    btnAdd.Visible = false;
                }

            }


        }
        catch (Exception ex)
        {

            throw ex;
        }
    }
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
    protected void ddlComponent_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlComponent = (DropDownList)sender;
        GridViewRow row = (GridViewRow)ddlComponent.NamingContainer;
        DropDownList ddlBatch = (DropDownList)row.FindControl("ddlBatch");

        if (ddlComponent.SelectedItem.Text != null)
        {
            SqlDataAdapter ad = new SqlDataAdapter("select Batch from vw_batchwisecomponent where BalanceQty>0 AND ComponentName='" + ddlComponent.SelectedItem.Text + "' ", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddlBatch.DataSource = dt;
                ddlComponent.DataTextField = "Batch";
                ddlComponent.DataValueField = "Batch";
                ddlBatch.DataBind();
            }
            else
            {
                ddlBatch.DataSource = null;
                //        //ddlBatch.DataValueField = "ID";
                ddlBatch.DataTextField = "Data Not Found....!";
                ddlBatch.DataBind();
                ddlBatch.Items.Insert(0, "Not Available....!");
            }

        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseBillList.aspx");
    }
    protected void txtsupliername_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT EmailID from tbl_VendorMaster where Vendorname='" + txtsupliername.Text.Trim() + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblEmailID.Text = dt.Rows[0]["EmailID"].ToString() == "" ? "Email Id not found" : dt.Rows[0]["EmailID"].ToString();
        }
        else
        {
            lblEmailID.Text = "Email Id not found";
        }
    }
    protected void txtSupplierBillNo_TextChanged(object sender, EventArgs e)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM tblPurchaseBillHdr WHERE SupplierBillNo='" + txtSupplierBillNo.Text + "' AND SupplierName='" + txtsupliername.Text + "' ", con);
        Da.Fill(Dt);

        if (Dt.Rows.Count > 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Supplier Bill No Alredy Exist...')", true);
        }
    }
    protected void ddlBillAgainst_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (txtsupliername.Text != "")
        {
            if (ddlBillAgainst.SelectedItem.Text == "Order")
            {

                BindPO("Order");
                ddlAgainstNumber.Enabled = true;

            }
            else if (ddlBillAgainst.SelectedItem.Text == "Verbal")
            {
                BindPO("Verbal");
                ddlAgainstNumber.Enabled = true;

            }
            else
            {
                ddlAgainstNumber.Enabled = false;

            }
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please Select Supplier Name !!');", true);
        }
    }
    protected void BindPO(string type)
    {
        string query = string.Empty;

        if (type == "Order")
        {
            query = "SELECT Distinct PONo FROM tbl_PendingInwardHdr where SupplierName='" + txtsupliername.Text.Trim() + "'AND pono IS NOT NULL";
        }
        else if (type == "Verbal")
        {
            query = @"select Distinct InvoiceNo AS PONo from tbl_PendingInwardHdr AS PH
                      INNER JOIN tbl_PendingInwardDtls AS  PD ON PD.OrderNo=PH.OrderNo where SupplierName='" + txtsupliername.Text.Trim() + "' AND pono IS NULL";
        }
        else
        {
            query = "SELECT Id,PONo FROM tbl_PendingInwardHdr where EntryType=2 ";
        }
        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlAgainstNumber.DataSource = dt;
            ddlAgainstNumber.DataBind();
            ddlAgainstNumber.DataTextField = "PONo";
            ddlAgainstNumber.DataValueField = "PONo";
            ddlAgainstNumber.DataBind();
        }
        ddlAgainstNumber.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select Order--", "0"));
    }
    protected void ddlAgainstNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            getOrderDatailsdts();
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    protected void ddlBindInwardChnage(object sender, EventArgs e)
    {
        BindInwardByPO();
    }
    protected void BindInwardByPO()
    {
        string query = string.Empty;
        if (ddlBillAgainst.SelectedItem.Text == "Order")
        {

            query = " SELECT OrderNo FROM tbl_PendingInwardHdr where SupplierName='" + txtsupliername.Text.Trim() + "'AND pono IS NOT NULL  AND pono ='" + ddlAgainstNumber.SelectedItem.Text + "' " +
                     " AND OrderNo NOT IN((SELECT OrderNo FROM tblPurchaseBillHdr WHERE OrderNo IS NOT NULL)) " +
                     " order by CAST(SUBSTRING(OrderNo, CHARINDEX('-', OrderNo) + 1, LEN(OrderNo)) AS INT) DESC ";
            if (Request.QueryString["OrderNo"] != null)
            {
                ddinwardAgainstNumber.Enabled = false;
            }
            else
            {
                ddinwardAgainstNumber.Enabled = true;
            }
        }
        else if (ddlBillAgainst.SelectedItem.Text == "Verbal")
        {
            query = " SELECT OrderNo FROM tbl_PendingInwardHdr where SupplierName='" + txtsupliername.Text.Trim() + "'AND pono IS NULL AND invoiceno ='" + ddlAgainstNumber.SelectedItem.Text + "' " +
                     " AND OrderNo NOT IN((SELECT OrderNo FROM tblPurchaseBillHdr WHERE OrderNo IS NOT NULL)) " +
                     " order by CAST(SUBSTRING(OrderNo, CHARINDEX('-', OrderNo) + 1, LEN(OrderNo)) AS INT) DESC ";
        }
        else
        {
            query = "SELECT Id,PONo FROM tbl_PendingInwardHdr where EntryType=2 ";
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        if (query != "")
        {
            ad.Fill(dt);
        }
        if (dt.Rows.Count > 0)
        {

            ddinwardAgainstNumber.DataSource = dt;
            ddinwardAgainstNumber.DataBind();
            ddinwardAgainstNumber.DataTextField = "OrderNo";
            ddinwardAgainstNumber.DataValueField = "OrderNo";
            ddinwardAgainstNumber.DataBind();

        }
        else
        {
            ddinwardAgainstNumber.DataSource = "No Data";
            ddinwardAgainstNumber.DataBind();
        }
        ddinwardAgainstNumber.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select Inward--", "0"));
    }
    protected void getOrderDatailsdts()
    {
        string query = string.Empty;
        if (ddlBillAgainst.SelectedValue == "Order")
        {
            query = @"select ComponentName AS Particulars,Rate,0 AS IsSelected,PD.ID,Description,HSN,Units,Quantity AS Qty,Quantity AS InQty,CGSTPer,SGSTPer,IGSTPer,Discountpercentage AS Discount,PD.Batch AS Batchno from tbl_PendingInwardHdr AS PH
INNER JOIN tbl_PendingInwardDtls AS  PD ON PD.OrderNo = PH.OrderNo where pono ='" + ddlAgainstNumber.SelectedValue + "' AND PD.OrderNo='" + ddinwardAgainstNumber.SelectedValue + "'AND IsSelected=1  ";
        }
        else if (ddlBillAgainst.SelectedValue == "Verbal")
        {
            query = @"select ComponentName AS Particulars,Rate,0 AS IsSelected,PD.ID,Description,HSN,Units,Quantity AS Qty,Quantity AS InQty,CGSTPer,SGSTPer,IGSTPer,Discountpercentage AS Discount,PD.Batch AS Batchno from tbl_PendingInwardHdr AS PH
INNER JOIN tbl_PendingInwardDtls AS  PD ON PD.OrderNo=PH.OrderNo where Invoiceno='" + ddlAgainstNumber.SelectedValue + "' AND IsSelected=1 ";
        }

        string ID = ddlAgainstNumber.SelectedValue;
        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            dgvTaxinvoiceDetails.DataSource = dt;
            dgvTaxinvoiceDetails.DataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('PO Details Not Found !!');", true);
        }

    }
    protected void txtDOR_TextChanged(object sender, EventArgs e)
    {
        //DateTime fromdate = Convert.ToDateTime(txtBilldate.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        //DateTime todate = Convert.ToDateTime(txtDOR.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        //if (fromdate > todate)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Bill Date is greater than Received Date...Please Choose Correct Date.');", true);
        //    btnadd.Enabled = false;
        //}
        //else
        //{
        //    btnadd.Enabled = true;
        //}
    }


    [ScriptMethod()]
    [WebMethod]
    public static List<string> GetSupplierList(string prefixText, int count)
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
                com.CommandText = "Select DISTINCT [Vendorname] from [tbl_VendorMaster] where Vendorname like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Vendorname"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {

        DataTable dt = new DataTable();
        dt.Columns.Add("IsSelected");
        dt.Columns.Add("id");
        dt.Columns.Add("Particulars", typeof(string));
        dt.Columns.Add("Description");
        dt.Columns.Add("HSN");
        dt.Columns.Add("Qty");
        dt.Columns.Add("Units");
        dt.Columns.Add("Rate");
        dt.Columns.Add("CGSTPer");
        dt.Columns.Add("SGSTPer");
        dt.Columns.Add("IGSTPer");
        dt.Columns.Add("Discount");
        dt.Columns.Add("Batchno");

        if (dgvTaxinvoiceDetails.Rows.Count > 0)
        {
            foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
            {

                TextBox Particulars = (grd1.FindControl("txtProduct") as TextBox);
                string Description = (grd1.FindControl("txtDescription") as TextBox).Text;
                string HSN = (grd1.FindControl("txtHSN") as TextBox).Text;
                string Qty = (grd1.FindControl("txtQuantity") as TextBox).Text;
                string unit = (grd1.FindControl("txtunit") as TextBox).Text;
                string Rate = (grd1.FindControl("txtRate") as TextBox).Text;
                string CGSTPer = (grd1.FindControl("txtCGST") as TextBox).Text;
                string SGSTPer = (grd1.FindControl("txtSGST") as TextBox).Text;
                string IGSTPer = (grd1.FindControl("txtIGST") as TextBox).Text;
                string Discount = (grd1.FindControl("txtDiscount") as TextBox).Text;
                string Batchno = (grd1.FindControl("txtBatchno") as TextBox).Text;

                dt.Rows.Add(false, "", Particulars.Text, Description, HSN, Qty, unit, Rate, CGSTPer, SGSTPer, IGSTPer, Discount, Batchno);
            }

        }

        dt.Rows.Add(false, "", "", "", "", "", "", "", "", "", "", "", "");

        dgvTaxinvoiceDetails.DataSource = dt;
        dgvTaxinvoiceDetails.DataBind();
    }

    [WebMethod]
    public static List<ListItem> GetComponent(string Component)
    {
        List<ListItem> Result = new List<ListItem>();
        SqlDataAdapter ad = new SqlDataAdapter("select ID,ComponentName from tbl_ComponentMaster where Status=1 and IsDeleted=0 AND  ComponentName like '%'+ '" + Component + "' + '%'", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            foreach (DataRow row in dt.Rows)
            {
                Result.Add(new ListItem(row["ComponentName"].ToString(), row["ID"].ToString()));
            }
        }
        return Result;
    }

}


