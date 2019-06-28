using Retail.Billing.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Invoice.Entities;
using Vanrise.InvToAccBalanceRelation.Entities;

namespace Retail.Billing.Business
{
    public class BillingInvoiceGenerator : InvoiceGenerator
    {
        #region Constructors
        GenericFinancialAccountConfiguration _financialAccountConfiguration;
        Guid? _invoiceTransactionTypeId;
        List<Guid> _usageTransactionTypeIds;
        Guid? _vatRuleDefinitionId;


        public BillingInvoiceGenerator(GenericFinancialAccountConfiguration financialAccountConfiguration, Guid? invoiceTransactionTypeId, List<Guid> usageTransactionTypeIds, Guid? vatRuleDefinitionId)
        {
            this._financialAccountConfiguration = financialAccountConfiguration;
            this._invoiceTransactionTypeId = invoiceTransactionTypeId;
            this._usageTransactionTypeIds = usageTransactionTypeIds;
            this._vatRuleDefinitionId = vatRuleDefinitionId;
        }

        #endregion

        #region Generator

        private CurrencyExchangeRateManager _currencyExchangeRateManager = new CurrencyExchangeRateManager();
        private BillingContractServiceManager _billingContractServiceManager = new BillingContractServiceManager();
        private BillingRatePlanServiceManager _billingRatePlanServiceManager = new BillingRatePlanServiceManager();
        private TaxRuleManager _taxRuleManager = new TaxRuleManager();

        public override void GenerateInvoice(IInvoiceGenerationContext context)
        {
            GenericFinancialAccountManager financialAccountManager = new GenericFinancialAccountManager(_financialAccountConfiguration);
            var financialAccountData = financialAccountManager.GetFinancialAccount(context.PartnerId);

            if (context.FromDate < financialAccountData.BED || context.ToDate > financialAccountData.EED)
            {
                context.ErrorMessage = "From date and To date should be within the effective date of financial account.";
                context.GenerateInvoiceResult = GenerateInvoiceResult.Failed;
                return;
            }

            decimal totalAmount;
            decimal recurringCharges;
            decimal totalActivationCharges;
            decimal totalSuspensionCharges;
            decimal? taxAmount = null;
            decimal? totalAmountAfterTaxes = null;
            decimal vatPercentage = 0;

            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = BuildGeneratedInvoiceItemSet(financialAccountData.FinancialAccountId, context.FromDate, context.ToDate, context.IssueDate, financialAccountData.CurrencyId, out totalAmount, out recurringCharges, out totalActivationCharges, out totalSuspensionCharges);

            if (!generatedInvoiceItemSets.Any(itemset => itemset.Items != null && itemset.Items.Count > 0))
            {
                context.GenerateInvoiceResult = GenerateInvoiceResult.NoData;
                return;
            }

            if (_vatRuleDefinitionId.HasValue)
            {
                taxAmount = GetTaxAmount(context.PartnerId, totalAmount, financialAccountData.CurrencyId, context.IssueDate, out vatPercentage);
                totalAmountAfterTaxes = Add(totalAmount, taxAmount);
            }

            var billingInvoiceDetail = new BillingInvoiceDetail()
            {
                TotalAmount = totalAmount,
                Currency = financialAccountData.CurrencyId,
                TotalRecurringCharges = recurringCharges,
                TotalActivationCharges = totalActivationCharges,
                TotalSuspensionCharges = totalSuspensionCharges,
                VatPercentage = vatPercentage,
                TaxAmount = taxAmount,
                TotalAmountAfterTaxes = totalAmountAfterTaxes
            };

            context.Invoice = new GeneratedInvoice
            {
                InvoiceItemSets = generatedInvoiceItemSets,
                InvoiceDetails = billingInvoiceDetail
            };

            SetInvoiceBillingTransactions(context, billingInvoiceDetail);
        }
        #endregion

        #region Private Methods
        private void SetInvoiceBillingTransactions(IInvoiceGenerationContext context, BillingInvoiceDetail billingInvoiceDetail)
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
                TransactionTypeId = this._invoiceTransactionTypeId.Value,
                Amount = billingInvoiceDetail.TotalAmount,
                CurrencyId = billingInvoiceDetail.Currency
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

        private List<GeneratedInvoiceItemSet> BuildGeneratedInvoiceItemSet(string accountId, DateTime fromDate, DateTime toDate, DateTime issueDate, int currencyId, out decimal totalAmount, out decimal totalRecurringCharges, out decimal totalActivationCharges, out decimal totalSuspensionCharges)
        {
            List<GeneratedInvoiceItemSet> generatedInvoiceItemSets = new List<GeneratedInvoiceItemSet>();
            List<BillingInvoiceItem> billingInvoiceItems = new List<BillingInvoiceItem>();

            totalAmount = 0;
            totalRecurringCharges = 0;
            totalActivationCharges = 0;
            totalSuspensionCharges = 0;

            List<object> ids = new List<object>() { accountId };

            var objectListRecordFilter = new ObjectListRecordFilter()
            {
                FieldName = "EffectiveBillingAccount",
                Values = ids,
                CompareOperator = ListRecordFilterOperator.In
            };

            var recordFilterGroup = new RecordFilterGroup()
            {
                Filters = new List<RecordFilter>() { objectListRecordFilter }
            };

            var billingContractServices = _billingContractServiceManager.GetBillingContractServices(recordFilterGroup);

            var billingContractServicesByContractServiceRatePlanCurrency = new Dictionary<ContractServiceRatePlanCurrency, BillingInvoiceItem>();

            if (billingContractServices.Any())
            {
                foreach (var bcsItem in billingContractServices)
                {
                    ContractServiceRatePlanCurrency contractServiceRatePlanCurrency = new ContractServiceRatePlanCurrency
                    {
                        ContractID = bcsItem.ContractID,
                        CurrencyId = currencyId,
                        RatePlanId = bcsItem.RatePlanId,
                        ServiceID = bcsItem.ServiceID
                    };
                    var billingRatePlanService = _billingRatePlanServiceManager.GetBillingRatePlanServiceByRatePlanAndService(bcsItem.RatePlanId, bcsItem.ServiceID);

                    var billingInvoiceItem = billingContractServicesByContractServiceRatePlanCurrency.GetOrCreateItem(contractServiceRatePlanCurrency, () => new BillingInvoiceItem()
                    {
                        ContractID = contractServiceRatePlanCurrency.ContractID,
                        ServiceID = contractServiceRatePlanCurrency.ServiceID,
                        RatePlanId = contractServiceRatePlanCurrency.RatePlanId,
                        CurrencyId = contractServiceRatePlanCurrency.CurrencyId
                    });

                    billingInvoiceItem = GroupBillingInvoiceItem(billingInvoiceItem, billingRatePlanService, fromDate, toDate, bcsItem.ActivationDate, bcsItem.SuspensionDate);
                }
            }

            GeneratedInvoiceItemSet billingInvoiceItemSet = new GeneratedInvoiceItemSet()
            {
                SetName = "BillingInvoiceItemSet",
                Items = new List<GeneratedInvoiceItem>()
            };

            if (billingContractServicesByContractServiceRatePlanCurrency.Count > 0)
            {
                foreach (var item in billingContractServicesByContractServiceRatePlanCurrency)
                {
                    if (item.Value.ActivationFee.HasValue)
                    {
                        var activationfee = _currencyExchangeRateManager.ConvertValueToCurrency(item.Value.ActivationFee.Value, item.Value.CurrencyId, currencyId, issueDate);
                        totalActivationCharges = Add(totalActivationCharges, activationfee);
                    }

                    if (item.Value.RecurringFee.HasValue)
                    {
                        var recurringFee = _currencyExchangeRateManager.ConvertValueToCurrency(item.Value.RecurringFee.Value, item.Value.CurrencyId, currencyId, issueDate);
                        totalRecurringCharges = Add(totalRecurringCharges, recurringFee);
                    }

                    if (item.Value.SuspensionCharge.HasValue)
                    {
                        var suspensionCharge = _currencyExchangeRateManager.ConvertValueToCurrency(item.Value.SuspensionCharge.Value, item.Value.CurrencyId, currencyId, issueDate);
                        totalSuspensionCharges = Add(totalSuspensionCharges, suspensionCharge);
                    }

                    //if (item.Value.SuspensionRecurringCharge.HasValue)
                    //{
                    //    var suspensionRecurringCharge = _currencyExchangeRateManager.ConvertValueToCurrency(item.Value.SuspensionRecurringCharge.Value, item.Value.CurrencyId, currencyId, issueDate);
                    //    totalSuspensionCharges = Add(totalSuspensionCharges, suspensionRecurringCharge);
                    //}

                    if (item.Value.TotalAmount.HasValue)
                    {
                        var totalamount = _currencyExchangeRateManager.ConvertValueToCurrency(item.Value.TotalAmount.Value, item.Value.CurrencyId, currencyId, issueDate);
                        totalAmount = Add(totalAmount, totalamount);
                    }

                    billingInvoiceItemSet.Items.Add(new GeneratedInvoiceItem
                    {
                        Details = item.Value,
                        Name = " "
                    });
                }
            }

            generatedInvoiceItemSets.Add(billingInvoiceItemSet);

            return generatedInvoiceItemSets;
        }

        private BillingInvoiceItem GroupBillingInvoiceItem(BillingInvoiceItem bcs, BillingRatePlanService rps, DateTime fromDate, DateTime toDate, DateTime? activationDate, DateTime? suspensionDate)
        {
            var context = new RetailBEChargeCalculateChargeContext();

            if (IsDateIncluded(fromDate, toDate, activationDate))
            {
                var activationFee = rps.ActivationFee != null && rps.ActivationFee.Settings != null ? rps.ActivationFee.Settings.CalculateCharge(context) : default(decimal?);
                bcs.ActivationFee = Add(bcs.ActivationFee, activationFee);
                bcs.TotalAmount = Add(bcs.TotalAmount, activationFee);
            }

            var recurringFee = rps.RecurringFee != null && rps.RecurringFee.Settings != null ? rps.RecurringFee.Settings.CalculateCharge(context) : default(decimal?);
            bcs.RecurringFee = Add(bcs.RecurringFee, recurringFee);
            bcs.TotalAmount = Add(bcs.TotalAmount, recurringFee);

            if (IsDateIncluded(fromDate, toDate, suspensionDate))
            {
                var suspensionCharge = rps.SuspensionCharge != null && rps.SuspensionCharge.Settings != null ? rps.SuspensionCharge.Settings.CalculateCharge(context) : default(decimal?);
                bcs.SuspensionCharge = Add(bcs.SuspensionCharge, suspensionCharge);
                bcs.TotalAmount = Add(bcs.TotalAmount, suspensionCharge);

                //var suspensionRecurringCharge = rps.SuspensionRecurringCharge != null && rps.SuspensionRecurringCharge.Settings != null ? rps.SuspensionRecurringCharge.Settings.CalculateCharge(context) : default(decimal?);
                //bcs.SuspensionRecurringCharge = Add(bcs.SuspensionRecurringCharge, suspensionRecurringCharge);
                //bcs.TotalAmount = Add(bcs.SuspensionRecurringCharge, suspensionRecurringCharge);
            }

            return bcs;
        }

        private decimal? Add(decimal? firstElement, decimal? secondElement)
        {
            if (firstElement.HasValue)
            {
                return Add(firstElement.Value, secondElement);
            }
            else
            {
                return secondElement;
            }
        }

        private decimal Add(decimal firstElement, decimal? secondElement)
        {
            if (secondElement.HasValue)
            {
                return firstElement + secondElement.Value;
            }
            else
            {
                return firstElement;
            }
        }

        private bool IsDateIncluded(DateTime from, DateTime to, DateTime? date)
        {
            if (date.HasValue)
            {
                return from <= date.Value && to >= date.Value;
            }
            else
            {
                return false;
            }
        }

        private Decimal GetTaxAmount(string accountId, decimal amount, int currencyId, DateTime issueDate, out decimal percentage)
        {
            var financialAccount = new GenericFinancialAccountManager(_financialAccountConfiguration).GetFinancialAccount(accountId);

            GenericRuleTarget ruleTarget = new GenericRuleTarget
            {
                Objects = new Dictionary<string, dynamic> { { "Account", new CustomerBeManager().GetCustomerObject(financialAccount.ExtraFields.GetRecord("Customer")) } },
                EffectiveOn = issueDate
            };
            TaxRuleContext taxRuleContext = new TaxRuleContext
            {
                Amount = amount,
                CurrencyId = currencyId,
            };
            _taxRuleManager.ApplyTaxRule(taxRuleContext, this._vatRuleDefinitionId.Value, ruleTarget);
            percentage = taxRuleContext.Percentage;
            return taxRuleContext.TaxAmount;
        }

        #endregion
    }
}