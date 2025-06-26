<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="PurchaseOrderList.aspx.cs" Inherits="Purchase_PurchaseOrderList" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />

    <style>
        .spancls {
            color: #5d5656 !important;
            font-size: 15px !important;
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
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="col-md-12">                 
                    <div class="row">
                        <div class="col-md-10">
                            <h4 class="mt-4">&nbsp <b>PURCHASE ORDER LIST</b></h4>
                        </div>
                        <div class="col-md-2 mt-4">
                            <asp:LinkButton ID="LinkButton1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton1_Click">
    <i class="fas fa-file-alt"></i>&nbsp&nbsp Add P.O.
                            </asp:LinkButton>
                        </div>
                    </div>
                    <hr />
                </div>
               
                <div class="row">
                    <div class="col-xl-3 col-md-3">
                        <asp:TextBox ID="txtcnamefilter" runat="server" CssClass="form-control" placeholder="Supplier name" Width="100%" OnTextChanged="txtcnamefilter_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList"
                            TargetControlID="txtcnamefilter">
                        </asp:AutoCompleteExtender>
                    </div>
                    <div class="col-xl-3 col-md-3">
                        <asp:TextBox ID="txtPONo" runat="server" CssClass="form-control" placeholder="Enter PO NO" Width="100%" OnTextChanged="txtPONo_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetPOList"
                            TargetControlID="txtPONo">
                        </asp:AutoCompleteExtender>
                    </div>
                    <div class="col-xl-2 col-md-2">
                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-control" OnTextChanged="ddlStatus_TextChanged" AutoPostBack="true">
                            <asp:ListItem>--Select--</asp:ListItem>
                            <asp:ListItem Value="0">All</asp:ListItem>
                            <asp:ListItem Value="Open">Open</asp:ListItem>
                            <asp:ListItem Value="Close">Close</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-xl-1 col-md-1">
                        <asp:Button ID="btnresetfilter" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" OnClick="btnresetfilter_Click" />
                    </div>
                </div>
                <div class="row">

                    <asp:GridView ID="GvPurchaseOrder" runat="server" CellPadding="4"
                        CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" llowPaging="true" Width="100%"
                        DataKeyNames="id" OnRowCommand="GvPurchaseOrder_RowCommand" AllowPaging="false" ShowHeader="true" PageSize="10" OnRowDataBound="GvPurchaseOrder_RowDataBound">
                        <Columns>
                            <asp:TemplateField HeaderText="SNo." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="PO NO" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblScode" runat="server" Text='<%# Eval("PONo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="linksname" runat="server" Text='<%# Eval("SupplierName") %>'></asp:Label>
                                    <%-- <asp:LinkButton ID="linksname" runat="server" CssClass="linkbtn" CommandName="Suppliername" Text='<%# Eval("SupplierName") %>' CommandArgument='<%# Eval("Id") %>' ToolTip="View Details"></asp:LinkButton>--%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="PO Date" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblPODate" runat="server" Text='<%# Convert.ToDateTime(Eval("PODate")).ToString("dd-MM-yyyy").TrimEnd("0:0".ToCharArray()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Delivery Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblDeliveryDate" runat="server" Text='<%# Convert.ToDateTime(Eval("DeliveryDate")).ToString("dd-MM-yyyy").TrimEnd("0:0".ToCharArray()) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Refer Quotation" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblReferQuotation" runat="server" Text='<%# Eval("ReferQuotation") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblMode" runat="server" Text='<%# Eval("Mode") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Created By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%# Eval("CreatedBy") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Ref Doc" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButtonfile1" ImageUrl="../Content/img/Open-file2.png" runat="server" Width="30px" OnClick="linkbtnfile_Click" CommandArgument='<%# Eval("Id") %>' ToolTip="Open File" />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <%--<asp:Button ID="Button4" CssClass="btn" runat="server" Text="Edit" Style="background-color: #09989a !important; color: #fff;" CommandName="RowEdit" CommandArgument='<%# Eval("Id") %>' />--%>
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="RowEdit" CommandArgument='<%# Eval("Id") %>' ToolTip="Edit"><i class="fa fa-edit" style="font-size:24px;color:black;"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnPDF" runat="server" CommandName="DownloadPDF" CommandArgument='<%# Eval("Id") %>' ToolTip="Download"><i class="fa fa-file-pdf" style="font-size:24px;color:red;"></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CommandName="RowDelete" CommandArgument='<%# Eval("id") %>' ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')"><i class="fa fa-trash" aria-hidden="true" style="font-size:24px;"></i></asp:LinkButton>


                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>



                </div>
            </div>
             <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

