<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="Strategies.aspx.cs" Inherits="Strategies" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Strategies</h4>
                   
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
                            <td class="caption">Description
                            </td>
                            <td class="inputData">
                                <asp:TextBox ID="txtSearchDescription" runat="server"></asp:TextBox>
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
                    <h4><i class="icon-reorder"></i>Strategies Data</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        OnRowCommand="gvData_RowCommand" Width="100%">
                        <Columns>
                            <asp:BoundField HeaderText="Name" DataField="Name" />
                            <asp:BoundField HeaderText="CreationDate" DataField="CreationDate" />
                            <asp:BoundField HeaderText="Description" DataField="description" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" ToolTip="Edit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("Id")%>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" ToolTip="Delete" runat="server" CssClass="command btn-danger"
                                        CommandArgument='<%#Eval("Id")%>' CommandName="Remove"
                                        OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                       <%-- OnClientClick="return showDialog(this, 'Delete Confirmation', 'Are you sure you want to delete this item?');">--%>
                                        <i class="icon-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkProperties" ToolTip="Strategy Properties" runat="server" Text="Properties" Width="120px" CssClass="command btn-success" CommandArgument='<%#Eval("Id")%>' CommandName="Properties">
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkSuspection" ToolTip="Suspicion Levels" runat="server" Text="Suspicion Levels" Width="120px" CssClass="btn btn-primary" CommandArgument='<%#Eval("Id")%>' CommandName="SuspectionLevels">
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="lnkMinValue" ToolTip="Minimum Values" runat="server" Text="Minimum Values" Width="120px" CssClass="command btn-danger" CommandArgument='<%#Eval("Id")%>' CommandName="MinimumValues">
                                    </asp:LinkButton>
                                     <asp:LinkButton ID="lnkRelatedCriteria" ToolTip="Related Criteria" runat="server" Text="Related Criteria" Width="120px" CssClass="command btn-success" CommandArgument='<%#Eval("Id")%>' CommandName="RelatedCriteria">
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
                    <h4><i class="icon-reorder"></i>Strategy Details</h4>
                </div>

                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <div class="span6 allborders">
                                    <h4 class="breadcrumb">Strategy Information</h4>
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
                                                <td class="caption required">Description
                                                </td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvType" runat="server" Display="Dynamic"
                                                        ControlToValidate="txtDescription" ErrorMessage="Description should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>

                                        </tbody>
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
                                          Return
                                </asp:LinkButton>
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
