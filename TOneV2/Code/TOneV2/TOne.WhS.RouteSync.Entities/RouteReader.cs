using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class RouteReader
    {
        public int ConfigId { get; set; }

        public abstract bool TryGetReadRanges(IRouteReaderGetReadRangesContext context);

        public abstract void ReadRoutes(IRouteReaderContext context);
    }

    public interface IRouteReaderGetReadRangesContext
    {
        List<RouteReadRange> Ranges { set; }
    }

    public class RouteReadRange
    {
        public object RangeInfo { get; set; }
    }

    public interface IRouteReaderContext
    {
        void OnRouteReceived(Route route, RouteReceivedAdditionInfo additionalInfo);
    }

    public class RouteReceivedAdditionInfo
    {

    }
}
