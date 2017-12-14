using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Common.Business;

namespace Vanrise.Web
{
    public class LocalizationConfig
    {
        public static Guid? GetLanguageIdFromCookies(HttpCookieCollection cookies)
        {
            string languageCookieName = "VR_Common_LocalizationLangauge";
            string languageIdAsString = cookies[languageCookieName].Value;

            Guid? languageId = null;
            if (languageIdAsString == null)
            {
                languageId = new VRLocalizationManager().GetDefaultLanguage();
            }
            else
            {
                Guid parsedLanguagedId;
                if (Guid.TryParse(languageIdAsString, out parsedLanguagedId))
                    languageId = parsedLanguagedId;
            }
            return languageId;
        }

    }
}