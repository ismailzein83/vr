(function (appControllers) {

    "use strict";

    customerZoneAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function customerZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        var controllerName = "CustomerZone";

        function GetCustomerZones(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCustomerZones"), {
                customerId: customerId
            });
        }

        function GetCountriesToSell(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetCountriesToSell"), {
                customerId: customerId
            });
        }

        function AddCustomerZones(customerZones) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddCustomerZones"), customerZones);
        }

        return ({
            GetCustomerZones: GetCustomerZones,
            GetCountriesToSell: GetCountriesToSell,
            AddCustomerZones: AddCustomerZones
        });

    }

    appControllers.service("WhS_BE_CustomerZoneAPIService", customerZoneAPIService);

})(appControllers);