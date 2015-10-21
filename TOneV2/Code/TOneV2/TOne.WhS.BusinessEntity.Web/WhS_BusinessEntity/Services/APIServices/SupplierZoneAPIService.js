(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetSupplierZones(supplierId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierZone", "GetSupplierZones"), {
                supplierId: supplierId
            });
        }

        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
        });
    }

    appControllers.service('WhS_BE_SupplierZoneAPIService', supplierZoneAPIService);
})(appControllers);