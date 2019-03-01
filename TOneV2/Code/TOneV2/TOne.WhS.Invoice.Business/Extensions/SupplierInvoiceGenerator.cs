﻿using System;
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
using Vanrise.Security.Business;
using Vanrise.Invoice.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SupplierInvoiceGenerator : InvoiceGenerator
    {
        PartnerManager _partnerManager = new PartnerManager();
        WHSFinancialAccountManager _financialAccountManager = new WHSFinancialAccountManager();
        TOneModuleManager _tOneModuleManager = new TOneModuleManager();
        InvoiceGenerationManager _invoiceGenerationManager = new InvoiceGenerationManager();

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            var financialAccount = _financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            int financialAccountId = financialAccount.FinancialAccountId;
            ResolvedInvoicePayloadObject resolvedPayload = _invoiceGenerationManager.GetDatesWithTimeZone<SupplierGenerationCustomSectionPayload>(context.CustomSectionPayload, financialAccountId, context.FromDate, context.ToDate);
            int currencyId = _financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);

            IEnumerable<VRTaxItemDetail> taxItemDetails = _financialAccountManager.GetFinancialAccountTaxItemDetails(financialAccount);

            List<SupplierInvoiceItemDetails> voiceItemSetNames = new List<SupplierInvoiceItemDetails>();
            List<SupplierSMSInvoiceItemDetails> smsItemSetNames = new List<SupplierSMSInvoiceItemDetails>();

            List<SupplierInvoiceBySaleCurrencyItemDetails> invoiceByCostCurrency = null;


            string dimensionName = "CostFinancialAccount";
            bool isVoiceEnabled = _tOneModuleManager.IsVoiceModuleEnabled();
            bool isSMSEnabled = _tOneModuleManager.IsSMSModuleEnabled();
            bool canGenerateVoiceInvoice = false;
            if (isVoiceEnabled)
            {
                canGenerateVoiceInvoice = CheckUnpricedVoiceCDRs(context, financialAccount);
                if (canGenerateVoiceInvoice)
                {
                    List<string> voiceListMeasures = new List<string> { "CostNetNotNULL", "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
                    List<string> voiceListDimensions = new List<string> { "SupplierZone", "Supplier", "CostCurrency", "CostRate", "CostRateType", "CostDealZoneGroupNb", "CostDealTierNb", "CostDeal", "CostDealRateTierNb" };
                    voiceItemSetNames = _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(voiceListDimensions, voiceListMeasures, dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, currencyId, resolvedPayload.OffsetValue, (analyticRecord) =>
                    {
                        return VoiceItemSetNamesMapper(analyticRecord, currencyId, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                    });
                    invoiceByCostCurrency = loadVoiceCurrencyItemSet(dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                }
            }

            if (isSMSEnabled)
            {
                List<string> smsListMeasures = new List<string> { "CostNetNotNULL", "NumberOfSMS", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
                List<string> smsListDimensions = new List<string> { "DestinationMobileNetwork", "Supplier", "CostCurrency", "CostRate" };
                smsItemSetNames = _invoiceGenerationManager.GetInvoiceSMSMappedRecords(smsListDimensions, smsListMeasures, dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, currencyId, resolvedPayload.OffsetValue, (analyticRecord) =>
                {
                    return SMSItemSetNamesMapper(analyticRecord, currencyId, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                });
                var sMSInvoiceByCostCurrencies = loadSMSCurrencyItemSet(dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
                if (invoiceByCostCurrency == null)
                    invoiceByCostCurrency = new List<SupplierInvoiceBySaleCurrencyItemDetails>();
                TryMergeByCurrencyItemSets(invoiceByCostCurrency, sMSInvoiceByCostCurrencies);
            }
           
            SupplierRecurringChargeManager supplierRecurringChargeManager = new SupplierRecurringChargeManager();
            List<RecurringChargeItem> evaluatedSupplierRecurringCharges = supplierRecurringChargeManager.GetEvaluatedRecurringCharges(financialAccount.FinancialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, context.IssueDate);

            if ((voiceItemSetNames == null || voiceItemSetNames.Count == 0) && (smsItemSetNames == null || smsItemSetNames.Count == 0) && (evaluatedSupplierRecurringCharges == null || evaluatedSupplierRecurringCharges.Count == 0))
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            decimal? minAmount = _partnerManager.GetPartnerMinAmount(context.InvoiceTypeId, context.PartnerId);
           
            if (taxItemDetails != null)
            {
                foreach (var tax in taxItemDetails)
                {
                    if (evaluatedSupplierRecurringCharges != null)
                    {
                        foreach (var item in evaluatedSupplierRecurringCharges)
                        {
                            item.AmountAfterTaxes += ((item.Amount * Convert.ToDecimal(tax.Value)) / 100);
                            item.VAT = tax.IsVAT ? tax.Value : 0;
                        }
                    }
                }
            }
            if (invoiceByCostCurrency == null)
                invoiceByCostCurrency = new List<SupplierInvoiceBySaleCurrencyItemDetails>();
            AddRecurringChargeToSupplierCurrency(invoiceByCostCurrency, evaluatedSupplierRecurringCharges, canGenerateVoiceInvoice);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(voiceItemSetNames, smsItemSetNames, taxItemDetails, invoiceByCostCurrency, evaluatedSupplierRecurringCharges, canGenerateVoiceInvoice);
            #region BuildSupplierInvoiceDetails
            SupplierInvoiceDetails supplierInvoiceDetails = BuildSupplierInvoiceDetails(voiceItemSetNames, smsItemSetNames, financialAccount.CarrierProfileId.HasValue ? "Profile" : "Account", context.FromDate, context.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, canGenerateVoiceInvoice);
            if (supplierInvoiceDetails != null)
            {
                supplierInvoiceDetails.TimeZoneId = resolvedPayload.TimeZoneId;
                supplierInvoiceDetails.TotalAmount = supplierInvoiceDetails.CostAmount;
                supplierInvoiceDetails.TotalAmountAfterCommission = supplierInvoiceDetails.AmountAfterCommission;
                supplierInvoiceDetails.TotalSMSAmountAfterCommission = supplierInvoiceDetails.SMSAmountAfterCommission;
                supplierInvoiceDetails.TotalOriginalAmountAfterCommission = supplierInvoiceDetails.OriginalAmountAfterCommission;
                supplierInvoiceDetails.TotalSMSOriginalAmountAfterCommission = supplierInvoiceDetails.SMSOriginalAmountAfterCommission;

                supplierInvoiceDetails.Commission = resolvedPayload.Commission;
                supplierInvoiceDetails.CommissionType = resolvedPayload.CommissionType;
                supplierInvoiceDetails.Offset = resolvedPayload.Offset;
                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        supplierInvoiceDetails.TotalAmountAfterCommission += ((supplierInvoiceDetails.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalSMSAmountAfterCommission += ((supplierInvoiceDetails.SMSAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalOriginalAmountAfterCommission += ((supplierInvoiceDetails.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalSMSOriginalAmountAfterCommission += ((supplierInvoiceDetails.SMSOriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalAmount += ((supplierInvoiceDetails.CostAmount * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceDetails.TotalSMSAmount += ((supplierInvoiceDetails.TotalSMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                    context.ActionAfterGenerateInvoice = (invoice) =>
                    {

                        SupplierBillingRecurringChargeManager supplierBillingRecurringChargeManager = new SupplierBillingRecurringChargeManager();
                        var userId = SecurityContext.Current.GetLoggedInUserId();
                        foreach (var item in evaluatedSupplierRecurringCharges)
                        {
                            supplierBillingRecurringChargeManager.AddSupplierBillingRecurringCharge(new SupplierBillingRecurringCharge
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
                        return true;
                    };

                }
                CurrencyManager currencyManager = new CurrencyManager();
                var systemCurrency = currencyManager.GetSystemCurrency();
                systemCurrency.ThrowIfNull("systemCurrency");
                CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
                decimal totalVoiceAmountAfterCommissionInSystemCurrency = currencyExchangeRateManager.ConvertValueToCurrency(supplierInvoiceDetails.TotalAmountAfterCommission, currencyId, systemCurrency.CurrencyId, context.IssueDate);
                decimal totalSMSAmountAfterCommissionInSystemCurrency = currencyExchangeRateManager.ConvertValueToCurrency(supplierInvoiceDetails.TotalSMSAmountAfterCommission, currencyId, systemCurrency.CurrencyId, context.IssueDate);

                decimal totalReccurringChargesInSystemCurrency = 0;
                foreach (var item in evaluatedSupplierRecurringCharges)
                {
                    totalReccurringChargesInSystemCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, systemCurrency.CurrencyId, context.IssueDate);
                }
                var totalAmountInSystemCurrency = totalReccurringChargesInSystemCurrency + totalVoiceAmountAfterCommissionInSystemCurrency + totalSMSAmountAfterCommissionInSystemCurrency;
                if ((minAmount.HasValue && totalAmountInSystemCurrency >= minAmount.Value) || (!minAmount.HasValue && totalAmountInSystemCurrency != 0))
                {
                    var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
                    definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
                    definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
                    var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == context.InvoiceTypeId);
                    financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");

                    if (!financialAccountInvoiceType.IgnoreFromBalance)
                    {
                        SetInvoiceBillingTransactions(context, supplierInvoiceDetails, financialAccount, resolvedPayload.FromDate, resolvedPayload.ToDateForBillingTransaction);
                    }

                    ConfigManager configManager = new ConfigManager();
                    InvoiceTypeSetting settings = configManager.GetInvoiceTypeSettingsById(context.InvoiceTypeId);

                    if (settings != null)
                    {
                        context.NeedApproval = settings.NeedApproval;
                    }

                    decimal totalReccurringChargesAfterTaxInAccountCurrency = 0;
                    decimal totalReccurringChargesInAccountCurrency = 0;

                    foreach (var item in evaluatedSupplierRecurringCharges)
                    {
                        totalReccurringChargesAfterTaxInAccountCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, currencyId, context.IssueDate);
                        totalReccurringChargesInAccountCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.Amount, item.CurrencyId, currencyId, context.IssueDate);

                    }
                    supplierInvoiceDetails.TotalReccurringChargesAfterTax = totalReccurringChargesAfterTaxInAccountCurrency;
                    supplierInvoiceDetails.TotalReccurringCharges = totalReccurringChargesInAccountCurrency;
                    supplierInvoiceDetails.TotalInvoiceAmount = supplierInvoiceDetails.TotalAmountAfterCommission + supplierInvoiceDetails.TotalReccurringChargesAfterTax + supplierInvoiceDetails.TotalSMSAmountAfterCommission;

                    context.Invoice = new GeneratedInvoice
                    {
                        InvoiceDetails = supplierInvoiceDetails,
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
        private void AddRecurringChargeToSupplierCurrency(List<SupplierInvoiceBySaleCurrencyItemDetails> costCurrencyItemDetails, List<RecurringChargeItem> recurringChargeItems, bool canGenerateVoiceInvoice)
        {
            if (recurringChargeItems != null && recurringChargeItems.Count > 0)
            {
                if (costCurrencyItemDetails == null)
                    costCurrencyItemDetails = new List<SupplierInvoiceBySaleCurrencyItemDetails>();

                foreach (var item in recurringChargeItems)
                {
                    if (canGenerateVoiceInvoice)
                    {
                        var supplierInvoiceBySaleCurrencyItemDetail = costCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
                        if (supplierInvoiceBySaleCurrencyItemDetail != null)
                        {
                            supplierInvoiceBySaleCurrencyItemDetail.Amount += item.Amount;
                            supplierInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.Amount;
                            supplierInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.AmountAfterTaxes;
                            supplierInvoiceBySaleCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
                        }
                        else
                        {
                            costCurrencyItemDetails.Add(new SupplierInvoiceBySaleCurrencyItemDetails
                            {
                                FromDate = item.From,
                                ToDate = item.To,
                                AmountAfterCommission = item.Amount,
                                AmountAfterCommissionWithTaxes = item.AmountAfterTaxes,
                                NumberOfCalls = 0,
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
        }
        private List<SupplierInvoiceBySaleCurrencyItemDetails> loadVoiceCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            List<string> voiceListMeasures = new List<string> { "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
            List<string> voiceListDimensions = new List<string> { "CostCurrency", "MonthQueryTimeShift" };
            return _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(voiceListDimensions, voiceListMeasures, dimentionName, dimensionValue, fromDate, toDate, null, offsetValue, (analyticRecord) => {
                return CurrencyItemSetNameMapper(analyticRecord, commission, taxItemDetails, true);
            });
        }
        private List<SupplierInvoiceBySaleCurrencyItemDetails> loadSMSCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            List<string> smsListMeasures = new List<string> { "NumberOfSMS", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
            List<string> smsListDimensions = new List<string> { "CostCurrency", "MonthQueryTimeShift" };
            return _invoiceGenerationManager.GetInvoiceSMSMappedRecords(smsListDimensions, smsListMeasures, dimentionName, dimensionValue, fromDate, toDate, null, offsetValue, (analyticRecord) => {
                return CurrencyItemSetNameMapper(analyticRecord, commission, taxItemDetails, false);
            });
        }
        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, SupplierInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount, DateTime fromDate, DateTime toDate)
        {
            var financialAccountDefinitionManager = new WHSFinancialAccountDefinitionManager();
            var balanceAccountTypeId = financialAccountDefinitionManager.GetBalanceAccountTypeId(financialAccount.FinancialAccountDefinitionId);
            if (balanceAccountTypeId.HasValue)
            {
                Vanrise.Invoice.Entities.InvoiceType invoiceType = new Vanrise.Invoice.Business.InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
                invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
                invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
                SupplierInvoiceSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<SupplierInvoiceSettings>("invoiceType.Settings.ExtendedSettings");

                var billingTransaction = new GeneratedInvoiceBillingTransaction()
                {
                    AccountTypeId = balanceAccountTypeId.Value,
                    AccountId = context.PartnerId,
                    TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
                    Amount = invoiceDetails.TotalAmountAfterCommission,
                    CurrencyId = invoiceDetails.SupplierCurrencyId,
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
        private SupplierInvoiceDetails BuildSupplierInvoiceDetails(List<SupplierInvoiceItemDetails> voiceItemSetNames, List<SupplierSMSInvoiceItemDetails> smsItemSetNames,  string partnerType, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, bool canGenerateVoiceInvoice)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            SupplierInvoiceDetails supplierInvoiceDetails = null;
            if (partnerType != null)
            {
                if (voiceItemSetNames != null && voiceItemSetNames.Count > 0 && canGenerateVoiceInvoice)
                {
                    supplierInvoiceDetails = new SupplierInvoiceDetails()
                    {
                        PartnerType = partnerType
                    };
                    foreach (var invoiceBillingRecord in voiceItemSetNames)
                    {
                        supplierInvoiceDetails.Duration += invoiceBillingRecord.Duration;
                        supplierInvoiceDetails.CostAmount += invoiceBillingRecord.CostAmount;
                        supplierInvoiceDetails.OriginalCostAmount += invoiceBillingRecord.OriginalCostAmount;
                        supplierInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.NumberOfCalls;
                        supplierInvoiceDetails.OriginalSupplierCurrencyId = invoiceBillingRecord.OriginalSupplierCurrencyId;
                        supplierInvoiceDetails.SupplierCurrencyId = invoiceBillingRecord.SupplierCurrencyId;
                        supplierInvoiceDetails.AmountAfterCommission += invoiceBillingRecord.AmountAfterCommission;
                        supplierInvoiceDetails.OriginalAmountAfterCommission += invoiceBillingRecord.OriginalAmountAfterCommission;
                    }
                }
                if (smsItemSetNames != null && smsItemSetNames.Count > 0)
                {
                    if (supplierInvoiceDetails == null)
                    {
                        supplierInvoiceDetails = new SupplierInvoiceDetails()
                        {
                            PartnerType = partnerType
                        };
                    }
                    foreach (var invoiceBillingRecord in smsItemSetNames)
                    {
                        supplierInvoiceDetails.TotalSMSAmount += invoiceBillingRecord.CostAmount;
                        supplierInvoiceDetails.TotalNumberOfSMS += invoiceBillingRecord.NumberOfSMS;
                        supplierInvoiceDetails.OriginalSupplierCurrencyId = invoiceBillingRecord.OriginalSupplierCurrencyId;
                        supplierInvoiceDetails.SupplierCurrencyId = invoiceBillingRecord.SupplierCurrencyId;
                        supplierInvoiceDetails.SMSAmountAfterCommission += invoiceBillingRecord.AmountAfterCommission;
                        supplierInvoiceDetails.SMSOriginalAmountAfterCommission += invoiceBillingRecord.OriginalAmountAfterCommission;
                    }
                }
            }
            if (supplierInvoiceDetails != null)
            {
                supplierInvoiceDetails.OriginalSupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.OriginalSupplierCurrencyId);
                supplierInvoiceDetails.SupplierCurrency = currencyManager.GetCurrencySymbol(supplierInvoiceDetails.SupplierCurrencyId);
                if (commissionType.HasValue)
                {
                    switch (commissionType.Value)
                    {
                        case CommissionType.Display:
                            supplierInvoiceDetails.DisplayComission = true;
                            break;
                    }
                }
                else
                {
                    supplierInvoiceDetails.DisplayComission = false;
                }
            }
            return supplierInvoiceDetails;
        }

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(List<SupplierInvoiceItemDetails> voiceItemSetNames, List<SupplierSMSInvoiceItemDetails> smsItemSetNames, IEnumerable<VRTaxItemDetail> taxItemDetails, List<SupplierInvoiceBySaleCurrencyItemDetails> supplierVoiceInvoicesBySaleCurrency, List<RecurringChargeItem> supplierRecurringCharges, bool canGenerateVoiceInvoice)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupingByCurrency", generatedInvoiceItemSets, supplierVoiceInvoicesBySaleCurrency);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupedByCostZone", generatedInvoiceItemSets, voiceItemSetNames);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("Taxes", generatedInvoiceItemSets, taxItemDetails);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupedByCostMobileNetwork", generatedInvoiceItemSets, smsItemSetNames);
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("RecurringCharge", generatedInvoiceItemSets, supplierRecurringCharges);
            return generatedInvoiceItemSets;
        }
        private SupplierSMSInvoiceItemDetails SMSItemSetNamesMapper(AnalyticRecord analyticRecord, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            var costNetValue = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "CostNetNotNULL");
            if (costNetValue != 0)
            {
                SupplierSMSInvoiceItemDetails invoiceBillingRecord = new SupplierSMSInvoiceItemDetails
                {
                    SupplierId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 1),
                    SupplierMobileNetworkId = _invoiceGenerationManager.GetDimensionValue<long>(analyticRecord, 0),
                    SupplierCurrencyId = currencyId,
                    OriginalSupplierCurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 2),
                    SupplierRate = _invoiceGenerationManager.GetDimensionValue<Decimal>(analyticRecord, 3),
                    CostAmount = costNetValue,
                    NumberOfSMS = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfSMS"),
                    OriginalCostAmount = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "CostNet_OrigCurr"),
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
                        invoiceBillingRecord.SupplierRate = invoiceBillingRecord.SupplierRate + ((invoiceBillingRecord.SupplierRate * commission.Value) / 100);
                    }
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalCostAmount + ((invoiceBillingRecord.OriginalCostAmount * commission.Value) / 100);
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.CostAmount + ((invoiceBillingRecord.CostAmount * commission.Value) / 100);
                }
                else
                {
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalCostAmount;
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.CostAmount;
                }

                invoiceBillingRecord.AmountAfterCommissionWithTaxes = invoiceBillingRecord.AmountAfterCommission;
                invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.OriginalAmountAfterCommission;
                invoiceBillingRecord.OriginalSupplierAmountWithTaxes = invoiceBillingRecord.OriginalCostAmount;
                invoiceBillingRecord.SupplierAmountWithTaxes = invoiceBillingRecord.CostAmount;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        invoiceBillingRecord.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                        invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                        invoiceBillingRecord.OriginalSupplierAmountWithTaxes += ((invoiceBillingRecord.OriginalCostAmount * Convert.ToDecimal(tax.Value)) / 100);

                        invoiceBillingRecord.SupplierAmountWithTaxes += ((invoiceBillingRecord.CostAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
                return invoiceBillingRecord;
            }
            return null;
        }
        private SupplierInvoiceItemDetails VoiceItemSetNamesMapper(AnalyticRecord analyticRecord, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            var costNetValue = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "CostNetNotNULL");
            if (costNetValue != 0)
            {
                SupplierInvoiceItemDetails invoiceBillingRecord = new SupplierInvoiceItemDetails
                {
                    SupplierId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord,1),
                    SupplierCurrencyId = currencyId,
                    OriginalSupplierCurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord,2),
                    SupplierRate = _invoiceGenerationManager.GetDimensionValue<Decimal>(analyticRecord, 3),
                    SupplierRateTypeId = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 4),
                    SupplierZoneId = _invoiceGenerationManager.GetDimensionValue<long>(analyticRecord, 0),
                    Duration = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "CostDuration"),
                    CostAmount = costNetValue,
                    NumberOfCalls = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfCalls"),
                    OriginalCostAmount = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "CostNet_OrigCurr"),
                    CostDeal = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 7),
                    CostDealRateTierNb = _invoiceGenerationManager.GetDimensionValue<Decimal?>(analyticRecord, 8),
                    CostDealZoneGroupNb = _invoiceGenerationManager.GetDimensionValue<long?>(analyticRecord, 5),
                    CostDealTierNb = _invoiceGenerationManager.GetDimensionValue<int?>(analyticRecord, 6),
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
                        invoiceBillingRecord.SupplierRate = invoiceBillingRecord.SupplierRate + ((invoiceBillingRecord.SupplierRate * commission.Value) / 100);
                    }
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalCostAmount + ((invoiceBillingRecord.OriginalCostAmount * commission.Value) / 100);
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.CostAmount + ((invoiceBillingRecord.CostAmount * commission.Value) / 100);
                }
                else
                {
                    invoiceBillingRecord.OriginalAmountAfterCommission = invoiceBillingRecord.OriginalCostAmount;
                    invoiceBillingRecord.AmountAfterCommission = invoiceBillingRecord.CostAmount;
                }

                invoiceBillingRecord.AmountAfterCommissionWithTaxes = invoiceBillingRecord.AmountAfterCommission;
                invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.OriginalAmountAfterCommission;
                invoiceBillingRecord.OriginalSupplierAmountWithTaxes = invoiceBillingRecord.OriginalCostAmount;
                invoiceBillingRecord.SupplierAmountWithTaxes = invoiceBillingRecord.CostAmount;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        invoiceBillingRecord.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.OriginalSupplierAmountWithTaxes += ((invoiceBillingRecord.OriginalCostAmount * Convert.ToDecimal(tax.Value)) / 100);
                        invoiceBillingRecord.SupplierAmountWithTaxes += ((invoiceBillingRecord.CostAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }
                return invoiceBillingRecord;
            }
            return null;
        }

        private SupplierInvoiceBySaleCurrencyItemDetails CurrencyItemSetNameMapper(AnalyticRecord analyticRecord, decimal? commission, IEnumerable<VRTaxItemDetail> taxItemDetails, bool fillVoiceData)
        {
            var saleNetValue = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "CostNet_OrigCurr");
            if (saleNetValue != 0)
            {
                var supplierInvoiceBySaleCurrencyItemDetails = new SupplierInvoiceBySaleCurrencyItemDetails
                {
                    CurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 0),
                    FromDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodFrom"),
                    ToDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodTo"),
                    Duration = _invoiceGenerationManager.GetMeasureValue<Decimal>(analyticRecord, "SaleDuration"),
                    NumberOfCalls = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfCalls"),
                    NumberOfSMS = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "NumberOfSMS"),
                    Amount = saleNetValue,
                    Month = _invoiceGenerationManager.GetDimensionValue<string>(analyticRecord, 1),
                };

                if (commission.HasValue)
                {
                    supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount + ((supplierInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
                    supplierInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.TotalSMSAmount + ((supplierInvoiceBySaleCurrencyItemDetails.TotalSMSAmount * commission.Value) / 100);
                }
                else
                {
                    supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount;
                    supplierInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.TotalSMSAmount;
                }
                supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;
                supplierInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes = supplierInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommission;

                if (taxItemDetails != null)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((supplierInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                        supplierInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes += ((supplierInvoiceBySaleCurrencyItemDetails.TotalSMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }

                supplierInvoiceBySaleCurrencyItemDetails.TotalTrafficAmount = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes;
                supplierInvoiceBySaleCurrencyItemDetails.TotalSMSAmount = supplierInvoiceBySaleCurrencyItemDetails.SMSAmountAfterCommissionWithTaxes;
                supplierInvoiceBySaleCurrencyItemDetails.TotalFullAmount = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes;

                return supplierInvoiceBySaleCurrencyItemDetails;
            }
            return null;

        }
        public void TryMergeByCurrencyItemSets(List<SupplierInvoiceBySaleCurrencyItemDetails> mainByCurrencyItemSets, List<SupplierInvoiceBySaleCurrencyItemDetails> sMSInvoiceBySaleCurrency)
        {
            if (sMSInvoiceBySaleCurrency != null && sMSInvoiceBySaleCurrency.Count > 0)
            {
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
                        mainByCurrencyItemSets.Add(new SupplierInvoiceBySaleCurrencyItemDetails
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

        #region CheckUnpricedCDRs

        private bool CheckUnpricedVoiceCDRs(IInvoiceGenerationContext context, WHSFinancialAccount financialAccount)
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
                {
                    values.Add(financialAccount.CarrierAccountId);
                }
                else
                {
                    var carrierAccounts = _carrierAccountManager.GetCarriersByProfileId(financialAccount.CarrierProfileId.Value, false, true);
                    values = carrierAccounts.Select(item => (object)item.CarrierAccountId).ToList();
                }

                List<DataRecord> invalidCDRs = _dataRecordStorageManager.GetDataRecords(context.FromDate, context.ToDate, new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>()
                {
                    new ObjectListRecordFilter(){FieldName = "SupplierId", CompareOperator= ListRecordFilterOperator.In, Values = values }
                }
                }, new List<string> { "SupplierId" }, 1, OrderDirection.Ascending, invalidCDRTableID);
                if (invalidCDRs.Count > 0)
                {
                    return true;
                }

                List<DataRecord> partialPricedCDRs = _dataRecordStorageManager.GetDataRecords(context.FromDate, context.ToDate, new RecordFilterGroup()
                {
                    LogicalOperator = RecordQueryLogicalOperator.And,
                    Filters = new List<RecordFilter>()
                {
                    new ObjectListRecordFilter(){FieldName = "SupplierId", CompareOperator= ListRecordFilterOperator.In, Values = values },
                    new NonEmptyRecordFilter(){FieldName ="CostNet"}
                }
                }, new List<string> { "SupplierId" }, 1, OrderDirection.Ascending, partialPricedCDRTableID);
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
