(function (appControllers) {

    "use strict";
    UserAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function UserAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesUser";
        function GetUsersInfo(vrConnectionId, siteId, serializedFilter) {
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
        function ChangeUserRoutingGroup(accountBEDefinitionId, accountId, vrConnectionId, routingGroupId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "ChangeUserRoutingGroup"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                vrConnectionId: vrConnectionId,
                routingGroupId: routingGroupId
            });
        }
        function GetUserTelesSiteId(accountBEDefinitionId, accountId, vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetUserTelesSiteId"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                vrConnectionId: vrConnectionId,
            });
        }
        function GetCurrentUserRoutingGroupId(accountBEDefinitionId, accountId, vrConnectionId) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetCurrentUserRoutingGroupId"), {
                accountBEDefinitionId: accountBEDefinitionId,
                accountId: accountId,
                vrConnectionId: vrConnectionId,
            });
        }
        return ({
            GetUsersInfo: GetUsersInfo,
            MapUserToAccount: MapUserToAccount,
            GetAccountDIDsCount: GetAccountDIDsCount,
            GetUserTelesInfo: GetUserTelesInfo,
            ChangeUserRoutingGroup: ChangeUserRoutingGroup,
            GetUserTelesSiteId: GetUserTelesSiteId,
            GetCurrentUserRoutingGroupId: GetCurrentUserRoutingGroupId
        });
    }

    appControllers.service('Retail_Teles_UserAPIService', UserAPIService);

})(appControllers);