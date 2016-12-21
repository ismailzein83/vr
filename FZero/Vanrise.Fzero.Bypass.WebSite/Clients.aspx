<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Clients.aspx.cs" Inherits="Clients" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/wucChangePassword.ascx" TagPrefix="uc1" TagName="wucChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        function applyToChildren(flag, id) {
            if (flag == false) {
                $('.' + id).removeAttr('checked');
                $('.' + id).attr('disabled', 'disabled');
            }
            else {
                $('.' + id).removeAttr('disabled');
            }
        }
    </script>

    <script>
        function setStyle() {
            $('#tblPermissions tr:odd').addClass('GVAltRow');
            $('#tblPermissions tr:even').addClass('GVRow');
            $('#tblPermissions tr:first').removeClass('GVRow');
        }

        $(document).ready(function () { setStyle(); });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <table cellpadding="0" cellspacing="0" width="100%" >

        <tr id="trAddEdit" runat="server" visible="false">
            <td class="section">


                <div class="row-fluid" id="div1" runat="server">
                    <div class="span12">
                        <div class="widget blue">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Manage Item</h4>

                            </div>
                            <div class="widget-body" style="display: block;">

                                <table width="100%">

                                    <tr >
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td ></td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td class="caption"><%=Resources.Resources.Name %></td>
                                                    <td ></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtName" runat="server" Enabled="False"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvUserName" runat="server" ControlToValidate="txtName" ErrorMessage="Username should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                              

                                                <tr>
                                                    <td class="caption"><%= Resources.Resources.IsClientReport %></td>
                                                    <td ></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkIsClientReport" runat="server" />
                                                    </td>
                                                </tr>
                                                
                                                     <tr>
                                                    <td class="caption"><%= Resources.Resources.IsClientReportSecurity %></td>
                                                    <td ></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkIsClientReportSecurity" runat="server" />
                                                    </td>
                                                </tr>


                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                    <td class=" commands">
                                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                                             <i class="icon-save icon-white"></i>
                                                                 Save
                                                        </asp:LinkButton>
                                                        <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                                                 <i class="icon-ban-circle icon-white"></i>
                                                                    Cancel
                                                        </asp:LinkButton>
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
            </td>
        </tr>


        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr>
                        <td valign="top">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="row-fluid" id="divData" runat="server">
                                            <div class="span12">
                                                <div class="widget blue">
                                                    <div class="widget-title">
                                                        <h4><i class="icon-reorder"></i>Search Results</h4>

                                                    </div>
                                                    <div class="widget-body" style="display: block;">
                                                        <telerik:RadGrid ID="gvApplicationUsers" AllowSorting="true" runat="server" CellSpacing="0" PageSize="10" BorderColor="LightGray"
                                                            AllowPaging="true" OnItemCommand="gvApplicationUsers_ItemCommand" ClientSettings-Selecting-AllowRowSelect="true"
                                                            AutoGenerateColumns="False" OnNeedDataSource="gvApplicationUsers_NeedDataSource" Skin="Metro">
                                                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                                                <Columns>
                                                                     <telerik:GridBoundColumn DataField="ID" Visible="false" UniqueName="ID" />

                                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />
                                                                    <telerik:GridBoundColumn DataField="ClientEmail" UniqueName="ClientEmail" />
                                                                    <telerik:GridBoundColumn DataField="ClientReport" UniqueName="ClientReport" />
                                                                    <telerik:GridBoundColumn DataField="ClientReportSecurity"  UniqueName="ClientReportSecurity" />
                                                                   

                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                                        <ItemTemplate>
                                                                            <telerik:RadButton ButtonType="LinkButton" ID="btnEdit" Text="Edit" ToolTip="Edit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("ID") + ";" +Eval("Name")%>' CommandName="Modify">
                                                                            </telerik:RadButton>
                                                                        </ItemTemplate>
                                                                    </telerik:GridTemplateColumn>
                                                                </Columns>
                                                            </MasterTableView>
                                                        </telerik:RadGrid>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
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

