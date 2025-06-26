
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


public partial class Admin_VendorMasterList : System.Web.UI.Page
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
        int pageSize = 10; // default fallback
        int.TryParse(ddlPageSize.SelectedValue, out pageSize);
        DataTable Dt = Cls_Main.Read_Table("SELECT  TOP (" + pageSize + ") * FROM [tbl_VendorMaster] WHERE IsDeleted = 0");
        GVVendor.DataSource = Dt;
        GVVendor.DataBind();
    }

    protected void btnCreate_Click(object sender, EventArgs e)
    {
        Response.Redirect("VendorMaster.aspx");
    }

    protected void GVVendor_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "RowEdit")
        {
            Response.Redirect("VendorMaster.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_VendorMaster] SET IsDeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE ID=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('User Deleted Successfully..!!')", true);
            FillGrid();
        }
    }

    protected void GVVendor_RowDataBound(object sender, GridViewRowEventArgs e)
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
                SqlDataAdapter Sdd = new SqlDataAdapter("Select * FROM tblUserRoleAuthorization where UserID = '" + id + "' AND PageName = 'VendorMasterList.aspx' AND PagesView = '1'", con);
                Sdd.Fill(Dtt);
                if (Dtt.Rows.Count > 0)
                {
                    btnCreate.Visible = false;
                    GVVendor.Columns[7].Visible = false;
                    btnEdit.Visible = false;
                    btnDelete.Visible = false;
                }
            }
        }
    }

    protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillGrid();
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetPartyList(string prefixText, int count)
    {
        return AutoFillPartylist(prefixText);
    }

    public static List<string> AutoFillPartylist(string prefixText)
    {
        using (SqlConnection con = new SqlConnection())
        {
            con.ConnectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlCommand com = new SqlCommand())
            {
                com.CommandText = "select DISTINCT Vendorname from tbl_VendorMaster where " + "Vendorname like @Search + '%' AND isdeleted='0'";

                com.Parameters.AddWithValue("@Search", prefixText);
                com.Connection = con;
                con.Open();
                List<string> Partyname = new List<string>();
                using (SqlDataReader sdr = com.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        Partyname.Add(sdr["Vendorname"].ToString());
                    }
                }
                con.Close();
                return Partyname;
            }

        }
    }

    protected void txtpartyname_TextChanged(object sender, EventArgs e)
    {
        try
        {
            int pageSize = 10; // default fallback
            int.TryParse(ddlPageSize.SelectedValue, out pageSize);
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select TOP (" + pageSize + ")  * from tbl_VendorMaster where Vendorname='" + txtpartyname.Text + "' AND isdeleted='0'", con);
            sad.Fill(dt);
            GVVendor.DataSource = dt;
            GVVendor.DataBind();
            GVVendor.EmptyDataText = "Record Not Found";

        }
        catch (Exception)
        {

            throw;
        }
    }
    protected void btnresetfilter_Click(object sender, EventArgs e)
    {
        Response.Redirect("VendorMasterList.aspx");
    }

}