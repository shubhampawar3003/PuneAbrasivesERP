<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="OutwardEntryList.aspx.cs" Inherits="Admin_OutwardEntryList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />

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
        #loader {
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            z-index: 9999;
            background: rgba(255, 255, 255, 0.8);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .spinner {
            border: 16px solid #f3f3f3; /* Light grey */
            border-top: 16px solid #3498db; /* Blue */
            border-radius: 50%;
            width: 120px;
            height: 120px;
            animation: spin 2s linear infinite;
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
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 "><b>OUTWARD ENTRY LIST</b></h4>
                    </div>
                    <br />

                </div>
                <hr />
                <div class="row">
                    <div class="col-md-3">
                        <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="Challan No.:"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txtOrderno" CssClass="form-control" placeholder="Search Order No" runat="server" OnTextChanged="txtOrderno_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Order No"
                                ControlToValidate="txtOrderno" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetOrderNoList"
                                TargetControlID="txtOrderno">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Customer Name :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txtCustomerName" CssClass="form-control" placeholder="Search Company" runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                ControlToValidate="txtCustomerName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                TargetControlID="txtCustomerName">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="lblfromdate" runat="server" Font-Bold="true" Text="From Date :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txtfromdate" Placeholder="Enter From Date" runat="server" TextMode="Date" AutoComplete="off" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="lbltodate" runat="server" Font-Bold="true" Text="To Date :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:TextBox ID="txttodate" Placeholder="Enter To Date" runat="server" TextMode="Date" AutoComplete="off" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-10 col-8">
                    </div>
                    <div class="col-md-1 col-2">
                        <asp:LinkButton ID="btnSearch" OnClick="btnSearch_Click" runat="server" Width="100%" CssClass="btn btn-info"><i style="color:white" class="fa">&#xf002;</i> &nbsp;</asp:LinkButton>
                    </div>
                    <div class="col-md-1 col-2">
                        <asp:LinkButton ID="btnrefresh" OnClick="btnrefresh_Click" runat="server" Width="100%" CssClass="btn btn-warning"><i style="color:white" class="fa">&#xf021;</i> &nbsp;</asp:LinkButton>
                    </div>
                </div>
                <br />
                <div>
                    <div class="row">
                        <%--<div class="table-responsive text-center">--%>
                        <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">
                            <asp:GridView ID="GVOutward" runat="server" CellPadding="4" DataKeyNames="ID" PageSize="10" AllowPaging="true" Width="100%"
                                OnRowCommand="GVOutward_RowCommand" OnPageIndexChanging="GVOutward_PageIndexChanging" OnRowDataBound="GVOutward_RowDataBound" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No." HeaderStyle-CssClass="gvhead sno">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSRNO" runat="server" Text='<%#Container.DataItemIndex +1 %>'></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Invoice No" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblInvoiceno" runat="server" Text='<%#Eval("invoiceno")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="P.O. No" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPONO" runat="server" Text='<%#Eval("pono")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Company Name" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblcompanyname" runat="server" Text='<%#Eval("companyname")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LR No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lbllrno" runat="server" Text='<%#Eval("LRNo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="ACTION" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnlrletter" runat="server" Height="27px" ToolTip="Show LR Letter" Visible='<%# Eval("LRLetterPath").ToString() == "" ? false : true %>' CausesValidation="false" CommandName="Letter" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-file-pdf' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                            &nbsp;
                                            <asp:LinkButton ID="btnshow" runat="server" Height="27px" ToolTip="Upload Letter"  CausesValidation="false" CommandName="show" CommandArgument='<%#Eval("ID")%>'><i class='fa fa-upload' style='font-size:24px;color: #212529;'></i></asp:LinkButton>
                                            &nbsp;
                                            <asp:LinkButton ID="btnDelete" runat="server" Height="27px" ToolTip="Delete" CausesValidation="false" CommandName="RowDelete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-trash' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                            &nbsp;                                        
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
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
                        <h4 class="modal-title">UPLOAD LR LETTER 
                                    <button type="button" id="Closepophistory" class="btnclose" style="display: inline-block;" data-dismiss="modal">Close</button></h4>
                    </div>

                    <br />
                    <div class="body" style="margin-right: 10px; margin-left: 10px; padding-right: 1px; padding-left: 1px;">
                        <div class="row">
                            <div class="col-md-6 col-12 mb-3">
                                <asp:HiddenField ID="HFfile1" runat="server" />
                                <asp:Label ID="lblCompanyPan" runat="server" Font-Bold="true" CssClass="form-label LblStyle">LR Letter:</asp:Label>
                                <asp:FileUpload ID="FileUpload1" CssClass="form-control" runat="server" />
                                <asp:Label ID="lblPath1" runat="server" Text="" ForeColor="Red"></asp:Label>
                            </div>
                            <div class="col-md-6 col-12 mb-3">
                                <asp:Label ID="Label3" Font-Bold="true" runat="server" CssClass="form-label LblStyle">LR No. :</asp:Label>
                                <asp:TextBox ID="txtLRno" CssClass="form-control" placeholder="Enter LR No." runat="server"></asp:TextBox>
                            </div>
                            <div class="col-md-4" style="margin-top: 18px">
                                <asp:Button ID="btnsave" OnClick="btnsave_Click" OnClientClick="showLoader();" ToolTip="Save & Send" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save & Send" />

                            </div>
                            <div class="col-md-12">
                                <span style="color: red">Note: Click on save button send LR letter on mail of customer.</span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>

        <div id="loader" style="display: none;">
            <div class="spinner"></div>
        </div>
    </asp:Panel>
</asp:Content>
