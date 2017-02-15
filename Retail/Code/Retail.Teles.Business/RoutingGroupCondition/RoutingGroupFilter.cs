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
        public List<string> RoutingGroupNames { get; set; }

        public override bool Evaluate(IRoutingGroupConditionContext context)
        {
            bool result = false;
            if (context.RoutingGroupName != null)
            {
                var routingGroupName = context.RoutingGroupName.ToLower();
                switch (Operator)
                {
                    case RoutingGroupFilterOperator.Equals:
                        if (RoutingGroupNames.Any(x=> x.ToLower() == routingGroupName))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.NotEquals:
                        if (!RoutingGroupNames.Any(x => x.ToLower() == routingGroupName))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.StartsWith:
                        if (RoutingGroupNames.Any(x => routingGroupName.StartsWith(x.ToLower())))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.NotStartsWith:
                        if (!RoutingGroupNames.Any(x => routingGroupName.StartsWith(x.ToLower())))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.EndsWith:
                        if (RoutingGroupNames.Any(x => routingGroupName.EndsWith(x.ToLower())))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.NotEndsWith:
                        if (!RoutingGroupNames.Any(x => routingGroupName.EndsWith(x.ToLower())))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.Contains:
                        if (RoutingGroupNames.Any(x => routingGroupName.Contains(x.ToLower())))
                            result = true;
                        break;
                    case RoutingGroupFilterOperator.NotContains:
                        if (!RoutingGroupNames.Any(x => routingGroupName.Contains(x.ToLower())))
                            result = true;
                        break;
                }
            }
            
            return result;
        }
    }
}
