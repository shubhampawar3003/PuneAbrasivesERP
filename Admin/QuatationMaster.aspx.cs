
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_QuatationMaster : System.Web.UI.Page
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
                ViewState["State"] = null;
                ViewState["RowNo"] = 0;
                Dt_Product.Columns.AddRange(new DataColumn[17] { new DataColumn("id"), new DataColumn("Productname"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
                ViewState["QuotationProduct"] = Dt_Product;
                QuotationCode(); FillddlProduct();

                txtquotationdate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtvalidtill.Text = DateTime.Now.ToString("yyyy-MM-dd");


                if (Request.QueryString["ID"] != null)
                {
                    string action = Request.QueryString["Action"].ToString();
                    if (action == "OLD")
                    {
                        string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                        hhd.Value = Id;
                        Load_Record(Id);
                        txtquotationno.ReadOnly = true;
                        btnsave.Text = "Update";
                        ShowDtlEdit();

                    }

                    if (action == "NEW")
                    {
                        txtcompanyname.Enabled = false;
                        txtmobileno.Enabled = false;
                        txtemail.Enabled = false;
                        txtgstno.Enabled = false;
                        btnsave.Text = "Save";
                        string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                        Load_Record(Id);
                        ShowDtlEdit();
                        QuotationCode();
                        txtquotationdate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    }
                }

                if (Request.QueryString["CODE"] != null)
                {
                    string code = objcls.Decrypt(Request.QueryString["CODE"].ToString());
                    EID.Value = code;
                    DataTable Dt = Cls_Main.Read_Table("select ccode from tbl_EnquiryData where id='" + code + "' ");
                    Load_Company_Details(Dt.Rows[0]["ccode"].ToString());

                }
            }

        }
    }
    private void Load_Company_Details(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CompanyMaster] AS CM LEFT JOIN tbl_CompanyContactDetails AS CC ON CC.CompanyCode=CM.CompanyCode WHERE CM.CompanyCode='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            Fillddlshippingaddress(Dt.Rows[0]["Companyname"].ToString());
            hhccode.Value = Dt.Rows[0]["CompanyCode"].ToString();
            txtcompanyname.Text = Dt.Rows[0]["Companyname"].ToString();
            txtgstno.Text = Dt.Rows[0]["GSTno"].ToString();

            ViewState["State"] = Dt.Rows[0]["Billing_statecode"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
            divTotalPart.Visible = true;
            FillKittens();

            SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM tbl_EnquiryData AS ED INNER JOIN tbl_EnquiryDtls AS ELS ON ELS.Invoiceno=ED.EnqCode WHERE ED.id='" + EID.Value + "'", Cls_Main.Conn);
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
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please fill company details..!!');window.location='EnquiryList.aspx'; ", true);

        }
    }

    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_QuotationHdr] WHERE Quotationno ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {

            hhccode.Value = Dt.Rows[0]["CompanyCode"].ToString();
            txtcompanyname.Text = Dt.Rows[0]["Companyname"].ToString();
            string number = Dt.Rows[0]["Quotationno"].ToString();
            txtquotationno.Text = Revicecode(number);
            FillKittens();
            DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["Quotationdate"].ToString());
            txtquotationdate.Text = ffff1.ToString("yyyy-MM-dd");
            DateTime ffff2 = Convert.ToDateTime(Dt.Rows[0]["Validdate"].ToString());
            txtvalidtill.Text = ffff1.ToString("yyyy-MM-dd");
            ddlContacts.SelectedItem.Text = Dt.Rows[0]["Customername"].ToString();
            txtmobileno.Text = Dt.Rows[0]["Mobileno"].ToString();
            txtemail.Text = Dt.Rows[0]["Emailid"].ToString();
            txtgstno.Text = Dt.Rows[0]["Gstno"].ToString();
            txtRef.Text = Dt.Rows[0]["Ref"].ToString();
            if (!string.IsNullOrEmpty(Dt.Rows[0]["Gstno"].ToString()))
            {
                string gstNumber = txtgstno.Text.Trim();
                ViewState["State"] = gstNumber.Substring(0, 2);

            }
            Fillddlshippingaddress(Dt.Rows[0]["Companyname"].ToString());
            ddlShippingaddress.SelectedItem.Text = Dt.Rows[0]["Shippingaddress"].ToString();
            ddlBillAddress.SelectedItem.Text = Dt.Rows[0]["Shippingaddress"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
            txtPayment.Text = Dt.Rows[0]["Payment"].ToString();
            txtTransport.Text = Dt.Rows[0]["Transport"].ToString();
            txtDeliveryTime.Text = Dt.Rows[0]["DeliveryTime"].ToString();
            txtPacking.Text = Dt.Rows[0]["Packing"].ToString();
            txtTaxs.Text = Dt.Rows[0]["Taxs"].ToString();

            lblfile1.Text = Dt.Rows[0]["fileName"].ToString();
            string fileName = Dt.Rows[0]["fileName"].ToString();
            if (!string.IsNullOrEmpty(Dt.Rows[0]["fileContent"].ToString()))
            {
                byte[] fileContent = (byte[])Dt.Rows[0]["fileContent"]; // Assuming this is a byte array
                ViewState["attachment"] = null;
                // Store the byte array directly in ViewState
                ViewState["attachment"] = fileContent;
                lblfile1.Text = fileName;
            }
        }
    }
    //Revised quotation number genrate
    public static string Revicecode(string number)
    {
        string total = string.Empty;
        string Lastresult = number.Substring(number.Length - 2);
        string Firstresult = number.Substring(0, number.Length - 2);
        if (Lastresult == "R0")
        {
            total = Firstresult + "R1";
        }
        if (Lastresult == "R1")
        {
            total = Firstresult + "R2";
        }
        if (Lastresult == "R2")
        {
            total = Firstresult + "R3";
        }
        if (Lastresult == "R3")
        {
            total = Firstresult + "R4";
        }
        if (Lastresult == "R4")
        {
            total = Firstresult + "R5";
        }
        if (Lastresult == "R5")
        {
            total = Firstresult + "R6";
        }
        if (Lastresult == "R6")
        {
            total = Firstresult + "R7";
        }
        return total;
    }
    protected void ShowDtlEdit()
    {
        divTotalPart.Visible = true;
        string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_QuotationDtls] WHERE Quotation_no='" + Id + "'", Cls_Main.Conn);
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
    //Quotation Code Auto
    protected void QuotationCode()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT MAX(CAST(SUBSTRING(Quotationno, LEN('PAPL/QN-') + 1, CASE WHEN CHARINDEX('-R', Quotationno) > 0 THEN CHARINDEX('-R', Quotationno) - LEN('PAPL/QN-') - 1 ELSE LEN(Quotationno) - LEN('PAPL/QN-') END) AS INT)) AS MaxCodeNumber FROM tbl_QuotationHdr ;", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["MaxCodeNumber"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["MaxCodeNumber"].ToString());
            txtquotationno.Text = "PAPL/QN-" + (maxid + 1).ToString() + "-R0";
            //string original = dt.Rows[0]["Quotationno"].ToString();
            //// string original = "PAPL/QN-10-R0";
            //string[] parts = original.Split('-');

            //if (parts.Length >= 2)
            //{
            //    // Get the numeric part and increment it
            //    string numericPart = dt.Rows[0]["MaxCodeNumber"].ToString();
            //    Double total = Convert.ToDouble(numericPart) + 1;
            //    string words = parts[0];
            //    txtquotationno.Text = words + "-" + total + "-R0";
            //}
            //else
            //{
            //    Console.WriteLine("The format of the string is not valid.");
            //}
        }
        else
        {
            txtquotationno.Text = string.Empty;
        }
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
    bool flgs;
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {

            if (txtcompanyname.Text == "" || ddlContacts.SelectedItem.Text == "" || txtmobileno.Text == "" || txtemail.Text == "" || txtvalidtill.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {

                if (dgvMachineDetails.Rows.Count > 0)
                {
                    Cls_Main.Conn_Open();
                    SqlCommand cmd = new SqlCommand("SP_QuotationHdr", Cls_Main.Conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Companyname", txtcompanyname.Text);
                    cmd.Parameters.AddWithValue("@CompanyCode", hhccode.Value);
                    cmd.Parameters.AddWithValue("@Quotationno", txtquotationno.Text);
                    cmd.Parameters.AddWithValue("@Quotationdate", txtquotationdate.Text);
                    cmd.Parameters.AddWithValue("@Validdate", txtvalidtill.Text);
                    cmd.Parameters.AddWithValue("@Customername", ddlContacts.SelectedItem.Text);
                    cmd.Parameters.AddWithValue("@Mobileno", txtmobileno.Text);
                    cmd.Parameters.AddWithValue("@Emailid", txtemail.Text);
                    cmd.Parameters.AddWithValue("@Gstno", txtgstno.Text);
                    cmd.Parameters.AddWithValue("@Payment", txtPayment.Text);
                    cmd.Parameters.AddWithValue("@Transport", txtTransport.Text);
                    cmd.Parameters.AddWithValue("@DeliveryTime", txtDeliveryTime.Text);
                    cmd.Parameters.AddWithValue("@Packing", txtPacking.Text);
                    cmd.Parameters.AddWithValue("@Taxs", txtTaxs.Text);
                    cmd.Parameters.AddWithValue("@Ref", txtRef.Text);

                    if (ViewState["attachment"] != null)
                    {
                        byte[] fileContent = (byte[])ViewState["attachment"];
                        cmd.Parameters.AddWithValue("@fileName", lblfile1.Text);
                        cmd.Parameters.AddWithValue("@fileContent", fileContent);

                    }


                    cmd.Parameters.AddWithValue("@Billingaddress", ddlBillAddress.SelectedItem.Text);
                    if (ddlShippingaddress.SelectedItem.Text != "-Select Shipping Address-")
                    {
                        cmd.Parameters.AddWithValue("@Shippingaddress", ddlShippingaddress.SelectedItem.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Shippingaddress", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@CGST_Amt", txt_cgstamt.Text);
                    cmd.Parameters.AddWithValue("@SGST_Amt", txt_sgstamt.Text);
                    cmd.Parameters.AddWithValue("@IGST_Amt", txt_igstamt.Text);
                    cmd.Parameters.AddWithValue("@PaymentTerm", txtPaymentTerm.Text);
                    cmd.Parameters.AddWithValue("@Total_Price", txt_grandTotal.Text);
                    cmd.Parameters.AddWithValue("@Totalinword", lbl_total_amt_Value.Text);
                    cmd.Parameters.AddWithValue("@IsDeleted", '0');
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Action", "Save");
                    if (EID.Value != null)
                    {
                        cmd.Parameters.AddWithValue("@EID", EID.Value);
                        cmd.Parameters.AddWithValue("@ISEID", true);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@EID", "");
                        cmd.Parameters.AddWithValue("@ISEID", false);
                    }
                    cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    if (Request.QueryString["ID"] != null)
                    {
                        string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                        if (Id != null)
                        {
                            Cls_Main.Conn_Open();
                            SqlCommand cmdUpdate = new SqlCommand("update tbl_QuotationHdr set Status=0 where Quotationno=@Quotationno", Cls_Main.Conn);
                            cmdUpdate.Parameters.AddWithValue("@Quotationno", Id);
                            cmdUpdate.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }
                    }
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
                        SqlCommand cmdd = new SqlCommand("INSERT INTO tbl_QuotationDtls (Quotation_no,Productname,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn) VALUES(@Quotation_no,@Productname,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn)", Cls_Main.Conn);
                        cmdd.Parameters.AddWithValue("@Quotation_no", txtquotationno.Text);
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

                    if (EID.Value != null && EID.Value != "")
                    {
                        Cls_Main.Conn_Open();
                        SqlCommand cmdUpdate = new SqlCommand("update tbl_EnquiryData set Status=2 where id=@id", Cls_Main.Conn);
                        cmdUpdate.Parameters.AddWithValue("@id", EID.Value);
                        cmdUpdate.ExecuteNonQuery();
                        Cls_Main.Conn_Close();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Quotation Save Successfully..!!');window.location='EnquiryList.aspx'; ", true);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Quotation Save Successfully..!!');window.location='QuatationList.aspx'; ", true);
                    }



                    //if (CheckBox1.Checked == true)
                    //{
                    //    ViewState["ID"] = hhccode.Value;
                    //    mailsendforCustomer();
                    //}
                    if (gvUserMail.Rows.Count > 0)
                    {
                        foreach (GridViewRow g2 in gvUserMail.Rows)
                        {
                            bool chk = (g2.FindControl("chkSelect") as CheckBox).Checked;

                            while (chk == true)
                            {
                                flgs = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        flgs = false;
                    }
                    if (flgs == true)
                    {
                        ViewState["ID"] = hhccode.Value;
                        mailsendforCustomerContact();

                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Quotation Save Successfully..!!');window.location='QuatationList.aspx'; ", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Product Data..!!')", true);
                }

            }
        }
        catch (Exception ex)
        {
            throw ex;
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

                if (txtIGST.Text == null || txtIGST.Text == "")
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
    string mailTo = string.Empty;
    byte[] bytefile;
    private void mailsendforCustomerContact()
    {
        try
        {
            Report(txtquotationno.Text);
            foreach (GridViewRow g2 in gvUserMail.Rows)
            {
                CheckBox chkBox = (CheckBox)g2.FindControl("chkSelect");
                if (chkBox != null && chkBox.Checked)
                {
                    string fileName = ViewState["ID"].ToString() + "-" + "QuotationInvoice.pdf";
                    string strMessage = "Hello Sir/Ma'am " + ddlContacts.SelectedItem.Text.Trim() + "<br/>" +
                 "We wish to thank you for your valued inquiry towards " + "<strong>Pune Abrasives Pvt. Ltd....!!!<strong>" + "<br/>" +
                    "We sent you an Quotation." + "Quotation - " + txtquotationno.Text.Trim() + "/" + txtquotationdate.Text.Trim() + ".pdf" + "<br/>" +

                    "Please find herewith attached best offer for your reference." + "<br/>" +

                    "Hope this offer is in line with your requirements." + "<br/>" +

                    "Looking forward to your valuable reply with Quotation." + "<br/>" +

                    "Feel free to contact us for any further queries & Clarifications." + "<br/>" +

                    "Kind Regards," + "<br/>" +
                    "<strong>Pune Abrasives Pvt. Ltd.<strong>";
                    string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
                    string mailTo = ((Label)g2.FindControl("lblEmailID")).Text.Trim();
                    MailMessage mm = new MailMessage();
                    //  mm.From = new MailAddress(fromMailID);

                    mm.Subject = ((Label)g2.FindControl("lblQuotationno")).Text.Trim() + " - Quotation";
                    mm.To.Add(mailTo);

                    mm.CC.Add("girish.kulkarni@puneabrasives.com");
                    mm.CC.Add("b.tikhe@puneabrasives.com");
                    mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        //byte[] file = File.ReadAllBytes(ViewState["file"].ToString());

                        Stream stream = new MemoryStream(bytefile);
                        Attachment aa = new Attachment(stream, fileName);
                        mm.Attachments.Add(aa);
                    }
                    if (!string.IsNullOrEmpty(lblfile1.Text))
                    {
                        byte[] fileContent = (byte[])ViewState["attachment"];
                        Stream stream = new MemoryStream(fileContent);
                        Attachment aa = new Attachment(stream, lblfile1.Text);
                        mm.Attachments.Add(aa);
                    }
                    StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
                    string readFile = reader.ReadToEnd();
                    string myString = "";
                    myString = readFile;

                    string multilineText = strMessage;
                    string formattedText = multilineText.Replace("\n", "<br />");
                    mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);
                    myString = myString.Replace("$Comment$", formattedText);
                    mm.ReplyToList.Add(new MailAddress(fromMailID));
                    mm.Body = myString.ToString();
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["Host"]; ;
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
                    smtp.Send(mm);
                }
            }

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }

    }
    //private void mailsendforCustomer()
    //{
    //    try
    //    {
    //        Report(txtquotationno.Text);
    //        string strMessage = "Hello Sir/Ma'am " + ddlContacts.SelectedItem.Text.Trim() + "<br/>" +
    //          "We wish to thank you for your valued inquiry towards " + "<strong>Pune Abrasives Pvt. Ltd.....!!!<strong>" + "<br/>" +
    //             "We sent you an Quotation." + "Quotation - " + txtquotationno.Text.Trim() + "/" + txtquotationdate.Text.Trim() + ".pdf" + "<br/>" +

    //             "Please find herewith attached best offer for your reference." + "<br/>" +

    //             "Hope this offer is in line with your requirements." + "<br/>" +

    //             "Looking forward to your valuable reply with Quotation." + "<br/>" +

    //             "Feel free to contact us for any further queries & Clarifications." + "<br/>" +

    //             "Kind Regards," + "<br/>" +
    //             "<strong>Pune Abrasives Pvt. Ltd.<strong>";
    //        string fileName = ViewState["ID"].ToString() + "-" + "QuotationInvoice.pdf";
    //        string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
    //        string mailTo = txtemail.Text.Trim().ToLower();
    //        MailMessage mm = new MailMessage();
    //        // mm.From = new MailAddress(fromMailID);

    //        mm.Subject = txtcompanyname.Text + "_Quotation.pdf";
    //        mm.To.Add(mailTo);

    //        mm.CC.Add("girish.kulkarni@puneabrasives.com");
    //        mm.CC.Add("b.tikhe@puneabrasives.com");
    //        mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
    //        if (!string.IsNullOrEmpty(fileName))
    //        {
    //            //byte[] file = File.ReadAllBytes(ViewState["file"].ToString());

    //            Stream stream = new MemoryStream(bytefile);
    //            Attachment aa = new Attachment(stream, fileName);
    //            mm.Attachments.Add(aa);
    //        }
    //        if (!string.IsNullOrEmpty(lblfile1.Text))
    //        {
    //            byte[] fileContent = (byte[])ViewState["attachment"];
    //            Stream stream = new MemoryStream(fileContent);
    //            Attachment aa = new Attachment(stream, lblfile1.Text);
    //            mm.Attachments.Add(aa);
    //        }
    //        mm.Body = strMessage;
    //        mm.IsBodyHtml = true;
    //        //mm.From = new MailAddress("girish.kulkarni@puneabrasives.com", fromMailID);
    //        mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);

    //        // Set the "Reply-To" header to indicate the desired display address
    //        mm.ReplyToList.Add(new MailAddress(fromMailID));
    //        //SmtpClient SmtpMail = new SmtpClient();
    //        //SmtpMail.Host = "us2.smtp.mailhostbox.com"; // Name or IP-Address of Host used for SMTP transactions  
    //        //SmtpMail.Port = 25; // Port for sending the mail  
    //        //SmtpMail.Credentials = new System.Net.NetworkCredential("girish.kulkarni@puneabrasives.com", "Qi#dKZN1"); // Username/password of network, if apply  
    //        //SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
    //        //SmtpMail.EnableSsl = false;

    //        //SmtpMail.ServicePoint.MaxIdleTime = 0;
    //        //SmtpMail.ServicePoint.SetTcpKeepAlive(true, 2000, 2000);
    //        //mm.BodyEncoding = Encoding.Default;
    //        //mm.Priority = MailPriority.High;
    //        //SmtpMail.Send(mm);
    //        SmtpClient smtp = new SmtpClient();
    //        smtp.Host = ConfigurationManager.AppSettings["Host"]; ;
    //        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
    //        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
    //        NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
    //        NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToString();

    //        smtp.UseDefaultCredentials = false;
    //        smtp.Credentials = NetworkCred;
    //        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
    //        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
    //        {
    //            return true;
    //        };
    //        smtp.Send(mm);
    //    }
    //    catch (Exception ex)
    //    {
    //        string errorMsg = "An error occurred : " + ex.Message;
    //        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
    //    }
    //}

    public void Report(string Invoiceno)
    {
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetQuotationDetails");
                    cmd.Parameters.AddWithValue("@Invoiceno", Invoiceno);

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(Dtt);

                        }
                    }
                }
            }

            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\Quotation.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    bytefile = bytePdfRep;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    //string Filename = Invoiceno + "_Quotation.pdf";
                    //Response.ContentType = "application/vnd.pdf";
                    //Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "");
                    //Response.BinaryWrite(bytePdfRep);
                    //ReportViewer1.LocalReport.DataSources.Clear();
                    //ReportViewer1.Reset();

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }
    }

    private void Fillddlshippingaddress(string ID)
    {
        try
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT SA.ShippingAddress FROM tbl_ShippingAddress  AS SA INNER JOIN tbl_CompanyMaster AS CM ON CM.ID=SA.c_id where Companyname='" + ID + "'", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ddlShippingaddress.DataSource = dt;
                ddlShippingaddress.DataValueField = "ShippingAddress";
                ddlShippingaddress.DataTextField = "ShippingAddress";
                ddlShippingaddress.DataBind();
                ddlShippingaddress.Items.Insert(0, "-Select Shipping Address-");
            }
            else
            {
                ddlShippingaddress.DataSource = null;
                ddlShippingaddress.DataBind();
                ddlShippingaddress.Items.Insert(0, "-Select Shipping Address-");
            }


            SqlDataAdapter ad1 = new SqlDataAdapter("SELECT SA.BillAddress FROM tbl_BillingAddress  AS SA INNER JOIN tbl_CompanyMaster AS CM ON CM.ID=SA.c_id where Companyname='" + ID + "'", Cls_Main.Conn);
            DataTable dt1 = new DataTable();
            ad1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                ddlBillAddress.DataSource = dt1;
                ddlBillAddress.DataValueField = "BillAddress";
                ddlBillAddress.DataTextField = "BillAddress";
                ddlBillAddress.DataBind();
                ddlBillAddress.Items.Insert(0, "-Select Billing Address-");
            }
            else
            {
                ddlBillAddress.DataSource = null;
                ddlBillAddress.DataBind();
                ddlBillAddress.Items.Insert(0, "-Select Billing Address-");
            }
        }
        catch { }


    }

    private void FillddlProduct()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT[ProductName] FROM[tbl_ProductMaster] WHERE status=1 AND isdeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlProduct.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            ddlProduct.DataTextField = "ProductName";
            ddlProduct.DataBind();
            ddlProduct.Items.Insert(0, "-Select Product-");
        }
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
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
        //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
        //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void dgvMachineDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvMachineDetails.EditIndex = e.NewEditIndex;
        dgvMachineDetails.DataSource = (DataTable)ViewState["QuotationProduct"];
        dgvMachineDetails.DataBind();
        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
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
        Response.Redirect("CompanyMaster.aspx");
    }

    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("QuatationList.aspx");
    }

    protected void txtQuantity_TextChanged(object sender, EventArgs e)
    {
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

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculations(row);
    }

    protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
    {
        if (CheckBox1.Checked == true)
        {
            CompanyContactUsers.Visible = true;
            SqlDataAdapter ad = new SqlDataAdapter("select * from tbl_CompanyContactDetails where CompanyCode='" + hhccode.Value + "' ", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                gvUserMail.DataSource = dt;
                gvUserMail.DataBind();

            }
        }
    }

    protected void chkHeader_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in gvUserMail.Rows)
        {
            CheckBox ChkHeader = (CheckBox)gvUserMail.HeaderRow.FindControl("chkHeader");
            CheckBox ChkRow = (CheckBox)row.FindControl("chkSelect");
            if (ChkHeader.Checked == true)
            {
                ChkRow.Checked = true;
            }
            else
            {
                ChkRow.Checked = false;
            }
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("QuatationList.aspx");
    }

    protected void txtcompanyname_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select CM.Companyname,CM.ID, CM.PaymentTerm,CM.StateCode,CC.Name,CM.CompanyCode,CC.Number,CM.PrimaryEmailID,CM.GSTno,CM.Billingaddress,Shippingaddress from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode WHERE Companyname='" + txtcompanyname.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            FillKittens();
            Fillddlshippingaddress(Dt.Rows[0]["Companyname"].ToString());
            hhccode.Value = Dt.Rows[0]["CompanyCode"].ToString();
            //ddlContacts.SelectedItem.Text = Dt.Rows[0]["Name"].ToString();
            // txtmobileno.Text = Dt.Rows[0]["Number"].ToString();
            hhdstate.Value = Dt.Rows[0]["StateCode"].ToString();
            // txtemail.Text = Dt.Rows[0]["PrimaryEmailID"].ToString();
            txtgstno.Text = Dt.Rows[0]["GSTno"].ToString();
            if (!string.IsNullOrEmpty(Dt.Rows[0]["GSTno"].ToString()))
            {
                string gstNumber = txtgstno.Text.Trim();
                ViewState["State"] = gstNumber.Substring(0, 2);

            }
            // txtaddress.Text = Dt.Rows[0]["Billingaddress"].ToString();
            //ddlShippingaddress.SelectedItem.Text = Dt.Rows[0]["Shippingaddress"].ToString();
            txtPaymentTerm.Text = Dt.Rows[0]["PaymentTerm"].ToString();
        }
    }

    protected void ddlProduct_TextChanged(object sender, EventArgs e)
    {
        try
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
                //string gstNumber = txtgstno.Text.Trim();
                //string stateCode = gstNumber.Substring(0, 2);
                //hhdstate.Value = stateCode;

                if (ViewState["State"].ToString() == "27")
                {
                    txtCGST.Text = "9";
                    txtSGST.Text = "9";
                }
                else
                {
                    txtIGST.Text = "18";
                }
                txtCGSTamt.Text = "0.00";
                txtSGSTamt.Text = "0.00";
                txtIGSTamt.Text = "0.00";
                txtdiscount.Text = "0.00";
                txtdiscountamt.Text = "0.00";
                //ddlProduct.Focus();

            }
        }
        catch
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please try again..!!')", true);
        }
    }

    protected void lnkEditCompany_Click(object sender, EventArgs e)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT ID FROM [tbl_CompanyMaster] where CompanyCode='" + hhccode.Value + "' ");
        Response.Redirect("CompanyMaster.aspx?ID=" + encrypt(Dt.Rows[0]["ID"].ToString()) + "&CODE=" + encrypt(EID.Value) + "");
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

    protected void uploadfile_Click(object sender, EventArgs e)
    {
        if (AttachmentUpload.HasFile)
        {
            string fileName = Path.GetFileName(AttachmentUpload.PostedFile.FileName);
            byte[] fileContent;

            using (Stream fs = AttachmentUpload.PostedFile.InputStream)
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fileContent = br.ReadBytes((int)fs.Length);
                }
            }
            ViewState["attachment"] = null;
            ViewState["attachment"] = fileContent;
            lblfile1.Text = fileName;
        }

    }

    //
    private void FillKittens()
    {
        SqlDataAdapter ad = new SqlDataAdapter("select CCD.ID,CCD.Name from tbl_CompanyMaster AS CM Inner JOIN tbl_CompanyContactDetails AS CCD ON CCD.CompanyCode=CM.CompanyCode where Companyname='" + txtcompanyname.Text.Trim() + "' ", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlContacts.DataSource = dt;
            ddlContacts.DataValueField = "ID";
            ddlContacts.DataTextField = "Name";
            ddlContacts.DataBind();
            ddlContacts.Items.Insert(0, "-- Select Kindd Att --");
        }
    }

    protected void ddlContacts_SelectedIndexChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select Name,EmailID,Number from tbl_CompanyMaster AS CM left join tbl_CompanyContactDetails AS CC on CM.CompanyCode=CC.CompanyCode  WHERE CC.ID='" + ddlContacts.SelectedValue + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {

            txtmobileno.Text = Dt.Rows[0]["Number"].ToString();
            txtemail.Text = Dt.Rows[0]["EmailID"].ToString();
        }
    }

    protected void ddlBillAddress_TextChanged(object sender, EventArgs e)
    {
        if (ddlBillAddress.SelectedItem.Text != "-Select Billing Address-")
        {
            SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM tbl_BillingAddress  AS SA where BillAddress='" + ddlBillAddress.SelectedItem.Text + "'", Cls_Main.Conn);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtgstno.Text = dt.Rows[0]["GSTNo"].ToString();
            }
        }
    }
}