using System;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.Business
{
    public class VRRestAPIAnalyticDataProvider : AnalyticDataProvider
    {
        public override Guid ConfigId { get { return new Guid("DD11C35D-3DA8-4094-9492-678D243EFE5A"); } }

        public Guid VRConnectionId { get; set; }

        public Guid RemoteAnalyticTableId { get; set; }

        public VRRestAPIAnalyticQueryInterceptor VRRestAPIAnalyticQueryInterceptor { get; set; }

        public override IAnalyticDataManager CreateDataManager(IAnalyticTableQueryContext queryContext)
        {
            return null;
        }

        public override IRemoteAnalyticDataProvider GetRemoteAnalyticDataProvider(IGetRemoteAnalyticDataProviderContext context)
        {
            return new RemoteAnalyticDataProvider(context.AnalyticTableId);
        }
    }
}
