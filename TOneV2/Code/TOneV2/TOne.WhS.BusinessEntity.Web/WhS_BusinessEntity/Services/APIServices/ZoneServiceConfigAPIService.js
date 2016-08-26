(function (appControllers) {

    "use strict";
    zoneServiceConfigAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function zoneServiceConfigAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {

        var controllerName = "ZoneServiceConfig";

        function GetFilteredZoneServiceConfigs(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredZoneServiceConfigs"), input);
        }

        function GetAllZoneServiceConfigs(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetAllZoneServiceConfigs"),
                {
                    serializedFilter: serializedFilter
                }
            );
        }

        function GetZoneServiceConfig(zoneServiceConfigId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetZoneServiceConfig"), {
                zoneServiceConfigId: zoneServiceConfigId
            });
        }

        function UpdateZoneServiceConfig(zoneServiceConfigObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateZoneServiceConfig"), zoneServiceConfigObject);
        }

        function AddZoneServiceConfig(zoneServiceConfigObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddZoneServiceConfig"), zoneServiceConfigObject);
        }

        function HasUpdateZoneServiceConfigPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateZoneServiceConfig']));
        }

        function HasAddZoneServiceConfigPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddZoneServiceConfig']));
        }


        return ({
            GetFilteredZoneServiceConfigs: GetFilteredZoneServiceConfigs,
            GetAllZoneServiceConfigs: GetAllZoneServiceConfigs,
            GetZoneServiceConfig: GetZoneServiceConfig,
            UpdateZoneServiceConfig: UpdateZoneServiceConfig,
            AddZoneServiceConfig: AddZoneServiceConfig,
            HasUpdateZoneServiceConfigPermission: HasUpdateZoneServiceConfigPermission,
            HasAddZoneServiceConfigPermission: HasAddZoneServiceConfigPermission
        });

    }

    appControllers.service("WhS_BE_ZoneServiceConfigAPIService", zoneServiceConfigAPIService);

})(appControllers);