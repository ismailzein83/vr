<%@ Control Language="C#" AutoEventWireup="true" CodeFile="wucGeneratedCallInformation.ascx.cs" Inherits="wucGeneratedCallInformation" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>





<table width="100%" cellpadding="0" cellspacing="0">
    <tr>
        <td valign="top" style="width: 49%">
            <table width="100%" cellpadding="0" cellspacing="0">

                <tr>
                    <td ></td>
                    <td>
                        <table width="100%">
                            <tr>
                                <td>
                                    <table width="100%">
                                        <tr>
                                            <td ><%= Resources.Resources.CaseID %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtCaseID" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.a_number %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txta_number" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.b_number %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtb_number" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.CLI %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtCLI" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.OriginationNetwork %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtOriginationNetwork" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.GeneratedBy %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtGeneratedBy" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.ReceivedBy %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtReceivedBy" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.Status %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtStatus" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.Priority %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtPriority" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.AttemptDateTime %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtAttemptDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.LevelOneComparisonDateTime %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtLevelOneComparisonDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.LevelTwoComparisonDateTime %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtLevelTwoComparisonDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.TOnefeedback %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtTOnefeedback" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.AssignmentDateTime %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtAssignmentDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.AssignedToFullName %></td>
                                            <td  ></td>
                                            <td  >
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
        <td></td>
        <td valign="top" style="width: 49%" rowspan="2">
            <table width="100%" cellpadding="0" cellspacing="0">

                <tr>
                    <td valign="top"  >
                        <table width="100%" cellpadding="0">

                            <tr>
                                <td></td>
                                <td>
                                    <table width="100%">

                                        <tr>
                                            <td  ><%= Resources.Resources.ReportingStatus %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtReportingStatus" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td  ><%= Resources.Resources.ReportID %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtReportRealID" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.MobileOperator %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtMobileOperator" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>

                                        <tr>
                                            <td  ><%= Resources.Resources.MobileOperatorFeedback %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtMobileOperatorFeedback" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.FeedbackDateTime %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtFeedbackDateTime" runat="server" ReadOnly="true"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                        <tr>
                                            <td  ><%= Resources.Resources.Notes %></td>
                                            <td  ></td>
                                            <td  >
                                                <telerik:RadTextBox ID="txtFeedbackNotes" runat="server" ReadOnly="true" TextMode="MultiLine" Rows="3" Width="300px"></telerik:RadTextBox>
                                            </td>
                                        </tr>


                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

            </table>












            <table width="100%" cellpadding="0" cellspacing="0">

                <tr>
                    <td valign="top"  >
                        <table width="100%" cellpadding="0">
                            <tr>
                                <td ></td>
                                <td >
                                    <%= Resources.Resources.TrackingHistory %></td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <telerik:RadGrid AllowSorting="true" ID="gvHistory" runat="server" CellSpacing="0" AutoGenerateColumns="False" ShowHeader="false" BorderColor="White">

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

            </table>
















        </td>
    </tr>
    <tr>
        <td   valign="top" style="width: 49%"></td>
        <td ></td>
    </tr>

</table>

<asp:HiddenField ID="hdnId" runat="server" Value="0" />






