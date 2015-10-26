(function (appControllers) {

    "use strict";
    setCustomerRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CDRProcessing_ModuleConfig'];

    function setCustomerRuleAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredSetCustomerRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetCustomerRule", "GetFilteredSetCustomerRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetCustomerRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function AddRule(customerRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetCustomerRule", "AddRule"), customerRuleObject);
        }

        function UpdateRule(customerRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetCustomerRule", "UpdateRule"), customerRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "SetCustomerRule", "DeleteRule"), { ruleId: ruleId });
        }
        return ({
            GetFilteredSetCustomerRules: GetFilteredSetCustomerRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
        });
    }

    appControllers.service('WhS_CDRProcessing_SetCustomerRuleAPIService', setCustomerRuleAPIService);
})(appControllers);