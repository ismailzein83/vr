
(function (appControllers) {

    "use strict";
    VRComponentTypeAPIService.$inject = ['BaseAPIService', 'SecurityService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRComponentTypeAPIService(BaseAPIService, SecurityService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRComponentType";


        function GetFilteredVRComponentTypes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRComponentTypes'), input);
        }

        function GetVRComponentType(vrComponentTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRComponentType'), {
                VRComponentTypeId: vrComponentTypeId
            });
        }

        function AddVRComponentType(vrComponentTypeItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRComponentType'), vrComponentTypeItem);
        }

        function UpdateVRComponentType(vrComponentTypeItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRComponentType'), vrComponentTypeItem);
        }


        function HasAddVRComponentTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRComponentType']));
        }

        function HasEditVRComponentTypePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRComponentType']));
        }

        function GetVRComponentTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRComponentTypeExtensionConfigs"));
        }

        function GetVRComponentTypeExtensionConfigById(extensionConfigId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRComponentTypeExtensionConfigById"), {
                extensionConfigId: extensionConfigId
            });
        }

        return ({
            GetFilteredVRComponentTypes: GetFilteredVRComponentTypes,
            GetVRComponentType: GetVRComponentType,
            AddVRComponentType: AddVRComponentType,
            UpdateVRComponentType: UpdateVRComponentType,
            HasAddVRComponentTypePermission: HasAddVRComponentTypePermission,
            HasEditVRComponentTypePermission: HasEditVRComponentTypePermission,
            GetVRComponentTypeExtensionConfigs: GetVRComponentTypeExtensionConfigs,
            GetVRComponentTypeExtensionConfigById: GetVRComponentTypeExtensionConfigById
        });
    }

    appControllers.service('VRCommon_VRComponentTypeAPIService', VRComponentTypeAPIService);

})(appControllers);