using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class BalanceUsageQueueDataManager
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
    }
}
