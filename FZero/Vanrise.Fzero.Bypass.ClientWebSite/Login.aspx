<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login"
    MasterPageFile="~/Master.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="headContent" runat="server" ContentPlaceHolderID="head">
     <script type="text/javascript">

         $(document).ready(function () {
             $(window).keydown(function (e) {
                 if (e.keyCode == 13) {
                     __doPostBack('<%=btnSignIn.UniqueID%>', "");
                }

            });
        });




        var txtUsername = '#<%= txtUsername.ClientID %>';
         var txtPassword = '#<%= txtPassword.ClientID %>';
         var tdError = '#<%= tdError.ClientID %>';
         function validateData() {
             if ($(txtUsername).val() == '') {
                 $(tdError).text('Username is required');
                 return false;
             }
             if ($(txtPassword).val() == '') {
                 $(tdError).text('Password is required');
                 return false;
             }
             $(tdError).text('');
             return true;
         }
    </script>
</asp:Content>

<asp:Content ID="bodyContent" runat="server" ContentPlaceHolderID="body">
    <table align="center" style="height: 400px;" cellpadding="0" cellspacing="0" border="0">

        <tr>
            <td valign="middle"  align="center" >
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/f Zero.png" />
            </td>
        </tr>
        <tr>
            <td valign="middle">
                <table class="ApplicationUser" cellpadding="0" cellspacing="0" width="100%" height="100%">
                    <tr>
                        <td valign="top" align="center">
                            <table cellpadding="0" cellspacing="0" align="center">
                                <tr>
                                    <td style="padding: 10px; font-size: 35px; color: #7B1126;" align="center">Login</td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtUsername" runat="server" placeholder="Username"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox></td>
                                </tr>

                                

                                <tr class="verticalSpace">
                                    <td id="tdError" runat="server" class="error"></td>
                                </tr>
                                <tr>
                                    <td>


                                        <asp:Button ID="btnSignIn" runat="server" Text="Sign In" CssClass="theme-btn"
                                            OnClientClick="return validateData();" OnClick="btnSignIn_Click" /></td>

                                </tr>
                                <tr class="verticalSpace">
                                    <td></td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnForgotPassword" runat="server" Text="Forgot Password" CssClass="theme-btn" OnClick="btnForgotPassword_Click" /></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</asp:Content>
