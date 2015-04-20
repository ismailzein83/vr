<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/MasterPage.master"
    CodeFile="SwitchProfiles.aspx.cs" Inherits="SwitchProfiles" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Source</h4>
                  
                </div>
                <div class="widget-body" style="display: block;">
                    <table cellspacing="0" cellpadding="0">
                        <tr>
                            <td class="caption">Name
                            </td>
                            <td class="inputData">
                                <asp:TextBox ID="txtSearchName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                       
                        <tr>
                            <td>&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                    <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger" OnClick="btnClear_Click">
                                    <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>
                                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-success" OnClick="btnAdd_Click">
                                    <i class="icon-plus icon-white"></i> Add New </asp:LinkButton>
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
                    <h4><i class="icon-reorder"></i>Source Data</h4>
                    
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>
                            <asp:BoundField HeaderText="Name" DataField="Name" />
                            <asp:BoundField HeaderText="Database Name" DataField="Switch_DatabaseConnections.DatabaseName" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("Id")%>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="command btn-danger"
                                        CommandArgument='<%#Eval("Id")%>' CommandName="Remove"
                                        OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                       <%-- OnClientClick="return showDialog(this, 'Delete Confirmation', 'Are you sure you want to delete this item?');">--%>
                                        <i class="icon-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <div class="row-fluid" id="divDetails" runat="server">
        <div class="span12">
            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Source Details</h4>
                </div>

                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <div class="span6 allborders">
                                    <h4 class="breadcrumb">Source Information</h4>
                                    <table cellspacing="0" cellpadding="1" class="table">
                                        <tbody>
                                            <tr>
                                                <td class="caption required">Name</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvName" runat="server" Display="Dynamic"
                                                        ControlToValidate="txtName" ErrorMessage="Name should not be empty" ValidationGroup="Save">
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                           

                                             <tr>
                                                <td class="caption required">Allow Auto Import</td>
                                                <td class="inputData">
                                                    <asp:CheckBox ID="chkAutoImport" runat="server"></asp:CheckBox>
                                                    <br />
                                                   
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="span6 allborders">
                                    <h4 class="breadcrumb" style="padding-right:5px;">
                                        Database Connection
                                        <asp:LinkButton ID="btnTestConnection" runat="server" CssClass="btn btn-warning"
                                            OnClick="btnTestConnection_Click">
                                            <i class="icon-warning-sign"></i>Test Connection
                                                </asp:LinkButton>
                                    </h4>
                                    <table cellspacing="0" cellpadding="0" class="table">
                                        <tr>
                                            <td class="caption required">Server Name
                            </td>
                                            <td class="inputData">
                                                <asp:TextBox ID="txtServerName" runat="server"></asp:TextBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvServerName" runat="server" Display="Dynamic"
                                                    ControlToValidate="txtServerName" ErrorMessage="Server Name should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption required">User Id
                            </td>
                                            <td class="inputData">
                                                <asp:TextBox ID="txtUserId" runat="server"></asp:TextBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvUserId" runat="server" Display="Dynamic"
                                                    ControlToValidate="txtUserId" ErrorMessage="User Id should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption required">User Password
                            </td>
                                            <td class="inputData">
                                                <asp:TextBox ID="txtUserPassword" runat="server"></asp:TextBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="alert-error" ID="rfvUserPassword" runat="server" Display="Dynamic"
                                                    ControlToValidate="txtUserPassword" ErrorMessage="User Password should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption required">Database Name
                            </td>
                                            <td class="inputData">
                                                <asp:TextBox ID="txtDatabaseName" runat="server"></asp:TextBox>
                                                <br />
                                                <asp:RequiredFieldValidator CssClass="alert-error" ID="rfvDatabaseName" runat="server" Display="Dynamic"
                                                    ControlToValidate="txtDatabaseName" ErrorMessage="Database Name should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="space10"></td>
                        </tr>
                        <tr>
                            <td style="text-align: center;">
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

                </div>
            </div>
        </div>
    </div>
</asp:Content>
