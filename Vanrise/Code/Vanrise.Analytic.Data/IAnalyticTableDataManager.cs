using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IAnalyticTableDataManager:IDataManager
    {
        bool AreAnalyticTableUpdated(ref object updateHandle);
        List<AnalyticTable> GetAnalyticTables();
        bool AddAnalyticTable(AnalyticTable analyticTable, out int analyticTableId);
        bool UpdateAnalyticTable(AnalyticTable analyticTable);
    }
}
