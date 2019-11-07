using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Cataleya.Entities;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Cataleya.Data
{
    public interface ISyncDataManager
    {
        Guid ConfigId { get; }
        void PrepareTables(IRouteInitializeContext context);
        List<CarrierAccountVersionInfo> GetCarrierAccountsPreviousVersionNumbers(IGetCarrierAccountsPreviousVersionNumbersContext context);
        Object PrepareDataForApply(List<ConvertedRoute> routes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void SwapTables(ISwapTableContext context);
    }
}