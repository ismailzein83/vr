﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="SuspectionDefinition.aspx.cs" Inherits="SuspectionDefinition" %>
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
                            <asp:BoundField HeaderText="Suspicion Level" DataField="Suspicion_Level.Name" />
                            <%-- <asp:BoundField HeaderText="CriteriaId1" DataField="CriteriaId1" />--%>


                            <asp:TemplateField HeaderText="Filt. 1">
                                <ItemTemplate>
                                    <%-- <asp:Label runat="server" ID="lbldiv" CssClass=<%#  System.Convert.ToString (Eval("CriteriaId1"))== "1" ? "command btn-success" :"command btn-danger" %> >--%>
                                    <asp:Label runat="server" ID="lblCriteriaId1">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId1"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                    <asp:Label runat="server" ID="Cr1Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId1"))== "1" ? true :false%>  Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr1Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Filt. 2">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId2">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId2"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr2Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId2"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr2Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Filt. 3">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId3">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId3"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr3Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId3"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr3Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Filt. 4">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId4">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId4"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr4Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId4"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr4Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Filt. 5">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId5">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId5"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr5Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId5"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr5Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Filt. 6">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId6">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr6Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr6Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                              <asp:TemplateField HeaderText="Filt. 7">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId7">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId7"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr7Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr7Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                              <asp:TemplateField HeaderText="Filt. 8">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId8">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId8"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr8Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId8"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr8Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                              <asp:TemplateField HeaderText="Filt. 9">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId9">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId9"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr9Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId9"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr9Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Filt. 10">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId10">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId10"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr10Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId10"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr10Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Filt. 11">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId11">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId11"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr11Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId11"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr11Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Filt. 12">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId12">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId12"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr12Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId12"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr12Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Filt. 13">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId13">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId13"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr13Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId13"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr13Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Filt. 14">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId14">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId14"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr14Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId14"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr14Per")))%>   ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                              <asp:TemplateField HeaderText="Filt. 15">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCriteriaId15">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId15"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                    </asp:Label>
                                     <asp:Label runat="server" ID="Cr15Per" Visible= <%# System.Convert.ToString(Eval("CriteriaId15"))== "1" ? true :false%> Text=<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr15Per")))%>   ></asp:Label>
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
                                <div >
                                    <table cellspacing="0" cellpadding="1" >
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
                                                <td>Filters</td>
                                                <td>Is Included</td>
                                                 <td  class="inputData">Filter Description</td>
                                            </tr>

                                            <tr>
                                                <td>Filter 1</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria1" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria1" ></asp:Label>
                                                 </td>

                                            </tr>
                                            <tr>
                                                <td>Filter 2</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria2" />
                                                </td>
                                                <td  >
                                                     <asp:Label runat="server" ID="lblCriteria2" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Filter 3</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria3" />
                                                </td>
                                               <td  >
                                                     <asp:Label runat="server" ID="lblCriteria3" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Filter 4</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria4" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria4" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Filter 5</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria5" />
                                                </td>
                                                  <td  >
                                                     <asp:Label runat="server" ID="lblCriteria5" ></asp:Label>
                                                 </td>
                                            </tr>
                                            <tr>
                                                <td>Filter 6</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria6" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria6" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 7</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria7" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria7" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 8</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria8" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria8" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 9</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria9" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria9" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 10</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria10" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria10" ></asp:Label>
                                                 </td>
                                            </tr>
                                             <tr>
                                                <td>Filter 11</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria11" />
                                                </td>
                                                 <td >
                                                     <asp:Label runat="server" ID="lblCriteria11" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 12</td>
                                                <td>
                                                   <asp:CheckBox runat="server" ID="chkCriteria12" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria12" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 13</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria13" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria13" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 14</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria14" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria14" ></asp:Label>
                                                 </td>
                                            </tr>

                                             <tr>
                                                <td>Filter 15</td>
                                                <td >
                                                   <asp:CheckBox runat="server" ID="chkCriteria15" />
                                                </td>
                                                 <td  >
                                                     <asp:Label runat="server" ID="lblCriteria15" ></asp:Label>
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
