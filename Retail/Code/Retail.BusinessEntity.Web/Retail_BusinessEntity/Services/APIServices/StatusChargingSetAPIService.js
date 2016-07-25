
(function (appControllers) {

    "use strict";
    StatusChargingSetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function StatusChargingSetAPIService(baseApiService, utilsService, retailBeModuleConfig) {

        var controllerName = "StatusChargingSet";

        function AddStatusChargingSet(statusChargingSetItem) {
            return baseApiService.post(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'AddStatusChargingSet'), statusChargingSetItem);
        }

        return ({
            AddStatusChargingSet: AddStatusChargingSet
        });
    }

    appControllers.service('Retail_BE_StatusChargingSetAPIService ', StatusChargingSetAPIService);

})(appControllers);