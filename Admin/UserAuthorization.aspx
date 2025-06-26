<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/WLSPLMaster.master" AutoEventWireup="true" CodeFile="UserAuthorization.aspx.cs" Inherits="Admin_UserAuthorization" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
       
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>

    <asp:UpdatePanel ID="update" runat="server">
        <ContentTemplate>
            <%--//////////--%>
            <div class="page-wrapper">
                <div class="page-body">
                 
                    <div class="container py-3">
                        <div class="card">
                            <div class="card-header text-uppercase">
                                <h5><b>User Authorization</b></h5>
                            </div>
                            <br />
                            <div class="row">
                                <div class="col-md-2"></div>
                                <div class="col-md-1">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Role :"></asp:Label>
                                </div>
                                <div class="col-md-3">
                                    <asp:DropDownList ID="ddlrole" runat="server" AutoPostBack="true" class="form-control active1 " Width="170px" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlrole_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-md-1">
                                    <asp:Label ID="Label2" runat="server" Font-Bold="true" Text="User :"></asp:Label>
                                </div>
                                <div class="col-md-3">
                                    <asp:DropDownList ID="ddluser" runat="server" AutoPostBack="true" class="form-control active1" Width="170px" AppendDataBoundItems="true" OnSelectedIndexChanged="ddluser_SelectedIndexChanged">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <br />
                            <%--user Authorization grid START--%>
                            <div id="GridDiv" runat="server">
                                <div class="col-lg-12">
                                    <div class="mb-4">
                                        <div id="DivRoot">
                                            <asp:GridView ID="gvUserAuthorization" runat="server" AutoGenerateColumns="False"
                                                EmptyDataText="No records found" DataKeyNames="ID"
                                                CellPadding="4" ForeColor="#333333" GridLines="None" Width="100%" HorizontalAlign="Center" BackColor="#0755a1" OnRowDataBound="gvUserAuthorization_RowDataBound" OnRowCommand="gvUserAuthorization_RowCommand" AllowPaging="false" PagerStyle-CssClass="paging" PageSize="10">
                                                <AlternatingRowStyle BackColor="White" VerticalAlign="Middle" />
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Sr. No." HeaderStyle-Width="50px" HeaderStyle-CssClass="text-center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblsno" runat="server" Text='<%# Container.DataItemIndex+1 %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle CssClass="text-center" Width="100px" />
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Page Name" HeaderStyle-CssClass="text-center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMenuName" runat="server" Text='<%# Eval("MenuName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Menu Name" Visible="false" HeaderStyle-CssClass="text-center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblPageName" runat="server" Text='<%# Eval("PageName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Menu ID" HeaderStyle-CssClass="text-center" Visible="false">

                                                        <ItemTemplate>
                                                            <asp:Label ID="lblMenuId" runat="server" Text='<%# Eval("MenuID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <HeaderStyle CssClass="text-center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Role" HeaderStyle-CssClass="text-center" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblrole" runat="server" Text='<%# Eval("Role") %>'></asp:Label>
                                                        </ItemTemplate>
                                                        <ItemTemplate>
                                                        </ItemTemplate>
                                                        <HeaderStyle CssClass="text-center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Edit" HeaderStyle-CssClass="text-center">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkPages" readonly="true" AutoPostBack="true" runat="server" name="chk" />
                                                        </ItemTemplate>
                                                        <HeaderStyle CssClass="text-center" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="View" HeaderStyle-CssClass="text-center">
                                                        <ItemTemplate>
                                                            <asp:CheckBox ID="chkPagesView" readonly="true" AutoPostBack="true" runat="server" name="chk" />
                                                        </ItemTemplate>
                                                        <HeaderStyle CssClass="text-center" />
                                                    </asp:TemplateField>
                                                </Columns>

                                                <EditRowStyle BackColor="#2461BF" HorizontalAlign="Center" />
                                                <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
                                                <HeaderStyle BackColor="#212529" Font-Bold="True" ForeColor="White" />
                                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                                <RowStyle BackColor="#eaeaea " HorizontalAlign="Center" />
                                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                                <SortedAscendingHeaderStyle BackColor="#0755a1" />
                                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                                            </asp:GridView>
                                            <div id="DivFooterRow" style="overflow: hidden">
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <center>   <div class="col-md-6">  
              <asp:Button  ID="btnSubmit" runat="server" class="btn btn-dark col-sm-3 " Text="Submit"  OnClick="btnSubmit_Click"  ></asp:Button>
                &nbsp;&nbsp;        &nbsp;&nbsp; 
             <asp:Button  ID="btnCancel" runat="server" class="btn btn-dark col-sm-3 " Text="Cancel"  CausesValidation="False" OnClick="btnCancel_Click" ></asp:Button>

            </div></center>
                                <br />
                            </div>
                        </div>
                    </div>
                </div>
                <%--//////////--%>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

