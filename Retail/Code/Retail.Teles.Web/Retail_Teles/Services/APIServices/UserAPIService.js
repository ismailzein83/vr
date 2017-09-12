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

        return ({
            GetUsersInfo: GetUsersInfo,
            MapUserToAccount: MapUserToAccount
        });
    }

    appControllers.service('Retail_Teles_UserAPIService', UserAPIService);

})(appControllers);