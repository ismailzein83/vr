using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.BP.Arguments
{
    public class BalanceAlertCheckerProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public override string GetTitle()
        {
            return "Balance Alert Checker";
        }
    }
}
