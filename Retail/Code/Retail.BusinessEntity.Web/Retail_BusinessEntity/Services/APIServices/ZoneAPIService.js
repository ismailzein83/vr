(function (appControllers) {

    "use strict";
    zoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function zoneAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "Zone";

        function GetZonesByCountryId(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetZonesByCountryId"),
                { serializedFilter: serializedFilter });
        }

        return ({
            GetZonesByCountryId: GetZonesByCountryId
        });
    }

    appControllers.service('Retail_BE_ZoneAPIService', zoneAPIService);

})(appControllers);