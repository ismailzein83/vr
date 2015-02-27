using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteOptionRateMarginAction : BaseRouteOptionRuleAction
    {
        public decimal MinMargin { get; set; }

        public bool RemoveOption { get; set; }

        public override RouteRuleOptionExecutionResult Execute(RouteSupplierOption option, RouteDetail routeDetail, BaseRouteRule ruleDefinition)
        {
            decimal margin = routeDetail.Rate - option.Rate;
            if(margin < this.MinMargin)
            {
                if (this.RemoveOption)
                    return new RouteRuleOptionExecutionResult
                    {
                        RemoveOption = true
                    };
            }
            return new RouteRuleOptionExecutionResult();
        }
    }
}
