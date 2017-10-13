using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class SalePricelistFileContext : ISalePricelistFileContext
    {
        ActivityContext _activityContext;

        public SalePricelistFileContext(ActivityContext activityContext)
        {
            this._activityContext = activityContext;
        }

        public int SellingNumberPlanId { get; set; }

        public long ProcessInstanceId { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<SalePLZoneChange> ZoneChanges { get; set; }

        public DateTime EffectiveDate { get; set; }

        public SalePLChangeType ChangeType { get; set; }

        public IEnumerable<int> EndedCountryIds { get; set; }

        public DateTime? CountriesEndedOn { get; set; }

        public IEnumerable<NewCustomerPriceListChange> CustomerPriceListChanges { get; set; }
        public IEnumerable<NewPriceList> SalePriceLists { get; set; }

        public int CurrencyId { get; set; }

        public int UserId { get; set; }

        public void WriteMessageToWorkflowLogs(string messageFormat, params object[] args)
        {
            this._activityContext.WriteTrackingMessage(LogEntryType.Information, messageFormat, args);
        }
    }
}
