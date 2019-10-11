using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Data
{
    public interface IRouteOptionsDataManager : IBulkApplyDataManager<RouteOptions>, IDataManager
    {
        string SwitchId { get; set; }
        void Initialize(IRouteOptionsInitializeContext context);
        List<RouteOptions> GetAllRoutesOptions();
        Dictionary<string, RouteOptions> GetRouteOptionsAfterRouteNumber(long maxRouteNumber);
        void ApplyRouteOptionsForDB(object preparedRouteOptions);
        List<RouteOptions> GetNotSyncedRoutesOptions();
        void UpdateSyncedRoutesOptions(IEnumerable<long> routeOptionsIds);
    }
}
