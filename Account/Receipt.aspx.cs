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

public partial class Account_Receipt : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["New_connectionString"].ConnectionString);
    string id;
    string checkinvooice;
    DataTable dtinvoiceno = new DataTable();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["Username"] == null)
            {
                Response.Redirect("../Login.aspx");
            }
            else
            {
                if (Request.QueryString["Id"] != null)
                {
                    id = Decrypt(Request.QueryString["Id"].ToString());
                    Session["UPID"] = id;
                    btnsubmit.Text = "Update";
                    LoadData(id);

                    hidden1.Value = id;
                }
                else
                {
                    txtdate.Text = DateTime.Today.ToString("dd-MM-yyyy");
                }
            }

        }
    }

    protected void LoadData(string id)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from tblReceiptHdr where Id='" + id + "'", con);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtPartyName.Text = dt.Rows[0]["Partyname"].ToString();
            ddlAgainst.Text = dt.Rows[0]["Against"].ToString();
            txttds.Text = dt.Rows[0]["TDS"].ToString();

            Session["AgainstVal"] = dt.Rows[0]["Against"].ToString();

            if (ddlAgainst.Text == "Invoice")
            {
                //BindInvoiceNo();
                // ddlInvoiceNo.SelectedItem.Text = dt.Rows[0]["InvNo"].ToString();
                InvoiceDtlsload();
                Session["Adv"] = "0";
            }
            else
            {
                //ddlInvoiceNo.Enabled = false;
                Gvreceipt.Visible = false;
            }

            ddltoaccountName.Text = dt.Rows[0]["ToAccountName"].ToString();
            txtbankname.Text = dt.Rows[0]["BankName"].ToString();
            ddltransactionmode.Text = dt.Rows[0]["TransactionMode"].ToString();
            txtmodedescription.Text = dt.Rows[0]["Modedescription"].ToString();
            txtagainstpi.Text = dt.Rows[0]["AgainstRefNo"].ToString();
            //string str = dt.Rows[0]["postDate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time = Convert.ToDateTime(str);
            txtdate.Text = dt.Rows[0]["postDate"].ToString();
            // txtPartyName.Text = dt.Rows[0]["postDate"].ToString();

            txtamount.Text = dt.Rows[0]["Amount"].ToString();
            txtremark.Text = dt.Rows[0]["TransactionRemark"].ToString();

            //new Advance Show 2-7-23
            con.Open();
            SqlCommand cmddtt = new SqlCommand("SELECT Max( Amount) FROM [tblReceiptHdr] WHERE Partyname='" + dt.Rows[0]["Partyname"].ToString() + "' AND Against='Advance'", con);
            Object AdvAmt = cmddtt.ExecuteScalar();
            var FnlAdv = AdvAmt == null ? "0" : AdvAmt.ToString();
            lblpendingAdvance.Text = "Received Advance is : " + FnlAdv;
        }
    }

    protected void InvoiceDtlsload()
    {
        try
        {
            DataTable dtt = new DataTable();
            SqlDataAdapter sad1 = new SqlDataAdapter("select * from tblReceiptDtls where HeaderID='" + id + "'", con);
            sad1.Fill(dtt);
            Gvreceipt.DataSource = dtt;
            Gvreceipt.DataBind();
            Gvreceipt.EmptyDataText = "Record Not Found";
        }
        catch (Exception)
        {

            throw;
        }


    }

    //protected void BindInvoiceNo()
    //{

    //    SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceHdr where BillingCustomer='" + txtPartyName.Text + "'", con);
    //    sad.Fill(dtinvoiceno);
    //    ddlInvoiceNo.DataValueField = "Id";
    //    ddlInvoiceNo.DataTextField = "InvoiceNo";
    //    ddlInvoiceNo.DataSource = dtinvoiceno;
    //    ddlInvoiceNo.DataBind();
    //    //if(ddlAgainst.Text=="Advance")
    //    //{
    //    //    ddlInvoiceNo.Enabled = false;
    //    //}

    //}

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

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetcustomerList(string prefixText, int count)
    {
        return AutoFillcustomerlist(prefixText);
    }

    public static List<string> AutoFillcustomerlist(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "select DISTINCT Companyname from tbl_CompanyMaster where " + "Companyname like @Search + '%' AND IsDeleted='0' ";

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
    public static List<string> GetBankList(string prefixText, int count)
    {
        return AutoFillbanklist(prefixText);
    }

    public static List<string> AutoFillbanklist(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "select DISTINCT BankName from tblBankMaster where " + "BankName like '%'+ @Search + '%'  ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> BankName = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        BankName.Add(sdr["BankName"].ToString());
                    }
                }
                con.Close();
                return BankName;
            }

        }
    }

    protected void ddlAgainst_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(txtPartyName.Text))
            {
                if (ddlAgainst.Text == "Invoice")
                {
                    Gvreceipt.Visible = true;
                    DataTable dtagainst = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceHdr where BillingCustomer='" + txtPartyName.Text + "' AND Status>=2 And isdeleted=0", con);
                    sad.Fill(dtagainst);
                    if (dtagainst.Rows.Count > 0)
                    {
                        Gvreceipt.DataSource = dtagainst;
                        Gvreceipt.DataBind();
                        Gvreceipt.EmptyDataText = "Record Not Found";

                        //txtamount.Text = dtagainst.Rows[0]["GrandTotalFinal"].ToString();
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Invoice Record Not Found');", true);

                    }
                    lblshowmsg.Visible = false;

                    Session["Adv"] = "1";
                }
                else
                {
                    lblshowmsg.Visible = true;
                    Gvreceipt.Visible = false;
                    lblshowmsg.Text = "No Information Found";

                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Enter Party Name First');window.location.href='Receipt.aspx';", true);

            }
        }
        catch (Exception ee)
        {
            throw ee;
        }

    }

    DateTime date = DateTime.Now;
    protected void btnsubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (btnsubmit.Text == "Submit")
            {
                fnInsert();
            }
            else if (btnsubmit.Text == "Update")
            {
                if (Session["AgainstVal"] != "")
                {
                    if (Session["Adv"].ToString() == "1")
                    {
                        fnInsert();

                        string Advanceamt = "";
                        DataTable dttupdateadv = new DataTable();
                        SqlDataAdapter sadupds = new SqlDataAdapter("select * from tblReceiptHdr where Id='" + Session["UPID"].ToString() + "'", con);
                        sadupds.Fill(dttupdateadv);
                        if (dttupdateadv.Rows.Count > 0)
                        {
                            Advanceamt = dttupdateadv.Rows[0]["Amount"].ToString();
                        }

                        var MinusPlusAdvn = Convert.ToDouble(Advanceamt) - Convert.ToDouble(txtamount.Text);
                        SqlCommand cmduptt = new SqlCommand("update tblReceiptHdr set Amount='" + MinusPlusAdvn + "' where Id='" + Session["UPID"].ToString() + "'", con);
                        con.Open();
                        cmduptt.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("SP_ReceiptHdr", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Partyname", txtPartyName.Text);
                        cmd.Parameters.AddWithValue("@ToAccountName", ddltoaccountName.Text);
                        cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
                        cmd.Parameters.AddWithValue("@TransactionMode", ddltransactionmode.Text);
                        cmd.Parameters.AddWithValue("@Modedescription", txtmodedescription.Text);
                        cmd.Parameters.AddWithValue("@postDate", txtdate.Text);
                        cmd.Parameters.AddWithValue("@TDS", txttds.Text);
                        cmd.Parameters.AddWithValue("@TDSAgainst", txtbasic.Text);
                        cmd.Parameters.AddWithValue("@Against", ddlAgainst.Text);
                        cmd.Parameters.AddWithValue("@Amount", txtamount.Text);
                        cmd.Parameters.AddWithValue("@TransactionRemark", txtremark.Text);
                        cmd.Parameters.AddWithValue("@AgainstRefNo", txtagainstpi.Text);
                        cmd.Parameters.AddWithValue("@Updatedby", Session["Username"].ToString());
                        cmd.Parameters.AddWithValue("@updatedate", date);
                        cmd.Parameters.AddWithValue("@Id", hidden1.Value);
                        cmd.Parameters.AddWithValue("@Action", "Update");
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        SqlCommand CmdDelete = new SqlCommand("DELETE FROM tblReceiptDtls WHERE HeaderID=@HeaderID", con);
                        CmdDelete.Parameters.AddWithValue("@HeaderID", hidden1.Value);
                        con.Open();
                        CmdDelete.ExecuteNonQuery();
                        con.Close();

                        if (ddlAgainst.Text == "Invoice")
                        {
                            foreach (GridViewRow g1 in Gvreceipt.Rows)
                            {
                                bool chk = (g1.FindControl("chkRow") as CheckBox).Checked;
                                string InvoiceNo = (g1.FindControl("lblinvoiceno") as Label).Text;
                                string Invoicedate = (g1.FindControl("lblInvoiceDate") as Label).Text;
                                string payable = (g1.FindControl("lblpayable") as Label).Text;
                                string lblbasic = (g1.FindControl("lblfinalbasic") as Label).Text;
                                string Recvd = (g1.FindControl("lblratee") as TextBox).Text;
                                string WO = (g1.FindControl("lblWO") as TextBox).Text;
                                string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
                                string TDS = (g1.FindControl("txtgvTDS") as TextBox).Text;
                                string Adjust = (g1.FindControl("txtgvadjust") as TextBox).Text;
                                string Excess = (g1.FindControl("txtgvExcess") as TextBox).Text;
                                string Pending = (g1.FindControl("txtgvpending") as TextBox).Text;
                                string Total = (g1.FindControl("lbltotal") as Label).Text;
                                string Notes = (g1.FindControl("txtgvNote") as TextBox).Text;

                                SqlCommand cmd1 = new SqlCommand("SP_ReceiptDtls", con);
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@HeaderId", hidden1.Value);
                                cmd1.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                                cmd1.Parameters.AddWithValue("@Invoicedate", Invoicedate);
                                cmd1.Parameters.AddWithValue("@GrandTotal", payable);
                                cmd1.Parameters.AddWithValue("@WO", WO);

                                var Recd = Recvd == "" ? "0" : Recvd;
                                var totRece = Convert.ToDouble(Recd) + Convert.ToDouble(paid);
                                cmd1.Parameters.AddWithValue("@Recvd", totRece);

                                cmd1.Parameters.AddWithValue("@Paid", paid);
                                cmd1.Parameters.AddWithValue("@TDS", TDS);
                                cmd1.Parameters.AddWithValue("@Adjust", Adjust);
                                cmd1.Parameters.AddWithValue("@Excess", Excess);
                                cmd1.Parameters.AddWithValue("@Basic", lblbasic);
                                cmd1.Parameters.AddWithValue("@Pending", Pending);
                                cmd1.Parameters.AddWithValue("@Total", Total);
                                cmd1.Parameters.AddWithValue("@Note", Notes);
                                cmd1.Parameters.AddWithValue("@Chk", chk);
                                if (chk == true)
                                {
                                    cmd1.Parameters.AddWithValue("@Action", "Insert");
                                }
                                con.Open();
                                cmd1.ExecuteNonQuery();
                                con.Close();

                                con.Open();
                                SqlCommand cmddtt = new SqlCommand("select MAX(Id) from tblReceiptDtls where InvoiceNo='" + InvoiceNo + "'", con);
                                Object mxid = cmddtt.ExecuteScalar();

                                SqlCommand cmddtot = new SqlCommand("select total from tblReceiptDtls where Id='" + mxid.ToString() + "'", con);
                                Object TOTalamt = cmddtot.ExecuteScalar();

                                var Payble = payable;//lblfooterpayble.Text;
                                var tot = TOTalamt == null ? "0" : TOTalamt.ToString();

                                double dPayble = Double.Parse(Payble);
                                string RoundPayble = string.Format("{0:f2}", dPayble);

                                double d = Double.Parse(tot);
                                string rounded_input = string.Format("{0:f2}", d);

                                if (rounded_input == RoundPayble)
                                {
                                    SqlCommand cmdpaid = new SqlCommand("update tblTaxInvoiceHdr set IsPaid=1 where InvoiceNo='" + InvoiceNo + "'", con);
                                    cmdpaid.ExecuteNonQuery();
                                }
                                else
                                {
                                    SqlCommand cmdpaid = new SqlCommand("update tblTaxInvoiceHdr set IsPaid=null where InvoiceNo='" + InvoiceNo + "'", con);
                                    cmdpaid.ExecuteNonQuery();
                                }
                                con.Close();
                            }

                        }
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Updated Sucessfully');window.location.href='ReceiptList.aspx';", true);
                    }
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("SP_ReceiptHdr", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Partyname", txtPartyName.Text);
                    cmd.Parameters.AddWithValue("@ToAccountName", ddltoaccountName.Text);
                    cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
                    cmd.Parameters.AddWithValue("@TransactionMode", ddltransactionmode.Text);
                    cmd.Parameters.AddWithValue("@Modedescription", txtmodedescription.Text);
                    cmd.Parameters.AddWithValue("@postDate", txtdate.Text);
                    cmd.Parameters.AddWithValue("@TDS", txttds.Text);
                    cmd.Parameters.AddWithValue("@TDSAgainst", txtbasic.Text);
                    cmd.Parameters.AddWithValue("@Against", ddlAgainst.Text);
                    cmd.Parameters.AddWithValue("@Amount", txtamount.Text);
                    cmd.Parameters.AddWithValue("@TransactionRemark", txtremark.Text);
                    cmd.Parameters.AddWithValue("@Updatedby", Session["Username"].ToString());
                    cmd.Parameters.AddWithValue("@updatedate", date);
                    cmd.Parameters.AddWithValue("@Id", hidden1.Value);
                    cmd.Parameters.AddWithValue("@Action", "Update");
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    SqlCommand CmdDelete = new SqlCommand("DELETE FROM  tblReceiptDtls WHERE HeaderID=@HeaderID", con);
                    CmdDelete.Parameters.AddWithValue("@HeaderID", hidden1.Value);
                    con.Open();
                    CmdDelete.ExecuteNonQuery();
                    con.Close();

                    if (ddlAgainst.Text == "Invoice")
                    {
                        foreach (GridViewRow g1 in Gvreceipt.Rows)
                        {
                            bool chk = (g1.FindControl("chkRow") as CheckBox).Checked;
                            string InvoiceNo = (g1.FindControl("lblinvoiceno") as Label).Text;
                            string Invoicedate = (g1.FindControl("lblInvoiceDate") as Label).Text;
                            string payable = (g1.FindControl("lblpayable") as Label).Text;
                            string lblbasic = (g1.FindControl("lblfinalbasic") as Label).Text;
                            string Recvd = (g1.FindControl("lblratee") as TextBox).Text;
                            string WO = (g1.FindControl("lblWO") as TextBox).Text;
                            string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
                            string TDS = (g1.FindControl("txtgvTDS") as TextBox).Text;
                            string Adjust = (g1.FindControl("txtgvadjust") as TextBox).Text;
                            string Excess = (g1.FindControl("txtgvExcess") as TextBox).Text;
                            string Pending = (g1.FindControl("txtgvpending") as TextBox).Text;
                            string Total = (g1.FindControl("lbltotal") as Label).Text;
                            string Notes = (g1.FindControl("txtgvNote") as TextBox).Text;

                            SqlCommand cmd1 = new SqlCommand("SP_ReceiptDtls", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@HeaderId", id);
                            cmd1.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                            cmd1.Parameters.AddWithValue("@Invoicedate", Invoicedate);
                            cmd1.Parameters.AddWithValue("@GrandTotal", payable);
                            cmd1.Parameters.AddWithValue("@WO", WO);

                            var Recd = Recvd == "" ? "0" : Recvd;
                            var totRece = Convert.ToDouble(Recd) + Convert.ToDouble(paid);
                            cmd1.Parameters.AddWithValue("@Recvd", totRece);

                            cmd1.Parameters.AddWithValue("@Paid", paid);
                            cmd1.Parameters.AddWithValue("@TDS", TDS);
                            cmd1.Parameters.AddWithValue("@Adjust", Adjust);
                            cmd1.Parameters.AddWithValue("@Excess", Excess);
                            cmd1.Parameters.AddWithValue("@Basic", lblbasic);
                            cmd1.Parameters.AddWithValue("@Pending", Pending);
                            cmd1.Parameters.AddWithValue("@Total", Total);
                            cmd1.Parameters.AddWithValue("@Note", Notes);
                            cmd1.Parameters.AddWithValue("@Chk", chk);
                            if (chk == true)
                            {
                                cmd1.Parameters.AddWithValue("@Action", "Insert");
                            }
                            con.Open();
                            cmd1.ExecuteNonQuery();
                            con.Close();

                            con.Open();
                            SqlCommand cmddtt = new SqlCommand("select MAX(Id) from tblReceiptDtls where InvoiceNo='" + InvoiceNo + "'", con);
                            Object mxid = cmddtt.ExecuteScalar();

                            SqlCommand cmddtot = new SqlCommand("select total from tblReceiptDtls where Id='" + mxid.ToString() + "'", con);
                            Object TOTalamt = cmddtot.ExecuteScalar();

                            var Payble = payable;//lblfooterpayble.Text;
                            var tot = TOTalamt == null ? "0" : TOTalamt.ToString();

                            double dPayble = Double.Parse(Payble);
                            string RoundPayble = string.Format("{0:f2}", dPayble);

                            double d = Double.Parse(tot);
                            string rounded_input = string.Format("{0:f2}", d);

                            if (rounded_input == RoundPayble)
                            {
                                SqlCommand cmdpaid = new SqlCommand("update tblTaxInvoiceHdr set IsPaid=1 where InvoiceNo='" + InvoiceNo + "'", con);
                                cmdpaid.ExecuteNonQuery();
                            }
                            else
                            {
                                SqlCommand cmdpaid = new SqlCommand("update  tblTaxInvoiceHdr set IsPaid=null where InvoiceNo='" + InvoiceNo + "'", con);
                                cmdpaid.ExecuteNonQuery();
                            }
                            con.Close();
                        }

                    }
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Updated Sucessfully');window.location.href='ReceiptList.aspx';", true);
                }

            }

        }
        catch (Exception)
        {
            throw;
        }
    }


    public void fnInsert()
    {
        int id;
        try
        {
            SqlCommand cmd = new SqlCommand("SP_ReceiptHdr", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Partyname", txtPartyName.Text);
            cmd.Parameters.AddWithValue("@ToAccountName", ddltoaccountName.Text);
            cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
            cmd.Parameters.AddWithValue("@TransactionMode", ddltransactionmode.Text);
            cmd.Parameters.AddWithValue("@Modedescription", txtmodedescription.Text);
            cmd.Parameters.AddWithValue("@postDate", txtdate.Text);
            cmd.Parameters.AddWithValue("@Against", ddlAgainst.Text);
            cmd.Parameters.AddWithValue("@TDS", txttds.Text);
            cmd.Parameters.AddWithValue("@TDSAgainst", txtbasic.Text);
            cmd.Parameters.AddWithValue("@Amount", txtamount.Text);
            cmd.Parameters.AddWithValue("@TransactionRemark", txtremark.Text);
            cmd.Parameters.AddWithValue("@AgainstRefNo", txtagainstpi.Text);
            cmd.Parameters.AddWithValue("@isdeleted", '0');
            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
            cmd.Parameters.AddWithValue("@Createddate", date);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@Action", "Insert");
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            id = Convert.ToInt32(cmd.Parameters["@Id"].Value);
            if (ddlAgainst.Text == "Invoice")
            {
                foreach (GridViewRow g1 in Gvreceipt.Rows)
                {
                    if (g1.Visible == true)
                    {
                        bool chk = (g1.FindControl("chkRow") as CheckBox).Checked;
                        string InvoiceNo = (g1.FindControl("lblinvoiceno") as Label).Text;
                        string Invoicedate = (g1.FindControl("lblInvoiceDate") as Label).Text;
                        string payable = (g1.FindControl("lblpayable") as Label).Text;
                        string lblbasic = (g1.FindControl("lblfinalbasic") as Label).Text;
                        string Recvd = (g1.FindControl("lblratee") as TextBox).Text;
                        string WO = (g1.FindControl("lblWO") as TextBox).Text;
                        string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
                        string TDS = (g1.FindControl("txtgvTDS") as TextBox).Text;
                        string Adjust = (g1.FindControl("txtgvadjust") as TextBox).Text;
                        string Excess = (g1.FindControl("txtgvExcess") as TextBox).Text;
                        string Pending = (g1.FindControl("txtgvpending") as TextBox).Text;
                        string Total = (g1.FindControl("lbltotal") as Label).Text;
                        string Notes = (g1.FindControl("txtgvNote") as TextBox).Text;

                        SqlCommand cmd1 = new SqlCommand("SP_ReceiptDtls", con);
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@HeaderId", id);
                        cmd1.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                        cmd1.Parameters.AddWithValue("@Invoicedate", Invoicedate);
                        cmd1.Parameters.AddWithValue("@GrandTotal", payable);
                        cmd1.Parameters.AddWithValue("@WO", WO);

                        var Recd = Recvd == "" ? "0" : Recvd;
                        var totRece = Convert.ToDouble(Recd) + Convert.ToDouble(paid);
                        cmd1.Parameters.AddWithValue("@Recvd", totRece);

                        cmd1.Parameters.AddWithValue("@Paid", paid);
                        cmd1.Parameters.AddWithValue("@TDS", TDS);
                        cmd1.Parameters.AddWithValue("@Adjust", Adjust);
                        cmd1.Parameters.AddWithValue("@Excess", Excess);
                        cmd1.Parameters.AddWithValue("@Basic", lblbasic);
                        cmd1.Parameters.AddWithValue("@Pending", Pending);
                        cmd1.Parameters.AddWithValue("@Total", Total);
                        cmd1.Parameters.AddWithValue("@Note", Notes);
                        cmd1.Parameters.AddWithValue("@Chk", chk);
                        if (chk == true)
                        {
                            cmd1.Parameters.AddWithValue("@Action", "Insert");
                        }
                        con.Open();
                        cmd1.ExecuteNonQuery();
                        con.Close();

                        Label lblfooterpayble = (Label)Gvreceipt.FooterRow.FindControl("footerpayble");

                        con.Open();
                        SqlCommand cmddtt = new SqlCommand("select MAX(Id) from tblReceiptDtls where InvoiceNo='" + InvoiceNo + "'", con);
                        Object mxid = cmddtt.ExecuteScalar();

                        SqlCommand cmddtot = new SqlCommand("select total from  tblReceiptDtls where Id='" + mxid.ToString() + "'", con);
                        Object TOTalamt = cmddtot.ExecuteScalar();

                        var Payble = payable;//lblfooterpayble.Text;
                        var tot = TOTalamt == null ? "0" : TOTalamt.ToString();

                        double dPayble = Double.Parse(Payble);
                        string RoundPayble = string.Format("{0:f2}", dPayble);

                        double d = Double.Parse(tot);
                        string rounded_input = string.Format("{0:f2}", d);

                        if (rounded_input == RoundPayble)
                        {
                            SqlCommand cmdpaid = new SqlCommand("update tblTaxInvoiceHdr set IsPaid=1 where InvoiceNo='" + InvoiceNo + "'", con);
                            cmdpaid.ExecuteNonQuery();
                        }
                        else
                        {
                        }
                        con.Close();
                    }
                }


                DataTable dt546665 = new DataTable();
                SqlDataAdapter sadparticular = new SqlDataAdapter("select * from tblReceiptDtls where HeaderID='" + id + "'", con);
                sadparticular.Fill(dt546665);
                if (dt546665.Rows.Count > 0)
                {

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('At Least Select One Record.');", true);
                }
            }
            else
            {

            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='ReceiptList.aspx';", true);
        }
        catch (Exception ex)
        {

            throw ex;
        }


    }

    //protected void ddlInvoiceNo_TextChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        DataTable dtinvoicenobind = new DataTable();
    //        SqlDataAdapter sad = new SqlDataAdapter("select * from tblTaxInvoiceDtls where HeaderID='" + ddlInvoiceNo.SelectedValue + "'", con);
    //        sad.Fill(dtinvoicenobind);
    //        if (dtinvoicenobind.Rows.Count > 0)
    //        {
    //            txtamount.Text = dtinvoicenobind.Rows[0]["GrandTotal"].ToString();
    //        }
    //        Gvreceipt.DataSource = dtinvoicenobind;
    //        Gvreceipt.DataBind();
    //        Gvreceipt.EmptyDataText = "Record Not Found";
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }
    //}

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("Receipt.aspx");
    }

    Double SumOfTotalFooter;
    protected void chkRow_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in Gvreceipt.Rows)
        {
            // GridViewRow row = (sender as CheckBox).NamingContainer as GridViewRow;
            TextBox paid = (TextBox)row.FindControl("txtgvpaid");
            TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
            TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
            TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
            TextBox Pending = (TextBox)row.FindControl("txtgvpending");
            TextBox Note = (TextBox)row.FindControl("txtgvNote");
            Label payable = (Label)row.FindControl("lblpayable");
            Label Totalamount = (Label)row.FindControl("lbltotal");
            TextBox Reced = (TextBox)row.FindControl("lblratee");

            CheckBox chk = (CheckBox)row.FindControl("chkRow");

            Label lblfooterpaid = (Label)Gvreceipt.FooterRow.FindControl("footerpaid");

            if (chk != null & chk.Checked)
            {
               
                var paidval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text == "" ? "0" : Reced.Text);

                var pendingval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text == "" ? "0" : Reced.Text) - Convert.ToDouble(paidval.ToString());
                //object creditNotesObject = ViewState["Creditnotes"];



                //if (creditNotesObject != null)
                //{
                //    Decimal received = Convert.ToDecimal(creditNotesObject);
                //    Reced.Text = received.ToString();

                //}

                //payable.Text = paid.ToString() ;
                paid.Enabled = true;
                TDS.Enabled = true;
                Adjust.Enabled = true;
                Excess.Enabled = true;
                Pending.Enabled = true;
                Note.Enabled = true;
                paid.Text = paidval.ToString();
                Totalamount.Text = payable.Text;
                Pending.Text = pendingval.ToString();
                SumOfTotalFooter += Convert.ToDouble(paidval.ToString());
                getTDScalculations();

            }
            else
            {
                paid.Enabled = false;
                TDS.Enabled = false;
                Adjust.Enabled = false;
                Excess.Enabled = false;
                Pending.Enabled = false;
                Note.Enabled = false;
                paid.Text = "0";
                TDS.Text = "0";
                Adjust.Text = "0";
                Excess.Text = "0";
                Pending.Text = "0";
                Note.Text = string.Empty;
                Totalamount.Text = "0";
            }
            lblfooterpaid.Text = SumOfTotalFooter.ToString();
            lblFooterPaidVal.Text = SumOfTotalFooter.ToString();
        }
    }

    protected void txtgvpaid_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GridViewRow roww = (sender as TextBox).NamingContainer as GridViewRow;
            Calculation(roww);

            foreach (GridViewRow row in Gvreceipt.Rows)
            {
                // GridViewRow row = (sender as CheckBox).NamingContainer as GridViewRow;
                TextBox paid = (TextBox)row.FindControl("txtgvpaid");
                TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
                TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
                TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
                TextBox Pending = (TextBox)row.FindControl("txtgvpending");
                TextBox Note = (TextBox)row.FindControl("txtgvNote");
                Label payable = (Label)row.FindControl("lblpayable");
                Label Totalamount = (Label)row.FindControl("lbltotal");
                TextBox Reced = (TextBox)row.FindControl("lblratee");

                CheckBox chk = (CheckBox)row.FindControl("chkRow");

                Label lblfooterpaid = (Label)Gvreceipt.FooterRow.FindControl("footerpaid");

                if (chk != null & chk.Checked)
                {
                    var paidval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text == "" ? "0" : Reced.Text);

                    var pendingval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text == "" ? "0" : Reced.Text) - Convert.ToDouble(paidval.ToString());
                    SumOfTotalFooter += Convert.ToDouble(paid.Text);
                }
                else
                {
                    paid.Text = "0";
                }
                lblfooterpaid.Text = SumOfTotalFooter.ToString();
                lblFooterPaidVal.Text = SumOfTotalFooter.ToString();

                if (Convert.ToDouble(txtamount.Text) != Convert.ToDouble(lblFooterPaidVal.Text))
                {
                    btnsubmit.Enabled = false;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Amount & Receipt Amount Didnt Match. Kindly Check Both Amounts..!!');", true);

                }
                else
                {
                    btnsubmit.Enabled = true;
                }
            }
        }
        catch
        {

        }
    }

    protected void Calculation(GridViewRow row)
    {

        Label lblPayable = (Label)row.FindControl("lblpayable");
        Label lbltotal = (Label)row.FindControl("lbltotal");
        TextBox txtTDS = (TextBox)row.FindControl("txtgvTDS");
        TextBox txtadjust = (TextBox)row.FindControl("txtgvadjust");
        TextBox txtexcess = (TextBox)row.FindControl("txtgvExcess");
        TextBox txtpending = (TextBox)row.FindControl("txtgvpending");
        TextBox txtpaid = (TextBox)row.FindControl("txtgvpaid");
        TextBox lblrecvd = (TextBox)row.FindControl("lblratee");
        txtpending.Text = lblPayable.ToString();



        var paidtotal = Convert.ToDouble(txtpaid.Text) + Convert.ToDouble(txtTDS.Text) + Convert.ToDouble(txtadjust.Text) + Convert.ToDouble(txtexcess.Text);


        double payAmt = Convert.ToDouble(lblPayable.Text);
        double Paid = Convert.ToDouble(paidtotal);

        var rcvval = lblrecvd.Text == "" ? "0" : lblrecvd.Text;

        var pending = payAmt - Convert.ToDouble(rcvval) - Paid;

        txtpending.Text = pending.ToString("#0.00");
        lbltotal.Text = paidtotal.ToString();


    }

    protected void txtgvTDS_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculation(row);
    }

    protected void txtgvadjust_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculation(row);
    }

    protected void txtgvExcess_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculation(row);
    }

    decimal payable, paid, TDS, Basic, Excess, Adjust, Total, pending = 0;
    protected void Gvreceipt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (btnsubmit.Text == "Update")
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (Session["AgainstVal"].ToString() == "Advance")
                {
                    System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en-IN");
                    int idd = Convert.ToInt32(Gvreceipt.DataKeys[e.Row.RowIndex].Values[0]);
                    string FinaleTotalamt = "";

                    con.Open();
                    CheckBox chckrow = (CheckBox)e.Row.FindControl("chkRow");
                    Label lblinvoicenovv = (Label)e.Row.FindControl("lblinvoiceno");
                    Label lblmsgpaid = (Label)e.Row.FindControl("lblmsgpaid");
                    Label payableAmt = (Label)e.Row.FindControl("lblpayable");
                    CheckBox chkk = (CheckBox)e.Row.FindControl("chkRow");
                    CheckBox chkheader = (CheckBox)Gvreceipt.HeaderRow.FindControl("chkheader");

                    SqlCommand cmdispaid = new SqlCommand("select IsPaid from tblTaxInvoiceHdr where InvoiceNo='" + lblinvoicenovv.Text + "'", con);
                    Object ispaid = cmdispaid.ExecuteScalar();

                    var ispaidval = ispaid == null ? "False" : ispaid.ToString();
                    if (ispaidval == "True")
                    {

                        payable = 0;
                        e.Row.Visible = false;
                    }
                    else
                    {
                        e.Row.Visible = true;


                        SqlCommand cmdmaxxx = new SqlCommand("SELECT min(Pending) FROM tblReceiptDtls where InvoiceNo='" + lblinvoicenovv.Text + "'", con);
                        Object smpayable = cmdmaxxx.ExecuteScalar();


                        SqlCommand cmddtl = new SqlCommand("select SUM(CAST(TaxableAmt as float)) from tblTaxInvoiceDtls where HeaderID='" + idd + "'", con);
                        Object TaxAmt = cmddtl.ExecuteScalar();

                        SqlCommand cmddtl1 = new SqlCommand("select CGSTPer from tblTaxInvoiceDtls where HeaderID='" + idd + "'", con);
                        Object GSTPer = cmddtl1.ExecuteScalar();

                        SqlCommand cmddtl11 = new SqlCommand("select IGSTPer from tblTaxInvoiceDtls where HeaderID='" + idd + "'", con);
                        Object IGSTPer = cmddtl11.ExecuteScalar();
                        con.Close();
                        Label lblBasic = (Label)e.Row.FindControl("lblfinalbasic");

                        var Amt = Convert.ToDecimal(TaxAmt.ToString()) + Convert.ToDecimal(lblBasic.Text);

                        decimal PaybleTotal = 0;
                        if (IGSTPer.ToString() == "0")
                        {
                            var CGSTAmt = Amt * Convert.ToInt32(GSTPer.ToString()) / 100;
                            var SGSTAmt = Amt * Convert.ToInt32(GSTPer.ToString()) / 100;

                            PaybleTotal = Amt + CGSTAmt + SGSTAmt;
                        }
                        else
                        {
                            var IGSTAmt = Amt * Convert.ToInt32(IGSTPer.ToString()) / 100;
                            PaybleTotal = Amt + IGSTAmt;
                        }


                        var Totalamtfff = Math.Round(PaybleTotal);
                        FinaleTotalamt = Totalamtfff.ToString();

                        Label lblPgTotall = (Label)e.Row.FindControl("lblpayable");
                        lblPgTotall.Text = FinaleTotalamt;
                        payable += Decimal.Parse(lblPgTotall.Text);

                        var basicvalue = Math.Round(Amt);
                        Label lblfinalbasicc = (Label)e.Row.FindControl("lblfinalbasic");
                        lblfinalbasicc.Text = basicvalue.ToString();
                        Basic += Decimal.Parse(lblfinalbasicc.Text);

                        Label lblinvoiceno = (Label)e.Row.FindControl("lblinvoiceno");
                        SqlCommand cmdmax = new SqlCommand("SELECT Recvd FROM tblReceiptDtls where InvoiceNo='" + lblinvoiceno.Text + "'", con);
                      //  SqlCommand cmdmax = new SqlCommand("SELECT Recvd FROM tblCreditDebitNoteHdr where BillNumber='" + lblinvoiceno.Text + "'", con);
                        con.Open();
                        Object Recvdval = cmdmax.ExecuteScalar();
                        con.Close();

                        TextBox lblRecvdd = (TextBox)e.Row.FindControl("lblratee");

                        if (Recvdval == null)
                        {
                            lblRecvdd.Text = "0";
                        }
                        else
                        {
                            lblRecvdd.Text = Recvdval.ToString();
                        }

                        if (smpayable.ToString() == "0")
                        {
                            payable = Convert.ToDecimal("0.00");
                            chkk.Enabled = false;
                            chkheader.Enabled = false;
                            e.Row.Visible = false;

                        }

                        TextBox lblpaid = (TextBox)e.Row.FindControl("txtgvpaid");
                        paid += Decimal.Parse(lblpaid.Text);
                        TextBox lblTDS = (TextBox)e.Row.FindControl("txtgvTDS");
                        TDS += Decimal.Parse(lblTDS.Text);
                        TextBox lblAdjust = (TextBox)e.Row.FindControl("txtgvadjust");
                        Adjust += Decimal.Parse(lblAdjust.Text);
                        TextBox lblexcess = (TextBox)e.Row.FindControl("txtgvExcess");
                        Excess += Decimal.Parse(lblexcess.Text);
                        Label lbltotal = (Label)e.Row.FindControl("lbltotal");
                        Total += Decimal.Parse(lbltotal.Text);
                        TextBox txtadjust = (TextBox)e.Row.FindControl("txtgvpending");
                        pending += Decimal.Parse(txtadjust.Text);
                    }
                    con.Close();
                }
                else
                {
                    int id = Convert.ToInt32(Gvreceipt.DataKeys[e.Row.RowIndex].Values[0]);
                    TextBox paid = (TextBox)e.Row.FindControl("txtgvpaid");
                    TextBox TDS = (TextBox)e.Row.FindControl("txtgvTDS");
                    TextBox Adjust = (TextBox)e.Row.FindControl("txtgvadjust");
                    TextBox Excess = (TextBox)e.Row.FindControl("txtgvExcess");
                    TextBox Pending = (TextBox)e.Row.FindControl("txtgvpending");
                    Label Total = (Label)e.Row.FindControl("lbltotal");
                    TextBox Notes = (TextBox)e.Row.FindControl("txtgvNote");
                    CheckBox chk = (CheckBox)e.Row.FindControl("chkRow");
                    Label lblPgTotal = (Label)e.Row.FindControl("lblpayable");
                    Label lblfinalbasic = (Label)e.Row.FindControl("lblfinalbasic");
                    con.Open();
                    SqlCommand cmd4525 = new SqlCommand("select * from tblReceiptDtls where Id='" + id + "'", con);
                    SqlDataReader dr = cmd4525.ExecuteReader();
                    if (dr.Read())
                    {

                        paid.Text = dr["Paid"].ToString();
                        TDS.Text = dr["TDS"].ToString();
                        Adjust.Text = dr["Adjust"].ToString();
                        Excess.Text = dr["Excess"].ToString();
                        Pending.Text = dr["Pending"].ToString();
                        Total.Text = dr["Total"].ToString();
                        Notes.Text = dr["Note"].ToString();
                        checkinvooice = dr["Chk"].ToString();
                        lblPgTotal.Text = dr["GrandTotalFinal"].ToString();
                        lblfinalbasic.Text = dr["Basic"].ToString();
                        dr.Close();
                    }
                    chk.Checked = checkinvooice == "True" ? true : false;
                    con.Close();


                    if (chk != null & chk.Checked)
                    {
                        paid.Enabled = true;
                        TDS.Enabled = true;
                        Adjust.Enabled = true;
                        Excess.Enabled = true;
                        Pending.Enabled = true;
                        Notes.Enabled = true;

                    }
                    else
                    {
                        paid.Enabled = false;
                        TDS.Enabled = false;
                        Adjust.Enabled = false;
                        Excess.Enabled = false;
                        Pending.Enabled = false;
                        Notes.Enabled = false;

                    }
                }
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Price Format
            System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en-IN");
            int id = Convert.ToInt32(Gvreceipt.DataKeys[e.Row.RowIndex].Values[0]);
            string FinaleTotalamt = "";
            if (btnsubmit.Text != "Update")
            {
                con.Open();
                CheckBox chckrow = (CheckBox)e.Row.FindControl("chkRow");
                Label lblinvoicenovv = (Label)e.Row.FindControl("lblinvoiceno");
                Label lblmsgpaid = (Label)e.Row.FindControl("lblmsgpaid");
                Label payableAmt = (Label)e.Row.FindControl("lblpayable");
                CheckBox chk = (CheckBox)e.Row.FindControl("chkRow");
                CheckBox chkheader = (CheckBox)Gvreceipt.HeaderRow.FindControl("chkheader");

                SqlCommand cmdispaid = new SqlCommand("SELECT IsPaid FROM tblTaxInvoiceHdr WHERE InvoiceNo = '" + lblinvoicenovv.Text + "'", con);

                Object ispaid = cmdispaid.ExecuteScalar();

                //Changes by shubham wankhade For Cedit Debit Notes
                SqlCommand cmdCreditNote = new SqlCommand("select Grandtotal  from  tblCreditDebitNoteHdr where  BillNumber='" + lblinvoicenovv.Text + "'", con);
                object CreditNotes = cmdCreditNote.ExecuteScalar();


                Label lblinvoiceno = (Label)e.Row.FindControl("lblinvoiceno");
                SqlCommand cmdmax = new SqlCommand("SELECT Max(CAST(Paid as float)) FROM tblReceiptDtls where InvoiceNo='" + lblinvoiceno.Text + "'", con);
                Object Recvdval = cmdmax.ExecuteScalar();
                TextBox lblRecvdd = (TextBox)e.Row.FindControl("lblratee");

                var CreditNote = Convert.ToDecimal(CreditNotes);
                if (CreditNote > 0)
                {
                    ViewState["Creditnotes"] = CreditNote;
                }


                if (Recvdval == null)
                {
                    lblRecvdd.Text = "0";
                }
                else
                {
                    //lblRecvdd.Text = Recvdval.ToString();
                    decimal RecvdDecimal = (Recvdval != DBNull.Value) ? Convert.ToDecimal(Recvdval) : 0;
                    decimal CreditNoteDecimal = Convert.ToDecimal(CreditNote);

                    lblRecvdd.Text = (RecvdDecimal + CreditNoteDecimal).ToString();
                }


                //End



                SqlCommand cmdmaxxx = new SqlCommand("SELECT min(Pending) FROM tblReceiptDtls where InvoiceNo='" + lblinvoicenovv.Text + "'", con);
                Object smpayable = cmdmaxxx.ExecuteScalar();

               SqlCommand cmddtl = new SqlCommand("select SUM(CAST(TaxableAmt as float)) from tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
                Object TaxAmt = cmddtl.ExecuteScalar();

                SqlCommand cmddtl1 = new SqlCommand("select CGSTPer from tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
                Object GSTPer = cmddtl1.ExecuteScalar();

                SqlCommand cmddtl11 = new SqlCommand("select IGSTPer from  tblTaxInvoiceDtls where HeaderID='" + id + "'", con);
                Object IGSTPer = cmddtl11.ExecuteScalar();
                con.Close();
                Label lblBasic = (Label)e.Row.FindControl("lblfinalbasic");

                var Amt = Convert.ToDecimal(TaxAmt.ToString()) + Convert.ToDecimal(lblBasic.Text);
                //var CreditNote = Convert.ToDecimal(CreditNotes);
                //if (CreditNote > 0)
                //{
                //    ViewState["Creditnotes"] = CreditNote;
                //}
                decimal PaybleTotal = 0;
                if (IGSTPer.ToString() == "0")
                {
                    var CGSTAmt = Amt * Convert.ToInt32(GSTPer.ToString()) / 100;
                    var SGSTAmt = Amt * Convert.ToInt32(GSTPer.ToString()) / 100;
                    //PaybleTotal = (Amt + CGSTAmt + SGSTAmt) - (CreditNote );
                   // PaybleTotal = (Amt + CGSTAmt + SGSTAmt) - (CreditNote + Convert.ToDecimal(string.IsNullOrEmpty(Recvdval.ToString()) ? "0" : Recvdval.ToString()));

                    PaybleTotal = (Amt + CGSTAmt + SGSTAmt);
                }
                else
                {
                    var IGSTAmt = Amt * Convert.ToInt32(IGSTPer.ToString()) / 100;
                    PaybleTotal = Amt + IGSTAmt- CreditNote;
                    //PaybleTotal = (Amt + IGSTAmt ) - (CreditNote + Convert.ToDecimal(Recvdval));
                }

                var Totalamtfff = Math.Round(PaybleTotal);
                FinaleTotalamt = Totalamtfff.ToString();

                var ispaidval = ispaid == null ? "False" : ispaid.ToString();
                if (ispaidval == "True")
                {
                    e.Row.Visible = false;
                }
                else
                {
                    e.Row.Visible = true;
                    Label lblPgTotal = (Label)e.Row.FindControl("lblpayable");
                    lblPgTotal.Text = FinaleTotalamt;
                    payable += Decimal.Parse(lblPgTotal.Text);
                }

                var basicvalue = Math.Round(Amt);
                Label lblfinalbasic = (Label)e.Row.FindControl("lblfinalbasic");
                lblfinalbasic.Text = basicvalue.ToString();
                Basic += Decimal.Parse(lblfinalbasic.Text);

                //Label lblinvoiceno = (Label)e.Row.FindControl("lblinvoiceno");
                //SqlCommand cmdmax = new SqlCommand("SELECT Max(CAST(Recvd as float)) FROM [ExcelEncLive]. tblReceiptDtls where InvoiceNo='" + lblinvoiceno.Text + "'", con);
                //con.Open();
                //Object Recvdval = cmdmax.ExecuteScalar();
                //con.Close();

                //TextBox lblRecvdd = (TextBox)e.Row.FindControl("lblratee");

                //if (Recvdval == null)
                //{
                //    lblRecvdd.Text = "0";
                //}
                //else
                //{
                //    lblRecvdd.Text = Recvdval.ToString();
                //}
                //if (smpayable.ToString() == "0")
                //{
                //    payable = Convert.ToDecimal("0.00");
                //    chk.Enabled = false;
                //    chkheader.Enabled = false;
                //    e.Row.Visible = false;
                //}
            }
            else
            {
                //Label lblPgTotal = (Label)e.Row.FindControl("lblpayable");
                //payable += Decimal.Parse(lblPgTotal.Text);
            }
            //Label payabled = (Label)e.Row.FindControl("lblpayable");
            //payable += Decimal.Parse(payabled.Text);

            TextBox lblpaid = (TextBox)e.Row.FindControl("txtgvpaid");
            paid += Decimal.Parse(lblpaid.Text);
            TextBox lblTDS = (TextBox)e.Row.FindControl("txtgvTDS");
            TDS += Decimal.Parse(lblTDS.Text);
            TextBox lblAdjust = (TextBox)e.Row.FindControl("txtgvadjust");
            Adjust += Decimal.Parse(lblAdjust.Text);
            TextBox lblexcess = (TextBox)e.Row.FindControl("txtgvExcess");
            Excess += Decimal.Parse(lblexcess.Text);
            Label lbltotal = (Label)e.Row.FindControl("lbltotal");
            Total += Decimal.Parse(lbltotal.Text);
            TextBox txtadjust = (TextBox)e.Row.FindControl("txtgvpending");
            pending += Decimal.Parse(txtadjust.Text);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblpayablefooter = (Label)e.Row.FindControl("footerpayble");
            lblpayablefooter.Text = payable.ToString();
            Label lblfooterpaid = (Label)e.Row.FindControl("footerpaid");
            lblfooterpaid.Text = paid.ToString();
            lblFooterPaidVal.Text = paid.ToString();   /// new Update
            Label lblfootertds = (Label)e.Row.FindControl("footertds");
            lblfootertds.Text = TDS.ToString();
            Label lblfooteradjust = (Label)e.Row.FindControl("footeradjust");
            lblfooteradjust.Text = Adjust.ToString();
            Label lblfooterexcess = (Label)e.Row.FindControl("footerexcess");
            lblfooterexcess.Text = Excess.ToString();
            Label lblfootertotal = (Label)e.Row.FindControl("footertotal");
            lblfootertotal.Text = Total.ToString();
            Label lblfooterpending = (Label)e.Row.FindControl("footerpending");
            lblfooterpending.Text = pending.ToString();

        }

    }

    protected void chkheader_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow row in Gvreceipt.Rows)
            {
                CheckBox chckheader = (CheckBox)Gvreceipt.HeaderRow.FindControl("chkHeader");
                CheckBox chckrow = (CheckBox)row.FindControl("chkRow");
                TextBox paid = (TextBox)row.FindControl("txtgvpaid");
                TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
                TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
                TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
                TextBox Pending = (TextBox)row.FindControl("txtgvpending");
                TextBox Note = (TextBox)row.FindControl("txtgvNote");
                Label payable = (Label)row.FindControl("lblpayable");
                Label Totalamount = (Label)row.FindControl("lbltotal");
                TextBox Reced = (TextBox)row.FindControl("lblratee");
                Label lblfooterpaid = (Label)Gvreceipt.FooterRow.FindControl("footerpaid");

                if (chckheader.Checked == true)
                {
                    var paidval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text == "" ? "0" : Reced.Text);

                    var pendingval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text == "" ? "0" : Reced.Text) - Convert.ToDouble(paidval.ToString());

                    var Recived = ViewState["Creditnotes"].ToString();


                    chckrow.Checked = true;
                    paid.Enabled = true;
                    TDS.Enabled = true;
                    Adjust.Enabled = true;
                    Excess.Enabled = true;
                    Pending.Enabled = true;
                    Note.Enabled = true;
                    paid.Text = paidval.ToString();
                    Totalamount.Text = payable.Text;
                    Pending.Text = pendingval.ToString();
                    SumOfTotalFooter += Convert.ToDouble(paidval.ToString());
                    Reced.Text = Recived.ToString();
                }
                else
                {
                    chckrow.Checked = false;
                    paid.Enabled = false;
                    TDS.Enabled = false;
                    Adjust.Enabled = false;
                    Excess.Enabled = false;
                    Pending.Enabled = false;
                    Note.Enabled = false;
                    paid.Text = "0";
                    TDS.Text = "0";
                    Adjust.Text = "0";
                    Excess.Text = "0";
                    Pending.Text = "0";
                    Note.Text = string.Empty;
                    Totalamount.Text = "0";
             


                }
                lblfooterpaid.Text = SumOfTotalFooter.ToString();
                lblFooterPaidVal.Text = SumOfTotalFooter.ToString();
                 
            }


            //// Find the header checkbox
            //CheckBox chkheader = Gvreceipt.HeaderRow.FindControl("chkheader") as CheckBox;

            //// Iterate through all the rows in the GridView
            //foreach (GridViewRow row in Gvreceipt.Rows)
            //{
            //    // Find the CheckBox in each row
            //    CheckBox chkRow = row.FindControl("chkRow") as CheckBox;

            //    // Set the Checked property of the CheckBox in each row to the value of the header CheckBox
            //    chkRow.Checked = chkheader.Checked;

            //    // Manually trigger the chkRow_CheckedChanged event for each row
            //    chkRow_CheckedChanged(chkRow, EventArgs.Empty);
            //}
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    protected void ddltoaccountName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddltoaccountName.Text == "Bank" || ddltoaccountName.Text == "CC" || ddltoaccountName.Text == "OD" || ddltoaccountName.Text == "OD")
        {
            txtbankname.Text = "HDFC BANK LIMITED";
        }
        else
        {
            txtbankname.Text = "Account";
        }
    }

    protected void txtamount_TextChanged(object sender, EventArgs e)
    {
        if (ddlAgainst.Text == "Invoice")
        {
            GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
            Label footertotal = (Label)Gvreceipt.FooterRow.FindControl("footerpaid");
            if (Convert.ToDouble(txtamount.Text) <= Convert.ToDouble(footertotal.Text))
            {
                btnsubmit.Enabled = true;
            }
            //else if (Convert.ToDouble(txtamount.Text) != Convert.ToDouble(footertotal.Text))
            //{
            //    btnsubmit.Enabled = false;
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Amount Is Greater Than Payable Amount');", true);
            //}
            if (Convert.ToDouble(txtamount.Text) != Convert.ToDouble(lblFooterPaidVal.Text))
            {
                btnsubmit.Enabled = false;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Amount & Receipt Amount Didnt Match. Kindly Check Both Amounts..!!');", true);

            }
            else
            {
                btnsubmit.Enabled = true;
            }
        }
    }

    protected void txtPartyName_TextChanged(object sender, EventArgs e)
    {
        //con.Open();
        ////SqlCommand cmddtt = new SqlCommand("SELECT Max( Amount) FROM [tblReceiptHdr] WHERE Partyname='" + dt.Rows[0]["Partyname"].ToString() + "' AND Against='Advance'", con);
        //SqlCommand cmddtt = new SqlCommand("select SUM(cast(Amount as float))from [tblReceiptHdr] where Against='Advance' AND Partyname='"+txtPartyName.Text+"'", con);
        //Object AdvAmt = cmddtt.ExecuteScalar();
        ////var FnlAdv = AdvAmt == null ? "0" : AdvAmt.ToString();
        //var FnlAdv = AdvAmt == null || AdvAmt == "" ? "0" : AdvAmt.ToString();
        //lblpendingAdvance.Text = "Received Advance is : " + FnlAdv;
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("Select SUM(cast(Amount as float)) As Amount from [tblReceiptHdr] where Against='Advance' AND Partyname='" + txtPartyName.Text + "'", con);
        sad.Fill(dt);
        var FnlAdv = dt.Rows[0]["Amount"].ToString() == null || dt.Rows[0]["Amount"].ToString() == "" ? "0" : dt.Rows[0]["Amount"].ToString().ToString();
        lblpendingAdvance.Text = "Received Advance is : " + FnlAdv;
        //txtPartyName.Text = dt.Rows[0]["Partyname"].ToString();
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("ReceiptList.aspx");
    }

    Double SumOfPaidFooter;
    protected void txttds_SelectedIndexChanged(object sender, EventArgs e)
    {
        getTDScalculations();
    }

    public void getTDScalculations()
    {
        try
        {

            foreach (GridViewRow row in Gvreceipt.Rows)
            {
                Label FinalBasic = (Label)row.FindControl("lblfinalbasic");
                TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
                TextBox txtgvpaid = (TextBox)row.FindControl("txtgvpaid");
                Label lbltotal = (Label)row.FindControl("lbltotal");

                CheckBox Chkrow = (CheckBox)row.FindControl("chkRow");
                if (Chkrow.Checked)
                {
                    Double TDSs = Convert.ToDouble(FinalBasic.Text) * Convert.ToDouble(txttds.Text) / 100;
                    TDS.Text = TDSs.ToString("#.00");

                    var tot = Convert.ToDecimal(txtgvpaid.Text) - Convert.ToDecimal(TDS.Text);

                    txtgvpaid.Text = tot.ToString();
                    lbltotal.Text = tot.ToString();

                }

                TextBox paid = (TextBox)row.FindControl("txtgvpaid");
                // TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
                TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
                TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
                TextBox Pending = (TextBox)row.FindControl("txtgvpending");
                TextBox Note = (TextBox)row.FindControl("txtgvNote");
                Label payable = (Label)row.FindControl("lblpayable");
                Label Totalamount = (Label)row.FindControl("lbltotal");
                Label totalfooter = (Label)Gvreceipt.FooterRow.FindControl("footertotal");
                Label footerpaid = (Label)Gvreceipt.FooterRow.FindControl("footerpaid");
                CheckBox chk = (CheckBox)row.FindControl("chkRow");

                TextBox Reced = (TextBox)row.FindControl("lblratee");

                Label lblfooterpaid = (Label)Gvreceipt.FooterRow.FindControl("footerpaid");

                var tdsval = TDS.Text == "" ? "0" : TDS.Text;

                var paidval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text) - Convert.ToDouble(tdsval);

                if (chk != null & chk.Checked)
                {

                    paid.Enabled = true;
                    TDS.Enabled = true;
                    Adjust.Enabled = true;
                    Excess.Enabled = true;
                    Pending.Enabled = true;
                    Note.Enabled = true;
                    paid.Text = paidval.ToString();
                    Totalamount.Text = (Convert.ToDecimal(payable.Text) - Convert.ToDecimal(TDS.Text == "" ? "0" : TDS.Text)).ToString();
                    Calculation(row);
                    //Pending.Text = finalpend.ToString();
                    SumOfTotalFooter += Convert.ToDouble(Totalamount.Text);
                    SumOfPaidFooter += Convert.ToDouble(paid.Text);

                }
                else
                {
                    paid.Enabled = false;
                    TDS.Enabled = false;
                    Adjust.Enabled = false;
                    Excess.Enabled = false;
                    Pending.Enabled = false;
                    Note.Enabled = false;
                    paid.Text = "0";
                    TDS.Text = "0";
                    Adjust.Text = "0";
                    Excess.Text = "0";
                    Pending.Text = "0";
                    Note.Text = string.Empty;
                    Totalamount.Text = "0";
                }
                totalfooter.Text = Math.Round(SumOfTotalFooter).ToString();
                footerpaid.Text = Math.Round(SumOfPaidFooter).ToString();
            }
            //GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
            //Calculation(row);
        }
        catch (Exception)
        {

            throw;
        }
    }
}