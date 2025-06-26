using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;

public partial class Admin_Dashboard : System.Web.UI.Page
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

            string financialYear;

            lbltoday.Text = "TODAY(" + DateTime.Now.ToString("dd-MM-yyyy") + ")";
            lblmonth.Text = "MONTH(" + DateTime.Now.ToString("MMMMMMMMMM") + ")";

            if (DateTime.Now.Month >= 4) // From April to December
            {
                financialYear = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Year + 1).ToString();
            }
            else // From January to March
            {
                financialYear = (DateTime.Now.Year - 1).ToString() + "-" + DateTime.Now.Year.ToString();
            }

            lblyear.Text = "YEAR(" + financialYear + ")";
            MeetingReminder();
           // mailsendforCustomer();
            ActiveUserCount(); CompanyCount(); MachineCounts(); VendorCounts(); Quotationlist(); TaxInvoicelist(); Purchaselist(); Receiptlist(); fillgridService();
            FillGrid();
            //today
            TodayEnquiryCount(); TodayQuotationCount(); TodayInvoiceCount(); TodaySampleCount();
            //month
            MonthEnquiryCount(); MonthQuotationCount(); MonthSampleCount(); MonthInvoiceCount();
            //year
            YearEnquiryCount(); YearQuotationCount(); YearSampleCount(); YearInvoiceCount();

            ActiveCount();
            getdetails();
            //if (Session["Designation"].ToString() == "Sales Manager" || Session["Role"].ToString() == "Admin")
            if (Session["Role"].ToString() == "Admin")
            {
                if (!IsPostBack)
                {
                    FillddlYear();
                    FillTargetFilldata();
                    FillTargetdata();
                    divsalestarget.Visible = true;
                    Updatetarget();
                    //  DispatchReminder();//acces for testing
                    // AccountReminder();//acces for testing
                }
            }
            if (Session["Designation"].ToString() == "Sales Engineer ")
            {//show Reminder popup for inventory person OA/Approved invoice reminder
                if (!IsPostBack)
                {
                    DispatchReminder();
                }
            }
            if (Session["Designation"].ToString() == "Accounts")
            {//created invoice pending for approval
                if (!IsPostBack)
                {
                    AccountReminder();
                }
            }

        }

    }
    //----------------------------------------Today---------------------------------------------
    protected void TodayEnquiryCount()
    {

        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND CONVERT(nvarchar(10),regdate,103)=CONVERT(nvarchar(10),GETDATE(),103) ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where sessionname='" + Session["UserCode"].ToString() + "'  AND IsActive=1 AND CONVERT(nvarchar(10),regdate,103)=CONVERT(nvarchar(10),GETDATE(),103) ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }

        lblenquiriesCount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void TodayQuotationCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_QuotationHdr where IsDeleted=0 AND CONVERT(nvarchar(10),CreatedOn,103)=CONVERT(nvarchar(10),GETDATE(),103)", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_QuotationHdr where CreatedBy='" + Session["UserCode"].ToString() + "'  AND IsDeleted=0 AND CONVERT(nvarchar(10),CreatedOn,103)=CONVERT(nvarchar(10),GETDATE(),103)", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        lblQutationCount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void TodayInvoiceCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tblTaxInvoiceHdr where IsDeleted=0 and Status>=2 AND CONVERT(nvarchar(10),CreatedOn,103)=CONVERT(nvarchar(10),GETDATE(),103)", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tblTaxInvoiceHdr where CreatedBy='" + Session["UserCode"].ToString() + "'  AND IsDeleted=0 and Status>=2 AND CONVERT(nvarchar(10),CreatedOn,103)=CONVERT(nvarchar(10),GETDATE(),103)", Cls_Main.Conn);

            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        lblInvoiceCount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void TodaySampleCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND  Sample=1 AND CONVERT(nvarchar(10),regdate,103)=CONVERT(nvarchar(10),GETDATE(),103) ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where sessionname='" + Session["UserCode"].ToString() + "' AND Sample=1   AND IsActive=1 AND CONVERT(nvarchar(10),regdate,103)=CONVERT(nvarchar(10),GETDATE(),103) ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        lblSampleCount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    //-------------------------------------Month---------------------------------------------
    protected void MonthEnquiryCount()
    {

        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND DATEPART(MONTH, regdate)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where sessionname='" + Session["UserCode"].ToString() + "'  AND IsActive=1 AND DATEPART(MONTH, regdate)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);

            count = Convert.ToInt16(cmd.ExecuteScalar());
        }

        lblmonthenquiry.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void MonthQuotationCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_QuotationHdr where IsDeleted=0 AND DATEPART(MONTH, CreatedOn)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {

            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_QuotationHdr where CreatedBy='" + Session["UserCode"].ToString() + "'  AND IsDeleted=0 AND DATEPART(MONTH, CreatedOn)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }

        lblmonthQuotation.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void MonthInvoiceCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tblTaxInvoiceHdr where IsDeleted=0 and Status>=2 AND DATEPART(MONTH, CreatedOn)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tblTaxInvoiceHdr where CreatedBy='" + Session["UserCode"].ToString() + "'  AND IsDeleted=0 and Status>=2 AND DATEPART(MONTH, CreatedOn)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);

            count = Convert.ToInt16(cmd.ExecuteScalar());
        }

        lblmothInvoice.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void MonthSampleCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where Sample=1 AND IsActive=1 AND DATEPART(MONTH, regdate)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where Sample=1 AND sessionname='" + Session["UserCode"].ToString() + "'  AND IsActive=1 AND DATEPART(MONTH, regdate)=DATEPART(MONTH, GETDATE())", Cls_Main.Conn);

            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        lblmonthsample.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    //-------------------------------------Year---------------------------------------------
    protected void YearEnquiryCount()
    {

        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND  regdate >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND regdate < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END; ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {

            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where sessionname='" + Session["UserCode"].ToString() + "'  AND IsActive=1 AND  regdate >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND regdate < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END; ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }

        lblyearenquiry.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void YearQuotationCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_QuotationHdr where IsDeleted=0 AND  CreatedOn >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND CreatedOn < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_QuotationHdr where CreatedBy='" + Session["UserCode"].ToString() + "'  AND IsDeleted=0 AND  CreatedOn >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND CreatedOn < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END", Cls_Main.Conn);

            count = Convert.ToInt16(cmd.ExecuteScalar());
        }


        lblyearquotation.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void YearInvoiceCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tblTaxInvoiceHdr where IsDeleted=0 and Status>=2 AND  CreatedOn >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND CreatedOn < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tblTaxInvoiceHdr where CreatedBy='" + Session["UserCode"].ToString() + "'  AND IsDeleted=0 and Status>=2 AND  CreatedOn >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND CreatedOn < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }


        lblyearinvoice.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    protected void YearSampleCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND Sample=1 AND  regdate >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND regdate < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END; ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {

            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where sessionname='" + Session["UserCode"].ToString() + "'  AND IsActive=1 AND Sample=1 AND  regdate >= CASE WHEN MONTH(GETDATE()) >= 4 THEN CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE) ELSE CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE) END AND regdate < CASE WHEN MONTH(GETDATE()) >= 4 THEN DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) AS VARCHAR) + '-04-01' AS DATE))) ELSE DATEADD(DAY, -1, DATEADD(YEAR, 1, CAST(CAST(YEAR(GETDATE()) - 1 AS VARCHAR) + '-04-01' AS DATE))) END; ", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }


        lblyearsample.Text = count.ToString();
        Cls_Main.Conn_Close();
    }
    //-----------------------------------------------------------------------------------------
    protected void ActiveUserCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM [tbl_UserMaster] where [IsDeleted]='0' AND Status='1'", Cls_Main.Conn);
        count = Convert.ToInt16(cmd.ExecuteScalar());
        lbluserscount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }

    protected void CompanyCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM [tbl_CompanyMaster] where [IsDeleted]='0' ", Cls_Main.Conn);
        count = Convert.ToInt16(cmd.ExecuteScalar());
        lblcompanycount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }

    protected void MachineCounts()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM [tbl_ProductMaster] where [IsDeleted]='0' ", Cls_Main.Conn);
        count = Convert.ToInt16(cmd.ExecuteScalar());
        lblProductCount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }

    protected void VendorCounts()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM [tbl_VendorMaster] where [IsDeleted]='0' AND Status = '1' ", Cls_Main.Conn);
        count = Convert.ToInt16(cmd.ExecuteScalar());
        lblvendorcount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }

    protected void ActiveCount()
    {
        Cls_Main.Conn_Open();
        int count = 0;
        if (Session["Role"].ToString() == "Admin")
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND Sample=1 AND Notiification=0  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE);", Cls_Main.Conn);
            count = Convert.ToInt16(cmd.ExecuteScalar());
        }
        else
        {
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where sessionname='" + Session["UserCode"].ToString() + "'  AND IsActive=1 AND Sample=1 AND Notiification=0  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE);", Cls_Main.Conn);

            count = Convert.ToInt16(cmd.ExecuteScalar());
        }


        lblcount.Text = count.ToString();
        Cls_Main.Conn_Close();
    }

    public void getdetails()
    {
        DataTable dt = new DataTable();
        if (Session["Role"].ToString() == "Admin")
        {
            SqlDataAdapter sad = new SqlDataAdapter("select id, cname,EnqCode,SampleDate from tbl_EnquiryData where IsActive=1 AND Sample=1 AND Notiification=0  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE)", Cls_Main.Conn);
            sad.Fill(dt);
        }
        else
        {
            SqlDataAdapter sad = new SqlDataAdapter("select id, cname,EnqCode,SampleDate from tbl_EnquiryData where IsActive=1 AND Sample=1 AND Notiification=0 AND  sessionname='" + Session["UserCode"].ToString() + "' AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE)", Cls_Main.Conn);
            sad.Fill(dt);
        }

        if (dt.Rows.Count > 0)
        {
            grdnotification.DataSource = dt;
            grdnotification.DataBind();
        }
    }

    // ------------------------------------------------------------------------------------------

    public void fillgridPurchase()
    {
        Cls_Main.Conn_Open();
        DataTable Dt = Cls_Main.Read_Table("SELECT Top 5  ProductName, SUM(Qty) AS PurchasedProducts FROM[tblInwardProduct] where IsDeleted = '0' GROUP BY ProductName ORDER BY 2 DESC");
        if (Dt.Rows.Count > 0)
        {
            gvQuotation.DataSource = Dt;
            gvQuotation.DataBind();
        }
    }

    public void fillgridSale()
    {
        Cls_Main.Conn_Open();
        DataTable Dt = Cls_Main.Read_Table("SELECT Top 5  SalesProductName, SUM(Quntity) AS SaleProducts FROM tblSellproduct where IsDeleted = '0' GROUP BY SalesProductName ORDER BY 2 DESC");
        if (Dt.Rows.Count > 0)
        {
            gvInvoice.DataSource = Dt;
            gvInvoice.DataBind();
        }
    }

    protected void Quotationlist()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select TOP 5 * FROM [tbl_QuotationHdr] where Convert(char(10), CreatedOn, 120) = Convert(date, getdate()) AND IsDeleted = '0'", Cls_Main.Conn);
        sad.Fill(dt);
        gvQuotation.EmptyDataText = " Records Not Found ";
        gvQuotation.DataSource = dt;
        gvQuotation.DataBind();
        Cls_Main.Conn.Close();
    }

    protected void TaxInvoicelist()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * FROM [tblTaxInvoiceHdr] where Convert(char(10), CreatedOn, 120) = Convert(date, getdate()) AND IsDeleted = '0'", Cls_Main.Conn);
        sad.Fill(dt);
        gvInvoice.EmptyDataText = " Records Not Found ";
        gvInvoice.DataSource = dt;
        gvInvoice.DataBind();
        Cls_Main.Conn.Close();
    }

    protected void Purchaselist()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * FROM [tbl_PurchaseOrderHdr] where Convert(char(10), CreatedOn, 120) = Convert(date, getdate()) AND IsDeleted = '0'", Cls_Main.Conn);
        sad.Fill(dt);
        GVPurchase.EmptyDataText = " Records Not Found ";
        GVPurchase.DataSource = dt;
        GVPurchase.DataBind();
        Cls_Main.Conn.Close();
    }

    protected void Receiptlist()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select * FROM [tbl_ReceiptVaucher] where Convert(char(10), CreatedOn, 120) = Convert(date, getdate()) AND IsDeleted = '0'", Cls_Main.Conn);
        sad.Fill(dt);
        GVReceipt.EmptyDataText = " Records Not Found ";
        GVReceipt.DataSource = dt;
        GVReceipt.DataBind();
        Cls_Main.Conn.Close();
    }

    public void fillgridService()
    {
        DataTable dt = new DataTable();
        if (Session["Role"].ToString() == "Admin")
        {
            SqlDataAdapter sad = new SqlDataAdapter("select CONVERT(nvarchar(10),CR.CreatedOn,103) AS CreatedOn, case when CR.Status=2 then 'Closed' else 'Open' END AS Action, * from tbl_ComplaintRegister AS CR inner JOIN tbl_UserMaster AS UM on UM.UserCode=CR.CreatedBy where CR.Isdeleted=0 order by CR.CreatedOn desc", Cls_Main.Conn);
            sad.Fill(dt);
        }
        else
        {
            SqlDataAdapter sad = new SqlDataAdapter("select CONVERT(nvarchar(10),CR.CreatedOn,103) AS CreatedOn, case when CR.Status=2 then 'Closed' else 'Open' END AS Action, * from tbl_ComplaintRegister AS CR inner JOIN tbl_UserMaster AS UM on UM.UserCode=CR.CreatedBy where CR.Isdeleted=0  and SEEmailID='" + Session["EmailID"].ToString() + "' order by CR.CreatedOn desc", Cls_Main.Conn);
            sad.Fill(dt);
        }

        if (dt.Rows.Count > 0)
        {
            GVServices.DataSource = dt;
            GVServices.DataBind();
        }
    }

    protected void GVServices_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        // int id = Convert.ToInt32(e.CommandArgument.ToString());
        if (e.CommandName == "Ticketno")
        {
            Response.Redirect("Service_Reports.aspx?Id=" + objcls.encrypt(e.CommandArgument.ToString()));
        }
        if (e.CommandName == "RowDelete")
        {
            Cls_Main.Conn_Open();
            SqlCommand Cmd = new SqlCommand("UPDATE [tbl_ComplaintRegister] SET Isdeleted=@IsDeleted,DeletedBy=@DeletedBy,DeletedOn=@DeletedOn WHERE TicketNo=@ID", Cls_Main.Conn);
            Cmd.Parameters.AddWithValue("@ID", e.CommandArgument.ToString());
            Cmd.Parameters.AddWithValue("@IsDeleted", '1');
            Cmd.Parameters.AddWithValue("@DeletedBy", Session["UserCode"].ToString());
            Cmd.Parameters.AddWithValue("@DeletedOn", DateTime.Now);
            Cmd.ExecuteNonQuery();
            Cls_Main.Conn_Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Complaint Deleted Successfully..!!')", true);
            Response.Redirect(Request.Url.AbsoluteUri);

        }
        if (e.CommandName == "RowView")
        {
            Response.Redirect("Pdf_ServiceReport.aspx?ID=" + objcls.encrypt(e.CommandArgument.ToString()) + "");
        }
    }

    byte[] bytePdfRep = null;
    private void mailsendforCustomer()
    {
        try
        {

            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("[SP_Reminders]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Action", "GetDashboardReminder"));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        foreach (DataRow obj in Dt.Rows)
                        {

                            var invoiceno = obj.ItemArray[0];
                            // PDF(Convert.ToString(invoiceno)); that code removed on 19102024
                            var MailTO = obj.ItemArray[6];
                            string stringInvoiceNo = Convert.ToString(invoiceno).Replace("/", "-");
                            string fileName = stringInvoiceNo + "_CustomerTaxInvoice.pdf";
                            var fromMailID = obj.ItemArray[14].ToString();
                           // var fromMailID = "testing@weblinkservices.net";
                            string mailTo = MailTO.ToString();
                            MailMessage mm = new MailMessage();
                            //string strMessage = "Apologies for the confusion caused by the previous email. It was sent in error, and we would like to clarify that the earlier message regarding the ERP system testing was meant as part of our regular testing process.\r\n\r\nPlease note that the data shared may not reflect the final figures as it is part of an ongoing test. We appreciate your understanding as we continue to ensure everything is functioning correctly.\r\n\r\nThank you for your attention and cooperation, and please feel free to reach out if you have any questions.";
                           // mm.From = new MailAddress("girish.kulkarni@puneabrasives.com", fromMailID);
                            string strMessage = "Invoice No.:" + invoiceno + "<br/>" +
                                "Invoice Date:" + obj.ItemArray[1] + "<br/>" +
                                "Grand total :" + obj.ItemArray[7] + "<br/>" +
                                "Received :" + obj.ItemArray[8] + "<br/>" +
                                "Pending :" + obj.ItemArray[10] + "<br/>" +
                "We sent you an Invoice." + fileName + "<br/>" +

                "Please find herewith attached Invoice for your reference." + "<br/>" +

                "Looking forward to your valuable reply with Invoice." + "<br/>" +

                "Feel free to contact us for any further queries & Clarifications." + "<br/>" +

                "Kind Regards," + "<br/>" +
                "<strong> PUNE ABRASIVES PVT. LTD.<strong>";
                            mm.Subject = " - UPDATE - FROM PUNE ABRASIVES PVT. LTD.";
                            mm.To.Add(mailTo);
                           // mm.To.Add("erpdeveloper3003@gmail.com");

                            mm.CC.Add("girish.kulkarni@puneabrasives.com");
                            mm.CC.Add(fromMailID);
                            EInvoiceReports(Convert.ToString(invoiceno));
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                byte[] file = bytePdfRep;

                                Stream stream = new MemoryStream(file);
                                Attachment aa = new Attachment(stream, fileName);
                                 mm.Attachments.Add(aa);
                            }
                            StreamReader reader = new StreamReader(Server.MapPath("~/Templates/CommentPage_templet.html"));
                            string readFile = reader.ReadToEnd();
                            string myString = "";
                            myString = readFile;

                            string multilineText = strMessage;
                            string formattedText = multilineText.Replace("\n", "<br />");

                            myString = myString.Replace("$Comment$", formattedText);

                            mm.Body = myString.ToString();
                            mm.From = new MailAddress(ConfigurationManager.AppSettings["mailUserName"].ToLower(), fromMailID);
                            mm.IsBodyHtml = true;
                            mm.ReplyToList.Add(new MailAddress(fromMailID));
                            mm.BodyEncoding = Encoding.Default;
                            mm.Priority = MailPriority.High;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = ConfigurationManager.AppSettings["Host"]; ;
                            smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                            NetworkCredential NetworkCred = new NetworkCredential();
                            NetworkCred.UserName = ConfigurationManager.AppSettings["mailUserName"].ToLower();
                            NetworkCred.Password = ConfigurationManager.AppSettings["mailUserPass"].ToString();

                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = NetworkCred;
                            smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
                            {
                                return true;
                            };
                            string date = Convert.ToString(obj.ItemArray[12]);
                            if (date != null && date != "")
                            {
                                DateTime reminderDate = Convert.ToDateTime(date).Date;
                                DateTime Todaydate = DateTime.Now.Date;
                                if (reminderDate.AddDays(2) < Todaydate)
                                {
                                    smtp.SendMailAsync(mm);
                                    Cls_Main.Conn_Open();
                                    SqlCommand Cmd = new SqlCommand("UPDATE [tblTaxInvoiceHdr] SET IsReminderMail=@IsReminderMail,Reminderdate=@Reminderdate WHERE InvoiceNo=@Invoiceno", Cls_Main.Conn);
                                    Cmd.Parameters.AddWithValue("@Invoiceno", invoiceno);
                                    Cmd.Parameters.AddWithValue("@IsReminderMail", '1');
                                    Cmd.Parameters.AddWithValue("@Reminderdate", DateTime.Now);
                                    Cmd.ExecuteNonQuery();
                                    Cls_Main.Conn_Close();
                                    // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('mail send succesfully') ", true);

                                }
                            }
                            else
                            {
                                smtp.SendMailAsync(mm);
                                Cls_Main.Conn_Open();
                                SqlCommand Cmd = new SqlCommand("UPDATE [tblTaxInvoiceHdr] SET IsReminderMail=@IsReminderMail,Reminderdate=@Reminderdate WHERE InvoiceNo=@Invoiceno", Cls_Main.Conn);
                                Cmd.Parameters.AddWithValue("@Invoiceno", invoiceno);
                                Cmd.Parameters.AddWithValue("@IsReminderMail", '1');
                                Cmd.Parameters.AddWithValue("@Reminderdate", DateTime.Now);
                                Cmd.ExecuteNonQuery();
                                Cls_Main.Conn_Close();
                                // ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('mail send succesfully') ", true);

                            }

                        }

                    }
                }
            }


        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('" + errorMsg + "') ", true);
        }
    }

    public string ConvertNumbertoWords(int number)
    {
        if (number == 0)
            return "ZERO";
        if (number < 0)
            return "minus " + ConvertNumbertoWords(Math.Abs(number));
        string words = "";
        if ((number / 1000000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000000) + " Million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += ConvertNumbertoWords(number / 1000) + " Thousand ";
            number %= 1000;
        }
        if ((number / 100) > 0)
        {
            words += ConvertNumbertoWords(number / 100) + " Hundred ";
            number %= 100;
        }
        if (number > 0)
        {
            if (words != "")
                words += "And ";
            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
        }
        return words;
    }

    private void FillGrid()
    {
        try
        {
            string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("[SP_GetProductpendingData]", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    //  cmd.Parameters.Add(new SqlParameter("@Action", "GetCashmasterlist"));
                    //cmd.Parameters.Add(new SqlParameter("@Invoiceno", txtinoiceno.Text));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable Dt = new DataTable();
                    adapter.Fill(Dt);
                    if (Dt.Rows.Count > 0)
                    {
                        grdProductpending.DataSource = Dt;
                        grdProductpending.DataBind();
                    }
                }
                connection.Close();
            }
        }
        catch (Exception ex)
        {

            //throw;
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);

        }

    }

    protected void lnkenquiries_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryList.aspx");
    }

    protected void lnkQutation_Click(object sender, EventArgs e)
    {
        Response.Redirect("QuatationList.aspx");
    }

    protected void lnkInvoice_Click(object sender, EventArgs e)
    {
        Response.Redirect("ApprovedInvoiceList.aspx");
    }

    protected void lnkSample_Click(object sender, EventArgs e)
    {
        Response.Redirect("EnquiryList.aspx");
    }

    protected void lnkvendors_Click(object sender, EventArgs e)
    {

    }

    protected void lblTotalProduct_Click(object sender, EventArgs e)
    {

    }

    protected void lnkcompany_Click(object sender, EventArgs e)
    {

    }

    protected void lnkUsers_Click(object sender, EventArgs e)
    {

    }

    protected void grdnotification_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        int rowCount = grdnotification.Rows.Count;

        // Update the label with the row count
        lblcount.Text = rowCount.ToString();

        // Optionally hide the badge if the count is zero
        if (rowCount == 0)
        {
            notificationCount.Style.Add("display", "none");
        }
        else
        {
            notificationCount.Style.Add("display", "inline-block");
        }
    }

    [WebMethod]
    public static string MarkNotificationsAsSeen()
    {

        //Cls_Main.Conn_Open();
        //SqlCommand Cmd = new SqlCommand("update tbl_EnquiryData set Notiification=1 where IsActive=1 AND  Notiification=0  AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE)", Cls_Main.Conn);
        ////  Cmd.Parameters.AddWithValue("@ID", Convert.ToInt32(e.CommandArgument.ToString()));
        ////  Cmd.Parameters.AddWithValue("@IsDeleted", '1');     
        //Cmd.ExecuteNonQuery();
        //Cls_Main.Conn_Close();

        return "success";
    }
    protected void MeetingReminder()
    {

        try
        {
            DataTable dt = new DataTable();
            if (Session["Role"].ToString() == "Admin")
            {
                SqlDataAdapter cmd = new SqlDataAdapter("select * from tbl_CallandMeetingDetails where Updatefor='Meeting' AND CONVERT(VARCHAR(10), CAST(followupdate AS DATETIME), 23) = CONVERT(VARCHAR(10), GETDATE(), 23)", Cls_Main.Conn);
                cmd.Fill(dt);
            }
            else
            {
                SqlDataAdapter cmd = new SqlDataAdapter("select * from tbl_CallandMeetingDetails where Updatefor='Meeting' AND  CreatedBy='" + Session["UserCode"].ToString() + "' AND CONVERT(VARCHAR(10), CAST(followupdate AS DATETIME), 23) = CONVERT(VARCHAR(10), GETDATE(), 23);", Cls_Main.Conn);
                cmd.Fill(dt);
            }
            if (dt.Rows.Count > 0)
            {
                grdreminder.DataSource = dt;
                grdreminder.DataBind();
                this.ModalPopupHistory.Show();
            }
            else
            {
                this.ModalPopupHistory.Hide();
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void grdreminder_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdreminder.PageIndex = e.NewPageIndex;
        MeetingReminder();
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        Response.Redirect("CallandMettingReport.aspx");
    }

    public void sendmail()
    {
        string smtpAddress = "us2.smtp.mailhostbox.com"; // Use the correct SMTP server address
        int portNumber = 25; // Typically 587 or 25 for SMTP
        bool enableSSL = true;

        string emailFrom = "girish.kulkarni@puneabrasives.com";
        string password = "Qi#dKZN1"; // Use secure storage for passwords in real applications
        string emailTo = "shubhpawar59@gmail.com";
        string subject = "Test Email";
        string body = "This is a test email sent from a .NET application.";

        using (MailMessage mail = new MailMessage())
        {
            mail.From = new MailAddress(emailFrom);
            mail.To.Add(emailTo);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = false; // Set to true if the body contains HTML

            using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
            {
                smtp.Credentials = new NetworkCredential(emailFrom, password);
                smtp.EnableSsl = enableSSL;
                try
                {
                    smtp.Send(mail);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: {ex.Message}");
                }
            }
        }
    }
    //------------------------------------Target-------------------------------------------------
    public void FillddlYear()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select DISTINCT Year from TargetMaster where IsDeleted=0", Cls_Main.Conn);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlYear.DataSource = dt;
            ddlYear.DataTextField = "Year";
            ddlYear.DataBind();

            int month = DateTime.Now.Month;
            ddlMonth.SelectedValue = Convert.ToString(month);

        }
    }
    public void FillTargetFilldata()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select Amount,Quantity from TargetMaster where IsDeleted=0 AND Year='" + ddlYear.SelectedValue + "' AND Month='" + ddlMonth.SelectedItem.Text + "'", Cls_Main.Conn);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblamount.Text = dt.Rows[0]["Amount"].ToString();
            lbltarget.Text = dt.Rows[0]["Quantity"].ToString();
        }
        else
        {
            lblamount.Text = "0";
            lbltarget.Text = "0";
        }
    }
    public void FillTargetdata()
    {
        DataTable dt = new DataTable();
        SqlDataAdapter sad = new SqlDataAdapter("select sum(CAST(Quantity AS float)) AS Quantity ,sum(CAST(Total AS float)) AS Alltotal from tbl_OutwardEntryDtls where  MONTH(CreatedOn)='" + ddlMonth.SelectedValue + "'", Cls_Main.Conn);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblAmtCompelete.Text = dt.Rows[0]["Alltotal"].ToString();
            lblQuantityCmpl.Text = dt.Rows[0]["Quantity"].ToString();
            if (lblAmtCompelete.Text != "" && lblQuantityCmpl.Text != "")
            {
                if (Convert.ToDouble(lblamount.Text) <= Convert.ToDouble(lblAmtCompelete.Text))
                {
                    lblAmtCompelete.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblAmtCompelete.ForeColor = System.Drawing.Color.Red;
                }
                if (Convert.ToDouble(lbltarget.Text) <= Convert.ToDouble(lblQuantityCmpl.Text))
                {
                    lblQuantityCmpl.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lblQuantityCmpl.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblAmtCompelete.Text = "0";
                lblQuantityCmpl.Text = "0";
            }

        }
        else
        {
            lblAmtCompelete.Text = "0";
            lblQuantityCmpl.Text = "0";
        }
    }

    protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        FillTargetFilldata();
        FillTargetdata();
    }
    public void Updatetarget()
    {
        try
        {
            int year = DateTime.Now.Year;
            DataTable dt = new DataTable();
            SqlDataAdapter sad = new SqlDataAdapter("select sum(CAST(Quantity AS float)) AS Quantity ,sum(CAST(Alltotal AS float)) AS Alltotal from tbl_OutwardEntryDtls where  MONTH(CreatedOn)='" + ddlMonth.SelectedValue + "'", Cls_Main.Conn);
            sad.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                string lblAmtCompelete = dt.Rows[0]["Alltotal"].ToString();
                string lblQuantityCmpl = dt.Rows[0]["Quantity"].ToString();

                Cls_Main.Conn_Open();
                SqlCommand Cmd = new SqlCommand("UPDATE [TargetMaster] SET AchivedAmount=@Alltotal, AchivedQuantity=@Quantity WHERE Year=@Year AND Month=@Month", Cls_Main.Conn);
                Cmd.Parameters.AddWithValue("@Year", ddlYear.SelectedItem.Text);
                Cmd.Parameters.AddWithValue("@Month", ddlMonth.SelectedItem.Text);
                Cmd.Parameters.AddWithValue("@Alltotal", lblAmtCompelete);
                Cmd.Parameters.AddWithValue("@Quantity", lblQuantityCmpl);

                Cmd.ExecuteNonQuery();
                Cls_Main.Conn_Close();

            }

        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }
    //-----------------------------------------------------------------------------------------

    public void DispatchReminder()
    {
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter cmd = new SqlDataAdapter("select Pono,PoDate,CustomerName,UM.Username from tbl_CustomerPurchaseOrderHdr AS CP INNER JOIN tbl_UserMaster AS UM ON UM.UserCode=CP.UserName WHERE PoDate BETWEEN CAST(DATEADD(DAY, -3, GETDATE()) AS DATE) AND CAST(GETDATE() AS DATE) AND Pono NOT IN (SELECT AgainstNumber FROM tblTaxInvoiceHdr where isdeleted=0) and CP.IsDeleted=0  ORDER BY CP.CreatedOn DESC", Cls_Main.Conn);
            cmd.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                GVOAreminder.DataSource = dt;
                GVOAreminder.DataBind();
                oadetails.Visible = true;
                LinkButton7.Visible = true;
            }


            DataTable dt1 = new DataTable();
            SqlDataAdapter cmd1 = new SqlDataAdapter("select * from tblTaxInvoiceHdr where Status=2  AND Invoicedate BETWEEN CAST(DATEADD(DAY, -3, GETDATE()) AS DATE) AND CAST(GETDATE() AS DATE) ORDER BY CreatedOn DESC", Cls_Main.Conn);
            cmd1.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                GVInvoiceList.DataSource = dt1;
                GVInvoiceList.DataBind();
                taxinvoice.Visible = true;
                LinkButton9.Visible = true;
            }

            if (dt.Rows.Count > 0 || dt1.Rows.Count > 0)
            {
                this.ModalPopupExtender1.Show();
            }
            else
            {
                this.ModalPopupExtender1.Hide();
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    public void AccountReminder()
    {
        try
        {
            DataTable dt = new DataTable();
            SqlDataAdapter cmd = new SqlDataAdapter("select * from tblTaxInvoiceHdr where Status=1  AND Invoicedate BETWEEN CAST(DATEADD(DAY, -3, GETDATE()) AS DATE) AND CAST(GETDATE() AS DATE) ORDER BY CreatedOn DESC", Cls_Main.Conn);
            cmd.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                GVInvoiceList.DataSource = dt;
                GVInvoiceList.DataBind();
                this.ModalPopupExtender1.Show();
                taxinvoice.Visible = true;
                LinkButton9.Visible = true;
            }
            else
            {
                this.ModalPopupExtender1.Hide();
            }
        }
        catch (Exception ex)
        {
            string errorMsg = "An error occurred : " + ex.Message;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + errorMsg + "');", true);
        }
    }

    protected void LinkButton9_Click(object sender, EventArgs e)
    {
        if (Session["Designation"].ToString() == "Accounts")
        {
            Response.Redirect("../Account/ApprovedInvoiceList.aspx");
        }
        else
        {
            Response.Redirect("../Account/TaxInvoiceList.aspx");
        }

    }

    protected void LinkButton7_Click(object sender, EventArgs e)
    {

        Response.Redirect("../Admin/OAListForWareHouse.aspx");
    }


    public void EInvoiceReports(string Invoiceno)
    {
        byte[] imageBytes = null;
        try
        {
            DataTable dt1 = new DataTable();

            SqlDataAdapter sad = new SqlDataAdapter("SELECT top 1 id FROM tblTaxinvoicehdr where isdeleted=0 and invoiceno='" + Invoiceno + "'", Cls_Main.Conn);
            sad.Fill(dt1);
            string idtax = dt1.Rows[0]["id"].ToString();
            bytePdfRep = null;
            DataSet Dtt = new DataSet();
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                using (SqlCommand cmd = new SqlCommand("[SP_Reports]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", "GetE_InvoiceDetails");
                    cmd.Parameters.AddWithValue("@Invoiceno", idtax);

                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = con;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(Dtt);

                        }
                    }
                }
            }

            if (Dtt.Tables.Count > 0)
            {
                if (Dtt.Tables[0].Rows.Count > 0)
                {
                    // Genrate Qr Code For E-Invoice RDLC
                    con.Open();
                    string id = idtax;
                    SqlCommand cmdQrdtl = new SqlCommand("select signedQRCode from tbltaxinvoicehdr where Id='" + id + "'", con);
                    Object Qrdtl = cmdQrdtl.ExecuteScalar();
                    string F_Qrdtl = Convert.ToString(Qrdtl);
                    con.Close();
                    if (F_Qrdtl != "")
                    {
                        con.Open();
                        SqlCommand cmdAckdtl = new SqlCommand("select AckNo from tbltaxinvoicehdr where Id='" + id + "'", con);
                        Object ACkdtl = cmdAckdtl.ExecuteScalar();
                        string F_Ackdtl = Convert.ToString(ACkdtl);
                        con.Close();
                        imageBytes = GenerateQR(F_Qrdtl);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('E-Invoice Not Created. Kindly Create First..!!');", true);
                    }

                    DataTable dt = new DataTable();
                    dt.Columns.Add("QRCodeImage", typeof(byte[]));
                    dt.Columns.Add("Type", typeof(string));
                    DataRow row = dt.NewRow();

                    row["QRCodeImage"] = imageBytes;
                    row["Type"] = "(ORIGINAL FOR RECIPIENT)";
                    dt.Rows.Add(row);

                    // DataTable Dt = Cls_Main.Read_Table("SELECT *,EmailID AS Email,Username AS Name FROM tbl_UserMaster  where UserCode='" + Session["UserCode"] + "'");
                    ReportDataSource obj1 = new ReportDataSource("DataSet1", Dtt.Tables[0]);
                    ReportDataSource obj2 = new ReportDataSource("DataSet2", Dtt.Tables[1]);
                    ReportDataSource obj3 = new ReportDataSource("DataSet3", Dtt.Tables[2]);
                    ReportDataSource obj4 = new ReportDataSource("DataSet4", dt);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(obj1);
                    ReportViewer1.LocalReport.DataSources.Add(obj2);
                    ReportViewer1.LocalReport.DataSources.Add(obj3);
                    ReportViewer1.LocalReport.DataSources.Add(obj4);

                    string e_invoice_cancel_status = Dtt.Tables[0].Rows[0]["e_invoice_cancel_status"].ToString();
                    if (e_invoice_cancel_status == true.ToString())
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\CancelEInvoice.rdlc";
                    }
                    else
                    {
                        ReportViewer1.LocalReport.ReportPath = "RDLC_Reports\\EInvoice.rdlc";
                    }                  
                    ReportViewer1.LocalReport.Refresh();
                    //-------- Print PDF directly without showing ReportViewer ----
                    Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;
                    bytePdfRep = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);



                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Not Found...........!')", true);
                }
            }
        }
        catch (Exception ex)
        {
            throw (ex);
        }
    }

    private byte[] GenerateQR(string QR_String)
    {
        byte[] QrBytes = null;
        var writer = new BarcodeWriter();
        writer.Format = BarcodeFormat.QR_CODE;
        var result = writer.Write(QR_String);
        string path = Server.MapPath("~/E_Inv_QrCOde/QR_Img.jpg");
        var barcodeBitmap = new System.Drawing.Bitmap(result);

        using (MemoryStream memory = new MemoryStream())
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {

                barcodeBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Jpeg);
                QrBytes = memory.ToArray();
                fs.Write(QrBytes, 0, QrBytes.Length);
            }
        }
        //img_QrCode.Visible = true;
        //img_QrCode.ImageUrl = "~/E_Inv_QrCOde/QR_Img.jpg";
        return QrBytes;
    }

}