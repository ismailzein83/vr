(function (appControllers) {

    "use strict";
    BusinessProcess_BPBusinessRuleSetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPBusinessRuleSetAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        function GetFilteredBPBusinessRuleSets(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSet", "GetFilteredBPBusinessRuleSets"), input);
        }

        function AddBusinessRuleSet(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSet", "AddBusinessRuleSet"), input);
        }

        function UpdateBusinessRuleSet(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSet", "UpdateBusinessRuleSet"), input);
        }

        function GetBusinessRuleSetsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSet", "GetBusinessRuleSetsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetBusinessRuleSetsByID(businessRuleSetId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleSet", "GetBusinessRuleSetsByID"), {
                businessRuleSetId: businessRuleSetId
            });
        }

        

        return ({
            GetFilteredBPBusinessRuleSets: GetFilteredBPBusinessRuleSets,
            AddBusinessRuleSet: AddBusinessRuleSet,
            UpdateBusinessRuleSet: UpdateBusinessRuleSet,
            GetBusinessRuleSetsInfo: GetBusinessRuleSetsInfo,
            GetBusinessRuleSetsByID: GetBusinessRuleSetsByID
        });
    }

    appControllers.service('BusinessProcess_BPBusinessRuleSetAPIService', BusinessProcess_BPBusinessRuleSetAPIService);

})(appControllers);