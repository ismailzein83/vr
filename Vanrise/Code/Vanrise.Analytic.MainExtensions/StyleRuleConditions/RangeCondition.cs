using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.StyleRuleConditions
{
    public class RangeCondition : StyleRuleCondition
    {
        public dynamic RangeStart { get; set; }

        public dynamic RangeEnd { get; set; }

        public override bool Evaluate(IStyleRuleConditionContext context)
        {
            return context.Value != null && context.Value >= this.RangeStart && context.Value <= this.RangeEnd;
        }
    }
}
