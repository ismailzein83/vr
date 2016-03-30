(function (appControllers) {

    'use strict';

    PartialMatchCDRAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function PartialMatchCDRAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'PartialMatchCDR';

        return {
            GetFilteredPartialMatchCDRs: GetFilteredPartialMatchCDRs
        };

        function GetFilteredPartialMatchCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFilteredPartialMatchCDRs'), input);
        }
    }

    appControllers.service('CDRComparison_PartialMatchCDRAPIService', PartialMatchCDRAPIService);

})(appControllers);