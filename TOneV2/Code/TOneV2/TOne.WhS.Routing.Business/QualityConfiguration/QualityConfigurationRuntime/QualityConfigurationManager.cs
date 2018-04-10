using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class QualityConfigurationManager
    {
        #region Public Methods

        public Dictionary<Guid, RouteRuleQualityConfiguration> GetRouteRuleQualityConfigurations()
        {
            ConfigManager configManager = new ConfigManager();
            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurations = configManager.GetQualityConfiguration().RouteRuleQualityConfigurationList;
            if (routeRuleQualityConfigurations == null)
                return null;

            return routeRuleQualityConfigurations.ToDictionary(itm => itm.QualityConfigurationId, itm => itm);
        }

        public List<RouteRuleQualityConfiguration> GetRouteRuleQualityConfigurationList(List<RouteRule> routeRules)
        {
            List<Guid> routeRuleQualityConfigurationIds = new List<Guid>();
            Guid defaultConfigurationId = GetDefaultQualityConfiguration().QualityConfigurationId;

            foreach (var routeRule in routeRules)
            {
                RouteRuleQualityContext qualityContext = new RouteRuleQualityContext();
                routeRule.Settings.GetQualityConfigurationIds(qualityContext);

                if (qualityContext.IsDefault)
                    routeRuleQualityConfigurationIds.Add(defaultConfigurationId);

                if (qualityContext.QualityConfigurationIds != null)
                    routeRuleQualityConfigurationIds.AddRange(qualityContext.QualityConfigurationIds);
            }

            if (routeRuleQualityConfigurationIds.Count == 0)
                return null;

            HashSet<Guid> distinctRouteRuleQualityConfigurationIds = routeRuleQualityConfigurationIds.ToHashSet();
            List<RouteRuleQualityConfiguration> routeRuleQualityConfigurationList = new List<RouteRuleQualityConfiguration>();

            foreach (var routeRuleQualityConfigurationId in distinctRouteRuleQualityConfigurationIds)
            {
                var routeRuleQualityConfiguration = GetQualityConfiguration(routeRuleQualityConfigurationId);
                if (routeRuleQualityConfiguration != null)
                    routeRuleQualityConfigurationList.Add(routeRuleQualityConfiguration);
            }

            return routeRuleQualityConfigurationList.Count > 0 ? routeRuleQualityConfigurationList : null;
        }

        public RouteRuleQualityConfiguration GetQualityConfiguration(Guid routeRuleQualityConfigurationId)
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = this.GetRouteRuleQualityConfigurations();
            if (qualityConfigurations == null)
                return null;

            return qualityConfigurations.GetRecord(routeRuleQualityConfigurationId);
        }

        public IEnumerable<QualityConfigurationInfo> GetQualityConfigurationInfo(QualityConfigurationInfoFilter filter)
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            List<QualityConfigurationInfo> qualityConfigurationInfoList = new List<QualityConfigurationInfo>();

            if (qualityConfigurations != null)
            {
                foreach (var qualityConfigurationKvp in qualityConfigurations)
                {
                    RouteRuleQualityConfiguration qualityConfiguration = qualityConfigurationKvp.Value;

                    QualityConfigurationInfo qualityConfigurationInfo = new QualityConfigurationInfo()
                    {
                        Name = qualityConfiguration.Name,
                        QualityConfigurationId = qualityConfiguration.QualityConfigurationId
                    };

                    qualityConfigurationInfoList.Add(qualityConfigurationInfo);
                }
            }
            return qualityConfigurationInfoList;
        }

        public bool ValidateRouteRuleQualityConfiguration(RouteRuleQualityConfiguration routeRuleQualityConfiguration)
        {
            if (routeRuleQualityConfiguration == null || routeRuleQualityConfiguration.Settings == null)
                return false;

            ValidateQualityConfigurationDataContext context = new ValidateQualityConfigurationDataContext()
            {
                QualityConfigurationDefinitionId = routeRuleQualityConfiguration.QualityConfigurationDefinitionId,
                QualityConfigurationName = routeRuleQualityConfiguration.Name
            };
            return routeRuleQualityConfiguration.Settings.ValidateRouteRuleQualityConfigurationSettings(context);
        }

        private struct GetCachedCustomerRouteQualityConfigurationDataCacheName
        {
            public int RoutingDatabaseId { get; set; }

            public override int GetHashCode()
            {
                return this.RoutingDatabaseId.GetHashCode();
            }
        }

        public Dictionary<long, List<CustomerRouteQualityConfigurationData>> GetCachedCustomerRouteQualityConfigurationData(RoutingDatabase routingDatabase)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QualityConfigurationDataCacheManager>();
            var cacheName = new GetCachedCustomerRouteQualityConfigurationDataCacheName() { RoutingDatabaseId = routingDatabase.ID };

            return cacheManager.GetOrCreateObject(cacheName, QualityConfigurationDataCacheExpirationChecker.Instance,
                () =>
                {
                    ICustomerQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
                    dataManager.RoutingDatabase = routingDatabase;

                    IEnumerable<CustomerRouteQualityConfigurationData> customerRouteQualityConfigurationsData = dataManager.GetCustomerRouteQualityConfigurationsData();
                    if (customerRouteQualityConfigurationsData == null)
                        return null;

                    Dictionary<long, List<CustomerRouteQualityConfigurationData>> results = new Dictionary<long, List<CustomerRouteQualityConfigurationData>>();

                    foreach (var itm in customerRouteQualityConfigurationsData)
                    {
                        List<CustomerRouteQualityConfigurationData> supplierZoneQualityConfigurationData = results.GetOrCreateItem(itm.SupplierZoneId);
                        supplierZoneQualityConfigurationData.Add(itm);
                    }

                    return results.Count > 0 ? results : null;
                });
        }

        private struct GetCachedRPQualityConfigurationDataCacheName
        {
            public int RoutingDatabaseId { get; set; }

            public override int GetHashCode()
            {
                return this.RoutingDatabaseId.GetHashCode();
            }
        }

        public Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>> GetCachedRPQualityConfigurationData(RoutingDatabase routingDatabase)
        {
            var cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<QualityConfigurationDataCacheManager>();
            var cacheName = new GetCachedRPQualityConfigurationDataCacheName() { RoutingDatabaseId = routingDatabase.ID };

            return cacheManager.GetOrCreateObject(cacheName, QualityConfigurationDataCacheExpirationChecker.Instance,
                () =>
                {
                    IRPQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPQualityConfigurationDataManager>();
                    dataManager.RoutingDatabase = routingDatabase;

                    IEnumerable<RPQualityConfigurationData> rpQualityConfigurationsData = dataManager.GetRPQualityConfigurationData();
                    if (rpQualityConfigurationsData == null)
                        return null;

                    Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>> results = new Dictionary<SaleZoneSupplier, List<RPQualityConfigurationData>>();

                    foreach (var itm in rpQualityConfigurationsData)
                    {
                        SaleZoneSupplier saleZoneSupplier = new SaleZoneSupplier() { SaleZoneId = itm.SaleZoneId, SupplierId = itm.SupplierId };
                        List<RPQualityConfigurationData> saleZoneSupplierQualityConfigurationData = results.GetOrCreateItem(saleZoneSupplier);
                        saleZoneSupplierQualityConfigurationData.Add(itm);
                    }

                    return results.Count > 0 ? results : null;
                });
        }

        #endregion

        #region Private Methods

        private RouteRuleQualityConfiguration GetDefaultQualityConfiguration()
        {
            Dictionary<Guid, RouteRuleQualityConfiguration> qualityConfigurations = GetRouteRuleQualityConfigurations();
            if (qualityConfigurations != null)
            {
                foreach (var routeRuleQualityConfigurationKvp in qualityConfigurations)
                {
                    var routeRuleQualityConfiguration = routeRuleQualityConfigurationKvp.Value;
                    if (routeRuleQualityConfiguration.IsDefault)
                    {
                        if (!routeRuleQualityConfiguration.IsActive)
                            throw new VRBusinessException("Default quality configuration is not active");

                        return routeRuleQualityConfiguration;
                    }
                }
            }

            throw new VRBusinessException("No default quality configuration is selected");
        }

        #endregion

        #region Private Classes

        private class QualityConfigurationDataCacheManager : BaseCacheManager
        {

        }

        private class QualityConfigurationDataCacheExpirationChecker : Vanrise.Caching.CacheExpirationChecker
        {
            static QualityConfigurationDataCacheExpirationChecker s_instance = new QualityConfigurationDataCacheExpirationChecker();
            public static QualityConfigurationDataCacheExpirationChecker Instance { get { return s_instance; } }

            public override bool IsCacheExpired(Vanrise.Caching.ICacheExpirationCheckerContext context)
            {
                TimeSpan entitiesTimeSpan = TimeSpan.FromMinutes(15);
                SlidingWindowCacheExpirationChecker slidingWindowCacheExpirationChecker = new SlidingWindowCacheExpirationChecker(entitiesTimeSpan);
                return slidingWindowCacheExpirationChecker.IsCacheExpired(context);
            }
        }

        #endregion
    }

    public class QualityConfigurationCacheManager : BaseCacheManager
    {
        RouteRuleManager _routeRuleManager = new RouteRuleManager();

        DateTime? _routeRuleCacheLastCheck;
        DateTime? _settingsCacheLastCheck;

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _routeRuleManager.IsCacheExpired(ref _routeRuleCacheLastCheck)
                    | Vanrise.Caching.CacheManagerFactory.GetCacheManager<Vanrise.Common.Business.SettingManager.CacheManager>().IsCacheExpired(ref _settingsCacheLastCheck);
        }
    }
}