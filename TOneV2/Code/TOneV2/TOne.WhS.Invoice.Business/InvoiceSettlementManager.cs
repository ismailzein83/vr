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
                if (TryGetCustomerAmountByCurrency(settlementInvoiceSettings.CustomerInvoiceTypeId, customerInvoices,customerInvoiceItems, selectedCustomerInvoices, fromDate, toDate, normalPrecisionValue, false, out customerAmountByCurrency, out  errorMessage))
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
                if (TryGetSupplierAmountByCurrency(settlementInvoiceSettings.SupplierInvoiceTypeId, supplierInvoices,supplierInvoiceItems, selectedSupplierInvoices, fromDate, toDate, normalPrecisionValue, false, out supplierAmountByCurrency, out  errorMessage))
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
                settlementGenerationCustomSectionPayload.AvailableCustomerInvoices = GetAvailableCustomerInvoices(customerInvoiceTypeId, currentPartnerId, customerInvoices,customerInvoiceItems, partnerIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out  customerAmountByCurrency, out  errorMessage);
                settlementGenerationCustomSectionPayload.Summary.ErrorMessage = errorMessage;
                settlementGenerationCustomSectionPayload.Summary.CustomerAmountByCurrency = customerAmountByCurrency;
            }

            if (isApplicableToSupplier)
            {
                string errorMessage;
                Dictionary<string, decimal> supplierAmountByCurrency;
                settlementGenerationCustomSectionPayload.AvailableSupplierInvoices = GetAvailableSupplierInvoices(supplierInvoiceTypeId, currentPartnerId, supplierInvoices,supplierInvoiceItems, partnerIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out supplierAmountByCurrency, out  errorMessage);
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
                                IsLocked = invoiceDetail.Entity.LockDate.HasValue
                            });
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
                                IsLocked = invoiceDetail.Entity.LockDate.HasValue
                            });
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

        private List<InvoiceAvailableForSettlement> GetAvailableCustomerInvoices(Guid invoiceTypeId, string currentPartnerId, List<Vanrise.Invoice.Entities.Invoice> customerInvoices,List<InvoiceItem> customerInvoiceItems, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate, int normalPrecisionValue, bool getAllCurrencies, out Dictionary<string, decimal> customerAmountByCurrency, out string errorMessage)
        {

            List<InvoiceAvailableForSettlement> availableCustomerInvoices = null;
            errorMessage = null;
            customerAmountByCurrency = null;

            if (customerInvoices != null && customerInvoices.Count > 0)
            {
                var partnerCustomerInvoices = customerInvoices.FindAllRecords(x => x.PartnerId == currentPartnerId);
                if (partnerCustomerInvoices != null & partnerCustomerInvoices.Count() > 0)
                {
                    availableCustomerInvoices = new List<InvoiceAvailableForSettlement>();
                    foreach (var partnerCustomerInvoice in partnerCustomerInvoices)
                    {
                        var partnerCustomerInvoiceDetail = partnerCustomerInvoice.Details as CustomerInvoiceDetails;

                        var invoiceItems = customerInvoiceItems.FindAllRecords(x => x.InvoiceId == partnerCustomerInvoice.InvoiceId);
                        if(invoiceItems != null)
                        {
                            foreach(var invoiceItem in invoiceItems)
                            {
                                var invoiceItemDetail = invoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                                if (invoiceItemDetail != null)
                                {
                                    availableCustomerInvoices.Add(new InvoiceAvailableForSettlement
                                    {
                                        InvoiceId = partnerCustomerInvoice.InvoiceId,
                                        IsSelected = true,
                                        CurrencyId = invoiceItemDetail.CurrencyId
                                    });
                                }
                            }
                        }
                       
                    }
                    var selectedInvoiceIds = availableCustomerInvoices.MapRecords(x => new SelectedInvoiceItem { InvoiceId = x.InvoiceId, CurrencyId = x.CurrencyId }, x => x.IsSelected).ToList();

                    if (!TryGetCustomerAmountByCurrency(invoiceTypeId, customerInvoices,customerInvoiceItems, selectedInvoiceIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out customerAmountByCurrency, out  errorMessage))
                    {
                        return null;
                    }

                }
            }
            return availableCustomerInvoices;
        }

        private bool TryGetCustomerAmountByCurrency(Guid invoiceTypeId, List<Vanrise.Invoice.Entities.Invoice> customerInvoices,List<InvoiceItem> customerInvoiceItems, List<SelectedInvoiceItem> selectedInvoices, DateTime fromDate, DateTime toDate, int normalPrecisionValue, bool getAllCurrencies, out Dictionary<string, decimal> customerAmountByCurrency, out string errorMessage)
        {
            errorMessage = null;
            customerAmountByCurrency = null;
            if (customerInvoices != null && customerInvoices.Count > 0)
            {
                if (!ValidateInvoicesDates(customerInvoices, fromDate, toDate, out errorMessage))
                    return false;
                customerAmountByCurrency = LoadAndGetAmountByCurrency(invoiceTypeId, selectedInvoices, customerInvoiceItems, _groupingBySaleCurrencyItemSetName, normalPrecisionValue, getAllCurrencies);
            }
            return true;
        }

        private List<InvoiceAvailableForSettlement> GetAvailableSupplierInvoices(Guid invoiceTypeId, string currentPartnerId, List<Vanrise.Invoice.Entities.Invoice> supplierInvoices, List<InvoiceItem> supplierInvoiceItems, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate, int normalPrecisionValue, bool getAllCurrencies, out Dictionary<string, decimal> supplierAmountByCurrency, out string errorMessage)
        {

            List<InvoiceAvailableForSettlement> availableSupplierInvoices = null;
            supplierAmountByCurrency = null;
            errorMessage = null;

            if (supplierInvoices != null && supplierInvoices.Count > 0)
            {
                var partnerSupplierInvoices = supplierInvoices.FindAllRecords(x => x.PartnerId == currentPartnerId);
                if (partnerSupplierInvoices != null & partnerSupplierInvoices.Count() > 0)
                {

                    List<SelectedInvoiceItem> selectedInvoiceIds = new List<SelectedInvoiceItem>();
                    foreach (var partnerSupplierInvoice in partnerSupplierInvoices)
                    {
                        if (availableSupplierInvoices == null)
                            availableSupplierInvoices = new List<InvoiceAvailableForSettlement>();

                        var supplierInvoiceDetails = partnerSupplierInvoice.Details as SupplierInvoiceDetails;

                        if (supplierInvoiceDetails != null)
                        {
                            var invoiceItems = supplierInvoiceItems.FindAllRecords(x => x.InvoiceId == partnerSupplierInvoice.InvoiceId);
                            if (invoiceItems != null)
                            {
                                foreach (var invoiceItem in invoiceItems)
                                {

                                    var invoiceAvailableForSettlement = new InvoiceAvailableForSettlement
                                    {
                                        InvoiceId = partnerSupplierInvoice.InvoiceId,
                                        IsSelected = true,
                                    };

                                    var invoiceItemDetail = invoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                                    if (invoiceItemDetail != null)
                                    {
                                        invoiceAvailableForSettlement.CurrencyId = invoiceItemDetail.CurrencyId;
                                        availableSupplierInvoices.Add(invoiceAvailableForSettlement);
                                        if (invoiceAvailableForSettlement.IsSelected)
                                        {
                                            OriginalDataCurrrency originalDataCurrrency;
                                            if (supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.TryGetValue(invoiceItemDetail.CurrencyId, out originalDataCurrrency) && originalDataCurrrency.IncludeOriginalAmountInSettlement && originalDataCurrrency.OriginalAmount.HasValue)
                                            {
                                                if (supplierAmountByCurrency == null)
                                                    supplierAmountByCurrency = new Dictionary<string, decimal>();

                                                string currencySymbol = _currencyManager.GetCurrencySymbol(invoiceItemDetail.CurrencyId);
                                                currencySymbol.ThrowIfNull("currencySymbol", invoiceItemDetail.CurrencyId);
                                                supplierAmountByCurrency.Add(currencySymbol, Math.Round(originalDataCurrrency.OriginalAmount.Value, normalPrecisionValue));
                                            }
                                            else
                                            {
                                                selectedInvoiceIds.Add(new SelectedInvoiceItem { InvoiceId = invoiceAvailableForSettlement.InvoiceId, CurrencyId = invoiceAvailableForSettlement.CurrencyId });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Dictionary<string, decimal> amountsByCurrency = null;
                    if (!TryGetSupplierAmountByCurrency(invoiceTypeId, supplierInvoices, supplierInvoiceItems, selectedInvoiceIds, fromDate, toDate, normalPrecisionValue, getAllCurrencies, out amountsByCurrency, out  errorMessage))
                    {
                        return null;
                    }

                    if (amountsByCurrency != null)
                    {
                        foreach (var amount in amountsByCurrency)
                        {

                            if (supplierAmountByCurrency == null)
                                supplierAmountByCurrency = new Dictionary<string, decimal>();
                            if (!supplierAmountByCurrency.ContainsKey(amount.Key))
                            {
                                supplierAmountByCurrency.Add(amount.Key, amount.Value);
                            }
                        }
                    }
                }
            }

            return availableSupplierInvoices;
        }

        private bool TryGetSupplierAmountByCurrency(Guid invoiceTypeId, List<Vanrise.Invoice.Entities.Invoice> supplierInvoices,List<InvoiceItem> supplierInvoiceItems, List<SelectedInvoiceItem> selectedInvoices, DateTime fromDate, DateTime toDate, int normalPrecisionValue, bool getAllCurrencies, out Dictionary<string, decimal> supplierAmountByCurrency, out string errorMessage)
        {
            errorMessage = null;
            supplierAmountByCurrency = null;
            if (supplierInvoices != null && supplierInvoices.Count > 0)
            {
                if (!ValidateInvoicesDates(supplierInvoices, fromDate, toDate, out errorMessage))
                    return false;
                supplierAmountByCurrency = LoadAndGetAmountByCurrency(invoiceTypeId, selectedInvoices,supplierInvoiceItems, _groupingBySaleCurrencyItemSetName, normalPrecisionValue, getAllCurrencies);
            }
            return true;
        }

        private Dictionary<string, decimal> LoadAndGetAmountByCurrency(Guid invoiceTypeId, List<SelectedInvoiceItem> selectedInvoiceIds,List<InvoiceItem> invoiceItems, string itemSetName, int normalPrecisionValue, bool getAllCurrencies)
        {
            Dictionary<string, decimal> amountByCurrency = null;
            if (selectedInvoiceIds != null && selectedInvoiceIds.Count > 0)
            {
                if (invoiceItems != null)
                {
                    amountByCurrency = GetAmountByCurrency(invoiceTypeId, invoiceItems, selectedInvoiceIds, itemSetName, normalPrecisionValue, getAllCurrencies);
                }
            }
            return amountByCurrency;
        }

        private Dictionary<string, decimal> GetAmountByCurrency(Guid invoiceTypeId, IEnumerable<InvoiceItem> invoiceItems, List<SelectedInvoiceItem> selectedInvoiceIds, string itemSetName, int normalPrecisionValue, bool getAllCurrencies)
        {
            Dictionary<string, decimal> amountByCurrency = null;
            if (invoiceItems != null && invoiceItems.Count() > 0)
            {
                amountByCurrency = new Dictionary<string, decimal>();
                foreach (var invoiceItem in invoiceItems)
                {
                    var invoiceBySaleCurrencyItemDetail = invoiceItem.Details as InvoiceBySaleCurrencyItemDetails;
                    invoiceBySaleCurrencyItemDetail.ThrowIfNull("invoiceBySaleCurrencyItemDetails");

                    if (getAllCurrencies || selectedInvoiceIds.Any(x => x.InvoiceId == invoiceItem.InvoiceId && x.CurrencyId == invoiceBySaleCurrencyItemDetail.CurrencyId))
                    {
                        string currencySymbol = _currencyManager.GetCurrencySymbol(invoiceBySaleCurrencyItemDetail.CurrencyId);
                        currencySymbol.ThrowIfNull("currencySymbol", invoiceBySaleCurrencyItemDetail.CurrencyId);
                        decimal amount;
                        if (amountByCurrency.TryGetValue(currencySymbol, out amount))
                        {
                            amountByCurrency[currencySymbol] += Math.Round(invoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes, normalPrecisionValue);
                        }
                        else
                        {
                            amountByCurrency.Add(currencySymbol, Math.Round(invoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes, normalPrecisionValue));
                        }
                    }
                }
            }
            return amountByCurrency;
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

        #endregion

    }

    public class SelectedInvoiceItem
    {
        public long InvoiceId { get; set; }
        public int CurrencyId { get; set; }
    }
}
