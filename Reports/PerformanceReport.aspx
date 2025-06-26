<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="PerformanceReport.aspx.cs" Inherits="Reports_PerformanceReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />

    <style>
        .spancls {
            color: #5d5656 !important;
            font-size: 13px !important;
            font-weight: 600;
            text-align: left;
        }

        .starcls {
            color: red;
            font-size: 18px;
            font-weight: 700;
        }

        .card .card-header span {
            color: #060606;
            display: block;
            font-size: 13px;
            margin-top: 5px;
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

        .reqcls {
            color: red;
            font-weight: 600;
            font-size: 14px;
        }

        .aspNetDisabled {
            cursor: not-allowed !important;
        }

        .rwotoppadding {
            padding-top: 10px;
        }
    </style>
    <style>
        .gvhead {
            text-align: center;
            color: #ffffff;
            background-color: #212529;
        }

        .pagination-ys {
            /*display: inline-block;*/
            padding-left: 0;
            margin: 20px 0;
            border-radius: 4px;
        }

            .pagination-ys table > tbody > tr > td {
                display: inline;
            }

                .pagination-ys table > tbody > tr > td > a,
                .pagination-ys table > tbody > tr > td > span {
                    position: relative;
                    float: left;
                    padding: 8px 12px;
                    line-height: 1.42857143;
                    text-decoration: none;
                    color: #dd4814;
                    background-color: #ffffff;
                    border: 1px solid #dddddd;
                    margin-left: -1px;
                }

                .pagination-ys table > tbody > tr > td > span {
                    position: relative;
                    float: left;
                    padding: 8px 12px;
                    line-height: 1.42857143;
                    text-decoration: none;
                    margin-left: -1px;
                    z-index: 2;
                    color: #aea79f;
                    background-color: #f5f5f5;
                    border-color: #dddddd;
                    cursor: default;
                }

                .pagination-ys table > tbody > tr > td:first-child > a,
                .pagination-ys table > tbody > tr > td:first-child > span {
                    margin-left: 13px;
                    border-bottom-left-radius: 4px;
                    border-top-left-radius: 4px;
                }

                /*.pagination-ys table > tbody > tr > td:last-child > a,
                .pagination-ys table > tbody > tr > td:last-child > span {
                    border-bottom-right-radius: 4px;
                    border-top-right-radius: 4px;
                }*/

                .pagination-ys table > tbody > tr > td > a:hover,
                .pagination-ys table > tbody > tr > td > span:hover,
                .pagination-ys table > tbody > tr > td > a:focus,
                .pagination-ys table > tbody > tr > td > span:focus {
                    color: #97310e;
                    background-color: #eeeeee;
                    border-color: #dddddd;
                }
    </style>

    <link href="../Content/css/cards.css" rel="stylesheet" />
    <style>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">PERFORMANCE REPORT</h4>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblcompanyname" Font-Bold="true" runat="server" Text="Company Name :"></asp:Label>
                            <asp:TextBox ID="txtCustomerName" CssClass="form-control" placeholder="Search Customer" runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Name"
                                ControlToValidate="txtCustomerName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                TargetControlID="txtCustomerName">
                            </asp:AutoCompleteExtender>

                        </div>
                    </div>
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lbluser" Font-Bold="true" runat="server" Text="Sales Person Name :"></asp:Label>
                            <asp:TextBox ID="txtusername" CssClass="form-control" placeholder="Search Sales Person Name" runat="server" OnTextChanged="txtusername_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetUserNameList"
                                TargetControlID="txtusername">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblfromdate" runat="server" Font-Bold="true" Text="From Date :"></asp:Label>
                            <asp:TextBox ID="txtfromdate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lbltodate" runat="server" Font-Bold="true" Text="To Date :"></asp:Label>
                            <asp:TextBox ID="txttodate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>
                        </div>
                    </div>

                </div>
                <div class="row">
                    <div class="col-xl-4 col-md-4" style="text-align: left">
                        <br />
                        <asp:Button ID="btnDownload" OnClick="btnDownload_Click" CssClass="btn btn-success" runat="server" Text="Excel" Style="padding: 8px;" />
                        <asp:Button ID="btnPDF" OnClick="btnPDF_Click" CssClass="btn btn-success" runat="server" Text="PDF" Style="padding: 8px;" />
                    </div>
                    <div class="col-xl-4 col-md-4" style="text-align: center">
                        <br />
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />
                    </div>

                    <div class="col-xl-4 col-md-4" style="text-align: center"></div>
                </div>

            </div>
          
            <div class="row">
                <asp:Repeater runat="server" ID="rptsales">
                    <ItemTemplate>
                        <div class="col-xl-3 col-sm-6 col-12">
                            <div class="card">
                                <div class="card-content">
                                    <div class="card-body">
                                        <div class="media d-flex">
                                            <div class="align-self-center">
                                                <asp:Label ID="lblnamestital" runat="server" Font-Bold="true">Name :  </asp:Label>
                                                <br />
                                                <asp:Label ID="lblusername" runat="server" Font-Bold="true"> Sale :</asp:Label>
                                            </div>
                                            <div class="media-body text-right">
                                                <asp:Label ID="lblname" runat="server" Font-Bold="true" ForeColor="Red" Text='<%# Eval("UserName") %>'></asp:Label>
                                                <br />
                                                <asp:Label ID="lbluserne" Font-Bold="true" runat="server" ForeColor="Green" Text='<%# Eval("TotalGrandAmount") %>'></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="container-fluid">
                <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">
                    <asp:GridView ID="GVfollowup" runat="server" CellPadding="4" Font-Names="Verdana" ShowFooter="true"
                        Font-Size="12pt" Width="100%"
                        GridLines="Both" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false"
                        OnRowDataBound="GVfollowup_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No." HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblSrNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblCustomer" runat="server" Text='<%#Eval("Customer")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotBasic" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="UserName" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblUserName" runat="server" Text='<%#Eval("UserName")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalSGST" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvoiceNo" runat="server" Text='<%#Eval("InvoiceNo")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Invoice Date" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvoicedate" runat="server" Text='<%#Eval("Invoicedate")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Due Date" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblDueDate" runat="server" Text='<%#Eval("DueDate")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalBasic" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="O/S Date" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblOSDate" runat="server" Text='<%#Eval("OSDate")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalIGST" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Due Days" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblPaymentterm" runat="server" Text='<%#Eval("Paymentterm")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblGraTotal" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="From due DateTo" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblFromdueDateTo" runat="server" Text='<%#Eval("FromdueDateTo")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblandTotal" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="From InvDate To TillDate" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblFromInvDateToTillDate" runat="server" Text='<%#Eval("FromInvDateToTillDate")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblGrndTotal" runat="server" Text="Total:" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="O/S Amount" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblOSAmount" runat="server" Text='<%#Eval("OSAmount")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalOSAmount" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText=">120 Days" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblMorethenonetwenty" runat="server" Text='<%#Eval("Morethenonetwenty")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalMorethenonetwenty" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="90-120 Days" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblNigntytoonetwenty" runat="server" Text='<%#Eval("Nigntytoonetwenty")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalNigntytoonetwenty" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="60-90 Days" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblSixtytonignty" runat="server" Text='<%#Eval("Sixtytonignty")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalSixtytonignty" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="30-60 Days" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblthirtytoSexty" runat="server" Text='<%#Eval("thirtytoSexty")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalthirtytoSexty" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<30 Days" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblLessThenthirty" runat="server" Text='<%#Eval("LessThenthirty")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTotalLessThenthirty" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Remarks" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblRemarks" runat="server" Text='<%#Eval("Remarks")%>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblTota" runat="server" />
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <FooterStyle Font-Bold="True" ForeColor="black" HorizontalAlign="Center" />

                    </asp:GridView>

                </div>

            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownload" />
            <asp:PostBackTrigger ControlID="btnPDF" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
