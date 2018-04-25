(function (appControllers) {

    "use strict";
    runtimeNodeConfigurationAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Runtime_ModuleConfig', 'SecurityService'];

    function runtimeNodeConfigurationAPIService(BaseAPIService, UtilsService, VRRuntime_ModuleConfig) {

        var controllerName = 'RuntimeNodeConfiguration';

        function GetFilteredRuntimeNodesConfigurations(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetFilteredRuntimeNodesConfigurations"), input);
        }

        function GetRuntimeNodeConfiguration(nodeConfigurationId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetNodeConfiguration"), {
                nodeConfigurationId: nodeConfigurationId
            });
        }

        function UpdateRuntimeNodeConfiguration(nodeConfig) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "UpdateRuntimeNodeConfiguration"), nodeConfig);
        }

        function AddRuntimeNodeConfiguration(nodeConfig) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "AddRuntimeNodeConfiguration"), nodeConfig);
        }
        function GetRuntimeServiceTypeTemplateConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, 'GetRuntimeServiceTypeTemplateConfigs'));
        }

        function GetRuntimeNodeConfigurationsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRRuntime_ModuleConfig.moduleName, controllerName, "GetRuntimeNodeConfigurationsInfo"), {
                filter: filter
            });
        }
        return ({
            GetFilteredRuntimeNodesConfigurations: GetFilteredRuntimeNodesConfigurations,
            GetRuntimeNodeConfiguration: GetRuntimeNodeConfiguration,
            UpdateRuntimeNodeConfiguration: UpdateRuntimeNodeConfiguration,
            AddRuntimeNodeConfiguration: AddRuntimeNodeConfiguration,
            GetRuntimeServiceTypeTemplateConfigs: GetRuntimeServiceTypeTemplateConfigs,
            GetRuntimeNodeConfigurationsInfo: GetRuntimeNodeConfigurationsInfo
        });
    }

    appControllers.service('VRRuntime_RuntimeNodeConfigurationAPIService', runtimeNodeConfigurationAPIService);

})(appControllers);