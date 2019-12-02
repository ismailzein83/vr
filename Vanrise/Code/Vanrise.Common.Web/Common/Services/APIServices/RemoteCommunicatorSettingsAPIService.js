(function (appControllers) {

    "use strict";
    remoteCommunicatorSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function remoteCommunicatorSettingsAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {
        var controllerName = 'RemoteCommunicatorSettings';

        function GetRemoteCommunicatorSettingsConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetRemoteCommunicatorSettingsConfigs"));
        }

        return {
            GetRemoteCommunicatorSettingsConfigs: GetRemoteCommunicatorSettingsConfigs
        };
    }

    appControllers.service('VRCommon_RemoteCommunicatorSettingsAPIService', remoteCommunicatorSettingsAPIService);

})(appControllers);