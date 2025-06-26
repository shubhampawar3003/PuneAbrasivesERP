<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="InventoryList.aspx.cs" Inherits="Admin_InventoryList" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />

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
                window.location.href = "Login.aspx";
            })
        };
    </script>
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

                .pagination-ys table > tbody > tr > td > a:hover,
                .pagination-ys table > tbody > tr > td > span:hover,
                .pagination-ys table > tbody > tr > td > a:focus,
                .pagination-ys table > tbody > tr > td > span:focus {
                    color: #97310e;
                    background-color: #eeeeee;
                    border-color: #dddddd;
                }
    </style>

    <link href="../Content/css/cards.css" rel="stylesheet" />
    <script>
        function resetNotificationCount() {
            document.getElementById('notificationCount').innerText = '0';
            // Optionally, if you want to hide the badge when the count is zero:
            document.getElementById('notificationCount').style.display = 'none';
        }

    </script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script>
        function markNotificationsAsSeen() {
            $.ajax({
                type: 'POST',
                url: 'Dashboard.aspx/MarkNotificationsAsSeen',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d === 'success') {
                        // Optionally refresh the notifications or update the UI
                        // $('#lblcount').text('0');
                        // window.location.reload();
                        window.location.href = "../Admin/LessComponantData.aspx";
                    }
                }
            });
        }
        $(document).ready(function () {
            $('#notificationDropdown').on('hidden.bs.dropdown', function () {

                // Refresh the page when the dropdown is closed
                window.location.reload();
            });
        });
    </script>
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
                    <div class="col-11 col-md-11">
                        <h4 class="mt-4 "><b>INVENTORY LIST</b></h4>
                    </div>
                    <div class="col-md-1" style="margin-top: 20px">
                        <a class="nav-link" href="#" id="notificationDropdown" role="button" aria-expanded="false" onclick="markNotificationsAsSeen()">
                            <i class="fas fa-bell" style="font-size: 20px;"></i>
                            <span class="badge bg-danger" id="notificationCount" runat="server">
                                <asp:Label ID="lblcount" runat="server" Text="0"></asp:Label>
                            </span>
                        </a>

                    </div>

                </div>
                <div class="row">
                    <div class="col-md-3">

                        <div style="margin-top: 14px;">
                            <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Component Name :"></asp:Label>
                            <asp:TextBox ID="txtproduct" CssClass="form-control" placeholder="Search Component" runat="server" OnTextChanged="txtproduct_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetProductList"
                                TargetControlID="txtproduct">
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
                    <div class="col-md-3" style="text-align: center; margin-top: 30px;">
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />

                        <asp:Button ID="btnDownload" OnClick="btnDownload_Click" CssClass="btn btn-success" runat="server" Text="Excel" Style="padding: 8px;" />

                    </div>
                </div>

                <div>
                    <div class="row">
                        <%--<div class="table-responsive text-center">--%>
                        <div style="overflow-x: auto; max-height: 400px; overflow-y: auto; border: 1px solid #ccc;">
                            <asp:GridView ID="GVInentory" runat="server" CellPadding="4" Width="100%" OnRowDataBound="GVInentory_RowDataBound" OnRowCommand="GVInentory_RowCommand"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="ACTION" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" Height="27px" CausesValidation="false" CommandName="ShowDetails" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-eye' style='font-size:24px;color: #212529;'></i></asp:LinkButton>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Invoice No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="InvoiceNo" runat="server" Text='<%#Eval("InvoiceNo")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Inward Date" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="InvoiceDate" runat="server" Text='<%#Eval("InvoiceDate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Component Name" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Productname" runat="server" Text='<%#Eval("ComponentName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Grade" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Description" runat="server" Text='<%#Eval("Description")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Batch" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Batch" runat="server" Text='<%#Eval("Batch")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Inward Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="IQuantity" runat="server" Text='<%#Eval("InwardQty")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Outward Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="OQuantity" runat="server" Text='<%#Eval("OutwardQty")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Pending Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="PQuantity" runat="server" Text='<%#Eval("BalanceQty")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Rate" runat="server" Text='<%#Eval("Rate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false"></rsweb:ReportViewer>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnDownload" />
            <asp:PostBackTrigger ControlID="GVInentory" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:Button ID="btnhist" runat="server" Style="display: none" />
    <asp:ModalPopupExtender ID="ModalPopupHistory" runat="server" TargetControlID="btnhist"
        PopupControlID="PopupHistoryDetail" OkControlID="Closepophistory" />

    <asp:Panel ID="PopupHistoryDetail" runat="server" CssClass="modelprofile1">
        <div class="row container">
            <div class="col-md-4"></div>
            <div class="col-md-8">
                <div class="profilemodel2">
                    <div class="headingcls">
                        <h4 class="modal-title">Component Details
                              
                            <button type="button" id="Closepophistory" class="btnclose" style="display: inline-block;" data-dismiss="modal">Close</button></h4>
                    </div>

                    <%--                    <br />
                    <div class="col-md-4" style="text-align: left">
                        <asp:LinkButton ID="LinkButton1" CssClass="form-control btn btn-info" data-dismiss="modal" Font-Bold="true" CausesValidation="false" runat="server" OnClick="LinkButton1_Click">
    <i class="fas fa-file-alt"></i>&nbsp&nbsp DSR LIST
                        </asp:LinkButton>
                    </div>--%>
                    <div class="body" style="margin-right: 10px; margin-left: 10px; padding-right: 1px; padding-left: 1px;">
                        <div class="row">

                            <asp:GridView ID="gvDetails" runat="server" DataKeyNames="ID"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="150px" DataField="ID" HeaderText="Sr. No." />
                                    <asp:BoundField ItemStyle-Width="150px" DataField="ComponentName" HeaderText="Component Name" />
                                    <asp:BoundField ItemStyle-Width="150px" DataField="Description" HeaderText="Description" />
                                    <asp:BoundField ItemStyle-Width="150px" DataField="InwardQty" HeaderText="Inward Qty" />
                                    <asp:BoundField ItemStyle-Width="150px" DataField="InvoiceDate" HeaderText="Inward DateTime" />
                                    <asp:BoundField ItemStyle-Width="150px" DataField="OutwardQty" HeaderText="Outward Qty" />
                                    <asp:BoundField ItemStyle-Width="150px" DataField="OutwradDate" HeaderText="Outward DateTime" />

                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>

                </div>
            </div>
        </div>


    </asp:Panel>


</asp:Content>

