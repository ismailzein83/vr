(function (appControllers) {

    "use strict";
    runtimeNodeStateAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function runtimeNodeStateAPIService(BaseAPIService, UtilsService, VRRuntime_ModuleConfig) {

        var controllerName = 'RuntimeNodeState';


        function GetAllNodesStates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetAllNodesStates"));
        }


        function GetRuntimeNodeState(nodeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetNodeState"), {
                nodeId: nodeId
            });
        }

        return ({
            GetAllNodesStates:GetAllNodesStates,
            GetRuntimeNodeState: GetRuntimeNodeState,
        });
    }

    appControllers.service('VRRuntime_RuntimeNodeStateAPIService', runtimeNodeStateAPIService);

})(appControllers);