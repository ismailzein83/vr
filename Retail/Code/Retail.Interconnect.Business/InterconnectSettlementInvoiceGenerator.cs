using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.Interconnect.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;
using ConfigManager = Retail.BusinessEntity.Business.ConfigManager;

namespace Retail.Interconnect.Business
{
    public class InterconnectSettlementInvoiceGenerator : InvoiceGenerator
    {
        Guid _accountBEDefinitionId;
        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;

        public InterconnectSettlementInvoiceGenerator(Guid AccountBEDefinitionId, Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId)
        {
            _accountBEDefinitionId = AccountBEDefinitionId;
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = supplierInvoiceTypeId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            bool isApplicableToCustomer;
            bool isApplicableToSupplier;
            List<InvoiceAvailableForSettlement> availableCustomerInvoices;
            List<InvoiceAvailableForSettlement> availableSupplierInvoices;
            int currencyId;
            PrepareDataForProcessing(context.PartnerId, context.InvoiceTypeId, context.CustomSectionPayload, out isApplicableToSupplier, out isApplicableToCustomer, out availableCustomerInvoices, out availableSupplierInvoices, out currencyId);


            IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices = null;
            IEnumerable<InvoiceItem> customerInvoiceItems = null;

            IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices = null;
            IEnumerable<InvoiceItem> supplierInvoiceItems = null;

            string errorMessage;
            GenerateInvoiceResult generateInvoiceResult;
            List<long> invoiceToSettleIds;

            if (!TryLoadInvoicesAndItemSetNames(isApplicableToCustomer, availableCustomerInvoices, isApplicableToSupplier, availableSupplierInvoices, context.FromDate, context.ToDate, out customerInvoices, out customerInvoiceItems, out supplierInvoices, out supplierInvoiceItems, out errorMessage, out generateInvoiceResult, out invoiceToSettleIds))
            {
                context.ErrorMessage = errorMessage;
                context.GenerateInvoiceResult = generateInvoiceResult;
                return;
            }

            context.InvoiceToSettleIds = invoiceToSettleIds;

            List<SettlementInvoiceItemDetails> customerInvoiceItemSet;
            List<SettlementInvoiceItemDetails> supplierInvoiceItemSet;
            Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryByCurrency;
            List<SettlementInvoiceDetailSummary> companySummary;
            List<SettlementInvoiceDetailSummary> systemSummary;

            List<SettlementInvoiceDetailSummary> companyRecurringChargesSummary;
            List<SettlementInvoiceDetailSummary> systemRecurringChargesSummary;

            Dictionary<String, SettlementInvoiceItemDetailByCurrency> settlementInvoiceByCurrency;

            Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice;

            BusinessEntity.Business.ConfigManager configManager = new ConfigManager();
            var invoiceSettings = configManager.GetRetailInvoiceSettings();
            if (invoiceSettings != null)
            {
                if (invoiceSettings.RequireGroupByMonth)
                {
                    if (customerInvoices != null && customerInvoices.Count() > 0)
                    {
                        foreach (var customerInvoice in customerInvoices)
                        {
                            if (customerInvoice.FromDate.Year != customerInvoice.ToDate.Year || customerInvoice.FromDate.Month != customerInvoice.ToDate.Month)
                            {
                                context.ErrorMessage = "Settlement invoice not supported. Reason: Specified customer invoices must be less than or equal to a month.";
                                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                                return;
                            }
                        }
                    }
                    if (supplierInvoices != null && supplierInvoices.Count() > 0)
                    {
                        foreach (var supplierInvoice in supplierInvoices)
                        {
                            if (supplierInvoice.FromDate.Year != supplierInvoice.ToDate.Year || supplierInvoice.FromDate.Month != supplierInvoice.ToDate.Month)
                            {
                                context.ErrorMessage = "Settlement invoice not supported. Reason: Specified supplier invoices must be less than or equal to a month.";
                                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                                return;
                            }
                        }
                    }
                }
            }

            ProcessItemSetName(customerInvoices, customerInvoiceItems, supplierInvoices, supplierInvoiceItems, availableCustomerInvoices, availableSupplierInvoices, out companyRecurringChargesSummary, out systemRecurringChargesSummary, out customerInvoiceItemSet, out supplierInvoiceItemSet, out settlementInvoiceItemSummaryByCurrency, out companySummary, out systemSummary, out settlementInvoiceCurrencyByInvoice, out settlementInvoiceByCurrency);

            context.Invoice = BuildGeneratedInvoice(customerInvoiceItemSet, supplierInvoiceItemSet, settlementInvoiceItemSummaryByCurrency, companySummary, systemSummary, companyRecurringChargesSummary, systemRecurringChargesSummary, settlementInvoiceCurrencyByInvoice, isApplicableToCustomer, isApplicableToSupplier, settlementInvoiceByCurrency, currencyId, context.IssueDate);
        }

        private void ProcessItemSetName(IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices, IEnumerable<InvoiceItem> customerInvoiceItems, IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices,
            IEnumerable<InvoiceItem> supplierInvoiceItems, List<InvoiceAvailableForSettlement> availableCustomerInvoices, List<InvoiceAvailableForSettlement> availableSupplierInvoices, out List<SettlementInvoiceDetailSummary> companyRecurringChargesSummary,
          out List<SettlementInvoiceDetailSummary> systemRecurringChargesSummary, out List<SettlementInvoiceItemDetails> customerInvoiceItemSet, out List<SettlementInvoiceItemDetails> supplierInvoiceItemSet, out Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryByCurrency, out List<SettlementInvoiceDetailSummary> companySummary, out List<SettlementInvoiceDetailSummary> systemSummary, out Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice, out Dictionary<String, SettlementInvoiceItemDetailByCurrency> settlementInvoiceByCurrency)
        {

            customerInvoiceItemSet = null;
            supplierInvoiceItemSet = null;

            var customerInvoiceByCurrencyItemDetailsByInvoice = GenerateInterconnectInvoiceByCurrencyItemDetailsByCurrencyDic(customerInvoices, customerInvoiceItems);
            var supplierInvoiceByCurrencyItemDetailsByInvoice = GenerateInterconnectInvoiceByCurrencyItemDetailsByCurrencyDic(supplierInvoices, supplierInvoiceItems);

            settlementInvoiceItemSummaryByCurrency = new Dictionary<int, SettlementInvoiceItemSummaryDetail>();

            settlementInvoiceCurrencyByInvoice = new Dictionary<long, List<SettlementInvoiceDetailByCurrency>>();

            settlementInvoiceByCurrency = new Dictionary<String, SettlementInvoiceItemDetailByCurrency>();

            if (customerInvoices != null && customerInvoiceByCurrencyItemDetailsByInvoice != null)
            {
                customerInvoiceItemSet = new List<SettlementInvoiceItemDetails>();

                foreach (var customerInvoice in customerInvoices)
                {
                    var customerInvoiceDetails = customerInvoice.Details as InterconnectInvoiceDetails;
                    if (customerInvoiceDetails == null)
                        continue;
                    bool multipleCurrencies = false;
                    var invoiceItemsByCurrency = customerInvoiceByCurrencyItemDetailsByInvoice.GetRecord(customerInvoice.InvoiceId);

                    if (invoiceItemsByCurrency != null)
                    {
                        var settlementInvoiceCurrency = settlementInvoiceCurrencyByInvoice.GetOrCreateItem(customerInvoice.InvoiceId);

                        foreach (var currency in invoiceItemsByCurrency.Currencies)
                        {
                            var months = invoiceItemsByCurrency.MonthsByCurrency.GetRecord(currency);
                            if (months == null)
                                continue;

                            foreach (var month in months)
                            {
                                var invoiceItemDetails = month as InterconnectInvoiceByCurrencyItemDetails;

                                if (invoiceItemDetails != null && invoiceItemDetails.CurrencyId != customerInvoiceDetails.InterconnectCurrencyId)
                                    multipleCurrencies = true;

                                if (invoiceItemDetails != null && availableCustomerInvoices.Any(x => x.InvoiceId == customerInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId && x.IsSelected))
                                {
                                    var settlementInvoicedetail = settlementInvoiceByCurrency.GetOrCreateItem(string.Format("{0}_{1}", invoiceItemDetails.CurrencyId, invoiceItemDetails.Month), () =>
                                    {
                                        return new SettlementInvoiceItemDetailByCurrency()
                                        {
                                            CurrencyId = invoiceItemDetails.CurrencyId,
                                            InvoiceId = customerInvoice.InvoiceId,
                                            Month = invoiceItemDetails.Month,
                                            FromDate = invoiceItemDetails.FromDate,
                                            ToDate = invoiceItemDetails.ToDate,
                                        };
                                    });

                                    OriginalDataCurrrency originalDataCurrrency;

                                    var settlementInvoiceDetailByCurrency = new SettlementInvoiceDetailByCurrency();
                                    settlementInvoiceDetailByCurrency.InvoiceId = customerInvoice.InvoiceId;
                                    settlementInvoiceDetailByCurrency.CurrencyId = invoiceItemDetails.CurrencyId;
                                    settlementInvoiceDetailByCurrency.TotalDuration = invoiceItemDetails.Duration;
                                    settlementInvoiceDetailByCurrency.NumberOfCalls = invoiceItemDetails.NumberOfCalls;

                                    if (customerInvoiceDetails.OriginalAmountByCurrency != null && customerInvoiceDetails.OriginalAmountByCurrency.TryGetValue(invoiceItemDetails.CurrencyId, out originalDataCurrrency) && originalDataCurrrency.IncludeOriginalAmountInSettlement && originalDataCurrrency.OriginalAmount.HasValue)
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToSystemNumberOfCalls += customerInvoiceDetails.TotalNumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToSystemFullAmount += originalDataCurrrency.OriginalAmount.Value;


                                        settlementInvoiceDetailByCurrency.OriginalAmount = originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceDetailByCurrency.TotalFullAmount = originalDataCurrrency.OriginalAmount.Value;

                                        settlementInvoicedetail.DueToSystemAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToSystemAmountWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToSystemNumberOfCalls += customerInvoiceDetails.TotalNumberOfCalls;
                                        settlementInvoicedetail.DueToSystemFullAmount += originalDataCurrrency.OriginalAmount.Value;

                                    }
                                    else
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmount += invoiceItemDetails.Amount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountWithTaxes += invoiceItemDetails.AmountWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToSystemNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = invoiceItemDetails.Amount;

                                        settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceDetailByCurrency.TotalTrafficAmount = invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceDetailByCurrency.TotalSMSAmount = invoiceItemDetails.TotalSMSAmount;
                                        settlementInvoiceDetailByCurrency.TotalFullAmount = invoiceItemDetails.TotalFullAmount;

                                        settlementInvoicedetail.DueToSystemAmount += invoiceItemDetails.AmountWithTaxes;
                                        settlementInvoicedetail.DueToSystemAmountWithTaxes += invoiceItemDetails.AmountWithTaxes;
                                        settlementInvoicedetail.DueToSystemFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoicedetail.DueToSystemNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoicedetail.DueToSystemAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoicedetail.DueToSystemTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoicedetail.DueToSystemTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;

                                    }
                                    settlementInvoiceCurrency.Add(settlementInvoiceDetailByCurrency);
                                }
                            }
                        }
                    }

                    var settlementInvoiceItemDetails = new SettlementInvoiceItemDetails
                    {

                        Amount = multipleCurrencies ? default(decimal?) : customerInvoiceDetails.TotalInvoiceAmount,
                        DurationInSeconds = customerInvoiceDetails.Duration,
                        CurrencyId = multipleCurrencies ? default(int?) : customerInvoiceDetails.InterconnectCurrencyId,
                        InvoiceId = customerInvoice.InvoiceId,
                        InvoiceTypeId = customerInvoice.InvoiceTypeId,
                        TotalNumberOfSMSs = customerInvoiceDetails.TotalNumberOfSMS,
                        TotalNumberOfCalls = customerInvoiceDetails.TotalNumberOfCalls,
                        IssueDate = customerInvoice.IssueDate,
                        SerialNumber = customerInvoice.SerialNumber,
                        MultipleCurrencies = multipleCurrencies,
                        DueDate = customerInvoice.DueDate,
                        FromDate = customerInvoice.FromDate,
                        OriginalAmount = !multipleCurrencies && customerInvoiceDetails.OriginalAmountByCurrency != null && customerInvoiceDetails.OriginalAmountByCurrency.Count > 0 ? customerInvoiceDetails.OriginalAmountByCurrency.First().Value.OriginalAmount : default(decimal?),
                        ToDate = customerInvoice.ToDate
                    };

                    customerInvoiceItemSet.Add(settlementInvoiceItemDetails);
                }
            }

            if (supplierInvoices != null && supplierInvoiceByCurrencyItemDetailsByInvoice != null)
            {
                supplierInvoiceItemSet = new List<SettlementInvoiceItemDetails>();

                foreach (var supplierInvoice in supplierInvoices)
                {
                    var supplierInvoiceDetails = supplierInvoice.Details as InterconnectInvoiceDetails;
                    if (supplierInvoiceDetails == null)
                        continue;
                    bool isOriginalAmountSetted = supplierInvoiceDetails.IsOriginalAmountSetted;
                    bool multipleCurrencies = false;

                    var invoiceItemsByCurrency = supplierInvoiceByCurrencyItemDetailsByInvoice.GetRecord(supplierInvoice.InvoiceId);
                    if (invoiceItemsByCurrency != null)
                    {
                        var settlementInvoiceCurrency = settlementInvoiceCurrencyByInvoice.GetOrCreateItem(supplierInvoice.InvoiceId);

                        foreach (var currency in invoiceItemsByCurrency.Currencies)
                        {
                            var months = invoiceItemsByCurrency.MonthsByCurrency.GetRecord(currency);
                            if (months == null)
                                continue;

                            foreach (var month in months)
                            {
                                var invoiceItemDetails = month as InterconnectInvoiceByCurrencyItemDetails;

                                if (invoiceItemDetails != null && invoiceItemDetails.CurrencyId != supplierInvoiceDetails.InterconnectCurrencyId)
                                    multipleCurrencies = true;

                                if (invoiceItemDetails != null && availableSupplierInvoices.Any(x => x.InvoiceId == supplierInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId && x.IsSelected))
                                {
                                    var settlementInvoicedetail = settlementInvoiceByCurrency.GetOrCreateItem(string.Format("{0}_{1}", invoiceItemDetails.CurrencyId, invoiceItemDetails.Month), () =>
                                    {
                                        return new SettlementInvoiceItemDetailByCurrency()
                                        {
                                            CurrencyId = invoiceItemDetails.CurrencyId,
                                            InvoiceId = supplierInvoice.InvoiceId,
                                            Month = invoiceItemDetails.Month,
                                            FromDate = invoiceItemDetails.FromDate,
                                            ToDate = invoiceItemDetails.ToDate,
                                        };
                                    });

                                    OriginalDataCurrrency originalDataCurrrency;

                                    var settlementInvoiceDetailByCurrency = new SettlementInvoiceDetailByCurrency();
                                    settlementInvoiceDetailByCurrency.InvoiceId = supplierInvoice.InvoiceId;
                                    settlementInvoiceDetailByCurrency.CurrencyId = invoiceItemDetails.CurrencyId;
                                    settlementInvoiceDetailByCurrency.TotalDuration = invoiceItemDetails.Duration;
                                    settlementInvoiceDetailByCurrency.NumberOfCalls = invoiceItemDetails.NumberOfCalls;

                                    if (supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.TryGetValue(invoiceItemDetails.CurrencyId, out originalDataCurrrency) && originalDataCurrrency.IncludeOriginalAmountInSettlement && originalDataCurrrency.OriginalAmount.HasValue)
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyAmountWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyNumberOfCalls += supplierInvoiceDetails.TotalNumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyFullAmount += originalDataCurrrency.OriginalAmount.Value;
                                        // settlementInvoiceItemSummaryDetail.DueToCompanyTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceDetailByCurrency.TotalFullAmount = originalDataCurrrency.OriginalAmount.Value;

                                        settlementInvoicedetail.DueToCompanyAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToCompanyAmountWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToCompanyNumberOfCalls += supplierInvoiceDetails.TotalNumberOfCalls;
                                        settlementInvoicedetail.DueToCompanyFullAmount += originalDataCurrrency.OriginalAmount.Value;

                                    }
                                    else
                                    {
                                        isOriginalAmountSetted = false;

                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyAmount += invoiceItemDetails.Amount;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyAmountWithTaxes += invoiceItemDetails.AmountWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCompanyTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;


                                        settlementInvoiceDetailByCurrency.OriginalAmount = invoiceItemDetails.Amount;

                                        settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceDetailByCurrency.TotalTrafficAmount = invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceDetailByCurrency.TotalSMSAmount = invoiceItemDetails.TotalSMSAmount;
                                        settlementInvoiceDetailByCurrency.TotalFullAmount = invoiceItemDetails.TotalFullAmount;

                                        settlementInvoicedetail.DueToCompanyAmount += invoiceItemDetails.Amount;
                                        settlementInvoicedetail.DueToCompanyAmountWithTaxes += invoiceItemDetails.AmountWithTaxes;
                                        settlementInvoicedetail.DueToCompanyNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoicedetail.DueToCompanyAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoicedetail.DueToCompanyTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoicedetail.DueToCompanyFullAmount += invoiceItemDetails.TotalFullAmount;

                                        settlementInvoicedetail.DueToCompanyTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;
                                    }
                                    settlementInvoiceCurrency.Add(settlementInvoiceDetailByCurrency);

                                }
                            }
                        }
                    }




                    var settlementInvoiceItemDetails = new SettlementInvoiceItemDetails
                    {
                        Amount = multipleCurrencies ? default(decimal?) : supplierInvoiceDetails.TotalInvoiceAmount,
                        DurationInSeconds = supplierInvoiceDetails.Duration,
                        CurrencyId = multipleCurrencies ? default(int?) : supplierInvoiceDetails.InterconnectCurrencyId,
                        InvoiceId = supplierInvoice.InvoiceId,
                        InvoiceTypeId = supplierInvoice.InvoiceTypeId,
                        TotalNumberOfSMSs = supplierInvoiceDetails.TotalNumberOfSMS,
                        TotalNumberOfCalls = supplierInvoiceDetails.TotalNumberOfCalls,
                        SerialNumber = supplierInvoice.SerialNumber,
                        IssueDate = supplierInvoice.IssueDate,
                        MultipleCurrencies = multipleCurrencies,
                        DueDate = supplierInvoice.DueDate,
                        FromDate = supplierInvoice.FromDate,
                        OriginalAmount = !multipleCurrencies && supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.Count > 0 ? supplierInvoiceDetails.OriginalAmountByCurrency.First().Value.OriginalAmount : default(decimal?),
                        ToDate = supplierInvoice.ToDate,
                        IsOriginalAmountSetted = isOriginalAmountSetted
                    };
                    supplierInvoiceItemSet.Add(settlementInvoiceItemDetails);
                }
            }


            systemSummary = null;
            companySummary = null;
            companyRecurringChargesSummary = null;
            systemRecurringChargesSummary = null;

            if (settlementInvoiceByCurrency != null && settlementInvoiceByCurrency.Count > 0)
            {
                foreach (var settlementInvoiceItemByCurrency in settlementInvoiceByCurrency)
                {
                    decimal sum = settlementInvoiceItemByCurrency.Value.DueToCompanyAmountWithTaxes - settlementInvoiceItemByCurrency.Value.DueToSystemAmountWithTaxes;
                    if (sum > 0)
                    {
                        settlementInvoiceItemByCurrency.Value.DueToCompanyDifference = sum;
                    }
                    else if (sum < 0)
                    {
                        settlementInvoiceItemByCurrency.Value.DueToSystemDifference = Math.Abs(sum);
                    }
                }
            }

            if (settlementInvoiceItemSummaryByCurrency != null && settlementInvoiceItemSummaryByCurrency.Count > 0)
            {
                foreach (var settlementInvoiceItemSummary in settlementInvoiceItemSummaryByCurrency)
                {
                    decimal difference = settlementInvoiceItemSummary.Value.DueToCompanyFullAmount - settlementInvoiceItemSummary.Value.DueToSystemFullAmount;
                    if (difference > 0)
                    {
                        settlementInvoiceItemSummary.Value.DueToCompanyDifference = difference;
                        if (companySummary == null)
                            companySummary = new List<SettlementInvoiceDetailSummary>();

                        companySummary.Add(new SettlementInvoiceDetailSummary
                        {
                            Amount = difference,
                            CurrencyId = settlementInvoiceItemSummary.Value.CurrencyId,
                            CurrencyIdDescription = settlementInvoiceItemSummary.Value.CurrencyIdDescription,
                            // OriginalAmountWithCommission = commission.HasValue ? sum + ((sum * commission.Value) / 100) : sum,
                        });

                    }
                    else if (difference < 0)
                    {
                        settlementInvoiceItemSummary.Value.DueToSystemDifference = Math.Abs(difference);

                        if (systemSummary == null)
                            systemSummary = new List<SettlementInvoiceDetailSummary>();
                        var amount = Math.Abs(difference);
                        systemSummary.Add(new SettlementInvoiceDetailSummary
                        {
                            Amount = amount,
                            CurrencyId = settlementInvoiceItemSummary.Value.CurrencyId,
                            CurrencyIdDescription = settlementInvoiceItemSummary.Value.CurrencyIdDescription,
                            //  OriginalAmountWithCommission = commission.HasValue ? amount + ((amount * commission.Value) / 100) : amount
                        });
                    }

                    if (settlementInvoiceItemSummary.Value.DueToCompanyAmountRecurringCharges > 0)
                    {
                        if (companyRecurringChargesSummary == null)
                            companyRecurringChargesSummary = new List<SettlementInvoiceDetailSummary>();
                        companyRecurringChargesSummary.Add(new SettlementInvoiceDetailSummary
                        {
                            Amount = settlementInvoiceItemSummary.Value.DueToCompanyAmountRecurringCharges,
                            CurrencyId = settlementInvoiceItemSummary.Value.CurrencyId,
                            CurrencyIdDescription = settlementInvoiceItemSummary.Value.CurrencyIdDescription,
                        });
                    }
                    if (settlementInvoiceItemSummary.Value.DueToSystemAmountRecurringCharges > 0)
                    {
                        if (systemRecurringChargesSummary == null)
                            systemRecurringChargesSummary = new List<SettlementInvoiceDetailSummary>();
                        systemRecurringChargesSummary.Add(new SettlementInvoiceDetailSummary
                        {
                            Amount = settlementInvoiceItemSummary.Value.DueToSystemAmountRecurringCharges,
                            CurrencyId = settlementInvoiceItemSummary.Value.CurrencyId,
                            CurrencyIdDescription = settlementInvoiceItemSummary.Value.CurrencyIdDescription,
                        });
                    }
                }
            }


        }
        private bool GetNoInvoiceSelected(out string errorMessage, out GenerateInvoiceResult generateInvoiceResult)
        {
            errorMessage = "No Invoices Selected.";
            generateInvoiceResult = GenerateInvoiceResult.Failed;
            return false;
        }
        private bool TryLoadInvoicesAndItemSetNames(bool isApplicableToCustomer, List<InvoiceAvailableForSettlement> availableCustomerInvoices, bool isApplicableToSupplier, List<InvoiceAvailableForSettlement> availableSupplierInvoices, DateTime fromDate, DateTime toDate, out IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices, out IEnumerable<InvoiceItem> customerInvoiceItems, out IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices, out IEnumerable<InvoiceItem> supplierInvoiceItems, out string errorMessage, out GenerateInvoiceResult generateInvoiceResult, out List<long> invoiceToSettleIds)
        {
            InvoiceItemManager _invoiceItemManager = new InvoiceItemManager();
            invoiceToSettleIds = null;
            customerInvoices = null;
            customerInvoiceItems = null;
            supplierInvoices = null;
            supplierInvoiceItems = null;
            errorMessage = null;
            generateInvoiceResult = GenerateInvoiceResult.Succeeded;



            if (isApplicableToCustomer && isApplicableToSupplier)
            {
                if ((availableCustomerInvoices == null || availableCustomerInvoices.Count == 0 || availableCustomerInvoices.All(x => x.IsSelected == false)) && (availableSupplierInvoices == null || availableSupplierInvoices.Count == 0 || availableSupplierInvoices.All(x => x.IsSelected == false)))
                {
                    return GetNoInvoiceSelected(out errorMessage, out generateInvoiceResult);
                }
            }

            if (isApplicableToCustomer && !isApplicableToSupplier)
            {
                if (availableCustomerInvoices == null || availableCustomerInvoices.Count == 0 || availableCustomerInvoices.All(x => x.IsSelected == false))
                {
                    return GetNoInvoiceSelected(out errorMessage, out generateInvoiceResult);

                }
            }
            if (isApplicableToSupplier && !isApplicableToCustomer)
            {
                if (availableSupplierInvoices == null || availableSupplierInvoices.Count == 0 || availableSupplierInvoices.All(x => x.IsSelected == false))
                {
                    return GetNoInvoiceSelected(out errorMessage, out generateInvoiceResult);
                }
            }


            if (isApplicableToCustomer && availableCustomerInvoices != null)
            {
                var selectedCustomerInvoices = availableCustomerInvoices.MapRecords(x => x.InvoiceId, x => x.IsSelected).ToList();

                invoiceToSettleIds = selectedCustomerInvoices;

                customerInvoices = GetInvoices(selectedCustomerInvoices);
                if (customerInvoices != null && customerInvoices.Count() > 0)
                {
                    if (!ValidateInvoicesDates(customerInvoices, fromDate, toDate, out errorMessage, out generateInvoiceResult))
                        return false;
                    customerInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(_customerInvoiceTypeId, selectedCustomerInvoices, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
                }
            }

            if (isApplicableToSupplier && availableSupplierInvoices != null)
            {
                if (invoiceToSettleIds == null)
                    invoiceToSettleIds = new List<long>();

                var selectedSupplierInvoices = availableSupplierInvoices.MapRecords(x => x.InvoiceId, x => x.IsSelected).ToList();

                invoiceToSettleIds.AddRange(selectedSupplierInvoices);

                supplierInvoices = GetInvoices(selectedSupplierInvoices);
                if (supplierInvoices != null && supplierInvoices.Count() > 0)
                {
                    if (!ValidateInvoicesDates(supplierInvoices, fromDate, toDate, out errorMessage, out generateInvoiceResult))
                        return false;
                    supplierInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(_supplierInvoiceTypeId, selectedSupplierInvoices, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
                }
            }



            if (isApplicableToCustomer && isApplicableToSupplier)
            {
                if ((supplierInvoices == null || supplierInvoices.Count() == 0) && (customerInvoices == null || customerInvoices.Count() == 0))
                {
                    generateInvoiceResult = GenerateInvoiceResult.NoData;
                    return false;
                }
            }

            if (isApplicableToCustomer && !isApplicableToSupplier)
            {
                if (customerInvoices == null || customerInvoices.Count() == 0)
                {
                    generateInvoiceResult = GenerateInvoiceResult.NoData;
                    return false;
                }
            }
            if (isApplicableToSupplier && !isApplicableToCustomer)
            {
                if (supplierInvoices == null || supplierInvoices.Count() == 0)
                {
                    generateInvoiceResult = GenerateInvoiceResult.NoData;
                    return false;
                }
            }
            return true;
        }
        private void PrepareDataForProcessing(string partnerId, Guid invoiceTypeId, dynamic customSectionPayload, out bool isApplicableToSupplier, out bool isApplicableToCustomer, out List<InvoiceAvailableForSettlement> availableCustomerInvoices, out List<InvoiceAvailableForSettlement> availableSupplierInvoices, out int currencyId)
        {
            var financialAccountMananger = new FinancialAccountManager();

            var settlementInvoiceTypeExtendedSettings = new InvoiceTypeManager().GetInvoiceTypeExtendedSettings(invoiceTypeId);
            var settlementInvoiceSettings = settlementInvoiceTypeExtendedSettings.CastWithValidate<InterconnectSettlementInvoiceSettings>("interconnectInvoiceSettings", invoiceTypeId);

            var accountBeDefinitionId = settlementInvoiceSettings.AccountBEDefinitionId;

            var settlementFinancialAccountData = financialAccountMananger.GetFinancialAccountData(accountBeDefinitionId, partnerId);

            if (!settlementFinancialAccountData.AccountCurrencyId.HasValue)
                throw new Exception($"Financial Account has no currency.");

            currencyId = settlementFinancialAccountData.AccountCurrencyId.Value;

            isApplicableToCustomer = settlementFinancialAccountData.InvoiceTypeIds.Contains(_customerInvoiceTypeId);
            isApplicableToSupplier = settlementFinancialAccountData.InvoiceTypeIds.Contains(_supplierInvoiceTypeId);

            availableCustomerInvoices = null;
            availableSupplierInvoices = null;

            var settlementGenerationCustomSectionPayload = customSectionPayload as SettlementGenerationCustomSectionPayload;
            if (settlementGenerationCustomSectionPayload == null)
                return;

            if (settlementGenerationCustomSectionPayload != null)
            {
                availableCustomerInvoices = settlementGenerationCustomSectionPayload.AvailableCustomerInvoices;
                availableSupplierInvoices = settlementGenerationCustomSectionPayload.AvailableSupplierInvoices;
            }


        }
        private bool ValidateInvoicesDates(IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, DateTime fromDate, DateTime toDate, out string errorMessage, out GenerateInvoiceResult generateInvoiceResult)
        {
            errorMessage = null;
            generateInvoiceResult = GenerateInvoiceResult.Succeeded;
            if (invoices.Min(x => x.FromDate) < fromDate || invoices.Max(x => x.ToDate) > toDate)
            {
                errorMessage = "Unable to generate settlement at this period.";
                generateInvoiceResult = GenerateInvoiceResult.Failed;
                return false;

            }
            return true;
        }
        private IEnumerable<Vanrise.Invoice.Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().GetInvoices(invoiceIds);
        }

        private GeneratedInvoice BuildGeneratedInvoice(List<SettlementInvoiceItemDetails> customerInvoiceItemSet, List<SettlementInvoiceItemDetails> supplierInvoiceItemSet,
            Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryByCurrency, List<SettlementInvoiceDetailSummary> companySummary, List<SettlementInvoiceDetailSummary> systemSummary,
            List<SettlementInvoiceDetailSummary> companyRecurringChargesSummary, List<SettlementInvoiceDetailSummary> systemRecurringChargesSummary,
            Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice, bool isApplicableToCustomer, bool isApplicableToSupplier, Dictionary<String, SettlementInvoiceItemDetailByCurrency> settlementInvoiceByCurrency, int currencyId, DateTime issueDate)
        {

            CurrencyExchangeRateManager exchangeRateManager = new CurrencyExchangeRateManager();
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            var settlementInvoiceDetails = new SettlementInvoiceDetails
            {
                IsApplicableToCustomer = isApplicableToCustomer,
                IsApplicableToSupplier = isApplicableToSupplier
            };


            if (customerInvoiceItemSet != null && customerInvoiceItemSet.Count > 0)
            {
                var customerItemSet = new GeneratedInvoiceItemSet
                {
                    SetName = "CustomerInvoices",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var customerInvoice in customerInvoiceItemSet)
                {
                    settlementInvoiceDetails.CustomerDuration += customerInvoice.DurationInSeconds;
                    settlementInvoiceDetails.CustomerTotalNumberOfCalls += customerInvoice.TotalNumberOfCalls;
                    settlementInvoiceDetails.CustomerTotalNumberOfSMSs += customerInvoice.TotalNumberOfSMSs;

                    customerItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = customerInvoice,
                        Name = " "
                    });

                }
                if (customerItemSet.Items.Count > 0)
                    generatedInvoiceItemSets.Add(customerItemSet);
            }



            if (supplierInvoiceItemSet != null && supplierInvoiceItemSet.Count > 0)
            {
                var supplierItemSet = new GeneratedInvoiceItemSet
                {
                    SetName = "SupplierInvoices",
                    Items = new List<GeneratedInvoiceItem>()
                };
                foreach (var supplierInvoice in supplierInvoiceItemSet)
                {
                    settlementInvoiceDetails.SupplierDuration += supplierInvoice.DurationInSeconds;
                    settlementInvoiceDetails.SupplierTotalNumberOfCalls += supplierInvoice.TotalNumberOfCalls;
                    settlementInvoiceDetails.SupplierTotalNumberOfSMSs += supplierInvoice.TotalNumberOfSMSs;

                    supplierItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierInvoice,
                        Name = " "
                    });
                }

                settlementInvoiceDetails.IsOriginalAmountSetted = supplierInvoiceItemSet.All(x => x.IsOriginalAmountSetted);

                if (supplierItemSet.Items.Count > 0)
                    generatedInvoiceItemSets.Add(supplierItemSet);
            }

            if (settlementInvoiceCurrencyByInvoice != null && settlementInvoiceCurrencyByInvoice.Count > 0)
            {
                foreach (var settlementInvoiceCurrency in settlementInvoiceCurrencyByInvoice)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                    generatedInvoiceItemSet.SetName = string.Format("InvoiceByCurrency_{0}", settlementInvoiceCurrency.Key);
                    generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                    foreach (var value in settlementInvoiceCurrency.Value)
                    {
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = value,
                            Name = " "
                        });
                    }
                    if (generatedInvoiceItemSet.Items.Count > 0)
                    {
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                    }
                }
            }

            if (settlementInvoiceByCurrency != null && settlementInvoiceByCurrency.Count > 0)
            {
                var sortedSettlementInvoiceByCurrency = settlementInvoiceByCurrency.OrderBy(item => item.Value.FromDate).ThenBy(item => item.Value.ToDate).ThenBy(item => item.Value.CurrencyId);

                foreach (var settlementInvoice in sortedSettlementInvoiceByCurrency)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                    generatedInvoiceItemSet.SetName = string.Format("SettlementInvoiceByCurrency");
                    generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                    settlementInvoiceDetails.DueToCompanyAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCompanyAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToCompanyAmountWithTaxes += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCompanyAmountWithTaxes, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToCompanyAmountRecurringCharges += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCompanyAmountRecurringCharges, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToCompanyTotalTrafficAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCompanyTotalTrafficAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToCompanyTotalSMSAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCompanyTotalSMSAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToCompanyFullAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCompanyFullAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate); // sattlementInvoiceDetails.ful.DueToCompanyTotalTrafficAmount + sattlementInvoiceDetails.DueToCompanyAmountRecurringCharges + sattlementInvoiceDetails.DueToCompanyTotalDealAmount + sattlementInvoiceDetails.DueToCompanyTotalSMSAmount;

                    settlementInvoiceDetails.DueToSystemAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToSystemAmountWithTaxes += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmountWithTaxes, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToSystemAmountRecurringCharges += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmountRecurringCharges, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToSystemTotalTrafficAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemTotalTrafficAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToSystemTotalSMSAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemTotalSMSAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    settlementInvoiceDetails.DueToSystemFullAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemFullAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate); //sattlementInvoiceDetails.DueToSystemTotalTrafficAmount + sattlementInvoiceDetails.DueToSystemAmountRecurringCharges + sattlementInvoiceDetails.DueToSystemTotalDealAmount + sattlementInvoiceDetails.DueToSystemTotalSMSAmount;



                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = settlementInvoice.Value,
                        Name = " "
                    });

                    if (generatedInvoiceItemSet.Items.Count > 0)
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

                }

            }

            if (settlementInvoiceDetails.DueToCompanyFullAmount > settlementInvoiceDetails.DueToSystemFullAmount)
            {
                settlementInvoiceDetails.DueToCompanyDifference = settlementInvoiceDetails.DueToCompanyFullAmount - settlementInvoiceDetails.DueToSystemFullAmount;
            }
            else
            {
                settlementInvoiceDetails.DueToSystemDifference = settlementInvoiceDetails.DueToSystemFullAmount - settlementInvoiceDetails.DueToCompanyFullAmount;
            }
            settlementInvoiceDetails.NoCompanySMS = settlementInvoiceDetails.DueToCompanyTotalSMSAmount == 0;
            settlementInvoiceDetails.NoCompanyVoice = settlementInvoiceDetails.DueToCompanyAmountWithTaxes == 0;
            settlementInvoiceDetails.NoCompanyRecurringCharges = settlementInvoiceDetails.DueToCompanyAmountRecurringCharges == 0;

            settlementInvoiceDetails.NoSystemSMS = settlementInvoiceDetails.DueToSystemTotalSMSAmount == 0;
            settlementInvoiceDetails.NoSystemVoice = settlementInvoiceDetails.DueToSystemAmountWithTaxes == 0;
            settlementInvoiceDetails.NoSystemRecurringCharges = settlementInvoiceDetails.DueToSystemAmountRecurringCharges == 0;


            if (settlementInvoiceItemSummaryByCurrency != null && settlementInvoiceItemSummaryByCurrency.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "SettlementInvoiceSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var settlementInvoiceItemSummaryDetail in settlementInvoiceItemSummaryByCurrency)
                {

                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = settlementInvoiceItemSummaryDetail.Value,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }


            if (systemSummary != null && systemSummary.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "SystemSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var summary in systemSummary)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summary,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }

            if (companySummary != null && companySummary.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "CompanySummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var summary in companySummary)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summary,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }
            if (companyRecurringChargesSummary != null && companyRecurringChargesSummary.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "CompanyRecurringChargesSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var summary in companyRecurringChargesSummary)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summary,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }
            if (systemRecurringChargesSummary != null && systemRecurringChargesSummary.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "SystemRecurringChargesSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var summary in systemRecurringChargesSummary)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summary,
                        Name = " "
                    });
                }
                if (generatedInvoiceItemSet.Items.Count > 0)
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
            }


            return new GeneratedInvoice
            {
                InvoiceDetails = settlementInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets
            };
        }

        private Dictionary<long, InterconnectInvoiceByCurrencyItemDetailsByCurrency> GenerateInterconnectInvoiceByCurrencyItemDetailsByCurrencyDic(IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, IEnumerable<InvoiceItem> invoiceItems)
        {
            var interconnectInvoiceByCurrencyItemDetailsByInvoiceIdDic = new Dictionary<long, InterconnectInvoiceByCurrencyItemDetailsByCurrency>();

            if (invoices == null)
                return null;

            foreach (var invoice in invoices)
            {
                var invoiceDetails = invoice.Details as InterconnectInvoiceDetails;

                if (invoiceDetails == null)
                    continue;

                invoiceItems.ThrowIfNull("Invoice Items", invoice.InvoiceId);

                var items = invoiceItems.FindAllRecords(x => x.InvoiceId == invoice.InvoiceId);

                if (items == null)
                    continue;

                var interconnectInvoiceByCurrencyItemDetailsByCurrency = interconnectInvoiceByCurrencyItemDetailsByInvoiceIdDic.GetOrCreateItem(invoice.InvoiceId);

                if (interconnectInvoiceByCurrencyItemDetailsByCurrency.Currencies == null)
                {
                    interconnectInvoiceByCurrencyItemDetailsByCurrency.Currencies = new HashSet<int>();
                    interconnectInvoiceByCurrencyItemDetailsByCurrency.MonthsByCurrency = new Dictionary<int, List<InterconnectInvoiceByCurrencyItemDetails>>();
                }

                foreach (var item in items)
                {
                    var invoiceItemDetails = item.Details as InterconnectInvoiceByCurrencyItemDetails;

                    if (invoiceItemDetails == null)
                        continue;

                    interconnectInvoiceByCurrencyItemDetailsByCurrency.Currencies.Add(invoiceItemDetails.CurrencyId);

                    var interconnectInvoiceByCurrencyItemDetails = interconnectInvoiceByCurrencyItemDetailsByCurrency.MonthsByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);

                    interconnectInvoiceByCurrencyItemDetails.Add(invoiceItemDetails);
                }
            }
            return interconnectInvoiceByCurrencyItemDetailsByInvoiceIdDic;
        }
    }
}
