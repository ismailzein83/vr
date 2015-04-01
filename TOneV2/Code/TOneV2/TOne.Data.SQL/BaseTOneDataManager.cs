using System;
using System.Collections.Generic;
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
    }
}
