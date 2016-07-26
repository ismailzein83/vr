
(function (appControllers) {

    "use strict";
    ReprocessDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Reprocess_ModuleConfig'];

    function ReprocessDefinitionAPIService(BaseAPIService, UtilsService, Reprocess_ModuleConfig) {

        var controllerName = "ReprocessDefinition";


        function GetFilteredReprocessDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetFilteredReprocessDefinitions'), input);
        }

        function GetReprocessDefinition(ReprocessDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetReprocessDefinition'), {
                ReprocessDefinitionId: ReprocessDefinitionId
            });
        }

        function AddReprocessDefinition(ReprocessDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'AddReprocessDefinition'), ReprocessDefinitionItem);
        }

        function UpdateReprocessDefinition(ReprocessDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'UpdateReprocessDefinition'), ReprocessDefinitionItem);
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