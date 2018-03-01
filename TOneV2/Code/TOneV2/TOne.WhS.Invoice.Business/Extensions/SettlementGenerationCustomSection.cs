using System;
using System.Linq;
using Vanrise.Common;
using Vanrise.Invoice.Entities;
using TOne.WhS.Invoice.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using System.Collections.Generic;
using Vanrise.Invoice.Business;
using Vanrise.Common.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SettlementGenerationCustomSection : GenerationCustomSection
    {
        CurrencyManager _currencyManager = new CurrencyManager();
        InvoiceItemManager _invoiceItemManager = new InvoiceItemManager();

        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;
        public SettlementGenerationCustomSection(Guid customerInvoiceTypeId,Guid sipplierInvoiceTypeId)
        {
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = sipplierInvoiceTypeId;
        }
        public override string GenerationCustomSectionDirective
        {
            get
            {
                return "whs-invoicetype-generationcustomsection-settlement";
            }
        }
        public override void EvaluateGenerationCustomPayload(IGetGenerationCustomPayloadContext context)
        {

            List<Vanrise.Invoice.Entities.Invoice> supplierInvoices = null;

            List<Vanrise.Invoice.Entities.Invoice> customerInvoices = null;

            IEnumerable<string> partnerIds = context.InvoiceGenerationInfo.MapRecords(x => x.PartnerId);
            if (context.InvoiceGenerationInfo != null)
            {
                foreach (var generationCustomPayload in context.InvoiceGenerationInfo)
                {
                    generationCustomPayload.CustomPayload = EvaluateGenerationCustomPayload(partnerIds, generationCustomPayload.PartnerId, context.InvoiceTypeId,  context.MinimumFrom, context.MaximumTo, supplierInvoices, customerInvoices);
                }
            }
        }
        private SettlementGenerationCustomSectionPayload EvaluateGenerationCustomPayload(IEnumerable<string> partnerIds, string currenctPartnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate, List<Vanrise.Invoice.Entities.Invoice> supplierInvoices, List<Vanrise.Invoice.Entities.Invoice> customerInvoices)
        {
            decimal? commission = null;
            CommissionType commissionType = CommissionType.Display;
            WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(currenctPartnerId));
            financialAccount.ThrowIfNull("financialAccount", currenctPartnerId);
            financialAccount.Settings.ThrowIfNull("financialAccount.Settings", currenctPartnerId);

            WHSFinancialAccountDefinitionManager whsFinancialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var definition = whsFinancialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);

            List<FinancialAccountInvoiceSetting> financialAccountInvoiceSettings = financialAccount.Settings.FinancialAccountInvoiceSettings;
            if (financialAccountInvoiceSettings != null && financialAccountInvoiceSettings.Count > 0)
            {
                FinancialAccountInvoiceSetting financialAccountInvoiceSetting = financialAccountInvoiceSettings.FindRecord(itm => itm.InvoiceTypeId == invoiceTypeId);

                if (financialAccountInvoiceSetting != null && financialAccountInvoiceSetting.FinancialAccountCommission != null)
                {
                    commission = financialAccountInvoiceSetting.FinancialAccountCommission.Commission;
                    commissionType = financialAccountInvoiceSetting.FinancialAccountCommission.CommissionType;
                }
            }

            var settlementGenerationCustomSectionPayload = new SettlementGenerationCustomSectionPayload
            {
                CommissionType = commissionType,
                Commission = commission,
                Summary = new SettlementGenerationCustomSectionPayloadSummary()
            };

            if (definition.ExtendedSettings.IsApplicableToCustomer)
            {
                List<long> invoiceIds;
                Dictionary<string, decimal> amountByCurrency;
                if (TryLoadInvoicesAndGetAmountByCurrency(_customerInvoiceTypeId, customerInvoices, "GroupingBySaleCurrency", partnerIds, fromDate, toDate, out invoiceIds, out amountByCurrency))
                {
                    settlementGenerationCustomSectionPayload.CustomerInvoiceIds = invoiceIds;
                    settlementGenerationCustomSectionPayload.Summary.CustomerAmountByCurrency = amountByCurrency;
                }
            }

            if (definition.ExtendedSettings.IsApplicableToSupplier)
            {
                List<long> invoiceIds;
                Dictionary<string, decimal> amountByCurrency;
                if (TryLoadInvoicesAndGetAmountByCurrency(_supplierInvoiceTypeId, supplierInvoices, "GroupingBySaleCurrency", partnerIds, fromDate, toDate, out invoiceIds, out amountByCurrency))
                {
                    settlementGenerationCustomSectionPayload.SupplierInvoiceIds = invoiceIds;
                    settlementGenerationCustomSectionPayload.Summary.SupplierAmountByCurrency = amountByCurrency;
                }
            }
            return settlementGenerationCustomSectionPayload;
        }
        private bool TryLoadInvoicesAndGetAmountByCurrency(Guid invoiceTypeId, List<Vanrise.Invoice.Entities.Invoice> invoices, string itemSetName,IEnumerable<string> partnerIds,DateTime fromDate,DateTime toDate, out List<long> invoiceIds, out Dictionary<string, decimal> amountByCurrency)
        {
            invoiceIds = null;
            amountByCurrency = null;

            if(invoices == null)
              invoices = new Vanrise.Invoice.Business.InvoiceManager().GetPartnerInvoicesByDate(_supplierInvoiceTypeId, partnerIds, fromDate, toDate);
            if(invoices != null && invoices.Count > 0)
            {
                invoiceIds = invoices.MapRecords(x => x.InvoiceId).ToList();
                amountByCurrency = LoadAndGetAmountByCurrency(invoiceTypeId, invoiceIds, itemSetName);
                return true;
            }
            return false;
        }
        private Dictionary<string,decimal> LoadAndGetAmountByCurrency(Guid invoiceTypeId,List<long> invoiceIds, string itemSetName)
        {
            Dictionary<string, decimal> amountByCurrency = null;
            var invoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(invoiceTypeId, invoiceIds, new List<string> { itemSetName }, CompareOperator.Equal);
            if (invoiceItems != null)
            {
                amountByCurrency = new Dictionary<string, decimal>();

                foreach (var invoiceItem in invoiceItems)
                {
                    var invoiceBySaleCurrencyItemDetails = invoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                    if (invoiceBySaleCurrencyItemDetails != null)
                    {
                        string currencySymbol = _currencyManager.GetCurrencySymbol(invoiceBySaleCurrencyItemDetails.CurrencyId);
                        currencySymbol.ThrowIfNull("currencySymbol", invoiceBySaleCurrencyItemDetails.CurrencyId);
                        amountByCurrency.Add(currencySymbol, invoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes);
                    }
                }
            }
            return amountByCurrency;
        }
    }
}
