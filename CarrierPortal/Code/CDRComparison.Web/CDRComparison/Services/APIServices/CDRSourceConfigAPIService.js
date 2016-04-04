(function (appControllers) {

    'use strict';

    CDRSourceConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function CDRSourceConfigAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'CDRSourceConfig';

        return {
            GetCDRSourceConfigs: GetCDRSourceConfigs
        };

        function GetCDRSourceConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetCDRSourceConfigs'));
        }
    }

    appControllers.service('CDRComparison_CDRSourceConfigAPIService', CDRSourceConfigAPIService);

})(appControllers);