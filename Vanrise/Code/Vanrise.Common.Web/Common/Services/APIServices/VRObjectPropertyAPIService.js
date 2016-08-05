
(function (appControllers) {

    "use strict";
    VRObjectPropertyAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRObjectPropertyAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRObjectProperty";


        function GetObjectPropertyExtensionConfigs(configType) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetObjectPropertyExtensionConfigs"), { configType: configType }, {useCache: true});
        }


        return ({
            GetObjectPropertyExtensionConfigs: GetObjectPropertyExtensionConfigs,
        });
    }

    appControllers.service('VRCommon_VRObjectPropertyAPIService', VRObjectPropertyAPIService);

})(appControllers);
