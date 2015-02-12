<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EmailCCs.aspx.cs" Inherits="EmailCCs" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/wucChangePassword.ascx" TagPrefix="uc1" TagName="wucChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table cellpadding="0" cellspacing="0" width="100%">


        <tr id="trAddEdit" runat="server" visible="false">
            <td class="section">

                <div class="row-fluid" id="div1" runat="server">
                    <div class="span12">
                        <div class="widget green">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Manage Item</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
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
                                                    <td class="caption"><%=Resources.Resources.MobileOperator %></td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlMobileOperator" runat="server"></telerik:RadComboBox>
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td class="caption"><%=Resources.Resources.Email %></td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadTextBox ID="txtEmailAddress" runat="server"></telerik:RadTextBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvEmailAddress" runat="server" Display="Dynamic" ControlToValidate="txtEmailAddress" ErrorMessage="Email Address should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                        <asp:RegularExpressionValidator CssClass="error" ID="revEmailAddress" runat="server" Display="Dynamic" ControlToValidate="txtEmailAddress" ErrorMessage=" Invalid email address" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>


                                                <tr>
                                                    <td class="caption"><%=Resources.Resources.Client %></td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlClients" runat="server"></telerik:RadComboBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvClients" runat="server" Display="Dynamic" ControlToValidate="ddlClients" ErrorMessage="Client should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>




                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                    <td class=" commands">
                                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                        <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />
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
                    <tr id="trSearch">
                        <td valign="top">
                             <div class="row-fluid" id="divFilter" runat="server">
                                <div class="span12">
                                    <div class="widget gray">
                                        <div class="widget-title">
                                            <h4><i class="icon-reorder"></i> Filters</h4>

                                        </div>
                                        <div class="widget-body" style="display: block;">
                            <table  width="100%">
                                <tr>
                                    <td>
                                        <table  >
                                            <tr>
                                                <td>&nbsp;</td>
                                                <td class="caption">
                                                    <%= Resources.Resources.MobileOperator %>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <telerik:RadComboBox ID="ddlSearchMobileOperator" runat="server"></telerik:RadComboBox>
                                                </td>

                                                <td ></td>

                                                <td class="caption">
                                                    <%= Resources.Resources.Email %>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <telerik:RadTextBox ID="txtSearchEmailAddress" runat="server"></telerik:RadTextBox>
                                                </td>



                                                <td ></td>

                                                <td class="caption">
                                                    <%= Resources.Resources.Client %>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <telerik:RadComboBox ID="ddlSearchClients" runat="server"></telerik:RadComboBox>
                                                </td>


                                            </tr>
                                            <tr>
                                                <td colspan="12" align="center" style="padding: 20px 0px 0px 0px" >

                                                                            <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>



                                                                            <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-danger" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>


                                                                            <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-success"  OnClick="btnAddNew_Click">
                                                                 <i class="icon-plus icon-white"></i> Add  </asp:LinkButton>



                                                                        </td>
                                            </tr>



                                        </table>
                                    </td>
                                </tr>
                            </table></div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                 
                    <tr>
                        <td valign="top">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <div class="row-fluid" id="divData" runat="server">
                                            <div class="span12">
                                                <div class="widget blue">
                                                    <div class="widget-title">
                                                        <h4><i class="icon-reorder"></i> Results</h4>

                                                    </div>
                                                    <div class="widget-body" style="display: block;">
                                        <telerik:RadGrid ID="gvEmailCCs" AllowSorting="true" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                            AllowPaging="true" OnItemCommand="gvEmailCCs_ItemCommand"
                                            AutoGenerateColumns="False" OnNeedDataSource="gvEmailCCs_NeedDataSource" OnItemDataBound="gvEmailCCs_ItemDataBound">




                                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="Client.Name" UniqueName="Client.Name" />
                                                    <telerik:GridBoundColumn DataField="Email" UniqueName="Email" />
                                                    <telerik:GridBoundColumn DataField="MobileOperator.User.FullName" UniqueName="Name" AllowSorting="false" />
                                                    <telerik:GridBoundColumn DataField="Id" Visible="false" UniqueName="Id" />
                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <telerik:RadButton ButtonType="LinkButton" ID="modifyButton" runat="server"
                                                                CommandArgument='<%#Eval("Id") %>' CommandName="Modify" Text='<%# Resources.Resources.Edit %>'>
                                                            </telerik:RadButton>

                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>


                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <telerik:RadButton ButtonType="LinkButton" ID="deleteButton" runat="server"
                                                                CommandArgument='<%#Eval("Id") %>' CommandName="Remove"
                                                                OnClientClick="return confirm('Are you sure you want to delete ?');" Text='<%# Resources.Resources.Delete %>'>
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

