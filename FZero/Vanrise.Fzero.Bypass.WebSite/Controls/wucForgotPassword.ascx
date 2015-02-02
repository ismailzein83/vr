<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucForgotPassword.ascx.cs" Inherits="Controls_wucForgotPassword" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<table  cellpadding="0" cellspacing="0" width="100%" height="100%" >
    <tr>
        <td valign="top" align="left" class="section">
            <table cellpadding="0" cellspacing="0" width="100%" >
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
                    <td></td>
                    <td>
                        <table width="">
                            <tr>
                                    <td class="caption">
                                         <asp:Label ID="Label1" runat="server" Width="150px"></asp:Label>
                                    </td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtEmail" runat="server" placeholder="Enter E-mail" Width="300px"></telerik:RadTextBox>
                                        <telerik:RadTextBox ID="txtCode" runat="server" placeholder="Enter Verification Code" Width="200px"></telerik:RadTextBox>
                                        <telerik:RadTextBox ID="txtUserName" runat="server"  Width="200px"></telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr id="tr2" runat="server">   
                                    <td class="caption">
                                         <asp:Label ID="Label2" runat="server" Width="150px"></asp:Label>
                                    </td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtNewPassword" runat="server" TextMode="Password" placeholder="Enter Password"  Width="200px"></telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr id="tr3" runat="server">
                                    <td class="caption">
                                         <asp:Label ID="Label3" runat="server" Width="150px"></asp:Label>
                                    </td>
                                    <td ></td>
                                    <td class="inputdata">
                                            <telerik:RadTextBox ID="txtConfirmNewPassword" runat="server" TextMode="Password"  placeholder="Confirm Password" Width="200px" ></telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td ></td>
                                    <td class="commands" >
                                           <telerik:RadButton ID="btnNext" runat="server" Text="Next"  Width="100px" OnClick="btnNext_Click"  >
                                               <Icon PrimaryIconUrl="Icons/images.jpg" />
                                           </telerik:RadButton>

                                           <telerik:RadButton ID="btnNext1" runat="server" Text="Next"  Width="100px" OnClick="btnNext1_Click" >
                                               <Icon PrimaryIconUrl="Icons/images.jpg" />
                                           </telerik:RadButton>

                                           <telerik:RadButton ID="btnSave" runat="server" Text="Save"    Width="100px" OnClick="btnSave_Click" >
                                               <Icon PrimaryIconUrl="Icons/save.png" />
                                           </telerik:RadButton>
                                    </td>
                                </tr>
                            <tr>
                                <td>
                                    <asp:HiddenField ID="hfID" runat="server" />
                                    <asp:HiddenField ID="hfCode" runat="server" />
                                </td>
                            </tr>
                        </table>  
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>