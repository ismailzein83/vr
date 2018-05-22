
(function (appControllers) {

    "use strict";
    EntityPersonalizationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function EntityPersonalizationAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "EntityPersonalization";

        function UpdateCurrentUserEntityPersonalization(inputs) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateCurrentUserEntityPersonalization'), inputs);
        }

        function UpdateGlobalEntityPersonalization(inputs) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateGlobalEntityPersonalization'), inputs);
        }

        function GetCurrentUserEntityPersonalization(entityUniqueNames) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetCurrentUserEntityPersonalization'), {
                entityUniqueNames: entityUniqueNames
            });
        }

        function DeleteCurrentUserEntityPersonalization(entityUniqueNames) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'DeleteCurrentUserEntityPersonalization'), entityUniqueNames);
        }

        function DeleteGlobalEntityPersonalization(entityUniqueNames) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'DeleteGlobalEntityPersonalization'), entityUniqueNames);
        }

        return ({
            UpdateCurrentUserEntityPersonalization: UpdateCurrentUserEntityPersonalization,
            UpdateGlobalEntityPersonalization: UpdateGlobalEntityPersonalization,
            GetCurrentUserEntityPersonalization: GetCurrentUserEntityPersonalization,
            DeleteCurrentUserEntityPersonalization: DeleteCurrentUserEntityPersonalization,
            DeleteGlobalEntityPersonalization: DeleteGlobalEntityPersonalization
        });
    }

    appControllers.service('VR_Common_EntityPersonalizationAPIService', EntityPersonalizationAPIService);

})(appControllers);