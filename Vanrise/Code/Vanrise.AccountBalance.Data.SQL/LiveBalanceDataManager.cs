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
using Vanrise.Entities;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class LiveBalanceDataManager : BaseSQLDataManager, ILiveBalanceDataManager
    {
        #region Fields / Constructors

        private const string LiveBalance_TABLENAME = "LiveBalance";

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
        public void GetLiveBalanceAccounts(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetAll]",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       onLiveBalanceReady(LiveBalanceMapper(reader));
                   }
               }, accountTypeId);
        }

        public IEnumerable<Vanrise.AccountBalance.Entities.AccountBalance> GetFilteredAccountBalances(AccountBalanceQuery query)
        {

            return GetItemsText(GetAccountBalanceQuery(query), AccountBalanceMapper, (cmd) =>
            {
                cmd.Parameters.Add(new SqlParameter("@Top", query.Top));
                cmd.Parameters.Add(new SqlParameter("@AccountTypeID", query.AccountTypeId));
                cmd.Parameters.Add(new SqlParameter("@OrderBy", query.OrderBy));
                if (query.EffectiveDate.HasValue)
                    cmd.Parameters.Add(new SqlParameter("@EffectiveDate", query.EffectiveDate.Value));
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
        public void GetLiveBalancesToAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetBalancesForAlert]",
               (reader) =>
               {
                   while (reader.Read())
                   {
                       onLiveBalanceReady(LiveBalanceMapper(reader));
                   }
               }, accountTypeId);
        }
        public void GetLiveBalancesToClearAlert(Guid accountTypeId, Action<LiveBalance> onLiveBalanceReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_LiveBalance_GetBalancesToClearAlert]",
                 (reader) =>
                 {
                     while (reader.Read())
                     {
                         onLiveBalanceReady(LiveBalanceMapper(reader));
                     }
                 }, accountTypeId);
        }

        public bool HasLiveBalancesUpdateData(Guid accountTypeId)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[VR_AccountBalance].[sp_LiveBalance_HasBalancesUpdateData]", accountTypeId));
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

        public LiveBalanceAccountInfo TryAddLiveBalanceAndGet(String accountId, Guid accountTypeId, decimal initialBalance, int currencyId, decimal currentBalance, DateTime? bed, DateTime? eed, VRAccountStatus status, bool isDeleted)
        {
            return GetItemSP("[VR_AccountBalance].[sp_LiveBalance_TryAddAndGet]", LiveBalanceAccountInfoMapper, accountId, accountTypeId, initialBalance, currencyId, currentBalance, bed, eed, status, isDeleted);
        }
        public bool UpdateLiveBalanceAndAccountUsageFromBalanceUsageQueue(long balanceUsageQueueId, IEnumerable<LiveBalanceToUpdate> liveBalnacesToUpdate, IEnumerable<AccountUsageToUpdate> accountsUsageToUpdate, Guid? correctionProcessId)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                if (liveBalnacesToUpdate != null && liveBalnacesToUpdate.Count() > 0)
                    UpdateLiveBalances(liveBalnacesToUpdate);

                if (accountsUsageToUpdate != null && accountsUsageToUpdate.Count() > 0)
                    new AccountUsageDataManager().UpdateAccountUsageFromBalanceUsageQueue(accountsUsageToUpdate, correctionProcessId);

                new BalanceUsageQueueDataManager().DeleteBalanceUsageQueue(balanceUsageQueueId);
                scope.Complete();
            }
            return true;
        }
        public bool UpdateLiveBalancesFromBillingTransactions(IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate, IEnumerable<long> billingTransactionIds, IEnumerable<long> accountUsageIdsToOverride, IEnumerable<AccountUsageOverride> accountUsageOverrides, IEnumerable<long> overridenAccountUsageIdsToRollback, IEnumerable<long> deletedTransactionIds)
        {
            var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = TransactionManager.DefaultTimeout
            };

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                var accountUsageDataManager = new AccountUsageDataManager();
                if (overridenAccountUsageIdsToRollback != null && overridenAccountUsageIdsToRollback.Count() > 0)
                    accountUsageDataManager.RollbackOverridenAccountUsagesByAccountUsageIds(overridenAccountUsageIdsToRollback);
                if (accountUsageIdsToOverride != null && accountUsageIdsToOverride.Count() > 0)
                    accountUsageDataManager.OverrideAccountUsagesByAccountUsageIds(accountUsageIdsToOverride);

                var accountUsageOverrideDataManager = new AccountUsageOverrideDataManager();
                if (deletedTransactionIds != null && deletedTransactionIds.Count() > 0)
                    accountUsageOverrideDataManager.Delete(deletedTransactionIds);
                if (accountUsageOverrides != null && accountUsageOverrides.Count() > 0)
                    accountUsageOverrideDataManager.Insert(accountUsageOverrides);

                var billingTransactionDataManager = new BillingTransactionDataManager();
                if (deletedTransactionIds != null && deletedTransactionIds.Count() > 0)
                    billingTransactionDataManager.SetBillingTransactionsAsSubtractedFromBalance(deletedTransactionIds);
                if (billingTransactionIds != null && billingTransactionIds.Count() > 0)
                    billingTransactionDataManager.UpdateBillingTransactionBalanceStatus(billingTransactionIds);

                UpdateLiveBalances(liveBalancesToUpdate);
                scope.Complete();
            }

            return true;
        }
        public bool CheckIfAccountHasTransactions(Guid accountTypeId, string accountId)
        {
            object hasTransactions;
            ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_CheckIfHasTransactions]", out hasTransactions, accountTypeId, accountId);
            return (bool)hasTransactions;
        }
        public bool TryUpdateLiveBalanceStatus(string accountId, Guid accountTypeId, VRAccountStatus status, bool isDeleted)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_TryUpdateStatus]", accountId, accountTypeId, status, isDeleted) > 0);
        }
        public bool TryUpdateLiveBalanceEffectiveDate(string accountId, Guid accountTypeId, DateTime? bed, DateTime? eed)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_LiveBalance_TryUpdateEffectiveDate]", accountId, accountTypeId, bed, eed) > 0);
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
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                AlertRuleID = GetReaderValue<int?>(reader, "AlertRuleID"),
                InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
                NextThreshold = GetReaderValue<decimal?>(reader, "NextAlertThreshold"),
                LastExecutedThreshold = GetReaderValue<decimal?>(reader, "LastExecutedActionThreshold"),
                LiveBalanceActiveAlertsInfo = Serializer.Deserialize(reader["ActiveAlertsInfo"] as string, typeof(VRBalanceActiveAlertInfo)) as VRBalanceActiveAlertInfo,
                BED = GetReaderValue<DateTime?>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                Status = GetReaderValue<VRAccountStatus>(reader, "Status"),
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
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                InitialBalance = GetReaderValue<Decimal>(reader, "InitialBalance"),
                AlertRuleID = GetReaderValue<int>(reader, "AlertRuleID"),
                BED = GetReaderValue<DateTime?>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                Status = GetReaderValue<VRAccountStatus>(reader, "Status"),

            };
        }
        private LiveBalanceAccountInfo LiveBalanceAccountInfoMapper(IDataReader reader)
        {
            return new LiveBalanceAccountInfo
            {
                LiveBalanceId = (long)reader["ID"],
                AccountId = reader["AccountId"] as String,
                CurrencyId = GetReaderValue<int>(reader, "CurrencyID"),
                BED = GetReaderValue<DateTime?>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                Status = GetReaderValue<VRAccountStatus>(reader, "Status"),
            };
        }

        #endregion

        #region Private Methods

        private bool UpdateLiveBalances(IEnumerable<LiveBalanceToUpdate> liveBalancesToUpdate)
        {
            DataTable liveBalanceToUpdate = GetLiveBalanceTable();
            foreach (var item in liveBalancesToUpdate)
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
        private DataTable GetLiveBalanceThresholdTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("Threshold", typeof(decimal));
            dt.Columns.Add("AlertRuleId", typeof(int));
            return dt;
        }
        private DataTable GetAccountBalanceAlertRuleTable()
        {
            DataTable dt = new DataTable(LiveBalance_TABLENAME);
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("AlertRuleId", typeof(decimal));
            return dt;
        }
        private void FillNextThresholdUpdateRow(DataRow dr, LiveBalanceNextThresholdUpdateEntity updateEntity)
        {
            dr["AccountTypeID"] = updateEntity.AccountTypeId;
            dr["AccountID"] = updateEntity.AccountId;

            if (updateEntity.NextAlertThreshold.HasValue)
                dr["NextAlertThreshold"] = updateEntity.NextAlertThreshold;
            if (updateEntity.AlertRuleId.HasValue)
                dr["AlertRuleId"] = updateEntity.AlertRuleId;
        }
        private DataTable GetNextThresholdUpdateTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("AccountTypeID", typeof(Guid));
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("NextAlertThreshold", typeof(decimal));
            dt.Columns.Add("AlertRuleId", typeof(int));
            return dt;
        }
        private void FillLastThresholdUpdateRow(DataRow dr, LiveBalanceLastThresholdUpdateEntity updateEntity)
        {
            dr["AccountTypeID"] = updateEntity.AccountTypeId;
            dr["AccountID"] = updateEntity.AccountId;
            if (updateEntity.LastExecutedActionThreshold.HasValue)
                dr["LastExecutedActionThreshold"] = updateEntity.LastExecutedActionThreshold;
            if (updateEntity.ActiveAlertsInfo != null)
                dr["ActiveAlertsInfo"] = Serializer.Serialize(updateEntity.ActiveAlertsInfo, true);
        }
        private DataTable GetLastThresholdUpdateTable()
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

            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                whereBuilder.Append(String.Format(@" AND lb.AccountID in ({0})", string.Join<String>(",", query.AccountsIds.Select(x => string.Format("'{0}'", x)))));

            if (query.Sign != null)
            {
                if (query.Sign.Length > 2)
                    throw new Exception(String.Format("Invalid Sign argument '{0}'", query.Sign));
                whereBuilder.Append(String.Format(@" AND  lb.CurrentBalance {0} {1}", query.Sign, query.Balance));
            }

            if (query.Status.HasValue)
                whereBuilder.Append(String.Format(@" AND  lb.[Status] = {0} ", (int)query.Status.Value));

            if (query.IsEffectiveInFuture.HasValue)
                whereBuilder.Append(String.Format(@" AND (({0} = 1 and (lb.EED IS NULL or lb.EED >=  GETDATE())) OR  ({0} = 0 and  lb.EED <=  GETDATE())) ", query.IsEffectiveInFuture.Value ? 1 : 0));

            if (query.EffectiveDate.HasValue)
                whereBuilder.Append(@" AND ((lb.BED <= @EffectiveDate OR lb.BED IS NULL) AND (lb.EED > @EffectiveDate OR lb.EED IS NULL))");

            StringBuilder queryBuilder = new StringBuilder(@"SELECT Top(@Top) lb.ID , lb.AccountTypeID , lb.AccountID , lb.CurrencyID , lb.InitialBalance, lb.CurrentBalance ,lb.AlertRuleID ,lb.BED,lb.EED,lb.[Status]
                                                                    FROM [VR_AccountBalance].[LiveBalance] as lb
                                                                    WHERE  (#WHEREPART#) AND ISNULL(IsDeleted,0) = 0
                                                                    ORDER BY  lb.CurrentBalance #ORDERDIRECTION#
                                                                    ");


            queryBuilder.Replace("#WHEREPART#", whereBuilder.ToString());

            queryBuilder.Replace("#ORDERDIRECTION#", query.OrderBy);

            return queryBuilder.ToString();
        }

        #endregion



    }
}
