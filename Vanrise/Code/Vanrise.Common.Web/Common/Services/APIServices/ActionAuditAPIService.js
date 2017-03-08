(function (appControllers) {

    "use strict";
    ActionAudit.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function ActionAudit(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controllerName = 'ActionAudit';

        function GetFilteredActionAudits(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetFilteredActionAudits"), input);
        }

        
        return ({
            GetFilteredActionAudits: GetFilteredActionAudits
           
        });
    }

    appControllers.service('VRCommon_ActionAuditAPIService',ActionAudit);

})(appControllers);