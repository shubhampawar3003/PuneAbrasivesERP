<%@ Page Language="C#" AutoEventWireup="true" CodeFile="E_InvoicePDF.aspx.cs" Inherits="Gov_Bills_E_InvoicePDF" %>

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
                    <div class="row" style="display: block; margin-top: 10px;">
                        <asp:Image ID="img_QrCode" runat="server" Visible="false" />
                        <asp:Image ID="imgBarcode" runat="server" Visible="false" />
                        <iframe id="ifrRight6" runat="server" enableviewstate="false" style="width: 100%; -ms-zoom: 0.75; height: 685px;"></iframe>
                    </div>
                </div>

            </ContentTemplate>
    
        </asp:UpdatePanel>
    </form>
</body>
</html>
