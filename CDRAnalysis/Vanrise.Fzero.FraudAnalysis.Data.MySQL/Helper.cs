using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public static class Helper
    {

        public static int? AsNullableInt(this string s)
        {
            int value;
            if (int.TryParse(s, out value))
                return value;

            return null;
        }



        public static decimal? AsNullableDecimal(this string s)
        {
            decimal value;
            if (decimal.TryParse(s, out value))
                return value;

            return null;
        }

        public static DateTime? AsNullableDateTime(this string s)
        {
            DateTime value;
            if (DateTime.TryParse(s, out value))
                return value;

            return null;
        }


        public static DateTime AsDateTime(this string s)
        {
            DateTime value;
            if (DateTime.TryParse(s, out value))
                return value;
            else
                return DateTime.Now;
        }

        public static int AsInt(this string s)
        {
            int value;
            if (int.TryParse(s, out value))
                return value;

            return 0;
        }


        public static Int16 AsShortInt(this string s)
        {
            Int16 value;
            if (Int16.TryParse(s, out value))
                return value;

            return 0;
        }

        public static decimal AsDecimal(this string s)
        {
            decimal value;
            if (decimal.TryParse(s, out value))
                return value;

            return 0;
        }
    }
}
