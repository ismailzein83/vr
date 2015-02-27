using System;
using System.Collections.Generic;
namespace TOne.LCR.Entities
{
    public interface IRouteBuildContext
    {
        void ApplyOptionsFilter(int? nbOfOptions, bool onlyImportantFilters);
        void BlockRoute();
        void BuildLCR();
        RouteDetail Route { get; }
        Dictionary<string, CodeMatch> SuppliersCodeMatches { get; }

        SupplierZoneRates SupplierZoneRates { get; }

        bool TryBuildSupplierOption(string supplierId, short? percentage, out RouteSupplierOption routeOption);
    }
}
