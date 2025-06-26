<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="EnquiryMaster.aspx.cs" Inherits="Admin_EnquiryMaster" %>

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
             $("[id*=ddlBStateCode]").select2();
        });
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

    <script>
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <div class="container-fluid px-4">
        <div class="row">
            <div class="col-md-10">
                <h4 class="mt-4">&nbsp <b>ADD ENQUIRY</b></h4>
            </div>
            <div class="col-md-2 mt-4">
                <asp:LinkButton ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="Button1_Click">
    <i class="fas fa-file-alt"></i> ENQUIRY LIST
                </asp:LinkButton>
            </div>
        </div>
        <br />
        <div class="container-fluid px-3">
            <div class="card mb-4">
                <div class="card-header LblStyle">
                    <i class="fa fa-cog"></i>
                    Company Details:                                             
                </div>
                <div class="card-body ">
                    <div class="row">
                        <div class="col-md-6 col-12 mb-3">
                            <asp:HiddenField ID="CompanycodeID" runat="server" />
                            <asp:Label ID="lblbrandname" runat="server" CssClass="form-label LblStyle">Company Name : <span class="spncls">*</span></asp:Label>
                            <%--     <asp:DropDownList ID="ddlCompanyname" CssClass="form-control" OnSelectedIndexChanged="ddlCompanyname_SelectedIndexChanged" runat="server" AutoPostBack="true">
                                    </asp:DropDownList>--%>
                            <asp:TextBox ID="txtcompanyname" OnTextChanged="txtcompanyname_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                ControlToValidate="txtcompanyname" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCompanyList"
                                TargetControlID="txtcompanyname" Enabled="true">
                            </asp:AutoCompleteExtender>
                        </div>
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="Label1" runat="server" CssClass="form-label LblStyle">Owner Name : </asp:Label>

                            <asp:TextBox ID="txtownname" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                        </div>
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="lblcontactno" runat="server" CssClass="form-label LblStyle">Contact No. :</asp:Label>
                            <asp:TextBox ID="txtcontactno" CssClass="form-control" onkeypress="return isNumberKey(event)" MaxLength="12" MinLength="10" runat="server" Width="100%"></asp:TextBox>
                        </div>
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="lblarea" runat="server" CssClass="form-label LblStyle">Area :</asp:Label>
                            <asp:TextBox ID="txtarea" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-6 col-12 mb-3">

                            <asp:Label ID="Label5" runat="server" CssClass="form-label LblStyle">State : <span class="spncls">*</span></asp:Label>
                            <asp:DropDownList ID="ddlBStateCode" ValidationGroup="form1" CssClass="form-control" runat="server">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please select state"
                                ControlToValidate="ddlBStateCode" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="lbladdress" runat="server" CssClass="form-label LblStyle">Billing Address :</asp:Label>
                            <asp:TextBox ID="txtaddress" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="lblEnquiry" runat="server" CssClass="form-label LblStyle">Enquiry Specification :</asp:Label>
                            <asp:TextBox ID="txtremark" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </div>
                        <div class="col-md-6 col-12 mb-3" id="DivMailID" runat="server">
                            <asp:Label ID="lblmail" runat="server" CssClass="form-label LblStyle">Mail ID :</asp:Label>
                            <asp:TextBox ID="txtmail" CssClass="form-control" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="txtccode" runat="server" Visible="false"></asp:Label>
                            <asp:Label ID="txtcid" runat="server" Text="Label" Visible="false"></asp:Label>
                        </div>
                    </div>
                </div>


            </div>

            <div class="card mb-4">
                <div class="card-body">
                    <div class="card-header head" style="margin-top: 10px;">
                        <h5 style="color: white">Product Details</h5>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-12">
                            <div class="table-responsive">
                                <table class="table text-center align-middle table-bordered table-hover" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                    <thead>
                                        <tr class="gvhead">
                                            <td>Product</td>
                                            <td>Description</td>
                                            <td>HSN / SAC</td>
                                            <td>Quantity</td>
                                            <td></td>

                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlProduct" Width="230px" OnTextChanged="ddlProduct_TextChanged"
                                                    CssClass="form-control" AutoPostBack="true" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtdescription" Width="230px" CssClass="form-control" runat="server"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txthsnsac" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtquantity" Width="190px" CssClass="form-control" OnTextChanged="txtquantity_TextChanged" AutoPostBack="true" runat="server"></asp:TextBox>
                                            </td>
                                            <td></td>
                                        </tr>

                                    </tbody>
                                    <thead class="gvhead">
                                        <tr>
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
                                                <asp:Button ID="btnAddMore" OnClick="btnAddMore_Click" CausesValidation="false" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add More" />
                                            </td>

                                        </tr>
                                    </tbody>

                                </table>
                            </div>


                            <%--<div class="row" id="divdtls">--%>
                            <div class="table-responsive text-center">
                                <asp:GridView ID="dgvMachineDetails" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                    OnRowEditing="dgvMachineDetails_RowEditing" OnRowDataBound="dgvMachineDetails_RowDataBound" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                            <ItemTemplate>
                                                <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Product" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                            <EditItemTemplate>
                                                <asp:TextBox Text='<%# Eval("Productname") %>' CssClass="form-control" Width="230px" ID="txtproduct" runat="server"></asp:TextBox>
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
                                                <%--<asp:LinkButton ID="btn_edit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>--%>

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
                </div>
            </div>
            <div class="card mb-4">
                <div class="card-body">
                    <div class="row">
                        <asp:HiddenField ID="HFccode" runat="server" />
                        <asp:HiddenField ID="HFfile1" runat="server" />
                        <asp:HiddenField ID="HFfile2" runat="server" />
                        <asp:HiddenField ID="HFfile3" runat="server" />
                        <asp:HiddenField ID="HFfile4" runat="server" />
                        <asp:HiddenField ID="HFfile5" runat="server" />
                        <asp:HiddenField ID="hfregby" runat="server" />
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-6">
                            <asp:Label ID="lblSample" runat="server" CssClass="form-label LblStyle">Sample : <span class="spncls">*</span></asp:Label>

                            <asp:RadioButtonList ID="rdsample" Font-Bold="true" AutoPostBack="true" OnSelectedIndexChanged="rdsample_SelectedIndexChanged" runat="server" CssClass="form-control">
                                <asp:ListItem  Value="1">&nbsp Yes</asp:ListItem>
                                <asp:ListItem Value="0">&nbsp No</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" Display="Dynamic" ErrorMessage="Please select sample"
                                ControlToValidate="rdsample" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <br />
                    <div class="row" id="divsampledate" runat="server" visible="false">
                        <div class="col-md-6 col-12 mb-3">
                            <asp:Label ID="Label4" runat="server" Font-Bold="true" CssClass="form-label">Sample Date(Sample send to client)  :</asp:Label>

                            <asp:TextBox ID="txtSampledate" Placeholder="Enter Sample Date." runat="server" AutoComplete="off" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please fill Sample Date." ControlToValidate="txtSampledate" ForeColor="Red"></asp:RequiredFieldValidator>

                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <%--<div class="col-md-2"></div>--%>
                        <div class="col-md-2 LblStyle">File 1 : </div>
                        <div class="col-md-4">
                            <asp:FileUpload ID="FileUpload1" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="lblfile1" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-1">
                            <asp:ImageButton ID="ImageButtonfile1" runat="server" Visible="false" OnClick="ImageButtonfile1_Click" ImageUrl="../Content/img/delete-icon.png" Width="22px" />
                        </div>
                        <div class="col-md-1"></div>
                    </div>

                    <br />
                    <div class="row">
                        <%-- <div class="col-md-2"></div>--%>
                        <div class="col-md-2 LblStyle">File 2 : </div>
                        <div class="col-md-4">
                            <asp:FileUpload ID="FileUpload2" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="lblfile2" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-1">
                            <asp:ImageButton ID="ImageButtonfile2" runat="server" Visible="false" OnClick="ImageButtonfile2_Click" ImageUrl="../img/delete-icon.png" Width="22px" />
                        </div>
                        <div class="col-md-1"></div>
                    </div>

                    <br />
                    <div class="row">
                        <%--<div class="col-md-2"></div>--%>
                        <div class="col-md-2 LblStyle">File 3 : </div>
                        <div class="col-md-4">
                            <asp:FileUpload ID="FileUpload3" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="lblfile3" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-1">
                            <asp:ImageButton ID="ImageButtonfile3" runat="server" Visible="false" OnClick="ImageButtonfile3_Click" ImageUrl="../img/delete-icon.png" Width="22px" />
                        </div>
                        <div class="col-md-1"></div>
                    </div>

                    <br />
                    <div class="row">
                        <%--<div class="col-md-2"></div>--%>
                        <div class="col-md-2 LblStyle">File 4 : </div>
                        <div class="col-md-4">
                            <asp:FileUpload ID="FileUpload4" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="lblfile4" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-1">
                            <asp:ImageButton ID="ImageButtonfile4" runat="server" Visible="false" OnClick="ImageButtonfile4_Click" ImageUrl="../img/delete-icon.png" Width="22px" />
                        </div>
                        <div class="col-md-1"></div>
                    </div>

                    <br />
                    <div class="row">
                        <%--<div class="col-md-2"></div>--%>
                        <div class="col-md-2 LblStyle">File 5 : </div>
                        <div class="col-md-4">
                            <asp:FileUpload ID="FileUpload5" runat="server" CssClass="form-control" />
                        </div>
                        <div class="col-md-2">
                            <asp:Label ID="lblfile5" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-1">
                            <asp:ImageButton ID="ImageButtonfile5" runat="server" Visible="false" OnClick="ImageButtonfile5_Click" ImageUrl="../img/delete-icon.png" Width="22px" />
                        </div>
                        <div class="col-md-1"></div>
                    </div>

                    <br />


                    <br />
                    <div class="row">
                        <div class="col-md-4"></div>
                        <div class="col-6 col-md-2">

                            <center> <asp:Button ID="btnadd" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Width="100%" Text="Add Enquiry" OnClick="btnadd_Click"/></center>
                        </div>
                        <div class="col-6 col-md-2">
                            <asp:Button ID="btncancel" OnClick="btncancel_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                        </div>
                        <div class="col-md-4"></div>
                    </div>
                </div>
            </div>
            <asp:HiddenField ID="hhd" runat="server" />
        </div>
    </div>



</asp:Content>
