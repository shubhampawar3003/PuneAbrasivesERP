<%@ Page Title="" EnableEventValidation="false" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="OutComponentEntry.aspx.cs" Inherits="Admin_OutComponentEntry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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

    <script type="text/javascript">
        var Component = "";
        var Batch = "";
        //Bind Component Dropdown
        $("[src*=plus]").live("click", function () {
            $.ajax({
                type: "POST",
                url: "OutComponentEntry.aspx/GetComponent",
                data: '{}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    var ddlComponent = $("[id*=ddlComponent]");
                    ddlComponent.empty().append('<option selected="selected" value="0">Select Component</option>');
                    $.each(r.d, function () {
                        ddlComponent.append($("<option></option>").val(this['Value']).html(this['Text']));
                        $("[id*=txtAviQuontity]").val("");
                        $("[id*=txtQuontity]").val("");
                    });
                }
            });

        });
        $(document).ready(function () {
            $('#ddlComponent').change(function () {
                BindBatch(this); // Pass the current dropdown element to the function
            });
        });

        //Bind Batch Dropdown Using Componant Dropdown Data
        function BindBatch(element) {
            var ddlComponent = $(element).val();
            document.getElementById('<%= hdncompo.ClientID %>').value = $(element).val();
            Component = ddlComponent;
            console.log('Selected Component:', ddlComponent);
            $.ajax({
                type: "POST",
                url: "OutComponentEntry.aspx/GetBatches",
                data: JSON.stringify({ "Component": ddlComponent }),
                dataType: "json",
                contentType: "application/json",
                success: function (res) {
                    var ddlBatch = $("[id*=ddlBatch]");
                    ddlBatch.empty().append('<option selected="selected" value="0">Select Batch</option>');
                    $.each(res.d, function () {
                        ddlBatch.append($("<option></option>").val(this['Value']).html(this['Text']));
                    });
                }

            });

        }


        $(document).ready(function () {
            $('#ddlBatch').change(function () {
                BindAviQuontity(this); // Pass the current dropdown element to the function
            });
        });

        //Bind BatchWise Avilable Quontity
        function BindAviQuontity(element) {
            var ddlBatch = $(element).val();
            console.log('Selected Batch:', ddlBatch);
            $.ajax({
                type: "POST",
                url: "OutComponentEntry.aspx/GetBatchesWiseQty",
                data: JSON.stringify({ "Batch": ddlBatch, "Component": Component }),
                dataType: "json",
                contentType: "application/json",
                success: function (res) {
                    var aviqty = res.d;  // Assuming res.d is a number

                    $("[id*=txtAviQuontity]").val(aviqty);

                    document.getElementById('<%= hdnQty.ClientID %>').value = res.d;
                    document.getElementById('<%= hdnbatch.ClientID %>').value = $(element).val();

                }

            });
            Component = "";
            Batch = "";
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="row">
                    <div class="col-md-10">
                        <h4 class="mt-4">&nbsp <b>WAREHOUSE OUT ENTRY</b></h4>
                    </div>
                    <div class="col-md-2 mt-4">
                        <asp:LinkButton ID="Button1" CssClass="form-control btn btn-warning" Font-Bold="true" CausesValidation="false" runat="server" OnClick="Button1_Click">
    <i class="fas fa-file-alt"></i>List
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="container-fluid px-3">
                    <div class="card mb-4">

                        <div class="card-body ">
                            <asp:HiddenField ID="hdnAVQty" runat="server" />
                            <asp:HiddenField ID="hdnQty" runat="server" />
                            <asp:HiddenField ID="hdnbatch" runat="server" />
                            <asp:HiddenField ID="hdncompo" runat="server" />
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblent" Font-Bold="true" runat="server" CssClass="form-label LblStyle">Challan No. :</asp:Label>
                                    <asp:TextBox ID="txtchallanno" AutoComplete="off" ReadOnly="true" CssClass=" uppercase  form-control" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblentrpe" Font-Bold="true" runat="server" CssClass="form-label LblStyle">Challan Date :</asp:Label>
                                    <asp:TextBox ID="txtchallandate" CssClass="form-control" runat="server" ReadOnly="true" TextMode="Date" Width="100%"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblenytype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Customer Name : </asp:Label>
                                    <asp:TextBox ID="txtbillingcustomer" CssClass="form-control" ReadOnly="true" runat="server" placeholder="Enter Customer Name " Width="100%"></asp:TextBox>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lbntrytype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Invoice Number:</asp:Label>
                                    <asp:TextBox ID="txtAgainstnumber" CssClass="form-control" ReadOnly="true" runat="server" Width="100%"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblentype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Invoice Date :</asp:Label>
                                    <asp:TextBox ID="txtInvoicedate" runat="server" ReadOnly="true" CssClass="form-control uppercase" Autocomplete="off"></asp:TextBox>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lbleytype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>P.O. No. :</asp:Label>
                                    <asp:TextBox ID="txtpono" runat="server" ReadOnly="true" CssClass="form-control uppercase" AutoPostBack="true"></asp:TextBox><br />

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblen" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>P.O. Date :</asp:Label>
                                    <asp:TextBox ID="txtpodate" runat="server" ReadOnly="true" TextMode="Date" CssClass="form-control uppercase"></asp:TextBox>

                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lbl152" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Payment Term : </asp:Label>
                                    <asp:TextBox ID="txtpaymentterm" runat="server" ReadOnly="true" CssClass="form-control uppercase"></asp:TextBox><br />
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lbl455type" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>E-Way Bill :</asp:Label>
                                    <asp:TextBox ID="txtewaybillno" runat="server" ReadOnly="true" CssClass="form-control uppercase"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblent48pe" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Bill To :</asp:Label>
                                    <asp:TextBox ID="txtbillito" Enabled="false" runat="server" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblent558type" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Ship To : </asp:Label>
                                    <asp:TextBox ID="txtShiftTo" AutoPostBack="true" runat="server" ReadOnly="true" CssClass="form-control uppercase"></asp:TextBox><br />
                                </div>
                            </div>
                            <asp:HiddenField ID="HiddenField1" runat="server" />
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblen45ype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Bill Address :</asp:Label>
                                    <asp:TextBox ID="txtaddress" Enabled="false" runat="server" ReadOnly="true" CssClass="form-control" TextMode="MultiLine" Height="75px"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblent58pe" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Ship Address:</asp:Label>
                                    <asp:TextBox ID="txtShiftAddress" Enabled="false" ReadOnly="true" runat="server" CssClass="form-control" TextMode="MultiLine" Height="75px"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lble879type" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>State Code :</asp:Label>
                                    <asp:TextBox ID="txtstatecode" Enabled="false" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblen897ype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Pan No. :</asp:Label>
                                    <asp:TextBox ID="txtpartypanno" Enabled="false" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lbl58type" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Email ID :</asp:Label>
                                    <asp:TextBox ID="txtemail" Enabled="false" runat="server" CssClass="form-control"></asp:TextBox><br />
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lblent878e" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>GST No. :</asp:Label>
                                    <asp:TextBox ID="txtpartygstno" Enabled="false" CssClass="form-control" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="lble897ytype" Font-Bold="true" runat="server" CssClass="form-label LblStyle"><span class="spncls">*</span>Vehicle No. :</asp:Label>
                                    <asp:TextBox ID="txtvehicleno" runat="server" ReadOnly="true" CssClass="form-control uppercase"></asp:TextBox><br />
                                </div>
                            </div>

                        </div>

                        <div class="table-responsive text-center">
                            <asp:GridView ID="dgvTaxinvoiceDetails" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="table table-striped table-bordered nowrap"
                                OnRowDataBound="dgvTaxinvoiceDetails_RowDataBound" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderStyle-Width="20" HeaderText=" " HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <img alt="" style="cursor: pointer" src="../Content/img/plus.png" />
                                            <asp:Panel ID="pnlOrders" runat="server" Style="display: none">
                                                <table class="table text-center align-middle table-bordered table-hover" border="1" style="width: 100%; border: 1px solid #0c7d38; column-span: all">
                                                    <thead>
                                                        <tr class="gvhead">
                                                            <td>Component</td>
                                                            <td>Batch</td>
                                                            <td>Available Quantity</td>
                                                            <td>Quantity</td>
                                                            <td>Action</td>



                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        <tr>
                                                            <td>
                                                                <asp:DropDownList ID="ddlComponent" Width="230px" onchange="BindBatch(this);" CssClass="form-control" runat="server"></asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <%-- <select id="ddlBatch" onchange="BindAviQuontity(this);">
                                                                    <option value="0">--Select Batch--</option>
                                                                </select>--%>
                                                                <asp:DropDownList ID="ddlBatch" Width="230px" onchange="BindAviQuontity(this);" CssClass="form-control" runat="server"></asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtAviQuontity" Style="text-align: center" ReadOnly="true" Width="230px" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtQuontity" Width="230px" TextMode="Number" Style="text-align: center" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <%--<input type="button" class="btn btn-primary btn-sm btncss" id="btnsave" value="Save" onclick="saveData();" />--%>
                                                                <asp:Button ID="btnSaveCompo" Autopostback="true" OnClick="btnSaveCompo_Click" CausesValidation="false" CssClass="btn btn-primary btn-sm btncss" runat="server" Text="Save" />
                                                            </td>
                                                        </tr>
                                                    </tbody>

                                                </table>
                                            </asp:Panel>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                            <asp:Label ID="lblid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblproduct" runat="Server" Text='<%# Eval("Particular") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Description" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDescription" runat="Server" Text='<%# Eval("Description") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="HSN" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblhsn" runat="Server" Text='<%# Eval("HSN") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblQuantity" runat="Server" Text='<%# Eval("Qty") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Batch No." ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txtBatchno" ValidationGroup="1" Text='<%# Eval("Batchno") %>' Style="text-align: center" CssClass="form-control" runat="server"></asp:TextBox>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                        <br />
                    </div>
                </div>
                <div class="table-responsive text-center">
                    <div class="row">
                        <div class="table-responsive text-center">
                            <asp:GridView ID="gvcomponent" runat="server" CellPadding="4" DataKeyNames="id" PageSize="10" AllowPaging="true" Width="100%" CssClass="grivdiv pagination-ys" OnRowEditing="gvcomponent_RowEditing"
                                AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr.No" ItemStyle-Width="20" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="lblComsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                            <asp:Label ID="lblComid" runat="Server" Text='<%# Eval("id") %>' Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Product" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <EditItemTemplate>
                                            <asp:TextBox Text='<%# Eval("Particular") %>' ReadOnly="true" CssClass="form-control" Width="230px" ID="txtCOMPParticular" runat="server"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblproduct" runat="Server" Text='<%# Eval("Particular") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Component" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <EditItemTemplate>
                                            <asp:TextBox Text='<%# Eval("ComponentName") %>' ReadOnly="true" CssClass="form-control" Width="230px" ID="txtCOMPComponent" runat="server"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblComComPonent" runat="Server" Text='<%# Eval("ComponentName") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Batch" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <EditItemTemplate>
                                            <asp:TextBox Text='<%# Eval("Batch") %>' CssClass="form-control" Width="230px" ID="txtCOMPBatch" runat="server"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblComBatch" runat="Server" Text='<%# Eval("Batch") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <EditItemTemplate>
                                            <asp:TextBox Text='<%# Eval("Quantity") %>' CssClass="form-control" ID="txtCOMPQuantity" Width="100px" runat="server"></asp:TextBox>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblComQuantity" runat="Server" Text='<%# Eval("Quantity") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="120" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <%--<asp:LinkButton ID="btn_edit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>--%>

                                            <asp:LinkButton ID="btn_Compedit" CausesValidation="false" Text="Edit" runat="server" CommandName="Edit"><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>

                                            <asp:LinkButton runat="server" ID="lnkbtnCompDelete" OnClick="lnkbtnCompDelete_Click" ToolTip="Delete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CausesValidation="false"><i class="fa fa-trash" style="font-size:24px"></i></asp:LinkButton>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:LinkButton ID="gv_Compupdate" OnClick="gv_Compupdate_Click" CausesValidation="false" Text="Update" CssClass="btn btn-primary btn-sm" runat="server"></asp:LinkButton>&nbsp;
                                                      
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-md-4"></div>
                    <div class="col-6 col-md-2">
                        <asp:Button ID="btnsave" ValidationGroup="1" OnClick="btnsave_Click" OnClientClick="showLoader();" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
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
                <div id="loader" class="loader-wrapper">
                    <div class="loader"></div>
                </div>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnsave" />
            <asp:PostBackTrigger ControlID="btncancel" />
            <asp:PostBackTrigger ControlID="gvcomponent" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

