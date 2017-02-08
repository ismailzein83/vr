(function (appControllers) {

    "use strict";
    zoneRoutingProductApiService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig"];
    function zoneRoutingProductApiService(baseApiService, utilsService, whSBeModuleConfig) {

        var controllerName = "ZoneRoutingProduct";

        function GetFilteredZoneRoutingProducts(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredZoneRoutingProducts"), input);
        }
        function GetSaleAreaSettingsData() {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetSaleAreaSettingsData"));
        }
        return ({
            GetFilteredZoneRoutingProducts: GetFilteredZoneRoutingProducts,
            GetSaleAreaSettingsData: GetSaleAreaSettingsData
        });
    }
    appControllers.service("WhS_BE_ZoneRoutingProductAPIService", zoneRoutingProductApiService);

})(appControllers);