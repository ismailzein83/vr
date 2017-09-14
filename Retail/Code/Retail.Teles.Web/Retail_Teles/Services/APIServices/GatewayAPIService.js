(function (appControllers) {

    "use strict";
    GatewayAPIService.$inject = ['BaseAPIService', 'UtilsService', 'Retail_Teles_ModuleConfig'];

    function GatewayAPIService(BaseAPIService, UtilsService, Retail_Teles_ModuleConfig) {

        var controllerName = "TelesGateway";
        function GetGatewaysInfo(vrConnectionId, siteId, serializedFilter) {
            return BaseAPIService.get(UtilsService.getServiceURL(Retail_Teles_ModuleConfig.moduleName, controllerName, "GetGatewaysInfo"), {
                vrConnectionId: vrConnectionId,
                siteId:siteId,
                serializedFilter: serializedFilter,
            });
        }
        return ({
            GetGatewaysInfo: GetGatewaysInfo,
        });
    }

    appControllers.service('Retail_Teles_GatewayAPIService', GatewayAPIService);

})(appControllers);