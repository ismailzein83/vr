(function (appControllers) {

    "use strict";

    customerZoneAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];

    function customerZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetCustomerZone(customerId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "CustomerZone", "GetCustomerZone"), {
                customerId: customerId
            });
        }

        return ({
            GetCustomerZone: GetCustomerZone
        });
    }

    appControllers.service("WhS_BE_CustomerZoneAPIService", customerZoneAPIService);

})(appControllers);