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



    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Sent Emails</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table cellpadding="2" cellspacing="2" >
                        <tr>
                            <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="caption">From Date
                            </td>
                            <td >
                                <telerik:RadDatePicker ID="rdpFromDate" runat="server"></telerik:RadDatePicker>
                            </td>
                             <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="caption">To Date
                            </td>
                            <td >
                                <telerik:RadDatePicker ID="rdpToDate" runat="server"></telerik:RadDatePicker>
                            </td>
                             <td>&nbsp;</td>
                            <td>&nbsp;</td>
                            <td class="caption">Email
                            </td>
                            <td >
                                <telerik:RadTextBox ID="txtEmail" runat="server" AutoPostBack="true"></telerik:RadTextBox>
                            </td>

                        </tr>

                        <tr>
                            <td colspan="12">


                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>



                                <asp:LinkButton ID="btnSearchCancel" runat="server" CssClass="btn btn-inverse" OnClick="btnSearchCancel_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>


                                <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger" OnClientClicking="ConfirmDelete" OnClick="btnDelete_Click">
                                                                 <i class="icon-undo icon-white"></i> Delete </asp:LinkButton>


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
                    <h4><i class="icon-reorder"></i>Sent Emails</h4>

                </div>
                <div class="widget-body" style="display: block;">

                    <telerik:RadGrid ID="gvEmails" AllowSorting="true" runat="server" CellSpacing="0" PageSize="10" BorderColor="LightGray"
                        AllowPaging="true" ClientSettings-Selecting-AllowRowSelect="true"
                        AutoGenerateColumns="False" OnNeedDataSource="gvEmails_NeedDataSource" Skin="Metro">

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
                </div>
            </div>
        </div>
    </div>





</asp:Content>

