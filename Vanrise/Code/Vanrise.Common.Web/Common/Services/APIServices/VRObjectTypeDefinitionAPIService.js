
(function (appControllers) {

    "use strict";
    VRObjectTypeDefinitionAPIService.$inject = ['BaseAPIService', 'SecurityService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRObjectTypeDefinitionAPIService(BaseAPIService, SecurityService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRObjectTypeDefinition";


        function GetFilteredVRObjectTypeDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRObjectTypeDefinitions'), input);
        }

        function GetVRObjectTypeDefinition(vrObjectTypeDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRObjectTypeDefinition'), {
                VRObjectTypeDefinitionId: vrObjectTypeDefinitionId
            });
        }

        function AddVRObjectTypeDefinition(vrObjectTypeDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRObjectTypeDefinition'), vrObjectTypeDefinitionItem);
        }

        function UpdateVRObjectTypeDefinition(vrObjectTypeDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRObjectTypeDefinition'), vrObjectTypeDefinitionItem);
        }

        function GetVRObjectTypeDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRObjectTypeDefinitionsInfo"), {filter:filter}, {useCache:true});
        }

        function HasAddVRObjectTypeDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['AddVRObjectTypeDefinition']));
        }

        function HasEditVRObjectTypeDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VRCommon_ModuleConfig.moduleName, controllerName, ['UpdateVRObjectTypeDefinition']));
        }


        return ({
            GetFilteredVRObjectTypeDefinitions: GetFilteredVRObjectTypeDefinitions,
            GetVRObjectTypeDefinition: GetVRObjectTypeDefinition,
            AddVRObjectTypeDefinition: AddVRObjectTypeDefinition,
            UpdateVRObjectTypeDefinition: UpdateVRObjectTypeDefinition,
            GetVRObjectTypeDefinitionsInfo: GetVRObjectTypeDefinitionsInfo,
            HasAddVRObjectTypeDefinitionPermission: HasAddVRObjectTypeDefinitionPermission,
            HasEditVRObjectTypeDefinitionPermission: HasEditVRObjectTypeDefinitionPermission
        });
    }

    appControllers.service('VRCommon_VRObjectTypeDefinitionAPIService', VRObjectTypeDefinitionAPIService);

})(appControllers);