<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" EnableEventValidation="false" AutoEventWireup="true" CodeFile="OutstandingReport.aspx.cs" Inherits="Repoerts_OutstandingReport" %>

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
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "../img/minus.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "../img/plus.png");
            $(this).closest("tr").next().remove();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <%--<asp:UpdatePanel ID="updatepnl" runat="server">
        <ContentTemplate>--%>
    <div class="page-wrapper">
        <div class="page-body">
            <div class="container py-3">
                <div class="card">
                    <div class="card-header">
                        <div class="row">
                            <div class="col-md-4">
                                <h5><b>OUTSTANDING REPORT</b></h5>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-xl-12 col-md-12">
                            <div class="card">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lbltype" runat="server" Font-Bold="true" Text="Type :"></asp:Label>
                                            <%--<asp:TextBox ID="ddltype" runat="server" CssClass="form-control" Width="100%" Text="SALE" ReadOnly="true"></asp:TextBox>--%>
                                            <asp:DropDownList runat="server" CssClass="form-control" ID="ddltype" OnTextChanged="ddltype_TextChanged" AutoPostBack="true">
                                                <asp:ListItem Value="0">Select</asp:ListItem>
                                                <asp:ListItem>SALE</asp:ListItem>
                                                <asp:ListItem>PURCHASE</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Select Type"
                                                ControlToValidate="ddltype" ValidationGroup="form1" InitialValue="0" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblpartyname" runat="server" Font-Bold="true" Text="Party Name:"></asp:Label>
                                            <asp:TextBox ID="txtPartyName" runat="server" CssClass="form-control" placeholder="Party Name" Width="100%" OnTextChanged="txtPartyName_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                                TargetControlID="txtPartyName">
                                            </asp:AutoCompleteExtender>

                                            <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList"
                                                TargetControlID="txtPartyName">
                                            </asp:AutoCompleteExtender>
                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblFromdate" runat="server" Font-Bold="true" Text="From Date :"></asp:Label>
                                            <asp:TextBox ID="txtfromdate" runat="server" CssClass="form-control" TextMode="Date" placeholder="From Date" Width="100%" AutoComplete="off"></asp:TextBox>

                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblTo" runat="server" Font-Bold="true" Text="To Date :"></asp:Label>
                                            <asp:TextBox ID="txttodate" runat="server" CssClass="form-control" TextMode="Date" placeholder="To Date" Width="100%" AutoComplete="off"></asp:TextBox>

                                        </div>

                                    </div>
                                    <div class="row">
                                        <div class="col-xl-4 col-md-4" style="text-align: left">
                                            <br />
                                            <asp:Button ID="btnexcel" runat="server" ValidationGroup="form1" CssClass="btn btn-success" Text="Excel" OnClick="ExportExcel" />&nbsp
                                            <asp:Button ID="btnpdf" runat="server" ValidationGroup="form1" CssClass="btn btn-success" Text="PDF" OnClick="btnpdf_Click" />

                                        </div>
                                        <div class="col-xl-4 col-md-4" style="text-align: center">
                                            <br />
                                            <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" />
                                            <asp:Button ID="btnresetfilter" CssClass="btn btn-danger" runat="server" Text="Reset" OnClick="btnresetfilter_Click" />
                                        </div>
                                        <div class="col-xl-2 col-md-2"></div>
                                    </div>

                                </div>
                            </div>
                            <%--   <div class="col-md-12" style="padding: 20px; margin-top: 5px;">
                                <iframe id="ifrRight6" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 685px;"></iframe>
                            </div>--%>
                            <br />
                            <div class="col-md-12" style="padding: 0px; margin-top: 5px;">

                                <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">
                                    <asp:GridView ID="dgvOutstanding" runat="server" OnRowDataBound="dgvOutstanding_RowDataBound" ShowFooter="true" Width="100%"
                                        CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" HeaderStyle-Font-Bold="true" HeaderStyle-HorizontalAlign="Center"
                                        AllowPaging="false" ShowHeader="true" PageSize="10" DataKeyNames="BillingCustomer">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sr. No." HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblSrNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Type" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblType" Text='<%#Eval("Type")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Invoice No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblInvoiceNo" Text='<%#Eval("InvoiceNo")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblInvoicedate" Text='<%#Eval("Invoicedate")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Party Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblBillingCustomer" Text='<%#Eval("BillingCustomer")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" Text="Total : " />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Payable" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPayable" Text='<%#Eval("Payable")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblSumPayable" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Received" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReceived" Text='<%#Eval("Received")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblsumReceived" />
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Balance" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblbalance" Text='<%#Eval("Balance")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblsumBalance" />
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Cum.Balance" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCum_Balance" Text='<%#Eval("Balance")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label runat="server" ID="lblsumCum_Balance" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Days" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDays" Text='<%#Eval("Days")%>' runat="server"></asp:Label>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                </FooterTemplate>
                                            </asp:TemplateField>

                                        </Columns>
                                        <FooterStyle BackColor="#d48b48" Font-Bold="True" ForeColor="White" HorizontalAlign="Center" />
                                    </asp:GridView>
                                    <div id="DivFooterRow1" style="overflow: hidden">
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>


    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="100%" Height="600px" Visible="false"></rsweb:ReportViewer>
</asp:Content>

