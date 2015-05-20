<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportChart.aspx.cs" Inherits="TOne.Web.Reports.ReportChart" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>    
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%" Height="100%" ShowBackButton="False" ShowFindControls="False" ShowPageNavigationControls="False" ShowPrintButton="False"></rsweb:ReportViewer>
    </div>
    </form>
    <p style="direction: ltr">
        &nbsp;</p>
</body>
</html>
