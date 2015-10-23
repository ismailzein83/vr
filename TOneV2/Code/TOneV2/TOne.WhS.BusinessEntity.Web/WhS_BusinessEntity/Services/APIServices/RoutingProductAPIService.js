(function (appControllers) {

    "use strict";
    routingProductAPIService.$inject = ['BaseAPIService'];

    function routingProductAPIService(BaseAPIService) {

        function GetFilteredRoutingProducts(input) {
            return BaseAPIService.post("/api/RoutingProduct/GetFilteredRoutingProducts", input);
        }

        function GetRoutingProducts() {
            return BaseAPIService.get("/api/RoutingProduct/GetRoutingProducts");
        }
        function GetRoutingProductsInfoBySellingNumberPlan(sellingNumberPlan) {
            return BaseAPIService.get("/api/RoutingProduct/GetRoutingProductsInfoBySellingNumberPlan", { sellingNumberPlan: sellingNumberPlan });
        }
        function GetRoutingProduct(routingProductId) {
            return BaseAPIService.get("/api/RoutingProduct/GetRoutingProduct", {
                routingProductId: routingProductId
            });
        }

        function AddRoutingProduct(routingProductObject) {
            return BaseAPIService.post("/api/RoutingProduct/AddRoutingProduct", routingProductObject);
        }

        function UpdateRoutingProduct(routingProductObject) {
            return BaseAPIService.post("/api/RoutingProduct/UpdateRoutingProduct", routingProductObject);
        }

        function DeleteRoutingProduct(routingProductId) {
            return BaseAPIService.get("/api/RoutingProduct/DeleteRoutingProduct", { routingProductId: routingProductId });
        }


        return ({
            GetFilteredRoutingProducts: GetFilteredRoutingProducts,
            GetRoutingProducts: GetRoutingProducts,
            GetRoutingProduct: GetRoutingProduct,
            AddRoutingProduct: AddRoutingProduct,
            UpdateRoutingProduct: UpdateRoutingProduct,
            DeleteRoutingProduct: DeleteRoutingProduct,
            GetRoutingProductsInfoBySellingNumberPlan: GetRoutingProductsInfoBySellingNumberPlan
        });
    }

    appControllers.service('WhS_BE_RoutingProductAPIService', routingProductAPIService);
})(appControllers);