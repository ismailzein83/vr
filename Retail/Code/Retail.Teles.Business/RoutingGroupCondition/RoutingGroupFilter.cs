using Retail.Teles.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public enum RoutingGroupFilterOperator
    {
        [Description(" = ")]
        Equals = 0,
        [Description(" <> ")]
        NotEquals = 1,
        [Description(" Starts With ")]
        StartsWith = 2,
        [Description(" Not Starts With ")]
        NotStartsWith = 3,
        [Description(" Ends With ")]
        EndsWith = 4,
        [Description(" Not Ends With ")]
        NotEndsWith = 5,
        [Description(" Contains ")]
        Contains = 6,
        [Description(" Not Contains ")]
        NotContains = 7
    }
    public class RoutingGroupFilter : RoutingGroupCondition
    {
        public override Guid ConfigId
        {
            get { return new Guid("772C0B47-A8C0-4F15-B2F1-AEBF52B3EF08"); }
        }
        public RoutingGroupFilterOperator Operator { get; set; }
        public string RoutingGroupName { get; set; }

        public override bool Evaluate(IRoutingGroupConditionContext context)
        {
            bool result = false;
            switch(Operator)
            {
                case RoutingGroupFilterOperator.Equals:
                    if(context.RoutingGroupName == this.RoutingGroupName)
                        result = true;
                    break;
                case RoutingGroupFilterOperator.NotEquals:
                    if(context.RoutingGroupName != this.RoutingGroupName)
                        result = true;
                    break;
                case RoutingGroupFilterOperator.StartsWith:
                    if(context.RoutingGroupName.StartsWith(this.RoutingGroupName))
                        result = true;
                    break;
                case RoutingGroupFilterOperator.NotStartsWith:
                    if(!context.RoutingGroupName.StartsWith(this.RoutingGroupName))
                        result = true;
                    break;
                case RoutingGroupFilterOperator.EndsWith:
                    if(context.RoutingGroupName.EndsWith(this.RoutingGroupName))
                        result = true;
                    break;
                case RoutingGroupFilterOperator.NotEndsWith:
                    if (!context.RoutingGroupName.EndsWith(this.RoutingGroupName))
                        result = true;
                    break;
                case RoutingGroupFilterOperator.Contains:
                    if(context.RoutingGroupName.Contains(this.RoutingGroupName))
                        result = true;
                    break;
                case RoutingGroupFilterOperator.NotContains:
                    if (!context.RoutingGroupName.Contains(this.RoutingGroupName))
                        result = true;
                    break;
            }
            return result;
        }
    }
}
