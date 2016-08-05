using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AlertRuleThresholdAction
    {
        public long AlertRuleThresholdActionId { get; set; }
        public int RuleId { get; set; }
        public decimal Threshold { get; set; }
        public int ThresholdActionIndex { get; set; }
    }
}
