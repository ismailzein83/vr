(function (appControllers) {

    "use strict";
    NodePartTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_NIM_ModuleConfig'];

    function NodePartTypeAPIService(BaseAPIService, UtilsService, Retail_NIM_ModuleConfig) {

        var controller = "NodePartType";

        function GetNodePartTypeInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_NIM_ModuleConfig.moduleName, controller, "GetNodePartTypeInfo"), {
                filter: filter
            });
        }

        return {
            GetNodePartTypeInfo: GetNodePartTypeInfo
        };

    }
    appControllers.service("Retail_NIM_NodePartTypeAPIService", NodePartTypeAPIService);
})(appControllers);