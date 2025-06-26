<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Admin/WLSPLMaster.master" CodeFile="TargetMaster.aspx.cs" Inherits="Admin_TargetMaster" %>


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
                            <h4 class="mt-4">&nbsp <b>TARGET MASTER</b></h4>
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
                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblyaer" class="form-label LblStyle">Year : </label><br />
                                    <asp:DropDownList ID="ddlYear" Width="120px" Font-Bold="true" CssClass="form-control" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblMonth" class="form-label LblStyle">Month :</label>
                                    <asp:DropDownList ID="ddlMonth" Width="120px" Font-Bold="true" CssClass="form-control" runat="server">
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
                                    <label for="lblMonth" class="form-label LblStyle">Target Amount :</label>
                                    <asp:TextBox ID="txtTarget" CssClass="form-control" TextMode="Number" placeholder="Enter Target" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please fill Target Amount" ControlToValidate="txtTarget" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-3 col-12 mb-3">
                                    <label for="lblkgston" class="form-label LblStyle">Target Kg/Ton :</label>
                                    <asp:TextBox ID="txtkgston" CssClass="form-control" placeholder="Enter kgs/ton" runat="server"></asp:TextBox>

                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please fill Target Kg/Ton" ControlToValidate="txtkgston" ForeColor="Red"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3" id="rolecodeid" runat="server" visible="false">
                                    <label for="lblRolecode" class="form-label LblStyle">Target Code :</label>
                                    <asp:TextBox ID="txtTargetcode" CssClass="form-control" ReadOnly="true" ForeColor="red" placeholder="Enter Target Code" runat="server"></asp:TextBox>
                                </div>
                            </div>

                            <br />
                            <div class="row">
                                <div class="col-md-4"></div>
                                <div class="col-6 col-md-2">
                                    <asp:Button ID="btnsave" OnClick="btnsave_Click" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
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
