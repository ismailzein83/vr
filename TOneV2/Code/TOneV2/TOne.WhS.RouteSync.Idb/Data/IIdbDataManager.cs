using System;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.Idb
{
    public interface IIdbDataManager
    {
        Guid ConfigId { get; }
        void PrepareTables(ISwitchRouteSynchronizerInitializeContext context);
        Object PrepareDataForApply(List<ConvertedRoute> idbRoutes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void SwapTables(ISwapTableContext context);
        bool BlockCustomer(IdbBlockCustomerContext context);
        void ApplyDifferentialRoutes(IApplyDifferentialRoutesContext context);
    }
}