using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Data;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Business
{
    public class RouteSyncDefinitionManager : IRouteSyncDefinitionManager
    {
        #region Public Methods
        public RouteSyncBPDefinitionSettings GetRouteSyncBPDefinitionSettings(int routeSyncDefinitionId)
        {
            throw new NotImplementedException();
        }

        public RouteSyncDefinition GetRouteSyncDefinitionById(int routeSyncDefinitionId)
        {
            RouteSyncDefinition routeSyncDefinition;
            var allRouteSyncDefinitions = GetCachedRouteSyncDefinitions();
            if (!allRouteSyncDefinitions.TryGetValue(routeSyncDefinitionId, out routeSyncDefinition))
                throw new NullReferenceException(string.Format("GetRouteSyncDefinitionById : {0}", routeSyncDefinitionId));
            return allRouteSyncDefinitions[routeSyncDefinitionId];
        }

        public Vanrise.Entities.InsertOperationOutput<RouteSyncDefinitionDetail> AddRouteSyncDefinition(RouteSyncDefinition routeSyncDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<RouteSyncDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int reprocessDefintionId = -1;

            IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();

            if (dataManager.Insert(routeSyncDefinitionItem, out reprocessDefintionId))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                routeSyncDefinitionItem.RouteSyncDefinitionId = reprocessDefintionId;
                insertOperationOutput.InsertedObject = RouteSyncDefinitionDetailMapper(routeSyncDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<RouteSyncDefinitionDetail> UpdateRouteSyncDefinition(RouteSyncDefinition routeSyncDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<RouteSyncDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();

            if (dataManager.Update(routeSyncDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = RouteSyncDefinitionDetailMapper(this.GetRouteSyncDefinitionById(routeSyncDefinitionItem.RouteSyncDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IDataRetrievalResult<RouteSyncDefinitionDetail> GetFilteredRouteSyncDefinitions(DataRetrievalInput<RouteSyncDefinitionQuery> input)
        {
            var allRouteSyncDefinitions = GetCachedRouteSyncDefinitions();
            Func<RouteSyncDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allRouteSyncDefinitions.ToBigResult(input, filterExpression, RouteSyncDefinitionDetailMapper));
        }

        public IEnumerable<RouteReaderConfig> GetRouteReaderExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<RouteReaderConfig>(RouteReaderConfig.EXTENSION_TYPE);
        }

        public IEnumerable<RouteSyncDefinitionInfo> GetRouteSyncDefinitionsInfo()
        {
            return GetCachedRouteSyncDefinitions().MapRecords(RouteSyncDefinitionInfoMapper).OrderBy(d => d.Name);
        }
        #endregion
         

        #region Private Classes
        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRouteSyncDefinitionDataManager _dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreRouteSyncDefinitionsUpdated(ref _updateHandle);
            }
        }
        #endregion


        #region Private Methods
        Dictionary<int, RouteSyncDefinition> GetCachedRouteSyncDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetRouteSyncDefinitions",
               () =>
               {
                   IRouteSyncDefinitionDataManager dataManager = RouteSyncDataManagerFactory.GetDataManager<IRouteSyncDefinitionDataManager>();
                   IEnumerable<RouteSyncDefinition> routeSyncDefinitions = dataManager.GetRouteSyncDefinitions();
                   return routeSyncDefinitions.ToDictionary(rsd => rsd.RouteSyncDefinitionId, rsd => rsd);
               });
        }
        #endregion


        #region Mappers
        public RouteSyncDefinitionDetail RouteSyncDefinitionDetailMapper(RouteSyncDefinition routeSyncDefinition)
        {
            RouteSyncDefinitionDetail routeSyncDefinitionDetail = new RouteSyncDefinitionDetail()
            {
                Entity = routeSyncDefinition
            };
            return routeSyncDefinitionDetail;
        }

        RouteSyncDefinitionInfo RouteSyncDefinitionInfoMapper(RouteSyncDefinition routeSyncDefinition)
        {
            return new RouteSyncDefinitionInfo
            {
                Name = routeSyncDefinition.Name,
                RouteSyncDefinitionId = routeSyncDefinition.RouteSyncDefinitionId
            };
        }
        #endregion
    }
}
