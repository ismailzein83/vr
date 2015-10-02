(function (appControllers) {

    "use strict";
    routeRuleAPIService.$inject = ['BaseAPIService'];

    function routeRuleAPIService(BaseAPIService) {

        function GetFilteredRouteRules(input) {
            return BaseAPIService.post("/api/RouteRule/GetFilteredRouteRules", input);
        }

        function GetRouteRule(routeRuleId) {
            return BaseAPIService.get("/api/RouteRule/GetRouteRule", {
                routeRuleId: routeRuleId
            });
        }

        function AddRouteRule(routeRuleObject) {
            return BaseAPIService.post("/api/RouteRule/AddRouteRule", routeRuleObject);
        }

        function UpdateRouteRule(routeRuleObject) {
            return BaseAPIService.post("/api/RouteRule/UpdateRouteRule", routeRuleObject);
        }

        //function GetSaleZoneGroupTemplates() {
        //    return BaseAPIService.get("/api/RoutingProduct/GetSaleZoneGroupTemplates");
        //}

        //function GetSupplierGroupTemplates() {
        //    return BaseAPIService.get("/api/RoutingProduct/GetSupplierGroupTemplates");
        //}

        function DeleteRouteRule(routeRuleId) {
            return BaseAPIService.get("/api/RouteRule/DeleteRouteRule", { routeRuleId: routeRuleId });
        }

        return ({
            GetFilteredRouteRules: GetFilteredRouteRules,
            AddRouteRule: AddRouteRule,
            UpdateRouteRule: UpdateRouteRule,
            DeleteRouteRule: DeleteRouteRule,
            GetRouteRule: GetRouteRule
        });
    }

    appControllers.service('WhS_BE_RouteRuleAPIService', routeRuleAPIService);
})(appControllers);