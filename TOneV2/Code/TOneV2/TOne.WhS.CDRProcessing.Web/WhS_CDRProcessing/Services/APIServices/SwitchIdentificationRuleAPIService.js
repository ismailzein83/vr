(function (appControllers) {

    "use strict";
    switchIdentificationRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CDRProcessing_ModuleConfig'];

    function switchIdentificationRuleAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredSwitchIdentificationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SwitchIdentificationRule", "GetFilteredSwitchIdentificationRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SwitchIdentificationRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function AddRule(customerRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SwitchIdentificationRule", "AddRule"), customerRuleObject);
        }

        function UpdateRule(customerRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SwitchIdentificationRule", "UpdateRule"), customerRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SwitchIdentificationRule", "DeleteRule"), { ruleId: ruleId });
        }
        return ({
            GetFilteredSwitchIdentificationRules: GetFilteredSwitchIdentificationRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
        });
    }

    appControllers.service('WhS_CDRProcessing_SwitchIdentificationRuleAPIService', switchIdentificationRuleAPIService);
})(appControllers);