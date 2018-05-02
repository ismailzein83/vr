using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public enum RouteRangeType { ByCustomer = 0, ByCodePrefix = 1, ByCustomerAndCode = 2, Random = 3 }

    public abstract class RouteReader
    {
        public abstract Guid ConfigId { get;}

        public abstract bool TryGetReadRanges(IRouteReaderGetReadRangesContext context);

        public abstract void ReadRoutes(IRouteReaderContext context);
    }

    public interface IRouteReaderGetReadRangesContext
    {
        RouteRangeType RangeType { set; }

        List<RouteRangeInfo> Ranges { set; }
    }

    public class RouteRangeInfo
    {
        public string CustomerId { get; set; }

        public string CodePrefix { get; set; }
    }

    public interface IRouteReaderContext
    {
        RouteRangeType? RouteRangeType { get; }

        RouteRangeInfo RouteRangeInfo { get; }

        bool ShouldStop();

        void OnRouteReceived(Route route, RouteReceivedContext context);
    }

    public class RouteReceivedContext
    {
    }
}
