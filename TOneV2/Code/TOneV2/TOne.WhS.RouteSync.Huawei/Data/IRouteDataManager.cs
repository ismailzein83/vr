using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data;

namespace TOne.WhS.RouteSync.Huawei.Data
{
    public interface IRouteDataManager : IDataManager, IBulkApplyDataManager<HuaweiConvertedRoute>
    {
        string SwitchId { get; set; }
        void Initialize(IRouteInitializeContext context);
        void CompareTables(IRouteCompareTablesContext context);
        void InsertRoutesToTempTable(IEnumerable<HuaweiConvertedRoute> routes);
        void RemoveRoutesFromTempTable(IEnumerable<HuaweiConvertedRoute> routes);
        void UpdateRoutesInTempTable(IEnumerable<HuaweiConvertedRoute> routes);
        void Finalize(IRouteFinalizeContext context);
    }
}