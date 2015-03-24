<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="LoggedActions.aspx.cs" Inherits="LoggedActions" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <table cellpadding="0" cellspacing="0" width="100%">
        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr id="trSearch">
                        <td valign="top">


                            <div class="row-fluid" id="divFilter" runat="server">
                                <div class="span12">
                                    <div class="widget gray">
                                        <div class="widget-title">
                                            <h4><i class="icon-reorder"></i>Search Filters</h4>

                                        </div>
                                        <div class="widget-body" style="display: block;">


                                            <table width="100%">
                                                <tr>
                                                    <td>&nbsp;</td>

                                                    <td>
                                                        <%=Resources.Resources.Action %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchActions" runat="server">
                                                        </telerik:RadComboBox>
                                                    </td>


                                                    <td></td>







                                                    <td>&nbsp;</td>

                                                    <td>
                                                        <%=Resources.Resources.By %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchActionBy" runat="server">
                                                        </telerik:RadComboBox>
                                                    </td>

                                                    <td></td>


                                                </tr>


                                                <tr>




                                                    <td>&nbsp;</td>

                                                    <td>
                                                        <%=Resources.Resources.From %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadDatePicker ID="rdpFromLogDate" runat="server"></telerik:RadDatePicker>
                                                    </td>


                                                    <td></td>






                                                    <td>&nbsp;</td>

                                                    <td>
                                                        <%=Resources.Resources.To %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadDatePicker ID="rdpToLogDate" runat="server"></telerik:RadDatePicker>
                                                    </td>
                                                    <td></td>

                                                </tr>


                                                <tr>
                                                    <td colspan="10" style="padding: 30px 0px 0px 0px" align="center">
                                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>



                                                        <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-danger" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>


                                                        <asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_Click" CssClass="btn btn-inverse"
                                                            OnClientClick="return confirm('Are you sure you want delete');" >
                                                                        <i class="icon-trash icon-white"></i> Delete </asp:LinkButton>




                                                    </td>

                                                </tr>



                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td valign="top">
                            <table width="100%" cellpadding="2" cellspacing="2">


                                <tr>
                                    <td>

                                        <div class="row-fluid" id="divData" runat="server">
                                            <div class="span12">
                                                <div class="widget blue">
                                                    <div class="widget-title">
                                                        <h4><i class="icon-reorder"></i>Search Results</h4>

                                                    </div>
                                                    <div class="widget-body" style="display: block;">
                                                        <telerik:RadGrid AllowSorting="true" ID="gvLoggedActions" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                                            AllowMultiRowSelection="true" AllowPaging="true"
                                                            AutoGenerateColumns="False" OnNeedDataSource="gvLoggedActions_NeedDataSource">

                                                            <ClientSettings>
                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                            </ClientSettings>

                                                            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                                                                <Columns>
                                                                    <telerik:GridClientSelectColumn></telerik:GridClientSelectColumn>
                                                                    <telerik:GridBoundColumn DataField="ID" UniqueName="ID" Visible="false" />
                                                                    <telerik:GridBoundColumn DataField="ActionType.Name" UniqueName="ActionType.Name" />
                                                                    <telerik:GridBoundColumn DataField="LogDate" UniqueName="LogDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                    <telerik:GridBoundColumn DataField="User.FullName" UniqueName="FullName" />
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

