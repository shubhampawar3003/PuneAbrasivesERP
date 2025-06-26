using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

public partial class Inventory_TestImport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e) { }

    protected void btnLedger_Click(object sender, EventArgs e) { ExportDataToTally("Ledger"); }
    protected void btnGroup_Click(object sender, EventArgs e) { ExportDataToTally("Group"); }
    protected void btnStockItem_Click(object sender, EventArgs e) { ExportDataToTally("StockItem"); }
    protected void btnUnit_Click(object sender, EventArgs e) { ExportDataToTally("Unit"); }

    private void ExportDataToTally(string command)
    {
        DataTable dt = GetMockData(command);
        if (dt == null || dt.Rows.Count == 0)
        {
            ltXmlOutput.Text = "<p style='color:red;'>No data found for " + command + "</p>";
            return;
        }

        string xml = string.Empty;
        switch (command)
        {
            case "Ledger":
                xml = GenerateLedgerXML(dt);
                break;
            case "Group":
                xml = GenerateGroupXML(dt);
                break;
            case "StockItem":
                xml = GenerateStockItemXML(dt);
                break;
            case "Unit":
                xml = GenerateUnitXML(dt);
                break;
        }
     
        string filename = command + ".xml";
        string localIP = GetClientIPAddress();

        string importUrl = string.Format("http://{0}:9000/import?file={1}", localIP, filename);

        string instructionHtml = GenerateInstruction(command, importUrl);
        ltPostmanInstructions.Text = instructionHtml;

        // Display the XML content
        ltXmlOutput.Text = "<h3>Generated XML for " + command + "</h3>" +
       "<pre style='background:#f4f4f4; padding:10px; border:1px solid #ccc; " +
       "max-height:400px; overflow:auto; width:1000px; height:500px;'>" +
       HttpUtility.HtmlEncode(xml) + "</pre>";
    }

    private string GetClientIPAddress()
    {
        try
        {
            string localIP = "Not Available";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // IPv4 and not loopback address
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            throw new Exception("No IP address found.");
        }
        catch (Exception ex)
        {
            // Optional: Log the error ex.Message here
            return "localhost"; // fallback value
        }
    }


    private DataTable GetMockData(string command)
    {
        DataTable dt = new DataTable();

        if (command == "Ledger")
        {
      
            dt = GetDetails("Ledger");
        }
        else if (command == "Group")
        {
       
            dt = GetDetails("Group");
        }
        else if (command == "StockItem")
        {
          
            dt = GetDetails("StockItem");
        }
        else if (command == "Unit")
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
            dt.Columns.Add("UnitCode", typeof(string));
            dt.Columns.Add("UnitName", typeof(string));

            foreach (var unit in unitList)
            {
                DataRow dr = dt.NewRow();
                dr["UnitCode"] = unit.Key;
                dr["UnitName"] = unit.Value;
                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    private string GenerateLedgerXML(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>");
        sb.Append("<REQUESTDESC><REPORTNAME>All Masters</REPORTNAME>");
        sb.Append("<STATICVARIABLES><SVCURRENTCOMPANY>WLSPL</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>");

        DataTable ledgerTable = dt.Copy();

        // Rename the columns only ONCE, before the loop
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

        // Then loop through the rows to generate XML
        foreach (DataRow row in ledgerTable.Rows)
        {
            string name = "Sundry Creditors";
            string group = HttpUtility.HtmlEncode(row["Group Name"] != DBNull.Value ? row["Group Name"].ToString() : string.Empty);
            string opening = row["Ledger - Opening Balance"] != DBNull.Value ? row["Ledger - Opening Balance"].ToString() : "0";
            string drcr = row["Ledger Opening Balance - Dr/Cr"] != DBNull.Value ? row["Ledger Opening Balance - Dr/Cr"].ToString() : "";


            // Adjust balance if "Cr"
            decimal val;
            if (drcr.Equals("Cr", StringComparison.OrdinalIgnoreCase) && decimal.TryParse(opening, out val))
            {
                opening = (-val).ToString();
            }

            // Append XML for each ledger
            sb.Append("<TALLYMESSAGE><LEDGER NAME=\"").Append(name).Append("\">\n");
            sb.Append("<NAME>").Append(name).Append("</NAME>\n");
            sb.Append("<PARENT>").Append(group).Append("</PARENT>\n");
            sb.Append("<OPENINGBALANCE>").Append(opening).Append("</OPENINGBALANCE>\n");
            sb.Append("<ISCOSTCENTRESON>No</ISCOSTCENTRESON>\n");
            sb.Append("<ISBILLWISEON>Yes</ISBILLWISEON>\n");
            sb.Append("</LEDGER></TALLYMESSAGE>\n");
        }


        sb.Append("</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>");
        return sb.ToString();
    }

    private string GenerateGroupXML(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>");
        sb.Append("<REQUESTDESC><REPORTNAME>All Masters</REPORTNAME>");
        sb.Append("<STATICVARIABLES><SVCURRENTCOMPANY>WLSPL</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>");

        foreach (DataRow row in dt.Rows)
        {
            string name = HttpUtility.HtmlEncode(row["Name"].ToString());
            string parent = HttpUtility.HtmlEncode(row["GroupName"].ToString());

            sb.Append("<TALLYMESSAGE><GROUP NAME=\"" + name + "\">\n");
            sb.Append("<NAME>" + name + "</NAME>\n");
            sb.Append("<PARENT>" + parent + "</PARENT>\n");
            sb.Append("</GROUP></TALLYMESSAGE>");
        }

        sb.Append("</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>");
        return sb.ToString();
    }

    private string GenerateStockItemXML(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>");
        sb.Append("<REQUESTDESC><REPORTNAME>All Masters</REPORTNAME>");
        sb.Append("<STATICVARIABLES><SVCURRENTCOMPANY>WLSPL</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>");

        foreach (DataRow row in dt.Rows)
        {
            string item = HttpUtility.HtmlEncode(row["Name"].ToString());
            string group = HttpUtility.HtmlEncode(row["GroupName"].ToString());
            string unit = HttpUtility.HtmlEncode(row["Units"].ToString());

            sb.Append("<TALLYMESSAGE><STOCKITEM NAME=\"" + item + "\">\n");
            sb.Append("<NAME>" + item + "</NAME>\n");
            sb.Append("<PARENT>" + group + "</PARENT>\n");
            sb.Append("<BASEUNITS>" + unit + "</BASEUNITS>\n");
            sb.Append("</STOCKITEM></TALLYMESSAGE>");

        }

        sb.Append("</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>");
        return sb.ToString();
    }

    private string GenerateUnitXML(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<ENVELOPE><HEADER><TALLYREQUEST>Import Data</TALLYREQUEST></HEADER><BODY><IMPORTDATA>");
        sb.Append("<REQUESTDESC><REPORTNAME>All Masters</REPORTNAME>");
        sb.Append("<STATICVARIABLES><SVCURRENTCOMPANY>WLSPL</SVCURRENTCOMPANY></STATICVARIABLES></REQUESTDESC><REQUESTDATA>");

        foreach (DataRow row in dt.Rows)
        {
            string symbol = HttpUtility.HtmlEncode(row["UnitCode"].ToString());
            sb.Append("<TALLYMESSAGE><UNIT NAME=\"" + symbol + "\">\n");
            sb.Append("<NAME>" + symbol + "</NAME>\n");
            sb.Append("<ISSIMPLEUNIT>Yes</ISSIMPLEUNIT>\n");
            sb.Append("<NUMBEROFDECIMAL>2</NUMBEROFDECIMAL>\n");
            sb.Append("</UNIT></TALLYMESSAGE>");

        }

        sb.Append("</REQUESTDATA></IMPORTDATA></BODY></ENVELOPE>");
        return sb.ToString();
    }

    private string GenerateInstruction(string title, string url)
    {
        string html = "<div style='border:1px solid #ccc;padding:15px;margin-top:20px;'>";
        html += "<h3>Postman Instruction - " + title + "</h3>";
        html += "<p><strong>Method:</strong> POST</p>";
        html += "<p><strong>URL:</strong> <code>" + url + "</code></p>";
        html += "<p><strong>Headers:</strong><br/>Content-Type: text/xml</p>";
        html += "<p><strong>Body:</strong> raw → Select XML format</p>";
        html += "<p>Paste the XML generated from export below into the body, then send the request to import it into Tally.</p>";
        html += "</div>";
        return html;
    }

    public DataTable GetDetails(string action)
    {
        DataTable Dtt = new DataTable();
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
                cmd.Parameters.AddWithValue("@PartyName", DBNull.Value);
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
