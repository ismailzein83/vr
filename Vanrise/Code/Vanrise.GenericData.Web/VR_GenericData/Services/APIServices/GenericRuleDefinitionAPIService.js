﻿(function (appControllers) {

    'use strict';

    GenericRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig'];

    function GenericRuleDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig) {
        return {
            GetFilteredGenericRuleDefinitions: GetFilteredGenericRuleDefinitions,
            GetGenericRuleDefinition: GetGenericRuleDefinition,
            AddGenericRuleDefinition: AddGenericRuleDefinition,
            UpdateGenericRuleDefinition: UpdateGenericRuleDefinition,
            DeleteGenericRuleDefinition: DeleteGenericRuleDefinition,
            GetGenericRuleDefinitionSettingsTemplates: GetGenericRuleDefinitionSettingsTemplates,
            GetGenericRuleDefinitionsInfo: GetGenericRuleDefinitionsInfo
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

        function DeleteGenericRuleDefinition(genericRuleDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'DeleteGenericRuleDefinition'), {
                genericRuleDefinitionId: genericRuleDefinitionId
            });
        }

        function GetGenericRuleDefinitionSettingsTemplates() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetGenericRuleDefinitionSettingsTemplates'));
        }

        function GetGenericRuleDefinitionsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_GenericData_ModuleConfig.moduleName, 'GenericRuleDefinition', 'GetGenericRuleDefinitionsInfo'), {
                filter: filter
            });
        }
    }

    appControllers.service('VR_GenericData_GenericRuleDefinitionAPIService', GenericRuleDefinitionAPIService);

})(appControllers);