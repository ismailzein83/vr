using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business
{
    public class BELookupRuleDefinitionManager
    {
        #region Fields

        BusinessEntityDefinitionManager _beDefinitionManager = new BusinessEntityDefinitionManager();

        #endregion

        #region Public Methods

        public IDataRetrievalResult<BELookupRuleDefinitionDetail> GetFilteredBELookupRuleDefinitions(DataRetrievalInput<BELookupRuleDefinitionQuery> input)
        {
            Dictionary<Guid, BELookupRuleDefinition> cachedEntities = this.GetCachedBELookupRuleDefinitions();

            Func<BELookupRuleDefinition, bool> filterExpression = (itm) =>
                (input.Query.Name == null || itm.Name.ToLower().Contains(input.Query.Name.ToLower())) &&
                (input.Query.BusinessEntityDefinitionIds == null || input.Query.BusinessEntityDefinitionIds.Contains(itm.BusinessEntityDefinitionId));

            return DataRetrievalManager.Instance.ProcessResult(input, cachedEntities.ToBigResult(input, filterExpression, BELookupRuleDefinitionDetailMapper));
        }

        public IEnumerable<BELookupRuleDefinitionInfo> GetBELookupRuleDefinitionsInfo(BELookupRuleDefinitionFilter filter)
        {
            return this.GetCachedBELookupRuleDefinitions().MapRecords(BELookupRuleDefinitionInfoMapper).OrderBy(x => x.Name);
        }

        public BELookupRuleDefinition GetBELookupRuleDefinition(Guid beLookupRuleDefinitionId)
        {
            Dictionary<Guid, BELookupRuleDefinition> cachedEntities = this.GetCachedBELookupRuleDefinitions();
            return cachedEntities.GetRecord(beLookupRuleDefinitionId);
        }

        public Vanrise.Entities.InsertOperationOutput<BELookupRuleDefinitionDetail> AddBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BELookupRuleDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IBELookupRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBELookupRuleDefinitionDataManager>();
            beLookupRuleDefinition.BELookupRuleDefinitionId = Guid.NewGuid();

            if (dataManager.InsertBELookupRuleDefinition(beLookupRuleDefinition))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = BELookupRuleDefinitionDetailMapper(beLookupRuleDefinition);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<BELookupRuleDefinitionDetail> UpdateBELookupRuleDefinition(BELookupRuleDefinition beLookupRuleDefinition)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<BELookupRuleDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IBELookupRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBELookupRuleDefinitionDataManager>();

            if (dataManager.UpdateBELookupRuleDefinition(beLookupRuleDefinition))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = BELookupRuleDefinitionDetailMapper(beLookupRuleDefinition);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Classes

        internal class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IBELookupRuleDefinitionDataManager _dataManager = GenericDataDataManagerFactory.GetDataManager<IBELookupRuleDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreBELookupRuleDefinitionsUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, BELookupRuleDefinition> GetCachedBELookupRuleDefinitions()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetBELookupRuleDefinitions", () =>
            {
                IBELookupRuleDefinitionDataManager dataManager = GenericDataDataManagerFactory.GetDataManager<IBELookupRuleDefinitionDataManager>();
                IEnumerable<BELookupRuleDefinition> beLookupRuleDefinitions = dataManager.GetBELookupRuleDefinitions();
                return beLookupRuleDefinitions.ToDictionary(kvp => kvp.BELookupRuleDefinitionId, kvp => kvp);
            });
        }

        #endregion

        #region Mappers

        BELookupRuleDefinitionDetail BELookupRuleDefinitionDetailMapper(BELookupRuleDefinition beLookupDefinition)
        {
            var detail = new BELookupRuleDefinitionDetail();
            detail.Entity = beLookupDefinition;
            detail.BusinessEntityDefinitionName = _beDefinitionManager.GetBusinessEntityDefinitionName(beLookupDefinition.BusinessEntityDefinitionId);
            return detail;
        }

        BELookupRuleDefinitionInfo BELookupRuleDefinitionInfoMapper(BELookupRuleDefinition beLookupRuleDefinition)
        {
            return new BELookupRuleDefinitionInfo()
            {
                BELookupRuleDefinitionId = beLookupRuleDefinition.BELookupRuleDefinitionId,
                Name = beLookupRuleDefinition.Name
            };
        }

        #endregion
    }
}
