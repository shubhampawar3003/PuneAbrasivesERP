<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="UserMaster.aspx.cs" Inherits="Admin_UserMaster" %>


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
                            <h4 class="mt-4">&nbsp <b>USER MASTER</b></h4>
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
                                    <label for="lblusername" class="form-label LblStyle">User Name : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtusernmae" CssClass="form-control" placeholder="Enter User Name" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please fill User Name" ControlToValidate="txtusernmae" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblcompanycode" class="form-label LblStyle">User Code :</label>
                                    <asp:TextBox ID="txtusercode" CssClass="form-control" ReadOnly="true" ForeColor="red" placeholder="Enter User Code" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblmobileno" class="form-label LblStyle">Mobile No. : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtmobileno" CssClass="form-control" placeholder="Enter Mobile No." MaxLength="12" MinLength="10" onkeypress="return isNumberKey(event)" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" Display="Dynamic" ErrorMessage="Please Enter Contact No."
                                        ControlToValidate="txtmobileno" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblemailid" class="form-label LblStyle">Email ID : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtemailid" CssClass="form-control" placeholder="Enter Email ID" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please fill Email ID" ControlToValidate="txtemailid" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">

                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblpassword" class="form-label LblStyle">Software Password : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtpassword" CssClass="form-control" placeholder="Enter Software Password" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Please fill Password" ControlToValidate="txtpassword" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblpassword" class="form-label LblStyle">Email ID Password : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txtEmailIDPASS" CssClass="form-control" placeholder="Enter Email ID Password" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please fill Password" ControlToValidate="txtEmailIDPASS" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="role" class="form-label LblStyle">Designation : <span class="spncls">*</span></label>
                                    <asp:DropDownList ID="ddldesignation" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="0" Text="--Select Designation--"></asp:ListItem>
                                        <asp:ListItem Value="Accounts " Text="Accounts"></asp:ListItem>
                                        <asp:ListItem Value="M.D" Text="M.D"></asp:ListItem>
                                        <asp:ListItem Value="Sales Manager" Text="Sales Manager"></asp:ListItem>
                                        <asp:ListItem Value="Sales Engineer" Text="Sales Engineer "></asp:ListItem>
                                        <asp:ListItem Value="Logistics" Text="Logistics"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="role" class="form-label LblStyle">Role : <span class="spncls">*</span></label>
                                    <asp:DropDownList ID="ddlrole" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="0" Text="--Select Role--"></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please fill Role" ControlToValidate="ddlrole" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblpanno" class="form-label LblStyle">PAN No. :</label>
                                    <asp:TextBox ID="txtpanno" CssClass="form-control" placeholder="Enter PAN No." MaxLength="10" MinLength="10" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblaadharno" class="form-label LblStyle">Aadhar No. :</label>
                                    <asp:TextBox ID="txtaadharno" CssClass="form-control" placeholder="Enter Aadhar No." MaxLength="12" MinLength="12" onkeypress="return isNumberKey(event)" runat="server"></asp:TextBox>
                                </div>

                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3" id="accountdetails" runat="server" visible="false">
                                    <label for="lblaccountdetails" class="form-label LblStyle">Bank Account Details :</label>
                                    <asp:TextBox ID="txtaccountdetails" CssClass="form-control" placeholder="Enter Account Details" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lbladdres" class="form-label LblStyle">Address :</label>
                                    <asp:TextBox ID="txtaddres" CssClass="form-control" placeholder="Enter Address" TextMode="MultiLine" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 mb-3">
                                    <label for="lblbirthdate" class="form-label LblStyle">Birth Date :</label>
                                    <asp:TextBox ID="txtbirthdate" Placeholder="Enter From Date" runat="server" AutoComplete="off" TextMode="Date" CssClass="form-control"></asp:TextBox>
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
