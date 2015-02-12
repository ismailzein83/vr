<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucChangePassword.ascx.cs" Inherits="Controls_wucChangePassword" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<table>
    <tr>
        <td class="caption">
            <%=Resources.Resources.UserName %>
        </td>
        <td ></td>
        <td class="inputdata">
            <telerik:RadTextBox ID="txtRPUserName" runat="server" Enabled="False"></telerik:RadTextBox>
        </td>
    </tr>

    <tr id="TrOldPass" runat="server">
        <td class="caption">
            <%=Resources.Resources.OldPassword %>
        </td>
        <td ></td>
        <td class="inputdata">
            <telerik:RadTextBox ID="txtOldPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
        </td>
    </tr>
    <tr>
        <td class="caption">
            <%=Resources.Resources.NewPassword %>
        </td>
        <td ></td>
        <td class="inputdata">
            <telerik:RadTextBox ID="txtRPPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
        </td>
    </tr>
    <tr>
        <td class="caption">
            <%=Resources.Resources.RetypePassword %>
        </td>
        <td ></td>
        <td class="inputdata">
            <telerik:RadTextBox ID="txtRPRetypePassword" runat="server" TextMode="Password"></telerik:RadTextBox>
        </td>
    </tr>
    <tr>

        <td class="caption"></td>
        <td >&nbsp;</td>
        <td class="commands">
            <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
            <asp:LinkButton ID="btnResetPassword" runat="server" CssClass="btn btn-primary" OnClick="btnResetPassword_Click">
                                                                 <i class="icon-search icon-white"></i> Reset </asp:LinkButton>


            <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>
        </td>


    </tr>
</table>
