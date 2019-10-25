(function (appControllers) {

    "use strict";
    NodePortTypeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_NIM_ModuleConfig'];

    function NodePortTypeAPIService(BaseAPIService, UtilsService, Retail_NIM_ModuleConfig) {

        var controller = "NodePortType";

        function GetNodePortTypeInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_NIM_ModuleConfig.moduleName, controller, "GetNodePortTypeInfo"), {
                filter: filter
            });
        }

        return {
            GetNodePortTypeInfo: GetNodePortTypeInfo
        };

    }
    appControllers.service("Retail_NIM_NodePortTypeAPIService", NodePortTypeAPIService);
})(appControllers);