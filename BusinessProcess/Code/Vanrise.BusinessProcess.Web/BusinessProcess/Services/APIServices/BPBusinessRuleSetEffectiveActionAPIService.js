(function (appControllers) {

    "use strict";
    BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig', 'SecurityService'];

    function BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig, SecurityService) {

        function GetBPBusinessRuleSetsEffectiveActions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", "GetBPBusinessRuleSetsEffectiveActions"), input);
        }

        function GetRuleActionsExtensionConfigs(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", 'GetRuleActionsExtensionConfigs'), { serializedFilter: serializedFilter });
        }
        function GetParentActionDescription(ruleSetId, ruleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", 'GetParentActionDescription'), { ruleSetId: ruleSetId, ruleDefinitionId: ruleDefinitionId });
        }
        function GetRuleSetEffectiveActions(bpBusinessRuleSetId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSetEffectiveAction", 'GetRuleSetEffectiveActions'), { bpBusinessRuleSetId: bpBusinessRuleSetId });
        }

        return ({
            GetBPBusinessRuleSetsEffectiveActions: GetBPBusinessRuleSetsEffectiveActions,
            GetRuleActionsExtensionConfigs: GetRuleActionsExtensionConfigs,
            GetParentActionDescription: GetParentActionDescription,
            GetRuleSetEffectiveActions: GetRuleSetEffectiveActions
        });
    }

    appControllers.service('BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService', BusinessProcess_BPBusinessRuleSetEffectiveActionAPIService);

})(appControllers);

