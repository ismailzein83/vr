<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="RelatedNumberMappings.aspx.cs" Inherits="RelatedNumberMappings" %>

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
                    <table cellpadding="0" cellspacing="0" width="100%">
                        <tr id="trAddEdit" runat="server">
                            <td class="section">
                                <table width="100%">
                                    <tr>
                                        <td></td>
                                        <td class="top">
                                            <asp:Label ID="lblSectionName" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td></td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td class="caption">
                                                        <%=Resources.Resources.MobileOperator %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlMobileOperator" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlMobileOperator_SelectedIndexChanged"></telerik:RadComboBox>
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
                                                    <td class="caption" valign="Top">
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

                                                                                        <telerik:RadButton ID="btnAddNew" runat="server" CommandName="Insert" Visible="<%# !Container.IsItemInserted %>"
                                                                                            Text="Add new mapping">
                                                                                            <Icon PrimaryIconUrl="~/Icons/add.png" />
                                                                                        </telerik:RadButton>

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

                                                                                    <td class="caption">
                                                                                        <asp:Label ID="lblColumnName" runat="server" Text="Column :"></asp:Label>
                                                                                    </td>

                                                                                    <td style="padding-right: 15px;" align="right">
                                                                                        <%# Eval("ColumnName") %>
                                                                                    </td>

                                                                                </tr>

                                                                                <tr>

                                                                                    <td class="caption">
                                                                                        <asp:Label ID="lblMappedtoColumnNumber" runat="server" Text="Mapped to Column:"></asp:Label>
                                                                                    </td>

                                                                                    <td style="padding-right: 15px;" align="right">

                                                                                        <%# Eval("PredefinedColumnsforRelatedNumber.Name") %>

                                                                                    </td>

                                                                                </tr>


                                                                                <tr>

                                                                                    <td colspan="2">

                                                                                        <telerik:RadButton ID="btnEditItemTemplate" runat="server" CommandArgument='<%# Eval("ID") %>' CommandName="Modify" Text="Modify">
                                                                                            <Icon PrimaryIconUrl="~/Icons/edit.gif" />
                                                                                        </telerik:RadButton>
                                                                                        &nbsp;
                                                                <telerik:RadButton ID="btnDeleteItemTemplate" runat="server" CommandArgument='<%# Eval("ID") %>' CommandName="Delete" Text="Delete">
                                                                    <Icon PrimaryIconUrl="~/Icons/delete.png" />
                                                                </telerik:RadButton>

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

                                                                            <td class="caption">
                                                                                <asp:Label ID="lblColumnNumber" runat="server" Text="Column :"></asp:Label>
                                                                            </td>

                                                                            <td>

                                                                                <telerik:RadTextBox ID="txtColumnName" runat="server"></telerik:RadTextBox>
                                                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvColumnNumber" runat="server" ControlToValidate="txtColumnName" ErrorMessage="Column name should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                                            </td>

                                                                        </tr>

                                                                        <tr>

                                                                            <td class="caption">
                                                                                <asp:Label ID="lblMappedtoColumnNumber" runat="server" Text="Mapped to Column:"></asp:Label>
                                                                            </td>

                                                                            <td>

                                                                                <telerik:RadComboBox ID="ddlMappedtoColumnNumber" runat="server"></telerik:RadComboBox>
                                                                                <asp:RequiredFieldValidator CssClass="error" ID="rfvMappedtoColumnNumber" runat="server" ControlToValidate="ddlMappedtoColumnNumber" ErrorMessage="Mapped Column should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                                            </td>

                                                                        </tr>



                                                                        <tr>

                                                                            <td colspan="2">
                                                                                <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                                                <telerik:RadButton ID="btnSave" Text="Save" runat="server" OnClick="btnSave_Click" ValidationGroup="Save" CausesValidation="true" />
                                                                                <telerik:RadButton ID="btnCancel" OnClick="btnCancel_Click" runat="server" CommandName="Cancel" Text="Cancel"></telerik:RadButton>

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
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>
    </div>
</asp:Content>

