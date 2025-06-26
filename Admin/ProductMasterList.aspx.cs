
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


public partial class Admin_ProductMasterList : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
           // Fillddlbrand();
       
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
        DataTable Dt = Cls_Main.Read_Table("SELECT * FROM [tbl_ProductMaster] WHERE IsDeleted = 0 ORDER BY ID DESC");
        GVProductlist.DataSource = Dt;
        GVProductlist.DataBind();
    }
    
    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProductMaster.aspx");
    }

    protected void GVProductlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("ProductMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_ProductMaster] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Product Deleted Successfully..!!')", true);
            FillGrid();
        }
    }

    protected void GVProductlist_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVProductlist.PageIndex = e.NewPageIndex;
        FillGrid();
    }

    protected void GVProductlist_RowDataBound(object sender, GridViewRowEventArgs e)
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'ProductMasterList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate.Visible = false;
                    GVProductlist.Columns[8].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }



    protected void btnrefresh_Click(object sender, EventArgs e)
    {
        Response.Redirect("ProductMasterList.aspx");
    }
    //Search Company Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetProductList(string prefixText, int count)
    {
        return AutoFillProductName(prefixText);
    }

    public static List<string> AutoFillProductName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [ProductName] FROM [tbl_ProductMaster] where " + "ProductName like @Search + '%' and IsDeleted=0 AND Status = '1'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["ProductName"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtProduct_TextChanged(object sender, EventArgs e)
    {
        if (txtProduct.Text != "" || txtProduct.Text != null)
        {
            string Product = txtProduct.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ProductMaster] where ProductName = '" + Product + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVProductlist.EmptyDataText = "Not Records Found";
            GVProductlist.DataSource = dt;
            GVProductlist.DataBind();
        }
    }

    //Search Company Search methods
    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetHSTList(string prefixText, int count)
    {
        return AutoFillHSNName(prefixText);
    }

    public static List<string> AutoFillHSNName(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "SELECT DISTINCT [HSN] FROM [tbl_ProductMaster] where " + "HSN like @Search + '%' and IsDeleted=0 AND Status = '1'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> countryNames = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        countryNames.Add(sdr["HSN"].ToString());
                    }
                }
                con.Close();
                return countryNames;
            }
        }
    }

    protected void txtHSN_TextChanged(object sender, EventArgs e)
    {
        if (txtHSN.Text != "" || txtHSN.Text != null)
        {
            string HSN = txtHSN.Text;

            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select * from [tbl_ProductMaster] where HSN = '" + HSN + "' AND IsDeleted = 0", Cls_Main.Conn);
            sad.Fill(dt);
            GVProductlist.EmptyDataText = "Not Records Found";
            GVProductlist.DataSource = dt;
            GVProductlist.DataBind();
        }
    }
}