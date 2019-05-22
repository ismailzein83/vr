(function (appControllers) {

    "use strict";

    modifiedCustomerRoutePreviewAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_Routing_ModuleConfig'];

    function modifiedCustomerRoutePreviewAPIService(BaseAPIService, UtilsService, WhS_Routing_ModuleConfig) {

        var controllerName = "ModifiedCustomerRoutePreview";

        function GetAllModifiedCustomerRoutes(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_Routing_ModuleConfig.moduleName, controllerName, "GetAllModifiedCustomerRoutes"), input);
        };

        return ({
            GetAllModifiedCustomerRoutes: GetAllModifiedCustomerRoutes
        });
    }

    appControllers.service('WhS_Routing_ModifiedCustomerRoutePreviewAPIService', modifiedCustomerRoutePreviewAPIService);
})(appControllers);