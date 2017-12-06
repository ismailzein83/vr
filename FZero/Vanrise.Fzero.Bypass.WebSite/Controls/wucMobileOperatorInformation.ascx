<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucMobileOperatorInformation.ascx.cs" Inherits="wucMobileOperatorInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>





<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td class="section" valign="top">
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td></td>
                    <td class="top">
                        <%= Resources.Resources.General %></td>
                </tr>
                <tr>
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr class="required">
                                            <td class="caption"><%= Resources.Resources.FullName %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtProfileName" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.MobileNumber %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobile" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr runat="server" id="trPrefix">
                                            <td class="caption"><%= Resources.Resources.NumberPrefixesSeperatedBySemiColon %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtPrefix" runat="server"></telerik:RadTextBox>
                                                <telerik:RadTextBox ID="lblPrefixes" Text="All Others" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Contact %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtContactName" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Website %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtWebsite" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.GMT %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadComboBox ID="ddlGMT" runat="server"></telerik:RadComboBox>
                                            </td>
                                        </tr>

                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>

        </td>
        <td></td>
        <td valign="top">
            <table width="100%" cellpadding="0" cellspacing="0">

                <tr>
                    <td valign="top" class="section">
                        <table width="100%" cellpadding="0">
                            <tr>
                                <td></td>
                                <td class="top">
                                    <%= Resources.Resources.Authentication %></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%">

                                        <tr class="required">
                                            <td class="caption"><%= Resources.Resources.Email %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtEmail" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr class="required">
                                            <td class="caption"><%= Resources.Resources.UserName %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorUserName" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr class="required" runat="server" id="trPassword1">
                                            <td class="caption"><%= Resources.Resources.Password %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr class="required" runat="server" id="trPassword2">
                                            <td class="caption"><%= Resources.Resources.RetypePassword %></td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorConfirmPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                         <tr>
                                            <td class="caption">Enable Auto-Reporting</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <asp:CheckBox ID="chkAutoReporting" runat="server"></asp:CheckBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption">Enable Auto-Block</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <asp:CheckBox ID="chkEnableAutoBlock" runat="server"></asp:CheckBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption">Auto-Block Email</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtAutoBlockEmail" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        
                                        
                                                                                <tr>
                                            <td class="caption">Enable Security</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <asp:CheckBox ID="chkEnableSecurity" runat="server"></asp:CheckBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption">Security Email</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtSecurityEmail" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        
                                          <tr>
                                            <td class="caption">Enable FTP</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <asp:CheckBox ID="enableFTP" runat="server"></asp:CheckBox>
                                            </td>
                                         </tr>
                                          <tr id="trFTPAddress" runat="server">
                                            <td class="caption">FTP Address</td>
                                            <td></td>
                                              <td class="inputdata">
                                                <telerik:RadTextBox ID="ftpAddress" runat="server"></telerik:RadTextBox>
                                            </td>
                                          
                                        </tr>
                                         <tr  id="trFTPUserName" runat="server">
                                            <td class="caption">FTP UserName</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="ftpUserName" runat="server"></telerik:RadTextBox>
                                            </td>
                                           
                                        </tr>
                                         <tr id="trFTPPassword" runat="server">
                                            <td class="caption">FTP Password</td>
                                            <td></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="ftpPassword" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="vspace-20">
                    <td></td>
                </tr>
            </table>
        </td>
    </tr>
</table>

<asp:HiddenField ID="hdnId" runat="server" Value="0" />


