<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucMobileOperatorInformation.ascx.cs" Inherits="wucMobileOperatorInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>





<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td class="section" valign="top">
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="top hspace-20"></td>
                    <td class="top">
                        <%= Resources.Resources.General %></td>
                </tr>
                <tr class="vspace-10">
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="hspace-20"></td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr class="required">
                                            <td class="caption"><%= Resources.Resources.FullName %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtProfileName" runat="server" ReadOnly="true"  ></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.MobileNumber %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobile" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr runat="server" id="trPrefix"  >
                                            <td class="caption"><%= Resources.Resources.NumberPrefixesSeperatedBySemiColon %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtPrefix" runat="server"></telerik:RadTextBox>
                                                <telerik:RadTextBox ID="lblPrefixes"  Text="All Others" runat="server"  ReadOnly="true" ></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Contact %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtContactName" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Website %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtWebsite" runat="server"></telerik:RadTextBox>
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
        <td class="top hspace-20"></td>
        <td valign="top">
            <table width="100%" cellpadding="0" cellspacing="0">
               
                <tr>
                    <td valign="top" class="section">
                        <table width="100%" cellpadding="0">
                            <tr>
                                <td class="top hspace-20"></td>
                                <td class="top">
                                    <%= Resources.Resources.Authentication %></td>
                            </tr>
                            <tr class="vspace-10">
                                <td></td>
                                <td></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%">

                                        <tr class="required">
                                            <td class="caption"><%= Resources.Resources.Email %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtEmail" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr class="required">
                                            <td class="caption"><%= Resources.Resources.UserName %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorUserName" runat="server"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr class="required" runat="server" id="trPassword1">
                                            <td class="caption"><%= Resources.Resources.Password %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr class="required" runat="server" id="trPassword2">
                                            <td class="caption"><%= Resources.Resources.RetypePassword %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorConfirmPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
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


