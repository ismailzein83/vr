﻿using System;
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

            Dictionary<InvoiceByPartnerInvoiceType, List<Invoice.Entities.Invoice>> invoices = GetUnPaidInvoices(context.AccountBalances, context.AccountTypeId, context.AccountTypeSettings, out accountBalanceInvoicesInfo);

            Dictionary<string, InvoiceFieldsData> invoiceFieldsByAccountId = new Dictionary<string, InvoiceFieldsData>();
            if (invoices != null && invoices.Count() > 0)
            {
                List<BillingTransactionByTime> billingTransactionsByTime = new List<BillingTransactionByTime>();
                foreach (var accountBalanceInvoiceInfo in accountBalanceInvoicesInfo)
                {
                    List<Invoice.Entities.Invoice> accountInvoices = new List<Invoice.Entities.Invoice>();
                    foreach (var item in accountBalanceInvoiceInfo.InvoiceByPartnerInvoiceTypes)
                    {
                        List<Invoice.Entities.Invoice> invoicesPerType;
                        if (invoices.TryGetValue(item, out invoicesPerType))
                        {
                            accountInvoices.AddRange(invoicesPerType);
                        }
                    }
                    if (accountInvoices.Count > 0)
                    {
                        var invoice = accountInvoices.OrderBy(x => x.ToDate).Last();
                        if (this.CalculateDueAmount)
                        {
                            billingTransactionsByTime.Add(new BillingTransactionByTime
                            {
                                AccountId = accountBalanceInvoiceInfo.AccountBalance.AccountId,
                                TransactionTime = invoice.DueDate
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
                            UpdatBillingTransactionValues(context.AccountTypeId, amountsByAccount, item);
                        }

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

        private void UpdatBillingTransactionValues(Guid accountTypeId, Dictionary<string, decimal> existingRecords, BillingTransactionMetaData billingTransaction)
        {
            var transactionType = billingTransactionTypeManager.GetBillingTransactionType(billingTransaction.TransactionTypeId);
            var accountInfo = accountManager.GetAccountInfo(accountTypeId, billingTransaction.AccountId);
            decimal amount = billingTransaction.Amount;
            if (billingTransaction.CurrencyId != accountInfo.CurrencyId)
                amount = ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, accountInfo.CurrencyId, billingTransaction.TransactionTime);

            decimal finalAmountValue;
            if(existingRecords.TryGetValue(billingTransaction.AccountId, out finalAmountValue))
            {
                finalAmountValue += transactionType != null && transactionType.IsCredit ? amount : -amount;
                existingRecords[billingTransaction.AccountId] = finalAmountValue;
            }
            else
            {
                finalAmountValue = transactionType != null && transactionType.IsCredit ? amount : -amount;
                existingRecords.Add(billingTransaction.AccountId, finalAmountValue);
            }
      
        }
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }
        private Dictionary<InvoiceByPartnerInvoiceType, List<Invoice.Entities.Invoice>> GetUnPaidInvoices(IEnumerable<AccountBalance.Entities.AccountBalance> accountBalances, Guid accountTypeId, AccountTypeSettings accountTypeSettings, out  List<AccountBalanceInvoiceInfo> accountBalanceInvoicesInfo)
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
            return invoiceManager.GetUnPaidPartnerInvoices(partnerInvoiceTypes);

        }
        private IEnumerable<BillingTransactionMetaData> GetBillingTransactionPaidData(Guid accountTypeId, List<BillingTransactionByTime> billingTransactionsByTime)
        {
            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            return billingTransactionManager.GetBillingTransactionsByTransactionTypes(accountTypeId, billingTransactionsByTime, this.TransactionTypeIds);
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
