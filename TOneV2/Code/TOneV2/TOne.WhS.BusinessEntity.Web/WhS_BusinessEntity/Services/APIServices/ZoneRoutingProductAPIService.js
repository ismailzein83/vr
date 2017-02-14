﻿(function (appControllers) {

    "use strict";
    zoneRoutingProductApiService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function zoneRoutingProductApiService(baseApiService, utilsService, whSBeModuleConfig) {

        var controllerName = "ZoneRoutingProduct";

        function GetFilteredZoneRoutingProducts(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredZoneRoutingProducts"), input);
        }
        function GetPrimarySaleEntity() {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetPrimarySaleEntity"));
        }
        return ({
            GetFilteredZoneRoutingProducts: GetFilteredZoneRoutingProducts,
            GetPrimarySaleEntity: GetPrimarySaleEntity
        });
    }
    appControllers.service("WhS_BE_ZoneRoutingProductAPIService", zoneRoutingProductApiService);

})(appControllers);