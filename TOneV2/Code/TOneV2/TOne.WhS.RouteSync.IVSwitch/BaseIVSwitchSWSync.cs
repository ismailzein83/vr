using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public abstract class BaseIVSwitchSWSync : SwitchRouteSynchronizer
    {
        #region properties
        public string OwnerName { get; set; }
        public string MasterConnectionString { get; set; }
        public string RouteConnectionString { get; set; }
        public string TariffConnectionString { get; set; }
        public int NumberOfOptions { get; set; }
        public string BlockedAccountMapping { get; set; }
        public Guid Uid { get; set; }

        #endregion
        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            PreparedConfiguration preparedData = GetPreparedConfiguration();
            BuildTempTables(preparedData);
        }
        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null)
                return;
            PreparedConfiguration preparedData = GetPreparedConfiguration();
            List<ConvertedRoute> routes = new List<ConvertedRoute>();
            foreach (var route in context.Routes)
            {
                CustomerDefinition customerDefiniton;
                if (preparedData.CustomerDefinitions.TryGetValue(route.CustomerId, out customerDefiniton))
                {
                    foreach (var elt in customerDefiniton.EndPoints)
                    {
                        routes.Add(BuildRouteAndRouteOptions(route, elt, preparedData));
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
            PreparedConfiguration preparedData = GetPreparedConfiguration();
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            IVSwitchTariffDataManager tariffDataManager = new IVSwitchTariffDataManager(TariffConnectionString, OwnerName);
            foreach (var customerTable in preparedData.CustomerDefinitions)
            {
                foreach (var gateway in customerTable.Value.EndPoints)
                {
                    routeDataManager.CreatePrimaryKey(string.Format("rt{0}", gateway.RouteTableId));
                    routeDataManager.Swap(string.Format("rt{0}", gateway.RouteTableId));
                    tariffDataManager.Swap(string.Format("trf{0}", gateway.TariffTableId));
                }
            }
        }
        public abstract PreparedConfiguration GetPreparedConfiguration();

        #region private functions
        private void BuildTempTables(PreparedConfiguration preparedData)
        {
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            IVSwitchTariffDataManager tariffDataManager = new IVSwitchTariffDataManager(TariffConnectionString, OwnerName);
            foreach (var customerTable in preparedData.CustomerDefinitions)
            {
                foreach (var gateway in customerTable.Value.EndPoints)
                {
                    routeDataManager.BuildRouteTable(string.Format("rt{0}", gateway.RouteTableId));
                    tariffDataManager.BuildTariffTable(string.Format("trf{0}", gateway.RouteTableId));
                }
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
        private IVSwitchConvertedRoute BuildRouteAndRouteOptions(Route route, EndPoint endPoint, PreparedConfiguration preparedData)
        {
            if (route == null)
                return null;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            IVSwitchConvertedRoute ivSwitch = new IVSwitchConvertedRoute
            {
                Routes = new List<IVSwitchRoute>(),
                Tariffs = new List<IVSwitchTariff>(),
                RouteTableName = string.Format("rt{0}", endPoint.RouteTableId),
                TariffTableName = string.Format("trf{0}", endPoint.TariffTableId)
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

            int priority = NumberOfOptions;
            List<IVSwitchRoute> routes = new List<IVSwitchRoute>();
            int gatewayCount = route.Options.Count;
            foreach (var option in route.Options)
            {
                int serial = 1;
                SupplierDefinition supplier;
                if (preparedData.SupplierDefinitions.TryGetValue(option.SupplierId, out supplier) && supplier.Gateways != null)
                {
                    foreach (var supplierGateWay in supplier.Gateways)
                    {
                        if (priority == 0) break;
                        string name = carrierAccountManager.GetCarrierAccountName(int.Parse(option.SupplierId));
                        IVSwitchRoute ivOption = new IVSwitchRoute
                        {
                            Destination = route.Code,
                            RouteId = supplierGateWay.RouteId,
                            TimeFrame = "* * * * *",
                            Preference = priority,
                            StateId = 1,
                            HuntStop = 0,
                            WakeUpTime = DateTime.UtcNow,
                            Description = name
                        };
                        if (supplierGateWay.Percentage != 0)
                        {
                            ivOption.RoutingMode = 8;
                            ivOption.TotalBkts = gatewayCount;
                            ivOption.Flag1 =
                                BuildPercentage(supplierGateWay.Percentage, option.Percentage, optionsPercenatgeSum,
                                    supplier.Gateways.Count);
                            ivOption.BktSerial = serial++;
                            ivOption.BktCapacity = decimal.ToInt32(BuildScaledDownPercentage(ivOption.Flag1 ?? 0, 1,
                                maxPercentage ?? 0, 1, optionsPercenatgeSum ?? 0));
                            ivOption.BktToken = ivOption.BktCapacity;
                        }
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

        public class PreparedRoute
        {
            public string RouteTableName { get; set; }
            public string TariffTableName { get; set; }
            public List<IVSwitchRoute> Routes { get; set; }
            public List<IVSwitchTariff> Tariffs { get; set; }
        }
    }
}
