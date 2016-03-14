﻿using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IRelatedNumberDataManager : IDataManager
    {
        void SavetoDB(AccountRelatedNumberDictionary record);

        void CreateTempTable();

        void SwapTableWithTemp();
        List<RelatedNumber> GetRelatedNumbersByAccountNumber(string accountNumber);
    }
}
