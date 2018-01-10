(function (appControllers) {

    'use strict';

    BusinessEntityDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService','SecurityService', 'VR_GenericData_ModuleConfig'];

    function BusinessEntityDefinitionAPIService(BaseAPIService, UtilsService, SecurityService, VR_GenericData_ModuleConfig) {

        var controllerName = "BusinessEntityDefinition";

        function GetFilteredBusinessEntityDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetFilteredBusinessEntityDefinitions'), input);
        }

        function GetBusinessEntityDefinition(businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBusinessEntityDefinition'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        function GetBusinessEntityDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBusinessEntityDefinitionsInfo'), {
                filter: filter
            });
        }

        function GetRemoteBusinessEntityDefinitionsInfo(connectionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetRemoteBusinessEntityDefinitionsInfo'), {
                connectionId: connectionId,
                serializedFilter: serializedFilter
            }); 
        }

        function AddBusinessEntityDefinition(businessEntityDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'AddBusinessEntityDefinition'), businessEntityDefinition);
        }

        function HasAddBusinessEntityDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['AddBusinessEntityDefinition']));
        }

        function UpdateBusinessEntityDefinition(businessEntityDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'UpdateBusinessEntityDefinition'), businessEntityDefinition);
        }

        function HasUpdateBusinessEntityDefinition() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['UpdateBusinessEntityDefinition']));
        }


        function GetBEDefinitionSettingConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBEDefinitionSettingConfigs'));
        }

        function GetBEDataRecordTypeIdIfGeneric(businessEntityDefinitionId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, controllerName, 'GetBEDataRecordTypeIdIfGeneric'), {
                businessEntityDefinitionId: businessEntityDefinitionId
            });
        }

        return {
            GetFilteredBusinessEntityDefinitions: GetFilteredBusinessEntityDefinitions,
            GetBusinessEntityDefinition: GetBusinessEntityDefinition,
            GetBusinessEntityDefinitionsInfo: GetBusinessEntityDefinitionsInfo,
            GetRemoteBusinessEntityDefinitionsInfo: GetRemoteBusinessEntityDefinitionsInfo,
            AddBusinessEntityDefinition: AddBusinessEntityDefinition,
            HasAddBusinessEntityDefinition: HasAddBusinessEntityDefinition,
            UpdateBusinessEntityDefinition: UpdateBusinessEntityDefinition,
            HasUpdateBusinessEntityDefinition: HasUpdateBusinessEntityDefinition,
            GetBEDefinitionSettingConfigs: GetBEDefinitionSettingConfigs,
            GetBEDataRecordTypeIdIfGeneric: GetBEDataRecordTypeIdIfGeneric
        };
    }

    appControllers.service('VR_GenericData_BusinessEntityDefinitionAPIService', BusinessEntityDefinitionAPIService);

})(appControllers); 