using System;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;
using Vanrise.Data.RDB;

namespace TOne.WhS.RouteSync.Huawei.RDB
{
    class WhSRouteSyncHuaweiDataManager : IWhSRouteSyncHuaweiDataManager
    {
        public string SwitchId { get; set; }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("RouteSync", "TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString");
        }

        public void Initialize(IWhSRouteSyncHuaweiInitializeContext context)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            queryContext.AddCreateSchemaIfNotExistsQuery().DBSchemaName($"WhS_RouteSync_Huawei_{SwitchId}");
            queryContext.ExecuteNonQuery();
        }

    }
}
