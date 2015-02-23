using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RateMarginRuleCondition : BaseAlertRuleCondition
    {
        public decimal Margin { get; set; }

        public override bool Check(RouteDetail route)
        {
            if(route.Options != null && route.Options.SupplierOptions != null)
            {
                foreach(var supOption in route.Options.SupplierOptions)
                {
                    if(supOption.Rate > route.Rate && (supOption.Rate - route.Rate) * 100/ supOption.Rate > this.Margin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
