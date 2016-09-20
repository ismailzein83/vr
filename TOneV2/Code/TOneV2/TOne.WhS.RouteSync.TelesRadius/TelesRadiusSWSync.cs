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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("423064C2-ACE8-4D70-8CFF-CDAA1461DBBE"); } }

        public IRadiusDataManager DataManager { get; set; }
        /// <summary>
        /// Key = Carrier Account Id
        /// </summary>
        public Dictionary<string, CarrierMapping> CarrierMappings { get; set; }

        public string MappingSeparator { get; set; }

        public override void Initialize(ISwitchRouteSynchronizerInitializeContext context)
        {
            this.DataManager.PrepareTables();
        }

        public override void ConvertRoutes(ISwitchRouteSynchronizerConvertRoutesContext context)
        {
            CarrierMapping carrierMapping;
            List<ConvertedRoute> radiusRoutes = new List<ConvertedRoute>();
            foreach (var route in context.Routes)
            {
                if (CarrierMappings.TryGetValue(route.CustomerId, out carrierMapping))
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
                    if (CarrierMappings.TryGetValue(routeOption.SupplierId, out carrierMapping))
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

        public override void UpdateConvertedRoutes(ISwitchRouteSynchronizerUpdateConvertedRoutesContext context)
        {
            this.DataManager.InsertRoutes(context.ConvertedRoutes);
            //throw new NotImplementedException();
        }

        public override void Finalize(ISwitchRouteSynchronizerFinalizeContext context)
        {
            this.DataManager.SwapTables();
        }
    }

    public class CarrierMapping
    {
        public int CarrierId { get; set; }

        public List<string> CustomerMapping { get; set; }

        public List<string> SupplierMapping { get; set; }

    }


}
