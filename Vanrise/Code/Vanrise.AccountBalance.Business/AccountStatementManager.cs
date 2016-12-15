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

            public override IEnumerable<AccountStatement> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
            {

                BillingTransactionQuery billingTransactionQuery = new BillingTransactionQuery
                {
                    AccountsIds = new List<long> { input.Query.AccountId },
                    AccountTypeId = input.Query.AccountTypeId,
                    FromDate = input.Query.FromDate,
                    ToDate = input.Query.ToDate
                };
                IBillingTransactionDataManager billingTransactionDataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
                var billingTransactions = billingTransactionDataManager.GetFilteredBillingTransactions(billingTransactionQuery);
                List<AccountStatement> accountStatements = new List<AccountStatement>();

                foreach(var billingTransaction in billingTransactions)
                {

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
                    TotalCount = allRecords.Count()
                };
                return analyticBigResult;
            }

        }
        #endregion

    }
}
