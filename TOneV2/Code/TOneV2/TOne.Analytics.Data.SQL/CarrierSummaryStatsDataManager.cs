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
    public class CarrierSummaryStatsDataManager : BaseTOneDataManager, ICarrierSummaryStatsDataManager
    {

        private CarrierZoneSummaryStatsCommon trafficStatisticCommon = new CarrierZoneSummaryStatsCommon();
        public CarrierSummaryBigResult<CarrierSummaryStats> GetCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<CarrierSummaryStatsQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierSummaryStats_CreateTempForFiltered", tempTableName, input.Query.CarrierType, input.Query.FromDate, input.Query.ToDate, input.Query.CustomerID, input.Query.SupplierID, input.Query.ZoneID, input.Query.TopRecord, input.Query.GroupByProfile, input.Query.CustomerAmuID, input.Query.SupplierAmuID, input.Query.Currency);

            }, CarrierSummaryStatsMapper, null, new CarrierSummaryBigResult<CarrierSummaryStats>()) as CarrierSummaryBigResult<CarrierSummaryStats>;
        }

//        public CarrierSummaryBigResult<CarrierSummaryStats> GetCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<CarrierZoneSummaryInput> input)
//        {
//            Dictionary<string, string> mapper = new Dictionary<string, string>();
//            string columnId;
//            CarrierZoneSummaryStatsCommon.GetColumnNames(input.Query.GroupKeys[0], out columnId);

//            string tempTable = null;
//            Action<string> createTempTableAction = (tempTableName) =>
//            {
//                tempTable = tempTableName;
//                ExecuteNonQueryText(CreateTempTableIfNotExists(tempTableName, input.Query.Filter, input.Query.GroupKeys), (cmd) =>
//                {
//                    cmd.Parameters.Add(new SqlParameter("@FromDate", input.Query.From));
//                    cmd.Parameters.Add(new SqlParameter("@ToDate", input.Query.To));
//                });
//            };



 //       }

//        private string CreateTempTableIfNotExists(string tempTableName, GenericFilter filter, IEnumerable<TrafficStatisticGroupKeys> groupKeys)
//        {


//            StringBuilder whereBuilder = new StringBuilder();
//            string tableName = trafficStatisticCommon.GetTableName(groupKeys, filter);
//            StringBuilder queryBuilder = new StringBuilder(@"
//                            IF NOT OBJECT_ID('#TEMPTABLE#', N'U') IS NOT NULL
//	                            BEGIN with
//                                Traffic_ AS (SELECT CustomerID, Calldate, Attempts, DeliveredAttempts, DeliveredNumberOfCalls, SuccessfulAttempts, DurationsInSeconds, PDDInSeconds, 
//                                                    NumberOfCalls FROM TrafficStatsDaily ts WITH(NOLOCK,INDEX(IX_TrafficStatsDaily_DateTimeFirst))
//                                            WHERE (Calldate >= @fromDate AND Calldate <  @ToDate )
//		                                        AND CustomerID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
//                                                AND SupplierID NOT IN (SELECT rasc.CID FROM @RepresentedAsSwitchCarriers rasc)
//                                                AND(@CustomerAmuID IS NULL OR CustomerID IN (SELECT * FROM @CustomerIDs))
//		                                        AND(@SupplierAmuID IS NULL OR SupplierID IN (SELECT * FROM @SupplierIDs))
//                                            ),
//                                Traffic AS
//                                     (
//	                                        SELECT 
//	                                            ISNULL(TS.CustomerID, '') AS CustomerID, 
//	                                            Sum(Attempts) as Attempts, 
//	                                            Sum(SuccessfulAttempts) as SuccessfulAttempts,
//	                                            Sum(DurationsInSeconds/60.) as DurationsInMinutes, 
//	                                            case when Sum(NumberofCalls) > 0 then Sum(SuccessfulAttempts)*100.0 / Sum(NumberofCalls) ELSE 0 end as ASR, 
//	                                            case when Sum(SuccessfulAttempts) > 0 then Sum(DurationsInSeconds)/(60.0*Sum(SuccessfulAttempts)) ELSE 0 end as ACD, 
//	                                            case when Sum(NumberofCalls) > 0 then Sum(DeliveredNumberofCalls) * 100.0 / SUM(NumberofCalls) ELSE 0 end as DeliveredASR, 
//	                                            Avg(PDDinSeconds) as AveragePDD 
//	                                        FROM Traffic_ TS WITH(NOLOCK, INDEX(IX_TrafficStatsDaily_DateTimeFirst))
//	                                        WHERE  (@CustomerID IS NULL OR ts.CustomerId = @CustomerID)   
//	                                        GROUP BY ISNULL(TS.CustomerID, '') 
//                             
//           
//                                     ),
//                                Billing AS 
//                                    (
//	                                        SELECT
//                                              BS.CustomerID AS CustomerID,
//	                                          ISNULL(SUM(BS.NumberOfCalls),0) AS Calls,
//	                                          ISNULL(SUM(BS.SaleDuration)/60,0) AS PricedDuration,
//	                                          ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0) AS Sale,
//	                                          ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Cost,
//	                                          ISNULL(SUM(BS.Sale_Nets/ISNULL(ERS.Rate, 1)),0)-ISNULL(SUM(BS.Cost_Nets/ISNULL(ERC.Rate, 1)),0) AS Profit,
//	                                          0 AS PercentageProfit
//	                                        FROM
//		                                         Billing_Stats BS WITH(NOLOCK,Index(IX_Billing_Stats_Date)) 
//	                                             LEFT JOIN @ExchangeRates ERC ON ERC.Currency = bs.Cost_Currency AND ERC.Date = bs.CallDate
//                                                 LEFT JOIN @ExchangeRates ERS ON ERS.Currency = bs.Sale_Currency AND ERS.Date = bs.CallDate
//	                                        WHERE  (BS.CallDate >= @fromDate AND BS.CallDate < @ToDate)
//		                                        AND (@CustomerID IS NULL OR BS.CustomerID =  @CustomerID)
//		                                        AND(@CustomerAmuID IS NULL OR BS.CustomerID IN (SELECT * FROM @CustomerIDs))
//		                                        AND(@SupplierAmuID IS NULL OR BS.SupplierID IN (SELECT * FROM @SupplierIDs))
//	                                        GROUP BY BS.CustomerID
//
//                                    ),
//                                     Results AS(
//			                        SELECT  T.CustomerID, T.Attempts, T.SuccessfulAttempts, T.DurationsInMinutes, T.ASR, T.ACD, T.DeliveredASR, T.AveragePDD,
//	                                        B.Calls AS NumberOfCalls,B.PricedDuration AS PricedDuration, isnull(B.Sale,0) AS Sale_Nets,isnull(B.Cost,0) AS Cost_Nets,isnull(B.Profit,0) AS Profit,0 AS Percentage
//	                                         ,  ROW_NUMBER() OVER (ORDER BY DurationsInMinutes DESC) AS rownIndex
//	                                FROM Traffic T WITH(NOLOCK)
//	                                LEFT JOIN Billing B ON T.CustomerID = B.CustomerID
//                                    )
//	                            SELECT * FROM Results WHERE  
//                            rownIndex <= @TopRecord
//                            and CustomerID NOT IN (SELECT grasc.CID FROM dbo.GetRepresentedAsSwitchCarriers() grasc)  
//
//                            SELECT * INTO #TEMPTABLE# FROM Results
//                            END");
//        }

        private CarrierSummaryStats CarrierSummaryStatsMapper(IDataReader reader)
        {
            return new CarrierSummaryStats
            {
                GroupID = reader["GroupID"] as string,
                Attempts = Math.Round(GetReaderValue<double>(reader, "Attempts"), 2),
                SuccessfulAttempts = Math.Round(GetReaderValue<double>(reader, "SuccessfulAttempts") ,2),
                DurationsInMinutes = Math.Round(GetReaderValue<double>(reader, "DurationsInMinutes"),2),
                ASR = Math.Round(GetReaderValue<double>(reader, "ASR"), 2),
                ACD = Math.Round(GetReaderValue<double>(reader, "ACD"), 2),
                DeliveredASR = Math.Round(GetReaderValue<double>(reader, "DeliveredASR"), 2),
                AveragePDD = Math.Round(GetReaderValue<double>(reader, "AveragePDD"), 2),
                NumberOfCalls = Math.Round(GetReaderValue<double>(reader, "NumberOfCalls"), 2),
                PricedDuration = Math.Round(GetReaderValue<double>(reader, "PricedDuration"), 2),
                Sale_Nets = Math.Round(GetReaderValue<double>(reader, "Sale_Nets"), 2),
                Cost_Nets = Math.Round(GetReaderValue<double>(reader, "Cost_Nets"), 2),
                Profit = Math.Round(GetReaderValue<double>(reader, "Profit"), 2),
                Percentage = Math.Round(GetReaderValue<double>(reader, "Percentage"), 2),
                rownIndex = GetReaderValue<int>(reader, "rownIndex")
            };
        }
    }
}
