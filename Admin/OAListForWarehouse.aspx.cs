
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


public partial class Admin_OAListForWarehouse : System.Web.UI.Page
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


    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("AddCustomerPO.aspx");
    }

    //Fill GridView
    private void FillGrid()
    {
        try
        {
            SqlCommand cmd = new SqlCommand("SP_GetTableDetails", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetOAList");
            cmd.Parameters.AddWithValue("@CompanyName", txtCustomerName.Text);
            cmd.Parameters.AddWithValue("@PONO", txtCpono.Text);
            cmd.Parameters.AddWithValue("@GST", txtGST.Text);
            cmd.Parameters.AddWithValue("@PageSize", Convert.ToInt32(ddlPageSize.SelectedValue));
            cmd.Parameters.AddWithValue("@FromDate", txtfromdate.Text);
            cmd.Parameters.AddWithValue("@ToDate", txttodate.Text);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Columns.Count > 0)
            {
                GVPurchase.DataSource = dt;
                GVPurchase.DataBind();
            }


        }
        catch (Exception ex)
        {
            //throw ex;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "HideLabel('" + errorMsg + "') ", true);
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
      
      
    }


    protected void GVPurchase_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName == "RowEdit")
        //{
        //    Response.Redirect("AddCustomerPO.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        //}
        if (e.CommandName == "AddInvoice")
        {
            Response.Redirect("../Account/TaxInvoice.aspx?PONO=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        //if (e.CommandName == "RowDelete")
        //{
        //    Cls_Main.Conn_Open();
        //    SqlCommand Cmd = new SqlCommand("UPDATE [tbl_CustomerPurchaseOrderHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
        //    Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
        //    Cmd.Parameters.AddWithValue("@IsDeleted", '1');
        //    Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
        //    Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
        //    Cmd.ExecuteNonQuery();
        //    Cls_Main.Conn_Close();
        //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Order Acceptance Deleted Successfully..!!')", true);
        //    FillGrid();
        //}
        if (e.CommandName == "RowView")
        {
            Response.Redirect("Pdf_CustomerPurchase.aspx?Pono=" + objcls.encrypt(e.CommandArgument.ToString()) + " ");
            // Response.Write("<script>window.open ('Pdf_Quotation.aspx?Quotationno=" + (e.CommandArgument.ToString()) + "','_blank');</script>");
        }
    }

    protected void GVPurchase_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVPurchase.PageIndex = e.NewPageIndex;
        FillGrid();
    }


    protected void GVPurchase_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Authorization
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
        //    LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

        //    string empcode = Session["UserCode"].ToString();
        //    DataTable Dt = new DataTable();
        //    SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
        //    Sd.Fill(Dt);
        //    if (Dt.Rows.Count > 0)
        //    {
        //        string id = Dt.Rows[0]["ID"].ToString();
        //        DataTable Dtt = new DataTable();
        //        SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'CustomerPurchaseOrderList.aspx' AND PagesView = '1'", con);
        //        Sdd.Fill(Dtt);
        //        if (Dtt.Rows.Count > 0)
        //        {
        //          //  btnCreate.Visible = false;
        //            //GVQuotation.Columns[15].Visible = false;
        //          //  btnEdit.Visible = false;
        //          //  btnDelete.Visible = false;
        //        }
        //    }
        //}
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnInvoice = e.Row.FindControl("btnInvoice") as LinkButton;
            Label OANumber = e.Row.FindControl("Pono") as Label;
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select TOP 1 InvoiceNo from tblTaxInvoiceHdr where AgainstNumber='" + OANumber.Text + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                btnInvoice.Visible = false;
            }
            else
            {
                btnInvoice.Visible = true;
            }
        }
    }

    //Search Company Search methods
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
                com.CommandText = "SELECT DISTINCT CustomerName FROM [tbl_CustomerPurchaseOrderHdr] where " + "CustomerName like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["CustomerName"].ToString());
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

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("OAListForWarehouse.aspx");
    }

    //Search Customer P.O.  Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCponoList(string prefixText, int count)
    {
        return AutoFillCponoName(prefixText);
    }

    public static List<string> AutoFillCponoName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT SerialNo FROM [tbl_CustomerPurchaseOrderHdr] where " + "SerialNo like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["SerialNo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void txtCpono_TextChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtCustomerName.Text) && string.IsNullOrEmpty(txtCpono.Text) && string.IsNullOrEmpty(txtfromdate.Text) && string.IsNullOrEmpty(txttodate.Text))
            {
                FillGrid();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Search Record');", true);
            }
            else
            {
                if (Session["Role"].ToString() == "Admin")
                {
                    if (txtCpono.Text != "")
                    {
                        string Quono = txtCpono.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where SerialNo = '" + Quono + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                    if (txtCustomerName.Text != "")
                    {
                        string company = txtCustomerName.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName where CustomerName = '" + company + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE  CP.IsDeleted = 0 AND CP.CreatedOn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        sad.Fill(dt);

                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                }
                else
                {
                    if (txtCpono.Text != "")
                    {
                        string Quono = txtCpono.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  SerialNo = '" + Quono + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                    if (txtCustomerName.Text != "")
                    {
                        string company = txtCustomerName.Text;

                        DataTable dt = new DataTable();
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  CustomerName = '" + company + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);
                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }

                    if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                    {
                        DataTable dt = new DataTable();

                        //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                        SqlDataAdapter sad = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CP LEFT JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE (CP.CreatedBy='" + Session["UserCode"].ToString() + "' OR CP.UserName='" + Session["UserCode"].ToString() + "') AND  CP.CreatedOn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' AND CP.IsDeleted = 0", Cls_Main.Conn);
                        sad.Fill(dt);

                        GVPurchase.EmptyDataText = "Not Records Found";
                        GVPurchase.DataSource = dt;
                        GVPurchase.DataBind();
                    }
                 
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //Search GST WIse Company methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetGSTList(string prefixText, int count)
    {
        return AutoFillGSTName(prefixText);
    }

    public static List<string> AutoFillGSTName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT GSTNo FROM [tbl_CustomerPurchaseOrderHdr] where " + "GSTNo like '%'+ @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["GSTNo"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtGST_TextChanged(object sender, EventArgs e)
    {
        FillGrid();

    }

    protected void ImageButtonfile5_Click(object sender, ImageClickEventArgs e)
    {
        string id = ((sender as ImageButton).CommandArgument).ToString();

        Display(id);
    }

    public void Display(string id)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                string CmdText = "select fileName from tbl_CustomerPurchaseOrderHdr where IsDeleted=0 AND ID='" + id + "'";

                SqlDataAdapter ad = new SqlDataAdapter(CmdText, con);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //Response.Write(dt.Rows[0]["Path"].ToString());
                    if (!string.IsNullOrEmpty(dt.Rows[0]["fileName"].ToString()))
                    {
                        Response.Redirect("~/PDF_Files/"+dt.Rows[0]["fileName"].ToString());
                    }
                    else
                    {
                        //lblnotfound.Text = "File Not Found or Not Available !!";
                    }
                }
                else
                {
                    //lblnotfound.Text = "File Not Found or Not Available !!";
                }

            }
        }
    }

    //protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    FillGrid();
    //}

    protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillGrid();
    }
}


