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
using System.Text;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;

public partial class Account_CreditDebitList : System.Web.UI.Page
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
                Gvbind();
            }
        }
    }

    private void Gvbind()
    {
        string query = string.Empty;
        query = @"select * from tblCreditDebitNoteHdr where NoteFor='Sale' AND Isdeleted=0 order by Id desc";
        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvCreditDebit.DataSource = dt;
            GvCreditDebit.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvCreditDebit.ClientID + "', 450, 1020 , 40 ,true); </script>", false);
        }
        else
        {
            GvCreditDebit.DataSource = null;
            GvCreditDebit.DataBind();
            ScriptManager.RegisterStartupScript(Page, this.GetType(), "Key", "<script>MakeStaticHeader('" + GvCreditDebit.ClientID + "', 450, 1020 , 40 ,true); </script>", false);
        }
    }

    protected void GvCreditDebit_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ViewState["CompRowId"] = e.CommandArgument.ToString();
        if (e.CommandName == "RowEdit")
        {
            ViewState["id"] = e.CommandArgument.ToString();
            Response.Redirect("CreditDebitNote_Sale.aspx?ID=" + encrypt(e.CommandArgument.ToString()));
        }

        if (e.CommandName == "DownloadPDF")
        {
            if (!string.IsNullOrEmpty(e.CommandArgument.ToString()))
            {
                Session["PDFID"] = e.CommandArgument.ToString();
                Response.Redirect("CDNotePDF.aspx?Id=" + encrypt(e.CommandArgument.ToString()) + " ");
                //Response.Write("<script>window.open('CDNotePDF.aspx?Id=" + encrypt(e.CommandArgument.ToString()) + "','_blank');</script>");
            }
        }

        if (e.CommandName == "RowDelete")
        {
            con.Open();

            //// Update ID in [tblCreditDebitNote_ID]
            SqlCommand cmdDocNo = new SqlCommand("SELECT BillNumber FROM [tblCreditDebitNoteHdr]  where Id='" + Convert.ToInt32(e.CommandArgument.ToString()) + "'", con);
            Object mxDocNo = cmdDocNo.ExecuteScalar();
            string DocNo = mxDocNo.ToString();

            con.Close();
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tblCreditDebitNoteHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE Id=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();

            SqlCommand cmddelete2 = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + DocNo + "' ", con);
            con.Open();
            cmddelete2.ExecuteNonQuery();
            con.Close();

            Cls_Main.Conn_Open();
            SqlCommand Cmd1 = new SqlCommand(@"Insert into tbl_InventoryOutwardManage([OrderNo]
      ,[Particular]
      ,[ComponentName]
      ,[Description]
      ,[HSN]
      ,[Quantity]
      ,[Units]
      ,[Rate]
      ,[CGSTPer]
      ,[CGSTAmt]
      ,[SGSTPer]
      ,[SGSTAmt]
      ,[IGSTPer]
      ,[IGSTAmt]
      ,[Total]
      ,[Discountpercentage]
      ,[DiscountAmount]
      ,[Alltotal]    
      ,[CreatedBy]
      ,[CreatedOn]     
      ,[Batch])
select [OrderNo]
      ,[Particular]
      ,[ComponentName]
      ,[Description]
      ,[HSN]
      ,[Quantity]
      ,[Units]
      ,[Rate]
      ,[CGSTPer]
      ,[CGSTAmt]
      ,[SGSTPer]
      ,[SGSTAmt]
      ,[IGSTPer]
      ,[IGSTAmt]
      ,[Total]
      ,[Discountpercentage]
      ,[DiscountAmount]
      ,[Alltotal]   
      ,[CreatedBy]
      ,GETDATE()     
      ,[Batch] 
from tbl_OutwardEntryComponentsDtls where OrderNo=@ID", Cls_Main.Conn);
            Cmd1.Parameters.AddWithValue("@ID", DocNo);

            Cmd1.ExecuteNonQuery();
            Cls_Main.Conn_Close();

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Deleted Sucessfully');window.location.href='CreditDebitNoteSaleList.aspx';", true);

        }
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

    #region Filter

    protected void txtcnamefilter_TextChanged(object sender, EventArgs e)
    {
        string query = string.Empty;
        if (!string.IsNullOrEmpty(txtcnamefilter.Text.Trim()))
        {
            query = "SELECT * FROM tblCreditDebitNoteHdr where Isdeleted=0 AND SupplierName like '" + txtcnamefilter.Text.Trim() + "%' order by Id desc";
        }
        else
        {
            query = "SELECT * FROM tblCreditDebitNoteHdr where  Isdeleted=0  order by Id desc";
        }

        SqlDataAdapter ad = new SqlDataAdapter(query, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            GvCreditDebit.DataSource = dt;
            GvCreditDebit.DataBind();
        }
        else
        {
            GvCreditDebit.DataSource = null;
            GvCreditDebit.DataBind();
        }
    }
    #endregion Filter

    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreditDebitNoteSaleList.aspx");
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
                //com.CommandText = "Select DISTINCT [SupplierName] from [tblSupplierMaster] where " + "SupplierName like @Search + '%'";
                com.CommandText = "Select DISTINCT [SupplierName] from [tblCreditDebitNoteHdr] where " + "SupplierName like + '%'+ @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["SupplierName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplierOwnerList(string prefixText, int count)
    {
        return AutoFillSupplierOwnerName(prefixText);
    }

    public static List<string> AutoFillSupplierOwnerName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT Ownername from tbl_VendorMaster where Ownername like @Search + '%' and status=0 and [isdeleted]=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Ownername"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    private string GetMailIdOfEmpl(string Empcode)
    {
        string query1 = "SELECT [email] FROM [employees] where [empcode]='" + Empcode + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        string email = string.Empty;
        if (dt.Rows.Count > 0)
        {
            email = dt.Rows[0]["email"].ToString();
        }
        return email;
    }

    protected void ddlsalesMainfilter_TextChanged(object sender, EventArgs e)
    {
        Gvbind();
    }

    protected void btnAddEnq_Click(object sender, EventArgs e)
    {
        string Cname = ((sender as Button).CommandArgument).ToString();
        Response.Redirect("Addenquiry.aspx?Cname=" + encrypt(Cname));
        //Page.ClientScript.RegisterStartupScript(GetType(), "", "window.open('EnquiryFile.aspx?Fileid=" + id + "','','width=700px,height=600px');", true);
    }
    protected void GvCreditDebit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblNoteType = (e.Row.FindControl("lblNoteType") as Label);
                Label txtbillno = (e.Row.FindControl("lblBillNumber") as Label);
                LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
                // LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
                LinkButton btnPDF = e.Row.FindControl("btnPDF") as LinkButton;
                string empcode = Session["UserCode"].ToString();
                DataTable Dt = new DataTable();
                SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
                Sd.Fill(Dt);
                if (Dt.Rows.Count > 0)
                {
                    string id = Dt.Rows[0]["ID"].ToString();
                    DataTable Dtt = new DataTable();
                    SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'CreditDebitNoteSaleList.aspx' AND PagesView = '1'", con);
                    Sdd.Fill(Dtt);
                    if (Dtt.Rows.Count > 0)
                    {
                        Button1.Visible = false;
                        btnEdit.Visible = false;
                        // btnDelete.Visible = false;
                        btnPDF.Visible = true;
                    }
                }


                if (lblNoteType.Text == "Debit_Sale Note")
                {
                    lblNoteType.Text = "Debit";
                }
                else if (lblNoteType.Text == "Credit_Sale Note")
                {
                    lblNoteType.Text = "Credit";
                }

                if (txtbillno.Text == "--Select Invoice Number--")
                {
                    txtbillno.Text = "";
                }
                else if (txtbillno.Text != "")
                {
                    //SqlCommand cmd = new SqlCommand("select SupplierBillNo from tblPurchaseBillHdr where BillNo='" + txtbillno.Text + "'", con);
                    SqlCommand cmd = new SqlCommand("select InvoiceNo from tblTaxInvoiceHdr where InvoiceNo='" + txtbillno.Text + "'", con);
                    con.Open();

                    //string Supplierbill = cmd.ExecuteScalar().ToString() == "--Select Invoice Number--" ? "" : cmd.ExecuteScalar().ToString();
                    string Supplierbill = cmd.ExecuteScalar().ToString();
                    txtbillno.Text = Supplierbill;

                    con.Close();
                }
                else
                {

                }

                // Check whether the E-Invoice is Created or not
                con.Open();
                int IDD = Convert.ToInt32(GvCreditDebit.DataKeys[e.Row.RowIndex].Values[0]);
                SqlCommand cmdIgstVal = new SqlCommand("select Irn from tblCreditDebitNoteHdr where Id='" + IDD + "'", con);
                Object F_IgstVal = cmdIgstVal.ExecuteScalar();
                string IsCreatedIRN = F_IgstVal.ToString();
                if (IsCreatedIRN == "")
                {
                    btnEdit.Visible = true;
                    //  btnDelete.Visible = true;

                }
                else
                {
                   // btnEdit.Visible = false;
                    // btnDelete.Visible = false;
                }
                con.Close();

            }


        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreditDebitNote_Sale.aspx");
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Gov_Bills/EInv_CrDbNote.aspx");
    }

    protected void GvInvoiceList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvCreditDebit.PageIndex = e.NewPageIndex;
        Gvbind();
    }

    public void GetInventoryCalculation(string lblCompo, string lblnewQuantity, string Product)
    {
        SqlCommand cmddelete = new SqlCommand("delete from tbl_InventoryOutwardManage where OrderNo='" + txtcnamefilter.Text.Trim() + "' ", con);
        con.Open();
        cmddelete.ExecuteNonQuery();
        con.Close();

        string lblDescription = "";
        string lblhsn = "";
        string lblUnit = "";
        string lblRate = "";
        string lblQuantity = "";
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
        string lblbatch = "";
        decimal TotalQuantity = 0;
        SqlDataAdapter sad2 = new SqlDataAdapter("select * from tbl_OutwardEntryComponentsDtls where OrderNo='" + txtcnamefilter.Text.Trim() + "'", con);
        DataTable Dt1 = new DataTable();
        sad2.Fill(Dt1);
        con.Close(); // Close the connection when done
        if (Dt1.Rows.Count > 0)
        {
            lblUnit = Dt1.Rows[0]["Units"].ToString();
            lblDescription = Dt1.Rows[0]["Description"].ToString();
            lblhsn = Dt1.Rows[0]["HSN"].ToString();
            lblRate = Dt1.Rows[0]["Rate"].ToString();
            lblCGSTPer = Dt1.Rows[0]["CGSTPer"].ToString();
            lblSGSTPer = Dt1.Rows[0]["SGSTPer"].ToString();
            lblIGSTPer = Dt1.Rows[0]["IGSTPer"].ToString();
            lblQuantity = Dt1.Rows[0]["Quantity"].ToString();
            lblbatch = Dt1.Rows[0]["Batch"].ToString();

            TotalQuantity = Convert.ToDecimal(lblQuantity) - Convert.ToDecimal(lblnewQuantity);


        }
        var total = Convert.ToDecimal(lblRate) * Convert.ToDecimal(TotalQuantity);
        lblTotal = string.Format("{0:0.00}", total);
        decimal tax_amt;
        decimal cgst_amt;
        decimal sgst_amt;
        decimal igst_amt;

        if (string.IsNullOrEmpty(lblCGSTPer))
        {
            cgst_amt = 0;
        }
        else
        {
            cgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblCGSTPer) / 100;
        }
        lblCGST = string.Format("{0:0.00}", cgst_amt);

        if (string.IsNullOrEmpty(lblSGSTPer))
        {
            sgst_amt = 0;
        }
        else
        {
            sgst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblSGSTPer) / 100;
        }
        lblSGST = string.Format("{0:0.00}", sgst_amt);

        if (string.IsNullOrEmpty(lblIGSTPer))
        {
            igst_amt = 0;
        }
        else
        {
            igst_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(lblIGSTPer) / 100;
        }
        lblIGST = string.Format("{0:0.00}", igst_amt);

        tax_amt = cgst_amt + sgst_amt + igst_amt;

        var totalWithTax = Convert.ToDecimal(total.ToString()) + Convert.ToDecimal(tax_amt.ToString());
        decimal disc_amt;
        if (string.IsNullOrEmpty(lblDiscount))
        {
            disc_amt = 0;
        }
        else
        {
            disc_amt = Convert.ToDecimal(totalWithTax.ToString()) * Convert.ToDecimal(lblDiscount) / 100;
            //disc_amt = Convert.ToDecimal(total.ToString()) * Convert.ToDecimal(Disc_Per.Text) / 100;
        }

        var Grossamt = Convert.ToDecimal(total.ToString()) - Convert.ToDecimal(disc_amt.ToString()) + tax_amt;
        lblAlltotal = string.Format("{0:0.00}", Grossamt);


        Cls_Main.Conn_Open();

        SqlCommand cmdd = new SqlCommand("INSERT INTO [tbl_InventoryOutwardManage] (OrderNo,Particular,ComponentName,Description,HSN,Quantity,Units,Rate,CGSTPer,CGSTAmt,SGSTPer,SGSTAmt,IGSTPer,IGSTAmt,Total,Discountpercentage,DiscountAmount,Alltotal,CreatedOn,Batch) VALUES(@OrderNo,@Particular,@ComponentName,@Description,@HSN,@Quantity,@Units,@Rate,@CGSTPer,@CGSTAmt,@SGSTPer,@SGSTAmt,@IGSTPer,@IGSTAmt,@Total,@Discountpercentage,@DiscountAmount,@Alltotal,@CreatedOn,@Batch)", Cls_Main.Conn);
        cmdd.Parameters.AddWithValue("@OrderNo", txtcnamefilter.Text);
        cmdd.Parameters.AddWithValue("@Particular", Product);
        cmdd.Parameters.AddWithValue("@ComponentName", lblCompo);
        cmdd.Parameters.AddWithValue("@Description", lblDescription);
        cmdd.Parameters.AddWithValue("@HSN", lblhsn);
        cmdd.Parameters.AddWithValue("@Quantity", TotalQuantity);
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
        cmdd.Parameters.AddWithValue("@Batch", lblbatch);
        cmdd.ExecuteNonQuery();
        Cls_Main.Conn_Close();
    }

}