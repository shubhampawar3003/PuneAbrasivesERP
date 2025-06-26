<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="CallandMeetingUpdate.aspx.cs" Inherits="Admin_CallandMeetingUpdate" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <title>Your Page Title</title>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.all.min.js"></script>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/sweetalert2@10.10.1/dist/sweetalert2.min.css' />




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
            margin-top: 5px;
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
    </style>
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
            margin-top: 5px;
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
    <script type="text/javascript">
        function validateAudioFile(input) {
            var filePath = input.value;
            var allowedExtensions = /(\.mp3|\.wav|\.ogg)$/i;

            if (!allowedExtensions.exec(filePath)) {
                alert('Please upload a valid audio file (mp3, wav, or ogg).');
                input.value = '';
                return false;
            }
            return true;
            showLoader();
        }
    </script>

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
    <style>
        #loader {
            position: fixed;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            z-index: 9999;
            background: rgba(255, 255, 255, 0.8);
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .spinner {
            border: 16px solid #f3f3f3; /* Light grey */
            border-top: 16px solid #3498db; /* Blue */
            border-radius: 50%;
            width: 120px;
            height: 120px;
            animation: spin 2s linear infinite;
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="container-fluid px-4">
                <div class="container-fluid px-3">
                    <div class="row">
                        <div class="col-8 col-md-8">
                            <h4 class="mt-4 ">&nbsp <b>DSR UPDATE</b></h4>
                        </div>
                        <div class="col-2 col-md-2 mt-4">
                            <asp:Button ID="btnAddCompany" CssClass="form-control btn btn-info" Font-Bold="true" OnClick="btnAddCompany_Click" runat="server" Text="Add Company" />
                        </div>
                        <div class="col-md-2 mt-4">
                            <asp:LinkButton ID="LinkButton1" CssClass="form-control btn btn-warning" Font-Bold="true" OnClientClick="showLoader();" CausesValidation="false" runat="server" OnClick="LinkButton1_Click">
    <i class="fas fa-file-alt"></i>List
                            </asp:LinkButton>
                        </div>
                    </div>
                    <br />
                    <div class="card mb-4">
                        <div class="card-header LblStyle">
                            <i class="fa fa-cog"></i>
                            Company Details:                                             
                        </div>
                        <br />
                        <div class="card-body ">
                            <div class="row">
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:HiddenField ID="CompanycodeID" runat="server" />
                                    <label for="lblbrandname" class="form-label LblStyle">Company Name : <span class="spncls">*</span></label>
                                    <%--     <asp:DropDownList ID="ddlCompanyname" CssClass="form-control" OnSelectedIndexChanged="ddlCompanyname_SelectedIndexChanged" runat="server" AutoPostBack="true">
                                    </asp:DropDownList>--%>
                                    <asp:TextBox ID="txtcompanyname" OnTextChanged="txtcompanyname_TextChanged" AutoPostBack="true" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" Display="Dynamic" ErrorMessage="Please Enter Company Name"
                                        ControlToValidate="txtcompanyname" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    <asp:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server" CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted" CompletionListItemCssClass="listItem"
                                        CompletionInterval="10" MinimumPrefixLength="1" ServiceMethod="GetCompanyList"
                                        TargetControlID="txtcompanyname" Enabled="true">
                                    </asp:AutoCompleteExtender>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lbldat" class="form-label LblStyle">Date :</label>
                                    <asp:TextBox ID="txttodate" CssClass="form-control" TextMode="Date" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblownname" class="form-label LblStyle">Owner Name :</label>
                                    <asp:TextBox ID="txtownname" CssClass="form-control" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblcontactno" class="form-label LblStyle">Contact No. :</label>
                                    <asp:TextBox ID="txtcontactno" CssClass="form-control" onkeypress="return isNumberKey(event)" MaxLength="12" MinLength="10" runat="server" Width="100%"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblarea" class="form-label LblStyle">Area :</label>
                                    <asp:TextBox ID="txtarea" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" Display="Dynamic" ErrorMessage="Please Enter Area"
                                        ControlToValidate="txtarea" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lbladdress" class="form-label LblStyle">Billing Address :</label>
                                    <asp:TextBox ID="txtaddress" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <label for="lblfeedback" class="form-label LblStyle">Feedback :</label>
                                    <asp:TextBox ID="txtfeedback" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="Label14" runat="server" Font-Bold="true" CssClass="form-label">Audio File :</asp:Label>
                                    <asp:FileUpload ID="AudiofileUpload" runat="server" CssClass="form-control" onchange="validateAudioFile(this)" />
                                    <asp:Label ID="lblfile" runat="server" Font-Bold="true" ForeColor="blue" Text=""></asp:Label>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <div class="col-md-2" style="margin-top: 18px">
                                        <asp:Button ID="uploadfile" runat="server" OnClientClick="showLoader();" CausesValidation="false" AutoPostBack="true" Text="Upload" CssClass="form-control btn btn-outline-primary m-2" OnClick="uploadfile_Click" />
                                    </div>
                                </div>

                                <div class="col-md-6 col-12 mb-3" id="DivMailID" runat="server">


                                    <label for="lblmail" class="form-label LblStyle">Mail ID :</label>
                                    <asp:TextBox ID="txtmail" CssClass="form-control" runat="server"></asp:TextBox>
                                    <asp:CheckBox ID="CheckBox1" runat="server" />
                                    <asp:Label ID="lblnote" runat="server" Font-Bold="true" ForeColor="Red" Text="Note : Sending mail to customer please check that box."></asp:Label>

                                </div>
                                <div class="col-md-6 col-12 mb-3">
                                    <asp:Label ID="txtccode" runat="server" Text="Label" Visible="false"></asp:Label>
                                    <asp:Label ID="txtcid" runat="server" Text="Label" Visible="false"></asp:Label>
                                </div>
                            </div>
                        </div>


                    </div>
                </div>
                <asp:HiddenField ID="HiddenField1" runat="server" />
            </div>
            <div class="container-fluid px-4">
                <div class="container-fluid px-3">
                    <div class="card mb-4">
                        <div class="card-header LblStyle">
                            <i class="fa fa-cog"></i>
                            DSR Update                                              
                        </div>
                        <div class="card-body ">
                            <div class="col-md-12">
                                <div class="row">
                                    <div class="col-md-2 LblStyle">Type of Update<i class="reqcls">*&nbsp;</i> : </div>
                                    <div class="col-md-4">
                                        <asp:DropDownList ID="ddlupdatefor" OnTextChanged="ddlupdatefor_TextChanged" CssClass="form-control" runat="server" Width="100%" AutoPostBack="true">
                                            <asp:ListItem>--Select--</asp:ListItem>
                                            <asp:ListItem>Call</asp:ListItem>
                                            <asp:ListItem>Meeting</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="Please Select Follow-up Type"
                                            ControlToValidate="ddlupdatefor" InitialValue="Select" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                    </div>
                                    <br />

                                    <div class="col-md-2 LblStyle">Type<i class="reqcls">*&nbsp;</i> : </div>
                                    <div class="col-md-4">
                                        <asp:DropDownList ID="ddltype" CssClass="form-control" runat="server" Width="100%" AutoPostBack="true">
                                            <asp:ListItem Value="- Select Type -" Text="- Select Type -"></asp:ListItem>
                                            <asp:ListItem Value="Fresh" Text="Fresh"></asp:ListItem>
                                            <asp:ListItem Value="FoIIowup" Text="FoIIowup"></asp:ListItem>
                                            <asp:ListItem Value="Trial Visit" Text="Trial Visit"></asp:ListItem>
                                            <asp:ListItem Value="Sample" Text="Sample"></asp:ListItem>
                                            <asp:ListItem Value="New Development" Text="New Development"></asp:ListItem>
                                            <asp:ListItem Value="Cold Call" Text="Cold Call"></asp:ListItem>
                                            <asp:ListItem Value="Services" Text="Services"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <br />
                                </div>
                                <br />


                                <br />
                                <div class="row" id="DivCallUpdate" runat="server">
                                    <div class="col-md-2">
                                        <asp:Label ID="lblcallstatus" class="form-label LblStyle" runat="server" Text="Call Status :"></asp:Label>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:DropDownList runat="server" class="form-control" OnTextChanged="ddlcallstatus_TextChanged" AutoPostBack="true" ID="ddlcallstatus">
                                            <asp:ListItem>--Select--</asp:ListItem>
                                            <asp:ListItem>Not Interested</asp:ListItem>
                                            <asp:ListItem>Ringing</asp:ListItem>
                                            <asp:ListItem>Follow-up</asp:ListItem>
                                            <asp:ListItem>Busy</asp:ListItem>
                                            <asp:ListItem>Call-Closed</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row" id="DivMeetingUpdate" runat="server">

                                    <div class="col-md-2 LblStyle">Meeting Update<i class="reqcls">*&nbsp;</i> : </div>
                                    <div class="col-md-4">
                                        <asp:DropDownList runat="server" class="form-control" OnTextChanged="ddlmeetingupdate_TextChanged" AutoPostBack="true" ID="ddlmeetingupdate">
                                            <asp:ListItem>--Select--</asp:ListItem>
                                            <asp:ListItem>Not Interested</asp:ListItem>
                                            <asp:ListItem>Follow-up</asp:ListItem>
                                            <asp:ListItem>Closed</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-2 LblStyle">Meeting With Manager<i class="reqcls">*&nbsp;</i> : </div>
                                    <div class="col-md-4">
                                        <asp:DropDownList ID="ddlmeetingwithm" CssClass="form-control" runat="server" Width="100%">
                                            <asp:ListItem Value="1" Text="Individual"></asp:ListItem>

                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <br />
                                <div class="row">
                                    <%--<div class="col-md-2 spancls">Follow-Up Date<i class="reqcls">*&nbsp;</i> : </div>--%>
                                    <div class="col-md-2">
                                        <asp:Label ID="lblfollowupdate" class="form-label LblStyle" runat="server" Text="Follow-Up Date"></asp:Label>
                                    </div>
                                    <div class="col-md-4">
                                        <asp:TextBox ID="txtfollowupdate" CssClass="form-control" TextMode="Date" placeholder="dd-MM-yyyy" runat="server" Width="100%"></asp:TextBox>

                                        <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" Display="Dynamic" ErrorMessage="Please Enter Follow-Up date"
                                                                ControlToValidate="txtfollowupdate" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>--%>
                                    </div>
                                </div>
                                <br />
                                <%--  <div class="row" id="DivTypeofclient" runat="server">
                                    <div class="col-md-2 LblStyle">Type of Client<i class="reqcls">*&nbsp;</i> : </div>
                                    <div class="col-md-4">
                                        <asp:DropDownList runat="server" class="form-control" ID="ddltypeofclient">
                                            <asp:ListItem>--Select--</asp:ListItem>
                                            <asp:ListItem>New</asp:ListItem>
                                            <asp:ListItem>Renewal </asp:ListItem>
                                            <asp:ListItem>Upgrade </asp:ListItem>
                                            <asp:ListItem>Renewal + Upgrade</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-2 LblStyle">Deal Details<i class="reqcls">*&nbsp;</i> : </div>
                                    <div class="col-md-4">
                                        <asp:TextBox ID="txtdealdetails" CssClass="form-control" runat="server" TextMode="MultiLine"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" Display="Dynamic" ErrorMessage="Please Enter Deal Details"
                                            ControlToValidate="txtdealdetails" ValidationGroup="form1" ForeColor="Red" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                        <a style="color: red">Enter Deal Details, Collected Amount, Cheque No, Online , UPI, etc. </a>
                                    </div>
                                </div>--%>

                                <br />
                                <div class="row">
                                    <div class="col-md-4"></div>
                                    <div class="col-6 col-md-2">
                                        <asp:Button ID="btnadd" OnClick="btnadd_Click1" OnClientClick="showLoader();" ValidationGroup="form1" CssClass="form-control btn btn-outline-primary m-2" runat="server" Text="Save" />
                                    </div>
                                    <div class="col-6 col-md-2">
                                        <asp:Button ID="Button1" OnClick="Button1_Click" CssClass="form-control btn btn-outline-danger m-2" runat="server" Text="Cancel" />
                                    </div>
                                    <div class="col-md-4"></div>
                                </div>

                                <br />
                            </div>

                        </div>

                    </div>
                </div>
                <div id="loader" style="display: none;">
                    <div class="spinner"></div>
                </div>
                <asp:HiddenField ID="HiddenField2" runat="server" />
            </div>
            <asp:HiddenField ID="hhd" runat="server" />
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="uploadfile" />
            <asp:PostBackTrigger ControlID="Button1" />
            <asp:PostBackTrigger ControlID="btnadd" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>

