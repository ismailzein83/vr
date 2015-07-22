using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    partial class BillingStatisticDataManager : BaseTOneDataManager, IBillingStatisticDataManager
    {
        public List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, int? supplierAMUId, int? customerAMUId)
        {
            return GetItemsSP("Analytics.SP_BillingRep_GetZoneProfits", (reader) => ZoneProfitMapper(reader, groupByCustomer),
                fromDate,
                toDate,
                (customerId == null || customerId == "") ? null : customerId,
                (supplierId == null || supplierId == "") ? null : supplierId,
                groupByCustomer,
                (supplierAMUId == 0) ? (object)DBNull.Value : supplierAMUId,
                (customerAMUId == 0) ? (object)DBNull.Value : customerAMUId);
        }
        public List<ZoneSummary> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier, out double services)
        {
            List<ZoneSummary> lstZoneSummary = new List<ZoneSummary>();
            services = 0;
            double servicesFees = 0;
            ExecuteReaderSP("Analytics.SP_BillingRep_GetZoneSummary", (reader) =>
            {
                while (reader.Read())
                {
                    lstZoneSummary.Add(new ZoneSummary
                    {
                        Zone = reader["Zone"] as string,
                        Calls = GetReaderValue<int>(reader, "Calls"),
                        Rate = GetReaderValue<double>(reader, "Rate"),
                        DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                        DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                        RateType = GetReaderValue<byte>(reader, "RateType"),
                        Net = GetReaderValue<double>(reader, "Net"),
                        CommissionValue = GetReaderValue<double>(reader, "CommissionValue"),
                        ExtraChargeValue = GetReaderValue<double>(reader, "ExtraChargeValue"),
                        SupplierID = (groupBySupplier == true ? reader["SupplierID"] as string : null)
                    });
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        servicesFees = GetReaderValue<double>(reader, "Services");
                    }
                }
            },
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               isCost,
               currencyId,
               (supplierGroup == null || supplierGroup == "") ? null : supplierGroup,
               (customerGroup == null || customerGroup == "") ? null : customerGroup,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
               groupBySupplier);
            services = servicesFees;
            return lstZoneSummary;

            //return GetItemsSP("Analytics.SP_BillingRep_GetZoneSummary", (reader) => ZoneSummaryMapper(reader, groupBySupplier),
            //   fromDate,
            //   toDate,
            //   (customerId == null || customerId == "") ? null : customerId,
            //   (supplierId == null || supplierId == "") ? null : supplierId,
            //   isCost,
            //   currencyId,
            //   (supplierGroup == null || supplierGroup == "") ? null : supplierGroup,
            //   (customerGroup == null || customerGroup == "") ? null : customerGroup,
            //   (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
            //   (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
            //   groupBySupplier
            //   );
        }
        public List<ZoneSummaryDetailed> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier, out double services)
        {
            List<ZoneSummaryDetailed> lstZoneSummaryDetailed = new List<ZoneSummaryDetailed>();
            services = 0;
            double servicesFees = 0;
            ExecuteReaderSP("Analytics.SP_BillingRep_GetZoneSummaryDetailed", (reader) =>
            {
                while (reader.Read())
                {
                    lstZoneSummaryDetailed.Add(new ZoneSummaryDetailed
                    {
                        Zone = reader["Zone"] as string,
                        ZoneId = GetReaderValue<int>(reader, "ZoneId"),
                        Calls = GetReaderValue<int>(reader, "Calls"),
                        DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                        DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                        Rate = GetReaderValue<double>(reader, "Rate"),
                        Net = GetReaderValue<double>(reader, "Net"),
                        OffPeakDurationInSeconds = GetReaderValue<decimal>(reader, "OffPeakDurationInSeconds"),
                        OffPeakRate = GetReaderValue<double>(reader, "OffPeakRate"),
                        OffPeakNet = GetReaderValue<double>(reader, "OffPeakNet"),
                        WeekEndDurationInSeconds = GetReaderValue<decimal>(reader, "WeekEndDurationInSeconds"),
                        WeekEndRate = GetReaderValue<double>(reader, "WeekEndRate"),
                        WeekEndNet = GetReaderValue<double>(reader, "WeekEndNet"),
                        Discount = GetReaderValue<double>(reader, "Discount"),
                        CommissionValue = GetReaderValue<double>(reader, "CommissionValue"),
                        ExtraChargeValue = GetReaderValue<double>(reader, "ExtraChargeValue"),
                        SupplierID = (groupBySupplier == true ? reader["SupplierID"] as string : null)
                    });
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        servicesFees = GetReaderValue<double>(reader, "Services");
                    }
                }
            },
               fromDate,
               toDate,
               (customerId == null || customerId == "") ? null : customerId,
               (supplierId == null || supplierId == "") ? null : supplierId,
               isCost,
               currencyId,
               (supplierGroup == null || supplierGroup == "") ? null : supplierGroup,
               (customerGroup == null || customerGroup == "") ? null : customerGroup,
               (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
               (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
               groupBySupplier);
            services = servicesFees;
            return lstZoneSummaryDetailed;
        }
        public List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {
            string query = String.Format(@"SELECT
                            Convert(varchar(7), BS.CallDate,121) AS [Month] , 
                            IsNull(SUM(BS.SaleDuration) / 60.0,0) AS Durations ,
                            IsNull(SUM({0}),0) AS Amount 
                            From Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_{1})) ,
                            Zone z (NOLOCK)
                            WHERE BS.CallDate BETWEEN @FromDate AND @ToDate
                            AND {2} = @CarrierAccountID
                            AND z.ZoneID = {3}
                            GROUP BY   Convert(varchar(7), BS.CallDate,121)
                            ORDER BY   Convert(varchar(7), BS.CallDate,121) DESC ",

                            isSale ? "BS.Sale_Nets / dbo.GetExchangeRate(BS.Sale_Currency, BS.CallDate) " : " BS.Cost_Nets / dbo.GetExchangeRate(BS.Sale_Currency, BS.CallDate) ",

                            isSale ? "Customer" : "Supplier",

                            isSale ? "BS.CustomerID" : "BS.SupplierID",

                            isSale ? "BS.SaleZoneID " : "BS.CostZoneID");

            return GetItemsText(query, MonthMapper,
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month))));
                cmd.Parameters.Add(new SqlParameter("@CarrierAccountID", carrierAccountID));

            });

        }
        public List<CarrierProfileReport> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount)
        {
            int DaysInTillDays = DateTime.DaysInMonth(toDate.Year, toDate.Month);

            TimeSpan span = toDate.Subtract(fromDate);

            int NumberOfMonths = (int)(span.TotalDays / 30);

            string DurationField = "BS.SaleDuration / 60.0 ";
            string AmountField = isSale ? "BS.Sale_Nets / dbo.GetExchangeRate(BS.Sale_Currency, BS.CallDate)  " : "BS.Cost_Nets / dbo.GetExchangeRate(BS.Sale_Currency, BS.CallDate)";


            string query = String.Format(@" Set RowCount @TopDestinations
                            SELECT z.Name as Zone,
                            CalldateYear = YEAR(BS.Calldate),
                            CalldateMonth = MONTH(BS.Calldate)  ,
                            IsNull(SUM({0}),0) as {4}                         
                            From Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_{1})) ,
                            Zone z (NOLOCK)
                            WHERE BS.CallDate BETWEEN @FromDate AND @ToDate
                            AND {2} = @CarrierAccountID
                            AND z.ZoneID = {3}
                            GROUP BY   z.Name ,YEAR(BS.Calldate) , MONTH(BS.Calldate)
                            ORDER BY   z.Name DESC, CalldateYear, CalldateMonth
                            Set RowCount 0 ",

                            IsAmount ? AmountField : DurationField,

                            isSale ? "Customer" : "Supplier",

                            isSale ? "BS.CustomerID" : "BS.SupplierID",

                            isSale ? "BS.SaleZoneID " : "BS.CostZoneID",
                            IsAmount ? "Amount" : "Durations"
                            );

            return GetItemsText(query, (reader) => CarrierProfileMapper(reader, IsAmount),
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month))));
                cmd.Parameters.Add(new SqlParameter("@CarrierAccountID", carrierAccountID));
                cmd.Parameters.Add(new SqlParameter("@TopDestinations", TopDestinations));

            });

        }
        public List<CarrierLost> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, int? supplierAMUId, int? customerAMUId)
        {
            return GetItemsSP("Analytics.SP_BillingRep_GetCarrierLostReport", CarrierLostMapper,
            fromDate,
            toDate,
            (customerId == null || customerId == "") ? null : customerId,
            (supplierId == null || supplierId == "") ? null : supplierId,
            margin,
            (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
            (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
            );

        }
        public List<CarrierSummaryDaily> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_billingRep_GetDailyCarrierSummary", CarrierSummaryDailyMapper,
                 fromDate,
                 toDate,
                 (customerId == null || customerId == "") ? null : customerId,
                 (supplierId == null || supplierId == "") ? null : supplierId,
                 isCost,
                 isGroupedByDay,
                 (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
                 (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
             );
        }
        public List<VariationReports> GetVariationReportsData(List<TimeRange> timeRange, VariationReportOptions variationReportOptions, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy, out int totalCount, out  List<decimal> totalValues, out List<DateTime> datetotalValues, out decimal TotalAverage)
        {
            List<VariationReports> variationReportList = new List<VariationReports>();
            DataTable timeRangeDataTable = new DataTable();
            timeRangeDataTable = ToDataTable(timeRange);
            DateTime beginTime = (from d in timeRange select d.FromDate).Min();
            DateTime endTime = (from d in timeRange select d.ToDate).Max();
            string selectedReportQuery = GetVariationReportQuery(timeRange, variationReportOptions, entityType, groupingBy);
            int totalCount_Internal = 0;
            List<decimal> totalValues_Internal = new List<decimal>();
            List<DateTime> datetotalValues_Internal = new List<DateTime>();
            decimal TotalAverage_Internal = 0;
            if (!string.IsNullOrEmpty(selectedReportQuery))
                ExecuteReaderText(selectedReportQuery,
                (reader) =>
                {
                    while (reader.Read())
                        variationReportList.Add(VariationReportsMapper(reader));
                    reader.NextResult();
                    reader.Read();
                    totalCount_Internal = reader["TotalCount"] != DBNull.Value ? (int)reader["TotalCount"] : 0;
                    reader.NextResult();
                    while (reader.Read())
                    {
                        totalValues_Internal.Add(reader["TotalValues"] != DBNull.Value ? Convert.ToDecimal(reader["TotalValues"]) : 0);
                        datetotalValues_Internal.Add(GetReaderValue<DateTime>(reader, "FromDate"));
                    }
                    reader.NextResult();
                    reader.Read();
                    TotalAverage_Internal = reader["TotalAverage"] != DBNull.Value ? Convert.ToDecimal(reader["TotalAverage"]) : 0;

                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@timeRange", SqlDbType.Structured);
                    dtPrm.TypeName = "Analytics.TimeRangeType";
                    dtPrm.Value = timeRangeDataTable;
                    cmd.Parameters.Add(dtPrm);
                    cmd.Parameters.Add(new SqlParameter("@BeginTime", beginTime));
                    cmd.Parameters.Add(new SqlParameter("@EndTime", endTime));
                    cmd.Parameters.Add(new SqlParameter("@FromRow", fromRow));
                    cmd.Parameters.Add(new SqlParameter("@ToRow", toRow));
                    if (entityID != null)
                        cmd.Parameters.Add(new SqlParameter("@EntityID", entityID));
                    else cmd.Parameters.Add(new SqlParameter("@EntityID", string.Empty));
                });
            totalCount = totalCount_Internal;
            totalValues = totalValues_Internal;
            datetotalValues = datetotalValues_Internal;
            TotalAverage = TotalAverage_Internal;
            return variationReportList;


        }
        public List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string zoneId, int attempts, VolumeReportsTimePeriod timePeriod)
        {
            List<VolumeTraffic> resultList = GetItemsSP("Analytics.SP_BillingStats_GetTrafficVolumes", (reader) => VolumeTrafficMapper(reader, timePeriod),
             fromDate,
             toDate,
             customerId,
             supplierId,
             zoneId,
             attempts,
             timePeriod );
            return resultList;
        }

        public List<DestinationVolumeTraffic> GetDestinationTrafficVolumes(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string zoneId, int attempts, VolumeReportsTimePeriod timePeriod, int topDestination) 
        {
            //Get List Of Top Destination Duration 
           return GetDestinationTrafficVolumesQuery(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod, topDestination, true);
           
            ////Get List Of Top Destination Attempts 
            //GetDestinationTrafficVolumesQuery(fromDate, toDate, customerId, supplierId, zoneId, attempts, timePeriod, topDestination, 0);
        }
        public List<DailySummary> GetDailySummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_BillingRep_GetDailySummary", DailySummaryMapper,
              fromDate,
              toDate,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
              );
        }
        public List<RateLoss> GetRateLoss(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? zoneId, int? customerAMUId, int? supplierAMUId)
        {

            return GetItemsSP("Analytics.SP_BillingRep_GetRateLoss", RateLossMapper,
              fromDate,
              toDate,
              (customerId == null || customerId == "") ? null : customerId,
              (supplierId == null || supplierId == "") ? null : supplierId,
              (zoneId == null || zoneId == 0) ? (object)DBNull.Value : zoneId,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
              );
        }
        public List<CarrierSummary> GetCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_BillingRep_GetCarrierSummary", CarrierSummaryMapper,
              fromDate,
              toDate,
              (customerId == null || customerId == "") ? null : customerId,
              (supplierId == null || supplierId == "") ? null : supplierId,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId
              );
        }
        public List<DetailedCarrierSummary> GetCarrierDetailedSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_BillingRep_GetDetailedCarrierSummary", CarrierDetailedSummaryMapper,
              fromDate,
              toDate,
              (customerId == null || customerId == "") ? null : customerId,
              (supplierId == null || supplierId == "") ? null : supplierId,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId
              );
        }
        public List<DailyForcasting> GetDailyForcasting(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {

            return GetItemsSP("Analytics.SP_BillingRep_GetDailySummaryForcasting", DailyForcastingMapper,
              fromDate,
              toDate,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId);
        }
        public List<ExchangeCarriers> GetExchangeCarriers(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {

            return GetItemsSP("Analytics.SP_BillingRep_GetExchangeCarriersSummary", ExchangeCarriersMapper,
           fromDate,
           toDate,
           (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
           (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId);
        }

        #region PrivatMethods
        private ZoneProfit ZoneProfitMapper(IDataReader reader, bool groupByCustomer)
        {
            ZoneProfit instance = new ZoneProfit
            {
                CostZone = reader["CostZone"] as string,
                SaleZone = reader["SaleZone"] as string,
                SupplierID = reader["SupplierID"] as string,
                Calls = GetReaderValue<int>(reader, "Calls"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet")
            };
            if (groupByCustomer == true)
            {
                instance.CustomerID = reader["CustomerID"] as string;

            }
            return instance;
        }
        private CarrierProfileReport CarrierProfileMapper(IDataReader reader, bool IsAmount)
        {
            CarrierProfileReport instance = new CarrierProfileReport
            {
                Zone = reader["Zone"] as string,

                MonthYear = GetReaderValue<int>(reader, "CalldateMonth").ToString() + " " + reader["CalldateYear"] as string,

                Month = GetReaderValue<int>(reader, "CalldateMonth"),
            };
            if (IsAmount == true)
            {
                instance.Amount = GetReaderValue<double>(reader, "Amount");

            }
            else
            {
                instance.Durations = GetReaderValue<decimal>(reader, "Durations");
            }
            return instance;
        }
        private ZoneSummary ZoneSummaryMapper(IDataReader reader, bool groupBySupplier)
        {
            ZoneSummary instance = new ZoneSummary
            {
                Zone = reader["Zone"] as string,
                Calls = GetReaderValue<int>(reader, "Calls"),
                Rate = GetReaderValue<double>(reader, "Rate"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                RateType = GetReaderValue<byte>(reader, "RateType"),
                DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                Net = GetReaderValue<double>(reader, "Net"),
                CommissionValue = GetReaderValue<double>(reader, "CommissionValue"),
                ExtraChargeValue = GetReaderValue<double>(reader, "ExtraChargeValue")
            };
            if (groupBySupplier == true)
            {
                instance.SupplierID = reader["SupplierID"] as string;
            }
            return instance;
        }
        private ZoneSummaryDetailed ZoneSummaryDetailedMapper(IDataReader reader, bool groupBySupplier)
        {
            ZoneSummaryDetailed instance = new ZoneSummaryDetailed
            {
                Zone = reader["Zone"] as string,
                ZoneId = GetReaderValue<int>(reader, "ZoneId"),
                Calls = GetReaderValue<int>(reader, "Calls"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                DurationInSeconds = GetReaderValue<decimal>(reader, "DurationInSeconds"),
                Rate = GetReaderValue<double>(reader, "Rate"),
                Net = GetReaderValue<double>(reader, "Net"),
                OffPeakDurationInSeconds = GetReaderValue<decimal>(reader, "OffPeakDurationInSeconds"),
                OffPeakRate = GetReaderValue<double>(reader, "OffPeakRate"),
                OffPeakNet = GetReaderValue<double>(reader, "OffPeakNet"),
                WeekEndDurationInSeconds = GetReaderValue<decimal>(reader, "WeekEndDurationInSeconds"),
                WeekEndRate = GetReaderValue<double>(reader, "WeekEndRate"),
                WeekEndNet = GetReaderValue<double>(reader, "WeekEndNet"),
                Discount = GetReaderValue<double>(reader, "Discount"),
                CommissionValue = GetReaderValue<double>(reader, "CommissionValue"),
                ExtraChargeValue = GetReaderValue<double>(reader, "ExtraChargeValue")
            };
            if (groupBySupplier == true)
            {
                instance.SupplierID = reader["SupplierID"] as string;
            }
            return instance;
        }
        private DailySummary DailySummaryMapper(IDataReader reader)
        {
            DailySummary instance = new DailySummary
            {
                Day = reader["Day"] as string,
                Calls = GetReaderValue<int>(reader, "Calls"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostNet = GetReaderValue<double>(reader, "CostNet")
            };
            return instance;
        }
        private CarrierLost CarrierLostMapper(IDataReader reader)
        {
            CarrierLost instance = new CarrierLost
            {
                CustomerID = reader["CustomerID"] as string,
                SupplierID = reader["SupplierID"] as string,
                CostZoneID = GetReaderValue<int>(reader, "CostZoneID"),
                SaleZoneID = GetReaderValue<int>(reader, "SaleZoneID"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostNet = GetReaderValue<double>(reader, "CostNet")

            };

            return instance;
        }
        private MonthTraffic MonthMapper(IDataReader reader)
        {
            MonthTraffic instance = new MonthTraffic
            {
                Month = reader["Month"] as string,
                Durations = GetReaderValue<decimal>(reader, "Durations"),
                Amount = GetReaderValue<double>(reader, "Amount")
            };

            return instance;
        }
        private ZoneProfitFormatted MapZoneProfit(ZoneProfit zoneProfit)
        {
            return new ZoneProfitFormatted
            {
                CostZone = zoneProfit.CostZone,
                SaleZone = zoneProfit.SaleZone,
                SupplierID = zoneProfit.SupplierID,
                CustomerID = zoneProfit.CustomerID,
                Calls = zoneProfit.Calls,

                DurationNet = zoneProfit.DurationNet,
                DurationNetFormated = String.Format("{0:#0.00}", zoneProfit.DurationNet),

                SaleDuration = zoneProfit.SaleDuration,
                SaleDurationFormated = (zoneProfit.SaleDuration.HasValue) ? String.Format("{0:#0.00}", zoneProfit.SaleDuration) : "0.00",

                SaleNet = zoneProfit.SaleNet,
                SaleNetFormated = (zoneProfit.SaleNet.HasValue) ? String.Format("{0:#0.00}", zoneProfit.SaleNet) : "0.00",

                CostDuration = zoneProfit.CostDuration,
                CostDurationFormated = String.Format("{0:#0.0000}", zoneProfit.CostDuration),

                CostNet = zoneProfit.CostNet,
                CostNetFormated = (zoneProfit.CostNet.HasValue) ? String.Format("{0:#0.0000}", zoneProfit.CostNet) : "0.00",

                Profit = String.Format("{0:#0.00}", (zoneProfit.SaleNet - zoneProfit.CostNet)),

                ProfitPercentage = (zoneProfit.SaleNet.HasValue) ? String.Format("{0:#,##0.00%}", (1 - zoneProfit.CostNet / zoneProfit.SaleNet)) : "-100%",
            };
        }
        private List<ZoneProfitFormatted> MapZoneProfits(List<ZoneProfit> zoneProfits)
        {
            List<ZoneProfitFormatted> models = new List<ZoneProfitFormatted>();
            if (zoneProfits != null)
                foreach (var z in zoneProfits)
                {
                    models.Add(MapZoneProfit(z));
                }
            return models;
        }
        private VariationReports VariationReportsMapper(IDataReader reader)
        {

            VariationReports instance = new VariationReports
            {
                Name = reader["Name"] as string,
                PeriodTypeValueAverage = GetReaderValue<decimal>(reader, "AVG"),
                PeriodTypeValuePercentage = GetReaderValue<decimal>(reader, "%"),
                FromDate = GetReaderValue<DateTime>(reader, "FromDate"),
                ToDate = GetReaderValue<DateTime>(reader, "ToDate"),
                TotalDuration = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0,
                PreviousPeriodTypeValuePercentage = GetReaderValue<decimal>(reader, "Prev %"),
                ID = reader["ID"] != DBNull.Value ? reader["ID"].ToString() : string.Empty,
                RowNumber = reader["RowNumber"] != DBNull.Value ? (long)reader["RowNumber"] : 0
            };

            return instance;
        }
        private CarrierSummaryDaily CarrierSummaryDailyMapper(IDataReader reader)
        {

            CarrierSummaryDaily instance = new CarrierSummaryDaily
            {
                Day = reader["Day"] as string,
                CarrierID = reader["CarrierID"] as string,
                Attempts = GetReaderValue<int>(reader, "Attempts"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet"),
                Duration = GetReaderValue<decimal>(reader, "Duration"),
                Net = GetReaderValue<double>(reader, "Net")
            };

            return instance;
        }
        private RateLoss RateLossMapper(IDataReader reader)
        {
            return new RateLoss
            {

                CostZone = reader["CostZone"] as string,
                SaleZone = reader["SaleZone"] as string,
                SupplierID = reader["SupplierID"] as string,
                CustomerID = reader["CustomerID"] as string,
                SaleRate = GetReaderValue<double>(reader, "SaleRate"),
                CostRate = GetReaderValue<double>(reader, "CostRate"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                SaleZoneID = GetReaderValue<int>(reader, "SaleZoneID")
            };
        }
        private CarrierSummary CarrierSummaryMapper(IDataReader reader)
        {
            return new CarrierSummary
            {

                SupplierID = reader["SupplierID"] as string,
                CustomerID = reader["CustomerID"] as string,
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostCommissionValue = GetReaderValue<double>(reader, "CostCommissionValue"),
                SaleCommissionValue = GetReaderValue<double>(reader, "SaleCommissionValue"),
                CostExtraChargeValue = GetReaderValue<double>(reader, "CostExtraChargeValue"),
                SaleExtraChargeValue = GetReaderValue<double>(reader, "SaleExtraChargeValue")

            };


        }
        private DetailedCarrierSummary CarrierDetailedSummaryMapper(IDataReader reader)
        {
            return new DetailedCarrierSummary
            {
                CustomerID = reader["CustomerID"] as string,
                SaleZoneID = GetReaderValue<int>(reader, "SaleZoneID"),
                SaleZoneName = reader["SaleZoneName"] as string,
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleRate = GetReaderValue<double>(reader, "SaleRate"),
                SaleRateChange = GetReaderValue<Int16>(reader, "SaleRateChange"),
                SaleRateEffectiveDate = GetReaderValue<DateTime>(reader, "SaleRateEffectiveDate"),
                SaleAmount = GetReaderValue<double>(reader, "SaleAmount"),
                SupplierID = reader["SupplierID"] as string,
                CostZoneID = GetReaderValue<int>(reader, "CostZoneID"),
                CostZoneName = reader["CostZoneName"] as string,
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                CostRate = GetReaderValue<double>(reader, "CostRate"),
                CostRateChange = GetReaderValue<Int16>(reader, "CostRateChange"),
                CostRateEffectiveDate = GetReaderValue<DateTime>(reader, "CostRateEffectiveDate"),
                CostAmount = GetReaderValue<double>(reader, "CostAmount"),
                Profit = GetReaderValue<double>(reader, "Profit")

            };

        }
        private DailyForcasting DailyForcastingMapper(IDataReader reader)
        {
            return new DailyForcasting
            {
                Day = reader["Day"] as string,
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet")

            };

        }
        private ExchangeCarriers ExchangeCarriersMapper(IDataReader reader)
        {
            return new ExchangeCarriers
            {
                CustomerID = reader["CustomerID"] as string,
                CustomerProfit = GetReaderValue<double>(reader, "CustomerProfit"),
                SupplierProfit = GetReaderValue<double>(reader, "SupplierProfit")

            };
        }
        private string GetVariationReportQuery(List<TimeRange> timeRange, VariationReportOptions variationReportOptions, EntityType entityType, GroupingBy groupingBy)
        {

            StringBuilder query = new StringBuilder(@"DECLARE @ExchangeRates TABLE(
		                                             Currency VARCHAR(3),
		                                             Date SMALLDATETIME,
		                                             Rate FLOAT
		                                             PRIMARY KEY(Currency, Date))
                                            INSERT INTO @ExchangeRates 
                                            SELECT * FROM dbo.GetDailyExchangeRates(@BeginTime,@EndTime)");
            query.Append(@" ;
            SELECT #IDColumn# AS ID, #NameColumn# AS Name, ROW_NUMBER()  OVER ( ORDER BY #ValueColumn# DESC) AS RowNumber
            INTO #OrderedEntities
            FROM 
            Billing_Stats BS
            LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate        
            #JoinStatement#           
            WHERE BS.CallDate >= @BeginTime AND BS.CallDate < @EndTime  #WhereStatement# #BillingStatsFilter# 
            GROUP BY #IDColumn#, #NameColumn#,ERC.Rate,ERS.Rate;
            
           
            WITH FilteredEntities AS (SELECT * FROM #OrderedEntities WHERE @ToRow=0 OR RowNumber BETWEEN @FromRow AND @ToRow)

            SELECT  Ent.Name , Ent.RowNumber,
            0.0 as [AVG],
            0.0 as [%], 
            0.0 as [Prev %],
            tr.FromDate as FromDate,
            tr.ToDate as ToDate,
            (#ValueColumn#) as Total,
            Ent.ID as ID
            INTO #Results
            From @timeRange tr
            LEFT JOIN Billing_Stats BS ON BS.CallDate >= tr.FromDate AND BS.CallDate < tr.ToDate
            LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate			
            LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate        
            JOIN FilteredEntities Ent ON Ent.ID = #BSIDColumn#
            #SecondBillingStatsFilter#
            GROUP BY Ent.ID, Ent.Name, Ent.rowNumber, tr.FromDate, tr.ToDate, ERC.Rate,ERS.Rate #additionalStatement#    

            SELECT * FROM #Results

            DECLARE @TotalCount int
            SELECT  @TotalCount= (SELECT COUNT(*) as TotalCount FROM @timeRange tr)
            SELECT  @TotalCount as TotalCount

             DECLARE @TotalsTables table(TotalValues numeric(10,2),FromDate Datetime )
             INSERT INTO @TotalsTables SELECT SUM(Total) as TotalValues ,FromDate From #Results GROUP BY FromDate Order By FromDate desc
             SELECT TotalValues,FromDate FROM @TotalsTables

             SELECT SUM(TotalValues)/@TotalCount as TotalAverage fROM @TotalsTables
             ");
            switch (variationReportOptions)
            {
                case VariationReportOptions.InBoundMinutes:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM(BS.SaleDuration)/60 ");
                    if (entityType == EntityType.none)
                    {

                        query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                        query.Replace("#BSIDColumn#", "BS.CustomerID");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                            JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        query.Replace("#additionalStatement#", "");
                    }

                    else
                    {
                        if (groupingBy == GroupingBy.Customers)
                        {

                            query.Replace("#IDColumn#", " BS.CustomerID ");
                            query.Replace("#BSIDColumn#", " BS.CustomerID ");
                            query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                                JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        }
                        if (groupingBy == GroupingBy.Suppliers)
                        {

                            query.Replace("#IDColumn#", " BS.SupplierID ");
                            query.Replace("#BSIDColumn#", " BS.SupplierID ");
                            query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.SupplierID
                                                                JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        }
                    }

                    query.Replace("#WhereStatement#", " ");
                    break;

                case VariationReportOptions.OutBoundMinutes:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(CostDuration)/60 ");
                    query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                    query.Replace("#BSIDColumn#", "BS.SupplierID");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.SupplierID
                                                            JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    query.Replace("#WhereStatement#", @" AND cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundMinutes:

                    break;

                case VariationReportOptions.TopDestinationMinutes:
                    query.Replace("#NameColumn#", " Z.Name ");
                    query.Replace("#ValueColumn#", " SUM(SaleDuration)/60 ");
                    query.Replace("#IDColumn#", " Z.ZoneID ");
                    query.Replace("#BSIDColumn#", "BS.SaleZoneID");
                    query.Replace("#JoinStatement#", @" JOIN Zone Z With(Nolock) ON Z.ZoneID=BS.SaleZoneID ");
                    query.Replace("#WhereStatement#", @" ");
                    if (entityType == EntityType.Customer)
                    {
                        query.Replace("#additionalStatement#", " , BS.CustomerID");
                    }
                    else if (entityType == EntityType.Supplier)
                    {
                        query.Replace("#additionalStatement#", " , BS.SupplierID");
                    }
                    break;

                case VariationReportOptions.InBoundAmount:
                    query.Replace("#NameColumn#", " cpc.Name ");

                    if (entityType == EntityType.none)
                    {

                        query.Replace("#ValueColumn#", " SUM(Sale_Nets)/ ISNULL(ERS.Rate,1) ");
                        query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                        query.Replace("#BSIDColumn#", "BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                            JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    }
                    else
                    {
                        if (groupingBy == GroupingBy.Customers)
                        {
                            query.Replace("#ValueColumn#", " SUM(Cost_Nets)/ ISNULL(ERS.Rate,1) ");
                            query.Replace("#IDColumn#", " BS.CustomerID ");
                            query.Replace("#BSIDColumn#", "BS.CustomerID ");
                            query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                                JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        }
                        else if (groupingBy == GroupingBy.Suppliers)
                        {
                            query.Replace("#ValueColumn#", " SUM(Cost_Nets)/ ISNULL(ERS.Rate,1) ");
                            query.Replace("#IDColumn#", " BS.SupplierID ");
                            query.Replace("#BSIDColumn#", " BS.SupplierID ");
                            query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.SupplierID
                                                                JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        }
                    }
                    query.Replace("#WhereStatement#", @" ");
                    break;

                case VariationReportOptions.OutBoundAmount:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(Cost_Nets)/ISNULL(ERC.Rate,1) ");
                    query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                    query.Replace("#BSIDColumn#", " BS.SupplierID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.SupplierID
                                                            JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    query.Replace("#WhereStatement#", @" AND cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundAmount:
                    break;

                case VariationReportOptions.TopDestinationAmount:
                    query.Replace("#NameColumn#", " Z.Name ");
                    query.Replace("#IDColumn#", " Z.ZoneID ");
                    query.Replace("#BSIDColumn#", "BS.SaleZoneID");
                    query.Replace("#JoinStatement#", @" JOIN Zone Z With(Nolock) ON Z.ZoneID=BS.SaleZoneID ");

                    if (entityType == EntityType.none)
                    {
                        query.Replace("#ValueColumn#", " SUM(BS.Sale_Nets)/ISNULL(ERS.Rate,1) ");

                    }
                    else if (entityType == EntityType.Customer)
                    {
                        query.Replace("#additionalStatement#", " , BS.CustomerID");
                        query.Replace("#ValueColumn#", " SUM(BS.Sale_Nets)/ISNULL(ERS.Rate,1) ");

                    }
                    else if (entityType == EntityType.Supplier)
                    {
                        query.Replace("#additionalStatement#", " , BS.SupplierID");
                        query.Replace("#ValueColumn#", " SUM(BS.Cost_Nets)/ISNULL(ERS.Rate,1) ");
                    }
                    else if (entityType == EntityType.Profit)
                    {
                        query.Replace("#additionalStatement#", " , BS.CustomerID");
                        query.Replace("#ValueColumn#", " SUM( (Sale_Nets/ISNULL(ERS.Rate,1)) - (Cost_Nets/ISNULL(ERC.Rate,1)) ) ");
                    }
                    query.Replace("#WhereStatement#", @" ");
                    break;

                case VariationReportOptions.Profit:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM( (Sale_Nets/ISNULL(ERS.Rate,1)) - (Cost_Nets/ISNULL(ERC.Rate,1)) ) ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                    if (entityType == EntityType.none)
                    {
                        query.Replace("#BSIDColumn#", " BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    }
                    else
                    {
                        if (groupingBy == GroupingBy.Customers)
                        {
                            query.Replace("#BSIDColumn#", " BS.CustomerID ");
                            query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        }
                        if (groupingBy == GroupingBy.Suppliers)
                        {
                            query.Replace("#BSIDColumn#", " BS.SupplierID ");
                            query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.SupplierID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                        }
                    }
                    query.Replace("#WhereStatement#", @" ");
                    break;

            }
            StringBuilder billingStatsFilterBuilder = new StringBuilder();
            StringBuilder SecondBillingStatsFilter = new StringBuilder();
            switch (entityType)
            {
                case EntityType.Zone:
                    query.Replace("#SecondBillingStatsFilter#", " WHERE BS.SaleZoneID = @EntityID ");
                    billingStatsFilterBuilder.Append(" AND BS.SaleZoneID = @EntityID");
                    switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers: query.Replace("#additionalStatement#", " ,BS.SupplierID "); break;
                        default: query.Replace("#additionalStatement#", " "); break;
                    }
                    break;

                case EntityType.Customer:
                    query.Replace("#SecondBillingStatsFilter#", " WHERE BS.CustomerID = @EntityID ");
                    billingStatsFilterBuilder.Append(" AND BS.CustomerID = @EntityID");
                    switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers: query.Replace("#additionalStatement#", " ,BS.SupplierID "); break;
                        default: query.Replace("#additionalStatement#", " "); break;
                    }
                    break;
                case EntityType.Supplier:
                    query.Replace("#SecondBillingStatsFilter#", " WHERE BS.SupplierID = @EntityID ");
                    billingStatsFilterBuilder.Append(" AND BS.SupplierID = @EntityID");
                    switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers: query.Replace("#additionalStatement#", " ,BS.SupplierID "); break;
                        default: query.Replace("#additionalStatement#", " "); break;
                    }
                    break;
                case EntityType.Profit:
                    query.Replace("#SecondBillingStatsFilter#", " WHERE BS.CustomerID = @EntityID ");
                    billingStatsFilterBuilder.Append(" AND BS.CustomerID = @EntityID");
                    switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers: query.Replace("#additionalStatement#", " ,BS.SupplierID "); break;
                        default: query.Replace("#additionalStatement#", " "); break;
                    }
                    break;

                default:
                    query.Replace("#SecondBillingStatsFilter#", " ");
                    query.Replace("#additionalStatement#", " ");
                    break;

            }
            if (billingStatsFilterBuilder.Length > 0)
                query.Replace("#BillingStatsFilter#", billingStatsFilterBuilder.ToString());
            else
                query.Replace("#BillingStatsFilter#", "");
            if (SecondBillingStatsFilter.Length > 0)
                query.Replace("#SecondBillingStatsFilter#", SecondBillingStatsFilter.ToString());
            else
                query.Replace("#SecondBillingStatsFilter#", "");
            return query.ToString();
        }
        private VolumeTraffic VolumeTrafficMapper(IDataReader reader, VolumeReportsTimePeriod timePeriod)
        {
            VolumeTraffic volumeTraffic = new VolumeTraffic
            {
                Attempts = GetReaderValue<int>(reader, "Attempts"),
                Duration = GetReaderValue<decimal>(reader, "Duration")
            };
            switch (timePeriod)
            {
                case VolumeReportsTimePeriod.None: volumeTraffic.Date = ""; break;
                case VolumeReportsTimePeriod.Daily: volumeTraffic.Date = reader["CallDate"] as string; break;
                case VolumeReportsTimePeriod.Weekly: volumeTraffic.Date = string.Concat(((int)reader["CallWeek"]).ToString(), "/", ((int)reader["CallYear"]).ToString()); break;
                case VolumeReportsTimePeriod.Monthly: volumeTraffic.Date = string.Concat(((int)reader["CallMonth"]).ToString(), "/", ((int)reader["CallYear"]).ToString()); break;
            }
            return volumeTraffic;
        }

        private List<DestinationVolumeTraffic> GetDestinationTrafficVolumesQuery(DateTime fromDate, DateTime toDate, string customerId, string supplierId, string zoneId, int attempts, VolumeReportsTimePeriod timePeriod, int topDestinations, bool isDuration)
        {
            string durationField = " SaleDuration / 60.0 ";
            string attemptsField = " NumberOfCalls ";
            string date = fromDate.ToString();
            StringBuilder query = new StringBuilder(@" Set RowCount @TopDestinations
                            SELECT SZ.Name AS SaleZone,SZ.ZoneID AS SaleZoneID,SUM(CASE WHEN Calldate BETWEEN @FromDate AND @ToDate THEN #ValueColumn# ELSE 0 END) AS [#ValueColumn#]                         
                            From Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date #IndexesValue# ))
                                Join Zone SZ With(Nolock) On SZ.ZoneID=BS.SaleZoneID 
                            WHERE BS.CallDate BETWEEN @FromDate AND @ToDate
                                AND BS.NumberOfCalls >  @Attempts
                                #CustomerFilter#
                                #SupplierFilter#
                                #ZoneFilter#
                            GROUP BY SZ.Name ,SZ.ZoneID
                            ORDER BY SUM(BS.SaleDuration / 60.0) DESC ");
           if(isDuration)
                query.Replace("#ValueColumn#", durationField);
           else query.Replace("#ValueColumn#", attemptsField);
           if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(supplierId))
           {
               query.Replace("#IndexesValue#", " ,IX_Billing_Stats_Customer,IX_Billing_Stats_Supplier ");
               query.Replace("#CustomerFilter#", " AND BS.CustomerID=@CustomerId ");
               query.Replace("#SupplierFilter#", " AND BS.SupplierID=@SupplierId ");
           }
         //  else   query.Replace("#IndexesValue#", !string.IsNullOrEmpty(customerId) ? " ,IX_Billing_Stats_Customer " : !string.IsNullOrEmpty(supplierId) ? " ,IX_Billing_Stats_Supplier " : string.Empty);
         
           else if (!string.IsNullOrEmpty(customerId)) {
               query.Replace("#IndexesValue#", " ,IX_Billing_Stats_Customer ");
               query.Replace("#CustomerFilter#", " AND BS.CustomerID=@CustomerId ");
               query.Replace("#SupplierFilter#", " ");
           
           }
           else if (!string.IsNullOrEmpty(supplierId)) {
               query.Replace("#IndexesValue#", " ,IX_Billing_Stats_Supplier ");
               query.Replace("#CustomerFilter#", "  ");
               query.Replace("#SupplierFilter#", " AND BS.SupplierID=@SupplierId ");
           }
           else if (string.IsNullOrEmpty(customerId) && string.IsNullOrEmpty(supplierId)) {

               query.Replace("#IndexesValue#", " ");
               query.Replace("#CustomerFilter#", "  ");
               query.Replace("#SupplierFilter#", " ");
           }
          
           if (zoneId != "0")
                query.Replace("#ZoneFilter#", " AND SZ.ZoneID= @ZoneId ");
           else query.Replace("#ZoneFilter#",string.Empty);
           



            return GetItemsText(query.ToString(), (reader) => DestinationVolumeTrafficMapper(reader, isDuration, durationField, attemptsField),
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@TopDestinations", topDestinations));
                cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", toDate));
                cmd.Parameters.Add(new SqlParameter("@Attempts", attempts));
                cmd.Parameters.Add(new SqlParameter("@ZoneId", zoneId));
                if(!string.IsNullOrEmpty(supplierId)) cmd.Parameters.Add(new SqlParameter("@SupplierId", supplierId));
                if (!string.IsNullOrEmpty(customerId)) cmd.Parameters.Add(new SqlParameter("@CustomerId", customerId));
            });
        }

        private DestinationVolumeTraffic DestinationVolumeTrafficMapper(IDataReader reader,bool isDuration,string durationField,string attemptsField ) {

            DestinationVolumeTraffic destinationVolumeTraffic = new DestinationVolumeTraffic
            {
                SaleZoneName = reader["SaleZone"] as string,
                SaleZoneID = GetReaderValue<int>(reader, "SaleZoneID"),
                Values = isDuration ? GetReaderValue<decimal>(reader, durationField) : GetReaderValue<decimal>(reader, attemptsField)
            };

            return destinationVolumeTraffic;
        }
        private static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #endregion

    }
}
