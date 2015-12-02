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
using Vanrise.Common.Business;

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
                Data = rpRouteResult.Data.MapRecords(x => RPRouteDetailMapper(x, input.Query.PolicyConfigId, input.Query.NumberOfOptions))
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, customerRouteDetailResult);
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

        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId)
        {
            IRPRouteDataManager routeManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            routeManager.DatabaseId = routingDatabaseId;

            Dictionary<int, RPRouteOptionSupplier> dicRouteOptionSuppliers = routeManager.GetRouteOptionSuppliers(routingProductId, saleZoneId);

            if (dicRouteOptionSuppliers == null || !dicRouteOptionSuppliers.ContainsKey(supplierId))
                return null;


            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount supplier = carrierAccountManager.GetCarrierAccount(supplierId);

            return new RPRouteOptionSupplierDetail()
            {
                SupplierName = (supplier != null) ? supplier.Name : null,
                SupplierZones = dicRouteOptionSuppliers[supplierId].SupplierZones.MapRecords(RPRouteOptionSupplierZoneDetailMapper)
            };
        }

        public IEnumerable<RPRouteOptionDetail> GetRouteOptionDetails(int routingDatabaseId, int policyOptionConfigId, int routingProductId, long saleZoneId)
        {
            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            manager.DatabaseId = routingDatabaseId;

            Dictionary<int, IEnumerable<RPRouteOption>> allOptions = manager.GetRouteOptions(routingProductId, saleZoneId);
            if (allOptions == null || !allOptions.ContainsKey(policyOptionConfigId))
                return null;

            IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[policyOptionConfigId];
            return routeOptionsByPolicy.MapRecords(RPRouteOptionMapper);
        }

        public IEnumerable<Vanrise.Entities.TemplateConfig> GetPoliciesOptionTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SupplierZoneToRPOptionConfigType);
        }

        #region Private Memebers

        private RPRouteDetail RPRouteDetailMapper(RPRoute rpRoute, int policyConfigId, int numberOfOptions)
        {
            return new RPRouteDetail()
            {
                RoutingProductId = rpRoute.RoutingProductId,
                SaleZoneId = rpRoute.SaleZoneId,
                RoutingProductName = this.GetRoutingProductName(rpRoute.RoutingProductId),
                SaleZoneName = this.GetSaleZoneName(rpRoute.SaleZoneId),
                IsBlocked = rpRoute.IsBlocked,
                RouteOptionsDetails = this.GetRouteOptionDetails(rpRoute.RPOptionsByPolicy, policyConfigId, numberOfOptions)
            };
        }

        private RPRouteOptionDetail RPRouteOptionMapper(RPRouteOption routeOption)
        {
            if (routeOption == null)
                return null;

            return new RPRouteOptionDetail()
            {
                Entity = routeOption,
                SupplierName = GetSupplierName(routeOption.SupplierId)
            };
        }

        private RPRouteOptionSupplierZoneDetail RPRouteOptionSupplierZoneDetailMapper(RPRouteOptionSupplierZone rpRouteOptionSupplierZone)
        {
            SupplierZoneManager manager = new SupplierZoneManager();
            SupplierZone supplierZone = manager.GetSupplierZone(rpRouteOptionSupplierZone.SupplierZoneId);

            return new RPRouteOptionSupplierZoneDetail()
            {
                Entity = rpRouteOptionSupplierZone,
                SupplierZoneName = supplierZone != null ? supplierZone.Name : null
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

        private string GetSupplierName(int supplierId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccount supplier = manager.GetCarrierAccount(supplierId);

            if (supplier != null)
                return supplier.Name;

            return "Not Found";
        }

        private IEnumerable<RPRouteOptionDetail> GetRouteOptionDetails(Dictionary<int, IEnumerable<RPRouteOption>> dicRouteOptions, int policyConfigId, int numberOfOptions)
        {
            if (dicRouteOptions == null || !dicRouteOptions.ContainsKey(policyConfigId))
                return null;

            IEnumerable<RPRouteOption> routeOptionDetails = dicRouteOptions[policyConfigId].Take(numberOfOptions);
            return routeOptionDetails.MapRecords(RPRouteOptionMapper);
        }
        

        #endregion
    }

    public class RPZone
    {
        public int RoutingProductId { get; set; }

        public long SaleZoneId { get; set; }
    }
}
