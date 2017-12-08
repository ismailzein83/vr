using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRLocalizationManager
    {
        #region Fields/Ctor

        static VRLocalizationLanguageManager s_languageManager = new VRLocalizationLanguageManager();
        static VRLocalizationTextResourceManager s_textResourceManager = new VRLocalizationTextResourceManager();
        static VRLocalizationTextResourceTranslationManager s_textResourceTranslationManager = new VRLocalizationTextResourceTranslationManager();

        #endregion

        #region Public Methods

        public string GetTranslatedTextResourceValue(string resourceKey, Guid languageId)
        {
            TextResourceWithTranslation resourceWithTranslation = GetTextResourcesWithTranslationsByKey().GetRecord(resourceKey);
            if (resourceWithTranslation != null)
            {
                VRLocalizationTextResourceTranslation translation;
                if (resourceWithTranslation.TranslationsByLanguageId.TryGetValue(languageId, out translation))
                    return translation.Settings.Value;
                else
                    return resourceWithTranslation.Resource.Settings.DefaultValue;
            }
            else
            {
                return null;
            }
        }

        public bool IsLocalizationEnabled()
        {
            return true;
        }
        #endregion

        #region Private Methods

        Dictionary<string, TextResourceWithTranslation> GetTextResourcesWithTranslationsByKey()
        {
            Dictionary<string, TextResourceWithTranslation> resourcesWithTranslations = new Dictionary<string, TextResourceWithTranslation>();
            Dictionary<Guid, VRLocalizationTextResource> allResources = s_textResourceManager.GetAllResources();
            if (allResources != null)
            {
                Dictionary<Guid, VRLocalizationLanguage> allLanguages = s_languageManager.GetAllLanguages();
                Dictionary<Guid, VRLocalizationTextResourceTranslationsById> allTranslationsByLanguageId = s_textResourceTranslationManager.GetAllResourceTranslationsByLanguageId();
                foreach (var resource in allResources.Values)
                {
                    resource.ResourceKey.ThrowIfNull("resource.ResourceKey", resource.VRLocalizationTextResourceId);
                    resource.Settings.ThrowIfNull("resource.Settings", resource.VRLocalizationTextResourceId);
                    Dictionary<Guid, VRLocalizationTextResourceTranslation> translationsByLanguageId = new Dictionary<Guid, VRLocalizationTextResourceTranslation>();
                    if (allLanguages != null && allTranslationsByLanguageId != null)
                    {
                        foreach (var language in allLanguages.Values)
                        {
                            VRLocalizationTextResourceTranslation translation = ResolveTextResourceTranslation(resource.VRLocalizationTextResourceId, language, allTranslationsByLanguageId, allLanguages);
                            if (translation != null)
                            {
                                translation.Settings.ThrowIfNull("translation.Settings", translation.VRLocalizationTextResourceTranslationId);
                                translationsByLanguageId.Add(language.VRLanguageId, translation);
                            }
                        }
                    }
                    TextResourceWithTranslation resourceWithTranslation = new TextResourceWithTranslation
                    {
                        Resource = resource,
                        TranslationsByLanguageId = translationsByLanguageId
                    };
                    resourcesWithTranslations.Add(resource.ResourceKey, resourceWithTranslation);
                }
            }
            return resourcesWithTranslations;
        }

        VRLocalizationTextResourceTranslation ResolveTextResourceTranslation(Guid resourceId, VRLocalizationLanguage language, Dictionary<Guid, VRLocalizationTextResourceTranslationsById> allTranslationsByLanguageId, Dictionary<Guid, VRLocalizationLanguage> allLanguages)
        {
            VRLocalizationTextResourceTranslation translation = allTranslationsByLanguageId.GetRecord(language.VRLanguageId).GetRecord(resourceId);
            if (translation != null)
            {
                return translation;
            }
            else if (language.ParentLanguageId.HasValue)
            {
                VRLocalizationLanguage parentLanguage;
                if (allLanguages.TryGetValue(language.ParentLanguageId.Value, out parentLanguage))
                    return ResolveTextResourceTranslation(resourceId, parentLanguage, allTranslationsByLanguageId, allLanguages);
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Classes

        private class TextResourceWithTranslation
        {
            public VRLocalizationTextResource Resource { get; set; }

            public Dictionary<Guid, VRLocalizationTextResourceTranslation> TranslationsByLanguageId { get; set; }
        }

        #endregion
    }
}
