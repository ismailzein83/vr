using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace Retail.Cost.Data.SQL
{
    public class CDRCostDataManager : BaseSQLDataManager, ICDRCostDataManager
    {
        public CDRCostDataManager()
            : base(GetConnectionStringName("RetailCDRDBConnStringKey", "RetailCDRDBConnString"))
        {

        }

        #region Public Methods
        public List<CDRCost> GetCDRCostByCDPNs(CDRCostBatchRequest cdrCostBatchRequest)
        {
            string cdpnsAsString = null;
            if (cdrCostBatchRequest.CDPNs != null)
                cdpnsAsString = String.Join(",", cdrCostBatchRequest.CDPNs);

            return GetItemsSP("[Retail_CDR].[sp_CDRCost_GetByCDPNs]", CDRCostMapper, cdrCostBatchRequest.BatchStart, cdrCostBatchRequest.BatchEnd, cdpnsAsString);
        }

        public void UpadeOverridenCostCDRAfterId(long? cdrCostId)
        {
            ExecuteNonQuerySP("[Retail_CDR].[sp_CDRCost_UpadeOverridenAfterId]", cdrCostId);
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