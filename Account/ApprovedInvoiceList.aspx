<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="ApprovedInvoiceList.aspx.cs" Inherits="Account_ApprovedInvoiceList" %>

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
            background-color: #260b1a;
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
    <script>
        // Function to set the image in the modal
        function showImage(base64Image) {
            document.getElementById('imageViewer').src = base64Image;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container">
                <div class="row">
                    <div class="col-8 col-md-8">
                        <h4 class="mt-4 ">Approved Invoice List</h4>
                    </div>
                    <div class="col-2 col-md-2"></div>
                    <div class="col-2 col-md-2">
                        <asp:Button ID="Button2" CssClass="form-control btn btn-success" Visible="false" Font-Bold="true" runat="server" Text="Add E-Invoice" OnClick="Button2_Click" />
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-md-3">

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
                    <div class="col-md-3">

                        <asp:Label ID="Label1" Font-Bold="true" runat="server" Text="Invoice No. :"></asp:Label>
                        <asp:TextBox ID="txtInvoiceNo" CssClass="form-control" placeholder="Search Invoice No. " runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                        <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                            CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                            CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetInvoicenowiseList"
                            TargetControlID="txtInvoiceNo">
                        </asp:AutoCompleteExtender>


                    </div>
                    <div class="col-md-3">

                        <asp:Label ID="lblfromdate" runat="server" Font-Bold="true" Text="From Date :"></asp:Label>
                        <asp:TextBox ID="txtfromdate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>

                    </div>
                    <div class="col-md-3">

                        <asp:Label ID="lbltodate" runat="server" Font-Bold="true" Text="To Date :"></asp:Label>
                        <asp:TextBox ID="txttodate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>

                    </div>
                    <div class="col-md-3" style="margin-top: 20px">
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />

                    </div>
                </div>
            </div>
            <div class="container py-3">
                <div class="row">
                    <asp:Label ID="lblsno" Font-Bold="true" runat="server"><span style="color:red">Note:</span> Pink color rows show invoice payment term not match on that customer payment term.
                            <br />
                            <span style="color:red">Note:</span> LightBlue color rows show E-invoice Cancelled.
                    </asp:Label>
                    <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px;">
                        <div style="flex-grow: 1;">
                            <!-- Left empty for future content if needed -->
                        </div>
                        <div class="col-md-1" style="text-align: right;">
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                                <asp:ListItem Text="10" Value="10" />
                                <asp:ListItem Text="50" Value="50" />
                                <asp:ListItem Text="All" Value="100000" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">
                        <asp:GridView ID="GVInvoice" runat="server" CellPadding="4" DataKeyNames="Id" Width="100%" OnRowDataBound="GVInvoice_RowDataBound"
                            CssClass="grivdiv pagination-ys" OnRowCommand="GVInvoice_RowCommand" AutoGenerateColumns="false" OnPageIndexChanging="GVInvoice_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="Invoiceno" runat="server" Text='<%# Eval("InvoiceNo") != "" ? Eval("InvoiceNo") : Eval("FinalBasic") %>'></asp:Label>
                                        <asp:Label ID="lblFinalBasic" runat="server" Text='<%# Eval("FinalBasic") %>' Visible="false"></asp:Label>

                                        <asp:Label ID="lblIsApprove" runat="server" Text='<%# Eval("IsApprove") %>' Visible="false"></asp:Label>

                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Invoice Date" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="Invoicedate" runat="server" Text='<%# Eval("Invoicedate", "{0:dd-MM-yyyy}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="Companyname" runat="server" Text='<%#Eval("BillingCustomer")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Bill No" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="billno" runat="server" Text='<%#Eval("EwbNo")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status" Visible="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--           <asp:TemplateField HeaderText="Basic Amount" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="basicamount" runat="server" Text='<%#Eval("Basic")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Total Price" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="Total_Price" runat="server" Text='<%#Eval("GrandTotalFinal")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Payment Term" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblpaymentterm" runat="server" Text='<%# Eval("PaymentTerm") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%--   <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPending" runat="server" Font-Bold="true" ForeColor="OrangeRed" Visible='<%# Eval("Show").ToString() == "Pending" ? true : false %>' Text='<%# Eval("Show") %>'></asp:Label>
                                                    <asp:Label ID="lblApproval" runat="server" Font-Bold="true" ForeColor="Green" Visible='<%# Eval("Show").ToString() == "Approved" ? true : false %>' Text='<%# Eval("Show") %>'></asp:Label>


                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="lnkEdit" CommandName="RowEdit" CommandArgument='<%# Eval("Id") %>' ToolTip="Edit"><i class="fa fa-edit" style="font-size:24px;color:black;"></i></asp:LinkButton>
                                        <asp:LinkButton ID="lnkRowView" runat="server" Height="27px" CausesValidation="false" CommandName="RowView" ToolTip="View Tax Invoice PDF" CommandArgument='<%# Eval("Id") %>'><i class="fas fa-file-pdf"  style="font-size: 26px; color:red;  "></i></asp:LinkButton>
                                        <asp:LinkButton ID="lnkbtnapprove" runat="server" Height="27px" CausesValidation="false" CommandName="Approve" ToolTip="Click Here To Approve" CommandArgument='<%#Eval("Id")%>' Visible='<%# Eval("Status").ToString() == "1" ? true : false %>' OnClientClick="Javascript:return confirm('Are you sure to Approve this Invoice?')"><i class="fa fa-check-circle" style="font-size:25px;color:green"  ></i></asp:LinkButton>
                                        <%--  <asp:LinkButton ID="lnkEinvoice" runat="server" Height="27px" CausesValidation="false" CommandName="CreateEinvoice" Visible='<%# Eval("Show").ToString() == "Dispatched" ? true : false %>' ToolTip="Create E-Invoice" CommandArgument='<%# Eval("Id") %>'><i class="fa fa-plus-square"  style="font-size: 26px; color:green;  "></i></asp:LinkButton>--%>
                                        <asp:LinkButton ID="lblEinvoicepdf" runat="server" Height="27px" CausesValidation="false" CommandName="RowViewEInvoice" Visible='<%# Eval("e_invoice_status").ToString() == "True" ? true : false %>' ToolTip="View E-Invoice PDF" CommandArgument='<%# Eval("Id") %>'><i class="fas fa-file-pdf"  style="font-size: 26px; color:green;  "></i></asp:LinkButton>
                                        &nbsp;         
                                            
                                                                <asp:LinkButton ID="lnkCancel" runat="server" CommandName="RowCancel" Visible="false" CommandArgument='<%# Eval("Id") %>' ToolTip="Cancel E-Invoice Manually.....!" OnClientClick="Javascript:return confirm('Do you want to  Manually cancel E-Invoice?')"><i class="fa fa-close" style="font-size:24px;color:red;"></i></asp:LinkButton>

                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="GVInvoice" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="imageModal" tabindex="-1" role="dialog" aria-labelledby="imageModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="imageModalLabel">Image Viewer</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <!-- Image will be displayed here -->
                    <img id="imageViewer" src="" alt="Image" class="img-fluid" />
                </div>
            </div>
        </div>
    </div>


</asp:Content>

