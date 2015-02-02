<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ResultedCases.aspx.cs" Inherits="ResultedCases" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Src="Controls/wucGeneratedCallInformation.ascx" TagName="wucGeneratedCallInformation" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">

    <div class="row-fluid" id="divView" runat="server" visible="false">
        <div class="span12">
            <div class="widget green">
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
            <div class="widget gray">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Filters</h4>

                </div>
                <div class="widget-body" style="display: block;">


                    <table width="100%">
                        <tr>
                            <td>
                                <table cellpadding="0" cellspacing="0" width="100%">

                                    <tr>

                                        <td>&nbsp;</td>
                                        <td>
                                            <%=Resources.Resources.DateRange %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlDateRange" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlDateRange_SelectedIndexChanged">
                                                <Items>
                                                    <telerik:RadComboBoxItem Selected="True" Value="0" Text="Specific Date"></telerik:RadComboBoxItem>
                                                    <telerik:RadComboBoxItem Value="1" Text="Today"></telerik:RadComboBoxItem>
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
                                        <td>
                                            <%=Resources.Resources.Status %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchStatus" runat="server"></telerik:RadComboBox>
                                        </td>


                                        <td>&nbsp;</td>

                                        <td>
                                            <%=Resources.Resources.CaseID %>
                                        </td>
                                        <td></td>
                                        <td>

                                            <telerik:RadTextBox ID="txtSearchCaseID" runat="server"></telerik:RadTextBox>
                                        </td>













                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>
                                    <tr>


                                        <td>&nbsp;</td>
                                        <td>
                                            <%=Resources.Resources.AttemptFromDate %>
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

                                        <td>
                                            <%=Resources.Resources.Priority %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchPriority" runat="server"></telerik:RadComboBox>
                                        </td>






                                        <td>&nbsp;</td>
                                        <td>
                                            <%=Resources.Resources.a_number %>
                                        </td>
                                        <td></td>
                                        <td>

                                            <telerik:RadTextBox ID="txtSearcha_number" runat="server"></telerik:RadTextBox>
                                        </td>
















                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>


                                    <tr>



                                        <td>&nbsp;</td>

                                        <td>
                                            <%=Resources.Resources.AttemptToDate %>
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
                                        <td>
                                            <%=Resources.Resources.MobileOperator %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchCLIMobileOperator" runat="server"></telerik:RadComboBox>
                                        </td>













                                        <td>&nbsp;</td>

                                        <td>
                                            <%=Resources.Resources.b_number %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadTextBox ID="txtSearchb_number" runat="server"></telerik:RadTextBox>
                                        </td>










                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>



                                    <tr>











                                        <td>&nbsp;</td>
                                        <td>
                                            <%=Resources.Resources.GeneratedBy %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchSource" runat="server"></telerik:RadComboBox>
                                        </td>



                                        <td>&nbsp;</td>
                                        <td>
                                            <%=Resources.Resources.ReportingStatus %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchReportingStatus" runat="server" DropDownWidth="150"></telerik:RadComboBox>

                                        </td>







                                        <td>&nbsp;</td>
                                        <td><%=Resources.Resources.CLI %></td>
                                        <td></td>
                                        <td>
                                            <telerik:RadTextBox ID="txtSearchCLIReceived" runat="server"></telerik:RadTextBox></td>



                                    </tr>

                                    <tr>
                                        <td></td>
                                    </tr>


                                    <tr>

                                        <td>&nbsp;</td>

                                        <td>
                                            <%=Resources.Resources.ReceivedBy %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchReceivedSource" runat="server"></telerik:RadComboBox>
                                        </td>





                                        <td>&nbsp;</td>
                                        <td><%=Resources.Resources.CLIReported %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <asp:RadioButtonList ID="rblCLIReported" runat="server" RepeatDirection="Horizontal">
                                                <asp:ListItem Selected="True" Value="0">All</asp:ListItem>
                                                <asp:ListItem Value="1">Yes</asp:ListItem>
                                                <asp:ListItem Value="2">No</asp:ListItem>
                                            </asp:RadioButtonList>
                                        </td>

                                        <td>&nbsp;</td>

                                        <td>
                                            <%=Resources.Resources.OriginationNetwork %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadTextBox ID="txtSearchOriginationNetwork" runat="server"></telerik:RadTextBox>
                                        </td>


                                    </tr>
                                    <tr>
                                        <td></td>
                                    </tr>




                                    <tr>



                                        <td>&nbsp;</td>
                                        <td style="color: red"><%=Resources.Resources.Client %>
                                        </td>
                                        <td></td>
                                        <td>
                                            <telerik:RadComboBox ID="ddlSearchClient" runat="server"></telerik:RadComboBox>
                                        </td>







                                        <td></td>
                                        <td style="color: red"></td>
                                        <td></td>
                                        <td></td>

                                        <td></td>
                                        <td></td>
                                        <td></td>
                                        <td></td>



                                    </tr>
                                    <tr>
                                        <td colspan="12" align="center">
                                            <asp:LinkButton ID="btnExport" runat="server" CssClass="btn btn-warning" OnClick="btnExport_Click">
                                                                 <i class="icon-barcode icon-white"></i> Export </asp:LinkButton>



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

    <div class="row-fluid" id="divData" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Results</h4>

                </div>
                <div class="widget-body" style="display: block;">
                    <table width="100%">
                        <tr>
                            <td>

                                <table style="width: 100%" runat="server" id="tblSummary">
                                    <tr>
                                        <td style="text-align: right;">



                                           

                                            <asp:Label ID="Label1" runat="server" Text="Fraud Cases: "></asp:Label>

                                        </td>
                                        <td style="text-align: left;">
                                            <span runat="server" id="spanFraudCases"></span>

                                        </td>
                                        <td style="text-align: right;">
                                            <asp:Label ID="Label2" runat="server" Text="Distinct Fraud Cases: "></asp:Label>

                                        </td>
                                        <td style="text-align: left;">
                                            <span runat="server" id="spanDistinctFraudCases"></span>
                                        </td>
                                        <td style="text-align: right;">



                                            <asp:LinkButton ID="btnReportingStatus_ToBeReported" runat="server" CssClass="btn btn-success" OnClick="btnReportingStatus_ToBeReported_Click">
                                                                 <i class="icon-fullscreen icon-white"></i> to be Rep </asp:LinkButton>

                                            <asp:LinkButton ID="btnReportingStatus_Ignored" runat="server" CssClass="btn btn-success" OnClick="btnReportingStatus_Ignored_Click">
                                                                 <i class="icon-certificate icon-white"></i> Ignored </asp:LinkButton>

                                            <asp:LinkButton ID="btnVerified" runat="server" CssClass="btn btn-success" OnClick="btnVerified_Click">
                                                                 <i class="icon-fire icon-white"></i> Verified </asp:LinkButton>

                                            <asp:LinkButton ID="btnReOpen" runat="server" CssClass="btn btn-success" OnClick="btnReOpen_Click">
                                                                 <i class="icon-remove-circle icon-white"></i> Re-Open </asp:LinkButton>

                                            <asp:LinkButton ID="btnToBeInvestigated" runat="server" CssClass="btn btn-success" OnClick="btnToBeInvestigated_Click">
                                                                 <i class="icon-wrench icon-white"></i> to be Inv </asp:LinkButton>




                                        </td>
                                    </tr>
                                </table>
                            </td>




                        </tr>








                        <tr>
                            <td>
                                <telerik:RadGrid ID="gvGeneratedCalls" ShowGroupPanel="true" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                    AllowPaging="true" OnItemCommand="gvGeneratedCalls_ItemCommand" AllowMultiRowSelection="true"
                                    AutoGenerateColumns="False" OnItemDataBound="gvGeneratedCalls_ItemDataBound" ShowFooter="true" OnNeedDataSource="gvGeneratedCalls_NeedDataSource">
                                    <ClientSettings>
                                        <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                                    </ClientSettings>
                                    <MasterTableView DataKeyNames="ID, ReportingStatusID, StatusID, CLIReported, CLI, b_number, MobileOperatorFeedbackName" ClientDataKeyNames="ID, ReportingStatusID, StatusID, CLIReported, CLI, b_number, MobileOperatorFeedbackName" ShowGroupFooter="true">
                                        <Columns>
                                            <telerik:GridClientSelectColumn UniqueName="Select"></telerik:GridClientSelectColumn>
                                            <telerik:GridBoundColumn DataField="CaseID" UniqueName="CaseID" Aggregate="Count" />
                                            <telerik:GridBoundColumn DataField="ClientName" UniqueName="ClientName" />
                                            <telerik:GridBoundColumn DataField="SourceName" UniqueName="SourceName" />
                                            <telerik:GridBoundColumn DataField="ReceivedSourceName" UniqueName="ReceivedSourceName" />

                                            <telerik:GridTemplateColumn UniqueName="StatusName">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatusName" runat="server" Text='<%# Eval("StatusName").ToString() %>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>


                                            <telerik:GridBoundColumn DataField="PriorityName" UniqueName="PriorityName" />
                                            <telerik:GridBoundColumn DataField="ReportingStatusName" UniqueName="ReportingStatusName" />
                                            <telerik:GridBoundColumn DataField="a_number" UniqueName="a_number" />
                                            <telerik:GridBoundColumn DataField="b_number" UniqueName="b_number" />
                                            <telerik:GridBoundColumn DataField="CLI" UniqueName="CLI" />
                                            <telerik:GridBoundColumn DataField="AttemptDateTime" UniqueName="AttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                            <telerik:GridCheckBoxColumn DataField="CLIReported" UniqueName="CLIReported" />


                                            <telerik:GridBoundColumn DataField="MobileOperatorFeedbackName" UniqueName="MobileOperatorFeedbackName" />

                                            <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                <ItemTemplate>

                                                    <asp:LinkButton ID="viewButton" CommandArgument='<%#Eval("ID") + ";" +Eval("CaseID")+ ";" +Eval("ReportRealID")%>' runat="server" CssClass="command btn-link" CommandName="View" ToolTip="View Details">
                                                                 <i class="icon-eye-open icon-white"></i>  </asp:LinkButton>


                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>

                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>


                            </td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>
    </div>














</asp:Content>

