using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountStatementManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AccountStatement> GetFilteredAccountStatments(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new AccountStatmentRequestHandler());
        }

        #region Private Classes
        private class AccountStatmentRequestHandler : BigDataRequestHandler<AccountStatementQuery, AccountStatement, AccountStatement>
        {
            public override AccountStatement EntityDetailMapper(AccountStatement entity)
            {
                return entity;
            }
            decimal _currenctBalance;
            public override IEnumerable<AccountStatement> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
            {

                BillingTransactionQuery billingTransactionQuery = new BillingTransactionQuery
                {
                    AccountsIds = new List<long> { input.Query.AccountId },
                    AccountTypeId = input.Query.AccountTypeId,
                    FromTime = input.Query.FromDate,
                    ToTime = input.Query.ToDate
                };
                BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
                IBillingTransactionDataManager billingTransactionDataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
                var billingTransactions = billingTransactionDataManager.GetFilteredBillingTransactions(billingTransactionQuery);
                List<AccountStatement> accountStatements = new List<AccountStatement>();


                billingTransactions.OrderBy(x => x.TransactionTime);
                _currenctBalance = 0;
                foreach(var billingTransaction in billingTransactions)
                {
                    var billingTransactionType = billingTransactionTypeManager.GetBillingTransactionType(billingTransaction.TransactionTypeId);
                   AccountStatement accountStatement= new AccountStatement
                   {
                       TransactionTime = billingTransaction.TransactionTime,
                        Description = billingTransaction.Notes,
                        TransactionType = billingTransactionType.Name,
                   };
                    if(billingTransactionType.IsCredit)
                    {
                        _currenctBalance += billingTransaction.Amount;
                        accountStatement.Balance =_currenctBalance;
                        accountStatement.Credit = billingTransaction.Amount;
                    }else
                    {
                         _currenctBalance -= billingTransaction.Amount;
                        accountStatement.Balance =_currenctBalance;
                        accountStatement.Debit = billingTransaction.Amount;
                    }
                    accountStatements.Add(accountStatement);
                }

                return accountStatements;
            }
            protected override BigResult<AccountStatement> AllRecordsToBigResult(DataRetrievalInput<AccountStatementQuery> input, IEnumerable<AccountStatement> allRecords)
            {
                var query = input.Query;

                var analyticBigResult = new AccountStatementDetail()
                {
                    ResultKey = input.ResultKey,
                    Data = allRecords.ToList(),
                    TotalCount = allRecords.Count(),
                    CurrentBalance = _currenctBalance,
                };
                return analyticBigResult;
            }

        }
        #endregion

    }
}
