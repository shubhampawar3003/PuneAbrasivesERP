<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master"  AutoEventWireup="true" CodeFile="ProformaInvoiceList.aspx.cs" Inherits="Admin_ProformaInvoiceList" %>

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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                            <h4 class="mt-4">&nbsp <b>PROFORMA INVOICE LIST</b></h4>                        
                    </div>
                    <div class="col-3 col-md-2 mt-4">
                        <asp:Button ID="btnCreate" CssClass="form-control btn btn-warning" OnClick="btnCreate_Click" runat="server" Text="Create" />
                    </div>
                </div>
                <br />
                <div>
                    <div class="row">
                        <div class="col-md-3">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Customer Name :"></asp:Label>
                            <div style="margin-top: 14px;">
                                <asp:TextBox ID="txtCustomerName" CssClass="form-control" placeholder="Search Company" runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                    ControlToValidate="txtCustomerName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                    CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                    CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                    TargetControlID="txtCustomerName">
                                </asp:AutoCompleteExtender>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Invoice No. :"></asp:Label>
                            <div style="margin-top: 14px;">
                                <asp:TextBox ID="txtCpono" CssClass="form-control" placeholder="Search Invoice No." runat="server" OnTextChanged="txtCpono_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                    CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                    CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCponoList"
                                    TargetControlID="txtCpono">
                                </asp:AutoCompleteExtender>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <asp:Label ID="lblfromdate" runat="server" Font-Bold="true" Text="From Date :"></asp:Label>
                            <div style="margin-top: 14px;">
                                <asp:TextBox ID="txtfromdate" Placeholder="Enter From Date" runat="server" TextMode="Date" AutoComplete="off" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <asp:Label ID="lbltodate" runat="server" Font-Bold="true" Text="To Date :"></asp:Label>
                            <div style="margin-top: 14px;">
                                <asp:TextBox ID="txttodate" Placeholder="Enter To Date" runat="server" TextMode="Date" AutoComplete="off" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-1 col-2" style="margin-top: 36px">
                            <asp:LinkButton ID="btnSearch" OnClick="btnSearch_Click" runat="server" Width="100%" CssClass="btn btn-info"><i style="color:white" class="fa">&#xf002;</i> &nbsp;</asp:LinkButton>
                        </div>
                        <div class="col-md-1" style="margin-top: 36px">
                            <asp:LinkButton ID="btnrefresh" runat="server" OnClick="btnrefresh_Click" Width="100%" CssClass="btn btn-warning"><i style="color:white" class="fa">&#xf021;</i> &nbsp;</asp:LinkButton>
                        </div>
                        <%--<div class="table-responsive text-center">--%>
                        <div class="table ">
                            <asp:GridView ID="GVPurchase" runat="server" CellPadding="4" DataKeyNames="ID" PageSize="10" AllowPaging="true" Width="100%" OnRowDataBound="GVPurchase_RowDataBound"
                                OnRowCommand="GVPurchase_RowCommand" OnPageIndexChanging="GVPurchase_PageIndexChanging" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Invoice No" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblcustomername" runat="server" Text='<%# Eval("BillingCustomer") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoicedate" runat="server" Text='<%# Eval("Invoicedate","{0:d}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Referance No" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustomerPO" runat="server" Text='<%# Eval("CustomerPO") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%--         <asp:TemplateField HeaderText="Product" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblProduct" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <asp:TemplateField HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrandTotal" runat="server" Text='<%# Eval("Total_Price") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Prepared By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrepared" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%--  <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("IsPaid") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="ACTION" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>
                                            &nbsp;
                                            <asp:LinkButton ID="btnDelete" runat="server" Height="27px" ToolTip="Delete" CausesValidation="false" CommandName="RowDelete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-trash' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                            &nbsp;
                                            <asp:LinkButton runat="server" ID="btnpdfview" ToolTip="View Quotation PDF" CommandName="RowView" CommandArgument='<%# Eval("InvoiceNo") %>'><i class="fas fa-file-pdf"  style="font-size: 26px; color:red; "></i></i></asp:LinkButton>
                                              &nbsp;
                                            <asp:LinkButton ID="lnkSendmail" runat="server" Height="27px" ToolTip="Send Mail" CausesValidation="false" CommandName="RowSendmail" OnClientClick="Javascript:return confirm('Are you sure to send mail..?')" CommandArgument='<%#Eval("InvoiceNo")%>'><i class='fas fa-envelope' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                          
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GVPurchase" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

