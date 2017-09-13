using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Idb
{
    public abstract class IdbConvertedRoute : ConvertedRoute
    {
        public string Pref { get; set; }
        public string Route { get; set; }
    }
}