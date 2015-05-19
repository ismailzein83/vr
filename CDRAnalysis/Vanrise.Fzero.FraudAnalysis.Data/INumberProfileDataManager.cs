using System;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INumberProfileDataManager : IDataManager 
    {
        void LoadCDR(DateTime from, DateTime to, int? batchSize, Action<CDR> onBatchReady);
    }
}
