using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Radius
{
    public abstract class RadiusConvertedRoute : ConvertedRoute
    {
        public string CustomerId { get; set; }
        public string Code { get; set; }
        public string Options { get; set; }
    }
}
