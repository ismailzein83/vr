(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function BELookupRuleDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        var controllerName = 'BELookupRuleDefinition';

        function GetFilteredBELookupRuleDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredBELookupRuleDefinitions'), input);
        }

        function GetBELookupRuleDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBELookupRuleDefinitionsInfo'), {
                filter: filter
            });
        }

        function GetBELookupRuleDefinition(beLookupRuleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBELookupRuleDefinition'), {
                beLookupRuleDefinitionId: beLookupRuleDefinitionId
            });
        }

        function AddBELookupRuleDefinition(beLookupRuleDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddBELookupRuleDefinition'), beLookupRuleDefinition);
        }

        function UpdateBELookupRuleDefinition(beLookupRuleDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateBELookupRuleDefinition'), beLookupRuleDefinition);
        }

        return {
            GetFilteredBELookupRuleDefinitions: GetFilteredBELookupRuleDefinitions,
            GetBELookupRuleDefinitionsInfo: GetBELookupRuleDefinitionsInfo,
            GetBELookupRuleDefinition: GetBELookupRuleDefinition,
            AddBELookupRuleDefinition: AddBELookupRuleDefinition
        };
    }

    appControllers.service('VR_GenericData_BELookupRuleDefinitionAPIService', BELookupRuleDefinitionAPIService);

})(appControllers);