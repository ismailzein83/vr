(function (appControllers) {

    "use strict";
    salePricingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function salePricingRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {


        function GetFilteredSalePricingRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "GetFilteredSalePricingRules"), input);
        }
        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "GetRule"), {
                ruleId: ruleId
            });
        }
        function AddRule(pricingRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "AddRule"), pricingRuleObj);
        }
        function UpdateRule(pricingRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "UpdateRule"), pricingRuleObj);
        }
        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "SalePricingRule", "DeleteRule"), {
                ruleId: ruleId
            });
        }
        return ({
            AddRule: AddRule,
            GetRule: GetRule,
            UpdateRule:UpdateRule,
            DeleteRule:DeleteRule,
            GetFilteredSalePricingRules: GetFilteredSalePricingRules
        });
    }

    appControllers.service('WhS_BE_SalePricingRuleAPIService', salePricingRuleAPIService);

})(appControllers);