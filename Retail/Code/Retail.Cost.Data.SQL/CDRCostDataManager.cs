using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Vanrise.Data.SQL;

namespace Retail.Cost.Data.SQL
{
    public class CDRCostDataManager : BaseSQLDataManager, ICDRCostDataManager
    {
        #region Properties/Ctor

        public CDRCostDataManager()
            : base(GetConnectionStringName("RetailCDRDBConnStringKey", "RetailCDRDBConnString"))
        {

        }

        #endregion

        #region Public Methods

        public List<CDRCost> GetCDRCostByCDPNs(CDRCostBatchRequest cdrCostBatchRequest)
        {
            string cdpnsAsString = null;
            if (cdrCostBatchRequest.CDPNs != null)
                cdpnsAsString = String.Join(",", cdrCostBatchRequest.CDPNs);

            return GetItemsSP("[Retail_CDR].[sp_CDRCost_GetByCDPNs]", CDRCostMapper, cdrCostBatchRequest.BatchStart, cdrCostBatchRequest.BatchEnd, cdpnsAsString);
        }

        public void UpadeOverridenCostCDRAfterDate(DateTime? fromTime)
        {
            ExecuteNonQuerySP("[Retail_CDR].[sp_CDRCost_UpadeOverridenAfterDate]", fromTime);
        }

        public HashSet<DateTime> GetDistinctDatesAfterId(long? cdrCostId)
        {
            HashSet<DateTime> distinctDates = new HashSet<DateTime>();
            ExecuteReaderSPCmd("[Retail_CDR].[sp_CDRCost_GetDistinctDatesAfterId]",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       DateTime? attemptDateTime = GetReaderValue<DateTime?>(reader, "AttemptDateTime");
                       if (attemptDateTime.HasValue)
                           distinctDates.Add(attemptDateTime.Value);
                   }
               },
               (cmd) =>
               {
                   SqlParameter cdrCostIdParameter = new SqlParameter() { ParameterName = "@CDRCostId" };
                   if (cdrCostId.HasValue)
                       cdrCostIdParameter.Value = cdrCostId.Value;
                   else
                       cdrCostIdParameter.Value = DBNull.Value;
                   cmd.Parameters.Add(cdrCostIdParameter);
               });

            return distinctDates.Count > 0 ? distinctDates : null;
        }

        public long? GetMaxCDRCostId()
        {
            object id = ExecuteScalarSP("[Retail_CDR].[sp_CDRCost_GetMaxId]");
            return id != null ? (long?)id : null;
        }

        #endregion

        #region Mappers

        CDRCost CDRCostMapper(IDataReader reader)
        {
            CDRCost cdrCost = new CDRCost
            {
                CDRCostId = (long)reader["ID"],
                SourceId = reader["SourceID"] as string,
                AttemptDateTime = GetReaderValue<DateTime?>(reader, "AttemptDateTime"),
                CGPN = reader["CGPN"] as string,
                CDPN = reader["CDPN"] as string,
                DurationInSeconds = GetReaderValue<decimal?>(reader, "DurationInSeconds"),
                Rate = GetReaderValue<decimal?>(reader, "Rate"),
                Amount = GetReaderValue<decimal?>(reader, "Amount"),
                SupplierName = reader["SupplierName"] as string,
                CurrencyId = GetReaderValue<int?>(reader, "CurrencyId")
            };
            return cdrCost;
        }

        #endregion
    }
}