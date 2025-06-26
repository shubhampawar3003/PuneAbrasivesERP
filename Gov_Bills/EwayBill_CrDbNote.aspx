<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="EwayBill_CrDbNote.aspx.cs" Inherits="Gov_Bills_EwayBill_CrDbNote" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        .dissablebtn {
            cursor: not-allowed;
        }
    </style>
    <style>
        .spancls {
            color: #5d5656 !important;
            font-size: 13px !important;
            font-weight: 700;
            text-align: left;
            font-family: 'Arial', sans-serif;             
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

        .card {
            border-radius: 5px;
            -webkit-box-shadow: 0 1px 20px 0 rgba(69,90,100,0.08);
            box-shadow: 0 1px 20px 0 rgba(69,90,100,0.08);
            border: none;
            margin-bottom: 12px;
        }
    </style>
    <style type="text/css">
        .cal_Theme1 .ajax__calendar_container {
            background-color: #DEF1F4;
            border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_header {
            background-color: #ffffff;
            margin-bottom: 4px;
        }

        .cal_Theme1 .ajax__calendar_title,
        .cal_Theme1 .ajax__calendar_next,
        .cal_Theme1 .ajax__calendar_prev {
            color: #004080;
            padding-top: 3px;
        }

        .cal_Theme1 .ajax__calendar_body {
            background-color: #ffffff;
            border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_dayname {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            color: #004080;
        }

        .cal_Theme1 .ajax__calendar_day {
            color: #004080;
            text-align: center;
        }

        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_day,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_month,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_year,
        .cal_Theme1 .ajax__calendar_active {
            color: #004080;
            font-weight: bold;
            background-color: #DEF1F4;
        }

        .cal_Theme1 .ajax__calendar_today {
            font-weight: bold;
            font-size: 10px;
        }

        .cal_Theme1 .ajax__calendar_other,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_today,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_title {
            color: #bbbbbb;
        }

        .ajax__calendar_body {
            height: 158px !important;
            width: 220px !important;
            position: relative;
            overflow: hidden;
            margin: 0 0 0 -5px !important;
        }

        .ajax__calendar_container {
            padding: 4px;
            cursor: default;
            width: 220px !important;
            font-size: 11px;
            text-align: center;
            font-family: tahoma,verdana,helvetica;
        }

        .cal_Theme1 .ajax__calendar_day {
            color: #004080;
            font-size: 14px;
            text-align: center;
        }

        .ajax__calendar_day {
            height: 22px !important;
            width: 27px !important;
            text-align: right;
            padding: 0 14px !important;
            cursor: pointer;
        }

        .cal_Theme1 .ajax__calendar_dayname {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            margin-left: 12px !important;
            color: #004080;
        }

        .ajax__calendar_year {
            height: 50px !important;
            width: 51px !important;
            font-weight: bold;
            text-align: center;
            cursor: pointer;
            overflow: hidden;
            color: #004080;
        }

        .ajax__calendar_month {
            height: 50px !important;
            width: 51px !important;
            text-align: center;
            font-weight: bold;
            cursor: pointer;
            overflow: hidden;
            color: #004080;
        }

        .grid tr:hover {
            background-color: #d4f0fa;
        }

        .pcoded[theme-layout="vertical"][vertical-nav-type="expanded"] .pcoded-header .pcoded-left-header, .pcoded[theme-layout="vertical"][vertical-nav-type="expanded"] .pcoded-navbar {
            width: 210px;
        }

        <style type="text/css" >
        .divgrid {
            height: 200px;
            width: 370px;
        }

        .divgrid table {
            width: 350px;
        }

            .divgrid table th {
                background-color: Green;
                color: #fff;
            }
    </style>
    <style type="text/css">
        .cal_Theme1 .ajax__calendar_container {
            background-color: #DEF1F4;
            border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_header {
            background-color: #ffffff;
            margin-bottom: 4px;
        }

        .cal_Theme1 .ajax__calendar_title,
        .cal_Theme1 .ajax__calendar_next,
        .cal_Theme1 .ajax__calendar_prev {
            color: #004080;
            padding-top: 3px;
        }

        .cal_Theme1 .ajax__calendar_body {
            background-color: #ffffff;
            border: solid 1px #77D5F7;
        }

        .cal_Theme1 .ajax__calendar_dayname {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            color: #004080;
        }

        .cal_Theme1 .ajax__calendar_day {
            color: #004080;
            text-align: center;
        }

        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_day,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_month,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_year,
        .cal_Theme1 .ajax__calendar_active {
            color: #004080;
            font-weight: bold;
            background-color: #DEF1F4;
        }

        .cal_Theme1 .ajax__calendar_today {
            font-weight: bold;
            font-size: 10px;
        }

        .cal_Theme1 .ajax__calendar_other,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_today,
        .cal_Theme1 .ajax__calendar_hover .ajax__calendar_title {
            color: #bbbbbb;
        }

        .ajax__calendar_body {
            height: 158px !important;
            width: 220px !important;
            position: relative;
            overflow: hidden;
            margin: 0 0 0 -5px !important;
        }

        .ajax__calendar_container {
            padding: 4px;
            cursor: default;
            width: 220px !important;
            font-size: 11px;
            text-align: center;
            font-family: tahoma,verdana,helvetica;
        }

        .cal_Theme1 .ajax__calendar_day {
            color: #004080;
            font-size: 14px;
            text-align: center;
        }

        .ajax__calendar_day {
            height: 22px !important;
            width: 27px !important;
            text-align: right;
            padding: 0 14px !important;
            cursor: pointer;
        }

        .cal_Theme1 .ajax__calendar_dayname {
            text-align: center;
            font-weight: bold;
            margin-bottom: 4px;
            margin-top: 2px;
            margin-left: 12px !important;
            color: #004080;
        }

        .ajax__calendar_year {
            height: 50px !important;
            width: 51px !important;
            font-weight: bold;
            text-align: center;
            cursor: pointer;
            overflow: hidden;
            color: #004080;
        }

        .ajax__calendar_month {
            height: 50px !important;
            width: 51px !important;
            text-align: center;
            font-weight: bold;
            cursor: pointer;
            overflow: hidden;
            color: #004080;
        }

        .grid tr:hover {
            background-color: #d4f0fa;
        }

        .pcoded[theme-layout="vertical"][vertical-nav-type="expanded"] .pcoded-header .pcoded-left-header, .pcoded[theme-layout="vertical"][vertical-nav-type="expanded"] .pcoded-navbar {
            width: 210px;
        }

        .modal-lg {
            max-width: 80% !important;
        }

        .modal-header {
            padding: 0px 30px !important;
            border-bottom: none;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <div id="divGenerateEwayBill" runat="server" visible="false">
        <div class="page-wrapper">
            <div class="page-body">
                <div class="container mt-4">
                    <div class="card">
                        <div class="card-header text-uppercase text-black">
                            <h5><b>E-Invoice Details</b></h5>
                        <%--    <a href="#">
                                <h5 data-toggle="modal" data-target=".bd-example-modal-lg" style="float: right; color: #000 !important;"><i class="fa fa-file-pdf-o" style="color: red"></i>View E-Invoice</h5>
                            </a>--%>

                        </div>
                        <div class="row">
                            <div class="col-xl-12 col-md-12">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="row">
                                                <div class="col-md-2 spancls">Invoice No. <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblInvoiceNo" runat="server"></asp:Label>
                                                    <asp:Label ID="lblInvoiceDate" Visible="false" runat="server"></asp:Label>
                                                </div>
                                                <div class="col-md-2 spancls">IRN <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblIrn" runat="server"></asp:Label>
                                                    <asp:Label ID="lblHiddenid" Visible="false" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 spancls">Ack. No. <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblAckNo" runat="server"></asp:Label>
                                                </div>
                                                <div class="col-md-2 spancls">Ack. Date <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:Label ID="lblAckDate" runat="server"></asp:Label>
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
            <div class="page-body">
                <div class="container">
                    <div class="card">
                        <div class="card-header text-uppercase text-black">
                            <h5><b>E-Way Bill Details</b></h5>
                        </div>
                        <div class="row">
                            <div class="col-xl-12 col-md-12">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="row">
                                                <div class="col-md-2 spancls">Supply Type<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:DropDownList ID="ddlSupplyType" CssClass="form-control" runat="server">
                                                        <%--<asp:ListItem Value="0" Text="-- Select Type of Supply --"></asp:ListItem>--%>
                                                        <asp:ListItem Value="O" Text="Outward"></asp:ListItem>
                                                        <%--<asp:ListItem Value="I" Text="Inward"></asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-2 spancls">Sub Type<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:DropDownList ID="ddlSubType" CssClass="form-control" runat="server">
                                                        <asp:ListItem Value="0" Text="-- Select Sub Type --"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="Supply"></asp:ListItem>
                                                        <asp:ListItem Value="3" Text="Export"></asp:ListItem>
                                                        <%--     <asp:ListItem Value="4" Text="Job Work"></asp:ListItem>
                                                    <asp:ListItem Value="5" Text="For Own Use"></asp:ListItem>
                                                    <asp:ListItem Value="12" Text="Exhibition or Fairs"></asp:ListItem>--%>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 spancls">Transaction Type<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:DropDownList ID="ddlTransactionType" CssClass="form-control" runat="server">
                                                        <asp:ListItem Value="0" Text="-- Select Transaction Type --"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="Regular"></asp:ListItem>
                                                        <asp:ListItem Value="2" Text="Bill To - Ship To"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-2 spancls">Date<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtdate" runat="server" CssClass="form-control" placeholder="Date" Width="100%"></asp:TextBox>
                                                    <asp:CalendarExtender ID="CalendarExtender2" TargetControlID="txtdate" Format="dd-MM-yyyy" CssClass="cal_Theme1" runat="server"></asp:CalendarExtender>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 spancls">Mode of Transportation<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:DropDownList ID="ddlTransportationMode" OnTextChanged="ddlTransportationMode_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server">
                                                        <asp:ListItem Value="0" Text="-- Select Mode of Transportation --"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="Road"></asp:ListItem>
                                                        <asp:ListItem Value="2" Text="Rail"></asp:ListItem>
                                                        <asp:ListItem Value="3" Text="Air"></asp:ListItem>
                                                        <asp:ListItem Value="4" Text="Ship or Ship Cum Road/Rail"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-2 spancls">Vehicle Type<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:DropDownList ID="ddlVehicleType" CssClass="form-control" runat="server">
                                                        <asp:ListItem Value="0" Text="-- Select Vehicle Type --"></asp:ListItem>
                                                        <asp:ListItem Value="R" Text="Regular"></asp:ListItem>
                                                        <asp:ListItem Value="O" Text="ODC"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 spancls">Transporter ID<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtTransporterID" runat="server" CssClass="form-control" OnTextChanged="txtTransporterID_TextChanged" AutoPostBack="true" placeholder="Transporter ID" Width="100%"></asp:TextBox>
                                                </div>
                                                <div class="col-md-2 spancls">Transporter Name<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtTransporterName" runat="server" CssClass="form-control" placeholder="Transporter Name" Width="100%"></asp:TextBox>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 spancls">Vehicle Number<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtVehicleNumber" runat="server" CssClass="form-control" Width="100%"></asp:TextBox>
                                                </div>
                                                <%--<div class="col-md-2 spancls">Transporter &nbsp;&nbsp;Doc. No<i class="reqcls">*&nbsp;</i> : </div>--%>
                                                <div class="col-md-2 spancls">
                                                    <asp:Label ID="lblTransporterDocNo" runat="server" Text="Transporter Doc. No"></asp:Label><i class="reqcls">*&nbsp;</i> :
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtTransporterDocNo" runat="server" CssClass="form-control" Width="100%"></asp:TextBox>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2 spancls">Distance<i class="reqcls">*&nbsp;</i> : </div>
                                                <div class="col-md-4">
                                                    <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control" Width="100%" AutoComplete="off"></asp:TextBox>

                                                </div>
                                                <%--<div class="col-md-2 spancls"><i class="reqcls"></i></div>--%>
                                                <div class="col-md-6">
                                                    <span><a target="_blank" href="https://einvoice1.gst.gov.in/Others/GetPinCodeDistance" style="font-size: 14px; color: red; padding: 4px;">&nbsp;You can Calculate Distance Here</a>&nbsp;
                                                    </span>
                                                </div>
                                            </div>
                                            <br />

                                            <div class="row">
                                                <div class="col-md-2" style="margin-left: 18%;"></div>
                                                <div class="col-md-2">
                                                    <asp:Button ID="btnSubmit" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Width="100%" Text="Submit" OnClick="btnSubmit_Click" />
                                                </div>
                                                <div class="col-md-2">
                                                    <asp:Button ID="btnreset" runat="server" OnClick="btnreset_Click" CssClass="btn btn-danger" Width="100%" Text="Cancel" />
                                                </div>
                                                <div class="col-md-6"></div>
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
        </div>
        <div class="modal fade bd-example-modal-lg" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title"></h6>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>

                    </div>
                    <div class="container-fluid">
                        <div class="row" style="display: block; margin-top: 10px;">
                            <asp:Image ID="img_QrCode" runat="server" Visible="false" />
                            <asp:Image ID="imgBarcode" runat="server" Visible="false" />
                            <iframe id="ifrRight6" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 550px;"></iframe>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="divCancelEwayBill" runat="server" visible="false">
        <div class="page-wrapper">
            <div class="page-body">
                <div class="container mt-4">
                    <div class="card">
                        <div class="card-header bg-primary text-uppercase text-white">
                            <h5>Cancel E-Way Bill</h5>
                            <a href="#">
                                <h5 data-toggle="modal" data-target=".bd-example-modal-lg1" style="float: right; color: #000 !important;"><i class="fa fa-file-pdf-o" style="color: red"></i>View E-Way Bill</h5>
                            </a>
                        </div>
                        <div class="row">
                            <div class="col-xl-12 col-md-12">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col-md-3"></div>
                                        <div class="col-md-6 mt-2">
                                            <div class="row">
                                                <div class="col-md-3 spancls">E-Way Bill No. <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-9">
                                                    <asp:Label ID="lblEwaybillno" CssClass="font-weight-bold;" runat="server"></asp:Label>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-3 spancls">Reason <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-9">
                                                    <asp:DropDownList ID="ddlcnlReason" CssClass="form-control" runat="server">
                                                        <asp:ListItem Value="0" Text="-- Select Reason --"></asp:ListItem>
                                                        <asp:ListItem Value="1" Text="Duplicate"></asp:ListItem>
                                                        <asp:ListItem Value="2" Text="Order Cancelled"></asp:ListItem>
                                                        <asp:ListItem Value="3" Text="Data Entry Mistake"></asp:ListItem>
                                                        <asp:ListItem Value="4" Text="Others"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-3 spancls">Remarks <i class="reqcls">&nbsp;</i> : </div>
                                                <div class="col-md-9">
                                                    <asp:TextBox ID="txtcnlRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                                </div>
                                            </div>
                                            <br />
                                            <div class="row">
                                                <div class="col-md-2"></div>
                                                <div class="col-md-4">
                                                    <asp:Button ID="btncancel" runat="server" ValidationGroup="form1" CssClass="btn btn-primary"  Text="Cancel E-Way Bill" OnClick="btncancel_Click" />
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:Button ID="btncnlreset" runat="server" OnClick="btncnlreset_Click" CssClass="btn btn-danger" Width="100%" Text="Exit" />
                                                </div>
                                                <div class="col-md-2"></div>
                                            </div>
                                            <br />
                                        </div>
                                        <div class="col-md-3"></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade bd-example-modal-lg1" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <h6 class="modal-title"></h6>
                        <button type="button" class="close" data-dismiss="modal">&times;</button>

                    </div>
                    <div class="container-fluid">
                        <div class="row" style="display: block; margin-top: 10px;">
                            <asp:Image ID="QR_Img_Eway" runat="server" Visible="false" />
                            <asp:Image ID="Barcode_Img_Eway" runat="server" Visible="false" />
                            <iframe id="Iframe1" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 550px;"></iframe>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


