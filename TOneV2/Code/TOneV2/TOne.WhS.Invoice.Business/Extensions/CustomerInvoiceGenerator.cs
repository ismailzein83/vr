﻿using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using TOne.WhS.Deal.MainExtensions;
using TOne.WhS.Invoice.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.Security.Business;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class CustomerInvoiceGenerator : InvoiceGenerator
    {
        #region Local Variables
        PartnerManager _partnerManager = new PartnerManager();
        WHSFinancialAccountManager _financialAccountManager = new WHSFinancialAccountManager();
        TOneModuleManager _tOneModuleManager = new TOneModuleManager();
        InvoiceGenerationManager _invoiceGenerationManager = new InvoiceGenerationManager();
        CustomerRecurringChargeManager _customerRecurringChargeManager = new CustomerRecurringChargeManager();
        CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();
        #endregion

        #region Public Methods
        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            var financialAccount = _financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            int financialAccountId = financialAccount.FinancialAccountId;
            string dimensionName = "SaleFinancialAccount";

            ResolvedInvoicePayloadObject resolvedPayload = _invoiceGenerationManager.GetDatesWithTimeZone<CustomerGenerationCustomSectionPayload>(context.CustomSectionPayload, financialAccountId, context.FromDate, context.ToDate);
            int currencyId = _financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);

            IEnumerable<VRTaxItemDetail> taxItemDetails = _financialAccountManager.GetFinancialAccountTaxItemDetails(financialAccount);
            List<RecurringChargeItem> evaluatedCustomerRecurringCharges = _customerRecurringChargeManager.GetEvaluatedRecurringCharges(financialAccount.FinancialAccountId, context.FromDate, context.ToDate, context.IssueDate);

            bool isVoiceEnabled = _tOneModuleManager.IsVoiceModuleEnabled();
            bool canGenerateVoiceInvoice = false;
            List<CustomerInvoiceItemDetails> voiceItemSetNames = null;
            List<CustomerInvoiceDealItemDetails> dealItemSetNames = null;
            List<CustomerInvoiceBySaleCurrencyItemDetails> invoiceBySaleCurrency = null;
            if (isVoiceEnabled)
            {
                canGenerateVoiceInvoice = CheckUnpricedCDRs(context, financialAccount);
                if (canGenerateVoiceInvoice)
                {
                    List<int> carrierAccountIds = new List<int>();
                    if (financialAccount.CarrierProfileId.HasValue)
                    {
                        CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                        var carrierAccountsByCarrierProfileId = carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
                        carrierAccountsByCarrierProfileId.ThrowIfNull("carrierAccountsByCarrierProfileId");
                        foreach (var carrierAccount in carrierAccountsByCarrierProfileId)
                        {
                            carrierAccountIds.Add(carrierAccount.CarrierAccountId);
                        }
                    }
                    else
                    {
                        carrierAccountIds.Add(financialAccount.CarrierAccountId.Value);
                    }

                    List<string> listMeasures = new List<string> { "SaleNetNotNULL", "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
                    List<string> listDimensions = new List<string> { "SaleZone", "Customer", "SaleCurrency", "SaleRate", "SaleRateType", "Supplier", "Country", "SupplierZone", "SaleDealZoneGroupNb", "SaleDealTierNb", "SaleDeal", "SaleDealRateTierNb" };
                    voiceItemSetNames = _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(listDimensions, listMeasures, dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, currencyId, resolvedPayload.OffsetValue, (analyticRecord) =>
                    {
                        return VoiceItemSetNamesMapper(analyticRecord, currencyId, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                    });
                    dealItemSetNames = GetDealItemSetNames(carrierAccountIds, context.FromDate, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.OffsetValue, dimensionName, financialAccountId, context.IssueDate, currencyId, taxItemDetails);
                    invoiceBySaleCurrency = loadVoiceCurrencyItemSet(dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                    AddDealToCustomerCurrency(() => { if (invoiceBySaleCurrency == null) invoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>(); return invoiceBySaleCurrency; }, dealItemSetNames);
                }
            }

            bool isSMSEnabled = _tOneModuleManager.IsSMSModuleEnabled();
            List<CustomerSMSInvoiceItemDetails> smsItemSetNames = null;
            if (isSMSEnabled)
            {
                List<string> listMeasures = new List<string> { "NumberOfSMS", "BillingPeriodFrom", "BillingPeriodTo", "SaleNetNotNULL", "SaleNet_OrigCurr" };
                List<string> listDimensions = new List<string> { "DestinationMobileNetwork", "DestinationMobileCountry", "Customer", "SaleCurrency", "SaleRate", "Supplier" };

                smsItemSetNames = _invoiceGenerationManager.GetInvoiceSMSMappedRecords(listDimensions, listMeasures, dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, currencyId, resolvedPayload.OffsetValue, (analyticRecord) =>
                {
                    return SMSItemSetNamesMapper(analyticRecord, currencyId, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                });

                var sMSInvoiceBySaleCurrencies = loadSMSCurrencyItemSet(dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                TryMergeByCurrencyItemSets(() => { if (invoiceBySaleCurrency == null) invoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>(); return invoiceBySaleCurrency; }, sMSInvoiceBySaleCurrencies);
            }

            if (((smsItemSetNames == null || smsItemSetNames.Count == 0) && ((voiceItemSetNames == null || voiceItemSetNames.Count == 0))) && (evaluatedCustomerRecurringCharges == null || evaluatedCustomerRecurringCharges.Count == 0) && (dealItemSetNames == null || dealItemSetNames.Count == 0))
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            decimal? minAmount = _partnerManager.GetPartnerMinAmount(context.InvoiceTypeId, context.PartnerId);

            if (resolvedPayload.Adjustment.HasValue)
            {
                if (invoiceBySaleCurrency == null)
                    invoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>();
                AddAdjustmentToCustomerCurrency(invoiceBySaleCurrency, currencyId, context.FromDate, context.ToDate, resolvedPayload.Adjustment.Value);
            }

            #region BuildCustomerInvoiceDetails
            CustomerInvoiceDetails customerInvoiceDetails = BuildCustomerInvoiceDetails(voiceItemSetNames, smsItemSetNames, financialAccount.CarrierProfileId.HasValue ? "Profile" : "Account", context.FromDate, context.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, canGenerateVoiceInvoice, dealItemSetNames, currencyId);
            if (customerInvoiceDetails != null)
            {
                customerInvoiceDetails.TimeZoneId = resolvedPayload.TimeZoneId;
                customerInvoiceDetails.TotalAmount = customerInvoiceDetails.SaleAmount;
                customerInvoiceDetails.TotalAmountAfterCommission = customerInvoiceDetails.AmountAfterCommission;
                customerInvoiceDetails.TotalSMSAmountAfterCommission = customerInvoiceDetails.SMSAmountAfterCommission;
                customerInvoiceDetails.TotalOriginalAmountAfterCommission = customerInvoiceDetails.OriginalAmountAfterCommission;
                customerInvoiceDetails.TotalSMSOriginalAmountAfterCommission = customerInvoiceDetails.SMSOriginalAmountAfterCommission;

                customerInvoiceDetails.Commission = resolvedPayload.Commission;
                customerInvoiceDetails.Adjustment = resolvedPayload.Adjustment;
                customerInvoiceDetails.CommissionType = resolvedPayload.CommissionType;
                customerInvoiceDetails.Offset = resolvedPayload.Offset;

                customerInvoiceDetails.TotalAmountAfterCommission = customerInvoiceDetails.AmountAfterCommission;
                customerInvoiceDetails.TotalOriginalAmountAfterCommission = customerInvoiceDetails.OriginalAmountAfterCommission;

                customerInvoiceDetails.TotalVoiceAmountBeforeTax = customerInvoiceDetails.TotalAmountAfterCommission;
                customerInvoiceDetails.TotalSMSAmountBeforeTax = customerInvoiceDetails.TotalSMSAmountAfterCommission;
                customerInvoiceDetails.TotalDealAmountAfterTax = customerInvoiceDetails.TotalDealAmount;
                customerInvoiceDetails.TotalInvoiceAmountBeforeTax = customerInvoiceDetails.TotalSMSAmountAfterCommission + customerInvoiceDetails.TotalAmountAfterCommission + customerInvoiceDetails.TotalDealAmount;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        customerInvoiceDetails.TotalAmountAfterCommission += ((customerInvoiceDetails.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalSMSAmountAfterCommission += ((customerInvoiceDetails.SMSAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalOriginalAmountAfterCommission += ((customerInvoiceDetails.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalSMSOriginalAmountAfterCommission += ((customerInvoiceDetails.SMSOriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalAmount += ((customerInvoiceDetails.SaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalSMSAmount += ((customerInvoiceDetails.TotalSMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceDetails.TotalDealAmountAfterTax += ((customerInvoiceDetails.TotalDealAmount * Convert.ToDecimal(tax.Value)) / 100);

                        if (evaluatedCustomerRecurringCharges != null)
                        {
                            foreach (var item in evaluatedCustomerRecurringCharges)
                            {
                                item.AmountAfterTaxes += ((item.Amount * Convert.ToDecimal(tax.Value)) / 100);
                                item.VAT = tax.IsVAT ? tax.Value : 0;
                            }
                        }
                    }
                    context.ActionAfterGenerateInvoice = (invoice) =>
                    {
                        CustomerBillingRecurringChargeManager customerBillingRecurringChargeManager = new CustomerBillingRecurringChargeManager();
                        var userId = SecurityContext.Current.GetLoggedInUserId();
                        if (evaluatedCustomerRecurringCharges != null && evaluatedCustomerRecurringCharges.Count > 0)
                        {
                            foreach (var item in evaluatedCustomerRecurringCharges)
                            {
                                customerBillingRecurringChargeManager.AddCustomerBillingRecurringCharge(new CustomerBillingRecurringCharge
                                {
                                    InvoiceId = invoice.InvoiceId,
                                    Amount = item.AmountAfterTaxes,
                                    RecurringChargeId = item.RecurringChargeId,
                                    VAT = item.VAT,
                                    From = item.From,
                                    To = item.To,
                                    CreatedBy = userId,
                                    FinancialAccountId = financialAccount.FinancialAccountId,
                                    CurrencyId = item.CurrencyId,
                                });
                            }
                        }
                        return true;
                    };
                }

                AddRecurringChargeToCustomerCurrency(() => { if (invoiceBySaleCurrency == null) invoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>(); return invoiceBySaleCurrency; }, evaluatedCustomerRecurringCharges);

                CurrencyManager currencyManager = new CurrencyManager();
                var systemCurrency = currencyManager.GetSystemCurrency();
                systemCurrency.ThrowIfNull("systemCurrency");
                decimal totalAmountAfterCommissionInSystemCurrency = _currencyExchangeRateManager.ConvertValueToCurrency(customerInvoiceDetails.TotalAmountAfterCommission, currencyId, systemCurrency.CurrencyId, context.IssueDate);
                decimal totalSMSAmountAfterCommissionInSystemCurrency = _currencyExchangeRateManager.ConvertValueToCurrency(customerInvoiceDetails.TotalSMSAmountAfterCommission, currencyId, systemCurrency.CurrencyId, context.IssueDate);
                decimal totalReccurringChargesInSystemCurrency = 0;
                decimal totalDealAmountInSystemCurrency = 0;
                if (dealItemSetNames != null && dealItemSetNames.Count > 0)
                {
                    foreach (var dealItemSetName in dealItemSetNames)
                    {
                        totalDealAmountInSystemCurrency += _currencyExchangeRateManager.ConvertValueToCurrency(dealItemSetName.Amount, currencyId, systemCurrency.CurrencyId, context.IssueDate);
                    }
                }
                if (evaluatedCustomerRecurringCharges != null)
                {
                    foreach (var item in evaluatedCustomerRecurringCharges)
                    {
                        totalReccurringChargesInSystemCurrency += _currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, systemCurrency.CurrencyId, context.IssueDate);
                    }
                }
                var totalAmountInSystemCurrency = totalReccurringChargesInSystemCurrency + totalAmountAfterCommissionInSystemCurrency + totalSMSAmountAfterCommissionInSystemCurrency + totalDealAmountInSystemCurrency;

                if ((minAmount.HasValue && totalAmountInSystemCurrency >= minAmount.Value) || (!minAmount.HasValue && totalAmountInSystemCurrency != 0))
                {
                    var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
                    definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
                    definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
                    var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == context.InvoiceTypeId);
                    financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");

                    ConfigManager configManager = new ConfigManager();
                    InvoiceTypeSetting settings = configManager.GetInvoiceTypeSettingsById(context.InvoiceTypeId);
                    if (settings != null)
                    {
                        context.NeedApproval = settings.NeedApproval;
                    }

                    decimal totalReccurringChargesAfterTaxInAccountCurrency = 0;
                    decimal totalReccurringChargesInAccountCurrency = 0;
                    if (evaluatedCustomerRecurringCharges != null && evaluatedCustomerRecurringCharges.Count > 0)
                    {
                        foreach (var item in evaluatedCustomerRecurringCharges)
                        {
                            totalReccurringChargesAfterTaxInAccountCurrency += _currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, currencyId, context.IssueDate);
                            totalReccurringChargesInAccountCurrency += _currencyExchangeRateManager.ConvertValueToCurrency(item.Amount, item.CurrencyId, currencyId, context.IssueDate);
                        }
                    }
                    customerInvoiceDetails.TotalReccurringChargesAfterTax = totalReccurringChargesAfterTaxInAccountCurrency;
                    customerInvoiceDetails.TotalReccurringCharges = totalReccurringChargesInAccountCurrency;
                    customerInvoiceDetails.TotalInvoiceAmountBeforeTax += customerInvoiceDetails.TotalReccurringCharges;

                    customerInvoiceDetails.NoDeals = customerInvoiceDetails.TotalDealAmount == 0;
                    customerInvoiceDetails.NoSMS = customerInvoiceDetails.TotalSMSAmountAfterCommission == 0;
                    customerInvoiceDetails.NoVoice = customerInvoiceDetails.TotalAmountAfterCommission == 0;
                    customerInvoiceDetails.NoRecurringCharges = customerInvoiceDetails.TotalReccurringChargesAfterTax == 0;

                    customerInvoiceDetails.TotalInvoiceAmount = customerInvoiceDetails.TotalAmountAfterCommission + customerInvoiceDetails.TotalReccurringChargesAfterTax + customerInvoiceDetails.TotalSMSAmountAfterCommission + customerInvoiceDetails.TotalDealAmountAfterTax;
                    if (customerInvoiceDetails.Adjustment.HasValue)
                        customerInvoiceDetails.TotalInvoiceAmount += customerInvoiceDetails.Adjustment.Value;

                    if (!financialAccountInvoiceType.IgnoreFromBalance)
                    {
                        SetInvoiceBillingTransactions(context, customerInvoiceDetails, financialAccount, resolvedPayload.FromDate, resolvedPayload.ToDateForBillingTransaction, currencyId);
                    }

                    if (taxItemDetails != null && taxItemDetails.Count() > 0)
                    {
                        foreach (var tax in taxItemDetails)
                        {
                            tax.TaxAmount = ((customerInvoiceDetails.TotalInvoiceAmountBeforeTax * Convert.ToDecimal(tax.Value)) / 100);
                        }
                    }
                    List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(voiceItemSetNames, smsItemSetNames, taxItemDetails, invoiceBySaleCurrency, evaluatedCustomerRecurringCharges, dealItemSetNames, canGenerateVoiceInvoice);

                    context.Invoice = new GeneratedInvoice
                    {
                        InvoiceDetails = customerInvoiceDetails,
                        InvoiceItemSets = generatedInvoiceItemSets,
                    };
                }
                else
                {
                    context.ErrorMessage = "Cannot generate invoice with amount less than threshold.";
                    context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                    return;
                }
            }
            else
            {
                context.ErrorMessage = "No billing data available.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }
            #endregion

        }
        #endregion

        #region Private Methods
        private void AddAdjustmentToCustomerCurrency(List<CustomerInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencyItemDetails, int currency, DateTime fromDate, DateTime toDate, decimal value)
        {
            string month = fromDate.Year == toDate.Year && fromDate.Month == toDate.Month ? fromDate.ToString("MMMM - yyyy") : string.Format("{0} / {1}", fromDate.ToString("MMMM - yyyy"), toDate.ToString("MMMM - yyyy"));
            var customerInvoiceBySaleCurrencyItemDetail = customerInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == currency && x.Month == month);
            if (customerInvoiceBySaleCurrencyItemDetail != null)
            {
                customerInvoiceBySaleCurrencyItemDetail.Amount += value;
                customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += value;
                customerInvoiceBySaleCurrencyItemDetail.AdjustmentAmount += value;
                customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += value;
                customerInvoiceBySaleCurrencyItemDetail.TotalFullAmount += value;
            }
            else
            {
                customerInvoiceBySaleCurrencyItemDetails.Add(new CustomerInvoiceBySaleCurrencyItemDetails
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    AmountAfterCommission = value,
                    AmountAfterCommissionWithTaxes = value,
                    NumberOfCalls = 0,
                    TotalFullAmount = value,
                    Duration = 0,
                    CurrencyId = currency,
                    Amount = value,
                    AdjustmentAmount = value,
                    Month = month,
                });
            }
        }

        private void AddDealToCustomerCurrency(Func<List<CustomerInvoiceBySaleCurrencyItemDetails>> getCustomerInvoiceBySaleCurrencyItemDetails, List<CustomerInvoiceDealItemDetails> dealItemSetNames)
        {
            if (dealItemSetNames != null && dealItemSetNames.Count > 0)
            {
                List<CustomerInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencyItemDetails = getCustomerInvoiceBySaleCurrencyItemDetails();

                foreach (var item in dealItemSetNames)
                {
                    var month = item.ToDate.ToString("MMMM - yyyy");
                    var customerInvoiceBySaleCurrencyItemDetail = customerInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == month);
                    if (customerInvoiceBySaleCurrencyItemDetail != null)
                    {
                        customerInvoiceBySaleCurrencyItemDetail.Amount += item.OriginalAmount;
                        customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.OriginalAmountAfterTax;
                        customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.OriginalAmountAfterTax;
                        customerInvoiceBySaleCurrencyItemDetail.TotalDealAmount += item.OriginalAmountAfterTax;
                        customerInvoiceBySaleCurrencyItemDetail.TotalFullAmount += item.OriginalAmountAfterTax;
                    }
                    else
                    {
                        customerInvoiceBySaleCurrencyItemDetails.Add(new CustomerInvoiceBySaleCurrencyItemDetails
                        {
                            FromDate = item.FromDate,
                            ToDate = item.ToDate,
                            AmountAfterCommission = item.OriginalAmount,
                            AmountAfterCommissionWithTaxes = item.OriginalAmountAfterTax,
                            NumberOfCalls = 0,
                            TotalFullAmount = item.OriginalAmountAfterTax,
                            Duration = 0,
                            CurrencyId = item.CurrencyId,
                            Amount = item.OriginalAmount,
                            TotalDealAmount = item.OriginalAmountAfterTax,
                            TotalTrafficAmount = 0,
                            Month = month,
                        });
                    }
                }
            }
        }
        private void AddRecurringChargeToCustomerCurrency(Func<List<CustomerInvoiceBySaleCurrencyItemDetails>> getCustomerInvoiceBySaleCurrencyItemDetails, List<RecurringChargeItem> recurringChargeItems)
        {
            if (recurringChargeItems != null && recurringChargeItems.Count > 0)
            {
                List<CustomerInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencyItemDetails = getCustomerInvoiceBySaleCurrencyItemDetails();

                foreach (var item in recurringChargeItems)
                {
                    var customerInvoiceBySaleCurrencyItemDetail = customerInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
                    if (customerInvoiceBySaleCurrencyItemDetail != null)
                    {
                        customerInvoiceBySaleCurrencyItemDetail.Amount += item.Amount;
                        customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.Amount;
                        customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.AmountAfterTaxes;
                        customerInvoiceBySaleCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
                        customerInvoiceBySaleCurrencyItemDetail.TotalFullAmount += item.AmountAfterTaxes;
                    }
                    else
                    {
                        customerInvoiceBySaleCurrencyItemDetails.Add(new CustomerInvoiceBySaleCurrencyItemDetails
                        {
                            FromDate = item.From,
                            ToDate = item.To,
                            AmountAfterCommission = item.Amount,
                            AmountAfterCommissionWithTaxes = item.AmountAfterTaxes,
                            NumberOfCalls = 0,
                            TotalFullAmount = item.AmountAfterTaxes,
                            Duration = 0,
                            CurrencyId = item.CurrencyId,
                            Amount = item.Amount,
                            TotalRecurringChargeAmount = item.AmountAfterTaxes,
                            TotalTrafficAmount = 0,
                            Month = item.RecurringChargeMonth
                        });
                    }
                }
            }
        }
        private List<CustomerInvoiceBySaleCurrencyItemDetails> loadVoiceCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            List<string> listMeasures = new List<string> { "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
            List<string> listDimensions = new List<string> { "SaleCurrency", "MonthQueryTimeShift" };
            return _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, null, offsetValue, (analyticRecord) =>
            {
                return CurrencyItemSetNameMapper(analyticRecord, commission, taxItemDetails, true);
            });
        }
        private List<CustomerInvoiceBySaleCurrencyItemDetails> loadSMSCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            List<string> listMeasures = new List<string> { "NumberOfSMS", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
            List<string> listDimensions = new List<string> { "SaleCurrency", "MonthQueryTimeShift" };
            return _invoiceGenerationManager.GetInvoiceSMSMappedRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, null, offsetValue, (analyticRecord) =>
            {
                return CurrencyItemSetNameMapper(analyticRecord, commission, taxItemDetails, false);
            });
        }
        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, CustomerInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount, DateTime fromDate, DateTime toDate, int currencyId)
        {
            var financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var balanceAccountTypeId = financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId);
            if (balanceAccountTypeId.HasValue)
            {
                Vanrise.Invoice.Entities.InvoiceType invoiceType = new Vanrise.Invoice.Business.InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
                CustomerInvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<CustomerInvoiceSettings>("invoiceType.Settings.ExtendedSettings");

                var billingTransaction = new GeneratedInvoiceBillingTransaction()
                {
                    AccountTypeId = balanceAccountTypeId.Value,
                    AccountId = context.PartnerId,
                    TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
                    Amount = invoiceDetails.TotalInvoiceAmount,
                    CurrencyId = currencyId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
                billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();
                invoiceSettings.UsageTransactionTypeIds.ThrowIfNull("invoiceSettings.UsageTransactionTypeIds");
                foreach (Guid usageTransactionTypeId in invoiceSettings.UsageTransactionTypeIds)
                {
                    billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                    {
                        TransactionTypeId = usageTransactionTypeId
                    });
                }
                context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
            }

        }
        private CustomerInvoiceDetails BuildCustomerInvoiceDetails(List<CustomerInvoiceItemDetails> voiceItemSetNames, List<CustomerSMSInvoiceItemDetails> smsItemSetNames, string partnerType, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, bool canGenerateVoiceInvoice, List<CustomerInvoiceDealItemDetails> dealItemSetNames, int currencyId)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            CustomerInvoiceDetails customerInvoiceDetails = null;
            if (partnerType != null)
            {
                customerInvoiceDetails = new CustomerInvoiceDetails()
                {
                    PartnerType = partnerType,
                    SaleCurrencyId = currencyId,
                    SaleCurrency = currencyManager.GetCurrencySymbol(currencyId)
                };
                if (voiceItemSetNames != null && canGenerateVoiceInvoice)
                {
                    foreach (var invoiceBillingRecord in voiceItemSetNames)
                    {
                        customerInvoiceDetails.Duration += invoiceBillingRecord.Duration;
                        customerInvoiceDetails.SaleAmount += invoiceBillingRecord.SaleAmount;
                        customerInvoiceDetails.OriginalSaleAmount += invoiceBillingRecord.OriginalSaleAmount;
                        customerInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.NumberOfCalls;
                        customerInvoiceDetails.OriginalSaleCurrencyId = invoiceBillingRecord.OriginalSaleCurrencyId;
                        customerInvoiceDetails.CountryId = invoiceBillingRecord.CountryId;
                        customerInvoiceDetails.SupplierId = invoiceBillingRecord.SupplierId;
                        customerInvoiceDetails.SupplierZoneId = invoiceBillingRecord.SupplierZoneId;
                        customerInvoiceDetails.AmountAfterCommission += invoiceBillingRecord.AmountAfterCommission;
                        customerInvoiceDetails.OriginalAmountAfterCommission += invoiceBillingRecord.OriginalAmountAfterCommission;
                    }
                    if (voiceItemSetNames.Count > 0)
                        customerInvoiceDetails.OriginalSaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.OriginalSaleCurrencyId);
                }

                if (smsItemSetNames != null)
                {
                    foreach (var smsInvoiceBillingRecord in smsItemSetNames)
                    {
                        customerInvoiceDetails.TotalNumberOfSMS += smsInvoiceBillingRecord.NumberOfSMS;
                        customerInvoiceDetails.OriginalSaleCurrencyId = smsInvoiceBillingRecord.OriginalSaleCurrencyId;
                        customerInvoiceDetails.CountryId = smsInvoiceBillingRecord.MobileCountryId;
                        customerInvoiceDetails.SupplierId = smsInvoiceBillingRecord.SupplierId;
                        customerInvoiceDetails.TotalSMSAmount = smsInvoiceBillingRecord.SaleAmount;
                        customerInvoiceDetails.SMSAmountAfterCommission += smsInvoiceBillingRecord.AmountAfterCommission;
                        customerInvoiceDetails.SMSOriginalAmountAfterCommission += smsInvoiceBillingRecord.OriginalAmountAfterCommission;
                    }
                    if (smsItemSetNames.Count > 0)
                        customerInvoiceDetails.OriginalSaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.OriginalSaleCurrencyId);
                }

                if (dealItemSetNames != null && dealItemSetNames.Count > 0)
                {
                    foreach (var dealBillingRecord in dealItemSetNames)
                    {
                        customerInvoiceDetails.TotalDealAmount += dealBillingRecord.Amount;
                    }
                }

                if (commissionType.HasValue)
                {
                    switch (commissionType.Value)
                    {
                        case CommissionType.Display:
                            customerInvoiceDetails.DisplayComission = true;
                            break;
                    }
                }
                else
                {
                    customerInvoiceDetails.DisplayComission = false;
                }
            }
            return customerInvoiceDetails;
        }
        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(List<CustomerInvoiceItemDetails> voiceItemSetNames, List<CustomerSMSInvoiceItemDetails> smsItemSetNames, IEnumerable<VRTaxItemDetail> taxItemDetails, List<CustomerInvoiceBySaleCurrencyItemDetails> customerInvoicesBySaleCurrency, List<RecurringChargeItem> customerRecurringCharges, List<CustomerInvoiceDealItemDetails> dealItemSetNames, bool canGenerateVoiceInvoice)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupingByCurrency", generatedInvoiceItemSets, customerInvoicesBySaleCurrency);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupedBySaleZone", generatedInvoiceItemSets, voiceItemSetNames);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("Taxes", generatedInvoiceItemSets, taxItemDetails);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupedByDestinationMobileNetwork", generatedInvoiceItemSets, smsItemSetNames);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupedBySaleDeal", generatedInvoiceItemSets, dealItemSetNames);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("RecurringCharge", generatedInvoiceItemSets, customerRecurringCharges);
            return generatedInvoiceItemSets;
        }
        private CustomerSMSInvoiceItemDetails SMSItemSetNamesMapper(AnalyticRecord analyticRecord, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {

            var saleNetValue = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleNetNotNULL");
            if (saleNetValue != 0)
            {
                CustomerSMSInvoiceItemDetails invoiceBillingRecord = new CustomerSMSInvoiceItemDetails
                {
                    CustomerId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 2),
                    SaleCurrencyId = currencyId,
                    OriginalSaleCurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 3),
                    SaleRate = _invoiceGenerationManager.GetDimensionValue<Decimal>(analyticRecord, 4),
                    CustomerMobileNetworkId = _invoiceGenerationManager.GetDimensionValue<long>(analyticRecord, 0),
                    MobileCountryId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 1),
                    SaleAmount = saleNetValue,
                    NumberOfSMS = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfSMS"),
                    OriginalSaleAmount = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleNet_OrigCurr"),
                    SupplierId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 5),
                };
                var billingPeriodFromDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodFrom");
                var billingPeriodToDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodTo");
                if (offsetValue.HasValue)
                {
                    billingPeriodFromDate = billingPeriodFromDate.Add(offsetValue.Value);
                    billingPeriodToDate = billingPeriodToDate.Add(offsetValue.Value);
                }
                invoiceBillingRecord.FromDate = billingPeriodFromDate;
                invoiceBillingRecord.ToDate = billingPeriodToDate;

                if (commission.HasValue)
                {
                    if (commissionType.HasValue && commissionType.Value == CommissionType.DoNotDisplay)
                    {
                        invoiceBillingRecord.SaleRate = invoiceBillingRecord.SaleRate + ((invoiceBillingRecord.SaleRate * commission.Value) / 100);
                    }

                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalSaleAmount + ((invoiceBillingRecord.OriginalSaleAmount * commission.Value) / 100);
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.SaleAmount + ((invoiceBillingRecord.SaleAmount * commission.Value) / 100);
                }
                else
                {
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalSaleAmount;
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.SaleAmount;
                }
                invoiceBillingRecord.AmountAfterCommissionWithTaxes = invoiceBillingRecord.AmountAfterCommission;
                invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.OriginalAmountAfterCommission;
                invoiceBillingRecord.OriginalSaleAmountWithTaxes = invoiceBillingRecord.OriginalSaleAmount;
                invoiceBillingRecord.SaleAmountWithTaxes = invoiceBillingRecord.SaleAmount;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        invoiceBillingRecord.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.OriginalSaleAmountWithTaxes += ((invoiceBillingRecord.OriginalSaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.SaleAmountWithTaxes += ((invoiceBillingRecord.SaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
                return invoiceBillingRecord;
            }
            return null;
        }
        private CustomerInvoiceItemDetails VoiceItemSetNamesMapper(AnalyticRecord analyticRecord, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            var saleNetValue = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleNetNotNULL");
            if (saleNetValue != 0)
            {
                CustomerInvoiceItemDetails invoiceBillingRecord = new CustomerInvoiceItemDetails
                {
                    CustomerId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 1),
                    SaleCurrencyId = currencyId,
                    OriginalSaleCurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 2),
                    SaleRate = _invoiceGenerationManager.GetDimensionValue<Decimal>(analyticRecord, 3),
                    SaleRateTypeId = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 4),
                    SaleZoneId = _invoiceGenerationManager.GetDimensionValue<long>(analyticRecord, 0),
                    Duration = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleDuration"),
                    SaleAmount = saleNetValue,
                    NumberOfCalls = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfCalls"),
                    OriginalSaleAmount = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleNet_OrigCurr"),
                    CountryId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 6),
                    SupplierId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 5),
                    SupplierZoneId = _invoiceGenerationManager.GetDimensionValue<long>(analyticRecord, 7),
                    SaleDeal = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 10),
                    SaleDealRateTierNb = _invoiceGenerationManager.GetDimensionValue<Decimal?>(analyticRecord, 11),
                    SaleDealZoneGroupNb = _invoiceGenerationManager.GetDimensionValue<long?>(analyticRecord, 8),
                    SaleDealTierNb = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 9),
                };
                var billingPeriodFromDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodFrom");
                var billingPeriodToDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodTo");
                if (offsetValue.HasValue)
                {
                    billingPeriodFromDate = billingPeriodFromDate.Add(offsetValue.Value);
                    billingPeriodToDate = billingPeriodToDate.Add(offsetValue.Value);
                }
                invoiceBillingRecord.FromDate = billingPeriodFromDate;
                invoiceBillingRecord.ToDate = billingPeriodToDate;

                if (commission.HasValue)
                {
                    if (commissionType.HasValue && commissionType.Value == CommissionType.DoNotDisplay)
                    {
                        invoiceBillingRecord.SaleRate = invoiceBillingRecord.SaleRate + ((invoiceBillingRecord.SaleRate * commission.Value) / 100);
                    }
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalSaleAmount + ((invoiceBillingRecord.OriginalSaleAmount * commission.Value) / 100);
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.SaleAmount + ((invoiceBillingRecord.SaleAmount * commission.Value) / 100);
                }
                else
                {
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalSaleAmount;
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.SaleAmount;
                }
                invoiceBillingRecord.AmountAfterCommissionWithTaxes = invoiceBillingRecord.AmountAfterCommission;
                invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.OriginalAmountAfterCommission;
                invoiceBillingRecord.OriginalSaleAmountWithTaxes = invoiceBillingRecord.OriginalSaleAmount;
                invoiceBillingRecord.SaleAmountWithTaxes = invoiceBillingRecord.SaleAmount;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        invoiceBillingRecord.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.OriginalSaleAmountWithTaxes += ((invoiceBillingRecord.OriginalSaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.SaleAmountWithTaxes += ((invoiceBillingRecord.SaleAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
                return invoiceBillingRecord;
            }
            return null;
        }
        private CustomerInvoiceDealItemDetails DealItemSetNameMapper(AnalyticRecord analyticRecord)
        {
            var saleNetValue = _invoiceGenerationManager.GetMeasureValue<decimal>(analyticRecord, "SaleNet_OrigCurr");
            if (saleNetValue == 0)
                return null;

            return new CustomerInvoiceDealItemDetails
            {
                SaleDeal = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 2),
                SaleDealRateTierNb = _invoiceGenerationManager.GetDimensionValue<decimal?>(analyticRecord, 3),
                SaleDealZoneGroupNb = _invoiceGenerationManager.GetDimensionValue<long?>(analyticRecord, 0),
                SaleDealTierNb = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 1),
                Duration = _invoiceGenerationManager.GetMeasureValue<decimal>(analyticRecord, "SaleDuration"),
                OriginalAmount = saleNetValue,
                NumberOfCalls = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfCalls"),
                CurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 4),
                Amount = _invoiceGenerationManager.GetMeasureValue<decimal>(analyticRecord, "SaleNetNotNULL")
            };
        }

        private CustomerInvoiceBySaleCurrencyItemDetails CurrencyItemSetNameMapper(AnalyticRecord analyticRecord, decimal? commission, IEnumerable<VRTaxItemDetail> taxItemDetails, bool fillVoiceData)
        {
            var saleNetValue = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleNet_OrigCurr");
            if (saleNetValue != 0)
            {
                var customerInvoiceBySaleCurrencyItemDetails = new CustomerInvoiceBySaleCurrencyItemDetails
                {
                    CurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 0),
                    FromDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodFrom"),
                    ToDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodTo"),
                    Duration = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleDuration"),
                    NumberOfCalls = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfCalls"),
                    NumberOfSMS = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfSMS"),
                    Month = _invoiceGenerationManager.GetDimensionValue<string>(analyticRecord, 1),
                };
                if (fillVoiceData)
                    customerInvoiceBySaleCurrencyItemDetails.Amount = saleNetValue;
                else
                    customerInvoiceBySaleCurrencyItemDetails.TotalSMSAmount = saleNetValue;

                if (commission.HasValue)
                {
                    customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.Amount + ((customerInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
                    customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.TotalSMSAmount + ((customerInvoiceBySaleCurrencyItemDetails.TotalSMSAmount * commission.Value) / 100);
                }
                else
                {
                    customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.Amount;
                    customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.TotalSMSAmount;
                }
                customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;
                customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes = customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommission;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((customerInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                        customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes += ((customerInvoiceBySaleCurrencyItemDetails.TotalSMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }

                customerInvoiceBySaleCurrencyItemDetails.TotalTrafficAmount = customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes;
                customerInvoiceBySaleCurrencyItemDetails.TotalSMSAmount = customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes;
                customerInvoiceBySaleCurrencyItemDetails.TotalFullAmount = fillVoiceData ? customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes : customerInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes;
                return customerInvoiceBySaleCurrencyItemDetails;
            }
            return null;

        }
        private void TryMergeByCurrencyItemSets(Func<List<CustomerInvoiceBySaleCurrencyItemDetails>> getCustomerInvoiceBySaleCurrencyItemDetails, List<CustomerInvoiceBySaleCurrencyItemDetails> sMSInvoiceBySaleCurrency)
        {
            if (sMSInvoiceBySaleCurrency != null && sMSInvoiceBySaleCurrency.Count > 0)
            {
                List<CustomerInvoiceBySaleCurrencyItemDetails> mainByCurrencyItemSets = getCustomerInvoiceBySaleCurrencyItemDetails();

                foreach (var item in sMSInvoiceBySaleCurrency)
                {
                    var invoiceBySaleCurrencyItem = mainByCurrencyItemSets.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.Month);
                    if (invoiceBySaleCurrencyItem != null)
                    {
                        invoiceBySaleCurrencyItem.TotalSMSAmount += item.TotalSMSAmount;
                        invoiceBySaleCurrencyItem.SMSAmountAfterCommission += item.SMSAmountAfterCommission;
                        invoiceBySaleCurrencyItem.SMSAmountAfterCommissionWithTaxes += item.SMSAmountAfterCommissionWithTaxes;
                        invoiceBySaleCurrencyItem.TotalFullAmount += item.SMSAmountAfterCommissionWithTaxes;
                    }
                    else
                    {
                        mainByCurrencyItemSets.Add(new CustomerInvoiceBySaleCurrencyItemDetails
                        {
                            FromDate = item.FromDate,
                            ToDate = item.ToDate,
                            TotalSMSAmount = item.TotalSMSAmount,
                            NumberOfSMS = item.NumberOfSMS,
                            CurrencyId = item.CurrencyId,
                            SMSAmountAfterCommission = item.SMSAmountAfterCommission,
                            SMSAmountAfterCommissionWithTaxes = item.SMSAmountAfterCommissionWithTaxes,
                            Month = item.Month,
                            TotalFullAmount = item.SMSAmountAfterCommissionWithTaxes,
                        });
                    }

                }
            }
        }

        private DateTime? GetDate(DateTime? firstDate, DateTime? secondDate, Func<DateTime, DateTime, DateTime> getDate)
        {
            if (!firstDate.HasValue)
                return secondDate;

            if (!secondDate.HasValue)
                return firstDate;

            return getDate(firstDate.Value, secondDate.Value);
        }

        private List<CustomerInvoiceDealItemDetails> GetDealItemSetNames(List<int> carrierAccountIds, DateTime fromDateBeforeShift, DateTime fromDate, DateTime toDate, TimeSpan? offsetValue, string dimensionName, int financialAccountId, DateTime issueDate, int currencyId, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            DateTime? effectiveVolCommitmentDealsMinBED = null;
            DateTime? effectiveVolCommitmentDealsMaxEED = null;
            var effectiveVolCommitmentDeals = new VolCommitmentDealManager().GetEffectiveVolCommitmentDeals(VolCommitmentDealType.Sell, carrierAccountIds, fromDate, toDate.Date.AddDays(1), out effectiveVolCommitmentDealsMinBED, out effectiveVolCommitmentDealsMaxEED);

            DateTime? effectiveSwapDealsMinBED = null;
            DateTime? effectiveSwapDealsMaxEED = null;
            var effectiveSwapDeals = new SwapDealManager().GetEffectiveSwapDeals(carrierAccountIds, fromDate, toDate.Date.AddDays(1), out effectiveSwapDealsMinBED, out effectiveSwapDealsMaxEED);

            bool hasEffectiveDeals = (effectiveVolCommitmentDeals != null && effectiveVolCommitmentDeals.Count > 0) || (effectiveSwapDeals != null && effectiveSwapDeals.Count > 0);

            DateTime? minBED = GetDate(effectiveVolCommitmentDealsMinBED, effectiveSwapDealsMinBED, Utilities.Min);
            DateTime? maxEED = GetDate(effectiveVolCommitmentDealsMaxEED, effectiveSwapDealsMaxEED, Utilities.Max);

            if (!hasEffectiveDeals || !minBED.HasValue || !maxEED.HasValue)
                return null;

            List<string> dealMeasures = new List<string> { "SaleNet_OrigCurr", "NumberOfCalls", "SaleDuration", "SaleNetNotNULL" };
            List<string> dealDimensions = new List<string> { "SaleDealZoneGroupNb", "SaleDealTierNb", "SaleDeal", "SaleDealRateTierNb", "SaleCurrency" };

            var allDealItemSetNames = _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(dealDimensions, dealMeasures, dimensionName, financialAccountId, minBED.Value, maxEED.Value, null, offsetValue, (analyticRecord) =>
            {
                return DealItemSetNameMapper(analyticRecord);
            });
            if (allDealItemSetNames == null)
                allDealItemSetNames = new List<CustomerInvoiceDealItemDetails>();

            var dealItemSetNames = new List<CustomerInvoiceDealItemDetails>();

            if (effectiveVolCommitmentDeals != null && effectiveVolCommitmentDeals.Count > 0)
            {
                foreach (var effectiveVolCommitmentDeal in effectiveVolCommitmentDeals)
                {
                    var effectiveVolCommitmentDealSettings = effectiveVolCommitmentDeal.Value;
                    if (effectiveVolCommitmentDealSettings.Items == null || effectiveVolCommitmentDealSettings.Items.Count == 0)
                        continue;

                    for (int i = 0; i < effectiveVolCommitmentDealSettings.Items.Count; i++)
                    {
                        var dealGroup = effectiveVolCommitmentDealSettings.Items[i];
                        if (dealGroup.Tiers == null || dealGroup.Tiers.Count == 0)
                            continue;

                        var tier = dealGroup.Tiers.First();
                        if (!tier.UpToVolume.HasValue || tier.EvaluatedRate == null)
                            continue;

                        var invoiceDealItemDetailsContext = new InvoiceDealItemDetailsContext
                        {
                            AllCustomerDealItemSetNames = allDealItemSetNames,
                            EffectiveDealId = effectiveVolCommitmentDeal.Key,
                            ZoneGroupName = dealGroup.Name,
                            ZoneGroupNumber = dealGroup.ZoneGroupNumber,
                            Rate = tier.EvaluatedRate.CastWithValidate<FixedSaleRateEvaluator>("fixedSaleRateEvaluator").Rate,
                            Volume = tier.UpToVolume.Value,
                            DealCurrencyId = effectiveVolCommitmentDealSettings.CurrencyId,
                            BeginDate = effectiveVolCommitmentDealSettings.RealBED,
                            EndDate = effectiveVolCommitmentDealSettings.RealEED,
                            FromDateBeforeShift = fromDateBeforeShift,
                            IssueDate = issueDate,
                            CurrencyId = currencyId,
                            TaxItemDetails = taxItemDetails
                        };

                        dealItemSetNames.AddRange(GetCustomerInvoiceDealItemDetails(invoiceDealItemDetailsContext));
                    }
                }
            }

            if (effectiveSwapDeals != null && effectiveSwapDeals.Count > 0)
            {
                foreach (var effectiveSwapDeal in effectiveSwapDeals)
                {
                    var effectiveSwapDealSettings = effectiveSwapDeal.Value;
                    if (effectiveSwapDealSettings.Inbounds == null || effectiveSwapDealSettings.Inbounds.Count == 0)
                        continue;

                    for (int i = 0; i < effectiveSwapDealSettings.Inbounds.Count; i++)
                    {
                        var dealGroup = effectiveSwapDealSettings.Inbounds[i];

                        var invoiceDealItemDetailsContext = new InvoiceDealItemDetailsContext
                        {
                            AllCustomerDealItemSetNames = allDealItemSetNames,
                            EffectiveDealId = effectiveSwapDeal.Key,
                            ZoneGroupName = dealGroup.Name,
                            ZoneGroupNumber = dealGroup.ZoneGroupNumber,
                            Rate = dealGroup.Rate,
                            Volume = dealGroup.Volume,
                            DealCurrencyId = effectiveSwapDealSettings.CurrencyId,
                            BeginDate = effectiveSwapDealSettings.RealBED,
                            EndDate = effectiveSwapDealSettings.RealEED,
                            FromDateBeforeShift = fromDateBeforeShift,
                            IssueDate = issueDate,
                            CurrencyId = currencyId,
                            TaxItemDetails = taxItemDetails
                        };

                        dealItemSetNames.AddRange(GetCustomerInvoiceDealItemDetails(invoiceDealItemDetailsContext));
                    }
                }
            }
            return dealItemSetNames;
        }

        private List<CustomerInvoiceDealItemDetails> GetCustomerInvoiceDealItemDetails(InvoiceDealItemDetailsContext getCustomerInvoiceDealItemDetailsContext)
        {
            var dealItemSetNames = new List<CustomerInvoiceDealItemDetails>();
            var dealItemSet = getCustomerInvoiceDealItemDetailsContext.AllCustomerDealItemSetNames.FindRecord(x => x.SaleDeal == getCustomerInvoiceDealItemDetailsContext.EffectiveDealId && getCustomerInvoiceDealItemDetailsContext.ZoneGroupNumber == x.SaleDealZoneGroupNb);

            var expectedAmount = getCustomerInvoiceDealItemDetailsContext.Volume * getCustomerInvoiceDealItemDetailsContext.Rate;
            if (dealItemSet != null)
            {
                if (expectedAmount <= dealItemSet.OriginalAmount || getCustomerInvoiceDealItemDetailsContext.DealCurrencyId != dealItemSet.CurrencyId)
                    return null;

                var originalAmount = expectedAmount - dealItemSet.OriginalAmount;
                decimal originalAmountAfterTax = originalAmount;
                if (getCustomerInvoiceDealItemDetailsContext.TaxItemDetails != null)
                {
                    foreach (var taxItemDetail in getCustomerInvoiceDealItemDetailsContext.TaxItemDetails)
                        originalAmountAfterTax += ((originalAmount * Convert.ToDecimal(taxItemDetail.Value)) / 100);
                }
                dealItemSetNames.Add(new CustomerInvoiceDealItemDetails()
                {
                    OriginalAmount = originalAmount,
                    Duration = getCustomerInvoiceDealItemDetailsContext.Volume - (dealItemSet.Duration / 60),
                    NumberOfCalls = dealItemSet.NumberOfCalls,
                    SaleDeal = dealItemSet.SaleDeal,
                    SaleDealRateTierNb = dealItemSet.SaleDealRateTierNb,
                    SaleDealTierNb = dealItemSet.SaleDealTierNb,
                    SaleDealZoneGroupNb = dealItemSet.SaleDealZoneGroupNb,
                    SaleDealZoneGroupName = getCustomerInvoiceDealItemDetailsContext.ZoneGroupName,
                    CurrencyId = dealItemSet.CurrencyId,
                    Amount = _currencyExchangeRateManager.ConvertValueToCurrency(originalAmount, dealItemSet.CurrencyId, getCustomerInvoiceDealItemDetailsContext.CurrencyId, getCustomerInvoiceDealItemDetailsContext.IssueDate),
                    ToDate = getCustomerInvoiceDealItemDetailsContext.EndDate.Value,
                    FromDate = getCustomerInvoiceDealItemDetailsContext.FromDateBeforeShift >= getCustomerInvoiceDealItemDetailsContext.BeginDate ? getCustomerInvoiceDealItemDetailsContext.FromDateBeforeShift : getCustomerInvoiceDealItemDetailsContext.BeginDate,
                    OriginalAmountAfterTax = originalAmountAfterTax,
                    SaleRate = getCustomerInvoiceDealItemDetailsContext.Rate
                });
            }
            else
            {
                decimal originalAmountAfterTax = expectedAmount;
                if (getCustomerInvoiceDealItemDetailsContext.TaxItemDetails != null)
                {
                    foreach (var taxItemDetail in getCustomerInvoiceDealItemDetailsContext.TaxItemDetails)
                        originalAmountAfterTax += ((expectedAmount * Convert.ToDecimal(taxItemDetail.Value)) / 100);
                }
                dealItemSetNames.Add(new CustomerInvoiceDealItemDetails()
                {
                    OriginalAmount = expectedAmount,
                    Duration = getCustomerInvoiceDealItemDetailsContext.Volume,
                    SaleDeal = getCustomerInvoiceDealItemDetailsContext.EffectiveDealId,
                    SaleDealRateTierNb = 1,
                    SaleDealTierNb = 1,
                    SaleDealZoneGroupNb = getCustomerInvoiceDealItemDetailsContext.ZoneGroupNumber,
                    SaleDealZoneGroupName = getCustomerInvoiceDealItemDetailsContext.ZoneGroupName,
                    CurrencyId = getCustomerInvoiceDealItemDetailsContext.DealCurrencyId,
                    Amount = _currencyExchangeRateManager.ConvertValueToCurrency(expectedAmount, getCustomerInvoiceDealItemDetailsContext.DealCurrencyId, getCustomerInvoiceDealItemDetailsContext.CurrencyId, getCustomerInvoiceDealItemDetailsContext.IssueDate),
                    ToDate = getCustomerInvoiceDealItemDetailsContext.EndDate.Value,
                    FromDate = getCustomerInvoiceDealItemDetailsContext.FromDateBeforeShift >= getCustomerInvoiceDealItemDetailsContext.BeginDate ? getCustomerInvoiceDealItemDetailsContext.FromDateBeforeShift : getCustomerInvoiceDealItemDetailsContext.BeginDate,
                    OriginalAmountAfterTax = originalAmountAfterTax,
                    SaleRate = getCustomerInvoiceDealItemDetailsContext.Rate
                });
            }
            return dealItemSetNames;
        }

        #endregion

        #region CheckUnpricedCDRs
        private bool CheckUnpricedCDRs(IInvoiceGenerationContext context, WHSFinancialAccount financialAccount)
        {
            CheckUnpricedCDRsInvoiceSettingPart checkUnpricedCDRsInvoiceSettingPart = _partnerManager.GetInvoicePartnerSettingPart<CheckUnpricedCDRsInvoiceSettingPart>(context.InvoiceTypeId, context.PartnerId);
            if (checkUnpricedCDRsInvoiceSettingPart != null && checkUnpricedCDRsInvoiceSettingPart.IsEnabled)
            {
                DataRecordStorageManager _dataRecordStorageManager = new DataRecordStorageManager();
                CarrierAccountManager _carrierAccountManager = new CarrierAccountManager();

                Guid invalidCDRTableID = new Guid("a6fa04a1-a683-4f79-8da8-b34a625af50f");
                Guid partialPricedCDRTableID = new Guid("ed4b26d7-8e08-4113-b0b1-c365adfefb50");

                List<object> values = new List<object>();
                if (financialAccount.CarrierAccountId != null)
                { values.Add(financialAccount.CarrierAccountId); }
                else
                {
                    var carrierAccounts = _carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, true, false);
                    values = carrierAccounts.Select(item => (object)item.CarrierAccountId).ToList();
                }

                List<DataRecord> invalidCDRs = _dataRecordStorageManager.GetDataRecords(context.FromDate, context.ToDate, new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>(){
                         new ObjectListRecordFilter(){FieldName = "CustomerId", CompareOperator= ListRecordFilterOperator.In, Values = values }
                    }
                }, new List<string> { "CustomerId" }, 1, OrderDirection.Ascending, invalidCDRTableID);

                if (invalidCDRs != null && invalidCDRs.Count > 0)
                {
                    return true;
                }

                List<DataRecord> partialPricedCDRs = _dataRecordStorageManager.GetDataRecords(context.FromDate, context.ToDate, new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>() {
                        new ObjectListRecordFilter(){FieldName = "CustomerId", CompareOperator= ListRecordFilterOperator.In, Values = values },
                        new NonEmptyRecordFilter(){FieldName ="SaleNet"}
                    }
                }, new List<string> { "CustomerId" }, 1, OrderDirection.Ascending, partialPricedCDRTableID);
                if (partialPricedCDRs.Count > 0)
                {
                    return true;
                }
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                context.ErrorMessage = "There are unpriced CDRs during the selected period";
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
