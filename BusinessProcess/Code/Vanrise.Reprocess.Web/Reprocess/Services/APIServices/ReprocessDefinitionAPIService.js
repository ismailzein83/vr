
(function (appControllers) {

    "use strict";
    ReprocessDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Reprocess_ModuleConfig'];

    function ReprocessDefinitionAPIService(BaseAPIService, UtilsService, Reprocess_ModuleConfig) {

        var controllerName = "ReprocessDefinition";


        function GetFilteredReprocessDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetFilteredReprocessDefinitions'), input);
        }

        function GetReprocessDefinition(reprocessDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetReprocessDefinition'), {
                ReprocessDefinitionId: reprocessDefinitionId
            });
        }

        function AddReprocessDefinition(reprocessDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'AddReprocessDefinition'), reprocessDefinitionItem);
        }

        function UpdateReprocessDefinition(reprocessDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'UpdateReprocessDefinition'), reprocessDefinitionItem);
        }


        return ({
            GetFilteredReprocessDefinitions: GetFilteredReprocessDefinitions,
            GetReprocessDefinition: GetReprocessDefinition,
            AddReprocessDefinition: AddReprocessDefinition,
            UpdateReprocessDefinition: UpdateReprocessDefinition,
        });
    }

    appControllers.service('Reprocess_ReprocessDefinitionAPIService', ReprocessDefinitionAPIService);

})(appControllers);