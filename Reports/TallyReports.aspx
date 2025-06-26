<%@ Page Title="" Debug="true" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="TallyReports.aspx.cs" Inherits="Reports_TallyReports" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <div class="page-wrapper">
        <div class="page-body">
            <div class="container py-3">
                <div class="card">
                    <div class="card-header bg-black text-uppercase text-white">
                        <div class="row">
                            <div class="col-md-4">
                                <h5>Tally Import Report</h5>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xl-12 col-md-12">
                            <div class="card">
                                <div class="card-header">

                                    <div class="row">
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblcutomername" runat="server" Font-Bold="true">Customer Name :</asp:Label>
                                            <asp:TextBox ID="txtPartyName" runat="server" CssClass="form-control" placeholder="Customer Name" Width="100%"></asp:TextBox>
                                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                                TargetControlID="txtPartyName">
                                            </asp:AutoCompleteExtender>

                                            <%--  <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList"
                                                TargetControlID="txtPartyName">
                                            </asp:AutoCompleteExtender>--%>
                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblFromdate" runat="server" Font-Bold="true">From Date :</asp:Label>
                                            <asp:TextBox ID="txtfromdate" runat="server" CssClass="form-control" TextMode="Date" placeholder="From Date" Width="100%" AutoComplete="off"></asp:TextBox>

                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblTodate" runat="server" Font-Bold="true">To Date :</asp:Label>
                                            <asp:TextBox ID="txttodate" runat="server" CssClass="form-control" TextMode="Date" placeholder="To Date" Width="100%" AutoComplete="off"></asp:TextBox>

                                        </div>
                                        <div class="col-xl-3 col-md-3" style="margin-top: auto">
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row" id="btn" runat="server">
                                        <div class="col-xl-12 col-md-12">
                                            <asp:Button ID="btnGroup" runat="server" CssClass="btn btn-success" Text="Group" CommandName="Group" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnCostCategory" runat="server" CssClass="btn btn-success" Text="Cost Category" CommandName="CostCategory" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnCostCenter" runat="server" CssClass="btn btn-success" Text="Cost Center" CommandName="CostCenter" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnStockGroup" runat="server" CssClass="btn btn-success" Text="Stock Group" CommandName="StockGroup" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnUnit" runat="server" CssClass="btn btn-success" Text="Unit" CommandName="Unit" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnStockItem" runat="server" CssClass="btn btn-success" Text="Stock Item" CommandName="StockItem" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnLedger" runat="server" CssClass="btn btn-success" Text="Ledger" CommandName="Ledger" OnClick="btnpdf_Click1" />
                                            <asp:Button ID="btnAccountingVoucher" runat="server" CssClass="btn btn-success" Text="Accounting Voucher" CommandName="AccountingVoucher" OnClick="btnpdf_Click1" />

                                        </div>
                                     
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-12" style="padding: 20px; margin-top: 5px;">
                <iframe id="ifrRight6" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 685px;"></iframe>
            </div>
        </div>
    </div>
    <%-- <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>--%>
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>

</asp:Content>

