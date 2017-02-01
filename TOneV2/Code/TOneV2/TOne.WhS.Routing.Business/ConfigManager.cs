using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.Business
{
    public class ConfigManager
    {
        #region public methods
        public int GetCustomerRouteIndexesCommandTimeoutInSeconds()
        {
            return GetCustomerRouteBuildConfiguration().IndexesCommandTimeoutInMinutes * 60;
        }

        public int GetProductRouteIndexesCommandTimeoutInSeconds()
        {
            return GetProductRouteBuildConfiguration().IndexesCommandTimeoutInMinutes * 60;
        }

        public int GetCustomerRouteBuildNumberOfOptions()
        {
            return GetCustomerRouteBuildConfiguration().NumberOfOptions;
        }

        public bool GetCustomerRouteBuildAddBlockedOptions()
        {
            return GetCustomerRouteBuildConfiguration().AddBlockedOptions;
        }

        public bool GetProductRouteBuildAddBlockedOptions()
        {
            return GetProductRouteBuildConfiguration().AddBlockedOptions;
        }

        public IncludedRulesConfiguration GetIncludedRulesConfiguration()
        {
            RouteBuildConfiguration routeBuildConfiguration = GetRouteBuildConfiguration();

            if (routeBuildConfiguration.IncludedRules == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration.IncludedRules");

            return routeBuildConfiguration.IncludedRules;
        }

        public Dictionary<Guid, RouteOptionRuleTypeConfiguration> GetRouteOptionRuleTypeConfigurationForCustomerRoutes()
        {
            CustomerRouteOptionRuleTypeConfiguration customerRouteOptionRuleTypeConfiguration = GetCustomerRouteOptionRuleTypeConfiguration();

            if (customerRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration == null)
                throw new NullReferenceException("customerRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration");

            return customerRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration;
        }

        public Dictionary<Guid, RouteOptionRuleTypeConfiguration> GetRouteOptionRuleTypeConfigurationForProductRoutes()
        {
            ProductRouteOptionRuleTypeConfiguration productRouteOptionRuleTypeConfiguration = GetProductRouteOptionRuleTypeConfiguration();

            if (productRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration == null)
                throw new NullReferenceException("productRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration");

            return productRouteOptionRuleTypeConfiguration.RouteOptionRuleTypeConfiguration;
        }


        public SubProcessSettings GetSubProcessSettings()
        {
            RouteSettingsData routeSettingsData = GetRouteSettingData();

            if (routeSettingsData.SubProcessSettings == null)
                throw new NullReferenceException("routeSettingsData.SubProcessSettings");

            return routeSettingsData.SubProcessSettings;
        }

        public Guid GetSupplierTransformationId()
        {
            RouteRuleDataTransformation routeRuleDataTransformation = GetRouteRuleDataTransformation();
            return routeRuleDataTransformation.SupplierTransformationId;
        }

        public Guid GetCustomerTransformationId()
        {
            RouteRuleDataTransformation routeRuleDataTransformation = GetRouteRuleDataTransformation();
            return routeRuleDataTransformation.CustomerTransformationId;
        }

        public RouteDatabaseConfiguration GetCustomerRouteConfiguration()
        {
            RouteDatabasesToKeep routeDatabasesToKeep = GetRouteDatabasesToKeep();
            RouteDatabaseConfiguration customerRouteDatabaseConfiguration = routeDatabasesToKeep.CustomerRouteConfiguration;

            if (customerRouteDatabaseConfiguration == null)
                throw new NullReferenceException("customerRouteDatabaseConfiguration");

            return customerRouteDatabaseConfiguration;
        }

        public RouteDatabaseConfiguration GetProductRouteConfiguration()
        {
            RouteDatabasesToKeep routeDatabasesToKeep = GetRouteDatabasesToKeep();
            RouteDatabaseConfiguration productRouteDatabaseConfiguration = routeDatabasesToKeep.ProductRouteConfiguration;

            if (productRouteDatabaseConfiguration == null)
                throw new NullReferenceException("productRouteDatabaseConfiguration");

            return productRouteDatabaseConfiguration;
        }

        #endregion

        #region private methods

        private RouteRuleDataTransformation GetRouteRuleDataTransformation()
        {
            RouteTechnicalSettingData routeTechnicalSettingData = GetRouteTechnicalSettingData();

            if (routeTechnicalSettingData.RouteRuleDataTransformation == null)
                throw new NullReferenceException("routeTechnicalSettingData.RouteRuleDataTransformation");

            return routeTechnicalSettingData.RouteRuleDataTransformation;
        }

        private RouteTechnicalSettingData GetRouteTechnicalSettingData()
        {
            SettingManager settingManager = new SettingManager();
            RouteTechnicalSettingData routeTechnicalSettingData = settingManager.GetSetting<RouteTechnicalSettingData>(Constants.RouteTechnicalSettings);

            if (routeTechnicalSettingData == null)
                throw new NullReferenceException("routeTechnicalSettingData");

            return routeTechnicalSettingData;
        }

        private RouteSettingsData GetRouteSettingData()
        {
            SettingManager settingManager = new SettingManager();
            RouteSettingsData routeSettingsData = settingManager.GetSetting<RouteSettingsData>(Constants.RouteSettings);

            if (routeSettingsData == null)
                throw new NullReferenceException("routeSettingsData");

            return routeSettingsData;
        }

        private RouteDatabasesToKeep GetRouteDatabasesToKeep()
        {
            RouteSettingsData routeSettingsData = GetRouteSettingData();

            if (routeSettingsData.RouteDatabasesToKeep == null)
                throw new NullReferenceException("routeSettingsData.RouteDatabasesToKeep");

            return routeSettingsData.RouteDatabasesToKeep;
        }

        private RouteBuildConfiguration GetRouteBuildConfiguration()
        {
            RouteSettingsData routeSettingsData = GetRouteSettingData();

            if (routeSettingsData.RouteBuildConfiguration == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration");

            return routeSettingsData.RouteBuildConfiguration;
        }

        private CustomerRouteBuildConfiguration GetCustomerRouteBuildConfiguration()
        {
            RouteBuildConfiguration routeBuildConfiguration = GetRouteBuildConfiguration();

            if (routeBuildConfiguration.CustomerRoute == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration.CustomerRoute");

            return routeBuildConfiguration.CustomerRoute;
        }

        private ProductRouteBuildConfiguration GetProductRouteBuildConfiguration()
        {
            RouteBuildConfiguration routeBuildConfiguration = GetRouteBuildConfiguration();

            if (routeBuildConfiguration.ProductRoute == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration.ProductRoute");

            return routeBuildConfiguration.ProductRoute;
        }

        private RouteOptionRuleConfiguration GetRouteOptionRuleConfiguration()
        {
            RouteSettingsData routeSettingsData = GetRouteSettingData();

            if (routeSettingsData.RouteOptionRuleConfiguration == null)
                throw new NullReferenceException("routeSettingsData.RouteOptionRuleConfiguration");

            return routeSettingsData.RouteOptionRuleConfiguration;
        }

        private CustomerRouteOptionRuleTypeConfiguration GetCustomerRouteOptionRuleTypeConfiguration()
        {
            RouteOptionRuleConfiguration routeOptionRuleConfiguration = GetRouteOptionRuleConfiguration();

            if (routeOptionRuleConfiguration.CustomerRouteOptionRuleTypeConfiguration == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration.CustomerRouteOptionRuleTypeConfiguration");

            return routeOptionRuleConfiguration.CustomerRouteOptionRuleTypeConfiguration;
        }

        private ProductRouteOptionRuleTypeConfiguration GetProductRouteOptionRuleTypeConfiguration()
        {
            RouteOptionRuleConfiguration routeOptionRuleConfiguration = GetRouteOptionRuleConfiguration();

            if (routeOptionRuleConfiguration.ProductRouteOptionRuleTypeConfiguration == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration.ProductRouteOptionRuleTypeConfiguration");

            return routeOptionRuleConfiguration.ProductRouteOptionRuleTypeConfiguration;
        }

        #endregion
    }
}