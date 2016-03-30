(function (appControllers) {

    "use strict";
    BusinessProcess_BPBusinessRuleDefintionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig'];

    function BusinessProcess_BPBusinessRuleDefintionAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig) {

        function GetBusinessRuleDefintionsByBPDefinitionID(bpDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPBusinessRuleDefinition", "GetBusinessRuleDefintionsByBPDefinitionID"), {
                bpDefinitionId: bpDefinitionId
            });
        }

        return ({
            GetBusinessRuleDefintionsByBPDefinitionID: GetBusinessRuleDefintionsByBPDefinitionID
        });
    }

    appControllers.service('BusinessProcess_BPBusinessRuleDefintionAPIService', BusinessProcess_BPBusinessRuleDefintionAPIService);

})(appControllers);