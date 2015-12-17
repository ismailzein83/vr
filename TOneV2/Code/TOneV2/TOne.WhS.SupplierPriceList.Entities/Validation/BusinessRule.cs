using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum RuleActionType { Info = 0, Warning = 1, ExecludeItem = 2, StopExecution = 3};

    public class BusinessRule
    {
        public string CheckType { get; set; }

        public RuleActionType ActionType { get; set; }

        public Type RuleTargetType { get; set; }
    }
}
