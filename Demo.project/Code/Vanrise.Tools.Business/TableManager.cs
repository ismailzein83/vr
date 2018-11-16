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

        #region Public Methods

        
        public IEnumerable<TableInfo> GetTablesInfo(TableInfoFilter tableInfoFilter)
        {
            ITableDataManager tableDataManager = VRToolsFactory.GetDataManager<ITableDataManager>();


            Guid connectionId = tableInfoFilter.ConnectionId;
            SQLConnection settings = new VRConnectionManager().GetVRConnection(connectionId).Settings as SQLConnection;
            string connectionString = (settings != null) ? settings.ConnectionString : null;

            tableDataManager.Connection_String = connectionString;

            List<Table> allTables = tableDataManager.GetTables(tableInfoFilter.SchemaName);

            return allTables.MapRecords(TableInfoMapper).OrderBy(table => table.Name);
        }


        #endregion

        #region Private Classes

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        public TableInfo TableInfoMapper(Table table)
        {
            return new TableInfo
            {
                Name = table.Name,
            };
        }
        #endregion

    }
}
