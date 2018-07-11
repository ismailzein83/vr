(function (appControllers) {
    "use strict";
    contractAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Demo_Module_ModuleConfig', 'SecurityService'];
    function contractAPIService(BaseAPIService, UtilsService, Demo_Module_ModuleConfig, SecurityService) {

        var controller = "Contract";

        function GetContract() {
            return BaseAPIService.get(UtilsService.getServiceURL(Demo_Module_ModuleConfig.moduleName, controller, "GetContract"));
        }

        return {
            GetContract: GetContract
        };
    };
    appControllers.service("Demo_Module_ContractAPIService", contractAPIService);

})(appControllers);