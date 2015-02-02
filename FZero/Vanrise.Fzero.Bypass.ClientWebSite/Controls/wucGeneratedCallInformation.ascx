<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucGeneratedCallInformation.ascx.cs" Inherits="wucGeneratedCallInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>





<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td class="section" valign="top" style="width: 49%">
            <table width="100%" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="top hspace-20"></td>
                    <td class="top">
                        <%= Resources.Resources.Analysis %></td>
                </tr>
                <tr class="vspace-10">
                    <td></td>
                    <td></td>
                </tr>
                <tr>
                    <td class="hspace-20"></td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td class="caption"><%= Resources.Resources.CaseID %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtCaseID" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.a_number %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txta_number" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.b_number %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtb_number" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.CLI %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtCLI" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.OriginationNetwork %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtOriginationNetwork" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.GeneratedBy %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtGeneratedBy" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.ReceivedBy %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtReceivedBy" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Status %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtStatus" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Priority %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtPriority" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.AttemptDateTime %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtAttemptDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.LevelOneComparisonDateTime %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtLevelOneComparisonDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.LevelTwoComparisonDateTime %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtLevelTwoComparisonDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.TOnefeedback %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtTOnefeedback" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.AssignmentDateTime %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtAssignmentDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.AssignedToFullName %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtAssignedToFullName" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>

        </td>
        <td class="top hspace-20"></td>
        <td class="section" valign="top" style="width: 49%" rowspan="2">
            <table width="100%" cellpadding="0" cellspacing="0">

                <tr>
                    <td valign="top" class="section">
                        <table width="100%" cellpadding="0">
                            <tr>
                                <td class="top hspace-20"></td>
                                <td class="top">
                                    <%= Resources.Resources.MobileOperatorRelatedFields %></td>
                            </tr>
                            <tr class="vspace-10">
                                <td></td>
                                <td></td>
                            </tr>

                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%">

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.ReportingStatus %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtReportingStatus" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="caption"><%= Resources.Resources.ReportID %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtReportRealID" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.MobileOperator %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperator" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td class="caption"><%= Resources.Resources.MobileOperatorFeedback %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtMobileOperatorFeedback" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.FeedbackDateTime %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtFeedbackDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td class="caption"><%= Resources.Resources.Notes %></td>
                                            <td class="hspace-20"></td>
                                            <td class="inputdata">
                                                <telerik:RadTextBox ID="txtFeedbackNotes" runat="server" ReadOnly="true" TextMode="MultiLine" Rows="3" Width="300px"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="vspace-20">
                    <td></td>
                </tr>
            </table>












            <table width="100%" cellpadding="0" cellspacing="0">

                <tr   >
                    <td valign="top" class="section">
                        <table width="100%" cellpadding="0">
                            <tr>
                                <td class="top hspace-20"></td>
                                <td class="top">
                                    <%= Resources.Resources.TrackingHistory %></td>
                            </tr>
                            <tr>
                                <td  colspan="2" >
                                    <telerik:RadGrid AllowSorting="true" ID="gvHistory" runat="server" CellSpacing="0" AutoGenerateColumns="False" ShowHeader="false" BorderColor="White"  >

                                        <ClientSettings>
                                            <Selecting AllowRowSelect="True"></Selecting>
                                        </ClientSettings>

                                        <MasterTableView>
                                            <Columns>
                                                <telerik:GridBoundColumn DataField="ChangeName" UniqueName="ChangeName" />
                                                <telerik:GridBoundColumn DataField="UpdatedOn" UniqueName="UpdatedOn" />
                                            </Columns>
                                        </MasterTableView>
                                    </telerik:RadGrid>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr class="vspace-20">
                    <td></td>
                </tr>
            </table>
















        </td>
    </tr>
    <tr>
        <td class="section" valign="top" style="width: 49%"></td>
        <td class="top hspace-20"></td>
    </tr>

</table>

<asp:HiddenField ID="hdnId" runat="server" Value="0" />






