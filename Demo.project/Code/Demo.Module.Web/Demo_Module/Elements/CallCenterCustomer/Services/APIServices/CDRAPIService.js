(function (appControllers) {
    "use strict";
    cdrAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function cdrAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "CDR";

        function GetCDR(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetCDR"), input);
        }

        return {
            GetCDR: GetCDR
        };
    };
    appControllers.service("Demo_Module_CDRAPIService", cdrAPIService);

})(appControllers);