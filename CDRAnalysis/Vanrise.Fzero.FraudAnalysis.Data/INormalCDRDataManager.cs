using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INormalCDRDataManager : IDataManager 
    {
        void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady);
        List<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn);
    }
}
