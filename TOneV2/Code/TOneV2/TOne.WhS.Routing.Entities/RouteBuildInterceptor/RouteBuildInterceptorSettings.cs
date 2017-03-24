using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public abstract class RouteBuildInterceptorSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Initialize(IRouteBuildInterceptorInitializeContext context);

        public abstract void ApplyChangesToRoute(IRouteBuildInterceptorApplyChangesContext context);
    }

    public interface IRouteBuildInterceptorInitializeContext
    {
        Object InitializationData { set; }
    }

    public interface IRouteBuildInterceptorApplyChangesContext
    {
        Object InitializationData { get; }

        string Code { get; }

        int CustomerId { get; }

        long SaleZoneId { get; }

        List<RouteOption> Options { get; set; }

        bool TryGetSupplierCodeMatch(int supplierId, out SupplierCodeMatchWithRate codeMatch);

        /// <summary>
        /// the interceptor should set this priority if it makes any changes to the Route. 
        /// the Customer Routes page relies on this property to show to user the impact of the interceptor on the route
        /// </summary>
        string ChangesIdentifier { set; }
    }
}
