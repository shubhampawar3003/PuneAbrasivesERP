using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_StoreInwardEntry : System.Web.UI.Page
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
                txtcreateddate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtchallandate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtinvoicedate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtmaterialrecivedby.Text = Session["Username"].ToString();
                ViewState["RowNo"] = 0;
                Dt_Product.Columns.AddRange(new DataColumn[5] { new DataColumn("id"), new DataColumn("Particular"), new DataColumn("ComponentName"), new DataColumn("Batch"), new DataColumn("Quantity") });
                ViewState["gvcomponent"] = Dt_Product;
                ddlponumber.Enabled = false;
                if (Request.QueryString["Id"] != null)
                {
                    ID = objcls.Decrypt(Request.QueryString["Id"].ToString());
                    hhd.Value = ID;
                    Load_Record(ID);
                    btnsave.Text = "Update";
                    divcdate.Visible = false;
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
                    dt.Columns.Add("TotalQty");
                    dt.Columns.Add("Qty");
                    dt.Columns.Add("InQty");
                    dt.Columns.Add("Rate");
                    dt.Columns.Add("Batchno");

                    //Added Empty Row
                    dt.Rows.Add(dt.NewRow());


                    // Bind to GridView
                    dgvTaxinvoiceDetails.DataSource = dt;
                    dgvTaxinvoiceDetails.DataBind();

                }

            }
        }
    }
    //Data Fetch
    private void Load_Record(string ID)
    {
        try
        {
            DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_PendingInwardHdr] WHERE ID ='" + ID + "' ");
            if (Dt.Rows.Count > 0)
            {
                btnsave.Text = "Update";
                ddlponumber.SelectedItem.Text = Dt.Rows[0]["pono"].ToString();
                if (!string.IsNullOrWhiteSpace(ddlponumber.SelectedItem.Text))
                {
                    ddlType.SelectedValue = "Order";
                }
                txtorderno.Text = Dt.Rows[0]["OrderNo"].ToString();
                hdnOrderNo.Value = Dt.Rows[0]["OrderNo"].ToString();
                //int id = Convert.ToInt32(Dt.Rows[0]["OrderNo"].ToString());
                // txtBatch.Text = Dt.Rows[0]["Batch"].ToString();
                txtinvoiceno.Text = Dt.Rows[0]["InvoiceNo"].ToString();
                DateTime ffff = Convert.ToDateTime(Dt.Rows[0]["InvoiceDate"].ToString());
                txtinvoicedate.Text = ffff.ToString("yyyy-MM-dd");
                DateTime ffff4 = Convert.ToDateTime(Dt.Rows[0]["CreatedOn"].ToString());
                txtcreateddate.Text = ffff4.ToString("yyyy-MM-dd");
                txtsupliername.Text = Dt.Rows[0]["Suppliername"].ToString();
                txtmobileno.Text = Dt.Rows[0]["MobileNo"].ToString();
                txtchallanno.Text = Dt.Rows[0]["ChallanNo"].ToString();
                DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["ChallanDate"].ToString());
                txtchallandate.Text = ffff1.ToString("yyyy-MM-dd");
                txttransportname.Text = Dt.Rows[0]["Transportname"].ToString();
                txtinwardtime.Text = Dt.Rows[0]["InwardTime"].ToString();
                txtmaterialrecivedby.Text = Dt.Rows[0]["MaterialRecivedBy"].ToString();
                txtvehicleno.Text = Dt.Rows[0]["VehicalNo"].ToString();
                txtmaterialdescription.Text = Dt.Rows[0]["MaterialDescription"].ToString();
                txtSuplieraddress.Text = Dt.Rows[0]["SuplierAddress"].ToString();
                txtPanno.Text = Dt.Rows[0]["PanNo"].ToString();
                txtGSTNO.Text = Dt.Rows[0]["GST"].ToString();
                txtState.Text = Dt.Rows[0]["State"].ToString();


                DataTable dtt1 = new DataTable();
                // SqlDataAdapter sad31 = new SqlDataAdapter(@"select componentname as Particulars,'N/A' AS TotalQty ,Quantity AS Qty,Quantity AS InQty,Batch AS Batchno, * from tbl_PendingInwardDtls  WHERE OrderNo = '" + hdnOrderNo.Value + "'", con);

                string query = @"
SELECT 
    PD.ID,
    PD.HSN,
    PD.IsSelected,
    PD.Description,
    PD.ComponentName AS Particulars,
    PD.Rate,
    Billed.TotalBilledQty AS TotalQty,
    PD.Quantity AS Qty,
    PD.Quantity AS InQty,
    PD.Batch AS Batchno
FROM tbl_PendingInwardHdr AS PH
LEFT JOIN tbl_PendingInwardDtls AS PD ON PD.OrderNo = PH.OrderNo
LEFT JOIN (
    SELECT 
        PBB.Particulars, 
        PBB.HSN, 
        SUM(CONVERT(INT, PBB.Qty)) AS TotalBilledQty
    FROM tblPurchaseOrderHdr AS PBH
    INNER JOIN tblPurchaseOrderDtls AS PBB ON PBB.HeaderID = PBH.ID
    WHERE PBH.Pono IN (SELECT Pono FROM tbl_PendingInwardHdr WHERE OrderNo = @OrderNo)
    GROUP BY PBB.Particulars, PBB.HSN
) AS Billed ON Billed.Particulars = PD.ComponentName AND Billed.HSN = PD.HSN
WHERE PH.OrderNo = @OrderNo";

                SqlDataAdapter sad31 = new SqlDataAdapter(query, con);
                sad31.SelectCommand.Parameters.AddWithValue("@OrderNo", hdnOrderNo.Value);
                sad31.Fill(dtt1);
                if (dtt1.Rows.Count > 0)
                {

                    dgvTaxinvoiceDetails.DataSource = dtt1;
                    dgvTaxinvoiceDetails.DataBind();
                }
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            int count = 0;
            foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
            {
                CheckBox Check = (grd1.FindControl("chkRow") as CheckBox);
                TextBox Batchno = (grd1.FindControl("txtBatchno") as TextBox);
                if (Check.Checked == true)
                {
                    if (string.IsNullOrWhiteSpace(Batchno.Text))
                    {
                        btnsave.Focus();
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('fill batch number in checked rows...!!');", true);
                        count = 0;
                        break;
                    }
                    count = 1;
                }

            }
            if (count == 1)
            {
                if (string.IsNullOrWhiteSpace(txtsupliername.Text))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!'); ", true);

                    return;
                }

                bool isSave = btnsave.Text == "Save";
                if (isSave)
                {
                    DataTable Dt = Cls_Main.Read_Table("select dbo.GenerateInwardNo() AS OrderNo");
                    if (Dt.Rows.Count > 0)
                    {
                        string OrderNo = Dt.Rows[0]["OrderNo"].ToString();
                        SaveHeader("Save", OrderNo);
                        SaveComponents(OrderNo);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Inward Entry Saved Successfully..!!');window.location='StoreInwardList.aspx';", true);

                    }
                }
                else
                {
                    string OrderNo = hdnOrderNo.Value;
                    SaveHeader("Update", OrderNo);
                    DeleteExistingComponents(OrderNo);
                    SaveComponents(OrderNo);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Inward Entry Updated Successfully..!!');window.location='StoreInwardList.aspx';", true);

                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please check atleast one checkbox..!!')", true);
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please try again..!!')", true);
        }
    }
    private void SaveHeader(string action, string OrderNo)
    {
        Cls_Main.Conn_Open();
        SqlCommand cmd = new SqlCommand("SP_InventoryDetails", Cls_Main.Conn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@EntryType", DBNull.Value);
        cmd.Parameters.AddWithValue("@OrderNo", OrderNo);
        cmd.Parameters.AddWithValue("@InvoiceNo", txtinvoiceno.Text.ToUpper());
        cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoicedate.Text);
        cmd.Parameters.AddWithValue("@Suppliername", txtsupliername.Text.ToUpper());
        cmd.Parameters.AddWithValue("@Address", txtSuplieraddress.Text.ToUpper());
        cmd.Parameters.AddWithValue("@Panno", txtPanno.Text.ToUpper());
        cmd.Parameters.AddWithValue("@GST", txtGSTNO.Text.ToUpper());
        cmd.Parameters.AddWithValue("@State", txtState.Text.ToUpper());
        cmd.Parameters.AddWithValue("@MobileNo", txtmobileno.Text.ToUpper());
        cmd.Parameters.AddWithValue("@ChallanNo", txtchallanno.Text.ToUpper());
        cmd.Parameters.AddWithValue("@ChallanDate", txtchallandate.Text);
        cmd.Parameters.AddWithValue("@Transportname", txttransportname.Text.ToUpper());
        cmd.Parameters.AddWithValue("@InwardTime", txtinwardtime.Text);
        cmd.Parameters.AddWithValue("@MaterialRecivedBy", txtmaterialrecivedby.Text.ToUpper());
        cmd.Parameters.AddWithValue("@VehicalNo", txtvehicleno.Text.ToUpper());
        cmd.Parameters.AddWithValue("@MaterialDescription", txtmaterialdescription.Text.ToUpper());
        cmd.Parameters.AddWithValue("@IsDeleted", '0');
        cmd.Parameters.AddWithValue("@CreatedOn", txtcreateddate.Text);

        if (action == "Save")
        {
            cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
        }
        else
        {
            cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
            cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
        }

        cmd.Parameters.AddWithValue("@pono", ddlponumber.SelectedItem.Text);
        cmd.Parameters.AddWithValue("@Action", action);
        cmd.ExecuteNonQuery();
        Cls_Main.Conn_Close();
        Cls_Main.Conn_Dispose();
    }
    private void SaveComponents(string OrderNo)
    {
        foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
        {

            string txtProduct = (grd1.FindControl("txtProduct") as TextBox).Text;
            string txtDescription = (grd1.FindControl("txtDescription") as TextBox).Text;
            string txtHSN = (grd1.FindControl("txtHSN") as TextBox).Text;
            string txtInQuantity = (grd1.FindControl("txtInQuantity") as TextBox).Text;
            string lblrate = (grd1.FindControl("lblrate") as Label).Text;
            string txtBatchno = (grd1.FindControl("txtBatchno") as TextBox).Text;
            bool chkRow = (grd1.FindControl("chkRow") as CheckBox).Checked;

            string lblDescription = "";
            string lblhsn = "";
            string lblUnit = "";
            string lblRate = lblrate;
            string lblTotal = "";
            string lblCGSTPer = "";
            string lblCGST = "";
            string lblSGSTPer = "";
            string lblSGST = "";
            string lblIGSTPer = "";
            string lblIGST = "";
            string lblDiscount = "";
            string lblDiscountAmount = "";
            string lblAlltotal = "";


            Cls_Main.Conn_Open();
            SqlCommand cmdd1 = new SqlCommand("INSERT INTO [tbl_PendingInwardDtls] (OrderNo,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,CreatedBy,Batch,IsSelected) VALUES (@OrderNo,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@CreatedBy,@Batch,@chkRow)", Cls_Main.Conn);
            cmdd1.Parameters.AddWithValue("@OrderNo", OrderNo);
            cmdd1.Parameters.AddWithValue("@Particular", txtProduct);
            cmdd1.Parameters.AddWithValue("@ComponentName", txtProduct);
            cmdd1.Parameters.AddWithValue("@Description", txtDescription);
            cmdd1.Parameters.AddWithValue("@HSN", txtHSN);
            cmdd1.Parameters.AddWithValue("@Quantity", txtInQuantity);
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
            cmdd1.Parameters.AddWithValue("@Batch", txtBatchno);
            cmdd1.Parameters.AddWithValue("@chkRow", chkRow);
            cmdd1.ExecuteNonQuery();
            Cls_Main.Conn_Close();

        }


    }
    private void DeleteExistingComponents(string orderNo)
    {
        Cls_Main.Conn_Open();
        SqlCommand cmd = new SqlCommand("DELETE FROM tbl_PendingInwardDtls WHERE OrderNo = @OrderNo", Cls_Main.Conn);
        cmd.Parameters.AddWithValue("@OrderNo", orderNo);
        cmd.ExecuteNonQuery();
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

                if (ddlType.SelectedValue == "Order")
                {
                    txtProduct.Enabled = false;
                    txtDescription.Enabled = false;
                    txtHSN.Enabled = false;
                    txtQuantity.Enabled = false;
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
        Response.Redirect("StoreInwardList.aspx");
    }
    protected void txtsupliername_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("select * from tbl_VendorMaster WHERE Vendorname='" + txtsupliername.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {
            txtState.Text = Dt.Rows[0]["State"].ToString();
            txtPanno.Text = Dt.Rows[0]["PANNo"].ToString();
            txtGSTNO.Text = Dt.Rows[0]["GSTNo"].ToString();
            txtSuplieraddress.Text = Dt.Rows[0]["Address"].ToString();
            txtmobileno.Text = Dt.Rows[0]["MobileNo"].ToString();
        }
    }
    public void GetPurchaseOrderNo()
    {
        //SqlDataAdapter ad = new SqlDataAdapter("select * from tblPurchaseOrderHdr where Suppliername='" + txtsupliername.Text + "'", Cls_Main.Conn);

        SqlDataAdapter ad = new SqlDataAdapter("WITH ABC AS(select ComponentName,SUM(CAST(quantity AS int)) AS QTY,Pono,'' AS PendingQty" +
                        " from tbl_PendingInwardHdr AS PH" +
                        " INNER JOIN tbl_PendingInwardDtls AS PD ON PH.OrderNo = PD.OrderNo" +
                        " WHERE PH.Suppliername = '" + txtsupliername.Text + "'" +
                        " GROUP BY ComponentName, Pono" +
                        " UNION" +
                        " select Product, '' AS QTY, Pono, SUM(CAST(Qty AS int)) AS PendingQty" +
                        " from tblPurchaseOrderHdr AS PH" +
                        " INNER JOIN tblPurchaseOrderDtls AS PD ON PH.Id = PD.HeaderID" +
                        " WHERE PH.Suppliername = '" + txtsupliername.Text + "'" +
                        " GROUP BY Product, Pono)" +
                        " SELECT Distinct Pono" +
                       " FROM ABC " +
                       " GROUP BY ComponentName, Pono " +
                       " HAVING(SUM(CAST(PendingQty AS int)) - SUM(CAST(Qty AS int))) > 0", Cls_Main.Conn);

        //SqlDataAdapter ad = new SqlDataAdapter("With NewTable as " +
        //    " (SELECT  PH.Pono,PD.Qty AS TotalQty, (CONVERT(INT, PD.Qty) - ISNULL(Billed.TotalBilledQty, 0)) AS Qty " +
        //    " FROM tblPurchaseOrderHdr AS PH " +
        //    " LEFT JOIN tblPurchaseOrderDtls AS PD ON PD.HeaderID = PH.Id " +
        //    " LEFT JOIN ( SELECT PBB.ComponentName,PBB.HSN, " +
        //    " SUM(CONVERT(INT, PBB.Quantity)) AS TotalBilledQty " +
        //    " FROM tbl_PendingInwardHdr AS PBH  " +
        //    " INNER JOIN tbl_PendingInwardDtls AS PBB ON PBB.OrderNo = PBH.OrderNo " +
        //    " WHERE PBH.SupplierName = '" + txtsupliername.Text + "' " +
        //    " GROUP BY PBB.ComponentName, PBB.HSN) AS Billed ON Billed.ComponentName = PD.Particulars " +
        //    " AND Billed.HSN = PD.HSN  WHERE PH.Suppliername='" + txtsupliername.Text + "') " +
        //    " select Distinct Pono from NewTable ", Cls_Main.Conn); //where Qty >'0'

        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlponumber.DataSource = dt;
            ddlponumber.DataTextField = "PoNo";
            ddlponumber.DataBind();
            ddlponumber.Items.Insert(0, "--- select ---");
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('There is no pending products..!!');", true);
        }

    }
    protected void ddlponumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlponumber.SelectedValue != "0")
        {
            DataTable dtt1 = new DataTable();
            //            SqlDataAdapter sad31 = new SqlDataAdapter(@"select *,Qty AS InQty from tblPurchaseOrderHdr AS PH
            //LEFT JOIN tblPurchaseOrderDtls AS PD ON PD.HeaderID = PH.Id
            //WHERE PONo = '" + ddlponumber.SelectedItem.Text + "'", con); 

            string query = @"
WITH Record AS (
    SELECT 
        PD.Qty AS TotalQty,
        PH.IsSelected,
        PD.ID AS id,
        PD.Particulars,
        PD.Batchno,
        PD.Rate,
        PD.Description,
        PD.HSN,
        (CONVERT(INT, PD.Qty) - ISNULL(Billed.TotalBilledQty, 0)) AS Qty,
        (CONVERT(INT, PD.Qty) - ISNULL(Billed.TotalBilledQty, 0)) AS InQty
    FROM tblPurchaseOrderHdr AS PH
    LEFT JOIN tblPurchaseOrderDtls AS PD ON PD.HeaderID = PH.Id
    LEFT JOIN (
        SELECT 
            PBB.ComponentName,
            PBB.HSN,
            SUM(CONVERT(INT, PBB.Quantity)) AS TotalBilledQty
        FROM tbl_PendingInwardHdr AS PBH
        INNER JOIN tbl_PendingInwardDtls AS PBB ON PBB.OrderNo = PBH.OrderNo
        WHERE PBH.PONo = @PONo
        GROUP BY PBB.ComponentName, PBB.HSN
    ) AS Billed ON Billed.ComponentName = PD.Particulars AND Billed.HSN = PD.HSN
    WHERE PH.PONo = @PONo
)
SELECT 
    R.TotalQty,
    R.IsSelected,
    R.ID AS id,
    R.Particulars,
    R.Rate,
    R.Batchno,
    CM.Grade AS Description,
    R.HSN,
    R.Qty,
    R.InQty,
    CM.ComponentName
FROM Record AS R
INNER JOIN tbl_ComponentMaster AS CM ON CM.ComponentName = R.Particulars
WHERE R.Qty <> 0;";

            SqlDataAdapter sad31 = new SqlDataAdapter(query, con);
            sad31.SelectCommand.Parameters.AddWithValue("@PONo", ddlponumber.SelectedItem.Text);

            sad31.Fill(dtt1);
            if (dtt1.Rows.Count > 0)
            {
                if (dtt1.Rows.Count == 0)
                {
                    dtt1.Rows.Add(dtt1.NewRow()); // Add a blank row
                }
                dgvTaxinvoiceDetails.DataSource = dtt1;
                dgvTaxinvoiceDetails.DataBind();
            }

        }
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
    protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlType.SelectedValue == "Order")
        {
            btnAdd.Visible = false;
            GetPurchaseOrderNo();
            ddlponumber.Enabled = true;
        }
        else
        {
            ddlponumber.Enabled = false;
            ddlponumber.Items.Insert(0, "--- select ---");
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
        dt.Columns.Add("TotalQty");
        dt.Columns.Add("Qty");
        dt.Columns.Add("InQty");
        dt.Columns.Add("Rate");
        dt.Columns.Add("Batchno");

        if (dgvTaxinvoiceDetails.Rows.Count > 0)
        {
            foreach (GridViewRow grd1 in dgvTaxinvoiceDetails.Rows)
            {

                TextBox Particulars = (grd1.FindControl("txtProduct") as TextBox);
                string Description = (grd1.FindControl("txtDescription") as TextBox).Text;
                string HSN = (grd1.FindControl("txtHSN") as TextBox).Text;
                string TotalQty = (grd1.FindControl("txtQuantity") as TextBox).Text;
                string Qty = (grd1.FindControl("txtQuantity") as TextBox).Text;
                string InQty = (grd1.FindControl("txtInQuantity") as TextBox).Text;
                string Rate = (grd1.FindControl("lblrate") as Label).Text;
                string Batchno = (grd1.FindControl("txtBatchno") as TextBox).Text;

                dt.Rows.Add(false, "", Particulars.Text, Description, HSN, TotalQty, Qty, InQty, Rate, Batchno);
            }

        }

        dt.Rows.Add(false, "", "", "", "", "", "", "", "", "");

        dgvTaxinvoiceDetails.DataSource = dt;
        dgvTaxinvoiceDetails.DataBind();
    }
    protected void ddlcomponent_SelectedIndexChanged(object sender, EventArgs e)
    {
        ViewState["Compo"] = null;
        DropDownList ddl = (DropDownList)sender;
        GridViewRow row = (GridViewRow)ddl.NamingContainer;
        int rowIndex = row.RowIndex;
        string selectedValue = ddl.SelectedValue;
        ViewState["SelectedRowIndex"] = rowIndex;
        ViewState["SelectedComponent"] = selectedValue;
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


