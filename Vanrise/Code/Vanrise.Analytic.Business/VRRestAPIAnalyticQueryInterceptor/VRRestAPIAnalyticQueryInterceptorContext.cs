using System;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class VRRestAPIAnalyticQueryInterceptorContext : IVRRestAPIAnalyticQueryInterceptorContext
    {
        public Guid VRConnectionId { get; set; }
        public AnalyticQuery Query { get; set; }
    }
}
