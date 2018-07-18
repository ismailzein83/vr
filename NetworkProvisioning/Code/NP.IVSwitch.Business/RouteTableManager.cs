using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;
using NP.IVSwitch.Entities.RouteTable;
using Vanrise.Rules;
using Vanrise.Caching;

namespace NP.IVSwitch.Business
{
    public class RouteTableManager
    {
        #region Public Methods
        public IDataRetrievalResult<RouteTableDetails> GetFilteredRouteTables(DataRetrievalInput<RouteTableQuery> input)
        {
            var allRouteTables = GetCachedRouteTables();
            Func<RouteTable, bool> filterExpression = (routeTable) =>
            {
                if (input.Query.Name != null && !routeTable.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    return false;

                return true;
            };
            return DataRetrievalManager.Instance.ProcessResult(input, allRouteTables.ToBigResult(input, filterExpression, RouteTableDetailMapper));

        }

        public InsertOperationOutput<RouteTableDetails> AddRouteTable(RouteTableInput routeTableItem)
        {
            IRouteTableDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(routeTableDataManager);
            
            InsertOperationOutput<RouteTableDetails> insertOperationOutput = new InsertOperationOutput<RouteTableDetails>();
            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int routeTableId = -1;

            bool insertActionSuccess = routeTableDataManager.Insert(routeTableItem, out routeTableId);
            if (insertActionSuccess)
            {
                routeTableItem.RouteTable.RouteTableId = routeTableId;
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = RouteTableDetailMapper(routeTableItem.RouteTable);
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }
            return insertOperationOutput;
        }


        public UpdateOperationOutput<RouteTableDetails> UpdateRouteTable(RouteTableInput routeTableItem)
        {
            IRouteTableDataManager routeTableDataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(routeTableDataManager);

            UpdateOperationOutput<RouteTableDetails> updateOperationOutput = new UpdateOperationOutput<RouteTableDetails>();
            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSuccess = routeTableDataManager.Update(routeTableItem);
            if (updateActionSuccess)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RouteTableDetailMapper(routeTableItem.RouteTable);
            }
            else
            {
                updateOperationOutput.Result = UpdateOperationResult.SameExists;
            }
            return updateOperationOutput;
        }

        public RouteTable GetRouteTableById(int routeTableId)
        {
            return GetCachedRouteTables().GetRecord(routeTableId);
        }

        #endregion

        #region Private Classes
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool IsTimeExpirable { get { return true; } }

        }
        #endregion


        #region Private Methods

        private Dictionary<int, RouteTable> GetCachedRouteTables()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
               .GetOrCreateObject("GetCachedRouteTables", PrepareCachedRouteTables);
        }

        private Dictionary<int, RouteTable> PrepareCachedRouteTables()
        {
            IRouteTableDataManager dataManager = IVSwitchDataManagerFactory.GetDataManager<IRouteTableDataManager>();
            Helper.SetSwitchConfig(dataManager);

            Dictionary<int, RouteTable> routeTables = new Dictionary<int, RouteTable>();

            if (dataManager.IvSwitchSync == null) return routeTables;

            routeTables = dataManager.GetRouteTables().ToDictionary(x => x.RouteTableId, x => x);
            return routeTables;

        }

        #endregion


        #region Mappers
        public RouteTableDetails RouteTableDetailMapper(RouteTable routeTable)
        {
            return new RouteTableDetails
            {
                RouteTableId = routeTable.RouteTableId,
                Name = routeTable.Name,
                Description = routeTable.Description,
                PScore = routeTable.PScore
            };
        }

        #endregion
    }
}
