using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RPRouteManager
    {
        #region Fields

        CarrierAccountManager _carrierAccountManager;
        RoutingProductManager _routingProductManager;
        CurrencyExchangeRateManager _currencyExchangeRateManager;
        SaleZoneManager _saleZoneManager;
        SellingNumberPlanManager _sellingNumberPlanManager;

        #endregion

        #region Constructors

        public RPRouteManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _routingProductManager = new RoutingProductManager();
            _currencyExchangeRateManager = new CurrencyExchangeRateManager();
            _saleZoneManager = new SaleZoneManager();
            _sellingNumberPlanManager = new SellingNumberPlanManager();
        }

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<RPRouteDetail> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<RPRouteQuery> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RPRouteRequestHandler());
        }

        public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones, bool includeBlockedSuppliers)
        {
            return GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, null, null, includeBlockedSuppliers);
        }

        public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones, int? toCurrencyId, int? customerId, bool includeBlockedSuppliers)
        {
            var latestRoutingDatabase = GetLatestRoutingDatabase(routingDatabaseId);
            if (latestRoutingDatabase == null)
                return null;

            int? customerProfileId = null;
            if (customerId.HasValue)
                customerProfileId = GetCarrierProfileId(customerId.Value);

            IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            dataManager.RoutingDatabase = latestRoutingDatabase;
            IEnumerable<RPRoute> rpRoutes = dataManager.GetRPRoutes(rpZones);

            int systemCurrencyId = GetSystemCurrencyId();
            DateTime effectiveDate = DateTime.Now;
            return rpRoutes.MapRecords(x => RPRouteDetailMapper(x, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, customerProfileId, effectiveDate, includeBlockedSuppliers));
        }

        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId, int? toCurrencyId)
        {
            var latestRoutingDatabase = GetLatestRoutingDatabase(routingDatabaseId);
            if (latestRoutingDatabase == null)
                return null;

            IRPRouteDataManager routeManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            routeManager.RoutingDatabase = latestRoutingDatabase;
            Dictionary<int, RPRouteOptionSupplier> dicRouteOptionSuppliers = routeManager.GetRouteOptionSuppliers(routingProductId, saleZoneId);

            if (dicRouteOptionSuppliers == null || !dicRouteOptionSuppliers.ContainsKey(supplierId))
                return null;

            RPRouteOptionSupplier rpRouteOptionSupplier = dicRouteOptionSuppliers[supplierId];
            Dictionary<long, SupplierRate> supplierRateByIds = new SupplierRateManager().GetSupplierRates(rpRouteOptionSupplier.SupplierZones.Select(itm => itm.SupplierRateId).ToHashSet());

            int? systemCurrencyId = null;
            if (toCurrencyId.HasValue)
                systemCurrencyId = GetSystemCurrencyId();

            SupplierZoneRateLocator futureSupplierZoneRateLocator = null;

            if (latestRoutingDatabase.Type != RoutingDatabaseType.Future)
            {
                List<RoutingSupplierInfo> routingSupplierInfoList = new List<RoutingSupplierInfo>() { new RoutingSupplierInfo() { SupplierId = supplierId } };
                futureSupplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadAllNoCache(routingSupplierInfoList, null, true));
            }

            DateTime effectiveDate = DateTime.Now;
            return new RPRouteOptionSupplierDetail()
            {
                SupplierName = new CarrierAccountManager().GetCarrierAccountName(supplierId),
                SupplierZones = rpRouteOptionSupplier.SupplierZones.MapRecords(x => RPRouteOptionSupplierZoneDetailMapper(futureSupplierZoneRateLocator, supplierRateByIds, x, systemCurrencyId, toCurrencyId, effectiveDate))
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<RPRouteOptionDetail> GetFilteredRPRouteOptions(Vanrise.Entities.DataRetrievalInput<RPRouteOptionQuery> input)
        {
            var latestRoutingDatabase = GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
            if (latestRoutingDatabase == null)
                return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, new BigResult<RPRouteOptionDetail>());

            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            manager.RoutingDatabase = latestRoutingDatabase;
            Dictionary<Guid, IEnumerable<RPRouteOption>> allOptions = manager.GetRouteOptions(input.Query.RoutingProductId, input.Query.SaleZoneId);
            if (allOptions == null || !allOptions.ContainsKey(input.Query.PolicyOptionConfigId))
                return null;

            int? systemCurrencyId = null;
            int? toCurrencyId = null;

            if (!input.Query.ShowInSystemCurrency && input.Query.CustomerId.HasValue)
            {
                systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                toCurrencyId = new CarrierAccountManager().GetCarrierAccountCurrencyId(input.Query.CustomerId.Value);
            }

            IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[input.Query.PolicyOptionConfigId];

            Func<RPRouteOption, bool> filterFunc = (rpRouteOption) =>
            {
                if (input.Query != null && !input.Query.IncludeBlockedSuppliers && rpRouteOption.SupplierStatus == SupplierStatus.Block)
                    return false;

                return true;
            };

            int counter = 0;
            DateTime effectiveDate = DateTime.Now;
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<RPRouteOptionDetail>(input, routeOptionsByPolicy.ToBigResult(input, filterFunc, x => RPRouteOptionMapper(x, systemCurrencyId, toCurrencyId, counter++, effectiveDate)));
        }

        public IEnumerable<RPRouteOptionPolicySetting> GetPoliciesOptionTemplates(RPRouteOptionPolicyFilter filter)
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            IEnumerable<RPRouteOptionPolicySetting> cachedConfigs = extensionConfigManager.GetExtensionConfigurations<RPRouteOptionPolicySetting>(Constants.SupplierZoneToRPOptionConfigType);

            if (filter == null)
                return cachedConfigs.OrderBy(x => x.Title);

            Guid defaultPolicyId;
            var routingDatabase = GetLatestRoutingDatabase(filter.RoutingDatabaseId);

            if (routingDatabase == null)
                return null;

            IEnumerable<Guid> selectedPolicyIds = this.GetRoutingDatabasePolicyIds(routingDatabase.ID, out defaultPolicyId);
            Func<RPRouteOptionPolicySetting, bool> filterExpression = (itm) => selectedPolicyIds.Contains(itm.ExtensionConfigurationId);

            IEnumerable<RPRouteOptionPolicySetting> cachedFilteredConfigs = cachedConfigs.FindAllRecords(filterExpression);

            if (cachedFilteredConfigs == null || cachedFilteredConfigs.Count() == 0)
                throw new NullReferenceException("cachedFilteredConfigs");

            List<RPRouteOptionPolicySetting> filteredConfigs = new List<RPRouteOptionPolicySetting>();

            // Set the default policy
            bool isDefaultPolicySet = false;
            foreach (RPRouteOptionPolicySetting config in cachedFilteredConfigs)
            {
                RPRouteOptionPolicySetting item = new RPRouteOptionPolicySetting()
                {
                    BehaviorFQTN = config.BehaviorFQTN,
                    Editor = config.Editor,
                    ExtensionConfigurationId = config.ExtensionConfigurationId,
                    IsDefault = config.IsDefault,
                    Name = config.Name,
                    Title = config.Title
                };

                if (item.ExtensionConfigurationId == defaultPolicyId)
                {
                    item.IsDefault = true;
                    isDefaultPolicySet = true;
                }
                filteredConfigs.Add(item);
            }

            if (!isDefaultPolicySet)
                throw new DataIntegrityValidationException(String.Format("RPRoutingDatabase '{0}' does not have a default policy", filter.RoutingDatabaseId));

            return filteredConfigs.OrderBy(x => x.Title);
        }

        public HashSet<int> GetSaleZoneServices(int routingProductId, long saleZoneId)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();

            RoutingProduct routingProduct = routingProductManager.GetRoutingProduct(routingProductId);

            if (routingProduct == null)
                throw new NullReferenceException(string.Format("routingProduct of Routing Product Id: {0}", routingProductId));

            if (routingProduct.Settings == null)
                throw new NullReferenceException(string.Format("routingProduct.Settings of Routing Product Id: {0}", routingProductId));

            return routingProduct.Settings.GetZoneServices(saleZoneId);
        }

        #endregion

        #region Private Members

        private class RPRouteExcelExportHandler : ExcelExportHandler<RPRouteDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RPRouteDetail> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Product Cost",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Routing Product", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Number Plan", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zone", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Blocked" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Route Options", Width = 125 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        sheet.Rows.Add(row);
                        row.Cells.Add(new ExportExcelCell { Value = record.RoutingProductName });
                        row.Cells.Add(new ExportExcelCell { Value = record.SellingNumberPlan });
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneServiceIds == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.SaleZoneServiceIds.ToList()) });
                        row.Cells.Add(new ExportExcelCell { Value = record.IsBlocked });
                        if (record.RouteOptionsDetails != null)
                        {
                            string routeOptionsDetails = "";
                            foreach (var customerRouteOptionDetail in record.RouteOptionsDetails)
                            {
                                routeOptionsDetails = routeOptionsDetails + customerRouteOptionDetail.SupplierName + " ";
                                if (customerRouteOptionDetail.Percentage != null)
                                    routeOptionsDetails = routeOptionsDetails + customerRouteOptionDetail.Percentage + "% ";
                            }
                            row.Cells.Add(new ExportExcelCell { Value = routeOptionsDetails });
                        }
                        else
                            row.Cells.Add(new ExportExcelCell { Value = "" });
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class RPRouteRequestHandler : BigDataRequestHandler<RPRouteQuery, RPRoute, RPRouteDetail>
        {
            RPRouteManager _manager = new RPRouteManager();

            public override RPRouteDetail EntityDetailMapper(RPRoute entity)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<RPRoute> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RPRouteQuery> input)
            {
                var latestRoutingDatabase = _manager.GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
                if (latestRoutingDatabase == null)
                    return null;

                IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
                dataManager.RoutingDatabase = latestRoutingDatabase;
                return dataManager.GetFilteredRPRoutes(input);
            }

            protected override BigResult<RPRouteDetail> AllRecordsToBigResult(DataRetrievalInput<RPRouteQuery> input, IEnumerable<RPRoute> allRecords)
            {
                int? systemCurrencyId = null;
                int? toCurrencyId = null;

                if (!input.Query.ShowInSystemCurrency && input.Query.CustomerId.HasValue)
                {
                    systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                    toCurrencyId = new CarrierAccountManager().GetCarrierAccountCurrencyId(input.Query.CustomerId.Value);
                }

                DateTime effectiveDate = DateTime.Now;

                return allRecords.ToBigResult(input, null, (entity) => _manager.RPRouteDetailMapper(entity, input.Query.PolicyConfigId, input.Query.NumberOfOptions, systemCurrencyId,
                    toCurrencyId, null, effectiveDate, input.Query.IncludeBlockedSuppliers));
            }

            protected override ResultProcessingHandler<RPRouteDetail> GetResultProcessingHandler(DataRetrievalInput<RPRouteQuery> input, BigResult<RPRouteDetail> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<RPRouteDetail>()
                {
                    ExportExcelHandler = new RPRouteExcelExportHandler()
                };
                return resultProcessingHandler;
            }
        }

        private RoutingDatabase GetLatestRoutingDatabase(int routingDatabaseId)
        {
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();

            var routingDatabase = routingDatabaseManager.GetRoutingDatabase(routingDatabaseId);

            if (routingDatabase == null)//in case of deleted database
                routingDatabase = routingDatabaseManager.GetRoutingDatabaseFromDB(routingDatabaseId);

            if (routingDatabase == null)
                throw new NullReferenceException(string.Format("routingDatabase. RoutingDatabaseId: {0}", routingDatabaseId));

            return routingDatabaseManager.GetLatestRoutingDatabase(routingDatabase.ProcessType, routingDatabase.Type);
        }

        private RPRouteDetail RPRouteDetailMapper(RPRoute rpRoute, Guid policyConfigId, int? numberOfOptions, int? systemCurrencyId, int? toCurrencyId, int? customerProfileId,
            DateTime effectiveDate, bool includeBlockedSuppliers)
        {
            CurrencyManager currencyManager = new CurrencyManager();
            string currencySymbol;
            decimal? effectiveRateValue = rpRoute.EffectiveRateValue;

            if (toCurrencyId.HasValue)
            {
                currencySymbol = currencyManager.GetCurrencySymbol(toCurrencyId.Value);
                if (effectiveRateValue.HasValue)
                {
                    if (!systemCurrencyId.HasValue)
                        throw new ArgumentNullException("systemCurrencyId");

                    effectiveRateValue = GetRateConvertedToCurrency(effectiveRateValue.Value, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate);
                }
            }
            else
            {
                currencySymbol = currencyManager.GetSystemCurrency().Symbol;
            }

            return new RPRouteDetail()
            {
                RoutingProductId = rpRoute.RoutingProductId,
                SaleZoneId = rpRoute.SaleZoneId,
                SaleZoneServiceIds = rpRoute.SaleZoneServiceIds,
                RoutingProductName = _routingProductManager.GetRoutingProductName(rpRoute.RoutingProductId),
                SellingNumberPlan = GetSellingNumberPlan(rpRoute.SaleZoneId),
                SaleZoneName = rpRoute.SaleZoneName,
                IsBlocked = rpRoute.IsBlocked,
                RouteOptionsDetails = this.GetRouteOptionDetails(rpRoute.RPOptionsByPolicy, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, customerProfileId, includeBlockedSuppliers),
                ExecutedRuleId = rpRoute.ExecutedRuleId,
                EffectiveRateValue = effectiveRateValue,
                CurrencySymbol = currencySymbol
            };
        }

        private string GetSellingNumberPlan(long saleZoneId)
        {
            SaleZone saleZone = _saleZoneManager.GetSaleZone(saleZoneId);
            if (saleZone == null)
                return null;

            SellingNumberPlan sellingNumberPlan = _sellingNumberPlanManager.GetSellingNumberPlan(saleZone.SellingNumberPlanId);
            if (sellingNumberPlan == null)
                return null;

            return sellingNumberPlan.Name;
        }

        private RPRouteOptionDetail RPRouteOptionMapper(RPRouteOption routeOption, int? systemCurrencyId, int? toCurrencyId, int optionOrder, DateTime effectiveDate)
        {
            if (routeOption == null)
                return null;

            CurrencyManager currencyManager = new CurrencyManager();
            string currencySymbol;
            decimal convertedSupplierRate;

            if (toCurrencyId.HasValue)
            {
                if (!systemCurrencyId.HasValue)
                    throw new ArgumentNullException("systemCurrencyId");

                currencySymbol = currencyManager.GetCurrencySymbol(toCurrencyId.Value);
                convertedSupplierRate = GetRateConvertedToCurrency(routeOption.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate);
            }
            else
            {
                currencySymbol = currencyManager.GetSystemCurrency().Symbol;
                convertedSupplierRate = routeOption.SupplierRate;
            }

            var routeOptionDetail = new RPRouteOptionDetail()
           {
               SupplierId = routeOption.SupplierId,
               SaleZoneId = routeOption.SaleZoneId,
               SupplierRate = routeOption.SupplierRate,
               Percentage = routeOption.Percentage,
               SupplierStatus = routeOption.SupplierStatus,
               SupplierZoneMatchHasClosedRate = routeOption.SupplierZoneMatchHasClosedRate,
               SupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId),
               ConvertedSupplierRate = convertedSupplierRate,
               OptionOrder = optionOrder,
               CurrencySymbol = currencySymbol
           };
            return routeOptionDetail;
        }

        private RPRouteOptionSupplierZoneDetail RPRouteOptionSupplierZoneDetailMapper(SupplierZoneRateLocator futureSupplierZoneRateLocator, Dictionary<long, SupplierRate> supplierRateByIds,
            RPRouteOptionSupplierZone rpRouteOptionSupplierZone, int? systemCurrencyId, int? toCurrencyId, DateTime effectiveDate)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            SupplierZone supplierZone = manager.GetSupplierZone(rpRouteOptionSupplierZone.SupplierZoneId);

            SupplierRate supplierRate = supplierRateByIds != null ? supplierRateByIds.GetRecord(rpRouteOptionSupplierZone.SupplierRateId) : null;

            bool shouldGetFutureRate = supplierRate != null && supplierRate.EED.HasValue && futureSupplierZoneRateLocator != null;

            var detailEntity = new RPRouteOptionSupplierZoneDetail()
            {
                Entity = rpRouteOptionSupplierZone,
                SupplierZoneName = supplierZone != null ? supplierZone.Name : null,
                ConvertedSupplierRate = rpRouteOptionSupplierZone.SupplierRate,
                FutureRate = shouldGetFutureRate ? GetFutureRate(supplierRate, futureSupplierZoneRateLocator, supplierZone.SupplierId, supplierZone.SupplierZoneId, effectiveDate) : null,
                RateEED = supplierRate != null ? supplierRate.EED : null
            };

            if (toCurrencyId.HasValue)
            {
                if (!systemCurrencyId.HasValue)
                    throw new ArgumentNullException("systemCurrencyId");

                detailEntity.ConvertedSupplierRate = GetRateConvertedToCurrency(rpRouteOptionSupplierZone.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate);
                if (detailEntity.FutureRate.HasValue)
                    detailEntity.FutureRate = GetRateConvertedToCurrency(detailEntity.FutureRate.Value, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate);
            }

            return detailEntity;
        }

        private decimal? GetFutureRate(SupplierRate supplierRate, SupplierZoneRateLocator futureSupplierZoneRateLocator, int supplierId, long supplierZoneId, DateTime effectiveDate)
        {
            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            int systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();

            SupplierZoneRate futureSupplierZoneRate = futureSupplierZoneRateLocator.GetSupplierZoneRate(supplierId, supplierZoneId, null);
            if (futureSupplierZoneRate == null)
                return null;

            SupplierRateManager supplierRateManager = new SupplierRateManager();

            decimal? normalRate = default(decimal?);
            if (futureSupplierZoneRate.Rate != null)
            {
                int normalRateCurrencyId = supplierRateManager.GetCurrencyId(futureSupplierZoneRate.Rate);
                normalRate = currencyExchangeRateManager.ConvertValueToCurrency(futureSupplierZoneRate.Rate.Rate, normalRateCurrencyId, systemCurrencyId, effectiveDate);
            }

            if (!supplierRate.RateTypeId.HasValue)
                return normalRate;

            if (futureSupplierZoneRate.RatesByRateType == null)
                return null;

            SupplierRate supplierRateByRateType = futureSupplierZoneRate.RatesByRateType.GetRecord(supplierRate.RateTypeId.Value);
            if (supplierRateByRateType == null)
                return null;

            int otherRateCurrencyId = supplierRateManager.GetCurrencyId(supplierRateByRateType);
            return currencyExchangeRateManager.ConvertValueToCurrency(supplierRateByRateType.Rate, otherRateCurrencyId, systemCurrencyId, effectiveDate);
        }

        private IEnumerable<RPRouteOptionDetail> GetRouteOptionDetails(Dictionary<Guid, IEnumerable<RPRouteOption>> dicRouteOptions, Guid policyConfigId, int? numberOfOptions, int? systemCurrencyId,
            int? toCurrencyId, int? customerProfileId, bool includeBlockedSuppliers)
        {
            if (dicRouteOptions == null || !dicRouteOptions.ContainsKey(policyConfigId))
                return null;

            IEnumerable<RPRouteOption> rpRouteOptions;
            dicRouteOptions.TryGetValue(policyConfigId, out rpRouteOptions);

            if (rpRouteOptions == null)
                return null;

            Func<RPRouteOption, bool> filterFunc = (rpRouteOption) =>
            {
                if (!includeBlockedSuppliers && rpRouteOption.SupplierStatus == SupplierStatus.Block)
                    return false;

                if (customerProfileId.HasValue && GetCarrierProfileId(rpRouteOption.SupplierId) == customerProfileId.Value)
                    return false;

                return true;
            };
            IEnumerable<RPRouteOption> filteredRPRouteOptions = rpRouteOptions.FindAllRecords(x => filterFunc(x));

            if (numberOfOptions.HasValue)
            {
                if (!includeBlockedSuppliers)
                {
                    filteredRPRouteOptions = filteredRPRouteOptions.Take(numberOfOptions.Value);
                }
                else
                {
                    int activeOptionsCounter = 0;
                    List<RPRouteOption> finalRoutOptions = new List<RPRouteOption>();

                    foreach (var routeOption in filteredRPRouteOptions)
                    {
                        if (activeOptionsCounter >= numberOfOptions.Value)
                            break;

                        if (routeOption.SupplierStatus != SupplierStatus.Block)
                            activeOptionsCounter++;

                        finalRoutOptions.Add(routeOption);
                    }

                    filteredRPRouteOptions = finalRoutOptions;
                }
            }

            int counter = 0;
            DateTime effectiveDate = DateTime.Now;
            return filteredRPRouteOptions.MapRecords(item => RPRouteOptionMapper(item, systemCurrencyId, toCurrencyId, counter++, effectiveDate));
        }

        private int GetCarrierProfileId(int carrierAccountId)
        {
            int? carrierProfileId = _carrierAccountManager.GetCarrierProfileId(carrierAccountId);
            if (!carrierProfileId.HasValue)
                throw new NullReferenceException(String.Format("CarrierAccount '{0}' was not found", carrierAccountId));
            return carrierProfileId.Value;
        }

        private IEnumerable<Guid> GetRoutingDatabasePolicyIds(int routingDbId, out Guid defaultPolicyId)
        {
            var routingDbManager = new RoutingDatabaseManager();
            var routingDbInfoFilter = new RoutingDatabaseInfoFilter() { ProcessType = RoutingProcessType.RoutingProductRoute };
            IEnumerable<RoutingDatabaseInfo> routingDbInfoEntities = routingDbManager.GetRoutingDatabaseInfo(routingDbInfoFilter);

            RoutingDatabaseInfo routingDbInfo = routingDbInfoEntities.FindRecord(itm => itm.RoutingDatabaseId == routingDbId);
            if (routingDbInfo == null)
                throw new NullReferenceException("routingDbInfo");
            if (routingDbInfo.Information == null)
                throw new NullReferenceException("routingDbInfo.Information");

            var rpRoutingDbInfo = routingDbInfo.Information as RPRoutingDatabaseInformation;
            if (rpRoutingDbInfo == null)
                throw new NullReferenceException("rpRoutingDbInfo");
            if (rpRoutingDbInfo.SelectedPoliciesIds == null || rpRoutingDbInfo.SelectedPoliciesIds.Count() == 0)
                throw new NullReferenceException("rpRoutingDbInfo.SelectedPoliciesIds");

            defaultPolicyId = rpRoutingDbInfo.DefaultPolicyId;
            return rpRoutingDbInfo.SelectedPoliciesIds;
        }

        private int GetSystemCurrencyId()
        {
            var currencyManager = new CurrencyManager();
            Currency systemCurrency = currencyManager.GetSystemCurrency();
            if (systemCurrency == null)
                throw new NullReferenceException("systemCurrency");
            return systemCurrency.CurrencyId;
        }

        private decimal GetRateConvertedToCurrency(decimal rate, int systemCurrencyId, int toCurrencyId, DateTime effectiveOn)
        {
            return _currencyExchangeRateManager.ConvertValueToCurrency(rate, systemCurrencyId, toCurrencyId, effectiveOn);
        }

        #endregion
    }
}