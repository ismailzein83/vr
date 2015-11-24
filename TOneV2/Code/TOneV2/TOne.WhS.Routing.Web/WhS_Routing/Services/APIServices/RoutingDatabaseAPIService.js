(function (appControllers) {

    "use strict";
    routingDatabaseAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function routingDatabaseAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        function GetRoutingDatabaseInfo() {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, "RoutingDatabase", "GetRoutingDatabaseInfo"));
        }

        return ({
            GetRoutingDatabaseInfo: GetRoutingDatabaseInfo
        });
    }

    appControllers.service('WhS_Routing_RoutingDatabaseAPIService', routingDatabaseAPIService);
})(appControllers);