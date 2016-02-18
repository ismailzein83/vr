(function (appControllers) {

    "use strict";
    cdrLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function cdrLogAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetCDRLogData(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "CDRLog", "GetCDRLogData"), input);
        }

        return ({
            GetCDRLogData: GetCDRLogData
        });
    }

    appControllers.service('WhS_Analytics_CDRLogAPIService', cdrLogAPIService);
})(appControllers);