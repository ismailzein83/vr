using System.Collections.Generic;

namespace TOne.WhS.Routing.Entities
{
    public interface IRouteOptionSettings
    {
        int SupplierId { get; }

        int NumberOfTries { get; }

        int? Percentage { get; }
    }

    public interface IRouteBackupOptionSettings
    {
        int SupplierId { get; }

        int NumberOfTries { get; }
    }

    public interface IFixedRouteOptionSettings
    {
        List<RouteOptionFilterSettings> Filters { get; set; }

        List<FixedRouteSupplierDeal> SupplierDeals { get; set; }
    }

    public interface ISpecialRequestRouteOptionSettings
    {
        bool ForceOption { get; set; }

        List<SpecialRequestRouteSupplierDeal> SupplierDeals { get; set; }
    }
}