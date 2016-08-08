using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class LiveBalanceDataManager : BaseSQLDataManager, ILiveBalanceDataManager
    {
         
        #region ctor/Local Variables
        const string LiveBalance_TABLENAME = "LiveBalance";
        public LiveBalanceDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public LiveBalance GetLiveBalance(long accountId)
        {
            return GetItemSP("[VR_AccountBalance].[sp_LiveBalance_GetById]", LiveBalanceMapper, accountId);
        }
        public bool UpdateLiveBalanceFromBillingTransaction(long accountId, List<long> billingTransactionIds, decimal amount)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_UpdateFromBillingTransaction]", accountId, amount);

                BillingTransactionDataManager dataManager = new BillingTransactionDataManager();
                dataManager.UpdateBillingTransactionBalanceStatus(billingTransactionIds);
                scope.Complete();
            } 
            return true;
        }
        public bool UpdateLiveBalanceFromBalanceUsageQueue(IEnumerable<UsageBalanceUpdate> groupedResult, long balanceUsageQueueId)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                DataTable liveBalanceToUpdate = GetLiveBalancesTable();
                foreach (var item in groupedResult)
                {
                    DataRow dr = liveBalanceToUpdate.NewRow();
                    FillLiveBalanceRow(dr, item);
                    liveBalanceToUpdate.Rows.Add(dr);
                }
                liveBalanceToUpdate.EndLoadData();
                if (liveBalanceToUpdate.Rows.Count > 0)
                 ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_LiveBalance_UpdateFromBalanceUsageQueue]",
                        (cmd) =>
                        {
                            var dtPrm = new System.Data.SqlClient.SqlParameter("@LiveBalanceTable", SqlDbType.Structured);
                            dtPrm.Value = liveBalanceToUpdate;
                            cmd.Parameters.Add(dtPrm);
                        });
                BalanceUsageQueueDataManager dataManager = new BalanceUsageQueueDataManager();
                dataManager.DeleteBalanceUsageQueue(balanceUsageQueueId);
                scope.Complete();
            }
            return true;
        }
        public  IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo()
        {
            return GetItemsSP("[VR_AccountBalance].[sp_LiveBalance_GetAccountsInfo]", LiveBalanceAccountInfoMapper);
        }
        public void GetLiveBalanceAccounts(Action<LiveBalance> onLiveBalanceReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetAll]",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       onLiveBalanceReady(LiveBalanceMapper(reader));
                   }
               });
        }
        public bool AddLiveBalance(long accountId, decimal initialBalance, int currencyId, decimal usageBalance, decimal currentBalance)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_Insert]", accountId, initialBalance, currencyId, usageBalance, currentBalance) > 0);
        }
        public bool UpdateLiveBalanceBalance()
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_UpdateBalance]") > 0);
        }

        public bool UpdateLiveBalanceThreshold(List<BalanceAccountThreshold> balanceAccountsThresholds)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                DataTable liveBalanceThresholdToUpdate = GetLiveBalanceThresholdTable();
                foreach (var item in balanceAccountsThresholds)
                {
                    DataRow dr = liveBalanceThresholdToUpdate.NewRow();
                    FillLiveBalanceThresholdRow(dr, item);
                    liveBalanceThresholdToUpdate.Rows.Add(dr);
                }
                liveBalanceThresholdToUpdate.EndLoadData();
                if (liveBalanceThresholdToUpdate.Rows.Count > 0)
                    ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_LiveBalance_UpdateBalanceThreshold]",
                           (cmd) =>
                           {
                               var dtPrm = new System.Data.SqlClient.SqlParameter("@LiveBalanceThresholdTable", SqlDbType.Structured);
                               dtPrm.Value = liveBalanceThresholdToUpdate;
                               cmd.Parameters.Add(dtPrm);
                           });
                scope.Complete();
            }
            return true;
        }

        public void UpdateLiveBalanceAlertRule(List<AccountBalanceAlertRule> accountBalanceAlertRules)
        {
            DataTable liveBalanceAlertRuleTable = GetAccountBalanceAlertRuleTable();
            foreach (var item in accountBalanceAlertRules)
            {
                DataRow dr = liveBalanceAlertRuleTable.NewRow();
                FillLiveBalanceAlerRuleRow(dr, item);
                liveBalanceAlertRuleTable.Rows.Add(dr);
            }
            liveBalanceAlertRuleTable.EndLoadData();
            if (liveBalanceAlertRuleTable.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_LiveBalance_UpdateAlertRule]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@LiveBalanceAlertRuleTable", SqlDbType.Structured);
                           dtPrm.Value = liveBalanceAlertRuleTable;
                           cmd.Parameters.Add(dtPrm);
                       });
        }

        #endregion

        #region Mappers

        private LiveBalance LiveBalanceMapper(IDataReader reader)
        {
            return new LiveBalance
            {
                CurrentBalance = GetReaderValue<Decimal>(reader, "CurrentBalance"),
                AccountId = (long)reader["AccountId"],
                UsageBalance = GetReaderValue<Decimal>(reader, "UsageBalance"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                AlertRuleID = GetReaderValue<int?>(reader, "AlertRuleID"),
                InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
            };
        }
        private LiveBalanceAccountInfo LiveBalanceAccountInfoMapper(IDataReader reader)
        {
            return new LiveBalanceAccountInfo
            {
                AccountId = (long)reader["AccountId"],
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
            };
        }

        #endregion

        #region Private Methods
        private void FillLiveBalanceRow(DataRow dr, UsageBalanceUpdate usageBalanceUpdate)
        {
            dr["AccountID"] = usageBalanceUpdate.AccountId;
            dr["UpdateValue"] = usageBalanceUpdate.Value;
        }
        private DataTable GetLiveBalancesTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("AccountID", typeof(long));
            dt.Columns.Add("UpdateValue", typeof(decimal));
            return dt;
        }

        private void FillLiveBalanceThresholdRow(DataRow dr, BalanceAccountThreshold balanceAccountThreshold)
        {
            dr["AccountID"] = balanceAccountThreshold.AccountId;
            dr["Threshold"] = balanceAccountThreshold.Threshold;
            dr["ThresholdActionIndex"] = balanceAccountThreshold.ThresholdActionIndex;
            dr["AlertRuleId"] = balanceAccountThreshold.AlertRuleId;
        }
        private DataTable GetLiveBalanceThresholdTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("AccountID", typeof(long));
            dt.Columns.Add("Threshold", typeof(decimal));
            dt.Columns.Add("ThresholdActionIndex", typeof(int));
            dt.Columns.Add("AlertRuleId", typeof(int));
            return dt;
        }

        private void FillLiveBalanceAlerRuleRow(DataRow dr, AccountBalanceAlertRule accountBalanceAlertRule)
        {
            dr["AccountID"] = accountBalanceAlertRule.AccountId;
            dr["AlertRuleId"] = accountBalanceAlertRule.AlertRuleId;
        }
        private DataTable GetAccountBalanceAlertRuleTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("AccountID", typeof(long));
            dt.Columns.Add("AlertRuleId", typeof(decimal));
            return dt;
        }

        #endregion

    }
}
