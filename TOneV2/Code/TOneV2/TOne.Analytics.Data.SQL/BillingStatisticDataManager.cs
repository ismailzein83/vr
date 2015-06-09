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
        public List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate ,string customerId ,string supplierId , bool groupByCustomer , int? supplierAMUId ,int? customerAMUId)
        {
            return GetItemsSP("Analytics.SP_Billing_GetZoneProfits", (reader) => ZoneProfitMapper(reader, groupByCustomer),
                fromDate,
                toDate,
                (customerId == null || customerId == "") ? null : customerId,
                (supplierId == null || supplierId == "") ? null : supplierId,
                groupByCustomer,
                (supplierAMUId == 0) ? (object)DBNull.Value : supplierAMUId,
                (customerAMUId== 0) ?  (object)DBNull.Value : customerAMUId );
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
               (supplierGroup==null || supplierGroup=="")?null:supplierGroup,
               (customerGroup == null || customerGroup == "") ? null : customerGroup,
               (supplierAMUId == 0 || supplierAMUId==null) ? (object)DBNull.Value : supplierAMUId,
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
                cmd.Parameters.Add(new SqlParameter("@ToDate",new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month))));
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

               Month =  GetReaderValue<int>(reader, "CalldateMonth"), 
            };
            if (IsAmount == true)
            {
                instance.Amount = GetReaderValue<double>(reader, "Amount") ;

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
                NumberOfCalls = GetReaderValue<int>(reader, "NumberOfCalls"),
                FirstCallTime = reader["FirstCallTime"] as string,
                LastCallTime = reader["LastCallTime"] as string,
                MinDuration = GetReaderValue<decimal>(reader, "MinDuration"),
                MaxDuration = GetReaderValue<decimal>(reader, "MaxDuration"),
                AvgDuration = GetReaderValue<decimal>(reader, "AvgDuration"),
                CostNets = GetReaderValue<double>(reader, "Cost_Nets"),
                CostDiscounts = GetReaderValue<double>(reader, "Cost_Discounts"),
                CostCommissions = GetReaderValue<double>(reader, "Cost_Commissions"),
                CostExtraCharges = GetReaderValue<double>(reader, "Cost_ExtraCharges"),
                SaleNets = GetReaderValue<double>(reader, "Sale_Nets"),
                SaleDiscounts = GetReaderValue<double>(reader, "Sale_Discounts"),
                SaleCommissions = GetReaderValue<double>(reader, "Sale_Commissions"),
                SaleExtraCharges = GetReaderValue<double>(reader, "Sale_ExtraCharges"),
                SaleRate = GetReaderValue<double>(reader, "Sale_Rate"),
                CostRate = GetReaderValue<double>(reader, "Cost_Rate"),
                SaleRateType = GetReaderValue<byte>(reader, "Sale_RateType"),
                CostRateType = GetReaderValue<byte>(reader, "Cost_RateType"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
            };

            return instance;
        }

        public List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue)
        {

            string query = String.Format(@"DECLARE @ExchangeRates TABLE(
		                                             Currency VARCHAR(3),
		                                             Date SMALLDATETIME,
		                                             Rate FLOAT
		                                             PRIMARY KEY(Currency, Date) )
	                                                                                 
	                                  	
                                           INSERT INTO @ExchangeRates 
                                            SELECT * FROM dbo.GetDailyExchangeRates(DATEADD(Day, -@PeriodCount, @FromDate), @FromDate)
                                            SELECT  cpc.Name , 0.0 as [AVG], 0.0 as [%], CallDate,
                                            SUM(BS.SaleDuration/60) AS [TotalDuration], 0.0 as [Prev %],cac.CarrierAccountID   
                                            From Billing_Stats BS With(Nolock,INDEX(IX_Billing_Stats_Date)) 
                                                LEFT JOIN @ExchangeRates ERC ON ERC.Currency = BS.Cost_Currency AND ERC.Date = BS.CallDate			
                                                LEFT JOIN @ExchangeRates ERS ON ERS.Currency = BS.Sale_Currency AND ERS.Date = BS.CallDate 
                                                JOIN CarrierAccount cac With(Nolock) ON cac.CarrierAccountID=BS.CustomerID
                                                JOIN CarrierProfile cpc With(Nolock) ON cpc.ProfileID = cac.ProfileID 
                                            WHERE CallDate BETWEEN DATEADD(Day, -@PeriodCount+1, @FromDate) AND @FromDate 
                                            GROUP BY cpc.Name,cac.CarrierAccountID , Calldate
                                          --  ORDER BY SUM(BS.SaleDuration/60) desc");
            return GetItemsText(query, VariationReportsMapper,
           (cmd) =>
           {
               cmd.Parameters.Add(new SqlParameter("@FromDate", selectedDate));
               cmd.Parameters.Add(new SqlParameter("@PeriodCount", periodCount));
               cmd.Parameters.Add(new SqlParameter("@PeriodTypeValue", periodTypeValue));
           });
        
        }

        private VariationReports VariationReportsMapper(IDataReader reader)
        {

            VariationReports instance = new VariationReports
            {
                Name = reader["Name"] as string,
                PeriodTypeValueAverage = GetReaderValue<decimal>(reader, "AVG"),
                PeriodTypeValuePercentage = GetReaderValue<decimal>(reader, "%"),
                CallDate = GetReaderValue<DateTime>(reader, "CallDate"),
                TotalDuration = GetReaderValue<decimal>(reader, "TotalDuration"),
                PreviousPeriodTypeValuePercentage = GetReaderValue<decimal>(reader, "Prev %"),
                CarrierAccountID = reader["CarrierAccountID"] as string,
            };
            
            return instance;
        }

    }
}
