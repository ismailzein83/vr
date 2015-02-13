using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IAnalyticsDataManager : IDataManager
    {
        List<TopNDestinationView> GetTopNDestinations(int topCount,
           DateTime fromDate,
           DateTime toDate,
           string sortOrder,
           string customerID,
           string supplierID,
           int? switchID,
           char groupByCodeGroup,
           string codeGroup,
           char showSupplier,
            int from,
            int to);

        List<Alert> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag, string source, int? userID);
    }
}
