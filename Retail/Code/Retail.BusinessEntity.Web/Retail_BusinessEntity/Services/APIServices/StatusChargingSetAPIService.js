
(function (appControllers) {

    "use strict";
    StatusDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function StatusDefinitionAPIService(baseApiService, utilsService, retailBeModuleConfig) {

        var controllerName = "StatusChargingSet";

        function GetFilteredStatusChargingSet(input) {
            return baseApiService.post(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'GetFilteredStatusChargingSet'), input);
        }

        return ({
            GetFilteredStatusChargingSet: GetFilteredStatusChargingSet
        });
    }

    appControllers.service('Retail_BE_StatusChargingSetAPIService ', StatusDefinitionAPIService);

})(appControllers);