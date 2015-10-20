(function (appControllers) {

    "use strict";
    purchasePricingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function purchasePricingRuleAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {


        function GetFilteredPurchasePricingRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PurchasePricingRule", "GetFilteredPurchasePricingRules"), input);
        }
        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PurchasePricingRule", "GetRule"), {
                ruleId: ruleId
            });
        }
        function AddRule(pricingRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PurchasePricingRule", "AddRule"), pricingRuleObj);
        }
        function UpdateRule(pricingRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PurchasePricingRule", "UpdateRule"), pricingRuleObj);
        }
        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, "PurchasePricingRule", "DeleteRule"), {
                ruleId: ruleId
            });
        }
        return ({
            AddRule: AddRule,
            GetRule: GetRule,
            DeleteRule: DeleteRule,
            UpdateRule: UpdateRule,
            GetFilteredPurchasePricingRules: GetFilteredPurchasePricingRules
        });
    }

    appControllers.service('WhS_BE_PurchasePricingRuleAPIService', purchasePricingRuleAPIService);

})(appControllers);