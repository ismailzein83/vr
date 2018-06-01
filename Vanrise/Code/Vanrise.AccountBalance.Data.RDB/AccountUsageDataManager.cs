using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class AccountUsageDataManager
    {
        public static string TABLE_NAME = "VR_AccountBalance_AccountUsage";

        static AccountUsageDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("TransactionTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("CurrencyId", new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add("UsageBalance", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("PeriodStart", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("IsOverridden", new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add("OverriddenAmount", new RDBTableColumnDefinition { DataType = RDBDataType.Decimal, Size = 20, Precision = 6 });
            columns.Add("CorrectionProcessID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "AccountUsage",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
            });
        }

    }
}
