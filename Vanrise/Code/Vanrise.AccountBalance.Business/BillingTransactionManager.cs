﻿using System;
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

        public Vanrise.Entities.IDataRetrievalResult<BillingTransactionDetail> GetFilteredBillingTransactions(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new BillingTransactionRequestHandler());
        }

        public Vanrise.Entities.InsertOperationOutput<BillingTransactionDetail> AddBillingTransaction(BillingTransaction billingTransaction)
        {         
            billingTransaction.TransactionTypeId = Guid.Parse("f178d94d-d622-4ebf-a1ba-2a4af1067d6b");
            billingTransaction.TransactionTime = DateTime.Now;
        
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BillingTransactionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            long billingTransactionId = -1;

            if (dataManager.Insert(billingTransaction, out billingTransactionId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                billingTransaction.AccountBillingTransactionId = billingTransactionId;
                insertOperationOutput.InsertedObject = BillingTransactionDetailMapper(billingTransaction);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
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
