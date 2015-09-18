using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BI.Data.SQL
{

    internal static class DateTimeColumns
    {
        static DateTimeColumns()
        {
            s_DATE = GetDimensionName("BIDimension_DATE", "[Date].[Date]");
            s_YEAR = GetDimensionName("BIDimension_YEAR", "[Date].[Year]");
            s_MONTHOFYEAR = GetDimensionName("BIDimension_MONTHOFYEAR", "[Date].[Month Of Year]");
            s_WEEKOFMONTH = GetDimensionName("BIDimension_WEEKOFMONTH", "[Date].[Week Of Month]");
            s_DAYOFMONTH = GetDimensionName("BIDimension_DAYOFMONTH", "[Date].[Day Of Month]");
            s_HOUR = GetDimensionName("BIDimension_HOUR", "[Time].[Hour]");
        }

        static string GetDimensionName(string configKey, string defaultValue)
        {
            return ConfigurationManager.AppSettings[configKey] ?? defaultValue;
        }

        static string s_DATE;
        static string s_YEAR;
        static string s_MONTHOFYEAR;
        static string s_WEEKOFMONTH;
        static string s_DAYOFMONTH;
        static string s_HOUR;
        

        public static string DATE
        {
            get
            {
                return s_DATE;
            }
        }

        public static string YEAR
        {
            get
            {
                return s_YEAR;
            }
        }

        public static string MONTHOFYEAR
        {
            get
            {
                return s_MONTHOFYEAR;
            }
        }

        public static string WEEKOFMONTH
        {
            get
            {
                return s_WEEKOFMONTH;
            }
        }

        public static string DAYOFMONTH
        {
            get
            {
                return s_DAYOFMONTH;
            }
        }

        public static string HOUR
        {
            get
            {
                return s_HOUR;
            }
        }
    }



}
