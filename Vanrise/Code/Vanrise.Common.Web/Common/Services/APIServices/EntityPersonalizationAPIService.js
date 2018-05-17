
(function (appControllers) {

    "use strict";
    EntityPersonalizationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function EntityPersonalizationAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "EntityPersonalization";

        function UpdateCurrentUserEntityPersonalization(inputs) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateCurrentUserEntityPersonalization'), inputs);
        }

        function GetCurrentUserEntityPersonalization(entityUniqueNames) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetCurrentUserEntityPersonalization'), {
                entityUniqueNames: entityUniqueNames
            });
        }

        return ({
            UpdateCurrentUserEntityPersonalization: UpdateCurrentUserEntityPersonalization,
            GetCurrentUserEntityPersonalization: GetCurrentUserEntityPersonalization
        });
    }

    appControllers.service('VR_Common_EntityPersonalizationAPIService', EntityPersonalizationAPIService);

})(appControllers);