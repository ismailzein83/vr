(function (appControllers) {

    "use strict";
    zoneServiceConfigAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function zoneServiceConfigAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        function GetFilteredZoneServiceConfigs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "ZoneServiceConfig", "GetFilteredZoneServiceConfigs"), input);
        }
        function GetAllZoneServiceConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "ZoneServiceConfig", "GetAllZoneServiceConfigs"));
        }
        function GetZoneServiceConfig(zoneServiceConfigId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "ZoneServiceConfig", "GetZoneServiceConfig"), {
                zoneServiceConfigId: zoneServiceConfigId
            });

        }
        function UpdateZoneServiceConfig(zoneServiceConfigObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "ZoneServiceConfig", "UpdateZoneServiceConfig"), zoneServiceConfigObject);
        }
        function AddZoneServiceConfig(zoneServiceConfigObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "ZoneServiceConfig", "AddZoneServiceConfig"), zoneServiceConfigObject);
        }
        return ({
            GetFilteredZoneServiceConfigs: GetFilteredZoneServiceConfigs,
            GetAllZoneServiceConfigs: GetAllZoneServiceConfigs,
            GetZoneServiceConfig: GetZoneServiceConfig,
            UpdateZoneServiceConfig: UpdateZoneServiceConfig,
            AddZoneServiceConfig: AddZoneServiceConfig
        });
    }

    appControllers.service('WhS_BE_ZoneServiceConfigAPIService', zoneServiceConfigAPIService);

})(appControllers);