using System;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.RDB
{
    public class WhSRouteSyncHuaweiDataManager : IWhSRouteSyncHuaweiDataManager
    {
        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        public void Initialize(IWhSRouteSyncHuaweiInitializeContext context)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            queryContext.AddCreateSchemaIfNotExistsQuery().DBSchemaName($"WhS_RouteSync_HuaweiSoftX3000_{SwitchId}");
            queryContext.ExecuteNonQuery();
        }
    }
}
