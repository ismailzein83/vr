<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="MobileOperators.aspx.cs" Inherits="MobileOperators" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="Controls/wucMobileOperatorInformation.ascx" TagName="wucMobileOperatorInformation" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/Styles/tilestyle.css" rel="stylesheet" />
    <link href="App_Themes/Styles/modalDialog.css" rel="stylesheet" />

    <script>
        function RemoveRequired() {
            $('.required').removeClass('required');
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">



    <table cellpadding="0" cellspacing="0" width="100%">

        <tr id="trAddEdit" runat="server" visible="false">
            <td valign="top">
                <div class="row-fluid" id="div1" runat="server">
                    <div class="span12">
                        <div class="widget green">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>View Item</h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table width="100%">
                                    <tr>
                                        <td valign="top">


                                            <table cellpadding="0" cellspacing="0" width="100%">
                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <uc1:wucMobileOperatorInformation ID="wucMobileOperatorInformation" runat="server" />
                                                    </td>
                                                    <td></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td></td>
                                                    <td></td>
                                                </tr>
                                            </table>




                                        </td>
                                    </tr>

                                    <tr class="vspace-20">
                                        <td></td>
                                    </tr>

                                    <tr>
                                        <td class=" commands" align="right">

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
                                    <tr>
                                        <td></td>
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
                                            <table width="100%">
                                                <tr>
                                                    <td>
                                                        <table >

                                                            <tr>
                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%=Resources.Resources.Name %>
                                                                </td>
                                                                <td></td>
                                                                <td class="inputdata">
                                                                    <telerik:RadTextBox ID="txtSearchName" runat="server"></telerik:RadTextBox>
                                                                </td>

                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%=Resources.Resources.Email %>
                                                                </td>
                                                                <td></td>
                                                                <td class="inputdata">
                                                                    <telerik:RadTextBox ID="txtSearchEmailAddress" runat="server"></telerik:RadTextBox>
                                                                </td>


                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%=Resources.Resources.NumberPrefixes %>
                                                                </td>
                                                                <td></td>
                                                                <td class="inputdata">
                                                                    <telerik:RadTextBox ID="txtSearchNumberPrefixes" runat="server"></telerik:RadTextBox>
                                                                </td>



                                                               

                                                            </tr>

                                                            <tr>
                                                               <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%=Resources.Resources.Website %>
                                                                </td>
                                                                <td></td>
                                                                <td class="inputdata">
                                                                    <telerik:RadTextBox ID="txtSearchWebsite" runat="server"></telerik:RadTextBox>
                                                                </td>



                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                    <%=Resources.Resources.GMT %>
                                                                </td>
                                                                <td></td>
                                                                <td class="inputdata">
                                                                    <telerik:RadComboBox ID="ddlSearchGMT" runat="server"></telerik:RadComboBox>
                                                                </td>



                                                                <td>&nbsp;</td>
                                                                <td class="caption">
                                                                </td>
                                                                <td></td>
                                                                <td class="inputdata">
                                                                </td>

                                                            </tr>
                                                            <tr>
                                                                <td colspan="12" align="center" style="padding: 20px 0px 0px 0px">

                                                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>



                                                                    <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-danger" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>





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

                    <tr id="tr1">
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
                                                        <telerik:RadGrid AllowSorting="true" ID="gvMobileOperators" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                                            AllowPaging="true" OnItemCommand="gvMobileOperators_ItemCommand" OnItemDataBound="gvMobileOperators_ItemDataBound" ClientSettings-Selecting-AllowRowSelect="true"
                                                            AutoGenerateColumns="False" OnNeedDataSource="gvMobileOperators_NeedDataSource">
                                                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                                                <Columns>
                                                                    <telerik:GridBoundColumn DataField="User.FullName" UniqueName="User.FullName" />
                                                                    <telerik:GridBoundColumn DataField="User.EmailAddress" UniqueName="User.EmailAddress" />
                                                                    <telerik:GridBoundColumn DataField="User.UserName" UniqueName="User.UserName" />
                                                                    <telerik:GridBoundColumn DataField="User.LastLoginTime" UniqueName="User.LastLoginTime" />

                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="45%">
                                                                        <ItemTemplate>
                                                                            <asp:TextBox TextMode="MultiLine" Width="98%" Enabled="false" BorderColor="White" ID="lblPrefixes" runat="server" Text='<%#Eval("User.Prefix")%>' />
                                                                        </ItemTemplate>
                                                                    </telerik:GridTemplateColumn>



                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                                        <ItemTemplate>
                                                                            <telerik:RadButton ButtonType="LinkButton" ID="viewButton" runat="server"
                                                                                CommandArgument='<%#Eval("Id") + ";" +Eval("UserId")+ ";" +Eval("User.FullName")%>' CommandName="View" Text='<%# Resources.Resources.View %>'>
                                                                            </telerik:RadButton>
                                                                        </ItemTemplate>
                                                                    </telerik:GridTemplateColumn>


                                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                                        <ItemTemplate>
                                                                            <telerik:RadButton ButtonType="LinkButton" ID="modifyButton" runat="server"
                                                                                CommandArgument='<%#Eval("Id") + ";" +Eval("UserId")+ ";" +Eval("User.FullName")%>' CommandName="Modify" Text='<%# Resources.Resources.Edit %>'>
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

