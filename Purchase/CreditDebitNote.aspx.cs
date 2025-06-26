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
using System.Globalization;

public partial class Purchase_CreditDebitNote : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable dt = new DataTable();
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
                txtDocdate.Text = DateTime.Today.ToString("dd-MM-yyyy");
                //fillddlCategory();
                UpdateHistorymsg = string.Empty; regdate = string.Empty;
                if (Request.QueryString["ID"] != null)
                {
                    ViewState["RowNo"] = 0;
                    dt.Columns.AddRange(new DataColumn[15] { new DataColumn("id"),
                 new DataColumn("Particulars"),new DataColumn("Description"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Discount")
            });

                    ViewState["ParticularDetails"] = dt;

                    ViewState["UpdateRowId"] = Decrypt(Request.QueryString["ID"].ToString());
                    GetCreditDebitNoteData(ViewState["UpdateRowId"].ToString());
                }
                else
                {
                    ViewState["RowNo"] = 0;
                    dt.Columns.AddRange(new DataColumn[15] { new DataColumn("id"),
                 new DataColumn("Particulars"),new DataColumn("Description"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Discount")
            });
                    ViewState["ParticularDetails"] = dt;
                    txtDocNo.Text = GenerateComCode();
                }
            }
        }
    }

    //private void fillddlCategory()
    // {
    //     SqlDataAdapter adpt = new SqlDataAdapter("select * from tblDebitCreditCategory", con);
    //     DataTable dtpt = new DataTable();
    //     adpt.Fill(dtpt);

    //     if (dtpt.Rows.Count > 0)
    //     {
    //          ddlCategory.DataSource = dtpt;
    //        ddlCategory.DataValueField = "Category";
    //        ddlCategory.DataTextField = "Category";
    //        ddlCategory.DataBind();

    //        ddlCategory.Items.Insert(0, new ListItem("--Select Category--", "0"));
    //   }
    //  }

    static string regdate = string.Empty;
    protected void GetCreditDebitNoteData(string id)
    {
        string query1 = string.Empty;
        query1 = @"select * from tblCreditDebitNoteHdr where Id='" + id + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtSupplierName.Text = dt.Rows[0]["SupplierName"].ToString();
            txtDocNo.Text = dt.Rows[0]["DocNo"].ToString();
            txtTCharge.Text = dt.Rows[0]["TransportationCharge"].ToString();
            txtTCGSTPer.Text = dt.Rows[0]["TCGSTPer"].ToString();
            txtTCGSTamt.Text = dt.Rows[0]["TCGSTAmt"].ToString();
            txtTSGSTPer.Text = dt.Rows[0]["TSGSTPer"].ToString();
            txtTSGSTamt.Text = dt.Rows[0]["TSGSTAmt"].ToString();
            txtTIGSTPer.Text = dt.Rows[0]["TIGSTPer"].ToString();
            txtTIGSTamt.Text = dt.Rows[0]["TIGSTAmt"].ToString();
            txtTCost.Text = dt.Rows[0]["TotalCost"].ToString();

            //  fillddlCategory();
            BindBillNO();

            ddlProcess.Text = dt.Rows[0]["Process"].ToString();
            ddlNoteType.Text = dt.Rows[0]["NoteType"].ToString();
            txtcategory.Text = dt.Rows[0]["CategoryName"].ToString();
            txtDocdate.Text = dt.Rows[0]["DocDate"].ToString();

            ddlBillNumber.SelectedItem.Text = dt.Rows[0]["BillNumber"].ToString();
            if (dt.Rows[0]["BillDate"].ToString() == "")
            {
            }
            else
            {
                txtBillDate.Text = dt.Rows[0]["BillDate"].ToString();
            }

            if (dt.Rows[0]["Process"].ToString() == "Manual")
            {
                ddlBillNumber.Enabled = false;
                txtBillDate.Enabled = false;
            }
            else
            {
                ddlBillNumber.SelectedItem.Text = dt.Rows[0]["BillNumber"].ToString();
                txtBillDate.Text = dt.Rows[0]["BillDate"].ToString();
            }
            txtPaymentDueDate.Text = dt.Rows[0]["PaymentDueDate"].ToString();
            txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();
            getParticularsdts(id);
            DivManual.Visible = true;
            btnadd.Text = "Update";
        }
    }

    protected void getParticularsdts(string id)
    {
        DataTable Dtproduct = new DataTable();
        SqlDataAdapter daa = new SqlDataAdapter("select * from tblCreditDebitNoteDtls where HeaderID='" + id + "'", con);
        daa.Fill(Dtproduct);
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;

        DataTable dt = ViewState["ParticularDetails"] as DataTable;

        if (Dtproduct.Rows.Count > 0)
        {
            for (int i = 0; i < Dtproduct.Rows.Count; i++)
            {
                dt.Rows.Add(ViewState["RowNo"], Dtproduct.Rows[i]["Particulars"].ToString(), Dtproduct.Rows[i]["Description"].ToString(), Dtproduct.Rows[i]["HSN"].ToString(), Dtproduct.Rows[i]["Qty"].ToString(),
                    Dtproduct.Rows[i]["Rate"].ToString(), Dtproduct.Rows[i]["Amount"].ToString(), Dtproduct.Rows[i]["CGSTPer"].ToString(), Dtproduct.Rows[i]["CGSTAmt"].ToString(),
                    Dtproduct.Rows[i]["SGSTPer"].ToString(), Dtproduct.Rows[i]["SGSTAmt"].ToString(), Dtproduct.Rows[i]["IGSTPer"].ToString(), Dtproduct.Rows[i]["IGSTAmt"].ToString(),
                    Dtproduct.Rows[i]["Total"].ToString());
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

        //SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tblCreditDebitNoteHdr] with (nolock) where NoteType='"+ ddlNoteType.SelectedValue+ "'", con);
        SqlDataAdapter ad = new SqlDataAdapter("SELECT ID_Max as maxid FROM [tblCreditDebitNote_ID] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
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

            ///comment this code for Id Matching
            if (ddlNoteType.SelectedValue == "Credit" && maxid == 8)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Credit Note No. Sequence Issue Resolved, Contact To Software Dept..!!!');window.location.href='CreditDebitNoteList.aspx';", true);
            }
            //////
        }
        else
        {
            invoiceno = string.Empty;
        }
        txtDocNo.Text = invoiceno;
        return invoiceno;

    }
    bool flgs;
    protected void btnadd_Click(object sender, EventArgs e)
    {
        #region Insert
        if (btnadd.Text == "Submit")
        {

            if (ddlProcess.Text == "Manual")
            {
                if (dgvParticularsDetails.Rows.Count > 0)
                {
                    flgs = true;
                }
                else
                {
                    flgs = false;
                }
            }
            else
            {
                if (dgvAutomatic.Rows.Count > 0)
                {
                    foreach (GridViewRow g2 in dgvAutomatic.Rows)
                    {
                        bool chk = (g2.FindControl("chkSelect") as CheckBox).Checked;

                        while (chk == true)
                        {
                            flgs = true;
                            break;
                        }
                    }
                }
                else
                {
                    flgs = false;
                }
            }

            if (flgs == true)
            {
                //string DocNo = GenerateComCode();
                if (ddlNoteType.SelectedValue == "Select" && txtDocNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Kindly Select Note Type..!!');", true);
                }
                else
                {
                    SqlCommand cmd = new SqlCommand("SP_CreditDebitNote", con);
                    cmd.Parameters.Clear();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "insert");
                    cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
                    cmd.Parameters.AddWithValue("@DocNo", txtDocNo.Text);
                    cmd.Parameters.AddWithValue("@DocDate", txtDocdate.Text);

                    cmd.Parameters.AddWithValue("@Process", ddlProcess.Text.Trim());
                    cmd.Parameters.AddWithValue("@NoteType", ddlNoteType.Text.Trim());
                    cmd.Parameters.AddWithValue("@CategoryName", txtcategory.Text.Trim());
                    cmd.Parameters.AddWithValue("@BillNumber", ddlBillNumber.SelectedItem.Text == "--Select Bill Number--" ? "" : ddlBillNumber.SelectedItem.Text.Trim());
                    cmd.Parameters.AddWithValue("@BillDate", txtBillDate.Text.Trim());
                    cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text);
                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
                    cmd.Parameters.AddWithValue("@GrandTotal", txtFGrandTot.Text);
                    cmd.Parameters.AddWithValue("@TransportationCharge", txtTCharge.Text);
                    cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text);
                    cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text);
                    cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text);
                    cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text);
                    cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text);
                    cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text);
                    cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text);
                    cmd.Parameters.AddWithValue("@NoteFor", "Purchase");

                    cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());


                    int a = 0;
                    con.Open();
                    a = cmd.ExecuteNonQuery();
                    con.Close();

                    ////////// Provide Max id from Max_iD from new table - 18/2/23
                    SqlCommand cmdmax = new SqlCommand("SELECT max([Id]) as maxid FROM [tblCreditDebitNoteHdr] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
                    con.Open();
                    Object mx = cmdmax.ExecuteScalar();
                    con.Close();
                    int MaxId = Convert.ToInt32(mx.ToString());

                    //DataTable dt = new DataTable();
                    //ad.Fill(dt);
                    //if (dt.Rows.Count > 0)
                    //{

                    //    int maxid = dt.Rows[0]["maxid"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[0]["maxid"].ToString());
                    //    invoiceno = previousyear.ToString() + "-" + FinYear + "/" + (maxid + 1).ToString();
                    //}


                    //SqlCommand cmdmax = new SqlCommand("select MAX(Id) as MAxID from tblCreditDebitNoteHdr", con);
                    //con.Open();
                    //Object mx = cmdmax.ExecuteScalar();
                    //con.Close();
                    //int MaxId = Convert.ToInt32(mx.ToString());


                    if (ddlProcess.Text == "Automatic")
                    {
                        if (dgvAutomatic.Rows.Count > 0)
                        {
                            foreach (GridViewRow row in dgvAutomatic.Rows)
                            {
                                bool chkrow = ((CheckBox)row.FindControl("chkSelect")).Checked;
                                string Particulars = ((Label)row.FindControl("txtParticulars")).Text;
                                string Description = ((TextBox)row.FindControl("txtDescription")).Text;
                                string HSN = ((TextBox)row.FindControl("txtHSN")).Text;
                                string Qty = ((TextBox)row.FindControl("txtautQty")).Text;
                                string Rate = ((TextBox)row.FindControl("txtRate")).Text;
                                string Amount = ((Label)row.FindControl("txtAmount")).Text;
                                string DiscPer = ((TextBox)row.FindControl("txtDiscPer")).Text;
                                string CGSTPer = ((TextBox)row.FindControl("txtCGST")).Text;
                                string SGSTPer = ((TextBox)row.FindControl("txtSGST")).Text;
                                string IGSTPer = ((TextBox)row.FindControl("txtIGST")).Text;
                                string TotalAmount = ((TextBox)row.FindControl("txtGrandTotal")).Text;

                                var CGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(CGSTPer.Trim())) / 100;
                                var SGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(SGSTPer.Trim())) / 100;
                                var IGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(IGSTPer.Trim())) / 100;

                                SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount])
                            VALUES(" + MaxId + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                                 "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                                 "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + ddlBillNumber.SelectedItem.Text + "','" + DiscPer + "')", con);
                                con.Open();
                                if (chkrow == true)
                                {
                                    cmdParticulardata.ExecuteNonQuery();

                                }
                                con.Close();
                            }
                            //DataTable dt546665 = new DataTable();
                            //SqlDataAdapter sadparticular = new SqlDataAdapter("select * from tblSaleCreditDebitNoteDtls where HeaderID='" + MaxId + "'", con);
                            //sadparticular.Fill(dt546665);
                            //if (dt546665.Rows.Count > 0)
                            //{

                            //}
                            //else
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('At Least Select One Record.');", true);
                            //}

                        }
                    }
                    else
                    {
                        if (dgvParticularsDetails.Rows.Count > 0)
                        {
                            foreach (GridViewRow row in dgvParticularsDetails.Rows)
                            {
                                string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
                                string Description = ((Label)row.FindControl("lblDescription")).Text;
                                string HSN = ((Label)row.FindControl("lblHSN")).Text;
                                string Qty = ((Label)row.FindControl("lblQty")).Text;
                                string Rate = ((Label)row.FindControl("lblRate")).Text;
                                string Amount = ((Label)row.FindControl("lblAmount")).Text;
                                string disc = ((Label)row.FindControl("lbldisc")).Text;
                                string CGSTPer = ((Label)row.FindControl("lblCGSTPer")).Text;
                                string CGSTAmt = ((Label)row.FindControl("lblCGSTAmt")).Text;
                                string SGSTPer = ((Label)row.FindControl("lblSGSTPer")).Text;
                                string SGSTAmt = ((Label)row.FindControl("lblSGSTAmt")).Text;
                                string IGSTPer = ((Label)row.FindControl("lblIGSTPer")).Text;
                                string IGSTAmt = ((Label)row.FindControl("lblIGSTAmt")).Text;
                                string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;

                                SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount])
                        VALUES(" + MaxId + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                                 "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                                 "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + ddlBillNumber.SelectedItem.Text + "','" + disc + "')", con);
                                con.Open();
                                cmdParticulardata.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }

                    // Update ID in CreditdebitNoteId Table For Increment value- 18/2/2023
                    SqlCommand cmdmaxx = new SqlCommand("SELECT ID_Max as maxid FROM [tblCreditDebitNote_ID] with (nolock) where NoteType='" + ddlNoteType.SelectedValue + "'", con);
                    con.Open();
                    Object mxx = cmdmaxx.ExecuteScalar();
                    con.Close();
                    int MaxIdd = Convert.ToInt32(mxx.ToString()) + 1;
                    SqlCommand cmdUpdateId = new SqlCommand(@"Update tblCreditDebitNote_ID Set ID_Max='" + MaxIdd + "' where NoteType='" + ddlNoteType.SelectedValue + "'", con);
                    con.Open();
                    cmdUpdateId.ExecuteNonQuery();
                    con.Close();

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='CreditDebitNoteList.aspx';", true);
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Select At Least One Record.');", true);

            }
        }
        #endregion Insert

        #region Update
        if (btnadd.Text == "Update")
        {
            SqlCommand cmd = new SqlCommand("SP_CreditDebitNote", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "update");
            cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(ViewState["UpdateRowId"].ToString()));
            cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
            cmd.Parameters.AddWithValue("@DocNo", txtDocNo.Text.Trim());
            cmd.Parameters.AddWithValue("@DocDate", txtDocdate.Text.Trim());
            cmd.Parameters.AddWithValue("@Process", ddlProcess.Text.Trim());
            cmd.Parameters.AddWithValue("@NoteType", ddlNoteType.Text.Trim());
            cmd.Parameters.AddWithValue("@CategoryName", txtcategory.Text.Trim());
            cmd.Parameters.AddWithValue("@BillNumber", ddlBillNumber.SelectedItem.Text == "--Select Bill Number--" ? "" : ddlBillNumber.SelectedItem.Text.Trim());
            cmd.Parameters.AddWithValue("@BillDate", txtBillDate.Text.Trim());
            cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text.Trim());

            cmd.Parameters.AddWithValue("@TransportationCharge", txtTCharge.Text);
            cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text);
            cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text);
            cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text);
            cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text);
            cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text);
            cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text);
            cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text);
            cmd.Parameters.AddWithValue("@NoteFor", "Purchase");

            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());
            cmd.Parameters.AddWithValue("@GrandTotal", txtFGrandTot.Text);
            cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());
            int a = 0;
            cmd.Connection.Open();
            a = cmd.ExecuteNonQuery();
            cmd.Connection.Close();


            SqlCommand cmddelete = new SqlCommand("delete from tblCreditDebitNoteDtls where HeaderID='" + Convert.ToInt32(ViewState["UpdateRowId"].ToString()) + "'", con);
            con.Open();
            cmddelete.ExecuteNonQuery();
            con.Close();

            if (ddlProcess.Text == "Automatic")
            {
                if (btnadd.Text == "Update")
                {
                    if (dgvParticularsDetails.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvParticularsDetails.Rows)
                        {
                            string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
                            string Description = ((Label)row.FindControl("lblDescription")).Text;
                            string HSN = ((Label)row.FindControl("lblHSN")).Text;
                            string Qty = ((Label)row.FindControl("lblQty")).Text;
                            string Rate = ((Label)row.FindControl("lblRate")).Text;
                            string Amount = ((Label)row.FindControl("lblAmount")).Text;
                            string disc = ((Label)row.FindControl("lbldisc")).Text;
                            string CGSTPer = ((Label)row.FindControl("lblCGSTPer")).Text;
                            string CGSTAmt = ((Label)row.FindControl("lblCGSTAmt")).Text;
                            string SGSTPer = ((Label)row.FindControl("lblSGSTPer")).Text;
                            string SGSTAmt = ((Label)row.FindControl("lblSGSTAmt")).Text;
                            string IGSTPer = ((Label)row.FindControl("lblIGSTPer")).Text;
                            string IGSTAmt = ((Label)row.FindControl("lblIGSTAmt")).Text;
                            string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;

                            SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount])
                        VALUES(" + ViewState["UpdateRowId"].ToString() + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                             "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                             "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + ddlBillNumber.SelectedItem.Text + "','" + disc + "')", con);
                            con.Open();
                            cmdParticulardata.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
                else
                {
                    if (dgvAutomatic.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvAutomatic.Rows)
                        {
                            string Particulars = ((Label)row.FindControl("txtParticulars")).Text;
                            string Description = ((TextBox)row.FindControl("txtDescription")).Text;
                            string HSN = ((TextBox)row.FindControl("txtHSN")).Text;
                            string Qty = ((TextBox)row.FindControl("txtautQty")).Text;
                            string Rate = ((TextBox)row.FindControl("txtRate")).Text;
                            string Amount = ((Label)row.FindControl("txtAmount")).Text;
                            string DiscPer = ((TextBox)row.FindControl("txtDiscPer")).Text;
                            string CGSTPer = ((TextBox)row.FindControl("txtCGST")).Text;
                            string SGSTPer = ((TextBox)row.FindControl("txtSGST")).Text;
                            string IGSTPer = ((TextBox)row.FindControl("txtIGST")).Text;
                            string TotalAmount = ((TextBox)row.FindControl("txtGrandTotal")).Text;

                            var CGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(CGSTPer.Trim())) / 100;
                            var SGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(SGSTPer.Trim())) / 100;
                            var IGSTAmt = Convert.ToDecimal(Amount) * (Convert.ToDecimal(IGSTPer.Trim())) / 100;

                            SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount])
                        VALUES(" + ViewState["UpdateRowId"].ToString() + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                             "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                             "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + ddlBillNumber.SelectedItem.Text + "','" + DiscPer + "')", con);
                            con.Open();
                            cmdParticulardata.ExecuteNonQuery();
                            con.Close();
                        }
                    }
                }
            }
            else
            {
                if (dgvParticularsDetails.Rows.Count > 0)
                {
                    foreach (GridViewRow row in dgvParticularsDetails.Rows)
                    {
                        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
                        string Description = ((Label)row.FindControl("lblDescription")).Text;
                        string HSN = ((Label)row.FindControl("lblHSN")).Text;
                        string Qty = ((Label)row.FindControl("lblQty")).Text;
                        string Rate = ((Label)row.FindControl("lblRate")).Text;
                        string Amount = ((Label)row.FindControl("lblAmount")).Text;
                        string disc = ((Label)row.FindControl("lbldisc")).Text;
                        string CGSTPer = ((Label)row.FindControl("lblCGSTPer")).Text;
                        string CGSTAmt = ((Label)row.FindControl("lblCGSTAmt")).Text;
                        string SGSTPer = ((Label)row.FindControl("lblSGSTPer")).Text;
                        string SGSTAmt = ((Label)row.FindControl("lblSGSTAmt")).Text;
                        string IGSTPer = ((Label)row.FindControl("lblIGSTPer")).Text;
                        string IGSTAmt = ((Label)row.FindControl("lblIGSTAmt")).Text;
                        string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;

                        SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblCreditDebitNoteDtls([HeaderID],[Particulars],[Description],[HSN],[Qty],[Rate],[Amount],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],InvoiceNo,[Discount])
                        VALUES(" + ViewState["UpdateRowId"].ToString() + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "'," +
                         "'" + Rate + "','" + Amount + "','" + CGSTPer + "','" + CGSTAmt + "'," +
                         "'" + SGSTPer + "','" + SGSTAmt + "','" + IGSTPer + "','" + IGSTAmt + "','" + TotalAmount + "','" + ddlBillNumber.SelectedItem.Text + "','" + disc + "')", con);
                        con.Open();
                        cmdParticulardata.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Updated Sucessfully');window.location.href='CreditDebitNoteList.aspx';", true);
        }
        #endregion Update
    }

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreditDebitNote.aspx");
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
                com.CommandText = "Select DISTINCT Vendorname from tbl_VendorMaster where " + "Vendorname like @Search + '%'";

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
        BindBillNO();
    }

    protected void BindBillNO()
    {
        string com = "SELECT * FROM tblPurchaseBillHdr where SupplierName='" + txtSupplierName.Text.Trim() + "'";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        ddlBillNumber.DataSource = dt;
        ddlBillNumber.DataBind();
        ddlBillNumber.DataTextField = "BillNo";
        ddlBillNumber.DataValueField = "Id";
        ddlBillNumber.DataBind();
        ddlBillNumber.Items.Insert(0, new ListItem("--Select Bill Number--", "0"));
    }

    protected void Insert(object sender, EventArgs e)
    {
        if (txtQty.Text == "" || txtParticulars.Text == "" || txtTotalamt.Text == "")
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

        dt.Rows.Add(ViewState["RowNo"], txtParticulars.Text, txtDescription.Text, txtHSN.Text, txtQty.Text, txtRate.Text, txtAmountt.Text, CGSTPer.Text, CGSTAmt.Text, SGSTPer.Text, SGSTAmt.Text, IGSTPer.Text, IGSTAmt.Text, txtTotalamt.Text, txtDisct.Text);
        ViewState["ParticularDetails"] = dt;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();

        txtParticulars.Text = string.Empty;
        txtDescription.Text = string.Empty;
        txtQty.Text = string.Empty;
        txtHSN.Text = string.Empty;
        txtRate.Text = string.Empty;
        txtAmountt.Text = string.Empty;
        CGSTPer.Text = string.Empty;
        CGSTAmt.Text = string.Empty;
        SGSTPer.Text = string.Empty;
        SGSTAmt.Text = string.Empty;
        IGSTPer.Text = string.Empty;
        IGSTAmt.Text = string.Empty;
        txtDisct.Text = string.Empty;
        txtTotalamt.Text = string.Empty;
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
        string Description = ((TextBox)row.FindControl("txtDescr")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((Label)row.FindControl("lblRate")).Text;
        string Amount = ((Label)row.FindControl("lblAmount")).Text;
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        string CGSTAmt = ((TextBox)row.FindControl("txtCGSTAmt")).Text;
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        string SGSTAmt = ((TextBox)row.FindControl("txtSGSTAmt")).Text;
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        string IGSTAmt = ((TextBox)row.FindControl("txtIGSTAmt")).Text;
        string TotalAmount = ((Label)row.FindControl("lblTotalAmount")).Text;
        string disc = ((TextBox)row.FindControl("txtdisc")).Text;

        DataTable Dt = ViewState["ParticularDetails"] as DataTable;

        Dt.Rows[row.RowIndex]["Particulars"] = Particulars;
        Dt.Rows[row.RowIndex]["Description"] = Description;
        Dt.Rows[row.RowIndex]["HSN"] = HSN;
        Dt.Rows[row.RowIndex]["Qty"] = Qty;
        Dt.Rows[row.RowIndex]["Rate"] = Rate;
        Dt.Rows[row.RowIndex]["Amount"] = Amount;
        Dt.Rows[row.RowIndex]["CGSTPer"] = CGSTPer;
        Dt.Rows[row.RowIndex]["CGSTAmt"] = CGSTAmt;
        Dt.Rows[row.RowIndex]["SGSTPer"] = SGSTPer;
        Dt.Rows[row.RowIndex]["SGSTAmt"] = SGSTAmt;
        Dt.Rows[row.RowIndex]["IGSTPer"] = IGSTPer;
        Dt.Rows[row.RowIndex]["IGSTAmt"] = IGSTAmt;
        Dt.Rows[row.RowIndex]["TotalAmount"] = TotalAmount;
        Dt.Rows[row.RowIndex]["Discount"] = disc;

        Dt.AcceptChanges();

        ViewState["ParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["ParticularDetails"];
        dgvParticularsDetails.DataBind();
        foreach (GridViewRow gv in dgvParticularsDetails.Rows)
        {
            Label GrandTotal = (Label)gv.FindControl("lblTotalAmount");
            Grandtotal2 += Convert.ToDouble(GrandTotal.Text);
        }
        txtFGrandTot.Text = Convert.ToDouble(txtTCost.Text) + Grandtotal2.ToString();

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

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetItemList(string prefixText, int count)
    {
        return AutoFilItem(prefixText);
    }

    public static List<string> AutoFilItem(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [ProductName] from tbl_ProductMaster where " + "ProductName like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> Items = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        Items.Add(sdr["ProductName"].ToString());
                    }
                }
                con.Close();
                return Items;
            }
        }
    }

    protected void txtParticulars_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT HSN,PurchaseRate FROM tblItemMaster where ItemName='" + txtParticulars.Text.Trim() + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtHSN.Text = dt.Rows[0]["HSN"].ToString() == "" ? "0" : dt.Rows[0]["HSN"].ToString();
            txtRate.Text = dt.Rows[0]["PurchaseRate"].ToString() == "" ? "0" : dt.Rows[0]["PurchaseRate"].ToString();
        }
        else
        {

        }
    }

    private void GST_Calculation()
    {
        var TotalAmt = Convert.ToDecimal(txtQty.Text.Trim()) * Convert.ToDecimal(txtRate.Text == "" ? "0" : txtRate.Text);

        decimal AmtwithDisc;
        if (txtDisct.Text != "0")
        {
            var disAmt = TotalAmt * Convert.ToDecimal(txtDisct.Text == "" ? "0" : txtDisct.Text) / 100;
            AmtwithDisc = TotalAmt - disAmt;
        }
        else
        {
            AmtwithDisc = TotalAmt;
        }

        txtAmountt.Text = AmtwithDisc.ToString();

        decimal CGST;
        if (string.IsNullOrEmpty(CGSTPer.Text))
        {
            CGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(AmtwithDisc);
            decimal Val2 = Convert.ToDecimal(CGSTPer.Text);

            CGST = (Val1 * Val2 / 100);
        }
        CGSTAmt.Text = CGST.ToString();

        decimal SGST;
        if (string.IsNullOrEmpty(SGSTPer.Text))
        {
            SGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(AmtwithDisc);
            decimal Val2 = Convert.ToDecimal(SGSTPer.Text);

            SGST = (Val1 * Val2 / 100);
        }
        SGSTAmt.Text = SGST.ToString();


        decimal IGST;
        if (string.IsNullOrEmpty(IGSTPer.Text))
        {
            IGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(AmtwithDisc);
            decimal Val2 = Convert.ToDecimal(IGSTPer.Text);

            IGST = (Val1 * Val2 / 100);
        }
        IGSTAmt.Text = IGST.ToString();

        decimal GSTTotal = 0;
        if (IGSTPer.Text != "0")
        {
            GSTTotal = IGST;
        }
        else
        {
            GSTTotal = CGST + SGST;
        }


        txtTotalamt.Text = (AmtwithDisc + GSTTotal).ToString();
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
        CGSTPer.Text = SGSTPer.Text;
        SGSTAmt.Text = CGSTAmt.Text;

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
        GST_Calculation();
    }

    protected void CGSTPer_TextChanged(object sender, EventArgs e)
    {
        SGSTPer.Text = CGSTPer.Text;
        SGSTAmt.Text = CGSTAmt.Text;


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
        GST_Calculation();
    }
    Double Grandtotal2 = 0;
    private void GRID_GST_Calculation(GridViewRow row)
        {
        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((Label)row.FindControl("lblRate")).Text;
        string Amount = ((Label)row.FindControl("lblAmount")).Text;
        Label lblamt = (Label)row.FindControl("lblAmount");
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        TextBox CGSTAmt = (TextBox)row.FindControl("txtCGSTAmt");
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        TextBox SGSTAmt = (TextBox)row.FindControl("txtSGSTAmt");
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        TextBox IGSTAmt = (TextBox)row.FindControl("txtIGSTAmt");
        Label TotalAmount = (Label)row.FindControl("lblTotalAmount");
        TextBox disc = (TextBox)row.FindControl("txtdisc");

        var totalamt = Convert.ToDecimal(Qty) * Convert.ToDecimal(Rate);
        decimal AmtwithDisc;
        if (disc.Text != "0")
        {
            var discamt = totalamt * Convert.ToDecimal(disc.Text) / 100;
            AmtwithDisc = totalamt - discamt;
        }
        else
        {
            AmtwithDisc = totalamt;
        }
        lblamt.Text = AmtwithDisc.ToString();

        decimal Vcgst;
        if (string.IsNullOrEmpty(CGSTAmt.Text))
        {
            Vcgst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(AmtwithDisc);
            decimal val2 = Convert.ToDecimal(CGSTPer == "" ? "0" : CGSTPer);

            Vcgst = (val1 * val2 / 100);
        }
        CGSTAmt.Text = Vcgst.ToString();

        decimal Vsgst;
        if (string.IsNullOrEmpty(SGSTAmt.Text))
        {
            Vsgst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(AmtwithDisc);
            decimal val2 = Convert.ToDecimal(SGSTPer == "" ? "0" : SGSTPer);

            Vsgst = (val1 * val2 / 100);
        }
        SGSTAmt.Text = Vsgst.ToString();

        decimal Vigst;
        if (string.IsNullOrEmpty(IGSTAmt.Text))
        {
            Vigst = 0;
        }
        else
        {
            decimal val1 = Convert.ToDecimal(AmtwithDisc);
            decimal val2 = Convert.ToDecimal(IGSTPer == "" ? "0" : IGSTPer);

            Vigst = (val1 * val2 / 100);
        }
        IGSTAmt.Text = Vigst.ToString();

        decimal GSTTotal;
        if (IGSTPer == "0")
        {
            GSTTotal = Vcgst + Vsgst;
        }
        else
        {
            GSTTotal = Vigst;
        }

        TotalAmount.Text = (AmtwithDisc + GSTTotal).ToString();
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

    Double Totalamt = 0, GrandTotal = 0;
    protected void dgvParticularsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            Totalamt += Convert.ToDouble((e.Row.FindControl("lblTotalAmount") as Label).Text);
            hdnGrandtotal.Value = Totalamt.ToString();
            GrandTotal = Convert.ToDouble(Totalamt.ToString()) + Convert.ToDouble(txtTCost.Text);
            txtFGrandTot.Text = GrandTotal.ToString();


        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            (e.Row.FindControl("lbltotal") as Label).Text = Totalamt.ToString();
        }
    }

    protected void ddlBillNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        string com = "SELECT * FROM tblPurchaseBillHdr where BillNo='" + ddlBillNumber.SelectedItem.Text.Trim() + "'";
        SqlDataAdapter adpt = new SqlDataAdapter(com, con);
        DataTable dt = new DataTable();
        adpt.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            //string str = dt.Rows[0]["Invoicedate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time = Convert.ToDateTime(str);
            txtBillDate.Text = dt.Rows[0]["BillDate"].ToString();
        }

        getOrderDatailsdts();
    }

    protected void ddlProcess_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlProcess.Text == "Automatic")
        {
            DivManual.Visible = false;
            DivAutomatic.Visible = true;
            ddlBillNumber.Enabled = true;
            hdnGrandtotal.Value = "";
            txtFGrandTot.Text = "";
        }
        else
        {
            DivManual.Visible = true;
            DivAutomatic.Visible = false;
            ddlBillNumber.Enabled = false;
            hdnGrandtotal.Value = "";
            txtFGrandTot.Text = "";
        }
    }

    protected void getOrderDatailsdts()
    {
        string ID = ddlBillNumber.SelectedValue;
        SqlDataAdapter ad = new SqlDataAdapter("select * from tblPurchaseBillDtls where HeaderID='" + ID.Trim() + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            dgvAutomatic.DataSource = dt;
            dgvAutomatic.DataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('PO Details Not Found !!');", true);
        }
    }

    protected void txtautQty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    Double FinalGeandTotal = 0;
    private void calculationA(GridViewRow row)
    {
        foreach (GridViewRow gv in dgvAutomatic.Rows)
        {
            CheckBox chkrow = (CheckBox)gv.FindControl("chkSelect");
            TextBox txt_Qty = (TextBox)gv.FindControl("txtautQty");
            TextBox txt_price = (TextBox)gv.FindControl("txtRate");
            TextBox txt_CGST = (TextBox)gv.FindControl("txtCGST");
            TextBox txt_SGST = (TextBox)gv.FindControl("txtSGST");
            TextBox txt_IGST = (TextBox)gv.FindControl("txtIGST");
            TextBox txt_amount = (TextBox)gv.FindControl("txtGrandTotal");
            TextBox txtDiscPer = (TextBox)gv.FindControl("txtDiscPer");
            Label Amount = (Label)gv.FindControl("txtAmount");

            var totalamt = Convert.ToDecimal(txt_Qty.Text.Trim()) * Convert.ToDecimal(txt_price.Text.Trim());
            decimal AmtWithDiscount;
            if (txtDiscPer.Text != "" || txtDiscPer.Text != null)
            {
                var disc = totalamt * (Convert.ToDecimal(txtDiscPer.Text.Trim())) / 100;
                AmtWithDiscount = totalamt - disc;
            }
            else
            {
                AmtWithDiscount = totalamt;
            }
            Amount.Text = AmtWithDiscount.ToString();


            var CGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_CGST.Text.Trim())) / 100;
            var SGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_SGST.Text.Trim())) / 100;
            var IGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_IGST.Text.Trim())) / 100;

            decimal GSTtotal;
            if (txt_IGST.Text == "0")
            {
                GSTtotal = SGSTamt + CGSTamt;
            }
            else
            {
                GSTtotal = IGSTamt;
            }

            var NetAmt = AmtWithDiscount + GSTtotal;

            txt_amount.Text = NetAmt.ToString("##.00");
            if (chkrow.Checked == true)
            {
                FinalGeandTotal += Convert.ToDouble(txt_amount.Text);
            }
        }
        Double GrantotalFinal = Convert.ToDouble(FinalGeandTotal.ToString()) + Convert.ToDouble(txtTCost.Text);
        txtFGrandTot.Text = GrantotalFinal.ToString();
    }

    protected void txtCGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtSGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtIGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    decimal GrandTotalamt = 0;
    protected void dgvAutomatic_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            GrandTotalamt += Convert.ToDecimal((e.Row.FindControl("txtGrandTotal") as TextBox).Text);
            hdnGrandtotal.Value = GrandTotalamt.ToString();
            txtFGrandTot.Text = GrandTotalamt.ToString();
            TextBox CGST = (TextBox)e.Row.FindControl("txtCGST");
            TextBox SGST = (TextBox)e.Row.FindControl("txtSGST");
            TextBox IGST = (TextBox)e.Row.FindControl("txtIGST");

            if (IGST.Text == "0")
            {
                txtTCGSTPer.Text = CGST.Text;
                txtTSGSTPer.Text = SGST.Text;
                txtTIGSTPer.Enabled = false;
            }
            else
            {
                txtTCGSTPer.Enabled = false;
                txtTSGSTPer.Enabled = false;
                txtTIGSTPer.Text = IGST.Text;
            }


            TextBox txt_Qty = (TextBox)e.Row.FindControl("txtautQty");
            CheckBox chk = (CheckBox)e.Row.FindControl("chkSelect");
            CheckBox chkheader = (CheckBox)dgvAutomatic.HeaderRow.FindControl("chkHeader");
            int id = Convert.ToInt32(dgvAutomatic.DataKeys[e.Row.RowIndex].Values[0]);
            int sumofqty;
            DataTable dtinvoice = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from tblCreditDebitNoteHdr where BillNumber='" + ddlBillNumber.SelectedItem.Text + "'", con);
            sad.Fill(dtinvoice);
            // string HeaderId = dtinvoice.Rows[0]["Id"].ToString();
            SqlCommand cmdmax = new SqlCommand("SELECT Max(Id) FROM tblCreditDebitNoteDtls where InvoiceNo='" + ddlBillNumber.SelectedItem.Text + "'", con);
            con.Open();
            Object mxid = cmdmax.ExecuteScalar();
            con.Close();

            SqlCommand cmdsumQty = new SqlCommand("SELECT SUM(CAST(Qty as int)) FROM tblCreditDebitNoteDtls where InvoiceNo='" + ddlBillNumber.SelectedItem.Text + "'", con);
            con.Open();
            Object smQty = cmdsumQty.ExecuteScalar();
            //sumofqty = Convert.ToInt32(smQty);
            con.Close();

            var mxiddd = mxid.ToString() == "" ? null : mxid.ToString();
            var smquantity = smQty.ToString() == "" ? "0" : smQty.ToString();

            DataTable dtTIDetails = new DataTable();
            SqlDataAdapter saTIDetails = new SqlDataAdapter("select * from tblCreditDebitNoteDtls where InvoiceNo='" + ddlBillNumber.SelectedItem.Text + "' and Id='" + Convert.ToInt32(mxiddd) + "'", con);
            saTIDetails.Fill(dtTIDetails);
            if (dtTIDetails.Rows.Count > 0)
            {
                //var ExistQty = dtTIDetails.Rows[0]["Qty"].ToString();
                //var Qty = txt_Qty.Text;
                var minusQty = Convert.ToInt32(txt_Qty.Text) - Convert.ToInt32(smquantity);
                txt_Qty.Text = minusQty.ToString();
                if (minusQty == 0)
                {
                    chk.Enabled = false;
                    chkheader.Enabled = false;
                }

            }
        }
    }

    Double GrandAmount = 0;
    protected void chkSelect_CheckedChanged(object sender, EventArgs e)
    {

        foreach (GridViewRow row in dgvAutomatic.Rows)
        {
            TextBox txt_Qty = (TextBox)row.FindControl("txtautQty");
            TextBox description = (TextBox)row.FindControl("txtDescription");
            TextBox CGST = (TextBox)row.FindControl("txtCGST");
            TextBox SGST = (TextBox)row.FindControl("txtSGST");
            TextBox IGST = (TextBox)row.FindControl("txtIGST");
            CheckBox ChkRow = (CheckBox)row.FindControl("chkSelect");
            TextBox GrandTotal = (TextBox)row.FindControl("txtGrandTotal");

            if (ChkRow.Checked == true)
            {

                txt_Qty.Enabled = true;
                if (IGST.Text == "" || IGST.Text == "0")
                {
                    CGST.Enabled = true;
                    SGST.Enabled = true;
                }
                else
                {
                    IGST.Enabled = true;
                }


                GrandAmount += Convert.ToDouble(GrandTotal.Text);
                // calculationA(row);
            }
            else
            {
                CGST.Enabled = false;
                SGST.Enabled = false;
                IGST.Enabled = false;
                txt_Qty.Enabled = false;
            }
        }
        Double Geandtotal = Convert.ToDouble(GrandAmount.ToString()) + Convert.ToDouble(txtTCost.Text);
        txtFGrandTot.Text = Geandtotal.ToString();

    }

    protected void chkHeader_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in dgvAutomatic.Rows)
        {
            TextBox txt_Qty = (TextBox)row.FindControl("txtautQty");
            TextBox description = (TextBox)row.FindControl("txtDescription");
            TextBox CGST = (TextBox)row.FindControl("txtCGST");
            TextBox SGST = (TextBox)row.FindControl("txtSGST");
            TextBox IGST = (TextBox)row.FindControl("txtIGST");
            CheckBox ChkRow = (CheckBox)row.FindControl("chkSelect");
            CheckBox ChkHeader = (CheckBox)dgvAutomatic.HeaderRow.FindControl("chkHeader");
            TextBox GrandTotal = (TextBox)row.FindControl("txtGrandTotal");

            if (ChkHeader.Checked == true)
            {
                ChkRow.Checked = true;
                txt_Qty.Enabled = true;
                //CGST.Enabled = true;
                //SGST.Enabled = true;
                //IGST.Enabled = true;
                if (IGST.Text == "" || IGST.Text == "0")
                {
                    CGST.Enabled = true;
                    SGST.Enabled = true;
                }
                else
                {
                    IGST.Enabled = true;
                }
                GrandAmount += Convert.ToDouble(GrandTotal.Text);
                // calculationA(row);
            }
            else
            {
                ChkRow.Checked = false;
                CGST.Enabled = false;
                SGST.Enabled = false;
                IGST.Enabled = false;
                txt_Qty.Enabled = false;
            }
        }
        Double Geandtotal = Convert.ToDouble(GrandAmount.ToString()) + Convert.ToDouble(txtTCost.Text);
        txtFGrandTot.Text = Geandtotal.ToString();

    }
    Double GrandAmount1 = 0, GAmount = 0;
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
        if (dgvAutomatic.Rows.Count > 0)
        {
            foreach (GridViewRow row in dgvAutomatic.Rows)
            {
                TextBox GrandTotalGrid = (TextBox)row.FindControl("txtGrandTotal");
                GrandAmount1 += Convert.ToDouble(GrandTotalGrid.Text);
            }
            GAmount = Convert.ToDouble(txtTCost.Text) + Convert.ToDouble(GrandAmount1.ToString());
            txtFGrandTot.Text = GAmount.ToString();
        }
        else
        {
            foreach (GridViewRow row in dgvParticularsDetails.Rows)
            {
                Label GrandTotalGrid = (Label)row.FindControl("lblTotalAmount");
                GrandAmount1 += Convert.ToDouble(GrandTotalGrid.Text);
            }
            GAmount = Convert.ToDouble(txtTCost.Text) + Convert.ToDouble(GrandAmount1.ToString());
            txtFGrandTot.Text = GAmount.ToString();
        }

    }

    protected void txtTCharge_TextChanged(object sender, EventArgs e)
    {
        Transportation_Calculation();
    }

    protected void txtTCGSTPer_TextChanged(object sender, EventArgs e)
    {
        Transportation_Calculation();
    }

    protected void txtTSGSTPer_TextChanged(object sender, EventArgs e)
    {
        Transportation_Calculation();
    }

    protected void txtTIGSTPer_TextChanged(object sender, EventArgs e)
    {
        Transportation_Calculation();
    }

    protected void txtPaymentDueDate_TextChanged(object sender, EventArgs e)
    {
        //DateTime fromdate = DateTime.Parse(Convert.ToDateTime(txtDocdate.Text).ToShortDateString());
        //DateTime todate = DateTime.Parse(Convert.ToDateTime(txtPaymentDueDate.Text).ToShortDateString());
        DateTime fromdate = Convert.ToDateTime(txtDocdate.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        DateTime todate = Convert.ToDateTime(txtPaymentDueDate.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);

        if (fromdate > todate)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Document Date Date is greater than Payment Due Date...Please Choose Correct Date.');", true);
            btnadd.Enabled = false;
        }
        else
        {
            btnadd.Enabled = true;
        }
    }

    protected void txtDisct_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtdisc_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtDiscPer_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
    }

    protected void ddlNoteType_TextChanged(object sender, EventArgs e)
    {
        if (ddlNoteType.SelectedValue == "Select")
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Kindly Select Note Type..!!!');", true);
            txtDocNo.Text = "";
        }
        else
        {
            GenerateComCode();
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CreditDebitNoteList.aspx");
    }
}