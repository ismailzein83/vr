
(function (appControllers) {

    "use strict";

    VRAlertRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig', 'SecurityService'];

    function VRAlertRuleAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig, SecurityService) {

        var controllerName = "VRAlertRule";


        function GetFilteredVRAlertRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetFilteredVRAlertRules'), input);
        }

        function GetVRAlertRule(vrAlertRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetVRAlertRule'), {
                VRAlertRuleId: vrAlertRuleId
            });
        }

        function AddVRAlertRule(vrAlertRuleItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'AddVRAlertRule'), vrAlertRuleItem);
        }

        function UpdateVRAlertRule(vrAlertRuleItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'UpdateVRAlertRule'), vrAlertRuleItem);
        }

        function HasAddVRAlertRulePermission() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'DoesUserHaveAddAlertRulePermission'));
        }

        function EnableVRAlertRule(enableInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'EnableVRAlertRule'), enableInput);
        }

        function DisableVRAlertRule(disableInput) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'DisableVRAlertRule'), disableInput);
        }
        
        return ({
            GetFilteredVRAlertRules: GetFilteredVRAlertRules,
            GetVRAlertRule: GetVRAlertRule,
            AddVRAlertRule: AddVRAlertRule,
            UpdateVRAlertRule: UpdateVRAlertRule,
            HasAddVRAlertRulePermission: HasAddVRAlertRulePermission,
            EnableVRAlertRule: EnableVRAlertRule,
            DisableVRAlertRule: DisableVRAlertRule
        });
    }

    appControllers.service('VR_Notification_VRAlertRuleAPIService', VRAlertRuleAPIService);

})(appControllers);