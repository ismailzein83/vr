using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class AlertRuleActionExecution
    {
        public long AlertRuleActionExecutionId { get; set; }
        public long AccountID { get; set; }
        public decimal Threshold { get; set; }
        public ActionExecutionInfo ActionExecutionInfo { get; set; }
        public DateTime ExecutionTime { get; set; }
    }
    public class ActionExecutionInfo
    {
        public List<VRAction> RollBackActions { get; set; }
    }
}
