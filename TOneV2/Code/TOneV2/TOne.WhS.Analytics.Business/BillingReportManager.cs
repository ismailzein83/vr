using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Data;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Entities;
using System.Drawing;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.Analytics.Business
{
    public partial class BillingReportManager
    {
        private readonly IBillingReportDataManager _datamanager;
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SaleZoneManager _saleZoneManager;

        public BillingReportManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingReportDataManager>();
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
        }

        private List<BusinessCaseStatus> GetBusinessCaseStatusDurationAmount(BusinessCaseStatusQuery query, bool isSale)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Month" },
                    MeasureFields = new List<string>() { "SaleDuration" },
                    TableId = 8,
                    FromTime = query.fromDate,
                    ToTime = query.toDate,
                    CurrencyId = query.currencyId,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>()
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            List<object> listCustomers = new List<object>
            {
                query.customerId
            };
            listCustomers.Add(query.customerId);
            if (isSale)
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Customer",
                    FilterValues = listCustomers
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
                analyticQuery.Query.MeasureFields.Add("CostNet");
            }
            else
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Supplier",
                    FilterValues = listCustomers
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
                analyticQuery.Query.MeasureFields.Add("SaleNet");
            }

            List<BusinessCaseStatus> listBusinessCaseStatus = new List<BusinessCaseStatus>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;

            if (result != null)
                foreach (var analyticRecord in result.Data)
                {
                    BusinessCaseStatus businessCaseStatus = new BusinessCaseStatus();

                    var monthValue = analyticRecord.DimensionValues[0];
                    if (monthValue != null)
                        businessCaseStatus.MonthYear = monthValue.Name;

                    MeasureValue net;
                    if (isSale)
                        analyticRecord.MeasureValues.TryGetValue("CostNet", out net);
                    else
                        analyticRecord.MeasureValues.TryGetValue("SaleNet", out net);
                    businessCaseStatus.Amount = (net == null) ? 0 : Convert.ToDouble(net.Value ?? 0.0);


                    MeasureValue saleDuration;
                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out saleDuration);
                    businessCaseStatus.Durations = Convert.ToDecimal(saleDuration.Value ?? 0.0);

                    listBusinessCaseStatus.Add(businessCaseStatus);
                }
            return listBusinessCaseStatus;
        }
        private List<BusinessCaseStatus> GetBusinessCaseStatus(BusinessCaseStatusQuery query, bool isSale, bool isAmount)
        {
            List<BusinessCaseStatus> listBusinessCaseStatus = _datamanager.GetBusinessCaseStatus(query.fromDate, query.toDate, query.customerId, query.topDestination, isSale, isAmount, query.currencyId);
            for (int i = 0; i < listBusinessCaseStatus.Count; i++)
            {
                listBusinessCaseStatus[i].Zone = _saleZoneManager.GetSaleZoneName(listBusinessCaseStatus[i].ZoneId);
            }
            List<BusinessCaseStatus> sortedList = listBusinessCaseStatus.OrderByDescending(o => o.Durations).ToList();

            return sortedList;
        }

        public ExcelResult ExportCarrierProfile(BusinessCaseStatusQuery businessCaseStatusQuery)
        {
            // Monthly Traffic Carrier As Customer (Amount,Durations) , Top N Destinations (Amount,Durations)  (3 datatables)
            List<BusinessCaseStatus> listBusinessCaseStatusDurationAmountSale = GetBusinessCaseStatusDurationAmount(businessCaseStatusQuery, true);
            List<BusinessCaseStatus> listBusinessCaseStatusSaleAmount = GetBusinessCaseStatus(businessCaseStatusQuery, true, true);
            List<BusinessCaseStatus> listBusinessCaseStatusSale = GetBusinessCaseStatus(businessCaseStatusQuery, true, false);

            // Monthly Traffic Carrier As Supplier (Amount,Durations) , Top N Destinations (Amount,Durations) (3 datatables)
            List<BusinessCaseStatus> listBusinessCaseStatusDurationAmount = GetBusinessCaseStatusDurationAmount(businessCaseStatusQuery, false);
            List<BusinessCaseStatus> listBusinessCaseStatusAmount = GetBusinessCaseStatus(businessCaseStatusQuery, false, true);
            List<BusinessCaseStatus> listBusinessCaseStatus = GetBusinessCaseStatus(businessCaseStatusQuery, false, false);

            //export to excel
            ////////////////////////////////
            Workbook wbk = new Workbook();
            wbk.Worksheets.RemoveAt("Sheet1");
            Vanrise.Common.Utilities.ActivateAspose();

            Style style = wbk.Styles[wbk.Styles.Add()];
            style.Font.Name = "Times New Roman";
            style.Font.Color = Color.FromArgb(255, 0, 0);
            style.Font.Size = 14;
            style.Font.IsBold = true;
            string currency = String.Format("Currency: [{0}] {1}", businessCaseStatusQuery.currencySymbol, businessCaseStatusQuery.currencyName);
            string chartTitle = "Monthly Traffic " + _carrierAccountManager.GetCarrierAccountName(businessCaseStatusQuery.customerId) + " As Customer " + currency;
            CreateWorkSheetDurationAmount(wbk, "Monthly Traffic as Customer", listBusinessCaseStatusDurationAmountSale, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, chartTitle, style, currency);
            CreateWorkSheet(wbk, "Traf Top Dest Amt  Cus", listBusinessCaseStatusSaleAmount, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Amount Customer", style);
            CreateWorkSheet(wbk, "Traf Top Dest Dur Cus", listBusinessCaseStatusSale, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Duration Customer", style);

            chartTitle = "Monthly Traffic " + _carrierAccountManager.GetCarrierAccountName(businessCaseStatusQuery.customerId) + " As Supplier " + currency;
            CreateWorkSheetDurationAmount(wbk, "Monthly Traffic as Supplier", listBusinessCaseStatusDurationAmount, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, chartTitle, style, currency);
            CreateWorkSheet(wbk, "Traf Top Dest Amt Sup", listBusinessCaseStatusAmount, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Amount Supplier", style);
            CreateWorkSheet(wbk, "Traf Top Dest Dur Sup", listBusinessCaseStatus, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Duration  Supplier", style);

            ExcelResult excelResult = new ExcelResult
            {
                ExcelFileStream = wbk.SaveToStream()
            };
            return excelResult;
        }

        private void CreateWorkSheetDurationAmount(Workbook workbook, string workSheetName, List<BusinessCaseStatus> listBusinessCaseStatus, DateTime fromDate, DateTime? toDate, int topDestination, string chartTitle, Style style, string currency)
        {
            Worksheet worksheet = workbook.Worksheets.Add(workSheetName);
            int lstCarrierProfileCount = listBusinessCaseStatus.Count();

            if (lstCarrierProfileCount > 0)
            {
                string colName = GetExcelColumnName(2 + lstCarrierProfileCount);

                worksheet.Cells.SetColumnWidth(0, 4);

                int HeaderIndex = 2;
                worksheet.Cells[2, 1].PutValue("Amount");
                worksheet.Cells[3, 1].PutValue("Duration");
                Range range = worksheet.Cells.CreateRange("B1:D1");
                Style s = workbook.Styles[workbook.Styles.Add()];
                s.Font.Name = "Times New Roman";
                s.Font.Size = 14;
                s.Font.IsBold = true;
                range.SetStyle(s);
                range.PutValue(currency, false, true);
                //Merge range into a single cell
                range.Merge();
                for (int i = 0; i < lstCarrierProfileCount; i++)
                {
                    worksheet.Cells[1, HeaderIndex].PutValue(listBusinessCaseStatus[i].MonthYear);
                    worksheet.Cells[2, HeaderIndex].PutValue(listBusinessCaseStatus[i].Amount);
                    worksheet.Cells[3, HeaderIndex++].PutValue(listBusinessCaseStatus[i].Durations);
                    worksheet.Cells.SetColumnWidth(i + 1, 20);
                }
                worksheet.Cells.SetColumnWidth(lstCarrierProfileCount + 1, 20);
                if (lstCarrierProfileCount == 1)
                    worksheet.Cells.SetColumnWidth(1, 50);

                worksheet.Cells.CreateRange("C2", colName + "2").SetStyle(style);

                //Adding a chart to the worksheet
                int chartIndex = worksheet.Charts.Add(Aspose.Cells.Charts.ChartType.ColumnStacked, 5, 1, 30, lstCarrierProfileCount + 2);

                //Accessing the instance of the newly added chart
                Aspose.Cells.Charts.Chart chart = worksheet.Charts[chartIndex];

                chart.NSeries.Add("C3:" + colName + "4", false);
                chart.NSeries.CategoryData = "C2:" + colName + "2";
                chart.NSeries[0].Name = "Amount";
                chart.NSeries[1].Name = "Duration";
                chart.ValueAxis.TickLabelPosition = Aspose.Cells.Charts.TickLabelPositionType.Low;
                chart.Legend.Position = Aspose.Cells.Charts.LegendPositionType.Left;
                chart.Legend.Width = 600;
                chart.Legend.Height = 600;
                chart.Title.Font.IsBold = true;
                chart.Title.Text = chartTitle;
                chart.Title.TextHorizontalAlignment = TextAlignmentType.Right;
                chart.Title.X = 2000;
                chart.Title.Y = 100;
            }
        }

        private void CreateWorkSheet(Workbook workbook, string workSheetName, List<BusinessCaseStatus> listBusinessCaseStatus, DateTime fromDate, DateTime? toDate, int topDestination, string chartTitle, Style style)
        {
            Worksheet worksheet = workbook.Worksheets.Add(workSheetName);
            int lstCarrierProfileCount = listBusinessCaseStatus.Count;
            if (lstCarrierProfileCount > 0)
            {
                TimeSpan span = (toDate.HasValue) ? ((DateTime)toDate).Subtract(fromDate) : DateTime.Now.Subtract(fromDate);
                int numberOfMonths = (int)(span.TotalDays / 30);

                int headerIndex = 2;
                int irow = 1;

                string colName = GetExcelColumnName(2 + numberOfMonths);

                worksheet.Cells.SetColumnWidth(0, 4);
                List<string> lstZones = listBusinessCaseStatus.Select(x => x.Zone).Distinct().ToList<string>();
                int listZonesCount = lstZones.Count;
                int maxZoneLenght = 0;
                for (int i = 0; i < listZonesCount; i++)
                {
                    if (lstZones[i].Length > maxZoneLenght)
                        maxZoneLenght = lstZones[i].Length;
                }

                DateTime d = fromDate;

                for (int i = 0; i < numberOfMonths; i++)
                {
                    worksheet.Cells.SetColumnWidth(i + 1, maxZoneLenght + 6);
                    string s = d.ToString("MMMM - yyyy");
                    d = d.AddMonths(1);
                    worksheet.Cells[irow, headerIndex++].PutValue(s);
                }

                worksheet.Cells.SetColumnWidth(numberOfMonths, maxZoneLenght + 6);
                worksheet.Cells.SetColumnWidth(numberOfMonths + 1, maxZoneLenght + 6);
                if (numberOfMonths == 1)
                    worksheet.Cells.SetColumnWidth(1, 50);

                for (int k = 0; k < listZonesCount; k++)
                {
                    irow++;
                    int valueIndex = 1;

                    worksheet.Cells[irow, valueIndex++].PutValue(lstZones[k]);
                    DateTime fDate = fromDate;
                    for (int i = 0; i < numberOfMonths; i++)
                    {
                        bool f = false;
                        for (int j = 0; j < lstCarrierProfileCount; j++)
                        {
                            if (listBusinessCaseStatus[j].Month == fDate.Month && listBusinessCaseStatus[j].Year == fDate.Year && lstZones[k] == listBusinessCaseStatus[j].Zone)
                            {
                                worksheet.Cells[irow, valueIndex++].PutValue(listBusinessCaseStatus[j].Durations);
                                f = true;
                            }
                        }
                        if (f == false)
                            worksheet.Cells[irow, valueIndex++].PutValue("0");
                        fDate = fDate.AddMonths(1);
                    }
                }

                worksheet.Cells.CreateRange("C2", colName + "2").SetStyle(style);

                int chartIndex = worksheet.Charts.Add(Aspose.Cells.Charts.ChartType.Column, topDestination + 3, 1, (int)(topDestination * 2.5), numberOfMonths + 2);
                Aspose.Cells.Charts.Chart chart = worksheet.Charts[chartIndex];

                int seriesCount = 0;
                seriesCount = (listZonesCount < topDestination) ? listZonesCount : topDestination;

                chart.NSeries.Add("C3:" + colName + (seriesCount + 2), false);
                chart.NSeries.CategoryData = "C2:" + colName + "2";
                for (int i = 0; i < listZonesCount; i++)
                {
                    chart.NSeries[i].Name = lstZones[i];
                }
                chart.ValueAxis.TickLabelPosition = Aspose.Cells.Charts.TickLabelPositionType.Low;
                chart.Legend.Position = Aspose.Cells.Charts.LegendPositionType.Left;
                chart.Title.Font.IsBold = true;
                chart.Title.Text = chartTitle;
                chart.Title.TextHorizontalAlignment = TextAlignmentType.Right;
                chart.Title.X = 2000;
                chart.Title.Y = 100;
                if ((35 * listZonesCount) > 200)
                    chart.ChartObject.Height = (35*listZonesCount);
                else
                    chart.ChartObject.Height = 200;
            }
        }
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }
       
    }
}
