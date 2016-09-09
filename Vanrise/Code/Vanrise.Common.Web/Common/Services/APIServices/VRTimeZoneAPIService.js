(function (appControllers) {

    "use strict";
    vrTimeZoneAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function vrTimeZoneAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'TimeZone';

        function GetFilteredVRTimeZones(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredVRTimeZones"), input);
        }
        function GetVRTimeZonesInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRTimeZonesInfo"));
        }
        function GetVRTimeZone(timeZoneId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRTimeZone"), {
                timeZoneId: timeZoneId
            });

        }
        function UpdateVRTimeZone(vrTimeZone) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "UpdateVRTimeZone"), vrTimeZone);
        }
        function AddVRTimeZone(vrTimeZone) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddVRTimeZone"), vrTimeZone);
        }
    
        function HasAddVRTimeZonePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRTimeZone']));
        }

        function HasEditVRTimeZonePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRTimeZone']));
        }

        return ({
            GetFilteredVRTimeZones: GetFilteredVRTimeZones,
            GetVRTimeZonesInfo: GetVRTimeZonesInfo,
            GetVRTimeZone: GetVRTimeZone,
            UpdateVRTimeZone: UpdateVRTimeZone,
            AddVRTimeZone: AddVRTimeZone,
            HasAddVRTimeZonePermission: HasAddVRTimeZonePermission,
            HasEditVRTimeZonePermission: HasEditVRTimeZonePermission
        });
    }

    appControllers.service('VRCommon_VRTimeZoneAPIService', vrTimeZoneAPIService);

})(appControllers);