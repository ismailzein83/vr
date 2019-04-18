﻿using System;
using System.Collections.Generic;
using System.Configuration;
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
		static VRLocalizationModuleManager s_moduleManager = new VRLocalizationModuleManager();
		static VRLocalizationTextResourceManager s_textResourceManager = new VRLocalizationTextResourceManager();
		static VRLocalizationTextResourceTranslationManager s_textResourceTranslationManager = new VRLocalizationTextResourceTranslationManager();
		#endregion

		#region Public Methods
		public Guid? GetCurrentLanguageId()
		{
			Guid languageId;
			if (TryGetLanguageId(out languageId))
				return languageId;
			return null;
		}
		public string GetTranslatedTextResourceValue(string resourceKey, string defaultValue, Guid languageId)
		{
			if (IsLocalizationEnabled())
			{
				TextResourceWithTranslation resourceWithTranslation = GetTextResourcesWithTranslationsByKey().GetRecord(resourceKey);
				if (resourceWithTranslation != null)
				{
					VRLocalizationTextResourceTranslation translation;
					if (resourceWithTranslation.TranslationsByLanguageId.TryGetValue(languageId, out translation))
						return translation.Value;
					else
						return resourceWithTranslation.Resource.DefaultValue;
				}
			}
			return defaultValue;
		}
		public string GetTranslatedTextResourceValue(string resourceKey, string defaultValue)
		{
			var languageId = GetCurrentLanguageId();
			if (languageId.HasValue)
			{
				return GetTranslatedTextResourceValue(resourceKey, defaultValue, languageId.Value);
			}
			return defaultValue;
		}
		public bool IsLocalizationEnabled()
		{
			var isLocalizationEnabled = ConfigurationManager.AppSettings["IsLocalizationEnabled"];
			if (isLocalizationEnabled != null && isLocalizationEnabled == "true")
			{
				return true;
			}

			ConfigManager configManager = new ConfigManager();
			var generalTechnicalSettings = configManager.GetGeneralTechnicalSetting();
			if (generalTechnicalSettings != null)
				return generalTechnicalSettings.IsLocalizationEnabled;
			return false;
		}
		public bool IsRTL()
		{
			var languageId = GetCurrentLanguageId();
			if (languageId.HasValue)
			{
				VRLocalizationLanguageManager vrLocalizationLanguageManager = new VRLocalizationLanguageManager();
				return vrLocalizationLanguageManager.IsRTL(languageId.Value);
			}
			return false;
		}

		#endregion

		#region Private Methods
		private Guid? GetDefaultLanguage()
		{
			ConfigManager configManager = new ConfigManager();
			var generalSettings = configManager.GetGeneralSetting();
			generalSettings.ThrowIfNull("generalSettings");
			generalSettings.UIData.ThrowIfNull("generalSettings.UIData");
			return generalSettings.UIData.DefaultLanguageId;
		}
		private bool TryGetLanguageId(out Guid languageId)
		{
			languageId = Guid.Empty;
			if (IsLocalizationEnabled())
			{
				if (!VRWebContext.IsInWebContext())
					return false;
				string languageIdAsString = null;

				if ((languageIdAsString = VRWebContext.GetCurrentRequestQueryString("vrlangId")) != null)
				{
					if (Guid.TryParse(languageIdAsString, out languageId))
						return true;
				}
				var urlParts = VRWebContext.GetCurrentRequestURLParts();
				string languageCookieName = string.Format("VR_Common_LocalizationLangauge_{0}", VRWebUtilities.UrlEncode(string.Format("{0}://{1}:{2}", urlParts.Scheme, urlParts.Host, urlParts.Port)));
				var languageCookie = VRWebContext.GetCurrentRequestCookie(languageCookieName);
				if (languageCookie != null)
					languageIdAsString = languageCookie.Value;
				if (!string.IsNullOrEmpty(languageIdAsString))
				{
					Guid parsedLanguagedId;
					if (Guid.TryParse(languageIdAsString, out parsedLanguagedId))
						languageId = parsedLanguagedId;
					return true;
				}
				else
				{
					var defaultLanguageId = GetDefaultLanguage();
					if (defaultLanguageId.HasValue)
					{
						languageId = defaultLanguageId.Value;
						return true;
					}
				}
			}
			return false;
		}

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
					Dictionary<Guid, VRLocalizationTextResourceTranslation> translationsByLanguageId = new Dictionary<Guid, VRLocalizationTextResourceTranslation>();
					if (allLanguages != null && allTranslationsByLanguageId != null)
					{
						foreach (var language in allLanguages.Values)
						{
							VRLocalizationTextResourceTranslation translation = ResolveTextResourceTranslation(resource.VRLocalizationTextResourceId, language, allTranslationsByLanguageId, allLanguages);
							if (translation != null)
							{
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
					resourcesWithTranslations.Add($"{s_moduleManager.GetVRModuleName(resource.ModuleId)}.{resource.ResourceKey}", resourceWithTranslation);
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
