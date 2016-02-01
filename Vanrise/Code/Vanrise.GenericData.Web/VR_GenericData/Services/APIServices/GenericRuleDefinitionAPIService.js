(function (appControllers) {

    'use strict';

    GenericRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredGenericRuleDefinitions: GetFilteredGenericRuleDefinitions,
            GetGenericRuleDefinition: GetGenericRuleDefinition
        };

        function GetFilteredGenericRuleDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetFilteredGenericRuleDefinitions'), input);
        }

        function GetGenericRuleDefinition(genericRuleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetGenericRuleDefinition'), {
                genericRuleDefinitionId: genericRuleDefinitionId
            });
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionAPIService', GenericRuleDefinitionAPIService);

})(appControllers);