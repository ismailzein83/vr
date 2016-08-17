
(function (appControllers) {

    "use strict";
    ReprocessDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Reprocess_ModuleConfig', 'SecurityService'];

    function ReprocessDefinitionAPIService(BaseAPIService, UtilsService, Reprocess_ModuleConfig, SecurityService) {

        var controllerName = "ReprocessDefinition";


        function GetFilteredReprocessDefinitions(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetFilteredReprocessDefinitions'), input);
        }

        function GetReprocessDefinition(reprocessDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetReprocessDefinition'), {
                ReprocessDefinitionId: reprocessDefinitionId
            });
        }

        function GetReprocessDefinitionsInfo(serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, "GetReprocessDefinitionsInfo"), {
                serializedFilter: serializedFilter
            });
        }

        function AddReprocessDefinition(reprocessDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'AddReprocessDefinition'), reprocessDefinitionItem);
        }
        function HasAddReprocessDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Reprocess_ModuleConfig.moduleName, controllerName, ['AddReprocessDefinition']));
        }
        function UpdateReprocessDefinition(reprocessDefinitionItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'UpdateReprocessDefinition'), reprocessDefinitionItem);
        }
        function HasUpdateReprocessDefinitionPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(Reprocess_ModuleConfig.moduleName, controllerName, ['UpdateReprocessDefinition']));
        }
        return ({
            GetFilteredReprocessDefinitions: GetFilteredReprocessDefinitions,
            GetReprocessDefinition: GetReprocessDefinition,
            AddReprocessDefinition: AddReprocessDefinition,
            HasAddReprocessDefinitionPermission:HasAddReprocessDefinitionPermission,
            UpdateReprocessDefinition: UpdateReprocessDefinition,
            HasUpdateReprocessDefinitionPermission:HasUpdateReprocessDefinitionPermission,
            GetReprocessDefinitionsInfo: GetReprocessDefinitionsInfo
        });
    }

    appControllers.service('Reprocess_ReprocessDefinitionAPIService', ReprocessDefinitionAPIService);

})(appControllers);