(function (appControllers) {

    "use strict";

    normalizationRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_CDRProcessing_ModuleConfig"];

    function normalizationRuleAPIService(BaseAPIService, UtilsService, WhS_CDRProcessing_ModuleConfig) {

        function GetFilteredNormalizationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "GetFilteredNormalizationRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "GetRule"), {
                ruleId: ruleId
            });
        }

        function GetNormalizationRuleAdjustNumberActionSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleAdjustNumberActionSettingsTemplates"));
        }

        function GetNormalizationRuleSetAreaSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleSetAreaSettingsTemplates"));
        }

        function AddRule(rule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "AddRule"), rule);
        }

        function UpdateRule(rule) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "UpdateRule"), rule);
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_CDRProcessing_ModuleConfig.moduleName, "NormalizationRule", "DeleteRule"), {
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

    appControllers.service("WhS_CDRProcessing_NormalizationRuleAPIService", normalizationRuleAPIService);

})(appControllers);
