﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Data.SQL;

namespace Vanrise.AccountBalance.Data.SQL
{
    public class BillingTransactionDataManager : BaseSQLDataManager, IBillingTransactionDataManager
    {
        #region ctor/Local Variables
        public BillingTransactionDataManager()
            : base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
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


            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetFiltered]", BillingTransactionMapper, accountsIds, transactionTypeIds, query.AccountTypeId, query.FromTime, query.ToTime);
        }
        public bool UpdateBillingTransactionBalanceStatus(List<long> billingTransactionIds)
        {
            string billingTransactionIDs = null;
            if (billingTransactionIds != null && billingTransactionIds.Count() > 0)
                billingTransactionIDs = string.Join<long>(",", billingTransactionIds);
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_SetBalanceUpdated]", billingTransactionIDs) > 0);
        }
        public void GetBillingTransactionsByBalanceUpdated(Guid accountTypeId, Action<BillingTransaction> onBillingTransactionReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_BillingTransaction_GetBalanceNotUpdated]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onBillingTransactionReady(BillingTransactionMapper(reader));
                    }
                }, accountTypeId);
        }
        public bool Insert(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            object billingTransactionID;
            int affectedRecords = ExecuteNonQuerySP("[VR_AccountBalance].sp_BillingTransaction_Insert",
                                                        out billingTransactionID,
                                                        billingTransaction.AccountId,
                                                        billingTransaction.AccountTypeId,
                                                        billingTransaction.Amount,
                                                        billingTransaction.CurrencyId,
                                                        billingTransaction.TransactionTypeId,
                                                        billingTransaction.TransactionTime,
                                                        billingTransaction.Notes,
                                                        billingTransaction.Reference,
                                                        billingTransaction.SourceId);

            if (affectedRecords > 0)
            {
                billingTransactionId = (int)billingTransactionID;
                return true;
            }
            billingTransactionId = -1;
            return false;
        }
        public bool InsertBillingTransactionFromAccountUsageAndUpdate(Guid accountTypeId, TimeSpan timeOffset)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_InsertFromAccountUsageAndUpdate]", accountTypeId, timeOffset.TotalSeconds) > 0);
        }
        public IEnumerable<BillingTransaction> GetBillingTransactionsForSynchronizerProcess(List<Guid> billingTransactionTypeIds, Guid accountTypeId)
        {
            string transactionTypeIds = null;
            if (billingTransactionTypeIds != null && billingTransactionTypeIds.Count() > 0)
                transactionTypeIds = string.Join<Guid>(",", billingTransactionTypeIds);
            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetForSynchronizerProcess]", BillingTransactionMapper, transactionTypeIds, accountTypeId);
        }
        #endregion

        #region Mappers

        private BillingTransaction BillingTransactionMapper(IDataReader reader)
        {
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
                SourceId = reader["SourceId"] as string
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
