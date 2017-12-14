using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vanrise.Common.Business;

namespace Vanrise.Web
{
    public class LocalizationConfig
    {
        //public static Guid? GetLanguageIdFromCookies(HttpCookieCollection cookies)
        //{
        //    Guid? languageId = null;
        //    string languageCookieName = "VR_Common_LocalizationLangauge";
        //    var cookie = cookies[languageCookieName];
        //    if(cookie!= null)
        //    {
        //        string languageIdAsString = cookies[languageCookieName].Value;
        //        if (languageIdAsString == null)
        //        {
        //            languageId = new VRLocalizationManager().GetDefaultLanguage();
        //        }
        //        else
        //        {
        //            Guid parsedLanguagedId;
        //            if (Guid.TryParse(languageIdAsString, out parsedLanguagedId))
        //                languageId = parsedLanguagedId;
        //        }
        //    }
           
        //    return languageId;
        //}

    }
}