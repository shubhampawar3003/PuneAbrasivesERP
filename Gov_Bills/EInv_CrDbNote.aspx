<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="EInv_CrDbNote.aspx.cs" Inherits="Gov_Bills_EInv_CrDbNote" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

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


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <div class="card">
        <div class="card-header">
            <div class="row">
                <div class="col-9 col-md-10">
                    <h4 class="mt-4 ">&nbsp <b>E-INVOICE LIST(Credit/Debit Note)</b></h4>
                </div>
                <div class="col-md-2 mt-4">
                    <asp:Button ID="btnewaybill" CssClass="form-control btn btn-warning" Font-Bold="true" Text="Add E-Way Bill" CausesValidation="false" runat="server" OnClick="btnewaybill_Click" />

                </div>
            </div>

        </div>
        <div class="card-body">

            <div class="row">
                <div class="col-xl-3 col-md-3">
                    <asp:TextBox ID="txtCustomerName" runat="server" CssClass="form-control" placeholder="Customer Name" Width="100%" AutoPostBack="true" OnTextChanged="txtCustomerName_TextChanged"></asp:TextBox>
                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                        TargetControlID="txtCustomerName">
                    </asp:AutoCompleteExtender>
                </div>
                <div class="col-xl-1 col-md-1">
                    <asp:Button ID="Button1" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" OnClick="btnresetfilter_Click" />
                </div>
            </div>
            <div class="row">
                <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">
                    <asp:GridView ID="GvInvoiceList" runat="server"
                        CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" Width="100%" 
                        AllowPaging="false" ShowHeader="true" OnRowDataBound="GvInvoiceList_RowDataBound" OnRowCommand="GvInvoiceList_RowCommand" DataKeyNames="Id">

                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No."  ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Doc. No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvoiceNo" runat="server" Text='<%# Eval("DocNo") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Note Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblNoteType" runat="server" Text='<%# Eval("NoteType") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Customer Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblcustomername" runat="server" Text='<%# Eval("SupplierName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Doc. Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblInvoicedate" runat="server" Text='<%# Eval("DocDate","{0:d}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Referance No" Visible="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblCustomerPO" runat="server" Text='<%# Eval("BillNumber") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblGrandTotal" runat="server" Text='<%# Eval("GrandTotal") %>'></asp:Label>
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
                                    <asp:LinkButton runat="server" ID="lnkCreateInv" CommandName="RowCreate" CommandArgument='<%# Eval("Id") %>' ToolTip="Create E-Invoice" OnClientClick="Javascript:return confirm('Do you want to Create E-Invoice?')"><i class="fa fa-file-text" style="font-size:24px;color:green;"></i></asp:LinkButton>
                                    &nbsp;                                                               
                           <asp:LinkButton ID="lnkPDF" Visible="false" runat="server" CommandName="DownloadPDF" CommandArgument='<%# Eval("Id") %>' ToolTip="Download"><i class="fa fa-file-pdf" style="font-size:24px;color:red;"></i></asp:LinkButton>
                                    &nbsp;                                                               
                           <asp:LinkButton ID="lnkCancel" Visible="false" runat="server" CommandName="RowCancel" CommandArgument='<%# Eval("Id") %>' ToolTip="Cancel E-Invoice" OnClientClick="Javascript:return confirm('Do you want to Cancel E-Invoice?')"><i class="fa fa-close" style="font-size:24px;color:red;"></i></asp:LinkButton>

                                    <%--<asp:LinkButton runat="server" ID="lnkDelete" CommandName="RowDelete" CommandArgument='<%# Eval("Id") %>' ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')"><i class="fa fa-trash" aria-hidden="true" style="font-size:24px;"></i></asp:LinkButton>--%>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>



    <asp:Image ID="img_QrCode" runat="server" Visible="false" />
    <asp:Image ID="imgBarcode" runat="server" Visible="false" />
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>

</asp:Content>

