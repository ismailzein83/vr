using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.Entities;
using Vanrise.Data.Postgres;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class IVSwitchSWSync : SwitchRouteSynchronizer
    {
        #region properties

        private IVSwitchRouteDataManager _routeDataManager;
        private IVSwitchTariffDataManager _tariffDataManager;
        private IVSwitchMasterDataManager _masterDataManager;
        public string OwnerName { get; set; }
        public string MasterConnectionString { get; set; }
        public string RouteConnectionString { get; set; }
        public string TariffConnectionString { get; set; }
        public int NumberOfOptions { get; set; }
        public string BlockedAccountMapping { get; set; }
        public string Separator { get; set; }
        private Dictionary<string, CarrierDefinition> CustomerDefinitions { get; set; }
        private Dictionary<string, CarrierDefinition> SupplierDefinitions { get; set; }

        private List<CarrierDefinition> CustomerTables { get; set; }

        #endregion

        public override Guid ConfigId { get { return new Guid("64152327-5DB5-47AE-9569-23D38BCB18CC"); } }
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        private int _blockRouteId;
        private Dictionary<string, IvSwitchMapping> IvSwitchMappings { get; set; }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            _routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            _tariffDataManager = new IVSwitchTariffDataManager(TariffConnectionString, OwnerName);
            _masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);

            CustomerDefinitions = _masterDataManager.GetCustomerDefinition();
            SupplierDefinitions = _masterDataManager.GetSupplierDefinition();
            _blockRouteId = SupplierDefinitions.ContainsKey(BlockedAccountMapping)
                     ? SupplierDefinitions[BlockedAccountMapping].RouteTableId
                     : 0;
            CustomerTables = new List<CarrierDefinition>();
            BuildIvSwitchMapping();
            BuildTempTables();
        }
        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;
            List<ConvertedRoute> routes = new List<ConvertedRoute>();
            foreach (var route in context.Routes)
            {
                IvSwitchMapping carrierMapping;
                if (IvSwitchMappings.TryGetValue(route.CustomerId, out carrierMapping) && carrierMapping.CustomerGateways != null)
                {
                    foreach (var customerGateway in carrierMapping.CustomerGateways)
                    {
                        CarrierDefinition customerDefinition;
                        if (!CustomerDefinitions.TryGetValue(customerGateway.Mapping, out customerDefinition)) continue;

                        if (customerDefinition.RouteTableId == 0) continue;
                        IVSwitchConvertedRoute ivSwitch = new IVSwitchConvertedRoute
                        {
                            Routes = new List<IVSwitchRoute>(),
                            Tariffs = new List<IVSwitchTariff>(),
                            RouteTableName = customerDefinition.RouteTableName,
                            TariffTableName = customerDefinition.TariffTableName
                        };
                        List<IVSwitchOption> options = BuildOptions(route);
                        if (options == null)
                        {
                            IVSwitchRoute ivSwitchRoute = BuildBlockedRoute();
                            if (ivSwitchRoute != null) ivSwitchRoute.Destination = route.Code;
                            ivSwitch.Routes.Add(ivSwitchRoute);
                        }
                        else
                        {
                            ivSwitch.Routes.AddRange(BuildIvSwitchvonvertedRoute(customerGateway, options, route));
                        }
                        IVSwitchTariff tariff = BuildTariff(route);
                        ivSwitch.Tariffs.Add(tariff);
                        routes.Add(ivSwitch);
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
                    byte[] bytes = GetBytes(sbRoute.ToString());
                    tempRoute.RoutesCount += ivSwitchConvertedRoute.Routes.Count;
                    tempRoute.Routes = Combine(tempRoute.Routes, bytes);
                    bytes = GetBytes(sbTariff.ToString());
                    tempRoute.TariffCount += ivSwitchConvertedRoute.Tariffs.Count;
                    tempRoute.Tariffs = Combine(tempRoute.Tariffs, bytes);
                }
                else
                {
                    tempRoute = new PreparedRoute
                    {
                        Routes = GetBytes(sbRoute.ToString()),
                        Tariffs = GetBytes(sbTariff.ToString()),
                        TariffTableName = ivSwitchConvertedRoute.TariffTableName,
                        RouteTableName = ivSwitchConvertedRoute.RouteTableName,
                        RoutesCount = ivSwitchConvertedRoute.Routes.Count,
                        TariffCount = ivSwitchConvertedRoute.Tariffs.Count
                    };
                    customerRoutes[ivSwitchConvertedRoute.CustomerID] = tempRoute;
                }
            }
            return customerRoutes;
        }
        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            Dictionary<int, PreparedRoute> routes = (Dictionary<int, PreparedRoute>)context.PreparedItemsForApply;
            foreach (var item in routes.Values)
            {
                _routeDataManager.BulkCopy(item.RouteTableName, item.Routes, item.RoutesCount);
                _tariffDataManager.BulkCopy(item.TariffTableName, item.Tariffs, item.TariffCount);
            }
        }
        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            foreach (var customerTable in CustomerTables)
            {
                _routeDataManager.CreatePrimaryKey(customerTable.RouteTableName);
                _routeDataManager.Swap(customerTable.RouteTableName);
                _tariffDataManager.Swap(customerTable.TariffTableName);
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
        private void BuildTempTables()
        {
            foreach (var customerTable in CustomerTables)
            {
                _routeDataManager.BuildRouteTable(customerTable.RouteTableName);
                _tariffDataManager.BuildTariffTable(customerTable.TariffTableName);
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
        private List<IVSwitchRoute> BuildIvSwitchvonvertedRoute(Gateway carrierGateway, List<IVSwitchOption> options, Route route)
        {
            if (!CustomerDefinitions.ContainsKey(carrierGateway.Mapping))
                return null;
            var customerDefinition = CustomerDefinitions[carrierGateway.Mapping];
            if (customerDefinition.RouteTableId == 0)
                return null;
            List<IVSwitchRoute> routes = new List<IVSwitchRoute>();
            int gatewayCount = options.Count;
            foreach (var option in options)
            {
                IVSwitchRoute ivSwitchRoute = new IVSwitchRoute
                {
                    Destination = route.Code,
                    RouteId = customerDefinition.RouteTableId,
                    TimeFrame = "* * * * *",
                    Preference = option.Priority ?? 0,
                    StateId = 1,
                    HuntStop = 0,
                    WakeUpTime = DateTime.UtcNow,
                    TotalBkts = gatewayCount,
                    Flag1 = option.Percentage ?? 0,
                    BktCapacity = decimal.ToInt32(option.ScaledDownPercentage),
                    BktToken = decimal.ToInt32(option.ScaledDownPercentage),
                    BktSerial = option.Serial
                };
                routes.Add(ivSwitchRoute);
            }
            return routes;
        }
        private List<IVSwitchOption> BuildOptions(Route route)
        {
            if (route == null || route.Options == null || route.Options.Count == 0)
                return null;
            List<IVSwitchOption> options = new List<IVSwitchOption>();
            decimal? optionsPercenatgeSum = route.Options.Sum(it => it.Percentage);
            Decimal? maxPercentage = route.Options.Max(it => it.Percentage);
            foreach (var option in route.Options)
            {
                int serial = 1;
                int priority = NumberOfOptions;
                IvSwitchMapping carrierMapping;
                if (IvSwitchMappings.TryGetValue(option.SupplierId, out carrierMapping) && carrierMapping.SupplierGateways != null)
                {
                    foreach (var supplierGateWay in carrierMapping.SupplierGateways)
                    {
                        if (priority == 0) break;
                        if (string.IsNullOrEmpty(supplierGateWay.Mapping)) continue;
                        if (!SupplierDefinitions.ContainsKey(supplierGateWay.Mapping)) continue;
                        IVSwitchOption ivOption = new IVSwitchOption
                        {
                            Option = supplierGateWay.Mapping,
                            Percentage = BuildPercentage(supplierGateWay.Percentage, option.Percentage, optionsPercenatgeSum, carrierMapping.SupplierGateways.Count),
                            Priority = priority,
                            Serial = serial++
                        };
                        ivOption.ScaledDownPercentage = BuildScaledDownPercentage(ivOption.Percentage ?? 0, 1,
                            maxPercentage ?? 0, 1, optionsPercenatgeSum ?? 0);

                        options.Add(ivOption);
                        priority--;
                    }
                }
            }
            return options;
        }
        private void BuildIvSwitchMapping()
        {
            IvSwitchMappings = new Dictionary<string, IvSwitchMapping>();
            foreach (var mapItem in CarrierMappings)
            {
                var map = mapItem.Value;
                if (map.CustomerMapping == null && map.SupplierMapping == null) continue;
                IvSwitchMapping ivSwitchMapping = new IvSwitchMapping
                {
                    CarrierId = map.CarrierId,
                    InnerPrefix = map.InnerPrefix,
                    CustomerGateways = new List<Gateway>(),
                    SupplierGateways = new List<Gateway>()
                };
                if (map.CustomerMapping != null)
                    foreach (var customerMapping in map.CustomerMapping)
                    {
                        CarrierDefinition definition;
                        if (!CustomerDefinitions.TryGetValue(customerMapping, out definition)) continue;
                        CustomerTables.Add(definition);
                        Gateway gateway = new Gateway { Mapping = customerMapping };
                        string[] parts = customerMapping.Split(':');
                        if (parts.Count() > 1)
                        {
                            gateway.Mapping = parts[0];
                            int percentage;
                            int.TryParse(parts[1], out percentage);
                            gateway.Percentage = percentage;
                        }
                        ivSwitchMapping.CustomerGateways.Add(gateway);
                    }
                if (map.SupplierMapping != null)
                    foreach (var supplierMapping in map.SupplierMapping)
                    {
                        if (!SupplierDefinitions.ContainsKey(supplierMapping)) continue;
                        Gateway gateway = new Gateway { Mapping = supplierMapping };
                        string[] parts = supplierMapping.Split(':');
                        if (parts.Count() > 1)
                        {
                            gateway.Mapping = parts[0];
                            int percentage;
                            int.TryParse(parts[1], out percentage);
                            gateway.Percentage = percentage;
                        }
                        ivSwitchMapping.SupplierGateways.Add(gateway);
                    }
                IvSwitchMappings[mapItem.Key] = ivSwitchMapping;
            }
        }
        private IVSwitchRoute BuildBlockedRoute()
        {
            if (SupplierDefinitions != null && SupplierDefinitions.Count > 0)
            {
                return new IVSwitchRoute
                {
                    Description = "BLK",
                    RouteId = _blockRouteId
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

    public class PreparedRoute
    {
        public string RouteTableName { get; set; }
        public string TariffTableName { get; set; }
        public byte[] Routes { get; set; }
        public int RoutesCount { get; set; }
        public byte[] Tariffs { get; set; }
        public int TariffCount { get; set; }
    }
    public class CarrierDefinition
    {
        public string AccountId { get; set; }
        public string GroupId { get; set; }
        public string RouteTableName
        {
            get { return "rt" + RouteTableId; }
        }

        public string TariffTableName
        {
            get { return "trf" + TariffTableId; }
        }
        public int RouteTableId { get; set; }
        public int TariffTableId { get; set; }
    }
    public class CarrierMapping
    {
        public int CarrierId { get; set; }
        public List<string> CustomerMapping { get; set; }
        public List<string> SupplierMapping { get; set; }
        public string InnerPrefix { get; set; }
    }
    public class IvSwitchMapping
    {
        public int CarrierId { get; set; }
        public List<Gateway> CustomerGateways { get; set; }
        public List<Gateway> SupplierGateways { get; set; }
        public string InnerPrefix { get; set; }
    }
    public class Gateway
    {
        public string Mapping { get; set; }
        public int Percentage { get; set; }
    }
}
