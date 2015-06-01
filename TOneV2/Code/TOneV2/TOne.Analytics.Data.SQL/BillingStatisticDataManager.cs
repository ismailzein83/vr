using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    class BillingStatisticDataManager : BaseTOneDataManager, IBillingStatisticDataManager
    {
        public List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string groupByCustomer)
        {
            return GetItemsSP("Analytics.sp_billing_GetZoneProfits", (reader) => ZoneProfitMapper(reader, groupByCustomer), fromDate, toDate, null, null, groupByCustomer, null, null);
        }
        public List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {
            int daysInTillDays = DateTime.DaysInMonth(toDate.Year, toDate.Month);

            int numberOfMonths = toDate.Month - fromDate.Month + 1;

            string query = String.Format(@"SELECT
                            Convert(varchar(7), BS.CallDate,121) AS [Month] , 
                            IsNull(SUM(BS.SaleDuration) / 60.0,0) AS Durations ,
                            IsNull(SUM({0}),0) AS Amount 
                            From Billing_Stats BS WITH(NOLOCK,INDEX(IX_Billing_Stats_Date,IX_Billing_Stats_{1})) ,
                            Zone z (NOLOCK)
                            WHERE BS.CallDate BETWEEN @FromDate AND '{2}' 
                            AND {3} = '{4}' 
                            AND z.ZoneID = {5}
                            GROUP BY   Convert(varchar(7), BS.CallDate,121)
                            ORDER BY   Convert(varchar(7), BS.CallDate,121) DESC ",

                            isSale ? "BS.Sale_Nets / dbo.GetExchangeRate(BS.Sale_Currency, BS.CallDate) " : " BS.Cost_Nets / dbo.GetExchangeRate(BS.Sale_Currency, BS.CallDate) ",
                           
                            isSale ? "Customer" : "Supplier",

                            toDate.Date.ToString("yyyy-MM-") + daysInTillDays.ToString(),
                            
                            isSale ? "BS.CustomerID" : "BS.SupplierID",

                            carrierAccountID,

                            isSale ? "BS.SaleZoneID " : "BS.CostZoneID");

                            return GetItemsText(query, MonthMapper,
                            (cmd) =>
                            {
                                cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                            });
           
        }


        private ZoneProfit ZoneProfitMapper(IDataReader reader, string groupByCustomer)
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
            if (groupByCustomer == "Y")
            {
                instance.CustomerID = reader["CustomerID"] as string;

            }
            return instance;
        }

        private MonthTraffic MonthMapper(IDataReader reader)
        {
            MonthTraffic instance = new MonthTraffic
            {
                Month = reader["Month"] as string,
                Durations = GetReaderValue<double>(reader, "Durations"),
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

        public List<BillingStatistic> GetBillingStatistics(DateTime fromDate, DateTime toDate)
        {
            string query = String.Format(@"SELECT TOP 100 * FROM Billing_Stats Where CallDate Between @FromDate AND @ToDate");

            return GetItemsText(query, BillingStatisticsMapper,
            (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@FromDate", fromDate));
                cmd.Parameters.Add(new SqlParameter("@ToDate", toDate));
            });
        }

        private BillingStatistic BillingStatisticsMapper(IDataReader reader)
        {

            BillingStatistic instance = new BillingStatistic
            {
                CallDate = GetReaderValue<DateTime>(reader, "CallDate"),
                CustomerID = reader["CustomerID"] as string,
                SupplierID = reader["SupplierID"] as string,
                CostZoneID = GetReaderValue<int>(reader, "CostZoneID"),
                SaleZoneID = GetReaderValue<int>(reader, "SaleZoneID"),
                CostCurrency = reader["Cost_Currency"] as string,
                SaleCurrency = reader["Sale_Currency"] as string,
                NumberOfCalls = GetReaderValue<int>(reader, "CostZonNumberOfCallseID"),
                FirstCallTime = GetReaderValue<TimeSpan>(reader, "FirstCallTime"),
                LastCallTime = GetReaderValue<TimeSpan>(reader, "LastCallTime"),
                MinDuration = GetReaderValue<decimal>(reader, "MinDuration"),
                MaxDuration = GetReaderValue<decimal>(reader, "MaxDuration"),
                AvgDuration = GetReaderValue<int>(reader, "AvgDuration"),
                CostNets = GetReaderValue<decimal>(reader, "Cost_Nets"),
                CostDiscounts = GetReaderValue<decimal>(reader, "Cost_Discounts"),
                CostCommissions = GetReaderValue<decimal>(reader, "Cost_Commissions"),
                CostExtraCharges = GetReaderValue<decimal>(reader, "Cost_ExtraCharges"),
                SaleNets = GetReaderValue<decimal>(reader, "Sale_Nets"),
                SaleDiscounts = GetReaderValue<decimal>(reader, "Sale_Discounts"),
                SaleCommissions = GetReaderValue<decimal>(reader, "Sale_Commissions"),
                SaleExtraCharges = GetReaderValue<decimal>(reader, "Sale_ExtraCharges"),
                SaleRate = GetReaderValue<decimal>(reader, "Sale_Rate"),
                CostRate = GetReaderValue<decimal>(reader, "Cost_Rate"),
                SaleRateType = GetReaderValue<byte>(reader, "Sale_RateType"),
                CostRateType = GetReaderValue<byte>(reader, "Cost_RateType"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
            };

            return instance;
        }

    }
}
