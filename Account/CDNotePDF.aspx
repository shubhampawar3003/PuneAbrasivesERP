<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CDNotePDF.aspx.cs" Inherits="Admin_TaxInvoicePDF" %>

<!DOCTYPE html>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/bootstrap.min.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server"></asp:ToolkitScriptManager>
        <asp:UpdatePanel ID="updatepnl" runat="server">
            <ContentTemplate>

                <div class="container-fluid">
                    <div class="row" style="display:none">
                        <div class="col-md-2" style="margin-top: 10px;"></div>
                        <div class="col-md-10" style="margin-top: 10px;">
                            <asp:Button runat="server" ID="btnOriginal" Text="ORIGINAL FOR BUYER" CssClass="btn btn-primary" OnClick="btnOriginal_Click" />
                            <asp:Button runat="server" ID="btnDuplicate" Text="DUPLICATE FOR TRANSPORTER" CssClass="btn btn-primary" OnClick="btnDuplicate_Click" />
                            <asp:Button runat="server" ID="btnTriplicate" Text="TRIPLICATE FOR SUPPLIER" CssClass="btn btn-primary" OnClick="btnTriplicate_Click" />
                            <asp:Button runat="server" ID="btnExtra" Text="EXTRA COPY" CssClass="btn btn-primary" OnClick="btnExtra_Click" />
                        </div>
                        <div class="col-md-2" style="margin-top: 10px;"></div>
                    </div>
                    <div class="row" style="display: block; margin-top: 10px;">
                        <iframe id="ifrRight6" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 685px;"></iframe>
                    </div>
                </div>

            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnOriginal" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </form>
</body>
</html>
