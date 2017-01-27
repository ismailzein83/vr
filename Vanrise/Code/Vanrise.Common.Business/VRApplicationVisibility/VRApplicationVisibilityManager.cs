using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRApplicationVisibilityManager
    {
        #region Public Methods

        public T GetModuleVisibility<T>() where T : VRModuleVisibility
        {
            throw new NotImplementedException();
        }

        public VRApplicationVisibility GetVRApplicationVisibility(Guid vrApplicationVisibilityId)
        {
            Dictionary<Guid, VRApplicationVisibility> cachedVRApplicationVisibilities = this.GetCachedVRApplicationVisibilities();
            return cachedVRApplicationVisibilities.GetRecord(vrApplicationVisibilityId);
        }

        public VRApplicationVisibilityEditorRuntime GetVRApplicationVisibilityEditorRuntime(Guid vrApplicationVisibilityId)
        {
            var editorRuntime = new VRApplicationVisibilityEditorRuntime();
            editorRuntime.ModulesVisibilityEditorRuntime = new Dictionary<Guid, VRModuleVisibilityEditorRuntime>();

            editorRuntime.Entity = GetVRApplicationVisibility(vrApplicationVisibilityId);
            if (editorRuntime.Entity.Settings == null)
                throw new NullReferenceException(string.Format("vrApplicationVisibility.Settings of vrApplicationVisibilityId {0}", vrApplicationVisibilityId));

            foreach (var moduleVisibility in editorRuntime.Entity.Settings.ModulesVisibility)
                editorRuntime.ModulesVisibilityEditorRuntime.Add(moduleVisibility.Key, moduleVisibility.Value.GetEditorRuntime());

            return editorRuntime;
        }

        public IDataRetrievalResult<VRApplicationVisibilityDetail> GetFilteredVRApplicationVisibilities(DataRetrievalInput<VRApplicationVisibilityQuery> input)
        {
            var allVRApplicationVisibilities = this.GetCachedVRApplicationVisibilities();
            Func<VRApplicationVisibility, bool> filterExpression = (x) => (input.Query.Name == null || x.Name.ToLower().Contains(input.Query.Name.ToLower()));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRApplicationVisibilities.ToBigResult(input, filterExpression, VRApplicationVisibilityDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<VRApplicationVisibilityDetail> AddVRApplicationVisibility(VRApplicationVisibility vrApplicationVisibilityItem)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRApplicationVisibilityDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRApplicationVisibilityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRApplicationVisibilityDataManager>();

            vrApplicationVisibilityItem.VRApplicationVisibilityId = Guid.NewGuid();

            if (dataManager.Insert(vrApplicationVisibilityItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRApplicationVisibilityDetailMapper(vrApplicationVisibilityItem);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<VRApplicationVisibilityDetail> UpdateVRApplicationVisibility(VRApplicationVisibility vrApplicationVisibilityItem)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRApplicationVisibilityDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRApplicationVisibilityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRApplicationVisibilityDataManager>();

            if (dataManager.Update(vrApplicationVisibilityItem))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRApplicationVisibilityDetailMapper(this.GetVRApplicationVisibility(vrApplicationVisibilityItem.VRApplicationVisibilityId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public IEnumerable<VRModuleVisibilityConfig> GetVRModuleVisibilityExtensionConfigs()
        {
            var templateConfigManager = new ExtensionConfigurationManager();
            return templateConfigManager.GetExtensionConfigurations<VRModuleVisibilityConfig>(VRModuleVisibilityConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRApplicationVisibilityDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRApplicationVisibilityDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRApplicationVisibilityUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        Dictionary<Guid, VRApplicationVisibility> GetCachedVRApplicationVisibilities()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetVRApplicationVisibilities",
               () =>
               {
                   IVRApplicationVisibilityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRApplicationVisibilityDataManager>();
                   return dataManager.GetVRApplicationVisibilities().ToDictionary(x => x.VRApplicationVisibilityId, x => x);
               });
        }

        #endregion

        #region Mappers

        public VRApplicationVisibilityDetail VRApplicationVisibilityDetailMapper(VRApplicationVisibility vrApplicationVisibility)
        {
            VRApplicationVisibilityDetail vrApplicationVisibilityDetail = new VRApplicationVisibilityDetail()
            {
                Entity = vrApplicationVisibility
            };
            return vrApplicationVisibilityDetail;
        }

        #endregion
    }
}
