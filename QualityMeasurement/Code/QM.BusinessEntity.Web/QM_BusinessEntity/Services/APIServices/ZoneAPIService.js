(function (appControllers) {

    "use strict";
    zoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_BE_ModuleConfig'];

    function zoneAPIService(BaseAPIService, UtilsService, QM_BE_ModuleConfig) {

        var controllerName = 'Zone';

        function GetFilteredZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetFilteredZones"), input);
        }

        function GetZoneSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetZoneSourceTemplates"));
        }

        function GetZonesInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, controllerName, "GetZonesInfo"),
                {
                    serializedFilter: serializedFilter
                });
        }


        return ({
            GetZoneSourceTemplates: GetZoneSourceTemplates,
            GetZonesInfo: GetZonesInfo,
            GetFilteredZones: GetFilteredZones
        });
    }

    appControllers.service('QM_BE_ZoneAPIService', zoneAPIService);
})(appControllers);