using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Analytic.Entities
{
    public abstract class VRRestAPIAnalyticQueryInterceptor
    {
        public abstract Guid ConfigId { get; }
        public abstract void PrepareQuery(IVRRestAPIAnalyticQueryInterceptorContext context);
    }

    public interface IVRRestAPIAnalyticQueryInterceptorContext
    {
        Guid VRConnectionId { get; }
        AnalyticQuery Query { get; set; }
    }
}
