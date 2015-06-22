<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestOperatorHistoryPrint.aspx.cs" Inherits="TestOperatorHistoryPrint" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Test Operator History</title>
</head>
<body>
    <form id="printForm" runat="server" style="text-align: center;">
        <div>
            <asp:ScriptManager ID="smScriptManager" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="rptApplication" runat="server" Width="29.7cm" Height="21cm" BackColor="White"
                ShowCredentialPrompts="False" ShowDocumentMapButton="False" ShowExportControls="true"
                ShowFindControls="true" ShowPrintButton="true">
                <LocalReport>
                    <DataSources>
                        <rsweb:ReportDataSource DataSourceId="" Name="gfdg" />
                    </DataSources>
                </LocalReport>
            </rsweb:ReportViewer>
        </div>
    </form>

</body>
</html>