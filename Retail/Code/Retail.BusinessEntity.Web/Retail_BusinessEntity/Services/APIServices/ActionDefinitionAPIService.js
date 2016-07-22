(function (appControllers) {

    "use strict";
    ActionDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_BE_ModuleConfig'];

    function ActionDefinitionAPIService(BaseAPIService, UtilsService, Retail_BE_ModuleConfig) {

        var controllerName = "ActionDefinition";

        function GetFilteredActionDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetFilteredActionDefinitions'), input);
        }

        function GetActionDefinition(actionDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetActionDefinition'), {
                actionDefinitionId: actionDefinitionId
            });
        }

        function AddActionDefinition(actionDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'AddActionDefinition'), actionDefinition);
        }

        function UpdateActionDefinition(actionDefinition) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'UpdateActionDefinition'), actionDefinition);
        }
        function GetActionDefinitionsInfo(filter)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetActionDefinitionsInfo'),
            {
                filter: filter
            });

        }

        function GetActionBPDefinitionExtensionConfigs()
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetActionBPDefinitionExtensionConfigs'));
        }
        function GetProvisionerDefinitionExtensionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_BE_ModuleConfig.moduleName, controllerName, 'GetProvisionerDefinitionExtensionConfigs'));
        }
        return ({
            GetFilteredActionDefinitions: GetFilteredActionDefinitions,
            GetActionDefinition: GetActionDefinition,
            AddActionDefinition: AddActionDefinition,
            UpdateActionDefinition: UpdateActionDefinition,
            GetActionBPDefinitionExtensionConfigs: GetActionBPDefinitionExtensionConfigs,
            GetProvisionerDefinitionExtensionConfigs: GetProvisionerDefinitionExtensionConfigs,
            GetActionDefinitionsInfo: GetActionDefinitionsInfo
        });
    }

    appControllers.service('Retail_BE_ActionDefinitionAPIService', ActionDefinitionAPIService);

})(appControllers);