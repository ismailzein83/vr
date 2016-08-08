using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public interface IAnalyticReportManager : IBEManager
    {
        AnalyticReport GetAnalyticReportById(int analyticReportId);
    }
}
