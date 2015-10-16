(function (appControllers) {

    "use strict";

    normalizationRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "PSTN_BE_ModuleConfig"];

    function normalizationRuleAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {

        function GetFilteredNormalizationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetFilteredNormalizationRules"), input);
        }
        
        function GetNormalizationRuleById(normalizationRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleById"), {
                normalizationRuleId: normalizationRuleId
            });
        }

        function GetNormalizationRuleAdjustNumberActionSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleAdjustNumberActionSettingsTemplates"));
        }

        function GetNormalizationRuleSetAreaSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "GetNormalizationRuleSetAreaSettingsTemplates"));
        }

        function AddNormalizationRule(normalizationRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "AddNormalizationRule"), normalizationRuleObj);
        }

        function UpdateNormalizationRule(normalizationRuleObj) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "UpdateNormalizationRule"), normalizationRuleObj);
        }

        function DeleteNormalizationRule(normalizationRuleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, "NormalizationRule", "DeleteNormalizationRule"), {
                normalizationRuleId: normalizationRuleId
            });
        }

        return ({
            GetFilteredNormalizationRules: GetFilteredNormalizationRules,
            GetNormalizationRuleById: GetNormalizationRuleById,
            GetNormalizationRuleAdjustNumberActionSettingsTemplates: GetNormalizationRuleAdjustNumberActionSettingsTemplates,
            GetNormalizationRuleSetAreaSettingsTemplates: GetNormalizationRuleSetAreaSettingsTemplates,
            AddNormalizationRule: AddNormalizationRule,
            UpdateNormalizationRule: UpdateNormalizationRule,
            DeleteNormalizationRule: DeleteNormalizationRule
        });
    }

    appControllers.service("NormalizationRuleAPIService", normalizationRuleAPIService);

})(appControllers);
