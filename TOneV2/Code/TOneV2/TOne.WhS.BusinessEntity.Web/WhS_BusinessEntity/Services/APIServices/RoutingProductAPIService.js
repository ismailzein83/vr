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
        function GetRoutingProductsInfoBySaleZonePackage(saleZonePackage) {
            return BaseAPIService.get("/api/RoutingProduct/GetRoutingProductsInfoBySaleZonePackage", { saleZonePackage: saleZonePackage });
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
            GetRoutingProductsInfoBySaleZonePackage: GetRoutingProductsInfoBySaleZonePackage
        });
    }

    appControllers.service('WhS_BE_RoutingProductAPIService', routingProductAPIService);
})(appControllers);