using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class AccountStatementManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AccountStatementItem> GetFilteredAccountStatments(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new AccountStatmentRequestHandler());
        }

        #region Private Classes

        private class AccountStatmentRequestHandler : BigDataRequestHandler<AccountStatementQuery, AccountStatementItem, AccountStatementItem>
        {
            #region Fields

            string _currencyName;
            decimal _currentBalance;

            decimal _totalDebit = 0;
            decimal _totalCredit = 0;

            #endregion

            public override IEnumerable<AccountStatementItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<AccountStatementQuery> input)
            {
                AccountInfo accountInfo = new AccountManager().GetAccountInfo(input.Query.AccountTypeId, input.Query.AccountId);

                int accountCurrencyId = accountInfo.CurrencyId;
                _currencyName = new CurrencyManager().GetCurrencySymbol(accountCurrencyId);

                bool shouldGroupUsagesByTransactionType = new AccountTypeManager().ShouldGroupUsagesByTransactionType(input.Query.AccountTypeId);

                return BuildAccountStatementItems(input.Query.AccountTypeId, input.Query.AccountId, input.Query.FromDate, accountCurrencyId, shouldGroupUsagesByTransactionType,input.Query.Status, input.Query.EffectiveDate, input.Query.IsEffectiveInFuture);
            }

            protected override BigResult<AccountStatementItem> AllRecordsToBigResult(DataRetrievalInput<AccountStatementQuery> input, IEnumerable<AccountStatementItem> allRecords)
            {
                var query = input.Query;
                int normalPression = GenericParameterManager.Current.GetNormalPrecision();
                var analyticBigResult = new AccountStatementResult()
                {
                    ResultKey = input.ResultKey,
                    Data = allRecords.VRGetPage(input),
                    TotalCount = allRecords.Count(),
                    CurrentBalance = Math.Round(Math.Abs(_currentBalance), normalPression),
                    BalanceFlagDescription = LiveBalanceManager.GetBalanceFlagDescription(_currentBalance),
                    Currency = _currencyName,
                    TotalCredit = Math.Round(_totalCredit, normalPression),
                    TotalDebit = Math.Round(_totalDebit, normalPression),

                };
                return analyticBigResult;
            }

            public override AccountStatementItem EntityDetailMapper(AccountStatementItem entity)
            {
                return entity;
            }

            protected override ResultProcessingHandler<AccountStatementItem> GetResultProcessingHandler(DataRetrievalInput<AccountStatementQuery> input, BigResult<AccountStatementItem> bigResult)
            {
                return new ResultProcessingHandler<AccountStatementItem>
                {
                    ExportExcelHandler = new AccountStatementExcelExportHandler(input.Query)
                };
            }

            #region Private Methods

            private List<AccountStatementItem> BuildAccountStatementItems(Guid accountTypeId, String accountId, DateTime fromDate, int accountCurrencyId, bool shouldGroupUsagesByTransactionType,VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
            {
                var transactionManager = new BillingTransactionManager();
                IEnumerable<BillingTransaction> transactions = transactionManager.GetBillingTransactionsByAccountId(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);

                IEnumerable<AccountUsage> accountUsages = new AccountUsageManager().GetAccountUsagesByAccount(accountTypeId, accountId, status,effectiveDate,isEffectiveInFuture);
                IEnumerable<BillingTransaction> convertedTransactions = transactionManager.ConvertAccountUsagesToBillingTransactions(accountUsages, shouldGroupUsagesByTransactionType);

                var allTransactions = new List<BillingTransaction>();
                if (transactions != null)
                    allTransactions.AddRange(transactions);
                if (convertedTransactions != null)
                    allTransactions.AddRange(convertedTransactions);

                var accountStatementItems = new List<AccountStatementItem>();
                IEnumerable<BillingTransaction> orderedTransactions = allTransactions.OrderBy(x => x.TransactionTime);

                var currencyExchangeRateManager = new CurrencyExchangeRateManager();
                var transactionTypeManager = new BillingTransactionTypeManager();

                decimal previousBalance = 0;
                decimal balance = 0;

                foreach (BillingTransaction transaction in orderedTransactions)
                {
                    BillingTransactionType transactionType = transactionTypeManager.GetBillingTransactionType(transaction.TransactionTypeId);
                    decimal convertedAmount = currencyExchangeRateManager.ConvertValueToCurrency(transaction.Amount, transaction.CurrencyId, accountCurrencyId, transaction.TransactionTime);

                    if (transactionType.IsCredit)
                    {
                        balance += convertedAmount;
                        _currentBalance += convertedAmount;
                    }
                    else
                    {
                        balance -= convertedAmount;
                        _currentBalance -= convertedAmount;
                    }

                    if (transaction.TransactionTime < fromDate)
                    {
                        if (transactionType.IsCredit)
                            previousBalance += convertedAmount;
                        else
                            previousBalance -= convertedAmount;
                    }
                    else
                    {
                        var accountStatementItem = new AccountStatementItem
                        {
                            TransactionType = transactionType.Name,
                            TransactionTime = transaction.TransactionTime,
                            Description = transaction.Notes
                        };

                        if (transactionType.IsCredit)
                        {
                            accountStatementItem.Credit = convertedAmount;
                            _totalCredit += convertedAmount;
                        }
                        else
                        {
                            accountStatementItem.Debit = convertedAmount;
                            _totalDebit += convertedAmount;
                        }

                        accountStatementItem.Balance = Math.Abs(balance);
                        accountStatementItem.BalanceFlagDescription = LiveBalanceManager.GetBalanceFlagDescription(balance);
                        accountStatementItems.Add(accountStatementItem);
                    }
                }

                accountStatementItems.Insert(0, new AccountStatementItem
                {
                    Balance = Math.Abs(previousBalance),
                    Description = "Balance brought forward",
                    BalanceFlagDescription = LiveBalanceManager.GetBalanceFlagDescription(previousBalance)
                });

                return accountStatementItems;
            }

            #endregion
        }

        private class AccountStatementExcelExportHandler : ExcelExportHandler<AccountStatementItem>
        {
            AccountStatementQuery _query;
            public AccountStatementExcelExportHandler(AccountStatementQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<AccountStatementItem> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Account Statement",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Transaction Time" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Transaction Type" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Description" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Debit" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Credit" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Balance" });

                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    var results = context.BigResult as AccountStatementResult;

                    sheet.SummaryRows = new List<ExportExcelRow>();
                    AccountManager accountManager = new AccountManager();
                    var account = accountManager.GetAccountInfo(_query.AccountTypeId, _query.AccountId);

                    var accountRow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.SummaryRows.Add(accountRow);
                    accountRow.Cells.Add(new ExportExcelCell { Value = "Account Name" });
                    accountRow.Cells.Add(new ExportExcelCell { Value = account.Name });

                    var periodRow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.SummaryRows.Add(periodRow);
                    periodRow.Cells.Add(new ExportExcelCell { Value = "From Date" });
                    periodRow.Cells.Add(new ExportExcelCell { Value = _query.FromDate.ToString() });
                    periodRow.Cells.Add(new ExportExcelCell { Value = "To Date" });
                    periodRow.Cells.Add(new ExportExcelCell { Value = DateTime.Now.ToString() });

                    var balanceRow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.SummaryRows.Add(balanceRow);
                    balanceRow.Cells.Add(new ExportExcelCell { Value = "Current Balance" });
                    balanceRow.Cells.Add(new ExportExcelCell { Value = results.CurrentBalance });
                    balanceRow.Cells.Add(new ExportExcelCell { Value = "Total Debit" });
                    balanceRow.Cells.Add(new ExportExcelCell { Value = results.TotalDebit });
                    balanceRow.Cells.Add(new ExportExcelCell { Value = "Total Credit" });
                    balanceRow.Cells.Add(new ExportExcelCell { Value = results.TotalCredit });

                    var currencyRow = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                    sheet.SummaryRows.Add(currencyRow);
                    currencyRow.Cells.Add(new ExportExcelCell { Value = "Currency" });
                    currencyRow.Cells.Add(new ExportExcelCell { Value = results.Currency });

                    sheet.Rows = new List<ExportExcelRow>();
                    foreach (var record in results.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        row.Cells.Add(new ExportExcelCell { Value = record.TransactionTime.ToString() });
                        row.Cells.Add(new ExportExcelCell { Value = record.TransactionType });
                        row.Cells.Add(new ExportExcelCell { Value = record.Description });
                        row.Cells.Add(new ExportExcelCell { Value = record.Debit });
                        row.Cells.Add(new ExportExcelCell { Value = record.Credit });
                        row.Cells.Add(new ExportExcelCell { Value = record.Balance });

                    }
                }

                context.MainSheet = sheet;
            }
        }

        #endregion
    }
}
