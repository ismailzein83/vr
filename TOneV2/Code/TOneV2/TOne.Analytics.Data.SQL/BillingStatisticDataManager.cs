﻿using System;
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
            return GetItemsSP("Analytics.SP_Billing_GetZoneProfits", (reader) => ZoneProfitMapper(reader, groupByCustomer),
                fromDate,
                toDate,
                (customerId == null || customerId == "") ? null : customerId,
                (supplierId == null || supplierId == "") ? null : supplierId,
                groupByCustomer,
                (supplierAMUId == 0) ? (object)DBNull.Value : supplierAMUId,
                (customerAMUId == 0) ? (object)DBNull.Value : customerAMUId);
        }
        public List<ZoneSummary> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return GetItemsSP("Analytics.SP_Billing_GetZoneSummary", (reader) => ZoneSummaryMapper(reader, groupBySupplier),
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
               groupBySupplier
               );
        }
        public List<ZoneSummaryDetailed> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return GetItemsSP("Analytics.SP_Billing_GetZoneSummaryDetailed", (reader) => ZoneSummaryDetailedMapper(reader, groupBySupplier),
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
               groupBySupplier
               );
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
        public List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount)
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
            return GetItemsSP("Analytics.SP_Billing_CarrierLostReport", CarrierLostMapper,
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
            return GetItemsSP("Analytics.SP_billing_DailyCarrierSummary", CarrierSummaryDailyMapper,
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
                    totalCount_Internal = (int)reader["TotalCount"];
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

        public List<DailySummary> GetDailySummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_GetDailySummary", DailySummaryMapper,
              fromDate,
              toDate,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId
              );
        }
        public List<RateLoss> GetRateLoss(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? zoneId, int? customerAMUId, int? supplierAMUId)
        {

            return GetItemsSP("Analytics.SP_Billing_RateLoss", RateLossMapper,
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
            return GetItemsSP("Analytics.SP_Billing_CarrierSummary", CarrierSummaryMapper,
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
            return GetItemsSP("Analytics.SP_Billing_DetailedCarrierSummary", CarrierDetailedSummaryMapper,
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

            return GetItemsSP("Analytics.SP_Billing_DailySummaryForcasting", DailyForcastingMapper,
              fromDate,
              toDate,
              (customerAMUId == 0 || customerAMUId == null) ? (object)DBNull.Value : customerAMUId,
              (supplierAMUId == 0 || supplierAMUId == null) ? (object)DBNull.Value : supplierAMUId);
        }
        public List<ExchangeCarriers> GetExchangeCarriers(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {

            return GetItemsSP("Analytics.SP_Billing_ExchangeCarriersSummary", ExchangeCarriersMapper,
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
        private CarrierProfile CarrierProfileMapper(IDataReader reader, bool IsAmount)
        {
            CarrierProfile instance = new CarrierProfile
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
                TotalDuration = Convert.ToDecimal(reader["Total"]),
                PreviousPeriodTypeValuePercentage = GetReaderValue<decimal>(reader, "Prev %"),
                ID = reader["ID"] != null ? reader["ID"].ToString() : string.Empty,
                RowNumber = (long)reader["RowNumber"]
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
                Duration = GetReaderValue<decimal>(reader, "DurationNet"),
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
            WHERE BS.CallDate >= @BeginTime AND BS.CallDate < @EndTime  #WhereStatement#  
            GROUP BY #IDColumn#, #NameColumn#,ERC.Rate,ERS.Rate;
            
            WITH FilteredEntities AS (SELECT * FROM #OrderedEntities WHERE RowNumber BETWEEN @FromRow AND @ToRow)

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
            #BillingStatsFilter#
            GROUP BY Ent.ID, Ent.Name, Ent.rowNumber, tr.FromDate, tr.ToDate, ERC.Rate,ERS.Rate #additionalStatement#     
            ORDER BY Ent.RowNumber, tr.FromDate, tr.ToDate;

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

                        query.Replace("#IDColumn#", " BS.CustomerID ");
                        query.Replace("#BSIDColumn#", " BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    }

                    query.Replace("#WhereStatement#", " ");
                    break;

                case VariationReportOptions.OutBoundMinutes:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(CostDuration)/60 ");
                    if (entityType == EntityType.none)
                    {
                        query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                        query.Replace("#BSIDColumn#", "BS.SupplierID");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.SupplierID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    }
                    else
                    {
                        query.Replace("#IDColumn#", " BS.SupplierID ");
                        query.Replace("#BSIDColumn#", " BS.SupplierID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.SupplierID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    }
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
                    if (entityType != EntityType.none)
                    {
                        query.Replace("#additionalStatement#", " , BS.SupplierID");
                    }
                    break;

                case VariationReportOptions.InBoundAmount:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM(Sale_Nets)/ ISNULL(ERS.Rate,1) ");
                    if (entityType == EntityType.none)
                    {
                        query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                        query.Replace("#BSIDColumn#", "BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    }
                    else
                    {
                        query.Replace("#IDColumn#", " BS.CustomerID ");
                        query.Replace("#BSIDColumn#", "BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    }
                    query.Replace("#WhereStatement#", @" ");
                    break;

                case VariationReportOptions.OutBoundAmount:
                    query.Replace("#NameColumn#", " cps.Name ");
                    query.Replace("#ValueColumn#", " SUM(Cost_Nets)/ISNULL(ERC.Rate,1) ");
                    if (entityType == EntityType.none)
                    {
                        query.Replace("#IDColumn#", " cas.CarrierAccountID ");
                        query.Replace("#BSIDColumn#", " BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    }
                    else
                    {
                        query.Replace("#IDColumn#", " BS.CustomerID ");
                        query.Replace("#BSIDColumn#", "BS.CustomerID ");
                        query.Replace("#JoinStatement#", @" JOIN CarrierAccount cas With(Nolock) ON cas.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cps With(Nolock) ON cps.ProfileID = cas.ProfileID ");
                    }
                    query.Replace("#WhereStatement#", @" AND cas.RepresentsASwitch <> 'Y' ");
                    break;

                case VariationReportOptions.InOutBoundAmount:
                    break;

                case VariationReportOptions.TopDestinationAmount:
                    query.Replace("#NameColumn#", " Z.Name ");
                    query.Replace("#ValueColumn#", " SUM(BS.Sale_Nets)/ISNULL(ERS.Rate,1) ");
                    query.Replace("#IDColumn#", " Z.ZoneID ");
                    query.Replace("#BSIDColumn#", "BS.SaleZoneID");
                    query.Replace("#JoinStatement#", @" JOIN Zone Z With(Nolock) ON Z.ZoneID=BS.SaleZoneID ");
                    query.Replace("#WhereStatement#", @" ");
                    break;

                case VariationReportOptions.Profit:
                    query.Replace("#NameColumn#", " cpc.Name ");
                    query.Replace("#ValueColumn#", " SUM( (Sale_Nets/ISNULL(ERS.Rate,1)) - (Cost_Nets/ISNULL(ERC.Rate,1)) ) ");
                    query.Replace("#IDColumn#", " cac.CarrierAccountID ");
                    query.Replace("#BSIDColumn#", " BS.CustomerID ");
                    query.Replace("#JoinStatement#", @" JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                        JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID ");
                    query.Replace("#WhereStatement#", @" ");
                    break;

            }
            StringBuilder billingStatsFilterBuilder = new StringBuilder();
            switch (entityType)
            {
                case EntityType.Zone:
                    query.Replace("#SecondWhereStatement#", " AND BS.SaleZoneID = @EntityID ");
                    billingStatsFilterBuilder.Append(" WHERE BS.SaleZoneID = @EntityID");
                    switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers:
                            query.Replace("#additionalStatement#", " ,BS.SupplierID ");
                            break;
                        default: query.Replace("#additionalStatement#", " ");
                            break;
                    }
                    break;
                case EntityType.Customer:
                    query.Replace("#SecondWhereStatement#", " AND BS.CustomerID = @EntityID ");
                    billingStatsFilterBuilder.Append(" WHERE BS.CustomerID = @EntityID");
                    //query.Replace("#additionalStatement#", " ,BS.CustomerID ");
                     switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers:
                            query.Replace("#additionalStatement#", " ,BS.SupplierID ");
                            break;
                        default: query.Replace("#additionalStatement#", " ");
                            break;
                    }
                   
                    break;
                case EntityType.Supplier:
                    query.Replace("#SecondWhereStatement#", " AND BS.SupplierID = @EntityID ");
                    billingStatsFilterBuilder.Append(" WHERE BS.SupplierID = @EntityID");
                    //query.Replace("#additionalStatement#", " ,BS.SupplierID ");
                     switch (groupingBy)
                    {
                        case GroupingBy.Customers: query.Replace("#additionalStatement#", " ,BS.CustomerID "); break;
                        case GroupingBy.Suppliers:
                            query.Replace("#additionalStatement#", " ,BS.SupplierID ");
                            break;
                        default: query.Replace("#additionalStatement#", " ");
                            break;
                    }
                     break;
                default:
                    query.Replace("#SecondWhereStatement#", " ");
                    query.Replace("#additionalStatement#", " ");
                    break;

            }
            if (billingStatsFilterBuilder.Length > 0)
                query.Replace("#BillingStatsFilter#", billingStatsFilterBuilder.ToString());
            else
                query.Replace("#BillingStatsFilter#", "");
            return query.ToString();
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
