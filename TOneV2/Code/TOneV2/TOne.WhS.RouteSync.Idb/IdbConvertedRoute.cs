using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.Idb
{
    public abstract class IdbConvertedRoute : ConvertedRoute, INpgBulkCopy
    {
        public string Pref { get; set; }
        public string Route { get; set; }

        public string ConvertToString()
        {
            return string.Format("{1}{0}{2}", "\t", Pref, Route);
        }
    }
}