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
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Net.Mail;
using System.Net.Mime;
using iTextSharp.tool.xml;


public partial class Purchase_Payment : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    string id;
    string checkinvooice;

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
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter sad1 = new SqlDataAdapter("select * from tblPaymentHdrs where Id='" + id + "'", con);
            sad1.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtPartyName.Text = dt.Rows[0]["PartyName"].ToString();
                ddlAgainst.Text = dt.Rows[0]["Against"].ToString();

                Session["AgainstVal"] = dt.Rows[0]["Against"].ToString();

                ddltoaccountName.Text = dt.Rows[0]["FromAccountName"].ToString();
                txtbankname.Text = dt.Rows[0]["BankName"].ToString();
                ddltransactionmode.Text = dt.Rows[0]["TransactionMode"].ToString();
                txtmodedescription.Text = dt.Rows[0]["ModeDescription"].ToString();
                txtdate.Text = dt.Rows[0]["PostDate"].ToString();
                txtamount.Text = dt.Rows[0]["Amount"].ToString();
                txtremark.Text = dt.Rows[0]["TransactionRemark"].ToString();
                txttds.Text = dt.Rows[0]["ApplyTDS"].ToString();
                txtbasic.Text = dt.Rows[0]["Basic"].ToString();
                if (ddlAgainst.Text == "Purchase Bill")
                {
                    BillDetailsload();
                    Session["Adv"] = "0";
                }else  if (ddlAgainst.Text == "Expenses")
                {
                    rowtype.Visible = true;
                }
                else
                {
                    Gvpayment.Visible = false;
                }

                //new Advance Show 2-7-23
                con.Open();
                SqlCommand cmddtt = new SqlCommand("SELECT Max( Amount) FROM [tblPaymentHdrs] WHERE Partyname='" + dt.Rows[0]["Partyname"].ToString() + "' AND Against='Advance'", con);
                Object AdvAmt = cmddtt.ExecuteScalar();
                var FnlAdv = AdvAmt == null ? "0" : AdvAmt.ToString();
                lblpendingAdvance.Text = "Pending Advance is : " + FnlAdv;

            }

        }
        catch (Exception)
        {

            throw;
        }


    }

    protected void BillDetailsload()
    {
        try
        {
            //Bind GridView
            DataTable dtbindgv = new DataTable();
            //SqlDataAdapter sad2 = new SqlDataAdapter("select * from tblPaymentDtls where HeaderId='" + id + "'", con);
            SqlDataAdapter sad2 = new SqlDataAdapter("SELECT [Id],[HeaderId],[BillNo] as SupplierBillNo,[BillDate],[Basic],[GrandTotal],[Recvd],[WO],[Paid],[TDS],[Adjust],[Excess],[Pending],[Total],[Note],[Chk],[SupplierName] FROM tblPaymentDtls where HeaderId='" + id + "'", con);
            sad2.Fill(dtbindgv);
            Gvpayment.DataSource = dtbindgv;
            Gvpayment.DataBind();
            Gvpayment.EmptyDataText = "Record Not Show";
        }
        catch (Exception)
        {

            throw;
        }
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
                com.CommandText = "select DISTINCT Vendorname from tbl_VendorMaster   where " + "Vendorname like @Search + '%'  ";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> SupplierName = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        SupplierName.Add(sdr["Vendorname"].ToString());
                    }
                }
                con.Close();
                return SupplierName;
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
                com.CommandText = "select DISTINCT BankName from tblBankMaster where " + "BankName like  '%'+ @Search + '%' ";

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
                if (ddlAgainst.Text == "Purchase Bill")
                {
                    Gvpayment.Visible = true;
                    DataTable dtagainst = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from tblPurchaseBillHdr where IsPaid is null and SupplierName='" + txtPartyName.Text + "'", con);//
                    sad.Fill(dtagainst);
                    if (dtagainst.Rows.Count > 0)
                    {
                        Gvpayment.DataSource = dtagainst;
                        Gvpayment.DataBind();
                        Gvpayment.EmptyDataText = "Record Not Found";
                        //txtamount.Text = dtagainst.Rows[0]["GrandTotal"].ToString();

                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Bill Record Not Found...Already Paid');window.location.href='Payment.aspx';", true);

                    }
                    lblshowmsg.Visible = false;
                    Session["Adv"] = "1";
                    rowtype.Visible = false;
                }
                else
                if (ddlAgainst.Text == "Expenses")
                {
                    rowtype.Visible = true;
                }
                else
                {
                    rowtype.Visible = false;
                    lblshowmsg.Visible = true;
                    Gvpayment.Visible = false;
                    lblshowmsg.Text = "No Information Found";

                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please Enter Party Name !!!');", true);
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void chkRow_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in Gvpayment.Rows)
        {
            TextBox paid = (TextBox)row.FindControl("txtgvpaid");
            TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
            TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
            TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
            TextBox Pending = (TextBox)row.FindControl("txtgvpending");
            TextBox Note = (TextBox)row.FindControl("txtgvNote");
            Label payable = (Label)row.FindControl("lblpayable");
            Label Totalamount = (Label)row.FindControl("lbltotal");
            Label totalfooter = (Label)Gvpayment.FooterRow.FindControl("footertotal");
            Label footerpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");
            CheckBox chk = (CheckBox)row.FindControl("chkRow");

            TextBox Reced = (TextBox)row.FindControl("lblrate");

            Label lblfooterpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");

            var tdsval = TDS.Text == "" ? "0" : TDS.Text;

            var paidval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text) - Convert.ToDouble(tdsval);

            //var pendingval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text) - Convert.ToDouble(paidval);

            //var pendn = Math.Round(pendingval).ToString();

            //var finalpend = Convert.ToDouble(tdsval) - Convert.ToDouble(pendn);


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
                lblFooterPaidVal.Text += Convert.ToDouble(paid.Text);

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
            totalfooter.Text = SumOfTotalFooter.ToString();
            footerpaid.Text = SumOfPaidFooter.ToString();
            lblFooterPaidVal.Text = SumOfPaidFooter.ToString();
        }
    }

    Double SumOfTotalFooter;
    Double SumOfPaidFooter;
    protected void Calculation(GridViewRow row)
    {
        Label FinalBasic = (Label)row.FindControl("lblfinalbasic");
        TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
        CheckBox Chkrow = (CheckBox)row.FindControl("chkRow");
        Double TDSs = Convert.ToDouble(FinalBasic.Text) * Convert.ToDouble(txttds.Text) / 100;
        TDS.Text = TDSs.ToString();

        Label lblPayable = (Label)row.FindControl("lblpayable");

        Label lbltotal = (Label)row.FindControl("lbltotal");
        TextBox txtTDS = (TextBox)row.FindControl("txtgvTDS");
        TextBox txtadjust = (TextBox)row.FindControl("txtgvadjust");
        TextBox txtexcess = (TextBox)row.FindControl("txtgvExcess");
        TextBox txtpending = (TextBox)row.FindControl("txtgvpending");
        TextBox txtpaid = (TextBox)row.FindControl("txtgvpaid");
        TextBox Reced = (TextBox)row.FindControl("lblrate");

        txtpending.Text = lblPayable.ToString();

        var paidtotal = Convert.ToDouble(txtpaid.Text);
        double payAmt = Convert.ToDouble(txtpaid.Text) + Convert.ToDouble(txtTDS.Text);
        double Paid = Convert.ToDouble(paidtotal);
        if (txtpaid.Text == lblPayable.Text)
        {
            Double lbltotal1 = Paid - Convert.ToDouble(txtTDS.Text);
            txtpaid.Text = Math.Round(lbltotal1).ToString();
            lbltotal.Text = Math.Round(lbltotal1).ToString();
            txtpending.Text = (Convert.ToDouble(lblPayable.Text) - Paid).ToString("#0.00");
        }
        else
        {
            lbltotal.Text = Math.Round(paidtotal).ToString();
            txtpending.Text = (Convert.ToDouble(lblPayable.Text) - Convert.ToDouble(Reced.Text) - payAmt).ToString("#0.00");
        }

    }

    decimal paidval = 0;
    decimal totval = 0;
    protected void txtgvpaid_TextChanged(object sender, EventArgs e)
    {

        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        Calculation(row);

        foreach (GridViewRow g1 in Gvpayment.Rows)
        {
            string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
            paidval += Convert.ToDecimal(paid);

            string tot = (g1.FindControl("lbltotal") as Label).Text;
            totval += Convert.ToDecimal(tot);
        }
        Label lblfinalbasic = (Label)Gvpayment.FooterRow.FindControl("footerpaid");
        lblfinalbasic.Text = paidval.ToString();

        Label footertotal = (Label)Gvpayment.FooterRow.FindControl("footertotal");
        footertotal.Text = totval.ToString();
        lblFooterPaidVal.Text = totval.ToString();

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

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("Payment.aspx");
    }

    DateTime date = DateTime.Now;
    protected void btnsubmit_Click(object sender, EventArgs e)
    {
        try
        {
            int id;
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
                        SqlDataAdapter sadupds = new SqlDataAdapter("select * from tblPaymentHdrs where Id='" + Session["UPID"].ToString() + "'", con);
                        sadupds.Fill(dttupdateadv);

                        if (dttupdateadv.Rows.Count > 0)
                        {
                            Advanceamt = dttupdateadv.Rows[0]["Amount"].ToString();
                        }

                        var MinusPlusAdvn = Convert.ToDouble(Advanceamt) - Convert.ToDouble(txtamount.Text);
                        SqlCommand cmduptt = new SqlCommand("update tblPaymentHdrs set Amount='" + Math.Round(MinusPlusAdvn).ToString() + "' where Id='" + Session["UPID"].ToString() + "'", con);
                        con.Open();
                        cmduptt.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        DateTime date = DateTime.Now;
                        SqlCommand cmd = new SqlCommand("SP_PaymentHdrs", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                        cmd.Parameters.AddWithValue("@FromAccountName", ddltoaccountName.Text);
                        cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
                        cmd.Parameters.AddWithValue("@TransactionMode", ddltransactionmode.Text);
                        cmd.Parameters.AddWithValue("@ModeDescription", txtmodedescription.Text);
                        cmd.Parameters.AddWithValue("@PostDate", txtdate.Text);
                        cmd.Parameters.AddWithValue("@Type", ddltype.SelectedItem.Text);

                        cmd.Parameters.AddWithValue("@Against", ddlAgainst.Text);
                        cmd.Parameters.AddWithValue("@Id", hidden1.Value);
                        cmd.Parameters.AddWithValue("@Amount", txtamount.Text);
                        cmd.Parameters.AddWithValue("@TransactionRemark", txtremark.Text);
                        cmd.Parameters.AddWithValue("@ApplyTDS", txttds.Text);
                        cmd.Parameters.AddWithValue("@Basic", txtbasic.Text);
                        //cmd.Parameters.AddWithValue("@UpdatedBy", Session["ProductionName"].ToString());
                        cmd.Parameters.AddWithValue("@UpdatedBy", Session["Username"].ToString());
                        cmd.Parameters.AddWithValue("@UpdatedOn", date);

                        cmd.Parameters.Add("@Idd", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.AddWithValue("@Action", "Update");
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        //id = Convert.ToInt32(cmd.Parameters["@Idd"].Value);

                        SqlCommand CmdDelete = new SqlCommand("DELETE FROM tblPaymentDtls WHERE HeaderID=@HeaderID", con);
                        CmdDelete.Parameters.AddWithValue("@HeaderID", hidden1.Value);
                        con.Open();
                        CmdDelete.ExecuteNonQuery();
                        con.Close();

                        if (ddlAgainst.Text == "Purchase Bill")
                        {
                            foreach (GridViewRow g1 in Gvpayment.Rows)
                            {
                                bool chk = (g1.FindControl("chkRow") as CheckBox).Checked;
                                string Billno = (g1.FindControl("lblBillno") as Label).Text;
                                string Billdate = (g1.FindControl("lblBillDate") as Label).Text;
                                string payable = (g1.FindControl("lblpayable") as Label).Text;
                                string Recvd = (g1.FindControl("lblrate") as TextBox).Text;
                                string WO = (g1.FindControl("lblWO") as TextBox).Text;
                                string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
                                string TDS = (g1.FindControl("txtgvTDS") as TextBox).Text;
                                string Adjust = (g1.FindControl("txtgvadjust") as TextBox).Text;
                                string Excess = (g1.FindControl("txtgvExcess") as TextBox).Text;
                                string Pending = (g1.FindControl("txtgvpending") as TextBox).Text;
                                string Total = (g1.FindControl("lbltotal") as Label).Text;
                                string Notes = (g1.FindControl("txtgvNote") as TextBox).Text;
                                string lblbasic = (g1.FindControl("lblfinalbasic") as Label).Text;
                                decimal Paid1 = Convert.ToDecimal(paid.ToString());
                                decimal TDS1 = Convert.ToDecimal(TDS.ToString());

                                SqlCommand cmd1 = new SqlCommand("SP_PaymentDtls", con);
                                cmd1.CommandType = CommandType.StoredProcedure;
                                cmd1.Parameters.AddWithValue("@HeaderId", hidden1.Value);
                                cmd1.Parameters.AddWithValue("@BillNo", Billno);
                                cmd1.Parameters.AddWithValue("@Billdate", Billdate);
                                cmd1.Parameters.AddWithValue("@Basic", lblbasic);
                                cmd1.Parameters.AddWithValue("@GrandTotal", payable);

                                var Recd = Recvd == "" ? "0" : Recvd;
                                var totRece = Convert.ToDouble(Recd) + Convert.ToDouble(paid);
                                cmd1.Parameters.AddWithValue("@Recvd", totRece);
                                cmd1.Parameters.AddWithValue("@WO", WO);
                                cmd1.Parameters.AddWithValue("@Paid", Paid1);
                                cmd1.Parameters.AddWithValue("@TDS", TDS1);
                                cmd1.Parameters.AddWithValue("@Adjust", Adjust);
                                cmd1.Parameters.AddWithValue("@Excess", Excess);
                                cmd1.Parameters.AddWithValue("@Pending", Math.Round(Convert.ToDouble(Pending)).ToString());
                                cmd1.Parameters.AddWithValue("@Total", Total);
                                cmd1.Parameters.AddWithValue("@Note", Notes);
                                cmd1.Parameters.AddWithValue("@Chk", chk);
                                cmd1.Parameters.AddWithValue("@SupplierName", txtPartyName.Text);
                                if (chk == true)
                                {
                                    cmd1.Parameters.AddWithValue("@Action", "Insert");
                                }
                                con.Open();
                                cmd1.ExecuteNonQuery();
                                con.Close();

                                if (chk == true)
                                {
                                    Label lblfooterpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");

                                    con.Open();
                                    SqlCommand cmddtt = new SqlCommand("select MAX(Id) from tblPaymentDtls where BillNo='" + Billno + "'", con);
                                    Object mxid = cmddtt.ExecuteScalar();

                                    SqlCommand cmddtot = new SqlCommand("select sum(cast(Total as float)) from tblPaymentDtls where BillNo='" + Billno + "'", con);
                                    Object TOTalamt = cmddtot.ExecuteScalar();

                                    SqlCommand cmddgtot = new SqlCommand("select GrandTotal from tblPaymentDtls where Id='" + mxid.ToString() + "'", con);
                                    Object GTOTalamt = cmddgtot.ExecuteScalar();

                                    var Payble = payable;//lblfooterpaid.Text;
                                    var tot = TOTalamt == null ? "0" : TOTalamt.ToString();
                                    var Gtot = GTOTalamt == null ? "0" : GTOTalamt.ToString();

                                    var totminus = Convert.ToDecimal(Gtot) - (Math.Round(Paid1 + TDS1));

                                    if (totminus == 0)
                                    {
                                        SqlCommand cmdpaid = new SqlCommand("update tblPurchaseBillHdr set IsPaid=1 where SupplierBillNo='" + Billno + "'", con);
                                        cmdpaid.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        SqlCommand cmdpaid = new SqlCommand("update tblPurchaseBillHdr set IsPaid=null where SupplierBillNo='" + Billno + "'", con);
                                        cmdpaid.ExecuteNonQuery();
                                    }
                                    con.Close();
                                }
                            }
                        }
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data updated Sucessfully');window.location.href='PaymentList.aspx';", true);
                        Session["AgainstVal"] = null;
                    }
                }
                else
                {

                    DateTime date = DateTime.Now;
                    SqlCommand cmd = new SqlCommand("SP_PaymentHdrs", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
                    cmd.Parameters.AddWithValue("@FromAccountName", ddltoaccountName.Text);
                    cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
                    cmd.Parameters.AddWithValue("@TransactionMode", ddltransactionmode.Text);
                    cmd.Parameters.AddWithValue("@ModeDescription", txtmodedescription.Text);
                    cmd.Parameters.AddWithValue("@PostDate", txtdate.Text);

                    cmd.Parameters.AddWithValue("@Against", ddlAgainst.Text);
                    cmd.Parameters.AddWithValue("@Id", hidden1.Value);
                    cmd.Parameters.AddWithValue("@Amount", txtamount.Text);
                    cmd.Parameters.AddWithValue("@TransactionRemark", txtremark.Text);
                    cmd.Parameters.AddWithValue("@ApplyTDS", txttds.Text);
                    cmd.Parameters.AddWithValue("@Basic", txtbasic.Text);
                    //cmd.Parameters.AddWithValue("@UpdatedBy", Session["ProductionName"].ToString());
                    cmd.Parameters.AddWithValue("@UpdatedBy", Session["Username"].ToString());
                    cmd.Parameters.AddWithValue("@UpdatedOn", date);

                    cmd.Parameters.Add("@Idd", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@Action", "Update");
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //id = Convert.ToInt32(cmd.Parameters["@Idd"].Value);
                    if (ddlAgainst.Text == "Purchase Bill")
                    {
                        foreach (GridViewRow g1 in Gvpayment.Rows)
                        {
                            bool chk = (g1.FindControl("chkRow") as CheckBox).Checked;
                            string Billno = (g1.FindControl("lblBillno") as Label).Text;
                            string Billdate = (g1.FindControl("lblBillDate") as Label).Text;
                            string payable = (g1.FindControl("lblpayable") as Label).Text;
                            string Recvd = (g1.FindControl("lblrate") as TextBox).Text;
                            string WO = (g1.FindControl("lblWO") as TextBox).Text;
                            string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
                            string TDS = (g1.FindControl("txtgvTDS") as TextBox).Text;
                            string Adjust = (g1.FindControl("txtgvadjust") as TextBox).Text;
                            string Excess = (g1.FindControl("txtgvExcess") as TextBox).Text;
                            string Pending = (g1.FindControl("txtgvpending") as TextBox).Text;
                            string Total = (g1.FindControl("lbltotal") as Label).Text;
                            string Notes = (g1.FindControl("txtgvNote") as TextBox).Text;
                            string lblbasic = (g1.FindControl("lblfinalbasic") as Label).Text;
                            decimal Paid1 = Convert.ToDecimal(paid.ToString());
                            decimal TDS1 = Convert.ToDecimal(TDS.ToString());

                            SqlCommand cmd1 = new SqlCommand("SP_PaymentDtls", con);
                            cmd1.CommandType = CommandType.StoredProcedure;
                            cmd1.Parameters.AddWithValue("@HeaderId", hidden1.Value);
                            cmd1.Parameters.AddWithValue("@BillNo", Billno);
                            cmd1.Parameters.AddWithValue("@Billdate", Billdate);
                            cmd1.Parameters.AddWithValue("@Basic", lblbasic);
                            cmd1.Parameters.AddWithValue("@GrandTotal", payable);

                            var Recd = Recvd == "" ? "0" : Recvd;
                            var totRece = Convert.ToDouble(Recd) + Convert.ToDouble(paid);
                            cmd1.Parameters.AddWithValue("@Recvd", totRece);
                            cmd1.Parameters.AddWithValue("@WO", WO);
                            cmd1.Parameters.AddWithValue("@Paid", Paid1);
                            cmd1.Parameters.AddWithValue("@TDS", TDS1);
                            cmd1.Parameters.AddWithValue("@Adjust", Adjust);
                            cmd1.Parameters.AddWithValue("@Excess", Excess);
                            cmd1.Parameters.AddWithValue("@Pending", Math.Round(Convert.ToDouble(Pending)).ToString());
                            cmd1.Parameters.AddWithValue("@Total", Total);
                            cmd1.Parameters.AddWithValue("@Note", Notes);
                            cmd1.Parameters.AddWithValue("@Chk", chk);
                            cmd1.Parameters.AddWithValue("@SupplierName", txtPartyName.Text);
                            if (chk == true)
                            {
                                cmd1.Parameters.AddWithValue("@Action", "Insert");
                            }
                            con.Open();
                            cmd1.ExecuteNonQuery();
                            con.Close();

                            if (chk == true)
                            {
                                Label lblfooterpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");

                                con.Open();
                                SqlCommand cmddtt = new SqlCommand("select MAX(Id) from tblPaymentDtls where BillNo='" + Billno + "'", con);
                                Object mxid = cmddtt.ExecuteScalar();

                                SqlCommand cmddtot = new SqlCommand("select sum(cast(Total as float)) from tblPaymentDtls where BillNo='" + Billno + "'", con);
                                Object TOTalamt = cmddtot.ExecuteScalar();

                                SqlCommand cmddgtot = new SqlCommand("select GrandTotal from tblPaymentDtls where Id='" + mxid.ToString() + "'", con);
                                Object GTOTalamt = cmddgtot.ExecuteScalar();

                                var Payble = payable;//lblfooterpaid.Text;
                                var tot = TOTalamt == null ? "0" : TOTalamt.ToString();
                                var Gtot = GTOTalamt == null ? "0" : GTOTalamt.ToString();

                                var totminus = Convert.ToDecimal(Gtot) - (Math.Round(Paid1 + TDS1));

                                if (totminus == 0)
                                {
                                    SqlCommand cmdpaid = new SqlCommand("update tblPurchaseBillHdr set IsPaid=1 where SupplierBillNo='" + Billno + "'", con);
                                    cmdpaid.ExecuteNonQuery();
                                }
                                else
                                {
                                    SqlCommand cmdpaid = new SqlCommand("update tblPurchaseBillHdr set IsPaid=null where SupplierBillNo='" + Billno + "'", con);
                                    cmdpaid.ExecuteNonQuery();
                                }
                                con.Close();
                            }
                        }
                    }
                    Session["AgainstVal"] = null;
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data updated Sucessfully');window.location.href='PaymentList.aspx';", true);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }

        ////new line
        Response.Redirect("PaymentList.aspx");
    }


    public void fnInsert()
    {
        try
        {
            int id;
            SqlCommand cmd = new SqlCommand("SP_PaymentHdrs", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PartyName", txtPartyName.Text);
            cmd.Parameters.AddWithValue("@FromAccountName", ddltoaccountName.Text);
            cmd.Parameters.AddWithValue("@BankName", txtbankname.Text);
            cmd.Parameters.AddWithValue("@TransactionMode", ddltransactionmode.Text);
            cmd.Parameters.AddWithValue("@ModeDescription", txtmodedescription.Text);
            cmd.Parameters.AddWithValue("@PostDate", txtdate.Text.Trim());
            cmd.Parameters.AddWithValue("@Against", ddlAgainst.Text);
            cmd.Parameters.AddWithValue("@Amount", txtamount.Text);
            cmd.Parameters.AddWithValue("@Type", ddltype.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@TransactionRemark", txtremark.Text);
            cmd.Parameters.AddWithValue("@ApplyTDS", txttds.Text);
            cmd.Parameters.AddWithValue("@Basic", txtbasic.Text);
            //cmd.Parameters.AddWithValue("@CreatedBy", Session["ProductionName"].ToString());
            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
            cmd.Parameters.AddWithValue("@CreatedOn", date);
            cmd.Parameters.Add("@Idd", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@isdeleted", '0');
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            id = Convert.ToInt32(cmd.Parameters["@Idd"].Value);
            if (ddlAgainst.Text == "Purchase Bill")
            {
                foreach (GridViewRow g1 in Gvpayment.Rows)
                {
                    bool chk = (g1.FindControl("chkRow") as CheckBox).Checked;
                    string Billno = (g1.FindControl("lblBillno") as Label).Text;
                    string Billdate = (g1.FindControl("lblBillDate") as Label).Text;
                    string payable = (g1.FindControl("lblpayable") as Label).Text;
                    string Recvd = (g1.FindControl("lblrate") as TextBox).Text;
                    string WO = (g1.FindControl("lblWO") as TextBox).Text;
                    string paid = (g1.FindControl("txtgvpaid") as TextBox).Text;
                    string TDS = (g1.FindControl("txtgvTDS") as TextBox).Text;
                    string Adjust = (g1.FindControl("txtgvadjust") as TextBox).Text;
                    string Excess = (g1.FindControl("txtgvExcess") as TextBox).Text;
                    string Pending = (g1.FindControl("txtgvpending") as TextBox).Text;
                    string Total = (g1.FindControl("lbltotal") as Label).Text;
                    string Notes = (g1.FindControl("txtgvNote") as TextBox).Text;
                    string lblbasic = (g1.FindControl("lblfinalbasic") as Label).Text;
                    decimal Paid1 = Convert.ToDecimal(paid.ToString());
                    decimal TDS1 = Convert.ToDecimal(TDS.ToString());

                    SqlCommand cmd1 = new SqlCommand("SP_PaymentDtls", con);
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@HeaderId", id);
                    cmd1.Parameters.AddWithValue("@BillNo", Billno);
                    cmd1.Parameters.AddWithValue("@Billdate", Billdate);
                    cmd1.Parameters.AddWithValue("@Basic", lblbasic);
                    cmd1.Parameters.AddWithValue("@GrandTotal", payable);

                    var Recd = Recvd == "" ? "0" : Recvd;
                    var totRece = Convert.ToDouble(Recd) + Convert.ToDouble(paid);
                    cmd1.Parameters.AddWithValue("@Recvd", totRece);
                    cmd1.Parameters.AddWithValue("@WO", WO);
                    cmd1.Parameters.AddWithValue("@Paid", Paid1);
                    cmd1.Parameters.AddWithValue("@TDS", TDS1);
                    cmd1.Parameters.AddWithValue("@Adjust", Adjust);
                    cmd1.Parameters.AddWithValue("@Excess", Excess);
                    cmd1.Parameters.AddWithValue("@Pending", Math.Round(Convert.ToDouble(Pending)).ToString());
                    cmd1.Parameters.AddWithValue("@Total", Total);
                    cmd1.Parameters.AddWithValue("@Note", Notes);
                    cmd1.Parameters.AddWithValue("@Chk", chk);
                    cmd1.Parameters.AddWithValue("@SupplierName", txtPartyName.Text);
                    if (chk == true)
                    {
                        cmd1.Parameters.AddWithValue("@Action", "Insert");
                    }
                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();

                    if (chk == true)
                    {
                        Label lblfooterpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");

                        con.Open();
                        SqlCommand cmddtt = new SqlCommand("select MAX(Id) from tblPaymentDtls where BillNo='" + Billno + "'", con);
                        Object mxid = cmddtt.ExecuteScalar();

                        SqlCommand cmddtot = new SqlCommand("select sum(cast(Total as float)) from tblPaymentDtls where BillNo='" + Billno + "'", con);
                        Object TOTalamt = cmddtot.ExecuteScalar();

                        SqlCommand cmddgtot = new SqlCommand("select GrandTotal from tblPaymentDtls where Id='" + mxid.ToString() + "'", con);
                        Object GTOTalamt = cmddgtot.ExecuteScalar();

                        var Payble = payable;//lblfooterpaid.Text;
                        var tot = TOTalamt == null ? "0" : TOTalamt.ToString();
                        var Gtot = GTOTalamt == null ? "0" : GTOTalamt.ToString();

                        var totminus = Convert.ToDecimal(Gtot) - (Math.Round(Paid1 + TDS1));

                        if (totminus == 0)
                        {
                            SqlCommand cmdpaid = new SqlCommand("update tblPurchaseBillHdr set IsPaid=1 where SupplierBillNo='" + Billno + "'", con);
                            cmdpaid.ExecuteNonQuery();
                        }
                        else
                        {
                        }
                        con.Close();
                    }
                }

                DataTable dt546665 = new DataTable();
                SqlDataAdapter sadparticular = new SqlDataAdapter("select * from tblPaymentDtls where HeaderId='" + id + "'", con);
                sadparticular.Fill(dt546665);
                if (dt546665.Rows.Count > 0)
                {

                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('At Least Select One Record.');", true);
                }
            }

            if (IsSedndMail.Checked == true)
            {
                string subject = "Payment Voucher from Pune Abrasives Pvt. Ltd.";
              //  Send_Mail(id, subject);
            }
            Session["AgainstVal"] = null;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='PaymentList.aspx';", true);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    decimal payable, paid, TDS, Excess, Basic, Adjust, Total, pending = 0; decimal footer = 0;
    protected void Gvpayment_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (btnsubmit.Text == "Update")
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //if (Session["AgainstVal"].ToString() == "Advance")
                //{

                int id = Convert.ToInt32(Gvpayment.DataKeys[e.Row.RowIndex].Values[0]);
                TextBox paid = (TextBox)e.Row.FindControl("txtgvpaid");
                TextBox TDS = (TextBox)e.Row.FindControl("txtgvTDS");
                TextBox Adjust = (TextBox)e.Row.FindControl("txtgvadjust");
                TextBox Excess = (TextBox)e.Row.FindControl("txtgvExcess");
                TextBox Pending = (TextBox)e.Row.FindControl("txtgvpending");
                Label Total = (Label)e.Row.FindControl("lbltotal");
                Label lblbasic = (Label)e.Row.FindControl("lblfinalbasic");
                TextBox Notes = (TextBox)e.Row.FindControl("txtgvNote");
                CheckBox chk = (CheckBox)e.Row.FindControl("chkRow");
                con.Open();
                SqlCommand cmd4525 = new SqlCommand("select * from tblPaymentDtls where Id='" + id + "'", con);
                SqlDataReader dr = cmd4525.ExecuteReader();
                if (dr.Read())
                {

                    paid.Text = dr["Paid"].ToString();
                    lblbasic.Text = dr["Basic"].ToString();
                    TDS.Text = dr["TDS"].ToString();
                    Adjust.Text = dr["Adjust"].ToString();
                    Excess.Text = dr["Excess"].ToString();
                    Pending.Text = dr["Pending"].ToString();
                    Total.Text = dr["Total"].ToString();
                    Notes.Text = dr["Note"].ToString();
                    checkinvooice = dr["Chk"].ToString();
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

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en-IN");
            int id = Convert.ToInt32(Gvpayment.DataKeys[e.Row.RowIndex].Values[0]);
            Label payableAmt = (Label)e.Row.FindControl("lblpayable");

            var pbalevalue = Convert.ToDouble(payableAmt.Text);
            payableAmt.Text = Math.Round(pbalevalue).ToString();

            Label BillNo = (Label)e.Row.FindControl("lblBillno");
            string billno = BillNo.Text;
            CheckBox chk = (CheckBox)e.Row.FindControl("chkRow");
            CheckBox chkheader = (CheckBox)Gvpayment.HeaderRow.FindControl("chkheader");

            if (Session["UPID"] != "")
            {
                con.Open();
                SqlCommand cmdmaxid = new SqlCommand("select Id from tblPurchaseBillHdr where SupplierBillNo='" + billno + "' AND SupplierName='" + txtPartyName.Text + "'", con);
                string idd = cmdmaxid.ExecuteScalar().ToString();

                SqlCommand cmddtl = new SqlCommand("select SUM(CAST(Amount as float)) from tblPurchaseBillDtls where HeaderID='" + idd + "'", con);
                Object TaxAmt = cmddtl.ExecuteScalar() == null ? "0" : cmddtl.ExecuteScalar();
                SqlCommand cmdTc = new SqlCommand("select TransportationCharges from tblPurchaseBillHdr where SupplierBillNo='" + billno + "' AND SupplierName='" + txtPartyName.Text + "'", con);
                Object cmdTcval = cmdTc.ExecuteScalar() == null ? "0" : cmdTc.ExecuteScalar();

                con.Close();
                Label lblBasic = (Label)e.Row.FindControl("lblfinalbasic");

                //var Amt = Convert.ToDecimal(TaxAmt.ToString()) + Convert.ToDecimal(lblBasic.Text==null?"0": lblBasic.Text) + Convert.ToDecimal(cmdTcval.ToString());
                var Amt = Convert.ToDecimal(TaxAmt.ToString()) + Convert.ToDecimal(cmdTcval.ToString());
                var basicvalue = Math.Round(Amt);
                Label lblfinalbasic = (Label)e.Row.FindControl("lblfinalbasic");
                lblfinalbasic.Text = basicvalue.ToString("N2", info);

                Label lblBillno = (Label)e.Row.FindControl("lblBillno");
                SqlCommand cmdmax = new SqlCommand("SELECT Recvd FROM tblPaymentDtls where BillNo='" + lblBillno.Text + "' AND SupplierName='" + txtPartyName.Text + "'", con);
                con.Open();
                Object Recvdval = cmdmax.ExecuteScalar();
                con.Close();

                TextBox lblRecvdd = (TextBox)e.Row.FindControl("lblrate");

                if (Recvdval == null)
                {
                    lblRecvdd.Text = "0";
                }
                else
                {
                    lblRecvdd.Text = Recvdval.ToString();
                }
            }
            else
            {
                SqlCommand cmdmax = new SqlCommand("SELECT min(Pending) FROM tblPaymentDtls  where SupplierName='" + txtPartyName.Text + "' AND BillNo='" + billno + "'", con);
                con.Open();
                Object smpayable = cmdmax.ExecuteScalar();

                SqlCommand cmddtl = new SqlCommand("select SUM(CAST(Amount as float)) from tblPurchaseBillDtls where HeaderID='" + id + "'", con);
                Object TaxAmt = cmddtl.ExecuteScalar();

                SqlCommand cmdTc = new SqlCommand("select TransportationCharges from tblPurchaseBillHdr where Id='" + id + "'", con);
                Object cmdTcval = cmdTc.ExecuteScalar();
                con.Close();

                Label lblBasic = (Label)e.Row.FindControl("lblfinalbasic");

                var bval = lblBasic.Text.Replace(",", "");

                var txval = TaxAmt.ToString() == "" ? "0" : TaxAmt.ToString();

                var Amt = Convert.ToDecimal(txval) + Convert.ToDecimal(bval) + Convert.ToDecimal(cmdTcval.ToString());
                var basicvalue = Math.Round(Amt);
                Label lblfinalbasic = (Label)e.Row.FindControl("lblfinalbasic");
                lblfinalbasic.Text = basicvalue.ToString("N2", info);
                Basic += Decimal.Parse(lblfinalbasic.Text);

                if (smpayable.ToString() == "0.00")
                {
                    payableAmt.Text = "0";
                    chk.Enabled = false;
                    chkheader.Enabled = false;
                    e.Row.Visible = false;
                }

                Label lblBillno = (Label)e.Row.FindControl("lblBillno");
                SqlCommand cmdmaxdd = new SqlCommand("SELECT Recvd FROM tblPaymentDtls where BillNo='" + lblBillno.Text + "'", con);
                con.Open();
                Object Recvdval = cmdmaxdd.ExecuteScalar();
                con.Close();

                TextBox lblRecvdd = (TextBox)e.Row.FindControl("lblrate");

                if (Recvdval == null)
                {
                    lblRecvdd.Text = "0";
                }
                else
                {
                    lblRecvdd.Text = Recvdval.ToString();
                }
            }
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblPgTotal = (Label)e.Row.FindControl("lblpayable");
            payable += Decimal.Parse(lblPgTotal.Text);
        }
        //if (e.Row.RowType == DataControlRowType.Footer)
        //{
        //    Label lblpayablefooter = (Label)e.Row.FindControl("footerpayble");
        //    lblpayablefooter.Text = payable.ToString();
        //    if (payable.ToString() == "0")
        //    {
        //        //CheckBox chk = Gvpayment.FindControl("chkRow") as CheckBox;
        //        //CheckBox chk = (CheckBox)Gvpayment.Rows.FindControl("chkRow");
        //        lblmsg.Text = "Payment Already Paid";
        //        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Payment Already Paid');", true);
        //        btnsubmit.Enabled = false;
        //        //chk.Enabled = false;
        //    }
        //}

        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblpayablefooter = (Label)e.Row.FindControl("footerpayble");
            lblpayablefooter.Text = payable.ToString();
            Label lblfooterpaid = (Label)e.Row.FindControl("footerpaid");
            lblfooterpaid.Text = TDS.ToString();
            lblFooterPaidVal.Text = TDS.ToString();
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
        foreach (GridViewRow row in Gvpayment.Rows)
        {
            CheckBox chckheader = (CheckBox)Gvpayment.HeaderRow.FindControl("chkheader");
            CheckBox chckrow = (CheckBox)row.FindControl("chkRow");
            TextBox paid = (TextBox)row.FindControl("txtgvpaid");
            TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
            TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
            TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
            TextBox Pending = (TextBox)row.FindControl("txtgvpending");
            TextBox Note = (TextBox)row.FindControl("txtgvNote");
            Label payable = (Label)row.FindControl("lblpayable");
            Label Totalamount = (Label)row.FindControl("lbltotal");
            Label totalfooter = (Label)Gvpayment.FooterRow.FindControl("footertotal");
            Label footerpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");
            TextBox Reced = (TextBox)row.FindControl("lblrate");

            Label lblfooterpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");

            var paidval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text);

            var pendingval = Convert.ToDouble(payable.Text) - Convert.ToDouble(Reced.Text) - Convert.ToDouble(paidval);

            if (chckheader.Checked == true)
            {
                chckrow.Checked = true;
                paid.Enabled = true;
                TDS.Enabled = true;
                Adjust.Enabled = true;
                Excess.Enabled = true;
                Pending.Enabled = true;
                Note.Enabled = true;
                paid.Text = Math.Round(paidval).ToString();
                Totalamount.Text = payable.Text;
                Pending.Text = Math.Round(pendingval).ToString();
                Calculation(row);
                SumOfTotalFooter += Convert.ToDouble(Totalamount.Text);
                SumOfPaidFooter += Convert.ToDouble(paid.Text);

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
            totalfooter.Text = Math.Round(SumOfTotalFooter).ToString();
            footerpaid.Text = Math.Round(SumOfPaidFooter).ToString();
            lblFooterPaidVal.Text = Math.Round(SumOfPaidFooter).ToString();
        }
    }

    protected void ddltoaccountName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddltoaccountName.Text == "Bank" || ddltoaccountName.Text == "CC" || ddltoaccountName.Text == "OD" || ddltoaccountName.Text == "OD")
        {
            txtbankname.Text = "HDFC Bank";
        }
        else
        {
            txtbankname.Text = "Shirke Sir";
        }
    }

    protected void txtamount_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlAgainst.Text == "Purchase Bill")
            {
                GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
                Label footertotal = (Label)Gvpayment.FooterRow.FindControl("footerpaid");
                if (Convert.ToDouble(txtamount.Text) <= Convert.ToDouble(footertotal.Text))
                {
                    btnsubmit.Enabled = true;
                }
                //else
                //{
                //    btnsubmit.Enabled = false;
                //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Amount is not match');", true);
                //    txtamount.Focus();

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
        catch (Exception ex)
        {
            throw ex;
        }

    }

    protected void txttds_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            foreach (GridViewRow row in Gvpayment.Rows)
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
                //TextBox TDS = (TextBox)row.FindControl("txtgvTDS");
                TextBox Adjust = (TextBox)row.FindControl("txtgvadjust");
                TextBox Excess = (TextBox)row.FindControl("txtgvExcess");
                TextBox Pending = (TextBox)row.FindControl("txtgvpending");
                TextBox Note = (TextBox)row.FindControl("txtgvNote");
                Label payable = (Label)row.FindControl("lblpayable");
                Label Totalamount = (Label)row.FindControl("lbltotal");
                Label totalfooter = (Label)Gvpayment.FooterRow.FindControl("footertotal");
                Label footerpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");
                CheckBox chk = (CheckBox)row.FindControl("chkRow");

                TextBox Reced = (TextBox)row.FindControl("lblrate");

                Label lblfooterpaid = (Label)Gvpayment.FooterRow.FindControl("footerpaid");

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

    protected void txtPartyName_TextChanged(object sender, EventArgs e)
    {
        try
        {
            BindEmailId();
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void BindEmailId()
    {
        SqlDataAdapter ad = new SqlDataAdapter("select EmailID from tbl_VendorMaster  where Vendorname='" + txtPartyName.Text.Trim() + "' ", con);
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

    Object ToMail;
    protected void SendEmail(string TotalAmt, string CompName, int? ID)
    {
        string Descrption = "We will Raised Payment as per following Details. ";

        string body = this.PopulateBody("Sir/Ma'am", TotalAmt, CompName, Descrption);
        string subject = "Payment Advised.";
        string ToMail11 = "erpweblink@gmail.com"; // For Demo Perpose

        this.SendHtmlFormattedEmail(ToMail11, subject, body, CompName, ID);
    }

    private string PopulateBody(string userName, string TotalAmt, string CompName, string description)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(Server.MapPath("~/SendMailFormat.htm")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        body = body.Replace("{TotalAmt}", TotalAmt);
        body = body.Replace("{CompName}", CompName);
        body = body.Replace("{Description}", description);
        return body;
    }

    protected void SendHtmlFormattedEmail(string recepientEmail, string subject, string body, string CompanyName, int? ID)
    {
        //SendPDFEmail(recepientEmail, subject, body, CompanyName, ID);
    }

    //private void SendPDFEmail(string recepientEmail, string subject, string body, string CompanyName, int? ID)
    //{
    //    using (MailMessage mailMessage = new MailMessage())
    //    {
    //        Byte[] FileBuffer = null;

    //        //Generate PDF
    //        Pdf(ID.ToString());

    //        string filename = "Payment";

    //        FileBuffer = File.ReadAllBytes(Server.MapPath("~/files/") + filename + ".pdf");
    //        mailMessage.Attachments.Add(new Attachment(new MemoryStream(FileBuffer), "" + filename + ".pdf"));

    //        mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["FromMail"]);
    //        mailMessage.Subject = subject;
    //        mailMessage.Body = body;
    //        mailMessage.IsBodyHtml = true;
    //        mailMessage.To.Add(new MailAddress(recepientEmail));
    //        //string ccmail = ConfigurationManager.AppSettings["AdminMailCC"];

    //        SmtpClient smtp = new SmtpClient();
    //        smtp.Host = ConfigurationManager.AppSettings["Host"];
    //        smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
    //        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
    //        NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"];
    //        NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"];
    //        smtp.UseDefaultCredentials = true;
    //        smtp.Credentials = NetworkCred;
    //        smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
    //        smtp.Send(mailMessage);
    //    }
    //}

    //protected void Pdf(string id)
    //{
    //    DataTable Dt = new DataTable();
    //    SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PaymentPDF where Id = '" + id + "'", con);

    //    Da.Fill(Dt);

    //    StringWriter sw = new StringWriter();
    //    StringReader sr = new StringReader(sw.ToString());

    //    Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
    //    //string Path = ;
    //    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "Payment.pdf", FileMode.Create));
    //    //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
    //    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

    //    doc.Open();
    //    string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";


    //    iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

    //    //Resize image depend upon your need

    //    png.ScaleToFit(70, 100);

    //    //For Image Position
    //    png.SetAbsolutePosition(40, 775);
    //    //var document = new Document();

    //    //Give space before image
    //    //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
    //    png.SpacingBefore = 50f;

    //    //Give some space after the image

    //    png.SpacingAfter = 1f;

    //    png.Alignment = Element.ALIGN_LEFT;


    //    doc.Add(png);

    //    if (Dt.Rows.Count > 0)
    //    {
    //        var VoucherDate = DateTime.Now.ToString("dd-MM-yyyy");
    //        string VoucherNo = Dt.Rows[0]["Id"].ToString();
    //        string chequedate = Dt.Rows[0]["postDate"].ToString().TrimEnd("0:0".ToCharArray());
    //        string bankName = Dt.Rows[0]["BankName"].ToString();
    //        string TransactionMode = Dt.Rows[0]["TransactionMode"].ToString();
    //        string ModeDiscription = Dt.Rows[0]["Modedescription"].ToString();
    //        string Paidto = Dt.Rows[0]["Partyname"].ToString();
    //        string Remark = Dt.Rows[0]["TransactionRemark"].ToString();
    //        string Paid = Dt.Rows[0]["Paid"].ToString();
    //        string AgainstNumber = Dt.Rows[0]["BillNo"].ToString();
    //        string Invoicedates = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
    //        string TDS = Dt.Rows[0]["TDS"].ToString();
    //        string Adjusted = Dt.Rows[0]["Adjust"].ToString();
    //        string Excess = Dt.Rows[0]["Excess"].ToString();
    //        string Notes = Dt.Rows[0]["Note"].ToString();
    //        string Total = Dt.Rows[0]["Total"].ToString();
    //        string Amount = Dt.Rows[0]["Amount"].ToString();

    //        string Against = Dt.Rows[0]["Against"].ToString();

    //        PdfContentByte cb = writer.DirectContent;
    //        cb.Rectangle(28f, -100f, 560f, 80f);

    //        cb.Stroke();
    //        // Header 
    //        cb.BeginText();
    //        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
    //        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Excel Enclosures", 250, 800, 0);
    //        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
    //        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Gat No. 1567, Shelar Vasti, Dehu-Alandi Road, Chikhali, Pune - 411062", 170, 785, 0);
    //        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
    //        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Mb No : 9225658662   EMAIL : accounts@excelenclosures.com  ", 190, 772, 0);
    //        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
    //        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
    //        cb.EndText();


    //        //PdfContentByte cbbb = writer.DirectContent;
    //        //cbbb.Rectangle(28f, 0f, 560f, 25f);
    //        //cbbb.Stroke();
    //        ////Header
    //        //cbbb.BeginText();
    //        //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
    //        //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GSTIN :27ATFPS1959J1Z4" + "", 48, 765, 0);
    //        //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
    //        //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: ATFPS1959J" + "", 170, 765, 0);
    //        //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
    //        //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "MAHARASHTRA STATE GST CODE : 27" + "", 280, 765, 0);
    //        //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
    //        //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : 9225658662", 455, 765, 0);
    //        //cbbb.EndText();

    //        //Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
    //        //doc.Add(p);


    //        PdfContentByte cd = writer.DirectContent;
    //        cd.Rectangle(28f, 760f, 560f, 0f);
    //        cd.Stroke();
    //        // Header 
    //        cd.BeginText();
    //        cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
    //        cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAYMENT VOUCHER", 250, 739, 0);
    //        cd.EndText();


    //        Paragraph paragraphTable1 = new Paragraph();
    //        paragraphTable1.SpacingBefore = 120f;
    //        paragraphTable1.SpacingAfter = 20f;

    //        PdfPTable table = new PdfPTable(4);

    //        float[] widths2 = new float[] { 100, 240, 100, 100 };
    //        table.SetWidths(widths2);
    //        table.TotalWidth = 560f;
    //        table.LockedWidth = true;


    //        //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
    //        //string datee = ffff1.ToString("yyyy-MM-dd");
    //        //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
    //        //table.DefaultCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
    //        table.DefaultCell.Border = Rectangle.NO_BORDER;

    //        table.AddCell(new Phrase("Voucher Number : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase(VoucherNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase("Voucher Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase(VoucherDate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase("Bank", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase(bankName, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase("Cheque Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase(chequedate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase(TransactionMode + " :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase(ModeDiscription, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase("Amount :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase("Rs. " + Amount, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase("Paid To :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
    //        table.AddCell(new Phrase(Paidto, FontFactory.GetFont("Arial", 10, Font.BOLD)));

    //        table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
    //        table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));

    //        table.AddCell(new Phrase("Remark :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase(Remark, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

    //        paragraphTable1.Add(table);
    //        doc.Add(paragraphTable1);

    //        if (Against == "Invoice Bill")
    //        {
    //            ///Description Table

    //            Paragraph paragraphTable2 = new Paragraph();
    //            paragraphTable2.SpacingAfter = 0f;
    //            table = new PdfPTable(8);
    //            float[] widths33 = new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f };
    //            table.SetWidths(widths33);

    //            decimal SumOfTotal = 0;
    //            decimal Paidval = 0;
    //            decimal ExcessVal = 0;
    //            decimal TDSs = 0;
    //            decimal Adjust = 0;
    //            if (Dt.Rows.Count > 0)
    //            {
    //                table.TotalWidth = 560f;
    //                table.LockedWidth = true;

    //                table.AddCell(new Phrase("SN.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("Against", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("Dated", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("Paid", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("TDS", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("Adjusted", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("Excess", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //                table.AddCell(new Phrase("Note", FontFactory.GetFont("Arial", 10, Font.BOLD)));

    //                int rowid = 1;
    //                foreach (DataRow dr in Dt.Rows)
    //                {
    //                    table.TotalWidth = 560f;
    //                    table.LockedWidth = true;
    //                    //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
    //                    table.AddCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["BillNo"].ToString(), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["BillDate"].ToString().TrimEnd("0:0".ToCharArray()), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["Paid"].ToString(), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["TDS"].ToString(), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["Adjust"].ToString(), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["Excess"].ToString(), FontFactory.GetFont("Arial", 9)));
    //                    table.AddCell(new Phrase(dr["Note"].ToString(), FontFactory.GetFont("Arial", 9)));
    //                    rowid++;
    //                    //SumOfTotal += Convert.ToDecimal(dr["Total"].ToString());
    //                    Paidval += Convert.ToDecimal(dr["Paid"].ToString());
    //                    TDSs += Convert.ToDecimal(dr["TDS"].ToString());
    //                    Adjust += Convert.ToDecimal(dr["Adjust"].ToString());
    //                    ExcessVal += Convert.ToDecimal(dr["Excess"].ToString());
    //                }
    //            }

    //            paragraphTable2.Add(table);
    //            doc.Add(paragraphTable2);
    //            ///End Description table
    //            //Space
    //            Paragraph paragraphTable3 = new Paragraph();

    //            string[] items = { "Goods once sold will not be taken back or exchange. \b",
    //                    "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
    //                    "Our risk and responsibility ceases the moment goods leaves out godown. \n",
    //                    };

    //            Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
    //            Font font10 = FontFactory.GetFont("Arial", 10, Font.BOLD);
    //            Paragraph paragraph = new Paragraph("", font12);

    //            for (int i = 0; i < items.Length; i++)
    //            {
    //                paragraph.Add(new Phrase("", font10));
    //            }

    //            table = new PdfPTable(8);
    //            //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
    //            table.TotalWidth = 560f;
    //            table.LockedWidth = true;
    //            table.SetWidths(new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f });
    //            table.AddCell(paragraph);
    //            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //            table.AddCell(new Phrase("Total", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //            table.AddCell(new Phrase(Paidval.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //            table.AddCell(new Phrase(TDSs.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //            table.AddCell(new Phrase(Adjust.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //            table.AddCell(new Phrase(ExcessVal.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));

    //            table.AddCell(new Phrase("\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

    //            doc.Add(table);

    //            Paragraph paragraphTable5 = new Paragraph();
    //            string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Paidval));
    //            //Total amount In word
    //            table = new PdfPTable(3);
    //            table.TotalWidth = 560f;
    //            table.LockedWidth = true;
    //            table.DefaultCell.Border = Rectangle.NO_BORDER;

    //            table.SetWidths(new float[] { 0f, 199f, 0f });
    //            table.AddCell(paragraphTable5);
    //            PdfPCell cell44345 = new PdfPCell(new Phrase("The Sum Of Rupess: " + Amtinword + "  RUPEES ONLY.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
    //            cell44345.HorizontalAlignment = Element.ALIGN_LEFT;

    //            table.AddCell(cell44345);
    //            PdfPCell cell44044 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
    //            cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
    //            table.AddCell(cell44044);
    //            doc.Add(table);
    //        }
    //        //Authorization
    //        Paragraph paragraphTable10000 = new Paragraph();


    //        string[] itemss4 = {
    //            "Payment Term     ",

    //                    };

    //        Font font144 = FontFactory.GetFont("Arial", 11);
    //        Font font155 = FontFactory.GetFont("Arial", 8);
    //        Paragraph paragraphhhhhff = new Paragraph();
    //        table = new PdfPTable(2);
    //        table.DefaultCell.Border = Rectangle.NO_BORDER;
    //        table.SpacingBefore = 30f;
    //        table.TotalWidth = 560f;
    //        table.LockedWidth = true;
    //        table.SetWidths(new float[] { 150f, 150f });


    //        // Bind stamp Image
    //        string imageStamp = Server.MapPath("~") + "/img/Account.png";
    //        iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imageStamp);
    //        image1.ScaleToFit(800, 120);
    //        PdfPCell imageCell = new PdfPCell(image1);
    //        imageCell.PaddingLeft = 140f;
    //        imageCell.PaddingTop = 0f;
    //        imageCell.Border = Rectangle.NO_BORDER;
    //        /////////////////

    //        //table.AddCell(paragraphhhhhff);
    //        table.DefaultCell.Border = Rectangle.NO_BORDER;
    //        table.AddCell(new Phrase("                              \n\n\n\n\n\n\n\n\n\n        Receiver Signature", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
    //        table.AddCell(imageCell);

    //        table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
    //        doc.Add(table);
    //        //doc.Close();
    //        ///end Sign Authorization
    //        //End Authorization

    //    }
    //    doc.Close();
    //    string filePath = @Server.MapPath("~/files/") + "Payment.pdf";
    //    Response.ContentType = "Receipt.pdf";
    //    Response.WriteFile(filePath);
    //}

    public static string ConvertNumbertoWords(int numbers)
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
            sb.Append("Ruppes ");
        }
        else if (paisaamt > 0)
        {
            var paisatext = ConvertNumbertoWords(paisaamt);
            sb.AppendFormat("Rupees {0} paise", paisatext);
        }
        return sb.ToString().TrimEnd();
    }

    //AB code
    /////pdf function
    /////pdf function
    protected void Send_Mail(int? id, string Subject)
    {
        try
        {

            string strMessage = "Hello " + txtPartyName.Text.Trim() + "<br/>" +

                        "Greetings From " + "<strong>Pune Abrasives Pvt. Ltd.<strong>" + "<br/>" +
                        "We have sent you Payment Voucher." + "<br/>" +

                         "We Look Foward to Conducting Future Business with you." + "<br/>" +

                        "Kind Regards," + "<br/>" +
                        "<strong>Pune Abrasives Pvt. Ltd.<strong>";
            string pdfname = txtPartyName.Text + "_" + txtbankname.Text + "_" + txtdate.Text + ".pdf";

            MailMessage mm = new MailMessage();
            // mm.From = new MailAddress(fromMailID);
            string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
            mm.Subject = "Payment Voucher";
            // mm.To.Add("shubhpawar59@gmail.com");
            mm.To.Add(lblEmailID.Text);
            mm.CC.Add("girish.kulkarni@puneabrasives.com");
            mm.CC.Add("virendra.sud@puneabrasives.com");
            mm.CC.Add("accounts@puneabrasives.com");
            mm.CC.Add(Session["EmailID"].ToString().Trim().ToLower());
            StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
            string readFile = reader.ReadToEnd();
            string myString = "";
            myString = readFile;

            string multilineText = strMessage;
            string formattedText = multilineText.Replace("\n", "<br />");

            myString = myString.Replace("$Comment$", formattedText);

            mm.Body = myString.ToString();

            mm.IsBodyHtml = true;

            mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);
            MemoryStream file = new MemoryStream(PDFF(id).ToArray());

            file.Seek(0, SeekOrigin.Begin);
            Attachment data = new Attachment(file, pdfname, "application/pdf");
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.DateTime.Now;
            disposition.ModificationDate = System.DateTime.Now;
            disposition.DispositionType = DispositionTypeNames.Attachment;
            mm.Attachments.Add(data);
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
          //  smtp.Send(mm);

            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='Payment.aspx';", true);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public MemoryStream PDF(int? id)
    {
        MemoryStream pdf = new MemoryStream();
        System.Data.DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PaymentPDF where Id = '" + id + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
        PdfWriter pdfWriter = PdfWriter.GetInstance(doc, pdf);
        //string Path = ;
        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "Payment.pdf", FileMode.Create));
        //PdfWriter writer = PdfWriter.GetInstance(doc, Response.OutputStream);
        XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();
        string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";


        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //Resize image depend upon your need

        png.ScaleToFit(70, 100);

        //For Image Position
        png.SetAbsolutePosition(40, 775);
        //var document = new Document();

        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;

        //Give some space after the image

        png.SpacingAfter = 1f;

        png.Alignment = Element.ALIGN_LEFT;

        doc.Add(png);

        if (Dt.Rows.Count > 0)
        {
            var VoucherDate = DateTime.Now.ToString("dd-MM-yyyy");
            string VoucherNo = Dt.Rows[0]["Id"].ToString();
            string chequedate = Dt.Rows[0]["postDate"].ToString().TrimEnd("0:0".ToCharArray());
            string bankName = Dt.Rows[0]["BankName"].ToString();
            string TransactionMode = Dt.Rows[0]["TransactionMode"].ToString();
            string ModeDiscription = Dt.Rows[0]["Modedescription"].ToString();
            string Paidto = Dt.Rows[0]["Partyname"].ToString();
            string Remark = Dt.Rows[0]["TransactionRemark"].ToString();
            string Paid = Dt.Rows[0]["Paid"].ToString();
            string AgainstNumber = Dt.Rows[0]["BillNo"].ToString();
            string Invoicedates = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
            string TDS = Dt.Rows[0]["TDS"].ToString();
            string Adjusted = Dt.Rows[0]["Adjust"].ToString();
            string Excess = Dt.Rows[0]["Excess"].ToString();
            string Notes = Dt.Rows[0]["Note"].ToString();
            string Total = Dt.Rows[0]["Total"].ToString();
            string Amount = Dt.Rows[0]["Amount"].ToString();

            string Against = Dt.Rows[0]["Against"].ToString();

            PdfContentByte cb = writer.DirectContent;
            cb.Rectangle(28f, -100f, 560f, 80f);

            cb.Stroke();
            // Header 
            cb.BeginText();
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Excel Enclosures", 250, 800, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Gat No. 1567, Shelar Vasti, Dehu-Alandi Road, Chikhali, Pune - 411062", 170, 785, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Mb No : 9225658662   EMAIL : accounts@excelenclosures.com  ", 190, 772, 0);
            cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
            cb.EndText();


            //PdfContentByte cbbb = writer.DirectContent;
            //cbbb.Rectangle(28f, 0f, 560f, 25f);
            //cbbb.Stroke();
            ////Header
            //cbbb.BeginText();
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GSTIN :27ATFPS1959J1Z4" + "", 48, 765, 0);
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: ATFPS1959J" + "", 170, 765, 0);
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "MAHARASHTRA STATE GST CODE : 27" + "", 280, 765, 0);
            //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : 9225658662", 455, 765, 0);
            //cbbb.EndText();

            //Paragraph p = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(1.5F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            //doc.Add(p);


            PdfContentByte cd = writer.DirectContent;
            cd.Rectangle(28f, 760f, 560f, 0f);
            cd.Stroke();
            // Header 
            cd.BeginText();
            cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
            cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAYMENT VOUCHER", 100, 739, 0);
            cd.EndText();


            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 20f;

            PdfPTable table = new PdfPTable(4);

            float[] widths2 = new float[] { 100, 240, 100, 100 };
            table.SetWidths(widths2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;


            //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            //string datee = ffff1.ToString("yyyy-MM-dd");
            //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.DefaultCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("Voucher Number : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VoucherNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Voucher Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VoucherDate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Bank", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(bankName, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Cheque Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(chequedate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(TransactionMode + " :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(ModeDiscription, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Amount :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("Rs. " + Amount, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Paid To :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(Paidto, FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("Remark :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(Remark, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            paragraphTable1.Add(table);
            doc.Add(paragraphTable1);

            if (Against == "Invoice Bill")
            {
                ///Description Table

                Paragraph paragraphTable2 = new Paragraph();
                paragraphTable2.SpacingAfter = 0f;
                table = new PdfPTable(8);
                float[] widths33 = new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f };
                table.SetWidths(widths33);

                decimal SumOfTotal = 0;
                decimal Paidval = 0;
                decimal ExcessVal = 0;
                decimal TDSs = 0;
                decimal Adjust = 0;
                if (Dt.Rows.Count > 0)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;

                    table.AddCell(new Phrase("SN.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Against", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Dated", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Paid", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("TDS", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Adjusted", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Excess", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Note", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                    int rowid = 1;
                    foreach (DataRow dr in Dt.Rows)
                    {
                        table.TotalWidth = 560f;
                        table.LockedWidth = true;
                        //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        table.AddCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["BillNo"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["BillDate"].ToString().TrimEnd("0:0".ToCharArray()), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Paid"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["TDS"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Adjust"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Excess"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Note"].ToString(), FontFactory.GetFont("Arial", 9)));
                        rowid++;
                        //SumOfTotal += Convert.ToDecimal(dr["Total"].ToString());
                        Paidval += Convert.ToDecimal(dr["Paid"].ToString());
                        TDSs += Convert.ToDecimal(dr["TDS"].ToString());
                        Adjust += Convert.ToDecimal(dr["Adjust"].ToString());
                        ExcessVal += Convert.ToDecimal(dr["Excess"].ToString());
                    }
                }

                paragraphTable2.Add(table);
                doc.Add(paragraphTable2);
                ///End Description table
                //Space
                Paragraph paragraphTable3 = new Paragraph();

                string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

                Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font font10 = FontFactory.GetFont("Arial", 10, Font.BOLD);
                Paragraph paragraph = new Paragraph("", font12);

                for (int i = 0; i < items.Length; i++)
                {
                    paragraph.Add(new Phrase("", font10));
                }

                table = new PdfPTable(8);
                //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Total", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(Paidval.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(TDSs.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(Adjust.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(ExcessVal.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));

                table.AddCell(new Phrase("\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                doc.Add(table);

                Paragraph paragraphTable5 = new Paragraph();
                string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Paidval));
                //Total amount In word
                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.NO_BORDER;

                table.SetWidths(new float[] { 0f, 199f, 0f });
                table.AddCell(paragraphTable5);
                PdfPCell cell44345 = new PdfPCell(new Phrase("The Sum Of Rupess: " + Amtinword + "  RUPEES ONLY.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                cell44345.HorizontalAlignment = Element.ALIGN_LEFT;

                table.AddCell(cell44345);
                PdfPCell cell44044 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell44044);
                doc.Add(table);
            }
            //Authorization
            Paragraph paragraphTable10000 = new Paragraph();


            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();
            table = new PdfPTable(2);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.SpacingBefore = 30f;
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 150f, 150f });


            // Bind stamp Image
            string imageStamp = Server.MapPath("~") + "/img/Account.png";
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imageStamp);
            image1.ScaleToFit(800, 120);
            PdfPCell imageCell = new PdfPCell(image1);
            imageCell.PaddingLeft = 140f;
            imageCell.PaddingTop = 0f;
            imageCell.Border = Rectangle.NO_BORDER;
            /////////////////

            //table.AddCell(paragraphhhhhff);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.AddCell(new Phrase("                              \n\n\n\n\n\n\n\n\n\n        Receiver Signature", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(imageCell);

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            doc.Add(table);
            //doc.Close();
            ///end Sign Authorization
            //End Authorization

        }
        doc.Close();
        string filePath = @Server.MapPath("~/files/") + "Payment.pdf";
        Response.ContentType = "Receipt.pdf";
        Response.WriteFile(filePath);
        return pdf;

    }

    public MemoryStream PDFF(int? id)
    {

        MemoryStream pdf = new MemoryStream();
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PaymentPDF where Id = '" + id + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 10f, 10f, 55f, 0f);
        PdfWriter pdfWriter = PdfWriter.GetInstance(doc, pdf);

        // PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "PurchaseOrder.pdf", FileMode.Create));
        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "Payment.pdf", FileMode.Create));
        XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);


        doc.Open();

        string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";

        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //Resize image depend upon your need

        png.ScaleToFit(70, 100);

        //For Image Position
        png.SetAbsolutePosition(40, 718);
        //var document = new Document();

        //Give space before image
        //png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
        png.SpacingBefore = 50f;

        //Give some space after the image

        png.SpacingAfter = 1f;

        png.Alignment = Element.ALIGN_LEFT;

        //paragraphimage.Add(png);
        //doc.Add(paragraphimage);
        doc.Add(png);


        PdfContentByte cb = pdfWriter.DirectContent;
        cb.Rectangle(17f, 710f, 560f, 60f);
        cb.Stroke();
        // Header 
        cb.BeginText();
        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Excel Enclosure", 250, 745, 0);
        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Gat No. 1567, Shelar Vasti, Dehu-Alandi Road, Chikhali, Pune - 411062", 145, 728, 0);
        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
        cb.EndText();

        //PdfContentByte cbb = writer.DirectContent;
        //cbb.Rectangle(17f, 710f, 560f, 25f);
        //cbb.Stroke();
        //// Header 
        //cbb.BeginText();
        //cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
        //cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " CONTACT : 9225658662   Email ID : mktg@excelenclosures.com", 153, 722, 0);
        //cbb.EndText();

        PdfContentByte cbbb = pdfWriter.DirectContent;
        cbbb.Rectangle(17f, 685f, 560f, 25f);
        cbbb.Stroke();
        // Header 
        cbbb.BeginText();
        cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
        cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Email : purchase@excelenclosures.com / accounts@excelenclosures.com ", 30, 695, 0);
        //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
        //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: ATFPS1959J", 160, 695, 0);
        //cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
        //cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "EMAIL : mktg@excelenclosures.com", 270, 695, 0);
        cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
        cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : 9225805669/9225814668", 400, 695, 0);
        cbbb.EndText();

        PdfContentByte cd = pdfWriter.DirectContent;
        cd.Rectangle(17f, 660f, 560f, 25f);
        cd.Stroke();
        // Header 
        cd.BeginText();
        cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
        cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Payment Voucher ", 260, 667, 0);
        cd.EndText();

        //MemoryStream pdf = new MemoryStream();
        //DataTable Dt = new DataTable();
        //SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PaymentPDF where Id = '" + id + "'", con);

        //Da.Fill(Dt);

        //StringWriter sw = new StringWriter();
        //StringReader sr = new StringReader(sw.ToString());

        //Document doc = new Document(PageSize.A4, 30f, 10f, -15f, 0f);
        //PdfWriter pdfWriter = PdfWriter.GetInstance(doc, pdf);

        //PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "Payment.pdf", FileMode.Create));
        //XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        //doc.Open();
        //string imageURL = Server.MapPath("~") + "/img/ExcelEncLogo.png";

        //iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //png.ScaleToFit(70, 100);
        //png.SetAbsolutePosition(40, 775);
        //png.SpacingBefore = 50f;
        //png.SpacingAfter = 1f;
        //png.Alignment = Element.ALIGN_LEFT;
        //doc.Add(png);

        if (Dt.Rows.Count > 0)
        {
            var VoucherDate = DateTime.Now.ToString("dd-MM-yyyy");
            string VoucherNo = Dt.Rows[0]["Id"].ToString();
            string chequedate = Dt.Rows[0]["postDate"].ToString().TrimEnd("0:0".ToCharArray());
            string bankName = Dt.Rows[0]["BankName"].ToString();
            string TransactionMode = Dt.Rows[0]["TransactionMode"].ToString();
            string ModeDiscription = Dt.Rows[0]["Modedescription"].ToString();
            string Paidto = Dt.Rows[0]["Partyname"].ToString();
            string Remark = Dt.Rows[0]["TransactionRemark"].ToString();
            string Paid = Dt.Rows[0]["Paid"].ToString();
            string AgainstNumber = Dt.Rows[0]["BillNo"].ToString();
            string Invoicedates = Dt.Rows[0]["BillDate"].ToString().TrimEnd("0:0".ToCharArray());
            string TDS = Dt.Rows[0]["TDS"].ToString();
            string Adjusted = Dt.Rows[0]["Adjust"].ToString();
            string Excess = Dt.Rows[0]["Excess"].ToString();
            string Notes = Dt.Rows[0]["Note"].ToString();
            string Total = Dt.Rows[0]["Total"].ToString();
            string Amount = Dt.Rows[0]["Amount"].ToString();

            string Against = Dt.Rows[0]["Against"].ToString();

            PdfContentByte cbb = writer.DirectContent;
            cbb.Rectangle(28f, -100f, 560f, 80f);

            cbb.Stroke();
            // Header 
            cbb.BeginText();
            cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
            cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Excel Enclosures", 250, 800, 0);
            cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Gat No. 1567, Shelar Vasti, Dehu-Alandi Road, Chikhali, Pune - 411062", 170, 785, 0);
            cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Mb No : 9225658662   EMAIL : accounts@excelenclosures.com  ", 190, 772, 0);
            cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
            cbb.EndText();


            PdfContentByte cdd = writer.DirectContent;
            cdd.Rectangle(28f, 760f, 560f, 0f);
            cdd.Stroke();
            // Header 
            cdd.BeginText();
            cdd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
            cdd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAYMENT VOUCHER", 250, 739, 0);
            cdd.EndText();


            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 20f;

            PdfPTable table = new PdfPTable(4);

            float[] widths2 = new float[] { 100, 240, 100, 100 };
            table.SetWidths(widths2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;


            //DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PODate"].ToString());
            //string datee = ffff1.ToString("yyyy-MM-dd");
            //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
            //table.DefaultCell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            table.DefaultCell.Border = Rectangle.NO_BORDER;

            table.AddCell(new Phrase("Voucher Number : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VoucherNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Voucher Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VoucherDate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Bank", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(bankName, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Cheque Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(chequedate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase(TransactionMode + " :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(ModeDiscription, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Amount :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("Rs. " + Amount, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Paid To :", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase(Paidto, FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));

            table.AddCell(new Phrase("Remark :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(Remark, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            paragraphTable1.Add(table);
            doc.Add(paragraphTable1);

            if (Against == "Invoice Bill")
            {
                ///Description Table

                Paragraph paragraphTable2 = new Paragraph();
                paragraphTable2.SpacingAfter = 0f;
                table = new PdfPTable(8);
                float[] widths33 = new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f };
                table.SetWidths(widths33);

                decimal SumOfTotal = 0;
                decimal Paidval = 0;
                decimal ExcessVal = 0;
                decimal TDSs = 0;
                decimal Adjust = 0;
                if (Dt.Rows.Count > 0)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;

                    table.AddCell(new Phrase("SN.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Against", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Dated", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Paid", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("TDS", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Adjusted", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Excess", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Note", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                    int rowid = 1;
                    foreach (DataRow dr in Dt.Rows)
                    {
                        table.TotalWidth = 560f;
                        table.LockedWidth = true;
                        //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        table.AddCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["BillNo"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["BillDate"].ToString().TrimEnd("0:0".ToCharArray()), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Paid"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["TDS"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Adjust"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Excess"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(dr["Note"].ToString(), FontFactory.GetFont("Arial", 9)));
                        rowid++;
                        //SumOfTotal += Convert.ToDecimal(dr["Total"].ToString());
                        Paidval += Convert.ToDecimal(dr["Paid"].ToString());
                        TDSs += Convert.ToDecimal(dr["TDS"].ToString());
                        Adjust += Convert.ToDecimal(dr["Adjust"].ToString());
                        ExcessVal += Convert.ToDecimal(dr["Excess"].ToString());
                    }
                }

                paragraphTable2.Add(table);
                doc.Add(paragraphTable2);
                ///End Description table
                //Space
                Paragraph paragraphTable3 = new Paragraph();

                string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

                Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font font10 = FontFactory.GetFont("Arial", 10, Font.BOLD);
                Paragraph paragraph = new Paragraph("", font12);

                for (int i = 0; i < items.Length; i++)
                {
                    paragraph.Add(new Phrase("", font10));
                }

                table = new PdfPTable(8);
                //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 10f, 11f, 10f, 10f, 10f, 10f, 28f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("Total", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(Paidval.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(TDSs.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(Adjust.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase(ExcessVal.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));

                table.AddCell(new Phrase("\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                doc.Add(table);

                Paragraph paragraphTable5 = new Paragraph();
                string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Paidval));
                //Total amount In word
                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.DefaultCell.Border = Rectangle.NO_BORDER;

                table.SetWidths(new float[] { 0f, 199f, 0f });
                table.AddCell(paragraphTable5);
                PdfPCell cell44345 = new PdfPCell(new Phrase("The Sum Of Rupess: " + Amtinword + "  RUPEES ONLY.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                cell44345.HorizontalAlignment = Element.ALIGN_LEFT;

                table.AddCell(cell44345);
                PdfPCell cell44044 = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(cell44044);
                doc.Add(table);
            }
            //Authorization
            Paragraph paragraphTable10000 = new Paragraph();


            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();
            table = new PdfPTable(2);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.SpacingBefore = 30f;
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 150f, 150f });


            // Bind stamp Image
            string imageStamp = Server.MapPath("~") + "/img/Account.png";
            iTextSharp.text.Image image1 = iTextSharp.text.Image.GetInstance(imageStamp);
            image1.ScaleToFit(800, 120);
            PdfPCell imageCell = new PdfPCell(image1);
            imageCell.PaddingLeft = 140f;
            imageCell.PaddingTop = 0f;
            imageCell.Border = Rectangle.NO_BORDER;
            /////////////////

            //table.AddCell(paragraphhhhhff);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.AddCell(new Phrase("                              \n\n\n\n\n\n\n\n\n\n        Receiver Signature", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(imageCell);

            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            doc.Add(table);
            //doc.Close();
            ///end Sign Authorization
            //End Authorization

        }
        doc.Close();
        string filePath = @Server.MapPath("~/files/") + "Payment.pdf";
        Response.ContentType = "Receipt.pdf";
        Response.WriteFile(filePath);
        return pdf;
    }



    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PaymentList.aspx");
    }
}