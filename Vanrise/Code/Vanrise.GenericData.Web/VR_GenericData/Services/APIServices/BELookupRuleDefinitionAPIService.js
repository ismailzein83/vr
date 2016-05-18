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

        function AddBELookupRuleDefinition(beLookupRuleDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddBELookupRuleDefinition'), beLookupRuleDefinition);
        }

        return {
            GetFilteredBELookupRuleDefinitions: GetFilteredBELookupRuleDefinitions,
            GetBELookupRuleDefinitionsInfo: GetBELookupRuleDefinitionsInfo,
            AddBELookupRuleDefinition: AddBELookupRuleDefinition
        };
    }

    appControllers.service('VR_GenericData_BELookupRuleDefinitionAPIService', BELookupRuleDefinitionAPIService);

})(appControllers);