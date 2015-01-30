using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CallGeneratorLibrary.Utilities;

namespace CallGeneratorLibrary.Models
{
    public class Languages
    {
        #region Members
        public static Language Arabic
        {
            get
            {
                return new Language(Enums.Lang.Arabic.ToString(), "ar", Enums.Align.right,
                    Enums.Lang.Arabic, Enums.Direction.rtl, new System.Globalization.CultureInfo("ar-LB"));
            }
        }
        public static Language English
        {
            get
            {
                return new Language(Enums.Lang.English.ToString(), "en", Enums.Align.left,
                    Enums.Lang.English, Enums.Direction.ltr, new System.Globalization.CultureInfo("en-US"));
            }
        }
        public static Language French
        {
            get
            {
                return new Language(Enums.Lang.French.ToString(), "fr", Enums.Align.left,
                    Enums.Lang.French, Enums.Direction.ltr, new System.Globalization.CultureInfo("fr-FR"));
            }
        }
        #endregion

        #region Properties
        public static Language DefaultLanguage
        {
            get { return English; }
        }
        public static Language GetLaunguage(Enums.Lang lang)
        {
            switch (lang)
            {
                case Enums.Lang.Arabic:
                    return Arabic;
                case Enums.Lang.English:
                    return English;
                case Enums.Lang.French:
                    return French;
            }
            return Arabic;
        }
        public static Language GetLaunguage(string lang)
        {
            switch (lang.ToLower())
            {
                case "ar":
                    return Arabic;
                case "en":
                    return English;
                case "fr":
                    return French;
            }
            return French;
        }
        #endregion
    }
}
