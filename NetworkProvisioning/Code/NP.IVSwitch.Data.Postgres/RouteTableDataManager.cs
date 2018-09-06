using NP.IVSwitch.Entities.RouteTable;
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
        public TOne.WhS.RouteSync.IVSwitch.BuiltInIVSwitchSWSync IvSwitchSync { get; set; }

        protected override string GetConnectionString()
        {
            return IvSwitchSync.MasterConnectionString;
        }   
        public bool Insert(RouteTableInput routeTableInput, out int insertedId)
        {

            String cmdText = @"INSERT INTO route_tables(route_table_name,description)
	                             SELECT  @route_table_name,@description where(NOT EXISTS(SELECT 1 FROM route_tables WHERE  route_table_name =@route_table_name))  
 	                             returning  route_table_id;";

            var recId = ExecuteScalarText(cmdText, (cmd) =>
            {
                cmd.Parameters.AddWithValue("@route_table_name", routeTableInput.RouteTable.Name);
                cmd.Parameters.AddWithValue("@description", routeTableInput.RouteTable.Description);
            });

            insertedId = -1;
            if (recId == null)
                return false;
            insertedId = Convert.ToInt32(recId);
            return insertedId > 0;
        }
        public bool DeleteRouteTable(int routeTableId)
        {
            string cmdText = string.Format("DELETE FROM route_tables where route_table_id={0};", routeTableId);
            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>{});
            return true;
        }
        public bool Update(RouteTableInput routeTableInput)
        {
            String cmdText = @"UPDATE route_tables
	                             SET route_table_name = @route_table_name, description = @description,p_score=@p_score 
                                 WHERE  route_table_id = @route_table_id and  NOT EXISTS(SELECT 1 FROM  route_tables WHERE  route_table_id != @route_table_id and route_table_name = @route_table_name);";

            int recordsEffected = ExecuteNonQueryText(cmdText, cmd =>
            {
                cmd.Parameters.AddWithValue("@route_table_name", routeTableInput.RouteTable.Name);
                cmd.Parameters.AddWithValue("@description", routeTableInput.RouteTable.Description);
                cmd.Parameters.AddWithValue("@p_score", routeTableInput.RouteTable.PScore);
                cmd.Parameters.AddWithValue("@route_table_id", routeTableInput.RouteTable.RouteTableId);

            }
           );
            return recordsEffected > 0;
        }
        public List<RouteTable> GetRouteTables()
        {
            String cmdText = @"SELECT route_table_id,route_table_name,description,p_score FROM route_tables;";
            return GetItemsText(cmdText, RouteTableMapper, (cmd) =>
            {
            });
        }
        private RouteTable RouteTableMapper(IDataReader reader)
        {
            RouteTable routeTable = new RouteTable();

            var routeTableId = reader["route_table_id"];
            var name = reader["route_table_name"];
            var description = reader["description"];
            var pScore = reader["p_score"];

            if (routeTableId != DBNull.Value)
            {
                routeTable.RouteTableId = (int)routeTableId;
            }
            if (name != DBNull.Value)
            {
                routeTable.Name = (string)name;
            }
            if (description != DBNull.Value)
            {
                routeTable.Description = (string)description;
            }
            if (pScore != DBNull.Value)
            {
                routeTable.PScore = (int?)pScore;
            }

            return routeTable;
        }


    }
}