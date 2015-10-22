(function (appControllers) {

    "use strict";
    supplierZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function supplierZoneAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetSupplierZonesInfo(supplierId, filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierZone", "GetSupplierZonesInfo"), {
                supplierId: supplierId,
                filter: filter
            });

        }
        function GetSupplierZonesInfoByIds(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SupplierZone", "GetSupplierZonesInfoByIds"), input);
        
        }


        return ({
            GetSupplierZonesInfo: GetSupplierZonesInfo,
            GetSupplierZonesInfoByIds: GetSupplierZonesInfoByIds
        });
    }

    appControllers.service('WhS_BE_SupplierZoneAPIService', supplierZoneAPIService);
})(appControllers);