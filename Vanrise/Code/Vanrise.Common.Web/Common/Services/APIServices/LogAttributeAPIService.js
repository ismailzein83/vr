(function (appControllers) {

    "use strict";
    logAttributeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function logAttributeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'LogAttribute';

        function GetSpecificLogAttribute(attribute) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetSpecificLogAttribute"), {
                attribute: attribute
            });

        }

        return ({
            GetSpecificLogAttribute: GetSpecificLogAttribute
        });
    }

    appControllers.service('VRCommon_LogAttributeAPIService', logAttributeAPIService);

})(appControllers);