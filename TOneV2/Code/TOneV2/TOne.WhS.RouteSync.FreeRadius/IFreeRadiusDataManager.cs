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
        void PrepareTables(IFreeRadiusPrepareTablesContext context);
        void BuildSaleCodeZones(IFreeRadiusBuildSaleCodeZonesContext context);
        Object PrepareDataForApply(List<ConvertedRoute> idbRoutes);
        void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context);
        void SwapTables(ISwapTableContext context);
        void ApplyDifferentialRoutes(IFreeRadiusApplyDifferentialRoutesContext context);
    }

    public interface IFreeRadiusPrepareTablesContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        RouteRangeType? RouteRangeType { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        SwitchSyncOutput SwitchSyncOutput { set; }

        SwitchRouteSyncInitializationData InitializationData { set; }
    }
    public class FreeRadiusPrepareTablesContext : IFreeRadiusPrepareTablesContext 
    {
        public string SwitchId { get; set; }

        public string SwitchName { get; set; }

        public RouteRangeType? RouteRangeType { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }

        public SwitchSyncOutput SwitchSyncOutput { get; set; }

        public SwitchRouteSyncInitializationData InitializationData { get; set; }
    }

    public interface IFreeRadiusBuildSaleCodeZonesContext
    {
        string SwitchId { get; }

        string SwitchName { get; }

        Action<Exception, bool> WriteBusinessHandledException { get; }

        SwitchSyncOutput PreviousSwitchSyncOutput { get; }

        List<FreeRadiusSaleCode> FreeRadiusSaleCodes { get;  }

        List<FreeRadiusSaleZone> FreeRadiusSaleZones { get; } 

        SwitchSyncOutput SwitchSyncOutput { set; }
    }
    public class FreeRadiusBuildSaleCodeZonesContext : IFreeRadiusBuildSaleCodeZonesContext
    {
        public string SwitchId { get; set; }

        public string SwitchName { get; set; }

        public Action<Exception, bool> WriteBusinessHandledException { get; set; }

        public SwitchSyncOutput PreviousSwitchSyncOutput { get; set; }

        public List<FreeRadiusSaleCode> FreeRadiusSaleCodes { get; set; }

        public List<FreeRadiusSaleZone> FreeRadiusSaleZones { get; set; } 

        public SwitchSyncOutput SwitchSyncOutput { get; set; }
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