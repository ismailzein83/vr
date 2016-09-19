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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("8C5B1E66-20F0-4B26-BC4F-01060B3C3DAA"); } }
        public dynamic RangeStart { get; set; }

        public dynamic RangeEnd { get; set; }

        public override bool Evaluate(IStyleRuleConditionContext context)
        {
            return context.Value != null && context.Value >= this.RangeStart && context.Value <= this.RangeEnd;
        }
    }
}
