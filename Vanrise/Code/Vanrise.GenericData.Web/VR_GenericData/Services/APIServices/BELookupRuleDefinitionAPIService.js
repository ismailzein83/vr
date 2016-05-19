(function (appControllers) {

    'use strict';

    BELookupRuleDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_GenericData_ModuleConfig', 'SecurityService'];

    function BELookupRuleDefinitionAPIService(BaseAPIService, UtilsService, VR_GenericData_ModuleConfig, SecurityService) {
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

        function HasAddBELookupRuleDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['AddBELookupRuleDefinition']));
        }

        function HasEditBELookupRuleDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_GenericData_ModuleConfig.moduleName, controllerName, ['UpdateBELookupRuleDefinition']));
        }

        return {
            GetFilteredBELookupRuleDefinitions: GetFilteredBELookupRuleDefinitions,
            GetBELookupRuleDefinitionsInfo: GetBELookupRuleDefinitionsInfo,
            GetBELookupRuleDefinition: GetBELookupRuleDefinition,
            AddBELookupRuleDefinition: AddBELookupRuleDefinition,
            UpdateBELookupRuleDefinition: UpdateBELookupRuleDefinition,
            HasAddBELookupRuleDefinitionPermission: HasAddBELookupRuleDefinitionPermission,
            HasEditBELookupRuleDefinitionPermission: HasEditBELookupRuleDefinitionPermission
        };
    }

    appControllers.service('VR_GenericData_BELookupRuleDefinitionAPIService', BELookupRuleDefinitionAPIService);

})(appControllers);