using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Radius;

namespace TOne.WhS.RouteSync.TelesRadius
{
    public class TelesRadiusSWSync : SwitchRouteSynchronizer
    {
        public override Guid ConfigId { get { return new Guid("423064C2-ACE8-4D70-8CFF-CDAA1461DBBE"); } }

        public IRadiusDataManager DataManager { get; set; }
        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public string MappingSeparator { get; set; }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            this.DataManager.PrepareTables(context);
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            CarrierMapping carrierMapping;
            List<ConvertedRoute> radiusRoutes = new List<ConvertedRoute>();
            foreach (var route in context.Routes)
            {
                if (CarrierMappings.TryGetValue(route.CustomerId, out carrierMapping) && carrierMapping.CustomerMapping != null)
                {
                    foreach (string CustomerMapping in carrierMapping.CustomerMapping)
                    {
                        //    if (string.IsNullOrEmpty(CustomerMapping))
                        //        continue;
                        TelesRadiusConvertedRoute radiusRoute = new TelesRadiusConvertedRoute()
                        {
                            CustomerId = string.IsNullOrEmpty(CustomerMapping) ? "Customer_No_Mapping" : CustomerMapping,
                            Code = route.Code,
                            Options = GetRouteOption(route.Options)
                        };
                        radiusRoutes.Add(radiusRoute);
                    }
                }
            }
            context.ConvertedRoutes = radiusRoutes;
        }

        private string GetRouteOption(List<RouteOption> options)
        {
            StringBuilder strOptions = new StringBuilder();
            CarrierMapping carrierMapping;
            if (options != null)
                foreach (RouteOption routeOption in options)
                {
                    if (CarrierMappings.TryGetValue(routeOption.SupplierId, out carrierMapping) && carrierMapping.SupplierMapping != null)
                    {
                        foreach (string supplierMapping in carrierMapping.SupplierMapping)
                        {
                            if (string.IsNullOrEmpty(supplierMapping))
                                continue;
                            strOptions.AppendFormat("{0}{1}", supplierMapping, routeOption.Percentage);
                        }
                    }
                }
            return strOptions.Length == 0 ? "BLK" : strOptions.ToString();
        }

        public override Object PrepareDataForApply(ISwitchRouteSynchronizerPrepareDataForApplyContext context)
        {
            return this.DataManager.PrepareDataForApply(context.ConvertedRoutes);
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            SwapTableContext swapTableContext = new SwapTableContext() { WriteTrackingMessage = context.WriteTrackingMessage, SwitchName = context.SwitchName, IndexesCommandTimeoutInSeconds = context.IndexesCommandTimeoutInSeconds };
            this.DataManager.SwapTables(swapTableContext);
        }

        public override void ApplySwitchRouteSyncRoutes(ISwitchRouteSynchronizerApplyRoutesContext context)
        {
            this.DataManager.ApplySwitchRouteSyncRoutes(context);
        }

        public override void RemoveConnection(ISwitchRouteSynchronizerRemoveConnectionContext context)
        {
            this.DataManager = null;
        }
    }

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }

    }


}
