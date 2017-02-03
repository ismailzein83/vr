using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Data;
using Mediation.Generic.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;

namespace Mediation.Generic.Business
{
    public class MediationDefinitionManager
    {
        public MediationDefinition GetMediationDefinition(int mediationDefinitionId)
        {
            var mediationDefinitions = GetCachedMediationDefinitions();
            return mediationDefinitions.GetRecord(mediationDefinitionId);
        }

        public IDataRetrievalResult<MediationDefinitionDetail> GetFilteredMediationDefinitions(DataRetrievalInput<MediationDefinitionQuery> input)
        {
            var allItems = GetCachedMediationDefinitions();

            Func<MediationDefinition, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, MediationDefinitionDetailMapper));
        }

        public Vanrise.Entities.UpdateOperationOutput<MediationDefinitionDetail> UpdateMediationDefinition(MediationDefinition mediationDefinition)
        {
            IMediationDefinitionDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationDefinitionDataManager>();
            bool updateActionSucc = dataManager.UpdateMediationDefinition(mediationDefinition);
            UpdateOperationOutput<MediationDefinitionDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<MediationDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = MediationDefinitionDetailMapper(mediationDefinition);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.InsertOperationOutput<MediationDefinitionDetail> AddMediationDefinition(MediationDefinition mediationDefinition)
        {
            InsertOperationOutput<MediationDefinitionDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<MediationDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int mediationId = -1;


            IMediationDefinitionDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationDefinitionDataManager>();
            bool insertActionSucc = dataManager.AddMediationDefinition(mediationDefinition, out mediationId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                mediationDefinition.MediationDefinitionId = mediationId;
                insertOperationOutput.InsertedObject = MediationDefinitionDetailMapper(mediationDefinition);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public IEnumerable<MediationDefinitionInfo> GetMediationDefinitionInfo()
        {
            var mediationDefinitions = GetCachedMediationDefinitions();
            return mediationDefinitions.MapRecords(MediationDefinitionInfoMapper).OrderBy(x => x.Name); 
        }

        public IEnumerable<MediationDefinitionInfo> GetMediationDefinitionInfoByIds(HashSet<int> mediationDefinitionIds)
        {
            var mediationDefinitions = GetCachedMediationDefinitions();
            Func<MediationDefinition, bool> mediationDefinitionFilter = (mediationDefinition) =>
            {
                if (!mediationDefinitionIds.Contains(mediationDefinition.MediationDefinitionId))
                    return false;
                return true;
            };
            return mediationDefinitions.MapRecords(MediationDefinitionInfoMapper, mediationDefinitionFilter).OrderBy(x => x.Name); ;
        }

        public IEnumerable<MediationOutputHandlerConfig> GetMediationHandlerConfigTypes()
        {
            return new ExtensionConfigurationManager().GetExtensionConfigurations<MediationOutputHandlerConfig>(MediationOutputHandlerConfig.EXTENSION_TYPE);
        }
        #region Private Methods
        private Dictionary<int, MediationDefinition> GetCachedMediationDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetMediationDefinitions",
               () =>
               {
                   IMediationDefinitionDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationDefinitionDataManager>();
                   IEnumerable<MediationDefinition> summaryTransformationDefinitions = dataManager.GetMediationDefinitions();
                   return summaryTransformationDefinitions.ToDictionary(kvp => kvp.MediationDefinitionId, kvp => kvp);
               });
        }

        #endregion

        #region Private Classes
        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IMediationDefinitionDataManager _dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreMediationDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mappers

        MediationDefinitionDetail MediationDefinitionDetailMapper(MediationDefinition mediationDefinitionObject)
        {
            MediationDefinitionDetail mediationDefinitionDetail = new MediationDefinitionDetail();
            mediationDefinitionDetail.Entity = mediationDefinitionObject;
            return mediationDefinitionDetail;
        }

        MediationDefinitionInfo MediationDefinitionInfoMapper(MediationDefinition mediationDefinitionObject)
        {
            return new MediationDefinitionInfo()
            {
                MediationDefinitionId = mediationDefinitionObject.MediationDefinitionId,
                Name = mediationDefinitionObject.Name
            };
        }
        #endregion

    }
}
