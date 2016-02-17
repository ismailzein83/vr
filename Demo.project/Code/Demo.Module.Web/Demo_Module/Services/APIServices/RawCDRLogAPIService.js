(function (appControllers) {

    "use strict";
    rawCDRLogAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_ModuleConfig'];

    function rawCDRLogAPIService(BaseAPIService, UtilsService, Demo_ModuleConfig) {

        function GetRawCDRData(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_ModuleConfig.moduleName, "RawCDR", "GetRawCDRData"), input);
        }

        return ({
            GetRawCDRData: GetRawCDRData
        });
    }

    appControllers.service('WhS_Analytics_RawCDRLogAPIService', rawCDRLogAPIService);
})(appControllers);
