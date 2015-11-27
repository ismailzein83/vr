using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class RPRouteManager
    {
        public Vanrise.Entities.IDataRetrievalResult<RPRouteDetail> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<RPRouteQuery> input)
        {
            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            manager.DatabaseId = input.Query.RoutingDatabaseId;

            BigResult<RPRoute> rpRouteResult = manager.GetFilteredRPRoutes(input);

            BigResult<RPRouteDetail> customerRouteDetailResult = new BigResult<RPRouteDetail>()
            {
                ResultKey = rpRouteResult.ResultKey,
                TotalCount = rpRouteResult.TotalCount,
                Data = rpRouteResult.Data.MapRecords(RPRouteDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
        }

        private RPRouteDetail RPRouteDetailMapper(RPRoute rpRoute)
        {
            return new RPRouteDetail()
            {
                Entity = rpRoute,
                RoutingProductName = this.GetRoutingProductName(rpRoute.RoutingProductId),
                SaleZoneName = this.GetSaleZoneName(rpRoute.SaleZoneId)
            };
        }

        private string GetRoutingProductName(int routingProductId)
        {
            RoutingProductManager manager = new RoutingProductManager();
            RoutingProduct routingProduct = manager.GetRoutingProduct(routingProductId);

            if (routingProduct != null)
                return routingProduct.Name;

            return "Not Found";
        }

        private string GetSaleZoneName(long saleZoneId)
        {
            SaleZoneManager manager = new SaleZoneManager();
            SaleZone saleZone = manager.GetSaleZone(saleZoneId);

            if (saleZone != null)
                return saleZone.Name;

            return "Not Found";
        }

        public IEnumerable<RPRoute> GetRPRoutes(IEnumerable<RPZone> rpZones)
        {
            List<RPRoute> rslt = new List<RPRoute>();
            foreach(var rpZone in rpZones)
            {
                RPRoute route = new RPRoute
                {
                    RoutingProductId = rpZone.RoutingProductId,
                    SaleZoneId = rpZone.SaleZoneId,
                    OptionsDetailsBySupplier = new Dictionary<int, RPRouteOptionSupplier>(),
                    RPOptionsByPolicy = new Dictionary<int, IEnumerable<RPRouteOption>>()
                };
                var optionSupplier = new RPRouteOptionSupplier
                    {
                        SupplierId = 1,                        
                        SupplierZones = new List<RPRouteOptionSupplierZone>()
                    };
                optionSupplier.SupplierZones.Add(new RPRouteOptionSupplierZone
                    {
                        SupplierZoneId = 1190,
                        SupplierCode = "444",
                        SupplierRate = 0.2m
                    });
                optionSupplier.SupplierZones.Add(new RPRouteOptionSupplierZone
                {
                    SupplierZoneId = 1191,
                    SupplierCode = "43563",
                    SupplierRate = 0.3m
                });
                route.OptionsDetailsBySupplier.Add(optionSupplier.SupplierId, optionSupplier);

                optionSupplier = new RPRouteOptionSupplier
                {
                    SupplierId = 38,
                    SupplierZones = new List<RPRouteOptionSupplierZone>()
                };
                optionSupplier.SupplierZones.Add(new RPRouteOptionSupplierZone
                {
                    SupplierZoneId = 1320,
                    SupplierCode = "645",
                    SupplierRate = 0.5m
                });
                optionSupplier.SupplierZones.Add(new RPRouteOptionSupplierZone
                {
                    SupplierZoneId = 1321,
                    SupplierCode = "345",
                    SupplierRate = 0.75m
                });
                route.OptionsDetailsBySupplier.Add(optionSupplier.SupplierId, optionSupplier);


                optionSupplier = new RPRouteOptionSupplier
                {
                    SupplierId = 40,
                    SupplierZones = new List<RPRouteOptionSupplierZone>()
                };
                optionSupplier.SupplierZones.Add(new RPRouteOptionSupplierZone
                {
                    SupplierZoneId = 1329,
                    SupplierCode = "3345",
                    SupplierRate = 0.32m
                });
                optionSupplier.SupplierZones.Add(new RPRouteOptionSupplierZone
                {
                    SupplierZoneId = 1333,
                    SupplierCode = "645",
                    SupplierRate = 0.53m
                });
                route.OptionsDetailsBySupplier.Add(optionSupplier.SupplierId, optionSupplier);

                List<RPRouteOption> options = new List<RPRouteOption>();
                options.Add(new RPRouteOption
                {
                    SupplierId = 38,
                    SupplierRate = 0.3m,
                    Percentage = 50
                });
                options.Add(new RPRouteOption
                {
                    SupplierId = 1,
                    SupplierRate = 0.32m,
                    Percentage = 30
                });
                options.Add(new RPRouteOption
                {
                    SupplierId = 40,
                    SupplierRate = 0.39m,
                    Percentage = 20
                });
                route.RPOptionsByPolicy.Add(1, options);
                rslt.Add(route);
            }
            return rslt;
        }

        public RPRouteOptionSupplier GetRPRouteSupplierOption(int routingProductId, long saleZoneId, int supplierId)
        {
            return null;
        }
    }

    public class RPZone
    {
        public int RoutingProductId { get; set; }

        public long SaleZoneId { get; set; }
    }
}
