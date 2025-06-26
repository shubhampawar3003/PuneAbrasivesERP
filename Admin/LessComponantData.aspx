<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LessComponantData.aspx.cs" MasterPageFile="~/Admin/WLSPLMaster.master" Inherits="Admin_LessComponantData" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />

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
                window.location.href = "Login.aspx";
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
                        <h4 class="mt-4 "><b>LESS QUANTITY LIST</b></h4>
                    </div>
                    <div class="col-md-2 mt-4">
                        <asp:LinkButton ID="Button1" CssClass="form-control btn btn-info" Font-Bold="true" CausesValidation="false" runat="server" OnClick="Button1_Click">
    <i class="fa fa-arrow-circle-left"></i>&nbsp Back
                        </asp:LinkButton>

                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">

                        <div style="margin-top: 14px;">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Component Name :"></asp:Label>
                            <asp:TextBox ID="txtproduct" CssClass="form-control" placeholder="Search Component" runat="server" OnTextChanged="txtproduct_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetProductList"
                                TargetControlID="txtproduct">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>

                    <div class="col-md-1" style="margin-top: 36px">
                        <asp:LinkButton ID="btnrefresh" runat="server" OnClick="btnrefresh_Click" Width="100%" CssClass="btn btn-warning"><i style="color:white" class="fa">&#xf021;</i> &nbsp;</asp:LinkButton>
                    </div>
                </div>

                <br />
                <div>
                    <div class="row">
                        <%--<div class="table-responsive text-center">--%>
                        <div class="table ">
                            <asp:GridView ID="GVInentory" runat="server" CellPadding="4" PageSize="10" AllowPaging="true" Width="100%"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" OnPageIndexChanging="GVInentory_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Component Name" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Productname" runat="server" Text='<%#Eval("ComponentName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--    <asp:TemplateField HeaderText="Batch" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Batch" runat="server" Text='<%#Eval("Batch")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <%--<asp:TemplateField HeaderText="Inward Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="IQuantity" runat="server" Text='<%#Eval("InwardQty")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Outward Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="OQuantity" runat="server" Text='<%#Eval("OutwardQty")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="PQuantity" runat="server" Text='<%#Eval("BalanceQty")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--      <asp:TemplateField HeaderText="Rate" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Rate" runat="server" Text='<%#Eval("Rate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <%--    <asp:TemplateField HeaderText="Total" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Total" runat="server" Text='<%#Eval("Total")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="All total" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Alltotal" runat="server" Text='<%#Eval("Alltotal")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>

                                    <%--     <asp:TemplateField HeaderText="GST No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="GSTno" runat="server" Text='<%#Eval("GSTno")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>   --%>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="Button1" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
