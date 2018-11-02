(function (appControllers) {
    "use strict";

    RepeatedNumbersAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'CP_WhS_ModuleConfig'];

    function RepeatedNumbersAPIService(BaseAPIService, UtilsService, SecurityService, CP_WhS_ModuleConfig) {
        var controllerName = "RepeatedNumbers";

        function GetFilteredRepeatedNumbers(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetFilteredRepeatedNumbers'), input);
        }
        return ({
            GetFilteredRepeatedNumbers: GetFilteredRepeatedNumbers
        });
    }
    appControllers.service('CP_WhS_RepeatedNumbersAPIService', RepeatedNumbersAPIService);

})(appControllers);