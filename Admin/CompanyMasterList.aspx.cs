
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


public partial class Admin_CompanyMasterList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           
            if (Session["UserCode"] == null)
            {
                Response.Redirect("../Login.aspx");
            }
            else
            {
                FillGrid();  
            }

        }
    }


    //Fill GridView
    private void FillGrid()
    {
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_CompanyMaster] WHERE IsDeleted = 0 ORDER BY ID DESC");
        GVCompany.DataSource = Dt;
        GVCompany.DataBind();
    }

    public void Display(string id, string Fpath)
    {
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                Int64 ID = Convert.ToInt64(id);
                string CmdText = "SELECT [ID],'../'+[VisitingCardPath] as Path FROM [tbl_CompanyMaster] where ID='" + ID + "'";

                SqlDataAdapter ad = new SqlDataAdapter(CmdText, con);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //Response.Write(dt.Rows[0]["Path"].ToString());
                    if (!string.IsNullOrEmpty(dt.Rows[0]["Path"].ToString()))
                    {
                        Response.Redirect(dt.Rows[0]["Path"].ToString());
                    }
                    else
                    {
                        lblnotfound.Text = "File Not Found or Not Available !!";
                    }
                }
                else
                {
                    lblnotfound.Text = "File Not Found or Not Available !!";
                }

            }
        }
    }

    protected void GVCompany_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("CompanyMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_CompanyMaster] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Company Deleted Successfully..!!')", true);
            FillGrid();
        }
    }

    protected void GVCompany_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVCompany.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("CompanyMaster.aspx");
    }

   

    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("CompanyMasterList.aspx");
    }

    protected void ImageButtonfile1_Click(object sender, ImageClickEventArgs e)
    {
        string Fpath;
    
            string id = ((sender as ImageButton).CommandArgument).ToString();
            Fpath = "filepath1";
            Display(id, Fpath);          
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
                com.CommandText = "SELECT DISTINCT [ID],[Companyname] FROM [tbl_CompanyMaster] where " + "Companyname like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Companyname"].ToString());
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
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_CompanyMaster] where Companyname = '" + company + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVCompany.EmptyDataText = "Not Records Found";
            GVCompany.DataSource = dt;
            GVCompany.DataBind();
        }
    }


    //Search Area WIse Company methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetAreaList(string prefixText, int count)
    {
        return AutoFillAreaName(prefixText);
    }

    public static List<string> AutoFillAreaName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT Billinglocation FROM [tbl_CompanyMaster] where " + "Billinglocation like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Billinglocation"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }


    protected void txtarea_TextChanged(object sender, EventArgs e)
    {
        if (txtarea.Text != "" || txtarea.Text != null)
        {
            string Area = txtarea.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_CompanyMaster] where Billinglocation = '" + Area + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVCompany.EmptyDataText = "Not Records Found";
            GVCompany.DataSource = dt;
            GVCompany.DataBind();
        }
    }

    protected void GVCompany_RowDataBound(object sender, GridViewRowEventArgs e)
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'CompanyMasterList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate.Visible = false;
                    GVCompany.Columns[8].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
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
                com.CommandText = "SELECT DISTINCT GSTno FROM [tbl_CompanyMaster] where " + "GSTno like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["GSTno"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtGST_TextChanged(object sender, EventArgs e)
    {
        if (txtGST.Text != "" || txtGST.Text != null)
        {
            string GST = txtGST.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_CompanyMaster] where GSTno = '" + GST + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVCompany.EmptyDataText = "Not Records Found";
            GVCompany.DataSource = dt;
            GVCompany.DataBind();
        }
    }

    //Search Supply WIse Company methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetSupplyList(string prefixText, int count)
    {
        return AutoFillSupplyName(prefixText);
    }

    public static List<string> AutoFillSupplyName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT E_inv_Typeof_supply FROM [tbl_CompanyMaster] where " + "E_inv_Typeof_supply like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["E_inv_Typeof_supply"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void txtSupply_TextChanged(object sender, EventArgs e)
    {
        if (txtSupply.Text != "" || txtSupply.Text != null)
        {
            string Supply = txtSupply.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_CompanyMaster] where E_inv_Typeof_supply = '" + Supply + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVCompany.EmptyDataText = "Not Records Found";
            GVCompany.DataSource = dt;
            GVCompany.DataBind();
        }
    }

    //Search Client WIse Company methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetClientList(string prefixText, int count)
    {
        return AutoFillClientName(prefixText);
    }

    public static List<string> AutoFillClientName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT Clienttype FROM [tbl_CompanyMaster] where " + "Clienttype like @Search + '%' and IsDeleted=0";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["Clienttype"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }
    protected void txtclient_TextChanged(object sender, EventArgs e)
    {
        if (txtclient.Text != "" || txtclient.Text != null)
        {
            string client = txtclient.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_CompanyMaster] where Clienttype = '" + client + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVCompany.EmptyDataText = "Not Records Found";
            GVCompany.DataSource = dt;
            GVCompany.DataBind();
        }
    }
}