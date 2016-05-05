using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Data
{
    public interface IRealTimeReportDataManager:IDataManager
    {
        bool AreRealTimeReportUpdated(ref object updateHandle);
        List<RealTimeReport> GetRealTimeReports();
        bool AddRealTimeReport(Entities.RealTimeReport realTimeReport, out int realTimeReportId);
        bool UpdateRealTimeReport(Entities.RealTimeReport realTimeReport);

    }
}
