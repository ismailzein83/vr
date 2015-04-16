<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="testGrid.aspx.cs" Inherits="testGrid" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Src="~/Controls/wucChangePassword.ascx" TagPrefix="uc1" TagName="wucChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    
    <telerik:RadListView ID="RadListView1" runat="server" DataKeyNames="Id" DataSourceID="SqlDataSource1" Skin="Metro">
        <LayoutTemplate>
            <div class="RadListView RadListView_Metro">
                <div id="itemPlaceholder" runat="server">
                </div>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <div class="rlvI">
                &nbsp;<asp:Label ID="IdLabel" runat="server" Text='<%# Eval("Id") %>' />
                &nbsp;<asp:Label ID="StrategyIdLabel" runat="server" Text='<%# Eval("StrategyId") %>' />
                &nbsp;<asp:Label ID="CriteriaId1Label" runat="server" Text='<%# Eval("CriteriaId1") %>' />
                &nbsp;<asp:Label ID="CriteriaId2Label" runat="server" Text='<%# Eval("CriteriaId2") %>' />
                &nbsp;<asp:Label ID="CriteriaId3Label" runat="server" Text='<%# Eval("CriteriaId3") %>' />
                &nbsp;<asp:Label ID="CriteriaId4Label" runat="server" Text='<%# Eval("CriteriaId4") %>' />
                &nbsp;<asp:Label ID="CriteriaId5Label" runat="server" Text='<%# Eval("CriteriaId5") %>' />
                &nbsp;<asp:Label ID="CriteriaId6Label" runat="server" Text='<%# Eval("CriteriaId6") %>' />
            </div>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <div class="rlvA">
                &nbsp;<asp:Label ID="IdLabel" runat="server" Text='<%# Eval("Id") %>' />
                &nbsp;<asp:Label ID="StrategyIdLabel" runat="server" Text='<%# Eval("StrategyId") %>' />
                &nbsp;<asp:Label ID="CriteriaId1Label" runat="server" Text='<%# Eval("CriteriaId1") %>' />
                &nbsp;<asp:Label ID="CriteriaId2Label" runat="server" Text='<%# Eval("CriteriaId2") %>' />
                &nbsp;<asp:Label ID="CriteriaId3Label" runat="server" Text='<%# Eval("CriteriaId3") %>' />
                &nbsp;<asp:Label ID="CriteriaId4Label" runat="server" Text='<%# Eval("CriteriaId4") %>' />
                &nbsp;<asp:Label ID="CriteriaId5Label" runat="server" Text='<%# Eval("CriteriaId5") %>' />
                &nbsp;<asp:Label ID="CriteriaId6Label" runat="server" Text='<%# Eval("CriteriaId6") %>' />
            </div>
        </AlternatingItemTemplate>
        <EditItemTemplate>
            <div class="rlvIEdit">
                <table cellspacing="0" class="rlvEditTable">
                    <tr>
                        <td>
                            <asp:Label ID="IdLabel2" runat="server" Text="Id"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="IdLabel1" runat="server" Text='<%# Eval("Id") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="StrategyIdLabel2" runat="server" AssociatedControlID="StrategyIdTextBox" Text="StrategyId"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="StrategyIdTextBox" runat="server" DataType="Int32" DbValue='<%# Bind("StrategyId") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId1Label2" runat="server" AssociatedControlID="CriteriaId1TextBox" Text="CriteriaId1"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId1TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId1") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId2Label2" runat="server" AssociatedControlID="CriteriaId2TextBox" Text="CriteriaId2"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId2TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId2") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId3Label2" runat="server" AssociatedControlID="CriteriaId3TextBox" Text="CriteriaId3"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId3TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId3") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId4Label2" runat="server" AssociatedControlID="CriteriaId4TextBox" Text="CriteriaId4"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId4TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId4") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId5Label2" runat="server" AssociatedControlID="CriteriaId5TextBox" Text="CriteriaId5"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId5TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId5") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId6Label2" runat="server" AssociatedControlID="CriteriaId6TextBox" Text="CriteriaId6"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId6TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId6") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                </table>
            </div>
        </EditItemTemplate>
        <InsertItemTemplate>
            <div class="rlvIEdit">
                <table cellspacing="0" class="rlvEditTable">
                    <tr>
                        <td>
                            <asp:Label ID="StrategyIdLabel2" runat="server" AssociatedControlID="StrategyIdTextBox" Text="StrategyId"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="StrategyIdTextBox" runat="server" DataType="Int32" DbValue='<%# Bind("StrategyId") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId1Label2" runat="server" AssociatedControlID="CriteriaId1TextBox" Text="CriteriaId1"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId1TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId1") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId2Label2" runat="server" AssociatedControlID="CriteriaId2TextBox" Text="CriteriaId2"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId2TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId2") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId3Label2" runat="server" AssociatedControlID="CriteriaId3TextBox" Text="CriteriaId3"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId3TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId3") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId4Label2" runat="server" AssociatedControlID="CriteriaId4TextBox" Text="CriteriaId4"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId4TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId4") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId5Label2" runat="server" AssociatedControlID="CriteriaId5TextBox" Text="CriteriaId5"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId5TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId5") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="CriteriaId6Label2" runat="server" AssociatedControlID="CriteriaId6TextBox" Text="CriteriaId6"></asp:Label>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox ID="CriteriaId6TextBox" runat="server" DataType="Int32" DbValue='<%# Bind("CriteriaId6") %>' NumberFormat-DecimalDigits="0" Skin="<%#Container.OwnerListView.Skin %>" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Button ID="PerformInsertButton" runat="server" CommandName="PerformInsert" CssClass="rlvBAdd" Text=" " ToolTip="Insert" />
                            <asp:Button ID="CancelButton" runat="server" CausesValidation="False" CommandName="Cancel" CssClass="rlvBCancel" Text=" " ToolTip="Cancel" />
                        </td>
                    </tr>
                </table>
            </div>
        </InsertItemTemplate>
        <EmptyDataTemplate>
            <div class="RadListView RadListView_Metro">
                <div class="rlvEmpty">
                    There are no items to be displayed.</div>
            </div>
        </EmptyDataTemplate>
        <SelectedItemTemplate>
            <div class="rlvISel">
                &nbsp;<asp:Label ID="IdLabel" runat="server" Text='<%# Eval("Id") %>' />
                &nbsp;<asp:Label ID="StrategyIdLabel" runat="server" Text='<%# Eval("StrategyId") %>' />
                &nbsp;<asp:Label ID="CriteriaId1Label" runat="server" Text='<%# Eval("CriteriaId1") %>' />
                &nbsp;<asp:Label ID="CriteriaId2Label" runat="server" Text='<%# Eval("CriteriaId2") %>' />
                &nbsp;<asp:Label ID="CriteriaId3Label" runat="server" Text='<%# Eval("CriteriaId3") %>' />
                &nbsp;<asp:Label ID="CriteriaId4Label" runat="server" Text='<%# Eval("CriteriaId4") %>' />
                &nbsp;<asp:Label ID="CriteriaId5Label" runat="server" Text='<%# Eval("CriteriaId5") %>' />
                &nbsp;<asp:Label ID="CriteriaId6Label" runat="server" Text='<%# Eval("CriteriaId6") %>' />
            </div>
        </SelectedItemTemplate>
    </telerik:RadListView>
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:CDRAnalysisMobileConnectionString %>" SelectCommand="SELECT [Id], [StrategyId], [CriteriaId1], [CriteriaId2], [CriteriaId3], [CriteriaId4], [CriteriaId5], [CriteriaId6] FROM [Strategy_Suspicion_Level]"></asp:SqlDataSource>



    









</asp:Content>

