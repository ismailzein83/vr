using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SattlementInvoiceGenerator : InvoiceGenerator
    {
        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;
        public SattlementInvoiceGenerator(Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId)
        {
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = supplierInvoiceTypeId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));

            int? timeZoneId = null;
            decimal? commission = null;
            CommissionType? commissionType = null;
            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;
      
            int currencyId = financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);
        
            string partnerType = null;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
            }
            else
            {
                partnerType = "Account";
            }

            var customerInvoices = GetCustomerInvoices(context.PartnerId, fromDate,  toDate);
            var supplierInvoices = GetSupplierInvoices(context.PartnerId,fromDate, toDate);

            if (customerInvoices.Min(x => x.FromDate) < fromDate || customerInvoices.Max(x => x.ToDate) > toDate || supplierInvoices.Min(x => x.FromDate) < fromDate || supplierInvoices.Max(x => x.ToDate) > toDate)
            {
                context.ErrorMessage = "Unable to generate settlement at this period.";
                throw new InvoiceGeneratorException("Unable to generate settlement at this period.");
            }

            Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic = ConvertInvoicesToItemSetNames(customerInvoices, supplierInvoices, currencyId);
            if (itemSetNamesDic.Count == 0)
            {
                context.ErrorMessage = "No data available between the selected period.";
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);
            #region BuildSupplierInvoiceDetails
            SattlementInvoiceDetails sattlementInvoiceDetails = BuilSattlementInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate, currencyId);
            if (sattlementInvoiceDetails != null)
            {
               // SetInvoiceBillingTransactions(context, sattlementInvoiceDetails, financialAccount, fromDate, toDateForBillingTransaction);
                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = sattlementInvoiceDetails,
                    InvoiceItemSets = generatedInvoiceItemSets,
                };
            }
            #endregion
        }

        public IEnumerable<Vanrise.Invoice.Entities.Invoice> GetCustomerInvoices(string partnerId, DateTime fromDate, DateTime toDate)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            return invoiceManager.GetPartnerInvoicesByDate(_customerInvoiceTypeId, partnerId, fromDate, toDate);
        }

        public IEnumerable<Vanrise.Invoice.Entities.Invoice> GetSupplierInvoices(string partnerId, DateTime fromDate, DateTime toDate)
        {
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            return invoiceManager.GetPartnerInvoicesByDate(_supplierInvoiceTypeId, partnerId, fromDate, toDate);
        }

        //private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, SattlementInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount, DateTime fromDate, DateTime toDate)
        //{
        //    var financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
        //    var balanceAccountTypeId = financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId);
        //    if (balanceAccountTypeId.HasValue)
        //    {
        //        Vanrise.Invoice.Entities.InvoiceType invoiceType = new Vanrise.Invoice.Business.InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
        //        invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
        //        invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
        //        SattlementInvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<SattlementInvoiceSettings>("invoiceType.Settings.ExtendedSettings");

        //        var billingTransaction = new GeneratedInvoiceBillingTransaction()
        //        {
        //            AccountTypeId = balanceAccountTypeId.Value,
        //            AccountId = context.PartnerId,
        //            TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
        //            Amount = invoiceDetails.TotalAmount,
        //            CurrencyId = invoiceDetails.CurrencyId,
        //            FromDate = fromDate,
        //            ToDate = toDate
        //        };

        //        billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
        //        billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();
        //        invoiceSettings.UsageTransactionTypeIds.ThrowIfNull("invoiceSettings.UsageTransactionTypeIds");
        //        foreach (Guid usageTransactionTypeId in invoiceSettings.UsageTransactionTypeIds)
        //        {
        //            billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
        //            {
        //                TransactionTypeId = usageTransactionTypeId
        //            });
        //        }
        //        context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
        //    }

        //}
        private SattlementInvoiceDetails BuilSattlementInvoiceDetails(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate, int currencyId)
        {
            SattlementInvoiceDetails sattlementInvoiceDetails = null;
            if (partnerType != null)
            {

                if (itemSetNamesDic != null)
                {
                    sattlementInvoiceDetails = new SattlementInvoiceDetails();
                    sattlementInvoiceDetails.PartnerType = partnerType;
                    List<SattlementInvoiceItemDetails> customerInvoices = null;
                    if (itemSetNamesDic.TryGetValue("CustomerInvoices", out customerInvoices))
                    {
                        foreach (var customerInvoice in customerInvoices)
                        {

                            sattlementInvoiceDetails.CustomerDuration += customerInvoice.DurationInSeconds;
                            sattlementInvoiceDetails.CustomerTotalNumberOfCalls += customerInvoice.TotalNumberOfCalls;
                        }
                       
                    };
                    
                    List<SattlementInvoiceItemDetails> supplierInvoices = null;
                    if (itemSetNamesDic.TryGetValue("SupplierInvoices", out supplierInvoices))
                    {
                        foreach (var supplierInvoice in supplierInvoices)
                        {
                            sattlementInvoiceDetails.SupplierDuration += supplierInvoice.DurationInSeconds;
                            sattlementInvoiceDetails.SupplierTotalNumberOfCalls += supplierInvoice.TotalNumberOfCalls;
                        }

                    };

                }
            }

            if (sattlementInvoiceDetails != null)
            {
                sattlementInvoiceDetails.CurrencyId = currencyId;
            }
            return sattlementInvoiceDetails;
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            if (itemSetNamesDic != null)
            {
                foreach (var itemSet in itemSetNamesDic)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                    generatedInvoiceItemSet.SetName = itemSet.Key;
                    var itemSetValues = itemSet.Value;
                    generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                    foreach (var item in itemSetValues)
                    {
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = item,
                            Name = " "
                        });
                    }
                    if (generatedInvoiceItemSet.Items.Count > 0)
                    {
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                    }
                }
            }
            return generatedInvoiceItemSets;
        }
        private Dictionary<string, List<SattlementInvoiceItemDetails>> ConvertInvoicesToItemSetNames(IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices, IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices, int currencyId)
        {
            Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic = new Dictionary<string, List<SattlementInvoiceItemDetails>>();

            if (customerInvoices != null)
            {
                foreach (var customerInvoice in customerInvoices)
                {
                    var customerInvoiceDetails = customerInvoice.Details as CustomerInvoiceDetails;
                    if (customerInvoiceDetails != null)
                    {
                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = customerInvoiceDetails.TotalAmountAfterCommission,
                            DurationInSeconds = customerInvoiceDetails.Duration,
                            CurrencyId = customerInvoiceDetails.SaleCurrencyId,
                            InvoiceId = customerInvoice.InvoiceId,
                            InvoiceTypeId = customerInvoice.InvoiceTypeId,
                            TotalNumberOfCalls = customerInvoiceDetails.TotalNumberOfCalls
                        };
                        AddItemToDictionary(itemSetNamesDic, "CustomerInvoices", sattlementInvoiceItemDetails);
                    }

                }
            }

            if (supplierInvoices != null)
            {
                foreach (var supplierInvoice in supplierInvoices)
                {

                    var supplierInvoiceDetails = supplierInvoice.Details as SupplierInvoiceDetails;
                    if (supplierInvoiceDetails != null)
                    {
                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = supplierInvoiceDetails.TotalAmountAfterCommission,
                            DurationInSeconds = supplierInvoiceDetails.Duration,
                            CurrencyId = supplierInvoiceDetails.SupplierCurrencyId,
                            InvoiceId = supplierInvoice.InvoiceId,
                            InvoiceTypeId = supplierInvoice.InvoiceTypeId,
                            TotalNumberOfCalls = supplierInvoiceDetails.TotalNumberOfCalls
                        };
                        AddItemToDictionary(itemSetNamesDic, "SupplierInvoices", sattlementInvoiceItemDetails);
                    }
                }
            }

            return itemSetNamesDic;
        }
        private void AddItemToDictionary(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, string key, SattlementInvoiceItemDetails sattlementInvoiceItemDetails)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<string, List<SattlementInvoiceItemDetails>>();
            List<SattlementInvoiceItemDetails> invoiceBillingRecordList = null;

            if (!itemSetNamesDic.TryGetValue(key, out invoiceBillingRecordList))
            {
                invoiceBillingRecordList = new List<SattlementInvoiceItemDetails>();
                invoiceBillingRecordList.Add(sattlementInvoiceItemDetails);
                itemSetNamesDic.Add(key, invoiceBillingRecordList);
            }
            else
            {
                invoiceBillingRecordList.Add(sattlementInvoiceItemDetails);
                itemSetNamesDic[key] = invoiceBillingRecordList;
            }
        }
    }
}
