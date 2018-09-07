using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class BalanceUsageQueueDataManager : IBalanceUsageQueueDataManager
    {
        static string TABLE_NAME = "VR_AccountBalance_BalanceUsageQueue";

        const string COL_ID = "ID";
        const string COL_AccountTypeID = "AccountTypeID";
        const string COL_QueueType = "QueueType";
        const string COL_UsageDetails = "UsageDetails";
        const string COL_CreatedTime = "CreatedTime";

        static BalanceUsageQueueDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_AccountTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_QueueType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_UsageDetails, new RDBTableColumnDefinition { DataType = RDBDataType.VarBinary });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "BalanceUsageQueue",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods

        private BalanceUsageQueue<T> BalanceUsageQueueMapper<T>(IRDBDataReader reader)
        {
            return new BalanceUsageQueue<T>
            {
                BalanceUsageQueueId = reader.GetLong(COL_ID),
                AccountTypeId = reader.GetGuid(COL_AccountTypeID),
                UsageDetails = Vanrise.Common.ProtoBufSerializer.Deserialize<T>(Common.Compressor.Decompress(reader.GetBytes(COL_UsageDetails)))
            };
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_AccountBalance", "VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString");
        }

        #endregion

        #region IBalanceUsageQueueDataManager

        public void LoadUsageBalance<T>(Guid accountTypeId, Entities.BalanceUsageQueueType balanceUsageQueueType, Action<Entities.BalanceUsageQueue<T>> onUsageBalanceUpdateReady)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "usageQueue");
            selectQuery.SelectColumns().AllTableColumns("usageQueue");

            var whereCondition = selectQuery.Where();
            whereCondition.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            whereCondition.EqualsCondition(COL_QueueType).Value((int)balanceUsageQueueType);

            queryContext.ExecuteReader(reader =>
            {
                while (reader.Read())
                {
                    onUsageBalanceUpdateReady(BalanceUsageQueueMapper<T>(reader));
                }
            });
        }

        public bool UpdateUsageBalance<T>(Guid accountTypeId, Entities.BalanceUsageQueueType balanceUsageQueueType, T updateUsageBalancePayload)
        {
            byte[] binaryArray = null;
            if (updateUsageBalancePayload != null)
            {
                binaryArray = Common.ProtoBufSerializer.Serialize(updateUsageBalancePayload);
                binaryArray = Common.Compressor.Compress(binaryArray);
            
            }
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.Column(COL_AccountTypeID).Value(accountTypeId);
            insertQuery.Column(COL_QueueType).Value((int)balanceUsageQueueType);
            insertQuery.Column(COL_UsageDetails).Value(binaryArray);
            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool HasUsageBalanceData(Guid accountTypeId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, "usageQueue", 1);
            selectQuery.SelectColumns().Column(COL_ID);

            selectQuery.Where().EqualsCondition(COL_AccountTypeID).Value(accountTypeId);

            return queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        #endregion

        public void AddQueryDeleteBalanceUsageQueue(RDBQueryContext queryContext, long balanceUsageQueueId)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().EqualsCondition(COL_ID).Value(balanceUsageQueueId);
        }
    }
}
