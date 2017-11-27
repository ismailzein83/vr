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
using System.IO;
using TOne.WhS.Analytics.Business.BillingReports;
using Vanrise.Common.Business;

namespace TOne.WhS.Analytics.Business
{
    public partial class BillingReportManager
    {
        private readonly IBillingReportDataManager _datamanager;
        private readonly CarrierAccountManager _carrierAccountManager;
        private readonly SaleZoneManager _saleZoneManager;
        private readonly SupplierZoneManager _supplierZoneManager;

        public BillingReportManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingReportDataManager>();
            _carrierAccountManager = new CarrierAccountManager();
            _saleZoneManager = new SaleZoneManager();
            _supplierZoneManager = new SupplierZoneManager();
        }

        private List<BusinessCaseStatus> GetBusinessCaseStatusDurationAmount(BusinessCaseStatusQuery query, bool isSale)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = new List<string> { "Month" },
                    MeasureFields = new List<string>() {  },
                    TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
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
                analyticQuery.Query.MeasureFields.Add("TotalSaleNet");
                analyticQuery.Query.MeasureFields.Add("SaleDuration");
            }
            else
            {
                DimensionFilter dimensionFilter = new DimensionFilter()
                {
                    Dimension = "Supplier",
                    FilterValues = listCustomers
                };
                analyticQuery.Query.Filters.Add(dimensionFilter);
                analyticQuery.Query.MeasureFields.Add("TotalCostNet");
                analyticQuery.Query.MeasureFields.Add("CostDuration");
            }

            var dates = monthsBetween(query.fromDate, query.toDate.Value);
            List<string> arrayOfDate = new List<string>();
            foreach (DateTime dateTime in dates.ToList())
            {
                arrayOfDate.Add(dateTime.ToString("MMMM - yyyy"));
            }
            List<BusinessCaseStatus> listBusinessCaseStatus = new List<BusinessCaseStatus>();

            var result = analyticManager.GetFilteredRecords(analyticQuery) as AnalyticSummaryBigResult<AnalyticRecord>;
            int monthCount = 0;
            for (int i = 0; i < arrayOfDate.Count(); i++)
            {
                BusinessCaseStatus busCaseStatus = new BusinessCaseStatus();
                busCaseStatus.MonthYear = arrayOfDate[i];
                busCaseStatus.Amount = 0;
                busCaseStatus.Durations = 0;
                listBusinessCaseStatus.Add(busCaseStatus);
            }

            if (result != null)
            {
                foreach (var analyticRecord in result.Data)
                {
                    var monthValue = analyticRecord.DimensionValues[0];
                    if (monthValue != null)
                    {
                        foreach (var caseStatus in listBusinessCaseStatus)
                        {
                            if (caseStatus.MonthYear == monthValue.Name)
                            {
                                caseStatus.MonthYear = monthValue.Name;
                                MeasureValue net;
                                if (isSale)
                                    analyticRecord.MeasureValues.TryGetValue("TotalSaleNet", out net);
                                else
                                    analyticRecord.MeasureValues.TryGetValue("TotalCostNet", out net);
                                caseStatus.Amount = Math.Round((net == null) ? 0 : Convert.ToDouble(net.Value ?? 0.0), ReportHelpers.GetLongNumberPrecision());

                                MeasureValue duration;
                                if (isSale)
                                    analyticRecord.MeasureValues.TryGetValue("SaleDuration", out duration);
                                else
                                    analyticRecord.MeasureValues.TryGetValue("CostDuration", out duration);
                                caseStatus.Durations = Math.Round(Convert.ToDecimal(duration.Value ?? 0.0), ReportHelpers.GetNormalNumberPrecision()); 
                            }
                        }
                    }
                }
            }
                
            return listBusinessCaseStatus;
        }
        private List<BusinessCaseStatus> GetBusinessCaseStatus(BusinessCaseStatusQuery query, bool isSale, bool isAmount)
        {
            List<BusinessCaseStatus> listBusinessCaseStatus = _datamanager.GetBusinessCaseStatus(query.fromDate, query.toDate, query.customerId, query.topDestination, isSale, isAmount, query.currencyId);
            for (int i = 0; i < listBusinessCaseStatus.Count; i++)
            {
                if (isSale)
                    listBusinessCaseStatus[i].Zone = _saleZoneManager.GetSaleZoneName(listBusinessCaseStatus[i].ZoneId);
                else
                    listBusinessCaseStatus[i].Zone = _supplierZoneManager.GetSupplierZoneName(listBusinessCaseStatus[i].ZoneId);
            }
            return listBusinessCaseStatus;
        }

        static IEnumerable<DateTime> monthsBetween(DateTime d0, DateTime d1)
        {
            return Enumerable.Range(0, (d1.Year - d0.Year) * 12 + (d1.Month - d0.Month + 1))
                             .Select(m => new DateTime(d0.Year, d0.Month, 1).AddMonths(m));
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
            string carrierName = _carrierAccountManager.GetCarrierAccountName(businessCaseStatusQuery.customerId);
            string currency = String.Format("Currency: [{0}] {1}", businessCaseStatusQuery.currencySymbol, businessCaseStatusQuery.currencyName);

            string duration = String.Format("Duration: [{0}] {1}", "Min", "Minutes");

            string chartTitle = "Monthly Traffic " + _carrierAccountManager.GetCarrierAccountName(businessCaseStatusQuery.customerId) + " As Customer ";
            CreateWorkSheetDurationAmount(wbk, "Monthly Traffic as Customer", listBusinessCaseStatusDurationAmountSale, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, chartTitle, style, currency, carrierName);
            CreateWorkSheet(wbk, "Traf Top Dest Amt  Cus", listBusinessCaseStatusSaleAmount, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Amount For Customer", style, currency, carrierName);
            CreateWorkSheet(wbk, "Traf Top Dest Dur Cus", listBusinessCaseStatusSale, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Duration For Customer", style, duration, carrierName);

            chartTitle = "Monthly Traffic " + _carrierAccountManager.GetCarrierAccountName(businessCaseStatusQuery.customerId) + " As Supplier ";
            CreateWorkSheetDurationAmount(wbk, "Monthly Traffic as Supplier", listBusinessCaseStatusDurationAmount, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, chartTitle, style, currency, carrierName);
            CreateWorkSheet(wbk, "Traf Top Dest Amt Sup", listBusinessCaseStatusAmount, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Amount For Supplier", style, currency, carrierName);
            CreateWorkSheet(wbk, "Traf Top Dest Dur Sup", listBusinessCaseStatus, businessCaseStatusQuery.fromDate, businessCaseStatusQuery.toDate, businessCaseStatusQuery.topDestination, "Traffic Top Destination Duration For Supplier", style, duration, carrierName);
            
            byte[] array;
            using (MemoryStream ms = new MemoryStream())
            {     
                wbk.Save(ms, SaveFormat.Xlsx);
                array = ms.ToArray();
            }
            ExcelResult excelResult = new ExcelResult
            {
                ExcelFileContent = array
            };
            return excelResult;
        }

        private void CreateWorkSheetDurationAmount(Workbook workbook, string workSheetName, List<BusinessCaseStatus> listBusinessCaseStatus, DateTime fromDate, DateTime? toDate, int topDestination, string chartTitle, Style style, string currency, string carrierName)
        {
            Worksheet worksheet = workbook.Worksheets.Add(workSheetName);
            int lstCarrierProfileCount = listBusinessCaseStatus.Count();
            double total = listBusinessCaseStatus.Sum(item => item.Amount.Value);
            decimal totalDuration = listBusinessCaseStatus.Sum(item => item.Durations.Value);
            int decimalPrecision = GenericParameterManager.Current.GetNormalPrecision();

            worksheet.Cells.SetColumnWidth(4, 20);
            worksheet.Cells.SetColumnWidth(5, 20);
            worksheet.Cells.SetColumnWidth(6, 20);
            worksheet.Cells.SetColumnWidth(7, 20);

            if (lstCarrierProfileCount > 0 && totalDuration >0 && totalDuration > 0)
            {
                string colName = GetExcelColumnName(2 + lstCarrierProfileCount);

                worksheet.Cells.SetColumnWidth(0, 4);

                int HeaderIndex = 2;
                worksheet.Cells[2, 1].PutValue("Amount");

                worksheet.Cells[3, 1].PutValue("Duration (Min)");
                Range range = worksheet.Cells.CreateRange("B1:D1");
                Style s = workbook.Styles[workbook.Styles.Add()];
                s.Font.Name = "Times New Roman";
                s.Font.Size = 14;
                s.Font.IsBold = true;
                range.SetStyle(s);
                range.PutValue(currency, false, true);


                Style value = workbook.Styles[workbook.Styles.Add()];
                value.Font.Name = "Times New Roman";
                value.Font.Size = 12;



                Style label = workbook.Styles[workbook.Styles.Add()];
                label.Font.Name = "Times New Roman";
                label.Font.Size = 12;
                label.Font.IsBold = true;

                worksheet.Cells[0, 4].SetStyle(label);
                worksheet.Cells[0, 4].PutValue("From Date");

                worksheet.Cells[0, 5].SetStyle(value);
                worksheet.Cells[0, 5].PutValue(fromDate.ToString("dd-MM-yyyy"));


                worksheet.Cells[0, 6].SetStyle(label);
                worksheet.Cells[0, 6].PutValue("To Date");

                worksheet.Cells[0, 7].SetStyle(value);
                worksheet.Cells[0, 7].PutValue(toDate.Value.ToString("dd-MM-yyyy"));


                //Merge range into a single cell
                range.Merge();
                for (int i = 0; i < lstCarrierProfileCount; i++)
                {
                    worksheet.Cells[1, HeaderIndex].PutValue(listBusinessCaseStatus[i].MonthYear);

                    Style cellstyle = new Style();
                    cellstyle.Custom = "0." + "".PadLeft(decimalPrecision, '0');

                    var cellAmmount = worksheet.Cells[2, HeaderIndex];
                    cellAmmount.PutValue(listBusinessCaseStatus[i].Amount);
                    cellAmmount.SetStyle(cellstyle);

                    var cellDuration = worksheet.Cells[3, HeaderIndex++];
                    cellDuration.PutValue(listBusinessCaseStatus[i].Durations);
                    cellDuration.SetStyle(cellstyle);

                    worksheet.Cells.SetColumnWidth(i + 1, 23);
                }
                worksheet.Cells.SetColumnWidth(lstCarrierProfileCount + 1, 23);
                if (lstCarrierProfileCount == 1)
                    worksheet.Cells.SetColumnWidth(1, 50);

                worksheet.Cells.CreateRange("C2", colName + "2").SetStyle(style);

                //Adding a chart to the worksheet
                int chartIndex = worksheet.Charts.Add(Aspose.Cells.Charts.ChartType.Column, 5, 1, 30, lstCarrierProfileCount + 2);

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
                chart.Title.Y = 50;
            }
        }

        private void CreateWorkSheet(Workbook workbook, string workSheetName, List<BusinessCaseStatus> listBusinessCaseStatus, DateTime fromDate, DateTime? toDate, int topDestination, string chartTitle, Style style, string currency, string carrierName)
        {
            Worksheet worksheet = workbook.Worksheets.Add(workSheetName);
            int lstCarrierProfileCount = listBusinessCaseStatus.Count;
            int decimalPrecision = GenericParameterManager.Current.GetNormalPrecision();
            Style cellstyle = new Style();
            cellstyle.Custom = "0." + "".PadLeft(decimalPrecision, '0');
            if (lstCarrierProfileCount > 0)
            {
                Range range = worksheet.Cells.CreateRange("B1:D1");
                Style styleCurrency = workbook.Styles[workbook.Styles.Add()];
                styleCurrency.Font.Name = "Times New Roman";
                styleCurrency.Font.Size = 14;
                styleCurrency.Font.IsBold = true;
                range.SetStyle(styleCurrency);
                range.PutValue(currency, false, true);
                //Merge range into a single cell
                range.Merge();

                Style value = workbook.Styles[workbook.Styles.Add()];
                value.Font.Name = "Times New Roman";
                value.Font.Size = 12;



                Style label = workbook.Styles[workbook.Styles.Add()];
                label.Font.Name = "Times New Roman";
                label.Font.Size = 12;
                label.Font.IsBold = true;
                worksheet.Cells.SetColumnWidth(4, 20);
                worksheet.Cells.SetColumnWidth(5, 20);
                worksheet.Cells.SetColumnWidth(6, 20);
                worksheet.Cells.SetColumnWidth(7, 20);

                worksheet.Cells[0, 4].SetStyle(label);
                worksheet.Cells[0, 4].PutValue("From Date");

                worksheet.Cells[0, 5].SetStyle(value);
                worksheet.Cells[0, 5].PutValue(fromDate.ToString("dd-MM-yyyy"));


                worksheet.Cells[0, 6].SetStyle(label);
                worksheet.Cells[0, 6].PutValue("To Date");

                worksheet.Cells[0, 7].SetStyle(value);
                worksheet.Cells[0, 7].PutValue(toDate.Value.ToString("dd-MM-yyyy"));

                TimeSpan span = (toDate.HasValue) ? ((DateTime)toDate).Subtract(fromDate) : DateTime.Now.Subtract(fromDate);
                int numberOfMonths = (int)(Math.Round(span.TotalDays / 30));
                if (toDate.HasValue && (toDate.Value.Month - fromDate.Month) == numberOfMonths )
                    numberOfMonths++;

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
                    string date = d.ToString("MMMM - yyyy");
                    d = d.AddMonths(1);
                    worksheet.Cells[irow, headerIndex++].PutValue(date);
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
                                var cell = worksheet.Cells[irow, valueIndex++];
                                cell.PutValue(listBusinessCaseStatus[j].Durations);
                                cell.SetStyle(cellstyle);
                                f = true;
                            }
                        }
                        if (f == false)
                        {
                           
                            var cell = worksheet.Cells[irow, valueIndex++];
                            cell.PutValue(0);
                            cell.SetStyle(cellstyle);
                        }
                        fDate = fDate.AddMonths(1);
                    }
                }

                worksheet.Cells.CreateRange("C2", colName + "2").SetStyle(style);
                int seriesCount = 0;
                seriesCount = (listZonesCount < topDestination) ? listZonesCount : topDestination;

                int chartIndex = worksheet.Charts.Add(Aspose.Cells.Charts.ChartType.Column, seriesCount + 3, 1, (int)(seriesCount * 2.5), numberOfMonths + 2);
                Aspose.Cells.Charts.Chart chart = worksheet.Charts[chartIndex];



                chart.NSeries.Add("C3:" + colName + (seriesCount + 2), false);
                chart.NSeries.CategoryData = "C2:" + colName + "2";
                for (int i = 0; i < listZonesCount; i++)
                {
                    chart.NSeries[i].Name = lstZones[i];
                }
                chart.ValueAxis.TickLabelPosition = Aspose.Cells.Charts.TickLabelPositionType.Low;
                chart.Legend.Position = Aspose.Cells.Charts.LegendPositionType.Left;
                chart.Title.Font.IsBold = true;
                chart.Title.Text = chartTitle + " " +carrierName;
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
