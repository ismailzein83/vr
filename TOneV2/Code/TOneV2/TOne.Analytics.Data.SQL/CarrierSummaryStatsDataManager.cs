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
        public Vanrise.Entities.BigResult<CarrierSummaryStats> GetCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<CarrierSummaryStatsQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierSummaryStats_CreateTempForFiltered", tempTableName, input.Query.CarrierType, input.Query.FromDate, input.Query.ToDate, input.Query.CustomerID, input.Query.SupplierID, input.Query.ZoneID, input.Query.TopRecord, input.Query.GroupByProfile, input.Query.CustomerAmuID, input.Query.SupplierAmuID, input.Query.Currency);

            }, CarrierSummaryStatsMapper);
        }

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
