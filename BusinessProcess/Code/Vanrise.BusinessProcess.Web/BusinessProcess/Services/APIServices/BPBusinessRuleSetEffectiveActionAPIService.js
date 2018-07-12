(function (appControllers) {

    "use strict";
    BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig', 'SecurityService'];

    function BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig, SecurityService) {

        function GetFilteredBPBusinessRuleSetsEffectiveActions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", "GetFilteredBPBusinessRuleSetsEffectiveActions"), input);
        }

        function GetRuleActionsExtensionConfigs(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", 'GetRuleActionsExtensionConfigs'), { serializedFilter: serializedFilter });
        }
        function GetParentActionDescription(ruleSetId, ruleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", 'GetParentActionDescription'), { ruleSetId: ruleSetId, ruleDefinitionId: ruleDefinitionId });
        }

        return ({
            GetFilteredBPBusinessRuleSetsEffectiveActions: GetFilteredBPBusinessRuleSetsEffectiveActions,
            GetRuleActionsExtensionConfigs: GetRuleActionsExtensionConfigs,
            GetParentActionDescription: GetParentActionDescription
        });
    }

    appControllers.service('BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService', BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService);

})(appControllers);