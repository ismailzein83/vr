using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IHourlyReportDataManager : IDataManager
    {
        GenericSummaryBigResult<HourlyReport> GetHourlyReportData(Vanrise.Entities.DataRetrievalInput<HourlyReportInput> input);
    }
}
