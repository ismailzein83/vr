
app.service('WhS_BE_RoutingProductAPIService', function (BaseAPIService) {

    return ({

        GetFilteredRoutingProducts: GetFilteredRoutingProducts,
        AddRoutingProduct: AddRoutingProduct,
        UpdateRoutingProduct: UpdateRoutingProduct,
        GetSaleZoneGroupTemplates: GetSaleZoneGroupTemplates,
        GetSupplierGroupTemplates: GetSupplierGroupTemplates
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

    function GetSaleZoneGroupTemplates() {
        return BaseAPIService.get("/api/RoutingProduct/GetSaleZoneGroupTemplates");
    }

    function GetSupplierGroupTemplates() {
        return BaseAPIService.get("/api/RoutingProduct/GetSupplierGroupTemplates");
    }
});