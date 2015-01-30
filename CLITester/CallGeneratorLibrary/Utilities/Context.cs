using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary.Models;

namespace CallGeneratorLibrary.Utilities
{
    public class Context
    {
        private Language language;
        public Language Language
        {
            get { return language; }
            set { language = value; }
        }

        private static Context current;
        public static Context Current
        {
            get
            {
                current = new Context();
                if (System.Web.HttpContext.Current.Request.QueryString["lang"] != null)
                    current.Language = Languages.GetLaunguage(System.Web.HttpContext.Current.Request.QueryString["lang"]);
                else if (System.Web.HttpContext.Current.Session["CurrentPreferences"] != null)
                    current.Language = Languages.GetLaunguage(System.Web.HttpContext.Current.Session["CurrentPreferences"].ToString());
                else
                    current.Language = Languages.DefaultLanguage;

                return current;
            }
        }

        public Context()
        {
            language = Languages.English;
        }

        public void SetLanguage(Language language)
        {
            //store the language in the session
            System.Web.HttpContext.Current.Session["CurrentPreferences"] = language.Code;

            //change the current language setting in the Al Riyada website library
            Context.Current.Language = language;
        }
    }
}
