(function (appControllers) {

    "use strict";
    customerIdentificationRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_CDRProcessing_ModuleConfig'];

    function customerIdentificationRuleAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredCustomerIdentificationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "CustomerIdentificationRule", "GetFilteredCustomerIdentificationRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "CustomerIdentificationRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function AddRule(customerRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "CustomerIdentificationRule", "AddRule"), customerRuleObject);
        }

        function UpdateRule(customerRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "CustomerIdentificationRule", "UpdateRule"), customerRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "CustomerIdentificationRule", "DeleteRule"), { ruleId: ruleId });
        }
        return ({
            GetFilteredCustomerIdentificationRules: GetFilteredCustomerIdentificationRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
        });
    }

    appControllers.service('WhS_CDRProcessing_CustomerIdentificationRuleAPIService', customerIdentificationRuleAPIService);
})(appControllers);