using System;
using TOne.LCR.Entities;
using System.Collections.Generic;
using System.Data;
using TOne.Entities;
namespace TOne.LCR.Data
{
    public interface ICodeMatchDataManager : IDataManager, IRoutingDataManager
    {
        void ApplyCodeMatchesToDB(Object preparedCodeMatches);

        void CreateIndexesOnTable();

        object InitialiazeStreamForDBApply();

        void WriteCodeMatchToStream(CodeMatch codeMatch, object stream);

        object FinishDBApplyStream(object stream);

        CodeMatchesByZoneId GetCodeMatchesByCodes(IEnumerable<String> codes, out HashSet<int> supplierZoneIds, out HashSet<int> customerZoneIds);
    }
}