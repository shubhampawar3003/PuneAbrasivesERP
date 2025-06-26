<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="PaymentList.aspx.cs" Inherits="Purchase_PaymentList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <style>
        .gvhead {
            text-align: center;
            color: #ffffff;
            background-color: #260b1a;
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

    <div class="page-wrapper">
        <div class="page-body">

            <div class="container py-3">
                <div class="card">
                    <div class="card-header text-uppercase text-black">
                        <div class="row">
                            <div class="col-10 col-md-10">
                                <h5>Payment List</h5>
                            </div>
                            <div class="col-2 col-md-2">
                                <asp:Button ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" runat="server" Text="Add Payment" OnClick="Button1_Click" />
                            </div>

                        </div>


                    </div>
                    <div class="row">
                        <div class="col-xl-12 col-md-12">


                            <div class="row">
                                <div class="col-xl-3 col-md-3">
                                    <asp:TextBox ID="txtpartyname" runat="server" CssClass="form-control" placeholder="Party Name" Width="100%" AutoPostBack="true" OnTextChanged="txtpartyname_TextChanged"></asp:TextBox>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetPartyList"
                                        TargetControlID="txtpartyname">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-xl-1 col-md-1">
                                    <asp:Button ID="btnresetfilter" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" OnClick="btnresetfilter_Click" />
                                </div>
                            </div>
                            <div class="col-md-12" style="padding: 0px; margin-top: 10px;">
                                <div id="DivRoot" align="left">
                                    <div style="overflow: hidden;" id="DivHeaderRow">
                                    </div>
                                    <div style="overflow: scroll;" class="dt-responsive table-responsive" onscroll="OnScrollDiv(this)" id="DivMainContent">
                                        <asp:GridView ID="GvPayment" runat="server"
                                            CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" Width="100%"
                                            AllowPaging="false" ShowHeader="true" DataKeyNames="Id" PageSize="10" OnRowCommand="GvPayment_RowCommand" OnRowDataBound="GvPayment_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="SNo." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="No" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblId" runat="server" Text='<%# Eval("Id") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="From - To" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblbankName" runat="server" Text='<%# Eval("BankName") %>'></asp:Label><br />
                                                        <asp:Label ID="lblpartyname" runat="server" Text='<%# Eval("PartyName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmount" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblpostDate" runat="server" Text='<%# Eval("PostDate","{0:d}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Mode" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTransactionMode" runat="server" Text='<%# Eval("TransactionMode") %>'></asp:Label><br />
                                                        <asp:Label ID="lblModeDescription" runat="server" Text='<%# Eval("ModeDescription") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Against" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAgainst" runat="server" Text='<%# Eval("Against") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Transaction By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTransactionBy" runat="server" Text='<%# Eval("CreatedBy") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnEdit" runat="server" CommandName="RowEdit" CommandArgument='<%# Eval("Id") %>' ToolTip="Edit"><i class="fa fa-edit" style="font-size:24px;color:black;"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="btnPDF" runat="server" CommandName="DownloadPDF" CommandArgument='<%# Eval("Id") %>' ToolTip="Download"><i class="fa fa-file-pdf" style="font-size:24px;color:red;"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="RowDelete" CommandArgument='<%# Eval("Id") %>' ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')"><i class="fa fa-trash" aria-hidden="true" style="font-size:24px;"></i></asp:LinkButton>

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <div id="DivFooterRow" style="overflow: hidden">
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <%-- </div>--%>

                            <br />
                            <br />

                        </div>
                    </div>
                    <br />
                </div>
            </div>
        </div>
    </div>
</asp:Content>

