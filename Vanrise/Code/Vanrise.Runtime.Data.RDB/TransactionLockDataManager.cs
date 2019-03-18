using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.RDB
{
    public class TransactionLockDataManager : ITransactionLockDataManager
    {
        static string TABLE_NAME = "runtime_TransactionLock";
        static string TABLE_ALIAS = "TLock";

        const string COL_ID = "ID";
        const string COL_TransactionUniqueName = "TransactionUniqueName";
        const string COL_ProcessID = "ProcessID";
        const string COL_CreatedTime = "CreatedTime";

        static TransactionLockDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_TransactionUniqueName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_ProcessID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "runtime",
                DBTableName = "TransactionLock",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("Runtime", "RuntimeConnStringKey", "RuntimeDBConnString");
        }

        #region Public Methods

        public void Add(TransactionLockItem lockItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_ID).Value(lockItem.LockItemUniqueId);
            insertQuery.Column(COL_TransactionUniqueName).Value(lockItem.TransactionUniqueName);
            insertQuery.Column(COL_ProcessID).Value(lockItem.ProcessId);

            queryContext.ExecuteNonQuery();
        }

        public void Delete(Guid lockItemId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);

            deleteQuery.Where().EqualsCondition(COL_ID).Value(lockItemId);

            queryContext.ExecuteNonQuery();
        }

        public List<TransactionLockItem> GetAll()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, false);

            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(TransactionLockItemMapper);
        }

        public List<TransactionLockItem> GetLocksForNotRunningProcesses()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, false);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            string runningProcessTableAlias = "runningProcess";
            RunningProcessDataManager.JoinRunningProcess(selectQuery.Join(), RDBJoinType.Left, runningProcessTableAlias, TABLE_ALIAS, COL_ProcessID);

            selectQuery.Where().NullCondition(runningProcessTableAlias, COL_ID);

            return queryContext.GetItems(TransactionLockItemMapper);
        }

        #endregion

        #region Private Methods

        private TransactionLockItem TransactionLockItemMapper(IRDBDataReader reader)
        {
            return new TransactionLockItem
            {
                LockItemUniqueId = reader.GetGuid(COL_ID),
                TransactionUniqueName = reader.GetString(COL_TransactionUniqueName),
                ProcessId = reader.GetInt(COL_ProcessID)
            };
        }

        #endregion
    }
}
