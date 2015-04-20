<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UserProfile.aspx.cs" Inherits="UserProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table cellpadding="0" cellspacing="0" width="100%" class="page">
        <tr id="trAddEdit" runat="server">
            <td  >
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
                                    <td class="caption">Username
                                    </td>
                                    <td  ></td>
                                    <td>
                                        <telerik:RadTextBox ID="txtUserName" runat="server" Enabled="False"></telerik:RadTextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="caption">Full Name
                                    </td>
                                    <td  ></td>
                                    <td>
                                        <telerik:RadTextBox ID="txtFullName" runat="server"></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvFullName" runat="server" ControlToValidate="txtFullName" ErrorMessage="Full name should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="caption">Email
                                    </td>
                                    <td  ></td>
                                    <td>
                                        <telerik:RadTextBox ID="txtEmailAddress" runat="server"></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Email address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator CssClass="error" ID="revEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage=" Invalid email address" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                    </td>
                                </tr>


                                <tr>
                                    <td class="caption">Mobile Number
                                    </td>
                                    <td  ></td>
                                    <td>
                                        <telerik:RadTextBox ID="txtMobileNumber" runat="server"></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ErrorMessage="Mobile number should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>





                                <tr>
                                    <td class="caption">Address
                                    </td>
                                    <td  ></td>
                                    <td>
                                        <telerik:RadTextBox ID="txtAddress" runat="server"></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>






                                <tr>
                                    <td class="caption">&nbsp;</td>
                                    <td  >&nbsp;</td>
                                    <td class="commands">

                                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                                 <i class="icon-save icon-white"></i>
                                          Save
                                        </asp:LinkButton>


                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                        <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
                                    </td>
                                </tr>







                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>




</asp:Content>

