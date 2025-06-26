using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Account_AccountMasterPage : System.Web.UI.MasterPage
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Username"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            //ActiveCount();
            //getdetails();
            //01/10/2021

            // lblusername.Text = Session["Username"].ToString();


            PageAuthorization();


            lblusername.Text = Session["Username"].ToString();
        }
    }

    protected void adminDashboard_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Admin/Dashboard.aspx");
    }
    protected void PageAuthorization()
    {
        string username = Session["ID"].ToString();
        DataTable dt = new DataTable();
        SqlCommand cmd1 = new SqlCommand("SELECT * FROM [tblUserRoleAuthorization] where UserID='" + username + "'", con);
        SqlDataAdapter sad = new SqlDataAdapter(cmd1);
        sad.Fill(dt);
        if (dt.Rows.Count > 0)
        {

            foreach (DataRow row in dt.Rows)
            {
                string MenuName = row["PageName"].ToString();
                //Masters
                if (MenuName == "UserMasterList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    UserListid.Visible = page1 == "True" ? true : false;
                }
                if (MenuName == "CompanyMasterList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        CompanyListid.Visible = false;
                    }
                    else
                    {
                        CompanyListid.Visible = true;
                    }
                }
                if (MenuName == "ComponentList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        ComponentID.Visible = false;
                    }
                    else
                    {
                        ComponentID.Visible = true;
                    }

                }
                if (MenuName == "RoleList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        RolelistID.Visible = false;
                    }
                    else
                    {
                        RolelistID.Visible = true;
                    }
                }
                if (MenuName == "ProductMasterList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        ProductMasterListID.Visible = false;
                    }
                    else
                    {
                        ProductMasterListID.Visible = true;
                    }
                }
                if (MenuName == "VendorMasterList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        VendorMasterListid.Visible = false;
                    }
                    else
                    {
                        VendorMasterListid.Visible = true;
                    }
                }
                if (MenuName == "TransporterList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        TrasnportermasterID.Visible = false;
                    }
                    else
                    {
                        TrasnportermasterID.Visible = true;
                    }
                }
                if (MenuName == "TargetList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        TargetMasterID.Visible = false;
                    }
                    else
                    {
                        TargetMasterID.Visible = true;
                    }
                }
                if (MenuName == "SalesTargetList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        SalesTargetID.Visible = false;
                    }
                    else
                    {
                        SalesTargetID.Visible = true;
                    }
                }

                //Sales section
                if (MenuName == "EnquiryList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        EnquiryID.Visible = false;
                    }
                    else
                    {
                        EnquiryID.Visible = true;
                    }
                }
                if (MenuName == "QuatationList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        GeneralQuotationListID.Visible = false;
                    }
                    else
                    {
                        GeneralQuotationListID.Visible = true;
                    }
                }
                if (MenuName == "CustomerPurchaseOrderList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        POCustomerID.Visible = false;
                    }
                    else
                    {
                        POCustomerID.Visible = true;
                    }
                }
                if (MenuName == "ProformaInvoiceList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        ProfarmaID.Visible = false;
                    }
                    else
                    {
                        ProfarmaID.Visible = true;
                    }
                }
                if (MenuName == "CallandMettingReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        CallandMeetingListID.Visible = false;
                    }
                    else
                    {
                        CallandMeetingListID.Visible = true;
                    }
                }
                if (MenuName == "DSRReports.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        DSRReports.Visible = false;
                    }
                    else
                    {
                        DSRReports.Visible = true;
                    }
                }
                //Account section
                if (MenuName == "TaxInvoiceList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        TaxInvoiceListid.Visible = false;
                    }
                    else
                    {
                        TaxInvoiceListid.Visible = true;
                    }
                }
                if (MenuName == "ApprovedInvoiceList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        ApprovedInvoiceListID.Visible = false;
                    }
                    else
                    {
                        ApprovedInvoiceListID.Visible = true;
                    }
                }
                if (MenuName == "ReceiptList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        ReceiptVoucherID.Visible = false;
                    }
                    else
                    {
                        ReceiptVoucherID.Visible = true;
                    }
                }
                if (MenuName == "CreditDebitNoteSaleList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        CreditDebitNoteSaleID.Visible = false;
                    }
                    else
                    {
                        CreditDebitNoteSaleID.Visible = true;
                    }
                }

                //Purchase Section
                if (MenuName == "PurchaseOrderList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        PurchaseOrderID.Visible = false;
                    }
                    else
                    {
                        PurchaseOrderID.Visible = true;
                    }
                }
                if (MenuName == "PurchaseBillList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        PurchaseBillID.Visible = false;
                    }
                    else
                    {
                        PurchaseBillID.Visible = true;
                    }
                }
                if (MenuName == "PaymentList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        PaymentVoucherID.Visible = false;
                    }
                    else
                    {
                        PaymentVoucherID.Visible = true;
                    }
                }
                if (MenuName == "CreditDebitNoteList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        CreditDebitNotePurchaseID.Visible = false;
                    }
                    else
                    {
                        CreditDebitNotePurchaseID.Visible = true;
                    }
                }
                //Stock Panel
                if (MenuName == "InventoryList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        InventryID.Visible = false;
                    }
                    else
                    {
                        InventryID.Visible = true;
                    }
                }
                if (MenuName == "InwardEntryList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        EnwardEntryID.Visible = false;
                    }
                    else
                    {
                        EnwardEntryID.Visible = true;
                    }
                }
                if (MenuName == "OutwardEntryList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        OutwardEntryID.Visible = false;
                    }
                    else
                    {
                        OutwardEntryID.Visible = true;
                    }
                }
                if (MenuName == "WarehouseInvoiceList.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        WarehouseInvoiceListID.Visible = false;
                    }
                    else
                    {
                        WarehouseInvoiceListID.Visible = true;
                    }
                }
                if (MenuName == "OAListForWarehouse.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        oaforwarehouseID.Visible = false;
                    }
                    else
                    {
                        oaforwarehouseID.Visible = true;
                    }
                }
                //Reports
                if (MenuName == "SalesReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        SalesReportID.Visible = false;
                    }
                    else
                    {
                        SalesReportID.Visible = true;
                    }
                }
                if (MenuName == "PurchaseReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        PurchaseReportID.Visible = false;
                    }
                    else
                    {
                        PurchaseReportID.Visible = true;
                    }
                }
                //if (MenuName == "InventoryForAccount.aspx")
                //{
                //    string page1 = row["Pages"].ToString();
                //    string pageView = row["PagesView"].ToString();
                //    if (page1 == "False" && pageView == "False")
                //    {
                //        InventoryForAccountID.Visible = false;
                //    }
                //    else
                //    {
                //        InventoryForAccountID.Visible = true;
                //    }
                //}
                if (MenuName == "PartyLedgerReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        PartyLedgerReportID.Visible = false;
                    }
                    else
                    {
                        PartyLedgerReportID.Visible = true;
                    }
                }
                if (MenuName == "OutstandingReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        OutstandingReportID.Visible = false;
                    }
                    else
                    {
                        OutstandingReportID.Visible = true;
                    }
                }
                if (MenuName == "PerformanceReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        PerformanceReportID.Visible = false;
                    }
                    else
                    {
                        PerformanceReportID.Visible = true;
                    }
                }
                if (MenuName == "MarginReport.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        ConsumptionReportID.Visible = false;
                    }
                    else
                    {
                        ConsumptionReportID.Visible = true;
                    }
                }
                //Authorization
                if (MenuName == "UserAuthorization.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        UserAuthorizationid.Visible = false;
                    }
                    else
                    {
                        UserAuthorizationid.Visible = true;
                    }
                }

                //Audit log
                if (MenuName == "AuditLogDashboard.aspx")
                {
                    string page1 = row["Pages"].ToString();
                    string pageView = row["PagesView"].ToString();
                    if (page1 == "False" && pageView == "False")
                    {
                        AuditlogID.Visible = false;
                    }
                    else
                    {
                        AuditlogID.Visible = true;
                    }
                }

                //condition

                if (UserListid.Visible == false && CompanyListid.Visible == false && ComponentID.Visible == false && RolelistID.Visible == false && ProductMasterListID.Visible == false && VendorMasterListid.Visible == false && TrasnportermasterID.Visible == false && TargetMasterID.Visible == false && SalesTargetID.Visible == false)
                {
                    Mastersid.Visible = false;
                }
                if (EnquiryID.Visible == false && GeneralQuotationListID.Visible == false && POCustomerID.Visible == false && ProfarmaID.Visible == false && DSRReports.Visible == false && CallandMeetingListID.Visible == false)
                {
                    Salesmarketing.Visible = false;
                }
                if (TaxInvoiceListid.Visible == false && ApprovedInvoiceListID.Visible == false && ReceiptVoucherID.Visible == false && CreditDebitNoteSaleID.Visible == false)
                {
                    Accounts.Visible = false;
                }
                if (InventryID.Visible == false && EnwardEntryID.Visible == false && OutwardEntryID.Visible == false && WarehouseInvoiceListID.Visible == false)
                {
                    Stock.Visible = false;
                }

                if (SalesReportID.Visible == false && PurchaseReportID.Visible == false && SalesReportID.Visible == false)
                {
                    Reports.Visible = false;
                }
                if (PurchaseOrderID.Visible == false && PurchaseBillID.Visible == false && PaymentVoucherID.Visible == false && CreditDebitNotePurchaseID.Visible == false)
                {
                    Purchase.Visible = false;
                }


            }
        }
        else
        {

            //entry.Visible = false;
            //entryhr.Visible = false;
            //master.Visible = false;
            //masterhr.Visible = false;
            //saleshr.Visible = false;
            //sales.Visible = false;
            //evalhr.Visible = false;
            //evalutionlist.Visible = false;
            //popup.Visible = true;
            //reporthr.Visible = false;
            //report.Visible = false;

            //string Role = Session["RoleName"].ToString();
            //if (Role=="Admin")
            //{
            //    Response.Redirect("~/Admin/AdminDashboard.aspx");
            //}
            //else
            //{
            //    Response.Redirect("~/Login.aspx");
            //}
            Response.Redirect("~/Login.aspx");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AlertScript", "alert('MyButton clicked!');", true);

        }
    }
    //protected void ActiveCount()
    //{
    //    Cls_Main.Conn_Open();
    //    int count = 0;
    //    SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM tbl_EnquiryData where IsActive=1 AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) = CAST(GETDATE() AS DATE);", Cls_Main.Conn);
    //    count = Convert.ToInt16(cmd.ExecuteScalar());
    //    lblcount.Text = count.ToString();
    //    Cls_Main.Conn_Close();
    //}

    //public void getdetails()
    //{
    //    DataTable dt = new DataTable();
    //    if (Session["Role"].ToString() == "Admin")
    //    {
    //        SqlDataAdapter sad = new SqlDataAdapter("select id, cname,EnqCode,SampleDate from tbl_EnquiryData where IsActive=1 AND Sample=1 AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE)", Cls_Main.Conn);
    //        sad.Fill(dt);
    //    }
    //    else
    //    {
    //        SqlDataAdapter sad = new SqlDataAdapter("select id, cname,EnqCode,SampleDate from tbl_EnquiryData where IsActive=1 AND Sample=1 AND  sessionname='" + Session["UserCode"].ToString() + "' AND Sample=1  AND DATEADD(DAY, 3, CAST(SampleDate AS DATE)) <= CAST(GETDATE() AS DATE)", Cls_Main.Conn);
    //        sad.Fill(dt);
    //    }

    //    if (dt.Rows.Count > 0)
    //    {
    //        GVServices.DataSource = dt;
    //        GVServices.DataBind();
    //    }
    //}
}
