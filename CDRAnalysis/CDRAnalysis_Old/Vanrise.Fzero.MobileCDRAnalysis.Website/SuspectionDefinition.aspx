<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="SuspectionDefinition.aspx.cs" Inherits="SuspectionDefinition" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<asp:Content ContentPlaceHolderID="head" runat="server" ID="Head">
</asp:Content>
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

                                <Telerik:RadComboBox ID="ddlSearchStrategy" runat="server" Visible="false"    ></Telerik:RadComboBox> <Telerik:RadTextBox ID="lblStrategy"  ReadOnly="true" runat ="server" Text="Label"></Telerik:RadTextBox>
                                
                            </td>
                        </tr>
                        
                        <tr>
                            <td>
                            </td>
                            <td>
                              <br />
                            </td>
                        </tr>

                        <tr>
                            <td>&nbsp;
                            </td>
                            <td>
                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                    <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                <asp:LinkButton ID="btnReturn" runat="server" CssClass="btn btn-danger" OnClick="btnReturn_Click">
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


                    <Telerik:RadGrid ID="gvData" runat="server"  OnItemCommand="gvData_ItemCommand">

                        <ClientSettings>

                            <Scrolling AllowScroll="True"   FrozenColumnsCount="2"></Scrolling>

                        </ClientSettings>

                        <HeaderStyle Width="225px"></HeaderStyle>


                        <MasterTableView>
                            <Columns>

                                <Telerik:GridBoundColumn HeaderText="Strategy" DataField="Strategy.Name" />
                                <Telerik:GridBoundColumn HeaderText="Suspicion Level" DataField="Suspicion_Level.Name"  />


                                <Telerik:GridTemplateColumn HeaderText="Filter 1">
                                    <ItemTemplate>
                                        <%-- <asp:Label runat="server" ID="lbldiv" CssClass=<%#  System.Convert.ToString (Eval("CriteriaId1"))== "1" ? "command btn-success" :"command btn-danger" %> >--%>
                                        <asp:Label runat="server" ID="lblCriteriaId1">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId1"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr1Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId1"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr1Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>

                                <Telerik:GridTemplateColumn HeaderText="Filter 2">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId2">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId2"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr2Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId2"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr2Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 3">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId3">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId3"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr3Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId3"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr3Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>

                                <Telerik:GridTemplateColumn HeaderText="Filter 4">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId4">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId4"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr4Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId4"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr4Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 5">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId5">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId5"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr5Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId5"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr5Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 6">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId6">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr6Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr6Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>

                                <Telerik:GridTemplateColumn HeaderText="Filter 7">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId7">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId7"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr7Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId6"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr7Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>

                                <Telerik:GridTemplateColumn HeaderText="Filter 8">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId8">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId8"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr8Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId8"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr8Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>

                                <Telerik:GridTemplateColumn HeaderText="Filter 9">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId9">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId9"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr9Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId9"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr9Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 10">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId10">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId10"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr10Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId10"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr10Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 11">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId11">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId11"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr11Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId11"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr11Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 12">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId12">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId12"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr12Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId12"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr12Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 13">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId13">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId13"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr13Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId13"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr13Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 14">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId14">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId14"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr14Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId14"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr14Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>
                                <Telerik:GridTemplateColumn HeaderText="Filter 15">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId15">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId15"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr15Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId15"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr15Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>


                                  <Telerik:GridTemplateColumn HeaderText="Filter 16">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId16">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId16"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr16Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId16"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr16Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>


                                  <Telerik:GridTemplateColumn HeaderText="Filter 17">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId17">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId17"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr17Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId17"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr17Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>


                                   <Telerik:GridTemplateColumn HeaderText="Filter 18">
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCriteriaId18">
                                    <i class= <%# System.Convert.ToString(Eval("CriteriaId18"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                        </asp:Label>
                                        <asp:Label runat="server" ID="Cr18Per" Visible='<%# System.Convert.ToString(Eval("CriteriaId18"))== "1" ? true :false%>' Text='<%# this.Convert_Decimal_Percentage(System.Convert.ToString(Eval("Cr18Per")))%>'></asp:Label>
                                    </ItemTemplate>
                                </Telerik:GridTemplateColumn>

                            
                                <Telerik:GridTemplateColumn>
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
                                </Telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </Telerik:RadGrid>





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
                                <div>
                                    <table cellspacing="3" cellpadding="3" class="table table-advance">
                                        <tbody>
                                            <tr>
                                                <td class="caption required">Strategy</td>
                                                <td class="inputData" colspan="3">
                                                    <Telerik:RadComboBox ID="ddlStrategies" runat="server"></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvStrategies" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlStrategies" InitialValue="0"
                                                        ErrorMessage="The Strategy should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                    <br />
                                                </td>
                                            </tr>

                                            <tr>
                                                <td class="caption required">Suspicion Level</td>
                                                <td class="inputData" colspan="3">
                                                    <Telerik:RadComboBox ID="ddlSuspectionLevel" runat="server"></Telerik:RadComboBox>
                                                    <br />
                                                    <asp:RequiredFieldValidator CssClass="error" ID="rfvSuspectionLevel" runat="server" Display="Dynamic"
                                                        ControlToValidate="ddlSuspectionLevel" InitialValue="0"
                                                        ErrorMessage="The Suspection Level should be selected" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>

                                            <tr>
                                                <td style="padding-top: 20px; font-weight: bold" colspan="2">Check Included:</td>

                                                <td style="padding-top: 20px; font-weight: bold"></td>
                                                <td style="padding-top: 20px; font-weight: bold"></td>
                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria1" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria1"></asp:Label>
                                                </td>

                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider1" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                     
                                                    </Telerik:RadSlider>

                                                </td>


                                            </tr>
                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria2" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>

                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria2"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider2" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>
                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria3" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria3"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider3" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>
                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria4" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria4"></asp:Label>
                                                </td>

                                                <td></td>

                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider4" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>
                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria5" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria5"></asp:Label>
                                                </td>

                                                <td></td>

                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider5" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>
                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria6" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>

                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria6"></asp:Label>
                                                </td>
                                                <td></td>

                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider6" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria7" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria7"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider7" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria8" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria8"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider8" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria9" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria9"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider9" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria10" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>

                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria10"></asp:Label>
                                                </td>

                                                <td></td>

                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider10" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                        
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>
                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria11" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria11"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider11" runat="server" ItemType="Item" Height="50px"   Width="1000px"     >
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria12" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria12"></asp:Label>
                                                </td>
                                                <td></td>

                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider12" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria13" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria13"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider13" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>

                                            <tr>

                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria14" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria14"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider14" runat="server" ItemType="Item" Height="50px"   Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>
                                             <tr>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria15" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria15"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider15" runat="server" ItemType="Item" Height="50px"  Width="1000px">
                                                       
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>



                                             <tr>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria16" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria16"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider16" IsSelectionRangeEnabled ="false"   runat="server" ItemType="Item"  Height="50px"  Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>



                                            <tr>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria17" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria17"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider17" IsSelectionRangeEnabled ="false"   runat="server" ItemType="Item"  Height="50px"  Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

                                                </td>

                                            </tr>


                                              <tr>
                                                <td>
                                                    <asp:CheckBox runat="server" ID="chkCriteria18" OnCheckedChanged="chkCriteria_CheckedChanged" AutoPostBack="true" />
                                                </td>
                                                <td>
                                                    <asp:Label runat="server" ID="lblCriteria18"></asp:Label>
                                                </td>
                                                <td></td>
                                                <td>
                                                    <Telerik:RadSlider ID="RadSlider18" IsSelectionRangeEnabled ="false"   runat="server" ItemType="Item"  Height="50px"  Width="1000px">
                                                      
                                                    </Telerik:RadSlider>

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
