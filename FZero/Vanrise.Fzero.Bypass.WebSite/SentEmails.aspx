<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="SentEmails.aspx.cs" Inherits="SentEmails" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript">
        function ConfirmDelete(sender, args) {
            args.set_cancel(!window.confirm("Please confirm,  Checked emails will be deleted?"));
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">
    <table cellpadding="0" cellspacing="0" width="100%" >
        <tr id="trSearch">
            <td valign="top">
                <table  width="100%">
                    <tr>
                        <td>
                            <table cellpadding="0" cellspacing="0" >
                                <tr>
                                    <td >&nbsp;</td>
                                    <td class="caption">
                                        <%=Resources.Resources.FromDate %>
                                    </td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadDatePicker ID="rdpFromDate" runat="server"></telerik:RadDatePicker>
                                    </td>
                                    <td ></td>
                                    <td class="caption">
                                        <%=Resources.Resources.ToDate %>
                                    </td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadDatePicker ID="rdpToDate" runat="server"></telerik:RadDatePicker>
                                    </td>
                              
                                    <td >&nbsp;</td>
                                    <td class="caption">
                                        <%=Resources.Resources.Email %>
                                    </td>
                                    <td ></td>
                                    <td class="inputdata">
                                        <telerik:RadTextBox ID="txtEmail" runat="server" AutoPostBack="true"></telerik:RadTextBox>
                                    </td>
                                   
                                </tr>
                            </table>
                            <tr id="trSearchCommands">
                                <td class="commands">
                                    <table cellpadding="3" cellspacing="2" border="0" align="center">
                                        <tr>
                                            <td>
                                                <telerik:RadButton ID="btnSearch" runat="server" Text="Search" CausesValidation="False"
                                                    OnClick="btnSearch_Click" >
                                                    <Icon PrimaryIconUrl="~/Icons/search_16.png"  />
                                                </telerik:RadButton>
                                            </td>
                                            <td class="hspace"></td>
                                            <td>
                                                <telerik:RadButton ID="btnSearchCancel" runat="server" Text="Clear" CausesValidation="False"
                                                    OnClick="btnSearchCancel_Click" >
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
            <td style="width: 100%" dir="rtl">
                <table width="100%"  cellpadding="2" cellspacing="2">

                    <tr>
                        <td style="width: 100%" dir="rtl">
                              <asp:LinkButton ID="btnDelete" runat="server" OnClick="btnDelete_Click" CssClass="btn btn-inverse"
                                                            OnClientClick="return confirm('Are you sure you want delete');" >
                                                                        <i class="icon-trash icon-white"></i> Delete </asp:LinkButton>


                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadGrid  AllowSorting="true" ID="gvEmails" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                    AllowMultiRowSelection="true" AllowPaging="true"
                    AutoGenerateColumns="False" OnNeedDataSource="gvEmails_NeedDataSource">

                    <ClientSettings>
                        <Selecting AllowRowSelect="True"></Selecting>
                    </ClientSettings>

                    <MasterTableView DataKeyNames="ID" ClientDataKeyNames="Id">
                        <Columns>
                            <telerik:GridClientSelectColumn></telerik:GridClientSelectColumn>
                            <telerik:GridBoundColumn DataField="ID" UniqueName="ID" Visible="false" />
                            <telerik:GridBoundColumn DataField="DestinationEmail" UniqueName="DestinationEmail" />
                            <telerik:GridBoundColumn DataField="Subject" UniqueName="Subject" />
                            <telerik:GridBoundColumn DataField="IsSent" UniqueName="IsSent" />
                            <telerik:GridBoundColumn DataField="SentDate" UniqueName="SentDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>


</asp:Content>

