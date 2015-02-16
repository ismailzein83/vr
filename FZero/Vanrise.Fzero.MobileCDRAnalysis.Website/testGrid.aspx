<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="testGrid.aspx.cs" Inherits="testGrid" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/wucChangePassword.ascx" TagPrefix="uc1" TagName="wucChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

 <telerik:RadGrid ID="gvEmailReceivers" AllowSorting="true" runat="server" CellSpacing="0" PageSize="10"  SkinID="None"
                        AllowPaging="true" OnItemCommand="gvEmailReceivers_ItemCommand" ClientSettings-Scrolling-AllowScroll="true"
                        AutoGenerateColumns="False" OnNeedDataSource="gvEmailReceivers_NeedDataSource" >

                        <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                            <Columns>
                                <telerik:GridBoundColumn DataField="EmailTemplate.Name" UniqueName="EmailTemplate.Name" HeaderText="Email Template" />
                                <telerik:GridBoundColumn DataField="EmailReceiverType.Name" UniqueName="EmailReceiverType.Name"  HeaderText="Receiver Type" />
                                <telerik:GridBoundColumn DataField="Email" UniqueName="Email"  HeaderText="Email" />
                                <telerik:GridBoundColumn DataField="Id" Visible="false" UniqueName="Id" />


                               <telerik:GridTemplateColumn  ItemStyle-Width="30" HeaderText="Edit" >
                                    <ItemTemplate >
                                        <asp:Button ID="btnEdit" ToolTip="Edit" runat="server"  CommandArgument='<%#Eval("Id")%>' CommandName="Modify">
                                        </asp:Button>
                                       
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>



                               <%--  <telerik:GridTemplateColumn ItemStyle-Width="30" HeaderText="Delete"  >
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnDelete" ToolTip="Delete" runat="server" CssClass="command btn-danger"
                                            CommandArgument='<%#Eval("Id")%>' CommandName="Remove"
                                            OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>--%>


                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>




    









</asp:Content>

