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
        //public List<Entities.CarrierSummaryStats> GetCarrierSummaryStats(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, char groupByProfile, int? topCount, string currency)
        //{
        //    return GetItemsSP("Analytics.SP_TrafficStats_CarrierSummary", CarrierSummaryStatsMapper, carrierType, fromDate, toDate, customerID, supplierID, topCount, groupByProfile, null, null, currency);
        //}

        public Vanrise.Entities.BigResult<CarrierSummaryStats> GetCarrierSummaryStatsByCriteria(Vanrise.Entities.DataRetrievalInput<CarrierSummaryStatsQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                ExecuteNonQuerySP("BEntity.sp_CarrierSummaryStats_CreateTempForFiltered", tempTableName, (input.Query.CarrierType == "True" ? "Customer" : "Supplier"), input.Query.FromDate, input.Query.ToDate, input.Query.CustomerID, input.Query.SupplierID, input.Query.TopRecord, (input.Query.GroupByProfile == "True" ? "Y" : "N"), input.Query.CustomerAmuID, input.Query.SupplierAmuID, input.Query.Currency);

            }, CarrierSummaryStatsMapper);
        }


        private CarrierSummaryStats CarrierSummaryStatsMapper(IDataReader reader)
        {
            return new CarrierSummaryStats
            {
                SupplierID = reader["SupplierID"] as string,
                Attempts = GetReaderValue<decimal>(reader, "Attempts"),
                SuccessfulAttempts = GetReaderValue<decimal>(reader, "SuccessfulAttempts"),
                DurationsInMinutes = GetReaderValue<decimal>(reader, "DurationsInMinutes"),
                ASR = GetReaderValue<decimal>(reader, "ASR"),
                ACD = GetReaderValue<decimal>(reader, "ACD"),
                DeliveredASR = GetReaderValue<decimal>(reader, "DeliveredASR"),
                AveragePDD = GetReaderValue<decimal>(reader, "AveragePDD"),
                NumberOfCalls = GetReaderValue<decimal>(reader, "NumberOfCalls"),
                PricedDuration = GetReaderValue<decimal>(reader, "PricedDuration"),
                Sale_Nets = GetReaderValue<decimal>(reader, "Sale_Nets"),
                Cost_Nets = GetReaderValue<decimal>(reader, "Cost_Nets"),
                Profit = GetReaderValue<decimal>(reader, "Profit"),
                Percentage = GetReaderValue<double>(reader, "Percentage")
            };
        }
    }
}
