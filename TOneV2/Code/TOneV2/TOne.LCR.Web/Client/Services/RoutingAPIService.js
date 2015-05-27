'use strict'
var RoutingAPIService = function (BaseAPIService) {

    return ({
        getRouteRuleDetails: getRouteRuleDetails,
        saveRouteRule: saveRouteRule,
        GetFilteredRouteRules: GetFilteredRouteRules,
        GetRoutes: GetRoutes
    });
    function getRouteRuleDetails(RouteRuleId) {
        return BaseAPIService.get("/api/routing/GetRouteRuleDetails",
            {
                RouteRuleId: RouteRuleId
            });
    }
    function GetFilteredRouteRules(filter) {
        return BaseAPIService.post("/api/routing/GetFilteredRouteRules", filter);
    }
    function saveRouteRule(routeRule) {

        return BaseAPIService.post("/api/routing/SaveRouteRule", routeRule);
    }

    function GetFilteredRoutes(filter) {
        return BaseAPIService.post("/api/routing/GetRoutes", filter);
    }

}
RoutingAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingAPIService', RoutingAPIService);
