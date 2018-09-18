(function (appControllers) {

    "use strict";
    zoneRoutingProductApiService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];
    function zoneRoutingProductApiService(baseApiService, utilsService, whSBeModuleConfig, SecurityService) {

        var controllerName = "ZoneRoutingProduct";

        function GetFilteredZoneRoutingProducts(input) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetFilteredZoneRoutingProducts"), input);
        }
        function GetPrimarySaleEntity() {
            return baseApiService.get(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "GetPrimarySaleEntity"));
        }
        function UpdateZoneRoutingProduct(routingProductObject) {
            return baseApiService.post(utilsService.getServiceURL(whSBeModuleConfig.moduleName, controllerName, "UpdateZoneRoutingProduct"), routingProductObject);
        }
        function hasUpdateZoneRoutingProductPermission() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(whSBeModuleConfig.moduleName, controllerName, ['UpdateZoneRoutingProduct']));
        }
        return ({
            GetFilteredZoneRoutingProducts: GetFilteredZoneRoutingProducts,
            GetPrimarySaleEntity: GetPrimarySaleEntity,
            UpdateZoneRoutingProduct: UpdateZoneRoutingProduct,
            hasUpdateZoneRoutingProductPermission: hasUpdateZoneRoutingProductPermission
        });
    }
    appControllers.service("WhS_BE_ZoneRoutingProductAPIService", zoneRoutingProductApiService);

})(appControllers);