<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="StrategyProperties.aspx.cs" Inherits="StrategyProperties" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">
    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget blue">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Strategies</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <table cellspacing="0" cellpadding="0">

                        <tr>
                            <td class="caption">Strategy
                            </td>
                            <td class="inputData">
                                <Telerik:RadComboBox ID="ddlSearchStrategy" runat="server"    ></Telerik:RadComboBox>
                                
                            </td>
                        </tr>


                        <tr>
                            <td class="caption">Criteria
                            </td>
                            <td class="inputData">
                                <Telerik:RadComboBox ID="ddlSearchCriteria" runat="server"    ></Telerik:RadComboBox>

                            </td>
                        </tr>


                        <tr>
                            <td>&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                    <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                <asp:LinkButton  ID="btnReturn" runat="server" CssClass="btn btn-danger" OnClick="btnReturn_Click" >
                                    <i class="icon-undo icon-white"></i> Return </asp:LinkButton>
                               
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
                    <h4><i class="icon-reorder"></i>Strategy Data</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>
                            <asp:BoundField HeaderText="Strategy Name" DataField="StrategyName" />
                            <asp:BoundField HeaderText="CriteriaId" DataField="CriteriaId" />
                            <asp:BoundField HeaderText="Criteria" DataField="Criteria" />
                            <asp:BoundField HeaderText="Period Value" DataField="PeriodValue" />
                            <asp:BoundField HeaderText="Period" DataField="Period" />
                            <asp:BoundField HeaderText="Threshold" DataField="MaxValue" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("StrategyThresholdId").ToString() +"-"+ Eval("StrategyPeriodId").ToString()%>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <div class="row-fluid" id="divDetails" runat="server">
        <div class="span12">
            <div class="widget green">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Strategy Details</h4>
                </div>

                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <div class="span6 allborders">
                                    <h4 class="breadcrumb">Threshold Information</h4>
                                    <table cellspacing="0" cellpadding="1" class="table">
                                        <tbody>
                                            <tr>
                                                <td>Strategy</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtStrategy" runat="server" Enabled="false"></asp:TextBox>
                                                    <br />

                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Criteria</td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtCriteria" runat="server" Enabled="false"></asp:TextBox>
                                                    <br />

                                                </td>
                                            </tr>


                                            <tr>
                                                <td>Period</td>
                                                <td class="inputData">
                                                    <Telerik:RadComboBox ID="ddlPeriod" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                                                                         <asp:RequiredFieldValidator CssClass="error" ID="rfvPeriod" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlPeriod" ErrorMessage="Period should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Period Value</td>
                                                <td class="inputData">

                                                    <br />
                                                    <telerik:RadNumericTextBox ID="txtPeriodValue" runat="server"  NumberFormat-DecimalDigits="0"     ></telerik:RadNumericTextBox>

                                                </td>
                                            </tr>




                                            <tr>
                                                <td>Threshold Value
                                                </td>
                                                <td class="inputData">
                                                    <br />
                                                    <telerik:RadNumericTextBox ID="txtMaxValue" runat="server"     ></telerik:RadNumericTextBox>
                                                </td>
                                            </tr>

                                        </tbody>
                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td class="space10"></td>
                        </tr>
                        <tr>
                            <td style="text-align: center;">
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
                    </table>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
