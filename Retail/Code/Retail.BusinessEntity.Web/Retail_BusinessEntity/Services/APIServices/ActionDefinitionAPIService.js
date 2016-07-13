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

        return ({
            GetFilteredActionDefinitions: GetFilteredActionDefinitions,
            GetActionDefinition: GetActionDefinition,
            AddActionDefinition: AddActionDefinition,
            UpdateActionDefinition: UpdateActionDefinition
        });
    }

    appControllers.service('Retail_BE_ActionDefinitionAPIService', ActionDefinitionAPIService);

})(appControllers);