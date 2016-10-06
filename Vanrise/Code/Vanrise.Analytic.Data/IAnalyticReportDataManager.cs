using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IAnalyticReportDataManager:IDataManager
    {
        List<AnalyticReport> GetAnalyticReports();
        bool AreAnalyticReportUpdated(ref object updateHandle);
        bool AddAnalyticReport(Entities.AnalyticReport analyticReport);
        bool UpdateAnalyticReport(Entities.AnalyticReport analyticReport);
    }
}
