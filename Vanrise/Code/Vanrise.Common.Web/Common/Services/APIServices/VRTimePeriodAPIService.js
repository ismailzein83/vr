
(function (appControllers) {

    "use strict";
    VRTimePeriodAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRCommon_ModuleConfig'];

    function VRTimePeriodAPIService(BaseAPIService, UtilsService, VRCommon_ModuleConfig) {

        var controllerName = "VRTimePeriod";

        function GetVRTimePeriodConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetVRTimePeriodConfigs"), {}, { useCache: true });
        }
        function GetTimePeriod(timePeriodInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRCommon_ModuleConfig.moduleName, controllerName, "GetTimePeriod"), timePeriodInput);
        }
        return ({
            GetVRTimePeriodConfigs: GetVRTimePeriodConfigs,
            GetTimePeriod: GetTimePeriod
        });
    }
    appControllers.service('VRCommon_VRTimePeriodAPIService', VRTimePeriodAPIService);

})(appControllers);

