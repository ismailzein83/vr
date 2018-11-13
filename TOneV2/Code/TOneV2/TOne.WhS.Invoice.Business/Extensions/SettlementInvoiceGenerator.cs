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

            PrepareDataForProcessing(context.PartnerId, context.InvoiceTypeId, context.CustomSectionPayload, out partnerType, out isApplicableToSupplier, out isApplicableToCustomer, out availableCustomerInvoices, out availableSupplierInvoices);


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

            ProcessItemSetName(customerInvoices, customerInvoiceItems, supplierInvoices, supplierInvoiceItems, availableCustomerInvoices, availableSupplierInvoices, out carrierRecurringChargesSummary, out systemRecurringChargesSummary, out customerInvoiceItemSet, out supplierInvoiceItemSet, out settlementInvoiceItemSummaryByCurrency, out carrierSummary, out systemSummary, out settlementInvoiceCurrencyByInvoice, out settlementInvoiceByCurrency);

            context.Invoice = BuildGeneratedInvoice(customerInvoiceItemSet, supplierInvoiceItemSet, settlementInvoiceItemSummaryByCurrency, carrierSummary, systemSummary, carrierRecurringChargesSummary, systemRecurringChargesSummary, settlementInvoiceCurrencyByInvoice, partnerType, isApplicableToCustomer, isApplicableToSupplier, settlementInvoiceByCurrency);


            ConfigManager configManager = new ConfigManager();
            InvoiceTypeSetting settings = configManager.GetInvoiceTypeSettingsById(context.InvoiceTypeId);

            if (settings != null)
            {
                context.NeedApproval = settings.NeedApproval;
            }
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

                    var customerInvoiceDetails = customerInvoice.Details as CustomerInvoiceDetails;
                    if (customerInvoiceDetails != null)
                    {

                        var invoiceItems = customerInvoiceItems.FindAllRecords(x => x.InvoiceId == customerInvoice.InvoiceId);
                        if (invoiceItems != null)
                        {
                            var settlementInvoiceCurrency = settlementInvoiceCurrencyByInvoice.GetOrCreateItem(customerInvoice.InvoiceId);
                            foreach (var invoiceItem in invoiceItems)
                            {
                                var invoiceItemDetails = invoiceItem.Details as CustomerInvoiceBySaleCurrencyItemDetails;
                                if (invoiceItemDetails != null && availableCustomerInvoices.Any(x => x.InvoiceId == customerInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId && x.IsSelected))
                                {
                                    var settlementInvoicedetail = settlementInvoiceByCurrency.GetOrCreateItem(string.Format("{0}_{1}", invoiceItemDetails.CurrencyId, invoiceItemDetails.Month), () =>
                                    {
                                        return new SettlementInvoiceItemDetail()
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
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommission += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommissionWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToSystemNumberOfCalls += customerInvoiceDetails.TotalNumberOfCalls;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = originalDataCurrrency.OriginalAmount.Value;

                                        settlementInvoicedetail.DueToSystemAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToSystemAmountAfterCommission += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToSystemAmountAfterCommissionWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToSystemNumberOfCalls += customerInvoiceDetails.TotalNumberOfCalls;
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

                                        settlementInvoiceDetailByCurrency.OriginalAmount = invoiceItemDetails.Amount;
                                        settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = invoiceItemDetails.AmountAfterCommissionWithTaxes;

                                        settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceDetailByCurrency.TotalTrafficAmount = invoiceItemDetails.TotalTrafficAmount;

                                        settlementInvoicedetail.DueToSystemAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToSystemAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToSystemAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToSystemNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoicedetail.DueToSystemAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoicedetail.DueToSystemTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                    }
                                    settlementInvoiceCurrency.Add(settlementInvoiceDetailByCurrency);
                                }
                            }
                        }
                        bool multipleCurrencies = invoiceItems != null && invoiceItems.Count() > 1;

                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {

                            Amount = multipleCurrencies ? default(decimal?) : customerInvoiceDetails.TotalAmountAfterCommission,
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
                            OriginalAmount = !multipleCurrencies && customerInvoiceDetails.OriginalAmountByCurrency != null && customerInvoiceDetails.OriginalAmountByCurrency.Count > 0 ? customerInvoiceDetails.OriginalAmountByCurrency.First().Value.OriginalAmount : default(decimal?),
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

                    var supplierInvoiceDetails = supplierInvoice.Details as SupplierInvoiceDetails;
                    if (supplierInvoiceDetails != null)
                    {

                        var invoiceItems = supplierInvoiceItems.FindAllRecords(x => x.InvoiceId == supplierInvoice.InvoiceId);
                        if (invoiceItems != null)
                        {
                            var settlementInvoiceCurrency = settlementInvoiceCurrencyByInvoice.GetOrCreateItem(supplierInvoice.InvoiceId);

                            foreach (var invoiceItem in invoiceItems)
                            {
                                var invoiceItemDetails = invoiceItem.Details as SupplierInvoiceBySaleCurrencyItemDetails;
                                if (invoiceItemDetails != null && availableSupplierInvoices.Any(x => x.InvoiceId == supplierInvoice.InvoiceId && x.CurrencyId == invoiceItemDetails.CurrencyId && x.IsSelected))
                                {
                                    var settlementInvoicedetail = settlementInvoiceByCurrency.GetOrCreateItem(string.Format("{0}_{1}", invoiceItemDetails.CurrencyId, invoiceItemDetails.Month), () =>
                                    {
                                        return new SettlementInvoiceItemDetail()
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
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommission += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommissionWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierNumberOfCalls += supplierInvoiceDetails.TotalNumberOfCalls;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = originalDataCurrrency.OriginalAmount.Value;

                                        settlementInvoicedetail.DueToCarrierAmount += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToCarrierAmountAfterCommission += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToCarrierAmountAfterCommissionWithTaxes += originalDataCurrrency.OriginalAmount.Value;
                                        settlementInvoicedetail.DueToCarrierNumberOfCalls += supplierInvoiceDetails.TotalNumberOfCalls;
                                    }
                                    else
                                    {
                                        var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryByCurrency.GetOrCreateItem(invoiceItemDetails.CurrencyId);
                                        settlementInvoiceItemSummaryDetail.CurrencyId = invoiceItemDetails.CurrencyId;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceItemSummaryDetail.DueToCarrierTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;

                                        settlementInvoiceDetailByCurrency.OriginalAmount = invoiceItemDetails.Amount;
                                        settlementInvoiceDetailByCurrency.OriginalAmountWithCommission = invoiceItemDetails.AmountAfterCommissionWithTaxes;

                                        settlementInvoiceDetailByCurrency.TotalRecurringChargeAmount = invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoiceDetailByCurrency.TotalTrafficAmount = invoiceItemDetails.TotalTrafficAmount;

                                        settlementInvoicedetail.DueToCarrierAmount += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToCarrierAmountAfterCommission += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToCarrierAmountAfterCommissionWithTaxes += invoiceItemDetails.AmountAfterCommissionWithTaxes;
                                        settlementInvoicedetail.DueToCarrierNumberOfCalls += invoiceItemDetails.NumberOfCalls;
                                        settlementInvoicedetail.DueToCarrierAmountRecurringCharges += invoiceItemDetails.TotalRecurringChargeAmount;
                                        settlementInvoicedetail.DueToCarrierTotalTrafficAmount += invoiceItemDetails.TotalTrafficAmount;
                                    }
                                    settlementInvoiceCurrency.Add(settlementInvoiceDetailByCurrency);


                                }
                            }
                        }

                        bool multipleCurrencies = invoiceItems != null && invoiceItems.Count() > 1;


                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = multipleCurrencies ? default(decimal?) : supplierInvoiceDetails.TotalAmountAfterCommission,
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
                            OriginalAmount = !multipleCurrencies && supplierInvoiceDetails.OriginalAmountByCurrency != null && supplierInvoiceDetails.OriginalAmountByCurrency.Count > 0 ? supplierInvoiceDetails.OriginalAmountByCurrency.First().Value.OriginalAmount : default(decimal?),
                            TimeZoneId = supplierInvoiceDetails.TimeZoneId,
                            ToDate = supplierInvoice.ToDate,
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
                    decimal sum = settlementInvoiceItemSummary.Value.DueToCarrierAmountAfterCommissionWithTaxes - settlementInvoiceItemSummary.Value.DueToSystemAmountAfterCommissionWithTaxes;
                    if (sum > 0)
                    {
                        settlementInvoiceItemSummary.Value.DueToCarrierDifference = sum;
                        if (carrierSummary == null)
                            carrierSummary = new List<SettlementInvoiceDetailSummary>();

                        carrierSummary.Add(new SettlementInvoiceDetailSummary
                        {
                            Amount = sum,
                            CurrencyId = settlementInvoiceItemSummary.Value.CurrencyId,
                            CurrencyIdDescription = settlementInvoiceItemSummary.Value.CurrencyIdDescription,
                            // OriginalAmountWithCommission = commission.HasValue ? sum + ((sum * commission.Value) / 100) : sum,
                        });

                    }
                    else if (sum < 0)
                    {
                        settlementInvoiceItemSummary.Value.DueToSystemDifference = Math.Abs(sum);

                        if (systemSummary == null)
                            systemSummary = new List<SettlementInvoiceDetailSummary>();
                        var amount = Math.Abs(sum);
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
        private void PrepareDataForProcessing(string partnerId, Guid invoiceTypeId, dynamic customSectionPayload, out string partnerType, out bool isApplicableToSupplier, out bool isApplicableToCustomer, out List<InvoiceAvailableForSettlement> availableCustomerInvoices, out List<InvoiceAvailableForSettlement> availableSupplierInvoices)
        {
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(partnerId));

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
            Dictionary<long, List<SettlementInvoiceDetailByCurrency>> settlementInvoiceCurrencyByInvoice, string partnerType, bool isApplicableToCustomer, bool isApplicableToSupplier, Dictionary<String, SettlementInvoiceItemDetail> settlementInvoiceByCurrency)
        {


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

                    generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = settlementInvoice.Value,
                        Name = " "
                    });

                    if (generatedInvoiceItemSet.Items.Count > 0)
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);

                }

            }

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
                {
                    generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                }
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
                InvoiceDetails = sattlementInvoiceDetails,
                InvoiceItemSets = generatedInvoiceItemSets
            };
        }
    }
}
