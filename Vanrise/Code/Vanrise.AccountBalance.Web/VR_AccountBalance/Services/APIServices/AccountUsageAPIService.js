(function (appControllers) {

    'use strict';

    AccountUsageAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_AccountBalance_ModuleConfig', 'SecurityService'];

    function AccountUsageAPIService(BaseAPIService, UtilsService, VR_AccountBalance_ModuleConfig, SecurityService) {
        var controllerName = 'AccountUsage';
        function GetAccountUsagePeriodSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_AccountBalance_ModuleConfig.moduleName, controllerName, "GetAccountUsagePeriodSettingsConfigs"));
        }
        
        return {
            GetAccountUsagePeriodSettingsConfigs: GetAccountUsagePeriodSettingsConfigs,
        };
    }

    appControllers.service('VR_AccountBalance_AccountUsageAPIService', AccountUsageAPIService);

})(appControllers);