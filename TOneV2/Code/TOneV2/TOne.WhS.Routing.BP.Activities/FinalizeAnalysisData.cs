using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class FinalizeAnalysisData : BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime?> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabaseType> RoutingDatabaseType { get; set; }

        [RequiredArgument]
        public InArgument<List<Guid>> RiskyMarginCategories { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            DateTime? effectiveDate = this.EffectiveDate.Get(context.ActivityContext);
            RoutingDatabaseType routingDatabaseType = this.RoutingDatabaseType.Get(context.ActivityContext);
            List<Guid> riskyMarginCategories = this.RiskyMarginCategories.Get(context.ActivityContext);
            bool isFuture = this.IsFuture.Get(context.ActivityContext);

            Action<string> trackStep = (trackingMsg) => { context.ActivityContext.WriteTrackingMessage(LogEntryType.Information, trackingMsg, null); };

            trackStep("Finalizing Analysis Data has started");

            CustomerRouteMarginStagingManager customerRouteMarginStagingManager = new CustomerRouteMarginStagingManager();
            List<CustomerRouteMarginStaging> customerRouteMarginStagingList = customerRouteMarginStagingManager.GetCustomerRouteMarginStagingList(routingDatabaseType);
            if (customerRouteMarginStagingList == null || customerRouteMarginStagingList.Count == 0)
                return;

            Dictionary<CustomerRouteMarginIdentifier, CustomerRouteMargin> customerRouteMarginDict;
            List<CustomerRouteMarginSummary> customerRouteMarginSummaryList;
            Dictionary<long, HashSet<string>> riskyCodesByCustomerRouteMarginID;
            BuildCustomerRouteMargins(customerRouteMarginStagingList, riskyMarginCategories, effectiveDate, isFuture, out customerRouteMarginDict, out customerRouteMarginSummaryList,
                out riskyCodesByCustomerRouteMarginID);

            CustomerRouteMarginManager customerRouteMarginManager = new CustomerRouteMarginManager();
            customerRouteMarginManager.CreateCustomerRouteMarginTempTable(routingDatabaseType, trackStep);
            customerRouteMarginManager.InsertCustomerRouteMarginsToDB(routingDatabaseType, customerRouteMarginDict.Values.ToList(), trackStep);
            customerRouteMarginManager.CreateIndexes(routingDatabaseType, trackStep);
            customerRouteMarginManager.SwapTables(routingDatabaseType);

            CustomerRouteMarginSummaryManager customerRouteMarginSummaryManager = new CustomerRouteMarginSummaryManager();
            customerRouteMarginSummaryManager.CreateCustomerRouteMarginSummaryTempTable(routingDatabaseType, trackStep);
            customerRouteMarginSummaryManager.InsertCustomerRouteMarginSummariesToDB(routingDatabaseType, customerRouteMarginSummaryList, trackStep);
            customerRouteMarginSummaryManager.CreateIndexes(routingDatabaseType, trackStep);
            customerRouteMarginSummaryManager.SwapTables(routingDatabaseType);

            RiskyMarginCodeManager riskyMarginCodeManager = new RiskyMarginCodeManager();
            riskyMarginCodeManager.CreateRiskyMarginCodeTempTable(routingDatabaseType, trackStep);

            if (riskyCodesByCustomerRouteMarginID != null && riskyCodesByCustomerRouteMarginID.Count > 0)
            {
                List<RiskyMarginCode> riskyMarginCodes = GetRiskyMarginCodes(riskyCodesByCustomerRouteMarginID);
                riskyMarginCodeManager.InsertRiskyMarginCodesToDB(routingDatabaseType, riskyMarginCodes, trackStep);
            }

            riskyMarginCodeManager.CreateIndexes(routingDatabaseType, trackStep);
            riskyMarginCodeManager.SwapTables(routingDatabaseType);

            customerRouteMarginStagingManager.DropTable(routingDatabaseType);

            trackStep("Finalizing Analysis Data is done");
        }

        public void BuildCustomerRouteMargins(List<CustomerRouteMarginStaging> customerRouteMarginStagingList, List<Guid> riskyMarginCategories, DateTime? effectiveDate, bool isFuture,
            out Dictionary<CustomerRouteMarginIdentifier, CustomerRouteMargin> customerRouteMarginDict, out List<CustomerRouteMarginSummary> customerRouteMarginSummaryList,
            out Dictionary<long, HashSet<string>> riskyCodesByCustomerRouteMarginID)
        {
            customerRouteMarginDict = new Dictionary<CustomerRouteMarginIdentifier, CustomerRouteMargin>();
            customerRouteMarginSummaryList = new List<CustomerRouteMarginSummary>();
            riskyCodesByCustomerRouteMarginID = new Dictionary<long, HashSet<string>>();

            Guid marginRuledefinitionID = new Guid("db3f662f-e770-4026-9f34-5c5c410b0776");
            MarginRuleManager marginRuleManager = new MarginRuleManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            Currency systemCurrency = new CurrencyManager().GetSystemCurrency();

            long customerRouteMarginID = 1;

            foreach (var customerRouteMarginStaging in customerRouteMarginStagingList)
            {
                decimal saleRate = customerRouteMarginStaging.SaleRate;
                decimal supplierRate = customerRouteMarginStaging.CustomerRouteOptionMarginStaging.SupplierRate;
                decimal optimalSupplierRate = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierRate;

                decimal margin = saleRate - supplierRate;
                decimal optimalMargin = saleRate - optimalSupplierRate;

                CustomerRouteMarginIdentifier customerRouteMarginIdentifier = new CustomerRouteMarginIdentifier()
                {
                    CustomerID = customerRouteMarginStaging.CustomerID,
                    SaleZoneID = customerRouteMarginStaging.SaleZoneID,
                    SupplierZoneID = customerRouteMarginStaging.CustomerRouteOptionMarginStaging.SupplierZoneID
                };

                CustomerRouteMargin customerRouteMargin;
                if (customerRouteMarginDict.TryGetValue(customerRouteMarginIdentifier, out customerRouteMargin))
                {
                    if (customerRouteMargin.OptimalCustomerRouteOptionMargin.SupplierRate < optimalSupplierRate)
                    {
                        Guid? optimalSupplierZoneCategoryID = null;

                        MarginRuleSettings marginRuleSettings = GetMarginRuleSettings(customerRouteMarginStaging, marginRuledefinitionID, effectiveDate, isFuture,
                            marginRuleManager, carrierAccountManager, saleZoneManager);

                        if (marginRuleSettings != null)
                            optimalSupplierZoneCategoryID = GetMarginCategory(marginRuleSettings, optimalMargin, systemCurrency.CurrencyId, effectiveDate);

                        customerRouteMargin.OptimalCustomerRouteOptionMargin = new CustomerRouteOptionMargin()
                        {
                            SupplierZoneID = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierZoneID,
                            SupplierServiceIDs = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierServiceIDs,
                            SupplierRate = optimalSupplierRate,
                            SupplierDealID = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierDealID,
                            Margin = optimalMargin,
                            MarginCategoryID = optimalSupplierZoneCategoryID
                        };
                    }

                    if (customerRouteMargin.IsRisky)
                    {
                        HashSet<string> codes = riskyCodesByCustomerRouteMarginID[customerRouteMargin.CustomerRouteMarginID];
                        codes.UnionWith(customerRouteMarginStaging.Codes);
                    }
                }
                else
                {
                    Guid? supplierZoneCategoryID = null;
                    Guid? optimalSupplierZoneCategoryID = null;

                    MarginRuleSettings marginRuleSettings = this.GetMarginRuleSettings(customerRouteMarginStaging, marginRuledefinitionID, effectiveDate, isFuture,
                        marginRuleManager, carrierAccountManager, saleZoneManager);
                    
                    if (marginRuleSettings != null)
                    {
                        supplierZoneCategoryID = GetMarginCategory(marginRuleSettings, margin, systemCurrency.CurrencyId, effectiveDate);
                        optimalSupplierZoneCategoryID = GetMarginCategory(marginRuleSettings, optimalMargin, systemCurrency.CurrencyId, effectiveDate);
                    }

                    bool isRisky = supplierZoneCategoryID.HasValue && riskyMarginCategories != null && riskyMarginCategories.Contains(supplierZoneCategoryID.Value);

                    customerRouteMarginDict.Add(customerRouteMarginIdentifier, new CustomerRouteMargin()
                    {
                        CustomerRouteMarginID = customerRouteMarginID,
                        CustomerID = customerRouteMarginStaging.CustomerID,
                        SaleZoneID = customerRouteMarginStaging.SaleZoneID,
                        SaleRate = saleRate,
                        SaleDealID = customerRouteMarginStaging.SaleDealID,
                        CustomerRouteOptionMargin = new CustomerRouteOptionMargin()
                        {
                            SupplierZoneID = customerRouteMarginStaging.CustomerRouteOptionMarginStaging.SupplierZoneID,
                            SupplierServiceIDs = customerRouteMarginStaging.CustomerRouteOptionMarginStaging.SupplierServiceIDs,
                            SupplierRate = supplierRate,
                            SupplierDealID = customerRouteMarginStaging.CustomerRouteOptionMarginStaging.SupplierDealID,
                            Margin = margin,
                            MarginCategoryID = supplierZoneCategoryID
                        },
                        OptimalCustomerRouteOptionMargin = new CustomerRouteOptionMargin()
                        {
                            SupplierZoneID = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierZoneID,
                            SupplierServiceIDs = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierServiceIDs,
                            SupplierRate = optimalSupplierRate,
                            SupplierDealID = customerRouteMarginStaging.OptimalCustomerRouteOptionMarginStaging.SupplierDealID,
                            Margin = optimalMargin,
                            MarginCategoryID = optimalSupplierZoneCategoryID
                        },
                        IsRisky = isRisky
                    });

                    if (isRisky)
                        riskyCodesByCustomerRouteMarginID.Add(customerRouteMarginID, customerRouteMarginStaging.Codes);

                    customerRouteMarginID++;
                }
            }

            var customerRouteMarginSummaryStagingDict = new Dictionary<CustomerSaleZone, CustomerRouteMarginSummaryStaging>();

            foreach (var customerRouteMargin in customerRouteMarginDict.Values)
            {
                CustomerRouteOptionMargin customerRouteOptionMargin = customerRouteMargin.CustomerRouteOptionMargin;
                CustomerRouteOptionMargin optimalCustomerRouteOptionMargin = customerRouteMargin.OptimalCustomerRouteOptionMargin;

                var customerSaleZone = new CustomerSaleZone() { CustomerId = customerRouteMargin.CustomerID, SaleZoneId = customerRouteMargin.SaleZoneID };

                CustomerRouteMarginSummaryStaging customerRouteMarginSummaryStaging;
                if (customerRouteMarginSummaryStagingDict.TryGetValue(customerSaleZone, out customerRouteMarginSummaryStaging))
                {
                    customerRouteMarginSummaryStaging.NumberOfCodes++;
                    customerRouteMarginSummaryStaging.SumOfSupplierRate += customerRouteOptionMargin.SupplierRate;

                    if (customerRouteMarginSummaryStaging.MinSupplierRate > customerRouteOptionMargin.SupplierRate)
                    {
                        customerRouteMarginSummaryStaging.MinSupplierRate = customerRouteOptionMargin.SupplierRate;
                        customerRouteMarginSummaryStaging.MaxMargin = customerRouteOptionMargin.Margin;
                        customerRouteMarginSummaryStaging.MaxMarginCategoryID = customerRouteOptionMargin.MarginCategoryID;
                    }

                    if (customerRouteMarginSummaryStaging.MaxSupplierRate < customerRouteOptionMargin.SupplierRate)
                    {
                        customerRouteMarginSummaryStaging.MaxSupplierRate = customerRouteOptionMargin.SupplierRate;
                        customerRouteMarginSummaryStaging.MinMargin = customerRouteOptionMargin.Margin;
                        customerRouteMarginSummaryStaging.MinMarginCategoryID = customerRouteOptionMargin.MarginCategoryID;
                    }
                }
                else
                {
                    customerRouteMarginSummaryStaging = new CustomerRouteMarginSummaryStaging()
                    {
                        NumberOfCodes = 1,
                        CustomerID = customerRouteMargin.CustomerID,
                        SaleZoneID = customerRouteMargin.SaleZoneID,
                        SaleRate = customerRouteMargin.SaleRate,
                        SaleDealID = customerRouteMargin.SaleDealID,
                        SumOfSupplierRate = customerRouteOptionMargin.SupplierRate,
                        MinSupplierRate = customerRouteOptionMargin.SupplierRate,
                        MaxMargin = customerRouteOptionMargin.Margin,
                        MaxMarginCategoryID = customerRouteOptionMargin.MarginCategoryID,
                        MaxSupplierRate = customerRouteOptionMargin.SupplierRate,
                        MinMargin = customerRouteOptionMargin.Margin,
                        MinMarginCategoryID = customerRouteOptionMargin.MarginCategoryID
                    };

                    customerRouteMarginSummaryStagingDict.Add(customerSaleZone, customerRouteMarginSummaryStaging);
                }
            }

            long customerRouteMarginSummaryID = 1;

            foreach (var customerRouteMarginSummaryStaging in customerRouteMarginSummaryStagingDict.Values)
            {
                decimal avgSupplierRate = customerRouteMarginSummaryStaging.SumOfSupplierRate / customerRouteMarginSummaryStaging.NumberOfCodes;
                decimal avgMargin = customerRouteMarginSummaryStaging.SaleRate - avgSupplierRate;

                Guid? avgCategoryID = null;

                MarginRuleSettings marginRuleSettings = this.GetMarginRuleSettings(customerRouteMarginSummaryStaging.CustomerID, customerRouteMarginSummaryStaging.SaleZoneID,
                    marginRuledefinitionID, effectiveDate, isFuture, marginRuleManager, carrierAccountManager, saleZoneManager);

                if (marginRuleSettings != null)
                    avgCategoryID = GetMarginCategory(marginRuleSettings, avgMargin, systemCurrency.CurrencyId, effectiveDate);

                customerRouteMarginSummaryList.Add(new CustomerRouteMarginSummary()
                {
                    CustomerRouteMarginSummaryID = customerRouteMarginSummaryID,
                    CustomerID = customerRouteMarginSummaryStaging.CustomerID,
                    SaleZoneID = customerRouteMarginSummaryStaging.SaleZoneID,
                    SaleRate = customerRouteMarginSummaryStaging.SaleRate,
                    SaleDealID = customerRouteMarginSummaryStaging.SaleDealID,
                    MinSupplierRate = customerRouteMarginSummaryStaging.MinSupplierRate,
                    MaxMargin = customerRouteMarginSummaryStaging.MaxMargin,
                    MaxMarginCategoryID = customerRouteMarginSummaryStaging.MaxMarginCategoryID,
                    MaxSupplierRate = customerRouteMarginSummaryStaging.MaxSupplierRate,
                    MinMargin = customerRouteMarginSummaryStaging.MinMargin,
                    MinMarginCategoryID = customerRouteMarginSummaryStaging.MinMarginCategoryID,
                    AvgSupplierRate = avgSupplierRate,
                    AvgMargin = avgMargin,
                    AvgMarginCategoryID = avgCategoryID
                });

                customerRouteMarginSummaryID++;
            }
        }

        private MarginRuleSettings GetMarginRuleSettings(CustomerRouteMarginStaging customerRouteMarginStaging, Guid marginRuledefinitionID, DateTime? effectiveDate,
             bool isFuture, MarginRuleManager marginRuleManager, CarrierAccountManager carrierAccountManager, SaleZoneManager saleZoneManager)
        {
            return GetMarginRuleSettings(customerRouteMarginStaging.CustomerID, customerRouteMarginStaging.SaleZoneID, marginRuledefinitionID, effectiveDate, isFuture,
                marginRuleManager, carrierAccountManager, saleZoneManager);
        }

        private MarginRuleSettings GetMarginRuleSettings(int customerID, long saleZoneID, Guid marginRuledefinitionID, DateTime? effectiveDate, bool isFuture,
            MarginRuleManager marginRuleManager, CarrierAccountManager carrierAccountManager, SaleZoneManager saleZoneManager)
        {
            SaleZone saleZone = saleZoneManager.GetSaleZone(saleZoneID);
            CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(customerID);

            var marginRuleTarget = new Vanrise.GenericData.Entities.GenericRuleTarget();
            marginRuleTarget.EffectiveOn = effectiveDate;
            marginRuleTarget.IsEffectiveInFuture = isFuture;
            marginRuleTarget.TargetFieldValues = new Dictionary<string, object>();
            marginRuleTarget.TargetFieldValues.Add("CustomerId", customerID);
            marginRuleTarget.TargetFieldValues.Add("SellingProductId", carrierAccount.SellingProductId);
            marginRuleTarget.TargetFieldValues.Add("Country", saleZone.CountryId);
            marginRuleTarget.TargetFieldValues.Add("SaleZoneId", saleZoneID);

            return marginRuleManager.GetMarginRuleSettings(marginRuledefinitionID, marginRuleTarget);
        }

        private Guid? GetMarginCategory(MarginRuleSettings marginRuleSettings, decimal margin, int currencyID, DateTime? effectiveDate)
        {
            ApplyMarginRuleContext applyMarginRuleContext = new ApplyMarginRuleContext()
            {
                Margin = margin,
                MarginCurrencyId = currencyID,
                EffectiveDate = effectiveDate
            };
            marginRuleSettings.ApplyMarginRule(applyMarginRuleContext);

            return applyMarginRuleContext.MarginCategory;
        }

        private List<RiskyMarginCode> GetRiskyMarginCodes(Dictionary<long, HashSet<string>> riskyCodesByCustomerRouteMarginID)
        {
            List<RiskyMarginCode> riskyMarginCodes = new List<RiskyMarginCode>();

            foreach (var kvp in riskyCodesByCustomerRouteMarginID)
            {
                long currentCustomerRouteMarginID = kvp.Key;
                HashSet<string> riskyCodes = kvp.Value;

                foreach (var riskyCode in riskyCodes)
                    riskyMarginCodes.Add(new RiskyMarginCode() { Code = riskyCode, CustomerRouteMarginID = currentCustomerRouteMarginID });
            }

            return riskyMarginCodes;
        }
    }
}