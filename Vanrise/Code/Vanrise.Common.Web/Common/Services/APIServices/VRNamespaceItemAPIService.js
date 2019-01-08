(function (appControllers) {

    "use strict";

    VRNamespaceItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRNamespaceItemAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRNamespaceItem";


        function GetFilteredVRNamespaceItems(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRNamespaceItems'), input);
        }

        function GetVRNamespaceItem(vrNamespaceItemId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRNamespaceItem'), {
                VRNamespaceItemId: vrNamespaceItemId
            });
        }

        function AddVRNamespaceItem(vrNamespaceItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRNamespaceItem'), vrNamespaceItem);
        }

        function UpdateVRNamespaceItem(vrNamespaceItem) {
           return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRNamespaceItem'), vrNamespaceItem);
       }

        function GetVRDynamicCodeSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRDynamicCodeSettingsConfigs"));
        }

        function TryCompileNamespaceItem(vrNamespaceItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "TryCompileNamespaceItem"), vrNamespaceItem);
        }

        function ExportCompilationResult(vrNamespaceItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'ExportCompilationResult'), vrNamespaceItem, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }

        function HasGetFilteredVRNamespaceItems() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['GetFilteredVRNamespaceItems']));
        }

        function HasAddVRNamespaceItemPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRNamespaceItem']));
        }

        function HasEditVRNamespaceItemPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRNamespaceItem']));
        }

        return ({
            GetFilteredVRNamespaceItems: GetFilteredVRNamespaceItems,
            GetVRNamespaceItem: GetVRNamespaceItem,
            AddVRNamespaceItem: AddVRNamespaceItem,
            UpdateVRNamespaceItem: UpdateVRNamespaceItem,
            GetVRDynamicCodeSettingsConfigs: GetVRDynamicCodeSettingsConfigs,
            TryCompileNamespaceItem: TryCompileNamespaceItem,
            ExportCompilationResult: ExportCompilationResult,
            HasGetFilteredVRNamespaceItems: HasGetFilteredVRNamespaceItems,
            HasAddVRNamespaceItemPermission: HasAddVRNamespaceItemPermission,
            HasEditVRNamespaceItemPermission: HasEditVRNamespaceItemPermission
        });
    }

    appControllers.service('VRCommon_VRNamespaceItemAPIService', VRNamespaceItemAPIService);

})(appControllers);