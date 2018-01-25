
(function (appControllers) {

    "use strict";
    ReprocessFilterDefinitionAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Reprocess_ModuleConfig'];

    function ReprocessFilterDefinitionAPIService(BaseAPIService, UtilsService, Reprocess_ModuleConfig) {

        var controllerName = "ReprocessFilterDefinition";

        function GetReprocessFilterDefinitionConfigs() {
            return BaseAPIService.get(UtilsService.getServiceURL(Reprocess_ModuleConfig.moduleName, controllerName, 'GetReprocessFilterDefinitionConfigs'));
        }

        return ({
            GetReprocessFilterDefinitionConfigs: GetReprocessFilterDefinitionConfigs
        });
    }

    appControllers.service('Reprocess_ReprocessFilterDefinitionAPIService', ReprocessFilterDefinitionAPIService);

})(appControllers);