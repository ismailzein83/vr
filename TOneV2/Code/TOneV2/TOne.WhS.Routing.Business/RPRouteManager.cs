using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;

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

        public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones)
        {
            return GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, null, null);
        }

        public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones, int? toCurrencyId, int? customerId)
        {
            var latestRoutingDatabase = GetLatestRoutingDatabase(routingDatabaseId);
            if (latestRoutingDatabase == null)
                return null;

            IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            dataManager.RoutingDatabase = latestRoutingDatabase;

            bool includeBlockedSupplierZones = GetIncludeBlockedSupplierZones(latestRoutingDatabase);

            int? customerProfileId = null;
            if (customerId.HasValue)
                customerProfileId = GetCarrierProfileId(customerId.Value);

            IEnumerable<RPRoute> rpRoutes = dataManager.GetRPRoutes(rpZones);
            int systemCurrencyId = GetSystemCurrencyId();
            return rpRoutes.MapRecords(x => RPRouteDetailMapper(x, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, includeBlockedSupplierZones, customerProfileId));
        }

        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId)
        {
            return GetRPRouteOptionSupplier(routingDatabaseId, routingProductId, saleZoneId, supplierId, null);
        }

        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId, int? toCurrencyId)
        {
            IRPRouteDataManager routeManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();

            var latestRoutingDatabase = GetLatestRoutingDatabase(routingDatabaseId);
            if (latestRoutingDatabase == null)
                return null;

            routeManager.RoutingDatabase = latestRoutingDatabase;
            Dictionary<int, RPRouteOptionSupplier> dicRouteOptionSuppliers = routeManager.GetRouteOptionSuppliers(routingProductId, saleZoneId);

            if (dicRouteOptionSuppliers == null || !dicRouteOptionSuppliers.ContainsKey(supplierId))
                return null;

            RPRouteOptionSupplier rpRouteOptionSupplier = dicRouteOptionSuppliers[supplierId];
            Dictionary<long, SupplierRate> supplierRateByIds = new SupplierRateManager().GetSupplierRates(rpRouteOptionSupplier.SupplierZones.Select(itm => itm.SupplierRateId).ToHashSet());

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            int? systemCurrencyId = null;
            if (toCurrencyId.HasValue)
                systemCurrencyId = GetSystemCurrencyId();


            SupplierZoneRateLocator futureSupplierZoneRateLocator = null;

            if (latestRoutingDatabase.Type != RoutingDatabaseType.Future)
            {
                List<RoutingSupplierInfo> routingSupplierInfoList = new List<RoutingSupplierInfo>() { new RoutingSupplierInfo() { SupplierId = supplierId } };
                futureSupplierZoneRateLocator = new SupplierZoneRateLocator(new SupplierRateReadAllNoCache(routingSupplierInfoList, null, true));
            }

            return new RPRouteOptionSupplierDetail()
            {
                SupplierName = carrierAccountManager.GetCarrierAccountName(supplierId),
                SupplierZones = rpRouteOptionSupplier.SupplierZones.MapRecords(x => RPRouteOptionSupplierZoneDetailMapper(futureSupplierZoneRateLocator, supplierRateByIds, x, systemCurrencyId, toCurrencyId))
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<RPRouteOptionDetail> GetFilteredRPRouteOptions(Vanrise.Entities.DataRetrievalInput<RPRouteOptionQuery> input)
        {
            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();

            var latestRoutingDatabase = GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
            if (latestRoutingDatabase == null)
                return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, new BigResult<RPRouteOptionDetail>());

            manager.RoutingDatabase = latestRoutingDatabase;

            Dictionary<Guid, IEnumerable<RPRouteOption>> allOptions = manager.GetRouteOptions(input.Query.RoutingProductId, input.Query.SaleZoneId);
            if (allOptions == null || !allOptions.ContainsKey(input.Query.PolicyOptionConfigId))
                return null;

            IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[input.Query.PolicyOptionConfigId];
            int counter = 0;
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<RPRouteOptionDetail>(input, routeOptionsByPolicy.ToBigResult(input, null, x => RPRouteOptionMapper(x, null, null, counter++)));
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
                                if (customerRouteOptionDetail.Entity.Percentage != null)
                                    routeOptionsDetails = routeOptionsDetails + customerRouteOptionDetail.Entity.Percentage + "% ";
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
                IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
                var latestRoutingDatabase = _manager.GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
                dataManager.RoutingDatabase = latestRoutingDatabase;
                if (latestRoutingDatabase == null)
                    return null;
                return dataManager.GetFilteredRPRoutes(input);

            }

            protected override BigResult<RPRouteDetail> AllRecordsToBigResult(DataRetrievalInput<RPRouteQuery> input, IEnumerable<RPRoute> allRecords)
            {

                var latestRoutingDatabase = _manager.GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
                bool includeBlockedSupplierZones = _manager.GetIncludeBlockedSupplierZones(latestRoutingDatabase);
                return allRecords.ToBigResult(input, null, (entity) => _manager.RPRouteDetailMapper(entity, input.Query.PolicyConfigId, input.Query.NumberOfOptions, null, null, includeBlockedSupplierZones, null));
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

        private RPRouteDetail RPRouteDetailMapper(RPRoute rpRoute, Guid policyConfigId, int? numberOfOptions, int? systemCurrencyId, int? toCurrencyId, bool includeBlockedSupplierZones, int? customerProfileId)
        {
            return new RPRouteDetail()
            {
                RoutingProductId = rpRoute.RoutingProductId,
                SaleZoneId = rpRoute.SaleZoneId,
                SaleZoneServiceIds = rpRoute.SaleZoneServiceIds,
                RoutingProductName = _routingProductManager.GetRoutingProductName(rpRoute.RoutingProductId),
                SellingNumberPlan = GetSellingNumberPlan(rpRoute.SaleZoneId),
                SaleZoneName = rpRoute.SaleZoneName,
                IsBlocked = rpRoute.IsBlocked,
                RouteOptionsDetails = this.GetRouteOptionDetails(rpRoute.RPOptionsByPolicy, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, includeBlockedSupplierZones, customerProfileId),
                ExecutedRuleId = rpRoute.ExecutedRuleId,
                EffectiveRateValue = rpRoute.EffectiveRateValue
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

        private RPRouteOptionDetail RPRouteOptionMapper(RPRouteOption routeOption, int? systemCurrencyId, int? toCurrencyId, int optionOrder)
        {
            if (routeOption == null)
                return null;

            var routeOptionDetail = new RPRouteOptionDetail()
            {
                Entity = routeOption,
                SupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId),
                ConvertedSupplierRate = routeOption.SupplierRate,
                OptionOrder = optionOrder
            };

            if (toCurrencyId.HasValue)
            {
                if (!systemCurrencyId.HasValue)
                    throw new ArgumentNullException("systemCurrencyId");
                routeOptionDetail.ConvertedSupplierRate = GetRateConvertedToCurrency(routeOption.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, DateTime.Now);
            }

            return routeOptionDetail;
        }

        private RPRouteOptionSupplierZoneDetail RPRouteOptionSupplierZoneDetailMapper(SupplierZoneRateLocator futureSupplierZoneRateLocator, Dictionary<long, SupplierRate> supplierRateByIds,
            RPRouteOptionSupplierZone rpRouteOptionSupplierZone, int? systemCurrencyId, int? toCurrencyId)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            SupplierZone supplierZone = manager.GetSupplierZone(rpRouteOptionSupplierZone.SupplierZoneId);

            SupplierRate supplierRate = supplierRateByIds != null ? supplierRateByIds.GetRecord(rpRouteOptionSupplierZone.SupplierRateId) : null;
            DateTime now = DateTime.Now;

            bool shouldGetFutureRate = supplierRate != null && supplierRate.EED.HasValue && futureSupplierZoneRateLocator != null;

            var detailEntity = new RPRouteOptionSupplierZoneDetail()
            {
                Entity = rpRouteOptionSupplierZone,
                SupplierZoneName = supplierZone != null ? supplierZone.Name : null,
                ConvertedSupplierRate = rpRouteOptionSupplierZone.SupplierRate,
                FutureRate = shouldGetFutureRate ? GetFutureRate(supplierRate, futureSupplierZoneRateLocator, supplierZone.SupplierId, supplierZone.SupplierZoneId, now) : null,
                RateEED = supplierRate != null ? supplierRate.EED : null
            };

            if (toCurrencyId.HasValue)
            {
                if (!systemCurrencyId.HasValue)
                    throw new ArgumentNullException("systemCurrencyId");

                detailEntity.ConvertedSupplierRate = GetRateConvertedToCurrency(rpRouteOptionSupplierZone.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, now);
                if (detailEntity.FutureRate.HasValue)
                    detailEntity.FutureRate = GetRateConvertedToCurrency(detailEntity.FutureRate.Value, systemCurrencyId.Value, toCurrencyId.Value, now);
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

        private IEnumerable<RPRouteOptionDetail> GetRouteOptionDetails(Dictionary<Guid, IEnumerable<RPRouteOption>> dicRouteOptions, Guid policyConfigId, int? numberOfOptions, int? systemCurrencyId, int? toCurrencyId, bool includeBlockedSupplierZones, int? customerProfileId)
        {
            if (dicRouteOptions == null || !dicRouteOptions.ContainsKey(policyConfigId))
                return null;

            IEnumerable<RPRouteOption> rpRouteOptions;
            dicRouteOptions.TryGetValue(policyConfigId, out rpRouteOptions);

            if (rpRouteOptions == null)
                return null;

            Func<RPRouteOption, bool> filterFunc = (rpRouteOption) =>
            {
                if (!includeBlockedSupplierZones && rpRouteOption.SupplierStatus == SupplierStatus.Block)
                    return false;
                if (customerProfileId.HasValue && GetCarrierProfileId(rpRouteOption.SupplierId) == customerProfileId.Value)
                    return false;
                return true;
            };

            int counter = 0;

            IEnumerable<RPRouteOption> routOptions = rpRouteOptions.FindAllRecords(x => filterFunc(x));

            if (numberOfOptions.HasValue)
                routOptions = routOptions.Take(numberOfOptions.Value);

            return routOptions.MapRecords(x => RPRouteOptionMapper(x, systemCurrencyId, toCurrencyId, counter++));
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

        private bool GetIncludeBlockedSupplierZones(RoutingDatabase routingDatabase)
        {
            if (routingDatabase.Information == null)
                throw new NullReferenceException("routingDatabase.Information");

            RPRoutingDatabaseInformation rpRoutingDatabaseInformation = routingDatabase.Information as RPRoutingDatabaseInformation;

            return rpRoutingDatabaseInformation.IncludeBlockedSupplierZones;
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
