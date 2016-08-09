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
        CarrierAccountManager _carrierAccountManager;
        RoutingProductManager _routingProductManager;
        SaleZoneManager _saleZoneManager;

        public RPRouteManager()
        {
            _carrierAccountManager = new CarrierAccountManager();
            _routingProductManager = new RoutingProductManager();
            _saleZoneManager = new SaleZoneManager();
        }

        public Vanrise.Entities.IDataRetrievalResult<RPRouteDetail> GetFilteredRPRoutes(Vanrise.Entities.DataRetrievalInput<RPRouteQuery> input)
        {
            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();

            var routingDatabase = routingDatabaseManager.GetRoutingDatabase(input.Query.RoutingDatabaseId);

            if (routingDatabase == null)//in case of deleted database
                routingDatabase = routingDatabaseManager.GetRoutingDatabaseFromDB(input.Query.RoutingDatabaseId);

            if (routingDatabase == null)
                throw new NullReferenceException(string.Format("routingDatabase. RoutingDatabaseId:{0}", input.Query.RoutingDatabaseId));

            var productRoutingDatabases = routingDatabaseManager.GetRoutingDatabasesReady(routingDatabase.ProcessType, routingDatabase.Type).OrderByDescending(itm => itm.CreatedTime);
            manager.RoutingDatabase = productRoutingDatabases.First();

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
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(routingDatabaseId);

            IEnumerable<RPRoute> rpRoutes = dataManager.GetRPRoutes(rpZones);
            return rpRoutes.MapRecords(x => RPRouteDetailMapper(x, policyConfigId, numberOfOptions));
        }

        public RPRouteOptionSupplierDetail GetRPRouteOptionSupplier(int routingDatabaseId, int routingProductId, long saleZoneId, int supplierId)
        {
            IRPRouteDataManager routeManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            routeManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(routingDatabaseId);

            Dictionary<int, RPRouteOptionSupplier> dicRouteOptionSuppliers = routeManager.GetRouteOptionSuppliers(routingProductId, saleZoneId);

            if (dicRouteOptionSuppliers == null || !dicRouteOptionSuppliers.ContainsKey(supplierId))
                return null;


            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();

            return new RPRouteOptionSupplierDetail()
            {
                SupplierName = carrierAccountManager.GetCarrierAccountName(supplierId),
                SupplierZones = dicRouteOptionSuppliers[supplierId].SupplierZones.MapRecords(RPRouteOptionSupplierZoneDetailMapper)
            };
        }

        public Vanrise.Entities.IDataRetrievalResult<RPRouteOptionDetail> GetFilteredRPRouteOptions(Vanrise.Entities.DataRetrievalInput<RPRouteOptionQuery> input)
        {
            IRPRouteDataManager manager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            manager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(input.Query.RoutingDatabaseId);

            Dictionary<int, IEnumerable<RPRouteOption>> allOptions = manager.GetRouteOptions(input.Query.RoutingProductId, input.Query.SaleZoneId);
            if (allOptions == null || !allOptions.ContainsKey(input.Query.PolicyOptionConfigId))
                return null;

            IEnumerable<RPRouteOption> routeOptionsByPolicy = allOptions[input.Query.PolicyOptionConfigId];
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult<RPRouteOptionDetail>(input, routeOptionsByPolicy.ToBigResult(input, null, RPRouteOptionMapper));
        }

        public IEnumerable<RPRouteOptionPolicySetting> GetPoliciesOptionTemplates(RPRouteOptionPolicyFilter filter)
        {
            var extensionConfigManager = new ExtensionConfigurationManager();
            IEnumerable<RPRouteOptionPolicySetting> cachedConfigs = extensionConfigManager.GetExtensionConfigurations<RPRouteOptionPolicySetting>(Constants.SupplierZoneToRPOptionConfigType);

            if (filter == null)
                return cachedConfigs.OrderBy(x => x.Title);

            int defaultPolicyId;
            IEnumerable<int> selectedPolicyIds = this.GetRoutingDatabasePolicyIds(filter.RoutingDatabaseId, out defaultPolicyId);
            Func<RPRouteOptionPolicySetting, bool> filterExpression = (itm) => selectedPolicyIds.Contains(itm.ExtensionConfigurationId);

            IEnumerable<RPRouteOptionPolicySetting> cachedFilteredConfigs = cachedConfigs.FindAllRecords(filterExpression);
            
            if (cachedFilteredConfigs == null || cachedFilteredConfigs.Count() == 0)
                throw new NullReferenceException("cachedFilteredConfigs");

            List<RPRouteOptionPolicySetting> filteredConfigs = new List<RPRouteOptionPolicySetting>();

            // Set the default policy
            bool isDefaultPolicySet = false;
            foreach (RPRouteOptionPolicySetting config in cachedFilteredConfigs)
            {
                RPRouteOptionPolicySetting item = new RPRouteOptionPolicySetting() { 
                    BehaviorFQTN = config.BehaviorFQTN,
                    Editor = config.Editor,
                    ExtensionConfigurationId = config.ExtensionConfigurationId,
                    IsDefault= config.IsDefault,
                    Name = config.Name,
                    Title = config.Title
                };

                if (item.ExtensionConfigurationId == defaultPolicyId)
                {
                    item.IsDefault = true;
                    isDefaultPolicySet = true;
                }
                filteredConfigs.Add(item);
            }

            if (!isDefaultPolicySet)
                throw new DataIntegrityValidationException(String.Format("RPRoutingDatabase '{0}' does not have a default policy", filter.RoutingDatabaseId));

            return filteredConfigs.OrderBy(x => x.Title);
        }

        #region Private Members

        private RPRouteDetail RPRouteDetailMapper(RPRoute rpRoute, int policyConfigId, int numberOfOptions)
        {
            return new RPRouteDetail()
            {
                RoutingProductId = rpRoute.RoutingProductId,
                SaleZoneId = rpRoute.SaleZoneId,
                RoutingProductName = _routingProductManager.GetRoutingProductName(rpRoute.RoutingProductId),
                SaleZoneName = _saleZoneManager.GetSaleZoneName(rpRoute.SaleZoneId),
                IsBlocked = rpRoute.IsBlocked,
                RouteOptionsDetails = this.GetRouteOptionDetails(rpRoute.RPOptionsByPolicy, policyConfigId, numberOfOptions),
                ExecutedRuleId = rpRoute.ExecutedRuleId
            };
        }

        private RPRouteOptionDetail RPRouteOptionMapper(RPRouteOption routeOption)
        {
            if (routeOption == null)
                return null;

            return new RPRouteOptionDetail()
            {
                Entity = routeOption,
                SupplierName = _carrierAccountManager.GetCarrierAccountName(routeOption.SupplierId)
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

        private IEnumerable<RPRouteOptionDetail> GetRouteOptionDetails(Dictionary<int, IEnumerable<RPRouteOption>> dicRouteOptions, int policyConfigId, int numberOfOptions)
        {
            if (dicRouteOptions == null || !dicRouteOptions.ContainsKey(policyConfigId))
                return null;

            IEnumerable<RPRouteOption> routeOptionDetails = dicRouteOptions[policyConfigId].Take(numberOfOptions);
            return routeOptionDetails.MapRecords(RPRouteOptionMapper);
        }

        private IEnumerable<int> GetRoutingDatabasePolicyIds(int routingDbId, out int defaultPolicyId)
        {
            var routingDbManager = new RoutingDatabaseManager();
            var routingDbInfoFilter = new RoutingDatabaseInfoFilter() { ProcessType = RoutingProcessType.RoutingProductRoute };
            IEnumerable<RoutingDatabaseInfo> routingDbInfoEntities = routingDbManager.GetRoutingDatabaseInfo(routingDbInfoFilter);
            RoutingDatabaseInfo routingDbInfo = routingDbInfoEntities.FindRecord(itm => itm.RoutingDatabaseId == routingDbId);
            if (routingDbInfo == null)
                throw new NullReferenceException("routingDbInfo");
            if (routingDbInfo.Information == null)
                throw new NullReferenceException("routingDbInfo.Information");
            var rpRoutingDbInfo = routingDbInfo.Information as RPRoutingDatabaseInformation;
            if (rpRoutingDbInfo == null)
                throw new NullReferenceException("rpRoutingDbInfo");
            if (rpRoutingDbInfo.SelectedPoliciesIds == null || rpRoutingDbInfo.SelectedPoliciesIds.Count() == 0)
                throw new NullReferenceException("rpRoutingDbInfo.SelectedPoliciesIds");

            defaultPolicyId = rpRoutingDbInfo.DefaultPolicyId;
            return rpRoutingDbInfo.SelectedPoliciesIds;
        }

        #endregion
    }
}
