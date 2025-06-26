using ClosedXML.Excel;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

public partial class Reports_PartyLedgerReport : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (!IsPostBack)
            {

                btn.Visible = true;
               // GetData();
            }
        }
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
    //int count = 0;
    //protected void ddltype_TextChanged(object sender, EventArgs e)
    //{
    //    GetData();
    //    if (ddltype.Text == "SALE")
    //    {
    //        AutoCompleteExtender1.Enabled = true;
    //        txtPartyName.Text = string.Empty;
    //        GetCustomerList(txtPartyName.Text, count);
    //        AutoCompleteExtender2.Enabled = false;

    //    }

    //    else if (ddltype.Text == "PURCHASE")
    //    {
    //        AutoCompleteExtender2.Enabled = true;
    //        txtPartyName.Text = string.Empty;
    //        GetSupplierList(txtPartyName.Text, count);
    //        AutoCompleteExtender1.Enabled = false;

    //    }
    //}

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


    protected void btnpdf_Click(object sender, EventArgs e)
    {

        Report("PDF");
    }



    protected void ExportExcel(object sender, EventArgs e)
    {
        Report("Excel");

    }
    protected void Report(string flg)
    {
        DataSet Dtt = new DataSet();
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand("[SP_PartyLedgerRDLC]", con))
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
                cmd.Parameters.AddWithValue("@Type", "SALE");
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

        if (Dtt.Tables.Count > 0)
        {
            if (Dtt.Tables[0].Rows.Count > 0)
            {
                ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                ReportViewer1.LocalReport.DataSources.Add(obj1);
                ReportViewer1.LocalReport.DataSources.Add(obj2);
                ReportViewer1.LocalReport.DataSources.Add(obj3);
                ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PartyLedger.rdlc";
                ReportViewer1.LocalReport.Refresh();

                //--------- For Exporting as XML --------------
                if (flg == "XML")
                {
                    // Export dataset as XML
                    //Response.Clear();
                    //Response.ContentType = "text/xml";
                    //Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + "_PartyLedger.xml\"");

                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    // Write the XML to memory stream
                    //    Dtt.WriteXml(ms);
                    //    Response.BinaryWrite(ms.ToArray());
                    //}

                    //Response.End();
                    //return;


                    //if (Dtt.Tables.Count > 0 && Dtt.Tables[0].Rows.Count > 0)
                    //{
                    //    XmlDocument tallyXml = new XmlDocument();
                    //    XmlElement envelope = tallyXml.CreateElement("ENVELOPE");
                    //    tallyXml.AppendChild(envelope);

                    //    // HEADER
                    //    XmlElement header = tallyXml.CreateElement("HEADER");
                    //    XmlElement tallyRequest = tallyXml.CreateElement("TALLYREQUEST");
                    //    tallyRequest.InnerText = "Import Data";
                    //    header.AppendChild(tallyRequest);
                    //    envelope.AppendChild(header);

                    //    // BODY
                    //    XmlElement body = tallyXml.CreateElement("BODY");
                    //    XmlElement importData = tallyXml.CreateElement("IMPORTDATA");

                    //    XmlElement requestDesc = tallyXml.CreateElement("REQUESTDESC");
                    //    XmlElement reportName = tallyXml.CreateElement("REPORTNAME");
                    //    reportName.InnerText = "Vouchers";
                    //    requestDesc.AppendChild(reportName);
                    //    importData.AppendChild(requestDesc);

                    //    XmlElement requestData = tallyXml.CreateElement("REQUESTDATA");

                    //    foreach (DataRow row in Dtt.Tables[0].Rows)
                    //    {
                    //        string docNo = row["DocNo"].ToString();
                    //        string chequeNo = row["ChekNo"].ToString();
                    //        string mode = row["Particulars"].ToString();
                    //        string amount = row["Credit"].ToString();
                    //        string dateStr = row["CDate"].ToString();

                    //        string customerLedger = txtPartyName.Text.Trim();
                    //        string paymentLedger = mode;

                    //        DateTime dt = DateTime.ParseExact(dateStr, "dd/MM/yyyy", null);
                    //        string tallyDate = dt.ToString("yyyyMMdd");

                    //        XmlElement tallyMessage = tallyXml.CreateElement("TALLYMESSAGE");
                    //        tallyMessage.SetAttribute("xmlns:UDF", "TallyUDF");

                    //        XmlElement voucher = tallyXml.CreateElement("VOUCHER");
                    //        voucher.SetAttribute("REMOTEID", "");
                    //        voucher.SetAttribute("VCHTYPE", "Receipt");
                    //        voucher.SetAttribute("ACTION", "Create");
                    //        voucher.SetAttribute("OBJVIEW", "Accounting Voucher View");

                    //        // Add standard voucher elements manually (no local function)
                    //        XmlElement elDate = tallyXml.CreateElement("DATE");
                    //        elDate.InnerText = tallyDate;
                    //        voucher.AppendChild(elDate);

                    //        XmlElement elNarration = tallyXml.CreateElement("NARRATION");
                    //        elNarration.InnerText = "Receipt via " + mode + " - Cheque: " + chequeNo;
                    //        voucher.AppendChild(elNarration);

                    //        XmlElement elVType = tallyXml.CreateElement("VOUCHERTYPENAME");
                    //        elVType.InnerText = "Receipt";
                    //        voucher.AppendChild(elVType);

                    //        XmlElement elVNo = tallyXml.CreateElement("VOUCHERNUMBER");
                    //        elVNo.InnerText = docNo;
                    //        voucher.AppendChild(elVNo);

                    //        XmlElement elParty = tallyXml.CreateElement("PARTYLEDGERNAME");
                    //        elParty.InnerText = customerLedger;
                    //        voucher.AppendChild(elParty);

                    //        XmlElement elView = tallyXml.CreateElement("PERSISTEDVIEW");
                    //        elView.InnerText = "Accounting Voucher View";
                    //        voucher.AppendChild(elView);

                    //        // Credit Entry (Payment mode - e.g. Cash/Bank)
                    //        XmlElement entry1 = tallyXml.CreateElement("ALLLEDGERENTRIES.LIST");
                    //        XmlElement ledger1 = tallyXml.CreateElement("LEDGERNAME");
                    //        ledger1.InnerText = paymentLedger;
                    //        entry1.AppendChild(ledger1);

                    //        XmlElement pos1 = tallyXml.CreateElement("ISDEEMEDPOSITIVE");
                    //        pos1.InnerText = "No";
                    //        entry1.AppendChild(pos1);

                    //        XmlElement amt1 = tallyXml.CreateElement("AMOUNT");
                    //        amt1.InnerText = amount;
                    //        entry1.AppendChild(amt1);
                    //        voucher.AppendChild(entry1);

                    //        // Debit Entry (Customer)
                    //        XmlElement entry2 = tallyXml.CreateElement("ALLLEDGERENTRIES.LIST");
                    //        XmlElement ledger2 = tallyXml.CreateElement("LEDGERNAME");
                    //        ledger2.InnerText = customerLedger;
                    //        entry2.AppendChild(ledger2);

                    //        XmlElement pos2 = tallyXml.CreateElement("ISDEEMEDPOSITIVE");
                    //        pos2.InnerText = "Yes";
                    //        entry2.AppendChild(pos2);

                    //        XmlElement amt2 = tallyXml.CreateElement("AMOUNT");
                    //        amt2.InnerText = "-" + amount;
                    //        entry2.AppendChild(amt2);
                    //        voucher.AppendChild(entry2);

                    //        // Add voucher to tally message and data
                    //        tallyMessage.AppendChild(voucher);
                    //        requestData.AppendChild(tallyMessage);
                    //    }

                    //    importData.AppendChild(requestData);
                    //    body.AppendChild(importData);
                    //    envelope.AppendChild(body);

                    //    // Export XML to user
                    //    Response.Clear();
                    //    Response.ContentType = "text/xml";
                    //    Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + "_ReceiptVouchers.xml\"");
                    //    tallyXml.Save(Response.OutputStream);
                    //    Response.End();
                    //    return;

                    //}
                    //XmlDocument tallyXml = new XmlDocument();
                    //XmlElement envelope = tallyXml.CreateElement("ENVELOPE");
                    //tallyXml.AppendChild(envelope);

                    //// Header
                    //XmlElement header = tallyXml.CreateElement("HEADER");
                    //XmlElement tallyRequest = tallyXml.CreateElement("TALLYREQUEST");
                    //tallyRequest.InnerText = "Import Data";
                    //header.AppendChild(tallyRequest);
                    //envelope.AppendChild(header);

                    //// Body
                    //XmlElement body = tallyXml.CreateElement("BODY");
                    //XmlElement importData = tallyXml.CreateElement("IMPORTDATA");
                    //XmlElement requestDesc = tallyXml.CreateElement("REQUESTDESC");
                    //XmlElement reportName = tallyXml.CreateElement("REPORTNAME");
                    //reportName.InnerText = "Vouchers";
                    //requestDesc.AppendChild(reportName);
                    //importData.AppendChild(requestDesc);
                    //XmlElement requestData = tallyXml.CreateElement("REQUESTDATA");

                    //// Loop over DataTable rows (assuming Dtt is a DataTable containing the relevant data)
                    //foreach (DataRow row in Dtt.Tables[0].Rows)
                    //{
                    //    string docNo = row["DocNo"].ToString();
                    //    string particulars = row["Particulars"].ToString();  // You can modify this to the actual particulars
                    //    string chequeNo = row["chekNo"].ToString();  // You can modify this to the actual cheque number
                    //    string credit = "0.00";  // As given, you want Credit to be 0.00
                    //    string debit = row["Balance"].ToString();
                    //    string dateStr = row["CDate"].ToString(); // dd/MM/yyyy
                    //    DateTime date = DateTime.ParseExact(dateStr, "dd/MM/yyyy", null);
                    //    string tallyDate = date.ToString("yyyyMMdd");

                    //    XmlElement tallyMessage = tallyXml.CreateElement("TALLYMESSAGE");
                    //    tallyMessage.SetAttribute("xmlns:UDF", "TallyUDF");

                    //    XmlElement voucher = tallyXml.CreateElement("VOUCHER");
                    //    voucher.SetAttribute("VCHTYPE", "Sales Invoice");
                    //    voucher.SetAttribute("ACTION", "Create");
                    //    voucher.SetAttribute("OBJVIEW", "Accounting Voucher View");

                    //    // DATE
                    //    XmlElement dateEl = tallyXml.CreateElement("DATE");
                    //    dateEl.InnerText = tallyDate;
                    //    voucher.AppendChild(dateEl);

                    //    // NARRATION
                    //    XmlElement narration = tallyXml.CreateElement("NARRATION");
                    //    narration.InnerText = "Sales Invoice for Doc No: " + docNo;
                    //    voucher.AppendChild(narration);

                    //    XmlElement type = tallyXml.CreateElement("VOUCHERTYPENAME");
                    //    type.InnerText = "Sales Invoice";
                    //    voucher.AppendChild(type);

                    //    XmlElement vchNo = tallyXml.CreateElement("VOUCHERNUMBER");
                    //    vchNo.InnerText = docNo;
                    //    voucher.AppendChild(vchNo);

                    //    XmlElement party = tallyXml.CreateElement("PARTYLEDGERNAME");
                    //    party.InnerText = row["SupplierName"].ToString();  // Assuming Party Name is in the DataTable
                    //    voucher.AppendChild(party);

                    //    XmlElement view = tallyXml.CreateElement("PERSISTEDVIEW");
                    //    view.InnerText = "Accounting Voucher View";
                    //    voucher.AppendChild(view);

                    //    // Debit Entry (Sales)
                    //    XmlElement debitEntry = tallyXml.CreateElement("ALLLEDGERENTRIES.LIST");
                    //    XmlElement ledger1 = tallyXml.CreateElement("LEDGERNAME");
                    //    ledger1.InnerText = "Sales"; // Assuming you have a Sales Ledger created in Tally
                    //    XmlElement pos1 = tallyXml.CreateElement("ISDEEMEDPOSITIVE");
                    //    pos1.InnerText = "Yes";
                    //    XmlElement amt1 = tallyXml.CreateElement("AMOUNT");
                    //    amt1.InnerText = "-" + debit;  // Negative because it's a debit
                    //    debitEntry.AppendChild(ledger1);
                    //    debitEntry.AppendChild(pos1);
                    //    debitEntry.AppendChild(amt1);
                    //    voucher.AppendChild(debitEntry);

                    //    // Credit Entry (Receivables / Party Ledger)
                    //    XmlElement creditEntry = tallyXml.CreateElement("ALLLEDGERENTRIES.LIST");
                    //    XmlElement ledger2 = tallyXml.CreateElement("LEDGERNAME");
                    //    ledger2.InnerText = row["SupplierName"].ToString();  // Use the party name from the DataTable
                    //    XmlElement pos2 = tallyXml.CreateElement("ISDEEMEDPOSITIVE");
                    //    pos2.InnerText = "No";
                    //    XmlElement amt2 = tallyXml.CreateElement("AMOUNT");
                    //    amt2.InnerText = debit;  // Positive amount for credit
                    //    creditEntry.AppendChild(ledger2);
                    //    creditEntry.AppendChild(pos2);
                    //    creditEntry.AppendChild(amt2);
                    //    voucher.AppendChild(creditEntry);

                    //    tallyMessage.AppendChild(voucher);
                    //    requestData.AppendChild(tallyMessage);

                    //    // Add Ledger Masters (if not already created)
                    //    XmlElement ledgerMasterSales = tallyXml.CreateElement("TALLYMESSAGE");
                    //    ledgerMasterSales.SetAttribute("xmlns:UDF", "TallyUDF");

                    //    XmlElement ledgerSales = tallyXml.CreateElement("LEDGER");
                    //    ledgerSales.SetAttribute("NAME", "Sales"); // Sales Ledger Master
                    //    ledgerSales.SetAttribute("ACTION", "Create");

                    //    XmlElement ledgerNameSales = tallyXml.CreateElement("NAME");
                    //    ledgerNameSales.InnerText = "Sales";
                    //    ledgerSales.AppendChild(ledgerNameSales);

                    //    XmlElement ledgerGroupSales = tallyXml.CreateElement("GROUP");
                    //    ledgerGroupSales.InnerText = "Sales Accounts";
                    //    ledgerSales.AppendChild(ledgerGroupSales);

                    //    ledgerMasterSales.AppendChild(ledgerSales);
                    //    requestData.AppendChild(ledgerMasterSales);

                    //    XmlElement ledgerMasterParty = tallyXml.CreateElement("TALLYMESSAGE");
                    //    ledgerMasterParty.SetAttribute("xmlns:UDF", "TallyUDF");

                    //    XmlElement ledgerParty = tallyXml.CreateElement("LEDGER");
                    //    ledgerParty.SetAttribute("NAME", row["SupplierName"].ToString()); // Party Ledger Master
                    //    ledgerParty.SetAttribute("ACTION", "Create");

                    //    XmlElement ledgerNameParty = tallyXml.CreateElement("NAME");
                    //    ledgerNameParty.InnerText = row["SupplierName"].ToString();
                    //    ledgerParty.AppendChild(ledgerNameParty);

                    //    XmlElement ledgerGroupParty = tallyXml.CreateElement("GROUP");
                    //    ledgerGroupParty.InnerText = "Sundry Creditors"; // Assuming it's a Sundry Creditors group
                    //    ledgerParty.AppendChild(ledgerGroupParty);

                    //    ledgerMasterParty.AppendChild(ledgerParty);
                    //    requestData.AppendChild(ledgerMasterParty);

                    //}

                    //// Append all request data
                    //importData.AppendChild(requestData);
                    //body.AppendChild(importData);
                    //envelope.AppendChild(body);

                    //// Export as file (download the XML)
                    //Response.Clear();
                    //Response.ContentType = "text/xml";
                    //Response.AddHeader("content-disposition", "attachment;filename=SalesInvoiceReport.xml");
                    //tallyXml.Save(Response.OutputStream);
                    //Response.End();
                    XmlDocument tallyXml = new XmlDocument();
                    XmlElement envelope = tallyXml.CreateElement("ENVELOPE");
                    tallyXml.AppendChild(envelope);

                    // Header
                    XmlElement header = tallyXml.CreateElement("HEADER");
                    XmlElement tallyRequest = tallyXml.CreateElement("TALLYREQUEST");
                    tallyRequest.InnerText = "Import Data";
                    header.AppendChild(tallyRequest);
                    envelope.AppendChild(header);

                    // Body
                    XmlElement body = tallyXml.CreateElement("BODY");
                    XmlElement importData = tallyXml.CreateElement("IMPORTDATA");
                    XmlElement requestDesc = tallyXml.CreateElement("REQUESTDESC");
                    XmlElement reportName = tallyXml.CreateElement("REPORTNAME");
                    reportName.InnerText = "Masters"; // For importing Ledger Masters
                    requestDesc.AppendChild(reportName);
                    importData.AppendChild(requestDesc);
                    XmlElement requestData = tallyXml.CreateElement("REQUESTDATA");

                    // Loop over DataTable rows (assuming Dtt is a DataTable containing the relevant ledger data)
                    foreach (DataRow row in Dtt.Tables[0].Rows)
                    {
                        // Assuming you have fields like Ledger Name, Group, Opening Balance etc. in your DataTable
                        string ledgerName = row["LedgerName"].ToString();
                        string group = row["Group"].ToString(); // Group can be dynamic or from your data
                        string openingBalance = row["OpeningBalance"].ToString();
                        string parentGroup = row["ParentGroup"].ToString(); // Parent group like 'Current Assets' or 'Sundry Debtors'

                        // Creating the TALLYMESSAGE for each ledger
                        XmlElement tallyMessage = tallyXml.CreateElement("TALLYMESSAGE");
                        tallyMessage.SetAttribute("xmlns:UDF", "TallyUDF");

                        XmlElement ledger = tallyXml.CreateElement("LEDGER");
                        ledger.SetAttribute("NAME", ledgerName);
                        ledger.SetAttribute("ACTION", "Create");

                        // Name of the Ledger
                        XmlElement name = tallyXml.CreateElement("NAME");
                        name.InnerText = ledgerName;
                        ledger.AppendChild(name);

                        // Group for the Ledger
                        XmlElement groupElement = tallyXml.CreateElement("GROUP");
                        groupElement.InnerText = group;
                        ledger.AppendChild(groupElement);

                        // Parent Group (optional, but useful for categorization)
                        XmlElement parent = tallyXml.CreateElement("PARENT");
                        parent.InnerText = parentGroup;
                        ledger.AppendChild(parent);

                        // Opening Balance
                        XmlElement openingBalanceElement = tallyXml.CreateElement("OPENINGBALANCE");
                        openingBalanceElement.InnerText = openingBalance;
                        ledger.AppendChild(openingBalanceElement);

                        // Add the created ledger to the request data
                        tallyMessage.AppendChild(ledger);
                        requestData.AppendChild(tallyMessage);
                    }

                    // Append all request data
                    importData.AppendChild(requestData);
                    body.AppendChild(importData);
                    envelope.AppendChild(body);

                    // Export as file (download the XML)
                    Response.Clear();
                    Response.ContentType = "text/xml";
                    Response.AddHeader("content-disposition", "attachment;filename=LedgerMasterImport.xml");
                    tallyXml.Save(Response.OutputStream);
                    Response.End();




                }
                else
                {
                    //--------- For other formats like PDF --------
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render(flg, null, out mimeType, out encoding, out extension, out streamids, out warnings);

                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.Buffer = true;

                    // Handle other formats like PDF, Excel, etc.
                    if (flg == "Excel")
                    {
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + "_PartyLedger.xls\"");

                        Response.BinaryWrite(bytePdfRep);
                    }
                    else
                    {
                        Response.ContentType = mimeType;
                        Response.AddHeader("content-disposition", "attachment;filename=\"" + txtPartyName.Text + "_PartyLedger." + extension + "\"");

                        Response.BinaryWrite(bytePdfRep);
                    }
                    Response.End();
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
            }
        }
    }



    public void GetData()
    {
        DataSet Dtt = new DataSet();
        string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        using (SqlConnection con = new SqlConnection(strConnString))
        {
            using (SqlCommand cmd = new SqlCommand("[SP_PartyLedgerRDLC]", con))
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

                    //var fttime = Convert.ToDateTime(ft);
                    //fdate = fttime.ToString("yyyy-MM-dd");
                }

                if (tt == "")
                {
                    tdate = "";
                }
                else
                {

                    DateTime date = Convert.ToDateTime(tt, System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                    tdate = date.ToString("yyyy-MM-dd");
                    //var tttime = Convert.ToDateTime(tt);
                    //tdate = tttime.ToString("yyyy-MM-dd");
                }
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Type", "SALE");
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
        if (Dtt.Tables.Count > 0)
        {
            if (Dtt.Tables[0].Rows.Count > 0)
            {
                GVfollowup.DataSource = Dtt;
                GVfollowup.DataBind();
            }
        }
    }

    protected void GVfollowup_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            decimal totalDebit = 0;
            decimal totalCredit = 0;
            decimal totalbalance = 0;

            // Loop through the data rows to calculate the totals
            foreach (GridViewRow row in GVfollowup.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Calculate the total for each column
                    totalDebit += Convert.ToDecimal((row.FindControl("lblDebit") as Label).Text);
                    totalCredit += Convert.ToDecimal((row.FindControl("lblCredit") as Label).Text);
                    totalbalance += Convert.ToDecimal((row.FindControl("lblBalance") as Label).Text);

                }
            }

      // Display the totals in the footer labels
      (e.Row.FindControl("lblTotalDebit") as Label).Text = totalDebit.ToString();
            (e.Row.FindControl("lblTotalCredit") as Label).Text = totalCredit.ToString();
            (e.Row.FindControl("lblTotalBalance") as Label).Text = totalbalance.ToString();

        }
    }

    protected void txtPartyName_TextChanged(object sender, EventArgs e)
    {
        GetData();
    }

    protected void GVfollowup_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVfollowup.PageIndex = e.NewPageIndex;
        GetData();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        GetData();
    }

    protected void btnpdf_Click1(object sender, EventArgs e)
    {
        // Report("XML");
        ExportToTally();
    }

    public void ExportToTally()
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
                cmd.Parameters.AddWithValue("@Action", "GetLedgerdata");
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
                    workbook.Worksheets.Add(ledgerTable, "Ledgers");
                    workbook.Worksheets.Add(Dtt.Tables[1], "Vouchers");

                    // Export to browser
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=TallyExport.xlsx");

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
