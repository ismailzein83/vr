using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;

namespace Vanrise.AccountBalance.Business
{
    public class BillingTransactionManager
    {
        public void AddBillingTransaction(BillingTransaction billingTransaction)
        {
            
        }

        public Vanrise.Entities.IDataRetrievalResult<BillingTransactionDetail> GetFilteredBillingTransactions(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BillingTransactionRequestHandler());
        }



        private BillingTransactionDetail BillingTransactionDetailMapper(BillingTransaction billingTransaction)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
            return new BillingTransactionDetail
            {
                Entity = billingTransaction,
                CurrencyDescription = currencyManager.GetCurrencyName(billingTransaction.CurrencyId),
                TransactionTypeDescription = billingTransactionTypeManager.GetBillingTransactionTypeName(billingTransaction.TransactionTypeId)
            };
        }


        #region Private Classes
        private class BillingTransactionRequestHandler : BigDataRequestHandler<BillingTransactionQuery, BillingTransaction, BillingTransactionDetail>
        {
            public override BillingTransactionDetail EntityDetailMapper(BillingTransaction entity)
            {
                BillingTransactionManager manager = new BillingTransactionManager();
                return manager.BillingTransactionDetailMapper(entity);
            }

            public override IEnumerable<BillingTransaction> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
            {
                IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
                return dataManager.GetFilteredBillingTransactions(input.Query);
            }
        }

        #endregion
    }
} 
