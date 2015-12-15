(function (appControllers) {

    "use strict";
    zoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'QM_BE_ModuleConfig'];

    function zoneAPIService(BaseAPIService, UtilsService, QM_BE_ModuleConfig) {

    
        function GetZoneSourceTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(QM_BE_ModuleConfig.moduleName, "Zone", "GetZoneSourceTemplates"));
        }

        
        return ({
            GetZoneSourceTemplates: GetZoneSourceTemplates
        });
    }

    appControllers.service('QM_BE_ZoneAPIService', zoneAPIService);
})(appControllers);