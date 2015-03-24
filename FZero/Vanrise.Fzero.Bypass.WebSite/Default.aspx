<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="Server">


    

    
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanelMain"  BackImageUrl="~/Images/framely.gif" BackgroundPosition="Center"    runat="server" ></telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxPanel ID="RadAjaxPanelMain" LoadingPanelID="RadAjaxLoadingPanelMain"  EnableAJAX="true"    runat ="server"  Width="100%">


    <table cellpadding="0" cellspacing="0" width="100%" >
        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">
                    <tr id="trSearch">
                        <td valign="top">
                            <table  width="100%">
                                <tr>
                                    <td>
                                        <table >
                                            <tr>

                                                <td >&nbsp;</td>

                                                <td>
                                                    <%=Resources.Resources.DateRange %>
                                                </td>
                                                <td ></td>
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



                                                <td >&nbsp;</td>

                                                <td>
                                                    <%=Resources.Resources.From %>
                                                </td>
                                                <td ></td>
                                                <td>
                                                    <telerik:RadDateTimePicker ID="rdpFromLogDate" runat="server"  >
                                                        <TimeView CellSpacing="-1"></TimeView>

                                                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

                                                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

                                                        <DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" EnableSingleInputRendering="True" LabelWidth="64px" AutoPostBack="True"></DateInput>

                                                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                                                    </telerik:RadDateTimePicker>
                                                </td>


                                                <td ></td>

                                                <td >&nbsp;</td>

                                                <td>
                                                    <%=Resources.Resources.To %>
                                                </td>
                                                <td ></td>
                                                <td>
                                                    <telerik:RadDateTimePicker ID="rdpToLogDate" runat="server" >
                                                        <TimeView CellSpacing="-1"></TimeView>

                                                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>

                                                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x"></Calendar>

                                                        <DateInput DisplayDateFormat="M/d/yyyy" DateFormat="M/d/yyyy" EnableSingleInputRendering="True" LabelWidth="64px" AutoPostBack="True"></DateInput>

                                                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                                                    </telerik:RadDateTimePicker>
                                                </td>

                                                <td >
                                                    <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary" OnClick="btnSearch_Click">
                                                                 <i class="icon-search icon-white"></i> Search </asp:LinkButton>
                                                </td>
                                            </tr>





                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>

                    <tr>
                        <td style="vertical-align: top">
                            <table width="100%" cellpadding="2" cellspacing="2">



                                <tr>
                                    <td style="vertical-align: top">




                                        <telerik:RadDockLayout runat="server" ID="RadDockLayout1">

                                            <div style="vertical-align: top;">

                                                <table>


                                                    <tr>
                                                        <td style="width: 33%; vertical-align: top">

                                                            <telerik:RadDockZone runat="server" ID="RadDockZone2" Orientation="vertical" Style="border: 0px; width: 100%;">


                                                                <telerik:RadDock runat="server" ID="rdGeneratedCalls" Title="Generated Calls" Width="100%"
                                                                    EnableAnimation="true" DefaultCommands="None">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvGeneratedCalls" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvGeneratedCalls_NeedDataSource" ShowFooter="True">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />


                                                                                    <telerik:GridBoundColumn DataField="attemptdate" UniqueName="attemptdate" DataFormatString="{0:dd/MM/yyyy}" />

                                                                                    <telerik:GridNumericColumn DataField="CountGenerated"  DataFormatString="{0:#,##0}"
                                                                                        HeaderText="Total" SortExpression="CountGenerated" UniqueName="CountGenerated" Aggregate="Sum" FooterText="Total : " ReadOnly="True">
                                                                                        <FooterStyle HorizontalAlign="Center" Font-Bold="True" CssClass="SoldFooter"></FooterStyle>
                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                    </telerik:GridNumericColumn>


                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>

                                                                <telerik:RadDock runat="server" ID="rdReceivedCalls" Title="Received Calls" Width="100%" EnableAnimation="true" DefaultCommands="None">

                                                                    <ContentTemplate>

                                                                        <telerik:RadGrid AllowSorting="true" ID="gvReceivedCalls" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvReceivedCalls_NeedDataSource" ShowFooter="true">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />
                                                                                    <telerik:GridBoundColumn DataField="ClientName" ItemStyle-Font-Bold="true" UniqueName="ClientName" />
                                                                                    <telerik:GridBoundColumn DataField="attemptdate" UniqueName="attemptdate" DataFormatString="{0:dd/MM/yyyy}" />
                                                                                    <telerik:GridNumericColumn DataField="CountRecieved" DataType="System.Decimal"
                                                                                        HeaderText="Total" SortExpression="CountRecieved" UniqueName="CountRecieved" DataFormatString="{0:#,##0}"  Aggregate="Sum" FooterText="Total : " ReadOnly="True">
                                                                                        <FooterStyle HorizontalAlign="Center" Font-Bold="True" CssClass="SoldFooter"></FooterStyle>
                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                    </telerik:GridNumericColumn>


                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>

                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>

                                                                <telerik:RadDock runat="server" ID="rdGeneratedReceived" Title="Generated vs Received Calls" Width="100%" EnableAnimation="true" DefaultCommands="None">

                                                                    <ContentTemplate>

                                                                        <telerik:RadGrid AllowSorting="true" ID="gvGeneratedRecieved" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvGeneratedRecieved_NeedDataSource" ShowFooter="true">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>

                                                                                    <telerik:GridBoundColumn DataField="SentBy" UniqueName="SentBy" />
                                                                                    <telerik:GridBoundColumn DataField="RecievedBy" UniqueName="RecievedBy" />
                                                                                    <telerik:GridBoundColumn DataField="ClientName" ItemStyle-Font-Bold="true" UniqueName="ClientName" />
                                                                                    <telerik:GridBoundColumn DataField="attemptdate" UniqueName="attemptdate" DataFormatString="{0:dd/MM/yyyy}" />
                                                                                    <telerik:GridNumericColumn DataField="CountGenerated" DataType="System.Decimal" DataFormatString="{0:#,##0}"
                                                                                        HeaderText="Total" SortExpression="CountGenerated" UniqueName="CountGenerated" Aggregate="Sum" FooterText="Total : " ReadOnly="True">
                                                                                        <FooterStyle HorizontalAlign="Center" Font-Bold="True" CssClass="SoldFooter"></FooterStyle>
                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                    </telerik:GridNumericColumn>

                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>

                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>

                                                            </telerik:RadDockZone>

                                                        </td>
                                                        <td style="width: 33%; vertical-align: top">

                                                            <telerik:RadDockZone runat="server" ID="RadDockZone1" Orientation="vertical" Style="border: 0; width: 100%;">

                                                                <telerik:RadDock runat="server" ID="rdCases" Title="Cases" Width="100%" DefaultCommands="None"
                                                                    EnableAnimation="true">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvCases" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvCases_NeedDataSource" OnItemDataBound="gvCases_ItemDataBound">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView DataKeyNames="Name">
                                                                                <Columns>
                                                                                    <telerik:GridTemplateColumn UniqueName="Name">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name").ToString() %>'></asp:Label>
                                                                                        </ItemTemplate>
                                                                                    </telerik:GridTemplateColumn>
                                                                                    <telerik:GridNumericColumn DataField="CountGenerated" UniqueName="CountGenerated"  DataFormatString="{0:#,##0}"    />
                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>

                                                                <telerik:RadDock runat="server" ID="RadDockClientCases" Title="Cases Detailed" Width="100%" DefaultCommands="None"
                                                                    EnableAnimation="true">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvClientCases" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvClientCases_NeedDataSource" OnItemDataBound="gvClientCases_ItemDataBound">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView DataKeyNames="Status">
                                                                                <Columns>

                                                                                    <telerik:GridBoundColumn DataField="ClientName" UniqueName="ClientName" ItemStyle-Font-Bold="true" />
                                                                                    <telerik:GridBoundColumn DataField="GeneratedBy" UniqueName="GeneratedBy" />
                                                                                    <telerik:GridBoundColumn DataField="ReceivedBy" UniqueName="ReceivedBy" />

                                                                                    <telerik:GridTemplateColumn UniqueName="Status">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("Status").ToString() %>'></asp:Label>
                                                                                        </ItemTemplate>
                                                                                    </telerik:GridTemplateColumn>
                                                                                    <telerik:GridNumericColumn DataField="Count" UniqueName="Count"  DataFormatString="{0:#,##0}"  />
                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>



                                                                <telerik:RadDock runat="server" ID="rdReports" Title="Reports" Width="100%" DefaultCommands="None"
                                                                    EnableAnimation="true">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvReports" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvReports_NeedDataSource">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn DataField="FullName" UniqueName="FullName" />
                                                                                    <telerik:GridBoundColumn DataField="ClientName" ItemStyle-Font-Bold="true" UniqueName="ClientName" />
                                                                                    <telerik:GridBoundColumn DataField="SentDateTime" UniqueName="SentDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                                                    <telerik:GridNumericColumn DataField="Cases" UniqueName="Cases" DataFormatString="{0:#,##0}" />

                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>


                                                            </telerik:RadDockZone>

                                                        </td>
                                                        <td style="width: 33%; vertical-align: top">

                                                            <telerik:RadDockZone runat="server" ID="RadDockZone3" Orientation="vertical" Style="border: 0; width: 100%;">
                                                                <telerik:RadDock runat="server" ID="rdCompares" Title="Compares" Width="100%" DefaultCommands="None"
                                                                    EnableAnimation="true">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvCompares" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvCompares_NeedDataSource">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn DataField="Type" UniqueName="Type" />
                                                                                    <telerik:GridBoundColumn DataField="VarCompareDateTime" UniqueName="VarCompareDateTime" />
                                                                                    <telerik:GridNumericColumn DataField="CountCalls" UniqueName="CountCalls" DataFormatString="{0:#,##0}" />
                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>
                                                                <telerik:RadDock runat="server" ID="rdLastImports" Title="Last Import of each source" Width="100%" DefaultCommands="None"
                                                                    EnableAnimation="true">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvLastImports" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnItemDataBound="gvLastImports_ItemDataBound" OnNeedDataSource="gvLastImports_NeedDataSource" MasterTableView-DataKeyNames="DiffHour">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />
                                                                                    <telerik:GridBoundColumn DataField="DiffHour" UniqueName="DiffHour" Visible="false" />
                                                                                    <telerik:GridBoundColumn DataField="VarImportDateTime" UniqueName="VarImportDateTime" />
                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>

                                                                <telerik:RadDock runat="server" ID="rdImports" Title="Imports" Width="100%" DefaultCommands="None"
                                                                    EnableAnimation="true">

                                                                    <ContentTemplate>


                                                                        <telerik:RadGrid AllowSorting="true" ID="gvImports" runat="server" CellSpacing="0" Height="125px" ShowHeader="false" BorderColor="White"
                                                                            AutoGenerateColumns="False" OnNeedDataSource="gvImports_NeedDataSource" ShowFooter="true">

                                                                            <ClientSettings>
                                                                                <Selecting AllowRowSelect="True"></Selecting>
                                                                            </ClientSettings>

                                                                            <MasterTableView>
                                                                                <Columns>
                                                                                    <telerik:GridBoundColumn DataField="Name" UniqueName="Name" />

                                                                                    <telerik:GridBoundColumn DataField="CountImportedCalls" DataType="System.Decimal"
                                                                                        HeaderText="Total" SortExpression="CountImportedCalls" UniqueName="CountImportedCalls" Aggregate="Sum" FooterText="Total : " ReadOnly="True">
                                                                                        <FooterStyle HorizontalAlign="Center" Font-Bold="True" CssClass="SoldFooter"></FooterStyle>
                                                                                        <HeaderStyle HorizontalAlign="Center" />
                                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                                    </telerik:GridBoundColumn>


                                                                                    <telerik:GridBoundColumn DataField="VarImportDateTime" UniqueName="VarImportDateTime" />
                                                                                </Columns>
                                                                            </MasterTableView>
                                                                        </telerik:RadGrid>


                                                                    </ContentTemplate>

                                                                    <Commands>
                                                                        <telerik:DockExpandCollapseCommand />
                                                                    </Commands>

                                                                </telerik:RadDock>



                                                            </telerik:RadDockZone>

                                                        </td>





                                                    </tr>

                                                </table>

                                            </div>

                                        </telerik:RadDockLayout>


                                    </td>
                                </tr>
                            </table>


                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>



    </telerik:RadAjaxPanel>






</asp:Content>

