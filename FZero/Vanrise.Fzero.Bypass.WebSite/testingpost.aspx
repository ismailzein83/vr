<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="testingpost.aspx.cs" Inherits="testingpost" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="Controls/wucGeneratedCallInformation.ascx" TagName="wucGeneratedCallInformation" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/Styles/tilestyle.css" rel="stylesheet" />
    <link href="App_Themes/Styles/modalDialog.css" rel="stylesheet" />
    <script type="text/javascript">

        function RowSelecting(sender, args) {

            var UsingColumn = args.getDataKeyValue("ReportingStatusID"); // Column UniqueName

            if (UsingColumn == '2') {
                args.set_cancel(true);

            }

        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

   <asp:LinkButton ID="btnTest" runat="server" CssClass="btn btn-success" OnClick="btnTest_Click"   >
                                                                 <i class="icon-arrow-right icon-white"></i> Test </asp:LinkButton>


</asp:Content>

