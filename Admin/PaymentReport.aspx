<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PaymentReport.aspx.cs" Inherits="Admin_PaymentReport" MasterPageFile="~/Admin/WLSPLMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>

    <script type="text/javascript">
        $(function () {
            $("[id*=ddlinvoice]").select2();
            $("[id*=ddlcompnay]").select2();
            $("[id*=ddlownername]").select2();
        });
    </script>
    <script>
        function HideLabelerror(msg) {
            Swal.fire({
                icon: 'error',
                text: msg,

            })
        };
        function HideLabel(msg) {

            Swal.fire({
                icon: 'success',
                text: msg,
                timer: 5000,
                showCancelButton: false,
                showConfirmButton: false
            }).then(function () {
                window.location.href = "ProductMaster.aspx";
            })
        };
    </script>
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
    <asp:ScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-md-4">
                        <h5><b>PAYMENT HISTORY</b></h5>
                    </div>
                </div>
                <div class="card">
                    <div class="row">
                        <div class="col-xl-12 col-md-12">
                            <div class="card">
                                <div class="card-body">
                                    <div class="row">

                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblinvoce" runat="server" Font-Bold="true" Text="Invoice No."></asp:Label>
                                            <%--                                              <asp:TextBox ID="txtinvoice" runat="server" CssClass="form-control" placeholder="Invoice No" Width="100%" AutoPostBack="true"></asp:TextBox>--%>
                                            <asp:DropDownList ID="ddlinvoice" CssClass="form-control" AutoPostBack="true" runat="server">
                                            </asp:DropDownList>

                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lblFromdate" runat="server" Font-Bold="true" Text="From Date"></asp:Label>
                                            <asp:TextBox ID="txtfromdate" runat="server" CssClass="form-control" placeholder="From Date" Width="100%" AutoComplete="off" TextMode="Date"></asp:TextBox>

                                        </div>
                                        <div class="col-xl-3 col-md-3">
                                            <asp:Label ID="lbltodate" runat="server" Font-Bold="true" Text="To Date"></asp:Label>
                                            <asp:TextBox ID="txttodate" runat="server" CssClass="form-control" placeholder="To Date" Width="100%" AutoComplete="off" TextMode="Date"></asp:TextBox>
                                        </div>

                                        <div class="col-xl-3 col-md-3" style="margin-top: 25px">
                                            <asp:LinkButton ID="btnsearch" runat="server" ValidationGroup="form1" Width="30%" CssClass="btn btn-info" Text="Search" OnClick="btnsearch_Click"><i style="color:white" class="fa">&#xf002;</i> </asp:LinkButton>
                                            <asp:Button ID="btnpdf" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Text="Export PDF" Visible="false" />
                                            <asp:LinkButton ID="btnrefresh" OnClick="btnrefresh_Click" runat="server" Width="30%" CssClass="btn btn-warning"><i style="color:white" class="fa">&#xf021;</i> &nbsp;</asp:LinkButton>
                                        </div>
                                    </div>
                                    <br />
                                    <br />


                                    <div class="row">
                                        <div class="col-md-12" style="text-align: center; font-size: 20px">
                                            <b>Customer Name :</b>
                                            <asp:Label ID="lblcompanyname" runat="server" ForeColor="#993366" Font-Bold="true"></asp:Label>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="row">
                                        <div class="table">
                                            <asp:GridView ID="Gvreceipt" runat="server"
                                                CssClass="form-control grivdiv pagination-ys" AutoGenerateColumns="false" CellPadding="4" Width="100%" PageSize="10"
                                                AllowPaging="false" ShowHeader="true" ShowFooter="true" OnRowDataBound="Gvreceipt_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Sr.No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Invoice No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtinoiceno" runat="server" Text='<%# Eval("InvoiceNo") %>'></asp:Label>
                                                            <asp:Label runat="server" ID="lblmsgpaid" ForeColor="Red"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Bill No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtbillno" runat="server" Text='<%# Eval("BillNo") %>'></asp:Label>
                                                            <asp:Label runat="server" ID="lblmsgpaid1" ForeColor="Red"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Invoice Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtinvoicedate" runat="server" Text='<%# Eval("Invoicedate") %>'></asp:Label>
                                                            <asp:Label runat="server" ID="lblmsgpaid2" ForeColor="Red"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Last Transaction Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="TransactionnDate" runat="server" Text='<%# Eval("LastTransactionDate") %>'></asp:Label>
                                                            <asp:Label runat="server" ID="lblmsTrdate" ForeColor="Red"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Grand Total" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtGrandtotal" runat="server" Text='<%# Eval("Totalammount") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:Label ID="footerGrandtotal" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Pending Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtPending" runat="server" Text='<%# Eval("Pending") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:Label ID="footerpending" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Received Amount" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:Label ID="txtRecived" runat="server" Text='<%# Eval("ReceivedAmount") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            <asp:Label ID="footerrecived" runat="server"></asp:Label>
                                                        </FooterTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Paid Amount" ItemStyle-HorizontalAlign="Center" Visible="false" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtPaid" runat="server" Text='<%# Eval("Paid") %>' AutoPostBack="true"></asp:TextBox>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Remarks" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                        <ItemTemplate>
                                                            <%--<asp:TextBox ID="Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:TextBox>--%>
                                                            <asp:Label ID="Remarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
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
                </div>
            </div>
        </ContentTemplate>

    </asp:UpdatePanel>
</asp:Content>

