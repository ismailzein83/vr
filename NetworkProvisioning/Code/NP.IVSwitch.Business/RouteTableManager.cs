using NP.IVSwitch.Data;
using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace NP.IVSwitch.Business
{
    public class RouteTableManager
    { 
        #region Public Methods
      
        public IEnumerable<RouteTableInfo> GetRouteTablesInfo(RouteTableFilter filter)
        {
            Func<RouteTable, bool> filterExpression = null;

            return this.GetCachedRouteTable().MapRecords(RouteTableInfoMapper, filterExpression).OrderBy(x => x.RouteTableName);
        }     
 
         

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRouteTableDataManager _dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion

        #region Private Methods

        Dictionary<int, RouteTable> GetCachedRouteTable()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRouteTable",
                () =>
                {
                    IRouteTableDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
                    return dataManager.GetRouteTables().ToDictionary(x => x.RouteTableId, x => x);
                });
        }

        #endregion

        #region Mappers

        public RouteTableInfo RouteTableInfoMapper(RouteTable routeTable)
        {
            RouteTableInfo routeTableInfo = new RouteTableInfo()
            {
                RouteTableId = routeTable.RouteTableId,
                RouteTableName = routeTable.RouteTableName,

            };
            return routeTableInfo;
        }

        #endregion
    }
}
