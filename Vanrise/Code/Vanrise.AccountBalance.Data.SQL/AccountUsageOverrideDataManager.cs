using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class AccountUsageOverrideDataManager : Vanrise.Data.SQL.BaseSQLDataManager, IAccountUsageOverrideDataManager
    {
        #region Constructors

        public AccountUsageOverrideDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }

        #endregion

        public bool Insert(IEnumerable<AccountUsageOverride> accountUsageOverrides)
        {
            DataTable accountUsageOverrideTable = GetAccountUsageOverrideTable(accountUsageOverrides);

            int affectedRecords = ExecuteNonQuerySPCmd("VR_AccountBalance.sp_AccountUsageOverride_Insert", (cmd) =>
            {
                var sqlTableParameter = new System.Data.SqlClient.SqlParameter("@AccountUsageOverrideTable", SqlDbType.Structured);
                sqlTableParameter.Value = accountUsageOverrideTable;
                cmd.Parameters.Add(sqlTableParameter);
            });
            return (affectedRecords > 0);
        }

        public bool Delete(IEnumerable<long> deletedTransactionIds)
        {
            string deletedTransactionIdsAsString = (deletedTransactionIds != null) ? string.Join(",", deletedTransactionIds) : null;
            int affectedRows = ExecuteNonQuerySP("VR_AccountBalance.sp_AccountUsageOverride_Delete", deletedTransactionIdsAsString);
            return affectedRows > 0;
        }

        public DataTable GetTransactionAccountQueryTable(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries)
        {
            var dataTable = new DataTable("TransactionAccountUsageQuery");

            dataTable.Columns.Add("TransactionID", typeof(long));
            dataTable.Columns.Add("TransactionTypeID", typeof(Guid));
            dataTable.Columns.Add("AccountTypeID", typeof(Guid));
            dataTable.Columns.Add("AccountID", typeof(string));
            dataTable.Columns.Add("PeriodStart", typeof(DateTime));
            dataTable.Columns.Add("PeriodEnd", typeof(DateTime));

            dataTable.BeginLoadData();

            foreach (TransactionAccountUsageQuery transactionAccountUsageQuery in transactionAccountUsageQueries)
            {
                DataRow dataRow = dataTable.NewRow();

                dataRow["TransactionID"] = transactionAccountUsageQuery.TransactionId;
                dataRow["TransactionTypeID"] = transactionAccountUsageQuery.TransactionTypeId;
                dataRow["AccountTypeID"] = transactionAccountUsageQuery.AccountTypeId;
                dataRow["AccountID"] = transactionAccountUsageQuery.AccountId;
                dataRow["PeriodStart"] = transactionAccountUsageQuery.PeriodStart;
                dataRow["PeriodEnd"] = transactionAccountUsageQuery.PeriodEnd;

                dataTable.Rows.Add(dataRow);
            }

            dataTable.EndLoadData();
            return dataTable;
        }

        public DataTable GetAccountUsageOverrideTable(IEnumerable<AccountUsageOverride> accountUsageOverrides)
        {
            var dataTable = new DataTable("AccountUsageOverride");

            dataTable.Columns.Add("AccountTypeID", typeof(Guid));
            dataTable.Columns.Add("AccountID", typeof(string));
            dataTable.Columns.Add("TransactionTypeID", typeof(Guid));
            dataTable.Columns.Add("PeriodStart", typeof(DateTime));
            dataTable.Columns.Add("PeriodEnd", typeof(DateTime));
            dataTable.Columns.Add("OverridenByTransactionID", typeof(long));

            dataTable.BeginLoadData();

            foreach (AccountUsageOverride accountUsageOverride in accountUsageOverrides)
            {
                DataRow dataRow = dataTable.NewRow();

                dataRow["AccountTypeID"] = accountUsageOverride.AccountTypeId;
                dataRow["AccountID"] = accountUsageOverride.AccountId;
                dataRow["TransactionTypeID"] = accountUsageOverride.TransactionTypeId;
                dataRow["PeriodStart"] = accountUsageOverride.PeriodStart;
                dataRow["PeriodEnd"] = accountUsageOverride.PeriodEnd;
                dataRow["OverridenByTransactionID"] = accountUsageOverride.OverriddenByTransactionId;

                dataTable.Rows.Add(dataRow);
            }

            dataTable.EndLoadData();
            return dataTable;
        }
    }
}
