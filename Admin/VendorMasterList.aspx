<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.Master" AutoEventWireup="true" CodeFile="VendorMasterList.aspx.cs" Inherits="Admin_VendorMasterList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="../Content/css/Griddiv.css" rel="stylesheet" />
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
                timer: 5000,
                showCancelButton: false,
                showConfirmButton: false
            }).then(function () {
                window.location.href = "VendorMaster.aspx";
            })
        };
    </script>
    <style>
        .gvhead {
            text-align: center;
            color: #ffffff;
            background-color: #212529;
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

                /*.pagination-ys table > tbody > tr > td:last-child > a,
                .pagination-ys table > tbody > tr > td:last-child > span {
                    border-bottom-right-radius: 4px;
                    border-top-right-radius: 4px;
                }*/

                .pagination-ys table > tbody > tr > td > a:hover,
                .pagination-ys table > tbody > tr > td > span:hover,
                .pagination-ys table > tbody > tr > td > a:focus,
                .pagination-ys table > tbody > tr > td > span:focus {
                    color: #97310e;
                    background-color: #eeeeee;
                    border-color: #dddddd;
                }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ScriptManager>

    <div class="container-fluid px-4">
        <div class="row">
            <div class="col-9 col-md-10">
                 <h4 class="mt-4">&nbsp <b>SUPPLIER LIST</b></h4>
            </div>
            <div class="col-3 col-md-2 mt-4">
                <asp:Button ID="btnCreate" CssClass="form-control btn btn-warning" OnClick="btnCreate_Click" runat="server" Text="Create" />
            </div>
        </div>
        <br />
        <div>
            <div class="row">
                <%--<div class="table-responsive text-center">--%>
                <div class="table ">
                    <asp:GridView ID="GVVendor" runat="server" CellPadding="4" DataKeyNames="ID" Width="100%" OnRowDataBound="GVVendor_RowDataBound"
                        CssClass="grivdiv pagination-ys" AutoGenerateColumns="false" OnRowCommand="GVVendor_RowCommand" OnPageIndexChanging="GVVendor_PageIndexChanging">
                        <Columns>
                            <asp:TemplateField HeaderText="Sr.No." HeaderStyle-Width="50px" ItemStyle-HorizontalAlign="Center" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Suplier Code" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Vendorcode" runat="server" Text='<%#Eval("Vendorcode")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Suplier Name" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Vendorname" runat="server" Text='<%#Eval("Vendorname")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Owner Name" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Ownername" runat="server" Text='<%#Eval("Ownername")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Email ID" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="EmailID" runat="server" Text='<%#Eval("EmailID")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Mobile No." HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="MobileNo" runat="server" Text='<%#Eval("MobileNo")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%-- <asp:TemplateField HeaderText="Designation" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Designation" runat="server" Text='<%#Eval("Designation")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PAN No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Panno" runat="server" Text='<%#Eval("Panno")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Aadhar No." HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Aadharno" runat="server" Text='<%#Eval("Aadharno")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Account Details" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Accountdetails" runat="server" Text='<%#Eval("Accountdetails")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Birth Date" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Birthdate" runat="server" Text='<%#Eval("Birthdate")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address" HeaderStyle-CssClass="gvhead">
                                        <ItemTemplate>
                                            <asp:Label ID="Address" runat="server" Text='<%#Eval("Address")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Status" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:Label ID="Status" runat="server" Text='<%#Eval("Status")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="ACTION" HeaderStyle-CssClass="gvhead">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" Height="27px" CausesValidation="false" CommandName="RowEdit" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-edit' style='font-size:24px;color: #212529;'></i></asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" Height="27px" ToolTip="Delete" CausesValidation="false" CommandName="RowDelete" OnClientClick="Javascript:return confirm('Are you sure to Delete?')" CommandArgument='<%#Eval("ID")%>'><i class='fas fa-trash' style='font-size:24px;color: red;'></i></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

