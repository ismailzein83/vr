using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Idb
{
    public interface IIdbDataManager
    {
        Guid ConfigId { get; }
        void PrepareTables(ISwitchRouteSynchronizerInitializeContext context);
        Object PrepareDataForApply(List<ConvertedRoute> idbRoutes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void SwapTables(ISwapTableContext context);
    }
}
