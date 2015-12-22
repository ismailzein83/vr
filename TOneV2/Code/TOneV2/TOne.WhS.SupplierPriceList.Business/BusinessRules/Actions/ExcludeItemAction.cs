using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ExcludeItemAction : BusinessRuleAction
    {
        public override void Execute(IRuleTarget target)
        {
            target.SetExcluded();
        }

        public override ActionSeverity GetSeverity()
        {
            return ActionSeverity.Warning;
        }
    }
}
