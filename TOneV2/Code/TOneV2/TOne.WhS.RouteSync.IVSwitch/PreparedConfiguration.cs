using System;
using System.Collections.Generic;
using System.Text;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Caching;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class PreparedConfiguration
    {
        public Dictionary<string, CustomerDefinition> CustomerDefinitions { get; set; }
        public Dictionary<string, SupplierDefinition> SupplierDefinitions { get; set; }
        public HashSet<int> RouteTableIdsHashSet { get; set; }

        public int BlockRouteId;
        public DateTime _switchTime;

        private static PreparedConfiguration BuildBuiltInConfiguration(BuiltInIVSwitchSWSync sync)
        {
            BuiltInConfigBuilder builder = new BuiltInConfigBuilder(sync);
            return builder.Build();
        }
        private static PreparedConfiguration BuildConfiguration(IVSwitchSWSync sync)
        {
            ConfigBuilder builder = new ConfigBuilder(sync);
            return builder.Build();
        }

        #region Caching
        private struct GetCachedPreparedConfigurationCacheName
        {
            public Guid Uid { get; set; }
        }

        public static PreparedConfiguration GetCachedPreparedConfiguration(IVSwitchSWSync sync)
        {
            var cacheName = new GetCachedPreparedConfigurationCacheName { Uid = sync.Uid };
            var cacheManager = CacheManagerFactory.GetCacheManager<ConfigurationCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
                () => BuildConfiguration(sync));
        }
        public static PreparedConfiguration GetBuiltInCachedPreparedConfiguration(BuiltInIVSwitchSWSync sync)
        {
            var cacheName = new GetCachedPreparedConfigurationCacheName { Uid = sync.Uid };
            var cacheManager = CacheManagerFactory.GetCacheManager<ConfigurationCacheManager>();
            return cacheManager.GetOrCreateObject(cacheName,
                () => BuildBuiltInConfiguration(sync));
        }

        #endregion

    }
    #region public classes

    public class EndPoint
    {
        public int RouteTableId { get; set; }
        public int TariffTableId { get; set; }
    }
    public class GateWay
    {
        public int RouteId { get; set; }
        public decimal Percentage { get; set; }
    }
    public class SupplierDefinition
    {
        public string SupplierId { get; set; }
        public List<GateWay> Gateways { get; set; }
    }

    public class CustomerDefinition
    {
        public string CustomerId { get; set; }
        public List<EndPoint> EndPoints { get; set; }
    }

    public class AccessListTable
    {
        public int UserId { get; set; }
        public string AccountId { get; set; }
        public string GroupId { get; set; }
        public int RouteTableId { get; set; }
        public int TariffTableId { get; set; }
    }

    public class RouteTable
    {
        public string AccountId { get; set; }
        public string GroupId { get; set; }
        public int RouteId { get; set; }
    }
    #endregion
}
