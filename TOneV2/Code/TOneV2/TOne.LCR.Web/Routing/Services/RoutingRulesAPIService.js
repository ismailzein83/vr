'use strict'
var RoutingRulesAPIService = function (BaseAPIService) {
    return ({
        getRouteRuleDetails: getRouteRuleDetails,
        InsertRouteRule: InsertRouteRule,
        GetFilteredRouteRules: GetFilteredRouteRules,
        UpdateRouteRule: UpdateRouteRule
    });
    function getRouteRuleDetails(RouteRuleId) {
        return BaseAPIService.get("/api/RouteRules/GetRouteRuleDetails",
            {
                RouteRuleId: RouteRuleId
            });
    }
    function InsertRouteRule(routeRule) {

        return BaseAPIService.post("/api/RouteRules/InsertRouteRule", routeRule);
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