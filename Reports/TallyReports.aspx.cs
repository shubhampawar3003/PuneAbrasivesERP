using ClosedXML.Excel;
using Microsoft.Reporting.WebForms;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Media.Animation;
using System.Xml;

public partial class Reports_TallyReports : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (Session["Username"] == null)
        //{
        //    Response.Redirect("../Login.aspx");
        //}
        //else
        //{
        if (!IsPostBack)
        {

            btn.Visible = true;

        }
        // }
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("PartyLedgerReport.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCustomerList(string prefixText, int count)
    {

        return AutoFillCustomerlist(prefixText);
    }

    public static List<string> AutoFillCustomerlist(string prefixText)
    {

        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {

                com.CommandText = "select DISTINCT Companyname from tbl_CompanyMaster where " + "Companyname like @Search + '%' AND IsDeleted=0 ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> cname = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        cname.Add(sdr["Companyname"].ToString());
                    }
                }
                con.Close();
                return cname;
            }

        }
    }


    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierList(string prefixText, int count)
    {

        return AutoFillSupplierlist(prefixText);
    }

    public static List<string> AutoFillSupplierlist(string prefixText)
    {

        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {

                com.CommandText = "select DISTINCT VendorName from tbl_VendorMaster where " + "VendorName like @Search + '%' AND IsDeleted=0 ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> SupplierName = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        SupplierName.Add(sdr["VendorName"].ToString());
                    }
                }
                con.Close();
                return SupplierName;
            }

        }
    }


    protected void btnpdf_Click1(object sender, EventArgs e)
    {

        string command = ((Button)sender).CommandName;

        if (command == "Ledger")
        {
            DataSet Dtt = GetDetails(command);
            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    DataTable ledgerTable = Dtt.Tables[0].Copy();

                    ledgerTable.Columns["GroupName"].ColumnName = "Group Name";
                    ledgerTable.Columns["OpeningBalance"].ColumnName = "Ledger - Opening Balance";
                    ledgerTable.Columns["Openingbalancedr"].ColumnName = "Ledger Opening Balance - Dr/Cr";
                    ledgerTable.Columns["Billdate"].ColumnName = "Bill - Date";
                    ledgerTable.Columns["BillNo"].ColumnName = "Bill - Name";
                    ledgerTable.Columns["BillAmount"].ColumnName = "Bill - Amount";
                    ledgerTable.Columns["BillDr"].ColumnName = "Bill Amount - Dr/Cr";
                    ledgerTable.Columns["BillingAddress"].ColumnName = "Address";
                    ledgerTable.Columns["BillingStatecode"].ColumnName = "State";
                    ledgerTable.Columns["BillingPincode"].ColumnName = "Pincode";
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(ledgerTable, command);
                        // workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                        // Export to browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=Ledgers.xlsx");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }
        if (command == "Group")
        {
            DataSet Dtt = GetDetails(command);
            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {

                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(Dtt.Tables[0], command);
                        // workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                        // Export to browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=Group.xlsx");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }
        if (command == "CostCategory" || command == "CostCenter")
        {
            DataSet Dtt = GetDetails("CostCategory");
            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {

                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(Dtt.Tables[0], command);
                        // workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                        // Export to browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=" + command + ".xlsx");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }
        if (command == "StockGroup")
        {
            DataSet Dtt = GetDetails(command);
            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {

                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(Dtt.Tables[0], command);

                        // Export to browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=" + command + ".xlsx");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }
        if (command == "StockItem")
        {
            DataSet Dtt = GetDetails(command);
            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    DataTable itemTable = Dtt.Tables[0].Copy();
                    itemTable.Columns["GroupName"].ColumnName = "Group Name";
                    itemTable.Columns["Units"].ColumnName = "Units";
                    itemTable.Columns["OpeningBalanceqty"].ColumnName = "Opening Balance - Quantity";
                    itemTable.Columns["OpeningBalanceRate"].ColumnName = "Opening Balance - Rate";
                    itemTable.Columns["OpeningBalanceRateper"].ColumnName = "Opening Balance - Rate per";
                    itemTable.Columns["OpeningBalanceValue"].ColumnName = "Opening Balance - Value";

                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(itemTable, command);
                        // workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                        // Export to browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=" + command + ".xlsx");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }
        if (command == "Unit")
        {

            List<KeyValuePair<string, string>> unitList = new List<KeyValuePair<string, string>>()
{
    new KeyValuePair<string, string>("BAG", "Bag"),
    new KeyValuePair<string, string>("BDL", "Bundle"),
    new KeyValuePair<string, string>("BTL", "Bottle"),
    new KeyValuePair<string, string>("BOX", "Box"),
    new KeyValuePair<string, string>("CAN", "Can"),
    new KeyValuePair<string, string>("CTN", "Carton"),
    new KeyValuePair<string, string>("DOZ", "Dozen"),
    new KeyValuePair<string, string>("GMS", "Grams"),
    new KeyValuePair<string, string>("KGS", "Kilograms"),
    new KeyValuePair<string, string>("LTR", "Litres"),
    new KeyValuePair<string, string>("MTR", "Meters"),
    new KeyValuePair<string, string>("NOS", "Numbers"),
    new KeyValuePair<string, string>("PAC", "Packet"),
    new KeyValuePair<string, string>("PCS", "Pieces"),
    new KeyValuePair<string, string>("QTL", "Quintal"),
    new KeyValuePair<string, string>("SET", "Set"),
    new KeyValuePair<string, string>("TBS", "Tablets"),
    new KeyValuePair<string, string>("TGM", "Ten Grams"),
    new KeyValuePair<string, string>("TON", "Tonnes"),
    new KeyValuePair<string, string>("UNT", "Unit"),
    new KeyValuePair<string, string>("MLT", "Millilitres"),
    new KeyValuePair<string, string>("MGS", "Milligrams"),
};

            // Convert to DataTable
            DataTable unitTable = new DataTable("Units");
            unitTable.Columns.Add("Symbol", typeof(string));
            //unitTable.Columns.Add("Description", typeof(string));

            foreach (var kvp in unitList)
            {
                unitTable.Rows.Add(kvp.Key);
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Worksheets.Add(unitTable, command);
                // workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                // Export to browser
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + command + ".xlsx");

                using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                {
                    workbook.SaveAs(memoryStream);
                    memoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        if (command == "AccountingVoucher")
        {
            DataSet Dtt = GetDetails(command);
            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    DataTable voucherTable = Dtt.Tables[0].Copy();

                    // Rename columns to your desired field names
                    voucherTable.Columns["CDate"].ColumnName = "Voucher Date";
                    voucherTable.Columns["Type"].ColumnName = "Voucher Type Name";
                    voucherTable.Columns["DocNo"].ColumnName = "Voucher Number";
                    voucherTable.Columns["billingaddress"].ColumnName = "Buyer/Supplier - Address";
                    voucherTable.Columns["pincode"].ColumnName = "Buyer/Supplier - Pincode";
                    voucherTable.Columns["SupplierName"].ColumnName = "Ledger Name";  
                    voucherTable.Columns["Balance"].ColumnName = "Ledger Amount";  
                    voucherTable.Columns["DrCr"].ColumnName = "Ledger Amount Dr/Cr";
                    voucherTable.Columns["Particulars"].ColumnName = "Item Name";
                    voucherTable.Columns["Qty"].ColumnName = "Billed Quantity";
                    voucherTable.Columns["Rate"].ColumnName = "Item Rate";                  
                    voucherTable.Columns["ItemRateper"].ColumnName = "Item Rate per";
                    voucherTable.Columns["ItemAmount"].ColumnName = "Item Amount";
                    voucherTable.Columns["ChangeMode"].ColumnName = "Change Mode";
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        workbook.Worksheets.Add(voucherTable, command);
                        // workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                        // Export to browser
                        Response.Clear();
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=" + command + ".xlsx");

                        using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
                        {
                            workbook.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
        }

    }
    public DataSet GetDetails(string action)
    {
        DataSet Dtt = new DataSet();
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand("dbo.SP_TallyReports", con))
            {
                string fdate;
                string tdate;
                string ft = txtfromdate.Text;
                string tt = txttodate.Text;
                if (ft == "")
                {
                    fdate = "";
                }
                else
                {
                    DateTime ftdate = Convert.ToDateTime(ft, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    fdate = ftdate.ToString("yyyy-MM-dd");
                }

                if (tt == "")
                {
                    tdate = "";
                }
                else
                {
                    DateTime date = Convert.ToDateTime(tt, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    tdate = date.ToString("yyyy-MM-dd");
                }

                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@Type", "SALE");
                cmd.Parameters.AddWithValue("@Action", action);
                cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                if (fdate != null && fdate != "")
                {
                    cmd.Parameters.AddWithValue("@FromDate", fdate);
                }
                if (tdate != null && tdate != "")
                {
                    cmd.Parameters.AddWithValue("@ToDate", tdate);
                }

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
        return Dtt;
    }


}
