(function (appControllers) {

    "use strict";
    cdrLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function cdrLogAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        function GetCDRLogData(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "CDRLog", "GetCDRLogData"), input);
        }

        return ({
            GetCDRLogData: GetCDRLogData
        });
    }

    appControllers.service('WhS_Analytics_CDRLogAPIService', cdrLogAPIService);
})(appControllers);