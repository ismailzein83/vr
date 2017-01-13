
(function (appControllers) {

    "use strict";

    RecurringChargeDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig', 'SecurityService'];

    function RecurringChargeDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig, SecurityService) {

        var controllerName = "RecurringChargeDefinition";

        function GetFilteredRecurringChargeDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredRecurringChargeDefinitions'), input);
        }

        function GetRecurringChargeDefinition(recurringChargeDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetRecurringChargeDefinition'), {
                recurringChargeDefinitionId: recurringChargeDefinitionId
            });
        }

        function AddRecurringChargeDefinition(recurringChargeDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddRecurringChargeDefinition'), recurringChargeDefinition);
        }

        function HasAddRecurringChargeDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['AddRecurringChargeDefinition']));

        }

        function UpdateRecurringChargeDefinition(recurringChargeDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateRecurringChargeDefinition'), recurringChargeDefinition);
        }

        function HasUpdateRecurringChargeDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Retail_BE_ModuleConfig.moduleName, controllerName, ['UpdateRecurringChargeDefinition']));
        }

        function GetRecurringChargeDefinitionsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, "GetRecurringChargeDefinitionsInfo"));
        }

        return ({
            GetFilteredRecurringChargeDefinitions: GetFilteredRecurringChargeDefinitions,
            GetRecurringChargeDefinition: GetRecurringChargeDefinition,
            AddRecurringChargeDefinition: AddRecurringChargeDefinition,
            HasAddRecurringChargeDefinitionPermission: HasAddRecurringChargeDefinitionPermission,
            UpdateRecurringChargeDefinition: UpdateRecurringChargeDefinition,
            HasUpdateRecurringChargeDefinitionPermission: HasUpdateRecurringChargeDefinitionPermission,
            GetRecurringChargeDefinitionsInfo: GetRecurringChargeDefinitionsInfo
        });
    }

    appControllers.service('Retail_BE_RecurringChargeDefinitionAPIService', RecurringChargeDefinitionAPIService);

})(appControllers);