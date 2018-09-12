using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NP.IVSwitch.Entities;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class ConfigBuilder
    {
        private IVSwitchSWSync _sync;
        private IVSwitchMasterDataManager MasterDataManager;
        private IVSwitchRouteDataManager RouteDataManager;
        private IVSwitchTariffDataManager TariffDataManager;
        private string BlockedMapping;
        public ConfigBuilder(IVSwitchSWSync sync)
        {
            MasterDataManager = new IVSwitchMasterDataManager(sync.MasterConnectionString);
            RouteDataManager = new IVSwitchRouteDataManager(sync.RouteConnectionString, sync.OwnerName);
            TariffDataManager = new IVSwitchTariffDataManager(sync.TariffConnectionString, sync.OwnerName);
            _sync = sync;
        }
        private Dictionary<string, AccessListTable> GetCustomers()
        {
            var customerByMapping = new Dictionary<string, AccessListTable>();
            List<AccessListTable> customersLst = MasterDataManager.GetAccessListTables();
            foreach (var customer in customersLst)
            {
                string key = string.Format("{0}_{1}", customer.AccountId, customer.GroupId);
                if (!customerByMapping.ContainsKey(key))
                    customerByMapping.Add(key, customer);
            }
            return customerByMapping;
        }
        private Dictionary<string, RouteTable> GetSuppliers()
        {
            Dictionary<string, RouteTable> supplierDictionary = new Dictionary<string, RouteTable>();
            List<RouteTable> supplierList = MasterDataManager.GetRouteTables(new List<int> { (int)State.Active, (int)State.Dormant });
            foreach (var supplier in supplierList)
            {
                string key = string.Format("{0}_{1}", supplier.AccountId, supplier.GroupId);
                if (!supplierDictionary.ContainsKey(key))
                    supplierDictionary.Add(key,supplier);
            }
            return supplierDictionary;
        }
        public PreparedConfiguration Build()
        {
            PreparedConfiguration preparedConfiguration = new PreparedConfiguration
            {
                CustomerDefinitions = new Dictionary<string, CustomerDefinition>(),
                SupplierDefinitions = new Dictionary<string, SupplierDefinition>(),
                RouteTableIdsHashSet = new HashSet<int>(),
                _switchTime = MasterDataManager.GetSwitchDate()
            };
            Dictionary<string, string> routeTableIds = RouteDataManager.GetAccessListTableNames();
            Dictionary<string, AccessListTable> dataBaseEndPoints = GetCustomers();
            Dictionary<string, RouteTable> dataBaseVendors = GetSuppliers();

            foreach (var mapItem in _sync.CarrierMappings)
            {
                var map = mapItem.Value;
                if (map.CustomerMapping == null && map.SupplierMapping == null)
                    continue;

                if (map.CustomerMapping != null)
                {
                    var customerDefinitions = BuildCustomerDefinition(map.CarrierId, map.CustomerMapping, routeTableIds, dataBaseEndPoints);
                    if (customerDefinitions != null)
                    {
                        preparedConfiguration.CustomerDefinitions.Add(map.CarrierId, customerDefinitions);
                        customerDefinitions.EndPoints.ForEach(item => preparedConfiguration.RouteTableIdsHashSet.Add(item.RouteTableId));
                    }
                }

                if (map.SupplierMapping != null)
                    foreach (var supplierMapping in map.SupplierMapping)
                    {
                        decimal? percentagePart = null;
                        string supplierMappingValue = supplierMapping;
                        string[] mappingWithPercentage = supplierMapping.Split(':');
                        if (mappingWithPercentage.Length > 1)
                        {
                            supplierMappingValue = mappingWithPercentage[0];
                            int percentage;
                            if (int.TryParse(mappingWithPercentage[1], out percentage))
                                percentagePart = percentage;
                        }

                        RouteTable supplierDefinition;
                        if (!dataBaseVendors.TryGetValue(supplierMappingValue, out supplierDefinition))
                            continue;

                        GateWay gateway = new GateWay
                        {
                            RouteId = supplierDefinition.RouteId
                        };
                        if (percentagePart.HasValue)
                            gateway.Percentage = percentagePart.Value;

                        SupplierDefinition supplier;
                        if (!preparedConfiguration.SupplierDefinitions.TryGetValue(map.CarrierId, out supplier))
                        {
                            supplier = new SupplierDefinition
                            {
                                SupplierId = map.CarrierId,
                                Gateways = new List<GateWay>()
                            };
                            preparedConfiguration.SupplierDefinitions.Add(map.CarrierId, supplier);
                        }
                        supplier.Gateways.Add(gateway);
                    }
            }
            RouteTable blockDefinition;
            if (dataBaseVendors.TryGetValue(_sync.BlockedAccountMapping, out blockDefinition))
                preparedConfiguration.BlockRouteId = blockDefinition.RouteId;
            return preparedConfiguration;
        }

        private CustomerDefinition BuildCustomerDefinition(string customerId, List<string> customerMappings, Dictionary<string, string> routeTableIds, Dictionary<string, AccessListTable> dataBaseEndPoints)
        {
            CustomerDefinition customerDefinition = null;
            var endPointById = new Dictionary<int, EndPoint>();
            foreach (var customerMapping in customerMappings)
            {
                AccessListTable definition;
                if (!dataBaseEndPoints.TryGetValue(customerMapping, out definition))
                    continue;

                if (!routeTableIds.ContainsKey(string.Format("rt{0}", definition.RouteTableId)))
                    continue;

                EndPoint endPoint;
                if (!endPointById.TryGetValue(definition.RouteTableId, out endPoint))
                {
                    endPoint = new EndPoint
                    {
                        RouteTableId = definition.RouteTableId,
                        TariffTableId = definition.TariffTableId
                    };
                    endPointById.Add(definition.RouteTableId, endPoint);
                }
            }

            if (endPointById.Any())
            {
                return new CustomerDefinition
                {
                    CustomerId = customerId,
                    EndPoints = endPointById.Values.ToList()
                };
            }
            return null;
        }
    }
}
