
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
        function GetStatusChargeInfos(entityTypeId) {
            return baseApiService.get(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'GetStatusChargeInfos'), {
                entityTypeId: entityTypeId
            });
        }
        function GetStatusChargingSet(statusChargingSetId) {
            return baseApiService.get(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'GetStatusChargingSet'), {
                statusChargingSetId: statusChargingSetId
            });
        }
        function UpdateStatusChargingSet(statusChargingSetItem) {
            return baseApiService.post(utilsService.getServiceURL(retailBeModuleConfig.moduleName, controllerName, 'UpdateStatusChargingSet'), statusChargingSetItem);
        }

        return ({
            AddStatusChargingSet: AddStatusChargingSet,
            GetFilteredStatusChargingSet: GetFilteredStatusChargingSet,
            GetStatusChargingSet: GetStatusChargingSet,
            UpdateStatusChargingSet: UpdateStatusChargingSet,
            GetStatusChargeInfos: GetStatusChargeInfos
        });
    }

    appControllers.service('Retail_BE_StatusChargingSetAPIService', StatusChargingSetAPIService);

})(appControllers);