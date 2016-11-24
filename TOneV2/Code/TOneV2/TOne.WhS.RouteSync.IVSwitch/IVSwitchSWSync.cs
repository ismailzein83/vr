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
            PreparedConfiguration.GetCachedPreparedConfiguration(this);
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
                StringBuilder sbRoute = new StringBuilder(), sbTariff = new StringBuilder();
                IVSwitchConvertedRoute ivSwitchConvertedRoute = (IVSwitchConvertedRoute)convertedRoute;
                foreach (var item in ivSwitchConvertedRoute.Routes)
                    sbRoute.AppendLine(PrepareRouteString(item));

                foreach (var itemTrff in ivSwitchConvertedRoute.Tariffs)
                    sbTariff.AppendLine(PrepareTariffString(itemTrff));

                PreparedRoute tempRoute;
                if (customerRoutes.TryGetValue(ivSwitchConvertedRoute.CustomerID, out tempRoute))
                {
                    tempRoute.RoutesCount += ivSwitchConvertedRoute.Routes.Count;
                    tempRoute.TariffCount += ivSwitchConvertedRoute.Tariffs.Count;
                    tempRoute.StrRoutes = tempRoute.StrRoutes.Append(sbRoute);
                    tempRoute.StrTariff = tempRoute.StrTariff.Append(sbTariff);
                }
                else
                {
                    tempRoute = new PreparedRoute
                    {
                        TariffTableName = ivSwitchConvertedRoute.TariffTableName,
                        RouteTableName = ivSwitchConvertedRoute.RouteTableName,
                        RoutesCount = ivSwitchConvertedRoute.Routes.Count,
                        TariffCount = ivSwitchConvertedRoute.Tariffs.Count,
                        StrRoutes = sbRoute,
                        StrTariff = sbTariff
                    };
                    customerRoutes[ivSwitchConvertedRoute.CustomerID] = tempRoute;
                }
            }
            foreach (var convertedRoute in customerRoutes)
            {
                PreparedRoute tempRoute = convertedRoute.Value;
                tempRoute.Routes = GetBytes(tempRoute.StrRoutes.ToString());
                tempRoute.Tariffs = GetBytes(tempRoute.StrTariff.ToString());
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
        private byte[] Combine(byte[] array1, byte[] array2)
        {
            byte[] rv = new byte[array1.Length + array2.Length];
            Buffer.BlockCopy(array1, 0, rv, 0, array1.Length);
            Buffer.BlockCopy(array2, 0, rv, array1.Length, array2.Length);
            return rv;
        }
        private byte[] GetBytes(string value)
        {
            Encoding encoding = new UTF8Encoding();
            return encoding.GetBytes(value);
        }
        private string PrepareTariffString(IVSwitchTariff tariff)
        {
            return string.Format(@"{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                "\t", tariff.DestinationCode, tariff.TimeFrame, tariff.DestinationName,
                tariff.InitPeiod, tariff.NextPeriod, tariff.InitCharge, tariff.NextCharge);
        }
        private string PrepareRouteString(IVSwitchRoute route)
        {
            return
                string.Format(
                    @"{1}{23}{2}{23}{3}{23}{4}{23}{5}{23}{6}{23}{7}{23}{8}{23}{9}{23}{10}{23}{11}{23}{12}{23}{13}{23}{14}{23}{15}{23}{16}{23}{17}{23}{18}{23}{19}{23}{20}{23}{21}{23}{22}",
                    string.Empty, route.Destination, route.RoutingMode, route.TimeFrame,
                    route.Preference, route.HuntStop, route.HuntStopRc, route.MinProfit,
                    route.StateId, route.WakeUpTime, route.Description
                    , route.RoutingMode, route.TotalBkts, route.BktSerial, route.BktCapacity,
                    route.BktToken, route.PScore, route.Flag1, route.Flag2, route.Flag3,
                    route.Flag4, route.Flag5, route.TechPrefix
                    , "\t");
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
