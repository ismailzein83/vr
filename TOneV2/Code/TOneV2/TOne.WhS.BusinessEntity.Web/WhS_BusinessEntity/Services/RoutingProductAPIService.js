
app.service('RoutingProductAPIService', function (BaseAPIService) {

    return ({

        GetFilteredRoutingProducts: GetFilteredRoutingProducts,
        AddRoutingProduct: AddRoutingProduct,
        UpdateRoutingProduct: UpdateRoutingProduct
    });


    function GetFilteredRoutingProducts() {
        return BaseAPIService.post("/api/RoutingProduct/GetFilteredRoutingProducts");
    }

    function AddRoutingProduct(routingProductObject) {
        return BaseAPIService.post("/api/RoutingProduct/AddRoutingProduct", routingProductObject);
    }

    function UpdateRoutingProduct(routingProductObject) {
        return BaseAPIService.post("/api/RoutingProduct/UpdateRoutingProduct", routingProductObject);
    }

});