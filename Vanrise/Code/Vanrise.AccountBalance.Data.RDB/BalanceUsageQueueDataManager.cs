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
        public static string TABLE_NAME = "VR_AccountBalance_BalanceUsageQueue";

        static BalanceUsageQueueDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("QueueType", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("UsageDetails", new RDBTableColumnDefinition { DataType = RDBDataType.VarBinary });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "BalanceUsageQueue",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

        #region Private Methods

        private BalanceUsageQueue<T> BalanceUsageQueueMapper<T>(IRDBDataReader reader)
        {
            return new BalanceUsageQueue<T>
            {
                BalanceUsageQueueId = reader.GetLong("ID"),
                AccountTypeId = reader.GetGuid("AccountTypeID"),
                UsageDetails = Vanrise.Common.ProtoBufSerializer.Deserialize<T>(Common.Compressor.Decompress(reader.GetBytes("UsageDetails")))
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
            new RDBQueryContext(GetDataProvider())
                    .Select()
                    .From(TABLE_NAME, "usageQueue")
                    .Where().And()
                                .EqualsCondition("AccountTypeID", accountTypeId)
                                .EqualsCondition("QueueType", (int)balanceUsageQueueType)
                            .EndAnd()
                    .SelectColumns().AllTableColumns("usageQueue").EndColumns()
                    .EndSelect()
                    .ExecuteReader(reader =>
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
            return new RDBQueryContext(GetDataProvider())
                        .Insert()
                        .IntoTable(TABLE_NAME)
                        .ColumnValue("AccountTypeID", accountTypeId)
                        .ColumnValue("QueueType", (int)balanceUsageQueueType)
                        .ColumnValue("UsageDetails", binaryArray)
                        .EndInsert()
                        .ExecuteNonQuery() > 0;
        }

        public bool HasUsageBalanceData(Guid accountTypeId)
        {
            return new RDBQueryContext(GetDataProvider())
                        .Select()
                        .From(TABLE_NAME, "usageQueue", 1)
                        .Where().EqualsCondition("AccountTypeID", accountTypeId)
                        .SelectColumns().Column("ID").EndColumns()
                        .EndSelect()
                        .ExecuteScalar().NullableLongValue.HasValue;
        }

        #endregion
    }
}
