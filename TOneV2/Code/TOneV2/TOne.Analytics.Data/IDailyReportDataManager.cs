using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IDailyReportDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<DailyReportCall> GetFilteredDailyReportCalls(Vanrise.Entities.DataRetrievalInput<DailyReportQuery> input, List<string> assignedCustomerIDs, List<string> assignedSupplierIDs);
    }
}
