(function (appControllers) {

    "use strict";
    BusinessProcess_BPDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'BusinessProcess_BP_ModuleConfig', 'SecurityService'];

    function BusinessProcess_BPDefinitionAPIService(BaseAPIService, UtilsService, BusinessProcess_BP_ModuleConfig, SecurityService) {

        function GetFilteredBPDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", "GetFilteredBPDefinitions"), input);
        }
        function GetFilteredBPDefinitionsForTechnical(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", "GetFilteredBPDefinitionsForTechnical"), input);
        }

        function GetBPDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", "GetBPDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function GetBPDefintion(bpDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", "GetBPDefintion"), {
                bpDefinitionId: bpDefinitionId
            });
        }

        function GetBPDefinitions() {
            return BaseAPIService.get(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", "GetDefinitions"));
        }
        function UpdateBPDefinition(BPDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", 'UpdateBPDefinition'), BPDefinition);
        }

        function HasUpdateBPDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(BusinessProcess_BP_ModuleConfig.moduleName, "BPDefinition", ['UpdateBPDefinition']));
        }
        return ({
            GetFilteredBPDefinitions: GetFilteredBPDefinitions,
            GetFilteredBPDefinitionsForTechnical:GetFilteredBPDefinitionsForTechnical,
            GetBPDefinitionsInfo: GetBPDefinitionsInfo,
            GetBPDefintion: GetBPDefintion,
            GetBPDefinitions: GetBPDefinitions,
            UpdateBPDefinition: UpdateBPDefinition,
            HasUpdateBPDefinitionPermission: HasUpdateBPDefinitionPermission
        });
    }

    appControllers.service('BusinessProcess_BPDefinitionAPIService', BusinessProcess_BPDefinitionAPIService);

})(appControllers);