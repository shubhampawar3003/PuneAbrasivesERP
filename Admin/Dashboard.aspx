<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" Async="true" CodeFile="Dashboard.aspx.cs" Inherits="Admin_Dashboard" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" type="text/css" href="https://stackpath.bootstrapcdn.com/bootstrap/4.4.1/css/bootstrap.min.css">
    <%--<link rel="stylesheet" type="text/css" href="https://pixinvent.com/stack-responsive-bootstrap-4-admin-template/app-assets/css/bootstrap-extended.min.css">--%>
    <link rel="stylesheet" type="text/css" href="https://pixinvent.com/stack-responsive-bootstrap-4-admin-template/app-assets/fonts/simple-line-icons/style.min.css">
    <link rel="stylesheet" type="text/css" href="https://pixinvent.com/stack-responsive-bootstrap-4-admin-template/app-assets/css/colors.min.css">
    <link rel="stylesheet" type="text/css" href="https://pixinvent.com/stack-responsive-bootstrap-4-admin-template/app-assets/css/bootstrap.min.css">
    <link href="https://fonts.googleapis.com/css?family=Montserrat&display=swap" rel="stylesheet">
    <link href="../Content/css/cards.css" rel="stylesheet" />
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <style>
        .gvhead {
            text-align: center;
            color: #0f0606;
            background-color: #31B0C4;
        }

        .count {
            font-size: 1.51rem;
        }

        .PriceSize {
            font-size: 2rem;
        }

        .card {
            position: relative;
            display: -webkit-box;
            display: -webkit-flex;
            display: -ms-flexbox;
            display: flex;
            -webkit-box-orient: vertical;
            -webkit-box-direction: normal;
            -webkit-flex-direction: column;
            -ms-flex-direction: column;
            flex-direction: column;
            min-width: 0;
            background-clip: border-box;
            border: 1px solid rgba(0,0,0,.06);
            border-radius: 0.25rem;
            box-shadow: 0 3px 10px rgb(0 0 0 / 0.2) !important;
            margin-bottom: 30px;
        }

        .media i {
            font-size: 40px;
        }

        .media svg {
            font-size: 40px;
        }

        .bg-dark {
            background-color: #212529 !important;
        }
    </style>

    <script>
        function resetNotificationCount() {
            document.getElementById('notificationCount').innerText = '0';
            // Optionally, if you want to hide the badge when the count is zero:
            document.getElementById('notificationCount').style.display = 'none';
        }

    </script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script>
        function markNotificationsAsSeen() {
            $.ajax({
                type: 'POST',
                url: 'Dashboard.aspx/MarkNotificationsAsSeen',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d === 'success') {
                        // Optionally refresh the notifications or update the UI
                        // $('#lblcount').text('0');
                        // window.location.reload();
                        window.location.href = "../Admin/SampleReminder.aspx";
                    }
                }
            });
        }
        $(document).ready(function () {
            $('#notificationDropdown').on('hidden.bs.dropdown', function () {

                // Refresh the page when the dropdown is closed
                window.location.reload();
            });
        });
    </script>
    <style>
        .modelprofile1 {
            background-color: rgba(0, 0, 0, 0.54);
            display: block;
            position: fixed;
            z-index: 1;
            left: 0;
            /*top: 10px;*/
            height: 100%;
            overflow: auto;
            width: 100%;
            margin-bottom: 25px;
        }

        .profilemodel2 {
            background-color: #fefefe;
            margin-top: 25px;
            /*padding: 17px 5px 18px 22px;*/
            padding: 0px 0px 15px 0px;
            width: 100%;
            top: 40px;
            color: #000;
            border-radius: 5px;
        }

        .lblpopup {
            text-align: left;
        }

        .wp-block-separator:not(.is-style-wide):not(.is-style-dots)::before, hr:not(.is-style-wide):not(.is-style-dots)::before {
            content: '';
            display: block;
            height: 1px;
            width: 100%;
            background: #cccccc;
        }

        .btnclose {
            background-color: #ef1e24;
            float: right;
            font-size: 18px !important;
            /* font-weight: 600; */
            color: #f7f6f6 !important;
            border: 0px groove !important;
            background-color: none !important;
            /*margin-right: 10px !important;*/
            cursor: pointer;
            font-weight: 600;
            border-radius: 4px;
            padding: 4px;
        }

        /*hr {
            margin-top: 5px !important;
            margin-bottom: 15px !important;
            border: 1px solid #eae6e6 !important;
            width: 100%;
        }*/
        hr.new1 {
            border-top: 1px dashed green !important;
            border: 0;
            margin-top: 5px !important;
            margin-bottom: 5px !important;
            width: 100%;
        }

        .errspan {
            float: right;
            margin-right: 6px;
            margin-top: -25px;
            position: relative;
            z-index: 2;
            color: black;
        }

        .currentlbl {
            text-align: center !important;
        }

        .completionList {
            border: solid 1px Gray;
            border-radius: 5px;
            margin: 0px;
            padding: 3px;
            height: 120px;
            overflow: auto;
            background-color: #FFFFFF;
        }

        .listItem {
            color: #191919;
        }

        .itemHighlighted {
            background-color: #ADD6FF;
        }

        .headingcls {
            background-color: #01a9ac;
            color: #fff;
            padding: 15px;
            border-radius: 5px 5px 0px 0px;
        }

        @media (min-width: 1200px) {
            .container {
                max-width: 1250px !important;
            }
        }
    </style>
    <style>
        /* Overall table style */
        .gridview {
            width: 100%;
            border-collapse: collapse;
            font-family: Arial, sans-serif;
            font-size: 14px;
            margin: 20px 0;
        }

            /* Header row */
            .gridview th {
                background-color: #4CAF50;
                color: white;
                text-align: left;
                padding: 8px;
                border-bottom: 2px solid #ddd;
            }

            /* Data rows */
            .gridview td {
                padding: 8px;
                border-bottom: 1px solid #ddd;
                text-align: left;
            }

            /* Alternating row color */
            .gridview tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            /* Hover effect */
            .gridview tr:hover {
                background-color: #e9e9e9;
            }

            /* Footer row */
            .gridview tfoot td {
                background-color: #4CAF50;
                color: white;
                font-weight: bold;
                text-align: right;
                padding: 8px;
                border-top: 2px solid #ddd;
            }

            /* Styling for empty rows (if any) */
            .gridview .emptyrow {
                background-color: #f9f9f9;
                color: #999999;
                text-align: center;
                padding: 20px;
                font-style: italic;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class=" container-fluid">
                <section id="minimal-statistics">
                    <br />
                    <div class="row" id="divsalestarget" runat="server" visible="false">
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <asp:LinkButton ID="LinkButton3" runat="server" CssClass="count primary" Text="Year  :" Font-Bold="true"></asp:LinkButton>
                                            </div>
                                            <div class="media-body text-right">
                                                <br />
                                                <asp:DropDownList ID="ddlYear" Width="120px" Font-Bold="true" runat="server"></asp:DropDownList>
                                                <br />
                                                <br />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <asp:LinkButton ID="LinkButton4" runat="server" CssClass="count primary" Text="Month:" Font-Bold="true"></asp:LinkButton>
                                            </div>
                                            <div class="media-body text-right">
                                                <br />
                                                <asp:DropDownList ID="ddlMonth" Width="100px" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="ddlMonth_SelectedIndexChanged" runat="server">
                                                    <asp:ListItem Value="1">January</asp:ListItem>
                                                    <asp:ListItem Value="2">February</asp:ListItem>
                                                    <asp:ListItem Value="3">March</asp:ListItem>
                                                    <asp:ListItem Value="4">April</asp:ListItem>
                                                    <asp:ListItem Value="5">May</asp:ListItem>
                                                    <asp:ListItem Value="6">June</asp:ListItem>
                                                    <asp:ListItem Value="7">July</asp:ListItem>
                                                    <asp:ListItem Value="8">August</asp:ListItem>
                                                    <asp:ListItem Value="9">September</asp:ListItem>
                                                    <asp:ListItem Value="10">October</asp:ListItem>
                                                    <asp:ListItem Value="11">November</asp:ListItem>
                                                    <asp:ListItem Value="12">December</asp:ListItem>
                                                </asp:DropDownList>
                                                <br />
                                                <br />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <asp:LinkButton ID="LinkButton5" runat="server" CssClass="count primary" Text="Target" Font-Bold="true"></asp:LinkButton>
                                                <br />
                                                <asp:LinkButton ID="LinkButton8" runat="server" CssClass="count primary" Text="Kg/Ton :" Font-Bold="true"></asp:LinkButton>
                                            </div>
                                            <div class="media-body text-right">

                                                <asp:LinkButton ID="lblQuantityCmpl" runat="server" Text="Total Suppliers" Font-Bold="true"></asp:LinkButton>
                                                <br />
                                                <hr />
                                                <asp:LinkButton ID="lbltarget" runat="server" Text="Total Suppliers" Font-Bold="true"></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <asp:LinkButton ID="LinkButton6" runat="server" CssClass="count primary" Text="Target" Font-Bold="true"></asp:LinkButton>
                                                <br />
                                                <asp:LinkButton ID="LinkButton2" runat="server" CssClass="count primary" Text="Amount :" Font-Bold="true"></asp:LinkButton>
                                            </div>
                                            <div class="media-body text-right">
                                                <asp:LinkButton ID="lblAmtCompelete" runat="server" Text="Total Suppliers" Font-Bold="true"></asp:LinkButton>

                                                <br />
                                                <hr />
                                                <asp:LinkButton ID="lblamount" runat="server" Text="Total Suppliers" Font-Bold="true"></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <hr />
                    </div>


                    <div class="row">
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <i class="icon-users primary font-large-2 float-left"></i>
                                            </div>
                                            <div class="media-body text-right">
                                                <asp:LinkButton ID="lbluserscount" runat="server" CssClass="count primary" Font-Bold="true" OnClick="lnkUsers_Click"></asp:LinkButton>

                                                <br />
                                                <asp:LinkButton ID="lnkUsers" runat="server" Text="Total Users" Font-Bold="true" OnClick="lnkUsers_Click"></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <i class="fas fa-industry warning font-large-2 float-left"></i>
                                            </div>
                                            <div class="media-body text-right">
                                                <asp:LinkButton ID="lblcompanycount" runat="server" CssClass="count warning" Font-Bold="true" OnClick="lnkcompany_Click"></asp:LinkButton>

                                                <br />
                                                <asp:LinkButton ID="lnkcompany" runat="server" Text="Total Customer" Font-Bold="true" OnClick="lnkcompany_Click"></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <%--<i class="fas fa-building-user success font-large-2 float-left"></i>--%>
                                                <i class="fa fa-cogs" style="font-size: 50px; color: red"></i>
                                            </div>
                                            <div class="media-body text-right">
                                                <asp:LinkButton ID="lblProductCount" runat="server" CssClass="count success" Font-Bold="true" OnClick="lblTotalProduct_Click"></asp:LinkButton>

                                                <br />
                                                <asp:LinkButton ID="lblTotalProduct" runat="server" Text="Total Products" Font-Bold="true" OnClick="lblTotalProduct_Click"></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <i class="fas fa-building-user success font-large-2 float-left"></i>
                                            </div>
                                            <div class="media-body text-right">
                                                <asp:LinkButton ID="lblvendorcount" runat="server" CssClass="count success" Font-Bold="true" OnClick="lnkvendors_Click"></asp:LinkButton>
                                                <br />
                                                <asp:LinkButton ID="lnkvendors" runat="server" Text="Total Suppliers" Font-Bold="true" OnClick="lnkvendors_Click"></asp:LinkButton>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <hr />
                    </div>

                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-md-11" style="text-align: center">
                                <asp:Label ID="lbltoday" runat="server" Font-Size="20px" Font-Bold="true"></asp:Label>
                            </div>
                            <div class="col-md-1">
                                <a class="nav-link" href="#" id="notificationDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false" onclick="markNotificationsAsSeen()">
                                    <i class="fas fa-bell" style="font-size: 20px;"></i>
                                    <span class="badge bg-danger" id="notificationCount" runat="server">
                                        <asp:Label ID="lblcount" runat="server" Text="0"></asp:Label>
                                    </span>
                                </a>
                            </div>
                            <%-- <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="notificationDropdown">
                                    <li class="dropdown-header">Notifications</li>--%>
                            <asp:GridView ID="grdnotification" runat="server" OnRowDataBound="grdnotification_RowDataBound" CellPadding="4" Visible="false" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSRNO" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="EnqCode" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Address" runat="server" Text='<%# Eval("EnqCode") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="cname" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Contactperson" runat="server" Text='<%# Eval("cname") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="SampleDate" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="mobileno" runat="server" Text='<%# Eval("SampleDate") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <%-- </ul>--%>
                        </div>
                        <div class="row">
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="EnquiryList.aspx"><i class="fa fa-question-circle primary font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblenquiriesCount" runat="server" CssClass="count success" Font-Bold="true" AutoPostBack="true" OnClick="lnkenquiries_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkenquiries" runat="server" Text="Total Enquiries" Font-Bold="true" AutoPostBack="true" OnClick="lnkenquiries_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="QuatationList.aspx"><i class="fa fa-file warning font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblQutationCount" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkQutation_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkQutation" runat="server" Text="Total Quotation" AutoPostBack="true" Font-Bold="true" OnClick="lnkQutation_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="ApprovedInvoiceList.aspx"><i class="fa fa-newspaper" style="font-size: 50px; color: red"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblInvoiceCount" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkInvoice_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkInvoice" runat="server" Text="Total Tax Invoice" AutoPostBack="true" Font-Bold="true" OnClick="lnkInvoice_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="EnquiryList.aspx"><i class="fa fa-paper-plane primary font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblSampleCount" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkSample_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkSample" runat="server" Text="Total Sample Send" AutoPostBack="true" Font-Bold="true" OnClick="lnkSample_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-11" style="text-align: center">
                                <asp:Label ID="lblmonth" runat="server" Font-Size="20px" Font-Bold="true"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="EnquiryList.aspx"><i class="fa fa-question-circle primary font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblmonthenquiry" runat="server" CssClass="count success" Font-Bold="true" AutoPostBack="true" OnClick="lnkenquiries_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkmonthenquiry" runat="server" Text="Total Enquiries" Font-Bold="true" AutoPostBack="true" OnClick="lnkenquiries_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="QuatationList.aspx"><i class="fa fa-file warning font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblmonthQuotation" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkQutation_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkmonthQuotation" runat="server" Text="Total Quotation" AutoPostBack="true" Font-Bold="true" OnClick="lnkQutation_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="ApprovedInvoiceList.aspx"><i class="fa fa-newspaper" style="font-size: 50px; color: red"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblmothInvoice" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkInvoice_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkmothInvoice" runat="server" Text="Total Tax Invoice" AutoPostBack="true" Font-Bold="true" OnClick="lnkInvoice_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="EnquiryList.aspx"><i class="fa fa-paper-plane primary font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblmonthsample" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkSample_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkmonthsample" runat="server" Text="Total Sample Send" AutoPostBack="true" Font-Bold="true" OnClick="lnkSample_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-11" style="text-align: center">
                                <asp:Label ID="lblyear" runat="server" Font-Size="20px" Font-Bold="true"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="EnquiryList.aspx"><i class="fa fa-question-circle primary font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblyearenquiry" runat="server" CssClass="count success" Font-Bold="true" AutoPostBack="true" OnClick="lnkenquiries_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkyearenquiry" runat="server" Text="Total Enquiries" Font-Bold="true" AutoPostBack="true" OnClick="lnkenquiries_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="QuatationList.aspx"><i class="fa fa-file warning font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblyearquotation" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkQutation_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkyearquotation" runat="server" Text="Total Quotation" AutoPostBack="true" Font-Bold="true" OnClick="lnkQutation_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="ApprovedInvoiceList.aspx"><i class="fa fa-newspaper" style="font-size: 50px; color: red"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblyearinvoice" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkInvoice_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkyearinvoice" runat="server" Text="Total Tax Invoice" AutoPostBack="true" Font-Bold="true" OnClick="lnkInvoice_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-xl-3 col-sm-6 col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="media d-flex">
                                                <div class="align-self-center">
                                                    <a href="EnquiryList.aspx"><i class="fa fa-paper-plane primary font-large-2 float-left"></i></a>
                                                </div>
                                                <div class="media-body text-right">
                                                    <asp:LinkButton ID="lblyearsample" runat="server" Font-Bold="true" AutoPostBack="true" CssClass="count success" OnClick="lnkSample_Click"></asp:LinkButton>
                                                    <br />
                                                    <asp:LinkButton ID="lnkyearsample" runat="server" Text="Total Sample Send" AutoPostBack="true" Font-Bold="true" OnClick="lnkSample_Click"></asp:LinkButton>

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="c-dashboardInfo col-lg-6 col-md-6" id="Quotation" runat="server" visible="false">
                                <div class="wrap">
                                    <%--<span class="hind-font caption-12 c-dashboardInfo__count" style="margin-top: -29px;">Purchased Products</span>--%>
                                    <h2 class="hind-font " style="margin-top: -19px;">Quotation List</h2>
                                    <h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">( Today Created TOP 5 )</h4>
                                    <br />
                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="gvQuotation" runat="server" CssClass="grivdiv"
                                            AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" PageSize="10"
                                            AllowPaging="true" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-CssClass="gvhead sno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblseno" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Quo No." HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQuotationno" runat="server" Text='<%#Eval("Quotationno")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Company Name" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCompanyname" runat="server" Text='<%#Eval("Companyname")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltotlprice" runat="server" Text='<%#Eval("Total_Price")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                            <div class="c-dashboardInfo col-lg-6 col-md-6" id="Tax" runat="server" visible="false">
                                <div class="wrap">
                                    <%--<span class="hind-font caption-12 c-dashboardInfo__count" style="margin-top: -29px;">Purchased Products</span>--%>
                                    <h2 class="hind-font " style="margin-top: -19px;">Tax Invoice List</h2>
                                    <h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">( Today Created TOP 5 )</h4>
                                    <br />
                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="gvInvoice" runat="server" CssClass="grivdiv"
                                            AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" PageSize="10"
                                            AllowPaging="true" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-CssClass="gvhead sno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblseno" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblInvoiceno" runat="server" Text='<%#Eval("Invoiceno")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Company Name" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCompanyname" runat="server" Text='<%#Eval("BillingCustomer")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltotalprice" runat="server" Text='<%#Eval("GrandTotalFinal")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="c-dashboardInfo col-lg-6 col-md-6" id="Purchase" runat="server" visible="false">
                                <div class="wrap">
                                    <%--<span class="hind-font caption-12 c-dashboardInfo__count" style="margin-top: -29px;">Purchased Products</span>--%>
                                    <h2 class="hind-font " style="margin-top: -19px;">Purchase List</h2>
                                    <h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">( Today Created TOP 5)</h4>
                                    <br />
                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="GVPurchase" runat="server" CssClass="grivdiv"
                                            AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" PageSize="10"
                                            AllowPaging="true" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-CssClass="gvhead sno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblseno" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="P.O. No." HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblpono" runat="server" Text='<%#Eval("Pono")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsuppliername" runat="server" Text='<%#Eval("SupplierName")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltotlprice" runat="server" Text='<%#Eval("Total_Price")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>

                            <div class="c-dashboardInfo col-lg-6 col-md-6" id="reciept" runat="server" visible="false">
                                <div class="wrap">
                                    <%--<span class="hind-font caption-12 c-dashboardInfo__count" style="margin-top: -29px;">Purchased Products</span>--%>
                                    <h2 class="hind-font " style="margin-top: -19px;">Receipt List</h2>
                                    <h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">( Today Created TOP 5 )</h4>
                                    <br />
                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="GVReceipt" runat="server" CssClass="grivdiv"
                                            AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" PageSize="10"
                                            AllowPaging="true" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-CssClass="gvhead sno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblseno" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Receipt No." HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblReceiptCode" runat="server" Text='<%#Eval("ReceiptCode")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Company Name" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCompanyname" runat="server" Text='<%#Eval("Companyname")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Amount" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmount" runat="server" Text='<%#Eval("Amount")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <div class="c-dashboardInfo col-lg-6 col-md-6" id="Div1" runat="server" visible="false">
                                <div class="wrap">
                                    <%--<span class="hind-font caption-12 c-dashboardInfo__count" style="margin-top: -29px;">Purchased Products</span>--%>
                                    <h2 class="hind-font " style="margin-top: -19px;">Availebal Product List</h2>
                                    <%--     <h4 class="heading heading5 hind-font medium-font-weight c-dashboardInfo__title">( Today Created TOP 5 )</h4>--%>
                                    <%--   <br />--%>
                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="grdProductpending" runat="server" CssClass="grivdiv"
                                            AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" PageSize="10"
                                            AllowPaging="true" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-CssClass="gvhead sno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblseno" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Product Name" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblProductName" runat="server" Text='<%#Eval("Productname")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQuantity" runat="server" Text='<%#Eval("PendingQuantity")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="table " id="tblservice" runat="server" visible="false">
                            <asp:GridView ID="GVServices" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%"
                                CssClass="grivdiv pagination-ys" OnRowCommand="GVServices_RowCommand" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSRNO" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Ticket No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblstatus" runat="server" Text='<%#Eval("TicketNo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnTicketno" runat="server" Font-Bold="true" CssClass="btn btn-outline-success" CausesValidation="false" Visible='<%# Eval("Action").ToString() == "Closed" ? false : true %>' CommandName="Ticketno" Text='<%#Eval("Action")%>' CommandArgument='<%#Eval("TicketNo")%>'></asp:LinkButton>
                                            <asp:Label ID="lblstatus4" runat="server" Font-Bold="true" ForeColor="Green" Visible='<%# Eval("Action").ToString() == "Open" ? false : true %>' Text='<%#Eval("Action")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Register By" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Register" runat="server" Text='<%#Eval("Username")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Custmer" runat="server" Text='<%#Eval("CompanyName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Address" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Address" runat="server" Text='<%#Eval("Address")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Person" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Contactperson" runat="server" Text='<%#Eval("OwnerName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mobile No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="mobileno" runat="server" Text='<%#Eval("MobileNo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email ID" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Email" runat="server" Text='<%#Eval("EmailID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Category" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Category" runat="server" Text='<%#Eval("call_category")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Registration Date" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Registrationdate" runat="server" Text='<%#Eval("CreatedOn")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ACTION" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnDelete" runat="server" Height="27px" ToolTip="Delete" CausesValidation="false" Visible='<%# Eval("Action").ToString() == "Open" ? true : false %>' CommandName="RowDelete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CommandArgument='<%#Eval("TicketNo")%>'><i class='fas fa-trash' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                            <asp:LinkButton runat="server" ID="btnpdfview" ToolTip="View Service Report" CommandName="RowView" Visible='<%# Eval("Action").ToString() == "Open" ? false : true %>' CommandArgument='<%# Eval("TicketNo") %>'><i class="fas fa-file-pdf"  style="font-size: 26px; color:orange; "></i></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                </section>
                <section id="stats-subtitle">
                </section>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:Button ID="btnhist" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupHistory" runat="server" TargetControlID="btnhist"
        PopupControlID="PopupHistoryDetail" OkControlID="Closepophistory" />

    <asp:Panel ID="PopupHistoryDetail" runat="server" CssClass="modelprofile1">
        <div class="row container">
            <div class="col-md-6"></div>
            <div class="col-md-6">
                <div class="profilemodel2">
                    <div class="headingcls">
                        <h4 class="modal-title">Meeting Reminders
                              
                            <button type="button" id="Closepophistory" class="btnclose" style="display: inline-block;" data-dismiss="modal">Close</button></h4>
                    </div>

                    <br />
                    <div class="col-md-4" style="text-align: left">
                        <asp:LinkButton ID="LinkButton1" CssClass="form-control btn btn-info" data-dismiss="modal" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton1_Click">
    <i class="fas fa-file-alt"></i>&nbsp&nbsp DSR LIST
                        </asp:LinkButton>
                    </div>
                    <div class="body" style="margin-right: 10px; margin-left: 10px; padding-right: 1px; padding-left: 1px;">
                        <div class="row">



                            <div class="table">
                                <asp:GridView ID="grdreminder" runat="server" DataKeyNames="ID" PageSize="10" AllowPaging="true" Width="100%" Height="50%"
                                    CssClass="grivdiv pagination-ys" OnPageIndexChanging="grdreminder_PageIndexChanging" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr No." HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSRNO" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Company Name" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="CompanyName" runat="server" Text='<%# Eval("CompanyName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status of Update" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>

                                                <asp:Label ID="lblUpdateStatus" runat="server" Text='<%# "<strong>" + Eval("Updatefor") + "</strong> - " + Eval("UpdateStatus") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Follow-up Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <%# Convert.ToDateTime(Eval("followupdate")).ToString("dd/MM/yyyy") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>


    </asp:Panel>

    <asp:Button ID="Button1" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupExtender1" runat="server" TargetControlID="Button1"
        PopupControlID="Panel1" OkControlID="Closepophistory1" />

    <asp:Panel ID="Panel1" runat="server" CssClass="modelprofile1">
        <div class="row container">
            <div class="col-md-6"></div>
            <div class="col-md-6">
                <div class="profilemodel2">
                    <div class="headingcls" style="background-color: #007bff; padding: 15px; color: white;">
                        <h4 class="modal-title" style="display: flex; align-items: center; justify-content: space-between;">Reminder
       
                            <div>
                                <asp:LinkButton ID="LinkButton7" CssClass="btn btn-primary btn-sm" Visible="false" data-dismiss="modal" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton7_Click" Style="font-weight: bold;">
                <i class="fas fa-file-alt"></i> OA List 
            </asp:LinkButton>
                                <asp:LinkButton ID="LinkButton9" CssClass="btn btn-primary btn-sm" Visible="false" data-dismiss="modal" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton9_Click" Style="font-weight: bold;">
                <i class="fas fa-file-alt"></i> Invoice List
            </asp:LinkButton>
                                <button type="button" id="Closepophistory1" class="btn btn-danger btn-sm" style="margin-left: 10px; font-weight: bold;" data-dismiss="modal">Close</button>
                            </div>
                        </h4>
                    </div>

                    <div class="body" style="margin-right: 10px; margin-left: 10px; padding-right: 1px; padding-left: 1px;">
                        <div class="row" runat="server" id="oadetails" visible="false">
                            <div class="col-10 col-md-10">
                                <h5>ORDER ACCEPTANCE LIST </h5>
                            </div>

                        </div>
                        <div class="row">

                            <div class="table">
                                <asp:GridView ID="GVOAreminder" runat="server" DataKeyNames="Pono" Width="100%" Height="50%"
                                    CssClass="gridview" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr No." HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSRNO" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="OA Number" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="OANumber" runat="server" Text='<%# Eval("Pono") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="OA Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <%# Convert.ToDateTime(Eval("PoDate")).ToString("dd/MM/yyyy") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="CustomerName" runat="server" Text='<%# Eval("CustomerName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Created Person" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="OANumber" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                    <div class="body" style="margin-right: 10px; margin-left: 10px; padding-right: 1px; padding-left: 1px;">
                        <div class="row" runat="server" id="taxinvoice" visible="false">
                            <div class="col-10 col-md-10">
                                <h5>APPROVED TAX-INVOICE LIST </h5>
                            </div>

                        </div>
                        <div class="row">

                            <div class="table">
                                <asp:GridView ID="GVInvoiceList" runat="server" DataKeyNames="InvoiceNo" Width="100%" Height="50%"
                                    CssClass="gridview" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr No." HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="lblSRNO" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="InvoiceNo" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <%# Convert.ToDateTime(Eval("Invoicedate")).ToString("dd/MM/yyyy") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="CustomerName" runat="server" Text='<%# Eval("BillingCustomer") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--       <asp:TemplateField HeaderText="Created Person" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="OANumber" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>  <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
</asp:Content>
