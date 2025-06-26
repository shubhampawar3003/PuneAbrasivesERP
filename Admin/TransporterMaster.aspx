<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="TransporterMaster.aspx.cs" Inherits="Admin_TransportorMastre" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
       <asp:ScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="container-fluid px-3">
                    <h4 class="mt-4 ">Transporter Master</h4>
                    <div class="card mb-4">
                        <div class="card-header LblStyle">
                            <i class="fas fa-user me-1"></i>
                            Transporter Master                          
                        </div>
                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblttransporter" class="form-label LblStyle">Transporter Name : <span class="spncls">*</span></label>
                                    <asp:TextBox ID="txttransporter" CssClass="form-control" placeholder="Enter Transporter Name" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please fill Transporter Name" ControlToValidate="txttransporter" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblmobileno" class="form-label LblStyle">Mobile No. :</label>
                                    <asp:TextBox ID="txtmobileno" CssClass="form-control"  placeholder="Enter Mobile No." runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblemail" class="form-label LblStyle">Email ID. : </label>
                                    <asp:TextBox ID="txtemail" CssClass="form-control" placeholder="Enter Email ID."  runat="server"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lbladdress" class="form-label LblStyle">Address : </label>
                                    <asp:TextBox ID="txtaddress" CssClass="form-control" placeholder="Enter Address" runat="server"></asp:TextBox>
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

