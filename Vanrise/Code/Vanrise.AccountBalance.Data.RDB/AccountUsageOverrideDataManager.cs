using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.RDB;

namespace Vanrise.AccountBalance.Data.RDB
{
    public class AccountUsageOverrideDataManager : IAccountUsageOverrideDataManager
    {
        static string TABLE_NAME = "VR_AccountBalance_AccountUsageOverride";

        const string COL_ID = "ID";
        internal const string COL_AccountTypeID = "AccountTypeID";
        internal const string COL_AccountID = "AccountID";
        internal const string COL_TransactionTypeID = "TransactionTypeID";
        internal const string COL_PeriodStart = "PeriodStart";
        internal const string COL_PeriodEnd = "PeriodEnd";
        const string COL_OverriddenByTransactionID = "OverriddenByTransactionID";
        const string COL_CreatedTime = "CreatedTime";

        static AccountUsageOverrideDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_AccountTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_AccountID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_TransactionTypeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_PeriodStart, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_PeriodEnd, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_OverriddenByTransactionID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "AccountUsageOverride",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_AccountBalance", "VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString");
        }

        #endregion

        public void AddQueryInsertAccountUsageOverrides(RDBQueryContext queryContext, IEnumerable<AccountUsageOverride> accountUsageOverrides)
        {
            foreach (var accountUsageOverride in accountUsageOverrides)
            {
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_AccountTypeID).Value(accountUsageOverride.AccountTypeId);
                insertQuery.Column(COL_AccountID).Value(accountUsageOverride.AccountId);
                insertQuery.Column(COL_TransactionTypeID).Value(accountUsageOverride.TransactionTypeId);
                insertQuery.Column(COL_PeriodStart).Value(accountUsageOverride.PeriodStart);
                insertQuery.Column(COL_PeriodEnd).Value(accountUsageOverride.PeriodEnd);
                insertQuery.Column(COL_OverriddenByTransactionID).Value(accountUsageOverride.OverriddenByTransactionId);
            }
        }

        public void AddQueryDeleteAccountUsageOverrideByTransactionIds(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().ListCondition(COL_OverriddenByTransactionID, RDBListConditionOperator.IN, billingTransactionIds);
        }

        internal bool IsAccountOverriddenInPeriod(Guid accountTypeId, Guid transactionTypeId, string accountId, DateTime periodStart, DateTime periodEnd)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(AccountUsageOverrideDataManager.TABLE_NAME, "au_override", 1);
            selectQuery.SelectColumns().Column(COL_ID);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_AccountTypeID).Value(accountTypeId);
            where.EqualsCondition(COL_AccountID).Value(accountId);
            where.EqualsCondition(COL_TransactionTypeID).Value(transactionTypeId);
            where.LessOrEqualCondition(COL_PeriodStart).Value(periodStart);
            where.GreaterOrEqualCondition(COL_PeriodEnd).Value(periodEnd);

            return  queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }

        internal void AddSelectAccountUsageOverrideToJoinSelect(RDBJoinSelectContext joinSelectContext, IEnumerable<long> deletedTransactionIds)
        {            
            var joinSelectQuery = joinSelectContext.SelectQuery();
            joinSelectQuery.From(TABLE_NAME, "usageOverride");
            joinSelectQuery.SelectColumns().Columns(COL_AccountTypeID, COL_AccountID, COL_TransactionTypeID, COL_PeriodStart, COL_PeriodEnd);
            joinSelectQuery.Where().ListCondition(COL_OverriddenByTransactionID, RDBListConditionOperator.IN, deletedTransactionIds);
        }
    }
}
