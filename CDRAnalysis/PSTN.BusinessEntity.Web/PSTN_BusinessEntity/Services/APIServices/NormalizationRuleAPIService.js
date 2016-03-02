(function (appControllers) {

    "use strict";

    normalizationRuleAPIService.$inject = ["BaseAPIService", "UtilsService", "PSTN_BE_ModuleConfig"];

    function normalizationRuleAPIService(BaseAPIService, UtilsService, PSTN_BE_ModuleConfig) {

        var controllerName = 'NormalizationRule';

        function GetFilteredNormalizationRules(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetFilteredNormalizationRules"), input);
        }

        function GetRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetRule"), {
                ruleId: ruleId
            });
        }

        function GetNormalizationRuleAdjustNumberActionSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetNormalizationRuleAdjustNumberActionSettingsTemplates"));
        }

        function GetNormalizationRuleSetAreaSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "GetNormalizationRuleSetAreaSettingsTemplates"));
        }

        function AddRule(rule) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "AddRule"), rule);
        }

        function HasAddRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['AddRule']));
        }

        function UpdateRule(rule) {
            return BaseAPIService.post(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "UpdateRule"), rule);
        }

        function HasUpdateRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['UpdateRule']));
        }

        function DeleteRule(ruleId) {
            return BaseAPIService.get(UtilsService.getServiceURL(PSTN_BE_ModuleConfig.moduleName, controllerName, "DeleteRule"), {
                ruleId: ruleId
            });
        }

        function HasDeleteRulePermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(PSTN_BE_ModuleConfig.moduleName, controllerName, ['DeleteRule']));
        }

        return ({
            HasAddRulePermission: HasAddRulePermission,
            HasUpdateRulePermission: HasUpdateRulePermission,
            HasDeleteRulePermission:HasDeleteRulePermission, 
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
