<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="EmailReceivers.aspx.cs" Inherits="EmailReceivers" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/wucChangePassword.ascx" TagPrefix="uc1" TagName="wucChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table cellpadding="0" cellspacing="0" width="100%">


        <tr id="trAddEdit" runat="server" visible="false">
            <td  >
                <div class="row-fluid" id="div1" runat="server">
                    <div class="span12">
                        <div class="widget green">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Manage</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table width="100%">


                                    <tr>
                                        <td></td>
                                        <td>
                                            <table cellpadding="2" cellspacing="2">


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
                                                    <td class="caption">Receiver Type</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlReceiverType" runat="server"></telerik:RadComboBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvReceiverType" runat="server" Display="Dynamic" ControlToValidate="ddlReceiverType" ErrorMessage="Receiver type should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>


                                                <tr>
                                                    <td class="caption">Email Template</td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlEmailTemplate" runat="server"></telerik:RadComboBox>
                                                        <asp:RequiredFieldValidator CssClass="error" ID="rfvEmailTemplate" runat="server" Display="Dynamic" ControlToValidate="ddlEmailTemplate" ErrorMessage="Email template should not be empty " ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>







                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                    <td class=" commands">
                                                        <asp:HiddenField ID="hdnId" runat="server" Value="0" />
                                                        <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />

                                                        <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-success" OnClick="btnSave_Click" ValidationGroup="Save">
                                                                 <i class="icon-save icon-white"></i> Save </asp:LinkButton>

                                                        <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-inverse" OnClick="btnCancel_Click">
                                                                 <i class="icon-undo icon-white"></i> Return </asp:LinkButton>



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
                                <table width="100%">
                                    <tr id="trSearch">
                                        <td valign="top">
                                            <table class="search" width="100%">
                                                <tr>
                                                    <td>
                                                        <table cellpadding="2" cellspacing="2">
                                                            <tr>


                                                                <td>&nbsp;</td>

                                                                <td class="caption">Email
                                                                </td>
                                                                <td>
                                                                    <telerik:RadTextBox ID="txtSearchEmailAddress" runat="server"></telerik:RadTextBox>
                                                                </td>



                                                                <td>&nbsp;</td>

                                                                <td class="caption">Receiver Type
                                                                </td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="ddlSearchReceiverType" runat="server"></telerik:RadComboBox>
                                                                </td>



                                                                <td>&nbsp;</td>

                                                                <td class="caption">Email Template
                                                                </td>
                                                                <td>
                                                                    <telerik:RadComboBox ID="ddlSearchEmailTemplate" runat="server"></telerik:RadComboBox>
                                                                </td>



                                                            </tr>

                                                            <tr>
                                                                <td colspan="9" align="center" style="padding: 30px 0px 0px 0px">
                                                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>

                                                                    <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-inverse" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-refresh icon-white"></i> Clear </asp:LinkButton>


                                                                    <asp:LinkButton ID="btnAddNew" runat="server" CssClass="btn btn-success" OnClick="btnAddNew_Click" CausesValidation="False">
                                                                     <i class="icon-plus icon-white"></i> Add New </asp:LinkButton>


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

                <div class="row-fluid" id="divData" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Results</h4>

                </div>
                <div class="widget-body" style="display: block;">

                    <telerik:RadGrid ID="gvEmailReceivers" AllowSorting="true" runat="server" CellSpacing="0" PageSize="10" 
                        AllowPaging="true" OnItemCommand="gvEmailReceivers_ItemCommand" ClientSettings-Scrolling-AllowScroll="true"
                        AutoGenerateColumns="False" OnNeedDataSource="gvEmailReceivers_NeedDataSource" >

                        <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                            <Columns>
                                <telerik:GridBoundColumn DataField="EmailTemplate.Name" UniqueName="EmailTemplate.Name" />
                                <telerik:GridBoundColumn DataField="EmailReceiverType.Name" UniqueName="EmailReceiverType.Name" />
                                <telerik:GridBoundColumn DataField="Email" UniqueName="Email" />
                                <telerik:GridBoundColumn DataField="Id" Visible="false" UniqueName="Id" />


                                <telerik:GridTemplateColumn  ItemStyle-Width="30" HeaderText="Edit" >
                                    <ItemTemplate >
                                        <asp:LinkButton ID="btnEdit" ToolTip="Edit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("Id")%>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                        </asp:LinkButton>
                                       
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>



                                 <telerik:GridTemplateColumn ItemStyle-Width="30" HeaderText="Delete"  >
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" ToolTip="Delete" runat="server" CssClass="command btn-danger"
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Remove"
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

