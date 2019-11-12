(function (appControllers) {

    "use strict";

    TelesAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];
    function TelesAccountAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {
        var controllerName = "TelesAccount";

        function UnmapTelesAccount(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "UnmapTelesAccount"), input);
        }

        return {
            UnmapTelesAccount: UnmapTelesAccount
        };
    }

    appControllers.service('Retail_Teles_TelesAccountAPIService', TelesAccountAPIService);
})(appControllers);


