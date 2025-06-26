<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="ProductMaster.aspx.cs" Inherits="Admin_ProductMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/css/select2.min.css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script type="text/javascript" src="https://cdn.jsdelivr.net/npm/select2@4.0.13/dist/js/select2.min.js"></script>
    <script type="text/javascript">
        $(function () {
            $("[id*=ddlbrandname]").select2();

        });

    </script>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="container-fluid px-3">
                    <div class="row">
                        <div class="col-md-10">
                            <h4 class="mt-4">&nbsp <b>PRODUCT MASTER</b></h4>
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

                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblproductname" class="form-label LblStyle"><span class="spncls">*</span>Product Name : </label>
                                    <asp:TextBox ID="txtproductname" CssClass="form-control" placeholder="Enter Product Name" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please fill Product Name" ControlToValidate="txtproductname" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblproductcode" class="form-label LblStyle"><span class="spncls">*</span>Product Code :</label>
                                    <asp:TextBox ID="txtproductcode" CssClass="form-control" ReadOnly="true" ForeColor="red" placeholder="Enter Brand Code" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblPrice" class="form-label LblStyle"><span class="spncls">*</span>Price : </label>
                                    <asp:TextBox ID="txtPrice" CssClass="form-control" placeholder="Enter Price" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please fill Price" ControlToValidate="txtPrice" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3" id="divqty" runat="server" visible="false">
                                    <label for="lblQTY" class="form-label LblStyle"><span class="spncls">*</span>Quantity : </label>
                                    <asp:TextBox ID="txtQTY" CssClass="form-control" TextMode="Number" placeholder="Enter Quantity" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please fill Quantity" ControlToValidate="txtQTY" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>

                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblDescription" class="form-label LblStyle"><span class="spncls">*</span>Description : </label>
                                    <asp:TextBox ID="txtDescription" CssClass="form-control" placeholder="Enter Description" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please fill Description" ControlToValidate="txtDescription" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblrate" class="form-label LblStyle"><span class="spncls">*</span>Tariff/HSN  : </label>
                                    <asp:TextBox ID="txttariff" CssClass="form-control" placeholder="Tariff/HSN" MaxLength="8" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Please Fill txttariff" ControlToValidate="txttariff" ForeColor="Red"></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="RegExValidatorHSN" ValidationGroup="1" runat="server" ErrorMessage="HSN/SAC Code must be an 8-digit number" ControlToValidate="txttariff" ForeColor="Red" ValidationExpression="^\d{8}$"></asp:RegularExpressionValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblUnit" class="form-label LblStyle">Unit  : </label>
                                    <asp:DropDownList ID="ddlUnit" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="-- Select Unit --" Text="-- Select Unit --"></asp:ListItem>
                                        <asp:ListItem Value="KGS" Text="KGS"></asp:ListItem>
                                        <asp:ListItem Value="Ton" Text="Ton"></asp:ListItem>
                                    </asp:DropDownList>
                                    <%--      <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Please Fill Unit" ControlToValidate="txtUnit" ForeColor="Red" ValidationGroup="Check"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="row">
                                    <div class="col-md-6 mb-3">
                                        <label for="pwd" class="form-label LblStyle">Status :</label><br />
                                        <asp:CheckBox ID="chkisactive" runat="server" />&nbsp; Active
                                    </div>
                                </div>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btnsave" OnClick="btnsave_Click" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
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
    </asp:UpdatePanel>
</asp:Content>

