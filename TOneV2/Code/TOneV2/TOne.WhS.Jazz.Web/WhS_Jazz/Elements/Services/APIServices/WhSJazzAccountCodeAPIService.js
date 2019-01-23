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

        function GetAllAccountCodes() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAllAccountCodes'), {
            });
        }
        function GetAccountCodesInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Jazz_ModuleConfig.moduleName, controllerName, 'GetAccountCodesInfo'), {
                filter: filter
            });
        }
        
        return ({
            GetTransctionType: GetTransctionType,
            GetAccountCodesInfo: GetAccountCodesInfo,
            GetAllAccountCodes: GetAllAccountCodes
        });
    }

    appControllers.service("WhS_Jazz_AccountCodeAPIService", whSJazzAccountCodeAPIService);
})(appControllers);