using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.QualityNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using Vanrise.InvToAccBalanceRelation.Business;
using Vanrise.InvToAccBalanceRelation.Entities;
using Vanrise.NumberingPlan.Business;
using Vanrise.Common.MainExtensions;

namespace Retail.QualityNet.Business
{
    public class QualityNetInvoiceGenerator : InvoiceGenerator
    {
        Guid _acountBEDefinitionId { get; set; }
        Guid _mainDataRecordStorageId { get; set; }

        static Guid InternationalServiceTypeId = new Guid("dc1e29af-a172-4539-88ab-24210d7b0fea");
        static Guid CountryInArabicBusinessEntityId = new Guid("3774cbe1-c43c-4b60-9f8b-86ff68256f6f");

        NumberToTextCurrency textCurrency = NumberToTextCurrency.KD;

        public QualityNetInvoiceGenerator(Guid acountBEDefinitionId, Guid mainDataRecordStorageId)
        {
            this._acountBEDefinitionId = acountBEDefinitionId;
            this._mainDataRecordStorageId = mainDataRecordStorageId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            FinancialAccountData financialAccountData = new FinancialAccountManager().GetFinancialAccountData(_acountBEDefinitionId, context.PartnerId);
            if (context.FromDate < financialAccountData.FinancialAccount.BED || context.ToDate > financialAccountData.FinancialAccount.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;
            }

            int currencyId = new AccountBEManager().GetCurrencyId(this._acountBEDefinitionId, financialAccountData.Account.AccountId);

            var qualityNetInvoiceGeneratorContext = new QualityNetInvoiceGeneratorContext()
            {
                IssueDate = context.IssueDate,
                FinancialAccount = financialAccountData.Account,
                CurrencyId = currencyId,
                FromDate = context.FromDate,
                ToDate = context.ToDate
            };

            var invoiceItems = GetQualityNetCDRsByDID(qualityNetInvoiceGeneratorContext);

            if (invoiceItems == null || invoiceItems.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            InvoiceDetails retailSubscriberInvoiceDetails = BuildGeneratedInvoiceDetails(invoiceItems, currencyId, financialAccountData.Account);

            context.Invoice = new GeneratedInvoice
            {
                InvoiceDetails = retailSubscriberInvoiceDetails,
                InvoiceItemSets = qualityNetInvoiceGeneratorContext.GeneratedInvoiceItemSets,
            };

            SetInvoiceBillingTransactions(context, retailSubscriberInvoiceDetails);
        }

        #region Private Methods

        private Dictionary<string, List<QualityNetCDR>> GetQualityNetCDRsByDID(QualityNetInvoiceGeneratorContext context)
        {
            BillingCDRsByDID billingCDRsByDID = LoadAndStructureCDRData(context.FromDate, context.ToDate, context.FinancialAccount.AccountId);

            if (billingCDRsByDID == null || billingCDRsByDID.Count == 0)
                return null;

            var saleZoneManager = new SaleZoneManager();
            var countryManager = new CountryManager();
            var businessEntityManager = new BusinessEntityManager();
            var currencyExchangeRateManager = new CurrencyExchangeRateManager();

            Dictionary<string, List<QualityNetCDR>> cdrsPerDID = new Dictionary<string, List<QualityNetCDR>>();
            Dictionary<string, InternationalQualityNetCDRsSummary> internationalCDRsSummaryPerDID = new Dictionary<string, InternationalQualityNetCDRsSummary>();

            int normalPrecisionValue = new GeneralSettingsManager().GetNormalPrecisionValue();
            decimal internationalGrandTotalAmount = 0;

            NumberToArabicText numberToArabicText = new NumberToArabicText();

            foreach (var billingCDRkvp in billingCDRsByDID)
            {
                var calling = billingCDRkvp.Key;
                var billingCDRs = billingCDRkvp.Value;

                if (billingCDRs == null || billingCDRs.Count == 0)
                    continue;

                List<QualityNetCDR> qualityNetCDRs = new List<QualityNetCDR>();

                decimal internationalTotalAmount = 0;
                int numberOfInternationalCalls = 0;

                foreach (var billingCDR in billingCDRs)
                {
                    QualityNetCDR qualityNetCDR = new QualityNetCDR
                    {
                        SubscriberAccountId = billingCDR.SubscriberAccountId,
                        AttemptDateTime = billingCDR.AttemptDateTime,
                        CallingNumber = billingCDR.CallingNumber,
                        CalledNumber = billingCDR.CalledNumber,
                        DurationInSeconds = billingCDR.DurationInSeconds,
                        SaleCurrencyId = billingCDR.SaleCurrencyId,
                        ServiceTypeId = billingCDR.ServiceTypeId,
                        TotalNumberOfCalls = 1
                    };

                    if (billingCDR.ZoneId.HasValue)
                    {
                        var zone = saleZoneManager.GetSaleZone(billingCDR.ZoneId.Value);
                        if (zone != null)
                        {
                            qualityNetCDR.Country = countryManager.GetCountryName(zone.CountryId);
                            qualityNetCDR.CountryInArabic = businessEntityManager.GetEntityDescription(CountryInArabicBusinessEntityId, zone.CountryId);
                        }
                    }

                    qualityNetCDR.TotalAmount = Decimal.Round(currencyExchangeRateManager.ConvertValueToCurrency(billingCDR.SaleAmount, qualityNetCDR.SaleCurrencyId, context.CurrencyId, qualityNetCDR.AttemptDateTime), normalPrecisionValue);
                    qualityNetCDR.TotalAmountInArabicWords = numberToArabicText.ConvertNumberToText(internationalTotalAmount, textCurrency);

                    qualityNetCDRs.Add(qualityNetCDR);

                    if (billingCDR.ServiceTypeId == InternationalServiceTypeId)
                    {
                        numberOfInternationalCalls++;
                        internationalTotalAmount += qualityNetCDR.TotalAmount;
                        internationalGrandTotalAmount += qualityNetCDR.TotalAmount;
                    }
                }

                cdrsPerDID.Add(calling, qualityNetCDRs);

                if (numberOfInternationalCalls > 0)
                {
                    InternationalQualityNetCDRsSummary internationalQualityNetCDRsSummary = new InternationalQualityNetCDRsSummary()
                    {
                        CallingNumber = calling,
                        TotalNumberOfCalls = numberOfInternationalCalls,
                        TotalAmount = internationalTotalAmount,
                        TotalAmountInArabicWords = numberToArabicText.ConvertNumberToText(internationalTotalAmount, textCurrency),
                        GrandTotalAmount = internationalGrandTotalAmount
                    };
                    internationalCDRsSummaryPerDID.Add(calling, internationalQualityNetCDRsSummary);
                }
            }

            if (cdrsPerDID.Count > 0)
                BuildGeneratedInvoiceItemSet(context, cdrsPerDID, internationalCDRsSummaryPerDID);

            return cdrsPerDID;
        }

        private void BuildGeneratedInvoiceItemSet(QualityNetInvoiceGeneratorContext context, Dictionary<string, List<QualityNetCDR>> cdrsPerDID, Dictionary<string, InternationalQualityNetCDRsSummary> internationalCDRsSummaryPerDID)
        {
            context.GeneratedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            GeneratedInvoiceItemSet invoiceItemSet = new GeneratedInvoiceItemSet()
            {
                SetName = "GroupedByDID",
                Items = new List<GeneratedInvoiceItem>()
            };

            foreach (var cdrs in cdrsPerDID)
            {
                if (cdrs.Value == null || cdrs.Value.Count == 0)
                    continue;

                foreach (var cdr in cdrs.Value)
                {
                    invoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Name = " ",
                        Details = cdr
                    });
                }
            }

            context.GeneratedInvoiceItemSets.Add(invoiceItemSet);

            GeneratedInvoiceItemSet internationalInvoiceItemSet = new GeneratedInvoiceItemSet()
            {
                SetName = "InternationalCDRSGroupedByDID",
                Items = new List<GeneratedInvoiceItem>()
            };

            foreach (var summary in internationalCDRsSummaryPerDID)
            {
                if (summary.Value == null)
                    continue;

                internationalInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                {
                    Name = " ",
                    Details = summary.Value
                });
            }

            context.GeneratedInvoiceItemSets.Add(internationalInvoiceItemSet);
        }

        private BillingCDRsByDID LoadAndStructureCDRData(DateTime fromDate, DateTime toDate, long accountId)
        {
            var columns = new List<string> { "FinancialAccountId", "AttemptDateTime", "SaleDurationInSeconds", "Calling", "Called", "SaleAmount", "SubscriberAccountId", "Zone", "SaleCurrencyId", "ServiceType" };

            var cdrData = new DataRecordStorageManager().GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
            {
                Query = new DataRecordQuery()
                {
                    DataRecordStorageIds = new List<Guid> { this._mainDataRecordStorageId },
                    Columns = columns,
                    ColumnTitles = columns,
                    FilterGroup = new RecordFilterGroup
                    {
                        Filters = new List<RecordFilter>
                       {
                           new NumberRecordFilter{
                               FieldName = "FinancialAccountId",
                               CompareOperator = NumberRecordFilterOperator.Equals,
                               Value = accountId,
                           }
                       }
                    },
                    LimitResult = int.MaxValue,
                    FromTime = fromDate,
                    ToTime = toDate,
                    SortColumns = new List<SortColumn> { new SortColumn { FieldName = "AttemptDateTime" } },
                },
                SortByColumnName = "FieldValues.AttemptDateTime.Value"
            }) as BigResult<DataRecordDetail>;

            if (cdrData == null || cdrData.Data == null)
                return null;

            BillingCDRsByDID billingCDRsByDID = new BillingCDRsByDID();

            foreach (var dataRecordDetail in cdrData.Data)
            {
                var fieldValues = dataRecordDetail.FieldValues;

                DataRecordFieldValue saleAmountField = fieldValues.GetRecord("SaleAmount");
                saleAmountField.ThrowIfNull("saleAmountField");
                decimal saleAmount = Convert.ToDecimal(saleAmountField.Value ?? 0.0);

                if (saleAmount == 0)
                    continue;

                DataRecordFieldValue subscriberAccountField = fieldValues.GetRecord("SubscriberAccountId");
                subscriberAccountField.ThrowIfNull("subscriberAccountField");

                DataRecordFieldValue callingField = fieldValues.GetRecord("Calling");
                callingField.ThrowIfNull("callingField");

                var calling = Convert.ToString(callingField.Value);
                var billingCDRsPerDID = billingCDRsByDID.GetOrCreateItem(calling);

                DataRecordFieldValue attemptDateTimeField = fieldValues.GetRecord("AttemptDateTime");
                attemptDateTimeField.ThrowIfNull("attemptDateTimeField");

                DataRecordFieldValue durationInSecondsField = fieldValues.GetRecord("SaleDurationInSeconds");
                durationInSecondsField.ThrowIfNull("durationInSecondsField");

                DataRecordFieldValue calledField = fieldValues.GetRecord("Called");
                calledField.ThrowIfNull("calledField");

                DataRecordFieldValue zoneField = fieldValues.GetRecord("Zone");
                zoneField.ThrowIfNull("zoneField");

                DataRecordFieldValue saleCurrencyIdField = fieldValues.GetRecord("SaleCurrencyId");
                saleCurrencyIdField.ThrowIfNull("saleCurrencyIdField");

                DataRecordFieldValue serviceTypeIdField = fieldValues.GetRecord("ServiceType");
                serviceTypeIdField.ThrowIfNull("serviceTypeIdField");

                BillingCDR billingCDR = new BillingCDR
                {
                    SubscriberAccountId = Convert.ToInt64(subscriberAccountField.Value),
                    AttemptDateTime = Convert.ToDateTime(attemptDateTimeField.Value),
                    CallingNumber = calling,
                    CalledNumber = Convert.ToString(calledField.Value),
                    SaleAmount = saleAmount,
                    DurationInSeconds = Convert.ToDecimal(durationInSecondsField.Value),
                    ZoneId = zoneField.Value != null ? Convert.ToInt64(zoneField.Value) : default(long?),
                    SaleCurrencyId = Convert.ToInt32(saleCurrencyIdField.Value),
                    ServiceTypeId = new Guid(serviceTypeIdField.Value.ToString())
                };

                billingCDRsPerDID.Add(billingCDR);
            }

            return billingCDRsByDID.Count > 0 ? billingCDRsByDID : null;
        }

        private InvoiceDetails BuildGeneratedInvoiceDetails(Dictionary<string, List<QualityNetCDR>> qualityNetCDRsPerDID, int currencyId, Account account)
        {
            if (qualityNetCDRsPerDID == null || qualityNetCDRsPerDID.Count == 0)
                return null;

            InvoiceDetails retailSubscriberInvoiceDetails = new InvoiceDetails();
            foreach (var qualityNetCDRkvp in qualityNetCDRsPerDID)
            {
                var qualityNetCDRs = qualityNetCDRkvp.Value;
                if (qualityNetCDRs == null || qualityNetCDRs.Count == 0)
                    continue;

                foreach (var qualityNetCDR in qualityNetCDRs)
                {
                    retailSubscriberInvoiceDetails.TotalNumberOfCalls += qualityNetCDR.TotalNumberOfCalls;
                    retailSubscriberInvoiceDetails.TotalDurationInMin += qualityNetCDR.DurationInSeconds / 60;
                    retailSubscriberInvoiceDetails.GrandTotalAmount += qualityNetCDR.TotalAmount;

                    if (qualityNetCDR.ServiceTypeId == InternationalServiceTypeId)
                    {
                        retailSubscriberInvoiceDetails.InternationalTotalNumberOfCalls += qualityNetCDR.TotalNumberOfCalls;
                        retailSubscriberInvoiceDetails.InternationalTotalDurationInMin += qualityNetCDR.DurationInSeconds / 60;
                        retailSubscriberInvoiceDetails.InternationalGrandTotalAmount += qualityNetCDR.TotalAmount;
                    }
                }
            }

            retailSubscriberInvoiceDetails.SubscriberAccountId = account.AccountId;
            retailSubscriberInvoiceDetails.CurrencyId = currencyId;

            return retailSubscriberInvoiceDetails;
        }

        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, InvoiceDetails invoiceDetails)
        {
            InvoiceType invoiceType = new InvoiceTypeManager().GetInvoiceType(context.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", context.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings", context.InvoiceTypeId);
            QualityNetInvoiceTypeSettings invoiceSettings = invoiceType.Settings.ExtendedSettings.CastWithValidate<QualityNetInvoiceTypeSettings>("invoiceType.Settings.ExtendedSettings");

            Guid accountTypeId = GetBillingTransactionAccountTypeId(context.InvoiceTypeId, context.PartnerId, context.IssueDate);

            var billingTransaction = new GeneratedInvoiceBillingTransaction()
            {
                AccountTypeId = accountTypeId,
                AccountId = context.PartnerId,
                TransactionTypeId = invoiceSettings.InvoiceTransactionTypeId,
                Amount = invoiceDetails.GrandTotalAmount,
                CurrencyId = invoiceDetails.CurrencyId
            };

            billingTransaction.Settings = new GeneratedInvoiceBillingTransactionSettings();
            billingTransaction.Settings.UsageOverrides = new List<GeneratedInvoiceBillingTransactionUsageOverride>();

            foreach (Guid usageTransactionTypeId in invoiceSettings.UsageTransactionTypeIds)
            {
                billingTransaction.Settings.UsageOverrides.Add(new GeneratedInvoiceBillingTransactionUsageOverride()
                {
                    TransactionTypeId = usageTransactionTypeId
                });
            }

            context.BillingTransactions = new List<GeneratedInvoiceBillingTransaction>() { billingTransaction };
        }

        private Guid GetBillingTransactionAccountTypeId(Guid invoiceTypeId, string accountId, DateTime effectiveOn)
        {
            List<BalanceAccountInfo> invoiceBalanceAccounts = new InvToAccBalanceRelationDefinitionManager().GetInvoiceBalanceAccounts(invoiceTypeId, accountId, effectiveOn);

            invoiceBalanceAccounts.ThrowIfNull("invoiceBalanceAccounts", accountId);
            if (invoiceBalanceAccounts.Count == 0)
                throw new DataIntegrityValidationException("invoiceBalanceAccounts");

            return invoiceBalanceAccounts.FirstOrDefault().AccountTypeId;
        }

        #endregion

        #region Classes

        private class QualityNetInvoiceGeneratorContext
        {
            public List<GeneratedInvoiceItemSet> GeneratedInvoiceItemSets { get; set; }
            public Account FinancialAccount { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public int CurrencyId { get; set; }
        }

        private class BillingCDR
        {
            public long SubscriberAccountId { get; set; }
            public DateTime AttemptDateTime { get; set; }
            public Decimal SaleAmount { get; set; }
            public String CalledNumber { get; set; }
            public String CallingNumber { get; set; }
            public Decimal DurationInSeconds { get; set; }
            public long? ZoneId { get; set; }
            public int SaleCurrencyId { get; set; }
            public Guid ServiceTypeId { get; set; }
        }

        private class QualityNetCDR
        {
            public long SubscriberAccountId { get; set; }
            public DateTime AttemptDateTime { get; set; }
            public String CallingNumber { get; set; }
            public String CalledNumber { get; set; }
            public Guid ServiceTypeId { get; set; }
            public Decimal DurationInSeconds { get; set; }
            public int SaleCurrencyId { get; set; }
            public string Country { get; set; }
            public string CountryInArabic { get; set; }
            public int TotalNumberOfCalls { get; set; }
            public decimal TotalAmount { get; set; }
            public string TotalAmountInArabicWords { get; set; }
        }

        private class InternationalQualityNetCDRsSummary
        {
            public String CallingNumber { get; set; }
            public int TotalNumberOfCalls { get; set; }
            public decimal TotalAmount { get; set; }
            public string TotalAmountInArabicWords { get; set; }
            public decimal GrandTotalAmount { get; set; }
        }

        private class BillingCDRsByDID : Dictionary<string, List<BillingCDR>> { }

        // private class BillingCDRsByDIDByAccount : Dictionary<long, BillingCDRsByDID> { }

        #endregion
    }
}