(function (appControllers) {

    'use strict';

    CDRSourceConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function CDRSourceConfigAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'CDRSourceConfig';

        return {
            GetCDRSourceConfigs: GetCDRSourceConfigs,
            GetCDRSourceConfig: GetCDRSourceConfig
        };

        function GetCDRSourceConfigs(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetCDRSourceConfigs'), {
                filter: filter
            });
        }

        function GetCDRSourceConfig(cdrSourceConfigId) {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetCDRSourceConfig'), {
                cdrSourceConfigId: cdrSourceConfigId
            });
        }
    }

    appControllers.service('CDRComparison_CDRSourceConfigAPIService', CDRSourceConfigAPIService);

})(appControllers);