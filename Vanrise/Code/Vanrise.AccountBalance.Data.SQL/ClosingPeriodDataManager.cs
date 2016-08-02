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
    public class ClosingPeriodDataManager:BaseSQLDataManager,IClosingPeriodDataManager
    {
       
        #region ctor/Local Variables
        public ClosingPeriodDataManager(): base(GetConnectionStringName("VR_AccountBalance_TransactionDBConnStringKey", "VR_AccountBalance_TransactionDBConnString"))
        {
        }
        #endregion

        #region Public Methods
        public BalanceClosingPeriod GetLastClosingPeriod()
        {
            return GetItemSP("[VR_AccountBalance].[sp_BalanceClosingPeriod_GetLastClosingPeriod]", BalanceClosingPeriodMapper);
        }
        public void CreateClosingPeriod(DateTime balanceClosingPeriod,Guid usageTransactionTypeId)
        {
             var options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
            };

             using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
             {
                 long closingPeriodID = -1;
                 if (Insert(balanceClosingPeriod, out closingPeriodID))
                 {
                     BillingTransactionDataManager billingTransactionDataManager = new BillingTransactionDataManager();
                     billingTransactionDataManager.InsertBillingTransactionFromLiveBalance(balanceClosingPeriod, usageTransactionTypeId, closingPeriodID);
                     BalanceHistoryDataManager balanceHistoryDataManager = new BalanceHistoryDataManager();
                     balanceHistoryDataManager.InsertBalanceHistoryFromLiveBalance(closingPeriodID);
                     LiveBalanceDataManager liveBalanceDataManager = new LiveBalanceDataManager();
                     liveBalanceDataManager.UpdateLiveBalanceBalance();
                     billingTransactionDataManager.UpdateBillingTransactionClosingPeriod(closingPeriodID);
                 };
                 scope.Complete();
             }
        }
        #endregion

        #region Private Methods
        private bool Insert(DateTime balanceClosingPeriod, out long insertedId)
        {
            object closingPeriodID;
            int recordesEffected = ExecuteNonQuerySP("[VR_AccountBalance].[sp_BalanceClosingPeriod_Insert]", out closingPeriodID, balanceClosingPeriod);
            insertedId = (recordesEffected > 0) ? (long)closingPeriodID : -1;
            return (recordesEffected > 0);
        }

        #endregion

        #region Mappers
        private BalanceClosingPeriod BalanceClosingPeriodMapper(IDataReader reader)
        {
            return new BalanceClosingPeriod
            {
                BalanceClosingPeriodId = (long)reader["ID"],
                ClosingTime = GetReaderValue<DateTime>(reader, "ClosingTime"),
            };
        }
        #endregion

    }
}
