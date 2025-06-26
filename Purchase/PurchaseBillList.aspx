<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="PurchaseBillList.aspx.cs" Inherits="Purchase_PurchaseBillList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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
        /*Bell Icon Css*/
        @keyframes bellBounce {
            0% {
                transform: translateX(0);
            }

            25% {
                transform: translateX(-5px);
            }

            50% {
                transform: translateX(5px);
            }

            75% {
                transform: translateX(-5px);
            }

            100% {
                transform: translateX(0);
            }
        }

        .bell-bounce {
            animation: bellBounce 1s ease-in-out infinite;
        }



        /*New CSS*/
        .panelinward {
            border: 1px solid darkgray;
            width: 55%;
            padding: 20px !important;
            background-color: whitesmoke;
            left: 378px !important;
        }

        .floa {
            float: right;
        }

        .lbl {
            font-weight: bold;
        }

        .btnclose {
            margin-top: -2%;
        }


        .auto-style1 {
            margin-left: 11px;
        }

        .completionList {
            border: solid 1px Gray;
            border-radius: 5px;
            margin: 0px;
            padding: 3px;
            7 height: 200px;
            overflow: auto;
            width: 500px;
            background-color: #FFFFFF;
            font-size: 16px;
        }

        .listItem {
            color: #191919;
        }

        .itemHighlighted {
            background-color: #ADD6FF;
            font-weight: 900;
        }

        .active1 {
            float: right;
            margin-right: 80px
        }
    </style>
    <script>    
        function ReloadPage() {
            window.location.href = "../Purchase/PurchaseBillList.aspx";
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <div class="container-fluid px-4">
        <div class="col-md-12">
            <div class="card-header py-3 d-flex flex-row align-items-center justify-content-between" style="background-color: transparent;">
                <h4 class="m-0 font-weight-bold text-dark"><b>PURCHASE BILL LIST</b></h4>

                <asp:LinkButton class="nav-link position-relative" runat="server" ID="lnkshow" role="button" aria-expanded="false" OnClick="lnkshow_Click">
                    <i class="fas fa-bell" style="font-size: 30px;"></i>
                    <span class="badge bg-danger position-absolute top-0 start-100 translate-middle" style="font-size: 0.8rem;">
                        <asp:Label ID="lblcount" runat="server" Text="0" CssClass="text-white"></asp:Label>
                    </span>
                </asp:LinkButton>
            </div>
        </div>
        <div class="row">
            <hr />
            <div class="col-xl-3 col-md-3">
                <asp:TextBox ID="txtcnamefilter" runat="server" CssClass="form-control" placeholder="Supplier name" Width="100%" OnTextChanged="txtcnamefilter_TextChanged" AutoPostBack="true"></asp:TextBox>
                <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                    CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                    CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList"
                    TargetControlID="txtcnamefilter">
                </asp:AutoCompleteExtender>
            </div>
            <div class="col-xl-3 col-md-3">
                <asp:TextBox ID="txtSupplierBill" runat="server" CssClass="form-control" placeholder="Supplier Bill No." Width="100%" OnTextChanged="txtSupplierBill_TextChanged" AutoPostBack="true"></asp:TextBox>
                <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                    CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                    CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetbillnoList"
                    TargetControlID="txtSupplierBill">
                </asp:AutoCompleteExtender>
            </div>
            <div class="col-xl-1 col-md-1">
                <asp:Button ID="btnresetfilter" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" OnClick="btnresetfilter_Click" />
            </div>
            <div class="col-xl-3 col-md-3">
                <asp:LinkButton ID="LinkButton1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton1_Click">
                 <i class="fas fa-file-alt"></i>&nbsp&nbsp Add Purchase Bill
                </asp:LinkButton>
            </div>
            <div class="col-xl-1 col-md-1">
            </div>
        </div>
        <asp:GridView ID="GvPurchaseBill" runat="server" Width="100%"
            CssClass="grivdiv pagination-ys" AutoGenerateColumns="false"
            DataKeyNames="id" OnRowCommand="GvPurchaseBill_RowCommand" OnRowDataBound="GvPurchaseBill_RowDataBound" AllowPaging="false" ShowHeader="true" PageSize="50">
            <Columns>
                <asp:TemplateField HeaderText="SNo." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" Visible="false" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Bill No" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblBillNo" runat="server" Text='<%# Eval("BillNo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Supplier Name" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblsname" runat="server" Text='<%# Eval("SupplierName") %>'></asp:Label>
                        <%-- <asp:LinkButton ID="linksname" runat="server" CssClass="linkbtn" CommandName="Suppliername" Text='<%# Eval("SupplierName") %>' CommandArgument='<%# Eval("Id") %>' ToolTip="View Details"></asp:LinkButton>--%>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Supplier BillNo" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblSupBillNo" runat="server" Text='<%# Eval("SupplierBillNo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Bill Date" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblBillDate" runat="server" Text='<%# Eval("BillDate") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="DateOfReceived" HeaderStyle-CssClass="gvhead" HeaderText="Receiving Date" DataFormatString="{0:dd-MM-yyyy}" />


                <asp:TemplateField HeaderText="Total Amt" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblGrandTotal" runat="server" Text='<%# Eval("GrandTotal") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Due Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblPaymentDueDate" runat="server" Text='<%# Eval("PaymentDueDate").ToString().TrimEnd("0:0".ToCharArray()) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Created By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                    <ItemTemplate>
                        <asp:Label ID="lblCreatedBy" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
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
                        <asp:LinkButton runat="server" ID="btnDelete" CommandName="RowDelete" CommandArgument='<%# Eval("id") %>' ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')"><i class="fa fa-trash" aria-hidden="true" style="font-size:24px;"></i></asp:LinkButton>

                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>


    <%-- New Code for PopUP --%>
    <asp:Button ID="btnprof" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="modelprofile" runat="server" TargetControlID="btnprof"
        PopupControlID="PopupAddDetail" OkControlID="Closepopdetail" BackgroundCssClass="modalBackground" />

    <asp:Panel ID="PopupAddDetail" runat="server" class="w3-panel w3-white panelinward m-0 font-weight-bold text-primary"
        Direction="LeftToRight" Wrap="true" Style="display: none;">

        <div class="d-flex justify-content-between align-items-center">
            <div class="cpl-md-12">
                <h4 class="m-0 font-weight-bold text-dark"><b>Pending Inward Entries</b></h4>
            </div>
            <asp:LinkButton ID="Closepopdetail" runat="server" OnClientClick="ReloadPage()">
               <i class="fas fa-close" style="font-size:27px;color:red;"></i>
            </asp:LinkButton>
        </div>
        <br />
        <div class="row">
            <div class="table-container" style="height: 300px; overflow-y: auto;">
                <asp:GridView ID="gv_EstimationList" runat="server" AutoGenerateColumns="False"
                    CellPadding="3" CssClass="custom-grid" AllowPaging="false" HeaderStyle-BackColor="Black"
                    HeaderStyle-HorizontalAlign="Center" OnRowCommand="gv_EstimationList_RowCommand" RowStyle-HorizontalAlign="Center">

                    <Columns>
                        <asp:TemplateField HeaderText="Sr. No." HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="Label28" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Order No" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblOrderNo" runat="server" Text='<%# Eval("OrderNo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Purchase Order No" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblcustomerName" runat="server" Text='<%# Eval("Pono") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                            <HeaderStyle Width="260px" />
                            <ItemStyle Width="260px" />
                            <ItemTemplate>
                                <asp:Label ID="Label30" runat="server" Text='<%# Eval("SupplierName") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice No" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="Label29" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Action" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lnkbtnCorrect" ToolTip="Correct"
                                    CommandArgument='<%# Eval("OrderNo") %>' CommandName="RowCorrect"
                                    CausesValidation="False">
                                   <%--OnClick="lnkbtnCorrect_Click"--%>
                                   <i class="fa fa-check" style="font-size:24px; color:green"></i>
                                </asp:LinkButton>
                            </ItemTemplate>

                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <RowStyle ForeColor="#000066" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>
    <%-- Old code --%>


    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
</asp:Content>

