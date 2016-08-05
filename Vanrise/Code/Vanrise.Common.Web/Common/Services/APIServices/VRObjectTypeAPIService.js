
(function (appControllers) {

    "use strict";
    VRObjectTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRObjectTypeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRObjectType";


        function GetObjectTypeExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetObjectTypeExtensionConfigs"), {}, { useCache: true });
        }


        return ({
            GetObjectTypeExtensionConfigs: GetObjectTypeExtensionConfigs,
        });
    }

    appControllers.service('VRCommon_VRObjectTypeAPIService', VRObjectTypeAPIService);

})(appControllers);

