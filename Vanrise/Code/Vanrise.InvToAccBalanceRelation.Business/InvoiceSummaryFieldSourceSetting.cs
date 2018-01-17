using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.Common;
using Vanrise.AccountBalance.Business;
using Vanrise.Common.Business;
namespace Vanrise.InvToAccBalanceRelation.Business
{
    public class InvoiceSummaryFieldSourceSetting: AccountBalanceFieldSourceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("1FD21C4E-0A0E-4C0D-B567-D65CE039910F"); }
        }
        public bool CalculateDueAmount { get; set; }
        public List<Guid> TransactionTypeIds { get; set; }
        CurrencyExchangeRateManager currencyExchangeRateManager;
        BillingTransactionTypeManager billingTransactionTypeManager;
        AccountManager accountManager;

        public override List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context)
        {
            List<AccountBalanceFieldDefinition> invoiceFieldDefinitions = new List<AccountBalanceFieldDefinition> {
                new AccountBalanceFieldDefinition{
                    Name = "DuePeriod",
                    Title = "Due Period",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Int },

                }, 
                new AccountBalanceFieldDefinition { 
                    Name = "DueAmount",
                    Title = "Due Amount",
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
                }, 
                new AccountBalanceFieldDefinition { 
                    Name = "DueDate",
                    Title = "Due Date",
                    FieldType = new FieldDateTimeType { DataType = FieldDateTimeDataType.Date },
                }
            };
            return invoiceFieldDefinitions;
        }
        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
            List<AccountBalanceInvoiceInfo> accountBalanceInvoicesInfo;

            IEnumerable<InvoiceByPartnerInfo> invoices = GetLastInvoicesByPartners(context.AccountBalances, context.AccountTypeId, context.AccountTypeSettings, out accountBalanceInvoicesInfo);

            Dictionary<string, InvoiceFieldsData> invoiceFieldsByAccountId = new Dictionary<string, InvoiceFieldsData>();
            if (invoices != null && invoices.Count() > 0)
            {
                List<BillingTransactionByTime> billingTransactionsByTime = new List<BillingTransactionByTime>();
                List<AccountUsageByTime> accountUsagesByTime = new List<AccountUsageByTime>();
                foreach (var accountBalanceInvoiceInfo in accountBalanceInvoicesInfo)
                {
                    List<InvoiceByPartnerInfo> accountInvoices = new List<InvoiceByPartnerInfo>();
                    foreach (var item in accountBalanceInvoiceInfo.InvoiceByPartnerInvoiceTypes)
                    {
                        InvoiceByPartnerInfo invoicePerType = invoices.FindRecord(x => x.PartnerId == item.PartnerId && x.InvoiceTypeId == item.InvoiceTypeId);
                        if (invoicePerType != null)
                        {
                            accountInvoices.Add(invoicePerType);
                        }
                    }
                    if (accountInvoices.Count > 0)
                    {
                        var invoice = accountInvoices.OrderBy(x => x.ToDate).Last();
                        if (this.CalculateDueAmount)
                        {
                            var datePeriod = invoice.ToDate.AddDays(1).Date;
                            billingTransactionsByTime.Add(new BillingTransactionByTime
                            {
                                AccountId = accountBalanceInvoiceInfo.AccountBalance.AccountId,
                                TransactionTime = datePeriod
                            });
                            accountUsagesByTime.Add(new AccountUsageByTime
                            {
                                AccountId = accountBalanceInvoiceInfo.AccountBalance.AccountId,
                                EndPeriod = datePeriod
                            });
                        }
                        invoiceFieldsByAccountId.Add(accountBalanceInvoiceInfo.AccountBalance.AccountId, new InvoiceFieldsData
                        {
                            DueDate = invoice.DueDate,
                            ToDate = invoice.ToDate,
                            DuePeriod = Convert.ToInt32((invoice.DueDate - invoice.IssueDate).TotalDays),
                            DueAmount = accountBalanceInvoiceInfo.AccountBalance.CurrentBalance
                        });
                    }
                }
                if(this.CalculateDueAmount)
                {
                    currencyExchangeRateManager = new CurrencyExchangeRateManager();
                    billingTransactionTypeManager = new BillingTransactionTypeManager();
                    accountManager = new AccountManager();
                    var billingTransactions = GetBillingTransactionPaidData(context.AccountTypeId, billingTransactionsByTime);
                    Dictionary<string, decimal> amountsByAccount = new Dictionary<string, decimal>();
                    if (billingTransactions != null)
                    {
                        foreach (var item in billingTransactions)
                        {
                            UpdatBillingTransactionValues(context.AccountTypeId, amountsByAccount, item.TransactionTypeId, item.AccountId, item.TransactionTime, item.Amount, item.CurrencyId);
                        }
                    }
                    var accountUsages = GetAccountUsagesByTransactionTypes(context.AccountTypeId, accountUsagesByTime);
                    if (accountUsages != null)
                    {
                        foreach (var item in accountUsages)
                        {
                            UpdatBillingTransactionValues(context.AccountTypeId, amountsByAccount, item.TransactionTypeId, item.AccountId, item.PeriodEnd, item.UsageBalance, item.CurrencyId);
                        }
                    }
                    if(amountsByAccount != null)
                    {
                        foreach (var amountByAccount in amountsByAccount)
                        {
                            InvoiceFieldsData invoiceFieldsData;
                            if (invoiceFieldsByAccountId.TryGetValue(amountByAccount.Key, out invoiceFieldsData))
                            {
                                invoiceFieldsData.DueAmount = invoiceFieldsData.DueAmount - amountByAccount.Value;
                            }
                        }
                    }
                }
               
            }
            return invoiceFieldsByAccountId;
        }
        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            Dictionary<string, InvoiceFieldsData> invoiceFieldsByAccountId = context.PreparedData as Dictionary<string, InvoiceFieldsData>;
            var invoiceFieldsData = invoiceFieldsByAccountId.GetRecord(context.AccountBalance.AccountId);
            if (invoiceFieldsData != null)
            {
                switch (context.FieldName)
                {
                    case "DuePeriod": return invoiceFieldsData.DuePeriod;
                    case "DueDate": return invoiceFieldsData.DueDate;
                    case "DueAmount": return invoiceFieldsData.DueAmount;
                    default: return null;
                }
            }
            return null;
        }

        private void UpdatBillingTransactionValues(Guid accountTypeId, Dictionary<string, decimal> existingRecords, Guid transactionTypeId, string accountId, DateTime transactionTime, decimal transactionAmount, int transactionCurrencyId)
        {
            var transactionType = billingTransactionTypeManager.GetBillingTransactionType(transactionTypeId);
            var accountInfo = accountManager.GetAccountInfo(accountTypeId, accountId);
            decimal amount = transactionAmount;
            if (transactionCurrencyId != accountInfo.CurrencyId)
                amount = ConvertValueToCurrency(transactionAmount, transactionCurrencyId, accountInfo.CurrencyId, transactionTime);

            decimal finalAmountValue;
            if (existingRecords.TryGetValue(accountId, out finalAmountValue))
            {
                finalAmountValue += transactionType != null && transactionType.IsCredit ? amount : -amount;
                existingRecords[accountId] = finalAmountValue;
            }
            else
            {
                finalAmountValue = transactionType != null && transactionType.IsCredit ? amount : -amount;
                existingRecords.Add(accountId, finalAmountValue);
            }
      
        }
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }
        private IEnumerable<InvoiceByPartnerInfo> GetLastInvoicesByPartners(IEnumerable<AccountBalance.Entities.AccountBalance> accountBalances, Guid accountTypeId, AccountTypeSettings accountTypeSettings, out  List<AccountBalanceInvoiceInfo> accountBalanceInvoicesInfo)
        {
            InvoiceManager invoiceManager = new InvoiceManager();
            InvToAccBalanceRelationDefinitionManager invToAccBalanceRelationDefinitionManager = new InvToAccBalanceRelationDefinitionManager();
            List<PartnerInvoiceType> partnerInvoiceTypes = new List<PartnerInvoiceType>();
            accountBalanceInvoicesInfo = new List<AccountBalanceInvoiceInfo>();
            foreach (var accountBalance in accountBalances)
            {
                if (accountTypeSettings.InvToAccBalanceRelationId.HasValue)
                {
                    var balanceInvoiceAccounts = invToAccBalanceRelationDefinitionManager.GetBalanceInvoiceAccounts(accountTypeId, accountBalance.AccountId, DateTime.Now);
                    if (balanceInvoiceAccounts != null && balanceInvoiceAccounts.Count > 0)
                    {
                        AccountBalanceInvoiceInfo accountBalanceInvoiceInfo = new AccountBalanceInvoiceInfo
                        {
                            AccountBalance = accountBalance,
                            InvoiceByPartnerInvoiceTypes = new List<InvoiceByPartnerInvoiceType>()
                        };
                        foreach (var balanceInvoiceAccount in balanceInvoiceAccounts)
                        {
                            accountBalanceInvoiceInfo.InvoiceByPartnerInvoiceTypes.Add(new InvoiceByPartnerInvoiceType
                            {
                                InvoiceTypeId = balanceInvoiceAccount.InvoiceTypeId,
                                PartnerId = balanceInvoiceAccount.PartnerId
                            });
                            partnerInvoiceTypes.Add(new PartnerInvoiceType { InvoiceTypeId = balanceInvoiceAccount.InvoiceTypeId, PartnerId = balanceInvoiceAccount.PartnerId });
                        }
                        accountBalanceInvoicesInfo.Add(accountBalanceInvoiceInfo);
                    }
                }
            }
            return invoiceManager.GetLastInvoicesByPartners(partnerInvoiceTypes);

        }
        private IEnumerable<BillingTransactionMetaData> GetBillingTransactionPaidData(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime)
        {
            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            return billingTransactionManager.GetBillingTransactionsByTransactionTypes(accountTypeId, billingTransactionsByTime, this.TransactionTypeIds);
        }
        private IEnumerable<AccountUsage> GetAccountUsagesByTransactionTypes(Guid accountTypeId, List<AccountUsageByTime> accountUsagesByTime)
        {
            AccountUsageManager accountUsageManager = new AccountUsageManager();
            return accountUsageManager.GetAccountUsagesByTransactionTypes(accountTypeId, accountUsagesByTime, this.TransactionTypeIds);
        }
        private class InvoiceFieldsData
        {
            public int DuePeriod { get; set; }
            public DateTime DueDate { get; set; }
            public decimal DueAmount { get; set; }
            public DateTime ToDate { get; set; }
        }
        private class AccountBalanceInvoiceInfo
        {

            public AccountBalance.Entities.AccountBalance AccountBalance { get; set; }
            public List<InvoiceByPartnerInvoiceType> InvoiceByPartnerInvoiceTypes { get; set; }
        }
    }

}
