(function (appControllers) {

    "use strict";
    genericLKUPDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function genericLKUPDefinitionAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'GenericLKUPDefinition';
        function GetGenericLKUPDefintionTemplateConfigs() {

            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetGenericLKUPDefintionTemplateConfigs"));
        }
        return ({
            GetGenericLKUPDefintionTemplateConfigs: GetGenericLKUPDefintionTemplateConfigs
        });
    }

    appControllers.service('VRCommon_GenericLKUPDefinitionAPIService', genericLKUPDefinitionAPIService);

})(appControllers);