using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.EntitiesMigrator.Entities
{
    public class InternationalRate
    {
        public RateDetails InternationalRateDetail { get; set; }
        public string ZoneName { get; set; }
        public long SubscriberId { get; set; }

        public DateTime BED { get; set; }
        public DateTime ActivationDate { get; set; }
    }

    public class OffNetRate
    {
        public string SourceBranchId { get; set; }

        public string OperatorName { get; set; }

        public RateDetails RateDetail { get; set; }
    }
}
