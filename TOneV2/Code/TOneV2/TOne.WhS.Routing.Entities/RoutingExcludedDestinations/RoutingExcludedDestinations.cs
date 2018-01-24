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
    }

    public class RoutingExcludedDestinationContext : IRoutingExcludedDestinationContext 
    { 
        public string Code { get; set; }
    }
}