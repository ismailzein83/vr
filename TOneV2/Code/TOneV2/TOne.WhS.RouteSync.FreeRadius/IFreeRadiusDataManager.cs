using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public interface IFreeRadiusDataManager
    {
        Guid ConfigId { get; }
        void PrepareTables(ISwitchRouteSynchronizerInitializeContext context);
        Object PrepareDataForApply(List<ConvertedRoute> idbRoutes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void SwapTables(ISwapTableContext context);
        void ApplyDifferentialRoutes(IApplyDifferentialRoutesContext context);
    }
}