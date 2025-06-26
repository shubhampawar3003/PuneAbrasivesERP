<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="AuditLogDashboard.aspx.cs" Inherits="Admin_AuditLogDashboard" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="page-wrapper">
                <div class="page-body">

                    <div class="row">
                        <div class="col-md-12" style="margin-top: 20px;">
                            <asp:Button runat="server" CssClass="btn btn-primary" ID="btnEnquiry" Text="Enquiry" OnClick="btnEnquiry_Click" />
                            <asp:Button runat="server" CssClass="btn btn-primary" ID="btnQuatation" Text="Quatations" OnClick="btnQuatation_Click" />
                            <asp:Button runat="server" CssClass="btn btn-primary" ID="btnOrderAccept" Text="PurchaseOrder" OnClick="btnOrderAccept_Click" />
                            <asp:Button runat="server" CssClass="btn btn-primary" ID="btnLoginList" Text="Login Log" OnClick="btnLoginList_Click" />
                            <asp:Button runat="server" CssClass="btn btn-warning" ID="btnRefresh" Text="Refresh" OnClick="btnRefresh_Click" />
                        </div>

                        <%--Login List--%>
                        <div class="col-sm-12" runat="server" id="DivLoginList" visible="false">
                            <!-- Basic Form Inputs card start -->
                            <div class="card">
                                <div class="card-header">
                                    <h4>Login List</h4>
                                </div>
                                <div class="card-block">
                                    <div class="form-group row">
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtDate" CssClass="form-control myDate" TextMode="Date" placeholder="From Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-md-1">
                                            <asp:Button ID="btnLoginSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnLoginSearch_Click" />
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="row" runat="server" id="Divnotfountimg" visible="false">
                                        <div class="col-md-2"></div>
                                        <div class="col-md-8">
                                            <center>
                                                <asp:Label runat="server" Text="No Record Found" style="font-size:26px;text-align:center;font-weight:600" cssClass="btn btn-info"></asp:Label>
                                                            </center>
                                        </div>
                                        <div class="col-md-2"></div>
                                    </div>

                                    <div class="form-group row">
                                        <div class="col-sm-12">
                                            <div id="divgv" runat="server">
                                                <asp:GridView ID="GvLoginlog" runat="server" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" Width="100%"
                                                    DataKeyNames="ID" AllowPaging="true" PageSize="10" OnPageIndexChanging="GvLoginlog_PageIndexChanging">
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="Sr. No." HeaderStyle-Width="80px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="User Code" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblempname" runat="server" Text='<%# Eval("CreatedBy") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Last Login Date & Time" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblccode" runat="server" Text='<%# Eval("CreatedOn") %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Last Login Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblccode" runat="server" Text='<%# Eval("UserName") %>'></asp:Label>
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

                        <%--Enquiry List--%>
                        <div class="col-sm-12" runat="server" id="DivEnquiry">
                            <!-- Basic Form Inputs card start -->
                            <div class="card">
                                <div class="card-header">
                                    <h4>Enquiry List</h4>
                                </div>
                                <div class="card-block">
                                    <div class="form-group row">
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtFromdate" TextMode="Date" CssClass="form-control myDate" placeholder="From Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <strong style="margin-top: 6px;"><i class="fa fa-arrow-right" aria-hidden="true"></i></strong><strong style="margin-top: 6px;"><i class="fa fa-arrow-left" aria-hidden="true"></i></strong>
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtTodate" TextMode="Date" CssClass="form-control myDate" placeholder="To Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-md-1">
                                            <asp:Button ID="btnEnquirySearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnEnquirySearch_Click" />
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="row" runat="server" id="Div1" visible="false">
                                        <div class="col-md-2"></div>
                                        <div class="col-md-8">
                                            <center>
                                                <asp:Label runat="server" Text="No Record Found" style="font-size:26px;text-align:center;font-weight:600" cssClass="btn btn-info"></asp:Label>
                                                            </center>
                                        </div>
                                        <div class="col-md-2"></div>
                                    </div>

                                    <div class="form-group row">
                                        <div class="col-sm-12">
                                            <asp:GridView ID="dgvEnquiryList" runat="server" CssClass="table table-striped table-bordered nowrap" AutoGenerateColumns="false" AllowPaging="false" PageSize="50">
                                                <PagerStyle CssClass="GridPager" />
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>Sr.No</HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblsno" Text="<%# Container.DataItemIndex +1 %>"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="EnqCode" ShowHeader="true" HeaderText="Enquiry Code" />
                                                    <asp:BoundField DataField="cname" ShowHeader="true" HeaderText="Company Name" />
                                                    <asp:BoundField DataField="Username" ShowHeader="true" HeaderText="Created By" />
                                                    <asp:BoundField DataField="regdate" ShowHeader="true" HeaderText="Created Date & Time" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%--Quatations List--%>
                        <div class="col-sm-12" runat="server" id="DivQuatation" visible="false">
                            <!-- Basic Form Inputs card start -->
                            <div class="card">
                                <div class="card-header">
                                    <h4>Quatation List</h4>
                                </div>
                                <div class="card-block">
                                    <div class="form-group row">
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtFromDate1" TextMode="Date" CssClass="form-control myDate" placeholder="From Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <strong style="margin-top: 6px;"><i class="fa fa-arrow-right" aria-hidden="true"></i></strong><strong style="margin-top: 6px;"><i class="fa fa-arrow-left" aria-hidden="true"></i></strong>
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtToDate1" TextMode="Date" CssClass="form-control myDate" placeholder="To Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-md-1">
                                            <asp:Button ID="btnQuatationSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnQuatationSearch_Click" />
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="row" runat="server" id="Div2" visible="false">
                                        <div class="col-md-2"></div>
                                        <div class="col-md-8">
                                            <center>
                                                <asp:Label runat="server" Text="No Record Found" style="font-size:26px;text-align:center;font-weight:600" cssClass="btn btn-info"></asp:Label>
                                                            </center>
                                        </div>
                                        <div class="col-md-2"></div>
                                    </div>

                                    <div class="form-group row">
                                        <div class="col-sm-12">
                                            <asp:GridView ID="dgvQuatationList" runat="server" AutoGenerateColumns="false" AllowPaging="false" CssClass="grivdiv pagination-ys" PageSize="50">
                                                <PagerStyle CssClass="GridPager" />
                                                <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>Sr.No</HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblsno" Text="<%# Container.DataItemIndex +1 %>"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="Quotationno" ShowHeader="true" HeaderText="Quotation No" />
                                                    <asp:BoundField DataField="Companyname" ShowHeader="true" HeaderText="Party Name" />
                                                    <%--<asp:BoundField DataField="material" ShowHeader="true" HeaderText="Material" />--%>
                                                    <asp:BoundField DataField="Username" ShowHeader="true" HeaderText="Created By" />
                                                    <asp:BoundField DataField="CreatedOn" ShowHeader="true" HeaderText="Date & Time" />
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%--Order Acceptance List--%>
                        <div class="col-sm-12" runat="server" id="DivOrderAccept" visible="false">
                            <!-- Basic Form Inputs card start -->
                            <div class="card">
                                <div class="card-header">
                                    <h4>Purchase Order List</h4>
                                </div>
                                <div class="card-block">
                                    <div class="form-group row">
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtfromDate2" TextMode="Date" CssClass="form-control myDate" placeholder="From Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <strong style="margin-top: 6px;"><i class="fa fa-arrow-right" aria-hidden="true"></i></strong><strong style="margin-top: 6px;"><i class="fa fa-arrow-left" aria-hidden="true"></i></strong>
                                        <div class="col-md-2">
                                            <asp:TextBox runat="server" ID="txtToDate2" TextMode="Date" CssClass="form-control myDate" placeholder="To Date" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-md-1">
                                            <asp:Button ID="btnOrderAcceptance" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnOrderAcceptance_Click" />
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="row" runat="server" id="Div3" visible="false">
                                        <div class="col-md-2"></div>
                                        <div class="col-md-8">
                                            <center>
                                                <asp:Label runat="server" Text="No Record Found" style="font-size:26px;text-align:center;font-weight:600" cssClass="btn btn-info"></asp:Label>
                                                            </center>
                                        </div>
                                        <div class="col-md-2"></div>
                                    </div>

                                    <div class="form-group row">
                                        <div class="col-sm-12">

                                            <asp:GridView ID="dgvOrderAcceptList" runat="server" AutoGenerateColumns="false" AllowPaging="false" CssClass="grivdiv pagination-ys" PageSize="50" Width="100%" HeaderStyle-CssClass="gvhead">
                                                <PagerStyle CssClass="GridPager" />
                                                <Columns>
                                                    <asp:TemplateField HeaderStyle-CssClass="gvhead">
                                                        <HeaderTemplate>S No</HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:Label runat="server" ID="lblsno" Text="<%# Container.DataItemIndex +1 %>"></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <%--     <asp:BoundField DataField="OAno" ShowHeader="true" HeaderText="OA Number" />--%>
                                                    <asp:BoundField DataField="CustomerName" HeaderStyle-CssClass="gvhead" ShowHeader="true" HeaderText="Customer Name" />
                                                    <asp:BoundField DataField="AgainstNumber" HeaderStyle-CssClass="gvhead" ShowHeader="true" HeaderText="Quotation No" />
                                                    <asp:BoundField DataField="Pono" HeaderStyle-CssClass="gvhead" ShowHeader="true" HeaderText="PO No" />
                                                    <asp:BoundField DataField="Username" HeaderStyle-CssClass="gvhead" ShowHeader="true" HeaderText="Created By" />
                                                    <asp:BoundField DataField="CreatedOn" HeaderStyle-CssClass="gvhead" ShowHeader="true" HeaderText="Created On" />
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

