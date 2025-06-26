
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


public partial class Admin_InwardEntryList : System.Web.UI.Page
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
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_InwardEntryHdr] WHERE IsDeleted = 0 ORDER BY ID DESC");
        GVInward.DataSource = Dt;
        GVInward.DataBind();
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
                com.CommandText = "SELECT DISTINCT [Suppliername] FROM [tbl_InwardEntryHdr] where " + "Suppliername like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Suppliername"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtCustomerName_TextChanged(object sender, EventArgs e)
    {
        if (txtCustomerName.Text != "" || txtCustomerName.Text != null)
        {
            string company = txtCustomerName.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_InwardEntryHdr] where Suppliername = '" + company + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVInward.EmptyDataText = "Not Records Found";
            GVInward.DataSource = dt;
            GVInward.DataBind();
        }
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("InwardEntry.aspx");
    }

    protected void GVInward_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("InwardEntry.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }

           if (e.CommandName == "RowDelete")
   {
       Cls_Main.Conn_Open();
       SqlCommand Cmd = new SqlCommand("UPDATE [tbl_InwardEntryHdr] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
       Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
       Cmd.Parameters.AddWithValue("@IsDeleted", '1');
       Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
       Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
       Cmd.ExecuteNonQuery();
       Cls_Main.Conn_Close();            
       FillGrid();

       con.Open();
       SqlCommand cmd1 = new SqlCommand("select OrderNo from tbl_InwardEntryHdr where Id = " + Convert.ToInt32(e.CommandArgument.ToString()) + "", con);
       Object OrderNo = cmd1.ExecuteScalar();

       SqlCommand Cmd4 = new SqlCommand("delete tbl_InwardComponentsdtls where OrderNo=@OrderNo", con);
       Cmd4.Parameters.AddWithValue("@OrderNo", OrderNo);
       Cmd4.ExecuteNonQuery();
       con.Close();
       ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "HideLabel('Enward Entry Deleted Successfully..!!')", true);
   }
       
    }

    protected void GVInward_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVInward.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVInward_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //Authorization
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnEdit = e.Row.FindControl("btnEdit") as LinkButton;
            LinkButton btnDelete = e.Row.FindControl("btnDelete") as LinkButton;

            string empcode = Session["UserCode"].ToString();
            DataTable Dt = new DataTable();
            SqlDataAdapter Sd = new SqlDataAdapter("Select ID from tbl_UserMaster where UserCode='" + empcode + "'", con);
            Sd.Fill(Dt);
            if (Dt.Rows.Count > 0)
            {
                string id = Dt.Rows[0]["ID"].ToString();
                DataTable Dtt = new DataTable();
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'InwardEntryList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate.Visible = false;
                    GVInward.Columns[7].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(txtOrderno.Text) && string.IsNullOrEmpty(txtCustomerName.Text) && string.IsNullOrEmpty(txtfromdate.Text) && string.IsNullOrEmpty(txttodate.Text))
            {
                FillGrid();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Please Search Record');", true);
            }
            else
            {
                if (txtOrderno.Text != "")
                {
                    string OrderNo = txtOrderno.Text;

                    DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_InwardEntryHdr] where OrderNo = '" + OrderNo + "' AND IsDeleted = 0", Cls_Main.Conn);
                    sad.Fill(dt);
                    GVInward.EmptyDataText = "Not Records Found";
                    GVInward.DataSource = dt;
                    GVInward.DataBind();
                }
                if (txtCustomerName.Text != "")
                {
                    string company = txtCustomerName.Text;

                    DataTable dt = new DataTable();
                    SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_InwardEntryHdr] where Suppliername = '" + company + "' AND IsDeleted = 0", Cls_Main.Conn);
                    sad.Fill(dt);
                    GVInward.EmptyDataText = "Not Records Found";
                    GVInward.DataSource = dt;
                    GVInward.DataBind();
                }

                if (!string.IsNullOrEmpty(txtfromdate.Text) && !string.IsNullOrEmpty(txttodate.Text))
                {
                    DataTable dt = new DataTable();

                    //SqlDataAdapter sad = new SqlDataAdapter(" select [Id],[JobNo],[DateIn],[CustName],[Subcustomer],[Branch],[MateName],[SrNo],[MateStatus],FinalStatus,[TestBy],[ModelNo],[otherinfo],[Imagepath],[CreatedBy],[CreatedDate],[UpdateBy],[UpdateDate] ,ProductFault,RepeatedNo,DATEDIFF(DAY, CreatedDate, getdate()) AS days FROM [tblInwardEntry] Where DateIn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                    SqlDataAdapter sad = new SqlDataAdapter(" SELECT * FROM [tbl_InwardEntryHdr] WHERE CreatedOn between'" + txtfromdate.Text + "' AND '" + txttodate.Text + "' ", Cls_Main.Conn);
                    sad.Fill(dt);

                    GVInward.EmptyDataText = "Not Records Found";
                    GVInward.DataSource = dt;
                    GVInward.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.RawUrl);
    }

    //Search Company Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetOrderNoList(string prefixText, int count)
    {
        return AutoFillOrderNo(prefixText);
    }

    public static List<string> AutoFillOrderNo(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT [ID],[InvoiceNo] FROM [tbl_InwardEntryHdr] where " + "InvoiceNo like '%'+ @Search + '%' and IsDeleted=0";

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

    protected void txtOrderno_TextChanged(object sender, EventArgs e)
    {
        string OrderNo = txtOrderno.Text;

        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_InwardEntryHdr] where InvoiceNo = '" + OrderNo + "' AND IsDeleted = 0", Cls_Main.Conn);
        sad.Fill(dt);
        GVInward.EmptyDataText = "Not Records Found";
        GVInward.DataSource = dt;
        GVInward.DataBind();

    }
}