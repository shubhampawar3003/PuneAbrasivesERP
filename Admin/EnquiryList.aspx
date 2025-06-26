<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="EnquiryList.aspx.cs" Inherits="Admin_EnquiryList" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=ddlproduct]").select2();

        });
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
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-9 col-md-10">
                        <h4 class="mt-4 ">&nbsp <b>ENQUIRY LIST</b></h4>
                    </div>
                    <div class="col-3 col-md-2 mt-4">
                        <asp:Button ID="Button1" CssClass="form-control btn btn-warning" runat="server" Text="Add Enquiry" OnClick="btnAddEnquiry_Click" />

                    </div>
                </div>

                <br />
                <div class="row">
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
                        <asp:Label ID="lblstatus" runat="server" Text="Status of Update :"></asp:Label>
                        <div style="margin-top: 14px;">
                            <asp:DropDownList ID="ddlStatus" CssClass="form-control" runat="server" OnSelectedIndexChanged="ddlfollowupStatus_SelectedIndexChanged" Width="100%" AutoPostBack="true">
                                <asp:ListItem Value="1" Text="Open"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Closed"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-1 col-3" style="margin-top: 36px">
                        <asp:Button ID="btnSearchData" CssClass="btn btn-primary" OnClick="btnSearchData_Click" runat="server" Text="Search" Style="padding: 8px;" />
                    </div>
                    <div class="col-md-1 col-3" style="margin-top: 36px">
                        <asp:Button ID="btnresetfilter" OnClick="btnresetfilter_Click" CssClass="btn btn-danger" runat="server" Text="Reset" Style="padding: 8px;" />
                    </div>

                </div>
            </div>
            <br />
            <div class="container-fluid">
                <div class="row">
                    <%--<div class="table-responsive text-center">--%>
                    <div class="table ">
                        <asp:GridView ID="GvCompany" runat="server" CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" Width="100%"
                            DataKeyNames="id" OnRowDataBound="GvCompany_RowDataBound" OnRowCommand="GvCompany_RowCommand" AllowPaging="false" OnPageIndexChanging="GvCompany_PageIndexChanging" PageSize="10">
                            <Columns>
                                <asp:TemplateField HeaderText="Sr. No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                        <asp:Label ID="lblfilepath1" runat="server" Visible="false" Text='<%# Eval("filepath1") %>'></asp:Label>
                                        <asp:Label ID="lblfilepath2" runat="server" Visible="false" Text='<%# Eval("filepath2") %>'></asp:Label>
                                        <asp:Label ID="lblfilepath3" runat="server" Visible="false" Text='<%# Eval("filepath3") %>'></asp:Label>
                                        <asp:Label ID="lblfilepath4" runat="server" Visible="false" Text='<%# Eval("filepath4") %>'></asp:Label>
                                        <asp:Label ID="lblfilepath5" runat="server" Visible="false" Text='<%# Eval("filepath5") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Reg. Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblvisitdate" runat="server" Text='<%# Eval("regdate") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblstatus1" runat="server" Text='<%# Eval("status") %>' Font-Bold="true" ForeColor="Green" Visible='<%# Eval("status").ToString() == "Open" ? true:false %>'></asp:Label>
                                        <asp:Label ID="lblIsActive" runat="server" Text='<%# Eval("status") %>' Font-Bold="true" ForeColor="Red" Visible='<%# Eval("status").ToString() == "Close" ? true:false %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Enquiry Code" ItemStyle-HorizontalAlign="Center" Visible="false" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEnqCode" runat="server" Text='<%# Eval("EnqCode") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Name" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblcname" runat="server" Text='<%# Eval("cname") %>'></asp:Label>
                                        <%-- <asp:LinkButton ID="linkcname" runat="server" CssClass="linkbtn" CommandName="companyname" Text='<%# Eval("cname").ToString().Replace(" ", "<br /><br />") %>' CommandArgument='<%# Eval("id") %>' ToolTip="View Details"></asp:LinkButton>--%>
                                    </ItemTemplate>
                                </asp:TemplateField>



                                <asp:TemplateField HeaderText="File1" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <%--<asp:ImageButton ID="ImageButton1" ImageUrl="../img/Open-file.ico" runat="server" Width="30px" OnClick="linkbtnfile_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File" />--%>
                                        <asp:ImageButton ID="ImageButtonfile1" ImageUrl="../Content/img/Open-file2.png" runat="server" Width="30px" OnClick="linkbtnfile_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File" />
                                        <%--<asp:LinkButton ID="linkbtnfile1" runat="server" CssClass="linkbtn" OnClick="linkbtnfile_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File">Open</asp:LinkButton>--%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="File2" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButtonfile2" ImageUrl="../Content/img/Open-file2.png" runat="server" Width="30px" OnClick="linkbtnfile2_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File" />
                                        <%--<asp:LinkButton ID="linkbtnfile2" runat="server" CssClass="linkbtn" OnClick="linkbtnfile2_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File">Open</asp:LinkButton>--%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="File3" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButtonfile3" ImageUrl="../Content/img/Open-file2.png" runat="server" Width="30px" OnClick="linkbtnfile3_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File" />
                                        <%--<asp:LinkButton ID="linkbtnfile3" runat="server" CssClass="linkbtn" OnClick="linkbtnfile3_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File">Open</asp:LinkButton>--%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="File4" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButtonfile4" ImageUrl="../Content/img/Open-file2.png" runat="server" Width="30px" OnClick="linkbtnfile4_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File" />
                                        <%--<asp:LinkButton ID="linkbtnfile4" runat="server" CssClass="linkbtn" OnClick="linkbtnfile4_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File">Open</asp:LinkButton>--%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="File5" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButtonfile5" ImageUrl="../Content/img/Open-file2.png" runat="server" Width="30px" OnClick="linkbtnfile5_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File" />
                                        <%--<asp:LinkButton ID="linkbtnfile5" runat="server" CssClass="linkbtn" OnClick="linkbtnfile5_Click" CommandArgument='<%# Eval("id") %>' ToolTip="Open File">Open</asp:LinkButton>--%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sample" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblsample" runat="server" Text='<%# Eval("Sample") %>' Font-Bold="true" ForeColor="Green" Visible='<%# Eval("Sample").ToString() == "Yes" ? true:false %>'></asp:Label>
                                        <asp:Label ID="lblsampleno" runat="server" Text='<%# Eval("Sample ") %>' Font-Bold="true" ForeColor="Red" Visible='<%# Eval("Sample").ToString() == "No" ? true:false %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sample Date" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSampledate" runat="server" Text='<%# Eval("SampleDate") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="lnkEdit" Visible='<%# Eval("status").ToString() == "Open" ? true:false %>' CommandName="RowEdit" CommandArgument='<%# Eval("id") %>' ToolTip="Edit"><i class="fa fa-edit" style="font-size:24px;color:black;"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" ID="LinkButton1" Visible='<%# Eval("status").ToString() == "Open" ? true:false %>' CommandName="Createquotation" CommandArgument='<%# Eval("id") %>' ToolTip="Create Quotation"><i class="fa fa-arrow-circle-right" style="font-size:24px;color:green;"></i></asp:LinkButton>


                                        <%--<asp:LinkButton ID="linkaccount" runat="server" CssClass="linkbtn" CommandName="status" OnClientClick="return confirm('Do you want to Activate/Deactivate this account ?')" CommandArgument='<%# Eval("id") %>' ToolTip="Activate/Deactivate" Text="Activated"></asp:LinkButton>--%>
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

