
(function (appControllers) {

    "use strict";
    EntityPersonalizationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function EntityPersonalizationAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = "EntityPersonalization";

        function UpdateEntityPersonalization(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'UpdateEntityPersonalization'), input);
        }

        function DeleteEntityPersonalization(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'DeleteEntityPersonalization'), input);
        }


        function GetCurrentUserEntityPersonalization(entityUniqueNames) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'GetCurrentUserEntityPersonalization'), {
                entityUniqueNames: entityUniqueNames
            });
        }

        function DosesUserHaveUpdateGlobalEntityPersonalization() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, 'DosesUserHaveUpdateGlobalEntityPersonalization'));
        }

        return ({
            UpdateEntityPersonalization: UpdateEntityPersonalization,
            DeleteEntityPersonalization: DeleteEntityPersonalization,
            GetCurrentUserEntityPersonalization: GetCurrentUserEntityPersonalization,
            DosesUserHaveUpdateGlobalEntityPersonalization: DosesUserHaveUpdateGlobalEntityPersonalization
        });
    }

    appControllers.service('VR_Common_EntityPersonalizationAPIService', EntityPersonalizationAPIService);

})(appControllers);