(function (appControllers) {

    "use strict";

    beConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function beConfigurationAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "BEConfiguration";

        function GetPackageUsageVolumeRecurringPeriodConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetPackageUsageVolumeRecurringPeriodConfigs"));
        }

        return {
            GetPackageUsageVolumeRecurringPeriodConfigs: GetPackageUsageVolumeRecurringPeriodConfigs
        };
    }

    appControllers.service('Retail_BE_BEConfigurationAPIService', beConfigurationAPIService);
})(appControllers);