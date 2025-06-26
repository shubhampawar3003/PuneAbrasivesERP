<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="SalesTargetList.aspx.cs" EnableEventValidation="false" Inherits="Admin_SalesTargetList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />
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
                window.location.href = "UserMaster.aspx";
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
                <div class="container-fluid px-4">
                    <div class="row">
                        <div class="col-md-10">
                            <h4 class="mt-4">&nbsp <b>SALES TARGET LIST</b></h4>
                        </div>
                        <div class="col-md-2 mt-4">
                            <asp:LinkButton ID="btnCreate1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="btnCreate_Click">
<i class="fas fa-file-alt"></i>&nbsp Create
                            </asp:LinkButton>
                        </div>
                        <div class="row">
                            <div class="col-md-3">
                                <div style="margin-top: 14px;">
                                    <asp:Label ID="lblyear" Font-Bold="true" runat="server" Text="YEAR :"></asp:Label>
                                    <asp:DropDownList ID="ddlYear" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div style="margin-top: 14px;">
                                    <asp:Label runat="server" ID="lblMonth" Font-Bold="true" class="form-label LblStyle">Month :</asp:Label>
                                    <asp:DropDownList ID="ddlMonth" Width="100%" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="0">-- Select Month --</asp:ListItem>
                                        <asp:ListItem Value="1">January</asp:ListItem>
                                        <asp:ListItem Value="2">February</asp:ListItem>
                                        <asp:ListItem Value="3">March</asp:ListItem>
                                        <asp:ListItem Value="4">April</asp:ListItem>
                                        <asp:ListItem Value="5">May</asp:ListItem>
                                        <asp:ListItem Value="6">June</asp:ListItem>
                                        <asp:ListItem Value="7">July</asp:ListItem>
                                        <asp:ListItem Value="8">August</asp:ListItem>
                                        <asp:ListItem Value="9">September</asp:ListItem>
                                        <asp:ListItem Value="10">October</asp:ListItem>
                                        <asp:ListItem Value="11">November</asp:ListItem>
                                        <asp:ListItem Value="12">December</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div style="margin-top: 14px;">
                                    <asp:Label ID="lblcompanyname" Font-Bold="true" runat="server" Text="Company Name :"></asp:Label>
                                    <asp:TextBox ID="txtCustomerName" CssClass="form-control" placeholder="Search Customer" runat="server" OnTextChanged="txtCustomerName_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Customer Name"
                                        ControlToValidate="txtCustomerName" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCustomerList"
                                        TargetControlID="txtCustomerName">
                                    </asp:AutoCompleteExtender>

                                </div>
                            </div>
                            <div class="col-md-3 col-12 mb-3">
                                <div style="margin-top: 14px;">
                                    <asp:Label for="lblcomponent" runat="server" Font-Bold="true">Component :</asp:Label>
                                    <asp:TextBox ID="txtcomponent" CssClass="form-control" placeholder="Search Component" runat="server" OnTextChanged="txtcomponent_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender4" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetComponentList"
                                        TargetControlID="txtcomponent">
                                    </asp:AutoCompleteExtender>
                                </div>
                            </div>
                            <div class="col-md-3 col-12 mb-3">
                                <div style="margin-top: 14px;">
                                    <asp:Label ID="lblGrade" Font-Bold="true" runat="server" Text="Grade :"></asp:Label>

                                    <asp:TextBox ID="txtGrade" ValidationGroup="1" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtGrade_TextChanged" placeholder="Search Grade" runat="server" Width="100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="Dynamic" ErrorMessage="Please Enter Grade"
                                        ControlToValidate="txtGrade" ValidationGroup="1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender3" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetGradeList"
                                        TargetControlID="txtGrade" Enabled="true">
                                    </asp:AutoCompleteExtender>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div style="margin-top: 14px;">
                                    <asp:Label ID="lbluser" Font-Bold="true" runat="server" Text="Sales Person Name :"></asp:Label>
                                    <asp:TextBox ID="txtusername" CssClass="form-control" placeholder="Search Sales Person Name" runat="server" OnTextChanged="txtusername_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetUserNameList"
                                        TargetControlID="txtusername">
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
                            <div class="col-md-3">
                                <div style="margin-top: 34px;">
                                    <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                                    <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />
                                    <asp:Button ID="btnExport" OnClick="btnExport_Click" CssClass="btn btn-success" runat="server" Text="Export" Style="padding: 8px;" />
                                </div>
                            </div>
                        </div>


                    </div>
                    <div class="col-md-12">
                        <br />
                    </div>
                    <div class="row">
                        <div style="overflow-x: auto; max-height: 600px; overflow-y: auto; border: 1px solid #ccc;">

                            <asp:GridView ID="GVSalesTarget" runat="server" CellPadding="4" DataKeyNames="TargetCode" Width="100%"
                                CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" OnRowCommand="GVSalesTarget_RowCommand" OnRowDataBound="GVSalesTarget_RowDataBound">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Target Code" HeaderStyle-CssClass="gvhead" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="targetcode" runat="server" Text='<%#Eval("TargetCode")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Year" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="yaer" runat="server" Text='<%#Eval("Year")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Month" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Month" runat="server" Text='<%#Eval("Months")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCustomername" runat="server" Text='<%#Eval("CustomerName")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Component" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="avComponent" runat="server" Text='<%#Eval("Component")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Grade" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="avGrade" runat="server" Text='<%#Eval("Grade")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sales Person" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Salesperson" runat="server" Text='<%#Eval("SalesPerson")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Rate" runat="server" Text='<%#Eval("Rate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Quantity" runat="server" Text='<%#Eval("Quantity")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="avAmount" runat="server" Text='<%#Eval("Amount")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sale Quantity" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="SaleQuantity" runat="server" Text='<%#Eval("SaleQuantity")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sale Amount" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="SaleAmount" runat="server" Text='<%#Eval("SaleAmount")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="ACTION" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnEdit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>
                                            <asp:LinkButton ID="btnDelete" runat="server" Height="27px" ToolTip="Delete" CausesValidation="false" CommandName="RowDelete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-trash' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnExport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
