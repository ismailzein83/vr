
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
        function GetVRBalanceActionTargetTypeByRuleTypeId(alertRuleTypeId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetVRBalanceActionTargetTypeByRuleTypeId"), {
                alertRuleTypeId: alertRuleTypeId
            });
        }
        return ({
            GetVRBalanceAlertThresholdConfigs: GetVRBalanceAlertThresholdConfigs,
            GetVRBalanceActionTargetTypeByRuleTypeId: GetVRBalanceActionTargetTypeByRuleTypeId
        });
    }

    appControllers.service('VR_Notification_VRBalanceAlertRuleAPIService', VRBalanceAlertRuleAPIService);

})(appControllers);