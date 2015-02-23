<%@ Page Title="" Language="C#" MasterPageFile="~/Master.master" AutoEventWireup="true" CodeFile="DefaultMobileOperator.aspx.cs" Inherits="DefaultMobileOperator" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Charting" TagPrefix="telerik" %>

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
        <tr id="trData" runat="server" valign="top">
            <td>
                <table width="100%">

                    <tr>
                        <td style="vertical-align: top; width: 20%">
                            <table width="100%" cellpadding="2" cellspacing="2" style="border-style: solid; border-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C; border-top-color: #3D556C">







                                <tr>
                                    <td style="vertical-align: top; width: 100%">



                                        <telerik:RadDockZone EnableDrag="false" runat="server" ID="RadDockZone3" Orientation="vertical" Style="border: 0px; width: 100%;">


                                            <telerik:RadDock EnableDrag="false" runat="server" ID="rdCases" Title="Cases" Width="100%"
                                                EnableAnimation="true" DefaultCommands="None">



                                                <ContentTemplate>


                                                    <telerik:RadGrid AllowSorting="true" ID="gvCases" runat="server" CellSpacing="0" Height="150px" ShowHeader="false" BorderColor="White"
                                                        AutoGenerateColumns="False" OnNeedDataSource="gvCases_NeedDataSource">

                                                        <ClientSettings>
                                                            <Selecting AllowRowSelect="True"></Selecting>
                                                        </ClientSettings>

                                                        <MasterTableView DataKeyNames="CallType">
                                                            <Columns>
                                                                <telerik:GridTemplateColumn UniqueName="CallType">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblCallType" Font-Size="10" runat="server" Text='<%# Eval("CallType").ToString() %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>

                                                                <telerik:GridTemplateColumn UniqueName="count">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblcount" Font-Bold="true" Font-Size="10" runat="server" Text='<%#  int.Parse(Eval("count").ToString()).ToString(Vanrise.CommonLibrary.Formatter.AmountFormat) %>' ></asp:Label>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>


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






                               <tr>
                                    <td style="vertical-align: top; width: 100%">
                                        <telerik:RadDockZone EnableDrag="false" runat="server" ID="rdzFrom" Orientation="vertical" Style="border: 0px; width: 100%;">


                                            <telerik:RadDock EnableDrag="false" runat="server" ID="rdFrom" Title="From - To" Width="100%"
                                                EnableAnimation="true" DefaultCommands="None">

                                                <ContentTemplate>

                                                    <table style="width: 100%">
                                                        <tr>


                                                            <td>
                                                                <telerik:RadDateTimePicker ID="rdpFrom" runat="server">
                                                                </telerik:RadDateTimePicker>
                                                            </td>
                                                           
                                                        </tr>


                                                        <tr>


                                                            <td>
                                                                <telerik:RadDateTimePicker ID="rdpTo" runat="server">
                                                                </telerik:RadDateTimePicker>
                                                            </td>
                                                           
                                                        </tr>
                                                        <tr>
                                                            <td >
                                                                <telerik:RadButton ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click">
                                                                </telerik:RadButton>
                                                            </td>
                                                        </tr>
                                                    </table>



                                                </ContentTemplate>
                                            </telerik:RadDock>
                                        </telerik:RadDockZone>
                                    </td>

                                </tr>
                                <tr>
                                    <td style="vertical-align: top; width: 100%">
                                        <telerik:RadDockZone EnableDrag="false" runat="server" ID="RadDockZone4" Orientation="vertical" Style="border: 0px; width: 100%;">


                                            <telerik:RadDock EnableDrag="false" runat="server" ID="RadDock3" Title="Date Range" Width="100%" Height="190px"
                                                EnableAnimation="true" DefaultCommands="None">

                                                <ContentTemplate>

                                                    <asp:RadioButtonList ID="rblDateRange" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rblDateRange_SelectedIndexChanged" RepeatColumns="1">
                                                        <asp:ListItem Value="0">Specific Date</asp:ListItem>
                                                        <asp:ListItem Selected="True" Value="1">Today</asp:ListItem>
                                                        <asp:ListItem Value="2">Yesterday</asp:ListItem>
                                                        <asp:ListItem Value="3">This Week</asp:ListItem>
                                                        <asp:ListItem Value="4">This Month</asp:ListItem>
                                                        <asp:ListItem Value="5">This Year</asp:ListItem>
                                                        <asp:ListItem Value="6">Last Week</asp:ListItem>
                                                        <asp:ListItem Value="7">Last Month</asp:ListItem>
                                                    </asp:RadioButtonList>

                                                </ContentTemplate>
                                            </telerik:RadDock>
                                        </telerik:RadDockZone>
                                    </td>

                                </tr>

                            </table>


                        </td>

                        <td style="vertical-align: top; width: 60%">


                            <telerik:RadTabStrip ID="rtsMain" runat="server" ShowBaseLine="true" AutoPostBack="True" SelectedIndex="0" MultiPageID="RadMultiPageMain" OnTabClick="rtsMain_TabClick">
                                <Tabs>





                                    <telerik:RadTab runat="server" Text="Origination: Route" Selected="True">
                                    </telerik:RadTab>

                                    <telerik:RadTab runat="server" Text="Origination: Zone">
                                    </telerik:RadTab>

                                    <telerik:RadTab runat="server" Text="Time Active">
                                    </telerik:RadTab>


                                    <telerik:RadTab runat="server" Text="Onnet Cases">
                                    </telerik:RadTab>

                                    <telerik:RadTab runat="server" Text="Ofnet Cases">
                                    </telerik:RadTab>

                                    <telerik:RadTab runat="server" Text="Call Summary">
                                    </telerik:RadTab>

                                    <telerik:RadTab runat="server" Text="Cases">
                                    </telerik:RadTab>

                                </Tabs>
                            </telerik:RadTabStrip>


                            <table width="100%" cellpadding="2" cellspacing="2" style="border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C">



                                <tr>
                                    <td style="vertical-align: top; width: 100%">


                                        <telerik:RadMultiPage ID="RadMultiPageMain" runat="server" SelectedIndex="0">

                                              <telerik:RadPageView ID="RadPageViewCarrier" runat="server">
                                                <telerik:RadChart ID="rcCarrier" runat="server" DefaultType="Pie" Skin="BlueStripes" Width="800px" Height="550px">
                                                    <Appearance>
                                                        <FillStyle MainColor="225, 235, 238" FillType="Hatch" SecondColor="207, 223, 229">
                                                        </FillStyle>
                                                        <Border Color="131, 171, 184" />
                                                    </Appearance>
                                                    <Series>
                                                        <telerik:ChartSeries DataYColumn="Count" Name="Series 1" Type="Pie">
                                                            <Appearance>
                                                                <FillStyle FillType="ComplexGradient" MainColor="222, 202, 152">
                                                                    <FillSettings>
                                                                        <ComplexGradient>
                                                                            <telerik:GradientElement Color="222, 202, 152" />
                                                                            <telerik:GradientElement Color="211, 185, 123" Position="0.5" />
                                                                            <telerik:GradientElement Color="183, 154, 84" Position="1" />
                                                                        </ComplexGradient>
                                                                    </FillSettings>
                                                                </FillStyle>
                                                                <TextAppearance TextProperties-Color="62, 117, 154">
                                                                </TextAppearance>
                                                                <Border Color="187, 149, 58" />
                                                            </Appearance>
                                                        </telerik:ChartSeries>
                                                    </Series>
                                                    <Legend>
                                                        <Appearance Dimensions-Margins="1px, 1%, 11%, 1px">
                                                            <ItemTextAppearance TextProperties-Color="81, 103, 114">
                                                            </ItemTextAppearance>
                                                            <FillStyle MainColor="241, 253, 255">
                                                            </FillStyle>
                                                            <Border Color="193, 214, 221" />
                                                        </Appearance>
                                                    </Legend>
                                                    <PlotArea>
                                                        <XAxis>
                                                            <Appearance Color="193, 214, 221" MajorTick-Color="154, 153, 129">
                                                                <MajorGridLines Color="221, 227, 221" Width="0" />
                                                                <TextAppearance TextProperties-Color="102, 103, 86">
                                                                </TextAppearance>
                                                            </Appearance>
                                                            <AxisLabel>
                                                                <TextBlock>
                                                                    <Appearance TextProperties-Color="102, 103, 86">
                                                                    </Appearance>
                                                                </TextBlock>
                                                            </AxisLabel>
                                                        </XAxis>
                                                        <YAxis>
                                                            <Appearance Color="193, 214, 221" MajorTick-Color="154, 153, 129" MinorTick-Color="193, 214, 221">
                                                                <MajorGridLines Color="221, 227, 221" />
                                                                <MinorGridLines Color="221, 227, 221" />
                                                                <TextAppearance TextProperties-Color="102, 103, 86">
                                                                </TextAppearance>
                                                            </Appearance>
                                                            <AxisLabel>
                                                                <TextBlock>
                                                                    <Appearance TextProperties-Color="102, 103, 86">
                                                                    </Appearance>
                                                                </TextBlock>
                                                            </AxisLabel>
                                                        </YAxis>
                                                        <Appearance Dimensions-Margins="18%, 21%, 12%, 8%">
                                                            <FillStyle MainColor="241, 253, 255" SecondColor="Transparent">
                                                            </FillStyle>
                                                            <Border Color="193, 214, 221" />
                                                        </Appearance>
                                                    </PlotArea>
                                                    <ChartTitle>
                                                        <Appearance Dimensions-Margins="3%, 10px, 14px, 5%">
                                                            <FillStyle MainColor="">
                                                            </FillStyle>
                                                        </Appearance>
                                                        <TextBlock Text="">
                                                            <Appearance TextProperties-Color="81, 103, 114" TextProperties-Font="Verdana, 18pt">
                                                            </Appearance>
                                                        </TextBlock>
                                                    </ChartTitle>
                                                </telerik:RadChart>


                                            </telerik:RadPageView>

                                            <telerik:RadPageView ID="RadPageViewOrigination" runat="server">
                                                <telerik:RadChart ID="rcOrigination" runat="server" DefaultType="Pie" Skin="BlueStripes" Width="800px" Height="550px">
                                                    <Appearance>
                                                        <FillStyle FillType="Hatch" MainColor="225, 235, 238" SecondColor="207, 223, 229">
                                                        </FillStyle>
                                                        <Border Color="131, 171, 184" />
                                                    </Appearance>
                                                    <Series>
                                                        <telerik:ChartSeries DataYColumn="Count" Name="Series 1" Type="Pie">
                                                            <Appearance>
                                                                <FillStyle FillType="ComplexGradient" MainColor="222, 202, 152">
                                                                    <FillSettings>
                                                                        <ComplexGradient>
                                                                            <telerik:GradientElement Color="222, 202, 152" />
                                                                            <telerik:GradientElement Color="211, 185, 123" Position="0.5" />
                                                                            <telerik:GradientElement Color="183, 154, 84" Position="1" />
                                                                        </ComplexGradient>
                                                                    </FillSettings>
                                                                </FillStyle>
                                                                <TextAppearance TextProperties-Color="62, 117, 154">
                                                                </TextAppearance>

                                                                <LabelAppearance Visible="True" Position-Auto="true">
                                                                </LabelAppearance>


                                                                <Border Color="187, 149, 58" />
                                                            </Appearance>



                                                        </telerik:ChartSeries>
                                                    </Series>
                                                    <Legend>
                                                        <Appearance Dimensions-Margins="1px, 1%, 11%, 1px">
                                                            <ItemTextAppearance TextProperties-Color="81, 103, 114">
                                                            </ItemTextAppearance>
                                                            <FillStyle MainColor="241, 253, 255">
                                                            </FillStyle>
                                                            <Border Color="193, 214, 221" />
                                                        </Appearance>
                                                    </Legend>
                                                    <PlotArea>
                                                        <XAxis>
                                                            <Appearance Color="193, 214, 221" MajorTick-Color="154, 153, 129">
                                                                <MajorGridLines Color="221, 227, 221" Width="0" />
                                                                <TextAppearance TextProperties-Color="102, 103, 86">
                                                                </TextAppearance>
                                                            </Appearance>
                                                            <AxisLabel>
                                                                <TextBlock>
                                                                    <Appearance TextProperties-Color="102, 103, 86">
                                                                    </Appearance>
                                                                </TextBlock>
                                                            </AxisLabel>
                                                        </XAxis>
                                                        <YAxis>
                                                            <Appearance Color="193, 214, 221" MajorTick-Color="154, 153, 129" MinorTick-Color="193, 214, 221">
                                                                <MajorGridLines Color="221, 227, 221" />
                                                                <MinorGridLines Color="221, 227, 221" />
                                                                <TextAppearance TextProperties-Color="102, 103, 86">
                                                                </TextAppearance>
                                                            </Appearance>
                                                            <AxisLabel>
                                                                <TextBlock>
                                                                    <Appearance TextProperties-Color="102, 103, 86">
                                                                    </Appearance>
                                                                </TextBlock>
                                                            </AxisLabel>
                                                        </YAxis>
                                                        <Appearance Dimensions-Margins="18%, 21%, 12%, 8%">
                                                            <FillStyle MainColor="241, 253, 255" SecondColor="Transparent">
                                                            </FillStyle>
                                                            <Border Color="193, 214, 221" />
                                                        </Appearance>
                                                    </PlotArea>
                                                    <ChartTitle>
                                                        <Appearance Dimensions-Margins="3%, 10px, 14px, 5%">
                                                            <FillStyle MainColor="">
                                                            </FillStyle>
                                                        </Appearance>
                                                        <TextBlock Text="">
                                                            <Appearance TextProperties-Color="81, 103, 114" TextProperties-Font="Verdana, 18pt">
                                                            </Appearance>
                                                        </TextBlock>
                                                    </ChartTitle>
                                                </telerik:RadChart>




                                            </telerik:RadPageView>

                                            <telerik:RadPageView ID="RadPageViewActiveTime" runat="server">

                                                <telerik:RadChart ID="rcActiveTime" runat="server" DefaultType="Pie" Skin="BlueStripes" Width="800px" Height="550px" SeriesOrientation="Vertical">
                                                    <Appearance>
                                                        <FillStyle MainColor="225, 235, 238" FillType="Hatch" SecondColor="207, 223, 229">
                                                        </FillStyle>
                                                        <Border Color="131, 171, 184" />
                                                    </Appearance>
                                                    <Series>
                                                        <telerik:ChartSeries DataYColumn="Attempts" Name="Series 1" Type="Pie">
                                                            <Appearance>
                                                                <FillStyle FillType="ComplexGradient" MainColor="222, 202, 152">
                                                                    <FillSettings>
                                                                        <ComplexGradient>
                                                                            <telerik:GradientElement Color="222, 202, 152" />
                                                                            <telerik:GradientElement Color="211, 185, 123" Position="0.5" />
                                                                            <telerik:GradientElement Color="183, 154, 84" Position="1" />
                                                                        </ComplexGradient>
                                                                    </FillSettings>
                                                                </FillStyle>
                                                                <TextAppearance TextProperties-Color="62, 117, 154">
                                                                </TextAppearance>
                                                                <Border Color="187, 149, 58" />
                                                            </Appearance>
                                                        </telerik:ChartSeries>
                                                    </Series>
                                                    <Legend>
                                                        <Appearance Dimensions-Margins="1px, 1%, 11%, 1px">
                                                            <ItemTextAppearance TextProperties-Color="81, 103, 114">
                                                            </ItemTextAppearance>
                                                            <FillStyle MainColor="241, 253, 255">
                                                            </FillStyle>
                                                            <Border Color="193, 214, 221" />
                                                        </Appearance>
                                                    </Legend>
                                                    <PlotArea>
                                                        <XAxis>
                                                            <Appearance Color="193, 214, 221" MajorTick-Color="154, 153, 129">
                                                                <MajorGridLines Color="221, 227, 221" Width="0" />
                                                                <TextAppearance TextProperties-Color="102, 103, 86">
                                                                </TextAppearance>
                                                            </Appearance>
                                                            <AxisLabel>
                                                                <Appearance RotationAngle="270">
                                                                </Appearance>
                                                                <TextBlock>
                                                                    <Appearance TextProperties-Color="102, 103, 86">
                                                                    </Appearance>
                                                                </TextBlock>
                                                            </AxisLabel>
                                                        </XAxis>
                                                        <YAxis>
                                                            <Appearance Color="193, 214, 221" MajorTick-Color="154, 153, 129" MinorTick-Color="193, 214, 221">
                                                                <MajorGridLines Color="221, 227, 221" />
                                                                <MinorGridLines Color="221, 227, 221" />
                                                                <TextAppearance TextProperties-Color="102, 103, 86">
                                                                </TextAppearance>
                                                            </Appearance>
                                                            <AxisLabel>
                                                                <Appearance RotationAngle="0">
                                                                </Appearance>
                                                                <TextBlock>
                                                                    <Appearance TextProperties-Color="102, 103, 86">
                                                                    </Appearance>
                                                                </TextBlock>
                                                            </AxisLabel>
                                                        </YAxis>
                                                        <YAxis2>
                                                            <AxisLabel>
                                                                <Appearance RotationAngle="0">
                                                                </Appearance>
                                                            </AxisLabel>
                                                        </YAxis2>
                                                        <Appearance Dimensions-Margins="18%, 21%, 12%, 8%">
                                                            <FillStyle MainColor="241, 253, 255" SecondColor="Transparent">
                                                            </FillStyle>
                                                            <Border Color="193, 214, 221" />
                                                        </Appearance>
                                                    </PlotArea>
                                                    <ChartTitle>
                                                        <Appearance Dimensions-Margins="3%, 10px, 14px, 5%">
                                                            <FillStyle MainColor="">
                                                            </FillStyle>
                                                        </Appearance>
                                                        <TextBlock Text="">
                                                            <Appearance TextProperties-Color="81, 103, 114" TextProperties-Font="Verdana, 18pt">
                                                            </Appearance>
                                                        </TextBlock>
                                                    </ChartTitle>
                                                </telerik:RadChart>


                                            </telerik:RadPageView>
                                            
                                            <telerik:RadPageView ID="RadPageViewFraudOnnet" runat="server">

                                                <telerik:RadButton ID="btnExportOnnetFraud" runat="server" OnClick="btnExportOnnetFraud_Click" Text="Export">
                                                    <Icon PrimaryIconUrl="~/Icons/icon_excel.png" />
                                                </telerik:RadButton>

                                                <telerik:RadGrid ID="gvOnnetFraudGeneratedCalls"  MasterTableView-Caption="Fraud Cases" ShowGroupPanel="true" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize="15"
                                                    AllowPaging="true" AllowCustomPaging="true" AllowMultiRowSelection="true" 
                                                    AutoGenerateColumns="False" OnItemDataBound="gvOnnetFraudGeneratedCalls_ItemDataBound" ShowFooter="true" OnNeedDataSource="gvOnnetFraudGeneratedCalls_NeedDataSource">

                                                    <MasterTableView ShowGroupFooter="true">
                                                        <Columns>
                                                            <telerik:GridBoundColumn DataField="CaseID" UniqueName="CaseID" Aggregate="Count" HeaderStyle-Width="120px" ItemStyle-Width="120px" />
                                                            <telerik:GridBoundColumn DataField="Carrier" UniqueName="Carrier" />
                                                            <telerik:GridBoundColumn DataField="OriginationNetwork" UniqueName="OriginationNetwork" />
                                                            <telerik:GridBoundColumn DataField="CLI" UniqueName="CLI" />
                                                            <telerik:GridBoundColumn DataField="Occurance" UniqueName="Occurance" />
                                                            <telerik:GridBoundColumn DataField="FirstAttemptDateTime" UniqueName="FirstAttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                            <telerik:GridBoundColumn DataField="LastAttemptDateTime" UniqueName="LastAttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />


                                                        </Columns>
                                                    </MasterTableView>
                                                </telerik:RadGrid>

                                            </telerik:RadPageView>

                                            <telerik:RadPageView ID="RadPageViewFraudOfnet" runat="server">

                                                <telerik:RadButton ID="btnExportOfnetFraud" runat="server" OnClick="btnExportOfnetFraud_Click" Text="Export">
                                                    <Icon PrimaryIconUrl="~/Icons/icon_excel.png" />
                                                </telerik:RadButton>

                                                <telerik:RadGrid ID="gvOfnetFraudGeneratedCalls" MasterTableView-Caption="Fraud Cases" ShowGroupPanel="true" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize="15"
                                                    AllowPaging="true" AllowCustomPaging="true" AllowMultiRowSelection="true" 
                                                    AutoGenerateColumns="False" OnItemDataBound="gvOfnetFraudGeneratedCalls_ItemDataBound" ShowFooter="true" OnNeedDataSource="gvOfnetFraudGeneratedCalls_NeedDataSource">

                                                    <MasterTableView ShowGroupFooter="true">
                                                        <Columns>
                                                            <telerik:GridBoundColumn DataField="CaseID" UniqueName="CaseID" Aggregate="Count" HeaderStyle-Width="120px" ItemStyle-Width="120px" />
                                                            <telerik:GridBoundColumn DataField="Carrier" UniqueName="Carrier" />
                                                            <telerik:GridBoundColumn DataField="OriginationNetwork" UniqueName="OriginationNetwork" />
                                                            <telerik:GridBoundColumn DataField="CLI" UniqueName="CLI" />
                                                            <telerik:GridBoundColumn DataField="Occurance" UniqueName="Occurance" />
                                                            <telerik:GridBoundColumn DataField="FirstAttemptDateTime" UniqueName="FirstAttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />
                                                            <telerik:GridBoundColumn DataField="LastAttemptDateTime" UniqueName="LastAttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />


                                                        </Columns>
                                                    </MasterTableView>
                                                </telerik:RadGrid>

                                            </telerik:RadPageView>

                                             <telerik:RadPageView ID="RadPageViewAll" runat="server">
                                                <telerik:RadButton ID="btnExport" runat="server" OnClick="btnExport_Click" Text="Export">
                                                    <Icon PrimaryIconUrl="~/Icons/icon_excel.png" />
                                                </telerik:RadButton>
                                                <telerik:RadGrid ID="gvAllGeneratedCalls" ShowGroupPanel="true" MasterTableView-Caption="Call Summary" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize="15"
                                                   AllowPaging="true" AllowCustomPaging="true" AllowMultiRowSelection="true"
                                                    AutoGenerateColumns="False" OnItemDataBound="gvAllGeneratedCalls_ItemDataBound" ShowFooter="true" OnNeedDataSource="gvAllGeneratedCalls_NeedDataSource">

                                                    <MasterTableView ShowGroupFooter="true">
                                                        <Columns>
                                                            <telerik:GridBoundColumn DataField="CaseID" UniqueName="CaseID" Aggregate="Count" HeaderStyle-Width="120px" ItemStyle-Width="120px" />
                                                            <telerik:GridBoundColumn DataField="Carrier" UniqueName="Carrier" />
                                                            <telerik:GridBoundColumn DataField="OriginationNetwork" UniqueName="OriginationNetwork" />

                                                            <telerik:GridTemplateColumn UniqueName="StatusName">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblStatusName" runat="server" Text='<%# Eval("StatusName").ToString() %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>

                                                            <telerik:GridBoundColumn DataField="b_number" UniqueName="b_number" HeaderStyle-Width="100px" />
                                                            <telerik:GridBoundColumn DataField="CLI" UniqueName="CLI" />
                                                            <telerik:GridBoundColumn DataField="AttemptDateTime" UniqueName="AttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />

                                                        </Columns>
                                                    </MasterTableView>
                                                </telerik:RadGrid>

                                            </telerik:RadPageView>

                                            <telerik:RadPageView ID="RadPageViewCases" runat="server">
                                                <table width="100%">
                                                    <tr>
                                                        <td class="right">
                                                            <asp:TextBox ID="txtFeedbackNotes" TextMode="MultiLine" Rows="2" Width="400px" runat="server" Text=""></asp:TextBox>
                                                            <telerik:RadComboBox ID="ddlMobileOperatorFeedback" DropDownWidth="200px" runat="server"></telerik:RadComboBox>

                                                            <telerik:RadButton ID="btnConfirm" runat="server" OnClick="btnConfirm_Click">
                                                                <Icon PrimaryIconUrl="Icons/Check.png" />
                                                            </telerik:RadButton>

                                                        </td>
                                                    </tr>
                                                    <tr class="vspace-5">
                                                        <td></td>
                                                    </tr>




                                                    <tr>
                                                        <td>
                                                            <telerik:RadGrid ID="gvGeneratedCalls" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize='<%# Vanrise.Fzero.Bypass.SysParameter.Global_GridPageSize %>'
                                                                AllowPaging="true" OnItemDataBound="gvGeneratedCalls_ItemDataBound" OnItemCommand="gvGeneratedCalls_ItemCommand" AllowMultiRowSelection="true" ClientSettings-Scrolling-AllowScroll="true" Height="460"
                                                                AutoGenerateColumns="False" ShowFooter="true" OnNeedDataSource="gvGeneratedCalls_NeedDataSource">
                                                                <ClientSettings>
                                                                    <Scrolling UseStaticHeaders="true" />
                                                                    <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                                                                    <ClientEvents OnRowSelecting="RowSelecting" />
                                                                </ClientSettings>
                                                                <MasterTableView DataKeyNames="ID, MobileOperatorFeedbackID, CaseID, ReportRealID,ReportingStatusID" ClientDataKeyNames="ID, MobileOperatorFeedbackID, CaseID, ReportRealID,ReportingStatusID" ShowGroupFooter="true">
                                                                    <Columns>
                                                                        <telerik:GridClientSelectColumn HeaderStyle-Width="30px" ItemStyle-Width="30px" UniqueName="Select"></telerik:GridClientSelectColumn>
                                                                        <telerik:GridBoundColumn DataField="ReportingStatusID" UniqueName="ReportingStatusID" Visible="false" />
                                                                        <telerik:GridBoundColumn HeaderStyle-Width="70px" ItemStyle-Width="70px" DataField="CaseID" UniqueName="CaseID" Aggregate="Count" />
                                                                        <telerik:GridBoundColumn HeaderStyle-Width="70px" ItemStyle-Width="70px" DataField="AttemptDateTime" UniqueName="AttemptDateTime" DataFormatString="{0:dd/MM/yyyy HH:mm:ss}" />

                                                                        <telerik:GridBoundColumn HeaderStyle-Width="70px" ItemStyle-Width="70px" DataField="CLI" UniqueName="CLI" />
                                                                        <telerik:GridBoundColumn HeaderStyle-Width="70px" ItemStyle-Width="70px" DataField="RecommendedActionName" UniqueName="RecommendedActionName" />

                                                                        <telerik:GridTemplateColumn HeaderStyle-Width="70px" ItemStyle-Width="70px" UniqueName="MobileOperatorFeedbackName">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblMobileOperatorFeedbackName" runat="server" Text='<%# Eval("MobileOperatorFeedbackName").ToString() %>'></asp:Label>
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



                                                                    </Columns>
                                                                </MasterTableView>
                                                            </telerik:RadGrid>

                                                        </td>
                                                    </tr>
                                                </table>

                                            </telerik:RadPageView>

                                        </telerik:RadMultiPage>



                                    </td>

                                </tr>

                            </table>

                        </td>

                        <td style="vertical-align: top; width: 20%">

                            <table width="100%" cellpadding="0" cellspacing="0" style="border-style: solid; border-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C; border-top-color: #3D556C">

                                <tr>
                                    <td style="vertical-align: top; width: 100%">
                                        <telerik:RadGrid AllowSorting="true" ID="gvCarrier" runat="server" CellSpacing="0" BorderColor="White"
                                            AutoGenerateColumns="False" OnNeedDataSource="gvCarrier_NeedDataSource" ClientSettings-Scrolling-AllowScroll="true" Height="310">

                                            <ClientSettings>
                                                <Selecting AllowRowSelect="True"></Selecting>
                                                <Scrolling UseStaticHeaders="true" />
                                            </ClientSettings>

                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="Carrier" UniqueName="Carrier" HeaderText="Route ID" />
                                                    <telerik:GridBoundColumn DataField="SourceKind" UniqueName="SourceKind" HeaderText="Type" />
                                                    <telerik:GridNumericColumn DataField="Count" UniqueName="Count" HeaderText="Attempts" DataFormatString="{0:#,##0}" />
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </td>

                                </tr>
                            </table>
                            <br />

                            <table width="100%" cellpadding="0" cellspacing="0" style="border-style: solid; border-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C; border-top-color: #3D556C">


                                <tr>
                                    <td style="vertical-align: top; width: 100%">
                                        <telerik:RadGrid AllowSorting="true" ID="gvOriginationNetwork" runat="server" CellSpacing="0" BorderColor="White"
                                            AutoGenerateColumns="False" OnNeedDataSource="gvOriginationNetwork_NeedDataSource" ClientSettings-Scrolling-AllowScroll="true" Height="210">

                                            <ClientSettings>
                                                <Selecting AllowRowSelect="True"></Selecting>
                                                <Scrolling UseStaticHeaders="true" />
                                            </ClientSettings>

                                            <MasterTableView>
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="OriginationNetwork" UniqueName="OriginationNetwork" HeaderText="Origination Network" />
                                                    <telerik:GridNumericColumn DataField="Count" UniqueName="Count" HeaderText="Attempts" DataFormatString="{0:#,##0}" />
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

