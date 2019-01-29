using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateChangesDetail
    {
        public string MobileCountryName { get; set; }

        public int MobileNetworkID { get; set; }

        public string MobileNetworkName { get; set; }

        public decimal? CurrentRate { get; set; }

        public decimal? NewRate { get; set; }

        public bool? HasFutureRate { get; set; }

        public SMSFutureRate FutureRate { get; set; }

    }

    public enum SMSRateChangeType
    {
        [Description("Same")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,

        [Description("Increase")]
        Increase = 3,

        [Description("Decrease")]
        Decrease = 4,

        [Description("Rate Not Available")]
        RateNotAvailable = 5
    }
}