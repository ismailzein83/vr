using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWTimeDataManager : IDataManager, IBulkApplyDataManager<DWTime>
    {
        List<DWTime> GetTimes(DateTime from, DateTime to);

        void ApplyDWTimesToDB(object preparedDWTimes);

        void SaveDWTimesToDB(List<DWTime> dwTimes);

    }
}
