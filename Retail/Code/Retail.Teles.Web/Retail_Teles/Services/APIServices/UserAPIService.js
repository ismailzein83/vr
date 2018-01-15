(function (appControllers) {

    "use strict";
    UserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function UserAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesUser";
        function GetUsersInfo(vrConnectionId, siteId, serializedFilter)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetUsersInfo"), {
                vrConnectionId: vrConnectionId,
                siteId: siteId,
                serializedFilter: serializedFilter,
            });
        }
        function MapUserToAccount(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "MapUserToAccount"), input);
        }
        function GetAccountDIDsCount(accountBEDefinitionId, accountId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetAccountDIDsCount"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId
            });
        }
        function GetUserTelesInfo(accountBEDefinitionId, accountId, vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetUserTelesInfo"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                vrConnectionId: vrConnectionId,
            });
        }
        return ({
            GetUsersInfo: GetUsersInfo,
            MapUserToAccount: MapUserToAccount,
            GetAccountDIDsCount: GetAccountDIDsCount,
            GetUserTelesInfo: GetUserTelesInfo
        });
    }

    appControllers.service('Retail_Teles_UserAPIService', UserAPIService);

})(appControllers);