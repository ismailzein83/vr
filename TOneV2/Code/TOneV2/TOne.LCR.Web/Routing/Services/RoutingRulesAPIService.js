
var RoutingRulesAPIService = function (BaseAPIService) {
    'use strict';
    return ({
        getRouteRuleDetails: getRouteRuleDetails,
        InsertRouteRule: InsertRouteRule,
        GetFilteredRouteRules: GetFilteredRouteRules,
        UpdateRouteRule: UpdateRouteRule,
        DeleteRouteRule: DeleteRouteRule
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

    function DeleteRouteRule(routeRule) {
        return BaseAPIService.post("/api/RouteRules/DeleteRouteRule", routeRule);
    }

}
RoutingRulesAPIService.$inject = ['BaseAPIService'];
appControllers.service('RoutingRulesAPIService', RoutingRulesAPIService);