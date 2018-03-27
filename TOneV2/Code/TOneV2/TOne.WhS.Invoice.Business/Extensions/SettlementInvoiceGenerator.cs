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
            WHSFinancialAccountManager financialAccountManager = new WHSFinancialAccountManager();
            var financialAccount = financialAccountManager.GetFinancialAccount(Convert.ToInt32(context.PartnerId));


            var definitionSettings = new WHSFinancialAccountDefinitionManager().GetFinancialAccountDefinitionSettings(financialAccount.FinancialAccountDefinitionId);
            definitionSettings.ThrowIfNull("definitionSettings", financialAccount.FinancialAccountDefinitionId);
            definitionSettings.FinancialAccountInvoiceTypes.ThrowIfNull("definitionSettings.FinancialAccountInvoiceTypes", financialAccount.FinancialAccountDefinitionId);
            var financialAccountInvoiceType = definitionSettings.FinancialAccountInvoiceTypes.FindRecord(x => x.InvoiceTypeId == context.InvoiceTypeId);
            financialAccountInvoiceType.ThrowIfNull("financialAccountInvoiceType");



            var settlementGenerationCustomSectionPayload = context.CustomSectionPayload as SettlementGenerationCustomSectionPayload;
            if (settlementGenerationCustomSectionPayload == null)
                return;

            decimal? commission = null;
            if (settlementGenerationCustomSectionPayload != null)
            {
                if (settlementGenerationCustomSectionPayload.Commission.HasValue)
                {
                    commission = settlementGenerationCustomSectionPayload.Commission.Value;
                }
            }

            DateTime fromDate = context.FromDate;
            DateTime toDate = context.ToDate;

            string partnerType = null;
            if (financialAccount.CarrierProfileId.HasValue)
            {
                partnerType = "Profile";
            }
            else
            {
                partnerType = "Account";
            }


            IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices = null;
            IEnumerable<InvoiceItem> customerInvoiceItems = null;

            IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices = null;
            IEnumerable<InvoiceItem> supplierInvoiceItems = null;

            InvoiceItemManager _invoiceItemManager = new InvoiceItemManager();


            List<long> invoiceToSettleIds = null;


            if (definitionSettings.ExtendedSettings.IsApplicableToCustomer)
            {
                if (settlementGenerationCustomSectionPayload.AvailableCustomerInvoices != null)
                {
                    if (invoiceToSettleIds == null)
                        invoiceToSettleIds = new List<long>();

                    List<long> customerInvoiceIds = new List<long>();
                    foreach (var availableCustomerInvoice in settlementGenerationCustomSectionPayload.AvailableCustomerInvoices)
                    {
                        if (availableCustomerInvoice.IsSelected)
                        {
                            invoiceToSettleIds.Add(availableCustomerInvoice.InvoiceId);
                            customerInvoiceIds.Add(availableCustomerInvoice.InvoiceId);
                        }
                    }

                    customerInvoices = GetInvoices(customerInvoiceIds);
                    if (customerInvoices != null && customerInvoices.Count() > 0)
                    {
                        ValidateInvoicesDates(customerInvoices, fromDate, toDate, context);
                        customerInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(_customerInvoiceTypeId, customerInvoiceIds, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
                    }
                }
            }

            if (definitionSettings.ExtendedSettings.IsApplicableToSupplier)
            {
                if (settlementGenerationCustomSectionPayload.AvailableSupplierInvoices != null)
                {
                    if (invoiceToSettleIds == null)
                        invoiceToSettleIds = new List<long>();

                    List<long> supplierInvoiceIds = new List<long>();

                    foreach (var availableSupplierInvoice in settlementGenerationCustomSectionPayload.AvailableSupplierInvoices)
                    {
                        if (availableSupplierInvoice.IsSelected)
                        {
                            invoiceToSettleIds.Add(availableSupplierInvoice.InvoiceId);
                            supplierInvoiceIds.Add(availableSupplierInvoice.InvoiceId);
                        }
                    }

                    supplierInvoices = GetInvoices(supplierInvoiceIds);
                    if (supplierInvoices != null && supplierInvoices.Count() > 0)
                    {
                        ValidateInvoicesDates(supplierInvoices, fromDate, toDate, context);

                        supplierInvoiceItems = _invoiceItemManager.GetInvoiceItemsByItemSetNames(_supplierInvoiceTypeId, supplierInvoiceIds, new List<string> { "GroupingByCurrency" }, CompareOperator.Equal);
                    }
                }
            }

            context.InvoiceToSettleIds = invoiceToSettleIds;



            Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic = ConvertInvoicesToItemSetNames(customerInvoices, supplierInvoices, commission);
            if (itemSetNamesDic.Count == 0)
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;

            }

            Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryDetails = BuildSettlementInvoiceItemSummaryDetails(customerInvoiceItems, supplierInvoiceItems, supplierInvoices);
            List<SettlementInvoiceDetailSummary> systemSummary = null;
            List<SettlementInvoiceDetailSummary> carrierSummary = null;
            BuildSettlementInvoiceDetailSummary(settlementInvoiceItemSummaryDetails, out systemSummary, out carrierSummary, commission);

            #region BuildSupplierInvoiceDetails

            SattlementInvoiceDetails sattlementInvoiceDetails = BuildSattlementInvoiceDetails(itemSetNamesDic, partnerType, context.FromDate, context.ToDate, commission);
            if (sattlementInvoiceDetails != null)
            {

                sattlementInvoiceDetails.PartnerType = partnerType;

                context.Invoice = new GeneratedInvoice
                {
                    InvoiceDetails = sattlementInvoiceDetails,
                    InvoiceItemSets = BuildGeneratedInvoiceItemSet(itemSetNamesDic, settlementInvoiceItemSummaryDetails, systemSummary, carrierSummary),
                };
            }

            #endregion
        }


        Dictionary<int, SettlementInvoiceItemSummaryDetail> BuildSettlementInvoiceItemSummaryDetails(IEnumerable<InvoiceItem> customerInvoiceItems, IEnumerable<InvoiceItem> supplierInvoiceItems, IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices)
       {
           var settlementInvoiceItemSummaryDetails = new Dictionary<int, SettlementInvoiceItemSummaryDetail>();

           if (customerInvoiceItems != null)
           {
               foreach (var customerInvoiceItem in customerInvoiceItems)
               {

                   var customerInvoiceItemDetails = customerInvoiceItem.Details as CustomerInvoiceBySaleCurrencyItemDetails;
                   if (customerInvoiceItemDetails != null)
                   {
                       var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryDetails.GetOrCreateItem(customerInvoiceItemDetails.CurrencyId);
                       settlementInvoiceItemSummaryDetail.CurrencyId = customerInvoiceItemDetails.CurrencyId;
                       settlementInvoiceItemSummaryDetail.DueToSystemAmount += customerInvoiceItemDetails.AmountAfterCommissionWithTaxes;
                       settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommission += customerInvoiceItemDetails.AmountAfterCommissionWithTaxes;
                       settlementInvoiceItemSummaryDetail.DueToSystemAmountAfterCommissionWithTaxes += customerInvoiceItemDetails.AmountAfterCommissionWithTaxes;
                       settlementInvoiceItemSummaryDetail.DueToSystemNumberOfCalls += customerInvoiceItemDetails.NumberOfCalls;
                   }

               }
           }

           if (supplierInvoiceItems != null && supplierInvoiceItems.Count() > 0)
           {
               foreach (var invoice in supplierInvoices)
               {
                   var invoiceDetail = invoice.Details as SupplierInvoiceDetails;
                   if(invoiceDetail.IncludeOriginalAmountInSettlement && invoiceDetail.OriginalAmount.HasValue)
                   {
                       var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryDetails.GetOrCreateItem(invoiceDetail.SupplierCurrencyId);
                       settlementInvoiceItemSummaryDetail.CurrencyId += invoiceDetail.SupplierCurrencyId;
                       settlementInvoiceItemSummaryDetail.DueToCarrierAmount += invoiceDetail.OriginalAmount.Value;
                       settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommission += invoiceDetail.OriginalAmount.Value;
                       settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommissionWithTaxes += invoiceDetail.OriginalAmount.Value;
                       settlementInvoiceItemSummaryDetail.DueToCarrierNumberOfCalls += invoiceDetail.TotalNumberOfCalls;

                   }else
                   {
                       foreach (var supplierInvoiceItem in supplierInvoiceItems)
                       {
                           if(supplierInvoiceItem.InvoiceId == invoice.InvoiceId)
                           {
                               var supplierInvoiceItemDetails = supplierInvoiceItem.Details as SupplierInvoiceBySaleCurrencyItemDetails;
                               if (supplierInvoiceItemDetails != null)
                               {
                                   var settlementInvoiceItemSummaryDetail = settlementInvoiceItemSummaryDetails.GetOrCreateItem(supplierInvoiceItemDetails.CurrencyId);
                                   settlementInvoiceItemSummaryDetail.CurrencyId = supplierInvoiceItemDetails.CurrencyId;
                                   settlementInvoiceItemSummaryDetail.DueToCarrierAmount += supplierInvoiceItemDetails.AmountAfterCommissionWithTaxes;
                                   settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommission += supplierInvoiceItemDetails.AmountAfterCommissionWithTaxes;
                                   settlementInvoiceItemSummaryDetail.DueToCarrierAmountAfterCommissionWithTaxes += supplierInvoiceItemDetails.AmountAfterCommissionWithTaxes;
                                   settlementInvoiceItemSummaryDetail.DueToCarrierNumberOfCalls += supplierInvoiceItemDetails.NumberOfCalls;
                               }
                           }
                       }
                   }
               }
               
           }
           return settlementInvoiceItemSummaryDetails;
       }
        void BuildSettlementInvoiceDetailSummary(Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryDetails, out List<SettlementInvoiceDetailSummary> systemSummary, out List<SettlementInvoiceDetailSummary> carrierSummary, decimal? commission)
        {
            systemSummary = null;
            carrierSummary = null;
             if(settlementInvoiceItemSummaryDetails!= null && settlementInvoiceItemSummaryDetails.Count > 0)
             {
                 foreach(var settlementInvoiceItemSummaryDetail in settlementInvoiceItemSummaryDetails)
                 {
                     decimal sum = settlementInvoiceItemSummaryDetail.Value.DueToCarrierAmount - settlementInvoiceItemSummaryDetail.Value.DueToSystemAmount;
                     if(sum > 0)
                     {
                         if(carrierSummary == null)
                             carrierSummary = new List<SettlementInvoiceDetailSummary>();

                         carrierSummary.Add(new SettlementInvoiceDetailSummary
                         {
                             Amount =sum ,
                             CurrencyId = settlementInvoiceItemSummaryDetail.Value.CurrencyId,
                             CurrencyIdDescription = settlementInvoiceItemSummaryDetail.Value.CurrencyIdDescription,
                             AmountWithCommission = commission.HasValue ? sum + ((sum * commission.Value) / 100) : sum
                         });

                     }
                     else if (sum < 0)
                     {
                         if (systemSummary == null)
                             systemSummary = new List<SettlementInvoiceDetailSummary>();
                         var amount = Math.Abs(sum);
                         systemSummary.Add(new SettlementInvoiceDetailSummary
                         {
                             Amount = amount,
                              CurrencyId = settlementInvoiceItemSummaryDetail.Value.CurrencyId,
                              CurrencyIdDescription = settlementInvoiceItemSummaryDetail.Value.CurrencyIdDescription,
                              AmountWithCommission = commission.HasValue ? amount + ((amount * commission.Value) / 100) : amount
                         });
                     }
                 }
             }
        }

        Dictionary<string, List<SattlementInvoiceItemDetails>> ConvertInvoicesToItemSetNames(IEnumerable<Vanrise.Invoice.Entities.Invoice> customerInvoices, IEnumerable<Vanrise.Invoice.Entities.Invoice> supplierInvoices,  decimal? commission)
        {
            Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic = new Dictionary<string, List<SattlementInvoiceItemDetails>>();


            if (customerInvoices != null)
            {
                foreach (var customerInvoice in customerInvoices)
                {

                    var customerInvoiceDetails = customerInvoice.Details as CustomerInvoiceDetails;
                    if (customerInvoiceDetails != null)
                    {
                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = customerInvoiceDetails.TotalAmountAfterCommission,
                            DurationInSeconds = customerInvoiceDetails.Duration,
                            CurrencyId = customerInvoiceDetails.SaleCurrencyId,
                            InvoiceId = customerInvoice.InvoiceId,
                            InvoiceTypeId = customerInvoice.InvoiceTypeId,
                            TotalNumberOfCalls = customerInvoiceDetails.TotalNumberOfCalls,
                            AmountWithCommission = commission.HasValue ? customerInvoiceDetails.TotalAmountAfterCommission + ((customerInvoiceDetails.TotalAmountAfterCommission * commission.Value) / 100) : customerInvoiceDetails.TotalAmountAfterCommission,
                            Commission = customerInvoiceDetails.Commission
                        };
                        AddItemToDictionary(itemSetNamesDic, "CustomerInvoices", sattlementInvoiceItemDetails);
                    }

                }
            }

            if (supplierInvoices != null)
            {
                foreach (var supplierInvoice in supplierInvoices)
                {

                    var supplierInvoiceDetails = supplierInvoice.Details as SupplierInvoiceDetails;
                    if (supplierInvoiceDetails != null)
                    {
                        var sattlementInvoiceItemDetails = new SattlementInvoiceItemDetails
                        {
                            Amount = supplierInvoiceDetails.IncludeOriginalAmountInSettlement && supplierInvoiceDetails.OriginalAmount.HasValue?supplierInvoiceDetails.OriginalAmount.Value: supplierInvoiceDetails.TotalAmountAfterCommission,
                            DurationInSeconds = supplierInvoiceDetails.Duration,
                            CurrencyId = supplierInvoiceDetails.SupplierCurrencyId,
                            InvoiceId = supplierInvoice.InvoiceId,
                            InvoiceTypeId = supplierInvoice.InvoiceTypeId,
                            TotalNumberOfCalls = supplierInvoiceDetails.TotalNumberOfCalls,
                            AmountWithCommission = commission.HasValue ? supplierInvoiceDetails.TotalAmountAfterCommission + ((supplierInvoiceDetails.TotalAmountAfterCommission * commission.Value) / 100) : supplierInvoiceDetails.TotalAmountAfterCommission,
                            Commission = supplierInvoiceDetails.Commission
                        };
                        AddItemToDictionary(itemSetNamesDic, "SupplierInvoices", sattlementInvoiceItemDetails);
                    }
                }
            }

            return itemSetNamesDic;
        }



        void ValidateInvoicesDates(IEnumerable<Vanrise.Invoice.Entities.Invoice> invoices, DateTime fromDate, DateTime toDate, IInvoiceGenerationContext context)
        {
            if (invoices.Min(x => x.FromDate) < fromDate || invoices.Max(x => x.ToDate) > toDate)
            {
                context.ErrorMessage = "Unable to generate settlement at this period.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;

            }
        }

        IEnumerable<Vanrise.Invoice.Entities.Invoice> GetInvoices(List<long> invoiceIds)
        {
            return new Vanrise.Invoice.Business.InvoiceManager().GetInvoices(invoiceIds);
        }

        SattlementInvoiceDetails BuildSattlementInvoiceDetails(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, string partnerType, DateTime fromDate, DateTime toDate,decimal? commission)
        {
            SattlementInvoiceDetails sattlementInvoiceDetails = null;
            if (partnerType != null)
            {

                if (itemSetNamesDic != null)
                {
                    sattlementInvoiceDetails = new SattlementInvoiceDetails();
                    sattlementInvoiceDetails.PartnerType = partnerType;
                    sattlementInvoiceDetails.Commission =commission ;
                    sattlementInvoiceDetails.HasComission = commission.HasValue ? true : false;

                    List<SattlementInvoiceItemDetails> customerInvoices = null;
                    if (itemSetNamesDic.TryGetValue("CustomerInvoices", out customerInvoices))
                    {
                        foreach (var customerInvoice in customerInvoices)
                        {

                            sattlementInvoiceDetails.CustomerDuration += customerInvoice.DurationInSeconds;
                            sattlementInvoiceDetails.CustomerTotalNumberOfCalls += customerInvoice.TotalNumberOfCalls;
                        }
                       
                    };
                    
                    List<SattlementInvoiceItemDetails> supplierInvoices = null;
                    if (itemSetNamesDic.TryGetValue("SupplierInvoices", out supplierInvoices))
                    {
                        foreach (var supplierInvoice in supplierInvoices)
                        {
                            sattlementInvoiceDetails.SupplierDuration += supplierInvoice.DurationInSeconds;
                            sattlementInvoiceDetails.SupplierTotalNumberOfCalls += supplierInvoice.TotalNumberOfCalls;
                        }

                    };

                }
            }
            return sattlementInvoiceDetails;
        }
      
        List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, Dictionary<int, SettlementInvoiceItemSummaryDetail> settlementInvoiceItemSummaryDetails, List<SettlementInvoiceDetailSummary> systemSummary,  List<SettlementInvoiceDetailSummary> carrierSummary)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            if (itemSetNamesDic != null)
            {
                foreach (var itemSet in itemSetNamesDic)
                {
                    GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                    generatedInvoiceItemSet.SetName = itemSet.Key;
                    var itemSetValues = itemSet.Value;
                    generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();

                    foreach (var item in itemSetValues)
                    {
                        generatedInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                        {
                            Details = item,
                            Name = " "
                        });
                    }
                    if (generatedInvoiceItemSet.Items.Count > 0)
                    {
                        generatedInvoiceItemSets.Add(generatedInvoiceItemSet);
                    }
                }
            }
            if (settlementInvoiceItemSummaryDetails != null && settlementInvoiceItemSummaryDetails.Count > 0)
            {
                GeneratedInvoiceItemSet generatedInvoiceItemSet = new GeneratedInvoiceItemSet();
                generatedInvoiceItemSet.SetName = "SettlementInvoiceSummary";
                generatedInvoiceItemSet.Items = new List<GeneratedInvoiceItem>();
                foreach (var settlementInvoiceItemSummaryDetail in settlementInvoiceItemSummaryDetails)
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
            return generatedInvoiceItemSets;
        }
   
        
        private void AddItemToDictionary(Dictionary<string, List<SattlementInvoiceItemDetails>> itemSetNamesDic, string key, SattlementInvoiceItemDetails sattlementInvoiceItemDetails)
        {
            if (itemSetNamesDic == null)
                itemSetNamesDic = new Dictionary<string, List<SattlementInvoiceItemDetails>>();
            List<SattlementInvoiceItemDetails> invoiceBillingRecordList = itemSetNamesDic.GetOrCreateItem(key);
            invoiceBillingRecordList.Add(sattlementInvoiceItemDetails);
        }
    }
}
