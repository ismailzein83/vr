(function (appControllers) {

    "use strict";
    repeatedNumberAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Analytics_ModuleConfig'];

    function repeatedNumberAPIService(BaseAPIService, UtilsService, WhS_Analytics_ModuleConfig) {

        function GetAllFilteredRepeatedNumbers(input) {

            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Analytics_ModuleConfig.moduleName, "RepeatedNumber", "GetAllFilteredRepeatedNumbers"), input);
        }

        return ({
            GetAllFilteredRepeatedNumbers: GetAllFilteredRepeatedNumbers
        });
    }

    appControllers.service('WhS_Analytics_RepeatedNumberAPIService', repeatedNumberAPIService);
})(appControllers);