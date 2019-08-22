using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Invoice.Business;
using Vanrise.Invoice.Entities;

namespace TOne.WhS.Invoice.Business.Extensions
{
    public class SettlementInvoiceGenerator : InvoiceGenerator
    {
        Guid _customerInvoiceTypeId;
        Guid _supplierInvoiceTypeId;

        public SettlementInvoiceGenerator(Guid customerInvoiceTypeId, Guid supplierInvoiceTypeId)
        {
            _customerInvoiceTypeId = customerInvoiceTypeId;
            _supplierInvoiceTypeId = supplierInvoiceTypeId;
        }

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            string partnerType;
            bool isApplicableToCustomer;
            bool isApplicableToSupplier;
            List<InvoiceAvailableForSettlement> availableCustomerInvoices;
            List<InvoiceAvailableForSettlement> availableSupplierInvoices;
            int currencyId;
            PrepareDataForProcessing(context.PartnerId, context.InvoiceTypeId, context.CustomSectionPayload, out partnerType, out isApplicableToSupplier, out isApplicableToCustomer,
                out availableCustomerInvoices, out availableSupplierInvoices, out currencyId);

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

            List<SattlementInvoiceItemDetails> customerInvoiceItemSet;
            List<SattlementInvoiceItemDetails> supplierInvoiceItemSet;
            Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryByCurrency;
            List<SettlementInvoiceDetailSummary> carrierSummary;
            List<SettlementInvoiceDetailSummary> systemSummary;

            List<SettlementInvoiceDetailSummary> carrierRecurringChargesSummary;
            List<SettlementInvoiceDetailSummary> systemRecurringChargesSummary;

            Dictionary<String, SettlementInvoiceItemDetail> settlementInvoiceByCurrency;

            Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice;

            ConfigManager configManager = new ConfigManager();
            InvoiceTypeSetting settings = configManager.GetInvoiceTypeSettingsById(context.InvoiceTypeId);
            var invoiceSettings = configManager.GetInvoiceSettings();
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

            ProcessItemSetName(customerInvoices, customerInvoiceItems, supplierInvoices, supplierInvoiceItems, availableCustomerInvoices, availableSupplierInvoices, out carrierRecurringChargesSummary, out systemRecurringChargesSummary, out customerInvoiceItemSet, out supplierInvoiceItemSet, out settlementInvoiceItemSummaryByCurrency, out carrierSummary, out systemSummary, out settlementInvoiceCurrencyByInvoice, out settlementInvoiceByCurrency);

            if (settings != null)
                context.NeedApproval = settings.NeedApproval;

            context.Invoice = BuildGeneratedInvoice(customerInvoiceItemSet, supplierInvoiceItemSet, settlementInvoiceItemSummaryByCurrency, carrierSummary, systemSummary, carrierRecurringChargesSummary, systemRecurringChargesSummary, settlementInvoiceCurrencyByInvoice, partnerType, isApplicableToCustomer, isApplicableToSupplier, settlementInvoiceByCurrency, currencyId, context.IssueDate);
        }

        private void ProcessItemSetName(IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices, IEnumerable<InvoiceItem> customerInvoiceItems, IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices,
            IEnumerable<InvoiceItem> supplierInvoiceItems, List<InvoiceAvailableForSettlement> availableCustomerInvoices, List<InvoiceAvailableForSettlement> availableSupplierInvoices, out List<SettlementInvoiceDetailSummary> carrierRecurringChargesSummary,
            out List<SettlementInvoiceDetailSummary> systemRecurringChargesSummary, out List<SattlementInvoiceItemDetails> customerInvoiceItemSet, out List<SattlementInvoiceItemDetails> supplierInvoiceItemSet, out Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryByCurrency, out List<SettlementInvoiceDetailSummary> carrierSummary, out List<SettlementInvoiceDetailSummary> systemSummary, out Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice, out Dictionary<String, SettlementInvoiceItemDetail> settlementInvoiceByCurrency)
        {
            customerInvoiceItemSet = null;
            supplierInvoiceItemSet = null;

            settlementInvoiceItemSummaryByCurrency = new Dictionary<int, SettlementInvoiceItemSummaryDetail>();
            settlementInvoiceCurrencyByInvoice = new Dictionary<long, List<SettlementInvoiceDetailByCurrency>>();
            settlementInvoiceByCurrency = new Dictionary<String, SettlementInvoiceItemDetail>();

            if (customerInvoices != null && customerInvoiceItems != null)
            {
                customerInvoiceItemSet = new List<SattlementInvoiceItemDetails>();

                foreach (var customerInvoice in customerInvoices)
                {
                    List<int> currenciesUsed = new List<int>();
                    //bool isNewInvoice = true;

                    var customerInvoiceDetails = customerInvoice.Details as CustomerInvoiceDetails;
                    if (customerInvoiceDetails != null)
                    {
                        bool multipleCurrencies = false;
                        var invoiceItems = customerInvoiceItems.FindAllRecords(x => x.InvoiceId == customerInvoice.InvoiceId);
                        if (invoiceItems != null)
                        {
                            var settlementInvoiceCurrency = settlementInvoiceCurrencyByInvoice.GetOrCreateItem(customerInvoice.InvoiceId);
                            foreach (var invoiceItem in invoiceItems)
                            {
                                var invoiceItemDetails = invoiceItem.Details as CustomerInvoiceBySaleCurrencyItemDetails;
                                if (invoiceItemDetails != null && invoiceItemDetails.CurrencyId != customerInvoiceDetails.SaleCurrencyId)
                                    multipleCurrencies = true;

                                if (invoiceItemDetails != null && availableCustomerInvoices.Any(x => x.InvoiceId == customerInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId && x.IsSelected))
                                {
                                    OriginalDataCurrrency originalDataCurrrency = new OriginalDataCurrrency();

                                    var month = invoiceItemDetails.Month;

                                    bool hasOriginalAmount = false;
                                    var fromDate = invoiceItemDetails.FromDate;
                                    var toDate = invoiceItemDetails.ToDate;

                                    if (customerInvoiceDetails.OriginalAmountByCurrency != null && customerInvoiceDetails.OriginalAmountByCurrency.TryGetValue(invoiceItemDetails.CurrencyId, out originalDataCurrrency) && customerInvoiceDetails.IncludeOriginalAmountInSettlement && (originalDataCurrrency.TrafficAmount.HasValue || originalDataCurrrency.SMSAmount.HasValue || originalDataCurrrency.DealAmount.HasValue || originalDataCurrrency.RecurringChargeAmount.HasValue))
                                    {
                                        hasOriginalAmount = true;
                                        fromDate = customerInvoice.FromDate;
                                        toDate = customerInvoice.ToDate;
                                        if (fromDate.Month != toDate.Month || fromDate.Year != toDate.Year)
                                            month = string.Format("{0} / {1}", fromDate.ToString("MMMM - yyyy"), toDate.ToString("MMMM - yyyy"));
                                        else
                                            month = string.Format("{0}", fromDate.ToString("MMMM - yyyy"));
                                    }

                                    var settlementInvoicedetail = settlementInvoiceByCurrency.GetOrCreateItem(string.Format("{0}_{1}", invoiceItemDetails.CurrencyId, month), () =>
                                    {
                                        return new SettlementInvoiceItemDetail()
                                        {
                                            CurrencyId = invoiceItemDetails.CurrencyId,
                                            InvoiceId = customerInvoice.InvoiceId,
                                            Month = month,
                                            FromDate = fromDate,
                                            ToDate = toDate,
                                        };
                                    });

                                    SettlementInvoiceDetailByCurrency settlementInvoiceDetailByCurrency = null;
                                    if (!hasOriginalAmount)
                                    {
                                        settlementInvoiceDetailByCurrency = new SettlementInvoiceDetailByCurrency();
                                    }
                                    else
                                    {
                                        settlementInvoiceDetailByCurrency = settlementInvoiceCurrency.FindRecord(x => x.InvoiceId == customerInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId);
                                        if (settlementInvoiceDetailByCurrency == null)
                                            settlementInvoiceDetailByCurrency = new SettlementInvoiceDetailByCurrency();
                                    }

                                    settlementInvoiceDetailByCurrency.InvoiceId = customerInvoice.InvoiceId;
                                    settlementInvoiceDetailByCurrency.CurrencyId = invoiceItemDetails.CurrencyId;
                                    settlementInvoiceDetailByCurrency.TotalDuration += invoiceItemDetails.Duration;
                                    settlementInvoiceDetailByCurrency.NumberOfCalls += invoiceItemDetails.NumberOfCalls;

                                    bool canAddSettlementInvoiceDetailByCurrency = false;

                                    if (hasOriginalAmount)
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;

                                        var canUseCurrency = false;
                                        if (!currenciesUsed.Contains(invoiceItemDetails.CurrencyId))
                                        {
                                            currenciesUsed.Add(invoiceItemDetails.CurrencyId);
                                            canUseCurrency = true;
                                            canAddSettlementInvoiceDetailByCurrency = true;
                                        }

                                        decimal dueToSystemFullAmountSettlementInvoiceItemSummaryDetail = 0;
                                        decimal dueToSystemFullAmountSettlementInvoicedetail = 0;
                                        if (originalDataCurrrency.TrafficAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceItemSummaryDetail.DueToSystemAmount += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommission += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommissionWithTaxes += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceItemSummaryDetail.DueToSystemTotalTrafficAmount += originalDataCurrrency.TrafficAmount.Value;

                                                settlementInvoiceDetailByCurrency.OriginalAmount = originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceDetailByCurrency.TotalTrafficAmount = originalDataCurrrency.TrafficAmount.Value;

                                                settlementInvoicedetail.DueToSystemAmount += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoicedetail.DueToSystemAmountAfterCommission += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoicedetail.DueToSystemAmountAfterCommissionWithTaxes += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoicedetail.DueToSystemTotalTrafficAmount += originalDataCurrrency.TrafficAmount.Value;

                                                dueToSystemFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.TrafficAmount.Value;
                                            }

                                            dueToSystemFullAmountSettlementInvoicedetail += originalDataCurrrency.TrafficAmount.Value;
                                            //if (isNewInvoice)
                                            //{

                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToSystemAmount = originalDataCurrrency.TrafficAmount.Value;
                                            //    settlementInvoicedetail.DueToSystemAmountAfterCommission = originalDataCurrrency.TrafficAmount.Value;
                                            //    settlementInvoicedetail.DueToSystemAmountAfterCommissionWithTaxes = originalDataCurrrency.TrafficAmount.Value;
                                            //    settlementInvoicedetail.DueToSystemTotalTrafficAmount = originalDataCurrrency.TrafficAmount.Value;
                                            //}
                                        }

                                        if (canUseCurrency)
                                        {
                                            settlementInvoiceItemSummaryDetail.DueToSystemNumberOfCalls += customerInvoiceDetails.TotalNumberOfCalls;
                                            settlementInvoicedetail.DueToSystemNumberOfCalls += customerInvoiceDetails.TotalNumberOfCalls;
                                        }

                                        //if (isNewInvoice)
                                        //{

                                        //}
                                        //else
                                        //{
                                        //    settlementInvoicedetail.DueToSystemNumberOfCalls = customerInvoiceDetails.TotalNumberOfCalls;
                                        //}

                                        if (originalDataCurrrency.SMSAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceDetailByCurrency.OriginalAmount += originalDataCurrrency.SMSAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission += originalDataCurrrency.SMSAmount.Value;

                                                settlementInvoiceItemSummaryDetail.DueToSystemTotalSMSAmount += originalDataCurrrency.SMSAmount.Value;
                                                dueToSystemFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.SMSAmount.Value;
                                                settlementInvoicedetail.DueToSystemTotalSMSAmount += originalDataCurrrency.SMSAmount.Value;

                                            }
                                            dueToSystemFullAmountSettlementInvoicedetail += originalDataCurrrency.SMSAmount.Value;
                                            //if (isNewInvoice)
                                            //{
                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToSystemTotalSMSAmount = originalDataCurrrency.SMSAmount.Value;
                                            //}
                                        }

                                        if (originalDataCurrrency.DealAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceDetailByCurrency.OriginalAmount += originalDataCurrrency.DealAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission += originalDataCurrrency.DealAmount.Value;

                                                settlementInvoiceItemSummaryDetail.DueToSystemTotalDealAmount += originalDataCurrrency.DealAmount.Value;
                                                dueToSystemFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.DealAmount.Value;
                                                settlementInvoicedetail.DueToSystemTotalDealAmount += originalDataCurrrency.DealAmount.Value;
                                            }

                                            dueToSystemFullAmountSettlementInvoicedetail += originalDataCurrrency.DealAmount.Value;
                                            //if (isNewInvoice)
                                            //{
                                            //    settlementInvoicedetail.DueToSystemTotalDealAmount += originalDataCurrrency.DealAmount.Value;
                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToSystemTotalDealAmount = originalDataCurrrency.DealAmount.Value;
                                            //}
                                        }

                                        if (originalDataCurrrency.RecurringChargeAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceDetailByCurrency.OriginalAmount += originalDataCurrrency.RecurringChargeAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission += originalDataCurrrency.RecurringChargeAmount.Value;

                                                settlementInvoiceItemSummaryDetail.DueToSystemAmountRecurringCharges += originalDataCurrrency.RecurringChargeAmount.Value;
                                                settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = originalDataCurrrency.RecurringChargeAmount.Value;
                                                dueToSystemFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.RecurringChargeAmount.Value;

                                                settlementInvoicedetail.DueToSystemAmountRecurringCharges += originalDataCurrrency.RecurringChargeAmount.Value;
                                            }

                                            dueToSystemFullAmountSettlementInvoicedetail += originalDataCurrrency.RecurringChargeAmount.Value;
                                            //if (isNewInvoice)
                                            //{
                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToSystemAmountRecurringCharges = originalDataCurrrency.RecurringChargeAmount.Value;
                                            //}
                                        }

                                        if (canUseCurrency)
                                        {
                                            settlementInvoiceItemSummaryDetail.DueToSystemFullAmount += dueToSystemFullAmountSettlementInvoiceItemSummaryDetail;
                                            settlementInvoicedetail.DueToSystemFullAmount += dueToSystemFullAmountSettlementInvoicedetail;
                                        }

                                        //if (isNewInvoice)
                                        //{

                                        //}
                                        //else
                                        //{
                                        //    settlementInvoicedetail.DueToSystemFullAmount = dueToSystemFullAmountSettlementInvoicedetail;
                                        //}
                                    }
                                    else
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToSystemNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemTotalDealAmount += invoiceItemDetails.TotalDealAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAdjustmentAmount += invoiceItemDetails.AdjustmentAmount;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = invoiceItemDetails.Amount;
                                        settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceDetailByCurrency.TotalAdjustmentAmount = invoiceItemDetails.AdjustmentAmount;
                                        settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceDetailByCurrency.TotalTrafficAmount = invoiceItemDetails.TotalTrafficAmount;

                                        settlementInvoicedetail.DueToSystemAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToSystemAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToSystemAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToSystemFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoicedetail.DueToSystemNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoicedetail.DueToSystemAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoicedetail.DueToSystemTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoicedetail.DueToSystemTotalDealAmount += invoiceItemDetails.TotalDealAmount;
                                        settlementInvoicedetail.DueToSystemTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;
                                        settlementInvoicedetail.DueToSystemAdjustmentAmount += invoiceItemDetails.AdjustmentAmount;
                                    }

                                    if (!hasOriginalAmount || (hasOriginalAmount && canAddSettlementInvoiceDetailByCurrency))
                                        settlementInvoiceCurrency.Add(settlementInvoiceDetailByCurrency);
                                }
                                // isNewInvoice = false;
                            }
                        }

                        decimal totalOriginalAmountByCurrency = 0;
                        bool originalAmountByCurrencyExists = false;
                        if (customerInvoiceDetails.OriginalAmountByCurrency != null && customerInvoiceDetails.OriginalAmountByCurrency.Count > 0)
                        {
                            originalAmountByCurrencyExists = true;
                            if (!multipleCurrencies)
                            {
                                var originalAmountByCurrencyItem = customerInvoiceDetails.OriginalAmountByCurrency.First();
                                if (originalAmountByCurrencyItem.Value.TrafficAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.TrafficAmount.Value;
                                if (originalAmountByCurrencyItem.Value.SMSAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.SMSAmount.Value;
                                if (originalAmountByCurrencyItem.Value.DealAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.DealAmount.Value;
                                if (originalAmountByCurrencyItem.Value.RecurringChargeAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.RecurringChargeAmount.Value;
                            }
                        }

                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = multipleCurrencies ? default(decimal?) : customerInvoiceDetails.TotalInvoiceAmount,
                            DurationInSeconds = customerInvoiceDetails.Duration,
                            CurrencyId = multipleCurrencies ? default(int?) : customerInvoiceDetails.SaleCurrencyId,
                            InvoiceId = customerInvoice.InvoiceId,
                            InvoiceTypeId = customerInvoice.InvoiceTypeId,
                            TotalNumberOfCalls = customerInvoiceDetails.TotalNumberOfCalls,
                            AmountWithCommission = multipleCurrencies ? default(decimal?) : customerInvoiceDetails.TotalAmountAfterCommission,
                            Commission = customerInvoiceDetails.Commission,
                            IssueDate = customerInvoice.IssueDate,
                            SerialNumber = customerInvoice.SerialNumber,
                            MultipleCurrencies = multipleCurrencies,
                            DueDate = customerInvoice.DueDate,
                            FromDate = customerInvoice.FromDate,
                            OriginalAmount = !multipleCurrencies && originalAmountByCurrencyExists ? totalOriginalAmountByCurrency : default(decimal?),
                            TimeZoneId = customerInvoiceDetails.TimeZoneId,
                            ToDate = customerInvoice.ToDate,
                        };

                        customerInvoiceItemSet.Add(sattlementInvoiceItemDetails);
                    }
                }
            }

            if (supplierInvoices != null && supplierInvoiceItems != null)
            {
                supplierInvoiceItemSet = new List<SattlementInvoiceItemDetails>();

                foreach (var supplierInvoice in supplierInvoices)
                {
                    List<int> currenciesUsed = new List<int>();
                    //bool isNewInvoice = true;
                    var supplierInvoiceDetails = supplierInvoice.Details as SupplierInvoiceDetails;
                    if (supplierInvoiceDetails != null)
                    {
                        bool isOriginalAmountSetted = supplierInvoiceDetails.IsOriginalAmountSetted;
                        bool multipleCurrencies = false;

                        var invoiceItems = supplierInvoiceItems.FindAllRecords(x => x.InvoiceId == supplierInvoice.InvoiceId);
                        if (invoiceItems != null)
                        {
                            var settlementInvoiceCurrency = settlementInvoiceCurrencyByInvoice.GetOrCreateItem(supplierInvoice.InvoiceId);
                            foreach (var invoiceItem in invoiceItems)
                            {
                                var invoiceItemDetails = invoiceItem.Details as SupplierInvoiceBySaleCurrencyItemDetails;

                                if (invoiceItemDetails != null && invoiceItemDetails.CurrencyId != supplierInvoiceDetails.SupplierCurrencyId)
                                    multipleCurrencies = true;

                                if (invoiceItemDetails != null && availableSupplierInvoices.Any(x => x.InvoiceId == supplierInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId && x.IsSelected))
                                {
                                    OriginalDataCurrrency originalDataCurrrency = new OriginalDataCurrrency();

                                    var month = invoiceItemDetails.Month;

                                    bool hasOriginalAmount = false;
                                    var fromDate = invoiceItemDetails.FromDate;
                                    var toDate = invoiceItemDetails.ToDate;

                                    if (supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.TryGetValue(invoiceItemDetails.CurrencyId, out originalDataCurrrency) && supplierInvoiceDetails.IncludeOriginalAmountInSettlement && (originalDataCurrrency.TrafficAmount.HasValue || originalDataCurrrency.SMSAmount.HasValue || originalDataCurrrency.DealAmount.HasValue || originalDataCurrrency.RecurringChargeAmount.HasValue))
                                    {
                                        fromDate = supplierInvoice.FromDate;
                                        toDate = supplierInvoice.ToDate;
                                        hasOriginalAmount = true;
                                        if (fromDate.Month != toDate.Month || fromDate.Year != toDate.Year)
                                            month = string.Format("{0} / {1}", fromDate.ToString("MMMM - yyyy"), toDate.ToString("MMMM - yyyy"));
                                        else
                                            month = string.Format("{0}", fromDate.ToString("MMMM - yyyy"));
                                    }
                                    var settlementInvoicedetail = settlementInvoiceByCurrency.GetOrCreateItem(string.Format("{0}_{1}", invoiceItemDetails.CurrencyId, month), () =>
                                    {
                                        return new SettlementInvoiceItemDetail()
                                        {
                                            CurrencyId = invoiceItemDetails.CurrencyId,
                                            InvoiceId = supplierInvoice.InvoiceId,
                                            Month = month,
                                            FromDate = fromDate,
                                            ToDate = toDate,
                                        };
                                    });

                                    SettlementInvoiceDetailByCurrency settlementInvoiceDetailByCurrency = null;
                                    if (!hasOriginalAmount)
                                    {
                                        settlementInvoiceDetailByCurrency = new SettlementInvoiceDetailByCurrency();
                                    }
                                    else
                                    {
                                        settlementInvoiceDetailByCurrency = settlementInvoiceCurrency.FindRecord(x => x.InvoiceId == supplierInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId);
                                        if (settlementInvoiceDetailByCurrency == null)
                                            settlementInvoiceDetailByCurrency = new SettlementInvoiceDetailByCurrency();
                                    }

                                    settlementInvoiceDetailByCurrency.InvoiceId = supplierInvoice.InvoiceId;
                                    settlementInvoiceDetailByCurrency.CurrencyId = invoiceItemDetails.CurrencyId;
                                    settlementInvoiceDetailByCurrency.TotalDuration += invoiceItemDetails.Duration;
                                    settlementInvoiceDetailByCurrency.NumberOfCalls += invoiceItemDetails.NumberOfCalls;

                                    bool canAddSettlementInvoiceDetailByCurrency = false;

                                    if (hasOriginalAmount)
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;

                                        var canUseCurrency = false;
                                        if (!currenciesUsed.Contains(invoiceItemDetails.CurrencyId))
                                        {
                                            currenciesUsed.Add(invoiceItemDetails.CurrencyId);
                                            canUseCurrency = true;
                                            canAddSettlementInvoiceDetailByCurrency = true;
                                        }

                                        decimal dueToCarrierFullAmountSettlementInvoiceItemSummaryDetail = 0;
                                        decimal dueToCarrierFullAmountSettlementInvoicedetail = 0;
                                        if (originalDataCurrrency.TrafficAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceItemSummaryDetail.DueToCarrierAmount += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommission += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommissionWithTaxes += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceItemSummaryDetail.DueToCarrierTotalTrafficAmount += originalDataCurrrency.TrafficAmount.Value;

                                                dueToCarrierFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.TrafficAmount.Value;

                                                settlementInvoiceDetailByCurrency.OriginalAmount = originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoiceDetailByCurrency.TotalTrafficAmount = originalDataCurrrency.TrafficAmount.Value;

                                                settlementInvoicedetail.DueToCarrierAmount += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoicedetail.DueToCarrierAmountAfterCommission += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoicedetail.DueToCarrierAmountAfterCommissionWithTaxes += originalDataCurrrency.TrafficAmount.Value;
                                                settlementInvoicedetail.DueToCarrierTotalTrafficAmount += originalDataCurrrency.TrafficAmount.Value;
                                            }
                                            dueToCarrierFullAmountSettlementInvoicedetail += originalDataCurrrency.TrafficAmount.Value;
                                            //if (isNewInvoice)
                                            //{

                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToCarrierAmount = originalDataCurrrency.TrafficAmount.Value;
                                            //    settlementInvoicedetail.DueToCarrierAmountAfterCommission = originalDataCurrrency.TrafficAmount.Value;
                                            //    settlementInvoicedetail.DueToCarrierAmountAfterCommissionWithTaxes = originalDataCurrrency.TrafficAmount.Value;
                                            //    settlementInvoicedetail.DueToCarrierTotalTrafficAmount = originalDataCurrrency.TrafficAmount.Value;
                                            //}
                                        }

                                        if (canUseCurrency)
                                        {
                                            settlementInvoiceItemSummaryDetail.DueToCarrierNumberOfCalls += supplierInvoiceDetails.TotalNumberOfCalls;
                                            settlementInvoicedetail.DueToCarrierNumberOfCalls += supplierInvoiceDetails.TotalNumberOfCalls;
                                        }

                                        //if (isNewInvoice)
                                        //{
                                        //}
                                        //else
                                        //{
                                        //    settlementInvoicedetail.DueToCarrierNumberOfCalls = supplierInvoiceDetails.TotalNumberOfCalls;
                                        //}

                                        if (originalDataCurrrency.SMSAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceDetailByCurrency.OriginalAmount += originalDataCurrrency.SMSAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission += originalDataCurrrency.SMSAmount.Value;

                                                dueToCarrierFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.SMSAmount.Value;

                                                settlementInvoiceItemSummaryDetail.DueToCarrierTotalSMSAmount += originalDataCurrrency.SMSAmount.Value;

                                                settlementInvoicedetail.DueToCarrierTotalSMSAmount += originalDataCurrrency.SMSAmount.Value;
                                            }

                                            dueToCarrierFullAmountSettlementInvoicedetail += originalDataCurrrency.SMSAmount.Value;
                                            //if (isNewInvoice)
                                            //{
                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToCarrierTotalSMSAmount = originalDataCurrrency.SMSAmount.Value;
                                            //}
                                        }

                                        if (originalDataCurrrency.DealAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceDetailByCurrency.OriginalAmount += originalDataCurrrency.DealAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission += originalDataCurrrency.DealAmount.Value;

                                                dueToCarrierFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.DealAmount.Value;

                                                settlementInvoiceItemSummaryDetail.DueToCarrierTotalDealAmount += originalDataCurrrency.DealAmount.Value;
                                                settlementInvoicedetail.DueToCarrierTotalDealAmount += originalDataCurrrency.DealAmount.Value;

                                            }
                                            dueToCarrierFullAmountSettlementInvoicedetail += originalDataCurrrency.DealAmount.Value;
                                            //if (isNewInvoice)
                                            //{
                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToCarrierTotalDealAmount = originalDataCurrrency.DealAmount.Value;
                                            //}
                                        }

                                        if (originalDataCurrrency.RecurringChargeAmount.HasValue)
                                        {
                                            if (canUseCurrency)
                                            {
                                                settlementInvoiceDetailByCurrency.OriginalAmount += originalDataCurrrency.RecurringChargeAmount.Value;
                                                settlementInvoiceDetailByCurrency.OriginalAmountWithCommission += originalDataCurrrency.RecurringChargeAmount.Value;

                                                dueToCarrierFullAmountSettlementInvoiceItemSummaryDetail += originalDataCurrrency.RecurringChargeAmount.Value;

                                                settlementInvoiceItemSummaryDetail.DueToCarrierAmountRecurringCharges += originalDataCurrrency.RecurringChargeAmount.Value;

                                                settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = originalDataCurrrency.RecurringChargeAmount.Value;
                                                settlementInvoicedetail.DueToCarrierAmountRecurringCharges += originalDataCurrrency.RecurringChargeAmount.Value;

                                            }
                                            dueToCarrierFullAmountSettlementInvoicedetail += originalDataCurrrency.RecurringChargeAmount.Value;
                                            //if (isNewInvoice)
                                            //{
                                            //}
                                            //else
                                            //{
                                            //    settlementInvoicedetail.DueToCarrierAmountRecurringCharges = originalDataCurrrency.RecurringChargeAmount.Value;
                                            //}
                                        }

                                        if (canUseCurrency)
                                        {
                                            settlementInvoiceItemSummaryDetail.DueToCarrierFullAmount += dueToCarrierFullAmountSettlementInvoiceItemSummaryDetail;
                                            settlementInvoicedetail.DueToCarrierFullAmount += dueToCarrierFullAmountSettlementInvoicedetail;
                                        }

                                        //if (isNewInvoice)
                                        //{

                                        //}
                                        //else
                                        //{
                                        //    settlementInvoicedetail.DueToCarrierFullAmount = dueToCarrierFullAmountSettlementInvoicedetail;
                                        //}
                                    }
                                    else
                                    {
                                        isOriginalAmountSetted = false;

                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierTotalDealAmount += invoiceItemDetails.TotalDealAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAdjustmentAmount += invoiceItemDetails.AdjustmentAmount;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = invoiceItemDetails.Amount;
                                        settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceDetailByCurrency.TotalTrafficAmount = invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoiceDetailByCurrency.TotalAdjustmentAmount = invoiceItemDetails.AdjustmentAmount;

                                        settlementInvoicedetail.DueToCarrierAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToCarrierAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToCarrierAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToCarrierNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoicedetail.DueToCarrierAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoicedetail.DueToCarrierTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                        settlementInvoicedetail.DueToCarrierTotalDealAmount += invoiceItemDetails.TotalDealAmount;
                                        settlementInvoicedetail.DueToCarrierFullAmount += invoiceItemDetails.TotalFullAmount;
                                        settlementInvoicedetail.DueToCarrierAdjustmentAmount += invoiceItemDetails.AdjustmentAmount;
                                        settlementInvoicedetail.DueToCarrierTotalSMSAmount += invoiceItemDetails.TotalSMSAmount;
                                    }

                                    if (!hasOriginalAmount || (hasOriginalAmount && canAddSettlementInvoiceDetailByCurrency))
                                        settlementInvoiceCurrency.Add(settlementInvoiceDetailByCurrency);
                                }

                                //isNewInvoice = false;
                            }
                        }

                        decimal totalOriginalAmountByCurrency = 0;
                        bool originalAmountByCurrencyExists = false;
                        if (supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.Count > 0)
                        {
                            originalAmountByCurrencyExists = true;
                            if (!multipleCurrencies)
                            {
                                var originalAmountByCurrencyItem = supplierInvoiceDetails.OriginalAmountByCurrency.First();
                                if (originalAmountByCurrencyItem.Value.TrafficAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.TrafficAmount.Value;
                                if (originalAmountByCurrencyItem.Value.SMSAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.SMSAmount.Value;
                                if (originalAmountByCurrencyItem.Value.DealAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.DealAmount.Value;
                                if (originalAmountByCurrencyItem.Value.RecurringChargeAmount.HasValue)
                                    totalOriginalAmountByCurrency += originalAmountByCurrencyItem.Value.RecurringChargeAmount.Value;
                            }
                        }

                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = multipleCurrencies ? default(decimal?) : supplierInvoiceDetails.TotalInvoiceAmount,
                            DurationInSeconds = supplierInvoiceDetails.Duration,
                            CurrencyId = multipleCurrencies ? default(int?) : supplierInvoiceDetails.SupplierCurrencyId,
                            InvoiceId = supplierInvoice.InvoiceId,
                            InvoiceTypeId = supplierInvoice.InvoiceTypeId,
                            TotalNumberOfCalls = supplierInvoiceDetails.TotalNumberOfCalls,
                            AmountWithCommission = multipleCurrencies ? default(decimal?) : supplierInvoiceDetails.TotalAmountAfterCommission,
                            Commission = supplierInvoiceDetails.Commission,
                            SerialNumber = supplierInvoice.SerialNumber,
                            IssueDate = supplierInvoice.IssueDate,
                            MultipleCurrencies = multipleCurrencies,
                            DueDate = supplierInvoice.DueDate,
                            FromDate = supplierInvoice.FromDate,
                            OriginalAmount = !multipleCurrencies && originalAmountByCurrencyExists ? totalOriginalAmountByCurrency : default(decimal?),
                            TimeZoneId = supplierInvoiceDetails.TimeZoneId,
                            ToDate = supplierInvoice.ToDate,
                            IsOriginalAmountSetted = isOriginalAmountSetted
                        };

                        supplierInvoiceItemSet.Add(sattlementInvoiceItemDetails);
                    }
                }
            }

            systemSummary = null;
            carrierSummary = null;
            carrierRecurringChargesSummary = null;
            systemRecurringChargesSummary = null;

            if (settlementInvoiceByCurrency != null && settlementInvoiceByCurrency.Count > 0)
            {
                foreach (var settlementInvoiceItemByCurrency in settlementInvoiceByCurrency)
                {
                    decimal sum = settlementInvoiceItemByCurrency.Value.DueToCarrierAmountAfterCommissionWithTaxes - settlementInvoiceItemByCurrency.Value.DueToSystemAmountAfterCommissionWithTaxes;
                    if (sum > 0)
                    {
                        settlementInvoiceItemByCurrency.Value.DueToCarrierDifference = sum;
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
                    decimal difference = settlementInvoiceItemSummary.Value.DueToCarrierFullAmount - settlementInvoiceItemSummary.Value.DueToSystemFullAmount;
                    if (difference > 0)
                    {
                        settlementInvoiceItemSummary.Value.DueToCarrierDifference = difference;
                        if (carrierSummary == null)
                            carrierSummary = new List<SettlementInvoiceDetailSummary>();

                        carrierSummary.Add(new SettlementInvoiceDetailSummary
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

                    if (settlementInvoiceItemSummary.Value.DueToCarrierAmountRecurringCharges > 0)
                    {
                        if (carrierRecurringChargesSummary == null)
                            carrierRecurringChargesSummary = new List<SettlementInvoiceDetailSummary>();
                        carrierRecurringChargesSummary.Add(new SettlementInvoiceDetailSummary
                        {
                            Amount = settlementInvoiceItemSummary.Value.DueToCarrierAmountRecurringCharges,
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

        private void PrepareDataForProcessing(string partnerId, Guid invoiceTypeId, dynamic customSectionPayload, out string partnerType, out bool isApplicableToSupplier, out bool isApplicableToCustomer, out List<InvoiceAvailableForSettlement> availableCustomerInvoices, out List<InvoiceAvailableForSettlement> availableSupplierInvoices, out int currencyId)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(partnerId));
            currencyId = financialAccountManager.GetFinancialAccountCurrencyId(financialAccount);

            var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
            definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
            var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == invoiceTypeId);
            financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");

            isApplicableToSupplier = financialAccountInvoiceType.IsApplicableToSupplier;
            isApplicableToCustomer = financialAccountInvoiceType.IsApplicableToCustomer;
            // commission = null;
            partnerType = null;

            availableCustomerInvoices = null;
            availableSupplierInvoices = null;

            var settlementGenerationCustomSectionPayload = customSectionPayload as SettlementGenerationCustomSectionPayload;
            if (settlementGenerationCustomSectionPayload == null)
                return;

            if (settlementGenerationCustomSectionPayload != null)
            {
                //if (settlementGenerationCustomSectionPayload.Commission.HasValue)
                //{
                //    commission = settlementGenerationCustomSectionPayload.Commission.Value;
                //}

                availableCustomerInvoices = settlementGenerationCustomSectionPayload.AvailableCustomerInvoices;
                availableSupplierInvoices = settlementGenerationCustomSectionPayload.AvailableSupplierInvoices;
            }

            if (financialAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
            }
            else
            {
                partnerType = "Account";
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

        private GeneratedInvoice BuildGeneratedInvoice(List<SattlementInvoiceItemDetails> customerInvoiceItemSet, List<SattlementInvoiceItemDetails> supplierInvoiceItemSet,
            Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryByCurrency, List<SettlementInvoiceDetailSummary> carrierSummary, List<SettlementInvoiceDetailSummary> systemSummary,
            List<SettlementInvoiceDetailSummary> carrierRecurringChargesSummary, List<SettlementInvoiceDetailSummary> systemRecurringChargesSummary,
            Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice, string partnerType, bool isApplicableToCustomer, bool isApplicableToSupplier, Dictionary<String, SettlementInvoiceItemDetail> settlementInvoiceByCurrency, int currencyId, DateTime issueDate)
        {
            CurrencyExchangeRateManager exchangeRateManager = new CurrencyExchangeRateManager();
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();

            var sattlementInvoiceDetails = new SattlementInvoiceDetails
            {
                PartnerType = partnerType,
                IsApplicableToCustomer = isApplicableToCustomer,
                IsApplicableToSupplier = isApplicableToSupplier
                // Commission = commission,
                // HasComission = commission.HasValue ? true : false
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
                    sattlementInvoiceDetails.CustomerDuration += customerInvoice.DurationInSeconds;
                    sattlementInvoiceDetails.CustomerTotalNumberOfCalls += customerInvoice.TotalNumberOfCalls;
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
                    sattlementInvoiceDetails.SupplierDuration += supplierInvoice.DurationInSeconds;
                    sattlementInvoiceDetails.SupplierTotalNumberOfCalls += supplierInvoice.TotalNumberOfCalls;

                    supplierItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = supplierInvoice,
                        Name = " "
                    });
                }

                sattlementInvoiceDetails.IsOriginalAmountSetted = supplierInvoiceItemSet.All(x => x.IsOriginalAmountSetted);

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
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
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

                    sattlementInvoiceDetails.DueToCarrierAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierAmountAfterCommission += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierAmountAfterCommission, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierAmountAfterCommissionWithTaxes += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierAmountAfterCommissionWithTaxes, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierAmountRecurringCharges += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierAmountRecurringCharges, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierTotalTrafficAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierTotalTrafficAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierTotalDealAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierTotalDealAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierTotalSMSAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierTotalSMSAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToCarrierFullAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToCarrierFullAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);// sattlementInvoiceDetails.ful.DueToCarrierTotalTrafficAmount + sattlementInvoiceDetails.DueToCarrierAmountRecurringCharges + sattlementInvoiceDetails.DueToCarrierTotalDealAmount + sattlementInvoiceDetails.DueToCarrierTotalSMSAmount;

                    sattlementInvoiceDetails.DueToSystemAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemAmountAfterCommission += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmountAfterCommission, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemAmountAfterCommissionWithTaxes += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmountAfterCommissionWithTaxes, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemAmountRecurringCharges += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemAmountRecurringCharges, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemTotalTrafficAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemTotalTrafficAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemTotalDealAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemTotalDealAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemTotalSMSAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemTotalSMSAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate);
                    sattlementInvoiceDetails.DueToSystemFullAmount += exchangeRateManager.ConvertValueToCurrency(settlementInvoice.Value.DueToSystemFullAmount, settlementInvoice.Value.CurrencyId, currencyId, issueDate); //sattlementInvoiceDetails.DueToSystemTotalTrafficAmount + sattlementInvoiceDetails.DueToSystemAmountRecurringCharges + sattlementInvoiceDetails.DueToSystemTotalDealAmount + sattlementInvoiceDetails.DueToSystemTotalSMSAmount;

                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = settlementInvoice.Value,
                        Name = " "
                    });

                    if (generatedInvoiceItemSet.Items.Count > 0)
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }

            }

            if (sattlementInvoiceDetails.DueToCarrierFullAmount > sattlementInvoiceDetails.DueToSystemFullAmount)
            {
                sattlementInvoiceDetails.DueToCarrierDifference = sattlementInvoiceDetails.DueToCarrierFullAmount - sattlementInvoiceDetails.DueToSystemFullAmount;
            }
            else
            {
                sattlementInvoiceDetails.DueToSystemDifference = sattlementInvoiceDetails.DueToSystemFullAmount - sattlementInvoiceDetails.DueToCarrierFullAmount;
            }

            sattlementInvoiceDetails.NoCarrierDeals = sattlementInvoiceDetails.DueToCarrierTotalDealAmount == 0;
            sattlementInvoiceDetails.NoCarrierSMS = sattlementInvoiceDetails.DueToCarrierTotalSMSAmount == 0;
            sattlementInvoiceDetails.NoCarrierVoice = sattlementInvoiceDetails.DueToCarrierAmountAfterCommissionWithTaxes == 0;
            sattlementInvoiceDetails.NoCarrierRecurringCharges = sattlementInvoiceDetails.DueToCarrierAmountRecurringCharges == 0;
            sattlementInvoiceDetails.NoSystemDeals = sattlementInvoiceDetails.DueToSystemTotalDealAmount == 0;
            sattlementInvoiceDetails.NoSystemSMS = sattlementInvoiceDetails.DueToSystemTotalSMSAmount == 0;
            sattlementInvoiceDetails.NoSystemVoice = sattlementInvoiceDetails.DueToSystemAmountAfterCommissionWithTaxes == 0;
            sattlementInvoiceDetails.NoSystemRecurringCharges = sattlementInvoiceDetails.DueToSystemAmountRecurringCharges == 0;

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
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
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
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
            }

            if (carrierSummary != null && carrierSummary.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "CarrierSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                foreach (var summary in carrierSummary)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summary,
                        Name = " "
                    });
                }

                if (generatedInvoiceItemSet.Items.Count > 0)
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
            }

            if (carrierRecurringChargesSummary != null && carrierRecurringChargesSummary.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "CarrierRecurringChargesSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                foreach (var summary in carrierRecurringChargesSummary)
                {
                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = summary,
                        Name = " "
                    });
                }

                if (generatedInvoiceItemSet.Items.Count > 0)
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
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
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
            }

            return new GeneratedInvoice
            {
                InvoiceDetails = sattlementInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets
            };
        }
    }
}