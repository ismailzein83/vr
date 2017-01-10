<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ToBeReportedCases.aspx.cs" Inherits="ToBeReportedCases" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="Controls/wucGeneratedCallInformation.ascx" TagName="wucGeneratedCallInformation" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/Styles/tilestyle.css" rel="stylesheet" />
    <link href="App_Themes/Styles/modalDialog.css" rel="stylesheet" />
    <script type="text/javascript">

        function RowSelecting(sender, args) {

            var UsingColumn = args.getDataKeyValue("ReportingStatusID"); // Column UniqueName

            if (UsingColumn == '2') {
                args.set_cancel(true);

            }

        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <div class="row-fluid" id="divView" runat="server" visible="false">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>View Item</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <uc1:wucGeneratedCallInformation ID="wucGeneratedCallInformation" runat="server" />

                    <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                                                 <i class="icon-undo icon-white"></i> Cancel </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Filters</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table width="100%">
                        <tr id="trSearch">
                            <td valign="top">
                                <table  width="100%">
                                    <tr>
                                        <td>
                                            <table cellpadding="2" cellspacing="2" >

                                                <tr>

                                                    <td>&nbsp;</td>
                                                    <td class="caption">
                                                        <%=Resources.Resources.Status %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchStatus" runat="server"></telerik:RadComboBox>
                                                    </td>



                                                    <td>&nbsp;</td>
                                                    <td class="caption">
                                                        <%=Resources.Resources.MobileOperator %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchMobileOperator" runat="server"></telerik:RadComboBox>
                                                    </td>



                                                    <td>&nbsp;</td>
                                                    <td class="caption">
                                                        <%=Resources.Resources.Client %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchClient" runat="server"></telerik:RadComboBox>
                                                    </td>




                                                </tr>
                                                <tr>
                                                    <td></td>
                                                </tr>

                                                <tr>




                                                    <td>&nbsp;</td>
                                                    <td class="caption">
                                                        <%=Resources.Resources.GeneratedBy %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchSource" runat="server"></telerik:RadComboBox>
                                                    </td>



                                                    <td>&nbsp;</td>

                                                    <td class="caption">
                                                        <%=Resources.Resources.ReceivedBy %>
                                                    </td>
                                                    <td></td>
                                                    <td>
                                                        <telerik:RadComboBox ID="ddlSearchReceivedSource" runat="server"></telerik:RadComboBox>
                                                    </td>


                                                    <td>&nbsp;</td>

                                                    <td class="caption"></td>
                                                    <td></td>
                                                    <td></td>



                                                </tr>
                                                <tr>
                                                    <td colspan="12" align="center" valign="Bottom" style="padding:20px 0px 0px  0px"   >
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
                            </td>
                        </tr>



                    </table>
                </div>
            </div>
        </div>
    </div>










    <div class="row-fluid" id="div1" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Results</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table width="100%">
                        <tr id="tr1">
                            <td valign="top">
                                <table width="100%">
                                    <tr>
                                        <td class="right">
                                            <table width="100%">
                                                <tr>
                                                    <td class="left">
                                                        <asp:TextBox ID="txtRecomnededAction" TextMode="MultiLine" Rows="4" Width="700px" runat="server" Text="It is highly recommended to immediately investigate and trace these international calls and provide us with the respective CDR's of these Fraudulent Calls as they were terminated to your Network and did not pass legally through ITPC's IGW."></asp:TextBox>
                                                    </td>

                                                    <td class="right">&nbsp;</td>

                                                    <td class="right">

                                                        <telerik:RadComboBox ID="ddlReportFormat" runat="server">
                                                            <Items>
                                                                <telerik:RadComboBoxItem runat="server" Text="PDF" Value="PDF" />
                                                                <telerik:RadComboBoxItem runat="server" Text="Excel" Value="Excel" />
                                                            </Items>
                                                        </telerik:RadComboBox>


                                                        <asp:LinkButton ID="btnSendReport" runat="server" CssClass="btn btn-success"  OnClientClick="return confirm('Are you sure you want to send this report?');" OnClick="btnSendReport_Click"   >
                                                                 <i class="icon-arrow-right icon-white"></i> Send </asp:LinkButton>

                                                      
                                                        
                                                    </td>

                                                </tr>
                                            </table>



                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>


                                    <tr>
                                        <td>
                                            <telerik:RadGrid ID="gvGeneratedCalls" ShowGroupPanel="true" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                                AllowPaging="true" OnItemCommand="gvGeneratedCalls_ItemCommand" AllowMultiRowSelection="true" OnItemDataBound="gvGeneratedCalls_ItemDataBound"
                                                AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvGeneratedCalls_NeedDataSource">
                                                <ClientSettings>
                                                    <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                                                    <ClientEvents OnRowSelecting="RowSelecting" />
                                                </ClientSettings>
                                                <MasterTableView DataKeyNames="ID, ReportingStatusID, CLI" ClientDataKeyNames="ID, ReportingStatusID, CLI" ShowGroupFooter="true">
                                                    <Columns>
                                                        <telerik:GridClientSelectColumn UniqueName="Select"></telerik:GridClientSelectColumn>
                                                        <telerik:GridBoundColumn DataField="CaseID" UniqueName="CaseID" Aggregate="Count" />
                                                        <telerik:GridBoundColumn DataField="SourceName" UniqueName="SourceName" />
                                                        <telerik:GridBoundColumn DataField="ReceivedSourceName" UniqueName="ReceivedSourceName" />
                                                        <telerik:GridBoundColumn DataField="DurationInSeconds" UniqueName="DurationInSeconds" />
                                                        <telerik:GridTemplateColumn UniqueName="StatusName">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblStatusName" runat="server" Text='<%# Eval("StatusName").ToString() %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>
                                                        <telerik:GridBoundColumn DataField="b_number" UniqueName="b_number" />
                                                        <telerik:GridBoundColumn DataField="CLI" UniqueName="CLI" />
                                                        <telerik:GridBoundColumn DataField="AttemptDateTime" UniqueName="AttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                        <telerik:GridBoundColumn DataField="ReportID" UniqueName="ReportID" ItemStyle-ForeColor="Red" ItemStyle-Font-Underline="true" ItemStyle-Font-Bold="true" />
                                                        <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                            <ItemTemplate>
                                                                <telerik:RadButton ButtonType="LinkButton" ID="viewButton" runat="server" Text='<%# Resources.Resources.View %>'
                                                                    CommandArgument='<%#Eval("ID") + ";" +Eval("CaseID")+ ";" +Eval("ReportRealID")%>' CommandName="View">
                                                                </telerik:RadButton>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>

                                                        <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                            <ItemTemplate>
                                                                <telerik:RadButton ButtonType="LinkButton" ID="ignoreButton" runat="server" Text='<%# Resources.Resources.Ignore %>'
                                                                    CommandArgument='<%#Eval("ID") + ";" +Eval("CaseID")+ ";" +Eval("ReportRealID")%>' CommandName="Ignore">
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
                </div>
            </div>
        </div>
    </div>























    <rsweb:ReportViewer Visible="false" ID="rvToOperator" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%">
        <LocalReport ReportPath="Reports\rptToOperator.rdlc">
        </LocalReport>
    </rsweb:ReportViewer>
    
        <rsweb:ReportViewer Visible="false" ID="rvToSecurity" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%">
        <LocalReport ReportPath="Reports\rptToOperatorIraqNationalSec.rdlc">
        </LocalReport>
    </rsweb:ReportViewer>


</asp:Content>

