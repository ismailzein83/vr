(function (appControllers) {

    "use strict";
    BusinessProcess_BPVisualEventAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPVisualEventAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        var controllerName = "BPVisualEvent";

        function GetAfterId(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, controllerName, "GetAfterId"), input);
        }

        return ({
            GetAfterId: GetAfterId
        });
    }

    appControllers.service('BusinessProcess_BPVisualEventAPIService', BusinessProcess_BPVisualEventAPIService);

})(appControllers);
