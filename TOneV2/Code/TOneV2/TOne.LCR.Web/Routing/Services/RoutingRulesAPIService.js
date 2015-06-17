'use strict'
var RoutingRulesAPIService = function (BaseAPIService) {
    return ({
        getRouteRuleDetails: getRouteRuleDetails,
        SaveRouteRule: SaveRouteRule,
        GetFilteredRouteRules: GetFilteredRouteRules,
        UpdateRouteRule: UpdateRouteRule
    });
    function getRouteRuleDetails(RouteRuleId) {
        return BaseAPIService.get("/api/RouteRules/GetRouteRuleDetails",
            {
                RouteRuleId: RouteRuleId
            });
    }
    function SaveRouteRule(routeRule) {

        return BaseAPIService.post("/api/RouteRules/SaveRouteRule", routeRule);
    }
    function UpdateRouteRule(routeRule) {
        return BaseAPIService.post("/api/RouteRules/UpdateRouteRule", routeRule);
    }
    function GetFilteredRouteRules(input) {
        return BaseAPIService.post("/api/RouteRules/GetFilteredRouteRules", input);
    }

}
RoutingRulesAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingRulesAPIService', RoutingRulesAPIService);