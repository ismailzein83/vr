using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.BP.Arguments
{
    public class BalanceAlertThresholdUpdateProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Balance Alert Threshold Update Process";
        }

        public Guid AlertRuleTypeId { get; set; }
    }
}
