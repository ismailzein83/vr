using Retail.Cost.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Retail.Cost.Data.SQL
{
    public class ProcessedCDRCostDataManager : BaseSQLDataManager, IProcessedCDRCostDataManager
    {
        public ProcessedCDRCostDataManager()
            : base(GetConnectionStringName("RetailCDRDBConnStringKey", "RetailCDRDBConnString"))
        {

        }

        #region Public Methods
        public List<ProcessedCDRCost> GetProcessedCDRCostByCDPNs(CDRCostBatchRequest cdrCostBatchRequest)
        {
            return GetItemsSP("[Retail_CDR].[sp_ProcessedCDRCost_GetByCDPNs]", ProcessedCDRCostMapper, cdrCostBatchRequest.BatchStart, cdrCostBatchRequest.BatchEnd, String.Join(",", cdrCostBatchRequest.CDPNs));
        }
        #endregion

        #region Mappers
        ProcessedCDRCost ProcessedCDRCostMapper(IDataReader reader)
        {
            ProcessedCDRCost processedCDRCost = new ProcessedCDRCost
            {
                ProcessedCDRCostId = (long)reader["ID"],
                AttemptDateTime = (DateTime)reader["AttemptDateTime"],
                CGPN = reader["CGPN"] as string,
                CDPN = reader["CDPN"] as string,
                DurationInSeconds = (decimal)reader["DurationInSeconds"],
                Rate = (decimal)reader["Rate"],
                Amount = GetReaderValue<decimal?>(reader, "Amount")
            };
            return processedCDRCost;
        }
        #endregion
    }
}
