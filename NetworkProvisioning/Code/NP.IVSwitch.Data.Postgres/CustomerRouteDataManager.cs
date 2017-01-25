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

        private string _routeOptionsQuery = @" select destination,route_id,flag_1,Preference
                                                from {0} {3} {2}";
        private string _customerRouteQuery = @"
                            ;with TopDistinctCodes as (
                            select distinct destination from {0}
                            {3}
                            {2}
                            {1}
                            )
                            select {0}.destination,{0}.route_id,{0}.flag_1,{0}.Preference from TopDistinctCodes
                            join {0} on {0}.destination = TopDistinctCodes.destination";

        public List<CustomerRoute> GetCustomerRoutes(List<EndPointInfo> acls, string topQuery, string orderByQuery, string destinationCondition)
        {
            return ExecuteRoutes(acls, _customerRouteQuery, topQuery, orderByQuery, destinationCondition);
        }
        public List<CustomerRoute> GetCustomerRouteOptions(List<EndPointInfo> acls, string topQuery, string orderByQuery, string destinationCondition)
        {
            return ExecuteRoutes(acls, _routeOptionsQuery, topQuery, orderByQuery, destinationCondition);
        }
        private List<CustomerRoute> ExecuteRoutes(List<EndPointInfo> acls,string mainQuery, string topQuery, string orderByQuery, string destinationCondition)
        {
            EndPointDataManager endPointDataManager = new EndPointDataManager { IvSwitchSync = IvSwitchSync };
            int routeId = endPointDataManager.GetTableId(acls.First().EndPointId);
            string routeTableName = string.Format("rt{0}", routeId);
            string query = string.Format(mainQuery
                , routeTableName, topQuery, orderByQuery, destinationCondition);
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
