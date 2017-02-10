using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;
using Vanrise.Notification.Entities;

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
        public LiveBalance GetLiveBalance(Guid accountTypeId, String accountId)
        {
            return GetItemSP("[VR_AccountBalance].[sp_LiveBalance_GetById]", LiveBalanceMapper, accountTypeId, accountId);
        }
        public IEnumerable<LiveBalanceAccountInfo> GetLiveBalanceAccountsInfo(Guid accountTypeId)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_LiveBalance_GetAccountsInfo]", LiveBalanceAccountInfoMapper, accountTypeId);
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

        public IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> GetFilteredAccountBalances(AccountBalanceQuery query)
        {

            return GetItemsText(GetAccountBalanceQuery(query), AccountBalanceMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Top", query.Top));
                cmd.Parameters.Add(new SqlParameter("@AccountTypeID", query.AccountTypeId));
                cmd.Parameters.Add(new SqlParameter("@OrderBy", query.OrderBy));

            });

        }

       
        public bool ResetInitialAndUsageBalance(Guid accountTypeId)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_ResetInitialAndUsage]", accountTypeId) > 0);
        }
        public bool UpdateLiveBalanceThreshold(Guid accountTypeId, List<BalanceAccountThreshold> balanceAccountsThresholds)
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
                               var dtPrm = new System.Data.SqlClient.SqlParameter("@AccountTypeId", SqlDbType.UniqueIdentifier);
                               dtPrm.Value = accountTypeId;
                               cmd.Parameters.Add(dtPrm);

                               dtPrm = new System.Data.SqlClient.SqlParameter("@LiveBalanceThresholdTable", SqlDbType.Structured);
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
        public void UpdateBalanceRuleInfos(List<LiveBalanceNextThresholdUpdateEntity> updateEntities)
        {
            DataTable liveBalanceThresholdToUpdate = GetNextThresholdUpdateTable();
            foreach (var liveBalance in updateEntities)
            {
                DataRow dr = liveBalanceThresholdToUpdate.NewRow();
                FillNextThresholdUpdateRow(dr, liveBalance);
                liveBalanceThresholdToUpdate.Rows.Add(dr);
            }
            liveBalanceThresholdToUpdate.EndLoadData();
            if (liveBalanceThresholdToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_LiveBalance_UpdateBalanceThresholdAndRule]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@LiveBalanceThresholdUpdateTable", SqlDbType.Structured);
                           dtPrm.Value = liveBalanceThresholdToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
        }
        public void GetLiveBalancesToAlert(Action<LiveBalance> onLiveBalanceReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetBalancesForAlert]",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       onLiveBalanceReady(LiveBalanceMapper(reader));
                   }
               });
        }
        public void GetLiveBalancesToClearAlert(Action<LiveBalance> onLiveBalanceReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetBalancesToClearAlert]",
                 (reader) =>
                 {
                     while (reader.Read())
                     {
                         onLiveBalanceReady(LiveBalanceMapper(reader));
                     }
                 });
        }
        public void UpdateBalanceLastAlertInfos(List<LiveBalanceLastThresholdUpdateEntity> updateEntities)
        {
            DataTable lastThresholdToUpdate = GetLastThresholdUpdateTable();
            foreach (var updateEntity in updateEntities)
            {
                DataRow dr = lastThresholdToUpdate.NewRow();
                FillLastThresholdUpdateRow(dr, updateEntity);
                lastThresholdToUpdate.Rows.Add(dr);
            }
            lastThresholdToUpdate.EndLoadData();
            if (lastThresholdToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_LiveBalance_UpdateBalanceLastThreshold]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@LiveBalanceLastThresholdUpdateTable", SqlDbType.Structured);
                           dtPrm.Value = lastThresholdToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
        }

        public LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal usageBalance, decimal currentBalance)
        {
            return GetItemSP("[VR_AccountBalance].[sp_LiveBalance_TryAddAndGet]", LiveBalanceAccountInfoMapper, accountId, accountTypeId, initialBalance, currencyId, usageBalance, currentBalance);
        }
        public bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {

                UpdateLiveBalancetoUpdate(liveBalnacesToUpdate);
                AccountUsageDataManager accountUsageDataManager = new SQL.AccountUsageDataManager();
                accountUsageDataManager.UpdateAccountUsageFromBalanceUsageQueue(accountsUsageToUpdate, correctionProcessId);
                BalanceUsageQueueDataManager dataManager = new BalanceUsageQueueDataManager();
                dataManager.DeleteBalanceUsageQueue(balanceUsageQueueId);
                scope.Complete();
            }
            return true;

        }
        public bool UpdateLiveBalanceFromBillingTransaction(IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, List<long> billingTransactionIds)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                UpdateLiveBalancetoUpdate(liveBalnacesToUpdate);
                BillingTransactionDataManager dataManager = new BillingTransactionDataManager();
                dataManager.UpdateBillingTransactionBalanceStatus(billingTransactionIds);
                scope.Complete();
            }
            return true;
        }
        #endregion

        #region Mappers
        private LiveBalance LiveBalanceMapper(IDataReader reader)
        {
            return new LiveBalance
            {
                CurrentBalance = GetReaderValue<Decimal>(reader, "CurrentBalance"),
                AccountId = reader["AccountId"] as string,
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeID"),
                UsageBalance = GetReaderValue<Decimal>(reader, "UsageBalance"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                AlertRuleID = GetReaderValue<int?>(reader, "AlertRuleID"),
                InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
                NextThreshold = GetReaderValue<decimal?>(reader, "NextAlertThreshold"),
                LastExecutedThreshold = GetReaderValue<decimal?>(reader, "LastExecutedActionThreshold"),
                ThresholdIndex = GetReaderValue<int?>(reader, "ThresholdActionIndex"),
                LiveBalanceActiveAlertsInfo = Serializer.Deserialize(reader["ActiveAlertsInfo"] as string, typeof(VRBalanceActiveAlertInfo)) as VRBalanceActiveAlertInfo
            };
        }

        private Vanrise.AccountBalance.Entities.AccountBalance AccountBalanceMapper(IDataReader reader)
        {
            return new Vanrise.AccountBalance.Entities.AccountBalance
            {
                AccountBalanceId = (long)reader["ID"],
                AccountId = reader["AccountId"] as String,
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeID"),
                CurrentBalance = GetReaderValue<Decimal>(reader, "CurrentBalance"),
                UsageBalance = GetReaderValue<Decimal>(reader, "UsageBalance"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
               
            };
        }
        private LiveBalanceAccountInfo LiveBalanceAccountInfoMapper(IDataReader reader)
        {
            return new LiveBalanceAccountInfo
            {
                LiveBalanceId = (long)reader["ID"],
                AccountId = reader["AccountId"] as String,
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
            };
        }

        #endregion

        #region Private Methods

        private bool UpdateLiveBalancetoUpdate(IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate)
        {
            DataTable liveBalanceToUpdate = GetLiveBalanceTable();
            foreach (var item in liveBalnacesToUpdate)
            {
                DataRow dr = liveBalanceToUpdate.NewRow();
                FillLiveBalanceTableRow(dr, item);
                liveBalanceToUpdate.Rows.Add(dr);
            }
            liveBalanceToUpdate.EndLoadData();
            if (liveBalanceToUpdate.Rows.Count > 0)
                ExecuteNonQuerySPCmd("[VR_AccountBalance].[sp_LiveBalance_UpdateBalance]",
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@BalanceTable", SqlDbType.Structured);
                           dtPrm.Value = liveBalanceToUpdate;
                           cmd.Parameters.Add(dtPrm);
                       });
            return true;
        }
        private DataTable GetLiveBalanceTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("ID", typeof(long));
            dt.Columns.Add("UpdateValue", typeof(decimal));
            return dt;
        }
        private void FillLiveBalanceTableRow(DataRow dr, LiveBalanceToUpdate liveBalanceToUpdate)
        {
            dr["ID"] = liveBalanceToUpdate.LiveBalanceId;
            dr["UpdateValue"] = liveBalanceToUpdate.Value;
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
            dt.Columns.Add("AccountID", typeof(String));
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
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("AlertRuleId", typeof(decimal));
            return dt;
        }
        void FillNextThresholdUpdateRow(DataRow dr, LiveBalanceNextThresholdUpdateEntity updateEntity)
        {
            dr["AccountTypeID"] = updateEntity.AccountTypeId;
            dr["AccountID"] = updateEntity.AccountId;

            if (updateEntity.NextAlertThreshold.HasValue)
                dr["NextAlertThreshold"] = updateEntity.NextAlertThreshold;
            if (updateEntity.AlertRuleId.HasValue)
                dr["AlertRuleId"] = updateEntity.AlertRuleId;
            if (updateEntity.ThresholdActionIndex.HasValue)
                dr["ThresholdActionIndex"] = updateEntity.ThresholdActionIndex;
        }
        DataTable GetNextThresholdUpdateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AccountTypeID", typeof(Guid));
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("NextAlertThreshold", typeof(decimal));
            dt.Columns.Add("AlertRuleId", typeof(int));
            dt.Columns.Add("ThresholdActionIndex", typeof(int));
            return dt;
        }
        void FillLastThresholdUpdateRow(DataRow dr, LiveBalanceLastThresholdUpdateEntity updateEntity)
        {
            dr["AccountTypeID"] = updateEntity.AccountTypeId;
            dr["AccountID"] = updateEntity.AccountId;
            if (updateEntity.LastExecutedActionThreshold.HasValue)
                dr["LastExecutedActionThreshold"] = updateEntity.LastExecutedActionThreshold;
            if (updateEntity.ActiveAlertsInfo != null)
                dr["ActiveAlertsInfo"] = Serializer.Serialize(updateEntity.ActiveAlertsInfo, true);
        }
        DataTable GetLastThresholdUpdateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AccountTypeID", typeof(Guid));
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("LastExecutedActionThreshold", typeof(decimal));
            dt.Columns.Add("ActiveAlertsInfo", typeof(string));
            return dt;
        }

        private string GetAccountBalanceQuery(AccountBalanceQuery query)
        {
            StringBuilder whereBuilder = new StringBuilder(@"lb.AccountTypeID = @AccountTypeID");

            if (query.AccountsIds!=null && query.AccountsIds.Count() > 0)
                whereBuilder.Append(String.Format(@" AND lb.AccountID in ({0})", string.Join<String>(",", query.AccountsIds)));

            if (query.Sign != null)
                whereBuilder.Append(String.Format(@" AND  lb.CurrentBalance {0} {1}", query.Sign , query.Balance));

            StringBuilder queryBuilder = new StringBuilder(@"SELECT Top(@Top) lb.ID , lb.AccountTypeID , lb.AccountID , lb.CurrencyID , lb.InitialBalance, lb.UsageBalance, lb.CurrentBalance 
                                                                    FROM [VR_AccountBalance].[LiveBalance] as lb
                                                                    WHERE  (#WHEREPART#) 
                                                                    ORDER BY  lb.CurrentBalance #ORDERDIRECTION#
                                                                    ");


            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());

            queryBuilder.Replace("#ORDERDIRECTION#", query.OrderBy);

            return queryBuilder.ToString();
        }


        #endregion

    }
}
