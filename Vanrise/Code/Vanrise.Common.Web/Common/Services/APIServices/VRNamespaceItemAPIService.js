(function (appControllers) {

    "use strict";

    VRNamespaceItemAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRNamespaceItemAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

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

        return ({
            GetFilteredVRNamespaceItems: GetFilteredVRNamespaceItems,
            GetVRNamespaceItem: GetVRNamespaceItem,
            AddVRNamespaceItem: AddVRNamespaceItem,
            UpdateVRNamespaceItem: UpdateVRNamespaceItem,
            GetVRDynamicCodeSettingsConfigs: GetVRDynamicCodeSettingsConfigs,
            TryCompileNamespaceItem: TryCompileNamespaceItem,
            ExportCompilationResult: ExportCompilationResult
        });
    }

    appControllers.service('VRCommon_VRNamespaceItemAPIService', VRNamespaceItemAPIService);

})(appControllers);