
app.service('WhS_BE_RoutingProductAPIService', function (BaseAPIService) {

    return ({

        GetFilteredRoutingProducts: GetFilteredRoutingProducts,
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

    function GetSaleZoneGroupTemplates() {
        return BaseAPIService.get("/api/RoutingProduct/GetSaleZoneGroupTemplates");
    }

    function GetSupplierGroupTemplates() {
        return BaseAPIService.get("/api/RoutingProduct/GetSupplierGroupTemplates");
    }

    function DeleteRoutingProduct(routingProductId) {
        return BaseAPIService.get("/api/RoutingProduct/DeleteRoutingProduct", { routingProductId: routingProductId });
    }
});