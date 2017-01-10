using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.Postgres;

namespace NP.IVSwitch.Data.Postgres
{
    public class CustomerRouteDataManager : BasePostgresDataManager, ICustomerRouteDataManager
    {
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }
        protected override string GetConnectionString()
        {
            return IvSwitchSync.RouteConnectionString;
        }
        public List<CustomerRoute> GetCustomerRoutes(List<EndPointInfo> acls, int top, string orderBy, string codePrefix)
        {
            EndPointDataManager endPointDataManager = new EndPointDataManager { IvSwitchSync = IvSwitchSync };
            int routeId = endPointDataManager.GetTableId(acls.First().EndPointId);
            string routeTableName = string.Format("rt{0}", routeId);
            string destinationQuery = string.Empty;
            if (!string.IsNullOrEmpty(codePrefix))
            {
                destinationQuery = string.Format("where destination like '{0}%'", codePrefix);
            }
            string query = string.Format(@"
                            ;with TopDistinctCodes as (
                            select distinct destination from {0}
                            {3}
                            order by destination {2}
                            limit {1}
                            )
                            select {0}.destination,{0}.route_id,{0}.flag_1,{0}.Preference from TopDistinctCodes
                            join {0} on {0}.destination = TopDistinctCodes.destination"
                               , routeTableName, top, orderBy, destinationQuery);
            return GetItemsText(query, CustomerRouteMapper, null);
        }
        #region mapper

        private CustomerRoute CustomerRouteMapper(IDataReader reader)
        {
            return new CustomerRoute
            {
                Destination = reader["destination"] as string,
                RouteId = GetReaderValue<int>(reader, "route_id"),
                Percentage = GetReaderValue<decimal>(reader, "flag_1"),
                Preference = GetReaderValue<Int16>(reader, "Preference")
            };
        }
        #endregion
    }
}
