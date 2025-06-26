<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="SalesTargetMaster.aspx.cs" Inherits="Admin_SalesTargetMaster" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        .LblStyle {
            font-weight: 500;
            color: black;
        }

        .spncls {
            color: red;
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
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
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
                timer: 2500,
                showCancelButton: false,
                showConfirmButton: false
            }).then(function () {
                window.location.href = "Admin/UserMasterList.aspx";
            })
        };
        function GetCalculation() {
            var rate = document.getElementById('<%=txtRate.ClientID%>').value;
            var qty = document.getElementById('<%=txtQuantity.ClientID%>').value;
            if (rate != "" && qty != "") {
                var total = rate * qty;
                document.getElementById('<%=txtamount.ClientID%>').value = total;
                document.getElementById('<%=hfCalculatedAmount.ClientID%>').value = total;

            }
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="container-fluid px-3">
                    <div class="row">
                        <div class="col-md-10">
                            <h4 class="mt-4">&nbsp <b>SALES TARGET MASTER</b></h4>
                        </div>
                        <div class="col-md-2 mt-4">
                            <asp:LinkButton ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="Button1_Click">
    <i class="fas fa-file-alt"></i>&nbsp List
                            </asp:LinkButton>
                        </div>
                    </div>
                    <hr />
                    <div class="card mb-4">

                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-3 col-12 mb-2">
                                    <label for="lblyaer" class="form-label LblStyle">Year : </label>
                                    <br />
                                    <asp:DropDownList ID="ddlYear" Enabled="false" Font-Bold="true" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-md-3 col-12 mb-2">
                                    <label for="lblMonth" class="form-label LblStyle">Month :</label>
                                    <asp:DropDownList ID="ddlMonth" Width="100%" Font-Bold="true" CssClass="form-control" runat="server">
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

                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblCustomername" class="form-label LblStyle"><span class="spncls">*</span>Customer Name :</label>
                                    <asp:TextBox ID="txtcompanyname" ValidationGroup="1" CssClass="form-control" placeholder="Search Customer Name " runat="server" Width="100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                        ControlToValidate="txtcompanyname" ValidationGroup="1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCompanyList"
                                        TargetControlID="txtcompanyname" Enabled="true">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblcomponent" class="form-label LblStyle"><span class="spncls">*</span>Component :</label>
                                    <asp:TextBox ID="txtcomponent" CssClass="form-control" placeholder="Search Component" runat="server" OnTextChanged="txtcomponent_TextChanged" Width="100%" AutoPostBack="true"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                        ControlToValidate="txtcomponent" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender3" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetComponentList"
                                        TargetControlID="txtcomponent">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblGrade" class="form-label LblStyle"><span class="spncls">*</span>Grade :</label>
                                    <asp:TextBox ID="txtGrade" ValidationGroup="1" CssClass="form-control" placeholder="Search Grade" runat="server" Width="100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="Dynamic" ErrorMessage="Please Enter Grade"
                                        ControlToValidate="txtGrade" ValidationGroup="1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender2" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetGradeList"
                                        TargetControlID="txtGrade" Enabled="true">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-3 col-12 mb-2">
                                    <label for="lblRate" class="form-label LblStyle"><span class="spncls">*</span>Rate :</label>
                                    <asp:TextBox ID="txtRate" CssClass="form-control" TextMode="Number" ValidationGroup="1" onkeyup="GetCalculation()" placeholder="Enter Rate" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="1" runat="server" ErrorMessage="Please fill Rate" ControlToValidate="txtRate" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-3 col-12 mb-2">
                                    <label for="lblQuantity" class="form-label LblStyle"><span class="spncls">*</span>Quantity :</label>
                                    <asp:TextBox ID="txtQuantity" CssClass="form-control" ValidationGroup="1" placeholder="Enter Quantity" onkeyup="GetCalculation()" runat="server"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="1" runat="server" ErrorMessage="Please fill Quantity" ControlToValidate="txtQuantity" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblAmount" class="form-label LblStyle"><span class="spncls">*</span>Total :</label>
                                    <asp:HiddenField ID="hfCalculatedAmount" runat="server" />
                                    <asp:TextBox ID="txtamount" ReadOnly="true" ClientIDMode="Static" ValidationGroup="1" CssClass="form-control" placeholder="Enter Total" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="1" runat="server" ErrorMessage="Please fill Total" ControlToValidate="txtamount" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-3 col-12 mb-3" id="rolecodeid" runat="server" visible="false">
                                    <label for="lblRolecode" class="form-label LblStyle"><span class="spncls">*</span>Target Code :</label>
                                    <asp:TextBox ID="txtTargetcode" CssClass="form-control" ValidationGroup="1" ReadOnly="true" ForeColor="red" placeholder="Enter Target Code" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-3 col-12 mb-2">
                                <label for="lblsalesperson" class="form-label LblStyle"><span class="spncls">*</span>Sales Person : </label>
                                <br />
                                <asp:DropDownList ID="ddlSalesperson" Font-Bold="true" CssClass="form-control" runat="server"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" ValidationGroup="1" runat="server" ErrorMessage="Select Sales Person" ControlToValidate="ddlSalesperson" ForeColor="Red"></asp:RequiredFieldValidator>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btnsave" OnClick="btnsave_Click" ValidationGroup="1" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
                                </div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btncancel" CausesValidation="false" OnClick="btncancel_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                                </div>
                                <div class="col-md-4"></div>
                            </div>
                        </div>
                    </div>
                    <asp:HiddenField ID="hhd" runat="server" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
