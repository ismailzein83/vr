using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class AnalyticsDataManager : BaseTOneDataManager, IAnalyticsDataManager
    {
        public List<Entities.TopNDestinationView> GetTopNDestinations(DateTime fromDate, DateTime toDate, string sortOrder, string customerID, string supplierID
            , int? switchID, char groupByCodeGroup, string codeGroup, char showSupplier, string orderTarget, int from, int to, int? topCount)
        {
            return GetItemsSP("Analytics.sp_Traffic_TopNDestination", (reader) =>
            {
                return new Entities.TopNDestinationView
                {
                    OurZoneID = Convert.ToInt32(reader["OurZoneID"]),
                    ZoneName = reader["Name"] as string,
                    SupplierID = reader["SupplierID"] as string,
                    Attempts = Convert.ToInt32(reader["Attempts"]),
                    DurationInMinutes = Convert.ToDecimal(reader["DurationsInMinutes"]),
                    ASR = Convert.ToDecimal(reader["ASR"]),
                    SuccessfulAttempts = Convert.ToInt32(reader["SuccessfulAttempt"]),
                    ACD = Convert.ToDecimal(reader["ACD"]),
                    DeliveredASR = Convert.ToDecimal(reader["DeliveredASR"]),
                    AveragePDD = Convert.ToDecimal(reader["AveragePDD"]),
                    CodeGroup = reader["CodeGroupName"] as string
                };
            },
                topCount, fromDate, toDate, sortOrder, customerID, supplierID, switchID, groupByCodeGroup, codeGroup, showSupplier, orderTarget, from, to, "TopNDestinationTemp");
        }

        public List<Entities.Alert> GetAlerts(int from, int to, int? topCount, char showHiddenAlerts, int? alertLevel, string tag, string source, int? userID)
        {
            return GetItemsSP("Analytics.sp_alerts_getalerts", (reader) =>
            {
                return new Entities.Alert
                {
                    ID = Convert.ToInt32(reader["ID"]),
                    Created = Convert.ToDateTime(reader["Created"]),
                    Source = reader["Source"] as string,
                    Level = (Entities.AlertLevel)Convert.ToInt32(reader["Level"]),
                    Progress = (Entities.AlertProgress)Convert.ToInt32(reader["Progress"]),
                    Tag = reader["Tag"] as string,
                    Description = reader["Description"] as string
                };
            }, topCount, showHiddenAlerts, alertLevel, tag, source, userID, from, to);
        }

        public List<Entities.CarrierRateView> GetRates(string carrierType, DateTime effectiveOn, string carrierID, string codeGroup, string code, string zoneName, int from, int to)
        {
            return GetItemsSP("Analytics.sp_Rates_GetRates", (reader) =>
                {
                    return new Entities.CarrierRateView
                    {
                        ZoneID = Convert.ToInt32(reader["ZoneID"]),
                        CodeGroup = reader["CodeGroup"] as string,
                        ZoneName = reader["ZoneName"] as string,
                        Code = reader["Code"] as string,
                        RateID = Convert.ToInt32(reader["RateID"]),
                        FlaggedServiceID = Convert.ToInt16(reader["ServicesFlag"]),
                        Rate = reader["Rate"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["Rate"]) : null,
                        CurrencyID = reader["CurrencyID"] as string,
                        OffPeakRate = reader["OffPeakRate"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["OffPeakRate"]) : null,
                        WeekendRate = reader["WeekendRate"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["WeekendRate"]) : null,
                        ChangeID = Convert.ToInt32(reader["Change"]),
                        RateBeginEffectiveDate = reader["RateBeginEffectiveDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["RateBeginEffectiveDate"]) : null,
                        RateEndEffectiveDate = reader["RateEndEffectiveDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["RateEndEffectiveDate"]) : null,
                        CodeBeginEffectiveDate = reader["CodeBeginEffectiveDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["CodeBeginEffectiveDate"]) : null,
                        CodeEndEffectiveDate = reader["CodeEndEffectiveDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["CodeEndEffectiveDate"]) : null,
                        PricelistID = Convert.ToInt32(reader["PricelistID"]),
                        PricelistBeginEffectiveDate = reader["PricelistBeginEffectiveDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["PricelistBeginEffectiveDate"]) : null,
                        UserName = reader["UserName"] as string
                    };
                }, carrierType, effectiveOn, codeGroup, code, zoneName, carrierID, from, to);
        }

        public List<Entities.CarrierSummaryView> GetCarrierSummary(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, char groupByProfile, int? topCount, int from, int to)
        {
            return GetItemsSP("Analytics.SP_Traffic_CarrierSummary", (reader) =>
                {
                    return new Entities.CarrierSummaryView
                    {
                        ProfileID = groupByProfile == 'Y' && reader["ProfileID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["ProfileID"]) : null,
                        CarrierID = (carrierType.ToLower() == "customer" ? reader["CustomerID"] as string : reader["SupplierID"] as string),
                        CarrierName = string.Format("{0}{1}", reader["ProfileName"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"].ToString()) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty),
                        ProfileName = groupByProfile == 'Y' ? (reader["ProfileName"] != DBNull.Value ? reader["ProfileName"] as string : "") : string.Empty,
                        SuccessfulAttempts = Convert.ToInt32(reader["SuccessfulAttempts"]),
                        DurationsInMinutes = Convert.ToDecimal(reader["DurationsInMinutes"]),
                        ASR = Convert.ToDecimal(reader["ASR"]),
                        ACD = Convert.ToDecimal(reader["ACD"]),
                        DeliveredASR = Convert.ToDecimal(reader["DeliveredASR"]),
                        AveragePDD = Convert.ToDecimal(reader["AveragePDD"]),
                        NumberOfCalls = reader["NumberOfCalls"] != DBNull.Value ? (int?)Convert.ToInt32(reader["NumberOfCalls"]) : null,
                        PricedDuration = reader["PricedDuration"] != DBNull.Value ? (decimal?)Convert.ToDecimal(reader["PricedDuration"]) : null,
                        SaleNets = Convert.ToDecimal(reader["Sale_Nets"]),
                        CostNets = Convert.ToDecimal(reader["Cost_Nets"]),
                        Profit = Convert.ToDecimal(reader["Profit"]),
                        Attempts = Convert.ToInt32(reader["Attempts"])
                    };
                }, carrierType, fromDate, toDate, customerID, supplierID, topCount, groupByProfile, null, null, from, to);

        }

        public List<Entities.TopCarriersView> GetTopCustomers(DateTime fromDate, DateTime toDate, int topCount)
        {
            return GetItemsSP("Analytics.SP_Traffic_TopCustomers", (reader) =>
                {
                    return new Entities.TopCarriersView
                    {
                        CarrierID = reader["CarrierAccountID"] as string,
                        CarrierName = string.Format("{0}{1}", reader["ProfileName"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"].ToString()) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty),
                        DurationInSeconds = Convert.ToDecimal(reader["DurationsInMinutes"]),
                        NumberOfCalls = Convert.ToInt32(reader["Attempts"])
                    };
                }, fromDate, toDate, topCount);
        }

        public List<Entities.TopCarriersView> GetTopSupplier(DateTime fromDate, DateTime toDate, int topCount)
        {
            return GetItemsSP("Analytics.SP_Traffic_TopSuppliers", (reader) =>
            {
                return new Entities.TopCarriersView
                {
                    CarrierID = reader["CarrierAccountID"] as string,
                    CarrierName = string.Format("{0}{1}", reader["ProfileName"] as string, reader["NameSuffix"] != DBNull.Value && !string.IsNullOrEmpty(reader["NameSuffix"].ToString()) ? " (" + reader["NameSuffix"] as string + ")" : string.Empty),
                    DurationInSeconds = Convert.ToDecimal(reader["DurationsInMinutes"]),
                    NumberOfCalls = Convert.ToInt32(reader["Attempts"])
                };
            }, fromDate, toDate, topCount);
        }

        public List<Entities.ProfitByDay> GetLastWeeksProfit(DateTime from, DateTime to)
        {
            return GetItemsSP("Analytics.SP_Traffic_DailyProfitSummary", (reader) =>
                {
                    return new Entities.ProfitByDay
                    {
                        Day = Convert.ToDateTime(reader["day"]),
                        DayOfWeek = reader["WeekDay"] as string,
                        DayNumber = Convert.ToInt32(reader["DayNumber"]),
                        Profit = Convert.ToDecimal(reader["Profit"])
                    };
                }, from, to);
        }

        public Entities.TrafficSummaryView GetSummary(DateTime fromDate, DateTime toDate)
        {
            return GetItemSP("Analytics.SP_Traffic_Summary", (reader) =>
                {
                    return new Entities.TrafficSummaryView
                    {
                        Sales = reader["SaleNet"] != DBNull.Value ? Convert.ToDecimal(reader["SaleNet"]) : (Decimal)0,
                        Purchases = reader["CostNet"] != DBNull.Value ? Convert.ToDecimal(reader["CostNet"]) : (Decimal)0,
                        Profit = (reader["SaleNet"] != DBNull.Value ? Convert.ToDecimal(reader["SaleNet"]) : (Decimal)0) - (reader["CostNet"] != DBNull.Value ? Convert.ToDecimal(reader["CostNet"]) : (Decimal)0),
                        DurationInMinutes = reader["SaleDuration"] != DBNull.Value ? Convert.ToDecimal(reader["SaleDuration"]) : (Decimal)0,
                        NumberOfCalls = reader["Calls"] != DBNull.Value ? Convert.ToInt32(reader["Calls"]) : (Int32)0,
                        AveragePurchases = reader["AverageCostNet"] != DBNull.Value ? Convert.ToDecimal(reader["AverageCostNet"]) : (Decimal)0,
                        AverageSales = reader["AverageSaleNet"] != DBNull.Value ? Convert.ToDecimal(reader["AverageSaleNet"]) : (Decimal)0
                    };
                }, fromDate, toDate);
        }

        public bool UpdateRateServiceFlag(int rateID, int serviceIDsSummation)
        {
            int recordesEffected = ExecuteNonQuerySP("Analytics.sp_CarrierRateView_Update", rateID, serviceIDsSummation);
            return (recordesEffected > 0);
        }

    }
}
