(function (appControllers) {

    "use strict";
    NodeTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_NIM_ModuleConfig'];

    function NodeTypeAPIService(BaseAPIService, UtilsService, Retail_NIM_ModuleConfig) {

        var controller = "NodeType";

        function GetNodeTypeInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_NIM_ModuleConfig.moduleName, controller, "GetNodeTypeInfo"), {
                filter: filter
            });
        }

        return {
            GetNodeTypeInfo: GetNodeTypeInfo
        };

    }
    appControllers.service("Retail_NIM_NodeTypeAPIService", NodeTypeAPIService);
})(appControllers);