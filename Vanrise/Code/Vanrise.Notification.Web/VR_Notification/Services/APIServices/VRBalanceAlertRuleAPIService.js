
(function (appControllers) {

    "use strict";
    VRBalanceAlertRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig'];

    function VRBalanceAlertRuleAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig) {

        var controllerName = "VRBalanceAlertRule";

        function GetVRBalanceAlertThresholdConfigs(extensionType) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRBalanceAlertThresholdConfigs"), {
                extensionType: extensionType
            });
        }

        return ({
            GetVRBalanceAlertThresholdConfigs: GetVRBalanceAlertThresholdConfigs,
        });
    }

    appControllers.service('VR_Notification_VRBalanceAlertRuleAPIService', VRBalanceAlertRuleAPIService);

})(appControllers);