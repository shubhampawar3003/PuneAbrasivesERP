<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="CreditDebitNote_Sale.aspx.cs" Inherits="Account_CreditDebitNote" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .dissablebtn {
            cursor: not-allowed;
        }
    </style>
    <style>
        .gvhead {
            background-color: #7ad2d4;
            color: #000;
            font-weight: 600;
            text-align: center;
        }

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


    <script>
        function HideLabel(msg) {
            Swal.fire({
                icon: 'success',
                text: msg,
                timer: 3000,
                showCancelButton: false,
                showConfirmButton: false
            }).then(function () {
                window.location.href = "../Admin/AllSupplierList.aspx";
            })
        };
    </script>
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="updatepnl" runat="server">
        <ContentTemplate>
            <div class="page-wrapper">
                <div class="page-body">
                    <div class="row">
                        <div class="col-md-7">
                        </div>
                        <div class="col-md-5">
                            <asp:HiddenField ID="hidden1" runat="server" />

                        </div>
                    </div>

                    <div class="container py-3">
                        <div class="card">
                            <div class="card-header text-uppercase text-black">
                                <div class="row">
                                    <div class="col-9 col-md-9">
                                        <h5>Create Credit/Debit Note</h5>
                                    </div>
                                    <div class="col-3 col-md-3">
                                        <asp:Button ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" runat="server" Text="Credit/Debit Note List" OnClick="Button1_Click" />
                                    </div>


                                </div>
                            </div>
                            <div class="row">
                                <div class="col-xl-12 col-md-12">
                                    <div class="card-header">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Note Type : </div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddlNoteType" OnTextChanged="ddlNoteType_TextChanged" AutoPostBack="true" runat="server" class="form-control">
                                                            <asp:ListItem Value="Select">--Select Note Type--</asp:ListItem>
                                                            <asp:ListItem Value="Debit_Sale">Debit</asp:ListItem>
                                                            <asp:ListItem Value="Credit_Sale">Credit</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>

                                                    <asp:HiddenField runat="server" ID="hdnfileData" />
                                                    <asp:HiddenField runat="server" ID="hdnGrandtotal" />
                                                    <asp:HiddenField runat="server" ID="hdnBasictotal" />

                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Doc No : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtDocNo" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls">Document Date:</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtDocdate" CssClass="form-control" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>
                                                        <asp:CalendarExtender ID="CalendarExtender1" TargetControlID="txtDocdate" Format="dd-MM-yyyy" CssClass="cal_Theme1" runat="server"></asp:CalendarExtender>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <%--<div class="col-md-2 spancls">Supplier Name<i class="reqcls">*&nbsp;</i> : </div>--%>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Billing Customer<i class="reqcls">*&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtSupplierName" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txtSupplierName_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Name"
                                                            ControlToValidate="txtSupplierName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList"
                                                            TargetControlID="txtSupplierName">
                                                        </asp:AutoCompleteExtender>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Shipping Customer<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtshippingcustomer" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txtshippingcustomer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                        <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                                            TargetControlID="txtshippingcustomer">
                                                        </asp:AutoCompleteExtender>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Billing Address<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddlBillAddress" AutoPostBack="true" OnSelectedIndexChanged="ddlBillAddress_SelectedIndexChanged"
                                                            CssClass="form-control" runat="server">
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Shipping Address<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <%-- <asp:TextBox ID="txtshippingAddress" MaxLength="100" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>--%>
                                                        <asp:DropDownList ID="ddlShippingaddress" AutoPostBack="true" OnTextChanged="ddlShippingaddress_SelectedIndexChanged"
                                                            CssClass="form-control" runat="server">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"  style="font-weight: bold;">E-Invoice Billing Address <i class="reqcls">&nbsp;*</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtshortBillingaddress" CssClass="form-control" MaxLength="100" placeholder="Enter Short Billing Address" runat="server"></asp:TextBox>
                                                        <asp:Label ID="Label24" runat="server" Font-Bold="true" ForeColor="Red" CssClass="form-label">You can only enter a maximum of 100 words.</asp:Label>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator21" ValidationGroup="form1" Font-Bold="true" runat="server" ControlToValidate="txtshortBillingaddress"
                                                            ForeColor="Red" ErrorMessage="* Please Enter Short Billing Address" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtshortBillingaddress"
                                                            ValidationExpression="^[^'&quot;]*$" ErrorMessage="* Single or double quotes are not allowed" ForeColor="Red"></asp:RegularExpressionValidator>
                                                    </div>
                                                    <div class="col-md-2 spancls"  style="font-weight: bold;">E-Invoice Shipping Address<i class="reqcls">&nbsp;*</i> :</div>
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
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Billing Location<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtbillinglocation" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Shipping Location<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtshippinglocation" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Billing GST<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtbillingGST" MaxLength="15" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="revGSTNumber" runat="server"
                                                            ControlToValidate="txtbillingGST"
                                                            ValidationExpression="^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$"
                                                            Display="Dynamic"
                                                            ErrorMessage="Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4"
                                                            ForeColor="Red" />
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Shipping GST<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtshippingGST" MaxLength="15" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                                            ControlToValidate="txtshippingGST"
                                                            ValidationExpression="^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$"
                                                            Display="Dynamic"
                                                            ErrorMessage="Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4"
                                                            ForeColor="Red" />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Billing Pincode<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtbillingPincode" MaxLength="6" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Shipping Pincode<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtshippingPincode" MaxLength="6" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Billing State Code<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtbillingstatecode" MaxLength="2" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Shipping State Code<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtshippingstatecode" MaxLength="2" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Category:</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtcategory" CssClass="form-control" runat="server" AutoPostBack="true"></asp:TextBox>
                                                        <%-- <asp:DropDownList ID="ddlCategory" runat="server" class="form-control"></asp:DropDownList> ---%>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Bill Number:</div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddlBillNumber" runat="server" class="form-control" OnSelectedIndexChanged="ddlBillNumber_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Payment Due Date <i class="reqcls">*&nbsp;</i>:</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtPaymentDueDate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off" OnTextChanged="txtPaymentDueDate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Payment Due Date"
                                                            ControlToValidate="txtPaymentDueDate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                                    </div>

                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Bill Date:</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtBillDate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off" ReadOnly="true"></asp:TextBox>
                                                    </div>

                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Transport Mode<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="txttransportMode" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txttransportMode_TextChanged" AutoPostBack="true">
                                                            <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                            <asp:ListItem>By Road</asp:ListItem>
                                                            <asp:ListItem>By Air</asp:ListItem>
                                                            <asp:ListItem>By Courier</asp:ListItem>
                                                            <asp:ListItem>By Hand</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Transport Details<i class="reqcls">&nbsp;</i> :</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtvehicalNumber" CssClass="form-control" runat="server" Width="100%" Visible="false" placeholder="Vehicle No"></asp:TextBox>
                                                        <asp:TextBox ID="txtByAir" CssClass="form-control" runat="server" Width="100%" Visible="false" placeholder="Air Details"></asp:TextBox>
                                                        <asp:TextBox ID="txtByHand" CssClass="form-control" runat="server" Width="100%" Visible="false" placeholder="Person Name"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="row" runat="server">
                                                    <div class="col-md-2 spancls" style="font-weight: bold;">Remark : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtRemarks" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="card-header bg-primary text-uppercase text-white" style="margin-top: 10px;" id="dvupdatepearticullar" runat="server" visible="false">
                                                    <h5>Particulars Details</h5>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-12" runat="server" id="DivManual" visible="false">
                                                        <div class="table-responsive">
                                                            <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;" id="tblparticular" runat="server">
                                                                <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                                                    <td style="width: 50%;">Invoice No.</td>
                                                                    <td style="width: 50%;">Particulars</td>
                                                                    <td style="width: 50%;">Description</td>
                                                                    <td>HSN</td>
                                                                    <td>Qty</td>
                                                                    <td>Rate</td>
                                                                    <td>Amount</td>
                                                                    <td>UOM</td>
                                                                    <td>Disc(%)</td>
                                                                    <td>CGST</td>
                                                                    <td>SGST</td>
                                                                    <td>IGST</td>
                                                                    <td>Total Amount</td>
                                                                    <td style="width: 50%;">Remarks</td>
                                                                    <td style="width: 10%">Action</td>
                                                                </tr>
                                                                <tr>
                                                                    <td>
                                                                        <asp:DropDownList runat="server" ID="ddlinvoice" AutoPostBack="true" Width="300px">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <%--<asp:TextBox ID="txtParticulars" Width="300px" runat="server" AutoPostBack="true" OnTextChanged="txtParticulars_TextChanged"></asp:TextBox>
                                                                        <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                                                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetItemList"
                                                                            TargetControlID="txtParticulars">
                                                                        </asp:AutoCompleteExtender>--%>


                                                                        <asp:DropDownList runat="server" ID="txtParticulars" OnTextChanged="txtParticulars_TextChanged" AutoPostBack="true">
                                                                            <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                                            <%--     <asp:ListItem>Enclosures for control panel</asp:ListItem>
                                                                            <asp:ListItem>Part of Enclosure for control panel</asp:ListItem>
                                                                            <asp:ListItem>Scrap Material</asp:ListItem>--%>
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtDescription" Width="250px" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtHSN" Width="100px" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtQty" Width="50px" runat="server" AutoPostBack="true" OnTextChanged="txtQty_TextChanged"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtRate" Width="100px" runat="server" OnTextChanged="txtRate_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtAmountt" Width="100px" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtUOM" Width="100px" runat="server"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtDisct" Width="100px" runat="server" Text="0" OnTextChanged="txtDisct_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="CGSTPer" Width="50px" runat="server" AutoPostBack="true" OnTextChanged="CGSTPer_TextChanged" placeholder="%"></asp:TextBox>
                                                                        <asp:TextBox ID="CGSTAmt" Width="100px" runat="server" placeholder="CGSTAmt"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="SGSTPer" Width="50px" runat="server" AutoPostBack="true" OnTextChanged="SGSTPer_TextChanged" placeholder="%"></asp:TextBox>
                                                                        <asp:TextBox ID="SGSTAmt" Width="100px" runat="server" placeholder="SGSTAmt"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="IGSTPer" Width="50px" runat="server" AutoPostBack="true" OnTextChanged="IGSTPer_TextChanged" placeholder="%"></asp:TextBox>
                                                                        <asp:TextBox ID="IGSTAmt" Width="100px" runat="server" placeholder="IGSTAmt"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtTotalamt" Width="100px" runat="server" ReadOnly="true"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:TextBox ID="txtremarkspar" Width="250px" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                                    </td>
                                                                    <td>
                                                                        <asp:Button ID="btnAddMore" CssClass="btn btn-success btn-sm btncss" OnClick="Insert" runat="server" Text="+ Add" />
                                                                    </td>
                                                                </tr>

                                                            </table>
                                                        </div>
                                                        <div class="row" id="divdtls">
                                                            <div class="table-responsive">
                                                                <asp:GridView ID="dgvParticularsDetails" runat="server" CssClass="table" HeaderStyle-BackColor="#009999" AutoGenerateColumns="false"
                                                                    EmptyDataText="No records has been added." OnRowCommand="dgvParticularsDetails_RowCommand" OnRowEditing="dgvParticularsDetails_RowEditing" OnRowDataBound="dgvParticularsDetails_RowDataBound" ShowFooter="true">
                                                                    <Columns>
                                                                        <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                                                <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Invoice No" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblinvoice" runat="Server" Text='<%# Eval("InvoiceNo") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Particulars" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblParticulars" runat="Server" Text='<%# Eval("Particulars") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("Description") %>' Width="50px" ID="txtDescr" runat="server"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblDescription" runat="Server" Text='<%# Eval("Description") %>' />

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblHSN" runat="Server" Text='<%# Eval("HSN") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Qty" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("Qty") %>' Width="50px" ID="txtQty" runat="server" AutoPostBack="true" OnTextChanged="txtQty_TextChanged1"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblQty" runat="Server" Text='<%# Eval("Qty") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblRate" runat="Server" Text='<%# Eval("Rate") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Amount" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblAmount" runat="Server" Text='<%# Eval("Amount") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="UOM" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUOM" runat="Server" Text='<%# Eval("UOM") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Discount" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("Discount") %>' Width="50px" ID="txtdisc" runat="server" OnTextChanged="txtdisc_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lbldisc" runat="Server" Text='<%# Eval("Discount") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="CGSTPer" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("CGSTPer") %>' Width="50px" ID="txtCGSTPer" runat="server" OnTextChanged="txtCGSTPer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblCGSTPer" runat="Server" Text='<%# Eval("CGSTPer") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="CGSTAmt" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("CGSTAmt") %>' Width="100px" ID="txtCGSTAmt" runat="server"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblCGSTAmt" runat="Server" Text='<%# Eval("CGSTAmt") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="SGSTPer" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("SGSTPer") %>' Width="50px" ID="txtSGSTPer" runat="server" OnTextChanged="txtSGSTPer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblSGSTPer" runat="Server" Text='<%# Eval("SGSTPer") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="SGSTAmt" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("SGSTAmt") %>' Width="100px" ID="txtSGSTAmt" runat="server"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblSGSTAmt" runat="Server" Text='<%# Eval("SGSTAmt") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="IGSTPer" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("IGSTPer") %>' Width="50px" ID="txtIGSTPer" runat="server" OnTextChanged="txtIGSTPer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblIGSTPer" runat="Server" Text='<%# Eval("IGSTPer") %>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="IGSTAmt" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                            <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("IGSTAmt") %>' Width="100px" ID="txtIGSTAmt" runat="server"></asp:TextBox>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblIGSTAmt" runat="Server" Text='<%# Eval("IGSTAmt") %>' />
                                                                            </ItemTemplate>
                                                                            <FooterTemplate>
                                                                                <asp:Label ID="lblGrand" Font-Bold="true" runat="server" Text="Grand Total"></asp:Label>
                                                                            </FooterTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Total Amount" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Left">
                                                                            <%-- <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("TotalAmount") %>' ID="txtTotalAmount" runat="server" ReadOnly="true"></asp:TextBox>
                                                                            </EditItemTemplate>--%>
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblTotalAmount" runat="Server" Text='<%# Eval("TotalAmount") %>' />
                                                                            </ItemTemplate>
                                                                            <FooterTemplate>
                                                                                <asp:Label ID="lbltotal" Font-Bold="true" runat="server" Text="Label"></asp:Label>
                                                                            </FooterTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" Width="100" ID="txtremarks" Text='<%# Eval("Remarks") %>' TextMode="MultiLine"></asp:TextBox>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField ItemStyle-Width="120">
                                                                            <EditItemTemplate>
                                                                                <asp:LinkButton Text="Update" ID="lnkbtnUpdate" ClientIDMode="Static" runat="server" OnClick="lnkbtnUpdate_Click" ToolTip="Update"><i class="fa fa-refresh" style="font-size:28px;color:green;"></i></asp:LinkButton>
                                                                                |
                                                                            <asp:LinkButton Text="Cancel" ID="lnkCancel" runat="server" OnClick="lnkCancel_Click" ToolTip="Cancel"><i class="fa fa-close" style="font-size:28px;color:red;"></i></asp:LinkButton>
                                                                            </EditItemTemplate>
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton Text="Edit" runat="server" CommandName="Edit" ToolTip="Edit"><i class="fa fa-edit" style="font-size:28px;color:blue;"></i></asp:LinkButton>
                                                                                | 
                                                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandArgument='<%# Eval("id") %>' OnClick="lnkDelete_Click" ToolTip="Delete"><i class="fa fa-trash" style="font-size:28px;color:red"></i></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                        <br />
                                                        <br />
                                                    </div>
                                                    <div class="col-md-12" runat="server" id="DivAutomatic">
                                                        <div class="table-responsive">
                                                            <asp:GridView ID="dgvAutomatic" runat="server" CssClass="table" HeaderStyle-BackColor="#009999" AutoGenerateColumns="false"
                                                                EmptyDataText="No records has been added." ShowFooter="true" OnRowDataBound="dgvAutomatic_RowDataBound" DataKeyNames="HeaderID">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Select" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <HeaderTemplate>
                                                                            <asp:CheckBox ID="chkHeader" runat="server" OnCheckedChanged="chkHeader_CheckedChanged" AutoPostBack="true" />
                                                                        </HeaderTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelect" OnCheckedChanged="chkSelect_CheckedChanged" runat="server" AutoPostBack="true" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                                            <asp:Label ID="lblID" runat="server" Text='<%# Eval("Id") %>' Visible="false"></asp:Label>
                                                                            <asp:Label ID="lblheaderId" runat="Server" Text='<%# Eval("HeaderID") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Invoice No" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <%--<asp:Label ID="txtParticulars" ReadOnly="true" runat="server" Text='<%# Eval("Particulars") %>'></asp:Label>--%>
                                                                            <asp:Label ID="lblinvoice" ReadOnly="true" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Particulars" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <%--<asp:Label ID="txtParticulars" ReadOnly="true" runat="server" Text='<%# Eval("Particulars") %>'></asp:Label>--%>
                                                                            <asp:Label ID="txtParticulars" ReadOnly="true" runat="server" Text='<%# Eval("Particular") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Description" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" Text='<%# Eval("Description") %>'></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="HSN" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="50" ID="txtHSN" Text='<%# Eval("HSN") %>' Enabled="false"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Qty" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="50" Enabled="false" ID="txtautQty" Text='<%# Eval("Qty") %>' OnTextChanged="txtautQty_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Rate" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="50" ID="txtRate" OnTextChanged="txtRate_TextChanged1" AutoPostBack="true" Text='<%# Eval("Rate") %>' Enabled="false"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Disc(%)" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="50" ID="txtDiscPer" Text='<%# Eval("Discount") %>' OnTextChanged="txtDiscPer_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="UOM" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="50" Enabled="false" ID="txtUOM" Text='<%# Eval("UOM") %>'></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Amount" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <%--<asp:Label runat="server" ID="txtAmount" Text='<%# Eval("Amount") %>'></asp:Label>--%>
                                                                            <asp:Label runat="server" ID="txtAmount" Text='<%# Eval("TaxableAmt") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="CGST(%)" ItemStyle-Width="10" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="30" ID="txtCGST" Enabled="false" Text='<%# Eval("CGSTPer") %>' OnTextChanged="txtCGST_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="SGST(%)" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="30" ID="txtSGST" Enabled="false" Text='<%# Eval("SGSTPer") %>' OnTextChanged="txtSGST_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="IGST(%)" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="30" ID="txtIGST" Enabled="false" Text='<%# Eval("IGSTPer") %>' OnTextChanged="txtIGST_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Discount(%)" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center" Visible="false">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="50" ID="txtdiscount" Text="0" OnTextChanged="txtdiscount_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Grand Total" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="100" ID="txtGrandTotal" ReadOnly="true" Text='<%# Eval("GrandTotal") %>'></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>


                                                                    <asp:TemplateField HeaderText="Remarks" ItemStyle-Width="20" ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox runat="server" Width="100" ID="txtremarks" Text='<%# Eval("Remarks") %>' TextMode="MultiLine"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                        <br />
                                                        <br />
                                                    </div>
                                                </div>

                                                <br />
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
                                                <div class="table-responsive text-center">
                                                    <div class="row">
                                                        <div class="table-responsive text-center">
                                                            <asp:GridView ID="gvcomponent" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys" OnRowEditing="gvcomponent_RowEditing"
                                                                AutoGenerateColumns="false">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblComsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                                            <asp:Label ID="lblComid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Product" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("Particular") %>' ReadOnly="true" CssClass="form-control" Width="230px" ID="txtCOMPParticular" runat="server"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblproduct" runat="Server" Text='<%# Eval("Particular") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Component" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("ComponentName") %>' ReadOnly="true" CssClass="form-control" Width="230px" ID="txtCOMPComponent" runat="server"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblComComPonent" runat="Server" Text='<%# Eval("ComponentName") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Batch" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("Batch") %>' ReadOnly="true" CssClass="form-control" Width="230px" ID="txtCOMPBatch" runat="server"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblComBatch" runat="Server" Text='<%# Eval("Batch") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                                        <EditItemTemplate>
                                                                            <asp:TextBox Text='<%# Eval("Quantity") %>' CssClass="form-control" ID="txtCOMPQuantity" Width="100px" runat="server"></asp:TextBox>
                                                                        </EditItemTemplate>
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblComQuantity" runat="Server" Text='<%# Eval("Quantity") %>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                                        <ItemTemplate>
                                                                            <%--<asp:LinkButton ID="btn_edit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>--%>

                                                                            <asp:LinkButton ID="btn_Compedit" CausesValidation="false" Text="Edit" runat="server" CommandName="Edit"><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>

                                                                            <asp:LinkButton runat="server" ID="lnkbtnCompDelete" OnClick="lnkbtnCompDelete_Click" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px"></i></asp:LinkButton>
                                                                        </ItemTemplate>
                                                                        <EditItemTemplate>
                                                                            <asp:LinkButton ID="gv_Compupdate" OnClick="gv_Compupdate_Click" CausesValidation="false" Text="Update" CssClass="btn btn-primary btn-sm" runat="server"></asp:LinkButton>&nbsp;
                                             
                                                                        </EditItemTemplate>
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                            </asp:GridView>
                                                        </div>
                                                    </div>
                                                </div>


                                                <%--23-12-23  New Transportation & Fright Functanility start--%>
                                                <div class="table-responsive">
                                                    <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                        <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                                            <td style="width: 50%;">Charges Description</td>
                                                            <td>HSN</td>
                                                            <td>Rate(%)</td>
                                                            <td>Basic</td>
                                                            <td>CGST</td>
                                                            <td>SGST</td>
                                                            <td>IGST</td>
                                                            <td>Cost</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox ID="TxtChargesDesc" Width="250px" runat="server" TextMode="MultiLine" Text="Freight"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txthsntcs" Width="100px" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtrateTcs" Width="100px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="txtrateTcs_TextChanged"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtBasic" Width="100px" runat="server" Text="0" Enabled="true" OnTextChanged="txtBasic_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="CGSTPertcs" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="CGSTPertcs_TextChanged"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="SGSTPertcs" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="SGSTPertcs_TextChanged"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="IGSTPertcs" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="IGSTPertcs_TextChanged"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtCost" Width="100px" runat="server" Enabled="false" Text="0"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                                <%--23-12-23  New Transportation & Fright Functanility End--%>

                                                <div class="row">
                                                    <div class="col-md-4"></div>
                                                    <div class="col-md-6">
                                                        <div class="col-md-4 spancls">Grand Total:</div>
                                                        <div class="col-md-4">
                                                            <asp:TextBox ID="txtFGrandTot" CssClass="form-control" runat="server" Width="100%" AutoComplete="off" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-md-2"></div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2" style="margin-left: 18%;"></div>
                                                    <div class="col-md-2">
                                                        <center>
                                                            <asp:Button ID="btnadd" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Width="100%" Text="Submit" OnClick="btnadd_Click" />
                                                        </center>
                                                    </div>
                                                    <div class="col-md-2">
                                                        <center>
                                                            <asp:Button ID="btnreset" runat="server" CssClass="btn btn-danger" Width="100%" Text="Reset" OnClick="btnreset_Click" />
                                                        </center>
                                                    </div>
                                                    <div class="col-md-6"></div>
                                                </div>
                                                <br />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnadd" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>
