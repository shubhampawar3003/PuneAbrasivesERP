<%@ Page Title="" Debug="true" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="PurchaseOrder.aspx.cs" Inherits="Purchase_PurchaseOrder" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
    <script type="text/javascript">
        $(function () {

            $("[id*=ddlcomponent]").select2();


        });
    </script>
    <style>
        .dissablebtn {
            cursor: not-allowed;
        }
    </style>
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
            background-color: #009999;
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
                    <div class="col-md-12 card-header">
                        <div class="row">
                            <div class="col-md-10">
                                <h4 class="mt-4">&nbsp <b>CREATE PURCHASE ORDER</b></h4>
                            </div>
                            <div class="col-md-2 mt-4">
                                <asp:LinkButton ID="LinkButton1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton1_Click">
    <i class="fas fa-file-alt"></i>&nbsp&nbsp P.O. List
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="container py-3">
                        <div class="card">

                            <div class="row">
                                <div class="col-xl-12 col-md-12">
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Supplier Name</b><i class="reqcls">*&nbsp;</i> : </div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtSupplierName" CssClass="form-control" runat="server" Width="100%" OnTextChanged="txtSupplierName_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Supplier Name"
                                                            ControlToValidate="txtSupplierName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                        <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList"
                                                            TargetControlID="txtSupplierName">
                                                        </asp:AutoCompleteExtender>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Kind. att:</b></div>
                                                    <div class="col-md-4">

                                                        <asp:HiddenField runat="server" ID="hdnfileData" />
                                                        <asp:HiddenField runat="server" ID="hdnGrandtotal" />
                                                        <asp:DropDownList runat="server" ID="ddlKindAtt" CssClass="form-control"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="row" >
                                                    <div  runat="server" visible="false" class="col-md-2 spancls"><b>PO No:</b></div>
                                                    <div  runat="server" visible="false" class="col-md-4">
                                                        <asp:TextBox ID="txtPONo" CssClass="form-control" runat="server" Width="100%" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>PO Date:<i class="reqcls">*&nbsp;</i></b></div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtPodate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>
                                                          <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Please Select Delivery Date"
                                                            ControlToValidate="txtPodate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                 
                                                        </div>
                                                </div>

                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Mode :</b> </div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddlMode" runat="server" class="form-control">
                                                            <asp:ListItem Text="Open"></asp:ListItem>
                                                            <asp:ListItem Text="Close"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Delivery Date :</b><i class="reqcls">*&nbsp;</i>:</div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtDeliverydate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off" OnTextChanged="txtDeliverydate_TextChanged" AutoPostBack="true"></asp:TextBox>

                                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Select Delivery Date"
                                                            ControlToValidate="txtDeliverydate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2 spancls"><b>Refer Quotation:</b></div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtReferQuotation" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Remarks:</b></div>
                                                    <div class="col-md-4">
                                                        <asp:TextBox ID="txtRemarks" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="row" runat="server">
                                                    <div class="col-md-2 spancls"><b>Order Close Mode : </b></div>
                                                    <div class="col-md-4">
                                                        <asp:DropDownList ID="ddlOrderCloseMode" CssClass="form-control" runat="server">
                                                            <asp:ListItem>Quantity</asp:ListItem>
                                                            <asp:ListItem>Amount</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                    <div class="col-md-2 spancls"><b>Send Mail:</b></div>
                                                    <div class="col-md-4">
                                                        <div class="row">
                                                            <div class="col-md-2">
                                                                <asp:CheckBox runat="server" ID="IsSedndMail" />
                                                            </div>
                                                            <div class="col-md-8">
                                                                <asp:Label runat="server" Font-Bold="true" ID="lblEmailID"></asp:Label>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-12 mb-3">
                                                        <hr />
                                                        <b style="color: red">*TERMS AND CONDITIONS</b>
                                                        <hr />
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-6 col-12 mb-3">
                                                            <asp:Label ID="lblDeliveryTime" runat="server" Font-Bold="true" CssClass="form-label">Delivery Time :</asp:Label>

                                                            <asp:TextBox ID="txtDeliveryTime" CssClass="form-control" Text="Door Delivery" placeholder="Enter Delivery Time" runat="server"></asp:TextBox>
                                                        </div>

                                                        <div class="col-md-6 col-12 mb-3">
                                                            <asp:Label ID="lblTransport" runat="server" Font-Bold="true" CssClass="form-label">Transport :</asp:Label>

                                                            <asp:TextBox ID="txtTransport" CssClass="form-control" Text="Our End" placeholder="Enter Transport" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-6 col-12 mb-3">

                                                            <asp:Label ID="lblPayment" runat="server" Font-Bold="true" CssClass="form-label">Payment Term :</asp:Label>

                                                            <asp:TextBox ID="txtPayment" CssClass="form-control" placeholder="Enter Payment " Text="30 Days Credit" runat="server"></asp:TextBox>
                                                        </div>
                                                        <div class="col-md-6 col-12 mb-3">
                                                            <asp:Label ID="lblPacking" runat="server" Font-Bold="true" CssClass="form-label">Packing :</asp:Label>

                                                            <asp:TextBox ID="txtPacking" CssClass="form-control" Text="As per our Standard - 25 Kg" placeholder="Enter Packing" runat="server"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-6 col-12 mb-3">
                                                            <asp:Label ID="lblTaxs" runat="server" Font-Bold="true" CssClass="form-label">Taxes :</asp:Label>

                                                            <asp:TextBox ID="txtTaxs" CssClass="form-control" Text="Applicable as above" placeholder="Enter Taxes" runat="server"></asp:TextBox>
                                                        </div>

                                                    </div>
                                                </div>


                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="card">
                            <div class="row">
                                <div class="col-xl-12 col-md-12">
                                    <div class="card-header">
                                        <div>
                                            <h5><b>Component Details</b></h5>
                                        </div>
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="row">
                                                            <div class="table-responsive">
                                                                <table class="table text-center align-middle table-bordered table-hover" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                                    <thead>
                                                                        <tr class="gvhead">
                                                                            <td>Component</td>
                                                                            <td>Particular</td>
                                                                            <td>HSN / SAC</td>
                                                                            <td>Quantity</td>
                                                                            <td>Unit</td>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:DropDownList ID="ddlcomponent" Width="230px" OnTextChanged="ddlcomponent_TextChanged" CssClass="form-control" AutoPostBack="true" runat="server"></asp:DropDownList>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtProduct" TextMode="MultiLine" Width="100px" runat="server"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtHSN" Width="100px" runat="server" ReadOnly="true"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtQty" Width="50px" runat="server" AutoPostBack="true" OnTextChanged="txtQty_TextChanged"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtUOM" Width="50px" runat="server"></asp:TextBox>
                                                                            </td>

                                                                        </tr>
                                                                    </tbody>
                                                                    <thead>
                                                                        <tr class="gvhead">
                                                                            <td>Rate</td>
                                                                            <td>Disc(%)</td>
                                                                            <td>Amount</td>
                                                                            <td>CGST</td>

                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="txtRate" Width="100px" runat="server" AutoPostBack="true" OnTextChanged="txtRate_TextChanged"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtDisc" Width="100px" runat="server" AutoPostBack="true" Text="0" OnTextChanged="txtDisc_TextChanged"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtAmountt" Width="100px" runat="server"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="CGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="CGSTPer_TextChanged" placeholder="%"></asp:TextBox>
                                                                                <asp:TextBox ID="CGSTAmt" Width="100px" runat="server" ReadOnly="true" placeholder="CGSTAmt"></asp:TextBox>
                                                                            </td>


                                                                        </tr>
                                                                    </tbody>
                                                                    <thead>
                                                                        <tr class="gvhead">
                                                                            <td>SGST</td>
                                                                            <td>IGST</td>
                                                                            <td>Total Amount</td>
                                                                            <td>Description</td>
                                                                            <td>Action</td>
                                                                        </tr>
                                                                    </thead>
                                                                    <tbody>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:TextBox ID="SGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="SGSTPer_TextChanged" placeholder="%"></asp:TextBox>
                                                                                <asp:TextBox ID="SGSTAmt" Width="100px" runat="server" ReadOnly="true" placeholder="SGSTAmt"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="IGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="IGSTPer_TextChanged" placeholder="%"></asp:TextBox>
                                                                                <asp:TextBox ID="IGSTAmt" Width="100px" runat="server" ReadOnly="true" placeholder="IGSTAmt"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtTotalamt" Width="100px" runat="server" ReadOnly="true"></asp:TextBox>
                                                                            </td>

                                                                            <td>
                                                                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                                            </td>
                                                                            <td>
                                                                                <asp:Button ID="btnAddMore" CssClass="btn btn-success btn-sm btncss" OnClick="Insert" runat="server" Text="+ Add" />
                                                                            </td>

                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                        <div class="row">
                                                            <div class="col-md-12">
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
                                                                                <asp:TemplateField HeaderText="Component" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <%-- <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("Particulars") %>' ID="txtParticulars" runat="server"></asp:TextBox>
                                                                                    </EditItemTemplate>--%>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblParticulars" runat="Server" Text='<%# Eval("Particulars") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Particulars" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("Product") %>' ID="txtProduct" runat="server"></asp:TextBox>
                                                                                    </EditItemTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblProduct" runat="Server" Text='<%# Eval("Product") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <%--   <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("HSN") %>' ID="txtHSN" runat="server"></asp:TextBox>
                                                                            </EditItemTemplate>--%>
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

                                                                                <asp:TemplateField HeaderText="UOM" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("UOM") %>' Width="50px" ID="txtUOM" runat="server"></asp:TextBox>
                                                                                    </EditItemTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblUOM" runat="Server" Text='<%# Eval("UOM") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("Rate") %>' ID="txtRate" runat="server" AutoPostBack="true" OnTextChanged="txtRate_TextChanged1"></asp:TextBox>
                                                                                    </EditItemTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblRate" runat="Server" Text='<%# Eval("Rate") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Discount" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("Discount") %>' ID="txtDiscount" runat="server" OnTextChanged="txtDiscount_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                                                    </EditItemTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblDiscount" runat="Server" Text='<%# Eval("Discount") %>' />
                                                                                    </ItemTemplate>
                                                                                </asp:TemplateField>
                                                                                <asp:TemplateField HeaderText="Amount" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Center">
                                                                                    <%-- <EditItemTemplate>
                                                                                <asp:TextBox Text='<%# Eval("Amount") %>' ID="txtAmount" runat="server"></asp:TextBox>
                                                                            </EditItemTemplate>--%>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblAmount" runat="Server" Text='<%# Eval("Amount") %>' />
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
                                                                                    <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("TotalAmount") %>' ID="txtTotalAmount" runat="server" ReadOnly="true"></asp:TextBox>
                                                                                    </EditItemTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblTotalAmount" runat="Server" Text='<%# Eval("TotalAmount") %>' />
                                                                                    </ItemTemplate>
                                                                                    <FooterTemplate>
                                                                                        <asp:Label ID="lbltotal" Font-Bold="true" runat="server" Text="Label"></asp:Label>
                                                                                    </FooterTemplate>
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" ItemStyle-HorizontalAlign="Left">
                                                                                    <EditItemTemplate>
                                                                                        <asp:TextBox Text='<%# Eval("Description") %>' ID="txttblDescription" runat="server"></asp:TextBox>
                                                                                    </EditItemTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:Label ID="lblDescription" runat="Server" Text='<%# Eval("Description") %>' />
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
                                                            </div>
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
                        <div class="card">

                            <div class="row">
                                <div class="col-xl-12 col-md-12">
                                    <div class="card-header">
                                        <div>
                                            <h5><b>Transportation Details</b></h5>
                                        </div>
                                    </div>
                                    <div class="card-body">
                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="table-responsive">
                                                    <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                                        <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                                            <td style="width: 15%;">Transportation Charges</td>
                                                            <td style="width: 15%;">Description</td>
                                                            <td>CGST(%)</td>
                                                            <td>SGST(%)</td>
                                                            <td>IGST(%)</td>
                                                            <td>Total Cost</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:TextBox ID="txtTCharge" Width="250px" runat="server" Text="0" OnTextChanged="txtTCharge_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtTDescription" Width="250px" runat="server" TextMode="MultiLine"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtTCGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="txtTCGSTPer_TextChanged"></asp:TextBox>
                                                                <asp:TextBox ID="txtTCGSTamt" Width="100px" runat="server" Text="0" ReadOnly="true"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtTSGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="txtTSGSTPer_TextChanged"></asp:TextBox>
                                                                <asp:TextBox ID="txtTSGSTamt" Width="100px" runat="server" Text="0" ReadOnly="true"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtTIGSTPer" Width="50px" runat="server" Text="0" AutoPostBack="true" OnTextChanged="txtTIGSTPer_TextChanged"></asp:TextBox>
                                                                <asp:TextBox ID="txtTIGSTamt" Width="100px" runat="server" Text="0" ReadOnly="true"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtTCost" Width="100px" runat="server" Enabled="false" Text="0"></asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12" style="text-align: center">
                            <div class="row" id="divcheckamend" runat="server">                               
                                <div class="col-md-2">
                                    <asp:CheckBox ID="chkAmended" runat="server" />
                                    <asp:Label ID="lblAmended" runat="server" Font-Bold="true" ForeColor="Green">Amended</asp:Label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-md-2">
                                    <asp:Button ID="btnadd" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Width="100%" Text="Add PO" OnClick="btnadd_Click" />
                                </div>
                                <div class="col-md-2">
                                    <asp:Button ID="btnreset" runat="server" CssClass="btn btn-danger" Width="100%" Text="Reset" OnClick="btnreset_Click" />
                                </div>
                                <div class="col-md-4"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnadd" />
        </Triggers>
    </asp:UpdatePanel>
    <script type="text/javascript">
        function DisableButton() {
            var btn = document.getElementById("<%=btnadd.ClientID %>");
            btn.value = 'Please wait...';
            document.getElementById("<%=btnadd.ClientID %>").disabled = true;
            document.getElementById("<%=btnadd.ClientID %>").classList.add("dissablebtn");
        }
        window.onbeforeunload = DisableButton;
    </script>
</asp:Content>
