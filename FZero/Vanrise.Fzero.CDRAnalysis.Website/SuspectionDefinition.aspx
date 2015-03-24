<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="SuspectionDefinition.aspx.cs" Inherits="SuspectionDefinition" %>
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
                    <h4><i class="icon-reorder"></i>Suspicion Levels</h4>
                   
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>

                            <asp:BoundField HeaderText="Strategy" DataField="Strategy.Name" />
                            <asp:BoundField HeaderText="Suspicion Level" DataField="Suspection_Level.Name" />
                            <%-- <asp:BoundField HeaderText="CriteriaId1" DataField="CriteriaId1" />--%>


                            <asp:TemplateField HeaderText="Criterion 1">
                                <ItemTemplate>
                                    <%-- <asp:Label runat="server" ID="lbldiv" CssClass=<%#  System.Convert.ToString (Eval("CriteriaId1"))== "1" ? "command btn-success" :"command btn-danger" %> >--%>
                                    <asp:Label runat="server" ID="lblCriteriaId1">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId1"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Criterion 2">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId2">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId2"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Criterion 3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId3">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId3"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Criterion 4">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId4">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId4"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Criterion 5">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId5">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId5"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Criterion 6">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId6">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
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
                                                    <Telerik:RadComboBox ID="ddlStrategies" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvStrategies" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlStrategies" InitialValue="0"
                                                        ErrorMessage="The Strategy should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    <br />
                                                </td>
                                                <td  class="inputData"></td>
                                            </tr>

                                            <tr>
                                                <td class="caption required">Suspicion Level</td>
                                                <td class="inputData">
                                                    <Telerik:RadComboBox ID="ddlSuspectionLevel" runat="server"    ></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvSuspectionLevel" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlSuspectionLevel" InitialValue="0"
                                                        ErrorMessage="The Suspection Level should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                                <td  class="inputData"></td>
                                            </tr>

                                             <tr>
                                                <td>Criteria</td>
                                                <td>Is Included</td>
                                                 <td  class="inputData">Criterion Description</td>
                                            </tr>

                                            <tr>
                                                <td>Criterion 1</td>
                                                <td class="inputData">
                                                   <asp:CheckBox runat="server" ID="chkCriteria1" />
                                                </td>
                                                 <td  class="inputData">
                                                     <asp:Label runat="server" ID="lblCriteria1" ></asp:Label>
                                                 </td>

                                            </tr>
                                            <tr>
                                                <td>Criterion 2</td>
                                                <td class="inputData">
                                                   <asp:CheckBox runat="server" ID="chkCriteria2" />
                                                </td>
                                                <td  class="inputData">
                                                     <asp:Label runat="server" ID="lblCriteria2" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Criterion 3</td>
                                                <td class="inputData">
                                                   <asp:CheckBox runat="server" ID="chkCriteria3" />
                                                </td>
                                               <td  class="inputData">
                                                     <asp:Label runat="server" ID="lblCriteria3" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Criterion 4</td>
                                                <td class="inputData">
                                                   <asp:CheckBox runat="server" ID="chkCriteria4" />
                                                </td>
                                                 <td  class="inputData">
                                                     <asp:Label runat="server" ID="lblCriteria4" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Criterion 5</td>
                                                <td class="inputData">
                                                   <asp:CheckBox runat="server" ID="chkCriteria5" />
                                                </td>
                                                  <td  class="inputData">
                                                     <asp:Label runat="server" ID="lblCriteria5" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Criterion 6</td>
                                                <td class="inputData">
                                                   <asp:CheckBox runat="server" ID="chkCriteria6" />
                                                </td>
                                                 <td  class="inputData">
                                                     <asp:Label runat="server" ID="lblCriteria6" ></asp:Label>
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
