using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class SwitchRouteSynchronizer
    {
        public int ConfigId { get; set; }

        public abstract void Initialize(ISwitchRouteSynchronizerInitializeContext context);

        public abstract void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context);

        public abstract void UpdateConvertedRoutes(ISwitchRouteSynchronizerUpdateConvertedRoutesContext context);

        public abstract void Finalize(ISwitchRouteSynchronizerFinalizeContext context);
    }

    public enum RouteSyncDeliveryMethod { Batches = 0, AllRoutes = 1 }

    public interface ISwitchRouteSynchronizerInitializeContext
    {
        RouteRangeType? RouteRangeType { get; }

        Object InitializationData { set; }

        RouteSyncDeliveryMethod SupportedDeliveryMethod { set; }
    }
    
    public interface ISwitchRouteSynchronizerConvertRoutesContext
    {
        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        Object InitializationData { get; }

        List<Route> Routes { get; }

        List<Object> ConvertedRoutes { set; }
    }

    public interface ISwitchRouteSynchronizerUpdateConvertedRoutesContext
    {
        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        Object InitializationData { get; }
        
        List<Object> ConvertedRoutes { get; }
    }

    public interface ISwitchRouteSynchronizerFinalizeContext
    {
        RouteRangeType? RouteRangeType { get; }

        Object InitializationData { get; }
    }
}
