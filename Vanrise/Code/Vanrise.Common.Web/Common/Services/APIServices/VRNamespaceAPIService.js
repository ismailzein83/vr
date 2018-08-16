(function (appControllers) {

    "use strict";

    VRNamespaceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function VRNamespaceAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "VRNamespace";


        function GetFilteredVRNamespaces(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRNamespaces'), input);
        }

        function GetVRNamespace(vrNamespaceId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRNamespace'), {
                VRNamespaceId: vrNamespaceId
            });
        }

        function AddVRNamespace(vrNamespace) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRNamespace'), vrNamespace);
        }

        function UpdateVRNamespace(vrNamespace) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRNamespace'), vrNamespace);
        }

        function HasAddVRNamespacePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRNamespace']));
        }

        function HasEditVRNamespacePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRNamespace']));
        }

        function TryCompileNamespace(vrNamespace) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "TryCompileNamespace"), vrNamespace);
        }

        function ExportCompilationResult(vrNamespace) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'ExportCompilationResult'), vrNamespace, {
                returnAllResponseParameters: true,
                responseTypeAsBufferArray: true
            });
        }


        return ({
            GetFilteredVRNamespaces: GetFilteredVRNamespaces,
            GetVRNamespace: GetVRNamespace,
            AddVRNamespace: AddVRNamespace,
            UpdateVRNamespace: UpdateVRNamespace,
            HasAddVRNamespacePermission: HasAddVRNamespacePermission,
            HasEditVRNamespacePermission: HasEditVRNamespacePermission,
            TryCompileNamespace: TryCompileNamespace,
            ExportCompilationResult: ExportCompilationResult
        });
    }

    appControllers.service('VRCommon_VRNamespaceAPIService', VRNamespaceAPIService);

})(appControllers);