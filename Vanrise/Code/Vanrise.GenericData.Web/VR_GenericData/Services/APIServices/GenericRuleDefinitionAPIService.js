(function (appControllers) {

    'use strict';

    GenericRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredGenericRuleDefinitions: GetFilteredGenericRuleDefinitions,
            GetGenericRuleDefinition: GetGenericRuleDefinition,
            AddGenericRuleDefinition: AddGenericRuleDefinition,
            UpdateGenericRuleDefinition: UpdateGenericRuleDefinition
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

        function UpdateGenericRuleDefinition(genericRuleDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'UpdateGenericRuleDefinition'), genericRuleDefinition);
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionAPIService', GenericRuleDefinitionAPIService);

})(appControllers);