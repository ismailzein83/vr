using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum RuleActionType { Inform = 0, Warn = 1, ExecludeItem = 2, StopExecution = 3};

    public abstract class BusinessRule
    {
        public string CheckType { get; set; }

        public RuleActionType ActionType { get; set; }

        public List<string> TargetFQTNList { get; set; }

        public abstract bool Validate(IRuleTarget target);
        
    }
}
