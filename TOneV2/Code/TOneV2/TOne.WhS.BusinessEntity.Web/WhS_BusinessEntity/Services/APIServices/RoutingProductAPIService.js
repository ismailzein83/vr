
app.service('WhS_BE_RoutingProductAPIService', function (BaseAPIService) {

    return ({

        GetFilteredRoutingProducts: GetFilteredRoutingProducts,
        GetRoutingProducts:GetRoutingProducts,
        GetRoutingProduct: GetRoutingProduct,
        AddRoutingProduct: AddRoutingProduct,
        UpdateRoutingProduct: UpdateRoutingProduct,
        GetSaleZoneGroupTemplates: GetSaleZoneGroupTemplates,
        GetSupplierGroupTemplates: GetSupplierGroupTemplates,
        DeleteRoutingProduct: DeleteRoutingProduct
    });


    function GetFilteredRoutingProducts(input) {
        return BaseAPIService.post("/api/RoutingProduct/GetFilteredRoutingProducts", input);
    }

    function GetRoutingProducts() {
        return BaseAPIService.get("/api/RoutingProduct/GetRoutingProducts");
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

    function GetSupplierGroupTemplates() {
        return BaseAPIService.get("/api/RoutingProduct/GetSupplierGroupTemplates");
    }

    function DeleteRoutingProduct(routingProductId) {
        return BaseAPIService.get("/api/RoutingProduct/DeleteRoutingProduct", { routingProductId: routingProductId });
    }
});