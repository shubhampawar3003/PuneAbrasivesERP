<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="EInvoiceList.aspx.cs" Inherits="Admin_TaxInvoiceList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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

        .sortable-handler {
            touch-action: none;
        }

        .card .card-header span {
            color: #060606;
            display: block;
            font-size: 13px;
            margin-top: 5px;
        }

        .btn {
            padding: 5px 5px !important;
        }
    </style>
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
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
        .modelprofile1 {
            background-color: rgba(0, 0, 0, 0.54);
            display: block;
            position: fixed;
            z-index: 1;
            left: 0;
            /*top: 10px;*/
            height: 100%;
            overflow: auto;
            width: 100%;
            margin-bottom: 25px;
        }

        .profilemodel2 {
            background-color: #fefefe;
            margin-top: 25px;
            /*padding: 17px 5px 18px 22px;*/
            padding: 0px 0px 15px 0px;
            width: 100%;
            top: 40px;
            color: #000;
            border-radius: 5px;
        }

        .lblpopup {
            text-align: left;
        }

        .wp-block-separator:not(.is-style-wide):not(.is-style-dots)::before, hr:not(.is-style-wide):not(.is-style-dots)::before {
            content: '';
            display: block;
            height: 1px;
            width: 100%;
            background: #cccccc;
        }

        .btnclose {
            background-color: #ef1e24;
            float: right;
            font-size: 18px !important;
            /* font-weight: 600; */
            color: #f7f6f6 !important;
            border: 0px groove !important;
            background-color: none !important;
            /*margin-right: 10px !important;*/
            cursor: pointer;
            font-weight: 600;
            border-radius: 4px;
            padding: 4px;
        }

        /*hr {
       margin-top: 5px !important;
       margin-bottom: 15px !important;
       border: 1px solid #eae6e6 !important;
       width: 100%;
   }*/
        hr.new1 {
            border-top: 1px dashed green !important;
            border: 0;
            margin-top: 5px !important;
            margin-bottom: 5px !important;
            width: 100%;
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

        .headingcls {
            background-color: #01a9ac;
            color: #fff;
            padding: 15px;
            border-radius: 5px 5px 0px 0px;
        }

        @media (min-width: 1200px) {
            .container {
                max-width: 1250px !important;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <div class="container-fluid px-4">
        <div class="row">
            <div class="card">
                <div class="card-header">
                    <div class="row">
                        <div class="col-9 col-md-10">
                            <h4 class="mt-4 ">&nbsp <b>E-INVOICE LIST</b></h4>
                        </div>
                        <div class="col-md-2 mt-4">
                            <asp:Button ID="btnewaybill" CssClass="form-control btn btn-warning" Font-Bold="true" Text="Add E-Way Bill" CausesValidation="false" runat="server" OnClick="btnewaybill_Click" />

                        </div>
                    </div>

                </div>
                <div class="card-body">

                    <div class="row">
                        <div class="col-xl-3 col-md-3">
                            <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control" placeholder="Search Customer Name" Width="100%" AutoPostBack="true" OnTextChanged="txtCustomerName_TextChanged"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                TargetControlID="txtCustomerName">
                            </asp:AutoCompleteExtender>
                        </div>
                        <div class="col-xl-1 col-md-1">
                            <asp:Button ID="btnresetfilter" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" OnClick="btnresetfilter_Click" />
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12" style="padding: 0px;">
                            <asp:GridView ID="GvInvoiceList" runat="server"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" Width="100%"
                                AllowPaging="false" ShowHeader="true" PageSize="50" OnRowDataBound="GvInvoiceList_RowDataBound" OnRowCommand="GvInvoiceList_RowCommand" DataKeyNames="Id">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Invoice No" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>--%>
                                            <asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Eval("InvoiceNo") != null ? Eval("InvoiceNo") : Eval("FinalBasic") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblcustomername" runat="server" Text='<%# Eval("BillingCustomer") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoicedate" runat="server" Text='<%# Eval("Invoicedate","{0:d}") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Referance No" Visible="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustomerPO" runat="server" Text='<%# Eval("CustomerPONo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrandTotal" runat="server" Text='<%# Eval("GrandTotalFinal") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Prepared By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPrepared" runat="server" Text='<%# Eval("CreatedBy") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <%--<asp:LinkButton runat="server" ID="lnkEdit" CommandName="RowEdit" CommandArgument='<%# Eval("Id") %>' ToolTip="Edit"><i class="fa fa-edit" style="font-size:24px;color:black;"></i></asp:LinkButton>--%>
                                            <asp:LinkButton runat="server" ID="lnkCreateInv" CommandName="RowCreate" CommandArgument='<%# Eval("Id") %>' ToolTip="Create E-Invoice" OnClientClick="Javascript:return confirm('Do you want to Create E-Invoice?')"><i class="fa fa-plus-square" style="font-size:24px;color:green;"></i></asp:LinkButton>
                                            &nbsp;                                                               
                                                                <asp:LinkButton ID="lnkPDF" Visible="false" runat="server" CommandName="DownloadPDF" CommandArgument='<%# Eval("Id") %>' ToolTip="Download"><i class="fa fa-file-pdf" style="font-size:24px;color:red;"></i></asp:LinkButton>
                                            &nbsp;         
                                            
                                                                <asp:LinkButton ID="lnkCancel" Visible="false" runat="server" CommandName="RowCancel" CommandArgument='<%# Eval("Id") %>' ToolTip="Cancel E-Invoice"><i class="fa fa-close" style="font-size:24px;color:red;"></i></asp:LinkButton>

                                            <%--<asp:LinkButton runat="server" ID="lnkDelete" CommandName="RowDelete" CommandArgument='<%# Eval("Id") %>' ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')"><i class="fa fa-trash" aria-hidden="true" style="font-size:24px;"></i></asp:LinkButton>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>


                        </div>
                    </div>
                </div>

            </div>

        </div>

    </div>
    <asp:Image ID="img_QrCode" runat="server" Visible="false" />
    <asp:Image ID="imgBarcode" runat="server" Visible="false" />
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>


    <asp:Button ID="btnhist" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupHistory" runat="server" TargetControlID="btnhist"
        PopupControlID="PopupHistoryDetail" OkControlID="Closepophistory" />

    <asp:Panel ID="PopupHistoryDetail" runat="server" CssClass="modelprofile1">
        <div class="row container">
            <div class="col-md-6"></div>
            <div class="col-md-6">
                <div class="profilemodel2">
                    <div class="headingcls">
                        <h4 class="modal-title">CANCEL E-INVOICE 
                                 <button type="button" id="Closepophistory" class="btnclose" style="display: inline-block;" data-dismiss="modal">Close</button></h4>
                    </div>

                    <br />
                    <div class="body" style="margin-right: 10px; margin-left: 10px; padding-right: 1px; padding-left: 1px;">
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Label CssClass="form-label" runat="server" Font-Bold="true">Company Name :</asp:Label>
                                <asp:Label ID="lblcompanynamepop" CssClass="form-label" runat="server"></asp:Label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <asp:Label CssClass="form-label" runat="server" Font-Bold="true">Invoice No. :</asp:Label>
                                <asp:Label ID="lblinvoicename" CssClass="form-label" runat="server"></asp:Label>
                                <asp:HiddenField ID="hdnid" runat="server" />
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-6 col-12 mb-3">
                                <asp:Label ID="Label3" Font-Bold="true" runat="server" CssClass="form-label LblStyle"> Remark  :</asp:Label>
                                <asp:TextBox ID="txtremark" TextMode="MultiLine" CssClass="form-control" placeholder="Enter Remark" runat="server"></asp:TextBox>
                                <div class="text-danger mt-1">
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic"
                                        ErrorMessage="Please Enter Remark" ControlToValidate="txtremark"
                                        ValidationGroup="2" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="col-md-4" style="margin-top: 18px">
                                <asp:Button ID="btnsave" OnClick="btnsave_Click" ValidationGroup="2" ToolTip="Save" CssClass="form-control btn btn-outline-success m-2" runat="server" Text="Save" />

                            </div>
                            <div class="col-md-12">
                                <span style="color: red;font-weight:bold">Note: Please enter remark for cancellation e-invoice.</span><br />
                                  <span style="color: red;font-weight:bold">Note: Ones you click save button then cancel E-Invoice.......?.</span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>

