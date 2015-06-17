<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="MinimumValues.aspx.cs" Inherits="MinimumValues" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
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
                                <asp:LinkButton ID="btnAdd" runat="server" CssClass="btn btn-success" OnClick="btnAdd_Click">
                                    <i class="icon-plus icon-white"></i> Add New </asp:LinkButton>
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
                    <h4><i class="icon-reorder"></i>Minimum Values</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>
                            <asp:BoundField HeaderText="Strategy Name" DataField="Strategy.Name" />
                            <asp:BoundField HeaderText="CriteriaId" DataField="Criteria_Profile.Id" />
                            <asp:BoundField HeaderText="Criteria" DataField="Criteria_Profile.Description" />
                            <asp:BoundField HeaderText="Min_Count_Value" DataField="Min_Count_Value" />
                            <asp:BoundField HeaderText="Minimum Aggregate Volume" DataField="Min_Aggreg_Volume" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" CssClass="command btn-success" CommandArgument='<%#Eval("Id").ToString()%>' CommandName="Modify">
                                        <i class="icon-pencil"></i>
                                    </asp:LinkButton>
                                    <asp:LinkButton ID="btnDelete" runat="server" CssClass="command btn-danger"
                                        CommandArgument='<%#Eval("Id")%>' CommandName="Remove"
                                        OnClientClick="return confirm('Are you sure you want to delete this Record?');">
                                        <i class="icon-trash"></i>
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
                                                <td class="caption required">Strategy</td>
                                                <td class="inputData">
                                                    
                                                    <br />
                                                    
                                                    <Telerik:RadComboBox ID="ddlStrategy" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvStrategies" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlStrategy" InitialValue="0"
                                                        ErrorMessage="The Strategy should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    <br />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="caption required">Criteria</td>
                                                <td class="inputData">
                                                    <Telerik:RadComboBox ID="ddlCriteria" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="RequiredFieldValidator1" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlCriteria" InitialValue="0"
                                                        ErrorMessage="The Strategy should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    <br />

                                                </td>
                                            </tr>
                                            <tr>
                                                <td>Minimum Count Value</td>
                                                <td class="inputData">

                                                    <br />
                                                    <asp:TextBox ID="txtMin_Count_Value" runat="server"></asp:TextBox>

                                                </td>
                                            </tr>




                                            <tr>
                                                <td>Minimum Aggregate Volume
                                                </td>
                                                <td class="inputData">
                                                    <asp:TextBox ID="txtMin_Aggreg_Volume" runat="server"></asp:TextBox>
                                                    <br />

                                                    <%--                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvType" runat="server" Display="Dynamic"
                                                        ControlToValidate="txtMaxValue" ErrorMessage="MaxValue should not be empty" ValidationGroup="Save"></asp:RequiredFieldValidator>--%>
                                                
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