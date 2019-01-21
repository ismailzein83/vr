(function (appControllers) {

    "use strict";
    whSJazzAccountCodeAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_Jazz_ModuleConfig"];

    function whSJazzAccountCodeAPIService(BaseAPIService, UtilsService, WhS_Jazz_ModuleConfig) {

        var controllerName = "WhSJazzAccountCode";

        function GetTransctionType(genericBusinessEntityId, businessEntityDefinitionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetTransctionType'), {
                businessEntityDefinitionId: businessEntityDefinitionId,
                trasctionTypeId: genericBusinessEntityId
            });
        }
        
        return ({
            GetTransctionType: GetTransctionType
        });
    }

    appControllers.service("WhS_Jazz_AccountCodeAPIService", whSJazzAccountCodeAPIService);
})(appControllers);