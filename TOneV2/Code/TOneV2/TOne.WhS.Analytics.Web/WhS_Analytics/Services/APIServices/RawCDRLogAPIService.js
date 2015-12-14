(function (appControllers) {

    "use strict";
    rawCDRLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function rawCDRLogAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        function GetRawCDRData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "RawCDR", "GetRawCDRData"), input);
        }

        return ({
            GetRawCDRData: GetRawCDRData
        });
    }

    appControllers.service('WhS_Analytics_RawCDRLogAPIService', rawCDRLogAPIService);
})(appControllers);
