using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceNextThresholdUpdateEntity
    {
        public Guid AccountTypeId { get; set; }
        public String AccountId { get; set; }
        public decimal? NextAlertThreshold { get; set; }
        public long? AlertRuleId { get; set; }
        public int? ThresholdActionIndex { get; set; }

    }
}
