
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Admin_ReturnProductsEntry : System.Web.UI.Page
{

    DataTable Dt_Compo = new DataTable();
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
                // BatchNo();
                InwardNo(); FillddlComponent();
                Fillddltransport();
                txtinvoicedate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtchallandate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtmaterialrecivedby.Text = Session["Username"].ToString();

                ViewState["RowNo"] = 0;

                Dt_Compo.Columns.AddRange(new DataColumn[18] { new DataColumn("id"), new DataColumn("ComponentName"), new DataColumn("Batch"), new DataColumn("Description"), new DataColumn("HSN"), new DataColumn("Quantity"), new DataColumn("Units"), new DataColumn("Rate"), new DataColumn("Total"), new DataColumn("CGSTPer"), new DataColumn("CGSTAmt"), new DataColumn("SGSTPer"), new DataColumn("SGSTAmt"), new DataColumn("IGSTPer"), new DataColumn("IGSTAmt"), new DataColumn("Discountpercentage"), new DataColumn("DiscountAmount"), new DataColumn("Alltotal") });
                ViewState["Components"] = Dt_Compo;
                if (Request.QueryString["ID"] != null)
                {
                    string Id = objcls.Decrypt(Request.QueryString["ID"].ToString());
                    hhd.Value = Id;
                    Load_Record(Id);
                    btnsave.Text = "Update";
                    ShowCompDtlEdit();
                }
                

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
                com.CommandText = "Select DISTINCT [Vendorname] from [tbl_VendorMaster] where " + "Vendorname like @Search + '%' and IsDeleted=0";

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
    //Data Fetch
    private void Load_Record(string ID)
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_InwardEntryHdr] WHERE ID ='" + ID + "' ");
        if (Dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            // ddlentrytype.SelectedItem.Text = Dt.Rows[0]["EntryType"].ToString();
            txtorderno.Text = Dt.Rows[0]["OrderNo"].ToString();
            txtBatch.Text = Dt.Rows[0]["Batch"].ToString();
            txtinvoiceno.Text = Dt.Rows[0]["InvoiceNo"].ToString();
            DateTime ffff = Convert.ToDateTime(Dt.Rows[0]["InvoiceDate"].ToString());
            txtinvoicedate.Text = ffff.ToString("yyyy-MM-dd");
            txtsupliername.Text = Dt.Rows[0]["Suppliername"].ToString();
            txtmobileno.Text = Dt.Rows[0]["MobileNo"].ToString();
            txtchallanno.Text = Dt.Rows[0]["ChallanNo"].ToString();
            DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["ChallanDate"].ToString());
            txtchallandate.Text = ffff1.ToString("yyyy-MM-dd");
            ddltransportname.SelectedItem.Text = Dt.Rows[0]["Transportname"].ToString();
            txtinwardtime.Text = Dt.Rows[0]["InwardTime"].ToString();
            txtmaterialrecivedby.Text = Dt.Rows[0]["MaterialRecivedBy"].ToString();
            txtvehicleno.Text = Dt.Rows[0]["VehicalNo"].ToString();
            txtmaterialdescription.Text = Dt.Rows[0]["MaterialDescription"].ToString();
            txtSuplieraddress.Text = Dt.Rows[0]["SuplierAddress"].ToString();
            txtPanno.Text = Dt.Rows[0]["PanNo"].ToString();
            txtGSTNO.Text = Dt.Rows[0]["GST"].ToString();
            txtState.Text = Dt.Rows[0]["State"].ToString();
        }
    }
    //Batch Number
    //protected void BatchNo()
    //{
    //    SqlDataAdapter ad = new SqlDataAdapter("SELECT max([ID]) as maxid FROM [tbl_InwardEntryHdr]", Cls_Main.Conn);
    //    DataTable dt = new DataTable();
    //    ad.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        int currentYear = DateTime.Now.Year;
    //        int nextYear = currentYear + 1;
    //        int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
    //        // txtInvoiceno.Text = "WLSPL/TI-" + (maxid + 1).ToString();
    //        txtBatch.Text =  currentYear.ToString() + "-" + nextYear.ToString().Substring(2) + "/" + "B-"+(maxid + 1).ToString("D4");
    //    }
    //    else
    //    {
    //        txtBatch.Text = string.Empty;
    //    }
    //}

    //Inward Code Auto
    protected void InwardNo()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([ID]) as maxid FROM [tbl_InwardEntryHdr]", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            txtorderno.Text = "PAPL/CN-" + (maxid + 1).ToString();
        }
        else
        {
            txtorderno.Text = string.Empty;
        }
    }


    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtsupliername.Text == "" || txtorderno.Text == "" || txtinvoiceno.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Kindly Enter the Data..!!')", true);
            }
            else
            {
                if (btnsave.Text == "Save")
                {
                    Cls_Main.Conn_Open();
                    SqlCommand cmd = new SqlCommand("SP_InwardEntryHdr", Cls_Main.Conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EntryType", DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderNo", txtorderno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@InvoiceNo", txtinvoiceno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Batch", txtBatch.Text);
                    cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoicedate.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Suppliername", txtsupliername.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Address", txtSuplieraddress.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Panno", txtPanno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@GST", txtGSTNO.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@State", txtState.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@MobileNo", txtmobileno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@ChallanNo", txtchallanno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@ChallanDate", txtchallandate.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Transportname", ddltransportname.SelectedItem.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@InwardTime", txtinwardtime.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@MaterialRecivedBy", txtmaterialrecivedby.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@VehicalNo", txtvehicleno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@MaterialDescription", txtmaterialdescription.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@CGST_Amt", txtCOMP_cgstamt.Text);
                    cmd.Parameters.AddWithValue("@SGST_Amt", txtCOMP_sgstamt.Text);
                    cmd.Parameters.AddWithValue("@IGST_Amt", txtCOMP_igstamt.Text);
                    cmd.Parameters.AddWithValue("@Total_Price", txtCOMP_grandTotal.Text);
                    cmd.Parameters.AddWithValue("@Totalinword", lblCOMP_total_amt_Value.Text);
                    cmd.Parameters.AddWithValue("@IsDeleted", '0');
                    cmd.Parameters.AddWithValue("@CreatedBy", Session["UserCode"].ToString());
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Action", "Save");
                    cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();


                    //Save Component Details 
                    if (gvcomponent.Rows.Count > 0)
                    {
                        foreach (GridViewRow grd1 in gvcomponent.Rows)
                        {
                            string lblCompo = (grd1.FindControl("lblComComPonent") as Label).Text;
                            string lblDescription = (grd1.FindControl("lblComDescription") as Label).Text;
                            string lblhsn = (grd1.FindControl("lblComhsn") as Label).Text;
                            string lblQuantity = (grd1.FindControl("lblComQuantity") as Label).Text;
                            string lblUnit = (grd1.FindControl("lblComUnit") as Label).Text;
                            string lblRate = (grd1.FindControl("lblComRate") as Label).Text;
                            string lblTotal = (grd1.FindControl("lblComTotal") as Label).Text;
                            string lblCGSTPer = (grd1.FindControl("lblComCGSTPer") as Label).Text;
                            string lblCGST = (grd1.FindControl("lblComCGST") as Label).Text;
                            string lblSGSTPer = (grd1.FindControl("lblComSGSTPer") as Label).Text;
                            string lblSGST = (grd1.FindControl("lblComSGST") as Label).Text;
                            string lblIGSTPer = (grd1.FindControl("lblComIGSTPer") as Label).Text;
                            string lblIGST = (grd1.FindControl("lblComIGST") as Label).Text;
                            string lblDiscount = (grd1.FindControl("lblComDiscount") as Label).Text;
                            string lblDiscountAmount = (grd1.FindControl("lblComDiscountAmount") as Label).Text;
                            string lblAlltotal = (grd1.FindControl("lblComAlltotal") as Label).Text;
                            string lblComBatch = (grd1.FindControl("lblComBatch") as Label).Text;

                            Cls_Main.Conn_Open();
                            SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_InwardComponentsdtls] (OrderNo,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@OrderNo", txtorderno.Text);
                            cmdd.Parameters.AddWithValue("@ComponentName", lblCompo);
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
                            cmdd.Parameters.AddWithValue("@Batch", lblComBatch);
                            cmdd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Inward Entry Save Successfully..!!');window.location='InwardEntryList.aspx'; ", true);
                }
                else if (btnsave.Text == "Update")
                {
                    // DateTime Date = DateTime.Now;
                    Cls_Main.Conn_Open();
                    SqlCommand cmd = new SqlCommand("SP_InwardEntryHdr", Cls_Main.Conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EntryType", DBNull.Value);
                    cmd.Parameters.AddWithValue("@OrderNo", txtorderno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@InvoiceNo", txtinvoiceno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Batch", txtBatch.Text);
                    cmd.Parameters.AddWithValue("@InvoiceDate", txtinvoicedate.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Suppliername", txtsupliername.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Address", txtSuplieraddress.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Panno", txtPanno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@GST", txtGSTNO.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@State", txtState.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@MobileNo", txtmobileno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@ChallanNo", txtchallanno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@ChallanDate", txtchallandate.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@Transportname", ddltransportname.SelectedItem.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@InwardTime", txtinwardtime.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@MaterialRecivedBy", txtmaterialrecivedby.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@VehicalNo", txtvehicleno.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@MaterialDescription", txtmaterialdescription.Text.ToUpper());
                    cmd.Parameters.AddWithValue("@CGST_Amt", txtCOMP_cgstamt.Text);
                    cmd.Parameters.AddWithValue("@SGST_Amt", txtCOMP_sgstamt.Text);
                    cmd.Parameters.AddWithValue("@IGST_Amt", txtCOMP_igstamt.Text);
                    cmd.Parameters.AddWithValue("@Total_Price", txtCOMP_grandTotal.Text);
                    cmd.Parameters.AddWithValue("@Totalinword", lblCOMP_total_amt_Value.Text);
                    cmd.Parameters.AddWithValue("@IsDeleted", '0');
                    cmd.Parameters.AddWithValue("@UpdatedBy", Session["UserCode"].ToString());
                    cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Action", "Update");
                    cmd.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();

                    Cls_Main.Conn_Open();
                    SqlCommand cmddelete1 = new SqlCommand("DELETE FROM tbl_InwardComponentsdtls WHERE OrderNo=@OrderNo", Cls_Main.Conn);
                    cmddelete1.Parameters.AddWithValue("@OrderNo", txtorderno.Text);
                    cmddelete1.ExecuteNonQuery();
                    Cls_Main.Conn_Close();
                    Cls_Main.Conn_Dispose();
                    //Save Component Details 
                    if (gvcomponent.Rows.Count > 0)
                    {
                        foreach (GridViewRow grd1 in gvcomponent.Rows)
                        {
                            string lblCompo = (grd1.FindControl("lblComComPonent") as Label).Text;
                            string lblDescription = (grd1.FindControl("lblComDescription") as Label).Text;
                            string lblhsn = (grd1.FindControl("lblComhsn") as Label).Text;
                            string lblQuantity = (grd1.FindControl("lblComQuantity") as Label).Text;
                            string lblUnit = (grd1.FindControl("lblComUnit") as Label).Text;
                            string lblRate = (grd1.FindControl("lblComRate") as Label).Text;
                            string lblTotal = (grd1.FindControl("lblComTotal") as Label).Text;
                            string lblCGSTPer = (grd1.FindControl("lblComCGSTPer") as Label).Text;
                            string lblCGST = (grd1.FindControl("lblComCGST") as Label).Text;
                            string lblSGSTPer = (grd1.FindControl("lblComSGSTPer") as Label).Text;
                            string lblSGST = (grd1.FindControl("lblComSGST") as Label).Text;
                            string lblIGSTPer = (grd1.FindControl("lblComIGSTPer") as Label).Text;
                            string lblIGST = (grd1.FindControl("lblComIGST") as Label).Text;
                            string lblDiscount = (grd1.FindControl("lblComDiscount") as Label).Text;
                            string lblDiscountAmount = (grd1.FindControl("lblComDiscountAmount") as Label).Text;
                            string lblAlltotal = (grd1.FindControl("lblComAlltotal") as Label).Text;
                            string lblComBatch = (grd1.FindControl("lblComBatch") as Label).Text;

                            Cls_Main.Conn_Open();
                            SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_InwardComponentsdtls] (OrderNo,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
                            cmdd.Parameters.AddWithValue("@OrderNo", txtorderno.Text);
                            cmdd.Parameters.AddWithValue("@ComponentName", lblCompo);
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
                            cmdd.Parameters.AddWithValue("@Batch", lblComBatch);
                            cmdd.ExecuteNonQuery();
                            Cls_Main.Conn_Close();
                        }
                    }
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Inward Entry Update Successfully..!!');window.location='InwardEntryList.aspx'; ", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
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
    //Component section 
    private void FillddlComponent()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT[ComponentName] FROM [tbl_ComponentMaster]  where IsDeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlcomponent.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            ddlcomponent.DataTextField = "ComponentName";
            ddlcomponent.DataBind();
            ddlcomponent.Items.Insert(0, "-- Select Component Name --");
        }
    }

    protected void ShowCompDtlEdit()
    {
        divCOMPTotalPart.Visible = true;

        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_InwardComponentsdtls] WHERE OrderNo='" + txtorderno.Text + "'", Cls_Main.Conn);
        DataTable DTCOMP = new DataTable();
        Da.Fill(DTCOMP);

        int count = 0;

        if (DTCOMP.Rows.Count > 0)
        {
            if (Dt_Compo.Columns.Count < 0)
            {
                ShowCom_Grid();
            }

            for (int i = 0; i < DTCOMP.Rows.Count; i++)
            {
                Dt_Compo.Rows.Add(count, DTCOMP.Rows[i]["ComponentName"].ToString(), DTCOMP.Rows[i]["Batch"].ToString(), DTCOMP.Rows[i]["Description"].ToString(), DTCOMP.Rows[i]["HSN"].ToString(), DTCOMP.Rows[i]["Quantity"].ToString(), DTCOMP.Rows[i]["Units"].ToString(), DTCOMP.Rows[i]["Rate"].ToString(), DTCOMP.Rows[i]["Total"].ToString(), DTCOMP.Rows[i]["CGSTPer"].ToString(), DTCOMP.Rows[i]["CGSTAmt"].ToString(), DTCOMP.Rows[i]["SGSTPer"].ToString(), DTCOMP.Rows[i]["SGSTAmt"].ToString(), DTCOMP.Rows[i]["IGSTPer"].ToString(), DTCOMP.Rows[i]["IGSTAmt"].ToString(), DTCOMP.Rows[i]["Discountpercentage"].ToString(), DTCOMP.Rows[i]["DiscountAmount"].ToString(), DTCOMP.Rows[i]["Alltotal"].ToString());
                count = count + 1;
            }
        }

        gvcomponent.EmptyDataText = "No Data Found";
        gvcomponent.DataSource = Dt_Compo;
        gvcomponent.DataBind();

    }

    protected void gvcomponent_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvcomponent.EditIndex = e.NewEditIndex;
        gvcomponent.DataSource = (DataTable)ViewState["Components"];
        gvcomponent.DataBind();
        //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    string lblCTotal, lblCCGST, lblCSGST;
    string lblCproduct, lblCproducttype, lblCsubtype, lblCproductbrand, lblCton, CDescription, Chsn, CQuantity, CUnit, CRate, CsubTotal, CCGSTPer, CCGST, CSGSTPer, CSGST, CIGSTPer, CIGST, CDiscount, lblCDiscountAmount, CGrandtotal;

    private decimal CTotal, CCGSTAmt, CSGSTAmt, CIGSTAmt, CAlltotal;
    protected void gvcomponent_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                LinkButton lnkedit = e.Row.FindControl("btn_COMPedit") as LinkButton;
                if (lnkedit == null)
                {

                    lblCproduct = (e.Row.FindControl("txtCOMPComponent") as TextBox).Text;
                    CDescription = (e.Row.FindControl("txtCOMPDescription") as TextBox).Text;
                    Chsn = (e.Row.FindControl("txtCOMPhsn") as TextBox).Text;
                    CQuantity = (e.Row.FindControl("txtCOMPQuantity") as TextBox).Text;
                    CUnit = (e.Row.FindControl("txtCOMPUnit") as TextBox).Text;
                    CRate = (e.Row.FindControl("txtCOMPRate") as TextBox).Text;
                    CsubTotal = (e.Row.FindControl("txtCOMPTotal") as TextBox).Text;
                    CCGSTPer = (e.Row.FindControl("txtCOMPCGSTPer") as TextBox).Text;
                    CCGST = (e.Row.FindControl("txtCOMPCGST") as TextBox).Text;
                    CSGSTPer = (e.Row.FindControl("txtCOMPSGSTPer") as TextBox).Text;
                    CSGST = (e.Row.FindControl("txtCOMPSGST") as TextBox).Text;
                    CIGSTPer = (e.Row.FindControl("txtCOMPIGSTPer") as TextBox).Text;
                    CIGST = (e.Row.FindControl("txtCOMPIGST") as TextBox).Text;
                    CDiscount = (e.Row.FindControl("txtCOMPDiscount") as TextBox).Text;
                    lblCDiscountAmount = (e.Row.FindControl("txtCOMPDiscountAmount") as TextBox).Text;
                    CGrandtotal = (e.Row.FindControl("txtCOMPAlltotal") as TextBox).Text;

                }
                else
                {
                    CTotal += Convert.ToDecimal((e.Row.FindControl("lblComTotal") as Label).Text);
                    txtCOMP_Subtotal.Text = CTotal.ToString("0.00");

                    CCGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblComCGST") as Label).Text);
                    txtCOMP_cgstamt.Text = CCGSTAmt.ToString("0.00");

                    CSGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblComSGST") as Label).Text);
                    txtCOMP_sgstamt.Text = CSGSTAmt.ToString("0.00");

                    CIGSTAmt += Convert.ToDecimal((e.Row.FindControl("lblComIGST") as Label).Text);
                    txtCOMP_igstamt.Text = CIGSTAmt.ToString("0.00");

                    CAlltotal += Convert.ToDecimal((e.Row.FindControl("lblComAlltotal") as Label).Text);
                    txtCOMP_grandTotal.Text = CAlltotal.ToString("0.00");

                    //Amount Convert into word
                    //string number = txt_grandTotal.Text;
                    //number = Convert.ToDouble(number).ToString();
                    //string Amtinword = ConvertNumbertoWords(Convert.ToInt32(number));
                    //lbl_total_amt_Value.Text = Amtinword;


                    string isNegative = "";
                    try
                    {
                        string number = txtCOMP_grandTotal.Text;

                        number = Convert.ToDouble(number).ToString();

                        if (number.Contains("-"))
                        {
                            isNegative = "Minus ";
                            number = number.Substring(1, number.Length - 1);
                        }
                        else
                        {
                            lblCOMP_total_amt_Value.Text = isNegative + ConvertToWords(number);
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

    protected void lnkbtnCompDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        DataTable dt = ViewState["Components"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["Components"] = dt;
        gvcomponent.DataSource = (DataTable)ViewState["Components"];
        gvcomponent.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Component Delete Succesfully !!!');", true);
        //   ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void gv_Compupdate_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Product = ((TextBox)row.FindControl("txtCOMPComponent")).Text;
        string Batch = ((TextBox)row.FindControl("txtCOMPBatch")).Text;
        string Description = ((TextBox)row.FindControl("txtCOMPDescription")).Text;
        string HSN = ((TextBox)row.FindControl("txtCOMPhsn")).Text;
        string Quantity = ((TextBox)row.FindControl("txtCOMPQuantity")).Text;
        string Unit = ((TextBox)row.FindControl("txtCOMPUnit")).Text;
        string Rate = ((TextBox)row.FindControl("txtCOMPRate")).Text;
        string Total = ((TextBox)row.FindControl("txtCOMPTotal")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCOMPCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCOMPCGST")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtCOMPSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtCOMPSGST")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtCOMPIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtCOMPIGST")).Text;
        string Discount = ((TextBox)row.FindControl("txtCOMPDiscount")).Text;
        string DiscountAmt = ((TextBox)row.FindControl("txtCOMPDiscountAmount")).Text;
        string AllTotal = ((TextBox)row.FindControl("txtCOMPAlltotal")).Text;
        DataTable Dt = ViewState["Components"] as DataTable;
        Dt.Rows[row.RowIndex]["ComponentName"] = Product;
        Dt.Rows[row.RowIndex]["Batch"] = Batch;
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
        ViewState["Components"] = Dt;
        gvcomponent.EditIndex = -1;
        gvcomponent.DataSource = (DataTable)ViewState["Components"];
        gvcomponent.DataBind();
        //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void gv_Compcancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;
        string Product = ((TextBox)row.FindControl("txtCOMPComponent")).Text;
        string Batch = ((TextBox)row.FindControl("txtCOMPBatch")).Text;
        string Description = ((TextBox)row.FindControl("txtCOMPDescription")).Text;
        string HSN = ((TextBox)row.FindControl("txtCOMPhsn")).Text;
        string Quantity = ((TextBox)row.FindControl("txtCOMPQuantity")).Text;
        string Unit = ((TextBox)row.FindControl("txtCOMPUnit")).Text;
        string Rate = ((TextBox)row.FindControl("txtCOMPRate")).Text;
        string Total = ((TextBox)row.FindControl("txtCOMPTotal")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCOMPCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCOMPCGST")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtCOMPSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtCOMPSGST")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtCOMPIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtCOMPIGST")).Text;
        string Discount = ((TextBox)row.FindControl("txtCOMPDiscount")).Text;
        string DiscountAmt = ((TextBox)row.FindControl("txtCOMPDiscountAmount")).Text;
        string AllTotal = ((TextBox)row.FindControl("txtCOMPAlltotal")).Text;
        DataTable Dt = ViewState["Components"] as DataTable;
        Dt.Rows[row.RowIndex]["ComponentName"] = Product;
        Dt.Rows[row.RowIndex]["Batch"] = Batch;
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
        ViewState["Components"] = Dt;
        gvcomponent.EditIndex = -1;
        gvcomponent.DataSource = (DataTable)ViewState["Components"];
        gvcomponent.DataBind();
    }


    protected void ddlcomponent_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ComponentMaster] WHERE ComponentName='" + ddlcomponent.SelectedItem.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {

            txtComHSN.Text = Dt.Rows[0]["HSN"].ToString();
            txtComRate.Text = Dt.Rows[0]["Price"].ToString();
            txtComUnit.Text = Dt.Rows[0]["Unit"].ToString();
            string gstNumber = txtGSTNO.Text.Trim();
            string tax = gstNumber.Substring(0, 2);
            if (tax == "27")
            {
                txtComCGST.Text = "9";
                txtComSGST.Text = "9";
            }
            else
            {
                txtComIGST.Text = "18";
            }
            txtComCGSTAMT.Text = "0.00";
            txtComSGSTAMT.Text = "0.00";
            txtComIGSTAMT.Text = "0.00";
            txtComDiscount.Text = "0.00";
            txtComDiscountAMT.Text = "0.00";
            txtComQuantity.Focus();
        }
    }

    protected void txtComQuantity_TextChanged(object sender, EventArgs e)
    {
        try
        {
            var TotalAmt = Convert.ToDecimal(txtComQuantity.Text.Trim()) * Convert.ToDecimal(txtComRate.Text.Trim());
            txtComTotal.Text = TotalAmt.ToString();


            decimal tax_amt;
            decimal cgst_amt;
            decimal sgst_amt;
            decimal igst_amt;

            if (string.IsNullOrEmpty(txtComCGST.Text))
            {
                cgst_amt = 0;
            }
            else
            {
                cgst_amt = Convert.ToDecimal(TotalAmt.ToString()) * Convert.ToDecimal(txtComCGST.Text) / 100;
            }
            txtComCGSTAMT.Text = string.Format("{0:0.00}", cgst_amt);

            if (string.IsNullOrEmpty(txtComSGST.Text))
            {
                sgst_amt = 0;
            }
            else
            {
                sgst_amt = Convert.ToDecimal(TotalAmt.ToString()) * Convert.ToDecimal(txtComSGST.Text) / 100;
            }
            txtComSGSTAMT.Text = string.Format("{0:0.00}", sgst_amt);

            if (string.IsNullOrEmpty(txtComIGST.Text))
            {
                igst_amt = 0;
            }
            else
            {
                igst_amt = Convert.ToDecimal(TotalAmt.ToString()) * Convert.ToDecimal(txtComIGST.Text) / 100;
            }
            txtComIGSTAMT.Text = string.Format("{0:0.00}", igst_amt);

            tax_amt = cgst_amt + sgst_amt + igst_amt;

            var totalWithTax = Convert.ToDecimal(TotalAmt.ToString()) + Convert.ToDecimal(tax_amt.ToString());
            decimal disc_amt;
            if (string.IsNullOrEmpty(txtComDiscount.Text))
            {
                disc_amt = 0;
            }
            else
            {
                disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(txtComDiscount.Text) / 100;
                //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
            }

            var Grossamt = Convert.ToDecimal(TotalAmt.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
            txtComGrandTotal.Text = string.Format("{0:0.00}", Grossamt);
            txtComDiscountAMT.Text = string.Format("{0:0}", disc_amt);
        }
        catch { }
    }

    public void CompCalculations(GridViewRow row)
    {
        TextBox Rate = (TextBox)row.FindControl("txtCOMPRate");

        TextBox Qty = (TextBox)row.FindControl("txtCOMPQuantity");
        TextBox Total = (TextBox)row.FindControl("txtCOMPTotal");
        TextBox CGSTPer = (TextBox)row.FindControl("txtCOMPCGSTPer");
        TextBox SGSTPer = (TextBox)row.FindControl("txtCOMPSGSTPer");
        TextBox IGSTPer = (TextBox)row.FindControl("txtCOMPIGSTPer");
        TextBox txtCGSTamt = (TextBox)row.FindControl("txtCOMPCGST");
        TextBox txtSGSTamt = (TextBox)row.FindControl("txtCOMPSGST");
        TextBox txtIGSTamt = (TextBox)row.FindControl("txtCOMPIGST");
        TextBox Disc_Per = (TextBox)row.FindControl("txtCOMPDiscount");
        TextBox txtDiscountAmount = (TextBox)row.FindControl("txtCOMPDiscountAmount");

        TextBox GrossTotal = (TextBox)row.FindControl("txtCOMPGrandTotal");
        TextBox txtAlltotal = (TextBox)row.FindControl("txtCOMPAlltotal");

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


        // ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void txtComCGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtComQuantity.Text.Trim()) * Convert.ToDecimal(txtComRate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtComCGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtComCGSTAMT.Text = total.ToString();

        txtComSGSTAMT.Text = txtComCGSTAMT.Text;

        txtComSGST.Text = txtComCGST.Text;

        var GrandTotal = Convert.ToDecimal(txtComTotal.Text.Trim()) + Convert.ToDecimal(txtComCGSTAMT.Text.Trim()) + Convert.ToDecimal(txtComSGSTAMT.Text.Trim());
        txtComGrandTotal.Text = GrandTotal.ToString();


        if (txtComCGST.Text == "0" || txtComCGST.Text == "")
        {
            txtComIGST.Enabled = true;
            txtComIGST.Text = "0";
        }
        else
        {
            txtComIGST.Enabled = false;
            txtComIGST.Text = "0";
        }
    }

    protected void txtComSGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtComQuantity.Text.Trim()) * Convert.ToDecimal(txtComRate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtComSGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtComSGSTAMT.Text = total.ToString();

        txtComCGSTAMT.Text = txtComSGSTAMT.Text;

        txtComCGST.Text = txtComSGST.Text;

        var GrandTotal = Convert.ToDecimal(txtComTotal.Text.Trim()) + Convert.ToDecimal(txtComCGSTAMT.Text.Trim()) + Convert.ToDecimal(txtComSGSTAMT.Text.Trim());
        txtComGrandTotal.Text = GrandTotal.ToString();


        if (txtComSGST.Text == "0" || txtComSGST.Text == "")
        {
            txtComIGST.Enabled = true;
            txtComIGST.Text = "0";
        }
        else
        {
            txtComIGST.Enabled = false;
            txtComIGST.Text = "0";
        }
    }

    protected void txtComIGST_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtComQuantity.Text.Trim()) * Convert.ToDecimal(txtComRate.Text.Trim());

        decimal total;

        decimal Percentage = Convert.ToDecimal(txtComIGST.Text);

        total = (TotalAmt * Percentage / 100);

        txtComIGSTAMT.Text = total.ToString();

        var GrandTotal = Convert.ToDecimal(txtComTotal.Text.Trim()) + Convert.ToDecimal(txtComIGSTAMT.Text.Trim());
        txtComGrandTotal.Text = GrandTotal.ToString();


        if (txtComIGST.Text == "0" || txtComIGST.Text == "")
        {
            txtComCGST.Enabled = true;
            txtComCGST.Text = "0";
            txtComSGST.Enabled = true;
            txtComSGST.Text = "0";
        }
        else
        {
            txtComCGST.Enabled = false;
            txtComCGST.Text = "0";
            txtComSGST.Enabled = false;
            txtComSGST.Text = "0";
        }
    }

    protected void txtComDiscount_TextChanged(object sender, EventArgs e)
    {
        decimal DiscountAmt;
        decimal val1 = Convert.ToDecimal(txtComGrandTotal.Text);
        decimal val2 = Convert.ToDecimal(txtComDiscount.Text);
        DiscountAmt = (val1 * val2 / 100);
        txtComGrandTotal.Text = (val1 - DiscountAmt).ToString();

        txtComDiscountAMT.Text = DiscountAmt.ToString();
    }

    protected void txtCombtn_Addmore_Click(object sender, EventArgs e)
    {
        if (txtComQuantity.Text == "" || txtComRate.Text == "" || txtComGrandTotal.Text == "" || txtBatch.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill Quantity and Price !!!');", true);
            txtComQuantity.Focus();
        }
        else
        {
            ShowCom_Grid();
        }
    }

    private void ShowCom_Grid()
    {
        divCOMPTotalPart.Visible = true;
        DataTable Dt = (DataTable)ViewState["Components"];
        Dt.Rows.Add(ViewState["RowNo"], ddlcomponent.SelectedItem.Text, txtBatch.Text, txtComDiscription.Text.Trim(), txtComHSN.Text.Trim(), txtComQuantity.Text, txtComUnit.Text, txtComRate.Text, txtComTotal.Text, txtComCGST.Text, txtComCGSTAMT.Text, txtComSGST.Text, txtComSGSTAMT.Text, txtComIGST.Text, txtComIGSTAMT.Text, txtComDiscount.Text, txtComDiscountAMT.Text, txtComGrandTotal.Text);
        ViewState["Components"] = Dt;
        FillddlComponent();
        txtComDiscription.Text = string.Empty;
        txtComHSN.Text = string.Empty;
        txtComQuantity.Text = string.Empty;
        txtComUnit.Text = string.Empty;
        txtComRate.Text = string.Empty;
        txtComTotal.Text = string.Empty;
        txtComCGST.Text = string.Empty;
        txtComCGSTAMT.Text = string.Empty;
        txtComSGST.Text = string.Empty;
        txtComSGSTAMT.Text = string.Empty;
        txtComIGST.Text = string.Empty;
        txtComIGSTAMT.Text = string.Empty;
        txtComDiscount.Text = string.Empty;
        txtComDiscountAMT.Text = string.Empty;
        txtComGrandTotal.Text = string.Empty;
        gvcomponent.DataSource = (DataTable)ViewState["Components"];
        gvcomponent.DataBind();
    }

    protected void txtCOMPDiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        CompCalculations(row);
    }

    protected void txtCOMPIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        CompCalculations(row);
    }

    protected void txtCOMPSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        CompCalculations(row);

    }

    protected void txtCOMPCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        CompCalculations(row);
    }

    protected void txtCOMPRate_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        CompCalculations(row);
    }

    protected void txtCOMPQuantity_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        CompCalculations(row);
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("InwardEntryList.aspx");
    }
    protected void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("InwardEntryList.aspx");
    }

    protected void txtComRate_TextChanged(object sender, EventArgs e)
    {
        var TotalAmt = Convert.ToDecimal(txtComQuantity.Text.Trim()) * Convert.ToDecimal(txtComRate.Text.Trim());
        txtComTotal.Text = TotalAmt.ToString();


        decimal tax_amt;
        decimal cgst_amt;
        decimal sgst_amt;
        decimal igst_amt;

        if (string.IsNullOrEmpty(txtComCGST.Text))
        {
            cgst_amt = 0;
        }
        else
        {
            cgst_amt = Convert.ToDecimal(TotalAmt.ToString()) * Convert.ToDecimal(txtComCGST.Text) / 100;
        }
        txtComCGSTAMT.Text = string.Format("{0:0.00}", cgst_amt);

        if (string.IsNullOrEmpty(txtComSGST.Text))
        {
            sgst_amt = 0;
        }
        else
        {
            sgst_amt = Convert.ToDecimal(TotalAmt.ToString()) * Convert.ToDecimal(txtComSGST.Text) / 100;
        }
        txtComSGSTAMT.Text = string.Format("{0:0.00}", sgst_amt);

        if (string.IsNullOrEmpty(txtComIGST.Text))
        {
            igst_amt = 0;
        }
        else
        {
            igst_amt = Convert.ToDecimal(TotalAmt.ToString()) * Convert.ToDecimal(txtComIGST.Text) / 100;
        }
        txtComIGSTAMT.Text = string.Format("{0:0.00}", igst_amt);

        tax_amt = cgst_amt + sgst_amt + igst_amt;

        var totalWithTax = Convert.ToDecimal(TotalAmt.ToString()) + Convert.ToDecimal(tax_amt.ToString());
        decimal disc_amt;
        if (string.IsNullOrEmpty(txtComDiscount.Text))
        {
            disc_amt = 0;
        }
        else
        {
            disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(txtComDiscount.Text) / 100;
            //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
        }

        var Grossamt = Convert.ToDecimal(TotalAmt.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
        txtComGrandTotal.Text = string.Format("{0:0.00}", Grossamt);
        txtComDiscountAMT.Text = string.Format("{0:0}", disc_amt);
    }

    private void Fillddltransport()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT DISTINCT TransporterName FROM tbl_TransporterMaster where Status=1 and IsDeleted=0", Cls_Main.Conn);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddltransportname.DataSource = dt;
            //ddlProduct.DataValueField = "ID";
            ddltransportname.DataTextField = "TransporterName";
            ddltransportname.DataBind();
            ddltransportname.Items.Insert(0, "-- Select Transporter Name --");
        }
    }
}