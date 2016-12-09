using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Notification.BP.Arguments
{
    public class BalanceAlertCheckerProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Balance Alert Checker Process";
        }

        public Guid AlertRuleTypeId { get; set; }
    }
}
