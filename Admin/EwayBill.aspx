<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="EwayBill.aspx.cs" Inherits="Admin_AddCompany" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
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

        .card {
            border-radius: 5px;
            -webkit-box-shadow: 0 1px 20px 0 rgba(69,90,100,0.08);
            box-shadow: 0 1px 20px 0 rgba(69,90,100,0.08);
            border: none;
            margin-bottom: 12px;
        }
    </style>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <div id="divGenerateEwayBill" runat="server" visible="false">
        <div class="container-fluid px-4">
            <div class="card">
                <div class="card-header">
                    <div class="row">
                        <div class="col-8 col-md-8">
                            <h5 class="mt-4 ">&nbsp <b>E-INVOICE DETAILS</b></h5>
                        </div>
                        <div class="col-md-2 mt-3">
                            <asp:Button ID="btneinvoice" CssClass="form-control btn btn-warning" Font-Bold="true" Text="E-Invoice PDF" CausesValidation="false" runat="server" OnClick="btneinvoice_Click" />

                        </div>
                        <div class="col-md-2 mt-3">
                            <asp:Button ID="btnEWaylist" CssClass="form-control btn btn-info" Font-Bold="true" Text="E-Way List" CausesValidation="false" runat="server" OnClick="btnEWaylist_Click" />

                        </div>
                    </div>

                </div>

                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Invoice No. </b><i class="reqcls">&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblInvoiceNo" runat="server"></asp:Label>
                                    <asp:Label ID="lblInvoiceDate" Visible="false" runat="server"></asp:Label>
                                </div>
                                <div class="col-md-2 spancls"><b>IRN</b> <i class="reqcls">&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblIrn" runat="server"></asp:Label>
                                    <asp:Label ID="lblHiddenid" Visible="false" runat="server"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Ack. No.</b> <i class="reqcls">&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblAckNo" runat="server"></asp:Label>
                                </div>
                                <div class="col-md-2 spancls"><b>Ack. Date</b> <i class="reqcls">&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:Label ID="lblAckDate" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <br />

                    </div>
                </div>
            </div>




            <div class="card">
                <div class="card-header">
                    <div class="row">
                        <div class="col-9 col-md-10">
                            <h5 class="mt-4 ">&nbsp <b>E-WAY BILL DETAILS</b></h5>
                        </div>
                    </div>
                </div>

                <div class="card-body">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Supply Type</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="ddlSupplyType" CssClass="form-control" runat="server">
                                        <%--<asp:ListItem Value="0" Text="-- Select Type of Supply --"></asp:ListItem>--%>
                                        <asp:ListItem Value="O" Text="Outward"></asp:ListItem>
                                        <%--<asp:ListItem Value="I" Text="Inward"></asp:ListItem>--%>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-2 spancls"><b>Sub Type</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="ddlSubType" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="0" Text="-- Select Sub Type --"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Supply"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Export"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Date</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtdate" runat="server" TextMode="Date" CssClass="form-control" placeholder="Date" Width="100%"></asp:TextBox>

                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Mode of Transportation</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="ddlTransportationMode" OnTextChanged="ddlTransportationMode_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="0" Text="-- Select Mode of Transportation --"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Road"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Rail"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Air"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="Ship or Ship Cum Road/Rail"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-2 spancls"><b>Vehicle Type</b><i class="reqcls">*&nbsp;</i> : </div>
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
                                <div class="col-md-2 spancls"><b>Transporter ID</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtTransporterID" runat="server" CssClass="form-control" OnTextChanged="txtTransporterID_TextChanged" AutoPostBack="true" placeholder="Transporter ID" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-2 spancls"><b>Transporter Name</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtTransporterName" runat="server" CssClass="form-control" placeholder="Transporter Name" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Vehicle Number</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtVehicleNumber" runat="server" CssClass="form-control" Width="100%"></asp:TextBox>
                                </div>
                                <%--<div class="col-md-2 spancls">Transporter &nbsp;&nbsp;Doc. No<i class="reqcls">*&nbsp;</i> : </div>--%>
                                <div class="col-md-2 spancls">
                                    <asp:Label ID="lblTransporterDocNo" runat="server" Font-Bold="true" Text="Transporter Doc. No"></asp:Label><i class="reqcls">*&nbsp;</i> :
                                </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtTransporterDocNo" runat="server" CssClass="form-control" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2 spancls"><b>Distance</b><i class="reqcls">*&nbsp;</i> : </div>
                                <div class="col-md-4">
                                    <asp:TextBox ID="txtDistance" runat="server" CssClass="form-control" Width="100%" AutoComplete="off"></asp:TextBox>
                                    <asp:Label ID="lblDistance" runat="server" Text="Kindly Enter The Distance Manually..!!" ForeColor="Red" Visible="false"></asp:Label>
                                </div>

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
    </div>

    <div id="divCancelEwayBill" runat="server" visible="false">

        <div class="container-fluid px-4">
            <div class="card">
                <div class="card-header">
                    <div class="row">
                        <div class="col-9 col-md-10">
                            <h4 class="mt-4 ">&nbsp <b>CANCEL E_WAY BILL</b></h4>
                        </div>
                        <div class="col-md-2 mt-4">
                            <asp:Button ID="btnewaybill" CssClass="form-control btn btn-warning" Font-Bold="true" Text="E-Way Bill List" CausesValidation="false" runat="server" OnClick="btnewaybill_Click" />
                        </div>
                    </div>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="col-md-3"></div>
                        <div class="col-md-6 mt-2">
                            <div class="row">
                                <div class="col-md-3 spancls"><b>E-Way Bill No.</b> <i class="reqcls">&nbsp;</i> : </div>
                                <div class="col-md-9">
                                    <asp:Label ID="lblEwaybillno" CssClass="font-weight-bold;" runat="server"></asp:Label>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-3 spancls"><b>Reason</b> <i class="reqcls">&nbsp;</i> : </div>
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
                                <div class="col-md-3 spancls"><b>Remarks</b> <i class="reqcls">&nbsp;</i> : </div>
                                <div class="col-md-9">
                                    <asp:TextBox ID="txtcnlRemark" runat="server" CssClass="form-control" TextMode="MultiLine" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2"></div>
                                <div class="col-md-4">
                                    <asp:Button ID="btncancel" runat="server" ValidationGroup="form1" CssClass="btn btn-primary" Text="Cancel E-Way Bill" OnClick="btncancel_Click" />
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
        <rsweb:reportviewer id="ReportViewer1" runat="server" visible="false"></rsweb:reportviewer>
        <div class="container-fluid px-4">
            <div class="card">
                <div class="card-header">
                    <div class="row">
                        <asp:Image ID="QR_Img_Eway" runat="server" Visible="false" />
                        <asp:Image ID="Barcode_Img_Eway" runat="server" Visible="false" />
                        <iframe id="Iframe1" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 550px;"></iframe>

                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>


