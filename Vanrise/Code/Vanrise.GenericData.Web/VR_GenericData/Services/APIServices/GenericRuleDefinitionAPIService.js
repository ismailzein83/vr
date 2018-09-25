(function (appControllers) {

    'use strict';

    GenericRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'SecurityService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredGenericRuleDefinitions: GetFilteredGenericRuleDefinitions,
            GetGenericRuleDefinition: GetGenericRuleDefinition,
            AddGenericRuleDefinition: AddGenericRuleDefinition,
            HasAddGenericRuleDefinition: HasAddGenericRuleDefinition,
            UpdateGenericRuleDefinition: UpdateGenericRuleDefinition,
            HasUpdateGenericRuleDefinition: HasUpdateGenericRuleDefinition,
            GetGenericRuleDefinitionsInfo: GetGenericRuleDefinitionsInfo,
            GetCriteriaDefinitionConfigs: GetCriteriaDefinitionConfigs
        };

        function GetFilteredGenericRuleDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetFilteredGenericRuleDefinitions'), input);
        }

        function GetGenericRuleDefinition(genericRuleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetGenericRuleDefinition'), {
                genericRuleDefinitionId: genericRuleDefinitionId
            });
        }

        function AddGenericRuleDefinition(genericRuleDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'AddGenericRuleDefinition'), genericRuleDefinition);
        }
        function HasAddGenericRuleDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "GenericRuleDefinition", ['AddGenericRuleDefinition']));
        }
        function UpdateGenericRuleDefinition(genericRuleDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'UpdateGenericRuleDefinition'), genericRuleDefinition);
        }
        function HasUpdateGenericRuleDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "GenericRuleDefinition", ['UpdateGenericRuleDefinition']));
        }
        function GetGenericRuleDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetGenericRuleDefinitionsInfo'), {
                filter: filter
            });
        }

        function GetCriteriaDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetCriteriaDefinitionConfigs'));
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionAPIService', GenericRuleDefinitionAPIService);

})(appControllers);