(function (appControllers) {


    "use strict";
    overriddenConfigGroupAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function overriddenConfigGroupAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'OverriddenConfigurationGroup';
        function GetOverriddenConfigurationGroupInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetOverriddenConfigurationGroupInfo"), {
                filter: filter
            });
        }
        function AddOverriddenConfigurationGroup(overriddenConfigGroupObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "AddOverriddenConfigurationGroup"), overriddenConfigGroupObject);
        }
        return ({
            GetOverriddenConfigurationGroupInfo: GetOverriddenConfigurationGroupInfo,
            AddOverriddenConfigurationGroup: AddOverriddenConfigurationGroup
        });
    }

    appControllers.service('VRCommon_OverriddenConfigGroupAPIService', overriddenConfigGroupAPIService);

})(appControllers);