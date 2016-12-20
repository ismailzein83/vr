
(function (appControllers) {

    "use strict";
    TechnicalSettingsAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function TechnicalSettingsAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {

        var controllerName = "TechnicalSettings";

        function GetDocumentItemDefinitionsInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetDocumentItemDefinitionsInfo"), {});
        };

        return ({
            GetDocumentItemDefinitionsInfo: GetDocumentItemDefinitionsInfo,
        });
    }

    appControllers.service('WhS_BE_TechnicalSettingsAPIService', TechnicalSettingsAPIService);

})(appControllers);