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
using Vanrise.Invoice.Business;
using Vanrise.Security.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
	public class CustomerInvoiceGenerator : InvoiceGenerator
	{
		#region Local Variables
		PartnerManager _partnerManager = new PartnerManager();
		WHSFinancialAccountManager _financialAccountManager = new WHSFinancialAccountManager();
		TOneModuleManager _tOneModuleManager = new TOneModuleManager();
		#endregion

		#region Public Methods
		public override void GenerateInvoice(IInvoiceGenerationContext context)
		{
			var financialAccount = _financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));
			var customerGenerationCustomSectionPayload = context.CustomSectionPayload as CustomerGenerationCustomSectionPayload;
			int? timeZoneId = null;
			decimal? commission = null;
			CommissionType? commissionType = null;
			if (customerGenerationCustomSectionPayload != null)
			{
				timeZoneId = customerGenerationCustomSectionPayload.TimeZoneId;
				if (customerGenerationCustomSectionPayload.Commission.HasValue)
				{
					commission = customerGenerationCustomSectionPayload.Commission.Value;
					commissionType = customerGenerationCustomSectionPayload.CommissionType;
				}

			}
			if (!timeZoneId.HasValue)
			{
				timeZoneId = _financialAccountManager.GetCustomerTimeZoneId(financialAccount.FinancialAccountId);
			}

			string offset = null;
			TimeSpan? offsetValue = null;
			DateTime fromDate = context.FromDate;
			DateTime toDate = context.ToDate;
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



			string dimensionName = "SaleFinancialAccount";
			int dimensionValue = financialAccount.FinancialAccountId;
			bool isVoiceEnabled = _tOneModuleManager.IsVoiceModuleEnabled();
			bool isSMSEnabled = _tOneModuleManager.IsSMSModuleEnabled();
            bool canGenerateVoiceInvoice = false;

			//common
			Dictionary<string, List<SMSInvoiceBillingRecord>> smsItemSetNamesDic = null;
			Dictionary<string, List<InvoiceBillingRecord>> voiceItemSetNamesDic = null;
			CustomerRecurringChargeManager customerRecurringChargeManager = new CustomerRecurringChargeManager();
			List<RecurringChargeItem> evaluatedCustomerRecurringCharges = customerRecurringChargeManager.GetEvaluatedRecurringCharges(financialAccount.FinancialAccountId, fromDate, toDate, context.IssueDate);

			decimal? minAmount = _partnerManager.GetPartnerMinAmount(context.InvoiceTypeId, context.PartnerId);

            List<CustomerInvoiceBySaleCurrencyItemDetails> customerVoiceInvoiceBySaleCurrency = null; 
			if (isVoiceEnabled)
			{
                canGenerateVoiceInvoice = CheckUnpricedCDRs(context, financialAccount);
                if (canGenerateVoiceInvoice)
                {
                    List<string> listMeasures = new List<string> { "SaleNetNotNULL", "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
                    List<string> listDimensions = new List<string> { "SaleZone", "Customer", "SaleCurrency", "SaleRate", "SaleRateType", "Supplier", "Country", "SupplierZone", "SaleDealZoneGroupNb", "SaleDealTierNb", "SaleDeal", "SaleDealRateTierNb" };

                    var analyticResult = GetFilteredVoiceAnalyticRecords(listDimensions, listMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId, offsetValue);

                    if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() > 0)
                    {
                        voiceItemSetNamesDic = PrepareVoiceItemSetNames(analyticResult.Data, currencyId, commission, commissionType, taxItemDetails, offsetValue);
                    }
                    customerVoiceInvoiceBySaleCurrency = loadVoiceCurrencyItemSet(dimensionName, dimensionValue, fromDate, toDate, commission, commissionType, taxItemDetails, offsetValue);
                    if (customerVoiceInvoiceBySaleCurrency == null)
                        customerVoiceInvoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>();
                    AddRecurringChargeToCustomerCurrency(customerVoiceInvoiceBySaleCurrency, evaluatedCustomerRecurringCharges);
                }
			}

            List<CustomerSMSInvoiceBySaleCurrencyItemDetails> customerSMSInvoiceBySaleCurrency = null; 
            if (isSMSEnabled)
			{
				List<string> listMeasures = new List<string> { "NumberOfSMS", "BillingPeriodFrom", "BillingPeriodTo", "SaleNetNotNULL", "SaleNet_OrigCurr" };
				List<string> listDimensions = new List<string> { "OriginationMobileNetwork", "Customer", "SaleCurrency", "SaleRate", "Supplier", "OriginationMobileCountry", "DestinationMobileNetwork" };

				var analyticResult = GetFilteredSMSAnalyticRecords(listDimensions, listMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId, offsetValue);
				if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() > 0)
				{
					smsItemSetNamesDic = PrepareSMSItemSetNames(analyticResult.Data, currencyId, commission, commissionType, taxItemDetails, offsetValue);
				}
                customerSMSInvoiceBySaleCurrency = loadSMSCurrencyItemSet(dimensionName, dimensionValue, fromDate, toDate, commission, commissionType, taxItemDetails, offsetValue);
                if (customerSMSInvoiceBySaleCurrency == null)
                    customerSMSInvoiceBySaleCurrency = new List<CustomerSMSInvoiceBySaleCurrencyItemDetails>();
                AddSMSRecurringChargeToCustomerCurrency(customerSMSInvoiceBySaleCurrency, evaluatedCustomerRecurringCharges);
			}

			if (((smsItemSetNamesDic == null || smsItemSetNamesDic.Count == 0) && ((voiceItemSetNamesDic == null || voiceItemSetNamesDic.Count == 0))) && (evaluatedCustomerRecurringCharges == null || evaluatedCustomerRecurringCharges.Count == 0))
			{
				context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
				return;
			}
			if (taxItemDetails != null)
			{
				foreach (var tax in taxItemDetails)
				{
					if (evaluatedCustomerRecurringCharges != null)
					{
						foreach (var item in evaluatedCustomerRecurringCharges)
						{
							item.AmountAfterTaxes += ((item.Amount * Convert.ToDecimal(tax.Value)) / 100);
							item.VAT = tax.IsVAT ? tax.Value : 0;
						}
					}
				}
			}
			List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(voiceItemSetNamesDic, smsItemSetNamesDic, taxItemDetails, customerVoiceInvoiceBySaleCurrency, customerSMSInvoiceBySaleCurrency, evaluatedCustomerRecurringCharges, canGenerateVoiceInvoice);

			#region BuildCustomerInvoiceDetails
			CustomerInvoiceDetails customerInvoiceDetails = BuilCustomerInvoiceDetails(voiceItemSetNamesDic, smsItemSetNamesDic, financialAccount.CarrierProfileId.HasValue ? "Profile" : "Account", context.FromDate, context.ToDate, commission, commissionType, minAmount, canGenerateVoiceInvoice);

			if (customerInvoiceDetails != null)
			{
				customerInvoiceDetails.TimeZoneId = timeZoneId;
				customerInvoiceDetails.TotalAmount = customerInvoiceDetails.SaleAmount;
				customerInvoiceDetails.TotalAmountAfterCommission = customerInvoiceDetails.AmountAfterCommission;
				customerInvoiceDetails.TotalSMSAmountAfterCommission = customerInvoiceDetails.SMSAmountAfterCommission;
				customerInvoiceDetails.TotalOriginalAmountAfterCommission = customerInvoiceDetails.OriginalAmountAfterCommission;
				customerInvoiceDetails.TotalSMSOriginalAmountAfterCommission = customerInvoiceDetails.SMSOriginalAmountAfterCommission;

				customerInvoiceDetails.Commission = commission;
				customerInvoiceDetails.CommissionType = commissionType;
				customerInvoiceDetails.Offset = offset;

				customerInvoiceDetails.TotalAmountAfterCommission = customerInvoiceDetails.AmountAfterCommission;
				customerInvoiceDetails.TotalOriginalAmountAfterCommission = customerInvoiceDetails.OriginalAmountAfterCommission;
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
					}

					context.ActionAfterGenerateInvoice = (invoice) =>
					{
						CustomerBillingRecurringChargeManager customerBillingRecurringChargeManager = new CustomerBillingRecurringChargeManager();
						var userId = SecurityContext.Current.GetLoggedInUserId();
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
						return true;
					};
				}

				CurrencyManager currencyManager = new CurrencyManager();
				var systemCurrency = currencyManager.GetSystemCurrency();
				systemCurrency.ThrowIfNull("systemCurrency");
				CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
				decimal totalAmountAfterCommissionInSystemCurrency = currencyExchangeRateManager.ConvertValueToCurrency(customerInvoiceDetails.TotalAmountAfterCommission, currencyId, systemCurrency.CurrencyId, context.IssueDate);
				decimal totalSMSAmountAfterCommissionInSystemCurrency = currencyExchangeRateManager.ConvertValueToCurrency(customerInvoiceDetails.TotalSMSAmountAfterCommission, currencyId, systemCurrency.CurrencyId, context.IssueDate);

				decimal totalReccurringChargesInSystemCurrency = 0;
				foreach (var item in evaluatedCustomerRecurringCharges)
				{
					totalReccurringChargesInSystemCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, systemCurrency.CurrencyId, context.IssueDate);
				}
				var totalAmountInSystemCurrency = totalReccurringChargesInSystemCurrency + totalAmountAfterCommissionInSystemCurrency + totalSMSAmountAfterCommissionInSystemCurrency;

				if ((minAmount.HasValue && totalAmountInSystemCurrency >= minAmount.Value) || (!minAmount.HasValue && totalAmountInSystemCurrency != 0))
				{
					var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
					definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
					definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
					var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == context.InvoiceTypeId);
					financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");
					if (!financialAccountInvoiceType.IgnoreFromBalance)
					{
						SetInvoiceBillingTransactions(context, customerInvoiceDetails, financialAccount, fromDate, toDateForBillingTransaction);
					}

					ConfigManager configManager = new ConfigManager();
					InvoiceTypeSetting settings = configManager.GetInvoiceTypeSettingsById(context.InvoiceTypeId);
					if (settings != null)
					{
						context.NeedApproval = settings.NeedApproval;
					}

					decimal totalReccurringChargesAfterTaxInAccountCurrency = 0;
					decimal totalReccurringChargesInAccountCurrency = 0;

					foreach (var item in evaluatedCustomerRecurringCharges)
					{
						totalReccurringChargesAfterTaxInAccountCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, currencyId, context.IssueDate);
						totalReccurringChargesInAccountCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.Amount, item.CurrencyId, currencyId, context.IssueDate);
					}
					customerInvoiceDetails.TotalReccurringChargesAfterTax = totalReccurringChargesAfterTaxInAccountCurrency;
					customerInvoiceDetails.TotalReccurringCharges = totalReccurringChargesInAccountCurrency;

					customerInvoiceDetails.TotalInvoiceAmount = customerInvoiceDetails.TotalAmountAfterCommission + customerInvoiceDetails.TotalReccurringChargesAfterTax + customerInvoiceDetails.TotalSMSAmountAfterCommission;
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
		private void AddRecurringChargeToCustomerCurrency(List<CustomerInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencyItemDetails, List<RecurringChargeItem> recurringChargeItems)
		{
			if (recurringChargeItems != null && recurringChargeItems.Count > 0)
			{
				if (customerInvoiceBySaleCurrencyItemDetails == null)
					customerInvoiceBySaleCurrencyItemDetails = new List<CustomerInvoiceBySaleCurrencyItemDetails>();

				foreach (var item in recurringChargeItems)
				{
					var customerInvoiceBySaleCurrencyItemDetail = customerInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
					if (customerInvoiceBySaleCurrencyItemDetail != null)
					{
						customerInvoiceBySaleCurrencyItemDetail.Amount += item.Amount;
						customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.Amount;
						customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.AmountAfterTaxes;
						customerInvoiceBySaleCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
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
		private void AddSMSRecurringChargeToCustomerCurrency(List<CustomerSMSInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencyItemDetails, List<RecurringChargeItem> recurringChargeItems)
		{
			if (recurringChargeItems != null && recurringChargeItems.Count > 0)
			{
				if (customerInvoiceBySaleCurrencyItemDetails == null)
					customerInvoiceBySaleCurrencyItemDetails = new List<CustomerSMSInvoiceBySaleCurrencyItemDetails>();

				foreach (var item in recurringChargeItems)
				{
					var customerInvoiceBySaleCurrencyItemDetail = customerInvoiceBySaleCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
					if (customerInvoiceBySaleCurrencyItemDetail != null)
					{
						customerInvoiceBySaleCurrencyItemDetail.Amount += item.Amount;
						customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommission += item.Amount;
						customerInvoiceBySaleCurrencyItemDetail.AmountAfterCommissionWithTaxes += item.AmountAfterTaxes;
						customerInvoiceBySaleCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
					}
					else
					{
						customerInvoiceBySaleCurrencyItemDetails.Add(new CustomerSMSInvoiceBySaleCurrencyItemDetails
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

		private List<CustomerInvoiceBySaleCurrencyItemDetails> loadVoiceCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
		{
			List<string> listMeasures = new List<string> { "NumberOfCalls", "SaleDuration", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
			List<string> listDimensions = new List<string> { "SaleCurrency", "MonthQueryTimeShift" };
			var analyticResult = GetFilteredVoiceAnalyticRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, null, offsetValue);
			if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
			{
				return BuildCurrencyItemSetNameFromAnalytic(analyticResult.Data, commission, commissionType, taxItemDetails);
			}
			return null;
		}

		private List<CustomerSMSInvoiceBySaleCurrencyItemDetails> loadSMSCurrencyItemSet(string dimentionName, int dimensionValue, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
		{
			List<string> listMeasures = new List<string> { "NumberOfSMS", "BillingPeriodTo", "BillingPeriodFrom", "SaleNet_OrigCurr" };
			List<string> listDimensions = new List<string> { "SaleCurrency", "MonthQueryTimeShift" };
			var analyticResult = GetFilteredSMSAnalyticRecords(listDimensions, listMeasures, dimentionName, dimensionValue, fromDate, toDate, null, offsetValue);
			if (analyticResult != null && analyticResult.Data != null && analyticResult.Data.Count() != 0)
			{
				return BuildSMSCurrencyItemSetNameFromAnalytic(analyticResult.Data, commission, commissionType, taxItemDetails);
			}
			return null;
		}
		private List<CustomerInvoiceBySaleCurrencyItemDetails> BuildCurrencyItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
		{
			List<CustomerInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencies = null;

			if (analyticRecords != null )
			{
				customerInvoiceBySaleCurrencies = new List<CustomerInvoiceBySaleCurrencyItemDetails>();
				foreach (var analyticRecord in analyticRecords)
				{
					#region ReadDataFromAnalyticResult
					DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
					DimensionValue month = analyticRecord.DimensionValues.ElementAtOrDefault(1);

					MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "SaleNet_OrigCurr");
					MeasureValue saleDuration = GetMeasureValue(analyticRecord, "SaleDuration");
					MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
					MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
					MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
					#endregion

					var saleNetValue = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0);
					if (saleNetValue != 0)
					{
						var customerInvoiceBySaleCurrencyItemDetails = new CustomerInvoiceBySaleCurrencyItemDetails
						{
							CurrencyId = Convert.ToInt32(saleCurrencyId.Value),
							FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
							ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
							Duration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
							NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
							Amount = saleNetValue,
							Month = month.Value != null ? month.Value.ToString() : null
						};
						if (commission.HasValue)
						{
							customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.Amount + ((customerInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
						}
						else
						{
							customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.Amount;
						}

						customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;

						if (taxItemDetails != null)
						{
							foreach (var tax in taxItemDetails)
							{
								customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((customerInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
							}
						}
						customerInvoiceBySaleCurrencyItemDetails.TotalTrafficAmount = customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes;


						customerInvoiceBySaleCurrencies.Add(customerInvoiceBySaleCurrencyItemDetails);
					}

				}
			}
			return customerInvoiceBySaleCurrencies;
		}

		private List<CustomerSMSInvoiceBySaleCurrencyItemDetails> BuildSMSCurrencyItemSetNameFromAnalytic(IEnumerable<AnalyticRecord> analyticRecords, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails)
		{
			List<CustomerSMSInvoiceBySaleCurrencyItemDetails> customerInvoiceBySaleCurrencies = null;

			if (analyticRecords != null)
			{
				customerInvoiceBySaleCurrencies = new List<CustomerSMSInvoiceBySaleCurrencyItemDetails>();
				foreach (var analyticRecord in analyticRecords)
				{
					#region ReadDataFromAnalyticResult
					DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
					DimensionValue month = analyticRecord.DimensionValues.ElementAtOrDefault(1);

					MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "SaleNet_OrigCurr");
					MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfSMS");
					MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
					MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
					#endregion

					var saleNetValue = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0);
					if (saleNetValue != 0)
					{
						var customerInvoiceBySaleCurrencyItemDetails = new CustomerSMSInvoiceBySaleCurrencyItemDetails
						{
							CurrencyId = Convert.ToInt32(saleCurrencyId.Value),
							FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
							ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
							NumberOfSMS = Convert.ToInt32(calls.Value ?? 0.0),
							Amount = saleNetValue,
							Month = month.Value != null ? month.Value.ToString() : null
						};
						if (commission.HasValue)
						{
							customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.Amount + ((customerInvoiceBySaleCurrencyItemDetails.Amount * commission.Value) / 100);
						}
						else
						{
							customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission = customerInvoiceBySaleCurrencyItemDetails.Amount;
						}

						customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes = customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommission;

						if (taxItemDetails != null)
						{
							foreach (var tax in taxItemDetails)
							{
								customerInvoiceBySaleCurrencyItemDetails.AmountAfterCommissionWithTaxes += ((customerInvoiceBySaleCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
							}
						}
						customerInvoiceBySaleCurrencies.Add(customerInvoiceBySaleCurrencyItemDetails);
					}
				}
			}
			return customerInvoiceBySaleCurrencies;
		}


		private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, CustomerInvoiceDetails invoiceDetails, WHSFinancialAccount financialAccount, DateTime fromDate, DateTime toDate)
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
					Amount = invoiceDetails.TotalAmountAfterCommission,
					CurrencyId = invoiceDetails.SaleCurrencyId,
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
		private CustomerInvoiceDetails BuilCustomerInvoiceDetails(Dictionary<string, List<InvoiceBillingRecord>> voiceItemSetNamesDic, Dictionary<string, List<SMSInvoiceBillingRecord>> smsItemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate, decimal? commission, CommissionType? commissionType, decimal? minAmount, bool canGenerateVoiceInvoice)
		{
			CurrencyManager currencyManager = new CurrencyManager();
			CustomerInvoiceDetails customerInvoiceDetails = null;
			if (partnerType != null)
			{
				if (voiceItemSetNamesDic != null && canGenerateVoiceInvoice)
				{
                    customerInvoiceDetails = new CustomerInvoiceDetails()
                    {
                        PartnerType = partnerType
                    };
                    List<InvoiceBillingRecord> invoiceBillingRecordList = null;
					if (voiceItemSetNamesDic.TryGetValue("GroupedBySaleZone", out invoiceBillingRecordList))
					{
						foreach (var invoiceBillingRecord in invoiceBillingRecordList)
						{
							customerInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.SaleDuration;
							customerInvoiceDetails.SaleAmount += invoiceBillingRecord.InvoiceMeasures.SaleNet;
							customerInvoiceDetails.OriginalSaleAmount += invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
							customerInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
							customerInvoiceDetails.OriginalSaleCurrencyId = invoiceBillingRecord.OriginalSaleCurrencyId;
							customerInvoiceDetails.SaleCurrencyId = invoiceBillingRecord.SaleCurrencyId;
							customerInvoiceDetails.CountryId = invoiceBillingRecord.CountryId;
							customerInvoiceDetails.SupplierId = invoiceBillingRecord.SupplierId;
							customerInvoiceDetails.SupplierZoneId = invoiceBillingRecord.SupplierZoneId;
							customerInvoiceDetails.AmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
							customerInvoiceDetails.OriginalAmountAfterCommission += invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
						}

					};
				}


				if (smsItemSetNamesDic != null)
				{
                    if (customerInvoiceDetails == null)
                    {
                        customerInvoiceDetails = new CustomerInvoiceDetails()
                        {
                            PartnerType = partnerType
                        };
                    }
                    List<SMSInvoiceBillingRecord> smsInvoiceBillingRecordList = null;
					if (smsItemSetNamesDic.TryGetValue("GroupedByOriginationMobileNetwork", out smsInvoiceBillingRecordList))
					{
						foreach (var smsInvoiceBillingRecord in smsInvoiceBillingRecordList)
						{
							customerInvoiceDetails.TotalNumberOfSMS += smsInvoiceBillingRecord.InvoiceMeasures.NumberOfSMS;
							customerInvoiceDetails.OriginalSaleCurrencyId = smsInvoiceBillingRecord.OriginalSaleCurrencyId;
							customerInvoiceDetails.SaleCurrencyId = smsInvoiceBillingRecord.SaleCurrencyId;
							customerInvoiceDetails.CountryId = smsInvoiceBillingRecord.CountryId;
							customerInvoiceDetails.SupplierId = smsInvoiceBillingRecord.SupplierId;
							customerInvoiceDetails.SMSAmountAfterCommission += smsInvoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
							customerInvoiceDetails.SMSOriginalAmountAfterCommission += smsInvoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
						}

					};
				}

                if(customerInvoiceDetails != null)
                {
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
			}
			if (customerInvoiceDetails != null && ((minAmount.HasValue && customerInvoiceDetails.SaleAmount >= minAmount.Value) || (!minAmount.HasValue && customerInvoiceDetails.SaleAmount != 0)))
			{
				customerInvoiceDetails.OriginalSaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.OriginalSaleCurrencyId);
				customerInvoiceDetails.SaleCurrency = currencyManager.GetCurrencySymbol(customerInvoiceDetails.SaleCurrencyId);
			}
			return customerInvoiceDetails;
		}
		private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<InvoiceBillingRecord>> voiceItemSetNamesDic, Dictionary<string, List<SMSInvoiceBillingRecord>> smsItemSetNamesDic, IEnumerable<VRTaxItemDetail> taxItemDetails, List<CustomerInvoiceBySaleCurrencyItemDetails> customerVoiceInvoicesBySaleCurrency, List<CustomerSMSInvoiceBySaleCurrencyItemDetails> customerSMSInvoicesBySaleCurrency, List<RecurringChargeItem> customerRecurringCharges, bool canGenerateVoiceInvoice)
		{
			List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
			if (customerVoiceInvoicesBySaleCurrency != null && customerVoiceInvoicesBySaleCurrency.Count > 0 && canGenerateVoiceInvoice)
			{
				GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
				generatedInvoiceItemSet.SetName = "VoiceGroupingByCurrency";
				generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

				foreach (var customerInvoiceBySaleCurrency in customerVoiceInvoicesBySaleCurrency)
				{
					generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
					{
						Details = customerInvoiceBySaleCurrency,
						Name = " "
					});
				}
				generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
			}

			if (customerSMSInvoicesBySaleCurrency != null && customerSMSInvoicesBySaleCurrency.Count > 0)
			{
				GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
				generatedInvoiceItemSet.SetName = "SMSGroupingByCurrency";
				generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

				foreach (var customerSMSInvoiceBySaleCurrency in customerSMSInvoicesBySaleCurrency)
				{
					generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
					{
						Details = customerSMSInvoiceBySaleCurrency,
						Name = " "
					});
				}
				generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
			}

			if (voiceItemSetNamesDic != null && voiceItemSetNamesDic.Count>0 && canGenerateVoiceInvoice)
			{
				foreach (var itemSet in voiceItemSetNamesDic)
				{
					GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
					generatedInvoiceItemSet.SetName = itemSet.Key;
					var itemSetValues = itemSet.Value;
					generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

					foreach (var item in itemSetValues)
					{
						CustomerInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerInvoiceItemDetails()
						{
							Duration = item.InvoiceMeasures.SaleDuration,
							NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
							OriginalSaleCurrencyId = item.OriginalSaleCurrencyId,
							OriginalSaleAmount = item.InvoiceMeasures.SaleNet_OrigCurr,
							SaleAmount = item.InvoiceMeasures.SaleNet,
							SaleZoneId = item.SaleZoneId,
							CustomerId = item.CustomerId,
							SaleCurrencyId = item.SaleCurrencyId,
							SaleRate = item.SaleRate,
							SaleRateTypeId = item.SaleRateTypeId,
							FromDate = item.InvoiceMeasures.BillingPeriodFrom,
							ToDate = item.InvoiceMeasures.BillingPeriodTo,
							CountryId = item.CountryId,
							SupplierId = item.SupplierId,
							SupplierZoneId = item.SupplierZoneId,
							AmountAfterCommission = item.InvoiceMeasures.AmountAfterCommission,
							OriginalAmountAfterCommission = item.InvoiceMeasures.OriginalAmountAfterCommission,
							AmountAfterCommissionWithTaxes = item.InvoiceMeasures.AmountAfterCommissionWithTaxes,
							OriginalAmountAfterCommissionWithTaxes = item.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes,
							OriginalSaleAmountWithTaxes = item.InvoiceMeasures.SaleNet_OrigCurrWithTaxes,
							SaleAmountWithTaxes = item.InvoiceMeasures.SaleNetWithTaxes,
							SaleDealZoneGroupNb = item.SaleDealZoneGroupNb,
							SaleDealTierNb = item.SaleDealTierNb,
							SaleDeal = item.SaleDeal,
							SaleDealRateTierNb = item.SaleDealRateTierNb,

						};
						generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
						{
							Details = customerInvoiceItemDetails,
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
					if (taxItemDetails != null)
					{
						GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
						generatedInvoiceItemSet.SetName = "Taxes";
						generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
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
			}

			if (smsItemSetNamesDic != null && smsItemSetNamesDic.Count>0)
			{
				foreach (var itemSet in smsItemSetNamesDic)
				{
					GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
					generatedInvoiceItemSet.SetName = itemSet.Key;
					var itemSetValues = itemSet.Value;
					generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

					foreach (var item in itemSetValues)
					{
						CustomerSMSInvoiceItemDetails customerInvoiceItemDetails = new Entities.CustomerSMSInvoiceItemDetails()
						{
							NumberOfSMS = item.InvoiceMeasures.NumberOfSMS,
							OriginalSaleCurrencyId = item.OriginalSaleCurrencyId,
							OriginalSaleAmount = item.InvoiceMeasures.SaleNet_OrigCurr,
							SaleAmount = item.InvoiceMeasures.SaleNet,
							SaleZoneId = item.SaleZoneId,
							CustomerId = item.CustomerId,
							SaleCurrencyId = item.SaleCurrencyId,
							SaleRate = item.SaleRate,
							FromDate = item.InvoiceMeasures.BillingPeriodFrom,
							ToDate = item.InvoiceMeasures.BillingPeriodTo,
							CountryId = item.CountryId,
							SupplierId = item.SupplierId,
							CustomerMobileNetworkId = item.CustomerMobileNetworkId,
							//SupplierZoneId = item.SupplierZoneId,
							AmountAfterCommission = item.InvoiceMeasures.AmountAfterCommission,
							OriginalAmountAfterCommission = item.InvoiceMeasures.OriginalAmountAfterCommission,
							AmountAfterCommissionWithTaxes = item.InvoiceMeasures.AmountAfterCommissionWithTaxes,
							OriginalAmountAfterCommissionWithTaxes = item.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes,
							OriginalSaleAmountWithTaxes = item.InvoiceMeasures.SaleNet_OrigCurrWithTaxes,
							SaleAmountWithTaxes = item.InvoiceMeasures.SaleNetWithTaxes,
						};
						generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
						{
							Details = customerInvoiceItemDetails,
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
					if (taxItemDetails != null)
					{
						GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
						generatedInvoiceItemSet.SetName = "Taxes";
						generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
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
			}

			if (customerRecurringCharges != null && customerRecurringCharges.Count > 0)
			{
				GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
				generatedInvoiceItemSet.SetName = "RecurringCharge";
				generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

				foreach (var customerRecurringCharge in customerRecurringCharges)
				{
					generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
					{
						Details = customerRecurringCharge,
						Name = " "
					});
				}
				generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
			}

			return generatedInvoiceItemSets;
		}
		private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredVoiceAnalyticRecords(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, TimeSpan? offsetValue)
		{
			Dictionary<string, dynamic> queryParameters = null;
			if (offsetValue.HasValue)
			{
				queryParameters = new Dictionary<string, dynamic>();
				queryParameters.Add("BatchStart_TimeShift", offsetValue.Value);
			}
			AnalyticManager analyticManager = new AnalyticManager();
			Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
			{
				Query = new AnalyticQuery()
				{
					DimensionFields = listDimensions,
					MeasureFields = listMeasures,
					TableId = Guid.Parse("4C1AAA1B-675B-420F-8E60-26B0747CA79B"),
					FromTime = fromDate,
					ToTime = toDate,
					ParentDimensions = new List<string>(),
					Filters = new List<DimensionFilter>(),
					CurrencyId = currencyId,
					// OrderType =AnalyticQueryOrderType.ByAllDimensions,
					QueryParameters = queryParameters
				},
				SortByColumnName = "DimensionValues[0].Name"
			};
			DimensionFilter dimensionFilter = new DimensionFilter()
			{
				Dimension = dimensionFilterName,
				FilterValues = new List<object> { dimentionFilterValue }
			};
			analyticQuery.Query.Filters.Add(dimensionFilter);
			return analyticManager.GetFilteredRecords(analyticQuery) as Vanrise.Analytic.Entities.AnalyticSummaryBigResult<AnalyticRecord>;
		}
		private AnalyticSummaryBigResult<AnalyticRecord> GetFilteredSMSAnalyticRecords(List<string> listDimensions, List<string> listMeasures, string dimensionFilterName, object dimentionFilterValue, DateTime fromDate, DateTime toDate, int? currencyId, TimeSpan? offsetValue)
		{
			Dictionary<string, dynamic> queryParameters = null;
			if (offsetValue.HasValue)
			{
				queryParameters = new Dictionary<string, dynamic>();
				queryParameters.Add("BatchStart_TimeShift", offsetValue.Value);
			}
			AnalyticManager analyticManager = new AnalyticManager();
			Vanrise.Entities.DataRetrievalInput<AnalyticQuery> analyticQuery = new DataRetrievalInput<AnalyticQuery>()
			{

				Query = new AnalyticQuery()
				{
					DimensionFields = listDimensions,
					MeasureFields = listMeasures,
					TableId = Guid.Parse("53e9ebc8-c674-4aff-b6c0-9b3b18f95c1f"),
					FromTime = fromDate,
					ToTime = toDate,
					ParentDimensions = new List<string>(),
					Filters = new List<DimensionFilter>(),
					CurrencyId = currencyId,
					// OrderType =AnalyticQueryOrderType.ByAllDimensions,
					QueryParameters = queryParameters
				},
				SortByColumnName = "DimensionValues[0].Name"
			};
			DimensionFilter dimensionFilter = new DimensionFilter()
			{
				Dimension = dimensionFilterName,
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
		private Dictionary<string, List<InvoiceBillingRecord>> PrepareVoiceItemSetNames(IEnumerable<AnalyticRecord> analyticRecords, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
		{
			Dictionary<string, List<InvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<InvoiceBillingRecord>>();
			if (analyticRecords != null)
			{
				foreach (var analyticRecord in analyticRecords)
				{

					#region ReadDataFromAnalyticResult

					DimensionValue saleZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
					DimensionValue customerId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
					DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
					DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
					DimensionValue saleRateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);
					DimensionValue supplier = analyticRecord.DimensionValues.ElementAtOrDefault(5);
					DimensionValue country = analyticRecord.DimensionValues.ElementAtOrDefault(6);
					DimensionValue supplierZone = analyticRecord.DimensionValues.ElementAtOrDefault(7);

					DimensionValue saleDealZoneGroupNb = analyticRecord.DimensionValues.ElementAtOrDefault(8);
					DimensionValue saleDealTierNb = analyticRecord.DimensionValues.ElementAtOrDefault(9);
					DimensionValue saleDeal = analyticRecord.DimensionValues.ElementAtOrDefault(10);
					DimensionValue saleDealRateTierNb = analyticRecord.DimensionValues.ElementAtOrDefault(11);

					MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "SaleNet_OrigCurr");
					MeasureValue saleDuration = GetMeasureValue(analyticRecord, "SaleDuration");
					MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNetNotNULL");
					MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfCalls");
					MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
					MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
					#endregion
					var saleNetValue = Convert.ToDecimal(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
					if (saleNetValue != 0)
					{

						InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
						{
							CustomerId = Convert.ToInt32(customerId.Value),
							SaleCurrencyId = currencyId,
							OriginalSaleCurrencyId = Convert.ToInt32(saleCurrencyId.Value),
							SaleRate = saleRate != null ? Convert.ToDecimal(saleRate.Value) : default(Decimal),
							SaleRateTypeId = saleRateTypeId != null && saleRateTypeId.Value != null ? Convert.ToInt32(saleRateTypeId.Value) : default(int?),
							SaleZoneId = Convert.ToInt64(saleZoneId.Value),
							InvoiceMeasures = new InvoiceMeasures
							{
								SaleDuration = Convert.ToDecimal(saleDuration.Value ?? 0.0),
								SaleNet = saleNetValue,
								NumberOfCalls = Convert.ToInt32(calls.Value ?? 0.0),
								SaleNet_OrigCurr = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0),
							},
							CountryId = Convert.ToInt32(country.Value),
							SupplierId = Convert.ToInt32(supplier.Value),
							SupplierZoneId = Convert.ToInt32(supplierZone.Value),
							SaleDeal = saleDeal != null ? Convert.ToInt32(saleDeal.Value) : default(int?),
							SaleDealRateTierNb = saleDealRateTierNb != null ? Convert.ToDecimal(saleDealRateTierNb.Value) : default(Decimal?),
							SaleDealZoneGroupNb = saleDealZoneGroupNb != null ? Convert.ToInt32(saleDealZoneGroupNb.Value) : default(int?),
							SaleDealTierNb = saleDealTierNb != null ? Convert.ToInt32(saleDealTierNb.Value) : default(int?),
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
								invoiceBillingRecord.SaleRate = invoiceBillingRecord.SaleRate + ((invoiceBillingRecord.SaleRate * commission.Value) / 100);
							}

							invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr * commission.Value) / 100);
							invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet + ((invoiceBillingRecord.InvoiceMeasures.SaleNet * commission.Value) / 100);
						}
						else
						{
							invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
							invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet;
						}

						invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
						invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
						invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurrWithTaxes = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
						invoiceBillingRecord.InvoiceMeasures.SaleNetWithTaxes = invoiceBillingRecord.InvoiceMeasures.SaleNet;

						if (taxItemDetails != null)
						{
							foreach (var tax in taxItemDetails)
							{
								invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

								invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

								invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurrWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr * Convert.ToDecimal(tax.Value)) / 100);

								invoiceBillingRecord.InvoiceMeasures.SaleNetWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.SaleNet * Convert.ToDecimal(tax.Value)) / 100);
							}
						}

						AddItemToDictionary<String, InvoiceBillingRecord>(itemSetNamesDic, "GroupedBySaleZone", invoiceBillingRecord);
					}

				}
			}
			return itemSetNamesDic;
		}
		private Dictionary<string, List<SMSInvoiceBillingRecord>> PrepareSMSItemSetNames(IEnumerable<AnalyticRecord> analyticRecords, int currencyId, decimal? commission, CommissionType? commissionType, IEnumerable<VRTaxItemDetail> taxItemDetails, TimeSpan? offsetValue)
		{


			Dictionary<string, List<SMSInvoiceBillingRecord>> itemSetNamesDic = new Dictionary<string, List<SMSInvoiceBillingRecord>>();
			if (analyticRecords != null)
			{
				foreach (var analyticRecord in analyticRecords)
				{

					#region ReadDataFromAnalyticResult

					DimensionValue originationMobileNetwork = analyticRecord.DimensionValues.ElementAtOrDefault(0);
					DimensionValue customerId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
					DimensionValue saleCurrencyId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
					DimensionValue saleRate = analyticRecord.DimensionValues.ElementAtOrDefault(3);

					MeasureValue saleNet_OrigCurr = GetMeasureValue(analyticRecord, "SaleNet_OrigCurr");
					MeasureValue saleNet = GetMeasureValue(analyticRecord, "SaleNetNotNULL");
					MeasureValue calls = GetMeasureValue(analyticRecord, "NumberOfSMS");
					MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
					MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
					#endregion
					var saleNetValue = Convert.ToDecimal(saleNet == null ? 0.0 : saleNet.Value ?? 0.0);
					if (saleNetValue != 0)
					{

						SMSInvoiceBillingRecord invoiceBillingRecord = new SMSInvoiceBillingRecord
						{
							CustomerId = Convert.ToInt32(customerId.Value),
							SaleCurrencyId = currencyId,
							OriginalSaleCurrencyId = Convert.ToInt32(saleCurrencyId.Value),
							SaleRate = saleRate != null ? Convert.ToDecimal(saleRate.Value) : default(Decimal),
							CustomerMobileNetworkId = Convert.ToInt64(originationMobileNetwork.Value),
							InvoiceMeasures = new SMSInvoiceMeasures
							{
								SaleNet = saleNetValue,
								NumberOfSMS = Convert.ToInt32(calls.Value ?? 0.0),
								SaleNet_OrigCurr = Convert.ToDecimal(saleNet_OrigCurr == null ? 0.0 : saleNet_OrigCurr.Value ?? 0.0),
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
								invoiceBillingRecord.SaleRate = invoiceBillingRecord.SaleRate + ((invoiceBillingRecord.SaleRate * commission.Value) / 100);
							}

							invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr + ((invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr * commission.Value) / 100);
							invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet + ((invoiceBillingRecord.InvoiceMeasures.SaleNet * commission.Value) / 100);
						}
						else
						{
							invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
							invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission = invoiceBillingRecord.InvoiceMeasures.SaleNet;
						}

						invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission;
						invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes = invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission;
						invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurrWithTaxes = invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr;
						invoiceBillingRecord.InvoiceMeasures.SaleNetWithTaxes = invoiceBillingRecord.InvoiceMeasures.SaleNet;

						if (taxItemDetails != null)
						{
							foreach (var tax in taxItemDetails)
							{
								invoiceBillingRecord.InvoiceMeasures.AmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.AmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

								invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommissionWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.OriginalAmountAfterCommission * Convert.ToDecimal(tax.Value)) / 100);

								invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurrWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.SaleNet_OrigCurr * Convert.ToDecimal(tax.Value)) / 100);

								invoiceBillingRecord.InvoiceMeasures.SaleNetWithTaxes += ((invoiceBillingRecord.InvoiceMeasures.SaleNet * Convert.ToDecimal(tax.Value)) / 100);
							}
						}
						AddItemToDictionary<String, SMSInvoiceBillingRecord>(itemSetNamesDic, "GroupedByOriginationMobileNetwork", invoiceBillingRecord);
					}

				}
			}
			return itemSetNamesDic;
		}
		private void AddItemToDictionary<T, T1>(Dictionary<T, List<T1>> itemSetNamesDic, T key, T1 invoiceBillingRecord)
		{
			if (itemSetNamesDic == null)
				itemSetNamesDic = new Dictionary<T, List<T1>>();
			List<T1> invoiceBillingRecordList = null;

			if (!itemSetNamesDic.TryGetValue(key, out invoiceBillingRecordList))
			{
				invoiceBillingRecordList = new List<T1>();
				invoiceBillingRecordList.Add(invoiceBillingRecord);
				itemSetNamesDic.Add(key, invoiceBillingRecordList);
			}
			else
			{
				invoiceBillingRecordList.Add(invoiceBillingRecord);
				itemSetNamesDic[key] = invoiceBillingRecordList;
			}
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

		#region Public Entities
		public class InvoiceMeasures
		{
			public decimal SaleNet { get; set; }
			public decimal SaleNetWithTaxes { get; set; }

			public decimal SaleNet_OrigCurr { get; set; }
			public decimal SaleNet_OrigCurrWithTaxes { get; set; }

			public int NumberOfCalls { get; set; }
			public Decimal SaleDuration { get; set; }
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
			public int? SaleDealZoneGroupNb { get; set; }
			public int? SaleDealTierNb { get; set; }
			public int? SaleDeal { get; set; }
			public Decimal? SaleDealRateTierNb { get; set; }
			public InvoiceMeasures InvoiceMeasures { get; set; }
			public long SaleZoneId { get; set; }
			public int CustomerId { get; set; }
			public int OriginalSaleCurrencyId { get; set; }
			public Decimal SaleRate { get; set; }
			public int? SaleRateTypeId { get; set; }
			public int SaleCurrencyId { get; set; }
			public int CountryId { get; set; }
			public int SupplierId { get; set; }
			public int SupplierZoneId { get; set; }
		}

		//public class SMSInvoiceBillingRecord
		//{
		//	public long SaleZoneId { get; set; }
		//	public int CustomerId { get; set; }
		//	public int OriginalSaleCurrencyId { get; set; }
		//	public Decimal SaleRate { get; set; }
		//	public int SaleCurrencyId { get; set; }
		//	public int CountryId { get; set; }
		//	public int SupplierId { get; set; }
		//	public int SupplierZoneId { get; set; }
		//	public SMSInvoiceMeasures InvoiceMeasures { get; set; }
		//}
		//public class SMSInvoiceMeasures
		//{
		//	public decimal SaleNet { get; set; }
		//	public int NumberOfSMS { get; set; }
		//	public DateTime BillingPeriodTo { get; set; }
		//	public DateTime BillingPeriodFrom { get; set; }
		//	public decimal SaleNetWithTaxes { get; set; }
		//	public DateTime OriginalBillingPeriodTo { get; set; }
		//	public DateTime OriginalBillingPeriodFrom { get; set; }
		//	public decimal SaleNet_OrigCurr { get; set; }
		//	public decimal SaleNet_OrigCurrWithTaxes { get; set; }
		//	public decimal AmountAfterCommission { get; set; }
		//	public decimal OriginalAmountAfterCommission { get; set; }
		//	public decimal AmountAfterCommissionWithTaxes { get; set; }
		//	public decimal OriginalAmountAfterCommissionWithTaxes { get; set; }

		//}


		#endregion
	}
}
