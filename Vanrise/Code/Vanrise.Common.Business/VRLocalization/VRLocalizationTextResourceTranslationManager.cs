using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Entities;
using Vanrise.Entities.VRLocalization;
using Vanrise.GenericData.Entities;

namespace Vanrise.Common.Business
{
    public class VRLocalizationTextResourceTranslationManager
    {
		#region Public Methods
		static Guid businessEntityDefinitionId = new Guid("d059c743-f131-40bf-bd90-53884eb10325");
		public Dictionary<Guid, VRLocalizationTextResourceTranslationsById> GetAllResourceTranslationsByLanguageId()
        {
            return GetCachedResourceTranslationsByLanguageId();
        }
        //public IDataRetrievalResult<VRLocalizationTextResourceTranslationDetail> GetFilteredVRLocalizationTextResourcesTranslation(DataRetrievalInput<VRLocalizationTextResourceTranslationQuery> input)
        //{
        //    var allVRLocalizationTextResourcesTranslation = GetCachedVRLocalizationTextResourcesTranslation();
        //    Func<VRLocalizationTextResourceTranslation, bool> filterExpression = (item) =>
        //    {
        //        if (input.Query.ResourceIds != null && !input.Query.ResourceIds.Contains(item.ResourceId))
        //            return false;
        //        if (input.Query.LanguageIds != null && !input.Query.LanguageIds.Contains(item.LanguageId))
        //            return false;
        //        return true;
        //    };
        //    var asd = Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationTextResourcesTranslation.ToBigResult(input, filterExpression, VRLocalizationTextResourceTranslationDetailMapper));
        //    return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allVRLocalizationTextResourcesTranslation.ToBigResult(input, filterExpression, VRLocalizationTextResourceTranslationDetailMapper));
        //}
        public VRLocalizationTextResourceTranslation GetVRLocalizationTextResourceTranslation(Guid vrLocalizationTextResourceTranslationId)
        {
            var vrLocalizationTextResources = GetCachedVRLocalizationTextResourcesTranslation();
            return vrLocalizationTextResources.GetRecord(vrLocalizationTextResourceTranslationId);
        }
        //public InsertOperationOutput<VRLocalizationTextResourceTranslationDetail> AddVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslation)
        //{
        //    var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<VRLocalizationTextResourceTranslationDetail>();

        //    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
        //    insertOperationOutput.InsertedObject = null;

        //    IVRLocalizationTextResourceTranslationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceTranslationDataManager>();

        //    vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId = Guid.NewGuid();

        //    if (dataManager.AddVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation))
        //    {
        //        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
        //        insertOperationOutput.InsertedObject = VRLocalizationTextResourceTranslationDetailMapper(vrLocalizationTextResourceTranslation);
        //    }
        //    else
        //    {
        //        insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
        //    }

        //    return insertOperationOutput;
        //}
        //public UpdateOperationOutput<VRLocalizationTextResourceTranslationDetail> UpdateVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslation)
        //{
        //    var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<VRLocalizationTextResourceTranslationDetail>();

        //    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
        //    updateOperationOutput.UpdatedObject = null;

        //    IVRLocalizationTextResourceTranslationDataManager dataManager = CommonDataManagerFactory.GetDataManager<IVRLocalizationTextResourceTranslationDataManager>();

        //    if (dataManager.UpdateVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation))
        //    {
        //        Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
        //        updateOperationOutput.UpdatedObject = VRLocalizationTextResourceTranslationDetailMapper(this.GetVRLocalizationTextResourceTranslation(vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId));
        //    }
        //    else
        //    {
        //        updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
        //    }

        //    return updateOperationOutput;
        //}

        #endregion

        #region Private Methods
        private Dictionary<Guid, VRLocalizationTextResourceTranslation> GetCachedVRLocalizationTextResourcesTranslation()
        {
			IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
			return genericBusinessEntityManager.GetCachedOrCreate("GetCachedVRLocalizationtextResourceTranslation", businessEntityDefinitionId, () =>
			{
				Dictionary<Guid, VRLocalizationTextResourceTranslation> result = new Dictionary<Guid, VRLocalizationTextResourceTranslation>();
				IEnumerable<GenericBusinessEntity> genericBusinessEntities = genericBusinessEntityManager.GetAllGenericBusinessEntities(businessEntityDefinitionId);
				if (genericBusinessEntities != null)
				{
					foreach (GenericBusinessEntity genericBusinessEntity in genericBusinessEntities)
					{
						VRLocalizationTextResourceTranslationSettings vrLocalizationTextResourceTranslationSettings = new VRLocalizationTextResourceTranslationSettings();
						vrLocalizationTextResourceTranslationSettings.Value = genericBusinessEntity.FieldValues.GetRecord("Value") as string;
						VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslation = new VRLocalizationTextResourceTranslation()
						{
							VRLocalizationTextResourceTranslationId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
							ResourceId = (Guid)genericBusinessEntity.FieldValues.GetRecord("TextResourceID"),
							LanguageId = (Guid)genericBusinessEntity.FieldValues.GetRecord("LanguageID"),
							Settings = vrLocalizationTextResourceTranslationSettings,
							CreatedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
							LastModifiedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
							CreatedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
							LastModifiedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
						};
						result.Add(vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId, vrLocalizationTextResourceTranslation);
					}
				}
				return result;
			});
		}
		private Dictionary<Guid, VRLocalizationTextResourceTranslationsById> GetCachedResourceTranslationsByLanguageId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedResourceTranslationsByLanguageId",
               () =>
               {
                   Dictionary<Guid, VRLocalizationTextResourceTranslationsById> allResourceTranslationsByLanguageId = null;
                   var allResourcesTranslations = GetCachedVRLocalizationTextResourcesTranslation();
                   if (allResourcesTranslations != null)
                   {
                       allResourceTranslationsByLanguageId = new Dictionary<Guid, VRLocalizationTextResourceTranslationsById>();
                       foreach (var resourcesTranslation in allResourcesTranslations)
                       {
                           var vrLocalizationTextResourceTranslationsById = allResourceTranslationsByLanguageId.GetOrCreateItem(resourcesTranslation.Value.LanguageId);
                           if (!vrLocalizationTextResourceTranslationsById.ContainsKey(resourcesTranslation.Value.ResourceId))
                           {
                               vrLocalizationTextResourceTranslationsById.Add(resourcesTranslation.Value.ResourceId, resourcesTranslation.Value);
                           }
                       }
                   }
                   return allResourceTranslationsByLanguageId;
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
                vrLocalizationTextResourceDetail.ResourceKey = textResourceManager.GetVRLocalizationResourceKey(vrLocalizationTextResourceDetail.ResourceId);
                vrLocalizationTextResourceDetail.languageName = languageManager.GetVRLocalizationLanguageName(vrLocalizationTextResourceDetail.LanguageId);
            return vrLocalizationTextResourceDetail;
        }
        #endregion
    }
}
