using System;
using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IRelatedNumberDataManager : IDataManager
    {
        void SavetoDB(List<RelatedNumber> record);

        List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber);

        void LoadRelatedNumberByNumberPrefix(string numberPrefix, Action<RelatedNumber> onBatchReady);
    }
}
