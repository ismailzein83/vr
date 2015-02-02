<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Sources.aspx.cs" Inherits="Sources" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Edit Item</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table width="100%">
                       
                        <tr>
                            <td></td>
                            <td>
                                <table>
                                    <tr>
                                        <td class="caption">
                                            <%=Resources.Resources.Source %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSources" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlSources_SelectedIndexChanged"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator CssClass="error" ID="rfvSources" runat="server" ControlToValidate="ddlSources" ErrorMessage="Source should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>

                                        </td>
                                    </tr>




                                    <tr>
                                        <td class="caption">
                                            <%=Resources.Resources.Email %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadTextBox ID="txtEmail" runat="server"></telerik:RadTextBox>
                                            <asp:RequiredFieldValidator CssClass="error" ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Email should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>

                                        </td>
                                    </tr>



                                    <tr>
                                        <td class="caption">
                                            Source Kind
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSourceKind" runat="server"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator CssClass="error" ID="rfvSourceKind" runat="server" ControlToValidate="ddlSourceKind" ErrorMessage="Source Kind should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>

                                        </td>
                                    </tr>



                                     <tr>
                                        <td class="caption">
                                            Switch
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSwitch" runat="server"></telerik:RadComboBox>
                                            <asp:RequiredFieldValidator CssClass="error" ID="rfvSwitch" runat="server" ControlToValidate="ddlSwitch" ErrorMessage="Switch should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>

                                        </td>
                                    </tr>




                                    <tr>
                                        <td class="caption">&nbsp;</td>
                                        <td>&nbsp;</td>
                                        <td class="commands">
                                           

                                             <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                                             <i class="icon-save icon-white"></i>
                                                                 Save
                                                                                </asp:LinkButton>



                                            <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                        </td>
                                    </tr>







                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </div>
    </div>



</asp:Content>

