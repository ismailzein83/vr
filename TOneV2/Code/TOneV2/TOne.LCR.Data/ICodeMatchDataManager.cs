using System;
using TOne.LCR.Entities;
using System.Collections.Generic;
using System.Data;
using TOne.Entities;
namespace TOne.LCR.Data
{
    public interface ICodeMatchDataManager : IDataManager
    {
        Object PrepareCodeMatchesForDBApply(List<CodeMatch> codeMatches, bool isFuture);

        void ApplyCodeMatchesToDB(Object preparedCodeMatches);

        void CreateTempTable(bool isFuture);

        void SwapTableWithTemp(bool isFuture);

        void CreateIndexesOnTable(bool isFuture);

        List<string> GetDistinctCodes(bool isFuture, List<SupplierCodeInfo> suppliersCodeInfo);

        void CopyCodeMatchTableWithValidItems(bool isFuture, CodeList distinctCodes, List<SupplierCodeInfo> suppliersCodeInfo);        
    }
}
