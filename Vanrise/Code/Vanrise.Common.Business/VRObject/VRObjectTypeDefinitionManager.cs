using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities;

namespace Vanrise.Common.Business.VRObject
{
    public class VRObjectTypeDefinitionManager
    {
        #region Public Methods

        public VRObjectTypeDefinition GetVRObjectTypeDefinition(Guid styleDefinitionId)
        {
            Dictionary<Guid, VRObjectTypeDefinition> cachedVRObjectTypeDefinitions = this.GetCachedVRObjectTypeDefinitions();
            return cachedVRObjectTypeDefinitions.GetRecord(styleDefinitionId);
        }

        public IDataRetrievalResult<VRObjectTypeDefinitionDetail> GetFilteredVRObjectTypeDefinitions(DataRetrievalInput<VRObjectTypeDefinitionQuery> input)
        {
            var allVRObjectTypeDefinitions = GetCachedVRObjectTypeDefinitions();
            Func<VRObjectTypeDefinition, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRObjectTypeDefinitions.ToBigResult(input, filterExpression, VRObjectTypeDefinitionDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRObjectTypeDefinitionDetail> AddVRObjectTypeDefinition(VRObjectTypeDefinition styleDefinitionItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRObjectTypeDefinitionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();

            styleDefinitionItem.VRObjectTypeDefinitionId = Guid.NewGuid();

            if (dataManager.Insert(styleDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRObjectTypeDefinitionDetailMapper(styleDefinitionItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRObjectTypeDefinitionDetail> UpdateVRObjectTypeDefinition(VRObjectTypeDefinition styleDefinitionItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRObjectTypeDefinitionDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();

            if (dataManager.Update(styleDefinitionItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRObjectTypeDefinitionDetailMapper(this.GetVRObjectTypeDefinition(styleDefinitionItem.VRObjectTypeDefinitionId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRObjectTypeDefinitionInfo> GetVRObjectTypeDefinitionsInfo(StyleDefinitionFilter filter)
        {
            Func<VRObjectTypeDefinition, bool> filterExpression = null;

            return this.GetCachedVRObjectTypeDefinitions().MapRecords(VRObjectTypeDefinitionInfoMapper, filterExpression).OrderBy(x => x.Name);
        }

        #endregion


        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRObjectTypeDefinitionDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRObjectTypeDefinitionUpdated(ref _updateHandle);
            }
        }

        #endregion


        #region Private Methods

        Dictionary<Guid, VRObjectTypeDefinition> GetCachedVRObjectTypeDefinitions()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRObjectTypeDefinitions",
               () =>
               {
                   IVRObjectTypeDefinitionDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRObjectTypeDefinitionDataManager>();
                   return dataManager.GetVRObjectTypeDefinitions().ToDictionary(x => x.VRObjectTypeDefinitionId, x => x);
               });
        }

        #endregion


        #region Mappers

        public VRObjectTypeDefinitionDetail VRObjectTypeDefinitionDetailMapper(VRObjectTypeDefinition styleDefinition)
        {
            VRObjectTypeDefinitionDetail styleDefinitionDetail = new VRObjectTypeDefinitionDetail()
            {
                Entity = styleDefinition
            };
            return styleDefinitionDetail;
        }

        public VRObjectTypeDefinitionInfo VRObjectTypeDefinitionInfoMapper(VRObjectTypeDefinition vrObjectTypeDefinition)
        {
            VRObjectTypeDefinitionInfo vrObjectTypeDefinitionInfo = new VRObjectTypeDefinitionInfo()
            {
                VRObjectTypeDefinitionId = vrObjectTypeDefinition.VRObjectTypeDefinitionId,
                Name = vrObjectTypeDefinition.Name
            };
            return vrObjectTypeDefinitionInfo;
        }

        #endregion
    }
}
