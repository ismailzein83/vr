using Vanrise.Tools.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Vanrise.Common;
using Vanrise.Tools.Entities;
using Vanrise.Data.SQL;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace Vanrise.Tools.Business
{
    public class TableManager
    {

        ITableDataManager tableDataManager = VRToolsFactory.GetDataManager<ITableDataManager>();

        //public void ExecuteQuery(Guid vrConnectionId, string commandQuery)
        //{

        //    SQLConnection settings = new VRConnectionManager().GetVRConnection(vrConnectionId).Settings as SQLConnection;
        //    string connectionString = (settings != null) ? settings.ConnectionString : null;
        //    if (String.IsNullOrEmpty(connectionString))
        //        throw new NullReferenceException(String.Format("connection string is null or empty"));

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        if (connection.State == System.Data.ConnectionState.Closed)
        //            connection.Open();


        //        var command = connection.CreateCommand();
        //        command.CommandText = commandQuery;
        //        command.CommandTimeout = 60;
        //        command.CommandType = System.Data.CommandType.Text;
        //        command.ExecuteNonQuery();
        //        connection.Close();
        //    }

        //}

        #region Public Methods

        
        public IEnumerable<TableInfo> GetTablesInfo(TableInfoFilter tableInfoFilter)
        {

            Guid ConnectionId = tableInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(ConnectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            tableDataManager.Connection_String = connectionString;


            List<Table> allTables = tableDataManager.GetTables();

            Func<Table, bool> filterFunc = (table) =>
            {
                
                return true;
            };
            return allTables.MapRecords(TableInfoMapper, filterFunc).OrderBy(table => table.Name);
        }


        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        public TableDetails TableDetailMapper(Table table)
        {
            var tableDetails = new TableDetails
            {
                Name = table.Name,
                ConnectionId = table.ConnectionId,

            };

            return tableDetails;
        }

        public TableInfo TableInfoMapper(Table table)
        {
            return new TableInfo
            {
                Name = table.Name,
                ConnectionId = table.ConnectionId
            };
        }
        #endregion

    }
}
