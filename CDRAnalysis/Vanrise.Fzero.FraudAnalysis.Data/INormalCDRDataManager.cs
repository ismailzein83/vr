using System;
using Vanrise.Entities;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INormalCDRDataManager : IDataManager 
    {

        void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady);

        

        BigResult<CDR> GetNormalCDRs(Vanrise.Entities.DataRetrievalInput<NormalCDRQuery> input);
    }
}
