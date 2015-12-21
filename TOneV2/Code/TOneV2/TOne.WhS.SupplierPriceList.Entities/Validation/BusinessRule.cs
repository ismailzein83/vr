using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum RuleActionType { Info = 0, Warning = 1, ExecludeItem = 2, StopExecution = 3};

    public abstract class BaseBusinessRule
    {
        public string CheckType { get; set; }

        public RuleActionType ActionType { get; set; }

        public abstract void SetExecluded();

        public abstract bool isValid();
        
    }
    
    public abstract class BusinessRule<T> : BaseBusinessRule
    {
        public IEnumerable<T> data { get; set; } 
    }
}
