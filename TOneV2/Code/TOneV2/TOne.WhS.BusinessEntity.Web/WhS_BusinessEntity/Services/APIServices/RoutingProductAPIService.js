(function (appControllers) {

    "use strict";

    routingProductAPIService.$inject = ['BaseAPIService', 'UtilsService', 'WhS_BE_ModuleConfig'];

    function routingProductAPIService(BaseAPIService, UtilsService, WhS_BE_ModuleConfig) {
        return ({
            GetFilteredRoutingProducts: GetFilteredRoutingProducts,
            GetRoutingProductInfo: GetRoutingProductInfo,
            GetRoutingProduct: GetRoutingProduct,
            GetRoutingProductsInfoBySellingNumberPlan: GetRoutingProductsInfoBySellingNumberPlan,
            AddRoutingProduct: AddRoutingProduct,
            UpdateRoutingProduct: UpdateRoutingProduct,
            DeleteRoutingProduct: DeleteRoutingProduct
        });

        function GetFilteredRoutingProducts(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'GetFilteredRoutingProducts'), input);
        }

        function GetRoutingProductInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'GetRoutingProductInfo'), {
                filter: filter
            });
        }

        function GetRoutingProduct(routingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'GetRoutingProduct'), {
                routingProductId: routingProductId
            });
        }

        function GetRoutingProductsInfoBySellingNumberPlan(sellingNumberPlan) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'GetRoutingProductsInfoBySellingNumberPlan'), {
                sellingNumberPlan: sellingNumberPlan
            });
        }

        function AddRoutingProduct(routingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'AddRoutingProduct'), routingProductObject);
        }

        function UpdateRoutingProduct(routingProductObject) {
            return BaseAPIService.post(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'UpdateRoutingProduct'), routingProductObject);
        }

        function DeleteRoutingProduct(routingProductId) {
            return BaseAPIService.get(UtilsService.getServiceURL(WhS_BE_ModuleConfig.moduleName, 'RoutingProduct', 'DeleteRoutingProduct'), {
                routingProductId: routingProductId
            });
        }
    }

    appControllers.service('WhS_BE_RoutingProductAPIService', routingProductAPIService);

})(appControllers);
