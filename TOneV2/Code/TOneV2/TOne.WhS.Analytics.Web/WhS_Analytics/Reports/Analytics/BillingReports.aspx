﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BillingReports.aspx.cs" Inherits="TOne.WhS.Analytics.Web.Reports.Analytics.BillingReports" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
    <div>
    <asp:scriptmanager runat="server"></asp:scriptmanager>
        wer wer wrwe w e
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%" height="100%" ShowPageNavigationControls="true" ></rsweb:ReportViewer>
    </div>
    </form>
</body>
</html>
