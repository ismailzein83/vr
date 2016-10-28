(function (appControllers) {

    "use strict";

    routingProductAPIService.$inject = ["BaseAPIService", "UtilsService", "WhS_BE_ModuleConfig", "SecurityService"];

    function routingProductAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig, SecurityService) {
        var controllerName = "RoutingProduct";

        function GetFilteredRoutingProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetFilteredRoutingProducts"), input);
        }

        function GetRoutingProductInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetRoutingProductInfo"), {
                filter: filter
            });
        }

        function GetRoutingProduct(routingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetRoutingProduct"), {
                routingProductId: routingProductId
            });
        }

        function GetRoutingProductEditorRuntime(routingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetRoutingProductEditorRuntime"), {
                routingProductId: routingProductId
            });
        }

        function GetRoutingProductsInfoBySellingNumberPlan(sellingNumberPlan) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "GetRoutingProductsInfoBySellingNumberPlan"), {
                sellingNumberPlan: sellingNumberPlan
            });
        }

        function AddRoutingProduct(routingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "AddRoutingProduct"), routingProductObject);
        }

        function UpdateRoutingProduct(routingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "UpdateRoutingProduct"), routingProductObject);
        }

        function DeleteRoutingProduct(routingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, controllerName, "DeleteRoutingProduct"), {
                routingProductId: routingProductId
            });
        }


        function HasUpdateRoutingProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['UpdateRoutingProduct']));
        }

        function HasAddRoutingProductPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(WhS_BE_ModuleConfig.moduleName, controllerName, ['AddRoutingProduct']));
        }


        return ({
            GetFilteredRoutingProducts: GetFilteredRoutingProducts,
            GetRoutingProductInfo: GetRoutingProductInfo,
            GetRoutingProduct: GetRoutingProduct,
            GetRoutingProductEditorRuntime: GetRoutingProductEditorRuntime,
            GetRoutingProductsInfoBySellingNumberPlan: GetRoutingProductsInfoBySellingNumberPlan,
            AddRoutingProduct: AddRoutingProduct,
            UpdateRoutingProduct: UpdateRoutingProduct,
            DeleteRoutingProduct: DeleteRoutingProduct,
            HasUpdateRoutingProductPermission: HasUpdateRoutingProductPermission,
            HasAddRoutingProductPermission: HasAddRoutingProductPermission
        });

    }

    appControllers.service("WhS_BE_RoutingProductAPIService", routingProductAPIService);

})(appControllers);
