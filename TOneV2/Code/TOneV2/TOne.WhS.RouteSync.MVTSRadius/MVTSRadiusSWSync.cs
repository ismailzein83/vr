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
            this.DataManager.PrepareTables();
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
            context.InvalidRoutes = invalidRoutes.Count > 0 ? invalidRoutes : null;
            context.ConvertedRoutes = radiusRoutes;
        }

        private List<MVTSRadiusOption> BuildOptions(Route route, List<string> invalidRoutes)
        {
            if (route == null || route.Options == null)
                return null;

            int priority = NumberOfOptions;

            CarrierMapping carrierMapping;
            List<MVTSRadiusOption> options = new List<MVTSRadiusOption>();
            StringBuilder strOptions = new StringBuilder();

            MVTSRouteSyncOptions roundedOptions = BuildRoundedPercentages(route, invalidRoutes);

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
                            Percentage = roundedOptions[routeOption.SupplierId],
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

        private MVTSRouteSyncOptions BuildRoundedPercentages(Route route, List<string> invalidRoutes)
        {
            MVTSRouteSyncOptions mvtsRouteSyncOptions = new MVTSRouteSyncOptions();
            int roundedPercentages = 0;
            foreach (RouteOption routeOption in route.Options)
            {
                int? roundedPercentage = null;
                if (routeOption.Percentage.HasValue)
                {
                    roundedPercentage = Convert.ToInt32(routeOption.Percentage.Value);
                    roundedPercentages += roundedPercentage.Value;
                }
                if (!mvtsRouteSyncOptions.ContainsKey(routeOption.SupplierId))
                    mvtsRouteSyncOptions.Add(routeOption.SupplierId, roundedPercentage);
                else
                {
                    invalidRoutes.Add(string.Format("Customer {0} and Code {1} have the supplier {2} more than once", route.CustomerId, route.Code, routeOption.SupplierId));
                }
            }
            if (roundedPercentages > 0 && roundedPercentages < 100)
            {
                var firstItem = mvtsRouteSyncOptions.First();
                mvtsRouteSyncOptions[firstItem.Key] = firstItem.Value.Value + (100 - roundedPercentages);
            }

            return mvtsRouteSyncOptions;
        }

        public override void UpdateConvertedRoutes(ISwitchRouteSynchronizerUpdateConvertedRoutesContext context)
        {
            this.DataManager.InsertRoutes(context.ConvertedRoutes);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            this.DataManager.SwapTables();
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
