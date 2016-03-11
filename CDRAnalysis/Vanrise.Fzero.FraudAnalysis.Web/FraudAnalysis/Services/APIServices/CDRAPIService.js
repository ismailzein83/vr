(function (appControllers) {

    "use strict";
    cdrAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CDRAnalysis_FA_ModuleConfig'];

    function cdrAPIService(BaseAPIService, UtilsService, CDRAnalysis_FA_ModuleConfig) {

        var controllerName = 'CDR';
        function GetCDRs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CDRAnalysis_FA_ModuleConfig.moduleName, controllerName, "GetCDRs"), input);
        }
        return ({
            GetCDRs: GetCDRs
        });
    }

    appControllers.service('CDRAPIService', cdrAPIService);
})(appControllers);