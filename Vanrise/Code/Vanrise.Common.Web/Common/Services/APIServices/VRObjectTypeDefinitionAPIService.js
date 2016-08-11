
(function (appControllers) {

    "use strict";
    VRObjectTypeDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRObjectTypeDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRObjectTypeDefinition";


        function GetFilteredVRObjectTypeDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetFilteredVRObjectTypeDefinitions'), input);
        }

        function GetVRObjectTypeDefinition(VRObjectTypeDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetVRObjectTypeDefinition'), {
                VRObjectTypeDefinitionId: VRObjectTypeDefinitionId
            });
        }

        function AddVRObjectTypeDefinition(VRObjectTypeDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'AddVRObjectTypeDefinition'), VRObjectTypeDefinitionItem);
        }

        function UpdateVRObjectTypeDefinition(VRObjectTypeDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateVRObjectTypeDefinition'), VRObjectTypeDefinitionItem);
        }


        return ({
            GetFilteredVRObjectTypeDefinitions: GetFilteredVRObjectTypeDefinitions,
            GetVRObjectTypeDefinition: GetVRObjectTypeDefinition,
            AddVRObjectTypeDefinition: AddVRObjectTypeDefinition,
            UpdateVRObjectTypeDefinition: UpdateVRObjectTypeDefinition,
        });
    }

    appControllers.service('VRCommon_VRObjectTypeDefinitionAPIService', VRObjectTypeDefinitionAPIService);

})(appControllers);