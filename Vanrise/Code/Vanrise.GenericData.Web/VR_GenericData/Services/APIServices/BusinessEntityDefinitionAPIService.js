(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function BusinessEntityDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetBusinessEntityDefinition: GetBusinessEntityDefinition,
            GetBusinessEntityDefinitionsInfo: GetBusinessEntityDefinitionsInfo,
            GetFilteredBusinessEntityDefinitions: GetFilteredBusinessEntityDefinitions
        };

        function GetBusinessEntityDefinition(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'GetBusinessEntityDefinition'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        function GetBusinessEntityDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'GetBusinessEntityDefinitionsInfo'), {
                filter: filter
            });
        }
        function GetFilteredBusinessEntityDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'GetFilteredBusinessEntityDefinitions'), input);
        }
    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionAPIService', BusinessEntityDefinitionAPIService);

})(appControllers);