using System;
namespace TOne.LCR.Entities
{
    public interface IRouteOptionBuildContext
    {
        RouteSupplierOption RouteOption { get; }
        RouteDetail Route { get; }
    }
}
