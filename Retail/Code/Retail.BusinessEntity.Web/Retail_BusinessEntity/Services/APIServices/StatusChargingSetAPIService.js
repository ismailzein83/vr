
(function (appControllers) {

    "use strict";
    StatusChargingSetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function StatusChargingSetAPIService(baseApiService, utilsService, retailBeModuleConfig) {

        var controllerName = "StatusChargingSet";

        function GetFilteredStatusChargingSet(input) {
            return baseApiService.post(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'GetFilteredStatusChargingSet'), input);
        }

        function AddStatusChargingSet(statusChargingSetItem) {
            return baseApiService.post(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'AddStatusChargingSet'), statusChargingSetItem);
        }

        return ({
            AddStatusChargingSet: AddStatusChargingSet,
            GetFilteredStatusChargingSet: GetFilteredStatusChargingSet
        });
    }

    appControllers.service('Retail_BE_StatusChargingSetAPIService', StatusChargingSetAPIService);

})(appControllers);