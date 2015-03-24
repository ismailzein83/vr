<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="ReportedCases.aspx.cs" Inherits="ReportedCases" %>

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



    <table cellpadding="0" cellspacing="0" width="100%" class="page">

        <tr id="trView" runat="server" visible="false">
            <td valign="top">
                <table width="100%">
                    <tr>
                        <td valign="top">


                            <table cellpadding="0" cellspacing="0" width="100%">
                                <tr class="vspace-10">
                                    <td></td>
                                    <td></td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="hspace-50"></td>
                                    <td>
                                        <uc1:wucGeneratedCallInformation ID="wucGeneratedCallInformation" runat="server" />
                                    </td>
                                    <td class="hspace-50"></td>
                                </tr>
                                <tr class="vspace-10">
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
                            <telerik:RadButton ID="btnCancel" runat="server" OnClick="btnCancel_Click" CausesValidation="false">
                                <Icon PrimaryIconUrl="Icons/cancel.png" />
                            </telerik:RadButton>
                        </td>
                    </tr>
                    <tr class="vspace-10">
                        <td></td>
                    </tr>

                </table>
            </td>
        </tr>


        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr id="trSearch">
                        <td valign="top">
                            <table class="search" width="100%">
                                <tr>
                                    <td>
                                        <table cellpadding="0" cellspacing="0" class="margin-bottom-10">

                                            <tr>

                                                <td class="hspace-20">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.DateRange %>
                                                </td>
                                                <td class="hspace-20"></td>
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


                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.Feedback %>
                                                </td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadComboBox ID="ddlSearchOperatorReply" runat="server"></telerik:RadComboBox>
                                                </td>



                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.ReportID %>
                                                </td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadTextBox ID="txtSearchReportID" runat="server"></telerik:RadTextBox>
                                                </td>

                                            </tr>
                                            <tr class="vspace-5">
                                                <td></td>
                                            </tr>
                                            <tr>


                                                <td class="hspace-20">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.FromDate %>
                                                </td>
                                                <td class="hspace-20"></td>
                                                <td>
                                                    <telerik:RadDateTimePicker ID="rdtpSearchFromAttemptDate" runat="server" AutoPostBackControl="Both" OnSelectedDateChanged="rdtpSearchFromAttemptDate_SelectedDateChanged">
                                                        <TimeView CellSpacing="-1"></TimeView>

                                                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

                                                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

                                                        <DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" EnableSingleInputRendering="True" LabelWidth="64px" AutoPostBack="True"></DateInput>

                                                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                                                    </telerik:RadDateTimePicker>
                                                </td>


                                                <td class="hspace-20">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.b_number %>
                                                </td>
                                                <td class="hspace-20"></td>
                                                <td>

                                                    <telerik:RadTextBox ID="txtSearchb_number" runat="server"></telerik:RadTextBox>
                                                </td>



                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption"><%=Resources.Resources.CLI %></td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadTextBox ID="txtSearchCLIReceived" runat="server"></telerik:RadTextBox></td>



                                            </tr>
                                            <tr class="vspace-5">
                                                <td></td>
                                            </tr>


                                            <tr>



                                                <td class="hspace-50">&nbsp;</td>

                                                <td class="caption">
                                                    <%=Resources.Resources.ToDate %>
                                                </td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadDateTimePicker ID="rdtpSearchToAttemptDate" runat="server" AutoPostBackControl="Both" OnSelectedDateChanged="rdtpSearchToAttemptDate_SelectedDateChanged">
                                                        <TimeView CellSpacing="-1"></TimeView>

                                                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

                                                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

                                                        <DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" EnableSingleInputRendering="True" LabelWidth="64px" AutoPostBack="True"></DateInput>

                                                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                                                    </telerik:RadDateTimePicker>


                                                </td>




                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.Action %>
                                                </td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadComboBox ID="ddlSearchRecommendedAction" runat="server"></telerik:RadComboBox>
                                                </td>


                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption">
                                                    <%=Resources.Resources.MobileOperator %>
                                                </td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadComboBox ID="ddlSearchMobileOperator" runat="server"></telerik:RadComboBox>
                                                </td>








                                            </tr>

                                            <tr class="vspace-5">
                                                <td></td>
                                            </tr>



                                            <tr>



                                                <td class="hspace-50">&nbsp;</td>

                                                <td class="caption">
                                                    <%=Resources.Resources.CaseID %>
                                                </td>
                                                <td class="hspace-50"></td>
                                                <td>
                                                    <telerik:RadTextBox ID="txtSearchCaseID" runat="server"></telerik:RadTextBox>
                                                </td>




                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption"></td>
                                                <td class="hspace-50"></td>
                                                <td></td>


                                                <td class="hspace-50">&nbsp;</td>
                                                <td class="caption"></td>
                                                <td class="hspace-50"></td>
                                                <td></td>








                                            </tr>

                                            <tr class="vspace-5">
                                                <td></td>
                                            </tr>





                                        </table>
                                        <tr id="trSearchCommands">
                                            <td class="commands">
                                                <table cellpadding="3" cellspacing="2" border="0" align="center">
                                                    <tr>
                                                        <td>
                                                            <telerik:RadButton ID="btnSearch" runat="server" CausesValidation="False"
                                                                OnClick="btnSearch_Click">
                                                                <Icon PrimaryIconUrl="Icons/search_16.png" />
                                                            </telerik:RadButton>
                                                        </td>
                                                        <td class="hspace"></td>
                                                        <td>
                                                            <telerik:RadButton ID="btnSearchClear" runat="server" CausesValidation="False"
                                                                OnClick="btnSearchClear_Click">
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


                    <tr id="tr1">
                        <td valign="top">
                            <table width="100%">
                                <tr>
                                    <td class="right">

                                        <table width="100%">
                                            <tr>
                                                <td>
                                                    <asp:TextBox ID="txtFeedbackNotes" TextMode="MultiLine" Rows="3" Width="700px" runat="server" Text=""></asp:TextBox></td>

                                                <td>
                                                    <telerik:RadComboBox ID="ddlMobileOperatorFeedback"  runat="server"></telerik:RadComboBox>
                                                </td>

                                                <td>
                                                    <telerik:RadDateTimePicker ID="rdtpFeedbackDateTime" runat="server"></telerik:RadDateTimePicker>
                                                </td>
                                                <td>
                                                    <telerik:RadButton ID="btnConfirm" runat="server" OnClick="btnConfirm_Click">
                                                        <Icon PrimaryIconUrl="Icons/Check.png" />
                                                    </telerik:RadButton>
                                                </td>

                                            </tr>

                                        </table>




                                    </td>

                                </tr>
                                <tr class="vspace-5">
                                    <td></td>
                                </tr>




                                <tr>
                                    <td>
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
                                                            <telerik:RadButton ButtonType="StandardButton" ID="reportButton" runat="server" Text='<%# Resources.Resources.Report %>'
                                                                CommandArgument='<%#Eval("ID") + ";" +Eval("CaseID")+ ";" +Eval("ReportRealID")%>' CommandName="Report">
                                                                <Icon PrimaryIconUrl="~/Icons/document.gif" />
                                                            </telerik:RadButton>
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>

                                                    <telerik:GridTemplateColumn HeaderStyle-Width="50px">
                                                        <ItemTemplate>
                                                            <telerik:RadButton ButtonType="StandardButton" ID="viewButton" runat="server" Text='<%# Resources.Resources.View %>'
                                                                CommandArgument='<%#Eval("ID") + ";" +Eval("CaseID")+ ";" +Eval("ReportRealID")%>' CommandName="View">
                                                                <Icon PrimaryIconUrl="~/Icons/view.gif" />
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
            </td>
        </tr>
    </table>













</asp:Content>

