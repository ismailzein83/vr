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

        private MediationDefinitionDetail MediationDefinitionDetailMapper(MediationDefinition mediationDefinitionObject)
        {
            MediationDefinitionDetail mediationDefinitionDetail = new MediationDefinitionDetail();
            mediationDefinitionDetail.Entity = mediationDefinitionObject;
            return mediationDefinitionDetail;
        }
        #endregion

    }
}
