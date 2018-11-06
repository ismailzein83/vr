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
        return ({
            GetRemoteSupplierZonesInfo: GetRemoteSupplierZonesInfo
        });
    }
    appControllers.service('CP_WhS_SupplierZonesAPIService', SupplierZonesAPIService);

})(appControllers);