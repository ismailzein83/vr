using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class VRLocalizationTextResourceManager
    {
        #region public methods
        public IDataRetrievalResult<VRLocalizationTextResourceDetail> GetFilteredVRLocalizationTextResources(DataRetrievalInput<VRLocalizationTextResourceQuery> input)
        {
            var allVRLocalizationTextResources = GetCachedVRLocalizationTextResources();
            Func<VRLocalizationTextResource, bool> filterExpression = (item) =>
            {
                if (input.Query.ResourceKey != null && !item.ResourceKey.ToLower().Contains(input.Query.ResourceKey.ToLower()))
                    return false;
                return true;
            };
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationTextResources.ToBigResult(input, filterExpression, VRLocalizationTextResourceDetailMapper));
        }

        public VRLocalizationTextResource GetVRLocalizationTextResource(Guid vrLocalizationTextResourceId)
        {
            var vrLocalizationTextResources = GetCachedVRLocalizationTextResources();
            if (vrLocalizationTextResources == null)
                return null;
            return vrLocalizationTextResources.GetRecord(vrLocalizationTextResourceId);
        }
        public InsertOperationOutput<VRLocalizationTextResourceDetail> AddVRLocalizationTextResource(VRLocalizationTextResource vrLocalizationTextResource)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRLocalizationTextResourceDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRLocalizationTextResourceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();

            vrLocalizationTextResource.VRLocalizationTextResourceId = Guid.NewGuid();

            if (dataManager.Insert(vrLocalizationTextResource))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRLocalizationTextResourceDetailMapper(vrLocalizationTextResource);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public UpdateOperationOutput<VRLocalizationTextResourceDetail> UpdateVRLocalizationTextResource(VRLocalizationTextResource vrLocalizationTextResource)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRLocalizationTextResourceDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRLocalizationTextResourceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();

            if (dataManager.Update(vrLocalizationTextResource))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRLocalizationTextResourceDetailMapper(this.GetVRLocalizationTextResource(vrLocalizationTextResource.VRLocalizationTextResourceId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Dictionary<Guid, VRLocalizationTextResource> GetAllResources()
        {
            return GetCachedVRLocalizationTextResources();
        }

        #endregion

        #region private methods
        private Dictionary<Guid, VRLocalizationTextResource> GetCachedVRLocalizationTextResources()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRLocalizationTextResources",
               () =>
               {
                   IVRLocalizationTextResourceDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();
                   IEnumerable<VRLocalizationTextResource> vrLocalizationTextResources = dataManager.GetVRLocalizationTextResources();
                   return vrLocalizationTextResources.ToDictionary(itm => itm.VRLocalizationTextResourceId, itm => itm);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRLocalizationTextResourceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRLocalizationTextResourcesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Mapper
        VRLocalizationTextResourceDetail VRLocalizationTextResourceDetailMapper(VRLocalizationTextResource localizationTextResource)
        {

            VRLocalizationTextResourceDetail vrLocalizationTextResourceDetail = new Entities.VRLocalizationTextResourceDetail
            {
                VRLocalizationTextResourceId = localizationTextResource.VRLocalizationTextResourceId,
                ResourceKey = localizationTextResource.ResourceKey,
                ModuleId = localizationTextResource.ModuleId
            };
            VRLocalizationModuleManager vrLocalizationModuleManager = new VRLocalizationModuleManager();
            vrLocalizationTextResourceDetail.ModuleName = vrLocalizationModuleManager.GetVRModuleName(localizationTextResource.ModuleId);
            return vrLocalizationTextResourceDetail;
        }
        #endregion
    }
}
