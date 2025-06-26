<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>




<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Login - Pune Abrasives PVT LTD.</title>

    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0, minimal-ui" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <!-- Google font-->
    <link href="https://fonts.googleapis.com/css?family=Open+Sans:400,600,800" rel="stylesheet" />
    <!-- Required Fremwork -->
    <link rel="stylesheet" type="text/css" href="..\Content\bower_components\bootstrap\css\bootstrap.min.css" />
    <!-- themify-icons line icon -->
    <link rel="stylesheet" type="text/css" href="..\Content\assets\icon\themify-icons\themify-icons.css" />
    <!-- ico font -->
    <link rel="stylesheet" type="text/css" href="..\Content\assets\icon\icofont\css\icofont.css" />
    <!-- Style.css -->
    <link rel="stylesheet" type="text/css" href="..\Content\assets\css\style.css" />
    <style>
        .login-block {
            /*background:none;*/
            /*background-color: #808080 !important;*/
        }

        .multiclr {
            background-image: url('/Content/img/CRM_Background.jpeg');
            background-position: center;
            background-size: cover;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />
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
                window.location.href = "Login.aspx";
            })
        };
    </script>
</head>
<body>
    <form id="form1" runat="server">

        <section class="login-block multiclr">
            <!-- Container-fluid starts -->
            <div class="container">
                <div class="row">
                    <%-- <div class="col-md-3"></div>
                    <div class="col-md-3"></div>--%>
                    <div class="col-md-12">
                        <!-- Authentication card start -->

                        <div class="md-float-material form-material">
                            <%-- <div class="text-center">
                                 <img src="img/logoWeb.png" alt="logo" width="150px" />
                            </div>--%>
                            <div class="auth-box card" style="max-width: 385px;">
                                <div class="card-block">
                                    <div class="row m-b-20">
                                        <div class="col-md-12">
                                            <h3 class="text-center">Sign In</h3>
                                        </div>
                                    </div>
                                    <br />
                                    <div class="form-group form-primary">
                                        <asp:TextBox ID="txtUsername" runat="server" class="form-control" placeholder="Your Email Address"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="Dynamic" ErrorMessage="Please Enter Your Email Address"
                                            ControlToValidate="txtUsername" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <span class="form-bar"></span>
                                    </div>
                                    <div class="form-group form-primary">
                                        <asp:TextBox ID="txtPassword" runat="server" class="form-control" placeholder="Password" TextMode="Password"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Enter Your Password"
                                            ControlToValidate="txtPassword" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <span class="form-bar"></span>
                                    </div>
                                    <asp:Label ID="lblmsg" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                    <div class="row m-t-25 text-left">
                                        <div class="col-12">
                                            <div class="checkbox-fade fade-in-primary d-">
                                                <label>
                                                    <asp:CheckBox ID="chkremember" runat="server" />
                                                    <span class="cr"><i class="cr-icon icofont icofont-ui-check txt-primary"></i></span>
                                                    <span class="text-inverse">Remember me</span>
                                                </label>
                                            </div>
                                            <div class="forgot-phone text-right f-right">
                                              <%--  <a href="#" class="text-right f-w-600">Forgot Password?</a>--%>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row m-t-20">
                                        <div class="col-md-12">
                                            <asp:Button ID="btnsave" OnClick="btnsave_Click" runat="server" Text="Sign in" ValidationGroup="form1" CssClass="btn btn-primary btn-md btn-block waves-effect waves-light text-center m-b-2" />
                                            <%--<button type="button" >Sign in</button>--%>
                                        </div>
                                    </div>
                                    <hr />
                                    <div class="row">
                                        <div class="col-md-4">
                                            <p class="text-inverse text-left m-b-0">Thank you!</p>
                                            <p class="text-inverse text-left"><a href="#" target="_blank"><b class="f-w-600">Back to website</b></a></p>
                                        </div>
                                        <div class="col-md-8">
                                            <a href="#" target="_blank">
                                                <img src="Content/img/Pune Abrasives_logo.png" alt="logo" width="200px" height="40px" />
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- end of form -->
                    </div>
                    <!-- end of col-sm-12 -->
                </div>
                <!-- end of row -->
            </div>
            <!-- end of container-fluid -->
        </section>

        <!-- Required Jquery -->
        <script type="text/javascript" src="..\Content\bower_components\jquery\js\jquery.min.js"></script>
        <script type="text/javascript" src="..\Content\bower_components\bootstrap\js\bootstrap.min.js"></script>

    </form>
</body>
</html>

