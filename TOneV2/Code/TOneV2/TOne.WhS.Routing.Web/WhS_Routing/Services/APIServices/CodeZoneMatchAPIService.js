(function (appControllers) {

    "use strict";

    codeZoneMatchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function codeZoneMatchAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "CodeZoneMatch";

        function GetSaleZonesMatchingSupplierDeals(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetSaleZonesMatchingSupplierDeals"), input);
        }

        return ({
            GetSaleZonesMatchingSupplierDeals: GetSaleZonesMatchingSupplierDeals
        });
    }

    appControllers.service('WhS_Routing_CodeZoneMatchAPIService', codeZoneMatchAPIService);
})(appControllers);