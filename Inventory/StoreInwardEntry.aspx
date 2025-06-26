<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="StoreInwardEntry.aspx.cs" Inherits="Admin_StoreInwardEntry" %>

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
                    url: 'StoreInwardEntry.aspx/GetComponent', // Corrected method name
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
                        <h4 class="mt-4">&nbsp <b>PENDING INWARD ENTRY</b></h4>
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

                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3" id="divorderno" runat="server" visible="false">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Order No. : </asp:Label>
                                    <asp:HiddenField ID="hdnOrderNo" runat="server" />
                                    <asp:TextBox ID="txtorderno" ReadOnly="true" CssClass=" uppercase  form-control" Width="100%" runat="server" onkeypress="return isNumberKey(event)"></asp:TextBox>


                                </div>
                                <div class="col-md-6 col-12 mb-3" id="divcdate" runat="server">
                                    <asp:Label ID="Label17" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Created Date.: </asp:Label>

                                    <asp:TextBox ID="txtcreateddate" CssClass="form-control" TextMode="Date" runat="server" Width="100%"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label2" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice No. :</asp:Label>

                                    <asp:TextBox ID="txtinvoiceno" AutoComplete="off" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label3" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Invoice Date.: </asp:Label>

                                    <asp:TextBox ID="txtinvoicedate" CssClass="form-control" TextMode="Date" runat="server" Width="100%"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label4" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Supplier Name : </asp:Label>

                                    <asp:TextBox ID="txtsupliername" CssClass=" uppercase  form-control" AutoPostBack="true" OnTextChanged="txtsupliername_TextChanged" runat="server"></asp:TextBox>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender2" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetSupplierList" TargetControlID="txtsupliername" runat="server">
                                    </asp:AutoCompleteExtender>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label5" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Mobile No. : </asp:Label>
                                    <asp:TextBox ID="txtmobileno" CssClass="form-control" runat="server" MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label runat="server" ID="Label18" Font-Bold="true" CssClass="form-label LblStyle">Purchase Type:</asp:Label>
                                    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="Direct">Direct</asp:ListItem>
                                        <asp:ListItem Value="Order">Order</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label runat="server" ID="lblponumber" Font-Bold="true" CssClass="form-label LblStyle">Purchase Order No.</asp:Label>
                                    <asp:DropDownList ID="ddlponumber" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlponumber_SelectedIndexChanged">
                                        <asp:ListItem Selected="True" Value="0">--- select ---</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                            </div>
                            <div class="row">

                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label6" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Address : </asp:Label>

                                    <asp:TextBox ID="txtSuplieraddress" CssClass="form-control" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label7" runat="server" Font-Bold="true" CssClass="form-label LblStyle">State : </asp:Label>
                                    <asp:TextBox ID="txtState" AutoComplete="off" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label8" runat="server" Font-Bold="true" CssClass="form-label LblStyle"> GST No.  : </asp:Label>

                                    <asp:TextBox ID="txtGSTNO" AutoComplete="off" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label9" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Pan No  : </asp:Label>

                                    <asp:TextBox ID="txtPanno" CssClass="form-control" runat="server"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label10" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Challan No.: </asp:Label>

                                    <asp:TextBox ID="txtchallanno" AutoComplete="off" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label11" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Challan Date : </asp:Label>

                                    <asp:TextBox ID="txtchallandate" CssClass="form-control" runat="server" TextMode="Date" Width="100%"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label12" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Transport Name : </asp:Label>
                                    <asp:TextBox ID="txttransportname" CssClass="form-control" Width="100%" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label13" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Inward Time : </asp:Label>

                                    <asp:TextBox ID="txtinwardtime" CssClass="form-control" Width="100%" runat="server"></asp:TextBox>
                                    <asp:MaskedEditExtender ID="MaskedEditExtender1" TargetControlID="txtinwardtime" Mask="99:99" MaskType="Time" AcceptAMPM="true" MessageValidatorTip="true" runat="server">
                                    </asp:MaskedEditExtender>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label14" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Material Receive By : </asp:Label>

                                    <asp:TextBox ID="txtmaterialrecivedby" CssClass=" uppercase  form-control" ReadOnly="true" Width="100%" runat="server"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label15" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Vehicle No: </asp:Label>

                                    <asp:TextBox ID="txtvehicleno" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label16" runat="server" Font-Bold="true" CssClass="form-label LblStyle">Material Description: </asp:Label>

                                    <asp:TextBox ID="txtmaterialdescription" CssClass=" uppercase  form-control" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>

                            </div>

                        </div>

                        <div class="table-responsive text-center">
                            <asp:GridView ID="dgvTaxinvoiceDetails" runat="server" CellPadding="4" DataKeyNames="id" Width="100%" CssClass="table table-striped table-bordered"
                                OnRowDataBound="dgvTaxinvoiceDetails_RowDataBound" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField ItemStyle-Width="20">
                                        <HeaderTemplate>
                                            <asp:CheckBox ID="chkAll" runat="server" onclick="toggleAll(this);" AutoPostBack="false" />
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
                                    <asp:TemplateField HeaderText="Total Qty" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtTotQuantity" runat="server" ReadOnly="true" Style="text-align: center" CssClass="form-control" Text='<%# Eval("TotalQty") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remaining Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtQuantity" runat="server" ReadOnly="true" Style="text-align: center" CssClass="form-control" Text='<%# Eval("Qty") %>'></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="In Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtInQuantity" runat="server" Style="text-align: center" CssClass="form-control" Text='<%# Eval("InQty") %>'></asp:TextBox>
                                            <asp:RequiredFieldValidator
                                                ID="rfvInQty"
                                                runat="server"
                                                ControlToValidate="txtInQuantity"
                                                ErrorMessage="In Quantity is required"
                                                ForeColor="Red"
                                                Display="Dynamic"
                                                ValidationGroup="1" />
                                            <asp:RegularExpressionValidator
                                                ID="revInQty"
                                                runat="server"
                                                ControlToValidate="txtInQuantity"
                                                ValidationExpression="^\d+(\.\d{1,2})?$"
                                                ErrorMessage="Enter a valid quantity (numbers only)"
                                                ForeColor="Red"
                                                Display="Dynamic"
                                                ValidationGroup="1" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Rate" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead" Visible="false">
                                        <ItemTemplate>

                                            <asp:Label runat="server" ID="lblrate" Text='<%# Eval("Rate") %>'></asp:Label>
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

