using System;
using TOne.LCR.Entities;
using System.Collections.Generic;
using System.Data;
using TOne.Entities;
namespace TOne.LCR.Data
{
    public interface ICodeMatchDataManager : IDataManager, IRoutingDataManager
    {
        Object PrepareCodeMatchesForDBApply(List<CodeMatch> codeMatches);

        void ApplyCodeMatchesToDB(Object preparedCodeMatches);

        void CreateIndexesOnTable();
    }
}
