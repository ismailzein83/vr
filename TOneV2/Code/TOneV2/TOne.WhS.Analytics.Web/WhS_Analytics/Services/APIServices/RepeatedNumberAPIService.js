(function (appControllers) {

    "use strict";
    repeatedNumberAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function repeatedNumberAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        function GetRepeatedNumberData(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "RepeatedNumber", "GetRepeatedNumberData"), input);
        }

        return ({
            GetRepeatedNumberData: GetRepeatedNumberData
        });
    }

    appControllers.service('WhS_Analytics_RepeatedNumberAPIService', repeatedNumberAPIService);
})(appControllers);