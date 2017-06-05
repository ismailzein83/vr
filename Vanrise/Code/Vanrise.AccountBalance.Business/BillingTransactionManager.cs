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
            if (TryAddBillingTransaction(billingTransaction, out billingTransactionId))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                billingTransaction.AccountBillingTransactionId = billingTransactionId;
                insertOperationOutput.InsertedObject = BillingTransactionDetailMapper(billingTransaction, BillingTransactionSource.BillingTransaction);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public bool TryAddBillingTransaction(BillingTransaction billingTransaction, out long billingTransactionId)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.Insert(billingTransaction, out billingTransactionId);
        }
        private BillingTransactionDetail BillingTransactionDetailMapper(BillingTransaction billingTransaction, BillingTransactionSource billingTransactionSource)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
            AccountManager accountManager = new AccountManager();
            bool isCredit = billingTransactionTypeManager.IsCredit(billingTransaction.TransactionTypeId);
            return new BillingTransactionDetail
            {
                Entity = billingTransaction,
                CurrencyDescription = currencyManager.GetCurrencySymbol(billingTransaction.CurrencyId),
                TransactionTypeDescription = billingTransactionTypeManager.GetBillingTransactionTypeName(billingTransaction.TransactionTypeId),
                AccountInfo = accountManager.GetAccountInfo(billingTransaction.AccountTypeId, billingTransaction.AccountId),
                Credit = isCredit ? (double?)billingTransaction.Amount : null,
                Debit = !isCredit ? (double?)billingTransaction.Amount : null,
                BillingTransactionSource = billingTransactionSource,
                DisplayId = string.Format("{0}_{1}", Utilities.GetEnumDescription(billingTransactionSource), billingTransaction.AccountBillingTransactionId)
            };
        }
        public IEnumerable<BillingTransactionMetaData> GetBillingTransactionsByAccountIds(Guid accountTypeId, List<Guid> transactionTypeIds, List<string> accountIds)
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
                AccountBillingTransactionId = accountUsage.AccountUsageId ,
                Amount = accountUsage.UsageBalance,
                CurrencyId =accountUsage.CurrencyId ,
                TransactionTime = accountUsage.PeriodEnd <= DateTime.Now ? accountUsage.PeriodEnd : DateTime.Now,
                TransactionTypeId = accountUsage.TransactionTypeId,
                Notes = string.Format("Usage From {0:yyyy-MM-dd HH:mm} to {1:yyyy-MM-dd HH:mm}", accountUsage.PeriodStart, accountUsage.PeriodEnd),
                IsBalanceUpdated = true,
            };
        }
        public IEnumerable<BillingTransaction> ConvertAccountUsagesToBillingTransactions(IEnumerable<AccountUsage> accountUsages)
        {
            List<BillingTransaction> billingTransactions = new List<BillingTransaction>();
            if (accountUsages != null)
            {
                foreach (var accountUsage in accountUsages)
                {
                    billingTransactions.Add(ConvertAccountUsageToBillingTransaction(accountUsage));
                }
            }
            return billingTransactions;
        }

        public IEnumerable<BillingTransaction> GetBillingTransactionsByAccountId(Guid accountTypeId, String accountId)
        {
            IBillingTransactionDataManager dataManager = AccountBalanceDataManagerFactory.GetDataManager<IBillingTransactionDataManager>();
            return dataManager.GetBillingTransactionsByAccountId(accountTypeId, accountId);
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
                    rslt.AddRange(billingTransactions.Select(itm => new BillingTransactionResultItem { BillingTransaction = itm, Source = BillingTransactionSource.BillingTransaction }));
                AccountUsageManager accountUsageManager = new AccountUsageManager();
                var accountUsages = accountUsageManager.GetAccountUsageForBillingTransactions(input.Query.AccountTypeId, input.Query.TransactionTypeIds, input.Query.AccountsIds, input.Query.FromTime, input.Query.ToTime);
                if (accountUsages != null)
                {                   
                    var usagesAsBillingTransactions = s_billingTransactionManager.ConvertAccountUsagesToBillingTransactions(accountUsages);
                    rslt.AddRange(usagesAsBillingTransactions.Select(itm => new BillingTransactionResultItem { BillingTransaction = itm, Source = BillingTransactionSource.AccountUsage }));
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
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Transaction Time", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.LongDateTime, Width = 25});
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Account", Width = 30});
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
    }
}
