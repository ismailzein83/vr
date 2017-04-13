
using System;
using System.Collections.Generic;
namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleExecPayload : DAProfCalcExecPayload
    {
        public List<long> AlertRuleIds { get; set; }

        public Guid AlertRuleTypeId { get; set; }
    }
}