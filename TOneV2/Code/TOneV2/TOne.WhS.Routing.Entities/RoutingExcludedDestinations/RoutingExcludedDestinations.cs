using System;
using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RoutingExcludedDestinations
    {
        public abstract Guid ConfigId { get; }

        public abstract bool IsExcludedDestination(IRoutingExcludedDestinationContext context);

        public abstract RoutingExcludedDestinationData GetRoutingExcludedDestinationData();
    }

    public interface IRoutingExcludedDestinationContext
    {
        string Code { get; }

        int RuleId { get; }
    }

    public class RoutingExcludedDestinationContext : IRoutingExcludedDestinationContext
    {
        public string Code { get; set; }

        public int RuleId { get; set; }
     
        public RoutingExcludedDestinationContext(string code, int ruleId)
        {
            this.Code = code;
            this.RuleId = ruleId;
        }
    }
}