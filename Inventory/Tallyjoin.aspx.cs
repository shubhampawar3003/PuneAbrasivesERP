using System;
using System.IO;
using System.Net;
using System.Text;

public partial class Inventory_Tallyjoin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string xml = GenerateTallyXML();
        string tallyUrl = "http://localhost:9000"; // Change if Tally runs on a different machine or port

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tallyUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                Response.ContentType = "text/plain";
                Response.Write("Tally Response:\n\n" + result);
            }
        }
        catch (Exception ex)
        {
            Response.Write("Error sending XML to Tally: " + ex.Message);
        }
    }

    private string GenerateTallyXML()
    {
        return @"<ENVELOPE>
  <HEADER>
    <TALLYREQUEST>Import Data</TALLYREQUEST>
  </HEADER>
  <BODY>
    <IMPORTDATA>
      <REQUESTDESC>
        <REPORTNAME>All Masters</REPORTNAME>
        <STATICVARIABLES>
          <SVCURRENTCOMPANY>Wlspl</SVCURRENTCOMPANY>
        </STATICVARIABLES>
      </REQUESTDESC>
      <REQUESTDATA>
        <TALLYMESSAGE xmlns:UDF=""TallyUDF"">
          <GROUP NAME=""Test Group"" RESERVEDNAME="""">
            <PARENT>Primary</PARENT>
            <ISSUBLEDGER>No</ISSUBLEDGER>
            <AFFECTSGROSSPROFIT>No</AFFECTSGROSSPROFIT>
          </GROUP>
        </TALLYMESSAGE>
      </REQUESTDATA>
    </IMPORTDATA>
  </BODY>
</ENVELOPE>";
    }


    private string SendRequestToTally(string url, string xmlData)
    {
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes(xmlData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml";
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                return sr.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            return "Error: " + ex.Message;
        }
    }


}