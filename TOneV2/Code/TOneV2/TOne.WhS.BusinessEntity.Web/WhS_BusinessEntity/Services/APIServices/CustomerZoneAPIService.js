(function (appControllers) {

    "use strict";

    customerZoneAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function customerZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        return ({
            GetCustomerZones: GetCustomerZones,
            GetCountriesToSell: GetCountriesToSell,
            AddCustomerZones: AddCustomerZones
        });

        function GetCustomerZones(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "GetCustomerZones"), {
                customerId: customerId
            });
        }

        function GetCountriesToSell(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "GetCountriesToSell"), {
                customerId: customerId
            });
        }

        function AddCustomerZones(customerZones) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "AddCustomerZones"), customerZones);
        }
    }

    appControllers.service("WhS_BE_CustomerZoneAPIService", customerZoneAPIService);

})(appControllers);