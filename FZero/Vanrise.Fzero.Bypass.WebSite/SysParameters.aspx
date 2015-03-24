<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SysParameters.aspx.cs" Inherits="SysParameters" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <table cellpadding="0" cellspacing="0" width="100%" >
        <tr id="trAddEdit" runat="server" visible="false">
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
                                    <td class="caption"><%=Resources.Resources.Name %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtName" runat="server" ReadOnly="True" Style="width: 750px !important;"></telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="caption"><%=Resources.Resources.Description %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtDescription" runat="server" Width="750px" Style="width: 750px !important;"></telerik:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="caption"><%=Resources.Resources.Type %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadComboBox ID="ddlType" runat="server" Enabled="False">
                                        </telerik:RadComboBox>
                                        <asp:RequiredFieldValidator ID="rfvType" runat="server" ControlToValidate="ddlType" ErrorMessage="Type should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>

                                    </td>
                                </tr>


                                <tr>
                                    <td class="caption"><%=Resources.Resources.Value %></td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtValue" runat="server" Style="width: 750px !important;"></telerik:RadTextBox>
                                        <asp:CheckBox ID="chkBooleanValue" runat="server" Visible="False"></asp:CheckBox>
                                        <telerik:RadComboBox ID="ddlMobileOperators" runat="server" Visible="False">
                                        </telerik:RadComboBox>
                                        <asp:RequiredFieldValidator ID="rfvValue" runat="server" ControlToValidate="txtValue" ErrorMessage="Field should not be empty " CssClass="error" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvMobileOperators" runat="server" ControlToValidate="ddlMobileOperators" ErrorMessage="Field should not be empty " CssClass="error" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>


                                <tr>
                                    <td class="caption">&nbsp;</td>
                                    <td >&nbsp;</td>
                                    <td class=" commands">
                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                        <telerik:RadButton ID="btnSave" runat="server" OnClick="btnSave_Click" CausesValidation="true" ValidationGroup="Save">
                                            <Icon PrimaryIconUrl="Icons/save.png" />
                                        </telerik:RadButton>
                                        <telerik:RadButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" CausesValidation="False">
                                            <Icon PrimaryIconUrl="Icons/cancel.png" />
                                        </telerik:RadButton>
                                    </td>
                                </tr>



                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr id="trSearch">
                        <td valign="top">
                            <table  width="100%">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" >
                                            <tr>
                                                <td >&nbsp;</td>
                                                <td>
                                                    <table>
                                                        <tr>
                                                            <td class="caption">
                                                                <%=Resources.Resources.Text %>
                                                            </td>
                                                            <td ></td>
                                                            <td class="inputdata">
                                                                <telerik:RadTextBox ID="txtSearchText" runat="server"></telerik:RadTextBox>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>



                                            </tr>

                                        </table>
                                        <tr id="trSearchCommands">
                                            <td class="commands">
                                                <table cellpadding="3" cellspacing="2" border="0" align="center">
                                                    <tr>
                                                        <td>
                                                            <telerik:RadButton ID="btnSearch" runat="server" CausesValidation="False"
                                                                OnClick="btnSearch_Click">
                                                                <Icon PrimaryIconUrl="Icons/search_16.png" />
                                                            </telerik:RadButton>
                                                        </td>
                                                        <td class="hspace"></td>
                                                        <td>
                                                            <telerik:RadButton ID="btnSearchClear" runat="server" CausesValidation="False"
                                                                OnClick="btnSearchClear_Click">
                                                                <Icon PrimaryIconUrl="Icons/clear.png" />
                                                            </telerik:RadButton>
                                                        </td>

                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td valign="top">
                            <table width="100%">
                                <tr>
                                    <td>


                                        <telerik:RadGrid AllowSorting="true" ID="gvSysParameters" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                            AllowPaging="true" OnItemCommand="gvSysParameters_ItemCommand"
                                            AutoGenerateColumns="False" OnNeedDataSource="gvSysParameters_NeedDataSource">
                                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />
                                                    <telerik:GridBoundColumn DataField="Description" UniqueName="Description" />
                                                    <telerik:GridBoundColumn DataField="ValueType.Name" UniqueName="ValueType.Name" />
                                                    <telerik:GridBoundColumn DataField="Value" UniqueName="Value" />
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <telerik:RadButton ButtonType="StandardButton" ID="btnEdit" runat="server"
                                                                CommandArgument='<%#Eval("Id")%>' CommandName="Modify" Text='<%# Resources.Resources.Edit %>'  >
                                                                <Icon PrimaryIconUrl="~/Icons/edit.gif"  />
                                                            </telerik:RadButton>

                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
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

