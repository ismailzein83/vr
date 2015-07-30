using System;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INumberProfileDataManager : IDataManager, IBulkApplyDataManager<NumberProfile>
    {
        void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady);

        void ApplyNumberProfilesToDB(object preparedNumberProfiles);
    }
}
