(function (appControllers) {

    "use strict";

    customerZoneAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function customerZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        return ({
            GetCustomerZones: GetCustomerZones,
            GetFilteredSaleZonesToSell: GetFilteredSaleZonesToSell,
            AddCustomerZones: AddCustomerZones
        });

        function GetCustomerZones(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "GetCustomerZones"), {
                customerId: customerId
            });
        }

        function GetFilteredSaleZonesToSell(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "GetFilteredSaleZonesToSell"), input);
        }

        function AddCustomerZones(customerZones) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "AddCustomerZones"), customerZones);
        }
    }

    appControllers.service("WhS_BE_CustomerZoneAPIService", customerZoneAPIService);

})(appControllers);