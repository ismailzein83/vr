using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class TelephoneServiceDetail
    {
        //Used in CRM
        public string Code { get; set; }
        public string Name { get; set; }
        public string RatePlanId { get; set; }
        public decimal SubscriptionFee { get; set; }
        public decimal RecurringCharge { get; set; }
        public List<ServiceParameters> ServiceParams { get; set; }

        //Not Used
        public string Description { get; set; }
        public bool IsMain { get; set; }
        public string Package { get; set; }
        public decimal UsageFee { get; set; }
    }

    public class ServiceParameters
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
