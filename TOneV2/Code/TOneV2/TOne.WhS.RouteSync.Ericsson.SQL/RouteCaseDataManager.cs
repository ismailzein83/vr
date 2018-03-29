using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Ericsson.Data;
using TOne.WhS.RouteSync.Ericsson.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.RouteSync.Ericsson.SQL
{
    public class RouteCaseDataManager : BaseSQLDataManager, IRouteCaseDataManager
    {
        public RouteCaseDataManager()
            : base(GetConnectionStringName("TOneWhS_RouteSync_DBConnStringKey", "RouteSyncDBConnString"))
        {

        }

        public IEnumerable<RouteCase> GetAllRouteCases(int switchId)
        {
            return GetItemsSP(string.Format("WhS_Ericsson{0}.sp_RouteCase_GetAll", switchId), RouteCaseMapper);
        }

        public RouteCase RouteCaseMapper(IDataReader reader)
        {
            string options = reader["Options"] as string;
            return new RouteCase()
            {
                RouteCaseId = (int)reader["ID"],
                RouteCaseNumber = (int)reader["RCNumber"],
                RouteCaseOptions = !string.IsNullOrEmpty(options) ? Vanrise.Common.Serializer.Deserialize<List<RouteCaseOption>>(options) : null
            };
        }
    }
}