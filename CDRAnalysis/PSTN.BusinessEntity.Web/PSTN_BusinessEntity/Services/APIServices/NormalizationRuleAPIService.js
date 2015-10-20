(function (appControllers) {

    "use strict";

    normalizationRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "PSTN_BE_ModuleConfig"];

    function normalizationRuleAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {

        function GetFilteredNormalizationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetFilteredNormalizationRules"), input);
        }
        
        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function GetNormalizationRuleAdjustNumberActionSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleAdjustNumberActionSettingsTemplates"));
        }

        function GetNormalizationRuleSetAreaSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleSetAreaSettingsTemplates"));
        }

        function AddRule(rule) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "AddRule"), rule);
        }

        function UpdateRule(rule) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "UpdateRule"), rule);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "DeleteRule"), {
                ruleId: ruleId
            });
        }

        return ({
            GetFilteredNormalizationRules: GetFilteredNormalizationRules,
            GetRule: GetRule,
            GetNormalizationRuleAdjustNumberActionSettingsTemplates: GetNormalizationRuleAdjustNumberActionSettingsTemplates,
            GetNormalizationRuleSetAreaSettingsTemplates: GetNormalizationRuleSetAreaSettingsTemplates,
            AddRule: AddRule,
            UpdateRule: UpdateRule,
            DeleteRule: DeleteRule
        });
    }

    appControllers.service("NormalizationRuleAPIService", normalizationRuleAPIService);

})(appControllers);
