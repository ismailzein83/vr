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
        List<ProfitByZone> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerIds, string supplierIds, int currencyId);

        List<SummaryByZone> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, int currencyId, string supplierGroup, string customerGroup, List<string> customerIds, List<string> supplierIds, bool groupBySupplier, out double services);
    }
}
