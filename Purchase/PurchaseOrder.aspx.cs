using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Globalization;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.Reporting.WebForms;

public partial class Purchase_PurchaseOrder : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable dt = new DataTable();
    public static string sName = "";
    byte[] bytePdf;
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
                fillddlUnit();
                FillddlComponent();
                UpdateHistorymsg = string.Empty; regdate = string.Empty;
                ViewState["RowNo"] = 0;
                dt.Columns.AddRange(new DataColumn[17] { new DataColumn("id"),
                 new DataColumn("Particulars"), new DataColumn("Product"),new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"),new DataColumn("Discount"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Description"),new DataColumn("UOM")
            });

                ViewState["ParticularDetails"] = dt;
                if (Request.QueryString["ID"] != null)
                {

                    string action = Request.QueryString["Action"].ToString();
                    if (action == "OLD")
                    {
                        ViewState["UpdateRowId"] = Decrypt(Request.QueryString["ID"].ToString());
                        GetPurchaseOrderData(ViewState["UpdateRowId"].ToString());
                        sName = txtSupplierName.Text;
                        divcheckamend.Visible = true;
                    }
                    else if (action == "NEW")
                    {
                        ViewState["UpdateRowId"] = Decrypt(Request.QueryString["ID"].ToString());
                        GetPurchaseOrderData(ViewState["UpdateRowId"].ToString());
                        sName = txtSupplierName.Text;
                        divcheckamend.Visible = false;
                    }

                }
                else
                {
                    divcheckamend.Visible = false;
                    txtPONo.Text = Code();
                }
            }
        }
    }

    //private void fillddlpaymentterm()
    //{
    //    SqlDataAdapter adpt = new SqlDataAdapter("select distinct paymentterm from tbl_QuotationMainFooter", con);
    //    DataTable dtpt = new DataTable();
    //    adpt.Fill(dtpt);

    //    if (dtpt.Rows.Count > 0)
    //    {
    //        dtpt.Rows.Add("Specify");
    //        ddlPaymentTerm.DataSource = dtpt;
    //        ddlPaymentTerm.DataValueField = "paymentterm";
    //        ddlPaymentTerm.DataTextField = "paymentterm";
    //        ddlPaymentTerm.DataBind();
    //    }
    //    ddlPaymentTerm.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //}
    private void fillddlUnit()
    {
        //SqlDataAdapter adpt = new SqlDataAdapter("select distinct Unit from tblUnit", con);
        //DataTable dtpt = new DataTable();
        //adpt.Fill(dtpt);

        //if (dtpt.Rows.Count > 0)
        //{
        //    txtUOM.DataSource = dtpt;
        //    txtUOM.DataValueField = "Unit";
        //    txtUOM.DataTextField = "Unit";
        //    txtUOM.DataBind();
        //}
        //txtUOM.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    }
    //private void fillddlFooter()
    //{
    //    SqlDataAdapter adpt = new SqlDataAdapter("select PackingAndForwarding,Transportation,Variation,Delivery,TestCertificate,WeeklyOff,Time,II from tblPOFooter", con);
    //    DataTable dtpt = new DataTable();
    //    adpt.Fill(dtpt);

    //    if (dtpt.Rows.Count > 0)
    //    {
    //        ddlPackingAndForwarding.DataSource = dtpt;
    //        ddlPackingAndForwarding.DataValueField = "PackingAndForwarding";
    //        ddlPackingAndForwarding.DataTextField = "PackingAndForwarding";
    //        ddlPackingAndForwarding.DataBind();

    //        ddlTransportation.DataSource = dtpt;
    //        ddlTransportation.DataValueField = "Transportation";
    //        ddlTransportation.DataTextField = "Transportation";
    //        ddlTransportation.DataBind();

    //        ddlVariation.DataSource = dtpt;
    //        ddlVariation.DataValueField = "Variation";
    //        ddlVariation.DataTextField = "Variation";
    //        ddlVariation.DataBind();

    //        ddlDelivery.DataSource = dtpt;
    //        ddlDelivery.DataValueField = "Delivery";
    //        ddlDelivery.DataTextField = "Delivery";
    //        ddlDelivery.DataBind();

    //        ddlTestCertificate.DataSource = dtpt;
    //        ddlTestCertificate.DataValueField = "TestCertificate";
    //        ddlTestCertificate.DataTextField = "TestCertificate";
    //        ddlTestCertificate.DataBind();

    //        ddlWeeklyOff.DataSource = dtpt;
    //        ddlWeeklyOff.DataValueField = "WeeklyOff";
    //        ddlWeeklyOff.DataTextField = "WeeklyOff";
    //        ddlWeeklyOff.DataBind();

    //        ddlTime.DataSource = dtpt;
    //        ddlTime.DataValueField = "Time";
    //        ddlTime.DataTextField = "Time";
    //        ddlTime.DataBind();

    //        ddlII.DataSource = dtpt;
    //        ddlII.DataValueField = "II";
    //        ddlII.DataTextField = "II";
    //        ddlII.DataBind();
    //    }
    //    ddlPackingAndForwarding.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlTransportation.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlVariation.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlDelivery.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlTestCertificate.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlWeeklyOff.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlTime.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //    ddlII.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    //}

    static string regdate = string.Empty;
    protected void GetPurchaseOrderData(string id)
    {
        string query1 = string.Empty;
        query1 = @"select * from tblPurchaseOrderHdr where Id='" + id + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtSupplierName.Text = dt.Rows[0]["SupplierName"].ToString();

            BindKindAtt();
            BindEmailId();
            txtPONo.Text = dt.Rows[0]["PONo"].ToString();
            ddlKindAtt.Text = dt.Rows[0]["KindAtt"].ToString();
            txtPodate.Text = dt.Rows[0]["PODate"].ToString();
            ddlMode.Text = dt.Rows[0]["Mode"].ToString();
            txtDeliverydate.Text = dt.Rows[0]["DeliveryDate"].ToString();
            txtReferQuotation.Text = dt.Rows[0]["ReferQuotation"].ToString();
            txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();
            ddlOrderCloseMode.Text = dt.Rows[0]["OrderCloseMode"].ToString();
            hdnfileData.Value = dt.Rows[0]["RefDocuments"].ToString();
            //if (dt.Rows[0]["RefDocuments"].ToString() != "")
            //{
            //    spnFileUploadData.InnerText = "File Already Exsist, if you can update then update it.";
            //}
            //else
            //{
            //    spnFileUploadData.InnerText = "File Not Found";
            //}

            //ddlPaymentTerm.Text = dt.Rows[0]["PaymentTerm"].ToString();
            //ddlPackingAndForwarding.Text = dt.Rows[0]["PackingAndForwarding"].ToString();
            //ddlTransportation.Text = dt.Rows[0]["Transportation"].ToString();
            //ddlVariation.Text = dt.Rows[0]["Variation"].ToString();
            //ddlDelivery.Text = dt.Rows[0]["Delivery"].ToString();
            //ddlTestCertificate.Text = dt.Rows[0]["TestCertificate"].ToString();
            //ddlWeeklyOff.Text = dt.Rows[0]["WeeklyOff"].ToString();
            //ddlTime.Text = dt.Rows[0]["Time"].ToString();
            //ddlII.Text = dt.Rows[0]["II"].ToString();

            txtTCharge.Text = dt.Rows[0]["TransportationCharges"].ToString();
            txtTDescription.Text = dt.Rows[0]["TransportationDescription"].ToString();
            txtTCGSTPer.Text = dt.Rows[0]["TCGSTPer"].ToString();
            txtTCGSTamt.Text = dt.Rows[0]["TCGSTAmt"].ToString();
            txtTSGSTPer.Text = dt.Rows[0]["TSGSTPer"].ToString();
            txtTSGSTamt.Text = dt.Rows[0]["TSGSTAmt"].ToString();
            txtTIGSTPer.Text = dt.Rows[0]["TIGSTPer"].ToString();
            txtTIGSTamt.Text = dt.Rows[0]["TIGSTAmt"].ToString();
            txtTCost.Text = dt.Rows[0]["TotalCost"].ToString();
            getParticularsdts(id);
            txtPayment.Text = dt.Rows[0]["Payment"].ToString();
            txtTransport.Text = dt.Rows[0]["Transport"].ToString();
            txtDeliveryTime.Text = dt.Rows[0]["DeliveryTime"].ToString();
            txtPacking.Text = dt.Rows[0]["Packing"].ToString();
            txtTaxs.Text = dt.Rows[0]["Taxs"].ToString();

        }
    }

    protected void getParticularsdts(string id)
    {
        DataTable Dtproduct = new DataTable();
        SqlDataAdapter daa = new SqlDataAdapter(@"SELECT  [Id]
      ,[Particulars]
      ,[Product]
      ,[HSN]
      ,[Qty]
      ,[Rate]
      ,[Amount]
      ,[CGSTPer]
      ,[CGSTAmt]
      ,[SGSTPer]
      ,[SGSTAmt]
      ,[IGSTPer]
      ,[IGSTAmt]
      ,[GrandTotal]
      ,[Description]
      ,[Discount]
      ,[UOM]
  FROM [tblPurchaseOrderDtls] where HeaderID='" + id + "'", con);
        daa.Fill(Dtproduct);
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;

        DataTable dt = ViewState["ParticularDetails"] as DataTable;

        if (Dtproduct.Rows.Count > 0)
        {
            for (int i = 0; i < Dtproduct.Rows.Count; i++)
            {
                dt.Rows.Add(ViewState["RowNo"], Dtproduct.Rows[i]["Particulars"].ToString(), Dtproduct.Rows[i]["Product"].ToString(), Dtproduct.Rows[i]["HSN"].ToString(), Dtproduct.Rows[i]["Qty"].ToString(),
                    Dtproduct.Rows[i]["Rate"].ToString(), Dtproduct.Rows[i]["Discount"].ToString(), Dtproduct.Rows[i]["Amount"].ToString(), Dtproduct.Rows[i]["CGSTPer"].ToString(), Dtproduct.Rows[i]["CGSTAmt"].ToString(),
                    Dtproduct.Rows[i]["SGSTPer"].ToString(), Dtproduct.Rows[i]["SGSTAmt"].ToString(), Dtproduct.Rows[i]["IGSTPer"].ToString(), Dtproduct.Rows[i]["IGSTAmt"].ToString(),
                    Dtproduct.Rows[i]["GrandTotal"].ToString(), Dtproduct.Rows[i]["Description"].ToString(), Dtproduct.Rows[i]["UOM"].ToString());
                ViewState["ParticularDetails"] = dt;
            }
        }
        dgvParticularsDetails.DataSource = dt;
        dgvParticularsDetails.DataBind();
    }

    static string UpdateHistorymsg = string.Empty;

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

    protected string GenerateComCode()
    {

        string invoiceno;
        DateTime date = DateTime.Now;
        string currentyeaar = date.ToString();

        string FinYear = null;

        if (DateTime.Today.Month > 3)
        {
            //FinYear = DateTime.Today.Year.ToString();
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();
        }
        string previousyear = (Convert.ToDecimal(FinYear) - 1).ToString();

        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tblPurchaseOrderHdr]", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {

            int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
            invoiceno = previousyear.ToString() + "-" + FinYear + "/" + (maxid + 1).ToString();
            if (maxid < 9)
            {
                invoiceno = previousyear.ToString() + "-" + FinYear + "/" + "000" + (maxid + 1).ToString();
            }
            else if (maxid <= 100)
            {
                invoiceno = previousyear.ToString() + "-" + FinYear + "/" + "00" + (maxid + 1).ToString();
            }
        }
        else
        {
            invoiceno = string.Empty;
        }
        return invoiceno;
    }

    protected string Code()
    {
        string FinYear = null;
        string FinFullYear = null;
        if (DateTime.Today.Month > 3)
        {
            FinYear = DateTime.Today.AddYears(1).ToString("yy");
            FinFullYear = DateTime.Today.AddYears(1).ToString("yyyy");
        }
        else
        {
            var finYear = DateTime.Today.AddYears(1).ToString("yy");
            FinYear = (Convert.ToInt32(finYear) - 1).ToString();

            var finfYear = DateTime.Today.AddYears(1).ToString("yyyy");
            FinFullYear = (Convert.ToInt32(finfYear) - 1).ToString();
        }
        string previousyear = (Convert.ToDecimal(FinFullYear) - 1).ToString();
        string strInvoiceNumber = "";
        string fY = previousyear.ToString() + "-" + FinYear;
        string strSelect = @"SELECT TOP 1
    LEFT(
        SUBSTRING(PONo, CHARINDEX('/', PONo) + 1, LEN(PONo)),
        PATINDEX('%[^0-9]%', SUBSTRING(PONo, CHARINDEX('/', PONo) + 1, LEN(PONo) + 1)) - 1
    ) AS maxno
FROM tblPurchaseOrderHdr
WHERE CHARINDEX('/', PONo) > 0 AND  PONo like  '%" + fY + "%' order by id desc";
        // string strSelect = @"SELECT TOP 1 ISNULL(MAX(PONo), '') AS maxno FROM tblPurchaseOrderHdr where PONo like '%" + fY + "%' ORDER BY ID DESC";
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;

        cmd.CommandType = CommandType.Text;
        cmd.CommandText = strSelect;
        con.Open();
        string result = cmd.ExecuteScalar().ToString();
        // string result = "";
        con.Close();
        if (result != "")
        {
            int numbervalue = Convert.ToInt32(result);
            numbervalue = numbervalue + 1;
            strInvoiceNumber = fY + "/" + result.Substring(0, result.IndexOf("/") + 1) + "" + numbervalue.ToString("00") + "-R0";
        }
        else
        {
            strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + "01";
        }
        return strInvoiceNumber;
    }

    //Revised quotation number genrate
    public static string Revicecode(string number)
    {
        string total = string.Empty;
        string Lastresult = number.Substring(number.Length - 2);
        string Firstresult = number.Substring(0, number.Length - 2);
        if (Lastresult == "A0")
        {
            total = Firstresult + "A1";
        }
        if (Lastresult == "A1")
        {
            total = Firstresult + "A2";
        }
        if (Lastresult == "A2")
        {
            total = Firstresult + "A3";
        }
        if (Lastresult == "A3")
        {
            total = Firstresult + "A4";
        }
        if (Lastresult == "A4")
        {
            total = Firstresult + "A5";
        }
        if (Lastresult == "A5")
        {
            total = Firstresult + "A6";
        }
        if (Lastresult == "A6")
        {
            total = Firstresult + "A7";
        }
        return total;
    }

    protected void btnadd_Click(object sender, EventArgs e)
    {
        try
        {
            string subject = string.Empty;
            string action = string.Empty;
            string PONo = string.Empty;
            #region Insert
            if (dgvParticularsDetails.Rows.Count < 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please Add Particulars Details !!');", true);
            }

            if (Request.QueryString["ID"] != null)
            {
                action = Request.QueryString["Action"].ToString();
                if (chkAmended.Checked == false && action == "OLD")
                {
                    PONo = txtPONo.Text;
                    Addheaderdata("Update", PONo);
                    DeleteData();
                    ProductData();
                    subject = "Updated PO from Pune Abrasieves Pvt. Ltd.";

                }
                else if (action == "NEW")
                {
                    PONo = Code();
                    Addheaderdata("Insert", PONo);
                    ProductData();
                    subject = "Created PO from Pune Abrasieves Pvt. Ltd.";

                }
                else
                {
                    PONo = Revicecode(txtPONo.Text);
                    Addheaderdata("Insert", PONo);
                    ProductData();
                    subject = "Created PO from Pune Abrasieves Pvt. Ltd.";


                }

            }
            else
            {
                PONo = Code();
                Addheaderdata("Insert", PONo);
                ProductData();
                subject = "Created PO from Pune Abrasieves Pvt. Ltd.";

            }

            if (IsSedndMail.Checked == true)
            {
                int idd = Convert.ToInt32(ViewState["UpdateRowId"].ToString());
                Send_Mail(idd, subject);
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Successfully Added !!');window.location.href='PurchaseOrderList.aspx';", true);
            #endregion Insert
        }
        catch { }



    }

    public void Addheaderdata(string action, string PONo)
    {
        SqlCommand cmd = new SqlCommand("SP_PurchaseOrder", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Action", action);
        if (ViewState["UpdateRowId"] != null)
        {
            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(ViewState["UpdateRowId"].ToString()));
        }
        cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
        cmd.Parameters.AddWithValue("@PONo", PONo);
        if (!string.IsNullOrWhiteSpace(txtPodate.Text))
        {
            DateTime PODate = Convert.ToDateTime(txtPodate.Text.ToString(), System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);

            txtPodate.Text = PODate.ToString("yyyy-MM-dd");
            cmd.Parameters.AddWithValue("@PODate", txtPodate.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@PODate", DBNull.Value);
        }
        cmd.Parameters.AddWithValue("@Mode", ddlMode.Text.Trim());
        DateTime DeliveryDate = Convert.ToDateTime(txtDeliverydate.Text.ToString(), System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
        txtDeliverydate.Text = DeliveryDate.ToString("yyyy-MM-dd");
        cmd.Parameters.AddWithValue("@DeliveryDate", txtDeliverydate.Text.Trim());
        cmd.Parameters.AddWithValue("@ReferQuotation", txtReferQuotation.Text.Trim());
        cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
        cmd.Parameters.AddWithValue("@OrderCloseMode", ddlOrderCloseMode.Text.Trim());
        cmd.Parameters.AddWithValue("@KindAtt", ddlKindAtt.Text.Trim());
        cmd.Parameters.AddWithValue("@Payment", txtPayment.Text);
        cmd.Parameters.AddWithValue("@Transport", txtTransport.Text);
        cmd.Parameters.AddWithValue("@DeliveryTime", txtDeliveryTime.Text);
        cmd.Parameters.AddWithValue("@Packing", txtPacking.Text);
        cmd.Parameters.AddWithValue("@Taxs", txtTaxs.Text);

        cmd.Parameters.AddWithValue("@GrandTotal", hdnGrandtotal.Value);
        cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());

        //17 March 2022
        cmd.Parameters.AddWithValue("@TransportationCharges", txtTCharge.Text.Trim());
        cmd.Parameters.AddWithValue("@TransportationDescription", txtTDescription.Text.Trim());
        cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text.Trim());
        cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text.Trim());
        cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text.Trim());
        cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text.Trim());
        cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text.Trim());
        int a = 0;
        cmd.Connection.Open();
        a = cmd.ExecuteNonQuery();
        cmd.Connection.Close();
    }
    public void DeleteData()
    {
        SqlCommand cmddelete = new SqlCommand("delete from tblPurchaseOrderDtls where HeaderID='" + Convert.ToInt32(ViewState["UpdateRowId"].ToString()) + "'", con);
        con.Open();
        cmddelete.ExecuteNonQuery();
        con.Close();
    }
    public void ProductData()
    {
        int MaxId = 0;
        if (Request.QueryString["ID"] != null)
        {
            string action = Request.QueryString["Action"].ToString();
            if (action == "NEW" || action == "OLD")
            {
                if (chkAmended.Checked == true || action == "NEW")
                {
                    SqlCommand cmdmax = new SqlCommand("select MAX(Id) as MAxID from tblPurchaseOrderHdr", con);
                    con.Open();
                    Object mx = cmdmax.ExecuteScalar();
                    con.Close();
                    MaxId = Convert.ToInt32(mx.ToString());
                }
                else
                {
                    MaxId = Convert.ToInt32(ViewState["UpdateRowId"].ToString());
                }

            }
            else
            {
                MaxId = Convert.ToInt32(ViewState["UpdateRowId"].ToString());
            }

        }
        else
        {
            SqlCommand cmdmax = new SqlCommand("select MAX(Id) as MAxID from tblPurchaseOrderHdr", con);
            con.Open();
            Object mx = cmdmax.ExecuteScalar();
            con.Close();
            MaxId = Convert.ToInt32(mx.ToString());
        }
        foreach (GridViewRow row in dgvParticularsDetails.Rows)
        {
            string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
            string Product = ((Label)row.FindControl("lblProduct")).Text;
            string HSN = ((Label)row.FindControl("lblHSN")).Text;
            string Qty = ((Label)row.FindControl("lblQty")).Text;
            string Rate = ((Label)row.FindControl("lblRate")).Text;
            string Discount = ((Label)row.FindControl("lblDiscount")).Text;
            string Amount = ((Label)row.FindControl("lblAmount")).Text;
            string CGSTPer = ((Label)row.FindControl("lblCGSTPer")).Text;
            string CGSTAmt = ((Label)row.FindControl("lblCGSTAmt")).Text;
            string SGSTPer = ((Label)row.FindControl("lblSGSTPer")).Text;
            string SGSTAmt = ((Label)row.FindControl("lblSGSTAmt")).Text;
            string IGSTPer = ((Label)row.FindControl("lblIGSTPer")).Text;
            string IGSTAmt = ((Label)row.FindControl("lblIGSTAmt")).Text;
            string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;
            string Description = ((Label)row.FindControl("lblDescription")).Text;
            string UOM = ((Label)row.FindControl("lblUOM")).Text;

            SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblPurchaseOrderDtls([HeaderID],[Particulars],[Product],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[GrandTotal],[Discount],[Description],[UOM]) 
                        VALUES(" + MaxId.ToString() + ",'" + Particulars + "','" + Product + "','" + HSN + "','" + Qty + "'," +
             "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
             "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + Discount + "','" + Description + "','" + UOM + "')", con);
            con.Open();
            cmdParticulardata.ExecuteNonQuery();
            con.Close();
        }
    }

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseOrder.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierList(string prefixText, int count)
    {
        return AutoFillSupplierName(prefixText);
    }

    public static List<string> AutoFillSupplierName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Vendorname] from tbl_VendorMaster where " + "Vendorname like '%'+ @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> SupplierNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        SupplierNames.Add(sdr["Vendorname"].ToString());
                    }
                }
                con.Close();
                return SupplierNames;
            }
        }
    }

    protected void txtSupplierName_TextChanged(object sender, EventArgs e)
    {
        BindKindAtt();

        BindEmailId();

        sName = txtSupplierName.Text;
    }

    protected void BindEmailId()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT [EmailID] FROM [tbl_VendorMaster] where Vendorname='" + txtSupplierName.Text.Trim() + "' ", con);
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

    //protected void BindParticular()
    //{
    //string com = "SELECT ItemName FROM tblItemMaster";
    //SqlDataAdapter adpt = new SqlDataAdapter(com, con);
    //DataTable dt = new DataTable();
    //adpt.Fill(dt);
    //ddlparticular.DataSource = dt;
    //ddlparticular.DataBind();
    //ddlparticular.DataTextField = "ItemName";
    //ddlparticular.DataValueField = "ItemName";
    //ddlparticular.DataBind();

    //ddlparticular.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select Particular--", "0"));
    //}

    protected void BindKindAtt()
    {
        try
        {
            string com = "SELECT * FROM tbl_vendormaster where Vendorname='" + txtSupplierName.Text.Trim() + "'";
            SqlDataAdapter adpt = new SqlDataAdapter(com, con);
            DataTable dt = new DataTable();
            adpt.Fill(dt);
            ddlKindAtt.DataSource = dt;
            ddlKindAtt.DataBind();
            ddlKindAtt.DataTextField = "Ownername";
            ddlKindAtt.DataValueField = "Ownername";
            ddlKindAtt.DataBind();

            ddlKindAtt.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select Kind. Att--", "0"));
        }
        catch
        {

        }
    }

    protected void Insert(object sender, EventArgs e)
    {
        if (txtQty.Text == "" || ddlcomponent.SelectedItem.Text == "" || txtTotalamt.Text == "")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please fill All Required fields !!!');", true);
        }
        else
        {
            Show_Grid();
        }
    }

    private void Show_Grid()
    {
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;
        DataTable dt = (DataTable)ViewState["ParticularDetails"];

        dt.Rows.Add(ViewState["RowNo"], ddlcomponent.SelectedItem.Text, txtProduct.Text, txtHSN.Text, txtQty.Text, txtRate.Text, txtDisc.Text, txtAmountt.Text, CGSTPer.Text, CGSTAmt.Text, SGSTPer.Text, SGSTAmt.Text, IGSTPer.Text, IGSTAmt.Text, txtTotalamt.Text, txtDescription.Text, txtUOM.Text);
        ViewState["ParticularDetails"] = dt;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();

        ddlcomponent.SelectedItem.Text = string.Empty;
        txtQty.Text = string.Empty;
        txtHSN.Text = string.Empty;
        txtRate.Text = string.Empty;
        txtDisc.Text = string.Empty;
        txtAmountt.Text = string.Empty;
        CGSTPer.Text = string.Empty;
        CGSTAmt.Text = string.Empty;
        SGSTPer.Text = string.Empty;
        SGSTAmt.Text = string.Empty;
        IGSTPer.Text = string.Empty;
        IGSTAmt.Text = string.Empty;
        txtTotalamt.Text = string.Empty;
        txtDescription.Text = string.Empty;
        //txtUOM.Text = string.Empty;
    }

    protected void dgvParticularsDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void dgvParticularsDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvParticularsDetails.EditIndex = e.NewEditIndex;
        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnUpdate_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string Product = ((TextBox)row.FindControl("txtProduct")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((TextBox)row.FindControl("txtRate")).Text;

        string Discount = ((TextBox)row.FindControl("txtDiscount")).Text;

        string Amount = ((Label)row.FindControl("lblAmount")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGSTAmt")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGSTAmt")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGSTAmt")).Text;
        string TotalAmount = ((TextBox)row.FindControl("txtTotalAmount")).Text;
        string Description = ((TextBox)row.FindControl("txttblDescription")).Text;
        string UOM = ((TextBox)row.FindControl("txtUOM")).Text;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;

        Dt.Rows[row.RowIndex]["Particulars"] = Particulars;
        Dt.Rows[row.RowIndex]["Product"] = Product;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Qty"] = Qty;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Discount"] = Discount;
        Dt.Rows[row.RowIndex]["Amount"] = Amount;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["TotalAmount"] = TotalAmount;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["UOM"] = UOM;

        Dt.AcceptChanges();

        ViewState["ParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();

        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable dt = ViewState["ParticularDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["ParticularDetails"] = dt;
        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void lnkCancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;
        dgvParticularsDetails.EditIndex = -1;

        ViewState["ParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

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

    protected void ddlcomponent_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_ComponentMaster] WHERE ComponentName='" + ddlcomponent.SelectedItem.Text + "' ", Cls_Main.Conn);
        // SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_MachineMaster] WHERE Productname='" + ddlProduct.SelectedItem.Text + "'", Cls_Main.Conn);
        DataTable Dt = new DataTable();
        Da.Fill(Dt);
        if (Dt.Rows.Count > 0)
        {

            txtHSN.Text = Dt.Rows[0]["HSN"].ToString() == "" ? "0" : Dt.Rows[0]["HSN"].ToString();
            txtRate.Text = Dt.Rows[0]["Price"].ToString() == "" ? "0" : Dt.Rows[0]["Price"].ToString();
            txtUOM.Text = Dt.Rows[0]["Unit"].ToString() == "" ? "0" : Dt.Rows[0]["Unit"].ToString();
            txtProduct.Text = ddlcomponent.SelectedItem.Text;

        }
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetUOMList(string prefixText, int count)
    {
        return AutoFilUOM(prefixText);
    }

    public static List<string> AutoFilUOM(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [Unit] from tbl_ProductMaster where " + "Unit like '%' + @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> StorageUnit = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        StorageUnit.Add(sdr["Unit"].ToString());
                    }
                }
                con.Close();
                return StorageUnit;
            }
        }
    }

    private void GST_Calculation()
    {
        var TotalAmt = Convert.ToDecimal(txtQty.Text.Trim()) * Convert.ToDecimal(txtRate.Text.Trim());

        decimal disc;
        if (string.IsNullOrEmpty(txtDisc.Text))
        {
            disc = 0;
            txtAmountt.Text = TotalAmt.ToString("0.00", CultureInfo.InvariantCulture);
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(TotalAmt);
            decimal Val2 = Convert.ToDecimal(txtDisc.Text);
            disc = (Val1 * Val2 / 100);
            var result = Val1 - disc;
            txtAmountt.Text = result.ToString("0.00", CultureInfo.InvariantCulture);
        }



        decimal CGST;
        if (string.IsNullOrEmpty(CGSTPer.Text))
        {
            CGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtAmountt.Text);
            decimal Val2 = Convert.ToDecimal(CGSTPer.Text);
            SGSTPer.Text = CGSTPer.Text;

            CGST = (Val1 * Val2 / 100);
        }
        CGSTAmt.Text = CGST.ToString("0.00", CultureInfo.InvariantCulture);

        decimal SGST;
        if (string.IsNullOrEmpty(SGSTPer.Text))
        {
            SGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtAmountt.Text);
            decimal Val2 = Convert.ToDecimal(SGSTPer.Text);

            SGST = (Val1 * Val2 / 100);
        }
        SGSTAmt.Text = SGST.ToString("0.00", CultureInfo.InvariantCulture);


        decimal IGST;
        if (string.IsNullOrEmpty(IGSTPer.Text))
        {
            IGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtAmountt.Text);
            decimal Val2 = Convert.ToDecimal(IGSTPer.Text);

            IGST = (Val1 * Val2 / 100);
        }
        IGSTAmt.Text = IGST.ToString("0.00", CultureInfo.InvariantCulture);

        var GSTTotal = CGST + SGST + IGST;

        var Finalresult = Convert.ToDecimal(txtAmountt.Text) + GSTTotal;

        txtTotalamt.Text = Finalresult.ToString("0.00", CultureInfo.InvariantCulture);
    }

    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void IGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();

        if (IGSTPer.Text == "" || IGSTPer.Text == "0")
        {
            SGSTPer.Enabled = true;
            CGSTPer.Enabled = true;
            SGSTPer.Text = "0";
            CGSTPer.Text = "0";
        }
        else
        {
            SGSTPer.Enabled = false;
            CGSTPer.Enabled = false;
            SGSTPer.Text = "0";
            CGSTPer.Text = "0";
        }
    }

    protected void SGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();

        if (SGSTPer.Text == "" || SGSTPer.Text == "0")
        {
            IGSTPer.Enabled = true;
            IGSTPer.Text = "0";
        }
        else
        {
            IGSTPer.Enabled = false;
            IGSTPer.Text = "0";
        }
    }

    protected void CGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();

        if (CGSTPer.Text == "" || CGSTPer.Text == "0")
        {
            IGSTPer.Enabled = true;
            IGSTPer.Text = "0";
        }
        else
        {
            IGSTPer.Enabled = false;
            IGSTPer.Text = "0";
        }
    }

    private void GRID_GST_Calculation(GridViewRow row)
    {
        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        TextBox Rate = ((TextBox)row.FindControl("txtRate"));
        TextBox Discount = ((TextBox)row.FindControl("txtDiscount"));
        Label Amount = ((Label)row.FindControl("lblAmount"));
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        TextBox CGSTAmt = (TextBox)row.FindControl("txtCGSTAmt");
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        TextBox SGSTAmt = (TextBox)row.FindControl("txtSGSTAmt");
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        TextBox IGSTAmt = (TextBox)row.FindControl("txtIGSTAmt");
        TextBox TotalAmount = (TextBox)row.FindControl("txtTotalAmount");

        var totalamt = Convert.ToDecimal(Qty) * Convert.ToDecimal(Rate.Text);
        string Tot = "";

        decimal disc;
        if (string.IsNullOrEmpty(Discount.Text))
        {
            disc = 0;
            Amount.Text = totalamt.ToString("0.00", CultureInfo.InvariantCulture);
        }
        else
        {
            decimal val1 = Convert.ToDecimal(totalamt);
            decimal val2 = Convert.ToDecimal(Discount.Text);

            disc = (val1 * val2 / 100);
            var result = val1 - disc;
            Amount.Text = result.ToString("0.00", CultureInfo.InvariantCulture);
        }


        decimal Vcgst;
        if (string.IsNullOrEmpty(CGSTAmt.Text))
        {
            Vcgst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(Amount.Text);
            decimal val2 = Convert.ToDecimal(CGSTPer);

            Vcgst = (val1 * val2 / 100);
        }
        CGSTAmt.Text = Vcgst.ToString("0.00", CultureInfo.InvariantCulture);

        decimal Vsgst;
        if (string.IsNullOrEmpty(SGSTAmt.Text))
        {
            Vsgst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(Amount.Text);
            decimal val2 = Convert.ToDecimal(CGSTPer);

            Vsgst = (val1 * val2 / 100);
        }
        SGSTAmt.Text = Vsgst.ToString("0.00", CultureInfo.InvariantCulture);

        decimal Vigst;
        if (string.IsNullOrEmpty(IGSTAmt.Text))
        {
            Vigst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(Amount.Text);
            decimal val2 = Convert.ToDecimal(IGSTPer);

            Vigst = (val1 * val2 / 100);
        }
        IGSTAmt.Text = Vigst.ToString("0.00", CultureInfo.InvariantCulture);

        var GSTTotal = Vcgst + Vsgst + Vigst;

        var taxamt = Convert.ToDecimal(Amount.Text) + GSTTotal;

        TotalAmount.Text = taxamt.ToString("0.00", CultureInfo.InvariantCulture);
    }

    protected void txtQty_TextChanged1(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    decimal Totalamt = 0;
    protected void dgvParticularsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txts = (e.Row.FindControl("txtTotalAmount") as TextBox);

            if (txts == null)
            {
                Totalamt += Convert.ToDecimal((e.Row.FindControl("lblTotalAmount") as Label).Text);
                hdnGrandtotal.Value = Totalamt.ToString();               
            }
            else
            {
                Totalamt += Convert.ToDecimal((e.Row.FindControl("txtTotalAmount") as TextBox).Text);
                hdnGrandtotal.Value = Totalamt.ToString();

            }
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            (e.Row.FindControl("lbltotal") as Label).Text = Totalamt.ToString();
        }
    }

    //Send Mail
    /////pdf function
    byte[] bytefile;
    protected void Send_Mail(int? Id, string Subject)
    {

        string strMessage = "Hello " + txtSupplierName.Text.Trim() + "<br/>" +


                        "Greetings From " + "<strong>Pune Abrasive Pvt. Ltd.<strong>" + "<br/>" +
                        "We sent you an Purchase Order Invoice." + txtPONo.Text.Trim() + "/" + txtPodate.Text.Trim() + ".pdf" + "<br/>" +

                         "We Look Foward to Conducting Future Business with you." + "<br/>" +

                        "Kind Regards," + "<br/>" +
                        "<strong>Pune Abrasive Pvt. Ltd.<strong>";
        string pdfname = "Purchase Order - " + txtPONo.Text.Trim() + "/" + txtPodate.Text.Trim() + ".pdf";
        string fileName = txtPONo.Text + "-" + "QuotationInvoice.pdf";
        MailMessage mm = new MailMessage();
        // mm.From = new MailAddress(fromMailID);
        string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
        mm.Subject = "Purchase Order Invoice";
        //mm.To.Add("shubhpawar59@gmail.com");
        mm.To.Add(lblEmailID.Text);
        mm.CC.Add("girish.kulkarni@puneabrasives.com");
        mm.CC.Add("virendra.sud@puneabrasives.com");
        mm.CC.Add("accounts@puneabrasives.com");
        mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
        Report(Id);
        if (bytePdf != null)
        {
            Stream stream = new MemoryStream(bytePdf);
            Attachment aa = new Attachment(stream, fileName);
            mm.Attachments.Add(aa);
        }
        StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
        string readFile = reader.ReadToEnd();
        string myString = "";
        myString = readFile;

        string multilineText = strMessage;
        string formattedText = multilineText.Replace("\n", "<br />");

        myString = myString.Replace("$Comment$", formattedText);

        mm.Body = myString.ToString();
        mm.IsBodyHtml = true;
        //mm.From = new MailAddress("girish.kulkarni@puneabrasives.com", fromMailID);
        mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);

        // Set the "Reply-To" header to indicate the desired display address
        mm.ReplyToList.Add(new MailAddress(fromMailID));

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

    public void Report(Int32? PONO)
    {
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_GetTableDetails]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetPOReport");
                    cmd.Parameters.AddWithValue("@PONO", PONO);

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
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\PurchaseOrder.rdlc";
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    bytePdf = null;
                    bytePdf = bytePdfRep;
                    Response.ClearContent();
                    Response.ClearHeaders();
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.Reset();

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static string ConvertNumbertoWords(int number)
    {
        if (number == 0)
            return "ZERO";
        if (number < 0)
            return "minus " + ConvertNumbertoWords(Math.Abs(number));
        string words = "";
        if ((number / 1000000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000000) + " MILLION ";
            number %= 1000000;
        }
        if ((number / 1000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000) + " THOUSAND ";
            number %= 1000;
        }
        if ((number / 100) > 0)
        {
            words += ConvertNumbertoWords(number / 100) + " HUNDRED ";
            number %= 100;
        }
        if (number > 0)
        {
            if (words != "")
                words += "AND ";
            var unitsMap = new[] { "ZERO", "ONE", "TWO", "THREE", "FOUR", "FIVE", "SIX", "SEVEN", "EIGHT", "NINE", "TEN", "ELEVEN", "TWELVE", "THIRTEEN", "FOURTEEN", "FIFTEEN", "SIXTEEN", "SEVENTEEN", "EIGHTEEN", "NINETEEN" };
            var tensMap = new[] { "ZERO", "TEN", "TWENTY", "THIRTY", "FORTY", "FIFTY", "SIXTY", "SEVENTY", "EIGHTY", "NINETY" };

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


    protected void txtDisc_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }
    //17 march 2022
    private void Transportation_Calculation()
    {
        var TotalAmt = Convert.ToDecimal(txtTCharge.Text.Trim());

        decimal CGST;
        if (string.IsNullOrEmpty(txtTCGSTPer.Text))
        {
            CGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtTCharge.Text.Trim());
            decimal Val2 = Convert.ToDecimal(txtTCGSTPer.Text);

            CGST = (Val1 * Val2 / 100);
        }
        txtTCGSTamt.Text = CGST.ToString("0.00", CultureInfo.InvariantCulture);

        decimal SGST;
        if (string.IsNullOrEmpty(txtTSGSTPer.Text))
        {
            SGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtTCharge.Text);
            decimal Val2 = Convert.ToDecimal(txtTSGSTPer.Text);

            SGST = (Val1 * Val2 / 100);
        }
        txtTSGSTamt.Text = SGST.ToString("0.00", CultureInfo.InvariantCulture);


        decimal IGST;
        if (string.IsNullOrEmpty(txtTIGSTPer.Text))
        {
            IGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtTCharge.Text);
            decimal Val2 = Convert.ToDecimal(txtTIGSTPer.Text);

            IGST = (Val1 * Val2 / 100);
        }
        txtTIGSTamt.Text = IGST.ToString("0.00", CultureInfo.InvariantCulture);

        var GSTTotal = CGST + SGST + IGST;

        var Finalresult = Convert.ToDecimal(txtTCharge.Text) + GSTTotal;

        txtTCost.Text = Finalresult.ToString("0.00", CultureInfo.InvariantCulture);


    }

    protected void txtTCharge_TextChanged(object sender, EventArgs e)
    {
        Transportation_Calculation();
    }

    protected void txtTCGSTPer_TextChanged(object sender, EventArgs e)
    {
        if (txtTCGSTPer.Text != "0")
        {
            txtTIGSTPer.Enabled = false;
            txtTSGSTPer.Text = txtTCGSTPer.Text;
        }
        else
        {
            txtTIGSTPer.Enabled = true;
            txtTSGSTPer.Text = "0";
        }


        Transportation_Calculation();
    }

    protected void txtTSGSTPer_TextChanged(object sender, EventArgs e)
    {
        if (txtTSGSTPer.Text != "0")
        {
            txtTIGSTPer.Enabled = false;
        }
        else
        {
            txtTIGSTPer.Enabled = true;
        }
        Transportation_Calculation();

        var TotalGrand = Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCost.Text);
        hdnGrandtotal.Value = TotalGrand.ToString("0.00", CultureInfo.InvariantCulture);
    }

    protected void txtTIGSTPer_TextChanged(object sender, EventArgs e)
    {
        if (txtTIGSTPer.Text != "0")
        {
            txtTSGSTPer.Enabled = false;
            txtTCGSTPer.Enabled = false;
        }
        else
        {
            txtTSGSTPer.Enabled = true;
            txtTCGSTPer.Enabled = true;
        }
        Transportation_Calculation();

        var TotalGrand = Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCost.Text);
        hdnGrandtotal.Value = TotalGrand.ToString("0.00", CultureInfo.InvariantCulture);
    }
    protected void txtDiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }
    protected void txtDeliverydate_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(txtPodate.Text) && !string.IsNullOrWhiteSpace(txtDeliverydate.Text))
        {
            DateTime PoDate = Convert.ToDateTime(txtPodate.Text);
            DateTime Ddate = Convert.ToDateTime(txtDeliverydate.Text);

            if (PoDate > Ddate)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('PO Date is greater than Delivery Date...Please Choose Correct Date.');", true);
                btnadd.Enabled = false;
            }
            else
            {
                btnadd.Enabled = true;
            }
        }
        //DateTime fromdate = DateTime.Parse(Convert.ToDateTime(txtBilldate.Text).ToShortDateString());
        //DateTime PoDate = Convert.ToDateTime(txtPodate.Text);
        //DateTime Ddate = Convert.ToDateTime(txtDeliverydate.Text);
        ////DateTime todate = DateTime.Parse(Convert.ToDateTime(txtDOR.Text).ToShortDateString());
        //if (PoDate > Ddate)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('PO Date is greater than Delivery Date...Please Choose Correct Date.');", true);
        //    btnadd.Enabled = false;
        //}
        //else
        //{
        //    btnadd.Enabled = true;
        //}
    }
    protected void txtRate_TextChanged1(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Purchase/PurchaseOrderList.aspx");
    }
}