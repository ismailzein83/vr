using System;
using System.Collections.Generic;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class NormalCDRDataManager : BaseMySQLDataManager, INormalCDRDataManager 
    {
        public NormalCDRDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady)
        {
            throw new NotImplementedException();
        }

        public List<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            throw new NotImplementedException();
        }

       
    }
}
