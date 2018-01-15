(function (appControllers) {

    "use strict";
    TelesAccountAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function TelesAccountAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesAccount";
        function GetAccountTelesInfo(accountBEDefinitionId, accountId, vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetAccountTelesInfo"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                vrConnectionId: vrConnectionId,
            });
        }
        return ({
            GetAccountTelesInfo: GetAccountTelesInfo,
        });
    }

    appControllers.service('Retail_Teles_TelesAccountAPIService', TelesAccountAPIService);

})(appControllers);