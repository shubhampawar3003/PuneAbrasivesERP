<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="AddProformaInvoice.aspx.cs" Inherits="Admin_AddProformaInvoice" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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
            background-color: #31B0C4;
        }

        .head {
            text-align: center;
            color: #000000;
            background-color: #FF7F50;
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="loader" class="loader-wrapper">
                <div class="loader"></div>
            </div>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-md-10">
                        <h4 class="mt-4">&nbsp <b>CREATE CUSTOMER PROFORMA INVOICE</b></h4>
                    </div>
                    <div class="col-md-2 mt-4">
                        <asp:LinkButton ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="Button1_Click">
    <i class="fas fa-file-alt"></i>List
                        </asp:LinkButton>
                    </div>
                </div>
                <br />
                <div class="container-fluid px-3">

                    <div class="card mb-4">

                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblinvoiceNo" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice No. :</asp:Label>

                                    <asp:TextBox ID="txtInvoiceno" CssClass="form-control" ReadOnly="true" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblcompanyname" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice Type :</asp:Label>

                                    <asp:DropDownList runat="server" ID="ddlInvoiceType" CssClass="form-control">
                                        <asp:ListItem Text="Regular"></asp:ListItem>
                                        <asp:ListItem Text="Export"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblinvoiceDate" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice Date  :</asp:Label>

                                    <asp:TextBox ID="txtinvoiceDate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>
                                    <%--   <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtinvoicedate" Format="dd-MM-yyyy" CssClass="cal_Theme1" runat="server"></asp:CalendarExtender>--%>
                                    <%--      <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Please Enter Date"
                                        ControlToValidate="txtinvoicedate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    --%>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblownernmae" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Billing Customer  :<span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtbillingcustomer" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txtbillingcustomer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Name"
                                        ControlToValidate="txtbillingcustomer" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                        TargetControlID="txtbillingcustomer">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label21" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Shipping Customer :<span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtshippingcustomer" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txtshippingcustomer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                        TargetControlID="txtshippingcustomer">
                                    </asp:AutoCompleteExtender>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblinvoiceagainst" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Billing Address : <span class="spncls">*</span></asp:Label>

                                    <asp:DropDownList ID="ddlBillAddress" Width="560px" AutoPostBack="true" OnSelectedIndexChanged="ddlBillAddress_SelectedIndexChanged"
                                        CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label11" runat="server" Font-Bold="true" CssClass="form-label">Shipping Address :<span class="spncls">*</span></asp:Label>
                                    <asp:DropDownList ID="ddlShippingaddress" Width="560px" OnSelectedIndexChanged="ddlShippingaddress_SelectedIndexChanged"
                                        CssClass="form-control" AutoPostBack="true" runat="server">
                                    </asp:DropDownList>
                                    <%--    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Display="Dynamic" ErrorMessage="Please select shipping address."
                                        ControlToValidate="ddlShippingaddress" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label19" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Billing location :</asp:Label>

                                    <asp:TextBox ID="txtbillinglocation" CssClass="form-control" ReadOnly="true" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label18" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Shipping location :</asp:Label>

                                    <asp:TextBox ID="txtshippinglocation" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label17" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Billing GST :</asp:Label>

                                    <asp:TextBox ID="txtbillingGST" CssClass="form-control" ReadOnly="true" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label16" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Shipping GST :</asp:Label>

                                    <asp:TextBox ID="txtshippingGST" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label15" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Billing Pincode :</asp:Label>

                                    <asp:TextBox ID="txtbillingPincode" CssClass="form-control" ReadOnly="true" TextMode="Number" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label14" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Shipping Pincode :</asp:Label>

                                    <asp:TextBox ID="txtshippingPincode" CssClass="form-control" TextMode="Number" runat="server" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label13" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Billing Statecode :<span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtbillingstatecode" CssClass="form-control" ReadOnly="true" runat="server" Width="100%"></asp:TextBox>


                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label12" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Shipping Statecode :<span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtshippingstatecode" CssClass="form-control" ValidationGroup="form1"  runat="server" Width="100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Please Enter state code"
                                        ControlToValidate="txtshippingstatecode" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label10" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Contact No.  :</asp:Label>

                                    <asp:TextBox ID="txtContactNo" CssClass="form-control" ReadOnly="true" TextMode="Number" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label9" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Email ID.  :</asp:Label>

                                    <asp:TextBox ID="txtemail" CssClass="form-control" ReadOnly="true" TextMode="Email" runat="server" Width="100%"></asp:TextBox>
                                    <%--        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Display="Dynamic" ErrorMessage="Please Enter Email."
                                        ControlToValidate="txtemail" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    --%>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice Against :<span class="spncls">*</span></asp:Label>

                                    <asp:DropDownList runat="server" ID="txtinvoiceagainst" CssClass="form-control" OnTextChanged="txtinvoiceagainst_TextChanged" AutoPostBack="true">
                                        <asp:ListItem Value="" Text="--Select--"></asp:ListItem>
                                        <asp:ListItem Text="Direct"></asp:ListItem>
                                        <asp:ListItem Text="Order"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label7" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Against OA Number :</asp:Label>

                                    <asp:DropDownList runat="server" ID="txtagainstNumber" CssClass="form-control" OnTextChanged="txtagainstNumber_TextChanged" AutoPostBack="true">
                                        <asp:ListItem Value="" Text="--Select--"></asp:ListItem>
                                    </asp:DropDownList>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label6" runat="server" Font-Bold="true" CssClass="form-label LblStyle">OA Number  :</asp:Label>

                                    <asp:TextBox ID="txtcustomerPoNo" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="true" CssClass="form-label LblStyle">OA Dates :</asp:Label>

                                    <asp:HiddenField runat="server" ID="hdnfileData" />
                                    <asp:HiddenField runat="server" ID="hdnGrandtotal" />
                                    <asp:HiddenField runat="server" ID="hdnInvoiceID" />
                                    <asp:HiddenField runat="server" ID="hdnBillingCustomer" />
                                    <asp:TextBox ID="txtpodate" CssClass="form-control" runat="server" TextMode="Date" Width="100%" AutoComplete="off"></asp:TextBox>
                                    <%--        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ErrorMessage="Please Enter Date"
                                        ControlToValidate="txtpodate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    --%>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label4" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Challan No. :</asp:Label>

                                    <asp:TextBox ID="txtchallanNo" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label3" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Challan Date :</asp:Label>

                                    <asp:TextBox ID="txtchallanDate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label2" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Transport Mode :</asp:Label>

                                    <asp:DropDownList ID="txttransportMode" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txttransportMode_TextChanged" AutoPostBack="true">
                                        <asp:ListItem Value="0">--Select--</asp:ListItem>
                                        <asp:ListItem>By Road</asp:ListItem>
                                        <asp:ListItem>By Air</asp:ListItem>
                                        <asp:ListItem>By Courier</asp:ListItem>
                                        <asp:ListItem>By Hand</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3" id="divtransportdetails" runat="server" visible="false">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Transport Details :</asp:Label>

                                    <asp:TextBox ID="txtvehicalNumber" CssClass="form-control" runat="server" Width="100%" Visible="false" placeholder="Vehicle No"></asp:TextBox>
                                    <asp:TextBox ID="txtByAir" CssClass="form-control" runat="server" Width="100%" Visible="false" placeholder="Air Details"></asp:TextBox>
                                    <asp:TextBox ID="txtByHand" CssClass="form-control" runat="server" Width="100%" Visible="false" placeholder="Person Name"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblmark" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Remark :</asp:Label>
                                    <asp:TextBox ID="txtremark" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblEbillno" runat="server" Font-Bold="true" CssClass="form-label">E-Way Bill No.  :</asp:Label>

                                    <asp:TextBox ID="txtebillnumber" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblpanno" class="form-label LblStyle"><span class="spncls">*</span>PAN No. :</label>
                                    <asp:TextBox ID="txtpanno" CssClass="form-control" placeholder="PAN No." runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="invoice" runat="server" ControlToValidate="txtpanno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Pan No" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblPaymentTerm" class="form-label LblStyle">Payment Term(In days) :</label>
                                    <asp:TextBox ID="txtPaymentTerm" CssClass="form-control" placeholder="Enter Payment Term" TextMode="Number" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <hr />
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblPayment" runat="server" Font-Bold="true" CssClass="form-label">Payment  :</asp:Label>

                                    <asp:TextBox ID="txtPayment" CssClass="form-control" placeholder="Enter Payment " Text="Against PI" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblTransport" runat="server" Font-Bold="true" CssClass="form-label">Transport :</asp:Label>

                                    <asp:TextBox ID="txtTransport" CssClass="form-control" Text="ACPL" placeholder="Enter Transport" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblDeliveryTime" runat="server" Font-Bold="true" CssClass="form-label"> Term of Delivery :</asp:Label>

                                    <asp:TextBox ID="txtDeliveryTime" CssClass="form-control" Text="Paid" placeholder="Enter Term of Delivery" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblPacking" runat="server" Font-Bold="true" CssClass="form-label">Packing :</asp:Label>

                                    <asp:TextBox ID="txtPacking" CssClass="form-control" Text="25 Kg Bags" placeholder="Enter Packing" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblTaxs" runat="server" Font-Bold="true" CssClass="form-label">Taxs :</asp:Label>

                                    <asp:TextBox ID="txtTaxs" CssClass="form-control" Text="As shown, Extra" placeholder="Enter Taxs" runat="server"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox1_CheckedChanged" />
                                    <asp:Label ID="lblnote" runat="server" Font-Bold="true" ForeColor="Red" Text="Note : Sending mail to customer please check that box."></asp:Label>
                                </div>
                            </div>
                            <%--Grid View Start--%>

                            <div class="c-dashboardInfo col-lg-6 col-md-6" id="CompanyContactUsers" runat="server" visible="false">
                                <div class="wrap">

                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="gvUserMail" runat="server" CssClass="grivdiv"
                                            AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" PageSize="10"
                                            AllowPaging="true" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Select" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="chkHeader" runat="server" OnCheckedChanged="chkHeader_CheckedChanged" AutoPostBack="true" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkSelect" runat="server" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-CssClass="gvhead sno">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblseno" runat="server" Text='<%#Container.DataItemIndex+1 %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Name" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQuotationno" runat="server" Text='<%#Eval("name")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Email ID" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblEmailID" runat="server" Text='<%#Eval("EmailID")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Designation" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbltotlprice" runat="server" Text='<%#Eval("Designation")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                            <br />

                            <%--Grid View Start--%>

                            <div id="TableDirect" runat="server" visible="false">

                                <div class="card-header head" style="margin-top: 10px;">
                                    <h5 style="color: white">Tax Invoice Details</h5>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="table-responsive" id="table1">
                                            <table class="table text-center align-middle table-bordered table-hover" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                <thead>
                                                    <tr class="gvhead">
                                                        <td>Product</td>
                                                        <td>Description</td>
                                                        <td>Batch</td>
                                                        <td>HSN / SAC</td>
                                                        <td>Quantity</td>

                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="ddlProduct" Width="230px" OnTextChanged="ddlProduct_TextChanged" CssClass="form-control" AutoPostBack="true" runat="server"></asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtdescription" Width="230px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtBatchno" Width="100px" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txthsnsac" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtquantity" Width="190px" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtquantity_TextChanged" runat="server"></asp:TextBox>
                                                        </td>

                                                    </tr>
                                                </tbody>
                                                <thead>
                                                    <tr class="gvhead">

                                                        <td>Unit</td>
                                                        <td>Rate</td>
                                                        <td>Total </td>
                                                        <td colspan="2">CGST</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>

                                                        <td>
                                                            <asp:TextBox ID="txtunit" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtrate" Width="190px" AutoPostBack="true" OnTextChanged="txtrate_TextChanged" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txttotal" Width="100" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtCGST" placeholder="%" Width="100px" OnTextChanged="txtCGST_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtCGSTamt" Width="100px" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                                <thead>
                                                    <tr class="gvhead">

                                                        <td colspan="2">SGST</td>
                                                        <td colspan="2">IGST</td>
                                                        <td>Discount(%)</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>

                                                        <td>
                                                            <asp:TextBox ID="txtSGST" placeholder="%" Width="100px" OnTextChanged="txtSGST_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtSGSTamt" Width="100px" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtIGST" OnTextChanged="txtIGST_TextChanged" AutoPostBack="true" placeholder="%" Width="100px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtIGSTamt" Width="100px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtdiscount" OnTextChanged="txtdiscount_TextChanged" Width="80px" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                            <asp:TextBox ID="txtdiscountamt" Visible="false" Width="80px" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </tbody>

                                                <thead>
                                                    <tr class="gvhead">

                                                        <td>Grand Total</td>
                                                        <td>Action</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>


                                                        <td>
                                                            <asp:TextBox ID="txtgrandtotal" Width="150px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnAddMore" CausesValidation="false" OnClick="btnAddMore_Click" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add More" />
                                                        </td>

                                                    </tr>
                                                </tbody>

                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="TableOrder" runat="server" visible="false">
                            <%--<div class="row" id="divdtls">--%>
                            <div class="table-responsive text-center">
                                <asp:GridView ID="dgvTaxinvoiceDetails" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                    OnRowEditing="dgvTaxinvoiceDetails_RowEditing" OnRowDataBound="dgvTaxinvoiceDetails_RowDataBound" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Product" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Productname") %>' ReadOnly="true" CssClass="form-control" Width="230px" ID="txtproduct" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblproduct" runat="Server" Text='<%# Eval("Productname") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Description") %>' CssClass="form-control" ID="txtDescription" Width="200px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDescription" runat="Server" Text='<%# Eval("Description") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Batchno" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox ID="txtBatchno" runat="server" Text='<%# Eval("Batchno") %>'></asp:TextBox>

                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblBatchno" runat="server" Text='<%# Eval("Batchno") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("HSN") %>' CssClass="form-control" ID="txthsn" Width="200px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblhsn" runat="Server" Text='<%# Eval("HSN") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Quantity") %>' CssClass="form-control" ID="txtQuantity" OnTextChanged="txtQuantity_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblQuantity" runat="Server" Text='<%# Eval("Quantity") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Unit" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Units") %>' CssClass="form-control" ID="txtUnit" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblUnit" runat="Server" Text='<%# Eval("Units") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Rate") %>' CssClass="form-control" OnTextChanged="txtRate_TextChanged" AutoPostBack="true" ID="txtRate" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblRate" runat="Server" Text='<%# Eval("Rate") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Total") %>' CssClass="form-control" ID="txtTotal" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblTotal" runat="Server" Text='<%# Eval("Total") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("CGSTPer") %>' CssClass="form-control" OnTextChanged="txtCGSTPer_TextChanged" AutoPostBack="true" ID="txtCGSTPer" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblCGSTPer" runat="Server" Text='<%# Eval("CGSTPer") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="CGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("CGSTAmt") %>' CssClass="form-control" ID="txtCGST" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblCGST" runat="Server" Text='<%# Eval("CGSTAmt") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("SGSTPer") %>' CssClass="form-control" ID="txtSGSTPer" OnTextChanged="txtSGSTPer_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblSGSTPer" runat="Server" Text='<%# Eval("SGSTPer") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("SGSTAmt") %>' CssClass="form-control" ID="txtSGST" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblSGST" runat="Server" Text='<%# Eval("SGSTAmt") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="IGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("IGSTPer") %>' CssClass="form-control" ID="txtIGSTPer" OnTextChanged="txtIGSTPer_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblIGSTPer" runat="Server" Text='<%# Eval("IGSTPer") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="IGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("IGSTAmt") %>' CssClass="form-control" ID="txtIGST" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblIGST" runat="Server" Text='<%# Eval("IGSTAmt") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discount(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Discountpercentage") %>' CssClass="form-control" ID="txtDiscount" OnTextChanged="txtDiscount_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDiscount" runat="Server" Text='<%# Eval("Discountpercentage") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discount Amount" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("DiscountAmount") %>' CssClass="form-control" ReadOnly="true" ID="txtDiscountAmount" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblDiscountAmount" runat="Server" Text='<%# Eval("DiscountAmount") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Grand Total" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Alltotal") %>' CssClass="form-control" ID="txtAlltotal" Width="100px" runat="server"></asp:TextBox>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label ID="lblAlltotal" runat="Server" Text='<%# Eval("Alltotal") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="btn_edit" CausesValidation="false" Text="Edit" runat="server" CommandName="Edit"><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>
                                                <asp:LinkButton runat="server" ID="lnkbtnDelete" OnClick="lnkbtnDelete_Click" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <EditItemTemplate>
                                                <asp:LinkButton ID="gv_update" OnClick="gv_update_Click" CausesValidation="false" Text="Update" CssClass="btn btn-primary btn-sm" runat="server"></asp:LinkButton>&nbsp;
                                                        <asp:LinkButton ID="gv_cancel" OnClick="gv_cancel_Click" CausesValidation="false" Text="Cancel" CssClass="btn btn-primary btn-sm " runat="server"></asp:LinkButton>
                                            </EditItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <br />
                    </div>
                </div>
                <%--Grid View End--%>

                <%--last total show--%>

                <div id="divTotalPart" runat="server" visible="false">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-6">
                                    <br />

                                    <center>
                                        <div class="col-md-12">
                                            <asp:Label ID="lbl_total_amt" runat="server" class="control-label col-sm-6">Total Amount (In Words) :<span class="spncls"></span></asp:Label><br />
                                            <asp:Label ID="lbl_total_amt_Value" ForeColor="red" class="control-label col-sm-6 font-weight-bold" runat="server" Text=""></asp:Label>
                                            <asp:HiddenField ID="hfTotal" runat="server" />
                                        </div>
                                    </center>
                                </div>
                                <div class="col-md-6" style="text-align: right">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:Label ID="lbl_Subtotal" runat="server" class="control-label col-sm-6">SubTotal :<span class="spncls"></span></asp:Label>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_Subtotal" Text="0.00"></asp:Label><br />
                                        </div>
                                    </div>
                                    <asp:Panel ID="taxPanel1" runat="server">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <asp:Label ID="lbl_cgst9" runat="server" class="control-label col-sm-6">CGST  Amount :<span class="spncls"></span></asp:Label>
                                            </div>
                                            <div class="col-md-4">
                                                <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_cgstamt" Text="0.00"></asp:Label><br />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <asp:Label ID="lbl_sgst9" runat="server" class="control-label col-sm-6">SGST  Amount :<span class="spncls"></span></asp:Label>
                                            </div>
                                            <div class="col-md-4">
                                                <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_sgstamt" Text="0.00"></asp:Label><br />
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <asp:Label ID="lbligst" runat="server" class="control-label col-sm-6">IGST  Amount :<span class="spncls"></span></asp:Label>
                                            </div>
                                            <div class="col-md-4">
                                                <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_igstamt" Text="0.00"></asp:Label><br />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                    <div class="row">
                                        <div class="col-md-6">
                                            <asp:Label ID="lbl_grandTotal" runat="server" class="control-label col-sm-6">Grand Total :<span class="spncls"></span></asp:Label>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_grandTotal" Text="0.00"></asp:Label><br />
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>
                <%--  last total show--%>



                <br />
                <div class="row">
                    <div class="col-md-4"></div>
                    <div class="col-6 col-md-2">
                        <asp:Button ID="btnsave" OnClick="btnsave_Click" OnClientClick="showLoader();" ValidationGroup="form1" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
                    </div>
                    <div class="col-6 col-md-2">
                        <asp:Button ID="btncancel" OnClick="btncancel_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                    </div>
                    <div class="col-md-4"></div>
                </div>

                <asp:Label ID="lblst" runat="server" Visible="false"></asp:Label>
            </div>
            </div>
                <asp:HiddenField ID="hhd" runat="server" />

            </div>
            </div>
                      <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>

            <asp:PostBackTrigger ControlID="btncancel" />


        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

