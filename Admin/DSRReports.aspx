<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="DSRReports.aspx.cs" Inherits="Admin_DSRReports" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />

    <%--    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=ddlCompanyname]").select2();
        });
    </script>--%>
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
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>

    <script type="text/javascript">
        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "../Content/img/minus.png");
        });
        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "../Content/img/plus.png");
            $(this).closest("tr").next().remove();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">&nbsp <b>DSR REPORTS</b></h4>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-3">
                        <asp:Label ID="lblcompanyname" runat="server" Text="Company Name :"></asp:Label>
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
                        <asp:Label ID="lblfollowupstatus" runat="server" Text="Status of Update :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:DropDownList ID="ddlfollowupStatus" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlfollowupStatus_SelectedIndexChanged" Width="100%" AutoPostBack="true">
                                <asp:ListItem Value="0" Text="- Select Status of Update -"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Not interested"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Ringing"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Follow-Up"></asp:ListItem>
                                <asp:ListItem Value="4" Text="Busy"></asp:ListItem>
                                <asp:ListItem Value="5" Text="Closed"></asp:ListItem>
                                <asp:ListItem Value="6" Text="Call-Closed"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblfromdate" runat="server" Text="From Date :"></asp:Label>
                            <asp:TextBox ID="txtfromdate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lbltodate" runat="server" Text="To Date :"></asp:Label>
                            <asp:TextBox ID="txttodate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>
                        </div>
                    </div>



                    <div class="col-md-1 col-2" style="margin-top: 36px">
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                    </div>
                    <div class="col-md-1 col-2" style="margin-top: 36px">
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />
                    </div>
                </div>
                <br />


            </div>
            <div class="container-fluid">
                <div>

                    <%--<div class="table-responsive text-center">--%>
                    <div class="table">
                        <asp:GridView ID="GVfollowup" runat="server" CellPadding="4" DataKeyNames="ID" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false"
                            OnRowCommand="GVfollowup_RowCommand" OnPageIndexChanging="GVfollowup_PageIndexChanging">
                            <Columns>

                                <asp:TemplateField HeaderText="Sr.No." ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText=" ID" ItemStyle-HorizontalAlign="Center" Visible="false" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblID" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Company ID" ItemStyle-HorizontalAlign="Center" Visible="false" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblccode" runat="server" Text='<%# Eval("CompanyCode") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Company Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <%--<asp:LinkButton ID="linkcname" runat="server" CssClass="linkbtn" CommandName="companyname" Text='<%# Eval("cname") %>' CommandArgument='<%# Eval("id") %>' ToolTip="View Details"></asp:LinkButton>--%>
                                        <asp:Label ID="linkcname" runat="server" CssClass="linkbtn" CommandName="companyname" Text='<%# Eval("CompanyName") %>' CommandArgument='<%# Eval("ID") %>' ToolTip="View Details"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Created Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblmes" runat="server" Text='<%# Eval("date") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Feedback of Client" HeaderStyle-Width="220px" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblmessagee" runat="server" Text='<%# Eval("Feedback") %>'></asp:Label>

                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Owner Name" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblownnamegv" runat="server" Text='<%# Eval("PersonName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Contact" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblcontact" runat="server" Text='<%# Eval("ContactNo") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Area" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblarea" runat="server" Text='<%# Eval("Area") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status of Update" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>

                                        <asp:Label ID="lblUpdateStatus" runat="server" Text='<%# "<strong>" + Eval("Updatefor") + "</strong> - " + Eval("UpdateStatus") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Created By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>

                                        <asp:Label ID="lblCreatedby" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Follow-up Date/Time" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblfolowupdate" runat="server" Text='<%# Eval("followupdate") %>'></asp:Label>
                                        <asp:Label ID="Label1" runat="server" Visible='<%# Eval("followupdate").ToString()==""?true:false %>' Font-Bold="true" Text="-"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Audio File" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <img alt="" style="cursor: pointer" src="../Content/img/plus.png" />
                                        <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                            <div class="row">
                                                <div class="col-md-3">
                                                    <audio controls>

                                                        <source src='<%# Eval("Base64Audio") %>' type="audio/mpeg">
                                                        Your browser does not support the audio element.
                                            
                                                    </audio>
                                                </div>
                                                <div class="col-md-3">
                                                </div>
                                                <div class="col-md-3">
                                                    <asp:Label ID="lblComment" runat="server" Font-Bold="true" Text="Comment :"></asp:Label>
                                                    <asp:TextBox ID="txtComment" CssClass="form-control" placeholder="Mention Comment here" runat="server" TextMode="MultiLine"></asp:TextBox>

                                                    <asp:Label ID="lblOldComment" runat="server" Text='<%# Eval("Comment") %>' Font-Bold="true"></asp:Label>
                                                </div>
                                                <div class="col-md-3" style="margin-top: 24px">
                                                    <asp:Button ID="btnSavecomment" CssClass="btn btn-primary" CommandName="SaveComment" CommandArgument='<%# Container.DataItemIndex %>' runat="server" Text="Save Comment" Style="padding: 8px;" />

                                                </div>
                                            </div>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </div>

                </div>


            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
