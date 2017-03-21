using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
using Vanrise.Common;
using Vanrise.Common.Business;
namespace Vanrise.AccountBalance.MainExtensions.AccountBalanceFieldSource
{
    public class BillingTransactionFieldSourceSetting : AccountBalanceFieldSourceSetting
    {
        public override Guid ConfigId
        {
            get { return new Guid("143BBD63-6E9D-46F4-8B3C-D899E9966120"); }
        }
        public List<BillingTransactionField> BillingTransactionFields { get; set; }

        CurrencyExchangeRateManager currencyExchangeRateManager;
        BillingTransactionTypeManager billingTransactionTypeManager;
        AccountManager accountManager;
        public override List<AccountBalanceFieldDefinition> GetFieldDefinitions(IAccountBalanceFieldSourceGetFieldDefinitionsContext context)
        {
            List<AccountBalanceFieldDefinition> accountBalanceFieldDefinitions = new List<AccountBalanceFieldDefinition> ();

            foreach (var item in this.BillingTransactionFields)
            {
                accountBalanceFieldDefinitions.Add(new AccountBalanceFieldDefinition
                {
                    Name = item.FieldName,
                    Title = item.FieldTitle,
                    FieldType = new FieldNumberType { DataType = FieldNumberDataType.Decimal, DataPrecision = FieldNumberPrecision.Normal },
                });
            }
            return accountBalanceFieldDefinitions;
        }
        public override object PrepareSourceData(IAccountBalanceFieldSourcePrepareSourceDataContext context)
        {
            List<Guid> transactionTypes = new List<Guid>();
            foreach(var billingTransactionField in this.BillingTransactionFields)
            {
                foreach (var transactionTypeId in billingTransactionField.TransactionTypeIds)
                {
                    if(!transactionTypes.Contains(transactionTypeId))
                        transactionTypes.Add(transactionTypeId);

                }
            }
            List<string> accountIds = null;
            if (context.AccountBalances != null && context.AccountBalances.Count() > 0)
            {
                accountIds = new List<string>();
                foreach (var accountBalance in context.AccountBalances)
                {
                    accountIds.Add(accountBalance.AccountId);
                }
            }
            billingTransactionTypeManager = new BillingTransactionTypeManager();
            currencyExchangeRateManager = new CurrencyExchangeRateManager();
            accountManager = new AccountManager();
            var billingTransactions = new BillingTransactionManager().GetBillingTransactionsByAccountIds(context.AccountTypeId, transactionTypes, accountIds);
            Dictionary<string, Dictionary<Guid, decimal>> existingRecords = ApplyBillingTransactionFinalGrouping(context.AccountTypeId , billingTransactions);
            return existingRecords;
        }
        public override object GetFieldValue(IAccountBalanceFieldSourceGetFieldValueContext context)
        {
            Dictionary<string, Dictionary<Guid, decimal>> existingRecords = context.PreparedData as Dictionary<string, Dictionary<Guid, decimal>>;
            decimal fieldValue = 0;
            if(existingRecords != null)
            {
                var field =  this.BillingTransactionFields.FirstOrDefault(x=>x.FieldName == context.FieldName);
                if(field != null)
                {
                    Dictionary<Guid, decimal> fields;
                    if(existingRecords.TryGetValue(context.AccountBalance.AccountId, out fields))
                    {
                        fields.TryGetValue(field.FieldId, out fieldValue);
                    }
                }
            }
            return fieldValue;
        }
        private Dictionary<string, Dictionary<Guid, decimal>> ApplyBillingTransactionFinalGrouping(Guid accountTypeId, IEnumerable<BillingTransactionMetaData> billingTransactions)
        {
            Dictionary<string, Dictionary<Guid, decimal>> existingRecords = new Dictionary<string, Dictionary<Guid, decimal>>();

            foreach (var item in billingTransactions)
            {
                UpdatBillingTransactionValues(accountTypeId, existingRecords, item);
            }
            return existingRecords;
        }
        private void UpdatBillingTransactionValues(Guid accountTypeId, Dictionary<string, Dictionary<Guid, decimal>> existingRecords, BillingTransactionMetaData billingTransaction)
        {
          
            Dictionary<Guid, decimal> valuesByFieldId = existingRecords.GetOrCreateItem(billingTransaction.AccountId,() => {
                return new  Dictionary<Guid, decimal>();
            });

            var transactionType = billingTransactionTypeManager.GetBillingTransactionType(billingTransaction.TransactionTypeId);
            var accountInfo = accountManager.GetAccountInfo(accountTypeId, billingTransaction.AccountId);

            var amount = billingTransaction.Amount;
            if(billingTransaction.CurrencyId !=  accountInfo.CurrencyId)
               amount = ConvertValueToCurrency(billingTransaction.Amount, billingTransaction.CurrencyId, accountInfo.CurrencyId, billingTransaction.TransactionTime);

            foreach(var item in this.BillingTransactionFields)
            {
                if(item.TransactionTypeIds.Contains(billingTransaction.TransactionTypeId))
                {
                    decimal value;
                    if (valuesByFieldId.TryGetValue(item.FieldId, out value))
                    {
                        value += transactionType != null && transactionType.IsCredit ? amount : -amount;
                        if (!item.IsCredit)
                            value = -value;
                        valuesByFieldId[item.FieldId] = value;
                    }
                    else
                    {
                        value = transactionType != null && transactionType.IsCredit ? amount : -amount;
                        if (!item.IsCredit)
                            value = -value;
                        valuesByFieldId.Add(item.FieldId, value);
                    }
                   
                }
            }
           
        }
        private decimal ConvertValueToCurrency(decimal amount, int fromCurrencyId, int currencyId, DateTime effectiveOn)
        {
            return currencyExchangeRateManager.ConvertValueToCurrency(amount, fromCurrencyId, currencyId, effectiveOn);
        }

    }
    public class BillingTransactionField
    {
        public List<Guid> TransactionTypeIds { get; set; }
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public bool IsCredit { get; set; }
        public Guid FieldId { get; set; }
    }
}
