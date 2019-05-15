(function (appControllers) {

    "use strict";

    codeZoneMatchAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function codeZoneMatchAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "CodeZoneMatch";

        function GetSaleZonesMatchingSpecificDeals(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetSaleZonesMatchingSpecificDeals"), input);
        }

        return ({
            GetSaleZonesMatchingSpecificDeals: GetSaleZonesMatchingSpecificDeals
        });
    }

    appControllers.service('WhS_Routing_CodeZoneMatchAPIService', codeZoneMatchAPIService);
})(appControllers);