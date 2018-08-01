﻿using System;
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
        public static string TABLE_NAME = "VR_AccountBalance_AccountUsageOverride";

        static AccountUsageOverrideDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add("ID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("AccountTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("AccountID", new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add("TransactionTypeID", new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add("PeriodStart", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("PeriodEnd", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add("OverriddenByTransactionID", new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add("CreatedTime", new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "VR_AccountBalance",
                DBTableName = "AccountUsageOverride",
                Columns = columns,
                IdColumnName = "ID",
                CreatedTimeColumnName = "CreatedTime"
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
                insertQuery.ColumnValue("AccountTypeID", accountUsageOverride.AccountTypeId);
                insertQuery.ColumnValue("AccountID", accountUsageOverride.AccountId);
                insertQuery.ColumnValue("TransactionTypeID", accountUsageOverride.TransactionTypeId);
                insertQuery.ColumnValue("PeriodStart", accountUsageOverride.PeriodStart);
                insertQuery.ColumnValue("PeriodEnd", accountUsageOverride.PeriodEnd);
                insertQuery.ColumnValue("OverriddenByTransactionID", accountUsageOverride.OverriddenByTransactionId);
            }
        }

        public void AddQueryDeleteAccountUsageOverrideByTransactionIds(RDBQueryContext queryContext, IEnumerable<long> billingTransactionIds)
        {
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(TABLE_NAME);
            deleteQuery.Where().ListCondition("OverriddenByTransactionID", RDBListConditionOperator.IN, billingTransactionIds);
        }

        internal bool IsAccountOverriddenInPeriod(Guid accountTypeId, Guid transactionTypeId, string accountId, DateTime periodStart, DateTime periodEnd)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(AccountUsageOverrideDataManager.TABLE_NAME, "au_override", 1);
            selectQuery.SelectColumns().Column("ID");

            var whereAndCondition = selectQuery.Where().And();
            whereAndCondition.EqualsCondition("AccountTypeID", accountTypeId);
            whereAndCondition.EqualsCondition("AccountID", accountId);
            whereAndCondition.EqualsCondition("TransactionTypeID", transactionTypeId);
            whereAndCondition.CompareCondition("PeriodStart", RDBCompareConditionOperator.LEq, periodStart);
            whereAndCondition.CompareCondition("PeriodEnd", RDBCompareConditionOperator.GEq, periodEnd);

            return  queryContext.ExecuteScalar().NullableLongValue.HasValue;
        }
    }
}
