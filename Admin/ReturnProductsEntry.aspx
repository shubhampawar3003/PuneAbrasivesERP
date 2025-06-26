<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="ReturnProductsEntry.aspx.cs" Inherits="Admin_ReturnProductsEntry" %>

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
    </style>
    <style>
        .LblStyle {
            font-weight: 500;
            color: black;
            font: bold;
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
            color: #000000;
            background-color: #31B0C4;
            font-weight: bolder;
            font-size: medium;
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

        .spncls {
            color: red;
        }


        .form-control, .dataTable-input {
            display: initial !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">

                <div class="container-fluid px-3">
                    <div class="row">
                        <div class="col-9 col-md-10">
                            <h4 class="mt-4">&nbsp <b>RETURN PRODUCT ENTRY</b></h4>
                        </div>
                        <div class="col-md-2 mt-4">
                               <asp:Button ID="Button1" CausesValidation="false" CssClass="form-control btn btn-warning" Font-Bold="true" runat="server" Text="LIST" OnClick="Button1_Click" />
                        
                        </div>
                    </div>
                    <hr />
                    <div class="card mb-4">
                        <div class="card-body ">
                            <asp:HiddenField ID="hfCustomerId" runat="server" />
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Order No. : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtorderno" ReadOnly="true" CssClass=" uppercase  form-control" Width="100%" runat="server" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ValidationGroup="invoice" runat="server" ControlToValidate="txtorderno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Order No." Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label2" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice No. : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtinvoiceno" AutoComplete="off" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ValidationGroup="invoice" runat="server" ControlToValidate="txtinvoiceno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Invoice No" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label3" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice Date.: <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtinvoicedate" CssClass="form-control" TextMode="Date" runat="server" Width="100%"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ValidationGroup="invoice" runat="server" ControlToValidate="txtinvoicedate"
                                        ForeColor="Red" ErrorMessage="* Please Enter Invoice Date"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label4" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Supplier Name : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtsupliername" CssClass=" uppercase  form-control" AutoPostBack="true" OnTextChanged="txtsupliername_TextChanged" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator12" ValidationGroup="invoice" runat="server" ControlToValidate="txtsupliername"
                                        ForeColor="Red" ErrorMessage="* Please Enter Supplier Name" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender2" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList" TargetControlID="txtsupliername" runat="server">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Mobile No. : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtmobileno" utoComplete="off" CssClass="form-control" ReadOnly="true" runat="server" MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator9" ValidationGroup="invoice" runat="server" ControlToValidate="txtmobileno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Mobile No." Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">

                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label6" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Address : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtSuplieraddress" CssClass="form-control" runat="server" ReadOnly="true" TextMode="MultiLine" Width="100%"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator15" ValidationGroup="invoice" runat="server" ControlToValidate="txtSuplieraddress"
                                        ForeColor="Red" ErrorMessage="* Please Enter Suplier address"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label7" runat="server" Font-Bold="true" CssClass="form-label LblStyle">State : <span class="spncls">*</span></asp:Label>
                                    <asp:TextBox ID="txtState" AutoComplete="off" CssClass=" uppercase  form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator14" ValidationGroup="invoice" runat="server" ControlToValidate="txtState"
                                        ForeColor="Red" ErrorMessage="* Please Enter State" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="true" CssClass="form-label LblStyle"> GST No.  : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtGSTNO" AutoComplete="off" CssClass=" uppercase  form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator11" ValidationGroup="invoice" runat="server" ControlToValidate="txtGSTNO"
                                        ForeColor="Red" ErrorMessage="* Please Enter GST No." Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label9" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Pan No  : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtPanno" CssClass="form-control" runat="server" ReadOnly="true"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator13" ValidationGroup="invoice" runat="server" ControlToValidate="txtPanno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Pan No"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label10" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Challan No.: <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtchallanno" AutoComplete="off" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator10" ValidationGroup="invoice" runat="server" ControlToValidate="txtchallanno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Challan No." Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label11" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Challan Date : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtchallandate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="invoice" runat="server" ControlToValidate="txtchallandate"
                                        ForeColor="Red" ErrorMessage="* Please Enter Challan Date"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label12" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Transport Name : </asp:Label>
                                    <asp:DropDownList ID="ddltransportname" runat="server" CssClass="form-control"></asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label13" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Inward Time : <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtinwardtime" CssClass="form-control" Width="100%" runat="server"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="MaskedEditExtender1" TargetControlID="txtinwardtime" Mask="99:99" MaskType="Time" AcceptAMPM="true" MessageValidatorTip="true" runat="server">
                                    </asp:MaskedEditExtender>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" ValidationGroup="invoice" runat="server" ControlToValidate="txtinwardtime"
                                        ForeColor="Red" ErrorMessage="* Please Enter Inward Time" Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label14" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Material Receive By : </asp:Label>

                                    <asp:TextBox ID="txtmaterialrecivedby" CssClass=" uppercase  form-control" ReadOnly="true" Width="100%" runat="server"></asp:TextBox>
                               
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label15" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Vehicle No: <span class="spncls">*</span></asp:Label>

                                    <asp:TextBox ID="txtvehicleno" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="invoice" runat="server" ControlToValidate="txtvehicleno"
                                        ForeColor="Red" ErrorMessage="* Please Enter Vehicle No " Display="Dynamic" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label16" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Material Description: </asp:Label>

                                    <asp:TextBox ID="txtmaterialdescription" CssClass=" uppercase  form-control" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>

                            </div>


                        </div>

                        <div>
                            <hr />
                        </div>
                        <div class="card-body ">
                            <div class="col-md-12">
                                <div class="card-header head" style="margin-top: 10px;">
                                    <h4 style="color: black">Component Details</h4>
                                </div>
                                <br />
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="table-responsive">
                                            <table class="table text-center align-middle table-bordered table-hover" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                <thead>
                                                    <tr class="gvhead">
                                                        <td>Component</td>
                                                        <td>Batch</td>
                                                        <td>Description</td>
                                                        <td>HSN / SAC</td>
                                                        <td>Quantity</td>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td>
                                                            <asp:DropDownList ID="ddlcomponent" Width="230px" OnTextChanged="ddlcomponent_TextChanged" CssClass="form-control" AutoPostBack="true" runat="server"></asp:DropDownList>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtBatch" Width="230px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComDiscription" Width="230px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComHSN" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComQuantity" Width="190px" CssClass="form-control" OnTextChanged="txtComQuantity_TextChanged" AutoPostBack="true" runat="server"></asp:TextBox>
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
                                                            <asp:TextBox ID="txtComUnit" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComRate" Width="190px" AutoPostBack="true" OnTextChanged="txtComRate_TextChanged" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComTotal" Width="100" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComCGST" placeholder="%" Width="100px" OnTextChanged="txtComCGST_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComCGSTAMT" Width="100px" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
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
                                                            <asp:TextBox ID="txtComSGST" placeholder="%" Width="100px" OnTextChanged="txtComSGST_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComSGSTAMT" Width="100px" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>

                                                        <td>
                                                            <asp:TextBox ID="txtComIGST" OnTextChanged="txtComIGST_TextChanged" AutoPostBack="true" placeholder="%" Width="100px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComIGSTAMT" Width="100px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtComDiscount" OnTextChanged="txtComDiscount_TextChanged" Width="80px" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                            <asp:TextBox ID="txtComDiscountAMT" Visible="false" Width="80px" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
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
                                                            <asp:TextBox ID="txtComGrandTotal" Width="150px" CssClass="form-control" runat="server"></asp:TextBox>
                                                        </td>

                                                        <td>
                                                            <asp:Button ID="txtCombtn_Addmore" CausesValidation="false" OnClick="txtCombtn_Addmore_Click" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add More" />
                                                        </td>

                                                    </tr>
                                                </tbody>

                                            </table>
                                        </div>

                                        <%--<div class="row" id="divdtls">--%>
                                        <div class="table-responsive text-center">
                                            <asp:GridView ID="gvcomponent" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                                OnRowEditing="gvcomponent_RowEditing" OnRowDataBound="gvcomponent_RowDataBound" AutoGenerateColumns="false">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                            <asp:Label ID="lblComid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
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
                                                            <asp:TextBox Text='<%# Eval("Batch") %>' CssClass="form-control" Width="230px" ID="txtCOMPBatch" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComBatch" runat="Server" Text='<%# Eval("Batch") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Description") %>' CssClass="form-control" ID="txtCOMPDescription" Width="200px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComDescription" runat="Server" Text='<%# Eval("Description") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("HSN") %>' ReadOnly="true" CssClass="form-control" ID="txtCOMPhsn" Width="200px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComhsn" runat="Server" Text='<%# Eval("HSN") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Quantity") %>' CssClass="form-control" ID="txtCOMPQuantity" OnTextChanged="txtCOMPQuantity_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComQuantity" runat="Server" Text='<%# Eval("Quantity") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Unit" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Units") %>' ReadOnly="true" CssClass="form-control" ID="txtCOMPUnit" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComUnit" runat="Server" Text='<%# Eval("Units") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Rate") %>' CssClass="form-control" OnTextChanged="txtCOMPRate_TextChanged" AutoPostBack="true" ID="txtCOMPRate" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComRate" runat="Server" Text='<%# Eval("Rate") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Total" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Total") %>' CssClass="form-control" ID="txtCOMPTotal" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComTotal" runat="Server" Text='<%# Eval("Total") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="CGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("CGSTPer") %>' CssClass="form-control" OnTextChanged="txtCOMPCGSTPer_TextChanged" AutoPostBack="true" ID="txtCOMPCGSTPer" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComCGSTPer" runat="Server" Text='<%# Eval("CGSTPer") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="CGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("CGSTAmt") %>' CssClass="form-control" ID="txtCOMPCGST" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComCGST" runat="Server" Text='<%# Eval("CGSTAmt") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("SGSTPer") %>' CssClass="form-control" ID="txtCOMPSGSTPer" OnTextChanged="txtCOMPSGSTPer_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComSGSTPer" runat="Server" Text='<%# Eval("SGSTPer") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="SGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("SGSTAmt") %>' CssClass="form-control" ID="txtCOMPSGST" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComSGST" runat="Server" Text='<%# Eval("SGSTAmt") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="IGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("IGSTPer") %>' CssClass="form-control" ID="txtCOMPIGSTPer" OnTextChanged="txtCOMPIGSTPer_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComIGSTPer" runat="Server" Text='<%# Eval("IGSTPer") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="IGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("IGSTAmt") %>' CssClass="form-control" ID="txtCOMPIGST" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComIGST" runat="Server" Text='<%# Eval("IGSTAmt") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Discount(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Discountpercentage") %>' CssClass="form-control" ID="txtCOMPDiscount" OnTextChanged="txtCOMPDiscount_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComDiscount" runat="Server" Text='<%# Eval("Discountpercentage") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Discount Amount" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("DiscountAmount") %>' CssClass="form-control" ReadOnly="true" ID="txtCOMPDiscountAmount" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComDiscountAmount" runat="Server" Text='<%# Eval("DiscountAmount") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Grand Total" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <EditItemTemplate>
                                                            <asp:TextBox Text='<%# Eval("Alltotal") %>' CssClass="form-control" ID="txtCOMPAlltotal" Width="100px" runat="server"></asp:TextBox>
                                                        </EditItemTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblComAlltotal" runat="Server" Text='<%# Eval("Alltotal") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <%--<asp:LinkButton ID="btn_edit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>--%>

                                                            <asp:LinkButton ID="btn_Compedit" CausesValidation="false" Text="Edit" runat="server" CommandName="Edit"><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>

                                                            <asp:LinkButton runat="server" ID="lnkbtnCompDelete" OnClick="lnkbtnCompDelete_Click" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px"></i></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:LinkButton ID="gv_Compupdate" OnClick="gv_Compupdate_Click" Text="Update" CssClass="btn btn-primary btn-sm" runat="server"></asp:LinkButton>&nbsp;
                                                        <asp:LinkButton ID="gv_Compcancel" OnClick="gv_Compcancel_Click" CausesValidation="false" Text="Cancel" CssClass="btn btn-primary btn-sm " runat="server"></asp:LinkButton>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                    <br />
                                </div>
                            </div>
                        </div>
                        <div id="divCOMPTotalPart" visible="false" runat="server">
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <br />

                                            <center>
                                        <div class="col-md-12">
                                            <asp:Label ID="lblCOMP_total_amt" runat="server" class="control-label col-sm-6">Total Amount (In Words) :<span class="spncls"></span></asp:Label><br />
                                            <asp:Label ID="lblCOMP_total_amt_Value" ForeColor="red" class="control-label col-sm-6 font-weight-bold" runat="server" Text=""></asp:Label>
                                             <asp:HiddenField ID="hfCOMPTotal" runat="server" />
                                        </div>
                                            </center>
                                        </div>
                                        <div class="col-md-6" style="text-align: right">
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblCOMP_Subtotal" runat="server" class="control-label col-sm-6">SubTotal :<span class="spncls"></span></asp:Label>
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txtCOMP_Subtotal" Text="0.00"></asp:Label><br />
                                                </div>
                                            </div>
                                            <asp:Panel ID="taxCOMPPanel1" runat="server">
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lblCOMP_cgst9" runat="server" class="control-label col-sm-6">CGST  Amount :<span class="spncls"></span></asp:Label>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txtCOMP_cgstamt" Text="0.00"></asp:Label><br />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lblCOMP_sgst9" runat="server" class="control-label col-sm-6">SGST  Amount :<span class="spncls"></span></asp:Label>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txtCOMP_sgstamt" Text="0.00"></asp:Label><br />
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lblCOMPigst" runat="server" class="control-label col-sm-6">IGST  Amount :<span class="spncls"></span></asp:Label>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txtCOMP_igstamt" Text="0.00"></asp:Label><br />
                                                    </div>
                                                </div>

                                            </asp:Panel>
                                            <div class="row">
                                                <div class="col-md-6">
                                                    <asp:Label ID="lblCOMP_grandTotal" runat="server" class="control-label col-sm-6">Grand Total :<span class="spncls"></span></asp:Label>
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txtCOMP_grandTotal" Text="0.00"></asp:Label><br />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <div class="col-md-4"></div>
                            <div class="col-6 col-md-2">
                                <asp:Button ID="btnsave" OnClick="btnsave_Click" ValidationGroup="invoice" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
                            </div>
                            <div class="col-6 col-md-2">
                                <asp:Button ID="btncancel" OnClick="btncancel_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                            </div>
                            <div class="col-md-4"></div>
                        </div>
                        <div>
                            <br />
                            <br />
                            <br />
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hhd" runat="server" />
            </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnsave" />
            <asp:PostBackTrigger ControlID="btncancel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

