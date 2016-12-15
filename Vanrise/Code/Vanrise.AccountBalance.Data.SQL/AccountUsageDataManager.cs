﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class AccountUsageDataManager:BaseSQLDataManager,IAccountUsageDataManager
    {

        #region ctor/Local Variables
        const string AccountUsage_TABLENAME = "AccountUsage";
        public AccountUsageDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }



        #endregion

        #region Public Methods
        public IEnumerable<AccountUsageInfo> GetAccountsUsageInfoByPeriod(Guid accountTypeId, DateTime periodStart)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetByPeriod]", AccountUsageInfoMapper, accountTypeId, periodStart);
        }
        public AccountUsageInfo TryAddAccountUsageAndGet(Guid accountTypeId, long accountId, DateTime periodStart, DateTime periodEnd, int currencyId, decimal usageBalance,string billingTransactionNote)
        {
            return GetItemSP("[VR_AccountBalance].[sp_AccountUsage_TryAddAndGet]", AccountUsageInfoMapper, accountTypeId, accountId, periodStart, periodEnd, currencyId, usageBalance, billingTransactionNote);
        }
        public bool UpdateAccountUsageFromBalanceUsageQueue(IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate)
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
                           });
            return true;
        }
        public IEnumerable<AccountUsage> GetPendingAccountUsages(Guid accountTypeId, long accountId)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_AccountUsage_GetAccountPendingUsage]", AccountUsageMapper, accountTypeId, accountId);
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
                AccountId = (long)reader["AccountId"],
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeId"),
                BillingTransactionId = GetReaderValue<long?>(reader, "BillingTransactionId"),
                ShouldRecreateTransaction = GetReaderValue<Boolean>(reader, "ShouldRecreateTransaction"),
                PeriodStart = GetReaderValue<DateTime>(reader, "PeriodStart"),
                PeriodEnd = GetReaderValue<DateTime>(reader, "PeriodEnd"),
                UsageBalance = GetReaderValue<Decimal>(reader, "UsageBalance"),
                BillingTransactionNote = reader["BillingTransactionNote"] as string,
                CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
            };
        }
        private AccountUsageInfo AccountUsageInfoMapper(IDataReader reader)
        {
            return new AccountUsageInfo
            {
                AccountId = (long)reader["AccountId"],
                AccountUsageId = GetReaderValue<long>(reader, "ID"),
            };
        }
        #endregion




       
    }
}
