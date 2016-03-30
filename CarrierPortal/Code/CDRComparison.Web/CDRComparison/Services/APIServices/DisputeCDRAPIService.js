(function (appControllers) {

    'use strict';

    DisputeCDRAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRComparison_ModuleConfig'];

    function DisputeCDRAPIService(BaseAPIService, UtilsService, CDRComparison_ModuleConfig) {
        var controllerName = 'DisputeCDR';

        return {
            GetFilteredDisputeCDRs: GetFilteredDisputeCDRs
        };

        function GetFilteredDisputeCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRComparison_ModuleConfig.moduleName, controllerName, 'GetFilteredDisputeCDRs'), input);
        }
    }

    appControllers.service('CDRComparison_DisputeCDRAPIService', DisputeCDRAPIService);

})(appControllers);