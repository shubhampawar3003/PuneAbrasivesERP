<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="CompanyMaster.aspx.cs" Inherits="Admin_CompanyMaster" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
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

        .spncls {
            color: red;
        }
    </style>
    <script type="text/javascript">
        /* Billing Address Validations for pincode*/
        function GetPincode() {

            var pin = document.getElementById("<%= txtBPincode.ClientID%>").value; // Get the value of the input field
            if (pin) { // Check if the pincode is not empty
                var regex = /^\d{6}$/;

                if (regex.test(pin)) {

                } else {
                    alert("Please enter correct pincode");
                    document.getElementById("<%= txtBPincode.ClientID%>").value = '';

                }
            } else {
                alert("Pincode cannot be empty");
            }


        }

        function GetPincode2() {


            // Loop through all GridView rows (if there's more than one row)
            var gridViewRows = document.getElementById('<%= GVBAddress.ClientID %>').getElementsByTagName('tr');

            for (var i = 0; i < gridViewRows.length; i++) {
                var row = gridViewRows[i];

                // Find the TextBox in each row (ensure it's not a header or footer row)
                if (row.getElementsByTagName('input').length > 0) {
                    var pin = row.getElementsByTagName('input')[2].value; // Assuming the TextBox is the first input field

                    if (pin) { // Check if the pincode is not empty
                        var regex = /^\d{6}$/;

                        if (!regex.test(pin)) {
                            row.cells[3].style.backgroundColor = 'red';
                        } else {
                            row.cells[3].style.backgroundColor = '';
                        }
                    } else {
                        alert("Pincode cannot be empty in row " + (i + 1)); // Alert if empty
                    }
                }
            }
        }

        /* Validations for GST NO.*/
        function GetBGST() {
            var valid = document.getElementById("<%= ddlTypeofSupply.SelectedItem.Text%>").value;
            if (valid != 'EXPWOP') {
                var BGST = document.getElementById("<%= txtBGST.ClientID%>").value; // Get the value of the input field
                if (BGST) { // Check if the pincode is not empty
                    // Corrected regular expression for GST number format
                    var regex = /^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d{1}[Z]{1}[A-Z\d]{1}$/;

                    if (regex.test(BGST)) {
                        // GST is valid; you can proceed with other operations here if needed.
                    } else {
                        alert("Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4");
                        document.getElementById("<%= txtBGST.ClientID%>").value = '';
                    }
                } else {
                    alert("Invalid GST Number.");
                }
            }
        }




        /* Shipping address Validations for pincode*/
        function GetSPincode() {

            var pin = document.getElementById("<%= txtSPincode.ClientID%>").value; // Get the value of the input field
            if (pin) { // Check if the pincode is not empty
                var regex = /^\d{6}$/;

                if (regex.test(pin)) {

                } else {
                    alert("Please enter correct pincode");
                    document.getElementById("<%= txtSPincode.ClientID%>").value = '';

                }
            } else {
                alert("Pincode cannot be empty");
            }
        }

        function GetSPincode2() {


            // Loop through all GridView rows (if there's more than one row)
            var gridViewRows = document.getElementById('<%= GVSAddress.ClientID %>').getElementsByTagName('tr');

            for (var i = 0; i < gridViewRows.length; i++) {
                var row = gridViewRows[i];

                // Find the TextBox in each row (ensure it's not a header or footer row)
                if (row.getElementsByTagName('input').length > 0) {
                    var pin = row.getElementsByTagName('input')[2].value; // Assuming the TextBox is the first input field

                    if (pin) { // Check if the pincode is not empty
                        var regex = /^\d{6}$/;

                        if (!regex.test(pin)) {
                            row.cells[3].style.backgroundColor = 'red';
                        } else {
                            row.cells[3].style.backgroundColor = '';
                        }
                    } else {
                        alert("Pincode cannot be empty in row " + (i + 1)); // Alert if empty
                    }
                }
            }
        }


        /* Validations for GST NO.*/
        function GetSGST() {
            var valid = document.getElementById("<%= ddlTypeofSupply.SelectedItem.Text%>").value;
            if (valid != 'EXPWOP') {
                var SGST = document.getElementById("<%= txtSGST.ClientID%>").value; // Get the value of the input field
                if (SGST) { // Check if the pincode is not empty
                    // Corrected regular expression for GST number format
                    var regex = /^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d{1}[Z]{1}[A-Z\d]{1}$/;

                    if (regex.test(SGST)) {

                    } else {
                        alert("Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4");
                        document.getElementById("<%= txtSGST.ClientID%>").value = '';
                    }
                } else {
                    alert("Invalid GST Number.");
                }
            }
        }






    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-3">
                <div class="container-fluid px-3">
                    <div class="row">
                        <div class="col-md-10">
                            <h4 class="mt-4">&nbsp <b>CUSTOMER MASTER</b></h4>
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
                                <div class="col-md-12 mb-3">
                                    <b>PRIMARY DETAILS<span class="spncls"> (MANDATORY FIELD)</span></b>
                                    <hr style="width: 25%" />
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblcompanyname" class="form-label LblStyle"><span class="spncls">*</span>Customer Name : </label>

                                    <asp:TextBox ID="txtcompanyname" OnTextChanged="txtcompanyname_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                        ControlToValidate="txtcompanyname" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCompanyList"
                                        TargetControlID="txtcompanyname" Enabled="true">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblcompanycode" class="form-label LblStyle">Customer Code :</label>
                                    <asp:TextBox ID="txtcompanycode" CssClass="form-control" ReadOnly="true" ForeColor="red" placeholder="Enter Company Code" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblArea" class="form-label LblStyle"><span class="spncls">*</span>Type of Supply For : </label>
                                    <asp:DropDownList ID="ddlTypeofSupply" OnTextChanged="ddlTypeofSupply_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="-- Select Type of Supply --" Text="-- Select Type of Supply --"></asp:ListItem>
                                        <asp:ListItem Selected="True" Value="B2B" Text="B2B(INDIA)"></asp:ListItem>
                                        <asp:ListItem Value="SEZWOP" Text="SEZWOP"></asp:ListItem>
                                        <asp:ListItem Value="EXPWOP" Text="EXPWOP"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblVendorcode" class="form-label LblStyle">Vendor Code :</label>
                                    <asp:TextBox ID="txtvendorcode" CssClass="form-control" placeholder="Enter Vendor Code" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3" id="contry" runat="server" visible="false">
                                    <label for="lblCountry" class="form-label LblStyle"><span class="spncls">*</span>Country Name :</label>
                                    <asp:DropDownList ID="ddlCountryCode" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblPrimaryemailid" class="form-label LblStyle"><span class="spncls">*</span>Primary Email ID : </label>
                                    <asp:TextBox ID="txtPrimaryEmail" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtPrimaryEmail_TextChanged" TextMode="Email" placeholder="Enter Primary Email ID" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ValidationGroup="1" ErrorMessage="Please Enter Primary Email ID" ControlToValidate="txtPrimaryEmail" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-6 col-12 mb-3" id="mail2" runat="server" visible="false">
                                    <label for="lblSecondaryemailid" class="form-label LblStyle">Secondary Email ID : </label>
                                    <asp:TextBox ID="txtSecondaryemailid" CssClass="form-control" TextMode="Email" placeholder="Enter Secondary Email ID" runat="server"></asp:TextBox>
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Please Enter Secondary Email ID" ControlToValidate="txtSecondaryemailid" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblgstno" class="form-label LblStyle">GST No. :</label>
                                    <asp:TextBox ID="txtgstno" CssClass="form-control" placeholder="Enter GST No." runat="server"></asp:TextBox>
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Please Enter  Company Pan No." ControlToValidate="txtCompanyPan" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblCompanyPan" class="form-label LblStyle">Company Pan No. :</label>
                                    <asp:TextBox ID="txtCompanyPan" CssClass="form-control" placeholder="Enter Company Pan No." MaxLength="12" MinLength="10" runat="server"></asp:TextBox>
                                    <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Please Enter  Company Pan No." ControlToValidate="txtCompanyPan" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label for="lblPaymentTerm" class="form-label LblStyle">Payment Term(In days) :</label>
                                    <asp:TextBox ID="txtPaymentTerm" CssClass="form-control" placeholder="Enter Payment Term" onkeypress="return isNumberKey(event)" MaxLength="3" MinLength="1" runat="server"></asp:TextBox>

                                </div>
                                <div class="card-header">
                                    <b>BILLING ADDRESS</b>
                                    <hr style="width: 25%" />
                                </div>
                                <br />

                                <div class="col-md-12">
                                    <div class="table-responsive">
                                        <table class="table" border="1" style="width: 100%; border: 1px solid #004068;">
                                            <tr class="gvhead">
                                                <td>Address</td>
                                                <td>Location</td>
                                                <td>Pincode</td>
                                                <td>State</td>
                                                <td>GST NO.</td>
                                                <td>Action</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtBAddress" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtBLocation" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtBPincode" CssClass="form-control" onblur="GetPincode()" runat="server"></asp:TextBox>
                                                </td>

                                                <td>

                                                    <asp:DropDownList ID="ddlBState" CssClass="form-control" runat="server"></asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtBGST" CssClass="form-control" onblur="GetBGST()" runat="server"></asp:TextBox>

                                                </td>
                                                <td>
                                                    <asp:Button ID="btnAddBAddress" ValidationGroup="1" CausesValidation="false" OnClick="btnAddBAddress_Click" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <%--<div class="row" id="divdtls">--%>
                                    <div class="table text-center">
                                        <asp:GridView ID="GVBAddress" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                            OnRowEditing="GVBAddress_RowEditing" AutoGenerateColumns="false" OnRowDataBound="GVBAddress_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                        <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Address" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("BillAddress") %>' CssClass="form-control" ID="txtBillAddress" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBillAddress" runat="Server" Text='<%# Eval("BillAddress") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Location" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("BillLocation") %>' CssClass="form-control" ID="txtBillLocation" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBillLocation" runat="Server" Text='<%# Eval("BillLocation") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Pincode" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("BillPincode") %>' onblur="GetPincode2()" CssClass="form-control" ID="txtBillPincode" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBillPincode" runat="Server" Text='<%# Eval("BillPincode") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="State" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:HiddenField runat="server" ID="lblBillStatehdn" Value='<%# Eval("BillState") %>' />

                                                        <asp:DropDownList ID="ddlBState1" CssClass="form-control" runat="server"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBillState" runat="Server" Text='<%# Eval("BillState") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="GST No." ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("GSTno") %>' CssClass="form-control" ID="txtBGSTno" runat="server"></asp:TextBox>

                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBGSTno" runat="Server" Text='<%# Eval("GSTno") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btn_editB" CausesValidation="false" runat="server" CommandName="Edit"><i class="fa fa-edit" style="font-size:24px"></i></asp:LinkButton>&nbsp;                                                 
                              <asp:LinkButton runat="server" ID="lnkbtnDeleteB" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" OnClick="lnkbtnDeleteB_Click" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px; color:red"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="gv_updateB" CausesValidation="false" ValidationGroup="2" OnClick="gv_updateB_Click" runat="server"><i class="fa fa-check-circle" style="font-size:24px; color:green"></i></asp:LinkButton>&nbsp;
                              <asp:LinkButton ID="gv_cancelB" CausesValidation="false" OnClick="gv_cancelB_Click" Text="Cancel" runat="server"><i class="fa fa-times-circle" style="font-size:24px; color:red"></i></asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <br />
                                <div class="col-md-12">
                                    <asp:CheckBox ID="check_addresss" CssClass="check-task border-checkbox-section" OnCheckedChanged="check_addresss_CheckedChanged" AutoPostBack="true" runat="server" />
                                    <asp:Label ID="lblnote" Font-Bold="true" ForeColor="Green" runat="server" Text="Copy Billing Address to Shipping Address"></asp:Label>
                                </div>
                                <br />
                                <div class="card-header">
                                    <b>SHIPPING ADDRESSS</b>
                                    <hr style="width: 25%" />
                                </div>
                                <br />

                                <div class="col-md-12">
                                    <div class="table-responsive">
                                        <table class="table" border="1" style="width: 100%; border: 1px solid #004068;">
                                            <tr class="gvhead">
                                                <td>Address</td>
                                                <td>Location</td>
                                                <td>Pincode</td>
                                                <td>State</td>
                                                <td>GST NO.</td>
                                                <td>Action</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtSAddress" TextMode="MultiLine" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSLocation" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSPincode" CssClass="form-control" onblur="GetSPincode()" runat="server"></asp:TextBox>
                                                </td>

                                                <td>

                                                    <asp:DropDownList ID="ddlSState" CssClass="form-control" runat="server"></asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtSGST" CssClass="form-control" onblur="GetSGST()" runat="server"></asp:TextBox>

                                                </td>
                                                <td>
                                                    <asp:Button ID="txtSbtnAdd" CausesValidation="false" OnClick="txtSbtnAdd_Click" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <%--<div class="row" id="divdtls">--%>
                                    <div class="table text-center">
                                        <asp:GridView ID="GVSAddress" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                            OnRowEditing="GVSAddress_RowEditing" AutoGenerateColumns="false" OnRowDataBound="GVSAddress_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                        <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Address" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("ShippingAddress") %>' CssClass="form-control" ID="txtShipAddress" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShipAddress" runat="Server" Text='<%# Eval("ShippingAddress") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Location" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("ShipLocation") %>' CssClass="form-control" ID="txtShipLocation" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShipLocation" runat="Server" Text='<%# Eval("ShipLocation") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Pincode" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("ShipPincode") %>' onblur="GetSPincode2()" CssClass="form-control" ID="txtShipPincode" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShipPincode" runat="Server" Text='<%# Eval("ShipPincode") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="State" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:HiddenField ID="lblShipStatehdn" runat="server" Value='<%# Eval("ShipState") %>' />
                                                        <asp:DropDownList ID="ddlSState1" CssClass="form-control" runat="server"></asp:DropDownList>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblShipState" runat="Server" Text='<%# Eval("ShipState") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="GST No." ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("ShipGSTNo") %>' CssClass="form-control" ID="txtSGSTno" runat="server"></asp:TextBox>

                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSGSTno" runat="Server" Text='<%# Eval("ShipGSTNo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btn_editS" CausesValidation="false" runat="server" CommandName="Edit"><i class="fa fa-edit" style="font-size:24px"></i></asp:LinkButton>&nbsp;                                                 
  <asp:LinkButton runat="server" ID="lnkbtnDeleteS" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" OnClick="lnkbtnDeleteS_Click" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px; color:red"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="gv_updateS" CausesValidation="false" OnClick="gv_updateS_Click" runat="server"><i class="fa fa-check-circle" style="font-size:24px; color:green"></i></asp:LinkButton>&nbsp;
  <asp:LinkButton ID="gv_cancelS" CausesValidation="false" OnClick="gv_cancelS_Click" Text="Cancel" runat="server"><i class="fa fa-times-circle" style="font-size:24px; color:red"></i></asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <br />


                                <hr />
                                <div class="col-md-12 mb-3">
                                    <b>SECONDORY DETAILS<span class="spncls"> (NOT-MANDATORY FIELD)</span></b>
                                    <hr style="width: 25%" />
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblUDYAM" class="form-label LblStyle">UDYAM No. :</label>
                                    <asp:TextBox ID="txtUDYAM" CssClass="form-control" placeholder="Enter UDYAM No." runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblCINNO" class="form-label LblStyle">CIN NO. : </label>
                                    <asp:TextBox ID="txtCINNO" CssClass="form-control" placeholder="CIN NO." runat="server"></asp:TextBox>
                                    <%--  <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please Enter CIN NO." ControlToValidate="txtCINNO" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblClienttype" class="form-label LblStyle">Client type. : </label>
                                    <%-- <asp:TextBox ID="txtClienttype" CssClass="form-control" placeholder="Client type" runat="server"></asp:TextBox>--%>
                                    <asp:DropDownList ID="ddlClientType" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="Paid" Text="Paid" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="UnPaid" Text="UnPaid"></asp:ListItem>
                                    </asp:DropDownList>
                                    <%--  <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Please Enter Client type" ControlToValidate="txtClienttype" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblWebsiteLink" class="form-label LblStyle">Website Link :</label>
                                    <asp:TextBox ID="txtWebsiteLink" CssClass="form-control" placeholder="Enter Website Link" runat="server"></asp:TextBox>
                                    <%--   <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ErrorMessage="Please Enter  Website Link" ControlToValidate="txtWebsiteLink" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:HiddenField ID="HFfile1" runat="server" />
                                    <label for="lblCompanyPan" class="form-label LblStyle">Visiting Card :</label>
                                    <asp:FileUpload ID="FileUpload1" CssClass="form-control" runat="server" />
                                    <asp:Label ID="lblPath1" runat="server" Text="" ForeColor="Red"></asp:Label>

                                    <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ErrorMessage="Please Enter  Company Pan No." ControlToValidate="txtCompanyPan" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblCreditLimit" class="form-label LblStyle">Credit Limit: </label>
                                    <asp:TextBox ID="TxtCreditLimit" CssClass="form-control" placeholder="Enter Credit Limit" runat="server"></asp:TextBox>
                                </div>

                                <div class="card-header">
                                    <b>CONTACT DETAILS</b>
                                    <hr style="width: 25%" />
                                </div>
                                <br />
                                <div class="col-md-12">
                                    <div class="table-responsive">
                                        <table class="table" border="1" style="width: 100%; border: 1px solid #004068;">
                                            <tr class="gvhead">
                                                <td>Name</td>
                                                <td>Mobile No.</td>
                                                <td>Email ID</td>
                                                <td>Department</td>
                                                <td>Designation</td>
                                                <td>Action</td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtname" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtmobile" TextMode="Number" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtemaili" TextMode="Email" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>

                                                <td>
                                                    <asp:TextBox ID="txtDepartment" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:TextBox ID="txtdesignation" CssClass="form-control" runat="server"></asp:TextBox>
                                                </td>
                                                <td>
                                                    <asp:Button ID="btnAddMore" CausesValidation="false" OnClick="btnAddMore_Click" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Add" />
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <%--<div class="row" id="divdtls">--%>
                                    <div class="table text-center">
                                        <asp:GridView ID="dgvContactDetails" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys"
                                            OnRowEditing="dgvContactDetails_RowEditing" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                        <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Name" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Name") %>' CssClass="form-control" ID="txtname" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblname" runat="Server" Text='<%# Eval("Name") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Number" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Number") %>' CssClass="form-control" ID="txtmobile" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblnumber" runat="Server" Text='<%# Eval("Number") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Email ID" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("EmailID") %>' CssClass="form-control" ID="txtemaili" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblemailid" runat="Server" Text='<%# Eval("EmailID") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Department" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Department") %>' CssClass="form-control" ID="txtDepartment" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDepartment" runat="Server" Text='<%# Eval("Department") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Designation" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <EditItemTemplate>
                                                        <asp:TextBox Text='<%# Eval("Designation") %>' CssClass="form-control" ID="txtdesignation" runat="server"></asp:TextBox>
                                                    </EditItemTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbldesignation" runat="Server" Text='<%# Eval("Designation") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btn_edit" CausesValidation="false" runat="server" CommandName="Edit"><i class="fa fa-edit" style="font-size:24px"></i></asp:LinkButton>&nbsp;                                                 
                               <asp:LinkButton runat="server" ID="lnkbtnDelete" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" OnClick="lnkbtnDelete_Click" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="gv_update" CausesValidation="false" OnClick="gv_update_Click" runat="server"><i class="fa fa-check-circle" style="font-size:24px; color:green"></i></asp:LinkButton>&nbsp;
                               <asp:LinkButton ID="gv_cancel" CausesValidation="false" OnClick="gv_cancel_Click" runat="server"><i class="fa fa-times-circle" style="font-size:24px; color:red"></i></asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <br />

                            </div>


                        </div>
                        <%--Grid View End--%>
                        <br />
                        <div class="row">
                            <div class="col-md-4"></div>
                            <div class="col-6 col-md-2">
                                <asp:Button ID="btnsave" OnClick="btnsave_Click" ValidationGroup="1" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
                            </div>
                            <div class="col-6 col-md-2">
                                <asp:Button ID="btncancel" OnClick="btncancel_Click" CausesValidation="false" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                            </div>
                            <div class="col-md-4"></div>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hhd" runat="server" />
            </div>
            <%--</div> --%>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnsave" />
            <asp:PostBackTrigger ControlID="btncancel" />
            <asp:AsyncPostBackTrigger ControlID="btnAddMore" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

