using System;
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
        public IEnumerable<BillingTransaction> GetFilteredBillingTransactions(BillingTransactionQuery query)
        {
            string accountsIds = null;
            if (query.AccountsIds != null && query.AccountsIds.Count() > 0)
                accountsIds = string.Join<long>(",", query.AccountsIds);

            return GetItemsSP("[VR_AccountBalance].[sp_BillingTransaction_GetFiltered]", BillingTransactionMapper, accountsIds);
        }
        public bool UpdateBillingTransactionBalanceStatus(List<long> billingTransactionIds)
        {
            string billingTransactionIDs = null;
            if (billingTransactionIds != null && billingTransactionIds.Count() > 0)
                billingTransactionIDs = string.Join<long>(",", billingTransactionIds);
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_SetBalanceUpdated]", billingTransactionIDs) > 0);
        }
        public void GetBillingTransactionsByBalanceUpdated(Action<BillingTransaction> onBillingTransactionReady)
        {
            ExecuteReaderSP("[VR_AccountBalance].[sp_BillingTransaction_GetBalanceNotUpdated]",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        onBillingTransactionReady(BillingTransactionMapper(reader));
                    }
                });
        }

        public bool InsertBillingTransactionFromLiveBalance(DateTime closingTime,Guid usageTransactionTypeId,long closingPeriodID)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_InsertFromLiveBalance]", closingTime, usageTransactionTypeId, closingPeriodID) > 0);
        }
        public bool UpdateBillingTransactionClosingPeriod(long closingPeriodID)
        {
            return (ExecuteNonQuerySP("[VR_AccountBalance].[sp_BillingTransaction_UpdateClosingPeriodId]", closingPeriodID) > 0);

        }
        #endregion


        #region Mappers

        private BillingTransaction BillingTransactionMapper(IDataReader reader)
        {
            return new BillingTransaction
            {
                AccountBillingTransactionId=(long)reader["ID"],
                AccountId = (long)reader["AccountID"],
                Amount = GetReaderValue<Decimal>(reader, "Amount"),
                CurrencyId = GetReaderValue<int>(reader, "CurrencyId"),
                Notes = reader["Notes"] as string,
                TransactionTime = GetReaderValue<DateTime>(reader, "TransactionTime"),
                TransactionTypeId = GetReaderValue<Guid>(reader, "TransactionTypeID"),
                Reference = reader["Reference"] as string
            };
        }

        #endregion

        public bool Insert(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            object billingTransactionID;
            int affectedRecords = ExecuteNonQuerySP("[VR_AccountBalance].sp_BillingTransaction_Insert", out billingTransactionID, billingTransaction.AccountId, billingTransaction.Amount,
                                                     billingTransaction.CurrencyId, billingTransaction.TransactionTypeId, billingTransaction.TransactionTime, billingTransaction.Notes, billingTransaction.Reference);

            if (affectedRecords > 0)
            {  
                billingTransactionId = (int)billingTransactionID;
                return true;
            }  

            billingTransactionId = -1;
            return false;
        }
    }
}
