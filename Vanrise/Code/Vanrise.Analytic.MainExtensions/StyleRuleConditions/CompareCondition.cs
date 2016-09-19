using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.StyleRuleConditions
{
    public enum CompareOperator { Equals = 0, NotEquals = 1, Greater = 2, GreaterOrEquals = 3, Less = 4, LessOrEquals = 5}
    public class CompareCondition : StyleRuleCondition
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("4E972A40-2887-412B-B192-8E4E9739631C"); } }
        public CompareOperator CompareOperator { get; set; }

        public dynamic CompareValue { get; set; }

        public override bool Evaluate(IStyleRuleConditionContext context)
        {
            if (context.Value == null)
                return false;
            switch(this.CompareOperator)
            {
                case CompareOperator.Equals: return context.Value == this.CompareValue;
                case CompareOperator.NotEquals: return context.Value != this.CompareValue;
                case CompareOperator.Greater: return context.Value > this.CompareValue;
                case CompareOperator.GreaterOrEquals: return context.Value >= this.CompareValue;
                case CompareOperator.Less: return context.Value < this.CompareValue;
                case CompareOperator.LessOrEquals: return context.Value <= this.CompareValue;
            }
            return false;
        }
    }
}
