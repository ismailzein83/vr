﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class AccountUsageDataManager : BaseSQLDataManager, IAccountUsageDataManager
    {
        #region Fields / Constructors

        private const string AccountUsage_TABLENAME = "AccountUsage";
        public AccountUsageDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart, Guid transactionTypeId)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetByPeriod]", AccountUsageInfoMapper, accountTypeId, periodStart, transactionTypeId);
        }
        public AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, Guid transactionTypeId, String accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance)
        {
            return GetItemSP("[VR_AccountBalance].[sp_AccountUsage_TryAddAndGet]", AccountUsageInfoMapper, accountTypeId, transactionTypeId, accountId, periodStart, periodEnd, currencyId, usageBalance);
        }
        public bool UpdateAccountUsageFromBalanceUsageQueue(IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            if (accountsUsageToUpdate != null)
            {
                DataTable accountUsageToUpdate = GetAccountUsageTable();
                foreach (var item in accountsUsageToUpdate)
                {
                    DataRow dr = accountUsageToUpdate.NewRow();
                    FillAccountUsageTableRow(dr, item);
                    accountUsageToUpdate.Rows.Add(dr);
                }
                accountUsageToUpdate.EndLoadData();
                if (accountUsageToUpdate.Rows.Count > 0)
                    ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_AccountUsage_UpdateBalance]",
                           (cmd) =>
                           {
                               var dtPrm = new System.Data.SqlClient.SqlParameter("@BalanceTable", SqlDbType.Structured);
                               dtPrm.Value = accountUsageToUpdate;
                               cmd.Parameters.Add(dtPrm);
                               var dtPrm1 = new System.Data.SqlClient.SqlParameter("@CorrectionProcessID", SqlDbType.UniqueIdentifier);
                               dtPrm1.Value = correctionProcessId;
                               cmd.Parameters.Add(dtPrm1);
                           });
            }

            return true;
        }

        public IEnumerable<AccountUsage> GetAccountUsageForSpecificPeriodByAccountIds(Guid accountTypeId, Guid transactionTypeId, DateTime datePeriod, List<String> accountIds)
        {
            string accountIdsString = null;
            if (accountIds != null)
            {
                accountIdsString = string.Join<String>(",", accountIds);
            }
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetForSpecificPeriodByAccountIds]", AccountUsageMapper, accountTypeId, transactionTypeId, datePeriod, accountIdsString);
        }
        public IEnumerable<AccountUsage> GetAccountUsageForBillingTransactions(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds, DateTime fromTime, DateTime? toTime)
        {
            string accountIdsString = null;
            if (accountIds != null)
            {
                accountIdsString = string.Join<String>(",", accountIds);
            }
            string transactionTypeIdsString = null;
            if (transactionTypeIds != null)
            {
                transactionTypeIdsString = string.Join<Guid>(",", transactionTypeIds);
            }
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetForFilteredForBillingTransaction]", AccountUsageMapper, accountTypeId, transactionTypeIdsString, accountIdsString, fromTime, toTime);
        }
        public IEnumerable<AccountUsage> GetAccountUsagesByAccount(Guid accountTypeId, String accountId)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetByAccount]", AccountUsageMapper, accountTypeId, accountId);
        }
        public List<AccountUsage> GetAccountUsageErrorData(Guid accountTypeId, Guid transactionTypeId, Guid correctionProcessId, DateTime periodDate)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetErrorData]", AccountUsageMapper, accountTypeId, transactionTypeId, correctionProcessId, periodDate);
        }

        public IEnumerable<AccountUsage> GetAccountUsagesByTransactionAccountUsageQueries(IEnumerable<TransactionAccountUsageQuery> transactionAccountUsageQueries)
        {
            DataTable accountUsageOverrideTable = new AccountUsageOverrideDataManager().GetTransactionAccountQueryTable(transactionAccountUsageQueries);

            return GetItemsSPCmd("VR_AccountBalance.sp_AccountUsage_GetByTransactionAccountUsageQueries", AccountUsageMapper, (cmd) =>
            {
                var sqlTableParameter = new System.Data.SqlClient.SqlParameter("@TransactionAccountUsageQueryTable", SqlDbType.Structured);
                sqlTableParameter.Value = accountUsageOverrideTable;
                cmd.Parameters.Add(sqlTableParameter);
            });
        }
        public bool OverrideAccountUsagesByAccountUsageIds(IEnumerable<long> accountUsageIds)
        {
            string accountUsageIdsAsString = (accountUsageIds != null) ? string.Join(",", accountUsageIds) : null;
            int affectedRows = ExecuteNonQuerySP("VR_AccountBalance.sp_AccountUsage_OverrideByAccountUsageIds", accountUsageIdsAsString);
            return (affectedRows > 0);
        }
        public bool RollbackOverridenAccountUsagesByAccountUsageIds(IEnumerable<long> accountUsageIds)
        {
            string accountUsageIdsAsString = (accountUsageIds != null) ? string.Join(",", accountUsageIds) : null;
            int affectedRows = ExecuteNonQuerySP("VR_AccountBalance.sp_AccountUsage_RollbackOverridenByAccountUsageIds", accountUsageIdsAsString);
            return (affectedRows > 0);
        }
        public IEnumerable<AccountUsage> GetOverridenAccountUsagesByDeletedTransactionIds(IEnumerable<long> deletedTransactionIds)
        {
            string deletedTransactionsAsString = (deletedTransactionIds != null) ? string.Join(",", deletedTransactionIds) : null;
            return GetItemsSP("VR_AccountBalance.sp_AccountUsage_GetOverridenByDeletedTransactionIds", AccountUsageMapper, deletedTransactionsAsString);
        }

        #endregion

        #region Mappers
        private DataTable GetAccountUsageTable()
        {
            DataTable dt = new DataTable(AccountUsage_TABLENAME);
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("UpdateValue", typeof(decimal));
            return dt;
        }
        private void FillAccountUsageTableRow(DataRow dr, AccountUsageToUpdate accountUsageToUpdate)
        {
            dr["ID"] = accountUsageToUpdate.AccountUsageId;
            dr["UpdateValue"] = accountUsageToUpdate.Value;
        }
        private AccountUsage AccountUsageMapper(IDataReader reader)
        {
            return new AccountUsage
            {
                AccountUsageId = GetReaderValue<long>(reader, "ID"),
                AccountId = reader["AccountId"] as string,
                TransactionTypeId = GetReaderValue<Guid>(reader, "TransactionTypeId"),
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeId"),
                PeriodStart = GetReaderValue<DateTime>(reader, "PeriodStart"),
                PeriodEnd = GetReaderValue<DateTime>(reader, "PeriodEnd"),
                UsageBalance = GetReaderValue<Decimal>(reader, "UsageBalance"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
                IsOverriden = GetReaderValue<bool>(reader, "IsOverridden"),
                OverridenAmount = GetReaderValue<decimal?>(reader, "OverriddenAmount"),
                CorrectionProcessId = GetReaderValue<Guid?>(reader, "CorrectionProcessID")
            };
        }
        private AccountUsageInfo AccountUsageInfoMapper(IDataReader reader)
        {
            return new AccountUsageInfo
            {
                AccountId = reader["AccountId"] as string,
                AccountUsageId = GetReaderValue<long>(reader, "ID"),
                TransactionTypeId = GetReaderValue<Guid>(reader, "TransactionTypeId"),
                IsOverridden = GetReaderValue<bool>(reader, "IsOverridden")
            };
        }
        #endregion
    }
}
