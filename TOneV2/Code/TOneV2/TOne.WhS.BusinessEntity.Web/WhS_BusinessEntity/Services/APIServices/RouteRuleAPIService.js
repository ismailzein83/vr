
app.service('WhS_BE_RouteRuleAPIService', function (BaseAPIService) {

    return ({
        GetFilteredRouteRules: GetFilteredRouteRules,
        DeleteRouteRule: DeleteRouteRule,
        GetRouteRule: GetRouteRule
    });


    function GetFilteredRouteRules(input) {
        return BaseAPIService.post("/api/RouteRule/GetFilteredRouteRules", input);
    }

    function GetRouteRule(routeRuleId) {
        return BaseAPIService.get("/api/RoutingProduct/GetRouteRule", {
            routeRuleId: routeRuleId
        });
    }

    //function AddRoutingProduct(routingProductObject) {
    //    return BaseAPIService.post("/api/RoutingProduct/AddRoutingProduct", routingProductObject);
    //}

    //function UpdateRoutingProduct(routingProductObject) {
    //    return BaseAPIService.post("/api/RoutingProduct/UpdateRoutingProduct", routingProductObject);
    //}

    //function GetSaleZoneGroupTemplates() {
    //    return BaseAPIService.get("/api/RoutingProduct/GetSaleZoneGroupTemplates");
    //}

    //function GetSupplierGroupTemplates() {
    //    return BaseAPIService.get("/api/RoutingProduct/GetSupplierGroupTemplates");
    //}

    function DeleteRouteRule(routeRuleId) {
        return BaseAPIService.get("/api/RouteRule/DeleteRouteRule", { routeRuleId: routeRuleId });
    }
});