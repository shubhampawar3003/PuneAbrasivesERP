<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="PaymentList.aspx.cs" Inherits="Admin_PaymentList" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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
                        <h2 class="mt-4"><b>PAYMENT LIST</b></h2>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-4">
                        <asp:Label ID="Label1" runat="server" Text="Company Name :"></asp:Label>
                        <div style="margin-top: 14px;">
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
                    <%--      <div class="col-md-4" runat="server" visible="false">
                        <asp:Label ID="Label2" runat="server" Text="Invoice No.:"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:DropDownList ID="ddlinvoice" CssClass="form-control" AutoPostBack="true" runat="server">
                            </asp:DropDownList>
                        </div>
                    </div>--%>

                  
                    <div class="col-md-1" style="margin-top: 36px">
                        <asp:LinkButton ID="btnrefresh" runat="server" OnClick="btnrefresh_Click" Width="100%" CssClass="btn btn-warning"><i style="color:white" class="fa">&#xf021;</i> &nbsp;</asp:LinkButton>
                    </div>

                    <div class="col-2" style="margin-top: 36px">
                        <asp:Button ID="btnCreate" CssClass="form-control btn btn-warning" OnClick="btnCreate_Click" runat="server" Text="Add Payment" />
                    </div>
                </div>

                <br />
                <div>
                    <div class="row">
                        <div class="table ">
                            <asp:GridView ID="GvCasHDetails" runat="server" CellPadding="4" DataKeyNames="Status,customername" PageSize="10" AllowPaging="true" Width="100%"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" OnPageIndexChanging="GvCasHDetails_PageIndexChanging" OnRowDataBound="GvCasHDetails_RowDataBound" OnRowCommand="GvCasHDetails_RowCommand">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="LblCustomerName" runat="server" Text='<%#Eval("customername")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--   <asp:TemplateField HeaderText="Invoice No" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoice" runat="server" Text='<%#Eval("InvoiceNo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Grand Total" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrandtotal" runat="server" Text='<%#Eval("grandtotal")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Recived Amount" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRecived" runat="server" Text='<%#Eval("recived")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pending Amount" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPending" runat="server" Text='<%#Eval("pending")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" Height="27px" CausesValidation="false" ToolTip="Edit Payment Details" CommandName="RowEdit" CommandArgument='<%# Container.DataItemIndex  %>' Visible='<%# Eval("Status").ToString() == "ISPAID" ? false : true %>' OnClientClick="return Check();"><i class='fas fa-edit' style='font-size:24px;color: #212529;' ></i></asp:LinkButton>
                                            <asp:LinkButton ID="btnpdf" runat="server" Height="27px" CausesValidation="false" CommandName="RowPDF" CommandArgument='<%# Container.DataItemIndex  %>' ToolTip="Download Payment Report"><i class="fa fa-file-pdf" aria-hidden="true" style="font-size:20px;color:red" ></i></asp:LinkButton>
                                            <asp:LinkButton ID="lnkview" runat="server" Height="27px" CausesValidation="false" CommandName="View" CommandArgument='<%# Container.DataItemIndex  %>' ToolTip="View All Payment Details"><i class="fa fa-eye" style="font-size:20px;color:black"></i></asp:LinkButton>

                                            <asp:LinkButton ID="btnDelete" Visible="false" runat="server" Height="27px" ToolTip="Delete" CausesValidation="false" CommandName="RowDelete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CommandArgument='<%# Container.DataItemIndex  %>'><i class='fas fa-trash' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>

                <br />
                <div class="table-responsive">
                    <asp:GridView ID="Gvreceipt" runat="server"
                        CssClass="table table-striped table-bordered nowrap" AutoGenerateColumns="false"
                        AllowPaging="false" ShowHeader="true" ShowFooter="true" PageSize="50">
                        <Columns>

                            <asp:TemplateField HeaderText="Invoice No." ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="txtinoiceno" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>
                                    <asp:Label runat="server" ID="lblmsgpaid" ForeColor="Red"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Bill No." ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="txtbillno" runat="server" Text='<%# Eval("BillNo") %>'></asp:Label>
                                    <asp:Label runat="server" ID="lblmsgpaid1" ForeColor="Red"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="txtinvoicedate" runat="server" Text='<%# Eval("Invoicedate", "{0:dd-MM-yyyy}") %>'></asp:Label>

                                    <asp:Label runat="server" ID="lblmsgpaid2" ForeColor="Red"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last Transaction Date" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="TransactionnDate" runat="server" Text='<%# Eval("LastTransactionDate", "{0:dd-MM-yyyy}") %>'></asp:Label>

                                    <asp:Label runat="server" ID="lblmsTrdate" ForeColor="Red"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="txtGrandtotal" runat="server" Text='<%# Eval("Totalammount") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="footerGrandtotal" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Received Amount" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="txtRecived" runat="server" Text='<%# Eval("ReceivedAmount") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="footerrecived" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Pending Amount" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="txtPending" runat="server" Text='<%# Eval("Pending") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="footerpending" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Paid Amount" ItemStyle-HorizontalAlign="Center" Visible="false">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtPaid" runat="server" Text='<%# Eval("Paid") %>' AutoPostBack="true"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Remarks" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%--                                    <asp:TextBox ID="Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:TextBox>--%>
                                    <asp:Label ID="txtPending" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>
                </div>
            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GvCasHDetails" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function Check() {
            var result = confirm("Are you sure you want to Edit ?");
            return result;
        }
    </script>
</asp:Content>

