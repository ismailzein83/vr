using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var customerByMapping= new Dictionary<string, AccessListTable>();
            List<AccessListTable> customersLst = MasterDataManager.GetAccessListTables();
            foreach (var customer in customersLst)
            {
                string key = string.Format("{0}_{1}", customer.AccountId, customer.GroupId);
                if (!customerByMapping.ContainsKey(key))
                   customerByMapping.Add(key,customer);
            }
            return customerByMapping;
        }
        private Dictionary<string, RouteTable> GetSuppliers()
        {
            Dictionary<string, RouteTable> supplierDictionary = new Dictionary<string, RouteTable>();
            List<RouteTable> supplierList = MasterDataManager.GetRouteTables();
            foreach (var supplier in supplierList)
            {
                string key = string.Format("{0}_{1}", supplier.AccountId, supplier.GroupId);
                if (!supplierDictionary.ContainsKey(key))
                    supplierDictionary[key] = supplier;
            }
            return supplierDictionary;
        }
        public PreparedConfiguration Build()
        {
            PreparedConfiguration preparedConfiguration = new PreparedConfiguration
            {
                CustomerDefinitions = new Dictionary<string, CustomerDefinition>(),
                SupplierDefinitions = new Dictionary<string, SupplierDefinition>(),
                _switchTime = MasterDataManager.GetSwitchDate()
            };
            Dictionary<string, string> routeTableIds = RouteDataManager.GetAccessListTableNames();
            Dictionary<string, string> tariffTableIds = TariffDataManager.GetRoutesTableNames();
            Dictionary<string, AccessListTable> dataBaseEndPoints = GetCustomers();
            Dictionary<string, RouteTable> dataBaseVendors = GetSuppliers();

            foreach (var mapItem in _sync.CarrierMappings)
            {
                var map = mapItem.Value;
                if (map.CustomerMapping == null && map.SupplierMapping == null) continue;

                if (map.CustomerMapping != null)
                    foreach (var customerMapping in map.CustomerMapping)
                    {
                        AccessListTable definition;
                        if (!dataBaseEndPoints.TryGetValue(customerMapping, out definition)) continue;
                        if (!routeTableIds.ContainsKey(string.Format("rt{0}", definition.RouteTableId))) continue;
                        if (!tariffTableIds.ContainsKey(string.Format("trf{0}", definition.TariffTableId))) continue;
                        EndPoint endPoint = new EndPoint
                        {
                            RouteTableId = definition.RouteTableId,
                            TariffTableId = definition.TariffTableId
                        };
                        CustomerDefinition customer;
                        if (!preparedConfiguration.CustomerDefinitions.TryGetValue(map.CarrierId, out customer))
                        {
                            customer = new CustomerDefinition
                            {
                                EndPoints = new List<EndPoint>(),
                                CustomerId = map.CarrierId
                            };
                            preparedConfiguration.CustomerDefinitions[map.CarrierId] = customer;
                        }
                        customer.EndPoints.Add(endPoint);
                    }
                if (map.SupplierMapping != null)
                    foreach (var supplierMapping in map.SupplierMapping)
                    {
                        RouteTable supplierDefinition;
                        if (!dataBaseVendors.TryGetValue(supplierMapping, out supplierDefinition)) continue;
                        GateWay gateway = new GateWay
                        {
                            RouteId = supplierDefinition.RouteId
                        };
                        string[] parts = supplierMapping.Split(':');
                        if (parts.Length > 1)
                        {
                            int percentage;
                            int.TryParse(parts[1], out percentage);
                            gateway.Percentage = percentage;
                        }
                        SupplierDefinition supplier;
                        if (!preparedConfiguration.SupplierDefinitions.TryGetValue(map.CarrierId, out supplier))
                        {
                            supplier = new SupplierDefinition
                            {
                                SupplierId = map.CarrierId,
                                Gateways = new List<GateWay>()
                            };
                            preparedConfiguration.SupplierDefinitions[map.CarrierId] = supplier;
                        }
                        supplier.Gateways.Add(gateway);
                    }
            }
            RouteTable blockDefinition;
            if (dataBaseVendors.TryGetValue(_sync.BlockedAccountMapping, out blockDefinition))
                preparedConfiguration.BlockRouteId = blockDefinition.RouteId;
            return preparedConfiguration;
        }

    }
}
