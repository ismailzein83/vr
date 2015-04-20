<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucChangePassword.ascx.cs" Inherits="Controls_wucChangePassword" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<table cellpadding="0" cellspacing="0" width="100%" class="page">
    <tr id="trAddEdit" runat="server">
        <td   valign="Top">
            <table width="100%">
               
                <tr class="vspace-10">
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td  ></td>
                    <td>
                        <table>
                            <tr>
                                <td class="caption">
                                    Username
                                </td>
                                <td  ></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtRPUserName" runat="server" Enabled="False"></telerik:RadTextBox>
                                </td>
                            </tr>

                            <tr id="TrOldPass" runat="server">
                                <td class="caption">
                                    <asp:Label ID="Label3" runat="server" Text="Old Password"></asp:Label>
                                </td>
                                <td  ></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtOldPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="caption">
                                    <asp:Label ID="Label2" runat="server" Text="New Password"></asp:Label>
                                </td>
                                <td  ></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtRPPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="caption">
                                     <asp:Label ID="Label1" runat="server" Text="Retype Password"></asp:Label>
                                </td>
                                <td  ></td>
                                <td class="inputdata">
                                    <telerik:RadTextBox ID="txtRPRetypePassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>

                                <td class="caption"></td>
                                <td  >&nbsp;</td>
                                <td class="commands" >
                                    <asp:HiddenField ID="hfId" runat="server" Value="0" />



                                      <asp:LinkButton ID="btnResetPassword" runat="server" CssClass="btn btn-primary" OnClick="btnResetPassword_Click">
                                                                 <i class="icon-search icon-white"></i> Reset </asp:LinkButton>


                                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>

                                   
                                </td>


                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
