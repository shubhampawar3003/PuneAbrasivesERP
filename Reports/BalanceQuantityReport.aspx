<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="BalanceQuantityReport.aspx.cs" Inherits="Admin_BalanceQuantityReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />
    <script>
        function HideLabelerror(msg) {
            Swal.fire({
                icon: 'error',
                text: msg,

            })
        };
        function HideLabel(msg) {

            Swal.fire({
                icon: 'success',
                text: msg,
                timer: 5000,
                showCancelButton: false,
                showConfirmButton: false
            }).then(function () {
                window.location.href = "UserMaster.aspx";
            })
        };
    </script>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">&nbsp <b>BALANCE QUANTITY REPORT</b></h4>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-3">
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Company Name :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txtCustomerName" CssClass="form-control" placeholder="Search Company" runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                TargetControlID="txtCustomerName">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Product Name :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txtProductname" CssClass="form-control" placeholder="Search Product" runat="server" OnTextChanged="txtProductname_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetProductList"
                                TargetControlID="txtProductname">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="Label3" runat="server" Font-Bold="true" Text="P.O.Code :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txtPOCode" CssClass="form-control" placeholder="Search Company" runat="server" OnTextChanged="txtPOCode_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender3" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetPOCOdeList"
                                TargetControlID="txtPOCode">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-1" style="margin-top: 36px">
                        <asp:LinkButton ID="btnrefresh" runat="server" OnClick="btnrefresh_Click" Width="100%" CssClass="btn btn-warning"><i style="color:white" class="fa">&#xf021;</i> &nbsp;</asp:LinkButton>
                    </div>
                </div>
                <br />
                <div class="row">

                    <div class="table ">
                        <asp:GridView ID="GVBalance" runat="server" CellPadding="4" DataKeyNames="ID" PageSize="10" AllowPaging="true" Width="100%"
                            CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" OnPageIndexChanging="GVBalance_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="CustomerName" runat="server" Text='<%#Eval("CustomerName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="P.O. Code" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="POCODE" runat="server" Text='<%#Eval("Pono")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Product Name" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="PName" runat="server" Text='<%#Eval("Productname")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Balance Quantity" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="Balance" runat="server" Text='<%#Eval("Balance")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
