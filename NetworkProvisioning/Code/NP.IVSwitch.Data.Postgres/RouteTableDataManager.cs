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
    public class RouteTableDataManager : BasePostgresDataManager, IRouteTableDataManager
    {
         public RouteTableDataManager()
            : base(GetConnectionStringName("NetworkProvisioningDBConnStringKey", "NetworkProvisioningDBConnString"))
        {

        }

        private RouteTable RouteTableMapper(IDataReader reader)
        {

            RouteTable routeTable = new RouteTable
            {

                RouteTableId = (int)reader["route_table_id"],
                RouteTableName = reader["route_table_name"] as string,
            };

            return routeTable;
        }


        public List<RouteTable> GetRouteTables()
        {
            String cmdText = @"SELECT  route_table_id, route_table_name            
                                       FROM route_tables;";
            return GetItemsText(cmdText, RouteTableMapper, (cmd) =>
            {
            });
        }

    }
}