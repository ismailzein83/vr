using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.AccountBalance.Business;
using Vanrise.Invoice.Business;
using Vanrise.InvToAccBalanceRelation.Business;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common.Business;
namespace Vanrise.AccountBalance.MainExtensions
{
    public enum DataSourceType { Invoice = 1 }

    public class BillingTransactionDataSourceSettings : InvoiceDataSourceSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("85BB0A94-C4C4-47B3-A575-3C630C0D000D"); }
        }
        public DataSourceType DataSourceType { get; set; }
        public string DataSourceName { get; set; }
        public List<Guid> TransactionTypesIds { get; set; }
        public int? TopRecords { get; set; }
        public override IEnumerable<dynamic> GetDataSourceItems(IInvoiceDataSourceSettingsContext context)
        {
            switch (this.DataSourceType)
            {
                case MainExtensions.DataSourceType.Invoice:
                    var invoice = context.InvoiceActionContext.GetInvoice();
                    var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoice.InvoiceTypeId);
                    invoiceType.ThrowIfNull("invoiceType");
                    invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
                    if(!invoiceType.Settings.InvToAccBalanceRelationId.HasValue)
                        new NullReferenceException("invoiceType.Settings.InvToAccBalanceRelationId");
                    BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
                    List<BillingTransactionItem> billingTransactionItems = new List<BillingTransactionItem>();
                    var invoiceDataSourceItems = context.GetDataSourceItems(this.DataSourceName, null);
                    if (invoiceDataSourceItems != null && invoiceDataSourceItems.Count() > 0)
                    {
                        InvToAccBalanceRelationDefinitionManager invToAccBalanceRelationDefinitionManager = new InvToAccBalanceRelationDefinitionManager();
                        var relationExtendedSettings = invToAccBalanceRelationDefinitionManager.GetRelationExtendedSettings(invoiceType.Settings.InvToAccBalanceRelationId.Value);
                        InvToAccBalanceRelGetInvoiceBalanceAccountsContext invToAccBalanceRelGetInvoiceBalanceAccountsContext = new InvToAccBalanceRelGetInvoiceBalanceAccountsContext
                        {
                            PartnerId = invoice.PartnerId
                        };
                        var invoiceBalanceAccounts = relationExtendedSettings.GetInvoiceBalanceAccounts(invToAccBalanceRelGetInvoiceBalanceAccountsContext);
                        invoiceDataSourceItems.CastWithValidate<IEnumerable<InvoiceDataSourceItem>>("invoiceDataSourceItems");
                        DateTime minDateTime = invoiceDataSourceItems.Min(x => x.CreatedTime);
                        List<Guid> accountTypeIds = new List<Guid>();
                        List<string> accountIds = new List<string>();
                        foreach (var invoiceBalanceAccount in invoiceBalanceAccounts)
                        {
                            accountTypeIds.Add(invoiceBalanceAccount.AccountTypeId);
                            accountIds.Add(invoiceBalanceAccount.AccountId);
                        }
                        var billingTransactions = billingTransactionManager.GetBillingTransactions(accountTypeIds, accountIds, this.TransactionTypesIds, minDateTime, invoice.IssueDate);
                        if (billingTransactions != null)
                        {
                            CurrencyManager currencyManager = new CurrencyManager();
                            BillingTransactionTypeManager billingTransactionTypeManager = new BillingTransactionTypeManager();
                            var orderedBillingTransactions = billingTransactions.OrderByDescending(x => x.TransactionTime);
                            if(orderedBillingTransactions != null)
                            {

                               
                                
                                foreach (var billingTransaction in orderedBillingTransactions)
                                {
                                    if (this.TopRecords.HasValue)
                                    {
                                        if (billingTransactionItems.Count == this.TopRecords.Value)
                                            return billingTransactionItems;
                                    }

                                    billingTransactionItems.Add(new BillingTransactionItem
                                    {
                                        Amount = billingTransaction.Amount,
                                        Notes = billingTransaction.Notes,
                                        TransactionTime = billingTransaction.TransactionTime,
                                        CurrencySymbol = currencyManager.GetCurrencySymbol(billingTransaction.CurrencyId),
                                        TransactionTypeName = billingTransactionTypeManager.GetBillingTransactionTypeName(billingTransaction.TransactionTypeId)
                                    });

                                   
                                }
                            }
                           
                        }
                    }
                    return billingTransactionItems;
            }
            return null;
        }
    }
}
