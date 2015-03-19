<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterPage.master" CodeFile="SuspectionAnalysis.aspx.cs" Inherits="SuspectionAnalysis" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="body">

    <link rel='stylesheet' type='text/css' href="App_Themes/Styles/jquery-ui.css" />
    <script src="Scripts/jquery-ui.min.js"></script>


    <div class="row-fluid" id="divFilter" runat="server">
        <div class="span12">
            <div class="widget gray">
                <div class="widget-title">
                    <h4><i class="icon-reorder"></i>Search Strategies</h4>
                    
                </div>
                <div class="widget-body" style="display: block;">

                    <table class="auto-style1">
                        <tr>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-top-style: solid; border-top-width: 1px; border-top-color: #C0C0C0; border-left-style: solid; border-left-width: 1px; border-left-color: #C0C0C0; padding-left: 5px;" class="caption required" valign="top">
                                <asp:Label ID="lblStrategy" runat="server" Text="Strategy" Font-Bold="true"></asp:Label></td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-top-style: solid; border-top-width: 1px; border-top-color: #C0C0C0; padding-left: 5px;" valign="top">

                                <asp:Label ID="Label1" runat="server" Text="Minimum Occurence/Period" Font-Bold="true"></asp:Label>
                            </td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-top-style: solid; border-top-width: 1px; border-top-color: #C0C0C0; padding-left: 5px;" valign="top" class="caption required">
                                <asp:Label ID="Label2" runat="server" Text="From date" Font-Bold="true"></asp:Label>
                            </td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-top-style: solid; border-top-width: 1px; border-top-color: #C0C0C0; padding-left: 5px;" valign="top" class="caption required">
                                <asp:Label ID="Label3" runat="server" Text="To date" Font-Bold="true"></asp:Label>

                            </td>
                        </tr>
                        <tr>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-left-style: solid; border-left-width: 1px; border-left-color: #C0C0C0; padding-left: 5px;" valign="top">

                                <Telerik:RadComboBox ID="ddlSearchStrategy" runat="server" Filter="Contains" EnableTextSelection="true"></Telerik:RadComboBox>
                            </td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; padding-left: 5px;" valign="top">
                                <Telerik:RadNumericTextBox ID="txtMinimumOccurance" Height="30" Width="200" runat="server" Value="1" ShowSpinButtons="true" NumberFormat-DecimalDigits="0" MinValue="1"></Telerik:RadNumericTextBox>
                            </td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; padding-left: 5px;" valign="top">
                                <Telerik:RadDatePicker ID="dtpFromDate" runat="server" Width="220"
                                    DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                                </Telerik:RadDatePicker>

                            </td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; padding-left: 5px;" valign="top">

                                <Telerik:RadDatePicker ID="dtpToDate" runat="server" Width="220"
                                    DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                                </Telerik:RadDatePicker>

                            </td>
                        </tr>
                        <tr>
                            <td style="width: 25%; border-bottom-style: solid; border-bottom-width: 1px; border-bottom-color: #C0C0C0; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-left-style: solid; border-left-width: 1px; border-left-color: #C0C0C0; padding-left: 5px;" valign="top">
                                <asp:RequiredFieldValidator CssClass="error" ID="rfvSwitch" runat="server" Display="Dynamic"
                                    ControlToValidate="ddlSearchStrategy" InitialValue="0"
                                    ErrorMessage="The Strategy should be selected" ValidationGroup="Search"></asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 25%; border-bottom-style: solid; border-bottom-width: 1px; border-bottom-color: #C0C0C0; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0;" valign="top">&nbsp;</td>
                            <td style="width: 25%; border-bottom-style: solid; border-bottom-width: 1px; border-bottom-color: #C0C0C0; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; padding-left: 5px;" valign="top">

                                <asp:RequiredFieldValidator CssClass="error" ID="rfvFromDate" runat="server" Display="Dynamic"
                                    ControlToValidate="dtpFromDate" ErrorMessage="Date should not be empty" ValidationGroup="Search"></asp:RequiredFieldValidator>

                            </td>
                            <td style="width: 25%; border-bottom-style: solid; border-bottom-width: 1px; border-bottom-color: #C0C0C0; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; padding-left: 5px;" valign="top">

                                <asp:RequiredFieldValidator CssClass="error" ID="rfvToDate" runat="server" Display="Dynamic"
                                    ControlToValidate="dtpToDate" ErrorMessage="Date should not be empty" ValidationGroup="Search"></asp:RequiredFieldValidator>

                                <asp:CompareValidator ID="dateCompareValidator" runat="server" ControlToValidate="dtpToDate"
                                    ControlToCompare="dtpFromDate" Operator="GreaterThan" Type="Date" ValidationGroup="Search"
                                    ErrorMessage="To Date should be greater than From Date." CssClass="error">
                                </asp:CompareValidator>

                            </td>
                        </tr>
                        <tr>
                            <td style="width: 25%; border-left-style: solid; border-left-width: 1px; border-left-color: #C0C0C0; padding-top: 10px; padding-left: 5px;" valign="top">
                                <asp:Label ID="lblSuspectionLevel" runat="server" Text="Suspicion Level" Font-Bold="true"></asp:Label></td>
                            <td style="width: 25%; padding-top: 10px;" valign="top">&nbsp;</td>
                            <td style="width: 25%; padding-top: 10px;" valign="top">&nbsp;</td>
                            <td style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; padding-bottom: 10px;" valign="top">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="4" valign="top" style="width: 25%; border-right-style: solid; border-right-width: 1px; border-right-color: #C0C0C0; border-left-style: solid; border-left-width: 1px; border-left-color: #C0C0C0; border-bottom-style: solid; border-bottom-width: 1px; border-bottom-color: #C0C0C0; padding-left: 5px;">

                                <asp:CheckBoxList ID="cblSuspiciousLevel" runat="server" RepeatDirection="Horizontal" Width="500">
                                    <asp:ListItem Selected="False" Text="Clean" Value="Clean"></asp:ListItem>
                                    <asp:ListItem Selected="True" Text="Suspicious" Value="Suspicious"></asp:ListItem>
                                    <asp:ListItem Selected="True" Text="Highly Suspicious" Value="HighlySuspicious"></asp:ListItem>
                                    <asp:ListItem Selected="True" Text="Fraud" Value="Fraud"></asp:ListItem>
                                </asp:CheckBoxList>

                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="center" valign="top" style="padding-top: 10px">

                                <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click" ValidationGroup="Search">
                                                        <i class="icon-search icon-white"></i> Search </asp:LinkButton>

                                 <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-danger" OnClick="btnClear_Click">
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
                    <h4><i class="icon-reorder"></i>Suspicions List</h4>
                    <span class="tools" style="height: 20px; vertical-align: top">

                        <asp:LinkButton ID="lnkReport" runat="server" OnClick="lnkReport_Click">
                             <i class="icon-file"></i> Add to Report </asp:LinkButton>

                        <a class="dropdown-toggle" href="#" data-toggle="dropdown">

                            <span class="username"></span></a>

                       
                    </span>
                    <span class="tools" style="height: 20px; margin-top: 0">
                        <Telerik:RadComboBox ID="ddlreports" runat="server"></Telerik:RadComboBox>
                        <%--<asp:TextBox runat="server" ID="txtReportnumber" Width="100" style="height:20px;margin-top:0"></asp:TextBox>--%>

                    </span>
                </div>
                <div class="widget-body" style="display: block;">
                    <asp:GridView ID="gvData" runat="server" SkinID="GridDefault" OnRowDataBound="gvData_RowDataBound"
                        OnRowCommand="gvData_RowCommand">
                        <Columns>

                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="chkReport" />

                                </ItemTemplate>
                            </asp:TemplateField>

                           <asp:BoundField HeaderText="Strategy Id" DataField="strategyId" />
                             <asp:BoundField HeaderText="Strategy" DataField="StrategyName" />



                            <asp:BoundField HeaderText="Subscriber Number" DataField="SubscriberNumber" />
                            <asp:BoundField HeaderText="Suspicion Level" DataField="SuspectionLevelName" />

                            <asp:TemplateField HeaderText="Last Report">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnReport" runat="server" Text='<%#Eval("LastReportNumber").ToString() %>' CommandArgument='<%#Eval("LastReportNumber").ToString() %>' CommandName="Report">
                                        
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Occurrence_Day">
                                <ItemTemplate>
                                    <asp:Label ID="lblOccurrence_Day" runat="server" Text='<%#Eval("Occurrence_Day").ToString() +"  Day(s) " %>'>
                                        
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="Occurrence_Hour">
                                <ItemTemplate>
                                    <asp:Label ID="lblOccurrence_Hour" runat="server" Text='<%#Eval("Occurrence_Hour").ToString() +"  Hour(s) " %>'>
                                        
                                    </asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>


                           <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEdit" runat="server" Text="Details" CommandArgument='<%#Eval("strategyId").ToString() +"-"+ (Eval("SubscriberNumber")==null?"":Eval("SubscriberNumber").ToString()) %>' CommandName="Modify">
                                        
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
                    <h4><i class="icon-reorder"></i>Subscriber Number Information</h4>
                </div>
                <div class="widget-body" style="display: block;">
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <%--<div class="span6 allborders">--%>
                                <table cellspacing="0" cellpadding="1" class="table">
                                    <tbody>

                                        <tr>
                                            <td>
                                                <table border="1">
                                                    <tr>
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td class="caption">Strategy</td>
                                                                    <td class="inputData">
                                                                        <asp:TextBox ID="txtStrategy" runat="server" Enabled="false"></asp:TextBox>
                                                                        <br />

                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td></td>
                                                                    <td></td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="caption">Subscriber Number</td>
                                                                    <td class="inputData">
                                                                        <asp:TextBox ID="txtSubscriberNumber" runat="server" Enabled="false"></asp:TextBox>
                                                                        <br />

                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <td style="vertical-align: top; padding-left: 30px; padding-right: 30px">
                                                            <!------- list  --->

                                                            <div>
                                                                <asp:Label runat="server" Text="Related Numbers" />
                                                            </div>

                                                            <asp:ListBox runat="server" ID="lstRelatedNumber" Rows="6"></asp:ListBox>
                                                        </td>

                                                        <!----- ----------------------->
                                                        <td>
                                                            <table>
                                                                <tr>
                                                                    <td class="caption">From Date
                                                                    </td>
                                                                    <td class="inputData" style="width: 100px">
                                                                        <Telerik:RadDatePicker ID="dtpNumberFromDate" runat="server"
                                                                            DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                                                                        </Telerik:RadDatePicker>
                                                                    </td>

                                                                </tr>
                                                                <tr>
                                                                    <td class="caption" style="width: 50px">To Date
                                                                    </td>
                                                                    <td class="inputData">
                                                                        <Telerik:RadDatePicker ID="dtpNumberToDate" runat="server"
                                                                            DateInput-DisplayDateFormat="dd/MM/yyyy" DateInput-DateFormat="dd/MM/yyyy">
                                                                        </Telerik:RadDatePicker>
                                                                    </td>
                                                                    <td class="validation">
                                                                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="dtpNumberToDate"
                                                                            ControlToCompare="dtpNumberFromDate" Operator="GreaterThanEqual" Type="Date" ValidationGroup="NumberSearch"
                                                                            ErrorMessage="To Date should be greater than From Date." CssClass="error">
                                                                        </asp:CompareValidator>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                        <!-----  ----------------------->
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <%-- <tr>
                                            <td>
                                                <table>

                                                    </table>
                                            </td>
                                        </tr>--%>
                                        <tr>
                                            <td>
                                                <table border="1">
                                                    <tr>
                                                        <td style="width: 220px; padding-top: 12px">
                                                            <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                                             <i class="icon-ban-circle icon-white"></i> Return </asp:LinkButton>
                                                            <asp:LinkButton ID="lnkSearchNumber" runat="server" CssClass="btn btn-primary" ValidationGroup="NumberSearch" OnClick="lnkSearchNumber_Click">
                                                             <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                                        </td>
                                                        <td>
                                                            <asp:CheckBox runat="server" ID="chkThresholds" Checked="true" /></td>
                                                        <td>Subscriber Thresholds</td>
                                                        <td>
                                                            <asp:CheckBox runat="server" ID="chkNormal" Checked="true" /></td>
                                                        <td>Normal CDR</td>
                                                        <td>
                                                            <asp:CheckBox runat="server" ID="chkValues" /></td>
                                                        <td>Subscriber Values</td>
                                                        <td>
                                                            <asp:CheckBox runat="server" ID="chkDailyProfile" /></td>
                                                        <td>Daily Number Profile </td>
                                                        <td>
                                                            <asp:CheckBox runat="server" ID="chkMonthlyProfile" Checked="true" /></td>
                                                        <td>Monthly Number Profile </td>
                                                        <td>
                                                            <asp:CheckBox runat="server" ID="chkHourlyProfile" /></td>
                                                        <td>Hourly Number Profile </td>


                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <h4 class="breadcrumb">
                                                    <asp:Label runat="server" ID="lblNumberInfo"></asp:Label>
                                                </h4>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <Telerik:RadPanelBar runat="server" ID="RadPanelBar1" Height="400" Width="99%" ExpandMode="MultipleExpandedItems">

                                                    <Items>

                                                        <Telerik:RadPanelItem Text="Subscriber Thresholds" ForeColor  ="Red" Font-Bold="true"  >
                                                           
                                                            <ContentTemplate>


                                                                <Telerik:RadGrid ID="gvSubscriberThreshold" AllowSorting="true" runat="server" CellSpacing="0" ClientSettings-Scrolling-AllowScroll="true" Height="200px"
                                                                    AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvSubscriberThreshold_NeedDataSource">

                                                                    <ClientSettings>
                                                                        <Scrolling UseStaticHeaders="true" />
                                                                    </ClientSettings>

                                                                    <MasterTableView ShowGroupFooter="true">
                                                                        <Columns>

                                                                            <Telerik:GridTemplateColumn HeaderText="Day" Aggregate="Count" HeaderStyle-Width="15%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lbDateDay" Text='<%# DateTime.Parse(Eval("DateDay").ToString()).ToString("dd/MM/yyy") %>'>
                                                                             
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridBoundColumn HeaderText="Suspicion Level" DataField="Suspicion_Level.Name" HeaderStyle-Width="15%"   />


                                                                            <Telerik:GridTemplateColumn HeaderText="Criteria 1" HeaderStyle-Width="15%"  >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblCriteriaId1">
                                                                            <i class= <%# System.Convert.ToString(Eval("Criteria1"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridTemplateColumn HeaderText="Criteria 2" HeaderStyle-Width="15%"  >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblCriteria2">
                                                                            <i class= <%# System.Convert.ToString(Eval("Criteria2"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridTemplateColumn HeaderText="Criteria 3" HeaderStyle-Width="15%"  >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblCriteria3">
                                                                            <i class= <%# System.Convert.ToString(Eval("Criteria3"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridTemplateColumn HeaderText="Criteria 4" HeaderStyle-Width="15%"  >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblCriteria4">
                                                                            <i class= <%# System.Convert.ToString(Eval("Criteria4"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridTemplateColumn HeaderText="Criteria 5" HeaderStyle-Width="15%"  >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblCriteria5">
                                                                            <i class= <%# System.Convert.ToString(Eval("Criteria5"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridTemplateColumn HeaderText="Criteria 6" HeaderStyle-Width="15%"  >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblCriteria6">
                                                                            <i class= <%# System.Convert.ToString(Eval("Criteria6"))== "1" ? "icon-check-sign" :"icon-check-empty"%> ></i>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>




                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </Telerik:RadGrid>








                                                            </ContentTemplate>

                                                        </Telerik:RadPanelItem>

                                                        <Telerik:RadPanelItem Text="Normal CDR" ForeColor  ="Red" Font-Bold="true"  >

                                                            <ContentTemplate>


                                                                <Telerik:RadGrid ID="gvNormalCdr" AllowSorting="true" runat="server" CellSpacing="0" ClientSettings-Scrolling-AllowScroll="true" Height="200px"
                                                                    AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvNormalCdr_NeedDataSource">

                                                                    <ClientSettings>
                                                                        <Scrolling UseStaticHeaders="true" />
                                                                    </ClientSettings>

                                                                    <MasterTableView ShowGroupFooter="true">
                                                                        <Columns>
                                                                            <Telerik:GridBoundColumn HeaderText="CDPN" DataField="B_Temp"  Aggregate="Count"  HeaderStyle-Width="8%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Connect" DataField="ConnectDateTime" HeaderStyle-Width="13%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Disconnect" DataField="DisconnectDateTime" HeaderStyle-Width="13%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Duration (sec)" DataField="DurationInSeconds" HeaderStyle-Width="11%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Switch" DataField="Switch" HeaderStyle-Width="11%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="In Trunk" DataField="In_Trunk" HeaderStyle-Width="11%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Out Trunk" DataField="Out_Trunk" HeaderStyle-Width="11%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="In Type" DataField="In_Type" HeaderStyle-Width="11%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Out Type" DataField="Out_Type" HeaderStyle-Width="11%"   />
                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </Telerik:RadGrid>




                                                            </ContentTemplate>

                                                        </Telerik:RadPanelItem>

                                                        <Telerik:RadPanelItem Text="Subscriber Values" ForeColor  ="Red" Font-Bold="true"  >

                                                            <ContentTemplate>



                                                                <Telerik:RadGrid ID="gvSubscriberValues" AllowSorting="true" runat="server" CellSpacing="0" ClientSettings-Scrolling-AllowScroll="true" Height="200px"
                                                                    AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvSubscriberValues_NeedDataSource">

                                                                    <ClientSettings>
                                                                        <Scrolling UseStaticHeaders="true" />
                                                                    </ClientSettings>

                                                                    <MasterTableView ShowGroupFooter="true">
                                                                        <Columns>

                                                                            <Telerik:GridTemplateColumn HeaderText="From Date" Aggregate="Count"  HeaderStyle-Width="25%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblFromDate" Text='<%# DateTime.Parse(Eval("FromDate").ToString()).ToString("dd/MM/yyy") %>'>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>


                                                                            <Telerik:GridTemplateColumn HeaderText="To Date"  HeaderStyle-Width="25%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblToDate" Text='<%# DateTime.Parse(Eval("ToDate").ToString()).ToString("dd/MM/yyy") %>'>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                           
                                                                            <Telerik:GridBoundColumn HeaderText="Criteria" DataField="Criteria_Profile.Description"   HeaderStyle-Width="25%"   />
                                                                             <Telerik:GridBoundColumn HeaderText="Value" DataField="Value"   HeaderStyle-Width="25%"   />
                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </Telerik:RadGrid>


                                                            </ContentTemplate>

                                                        </Telerik:RadPanelItem>

                                                        <Telerik:RadPanelItem Text="Daily Number Profile" ForeColor  ="Red" Font-Bold="true"  >
                                                            <ContentTemplate>
                                                                <Telerik:RadGrid ID="gvDailyProfile" AllowSorting="true" runat="server" CellSpacing="0" ClientSettings-Scrolling-AllowScroll="true" Height="200px"
                                                                    AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvDailyProfile_NeedDataSource">

                                                                    <ClientSettings>
                                                                        <Scrolling UseStaticHeaders="true" />
                                                                    </ClientSettings>

                                                                    <MasterTableView ShowGroupFooter="true">
                                                                        <Columns>

                                                                            <Telerik:GridTemplateColumn HeaderText="Day" Aggregate="Count"   HeaderStyle-Width="15%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lbDateDay" Text='<%# DateTime.Parse(Eval("Date_Day").ToString()).ToString("dd/MM/yyy") %>'>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridBoundColumn HeaderText="Total Out Volume" DataField="Total_Out_Volume"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total In Volume" DataField="Total_In_Volume"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total Out Calls" DataField="Count_out" HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total In Calls" DataField="Count_in" HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Average Out Duration" DataField="Call_Out_Dur_Avg" HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Average In Duration" DataField="Call_In_Dur_Avg"  HeaderStyle-Width="15%"   />

                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </Telerik:RadGrid>

                                                            </ContentTemplate>

                                                        </Telerik:RadPanelItem>

                                                        <Telerik:RadPanelItem Text="Monthly Number Profile"  ForeColor  ="Red" Font-Bold="true"  >

                                                            <ContentTemplate>

                                                                <Telerik:RadGrid ID="gvMonthlyProfile" AllowSorting="true" runat="server" CellSpacing="0" ClientSettings-Scrolling-AllowScroll="true" Height="200px"
                                                                    AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvMonthlyProfile_NeedDataSource">

                                                                    <ClientSettings>
                                                                        <Scrolling UseStaticHeaders="true" />
                                                                    </ClientSettings>

                                                                    <MasterTableView ShowGroupFooter="true">
                                                                        <Columns>

                                                                            <Telerik:GridTemplateColumn HeaderText="Month"  Aggregate="Count"  HeaderStyle-Width="15%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lbDateDay" Text='<%# DateTime.Parse(Eval("Date_Day").ToString()).Month.ToString()+"-"+DateTime.Parse(Eval("Date_Day").ToString()).Year.ToString() %>'>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                               <Telerik:GridBoundColumn HeaderText="Total Out Volume" DataField="Total_Out_Volume"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total In Volume" DataField="Total_In_Volume"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total Out Calls" DataField="Count_out" HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total In Calls" DataField="Count_in" HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Distinct Called Numbers" DataField="Diff_Output_Numb_"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Average Out Duration" DataField="Call_Out_Dur_Avg" HeaderStyle-Width="15%"   />

                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </Telerik:RadGrid>


                                                            </ContentTemplate>

                                                        </Telerik:RadPanelItem>

                                                        <Telerik:RadPanelItem Text="Hourly Number Profile" ForeColor  ="Red" Font-Bold="true"  >

                                                            <ContentTemplate>

                                                                <Telerik:RadGrid ID="gvHourlyProfile" AllowSorting="true" runat="server" CellSpacing="0" ClientSettings-Scrolling-AllowScroll="true" Height="200px"
                                                                    AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvHourlyProfile_NeedDataSource">

                                                                    <ClientSettings>
                                                                        <Scrolling UseStaticHeaders="true" />
                                                                    </ClientSettings>

                                                                    <MasterTableView ShowGroupFooter="true">
                                                                        <Columns>

                                                                            <Telerik:GridTemplateColumn HeaderText="Day"  Aggregate="Count"  HeaderStyle-Width="12%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblDateDay" Text='<%# DateTime.Parse(Eval("Date_Day").ToString()).ToString("dd/MM/yyy") %>'>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>
                                                                            <Telerik:GridTemplateColumn HeaderText="Hour" HeaderStyle-Width="12%"   >
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblHour" Text='<%#(Eval("Day_Hour").ToString())%>'>
                                                                                    </asp:Label>
                                                                                </ItemTemplate>
                                                                            </Telerik:GridTemplateColumn>

                                                                            <Telerik:GridBoundColumn HeaderText="Total Out Volume" DataField="Total_Out_Volume"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total In Volume" DataField="Total_In_Volume"  HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Total Out Calls" DataField="Count_out" HeaderStyle-Width="15%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Distinct Called Numbers" DataField="Diff_Output_Numb_"  HeaderStyle-Width="12%"   />
                                                                            <Telerik:GridBoundColumn HeaderText="Average Out Duration" DataField="Call_Out_Dur_Avg"  HeaderStyle-Width="12%"   />

                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </Telerik:RadGrid>




                                                            </ContentTemplate>

                                                        </Telerik:RadPanelItem>



                                                    </Items>

                                                </Telerik:RadPanelBar>








                                            </td>
                                        </tr>

                                    </tbody>
                                </table>

                                <%-- </div>--%>

                            </td>
                        </tr>
                        <tr>
                            <td class="space10"></td>
                        </tr>
                        <%-- <tr>
                            <td style="text-align: center;">
                                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-danger" OnClick="btnCancel_Click" CausesValidation="false">
                                    <i class="icon-ban-circle icon-white"></i>
                                          Return
                                </asp:LinkButton>
                            </td>
                        </tr>--%>
                    </table>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
    <style type="text/css">
        .auto-style1
        {
            width: 100%;
        }
    </style>
</asp:Content>

