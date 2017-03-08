
(function (appControllers) {

    'use strict';

    ActionAuditLKUPAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig', 'SecurityService'];

    function ActionAuditLKUPAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig, SecurityService) {

        var controller = 'VRActionAuditLKUP';

       
        function GetVRActionAuditLKUPInfo(filter) {
         
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controller, "GetVRActionAuditLKUPInfo"), {
                filter: filter
            });
        }
    
       
        return ({
            GetVRActionAuditLKUPInfo: GetVRActionAuditLKUPInfo
        });
    }


    appControllers.service('VRCommon_ActionAuditLKUPAPIService', ActionAuditLKUPAPIService);
})(appControllers);