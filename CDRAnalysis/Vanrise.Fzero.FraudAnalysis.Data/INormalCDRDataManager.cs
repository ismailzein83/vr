using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INormalCDRDataManager : IDataManager 
    {
        void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady);
        BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRResultQuery> input);
    }
}
