﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="WarehouseInvoiceList.aspx.cs" Inherits="Account_WarehouseInvoiceList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script>
        window.addEventListener('DOMContentLoaded', function () {
            document.body.classList.add('sb-sidenav-toggled');
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
            font-size: 12px;
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div id="loader" class="loader-wrapper">
                <div class="loader"></div>
            </div>

            <div class="container-fluid">
                <div class="card-header text-uppercase text-black">
                    <div class="col-md-12">
                        <div class="row">
                            <div class="col-md-4">
                                <h4 class="mt-4">WareHouse Invoice List</h4>
                            </div>
                            <div class="col-md-4">
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="btnDownloadList" Style="margin-top: 24px; width: 110px" CssClass="form-control btn btn-success" Font-Bold="true" runat="server" Text="Components" OnClientClick="showLoader();" OnClick="btnDownloadList_Click" />
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="Button2" Style="margin-top: 24px; width: 130px" CssClass="form-control btn btn-success" Font-Bold="true" runat="server" Text="Add E-Invoice" OnClientClick="showLoader();" OnClick="Button2_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="container-fluid">
                <div class="row">
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblcompanyname" Font-Bold="true" runat="server" Text="Customer Name :"></asp:Label>
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
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblfromdate" runat="server" Font-Bold="true" Text="From Date :"></asp:Label>
                            <asp:TextBox ID="txtfromdate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lbltodate" runat="server" Font-Bold="true" Text="To Date :"></asp:Label>
                            <asp:TextBox ID="txttodate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" Style="margin-top: 32px" OnClick="btnSearchData_Click" runat="server" Text="Search" />
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" Style="margin-top: 32px" CssClass="btn btn-danger" runat="server" Text="Reset" />

                    </div>
                </div>
            </div>
            <br />
            <div class="container-fluid">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                    <div style="flex-grow: 1;">
                        <asp:Label ID="lblsno" runat="server"><span style="color:red;font-weight:bold">Note:</span>1) Light Sky Blue color rows show outward edit request Pending.
                        </asp:Label>&nbsp;&nbsp;
                        <asp:Label ID="Label1" runat="server">2) Light green color rows show outward edit request Approved.
                        </asp:Label>
                    </div>
                    <div class="col-md-1" style="text-align: right;">
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                            <asp:ListItem Text="10" Value="10" />
                            <asp:ListItem Text="50" Value="50" />
                            <asp:ListItem Text="All" Value="100000" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div style="overflow-x: auto; max-height: 600px; overflow-y: auto; border: 1px solid #ccc;">

                    <asp:GridView ID="GVInvoice" runat="server" CellPadding="4" DataKeyNames="Id" PageSize="500" AllowPaging="true" Width="100%" OnRowDataBound="GVInvoice_RowDataBound"
                        CssClass="grivdiv pagination-ys" OnRowCommand="GVInvoice_RowCommand" AutoGenerateColumns="false" OnPageIndexChanging="GVInvoice_PageIndexChanging">
                        <Columns>

                            <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblStatus" Visible="false" runat="server" Text='<%#Eval("Status")%>'></asp:Label>
                                    <asp:Label ID="Invoiceno" runat="server" Text='<%# Eval("InvoiceNo") != "" ? Eval("InvoiceNo") : Eval("FinalBasic") %>'></asp:Label>
                                    <asp:Label ID="lblFinalBasic" runat="server" Text='<%# Eval("FinalBasic") %>' Visible="false"></asp:Label>

                                    <asp:Label ID="lblIsApprove" runat="server" Text='<%# Eval("IsApprove") %>' Visible="false"></asp:Label>

                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Companyname" runat="server" Text='<%#Eval("BillingCustomer")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Invoice Date" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Invoicedate" runat="server" Text='<%# Eval("Invoicedate", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total Price" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Total_Price" runat="server" Text='<%#Eval("GrandTotalFinal")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- <asp:TemplateField HeaderText="PDF" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkRowView" runat="server" OnClientClick="showLoader();" Height="27px" CausesValidation="false" CommandName="RowView" ToolTip="View Tax Invoice PDF" CommandArgument='<%# Eval("Id") %>'><i class="fas fa-file-pdf"  style="font-size: 32px; color:red;  "></i></asp:LinkButton>
                                            &nbsp;
                                                <asp:LinkButton ID="lnkEInvoicePDF" runat="server" CausesValidation="false"  CommandName="DownloadPDFEInvoice" CommandArgument='<%# Eval("Id") %>' ToolTip="Download E-Invoice"><i class="fa fa-file-pdf" style="font-size:32px;color:green;"></i></asp:LinkButton>
                                            &nbsp;  
                                                <asp:LinkButton ID="lnkEWayPDF" runat="server" CausesValidation="false"  CommandName="DownloadEWay" CommandArgument='<%# Eval("Id") %>' ToolTip="Download E-Way Bill"><i class="fa fa-file-pdf" style="font-size:32px;color:orangered;"></i></asp:LinkButton>

                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="E-Invoice PDF" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEInvoicePDF" runat="server" CausesValidation="false" CommandName="DownloadPDFEInvoice" CommandArgument='<%# Eval("Id") %>' ToolTip="Download E-Invoice"><i class="fa fa-file-pdf" style="font-size:28px;color:green;"></i></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="E-Way Bill PDF" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkEWayPDF" runat="server" CausesValidation="false" CommandName="DownloadEWay" CommandArgument='<%# Eval("Id") %>' ToolTip="Download E-Way Bill"><i class="fa fa-file-pdf" style="font-size:28px;color:orangered;"></i></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action" HeaderStyle-CssClass="gvhead" HeaderStyle-Width="160px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkRowView" runat="server" OnClientClick="showLoader();" Height="27px" CausesValidation="false" CommandName="RowView" ToolTip="View Tax Invoice PDF" CommandArgument='<%# Eval("Id") %>'><i class="fas fa-file-pdf"  style="font-size: 28px; color:red;  "></i></asp:LinkButton>
                                    &nbsp;   
                                            <asp:LinkButton runat="server" ID="lnkEdit" OnClientClick="showLoader();" CommandName="RowEdit" CommandArgument='<%# Eval("InvoiceNo") %>' ToolTip="Add Component"><i class="fa fa-edit" style="font-size:28px;color:black;"></i></asp:LinkButton>
                                    <asp:LinkButton ID="lnkbtnapprove" runat="server" Height="27px" CausesValidation="false" CommandName="Approve" ToolTip="Click Here To Approve" CommandArgument='<%#Eval("Id")%>' Visible='<%# Eval("Status").ToString() == "2" ? true : false %>' OnClientClick="Javascript:return confirm('Are you sure to create e-invoice (please check ones componant details added or not.).....!')"><i class="fa fa-check-circle" style="font-size:28px;color:green"  ></i></asp:LinkButton>
                                    &nbsp;  
                                            <asp:LinkButton ID="lnkComponent" OnClientClick="showLoader();" runat="server" Height="27px" CausesValidation="false" CommandName="Showcomponent" ToolTip="Show Component" CommandArgument='<%# Eval("InvoiceNo") %>'><i class="fa fa-eye"  style="font-size: 28px; color:black;  "></i></asp:LinkButton>
                                    <%--<asp:LinkButton ID="lblEinvoicepdf" runat="server" Height="27px" CausesValidation="false" CommandName="Einvoicepdf" Visible='<%# Eval("Status").ToString() == "3" ? true : false %>' ToolTip="View E-Invoice PDF" CommandArgument='<%# Eval("Id") %>'><i class="fas fa-file-pdf"  style="font-size: 26px; color:green;  "></i></asp:LinkButton>--%>
                                                    &nbsp;
                                            <asp:LinkButton ID="lnkSendmail" runat="server" Height="27px" ToolTip="Send Mail" CausesValidation="false" CommandName="RowSendmail" OnClientClick="Javascript:return confirm('Are you sure to send mail..?')" CommandArgument='<%#Eval("Id")%>'><i class='fas fa-envelope' style='font-size:28px;color: red;'></i></asp:LinkButton>

                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>


            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GVInvoice" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:Button ID="btnhist" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupHistory" runat="server" TargetControlID="btnhist"
        PopupControlID="PopupHistoryDetail" OkControlID="Closepophistory" />

    <asp:Panel ID="PopupHistoryDetail" runat="server" CssClass="modelprofile1">
        <div class="row container">
            <div class="col-md-6"></div>
            <div class="col-md-6">
                <div class="profilemodel2">
                    <div class="headingcls">
                        <h4 class="modal-title">COMPONENT EDIT REQUEST 
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
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-md-6 col-12 mb-3">
                                <asp:Label ID="Label3" Font-Bold="true" runat="server" CssClass="form-label LblStyle"> Remark  :</asp:Label>
                                <asp:TextBox ID="txtremark" TextMode="MultiLine" CssClass="form-control" placeholder="Enter Remark" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Remark"
                                    ControlToValidate="txtremark" ValidationGroup="2" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-4" style="margin-top: 18px">

                                <asp:Button ID="btnsave" OnClick="btnsave_Click" ValidationGroup="2" ToolTip="Save & Send" CssClass="form-control btn btn-outline-success m-2" runat="server" Text="Save & Send" />


                            </div>
                            <div class="col-md-12">
                                <span style="color: red">Note: Click on save & send button Request send to Admin.</span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

    </asp:Panel>
</asp:Content>

