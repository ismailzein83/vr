using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Caching;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class PreparedConfiguration
    {
        public Dictionary<string, CustomerDefinition> CustomerDefinitions { get; set; }
        public Dictionary<string, CarrierDefinition> SupplierDefinitions { get; set; }
        public List<CustomerDefinition> CustomerTables { get; set; }
        public int BlockRouteId;
        public Dictionary<string, IvSwitchMapping> IvSwitchMappings { get; set; }

        private static PreparedConfiguration GetConfiguration(IVSwitchSWSync sync)
        {
            IVSwitchMasterDataManager masterDataManager = new IVSwitchMasterDataManager(sync.MasterConnectionString);
            PreparedConfiguration preparedConfiguration = new PreparedConfiguration
            {
                CustomerTables = new List<CustomerDefinition>(),
                CustomerDefinitions = masterDataManager.GetCustomers(),
                SupplierDefinitions = masterDataManager.GetSuppliers()
            };

            CarrierDefinition blockDefinition;
            if (preparedConfiguration.SupplierDefinitions.TryGetValue(sync.BlockedAccountMapping, out blockDefinition) && blockDefinition.RouteTableId.HasValue)
                preparedConfiguration.BlockRouteId = blockDefinition.RouteTableId.Value;
            preparedConfiguration.BuildIvSwitchMapping(sync);
            return preparedConfiguration;
        }
        private struct GetCachedPreparedConfigurationCacheName
        {
            public Guid Uid { get; set; }
        }

        public static PreparedConfiguration GetCachedPreparedConfiguration(IVSwitchSWSync sync)
        {
            var cacheName = new GetCachedPreparedConfigurationCacheName { Uid = sync.Uid };
            var cacheManager = CacheManagerFactory.GetCacheManager<ConfigurationCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
                () => GetConfiguration(sync));
        }

        private void BuildIvSwitchMapping(IVSwitchSWSync sync)
        {
            IvSwitchMappings = new Dictionary<string, IvSwitchMapping>();
            foreach (var mapItem in sync.CarrierMappings)
            {
                var map = mapItem.Value;
                if (map.CustomerMapping == null && map.SupplierMapping == null) continue;
                IvSwitchMapping ivSwitchMapping = new IvSwitchMapping
                {
                    CarrierId = map.CarrierId,
                    InnerPrefix = map.InnerPrefix,
                    CustomerMapping = new List<string>(),
                    SupplierGateways = new List<Gateway>()
                };
                if (map.CustomerMapping != null)
                    foreach (var customerMapping in map.CustomerMapping)
                    {
                        CustomerDefinition definition;
                        if (!CustomerDefinitions.TryGetValue(customerMapping, out definition)) continue;
                        CustomerTables.Add(definition);
                        ivSwitchMapping.CustomerMapping.Add(customerMapping);
                    }
                if (map.SupplierMapping != null)
                    foreach (var supplierMapping in map.SupplierMapping)
                    {
                        if (!SupplierDefinitions.ContainsKey(supplierMapping)) continue;
                        Gateway gateway = new Gateway { Mapping = supplierMapping };
                        string[] parts = supplierMapping.Split(':');
                        if (parts.Length > 1)
                        {
                            gateway.Mapping = parts[0];
                            int percentage;
                            int.TryParse(parts[1], out percentage);
                            gateway.Percentage = percentage;
                        }
                        ivSwitchMapping.SupplierGateways.Add(gateway);
                    }
                IvSwitchMappings[mapItem.Key] = ivSwitchMapping;
            }
        }
    }

    public class PreparedRoute
    {
        public string RouteTableName { get; set; }
        public string TariffTableName { get; set; }
        public List<IVSwitchRoute> Routes { get; set; }
        public List<IVSwitchTariff> Tariffs { get; set; }
    }
    public class CarrierDefinition
    {
        public string AccountId { get; set; }
        public string GroupId { get; set; }
        public int? RouteTableId { get; set; }
    }

    public class CustomerDefinition : CarrierDefinition
    {
        public int TariffTableId { get; set; }
        public string RouteTableName
        {
            get { return string.Format("rt{0}", RouteTableId); }
        }
        public string TariffTableName
        {
            get { return String.Format("trf{0}", TariffTableId); }
        }
    }
    public class CarrierMapping
    {
        public string CarrierId { get; set; }
        public List<string> CustomerMapping { get; set; }
        public List<string> SupplierMapping { get; set; }
        public string InnerPrefix { get; set; }
    }
    public class IvSwitchMapping
    {
        public string CarrierId { get; set; }
        public List<string> CustomerMapping { get; set; }
        public List<Gateway> SupplierGateways { get; set; }
        public string InnerPrefix { get; set; }
    }
    public class Gateway
    {
        public string Mapping { get; set; }
        public int Percentage { get; set; }
    }
}
