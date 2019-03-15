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
using Vanrise.Security.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.MainExtensions;
using TOne.WhS.Deal.Entities;

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
			List<RecurringChargeItem> evaluatedCustomerRecurringCharges = _customerRecurringChargeManager.GetEvaluatedRecurringCharges(financialAccount.FinancialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, context.IssueDate);

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
					invoiceBySaleCurrency = loadVoiceCurrencyItemSet(dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
					dealItemSetNames = GetDealItemSetNames(carrierAccountIds, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.OffsetValue, dimensionName, financialAccountId, context.IssueDate, currencyId);
				}
			}

			bool isSMSEnabled = _tOneModuleManager.IsSMSModuleEnabled();
			List<CustomerSMSInvoiceItemDetails> smsItemSetNames = null;
			if (isSMSEnabled)
			{
				List<string> listMeasures = new List<string> { "NumberOfSMS", "BillingPeriodFrom", "BillingPeriodTo", "SaleNetNotNULL", "SaleNet_OrigCurr" };
				List<string> listDimensions = new List<string> { "DestinationMobileNetwork",  "DestinationMobileCountry", "Customer", "SaleCurrency", "SaleRate", "Supplier"};

				smsItemSetNames = _invoiceGenerationManager.GetInvoiceSMSMappedRecords(listDimensions, listMeasures, dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, currencyId, resolvedPayload.OffsetValue, (analyticRecord) =>
				{
					return SMSItemSetNamesMapper(analyticRecord, currencyId, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
				});

				var sMSInvoiceBySaleCurrencies = loadSMSCurrencyItemSet(dimensionName, financialAccountId, resolvedPayload.FromDate, resolvedPayload.ToDate, resolvedPayload.Commission, resolvedPayload.CommissionType, taxItemDetails, resolvedPayload.OffsetValue);
				if (invoiceBySaleCurrency == null)
					invoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>();
				TryMergeByCurrencyItemSets(invoiceBySaleCurrency, sMSInvoiceBySaleCurrencies);
			}



			if (((smsItemSetNames == null || smsItemSetNames.Count == 0) && ((voiceItemSetNames == null || voiceItemSetNames.Count == 0))) && (evaluatedCustomerRecurringCharges == null || evaluatedCustomerRecurringCharges.Count == 0) && (dealItemSetNames == null || dealItemSetNames.Count == 0))
			{
				context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
				return;
			}


			decimal? minAmount = _partnerManager.GetPartnerMinAmount(context.InvoiceTypeId, context.PartnerId);


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
				customerInvoiceDetails.CommissionType = resolvedPayload.CommissionType;
				customerInvoiceDetails.Offset = resolvedPayload.Offset;

				customerInvoiceDetails.TotalAmountAfterCommission = customerInvoiceDetails.AmountAfterCommission;
				customerInvoiceDetails.TotalOriginalAmountAfterCommission = customerInvoiceDetails.OriginalAmountAfterCommission;


                customerInvoiceDetails.TotalAmountBeforeTax = customerInvoiceDetails.TotalSMSAmountAfterCommission + customerInvoiceDetails.TotalAmountAfterCommission + customerInvoiceDetails.TotalDealAmount;

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
				if (invoiceBySaleCurrency == null)
					invoiceBySaleCurrency = new List<CustomerInvoiceBySaleCurrencyItemDetails>();
				AddRecurringChargeToCustomerCurrency(invoiceBySaleCurrency, evaluatedCustomerRecurringCharges);


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
					customerInvoiceDetails.TotalAmountBeforeTax += customerInvoiceDetails.TotalReccurringCharges;

                    customerInvoiceDetails.HasDeals = customerInvoiceDetails.TotalDealAmount > 0;
                    customerInvoiceDetails.HasSMS = customerInvoiceDetails.TotalSMSAmountAfterCommission > 0;
                    customerInvoiceDetails.HasVoice = customerInvoiceDetails.TotalAmountAfterCommission > 0;
                    customerInvoiceDetails.HasRecurringCharges = customerInvoiceDetails.TotalReccurringChargesAfterTax > 0;

                    customerInvoiceDetails.TotalInvoiceAmount = customerInvoiceDetails.TotalAmountAfterCommission + customerInvoiceDetails.TotalReccurringChargesAfterTax + customerInvoiceDetails.TotalSMSAmountAfterCommission + customerInvoiceDetails.TotalDealAmount;

                    if (!financialAccountInvoiceType.IgnoreFromBalance)
                    {
                        SetInvoiceBillingTransactions(context, customerInvoiceDetails, financialAccount, resolvedPayload.FromDate, resolvedPayload.ToDateForBillingTransaction, currencyId);
                    }

                    if (taxItemDetails != null && taxItemDetails.Count() > 0)
					{
						foreach (var tax in taxItemDetails)
						{
							tax.TaxAmount = ((customerInvoiceDetails.TotalAmountBeforeTax * Convert.ToDecimal(tax.Value)) / 100);
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
					OriginalSaleCurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord,3),
					SaleRate = _invoiceGenerationManager.GetDimensionValue<Decimal>(analyticRecord, 4),
					CustomerMobileNetworkId = _invoiceGenerationManager.GetDimensionValue<long>(analyticRecord, 0),
                    MobileCountryId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord,1),
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
			if (saleNetValue != 0)
			{
				CustomerInvoiceDealItemDetails invoiceBillingRecord = new CustomerInvoiceDealItemDetails
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
				return invoiceBillingRecord;
			}
			return null;
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
					Amount = saleNetValue,
					Month = _invoiceGenerationManager.GetDimensionValue<string>(analyticRecord, 1),
					TotalSMSAmount = saleNetValue
				};

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
		private void TryMergeByCurrencyItemSets(List<CustomerInvoiceBySaleCurrencyItemDetails> mainByCurrencyItemSets, List<CustomerInvoiceBySaleCurrencyItemDetails> sMSInvoiceBySaleCurrency)
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
		private List<CustomerInvoiceDealItemDetails> GetDealItemSetNames(List<int> carrierAccountIds, DateTime fromDate, DateTime toDate, TimeSpan? offsetValue, string dimensionName, int financialAccountId, DateTime issueDate, int currencyId)
		{
			VolCommitmentDealManager volCommitmentDealManager = new VolCommitmentDealManager();
			DateTime? minBED = null;
			DateTime? maxEED = null;
			var effectiveVolCommitmentDeals = volCommitmentDealManager.GetEffectiveVolCommitmentDeals(VolCommitmentDealType.Sell,carrierAccountIds, fromDate, toDate, out minBED, out maxEED);
			if (effectiveVolCommitmentDeals != null && effectiveVolCommitmentDeals.Count() > 0 && minBED.HasValue && maxEED.HasValue)
			{
				List<string> dealMeasures = new List<string> { "SaleNet_OrigCurr", "NumberOfCalls", "SaleDuration", "SaleNetNotNULL" };
				List<string> dealDimensions = new List<string> { "SaleDealZoneGroupNb", "SaleDealTierNb", "SaleDeal", "SaleDealRateTierNb", "SaleCurrency" };
				var allDealItemSetNames = _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(dealDimensions, dealMeasures, dimensionName, financialAccountId, minBED.Value, maxEED.Value, null, offsetValue, (analyticRecord) =>
				{
					return DealItemSetNameMapper(analyticRecord);
				});
				if (allDealItemSetNames == null)
					allDealItemSetNames = new List<CustomerInvoiceDealItemDetails>();
				var dealItemSetNames = new List<CustomerInvoiceDealItemDetails>();
				foreach (var effectiveDeal in effectiveVolCommitmentDeals)
				{
					var effectiveDealSettings = effectiveDeal.Value;
					if (effectiveDealSettings.Items != null && effectiveDealSettings.Items.Count > 0)
					{
						for (int i = 0; i < effectiveDealSettings.Items.Count; i++)
						{
							var dealGroup = effectiveDealSettings.Items[i];
							if (dealGroup.Tiers != null && dealGroup.Tiers.Count > 0)
							{
								var tier = dealGroup.Tiers.First();
								var dealItemSet = allDealItemSetNames.FindRecord(x => x.SaleDeal == effectiveDeal.Key && dealGroup.ZoneGroupNumber == x.SaleDealZoneGroupNb);
								if (tier.UpToVolume.HasValue && tier.EvaluatedRate != null)
								{
									var fixedSaleRateEvaluator = tier.EvaluatedRate.CastWithValidate<FixedSaleRateEvaluator>("fixedSaleRateEvaluator");
									var expectedAmount = tier.UpToVolume.Value * fixedSaleRateEvaluator.Rate;
									if (dealItemSet != null)
									{
										if (expectedAmount > dealItemSet.OriginalAmount && effectiveDealSettings.CurrencyId == dealItemSet.CurrencyId)
										{
											var originalAmount = expectedAmount - dealItemSet.OriginalAmount;
											dealItemSetNames.Add(new CustomerInvoiceDealItemDetails()
											{
												OriginalAmount = originalAmount,
												Duration = tier.UpToVolume.Value - (dealItemSet.Duration / 60),
												NumberOfCalls = dealItemSet.NumberOfCalls,
												SaleDeal = dealItemSet.SaleDeal,
												SaleDealRateTierNb = dealItemSet.SaleDealRateTierNb,
												SaleDealTierNb = dealItemSet.SaleDealTierNb,
												SaleDealZoneGroupNb = dealItemSet.SaleDealZoneGroupNb,
												CurrencyId = dealItemSet.CurrencyId,
												Amount = _currencyExchangeRateManager.ConvertValueToCurrency(originalAmount, dealItemSet.CurrencyId, currencyId, issueDate),
												ToDate = effectiveDealSettings.RealEED.Value,
												FromDate = fromDate >= effectiveDealSettings.RealBED? fromDate : effectiveDealSettings.RealBED,
											});
										}
									}
									else
									{
										dealItemSetNames.Add(new CustomerInvoiceDealItemDetails()
										{
											OriginalAmount = expectedAmount,
											Duration = tier.UpToVolume.Value,
											SaleDeal = effectiveDeal.Key,
											SaleDealRateTierNb = 1,
											SaleDealTierNb = 1,
											SaleDealZoneGroupNb = dealGroup.ZoneGroupNumber,
											CurrencyId = effectiveDealSettings.CurrencyId,
											Amount = _currencyExchangeRateManager.ConvertValueToCurrency(expectedAmount, effectiveDealSettings.CurrencyId, currencyId, issueDate),
											ToDate = effectiveDealSettings.RealEED.Value ,
											FromDate = fromDate >= effectiveDealSettings.RealBED? fromDate : effectiveDealSettings.RealBED,
										});
									}
								}
							}
						}
					}
				}
				return dealItemSetNames;
			}
			return null;
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
