<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login"
    MasterPageFile="~/MasterPage.master" %>

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

    <table cellpadding="0" cellspacing="0" style="width: 100%"   >
        <tr>
            <td valign="top" align="center"    >
                <div class="row-fluid" id="div1" runat="server" style="width: 25%">
                    <div class="span12">
                        <div class="widget green">
                            <div class="widget-title">
                                <h4><i class="icon-key"></i>Login</h4>

                            </div>
                            <div class="widget-body" style="display: block;">



                                <table cellpadding="0" cellspacing="0">


                                    <tr>
                                        <td valign="top" align="center">
                                            <table cellpadding="0" cellspacing="0" align="center">


                                                <tr>
                                                    <td>
                                                        <div class="input-icon left">
                                                            <i class="icon-user"></i>
                                                            <asp:TextBox ID="txtUsername" runat="server" placeholder="Username"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <div class="input-icon left">
                                                            <i class="icon-lock"></i>
                                                            <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" placeholder="Password"></asp:TextBox>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr class="verticalSpace">
                                                    <td id="tdError" runat="server" class="error"></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:LinkButton ID="btnSignIn" runat="server" class="btn btn-success"
                                                            OnClientClick="return validateData();" OnClick="btnSignIn_Click">
                                             <i class="icon-signin icon-white"></i> Sign In
                                                        </asp:LinkButton>


                                                    </td>

                                                </tr>
                                                <tr class="verticalSpace">
                                                    <td></td>
                                                </tr>

                                            </table>
                                        </td>
                                    </tr>
                                </table>

                            </div>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>


</asp:Content>
