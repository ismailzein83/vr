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
                ExecuteNonQuerySP("BEntity.sp_CarrierSummaryStats_CreateTempForFiltered", tempTableName, input.Query.CarrierType, input.Query.FromDate, input.Query.ToDate, input.Query.CustomerID, input.Query.SupplierID, input.Query.TopRecord, input.Query.GroupByProfile, input.Query.CustomerAmuID, input.Query.SupplierAmuID, input.Query.Currency);

            }, CarrierSummaryStatsMapper);
        }

        private CarrierSummaryStats CarrierSummaryStatsMapper(IDataReader reader)
        {
            return new CarrierSummaryStats
            {
                GroupID = reader["GroupID"] as string,
                Attempts = GetReaderValue<double>(reader, "Attempts"),
                SuccessfulAttempts = GetReaderValue<double>(reader, "SuccessfulAttempts"),
                DurationsInMinutes = GetReaderValue<double>(reader, "DurationsInMinutes"),
                ASR = GetReaderValue<double>(reader, "ASR"),
                ACD = GetReaderValue<double>(reader, "ACD"),
                DeliveredASR = GetReaderValue<double>(reader, "DeliveredASR"),
                AveragePDD = GetReaderValue<double>(reader, "AveragePDD"),
                NumberOfCalls = GetReaderValue<double>(reader, "NumberOfCalls"),
                PricedDuration = GetReaderValue<double>(reader, "PricedDuration"),
                Sale_Nets = GetReaderValue<double>(reader, "Sale_Nets"),
                Cost_Nets = GetReaderValue<double>(reader, "Cost_Nets"),
                Profit = GetReaderValue<double>(reader, "Profit"),
                Percentage = GetReaderValue<double>(reader, "Percentage"),
                rownIndex = GetReaderValue<int>(reader, "rownIndex")
            };
        }
    }
}
