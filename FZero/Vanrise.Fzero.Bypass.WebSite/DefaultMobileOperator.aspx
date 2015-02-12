<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="DefaultMobileOperator.aspx.cs" Inherits="DefaultMobileOperator" %>


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

    <table width="98%">

        <tr>
            <td style="vertical-align: top; width: 15%">
                <table width="100%" cellpadding="2" cellspacing="2" style="border-style: solid; border-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C; border-top-color: #3D556C">

                    <tr>
                        <td style="vertical-align: top; width: 100%">


                            <div class="row-fluid" id="div1" runat="server">
                                <div class="span12">
                                    <div class="widget blue">
                                        <div class="widget-title">
                                            <h4><i class="icon-reorder"></i>Summary</h4>

                                        </div>
                                        <div class="widget-body" style="display: block;">

                                            <telerik:RadComboBox ID="ddlMobileOperators" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlMobileOperators_SelectedIndexChanged"></telerik:RadComboBox>


                                            <telerik:RadGrid AllowSorting="true" ID="gvCases" runat="server" CellSpacing="0" ShowHeader="false" BorderColor="White"
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
                                                                <asp:Label ID="lblcount" Font-Bold="true" Font-Size="10" runat="server" Text='<%#  int.Parse(Eval("count").ToString()).ToString(Vanrise.CommonLibrary.Formatter.AmountFormat) %>'></asp:Label>
                                                            </ItemTemplate>
                                                        </telerik:GridTemplateColumn>


                                                    </Columns>
                                                </MasterTableView>
                                            </telerik:RadGrid>
                                        </div>
                                    </div>
                                </div>
                            </div>


                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: top; width: 100%">
                            <div class="row-fluid" id="div3" runat="server">
                                <div class="span12">
                                    <div class="widget blue">
                                        <div class="widget-title">
                                            <h4><i class="icon-reorder"></i>From - To </h4>

                                        </div>
                                        <div class="widget-body" style="display: block;">

                                            <table style="width: 100%">
                                                <tr>


                                                    <td>
                                                        <telerik:RadDateTimePicker ID="rdpFrom" runat="server" AutoPostBack="True" OnSelectedDateChanged="rdpFrom_SelectedDateChanged" AutoPostBackControl="Both">
                                                        </telerik:RadDateTimePicker>
                                                    </td>
                                                    <td>
                                                        <asp:ImageButton ID="btnRefershFrom" Text="" runat="server" ImageUrl="Icons/refresh-16x16.gif" OnClick="btnRefershFrom_Click"></asp:ImageButton>
                                                    </td>
                                                </tr>

                                                <tr>


                                                    <td>
                                                        <telerik:RadDateTimePicker ID="rdpTo" runat="server" AutoPostBack="True" OnSelectedDateChanged="rdpTo_SelectedDateChanged" AutoPostBackControl="Both">
                                                        </telerik:RadDateTimePicker>
                                                    </td>
                                                    <td>
                                                        <asp:ImageButton ID="btnRefershTo" Text="" runat="server" ImageUrl="Icons/refresh-16x16.gif" OnClick="btnRefershTo_Click"></asp:ImageButton>
                                                    </td>
                                                </tr>


                                            </table>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td style="vertical-align: top; width: 100%">
                            <div class="row-fluid" id="div5" runat="server">
                                <div class="span12">
                                    <div class="widget blue">
                                        <div class="widget-title">
                                            <h4><i class="icon-reorder"></i>Date Range </h4>

                                        </div>
                                        <div class="widget-body" style="display: block;">

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

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>

                    </tr>

                </table>
            </td>

            <td style="vertical-align: top; width: 60%">

                <table width="100%" cellpadding="2" cellspacing="2" style="border-right-style: solid; border-bottom-style: solid; border-left-style: solid; border-right-width: 1px; border-bottom-width: 1px; border-left-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C">

                    <tr>
                        <td style="vertical-align: top; width: 100%">
                              <telerik:RadTabStrip ID="rtsMain" runat="server" ShowBaseLine="true" AutoPostBack="True" SelectedIndex="0" MultiPageID="RadMultiPageMain" OnTabClick="rtsMain_TabClick" ScrollButtonsPosition="Right"  ScrollChildren  ="true"      >
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



                    </Tabs>
                </telerik:RadTabStrip>
                            </td></tr>

                    <tr>
                        <td style="vertical-align: top; width: 100%">


                            <telerik:RadMultiPage ID="RadMultiPageMain" Width="100%"  runat="server" SelectedIndex="0">

                                <telerik:RadPageView ID="RadPageViewCarrier"  Width="100%" runat="server">
                                    <telerik:RadChart ID="rcCarrier" runat="server" DefaultType="Pie" Skin="BlueStripes" Width="650px" Height="550px">
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
                                    <telerik:RadChart ID="rcOrigination" runat="server" DefaultType="Pie" Skin="BlueStripes" Width="650px" Height="550px">
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

                                    <telerik:RadChart ID="rcActiveTime" runat="server" DefaultType="Pie" Skin="BlueStripes" Width="800px" Height="650px" SeriesOrientation="Vertical">
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

                                    <telerik:RadGrid ID="gvOnnetFraudGeneratedCalls" MasterTableView-Caption="Fraud Cases" ShowGroupPanel="true" EnableHeaderContextMenu="true" EnableHeaderContextFilterMenu="false" GroupingEnabled="true" ClientSettings-AllowColumnHide="false" AllowSorting="true" runat="server" CellSpacing="0" PageSize="15"
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


                            </telerik:RadMultiPage>



                        </td>

                    </tr>

                </table>

            </td>

            <td style="vertical-align: top; width: 25%">
                <div class="row-fluid" id="div2" runat="server">
                    <div class="span12">
                        <div class="widget blue">
                            <div class="widget-title">
                                <h4><i class="icon-reorder"></i>Date Range </h4>

                            </div>
                            <div class="widget-body" style="display: block;">
                                <table width="100%" cellpadding="0" cellspacing="0" style="border-style: solid; border-width: 1px; border-right-color: #3D556C; border-bottom-color: #3D556C; border-left-color: #3D556C; border-top-color: #3D556C">

                                    <tr>
                                        <td style="vertical-align: top; width: 100%">
                                            <telerik:RadGrid AllowSorting="true" ID="gvCarrier" runat="server" CellSpacing="0" BorderColor="White"
                                                AutoGenerateColumns="False" OnNeedDataSource="gvCarrier_NeedDataSource" ClientSettings-Scrolling-AllowScroll="true" Height="230">

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
                                                AutoGenerateColumns="False" OnNeedDataSource="gvOriginationNetwork_NeedDataSource" ClientSettings-Scrolling-AllowScroll="true" Height="150">

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


                                <br />

                            </div>
                        </div>
                    </div>
                </div>
            </td>

        </tr>
    </table>





</asp:Content>


