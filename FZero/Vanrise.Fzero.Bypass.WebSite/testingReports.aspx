<%@ Page Language="C#" AutoEventWireup="true" CodeFile="testingReports.aspx.cs" Inherits="testingReports" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <table width="100%">
                <tr>

                    <td>
                        <telerik:RadTextBox ID="txtReportID"  runat="server" Label="Report ID" SkinID="non" CssClass="non"></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>

                    <td>
                        <telerik:RadTextBox ID="txtRecomnededAction" Label="Recomneded Action" SkinID="non" CssClass="non" TextMode="MultiLine" Rows="4" Width="700px" runat="server" Text="It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fraudulent Calls as they were terminated to your Network and did not pass legally through ITPC's IGW."></telerik:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <telerik:RadComboBox ID="ddlSearchMobileOperator" Label="Mobile Operator" SkinID="non" CssClass="non" runat="server"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>

                        <telerik:RadComboBox ID="ddlSearchClient" Label="Mobile Client" SkinID="non" CssClass="non" runat="server"></telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <telerik:RadComboBox ID="ddlReportFormat" Label="Format" SkinID="non" CssClass="non" runat="server">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="PDF" Value="PDF" />
                                <telerik:RadComboBoxItem runat="server" Text="Excel" Value="Excel" />
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>

                        <asp:LinkButton ID="btnSendReport" runat="server" CssClass="btn btn-success" OnClientClick="return confirm('Are you sure you want to send this report?');" OnClick="btnSendReport_Click">
                                                                 <i class="icon-arrow-right icon-white"></i> Send </asp:LinkButton>



                    </td>

                </tr>
            </table>

            <rsweb:ReportViewer Visible="false" ID="rvToOperator" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%">
                <LocalReport ReportPath="Reports\rptToOperator.rdlc">
                </LocalReport>
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
