
(function (appControllers) {

    "use strict";
    VRActionDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRActionDefinitionAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRActionDefinition";

        function GetVRActionDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRActionDefinitionConfigs"));
        };

        function GetVRActionDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRActionDefinitionsInfo"), {
                filter: filter
            });
        };


        function GetVRActionDefinition(vrActionDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetVRActionDefinition'), {
                VRActionDefinitionId: vrActionDefinitionId
            });
        };

        return ({
            GetVRActionDefinitionConfigs: GetVRActionDefinitionConfigs,
            GetVRActionDefinitionsInfo: GetVRActionDefinitionsInfo,
            GetVRActionDefinition: GetVRActionDefinition
        });
    }

    appControllers.service('VR_Notification_VRActionDefinitionAPIService', VRActionDefinitionAPIService);

})(appControllers);