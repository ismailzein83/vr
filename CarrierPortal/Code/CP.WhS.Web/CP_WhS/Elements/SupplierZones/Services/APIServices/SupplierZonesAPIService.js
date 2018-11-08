(function (appControllers) {
    "use strict";
    SupplierZonesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

    function SupplierZonesAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {
        var controllerName = "WhSSupplierZoneBE";

        function GetRemoteSupplierZonesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetRemoteSupplierZonesInfo'), {
                serializedFilter: serializedFilter
            });
        }
        function GetSupplierIdBySupplierZoneIds(supplierZoneIds) {
            return BaseAPIService.post(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetSupplierIdBySupplierZoneIds"), supplierZoneIds);
        }
        function GetSupplierZoneInfoByIds(serializedObj) {
            return BaseAPIService.get(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, "GetSupplierZoneInfoByIds"),
                {
                    serializedObj: serializedObj
                });
        }
        return ({
            GetRemoteSupplierZonesInfo: GetRemoteSupplierZonesInfo, 
            GetSupplierIdBySupplierZoneIds: GetSupplierIdBySupplierZoneIds,
            GetSupplierZoneInfoByIds: GetSupplierZoneInfoByIds,
        });
    }
    appControllers.service('CP_WhS_SupplierZonesAPIService', SupplierZonesAPIService);

})(appControllers);