<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="VendorMaster.aspx.cs" Inherits="Admin_VendorMaster" %>

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
                            <h4 class="mt-4">&nbsp <b>SUPPLIER MASTER</b></h4>
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
                                    <label for="lblvendorname" class="form-label LblStyle"><span class="spncls">*</span>Suplier Name :</label>
                                    <asp:TextBox ID="txtvendorname" CssClass="form-control" placeholder="Enter Suplier Name" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator15" ValidationGroup="invoice" runat="server" ControlToValidate="txtvendorname"
                                        ForeColor="Red" ErrorMessage="* Please Enter Suplier Name"></asp:RequiredFieldValidator>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblvendorcode" class="form-label LblStyle"><span class="spncls">*</span>Supplier Code :</label>
                                    <asp:TextBox ID="txtvendorcode" CssClass="form-control" ReadOnly="true" ForeColor="red" placeholder="Enter Suplier Code" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="invoice" runat="server" ControlToValidate="txtvendorcode"
                                        ForeColor="Red" ErrorMessage="* Please Enter Suplier Code"></asp:RequiredFieldValidator>

                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblownename" class="form-label LblStyle">Owner Name : </label>
                                    <asp:TextBox ID="txtownername" CssClass="form-control" placeholder="Enter Owner Name" runat="server"></asp:TextBox>
                                    <%--                     <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="invoice" runat="server" ControlToValidate="txtownername"
                                        ForeColor="Red" ErrorMessage="* Please Enter owner name"></asp:RequiredFieldValidator>--%>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblemailid" class="form-label LblStyle">Email ID : </label>
                                    <asp:TextBox ID="txtemailid" CssClass="form-control" TextMode="Email" placeholder="Enter Email ID" runat="server"></asp:TextBox>
                                    <%--   <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="invoice" runat="server" ControlToValidate="txtemailid"
                                        ForeColor="Red" ErrorMessage="* Please Enter Email ID"></asp:RequiredFieldValidator>--%>
                                </div>

                            </div>

                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblgstno" class="form-label LblStyle">GST No : </label>
                                    <asp:TextBox ID="txtgstno" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtgstno_TextChanged" placeholder="Enter GST No." runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="revGSTNumber" runat="server"
                                        ControlToValidate="txtgstno"
                                        ValidationExpression="^\d{2}[A-Z]{5}\d{4}[A-Z]{1}\d[Z]{1}[A-Z\d]{1}$"
                                        ErrorMessage="Invalid GST Number. GST number should be in the format 27ATFPS1959J1Z4"
                                        Display="Dynamic"
                                        ForeColor="Red" />
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblmobileno" class="form-label LblStyle">Mobile No. :</label>
                                    <asp:TextBox ID="txtmobileno" CssClass="form-control" placeholder="Mobile No." MaxLength="12" MinLength="10" onkeypress="return isNumberKey(event)" runat="server"></asp:TextBox>


                                </div>

                            </div>
                            <hr />
                            <div class="row">
                                <label for="lbladdres" class="form-label LblStyle"><span class="spncls"></span>Address :</label>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lbladdresLINE1" class="form-label LblStyle"><span class="spncls">*</span>Line 1 :</label>
                                    <asp:TextBox ID="txtaddressline1" CssClass="form-control" placeholder="Enter Address" TextMode="MultiLine" runat="server"></asp:TextBox>
                                   
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lbladdresLINE2" class="form-label LblStyle"><span class="spncls"></span>Line 2 :</label>
                                    <asp:TextBox ID="txtaddresLINE2" CssClass="form-control" placeholder="Enter Address" TextMode="MultiLine" runat="server"></asp:TextBox>

                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblCity" class="form-label LblStyle"><span class="spncls"></span>City :</label>
                                    <asp:TextBox ID="txtcity" CssClass="form-control" placeholder="Enter City" TextMode="MultiLine" runat="server"></asp:TextBox>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblState" class="form-label LblStyle"><span class="spncls"></span>State. : </label>
                                    <asp:DropDownList ID="ddlStatecode" CssClass="form-control" runat="server">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblPincode" class="form-label LblStyle">PinCode. :</label>
                                    <asp:TextBox ID="txtPincode" CssClass="form-control" placeholder="Enter Pincode" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server"
                                        ControlToValidate="txtPincode"
                                        ValidationExpression="^\d{6}$"
                                        Display="Dynamic"
                                        ErrorMessage="Invalid PIN Code. PIN code should be a 6-digit number."
                                        ForeColor="Red" />
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblPanno" class="form-label LblStyle">Pan No. : </label>
                                    <asp:TextBox ID="txtPanno" CssClass="form-control" placeholder="Pan No." runat="server"></asp:TextBox>
                                   
                                </div>

                            </div>
                            <div class="row">

                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblPaymentTerm" class="form-label LblStyle">Payment Term(In days) :</label>
                                    <asp:TextBox ID="txtPaymentTerm" CssClass="form-control" placeholder="Enter Payment Term" onkeypress="return isNumberKey(event)" MaxLength="3" MinLength="1" runat="server"></asp:TextBox>

                                </div>
                                <div class="col-md-5 col-12 mb-3">
                                    <label for="lbllocation" class="form-label LblStyle">Location : </label>
                                    <asp:TextBox ID="txtlocation" CssClass="form-control" placeholder="Enter Location" runat="server"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">

                                <div class="col-md-6 mb-3">
                                    <label for="pwd" class="form-label LblStyle">Status :</label><br />
                                    <asp:CheckBox ID="chkisactive" runat="server" />&nbsp; Active
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

