<%@ Page Title="" Debug="true" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="TaxInvoice.aspx.cs" Inherits="Account_TaxInvoice" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        span.select2.select2-container.select2-container--default.select2-container--focus {
            max-width: 100% !important;
        }

        .select2-container {
            box-sizing: border-box;
            display: inline-block;
            margin: 0;
            position: relative;
            vertical-align: middle;
            width: 100% !important;
        }
    </style>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=ddlProduct]").select2();


        });
    </script>
    <script type='text/javascript'>
        function scrollToElement() {
            var target = document.getElementById("divdtls").offsetTop;
            window.scrollTo(0, target);
        }
    </script>
    <script type='text/javascript'>
        function scrollToElement() {
            var target = document.getElementById("divdtls1").offsetTop;
            window.scrollTo(0, target);
        }
    </script>
    <style>
        .uppercase {
            text-transform: uppercase;
        }

        .spancls {
            color: #1a1616 !important;
            font-size: 15px !important;
            font-weight: bold;
            text-align: left;
        }

        .starcls {
            color: red;
            font-size: 18px;
            font-weight: 700;
        }

        .row {
            margin-top: 5px;
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
            border: solid 1px #444444;
            margin: 0px;
            padding: 2px;
            height: 100px;
            overflow: auto;
            background-color: #FFFFFF;
        }

        .listItem {
            color: #1C1C1C;
        }

        .itemHighlighted {
            background-color: #ffc0c0;
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
    <script>
        $(document).ready(function () {
            $('#<%= txtvehicalNumber.ClientID %>').on('input', function () {
                $(this).val($(this).val().toUpperCase());
            });
            $('#<%= txtByAir.ClientID %>').on('input', function () {
                $(this).val($(this).val().toUpperCase());
            });
            $('#<%= txtByHand.ClientID %>').on('input', function () {
                $(this).val($(this).val().toUpperCase());
            });
        });
    </script>


    <style>
        /* Loader CSS */
        .loader-wrapper {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.8);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 1000;
            /* Ensure it appears above other content */
            display: none;
            /* Hidden by default */
        }

        .loader {
            border: 8px solid #f3f3f3;
            /* Light grey */
            border-top: 8px solid #3498db;
            /* Blue */
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
        }

        @keyframes spin {
            0% {
                transform: rotate(0deg);
            }

            100% {
                transform: rotate(360deg);
            }
        }
    </style>
    <script type="text/javascript">
        function showLoader() {
            document.getElementById('loader').style.display = 'flex';
        }

        function hideLoader() {
            document.getElementById('loader').style.display = 'none';
        }

        document.onreadystatechange = function () {
            if (document.readyState === "complete") {
                hideLoader();
            }
        };
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepnl" runat="server">
        <ContentTemplate>
            <div class="page-wrapper">
                <div class="page-body">
                    <div class="card">
                        <div class="card-header text-uppercase text-black">
                            <div class="row">
                                <div class="col-10 col-md-10">
                                    <h5>CREATE TAX INVOICE</h5>
                                </div>
                                <div class="col-2 col-md-2">
                                    <asp:Button ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" runat="server" Text="Tax Invoice List" OnClientClick="showLoader();" OnClick="Button1_Click" />
                                </div>
                            </div>


                        </div>

                        <div class="container py-3">
                            <div class="card">
                                <div class="row">
                                    <div class="col-xl-12 col-md-12">
                                        <div class="card-header">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <asp:HiddenField ID="hiddeninvoiceno" runat="server" />
                                                    <asp:HiddenField ID="hidden1" runat="server" />
                                                     <asp:HiddenField ID="hdnUsername" runat="server" />
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Invoice No.<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtinvoiceno" CssClass="form-control" ReadOnly="true" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>

                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Invoice Type<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:DropDownList runat="server" ID="ddlInvoiceType" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlInvoiceType_SelectedIndexChanged">
                                                                <asp:ListItem Text="Regular"></asp:ListItem>
                                                                <asp:ListItem Text="Export"></asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                        <div class="col-md-2 spancls">Invoice Date<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtinvoicedate" CssClass="form-control" ReadOnly="true" runat="server" TextMode="Date" Width="100%" AutoComplete="off"></asp:TextBox>

                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Please Enter Date"
                                                                ControlToValidate="txtinvoicedate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Billing Customer<i class="reqcls">*&nbsp;</i> : </div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtbillingcustomer" CssClass="form-control" placeholder="Search Customer Name" runat="server" Width="100%" OnTextChanged="txtbillingcustomer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Name"
                                                                ControlToValidate="txtbillingcustomer" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                                                TargetControlID="txtbillingcustomer">
                                                            </asp:AutoCompleteExtender>
                                                        </div>
                                                        <div class="col-md-2 spancls">Shipping Customer<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshippingcustomer" CssClass="form-control" runat="server" placeholder="Search Customer Name" Width="100%" OnTextChanged="txtshippingcustomer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Customer"
                                                                ControlToValidate="txtshippingcustomer" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                            <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                                                TargetControlID="txtshippingcustomer">
                                                            </asp:AutoCompleteExtender>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Billing Address<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtbillingaddress" MaxLength="100" placeholder="Billing Address" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" Display="Dynamic" ErrorMessage="Please Enter Billing Address"
                                                                ControlToValidate="txtbillingaddress" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                        <div class="col-md-2 spancls">Shipping Address<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtShippingaddress" MaxLength="100" placeholder="Shipping Address" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" Display="Dynamic" ErrorMessage="Please Enter Shipping Address"
                                                                ControlToValidate="txtShippingaddress" ValidationGroup="form1" InitialValue="" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">E-Invoice Billing Address <i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshortBillingaddress" CssClass="form-control" MaxLength="100" placeholder="Enter Short Billing Address" runat="server"></asp:TextBox>
                                                            <asp:Label ID="Label24" runat="server" Font-Bold="true" ForeColor="Red" CssClass="form-label">You can only enter a maximum of 100 words.</asp:Label>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator21" ValidationGroup="form1" Font-Bold="true" runat="server" ControlToValidate="txtshortBillingaddress"
                                                                ForeColor="Red" ErrorMessage="* Please Enter Short Billing Address" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtshortBillingaddress"
                                                                ValidationExpression="^[^'&quot;]*$" ErrorMessage="* Single or double quotes are not allowed" ForeColor="Red"></asp:RegularExpressionValidator>
                                                        </div>
                                                        <div class="col-md-2 spancls">E-Invoice Shipping Address<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshortShippingaddress" CssClass="form-control" MaxLength="100" placeholder="Enter Short Shipping Address" runat="server"></asp:TextBox>
                                                            <asp:Label ID="Label25" runat="server" Font-Bold="true" ForeColor="Red" CssClass="form-label">You can only enter a maximum of 100 words.</asp:Label>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator22" ValidationGroup="form1" Font-Bold="true" runat="server" ControlToValidate="txtshortShippingaddress"
                                                                ForeColor="Red" ErrorMessage="* Please Enter Short Shipping Address" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtshortShippingaddress"
                                                                ValidationExpression="^[^'&quot;]*$" ErrorMessage="* Single or double quotes are not allowed" ForeColor="Red"></asp:RegularExpressionValidator>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Billing Location<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtbillinglocation" placeholder="Billing Location" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" Display="Dynamic" ErrorMessage="Please Enter Billing Location"
                                                                ControlToValidate="txtbillinglocation" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </div>
                                                        <div class="col-md-2 spancls">Shipping Location<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshippinglocation" placeholder="Shipping Location" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" Display="Dynamic" ErrorMessage="Please Enter Shipping Location"
                                                                ControlToValidate="txtshippinglocation" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Billing GST<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtbillingGST" MaxLength="15" placeholder="Billing GST" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" Display="Dynamic" ErrorMessage="Please Enter Billing GST"
                                                                ControlToValidate="txtbillingGST" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                            <asp:RegularExpressionValidator ID="revGSTNumber" runat="server"
                                                                ControlToValidate="txtbillingGST"
                                                                ValidationExpression="^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$"
                                                                Display="Dynamic"
                                                                ErrorMessage="Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4"
                                                                ForeColor="Red" />
                                                        </div>
                                                        <div class="col-md-2 spancls">Shipping GST<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshippingGST" MaxLength="15" placeholder="Shipping GST" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator20" runat="server" Display="Dynamic" ErrorMessage="Please Enter Shipping GST"
                                                                ControlToValidate="txtshippingGST" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                                ControlToValidate="txtshippingGST"
                                                                ValidationExpression="^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$"
                                                                Display="Dynamic"
                                                                ErrorMessage="Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4"
                                                                ForeColor="Red" />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Billing Pincode<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtbillingPincode" MaxLength="6" placeholder="Billing Pincode" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" Display="Dynamic" ErrorMessage="Please Enter Billing Pincode."
                                                                ControlToValidate="txtbillingPincode" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                        <div class="col-md-2 spancls">Shipping Pincode<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshippingPincode" MaxLength="6" placeholder="Shipping Pincode" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" Display="Dynamic" ErrorMessage="Please Enter Shipping Pincode."
                                                                ControlToValidate="txtshippingPincode" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Billing State Code<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtbillingstatecode" MaxLength="2" placeholder="Billing State Code" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Display="Dynamic" ErrorMessage="Please Enter billing state code."
                                                                ControlToValidate="txtbillingstatecode" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                        <div class="col-md-2 spancls">Shipping State Code<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtshippingstatecode" MaxLength="2" placeholder="Shipping State Code" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="Dynamic" ErrorMessage="Please Enter shipping state code."
                                                                ControlToValidate="txtshippingstatecode" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Contact No<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtContactNo" MaxLength="10" placeholder="Contact No" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-2 spancls">Email<i class="reqcls">&nbsp;*</i>  :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtemail" CssClass="form-control" placeholder="Email" runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Display="Dynamic" ErrorMessage="Please Enter Email."
                                                                ControlToValidate="txtemail" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Invoice Against<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtinvoicetype" CssClass="form-control" Text="Order" ReadOnly="true" placeholder="Email" runat="server" Width="100%"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-2 spancls">Against Number<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtagainstno" CssClass="form-control" placeholder="Email" runat="server" Width="100%"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Customer PO No.<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtcustomerPoNo" CssClass="form-control" placeholder="Customer PO No." runat="server" Width="100%"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer PO NUmber"
                                                                ControlToValidate="txtcustomerPoNo" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                        <div class="col-md-2 spancls">PO Date<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:HiddenField runat="server" ID="hdnfileData" />
                                                            <asp:HiddenField runat="server" ID="hdnGrandtotal" />
                                                            <asp:TextBox ID="txtpodate" CssClass="form-control" runat="server" TextMode="date" Width="100%" AutoComplete="off"></asp:TextBox>

                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ErrorMessage="Please Enter Date"
                                                                ControlToValidate="txtpodate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Challan No<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtchallanNo" CssClass="form-control" placeholder="Challan No" runat="server" Width="100%"></asp:TextBox>
                                                            <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="Dynamic" ErrorMessage="Please Enter Challan NUmber"
                                                            ControlToValidate="txtchallanNo" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                                        </div>
                                                        <div class="col-md-2 spancls">Challan Date<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtchallanDate" CssClass="form-control" runat="server" Width="100%" TextMode="Date" AutoComplete="off"></asp:TextBox>
                                                            <%--<asp:CalendarExtender ID="CalendarExtender3" TargetControlID="txtchallanDate" Format="dd-MM-yyyy" CssClass="cal_Theme1" runat="server"></asp:CalendarExtender>--%>
                                                            <%--          <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Display="Dynamic" ErrorMessage="Please Enter Challan Date"
                                                            ControlToValidate="txtchallanDate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Transport Mode<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:DropDownList ID="txttransportMode" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txttransportMode_TextChanged" AutoPostBack="true">
                                                                <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                                <asp:ListItem>By Road</asp:ListItem>
                                                                <asp:ListItem>By Air</asp:ListItem>
                                                                <asp:ListItem>By Courier</asp:ListItem>
                                                                <asp:ListItem>By Hand</asp:ListItem>
                                                            </asp:DropDownList>
                                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" Display="Dynamic" ErrorMessage="Please Enter Transport Mode"
                                                                ControlToValidate="txttransportMode" ValidationGroup="form1" InitialValue="0" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                        </div>
                                                        <div class="col-md-2 spancls">Transport Details<i class="reqcls">&nbsp;*</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtvehicalNumber" CssClass="form-control uppercase" runat="server" Width="100%" Visible="false" placeholder="Vehicle No"></asp:TextBox>
                                                            <%--                  <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" Display="Dynamic" ErrorMessage="Please Enter Transport Details"
                                                                ControlToValidate="txtvehicalNumber" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>

                                                            <asp:TextBox ID="txtByAir" CssClass="form-control uppercase" runat="server" Width="100%" Visible="false" placeholder="Air Details"></asp:TextBox>
                                                            <%--                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" Display="Dynamic" ErrorMessage="Please Enter Transport Details"
                                                                ControlToValidate="txtByAir" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>

                                                            <asp:TextBox ID="txtByHand" CssClass="form-control uppercase" runat="server" Width="100%" Visible="false" placeholder="Person Name"></asp:TextBox>
                                                            <%--   <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" Display="Dynamic" ErrorMessage="Please Enter Transport Details"
                                                                ControlToValidate="txtByHand" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <%--<div class="col-md-2 spancls">Payment Due Date<i class="reqcls">&nbsp;*</i> :</div>--%>
                                                        <%-- <div class="col-md-4" style="display: none;">
                                                        <asp:TextBox ID="txtpaymentDuedate" CssClass="form-control" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>
                                                        <asp:CalendarExtender ID="CalendarExtender3" TargetControlID="txtpaymentDuedate" Format="dd-MM-yyyy" CssClass="cal_Theme1" runat="server"></asp:CalendarExtender>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="Dynamic" ErrorMessage="Please Enter Date"
                                                            ControlToValidate="txtpaymentDuedate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                    </div>--%>
                                                        <div class="col-md-2 spancls">Remark<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtremark" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-2 spancls">E-Bill Number<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtebillnumber" CssClass="form-control" placeholder="E-Bill Number" runat="server" Width="100%"></asp:TextBox>
                                                        </div>

                                                        <%--    <div class="col-md-2 spancls">Batch No.<i class="reqcls">&nbsp;</i> :</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtBatchNo" CssClass="form-control" placeholder="Batch No." runat="server" Width="100%"></asp:TextBox>
                                                        </div>--%>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-2 spancls">Payment Term(In days):<i class="reqcls">&nbsp;</i></div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtpaymentterm" TextMode="Number" CssClass="form-control" placeholder="Payment Term(In days)" runat="server" Width="100%"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-2 spancls">Terms of Delivery :<i class="reqcls">&nbsp;</i></div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtTermofdelivery" CssClass="form-control" placeholder="Enter Terms of Delivery" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <br />
                                                    <div class="table-responsive" id="manuallytable" runat="server">
                                                        <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                            <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                                                <td style="width: 50%;">Particulars</td>
                                                                <td style="width: 50%;">Description</td>
                                                                <td>Batch</td>
                                                                <td>HSN</td>
                                                                <td>Qty</td>
                                                                <td>UOM</td>
                                                                <td>Rate</td>
                                                                <td>Dis(%)</td>
                                                                <td>Amount</td>
                                                                <td>CGST</td>
                                                                <td>SGST</td>
                                                                <td>IGST</td>
                                                                <td>Grand Total</td>
                                                                <td style="width: 10%">Action</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <%--   <asp:TextBox ID="txtParticulars" Width="200px" runat="server" AutoPostBack="true" OnTextChanged="txtParticulars_TextChanged"></asp:TextBox>--%>
                                                                    <%--    <asp:TextBox ID="txtPartname" CssClass="form-control" placeholder="Search Part" runat="server" Width="230px" AutoPostBack="true"></asp:TextBox>
                                                                <asp:AutoCompleteExtender ID="AutoCompleteExtender3" runat="server" CompletionListCssClass="completionList"
                                                                    CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                                    CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetPartList"
                                                                    TargetControlID="txtPartname">
                                                                </asp:AutoCompleteExtender>--%>

                                                                    <asp:DropDownList runat="server" ID="txtParticulars" OnTextChanged="txtParticulars_TextChanged" AutoPostBack="true">
                                                                        <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                                        <%--     <asp:ListItem>Enclosures for control panel</asp:ListItem>
                                                                    <asp:ListItem>Part of Enclosure for control panel</asp:ListItem>
                                                                    <asp:ListItem>Scrap Material</asp:ListItem>--%>
                                                                    </asp:DropDownList>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtdiscription" Width="200px" Rows="4" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtBatchno" Width="100px" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtHSN" Width="100px" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtQty" Width="50px" runat="server" AutoPostBack="true" OnTextChanged="txtQty_TextChanged" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtuom" Width="100px" runat="server" Text="Nos"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtRate" Width="100px" runat="server" OnTextChanged="txtRate_TextChanged" AutoPostBack="true" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtdiscount" Width="50px" Text="0" runat="server" OnTextChanged="txtdiscount_TextChanged" AutoPostBack="true" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtAmountt" Width="100px" runat="server" ReadOnly="true" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="CGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" placeholder="%" OnTextChanged="CGSTPer_TextChanged" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                    <asp:TextBox ID="CGSTAmt" Width="100px" runat="server" Text="0" ReadOnly="true" placeholder="CGSTAmt"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="SGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" placeholder="%" OnTextChanged="SGSTPer_TextChanged1" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                    <asp:TextBox ID="SGSTAmt" Width="100px" runat="server" Text="0" ReadOnly="true" placeholder="SGSTAmt"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="IGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" placeholder="%" OnTextChanged="IGSTPer_TextChanged1" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                    <asp:TextBox ID="IGSTAmt" Width="100px" runat="server" Text="0" ReadOnly="true" placeholder="IGSTAmt"></asp:TextBox>
                                                                </td>

                                                                <td>
                                                                    <asp:TextBox ID="txtGrandtotal" Width="100px" runat="server" ReadOnly="true"></asp:TextBox>
                                                                </td>

                                                                <td>
                                                                    <asp:Button ID="btnAddMore" CssClass="btn btn-success btn-sm btncss" runat="server" Text="+ Add" OnClick="btnAddMore_Click" />
                                                                </td>
                                                            </tr>

                                                        </table>
                                                    </div>

                                                    <div class="row" id="divdtls">
                                                        <div class="table-responsive">

                                                            <asp:GridView ID="gvinvoiceParticular" runat="server" CssClass="table" HeaderStyle-BackColor="#009999" AutoGenerateColumns="false"
                                                                EmptyDataText="No records has been added." ShowFooter="false" OnRowEditing="gvinvoiceParticular_RowEditing" OnRowDataBound="gvinvoiceParticular_RowDataBound" DataKeyNames="Id">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                                            <asp:Label ID="lblid" runat="Server" Text='<%# Eval("Id") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Particulars" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">

                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblParticulars" runat="Server" Text='<%# Eval("Particular") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Description" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" Text='<%# Eval("Description") %>'></asp:TextBox>

                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="txtDescription" TextMode="MultiLine" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Batchno" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox ID="txtBatchno" runat="server" Text='<%# Eval("Batchno") %>'></asp:TextBox>

                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblBatchno" runat="server" Text='<%# Eval("Batchno") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">

                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblHSN" runat="Server" Text='<%# Eval("HSN") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Qty" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("Qty") %>' Width="50px" ID="txtQty" runat="server" AutoPostBack="true" OnTextChanged="txtQty_TextChanged2" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblQty" runat="Server" Text='<%# Eval("Qty") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderText="UOM" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" Width="50" ID="txtUOM" Text='<%# Eval("UOM") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">

                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRate" runat="Server" Text='<%# Eval("Rate") %>' />
                                                                        </ItemTemplate>
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("Rate") %>' Width="50px" ID="txtrate" runat="server" AutoPostBack="true" OnTextChanged="txtrate_TextChanged" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Dis(%)" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("Discount") %>' Width="50px" ID="txtdiscountedit" onkeypress="return isNumberKey(event)" runat="server" AutoPostBack="true" OnTextChanged="txtdiscountedit_TextChanged"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" Width="50" ID="txtdiscount" Text='<%# Eval("Discount") %>' AutoPostBack="true"></asp:Label>
                                                                        </ItemTemplate>

                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Amount" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAmount" runat="Server" Text='<%# Eval("TaxableAmt") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="CGSTPer" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("CGSTPer") %>' Width="50px" ID="txtCGSTPer" onkeypress="return isNumberKey(event)" runat="server" AutoPostBack="true" OnTextChanged="txtCGSTPer_TextChanged1"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblCGSTPer" runat="Server" Text='<%# Eval("CGSTPer") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="CGSTAmt" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblCgstAmt" runat="Server" Text='<%# Eval("CGSTAmt") %>' />
                                                                        </ItemTemplate>
                                                                        <EditItemTemplate>
                                                                            <asp:Label ID="lblCgstAmt" runat="Server" Text='<%# Eval("CGSTAmt") %>' />
                                                                        </EditItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="SGSTPer" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("SGSTPer") %>' Width="50px" ID="txtSGSTPer" onkeypress="return isNumberKey(event)" runat="server" AutoPostBack="true" OnTextChanged="txtSGSTPer_TextChanged"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSGSTPer" runat="Server" Text='<%# Eval("SGSTPer") %>' />
                                                                        </ItemTemplate>

                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="SGSTAmt" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblSgstAmt" runat="Server" Text='<%# Eval("SGSTAmt") %>' />
                                                                        </ItemTemplate>
                                                                        <EditItemTemplate>
                                                                            <asp:Label ID="lblSgstAmt" runat="Server" Text='<%# Eval("SGSTAmt") %>' />
                                                                        </EditItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="IGSTPer" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("IGSTPer") %>' Width="50px" ID="txtIGSTPer" onkeypress="return isNumberKey(event)" runat="server" AutoPostBack="true" OnTextChanged="txtIGSTPer_TextChanged"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblIGSTPer" runat="Server" Text='<%# Eval("IGSTPer") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="IGSTAmt" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblIGSTAmt" runat="Server" Text='<%# Eval("IGSTAmt") %>' />
                                                                        </ItemTemplate>
                                                                        <EditItemTemplate>
                                                                            <asp:Label ID="lblIGSTAmt" runat="Server" Text='<%# Eval("IGSTAmt") %>' />
                                                                        </EditItemTemplate>
                                                                    </asp:TemplateField>


                                                                    <asp:TemplateField HeaderText="Grand Total" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" Width="100" ID="txtGrandTotal" ReadOnly="true" Text='<%# Eval("GrandTotal") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField ItemStyle-Width="120">
                                                                        <EditItemTemplate>
                                                                            <asp:LinkButton Text="Update" ID="lnkbtnUpdate" ClientIDMode="Static" runat="server" ToolTip="Update" OnClick="lnkbtnUpdate_Click"><i class="fa fa-refresh" style="font-size:28px;color:green;"></i></asp:LinkButton>
                                                                            |
                                                                            <asp:LinkButton Text="Cancel" ID="lnkCancel" runat="server" OnClick="lnkCancel_Click" ToolTip="Cancel"><i class="fa fa-close" style="font-size:28px;color:red;"></i></asp:LinkButton>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton Text="Edit" runat="server" CommandName="Edit" ToolTip="Edit"><i class="fa fa-edit" style="font-size:28px;color:blue;"></i></asp:LinkButton>
                                                                            | 
                                                                    <asp:LinkButton ID="lnkDelete" runat="server" CommandArgument='<%# Eval("id") %>' ToolTip="Delete" OnClick="lnkDelete_Click"><i class="fa fa-trash" style="font-size:28px;color:red"></i></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>

                                                        </div>
                                                    </div>

                                                    <div class="col-md-12">
                                                        <div class="col-md-2"></div>
                                                        <center>
                                                            <div class="col-md-8">
                                                                <div class="col-md-4"><b>Sum of Product Amount :</b></div>
                                                                <div class="col-md-4">
                                                                    <asp:TextBox ID="sumofAmount" CssClass="form-control" runat="server" Width="100%" ReadOnly="true" Text="0"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                                </center>
                                                        <div class="col-md-2"></div>
                                                    </div>
                                                    <br />

                                                    <div class="table-responsive">
                                                        <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                            <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                                                <td style="width: 30%;">Charges Description</td>
                                                                <td>HSN</td>
                                                                <td id="rate" runat="server" visible="false">Rate(%)</td>
                                                                <td>Basic</td>
                                                                <td>CGST</td>
                                                                <td>SGST</td>
                                                                <td>IGST</td>
                                                                <td>Cost</td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <asp:TextBox ID="txtDescription" Width="350px" runat="server" TextMode="MultiLine" Text="Freight"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txthsntcs" Width="100px" runat="server"></asp:TextBox>
                                                                </td>
                                                                <td id="tdh" runat="server" visible="false">
                                                                    <asp:TextBox ID="txtrateTcs" Width="100px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="txtrateTcs_TextChanged"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtBasic" Width="100px" runat="server" Text="0" Enabled="true" OnTextChanged="txtBasic_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="CGSTPertcs" Width="50px" runat="server"   Text="0" AutoPostBack="true" OnTextChanged="CGSTPertcs_TextChanged"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="SGSTPertcs" Width="50px" runat="server"  Text="0" AutoPostBack="true" OnTextChanged="SGSTPertcs_TextChanged"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="IGSTPertcs" Width="50px" runat="server"  Text="0" AutoPostBack="true" OnTextChanged="IGSTPertcs_TextChanged"></asp:TextBox>
                                                                </td>
                                                                <td>
                                                                    <asp:TextBox ID="txtCost" Width="100px" runat="server" Enabled="false" Text="0"></asp:TextBox>
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                    <br />
                                                    <br />
                                                    <div class="col-md-12">
                                                        <center>
                                                            <div class="col-md-4">
                                                                <div class="row">
                                                                    <div class="col-md-4">  TCS (%)<asp:DropDownList runat="server" ID="txtTCSPer" CssClass="form-control" placeholder="TCS (%)"  AutoPostBack="true" OnTextChanged="txtTCSPer_TextChanged1">
                                                                        <asp:ListItem Value="0">Select</asp:ListItem>
                                                                        <asp:ListItem>0.5</asp:ListItem>
                                                                         <asp:ListItem>1</asp:ListItem>
                                                                         <asp:ListItem>1.5</asp:ListItem>
                                                                         <asp:ListItem>2</asp:ListItem>
                                                                         </asp:DropDownList></div>
                                                                    <div class="col-md-8">TCS Amt<asp:TextBox ID="txtTCSAmt" CssClass="form-control" runat="server" Width="100%" ReadOnly="true" Text="0" placeholder="TCS amount"></asp:TextBox></div>
                                                                </div>
                                                            </div>
                                                                   </center>
                                                        <center>
                                                            <div class="col-md-8">
                                                                <div class="col-md-4"><b>Grand Total :</b></div>
                                                                <div class="col-md-4">
                                                                    <asp:TextBox ID="txtGrandTot" CssClass="form-control" runat="server" Width="100%" ReadOnly="true"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                                </center>
                                                    </div>


                                                    <div class="row">
                                                        <div class="col-md-2" style="margin-left: 18%;"></div>

                                                        <div class="col-md-2">
                                                            <asp:Button ID="btnSubmit" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Width="100%" Text="Submit" OnClientClick="showLoader();" OnClick="btnSubmit_Click1" />
                                                        </div>
                                                        <div class="col-md-2">
                                                            <asp:Button ID="btnreset" runat="server" CssClass="btn btn-danger" Width="100%" Text="Reset" />
                                                        </div>

                                                        <div class="col-md-6"></div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="loader" class="loader-wrapper">
                <div class="loader"></div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="btnreset" EventName="Click" />
            <asp:PostBackTrigger ControlID="gvinvoiceParticular" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

