using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Data;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;

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

            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<BillingTransactionDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            long billingTransactionId = -1;
            string errorMessage;
            if (TryAddBillingTransaction(billingTransaction, out billingTransactionId,out  errorMessage))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                billingTransaction.AccountBillingTransactionId = billingTransactionId;
                insertOperationOutput.InsertedObject = BillingTransactionDetailMapper(billingTransaction, BillingTransactionSource.Transaction);
            }
            else
            {
                insertOperationOutput.Message = errorMessage;
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                insertOperationOutput.ShowExactMessage = true;
            }

            return insertOperationOutput;
        }
        public bool TryAddBillingTransaction(BillingTransaction billingTransaction, out long billingTransactionId, out string errorMessage)
        {
            errorMessage = null;
            if (!ValidateAddBillingTransaction(billingTransaction.AccountId,billingTransaction.AccountTypeId,out  errorMessage))
            {
                billingTransactionId = -1;
                return false;
            }
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.Insert(billingTransaction, out billingTransactionId);
        }

        private bool ValidateAddBillingTransaction(string accountId, Guid accountTypeId, out string errorMessage)
        {
            errorMessage = null;
            AccountManager accountManager = new AccountManager();
            var accountInfo = accountManager.GetAccountInfo(accountTypeId, accountId);
            accountInfo.ThrowIfNull("accountInfo", accountId);
            //if( accountInfo.Status != VRAccountStatus.Active)
            //{
            //    errorMessage = "Cannot add billing transaction for inactive account.";
            //    return false;
            //}
            return true;
        }
        private BillingTransactionDetail BillingTransactionDetailMapper(BillingTransaction billingTransaction, BillingTransactionSource billingTransactionSource)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
            AccountManager accountManager = new AccountManager();
            bool isCredit = billingTransactionTypeManager.IsCredit(billingTransaction.TransactionTypeId);

            string displayId = (billingTransactionSource == BillingTransactionSource.GroupedUsage) ? string.Format("BillingTransaction_{0}", billingTransactionTypeManager.GetBillingTransactionTypeName(billingTransaction.TransactionTypeId)) : string.Format("{0}_{1}", Utilities.GetEnumDescription(billingTransactionSource), billingTransaction.AccountBillingTransactionId);

            return new BillingTransactionDetail
            {
                Entity = billingTransaction,
                CurrencyDescription = currencyManager.GetCurrencySymbol(billingTransaction.CurrencyId),
                TransactionTypeDescription = billingTransactionTypeManager.GetBillingTransactionTypeName(billingTransaction.TransactionTypeId),
                AccountInfo = accountManager.GetAccountInfo(billingTransaction.AccountTypeId, billingTransaction.AccountId),
                Credit = isCredit ? (double?)billingTransaction.Amount : null,
                Debit = !isCredit ? (double?)billingTransaction.Amount : null,
                BillingTransactionSource = billingTransactionSource,
                DisplayId = displayId
            };
        }
        public List<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetBillingTransactionsByAccountIds(accountTypeId, transactionTypeIds, accountIds);
        }
        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByTransactionTypes(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime, List<Guid> transactionTypeIds)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetBillingTransactionsByTransactionTypes(accountTypeId, billingTransactionsByTime, transactionTypeIds);
        }
        public BillingTransaction ConvertAccountUsageToBillingTransaction(AccountUsage accountUsage)
        {
            return new BillingTransaction
            {
                AccountTypeId = accountUsage.AccountTypeId,
                AccountId = accountUsage.AccountId,
                AccountBillingTransactionId = accountUsage.AccountUsageId,
                Amount = accountUsage.UsageBalance,
                CurrencyId = accountUsage.CurrencyId,
                TransactionTime = accountUsage.PeriodEnd <= DateTime.Now ? accountUsage.PeriodEnd : DateTime.Now,
                TransactionTypeId = accountUsage.TransactionTypeId,
                Notes = string.Format("Usage From {0:yyyy-MM-dd HH:mm} to {1:yyyy-MM-dd HH:mm}", accountUsage.PeriodStart, accountUsage.PeriodEnd),
                IsBalanceUpdated = true,
            };
        }
        public IEnumerable<BillingTransaction> ConvertAccountUsagesToBillingTransactions(IEnumerable<AccountUsage> accountUsages, bool shouldGroupUsagesByTransactionTypes)
        {
            if (accountUsages == null || accountUsages.Count() == 0)
                return null;

            if (shouldGroupUsagesByTransactionTypes)
            {
                var transactions = new Dictionary<string, Dictionary<Guid, BillingTransaction>>();

                var transactionTypeManager = new BillingTransactionTypeManager();
                var rateExchangeManager = new CurrencyExchangeRateManager();

                foreach (AccountUsage accountUsage in accountUsages)
                {
                    Dictionary<Guid, BillingTransaction> accountTransactions = transactions.GetOrCreateItem(accountUsage.AccountId, () =>
                    {
                        return new Dictionary<Guid, BillingTransaction>();
                    });

                    BillingTransaction matchedTransaction = accountTransactions.GetOrCreateItem(accountUsage.TransactionTypeId, () =>
                    {
                        BillingTransactionType transactionType = transactionTypeManager.GetBillingTransactionType(accountUsage.TransactionTypeId);
                        transactionType.ThrowIfNull("transactionType", accountUsage.TransactionTypeId);

                        return new BillingTransaction()
                        {
                            AccountBillingTransactionId = 0,
                            TransactionTypeId = accountUsage.TransactionTypeId,
                            AccountTypeId = accountUsage.AccountTypeId,
                            AccountId = accountUsage.AccountId,
                            Amount = 0,
                            CurrencyId = accountUsage.CurrencyId,
                            TransactionTime = DateTime.Now,
                            Notes = string.Format("Live {0}", transactionType.Name),
                            IsBalanceUpdated = true
                        };
                    });

                    decimal convertedAmount = rateExchangeManager.ConvertValueToCurrency(accountUsage.UsageBalance, accountUsage.CurrencyId, matchedTransaction.CurrencyId, accountUsage.PeriodEnd);
                    matchedTransaction.Amount += convertedAmount;
                }

                return transactions.SelectMany(x => x.Value.Values);
            }
            else
            {
                var transactions = new List<BillingTransaction>();
                foreach (AccountUsage accountUsage in accountUsages)
                    transactions.Add(ConvertAccountUsageToBillingTransaction(accountUsage));
                return transactions;
            }
        }
        public IEnumerable<BillingTransaction> GetBillingTransactionsByAccountId(Guid accountTypeId, String accountId,VRAccountStatus? status, DateTime? effectiveDate, bool? isEffectiveInFuture)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetBillingTransactionsByAccountId(accountTypeId, accountId, status, effectiveDate, isEffectiveInFuture);
        }
        public BillingTransactionMetaData ConvertAccountUsageToBillingTransactionMetaDeta(AccountUsage accountUsage)
        {
            return new BillingTransactionMetaData
            {
                AccountId = accountUsage.AccountId,
                Amount = accountUsage.UsageBalance,
                CurrencyId = accountUsage.CurrencyId,
                TransactionTime = accountUsage.PeriodEnd <= DateTime.Now ? accountUsage.PeriodEnd : DateTime.Now,
                TransactionTypeId = accountUsage.TransactionTypeId,
            };
        }
        public IEnumerable<BillingTransactionMetaData> ConvertAccountUsagesToBillingTransactionsMetaDeta(IEnumerable<AccountUsage> accountUsages, bool shouldGroupUsagesByTransactionTypes)
        {
            if (accountUsages == null || accountUsages.Count() == 0)
                return null;

            if (shouldGroupUsagesByTransactionTypes)
            {
                var transactions = new Dictionary<string, Dictionary<Guid, BillingTransactionMetaData>>();

                var transactionTypeManager = new BillingTransactionTypeManager();
                var rateExchangeManager = new CurrencyExchangeRateManager();

                foreach (AccountUsage accountUsage in accountUsages)
                {
                    Dictionary<Guid, BillingTransactionMetaData> accountTransactions = transactions.GetOrCreateItem(accountUsage.AccountId, () =>
                    {
                        return new Dictionary<Guid, BillingTransactionMetaData>();
                    });

                    BillingTransactionMetaData matchedTransaction = accountTransactions.GetOrCreateItem(accountUsage.TransactionTypeId, () =>
                    {
                        return new BillingTransactionMetaData()
                        {
                            TransactionTypeId = accountUsage.TransactionTypeId,
                            AccountId = accountUsage.AccountId,
                            Amount = 0,
                            CurrencyId = accountUsage.CurrencyId,
                            TransactionTime = DateTime.Now,
                        };
                    });

                    decimal convertedAmount = rateExchangeManager.ConvertValueToCurrency(accountUsage.UsageBalance, accountUsage.CurrencyId, matchedTransaction.CurrencyId, accountUsage.PeriodEnd);
                    matchedTransaction.Amount += convertedAmount;
                }
                return transactions.SelectMany(x => x.Value.Values);
            }
            else
            {
                var transactions = new List<BillingTransactionMetaData>();
                foreach (AccountUsage accountUsage in accountUsages)
                    transactions.Add(ConvertAccountUsageToBillingTransactionMetaDeta(accountUsage));
                return transactions;
            }
        }

        public IEnumerable<BillingTransaction> GetBillingTransactions(List<Guid> accountTypes, List<string> accountIds, List<Guid> transactionTypeIds, DateTime fromDate, DateTime? toDate)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetBillingTransactions(accountTypes, accountIds, transactionTypeIds, fromDate, toDate);
        }

        public DateTime? GetLastBillingTransactionDate(Guid accountTypeId, string accountId)
        {
            var lastBillingTransaction = GetLastBillingTransaction(accountTypeId, accountId);
            return lastBillingTransaction != null ? lastBillingTransaction.TransactionTime : default(DateTime?);
        }

        public DateTime? GetLastTransactionDate(Guid accountTypeId, string accountId)
        {

            var lastBillingTransactionDate =GetLastBillingTransactionDate(accountTypeId,  accountId);
            var lastAccountUsageDate = new AccountUsageManager().GetLastAccountUsageDate(accountTypeId, accountId);
            if (lastBillingTransactionDate.HasValue && lastAccountUsageDate.HasValue)
            {
                return lastBillingTransactionDate.Value > lastAccountUsageDate.Value ? lastBillingTransactionDate.Value : lastAccountUsageDate.Value;
            }
            else if (lastBillingTransactionDate.HasValue)
                return lastBillingTransactionDate.Value;
            else if (lastAccountUsageDate.HasValue)
                return lastAccountUsageDate.Value;
            else
                return null;
        }
        public BillingTransaction GetLastBillingTransaction(Guid accountTypeId, string accountId)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetLastBillingTransaction(accountTypeId, accountId);
        }
        
        #region Private Classes

        private class BillingTransactionRequestHandler : BigDataRequestHandler<BillingTransactionQuery, BillingTransactionResultItem, BillingTransactionDetail>
        {
            static BillingTransactionManager s_billingTransactionManager = new BillingTransactionManager();
            public override BillingTransactionDetail EntityDetailMapper(BillingTransactionResultItem entity)
            {
                return s_billingTransactionManager.BillingTransactionDetailMapper(entity.BillingTransaction, entity.Source);
            }

            public override IEnumerable<BillingTransactionResultItem> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<BillingTransactionQuery> input)
            {
                List<BillingTransactionResultItem> rslt = new List<BillingTransactionResultItem>();
                IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
                var billingTransactions = dataManager.GetFilteredBillingTransactions(input.Query);
                if (billingTransactions != null)
                    rslt.AddRange(billingTransactions.Select(itm => new BillingTransactionResultItem { BillingTransaction = itm, Source = BillingTransactionSource.Transaction }));
                AccountUsageManager accountUsageManager = new AccountUsageManager();
                var accountUsages = accountUsageManager.GetAccountUsageForBillingTransactions(input.Query.AccountTypeId, input.Query.TransactionTypeIds, input.Query.AccountsIds, input.Query.FromTime, input.Query.ToTime, input.Query.Status, input.Query.EffectiveDate, input.Query.IsEffectiveInFuture);
                if (accountUsages != null && accountUsages.Count() > 0)
                {
                    bool shouldGroupUsagesByTransactionType = new AccountTypeManager().ShouldGroupUsagesByTransactionType(input.Query.AccountTypeId);
                    var usagesAsBillingTransactions = s_billingTransactionManager.ConvertAccountUsagesToBillingTransactions(accountUsages, shouldGroupUsagesByTransactionType);
                    BillingTransactionSource transactionSource = (shouldGroupUsagesByTransactionType) ? BillingTransactionSource.GroupedUsage : BillingTransactionSource.Usage;
                    rslt.AddRange(usagesAsBillingTransactions.Select(itm => new BillingTransactionResultItem { BillingTransaction = itm, Source = transactionSource }));
                }
                return rslt;
            }

            protected override ResultProcessingHandler<BillingTransactionDetail> GetResultProcessingHandler(DataRetrievalInput<BillingTransactionQuery> input, BigResult<BillingTransactionDetail> bigResult)
            {
                return new ResultProcessingHandler<BillingTransactionDetail>
                {
                    ExportExcelHandler = new BillingTransactionExcelExportHandler(input.Query)
                };
            }
        }

        private class BillingTransactionResultItem
        {
            public BillingTransaction BillingTransaction { get; set; }

            public BillingTransactionSource Source { get; set; }
        }
        private class BillingTransactionExcelExportHandler : ExcelExportHandler<BillingTransactionDetail>
        {
            BillingTransactionQuery _query;
            public BillingTransactionExcelExportHandler(BillingTransactionQuery query)
            {
                if (query == null)
                    throw new ArgumentNullException("query");
                _query = query;
            }
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<BillingTransactionDetail> context)
            {
                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Financial Transactions",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Transaction Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime, Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Account", Width = 30 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Transaction Type", Width = 22 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Debit" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Credit" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Notes" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Reference" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Balance Updated" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                            sheet.Rows.Add(row);
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.AccountBillingTransactionId });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.TransactionTime });
                            row.Cells.Add(new ExportExcelCell { Value = record.AccountInfo == null ? "" : record.AccountInfo.Name });
                            row.Cells.Add(new ExportExcelCell { Value = record.TransactionTypeDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.Debit });
                            row.Cells.Add(new ExportExcelCell { Value = record.Credit });
                            row.Cells.Add(new ExportExcelCell { Value = record.CurrencyDescription });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Notes });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.Reference });
                            row.Cells.Add(new ExportExcelCell { Value = record.Entity.IsBalanceUpdated });
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }

        #endregion

        public Dictionary<string, BillingTransaction> GetBillingTransactionsForSynchronizerProcess(List<Guid> billingTransactionTypeIds, Guid balanceAccountTypeId)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetBillingTransactionsForSynchronizerProcess(billingTransactionTypeIds, balanceAccountTypeId).ToDictionary(kvp => kvp.SourceId, kvp => kvp);
        }

        public bool HasBillingTransactionData(Guid accountTypeId) 
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.HasBillingTransactionData(accountTypeId);
        }
    }
}
