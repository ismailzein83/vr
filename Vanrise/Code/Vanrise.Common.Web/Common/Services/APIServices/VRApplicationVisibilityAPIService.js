(function (appControllers) {

    "use strict";

    VRApplicationVisibilityAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRApplicationVisibilityAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRApplicationVisibility";


        function GetFilteredVRApplicationVisibilities(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRApplicationVisibilities'), input);
        }

        function GetVRApplicationVisibility(vrApplicationVisibilityId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRApplicationVisibility'), {
                VRApplicationVisibilityId: vrApplicationVisibilityId
            });
        }

        function AddVRApplicationVisibility(vrApplicationVisibilityItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRApplicationVisibility'), vrApplicationVisibilityItem);
        }

        function UpdateVRApplicationVisibility(vrApplicationVisibilityItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRApplicationVisibility'), vrApplicationVisibilityItem);
        }

        function GetVRModuleVisibilityExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRModuleVisibilityExtensionConfigs"), {}, { useCache: true });
        }

        function GetVRApplicationVisibilitiesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRApplicationVisibilitiesInfo"), {
                filter: filter
            });
        }

        function HasAddVRApplicationVisibilityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRApplicationVisibility']));
        }

        function HasEditVRApplicationVisibilityPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRApplicationVisibility']));
        }


        return ({
            GetFilteredVRApplicationVisibilities: GetFilteredVRApplicationVisibilities,
            GetVRApplicationVisibility: GetVRApplicationVisibility,
            AddVRApplicationVisibility: AddVRApplicationVisibility,
            UpdateVRApplicationVisibility: UpdateVRApplicationVisibility,
            GetVRModuleVisibilityExtensionConfigs: GetVRModuleVisibilityExtensionConfigs,
            GetVRApplicationVisibilitiesInfo: GetVRApplicationVisibilitiesInfo,
            HasAddVRApplicationVisibilityPermission: HasAddVRApplicationVisibilityPermission,
            HasEditVRApplicationVisibilityPermission: HasEditVRApplicationVisibilityPermission,
        });
    }

    appControllers.service('VRCommon_VRApplicationVisibilityAPIService', VRApplicationVisibilityAPIService);

})(appControllers);