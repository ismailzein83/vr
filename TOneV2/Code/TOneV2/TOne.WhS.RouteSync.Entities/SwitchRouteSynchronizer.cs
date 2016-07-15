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

        public abstract void ConvertRoute(ISwitchRouteSynchronizerConvertRouteContext context);

        public abstract void UpdateConvertedRoute(ISwitchRouteSynchronizerUpdateConvertedRouteContext context);

        public abstract void Finalize(ISwitchRouteSynchronizerFinalizeContext context);
    }

    public interface ISwitchRouteSynchronizerInitializeContext
    {
        Object InitializationData { set; }
    }

    public interface ISwitchRouteSynchronizerConvertRouteContext
    {
        Object InitializationData { get; }

        Route Route { get; }

        Object ConvertedRoute { set; }
    }

    public interface ISwitchRouteSynchronizerUpdateConvertedRouteContext
    {
        Object InitializationData { get; }

        Object ConvertedRoute { get; }
    }

    public interface ISwitchRouteSynchronizerFinalizeContext
    {
        Object InitializationData { get; }
    }
}
