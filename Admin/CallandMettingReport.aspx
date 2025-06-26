<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="CallandMettingReport.aspx.cs" Inherits="Admin_CallandMettingReport" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />

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
    <style>
        .dissablebtn {
            cursor: not-allowed;
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
        .LblStyle {
            font-weight: 500;
            color: black;
        }

        .spncls {
            color: red;
        }

        .card_adj {
            margin-bottom: 3px;
            height: 35px;
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">&nbsp <b>DSR REPORT</b></h4>
                    </div>
                    <div class="col-3 col-md-2 mt-4">
                        <asp:Button ID="btnCreate" CssClass="form-control btn btn-warning" OnClick="btnCreate_Click" runat="server" Text="Create" />
                    </div>
                </div>
                <hr />
                <div class="row">
                    <div class="col-md-3">
                        <div style="margin-top: 14px;">
                            <asp:Label ID="lblcompanyname" runat="server" Font-Bold="true" Text="Company Name :"></asp:Label>
                            <asp:TextBox ID="txtcompanyname" OnTextChanged="txtcompanyname_TextChanged" placeholder="Search Company" AutoPostBack="true" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                            <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCompanyList"
                                TargetControlID="txtcompanyname" Enabled="true">
                            </asp:AutoCompleteExtender>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="lblfollowup" runat="server" Font-Bold="true" Text="Type of Update :"></asp:Label>
                        <div style="margin-top: 14px;">

                            <asp:DropDownList ID="ddlfollowup" OnSelectedIndexChanged="ddlfollowup_SelectedIndexChanged" CssClass="form-control" runat="server" Width="100%" AutoPostBack="true">
                                <asp:ListItem Value="- Select Type of Update -" Text="- Select Type of Update -"></asp:ListItem>
                                <asp:ListItem Value="Call" Text="Call"></asp:ListItem>
                                <asp:ListItem Value="Meeting" Text="Meeting"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="lblfollowupstatus" runat="server" Font-Bold="true" Text="Status of Update :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:DropDownList ID="ddlfollowupStatus" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlfollowupStatus_SelectedIndexChanged" Width="100%" AutoPostBack="true">
                                <asp:ListItem Value="0" Text="- Select Status of Update -"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Status of Update :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:DropDownList ID="ddltype" CssClass="form-control" runat="server" Width="100%" AutoPostBack="true">
                                <asp:ListItem Value="- Select Type -" Text="- Select Type -"></asp:ListItem>
                                <asp:ListItem Value="Fresh" Text="Fresh"></asp:ListItem>
                                <asp:ListItem Value="FoIIowup" Text="FoIIowup"></asp:ListItem>
                                <asp:ListItem Value="Trial Visit" Text="Trial Visit"></asp:ListItem>
                                <asp:ListItem Value="Sample" Text="Sample"></asp:ListItem>
                                <asp:ListItem Value="New Development" Text="New Development"></asp:ListItem>
                                <asp:ListItem Value="Cold Call" Text="Cold Call"></asp:ListItem>
                                <asp:ListItem Value="Services" Text="Services"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-3">
                        <asp:Label ID="lblusers" runat="server" Font-Bold="true" Text="Sales Manager :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:DropDownList ID="ddlTeamuser" CssClass="form-control" OnSelectedIndexChanged="ddlTeamuser_SelectedIndexChanged" runat="server" Width="100%" AutoPostBack="true">
                            </asp:DropDownList>

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
                            OnRowDataBound="GVfollowup_RowDataBound" OnRowCommand="GVfollowup_RowCommand" OnPageIndexChanging="GVfollowup_PageIndexChanging">
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="20" HeaderText=" " HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <img alt="" style="cursor: pointer" src="../Content/img/plus.png" />
                                        <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                            <%-- <asp:Label ID="lblmessageee" runat="server" Text='<%# "Feedback : " +  Eval("Feedback") %>'></asp:Label>--%>
                                            <asp:Label ID="lblo" runat="server" CssClass="font-weight-bold" ForeColor="Red" Text="Feedback of Client Update :"></asp:Label>
                                            <asp:Label ID="lblmessagee" runat="server" Text='<%# Eval("Feedback") %>'></asp:Label>
                                           <br />
                                            <audio controls>

                                                <source src='<%# Eval("Base64Audio") %>' type="audio/mpeg">
                                                Your browser does not support the audio element.
                                            
                                            </audio>
                                        </asp:Panel>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Sr. No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
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

                                <asp:TemplateField HeaderText="Created Date" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblmes" runat="server" Text='<%# Eval("ToDate") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Owner Name" Visible="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblownnamegv" runat="server" Text='<%# Eval("PersonName") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Contact" Visible="false" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblcontact" runat="server" Text='<%# Eval("ContactNo") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Area" Visible="false" HeaderStyle-Width="90px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblarea" runat="server" Text='<%# Eval("Area") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status of Update" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>

                                        <asp:Label ID="lblUpdateStatus" runat="server" Text='<%# "<strong>" + Eval("Updatefor") + "</strong> - " + Eval("UpdateStatus") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Follow-up Date" HeaderStyle-Width="120px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <%# Eval("followupdate") %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Created By" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>

                                        <asp:Label ID="lblCreatedby" runat="server" Text='<%# Eval("Username") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                         <%--       <asp:TemplateField HeaderText="Audio File" HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <audio controls>
                                            <source src='<%# Eval("Base64Audio") %>' type="audio/mpeg">
                                            Your browser does not support the audio element.
               
                                        </audio>
                                    </ItemTemplate>
                                </asp:TemplateField>--%>
                                     <asp:TemplateField HeaderText="Comment" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblComment" runat="server" Text='<%# Eval("Comment") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Close Follow-up" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStatus" runat="server" ForeColor="Green" Text="Followup Closed...!" Visible='<%# Eval("Status").ToString() == "2" ? true : false %>'></asp:Label>
                                        <asp:LinkButton ID="lnkCloseUpdate" runat="server" CssClass="linkbtn" CommandArgument='<%#Eval("ID")%>' CommandName="CloseFollowup" Visible='<%# Eval("Status").ToString() == "1" ? true : false %>' Text="Close Follow-up" ToolTip="View Details"></asp:LinkButton>
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
