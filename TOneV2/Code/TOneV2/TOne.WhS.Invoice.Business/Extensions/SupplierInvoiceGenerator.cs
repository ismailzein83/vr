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

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            var financialAccount = _financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
            var supplierGenerationCustomSectionPayload = context.CustomSectionPayload as SupplierGenerationCustomSectionPayload;
            int? timeZoneId = null;
            decimal? commission = null;
            CommissionType? commissionType = null;
            if (supplierGenerationCustomSectionPayload != null)
            {
                timeZoneId = supplierGenerationCustomSectionPayload.TimeZoneId;
                if (supplierGenerationCustomSectionPayload.Commission.HasValue)
                {
                    commission = supplierGenerationCustomSectionPayload.Commission.Value;
                    commissionType = supplierGenerationCustomSectionPayload.CommissionType;
                }
            }
            if (!timeZoneId.HasValue)
            {
                timeZoneId = _financialAccountManager.GetSupplierTimeZoneId(financialAccount.FinancialAccountId);
            }
            string offset = null;
            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;
            TimeSpan? offsetValue = null;

            DateTime toDateForBillingTransaction = context.ToDate.Date.AddDays(1);
            if (timeZoneId.HasValue)
            {
                VRTimeZone timeZone = new VRTimeZoneManager().GetVRTimeZone(timeZoneId.Value);
                if (timeZone != null)
                {
                    offsetValue = timeZone.Settings.Offset;
                    offset = timeZone.Settings.Offset.ToString();
                    fromDate = context.FromDate.Add(-timeZone.Settings.Offset);
                    toDate = context.ToDate.Add(-timeZone.Settings.Offset);
                    toDateForBillingTransaction = toDateForBillingTransaction.Add(-timeZone.Settings.Offset);
                }
            }
            int currencyId = _financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);
            IEnumerable<VRTaxItemDetail> taxItemDetails = _financialAccountManager.GetFinancialAccountTaxItemDetails(financialAccount);

            AnalyticSummaryBigResult<AnalyticRecord> voiceAnalyticResult = new AnalyticSummaryBigResult<AnalyticRecord>();
            AnalyticSummaryBigResult<AnalyticRecord> smsAnalyticResult = new AnalyticSummaryBigResult<AnalyticRecord>();

            var voiceAnalyticTableId = new Guid("4C1AAA1B-675B-420F-8E60-26B0747CA79B");
            var smsAnalytictableId = new Guid("53e9ebc8-c674-4aff-b6c0-9b3b18f95c1f");
            string dimensionName = "CostFinancialAccount";
            int dimensionValue = financialAccount.FinancialAccountId;
            bool isVoiceEnabled = _tOneModuleManager.IsVoiceModuleEnabled();
            bool isSMSEnabled = _tOneModuleManager.IsSMSModuleEnabled();
            if (isVoiceEnabled)
            {
                if (!CheckUnpricedVoiceCDRs(context, financialAccount))
                    return;
                List<string> voiceListMeasures = new List<string> { "CostNetNotNULL", "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
                List<string> voiceListDimensions = new List<string> { "SupplierZone", "Supplier", "CostCurrency", "CostRate", "CostRateType", "CostDealZoneGroupNb", "CostDealTierNb", "CostDeal", "CostDealRateTierNb" };
                voiceAnalyticResult = GetFilteredRecords(voiceListDimensions, voiceListMeasures, dimensionName, dimensionValue, fromDate, toDate, voiceAnalyticTableId, currencyId, offsetValue);
            }

            if (isSMSEnabled)
            {
                List<string> smsListMeasures = new List<string> { "CostNetNotNULL", "NumberOfSMS", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
                List<string> smsListDimensions = new List<string> { "DestinationMobileNetwork", "Supplier", "CostCurrency", "CostRate" };
                smsAnalyticResult = GetFilteredRecords(smsListDimensions, smsListMeasures, dimensionName, dimensionValue, fromDate, toDate, smsAnalytictableId, currencyId, offsetValue);
            }

            List<InvoiceBillingRecord> voiceItemSetNames = new List<InvoiceBillingRecord>();
            List<SMSInvoiceBillingRecord> smsItemSetNames = new List<SMSInvoiceBillingRecord>();
            ConvertAnalyticDataToList(voiceAnalyticResult.Data, smsAnalyticResult.Data, currencyId, voiceItemSetNames, smsItemSetNames, commission, commissionType, taxItemDetails, offsetValue);

            SupplierRecurringChargeManager supplierRecurringChargeManager = new SupplierRecurringChargeManager();
            List<RecurringChargeItem> evaluatedSupplierRecurringCharges = supplierRecurringChargeManager.GetEvaluatedRecurringCharges(financialAccount.FinancialAccountId, fromDate, toDate, context.IssueDate);

            if ( voiceItemSetNames.Count == 0 && smsItemSetNames.Count == 0 && (evaluatedSupplierRecurringCharges == null || evaluatedSupplierRecurringCharges.Count == 0))
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            decimal? minAmount = _partnerManager.GetPartnerMinAmount(context.InvoiceTypeId, context.PartnerId);
            List<SupplierInvoiceBySaleCurrencyItemDetails> supplierVoiceInvoiceBySaleCurrencyItemDetails = null;
            List<SupplierSMSInvoiceBySaleCurrencyItemDetails> supplierSMSInvoiceBySaleCurrencyItemDetails = null;
            LoadCurrencyItemSet(dimensionName, dimensionValue, fromDate, toDate, commission, commissionType, taxItemDetails, offsetValue, voiceAnalyticTableId, smsAnalytictableId, isVoiceEnabled, isSMSEnabled, out supplierVoiceInvoiceBySaleCurrencyItemDetails, out supplierSMSInvoiceBySaleCurrencyItemDetails);

            if (supplierVoiceInvoiceBySaleCurrencyItemDetails == null)
                supplierVoiceInvoiceBySaleCurrencyItemDetails = new List<SupplierInvoiceBySaleCurrencyItemDetails>();
            if (supplierSMSInvoiceBySaleCurrencyItemDetails == null)
                supplierSMSInvoiceBySaleCurrencyItemDetails = new List<SupplierSMSInvoiceBySaleCurrencyItemDetails>();
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

            AddRecurringChargeToSupplierCurrency(supplierVoiceInvoiceBySaleCurrencyItemDetails, supplierSMSInvoiceBySaleCurrencyItemDetails, evaluatedSupplierRecurringCharges);

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(voiceItemSetNames, smsItemSetNames, taxItemDetails, supplierVoiceInvoiceBySaleCurrencyItemDetails, supplierSMSInvoiceBySaleCurrencyItemDetails, evaluatedSupplierRecurringCharges);
            #region BuildSupplierInvoiceDetails
            SupplierInvoiceDetails supplierInvoiceDetails = BuildSupplierInvoiceDetails(voiceItemSetNames, smsItemSetNames, financialAccount.CarrierProfileId.HasValue ? "Profile" : "Account", context.FromDate, context.ToDate, commission, commissionType);
            if (supplierInvoiceDetails != null)
            {
                supplierInvoiceDetails.TimeZoneId = timeZoneId;
                supplierInvoiceDetails.TotalAmount = supplierInvoiceDetails.CostAmount;
                supplierInvoiceDetails.TotalAmountAfterCommission = supplierInvoiceDetails.AmountAfterCommission;
                supplierInvoiceDetails.TotalSMSAmountAfterCommission = supplierInvoiceDetails.SMSAmountAfterCommission;
                supplierInvoiceDetails.TotalOriginalAmountAfterCommission = supplierInvoiceDetails.OriginalAmountAfterCommission;
                supplierInvoiceDetails.TotalSMSOriginalAmountAfterCommission = supplierInvoiceDetails.SMSOriginalAmountAfterCommission;

                supplierInvoiceDetails.Commission = commission;
                supplierInvoiceDetails.CommissionType = commissionType;
                supplierInvoiceDetails.Offset = offset;
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
                        SetInvoiceBillingTransactions(context, supplierInvoiceDetails, financialAccount, fromDate, toDateForBillingTransaction);
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

        private void AddRecurringChargeToSupplierCurrency(List<SupplierInvoiceBySaleCurrencyItemDetails> supplierInvoiceBySaleCurrencyItemDetails, List<SupplierSMSInvoiceBySaleCurrencyItemDetails> supplierSMSInvoiceBySaleCurrencyItemDetails, List<RecurringChargeItem> recurringChargeItems)
        {
            if (recurringChargeItems != null && recurringChargeItems.Count > 0)
            {
                if (supplierInvoiceBySaleCurrencyItemDetails == null)
                    supplierInvoiceBySaleCurrencyItemDetails = new List<SupplierInvoiceBySaleCurrencyItemDetails>();

                foreach (var item in recurringChargeItems)
                {
                    var supplierInvoiceBySaleCurrencyItemDetail = supplierInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
                    if (supplierInvoiceBySaleCurrencyItemDetail != null)
                    {
                        supplierInvoiceBySaleCurrencyItemDetail.Amount += item.Amount;
                        supplierInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.Amount;
                        supplierInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.AmountAfterTaxes;
                        supplierInvoiceBySaleCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
                    }
                    else
                    {
                        supplierInvoiceBySaleCurrencyItemDetails.Add(new SupplierInvoiceBySaleCurrencyItemDetails
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
                    if (supplierSMSInvoiceBySaleCurrencyItemDetails == null)
                        supplierSMSInvoiceBySaleCurrencyItemDetails = new List<SupplierSMSInvoiceBySaleCurrencyItemDetails>();
                    var supplierSMSInvoiceBySaleCurrencyItemDetail = supplierSMSInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
                    if (supplierSMSInvoiceBySaleCurrencyItemDetail != null)
                    {
                        supplierSMSInvoiceBySaleCurrencyItemDetail.Amount += item.Amount;
                        supplierSMSInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.Amount;
                        supplierSMSInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.AmountAfterTaxes;
                        supplierSMSInvoiceBySaleCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
                    }
                    else
                    {
                        supplierSMSInvoiceBySaleCurrencyItemDetails.Add(new SupplierSMSInvoiceBySaleCurrencyItemDetails
                        {
                            FromDate = item.From,
                            ToDate = item.To,
                            AmountAfterCommission = item.Amount,
                            AmountAfterCommissionWithTaxes = item.AmountAfterTaxes,
                            NumberOfSMS = 0,
                            CurrencyId = item.CurrencyId,
                            Amount = item.Amount,
                            TotalRecurringChargeAmount = item.AmountAfterTaxes,
                            Month = item.RecurringChargeMonth
                        });

                    }
                }
            }
        }


        private void LoadCurrencyItemSet(string dimensionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue, Guid voiceAnalytictableId, Guid smsAnalyticTableId, bool isVoiceEnabled, bool isSMSEnabled, out List<SupplierInvoiceBySaleCurrencyItemDetails> supplierVoiceInvoiceBySaleCurrencyItemDetails,  out List<SupplierSMSInvoiceBySaleCurrencyItemDetails> supplierSMSInvoiceBySaleCurrencyItemDetails)
        {
            supplierVoiceInvoiceBySaleCurrencyItemDetails = null;
            supplierSMSInvoiceBySaleCurrencyItemDetails = null;
            if (isVoiceEnabled)
            {
                List<string> voiceListMeasures = new List<string> { "NumberOfCalls", "CostDuration", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
                List<string> voiceListDimensions = new List<string> { "CostCurrency", "MonthQueryTimeShift" };
                var voiceAnalyticResult = GetFilteredRecords(voiceListDimensions, voiceListMeasures, dimensionName, dimensionValue, fromDate, toDate, voiceAnalytictableId, null, offsetValue);
                if (voiceAnalyticResult != null && voiceAnalyticResult.Data != null && voiceAnalyticResult.Data.Count() != 0)
                {
                    supplierVoiceInvoiceBySaleCurrencyItemDetails =  BuildVoiceCurrencyItemSetNameFromAnalytic(voiceAnalyticResult.Data, commission, commissionType, taxItemDetails);
                }
            }
            if (isSMSEnabled)
            {
                List<string> smsListMeasures = new List<string> { "NumberOfSMS", "BillingPeriodTo", "BillingPeriodFrom", "CostNet_OrigCurr" };
                List<string> smsListDimensions = new List<string> { "CostCurrency", "MonthQueryTimeShift" };
                var smsAnalyticResult = GetFilteredRecords(smsListDimensions, smsListMeasures, dimensionName, dimensionValue, fromDate, toDate, smsAnalyticTableId, null, offsetValue);
                if (smsAnalyticResult != null && smsAnalyticResult.Data != null && smsAnalyticResult.Data.Count() != 0)
                {
                    supplierSMSInvoiceBySaleCurrencyItemDetails = BuildSMSCurrencyItemSetNameFromAnalytic(smsAnalyticResult.Data, commission, commissionType, taxItemDetails);
                }
            }
        }

        private List<SupplierInvoiceBySaleCurrencyItemDetails> BuildVoiceCurrencyItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            List<SupplierInvoiceBySaleCurrencyItemDetails> supplierInvoiceBySaleCurrencies = null;
            if (analyticRecords != null)
            {
                supplierInvoiceBySaleCurrencies = new List<SupplierInvoiceBySaleCurrencyItemDetails>();
                foreach (var analyticRecord in analyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue costCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue month = analyticRecord.DimensionValues.ElementAtOrDefault(1);

                    MeasureValue costNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue costDuration = GetMeasureValue(analyticRecord, "CostDuration");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion

                    var costNet = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0);
                    if (costNet != 0)
                    {
                        var supplierInvoiceBySaleCurrencyItemDetails = new SupplierInvoiceBySaleCurrencyItemDetails
                        {
                            CurrencyId = Convert.ToInt32(costCurrencyId.Value),
                            FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            Duration = Convert.ToDecimal(costDuration.Value ?? 0.0),
                            NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                            Amount = costNet,
                            Month = month.Value != null ? month.Value.ToString() : null
                        };
                        if (commission.HasValue)
                        {
                            supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount + ((supplierInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
                        }
                        else
                        {
                            supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount;
                        }

                        supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;

                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((supplierInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }
                        supplierInvoiceBySaleCurrencyItemDetails.TotalTrafficAmount = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes;
                        supplierInvoiceBySaleCurrencies.Add(supplierInvoiceBySaleCurrencyItemDetails);
                    }

                }
            }
            return supplierInvoiceBySaleCurrencies;
        }
        private List<SupplierSMSInvoiceBySaleCurrencyItemDetails> BuildSMSCurrencyItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            List<SupplierSMSInvoiceBySaleCurrencyItemDetails> supplierInvoiceBySaleCurrencies = null;

            if (analyticRecords != null)
            {
                supplierInvoiceBySaleCurrencies = new List<SupplierSMSInvoiceBySaleCurrencyItemDetails>();
                foreach (var analyticRecord in analyticRecords)
                {
                    #region ReadDataFromAnalyticResult
                    DimensionValue costCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue month = analyticRecord.DimensionValues.ElementAtOrDefault(1);

                    MeasureValue costNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue sms = GetMeasureValue(analyticRecord, "NumberOfSMS");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion

                    var costNet = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0);
                    if (costNet != 0)
                    {
                        var supplierInvoiceBySaleCurrencyItemDetails = new SupplierSMSInvoiceBySaleCurrencyItemDetails
                        {
                            CurrencyId = Convert.ToInt32(costCurrencyId.Value),
                            FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                            ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                            NumberOfSMS = Convert.ToInt32(sms.Value ?? 0.0),
                            Amount = costNet,
                            Month = month.Value != null ? month.Value.ToString() : null
                        };
                        if (commission.HasValue)
                        {
                            supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount + ((supplierInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
                        }
                        else
                        {
                            supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = supplierInvoiceBySaleCurrencyItemDetails.Amount;
                        }

                        supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;

                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                supplierInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((supplierInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }
                        supplierInvoiceBySaleCurrencies.Add(supplierInvoiceBySaleCurrencyItemDetails);
                    }

                }
            }
            return supplierInvoiceBySaleCurrencies;
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
        private SupplierInvoiceDetails BuildSupplierInvoiceDetails(List<InvoiceBillingRecord> voiceItemSetNames, List<SMSInvoiceBillingRecord> smsItemSetNames,  string partnerType, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            SupplierInvoiceDetails supplierInvoiceDetails = null;
            if (partnerType != null)
            {
                supplierInvoiceDetails = new SupplierInvoiceDetails();
                supplierInvoiceDetails.PartnerType = partnerType;
                if (voiceItemSetNames != null && voiceItemSetNames.Count > 0)
                {
                    foreach (var invoiceBillingRecord in voiceItemSetNames)
                    {
                        supplierInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.CostDuration;
                        supplierInvoiceDetails.CostAmount += invoiceBillingRecord.InvoiceMeasures.CostNet;
                        supplierInvoiceDetails.OriginalCostAmount += invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                        supplierInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                        supplierInvoiceDetails.OriginalSupplierCurrencyId = invoiceBillingRecord.OriginalSupplierCurrencyId;
                        supplierInvoiceDetails.SupplierCurrencyId = invoiceBillingRecord.SupplierCurrencyId;
                        supplierInvoiceDetails.AmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
                        supplierInvoiceDetails.OriginalAmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;

                    }
                 
                }

                if (smsItemSetNames != null && smsItemSetNames.Count > 0)
                {
                    foreach (var invoiceBillingRecord in smsItemSetNames)
                    {
                        supplierInvoiceDetails.TotalSMSAmount += invoiceBillingRecord.InvoiceMeasures.CostNet;
                        supplierInvoiceDetails.TotalNumberOfSMS += invoiceBillingRecord.InvoiceMeasures.NumberOfSMS;
                        supplierInvoiceDetails.OriginalSupplierCurrencyId = invoiceBillingRecord.OriginalSupplierCurrencyId;
                        supplierInvoiceDetails.SupplierCurrencyId = invoiceBillingRecord.SupplierCurrencyId;
                        supplierInvoiceDetails.SMSAmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
                        supplierInvoiceDetails.SMSOriginalAmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;

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

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(List<InvoiceBillingRecord> voiceItemSetNames, List<SMSInvoiceBillingRecord> smsItemSetNames, IEnumerable<VRTaxItemDetail> taxItemDetails, List<SupplierInvoiceBySaleCurrencyItemDetails> supplierVoiceInvoicesBySaleCurrency, List<SupplierSMSInvoiceBySaleCurrencyItemDetails> supplierSMSInvoicesBySaleCurrency, List<RecurringChargeItem> supplierRecurringCharges)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            if (supplierVoiceInvoicesBySaleCurrency != null && supplierVoiceInvoicesBySaleCurrency.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupingByCurrency",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var supplierInvoiceBySaleCurrency in supplierVoiceInvoicesBySaleCurrency)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierInvoiceBySaleCurrency,
                        Name = " "
                    });
                }

                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

            }

            if(supplierSMSInvoicesBySaleCurrency!=null && supplierSMSInvoicesBySaleCurrency.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "SMSGroupingByCurrency",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var supplierSMSInvoiceBySaleCurrency in supplierSMSInvoicesBySaleCurrency)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierSMSInvoiceBySaleCurrency,
                        Name = " "
                    });
                }

                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
            }

            if (voiceItemSetNames != null && voiceItemSetNames.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupedByCostZone",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var item in voiceItemSetNames)
                {
                    SupplierInvoiceItemDetails supplierInvoiceItemDetails = new Entities.SupplierInvoiceItemDetails()
                    {
                        Duration = item.InvoiceMeasures.CostDuration,
                        NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
                        OriginalSupplierCurrencyId = item.OriginalSupplierCurrencyId,
                        OriginalCostAmount = item.InvoiceMeasures.CostNet_OrigCurr,
                        CostAmount = item.InvoiceMeasures.CostNet,
                        SupplierZoneId = item.SupplierZoneId,
                        SupplierId = item.SupplierId,
                        SupplierCurrencyId = item.SupplierCurrencyId,
                        SupplierRate = item.SupplierRate,
                        SupplierRateTypeId = item.SupplierRateTypeId,
                        FromDate = item.InvoiceMeasures.BillingPeriodFrom,
                        ToDate = item.InvoiceMeasures.BillingPeriodTo,
                        AmountAfterCommission = item.InvoiceMeasures.AmountAfterCommission,
                        OriginalAmountAfterCommission = item.InvoiceMeasures.OriginalAmountAfterCommission,
                        AmountAfterCommissionWithTaxes = item.InvoiceMeasures.AmountAfterCommissionWithTaxes,
                        OriginalAmountAfterCommissionWithTaxes = item.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes,
                        OriginalSupplierAmountWithTaxes = item.InvoiceMeasures.CostNet_OrigCurrWithTaxes,
                        SupplierAmountWithTaxes = item.InvoiceMeasures.CostNetWithTaxes,
                        CostDealZoneGroupNb = item.CostDealZoneGroupNb,
                        CostDealTierNb = item.CostDealTierNb,
                        CostDeal = item.CostDeal,
                        CostDealRateTierNb = item.CostDealRateTierNb,
                    };
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierInvoiceItemDetails,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }

            if(smsItemSetNames!=null && smsItemSetNames.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupedByCostMobileNetwork",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var item in smsItemSetNames)
                {
                    SupplierSMSInvoiceItemDetails supplierInvoiceItemDetails = new Entities.SupplierSMSInvoiceItemDetails()
                    {
                        NumberOfSMS = item.InvoiceMeasures.NumberOfSMS,
                        OriginalSupplierCurrencyId = item.OriginalSupplierCurrencyId,
                        OriginalCostAmount = item.InvoiceMeasures.CostNet_OrigCurr,
                        CostAmount = item.InvoiceMeasures.CostNet,
                        SupplierMobileNetworkId = item.SupplierMobileNetworkId,
                        SupplierId = item.SupplierId,
                        SupplierCurrencyId = item.SupplierCurrencyId,
                        SupplierRate = item.SupplierRate,
                        FromDate = item.InvoiceMeasures.BillingPeriodFrom,
                        ToDate = item.InvoiceMeasures.BillingPeriodTo,
                        AmountAfterCommission = item.InvoiceMeasures.AmountAfterCommission,
                        OriginalAmountAfterCommission = item.InvoiceMeasures.OriginalAmountAfterCommission,
                        AmountAfterCommissionWithTaxes = item.InvoiceMeasures.AmountAfterCommissionWithTaxes,
                        OriginalAmountAfterCommissionWithTaxes = item.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes,
                        OriginalSupplierAmountWithTaxes = item.InvoiceMeasures.CostNet_OrigCurrWithTaxes,
                        SupplierAmountWithTaxes = item.InvoiceMeasures.CostNetWithTaxes,
                    };
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierInvoiceItemDetails,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }
            if (generatedInvoiceItemSets.Count > 0)
            {
                if (taxItemDetails != null && taxItemDetails.Count() > 0)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                    {
                        SetName = "Taxes",
                        Items = new List<GeneratedInvoiceItem>()
                    };
                    foreach (var item in taxItemDetails)
                    {
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = item,
                            Name = " "
                        });
                    }
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }
            if (supplierRecurringCharges != null && supplierRecurringCharges.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "RecurringCharge",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var supplierRecurringCharge in supplierRecurringCharges)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierRecurringCharge,
                        Name = " "
                    });
                }
                generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
            }

            return generatedInvoiceItemSets;
        }
        private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredRecords(List<string> listDimensions, List<string> listMeasures, string dimentionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, Guid analytictableId, int? currencyId,TimeSpan? offset)
        {
            AnalyticManager analyticManager = new AnalyticManager();

            Dictionary<string, dynamic> queryParameters = null;
            if(offset.HasValue)
            {
                queryParameters = new Dictionary<string, dynamic>();
                queryParameters.Add("BatchStart_TimeShift", offset.Value);
            }
            Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
            {
                Query = new AnalyticQuery()
                {
                    DimensionFields = listDimensions,
                    MeasureFields = listMeasures,
                    TableId = analytictableId,
                    FromTime = fromDate,
                    ToTime = toDate,
                    ParentDimensions = new List<string>(),
                    Filters = new List<DimensionFilter>(),
                    CurrencyId = currencyId,
                    QueryParameters = queryParameters
                    //  OrderType = AnalyticQueryOrderType.ByAllDimensions
                },
                SortByColumnName = "DimensionValues[0].Name"
            };
            DimensionFilter dimensionFilter = new DimensionFilter()
            {
                Dimension = dimentionFilterName,
                FilterValues = new List<object> { dimentionFilterValue }
            };
            analyticQuery.Query.Filters.Add(dimensionFilter);
            return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
        }
        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private void ConvertAnalyticDataToList(IEnumerable<AnalyticRecord> voiceAnalyticRecords, IEnumerable<AnalyticRecord> smsAnalyticRecords, int currencyId, List<InvoiceBillingRecord> voiceItemSetNames, List<SMSInvoiceBillingRecord> smsItemSetNames, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
        {
            if (voiceAnalyticRecords != null && voiceAnalyticRecords.Count()>0)
            {
                foreach (var analyticRecord in voiceAnalyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue supplierZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue supplierId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue supplierCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue supplierRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
                    DimensionValue supplierRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);

                    DimensionValue costDealZoneGroupNb = analyticRecord.DimensionValues.ElementAtOrDefault(5);
                    DimensionValue costDealTierNb = analyticRecord.DimensionValues.ElementAtOrDefault(6);
                    DimensionValue costDeal = analyticRecord.DimensionValues.ElementAtOrDefault(7);
                    DimensionValue costDealRateTierNb = analyticRecord.DimensionValues.ElementAtOrDefault(8);


                    MeasureValue costNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue costDuration = GetMeasureValue(analyticRecord, "CostDuration");
                    MeasureValue costNet = GetMeasureValue(analyticRecord, "CostNetNotNULL");
                    MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion

                    var costNetValue = Convert.ToDecimal(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                    if (costNetValue != 0)
                    {
                        InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                        {
                            SupplierId = Convert.ToInt32(supplierId.Value),
                            SupplierCurrencyId = currencyId,
                            OriginalSupplierCurrencyId = Convert.ToInt32(supplierCurrencyId.Value),
                            SupplierRate = supplierRate != null ? Convert.ToDecimal(supplierRate.Value) : default(Decimal),
                            SupplierRateTypeId = supplierRateTypeId != null && supplierRateTypeId.Value != null ? Convert.ToInt32(supplierRateTypeId.Value) : default(int?),
                            SupplierZoneId = Convert.ToInt64(supplierZoneId.Value),
                            InvoiceMeasures = new InvoiceMeasures
                            {
                                CostDuration = Convert.ToDecimal(costDuration.Value ?? 0.0),
                                CostNet = costNetValue,
                                NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
                                CostNet_OrigCurr = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0),
                            },
                            CostDeal = costDeal != null ? Convert.ToInt32(costDeal.Value) : default(int?),
                            CostDealRateTierNb = costDealRateTierNb != null ? Convert.ToDecimal(costDealRateTierNb.Value) : default(Decimal?),
                            CostDealZoneGroupNb = costDealZoneGroupNb != null ? Convert.ToInt32(costDealZoneGroupNb.Value) : default(int?),
                            CostDealTierNb = costDealTierNb != null ? Convert.ToInt32(costDealTierNb.Value) : default(int?),
                        };


                        if (billingPeriodFrom != null)
                        {
                            var originalBillingPeriodFromDate = Convert.ToDateTime(billingPeriodFrom.Value);
                            var originalBillingPeriodToDate = Convert.ToDateTime(billingPeriodTo.Value);
                            var billingPeriodFromDate = originalBillingPeriodFromDate;
                            var billingPeriodToDate = originalBillingPeriodToDate;
                            if (offsetValue.HasValue)
                            {
                                billingPeriodFromDate = billingPeriodFromDate.Add(offsetValue.Value);
                                billingPeriodToDate = billingPeriodToDate.Add(offsetValue.Value);
                            }

                            invoiceBillingRecord.InvoiceMeasures.OriginalBillingPeriodFrom = originalBillingPeriodFromDate;
                            invoiceBillingRecord.InvoiceMeasures.OriginalBillingPeriodTo = originalBillingPeriodToDate;
                            invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom = billingPeriodFromDate;
                            invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo = billingPeriodToDate;
                        }


                        if (commission.HasValue)
                        {
                            if (commissionType.HasValue && commissionType.Value == CommissionType.DoNotDisplay)
                            {
                                invoiceBillingRecord.SupplierRate = invoiceBillingRecord.SupplierRate + ((invoiceBillingRecord.SupplierRate * commission.Value) / 100);
                            }
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * commission.Value) / 100);
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet + ((invoiceBillingRecord.InvoiceMeasures.CostNet * commission.Value) / 100);
                        }
                        else
                        {
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet;
                        }

                        invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
                        invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
                        invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurrWithTaxes = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                        invoiceBillingRecord.InvoiceMeasures.CostNetWithTaxes = invoiceBillingRecord.InvoiceMeasures.CostNet;

                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurrWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.CostNetWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.CostNet * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }

                        voiceItemSetNames.Add(invoiceBillingRecord);

                    }

                }
            }

            if(smsAnalyticRecords != null && smsAnalyticRecords.Count() > 0)
            {
                foreach (var analyticRecord in smsAnalyticRecords)
                {

                    #region ReadDataFromAnalyticResult
                    DimensionValue supplierMobileNetworkId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
                    DimensionValue supplierId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
                    DimensionValue supplierCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
                    DimensionValue supplierRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);


                    MeasureValue costNet_OrigCurr = GetMeasureValue(analyticRecord, "CostNet_OrigCurr");
                    MeasureValue costNet = GetMeasureValue(analyticRecord, "CostNetNotNULL");
                    MeasureValue sms = GetMeasureValue(analyticRecord, "NumberOfSMS");
                    MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
                    MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
                    #endregion

                    var costNetValue = Convert.ToDecimal(costNet == null ? 0.0 : costNet.Value ?? 0.0);
                    if (costNetValue != 0)
                    {
                        SMSInvoiceBillingRecord invoiceBillingRecord = new SMSInvoiceBillingRecord
                        {
                            SupplierId = Convert.ToInt32(supplierId.Value),
                            SupplierMobileNetworkId = Convert.ToInt64(supplierMobileNetworkId.Value),
                            SupplierCurrencyId = currencyId,
                            OriginalSupplierCurrencyId = Convert.ToInt32(supplierCurrencyId.Value),
                            SupplierRate = supplierRate != null ? Convert.ToDecimal(supplierRate.Value) : default(Decimal),
                            InvoiceMeasures = new SMSInvoiceMeasures
                            {
                                CostNet = costNetValue,
                                NumberOfSMS = Convert.ToInt32(sms.Value ?? 0.0),
                                CostNet_OrigCurr = Convert.ToDecimal(costNet_OrigCurr == null ? 0.0 : costNet_OrigCurr.Value ?? 0.0),
                            }
                        };


                        if (billingPeriodFrom != null)
                        {
                            var originalBillingPeriodFromDate = Convert.ToDateTime(billingPeriodFrom.Value);
                            var originalBillingPeriodToDate = Convert.ToDateTime(billingPeriodTo.Value);
                            var billingPeriodFromDate = originalBillingPeriodFromDate;
                            var billingPeriodToDate = originalBillingPeriodToDate;
                            if (offsetValue.HasValue)
                            {
                                billingPeriodFromDate = billingPeriodFromDate.Add(offsetValue.Value);
                                billingPeriodToDate = billingPeriodToDate.Add(offsetValue.Value);
                            }

                            invoiceBillingRecord.InvoiceMeasures.OriginalBillingPeriodFrom = originalBillingPeriodFromDate;
                            invoiceBillingRecord.InvoiceMeasures.OriginalBillingPeriodTo = originalBillingPeriodToDate;
                            invoiceBillingRecord.InvoiceMeasures.BillingPeriodFrom = billingPeriodFromDate;
                            invoiceBillingRecord.InvoiceMeasures.BillingPeriodTo = billingPeriodToDate;
                        }


                        if (commission.HasValue)
                        {
                            if (commissionType.HasValue && commissionType.Value == CommissionType.DoNotDisplay)
                            {
                                invoiceBillingRecord.SupplierRate = invoiceBillingRecord.SupplierRate + ((invoiceBillingRecord.SupplierRate * commission.Value) / 100);
                            }
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * commission.Value) / 100);
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet + ((invoiceBillingRecord.InvoiceMeasures.CostNet * commission.Value) / 100);
                        }
                        else
                        {
                            invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                            invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.CostNet;
                        }

                        invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
                        invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
                        invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurrWithTaxes = invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr;
                        invoiceBillingRecord.InvoiceMeasures.CostNetWithTaxes = invoiceBillingRecord.InvoiceMeasures.CostNet;

                        if (taxItemDetails != null)
                        {
                            foreach (var tax in taxItemDetails)
                            {
                                invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurrWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.CostNet_OrigCurr * Convert.ToDecimal(tax.Value)) / 100);

                                invoiceBillingRecord.InvoiceMeasures.CostNetWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.CostNet * Convert.ToDecimal(tax.Value)) / 100);
                            }
                        }

                        smsItemSetNames.Add(invoiceBillingRecord);

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


        public class InvoiceMeasures
        {
            public decimal CostNet { get; set; }
            public decimal CostNetWithTaxes { get; set; }
            public decimal CostNet_OrigCurr { get; set; }
            public decimal CostNet_OrigCurrWithTaxes { get; set; }
            public int NumberOfCalls { get; set; }
            public Decimal CostDuration { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }
            public DateTime OriginalBillingPeriodTo { get; set; }
            public DateTime OriginalBillingPeriodFrom { get; set; }
            public decimal AmountAfterCommission { get; set; }
            public decimal OriginalAmountAfterCommission { get; set; }
            public decimal AmountAfterCommissionWithTaxes { get; set; }
            public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }

        }
        public class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long SupplierZoneId { get; set; }
            public int SupplierId { get; set; }
            public int OriginalSupplierCurrencyId { get; set; }
            public Decimal SupplierRate { get; set; }
            public int? SupplierRateTypeId { get; set; }
            public int SupplierCurrencyId { get; set; }
            public int? CostDealZoneGroupNb { get; set; }
            public int? CostDealTierNb { get; set; }
            public int? CostDeal { get; set; }
            public Decimal? CostDealRateTierNb { get; set; }

        }

        public class SMSInvoiceMeasures
        {
            public decimal CostNet { get; set; }
            public decimal CostNetWithTaxes { get; set; }
            public decimal CostNet_OrigCurr { get; set; }
            public decimal CostNet_OrigCurrWithTaxes { get; set; }
            public int NumberOfSMS { get; set; }
            public DateTime BillingPeriodTo { get; set; }
            public DateTime BillingPeriodFrom { get; set; }
            public DateTime OriginalBillingPeriodTo { get; set; }
            public DateTime OriginalBillingPeriodFrom { get; set; }
            public decimal AmountAfterCommission { get; set; }
            public decimal OriginalAmountAfterCommission { get; set; }
            public decimal AmountAfterCommissionWithTaxes { get; set; }
            public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }

        }
        public class SMSInvoiceBillingRecord
        {
            public SMSInvoiceMeasures InvoiceMeasures { get; set; }
            public long SupplierMobileNetworkId { get; set; }
            public int SupplierId { get; set; }
            public int OriginalSupplierCurrencyId { get; set; }
            public Decimal SupplierRate { get; set; }
            public int? SupplierRateTypeId { get; set; }
            public int SupplierCurrencyId { get; set; }
        }
    }
}
