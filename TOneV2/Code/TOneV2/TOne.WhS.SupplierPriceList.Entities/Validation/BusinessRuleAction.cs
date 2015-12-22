using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public enum ActionSeverity { Information = 0, Warning = 1, Error = 2 };

    public abstract class BusinessRuleAction
    {
        public abstract void Execute(IRuleTarget target);

        public abstract ActionSeverity GetSeverity();
    }
}
