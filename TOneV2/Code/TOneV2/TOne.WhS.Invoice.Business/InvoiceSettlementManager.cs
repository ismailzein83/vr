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

        public SettlementGenerationCustomSectionPayloadSummary TryLoadInvoicesAndGetAmountByCurrency(Guid invoiceTypeId, List<long> selectedCustomerInvoices, List<long> selectedSupplierInvoices,DateTime fromDate,DateTime toDate)
        {
            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<SettlementInvoiceSettings>("settlementInvoiceSettings", invoiceTypeId);

            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();
            var normalPrecisionValue = new GeneralSettingsManager().GetNormalPrecisionValue();

            SettlementGenerationCustomSectionPayloadSummary summary = new SettlementGenerationCustomSectionPayloadSummary();
            if (selectedCustomerInvoices != null && selectedCustomerInvoices.Count > 0)
            {
                var customerInvoices = invoiceManager.GetInvoices(selectedCustomerInvoices);

                string errorMessage = null;

                if (!ValidateInvoicesDates(customerInvoices, fromDate, toDate, out errorMessage))
                    summary.ErrorMessage = errorMessage;
              
                summary.CustomerAmountByCurrency = LoadAndGetAmountByCurrency(settlementInvoiceSettings.CustomerInvoiceTypeId, selectedCustomerInvoices, _groupingBySaleCurrencyItemSetName,normalPrecisionValue);
            }
            if (selectedSupplierInvoices != null && selectedSupplierInvoices.Count > 0)
            {
                var supplierInvoices = invoiceManager.GetInvoices(selectedSupplierInvoices);
                string errorMessage = null;
                if (!ValidateInvoicesDates(supplierInvoices, fromDate, toDate, out errorMessage))
                    summary.ErrorMessage = errorMessage;
                
                summary.SupplierAmountByCurrency = LoadAndGetAmountByCurrency(settlementInvoiceSettings.SupplierInvoiceTypeId, selectedSupplierInvoices, _groupingBySaleCurrencyItemSetName,normalPrecisionValue);

            }
            return summary;
        }

        public SettlementGenerationCustomSectionPayload EvaluatePartnerCustomPayload(long? currenctInvoiceId, string currentPartnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate)
        {
            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<SettlementInvoiceSettings>("settlementInvoiceSettings", invoiceTypeId);

            return EvaluateGenerationCustomPayload(settlementInvoiceSettings.CustomerInvoiceTypeId, settlementInvoiceSettings.SupplierInvoiceTypeId, new List<string> { currentPartnerId }, currentPartnerId, invoiceTypeId, fromDate, toDate, null, null, currenctInvoiceId);
        }

        public SettlementGenerationCustomSectionPayload EvaluateGenerationCustomPayload(Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId, IEnumerable<string> partnerIds, string currentPartnerId, Guid invoiceTypeId, DateTime fromDate, DateTime toDate, List<Vanrise.Invoice.Entities.Invoice> supplierInvoices, List<Vanrise.Invoice.Entities.Invoice> customerInvoices, long? currenctInvoiceId)
        {
            decimal? commission = null;
            WHSFinancialAccountManager whsFinancialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = whsFinancialAccountManager.GetFinancialAccount(int.Parse(currentPartnerId));
            financialAccount.ThrowIfNull("financialAccount", currentPartnerId);
            financialAccount.Settings.ThrowIfNull("financialAccount.Settings", currentPartnerId);

            WHSFinancialAccountDefinitionManager whsFinancialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var definition = whsFinancialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);

            List<FinancialAccountInvoiceSetting> financialAccountInvoiceSettings = financialAccount.Settings.FinancialAccountInvoiceSettings;
            if (financialAccountInvoiceSettings != null && financialAccountInvoiceSettings.Count > 0)
            {
                FinancialAccountInvoiceSetting financialAccountInvoiceSetting = financialAccountInvoiceSettings.FindRecord(itm => itm.InvoiceTypeId == invoiceTypeId);

                if (financialAccountInvoiceSetting != null && financialAccountInvoiceSetting.FinancialAccountCommission != null)
                {
                    commission = financialAccountInvoiceSetting.FinancialAccountCommission.Commission;
                }
            }
            var normalPrecisionValue = new GeneralSettingsManager().GetNormalPrecisionValue();


            var settlementGenerationCustomSectionPayload = new SettlementGenerationCustomSectionPayload
            {
                IsCustomerApplicable = definition.ExtendedSettings.IsApplicableToCustomer,
                IsSupplierApplicable = definition.ExtendedSettings.IsApplicableToSupplier,
                Commission = commission,
                Summary = new SettlementGenerationCustomSectionPayloadSummary()
            };

            if (definition.ExtendedSettings.IsApplicableToCustomer)
            {
                List<InvoiceAvailableForSettlement> availableCustomerInvoices;

                if (customerInvoices == null)
                    customerInvoices = LoadInvoices(customerInvoiceTypeId, partnerIds, fromDate, toDate);

                if (customerInvoices != null && customerInvoices.Count > 0)
                {
                    var partnerCustomerInvoices = customerInvoices.FindAllRecords(x => x.PartnerId == currentPartnerId);
                    if (partnerCustomerInvoices != null & partnerCustomerInvoices.Count() > 0)
                    {
                        string errorMessage = null;
                        if (!ValidateInvoicesDates(customerInvoices, fromDate, toDate, out errorMessage))
                            settlementGenerationCustomSectionPayload.Summary.ErrorMessage = errorMessage;

                        availableCustomerInvoices = partnerCustomerInvoices.MapRecords(x => new InvoiceAvailableForSettlement { InvoiceId = x.InvoiceId, IsSelected = !currenctInvoiceId.HasValue || x.SettlementInvoiceId.HasValue }).ToList();

                        settlementGenerationCustomSectionPayload.AvailableCustomerInvoices = availableCustomerInvoices;

                        var selectedInvoiceIds = availableCustomerInvoices.MapRecords(x => x.InvoiceId, x => x.IsSelected).ToList();

                        settlementGenerationCustomSectionPayload.Summary.CustomerAmountByCurrency = LoadAndGetAmountByCurrency(customerInvoiceTypeId, selectedInvoiceIds, _groupingBySaleCurrencyItemSetName, normalPrecisionValue);

                    }
                }

            }

            if (definition.ExtendedSettings.IsApplicableToSupplier)
            {
                List<InvoiceAvailableForSettlement> availableSupplierInvoices =null;

                if (supplierInvoices == null)
                    supplierInvoices = LoadInvoices(supplierInvoiceTypeId, partnerIds, fromDate, toDate);


                if (supplierInvoices != null && supplierInvoices.Count > 0)
                {
                    var partnerSupplierInvoices = supplierInvoices.FindAllRecords(x => x.PartnerId == currentPartnerId);
                    if (partnerSupplierInvoices != null & partnerSupplierInvoices.Count() > 0)
                    {
                        string errorMessage = null;
                        if (ValidateInvoicesDates(supplierInvoices, fromDate, toDate, out errorMessage))
                            settlementGenerationCustomSectionPayload.Summary.ErrorMessage = errorMessage;

                        List<long> selectedInvoiceIds = new List<long>();
                        Dictionary<string, decimal> supplierAmountByCurrency = null;
                        foreach (var partnerSupplierInvoice in partnerSupplierInvoices)
                        {
                            if (availableSupplierInvoices == null)
                                availableSupplierInvoices = new List<InvoiceAvailableForSettlement>();
                           
                            var invoiceAvailableForSettlement = new InvoiceAvailableForSettlement
                            { 
                                InvoiceId = partnerSupplierInvoice.InvoiceId,
                                IsSelected = !currenctInvoiceId.HasValue || partnerSupplierInvoice.SettlementInvoiceId.HasValue 
                            };

                            availableSupplierInvoices.Add(invoiceAvailableForSettlement);
                            if(invoiceAvailableForSettlement.IsSelected)
                            {
                                var supplierInvoiceDetails = partnerSupplierInvoice.Details as SupplierInvoiceDetails;
                                if (supplierInvoiceDetails != null && supplierInvoiceDetails.IncludeOriginalAmountInSettlement && supplierInvoiceDetails.OriginalAmount.HasValue)
                                {
                                    if (supplierAmountByCurrency == null)
                                        supplierAmountByCurrency = new Dictionary<string, decimal>();

                                     string currencySymbol = _currencyManager.GetCurrencySymbol(supplierInvoiceDetails.SupplierCurrencyId);
                                    currencySymbol.ThrowIfNull("currencySymbol", supplierInvoiceDetails.SupplierCurrencyId);
                                    supplierAmountByCurrency.Add(currencySymbol, Math.Round(supplierInvoiceDetails.OriginalAmount.Value, normalPrecisionValue));
                                }
                                else
                                {
                                    selectedInvoiceIds.Add(invoiceAvailableForSettlement.InvoiceId);
                                }
                            }
                        }

                        settlementGenerationCustomSectionPayload.AvailableSupplierInvoices = availableSupplierInvoices;
                        var amountsByCurrency = LoadAndGetAmountByCurrency(supplierInvoiceTypeId, selectedInvoiceIds, _groupingBySaleCurrencyItemSetName, normalPrecisionValue);
                        if(amountsByCurrency != null)
                        {
                            foreach(var amount in amountsByCurrency)
                            {
                                if (supplierAmountByCurrency == null)
                                    supplierAmountByCurrency = new  Dictionary<string,decimal>();
                                supplierAmountByCurrency.Add(amount.Key, amount.Value);
                            }
                        }
                        settlementGenerationCustomSectionPayload.Summary.SupplierAmountByCurrency = supplierAmountByCurrency;

                    }
                }
            }
            return settlementGenerationCustomSectionPayload;
        }

        public List<Vanrise.Invoice.Entities.Invoice> LoadInvoices(Guid invoiceTypeId, IEnumerable<string> partnerIds, DateTime fromDate, DateTime toDate)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().GetPartnerInvoicesByDate(invoiceTypeId, partnerIds, fromDate, toDate);
        }
        public Dictionary<string, decimal> LoadAndGetAmountByCurrency(Guid invoiceTypeId, List<long> selectedInvoiceIds, string itemSetName, int normalPrecisionValue)
        {
            Dictionary<string, decimal> amountByCurrency = null;
            if (selectedInvoiceIds != null && selectedInvoiceIds.Count > 0)
            {
                var invoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(invoiceTypeId, selectedInvoiceIds, new List<string> { itemSetName }, CompareOperator.Equal);
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
                            decimal amount;
                            if (amountByCurrency.TryGetValue(currencySymbol, out amount))
                            {
                                amountByCurrency[currencySymbol] += Math.Round(invoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes,normalPrecisionValue);
                            }else
                            {
                                amountByCurrency.Add(currencySymbol, Math.Round(invoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes, normalPrecisionValue));
                            }
                        }
                    }
                }
            }
            return amountByCurrency;

        }
        public SettlementGenerationCustomSectionInvoices LoadInvoicesDetails(Guid invoiceTypeId, string partnerId, List<long> customerInvoiceIds, List<long> supplierInvoiceIds)
        {
            VRTimeZoneManager vrTimeZoneManager = new Vanrise.Common.Business.VRTimeZoneManager();
            SettlementGenerationCustomSectionInvoices settlementGenerationCustomSectionInvoices = new SettlementGenerationCustomSectionInvoices();

            var invoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = invoiceTypeExtendedSettings.CastWithValidate<SettlementInvoiceSettings>("settlementInvoiceSettings", invoiceTypeId);
            Vanrise.Invoice.Business.InvoiceManager invoiceManager = new Vanrise.Invoice.Business.InvoiceManager();

            settlementGenerationCustomSectionInvoices.PartnerName = invoiceTypeExtendedSettings.GetPartnerManager().GetFullPartnerName(new PartnerNameManagerContext
            {
                PartnerId = partnerId
            });
            if (customerInvoiceIds != null && customerInvoiceIds.Count > 0)
            {
                var customerInvoiceDetails = invoiceManager.GetInvoicesDetails(settlementInvoiceSettings.CustomerInvoiceTypeId, partnerId, customerInvoiceIds);

                if (customerInvoiceDetails != null && customerInvoiceDetails.Count() > 0)
                {
                    settlementGenerationCustomSectionInvoices.CustomerInvoiceDetails = new List<CustomerInvoiceDetail>();

                    foreach (var customerInvoiceDetail in customerInvoiceDetails)
                    {
                        var customerDetail = customerInvoiceDetail.Entity.Details as CustomerInvoiceDetails;
                        customerDetail.ThrowIfNull("customerDetail");

                        settlementGenerationCustomSectionInvoices.CustomerInvoiceDetails.Add(new CustomerInvoiceDetail
                        {
                            InvoiceId = customerInvoiceDetail.Entity.InvoiceId,
                            TotalNumberOfCalls = customerDetail.TotalNumberOfCalls,
                            TimeZoneId = customerDetail.TimeZoneId,
                            TotalAmountAfterCommission = customerDetail.TotalAmountAfterCommission,
                            SaleCurrency = _currencyManager.GetCurrencySymbol(customerDetail.SaleCurrencyId),
                            DueDate = customerInvoiceDetail.Entity.DueDate,
                            Commission = customerDetail.Commission,
                            FromDate = customerInvoiceDetail.Entity.FromDate,
                            IssueDate = customerInvoiceDetail.Entity.IssueDate,
                            Duration = customerDetail.Duration,
                            Offset = customerDetail.Offset,
                            ToDate = customerInvoiceDetail.Entity.ToDate,
                            SerialNumber = customerInvoiceDetail.Entity.SerialNumber,
                            TimeZoneDescription = customerDetail.TimeZoneId.HasValue ? vrTimeZoneManager.GetVRTimeZoneName(customerDetail.TimeZoneId.Value) : null
                        });
                    }
                }

            }

            if (supplierInvoiceIds != null && supplierInvoiceIds.Count > 0)
            {
                var supplierInvoiceDetails = invoiceManager.GetInvoicesDetails(settlementInvoiceSettings.SupplierInvoiceTypeId, partnerId, supplierInvoiceIds);

                if (supplierInvoiceDetails != null && supplierInvoiceDetails.Count() > 0)
                {
                    settlementGenerationCustomSectionInvoices.SupplierInvoiceDetails = new List<SupplierInvoiceDetail>();

                    foreach (var supplierInvoiceDetail in supplierInvoiceDetails)
                    {

                        var supplierDetail = supplierInvoiceDetail.Entity.Details as SupplierInvoiceDetails;
                        supplierDetail.ThrowIfNull("supplierDetail");

                        settlementGenerationCustomSectionInvoices.SupplierInvoiceDetails.Add(new SupplierInvoiceDetail
                        {
                            InvoiceId = supplierInvoiceDetail.Entity.InvoiceId,
                            TotalNumberOfCalls = supplierDetail.TotalNumberOfCalls,
                            TimeZoneId = supplierDetail.TimeZoneId,
                            TotalAmountAfterCommission = supplierDetail.IncludeOriginalAmountInSettlement && supplierDetail.OriginalAmount.HasValue?supplierDetail.OriginalAmount.Value: supplierDetail.TotalAmountAfterCommission,
                            SupplierCurrency = _currencyManager.GetCurrencySymbol(supplierDetail.SupplierCurrencyId),
                            DueDate = supplierInvoiceDetail.Entity.DueDate,
                            Commission = supplierDetail.Commission,
                            FromDate = supplierInvoiceDetail.Entity.FromDate,
                            IssueDate = supplierInvoiceDetail.Entity.IssueDate,
                            Duration = supplierDetail.Duration,
                            Offset = supplierDetail.Offset,
                            ToDate = supplierInvoiceDetail.Entity.ToDate,
                            SerialNumber = supplierInvoiceDetail.Entity.SerialNumber,
                            TimeZoneDescription = supplierDetail.TimeZoneId.HasValue ? vrTimeZoneManager.GetVRTimeZoneName(supplierDetail.TimeZoneId.Value) : null
                        });
                    }
                }


            }

            return settlementGenerationCustomSectionInvoices;
        }

        bool ValidateInvoicesDates(IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, DateTime fromDate, DateTime toDate, out string errorMessage)
        {
            errorMessage = null;

            if (invoices.Min(x => x.FromDate) < fromDate || invoices.Max(x => x.ToDate) > toDate)
            {
                errorMessage = "Unable to generate settlement at this period.";
                return false;
            }
            return true;

        }
    }
}
