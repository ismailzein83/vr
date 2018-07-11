(function (appControllers) {
    "use strict";
    serviceAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function serviceAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Service";

        function GetServices() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetServices"));
        }

        return {
            GetServices: GetServices
        };
    };
    appControllers.service("Demo_Module_ServiceAPIService", serviceAPIService);

})(appControllers);