(function (appControllers) {

    "use strict";
    sellingRuleAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Sales_ModuleConfig'];

    function sellingRuleAPIService(BaseAPIService, UtilsService, WhS_Sales_ModuleConfig) {

        function GetFilteredSellingRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "GetFilteredSellingRules"), input);
        }

        function GetRule(sellingRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "GetRule"), {
                ruleId: sellingRuleId
            });
        }

        function AddRule(sellingRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "AddRule"), sellingRuleObject);
        }

        function UpdateRule(sellingRuleObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "UpdateRule"), sellingRuleObject);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "DeleteRule"), { ruleId: ruleId });
        }

        function GetCodeCriteriaGroupTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "GetCodeCriteriaGroupTemplates"));
        }

        function GetSellingRuleSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Sales_ModuleConfig.moduleName, "SellingRule", "GetSellingRuleSettingsTemplates"));
        }

        return ({
            GetFilteredSellingRules: GetFilteredSellingRules,
            GetRule: GetRule,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule,
            GetCodeCriteriaGroupTemplates: GetCodeCriteriaGroupTemplates,
            GetSellingRuleSettingsTemplates: GetSellingRuleSettingsTemplates
        });
    }

    appControllers.service('WhS_Sales_SellingRuleAPIService', sellingRuleAPIService);
})(appControllers);