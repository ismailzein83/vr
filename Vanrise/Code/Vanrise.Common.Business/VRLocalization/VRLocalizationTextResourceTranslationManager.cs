using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities.VRLocalization;

namespace Vanrise.Common.Business
{
    public class VRLocalizationTextResourceTranslationManager
    {
        #region Public Methods
        public Dictionary<Guid, VRLocalizationTextResourceTranslationsById> GetAllResourceTranslationsByLanguageId()
        {
            throw new NotImplementedException();
        }
        public IDataRetrievalResult<VRLocalizationTextResourceTranslationDetail> GetFilteredVRLocalizationTextResourcesTranslation(DataRetrievalInput<VRLocalizationTextResourceTranslationQuery> input)
        {
            var allVRLocalizationTextResourcesTranslation = GetCachedVRLocalizationTextResourcesTranslation();
            Func<VRLocalizationTextResourceTranslation, bool> filterExpression = (item) =>
            {
                if (input.Query.ResourceIds != null && !input.Query.ResourceIds.Contains(item.ResourceId))
                    return false;
                if (input.Query.LanguageIds != null && !input.Query.LanguageIds.Contains(item.LanguageId))
                    return false;
                return true;
            };
            var asd = Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationTextResourcesTranslation.ToBigResult(input, filterExpression, VRLocalizationTextResourceTranslationDetailMapper));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationTextResourcesTranslation.ToBigResult(input, filterExpression, VRLocalizationTextResourceTranslationDetailMapper));
        }
        public VRLocalizationTextResourceTranslation GetVRLocalizationTextResourceTranslation(Guid vrLocalizationTextResourceTranslationId)
        {
            var vrLocalizationTextResources = GetCachedVRLocalizationTextResourcesTranslation();
            if (vrLocalizationTextResources == null)
                return null;
            return vrLocalizationTextResources.GetRecord(vrLocalizationTextResourceTranslationId);
        }
        public InsertOperationOutput<VRLocalizationTextResourceTranslationDetail> AddVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslation)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRLocalizationTextResourceTranslationDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IVRLocalizationTextResourceTranslationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceTranslationDataManager>();

            vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId = Guid.NewGuid();

            if (dataManager.AddVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = VRLocalizationTextResourceTranslationDetailMapper(vrLocalizationTextResourceTranslation);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public UpdateOperationOutput<VRLocalizationTextResourceTranslationDetail> UpdateVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslation)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRLocalizationTextResourceTranslationDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IVRLocalizationTextResourceTranslationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceTranslationDataManager>();

            if (dataManager.UpdateVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation))
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = VRLocalizationTextResourceTranslationDetailMapper(this.GetVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId));
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, VRLocalizationTextResourceTranslation> GetCachedVRLocalizationTextResourcesTranslation()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedVRLocalizationTextResources",
               () =>
               {
                   IVRLocalizationTextResourceTranslationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceTranslationDataManager>();
                   IEnumerable<VRLocalizationTextResourceTranslation> vrLocalizationTextResourcesTranslation = dataManager.GetVRLocalizationTextResourcesTranslation();
                   return vrLocalizationTextResourcesTranslation.ToDictionary(itm => itm.VRLocalizationTextResourceTranslationId, itm => itm);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IVRLocalizationTextResourceTranslationDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceTranslationDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreVRLocalizationTextResourcesTranslationUpdated(ref _updateHandle);
            }
        }
        #endregion

        #region Mapper
        VRLocalizationTextResourceTranslationDetail VRLocalizationTextResourceTranslationDetailMapper(VRLocalizationTextResourceTranslation localizationTextResourceTranslation)
        {
            VRLocalizationLanguageManager languageManager = new VRLocalizationLanguageManager();
            VRLocalizationTextResourceManager textResourceManager = new VRLocalizationTextResourceManager();
            VRLocalizationTextResourceTranslationDetail vrLocalizationTextResourceDetail = new VRLocalizationTextResourceTranslationDetail
            {
                VRLocalizationTextResourceTranslationId = localizationTextResourceTranslation.VRLocalizationTextResourceTranslationId,
                ResourceId = localizationTextResourceTranslation.ResourceId,
                LanguageId = localizationTextResourceTranslation.LanguageId,
                Settings = localizationTextResourceTranslation.Settings,
            };
            if (vrLocalizationTextResourceDetail.ResourceId != null)
                vrLocalizationTextResourceDetail.ResourceKey = textResourceManager.GetVRLocalizationResourceKey(vrLocalizationTextResourceDetail.ResourceId);
            if (vrLocalizationTextResourceDetail.LanguageId != null)
                vrLocalizationTextResourceDetail.languageName = languageManager.GetVRLocalizationLanguageName(vrLocalizationTextResourceDetail.LanguageId);
            return vrLocalizationTextResourceDetail;
        }
        #endregion
    }
}
