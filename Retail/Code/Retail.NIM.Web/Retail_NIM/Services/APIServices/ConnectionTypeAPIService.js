(function (appControllers) {

    "use strict";
    ConnectionTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_NIM_ModuleConfig'];

    function ConnectionTypeAPIService(BaseAPIService, UtilsService, Retail_NIM_ModuleConfig) {

        var controller = "ConnectionType";

        function GetConnectionTypeInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_NIM_ModuleConfig.moduleName, controller, "GetConnectionTypeInfo"), {
                filter: filter
            });
        }

        return {
            GetConnectionTypeInfo: GetConnectionTypeInfo
        };

    }
    appControllers.service("Retail_NIM_ConnectionTypeAPIService", ConnectionTypeAPIService);
})(appControllers);