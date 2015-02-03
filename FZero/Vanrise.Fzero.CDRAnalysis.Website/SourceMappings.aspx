<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SourceMappings.aspx.cs" Inherits="SourceMappings" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Mapping</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table width="100%">

                        <tr>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Switch
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSwitch" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSwitch_SelectedIndexChanged"></telerik:RadComboBox>
                                        </td>
                                    </tr>

                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>




                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>


                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>

                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>

                                    <tr>
                                        <td valign="Top">
                                            <%=Resources.Resources.Mappings %>
                                        </td>
                                        <td valign="Top"></td>
                                        <td valign="Top">

                                            <telerik:RadMultiPage ID="RadMultiPageMain" runat="server" SelectedIndex="0">
                                                <telerik:RadPageView ID="RadPageViewData" runat="server">
                                                    <telerik:RadListView ID="lvColumns" Enabled="false" OnNeedDataSource="lvColumns_NeedDataSource" OnItemCommand="lvColumns_ItemCommand" runat="server" Width="900px"
                                                        ItemPlaceholderID="SourceMappingItemContainer" DataKeyNames="ID">

                                                        <LayoutTemplate>

                                                            <fieldset>

                                                                <table cellpadding="2" cellspacing="2">

                                                                    <tr>

                                                                        <td class="right">


                                                                            <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-success" CommandName="Insert" Visible="<%# !Container.IsItemInserted %>">
                                                                 <i class="icon-pencil icon-white"></i> Add  </asp:LinkButton>



                                                                        </td>

                                                                    </tr>

                                                                    <tr>

                                                                        <td>

                                                                            <asp:Panel ID="SourceMappingItemContainer" runat="server">
                                                                            </asp:Panel>

                                                                        </td>

                                                                    </tr>


                                                                </table>

                                                            </fieldset>

                                                        </LayoutTemplate>

                                                        <ItemTemplate>

                                                            <fieldset style="float: left; border: 1px dotted #7B1126">

                                                                <table cellpadding="2" cellspacing="2" style="width: 300px">


                                                                    <tr>

                                                                        <td>
                                                                            <asp:Label ID="lblColumnName" runat="server" Text="Column :"></asp:Label>
                                                                        </td>

                                                                        <td style="padding-right: 15px;" align="right">
                                                                            <%# Eval("ColumnName") %>
                                                                        </td>

                                                                    </tr>

                                                                    <tr>

                                                                        <td>
                                                                            <asp:Label ID="lblMappedtoColumnNumber" runat="server" Text="Mapped to Column:"></asp:Label>
                                                                        </td>

                                                                        <td style="padding-right: 15px;" align="right">

                                                                            <%# Eval("PredefinedColumn.Name") %>

                                                                        </td>

                                                                    </tr>



                                                                    <tr>

                                                                        <td colspan="2">
                                                                           

                                                                            <asp:LinkButton ID="btnEditItemTemplate" CommandArgument='<%# Eval("ID") %>' runat="server" CssClass="command btn-primary"
                                                                                CommandName="Modify" ToolTip="Modify">
                                                                             <i class="icon-pencil"></i> 
                                                                            </asp:LinkButton>
                                                                             <asp:LinkButton ID="btnDeleteItemTemplate" runat="server" CssClass="command btn-inverse"
                                                                                CommandArgument='<%# Eval("ID") %>' CommandName="Delete" ToolTip="Delete" OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                                                             <i class="icon-trash"></i> 
                                                                            </asp:LinkButton>
                                                                        </td>

                                                                    </tr>

                                                                </table>

                                                            </fieldset>

                                                        </ItemTemplate>


                                                    </telerik:RadListView>


                                                    <asp:ObjectDataSource runat="server" ID="odsSourceMappings" SelectMethod="GetSourceMappings"
                                                        TypeName="Vanrise.Fzero.Bypass.SourceMapping" DeleteMethod="Delete">
                                                        <DeleteParameters>
                                                            <asp:Parameter Name="ID" Type="Int32" />
                                                        </DeleteParameters>
                                                    </asp:ObjectDataSource>
                                                </telerik:RadPageView>
                                                <telerik:RadPageView ID="RadPageViewAddEdit" runat="server">
                                                    <fieldset style="float: left; border: 1px dotted #7B1126">

                                                        <table cellpadding="2" cellspacing="2" style="width: 900px">


                                                            <tr>

                                                                <td>
                                                                    <asp:Label ID="lblColumnNumber" runat="server" Text="Column :"></asp:Label>
                                                                </td>

                                                                <td>

                                                                    <telerik:RadTextBox ID="txtColumnName" runat="server"></telerik:RadTextBox>
                                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvColumnNumber" runat="server" ControlToValidate="txtColumnName" ErrorMessage="Column name should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                                </td>

                                                            </tr>

                                                            <tr>

                                                                <td>
                                                                    <asp:Label ID="lblMappedtoColumnNumber" runat="server" Text="Mapped to Column:"></asp:Label>
                                                                </td>

                                                                <td>

                                                                    <telerik:RadComboBox ID="ddlMappedtoColumnNumber" runat="server"></telerik:RadComboBox>
                                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvMappedtoColumnNumber" runat="server" ControlToValidate="ddlMappedtoColumnNumber" ErrorMessage="Mapped Column should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                                </td>

                                                            </tr>



                                                            <tr>


                                                                <td></td>
                                                                <td style="padding: 30px 0px 0px 0px">
                                                                    <asp:HiddenField ID="hdnId" runat="server" Value="0" />

                                                                    <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" CausesValidation="true" CommandName="Save" OnClick="btnSave_Click">
                                                                 <i class="icon-save icon-white"></i> Save  </asp:LinkButton>



                                                                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" CommandName="Cancel" OnClick="btnCancel_Click">
                                                                 <i class="icon-undo icon-white"></i> Cancel  </asp:LinkButton>



                                                                </td>

                                                            </tr>

                                                        </table>

                                                    </fieldset>
                                                </telerik:RadPageView>

                                            </telerik:RadMultiPage>





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

