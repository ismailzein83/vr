using NP.IVSwitch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class BuiltInConfigBuilder
    {
        private IVSwitchMasterDataManager MasterDataManager;
        private string BlockedMapping;
        public BuiltInConfigBuilder(BuiltInIVSwitchSWSync sync)
        {
            MasterDataManager = new IVSwitchMasterDataManager(sync.MasterConnectionString);
            BlockedMapping = sync.BlockedAccountMapping;
        }
        private Dictionary<int, AccessListTable> GetCustomersGroupedByUser()
        {
            Dictionary<int, AccessListTable> customerDictionary = new Dictionary<int, AccessListTable>();
            List<AccessListTable> customersLst = MasterDataManager.GetAccessListTables();
            foreach (var customer in customersLst)
            {
                if (!customerDictionary.ContainsKey(customer.UserId))
                    customerDictionary[customer.UserId] = customer;
            }
            return customerDictionary;
        }
        public PreparedConfiguration Build()
        {
            HashSet<int> routeTableIds;
            Dictionary<string, CustomerDefinition> customerDefinitions = GetConfiguredCustomers(out routeTableIds);
            PreparedConfiguration prepredConfiguration = new PreparedConfiguration
            {
                CustomerDefinitions = customerDefinitions,
                SupplierDefinitions = GetConfiguredSuppliers(),
                _switchTime = MasterDataManager.GetSwitchDate(),
                RouteTableIdsHashSet = routeTableIds
            };
            int id;
            int.TryParse(BlockedMapping, out id);
            prepredConfiguration.BlockRouteId = id;
            return prepredConfiguration;
        }

        public Dictionary<string, CustomerDefinition> GetConfiguredCustomers(out HashSet<int> routeTableIds)
        {
            routeTableIds = new HashSet<int>();
            Dictionary<int, AccessListTable> dataBaseCustomers = GetCustomersGroupedByUser();
            var customers = new Dictionary<string, CustomerDefinition>();
            var carrierAccountManager = new CarrierAccountManager();
            var allCustomers = carrierAccountManager.GetAllCustomers();

            foreach (var customer in allCustomers)
            {
                EndPointCarrierAccountExtension ivCustomer = carrierAccountManager.GetExtendedSettings<EndPointCarrierAccountExtension>(customer);

                if (ivCustomer == null || ivCustomer.AclEndPointInfo == null)
                    continue;

                CustomerDefinition definition = new CustomerDefinition
                {
                    CustomerId = customer.CarrierAccountId.ToString(),
                    EndPoints = new List<EndPoint>()
                };
                Dictionary<int, EndPoint> endPointsById = new Dictionary<int, EndPoint>();
                foreach (var elt in ivCustomer.AclEndPointInfo)
                {
                    AccessListTable access;
                    if (dataBaseCustomers.TryGetValue(elt.EndPointId, out access))
                    {
                        EndPoint endPoint = new EndPoint
                        {
                            TariffTableId = access.TariffTableId,
                            RouteTableId = access.RouteTableId
                        };
                        if (!endPointsById.ContainsKey(endPoint.RouteTableId))
                        {
                            endPointsById.Add(endPoint.RouteTableId, endPoint);
                            routeTableIds.Add(endPoint.RouteTableId);
                        }
                    }
                }
                definition.EndPoints = endPointsById.Values.ToList();
                customers.Add(definition.CustomerId, definition);
            }
            return customers;
        }

        public Dictionary<string, SupplierDefinition> GetConfiguredSuppliers()
        {
            Dictionary<string, SupplierDefinition> suppliers = new Dictionary<string, SupplierDefinition>();
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var allSuppliers = carrierAccountManager.GetAllSuppliers();
            foreach (var supplier in allSuppliers)
            {
                RouteCarrierAccountExtension ivVendor =
                    carrierAccountManager.GetExtendedSettings<RouteCarrierAccountExtension>(supplier);
                if (ivVendor == null) continue;
                SupplierDefinition definition = new SupplierDefinition
                {
                    SupplierId = supplier.CarrierAccountId.ToString(),
                    Gateways = new List<GateWay>()
                };
                foreach (var elt in ivVendor.RouteInfo)
                {
                    GateWay gateway = new GateWay
                    {
                        RouteId = elt.RouteId,
                        Percentage = elt.Percentage
                    };
                    definition.Gateways.Add(gateway);
                }
                suppliers[definition.SupplierId] = definition;
            }
            return suppliers;

        }

    }
}
