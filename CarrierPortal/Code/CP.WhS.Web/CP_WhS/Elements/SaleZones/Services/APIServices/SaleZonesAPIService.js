(function (appControllers) {
    "use strict";
    SupplierZonesAPIService.$inject = ['BaseAPIService', 'UtilsService', 'CP_WhS_ModuleConfig'];

    function SupplierZonesAPIService(BaseAPIService, UtilsService, CP_WhS_ModuleConfig) {
        var controllerName = "WhSSaleZoneBE";

        function GetRemoteSaleZonesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(CP_WhS_ModuleConfig.moduleName, controllerName, 'GetRemoteSaleZonesInfo'), {
                serializedFilter: serializedFilter
            });
        }
        return ({
            GetRemoteSaleZonesInfo: GetRemoteSaleZonesInfo
        });
    }
    appControllers.service('CP_WhS_SaleZonesAPIService', SupplierZonesAPIService);

})(appControllers);