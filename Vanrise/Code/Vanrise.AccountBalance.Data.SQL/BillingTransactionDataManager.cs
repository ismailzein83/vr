using System;
using System.Collections.Generic;
using System.Data;
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
                TransactionTypeId = GetReaderValue<Guid>(reader, "TransactionTypeID")

            };
        }

        #endregion

        public bool Insert(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            object billingTransactionID;
            int affectedRecords = ExecuteNonQuerySP("[VR_AccountBalance].sp_BillingTransaction_Insert", out billingTransactionID, billingTransaction.AccountId, billingTransaction.Amount, billingTransaction.CurrencyId, billingTransaction.TransactionTypeId, billingTransaction.TransactionTime, billingTransaction.Notes);

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
