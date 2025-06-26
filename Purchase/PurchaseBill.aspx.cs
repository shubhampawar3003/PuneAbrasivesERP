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
using System.Activities.Expressions;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.Net;
using DocumentFormat.OpenXml.Office2010.Excel;



public partial class Purchase_PurchaseBill : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    DataTable dt = new DataTable();
    public static string sName = "";
    DataTable vdt = new DataTable();
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
                //fillddlpaymentterm();
                //fillddlFooter();
              //  fillddlUnit();
                Session["Iscompleted"] = "true";
                txtBilldate.Text = DateTime.Today.ToString("dd-MM-yyyy");
                UpdateHistorymsg = string.Empty; //regdate = string.Empty;
                if (Request.QueryString["ID"] != null)
                {
                    ViewState["RowNo"] = 0;
                    dt.Columns.AddRange(new DataColumn[14] { new DataColumn("id"),
                 new DataColumn("Particulars"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("POId")


                });
                    ViewState["ParticularDetails"] = dt;
                    ViewState["UpdateRowId"] = Decrypt(Request.QueryString["ID"].ToString());


                    //Verbal DT

                    ViewState["RowNo"] = 0;
                    vdt.Columns.AddRange(new DataColumn[16] { new DataColumn("id"),
                 new DataColumn("Particulars"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"),new DataColumn("Discount"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Description"),new DataColumn("UOM")
                });
                    ViewState["VParticularDetails"] = vdt;
                    GetPurchaseBillData(ViewState["UpdateRowId"].ToString());
                }
                else
                {
                    ViewState["RowNo"] = 0;
                    dt.Columns.AddRange(new DataColumn[14] { new DataColumn("id"),
                 new DataColumn("Particulars"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("POId")
                });
                    ViewState["ParticularDetails"] = dt;
                    txtBillNo.Text = Code();

                    //verbal DT

                    ViewState["RowNo"] = 0;
                    vdt.Columns.AddRange(new DataColumn[16] { new DataColumn("id"),
                 new DataColumn("Particulars"), new DataColumn("HSN")
                , new DataColumn("Qty"), new DataColumn("Rate"),new DataColumn("Discount"), new DataColumn("Amount"),
                    new DataColumn("CGSTPer"),new DataColumn("CGSTAmt"),new DataColumn("SGSTPer")
                    ,new DataColumn("SGSTAmt"),new DataColumn("IGSTPer"),new DataColumn("IGSTAmt"),new DataColumn("TotalAmount"),new DataColumn("Description"),new DataColumn("UOM")
                });
                    ViewState["VParticularDetails"] = vdt;
                }
            }
        }
    }

    private void fillddlUnit()
    {
        SqlDataAdapter adpt = new SqlDataAdapter("select distinct Unit from tblUnit", con);
        DataTable dtpt = new DataTable();
        adpt.Fill(dtpt);

        if (dtpt.Rows.Count > 0)
        {
            txtUOM.DataSource = dtpt;
            txtUOM.DataValueField = "Unit";
            txtUOM.DataTextField = "Unit";
            txtUOM.DataBind();
        }
        //txtUOM.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Nill", "0"));
    }

    static string regdate = string.Empty;
    protected void GetPurchaseBillData(string id)
    {
        string query1 = string.Empty;
        query1 = @"select * from tblPurchaseBillHdr where Id='" + id + "' ";
        SqlDataAdapter ad = new SqlDataAdapter(query1, con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            btnadd.Text = "Update";
            txtSupplierName.Text = dt.Rows[0]["SupplierName"].ToString();

            BindPO();
            txtSupplierBillNo.Text = dt.Rows[0]["SupplierBillNo"].ToString();
            txtBillNo.Text = dt.Rows[0]["BillNo"].ToString();

            //string str = dt.Rows[0]["BillDate"].ToString();
            //str = str.Replace("00:00:00 AM", "");
            //var time = Convert.ToDateTime(str);
            DateTime BillDate = Convert.ToDateTime(dt.Rows[0]["BillDate"].ToString());
            txtBilldate.Text = BillDate.ToString("dd-MM-yyyy");
            //txtBilldate.Text = dt.Rows[0]["BillDate"].ToString();

            ddlBillAgainst.Text = dt.Rows[0]["BillAgainst"].ToString();
            ddlAgainstNumber.SelectedItem.Text = dt.Rows[0]["AgainstNumber"].ToString();
            txtTransportMode.Text = dt.Rows[0]["TransportMode"].ToString();
            txtVehicleNumber.Text = dt.Rows[0]["VehicleNo"].ToString();

            //string strdue = dt.Rows[0]["PaymentDueDate"].ToString();
            //strdue = strdue.Replace("00:00:00 AM", "");
            //var timedue = Convert.ToDateTime(strdue);
            txtPaymentDueDate.Text = dt.Rows[0]["PaymentDueDate"].ToString();

            txtAccontHead.Text = dt.Rows[0]["AccountHead"].ToString();
            txtRemark.Text = dt.Rows[0]["Remarks"].ToString();
            txtEBillNumber.Text = dt.Rows[0]["EBillNumber"].ToString();
            txtDescription.Text = dt.Rows[0]["ChargesDescription"].ToString();
            txtHSN.Text = dt.Rows[0]["HSNSAC"].ToString();
            txtRate.Text = dt.Rows[0]["Rate"].ToString();
            txtBasic.Text = dt.Rows[0]["Basic"].ToString();
            CGSTPer.Text = dt.Rows[0]["CGSTPer"].ToString();
            SGSTPer.Text = dt.Rows[0]["SGSTPer"].ToString();
            IGSTPer.Text = dt.Rows[0]["IGSTPer"].ToString();
            txtCost.Text = dt.Rows[0]["Cost"].ToString();
            txtTCSPer.Text = dt.Rows[0]["TCSPer"].ToString();
            txtTCSAmt.Text = dt.Rows[0]["TCSAmount"].ToString();
            txtGrandTot.Text = dt.Rows[0]["GrandTotal"].ToString();
            hdnfileData.Value = dt.Rows[0]["RefDocument"].ToString();
            //17 march 2022
            txtTCharge.Text = dt.Rows[0]["TransportationCharges"].ToString();
            txtTCGSTPer.Text = dt.Rows[0]["TCGSTPer"].ToString();
            txtTCGSTamt.Text = dt.Rows[0]["TCGSTAmt"].ToString();
            txtTSGSTPer.Text = dt.Rows[0]["TSGSTPer"].ToString();
            txtTSGSTamt.Text = dt.Rows[0]["TSGSTAmt"].ToString();
            txtTIGSTPer.Text = dt.Rows[0]["TIGSTPer"].ToString();
            txtTIGSTamt.Text = dt.Rows[0]["TIGSTAmt"].ToString();
            txtTCost.Text = dt.Rows[0]["TotalCost"].ToString();

            txtDOR.Text = dt.Rows[0]["DateOfReceived"].ToString();

            if (dt.Rows[0]["RefDocument"].ToString() != "")
            {
                spnFileUploadData.InnerText = "File Already Exsist, if you can update then update it.";
            }
            else
            {
                spnFileUploadData.InnerText = "File Not Found";
            }

            if (dt.Rows[0]["BillAgainst"].ToString() == "Verbal")
            {
                getVParticularsdts(id);
                DivVerbal.Visible = true;
                divVerbaldtls.Visible = true;
            }
            else if (dt.Rows[0]["BillAgainst"].ToString() == "Order")
            {
                getParticularsdts(id);
            }
        }
    }

    protected void getVParticularsdts(string id)
    {

        DataTable Dtproduct = new DataTable();
        SqlDataAdapter daa = new SqlDataAdapter("select * from tblPurchaseBillDtls where HeaderID='" + id + "'", con);
        daa.Fill(Dtproduct);
        ViewState["RowNo"] = (int)ViewState["RowNo"] + 1;

        DataTable dt = ViewState["VParticularDetails"] as DataTable;

        if (Dtproduct.Rows.Count > 0)
        {
            for (int i = 0; i < Dtproduct.Rows.Count; i++)
            {
                dt.Rows.Add(ViewState["RowNo"], Dtproduct.Rows[i]["Particulars"].ToString(), Dtproduct.Rows[i]["HSN"].ToString(), Dtproduct.Rows[i]["Qty"].ToString(),
                    Dtproduct.Rows[i]["Rate"].ToString(), Dtproduct.Rows[i]["Discount"].ToString(), Dtproduct.Rows[i]["Amount"].ToString(), Dtproduct.Rows[i]["CGSTPer"].ToString(), "0",
                    Dtproduct.Rows[i]["SGSTPer"].ToString(), "0", Dtproduct.Rows[i]["IGSTPer"].ToString(), "0",
                    Dtproduct.Rows[i]["GrandTotal"].ToString(), Dtproduct.Rows[i]["Description"].ToString(), Dtproduct.Rows[i]["UOM"].ToString());
                ViewState["VParticularDetails"] = dt;
            }
        }
        dgvParticularsDetails.DataSource = dt;
        dgvParticularsDetails.DataBind();
    }

    protected void getParticularsdts(string id)
    {
        SqlDataAdapter ad = new SqlDataAdapter("select * from tblPurchaseBillDtls where HeaderID='" + id + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            dgvOrderDtl.DataSource = dt;
            dgvOrderDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('PO Details Not Found !!');", true);
        }
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

        SqlDataAdapter ad = new SqlDataAdapter("SELECT max([Id]) as maxid FROM [tblPurchaseBillHdr]", con);
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
        string strSelect = @"select ISNULL(MAX(BillNo), '') AS maxno from tblPurchaseBillHdr where BillNo like '%" + fY + "%'";
        //string strSelect = @"SELECT TOP 1 BillNo FROM tblPurchaseBillHdr where BillNo like '%" + fY + "%' ORDER BY ID DESC";
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = strSelect;
        con.Open();
        string result = cmd.ExecuteScalar().ToString();
        //string result = "";
        con.Close();
        if (result != "")
        {
            int numbervalue = Convert.ToInt32(result.Substring(result.IndexOf("/") + 1, result.Length - (result.IndexOf("/") + 1)));
            numbervalue = numbervalue + 1;
            strInvoiceNumber = result.Substring(0, result.IndexOf("/") + 1) + "" + numbervalue.ToString("00");
        }
        else
        {
            strInvoiceNumber = previousyear.ToString() + "-" + FinYear + "/" + "01";
        }
        return strInvoiceNumber;
    }


    bool flg = false;
    protected void btnadd_Click(object sender, EventArgs e)
    {
        #region Insert
        if (btnadd.Text == "Submit")
        {
            SqlCommand cmd = new SqlCommand("SP_PurchaseBill", con);
            cmd.Parameters.Clear();
            cmd.CommandType = CommandType.StoredProcedure;

            byte[] bytes = null;
            if (UploadRefDocs.HasFile)
            {
                string filename = Path.GetFileName(UploadRefDocs.PostedFile.FileName);
                string contentType = UploadRefDocs.PostedFile.ContentType;
                using (Stream fs = UploadRefDocs.PostedFile.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((Int32)fs.Length);
                    }
                }
            }
            if (ddlBillAgainst.Text == "Order")
            {
                foreach (GridViewRow row in dgvOrderDtl.Rows)
                {
                    bool chk = (row.FindControl("chkSelect") as CheckBox).Checked;
                    if (chk == true)
                    {
                        flg = true;
                        break;
                    }
                    else
                    {
                        flg = false;
                    }
                }
            }
            else if (ddlBillAgainst.Text == "Verbal")
            {
                if (dgvParticularsDetails.Rows.Count > 0)
                {
                    flg = true;
                }
                else
                {
                    flg = false;
                }
            }

            if (flg == true)
            {

                //else
                //{
                cmd.Parameters.AddWithValue("@Action", "insert");
                cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
                cmd.Parameters.AddWithValue("@SupplierBillNo", txtSupplierBillNo.Text.Trim());
                cmd.Parameters.AddWithValue("@BillNo", txtBillNo.Text.Trim());
                DateTime BillDate = Convert.ToDateTime(txtBilldate.Text.ToString(), System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                txtBilldate.Text = BillDate.ToString("yyyy-MM-dd");
                cmd.Parameters.AddWithValue("@BillDate", txtBilldate.Text);
                cmd.Parameters.AddWithValue("@BillAgainst", ddlBillAgainst.Text.Trim());
                cmd.Parameters.AddWithValue("@AgainstNumber", ddlBillAgainst.Text == "Verbal" ? DBNull.Value.ToString() : ddlAgainstNumber.SelectedItem.Text.Trim());
                cmd.Parameters.AddWithValue("@TransportMode", txtTransportMode.Text.Trim());
                cmd.Parameters.AddWithValue("@VehicleNo", txtVehicleNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@TransportDescription", txtTransportMode.Text.Trim());
                cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text.Trim());
                cmd.Parameters.AddWithValue("@RefDocument", bytes);
                cmd.Parameters.AddWithValue("@AccountHead", txtAccontHead.Text.Trim());
                cmd.Parameters.AddWithValue("@Remarks", txtRemark.Text.Trim());
                cmd.Parameters.AddWithValue("@EBillNumber", txtEBillNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@ChargesDescription", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@HSNSAC", txtHSN.Text.Trim());
                cmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
                cmd.Parameters.AddWithValue("@Basic", txtBasic.Text.Trim());
                cmd.Parameters.AddWithValue("@CGSTPer", CGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@SGSTPer", SGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@IGSTPer", IGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@Cost", txtCost.Text.Trim());
                cmd.Parameters.AddWithValue("@TCSPer", txtTCSPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TCSAmount", txtTCSAmt.Text.Trim());
                cmd.Parameters.AddWithValue("GrandTotal", txtGrandTot.Text.Trim());
                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());

                //17 March 2022
                cmd.Parameters.AddWithValue("@TransportationCharges", txtTCharge.Text.Trim());
                cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text.Trim());
                cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text.Trim());
                cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text.Trim());
                cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text.Trim());

                //25 march 2022
                cmd.Parameters.AddWithValue("@DateOfReceived", txtDOR.Text.Trim());
                int a = 0;
                con.Open();
                a = cmd.ExecuteNonQuery();
                con.Close();

                SqlCommand cmdmax = new SqlCommand("select MAX(Id) as MAxID from tblPurchaseBillHdr", con);
                con.Open();
                Object mx = cmdmax.ExecuteScalar();
                con.Close();
                int MaxId = Convert.ToInt32(mx.ToString());
                int count = 0;
                if (ddlBillAgainst.Text == "Order")
                {
                    if (dgvOrderDtl.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvOrderDtl.Rows)
                        {
                            bool chk = (row.FindControl("chkSelect") as CheckBox).Checked;
                            if (chk == true)
                            {
                                count++;
                                string Particulars = ((Label)row.FindControl("txtParticulars")).Text;
                                string Description = ((TextBox)row.FindControl("txtDescription")).Text;
                                string HSN = ((TextBox)row.FindControl("txtHSN")).Text;
                                string Qty = ((TextBox)row.FindControl("txtQty")).Text;
                                string UOM = ((TextBox)row.FindControl("txtUOM")).Text;
                                string Rate = ((TextBox)row.FindControl("txtRate")).Text;
                                string Amount = ((Label)row.FindControl("txtAmount")).Text;
                                string CGST = ((TextBox)row.FindControl("txtCGST")).Text;
                                string SGST = ((TextBox)row.FindControl("txtSGST")).Text;
                                string IGST = ((TextBox)row.FindControl("txtIGST")).Text;
                                string Discount = ((TextBox)row.FindControl("txtdiscount")).Text == "" ? "0" : ((TextBox)row.FindControl("txtdiscount")).Text;
                                string GrandTotal = ((TextBox)row.FindControl("txtGrandTotal")).Text;
                                string POId = ((Label)row.FindControl("lblID")).Text;

                                SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblPurchaseBillDtls ([HeaderID],[Particulars],[Description],[HSN],[Qty],[UOM],[Rate],[Discount],[Amount],[CGSTPer],[SGSTPer],[IGSTPer],[GrandTotal],[POId])
                                VALUES(" + MaxId + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "','" + UOM + "'," +
                                 "'" + Rate + "','" + Discount + "','" + Amount + "','" + CGST + "'," +
                                 "'" + SGST + "','" + IGST + "','" + GrandTotal + "','" + POId + "')", con);
                                con.Open();
                                cmdParticulardata.ExecuteNonQuery();
                                con.Close();

                                //SqlCommand cmdgetqty = new SqlCommand("select OpeningStock from tblItemMaster where ItemName='" + Particulars + "'", con);
                                //con.Open();
                                //Object openeningstock = cmdgetqty.ExecuteScalar();
                                //con.Close();

                                //string Openningstk = openeningstock.ToString() == "" ? "0" : openeningstock.ToString();

                                //var PlusQty = Convert.ToDouble(Openningstk) + Convert.ToDouble(Qty);

                                //SqlCommand cmdupdateqty = new SqlCommand("update tblItemMaster set OpeningStock='" + PlusQty + "' where ItemName='" + Particulars + "'", con);
                                //con.Open();
                                //cmdupdateqty.ExecuteNonQuery();
                                //con.Close();
                            }
                        }
                    }

                    int numVisible = 0;
                    foreach (GridViewRow row in dgvOrderDtl.Rows)
                    {
                        if (row.Visible == true)
                        {
                            numVisible += 1;
                        }
                    }
                    if (Session["Iscompleted"].ToString() == "false" || numVisible != count)
                    {
                        SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed=null,Mode='Open' where PONo='" + ddlAgainstNumber.SelectedItem.Text + "'", con);
                        con.Open();
                        cmdupdate.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed=1,Mode='Close' where PONo='" + ddlAgainstNumber.SelectedItem.Text + "'", con);
                        con.Open();
                        cmdupdate.ExecuteNonQuery();
                        con.Close();
                    }

                    //if (dgvOrderDtl.Rows.Count == count)
                    //{
                    //    SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed=1,Mode='Close' where PONo='" + ddlAgainstNumber.SelectedItem.Text + "'", con);
                    //    con.Open();
                    //    cmdupdate.ExecuteNonQuery();
                    //    con.Close();
                    //}
                    //else
                    //{
                    //    SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed=0,Mode='Open' where PONo='" + ddlAgainstNumber.SelectedItem.Text + "'", con);
                    //    con.Open();
                    //    cmdupdate.ExecuteNonQuery();
                    //    con.Close();
                    //}
                }
                else if (ddlBillAgainst.Text == "Verbal")
                {
                    if (dgvParticularsDetails.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvParticularsDetails.Rows)
                        {
                            string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
                            string Description = ((Label)row.FindControl("lblDescription")).Text;
                            string HSN = ((Label)row.FindControl("lblHSN")).Text;
                            string Qty = ((Label)row.FindControl("lblQty")).Text;
                            string UOM = ((Label)row.FindControl("lblUOM")).Text;
                            string Rate = ((Label)row.FindControl("lblRate")).Text;
                            string Amount = ((Label)row.FindControl("lblAmount")).Text;
                            string CGST = ((Label)row.FindControl("lblCGSTPer")).Text;
                            string SGST = ((Label)row.FindControl("lblSGSTPer")).Text;
                            string IGST = ((Label)row.FindControl("lblIGSTPer")).Text;
                            string Discount = ((Label)row.FindControl("lblDiscount")).Text == "" ? "0" : ((Label)row.FindControl("lblDiscount")).Text;
                            string GrandTotal = ((Label)row.FindControl("lblTotalAmount")).Text;
                            //string POId = ((Label)row.FindControl("lblID")).Text;

                            SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblPurchaseBillDtls ([HeaderID],[Particulars],[Description],[HSN],[Qty],[UOM],[Rate],[Discount],[Amount],[CGSTPer],[SGSTPer],[IGSTPer],[GrandTotal],[POId])
                            VALUES(" + MaxId + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "','" + UOM + "'," +
                             "'" + Rate + "','" + Discount + "','" + Amount + "','" + CGST + "'," +
                             "'" + SGST + "','" + IGST + "','" + GrandTotal + "',NULL)", con);
                            con.Open();
                            cmdParticulardata.ExecuteNonQuery();
                            con.Close();

                            //SqlCommand cmdgetqty = new SqlCommand("select OpeningStock from tblItemMaster where ItemName='" + Particulars + "'", con);
                            //con.Open();
                            //Object openeningstock = cmdgetqty.ExecuteScalar();
                            //con.Close();

                            //string Openningstk = openeningstock.ToString() == "" ? "0" : openeningstock.ToString();

                            //var PlusQty = Convert.ToDouble(Openningstk) + Convert.ToDouble(Qty);

                            //SqlCommand cmdupdateqty = new SqlCommand("update tblItemMaster set OpeningStock='" + PlusQty + "' where ItemName='" + Particulars + "'", con);
                            //con.Open();
                            //cmdupdateqty.ExecuteNonQuery();
                            //con.Close();
                        }
                    }
                    //}

                    if (IsSedndMail.Checked == true)
                    {
                        string subject = "PO from";
                        Send_Mail(MaxId, subject);
                    }

                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Saved Sucessfully');window.location.href='PurchaseBillList.aspx';", true);
                }
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Add Particulars');", true);
            }
        }
        #endregion Insert

        #region Update
        if (btnadd.Text == "Update")
        {
            byte[] bytes = null;
            if (hdnfileData.Value == "")
            {
                if (UploadRefDocs.HasFile)
                {
                    string filename = Path.GetFileName(UploadRefDocs.PostedFile.FileName);
                    string contentType = UploadRefDocs.PostedFile.ContentType;
                    using (Stream fs = UploadRefDocs.PostedFile.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            bytes = br.ReadBytes((Int32)fs.Length);
                        }
                    }
                }
            }
            else
            {
                if (UploadRefDocs.HasFile)
                {
                    string filename = Path.GetFileName(UploadRefDocs.PostedFile.FileName);
                    string contentType = UploadRefDocs.PostedFile.ContentType;
                    using (Stream fs = UploadRefDocs.PostedFile.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            bytes = br.ReadBytes((Int32)fs.Length);
                        }
                    }
                }
            }

            if (ddlBillAgainst.Text == "Order")
            {
                foreach (GridViewRow row in dgvOrderDtl.Rows)
                {
                    bool chk = (row.FindControl("chkSelect") as CheckBox).Checked;
                    if (chk == true)
                    {
                        flg = true;
                        break;
                    }
                    else
                    {
                        flg = false;
                    }
                }
            }
            else if (ddlBillAgainst.Text == "Verbal")
            {
                if (dgvParticularsDetails.Rows.Count > 0)
                {
                    flg = true;
                }
                else
                {
                    flg = false;
                }
            }

            if (flg == true)
            {
                SqlCommand cmd = new SqlCommand("SP_PurchaseBill", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Action", "update");
                cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(ViewState["UpdateRowId"].ToString()));
                cmd.Parameters.AddWithValue("@SupplierName", txtSupplierName.Text.Trim());
                cmd.Parameters.AddWithValue("@SupplierBillNo", txtSupplierBillNo.Text.Trim());
                cmd.Parameters.AddWithValue("@BillNo", txtBillNo.Text.Trim());
                DateTime BillDate = Convert.ToDateTime(txtBilldate.Text.ToString(), System.Globalization.CultureInfo.GetCultureInfo("ur-PK").DateTimeFormat);
                txtBilldate.Text = BillDate.ToString("yyyy-MM-dd");
                cmd.Parameters.AddWithValue("@BillDate", txtBilldate.Text);
                cmd.Parameters.AddWithValue("@BillAgainst", ddlBillAgainst.Text.Trim());
                cmd.Parameters.AddWithValue("@AgainstNumber", ddlAgainstNumber.SelectedItem.Text.Trim());
                cmd.Parameters.AddWithValue("@TransportMode", txtTransportMode.Text.Trim());
                cmd.Parameters.AddWithValue("@VehicleNo", txtVehicleNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@TransportDescription", txtTransportMode.Text.Trim());
                cmd.Parameters.AddWithValue("@PaymentDueDate", txtPaymentDueDate.Text.Trim());
                cmd.Parameters.AddWithValue("@AccountHead", txtAccontHead.Text.Trim());
                cmd.Parameters.AddWithValue("@Remarks", txtRemark.Text.Trim());
                cmd.Parameters.AddWithValue("@EBillNumber", txtEBillNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@ChargesDescription", txtDescription.Text.Trim());
                cmd.Parameters.AddWithValue("@HSNSAC", txtHSN.Text.Trim());
                cmd.Parameters.AddWithValue("@Rate", txtRate.Text.Trim());
                cmd.Parameters.AddWithValue("@Basic", txtBasic.Text.Trim());
                cmd.Parameters.AddWithValue("@CGSTPer", CGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@SGSTPer", SGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@IGSTPer", IGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@Cost", txtCost.Text.Trim());
                cmd.Parameters.AddWithValue("@TCSPer", txtTCSPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TCSAmount", txtTCSAmt.Text.Trim());
                cmd.Parameters.AddWithValue("@GrandTotal", txtGrandTot.Text.Trim());
                cmd.Parameters.AddWithValue("@CreatedBy", Session["Username"].ToString());

                //17 March 2022
                cmd.Parameters.AddWithValue("@TransportationCharges", txtTCharge.Text.Trim());
                cmd.Parameters.AddWithValue("@TCGSTPer", txtTCGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TCGSTAmt", txtTCGSTamt.Text.Trim());
                cmd.Parameters.AddWithValue("@TSGSTPer", txtTSGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TSGSTAmt", txtTSGSTamt.Text.Trim());
                cmd.Parameters.AddWithValue("@TIGSTPer", txtTIGSTPer.Text.Trim());
                cmd.Parameters.AddWithValue("@TIGSTAmt", txtTIGSTamt.Text.Trim());
                cmd.Parameters.AddWithValue("@TotalCost", txtTCost.Text.Trim());

                //25 March 2022
                cmd.Parameters.AddWithValue("@DateOfReceived", txtDOR.Text.Trim());

                if (hdnfileData.Value == "")
                {
                    cmd.Parameters.AddWithValue("@RefDocument", bytes);
                }
                else
                {
                    if (UploadRefDocs.HasFile)
                    {
                        cmd.Parameters.AddWithValue("@RefDocument", hdnfileData.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RefDocument", bytes);
                    }
                }
                int a = 0;
                cmd.Connection.Open();
                a = cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                SqlCommand cmddelete = new SqlCommand("delete from tblPurchaseBillDtls where HeaderID='" + Convert.ToInt32(ViewState["UpdateRowId"].ToString()) + "'", con);
                con.Open();
                cmddelete.ExecuteNonQuery();
                con.Close();

                if (ddlBillAgainst.Text == "Order")
                {
                    if (dgvOrderDtl.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvOrderDtl.Rows)
                        {
                            bool chk = (row.FindControl("chkSelect") as CheckBox).Checked;
                            if (chk == true)
                            {
                                string Particulars = ((Label)row.FindControl("txtParticulars")).Text;
                                string Description = ((TextBox)row.FindControl("txtDescription")).Text;
                                string HSN = ((TextBox)row.FindControl("txtHSN")).Text;
                                string Qty = ((TextBox)row.FindControl("txtQty")).Text;
                                string UOM = ((TextBox)row.FindControl("txtUOM")).Text;
                                string Rate = ((TextBox)row.FindControl("txtRate")).Text;
                                string Amount = ((Label)row.FindControl("txtAmount")).Text;
                                string CGST = ((TextBox)row.FindControl("txtCGST")).Text;
                                string SGST = ((TextBox)row.FindControl("txtSGST")).Text;
                                string IGST = ((TextBox)row.FindControl("txtIGST")).Text;
                                string Discount = ((TextBox)row.FindControl("txtdiscount")).Text == "" ? "0" : ((TextBox)row.FindControl("txtdiscount")).Text;
                                string GrandTotal = ((TextBox)row.FindControl("txtGrandTotal")).Text;
                                string POId = ((Label)row.FindControl("lblID")).Text;

                                SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblPurchaseBillDtls ([HeaderID],[Particulars],[Description],[HSN],[Qty],[UOM],[Rate],[Discount],[Amount],[CGSTPer],[SGSTPer],[IGSTPer],[GrandTotal],[POId])
                                VALUES(" + ViewState["UpdateRowId"].ToString() + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "','" + UOM + "'," +
                                 "'" + Rate + "','" + Discount + "','" + Amount + "','" + CGST + "'," +
                                 "'" + SGST + "','" + IGST + "','" + GrandTotal + "','" + POId + "')", con);
                                con.Open();
                                cmdParticulardata.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                    if (Session["Iscompleted"].ToString() == "false")
                    {
                        SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed=null where PONo='" + ddlAgainstNumber.SelectedItem.Text + "'", con);
                        con.Open();
                        cmdupdate.ExecuteNonQuery();
                        con.Close();
                    }
                    else
                    {
                        SqlCommand cmdupdate = new SqlCommand("update tblPurchaseOrderHdr set IsClosed=1 where PONo='" + ddlAgainstNumber.SelectedItem.Text + "'", con);
                        con.Open();
                        cmdupdate.ExecuteNonQuery();
                        con.Close();
                    }
                }
                else if (ddlBillAgainst.Text == "Verbal")
                {
                    if (dgvParticularsDetails.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvParticularsDetails.Rows)
                        {
                            string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
                            string Description = ((Label)row.FindControl("lblDescription")).Text;
                            string HSN = ((Label)row.FindControl("lblHSN")).Text;
                            string Qty = ((Label)row.FindControl("lblQty")).Text;
                            string UOM = ((Label)row.FindControl("lblUOM")).Text;
                            string Rate = ((Label)row.FindControl("lblRate")).Text;
                            string Amount = ((Label)row.FindControl("lblAmount")).Text;
                            string CGST = ((Label)row.FindControl("lblCGSTPer")).Text;
                            string SGST = ((Label)row.FindControl("lblSGSTPer")).Text;
                            string IGST = ((Label)row.FindControl("lblIGSTPer")).Text;
                            string Discount = ((Label)row.FindControl("lblDiscount")).Text == "" ? "0" : ((Label)row.FindControl("lblDiscount")).Text;
                            string GrandTotal = ((Label)row.FindControl("lblTotalAmount")).Text;
                            //string POId = ((Label)row.FindControl("lblID")).Text;

                            SqlCommand cmdParticulardata = new SqlCommand(@"INSERT INTO tblPurchaseBillDtls ([HeaderID],[Particulars],[Description],[HSN],[Qty],[UOM],[Rate],[Discount],[Amount],[CGSTPer],[SGSTPer],[IGSTPer],[GrandTotal],[POId])
                           VALUES(" + ViewState["UpdateRowId"].ToString() + ",'" + Particulars + "','" + Description + "','" + HSN + "','" + Qty + "','" + UOM + "'," +
                             "'" + Rate + "','" + Discount + "','" + Amount + "','" + CGST + "'," +
                             "'" + SGST + "','" + IGST + "','" + GrandTotal + "',NULL)", con);
                            con.Open();
                            cmdParticulardata.ExecuteNonQuery();
                            con.Close();

                            //SqlCommand cmdgetqty = new SqlCommand("select OpeningStock from tblItemMaster where ItemName='" + Particulars + "'", con);
                            //con.Open();
                            //Object openeningstock = cmdgetqty.ExecuteScalar();
                            //con.Close();

                            //string Openningstk = openeningstock.ToString() == "" ? "0" : openeningstock.ToString();

                            //var PlusQty = Convert.ToDecimal(Openningstk) + Convert.ToDecimal(Qty);

                            //SqlCommand cmdupdateqty = new SqlCommand("update tblItemMaster set OpeningStock='" + PlusQty + "' where ItemName='" + Particulars + "'", con);
                            //con.Open();
                            //cmdupdateqty.ExecuteNonQuery();
                            //con.Close();
                        }
                    }
                }
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Data Updated Sucessfully');window.location.href='PurchaseBillList.aspx';", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Please Add Particulars');", true);
            }
        }
        #endregion Update
    }

    protected void btnreset_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseBill.aspx");
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
                com.CommandText = "select Vendorname from tbl_VendorMaster where " + "Vendorname like @Search + '%'";

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

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetTransportList(string prefixText, int count)
    {
        return AutoFillTransport(prefixText);
    }

    public static List<string> AutoFillTransport(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "Select DISTINCT [TransportMode] from tblPurchaseBillHdr where " + "TransportMode like @Search + '%'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> TransportModes = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        TransportModes.Add(sdr["TransportMode"].ToString());
                    }
                }
                con.Close();
                return TransportModes;
            }
        }
    }

    protected void BindPO()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT Id,PONo FROM tblPurchaseOrderHdr where SupplierName='" + txtSupplierName.Text.Trim() + "' and IsClosed is null", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlAgainstNumber.DataSource = dt;
            ddlAgainstNumber.DataBind();
            ddlAgainstNumber.DataTextField = "PONo";
            ddlAgainstNumber.DataValueField = "Id";
            ddlAgainstNumber.DataBind();
        }
        else
        {

        }
        ddlAgainstNumber.Items.Insert(0, new System.Web.UI.WebControls.ListItem("--Select Order--", "0"));
    }

    protected void ddlBillAgainst_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBillAgainst.SelectedItem.Text == "Order")
        {
            if (txtSupplierName.Text != "")
            {
                BindPO();
                ddlAgainstNumber.Enabled = true;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please Select Supplier Name !!');", true);
            }
        }
        else if (ddlBillAgainst.SelectedItem.Text == "Verbal")
        {
            if (txtSupplierName.Text != "")
            {
                DivVerbal.Visible = true;
                divVerbaldtls.Visible = true;
                ddlAgainstNumber.Enabled = false;
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Please Select Supplier Name !!');", true);
            }
        }
    }

    protected void ddlAgainstNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            hdnGrandtotal.Value = "0";
            txtGrandTot.Text = "0";
            txtTCost.Text = "0";
            getOrderDatailsdts();
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected void getOrderDatailsdts()
    {
        string ID = ddlAgainstNumber.SelectedValue;
        SqlDataAdapter ad = new SqlDataAdapter("select * from tblPurchaseOrderDtls where HeaderID='" + ID.Trim() + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            dgvOrderDtl.DataSource = dt;
            dgvOrderDtl.DataBind();
        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('PO Details Not Found !!');", true);
        }

        SqlDataAdapter add = new SqlDataAdapter("select * from tblPurchaseOrderHdr where Id='" + ID.Trim() + "' ", con);
        DataTable dtt = new DataTable();
        add.Fill(dtt);
        if (dtt.Rows.Count > 0)
        {
            txtTCharge.Text = dtt.Rows[0]["TransportationCharges"].ToString();
            txtTCGSTPer.Text = dtt.Rows[0]["TCGSTPer"].ToString();
            txtTCGSTamt.Text = dtt.Rows[0]["TCGSTAmt"].ToString();
            txtTSGSTPer.Text = dtt.Rows[0]["TSGSTPer"].ToString();
            txtTSGSTamt.Text = dtt.Rows[0]["TSGSTAmt"].ToString();
            txtTIGSTPer.Text = dtt.Rows[0]["TIGSTPer"].ToString();
            txtTIGSTamt.Text = dtt.Rows[0]["TIGSTAmt"].ToString();
            txtTCost.Text = dtt.Rows[0]["TotalCost"].ToString();

            var tot = Convert.ToDouble(txtGrandTot.Text) + Convert.ToDouble(txtTCost.Text);
            txtGrandTot.Text = tot.ToString();

        }
        else
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('PO Details Not Found !!');", true);
        }
    }

    decimal grandtotalt = 0;
    protected void chkSelect_CheckedChanged(object sender, EventArgs e)
    {
        foreach (GridViewRow row in dgvOrderDtl.Rows)
        {
            TextBox txtqty = (TextBox)row.FindControl("txtQty");
            TextBox UOM = (TextBox)row.FindControl("txtUOM");
            TextBox rate = (TextBox)row.FindControl("txtRate");
            Label Amount = (Label)row.FindControl("txtAmount");
            TextBox cgst = (TextBox)row.FindControl("txtCGST");
            TextBox sgst = (TextBox)row.FindControl("txtSGST");
            TextBox Igst = (TextBox)row.FindControl("txtIGST");
            TextBox discount = (TextBox)row.FindControl("txtdiscount");
            TextBox grandtotal = (TextBox)row.FindControl("txtGrandTotal");
            CheckBox chk = (CheckBox)row.FindControl("chkSelect");
            //CheckBox chkheader = (CheckBox)dgvOrderDtl.HeaderRow.FindControl("chkHeader");
            if (chk != null & chk.Checked)
            {
                Double qty1 = Convert.ToDouble(txtqty.Text);

                if (qty1 == 0)
                {
                    txtqty.Enabled = false;
                    UOM.Enabled = false;
                    rate.Enabled = false;
                    Amount.Enabled = false;
                    cgst.Enabled = false;
                    sgst.Enabled = false;
                    Igst.Enabled = false;
                    discount.Enabled = false;
                    chk.Enabled = false;
                }
                else
                {
                    txtqty.Enabled = true;
                    UOM.Enabled = false;
                    rate.Enabled = false;
                    Amount.Enabled = false;
                    cgst.Enabled = false;
                    sgst.Enabled = false;
                    Igst.Enabled = false;
                    discount.Enabled = false;
                    chk.Enabled = true;
                    //Double price1 = Convert.ToDouble(rate.Text);
                    //total = (qty1 * price1);
                    //Amount.Text = total.ToString();


                    //Totalamt += Convert.ToDecimal((e.Row.FindControl("txtAmount") as Label).Text);
                    //GrandTotalamt += Convert.ToDecimal((e.Row.FindControl("txtGrandTotal") as TextBox).Text);
                    //hdnGrandtotal.Value = GrandTotalamt.ToString();
                    //sumofAmount.Text = Totalamt.ToString();

                    //var Total = Convert.ToDecimal(txtCost.Text) + GrandTotalamt + Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(txtTCost.Text);
                    //txtGrandTot.Text = Total.ToString("##.00");


                    Totalamt += Convert.ToDecimal(Amount.Text);
                    sumofAmount.Text = Totalamt.ToString();
                    grandtotalt += Convert.ToDecimal(grandtotal.Text);
                    //FinalGrandtotalTcs += grandtotalt;
                    ////txtGrandTot.Text = FinalGrandtotalTcs.ToString();
                    hdnGrandtotal.Value = grandtotalt.ToString();
                    var Total = Convert.ToDecimal(txtCost.Text) + grandtotalt + Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(txtTCost.Text);
                    txtGrandTot.Text = Total.ToString("##.00");
                }
            }
            else
            {
                txtqty.Enabled = false;
                UOM.Enabled = false;
                rate.Enabled = false;
                Amount.Enabled = false;
                cgst.Enabled = false;
                sgst.Enabled = false;
                Igst.Enabled = false;
                discount.Enabled = false;
            }
        }
    }

    protected void txtQty_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);
        decimal Amount = 0;
        decimal GrandTotal = 0;
        foreach (GridViewRow rows in dgvOrderDtl.Rows)
        {
            bool chk = (rows.FindControl("chkSelect") as CheckBox).Checked;
            if (chk == true)
            {
                Amount += Convert.ToDecimal(((Label)rows.FindControl("txtAmount")).Text);
                GrandTotal += Convert.ToDecimal(((TextBox)rows.FindControl("txtGrandTotal")).Text);
            }
        }
        sumofAmount.Text = Amount.ToString();
        txtGrandTot.Text = GrandTotal.ToString();
        Session["Iscompleted"] = "false";
    }

    private void calculationA(GridViewRow row)
    {
        TextBox txt_Qty = (TextBox)row.FindControl("txtQty");
        TextBox txt_price = (TextBox)row.FindControl("txtRate");
        TextBox txt_CGST = (TextBox)row.FindControl("txtCGST");
        TextBox txt_SGST = (TextBox)row.FindControl("txtSGST");
        TextBox txt_IGST = (TextBox)row.FindControl("txtIGST");
        Label txtBasicAmount = (Label)row.FindControl("txtAmount");
        TextBox txt_amount = (TextBox)row.FindControl("txtGrandTotal");
        TextBox txt_discount = (TextBox)row.FindControl("txtdiscount");

        var totalamt = Convert.ToDecimal(txt_Qty.Text.Trim()) * Convert.ToDecimal(txt_price.Text.Trim());

        decimal AmtWithDiscount;
        if (txt_discount.Text != "")
        {
            var disc = totalamt * (Convert.ToDecimal(txt_discount.Text.Trim())) / 100;
            AmtWithDiscount = totalamt - disc;
        }
        else
        {
            AmtWithDiscount = totalamt + 0;
        }
        txtBasicAmount.Text = AmtWithDiscount.ToString();

        var CGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_CGST.Text.Trim())) / 100;
        var SGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_SGST.Text.Trim())) / 100;
        var IGSTamt = AmtWithDiscount * (Convert.ToDecimal(txt_IGST.Text.Trim())) / 100;
        decimal GSTtotal = 0;
        if (IGSTamt == 0)
        {
            GSTtotal = SGSTamt + CGSTamt;
        }
        else
        {
            GSTtotal = IGSTamt;
        }

        var NetAmt = AmtWithDiscount + GSTtotal;

        txt_amount.Text = Math.Round(NetAmt).ToString();
    }

    protected void txtCGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);

        decimal Amount = 0;
        decimal GrandTotal = 0;
        foreach (GridViewRow rows in dgvOrderDtl.Rows)
        {
            Amount += Convert.ToDecimal(((Label)rows.FindControl("txtAmount")).Text);
            GrandTotal += Convert.ToDecimal(((TextBox)rows.FindControl("txtGrandTotal")).Text);
        }
        sumofAmount.Text = Amount.ToString();
        txtGrandTot.Text = GrandTotal.ToString();
    }

    protected void txtSGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);

        decimal Amount = 0;
        decimal GrandTotal = 0;
        foreach (GridViewRow rows in dgvOrderDtl.Rows)
        {
            Amount += Convert.ToDecimal(((Label)rows.FindControl("txtAmount")).Text);
            GrandTotal += Convert.ToDecimal(((TextBox)rows.FindControl("txtGrandTotal")).Text);
        }
        sumofAmount.Text = Amount.ToString();
        txtGrandTot.Text = GrandTotal.ToString();
    }

    protected void txtIGST_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);

        decimal Amount = 0;
        decimal GrandTotal = 0;
        foreach (GridViewRow rows in dgvOrderDtl.Rows)
        {
            Amount += Convert.ToDecimal(((Label)rows.FindControl("txtAmount")).Text);
            GrandTotal += Convert.ToDecimal(((TextBox)rows.FindControl("txtGrandTotal")).Text);
        }
        sumofAmount.Text = Amount.ToString();
        txtGrandTot.Text = GrandTotal.ToString();
    }

    protected void txtdiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        calculationA(row);

        decimal Amount = 0;
        decimal GrandTotal = 0;
        foreach (GridViewRow rows in dgvOrderDtl.Rows)
        {
            Amount += Convert.ToDecimal(((Label)rows.FindControl("txtAmount")).Text);
            GrandTotal += Convert.ToDecimal(((TextBox)rows.FindControl("txtGrandTotal")).Text);
        }
        sumofAmount.Text = Amount.ToString();
        txtGrandTot.Text = GrandTotal.ToString();
    }

    decimal Totalamt = 0;
    decimal GrandTotalamt = 0;
    protected void dgvOrderDtl_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txt_Qty = (TextBox)e.Row.FindControl("txtQty");
            Label id = (Label)e.Row.FindControl("lblID");
            int Id = Convert.ToInt32(id.Text);

            TextBox txt_price = (TextBox)e.Row.FindControl("txtRate");
            TextBox txt_CGST = (TextBox)e.Row.FindControl("txtCGST");
            TextBox txt_SGST = (TextBox)e.Row.FindControl("txtSGST");
            TextBox txt_IGST = (TextBox)e.Row.FindControl("txtIGST");
            Label txtBasicAmount = (Label)e.Row.FindControl("txtAmount");
            TextBox txt_amount = (TextBox)e.Row.FindControl("txtGrandTotal");
            TextBox txt_discount = (TextBox)e.Row.FindControl("txtdiscount");

            if (btnadd.Text == "Submit")
            {



                SqlCommand cmdmax = new SqlCommand("SELECT Qty FROM tblPurchaseBillDtls where POId='" + id.Text + "'", con);
                con.Open();
                Object billQtyid = cmdmax.ExecuteScalar();
                con.Close();

                SqlCommand cmdsumQty = new SqlCommand("SELECT SUM(CAST(Qty as float)) FROM tblPurchaseOrderDtls where Id='" + Id + "'", con);
                con.Open();
                string smQty = cmdsumQty.ExecuteScalar().ToString();
                //sumofqty = Convert.ToInt32(smQty);
                con.Close();
                var PObillqty = billQtyid == null ? "0" : billQtyid.ToString();
                var smquantity = smQty == "" ? "0" : smQty.ToString();
                var minusQty = Convert.ToDecimal(smquantity) - Convert.ToDecimal(PObillqty);
                txt_Qty.Text = minusQty.ToString();
            }

            //Calculation
            var totalamt = Convert.ToDouble(txt_Qty.Text.Trim()) * Convert.ToDouble(txt_price.Text.Trim());
            //txtBasicAmount.Text = totalamt.ToString();

            double AmtWithDiscount;
            if (txt_discount.Text != "")
            {
                var disc = totalamt * (Convert.ToDouble(txt_discount.Text.Trim())) / 100;
                AmtWithDiscount = totalamt - disc;
            }
            else
            {
                AmtWithDiscount = totalamt + 0;
            }
            txtBasicAmount.Text = AmtWithDiscount.ToString();

            var CGSTamt = AmtWithDiscount * (Convert.ToDouble(txt_CGST.Text.Trim())) / 100;
            var SGSTamt = AmtWithDiscount * (Convert.ToDouble(txt_SGST.Text.Trim())) / 100;
            var IGSTamt = AmtWithDiscount * (Convert.ToDouble(txt_IGST.Text.Trim())) / 100;
            double GSTtotal = 0;
            if (IGSTamt == 0)
            {
                GSTtotal = SGSTamt + CGSTamt;
            }
            else
            {
                GSTtotal = IGSTamt;
            }

            var NetAmt = AmtWithDiscount + GSTtotal;

            txt_amount.Text = Math.Round(NetAmt).ToString();

            if (txt_Qty.Text == "0")
            {
                e.Row.Visible = false;
            }
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            Totalamt += Convert.ToDecimal((e.Row.FindControl("txtAmount") as Label).Text);
            GrandTotalamt += Convert.ToDecimal((e.Row.FindControl("txtGrandTotal") as TextBox).Text);
            //hdnGrandtotal.Value = GrandTotalamt.ToString();
            //sumofAmount.Text = Totalamt.ToString();

            var Total = Convert.ToDecimal(txtCost.Text) + GrandTotalamt + Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(txtTCost.Text);
            // txtGrandTot.Text = Total.ToString("##.00");
        }
    }

    protected void txtRate_TextChanged(object sender, EventArgs e)
    {
        string Amt = sumofAmount.Text;
        string Rate = txtRate.Text;
        if (Rate == "0")
        {
            txtBasic.Text = "0";
            txtCost.Text = "0";
            CGSTPer.Text = "0";
            SGSTPer.Text = "0";
            IGSTPer.Text = "0";
        }
        else
        {
            var Basic = Convert.ToDecimal(Amt) * Convert.ToDecimal(Rate) / 100;
            txtBasic.Text = Basic.ToString("##.00");

            var grandtot = Convert.ToDecimal(Basic) + Convert.ToDecimal(hdnGrandtotal.Value) + Convert.ToDecimal(txtTCost.Text);
            txtGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void CGSTPer_TextChanged(object sender, EventArgs e)
    {
        GstCalculation();
        var grandtot = Convert.ToDouble(txtCost.Text) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCost.Text);
        txtGrandTot.Text = grandtot.ToString("##.00");
    }

    protected void SGSTPer_TextChanged(object sender, EventArgs e)
    {
        GstCalculation();
        var grandtot = Convert.ToDouble(txtCost.Text) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCost.Text);
        txtGrandTot.Text = grandtot.ToString("##.00");
    }

    protected void GstCalculation()
    {
        string Basic = txtBasic.Text;
        string CGST = CGSTPer.Text;
        string SGST = SGSTPer.Text;
        if (CGST == "0" || SGST == "0")
        {
            if (CGST == "0" && SGST == "0" && IGSTPer.Text == "0")
            {
                IGSTPer.Enabled = true;
                CGSTPer.Enabled = true;
                SGSTPer.Enabled = true;
                txtCost.Text = Basic.ToString();
            }
            else
            {
                if (IGSTPer.Text == "0")
                {
                    IGSTPer.Enabled = false;
                    CGSTPer.Enabled = true;
                    SGSTPer.Enabled = true;
                    var CGSTAmt = Convert.ToDecimal(Basic) * Convert.ToDecimal(CGST) / 100;
                    var SGSTAmt = Convert.ToDecimal(Basic) * Convert.ToDecimal(SGST) / 100;
                    var GSTTaxTotal = Convert.ToDecimal(Basic) + CGSTAmt + SGSTAmt;
                    txtCost.Text = GSTTaxTotal.ToString("##.00");

                    var grandtot = Convert.ToDecimal(GSTTaxTotal) + Convert.ToDecimal(hdnGrandtotal.Value);
                    txtGrandTot.Text = grandtot.ToString("##.00");
                }
                else
                {
                    IGSTPer.Enabled = true;
                    CGSTPer.Enabled = false;
                    SGSTPer.Enabled = false;
                    var IGSTAmt = Convert.ToDecimal(Basic) * Convert.ToDecimal(IGSTPer.Text) / 100;
                    var GSTTaxTotal = Convert.ToDecimal(Basic) + IGSTAmt;
                    txtCost.Text = GSTTaxTotal.ToString("##.00");

                    var grandtot = Convert.ToDecimal(GSTTaxTotal) + Convert.ToDecimal(hdnGrandtotal.Value);
                    txtGrandTot.Text = grandtot.ToString("##.00");
                }
            }
        }
        else
        {
            IGSTPer.Enabled = false;
            CGSTPer.Enabled = true;
            SGSTPer.Enabled = true;
            var CGSTAmt = Convert.ToDecimal(Basic) * Convert.ToDecimal(CGST) / 100;
            var SGSTAmt = Convert.ToDecimal(Basic) * Convert.ToDecimal(SGST) / 100;

            var GSTTaxTotal = Convert.ToDecimal(Basic) + CGSTAmt + SGSTAmt;
            txtCost.Text = GSTTaxTotal.ToString("##.00");

            var grandtot = Convert.ToDecimal(GSTTaxTotal) + Convert.ToDecimal(hdnGrandtotal.Value);
            txtGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void IGSTPer_TextChanged(object sender, EventArgs e)
    {
        GstCalculation();
    }

    //protected void txtTCSPer_TextChanged(object sender, EventArgs e)
    //{
    //    if (txtTCSPer.Text == "0" || txtTCSPer.Text == "")
    //    {
    //        var tot = Convert.ToDecimal(sumofAmount.Text) + Convert.ToDecimal(txtCost.Text);
    //        var TcsAmt = Convert.ToDecimal(txtTCSPer.Text) * tot / 100;
    //        txtTCSAmt.Text = TcsAmt.ToString("##.00");

    //        var grandtot = Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(hdnGrandtotal.Value) + Convert.ToDecimal(txtCost.Text);
    //        txtGrandTot.Text = grandtot.ToString("##.00");
    //        txtTCSAmt.Text = "0";
    //    }
    //    else
    //    {
    //        var tot = Convert.ToDecimal(sumofAmount.Text) + Convert.ToDecimal(txtCost.Text);
    //        var TcsAmt = Convert.ToDecimal(txtTCSPer.Text) * tot / 100;
    //        txtTCSAmt.Text = TcsAmt.ToString("##.00");

    //        var grandtot = Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(hdnGrandtotal.Value) + Convert.ToDecimal(txtCost.Text);
    //        txtGrandTot.Text = grandtot.ToString("##.00");
    //    }
    //}

    protected void txtTCSPer_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (txtTCSPer.Text == "0" || txtTCSPer.Text == "")
        {
            var tot = Convert.ToDecimal(txtGrandTot.Text) + Convert.ToDecimal(txtCost.Text);
            var TcsAmt = Convert.ToDecimal(txtTCSPer.Text) * tot / 100;
            txtTCSAmt.Text = TcsAmt.ToString("##.00");

            var grandtot = Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(txtGrandTot.Text) + Convert.ToDecimal(txtCost.Text) + Convert.ToDecimal(txtTCost.Text);
            txtGrandTot.Text = grandtot.ToString("##.00");
            txtTCSAmt.Text = "0";
        }
        else
        {
            var tot = Convert.ToDecimal(txtGrandTot.Text) + Convert.ToDecimal(txtCost.Text); //hdnGrandtotal.Value
            var TcsAmt = Convert.ToDecimal(txtTCSPer.Text) * tot / 100;
            txtTCSAmt.Text = TcsAmt.ToString("##.00");

            var grandtot = Convert.ToDecimal(txtTCSAmt.Text) + Convert.ToDecimal(txtGrandTot.Text) + Convert.ToDecimal(txtCost.Text) + Convert.ToDecimal(txtTCost.Text);
            txtGrandTot.Text = grandtot.ToString("##.00");
        }
    }

    protected void txtBasic_TextChanged(object sender, EventArgs e)
    {
        string Amt = sumofAmount.Text;
        string Basic = txtBasic.Text;
        if (Basic == "0")
        {
            txtBasic.Text = "0";
            txtCost.Text = "0";
            CGSTPer.Text = "0";
            SGSTPer.Text = "0";
            IGSTPer.Text = "0";
        }
        else
        {
            var Per = Convert.ToDouble(Basic) / Convert.ToDouble(Amt) * 100;
            txtRate.Text = Per.ToString("##.00");

            var grandtot = Convert.ToDouble(Basic) + Convert.ToDouble(hdnGrandtotal.Value) + Convert.ToDouble(txtTCost.Text);
            txtGrandTot.Text = grandtot.ToString("##.00");
        }
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

        var TotalGrand = Convert.ToDouble(txtGrandTot.Text) + Convert.ToDouble(txtTCost.Text);
        txtGrandTot.Text = TotalGrand.ToString("0.00", CultureInfo.InvariantCulture);
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

        var TotalGrand = Convert.ToDouble(txtGrandTot.Text) + Convert.ToDouble(txtTCost.Text);
        hdnGrandtotal.Value = TotalGrand.ToString("0.00", CultureInfo.InvariantCulture);
        txtGrandTot.Text = hdnGrandtotal.Value;
    }

    protected void txtDOR_TextChanged(object sender, EventArgs e)
    {
        DateTime fromdate = Convert.ToDateTime(txtBilldate.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        DateTime todate = Convert.ToDateTime(txtDOR.Text, System.Globalization.CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
        if (fromdate > todate)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Bill Date is greater than Received Date...Please Choose Correct Date.');", true);
            btnadd.Enabled = false;
        }
        else
        {
            btnadd.Enabled = true;
        }
    }

    protected void Insert(object sender, EventArgs e)
    {
        if (txtVQty.Text == "" || txtParticulars.Text == "" || txtVTotalamt.Text == "")
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
        DataTable dt = (DataTable)ViewState["VParticularDetails"];

        dt.Rows.Add(ViewState["RowNo"], txtParticulars.Text, txtVHSN.Text, txtVQty.Text, txtVRate.Text, txtVDisc.Text, txtVAmount.Text, txtVCGSTPer.Text, txtVCGSTAmt.Text, txtVSGSTPer.Text, txtVSGSTAmt.Text, txtVIGSTPer.Text, txtVIGSTAmt.Text, txtVTotalamt.Text, txtVDescription.Text, txtUOM.Text);
        ViewState["VParticularDetails"] = dt;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["VParticularDetails"];
        dgvParticularsDetails.DataBind();

        txtParticulars.Text = string.Empty;
        txtVQty.Text = string.Empty;
        txtVHSN.Text = string.Empty;
        txtVRate.Text = string.Empty;
        txtVDisc.Text = "0";
        txtVAmount.Text = string.Empty;
        txtVCGSTPer.Text = string.Empty;
        txtVCGSTAmt.Text = string.Empty;
        txtVSGSTPer.Text = string.Empty;
        txtVSGSTAmt.Text = string.Empty;
        txtVIGSTPer.Text = string.Empty;
        txtVIGSTAmt.Text = string.Empty;
        txtVTotalamt.Text = string.Empty;
        txtVDescription.Text = string.Empty;
        //txtUOM.SelectedItem.Text = string.Empty;


        decimal Amount = 0;
        decimal GrandTotal = 0;
        foreach (GridViewRow rows in dgvParticularsDetails.Rows)
        {
            Amount += Convert.ToDecimal(((Label)rows.FindControl("lblAmount")).Text);
            GrandTotal += Convert.ToDecimal(((Label)rows.FindControl("lblTotalAmount")).Text);
        }
        sumofAmount.Text = Amount.ToString();
        txtGrandTot.Text = (GrandTotal + Convert.ToDecimal(txtCost.Text) + Convert.ToDecimal(txtTCost.Text)).ToString();
    }

    protected void dgvParticularsDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void dgvParticularsDetails_RowEditing(object sender, GridViewEditEventArgs e)
    {
        dgvParticularsDetails.EditIndex = e.NewEditIndex;
        dgvParticularsDetails.DataSource = (DataTable)ViewState["VParticularDetails"];
        dgvParticularsDetails.DataBind();

        TextBox txtIgst = (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[12].Controls[1] as TextBox);
        var Igst = txtIgst.Text;
        if (Igst == "0")
        {
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[12].Controls[1] as TextBox).Enabled = false;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[13].Controls[1] as TextBox).Enabled = false;

            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[8].Controls[1] as TextBox).Enabled = true;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[9].Controls[1] as TextBox).Enabled = true;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[10].Controls[1] as TextBox).Enabled = true;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[11].Controls[1] as TextBox).Enabled = true;
        }
        else
        {
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[12].Controls[1] as TextBox).Enabled = true;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[13].Controls[1] as TextBox).Enabled = true;

            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[8].Controls[1] as TextBox).Enabled = false;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[9].Controls[1] as TextBox).Enabled = false;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[10].Controls[1] as TextBox).Enabled = false;
            (dgvParticularsDetails.Rows[e.NewEditIndex].Cells[11].Controls[1] as TextBox).Enabled = false;
        }
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkbtnUpdate_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((Label)row.FindControl("lblRate")).Text;

        string Discount = ((TextBox)row.FindControl("txtPerDiscount")).Text;

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

        DataTable Dt = ViewState["VParticularDetails"] as DataTable;

        Dt.Rows[row.RowIndex]["Particulars"] = Particulars;
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

        ViewState["VParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["VParticularDetails"];
        dgvParticularsDetails.DataBind();

        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);
    }

    protected void lnkDelete_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable dt = ViewState["VParticularDetails"] as DataTable;
        dt.Rows.Remove(dt.Rows[row.RowIndex]);
        ViewState["VParticularDetails"] = dt;
        dgvParticularsDetails.DataSource = (DataTable)ViewState["VParticularDetails"];
        dgvParticularsDetails.DataBind();
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Alert", "alert('Data Delete Succesfully !!!');", true);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "scrollToElement();", true);

    }

    protected void lnkCancel_Click(object sender, EventArgs e)
    {
        GridViewRow row = (sender as LinkButton).NamingContainer as GridViewRow;

        DataTable Dt = ViewState["VParticularDetails"] as DataTable;
        dgvParticularsDetails.EditIndex = -1;

        ViewState["VParticularDetails"] = Dt;
        dgvParticularsDetails.EditIndex = -1;

        dgvParticularsDetails.DataSource = (DataTable)ViewState["VParticularDetails"];
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
                com.CommandText = "select DISTINCT ProductName from tbl_ProductMaster where " + "ProductName like '%'+ @Search + '%' AND IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                //com.Parameters.AddWithValue("@SName", sName);
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
                com.CommandText = "Select DISTINCT Unit from tbl_ProductMaster where " + "Unit like '%' + @Search + '%' AND IsDeleted=0";

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

    protected void txtParticulars_TextChanged(object sender, EventArgs e)
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM tbl_ProductMaster where ProductName='" + txtParticulars.Text.Trim() + "' ", con);
        DataTable dt = new DataTable();
        ad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtVHSN.Text = dt.Rows[0]["HSN"].ToString() == "" ? "0" : dt.Rows[0]["HSN"].ToString();
            txtVRate.Text = dt.Rows[0]["Price"].ToString() == "" ? "0" : dt.Rows[0]["Price"].ToString();
           // txtVCGSTPer.Text = dt.Rows[0]["CGST"].ToString() == "" ? "0" : dt.Rows[0]["CGST"].ToString();
          //  txtVSGSTPer.Text = dt.Rows[0]["SGST"].ToString() == "" ? "0" : dt.Rows[0]["SGST"].ToString();
           // txtVIGSTPer.Text = dt.Rows[0]["IGST"].ToString() == "" ? "0" : dt.Rows[0]["IGST"].ToString();
          //  txtUOM.SelectedValue = dt.Rows[0]["Unit"].ToString() == "" ? "0" : dt.Rows[0]["Unit"].ToString();
            //txtVDescription.Text = dt.Rows[0]["Description"].ToString() == "" ? "0" : dt.Rows[0]["Description"].ToString();
        }
        else
        {

        }
    }

    private void GST_Calculation()
    {
        var TotalAmt = Convert.ToDecimal(txtVQty.Text.Trim()) * Convert.ToDecimal(txtVRate.Text.Trim());

        decimal disc;
        if (string.IsNullOrEmpty(txtVDisc.Text))
        {
            disc = 0;
            txtVAmount.Text = TotalAmt.ToString("0.00", CultureInfo.InvariantCulture);
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(TotalAmt);
            decimal Val2 = Convert.ToDecimal(txtVDisc.Text);
            disc = (Val1 * Val2 / 100);
            var result = Val1 - disc;
            txtVAmount.Text = result.ToString("0.00", CultureInfo.InvariantCulture);
        }

        decimal CGST;
        if (string.IsNullOrEmpty(txtVCGSTPer.Text))
        {
            CGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtVAmount.Text);
            decimal Val2 = Convert.ToDecimal(txtVCGSTPer.Text);
            txtVSGSTPer.Text = txtVCGSTPer.Text;

            CGST = (Val1 * Val2 / 100);

        }
        txtVCGSTAmt.Text = CGST.ToString("0.00", CultureInfo.InvariantCulture);

        decimal SGST;
        if (string.IsNullOrEmpty(txtVSGSTPer.Text))
        {
            SGST = 0;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtVAmount.Text);
            decimal Val2 = Convert.ToDecimal(txtVSGSTPer.Text);

            SGST = (Val1 * Val2 / 100);
        }
        txtVSGSTAmt.Text = SGST.ToString("0.00", CultureInfo.InvariantCulture);


        decimal IGST;
        if (txtVIGSTPer.Text == "0")
        {
            IGST = 0;
            txtVIGSTPer.Enabled = false;
            txtVIGSTAmt.Enabled = false;

            txtVCGSTPer.Enabled = true;
            txtVCGSTAmt.Enabled = true;

            txtVSGSTPer.Enabled = true;
            txtVSGSTAmt.Enabled = true;
        }
        else
        {
            decimal Val1 = Convert.ToDecimal(txtVAmount.Text);
            decimal Val2 = Convert.ToDecimal(txtVIGSTPer.Text);

            IGST = (Val1 * Val2 / 100);

            txtVIGSTPer.Enabled = true;
            txtVIGSTAmt.Enabled = true;

            txtVCGSTPer.Enabled = false;
            txtVCGSTAmt.Enabled = false;

            txtVSGSTPer.Enabled = false;
            txtVSGSTAmt.Enabled = false;


        }
        txtVIGSTAmt.Text = IGST.ToString("0.00", CultureInfo.InvariantCulture);

        var GSTTotal = CGST + SGST + IGST;

        var Finalresult = Convert.ToDecimal(txtVAmount.Text) + GSTTotal;

        txtVTotalamt.Text = Finalresult.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private void GRID_GST_Calculation(GridViewRow row)
    {
        string Particulars = ((Label)row.FindControl("lblParticulars")).Text;
        string HSN = ((Label)row.FindControl("lblHSN")).Text;
        string Qty = ((TextBox)row.FindControl("txtQty")).Text;
        string Rate = ((Label)row.FindControl("lblRate")).Text;
        TextBox Discount = ((TextBox)row.FindControl("txtPerDiscount"));
        Label Amount = ((Label)row.FindControl("lblAmount"));
        string CGSTPer = ((TextBox)row.FindControl("txtCGSTPer")).Text;
        TextBox CGSTAmt = (TextBox)row.FindControl("txtCGSTAmt");
        string SGSTPer = ((TextBox)row.FindControl("txtSGSTPer")).Text;
        TextBox SGSTAmt = (TextBox)row.FindControl("txtSGSTAmt");
        string IGSTPer = ((TextBox)row.FindControl("txtIGSTPer")).Text;
        TextBox IGSTAmt = (TextBox)row.FindControl("txtIGSTAmt");
        TextBox TotalAmount = (TextBox)row.FindControl("txtTotalAmount");

        var totalamt = Convert.ToDecimal(Qty) * Convert.ToDecimal(Rate);
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

    protected void txtPerDiscount_TextChanged(object sender, EventArgs e)
    {
        GridViewRow row = (sender as TextBox).NamingContainer as GridViewRow;
        GRID_GST_Calculation(row);
    }

    protected void txtVDisc_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtVQty_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtVRate_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtVCGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtVSGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    protected void txtVIGSTPer_TextChanged(object sender, EventArgs e)
    {
        GST_Calculation();
    }

    decimal VTotalamt = 0;
    protected void dgvParticularsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (btnadd.Text == "Update")
            {

                if (ddlBillAgainst.Text == "Verbal")
                {
                    Label txtAmount = (Label)e.Row.FindControl("lblAmount");
                    Label lblCGSTPer = (e.Row.FindControl("lblCGSTPer") as Label);
                    Label lblSGSTPer = (e.Row.FindControl("lblSGSTPer") as Label);
                    Label lblIGSTPer = (e.Row.FindControl("lblIGSTPer") as Label);

                    Label lblCGSTAmt = (e.Row.FindControl("lblCGSTAmt") as Label);
                    Label lblSGSTAmt = (e.Row.FindControl("lblSGSTAmt") as Label);
                    Label lblIGSTAmt = (e.Row.FindControl("lblIGSTAmt") as Label);

                    var cgstamt = Convert.ToDecimal(txtAmount.Text) * Convert.ToDecimal(lblCGSTPer.Text == "" ? "0" : lblCGSTPer.Text) / 100;
                    var sgstamt = Convert.ToDecimal(txtAmount.Text) * Convert.ToDecimal(lblSGSTPer.Text == "" ? "0" : lblSGSTPer.Text) / 100;
                    var igstamt = Convert.ToDecimal(txtAmount.Text) * Convert.ToDecimal(lblIGSTPer.Text == "" ? "0" : lblIGSTPer.Text) / 100;

                    lblCGSTAmt.Text = cgstamt.ToString("#0.00");
                    lblSGSTAmt.Text = sgstamt.ToString("#0.00");
                    lblIGSTAmt.Text = igstamt.ToString("#0.00");
                }
            }

            TextBox txts = (e.Row.FindControl("txtTotalAmount") as TextBox);

            if (txts == null)
            {
                Totalamt += Convert.ToDecimal((e.Row.FindControl("lblTotalAmount") as Label).Text);
                VTotalamt += Convert.ToDecimal((e.Row.FindControl("lblAmount") as Label).Text);
                hdnGrandtotal.Value = Totalamt.ToString();
                sumofAmount.Text = VTotalamt.ToString();
                txtGrandTot.Text = (Totalamt + Convert.ToDecimal(txtCost.Text) + Convert.ToDecimal(txtTCost.Text)).ToString();
            }
            else
            {
                Totalamt += Convert.ToDecimal((e.Row.FindControl("txtTotalAmount") as TextBox).Text);
                VTotalamt += Convert.ToDecimal((e.Row.FindControl("lblAmount") as Label).Text);
                hdnGrandtotal.Value = Totalamt.ToString();
                sumofAmount.Text = VTotalamt.ToString();
                txtGrandTot.Text = (Totalamt + Convert.ToDecimal(txtCost.Text) + Convert.ToDecimal(txtTCost.Text)).ToString();
            }
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            (e.Row.FindControl("lbltotal") as Label).Text = Totalamt.ToString();
        }
    }

    //protected void txtSupplierName_TextChanged(object sender, EventArgs e)
    //{
    //    BindEmailID();
    //}

    //protected void BindEmailID()
    //{
    //    SqlDataAdapter ad = new SqlDataAdapter("SELECT EmailID FROM tblSupplierMaster WHERE SupplierName='" + txtSupplierName.Text.Trim() + "'", con);
    //    DataTable dt = new DataTable();
    //    ad.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        lblEmailID.Text = dt.Rows[0]["EmailID"].ToString() == "" ? "Email Id Not Found" : dt.Rows[0]["EmailID"].ToString();
    //    }
    //    else
    //    {
    //        lblEmailID.Text = "Email Id is not found";
    //    }
    //}

    protected void txtSupplierName_TextChanged(object sender, EventArgs e)
    {
        BindEmail();
    }

    protected void BindEmail()
    {
        SqlDataAdapter ad = new SqlDataAdapter("SELECT EmailID from tbl_VendorMaster where Vendorname='" + txtSupplierName.Text.Trim() + "' ", con);
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


    //Send Mail
    /////pdf function
    protected void Send_Mail(int? Id, string Subject)
    {

        string strMessage = "Hello " + txtSupplierName.Text.Trim() + "<br/>" +


                        "Greetings From " + "<strong>Pune Abrasive Pvt. Ltd.<strong>" + "<br/>" +
                        "We sent you an Purchase Bill Invoice." + txtBillNo.Text.Trim() + "/" + txtBilldate.Text.Trim() + ".pdf" + "<br/>" +

                         "We Look Foward to Conducting Future Business with you." + "<br/>" +

                        "Kind Regards," + "<br/>" +
                        "<strong>Pune Abrasive Pvt. Ltd.<strong>";
        string pdfname = "Purchase Bill - " + txtBillNo.Text.Trim() + "/" + txtBilldate.Text.Trim() + ".pdf";

        MailMessage mm = new MailMessage();
        // mm.From = new MailAddress(fromMailID);
        string fromMailID = Session["EmailID"].ToString().Trim().ToLower();
        mm.Subject = "Purchase Bill Invoice";
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
        MemoryStream file = new MemoryStream(PDF(Id).ToArray());

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
        smtp.Send(mm);
    }

    public MemoryStream PDF(int? Id)
    {
        MemoryStream pdf = new MemoryStream();
        DataTable Dt = new DataTable();

        SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PurchaseBill where Id = '" + Id + "'", con);
        //SqlDataAdapter Da = new SqlDataAdapter("select * from vw_PurchaseBill where Id = '" + Session["PDFID"].ToString() + "'", con);

        Da.Fill(Dt);

        StringWriter sw = new StringWriter();
        StringReader sr = new StringReader(sw.ToString());

        Document doc = new Document(PageSize.A4, 30f, 10f, -11f, 0f);

        string DocName = (Dt.Rows[0]["SupplierName"].ToString() + "-" + Dt.Rows[0]["BillNo"].ToString() + "_PBill.pdf").Replace("/", "_");
        PdfWriter pdfWriter = PdfWriter.GetInstance(doc, pdf);

        PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/files/") + "PurchaseOrder.pdf", FileMode.Create));
        XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);

        doc.Open();

        //Price Format
        System.Globalization.CultureInfo info = System.Globalization.CultureInfo.GetCultureInfo("en-IN");

        string imageURL = Server.MapPath("~") + "/img/Whitelogo.png";
        iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);

        //Resize image depend upon your need

        png.ScaleToFit(70, 100);

        //For Image Position
        png.SetAbsolutePosition(40, 755);
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
        cb.Rectangle(28f, 720f, 560f, 60f);
        cb.Stroke();
        // Header 
        cb.BeginText();
        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "girish.kulkarni@puneabrasives.com", 250, 745, 0);
        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk,Near Shell Petrol Pump, Pune-411019", 145, 728, 0);
        cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);

        cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 227, 740, 0);
        cb.EndText();

        if (Dt.Rows.Count > 0)
        {
            var CreateDate = DateTime.Now.ToString("yyyy-MM-dd");
            string SupplierBillNo = Dt.Rows[0]["SupplierBillNo"].ToString();
            string BillDate = Dt.Rows[0]["BillDate"].ToString().Replace("12:00AM", "");
            string AgainstNumber = Dt.Rows[0]["AgainstNumber"].ToString();
            string SupplierName = Dt.Rows[0]["SupplierName"].ToString();
            string TransportMode = Dt.Rows[0]["TransportMode"].ToString();
            string TransportDescription = Dt.Rows[0]["TransportDescription"].ToString();
            string VehicleNo = Dt.Rows[0]["VehicleNo"].ToString();
            string EBillNumber = Dt.Rows[0]["EBillNumber"].ToString();
            string chargeDescription = Dt.Rows[0]["ChargesDescription"].ToString() == "" ? "" : Dt.Rows[0]["ChargesDescription"].ToString();
            string HSNSAC = Dt.Rows[0]["HSNSAC"].ToString() == "" ? "" : Dt.Rows[0]["HSNSAC"].ToString();
            string Rate = Dt.Rows[0]["Rate"].ToString() == "" ? "0" : Dt.Rows[0]["Rate"].ToString();
            string Basic = Dt.Rows[0]["Basic"].ToString() == "" ? "0" : Dt.Rows[0]["Basic"].ToString();
            string CGSTPerr = Dt.Rows[0]["CGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["CGSTPer"].ToString();
            string SGSTPerr = Dt.Rows[0]["SGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["SGSTPer"].ToString();
            string IGSTPerr = Dt.Rows[0]["IGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["IGSTPer"].ToString();
            string Cost = Dt.Rows[0]["Cost"].ToString() == "" ? "0" : Dt.Rows[0]["Cost"].ToString();

            string TransportationCharges = Dt.Rows[0]["TransportationCharges"].ToString() == "" ? "0" : Dt.Rows[0]["TransportationCharges"].ToString();
            string TCGSTPer = Dt.Rows[0]["TCGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TCGSTPer"].ToString();
            string TCGSTAmt = Dt.Rows[0]["TCGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TCGSTAmt"].ToString();
            string TSGSTPer = Dt.Rows[0]["TSGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTPer"].ToString();
            string TSGSTAmt = Dt.Rows[0]["TSGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTAmt"].ToString();
            //string TIGSTAmt = Dt.Rows[0]["TIGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TSGSTAmt"].ToString();

            string TIGSTPer = Dt.Rows[0]["TIGSTPer"].ToString() == "" ? "0" : Dt.Rows[0]["TIGSTPer"].ToString();
            string TIGSTAmt = Dt.Rows[0]["TIGSTAmt"].ToString() == "" ? "0" : Dt.Rows[0]["TIGSTAmt"].ToString();

            string TotalCost = Dt.Rows[0]["TotalCost"].ToString() == "" ? "0" : Dt.Rows[0]["TotalCost"].ToString();

            string DateOfReceived = Dt.Rows[0]["DateOfReceived"].ToString() == "" ? "0" : Dt.Rows[0]["DateOfReceived"].ToString();

            string TCSPer = Dt.Rows[0]["TCSPer"].ToString() == "" ? "0" : Dt.Rows[0]["TCSPer"].ToString();
            string TCSAmount = Dt.Rows[0]["TCSAmount"].ToString() == "" ? "0" : Dt.Rows[0]["TCSAmount"].ToString();
            string Remarks = Dt.Rows[0]["Remarks"].ToString() == "" ? "" : Dt.Rows[0]["Remarks"].ToString();


            //17 march 2022
            string Transporattioncharges = Dt.Rows[0]["TotalCost"].ToString();
            string TGST = Dt.Rows[0]["TCGSTPer"].ToString();
            string TIGST = Dt.Rows[0]["TIGSTPer"].ToString();
            string gstper = "0";
            if (TIGST == "0")
            {
                gstper = TGST.ToString();
            }
            else
            {
                gstper = TIGST.ToString();
            }

            string BillToAddress = "";
            string ShipToAddress = "";
            string StateName = "";
            string GSTNo = "";
            string PANNo = "";
            string EmailID = "";
            string PODate = "";
            string contNo = "";

            double CGSTamt = 0;
            double SGSTamt = 0;
            double IGSTamt = 0;

            SqlDataAdapter add = new SqlDataAdapter("select * from tblPurchaseOrderHdr where PONo='" + AgainstNumber + "'", con);
            DataTable dtt = new DataTable();
            add.Fill(dtt);
            if (dtt.Rows.Count > 0)
            {
                PODate = dtt.Rows[0]["PODate"].ToString().TrimEnd("0:0".ToCharArray());
            }

            SqlDataAdapter ad = new SqlDataAdapter("select * from tbl_VendorMaster where Vendorname='" + SupplierName + "'", con);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                BillToAddress = dt.Rows[0]["Address"].ToString();
                ShipToAddress = dt.Rows[0]["Address"].ToString();
                StateName = dt.Rows[0]["State"].ToString();
                GSTNo = dt.Rows[0]["GSTNo"].ToString();
                PANNo = dt.Rows[0]["PANNo"].ToString();
                EmailID = dt.Rows[0]["EmailID"].ToString();
                contNo = dt.Rows[0]["MobileNo"].ToString();
                var id = dt.Rows[0]["ID"].ToString();
               
            }

            //PdfContentByte cb = writer.DirectContent;
            //cb.Rectangle(28f, 740f, 560f, 80f);
            //cb.Stroke();
            //// Header 

            //cb.BeginText();
            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 20);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, SupplierName, 300, 790, 0);
            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 9);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, BillToAddress, 290, 775, 0);
            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 9);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 250, 748, 0);
            //cb.EndText();

            //PdfContentByte cbb = writer.DirectContent;
            //cbb.Rectangle(17f, 710f, 560f, 25f);
            //cbb.Stroke();
            //// Header 
            //cbb.BeginText();
            //cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            //cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " CONTACT : 9225658662   Email ID : mktg@excelenclosures.com", 153, 722, 0);
            //cbb.EndText();

            PdfContentByte cbbb = writer.DirectContent;
            cbbb.Rectangle(28f, 740f, 560f, 25f);
            cbbb.Stroke();
            // Header 
            cbbb.BeginText();
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GSTIN :" + GSTNo + "", 48, 750, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PAN NO: " + PANNo + "", 170, 750, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "EMAIL : " + EmailID + "", 280, 750, 0);
            cbbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 10);
            cbbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "CONTACT : " + contNo + "", 455, 750, 0);
            cbbb.EndText();

            PdfContentByte cd = writer.DirectContent;
            cd.Rectangle(28f, 715f, 560f, 25f);
            cd.Stroke();
            // Header 
            cd.BeginText();
            cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 14);
            cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "PURCHASE BILL", 270, 722, 0);
            cd.EndText();

            Paragraph paragraphTable1 = new Paragraph();
            paragraphTable1.SpacingBefore = 120f;
            paragraphTable1.SpacingAfter = 1f;

            PdfPTable mtable = new PdfPTable(2);
            mtable.WidthPercentage = 102;
            mtable.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 275f;
            table.LockedWidth = true;
            table.HorizontalAlignment = 1;
            table.SetWidths(new float[] { 180f });
            table.AddCell(new Phrase(" Details of Consignee \n\n Pune Abrasive Pvt. Ltd. \n Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk, Near Shell Petrol Pump, Pune 411019 \n\n GSTIN :27ABCCS7002A1ZW  PAN No: ATF****5J  \n\n State Name:Maharashtra(27) \n  ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(" Details of Shipped to \n\n Pune Abrasive Pvt. Ltd. \n Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk, Near Shell Petrol Pump, Pune 411019 \n\n GSTIN :27ABCCS7002A1ZW  PAN No: ATF****5J \n\n State Name:Maharashtra(27) \n  ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            mtable.AddCell(table);

            table = new PdfPTable(2);
            float[] widths2 = new float[] { 100, 180 };
            table.SetWidths(widths2);
            table.TotalWidth = 285f;
            table.HorizontalAlignment = 2;
            table.LockedWidth = true;

            var date = DateTime.Now.ToString("yyyy-MM-dd");

            table.AddCell(new Phrase("Bill Number : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(SupplierBillNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Bill Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(BillDate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("PO No : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(AgainstNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("PO Date :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(PODate, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Transportation Mode", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(TransportMode, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Vehicle No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(VehicleNo, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Place of Supply :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(BillToAddress, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Date of Received :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(DateOfReceived, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("E-Bill No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase(EBillNumber, FontFactory.GetFont("Arial", 9, Font.NORMAL)));

            table.AddCell(new Phrase("Reverse Charge :", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            table.AddCell(new Phrase("No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            mtable.AddCell(table);


            paragraphTable1.Add(mtable);
            doc.Add(paragraphTable1);

            PdfPCell tblcell = null;
            Paragraph paragraphTable2 = new Paragraph();
            paragraphTable2.SpacingAfter = 0f;
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                float[] widths3 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f };
                table.SetWidths(widths3);
            }
            else
            {
                table = new PdfPTable(10);
                float[] widths3 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f };
                table.SetWidths(widths3);
            }
            double Ttotal_price = 0;
            double CGST_price = 0;
            double SGST_price = 0;
            double IGST_price = 0;
            if (Dt.Rows.Count > 0)
            {
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                tblcell = new PdfPCell(new Phrase("SN.", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Name Of Particulars", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("HSN Code", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Qty", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Rate", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Disc (%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Amount", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                if (Dt.Rows[0]["igstperc"].ToString() == "0")
                {
                    tblcell = new PdfPCell(new Phrase("CGST(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("CGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("SGST(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("SGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                else
                {
                    tblcell = new PdfPCell(new Phrase("IGST(%)", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase("IGST Amt", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                tblcell = new PdfPCell(new Phrase("Total", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);

                int rowid = 1;
                foreach (DataRow dr in Dt.Rows)
                {
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;
                    table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;

                    double Ftotal = Convert.ToDouble(dr["grandtot"].ToString());
                    string _ftotal = Ftotal.ToString("##.00");

                    string Description = dr["Particulars"].ToString() + "\n" + dr["Description"].ToString();

                    var amt = dr["Amount"].ToString();
                    var cgstper = dr["cgstperc"].ToString() == "" ? "0" : dr["cgstperc"].ToString();
                    var sgstper = dr["sgstperc"].ToString() == "" ? "0" : dr["sgstperc"].ToString();
                    var igstper = dr["igstperc"].ToString() == "" ? "0" : dr["igstperc"].ToString();

                    var cgstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(cgstper) / 100;
                    var sgstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(sgstper) / 100;
                    var igstamt = Convert.ToDecimal(amt) * Convert.ToDecimal(igstper) / 100;

                    var UOM = dr["UOM"].ToString();

                    tblcell = new PdfPCell(new Phrase(rowid.ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Description, FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(dr["HSN"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(dr["Qty"].ToString() + " " + UOM, FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(dr["ratee"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    tblcell = new PdfPCell(new Phrase(dr["Discount"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    tblcell = new PdfPCell(new Phrase(dr["Amount"].ToString(), FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    if (Dt.Rows[0]["igstperc"].ToString() == "0")
                    {
                        tblcell = new PdfPCell(new Phrase(Convert.ToDouble(dr["cgstperc"].ToString() == "" ? "0" : dr["cgstperc"].ToString()).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(cgstamt.ToString(), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(Convert.ToDouble(dr["sgstperc"].ToString() == "" ? "0" : dr["sgstperc"].ToString()).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(sgstamt.ToString(), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                    }
                    else
                    {
                        tblcell = new PdfPCell(new Phrase(Convert.ToDouble(dr["igstperc"].ToString() == "" ? "0" : dr["igstperc"].ToString()).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                        tblcell = new PdfPCell(new Phrase(igstamt.ToString(), FontFactory.GetFont("Arial", 8)));
                        tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                        tblcell.HorizontalAlignment = 1;
                        table.AddCell(tblcell);
                    }
                    tblcell = new PdfPCell(new Phrase(_ftotal, FontFactory.GetFont("Arial", 8)));
                    tblcell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    rowid++;

                    Ttotal_price += Convert.ToDouble(dr["Amount"].ToString());
                    CGST_price += Convert.ToDouble(cgstamt);
                    SGST_price += Convert.ToDouble(sgstamt);
                    IGST_price += Convert.ToDouble(igstamt);
                }

            }

            var Resultamt = Ttotal_price + Convert.ToDouble(Basic) + Convert.ToDouble(TransportationCharges); ;

            string amount = Resultamt.ToString();
            paragraphTable2.Add(table);
            doc.Add(paragraphTable2);

            //var CGSTResAmt = Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["cgstperc"].ToString()) / 100;
            //var SGSTResAmt = Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["sgstperc"].ToString()) / 100;
            //var IGSTResAmt = Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["igstperc"].ToString()) / 100;



            //Space
            Paragraph paragraphTable3 = new Paragraph();

            string[] items = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font10 = FontFactory.GetFont("Arial", 10, Font.NORMAL);
            Paragraph paragraph = new Paragraph("", font12);

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                if (Dt.Rows.Count == 1)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 2)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 3)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 4)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                }
                doc.Add(table);
            }
            else
            {
                table = new PdfPTable(10);
                table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                // table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                if (Dt.Rows.Count == 1)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 2)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 3)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else if (Dt.Rows.Count == 4)
                {
                    table.AddCell(new Phrase("\n\n\n\n\n\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                }
                else
                {
                    table.AddCell(new Phrase("\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                }
                doc.Add(table);
            }

            //Freight
            Paragraph paragraphTable222 = new Paragraph();
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.TotalWidth = 560f;
                float[] widths33 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f };
                paragraphTable222.SpacingAfter = 0f;
                table.SetWidths(widths33);
            }
            else
            {
                table = new PdfPTable(10);
                table.TotalWidth = 560f;
                float[] widths33 = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f };
                paragraphTable222.SpacingAfter = 0f;
                table.SetWidths(widths33);
            }

            if (Dt.Rows.Count > 0)
            {
                table.LockedWidth = true;
                //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(chargeDescription, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(HSNSAC, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(Convert.ToDouble(Basic).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                if (Dt.Rows[0]["igstperc"].ToString() == "0")
                {
                    tblcell = new PdfPCell(new Phrase(Convert.ToDouble(CGSTPerr).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    CGSTamt = Convert.ToDouble(Basic) * Convert.ToDouble(CGSTPerr) / 100;
                    SGSTamt = Convert.ToDouble(Basic) * Convert.ToDouble(SGSTPerr) / 100;

                    tblcell = new PdfPCell(new Phrase(CGSTamt.ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(Convert.ToDouble(SGSTPerr).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(SGSTamt.ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                else
                {
                    IGSTamt = Convert.ToDouble(Basic) * Convert.ToDouble(IGSTPerr) / 100;
                    tblcell = new PdfPCell(new Phrase(Convert.ToDouble(IGSTPerr).ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);

                    tblcell = new PdfPCell(new Phrase(IGSTamt.ToString("N2", info), FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                tblcell = new PdfPCell(new Phrase(Cost, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
            }
            paragraphTable222.Add(table);
            doc.Add(paragraphTable222);

            /////////////////////////


            //Transportation
            Paragraph paragraphTableTransport = new Paragraph();
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                table = new PdfPTable(12);
                table.TotalWidth = 560f;
                float[] widths33tr = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 8f, 12f, 16f };
                paragraphTableTransport.SpacingAfter = 0f;
                table.SetWidths(widths33tr);
            }
            else
            {
                table = new PdfPTable(10);
                table.TotalWidth = 560f;
                float[] widths33tr = new float[] { 4f, 40f, 13f, 8f, 10f, 8f, 15f, 8f, 12f, 16f };
                paragraphTableTransport.SpacingAfter = 0f;
                table.SetWidths(widths33tr);
            }
            if (Dt.Rows.Count > 0)
            {
                table.LockedWidth = true;
                //table.DefaultCell.Border = Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("Transporation Charges", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase("", FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                tblcell = new PdfPCell(new Phrase(TransportationCharges, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
                if (Dt.Rows[0]["igstperc"].ToString() == "0")
                {
                    tblcell = new PdfPCell(new Phrase(TCGSTPer, FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(TCGSTAmt, FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(TSGSTPer, FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(TSGSTAmt, FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                else
                {
                    tblcell = new PdfPCell(new Phrase(TIGSTPer, FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                    tblcell = new PdfPCell(new Phrase(TIGSTAmt, FontFactory.GetFont("Arial", 8)));
                    tblcell.HorizontalAlignment = 1;
                    table.AddCell(tblcell);
                }
                tblcell = new PdfPCell(new Phrase(TotalCost, FontFactory.GetFont("Arial", 8)));
                tblcell.HorizontalAlignment = 1;
                table.AddCell(tblcell);
            }
            paragraphTableTransport.Add(table);
            doc.Add(paragraphTableTransport);

            /////////////////////////
            ///

            var CGSTResAmt = CGST_price + CGSTamt + Convert.ToDouble(TCGSTAmt); //Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["cgstperc"].ToString()) / 100;
            var SGSTResAmt = SGST_price + SGSTamt + Convert.ToDouble(TSGSTAmt); //Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["sgstperc"].ToString()) / 100;
            var IGSTResAmt = IGST_price + IGSTamt + Convert.ToDouble(TIGSTAmt); //Convert.ToDecimal(amount) * Convert.ToDecimal(Dt.Rows[0]["igstperc"].ToString()) / 100;


            //Add Total Row start
            Paragraph paragraphTable5 = new Paragraph();

            string[] itemsss = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font13 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font11 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphh = new Paragraph("", font12);



            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);
            PdfPCell cell = new PdfPCell(new Phrase("Value of Supply", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell);

            var valueTotalf = Convert.ToDouble(amount);

            string ValueTotalamt = valueTotalf.ToString("N2", info);

            PdfPCell cell11 = new PdfPCell(new Phrase(ValueTotalamt, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cell11.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell11);
            doc.Add(table);

            //Grand total Row STart
            Paragraph paragraphTable17 = new Paragraph();
            paragraphTable5.SpacingAfter = 0f;

            string[] itemm = { "Goods once sold will not be taken back or exchange. \b",
                        "Interest at the rate of 18% will be charged if bill is'nt paid within 30 days.\b",
                        "Our risk and responsibility ceases the moment goods leaves out godown. \n",
                        };

            Font font16 = FontFactory.GetFont("Arial", 12, Font.BOLD);
            Font font17 = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphhhhh = new Paragraph("", font12);

            //paragraphh.SpacingAfter = 10f;

            for (int i = 0; i < items.Length; i++)
            {
                paragraph.Add(new Phrase("", font10));
            }

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);

            decimal Taxamot = 0; decimal Totalamt = 0;
            if (Dt.Rows[0]["igstperc"].ToString() == "0")
            {
                PdfPCell cell444 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell444);
                PdfPCell cell555 = new PdfPCell(new Phrase(CGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell555);

                PdfPCell cell4440 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell4440);
                PdfPCell cell5550 = new PdfPCell(new Phrase(CGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell5550);

                doc.Add(table);

                //double Taxamot = CGST_price + SGST_price;
                Taxamot = Convert.ToDecimal(CGSTResAmt + SGSTResAmt);

                Totalamt = Convert.ToDecimal(amount) + Convert.ToDecimal(Taxamot) + Convert.ToDecimal(TCSAmount);

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                paragraph.Alignment = Element.ALIGN_RIGHT;

                table.SetWidths(new float[] { 0f, 119f, 14f });
                table.AddCell(paragraph);
                PdfPCell cell4444 = new PdfPCell(new Phrase("Add SGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell4444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell4444);

                PdfPCell cell5555 = new PdfPCell(new Phrase(SGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell5555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell5555);
                doc.Add(table);

            }
            else
            {
                PdfPCell cell444 = new PdfPCell(new Phrase("Add IGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell444);
                PdfPCell cell555 = new PdfPCell(new Phrase(IGSTResAmt.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell555);

                //PdfPCell cell4440 = new PdfPCell(new Phrase("Add CGST", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                //cell444.HorizontalAlignment = Element.ALIGN_RIGHT;
                //table.AddCell(cell4440);
                //PdfPCell cell5550 = new PdfPCell(new Phrase(CGSTResAmt.ToString(), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                //cell555.HorizontalAlignment = Element.ALIGN_RIGHT;
                //table.AddCell(cell5550);

                doc.Add(table);

                //double Taxamot = CGST_price + SGST_price;
                Taxamot = Convert.ToDecimal(IGSTResAmt);

                Totalamt = Convert.ToDecimal(amount) + Convert.ToDecimal(Taxamot) + Convert.ToDecimal(TCSAmount);
            }
            ///

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);
            PdfPCell cellTaxAmount = new PdfPCell(new Phrase("Tax Amount", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTaxAmount.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTaxAmount);

            PdfPCell cellTaxAmount1 = new PdfPCell(new Phrase(Taxamot.ToString("N2", info), FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTaxAmount1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTaxAmount1);
            doc.Add(table);

            //TCS
            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);
            PdfPCell cellTCharges = new PdfPCell(new Phrase("TCS [ " + TCSPer + " % ]", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTCharges.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTCharges);

            PdfPCell cellTCharges1 = new PdfPCell(new Phrase(TCSAmount, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
            cellTCharges1.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cellTCharges1);
            doc.Add(table);

            //////////////////////////////

            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 14f });
            table.AddCell(paragraph);

            PdfPCell cell44 = new PdfPCell(new Phrase("Total Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell44.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell44);

            //decimal FinalTot = Math.Round(Totalamt);

            var Totalamtf = Convert.ToDouble(Totalamt);
            var Totalamtfff = Math.Round(Totalamtf);
            string FinaleTotalamt = Totalamtfff.ToString("N2", info);

            //var FTotalf = Convert.ToDouble(FinalTot);
            //string FTotalamt = FTotalf.ToString("N2", info);

            PdfPCell cell440 = new PdfPCell(new Phrase(FinaleTotalamt, FontFactory.GetFont("Arial", 10, Font.BOLD)));
            cell440.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(cell440);
            doc.Add(table);

            string Amtinword = ConvertNumbertoWords(Convert.ToInt32(Totalamtf));

            //Total amount In word
            table = new PdfPTable(3);
            table.TotalWidth = 560f;
            table.LockedWidth = true;

            paragraph.Alignment = Element.ALIGN_RIGHT;

            table.SetWidths(new float[] { 0f, 119f, 0f });
            table.AddCell(paragraph);

            PdfPCell cell4434 = new PdfPCell(new Phrase("Total Amount: " + Amtinword + " Only" + "\n\n" + "Remarks :" + Remarks, FontFactory.GetFont("Arial", 9, Font.BOLD)));
            cell4434.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell4434);

            //var finaleTotalf = Convert.ToDouble(FinalTot);
            //string FinaleTotalamt = finaleTotalf.ToString("N2", info);

            PdfPCell cell44044 = new PdfPCell(new Phrase(FinaleTotalamt + "\n\n" + "Remarks :" + Remarks, FontFactory.GetFont("Arial", 9, Font.BOLD)));
            cell44044.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell44044);
            doc.Add(table);

            Paragraph paragraphTable99 = new Paragraph(" Remarks :\n\n", font12);

            //Puja Enterprises Sign
            string[] itemss = {
                "Declaration  : I/We hereby certify that my/our registration certificate under the GST Act, 2017 is in force on the date on which the supply of the goods specified in this tax invoice is made by me/us and that the transaction of supplies covered by this tax invoice has been effected by me/us and it shall be accounted for in the turnover of supplies while filing of return and the due tax, if any, payable on the supplies has been paid or shall be paid.",
                " \n",
                        };

            Font font14 = FontFactory.GetFont("Arial", 11);
            Font font15 = FontFactory.GetFont("Arial", 9, Font.NORMAL);
            Font fontBold = FontFactory.GetFont("Arial", 10, Font.BOLD);
            Paragraph paragraphhh = new Paragraph(" Terms & Condition :\n\n", font10);


            for (int i = 0; i < itemss.Length; i++)
            {
                //paragraphhh.Add(new Phrase("\u2022 \u00a0" + itemss[i] + "\n", font15));
                paragraphhh.Add(new Phrase(itemss[i] + "\n", font15));
            }

            table = new PdfPTable(1);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 560f });

            table.AddCell(paragraphhh);
            //table.AddCell(new Phrase("Puja Enterprises \n\n\n\n         Sign", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            //table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD)));
            //doc.Add(table);

            Paragraph paragraphTable10000 = new Paragraph();

            //Puja Enterprises Sign
            string[] itemss4 = {
                "Payment Term     ",

                        };

            Font font144 = FontFactory.GetFont("Arial", 11);
            Font font155 = FontFactory.GetFont("Arial", 8);
            Paragraph paragraphhhhhff = new Paragraph();


            //for (int i = 0; i < itemss4.Length; i++)
            //{
            //    paragraphhhhhff.Add(new Phrase("\u2022 \u00a0" + itemss4[i] + "\n", font155));
            //}

            table = new PdfPTable(2);
            table.TotalWidth = 560f;
            table.LockedWidth = true;
            table.SetWidths(new float[] { 300f, 100f });

            //table.AddCell(paragraphhhhhff);
            table.AddCell(paragraphhh);
            table.AddCell(new Phrase("" + SupplierName + " \n\n\n\n\n\n\nAuthorised Signature", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 9, Font.BOLD)));
            doc.Add(table);
            doc.Close();


            // Byte[] FileBuffer = File.ReadAllBytes(Server.MapPath("~/files/") + DocName);

            //Font blackFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    PdfReader reader = new PdfReader(FileBuffer);
            //    using (PdfStamper stamper = new PdfStamper(reader, stream))
            //    {
            //        int pages = reader.NumberOfPages;
            //        for (int i = 1; i <= pages; i++)
            //        {
            //            if (i == 1)
            //            {

            //            }
            //            else
            //            {
            //                var pdfbyte = stamper.GetOverContent(i);
            //                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageURL);
            //                image.ScaleToFit(70, 100);
            //                image.SetAbsolutePosition(40, 792);
            //                image.SpacingBefore = 50f;
            //                image.SpacingAfter = 1f;
            //                image.Alignment = Element.ALIGN_LEFT;
            //                pdfbyte.AddImage(image);
            //            }
            //            var PageName = "Page No. " + i.ToString();
            //            ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_RIGHT, new Phrase(PageName, blackFont), 568f, 820f, 0);
            //        }
            //    }
            //    FileBuffer = stream.ToArray();
            //}


            //string empFilename = QuatationNumber + " " + PartyName + ".pdf";

            //if (FileBuffer != null)
            //{
            //    Response.ContentType = "application/pdf";
            //    Response.AddHeader("content-length", FileBuffer.Length.ToString());
            //    Response.BinaryWrite(FileBuffer);
            //    Response.AddHeader("Content-Disposition", "attachment;filename=" + empFilename);
            //}
            //ifrRight6.Attributes["src"] = @"../files/" + DocName;
        }





        doc.Close();
        return pdf;
        //Session["PDFID"] = null
    }


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
            sb.Append("Rupees ");
        }
        else if (paisaamt > 0)
        {
            var paisatext = ConvertNumbertoWords(paisaamt);
            sb.AppendFormat("Rupees {0} paise", paisatext);
        }
        return sb.ToString().TrimEnd();
    }


    protected void txtSupplierBillNo_TextChanged(object sender, EventArgs e)
    {
        DataTable Dt = new DataTable();
        SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM tblPurchaseBillHdr WHERE SupplierBillNo='" + txtSupplierBillNo.Text + "' AND SupplierName='" + txtSupplierName.Text + "' ", con);
        Da.Fill(Dt);

        if (Dt.Rows.Count > 0)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Supplier Bill No Alredy Exist...')", true);
        }
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("PurchaseBillList.aspx");
    }
}