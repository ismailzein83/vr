using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
using Retail.Interconnect.Entities;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Common;
using Vanrise.InvToAccBalanceRelation.Entities;
namespace Retail.Interconnect.Business
{
    public class InterconnectInvoiceGenerator : InvoiceGenerator
    {
        #region Constructors
        Guid _acountBEDefinitionId;
        Guid _invoiceTransactionTypeId;
        List<Guid> _usageTransactionTypeIds;
        InterconnectInvoiceType _type;

        public InterconnectInvoiceGenerator(Guid acountBEDefinitionId, Guid invoiceTransactionTypeId, List<Guid> usageTransactionTypeIds, InterconnectInvoiceType type)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._invoiceTransactionTypeId = invoiceTransactionTypeId;
            this._usageTransactionTypeIds = usageTransactionTypeIds;
            this._type = type;
        }

        #endregion

        #region Fields

        private FinancialAccountManager _financialAccountManager = new FinancialAccountManager();
        private AccountBEManager _accountBEManager = new AccountBEManager();
        private FinancialRecurringChargeManager _recurringChargeManager = new FinancialRecurringChargeManager();
        private InterconnectInvoiceGenerationManager _invoiceGenerationManager = new InterconnectInvoiceGenerationManager();
        private CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();

        #endregion

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            Guid voiceAnalyticTableId = new Guid("6cd535c0-ac49-46bb-aecf-0eae33823b20");
            Guid smsAnalyticTableId = new Guid("c1bd3f2f-6213-44d1-9d58-99f81e169930");

            FinancialAccountData financialAccountData = _financialAccountManager.GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;
            }

            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;

            string dimensionName = "BillingAccountId";
            string dimensionValue = financialAccountData.FinancialAccountId;
            int currencyId = _accountBEManager.GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);
            string classification = _type == InterconnectInvoiceType.Customer ? "Customer" : "Supplier";

            InterconnectModuleManager interconnectModuleManager = new InterconnectModuleManager();

            List<InterconnectInvoiceByCurrencyItemDetails> invoiceByCurrency = null;
            IEnumerable<VRTaxItemDetail> taxItemDetails = _financialAccountManager.GetFinancialAccountTaxItemDetails(context.InvoiceTypeId, _acountBEDefinitionId, context.PartnerId);
            List<InvoiceBillingRecord> voiceItemSetNames = new List<InvoiceBillingRecord>();
            List<InvoiceSMSBillingRecord> smsItemSetNames = new List<InvoiceSMSBillingRecord>();

            if (interconnectModuleManager.IsVoiceModuleEnabled())
            {
                List<string> voiceListMeasures = new List<string> { "TotalBillingDuration", "Amount", "CountCDRs", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OrigCurr" };
                List<string> voiceListDimensions = new List<string> { "DestinationZone", "OriginationZone", "Operator", "Rate", "RateType", "BillingType", "Currency" };
                voiceItemSetNames = _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(voiceListDimensions, voiceListMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId, (analyticRecord) =>
                 {
                     return VoiceItemSetNameMapper(analyticRecord);
                 });
                invoiceByCurrency = LoadVoiceCurrencyItemSetName(dimensionName, financialAccountData.FinancialAccountId, fromDate, toDate, taxItemDetails);
            }
            if (interconnectModuleManager.IsSMSModuleEnabled())
            {
                List<string> smsListMeasures = new List<string> { "Amount", "DeliveredSMS", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OriginalCurrency" };
                List<string> smsListDimensions = new List<string> { "DestinationMobileNetwork", "OriginationMobileNetwork", "Operator", "Rate", "RateType", "BillingType", "Currency" };

                smsItemSetNames = _invoiceGenerationManager.GetInvoiceSMSMappedRecords(smsListDimensions, smsListMeasures, dimensionName, dimensionValue, fromDate, toDate, currencyId, (analyticRecord) =>
                {
                    return SMSItemSetNameMapper(analyticRecord);
                });
                var smsInvoiceByCurrency = LoadSMSCurrencyItemSetName(dimensionName, financialAccountData.FinancialAccountId, fromDate, toDate, taxItemDetails);
                if (invoiceByCurrency == null)
                    invoiceByCurrency = new List<InterconnectInvoiceByCurrencyItemDetails>();
                TryMergeByCurrencyItemSets(invoiceByCurrency, smsInvoiceByCurrency);
            }

            List<FinancialRecurringChargeItem> evaluatedRecurringCharges = _recurringChargeManager.GetEvaluatedRecurringCharges(financialAccountData.FinancialAccountId, fromDate, toDate, context.IssueDate, classification);

            if ((voiceItemSetNames==null || voiceItemSetNames.Count == 0) && (smsItemSetNames==null || smsItemSetNames.Count == 0) && (evaluatedRecurringCharges == null || evaluatedRecurringCharges.Count == 0))
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            #region BuildInterconnectInvoiceDetails
            InterconnectInvoiceDetails interconnectInvoiceDetails = BuildInterconnectInvoiceDetails(voiceItemSetNames, smsItemSetNames, context.FromDate, context.ToDate, currencyId);
            if (interconnectInvoiceDetails != null)
            {
                interconnectInvoiceDetails.AmountWithTaxes = interconnectInvoiceDetails.Amount;
                interconnectInvoiceDetails.SMSAmountWithTaxes = interconnectInvoiceDetails.SMSAmount;
                interconnectInvoiceDetails.TotalAmountBeforeTaxes = interconnectInvoiceDetails.Amount + interconnectInvoiceDetails.SMSAmount;
                if (taxItemDetails != null && taxItemDetails.Count() > 0)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        interconnectInvoiceDetails.AmountWithTaxes += ((interconnectInvoiceDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                        interconnectInvoiceDetails.SMSAmountWithTaxes += ((interconnectInvoiceDetails.SMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                        if (evaluatedRecurringCharges != null && evaluatedRecurringCharges.Count > 0)
                        {
                            foreach (var item in evaluatedRecurringCharges)
                            {
                                item.AmountAfterTaxes += ((item.Amount * Convert.ToDecimal(tax.Value)) / 100);
                                item.VAT = tax.IsVAT ? tax.Value : 0;
                            }
                        }
                    }
                }
                interconnectInvoiceDetails.TotalAmountWithTaxes = interconnectInvoiceDetails.AmountWithTaxes + interconnectInvoiceDetails.SMSAmountWithTaxes;

                if (invoiceByCurrency == null)
                    invoiceByCurrency = new List<InterconnectInvoiceByCurrencyItemDetails>();
                AddRecurringChargeToInvoiceByCurrency(invoiceByCurrency, evaluatedRecurringCharges);

                decimal totalReccurringChargesAfterTaxInAccountCurrency = 0;
                decimal totalReccurringChargesInAccountCurrency = 0;
                if (evaluatedRecurringCharges != null && evaluatedRecurringCharges.Count > 0)
                {
                    foreach (var item in evaluatedRecurringCharges)
                    {
                        totalReccurringChargesAfterTaxInAccountCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.AmountAfterTaxes, item.CurrencyId, currencyId, context.IssueDate);
                        totalReccurringChargesInAccountCurrency += currencyExchangeRateManager.ConvertValueToCurrency(item.Amount, item.CurrencyId, currencyId, context.IssueDate);
                    }
                }


                interconnectInvoiceDetails.TotalRecurringChargesAfterTaxes = totalReccurringChargesAfterTaxInAccountCurrency;
                interconnectInvoiceDetails.TotalRecurringCharges = totalReccurringChargesInAccountCurrency;

				interconnectInvoiceDetails.NoVoice = interconnectInvoiceDetails.AmountWithTaxes == 0;
				interconnectInvoiceDetails.NoSMS = interconnectInvoiceDetails.SMSAmountWithTaxes == 0;
				interconnectInvoiceDetails.NoRecurringCharges = interconnectInvoiceDetails.TotalRecurringChargesAfterTaxes == 0;

				interconnectInvoiceDetails.TotalInvoiceAmount = interconnectInvoiceDetails.TotalAmountWithTaxes + interconnectInvoiceDetails.TotalRecurringChargesAfterTaxes;
                interconnectInvoiceDetails.TotalAmountBeforeTaxes += interconnectInvoiceDetails.TotalRecurringCharges;

                if (taxItemDetails != null && taxItemDetails.Count() > 0)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        tax.TaxAmount = ((interconnectInvoiceDetails.TotalAmountBeforeTaxes * Convert.ToDecimal(tax.Value)) / 100);
                    }
                }

                if (interconnectInvoiceDetails.TotalInvoiceAmount != 0)
                {
                    List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(voiceItemSetNames, smsItemSetNames, taxItemDetails, invoiceByCurrency, evaluatedRecurringCharges);

                    context.Invoice = new GeneratedInvoice
                    {
                        InvoiceDetails = interconnectInvoiceDetails,
                        InvoiceItemSets = generatedInvoiceItemSets,
                    };
                    SetInvoiceBillingTransactions(context, interconnectInvoiceDetails);
                }
                else
                {
                    context.ErrorMessage = "No billing data available.";
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

        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, InterconnectInvoiceDetails invoiceDetails)
        {
            var relationManager = new Vanrise.InvToAccBalanceRelation.Business.InvToAccBalanceRelationDefinitionManager();
            List<BalanceAccountInfo> invoiceBalanceAccounts = relationManager.GetInvoiceBalanceAccounts(context.InvoiceTypeId, context.PartnerId, context.IssueDate);

            invoiceBalanceAccounts.ThrowIfNull("invoiceBalanceAccounts", context.PartnerId);
            if (invoiceBalanceAccounts.Count == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException("invoiceBalanceAccounts.Count == 0");

            var billingTransaction = new GeneratedInvoiceBillingTransaction()
            {
                AccountTypeId = invoiceBalanceAccounts.FirstOrDefault().AccountTypeId,
                AccountId = context.PartnerId,
                TransactionTypeId = this._invoiceTransactionTypeId,
                Amount = invoiceDetails.TotalInvoiceAmount,
                CurrencyId = invoiceDetails.InterconnectCurrencyId
            };

            billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
            billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();
            if (this._usageTransactionTypeIds != null)
            {
                foreach (Guid usageTransactionTypeId in this._usageTransactionTypeIds)
                {
                    billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                    {
                        TransactionTypeId = usageTransactionTypeId
                    });
                }
            }
            context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
        }
        private InterconnectInvoiceDetails BuildInterconnectInvoiceDetails(List<InvoiceBillingRecord> voiceItemSetNames, List<InvoiceSMSBillingRecord> smsItemSetNames, DateTime fromDate, DateTime toDate, int currencyId)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            InterconnectInvoiceDetails interconnectInvoiceDetails = new InterconnectInvoiceDetails()
            {
                InterconnectCurrencyId = currencyId,
                InterconnectCurrency = currencyManager.GetCurrencySymbol(currencyId)
            };

            if (voiceItemSetNames != null && voiceItemSetNames.Count > 0)
            {
                foreach (var invoiceBillingRecord in voiceItemSetNames)
                {
                    interconnectInvoiceDetails.Duration += invoiceBillingRecord.InvoiceMeasures.TotalBillingDuration;
                    interconnectInvoiceDetails.Amount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.TotalAmount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.InterconnectCurrencyId = invoiceBillingRecord.CurrencyId;
                    interconnectInvoiceDetails.TotalNumberOfCalls += invoiceBillingRecord.InvoiceMeasures.NumberOfCalls;
                }
            }
            if (smsItemSetNames != null && smsItemSetNames.Count > 0)
            {
                foreach (var invoiceBillingRecord in smsItemSetNames)
                {
                    interconnectInvoiceDetails.TotalAmount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.SMSAmount += invoiceBillingRecord.InvoiceMeasures.Amount;
                    interconnectInvoiceDetails.InterconnectCurrencyId = invoiceBillingRecord.CurrencyId;
                    interconnectInvoiceDetails.TotalNumberOfSMS += invoiceBillingRecord.InvoiceMeasures.NumberOfSMS;
                }
            }
            return interconnectInvoiceDetails;
        }
        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(List<InvoiceBillingRecord> voiceItemSetNames, List<InvoiceSMSBillingRecord> smsItemSetNames, IEnumerable<VRTaxItemDetail> taxItemDetails, List<InterconnectInvoiceByCurrencyItemDetails> invoiceByCurrency, List<FinancialRecurringChargeItem> evaluatedRecurringCharges)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("GroupingByCurrency", generatedInvoiceItemSets, invoiceByCurrency);
            if (voiceItemSetNames != null && voiceItemSetNames.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupedByDestinationZone",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var item in voiceItemSetNames)
                {
                    InterconnectInvoiceItemDetails interconnectInvoiceItemDetails = new Entities.InterconnectInvoiceItemDetails()
                    {
                        Duration = item.InvoiceMeasures.TotalBillingDuration,
                        Amount = item.InvoiceMeasures.Amount,
                        DestinationZoneId = item.DestinationZoneId,
                        OriginationZoneId = item.OriginationZoneId,
                        OperatorId = item.OperatorId,
                        Rate = item.Rate,
                        RateTypeId = item.RateTypeId,
                        TrafficDirection = item.TrafficDirection,
                        CurrencyId = item.CurrencyId,
                        NumberOfCalls = item.InvoiceMeasures.NumberOfCalls,
                        FromDate = item.InvoiceMeasures.FromDate,
                        ToDate = item.InvoiceMeasures.ToDate,
                        OriginalAmount = item.InvoiceMeasures.Amount_OrigCurr,
                    };
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = interconnectInvoiceItemDetails,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }
            if (smsItemSetNames != null && smsItemSetNames.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet()
                {
                    SetName = "GroupedByDestinationMobileNetwork",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var item in smsItemSetNames)
                {
                    InterconnectSMSInvoiceItemDetails interconnectInvoiceItemDetails = new Entities.InterconnectSMSInvoiceItemDetails()
                    {
                        Amount = item.InvoiceMeasures.Amount,
                        DestinationMobileNetworkId = item.DestinationMobileNetworkId,
                        OriginationMobileNetworkId = item.OriginationMobileNetworkId,
                        OperatorId = item.OperatorId,
                        Rate = item.Rate,
                        RateTypeId = item.RateTypeId,
                        BillingType = item.BillingType,
                        CurrencyId = item.CurrencyId,
                        NumberOfSMS = item.InvoiceMeasures.NumberOfSMS,
                        FromDate = item.InvoiceMeasures.FromDate,
                        ToDate = item.InvoiceMeasures.ToDate,
                        OriginalAmount = item.InvoiceMeasures.Amount_OrigCurr,
                    };
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = interconnectInvoiceItemDetails,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }
            _invoiceGenerationManager.AddGeneratedInvoiceItemSet("RecurringCharge", generatedInvoiceItemSets, evaluatedRecurringCharges);
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
            return generatedInvoiceItemSets;
        }
        private List<InterconnectInvoiceByCurrencyItemDetails> LoadVoiceCurrencyItemSetName(string dimensionName, string dimensionValue, DateTime fromDate, DateTime toDate, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            List<string> listMeasures = new List<string> { "CountCDRs", "TotalBillingDuration", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OrigCurr" };
            List<string> listDimensions = new List<string> { "Currency", "YearMonth"};

            return _invoiceGenerationManager.GetInvoiceVoiceMappedRecords(listDimensions, listMeasures, dimensionName, dimensionValue, fromDate, toDate, null, (analyticRecord) =>
            {
                return CurrencyItemSetNameMapper(analyticRecord, taxItemDetails, true);
            });
        }
        private List<InterconnectInvoiceByCurrencyItemDetails> LoadSMSCurrencyItemSetName(string dimensionName, string dimensionValue, DateTime fromDate, DateTime toDate, IEnumerable<VRTaxItemDetail> taxItemDetails)
        {
            List<string> listMeasures = new List<string> { "DeliveredSMS", "BillingPeriodTo", "BillingPeriodFrom", "Amount_OriginalCurrency" };
            List<string> listDimensions = new List<string> { "Currency", "YearMonth" };

            return _invoiceGenerationManager.GetInvoiceSMSMappedRecords(listDimensions, listMeasures, dimensionName, dimensionValue, fromDate, toDate, null, (analyticRecord) =>
            {
                return CurrencyItemSetNameMapper(analyticRecord, taxItemDetails, false);
            });
        }


        private MeasureValue GetMeasureValue(AnalyticRecord analyticRecord, string measureName)
        {
            MeasureValue measureValue;
            analyticRecord.MeasureValues.TryGetValue(measureName, out measureValue);
            return measureValue;
        }
        private InterconnectInvoiceByCurrencyItemDetails CurrencyItemSetNameMapper(AnalyticRecord analyticRecord, IEnumerable<VRTaxItemDetail> taxItemDetails, bool fillVoiceData)
        {
            var netValue = fillVoiceData ? _invoiceGenerationManager.GetMeasureValue<decimal>(analyticRecord, "Amount_OrigCurr") : _invoiceGenerationManager.GetMeasureValue<decimal>(analyticRecord, "Amount_OriginalCurrency");
            if (netValue != 0)
            {
                var month = _invoiceGenerationManager.GetDimensionValue<DateTime>(analyticRecord, 1);
                var invoiceByCurrencyItemDetails = new InterconnectInvoiceByCurrencyItemDetails
                {
                    CurrencyId = _invoiceGenerationManager.GetDimensionValue<int>(analyticRecord, 0),
                    FromDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodFrom"),
                    ToDate = _invoiceGenerationManager.GetMeasureValue<DateTime>(analyticRecord, "BillingPeriodTo"),
                    Month = month.ToString("MMMM - yyyy")
                };
                if (fillVoiceData)
                {
                    invoiceByCurrencyItemDetails.Amount = netValue;
                    invoiceByCurrencyItemDetails.AmountWithTaxes = netValue;
                    invoiceByCurrencyItemDetails.NumberOfCalls = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "CountCDRs");
                    invoiceByCurrencyItemDetails.Duration = _invoiceGenerationManager.GetMeasureValue<decimal>(analyticRecord, "TotalBillingDuration");
                }
                else
                {
                    invoiceByCurrencyItemDetails.SMSAmount = netValue;
                    invoiceByCurrencyItemDetails.SMSAmountWithTaxes = netValue;
                    invoiceByCurrencyItemDetails.NumberOfSMS = _invoiceGenerationManager.GetMeasureValue<int>(analyticRecord, "DeliveredSMS");
                }
                if (taxItemDetails != null && taxItemDetails.Count() > 0)
                {
                    foreach (var tax in taxItemDetails)
                    {
                        if (fillVoiceData)
                        {
                            invoiceByCurrencyItemDetails.AmountWithTaxes += ((invoiceByCurrencyItemDetails.Amount * Convert.ToDecimal(tax.Value)) / 100);
                        }
                        else
                        {
                            invoiceByCurrencyItemDetails.SMSAmountWithTaxes += ((invoiceByCurrencyItemDetails.SMSAmount * Convert.ToDecimal(tax.Value)) / 100);
                        }
                    }
                }
                invoiceByCurrencyItemDetails.TotalTrafficAmount = invoiceByCurrencyItemDetails.AmountWithTaxes;
                invoiceByCurrencyItemDetails.TotalSMSAmount = invoiceByCurrencyItemDetails.SMSAmountWithTaxes;
                invoiceByCurrencyItemDetails.TotalFullAmount = fillVoiceData ? invoiceByCurrencyItemDetails.AmountWithTaxes : invoiceByCurrencyItemDetails.SMSAmountWithTaxes;

                return invoiceByCurrencyItemDetails;
            }
            return null;

        }

        private void TryMergeByCurrencyItemSets(List<InterconnectInvoiceByCurrencyItemDetails> voiceByCurrencyItemSets, List<InterconnectInvoiceByCurrencyItemDetails> smsInvoiceByCurrency)
        {
            if (smsInvoiceByCurrency != null && smsInvoiceByCurrency.Count > 0)
            {
                foreach (var item in smsInvoiceByCurrency)
                {
                    var invoiceByCurrencyItem = voiceByCurrencyItemSets.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.Month);
                    if (invoiceByCurrencyItem != null)
                    {
                        invoiceByCurrencyItem.TotalSMSAmount += item.TotalSMSAmount;
                        invoiceByCurrencyItem.TotalFullAmount += item.SMSAmountWithTaxes;
                    }
                    else
                    {
                        voiceByCurrencyItemSets.Add(new InterconnectInvoiceByCurrencyItemDetails
                        {
                            FromDate = item.FromDate,
                            ToDate = item.ToDate,
                            TotalSMSAmount = item.TotalSMSAmount,
                            NumberOfSMS = item.NumberOfSMS,
                            CurrencyId = item.CurrencyId,
                            Month = item.Month,
                            TotalFullAmount = item.SMSAmountWithTaxes
                        });
                    }
                }
            }
        }

        private void AddRecurringChargeToInvoiceByCurrency(List<InterconnectInvoiceByCurrencyItemDetails> invoiceByCurrencyItemDetails, List<FinancialRecurringChargeItem> recurringChargeItems)
        {
            if (recurringChargeItems != null && recurringChargeItems.Count > 0)
            {
                if (invoiceByCurrencyItemDetails == null)
                    invoiceByCurrencyItemDetails = new List<InterconnectInvoiceByCurrencyItemDetails>();

                foreach (var item in recurringChargeItems)
                {
                    var invoiceByCurrencyItemDetail = invoiceByCurrencyItemDetails.FindRecord(x => x.CurrencyId == item.CurrencyId && x.Month == item.RecurringChargeMonth);
                    if (invoiceByCurrencyItemDetail != null)
                    {
                        invoiceByCurrencyItemDetail.Amount += item.Amount;
                        invoiceByCurrencyItemDetail.AmountWithTaxes += item.AmountAfterTaxes;
                        invoiceByCurrencyItemDetail.TotalRecurringChargeAmount += item.AmountAfterTaxes;
                        invoiceByCurrencyItemDetail.TotalFullAmount += item.AmountAfterTaxes;
                    }
                    else
                    {
                        invoiceByCurrencyItemDetails.Add(new InterconnectInvoiceByCurrencyItemDetails
                        {
                            FromDate = item.From,
                            ToDate = item.To,
                            AmountWithTaxes = item.AmountAfterTaxes,
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
        private InvoiceSMSBillingRecord SMSItemSetNameMapper(AnalyticRecord analyticRecord)
        {
            #region ReadDataFromAnalyticResult
            DimensionValue destinationMobileNetworkId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
            DimensionValue originationMobileNetworkId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
            DimensionValue operatorId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
            DimensionValue rate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
            DimensionValue rateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);
            DimensionValue billingType = analyticRecord.DimensionValues.ElementAtOrDefault(5);
            DimensionValue currency = analyticRecord.DimensionValues.ElementAtOrDefault(6);

            MeasureValue amount = GetMeasureValue(analyticRecord, "Amount");
            MeasureValue deliveredSMS = GetMeasureValue(analyticRecord, "DeliveredSMS");
            MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
            MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
            MeasureValue amount_OrigCurr = GetMeasureValue(analyticRecord, "Amount_OriginalCurrency");

            #endregion

            var amountValue = Convert.ToDecimal(amount == null ? 0.0 : amount.Value ?? 0.0);
            if (amountValue != 0)
            {
                InvoiceSMSBillingRecord invoiceBillingRecord = new InvoiceSMSBillingRecord
                {
                    DestinationMobileNetworkId = Convert.ToInt64(destinationMobileNetworkId.Value),
                    OriginationMobileNetworkId = Convert.ToInt64(originationMobileNetworkId.Value),
                    OperatorId = Convert.ToInt64(operatorId.Value),
                    Rate = rate != null ? Convert.ToDecimal(rate.Value) : default(decimal),
                    RateTypeId = rateTypeId != null && rateTypeId.Value != null ? Convert.ToInt32(rateTypeId.Value) : default(int),
                    BillingType = Convert.ToInt32(billingType.Value),
                    CurrencyId = Convert.ToInt32(currency.Value),
                    InvoiceMeasures = new InvoiceSMSMeasures
                    {
                        FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                        ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                        Amount = amountValue,
                        Amount_OrigCurr = Convert.ToDecimal(amount_OrigCurr == null ? 0.0 : amount_OrigCurr.Value ?? 0.0),
                        NumberOfSMS = Convert.ToInt32(deliveredSMS.Value ?? 00),
                    }
                };
                return invoiceBillingRecord;
            }
            return null;
        }

        private InvoiceBillingRecord VoiceItemSetNameMapper(AnalyticRecord analyticRecord)
        {
            DimensionValue destinationZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(0);
            DimensionValue originationZoneId = analyticRecord.DimensionValues.ElementAtOrDefault(1);
            DimensionValue operatorId = analyticRecord.DimensionValues.ElementAtOrDefault(2);
            DimensionValue rate = analyticRecord.DimensionValues.ElementAtOrDefault(3);
            DimensionValue rateTypeId = analyticRecord.DimensionValues.ElementAtOrDefault(4);
            DimensionValue trafficDirection = analyticRecord.DimensionValues.ElementAtOrDefault(5);
            DimensionValue currency = analyticRecord.DimensionValues.ElementAtOrDefault(6);

            MeasureValue totalBillingDuration = GetMeasureValue(analyticRecord, "TotalBillingDuration");
            MeasureValue amount = GetMeasureValue(analyticRecord, "Amount");
            MeasureValue countCDRs = GetMeasureValue(analyticRecord, "CountCDRs");
            MeasureValue billingPeriodTo = GetMeasureValue(analyticRecord, "BillingPeriodTo");
            MeasureValue billingPeriodFrom = GetMeasureValue(analyticRecord, "BillingPeriodFrom");
            MeasureValue amount_OrigCurr = GetMeasureValue(analyticRecord, "Amount_OrigCurr");

            var amountValue = Convert.ToDecimal(amount == null ? 0.0 : amount.Value ?? 0.0);
            if (amountValue != 0)
            {
                InvoiceBillingRecord invoiceBillingRecord = new InvoiceBillingRecord
                {
                    DestinationZoneId = Convert.ToInt64(destinationZoneId.Value),
                    OriginationZoneId = Convert.ToInt64(originationZoneId.Value),
                    OperatorId = Convert.ToInt64(operatorId.Value),

                    Rate = rate != null ? Convert.ToDecimal(rate.Value) : default(decimal),
                    RateTypeId = rateTypeId != null && rateTypeId.Value != null ? Convert.ToInt32(rateTypeId.Value) : default(int),
                    TrafficDirection = Convert.ToInt32(trafficDirection.Value),
                    CurrencyId = Convert.ToInt32(currency.Value),
                    InvoiceMeasures = new InvoiceMeasures
                    {
                        FromDate = billingPeriodFrom != null ? Convert.ToDateTime(billingPeriodFrom.Value) : default(DateTime),
                        ToDate = billingPeriodTo != null ? Convert.ToDateTime(billingPeriodTo.Value) : default(DateTime),
                        TotalBillingDuration = Convert.ToDecimal(totalBillingDuration.Value ?? 0.0),
                        Amount = amountValue,
                        Amount_OrigCurr = Convert.ToDecimal(amount_OrigCurr == null ? 0.0 : amount_OrigCurr.Value ?? 0.0),
                        NumberOfCalls = Convert.ToInt32(countCDRs.Value ?? 00),
                    }
                };
                return invoiceBillingRecord;
            }
            return null;
        }
        public class InvoiceMeasures
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public Decimal TotalBillingDuration { get; set; }
            public decimal Amount { get; set; }
            public decimal Amount_OrigCurr { get; set; }
            public int NumberOfCalls { get; set; }
        }
        public class InvoiceBillingRecord
        {
            public InvoiceMeasures InvoiceMeasures { get; set; }
            public long DestinationZoneId { get; set; }
            public long OriginationZoneId { get; set; }
            public long OperatorId { get; set; }
            public decimal Rate { get; set; }
            public int RateTypeId { get; set; }
            public int TrafficDirection { get; set; }
            public int CurrencyId { get; set; }

        }
        public class InvoiceSMSMeasures
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public decimal Amount { get; set; }
            public decimal Amount_OrigCurr { get; set; }
            public int NumberOfSMS { get; set; }
        }
        public class InvoiceSMSBillingRecord
        {
            public InvoiceSMSMeasures InvoiceMeasures { get; set; }
            public long DestinationMobileNetworkId { get; set; }
            public long OriginationMobileNetworkId { get; set; }
            public long OperatorId { get; set; }
            public decimal Rate { get; set; }
            public int RateTypeId { get; set; }
            public int BillingType { get; set; }
            public int CurrencyId { get; set; }
        }
    }
}
