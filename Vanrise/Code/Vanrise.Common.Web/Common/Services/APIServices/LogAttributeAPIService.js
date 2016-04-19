(function (appControllers) {

    "use strict";
    logAttributeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function logAttributeAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {
        var controllerName = 'LogAttribute';

        function GetLogAttributesById(attribute) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetLogAttributesById"), {
                attribute: attribute
            });

        }

        return ({
            GetLogAttributesById: GetLogAttributesById
        });
    }

    appControllers.service('VRCommon_LogAttributeAPIService', logAttributeAPIService);

})(appControllers);