<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="Service_Reports.aspx.cs" Inherits="Admin_Service_Reports" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <style>
        span.select2.select2-container.select2-container--default.select2-container--focus {
            max-width: 100% !important;
        }

        .select2-container {
            box-sizing: border-box;
            display: inline-block;
            margin: 0;
            position: relative;
            vertical-align: middle;
            width: 100% !important;
        }
    </style>

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
 

    <script type="text/javascript">
        $(function () {
            $("[id*=ddlProduct]").select2();
            $("[id*=ddlcompanyname]").select2();

        });
    </script>

    <style>
        .LblStyle {
            font-weight: 500;
            color: black;
        }

        .card_adj {
            margin-bottom: 3px;
            height: 35px;
        }
    </style>
    <!---Number--->
    <script>
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        }
    </script>
    <style>
        .gvhead {
            text-align: center;
            color: #ffffff;
            background-color: #31B0C4;
        }

        .head {
            text-align: center;
            color: #000000;
            background-color: #FF7F50;
        }

        .lnk {
            font-weight: bolder;
            font-size: large;
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

        .spncls {
            color: red;
        }


        .form-control, .dataTable-input {
            display: initial !important;
        }
    </style>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-3">
                <div class="container-fluid px-3" style="padding: 23px;">
                    <%--<h2 class="mt-4 ">Quotation Master</h2>--%>
                    <div class="card mb-4">
                        <div class="card-header LblStyle">
                            <i class="fas fa-user me-1"></i>
                            SERVICE CALL REPORT                          
                        </div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblcompanyname" class="form-label LblStyle">LOCATION : </label>
                                    <asp:DropDownList ID="ddllocation" CssClass="form-control" runat="server">
                                        <asp:ListItem>Select</asp:ListItem>
                                        <asp:ListItem>Kolkata</asp:ListItem>
                                        <asp:ListItem>Guwahati</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblTickit" class="form-label LblStyle"><span class="spncls">*</span>Ticket No. : </label>
                                    <asp:TextBox ID="txtticket" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblCustomerName" class="form-label LblStyle"><span class="spncls">*</span>Customer Name :</label>
                                    <asp:TextBox ID="TextBox1" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblMobile" class="form-label LblStyle"><span class="spncls">*</span>Mobile No. : </label>
                                    <asp:TextBox ID="TextBox2" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblCompanyAddress" class="form-label LblStyle">Company Address : </label>
                                    <asp:TextBox ID="TextBox3" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblEmail" class="form-label LblStyle"><span class="spncls">*</span>Email ID:</label>
                                    <asp:TextBox ID="TextBox4" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>
                                <div class="col-md-4 col-12 mb-3">
                                    <label for="lblCperson" class="form-label LblStyle">Contact Person :</label>
                                    <asp:TextBox ID="TextBox5" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-12">

                                    <div class="card-header">
                                        <div class="table-responsive">
                                            <table class="table">
                                                <tr>
                                                    <td><b>Call Category :</b></td>
                                                    <td><b>Product Model :</b></td>
                                                    <td><b>Serial Number :</b></td>
                                                    <td><b>Product Status :</b></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtinstallation" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtmachinemodel" CssClass="form-control" runat="server" Placeholder="Product Model" ReadOnly="true"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox8" runat="server" CssClass="form-control" Placeholder="Serial Number"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DropDownList2" runat="server" CssClass="form-control" >
                                                            <asp:ListItem>None</asp:ListItem>
                                                            <asp:ListItem>Warranty</asp:ListItem>
                                        <%--                    <asp:ListItem>AMC</asp:ListItem>--%>

                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                      
                                    <div class="card-header" id="installation" runat="server" visible="false">
                                        <div class="table-responsive">
                                            <table class="table">
                                                <tr>
                                                    <td><b>Date Of Installation :</b></td>
                                                    <td ><b>Warranty duration :</b></td>
                                                    <td><b>Warranty Period :</b></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="TextBox6" runat="server" TextMode="Date" CssClass="form-control" Placeholder="Date"></asp:TextBox>


                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="DropDownList7" runat="server" Width="180px" CssClass="form-control"  AutoPostBack="true" OnSelectedIndexChanged="DropDownList7_SelectedIndexChanged">
                                                            <asp:ListItem> None </asp:ListItem>
                                                            <asp:ListItem>6 months</asp:ListItem>
                                                            <asp:ListItem>12 months</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="TextBox9" runat="server" TextMode="Date"  CssClass="form-control" Width="180px" Placeholder="From"></asp:TextBox>

                                                        -
                                             <asp:TextBox ID="TextBox7" runat="server" Width="180px" TextMode="Date"  CssClass="form-control" Placeholder="To"></asp:TextBox>


                                                    </td>

                                                </tr>
                                            </table>
                                        </div>

                                        <div class="table-responsive">
                                            <table class="table">
                                                <tr class="gvhead">
                                                    <td id="pshead1" runat="server" visible="false">Preventive schedule 1</td>
                                                    <td id="pshead2" runat="server" visible="false">Preventive schedule 2</td>
                                                    <td id="pshead3" runat="server" visible="false">Preventive schedule 3</td>
                                                    <td id="pshead4" runat="server" visible="false">Preventive schedule 4</td>
                                                </tr>
                                                <tr>
                                                    <td id="ps1" runat="server" visible="false">
                                                        <asp:DropDownList ID="ddlmonth1" runat="server" Width="60%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>January</asp:ListItem>
                                                            <asp:ListItem>February</asp:ListItem>
                                                            <asp:ListItem>March</asp:ListItem>
                                                            <asp:ListItem>April</asp:ListItem>
                                                            <asp:ListItem>May</asp:ListItem>
                                                            <asp:ListItem>June</asp:ListItem>
                                                            <asp:ListItem>July</asp:ListItem>
                                                            <asp:ListItem>August</asp:ListItem>
                                                            <asp:ListItem>September</asp:ListItem>
                                                            <asp:ListItem>October</asp:ListItem>
                                                            <asp:ListItem>November</asp:ListItem>
                                                            <asp:ListItem>December</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddldate1" runat="server" Width="30%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>2017</asp:ListItem>
                                                            <asp:ListItem>2018</asp:ListItem>
                                                            <asp:ListItem>2019</asp:ListItem>
                                                            <asp:ListItem>2020</asp:ListItem>
                                                            <asp:ListItem>2021</asp:ListItem>
                                                            <asp:ListItem>2022</asp:ListItem>
                                                            <asp:ListItem>2023</asp:ListItem>
                                                            <asp:ListItem>2024</asp:ListItem>
                                                            <asp:ListItem>2025</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td id="ps2" runat="server" visible="false">
                                                        <asp:DropDownList ID="ddlmonth2" runat="server" Width="60%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>January</asp:ListItem>
                                                            <asp:ListItem>February</asp:ListItem>
                                                            <asp:ListItem>March</asp:ListItem>
                                                            <asp:ListItem>April</asp:ListItem>
                                                            <asp:ListItem>May</asp:ListItem>
                                                            <asp:ListItem>June</asp:ListItem>
                                                            <asp:ListItem>July</asp:ListItem>
                                                            <asp:ListItem>August</asp:ListItem>
                                                            <asp:ListItem>September</asp:ListItem>
                                                            <asp:ListItem>October</asp:ListItem>
                                                            <asp:ListItem>November</asp:ListItem>
                                                            <asp:ListItem>December</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddldate2" runat="server" Width="30%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>2017</asp:ListItem>
                                                            <asp:ListItem>2018</asp:ListItem>
                                                            <asp:ListItem>2019</asp:ListItem>
                                                            <asp:ListItem>2020</asp:ListItem>
                                                            <asp:ListItem>2021</asp:ListItem>
                                                            <asp:ListItem>2022</asp:ListItem>
                                                            <asp:ListItem>2023</asp:ListItem>
                                                            <asp:ListItem>2024</asp:ListItem>
                                                            <asp:ListItem>2025</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td id="ps3" runat="server" visible="false">
                                                        <asp:DropDownList ID="ddlmonth3" runat="server" Width="60%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>January</asp:ListItem>
                                                            <asp:ListItem>February</asp:ListItem>
                                                            <asp:ListItem>March</asp:ListItem>
                                                            <asp:ListItem>April</asp:ListItem>
                                                            <asp:ListItem>May</asp:ListItem>
                                                            <asp:ListItem>June</asp:ListItem>
                                                            <asp:ListItem>July</asp:ListItem>
                                                            <asp:ListItem>August</asp:ListItem>
                                                            <asp:ListItem>September</asp:ListItem>
                                                            <asp:ListItem>October</asp:ListItem>
                                                            <asp:ListItem>November</asp:ListItem>
                                                            <asp:ListItem>December</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddldate3" runat="server" Width="30%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>2017</asp:ListItem>
                                                            <asp:ListItem>2018</asp:ListItem>
                                                            <asp:ListItem>2019</asp:ListItem>
                                                            <asp:ListItem>2020</asp:ListItem>
                                                            <asp:ListItem>2021</asp:ListItem>
                                                            <asp:ListItem>2022</asp:ListItem>
                                                            <asp:ListItem>2023</asp:ListItem>
                                                            <asp:ListItem>2024</asp:ListItem>
                                                            <asp:ListItem>2025</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td id="ps4" runat="server" visible="false">
                                                        <asp:DropDownList ID="ddlmonth4" runat="server" Width="60%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>January</asp:ListItem>
                                                            <asp:ListItem>February</asp:ListItem>
                                                            <asp:ListItem>March</asp:ListItem>
                                                            <asp:ListItem>April</asp:ListItem>
                                                            <asp:ListItem>May</asp:ListItem>
                                                            <asp:ListItem>June</asp:ListItem>
                                                            <asp:ListItem>July</asp:ListItem>
                                                            <asp:ListItem>August</asp:ListItem>
                                                            <asp:ListItem>September</asp:ListItem>
                                                            <asp:ListItem>October</asp:ListItem>
                                                            <asp:ListItem>November</asp:ListItem>
                                                            <asp:ListItem>December</asp:ListItem>
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="ddldate4" runat="server" Width="30%" Height="30px">
                                                            <asp:ListItem></asp:ListItem>
                                                            <asp:ListItem>2017</asp:ListItem>
                                                            <asp:ListItem>2018</asp:ListItem>
                                                            <asp:ListItem>2019</asp:ListItem>
                                                            <asp:ListItem>2020</asp:ListItem>
                                                            <asp:ListItem>2021</asp:ListItem>
                                                            <asp:ListItem>2022</asp:ListItem>
                                                            <asp:ListItem>2023</asp:ListItem>
                                                            <asp:ListItem>2024</asp:ListItem>
                                                            <asp:ListItem>2025</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>

                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-6 col-12 mb-3">
                                            <b>Nature Of Complaint : </b>
                                            <asp:TextBox ID="TextBox10" runat="server" CssClass="form-control" TextMode="MultiLine" Width="100%" Height="100px" placeholder="Nature Of Complaint"></asp:TextBox><br />
                                            <b>Complaint Description : </b>
                                            <asp:TextBox ID="TextBox17" runat="server" CssClass="form-control" TextMode="MultiLine" Width="100%" Height="100px" placeholder="Complaint Description"></asp:TextBox>
                                        </div>
                                        <div class="col-md-6 col-12 mb-3">
                                            <div class="table-responsive">
                                                <table class="table">
                                                    <tr>
                                                        <td></td>

                                                        <td>
                                                            <center><b>Date - Time </b></center>
                                                        </td>

                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <b><span class="spncls">*</span>Call registered :</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox11" runat="server" TextMode="DateTimeLocal" CssClass="form-control" ReadOnly="true"></asp:TextBox>&nbsp;
                       
                                                        </td>

                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <b><span class="spncls">*</span>Call Attended :</b>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox12" TextMode="DateTimeLocal" runat="server" CssClass="form-control"></asp:TextBox>
                                                          
                                                            <asp:Label ID="lblmsg" runat="server" ForeColor="Red" Visible="false" Text="**Required"></asp:Label>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td>
                                                            <b><span class="spncls">*</span>Call Completed :</b>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="TextBox13" TextMode="DateTimeLocal" runat="server" CssClass="form-control"></asp:TextBox>
                                                          
                                                            <asp:Label ID="lblmsg1" runat="server" ForeColor="Red" Visible="false" Text="**Required"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </div>

                                    </div>

                                </div>
                                <br />
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    <div class="table-responsive">
                                        <table class="table text-center align-middle table-bordered table-hover" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                            <thead>
                                                <tr class="gvhead">
                                                    <td>Product</td>

                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:DropDownList ID="ddlProduct" Width="230px" OnTextChanged="ddlProduct_TextChanged" CssClass="form-control" AutoPostBack="true" runat="server"></asp:DropDownList>
                                                    </td>


                                                </tr>
                                            </tbody>
                                            <thead>
                                                <tr class="gvhead">
                                                    <td>Description</td>
                                                    <td>HSN / SAC</td>
                                                    <td>Quantity</td>
                                                    <td>Unit</td>
                                                    <td>Service Charge</td>

                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtdescription" Width="230px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txthsnsac" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtquantity" Width="190px" CssClass="form-control" OnTextChanged="txtquantity_TextChanged" AutoPostBack="true" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtunit" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtrate" Width="190px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </tbody>
                                            <thead>
                                                <tr class="gvhead">
                                                    <td>Total </td>
                                                    <td colspan="2">CGST</td>
                                                    <td colspan="2">SGST</td>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txttotal" Width="100" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCGST" placeholder="%" Width="100px" OnTextChanged="txtCGST_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCGSTamt" Width="100px" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSGST" placeholder="%" Width="100px" OnTextChanged="txtSGST_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtSGSTamt" Width="100px" ReadOnly="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>

                                                </tr>
                                            </tbody>

                                            <thead>
                                                <tr class="gvhead">
                                                    <td colspan="2">IGST</td>
                                                    <td>Discount(%)</td>
                                                    <td>Grand Total</td>
                                                    <td>Action</td>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>

                                                    <td>
                                                        <asp:TextBox ID="txtIGST" OnTextChanged="txtIGST_TextChanged" AutoPostBack="true" placeholder="%" Width="100px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtIGSTamt" Width="100px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtdiscount" OnTextChanged="txtdiscount_TextChanged" Width="80px" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                        <asp:TextBox ID="txtdiscountamt" Visible="false" Width="80px" AutoPostBack="true" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtgrandtotal" Width="150px" CssClass="form-control" runat="server"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        <asp:Button ID="btnAddMore" OnClick="btnAddMore_Click" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add More" />
                                                    </td>

                                                </tr>
                                            </tbody>

                                        </table>
                                    </div>

                                    <%--<div class="row" id="divdtls">--%>
                                    <div class="table-responsive text-center">
                                        <asp:GridView ID="dgvMachineDetails" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                            OnRowEditing="dgvMachineDetails_RowEditing" OnRowDataBound="dgvMachineDetails_RowDataBound" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                        <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Product" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Productname") %>' CssClass="form-control" Width="230px" ID="txtproduct" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblproduct" runat="Server" Text='<%# Eval("Productname") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Description") %>' CssClass="form-control" ID="txtDescription" Width="200px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDescription" runat="Server" Text='<%# Eval("Description") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("HSN") %>' CssClass="form-control" ID="txthsn" Width="200px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblhsn" runat="Server" Text='<%# Eval("HSN") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Quantity") %>' CssClass="form-control" ID="txtQuantity" OnTextChanged="txtQuantity_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblQuantity" runat="Server" Text='<%# Eval("Quantity") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Unit" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Units") %>' CssClass="form-control" ID="txtUnit" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblUnit" runat="Server" Text='<%# Eval("Units") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Rate") %>' CssClass="form-control" OnTextChanged="txtRate_TextChanged" AutoPostBack="true" ID="txtRate" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRate" runat="Server" Text='<%# Eval("Rate") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Total") %>' CssClass="form-control" ID="txtTotal" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal" runat="Server" Text='<%# Eval("Total") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("CGSTPer") %>' CssClass="form-control" OnTextChanged="txtCGSTPer_TextChanged" AutoPostBack="true" ID="txtCGSTPer" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCGSTPer" runat="Server" Text='<%# Eval("CGSTPer") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("CGSTAmt") %>' CssClass="form-control" ID="txtCGST" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCGST" runat="Server" Text='<%# Eval("CGSTAmt") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("SGSTPer") %>' CssClass="form-control" ID="txtSGSTPer" OnTextChanged="txtSGSTPer_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSGSTPer" runat="Server" Text='<%# Eval("SGSTPer") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("SGSTAmt") %>' CssClass="form-control" ID="txtSGST" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSGST" runat="Server" Text='<%# Eval("SGSTAmt") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IGST %" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("IGSTPer") %>' CssClass="form-control" ID="txtIGSTPer" OnTextChanged="txtIGSTPer_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIGSTPer" runat="Server" Text='<%# Eval("IGSTPer") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="IGST" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("IGSTAmt") %>' CssClass="form-control" ID="txtIGST" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblIGST" runat="Server" Text='<%# Eval("IGSTAmt") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Discount(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Discountpercentage") %>' CssClass="form-control" ID="txtDiscount" OnTextChanged="txtDiscount_TextChanged" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDiscount" runat="Server" Text='<%# Eval("Discountpercentage") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Discount Amount" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("DiscountAmount") %>' CssClass="form-control" ReadOnly="true" ID="txtDiscountAmount" AutoPostBack="true" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDiscountAmount" runat="Server" Text='<%# Eval("DiscountAmount") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Grand Total" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Alltotal") %>' CssClass="form-control" ID="txtAlltotal" Width="100px" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAlltotal" runat="Server" Text='<%# Eval("Alltotal") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <%--<asp:LinkButton ID="btn_edit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>--%>

                                                        <asp:LinkButton ID="btn_edit" CausesValidation="false" Text="Edit" runat="server" CommandName="Edit"><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>

                                                        <asp:LinkButton runat="server" ID="lnkbtnDelete" OnClick="lnkbtnDelete_Click" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="gv_update" OnClick="gv_update_Click" Text="Update" CssClass="btn btn-primary btn-sm" runat="server"></asp:LinkButton>&nbsp;
                                                        <asp:LinkButton ID="gv_cancel" OnClick="gv_cancel_Click" CausesValidation="false" Text="Cancel" CssClass="btn btn-primary btn-sm " runat="server"></asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <br />
                            </div>

                            <%--Grid View End--%>


                            <%--last total show--%>
                            <div id="divTotalPart" visible="false" runat="server">
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <br />

                                                <center>
                                        <div class="col-md-12">
                                            <asp:Label ID="lbl_total_amt" runat="server" class="control-label col-sm-6">Total Amount (In Words) :<span class="spncls"></span></asp:Label><br />
                                            <asp:Label ID="lbl_total_amt_Value" ForeColor="red" class="control-label col-sm-6 font-weight-bold" runat="server" Text=""></asp:Label>
                                             <asp:HiddenField ID="hfTotal" runat="server" />
                                        </div>
                                            </center>
                                            </div>
                                            <div class="col-md-6" style="text-align: right">
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lbl_Subtotal" runat="server" class="control-label col-sm-6">SubTotal :<span class="spncls"></span></asp:Label>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_Subtotal" Text="0.00"></asp:Label><br />
                                                    </div>
                                                </div>
                                                <asp:Panel ID="taxPanel1" runat="server">
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <asp:Label ID="lbl_cgst9" runat="server" class="control-label col-sm-6">CGST  Amount :<span class="spncls"></span></asp:Label>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_cgstamt" Text="0.00"></asp:Label><br />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <asp:Label ID="lbl_sgst9" runat="server" class="control-label col-sm-6">SGST  Amount :<span class="spncls"></span></asp:Label>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_sgstamt" Text="0.00"></asp:Label><br />
                                                        </div>
                                                    </div>
                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <asp:Label ID="lbligst" runat="server" class="control-label col-sm-6">IGST  Amount :<span class="spncls"></span></asp:Label>
                                                        </div>
                                                        <div class="col-md-4">
                                                            <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_igstamt" Text="0.00"></asp:Label><br />
                                                        </div>
                                                    </div>

                                                </asp:Panel>
                                                <div class="row">
                                                    <div class="col-md-6">
                                                        <asp:Label ID="lbl_grandTotal" runat="server" class="control-label col-sm-6">Grand Total :<span class="spncls"></span></asp:Label>
                                                    </div>
                                                    <div class="col-md-4">
                                                        <asp:Label runat="server" class="control-label col-sm-6" ReadOnly="true" ID="txt_grandTotal" Text="0.00"></asp:Label><br />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%--  last total show--%>
                            <div class="row">
                                <div class="col-md-4">
                                    <label for="lblcompanyname" class="form-label LblStyle">Advice by Web Link Services  Pvt. Ltd. : </label>

                                    <asp:TextBox ID="TextBox61" runat="server" TextMode="MultiLine" CssClass="form-control" Width="100%" Height="30px"></asp:TextBox>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-12">

                                    <label for="lblco" class="form-label LblStyle">Name: </label>
                                    <asp:Label ID="Label3" runat="server"></asp:Label><br />
                                    <label for="lblcos" class="form-label LblStyle">Designation: </label>
                                    <asp:Label ID="Label4" runat="server"></asp:Label><br />
                                    <label for="lblco" class="form-label LblStyle">Service attend with: </label>

                                    <asp:DropDownList ID="DropDownList3" Width="280px" CssClass="form-control" runat="server"></asp:DropDownList>

                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btnsave" OnClick="btnsave_Click" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
                                </div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btncancel" OnClick="btncancel_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                                </div>
                                <div class="col-md-4">
                       |
                                </div>
                            </div>
                            <div>
                                <br />
                                <br />
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hhd" runat="server" />
            </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

