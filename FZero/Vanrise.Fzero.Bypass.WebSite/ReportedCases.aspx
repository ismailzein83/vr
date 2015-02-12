<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ReportedCases.aspx.cs" Inherits="ReportedCases" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="Controls/wucGeneratedCallInformation.ascx" TagName="wucGeneratedCallInformation" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="App_Themes/Styles/tilestyle.css" rel="stylesheet" />
    <link href="App_Themes/Styles/modalDialog.css" rel="stylesheet" />
    <script type="text/javascript">

        function RowSelecting(sender, args) {

            var MobileOperatorFeedbackID = args.getDataKeyValue("ReportingStatusID"); // Verified

            if (MobileOperatorFeedbackID == '5') {
                args.set_cancel(true);

            }

        }


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Filters</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table width="100%">
                        <tr>

                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.DateRange %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadComboBox ID="ddlDateRange" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDateRange_SelectedIndexChanged">
                                    <Items>
                                        <telerik:RadComboBoxItem Value="0" Text="Specific Date"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Selected="True" Value="1" Text="Today"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Value="2" Text="Yesterday"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Value="3" Text="This Week"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Value="4" Text="This Month"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Value="5" Text="This Year"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Value="6" Text="Last Week"></telerik:RadComboBoxItem>
                                        <telerik:RadComboBoxItem Value="7" Text="Last Month"></telerik:RadComboBoxItem>
                                    </Items>
                                </telerik:RadComboBox>
                            </td>


                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.Feedback %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadComboBox ID="ddlSearchOperatorReply" runat="server"></telerik:RadComboBox>
                            </td>



                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.ReportID %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadTextBox ID="txtSearchReportID" runat="server"></telerik:RadTextBox>
                            </td>

                        </tr>
                        <tr>


                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.FromDate %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadDateTimePicker ID="rdtpSearchFromAttemptDate" runat="server" AutoPostBackControl="Both" OnSelectedDateChanged="rdtpSearchFromAttemptDate_SelectedDateChanged">
                                    <TimeView CellSpacing="-1"></TimeView>

                                    <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

                                    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

                                    <DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" EnableSingleInputRendering="True" LabelWidth="64px" AutoPostBack="True"></DateInput>

                                    <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                                </telerik:RadDateTimePicker>
                            </td>


                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.b_number %>
                            </td>
                            <td></td>
                            <td>

                                <telerik:RadTextBox ID="txtSearchb_number" runat="server"></telerik:RadTextBox>
                            </td>



                            <td>&nbsp;</td>
                            <td class="caption"><%=Resources.Resources.CLI %></td>
                            <td></td>
                            <td>
                                <telerik:RadTextBox ID="txtSearchCLIReceived" runat="server"></telerik:RadTextBox></td>



                        </tr>
                        <tr>



                            <td>&nbsp;</td>

                            <td class="caption">
                                <%=Resources.Resources.ToDate %>
                            </td>
                            <td></td>
                            <td>

                                <telerik:RadDateTimePicker ID="rdtpSearchToAttemptDate" runat="server" AutoPostBackControl="Both" OnSelectedDateChanged="rdtpSearchToAttemptDate_SelectedDateChanged">
                                    <TimeView CellSpacing="-1"></TimeView>

                                    <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

                                    <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

                                    <DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" EnableSingleInputRendering="True" LabelWidth="64px" AutoPostBack="True"></DateInput>

                                    <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                                </telerik:RadDateTimePicker>
                            </td>




                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.Action %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadComboBox ID="ddlSearchRecommendedAction" runat="server"></telerik:RadComboBox>
                            </td>


                            <td>&nbsp;</td>
                            <td class="caption">
                                <%=Resources.Resources.MobileOperator %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadComboBox ID="ddlSearchMobileOperator" runat="server"></telerik:RadComboBox>
                            </td>








                        </tr>
                        <tr>



                            <td>&nbsp;</td>

                            <td class="caption">
                                <%=Resources.Resources.CaseID %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadTextBox ID="txtSearchCaseID" runat="server"></telerik:RadTextBox>
                            </td>




                            <td>&nbsp;</td>
                            <td class="caption"><%=Resources.Resources.Client %>
                            </td>
                            <td></td>
                            <td>
                                <telerik:RadComboBox ID="ddlSearchClient" runat="server"></telerik:RadComboBox>
                            </td>


                            <td>&nbsp;</td>
                            <td class="caption"></td>
                            <td></td>
                            <td></td>


                        </tr>
                        <tr>
                            <td colspan="12" align="center">
                                <asp:LinkButton ID="btnExport" runat="server" CssClass="btn btn-inverse" OnClick="btnExport_Click">
                                                                 <i class="icon-barcode icon-white"></i> Export </asp:LinkButton>



                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>



                                <asp:LinkButton ID="btnSearchClear" runat="server" CssClass="btn btn-danger" OnClick="btnSearchClear_Click">
                                                                 <i class="icon-undo icon-white"></i> Clear </asp:LinkButton>

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
                    <h4><i class="icon-reorder"></i>Search Results</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <telerik:RadGrid ID="gvGeneratedCalls" ShowGroupPanel="true" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                        AllowPaging="true" OnItemCommand="gvGeneratedCalls_ItemCommand" AllowMultiRowSelection="true"
                        AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvGeneratedCalls_NeedDataSource">
                        <ClientSettings>
                            <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                            <ClientEvents OnRowSelecting="RowSelecting" />
                        </ClientSettings>
                        <MasterTableView DataKeyNames="ID, MobileOperatorFeedbackID, CaseID, ReportRealID,ReportingStatusID" ClientDataKeyNames="ID, MobileOperatorFeedbackID, CaseID, ReportRealID,ReportingStatusID" ShowGroupFooter="true">
                            <Columns>
                                <telerik:GridClientSelectColumn UniqueName="Select"></telerik:GridClientSelectColumn>
                                <telerik:GridBoundColumn DataField="ReportRealID" UniqueName="ReportRealID" />
                                <telerik:GridBoundColumn DataField="ReportingStatusID" UniqueName="ReportingStatusID" Visible="false" />
                                <telerik:GridBoundColumn DataField="CaseID" UniqueName="CaseID" Aggregate="Count" />
                                <telerik:GridBoundColumn DataField="AttemptDateTime" UniqueName="AttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />

                                <telerik:GridBoundColumn DataField="b_number" UniqueName="b_number" />
                                <telerik:GridBoundColumn DataField="CLI" UniqueName="CLI" />
                                <telerik:GridBoundColumn DataField="DurationInSeconds" UniqueName="DurationInSeconds" />
                                <telerik:GridBoundColumn DataField="RecommendedActionName" UniqueName="RecommendedActionName" />
                                <telerik:GridBoundColumn DataField="MobileOperatorFeedbackName" UniqueName="MobileOperatorFeedbackName" />
                                <telerik:GridBoundColumn DataField="FeedbackDateTime" UniqueName="FeedbackDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />

                                <telerik:GridTemplateColumn HeaderStyle-Width="50px" DataField="ReportingStatusName" UniqueName="ReportingStatusName">
                                    <ItemTemplate>
                                        <asp:Label ID="reportLabel" runat="server" Text='<%# Eval("ReportingStatusName").ToString() %>' Visible='<%# !ReportingStatusVisible(int.Parse(Eval("ReportingStatusID").ToString())) %>' />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                    <ItemTemplate>
                                        <telerik:RadButton ButtonType="LinkButton" ID="reportButton" runat="server" Text='<%# Resources.Resources.Report %>'
                                            CommandArgument='<%#Eval("ID") + ";" +Eval("CaseID")+ ";" +Eval("ReportRealID")%>' CommandName="Report">
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





</asp:Content>

