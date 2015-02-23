using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class AlertRule
    {
        public int AlertRuleId { get; set; }

        public BaseCarrierAccountSet CarrierAccountSet { get; set; }

        public BaseCodeSet CodeSet { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public BaseAlertRuleCondition Condition { get; set; }

        public BaseAlertRuleAction Action { get; set; }
    }
}
