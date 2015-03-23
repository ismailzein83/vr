using System;
using System.Collections.Generic;
namespace TOne.LCR.Entities
{
    public interface IRouteBuildContext
    {
        void ExecuteOptionsActions(bool retrieveFromLCR, int? nbOfOptions);
        void BlockRoute();
        //void BuildLCR();
        RouteDetail Route { get; }
       
        RouteSupplierOption GetNextOptionInLCR();

        bool TryBuildSupplierOption(string supplierId, short? percentage, out RouteSupplierOption routeOption);
    }
}
