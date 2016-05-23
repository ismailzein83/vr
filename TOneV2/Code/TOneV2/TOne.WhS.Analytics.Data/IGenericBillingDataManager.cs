using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;
using TOne.WhS.Analytics.Entities.BillingReport;
using Vanrise.Entities;

namespace TOne.WhS.Analytics.Data
{
    public interface IGenericBillingDataManager : IDataManager
    {
        List<BillingReportRecord> GetFilteredBillingReportRecords(BillingReportQuery input);

    }
}
