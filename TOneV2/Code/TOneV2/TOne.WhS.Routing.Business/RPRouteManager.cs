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

        public IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<RPZone> rpZones)
        {
            IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            dataManager.DatabaseId = routingDatabaseId;

            IEnumerable<RPRoute> rpRoutes = dataManager.GetRPRoutes(rpZones);
            return rpRoutes.MapRecords(x => RPRouteDetailMapper(x, policyConfigId, numberOfOptions));
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

        public Vanrise.Entities.IDataRetrievalResult<RPRouteOptionDetail> GetFilteredRPRouteOptions(Vanrise.Entities.DataRetrievalInput<RPRouteOptionQuery> input)
        {
            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            manager.DatabaseId = input.Query.RoutingDatabaseId;

            Dictionary<int, IEnumerable<RPRouteOption>> allOptions = manager.GetRouteOptions(input.Query.RoutingProductId, input.Query.SaleZoneId);
            if (allOptions == null || !allOptions.ContainsKey(input.Query.PolicyOptionConfigId))
                return null;

            IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[input.Query.PolicyOptionConfigId];
            return routeOptionsByPolicy.ToBigResult(input, null, RPRouteOptionMapper);
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

            return null;
        }

        private string GetSaleZoneName(long saleZoneId)
        {
            SaleZoneManager manager = new SaleZoneManager();
            SaleZone saleZone = manager.GetSaleZone(saleZoneId);

            if (saleZone != null)
                return saleZone.Name;

            return null;
        }

        private string GetSupplierName(int supplierId)
        {
            CarrierAccountManager manager = new CarrierAccountManager();
            CarrierAccount supplier = manager.GetCarrierAccount(supplierId);

            if (supplier != null)
                return supplier.Name;

            return null;
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
}
