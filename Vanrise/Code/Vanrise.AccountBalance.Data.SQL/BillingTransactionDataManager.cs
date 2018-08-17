using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class BillingTransactionDataManager : BaseSQLDataManager, IBillingTransactionDataManager
    {
        #region Fields / Constructors

        const string BillingTransactionByTime_TABLENAME = "BillingTransactionByTime";

        public BillingTransactionDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }

        #endregion

        #region Public Methods

        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds)
        {

            string transactionTypeIDs = null;
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                transactionTypeIDs = string.Join<Guid>(",", transactionTypeIds);


            DataTable billingTransactionsByTimeTable = GetBillingTransactionsByTimeTable();
            foreach (var billingTransactionByTime in billingTransactionsByTime)
            {
                DataRow dr = billingTransactionsByTimeTable.NewRow();
                FillBillingTransactionsByTimeRow(dr, billingTransactionByTime);
                billingTransactionsByTimeTable.Rows.Add(dr);
            }
            billingTransactionsByTimeTable.EndLoadData();
            if (billingTransactionsByTimeTable.Rows.Count > 0)
                return GetItemsSPCmd("[VR_AccountBalance].[sp_BillingTransaction_GetByTime]", BillingTransactionMetaDataMapper,
                       (cmd) =>
                       {
                           var dtPrm = new System.Data.SqlClient.SqlParameter("@BillingTransactionsByTimeTable", SqlDbType.Structured);
                           dtPrm.Value = billingTransactionsByTimeTable;
                           cmd.Parameters.Add(dtPrm);

                           var accountTypeIdPrm = new System.Data.SqlClient.SqlParameter("@AccountTypeId", accountTypeId);
                           cmd.Parameters.Add(accountTypeIdPrm);

                           var transactionTypeIdsPrm = new System.Data.SqlClient.SqlParameter("@TransactionTypeIds", transactionTypeIDs);
                           cmd.Parameters.Add(transactionTypeIdsPrm);
                       });
            return null;
        }

        DataTable GetBillingTransactionsByTimeTable()
        {
            DataTable dt = new DataTable(BillingTransactionByTime_TABLENAME);
            dt.Columns.Add("AccountID", typeof(String));
            dt.Columns.Add("TransactionTime", typeof(DateTime));
            return dt;
        }
        void FillBillingTransactionsByTimeRow(DataRow dr, BillingTransactionByTime billingTransactionByTime)
        {
            dr["AccountID"] = billingTransactionByTime.AccountId;
            dr["TransactionTime"] = billingTransactionByTime.TransactionTime;
        }

        public List<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            string accountsIDs = null;
            if (accountIds != null && accountIds.Count() > 0)
                accountsIDs = string.Join<String>(",", accountIds);

            string transactionTypeIDs = null;
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                transactionTypeIDs = string.Join<Guid>(",", transactionTypeIds);
            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetByAccountIds]", BillingTransactionMetaDataMapper, accountTypeId, accountsIDs, transactionTypeIDs);
        }
        public IEnumerable<BillingTransaction> GetFilteredBillingTransactions(BillingTransactionQuery query)
        {
            string accountsIds = null;
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                accountsIds = string.Join<String>(",", query.AccountsIds);

            string transactionTypeIds = null;
            if (query.TransactionTypeIds != null && query.TransactionTypeIds.Count() > 0)
                transactionTypeIds = string.Join<Guid>(",", query.TransactionTypeIds);


            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetFiltered]", BillingTransactionMapper, accountsIds, transactionTypeIds, query.AccountTypeId, query.FromTime, query.ToTime,query.Status,query.EffectiveDate,query.IsEffectiveInFuture);
        }
        public bool UpdateBillingTransactionBalanceStatus(IEnumerable<long> billingTransactionIds)
        {
            string billingTransactionIDs = null;
            if (billingTransactionIds != null && billingTransactionIds.Count() > 0)
                billingTransactionIDs = string.Join<long>(",", billingTransactionIds);
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_SetBalanceUpdated]", billingTransactionIDs) > 0);
        }
        public BillingTransaction GetBillingTransactionById(long billingTransactionId)
        {
            return GetItemSP("[VR_AccountBalance].[sp_BillingTransaction_GetById]", BillingTransactionMapper, billingTransactionId);
        }
        public bool SetBillingTransactionsAsSubtractedFromBalance(IEnumerable<long> billingTransactionIds)
        {
            string billingTransactionIdsAsString = (billingTransactionIds != null) ? string.Join(",", billingTransactionIds) : null;
            int affectedRows = ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_SetAsSubtractedFromBalance]", billingTransactionIdsAsString);
            return affectedRows > 0;
        }
        public bool SetBillingTransactionsAsDeleted(long deletedInvoiceId)
        {
            int affectedRows = ExecuteNonQuerySP("VR_AccountBalance.sp_BillingTransaction_SetAsDeletedByInvoiceID", deletedInvoiceId);
            return affectedRows > 0;
        }
        public void GetBillingTransactionsByBalanceUpdated(Guid accountTypeId, Action<BillingTransaction> onBillingTransactionReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_BillingTransaction_GetTransactionsToProcess]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onBillingTransactionReady(BillingTransactionMapper(reader));
                    }
                }, accountTypeId);
        }

        public bool HasBillingTransactionData(Guid accountTypeId)
        {
            return Convert.ToBoolean(ExecuteScalarSP("[VR_AccountBalance].[sp_BillingTransaction_HasData]", accountTypeId));
        }

        public bool Insert(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            return Insert(billingTransaction, null, out billingTransactionId);
        }
      
        public bool Insert(BillingTransaction billingTransaction, long? invoiceId, out long billingTransactionId)
        {
            object billingTransactionID;

            int affectedRecords = ExecuteNonQuerySP
            (
                "[VR_AccountBalance].sp_BillingTransaction_Insert",
                out billingTransactionID,
                billingTransaction.AccountId,
                billingTransaction.AccountTypeId,
                billingTransaction.Amount,
                billingTransaction.CurrencyId,
                billingTransaction.TransactionTypeId,
                billingTransaction.TransactionTime,
                billingTransaction.Notes,
                billingTransaction.Reference,
                billingTransaction.SourceId,
                (billingTransaction.Settings != null) ? Vanrise.Common.Serializer.Serialize(billingTransaction.Settings) : null,
                invoiceId
            );

            if (affectedRecords > 0)
            {
                billingTransactionId = (int)billingTransactionID;
                return true;
            }

            billingTransactionId = -1;
            return false;
        }
        public IEnumerable<BillingTransaction> GetBillingTransactions(List<Guid> accountTypeIds, List<string> accountIds, List<Guid> transactionTypeIds, DateTime fromDate, DateTime? toDate)
        {
            string accountsIDs = null;
            if (accountIds != null && accountIds.Count() > 0)
                accountsIDs = string.Join<String>(",", accountIds);

            string transactionTypeIDs = null;
            if (transactionTypeIds != null && transactionTypeIds.Count() > 0)
                transactionTypeIDs = string.Join<Guid>(",", transactionTypeIds);

            string accountTypeIDs = null;
            if (accountTypeIds != null && accountTypeIds.Count() > 0)
                accountTypeIDs = string.Join<Guid>(",", accountTypeIds);
            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetByPeriod]", BillingTransactionMapper, accountTypeIDs, accountsIDs, transactionTypeIDs, fromDate, toDate);
        }
        public IEnumerable<BillingTransaction> GetBillingTransactionsByAccountId(Guid accountTypeId, string accountId, VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetByAccount]", BillingTransactionMapper, accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);
        }
        public IEnumerable<BillingTransaction> GetBillingTransactionsForSynchronizerProcess(List<Guid> billingTransactionTypeIds, Guid accountTypeId)
        {
            string transactionTypeIds = null;
            if (billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0)
                transactionTypeIds = string.Join<Guid>(",", billingTransactionTypeIds);
            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetForSynchronizerProcess]", BillingTransactionMapper, transactionTypeIds, accountTypeId);
        }

        public BillingTransaction GetLastBillingTransaction(Guid accountTypeId, string accountId)
        {
            return GetItemSP("[VR_AccountBalance].[sp_BillingTransaction_GetLast]", BillingTransactionMapper, accountTypeId, accountId);
        }
        #endregion

        #region Mappers

        private BillingTransaction BillingTransactionMapper(IDataReader reader)
        {
            string settingsAsString = reader["Settings"] as string;

            return new BillingTransaction
            {
                AccountBillingTransactionId = (long)reader["ID"],
                AccountId = reader["AccountID"] as string,
                AccountTypeId = GetReaderValue<Guid>(reader, "AccountTypeId"),
                Amount = GetReaderValue<Decimal>(reader, "Amount"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
                Notes = reader["Notes"] as string,
                TransactionTime = GetReaderValue<DateTime>(reader, "TransactionTime"),
                TransactionTypeId = GetReaderValue<Guid>(reader, "TransactionTypeID"),
                Reference = reader["Reference"] as string,
                IsBalanceUpdated = GetReaderValue<bool>(reader, "IsBalanceUpdated"),
                SourceId = reader["SourceId"] as string,
                Settings = (settingsAsString != null) ? Vanrise.Common.Serializer.Deserialize<BillingTransactionSettings>(settingsAsString) : null,
                IsDeleted = GetReaderValue<bool>(reader, "IsDeleted"),
                IsSubtractedFromBalance = GetReaderValue<bool>(reader, "IsSubtractedFromBalance")
            };
        }

        private BillingTransactionMetaData BillingTransactionMetaDataMapper(IDataReader reader)
        {
            return new BillingTransactionMetaData
        {
            AccountId = reader["AccountID"] as string,
            Amount = GetReaderValue<Decimal>(reader, "Amount"),
            CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
            TransactionTime = GetReaderValue<DateTime>(reader, "TransactionTime"),
            TransactionTypeId = GetReaderValue<Guid>(reader, "TransactionTypeID"),
        };
        }
        #endregion





      
    }
}
