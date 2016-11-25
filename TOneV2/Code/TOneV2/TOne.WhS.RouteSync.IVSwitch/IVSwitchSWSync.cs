using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchSWSync : SwitchRouteSynchronizer
    {
        #region properties
        public string OwnerName { get; set; }
        public string MasterConnectionString { get; set; }
        public string RouteConnectionString { get; set; }
        public string TariffConnectionString { get; set; }
        public int NumberOfOptions { get; set; }
        public string BlockedAccountMapping { get; set; }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }
        public string Separator { get; set; }
        public Guid Uid { get; set; }

        #endregion

        public override Guid ConfigId { get { return new Guid("64152327-5DB5-47AE-9569-23D38BCB18CC"); } }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            PreparedConfiguration preparedData = PreparedConfiguration.GetCachedPreparedConfiguration(this);
            BuildTempTables(preparedData);
        }
        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;
            PreparedConfiguration preparedData = PreparedConfiguration.GetCachedPreparedConfiguration(this);
            List<ConvertedRoute> routes = new List<ConvertedRoute>();
            foreach (var route in context.Routes)
            {
                IvSwitchMapping carrierMapping;
                if (preparedData.IvSwitchMappings.TryGetValue(route.CustomerId, out carrierMapping))
                {
                    foreach (var customerMapping in carrierMapping.CustomerMapping)
                    {
                        routes.Add(BuildRouteAndRouteOptions(route, customerMapping, preparedData));
                    }
                }
            }
            context.ConvertedRoutes = routes;
        }
        public override object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            Dictionary<int, PreparedRoute> customerRoutes = new Dictionary<int, PreparedRoute>();
            foreach (var convertedRoute in context.ConvertedRoutes)
            {
                IVSwitchConvertedRoute ivSwitchConvertedRoute = (IVSwitchConvertedRoute)convertedRoute;

                PreparedRoute tempRoute;
                if (customerRoutes.TryGetValue(ivSwitchConvertedRoute.CustomerID, out tempRoute))
                {
                    tempRoute.Routes.AddRange(ivSwitchConvertedRoute.Routes);
                    tempRoute.Tariffs.AddRange(ivSwitchConvertedRoute.Tariffs);
                }
                else
                {
                    tempRoute = new PreparedRoute
                    {
                        TariffTableName = ivSwitchConvertedRoute.TariffTableName,
                        RouteTableName = ivSwitchConvertedRoute.RouteTableName,
                        Routes = ivSwitchConvertedRoute.Routes,
                        Tariffs = ivSwitchConvertedRoute.Tariffs
                    };
                    customerRoutes[ivSwitchConvertedRoute.CustomerID] = tempRoute;
                }
            }
            return customerRoutes;
        }
        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            Dictionary<int, PreparedRoute> routes = (Dictionary<int, PreparedRoute>)context.PreparedItemsForApply;
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            IVSwitchTariffDataManager tariffDataManager = new IVSwitchTariffDataManager(TariffConnectionString, OwnerName);
            foreach (var item in routes.Values)
            {
                routeDataManager.Bulk(item.Routes, string.Format("{0}_temp", item.RouteTableName));
                tariffDataManager.Bulk(item.Tariffs, string.Format("{0}_temp", item.TariffTableName));
            }
        }
        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            PreparedConfiguration preparedData = PreparedConfiguration.GetCachedPreparedConfiguration(this);
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            IVSwitchTariffDataManager tariffDataManager = new IVSwitchTariffDataManager(TariffConnectionString, OwnerName);
            foreach (var customerTable in preparedData.CustomerTables)
            {
                routeDataManager.CreatePrimaryKey(customerTable.RouteTableName);
                routeDataManager.Swap(customerTable.RouteTableName);
                tariffDataManager.Swap(customerTable.TariffTableName);
            }
        }
        #region private functions
        private void BuildTempTables(PreparedConfiguration preparedData)
        {
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            IVSwitchTariffDataManager tariffDataManager = new IVSwitchTariffDataManager(TariffConnectionString, OwnerName);
            foreach (var customerTable in preparedData.CustomerTables)
            {
                routeDataManager.BuildRouteTable(customerTable.RouteTableName);
                tariffDataManager.BuildTariffTable(customerTable.TariffTableName);
            }
        }
        private IVSwitchTariff BuildTariff(Route route)
        {
            IVSwitchTariff tariff = new IVSwitchTariff
            {
                DestinationCode = route.Code,
                TimeFrame = "* * * * *",
                InitCharge = route.SaleRate
            };
            SaleZoneManager manager = new SaleZoneManager();
            if (route.SaleZoneId.HasValue)
            {
                tariff.DestinationName = manager.GetSaleZoneName(route.SaleZoneId.Value);
            }
            return tariff;
        }

        private IVSwitchConvertedRoute BuildRouteAndRouteOptions(Route route, string carrierMapping, PreparedConfiguration preparedData)
        {
            if (route == null)
                return null;
            if (!preparedData.CustomerDefinitions.ContainsKey(carrierMapping))
                return null;
            var customerDefinition = preparedData.CustomerDefinitions[carrierMapping];
            if (!customerDefinition.RouteTableId.HasValue)
                return null;
            IVSwitchConvertedRoute ivSwitch = new IVSwitchConvertedRoute
            {
                Routes = new List<IVSwitchRoute>(),
                Tariffs = new List<IVSwitchTariff>(),
                RouteTableName = customerDefinition.RouteTableName,
                TariffTableName = customerDefinition.TariffTableName
            };
            if (route.Options == null || route.Options.Count == 0)
            {
                IVSwitchRoute ivSwitchRoute = BuildBlockedRoute(preparedData);
                if (ivSwitchRoute != null) ivSwitchRoute.Destination = route.Code;
                ivSwitch.Routes.Add(ivSwitchRoute);
                return ivSwitch;
            }
            decimal? optionsPercenatgeSum = route.Options.Sum(it => it.Percentage);
            Decimal? maxPercentage = route.Options.Max(it => it.Percentage);


            List<IVSwitchRoute> routes = new List<IVSwitchRoute>();
            int gatewayCount = route.Options.Count;
            foreach (var option in route.Options)
            {
                int serial = 1;
                int priority = NumberOfOptions;
                IvSwitchMapping supplierMapping;
                if (preparedData.IvSwitchMappings.TryGetValue(option.SupplierId, out supplierMapping) && supplierMapping.SupplierGateways != null)
                {
                    foreach (var supplierGateWay in supplierMapping.SupplierGateways)
                    {
                        if (priority == 0) break;
                        if (string.IsNullOrEmpty(supplierGateWay.Mapping)) continue;
                        if (!preparedData.SupplierDefinitions.ContainsKey(supplierGateWay.Mapping)) continue;

                        IVSwitchRoute ivOption = new IVSwitchRoute
                        {
                            Destination = route.Code,
                            RouteId = customerDefinition.RouteTableId,
                            TimeFrame = "* * * * *",
                            Preference = priority,
                            StateId = 1,
                            HuntStop = 0,
                            WakeUpTime = DateTime.UtcNow,
                            TotalBkts = gatewayCount,
                            Flag1 =
                                BuildPercentage(supplierGateWay.Percentage, option.Percentage, optionsPercenatgeSum,
                                    supplierMapping.SupplierGateways.Count),
                            BktSerial = serial++
                        };
                        ivOption.BktCapacity = decimal.ToInt32(BuildScaledDownPercentage(ivOption.Flag1 ?? 0, 1,
                            maxPercentage ?? 0, 1, optionsPercenatgeSum ?? 0));
                        ivOption.BktToken = ivOption.BktCapacity;

                        routes.Add(ivOption);
                        priority--;
                    }
                }
            }
            ivSwitch.Tariffs.Add(BuildTariff((route)));
            ivSwitch.Routes.AddRange(routes);
            return ivSwitch;
        }
        private IVSwitchRoute BuildBlockedRoute(PreparedConfiguration preparedConfiguration)
        {
            if (preparedConfiguration.SupplierDefinitions != null && preparedConfiguration.SupplierDefinitions.Count > 0)
            {
                return new IVSwitchRoute
                {
                    Description = "BLK",
                    RouteId = preparedConfiguration.BlockRouteId
                };
            }
            return null;
        }

        #endregion

        #region Percentage Routing
        private decimal BuildScaledDownPercentage(decimal x, decimal z1, decimal z2, decimal y1, decimal y2)
        {
            return Math.Ceiling(z1 * (1 - ((x - y1) / (y2 - y1))) + (z2 * ((x - y1) / (y2 - y1))));
        }
        private decimal? BuildPercentagePerGateway(decimal gatewayPercentage, decimal? optionPercentage, decimal? optionsPercenatgeSum)
        {
            if (optionPercentage.HasValue)
                return ((gatewayPercentage * optionPercentage.Value) / optionsPercenatgeSum);
            return 0;
        }

        private decimal? BuildOptionPercentage(decimal? optionPercentage, decimal? optionsPercentageSum, int gatewayCount)
        {
            if (optionPercentage.HasValue && optionsPercentageSum.HasValue)
                return ((optionPercentage.Value * 100) / optionsPercentageSum.Value) / gatewayCount;
            return 0;
        }
        private decimal? BuildPercentage(decimal gatewayPercentage, decimal? optionPercentage, decimal? optionsPercenatgeSum, int gatewayCount)
        {
            return gatewayPercentage > 0
                ? BuildPercentagePerGateway(gatewayPercentage, optionPercentage, optionsPercenatgeSum)
                : BuildOptionPercentage(optionPercentage, optionsPercenatgeSum, gatewayCount);
        }
        #endregion
    }
}
