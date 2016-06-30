using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities.BillingReport;

namespace TOne.WhS.Analytics.Data
{
    public interface IBillingReportDataManager : IDataManager
    {
       List<BusinessCaseStatus> GetBusinessCaseStatus(DateTime fromDate, DateTime toDate, int carrierAccountId, int topDestination, bool isSale, bool isAmount, int currencyId);
    }
}
