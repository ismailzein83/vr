using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Invoice.Business.Extensions;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Business.Context;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business
{
    public class InvoiceSettlementManager
    {
        InvoiceItemManager _invoiceItemManager = new InvoiceItemManager();
        CurrencyManager _currencyManager = new CurrencyManager();
        const string _groupingBySaleCurrencyItemSetName = "GroupingByCurrency";

        #region Public Methods

        public SettlementGenerationCustomSectionPayloadSummary TryLoadInvoicesAndGetAmountByCurrency(Guid invoiceTypeId, List<SelectedInvoiceItem> selectedCustomerInvoices, List<SelectedInvoiceItem> selectedSupplierInvoices, DateTime fromDate, DateTime toDate)
        {
            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<SettlementInvoiceSettings>("settlementInvoiceSettings", invoiceTypeId);

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var normalPrecisionValue = new GeneralSettingsManager().GetNormalPrecisionValue();

            SettlementGenerationCustomSectionPayloadSummary summary = new SettlementGenerationCustomSectionPayloadSummary();
            if (selectedCustomerInvoices != null && selectedCustomerInvoices.Count > 0)
            {
                var customerInvoices = invoiceManager.GetInvoices(selectedCustomerInvoices.MapRecords(x => x.InvoiceId).ToList());

                string errorMessage;
                Dictionary<string, decimal> customerAmountByCurrency;
                List<InvoiceItem> customerInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(settlementInvoiceSettings.CustomerInvoiceTypeId, customerInvoices.MapRecords(x => x.InvoiceId).ToList(), new List<string> { _groupingBySaleCurrencyItemSetName }, CompareOperator.Equal).ToList();
                if (TryGetAmountByCurrency(settlementInvoiceSettings.CustomerInvoiceTypeId, customerInvoices,customerInvoiceItems, selectedCustomerInvoices, fromDate, toDate, normalPrecisionValue, false, out customerAmountByCurrency, out  errorMessage))
                {
                    summary.CustomerAmountByCurrency = customerAmountByCurrency;
                }
                else
                {
                    summary.ErrorMessage = errorMessage;
                }
            }
            if (selectedSupplierInvoices != null && selectedSupplierInvoices.Count > 0)
            {
                var supplierInvoices = invoiceManager.GetInvoices(selectedSupplierInvoices.MapRecords(x => x.InvoiceId).ToList());

                string errorMessage;
                Dictionary<string, decimal> supplierAmountByCurrency;
                List<InvoiceItem> supplierInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(settlementInvoiceSettings.SupplierInvoiceTypeId, supplierInvoices.MapRecords(x => x.InvoiceId).ToList(), new List<string> { _groupingBySaleCurrencyItemSetName }, CompareOperator.Equal).ToList();
                if (TryGetAmountByCurrency(settlementInvoiceSettings.SupplierInvoiceTypeId, supplierInvoices,supplierInvoiceItems, selectedSupplierInvoices, fromDate, toDate, normalPrecisionValue, false, out supplierAmountByCurrency, out  errorMessage))
                {
                    summary.SupplierAmountByCurrency = supplierAmountByCurrency;
                }
                else
                {
                    summary.ErrorMessage = errorMessage;
                }
            }
            return summary;
        }
        public SettlementGenerationCustomSectionInvoices LoadInvoicesDetails(Guid invoiceTypeId, string partnerId, List<SelectedInvoiceItem> customerInvoices, List<SelectedInvoiceItem> supplierInvoices)
        {
            SettlementGenerationCustomSectionInvoices settlementGenerationCustomSectionInvoices = new SettlementGenerationCustomSectionInvoices();

            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<SettlementInvoiceSettings>("settlementInvoiceSettings", invoiceTypeId);

            settlementGenerationCustomSectionInvoices.PartnerName = invoiceTypeExtendedSettings.GetPartnerManager().GetFullPartnerName(new PartnerNameManagerContext
            {
                PartnerId = partnerId
            });

            if (customerInvoices != null && customerInvoices.Count > 0)
            {
                settlementGenerationCustomSectionInvoices.CustomerInvoiceDetails = GetCustomerInvoiceDetail(settlementInvoiceSettings.CustomerInvoiceTypeId, partnerId, customerInvoices);
            }

            if (supplierInvoices != null && supplierInvoices.Count > 0)
            {
                settlementGenerationCustomSectionInvoices.SupplierInvoiceDetails = GetSupplierInvoiceDetail(settlementInvoiceSettings.SupplierInvoiceTypeId, partnerId, supplierInvoices);
            }

            return settlementGenerationCustomSectionInvoices;
        }
        public SettlementGenerationCustomSectionPayload EvaluatePartnerCustomPayload(string currentPartnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate)
        {
            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<SettlementInvoiceSettings>("settlementInvoiceSettings", invoiceTypeId);
            List<InvoiceItem> customerInvoiceItems = new List<InvoiceItem>(), supplierInvoiceItems = new List<InvoiceItem>();
            List<Vanrise.Invoice.Entities.Invoice> customerInvoices = new List<Vanrise.Invoice.Entities.Invoice>(), supplierInvoices = new List<Vanrise.Invoice.Entities.Invoice>();

            return EvaluateGenerationCustomPayload(settlementInvoiceSettings.CustomerInvoiceTypeId, settlementInvoiceSettings.SupplierInvoiceTypeId, new List<string> { currentPartnerId }, currentPartnerId, invoiceTypeId, fromDate, toDate, supplierInvoices, supplierInvoiceItems, customerInvoices, customerInvoiceItems, true);
        }
        public SettlementGenerationCustomSectionPayload EvaluateGenerationCustomPayload(Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId, IEnumerable<string> partnerIds, string currentPartnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate, List<Vanrise.Invoice.Entities.Invoice> supplierInvoices, List<InvoiceItem> supplierInvoiceItems, List<Vanrise.Invoice.Entities.Invoice> customerInvoices, List<InvoiceItem> customerInvoiceItems, bool getAllCurrencies)
        {
           // decimal? commission = null;
            int normalPrecisionValue;
            bool isApplicableToCustomer, isApplicableToSupplier;
            PrepareDataFoProcessing(currentPartnerId, invoiceTypeId, customerInvoiceTypeId, supplierInvoiceTypeId, partnerIds, fromDate, toDate, customerInvoices,customerInvoiceItems, supplierInvoices,supplierInvoiceItems, out isApplicableToCustomer, out isApplicableToSupplier, out  normalPrecisionValue);

            var settlementGenerationCustomSectionPayload = new SettlementGenerationCustomSectionPayload
            {
                IsCustomerApplicable = isApplicableToCustomer,
                IsSupplierApplicable = isApplicableToSupplier,
              //  Commission = commission,
                Summary = new SettlementGenerationCustomSectionPayloadSummary()
            };


            if (isApplicableToCustomer)
            {
                string errorMessage;
                Dictionary<string, decimal> customerAmountByCurrency;
                settlementGenerationCustomSectionPayload.AvailableCustomerInvoices = GetAvailableInvoices(customerInvoiceTypeId, currentPartnerId, customerInvoices,customerInvoiceItems, partnerIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out  customerAmountByCurrency, out  errorMessage);
                settlementGenerationCustomSectionPayload.Summary.ErrorMessage = errorMessage;
                settlementGenerationCustomSectionPayload.Summary.CustomerAmountByCurrency = customerAmountByCurrency;
            }

            if (isApplicableToSupplier)
            {
                string errorMessage;
                Dictionary<string, decimal> supplierAmountByCurrency;
                settlementGenerationCustomSectionPayload.AvailableSupplierInvoices = GetAvailableInvoices(supplierInvoiceTypeId, currentPartnerId, supplierInvoices,supplierInvoiceItems, partnerIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out supplierAmountByCurrency, out  errorMessage);
                if(settlementGenerationCustomSectionPayload.Summary.ErrorMessage == null)
                    settlementGenerationCustomSectionPayload.Summary.ErrorMessage = errorMessage;
                settlementGenerationCustomSectionPayload.Summary.SupplierAmountByCurrency = supplierAmountByCurrency;
            }
            return settlementGenerationCustomSectionPayload;
        }

        #endregion

        #region Private Methods

        private List<SupplierInvoiceDetail> GetSupplierInvoiceDetail(Guid invoiceTypeId, string partnerId, List<SelectedInvoiceItem> selectedSupplierInvoices)
        {

            List<SupplierInvoiceDetail> supplierInvoiceDetails = null;
            var supplierInvoiceIds = selectedSupplierInvoices.MapRecords(x => x.InvoiceId).ToList();

            var invoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(invoiceTypeId, supplierInvoiceIds, new List<string> { _groupingBySaleCurrencyItemSetName }, CompareOperator.Equal);

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            VRTimeZoneManager vrTimeZoneManager = new VRTimeZoneManager();

            var invoiceDetails = invoiceManager.GetInvoicesDetails(invoiceTypeId, partnerId, supplierInvoiceIds);

            if (invoiceDetails != null && invoiceDetails.Count() > 0)
            {

                supplierInvoiceDetails = new List<SupplierInvoiceDetail>();

                foreach (var invoiceDetail in invoiceDetails)
                {
                    var currentInvoiceItems = invoiceItems.FindAllRecords(x => x.InvoiceId == invoiceDetail.Entity.InvoiceId);
                    if (currentInvoiceItems != null)
                    {
                        var supplierDetail = invoiceDetail.Entity.Details as SupplierInvoiceDetails;
                        supplierDetail.ThrowIfNull("supplierDetail");
                        foreach (var currentInvoiceItem in currentInvoiceItems)
                        {
                            var currentInvoiceItemDetail = currentInvoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                            bool useOriginalAmount = false;
                            decimal? originalAmount = null;
                            if (supplierDetail.OriginalAmountByCurrency != null)
                            {
                                var record = supplierDetail.OriginalAmountByCurrency.GetRecord(currentInvoiceItemDetail.CurrencyId);
                                if (record != null)
                                {
                                    useOriginalAmount =record.IncludeOriginalAmountInSettlement;
                                    originalAmount = record.OriginalAmount;
                                }
                            }
                            var supplierInvoiceDetail = supplierInvoiceDetails.FindRecord(x => x.InvoiceId == invoiceDetail.Entity.InvoiceId && x.CurrencyId == currentInvoiceItemDetail.CurrencyId);
                            if (supplierInvoiceDetail != null)
                            {
                                supplierInvoiceDetail.TotalAmountAfterCommission += currentInvoiceItemDetail.AmountAfterCommissionWithTaxes;
                                supplierInvoiceDetail.TotalNumberOfCalls += currentInvoiceItemDetail.NumberOfCalls;
                                supplierInvoiceDetail.Duration += currentInvoiceItemDetail.Duration;
                                supplierInvoiceDetail.OriginalAmount = originalAmount;
                                if (!supplierInvoiceDetail.HasRecurringCharge)
                                    supplierInvoiceDetail.HasRecurringCharge = currentInvoiceItemDetail.TotalRecurringChargeAmount > 0;

                            }
                            else
                            {
                                supplierInvoiceDetails.Add(new SupplierInvoiceDetail
                            {
                                InvoiceId = invoiceDetail.Entity.InvoiceId,
                                TotalNumberOfCalls = currentInvoiceItemDetail.NumberOfCalls,
                                TimeZoneId = supplierDetail.TimeZoneId,
                                CurrencyId = currentInvoiceItemDetail.CurrencyId,
                                TotalAmountAfterCommission = currentInvoiceItemDetail.AmountAfterCommissionWithTaxes,
                                SupplierCurrency = _currencyManager.GetCurrencySymbol(currentInvoiceItemDetail.CurrencyId),
                                DueDate = invoiceDetail.Entity.DueDate,
                                Commission = supplierDetail.Commission,
                                FromDate = invoiceDetail.Entity.FromDate,
                                IssueDate = invoiceDetail.Entity.IssueDate,
                                Duration = currentInvoiceItemDetail.Duration,
                                Offset = supplierDetail.Offset,
                                ToDate = invoiceDetail.Entity.ToDate,
                                SerialNumber = invoiceDetail.Entity.SerialNumber,
                                TimeZoneDescription = supplierDetail.TimeZoneId.HasValue ? vrTimeZoneManager.GetVRTimeZoneName(supplierDetail.TimeZoneId.Value) : null,
                                OriginalAmount = originalAmount ,
                                UseOriginalAmount= useOriginalAmount,
                                IsLocked = invoiceDetail.Entity.LockDate.HasValue,
                                HasRecurringCharge = currentInvoiceItemDetail.TotalRecurringChargeAmount > 0
                                });
                            }
                        }
                    }
                }
            }
            return supplierInvoiceDetails;
        }
        private List<CustomerInvoiceDetail> GetCustomerInvoiceDetail(Guid invoiceTypeId, string partnerId, List<SelectedInvoiceItem> selectedCustomerInvoices)
        {

            List<CustomerInvoiceDetail> customerInvoiceDetails = null;
            var customerInvoiceIds = selectedCustomerInvoices.MapRecords(x => x.InvoiceId).ToList();
            var invoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(invoiceTypeId, customerInvoiceIds, new List<string> { _groupingBySaleCurrencyItemSetName }, CompareOperator.Equal);

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            VRTimeZoneManager vrTimeZoneManager = new VRTimeZoneManager();

            var invoiceDetails = invoiceManager.GetInvoicesDetails(invoiceTypeId, partnerId, customerInvoiceIds);

            if (invoiceDetails != null && invoiceDetails.Count() > 0 && invoiceItems != null)
            {
                customerInvoiceDetails = new List<CustomerInvoiceDetail>();

                foreach (var invoiceDetail in invoiceDetails)
                {
                    var currentInvoiceItems = invoiceItems.FindAllRecords(x => x.InvoiceId == invoiceDetail.Entity.InvoiceId);

                    if (currentInvoiceItems != null)
                    {
                        foreach (var currentInvoiceItem in currentInvoiceItems)
                        {
                            var currentInvoiceItemDetail = currentInvoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                            var customerDetail = invoiceDetail.Entity.Details as CustomerInvoiceDetails;
                            customerDetail.ThrowIfNull("customerDetail");

                            bool useOriginalAmount = false;
                            decimal? originalAmount = null;
                            if (customerDetail.OriginalAmountByCurrency != null)
                            {
                                var record = customerDetail.OriginalAmountByCurrency.GetRecord(currentInvoiceItemDetail.CurrencyId);
                                if (record != null)
                                {
                                    useOriginalAmount = record.IncludeOriginalAmountInSettlement;
                                    originalAmount = record.OriginalAmount;
                                }
                            }
                            var customerInvoiceDetail = customerInvoiceDetails.FindRecord(x => x.InvoiceId == invoiceDetail.Entity.InvoiceId && x.CurrencyId == currentInvoiceItemDetail.CurrencyId);
                            if (customerInvoiceDetail != null)
                            {
                                customerInvoiceDetail.TotalAmountAfterCommission += currentInvoiceItemDetail.AmountAfterCommissionWithTaxes;
                                customerInvoiceDetail.TotalNumberOfCalls += currentInvoiceItemDetail.NumberOfCalls;
                                customerInvoiceDetail.Duration += currentInvoiceItemDetail.Duration;
                                customerInvoiceDetail.OriginalAmount = originalAmount;
                                if (!customerInvoiceDetail.HasRecurringCharge)
                                    customerInvoiceDetail.HasRecurringCharge = currentInvoiceItemDetail.TotalRecurringChargeAmount > 0;
                            }
                            else
                            {
                                customerInvoiceDetails.Add(new CustomerInvoiceDetail
                                {
                                    InvoiceId = invoiceDetail.Entity.InvoiceId,
                                    TotalNumberOfCalls = currentInvoiceItemDetail.NumberOfCalls,
                                    CurrencyId = currentInvoiceItemDetail.CurrencyId,
                                    TimeZoneId = customerDetail.TimeZoneId,
                                    TotalAmountAfterCommission = currentInvoiceItemDetail.AmountAfterCommissionWithTaxes,
                                    SaleCurrency = _currencyManager.GetCurrencySymbol(currentInvoiceItemDetail.CurrencyId),
                                    DueDate = invoiceDetail.Entity.DueDate,
                                    Commission = customerDetail.Commission,
                                    FromDate = invoiceDetail.Entity.FromDate,
                                    IssueDate = invoiceDetail.Entity.IssueDate,
                                    Duration = currentInvoiceItemDetail.Duration,
                                    Offset = customerDetail.Offset,
                                    ToDate = invoiceDetail.Entity.ToDate,
                                    SerialNumber = invoiceDetail.Entity.SerialNumber,
                                    TimeZoneDescription = customerDetail.TimeZoneId.HasValue ? vrTimeZoneManager.GetVRTimeZoneName(customerDetail.TimeZoneId.Value) : null,
                                    IsLocked = invoiceDetail.Entity.LockDate.HasValue,
                                    OriginalAmount = originalAmount,
                                    UseOriginalAmount = useOriginalAmount,
                                    HasRecurringCharge = currentInvoiceItemDetail.TotalRecurringChargeAmount > 0

                                });
                            }
                        }
                    }
                }
            }
            return customerInvoiceDetails;
        }
        private List<Vanrise.Invoice.Entities.Invoice> LoadInvoices(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);
        }
        private void PrepareDataFoProcessing(string currentPartnerId, Guid invoiceTypeId, Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate, List<Vanrise.Invoice.Entities.Invoice> customerInvoices, List<InvoiceItem> customerInvoiceItems, List<Vanrise.Invoice.Entities.Invoice> supplierInvoices, List<InvoiceItem> supplierInvoiceItems, out bool isApplicableToCustomer, out bool isApplicableToSupplier, out int normalPrecisionValue)
        {
            normalPrecisionValue = new GeneralSettingsManager().GetNormalPrecisionValue();
           
            WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(currentPartnerId));
            financialAccount.ThrowIfNull("financialAccount", currentPartnerId);
            financialAccount.Settings.ThrowIfNull("financialAccount.Settings", currentPartnerId);

            WHSFinancialAccountDefinitionManager whsFinancialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var definition = whsFinancialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            definition.ExtendedSettings.ThrowIfNull("definition.ExtendedSettings");

            isApplicableToCustomer = definition.ExtendedSettings.IsApplicableToCustomer;
            isApplicableToSupplier = definition.ExtendedSettings.IsApplicableToSupplier;

            if (isApplicableToCustomer && (customerInvoices == null || customerInvoices.Count() == 0))
            {
              
                var customerInvoicesEntities = LoadInvoices(customerInvoiceTypeId, partnerIds, fromDate, toDate);
                if (customerInvoicesEntities != null)
                {
                    customerInvoices.AddRange(customerInvoicesEntities);
                }
                var customerInvoiceItemsEntities = _invoiceItemManager.GetInvoiceItemsByItemSetNames(customerInvoiceTypeId, customerInvoices.MapRecords(x => x.InvoiceId).ToList(), new List<string> { _groupingBySaleCurrencyItemSetName }, CompareOperator.Equal).ToList();
                if (customerInvoiceItemsEntities != null)
                {
                    customerInvoiceItems.AddRange(customerInvoiceItemsEntities);
                }
            }

            if (isApplicableToSupplier && (supplierInvoices == null || supplierInvoices.Count() == 0))
            {

                var supplierInvoicesEntities = LoadInvoices(supplierInvoiceTypeId, partnerIds, fromDate, toDate);
                if (supplierInvoicesEntities != null)
                {
                    supplierInvoices.AddRange(supplierInvoicesEntities);
                }

                var supplierInvoiceItemsEntities = _invoiceItemManager.GetInvoiceItemsByItemSetNames(supplierInvoiceTypeId, supplierInvoices.MapRecords(x => x.InvoiceId).ToList(), new List<string> { _groupingBySaleCurrencyItemSetName }, CompareOperator.Equal).ToList();
                if (supplierInvoiceItemsEntities != null)
                {
                    supplierInvoiceItems.AddRange(supplierInvoiceItemsEntities);
                }
            }

        }


        private List<InvoiceAvailableForSettlement> GetAvailableInvoices(Guid invoiceTypeId, string currentPartnerId, List<Vanrise.Invoice.Entities.Invoice> invoices, List<InvoiceItem> invoiceItems, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate, int normalPrecisionValue, bool getAllCurrencies, out Dictionary<string, decimal> amountByCurrency, out string errorMessage)
        {
            List<InvoiceAvailableForSettlement> availableInvoices = null;
            amountByCurrency = null;
            errorMessage = null;
            if (invoices != null && invoices.Count > 0)
            {
                var partnerInvoices = invoices.FindAllRecords(x => x.PartnerId == currentPartnerId);
                if (partnerInvoices != null & partnerInvoices.Count() > 0)
                {
                    List<SelectedInvoiceItem> selectedInvoiceIds = new List<SelectedInvoiceItem>();
                    foreach (var partnerInvoice in partnerInvoices)
                    {
                        if (availableInvoices == null)
                            availableInvoices = new List<InvoiceAvailableForSettlement>();

                        var invoiceDetails = partnerInvoice.Details as BaseInvoiceDetails;
                        if (invoiceDetails != null)
                        {
                            var items = invoiceItems.FindAllRecords(x => x.InvoiceId == partnerInvoice.InvoiceId);
                            if (items != null)
                            {
                                foreach (var invoiceItem in items)
                                {
                                    var invoiceItemDetail = invoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                                    if (!availableInvoices.Any(x => x.InvoiceId == partnerInvoice.InvoiceId && x.CurrencyId == invoiceItemDetail.CurrencyId))
                                    {
                                        availableInvoices.Add(new InvoiceAvailableForSettlement
                                        {
                                            InvoiceId = partnerInvoice.InvoiceId,
                                            IsSelected = true,
                                            CurrencyId = invoiceItemDetail.CurrencyId
                                        });
                                        selectedInvoiceIds.Add(new SelectedInvoiceItem { InvoiceId = partnerInvoice.InvoiceId, CurrencyId = invoiceItemDetail.CurrencyId });
                                    }
                                }
                            }
                        }
                    }
                    Dictionary<string, decimal> amountsByCurrency = null;
                    if (!TryGetAmountByCurrency(invoiceTypeId, partnerInvoices, invoiceItems, selectedInvoiceIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out amountsByCurrency, out  errorMessage))
                    {
                        return null;
                    }
                    if (amountsByCurrency != null)
                    {
                        foreach (var amount in amountsByCurrency)
                        {
                            if (amountByCurrency == null)
                                amountByCurrency = new Dictionary<string, decimal>();
                            if (!amountByCurrency.ContainsKey(amount.Key))
                            {
                                amountByCurrency.Add(amount.Key, amount.Value);
                            }
                        }
                    }
                }
            }
            return availableInvoices;
        }
        private bool ValidateInvoicesDates(IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, DateTime fromDate, DateTime toDate, out string errorMessage)
        {
            errorMessage = null;

            if (invoices.Min(x => x.FromDate) < fromDate || invoices.Max(x => x.ToDate) > toDate)
            {
                errorMessage = "Unable to generate settlement at this period.";
                return false;
            }
            return true;

        }
        private bool TryGetAmountByCurrency(Guid invoiceTypeId, IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, List<InvoiceItem> invoiceItems, List<SelectedInvoiceItem> selectedInvoices, DateTime fromDate, DateTime toDate, int normalPrecisionValue, bool getAllCurrencies, out Dictionary<string, decimal> amountByCurrency, out string errorMessage)
        {
            errorMessage = null;
            amountByCurrency = null;
            if (invoices != null && invoices.Count() > 0)
            {
                if (!ValidateInvoicesDates(invoices, fromDate, toDate, out errorMessage))
                    return false;
                amountByCurrency = LoadAndGetAmountByCurrency(invoiceTypeId, invoices, selectedInvoices, invoiceItems, _groupingBySaleCurrencyItemSetName, normalPrecisionValue, getAllCurrencies);
            }
            return true;
        }
        private Dictionary<string, decimal> LoadAndGetAmountByCurrency(Guid invoiceTypeId, IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, List<SelectedInvoiceItem> selectedInvoiceIds, List<InvoiceItem> invoiceItems, string itemSetName, int normalPrecisionValue, bool getAllCurrencies)
        {
            Dictionary<string, decimal> amountByCurrency = null;
            if (selectedInvoiceIds != null && selectedInvoiceIds.Count > 0)
            {
                if (invoiceItems != null)
                {
                    amountByCurrency = GetAmountByCurrency(invoiceTypeId, invoices, invoiceItems, selectedInvoiceIds, itemSetName, normalPrecisionValue, getAllCurrencies);
                }
            }
            return amountByCurrency;
        }
        private Dictionary<string, decimal> GetAmountByCurrency(Guid invoiceTypeId,IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, IEnumerable<InvoiceItem> invoiceItems, List<SelectedInvoiceItem> selectedInvoiceIds, string itemSetName, int normalPrecisionValue, bool getAllCurrencies)
        {
            Dictionary<string, decimal> amountByCurrency = null;
            if (invoices != null && invoices.Count() > 0)
            {
                amountByCurrency = new Dictionary<string, decimal>();
                foreach (var invoice in invoices)
                {
                    bool newInvoiceCheck = true;
                    if (getAllCurrencies || selectedInvoiceIds.Any(x => x.InvoiceId == invoice.InvoiceId))
                    {
                        var innvoiceDetails = invoice.Details as BaseInvoiceDetails;
                        if (innvoiceDetails != null)
                        {
                            var items = invoiceItems.FindAllRecords(x => x.InvoiceId == invoice.InvoiceId);
                            foreach (var invoiceItem in items)
                            {
                                var invoiceItemDetail = invoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                                invoiceItemDetail.ThrowIfNull("invoiceBySaleCurrencyItemDetails");
                                if (invoiceItemDetail != null)
                                {
                                    if (getAllCurrencies || selectedInvoiceIds.Any(x => x.InvoiceId == invoice.InvoiceId && x.CurrencyId == invoiceItemDetail.CurrencyId))
                                    {

                                        string currencySymbol = _currencyManager.GetCurrencySymbol(invoiceItemDetail.CurrencyId);
                                        currencySymbol.ThrowIfNull("currencySymbol", invoiceItemDetail.CurrencyId);
                                        OriginalDataCurrrency originalDataCurrrency;
                                        if (innvoiceDetails.OriginalAmountByCurrency != null && innvoiceDetails.OriginalAmountByCurrency.TryGetValue(invoiceItemDetail.CurrencyId, out originalDataCurrrency) && originalDataCurrrency.IncludeOriginalAmountInSettlement && originalDataCurrrency.OriginalAmount.HasValue)
                                        {
                                            decimal amountValue;
                                            if (!amountByCurrency.TryGetValue(currencySymbol, out amountValue))
                                            {
                                                amountByCurrency.Add(currencySymbol, Math.Round(originalDataCurrrency.OriginalAmount.Value, normalPrecisionValue));
                                            }
                                            else if(newInvoiceCheck)
                                            {
                                                amountByCurrency[currencySymbol] = amountValue + Math.Round(originalDataCurrrency.OriginalAmount.Value, normalPrecisionValue);
                                            }
                                        }
                                        else
                                        {
                                            decimal amountValue;
                                            if (!amountByCurrency.TryGetValue(currencySymbol, out amountValue))
                                            {
                                                amountByCurrency.Add(currencySymbol, Math.Round(invoiceItemDetail.AmountAfterCommissionWithTaxes, normalPrecisionValue));
                                            }
                                            else
                                            {
                                                amountByCurrency[currencySymbol] = amountValue + Math.Round(invoiceItemDetail.AmountAfterCommissionWithTaxes, normalPrecisionValue);
                                            }
                                        }
                                    }
                                }
                                newInvoiceCheck = false;
                            }
                        }
                    }
                }
            }
            return amountByCurrency;
        }
        #endregion

    }

    public class SelectedInvoiceItem
    {
        public long InvoiceId { get; set; }
        public int CurrencyId { get; set; }
    }
}
