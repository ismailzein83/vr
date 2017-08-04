(function (appControllers) {

    'use strict';

    InvalidCDRAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function InvalidCDRAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'InvalidCDR';

        return {
            GetFilteredInvalidCDRs: GetFilteredInvalidCDRs
        };

        function GetFilteredInvalidCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFilteredInvalidCDRs'), input);
        }
    }

    appControllers.service('CDRComparison_InvalidCDRAPIService', InvalidCDRAPIService);

})(appControllers);