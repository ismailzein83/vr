<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Users.aspx.cs" Inherits="Users" %>

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

    <table cellpadding="0" cellspacing="0" width="100%" class="page">

        <tr id="trResetPassword" runat="server" visible="false">
            <td>


                <div class="row-fluid" id="div2" runat="server">
                    <div class="span12">
                        <div class="widget orange">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Change Password</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <uc1:wucChangePassword runat="server" ID="wucChangePassword" ShowOldPassword="false" OnShowError="wucChangePassword_ShowError" OnSuccess="wucChangePassword_Success" />

                            </div>
                        </div>
                    </div>
                </div>













            </td>
        </tr>


        <tr id="trAddEdit" runat="server" visible="false">
            <td>

                <div class="row-fluid" id="div1" runat="server">
                    <div class="span12">
                        <div class="widget green">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Manage Items</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table width="100%">

                                    <tr>
                                        <td></td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td class="caption">Username</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtUserName" runat="server"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvUserName" runat="server" ControlToValidate="txtUserName" ErrorMessage="Username should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="caption">Name</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtFullName" runat="server"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvFullName" runat="server" ControlToValidate="txtFullName" ErrorMessage="Full name should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>


                                                <tr>
                                                    <td class="caption">Email</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtEmailAddress" runat="server"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvEmailAddress" runat="server" Display="Dynamic" ControlToValidate="txtEmailAddress" ErrorMessage="Email Address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator CssClass="error" ID="revEmailAddress" runat="server" Display="Dynamic" ControlToValidate="txtEmailAddress" ErrorMessage=" Invalid email address" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="caption">Mobile Number</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtMobileNumber" runat="server"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvMobileNumber" runat="server" ControlToValidate="txtMobileNumber" ErrorMessage="Mobile Number should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>





                                                <tr>
                                                    <td class="caption">Address</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtAddress" runat="server" TextMode="MultiLine"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvAddress" runat="server" ControlToValidate="txtAddress" ErrorMessage="Address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="trPassword">
                                                    <td class="caption">Password</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtPassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="trRetypePassword">
                                                    <td class="caption">Retype Password</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtRetypePassword" runat="server" TextMode="Password"></telerik:RadTextBox>
                                                    </td>
                                                </tr>




                                                <tr>
                                                    <td class="caption">Is Active</td>
                                                    <td></td>
                                                    <td>
                                                        <asp:CheckBox ID="chkIsActive" runat="server" />
                                                    </td>
                                                </tr>

                                                <tr id="trPermissionNotes" runat="server" visible="false">
                                                    <td class="caption">Permissions</td>
                                                    <td></td>
                                                    <td>A super user has all the permissions in the system </td>
                                                </tr>
                                                <tr id="trPermission" runat="server">
                                                    <td class="caption">Permissions</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTreeView ID="rtvPermissions" runat="server" DataTextField="Name" DataValueField="Id" DataFieldID="Id" DataFieldParentID="ParentId" CheckBoxes="true" ShowLineImages="true" CheckChildNodes="true"></telerik:RadTreeView>
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

                <div class="row-fluid" id="divFilter" runat="server">
                    <div class="span12">
                        <div class="widget gray">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Filters</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td>
                                            <table cellpadding="2" cellspacing="2">
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td class="caption">Username
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtSearchUserName" runat="server"></telerik:RadTextBox>
                                                    </td>

                                                    <td></td>

                                                    <td class="caption">Name
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtSearchFullName" runat="server"></telerik:RadTextBox>
                                                    </td>
                                                    <td>&nbsp;</td>
                                                    <td class="caption">Address
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtSearchAddress" runat="server"></telerik:RadTextBox>
                                                    </td>




                                                </tr>
                                                <tr>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>

                                                    <td class="caption">Email
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtSearchEmailAddress" runat="server"></telerik:RadTextBox>
                                                    </td>
                                                    <td></td>
                                                    <td class="caption">Mobile Number
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtSearchMobileNumber" runat="server"></telerik:RadTextBox>
                                                    </td>
                                                    <td></td>

                                                    <td class="caption">Activation Status
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <asp:RadioButtonList ID="rblSearchStatus" RepeatDirection="Horizontal"
                                                            RepeatLayout="Table" CellPadding="2" CssClass="options"
                                                            runat="server">
                                                            <asp:ListItem Value="" Selected="True">All</asp:ListItem>
                                                            <asp:ListItem Value="True">Active</asp:ListItem>
                                                            <asp:ListItem Value="False">Inactive</asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </td>
                                                    <td></td>



                                                    <td></td>
                                                    <td class="caption"></td>
                                                    <td></td>
                                                    <td></td>



                                                </tr>

                                                <tr>
                                                    <td colspan="16" align="center" style="padding: 30px 0px 0px 0px">

                                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>



                                                        <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-danger" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>

                                                        <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-success" OnClick="btnAddNew_Click" CausesValidation="False">
                                                                     <i class="icon-plus icon-white"></i> Add New </asp:LinkButton>


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







                <div class="row-fluid" id="divData" runat="server">
                    <div class="span12">
                        <div class="widget blue">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Results</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <telerik:RadGrid ID="gvApplicationUsers" AllowSorting="true" runat="server" CellSpacing="0" PageSize="10" BorderColor="LightGray"
                                    AllowPaging="true" OnItemCommand="gvApplicationUsers_ItemCommand" ClientSettings-Selecting-AllowRowSelect="true"
                                    AutoGenerateColumns="False" OnNeedDataSource="gvApplicationUsers_NeedDataSource" Skin="Metro">
                                    <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="FullName" UniqueName="FullName" />
                                            <telerik:GridBoundColumn DataField="EmailAddress" UniqueName="EmailAddress" />
                                            <telerik:GridBoundColumn DataField="UserName" UniqueName="UserName" />
                                            <telerik:GridBoundColumn DataField="Id" Visible="false" UniqueName="Id" />
                                            <telerik:GridBoundColumn DataField="Address" UniqueName="Address" />
                                            <telerik:GridBoundColumn DataField="MobileNumber" UniqueName="MobileNumber" />
                                            <telerik:GridCheckBoxColumn DataField="IsActive" UniqueName="IsActive" />
                                            <telerik:GridBoundColumn DataField="LastLoginTime" UniqueName="LastLoginTime" />
                                            <telerik:GridBoundColumn DataField="IsSuperUser" Visible="false" UniqueName="IsSuperUser" />

                                            <telerik:GridTemplateColumn ItemStyle-Width="30px" HeaderText="Status">
                                                <ItemTemplate>

                                                    <asp:LinkButton ID="btnActivateDeactivate" ToolTip="Activate / Deactivate" runat="server" Visible='<%#!(bool)Eval("IsSuperUser")%>' CssClass="command btn-info" CommandArgument='<%#Eval("Id") + ";" +Eval("FullName")%>' CommandName="ActivateDeactivate">
                                                        <%# ProcessMyDataItemText((bool)Eval("IsActive"))%>
                                                    </asp:LinkButton>


                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>


                                            <telerik:GridTemplateColumn ItemStyle-Width="30px" HeaderText="Reset">
                                                <ItemTemplate>

                                                    <asp:LinkButton ID="btnReset" ToolTip="Reset Password" runat="server" CssClass="command btn-warning" CommandArgument='<%#Eval("Id") + ";" +Eval("FullName")%>' CommandName="Reset">
                                                        <i class="icon-asterisk"></i>
                                                    </asp:LinkButton>


                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>


                                            <telerik:GridTemplateColumn ItemStyle-Width="30px" HeaderText="Edit">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEdit" ToolTip="Edit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("Id") + ";" +Eval("FullName")%>' CommandName="Modify">
                                                        <i class="icon-pencil"></i>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>


                                            <telerik:GridTemplateColumn ItemStyle-Width="30px" HeaderText="Delete">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnDelete" ToolTip="Delete" runat="server" CssClass="command btn-danger"
                                                        CommandArgument='<%#Eval("Id") + ";"  +Eval("FullName")%>' CommandName="Remove" Visible='<%#!(bool)Eval("IsSuperUser")%>'
                                                        OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                                         <i class="icon-trash"></i>
                                                    </asp:LinkButton>

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
</asp:Content>

