//using Retail.BusinessEntity.Business;
//using Retail.BusinessEntity.Entities;
//using Retail.Demo.Data;
//using Retail.Demo.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Invoice.Entities;
//using Vanrise.Common.Business;

//namespace Retail.Demo.Business
//{
//    public class SMSInvoiceGenerator : InvoiceGenerator
//    {
//        Guid _acountBEDefinitionId { get; set; }
//        SMSInvoiceType _type;

//        public SMSInvoiceGenerator(Guid acountBEDefinitionId, SMSInvoiceType type)
//        {
//            this._acountBEDefinitionId = acountBEDefinitionId;
//            this._type = type;
//        }

//        #region Fields

//        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
//        private AccountBEManager _accountBEManager = new AccountBEManager();
//        private CurrencyManager _currencyManager = new CurrencyManager();

//        #endregion
//        public override void GenerateInvoice(IInvoiceGenerationContext context)
//        {
//            FinancialAccountData financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);

//            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
//            {
//                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
//                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
//                return;
//            }
//            decimal totalMCR;
//            decimal totalNCR;
//            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(financialAccountData.Account.AccountId, context.FromDate, context.ToDate, out totalMCR, out totalNCR);
//            decimal amount = 0;
//            if (generatedInvoiceItemSets != null && generatedInvoiceItemSets.Count > 0)
//                foreach (var generatedInvoiceItemSet in generatedInvoiceItemSets)
//                {
//                    foreach (var item in generatedInvoiceItemSet.Items)
//                    {
//                        amount += item.Details.TotalTarrif;
//                    }
//                }

//            var invoiceDetail= new InvoiceDetails()
//            {
//                Amount = amount,
//                Currency = _currencyManager.GetSystemCurrency().CurrencyId,
//                TotalMCR = totalMCR,
//                TotalNCR = totalNCR
//            };
//            context.Invoice = new GeneratedInvoice
//            {
//                InvoiceItemSets = generatedInvoiceItemSets,
//                InvoiceDetails = invoiceDetail
//            };
//            SetInvoiceBillingTransactions(context, invoiceDetail);

//        }
//        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, InvoiceDetails invoiceDetails)
//        {
//            var billingTransaction = new GeneratedInvoiceBillingTransaction()
//            {
//                AccountTypeId = new Guid("F260AEFF-2283-49B6-B371-8B314EFE8009"),
//                AccountId = context.PartnerId,
//                TransactionTypeId = new Guid("2B3D86AB-1689-49E8-A5FA-F65227A1EC4C"),
//                Amount = invoiceDetails.Amount,
//                CurrencyId = invoiceDetails.Currency
//            };
//            context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
//        }

//        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(long accountId, DateTime fromDate, DateTime toDate, out decimal totalMCR, out decimal totalNCR)
//        {
//            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
//            IDemoDataManager dataManager = RetailDemoDataManagerFactory.GetDataManager<IDemoDataManager>();

//            List<NewOrders> newOrders = new List<NewOrders>();
//            List<ActiveServices> activeServices = new List<ActiveServices>();

//            totalMCR = 0;
//            totalNCR = 0;

//            GeneratedInvoiceItemSet newOrdersInvoiceItemSet = new GeneratedInvoiceItemSet()
//            {
//                SetName = "NetworkRentalNewOrders",
//                Items = new List<GeneratedInvoiceItem>()
//            }; ;

//            newOrders = dataManager.GetNewOrders(accountId, fromDate, toDate);
//            if (newOrders != null && newOrders.Count > 0)
//            {
//                foreach (var newOrder in newOrders)
//                {
//                    totalNCR += newOrder.TotalTarrif;
//                    newOrdersInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
//                    {
//                        Details = newOrder,
//                        Name = " "
//                    });
//                }
//            }

//            generatedInvoiceItemSets.Add(newOrdersInvoiceItemSet);

//            GeneratedInvoiceItemSet activeServicesInvoiceItemSet = new GeneratedInvoiceItemSet()
//            {
//                SetName = "NetworkRentalActiveProducts",
//                Items = new List<GeneratedInvoiceItem>()
//            };

//            activeServices = dataManager.GetActiveServices(accountId, fromDate, toDate);
//            foreach (var activeService in activeServices)
//            {
//                totalMCR += activeService.TotalTarrif;
//                activeServicesInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
//                {
//                    Details = activeService,
//                    Name = " "
//                });
//            }
//            generatedInvoiceItemSets.Add(activeServicesInvoiceItemSet);
//            return generatedInvoiceItemSets;
//        }
//    }
//}
