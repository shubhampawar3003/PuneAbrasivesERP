<%@ Page Title="" Debug="true" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="PartyLedgerReport.aspx.cs" Inherits="Reports_PartyLedgerReport" %>

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
                                <h5>Party Ledger Report</h5>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xl-12 col-md-12">
                            <div class="card">
                                <div class="card-header">
                                    <%--<div class="row" style="margin-left: -12px;">
                                        <asp:HiddenField ID="hiddenopening" runat="server" />
                                        <asp:HiddenField ID="HiddenClosing" runat="server" />
                                         <div class="col-xl-1 col-md-1 spancls">Type<i class="reqcls">&nbsp;</i> :</div>
                                        <div class="col-xl-2 col-md-2 spancls" style="margin-left: 177px;">Party Name<i class="reqcls">&nbsp;</i> :</div>
                                        <div class="col-xl-2 col-md-2 spancls" style="margin-left: 87px;">From Date<i class="reqcls">&nbsp;</i> :</div>
                                        <div class="col-xl-2 col-md-2 spancls" style="margin-left: 92px;">To Date<i class="reqcls">&nbsp;</i> :</div>
                                    </div>--%>
                                    <div class="row">
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblcutomername" runat="server" Font-Bold="true">Customer Name :</asp:Label>
                                            <asp:TextBox ID="txtPartyName" runat="server" CssClass="form-control" OnTextChanged="txtPartyName_TextChanged" placeholder="Customer Name" Width="100%" AutoPostBack="true"></asp:TextBox>
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
                                            <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-info" Text="Search" OnClick="btnSearch_Click" />
                                            <asp:Button ID="btnresetfilter" CssClass="btn btn-danger" runat="server" Text="Reset" OnClick="btnresetfilter_Click" />
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row" id="btn" runat="server">
                                        <div class="col-xl-6 col-md-6">
                                            <asp:Button ID="btnexcel" runat="server" ValidationGroup="form1" CssClass="btn btn-success" Text="Excel" OnClick="ExportExcel" />
                                            <asp:Button ID="btnpdf" runat="server" ValidationGroup="form1" CssClass="btn btn-success" Text="PDF" OnClick="btnpdf_Click" />

                                        </div>
                                        <div class="col-xl-4 col-md-4"></div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="container-fluid">
                <div class="col-md-12" style="padding: 0px; margin-top: 5px;">

                    <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">

                        <asp:GridView ID="GVfollowup" runat="server" CellPadding="4" Font-Names="Verdana" ShowFooter="true"
                            Font-Size="12pt" Width="100%"
                            GridLines="Both" PageSize="100000" AllowPaging="true" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false"
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
                                <asp:TemplateField HeaderText="Type" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblType" runat="server" Text='<%#Eval("Type")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Doc No." HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDocNo" runat="server" Text='<%#Eval("DocNo")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDate" runat="server" Text='<%#Eval("CDate")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Particulers" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblParticulers" runat="server" Text='<%#Eval("Particulars")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label runat="server" Text="Total:" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Debit" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDebit" runat="server" Text='<%#Eval("Debit")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalDebit" runat="server" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Credit" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCredit" runat="server" Text='<%#Eval("Credit")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalCredit" runat="server" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Balance" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBalance" runat="server" Text='<%#Eval("Balance")%>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalBalance" runat="server" />
                                    </FooterTemplate>
                                </asp:TemplateField>

                            </Columns>
                            <FooterStyle Font-Bold="True" ForeColor="Yellow" HorizontalAlign="Center" />

                        </asp:GridView>
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

