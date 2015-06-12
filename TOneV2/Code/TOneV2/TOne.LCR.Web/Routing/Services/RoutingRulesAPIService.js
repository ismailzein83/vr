'use strict'
var RoutingRulesAPIService = function (BaseAPIService) {
    return ({
        getRouteRuleDetails: getRouteRuleDetails,
        saveRouteRule: saveRouteRule,
        GetFilteredRouteRules: GetFilteredRouteRules
    });
    function getRouteRuleDetails(RouteRuleId) {
        return BaseAPIService.get("/api/routing/GetRouteRuleDetails",
            {
                RouteRuleId: RouteRuleId
            });
    }
    function saveRouteRule(routeRule) {

        return BaseAPIService.post("/api/routing/SaveRouteRule", routeRule);
    }
    function GetFilteredRouteRules(filter) {
        return BaseAPIService.post("/api/routing/GetFilteredRouteRules", filter);
    }
}
RoutingRulesAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingRulesAPIService', RoutingRulesAPIService);