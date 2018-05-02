(function (appControllers) {

    "use strict";
    runtimeNodeAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function runtimeNodeAPIService(BaseAPIService, UtilsService, VRRuntime_ModuleConfig) {

        var controllerName = 'RuntimeNode';


        function GetAllNodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetAllNodes"));
        }

        function GetRuntimeNode(nodeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetNode"), {
                nodeId: nodeId
            });
        }

        function UpdateRuntimeNode(node) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "UpdateRuntimeNode"), node);
        }

        function AddRuntimeNode(node) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "AddRuntimeNode"), node);
        }

        return ({
            GetAllNodes: GetAllNodes,
            GetRuntimeNode: GetRuntimeNode,
            UpdateRuntimeNode: UpdateRuntimeNode,
            AddRuntimeNode: AddRuntimeNode,
        });
    }

    appControllers.service('VRRuntime_RuntimeNodeAPIService', runtimeNodeAPIService);

})(appControllers);