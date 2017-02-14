(function (appControllers) {

    "use strict";
    EnterpriseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function EnterpriseAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesEnterprise";

        function GetEnterprisesInfo(vrConnectionId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetEnterprisesInfo"), {
                vrConnectionId: vrConnectionId,
                serializedFilter: serializedFilter,
            });
        }

        function MapEnterpriseToAccount(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "MapEnterpriseToAccount"), input);
        }
        return ({
            GetEnterprisesInfo: GetEnterprisesInfo,
            MapEnterpriseToAccount: MapEnterpriseToAccount
        });
    }

    appControllers.service('Retail_Teles_EnterpriseAPIService', EnterpriseAPIService);

})(appControllers);