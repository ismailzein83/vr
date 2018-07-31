using System;
using System.Collections.Generic;
using System.Text;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Radius;
using System.Linq;

namespace TOne.WhS.RouteSync.MVTSRadius
{
    public class MVTSRadiusSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("6D6EB5E6-E1F6-49FF-A78F-E3A255FBC320"); } }

        public IRadiusDataManager DataManager { get; set; }

        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public int NumberOfOptions { get; set; }

        public string MappingSeparator { get; set; }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            this.DataManager.PrepareTables(context);
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            if (context.Routes == null || CarrierMappings == null)
                return;

            List<string> invalidRoutes = new List<string>();
            CarrierMapping carrierMapping;
            List<ConvertedRoute> radiusRoutes = new List<ConvertedRoute>();
            foreach (var route in context.Routes)
            {
                if (CarrierMappings.TryGetValue(route.CustomerId, out carrierMapping))
                {
                    if (carrierMapping.CustomerMapping == null)
                        continue;

                    List<MVTSRadiusOption> options = BuildOptions(route, invalidRoutes);
                    string radiusRouteOptions = options != null && options.Count > 0 ? String.Join("|", options.Select(itm => itm.Option)) : "BLK";

                    foreach (string CustomerMapping in carrierMapping.CustomerMapping)
                    {
                        if (string.IsNullOrEmpty(CustomerMapping))
                            continue;

                        MVTSRadiusConvertedRoute radiusRoute = new MVTSRadiusConvertedRoute()
                        {
                            CustomerId = CustomerMapping,
                            Code = route.Code,
                            Options = radiusRouteOptions,
                            MVTSRadiusOptions = options
                        };
                        radiusRoutes.Add(radiusRoute);
                    }
                }
            }
            context.ConvertedRoutes = radiusRoutes;
        }

        private List<MVTSRadiusOption> BuildOptions(Route route, List<string> invalidRoutes)
        {
            if (route == null || route.Options == null || route.Options.Count == 0)
                return null;

            int priority = NumberOfOptions;

            CarrierMapping carrierMapping;
            List<MVTSRadiusOption> options = new List<MVTSRadiusOption>();
            StringBuilder strOptions = new StringBuilder();

            foreach (RouteOption routeOption in route.Options)
            {
                if (priority == 0)
                    break;

                if (CarrierMappings.TryGetValue(routeOption.SupplierId, out carrierMapping) && carrierMapping.SupplierMapping != null)
                {
                    foreach (string supplierMapping in carrierMapping.SupplierMapping)
                    {
                        if (string.IsNullOrEmpty(supplierMapping))
                            continue;

                        MVTSRadiusOption option = new MVTSRadiusOption()
                        {
                            Option = supplierMapping,
                            //Percentage = roundedOptions[routeOption.SupplierId],
                            Percentage = routeOption.Percentage,
                            Priority = priority
                        };
                        options.Add(option);
                        priority--;
                        if (priority == 0)
                            break;
                    }
                }
            }

            return options;
        }

        public override Object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            return this.DataManager.PrepareDataForApply(context.ConvertedRoutes);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            this.DataManager.ApplySwitchRouteSyncRoutes(context);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            SwapTableContext swapTableContext = new SwapTableContext()
            {
                WriteTrackingMessage = context.WriteTrackingMessage,
                SwitchName = context.SwitchName,
                IndexesCommandTimeoutInSeconds = context.IndexesCommandTimeoutInSeconds,
                SwitchId = context.SwitchId,
                PreviousSwitchSyncOutput = context.PreviousSwitchSyncOutput,
                WriteBusinessHandledException = context.WriteBusinessHandledException
            };
            this.DataManager.SwapTables(swapTableContext);
            context.SwitchSyncOutput = swapTableContext.SwitchSyncOutput;
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            this.DataManager = null;
        }
        public class CarrierMapping
        {
            public int CarrierId { get; set; }

            public List<string> CustomerMapping { get; set; }

            public List<string> SupplierMapping { get; set; }

        }

        private class MVTSRouteSyncOptions : Dictionary<string, int?>
        {
        }
    }
}
