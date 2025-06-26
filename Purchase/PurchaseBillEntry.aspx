<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="PurchaseBillEntry.aspx.cs" Inherits="Admin_PurchaseBillEntry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
   
    <script>
        window.addEventListener('DOMContentLoaded', function () {
            document.body.classList.add('sb-sidenav-toggled');
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
            /*margin-top: 5px;*/
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
        function toggleAll(source) {
            var grid = document.getElementById('<%= dgvTaxinvoiceDetails.ClientID %>');
            var checkBoxes = grid.getElementsByTagName("input");

            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i].type == "checkbox" && checkBoxes[i].id.indexOf("chkRow") !== -1) {
                    checkBoxes[i].checked = source.checked;
                }
            }
        }



    </script>

    <style>
        /* Loader CSS */
        .loader-wrapper {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(255, 255, 255, 0.8);
            display: flex;
            justify-content: center;
            align-items: center;
            z-index: 1000;
            /* Ensure it appears above other content */
            display: none;
            /* Hidden by default */
        }

        .loader {
            border: 8px solid #f3f3f3;
            /* Light grey */
            border-top: 8px solid #3498db;
            /* Blue */
            border-radius: 50%;
            width: 50px;
            height: 50px;
            animation: spin 1s linear infinite;
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

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/jquery-validation@1.19.3/dist/jquery.validate.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/jquery-validation-unobtrusive@3.2.11/dist/jquery.validate.unobtrusive.min.js"></script>
    <style>
        /* New style for suggestion tab */
        .suggestion-list {
            position: absolute;
            z-index: 1000;
            background-color: #fff;
            max-height: 100px;
            width: 20%;
            box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
        }

        .suggestion-item {
            padding: 8px;
            cursor: pointer;
        }

            .suggestion-item:hover {
                background-color: #f0f0f0;
            }
    </style>
    <script>
        function GetComponent(component) {
            if (component) {
                $.ajax({
                    type: 'POST',
                    url: 'PurchaseBillEntry.aspx/GetComponent', // Corrected method name
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    data: JSON.stringify({ Component: component }), // Proper JSON formatting
                    success: function (response) {
                        var suggestions = response.d; // ASP.NET typically wraps response in "d"
                        var suggestionList = $('#suggestion-list');

                        suggestionList.empty();

                        if (suggestions.length > 0) {
                            suggestions.forEach(function (item) {
                                suggestionList.append('<div class="suggestion-item" data-value="' + item.Value + '">' + item.Text + '</div>');
                            });
                            suggestionList.show();
                        } else {
                            suggestionList.hide();
                        }
                    },
                    error: function (xhr, status, error) {
                        console.log('Error: ' + error);
                    }
                });
            } else {
                $('#suggestion-list').hide();
            }
        }
    </script>


    <script>


        $(document).on('click', '.suggestion-item', function () {
            var selectedText = $(this).text();
            // Find the parent GridView row
            var row = $(this).closest('tr');
            var grid = document.getElementById("ContentPlaceHolder1_dgvTaxinvoiceDetails");
            var rows = grid.getElementsByTagName("tr");

            for (var i = 0; i < rows.length; i++) {
                var txtAmountReceive = rows[i].querySelector('[id*="txtProduct"]');
                if (txtAmountReceive && txtAmountReceive.value !== '') {
                    if (txtAmountReceive.innerHTML === '') {
                        txtAmountReceive.value = selectedText;
                    }

                }
            }
            row.find('.suggestion-list').hide();
        });
    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            calculateRowTotal(2);
        });
        function calculateRowTotal(input) {
            var grid = document.getElementById("ContentPlaceHolder1_dgvTaxinvoiceDetails");
            var rows = grid.getElementsByTagName("tr");
            var total = 0;
            var totals = 0;

            for (var i = 0; i < rows.length; i++) {
                var qtyField = rows[i].querySelector('[id*="txtQuantity"]');
                var rateField = rows[i].querySelector('[id*="txtRate"]');
                var cgstField = rows[i].querySelector('[id*="txtCGST"]');
                var sgstField = rows[i].querySelector('[id*="txtSGST"]');
                var igstField = rows[i].querySelector('[id*="txtIGST"]');
                var discountField = rows[i].querySelector('[id*="txtDiscount"]');
                var qty = parseFloat(qtyField?.value) || 0;
                var rate = parseFloat(rateField?.value) || 0;
                var cgst = parseFloat(cgstField?.value) || 0;
                var sgst = parseFloat(sgstField?.value) || 0;
                var igst = parseFloat(igstField?.value) || 0;
                var discount = parseFloat(discountField?.value) || 0;
                total = qty * rate;
                totals += total;
            }

            var discountAmount = (totals * discount) / 100;
            var afterDiscount = totals - discountAmount;
            var taxAmount = (afterDiscount * (cgst + sgst + igst)) / 100;
            var total1 = afterDiscount + taxAmount;
            var sumofAmount = document.querySelector("[id$='sumofAmount']");
            var hdnGrandtotal = document.querySelector("[id$='hdnGrandtotal']");
            var txtGrandTot = document.querySelector("[id$='txtGrandTot']");
            var hdnProductAmt = document.querySelector("[id$='hdnProductAmt']");

            if (totals) {
                sumofAmount.value = totals.toFixed(2);

            }
            if (total1) {
                hdnGrandtotal.value = total1.toFixed(2);
                hdnProductAmt.value = total1.toFixed(2);
                txtGrandTot.value = total1.toFixed(2);
            }

            calculateFright(2);
            calculateTransport(2);



        }

        function calculateFright(input) {
            var sumofAmount = document.getElementById('<%= sumofAmount.ClientID %>');
                var rateField = document.getElementById('<%= txtRate.ClientID %>');
                var FBasicAmt = document.getElementById('<%= txtBasic.ClientID %>');
                var FCost = document.getElementById('<%= txtCost.ClientID %>');
                var hdnFCost = document.getElementById('<%= hdnFCost.ClientID %>');
                var FCGSTPer = document.getElementById('<%= CGSTPer.ClientID %>');
                var FSGSTPer = document.getElementById('<%= SGSTPer.ClientID %>');
                var FIGSTPer = document.getElementById('<%= IGSTPer.ClientID %>');
            var amount = parseFloat(sumofAmount.value) || 0;
            var rate = parseFloat(rateField.value) || 0;
            var Cost = parseFloat(FCost.value) || 0;
            var CGSTPer = parseFloat(FCGSTPer.value) || 0;
            var SGSTPer = parseFloat(FSGSTPer.value) || 0;
            var IGSTPer = parseFloat(FIGSTPer.value) || 0;

            var FranAmount = (amount * rate) / 100;
            var FGstAmount = (FranAmount * (CGSTPer + SGSTPer + IGSTPer)) / 100;
            var FgrdTotal = FGstAmount + FranAmount;

            FBasicAmt.value = FranAmount.toFixed(2);
            FCost.value = FgrdTotal.toFixed(2);
            hdnFCost.value = FgrdTotal.toFixed(2);
            calculateTCS(2);
        }

        function calculateTransport(input) {
            var sumofAmount = document.getElementById('<%= sumofAmount.ClientID %>');
                var TCharge = document.getElementById('<%= txtTCharge.ClientID %>');
                var TCost = document.getElementById('<%= txtTCost.ClientID %>');
                var hdnTCost = document.getElementById('<%= hdnTCost.ClientID %>');
                var TCGSTPer = document.getElementById('<%= txtTCGSTPer.ClientID %>');
                var TSGSTPer = document.getElementById('<%= txtTSGSTPer.ClientID %>');
                var TIGSTPer = document.getElementById('<%= txtTIGSTPer.ClientID %>');

                var TCGSTAmt = document.getElementById('<%= txtTCGSTamt.ClientID %>');
                var TSGSTAmt = document.getElementById('<%= txtTSGSTamt.ClientID %>');
                var TIGSTAmt = document.getElementById('<%= txtTIGSTamt.ClientID %>');

            var amount = parseFloat(sumofAmount.value) || 0;
            var Charge = parseFloat(TCharge.value) || 0;
            var CGST = parseFloat(TCGSTPer.value) || 0;
            var SGST = parseFloat(TSGSTPer.value) || 0;
            var IGST = parseFloat(TIGSTPer.value) || 0;

            var CGSTAmt = parseFloat(TCGSTAmt.value) || 0;
            var SGSTAmt = parseFloat(TSGSTAmt.value) || 0;
            var IGSTAmt = parseFloat(TIGSTAmt.value) || 0;
            if (CGST) {
                CGSTAmt = (Charge * CGST) / 100;
                TCGSTAmt.value = CGSTAmt.toFixed(2);
            }
            if (SGST) {

                SGSTAmt = (Charge * SGST) / 100;
                TSGSTAmt.value = SGSTAmt.toFixed(2);
            }
            if (IGST) {
                IGSTAmt = (Charge * IGST) / 100;
                TIGSTAmt.value = IGSTAmt.toFixed(2);
            }

            var TGstAmount = Charge + CGSTAmt + SGSTAmt + IGSTAmt;
            var TgrdTotal = TGstAmount;
            TCost.value = TgrdTotal.toFixed(2);
            hdnTCost.value = TgrdTotal.toFixed(2);
            calculateTCS(2);
        }

        function calculateTCS(input) {
            var txtTCSPer = document.getElementById('<%= txtTCSPer.ClientID %>');
                var txtTCSAmt = document.getElementById('<%= txtTCSAmt.ClientID %>');

                var TCSPer = parseFloat(txtTCSPer.value) || 0;
                var TCSAmt = parseFloat(txtTCSAmt.value) || 0;

                var hdnProductAmt = document.getElementById('<%= hdnProductAmt.ClientID %>');
                var hdnTCSAmount = document.getElementById('<%= hdnTCSAmount.ClientID %>');
                var FCost = document.getElementById('<%= txtCost.ClientID %>');
                var TCost = document.getElementById('<%= txtTCost.ClientID %>');

                var ProductAmt = parseFloat(hdnProductAmt.value) || 0;
                var FRigCost = parseFloat(FCost.value) || 0;
                var TranCost = parseFloat(TCost.value) || 0;

                var Total = ProductAmt + FRigCost + TranCost;
                var TCSAmt = (Total * TCSPer) / 100;
                if (TCSAmt) {
                    txtTCSAmt.value = TCSAmt.toFixed(2);
                    hdnTCSAmount.value = TCSAmt.toFixed(2);
                }

                var grandtotal = TCSAmt + Total;

                //Grandtotal calculation
                var hdnGrandtotal = document.getElementById('<%= hdnGrandtotal.ClientID %>');
                var txtGrandTot = document.getElementById('<%= txtGrandTot.ClientID %>');
            if (grandtotal) {
                hdnGrandtotal.value = grandtotal.toFixed(2);
                txtGrandTot.value = grandtotal.toFixed(2);
            }

        }

    </script>

    <script>
        function deleteRow(btn) {
            if (confirm('Are you sure you want to delete this row?')) {
                // Find the row and remove it
                var row = btn.closest('tr');
                row.parentNode.removeChild(row);
            }
        }
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnablePageMethods="true"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-md-10">
                        <h4 class="mt-4">&nbsp <b>PURCHASE BILL ENTRY</b></h4>
                    </div>
                    <div class="col-md-2 mt-4">
                        <asp:LinkButton ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="Button1_Click">
    <i class="fas fa-file-alt"></i>List
                        </asp:LinkButton>
                    </div>
                </div>
                <br />
                <div class="container-fluid px-3">
                    <div class="card mb-4">
                        <asp:HiddenField ID="hdnBillNo" runat="server" />
                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label4" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Supplier Name : </asp:Label>
                                    <asp:HiddenField runat="server" ID="hdnpbno" />
                                    <asp:TextBox ID="txtsupliername" CssClass=" uppercase  form-control" AutoPostBack="true" OnTextChanged="txtsupliername_TextChanged" runat="server"></asp:TextBox>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender2" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList" TargetControlID="txtsupliername" runat="server">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label3" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Bill No.: </asp:Label>
                                    <asp:HiddenField runat="server" ID="hdnfileData" />
                                    <asp:HiddenField runat="server" ID="hdnGrandtotal" />
                                    <asp:TextBox runat="server" ID="txtBillNo" CssClass="form-control" ReadOnly="true"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Supplier Bill No. : </asp:Label>
                                    <asp:TextBox ID="txtSupplierBillNo" CssClass="form-control" runat="server" AutoPostBack="true" OnTextChanged="txtSupplierBillNo_TextChanged" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Bill Date : </asp:Label>
                                    <asp:TextBox ID="txtBilldate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label runat="server" ID="Label18" Font-Bold="true" CssClass="form-label LblStyle">Bill Against :</asp:Label>
                                    <asp:DropDownList ID="ddlBillAgainst" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlBillAgainst_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Text="--Select--"></asp:ListItem>
                                        <asp:ListItem Text="Verbal"></asp:ListItem>
                                        <asp:ListItem Text="Order"></asp:ListItem>
                                        <%--      <asp:ListItem Text="Cash"></asp:ListItem>--%>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Select Bill Against"
                                        ControlToValidate="ddlBillAgainst" InitialValue="0" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <div class="row">
                                        <div class="col-md-6 col-12 mb-3">
                                            <asp:Label runat="server" ID="lblponumber" Font-Bold="true" CssClass="form-label LblStyle">Against No.</asp:Label>
                                            <asp:DropDownList runat="server" ID="ddlAgainstNumber" class="form-control" OnSelectedIndexChanged="ddlBindInwardChnage" AutoPostBack="true"></asp:DropDownList>
                                            <%--OnSelectedIndexChanged="ddlAgainstNumber_SelectedIndexChanged" AutoPostBack="true" --%>
                                        </div>
                                        <div class="col-md-6 col-12 mb-3">
                                            <asp:Label runat="server" ID="lblInwardNo" Font-Bold="true" CssClass="form-label LblStyle">Inward No.</asp:Label>
                                            <asp:DropDownList runat="server" ID="ddinwardAgainstNumber" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlAgainstNumber_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label6" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Transport Mode: </asp:Label>
                                    <asp:TextBox ID="txtTransportMode" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label7" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Vehicle Number: </asp:Label>
                                    <asp:TextBox ID="txtVehicleNumber" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="true" CssClass="form-label LblStyle"> Payment Due Date  : </asp:Label>
                                    <asp:TextBox ID="txtPaymentDueDate" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label9" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Account Head : </asp:Label>

                                    <asp:TextBox ID="txtAccontHead" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label10" runat="server" Font-Bold="true" CssClass="form-label LblStyle">E-Bill Number: </asp:Label>
                                    <asp:TextBox ID="txtEBillNumber" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label11" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Remark : </asp:Label>

                                    <asp:TextBox ID="txtRemark" CssClass="form-control" runat="server" Width="100%" TextMode="MultiLine"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label12" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Ref. Documents : </asp:Label>
                                    <asp:FileUpload runat="server" ID="UploadRefDocs" CssClass="form-control" />
                                    <span runat="server" id="spnFileUploadData" style="color: red;"></span>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label13" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Date Of Received: </asp:Label>
                                    <asp:TextBox ID="txtDOR" CssClass="form-control" TextMode="Date" runat="server" Width="100%" AutoComplete="off" OnTextChanged="txtDOR_TextChanged" AutoPostBack="true"></asp:TextBox>


                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-2">
                                    <asp:CheckBox runat="server" ID="IsSedndMail" />
                                </div>
                                <div class="col-md-8">
                                    <asp:Label runat="server" Font-Bold="true" ID="lblEmailID"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive text-center">
                            <asp:GridView ID="dgvTaxinvoiceDetails" runat="server" CellPadding="4" DataKeyNames="id" Width="100%" CssClass="table table-striped table-bordered"
                                OnRowDataBound="dgvTaxinvoiceDetails_RowDataBound" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField ItemStyle-Width="20">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAll" runat="server" onclick="toggleAll(this);" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkRow" runat="server" Checked='<%# Eval("IsSelected") == DBNull.Value ? false : Convert.ToBoolean(Eval("IsSelected")) %>' AutoPostBack="false" />

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                            <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--  --%>
                                    <asp:TemplateField HeaderText="Component" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtProduct" TextMode="MultiLine" onkeyup="GetComponent(this.value)" placeholder="Search company..." runat="server" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Particulars") %>'></asp:TextBox>
                                            <input type="hidden" id="ComponentId" />
                                            <div id="suggestion-list" class="suggestion-list" style="display: none; border: 1px solid #ccc; max-height: 200px; overflow-y: auto;">
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtDescription" TextMode="MultiLine" runat="server" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Description") %>'></asp:TextBox>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtHSN" runat="server" Style="text-align: center" CssClass="form-control" Text='<%# Eval("HSN") %>'></asp:TextBox>
                                            <asp:RequiredFieldValidator
                                                ID="rfvHSN"
                                                runat="server"
                                                ControlToValidate="txtHSN"
                                                ErrorMessage="HSN is required"
                                                ForeColor="Red"
                                                Display="Dynamic"
                                                ValidationGroup="1" />
                                            <asp:RegularExpressionValidator
                                                ID="revHSN"
                                                runat="server"
                                                ControlToValidate="txtHSN"
                                                ValidationExpression="^\d{4,8}$"
                                                ErrorMessage="Enter valid HSN (4-8 digits)"
                                                ForeColor="Red"
                                                Display="Dynamic"
                                                ValidationGroup="1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQuantity" runat="server" oninput="calculateRowTotal(this)" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Qty") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Unit" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtunit" runat="server" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Units") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtRate" runat="server" oninput="calculateRowTotal(this)" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Rate") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CGST(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtCGST" runat="server" oninput="calculateRowTotal(this)" Style="text-align: center" CssClass="form-control" Text='<%# Eval("CGSTPer") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="SGST(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtSGST" runat="server" oninput="calculateRowTotal(this)" Style="text-align: center" CssClass="form-control" Text='<%# Eval("SGSTPer") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="IGST(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtIGST" runat="server" oninput="calculateRowTotal(this)" Style="text-align: center" CssClass="form-control" Text='<%# Eval("IGSTPer") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Discount(%)" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtDiscount" runat="server" oninput="calculateRowTotal(this)" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Discount") %>'></asp:TextBox>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Batch No." ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtBatchno" Text='<%# Eval("Batchno") %>' Style="text-align: center" CssClass="form-control" runat="server"></asp:TextBox>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="60" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <button type="button" class="btn btn-danger btn-sm" onclick="deleteRow(this)">
                                                <i class="fas fa-trash-alt"></i>
                                            </button>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                        <div class="row">
                            <div class="col-md-12" style="text-align: center">
                                <asp:Button ID="btnAdd" runat="server" Text="Add Row" OnClick="btnAdd_Click" CssClass="btn btn-primary" />
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="col-md-2"></div>
                                <center>
                                    <div class="col-md-8">
                                        <div class="col-md-4"><b>Sum of Material Amount :</b></div>
                                        <div class="col-md-4">
                                            <asp:TextBox ID="sumofAmount" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                            <asp:HiddenField ID="hdnProductAmt" runat="server" />
                                        </div>
                                    </div>
                                </center>
                                <div class="col-md-2"></div>
                            </div>
                        </div>
                        <br />

                        <div class="table-responsive">
                            <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                    <td style="width: 50%;">Charges Description</td>
                                    <td>HSN</td>
                                    <td>Rate(%)</td>
                                    <td>Basic</td>
                                    <td>CGST</td>
                                    <td>SGST</td>
                                    <td>IGST</td>
                                    <td>Cost</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtDescription" Width="250px" runat="server" TextMode="MultiLine" Text="Freight"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtHSN" Width="100px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtRate" Width="100px" runat="server" oninput="calculateFright(this)"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtBasic" Width="100px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="CGSTPer" Width="50px" runat="server" oninput="calculateFright(this)"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="SGSTPer" Width="50px" runat="server" oninput="calculateFright(this)"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="IGSTPer" Width="50px" runat="server" oninput="calculateFright(this)"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtCost" Width="100px" runat="server"></asp:TextBox>
                                        <asp:HiddenField ID="hdnFCost" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <div class="table-responsive">
                            <table class="table" border="1" style="width: 100%; border: 1px solid #0c7d38;">
                                <tr style="background-color: #7ad2d4; color: #000; font-weight: 600; text-align: center;">
                                    <td style="width: 15%;">Transportation Charges</td>
                                    <td>CGST(%)</td>
                                    <td>SGST(%)</td>
                                    <td>IGST(%)</td>
                                    <td>Total Cost</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtTCharge" Width="250px" runat="server" oninput="calculateTransport(this)"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTCGSTPer" Width="50px" runat="server" oninput="calculateTransport(this)"></asp:TextBox>
                                        <asp:TextBox ID="txtTCGSTamt" Width="100px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTSGSTPer" Width="50px" runat="server" oninput="calculateTransport(this)"></asp:TextBox>
                                        <asp:TextBox ID="txtTSGSTamt" Width="100px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTIGSTPer" Width="50px" runat="server" oninput="calculateTransport(this)"></asp:TextBox>
                                        <asp:TextBox ID="txtTIGSTamt" Width="100px" runat="server"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtTCost" Width="100px" runat="server"></asp:TextBox>
                                        <asp:HiddenField ID="hdnTCost" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <br />
                        <div class="col-md-12">
                            <center>
                                <div class="col-md-4">
                                    <div class="row">
                                        <div class="col-md-4">
                                            TCS (%)
                                            <asp:DropDownList runat="server" CssClass="form-control" ID="txtTCSPer" oninput="calculateTCS(this)">

                                                <asp:ListItem Value="">--SELECT--</asp:ListItem>
                                                <asp:ListItem Value="0.0888">0.0888</asp:ListItem>
                                                <asp:ListItem Value="0.118">0.118</asp:ListItem>
                                                <asp:ListItem Value="0.65">0.65</asp:ListItem>
                                                <asp:ListItem Value="0.75">0.75</asp:ListItem>
                                                <asp:ListItem Value="0.9">0.9</asp:ListItem>
                                                <asp:ListItem Value="0.6">0.6</asp:ListItem>
                                                <asp:ListItem Value="0.1">0.1</asp:ListItem>
                                                <asp:ListItem Value="0.08">0.08</asp:ListItem>
                                                <asp:ListItem Value="0">0</asp:ListItem>
                                                <asp:ListItem Value="1">1</asp:ListItem>
                                                <asp:ListItem Value="20">20</asp:ListItem>

                                            </asp:DropDownList>

                                            <%-- <asp:TextBox runat="server" ID="txtTCSPer" CssClass="form-control" placeholder="TCS (%)" Text="0" OnTextChanged="txtTCSPer_TextChanged" AutoPostBack="true"></asp:TextBox>--%>
                                        </div>
                                        <div class="col-md-8">
                                            TCS Amt<asp:TextBox ID="txtTCSAmt" CssClass="form-control" runat="server" Width="100%" placeholder="TCS amount"></asp:TextBox>
                                            <asp:HiddenField ID="hdnTCSAmount" runat="server" />

                                        </div>
                                    </div>
                                </div>
                            </center>
                            <center>
                                <div class="col-md-8">
                                    <div class="col-md-4"><b>Grand Total :</b></div>
                                    <div class="col-md-4">
                                        <asp:TextBox ID="txtGrandTot" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                    </div>
                                </div>
                            </center>
                        </div>


                        <br />
                    </div>
                </div>

                <br />
                <div class="row">
                    <div class="col-md-4"></div>
                    <div class="col-6 col-md-2">
                        <asp:Button ID="btnsave" ValidationGroup="1" OnClick="btnsave_Click" CssClass="form-control btn btn-outline-success m-2" runat="server" Text="Save" />
                    </div>
                    <div class="col-6 col-md-2">
                        <asp:Button ID="btncancel" OnClick="btncancel_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                    </div>
                    <div class="col-md-4"></div>
                </div>
            </div>
            </div>
                <asp:HiddenField ID="hhd" runat="server" />
            </div>
            </div>
           
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnsave" />
            <asp:PostBackTrigger ControlID="btncancel" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

