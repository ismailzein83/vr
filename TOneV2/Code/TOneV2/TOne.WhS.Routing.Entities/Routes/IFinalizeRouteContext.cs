
namespace TOne.WhS.Routing.Entities
{
    public interface IFinalizeRouteContext
    {
        bool UpdateLastRouteBuild { get; }

        bool UpdateLastRouteSync { get; }
    }

    public class FinalizeRouteContext : IFinalizeRouteContext
    {
        public bool UpdateLastRouteBuild { get; set; }

        public bool UpdateLastRouteSync { get; set; }
    }
}