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
        #region Fields/Ctor

        CarrierAccountManager _carrierAccountManager;
        RoutingProductManager _routingProductManager;
        CurrencyExchangeRateManager _currencyExchangeRateManager;
        SaleZoneManager _saleZoneManager;
        SellingNumberPlanManager _sellingNumberPlanManager;

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

        public Vanrise.Entities.IDataRetrievalResult<RPRouteDetailByZone> GetFilteredRPRoutesByZone(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByZone> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RPRouteByZoneRequestHandler());
        }

        public Vanrise.Entities.IDataRetrievalResult<RPRouteDetailByCode> GetFilteredRPRoutesByCode(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input)
        {
            return BigDataManager.Instance.RetrieveData(input, new RPRouteByCodeRequestHandler());
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
            int? customerProfileId = null;

            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            if (input.Query.CustomerId.HasValue)
            {
                customerProfileId = carrierAccountManager.GetCarrierProfileId(input.Query.CustomerId.Value);
                if (!input.Query.ShowInSystemCurrency)
                {
                    systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                    toCurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(input.Query.CustomerId.Value);
                }
            }



            IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[input.Query.PolicyOptionConfigId];
            if (routeOptionsByPolicy == null || !routeOptionsByPolicy.Any())
                return null;

            int counter = 0;
            DateTime effectiveDate = DateTime.Now;
            List<RPRouteOptionDetail> rpRouteOptionDetails = new List<RPRouteOptionDetail>();
            foreach (var rpRouteOption in routeOptionsByPolicy)
            {
                if (customerProfileId.HasValue)
                {
                    int? supplierProfileId = carrierAccountManager.GetCarrierProfileId(rpRouteOption.SupplierId);
                    if (supplierProfileId.HasValue && supplierProfileId.Value == customerProfileId.Value)
                        continue;
                }

                RPRouteOptionDetail rpRouteOptionDetail = RPRouteOptionMapper(rpRouteOption, systemCurrencyId, toCurrencyId, counter++, effectiveDate, input.Query.EffectiveSaleRateValue);

                if (input.Query != null && !input.Query.IncludeBlockedSuppliers && rpRouteOption.SupplierStatus == SupplierStatus.Block)
                    continue;

                if (input.Query != null && input.Query.MaxSupplierRate.HasValue && input.Query.MaxSupplierRate < rpRouteOptionDetail.ConvertedSupplierRate)
                    continue;

                rpRouteOptionDetails.Add(rpRouteOptionDetail);
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<RPRouteOptionDetail>(input, rpRouteOptionDetails.ToBigResult(input, null));
        }

        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId, int? toCurrencyId, decimal? saleRate)
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
                SupplierZones = rpRouteOptionSupplier.SupplierZones.MapRecords(x => RPRouteOptionSupplierZoneDetailMapper(futureSupplierZoneRateLocator, supplierRateByIds, x, systemCurrencyId, toCurrencyId, effectiveDate, saleRate))
            };
        }

        public IEnumerable<RPRouteDetailByZone> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones, bool includeBlockedSuppliers)
        {
            return GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, null, null, includeBlockedSuppliers);
        }

        public IEnumerable<RPRouteDetailByZone> GetRPRoutes(int routingDatabaseId, Guid policyConfigId, int? numberOfOptions, IEnumerable<RPZone> rpZones, int? toCurrencyId, int? customerId, bool includeBlockedSuppliers)
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
            return rpRoutes.MapRecords(x => RPRouteDetailMapper(x, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, customerProfileId, effectiveDate, includeBlockedSuppliers, null));
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

        #region Private Methods

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
            int? toCurrencyId, int? customerProfileId, bool includeBlockedSuppliers, decimal? effectiveRateValue, decimal? maxSupplierRate)
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
            if (filteredRPRouteOptions == null || !filteredRPRouteOptions.Any())
                return null;

            int optionOrder = 0;
            int finalOptionOrder = 0;
            int addedActiveItems = 0;
            DateTime effectiveDate = DateTime.Now;
            List<RPRouteOptionDetail> finalResult = new List<RPRouteOptionDetail>();

            foreach (RPRouteOption rpRouteOption in filteredRPRouteOptions)
            {
                if (!includeBlockedSuppliers && rpRouteOption.SupplierStatus == SupplierStatus.Block)
                    continue;

                RPRouteOptionDetail detail = RPRouteOptionMapper(rpRouteOption, systemCurrencyId, toCurrencyId, optionOrder++, effectiveDate, effectiveRateValue);
                if (maxSupplierRate.HasValue && maxSupplierRate.Value < detail.ConvertedSupplierRate)
                    continue;

                detail.OptionOrder = finalOptionOrder++;
                finalResult.Add(detail);

                if (rpRouteOption.SupplierStatus == SupplierStatus.Block)
                    continue;

                addedActiveItems++;
                if (numberOfOptions.HasValue && numberOfOptions.Value == addedActiveItems)
                    break;
            }

            return finalResult;
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

        #region Private Classes

        private class RPRouteByZoneExcelExportHandler : ExcelExportHandler<RPRouteDetailByZone>
        {
            public bool RatesIncluded { get; set; }

            int _longPrecisionValue;
            public RPRouteByZoneExcelExportHandler()
            {
                _longPrecisionValue = new GeneralSettingsManager().GetLongPrecisionValue();
            }

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RPRouteDetailByZone> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Product Cost",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Routing Product", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Number Plan", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zone", Width = 25 });
                if (RatesIncluded)
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 25 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency", Width = 25 });
                }
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Blocked" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Option 1", Width = 50 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {
                    int maxNumberOfOptions = context.BigResult.Data.Max(itm => itm.RouteOptionsDetails != null ? itm.RouteOptionsDetails.Count() : 0);

                    for (var optionNb = 2; optionNb <= maxNumberOfOptions; optionNb++)
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = string.Format("Option {0}", optionNb), Width = 50 });

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.RoutingProductName });
                        row.Cells.Add(new ExportExcelCell { Value = record.SellingNumberPlan });
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                        if (RatesIncluded)
                        {
                            row.Cells.Add(new ExportExcelCell { Value = Math.Round(record.EffectiveRateValue.Value, _longPrecisionValue) });
                            row.Cells.Add(new ExportExcelCell { Value = record.CurrencySymbol });
                        }
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneServiceIds == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.SaleZoneServiceIds.ToList()) });
                        row.Cells.Add(new ExportExcelCell { Value = record.IsBlocked });

                        if (record.RouteOptionsDetails != null)
                        {
                            foreach (var customerRouteOptionDetail in record.RouteOptionsDetails)
                            {
                                string optionPercentage = string.Empty;
                                if (customerRouteOptionDetail.Percentage != null)
                                    optionPercentage = customerRouteOptionDetail.Percentage + "% ";

                                string routeOptionsDetails = string.Concat(optionPercentage, customerRouteOptionDetail.SupplierName, " (", Math.Round(customerRouteOptionDetail.ConvertedSupplierRate, _longPrecisionValue), ")");
                                row.Cells.Add(new ExportExcelCell { Value = routeOptionsDetails });
                            }

                            int remainingOptions = maxNumberOfOptions - record.RouteOptionsDetails.Count();
                            if (remainingOptions > 0)
                            {
                                for (int i = 1; i <= remainingOptions; i++)
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                            }
                        }
                        else
                        {
                            for (var optionNb = 1; optionNb <= maxNumberOfOptions; optionNb++)
                                row.Cells.Add(new ExportExcelCell { Value = "" });
                        }

                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class RPRouteByZoneRequestHandler : BigDataRequestHandler<RPRouteQueryByZone, RPRoute, RPRouteDetailByZone>
        {
            RPRouteManager _manager = new RPRouteManager();

            public override RPRouteDetailByZone EntityDetailMapper(RPRoute entity)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<RPRoute> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByZone> input)
            {
                var latestRoutingDatabase = _manager.GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
                if (latestRoutingDatabase == null)
                    return null;

                IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
                dataManager.RoutingDatabase = latestRoutingDatabase;
                return dataManager.GetFilteredRPRoutesByZone(input);
            }

            protected override BigResult<RPRouteDetailByZone> AllRecordsToBigResult(DataRetrievalInput<RPRouteQueryByZone> input, IEnumerable<RPRoute> allRecords)
            {
                int? systemCurrencyId = null;
                int? toCurrencyId = null;
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                if (!input.Query.ShowInSystemCurrency && input.Query.CustomerId.HasValue)
                {
                    systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                    toCurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(input.Query.CustomerId.Value);
                }

                int? carrierProfileId = null;
                if (input.Query.CustomerId.HasValue)
                    carrierProfileId = carrierAccountManager.GetCarrierProfileId(input.Query.CustomerId.Value);

                DateTime effectiveDate = DateTime.Now;

                return allRecords.ToBigResult(input, null, (entity) => _manager.RPRouteDetailMapper(entity, input.Query.PolicyConfigId, input.Query.NumberOfOptions, systemCurrencyId,
                    toCurrencyId, carrierProfileId, effectiveDate, input.Query.IncludeBlockedSuppliers, input.Query.MaxSupplierRate));
            }

            protected override ResultProcessingHandler<RPRouteDetailByZone> GetResultProcessingHandler(DataRetrievalInput<RPRouteQueryByZone> input, BigResult<RPRouteDetailByZone> bigResult)
            {
                return new ResultProcessingHandler<RPRouteDetailByZone>()
                {
                    ExportExcelHandler = new RPRouteByZoneExcelExportHandler() { RatesIncluded = input.Query.CustomerId.HasValue }
                };
            }
        }

        private class RPRouteByCodeExcelExportHandler : ExcelExportHandler<RPRouteDetailByCode>
        {
            public bool RatesIncluded { get; set; }

            int _longPrecisionValue;
            public RPRouteByCodeExcelExportHandler()
            {
                _longPrecisionValue = new GeneralSettingsManager().GetLongPrecisionValue();
            }

            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<RPRouteDetailByCode> context)
            {
                ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

                ExportExcelSheet sheet = new ExportExcelSheet()
                {
                    SheetName = "Product Cost",
                    Header = new ExportExcelHeader { Cells = new List<ExportExcelHeaderCell>() },
                    AutoFitColumns = true
                };
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Code", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Routing Product", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Selling Number Plan", Width = 25 });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Sale Zone", Width = 25 });
                if (RatesIncluded)
                {
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Rate", Width = 25 });
                    sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Currency", Width = 25 });
                }
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Services" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Blocked" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = "Option 1", Width = 50 });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null && context.BigResult.Data.Count() > 0)
                {
                    int maxNumberOfOptions = context.BigResult.Data.Max(itm => itm.RouteOptionsDetails != null ? itm.RouteOptionsDetails.Count() : 0);

                    for (var optionNb = 2; optionNb <= maxNumberOfOptions; optionNb++)
                        sheet.Header.Cells.Add(new ExportExcelHeaderCell { Title = string.Format("Option {0}", optionNb), Width = 50 });

                    foreach (var record in context.BigResult.Data)
                    {
                        var row = new ExportExcelRow { Cells = new List<ExportExcelCell>() };
                        row.Cells.Add(new ExportExcelCell { Value = record.Code });
                        row.Cells.Add(new ExportExcelCell { Value = record.RoutingProductName });
                        row.Cells.Add(new ExportExcelCell { Value = record.SellingNumberPlan });
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneName });
                        if (RatesIncluded)
                        {
                            row.Cells.Add(new ExportExcelCell { Value = Math.Round(record.EffectiveRateValue.Value, _longPrecisionValue) });
                            row.Cells.Add(new ExportExcelCell { Value = record.CurrencySymbol });
                        }
                        row.Cells.Add(new ExportExcelCell { Value = record.SaleZoneServiceIds == null ? "" : zoneServiceConfigManager.GetZoneServicesNames(record.SaleZoneServiceIds.ToList()) });
                        row.Cells.Add(new ExportExcelCell { Value = record.IsBlocked });

                        if (record.RouteOptionsDetails != null)
                        {
                            foreach (var customerRouteOptionDetail in record.RouteOptionsDetails)
                            {
                                string optionPercentage = string.Empty;
                                if (customerRouteOptionDetail.Percentage != null)
                                    optionPercentage = customerRouteOptionDetail.Percentage + "% ";

                                string routeOptionsDetails = string.Concat(optionPercentage, customerRouteOptionDetail.SupplierName, " (", Math.Round(customerRouteOptionDetail.ConvertedSupplierRate, _longPrecisionValue), ")");
                                row.Cells.Add(new ExportExcelCell { Value = routeOptionsDetails });
                            }

                            int remainingOptions = maxNumberOfOptions - record.RouteOptionsDetails.Count();
                            if (remainingOptions > 0)
                            {
                                for (int i = 1; i <= remainingOptions; i++)
                                    row.Cells.Add(new ExportExcelCell { Value = "" });
                            }
                        }
                        else
                        {
                            for (var optionNb = 1; optionNb <= maxNumberOfOptions; optionNb++)
                                row.Cells.Add(new ExportExcelCell { Value = "" });
                        }

                        sheet.Rows.Add(row);
                    }
                }
                context.MainSheet = sheet;
            }
        }

        private class RPRouteByCodeRequestHandler : BigDataRequestHandler<RPRouteQueryByCode, RPRouteByCode, RPRouteDetailByCode>
        {
            RPRouteManager _manager = new RPRouteManager();

            RoutingDatabase RoutingDatabase { get; set; }

            public override RPRouteDetailByCode EntityDetailMapper(RPRouteByCode entity)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<RPRouteByCode> RetrieveAllData(Vanrise.Entities.DataRetrievalInput<RPRouteQueryByCode> input)
            {
                var latestRoutingDatabase = _manager.GetLatestRoutingDatabase(input.Query.RoutingDatabaseId);
                if (latestRoutingDatabase == null)
                    return null;

                RoutingDatabase = latestRoutingDatabase;
                List<RPRouteByCode> rpRouteByCodes = new List<RPRouteByCode>();

                IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
                dataManager.RoutingDatabase = latestRoutingDatabase;
                return dataManager.GetFilteredRPRoutesByCode(input);
            }

            protected override BigResult<RPRouteDetailByCode> AllRecordsToBigResult(DataRetrievalInput<RPRouteQueryByCode> input, IEnumerable<RPRouteByCode> allRecords)
            {
                int? systemCurrencyId = null;
                int? toCurrencyId = null;
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

                if (!input.Query.ShowInSystemCurrency && input.Query.CustomerId.HasValue)
                {
                    systemCurrencyId = new Vanrise.Common.Business.ConfigManager().GetSystemCurrencyId();
                    toCurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(input.Query.CustomerId.Value);
                }

                int? carrierProfileId = null;
                if (input.Query.CustomerId.HasValue)
                    carrierProfileId = carrierAccountManager.GetCarrierProfileId(input.Query.CustomerId.Value);

                DateTime effectiveDate = DateTime.Now;

                return allRecords.ToBigResult(input, null, (entity) => _manager.RPRouteDetailByCodeMapper(entity, input.Query.NumberOfOptions, systemCurrencyId,
                    toCurrencyId, carrierProfileId, effectiveDate, input.Query.RoutingDatabaseId, input.Query.IncludeBlockedSuppliers, input.Query.MaxSupplierRate, input.Query.CustomerId));
            }

            protected override ResultProcessingHandler<RPRouteDetailByCode> GetResultProcessingHandler(DataRetrievalInput<RPRouteQueryByCode> input, BigResult<RPRouteDetailByCode> bigResult)
            {
                var resultProcessingHandler = new ResultProcessingHandler<RPRouteDetailByCode>()
                {
                    ExportExcelHandler = new RPRouteByCodeExcelExportHandler() { RatesIncluded = input.Query.CustomerId.HasValue }
                };
                return resultProcessingHandler;
            }
        }

        #endregion

        #region Mappers

        private RPRouteDetailByZone RPRouteDetailMapper(RPRoute rpRoute, Guid policyConfigId, int? numberOfOptions, int? systemCurrencyId, int? toCurrencyId, int? customerProfileId, DateTime effectiveDate,
            bool includeBlockedSuppliers, decimal? maxSupplierRate)
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

            return new RPRouteDetailByZone()
            {
                RoutingProductId = rpRoute.RoutingProductId,
                SaleZoneId = rpRoute.SaleZoneId,
                SaleZoneServiceIds = rpRoute.SaleZoneServiceIds,
                RoutingProductName = _routingProductManager.GetRoutingProductName(rpRoute.RoutingProductId),
                SellingNumberPlan = GetSellingNumberPlan(rpRoute.SaleZoneId),
                SaleZoneName = rpRoute.SaleZoneName,
                IsBlocked = rpRoute.IsBlocked,
                RouteOptionsDetails = this.GetRouteOptionDetails(rpRoute.RPOptionsByPolicy, policyConfigId, numberOfOptions, systemCurrencyId, toCurrencyId, customerProfileId, includeBlockedSuppliers, effectiveRateValue, maxSupplierRate),
                ExecutedRuleId = rpRoute.ExecutedRuleId,
                EffectiveRateValue = effectiveRateValue,
                CurrencySymbol = currencySymbol
            };
        }

        private RPRouteDetailByCode RPRouteDetailByCodeMapper(RPRouteByCode rpRoute, int? numberOfOptions, int? systemCurrencyId, int? toCurrencyId, int? customerProfileId,
          DateTime effectiveDate, int routingDatabaseId, bool includeBlockedSuppliers, decimal? maxSupplierRate, int? customerId)
        {
            RoutingDatabase routingDatabase = GetLatestRoutingDatabase(routingDatabaseId);

            var rpRouteByCode = BuildRPRouteByCodeWithOptions(rpRoute, routingDatabase, customerId);

            CurrencyManager currencyManager = new CurrencyManager();
            string currencySymbol;
            decimal? effectiveRateValue = rpRoute.Rate;

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

            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            List<RPRouteOptionByCodeDetail> details = null;
            if (rpRouteByCode.Options != null)
            {
                details = new List<RPRouteOptionByCodeDetail>();

                int addedActiveItems = 0;
                var optionOrder = 1;

                foreach (RouteOption option in rpRouteByCode.Options)
                {
                    if (!includeBlockedSuppliers && option.IsBlocked)
                        continue;

                    decimal convertedSupplierRate = toCurrencyId.HasValue ? GetRateConvertedToCurrency(option.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate) : option.SupplierRate;
                    bool isLossy = effectiveRateValue.HasValue ? effectiveRateValue.Value < convertedSupplierRate : false;

                    if (maxSupplierRate.HasValue && maxSupplierRate.Value < convertedSupplierRate)
                        continue;

                    RPRouteOptionByCodeDetail detail = BuildRPRouteOptionByCodeDetail(option, convertedSupplierRate, currencySymbol, isLossy, systemCurrencyId, toCurrencyId, effectiveDate, effectiveRateValue, includeBlockedSuppliers, optionOrder);
                    details.Add(detail);
                    optionOrder++;

                    if (option.IsBlocked)
                        continue;

                    addedActiveItems++;
                    if (numberOfOptions.HasValue && numberOfOptions.Value == addedActiveItems)
                        break;
                }
            }

            return new RPRouteDetailByCode()
            {
                RoutingProductId = rpRoute.RoutingProductId,
                RoutingProductName = new RoutingProductManager().GetRoutingProductName(rpRoute.RoutingProductId),
                Code = rpRoute.Code,
                SaleZoneId = rpRoute.SaleZoneId,
                SaleZoneServiceIds = rpRoute.SaleZoneServiceIds,
                SellingNumberPlan = GetSellingNumberPlan(rpRoute.SaleZoneId),
                SaleZoneName = rpRoute.SaleZoneName,
                IsBlocked = rpRoute.IsBlocked,
                RouteOptionsDetails = details,
                ExecutedRuleId = rpRoute.ExecutedRuleId.Value,
                EffectiveRateValue = effectiveRateValue,
                CurrencySymbol = currencySymbol
            };
        }

        private RPRouteOptionByCodeDetail BuildRPRouteOptionByCodeDetail(RouteOption option, decimal convertedSupplierRate, string currencySymbol, bool isLossy,
            int? systemCurrencyId, int? toCurrencyId, DateTime effectiveDate, decimal? effectiveRateValue, bool includeBlockedSuppliers, int optionOrder)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            SupplierZoneManager supplierZoneManager = new SupplierZoneManager();
            RPRouteOptionByCodeDetail detail = new RPRouteOptionByCodeDetail()
              {
                  ConvertedSupplierRate = convertedSupplierRate,
                  CurrencySymbol = currencySymbol,
                  EvaluatedStatus = TOne.WhS.Routing.Entities.Helper.GetEvaluatedStatus(option.IsBlocked, isLossy, option.IsForced, option.ExecutedRuleId),
                  ExecutedRuleId = option.ExecutedRuleId,
                  Percentage = option.Percentage,
                  SupplierCode = option.SupplierCode,
                  SupplierId = option.SupplierId,
                  SupplierName = carrierAccountManager.GetCarrierAccountName(option.SupplierId),
                  SupplierRate = option.SupplierRate,
                  SupplierZoneId = option.SupplierZoneId,
                  SupplierZoneName = supplierZoneManager.GetSupplierZoneName(option.SupplierZoneId),
                  OptionOrder = optionOrder
              };

            if (option.Backups != null)
            {
                detail.Backups = new List<RPRouteBackupOptionByCodeDetail>();
                foreach (var backupOption in option.Backups)
                {
                    if (!includeBlockedSuppliers && backupOption.IsBlocked)
                        continue;

                    decimal convertedBackupRate = toCurrencyId.HasValue ? GetRateConvertedToCurrency(backupOption.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate) : backupOption.SupplierRate;
                    bool isBackupLossy = effectiveRateValue.HasValue ? effectiveRateValue.Value < convertedSupplierRate : false;

                    RPRouteBackupOptionByCodeDetail backup = new RPRouteBackupOptionByCodeDetail()
                    {
                        ConvertedSupplierRate = convertedBackupRate,
                        CurrencySymbol = currencySymbol,
                        EvaluatedStatus = TOne.WhS.Routing.Entities.Helper.GetEvaluatedStatus(backupOption.IsBlocked, isBackupLossy, backupOption.IsForced, backupOption.ExecutedRuleId),
                        ExecutedRuleId = backupOption.ExecutedRuleId,
                        SupplierCode = backupOption.SupplierCode,
                        SupplierId = backupOption.SupplierId,
                        SupplierName = carrierAccountManager.GetCarrierAccountName(backupOption.SupplierId),
                        SupplierRate = backupOption.SupplierRate,
                        SupplierZoneId = backupOption.SupplierZoneId,
                        SupplierZoneName = supplierZoneManager.GetSupplierZoneName(backupOption.SupplierZoneId)
                    };
                    detail.Backups.Add(backup);
                }
            }

            return detail;
        }

        private RPRouteByCode BuildRPRouteByCodeWithOptions(RPRouteByCode rpRoute, RoutingDatabase routingDatabase, int? customerId)
        {
            RouteBuilder routeBuilder = new RouteBuilder(RoutingProcessType.RoutingProductRoute);
            Dictionary<RouteRule, List<RouteOptionRuleTarget>> optionsByRules = new Dictionary<RouteRule, List<RouteOptionRuleTarget>>();

            SaleZoneDefintion saleZoneDefintion = new SaleZoneDefintion() { SaleZoneId = rpRoute.SaleZoneId, SellingNumberPlanId = rpRoute.SellingNumberPlanID };
            CustomerZoneDetailData customerZoneDetailData = new CustomerZoneDetailData() { CustomerId = customerId, EffectiveRateValue = rpRoute.Rate, SaleZoneId = rpRoute.SaleZoneId, SaleZoneServiceIds = rpRoute.SaleZoneServiceIds };
            RouteRule routeRule = new RouteRuleManager().GetRule(rpRoute.ExecutedRuleId.Value);

            List<SupplierCodeMatchWithRate> supplierCodeMatchWithRates = new List<SupplierCodeMatchWithRate>();
            SupplierCodeMatchWithRateBySupplier supplierCodeMatchWithRateBySupplier = new Entities.SupplierCodeMatchWithRateBySupplier();

            var routeRuleTarget = new RouteRuleTarget
            {
                CustomerId = customerId,
                Code = rpRoute.Code,
                SaleZoneId = saleZoneDefintion.SaleZoneId,
                CountryId = new SaleZoneManager().GetSaleZoneCountryId(saleZoneDefintion.SaleZoneId).Value,
                RoutingProductId = rpRoute.RoutingProductId,
                SaleRate = rpRoute.Rate,
                EffectiveOn = routingDatabase.EffectiveTime,
                IsEffectiveInFuture = routingDatabase.Type == RoutingDatabaseType.Future
            };

            if (rpRoute.SupplierZoneDetails != null)
            {
                foreach (var supplierZoneDetail in rpRoute.SupplierZoneDetails)
                {
                    SupplierCodeMatchWithRate supplierCodeMatchWithRate = new SupplierCodeMatchWithRate()
                    {
                        CodeMatch = new SupplierCodeMatch()
                        {
                            SupplierCode = rpRoute.SupplierCodeMatchByZoneId.GetRecord(supplierZoneDetail.SupplierZoneId),
                            SupplierId = supplierZoneDetail.SupplierId,
                            SupplierZoneId = supplierZoneDetail.SupplierZoneId
                        },
                        ExactSupplierServiceIds = supplierZoneDetail.ExactSupplierServiceIds,
                        RateValue = supplierZoneDetail.EffectiveRateValue,
                        SupplierRateEED = supplierZoneDetail.SupplierRateEED,
                        SupplierRateId = supplierZoneDetail.SupplierRateId,
                        SupplierServiceIds = supplierZoneDetail.SupplierServiceIds,
                        SupplierServiceWeight = supplierZoneDetail.SupplierServiceWeight
                    };
                    supplierCodeMatchWithRates.Add(supplierCodeMatchWithRate);

                    supplierCodeMatchWithRateBySupplier.Add(supplierZoneDetail.SupplierId, supplierCodeMatchWithRate);
                }
            }

            bool keepBackupsForRemovedOptions = new ConfigManager().GetProductRouteBuildKeepBackUpsForRemovedOptions();

            return routeBuilder.ExecuteRule<RPRouteByCode>(optionsByRules, rpRoute.Code, saleZoneDefintion, customerZoneDetailData, supplierCodeMatchWithRates, supplierCodeMatchWithRateBySupplier, routeRuleTarget, routeRule, routingDatabase, RoutingProcessType.RoutingProductRoute, keepBackupsForRemovedOptions);
        }

        private RPRouteOptionDetail RPRouteOptionMapper(RPRouteOption routeOption, int? systemCurrencyId, int? toCurrencyId, int optionOrder, DateTime effectiveDate, decimal? effectiveRateValue)
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

            bool isblocked = routeOption.SupplierStatus == SupplierStatus.Block;
            bool isLossy = effectiveRateValue.HasValue ? effectiveRateValue.Value < convertedSupplierRate : false;

            var routeOptionDetail = new RPRouteOptionDetail()
            {
                SaleZoneId = routeOption.SaleZoneId,
                SupplierId = routeOption.SupplierId,
                SupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId),
                SupplierRate = routeOption.SupplierRate,
                Percentage = routeOption.Percentage,
                SupplierZoneMatchHasClosedRate = routeOption.SupplierZoneMatchHasClosedRate,
                ConvertedSupplierRate = convertedSupplierRate,
                OptionOrder = optionOrder,
                CurrencySymbol = currencySymbol,
                SupplierStatus = routeOption.SupplierStatus,
                IsForced = routeOption.IsForced,
                EvaluatedStatus = TOne.WhS.Routing.Entities.Helper.GetEvaluatedStatus(isblocked, isLossy, routeOption.IsForced, null)
            };
            return routeOptionDetail;
        }

        private RPRouteOptionSupplierZoneDetail RPRouteOptionSupplierZoneDetailMapper(SupplierZoneRateLocator futureSupplierZoneRateLocator, Dictionary<long, SupplierRate> supplierRateByIds,
            RPRouteOptionSupplierZone rpRouteOptionSupplierZone, int? systemCurrencyId, int? toCurrencyId, DateTime effectiveDate, decimal? saleRate)
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
                RateEED = supplierRate != null ? supplierRate.EED : null,
            };

            if (toCurrencyId.HasValue)
            {
                if (!systemCurrencyId.HasValue)
                    throw new ArgumentNullException("systemCurrencyId");

                detailEntity.ConvertedSupplierRate = GetRateConvertedToCurrency(rpRouteOptionSupplierZone.SupplierRate, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate);
                if (detailEntity.FutureRate.HasValue)
                    detailEntity.FutureRate = GetRateConvertedToCurrency(detailEntity.FutureRate.Value, systemCurrencyId.Value, toCurrencyId.Value, effectiveDate);
            }

            bool isLossy = saleRate.HasValue ? saleRate.Value < detailEntity.ConvertedSupplierRate : false;
            detailEntity.EvaluatedStatus = TOne.WhS.Routing.Entities.Helper.GetEvaluatedStatus(rpRouteOptionSupplierZone.IsBlocked, isLossy, rpRouteOptionSupplierZone.IsForced, rpRouteOptionSupplierZone.ExecutedRuleId);

            return detailEntity;
        }

        #endregion
    }
}