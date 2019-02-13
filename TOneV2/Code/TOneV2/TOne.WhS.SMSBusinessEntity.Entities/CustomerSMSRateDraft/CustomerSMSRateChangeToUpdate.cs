using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateDraftToUpdate
    {
        public long? ProcessDraftID { get; set; }

        public int CustomerID { get; set; }

        public int CurrencyId { get; set; }

        public List<CustomerSMSRateChangeToUpdate> SMSRates { get; set; }

        public DateTime EffectiveDate { get; set; }
    }

    public class CustomerSMSRateChangeToUpdate
    {
        public int MobileNetworkID { get; set; }

        public decimal? NewRate { get; set; }
    }
}
