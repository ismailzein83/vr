using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TOne.Entities;
using Vanrise.Data.SQL;

namespace TOne.Data.SQL
{
    public class BaseTOneDataManager : BaseSQLDataManager
    {
        public BaseTOneDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {
        }

        public BaseTOneDataManager()
            : base()
        {
        }

        public TempTableName GenerateTempTableName()
        {
            string tableName = Guid.NewGuid().ToString().Replace("-", "");
            return new TempTableName
            {
                Key = tableName,
                TableName = String.Format("tempdb.dbo.t_{0}", tableName)
            };
        }

        public TempTableName GetTempTableName(string tableNameKey)
        {
            return new TempTableName
            {
                Key = tableNameKey,
                TableName = String.Format("tempdb.dbo.t_{0}", tableNameKey)
            };
        }

        public IEnumerable<T> GetPagedItemsText<T>(string cmdText, Func<IDataReader, T> objectBuilder, Action<DbCommand> prepareCommand, int fromRow, int toRow, bool isDescending, string orderBy)
        {

            string query = String.Format(@"WITH OrderedResult AS (SELECT *, ROW_NUMBER()  OVER ( ORDER BY {1} {2}) AS rowNumber FROM ({0}) tbl)
	                                    SELECT * FROM OrderedResult WHERE rowNumber between @FromRow AND @ToRow", cmdText, orderBy, isDescending ? "DESC" : "ASC");
            List<T> lst = new List<T>();

            GetItemsText<T>(query, objectBuilder,
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromRow", fromRow));
                    cmd.Parameters.Add(new SqlParameter("@ToRow", toRow));
                    if (prepareCommand != null)
                        prepareCommand(cmd);
                });
            return lst;
        }
    }
}
