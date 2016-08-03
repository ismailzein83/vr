using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceAccountThreshold
    {
        public long AccountId { get; set; }
        public decimal Threshold { get; set; }
        public int ThresholdActionIndex { get; set; }
        public int AlertRuleId { get; set; }
    }
}
