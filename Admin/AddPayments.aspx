<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="AddPayments.aspx.cs" Inherits="Admin_AddPayments" %>

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
        .LblStyle {
            font-weight: 500;
            color: black;
        }

        .card_adj {
            margin-bottom: 3px;
            height: 35px;
        }
    </style>
    <!---Number--->
    <script>
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
    <style>
        .gvhead {
            text-align: center;
            color: #ffffff;
            background-color: #212529;
        }

        .lnk {
            font-weight: bolder;
            font-size: large;
        }

        .pagination-ys {
            /*display: inline-block;*/
            padding-left: 0;
            margin: 20px 0;
            border-radius: 4px;
        }

        .card_adj {
            margin-bottom: 3px;
            height: 35px;
        }

        /*pagination-ys table > tbody > tr > td {
        display: inline;
        }*/

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

        .spncls {
            color: red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-3">
                <div class="container-fluid px-3">
                    <h2 class="mt-4 ">Add Payments</h2>
                    <div class="card mb-4">
                        <div class="card-header LblStyle">
                            <i class="fa fa-inr"></i>
                            Add Payments                   
                        </div>
                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblcompanyname" class="form-label LblStyle">Customer Name : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtCustomerName" CssClass="form-control" placeholder="Search Customer" runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Name"
                                        ControlToValidate="txtCustomerName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                        TargetControlID="txtCustomerName">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblbilldate" class="form-label LblStyle">Transaction Date : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtTransactionDate" CssClass="form-control" TextMode="Date" placeholder="Enter Bill Date" runat="server"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                            </div>



                            <br />
                            <br />

                            <div class="table-responsive">
                                <div class="col-md-12" id="divtag" runat="server" visible="false">
                                    <marquee style="color: red; font: 100px">
  
    Please check at least one row checkbox or the header checkbox before submitting.
</marquee>

                                </div>
                                <asp:GridView ID="Gvreceipt" runat="server"
                                    AutoGenerateColumns="false"
                                    AllowPaging="false" ShowHeader="true" ShowFooter="true" CssClass="grivdiv pagination-ys" PageSize="50" OnRowDataBound="Gvreceipt_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">

                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chkheader" runat="server" OnCheckedChanged="chkheader_CheckedChanged" AutoPostBack="true" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkRow" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Payment Term" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtPayment" runat="server" Text='<%# Eval("PaymentTerm") %>'></asp:Label>
                                                <asp:Label runat="server" ID="lblmsgPayment" ForeColor="Red"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtinoiceno" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>
                                                <asp:Label runat="server" ID="lblmsgpaid" ForeColor="Red"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Bill No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtbillno" runat="server" Text='<%# Eval("BillNo") %>'></asp:Label>
                                                <asp:Label runat="server" ID="lblmsgpaid1" ForeColor="Red"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtinvoicedate" runat="server" Text='<%# Eval("Invoicedate") %>'></asp:Label>

                                                <asp:Label runat="server" ID="lblmsgpaid2" ForeColor="Red"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Last Transaction Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="TransactionnDate" runat="server" Text='<%# Eval("LastTransactionDate") %>'></asp:Label>

                                                <asp:Label runat="server" ID="lblmsTrdate" ForeColor="Red"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtGrandtotal" runat="server" Text='<%# Eval("Totalammount") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="footerGrandtotal" runat="server"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Received Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtRecived" runat="server" Text='<%# Eval("ReceivedAmount") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="footerrecived" runat="server"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Current Received Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtcurrentrecived" runat="server" Enabled="false"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Pending Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="txtPending" runat="server" Text='<%# Eval("Pending") %>'></asp:Label>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:Label ID="footerpending" runat="server"></asp:Label>
                                            </FooterTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Paid Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtPaid" OnTextChanged="txtPaid_TextChanged" runat="server" Text='<%# Eval("Paid") %>' AutoPostBack="true" placeholder="Enter Amount"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Remarks" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:TextBox ID="Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-md-4">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                    <asp:Label runat="server" ID="lblGrandtotal" ForeColor="Red"></asp:Label>
                                </div>
                                <div class="col-md-4"></div>
                            </div>
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblRecived" runat="server" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                </div>
                                <div class="col-md-4"></div>
                            </div>

                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblpending" runat="server" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                </div>
                                <div class="col-md-4"></div>
                            </div>

                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblpaid" runat="server" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                </div>
                                <div class="col-md-4"></div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btnsave" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" OnClick="btnsave_Click" ValidationGroup="check" OnClientClick="return chkAllValues();" />
                                    <%--<asp:Button ID="btnUpdate" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Update" OnClick="btnUpdate_Click" Visible="false" OnClientClick="return validateCheckBox();" />--%>
                                </div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btncancel" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" OnClick="btncancel_Click" />
                                </div>
                                <div class="col-md-4"></div>
                            </div>
                        </div>
                        <br />
                    </div>
                </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="Gvreceipt" />
            <asp:PostBackTrigger ControlID="btnsave" />
        </Triggers>
    </asp:UpdatePanel>
    <script>
        function chkAllValues() {
            var chkHeader = document.getElementById('<%= Gvreceipt.ClientID %>_chkheader'); // Replace "chkheader" with the actual ID of your header checkbox
            var gridView = document.getElementById('<%= Gvreceipt.ClientID %>');
            var chkRows = gridView.querySelectorAll('[id*=chkRow]');

            var headerChecked = chkHeader.checked;
            var rowChecked = Array.from(chkRows).some(function (chk) {
                return chk.checked;
            });

            if (!headerChecked && !rowChecked) {
                alert("Please check at least one row checkbox or the header checkbox before submitting.");
                return false;
            }

            return true;
        }
    </script>
</asp:Content>




