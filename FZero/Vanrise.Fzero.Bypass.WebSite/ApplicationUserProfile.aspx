<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ApplicationUserProfile.aspx.cs" Inherits="ApplicationUserProfile" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
  
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">

      <table cellpadding="0" cellspacing="0" width="100%" >
        <tr id="trAddEdit" runat="server" >
            <td class="section">
                <table width="100%">
                    <tr>
                        <td ></td>
                        <td class="top">
                            <asp:Label ID="lblSectionName" runat="server"></asp:Label></td>
                    </tr>
                    <tr >
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td ></td>
                        <td>
                            <table>
                                <tr>
                                    <td class="caption">
                                        <%=Resources.Resources.UserName %>
                                    </td>
                                    <td ></td>
                                    <td >
                                        <telerik:RadTextBox ID="txtUserName" runat="server" Enabled="False"></telerik:RadTextBox>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="caption">
                                        <%=Resources.Resources.FullName %>
                                    </td>
                                    <td ></td>
                                    <td >
                                      <telerik:RadTextBox ID="txtFullName" runat="server"></telerik:RadTextBox>
                                      <asp:RequiredFieldValidator CssClass="error" ID="rfvFullName" runat="server" ControlToValidate="txtFullName" ErrorMessage="Full name should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>

                                <tr>
                                    <td class="caption">
                                        <%=Resources.Resources.Email %>
                                    </td>
                                    <td ></td>
                                    <td >
                                          <telerik:RadTextBox ID="txtEmailAddress" runat="server"></telerik:RadTextBox>
                                          <asp:RequiredFieldValidator CssClass="error" ID="rfvEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Email address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                          <asp:RegularExpressionValidator CssClass="error" ID="revEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage=" Invalid email address" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                    </td>
                                </tr>


                                  <tr>
                                    <td class="caption">
                                         <%=Resources.Resources.MobileNumber %>
                                    </td>
                                    <td ></td>
                                    <td >
                                         <telerik:RadTextBox ID="txtMobileNumber" runat="server"></telerik:RadTextBox>
                                         <asp:RequiredFieldValidator CssClass="error" ID="rfvMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ErrorMessage="Mobile number should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>


                                


                                  <tr>
                                    <td class="caption">
                                       <%=Resources.Resources.Address %>
                                    </td>
                                    <td ></td>
                                    <td >
                                       <telerik:RadTextBox ID="txtAddress" runat="server"   ></telerik:RadTextBox>
                                       <asp:RequiredFieldValidator CssClass="error" ID="rfvAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>


                            

                               

                                  <tr>
                                    <td class="caption">
                                        &nbsp;</td>
                                    <td >&nbsp;</td>
                                    <td class="commands" >
                                        <telerik:RadButton ID="btnSave" runat="server"   OnClick="btnSave_Click" ValidationGroup="Save" CausesValidation="true"  >
                                            <Icon PrimaryIconUrl="Icons/save.png" />
                                        </telerik:RadButton>
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

