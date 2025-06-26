<%@ Page Title="" Debug="true" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="Receipt.aspx.cs" Inherits="Account_Receipt" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

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

        .gvtxtwidth {
            width: 60px;
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
        .row {
            margin-top: 10px;
        }
    </style>
    <script type='text/javascript'>
        function scrollToElement() {
            var target = document.getElementById("divdtls").offsetTop;
            window.scrollTo(0, target);
        }
    </script>
    <style type="text/css">
        .cal_Theme1 .ajax__calendar_container {
            background-color: #DEF1F4;
            border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_header {
            background-color: #ffffff;
            margin-bottom: 4px;
        }

        .cal_Theme1 .ajax__calendar_title,
        .cal_Theme1 .ajax__calendar_next,
        .cal_Theme1 .ajax__calendar_prev {
            color: #004080;
            padding-top: 3px;
        }

        .cal_Theme1 .ajax__calendar_body {
            background-color: #ffffff;
            border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_dayname {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            color: #004080;
        }

        .cal_Theme1 .ajax__calendar_day {
            color: #004080;
            text-align: center;
        }

        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_day,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_month,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_year,
        .cal_Theme1 .ajax__calendar_active {
            color: #004080;
            font-weight: bold;
            background-color: #DEF1F4;
        }

        .cal_Theme1 .ajax__calendar_today {
            font-weight: bold;
            font-size: 10px;
        }

        .cal_Theme1 .ajax__calendar_other,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_today,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_title {
            color: #bbbbbb;
        }

        .ajax__calendar_body {
            height: 158px !important;
            width: 220px !important;
            position: relative;
            overflow: hidden;
            margin: 0 0 0 -5px !important;
        }

        .ajax__calendar_container {
            padding: 4px;
            cursor: default;
            width: 220px !important;
            font-size: 11px;
            text-align: center;
            font-family: tahoma,verdana,helvetica;
        }

        .cal_Theme1 .ajax__calendar_day {
            color: #004080;
            font-size: 14px;
            text-align: center;
        }

        .ajax__calendar_day {
            height: 22px !important;
            width: 27px !important;
            text-align: right;
            padding: 0 14px !important;
            cursor: pointer;
        }

        .cal_Theme1 .ajax__calendar_dayname {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            margin-left: 12px !important;
            color: #004080;
        }

        .ajax__calendar_year {
            height: 50px !important;
            width: 51px !important;
            font-weight: bold;
            text-align: center;
            cursor: pointer;
            overflow: hidden;
            color: #004080;
        }

        .ajax__calendar_month {
            height: 50px !important;
            width: 51px !important;
            text-align: center;
            font-weight: bold;
            cursor: pointer;
            overflow: hidden;
            color: #004080;
        }

        .grid tr:hover {
            background-color: #d4f0fa;
        }

        .pcoded[theme-layout="vertical"][vertical-nav-type="expanded"] .pcoded-header .pcoded-left-header, .pcoded[theme-layout="vertical"][vertical-nav-type="expanded"] .pcoded-navbar {
            width: 210px;
        }
    </style>
    <script>
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && charCode < 46 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepnl" runat="server">
        <ContentTemplate>
            <div class="page-wrapper">
                <div class="page-body">

                    <div class="container py-3">
                        <div class="card">
                            <div class="card-header text-uppercase text-black">
                                <div class="row">
                                    <div class="col-10 col-md-10">
                                        <h5>ADD RECEIPT</h5>
                                    </div>
                                    <div class="col-2 col-md-2">
                                        <asp:Button ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" runat="server" Text="Receipt List" OnClick="Button1_Click" />
                                    </div>
                                </div>
                            </div>
                            <asp:HiddenField ID="hidden" runat="server" />
                            <asp:HiddenField ID="hidden1" runat="server" />
                            <div class="row">
                                <div class="col-xl-12 col-md-12">
                                    <div class="card-header">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Party Name</b><i class="reqcls">*&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtPartyName" CssClass="form-control" OnTextChanged="txtPartyName_TextChanged" runat="server" Width="100%" AutoPostBack="true"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Party Name"
                                                            ControlToValidate="txtPartyName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetcustomerList"
                                                            TargetControlID="txtPartyName">
                                                        </asp:AutoCompleteExtender>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Payment Date</b><i class="reqcls">*&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtdate" CssClass="form-control" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>
                                                        <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtdate" Format="dd-MM-yyyy" CssClass="cal_Theme1" runat="server"></asp:CalendarExtender>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ErrorMessage="Please Enter Date"
                                                            ControlToValidate="txtdate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>To Account Name</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddltoaccountName" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddltoaccountName_SelectedIndexChanged">
                                                            <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                                            <asp:ListItem Text="Bank"></asp:ListItem>
                                                            <asp:ListItem Text="CC"></asp:ListItem>
                                                            <asp:ListItem Text="OD"></asp:ListItem>
                                                            <asp:ListItem Text="Petty Cash"></asp:ListItem>
                                                        </asp:DropDownList>

                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Bank Name</b><i class="reqcls">*&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtbankname" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                        <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetBankList"
                                                            TargetControlID="txtbankname">
                                                        </asp:AutoCompleteExtender>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Please Enter Bank Name"
                                                            ControlToValidate="txtbankname" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                    </div>

                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Transaction Mode</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddltransactionmode" runat="server" CssClass="form-control">
                                                            <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                                            <asp:ListItem Text="Ac Cheque"></asp:ListItem>
                                                            <asp:ListItem Text="Bearer Cheque"></asp:ListItem>
                                                            <asp:ListItem Text="DD"></asp:ListItem>
                                                            <asp:ListItem Text="RTGS"></asp:ListItem>
                                                            <asp:ListItem Text="IMPS"></asp:ListItem>
                                                            <asp:ListItem Text="NEFT"></asp:ListItem>
                                                            <asp:ListItem Text="Cash"></asp:ListItem>
                                                            <asp:ListItem Text="Other"></asp:ListItem>

                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Mode Description</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtmodedescription" CssClass="form-control" TextMode="MultiLine" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Against</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddlAgainst" runat="server" CssClass="form-control" OnTextChanged="ddlAgainst_TextChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                                            <asp:ListItem Text="Invoice"></asp:ListItem>
                                                            <asp:ListItem Text="Advance"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <%--  <div class="col-md-2">
                                                        <asp:DropDownList ID="ddlInvoiceNo" runat="server" CssClass="form-control" OnTextChanged="ddlInvoiceNo_TextChanged" AutoPostBack="true">
                                                        </asp:DropDownList>
                                                    </div>--%>
                                                    <div class="col-md-2 spancls"><b>Amount</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtamount" onkeypress="return isNumberKey(event)" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txtamount_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Amount"
                                                            ControlToValidate="txtamount" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Transaction Remark</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtremark" CssClass="form-control" TextMode="MultiLine" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Against Ref. No.</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">

                                                        <asp:TextBox ID="txtagainstpi" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                        <asp:Label ID="lblpendingAdvance" runat="server" Style="color: Red; font-weight: bolder!important; font-size: large;"></asp:Label>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>TDS</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="txttds" CssClass="form-control" runat="server" Width="100%" OnSelectedIndexChanged="txttds_SelectedIndexChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                            <asp:ListItem Value="0.0085">0.0085</asp:ListItem>
                                                            <asp:ListItem Value="0.01">0.01</asp:ListItem>
                                                            <asp:ListItem Value="0.1">0.1</asp:ListItem>
                                                            <asp:ListItem Value="0.75">0.75</asp:ListItem>
                                                            <asp:ListItem Value="0.76">0.76</asp:ListItem>
                                                            <asp:ListItem Value="0.9">0.9</asp:ListItem>
                                                            <asp:ListItem Value="1">1</asp:ListItem>
                                                            <asp:ListItem Value="1">1</asp:ListItem>
                                                            <asp:ListItem Value="1.01">1.01</asp:ListItem>
                                                            <asp:ListItem Value="1.04">1.04</asp:ListItem>
                                                            <asp:ListItem Value="1.5">1.5</asp:ListItem>
                                                            <asp:ListItem Value="1.6">1.6</asp:ListItem>
                                                            <asp:ListItem Value="1.7">1.7</asp:ListItem>
                                                            <asp:ListItem Value="1.9">1.9</asp:ListItem>
                                                            <asp:ListItem Value="2">2</asp:ListItem>
                                                            <asp:ListItem Value="3">3</asp:ListItem>
                                                            <asp:ListItem Value="3.161">3.161</asp:ListItem>
                                                            <asp:ListItem Value="4">4</asp:ListItem>
                                                            <asp:ListItem Value="5">5</asp:ListItem>
                                                            <asp:ListItem Value="5.5">5.5</asp:ListItem>
                                                            <asp:ListItem Value="6">6</asp:ListItem>
                                                            <asp:ListItem Value="6.5">6.5</asp:ListItem>
                                                            <asp:ListItem Value="7">7</asp:ListItem>
                                                            <asp:ListItem Value="7.5">7.5</asp:ListItem>
                                                            <asp:ListItem Value="8">8</asp:ListItem>
                                                            <asp:ListItem Value="9">9</asp:ListItem>
                                                            <asp:ListItem Value="10">10</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>On</b><i class="reqcls">&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtbasic" CssClass="form-control" runat="server" Width="100%" ReadOnly="true" Text="Basic"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <br />
                                                <div class="table-responsive">
                                                    <asp:GridView ID="Gvreceipt" runat="server"
                                                        CssClass="table table-striped table-bordered nowrap" AutoGenerateColumns="false"
                                                        AllowPaging="false" ShowHeader="true" ShowFooter="true" PageSize="50" OnRowDataBound="Gvreceipt_RowDataBound" DataKeyNames="Id">
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="chkheader" runat="server" OnCheckedChanged="chkheader_CheckedChanged" AutoPostBack="true" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkRow" runat="server" OnCheckedChanged="chkRow_CheckedChanged" AutoPostBack="true" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Invoice No" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblinvoiceno" runat="server" Text='<%# Eval("Invoiceno") %>'></asp:Label>
                                                                    <asp:Label runat="server" ID="lblmsgpaid" ForeColor="Red"></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblInvoiceDate" runat="server" Text='<%# Eval("Invoicedate","{0:d}") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="FinalBasic" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblfinalbasic" runat="server" Text='<%# Eval("Basic") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Payable" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblpayable" runat="server" Text='<%# Eval("GrandTotalFinal") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footerpayble" runat="server"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Recvd" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="lblratee" runat="server" Enabled="false" CssClass="gvtxtwidth" Text="0"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="WO" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="lblWO" runat="server" CssClass="gvtxtwidth" Text="0" Enabled="false"></asp:TextBox>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Paid" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtgvpaid" onkeypress="return isNumberKey(event)" runat="server" Enabled="false" CssClass="gvtxtwidth" OnTextChanged="txtgvpaid_TextChanged" AutoPostBack="true" Text="0"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footerpaid" runat="server" Visible="true"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="TDS" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtgvTDS" onkeypress="return isNumberKey(event)" ReadOnly="true" runat="server" CssClass="gvtxtwidth" Text="0"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footertds" runat="server" Visible="false"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Adjust" ItemStyle-HorizontalAlign="Center" Visible="false" >
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtgvadjust" onkeypress="return isNumberKey(event)" runat="server" CssClass="gvtxtwidth" Enabled="false" OnTextChanged="txtgvadjust_TextChanged" AutoPostBack="true" Text="0"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footeradjust" runat="server" Visible="false" ></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Excess" ItemStyle-HorizontalAlign="Center" Visible="false" >
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtgvExcess" onkeypress="return isNumberKey(event)" runat="server" OnTextChanged="txtgvExcess_TextChanged" CssClass="gvtxtwidth" Enabled="false" Text="0" AutoPostBack="true"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footerexcess" runat="server" Visible="false"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Pending" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtgvpending" onkeypress="return isNumberKey(event)" runat="server" CssClass="gvtxtwidth" Enabled="false" Text="0"></asp:TextBox>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footerpending" runat="server" Visible="false"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Total" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lbltotal" runat="server" CssClass="gvtxtwidth" Text="0"></asp:Label>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    <asp:Label ID="footertotal" runat="server" Visible="false"></asp:Label>
                                                                </FooterTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Note" ItemStyle-HorizontalAlign="Center">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtgvNote" TextMode="MultiLine" Rows="5" Width="100" runat="server" CssClass="gvtxtwidth" Enabled="false"></asp:TextBox>
                                                                </ItemTemplate>

                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-4">
                                                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                                        <asp:Label runat="server" ID="lblmsg" ForeColor="Red"></asp:Label>
                                                    </div>
                                                    <div class="col-md-4"></div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-4">
                                                        <asp:Label ID="lblshowmsg" runat="server" Font-Bold="true" Font-Size="Larger"></asp:Label>
                                                    </div>
                                                    <div class="col-md-4"></div>
                                                </div>
                                                <div class="row">

                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-2">
                                                        <asp:Button ID="btnsubmit" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Width="100%" Text="Submit" OnClick="btnsubmit_Click" />
                                                    </div>
                                                    <div class="col-md-2">
                                                        <asp:Button ID="btnreset" runat="server" CssClass="btn btn-danger" Width="100%" Text="Reset" OnClick="btnreset_Click" />
                                                    </div>
                                                    <div class="col-md-4"></div>
                                                </div>
                                                <asp:Label runat="server" ID="lblFooterPaidVal" Visible="false"></asp:Label>
                                            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

