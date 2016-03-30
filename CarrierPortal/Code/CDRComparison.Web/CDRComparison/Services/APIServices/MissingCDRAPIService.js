(function (appControllers) {

    'use strict';

    MissingCDRAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function MissingCDRAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'MissingCDR';

        return {
            GetFilteredMissingCDRs: GetFilteredMissingCDRs
        };

        function GetFilteredMissingCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFilteredMissingCDRs'), input);
        }
    }

    appControllers.service('CDRComparison_MissingCDRAPIService', MissingCDRAPIService);

})(appControllers);