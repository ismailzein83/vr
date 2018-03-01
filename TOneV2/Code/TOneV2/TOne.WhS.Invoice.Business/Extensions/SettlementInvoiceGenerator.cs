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
using Vanrise.Invoice.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SettlementInvoiceGenerator : InvoiceGenerator
    {
        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;
        public SettlementInvoiceGenerator(Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId)
        {
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = supplierInvoiceTypeId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));


            var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
            definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
            var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == context.InvoiceTypeId);
            financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");



            var settlementGenerationCustomSectionPayload = context.CustomSectionPayload as SettlementGenerationCustomSectionPayload;
            if (settlementGenerationCustomSectionPayload == null)
                return;

            decimal? commission = null;
            CommissionType? commissionType = null;
            if (settlementGenerationCustomSectionPayload != null)
            {
                if (settlementGenerationCustomSectionPayload.Commission.HasValue)
                {
                    commission = settlementGenerationCustomSectionPayload.Commission.Value;
                    commissionType = settlementGenerationCustomSectionPayload.CommissionType;
                }
            }

            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;
      
            string partnerType = null;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
            }
            else
            {
                partnerType = "Account";
            }


            IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices = null;
            IEnumerable<InvoiceItem> customerInvoiceItems = null;

            IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices = null;
            IEnumerable<InvoiceItem> supplierInvoiceItems = null;

            InvoiceItemManager _invoiceItemManager = new InvoiceItemManager();

            if (definitionSettings.ExtendedSettings.IsApplicableToCustomer)
            {
                customerInvoices = GetInvoices(settlementGenerationCustomSectionPayload.CustomerInvoiceIds);
                if (customerInvoices != null)
                {
                    ValidateInvoicesDates(customerInvoices, fromDate, toDate, context);
                    customerInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(_customerInvoiceTypeId, settlementGenerationCustomSectionPayload.CustomerInvoiceIds, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
                }
            }

            if (definitionSettings.ExtendedSettings.IsApplicableToSupplier)
            {
                supplierInvoices = GetInvoices(settlementGenerationCustomSectionPayload.SupplierInvoiceIds);
                if (supplierInvoices != null)
                {
                    ValidateInvoicesDates(supplierInvoices, fromDate, toDate, context);

                    supplierInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(_supplierInvoiceTypeId, settlementGenerationCustomSectionPayload.SupplierInvoiceIds, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
                }
            }


            Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic = ConvertInvoicesToItemSetNames(customerInvoices, supplierInvoices);
            if (itemSetNamesDic.Count == 0)
            {
                context.ErrorMessage = "No data available between the selected period.";
                throw new InvoiceGeneratorException("No data available between the selected period.");
            }
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic);

            #region BuildSupplierInvoiceDetails

            SattlementInvoiceDetails sattlementInvoiceDetails = BuilSattlementInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate);
            if (sattlementInvoiceDetails != null)
            {

                sattlementInvoiceDetails.PartnerType = partnerType;

                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = sattlementInvoiceDetails,
                    InvoiceItemSets = generatedInvoiceItemSets,
                };
            }
         
            #endregion
        }

        void ValidateInvoicesDates(IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices,DateTime fromDate,DateTime toDate, IInvoiceGenerationContext context)
        {
            if (invoices.Min(x => x.FromDate) < fromDate || invoices.Max(x => x.ToDate) > toDate)
            {
                context.ErrorMessage = "Unable to generate settlement at this period.";
                throw new InvoiceGeneratorException("Unable to generate settlement at this period.");
            }
        }
        IEnumerable<Vanrise.Invoice.Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().GetInvoices(invoiceIds);
        }
        Dictionary<string, List<SattlementInvoiceItemDetails>> ConvertInvoicesToItemSetNames(IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices, IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices)
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
        SattlementInvoiceDetails BuilSattlementInvoiceDetails(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate)
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
            //if (sattlementInvoiceDetails != null)
            //{
            //    sattlementInvoiceDetails.CurrencyId = currencyId;
            //}
            return sattlementInvoiceDetails;
        }
         List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic)
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
        private void AddItemToDictionary(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, string key, SattlementInvoiceItemDetails sattlementInvoiceItemDetails)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<string, List<SattlementInvoiceItemDetails>>();
            List<SattlementInvoiceItemDetails> invoiceBillingRecordList = itemSetNamesDic.GetOrCreateItem(key);
            invoiceBillingRecordList.Add(sattlementInvoiceItemDetails);
        }
    }
}
