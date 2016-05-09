using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities.BillingReport;

namespace TOne.WhS.Analytics.Data
{
    public interface IBillingStatisticDataManager : IDataManager
    {
        List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string currencyId);
        List<ZoneProfit> GetZoneProfitOld(DateTime fromDate, DateTime toDate, int customerId, int supplierId, bool groupByCustomer, List<string> supplierIds, List<string> customerIds, string currencyId);
    }
}
