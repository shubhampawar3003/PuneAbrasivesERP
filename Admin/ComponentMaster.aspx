<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="ComponentMaster.aspx.cs" Inherits="Admin_ComponentMaster" %>

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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="container-fluid px-3">
                    <div class="row">
                        <div class="col-md-10">
                            <h4 class="mt-4">&nbsp <b>COMPONENT MASTER</b></h4>
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
                                    <label for="lblComponentname" class="form-label LblStyle"><span class="spncls">*</span>Component Name : </label>
                                    <asp:TextBox ID="txtComponentname" CssClass="form-control" placeholder="Enter Component Name" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="1" runat="server" ErrorMessage="Please fill Component Name" ControlToValidate="txtComponentname" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblComponentcode" class="form-label LblStyle"><span class="spncls">*</span>Component Code :</label>
                                    <asp:TextBox ID="txtComponentcode" CssClass="form-control" ReadOnly="true" ForeColor="red" placeholder="Enter Component Code" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblrate" class="form-label LblStyle"><span class="spncls">*</span>HSN/SAC Code  : </label>
                                    <asp:TextBox ID="txtHSN" CssClass="form-control" MaxLength="8" placeholder="HSN/SAC" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" ValidationGroup="1" runat="server" ErrorMessage="Please Fill HSN/SAC" ControlToValidate="txtHSN" ForeColor="Red"></asp:RequiredFieldValidator>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblUnit" class="form-label LblStyle">Unit  : </label>
                                    <%--  <asp:TextBox ID="txtUnit" CssClass="form-control" placeholder="Unit" runat="server"></asp:TextBox>--%>
                                    <asp:DropDownList ID="ddlUnit" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="-- Select Unit --" Text="-- Select Unit --"></asp:ListItem>
                                        <asp:ListItem Value="Kg" Text="Kg"></asp:ListItem>
                                        <asp:ListItem Value="Ton" Text="Ton"></asp:ListItem>
                                         <asp:ListItem Value="NOS" Text="NOS"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3" id="divprice" runat="server" visible="false">
                                    <label for="lblPrice" class="form-label LblStyle"><span class="spncls">*</span>Price : </label>
                                    <asp:TextBox ID="txtPrice" CssClass="form-control" TextMode="Number" placeholder="Enter Price" runat="server"></asp:TextBox>
                                    <%--   <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="1" runat="server" ErrorMessage="Please fill Price" ControlToValidate="txtPrice" ForeColor="Red"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblQtylimit" class="form-label LblStyle"><span class="spncls">*</span>Less Quantity limit : </label>
                                    <asp:TextBox ID="txtQtylimit" CssClass="form-control" placeholder="Enter Quantity limit " runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="1" runat="server" ErrorMessage="Please fill Quantity limit " ControlToValidate="txtQtylimit" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblQtylimit" class="form-label LblStyle"><span class="spncls">*</span>Grade : </label>
                                    <asp:TextBox ID="txtGarde" CssClass="form-control" placeholder="Enter grade " runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="1" runat="server" ErrorMessage="Please fill Grade" ControlToValidate="txtGarde" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="row" id="divpayment" runat="server" visible="false">
                                    <div class="col-md-6 col-12 mb-3">
                                        <label for="lblPaymentTerm" class="form-label LblStyle">Payment Term(In days) :</label>
                                        <asp:TextBox ID="txtPaymentTerm" CssClass="form-control" placeholder="Enter Payment Term" TextMode="Number" runat="server"></asp:TextBox>
                                    </div>
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
                                    <asp:Button ID="btnsave" OnClick="btnsave_Click" ValidationGroup="1" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
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

