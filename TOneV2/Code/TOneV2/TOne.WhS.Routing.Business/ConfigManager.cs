using System;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Routing.Business
{
    public class ConfigManager 
    {
        #region public methods
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

        private RouteBuildConfiguration GetRouteBuildConfiguration()
        {
            RouteSettingsData routeSettingsData = GetRouteSettingData();

            if (routeSettingsData.RouteBuildConfiguration == null)
                throw new NullReferenceException("routeSettingsData.RouteBuildConfiguration");

            return routeSettingsData.RouteBuildConfiguration;
        }

        private RouteDatabasesToKeep GetRouteDatabasesToKeep()
        {
            RouteSettingsData routeSettingsData = GetRouteSettingData();

            if (routeSettingsData.RouteDatabasesToKeep == null)
                throw new NullReferenceException("routeSettingsData.RouteDatabasesToKeep");

            return routeSettingsData.RouteDatabasesToKeep;
        }

        private RouteSettingsData GetRouteSettingData()
        {
            SettingManager settingManager = new SettingManager();
            RouteSettingsData routeSettingsData = settingManager.GetSetting<RouteSettingsData>(Constants.RouteSettings);

            if (routeSettingsData == null)
                throw new NullReferenceException("routeSettingsData");

            return routeSettingsData;
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


        #endregion
    }
}