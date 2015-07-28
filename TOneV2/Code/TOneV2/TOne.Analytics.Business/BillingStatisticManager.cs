using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using System.Drawing;

namespace TOne.Analytics.Business
{
    public partial class BillingStatisticManager
    {
        private readonly IBillingStatisticDataManager _datamanager;
        private readonly BusinessEntityInfoManager _bemanager;
        private readonly CarrierAccountManager _cmanager;

        public BillingStatisticManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            _bemanager = new BusinessEntityInfoManager();
            _cmanager = new CarrierAccountManager();
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate, bool groupByCustomer)
        {

            return GetZoneProfit(fromDate, toDate, null, null, groupByCustomer, null, null);
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, int? supplierAMUId, int? customerAMUId)
        {

            return FormatZoneProfits(_datamanager.GetZoneProfit(fromDate, toDate, customerId, supplierId, groupByCustomer, supplierAMUId, customerAMUId));
        }

        public List<DailySummaryFormatted> GetDailySummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatDailySummaries(_datamanager.GetDailySummary(fromDate, toDate, customerAMUId, supplierAMUId));
        }

        public HttpResponseMessage ExportSupplierCostDetails(DateTime fromDate, DateTime toDate, string customerId, int topDestination)
        {
            //List<SupplierCostDetails> lstSuppplierCostDetails = _datamanager.GetSupplierCostDetails(fromDate, toDate, customerAMUId, supplierAMUId);
            List<CarrierProfileReport> lstSuppplierCostDetails = GetCarrierProfileMTDAndMTA(fromDate, toDate, customerId, true);
            List<CarrierProfileReport> lstSuppplierCostDetails2 = GetCarrierProfileMTDAndMTA(fromDate, toDate, customerId, false);

            List<CarrierProfileReport> lstSuppplierCostDetails3 = GetCarrierProfile(fromDate, toDate, customerId, topDestination,true, false);
            int DaysInTillDays = DateTime.DaysInMonth(toDate.Year, toDate.Month);

            TimeSpan span = toDate.Subtract(fromDate);

            int NumberOfMonths = (int)(span.TotalDays / 30);

       
            //export to excel
            ////////////////////////////////

            Workbook wbk = new Workbook();
            Worksheet RateWorkSheet = wbk.Worksheets.Add("Monthly Traffic as Customer");
            Worksheet RateWorkSheet2 = wbk.Worksheets.Add("Monthly Traffic as Supplier");
            Worksheet RateWorkSheet3 = wbk.Worksheets.Add("Traff Top Dest. Durat");
            RateWorkSheet.Cells.SetColumnWidth(0, 4);
            RateWorkSheet.Cells.SetColumnWidth(1, 15);
            RateWorkSheet.Cells.SetColumnWidth(2, 15);
            RateWorkSheet.Cells.SetColumnWidth(3, 15);

            RateWorkSheet2.Cells.SetColumnWidth(0, 4);
            RateWorkSheet2.Cells.SetColumnWidth(1, 15);
            RateWorkSheet2.Cells.SetColumnWidth(2, 15);
            RateWorkSheet2.Cells.SetColumnWidth(3, 15);

            RateWorkSheet3.Cells.SetColumnWidth(0, 4);
            for (int i = 0; i < NumberOfMonths + 1; i++)
            {
                RateWorkSheet3.Cells.SetColumnWidth(i + 1, 20);
            }


            int HeaderIndex = 1;
            int Irow = 1;
            RateWorkSheet.Cells[Irow, HeaderIndex++].PutValue("MonthYear");
            RateWorkSheet.Cells[Irow, HeaderIndex++].PutValue("Amount");
            RateWorkSheet.Cells[Irow, HeaderIndex++].PutValue("Duration");

            foreach (CarrierProfileReport supplier in lstSuppplierCostDetails)
            {
                Irow++;
                int valueIndex = 1;

                RateWorkSheet.Cells[Irow, valueIndex++].PutValue(supplier.MonthYear);
                RateWorkSheet.Cells[Irow, valueIndex++].PutValue(supplier.Amount);
                RateWorkSheet.Cells[Irow, valueIndex++].PutValue(supplier.Durations);
            }

            HeaderIndex = 1;
            Irow = 1;
            RateWorkSheet2.Cells[Irow, HeaderIndex++].PutValue("MonthYear");
            RateWorkSheet2.Cells[Irow, HeaderIndex++].PutValue("Amount");
            RateWorkSheet2.Cells[Irow, HeaderIndex++].PutValue("Duration");

            foreach (CarrierProfileReport supplier in lstSuppplierCostDetails2)
            {
                Irow++;
                int valueIndex = 1;

                RateWorkSheet2.Cells[Irow, valueIndex++].PutValue(supplier.MonthYear);
                RateWorkSheet2.Cells[Irow, valueIndex++].PutValue(supplier.Amount);
                RateWorkSheet2.Cells[Irow, valueIndex++].PutValue(supplier.Durations);
            }



            //////////////////////////
            HeaderIndex = 1;
            Irow = 1;
            RateWorkSheet3.Cells[Irow, HeaderIndex++].PutValue("Name");
            DateTime d = fromDate;
            for (int i = 0; i < NumberOfMonths; i++)
            {
                string s = d.ToString("MMMM - yyyy");
                d = d.AddMonths(1);
                RateWorkSheet3.Cells[Irow, HeaderIndex++].PutValue(s);
            }
            List<string> lstZones = lstSuppplierCostDetails3.Select(x => x.Zone).Distinct().ToList<string>();

            for (int ii = 0; ii < lstZones.Count(); ii++)
            {
                Irow++;
                HeaderIndex = 1;
                int valueIndex = 1;
                RateWorkSheet3.Cells[Irow, valueIndex++].PutValue(lstZones[ii]);
                DateTime d2 = fromDate;
                for (int i = 0; i < NumberOfMonths; i++)
                {
                    bool f = false;
                    for (int j = 0; j < lstSuppplierCostDetails3.Count; j++)
                    {
                        if (lstSuppplierCostDetails3[j].Month == d2.Month && lstSuppplierCostDetails3[j].Year == d2.Year && lstZones[ii] == lstSuppplierCostDetails3[j].Zone)
                        {
                            RateWorkSheet3.Cells[Irow, valueIndex++].PutValue(lstSuppplierCostDetails3[j].Durations);
                            f = true;
                        }
                    }
                    if(f== false)
                        RateWorkSheet3.Cells[Irow, valueIndex++].PutValue("0");
                    d2 = d2.AddMonths(1);
                }
            }


            //////////////////////////////////

            //Styles
            for (int i = 1; i <= 3; i++)
            {
                Cell cell = RateWorkSheet.Cells.GetCell(1, i);
                Cell cell2 = RateWorkSheet2.Cells.GetCell(1, i);
                
                Style style = cell.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell.SetStyle(style);
                cell2.SetStyle(style);
            }
            for (int i = 1; i <= NumberOfMonths +1; i++)
            {
                Cell cell3 = RateWorkSheet3.Cells.GetCell(1, i);
                Style style = cell3.GetStyle();
                style.Font.Name = "Times New Roman";
                style.Font.Color = Color.FromArgb(255, 0, 0); ;
                style.Font.Size = 14;
                style.Font.IsBold = true;
                cell3.SetStyle(style);
            }

            //Adding a chart to the worksheet
            int chartIndex = RateWorkSheet.Charts.Add(Aspose.Cells.Charts.ChartType.ColumnStacked, 6, 5, 30, 20);
            int chartIndex2 = RateWorkSheet2.Charts.Add(Aspose.Cells.Charts.ChartType.ColumnStacked, 6, 5, 30, 20);

            //Accessing the instance of the newly added chart
            Aspose.Cells.Charts.Chart chart = RateWorkSheet.Charts[chartIndex];
            Aspose.Cells.Charts.Chart chart2 = RateWorkSheet2.Charts[chartIndex2];

            //Adding SeriesCollection (chart data source) to the chart ranging from "A1" cell to "B3"
            chart.NSeries.Add("C3:D7", true);
            chart2.NSeries.Add("C3:D7", true);

            wbk.Save("D:\\book1.xls");

            byte[] array;
            MemoryStream ms = new MemoryStream();
            ms = wbk.SaveToStream();
            array = ms.ToArray();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            ms.Position = 0;
            result.Content = new StreamContent(ms);

            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Data.xls"
            };

            return result;
        }
        public List<SupplierCostDetailsFormatted> GetSupplierCostDetails(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            List<SupplierCostDetails> lstSuppplierCostDetails = _datamanager.GetSupplierCostDetails(fromDate, toDate, customerAMUId, supplierAMUId);

            List<SupplierCostDetails> lstSuppplierCostDetailsGrouped = AddGroupNames(lstSuppplierCostDetails);

            var grouped = lstSuppplierCostDetailsGrouped.GroupBy(c => new { supplier = c.Carrier, subKey = c.CustomerGroupName })
              .Select(r => new SupplierCostDetails
              {
                  Carrier = r.First().Carrier,
                  CustomerGroupName = r.Key.subKey,
                  Duration = r.Sum(rr => (decimal)(string.IsNullOrEmpty(rr.Duration.ToString()) ? 0 : rr.Duration)),
                  Amount = r.Sum(rr => (double)(string.IsNullOrEmpty(rr.Amount.ToString()) ? 0 : rr.Amount))
              });


            List<SupplierCostDetails> selectedCollection = grouped.ToList();
            List<SupplierCostDetailsFormatted> lstSupplierCostDetailsFormatted = FormatSupplierCostDetails(selectedCollection);
            return lstSupplierCostDetailsFormatted;
        }

        public List<SaleZoneCostSummaryFormatted> GetSaleZoneCostSummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatSaleZoneCostSummary(_datamanager.GetSaleZoneCostSummary(fromDate, toDate, supplierAMUId, customerAMUId));
        }
        public List<SaleZoneCostSummaryServiceFormatted> GetSaleZoneCostSummaryService(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatSaleZoneCostSummaryService(_datamanager.GetSaleZoneCostSummaryService(fromDate, toDate, supplierAMUId, customerAMUId));
        }
        public List<SaleZoneCostSummarySupplierFormatted> GetSaleZoneCostSummarySupplier(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatSaleZoneCostSummarySupplier(_datamanager.GetSaleZoneCostSummarySupplier(fromDate, toDate, supplierAMUId, customerAMUId));
        }

        public List<ZoneSummaryFormatted> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier, out double services)
        {
            return FormatZoneSummaries(_datamanager.GetZoneSummary(fromDate, toDate, customerId, supplierId, isCost, currencyId, supplierGroup, customerGroup, customerAMUId, supplierAMUId, groupBySupplier, out services));
        }
        public List<ZoneSummaryDetailedFormatted> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier, out double services)
        {
            return FormatZoneSummariesDetailed(_datamanager.GetZoneSummaryDetailed(fromDate, toDate, customerId, supplierId, isCost, currencyId, supplierGroup, customerGroup, customerAMUId, supplierAMUId, groupBySupplier, out services));
        }

        public List<CarrierLostFormatted> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, int? supplierAMUId, int? customerAMUId)
        {
            return FormatCarrierLost(_datamanager.GetCarrierLost(fromDate, toDate, customerId, supplierId, margin, supplierAMUId, customerAMUId));
        }
        public List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {
            return _datamanager.GetMonthTraffic(fromDate, toDate, carrierAccountID, isSale);
        }

        public List<CarrierProfileReport> GetCarrierProfileMTDAndMTA(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {
            return _datamanager.GetCarrierProfileMTDAndMTA(fromDate, toDate, carrierAccountID, isSale);
        }

        public List<CarrierProfileReport> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int topDestination, bool isSale, bool isAmount)
        {
            return _datamanager.GetCarrierProfile(fromDate, toDate, carrierAccountID, topDestination, isSale, isAmount);
        }

        public List<CarrierSummaryDailyFormatted> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, int? customerAMUId, int? supplierAMUId)
        {
            return FormatCarrieresSummaryDaily(_datamanager.GetDailyCarrierSummary(fromDate, toDate, customerId, supplierId, isCost, isGroupedByDay, supplierAMUId, customerAMUId), isGroupedByDay);
        }

        public List<RateLossFormatted> GetRateLoss(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? zoneId, int? customerAMUId, int? supplierAMUId)
        {
            return FormatRateLosses(_datamanager.GetRateLoss(fromDate, toDate, customerId, supplierId, zoneId, supplierAMUId, customerAMUId));
        }

        public List<CarrierSummaryFormatted> GetCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {
            List<CarrierSummaryFormatted> lstCarrierSummaryFormatted = FormatCarrierSummaries(_datamanager.GetCarrierSummary(fromDate, toDate, customerId, supplierId, supplierAMUId, customerAMUId));
            lstCarrierSummaryFormatted = lstCarrierSummaryFormatted.OrderBy(x => x.Customer).ToList<CarrierSummaryFormatted>();
            return lstCarrierSummaryFormatted;
        }

        public List<DetailedCarrierSummaryFormatted> GetDetailedCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {
            List<DetailedCarrierSummary> lstDetailedCarrierSummary = _datamanager.GetCarrierDetailedSummary(fromDate, toDate, customerId, supplierId, supplierAMUId, customerAMUId);
            lstDetailedCarrierSummary = lstDetailedCarrierSummary.OrderBy(x => x.SaleZoneName).ToList();
            return FormatDetailedCarrierSummaries(lstDetailedCarrierSummary);
        }

        public List<CustomerSummaryFormatted> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, int? customerAMUId, int? supplierAMUId)
        {
            List<CustomerSummary> customerSummaries = _datamanager.GetCustomerSummary(fromDate, toDate, customerId, customerAMUId, supplierAMUId);

            List<CustomerServices> customerServices = _datamanager.GetCustomerServices(fromDate, toDate);
            List<CustomerSummaryFormatted> customerSummariesFormatted = new List<CustomerSummaryFormatted>();

            Dictionary<string, double> totalServicesPerCustomer = new Dictionary<string, double>();
            foreach (var obj in customerServices)
            {
                if (obj.AccountId != null)
                    totalServicesPerCustomer.Add(obj.AccountId, obj.Services);

            }
            foreach (var cs in customerSummaries)
            {
                CustomerSummaryFormatted entitie = new CustomerSummaryFormatted()
                {
                    Carrier = cs.Carrier,
                    SaleDuration = cs.SaleDuration,
                    SaleDurationFormatted = FormatNumber(cs.SaleDuration),
                    SaleNet = cs.SaleNet,
                    SaleNetFormatted = FormatNumberDigitRate(cs.SaleNet),
                    CostDuration = cs.CostDuration,
                    CostDurationFormatted = FormatNumber(cs.CostDuration),
                    CostNet = cs.CostNet,
                    CostNetFormatted = FormatNumberDigitRate(cs.CostNet),
                    Profit = (cs.SaleNet > 0) ? ((cs.SaleNet - cs.CostNet)) : 0,
                    ProfitFormatted = FormatNumber((cs.SaleNet > 0) ? ((cs.SaleNet - cs.CostNet)) : 0),
                    ProfitPercentageFormatted = (cs.SaleNet > 0) ? FormatNumberPercentage(((cs.SaleNet - cs.CostNet) / cs.SaleNet)) : FormatNumberPercentage(0),
                };
                if (cs.Carrier != null)
                {
                    entitie.Customer = _bemanager.GetCarrirAccountName(cs.Carrier);
                    entitie.Services = (totalServicesPerCustomer.ContainsKey(cs.Carrier)) ? totalServicesPerCustomer[cs.Carrier] : 0;
                    entitie.ServicesFormatted = FormatNumber((totalServicesPerCustomer.ContainsKey(cs.Carrier)) ? totalServicesPerCustomer[cs.Carrier] : 0);
                }
                else
                {
                    entitie.Customer = null;
                    entitie.Services = 0;
                    entitie.ServicesFormatted = FormatNumber(0.00);

                }
                customerSummariesFormatted.Add(entitie);
            }

            return customerSummariesFormatted.OrderBy(cs => cs.Customer).ToList();
        }

        public List<ProfitSummary> GetCustomerProfitSummary(List<CustomerSummaryFormatted> summarieslist)
        {
            List<ProfitSummary> lstProfitSummary = new List<ProfitSummary>();
            lstProfitSummary = summarieslist.Select(r => new ProfitSummary
            {
                Profit = (r.SaleNet > 0) ? (double)((r.SaleNet - r.CostNet)) : 0,
                FormattedProfit = FormatNumber((r.SaleNet > 0) ? ((r.SaleNet - r.CostNet)) : 0),
                Customer = r.Customer.ToString()
            }).OrderByDescending(r => r.Profit).Take(10).ToList();
            return lstProfitSummary;
        }
        public List<SaleAmountSummary> GetCustomerSaleAmountSummary(List<CustomerSummaryFormatted> summarieslist)
        {
            List<SaleAmountSummary> saleAmountSummary = new List<SaleAmountSummary>();

            foreach (var cs in summarieslist)
            {
                SaleAmountSummary s = new SaleAmountSummary()
                {
                    SaleAmount = cs.SaleNet != null ? (double)cs.SaleNet : 0,
                    FormattedSaleAmount = FormatNumber(cs.SaleNet != null ? (double)cs.SaleNet : 0),
                    Customer = cs.Customer
                };
                saleAmountSummary.Add(s);
            }
            return saleAmountSummary.OrderByDescending(s => s.SaleAmount).Take(10).ToList();
        }

        public List<DailyForcastingFormatted> GetDailyForcasting(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatDailyForcastingSummaries(_datamanager.GetDailyForcasting(fromDate, toDate, supplierAMUId, customerAMUId));
        }
        public List<ExchangeCarrierFormatted> GetExchangeCarriers(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId, bool IsExchange)
        {
            List<ExchangeCarriers> baseList = _datamanager.GetExchangeCarriers(fromDate, toDate, supplierAMUId, customerAMUId);

            List<ExchangeCarrierProfit> ecpList = new List<ExchangeCarrierProfit>();
            foreach (var row in baseList)
            {
                ExchangeCarrierProfit ecp = new ExchangeCarrierProfit()
                {
                    CarrierAccount = _cmanager.GetCarrierAccount(row.CustomerID),
                    CustomerProfit = (double)row.CustomerProfit,
                    SupplierProfit = (double)row.SupplierProfit
                };
                ecpList.Add(ecp);
            }

            List<ExchangeCarrierFormatted> result = new List<ExchangeCarrierFormatted>();

            var test = ecpList;
            if (IsExchange == true)
            {
                result = ecpList.Where(c => (AccountType)c.CarrierAccount.AccountType == AccountType.Exchange)
                .Select(c =>
                new ExchangeCarrierFormatted
                {
                    Customer = c.CarrierAccount.ProfileName,
                    CustomerProfit = c.CustomerProfit,
                    SupplierProfit = c.SupplierProfit,
                    FormattedCustomerProfit = FormatNumber(c.CustomerProfit),
                    FormattedSupplierProfit = FormatNumber(c.SupplierProfit),
                    Total = FormatNumber(c.CustomerProfit + c.SupplierProfit)

                }).OrderByDescending(r => r.CustomerProfit + r.SupplierProfit).ToList();
            }
            else
            {
                result = ecpList.GroupBy(c => new { c.CarrierAccount.ProfileName }).Select(obj => new ExchangeCarrierFormatted
                {
                    Customer = obj.Key.ProfileName,
                    CustomerProfit = obj.Sum(d => d.CustomerProfit),
                    SupplierProfit = obj.Sum(d => d.SupplierProfit),
                    FormattedCustomerProfit = FormatNumber(obj.Sum(d => d.CustomerProfit)),
                    FormattedSupplierProfit = FormatNumber(obj.Sum(d => d.SupplierProfit)),
                    Total = FormatNumber(obj.Sum(d => d.CustomerProfit) + obj.Sum(d => d.SupplierProfit))

                }).OrderByDescending(r => r.CustomerProfit + r.SupplierProfit).ToList();
            }
            return result;
        }

        public VariationReportResult GetVariationReportsData(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOptions, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy)
        {
            List<TimeRange> timeRanges = new List<TimeRange>();
            DateTime currentDate = new DateTime();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();
            currentDate = selectedDate.AddDays(1);
            int counter = periodCount;
            while (counter > 0)
            {
                switch (timePeriod)
                {
                    case TimePeriod.Days:
                        fromDate = currentDate.AddDays(-1);
                        toDate = currentDate;
                        break;
                    case TimePeriod.Weeks:
                        fromDate = currentDate.AddDays(-7);
                        toDate = currentDate;
                        break;
                    case TimePeriod.Months:
                        fromDate = currentDate.AddMonths(-1);
                        toDate = currentDate;
                        break;
                }
                TimeRange timeRange = new TimeRange { FromDate = fromDate, ToDate = toDate };
                timeRanges.Add(timeRange);
                currentDate = fromDate;
                counter = counter - 1;

            }
            int totalCount;
            List<decimal> totalValues, totals;
            List<DateTime> datetotalValues;
            decimal totalAverage, totalPercentage, totalPreviousPercentage;
            List<VariationReports> variationReports = _datamanager.GetVariationReportsData(timeRanges, variationReportOptions, fromRow, toRow, entityType, entityID, groupingBy, out totalCount, out totalValues, out datetotalValues, out totalAverage);
            var result = GetVariationReportsData(variationReports, timeRanges, selectedDate, periodCount, totalValues, datetotalValues, totalAverage, out totals, out totalPercentage, out totalPreviousPercentage);
            result.TotalCount = totalCount;
            result.TotalValues = totals;
            result.TotalAverage = totalAverage;
            result.TotalPercentage = totalPercentage;
            result.TotalPreviousPercentage = totalPreviousPercentage;
            return result;
        }

        public VariationReportResult GetVariationReportsData(List<VariationReports> variationReports, List<TimeRange> timeRanges, DateTime selectedDate, int periodCount, List<decimal> totalValues, List<DateTime> datetotalValues, decimal totalAverage, out List<decimal> totals, out decimal totalPercentage, out decimal totalPreviousPercentage)
        {
            List<VariationReportsData> variationReportsData = new List<VariationReportsData>();
            VariationReportsData current = null;
            foreach (var item in variationReports)
            {
                if (current == null || current.ID != item.ID)
                {
                    current = new VariationReportsData
                    {
                        ID = item.ID,
                        Name = item.Name,
                        RowNumber = item.RowNumber,
                        Values = new List<decimal>()
                    };
                    variationReportsData.Add(current);

                }
            }

            foreach (var rep in variationReportsData)
            {
                foreach (var timeRange in timeRanges)
                {
                    var value = variationReports.FirstOrDefault(itm => itm.ID == rep.ID && itm.FromDate == timeRange.FromDate && itm.ToDate == timeRange.ToDate);
                    if (value != null)
                        rep.Values.Add(value.TotalDuration);
                    else
                        rep.Values.Add(0);
                }
            }

            foreach (var item in variationReportsData)
            {
                decimal average = 0;
                double CurrentValue = double.Parse(item.Values.First().ToString());
                double PrevValue = double.Parse(item.Values[1].ToString());
                foreach (var totalDurations in item.Values)
                    average += totalDurations;
                average = average / periodCount;
                item.PeriodTypeValueAverage = average;
                item.PeriodTypeValuePercentage = Convert.ToDecimal((CurrentValue - Convert.ToDouble(average)) / (average == 0 ? double.MaxValue : Convert.ToDouble(average))) * 100;
                item.PreviousPeriodTypeValuePercentage = Convert.ToDecimal((CurrentValue - PrevValue) / (PrevValue == 0 ? double.MaxValue : PrevValue)) * 100;

            }

            totals = new List<decimal>();
            int i = 0;

            foreach (var timeRange in timeRanges)
            {
                if (datetotalValues.Contains(timeRange.FromDate))
                {
                    totals.Add(totalValues[i]);
                    i++;
                }
                else
                    totals.Add(0);

            }

            ////Calcule of Total AVG, Total %, Total Previouss %: 
            foreach (var item in variationReportsData)
            {
                decimal average = 0;
                double CurrentDayValue = double.Parse(item.Values.First().ToString());
                double PrevDayValue = double.Parse(item.Values[1].ToString());
                foreach (var totalDurations in item.Values)
                    average += totalDurations;
                average = average / periodCount;
                item.PeriodTypeValueAverage = average;
                item.PeriodTypeValuePercentage = Convert.ToDecimal((CurrentDayValue - Convert.ToDouble(average)) / (average == 0 ? double.MaxValue : Convert.ToDouble(average))) * 100;
                item.PreviousPeriodTypeValuePercentage = Convert.ToDecimal((CurrentDayValue - PrevDayValue) / (PrevDayValue == 0 ? double.MaxValue : PrevDayValue)) * 100;

            }
            double currentValue = double.Parse(totals.FirstOrDefault().ToString());
            double prevValue = double.Parse(totals[1].ToString());
            totalPercentage = Convert.ToDecimal((currentValue - Convert.ToDouble(totalAverage)) / (totalAverage == 0 ? double.MaxValue : Convert.ToDouble(totalAverage))) * 100;
            totalPreviousPercentage = Convert.ToDecimal((currentValue - prevValue) / (prevValue == 0 ? double.MaxValue : prevValue)) * 100;

            return new VariationReportResult() { VariationReportsData = variationReportsData.OrderBy(itm => itm.RowNumber), TimeRange = timeRanges };

        }

        public VariationReportResult GetInOutVariationReportsData(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOptions, EntityType entityType, string entityID, GroupingBy groupingBy, int fromRow, int toRow)
        {
            List<VariationReportsData> customersList = new List<VariationReportsData>();
            List<VariationReportsData> onlyCustomersList = new List<VariationReportsData>();
            List<VariationReportsData> suppliersList = new List<VariationReportsData>();
            List<VariationReportsData> customersAndSuppliersList = new List<VariationReportsData>();
            List<TimeRange> timeRanges;
            VariationReportOptions customersreport;
            VariationReportOptions suppliersReport;


            if (variationReportOptions == VariationReportOptions.InOutBoundMinutes)
            {
                customersreport = VariationReportOptions.InBoundMinutes;
                suppliersReport = VariationReportOptions.OutBoundMinutes;
            }
            else
            {

                customersreport = VariationReportOptions.InBoundAmount;
                suppliersReport = VariationReportOptions.OutBoundAmount;
            }
            var customersResult = GetVariationReportsData(selectedDate, periodCount, timePeriod, customersreport, 0, 0, entityType, entityID, groupingBy);
            timeRanges = customersResult.TimeRange;
            foreach (var customer in customersResult.VariationReportsData)
            {
                customersList.Add(customer);

            }
            var suppliersResult = GetVariationReportsData(selectedDate, periodCount, timePeriod, suppliersReport, 0, 0, entityType, entityID, groupingBy);
            foreach (var supplier in suppliersResult.VariationReportsData)
            {
                suppliersList.Add(supplier);

            }

            foreach (var customer in customersList)
            {
                var matchedSupplier = suppliersList.FirstOrDefault(s => s.ID == customer.ID);
                if (matchedSupplier != null)
                {
                    //create 3 item in reslut list : IN, Out ,Total
                    var name = customer.Name;
                    customer.Name = string.Format("{0}/IN", name);
                    customersAndSuppliersList.Add(customer);
                    matchedSupplier.Name = string.Format("{0}/OUT", name);
                    customersAndSuppliersList.Add(matchedSupplier);

                    //Total:
                    var total = new VariationReportsData();
                    total.Name = string.Format("{0}/TOTAL", name);
                    total.PeriodTypeValueAverage = customer.PeriodTypeValueAverage - matchedSupplier.PeriodTypeValueAverage;
                    total.PeriodTypeValuePercentage = customer.PeriodTypeValuePercentage - matchedSupplier.PeriodTypeValuePercentage;
                    total.PreviousPeriodTypeValuePercentage = matchedSupplier.PreviousPeriodTypeValuePercentage - customer.PreviousPeriodTypeValuePercentage;
                    List<decimal> totalValues = new List<decimal>();
                    for (int i = 0; i < customer.Values.Count; i++)
                        totalValues.Add(customer.Values[i] - matchedSupplier.Values[i]);
                    total.Values = totalValues;
                    customersAndSuppliersList.Add(total);

                    //remove out element from out list
                    suppliersList.Remove(matchedSupplier);

                }
                else
                    //add item to in list
                    onlyCustomersList.Add(customer);

            }
            customersAndSuppliersList = customersAndSuppliersList.OrderBy(item => item.Name).ToList();

            //add items in in list
            //add items remaining in out list
            foreach (var item in onlyCustomersList)
                item.Name = string.Format("{0}/IN", item.Name);
            foreach (var item in suppliersList)
                item.Name = string.Format("{0}/OUT", item.Name);

            customersAndSuppliersList.AddRange(onlyCustomersList.OrderBy(customer => customer.Name).ToList());

            customersAndSuppliersList.AddRange(suppliersList.OrderBy(supplier => supplier.Name).ToList());

            if (customersAndSuppliersList.Count > toRow && customersAndSuppliersList.Count > fromRow)
                customersAndSuppliersList = customersAndSuppliersList.GetRange(fromRow, toRow);
            else customersAndSuppliersList = customersAndSuppliersList.GetRange(0, customersAndSuppliersList.Count());
            return new VariationReportResult() { VariationReportsData = customersAndSuppliersList, TimeRange = timeRanges };

        }
     
        public List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string customerID, string supplierID, string zoneID, int attempts, VolumeReportsTimePeriod timePeriod)
        {
            return _datamanager.GetTrafficVolumes(fromDate, toDate, customerID, supplierID, zoneID, attempts, timePeriod);

        }

        public DestinationVolumeTrafficResult GetDestinationTrafficVolumes(DateTime fromDate, DateTime toDate, string customerID, string supplierID, int zoneID, int attempts, VolumeReportsTimePeriod timePeriod, int topDestination)
        {
            return _datamanager.GetDestinationTrafficVolumes(fromDate, toDate, customerID, supplierID, zoneID, attempts, timePeriod, topDestination);
       }
        #region Private Methods
        private ZoneProfitFormatted FormatZoneProfit(ZoneProfit zoneProfit)
        {
            return new ZoneProfitFormatted
            {
                CostZone = zoneProfit.CostZone,
                SaleZone = zoneProfit.SaleZone,
                SupplierID = (zoneProfit.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneProfit.SupplierID) : null,
                CustomerID = (zoneProfit.CustomerID != null) ? _bemanager.GetCarrirAccountName(zoneProfit.CustomerID) : null,
                Calls = zoneProfit.Calls,

                DurationNet = zoneProfit.DurationNet,
                DurationNetFormated = FormatNumber(zoneProfit.DurationNet),

                SaleDuration = zoneProfit.SaleDuration,
                SaleDurationFormated = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleDuration.HasValue) ? FormatNumber(zoneProfit.SaleDuration) : "0.00",

                SaleNet = zoneProfit.SaleNet,
                SaleNetFormated = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleNet.HasValue) ? FormatNumberDigitRate(zoneProfit.SaleNet) : "0.00",

                CostDuration = zoneProfit.CostDuration,
                CostDurationFormated = FormatNumberDigitRate(zoneProfit.CostDuration),

                CostNet = zoneProfit.CostNet,
                CostNetFormated = (zoneProfit.CostNet.HasValue) ? FormatNumberDigitRate(zoneProfit.CostNet) : "0.00",

                Profit = zoneProfit.SaleNet == 0 ? "" : FormatNumber((!zoneProfit.SaleNet.HasValue) ? 0 : zoneProfit.SaleNet - zoneProfit.CostNet),
                ProfitSum = (!zoneProfit.SaleNet.HasValue || zoneProfit.SaleNet == 0) ? 0 : zoneProfit.SaleNet - zoneProfit.CostNet,
                ProfitPercentage = zoneProfit.SaleNet == 0 ? "" : (zoneProfit.SaleNet.HasValue) ? FormatNumber(((1 - zoneProfit.CostNet / zoneProfit.SaleNet)) * 100) : "-100%",
            };
        }

        private ZoneSummaryFormatted FormatZoneSummary(ZoneSummary zoneSummary)
        {
            return new ZoneSummaryFormatted
            {
                Zone = zoneSummary.Zone,
                SupplierID = (zoneSummary.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneSummary.SupplierID) : null,

                Calls = zoneSummary.Calls,

                Rate = zoneSummary.Rate,
                RateFormatted = FormatNumberDigitRate(zoneSummary.Rate),

                DurationNet = zoneSummary.DurationNet,
                DurationNetFormatted = FormatNumber(zoneSummary.DurationNet),

                RateType = zoneSummary.RateType,
                RateTypeFormatted = ((RateType)zoneSummary.RateType).ToString(),

                DurationInSeconds = zoneSummary.DurationInSeconds,
                DurationInSecondsFormatted = FormatNumber(zoneSummary.DurationInSeconds),

                Net = zoneSummary.Net,
                NetFormatted = FormatNumberDigitRate(zoneSummary.Net),

                CommissionValue = zoneSummary.CommissionValue,
                CommissionValueFormatted = FormatNumber(zoneSummary.CommissionValue),

                ExtraChargeValue = zoneSummary.ExtraChargeValue
            };
        }

        private ZoneSummaryDetailedFormatted FormatZoneSummaryDetailed(ZoneSummaryDetailed zoneSummaryDetailed)
        {
            return new ZoneSummaryDetailedFormatted
            {
                Zone = zoneSummaryDetailed.Zone,
                ZoneId = zoneSummaryDetailed.ZoneId,

                SupplierID = (zoneSummaryDetailed.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneSummaryDetailed.SupplierID) : null,

                Calls = zoneSummaryDetailed.Calls,

                Rate = zoneSummaryDetailed.Rate,
                RateFormatted = FormatNumberDigitRate(zoneSummaryDetailed.Rate),

                DurationNet = zoneSummaryDetailed.DurationNet,
                DurationNetFormatted = FormatNumber(zoneSummaryDetailed.DurationNet),

                OffPeakDurationInSeconds = zoneSummaryDetailed.OffPeakDurationInSeconds,
                OffPeakDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.OffPeakDurationInSeconds),

                OffPeakRate = zoneSummaryDetailed.OffPeakRate,
                OffPeakRateFormatted = FormatNumberDigitRate(zoneSummaryDetailed.OffPeakRate),

                OffPeakNet = zoneSummaryDetailed.OffPeakNet,
                OffPeakNetFormatted = FormatNumberDigitRate(zoneSummaryDetailed.OffPeakRate),

                WeekEndDurationInSeconds = zoneSummaryDetailed.WeekEndDurationInSeconds,
                WeekEndDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.WeekEndDurationInSeconds),

                WeekEndRate = zoneSummaryDetailed.WeekEndRate,
                WeekEndRateFormatted = FormatNumberDigitRate(zoneSummaryDetailed.WeekEndRate),

                WeekEndNet = zoneSummaryDetailed.WeekEndNet,
                WeekEndNetFormatted = FormatNumberDigitRate(zoneSummaryDetailed.WeekEndNet),

                DurationInSeconds = zoneSummaryDetailed.DurationInSeconds,
                DurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.DurationInSeconds),

                Net = zoneSummaryDetailed.Net,
                NetFormatted = FormatNumberDigitRate(zoneSummaryDetailed.Net),

                TotalDurationFormatted = FormatNumber((zoneSummaryDetailed.DurationInSeconds + zoneSummaryDetailed.OffPeakDurationInSeconds + zoneSummaryDetailed.WeekEndDurationInSeconds)),

                TotalAmountFormatted = FormatNumberDigitRate(zoneSummaryDetailed.Net + zoneSummaryDetailed.OffPeakNet + zoneSummaryDetailed.WeekEndNet),

                CommissionValue = zoneSummaryDetailed.CommissionValue,
                CommissionValueFormatted = FormatNumber(zoneSummaryDetailed.CommissionValue),

                ExtraChargeValue = zoneSummaryDetailed.ExtraChargeValue,
                ExtraChargeValueFormatted = FormatNumber(zoneSummaryDetailed.ExtraChargeValue)
            };
        }

        private DailySummaryFormatted FormatDailySummary(DailySummary dailySummary)
        {
            return new DailySummaryFormatted
            {
                Day = dailySummary.Day,

                Calls = dailySummary.Calls,
                CallsFormatted = FormatNumber(dailySummary.Calls),

                DurationNet = dailySummary.DurationNet,
                DurationNetFormatted = FormatNumber(dailySummary.DurationNet),

                SaleDuration = dailySummary.SaleDuration,
                SaleDurationFormatted = FormatNumber(dailySummary.SaleDuration),

                SaleNet = dailySummary.SaleNet,
                SaleNetFormatted = FormatNumberDigitRate(dailySummary.SaleNet),

                CostNet = dailySummary.CostNet,
                CostNetFormatted = FormatNumberDigitRate(dailySummary.CostNet),

                ProfitFormatted = FormatNumber(dailySummary.SaleNet - dailySummary.CostNet),

                Profit = dailySummary.SaleNet - dailySummary.CostNet,

                ProfitPercentageFormatted = (dailySummary.SaleNet.HasValue) && (dailySummary.SaleNet != 0) ? FormatNumberPercentage(1 - dailySummary.CostNet / dailySummary.SaleNet) : "",
            };
        }

        private SupplierCostDetails AddGroupName(SupplierCostDetails supplierCostDetails)
        {
            CarrierAccount ca = (supplierCostDetails.Customer == null) ? null : _cmanager.GetCarrierAccount(supplierCostDetails.Customer) == null ? null : _cmanager.GetCarrierAccount(supplierCostDetails.Customer);

            CarrierGroup cg;
            String customerGroupName = null;
            if (ca != null)
            {
                if (ca.CarrierGroupID.HasValue)
                {
                    _cmanager.GetAllCarrierGroups().TryGetValue(ca.CarrierGroupID.Value, out cg);
                    customerGroupName = cg.CarrierGroupName;
                }
            }

            return new SupplierCostDetails
            {
                Customer = (supplierCostDetails.Customer != null) ? _bemanager.GetCarrirAccountName(supplierCostDetails.Customer) : null,
                Carrier = (supplierCostDetails.Carrier != null) ? _bemanager.GetCarrirAccountName(supplierCostDetails.Carrier) : null,
                Duration = supplierCostDetails.Duration,
                Amount = supplierCostDetails.Amount,
                CustomerGroupName = customerGroupName == null ? "Others" : customerGroupName
            };
        }


        private SupplierCostDetailsFormatted FormatSupplierCostDetails(SupplierCostDetails supplierCostDetails)
        {
            return new SupplierCostDetailsFormatted
            {
                Customer = supplierCostDetails.Customer,
                Carrier = supplierCostDetails.Carrier,
                Duration = supplierCostDetails.Duration,
                DurationFormatted = FormatNumber(supplierCostDetails.Duration),
                Amount = supplierCostDetails.Amount,
                AmountFormatted = FormatNumberDigitRate(supplierCostDetails.Amount),
                CustomerGroupName = supplierCostDetails.CustomerGroupName
            };
        }

        private SaleZoneCostSummaryFormatted FormatSaleZoneCostSummary(SaleZoneCostSummary saleZoneCostSummary)
        {
            return new SaleZoneCostSummaryFormatted
            {
                AvgCost = saleZoneCostSummary.AvgCost,
                AvgCostFormatted = FormatNumberDigitRate(saleZoneCostSummary.AvgCost),
                salezoneID = saleZoneCostSummary.salezoneID,
                salezoneIDFormatted = _bemanager.GetZoneName(saleZoneCostSummary.salezoneID),
                AvgDuration = saleZoneCostSummary.AvgDuration,
                AvgDurationFormatted = FormatNumber(saleZoneCostSummary.AvgDuration)
            };
        }

        private SaleZoneCostSummaryServiceFormatted FormatSaleZoneCostSummaryService(SaleZoneCostSummaryService saleZoneCostSummaryService)
        {
            return new SaleZoneCostSummaryServiceFormatted
            {
                AvgServiceCost = saleZoneCostSummaryService.AvgServiceCost,
                AvgServiceCostFormatted = FormatNumber(saleZoneCostSummaryService.AvgServiceCost),

                salezoneID = saleZoneCostSummaryService.salezoneID,
                salezoneIDFormatted = _bemanager.GetZoneName(saleZoneCostSummaryService.salezoneID),
                Service = saleZoneCostSummaryService.Service,

                AvgDuration = saleZoneCostSummaryService.AvgDuration,
                AvgDurationFormatted = FormatNumber(saleZoneCostSummaryService.AvgDuration)
            };
        }


        private SaleZoneCostSummarySupplierFormatted FormatSaleZoneCostSummarySupplier(SaleZoneCostSummarySupplier saleZoneCostSummarySupplier)
        {
            return new SaleZoneCostSummarySupplierFormatted
            {
                SupplierID = (saleZoneCostSummarySupplier.SupplierID != null) ? _bemanager.GetCarrirAccountName(saleZoneCostSummarySupplier.SupplierID) : null,

                HighestRate = saleZoneCostSummarySupplier.HighestRate,
                HighestRateFormatted = FormatNumber(saleZoneCostSummarySupplier.HighestRate),

                salezoneID = saleZoneCostSummarySupplier.salezoneID,

                salezoneIDFormatted = _bemanager.GetZoneName(saleZoneCostSummarySupplier.salezoneID),

                AvgDuration = saleZoneCostSummarySupplier.AvgDuration,
                AvgDurationFormatted = FormatNumber(saleZoneCostSummarySupplier.AvgDuration)
            };
        }

        private CarrierLostFormatted FormatCarrierLost(CarrierLost carrierLost)
        {

            return new CarrierLostFormatted
            {
                CustomerID = carrierLost.CustomerID,
                CustomerName = _bemanager.GetCarrirAccountName(carrierLost.CustomerID),
                SupplierID = carrierLost.SupplierID,
                SupplierName = _bemanager.GetCarrirAccountName(carrierLost.SupplierID),
                SaleZoneID = carrierLost.SaleZoneID,
                SaleZoneName = _bemanager.GetZoneName(carrierLost.SaleZoneID),
                CostZoneID = carrierLost.CostZoneID,
                CostZoneName = _bemanager.GetZoneName(carrierLost.CostZoneID),
                Duration = carrierLost.Duration,
                DurationFormatted = FormatNumber(carrierLost.Duration),
                CostNet = carrierLost.CostNet,
                CostNetFormatted = FormatNumberDigitRate(carrierLost.CostNet),
                SaleNet = carrierLost.SaleNet,
                SaleNetFormatted = FormatNumberDigitRate(carrierLost.SaleNet),
                Margin = FormatNumber(carrierLost.SaleNet - carrierLost.CostNet),
                Percentage = FormatNumberPercentage(1 - carrierLost.CostNet / carrierLost.SaleNet)

            };
        }
        private List<RateLossFormatted> FormatRateLosses(List<RateLoss> rateLosses)
        {
            List<RateLossFormatted> models = new List<RateLossFormatted>();

            Dictionary<string, CarrierAccount> carrierAccounts = _cmanager.GetAllCarrierAccounts();

            Dictionary<int, CarrierGroup> carrierGroups = _cmanager.GetAllCarrierGroups();

            if (rateLosses != null)
                for (int j = 0; j < rateLosses.Count(); j++)
                {
                    if (carrierAccounts.ContainsKey(rateLosses[j].CustomerID))
                    {
                        CarrierAccount carrierAccount = carrierAccounts[rateLosses[j].CustomerID];

                        if (carrierAccount.GroupIds != null)
                            //for (int i = 0; i < carrierAccount.GroupIds.Count(); i++)
                            if (carrierGroups.ContainsKey(carrierAccount.GroupIds[0]))
                            {
                                CarrierGroup carrierGroup = carrierGroups[carrierAccount.GroupIds[0]];
                                //if (i == 0)
                                rateLosses[j].CarrierGroupsNames = carrierGroup.CarrierGroupName;
                                //else
                                //    rateLosses[j].CarrierGroupsNames = rateLosses[j].CarrierGroupsNames + ", " + carrierGroup.CarrierGroupName;
                            }
                    }
                    models.Add(FormatRateLoss(rateLosses[j]));
                }
            models = models.OrderByDescending(x => x.Loss).ToList<RateLossFormatted>();
            return models;
        }
        private RateLossFormatted FormatRateLoss(RateLoss rateLoss)
        {
            return new RateLossFormatted
           {
               CostZone = rateLoss.CostZone,
               SaleZone = rateLoss.SaleZone,
               CustomerID = rateLoss.CustomerID,
               Customer = (rateLoss.CustomerID != null) ? _bemanager.GetCarrirAccountName(rateLoss.CustomerID) : null,
               SupplierID = rateLoss.SupplierID,
               Supplier = (rateLoss.SupplierID != null) ? _bemanager.GetCarrirAccountName(rateLoss.SupplierID) : null,
               SaleRate = rateLoss.SaleRate,
               SaleRateFormatted = FormatNumberDigitRate(rateLoss.SaleRate),
               CostRate = rateLoss.CostRate,
               CostRateFormatted = FormatNumberDigitRate(rateLoss.CostRate),
               CostDuration = rateLoss.CostDuration,
               CostDurationFormatted = FormatNumber(rateLoss.CostDuration),
               SaleDuration = rateLoss.SaleDuration,
               SaleDurationFormatted = FormatNumber(rateLoss.SaleDuration),
               CostNet = rateLoss.CostNet,
               CostNetFormatted = FormatNumberDigitRate(rateLoss.CostNet),
               SaleNet = rateLoss.SaleNet,
               SaleNetFormatted = FormatNumberDigitRate(rateLoss.SaleNet),
               SaleZoneID = rateLoss.SaleZoneID,
               Loss = rateLoss.CostNet.Value - rateLoss.SaleNet.Value,
               LossFormatted = FormatNumber(rateLoss.CostNet - rateLoss.SaleNet),
               LossPerFormatted = FormatNumber(((rateLoss.CostNet - rateLoss.SaleNet) * 100) / rateLoss.CostNet),
               CarrierGroupsNames = rateLoss.CarrierGroupsNames
           };
        }
        private string FormatNumberDigitRate(Decimal? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }
        private string FormatNumberDigitRate(Double? number)
        {
            int precision = 4;//Digit Rate 
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }

        private string FormatNumber(Decimal? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }

        private string FormatNumber(int? number)
        {
            return String.Format("{0:#,###0}", number);
        }

        private string FormatNumberPercentage(Double? number)
        {
            return String.Format("{0:#,##0.00%}", number);
        }
        private string FormatNumber(Double? number)
        {
            return String.Format("{0:#,###0.00}", number);
        }
        private List<ZoneProfitFormatted> FormatZoneProfits(List<ZoneProfit> zoneProfits)
        {
            List<ZoneProfitFormatted> models = new List<ZoneProfitFormatted>();
            if (zoneProfits != null)
                foreach (var z in zoneProfits)
                {
                    models.Add(FormatZoneProfit(z));
                }
            return models;
        }

        private List<ZoneSummaryFormatted> FormatZoneSummaries(List<ZoneSummary> zoneSummaries)
        {
            List<ZoneSummaryFormatted> models = new List<ZoneSummaryFormatted>();
            if (zoneSummaries != null)
                foreach (var z in zoneSummaries)
                {
                    models.Add(FormatZoneSummary(z));
                }
            return models;
        }
        private List<ZoneSummaryDetailedFormatted> FormatZoneSummariesDetailed(List<ZoneSummaryDetailed> zoneSummariesDetailed)
        {
            List<ZoneSummaryDetailedFormatted> models = new List<ZoneSummaryDetailedFormatted>();
            if (zoneSummariesDetailed != null)
                foreach (var z in zoneSummariesDetailed)
                {
                    models.Add(FormatZoneSummaryDetailed(z));
                }
            return models;
        }

        private List<DailySummaryFormatted> FormatDailySummaries(List<DailySummary> dailySummary)
        {
            List<DailySummaryFormatted> models = new List<DailySummaryFormatted>();
            if (dailySummary != null)
                foreach (var z in dailySummary)
                {
                    models.Add(FormatDailySummary(z));
                }
            return models;
        }

        private List<SupplierCostDetails> AddGroupNames(List<SupplierCostDetails> supplierCostDetails)
        {
            List<SupplierCostDetails> models = new List<SupplierCostDetails>();
            if (supplierCostDetails != null)
                foreach (var z in supplierCostDetails)
                {
                    models.Add(AddGroupName(z));
                }
            return models;
        }

        private List<SupplierCostDetailsFormatted> FormatSupplierCostDetails(List<SupplierCostDetails> supplierCostDetails)
        {
            List<SupplierCostDetailsFormatted> models = new List<SupplierCostDetailsFormatted>();
            if (supplierCostDetails != null)
                foreach (var z in supplierCostDetails)
                {
                    models.Add(FormatSupplierCostDetails(z));
                }
            return models;
        }

        private List<SaleZoneCostSummaryFormatted> FormatSaleZoneCostSummary(List<SaleZoneCostSummary> saleZoneCostSummary)
        {
            List<SaleZoneCostSummaryFormatted> models = new List<SaleZoneCostSummaryFormatted>();
            if (saleZoneCostSummary != null)
                foreach (var z in saleZoneCostSummary)
                {
                    models.Add(FormatSaleZoneCostSummary(z));
                }
            return models;
        }

        private List<SaleZoneCostSummaryServiceFormatted> FormatSaleZoneCostSummaryService(List<SaleZoneCostSummaryService> saleZoneCostSummaryService)
        {
            List<SaleZoneCostSummaryServiceFormatted> models = new List<SaleZoneCostSummaryServiceFormatted>();
            if (saleZoneCostSummaryService != null)
                foreach (var z in saleZoneCostSummaryService)
                {
                    models.Add(FormatSaleZoneCostSummaryService(z));
                }
            return models;
        }
        private List<SaleZoneCostSummarySupplierFormatted> FormatSaleZoneCostSummarySupplier(List<SaleZoneCostSummarySupplier> saleZoneCostSummarySupplier)
        {
            List<SaleZoneCostSummarySupplierFormatted> models = new List<SaleZoneCostSummarySupplierFormatted>();
            if (saleZoneCostSummarySupplier != null)
                foreach (var z in saleZoneCostSummarySupplier)
                {
                    models.Add(FormatSaleZoneCostSummarySupplier(z));
                }
            return models;
        }
        private List<CarrierLostFormatted> FormatCarrierLost(List<CarrierLost> carriersLost)
        {

            List<CarrierLostFormatted> models = new List<CarrierLostFormatted>();
            if (carriersLost != null)
                foreach (var z in carriersLost)
                {
                    models.Add(FormatCarrierLost(z));
                }
            return models;

        }

        private List<CarrierSummaryDailyFormatted> FormatCarrieresSummaryDaily(List<CarrierSummaryDaily> carrierSummaryDaily, bool isGroupeByday)
        {

            List<CarrierSummaryDailyFormatted> models = new List<CarrierSummaryDailyFormatted>();
            if (carrierSummaryDaily != null)
                foreach (var z in carrierSummaryDaily)
                {
                    models.Add(FormatCarrierSummaryDaily(z, isGroupeByday));
                }
            return models;

        }
        private List<DailyForcastingFormatted> FormatDailyForcastingSummaries(List<DailyForcasting> carrierDaily)
        {

            List<DailyForcastingFormatted> models = new List<DailyForcastingFormatted>();
            if (carrierDaily != null)
                foreach (var d in carrierDaily)
                {
                    models.Add(FormatDailyForcasting(d));
                }
            return models;

        }

        private CarrierSummaryDailyFormatted FormatCarrierSummaryDaily(CarrierSummaryDaily carrierSummaryDaily, bool isGroupeByday)
        {

            CarrierSummaryDailyFormatted obj = new CarrierSummaryDailyFormatted
            {

                Day = carrierSummaryDaily.Day,
                CarrierID = carrierSummaryDaily.CarrierID,
                Carrier = _bemanager.GetCarrirAccountName(carrierSummaryDaily.CarrierID),
                Attempts = carrierSummaryDaily.Attempts,
                AttemptsFormatted = FormatNumber(carrierSummaryDaily.Attempts),
                DurationNet = carrierSummaryDaily.DurationNet,
                DurationNetFormatted = FormatNumber(carrierSummaryDaily.DurationNet),
                Duration = carrierSummaryDaily.Duration,
                DurationFormatted = FormatNumber(carrierSummaryDaily.Duration),
                Net = carrierSummaryDaily.Net,
                NetFormatted = FormatNumberDigitRate(carrierSummaryDaily.Net)

            };
            if (isGroupeByday)
                obj.Date = obj.Day.ToString();

            return obj;
        }

        private List<CarrierSummaryFormatted> FormatCarrierSummaries(List<CarrierSummary> carrierSummaries)
        {
            List<CarrierSummaryFormatted> models = new List<CarrierSummaryFormatted>();
            if (carrierSummaries != null)
                foreach (var c in carrierSummaries)
                {
                    models.Add(FormatCarrierSummary(c));
                }
            return models;
        }
        private List<DetailedCarrierSummaryFormatted> FormatDetailedCarrierSummaries(List<DetailedCarrierSummary> detailedcarrierSummaries)
        {
            List<DetailedCarrierSummaryFormatted> models = new List<DetailedCarrierSummaryFormatted>();
            if (detailedcarrierSummaries != null)
                foreach (var c in detailedcarrierSummaries)
                {
                    models.Add(FormatDetailedCarrierSummary(c));
                }
            return models.OrderBy(cs => cs.Customer).ToList();
        }

        private DailyForcastingFormatted FormatDailyForcasting(DailyForcasting daily)
        {
            return new DailyForcastingFormatted
            {
                Day = daily.Day,
                SaleNet = daily.SaleNet,
                SaleNetFormatted = FormatNumber(daily.SaleNet),
                CostNet = daily.CostNet,
                CostNetFormatted = FormatNumber(daily.CostNet),
                ProfitFormatted = FormatNumber(daily.SaleNet - daily.CostNet),
                ProfitPercentageFormatted = (daily.SaleNet > 0) ? FormatNumberPercentage((daily.SaleNet - daily.CostNet) / daily.SaleNet) : "0.00%"

            };
        }
        private CarrierSummaryFormatted FormatCarrierSummary(CarrierSummary carrierSummary)
        {
            return new CarrierSummaryFormatted
            {

                SupplierID = carrierSummary.SupplierID,
                Supplier = (carrierSummary.SupplierID != null) ? _bemanager.GetCarrirAccountName(carrierSummary.SupplierID) : null,
                CustomerID = carrierSummary.CustomerID,
                Customer = (carrierSummary.CustomerID != null) ? _bemanager.GetCarrirAccountName(carrierSummary.CustomerID) : null,
                SaleDuration = carrierSummary.SaleDuration,
                SaleDurationFormatted = FormatNumber(carrierSummary.SaleDuration),
                CostDuration = carrierSummary.CostDuration,
                CostDurationFormatted = FormatNumber(carrierSummary.CostDuration),
                CostNet = carrierSummary.CostNet,
                CostNetFormatted = (carrierSummary.CostNet == 0.00) ? "0.00" : FormatNumberDigitRate(carrierSummary.CostNet),
                SaleNet = carrierSummary.SaleNet,
                SaleNetFormatted = (carrierSummary.SaleNet == 0.00) ? "0.00" : FormatNumberDigitRate(carrierSummary.SaleNet),
                CostCommissionValue = carrierSummary.CostCommissionValue,
                CostCommissionValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.CostCommissionValue.Value))),
                SaleCommissionValue = carrierSummary.SaleCommissionValue,
                SaleCommissionValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.SaleCommissionValue.Value))),
                CostExtraChargeValue = carrierSummary.CostExtraChargeValue,
                CostExtraChargeValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.CostExtraChargeValue.Value))),
                SaleExtraChargeValue = carrierSummary.SaleExtraChargeValue,
                SaleExtraChargeValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.SaleExtraChargeValue.Value))),
                Profit = (carrierSummary.SaleNet > 0) ? (double)((carrierSummary.SaleNet - carrierSummary.CostNet)) : 0,
                ProfitFormatted = FormatNumber((carrierSummary.SaleNet > 0) ? ((carrierSummary.SaleNet - carrierSummary.CostNet)) : 0),

                AvgMin = (carrierSummary.SaleDuration.Value != 0) ? (decimal)(((double)carrierSummary.SaleNet.Value - (double)carrierSummary.CostNet.Value) / (double)carrierSummary.SaleDuration.Value) : 0,

                AvgMinFormatted = (carrierSummary.SaleDuration.Value != 0 && carrierSummary.SaleDuration.Value != 0) ? FormatNumberDigitRate((decimal)carrierSummary.SaleNet / carrierSummary.SaleDuration - (decimal)carrierSummary.CostNet / carrierSummary.SaleDuration) : "0.00"

            };
        }

        private DetailedCarrierSummaryFormatted FormatDetailedCarrierSummary(DetailedCarrierSummary detailedCarrier)
        {
            return new DetailedCarrierSummaryFormatted
            {

                CustomerID = detailedCarrier.CustomerID,
                Customer = (detailedCarrier.CustomerID != null) ? _bemanager.GetCarrirAccountName(detailedCarrier.CustomerID) : null,
                SaleZoneID = detailedCarrier.SaleZoneID,
                SaleZoneName = detailedCarrier.SaleZoneName,
                SaleDuration = detailedCarrier.SaleDuration,
                SaleDurationFormatted = FormatNumber(detailedCarrier.SaleDuration),
                SaleRate = detailedCarrier.SaleRate,
                SaleRateFormatted = FormatNumberDigitRate(detailedCarrier.SaleRate),
                SaleRateChange = detailedCarrier.SaleRateChange,
                SaleRateChangeFormatted = ((TOne.Analytics.Entities.Change)detailedCarrier.SaleRateChange).ToString(),
                SaleRateEffectiveDate = detailedCarrier.SaleRateEffectiveDate,
                SaleAmount = detailedCarrier.SaleAmount,
                SaleAmountFormatted = FormatNumber(detailedCarrier.SaleAmount),
                SupplierID = detailedCarrier.SupplierID,
                Supplier = (detailedCarrier.SupplierID != null) ? _bemanager.GetCarrirAccountName(detailedCarrier.SupplierID) : null,
                CostZoneID = detailedCarrier.CostZoneID,
                CostZoneName = detailedCarrier.CostZoneName,
                CostDuration = detailedCarrier.CostDuration,
                CostDurationFormatted = FormatNumber(detailedCarrier.CostDuration),
                CostRate = detailedCarrier.CostRate,
                CostRateFormatted = FormatNumberDigitRate(detailedCarrier.CostRate),
                CostRateChange = detailedCarrier.CostRateChange,
                CostRateChangeFormatted = ((TOne.Analytics.Entities.Change)detailedCarrier.CostRateChange).ToString(),
                CostRateEffectiveDate = detailedCarrier.CostRateEffectiveDate,
                CostAmount = detailedCarrier.CostAmount,
                CostAmountFormatted = FormatNumber(detailedCarrier.CostAmount),
                Profit = detailedCarrier.Profit,
                ProfitFormatted = FormatNumber(detailedCarrier.Profit),
                ProfitPercentage = (detailedCarrier.SaleAmount > 0) ? FormatNumberPercentage(1 - detailedCarrier.CostAmount / detailedCarrier.SaleAmount) : "-100%"
            };
        }

        #endregion
    }
}
