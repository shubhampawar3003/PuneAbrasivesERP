
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
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;


public partial class Account_ApprovedInvoiceList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
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

                FillGrid();
            }
        }
    }

    //Fill GridView
    private void FillGrid()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_Reports", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetInvoiceListForAccount");
            cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@Invoiceno", txtInvoiceNo.Text);
            cmd.Parameters.AddWithValue("@PageSize", Convert.ToInt32(ddlPageSize.SelectedValue));
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Columns.Count > 0)
            {
                GVInvoice.DataSource = dt;
                GVInvoice.DataBind();
            }


        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }

    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("TaxInvoice.aspx");
    }
    byte[] bytePdfRep = null;
    protected void GVInvoice_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "RowEdit")
            {
                Response.Redirect("TaxInvoice.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "&type=aprove");
            }
            if (e.CommandName == "Approve")
            {
                Cls_Main.Conn_Open();
                SqlCommand Cmd = new SqlCommand("UPDATE [tblTaxInvoiceHdr] SET Status=@Status WHERE ID=@ID", Cls_Main.Conn);
                Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
                Cmd.Parameters.AddWithValue("@Status", '2');
                Cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();
                FillGrid();
            }
            if (e.CommandName == "RowView")
            {

                CancelReport(e.CommandArgument.ToString(), "TaxInvoice");
            }
            if (e.CommandName == "RowViewEInvoice")
            {
                if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
                {
                    con.Open();
                    SqlCommand cmdQrdtl = new SqlCommand("select e_invoice_cancel_status from tbltaxinvoicehdr where Id='" + e.CommandArgument.ToString() + "'", con);
                    Object Qrdtl = cmdQrdtl.ExecuteScalar();
                    string e_invoice_cancel_status = Convert.ToString(Qrdtl);
                    con.Close();
                    if (e_invoice_cancel_status == "True")
                    {
                        CancelReport(e.CommandArgument.ToString(), "CancelEInvoice");
                    }
                    else
                    {
                        ViewState["PDFID"] = e.CommandArgument.ToString();
                        string[] types = { "ORIGINAL", "DUPLICATE", "TRIPLICATE" };
                        foreach (var type in types)
                        {
                            // Generate the report and get the PDF byte array
                            EInvoiceReports(e.CommandArgument.ToString(), "show", type);

                            using (MemoryStream pdfStream = new MemoryStream(bytePdfRep))
                            {
                                try
                                {
                                    using (PdfReader reader = new PdfReader(pdfStream))
                                    {
                                        // Define the file path for each type
                                        string filePath = Server.MapPath("~/PDF_Files/" + type + "_EInvoice.pdf");

                                        // Use FileStream to write the PDF to disk
                                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                                        {
                                            // Create a document and copy the content
                                            using (Document document = new Document())
                                            {
                                                iTextSharp.text.pdf.PdfCopy copy = new PdfCopy(document, fileStream);
                                                document.Open();
                                                copy.AddDocument(reader);
                                                document.Close();
                                            }
                                        }
                                    }
                                }
                                catch (iTextSharp.text.pdf.PdfException pdfEx)
                                {
                                    // Handle PDF-specific errors (log or notify)
                                    throw new Exception("Error reading PDF for '" + type + "': {pdfEx.Message}");
                                }
                            }
                        }
                        termsconditionreport();
                        string folder3 = HttpContext.Current.Server.MapPath("~/PDF_Files/Triplicate_EInvoice.pdf");
                        string folder2 = HttpContext.Current.Server.MapPath("~/PDF_Files/Duplicate_EInvoice.pdf");
                        string folder1 = HttpContext.Current.Server.MapPath("~/PDF_Files/Original_EInvoice.pdf");
                        string folder4 = HttpContext.Current.Server.MapPath("~/PDF_Files/TermCondition.pdf");
                        string outputPath = HttpContext.Current.Server.MapPath("~/PDF_Files/_EInvoice.pdf");

                        // Array containing the file paths
                        string[] fileArray = { folder1, folder2, folder3, folder4 };
                        //string[] fileArray = { folder1, folder4, folder2, folder4, folder3, folder4 };
                        // Merge the PDF files
                        MergePDFFiles(fileArray, outputPath);
                        Response.Redirect("~/PDF_Files/_EInvoice.pdf");
                    }


                }
            }
            if (e.CommandName == "RowCancel")
            {
                Cls_Main.Conn_Open();
                SqlCommand Cmd = new SqlCommand("UPDATE [tblTaxInvoiceHdr] SET e_invoice_cancel_status=@Status,e_invoice_cancel_date=@Date WHERE ID=@ID", Cls_Main.Conn);
                Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
                Cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                Cmd.Parameters.AddWithValue("@Status", 1);
                Cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();

                con.Open();
                SqlCommand cmd1 = new SqlCommand("select InvoiceNo from tblTaxInvoiceHdr where Id = " + Convert.ToInt32(e.CommandArgument.ToString()) + "", con);
                Object InvoiceNo = cmd1.ExecuteScalar();

                //Delete Outward Data For Inventory
                SqlCommand Cmd2 = new SqlCommand("delete tbl_OutwardEntryHdr where invoiceno=@invoiceno", con);
                Cmd2.Parameters.AddWithValue("@invoiceno", InvoiceNo);
                Cmd2.ExecuteNonQuery();

                SqlCommand Cmd3 = new SqlCommand("delete tbl_OutwardEntryDtls where RefNo=@invoiceno", con);
                Cmd3.Parameters.AddWithValue("@invoiceno", InvoiceNo);
                Cmd3.ExecuteNonQuery();

                SqlCommand Cmd4 = new SqlCommand("delete tbl_OutwardEntryComponentsDtls where OrderNo=@invoiceno", con);
                Cmd4.Parameters.AddWithValue("@invoiceno", InvoiceNo);
                Cmd4.ExecuteNonQuery();

                con.Close();
                SqlCommand cmddelete2 = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + InvoiceNo + "' ", con);
                con.Open();
                cmddelete2.ExecuteNonQuery();
                con.Close();

                FillGrid();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('e-invoice is canceled successfully..!!');", true);
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void GVInvoice_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVInvoice.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Authorization
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnEdit = e.Row.FindControl("lnkEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

            string empcode = Session["UserCode"].ToString();
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                string id = Dt.Rows[0]["ID"].ToString();
                DataTable Dtt = new DataTable();
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'CustomerTaxInvoiceList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    // btnCreate.Visible = false;
                    //GVQuotation.Columns[15].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }


            Label lblpaymentterm = (Label)e.Row.FindControl("lblpaymentterm");
            Label Companyname = (Label)e.Row.FindControl("Companyname");
            if (lblpaymentterm.Text != "")
            {
                DataTable Dt1 = new DataTable();
                SqlDataAdapter Sd1 = new SqlDataAdapter("Select PaymentTerm from tbl_CompanyMaster where Companyname='" + Companyname.Text + "'", con);
                Sd1.Fill(Dt1);
                if (Dt1.Rows.Count > 0)
                {
                    string paymentterm = Dt1.Rows[0]["PaymentTerm"].ToString();
                    if (Convert.ToDecimal(paymentterm) != Convert.ToDecimal(lblpaymentterm.Text))
                    {
                        e.Row.BackColor = System.Drawing.Color.LightPink;
                    }
                }
            }

            con.Open();
            LinkButton lnkCancel = (LinkButton)e.Row.FindControl("lnkCancel");
            LinkButton lnkRowView = (LinkButton)e.Row.FindControl("lnkRowView");
            LinkButton lblEinvoicepdf = (LinkButton)e.Row.FindControl("lblEinvoicepdf");
            int idd = Convert.ToInt32(GVInvoice.DataKeys[e.Row.RowIndex].Values[0]);
            DataTable Dtt1 = new DataTable();
            SqlDataAdapter Sdd1 = new SqlDataAdapter("Select * FROM tbltaxinvoicehdr where Id = '" + idd + "'", con);
            Sdd1.Fill(Dtt1);
            if (Dtt1.Rows.Count > 0)
            {
                string e_invoice_status = Dtt1.Rows[0]["e_invoice_status"].ToString();
                string e_invoice_cancel_status = Dtt1.Rows[0]["e_invoice_cancel_status"].ToString();
             

                if (e_invoice_cancel_status == true.ToString())
                {
                    btnEdit.Visible = false;
                    lnkCancel.Visible = false;
                     e.Row.BackColor = System.Drawing.Color.LightBlue;
                }
                if (e_invoice_status == true.ToString())
                {
                    btnEdit.Visible = false;
                    lnkRowView.Visible = false;
                }

            }
            con.Close();
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("EInvoiceList.aspx");
    }

    #region E-invoice and cancel PDF

    public void CancelReport(string Invoiceno, string type)
    {
        byte[] imageBytes = null;
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetE_InvoiceDetails");
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
                    // Genrate Qr Code For E-Invoice RDLC
                    con.Open();
                    string id = Invoiceno;
                    SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tbltaxinvoicehdr where Id='" + id + "'", con);
                    Object Qrdtl = cmdQrdtl.ExecuteScalar();
                    string F_Qrdtl = Convert.ToString(Qrdtl);
                    con.Close();
                    if (F_Qrdtl != "")
                    {
                        con.Open();
                        SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tbltaxinvoicehdr where Id='" + id + "'", con);
                        Object ACkdtl = cmdAckdtl.ExecuteScalar();
                        string F_Ackdtl = Convert.ToString(ACkdtl);
                        con.Close();
                        imageBytes = GenerateQR(F_Qrdtl);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("QRCodeImage", typeof(byte[]));

                    // Add the QR code image to the table
                    DataRow row = dt.NewRow();
                    row["QRCodeImage"] = imageBytes;
                    dt.Rows.Add(row);

                    // DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                    ReportDataSource obj4 = new ReportDataSource("DataSet4", dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.DataSources.Add(obj4);

                    if (type == "TaxInvoice")
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\TaxInvoice.rdlc";
                    }
                    else
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\CancelEInvoice.rdlc";

                    }
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    byte[] bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    string filePath = Server.MapPath("~/PDF_Files/") + "" + type + ".pdf";

                    // Save the file to the specified path
                    System.IO.File.WriteAllBytes(filePath, bytePdfRep);
                    Response.Redirect("~/PDF_Files/" + type + ".pdf");

                    //Response.ClearContent();
                    //Response.ClearHeaders();
                    //if (mail == "show")
                    ////{
                    ////    Response.Buffer = true;
                    ////    string Filename = Invoiceno + "_EInvoice.pdf";
                    ////    Response.ContentType = "application/vnd.pdf";
                    ////    Response.AddHeader("content-disposition", "attachment; filename=" + Filename + "");
                    ////    Response.BinaryWrite(bytePdfRep);
                    ////}
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

    public void termsconditionreport()
    {
        byte[] imageBytes = null;
        DataTable dt = new DataTable();
        dt.Columns.Add("QRCodeImage", typeof(byte[]));
        dt.Columns.Add("Type", typeof(string));
        DataRow row = dt.NewRow();
        row["QRCodeImage"] = imageBytes;
        row["Type"] = "(ORIGINAL FOR RECIPIENT)";
        dt.Rows.Add(row);

        ReportDataSource obj4 = new ReportDataSource("DataSet1", dt);
        ReportViewer1.LocalReport.DataSources.Add(obj4);
        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\TermCondition.rdlc";
        ReportViewer1.LocalReport.Refresh();
        //-------- Print PDF directly without showing ReportViewer ----
        Warning[] warnings;
        string[] streamids;
        string mimeType;
        string encoding;
        string extension;
        bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
        string filePath = Server.MapPath("~/PDF_Files/") + "TermCondition.pdf";

        // Save the file to the specified path
        System.IO.File.WriteAllBytes(filePath, bytePdfRep);
        // Response.Redirect("~/PDF_Files/TermCondition.pdf");

    }

    private byte[] GenerateQR(string QR_String)
    {
        byte[] QrBytes = null;
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img.jpg");
        var barcodeBitmap = new System.Drawing.Bitmap(result);

        using (MemoryStream memory = new MemoryStream())
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {

                barcodeBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                QrBytes = memory.ToArray();
                fs.Write(QrBytes, 0, QrBytes.Length);
            }
        }
        //img_QrCode.Visible = true;
        //img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img.jpg";
        return QrBytes;
    }

    public void EInvoiceReports(string Invoiceno, string mail, string type)
    {
        byte[] imageBytes = null;
        try
        {
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetE_InvoiceDetails");
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
                    // Genrate Qr Code For E-Invoice RDLC
                    con.Open();
                    string id = ViewState["PDFID"].ToString();
                    SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tbltaxinvoicehdr where Id='" + id + "'", con);
                    Object Qrdtl = cmdQrdtl.ExecuteScalar();
                    string F_Qrdtl = Convert.ToString(Qrdtl);
                    con.Close();
                    if (F_Qrdtl != "")
                    {
                        con.Open();
                        SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tbltaxinvoicehdr where Id='" + id + "'", con);
                        Object ACkdtl = cmdAckdtl.ExecuteScalar();
                        string F_Ackdtl = Convert.ToString(ACkdtl);
                        con.Close();
                        imageBytes = GenerateQR(F_Qrdtl);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("QRCodeImage", typeof(byte[]));
                    dt.Columns.Add("Type", typeof(string));
                    DataRow row = dt.NewRow();
                    if (type == "ORIGINAL")
                    {  // Add the QR code image to the table
                        row["QRCodeImage"] = imageBytes;
                        row["Type"] = "(ORIGINAL FOR RECIPIENT)";
                        dt.Rows.Add(row);
                    }
                    if (type == "DUPLICATE")
                    {
                        // Add the QR code image to the table
                        row["QRCodeImage"] = imageBytes;
                        row["Type"] = "(DUPLICATE FOR TRANSPORTER)";
                        dt.Rows.Add(row);

                    }
                    if (type == "TRIPLICATE")
                    {
                        // Add the QR code image to the table
                        row["QRCodeImage"] = imageBytes;
                        row["Type"] = "(TRIPLICATE FOR SUPPLIER)";
                        dt.Rows.Add(row);
                    }


                    // DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                    ReportDataSource obj4 = new ReportDataSource("DataSet4", dt);
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.DataSources.Add(obj4);

                    string e_invoice_cancel_status = Dtt.Tables[0].Rows[0]["e_invoice_cancel_status"].ToString();
                    if (e_invoice_cancel_status == true.ToString())
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\CancelEInvoice.rdlc";
                    }
                    else
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\EInvoice.rdlc";
                    }

                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

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
            throw (ex);
        }
    }

    public static void MergePDFFiles(string[] fileArray, string outputFile)
    {
        // Create a Document object to hold the final merged PDF
        Document document = new Document();
        try
        {
            // Create a PdfCopy instance to write the combined PDF
            PdfCopy writer = new PdfCopy(document, new FileStream(outputFile, FileMode.Create));
            document.Open();

            PdfReader reader;
            PdfImportedPage page;

            // Iterate over each PDF file to merge
            foreach (string file in fileArray)
            {
                reader = new PdfReader(file);
                int numberOfPages = reader.NumberOfPages;

                // Copy each page from the current PDF to the final document
                for (int i = 1; i <= numberOfPages; i++)
                {
                    page = writer.GetImportedPage(reader, i);
                    writer.AddPage(page);
                }

                reader.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
        finally
        {
            // Close the document after all pages have been added
            document.Close();
        }
    }
    #endregion

    //Search Customers methods
    //Added on 15-11-2024 
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCustomerList(string prefixText, int count)
    {
        return AutoFillCustomerName(prefixText);
    }

    public static List<string> AutoFillCustomerName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "select DISTINCT BillingCustomer from tblTaxInvoiceHdr where  " + "BillingCustomer like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["BillingCustomer"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnSearchData_Click(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillGrid();
    }


    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetInvoicenowiseList(string prefixText, int count)
    {
        return AutoFillInvoiceNo(prefixText);
    }

    public static List<string> AutoFillInvoiceNo(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "select DISTINCT InvoiceNo from tblTaxInvoiceHdr where  " + "InvoiceNo like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["InvoiceNo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
}