(function (appControllers) {

    'use strict';

    CDRAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function CDRAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'CDR';

        return {
            GetFilteredCDRs: GetFilteredCDRs
        };

        function GetFilteredCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFilteredCDRs'), input);
        }
    }

    appControllers.service('CDRComparison_CDRAPIService', CDRAPIService);

})(appControllers);