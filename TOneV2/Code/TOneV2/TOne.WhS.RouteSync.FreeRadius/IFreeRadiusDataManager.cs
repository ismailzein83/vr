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
        void ApplyDifferentialRoutes(IFreeRadiusApplyDifferentialRoutesContext context);
    }

    public interface IFreeRadiusApplyDifferentialRoutesContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        List<FreeRadiusConvertedRoute> ConvertedUpdatedRoutes { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }
    }

    public class FreeRadiusApplyDifferentialRoutesContext : IFreeRadiusApplyDifferentialRoutesContext
    {
        public string SwitchId { get; set; }

        public string SwitchName { get; set; }

        public List<FreeRadiusConvertedRoute> ConvertedUpdatedRoutes { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }

        public SwitchSyncOutput SwitchSyncOutput { get; set; }
    }
}