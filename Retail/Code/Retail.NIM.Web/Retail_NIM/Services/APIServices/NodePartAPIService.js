(function (appControllers) {

    "use strict";
    NodePartAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_NIM_ModuleConfig'];

    function NodePartAPIService(BaseAPIService, UtilsService, Retail_NIM_ModuleConfig) {

        var controller = "NodePart";

        function GetNodePartTree(nodeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_NIM_ModuleConfig.moduleName, controller, "GetNodePartTree"), {
                nodeId: nodeId
            });
        }

        return {
            GetNodePartTree: GetNodePartTree
        };

    }
    appControllers.service("Retail_NIM_NodePartAPIService", NodePartAPIService);
})(appControllers);