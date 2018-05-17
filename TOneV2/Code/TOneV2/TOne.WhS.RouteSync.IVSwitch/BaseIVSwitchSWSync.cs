using NP.IVSwitch.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.BusinessEntity.Business;

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
            var routes = new List<ConvertedRoute>();

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
            Dictionary<string, PreparedRoute> customerRoutes = new Dictionary<string, PreparedRoute>();
            foreach (var convertedRoute in context.ConvertedRoutes)
            {
                IVSwitchConvertedRoute ivSwitchConvertedRoute = (IVSwitchConvertedRoute)convertedRoute;

                PreparedRoute preparedRoute;
                if (!customerRoutes.TryGetValue(ivSwitchConvertedRoute.RouteTableName, out preparedRoute))
                {
                    preparedRoute = new PreparedRoute
                    {
                        RouteTableName = ivSwitchConvertedRoute.RouteTableName,
                        Routes = new List<IVSwitchRoute>()
                    };
                    customerRoutes.Add(ivSwitchConvertedRoute.RouteTableName, preparedRoute);
                }
                preparedRoute.Routes.AddRange(ivSwitchConvertedRoute.Routes);

            }
            return customerRoutes;
        }
        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            Dictionary<string, PreparedRoute> routes = (Dictionary<string, PreparedRoute>)context.PreparedItemsForApply;
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            foreach (var item in routes.Values)
            {
                if (item.Routes != null && item.Routes.Any())
                    routeDataManager.Bulk(item.Routes, string.Format("{0}_temp", item.RouteTableName));
            }
        }
        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            PreparedConfiguration preparedData = GetPreparedConfiguration();
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            foreach (var routeTableId in preparedData.RouteTableIdsHashSet)
            {
                routeDataManager.CreatePrimaryKey(string.Format("rt{0}_temp", routeTableId));
                routeDataManager.Swap(GetRouteTableName(routeTableId));
                context.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Table rt'{0}' for switch '{1}' is finalized", new object[] { routeTableId, context.SwitchName });
            }
        }
        public abstract PreparedConfiguration GetPreparedConfiguration();
        public abstract List<EndPointStatus> PrepareEndPointStatus(string carrierId, List<int> endPointstatuses);
        public override bool TryBlockCustomer(ITryBlockCustomerContext context)
        {
            IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
            List<int> endPointStatus = new List<int> { (int)State.Active, (int)State.Dormant };
            List<EndPointStatus> endPointStatuses = PrepareEndPointStatus(context.CustomerId, endPointStatus);
            if (endPointStatuses == null || !endPointStatuses.Any()) return false;
            context.SwitchBlockingInfo = endPointStatuses;
            return masterDataManager.BlockEndPoints(endPointStatuses.Select(it => it.EndPointId), "access_list", (int)State.Suspended);
        }

        public override bool TryUnBlockCustomer(ITryUnBlockCustomerContext context)
        {
            var unblockContext = context as TryUnBlockCustomerContext;
            if (unblockContext == null) return false;

            var endPointBlockingInfos = unblockContext.SwitchBlockingInfo as List<EndPointStatus>;
            if (endPointBlockingInfos == null) return false;

            List<int> endPointStatus = new List<int> { (int)State.Suspended };
            List<EndPointStatus> switchEndPointStatuses = PrepareEndPointStatus(context.CustomerId, endPointStatus);
            if (switchEndPointStatuses == null || !switchEndPointStatuses.Any()) return false;

            List<EndPointStatus> suspendedEndPoints = GetSuspendedEndPoints(endPointBlockingInfos, switchEndPointStatuses);
            IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(MasterConnectionString);
            return masterDataManager.UpdateEndPointState(suspendedEndPoints);
        }

        private string GetRouteTableName(int routeTableId)
        {
            return string.Format("rt{0}", routeTableId);
        }
        private string GetTariffTableName(int tariffTableId)
        {
            return string.Format("trf{0}", tariffTableId);
        }

        #region private functions

        private List<EndPointStatus> GetSuspendedEndPoints(List<EndPointStatus> endPointBlockingInfo, List<EndPointStatus> switchEndPointstatuses)
        {
            List<EndPointStatus> suspendedList = new List<EndPointStatus>();
            Dictionary<int, EndPointStatus> endPointStatuseByEndPointId = switchEndPointstatuses.ToDictionary(item => item.EndPointId, item => item);
            foreach (var currentEndPointStatus in endPointBlockingInfo)
            {
                EndPointStatus switchEndPointStatus;
                if (endPointStatuseByEndPointId.TryGetValue(currentEndPointStatus.EndPointId, out switchEndPointStatus))
                {
                    if (switchEndPointStatus.Status == State.Suspended)
                        suspendedList.Add(currentEndPointStatus);
                }
            }
            return suspendedList;
        }
        private void BuildTempTables(PreparedConfiguration preparedData)
        {
            IVSwitchRouteDataManager routeDataManager = new IVSwitchRouteDataManager(RouteConnectionString, OwnerName);
            foreach (var routeTableId in preparedData.RouteTableIdsHashSet)
            {
                routeDataManager.BuildRouteTable(GetRouteTableName(routeTableId));
            }
        }
        private IVSwitchTariff BuildTariff(Entities.Route route)
        {
            IVSwitchTariff tariff = new IVSwitchTariff
            {
                DestinationCode = route.Code,
                TimeFrame = "* * * * *"
            };
            if (route.SaleRate.HasValue)
                tariff.InitCharge = Math.Round(route.SaleRate.Value, 4);

            if (route.SaleZoneId.HasValue)
                tariff.DestinationName = new SaleZoneManager().GetSaleZoneName(route.SaleZoneId.Value);

            return tariff;
        }
        private IVSwitchConvertedRoute BuildRouteAndRouteOptions(Entities.Route route, EndPoint endPoint, PreparedConfiguration preparedData)
        {
            if (route == null)
                return null;

            if (preparedData.SupplierDefinitions == null)
                return null;

            var ivSwitch = new IVSwitchConvertedRoute
            {
                CustomerID = route.CustomerId,
                Routes = new List<IVSwitchRoute>(),
                Tariffs = new List<IVSwitchTariff>(),
                RouteTableName = GetRouteTableName(endPoint.RouteTableId),
                TariffTableName = GetTariffTableName(endPoint.TariffTableId)
            };
            int priority = 0;
            if (route.Options == null || route.Options.Count == 0)
            {
                ivSwitch.Routes.Add(BuildBlockedRoute(preparedData.BlockRouteId, preparedData._switchTime, route.Code, ref priority));
                return ivSwitch;
            }

            var nonBlockedOptions = route.Options.Where(r => !r.IsBlocked);

            decimal? optionsPercenatgeSum = 0;
            decimal? maxPercentage = 0;
            int gatewayCount = 0;
            priority = NumberOfOptions;

            foreach (var option in nonBlockedOptions)
            {
                optionsPercenatgeSum += option.Percentage;
                if (option.Percentage.HasValue && option.Percentage.Value > maxPercentage)
                    maxPercentage = option.Percentage;
            }
            int totalBkt = 0;
            int serial = 1;
            var routes = new List<IVSwitchRoute>();
            foreach (var option in nonBlockedOptions)
            {
                SupplierDefinition supplier;
                if (preparedData.SupplierDefinitions.TryGetValue(option.SupplierId, out supplier) && supplier.Gateways != null)
                {
                    foreach (var supplierGateWay in supplier.Gateways)
                    {
                        if (priority == 0)
                            break;

                        totalBkt++;
                        var ivOption = new IVSwitchRoute
                        {
                            Destination = route.Code,
                            RouteId = supplierGateWay.RouteId,
                            Preference = priority--,
                            StateId = 1,
                            HuntStop = 0,
                            WakeUpTime = preparedData._switchTime,
                            Description = new CarrierAccountManager().GetCarrierAccountName(int.Parse(option.SupplierId))
                        };
                        if (option.Percentage != null && option.Percentage != 0)
                        {
                            ivOption.RoutingMode = 8;
                            ivOption.Flag1 = BuildPercentage(supplierGateWay.Percentage, option.Percentage, optionsPercenatgeSum,
                                    supplier.Gateways.Count);
                            ivOption.BktSerial = serial++;
                            ivOption.BktCapacity = (int)Math.Ceiling(ivOption.Flag1.Value / 10);
                            ivOption.BktToken = ivOption.BktCapacity;
                        }
                        routes.Add(ivOption);
                    }
                    routes.AddRange(GetBackupRoutes(route.Code, preparedData._switchTime, preparedData.BlockRouteId, option.Backups, preparedData.SupplierDefinitions, ref priority, ref serial));
                }
                int totalCount = routes.Count;
                routes.ForEach(r => r.TotalBkts = totalCount);
                if (option.Percentage != null && option.Percentage > 0)
                {
                    if (routes.Any())
                        routes.Last().HuntStop = 1;
                }
            }
            ivSwitch.Routes.AddRange(routes);
            return ivSwitch;
        }

        private List<IVSwitchRoute> GetBackupRoutes(string code, DateTime wakeUpTime, int blockedRouteId, List<BackupRouteOption> backups, Dictionary<string, SupplierDefinition> suppliersDefinition, ref int priority, ref int serial)
        {
            var backupRoutes = new List<IVSwitchRoute>();

            if (backups == null)
                return backupRoutes;

            foreach (var backupRouteOption in backups)
            {
                if (priority == 0)
                    break;

                if (backupRouteOption.IsBlocked)
                    backupRoutes.Add(BuildBlockedRoute(blockedRouteId, wakeUpTime, code, ref priority));

                else
                {
                    SupplierDefinition supplier;
                    if (suppliersDefinition.TryGetValue(backupRouteOption.SupplierId, out supplier) && supplier.Gateways != null)
                    {
                        foreach (var supplierGateWay in supplier.Gateways)
                        {
                            backupRoutes.Add(new IVSwitchRoute
                            {
                                RoutingMode = 1,
                                Destination = code,
                                RouteId = supplierGateWay.RouteId,
                                TimeFrame = "* * * * *",
                                Preference = priority--,
                                BktSerial = serial++,
                                StateId = 1,
                                HuntStop = 0,
                                WakeUpTime = wakeUpTime,
                                Description = new CarrierAccountManager().GetCarrierAccountName(int.Parse(backupRouteOption.SupplierId))
                            });
                        }
                    }
                }
            }
            return backupRoutes;
        }

        private IVSwitchRoute BuildBlockedRoute(int blockedRouteId, DateTime switchDate, string code, ref int priority)
        {
            return new IVSwitchRoute
             {
                 Description = "BLK",
                 RouteId = blockedRouteId,
                 WakeUpTime = switchDate,
                 Destination = code,
                 Preference = priority--
             };
        }

        #endregion

        #region Percentage Routing
        private int BuildScaledDownPercentage(decimal? initialPercentage, decimal z1, decimal? maxPercentage, decimal y1, decimal? optionsPercenatgeSum)
        {
            decimal optionsPercenatgeSumTemp = optionsPercenatgeSum ?? 0;
            decimal maxPercentageTemp = maxPercentage ?? 0;
            decimal initialPercentageTemp = initialPercentage ?? 0;
            var scaledDownPercentage = Math.Ceiling(z1 * (1 - ((initialPercentageTemp - y1) / (optionsPercenatgeSumTemp - y1))) + (maxPercentageTemp * ((initialPercentageTemp - y1) / (optionsPercenatgeSumTemp - y1))));
            return decimal.ToInt32(scaledDownPercentage);
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
