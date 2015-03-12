using System;
using System.Collections.Generic;
namespace TOne.LCR.Entities
{
    public interface IRouteBuildContext
    {
        void ExecuteOptionsActions(bool retrieveFromLCR, int? nbOfOptions, bool onlyImportantFilters);
        void BlockRoute();
        //void BuildLCR();
        RouteDetail Route { get; }
        CodeMatchesBySupplierId CodeMatchesBySupplierId { get; }

        SupplierZoneRates SupplierZoneRates { get; }

        bool TryBuildSupplierOption(string supplierId, short? percentage, out RouteSupplierOption routeOption);
    }
}
