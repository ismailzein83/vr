<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucChangePassword.ascx.cs" Inherits="Controls_wucChangePassword" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<table cellpadding="0" cellspacing="0" width="100%" class="page">
    <tr id="trAddEdit" runat="server">
        <td class="section" valign="Top">
            <table width="100%">
                <tr>
                    <td class="top hspace-20"></td>
                    <td class="top">
                        <asp:Label ID="lblSectionName" runat="server"></asp:Label></td>
                </tr>
                <tr class="vspace-10">
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="hspace-20"></td>
                    <td>
                        <table>
                            <tr>
                                <td class="caption">
                                    <%=Resources.Resources.UserName %>
                                </td>
                                <td class="hspace-20"></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtRPUserName" runat="server" Enabled="False"></telerik:RadTextBox>
                                </td>
                            </tr>

                            <tr id="TrOldPass" runat="server">
                                <td class="caption">
                                    <%=Resources.Resources.OldPassword %>
                                </td>
                                <td class="hspace-20"></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtOldPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="caption">
                                    <%=Resources.Resources.NewPassword %>
                                </td>
                                <td class="hspace-20"></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtRPPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="caption">
                                    <%=Resources.Resources.RetypePassword %>
                                </td>
                                <td class="hspace-20"></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtRPRetypePassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>

                                <td class="caption"></td>
                                <td class="hspace-20">&nbsp;</td>
                                <td class="commands" >
                                    <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
                                    <telerik:RadButton ID="btnResetPassword" runat="server" OnClick="btnResetPassword_Click" >
                                        <Icon PrimaryIconUrl="Icons/save.png" />
                                    </telerik:RadButton>
                                    <telerik:RadButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" >
                                        <Icon PrimaryIconUrl="Icons/cancel.png" />
                                    </telerik:RadButton>
                                </td>


                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
