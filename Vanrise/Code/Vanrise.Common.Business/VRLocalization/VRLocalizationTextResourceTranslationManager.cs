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
        
        public VRLocalizationTextResourceTranslation GetVRLocalizationTextResourceTranslation(Guid vrLocalizationTextResourceTranslationId)
        {
            var vrLocalizationTextResources = GetCachedVRLocalizationTextResourcesTranslation();
            return vrLocalizationTextResources.GetRecord(vrLocalizationTextResourceTranslationId);
        }
		
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
						VRLocalizationTextResourceTranslation vrLocalizationTextResourceTranslation = new VRLocalizationTextResourceTranslation()
						{
							VRLocalizationTextResourceTranslationId = (Guid)genericBusinessEntity.FieldValues.GetRecord("ID"),
							ResourceId = (Guid)genericBusinessEntity.FieldValues.GetRecord("TextResourceID"),
							LanguageId = (Guid)genericBusinessEntity.FieldValues.GetRecord("LanguageID"),
							CreatedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("CreatedTime"),
							LastModifiedTime = (DateTime?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedTime"),
							CreatedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("CreatedBy"),
							LastModifiedBy = (int?)genericBusinessEntity.FieldValues.GetRecord("LastModifiedBy"),
							Value = genericBusinessEntity.FieldValues.GetRecord("Value") as string
						};
						result.Add(vrLocalizationTextResourceTranslation.VRLocalizationTextResourceTranslationId, vrLocalizationTextResourceTranslation);
					}
				}
				return result;
			});
		}
		private Dictionary<Guid, VRLocalizationTextResourceTranslationsById> GetCachedResourceTranslationsByLanguageId()
		{
			IGenericBusinessEntityManager genericBusinessEntityManager = Vanrise.GenericData.Entities.BusinessManagerFactory.GetManager<IGenericBusinessEntityManager>();
			return genericBusinessEntityManager.GetCachedOrCreate("GetCachedResourceTranslationsByLanguageId", businessEntityDefinitionId,
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
      
        #endregion

        #region Mapper
        #endregion
    }
}
