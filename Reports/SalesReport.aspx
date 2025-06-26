<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="SalesReport.aspx.cs" Inherits="Reports_SalesReport" %>

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

    <%-- <style>
        .checkbox-row {
            display: flex;
            gap: 15px; /* spacing between checkboxes */
            font-weight:bold;
        }

            .checkbox-row label {
                display: flex;
                align-items: center;
            }
    </style>--%>
    <style>
        .checkbox-row {
            display: flex;
            gap: 25px;
            padding: 15px 20px;
            background-color: #f9f9f9;
            border-radius: 8px;
            /*    border: 1px solid #ddd;*/
            font-weight: bold;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
            flex-wrap: wrap; /* wrap if screen is too narrow */
        }

            .checkbox-row label {
                display: flex;
                align-items: center;
                gap: 8px;
                cursor: pointer;
                transition: color 0.3s;
            }

            .checkbox-row input[type="checkbox"] {
                width: 18px;
                height: 18px;
                cursor: pointer;
                accent-color: #0078D4; /* for modern browsers */
            }

            .checkbox-row label:hover {
                color: #0078D4;
            }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">SALES REPORT</h4>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblcompanyname" Font-Bold="true" runat="server" Text="Company Name :"></asp:Label>
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
                    <div class="col-md-3" style="margin-top: 12px">

                        <br />
                        <asp:Button ID="btnDownload" OnClick="btnDownload_Click" CssClass="btn btn-success" runat="server" Text="Excel" Style="padding: 8px;" />
                        <asp:Button ID="btnPDF" OnClick="btnPDF_Click" Visible="false" CssClass="btn btn-success" runat="server" Text="PDF" Style="padding: 8px;" />

                    </div>
                </div>
                <br />

                <div class="row">
                    <div class="col-md-10">
                        <div class="checkbox-row">
                            <label>
                                <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <span>Tax Invoice</span>
                            </label>
                            <label>
                                <asp:CheckBox ID="CheckBox2" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <span>E-Invoice</span>
                            </label>
                            <label>
                                <asp:CheckBox ID="CheckBox3" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <span>Cancel E-Invoice</span>
                            </label>
                            <label>
                                <asp:CheckBox ID="CheckBox4" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <span>E-Way Bill</span>
                            </label>
                            <label>
                                <asp:CheckBox ID="CheckBox5" runat="server" AutoPostBack="true" OnCheckedChanged="CheckBox_CheckedChanged" />
                                <span>Cancel E-Way Bill</span>
                            </label>
                        </div>
                    </div>
                    <div class="col-md-2">
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />
                    </div>

                </div>



                <div class="row">



                    <div class="col-xl-4 col-md-4" style="text-align: center"></div>
                </div>
                <br />
            </div>

            <div style="overflow-x: auto; max-height: 600px; overflow-y: auto; border: 1px solid #ccc;">
                <asp:GridView ID="GVfollowup" runat="server" CellPadding="4" Font-Names="Verdana" ShowFooter="true"
                    Font-Size="12pt" Width="100%"
                    GridLines="Both" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false"
                    OnRowDataBound="GVfollowup_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Sr. No." HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblSrNo" runat="server" Text='<%# Container.DataItemIndex + 1 %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblCustomer" runat="server" Text='<%#Eval("companyname")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sales Person" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblUserName" runat="server" Text='<%#Eval("UserName")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice Date" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblInvoicedate" runat="server" Text='<%#Eval("invoicedate")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblInvoiceNo" runat="server" Text='<%#Eval("invoiceno")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="GST No." HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblGSTNo" runat="server" Text='<%#Eval("companygstno")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Rate" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblRate" runat="server" Text='<%#Eval("Rate")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label runat="server" Text="Total:" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblQuantity" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalQuantity" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Basic" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblBasic" runat="server" Text='<%#Eval("Total")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalBasic" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="CGST" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblCGSTAmt" runat="server" Text='<%#Eval("CGST")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalCGST" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="SGST" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblSGSTAmt" runat="server" Text='<%#Eval("SGST")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalSGST" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="IGST" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblIGSTAmt" runat="server" Text='<%#Eval("IGST")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblTotalIGST" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="TOTAL" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblTOTAL" runat="server" Text='<%#Eval("AllTotal")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblGrandTotal" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Other Basic" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblOTTotal" runat="server" Text='<%#Eval("OTTotal")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblOTTotalBasic" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Other CGST" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblOTCGST" runat="server" Text='<%#Eval("OTCGST")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblOTTotalCGST" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Other SGST" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblOTSGST" runat="server" Text='<%#Eval("OTSGST")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblOTTotalSGST" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Other IGST" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblOTIGST" runat="server" Text='<%#Eval("OTIGST")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblOTTotalIGST" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Other Total" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblOTALLTotal" runat="server" Text='<%#Eval("OTALLTotal")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblOTTotal" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Grand Total" HeaderStyle-CssClass="gvhead">
                            <ItemTemplate>
                                <asp:Label ID="lblTotalgrandTOTAL" runat="server" Text='<%#Eval("GrandTotal")%>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lblallGrandTotal" runat="server" />
                            </FooterTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle Font-Bold="True" ForeColor="Yellow" HorizontalAlign="Center" />

                </asp:GridView>
            </div>


            <br />

            <div class="container-fluid">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">&nbsp HSN SUMMARY REPORT</h4>
                    </div>
                </div>
                <div style="overflow-x: auto; max-height: 600px; overflow-y: auto; border: 1px solid #ccc;">
                    <asp:GridView ID="GvTotalSummary" runat="server" AutoGenerateColumns="False" CssClass="grivdiv pagination-ys"
                        CellPadding="2" Font-Names="Verdana" OnRowDataBound="GvTotalSummary_RowDataBound"
                        Font-Size="12pt" Width="100%" ShowFooter="true"
                        GridLines="Both">
                        <Columns>
                            <asp:TemplateField HeaderText="HSN Code" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("HSN") %>' ID="lblHSN" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalHSn" Text="Total : " runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("Quantity") %>' ID="lblQty" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalQty" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Units" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("Units") %>' ID="lblUOM" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="BasicTotal" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("Total") %>' ID="lblBasicTotal" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalBasicTotal" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="CGST" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("CGST") %>' ID="lblCGST" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalCGST" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="SGST" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("SGST") %>' ID="lblSGST" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalSGST" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="IGST" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("IGST") %>' ID="lblIGST" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalIGST" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Garnd Total" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label Text='<%# Eval("ALLTotal") %>' ID="lblGrandTptals" runat="server"></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="totalgrand" runat="server"></asp:Label>
                                </FooterTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <FooterStyle Font-Bold="True" ForeColor="Yellow" HorizontalAlign="Center" />
                    </asp:GridView>

                </div>


            </div>
            <br />
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownload" />
            <asp:PostBackTrigger ControlID="btnPDF" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
