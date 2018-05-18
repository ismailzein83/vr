using System;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.FreeRadius
{
    public class FreeRadiusConvertedRoute : ConvertedRoute, INpgBulkCopy
    {
        public string Customer_id { get; set; }
        public string Clisis { get; set; }
        public string Cldsid { get; set; }

        /// <summary>
        /// Related to Code column at DB
        /// </summary>
        public string Option { get; set; } 
        public decimal Min_perc { get; set; }
        public decimal Max_perc { get; set; }

        public string ConvertToString()
        {
            //object clisis;
            //if (!string.IsNullOrEmpty(Clisis))
            //    clisis = Clisis;
            //else
            //    clisis = "\\N";

            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}", "\t", Customer_id, Clisis, Cldsid, Option, Min_perc, Max_perc);
        }
    }
}