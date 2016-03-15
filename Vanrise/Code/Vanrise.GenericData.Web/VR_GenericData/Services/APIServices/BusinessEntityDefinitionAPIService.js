(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService','SecurityService', 'VR_GenericData_ModuleConfig'];

    function BusinessEntityDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {
        return {
            GetBusinessEntityDefinition: GetBusinessEntityDefinition,
            GetBusinessEntityDefinitionsInfo: GetBusinessEntityDefinitionsInfo,
            GetFilteredBusinessEntityDefinitions: GetFilteredBusinessEntityDefinitions,
            AddBusinessEntityDefinition: AddBusinessEntityDefinition,
            HasAddBusinessEntityDefinition: HasAddBusinessEntityDefinition,
            UpdateBusinessEntityDefinition: UpdateBusinessEntityDefinition,
            GetGenericBEDefinitionView: GetGenericBEDefinitionView
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
        function AddBusinessEntityDefinition(businessEntityDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'AddBusinessEntityDefinition'), businessEntityDefinition);
        }
        function HasAddBusinessEntityDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, "BusinessEntityDefinition", ['AddBusinessEntityDefinition']));
        }
        function UpdateBusinessEntityDefinition(businessEntityDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'UpdateBusinessEntityDefinition'), businessEntityDefinition);
        }

        function GetGenericBEDefinitionView(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'BusinessEntityDefinition', 'GetGenericBEDefinitionView'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }
    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionAPIService', BusinessEntityDefinitionAPIService);

})(appControllers);